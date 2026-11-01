namespace SistemaGestionEstudiantil.Modelos;

public enum Rol { Administrador, Profesor, Estudiante }

public enum Genero { Masculino, Femenino }

/// <summary>
/// Datos comunes de cualquier usuario del sistema.
/// IMPORTANTE: el campo Estado maneja 1 = activo y 0 = inactivo.
/// El sistema NO elimina registros; únicamente cambia su estado
/// (indicación del docente: no se utiliza DELETE).
/// </summary>
public abstract class Persona
{
    public string Nombre { get; set; }
    public string Cedula { get; set; }
    public string Usuario { get; set; }
    public string Contrasena { get; set; }
    public string Celular { get; set; }
    public string Correo { get; set; }
    public string Direccion { get; set; }
    public Genero Genero { get; set; }
    public int Estado { get; set; } = 1; // 1 = activo, 0 = inactivo (sustituye a DELETE)

    public Rol Rol { get; protected set; }

    public string EstadoTexto => Estado == 1 ? "Activo (1)" : "Inactivo (0)";
}

/// <summary>Estudiante registrado por el administrador.</summary>
public class Estudiante : Persona
{
    public DateTime FechaNacimiento { get; set; }
    public int CarreraId { get; set; }

    public Estudiante()
    {
        Rol = Rol.Estudiante;
    }
}

/// <summary>Profesor registrado por el administrador y con clases asignadas.</summary>
public class Profesor : Persona
{
    public string EstadoCivil { get; set; }
    public string Profesion { get; set; }
    public string Especialidad { get; set; } // acepta valor nulo

    public Profesor()
    {
        Rol = Rol.Profesor;
    }
}

/// <summary>Usuario administrador que gestiona el sistema.</summary>
public class Administrador : Persona
{
    public Administrador()
    {
        Rol = Rol.Administrador;
    }
}
