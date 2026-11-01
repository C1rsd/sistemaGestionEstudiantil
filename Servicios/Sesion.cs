using SistemaGestionEstudiantil.Excepciones;
using SistemaGestionEstudiantil.Modelos;

namespace SistemaGestionEstudiantil.Servicios;

/// <summary>
/// Sesión activa del sistema. Guarda el usuario autenticado y permite
/// exigir un rol antes de ejecutar una acción; si el rol no corresponde,
/// se lanza la excepción de acceso denegado que pide el enunciado.
/// </summary>
public class Sesion
{
    public Persona Usuario { get; private set; }

    public bool Activa => Usuario != null;

    public void Iniciar(Persona usuario) => Usuario = usuario;

    public void Cerrar() => Usuario = null;

    public void ExigirRol(Rol rolRequerido)
    {
        if (Usuario == null || Usuario.Rol != rolRequerido)
            throw new AccesoDenegadoException(
                $"Acceso denegado: esta acción corresponde únicamente al rol de {rolRequerido}.");
    }
}
