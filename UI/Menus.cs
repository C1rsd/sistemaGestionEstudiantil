using SistemaGestionEstudiantil.Excepciones;
using SistemaGestionEstudiantil.Modelos;
using SistemaGestionEstudiantil.Servicios;

namespace SistemaGestionEstudiantil.UI;

/// <summary>
/// Interfaz de usuario por consola: menús por rol y flujos completos del
/// enunciado (inscripción, notas, convocatoria, asistencia, contratos,
/// planilla y pagos). Toda excepción de negocio se captura y se muestra
/// en rojo sin interrumpir la aplicación.
/// </summary>
public class Menus
{
    private readonly SistemaGestion _sistema;
    private readonly Sesion _sesion = new();

    public Menus(SistemaGestion sistema)
    {
        _sistema = sistema;
    }

    // =====================================================================
    //  Bucle principal e inicio de sesión
    // =====================================================================
    public void Ejecutar()
    {
        try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { /* consolas sin UTF-8 */ }

        while (true)
        {
            if (!_sesion.Activa)
            {
                MenuLogin();
                continue;
            }

            switch (_sesion.Usuario.Rol)
            {
                case Rol.Administrador: MenuAdministrador(); break;
                case Rol.Profesor: MenuProfesor(); break;
                default: MenuEstudiante(); break;
            }
        }
    }

    private void MenuLogin()
    {
        Consola.Titulo("SISTEMA DE GESTIÓN ESTUDIANTIL — UNIVERSIDADES DE NICARAGUA");
        Consola.Linea($"Periodo académico actual: {_sistema.PeriodoActual}");
        Consola.Linea("Usuarios de prueba: admin / Admin#2026  ·  ddonaire / David#2026  ·  cmartinez / Clave#2026");
        Consola.Linea("(la lista completa de cuentas está en el README.md)");
        Consola.Linea();
        Consola.Linea("1. Iniciar sesión");
        Consola.Linea("2. Salir del sistema");
        int op = Consola.LeerOpcion("Seleccione una opción: ", 1, 2);
        if (op == 2)
        {
            Consola.Exito("Gracias por usar el sistema. ¡Hasta pronto!");
            Environment.Exit(0);
        }

        string usuario = Consola.LeerTexto("Usuario: ");
        string clave = Consola.LeerTexto("Contraseña: ");
        try
        {
            Persona p = _sistema.Login(usuario, clave);
            _sesion.Iniciar(p);
            Consola.Exito($"Bienvenido(a): {p.Nombre}  [Rol: {p.Rol}]");
        }
        catch (AppException ex)
        {
            Consola.Error(ex.Message);
        }
        Consola.Pausa();
    }

    // =====================================================================
    //  MÓDULO ESTUDIANTE
    // =====================================================================
    private void MenuEstudiante()
    {
        Estudiante est = (Estudiante)_sesion.Usuario;

        while (true)
        {
            Consola.Titulo("MÓDULO ESTUDIANTE");
            Consola.Linea($"Estudiante: {est.Nombre} ({est.Cedula})");
            Consola.Linea($"Carrera: {_sistema.NombreCarrera(est.CarreraId)}");
            Consola.Linea("1. Inscribir clases");
            Consola.Linea("2. Ver mis notas");
            Consola.Linea("3. Ver mi historial académico (cursos aprobados)");
            Consola.Linea("4. Editar mi perfil");
            Consola.Linea("5. Cerrar sesión");
            int op = Consola.LeerOpcion("Opción: ", 1, 5);
            if (op == 5) { _sesion.Cerrar(); return; }

            try
            {
                switch (op)
                {
                    case 1: InscribirClases(est); break;
                    case 2: VerMisNotas(est); break;
                    case 3: HistorialAcademico(est); break;
                    case 4: EditarPerfil(est); break;
                }
            }
            catch (AppException ex) { Consola.Error(ex.Message); }
            Consola.Pausa();
        }
    }

    private void InscribirClases(Estudiante est)
    {
        // Paso 1: cursos disponibles de la carrera del estudiante.
        List<Curso> cursos = _sistema.Cursos
            .Where(c => c.CarreraId == est.CarreraId && c.Estado == 1)
            .OrderBy(c => c.Codigo)
            .ToList();
        if (cursos.Count == 0)
            throw new DatoInvalidoException("No hay cursos disponibles para su carrera.");

        Consola.Subtitulo("CURSOS DISPONIBLES DE SU CARRERA");
        foreach (Curso c in cursos)
        {
            string pre = c.Prerrequisitos.Count == 0
                ? "ninguno"
                : string.Join(", ", c.Prerrequisitos);
            Consola.Linea($"  {c.Codigo,-10}{Consola.Recortar(c.Nombre, 38),-38}Prerrequisitos: {pre}");
        }

        string codigo = Consola.LeerTexto("Código del curso que desea inscribir: ").ToUpper();
        Curso curso = cursos.FirstOrDefault(c => c.Codigo == codigo)
            ?? throw new DatoInvalidoException("El código ingresado no corresponde a un curso disponible de su carrera.");

        // Paso 2: se muestran los prerrequisitos y si están aprobados o pendientes.
        Consola.Subtitulo($"PRERREQUISITOS DE {curso.Nombre}");
        HashSet<string> aprobados = _sistema.CursosAprobados(est.Cedula);
        if (curso.Prerrequisitos.Count == 0)
            Consola.Linea("  Este curso no tiene prerrequisitos.");
        foreach (string pre in curso.Prerrequisitos)
        {
            Curso cpre = _sistema.BuscarCurso(pre);
            string estadoPre = aprobados.Contains(pre) ? "APROBADO" : "PENDIENTE";
            Consola.Linea($"  {pre,-10}{Consola.Recortar(cpre?.Nombre ?? pre, 38),-38}{estadoPre}");
        }

        // Paso 3: si falta un prerrequisito se bloquea la inscripción (excepción).
        _sistema.VerificarPrerrequisitos(est, curso);

        // Paso 4-5: grupos activos con cupo en el periodo actual.
        List<Grupo> grupos = _sistema.Grupos
            .Where(g => g.CursoCodigo == curso.Codigo
                     && g.Periodo == _sistema.PeriodoActual
                     && g.Estado == 1)
            .ToList();
        if (grupos.Count == 0)
            throw new DatoInvalidoException($"No hay grupos activos para este curso en el periodo {_sistema.PeriodoActual}.");

        Consola.Subtitulo("GRUPOS DISPONIBLES");
        foreach (Grupo g in grupos)
        {
            string docente = g.ProfesorCedula == null
                ? "Sin asignar"
                : _sistema.BuscarProfesor(g.ProfesorCedula).Nombre;
            Consola.Linea($"  {g.Codigo,-11}{g.Modalidad,-10}{g.Dia,-10}" +
                          $"{Consola.Hora(g.HoraInicio)}-{Consola.Hora(g.HoraFin)}  " +
                          $"Docente: {Consola.Recortar(docente, 24),-24} Cupo: {g.CupoDisponible}/{g.Cupo}");
        }

        string gcod = Consola.LeerTexto("Código del grupo: ").ToUpper();
        Grupo grupo = grupos.FirstOrDefault(g => g.Codigo == gcod)
            ?? throw new DatoInvalidoException("El grupo ingresado no existe o no está activo.");

        // Validaciones de negocio (ya aprobado, ya inscrito en el periodo, horario, cupo).
        _sistema.VerificarInscripcion(est, grupo);

        // Paso 6: confirmación, guardado y comprobante.
        string nombreDocente = grupo.ProfesorCedula == null
            ? "Sin asignar"
            : _sistema.BuscarProfesor(grupo.ProfesorCedula).Nombre;
        Consola.Subtitulo("CONFIRMACIÓN DE INSCRIPCIÓN");
        Consola.Linea($"  Estudiante : {est.Nombre} ({est.Cedula})");
        Consola.Linea($"  Curso      : {curso.Nombre} ({curso.Codigo})");
        Consola.Linea($"  Grupo      : {grupo.Codigo} — {grupo.Modalidad}");
        Consola.Linea($"  Horario    : {grupo.HorarioTexto}");
        Consola.Linea($"  Docente    : {nombreDocente}");
        Consola.Linea($"  Periodo    : {grupo.Periodo}");
        if (!Consola.Confirmar("¿Confirma la inscripción?"))
        {
            Consola.Linea("Inscripción cancelada.");
            return;
        }

        Inscripcion insc = _sistema.Inscribir(est, grupo);

        Consola.Subtitulo("COMPROBANTE DE INSCRIPCIÓN");
        Consola.Linea($"  Estudiante : {est.Nombre} ({est.Cedula})");
        Consola.Linea($"  Curso      : {curso.Nombre} ({curso.Codigo})");
        Consola.Linea($"  Grupo      : {grupo.Codigo} — {grupo.Modalidad}");
        Consola.Linea($"  Horario    : {grupo.HorarioTexto}");
        Consola.Linea($"  Docente    : {nombreDocente}");
        Consola.Linea($"  Periodo    : {insc.Periodo}    Fecha: {insc.Fecha:dd/MM/yyyy}");
        Consola.Linea($"  No. de inscripción: {insc.Id}");
        Consola.Exito("Inscripción registrada correctamente.");
    }

    private void VerMisNotas(Estudiante est)
    {
        Consola.Subtitulo("MIS NOTAS");
        List<Inscripcion> inscripciones = _sistema.InscripcionesDeEstudiante(est.Cedula);
        if (inscripciones.Count == 0)
        {
            Consola.Linea("Aún no tiene inscripciones registradas.");
            return;
        }

        int aprobadas = 0, reprobadas = 0;
        foreach (Inscripcion insc in inscripciones)
        {
            Grupo g = _sistema.BuscarGrupo(insc.GrupoCodigo);
            if (g == null) continue;
            Curso c = _sistema.BuscarCurso(g.CursoCodigo);
            string docente = g.ProfesorCedula == null ? "Sin asignar" : _sistema.BuscarProfesor(g.ProfesorCedula).Nombre;

            Consola.Subtitulo($"{c.Nombre} ({c.Codigo}) — Grupo {g.Codigo} — Periodo {insc.Periodo}");
            Consola.Linea($"  Docente: {docente}   Horario: {g.HorarioTexto}");

            Nota nota = _sistema.NotaDe(insc);
            if (nota == null)
            {
                Consola.Linea("  Estado: Nota pendiente");
                continue;
            }

            Consola.Linea($"  Acumulado: {nota.Acumulado}/60    Examen: {nota.Examen}/40    " +
                          $"Nota final: {nota.Acumulado} + {nota.Examen} = {nota.Final}");
            Consola.Linea($"  Estado: {nota.EstadoFinal}");
            if (nota.TieneConvocatoria)
                Consola.Linea($"  Convocatoria: extraordinaria {nota.NotaExtraordinaria}/100 → " +
                              $"tope de 80 aplicado: {nota.NotaAplicada} → {nota.EstadoFinal}");

            if (nota.Aprobado) aprobadas++; else reprobadas++;
        }

        Consola.Linea();
        Consola.Linea($"RESUMEN: clases aprobadas: {aprobadas}  |  clases reprobadas: {reprobadas}");
    }

    private void HistorialAcademico(Estudiante est)
    {
        Consola.Subtitulo("HISTORIAL ACADÉMICO — CURSOS APROBADOS");
        HashSet<string> aprobados = _sistema.CursosAprobados(est.Cedula);
        if (aprobados.Count == 0)
        {
            Consola.Linea("Aún no tiene cursos aprobados.");
            return;
        }
        foreach (string cod in aprobados.OrderBy(x => x))
        {
            Curso c = _sistema.BuscarCurso(cod);
            Consola.Linea($"  {cod,-10}{c?.Nombre ?? "(curso no encontrado)"}");
        }
        Consola.Linea($"Total de cursos aprobados: {aprobados.Count}");
    }

    private void EditarPerfil(Persona p)
    {
        Consola.Subtitulo("EDITAR MI PERFIL — " + p.Nombre);
        Consola.Linea("1. Cambiar datos de contacto (celular, correo, dirección)");
        if (p is Estudiante) Consola.Linea("2. Cambiar contraseña");
        if (p is Profesor)
        {
            Consola.Linea("2. Cambiar datos profesionales (estado civil, profesión, especialidad)");
            Consola.Linea("3. Cambiar contraseña");
        }
        Consola.Linea("0. Volver");
        int op = Consola.LeerOpcion("Opción: ", 0, 3);
        switch (op)
        {
            case 1:
                string celular = Consola.LeerTexto("Nuevo celular (8 dígitos): ");
                string correo = Consola.LeerTexto("Nuevo correo: ");
                string dir = Consola.LeerTexto("Nueva dirección: ");
                _sistema.ActualizarDatosContacto(p, celular, correo, dir);
                Consola.Exito("Datos de contacto actualizados.");
                break;
            case 2 when p is Estudiante:
                string clave1 = Consola.LeerTexto("Nueva contraseña: ");
                _sistema.ActualizarContrasena(p, clave1);
                Consola.Exito("Contraseña actualizada.");
                break;
            case 2 when p is Profesor prof:
                string ec = Consola.LeerTexto("Estado civil: ");
                string pro = Consola.LeerTexto("Profesión: ");
                string esp = Consola.LeerTexto("Especialidad (Enter para dejarla vacía): ", obligatorio: false);
                _sistema.ActualizarDatosProfesionales(prof, ec, pro, esp);
                Consola.Exito("Datos profesionales actualizados.");
                break;
            case 3 when p is Profesor:
                string clave3 = Consola.LeerTexto("Nueva contraseña: ");
                _sistema.ActualizarContrasena(p, clave3);
                Consola.Exito("Contraseña actualizada.");
                break;
        }
    }

    // =====================================================================
    //  MÓDULO PROFESOR
    // =====================================================================
    private void MenuProfesor()
    {
        Profesor prof = (Profesor)_sesion.Usuario;

        while (true)
        {
            bool hayConvocatoria = _sistema.HayEstudiantesConDerechoConvocatoria(prof);

            Consola.Titulo("MÓDULO PROFESOR");
            Consola.Linea($"Profesor: {prof.Nombre} — {prof.Profesion}");
            Consola.Linea("1. Ver mis grupos y sus estudiantes");
            Consola.Linea("2. Registrar notas");
            Consola.Linea("3. Ver notas de un grupo (solo lectura)");
            Consola.Linea(hayConvocatoria
                ? "4. Registrar nota extraordinaria (convocatoria)  [HABILITADA]"
                : "4. Registrar nota extraordinaria (convocatoria)  [deshabilitada: nadie tiene derecho]");
            Consola.Linea("5. Marcar asistencia a clase");
            Consola.Linea("6. Ver mi contrato");
            Consola.Linea("7. Ver mi planilla de pago");
            Consola.Linea("8. Editar mi perfil");
            Consola.Linea("9. Cerrar sesión");
            int op = Consola.LeerOpcion("Opción: ", 1, 9);
            if (op == 9) { _sesion.Cerrar(); return; }

            try
            {
                switch (op)
                {
                    case 1: VerMisGruposYEstudiantes(prof); break;
                    case 2: RegistrarNotas(prof); break;
                    case 3: VerNotasGrupo(prof); break;
                    case 4:
                        if (!hayConvocatoria)
                            throw new AccesoDenegadoException(
                                "La opción está deshabilitada: no hay estudiantes con derecho a convocatoria.");
                        RegistrarConvocatoria(prof); break;
                    case 5: MarcarAsistencia(prof); break;
                    case 6: VerContrato(prof); break;
                    case 7: VerPlanillaProfesor(prof); break;
                    case 8: EditarPerfil(prof); break;
                }
            }
            catch (AppException ex) { Consola.Error(ex.Message); }
            Consola.Pausa();
        }
    }

    private Grupo ElegirGrupoDelProfesor(Profesor prof)
    {
        List<Grupo> grupos = _sistema.GruposDeProfesor(prof.Cedula);
        if (grupos.Count == 0)
            throw new DatoInvalidoException("No tiene grupos asignados en el periodo actual.");

        Consola.Subtitulo("MIS GRUPOS");
        for (int i = 0; i < grupos.Count; i++)
        {
            Grupo g = grupos[i];
            Curso c = _sistema.BuscarCurso(g.CursoCodigo);
            Consola.Linea($"  {i + 1}. {g.Codigo,-11}{Consola.Recortar(c?.Nombre ?? "?", 30),-30}" +
                          $"{g.Dia,-10}{Consola.Hora(g.HoraInicio)}-{Consola.Hora(g.HoraFin)} ({g.Modalidad})");
        }
        int idx = Consola.LeerOpcion("Elija un grupo: ", 1, grupos.Count);
        return grupos[idx - 1];
    }

    private void VerMisGruposYEstudiantes(Profesor prof)
    {
        Grupo grupo = ElegirGrupoDelProfesor(prof);
        Consola.Subtitulo($"ESTUDIANTES DEL GRUPO {grupo.Codigo}");
        List<(Inscripcion insc, Estudiante est)> lista = _sistema.InscripcionesDelGrupo(grupo);
        if (lista.Count == 0)
        {
            Consola.Linea("El grupo todavía no tiene estudiantes inscritos.");
            return;
        }
        foreach ((Inscripcion insc, Estudiante est) in lista)
        {
            Consola.Linea($"  {est.Cedula,-18}{Consola.Recortar(est.Nombre, 34),-34}" +
                          $"Inscrito el {insc.Fecha:dd/MM/yyyy}  [{est.EstadoTexto}]");
        }
        Consola.Linea($"Total: {lista.Count} estudiante(s).");
    }

    private void RegistrarNotas(Profesor prof)
    {
        Grupo grupo = ElegirGrupoDelProfesor(prof);

        // Validación de la fecha tope definida por el administrador.
        _sistema.ValidarFechaTope();

        // Estudiantes inscritos que todavía no tienen nota.
        List<(Inscripcion insc, Estudiante est)> sinNota = _sistema.InscripcionesDelGrupo(grupo)
            .Where(par => _sistema.NotaDe(par.insc) == null)
            .ToList();
        if (sinNota.Count == 0)
        {
            Consola.Linea("Todos los estudiantes del grupo ya tienen nota registrada.");
            return;
        }

        Consola.Subtitulo($"ESTUDIANTES SIN NOTA — GRUPO {grupo.Codigo}");
        foreach ((Inscripcion _, Estudiante est) in sinNota)
            Consola.Linea($"  {est.Cedula,-18}{est.Nombre}");

        List<(Inscripcion insc, decimal ac, decimal ex)> nuevas = new();
        foreach ((Inscripcion insc, Estudiante est) in sinNota)
        {
            Consola.Subtitulo("Registrando nota de: " + est.Nombre);
            decimal acumulado = PedirValorEnRango("  Acumulado (0 a 60): ", 0, 60);
            decimal examen = PedirValorEnRango("  Examen (0 a 40): ", 0, 40);

            Nota calculo = new() { Acumulado = acumulado, Examen = examen };
            Consola.Linea($"  Nota final: {acumulado} + {examen} = {calculo.Final}  →  {calculo.EstadoBase}");
            nuevas.Add((insc, acumulado, examen));
        }

        // Advertencia: las notas no se podrán editar.
        Consola.Subtitulo("CONFIRMACIÓN");
        Consola.Linea("ADVERTENCIA: una vez guardadas, las notas NO podrán editarse.");
        if (!Consola.Confirmar("¿Desea guardar las notas del grupo?"))
        {
            Consola.Linea("Operación cancelada. No se guardó ninguna nota.");
            return;
        }

        foreach ((Inscripcion insc, decimal ac, decimal ex) in nuevas)
            _sistema.RegistrarNota(prof, grupo, insc, ac, ex);

        Consola.Exito("Notas guardadas correctamente.");
        MostrarNotasDelGrupo(grupo);
    }

    /// <summary>Pide un valor numérico; lanza excepción y vuelve a pedirlo si es inválido.</summary>
    private decimal PedirValorEnRango(string mensaje, decimal min, decimal max)
    {
        while (true)
        {
            try { return Consola.LeerNota(mensaje, min, max); }
            catch (DatoInvalidoException ex) { Consola.Error(ex.Message); }
        }
    }

    private void RegistrarConvocatoria(Profesor prof)
    {
        Grupo grupo = ElegirGrupoDelProfesor(prof);
        _sistema.ValidarFechaTope();

        List<(Inscripcion insc, Estudiante est)> conDerecho = _sistema.InscripcionesDelGrupo(grupo)
            .Where(par => { Nota n = _sistema.NotaDe(par.insc); return n != null && n.DerechoConvocatoria; })
            .ToList();
        if (conDerecho.Count == 0)
        {
            Consola.Linea("No hay estudiantes con derecho a convocatoria pendientes en este grupo.");
            return;
        }

        Consola.Subtitulo("ESTUDIANTES CON DERECHO A CONVOCATORIA (nota final entre 40 y 59)");
        for (int i = 0; i < conDerecho.Count; i++)
        {
            Nota n = _sistema.NotaDe(conDerecho[i].insc);
            Consola.Linea($"  {i + 1}. {Consola.Recortar(conDerecho[i].est.Nombre, 32),-32}Nota final: {n.Final}");
        }

        int idx = Consola.LeerOpcion("Elija el número del estudiante: ", 1, conDerecho.Count);
        (Inscripcion insc, Estudiante est) elegido = conDerecho[idx - 1];

        decimal extra = PedirValorEnRango("Nota extraordinaria (0 a 100): ", 0, 100);
        Nota nota = _sistema.RegistrarConvocatoria(prof, grupo, elegido.insc, extra);

        Consola.Linea($"  {elegido.est.Nombre}: extraordinaria {nota.NotaExtraordinaria}/100 → " +
                      $"tope de 80 aplicado: {nota.NotaAplicada} → {nota.EstadoFinal}");
        Consola.Exito("Convocatoria registrada. La nota extraordinaria no puede editarse.");
    }

    private void VerNotasGrupo(Profesor prof)
    {
        Grupo grupo = ElegirGrupoDelProfesor(prof);
        MostrarNotasDelGrupo(grupo);
    }

    private void MostrarNotasDelGrupo(Grupo grupo)
    {
        Consola.Subtitulo($"NOTAS DEL GRUPO {grupo.Codigo} (solo lectura)");
        int aprobados = 0, conDerecho = 0, sinDerecho = 0, pendientes = 0;

        foreach ((Inscripcion insc, Estudiante est) in _sistema.InscripcionesDelGrupo(grupo))
        {
            Nota n = _sistema.NotaDe(insc);
            if (n == null)
            {
                Consola.Linea($"  {Consola.Recortar(est.Nombre, 32),-32}{"—",8}{"—",8}{"—",8}  Nota pendiente");
                pendientes++;
                continue;
            }

            Consola.Linea($"  {Consola.Recortar(est.Nombre, 32),-32}{n.Acumulado,8}{n.Examen,8}{n.Final,8}  {n.EstadoFinal}");
            if (n.Aprobado) aprobados++;
            else if (n.DerechoConvocatoria) conDerecho++;
            else sinDerecho++;
        }

        Consola.Linea();
        Consola.Linea($"RESUMEN — Aprobados: {aprobados} | Reprobados con derecho a convocatoria: {conDerecho} | " +
                      $"Reprobados sin derecho: {sinDerecho} | Nota pendiente: {pendientes}");
    }

    private void MarcarAsistencia(Profesor prof)
    {
        Grupo grupo = ElegirGrupoDelProfesor(prof);
        Consola.Subtitulo($"MARCAR ASISTENCIA — GRUPO {grupo.Codigo}");
        Consola.Linea($"Clase: {grupo.HorarioTexto} ({grupo.Modalidad}) — {grupo.HorasPorClase} horas por clase.");
        DateTime fecha = Consola.LeerFecha("Fecha de la clase a marcar");

        Consola.Linea("1. Marcar asistencia (se simula la hora de entrada)");
        Consola.Linea("2. Registrar ausencia (no asistió a la clase)");
        int op = Consola.LeerOpcion("Opción: ", 1, 2);

        if (op == 1)
        {
            TimeSpan entrada = Consola.LeerHora("Hora de entrada simulada (hh:mm): ");
            try
            {
                Asistencia a = _sistema.MarcarAsistencia(prof, grupo, fecha, entrada);
                Consola.Exito($"Asistencia registrada: {a.Fecha:dd/MM/yyyy} a las {Consola.Hora(a.HoraMarcada.Value)} → PRESENTE.");
            }
            catch (AsistenciaFueraDeTiempoException ex)
            {
                // Llegó tarde: la excepción indica que ya no puede marcar.
                Consola.Error(ex.Message);
                if (Consola.Confirmar("¿Desea registrar esta clase como AUSENCIA?"))
                {
                    _sistema.RegistrarAusencia(prof, grupo, fecha);
                    Consola.Exito($"Ausencia registrada: se descontarán C$ {grupo.HorasPorClase * 400} " +
                                  $"({grupo.HorasPorClase} horas × C$ 400) en la planilla.");
                }
            }
        }
        else
        {
            _sistema.RegistrarAusencia(prof, grupo, fecha);
            Consola.Exito($"Ausencia registrada: se descontarán C$ {grupo.HorasPorClase * 400} " +
                          $"({grupo.HorasPorClase} horas × C$ 400) en la planilla.");
        }
    }

    private void VerContrato(Profesor prof)
    {
        Consola.Subtitulo($"MI CONTRATO — PERIODO {_sistema.PeriodoActual}");
        List<Contrato> contratos = _sistema.Contratos
            .Where(c => c.ProfesorCedula == prof.Cedula && c.Estado == 1)
            .ToList();
        if (contratos.Count == 0)
        {
            Consola.Linea("No tiene contratos activos.");
            return;
        }

        decimal total = 0;
        foreach (Contrato c in contratos)
        {
            Grupo g = _sistema.BuscarGrupo(c.GrupoCodigo);
            Curso curso = g != null ? _sistema.BuscarCurso(g.CursoCodigo) : null;
            Consola.Linea($"  Contrato #{c.Id}: grupo {c.GrupoCodigo} — {curso?.Nombre ?? "?"}");
            Consola.Linea($"     Horario: {g?.HorarioTexto ?? "?"} ({g?.Modalidad})");
            Consola.Linea($"     Horas contratadas: {c.Horas} h × C$ {c.TarifaHora} = C$ {c.Salario}");
            total += c.Salario;
        }
        Consola.Linea();
        Consola.Linea($"  Salario total contratado: C$ {total}");
    }

    private void VerPlanillaProfesor(Profesor prof)
    {
        Consola.Subtitulo($"MI PLANILLA DE PAGO — PERIODO {_sistema.PeriodoActual}");
        LineaPlanilla linea = _sistema.GenerarPlanilla()
            .FirstOrDefault(l => l.Profesor.Cedula == prof.Cedula);
        if (linea == null)
        {
            Consola.Linea("No tiene contratos activos en el periodo actual.");
            return;
        }

        Consola.Linea($"  Salario base: C$ {linea.SalarioBase}");
        Consola.Linea($"  Ausencias registradas: {linea.Ausencias} → Descuentos: C$ {linea.Descuentos}");
        Consola.Linea($"  SALARIO NETO: C$ {linea.SalarioNeto}");

        Pago pago = _sistema.PagoDe(prof.Cedula);
        Consola.Linea(pago == null
            ? "  Estado de pago: PENDIENTE"
            : $"  Estado de pago: PAGADO (C$ {pago.Monto} el {pago.Fecha:dd/MM/yyyy})");
    }

    // =====================================================================
    //  MÓDULO ADMINISTRADOR
    // =====================================================================
    private void MenuAdministrador()
    {
        while (true)
        {
            Consola.Titulo("MÓDULO ADMINISTRADOR");
            Consola.Linea($"Periodo actual: {_sistema.PeriodoActual}   |   Fecha tope de notas: {_sistema.FechaTopeNotas:dd/MM/yyyy}");
            Consola.Linea("--- Gestión académica ---");
            Consola.Linea(" 1. Listar universidades y carreras");
            Consola.Linea(" 2. Listar cursos");
            Consola.Linea(" 3. Agregar curso");
            Consola.Linea(" 4. Editar curso");
            Consola.Linea(" 5. Listar grupos");
            Consola.Linea(" 6. Agregar grupo");
            Consola.Linea(" 7. Editar grupo (cupo / estado)");
            Consola.Linea("--- Gestión de personas ---");
            Consola.Linea(" 8. Registrar estudiante");
            Consola.Linea(" 9. Registrar profesor");
            Consola.Linea("10. Listar estudiantes");
            Consola.Linea("11. Listar profesores");
            Consola.Linea("12. Editar estudiante (datos y estado 1/0)");
            Consola.Linea("13. Editar profesor (datos y estado 1/0)");
            Consola.Linea("--- Contratos, notas y pagos ---");
            Consola.Linea("14. Asignar clase a profesor (genera contrato)");
            Consola.Linea("15. Listar contratos");
            Consola.Linea("16. Definir fecha tope para ingresar notas");
            Consola.Linea("17. Generar planilla de pagos");
            Consola.Linea("18. Registrar y validar pago");
            Consola.Linea("19. Cerrar sesión");
            int op = Consola.LeerOpcion("Opción: ", 1, 19);
            if (op == 19) { _sesion.Cerrar(); return; }

            try
            {
                switch (op)
                {
                    case 1: ListarUniversidades(); break;
                    case 2: ListarCursos(); break;
                    case 3: AgregarCurso(); break;
                    case 4: EditarCurso(); break;
                    case 5: ListarGrupos(); break;
                    case 6: AgregarGrupo(); break;
                    case 7: EditarGrupo(); break;
                    case 8: RegistrarEstudiante(); break;
                    case 9: RegistrarProfesor(); break;
                    case 10: ListarEstudiantes(); break;
                    case 11: ListarProfesores(); break;
                    case 12: EditarEstudiante(); break;
                    case 13: EditarProfesor(); break;
                    case 14: AsignarClase(); break;
                    case 15: ListarContratos(); break;
                    case 16: DefinirFechaTope(); break;
                    case 17: GenerarPlanilla(); break;
                    case 18: RegistrarPago(); break;
                }
            }
            catch (AppException ex) { Consola.Error(ex.Message); }
            Consola.Pausa();
        }
    }

    // ---------------- Gestión académica ----------------
    private void ListarUniversidades()
    {
        Consola.Subtitulo("UNIVERSIDADES DE NICARAGUA (catálogo con información real)");
        Consola.Linea($"  {"Id",-4}{"Universidad",-52}{"Ciudad",-11}{"Tipo",-9}{"Fundación",-10}");
        foreach (Universidad u in _sistema.Universidades)
            Consola.Linea($"  {u.Id,-4}{Consola.Recortar(u.Nombre, 52),-52}{u.Ciudad,-11}{u.Tipo,-9}" +
                          $"{(u.AnioFundacion?.ToString() ?? "—"),-10}");

        Consola.Subtitulo("CARRERAS");
        Consola.Linea($"  {"Id",-4}{"Carrera",-44}{"Universidad",-22}{"Duración",-18}{"Estado",-12}");
        foreach (Carrera c in _sistema.Carreras)
        {
            Universidad u = _sistema.BuscarUniversidad(c.UniversidadId);
            Consola.Linea($"  {c.Id,-4}{Consola.Recortar(c.Nombre, 44),-44}" +
                          $"{Consola.Recortar(u?.Nombre ?? "?", 22),-22}{c.Duracion,-18}{c.EstadoTexto,-12}");
        }
    }

    private void ListarCursos()
    {
        Consola.Subtitulo("CATÁLOGO DE CURSOS");
        foreach (Curso c in _sistema.Cursos.OrderBy(x => x.Codigo))
        {
            string pre = c.Prerrequisitos.Count == 0 ? "ninguno" : string.Join(", ", c.Prerrequisitos);
            Consola.Linea($"  {c.Codigo,-10}{Consola.Recortar(c.Nombre, 30),-30}" +
                          $"{Consola.Recortar(_sistema.NombreCarrera(c.CarreraId), 28),-28}" +
                          $"Prerrequisitos: {pre}  [{c.EstadoTexto}]");
        }
    }

    private void AgregarCurso()
    {
        Consola.Subtitulo("AGREGAR CURSO");
        string codigo = Consola.LeerTexto("Código del curso (ej.: IS-701): ").ToUpper();
        int carreraId = ElegirCarrera();
        string nombre = Consola.LeerTexto("Nombre del curso: ");
        int creditos = Consola.LeerEntero("Créditos: ", 1, 30);

        List<string> prereqs = new();
        while (Consola.Confirmar("¿Desea agregar un prerrequisito?"))
        {
            string pre = Consola.LeerTexto("Código del prerrequisito: ").ToUpper();
            prereqs.Add(pre);
        }

        _sistema.AgregarCurso(new Curso
        {
            Codigo = codigo,
            CarreraId = carreraId,
            Nombre = nombre,
            Creditos = creditos,
            Prerrequisitos = prereqs,
            Estado = 1
        });
        Consola.Exito($"Curso agregado: {codigo} — {nombre}.");
    }

    private void EditarCurso()
    {
        ListarCursos();
        string codigo = Consola.LeerTexto("Código del curso a editar: ").ToUpper();
        Curso curso = _sistema.BuscarCurso(codigo)
            ?? throw new DatoInvalidoException("No existe un curso con ese código.");

        Consola.Subtitulo("EDITAR CURSO — " + curso.Nombre);
        Consola.Linea("1. Cambiar nombre y créditos");
        Consola.Linea("2. Reemplazar prerrequisitos");
        Consola.Linea("3. Cambiar estado (1 activo / 0 inactivo)");
        Consola.Linea("0. Volver");
        int op = Consola.LeerOpcion("Opción: ", 0, 3);
        switch (op)
        {
            case 1:
                string nombre = Consola.LeerTexto("Nuevo nombre: ");
                int creditos = Consola.LeerEntero("Créditos: ", 1, 30);
                _sistema.ActualizarDatosCurso(curso, nombre, creditos);
                Consola.Exito("Curso actualizado.");
                break;
            case 2:
                List<string> prereqs = new();
                while (Consola.Confirmar("¿Desea agregar un prerrequisito?"))
                {
                    string pre = Consola.LeerTexto("Código del prerrequisito: ").ToUpper();
                    prereqs.Add(pre);
                }
                _sistema.ReemplazarPrerrequisitos(curso, prereqs);
                Consola.Exito("Prerrequisitos actualizados.");
                break;
            case 3:
                int nuevo = Consola.LeerOpcion("Nuevo estado (1 = activo, 0 = inactivo): ", 0, 1);
                _sistema.CambiarEstadoCurso(curso, nuevo);
                Consola.Exito($"Estado actualizado a {curso.EstadoTexto}.");
                break;
        }
    }

    private void ListarGrupos()
    {
        Consola.Subtitulo("GRUPOS");
        foreach (Grupo g in _sistema.Grupos.OrderBy(x => x.Codigo))
        {
            Curso c = _sistema.BuscarCurso(g.CursoCodigo);
            string docente = g.ProfesorCedula == null
                ? "Sin asignar"
                : _sistema.BuscarProfesor(g.ProfesorCedula).Nombre;
            Consola.Linea($"  {g.Codigo,-11}{g.Periodo,-9}{g.CursoCodigo,-10}" +
                          $"{Consola.Recortar(c?.Nombre ?? "?", 22),-22}{g.Modalidad,-10}{g.Dia,-10}" +
                          $"{Consola.Hora(g.HoraInicio)}-{Consola.Hora(g.HoraFin)}  " +
                          $"Cupo {g.CupoDisponible}/{g.Cupo}  Docente: {Consola.Recortar(docente, 20),-20}[{g.EstadoTexto}]");
        }
    }

    private void AgregarGrupo()
    {
        Consola.Subtitulo("AGREGAR GRUPO");
        string codigo = Consola.LeerTexto("Código del grupo (ej.: G-IS701R): ").ToUpper();

        ListarCursos();
        string curso = Consola.LeerTexto("Código del curso: ").ToUpper();

        Modalidad modalidad = Consola.LeerOpcion("Modalidad (1 = Regular, 2 = Dominical): ", 1, 2) == 1
            ? Modalidad.Regular
            : Modalidad.Dominical;

        string[] dias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
        for (int i = 0; i < dias.Length; i++) Consola.Linea($"  {i + 1}. {dias[i]}");
        string dia = dias[Consola.LeerOpcion("Día de la clase: ", 1, 7) - 1];

        TimeSpan inicio = Consola.LeerHora("Hora de inicio (hh:mm): ");
        TimeSpan fin = Consola.LeerHora("Hora de fin (hh:mm): ");
        int cupo = Consola.LeerEntero("Cupo total: ", 1, 200);
        int horas = Consola.LeerEntero("Horas por clase: ", 1, 8);
        string cedulaProf = Consola.LeerTexto("Cédula del profesor (Enter = sin asignar): ", obligatorio: false);

        _sistema.AgregarGrupo(new Grupo
        {
            Codigo = codigo,
            CursoCodigo = curso,
            Periodo = _sistema.PeriodoActual,
            Modalidad = modalidad,
            Dia = dia,
            HoraInicio = inicio,
            HoraFin = fin,
            ProfesorCedula = string.IsNullOrWhiteSpace(cedulaProf) ? null : cedulaProf.Trim().ToUpper(),
            Cupo = cupo,
            HorasPorClase = horas,
            Estado = 1
        });
        Consola.Exito($"Grupo agregado: {codigo} — {dia} {Consola.Hora(inicio)}-{Consola.Hora(fin)}.");
    }

    private void EditarGrupo()
    {
        ListarGrupos();
        string codigo = Consola.LeerTexto("Código del grupo a editar: ").ToUpper();
        Grupo grupo = _sistema.BuscarGrupo(codigo)
            ?? throw new DatoInvalidoException("No existe un grupo con ese código.");

        Consola.Subtitulo($"EDITAR GRUPO — {grupo.Codigo}");
        Consola.Linea("1. Cambiar cupo");
        Consola.Linea("2. Cambiar estado (1 activo / 0 inactivo)");
        Consola.Linea("0. Volver");
        int op = Consola.LeerOpcion("Opción: ", 0, 2);
        switch (op)
        {
            case 1:
                int cupo = Consola.LeerEntero("Nuevo cupo total: ", 1, 200);
                _sistema.CambiarCupo(grupo, cupo);
                Consola.Exito($"Cupo actualizado a {grupo.Cupo} (disponible: {grupo.CupoDisponible}).");
                break;
            case 2:
                int nuevo = Consola.LeerOpcion("Nuevo estado (1 = activo, 0 = inactivo): ", 0, 1);
                _sistema.CambiarEstadoGrupo(grupo, nuevo);
                Consola.Exito($"Estado actualizado a {grupo.EstadoTexto}.");
                break;
        }
    }

    // ---------------- Gestión de personas ----------------
    private int ElegirCarrera()
    {
        Consola.Subtitulo("CARRERAS DISPONIBLES");
        foreach (Carrera c in _sistema.Carreras.Where(c => c.Estado == 1))
        {
            Universidad u = _sistema.BuscarUniversidad(c.UniversidadId);
            Consola.Linea($"  {c.Id}. {c.Nombre} — {u?.Nombre} ({c.Duracion})");
        }
        while (true)
        {
            Console.Write("Id de la carrera: ");
            if (int.TryParse(Console.ReadLine(), out int id)
                && _sistema.Carreras.Any(c => c.Id == id && c.Estado == 1))
                return id;
            Consola.Error("Carrera inválida.");
        }
    }

    private Genero ElegirGenero()
    {
        return Consola.LeerOpcion("Género (1 = Masculino, 2 = Femenino): ", 1, 2) == 1
            ? Genero.Masculino
            : Genero.Femenino;
    }

    private void RegistrarEstudiante()
    {
        Consola.Subtitulo("REGISTRAR ESTUDIANTE");
        string nombre = Consola.LeerTexto("Nombre completo: ");
        string cedula = Consola.LeerTexto("Cédula (XXX-XXXXXX-XXXXL): ");
        DateTime nacimiento = Consola.LeerFecha("Fecha de nacimiento");
        Genero genero = ElegirGenero();
        string celular = Consola.LeerTexto("Número de celular (8 dígitos): ");
        string correo = Consola.LeerTexto("Correo electrónico: ");
        string direccion = Consola.LeerTexto("Dirección: ");
        int carreraId = ElegirCarrera();
        int estado = Consola.LeerOpcion("Estado (1 = activo, 0 = inactivo): ", 0, 1);
        string usuario = Consola.LeerTexto("Usuario (sin espacios, único): ");
        string clave = Consola.LeerTexto("Contraseña (8+, mayúscula, minúscula, número y símbolo): ");

        _sistema.RegistrarEstudiante(new Estudiante
        {
            Nombre = nombre,
            Cedula = cedula,
            FechaNacimiento = nacimiento,
            Genero = genero,
            Celular = celular,
            Correo = correo,
            Direccion = direccion,
            CarreraId = carreraId,
            Estado = estado,
            Usuario = usuario,
            Contrasena = clave
        });
        Consola.Exito($"Estudiante registrado: {nombre} ({cedula}) — {_sistema.NombreCarrera(carreraId)}.");
    }

    private void RegistrarProfesor()
    {
        Consola.Subtitulo("REGISTRAR PROFESOR");
        string nombre = Consola.LeerTexto("Nombre: ");
        string cedula = Consola.LeerTexto("Cédula (XXX-XXXXXX-XXXXL): ");
        string celular = Consola.LeerTexto("Número de celular (8 dígitos): ");
        string correo = Consola.LeerTexto("Correo electrónico: ");
        Consola.Linea("Estado civil: 1. Soltero/a  2. Casado/a  3. Divorciado/a  4. Unión libre");
        string estadoCivil = new[] { "Soltero/a", "Casado/a", "Divorciado/a", "Unión libre" }[Consola.LeerOpcion("Opción: ", 1, 4) - 1];
        string profesion = Consola.LeerTexto("Profesión: ");
        string especialidad = Consola.LeerTexto("Especialidad (Enter para dejarla vacía — acepta nulo): ", obligatorio: false);
        string direccion = Consola.LeerTexto("Dirección: ");
        Genero genero = ElegirGenero();
        int estado = Consola.LeerOpcion("Estado (1 = activo, 0 = inactivo): ", 0, 1);
        string usuario = Consola.LeerTexto("Usuario (sin espacios, único): ");
        string clave = Consola.LeerTexto("Contraseña (8+, mayúscula, minúscula, número y símbolo): ");

        _sistema.RegistrarProfesor(new Profesor
        {
            Nombre = nombre,
            Cedula = cedula,
            Celular = celular,
            Correo = correo,
            EstadoCivil = estadoCivil,
            Profesion = profesion,
            Especialidad = especialidad,
            Direccion = direccion,
            Genero = genero,
            Estado = estado,
            Usuario = usuario,
            Contrasena = clave
        });
        Consola.Exito($"Profesor registrado: {nombre} ({cedula}).");
    }

    private void ListarEstudiantes()
    {
        Consola.Subtitulo("ESTUDIANTES REGISTRADOS");
        Consola.Linea($"  {"Cédula",-18}{"Nombre",-32}{"Usuario",-12}{"Carrera",-30}{"Estado",-12}");
        foreach (Estudiante e in _sistema.Estudiantes)
        {
            Consola.Linea($"  {e.Cedula,-18}{Consola.Recortar(e.Nombre, 32),-32}{e.Usuario,-12}" +
                          $"{Consola.Recortar(_sistema.NombreCarrera(e.CarreraId), 30),-30}{e.EstadoTexto,-12}");
        }
        Consola.Linea($"Total: {_sistema.Estudiantes.Count} estudiante(s).");
    }

    private void ListarProfesores()
    {
        Consola.Subtitulo("PROFESORES REGISTRADOS");
        Consola.Linea($"  {"Cédula",-18}{"Nombre",-30}{"Profesión",-22}{"Especialidad",-24}{"Estado",-12}");
        foreach (Profesor p in _sistema.Profesores)
        {
            Consola.Linea($"  {p.Cedula,-18}{Consola.Recortar(p.Nombre, 30),-30}" +
                          $"{Consola.Recortar(p.Profesion, 22),-22}" +
                          $"{Consola.Recortar(p.Especialidad ?? "(sin especialidad)", 24),-24}{p.EstadoTexto,-12}");
        }
        Consola.Linea($"Total: {_sistema.Profesores.Count} profesor(es).");
    }

    private void EditarEstudiante()
    {
        string cedula = Consola.LeerTexto("Cédula del estudiante a editar: ").ToUpper();
        Estudiante e = _sistema.BuscarEstudiante(cedula)
            ?? throw new DatoInvalidoException("No existe un estudiante con esa cédula.");

        Consola.Subtitulo($"EDITAR ESTUDIANTE — {e.Nombre}");
        Consola.Linea($"  Carrera actual: {_sistema.NombreCarrera(e.CarreraId)}   Estado: {e.EstadoTexto}");
        Consola.Linea("1. Cambiar datos de contacto");
        Consola.Linea("2. Cambiar carrera");
        Consola.Linea("3. Cambiar contraseña");
        Consola.Linea("4. Cambiar estado (1 activo / 0 inactivo)");
        Consola.Linea("0. Volver");
        int op = Consola.LeerOpcion("Opción: ", 0, 4);
        switch (op)
        {
            case 1:
                string celular = Consola.LeerTexto("Nuevo celular: ");
                string correo = Consola.LeerTexto("Nuevo correo: ");
                string dir = Consola.LeerTexto("Nueva dirección: ");
                _sistema.ActualizarDatosContacto(e, celular, correo, dir);
                Consola.Exito("Datos de contacto actualizados.");
                break;
            case 2:
                int carreraId = ElegirCarrera();
                _sistema.ActualizarCarrera(e, carreraId);
                Consola.Exito("Carrera actualizada.");
                break;
            case 3:
                string clave = Consola.LeerTexto("Nueva contraseña: ");
                _sistema.ActualizarContrasena(e, clave);
                Consola.Exito("Contraseña actualizada.");
                break;
            case 4:
                int nuevo = Consola.LeerOpcion("Nuevo estado (1 = activo, 0 = inactivo): ", 0, 1);
                _sistema.CambiarEstado(e, nuevo);
                Consola.Exito($"Estado actualizado a {e.EstadoTexto}.");
                break;
        }
    }

    private void EditarProfesor()
    {
        string cedula = Consola.LeerTexto("Cédula del profesor a editar: ").ToUpper();
        Profesor p = _sistema.BuscarProfesor(cedula)
            ?? throw new DatoInvalidoException("No existe un profesor con esa cédula.");

        Consola.Subtitulo($"EDITAR PROFESOR — {p.Nombre}");
        Consola.Linea($"  Profesión: {p.Profesion}   Especialidad: {p.Especialidad ?? "(ninguna)"}   Estado: {p.EstadoTexto}");
        Consola.Linea("1. Cambiar datos de contacto");
        Consola.Linea("2. Cambiar datos profesionales");
        Consola.Linea("3. Cambiar contraseña");
        Consola.Linea("4. Cambiar estado (1 activo / 0 inactivo)");
        Consola.Linea("0. Volver");
        int op = Consola.LeerOpcion("Opción: ", 0, 4);
        switch (op)
        {
            case 1:
                string celular = Consola.LeerTexto("Nuevo celular: ");
                string correo = Consola.LeerTexto("Nuevo correo: ");
                string dir = Consola.LeerTexto("Nueva dirección: ");
                _sistema.ActualizarDatosContacto(p, celular, correo, dir);
                Consola.Exito("Datos de contacto actualizados.");
                break;
            case 2:
                string ec = Consola.LeerTexto("Estado civil: ");
                string pro = Consola.LeerTexto("Profesión: ");
                string esp = Consola.LeerTexto("Especialidad (Enter para dejarla vacía): ", obligatorio: false);
                _sistema.ActualizarDatosProfesionales(p, ec, pro, esp);
                Consola.Exito("Datos profesionales actualizados.");
                break;
            case 3:
                string clave = Consola.LeerTexto("Nueva contraseña: ");
                _sistema.ActualizarContrasena(p, clave);
                Consola.Exito("Contraseña actualizada.");
                break;
            case 4:
                int nuevo = Consola.LeerOpcion("Nuevo estado (1 = activo, 0 = inactivo): ", 0, 1);
                _sistema.CambiarEstado(p, nuevo);
                Consola.Exito($"Estado actualizado a {p.EstadoTexto}.");
                break;
        }
    }

    // ---------------- Contratos, notas y pagos ----------------
    private void AsignarClase()
    {
        Consola.Subtitulo("ASIGNAR CLASE A PROFESOR (GENERA CONTRATO)");

        Consola.Subtitulo("PROFESORES ACTIVOS");
        foreach (Profesor p in _sistema.Profesores.Where(p => p.Estado == 1))
            Consola.Linea($"  {p.Cedula,-18}{Consola.Recortar(p.Nombre, 30),-30}{p.Profesion}");
        string cedProf = Consola.LeerTexto("Cédula del profesor: ").ToUpper();

        List<Grupo> disponibles = _sistema.Grupos
            .Where(g => g.ProfesorCedula == null && g.Estado == 1 && g.Periodo == _sistema.PeriodoActual)
            .ToList();
        if (disponibles.Count == 0)
            throw new DatoInvalidoException("No hay grupos sin profesor en el periodo actual.");

        Consola.Subtitulo("GRUPOS SIN PROFESOR");
        foreach (Grupo g in disponibles)
        {
            Curso c = _sistema.BuscarCurso(g.CursoCodigo);
            Consola.Linea($"  {g.Codigo,-11}{Consola.Recortar(c?.Nombre ?? "?", 28),-28}" +
                          $"{g.Modalidad,-10}{g.HorarioTexto}");
        }
        string gcod = Consola.LeerTexto("Código del grupo a asignar: ").ToUpper();

        if (!Consola.Confirmar("¿Confirmar la asignación?"))
        {
            Consola.Linea("Asignación cancelada.");
            return;
        }

        Contrato contrato = _sistema.AsignarClase(cedProf, gcod);
        Grupo g2 = _sistema.BuscarGrupo(contrato.GrupoCodigo);
        Curso curso = g2 != null ? _sistema.BuscarCurso(g2.CursoCodigo) : null;

        Consola.Subtitulo("INFORMACIÓN DEL CONTRATO GENERADO");
        Consola.Linea($"  Profesor : {_sistema.BuscarProfesor(contrato.ProfesorCedula).Nombre}");
        Consola.Linea($"  Grupo    : {contrato.GrupoCodigo} — {curso?.Nombre}");
        Consola.Linea($"  Horario  : {g2?.HorarioTexto} ({g2?.Modalidad})");
        Consola.Linea($"  Horas    : {contrato.Horas} h a C$ {contrato.TarifaHora} por hora");
        Consola.Linea($"  Salario  : C$ {contrato.Salario}");
        Consola.Exito("Clase asignada y contrato generado correctamente.");
    }

    private void ListarContratos()
    {
        Consola.Subtitulo("CONTRATOS ACTIVOS");
        List<Contrato> contratos = _sistema.Contratos.Where(c => c.Estado == 1).ToList();
        if (contratos.Count == 0)
        {
            Consola.Linea("No hay contratos activos.");
            return;
        }
        foreach (Contrato c in contratos)
        {
            Grupo g = _sistema.BuscarGrupo(c.GrupoCodigo);
            Curso curso = g != null ? _sistema.BuscarCurso(g.CursoCodigo) : null;
            Consola.Linea($"  #{c.Id,-3}{Consola.Recortar(_sistema.BuscarProfesor(c.ProfesorCedula)?.Nombre ?? "?", 26),-26}" +
                          $"{c.GrupoCodigo,-11}{Consola.Recortar(curso?.Nombre ?? "?", 24),-24}" +
                          $"Horas: {c.Horas}  Tarifa: C$ {c.TarifaHora}/h  Salario: C$ {c.Salario}");
        }
    }

    private void DefinirFechaTope()
    {
        Consola.Subtitulo("DEFINIR FECHA TOPE PARA INGRESAR NOTAS");
        Consola.Linea($"Fecha tope actual: {_sistema.FechaTopeNotas:dd/MM/yyyy}");
        _sistema.FechaTopeNotas = Consola.LeerFecha("Nueva fecha tope");
        Consola.Exito($"Fecha tope actualizada a {_sistema.FechaTopeNotas:dd/MM/yyyy}. " +
                      "Los profesores no podrán registrar notas ni convocatorias después de esa fecha.");
    }

    private void GenerarPlanilla()
    {
        Consola.Subtitulo($"PLANILLA DE PAGOS — PERIODO {_sistema.PeriodoActual}");
        List<LineaPlanilla> planilla = _sistema.GenerarPlanilla();
        if (planilla.Count == 0)
        {
            Consola.Linea("No hay contratos activos en el periodo actual.");
            return;
        }

        Consola.Linea($"  {"Profesor",-28}{"Salario base",14}{"Ausencias",10}{"Descuentos",13}{"Salario neto",14}");
        decimal total = 0;
        foreach (LineaPlanilla l in planilla)
        {
            Consola.Linea($"  {Consola.Recortar(l.Profesor.Nombre, 28),-28}{l.SalarioBase,14}{l.Ausencias,10}" +
                          $"{l.Descuentos,13}{l.SalarioNeto,14}");
            total += l.SalarioNeto;
        }
        Consola.Linea($"  {"TOTAL",-28}{"",14}{"",10}{"",13}{total,14}");
        Consola.Linea();
        Consola.Linea("Los descuentos corresponden a ausencias: C$ 400 por cada hora de clase no impartida.");
    }

    private void RegistrarPago()
    {
        Consola.Subtitulo($"REGISTRAR / VALIDAR PAGO — PERIODO {_sistema.PeriodoActual}");
        List<LineaPlanilla> planilla = _sistema.GenerarPlanilla();
        if (planilla.Count == 0)
        {
            Consola.Linea("No hay planilla generada en el periodo actual.");
            return;
        }

        foreach (LineaPlanilla l in planilla)
        {
            Pago pago = _sistema.PagoDe(l.Profesor.Cedula);
            string estadoPago = pago == null
                ? "PENDIENTE"
                : $"PAGADO (C$ {pago.Monto} el {pago.Fecha:dd/MM/yyyy})";
            Consola.Linea($"  {l.Profesor.Cedula,-18}{Consola.Recortar(l.Profesor.Nombre, 26),-26}" +
                          $"Neto: C$ {l.SalarioNeto,-10}{estadoPago}");
        }

        string cedula = Consola.LeerTexto("Cédula del profesor a pagar: ").ToUpper();
        decimal monto = Consola.LeerMonto("Monto a pagar (C$): ");
        Pago p = _sistema.RegistrarPago(cedula, monto);
        Consola.Exito($"Pago registrado: {p.ProfesorCedula} — C$ {p.Monto} el {p.Fecha:dd/MM/yyyy}.");
    }
}
