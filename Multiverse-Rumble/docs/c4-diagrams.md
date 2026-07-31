# Arquitectura de Multiverse-Rumble — Modelo C4

Este documento describe la arquitectura de **Multiverse-Rumble** usando el Modelo C4, en sus
tres primeros niveles de detalle progresivo. Todos los diagramas están escritos como código
(Mermaid) para versionarse junto con el resto del repositorio y renderizarse automáticamente
al ver este archivo en GitHub.

> Actualizado a la versión final del proyecto (Unidad 3 — Actividad 40). Refleja el estado
> real del código en este momento, no un diseño aspiracional.

---

## Nivel 1 — Contexto

**¿Para quién es este diagrama?** Para cualquier persona sin conocimiento técnico que quiera
entender qué hace el sistema y quién lo usa, sin entrar en detalles de tecnología.

**¿Qué pregunta responde?** *¿Qué es Multiverse-Rumble y quién interactúa con él?*

```mermaid
graph TD
    Jugador([Jugador<br/>Persona con navegador web])

    subgraph Sistema["Sistema Multiverse-Rumble"]
        MR[Multiverse-Rumble<br/>Juego de peleas 1v1 en pixel art<br/>entre personajes de distintos universos]
    end

    Jugador -->|Elige personajes y escenario,<br/>juega el combate,<br/>consulta el historial| MR
```

**Audiencia:** cualquier persona interesada en el proyecto (evaluadores no técnicos, otros
alumnos, el propio jugador).

---

## Nivel 2 — Contenedores

**¿Para quién es este diagrama?** Para desarrolladores o arquitectos que necesitan entender
las piezas técnicas grandes del sistema y cómo se comunican, sin ver el código interno de
cada una.

**¿Qué pregunta responde?** *¿Cuáles son las piezas técnicas principales de Multiverse-Rumble
y cómo interactúan hoy?*

```mermaid
graph TD
    Jugador([Jugador<br/>Navegador web])

    subgraph Navegador["Navegador (Cliente)"]
        Motor["Motor de Combate<br/>[JavaScript + Canvas API]<br/>combate.js: físicas, colisiones,<br/>animaciones, entrada de teclado"]
    end

    subgraph Servidor["Servidor"]
        Web["Multiverse-Rumble Web<br/>[ASP.NET Core MVC / .NET 10]<br/>Controllers + Razor Views.<br/>Datos en listas estáticas en memoria"]
        Api["Multiverse-Rumble-API<br/>[ASP.NET Core Web API / .NET 10]<br/>Endpoints REST /api/matches<br/>documentados con Swagger"]
    end

    DB[("SQLite<br/>multiverserumble.db")]

    Jugador -->|HTTP/HTTPS<br/>navega catálogo, arma el combate| Web
    Web -->|Sirve la página Arena.cshtml<br/>con el motor embebido| Motor
    Motor -->|Captura teclado,<br/>corre el combate a 60 FPS| Jugador
    Api -->|EF Core| DB

    Web -.->|"aún no integrado<br/>(ver nota)"| Api
```

**Nota importante — estado real de la integración:** `Multiverse-Rumble-API` es un proyecto
independiente y funcional (persiste partidas en SQLite vía EF Core, expone `GET/POST/DELETE
/api/matches` y `GET /api/matches/stats/{character}`, documentado con Swagger), pero **el sitio
MVC todavía no lo consume**: `CombateController.GuardarResultado` guarda el resultado del
combate en su propia lista estática en memoria (`_historial`), no vía HTTP contra la API. La
flecha punteada del diagrama representa esa integración pendiente, documentada como decisión
consciente en `ADR-API-REST` ("sienta la base para futuras features... sin rediseñar la capa
de datos"). Conectar `Web → Api` es el siguiente paso natural de evolución de la arquitectura.

**Audiencia:** desarrolladores y el arquitecto de la solución, para validar que las piezas
grandes del sistema y sus fronteras de comunicación (o falta de ellas) están correctamente
identificadas.

---

## Nivel 3 — Componentes

Dado que el sistema tiene dos contenedores de servidor con lógica propia, se documentan los
componentes de **ambos**.

### 3.1 — Componentes de `Multiverse-Rumble` (Web MVC)

**¿Qué pregunta responde?** *¿Cómo resuelve el sitio MVC una petición, desde el controlador
hasta la vista que entrega el motor de combate al navegador?*

```mermaid
graph TD
    Jugador([Jugador])

    subgraph "Multiverse-Rumble (Web MVC)"
        PersonajeController["PersonajeController<br/>catálogo + alta/baja de personajes"]
        EscenarioController["EscenarioController<br/>catálogo de escenarios"]
        CombateController["CombateController<br/>Seleccion / Arena / GuardarResultado / Historial"]
        JugadorController["JugadorController"]
        HomeController["HomeController"]

        Models["Models (POCOs)<br/>Personaje, Escenario, Combate,<br/>Jugador, Habilidad, Universo"]

        Listas["Listas estáticas en memoria<br/>_personajes, _escenarios, _historial<br/>(una por Controller)"]

        ArenaView["Views/Combate/Arena.cshtml<br/>expone BATTLE_DATA (JSON) al cliente"]
    end

    Estaticos["wwwroot/js/combate.js<br/>wwwroot/img/personajes, escenarios, ui<br/>(servidos como archivos estáticos)"]

    Jugador -->|HTTP GET/POST| PersonajeController
    Jugador -->|HTTP GET/POST| EscenarioController
    Jugador -->|HTTP GET/POST| CombateController
    Jugador -->|HTTP GET| JugadorController
    Jugador -->|HTTP GET| HomeController

    PersonajeController --> Listas
    EscenarioController --> Listas
    CombateController --> Listas
    PersonajeController --> Models
    EscenarioController --> Models
    CombateController --> Models

    CombateController -->|arma ViewBag con<br/>Personaje1, Personaje2, Escenario| ArenaView
    ArenaView -->|el navegador descarga| Estaticos
    Jugador -->|GET archivos estáticos| Estaticos
```

**Nota:** no existe una capa `Data/` separada ni un `DbContext`: cada Controller declara y
gestiona su propia lista `static` en memoria (decisión documentada en `ADR-docs/adr-01.txt` y
`ADR-docs/adr-02.txt` como deuda técnica consciente — ver sección de ATAM).

### 3.2 — Componentes de `Multiverse-Rumble-API`

**¿Qué pregunta responde?** *¿Qué hay dentro de Multiverse-Rumble-API y cómo se resuelve una
petición de principio a fin?*

```mermaid
graph TD
    Cliente([Cliente HTTP<br/>Swagger UI / futuro cliente Web])

    subgraph "Multiverse-Rumble-API"
        MatchesController["MatchesController<br/>GET/POST/DELETE /api/matches<br/>GET /api/matches/stats/{character}"]
        DTOs["DTOs<br/>CreateMatchDto, MatchResponseDto,<br/>CharacterStatsDto"]
        AppDbContext["AppDbContext : DbContext<br/>DbSet&lt;MatchResult&gt; Matches"]
        MatchResult["Models.MatchResult"]
    end

    DB[("SQLite<br/>multiverserumble.db")]

    Cliente -->|HTTP/JSON| MatchesController
    MatchesController -->|mapea/valida| DTOs
    DTOs -->|se traduce a/desde| MatchResult
    MatchesController --> AppDbContext
    AppDbContext -->|EF Core| DB
    AppDbContext --> MatchResult
```

**Audiencia (ambos diagramas de Nivel 3):** el desarrollador que da mantenimiento directo al
código de cada proyecto.

---

## Declaración de uso de IA

Estos diagramas fueron elaborados con apoyo de un asistente de inteligencia artificial
(Claude, Anthropic) para estructurar el modelo C4 y la sintaxis Mermaid a partir de la
arquitectura real del proyecto (código de `Controllers`, `Models`, `combate.js` y
`Multiverse-Rumble-API` inspeccionado directamente). El análisis de qué construir, las
decisiones de arquitectura y el desarrollo del código fuente son de autoría y responsabilidad
exclusiva del autor del repositorio.
