namespace SistemaGestionEstudiantil.Modelos;

/// <summary>Inscripción de un estudiante en un grupo de un periodo académico.</summary>
public class Inscripcion
{
    public int Id { get; set; }
    public string EstudianteCedula { get; set; }
    public string GrupoCodigo { get; set; }
    public string Periodo { get; set; }
    public DateTime Fecha { get; set; }
    public int Estado { get; set; } = 1;
}

/// <summary>
/// Nota final de un estudiante en una inscripción.
/// La nota final es de 100 puntos: 60 de acumulado y 40 de examen.
/// Se aprueba con 60 o más; entre 40 y 59 hay derecho a convocatoria.
/// </summary>
public class Nota
{
    public int InscripcionId { get; set; }
    public decimal Acumulado { get; set; }          // 0 a 60
    public decimal Examen { get; set; }             // 0 a 40
    public decimal? NotaExtraordinaria { get; set; } // convocatoria: 0 a 100
    public decimal? NotaAplicada { get; set; }       // convocatoria con tope de 80

    /// <summary>Nota final del curso: acumulado + examen.</summary>
    public decimal Final => Acumulado + Examen;

    public bool TieneConvocatoria => NotaAplicada.HasValue;

    /// <summary>Nota que queda acreditada: la de convocatoria (tope 80) o, si no hubo, la final.</summary>
    public decimal NotaFinalEfectiva => NotaAplicada ?? Final;

    /// <summary>Estado según la nota final, antes de la convocatoria.</summary>
    public string EstadoBase
    {
        get
        {
            if (Final >= 60) return "Aprobado";
            if (Final >= 40) return "Reprobado con derecho a convocatoria";
            return "Reprobado sin derecho a convocatoria";
        }
    }

    /// <summary>Estado definitivo, considerando la convocatoria si ya se realizó.</summary>
    public string EstadoFinal
    {
        get
        {
            if (TieneConvocatoria)
                return NotaAplicada >= 60 ? "Aprobado (por convocatoria)" : "Reprobado definitivo";
            return EstadoBase;
        }
    }

    public bool Aprobado => NotaFinalEfectiva >= 60;

    /// <summary>Verdadero si el estudiante tiene nota final entre 40 y 59 y aún no hizo convocatoria.</summary>
    public bool DerechoConvocatoria => !TieneConvocatoria && Final >= 40 && Final < 60;
}

/// <summary>
/// Contrato del profesor generado al asignarle una clase.
/// Tarifa: C$ 400 por hora. Modalidad dominical: 60 horas (C$ 24,000).
/// Modalidad regular: 80 horas (C$ 32,000).
/// </summary>
public class Contrato
{
    public int Id { get; set; }
    public string ProfesorCedula { get; set; }
    public string GrupoCodigo { get; set; }
    public string Periodo { get; set; }
    public int Horas { get; set; }
    public decimal TarifaHora { get; set; } = 400m;
    public decimal Salario { get; set; }
    public DateTime Fecha { get; set; }
    public int Estado { get; set; } = 1;
}

/// <summary>Registro de asistencia (o ausencia) del profesor a una clase.</summary>
public class Asistencia
{
    public int Id { get; set; }
    public string ProfesorCedula { get; set; }
    public string GrupoCodigo { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? HoraMarcada { get; set; }
    public bool Presente { get; set; }
    public decimal HorasDescontadas { get; set; } // horas que se descontarán del salario
    public int Estado { get; set; } = 1;
}

/// <summary>Pago validado de la planilla de un profesor en un periodo.</summary>
public class Pago
{
    public int Id { get; set; }
    public string ProfesorCedula { get; set; }
    public string Periodo { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public int Estado { get; set; } = 1;
}

/// <summary>Línea de la planilla de pagos de un periodo.</summary>
public class LineaPlanilla
{
    public Profesor Profesor { get; set; }
    public decimal SalarioBase { get; set; }
    public decimal Descuentos { get; set; }
    public int Ausencias { get; set; }

    public decimal SalarioNeto => SalarioBase - Descuentos;
}
