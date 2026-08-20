# The Rift - Documento de Diseno y Sistemas

## Vision General

**Genero:** Terror / aventura VR estatica.

**Premisa:** El jugador interpreta a un piloto confinado en un submarino sarcofago metalico. La nave explora el interior de un ser cosmico consumidor de estrellas que esta muriendo por culpa de unos parasitos.

La experiencia se apoya en tres pilares:

- **Confinamiento:** el jugador no abandona la cabina. La tension nace de estar encerrado dentro de una maquina vulnerable.
- **Interaccion fisica:** los sistemas del submarino se activan y manipulan mediante mandos, botones, joysticks, diales y pantallas diegeticas.
- **Informacion limitada:** el mundo exterior se entiende a traves de sonar, hologramas, monitores, sonidos, vibracion, luces de advertencia y lecturas parciales.

## Estructura de Juego

### Fase 1: El Despertar y el Descenso

**Secuencia 0 - El Vacio**

El jugador aparece en un vacio negro. Un texto dramatico introduce el tono de la experiencia.

**Secuencia 1 - Sellado**

El vacio se desvanece y aparece la cabina. La escotilla se cierra mediante una secuencia de sellado con soldadura, particulas y sonido metalico intenso.

**Secuencia 2 - Inmersion**

El submarino es desplazado por una grua. Se utilizan sonido mecanico, vibracion haptica y silencio para marcar el momento en el que la grua lo suelta. Tras la caida, el impacto contra el agua da paso al audio envolvente de presion e inmersion.

**Objetivos iniciales**

- Arrancar el submarino.
- Encender sistemas progresivamente.
- Recibir instrucciones en la pantalla de lore.
- Maniobrar hacia coordenadas y checkpoints iniciales.

### Fase 2: Minijuego 1 - Extraccion de Mineral

**Objetivo:** navegar hasta el punto amarillo del holograma.

**Mecanica:** despliegue de taladro industrial. El jugador debe perforar la roca ajustando la velocidad para evitar romper la broca.

**Feedback previsto:** vibracion haptica en mandos, luces de tension en paneles y respuesta sonora del taladro.

**Giro diegetico:** al principio parece una extraccion minera normal, pero durante la perforacion el monitor puede sufrir interferencias y mostrar un analisis inesperado del material. En vez de identificarlo como roca o mineral, el sistema podria devolver lecturas como:

```text
ANALISIS DE MUESTRA
COMPOSICION: TEJIDO CALCIFICADO
ACTIVIDAD RESIDUAL: DETECTADA
```

La idea es que el jugador siga haciendo una tarea industrial aparentemente rutinaria, mientras la interfaz empieza a revelar que el entorno no es geologico, sino biologico.

**Interferencia de frecuencia:** durante esta fase tambien se puede introducir una transmision parcial de un piloto anterior. No debe explicarse de forma directa; funcionaria mejor como pista ambiental. Por ejemplo, una frecuencia capturada de forma accidental podria contener fragmentos de voz, texto corrupto o coordenadas que coinciden con zonas que el jugador todavia no ha visitado.

### Fase 3: Tension y Sigilo

Durante el trayecto hacia el segundo punto, el sonar detecta una anomalia.

El monitor frontal avisa al jugador y recomienda apagar todos los sistemas. La mecanica de sigilo se basa en silencio radiofonico: el jugador debe apagar manualmente luces, motores y sistemas para reducir la probabilidad de ser detectado por una criatura externa.

### Fase 4: Minijuego 2 - Sintonizacion de Radio

**Objetivo:** llegar a la zona de estudio.

**Mecanica:** despliegue de una consola de radio. El jugador usa diales fisicos para sintonizar una frecuencia precisa y escuchar un mensaje encriptado.

**Eventos de audio:**

- Una frecuencia captura sonidos agonizantes, similares a gritos humanos.
- Otra frecuencia captura una respiracion grave con filtro de paso bajo y latidos muy leves.
- Una frecuencia secundaria puede recuperar fragmentos de un piloto muerto antes que el jugador. El contenido deberia insinuar repeticion, fracaso o advertencias incompletas, sin confirmar explicitamente quien era ni que le ocurrio.

Esta mecanica puede conectarse con la interferencia del taladro: primero se escucha o se lee algo fuera de lugar durante la extraccion, y mas adelante el minijuego de radio permite perseguir esa pista con mas intencion.

### Fase 5: Minijuego 3 - Fotografias

**Objetivo:** usar una camara de rayos X para fotografiar secciones especificas.

El jugador navega por zonas concretas de una caverna y realiza capturas usando la pantalla frontal como disparador.

Al tomar la tercera foto, se revela el parasito que ha estado acechando al jugador. La pantalla muestra:

> FORMA DE VIDA HOSTIL, ABANDONE INMEDIATAMENTE LA ZONA

Aparece un marcador de huida.

### Fase 6: Minijuego 4 - El Revelado

**Objetivo:** lanzar una baliza de profundidad.

**Mecanica:** control remoto de proyectil con punto de vista en pantalla. El jugador navega por tuneles desde una interfaz remota.

**Giro:** la baliza sale a una cavidad mayor y descubre un corazon gigante latiendo.

**Estado:** estatica en pantalla y sonido estremecedor de latido.

### Fase 7: El Colapso

Fallo critico de sistemas. El corazon empieza a colapsar bajo su propio peso, generando una supernova lenta: una bola de luz creciente que acaba envolviendo todo.

Fin de la experiencia.

## Sistemas Implementados

### Sistema de Pantalla de Lore

El sistema de lore permite mostrar informacion narrativa y tutorial en un monitor diegetico de la cabina.

Archivos principales:

- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/MonitorSlideData.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/MonitorSequenceData.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/MonitorSequenceDirector.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/MonitorSlideManager.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/CustomSlideTextAnimator.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/MonitorSlideAudioPlayer.cs`

#### Slide Data

`MonitorSlideData` es un ScriptableObject que define una diapositiva individual.

Cada slide puede usar dos modos:

- **Default UI:** usa imagen, texto y layout base del monitor.
- **Custom Prefab:** instancia un prefab propio dentro del contenedor de la pantalla.

Tambien incluye:

- Texto de display.
- Imagen de display.
- Prefab personalizado.
- Delay de autoavance.
- Configuracion de audio por slide.

#### Secuencias

`MonitorSequenceData` agrupa una lista ordenada de `MonitorSlideData`. `MonitorSequenceDirector` reproduce esa secuencia, espera a que cada slide termine y permite avanzar mediante interaccion.

El sistema permite:

- Reproducir una secuencia completa.
- Reproducir una slide individual.
- Saltar el typewriter si el jugador interactua.
- Invocar eventos al completar una secuencia.

#### Manager Visual

`MonitorSlideManager` se encarga de renderizar la slide activa.

En modo default:

- Activa la UI base.
- Coloca la imagen.
- Escribe el texto caracter a caracter.
- Muestra una flecha parpadeante al terminar.

En modo prefab:

- Oculta la UI default.
- Instancia el prefab custom.
- Busca `CustomSlideTextAnimator` para animar los textos internos.

#### Audio de Slides

`MonitorSlideAudioPlayer` centraliza el audio del monitor de lore.

Cada slide puede definir:

- Sonido al iniciar.
- Sonido de typing.
- Intervalo de caracteres para reproducir typing.
- Delay minimo entre sonidos.
- Volumen.
- Variacion de pitch.

Esto permite que las slides default y custom compartan una misma salida sonora del monitor.

#### Corrupciones del Monitor

El monitor puede usarse como canal de comunicacion inestable entre el submarino, el sistema de mision y la consciencia moribunda del ser cosmico.

La idea no es crear otro monitor aparte, sino aprovechar el sistema de slides existente:

- Slides normales para instrucciones, objetivos y analisis.
- Slides corruptas para mensajes intrusivos.
- Slides custom para glitches, ruido, simbolos, lecturas biologicas o texto roto.
- Eventos de secuencia para disparar audio, cambios de luz, estatica o alteraciones de material CRT.

Ejemplos de uso:

```text
TRANSMISION INTERRUMPIDA
...NO EXTRAIGA...
...NO ES ROCA...
```

```text
ERROR DE CLASIFICACION
MATERIAL: ORGANICO / MINERAL
ESTADO: DOLOR RESIDUAL
```

Estas corrupciones pueden aparecer periodicamente o como respuesta a acciones concretas del jugador: perforar, sintonizar una frecuencia, fotografiar una zona sensible o acercarse demasiado a una pared organica.

Para implementarlo de forma limpia, conviene tratarlas como `MonitorSequenceData` independientes que se puedan lanzar desde `GameEvent`, `StoryStepData` o desde el propio minijuego que las necesite.

### Sistema de Typing Custom

`CustomSlideTextAnimator` permite que un prefab personalizado revele varios textos con efecto typewriter.

Modos:

- **Sequential:** revela un texto tras otro.
- **Simultaneous:** revela todos los textos al mismo tiempo.

El sistema emite callbacks al revelar caracteres, lo que permite reutilizar el audio de typing del monitor sin acoplar el prefab al sistema de audio.

### Image Blink

Archivo:

- `Assets/_ProjectRift/_Scripts/Submarine/LoreScreen/ImageBlink.cs`

`ImageBlink` es un componente para imagenes UI. Controla el parpadeo modificando el alfa de una `Image`.

Permite:

- Parpadeo instantaneo.
- Parpadeo interpolado.
- Alfa visible.
- Alfa oculto.
- Duracion visible.
- Duracion oculta.
- Duracion de transicion.
- Reproducir al activar.
- Restaurar alfa inicial al desactivar.

Uso actual previsto:

- Flechas de avance.
- Indicadores de atencion.
- Elementos de monitor que necesitan comportamiento simple de blink.

### Estetica CRT / VHS del Monitor

El monitor de lore se apoya en una capa visual por encima del canvas.

Estrategia usada:

- Una `Image` overlay como ultimo hijo del canvas.
- Material custom de CRT asignado manualmente.
- Control manual desde el material para evitar dependencias inestables del editor.

Efectos buscados:

- Scanlines.
- Ruido.
- Flicker.
- Tinte verde/rojo.
- Sensacion de monitor antiguo de ciencia ficcion industrial.

### Video en Pantallas

El flujo recomendado para video en la pantalla del monitor es:

- `VideoPlayer`
- `RenderTexture`
- `RawImage`

Estructura esperada:

```text
Canvas
├── Contenido de slide
├── RawImage con RenderTexture del video
└── Overlay CRT
```

Notas:

- Evitar GIFs en runtime.
- Usar MP4/H.264 para videos sencillos.
- Usar `RawImage` para mostrar `RenderTexture`; `Image` no es el componente adecuado para ese caso.
- Si el video tiene audio, asignar un `AudioSource` al `VideoPlayer`.

### Display de Proximidad del Submarino

El display de proximidad representa en cabina la cercania del submarino a superficies externas.

Archivos principales:

- `Assets/_ProjectRift/_Scripts/Submarine/ProximityDisplay/SubmarineProximityRaycaster.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/ProximityDisplay/SubmarineProximityDisplay.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/ProximityDisplay/ProximityAxisController.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/ProximityDisplay/ProximitySegmentVisual.cs`

#### Sensor

`SubmarineProximityRaycaster` lanza raycasts en los seis ejes locales del submarino:

- Forward
- Backward
- Right
- Left
- Up
- Down

Devuelve valores normalizados:

- `1`: no hay obstaculo o esta al final del rango.
- `0`: obstaculo muy cerca del origen del raycast.

El sensor mantiene valores fiables. Cualquier falseo de lectura para cabina se aplica en el display, no en la deteccion.

#### Display

`SubmarineProximityDisplay` lee el sensor y transforma la distancia en segmentos visuales.

Responsabilidades:

- Activar o desactivar el aparato.
- Leer cada eje.
- Remapear la distancia para exagerar la proximidad visual.
- Calcular cuantos segmentos se encienden.
- Aplicar patron de parpadeo.
- Lanzar audio de alerta.

Parametros relevantes:

- `_maxProximityValue`: a partir de que valor del sensor se considera proximidad maxima visual.
- `_activationDeadZone`: margen para mantener el eje apagado si el peligro es minimo.
- `_synchronizeBlink`: decide si todos los ejes usan reloj global o si cada eje empieza su propio ciclo al activarse.
- `_blinkPatterns`: define duracion encendida y apagada por nivel de proximidad.

#### Patrones de Parpadeo

El parpadeo no usa ya una velocidad simetrica. Cada nivel usa un patron:

```text
Nivel 1: ON 1.00s / OFF 0.125s
Nivel 2: ON 0.60s / OFF 0.125s
Nivel 3: ON 0.30s / OFF 0.10s
Nivel 4: ON 0.12s / OFF 0.08s
```

La intencion es que, a baja proximidad, el aparato este casi siempre encendido con cortes breves. A alta proximidad, el parpadeo se vuelve mas urgente y nervioso.

#### Audio de Proximidad

El display usa un unico `AudioSource` y un unico clip de blink.

El sonido se reproduce cuando un eje pasa de apagado a encendido dentro de su ciclo de parpadeo.

Esto permite:

- Mismo sonido para todos los ejes.
- Multiples ejes sonando a la vez mediante `PlayOneShot`.
- Sin necesidad de un `AudioSource` por eje.

#### Segmentos Visuales

`ProximityAxisController` agrupa los segmentos de un eje y decide cuales deben encenderse.

Permite:

- Llenar desde el centro hacia fuera.
- Llenar desde fuera hacia el centro.
- Controlar una luz opcional por eje.

`ProximitySegmentVisual` controla el aspecto de una bombilla:

- Renderer.
- Color encendido/apagado.
- Emission encendida/apagada.
- Luz opcional.

La recomendacion para rendimiento es usar emission por segmento y, si hace falta iluminacion real, una luz por eje en vez de una luz por bombilla.

### Sistema de Holograma

Archivos:

- `Assets/_ProjectRift/_Scripts/Hologram/HologramSync.cs`
- `Assets/_ProjectRift/_Scripts/Hologram/HologramChunk.cs`

El sistema de holograma esta pensado para representar informacion espacial del entorno o del submarino en una escala reducida.

Uso previsto:

- Mostrar puntos de navegacion.
- Representar objetivos de mineria.
- Mostrar checkpoints o marcadores de huida.
- Dar soporte visual a fases de exploracion y orientacion.

### Sistema de Transmisiones y Frecuencias

El sistema de radio todavia esta planteado como minijuego, pero narrativamente puede funcionar como una extension del monitor y del sistema de historia.

Usos previstos:

- Sintonizar frecuencias con diales fisicos.
- Reproducir audio diegetico desde la cabina.
- Mostrar subtitulos o transcripciones corruptas en el monitor.
- Lanzar secuencias de lore vinculadas a una frecuencia concreta.
- Introducir el rastro del piloto anterior sin convertirlo en exposicion directa.

La pista del piloto muerto funcionaria mejor si aparece en varias capas:

- Primero como interferencia accidental durante el taladro.
- Despues como frecuencia localizable en el minijuego de radio.
- Mas adelante como mensajes que parecen anticipar errores del jugador.

El objetivo es que el jugador pueda sospechar que alguien hizo el mismo recorrido antes, pero sin recibir una explicacion cerrada.

### Sistema de Historia

Archivos:

- `Assets/_ProjectRift/_Scripts/Story/StoryCampaignData.cs`
- `Assets/_ProjectRift/_Scripts/Story/StoryChapterData.cs`
- `Assets/_ProjectRift/_Scripts/Story/StoryStepData.cs`
- `Assets/_ProjectRift/_Scripts/Story/StoryManager.cs`

Este sistema estructura la progresion narrativa en campana, capitulos y pasos.

Uso previsto:

- Encadenar objetivos.
- Lanzar eventos de capitulo.
- Mantener la progresion del jugador.
- Coordinar sistemas como lore screen, controles, escenas y eventos.

### Game Events

Archivos:

- `Assets/_ProjectRift/_Scripts/Events/GameEvent.cs`
- `Assets/_ProjectRift/_Scripts/Events/GameEventInvoker.cs`
- `Assets/_ProjectRift/_Scripts/Events/GameEventListener.cs`

Los Game Events funcionan como ScriptableObjects para desacoplar emisores y receptores.

Uso previsto:

- Inicio y final de capitulos.
- Finalizacion de slides.
- Activacion de misiones.
- Comunicacion entre sistemas sin referencias directas.

### Misiones y Areas

Archivos:

- `Assets/_ProjectRift/_Scripts/Missions/MissionAreaBase.cs`
- `Assets/_ProjectRift/_Scripts/Missions/MissionTest.cs`

El sistema de misiones permite definir zonas u objetivos que reaccionan a la presencia o progreso del jugador.

Uso previsto:

- Checkpoints de navegacion.
- Zonas de mineria.
- Zonas de estudio.
- Activacion de eventos narrativos al entrar en areas.

### Control del Submarino

Archivos:

- `Assets/_ProjectRift/_Scripts/Submarine/SubmarineTestController.cs`
- `Assets/_ProjectRift/_Scripts/Submarine/DrillJoystickInputReader.cs`

El control del submarino se apoya en inputs fisicos y lectura de joysticks.

Uso previsto:

- Orientar el submarino.
- Navegar a checkpoints.
- Controlar sistemas desplegables como taladro o proyectil remoto.

## Herramientas de Editor

### Botones de Editor

Archivo:

- `Assets/_ProjectRift/_Scripts/CustomEditor/EditorButtonAttribute.cs`

Permite marcar metodos con `[EditorButton]` para exponer botones de debug en el inspector.

Uso actual:

- Encender/apagar luces del display de proximidad.
- Testear secuencias o sistemas durante Play Mode.

Nota tecnica: el injector global sobre `MonoBehaviour` es comodo, pero conviene usarlo con cuidado porque afecta a muchos inspectores del proyecto.

### Inspectores Custom

Archivos:

- `Assets/_ProjectRift/_Scripts/CustomEditor/MonitorSlideDataCE.cs`
- `Assets/_ProjectRift/_Scripts/CustomEditor/MonitorSequenceDirectorCE.cs`
- `Assets/_ProjectRift/_Scripts/CustomEditor/GameEventEditor.cs`

Estos inspectores simplifican el flujo de trabajo:

- `MonitorSlideDataCE`: muestra campos distintos segun el tipo de slide y permite multi-editing.
- `MonitorSequenceDirectorCE`: muestra botones de testeo de slides/secuencias.
- `GameEventEditor`: permite invocar eventos en Play Mode.

## Relacion Entre Sistemas y Fases

| Fase | Sistemas Principales |
| --- | --- |
| Despertar y descenso | Story, Game Events, Lore Screen, audio diegetico |
| Arranque | Lore Screen, MonitorSlideData, eventos de sistemas |
| Maniobra | Submarine controller, holograma, proximity display |
| Mineria | Drill input, haptica, luces de tension, misiones, analisis biologico corrupto |
| Sigilo | apagado de sistemas, audio ambiental, eventos de deteccion |
| Radio | diales fisicos, audio diegetico, Story, transmisiones del piloto anterior |
| Fotografias | camara/pantalla, triggers, marcador de huida |
| Baliza | control remoto, monitor frontal, audio de heartbeat |
| Colapso | fallo de sistemas, luces, CRT/static, audio final |

## Principios de Implementacion

- Mantener la cabina como interfaz principal.
- Evitar UI flotante no diegetica siempre que sea posible.
- Separar deteccion, logica y visualizacion.
- Usar ScriptableObjects para datos narrativos y eventos reutilizables.
- Mantener los sistemas visuales configurables desde inspector.
- Centralizar audio por aparato cuando sea posible, evitando un `AudioSource` por elemento pequeno.
- Usar efectos manuales simples cuando sean mas estables que automatizaciones complejas de editor.

## Pendientes y Proximos Pasos

- Consolidar el flujo completo de capitulos en `StoryManager`.
- Definir prefabs finales para cada herramienta de cabina.
- Crear secuencias definitivas de `MonitorSlideData` para cada fase.
- Integrar audio espacial y haptica por evento.
- Montar minijuego de taladro con feedback de tension.
- Anadir evento de analisis de material extrano durante la perforacion.
- Crear secuencias corruptas del monitor para interferencias de la consciencia cosmica.
- Definir fragmentos de audio/texto del piloto anterior y repartirlos entre taladro, radio y eventos posteriores.
- Montar minijuego de radio con diales fisicos.
- Montar sistema de camara/fotografia y revelado de amenaza.
- Montar control remoto de baliza/proyectil.
- Definir estados globales del submarino: encendido, sigilo, alerta, fallo critico y colapso.
