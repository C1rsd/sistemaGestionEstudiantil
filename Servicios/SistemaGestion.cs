using SistemaGestionEstudiantil.Excepciones;
using SistemaGestionEstudiantil.Modelos;

namespace SistemaGestionEstudiantil.Servicios;

/// <summary>
/// Núcleo del sistema: almacena la información en memoria y centraliza
/// todas las reglas de negocio del enunciado (acceso por roles,
/// validaciones de registro, inscripciones con prerrequisitos,
/// calificaciones con convocatoria, asistencia y planilla de pagos).
/// </summary>
public class SistemaGestion
{
    // ==================== Catálogos ====================
    public List<Universidad> Universidades { get; } = new();
    public List<Carrera> Carreras { get; } = new();
    public List<Curso> Cursos { get; } = new();
    public List<Grupo> Grupos { get; } = new();

    // ==================== Personas ====================
    public List<Estudiante> Estudiantes { get; } = new();
    public List<Profesor> Profesores { get; } = new();

    public Persona Admin { get; } = new Administrador
    {
        Nombre = "Administrador General",
        Usuario = "admin",
        Contrasena = "Admin#2026",
        Celular = "88888888",
        Correo = "admin.registro@sge.edu.ni",
        Direccion = "Oficina de Registro Académico, Managua",
        Genero = Genero.Masculino,
        Estado = 1
    };

    // ==================== Registros ====================
    public List<Inscripcion> Inscripciones { get; } = new();
    public List<Nota> Notas { get; } = new();
    public List<Contrato> Contratos { get; } = new();
    public List<Asistencia> Asistencias { get; } = new();
    public List<Pago> Pagos { get; } = new();

    // ==================== Configuración ====================
    public string PeriodoActual { get; set; } = "2026-II";
    public DateTime FechaTopeNotas { get; set; } = new(2026, 12, 18);

    private int _idInscripcion = 1;
    private int _idContrato = 1;
    private int _idAsistencia = 1;
    private int _idPago = 1;

    public int NuevoIdInscripcion() => _idInscripcion++;
    public int NuevoIdContrato() => _idContrato++;
    public int NuevoIdAsistencia() => _idAsistencia++;
    public int NuevoIdPago() => _idPago++;

    // =====================================================================
    //  Autenticación
    // =====================================================================
    public Persona Login(string usuario, string clave)
    {
        if (usuario == "admin")
        {
            if (clave == Admin.Contrasena) return Admin;
            throw new CredencialesInvalidasException("Usuario o contraseña incorrectos.");
        }

        Estudiante est = Estudiantes.FirstOrDefault(e => e.Usuario == usuario);
        if (est != null)
        {
            if (est.Contrasena != clave)
                throw new CredencialesInvalidasException("Usuario o contraseña incorrectos.");
            if (est.Estado != 1)
                throw new AccesoDenegadoException(
                    "El estudiante está INACTIVO (0). Solo un estudiante en estado activo (1) puede iniciar sesión.");
            return est;
        }

        Profesor prof = Profesores.FirstOrDefault(p => p.Usuario == usuario);
        if (prof != null)
        {
            if (prof.Contrasena != clave)
                throw new CredencialesInvalidasException("Usuario o contraseña incorrectos.");
            if (prof.Estado != 1)
                throw new AccesoDenegadoException("El profesor está INACTIVO (0) y no puede iniciar sesión.");
            return prof;
        }

        throw new CredencialesInvalidasException("Usuario o contraseña incorrectos.");
    }

    // =====================================================================
    //  Registro de personas (administrador)
    // =====================================================================
    public void RegistrarEstudiante(Estudiante e)
    {
        e.Nombre = e.Nombre.Trim();
        e.Cedula = e.Cedula.Trim().ToUpper();
        e.Usuario = e.Usuario.Trim();
        e.Correo = e.Correo.Trim().ToLower();

        Validador.ValidarTextoObligatorio(e.Nombre, "nombre completo");
        Validador.ValidarCedula(e.Cedula);
        Validador.ValidarCelular(e.Celular);
        Validador.ValidarCorreo(e.Correo);
        Validador.ValidarTextoObligatorio(e.Direccion, "dirección");
        Validador.ValidarUsuario(e.Usuario);
        Validador.ValidarContrasena(e.Contrasena);

        if (ExisteUsuario(e.Usuario))
            throw new UsuarioInvalidoException("El usuario ya existe: debe ser único.");
        if (ExisteCedula(e.Cedula))
            throw new CedulaDuplicadaException(
                "La cédula ya está registrada (como estudiante o como profesor) y no puede repetirse.");
        if (ExisteCorreo(e.Correo))
            throw new CorreoDuplicadoException("El correo ya está registrado por otro estudiante.");
        if (BuscarCarrera(e.CarreraId) == null)
            throw new DatoInvalidoException("La carrera indicada no existe.");
        if (e.FechaNacimiento > DateTime.Now || e.FechaNacimiento < new DateTime(1930, 1, 1))
            throw new DatoInvalidoException("La fecha de nacimiento no es válida.");

        Estudiantes.Add(e);
    }

    public void RegistrarProfesor(Profesor p)
    {
        p.Nombre = p.Nombre.Trim();
        p.Cedula = p.Cedula.Trim().ToUpper();
        p.Usuario = p.Usuario.Trim();
        p.Correo = p.Correo.Trim().ToLower();
        if (!string.IsNullOrWhiteSpace(p.Especialidad)) p.Especialidad = p.Especialidad.Trim();
        else p.Especialidad = null;

        Validador.ValidarTextoObligatorio(p.Nombre, "nombre");
        Validador.ValidarCedula(p.Cedula);
        Validador.ValidarCelular(p.Celular);
        Validador.ValidarCorreo(p.Correo);
        Validador.ValidarTextoObligatorio(p.EstadoCivil, "estado civil");
        Validador.ValidarTextoObligatorio(p.Profesion, "profesión");
        Validador.ValidarTextoObligatorio(p.Direccion, "dirección");
        Validador.ValidarUsuario(p.Usuario);
        Validador.ValidarContrasena(p.Contrasena);

        if (ExisteUsuario(p.Usuario))
            throw new UsuarioInvalidoException("El usuario ya existe: debe ser único.");
        if (ExisteCedula(p.Cedula))
            throw new CedulaDuplicadaException(
                "La cédula ya está registrada (como estudiante o como profesor) y no puede repetirse.");

        Profesores.Add(p);
    }

    // =====================================================================
    //  Búsquedas y consultas
    // =====================================================================
    public Universidad BuscarUniversidad(int id) => Universidades.FirstOrDefault(u => u.Id == id);
    public Carrera BuscarCarrera(int id) => Carreras.FirstOrDefault(c => c.Id == id);
    public string NombreCarrera(int id) => BuscarCarrera(id)?.Nombre ?? "(carrera no encontrada)";
    public string NombreUniversidad(int id) => BuscarUniversidad(id)?.Nombre ?? "(universidad no encontrada)";
    public Estudiante BuscarEstudiante(string cedula) => Estudiantes.FirstOrDefault(e => e.Cedula == cedula);
    public Profesor BuscarProfesor(string cedula) => Profesores.FirstOrDefault(p => p.Cedula == cedula);
    public Curso BuscarCurso(string codigo) => Cursos.FirstOrDefault(c => c.Codigo == codigo);
    public Grupo BuscarGrupo(string codigo) => Grupos.FirstOrDefault(g => g.Codigo == codigo);

    public bool ExisteUsuario(string usuario) =>
        usuario == "admin"
        || Estudiantes.Any(e => e.Usuario == usuario)
        || Profesores.Any(p => p.Usuario == usuario);

    public bool ExisteCedula(string cedula) =>
        Estudiantes.Any(e => e.Cedula == cedula) || Profesores.Any(p => p.Cedula == cedula);

    public bool ExisteCorreo(string correo, string cedulaExcluir = null) =>
        Estudiantes.Any(e => e.Correo == correo && e.Cedula != cedulaExcluir);

    public List<Inscripcion> InscripcionesDeEstudiante(string cedula) =>
        Inscripciones.Where(i => i.EstudianteCedula == cedula && i.Estado == 1)
                     .OrderByDescending(i => i.Periodo)
                     .ThenBy(i => i.Fecha)
                     .ToList();

    public List<(Inscripcion insc, Estudiante est)> InscripcionesDelGrupo(Grupo grupo)
    {
        List<(Inscripcion, Estudiante)> resultado = new();
        foreach (Inscripcion i in Inscripciones
            .Where(i => i.GrupoCodigo == grupo.Codigo && i.Estado == 1)
            .OrderBy(i => i.Fecha))
        {
            Estudiante e = Estudiantes.FirstOrDefault(x => x.Cedula == i.EstudianteCedula);
            if (e != null) resultado.Add((i, e));
        }
        return resultado;
    }

    public Inscripcion InscripcionDe(string cedula, string grupoCodigo) =>
        Inscripciones.FirstOrDefault(i => i.EstudianteCedula == cedula
                                       && i.GrupoCodigo == grupoCodigo
                                       && i.Estado == 1);

    /// <summary>Códigos de curso que el estudiante tiene aprobados (incluidos por convocatoria).</summary>
    public HashSet<string> CursosAprobados(string cedula)
    {
        HashSet<string> set = new();
        foreach (Inscripcion insc in Inscripciones.Where(i => i.EstudianteCedula == cedula && i.Estado == 1))
        {
            Nota nota = Notas.FirstOrDefault(n => n.InscripcionId == insc.Id);
            if (nota == null || !nota.Aprobado) continue;
            Grupo grupo = Grupos.FirstOrDefault(g => g.Codigo == insc.GrupoCodigo);
            if (grupo != null) set.Add(grupo.CursoCodigo);
        }
        return set;
    }

    public Nota NotaDe(Inscripcion insc) => Notas.FirstOrDefault(n => n.InscripcionId == insc.Id);

    public List<Grupo> GruposDeProfesor(string cedula) =>
        Grupos.Where(g => g.ProfesorCedula == cedula && g.Estado == 1 && g.Periodo == PeriodoActual)
              .ToList();

    public bool HayEstudiantesConDerechoConvocatoria(Profesor prof) =>
        GruposDeProfesor(prof.Cedula).Any(g => InscripcionesDelGrupo(g)
            .Any(par => { Nota n = NotaDe(par.insc); return n != null && n.DerechoConvocatoria; }));

    // =====================================================================
    //  Inscripción de clases
    // =====================================================================
    /// <summary>Valida prerrequisitos y que el curso no esté ya aprobado. Lanza las excepciones del enunciado.</summary>
    public void VerificarPrerrequisitos(Estudiante est, Curso curso)
    {
        if (est.Estado != 1)
            throw new AccesoDenegadoException("Solo un estudiante en estado activo (1) puede inscribir clases.");

        HashSet<string> aprobados = CursosAprobados(est.Cedula);

        foreach (string pre in curso.Prerrequisitos)
        {
            if (!aprobados.Contains(pre))
            {
                Curso falta = BuscarCurso(pre);
                throw new PrerrequisitoPendienteException(
                    $"Inscripción bloqueada: falta aprobar {falta?.Nombre ?? pre} ({pre}).");
            }
        }

        if (aprobados.Contains(curso.Codigo))
            throw new CursoYaAprobadoException(
                $"Ya aprobó {curso.Nombre} ({curso.Codigo}); no puede inscribirlo de nuevo.");
    }

    /// <summary>Todas las validaciones del flujo de inscripción (pasos 3 y 4 del enunciado).</summary>
    public void VerificarInscripcion(Estudiante est, Grupo grupo)
    {
        if (est.Estado != 1)
            throw new AccesoDenegadoException("Solo un estudiante en estado activo (1) puede inscribir clases.");
        if (grupo == null || grupo.Estado != 1)
            throw new DatoInvalidoException("El grupo no existe o está inactivo (0).");

        Curso curso = BuscarCurso(grupo.CursoCodigo);
        if (curso == null || curso.Estado != 1)
            throw new DatoInvalidoException("El curso no existe o está inactivo (0).");
        if (curso.CarreraId != est.CarreraId)
            throw new DatoInvalidoException("El curso no pertenece a la carrera del estudiante.");

        VerificarPrerrequisitos(est, curso);

        List<Inscripcion> delPeriodo = Inscripciones
            .Where(i => i.EstudianteCedula == est.Cedula && i.Periodo == grupo.Periodo && i.Estado == 1)
            .ToList();

        foreach (Inscripcion insc in delPeriodo)
        {
            Grupo otro = BuscarGrupo(insc.GrupoCodigo);
            if (otro == null) continue;
            if (otro.CursoCodigo == curso.Codigo)
                throw new InscripcionDuplicadaException(
                    $"Ya está inscrito en {curso.Nombre} en el periodo {grupo.Periodo}.");
            if (otro.Dia == grupo.Dia && SeSolapan(otro, grupo))
                throw new HorarioConflictoException(
                    $"Conflicto de horario: el {grupo.Dia} ya tiene el grupo {otro.Codigo} de " +
                    $"{BuscarCurso(otro.CursoCodigo).Nombre} ({HoraCorta(otro.HoraInicio)}-{HoraCorta(otro.HoraFin)}).");
        }

        if (grupo.CupoDisponible <= 0)
            throw new CupoInsuficienteException($"El grupo {grupo.Codigo} no tiene cupo disponible.");
    }

    public Inscripcion Inscribir(Estudiante est, Grupo grupo)
    {
        VerificarInscripcion(est, grupo);
        Inscripcion nueva = new()
        {
            Id = NuevoIdInscripcion(),
            EstudianteCedula = est.Cedula,
            GrupoCodigo = grupo.Codigo,
            Periodo = grupo.Periodo,
            Fecha = DateTime.Now,
            Estado = 1
        };
        Inscripciones.Add(nueva);
        grupo.CupoOcupado++;
        return nueva;
    }

    public bool SeSolapan(Grupo a, Grupo b) => a.HoraInicio < b.HoraFin && b.HoraInicio < a.HoraFin;

    // =====================================================================
    //  Asignación de clases y contratos
    // =====================================================================
    public Contrato AsignarClase(string profesorCedula, string grupoCodigo)
    {
        Profesor prof = BuscarProfesor(profesorCedula)
            ?? throw new DatoInvalidoException("No existe un profesor con esa cédula.");
        if (prof.Estado != 1)
            throw new AccesoDenegadoException("El profesor está inactivo (0); actívelo antes de asignarle clases.");

        Grupo grupo = BuscarGrupo(grupoCodigo) ?? throw new DatoInvalidoException("No existe un grupo con ese código.");
        if (grupo.Estado != 1)
            throw new DatoInvalidoException("El grupo está inactivo (0).");
        if (grupo.ProfesorCedula != null)
            throw new RegistroDuplicadoException($"El grupo {grupo.Codigo} ya tiene profesor asignado.");

        // Conflicto: clase ya asignada el mismo día y horario.
        foreach (Contrato c in Contratos.Where(c => c.ProfesorCedula == prof.Cedula && c.Estado == 1))
        {
            Grupo otro = BuscarGrupo(c.GrupoCodigo);
            if (otro == null) continue;
            if (otro.Dia == grupo.Dia && SeSolapan(otro, grupo))
                throw new HorarioConflictoException(
                    $"No se puede asignar: el profesor ya tiene el grupo {otro.Codigo} de " +
                    $"{BuscarCurso(otro.CursoCodigo).Nombre} el {grupo.Dia} de " +
                    $"{HoraCorta(otro.HoraInicio)} a {HoraCorta(otro.HoraFin)}.");
        }

        // Tarifa real del enunciado: C$400 por hora; dominical 60 h, regular 80 h.
        int horas = grupo.Modalidad == Modalidad.Dominical ? 60 : 80;

        Contrato contrato = new()
        {
            Id = NuevoIdContrato(),
            ProfesorCedula = prof.Cedula,
            GrupoCodigo = grupo.Codigo,
            Periodo = grupo.Periodo,
            Horas = horas,
            TarifaHora = 400m,
            Salario = horas * 400m,
            Fecha = DateTime.Now,
            Estado = 1
        };
        grupo.ProfesorCedula = prof.Cedula;
        Contratos.Add(contrato);
        return contrato;
    }

    // =====================================================================
    //  Calificaciones
    // =====================================================================
    public void ValidarFechaTope()
    {
        if (DateTime.Now.Date > FechaTopeNotas.Date)
            throw new FechaTopeVencidaException(
                $"La fecha tope para ingresar notas ({FechaTopeNotas:dd/MM/yyyy}) ya venció. No es posible continuar.");
    }

    public Nota RegistrarNota(Profesor prof, Grupo grupo, Inscripcion insc, decimal acumulado, decimal examen)
    {
        if (grupo.ProfesorCedula != prof.Cedula)
            throw new AccesoDenegadoException("Solo puede calificar grupos que tiene asignados.");
        ValidarFechaTope();
        if (acumulado < 0 || acumulado > 60)
            throw new DatoInvalidoException("El acumulado debe estar entre 0 y 60.");
        if (examen < 0 || examen > 40)
            throw new DatoInvalidoException("El examen debe estar entre 0 y 40.");
        if (NotaDe(insc) != null)
            throw new NotaYaRegistradaException("El estudiante ya tiene nota registrada en este grupo.");

        Nota nota = new() { InscripcionId = insc.Id, Acumulado = acumulado, Examen = examen };
        Notas.Add(nota);
        return nota;
    }

    public Nota RegistrarConvocatoria(Profesor prof, Grupo grupo, Inscripcion insc, decimal notaExtraordinaria)
    {
        if (grupo.ProfesorCedula != prof.Cedula)
            throw new AccesoDenegadoException("Solo puede gestionar convocatorias de sus grupos.");
        ValidarFechaTope();
        if (notaExtraordinaria < 0 || notaExtraordinaria > 100)
            throw new DatoInvalidoException("La nota extraordinaria debe estar entre 0 y 100.");

        Nota nota = NotaDe(insc)
            ?? throw new DatoInvalidoException("El estudiante todavía no tiene nota registrada (Nota pendiente).");
        if (nota.Final >= 60)
            throw new SinDerechoConvocatoriaException("El estudiante aprobó la clase; no requiere convocatoria.");
        if (nota.Final < 40)
            throw new SinDerechoConvocatoriaException(
                "El estudiante reprobó con nota menor a 40; no tiene derecho a convocatoria.");
        if (nota.TieneConvocatoria)
            throw new ConvocatoriaYaRealizadaException(
                "El estudiante ya realizó su única convocatoria en esta clase (la nota no se puede editar).");

        nota.NotaExtraordinaria = notaExtraordinaria;
        nota.NotaAplicada = Math.Min(notaExtraordinaria, 80m); // tope de 80
        return nota;
    }

    // =====================================================================
    //  Asistencia del profesor
    // =====================================================================
    public Asistencia MarcarAsistencia(Profesor prof, Grupo grupo, DateTime fecha, TimeSpan horaEntrada)
    {
        if (grupo.ProfesorCedula != prof.Cedula)
            throw new AccesoDenegadoException("Solo puede marcar asistencia en sus grupos.");
        ValidarAsistenciaNoDuplicada(prof, grupo, fecha);

        // 15 minutos hábiles después de la hora de inicio.
        TimeSpan limite = grupo.HoraInicio.Add(TimeSpan.FromMinutes(15));
        if (horaEntrada > limite)
            throw new AsistenciaFueraDeTiempoException(
                $"Llegó a las {HoraCorta(horaEntrada)}; la clase inició a las {HoraCorta(grupo.HoraInicio)} y solo " +
                $"tenía hasta las {HoraCorta(limite)} para marcar. Ya no puede marcar asistencia.");

        Asistencia a = new()
        {
            Id = NuevoIdAsistencia(),
            ProfesorCedula = prof.Cedula,
            GrupoCodigo = grupo.Codigo,
            Fecha = fecha,
            HoraMarcada = horaEntrada,
            Presente = true,
            HorasDescontadas = 0,
            Estado = 1
        };
        Asistencias.Add(a);
        return a;
    }

    public Asistencia RegistrarAusencia(Profesor prof, Grupo grupo, DateTime fecha)
    {
        if (grupo.ProfesorCedula != prof.Cedula)
            throw new AccesoDenegadoException("Solo puede registrar ausencias en sus grupos.");
        ValidarAsistenciaNoDuplicada(prof, grupo, fecha);

        // Si no asiste, se le descuentan las horas de esa clase a C$400 por hora.
        Asistencia a = new()
        {
            Id = NuevoIdAsistencia(),
            ProfesorCedula = prof.Cedula,
            GrupoCodigo = grupo.Codigo,
            Fecha = fecha,
            HoraMarcada = null,
            Presente = false,
            HorasDescontadas = grupo.HorasPorClase,
            Estado = 1
        };
        Asistencias.Add(a);
        return a;
    }

    private void ValidarAsistenciaNoDuplicada(Profesor prof, Grupo grupo, DateTime fecha)
    {
        if (Asistencias.Any(a => a.ProfesorCedula == prof.Cedula
                              && a.GrupoCodigo == grupo.Codigo
                              && a.Fecha.Date == fecha.Date
                              && a.Estado == 1))
            throw new DatoInvalidoException("Ya existe un registro de asistencia para esa clase en esa fecha.");
    }

    // =====================================================================
    //  Planilla y pagos
    // =====================================================================
    public List<LineaPlanilla> GenerarPlanilla()
    {
        List<LineaPlanilla> planilla = new();
        foreach (Profesor prof in Profesores.Where(p => p.Estado == 1))
        {
            decimal salarioBase = Contratos
                .Where(c => c.ProfesorCedula == prof.Cedula && c.Estado == 1 && c.Periodo == PeriodoActual)
                .Sum(c => c.Salario);
            if (salarioBase == 0) continue;

            List<Asistencia> ausencias = Asistencias
                .Where(a => a.ProfesorCedula == prof.Cedula && a.Estado == 1 && !a.Presente)
                .ToList();
            decimal descuentos = ausencias.Sum(a => a.HorasDescontadas * 400m);

            planilla.Add(new LineaPlanilla
            {
                Profesor = prof,
                SalarioBase = salarioBase,
                Descuentos = descuentos,
                Ausencias = ausencias.Count
            });
        }
        return planilla;
    }

    public Pago PagoDe(string profesorCedula) =>
        Pagos.FirstOrDefault(p => p.ProfesorCedula == profesorCedula
                                && p.Periodo == PeriodoActual
                                && p.Estado == 1);

    public Pago RegistrarPago(string profesorCedula, decimal monto)
    {
        LineaPlanilla linea = GenerarPlanilla().FirstOrDefault(l => l.Profesor.Cedula == profesorCedula)
            ?? throw new PagoInvalidoException("El profesor no tiene planilla en el periodo actual.");
        if (PagoDe(profesorCedula) != null)
            throw new PagoInvalidoException("El profesor ya recibió su pago de este periodo; no puede pagarse dos veces.");
        if (monto != linea.SalarioNeto)
            throw new PagoInvalidoException(
                $"El monto (C$ {monto}) no coincide con el salario neto de la planilla (C$ {linea.SalarioNeto}).");

        Pago pago = new()
        {
            Id = NuevoIdPago(),
            ProfesorCedula = profesorCedula,
            Periodo = PeriodoActual,
            Monto = monto,
            Fecha = DateTime.Now,
            Estado = 1
        };
        Pagos.Add(pago);
        return pago;
    }

    // =====================================================================
    //  Agregar / editar / listar  (sin DELETE: se usa estado 1/0)
    // =====================================================================
    public void AgregarCurso(Curso curso)
    {
        curso.Codigo = curso.Codigo.Trim().ToUpper();
        curso.Nombre = curso.Nombre.Trim();
        Validador.ValidarTextoObligatorio(curso.Codigo, "código del curso");
        Validador.ValidarTextoObligatorio(curso.Nombre, "nombre del curso");
        if (Cursos.Any(c => c.Codigo == curso.Codigo))
            throw new RegistroDuplicadoException("Ya existe un curso con ese código.");
        if (BuscarCarrera(curso.CarreraId) == null)
            throw new DatoInvalidoException("La carrera indicada no existe.");
        if (curso.Creditos <= 0)
            throw new DatoInvalidoException("Los créditos deben ser mayores que cero.");
        foreach (string pre in curso.Prerrequisitos)
            if (Cursos.All(c => c.Codigo != pre))
                throw new DatoInvalidoException($"El prerrequisito {pre} no existe.");
        Cursos.Add(curso);
    }

    public void AgregarGrupo(Grupo grupo)
    {
        grupo.Codigo = grupo.Codigo.Trim().ToUpper();
        Validador.ValidarTextoObligatorio(grupo.Codigo, "código del grupo");
        if (Grupos.Any(g => g.Codigo == grupo.Codigo))
            throw new RegistroDuplicadoException("Ya existe un grupo con ese código.");
        if (BuscarCurso(grupo.CursoCodigo) == null)
            throw new DatoInvalidoException("El curso indicado no existe.");
        if (grupo.HoraFin <= grupo.HoraInicio)
            throw new DatoInvalidoException("La hora de fin debe ser posterior a la hora de inicio.");
        if (grupo.Cupo <= 0)
            throw new DatoInvalidoException("El cupo debe ser mayor que cero.");
        if (grupo.HorasPorClase <= 0)
            throw new DatoInvalidoException("Las horas por clase deben ser mayores que cero.");
        if (grupo.ProfesorCedula != null)
        {
            Profesor prof = BuscarProfesor(grupo.ProfesorCedula)
                ?? throw new DatoInvalidoException("El profesor indicado no existe.");
            if (prof.Estado != 1)
                throw new DatoInvalidoException("El profesor indicado está inactivo (0).");
        }
        Grupos.Add(grupo);
    }

    public void CambiarEstado(Persona p, int estado)
    {
        if (estado != 0 && estado != 1)
            throw new DatoInvalidoException("El estado solo puede ser 1 (activo) o 0 (inactivo).");
        p.Estado = estado;
    }

    public void CambiarEstadoCurso(Curso c, int estado)
    {
        if (estado != 0 && estado != 1)
            throw new DatoInvalidoException("El estado solo puede ser 1 (activo) o 0 (inactivo).");
        c.Estado = estado;
    }

    public void CambiarEstadoGrupo(Grupo g, int estado)
    {
        if (estado != 0 && estado != 1)
            throw new DatoInvalidoException("El estado solo puede ser 1 (activo) o 0 (inactivo).");
        g.Estado = estado;
    }

    public void CambiarCupo(Grupo g, int cupo)
    {
        if (cupo < g.CupoOcupado)
            throw new DatoInvalidoException(
                $"El cupo no puede ser menor que los ya inscritos ({g.CupoOcupado}).");
        g.Cupo = cupo;
    }

    public void ActualizarDatosContacto(Persona p, string celular, string correo, string direccion)
    {
        Validador.ValidarCelular(celular);
        Validador.ValidarCorreo(correo);
        Validador.ValidarTextoObligatorio(direccion, "dirección");
        if (ExisteCorreo(correo, p.Cedula))
            throw new CorreoDuplicadoException("El correo ya pertenece a otro estudiante.");
        p.Celular = celular.Trim();
        p.Correo = correo.Trim().ToLower();
        p.Direccion = direccion.Trim();
    }

    public void ActualizarContrasena(Persona p, string nueva)
    {
        Validador.ValidarContrasena(nueva);
        p.Contrasena = nueva;
    }

    public void ActualizarCarrera(Estudiante e, int carreraId)
    {
        if (BuscarCarrera(carreraId) == null)
            throw new DatoInvalidoException("La carrera indicada no existe.");
        e.CarreraId = carreraId;
    }

    public void ActualizarDatosProfesionales(Profesor p, string estadoCivil, string profesion, string especialidad)
    {
        Validador.ValidarTextoObligatorio(estadoCivil, "estado civil");
        Validador.ValidarTextoObligatorio(profesion, "profesión");
        p.EstadoCivil = estadoCivil.Trim();
        p.Profesion = profesion.Trim();
        p.Especialidad = string.IsNullOrWhiteSpace(especialidad) ? null : especialidad.Trim();
    }

    public void ActualizarDatosCurso(Curso c, string nombre, int creditos)
    {
        Validador.ValidarTextoObligatorio(nombre, "nombre del curso");
        if (creditos <= 0)
            throw new DatoInvalidoException("Los créditos deben ser mayores que cero.");
        c.Nombre = nombre.Trim();
        c.Creditos = creditos;
    }

    public void ReemplazarPrerrequisitos(Curso c, List<string> prerrequisitos)
    {
        foreach (string pre in prerrequisitos)
            if (Cursos.All(x => x.Codigo != pre))
                throw new DatoInvalidoException($"El prerrequisito {pre} no existe.");
        c.Prerrequisitos = prerrequisitos;
    }

    private static string HoraCorta(TimeSpan t) => $"{t.Hours:D2}:{t.Minutes:D2}";
}
