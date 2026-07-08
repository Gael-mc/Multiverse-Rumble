# Arquitectura de Multiverse-Rumble — Modelo C4

Este documento describe la arquitectura de **Multiverse-Rumble** usando el Modelo C4, en tres niveles de detalle progresivo. Todos los diagramas están escritos como código (Mermaid) para versionarse junto con el resto del repositorio.

---

## Nivel 1 — Contexto

**¿Para quién es este diagrama?** Para cualquier persona sin conocimiento técnico que quiera entender qué hace el sistema y quién lo usa, sin entrar en detalles de tecnología.

**¿Qué pregunta responde?** *¿Qué es Multiverse-Rumble y quién interactúa con él?*

```mermaid
graph TD
    Jugador([Jugador])

    subgraph Sistema
        MR[Multiverse-Rumble\nSistema de combates entre\npersonajes de distintos universos]
    end

    Jugador -->|Elige personajes, escenarios\ny consulta resultados de combate| MR
```



## Nivel 2 — Contenedores

**¿Para quién es este diagrama?** Para desarrolladores o arquitectos que necesitan entender las piezas técnicas grandes del sistema y cómo se comunican, sin ver el código interno de cada una.

**¿Qué pregunta responde?** *¿Cuáles son las piezas técnicas principales de Multiverse-Rumble y cómo interactúan?*

```mermaid
graph TD
    Jugador([Jugador\nNavegador web])

    subgraph "Multiverse-Rumble - Sistema"
        Web[Multiverse-Rumble\nASP.NET Core MVC\nControllers + Views]
        Api[Multiverse-Rumble-API\nASP.NET Core Web API\nControllers + DTOs]
        DB[(SQLite\nmultiverserumble.db)]
    end

    Jugador -->|HTTP/HTTPS| Web
    Web -->|HTTP/JSON| Api
    Api -->|Lee/Escribe| DB
```

## Nivel 3 — Componentes

**¿Para quién es este diagrama?** Para el desarrollador que trabaja directamente dentro de `Multiverse-Rumble-API`, y necesita ver cómo colaboran controladores, DTOs y la capa de datos.

**¿Qué pregunta responde?** *¿Qué hay dentro de Multiverse-Rumble-API y cómo se resuelve una petición de principio a fin?*

```mermaid
graph TD
    Web([Multiverse-Rumble\nCliente MVC])

    subgraph "Multiverse-Rumble-API"
        PersonajeController[PersonajeController]
        CombateController[CombateController]
        JugadorController[JugadorController]
        EscenarioController[EscenarioController]

        PersonajeDTO[DTOs\nPersonajeDTO, CombateDTO, etc.]

        DataLayer[Data\nAcceso a datos / Repository]

        Models[Models\nPersonaje, Combate,\nEscenario, Jugador, Habilidad, Universo]
    end

    DB[(SQLite\nmultiverserumble.db)]

    Web -->|HTTP/JSON| PersonajeController
    Web -->|HTTP/JSON| CombateController
    Web -->|HTTP/JSON| JugadorController
    Web -->|HTTP/JSON| EscenarioController

    PersonajeController -->|Mapea/valida| PersonajeDTO
    CombateController -->|Mapea/valida| PersonajeDTO
    JugadorController -->|Mapea/valida| PersonajeDTO
    EscenarioController -->|Mapea/valida| PersonajeDTO

    PersonajeDTO -->|Se traduce a/desde| Models
    PersonajeController --> DataLayer
    CombateController --> DataLayer
    JugadorController --> DataLayer
    EscenarioController --> DataLayer

    DataLayer -->|Usa| Models
    DataLayer -->|Lee/Escribe| DB
```



---

## Declaración de uso de IA

Estos diagramas fueron elaborados con apoyo de una IA (Claude, Anthropic) para estructurar el modelo C4 y la sintaxis Mermaid, a partir de la arquitectura real del proyecto. El contenido fue revisado y ajustado por el autor del repositorio.

---

## Notas de proceso

Este archivo se construyó en tres commits separados sobre la rama `diagramas`, uno por cada nivel, para reflejar el proceso de documentación de la arquitectura.
