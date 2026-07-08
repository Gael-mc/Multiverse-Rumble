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