using SistemaGestionEstudiantil.Datos;
using SistemaGestionEstudiantil.Servicios;
using SistemaGestionEstudiantil.UI;

namespace SistemaGestionEstudiantil;

/// <summary>
/// Sistema de Gestión Estudiantil para universidades nicaragüenses.
/// Aplicación de consola en C# (.NET 8) con autenticación por roles,
/// inscripciones con prerrequisitos, calificaciones con convocatoria,
/// asistencia docente, contratos y planilla de pagos.
/// </summary>
public static class Program
{
    public static void Main()
    {
        try
        {
            Console.Title = "Sistema de Gestión Estudiantil — Universidades de Nicaragua";
        }
        catch { /* plataformas sin título de consola */ }

        SistemaGestion sistema = new();
        DatosIniciales.Cargar(sistema);
        new Menus(sistema).Ejecutar();
    }
}
