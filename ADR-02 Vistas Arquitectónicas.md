# **ADR-02: Vistas arquitectónicas del Sistema Multiverse Rumble**

| Campo | Valor |
| :---- | :---- |
| **Autor** | Gael |
| **Fecha** | 15/05/2026 |
| **Estado** | Aceptado |

## **Entorno**

El ADR-01 estableció el framework ASP.NET Core MVC en el servidor junto con JavaScript y Canvas API en el cliente como el stack tecnológico base de Multiverse Rumble. Sin embargo, debido a la naturaleza híbrida del sistema —donde coexisten la lógica administrativa y el motor de simulación gráfica en tiempo real—, un solo diagrama de contexto no es suficiente para comunicar el diseño a todas las audiencias interesadas.

Este documento complementa las decisiones tecnológicas iniciales describiendo el sistema desde sus cuatro perspectivas arquitectónicas independientes —**lógica, desarrollo, procesos y despliegue/física**— exigidas para la validación del sistema, evidenciando la separación de responsabilidades y los Trade-Offs asumidos conscientemente para el desarrollo del proyecto.

## **Trade-offs de las decisiones arquitectónicas**

Toda decisión de diseño beneficia ciertos atributos de calidad a costa de comprometer otros. La siguiente tabla resume los compromisos y balances críticos adoptados de manera consciente en la arquitectura actual de Multiverse Rumble.

| Decisión | Lo que gano | Lo que sacrifico |
| :---- | :---- | :---- |
| **Motor gráfico en el cliente (Canvas API) en lugar de procesamiento en servidor** | **Rendimiento e interactividad inmediata**: latencia cero en la captura de comandos físicos por frame (60 FPS) y nula sobrecarga de cómputo en el servidor web. | **Vulnerabilidad de estado (Seguridad)**: la simulación corre por completo en el navegador del usuario; un cliente malicioso podría alterar variables de juego en memoria local antes de reportar el resultado. |
| **Almacenamiento global estático en memoria RAM en lugar de Base de Datos SQL** | **Velocidad de desarrollo extrema** y simplicidad de configuración; lógicas de lectura y escritura instantáneas en colecciones genéricas de C\# sin la fricción de gestionar migraciones en el bloque cuatrimestral. | **Persistencia volátil**: los historiales de batallas, estadísticas y nuevos registros se destruyen por completo cada vez que el proceso del servidor web se reinicia. |
| **Monolito ASP.NET Core MVC con Vistas Razor** | **Centralización y cohesión**: un único proyecto unificado para administrar el ciclo de vida completo de la aplicación, alineado directamente con los requisitos obligatorios de la cátedra. | **Flexibilidad de interfaz**: la navegación entre el catálogo administrativo y la arena requiere recargas de página tradicionales, limitando una transición fluida tipo Single Page Application (SPA). |
| **Sincronización final vía HTTP POST asíncrono en lugar de WebSockets (SignalR)** | **Simplicidad de implementación**: el servidor no necesita mantener hilos de ejecución o conexiones abiertas persistentes mientras los jugadores disputan el combate local. | **Validación en tiempo real**: el servidor permanece "ciego" durante el transcurso de la pelea y solo se entera de la partida cuando esta ha finalizado, imposibilitando un modo espectador nativo. |
| **Combate local compartido (Mismo teclado) en lugar de arquitectura de red Online** | **Cero problemas de sincronización de red** (Netcode/Rollback); emparejamiento instantáneo y físicas de movimiento deterministas sin lidiar con latencia de red. | **Limitación de alcance**: restringe la experiencia de juego a dos usuarios sentados físicamente frente al mismo dispositivo de entrada. |

## **1\. Vista lógica**

Muestra las **responsabilidades funcionales** del sistema: identifica cómo se agrupan los componentes de software en módulos lógicos y cómo cooperan entre sí para cumplir con las reglas del juego.

<img width="1031" height="436" alt="image" src="https://github.com/user-attachments/assets/7db58d20-a266-4185-a905-d0ad44d981ca" />


**Módulos del Sistema:**

* **Controladores del Servidor (C\#):** Mapeo directo de PersonajeController, CombateController, EscenarioController y JugadorController para canalizar las peticiones del cliente.  
* **Modelos de Dominio:** Entidades base acopladas (Personaje, Habilidad, Escenario, Jugador, Universo) que dan estructura al ecosistema de juego.  
* **Módulos del Motor (JavaScript):** El bucle central (MotorDeCombate), el capturador de interrupciones de periféricos (ControlesJS) y las instancias dinámicas en pantalla (FighterEntity).

**Audiencia:** El diseñador de la solución y evaluadores, con el fin de constatar que las fronteras funcionales dividen correctamente las características de gestión web del bucle interactivo de acción.

## **2\. Vista de desarrollo**

Muestra cómo está **organizado el código fuente** dentro del entorno de desarrollo. Expone la jerarquía de directorios del proyecto monolítico en C\# y JavaScript, reflejando el acoplamiento y las responsabilidades del patrón arquitectónico MVC.

MultiverseRumble/  
│  
├── Controllers/            \# Procesamiento de solicitudes HTTP y coordinación de datos  
│   ├── HomeController.cs  
│   ├── PersonajesController.cs  
│   ├── EscenariosController.cs  
│   ├── JugadorController.cs  
│   └── CombatesController.cs  
│  
├── Models/                 \# Estructuras de datos del dominio (Entidades de C\#)  
│   ├── Universo.cs  
│   ├── Personaje.cs  
│   ├── Habilidad.cs  
│   ├── Escenario.cs  
│   └── Jugador.cs  
│  
├── Data/                   \# Infraestructura de persistencia volátil  
│   └── MultiverseDbContextMemory.cs  (Contexto global con colecciones List\<T\> estáticas)  
│  
├── Views/                  \# Archivos de interfaz de usuario e inserción de Canvas  
│   ├── Home/  
│   │   └── Index.cshtml    \# Pantalla de Bienvenida Principal  
│   ├── Personajes/         \# Catálogos y Vistas de Administración  
│   ├── Escenarios/  
│   ├── Combates/  
│   │   ├── Seleccion.cshtml \# Interfaz de preparación previa a la pelea  
│   │   └── Arena.cshtml     \# Contenedor HTML5 crítico donde se ejecuta el Canvas del juego  
│   └── Shared/  
│       └── \_Layout.cshtml  
│  
└── wwwroot/                \# Recursos estáticos distribuidos al navegador del cliente  
    ├── css/                \# Hojas de estilo de la aplicación y estética retro  
    ├── sprites/            \# Hojas de animación pixel art (Sprite Sheets de personajes)  
    └── js/  
        └── engine/         \# Componentes del motor gráfico y físico en JavaScript  
            ├── game.js     \# Bucle principal (Game Loop) y control de estados  
            ├── input.js    \# Captura de eventos físicos del teclado (Keydown/Keyup)  
            ├── physics.js  \# Cálculo de vectores de movimiento y detección de colisiones  
            └── render.js   \# Dibujado de buffers de imagen sobre el contexto del Canvas

**Dependencias:** Los controladores interactúan con la instancia compartida de memoria en Data/. Al invocar la vista de la arena de combate (Arena.cshtml), el servidor despacha la estructura HTML que enlaza los scripts de lógica de juego del cliente en wwwroot/js/engine/, abstrayendo por completo el procesamiento en tiempo real del servidor.

**Audiencia:** Desarrolladores encargados del mantenimiento técnico o de la incorporación de nuevos luchadores y lógicas físicas al motor de juego.

## **3\. Vista de procesos**

Muestra el **comportamiento en ejecución** del sistema durante su escenario más dinámico y crítico: el ciclo de vida completo de un combate local. Ilustra la interacción de eventos reactivos desde los periféricos de entrada hasta la persistencia final.

<img width="746" height="593" alt="Screenshot 2026-06-05 131703" src="https://github.com/user-attachments/assets/1124c392-f7b2-4f28-9bef-a9b796dadb2d" />


**Flujo Dinámico:** Los usuarios interactúan enviando comandos asíncronos concurrentes sobre el mismo periférico. El script input.js mapea las ráfagas discretas de interrupción. El bucle de juego sincroniza a una tasa fija de 60 FPS las rutinas de físicas, colisiones de cajas de golpe (*hitboxes*) y actualización de la vista gráfica. Al gatillarse la condición de resolución de victoria, el ciclo del cliente se rompe y dispara un hilo HTTP POST asíncrono hacia el backend para consolidar el registro de la partida.

**Audiencia:** El arquitecto de software, para validar el comportamiento síncrono del motor gráfico frente al aislamiento asíncrono de la comunicación con el servidor.

## **4\. Vista de despliegue y física**

Muestra la **distribución física, empaquetamiento y los entornos de hardware** donde residen los componentes de software, describiendo la infraestructura lógica frente a los componentes de hardware tangibles.
<img width="688" height="508" alt="Screenshot 2026-06-05 131713" src="https://github.com/user-attachments/assets/29d2edfe-fee9-4039-a480-24c6f363e6f5" />


**Infraestructura y Hosting:** El sistema opera actualmente bajo un esquema unificado de desarrollo local (localhost). El servidor web ligero **Kestrel** aloja y compila la lógica de la solución en capas, administrando las colecciones de persistencia directamente sobre el direccionamiento volátil de su memoria **RAM**. El cliente interactúa dentro de la caja de arena del navegador del terminal consumiendo directamente recursos de la **GPU** dedicada para acelerar el renderizado del Canvas sin comprometer hilos de procesamiento de la CPU central.

A futuro, se proyecta el despliegue del host web en plataformas de nube (Azure App Services o AWS EC2). Debido a la naturaleza volátil del almacenamiento actual en memoria RAM, esta miigración requerirá integrar una instancia de base de datos relacional externa con un motor persistente permanente (SQL Server o PostgreSQL) acoplado mediante Entity Framework Core.

**Audiencia:** DevOps, ingenieros de despliegue y personal técnico de soporte encargados de planificar las capacidades de hardware y hosting del proyecto.

## **Declaración de uso de IA**

Este ADR y la estructuración técnica de sus diagramas complementarios bajo el estándar del modelo 4+1 se desarrollaron con el apoyo de un asistente de inteligencia artificial para la optimización de la redacción formal, consistencia estilística y generación de los bloques de sintaxis nativa de Mermaid. El análisis de requerimientos, las decisiones arquitectónicas, las mecánicas del juego y el desarrollo total del código fuente son de exclusiva autoría y responsabilidad del diseñador del proyecto.
