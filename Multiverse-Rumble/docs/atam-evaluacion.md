# Evaluación ATAM — Multiverse-Rumble

| Campo | Valor |
|---|---|
| Método | ATAM (Architecture Tradeoff Analysis Method), versión simplificada para proyecto individual |
| Fecha | 2026-07-31 |
| Autor | Gael |
| Alcance | Arquitectura final del sistema: `Multiverse-Rumble` (MVC + motor de combate en Canvas) y `Multiverse-Rumble-API` (REST + SQLite) |

## 1. Motivadores del negocio (contexto)

Multiverse-Rumble es un proyecto individual de un cuatrimestre: un juego de peleas 1v1 en
pixel art, jugado localmente por dos personas en el mismo teclado, con un catálogo
administrable de personajes/escenarios y un historial de partidas. Los atributos de calidad
más relevantes para este contexto, en orden de prioridad real del proyecto, son:

1. **Facilidad de desarrollo y mantenimiento** (proyecto individual, tiempo limitado a un
   cuatrimestre).
2. **Rendimiento/interactividad del combate** (debe sentirse como un juego, no como una
   aplicación web con recargas).
3. **Disponibilidad/persistencia de los datos** (catálogo e historial no deberían perderse).
4. **Seguridad** (prioridad baja: es un juego local, sin cuentas de usuario ni datos
   sensibles — pero no inexistente, como se ve en el riesgo #1).

Estos cuatro atributos ya compiten entre sí en varias decisiones tomadas a lo largo del
proyecto (ver `ADR-01`, `ADR-02 Vistas Arquitectónicas.md`, `ADR-03-GAEL-MAGAÑA.md`,
`ADR-API-REST`, `ADR-docs/adr-01.txt`, `ADR-docs/adr-02.txt`). Esta evaluación identifica un
riesgo, un trade-off y un punto de sensibilidad concretos, cada uno anclado a una decisión
real ya tomada (no hipotética).

---

## 2. Riesgo

> **Definición ATAM:** decisión arquitectónica que podría causar problemas futuros si no se
> corrige — algo que "podría salir mal".

### Riesgo identificado: pérdida irrecuperable del esquema de la API si se pierde el archivo `.db`

**Decisión relacionada:** `ADR-API-REST` — usar Entity Framework Core con SQLite para
`Multiverse-Rumble-API`, con el archivo `multiverserumble.db` versionado junto al proyecto
(documentado también como deuda técnica en `ADR-docs/adr-01.txt`).

**Descripción del riesgo:** `Program.cs` de `Multiverse-Rumble-API` llama a
`db.Database.Migrate()` al arrancar, pero el proyecto **no tiene carpeta `Migrations/`** con
clases de migración de EF Core. Esto significa que `Migrate()` no tiene nada que aplicar: el
esquema de la tabla `Matches` existe *únicamente* porque quedó grabado a mano dentro del
archivo binario `multiverserumble.db` que se sube al repositorio. Si ese archivo se pierde,
se corrompe, o alguien lo regenera desde cero (por ejemplo, borrándolo para "empezar limpio",
una reacción natural dado que `ADR-docs/adr-01.txt` recomienda sacarlo del control de
versiones), la aplicación arranca sin errores pero **la tabla `Matches` nunca se crea**, y
cada request a `/api/matches` falla en tiempo de ejecución con `SQLite Error: no such table:
Matches`. El problema es silencioso hasta ese momento: compila y arranca sin ninguna
advertencia.

**Atributo de calidad afectado:** disponibilidad / capacidad de recuperación (recoverability).

**Cómo se detectó:** al preparar esta entrega se inspeccionó el archivo `.db` real del
proyecto y se confirmó que solo contenía las tablas internas de EF Core
(`__EFMigrationsHistory`, `__EFMigrationsLock`) pero no la tabla `Matches` — es decir, el
riesgo ya se había materializado parcialmente antes de esta revisión.

**Mitigación propuesta:** generar migraciones formales con
`dotnet ef migrations add InicialMatches` y dejar que `Database.Migrate()` las aplique en
cualquier entorno nuevo, en vez de depender de un archivo binario pre-poblado. Es la misma
solución que ya propone `ADR-docs/adr-01.txt` para el problema relacionado de versionar la
base de datos.

---

## 3. Trade-off

> **Definición ATAM:** decisión que mejora un atributo de calidad a costa de otro,
> conscientemente aceptada.

### Trade-off identificado: motor de combate en el cliente (Canvas API) en lugar de procesamiento en el servidor

**Decisión relacionada:** `ADR-01` (Arquitectura Base) y `ADR-02 Vistas Arquitectónicas.md`
— toda la simulación del combate (física, colisiones, animaciones a 60 FPS) corre en
JavaScript dentro del navegador (`wwwroot/js/combate.js`), y el servidor solo recibe el
resultado final vía `POST` cuando el combate termina.

**Lo que se gana:** rendimiento e interactividad inmediata. No hay latencia de red por
frame, el servidor ASP.NET Core MVC no necesita mantener conexiones persistentes ni procesar
físicas 60 veces por segundo, y la arquitectura se mantiene simple (HTTP tradicional
request/response, sin SignalR ni WebSockets).

**Lo que se sacrifica:** integridad/seguridad del resultado. Como toda la lógica de "quién
ganó" se calcula en el navegador del propio jugador y el servidor solo confía en el valor que
le llega por `POST` (`CombateController.GuardarResultado(string ganador, ...)`), un cliente
modificado (por ejemplo, editando `combate.js` con las herramientas de desarrollador del
navegador) podría reportar un ganador falso sin que el servidor tenga forma de validarlo. El
servidor permanece "ciego" durante todo el combate, tal como ya lo señala la tabla de
trade-offs de `ADR-02 Vistas Arquitectónicas.md`.

**Por qué se acepta:** para un juego local de dos jugadores en el mismo dispositivo, sin
cuentas de usuario, sin ranking competitivo en línea y sin datos sensibles en juego, el costo
de esta vulnerabilidad es bajo — el único "atacante" posible es uno de los dos jugadores
presentes físicamente, y el peor caso es que el historial local muestre un resultado
incorrecto. Sería un trade-off inaceptable si el proyecto evolucionara a un modo competitivo
en línea con ranking global, momento en el que el servidor tendría que validar el resultado
de forma autoritativa.

---

## 4. Punto de sensibilidad

> **Definición ATAM:** parámetro arquitectónico tal que un pequeño cambio en él provoca un
> cambio significativo en un atributo de calidad — el sistema es "sensible" a esa decisión.

### Punto de sensibilidad identificado: la disponibilidad del catálogo e historial depende por completo del ciclo de vida del proceso del servidor

**Decisión relacionada:** `ADR-03-GAEL-MAGAÑA.md` y `ADR-docs/adr-01.txt` — `Personaje`,
`Escenario` y `Combate` (historial) se almacenan en listas `static` en memoria dentro de cada
Controller de `Multiverse-Rumble` (`_personajes`, `_escenarios`, `_historial`), sin base de
datos.

**Por qué es un punto de sensibilidad:** el atributo de calidad "disponibilidad de los datos"
no varía gradualmente con ningún parámetro de configuración — depende de una sola variable
binaria: *¿el proceso del servidor Kestrel sigue corriendo o no?* Mientras el proceso vive,
el catálogo y el historial funcionan perfectamente. En el instante exacto en que el proceso
se reinicia (un deploy, un crash, un simple `Ctrl+C` en desarrollo, o el reciclado automático
de un App Service en la nube), **el 100% de los datos desaparece** sin ningún estado
intermedio de degradación. No existe un punto donde el sistema "se ponga lento" o "pierda
solo una parte" — es un salto abrupto de disponibilidad completa a pérdida completa.

**Consecuencia para las pruebas automatizadas:** este mismo punto de sensibilidad ya está
documentado como riesgo en `ADR-docs/adr-03.txt` (pruebas xUnit): las pruebas comparten el
mismo estado estático entre casos, así que el orden de ejecución y la persistencia entre
pruebas dependen exactamente del mismo mecanismo frágil.

**Qué lo haría menos sensible:** mover el almacenamiento a algo con persistencia real
(SQLite/SQL Server vía EF Core, igual que ya se hizo en `Multiverse-Rumble-API`) convertiría
esta relación de "todo o nada" en una relación gradual y mucho más resiliente — el mismo
cambio que ya proponen `ADR-docs/adr-01.txt` y `ADR-docs/adr-02.txt` como deuda técnica
pendiente de pago.

---

## 5. No-riesgos (para contraste)

Dos decisiones que en un primer vistazo podrían parecer riesgos, pero que el análisis
descarta para el alcance actual del proyecto:

- **Combate local en el mismo teclado (sin red):** parece una limitación, pero elimina por
  completo la superficie de riesgo de netcode/sincronización — es una decisión de alcance,
  no un riesgo técnico latente.
- **Sin autenticación en `Multiverse-Rumble-API`:** aceptable mientras la API no se exponga
  públicamente ni maneje datos de usuarios reales, tal como ya lo aclara `ADR-API-REST` en su
  sección de consecuencias negativas.

---

## Declaración de uso de IA

Este documento fue elaborado con apoyo de un asistente de inteligencia artificial (Claude,
Anthropic) para estructurar la evaluación según el método ATAM y redactar el análisis. El
riesgo de migraciones faltantes se identificó inspeccionando directamente el archivo
`multiverserumble.db` del proyecto real; el trade-off y el punto de sensibilidad se basan en
decisiones ya documentadas y de autoría del desarrollador en los ADRs existentes del
repositorio. El análisis final y la validez de las conclusiones son responsabilidad del
autor del proyecto.
