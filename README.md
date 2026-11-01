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
cd sistemaGestionEstudiantil
dotnet run
~~~

Opción B — VS Code:

1. Abra la carpeta del proyecto en VS Code.
2. Abra la terminal integrada (`Ctrl + Ñ` o Terminal → Nueva terminal).
3. Ejecute `dotnet run`.

La primera ejecución restaura y compila automáticamente el proyecto.

## 3. Especificaciones del sistema

Requisitos funcionales del enunciado que el sistema cumple, con el componente que implementa cada uno:

| N. | Requisito | Componente |
|---|---|---|
| 1 | Acceso con usuario y contraseña por roles (administrador, profesor, estudiante) | `Servicios/Sesion.cs`, `SistemaGestion.Login` |
| 2 | Acción fuera del rol → excepción de acceso denegado | `AccesoDenegadoException` |
| 3 | Estudiante: únicamente inscribir clases y ver notas | Menú de estudiante |
| 4 | Profesor: ver grupos, registrar notas y convocatorias, marcar asistencia, ver contrato y planilla | Menú de profesor (opciones 1-8) |
| 5 | Administrador: registrar personas, gestionar catálogos, asignar clases, fecha tope, planilla y pagos | Menú de administrador (opciones 1-18) |
| 6 | Usuario obligatorio, único y sin espacios | `Validador.ValidarUsuario`, `UsuarioInvalidoException` |
| 7 | Contraseña: mínimo 8 caracteres, con mayúscula, minúscula, número y símbolo | `Validador.ValidarContrasena`, `ContrasenaInvalidaException` |
| 8 | Cédula con formato nicaragüense (XXX-XXXXXX-XXXXL) y única en el sistema | `CedulaInvalidaException`, `CedulaDuplicadaException` |
| 9 | Registro de profesor con 9 campos (especialidad acepta nulo) | `SistemaGestion.RegistrarProfesor` |
| 10 | Registro de estudiante con todos los datos obligatorios | `SistemaGestion.RegistrarEstudiante` |
| 11 | Solo usuarios activos (estado 1) inician sesión | `SistemaGestion.Login` |
| 12 | Asignación de clase con contrato: grupo, horas, horario y salario | `SistemaGestion.AsignarClase`, `Contrato` |
| 13 | Conflicto de día y horario del profesor → excepción | `HorarioConflictoException` |
| 14 | Salario: C$ 400 por hora; dominical 60 h = C$ 24,000; regular 80 h = C$ 32,000 | `AsignarClase` |
| 15 | Inscripción con periodo, curso, prerrequisitos, grupo, cupo y comprobante | `UI/Menus.InscribirClases` |
| 16 | Prerrequisito pendiente → bloqueo con excepción | `PrerrequisitoPendienteException` |
| 17 | No inscribir un curso ya aprobado ni dos veces en el mismo periodo | `CursoYaAprobadoException`, `InscripcionDuplicadaException` |
| 18 | Conflicto de horario entre grupos del estudiante → excepción | `HorarioConflictoException` |
| 19 | Nota final de 100: 60 de acumulado + 40 de examen; aprueba con 60 | `Modelos/Nota.cs` |
| 20 | Estados: aprobado (60+), convocatoria (40-59), sin derecho (<40) | `Nota.EstadoBase` |
| 21 | Convocatoria: 0 a 100, con tope de 80 y una sola por clase | `SistemaGestion.RegistrarConvocatoria` |
| 22 | Notas no editables: re-registrar → excepción | `NotaYaRegistradaException` |
| 23 | "Nota pendiente" para grupos sin nota y resumen académico | `VerMisNotas`, `MostrarNotasDelGrupo` |
| 24 | Fecha tope del administrador para notas y convocatorias | `FechaTopeVencidaException` |
| 25 | Asistencia con 15 minutos hábiles y registro de ausencias | `MarcarAsistencia`, `RegistrarAusencia` |
| 26 | Descuento de C$ 400 por cada hora de clase no impartida | `GenerarPlanilla` |
| 27 | Planilla con salario base, descuentos y neto; pago validado y único | `GenerarPlanilla`, `RegistrarPago`, `PagoInvalidoException` |
| 28 | Indicación del docente: agregar, editar y listar; estado 1/0 en lugar de DELETE | Menús de administración y campo `Estado` en cada modelo |

## 4. Cuentas de prueba

### Administrador

| Usuario | Contraseña |
|---------|-----------|
| admin   | Admin#2026 |

### Estudiantes (carrera: Ingeniería en Sistemas de Información — UNAN-León)

| Usuario   | Contraseña   | Situación académica |
|-----------|--------------|---------------------|
| ddonaire  | David#2026   | Aprobó Fundamentos de Programación (ISI-II-01, nota 88) → puede inscribir Desarrollo de Aplicaciones I |
| lbustos   | Luis#2026B   | Aprobó ISI-II-01, ISI-III-01, ISI-III-02 e ISI-III-04 → puede inscribir Desarrollo de Aplicaciones II; ya está inscrito en G-IV02R (nota pendiente) |
| msilva    | Maria#2026   | Reprobó ISI-II-01 con 35 (sin derecho a convocatoria) → reinscrita en G-II01R |
| dzapata   | Daniel#26    | Aprobó ISI-II-01 por convocatoria (obtuvo 90 → quedó en 80 por el tope) |
| kmendoza  | Kenny#2026   | Inscrito en G-IV02R (Desarrollo de Aplicaciones II) con nota 78 ya registrada |
| ilopez    | Ivan#2026L   | **Inactivo (0)** → el sistema le niega el inicio de sesión |

Otros estudiantes (de carreras distintas, para mostrar el catálogo multiuniversidad): atorres (Medicina, UNAN-León), stellez (Ing. en Sistemas, UdeM), privas (Administración de Empresas, UCA), xpalacios (Contaduría, UNAN-Managua).

### Profesores

| Usuario   | Contraseña    | Grupos asignados |
|-----------|---------------|------------------|
| cmartinez | Clave#2026    | G-II01R, G-III02R (regular) — ya cobró su pago |
| mlopez    | Martha#2026   | G-III01D, G-II01D (dominical) |
| jrivera   | Jose#2026R    | G-IV05R, G-CV201S (sabatino) |
| fzamora   | Favio#2026    | G-IV02R — tiene una ausencia (C$ 800 de descuento) |
| knunez    | Karla#2026    | G-V02V, G-V01R |
| asanchez  | Ana#2026S     | sin grupos (para demostrar la asignación de clases) |

## 5. Datos basados en la vida real

- Catálogo de universidades nicaragüenses reales (miembros del CNU y privadas reconocidas): UNAN-Managua, UNAN-León, UNI, UCA, UNA, UdeM, UCYT y BICU.
- **Pénsum completo y marcado de la carrera principal**: Ingeniería en Sistemas de Información (ISI) de la UNAN-León, Facultad de Ciencias y Tecnología — 10 semestres y 64 asignaturas con los códigos del pénsum (ISI-I-01 a ISI-X-07), desde Inglés Técnico I hasta las Formas de Culminación de Estudios (Trabajo de Diploma I y II). IMPORTANTE: la malla oficial completa no está publicada en línea, por lo que los nombres de las asignaturas, los prerrequisitos y los créditos de este proyecto son PROPUESTOS y siguen el orden natural de los 10 semestres del plan de estudios.
- Cursos de Ingeniería Civil de la UNI (Cálculo I) y códigos reales de la UCYT (pénsum 2026-2030 de Ingeniería de Sistemas): IF-3900, IF-1100, IF-1400.
- Modalidades reales: **regular** (diurno/vespertino/sabatino) y **dominical** (clases por encuentro los domingos).
- Formato real de cédula nicaragüense (XXX-XXXXXX-XXXXL), celulares de 8 dígitos y periodos tipo 2026-II.
- Salarios del enunciado, coherentes con la escala docente: C$ 400 por hora; 60 horas en dominical (C$ 24,000) y 80 horas en regular (C$ 32,000).

Los nombres de personas, cédulas y celulares son ficticios, creados solo para la demostración.

## 6. Ejemplo de sesión

Salida de la aplicación (fragmento de una sesión de demostración con la cuenta ddonaire):

~~~text
==============================================================
 SISTEMA DE GESTIÓN ESTUDIANTIL — UNIVERSIDADES DE NICARAGUA
==============================================================
 Periodo académico actual: 2026-II
 Usuarios de prueba: admin / Admin#2026  ·  ddonaire / David#2026  ·  cmartinez / Clave#2026
 (la lista completa de cuentas está en el README.md)

 1. Iniciar sesión
 2. Salir del sistema
Seleccione una opción: 1
Usuario: ddonaire
Contraseña: ********
[OK] Bienvenido(a): David Donaire  [Rol: Estudiante]

==============================================================
 MÓDULO ESTUDIANTE
==============================================================
 Estudiante: David Donaire (001-120703-0004A)
 Carrera: Ingeniería en Sistemas de Información (ISI)
 1. Inscribir clases
 2. Ver mis notas
 3. Ver mi historial académico (cursos aprobados)
 4. Editar mi perfil
 5. Cerrar sesión
Opción: 1

 --- CURSOS DISPONIBLES DE SU CARRERA ---
  ISI-III-02 Desarrollo de Aplicaciones I               Prerrequisitos: ISI-II-01
  ISI-III-05 Género en la Vida Cotidiana               Prerrequisitos: ninguno
  ISI-IV-02  Desarrollo de Aplicaciones II              Prerrequisitos: ISI-III-02
  ...
Código del curso que desea inscribir: ISI-III-02

 --- PRERREQUISITOS DE Desarrollo de Aplicaciones I ---
  ISI-II-01  Fundamentos de Programación                       APROBADO

 --- GRUPOS DISPONIBLES ---
  G-III02R   Regular   Miércoles  07:00-09:00  Docente: Carlos Adán Martínez Solís  Cupo: 40/40
Código del grupo: G-III02R

 --- CONFIRMACIÓN DE INSCRIPCIÓN ---
  Estudiante : David Donaire (001-120703-0004A)
  Curso      : Desarrollo de Aplicaciones I (ISI-III-02)
  Grupo      : G-III02R — Regular
  Horario    : Miércoles 07:00-09:00
  Docente    : Carlos Adán Martínez Solís
  Periodo    : 2026-II
¿Confirma la inscripción? (S/N): S

 --- COMPROBANTE DE INSCRIPCIÓN ---
  Estudiante : David Donaire (001-120703-0004A)
  Curso      : Desarrollo de Aplicaciones I (ISI-III-02)
  Grupo      : G-III02R — Regular
  Horario    : Miércoles 07:00-09:00
  Docente    : Carlos Adán Martínez Solís
  Periodo    : 2026-II    Fecha: 01/11/2026
  No. de inscripción: 13
[OK] Inscripción registrada correctamente.
~~~

Ejemplo de excepción (intento de inscripción con prerrequisito pendiente; se muestra en rojo y la aplicación continúa):

~~~text
Código del curso que desea inscribir: ISI-IV-02

 --- PRERREQUISITOS DE Desarrollo de Aplicaciones II ---
  ISI-III-02 Desarrollo de Aplicaciones I                       PENDIENTE

[X] Inscripción bloqueada: falta aprobar Desarrollo de Aplicaciones I (ISI-III-02).
Presione Enter para continuar...
~~~

Ejemplo de excepción de acceso denegado (estudiante inactivo):

~~~text
Usuario: ilopez
Contraseña: ********
[X] El estudiante está INACTIVO (0). Solo un estudiante en estado activo (1) puede iniciar sesión.
~~~

## 7. Ruta de demostración sugerida (para presentar el trabajo)

1. **Login**: pruebe con `ilopez` (estudiante inactivo → excepción de acceso denegado).
2. **Estudiante** (`ddonaire` / `David#2026`):
   - Inscribir clases → elija ISI-IV-02 (Desarrollo de Aplicaciones II) → se bloquea: falta el prerrequisito ISI-III-02.
   - Inscribir clases → elija ISI-III-02 (Desarrollo de Aplicaciones I) → grupo G-III02R → confirme → comprobante.
3. **Estudiante** (`lbustos` / `Luis#2026B`):
   - Ver mis notas → muestra "Nota pendiente" en Desarrollo de Aplicaciones II.
   - Intente inscribir G-IV05R (Estadística General, Viernes 07:00-09:00) → conflicto de horario con G-IV02R.
4. **Profesor** (`fzamora` / `Favio#2026`):
   - Registrar notas → G-IV02R → ingrese (por ejemplo) 35 y 20 para Luis Bustos → nota final 55 → derecho a convocatoria.
   - La opción de convocatoria se habilita → registrar convocatoria → ingrese 90 → el sistema aplica el tope de 80 → aprobado.
   - Intente registrar de nuevo la misma nota → excepción.
   - Marcar asistencia → simule una entrada a las 07:20 → excepción (solo hay 15 minutos hábiles); regístrela como ausencia y vea el descuento en su planilla.
5. **Administrador** (`admin` / `Admin#2026`):
   - Listar universidades, carreras y cursos: pénsum completo marcado de Ingeniería en Sistemas de Información (ISI-UNAN-León), con las 64 asignaturas de los 10 semestres.
   - Registre un estudiante con datos inválidos (usuario con espacios, cédula mal formateada, contraseña débil, cédula duplicada) → observe cada excepción.
   - Asigne el grupo G-IV01S a `jrivera` → excepción de conflicto de horario (ya tiene G-CV201S el sábado de 08:00 a 12:00); luego asígnelo a `asanchez` → se genera el contrato.
   - Genere la planilla de pagos → valide el pago de fzamora con monto incorrecto (excepción) y con el monto exacto (C$ 31,200).
   - Intente pagar de nuevo a cmartinez → excepción (ya cobró).

## 8. Requisitos del enunciado implementados

| Requisito | Implementación |
|---|---|
| Acceso con usuario y contraseña por roles | `Servicios/Sesion.cs`, `SistemaGestion.Login` |
| Excepción de acceso denegado por rol | `AccesoDenegadoException` |
| Estudiante: solo inscribir clases y ver notas | Menú de estudiante |
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

## 9. Estructura del proyecto

~~~text
sistemaGestionEstudiantil/
├── SistemaGestionEstudiantil.csproj   # proyecto .NET 8 (dotnet run)
├── Program.cs                         # punto de entrada
├── README.md                           # este archivo
├── Excepciones/
│   └── Excepciones.cs                 # excepciones de negocio del enunciado
├── Modelos/
│   ├── Personas.cs                    # Persona, Estudiante, Profesor, Administrador
│   ├── Academico.cs                   # Universidad, Carrera, Curso, Grupo
│   └── Registros.cs                   # Inscripción, Nota, Contrato, Asistencia, Pago
├── Datos/
│   └── DatosIniciales.cs              # pénsum ISI de la UNAN-León y datos reales
├── Servicios/
│   ├── Validador.cs                  # validaciones de formato
│   ├── Sesion.cs                     # sesión y control de acceso por rol
│   └── SistemaGestion.cs              # reglas de negocio
└── UI/
    ├── Consola.cs                     # utilidades de consola
    └── Menus.cs                       # menús y flujos por rol
~~~
