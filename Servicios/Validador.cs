using System.Text.RegularExpressions;
using SistemaGestionEstudiantil.Excepciones;

namespace SistemaGestionEstudiantil.Servicios;

/// <summary>
/// Validaciones de formato según las reglas del enunciado y los formatos
/// reales usados en Nicaragua (cédula, celular, correo).
/// </summary>
public static class Validador
{
    // Usuario: obligatorio y sin espacios.
    private static readonly Regex RxUsuario = new("^[^\\s]+$", RegexOptions.Compiled);

    // Cédula nicaragüense: 3 dígitos - 6 dígitos - 4 dígitos + letra. Ej.: 001-240398-0002B
    private static readonly Regex RxCedula = new("^\\d{3}-\\d{6}-\\d{4}[A-Z]$", RegexOptions.Compiled);

    // Celular de Nicaragua: 8 dígitos, inicia con 5, 7 u 8.
    private static readonly Regex RxCelular = new("^[578]\\d{7}$", RegexOptions.Compiled);

    // Correo: forma básica usuario@dominio.ext
    private static readonly Regex RxCorreo = new("^[\\w.\\-+]+@[\\w\\-]+(\\.[\\w\\-]+)+$", RegexOptions.Compiled);

    public static void ValidarUsuario(string usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario) || !RxUsuario.IsMatch(usuario))
            throw new UsuarioInvalidoException("El usuario es obligatorio y no puede contener espacios.");
    }

    public static void ValidarContrasena(string contrasena)
    {
        bool valida = !string.IsNullOrEmpty(contrasena)
            && contrasena.Length >= 8
            && contrasena.Any(char.IsUpper)
            && contrasena.Any(char.IsLower)
            && contrasena.Any(char.IsDigit)
            && contrasena.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));

        if (!valida)
            throw new ContrasenaInvalidaException(
                "La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula, número y símbolo.");
    }

    public static void ValidarCedula(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula) || !RxCedula.IsMatch(cedula.Trim()))
            throw new CedulaInvalidaException(
                "La cédula debe tener el formato nicaragüense XXX-XXXXXX-XXXXL (ejemplo: 001-240398-0002B).");
    }

    public static void ValidarCelular(string celular)
    {
        if (string.IsNullOrWhiteSpace(celular) || !RxCelular.IsMatch(celular.Trim()))
            throw new CelularInvalidoException(
                "El celular debe tener 8 dígitos e iniciar con 5, 7 u 8 (ejemplo: 88121234).");
    }

    public static void ValidarCorreo(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo) || !RxCorreo.IsMatch(correo.Trim()))
            throw new CorreoInvalidoException(
                "El correo electrónico no tiene un formato válido (ejemplo: nombre@dominio.com).");
    }

    public static void ValidarTextoObligatorio(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DatoInvalidoException($"El campo {campo} es obligatorio.");
    }
}
