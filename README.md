# Multiverse-Rumble

[![CI](https://github.com/Gael-mc/Multiverse-Rumble/actions/workflows/ci.yml/badge.svg)](https://github.com/Gael-mc/Multiverse-Rumble/actions/workflows/ci.yml)

Juego de peleas 1v1 en pixel art hecho con **ASP.NET Core MVC (.NET 10)** para la parte
administrativa (catálogo de personajes, escenarios e historial) y **JavaScript + Canvas API**
para el motor de combate en tiempo real, dentro del navegador. Incluye una **API REST**
independiente (`Multiverse-Rumble-API`) para persistir el historial de partidas con
EF Core + SQLite, documentada con Swagger.

**Demo en vivo:** _(agregar aquí el link una vez desplegado — ver sección "Desplegar la demo"
más abajo)_.

---

## Cómo correr el proyecto en local

Requisitos: [.NET SDK 10.0](https://dotnet.microsoft.com/download).

```bash
# 1. Clonar y entrar al repo
git clone https://github.com/Gael-mc/Multiverse-Rumble.git
cd Multiverse-Rumble

# 2. Restaurar y compilar toda la solución (web + API + tests)
dotnet restore Multiverse-Rumble.slnx
dotnet build Multiverse-Rumble.slnx

# 3. Correr el juego (sitio MVC)
dotnet run --project Multiverse-Rumble/Multiverse-Rumble.csproj
# -> https://localhost:7294  (o http://localhost:5183)

# 4. (Opcional, en otra terminal) correr la API de historial de partidas
dotnet run --project Multiverse-Rumble-API/Multiverse-Rumble-API.csproj
# -> https://localhost:7032/swagger  (documentación interactiva)

# 5. Correr las pruebas automatizadas
dotnet test Multiverse-Rumble.slnx
```

Desde el sitio (`/Combate/Seleccion`): Jugador 1 usa **WASD + F**, Jugador 2 usa **flechas +
L**. `W`/`↑` saltan, `F`/`L` atacan.

## Pipeline de Integración Continua

En cada `push` y `pull request` corre `.github/workflows/ci.yml`: `dotnet restore` →
`dotnet build` → `dotnet test` sobre toda la solución. El resultado se ve como check
✅/❌ directamente en GitHub y en el badge de arriba. Ver detalle de qué se prueba y por qué en
[`Multiverse-Rumble/ADR-docs/adr-03.txt`](Multiverse-Rumble/ADR-docs/adr-03.txt).

## Desplegar la demo

⚠️ **GitHub Pages no sirve para este proyecto**: solo sirve archivos estáticos (HTML/CSS/JS
sueltos) y no puede ejecutar un servidor ASP.NET Core con Controllers, Razor Views renderizadas
en el servidor ni el endpoint `POST /Combate/GuardarResultado`. Para una demo con link público
real hace falta un hosting que corra .NET, por ejemplo (ambos tienen plan gratuito):

- **Render.com** — solo despliega .NET vía Docker (no tiene buildpack nativo). El repo ya
  incluye [`Dockerfile`](Dockerfile) listo para esto: Web Service nuevo → conectar el repo de
  GitHub → Runtime "Docker" (Render detecta el `Dockerfile` solo) → Free plan. No hace falta
  configurar Build/Start Command, ya están en el Dockerfile. `Program.cs` ya lee la variable
  `PORT` que Render inyecta automáticamente.
- **Azure App Service** (plan gratuito F1, suele venir incluido con el GitHub Student
  Developer Pack) — `az webapp up` o publicar directo desde Visual Studio.

Si no hay tiempo de desplegar antes de la sesión, la demo se corre en local (`dotnet run`,
paso 3 de arriba) frente al profesor; el pipeline de CI en GitHub Actions y las pruebas
pasando quedan como evidencia automatizada en el repositorio sin depender de un hosting.

---

## Documentación de arquitectura

| Documento | Contenido |
|---|---|
| [`ADR-01-Gael-Magaña.pdf`](ADR-01-Gael-Magaña.pdf) | ADR-01 — Arquitectura base del sistema (stack tecnológico, alternativas consideradas) |
| [`ADR-02 Vistas Arquitectónicas.md`](ADR-02%20Vistas%20Arquitectónicas.md) | ADR-02 — Modelo 4+1 (vista lógica, de desarrollo, de procesos, de despliegue) y tabla de trade-offs |
| [`ADR-03-GAEL-MAGAÑA.md`](ADR-03-GAEL-MAGAÑA.md) | ADR — Estilo arquitectónico: Capas + Cliente-Servidor, alternativas descartadas |
| [`ADR-API-REST`](ADR-API-REST) | ADR — Decisión de incorporar la API REST de historial de partidas (EF Core + SQLite vs. GraphQL/gRPC) |
| [`Multiverse-Rumble/ADR-docs/adr-01.txt`](Multiverse-Rumble/ADR-docs/adr-01.txt) | Deuda técnica 1 — Base de datos SQLite versionada en el repo |
| [`Multiverse-Rumble/ADR-docs/adr-02.txt`](Multiverse-Rumble/ADR-docs/adr-02.txt) | Deuda técnica 2 — Modelos de dominio duplicados entre Web y API |
| [`Multiverse-Rumble/ADR-docs/adr-03.txt`](Multiverse-Rumble/ADR-docs/adr-03.txt) | ADR — Pruebas unitarias con xUnit y pipeline de CI |
| [`Multiverse-Rumble/docs/c4-diagrams.md`](Multiverse-Rumble/docs/c4-diagrams.md) | **Modelo C4** — Niveles 1 a 3 (Contexto, Contenedores, Componentes), actualizado a la versión final |
| [`Multiverse-Rumble/docs/atam-evaluacion.md`](Multiverse-Rumble/docs/atam-evaluacion.md) | **Evaluación ATAM** — 1 riesgo, 1 trade-off y 1 punto de sensibilidad |

> Nota: hay dos documentos numerados "ADR-03" (estilo arquitectónico y pruebas/CI) por cómo
> evolucionó el proyecto entre unidades; ambos están vigentes y se listan arriba por su
> contenido, no por número.

## Estructura de la solución

```
Multiverse-Rumble.slnx
├── Multiverse-Rumble/           # Sitio MVC: catálogo, selección y arena de combate
├── Multiverse-Rumble-API/       # API REST: historial de partidas (EF Core + SQLite + Swagger)
└── Multiverse-Rumble.Tests/     # Pruebas xUnit sobre los Controllers del sitio MVC
```

## Declaración de uso de IA

Este proyecto se desarrolló con apoyo de un asistente de inteligencia artificial (Claude,
Anthropic) para: redacción y estructuración de los ADR, diagramas C4 en sintaxis Mermaid,
evaluación ATAM, extracción y verificación de orientación de los sprites de pixel art a
partir de hojas de sprites provistas por el autor, y este README. El análisis de
requerimientos, las decisiones de arquitectura, las mecánicas del juego y la autoría del
código fuente del proyecto son responsabilidad exclusiva del autor. Cada ADR individual
incluye su propia declaración de uso de IA con más detalle.
