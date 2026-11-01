using SistemaGestionEstudiantil.Modelos;
using SistemaGestionEstudiantil.Servicios;

namespace SistemaGestionEstudiantil.Datos;

/// <summary>
/// Datos iniciales del sistema, basados en información REAL de Nicaragua:
///
/// 1. Catálogo de universidades: miembros del Consejo Nacional de
///    Universidades (CNU) y universidades privadas reconocidas, con su
///    ciudad y año de fundación (solo cuando el dato está verificado
///    en fuentes oficiales o enciclopédicas).
/// 2. Pénsum real de la carrera Ingeniería de Sistemas de la UNI
///    (Facultad de Ciencias y Sistemas): Introducción a la Programación,
///    Programación I y II, Base de Datos I y II, Ingeniería de Software,
///    Sistemas Operativos, Redes, Inteligencia Artificial, etc.
/// 3. Códigos reales de cursos de la UCYT (pénsum 2026-2030 de
///    Ingeniería de Sistemas): IF-3900, IF-1100, IF-1400.
/// 4. Modalidades reales: regular (diurno/vespertino/sabatino) y
///    dominical, tal como se imparten en universidades nicaragüenses.
/// 5. Formato real de cédula (XXX-XXXXXX-XXXXL) y de celular (8 dígitos).
/// 6. Periodo académico en formato real: 2026-II.
///
/// Los nombres de personas, cédulas y celulares son ficticios, creados
/// únicamente para la demostración académica.
/// </summary>
public static class DatosIniciales
{
    public static void Cargar(SistemaGestion s)
    {
        CargarUniversidadesYCarreras(s);
        CargarCursos(s);
        CargarProfesores(s);
        CargarGruposYContratos(s);
        CargarEstudiantes(s);
        CargarHistorialAcademico(s);
        CargarPeriodoActual(s);
        CargarAsistenciasYPagos(s);
    }

    // =====================================================================
    //  Universidades (reales) y carreras
    // =====================================================================
    private static void CargarUniversidadesYCarreras(SistemaGestion s)
    {
        s.Universidades.Add(new Universidad { Id = 1, Nombre = "Universidad Nacional Autónoma de Nicaragua, Managua (UNAN-Managua)", Ciudad = "Managua", Tipo = "Pública", AnioFundacion = 1969 });
        s.Universidades.Add(new Universidad { Id = 2, Nombre = "Universidad Nacional Autónoma de Nicaragua, León (UNAN-León)", Ciudad = "León", Tipo = "Pública", AnioFundacion = 1812 });
        s.Universidades.Add(new Universidad { Id = 3, Nombre = "Universidad Nacional de Ingeniería (UNI)", Ciudad = "Managua", Tipo = "Pública", AnioFundacion = 1983 });
        s.Universidades.Add(new Universidad { Id = 4, Nombre = "Universidad Centroamericana (UCA)", Ciudad = "Managua", Tipo = "Privada", AnioFundacion = 1960 });
        s.Universidades.Add(new Universidad { Id = 5, Nombre = "Universidad Nacional Agraria (UNA)", Ciudad = "Managua", Tipo = "Pública", AnioFundacion = null });
        s.Universidades.Add(new Universidad { Id = 6, Nombre = "Universidad de Managua (UdeM)", Ciudad = "Managua", Tipo = "Privada", AnioFundacion = 1998 });
        s.Universidades.Add(new Universidad { Id = 7, Nombre = "Universidad Nicaragüense de Ciencia y Tecnología (UCYT)", Ciudad = "Managua", Tipo = "Privada", AnioFundacion = null });
        s.Universidades.Add(new Universidad { Id = 8, Nombre = "Bluefields Indian and Caribbean University (BICU)", Ciudad = "Bluefields", Tipo = "Pública", AnioFundacion = null });

        s.Carreras.Add(new Carrera { Id = 1, UniversidadId = 3, Nombre = "Ingeniería de Sistemas", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 2, UniversidadId = 3, Nombre = "Ingeniería en Computación", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 3, UniversidadId = 3, Nombre = "Ingeniería Civil", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 4, UniversidadId = 2, Nombre = "Medicina y Cirugía", Duracion = "14 semestres" });
        s.Carreras.Add(new Carrera { Id = 5, UniversidadId = 1, Nombre = "Derecho", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 6, UniversidadId = 1, Nombre = "Contaduría Pública", Duracion = "9 semestres" });
        s.Carreras.Add(new Carrera { Id = 7, UniversidadId = 4, Nombre = "Administración de Empresas", Duracion = "8 semestres" });
        s.Carreras.Add(new Carrera { Id = 8, UniversidadId = 4, Nombre = "Psicología", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 9, UniversidadId = 5, Nombre = "Agronomía", Duracion = "10 semestres" });
        s.Carreras.Add(new Carrera { Id = 10, UniversidadId = 6, Nombre = "Ingeniería en Sistemas", Duracion = "9 semestres" });
        s.Carreras.Add(new Carrera { Id = 11, UniversidadId = 7, Nombre = "Ingeniería de Sistemas", Duracion = "14 cuatrimestres" });
    }

    // =====================================================================
    //  Cursos (asignaturas reales del pénsum de la UNI y códigos de la UCYT)
    // =====================================================================
    private static void CargarCursos(SistemaGestion s)
    {
        // Ingeniería de Sistemas — UNI (pénsum real)
        s.Cursos.Add(new Curso { Codigo = "IS-101", CarreraId = 1, Nombre = "Introducción a la Programación I", Creditos = 4, Prerrequisitos = { } });
        s.Cursos.Add(new Curso { Codigo = "IS-102", CarreraId = 1, Nombre = "Matemática I", Creditos = 4, Prerrequisitos = { } });
        s.Cursos.Add(new Curso { Codigo = "IS-103", CarreraId = 1, Nombre = "Inglés I", Creditos = 2, Prerrequisitos = { } });
        s.Cursos.Add(new Curso { Codigo = "IS-201", CarreraId = 1, Nombre = "Programación I", Creditos = 4, Prerrequisitos = { "IS-101" } });
        s.Cursos.Add(new Curso { Codigo = "IS-202", CarreraId = 1, Nombre = "Matemática II", Creditos = 4, Prerrequisitos = { "IS-102" } });
        s.Cursos.Add(new Curso { Codigo = "IS-203", CarreraId = 1, Nombre = "Estadística I", Creditos = 3, Prerrequisitos = { "IS-202" } });
        s.Cursos.Add(new Curso { Codigo = "IS-301", CarreraId = 1, Nombre = "Programación II", Creditos = 4, Prerrequisitos = { "IS-201" } });
        s.Cursos.Add(new Curso { Codigo = "IS-302", CarreraId = 1, Nombre = "Base de Datos I", Creditos = 4, Prerrequisitos = { "IS-301" } });
        s.Cursos.Add(new Curso { Codigo = "IS-303", CarreraId = 1, Nombre = "Sistemas Operativos", Creditos = 4, Prerrequisitos = { "IS-301" } });
        s.Cursos.Add(new Curso { Codigo = "IS-401", CarreraId = 1, Nombre = "Base de Datos II", Creditos = 4, Prerrequisitos = { "IS-302" } });
        s.Cursos.Add(new Curso { Codigo = "IS-402", CarreraId = 1, Nombre = "Ingeniería de Software I", Creditos = 4, Prerrequisitos = { "IS-301", "IS-302" } });
        s.Cursos.Add(new Curso { Codigo = "IS-501", CarreraId = 1, Nombre = "Ingeniería de Software II", Creditos = 4, Prerrequisitos = { "IS-402" } });
        s.Cursos.Add(new Curso { Codigo = "IS-502", CarreraId = 1, Nombre = "Redes de Computadoras", Creditos = 4, Prerrequisitos = { "IS-303" } });
        s.Cursos.Add(new Curso { Codigo = "IS-601", CarreraId = 1, Nombre = "Inteligencia Artificial", Creditos = 4, Prerrequisitos = { "IS-501" } });

        // Ingeniería Civil — UNI
        s.Cursos.Add(new Curso { Codigo = "CV-201", CarreraId = 3, Nombre = "Cálculo I", Creditos = 5, Prerrequisitos = { } });
        s.Cursos.Add(new Curso { Codigo = "CV-202", CarreraId = 3, Nombre = "Dibujo Lineal y Geometría Descriptiva", Creditos = 4, Prerrequisitos = { } });

        // Ingeniería de Sistemas — UCYT (códigos reales del pénsum 2026-2030)
        s.Cursos.Add(new Curso { Codigo = "IF-3900", CarreraId = 11, Nombre = "Introducción a la Programación", Creditos = 3, Prerrequisitos = { } });
        s.Cursos.Add(new Curso { Codigo = "IF-1100", CarreraId = 11, Nombre = "Programación de Computadoras I", Creditos = 3, Prerrequisitos = { "IF-3900" } });
        s.Cursos.Add(new Curso { Codigo = "IF-1400", CarreraId = 11, Nombre = "Programación de Computadoras II", Creditos = 3, Prerrequisitos = { "IF-1100" } });
    }

    // =====================================================================
    //  Profesores (datos ficticios con formatos reales)
    // =====================================================================
    private static void CargarProfesores(SistemaGestion s)
    {
        s.Profesores.Add(new Profesor
        {
            Nombre = "Carlos Adán Martínez Solís", Cedula = "001-020573-0845A", Celular = "88124567",
            Correo = "carlos.martinez.s@gmail.com", EstadoCivil = "Casado/a", Profesion = "Ingeniero de Sistemas",
            Especialidad = "Bases de Datos", Direccion = "Residencial El Prado, Managua",
            Genero = Genero.Masculino, Estado = 1, Usuario = "cmartinez", Contrasena = "Clave#2026"
        });
        s.Profesores.Add(new Profesor
        {
            Nombre = "Martha Elena López Rivas", Cedula = "001-031180-1012C", Celular = "87654321",
            Correo = "martha.lopez.r@gmail.com", EstadoCivil = "Casado/a", Profesion = "Ingeniera en Computación",
            Especialidad = null, Direccion = "Barrio Monseñor Lezcano, Managua",
            Genero = Genero.Femenino, Estado = 1, Usuario = "mlopez", Contrasena = "Martha#2026"
        });
        s.Profesores.Add(new Profesor
        {
            Nombre = "José Antonio Rivera Gómez", Cedula = "001-150970-0004B", Celular = "84903456",
            Correo = "jose.rivera.g@gmail.com", EstadoCivil = "Soltero/a", Profesion = "Ingeniero Civil",
            Especialidad = "Matemática Aplicada", Direccion = "Barrio San Judas, Managua",
            Genero = Genero.Masculino, Estado = 1, Usuario = "jrivera", Contrasena = "Jose#2026R"
        });
        s.Profesores.Add(new Profesor
        {
            Nombre = "Ana Gabriela Sánchez Duarte", Cedula = "001-110685-0305A", Celular = "82334412",
            Correo = "ana.sanchez.d@gmail.com", EstadoCivil = "Soltero/a", Profesion = "Licenciada en Idiomas",
            Especialidad = null, Direccion = "Colonia Máximo Jerez, Managua",
            Genero = Genero.Femenino, Estado = 1, Usuario = "asanchez", Contrasena = "Ana#2026S"
        });
        s.Profesores.Add(new Profesor
        {
            Nombre = "Favio Ernesto Zamora Cruz", Cedula = "001-080775-0909D", Celular = "89345678",
            Correo = "favio.zamora.c@gmail.com", EstadoCivil = "Casado/a", Profesion = "Ingeniero de Sistemas",
            Especialidad = "Sistemas Operativos", Direccion = "Colonia Centroamérica, Managua",
            Genero = Genero.Masculino, Estado = 1, Usuario = "fzamora", Contrasena = "Favio#2026"
        });
        s.Profesores.Add(new Profesor
        {
            Nombre = "Karla Vanessa Núñez Bonilla", Cedula = "001-210488-0007B", Celular = "85678934",
            Correo = "karla.nunez.b@gmail.com", EstadoCivil = "Casado/a", Profesion = "Ingeniera de Sistemas",
            Especialidad = "Inteligencia Artificial", Direccion = "Colonia Las Colinas, Managua",
            Genero = Genero.Femenino, Estado = 1, Usuario = "knunez", Contrasena = "Karla#2026"
        });
    }

    // =====================================================================
    //  Grupos, asignación de docentes y contratos
    // =====================================================================
    private static void CargarGruposYContratos(SistemaGestion s)
    {
        // ---------- Periodo 2026-I (finalizado; estado 0) ----------
        s.Grupos.Add(new Grupo { Codigo = "G-IS101I", CursoCodigo = "IS-101", Periodo = "2026-I", Modalidad = Modalidad.Regular, Dia = "Lunes", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-020573-0845A", Cupo = 45, CupoOcupado = 5, HorasPorClase = 2, Estado = 0 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS102I", CursoCodigo = "IS-102", Periodo = "2026-I", Modalidad = Modalidad.Regular, Dia = "Miércoles", HoraInicio = new TimeSpan(9, 0, 0), HoraFin = new TimeSpan(11, 0, 0), ProfesorCedula = "001-150970-0004B", Cupo = 45, CupoOcupado = 1, HorasPorClase = 2, Estado = 0 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS201I", CursoCodigo = "IS-201", Periodo = "2026-I", Modalidad = Modalidad.Regular, Dia = "Martes", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-020573-0845A", Cupo = 40, CupoOcupado = 2, HorasPorClase = 2, Estado = 0 });

        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-020573-0845A", GrupoCodigo = "G-IS101I", Periodo = "2026-I", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 1, 20), Estado = 0 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-150970-0004B", GrupoCodigo = "G-IS102I", Periodo = "2026-I", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 1, 20), Estado = 0 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-020573-0845A", GrupoCodigo = "G-IS201I", Periodo = "2026-I", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 1, 20), Estado = 0 });

        // ---------- Periodo 2026-II (actual; estado 1) ----------
        // Regular: diurno o vespertino entre semana.
        s.Grupos.Add(new Grupo { Codigo = "G-IS101R", CursoCodigo = "IS-101", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Lunes", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-020573-0845A", Cupo = 45, CupoOcupado = 1, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS201R", CursoCodigo = "IS-201", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Miércoles", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-020573-0845A", Cupo = 40, CupoOcupado = 1, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS301R", CursoCodigo = "IS-301", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Viernes", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-080775-0909D", Cupo = 35, CupoOcupado = 1, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS202R", CursoCodigo = "IS-202", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Viernes", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-150970-0004B", Cupo = 30, CupoOcupado = 0, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS302V", CursoCodigo = "IS-302", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Martes", HoraInicio = new TimeSpan(17, 0, 0), HoraFin = new TimeSpan(19, 0, 0), ProfesorCedula = "001-210488-0007B", Cupo = 35, CupoOcupado = 0, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS402R", CursoCodigo = "IS-402", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Jueves", HoraInicio = new TimeSpan(7, 0, 0), HoraFin = new TimeSpan(9, 0, 0), ProfesorCedula = "001-210488-0007B", Cupo = 30, CupoOcupado = 0, HorasPorClase = 2, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-CV201S", CursoCodigo = "CV-201", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Sábado", HoraInicio = new TimeSpan(8, 0, 0), HoraFin = new TimeSpan(12, 0, 0), ProfesorCedula = "001-150970-0004B", Cupo = 40, CupoOcupado = 0, HorasPorClase = 4, Estado = 1 });
        // Grupo SIN profesor: permite demostrar la asignación de clases y el conflicto de horario.
        s.Grupos.Add(new Grupo { Codigo = "G-IS303S", CursoCodigo = "IS-303", Periodo = "2026-II", Modalidad = Modalidad.Regular, Dia = "Sábado", HoraInicio = new TimeSpan(8, 0, 0), HoraFin = new TimeSpan(10, 0, 0), ProfesorCedula = null, Cupo = 30, CupoOcupado = 0, HorasPorClase = 2, Estado = 1 });

        // Dominical: clases por encuentro los domingos.
        s.Grupos.Add(new Grupo { Codigo = "G-IS101D", CursoCodigo = "IS-101", Periodo = "2026-II", Modalidad = Modalidad.Dominical, Dia = "Domingo", HoraInicio = new TimeSpan(8, 0, 0), HoraFin = new TimeSpan(12, 0, 0), ProfesorCedula = "001-031180-1012C", Cupo = 40, CupoOcupado = 0, HorasPorClase = 4, Estado = 1 });
        s.Grupos.Add(new Grupo { Codigo = "G-IS201D", CursoCodigo = "IS-201", Periodo = "2026-II", Modalidad = Modalidad.Dominical, Dia = "Domingo", HoraInicio = new TimeSpan(13, 0, 0), HoraFin = new TimeSpan(17, 0, 0), ProfesorCedula = "001-031180-1012C", Cupo = 35, CupoOcupado = 0, HorasPorClase = 4, Estado = 1 });

        // Contratos del periodo actual: dominical 60 h (C$ 24,000), regular 80 h (C$ 32,000).
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-020573-0845A", GrupoCodigo = "G-IS101R", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-020573-0845A", GrupoCodigo = "G-IS201R", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-031180-1012C", GrupoCodigo = "G-IS101D", Periodo = "2026-II", Horas = 60, TarifaHora = 400m, Salario = 24000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-031180-1012C", GrupoCodigo = "G-IS201D", Periodo = "2026-II", Horas = 60, TarifaHora = 400m, Salario = 24000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-080775-0909D", GrupoCodigo = "G-IS301R", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-210488-0007B", GrupoCodigo = "G-IS302V", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-210488-0007B", GrupoCodigo = "G-IS402R", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-150970-0004B", GrupoCodigo = "G-IS202R", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
        s.Contratos.Add(new Contrato { Id = s.NuevoIdContrato(), ProfesorCedula = "001-150970-0004B", GrupoCodigo = "G-CV201S", Periodo = "2026-II", Horas = 80, TarifaHora = 400m, Salario = 32000m, Fecha = new DateTime(2026, 8, 1), Estado = 1 });
    }

    // =====================================================================
    //  Estudiantes (datos ficticios con formatos reales)
    // =====================================================================
    private static void CargarEstudiantes(SistemaGestion s)
    {
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "David Donaire", Cedula = "001-120703-0004A", FechaNacimiento = new DateTime(2003, 7, 12),
            Genero = Genero.Masculino, Celular = "88547812", Correo = "ddonaire@gmail.com",
            Direccion = "Colonia Las Colinas, Managua", CarreraId = 1, Estado = 1,
            Usuario = "ddonaire", Contrasena = "David#2026"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Luis Alfonso Bustos Hernández", Cedula = "001-240398-0002B", FechaNacimiento = new DateTime(1998, 3, 24),
            Genero = Genero.Masculino, Celular = "88121234", Correo = "lbustos@gmail.com",
            Direccion = "Colonia Centroamérica, Managua", CarreraId = 1, Estado = 1,
            Usuario = "lbustos", Contrasena = "Luis#2026B"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "María Fernanda Silva Chávez", Cedula = "001-120402-0015A", FechaNacimiento = new DateTime(2002, 4, 12),
            Genero = Genero.Femenino, Celular = "85127654", Correo = "msilva@gmail.com",
            Direccion = "Barrio Martha Quezada, Managua", CarreraId = 1, Estado = 1,
            Usuario = "msilva", Contrasena = "Maria#2026"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Daniel José Zapata Membreño", Cedula = "001-050801-0021C", FechaNacimiento = new DateTime(2001, 8, 5),
            Genero = Genero.Masculino, Celular = "84234167", Correo = "dzapata@gmail.com",
            Direccion = "Colonia Miguel Bonilla, Managua", CarreraId = 1, Estado = 1,
            Usuario = "dzapata", Contrasena = "Daniel#26"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Kenny Arturo Mendoza Blanco", Cedula = "001-020900-0011A", FechaNacimiento = new DateTime(2000, 9, 2),
            Genero = Genero.Masculino, Celular = "87661290", Correo = "kmendoza@gmail.com",
            Direccion = "Barrio Camilo Ortega, Managua", CarreraId = 1, Estado = 1,
            Usuario = "kmendoza", Contrasena = "Kenny#2026"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Ana Fátima Torres Núñez", Cedula = "001-080505-0013B", FechaNacimiento = new DateTime(2005, 5, 8),
            Genero = Genero.Femenino, Celular = "83344560", Correo = "atorres@gmail.com",
            Direccion = "De la parroquia San Sebastián 1c al oeste, León", CarreraId = 4, Estado = 1,
            Usuario = "atorres", Contrasena = "Ana#2026F"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Scarleth Mariana Téllez Duarte", Cedula = "001-190705-0019B", FechaNacimiento = new DateTime(2005, 7, 19),
            Genero = Genero.Femenino, Celular = "81122334", Correo = "stellez@gmail.com",
            Direccion = "Km 4.5 Carretera a Masaya, Managua", CarreraId = 10, Estado = 1,
            Usuario = "stellez", Contrasena = "Scar#2026T"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Paula Andrea Rivas Meza", Cedula = "001-150903-0009B", FechaNacimiento = new DateTime(2003, 9, 15),
            Genero = Genero.Femenino, Celular = "88900123", Correo = "privas@gmail.com",
            Direccion = "Colonia Bello Horizonte, Managua", CarreraId = 7, Estado = 1,
            Usuario = "privas", Contrasena = "Paula#2026"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Xiomara del Socorro Palacios Cruz", Cedula = "001-011095-0020A", FechaNacimiento = new DateTime(1995, 10, 1),
            Genero = Genero.Femenino, Celular = "85443322", Correo = "xpalacios@gmail.com",
            Direccion = "Barrio Concepción, Managua", CarreraId = 6, Estado = 1,
            Usuario = "xpalacios", Contrasena = "Xiomara#26"
        });
        s.Estudiantes.Add(new Estudiante
        {
            Nombre = "Iván Antonio López Sequeira", Cedula = "001-230997-0018C", FechaNacimiento = new DateTime(1997, 9, 23),
            Genero = Genero.Masculino, Celular = "82006655", Correo = "ilopez@gmail.com",
            Direccion = "Colonia Morazán, Managua", CarreraId = 1, Estado = 0, // inactivo: no puede iniciar sesión
            Usuario = "ilopez", Contrasena = "Ivan#2026L"
        });
    }

    // =====================================================================
    //  Historial académico del periodo 2026-I (para los prerrequisitos)
    // =====================================================================
    private static void CargarHistorialAcademico(SistemaGestion s)
    {
        void Agregar(string cedula, string grupo, decimal acumulado, decimal examen, decimal? extraordinaria = null)
        {
            Inscripcion insc = new()
            {
                Id = s.NuevoIdInscripcion(),
                EstudianteCedula = cedula,
                GrupoCodigo = grupo,
                Periodo = "2026-I",
                Fecha = new DateTime(2026, 2, 15),
                Estado = 1
            };
            s.Inscripciones.Add(insc);

            Nota nota = new() { InscripcionId = insc.Id, Acumulado = acumulado, Examen = examen };
            if (extraordinaria.HasValue)
            {
                nota.NotaExtraordinaria = extraordinaria;
                nota.NotaAplicada = Math.Min(extraordinaria.Value, 80m); // tope de 80
            }
            s.Notas.Add(nota);
        }

        // David Donaire: aprobó Introducción a la Programación I → puede inscribir Programación I.
        Agregar("001-120703-0004A", "G-IS101I", 54, 34);   // 88 — Aprobado

        // Luis Bustos: aprobó IS-101, IS-102 y IS-201 → puede inscribir Programación II.
        Agregar("001-240398-0002B", "G-IS101I", 52, 34);   // 86 — Aprobado
        Agregar("001-240398-0002B", "G-IS102I", 45, 32);   // 77 — Aprobado
        Agregar("001-240398-0002B", "G-IS201I", 48, 30);   // 78 — Aprobado

        // María Silva: reprobó IS-101 con 35 → sin derecho a convocatoria (debe repetir el curso).
        Agregar("001-120402-0015A", "G-IS101I", 20, 15);   // 35 — Reprobado sin derecho

        // Daniel Zapata: 55 (35 + 20) → derecho a convocatoria; obtuvo 90 → tope 80 → aprobado.
        Agregar("001-050801-0021C", "G-IS101I", 35, 20, 90); // Aprobado (por convocatoria)

        // Kenny Mendoza: aprobó IS-101 e IS-201 → puede inscribir Programación II.
        Agregar("001-020900-0011A", "G-IS101I", 40, 22);   // 62 — Aprobado
        Agregar("001-020900-0011A", "G-IS201I", 42, 22);   // 64 — Aprobado
    }

    // =====================================================================
    //  Inscripciones del periodo actual (2026-II)
    // =====================================================================
    private static void CargarPeriodoActual(SistemaGestion s)
    {
        // Luis Bustos inscrito en Programación II (nota pendiente).
        s.Inscripciones.Add(new Inscripcion
        {
            Id = s.NuevoIdInscripcion(), EstudianteCedula = "001-240398-0002B",
            GrupoCodigo = "G-IS301R", Periodo = "2026-II", Fecha = new DateTime(2026, 8, 10), Estado = 1
        });

        // María Silva reinscrita en Introducción a la Programación I (nota pendiente).
        s.Inscripciones.Add(new Inscripcion
        {
            Id = s.NuevoIdInscripcion(), EstudianteCedula = "001-120402-0015A",
            GrupoCodigo = "G-IS101R", Periodo = "2026-II", Fecha = new DateTime(2026, 8, 10), Estado = 1
        });

        // Kenny Mendoza inscrito en Programación I con nota ya registrada (78).
        Inscripcion inscKenny = new()
        {
            Id = s.NuevoIdInscripcion(), EstudianteCedula = "001-020900-0011A",
            GrupoCodigo = "G-IS201R", Periodo = "2026-II", Fecha = new DateTime(2026, 8, 10), Estado = 1
        };
        s.Inscripciones.Add(inscKenny);
        s.Notas.Add(new Nota { InscripcionId = inscKenny.Id, Acumulado = 48, Examen = 30 }); // 78 — Aprobado
    }

    // =====================================================================
    //  Asistencias y pagos del periodo actual
    // =====================================================================
    private static void CargarAsistenciasYPagos(SistemaGestion s)
    {
        s.Asistencias.Add(new Asistencia
        {
            Id = s.NuevoIdAsistencia(), ProfesorCedula = "001-080775-0909D", GrupoCodigo = "G-IS301R",
            Fecha = new DateTime(2026, 10, 16), HoraMarcada = new TimeSpan(6, 55, 0), Presente = true,
            HorasDescontadas = 0, Estado = 1
        });
        // Ausencia: se descuentan 2 horas × C$ 400 = C$ 800 en la planilla.
        s.Asistencias.Add(new Asistencia
        {
            Id = s.NuevoIdAsistencia(), ProfesorCedula = "001-080775-0909D", GrupoCodigo = "G-IS301R",
            Fecha = new DateTime(2026, 10, 23), HoraMarcada = null, Presente = false,
            HorasDescontadas = 2, Estado = 1
        });
        s.Asistencias.Add(new Asistencia
        {
            Id = s.NuevoIdAsistencia(), ProfesorCedula = "001-210488-0007B", GrupoCodigo = "G-IS402R",
            Fecha = new DateTime(2026, 10, 15), HoraMarcada = new TimeSpan(6, 58, 0), Presente = true,
            HorasDescontadas = 0, Estado = 1
        });
        s.Asistencias.Add(new Asistencia
        {
            Id = s.NuevoIdAsistencia(), ProfesorCedula = "001-020573-0845A", GrupoCodigo = "G-IS101R",
            Fecha = new DateTime(2026, 10, 19), HoraMarcada = new TimeSpan(7, 0, 0), Presente = true,
            HorasDescontadas = 0, Estado = 1
        });
        s.Asistencias.Add(new Asistencia
        {
            Id = s.NuevoIdAsistencia(), ProfesorCedula = "001-031180-1012C", GrupoCodigo = "G-IS101D",
            Fecha = new DateTime(2026, 10, 18), HoraMarcada = new TimeSpan(7, 48, 0), Presente = true,
            HorasDescontadas = 0, Estado = 1
        });

        // Carlos Martínez ya recibió su pago del periodo (32,000 × 2 contratos = 64,000).
        s.Pagos.Add(new Pago
        {
            Id = s.NuevoIdPago(), ProfesorCedula = "001-020573-0845A", Periodo = "2026-II",
            Monto = 64000m, Fecha = new DateTime(2026, 10, 28), Estado = 1
        });

        // Fecha tope para que los profesores ingresen notas.
        s.FechaTopeNotas = new DateTime(2026, 12, 18);
    }
}
