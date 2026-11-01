namespace SistemaGestionEstudiantil.Excepciones;

/// <summary>
/// Excepción base del sistema. Toda excepción de negocio hereda de ella,
/// lo que permite a la interfaz capturarla, mostrar el mensaje al usuario
/// y continuar sin interrumpir la aplicación.
/// </summary>
public class AppException : Exception
{
    public AppException(string mensaje) : base(mensaje) { }
}

/// <summary>Se lanza cuando un usuario intenta realizar una acción que su rol no permite.</summary>
public class AccesoDenegadoException : AppException
{
    public AccesoDenegadoException(string mensaje) : base(mensaje) { }
}

/// <summary>Usuario o contraseña incorrectos al iniciar sesión.</summary>
public class CredencialesInvalidasException : AppException
{
    public CredencialesInvalidasException(string mensaje) : base(mensaje) { }
}

/// <summary>El usuario no cumple el formato (obligatorio, sin espacios, único).</summary>
public class UsuarioInvalidoException : AppException
{
    public UsuarioInvalidoException(string mensaje) : base(mensaje) { }
}

/// <summary>La contraseña no cumple el mínimo de 8 caracteres con mayúscula, minúscula, número y símbolo.</summary>
public class ContrasenaInvalidaException : AppException
{
    public ContrasenaInvalidaException(string mensaje) : base(mensaje) { }
}

/// <summary>La cédula no tiene el formato nicaragüense XXX-XXXXXX-XXXXL.</summary>
public class CedulaInvalidaException : AppException
{
    public CedulaInvalidaException(string mensaje) : base(mensaje) { }
}

/// <summary>La cédula ya está registrada (como estudiante o como profesor).</summary>
public class CedulaDuplicadaException : AppException
{
    public CedulaDuplicadaException(string mensaje) : base(mensaje) { }
}

/// <summary>El correo electrónico no tiene un formato válido.</summary>
public class CorreoInvalidoException : AppException
{
    public CorreoInvalidoException(string mensaje) : base(mensaje) { }
}

/// <summary>El correo ya pertenece a otro estudiante.</summary>
public class CorreoDuplicadoException : AppException
{
    public CorreoDuplicadoException(string mensaje) : base(mensaje) { }
}

/// <summary>El celular no tiene el formato de Nicaragua (8 dígitos, inicia con 5, 7 u 8).</summary>
public class CelularInvalidoException : AppException
{
    public CelularInvalidoException(string mensaje) : base(mensaje) { }
}

/// <summary>Cualquier dato que no cumple una regla general del sistema.</summary>
public class DatoInvalidoException : AppException
{
    public DatoInvalidoException(string mensaje) : base(mensaje) { }
}

/// <summary>Intento de crear un registro que ya existe (código, usuario, grupo con profesor, etc.).</summary>
public class RegistroDuplicadoException : AppException
{
    public RegistroDuplicadoException(string mensaje) : base(mensaje) { }
}

/// <summary>Falta aprobar un prerrequisito para inscribir un curso.</summary>
public class PrerrequisitoPendienteException : AppException
{
    public PrerrequisitoPendienteException(string mensaje) : base(mensaje) { }
}

/// <summary>El estudiante ya aprobó el curso e intenta inscribirlo de nuevo.</summary>
public class CursoYaAprobadoException : AppException
{
    public CursoYaAprobadoException(string mensaje) : base(mensaje) { }
}

/// <summary>El estudiante ya está inscrito en el curso en el mismo periodo.</summary>
public class InscripcionDuplicadaException : AppException
{
    public InscripcionDuplicadaException(string mensaje) : base(mensaje) { }
}

/// <summary>Choque de horario entre grupos (del estudiante o del profesor).</summary>
public class HorarioConflictoException : AppException
{
    public HorarioConflictoException(string mensaje) : base(mensaje) { }
}

/// <summary>El grupo elegido no tiene cupo disponible.</summary>
public class CupoInsuficienteException : AppException
{
    public CupoInsuficienteException(string mensaje) : base(mensaje) { }
}

/// <summary>La fecha tope definida por el administrador para ingresar notas ya venció.</summary>
public class FechaTopeVencidaException : AppException
{
    public FechaTopeVencidaException(string mensaje) : base(mensaje) { }
}

/// <summary>Intento de registrar una nota que ya fue grabada (las notas no se editan).</summary>
public class NotaYaRegistradaException : AppException
{
    public NotaYaRegistradaException(string mensaje) : base(mensaje) { }
}

/// <summary>El estudiante ya realizó su única convocatoria en esa clase.</summary>
public class ConvocatoriaYaRealizadaException : AppException
{
    public ConvocatoriaYaRealizadaException(string mensaje) : base(mensaje) { }
}

/// <summary>El estudiante no cumple los requisitos para presentarse a convocatoria.</summary>
public class SinDerechoConvocatoriaException : AppException
{
    public SinDerechoConvocatoriaException(string mensaje) : base(mensaje) { }
}

/// <summary>El profesor intentó marcar asistencia después de los 15 minutos hábiles.</summary>
public class AsistenciaFueraDeTiempoException : AppException
{
    public AsistenciaFueraDeTiempoException(string mensaje) : base(mensaje) { }
}

/// <summary>El pago no coincide con la planilla o ya fue realizado.</summary>
public class PagoInvalidoException : AppException
{
    public PagoInvalidoException(string mensaje) : base(mensaje) { }
}
