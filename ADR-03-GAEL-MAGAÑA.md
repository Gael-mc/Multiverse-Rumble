# ADR-03: Estilo Arquitectónico de Multiverse Rumble

| Campo  | Valor                                |
|--------|--------------------------------------|
| Autor  | Gael                                 |
| Fecha  | 15/05/2026                           |
| Estado | Aceptado                             |

---

## Contexto

Multiverse Rumble es una aplicación web de peleas 1v1 con estética pixel art dirigida
a jóvenes de 15 a 30 años fanáticos de animes, películas, cómics y videojuegos.

El sistema tiene dos contextos claramente distintos:
- **Contexto administrativo:** registro de personajes, escenarios, habilidades e
  historial de combates — manejado en el servidor con ASP.NET Core MVC
- **Contexto de combate en tiempo real:** motor de pelea con sprites, movimiento,
  golpes y colisiones — manejado en el navegador con JavaScript + Canvas API

Condiciones que influyeron en la decisión:
- Proyecto individual desarrollado en un cuatrimestre
- Tecnología requerida por la materia: C# con ASP.NET Core MVC (.NET 10)
- El desarrollador tiene conocimiento previo en .NET, Razor Views y JavaScript
- La lógica de combate en tiempo real no puede vivir en el servidor sin
  introducir WebSockets, lo que aumentaría la complejidad del proyecto

---

## Decisión

Se elige el estilo arquitectónico de **Capas (Layered Architecture)** combinado con
**Cliente-Servidor**, aplicado de la siguiente manera:

| Capa | Tecnología | Responsabilidad |
|------|-----------|-----------------|
| Presentación (Cliente) | Razor Views + JavaScript Canvas | UI, motor de combate en tiempo real |
| Lógica de Negocio (Servidor) | ASP.NET Core MVC Controllers | Reglas del juego, gestión de datos |
| Modelo de Datos | C# Models (List estática) | Personaje, Habilidad, Escenario, Combate, Jugador, Universo |

---

## ¿Por qué?

El estilo de **Capas + Cliente-Servidor** resuelve los dos contextos del sistema:

- **Capas** separa claramente la presentación, la lógica y los datos, lo que
  permite modificar una capa sin afectar las demás. Agregar un nuevo personaje
  o escenario solo requiere modificar el Model y el Controller correspondiente,
  sin tocar el motor de combate.

- **Cliente-Servidor** es natural para una aplicación web. El servidor entrega
  la interfaz y gestiona los datos vía HTTP, mientras el cliente ejecuta el
  combate en tiempo real con Canvas API sin necesidad de round-trips al servidor
  en cada frame.

La combinación de ambos estilos permite que el sistema sea simple de mantener
hoy y escalable en el futuro (agregar más personajes, escenarios o modos de
juego sin rediseñar la arquitectura).

---

## Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Microservicios | El proyecto es individual y de un cuatrimestre. Dividir en microservicios (personajes, combate, usuarios) añadiría complejidad de comunicación entre servicios innecesaria para la escala actual |
| Event-Driven | El combate podría modelarse con eventos, pero ASP.NET Core MVC no está diseñado para eso de forma nativa. Requeriría SignalR o un message broker que está fuera del alcance de la materia |
| Hexagonal (Puertos y Adaptadores) | Útil para aislar la lógica de negocio de la infraestructura, pero innecesariamente complejo para un sistema con almacenamiento en memoria y sin base de datos real aún |
| Serverless | No es compatible con el requisito de usar ASP.NET Core MVC con estado en memoria. Las funciones serverless son stateless por diseño |

---

## Consecuencias

### ✅ Lo que gano

**Consecuencia técnica:** La separación en capas hace que cada parte del sistema
tenga una responsabilidad clara. Los Controllers no tienen lógica de UI, los Models
no tienen lógica de negocio y las Views no acceden directamente a los datos.
Esto hace que el código sea más fácil de leer, mantener y extender.

**Consecuencia sobre el proceso:** Al usar tecnologías conocidas (.NET, Razor,
JavaScript) con un estilo arquitectónico estándar, el tiempo de desarrollo se
reduce y es posible avanzar por módulos independientes: primero personajes,
luego escenarios, finalmente el combate.

### ⚠️ Lo que sacrifico o asumo

**Limitación técnica:** El almacenamiento en memoria (List estática) se pierde
al reiniciar el servidor. No hay persistencia real hasta migrar a SQL Server
con Entity Framework, lo que es una deuda técnica asumida conscientemente.

**Deuda técnica:** El motor de combate en JavaScript no sigue el patrón de capas
formalmente — toda la lógica de combate vive en un solo archivo JS. Si el
juego crece, será necesario refactorizarlo en módulos separados (motor, renderer,
input, colisiones) para mantener la separación de responsabilidades también
en el cliente.

---

## Diagrama

Ver archivo `diagramas/estilo-arquitectonico.drawio` en el repositorio.

---

## Declaración de uso de IA

Este ADR fue desarrollado con apoyo de Claude (Anthropic) como asistente de
redacción y estructuración del documento. El análisis arquitectónico, las
decisiones de diseño y la comprensión del sistema son responsabilidad del autor.