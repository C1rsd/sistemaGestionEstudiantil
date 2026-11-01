# Sistema de Gestión Estudiantil — C# (aplicación de consola)

Sistema de gestión estudiantil para universidades nicaragüenses, desarrollado en **C# (.NET 8)** como aplicación de consola. Implementa el enunciado completo del ejercicio: acceso por roles, validaciones de registro, asignación de clases con contrato, inscripción con prerrequisitos, calificaciones con convocatoria, asistencia docente y planilla de pagos.

Como indicación del docente, **no se utiliza DELETE**: todos los registros manejan un campo de estado con **1 = activo** y **0 = inactivo**, y el sistema incluye funcionalidades de **agregar, editar y listar** en los módulos correspondientes.

---

## 1. Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0) o superior (Windows, macOS o Linux).
- Opcional: [Visual Studio Code](https://code.visualstudio.com/) con la extensión *C# Dev Kit*.

Verifique la instalación desde la terminal:

~~~bash
dotnet --version
~~~

## 2. Cómo ejecutar el proyecto

Opción A — Terminal (Windows, macOS o Linux):

~~~bash
cd SistemaGestionEstudiantil
dotnet run
~~~

Opción B — VS Code:

1. Abra la carpeta `SistemaGestionEstudiantil` en VS Code.
2. Abra la terminal integrada (`Ctrl + Ñ` o Terminal → Nueva terminal).
3. Ejecute `dotnet run`.

La primera ejecución restaura y compila automáticamente el proyecto.

## 3. Cuentas de prueba

### Administrador

| Usuario | Contraseña |
|---------|-----------|
| admin   | Admin#2026 |

### Estudiantes (carrera: Ingeniería de Sistemas — UNI)

| Usuario   | Contraseña   | Situación académica |
|-----------|--------------|---------------------|
| ddonaire  | David#2026   | Aprobó Introducción a la Programación I (88) → puede inscribir Programación I |
| lbustos   | Luis#2026B   | Aprobó IS-101, IS-102 e IS-201 → puede inscribir Programación II; ya está inscrito en G-IS301R (nota pendiente) |
| msilva    | Maria#2026   | Reprobó IS-101 con 35 (sin derecho a convocatoria) → reinscrita en G-IS101R |
| dzapata   | Daniel#26    | Aprobó IS-101 por convocatoria (obtuvo 90 → quedó en 80) |
| kmendoza  | Kenny#2026   | Inscrito en G-IS201R con nota 78 ya registrada |
| ilopez    | Ivan#2026L   | **Inactivo (0)** → el sistema le niega el inicio de sesión |

Otros estudiantes (de carreras distintas) para mostrar el catálogo multiuniversidad: atorres (Medicina, UNAN-León), stellez (Ing. en Sistemas, UdeM), privas (Administración de Empresas, UCA), xpalacios (Contaduría, UNAN-Managua).

### Profesores

| Usuario   | Contraseña    | Grupos asignados |
|-----------|---------------|------------------|
| cmartinez | Clave#2026    | G-IS101R, G-IS201R (regular) — ya cobró su pago |
| mlopez    | Martha#2026   | G-IS101D, G-IS201D (dominical) |
| jrivera   | Jose#2026R    | G-IS202R, G-CV201S |
| fzamora   | Favio#2026    | G-IS301R — tiene una ausencia (C$ 800 de descuento) |
| knunez    | Karla#2026    | G-IS302V, G-IS402R |
| asanchez  | Ana#2026S     | sin grupos (para demostrar la asignación de clases) |

## 4. Datos basados en la vida real

- Catálogo de universidades nicaragüenses reales (miembros del CNU y privadas reconocidas): UNAN-Managua, UNAN-León, UNI, UCA, UNA, UdeM, UCYT y BICU.
- Pénsum real de Ingeniería de Sistemas de la UNI (Facultad de Ciencias y Sistemas): Introducción a la Programación I, Programación I y II, Base de Datos I y II, Ingeniería de Software I y II, Sistemas Operativos, Redes de Computadoras, Inteligencia Artificial, Matemáticas, Estadística.
- Códigos reales de la UCYT (pénsum 2026-2030): IF-3900, IF-1100, IF-1400.
- Modalidades reales: **regular** (diurno/vespertino/sabatino) y **dominical** (clases por encuentro los domingos).
- Formato real de cédula nicaragüense (XXX-XXXXXX-XXXXL), celulares de 8 dígitos y periodos tipo 2026-II.
- Salarios del enunciado, coherentes con la escala docente: C$ 400 por hora; 60 horas en dominical (C$ 24,000) y 80 horas en regular (C$ 32,000).

Los nombres de personas, cédulas y celulares son ficticios, creados solo para la demostración.

## 5. Ruta de demostración sugerida (para presentar el trabajo)

1. **Login**: pruebe con `ilopez` (estudiante inactivo → excepción de acceso denegado).
2. **Estudiante** (`ddonaire` / `David#2026`):
   - Inscribir clases → elija IS-301 → se bloquea: falta el prerrequisito Programación I.
   - Inscribir clases → elija IS-201 → grupo G-IS201R → confirme → comprobante.
3. **Estudiante** (`lbustos` / `Luis#2026B`):
   - Ver mis notas → muestra "Nota pendiente" en Programación II.
   - Intente inscribir G-IS202R (Viernes 07:00-09:00) → conflicto de horario con G-IS301R.
4. **Profesor** (`fzamora` / `Favio#2026`):
   - Registrar notas → G-IS301R → ingrese (por ejemplo) 35 y 20 para Luis Bustos → nota final 55 → convocatoria.
   - La opción 4 se habilita → registrar convocatoria → ingrese 90 → el sistema aplica el tope de 80 → aprobado.
   - Intente registrar de nuevo la misma nota → excepción.
   - Marcar asistencia → simule una entrada a las 07:20 → excepción (solo hay 15 minutos hábiles); regístrela como ausencia y vea el descuento en su planilla.
5. **Administrador** (`admin` / `Admin#2026`):
   - Listar universidades, carreras, cursos y grupos (información real).
   - Registre un estudiante con datos inválidos (usuario con espacios, cédula mal formateada, contraseña débil, cédula duplicada) → observe cada excepción.
   - Asigne el grupo G-IS303S a `jrivera` → excepción de conflicto de horario (ya tiene G-CV201S el sábado de 08:00 a 12:00); luego asígnelo a `asanchez` → se genera el contrato.
   - Genere la planilla de pagos → valide el pago de fzamora con monto incorrecto (excepción) y con el monto exacto (C$ 31,200).
   - Intente pagar de nuevo a cmartinez → excepción (ya cobró).

## 6. Requisitos del enunciado implementados

| Requisito | Implementación |
|---|---|
| Acceso con usuario y contraseña por roles | `Servicios/Sesion.cs`, `SistemaGestion.Login` |
| Excepción de acceso denegado por rol | `AccesoDenegadoException` |
| Estudiante: solo inscribir clases y ver notas | Menú de estudiante (2 módulos + perfil) |
| Profesor: grupos, notas, convocatoria, asistencia, contrato y planilla | Menú de profesor (opciones 1-8) |
| Administrador: registro, asignación, fecha tope, planilla y pagos | Menú de administrador (opciones 1-18) |
| Validaciones de usuario, contraseña y cédula | `Servicios/Validador.cs` |
| Usuario único y sin espacios | `UsuarioInvalidoException` |
| Contraseña: 8+, mayúscula, minúscula, número y símbolo | `ContrasenaInvalidaException` |
| Cédula con formato válido y única (ni estudiante ni profesor) | `CedulaInvalidaException`, `CedulaDuplicadaException` |
| Registro de profesores con 9 campos (especialidad nula) | `RegistrarProfesor` |
| Registro de estudiantes con todos los datos obligatorios | `RegistrarEstudiante` |
| Solo estudiantes activos inician sesión | `SistemaGestion.Login` |
| Asignación con contrato (grupo, horas, horario y salario) | `SistemaGestion.AsignarClase` |
| Conflicto de día y horario del profesor | `HorarioConflictoException` |
| Salario: C$ 400/h; dominical 60 h = C$ 24,000; regular 80 h = C$ 32,000 | `AsignarClase` |
| Inscripción con periodo, curso, prerrequisitos, grupo, cupo y comprobante | `UI/Menus.InscribirClases` |
| Prerrequisitos aprobado/pendiente y bloqueo con excepción | `PrerrequisitoPendienteException` |
| No repetir curso aprobado ni inscripción en el mismo periodo | `CursoYaAprobadoException`, `InscripcionDuplicadaException` |
| Conflicto de horario del estudiante | `HorarioConflictoException` |
| Nota: 60 acumulado + 40 examen; aprueba con 60 | `Modelos/Nota.cs` |
| Estados: aprobado / convocatoria (40-59) / sin derecho (<40) | `Nota.EstadoBase` |
| Convocatoria 0-100 con tope de 80 y una sola por clase | `RegistrarConvocatoria` |
| Notas no editables; excepción al re-registrar | `NotaYaRegistradaException` |
| "Nota pendiente" y resumen de aprobadas/reprobadas | `VerMisNotas`, `MostrarNotasDelGrupo` |
| Fecha tope para notas y convocatorias | `FechaTopeVencidaException` |
| Asistencia con 15 minutos hábiles y ausencias | `MarcarAsistencia`, `RegistrarAusencia` |
| Descuento de C$ 400 por hora de clase perdida | `GenerarPlanilla` |
| Planilla con salario, descuentos y neto; pago validado y único | `GenerarPlanilla`, `RegistrarPago` |
| Indicación del docente: agregar, editar y listar; estado 1/0 en lugar de DELETE | Menús de administración y campo `Estado` en cada modelo |

## 7. Estructura del proyecto

~~~text
SistemaGestionEstudiantil/
├── SistemaGestionEstudiantil.csproj   # proyecto .NET 8 (dotnet run)
├── Program.cs                         # punto de entrada
├── README.md                          # este archivo
├── Excepciones/
│   └── Excepciones.cs                 # excepciones de negocio del enunciado
├── Modelos/
│   ├── Personas.cs                    # Persona, Estudiante, Profesor, Administrador
│   ├── Academico.cs                   # Universidad, Carrera, Curso, Grupo
│   └── Registros.cs                   # Inscripción, Nota, Contrato, Asistencia, Pago
├── Datos/
│   └── DatosIniciales.cs              # datos reales de universidades nicaragüenses
├── Servicios/
│   ├── Validador.cs                  # validaciones de formato
│   ├── Sesion.cs                     # sesión y control de acceso por rol
│   └── SistemaGestion.cs              # reglas de negocio
└── UI/
    ├── Consola.cs                     # utilidades de consola
    └── Menus.cs                       # menús y flujos por rol
~~~
