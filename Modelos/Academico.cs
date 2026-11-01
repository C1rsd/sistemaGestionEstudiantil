namespace SistemaGestionEstudiantil.Modelos;

/// <summary>
/// Universidad nicaragüense del catálogo (datos reales: miembros del CNU
/// y universidades privadas reconocidas).
/// </summary>
public class Universidad
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Ciudad { get; set; }
    public string Tipo { get; set; }         // Pública / Privada
    public int? AnioFundacion { get; set; }  // solo cuando el dato está verificado
}

/// <summary>Carrera o programa de estudio de una universidad.</summary>
public class Carrera
{
    public int Id { get; set; }
    public int UniversidadId { get; set; }
    public string Nombre { get; set; }
    public string Duracion { get; set; }     // por ejemplo: "10 semestres"
    public int Estado { get; set; } = 1;

    public string EstadoTexto => Estado == 1 ? "Activo (1)" : "Inactivo (0)";
}

/// <summary>
/// Asignatura del pénsum. El campo Prerrequisitos contiene los códigos
/// de los cursos que deben estar aprobados antes de poder inscribirlo.
/// </summary>
public class Curso
{
    public string Codigo { get; set; }
    public int CarreraId { get; set; }
    public string Nombre { get; set; }
    public int Creditos { get; set; }
    public List<string> Prerrequisitos { get; set; } = new List<string>();
    public int Estado { get; set; } = 1;

    public string EstadoTexto => Estado == 1 ? "Activo (1)" : "Inactivo (0)";
}

public enum Modalidad { Regular, Dominical }

/// <summary>
/// Grupo o sección de un curso en un periodo académico.
/// Modalidades reales usadas en Nicaragua: regular (diurno, vespertino o
/// sabatino) y dominical (clases por encuentro los domingos).
/// </summary>
public class Grupo
{
    public string Codigo { get; set; }
    public string CursoCodigo { get; set; }
    public string Periodo { get; set; }          // por ejemplo: 2026-II
    public Modalidad Modalidad { get; set; }
    public string Dia { get; set; }              // Lunes ... Domingo
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string ProfesorCedula { get; set; }   // null si todavía no tiene docente
    public int Cupo { get; set; }
    public int CupoOcupado { get; set; } = 0;
    public int HorasPorClase { get; set; }       // horas de cada clase (para descuentos por ausencia)
    public int Estado { get; set; } = 1;

    public int CupoDisponible => Cupo - CupoOcupado;
    public string EstadoTexto => Estado == 1 ? "Activo (1)" : "Inactivo (0)";
    public string HorarioTexto => $"{Dia} {HoraInicio.Hours:D2}:{HoraInicio.Minutes:D2}-{HoraFin.Hours:D2}:{HoraFin.Minutes:D2}";
}
