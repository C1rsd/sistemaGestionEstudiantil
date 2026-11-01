using System.Globalization;
using SistemaGestionEstudiantil.Excepciones;

namespace SistemaGestionEstudiantil.UI;

/// <summary>
/// Utilidades de lectura y escritura por consola. Los métodos de lectura
/// repiten la petición hasta obtener un dato válido; LeerNota lanza una
/// excepción cuando el valor no es numérico o está fuera de rango, tal
/// como lo pide el enunciado.
/// </summary>
public static class Consola
{
    /// <summary>Formatea una hora como HH:mm (evita problemas de escape de formatos).</summary>
    public static string Hora(TimeSpan t) => $"{t.Hours:D2}:{t.Minutes:D2}";

    /// <summary>Recorta un texto al ancho indicado para que las tablas no se deformen.</summary>
    public static string Recortar(string texto, int max)
    {
        if (string.IsNullOrEmpty(texto)) return "";
        return texto.Length <= max ? texto : texto.Substring(0, Math.Max(0, max - 1)) + "…";
    }

    public static void Linea(string texto = "") => Console.WriteLine(texto);

    public static void Titulo(string texto)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================================================");
        Console.WriteLine("  " + texto);
        Console.WriteLine("====================================================================");
        Console.ResetColor();
    }

    public static void Subtitulo(string texto)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("-- " + texto + " --");
        Console.ResetColor();
    }

    public static void Error(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[X] " + mensaje);
        Console.ResetColor();
    }

    public static void Exito(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[OK] " + mensaje);
        Console.ResetColor();
    }

    public static void Pausa()
    {
        Console.WriteLine();
        Console.Write("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    public static string LeerTexto(string mensaje, bool obligatorio = true)
    {
        while (true)
        {
            Console.Write(mensaje);
            string t = Console.ReadLine();
            if (t == null) continue;
            t = t.Trim();
            if (!obligatorio) return t;
            if (t.Length > 0) return t;
            Console.WriteLine("  [!] Este dato es obligatorio.");
        }
    }

    public static int LeerOpcion(string mensaje, int min, int max)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out int n) && n >= min && n <= max) return n;
            Console.WriteLine($"  [!] Ingrese un número entre {min} y {max}.");
        }
    }

    public static int LeerEntero(string mensaje, int min, int max) => LeerOpcion(mensaje, min, max);

    public static bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (s/n): ");
        string r = Console.ReadLine();
        if (r == null) return false;
        r = r.Trim().ToLower();
        return r == "s" || r == "si" || r == "sí";
    }

    public static DateTime LeerFecha(string mensaje)
    {
        string[] formatos = { "dd/MM/yyyy", "d/M/yyyy" };
        while (true)
        {
            Console.Write(mensaje + " (dd/mm/aaaa): ");
            string t = Console.ReadLine();
            if (t != null
                && DateTime.TryParseExact(t.Trim(), formatos, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out DateTime fecha))
                return fecha;
            Console.WriteLine("  [!] Fecha inválida. Ejemplo: 15/03/2004");
        }
    }

    public static TimeSpan LeerHora(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje + " (hh:mm): ");
            string t = Console.ReadLine();
            if (t != null
                && TimeSpan.TryParseExact(t.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan hora))
                return hora;
            Console.WriteLine("  [!] Hora inválida. Ejemplo: 07:30");
        }
    }

    /// <summary>
    /// Lee un valor numérico obligatorio para notas: si no es numérico o está
    /// fuera de rango lanza DatoInvalidoException (el llamador la captura
    /// y vuelve a pedir el dato, como exige el enunciado).
    /// </summary>
    public static decimal LeerNota(string mensaje, decimal min, decimal max)
    {
        Console.Write(mensaje);
        string t = Console.ReadLine();
        if (!decimal.TryParse((t ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor))
            throw new DatoInvalidoException("El valor ingresado no es numérico.");
        if (valor < min || valor > max)
            throw new DatoInvalidoException($"El valor debe estar entre {min} y {max}.");
        return valor;
    }

    public static decimal LeerMonto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string t = Console.ReadLine();
            if (t != null
                && decimal.TryParse(t.Trim().Replace(',', '.'), NumberStyles.Number,
                                    CultureInfo.InvariantCulture, out decimal m)
                && m >= 0)
                return m;
            Console.WriteLine("  [!] Monto inválido. Ejemplo: 32000");
        }
    }
}
