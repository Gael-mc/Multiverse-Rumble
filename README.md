# Multiverse-Rumble — Documentación de Arquitectura (C4 Model)

Esta rama (`diagramas`) contiene la documentación de arquitectura del proyecto **Multiverse-Rumble**, modelada con el **Modelo C4** en sus primeros tres niveles: Contexto, Contenedores y Componentes.

## Objetivo

Documentar la arquitectura completa del proyecto usando el Modelo C4, versionada como código (Mermaid) dentro del propio repositorio, en lugar de imágenes sueltas o diagramas externos.

## Contenido

| Archivo | Descripción |
|---|---|
| [`Multiverse-Rumble/docs/c4-diagrams.md`](./Multiverse-Rumble/docs/c4-diagrams.md) | Los tres niveles de diagramas C4 (Contexto, Contenedores, Componentes), cada uno con una nota sobre para quién es y qué pregunta responde, más la declaración de uso de IA. |

## Niveles documentados

- **Nivel 1 — Contexto**: quién usa Multiverse-Rumble y qué es el sistema, en términos simples (sin tecnicismos).
- **Nivel 2 — Contenedores**: las piezas técnicas grandes del sistema (`Multiverse-Rumble` MVC, `Multiverse-Rumble-API`, base de datos SQLite) y cómo se comunican entre sí.
- **Nivel 3 — Componentes**: el detalle interno de la pieza principal (`Multiverse-Rumble-API`), incluyendo controladores, DTOs y la capa de acceso a datos.

## Estructura del proyecto (referencia)

```
Multiverse-Rumble/ (solución)
├── Multiverse-Rumble/          # Cliente MVC: Controllers, Models, Views
├── Multiverse-Rumble-API/      # API REST: Controllers, Data, DTOs, Models
│   └── multiverserumble.db     # Base de datos SQLite
└── ADR-docs/                   # Registros de decisiones de arquitectura (ADRs)
```

## Proceso de esta entrega

Los diagramas se agregaron en commits separados (uno por nivel) sobre esta rama, para reflejar el proceso incremental de documentación en lugar de un solo commit con todo el contenido de golpe.

## Cómo visualizar los diagramas

Los diagramas están escritos en sintaxis [Mermaid](https://mermaid.js.org/). Se renderizan automáticamente al ver el archivo `c4-diagrams.md` en GitHub, o pueden previsualizarse en Visual Studio Code con la extensión "Markdown Preview Mermaid Support".
