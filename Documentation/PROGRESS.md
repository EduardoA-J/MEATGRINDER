# Progreso

Actualización vigente: 26 de noviembre de 2026.
Alcance: juego completo (`GDD_SCOPE_ADDENDUM.md`). Estados: `MASTER_CHECKLIST.md`.

## Versus con Photon Fusion — 26 de noviembre

Se integraron los demonios ligero y pesado. Sus cápsulas de combate, salud, movilidad y ataques se definen por perfil. Cada `MonsterDefinition` enlaza un prefab visual provisional reemplazable desde el Inspector. Las reglas de recursos y victoria figuran en D-13 y D-14.

Para sustituir el arte, asignar el prefab final al campo **Visual Prefab** de `DemonLight.asset` o `DemonHeavy.asset` en `02_ScriptableObjects/Resources`. Preparar la raíz del modelo con el pivote a los pies, eje Y hacia arriba y frente +Z. Las hitboxes y los sockets de juego quedan en el cuerpo autoritativo; el modelo remoto solo aporta imagen.

Las sondas de Private Match 2v2 y Campaign 4v4 pasaron en cuatro y ocho ejecutables Win64. Confirmaron equipos equilibrados, cambio de demonio al reaparecer y resultados por bajas o por salidas. Campaign recorrió M01–M04 en ambas mitades. Las pruebas se automatizaron en un solo equipo. El balance con personas y el rendimiento en equipos separados se mantienen pendientes.

Se generó una nueva build Win64 con el App ID local. La prueba con dos procesos de Versus Continuous pasó por conexión directa y por relay simulado. Se corrigió la duplicación de identificadores de combate al clonar al jugador de plantilla. La validación en dos equipos distintos sigue pendiente. [Procedimiento y alcance](NETWORK_VALIDATION.md).

La pantalla de creación y unión usa uGUI y reutiliza `GameUi` y el fondo del menú principal. El lobby y sus mensajes de carga, pausa y resultados comparten el mismo panel.

Boot incluye Multijugador con salas por código, lobby, Campaign con intercambio de roles tras recorrer M01–M04, Continuous con rotación automática y Private Match con mapa y fuego amigo configurables. El host confirma movimiento, combate, bajas, progresión de puertas, pickups y resultados. La práctica local conserva su flujo. Se compilaron los ensamblados afectados con las referencias de Unity 6000.5.6f1. Falta conectar dos procesos con un App ID local y validar la jugabilidad de red. Ranked requiere identidad y un backend de resultados confiable.

## Interfaz compartida, escala y fondos originales — 26 de noviembre

GameUi centraliza fuentes y paleta de controles: Barlow Semi Condensed SemiBold para lectura/etiquetas, IBM Plex Mono para datos y Metal Mania para marca y encabezados expresivos. Se reutilizan las fuentes existentes con sus avisos SIL OFL 1.1. Botones, sliders, opciones, créditos, creación local, pausa, resultados, mensajes, carga y HUD comparten paneles metálicos, bordes, indicadores segmentados y estados normal/hover/seleccionado/pulsado/deshabilitado. Se actualizan también los rótulos TextMesh de M01–M04 y los tres prefabs de blancos. No se modifica Fusion ni las reglas de combate.

La petición posterior del usuario conserva PrincipalMenu y level1–level4 intactos: sin material de recoloreado, tinte rojo/gris ni velo oscuro sobre las imágenes. Se mantiene el recorte proporcional sin deformación. El oscurecimiento de pausa sigue limitado a la vista de juego. La referencia común pasa de 1600×900 a 1920×1080: textos, botones y HUD se reducen un 16,7 % a igual resolución. No se añade todavía un ajuste de escala al menú.

LocalGameSession conserva cada pantalla de carga al menos 3 segundos (campo serializado minimumLoadingSeconds). El porcentaje sigue reflejando la operación real; si acaba antes, se mantiene el 100 % hasta completar ese mínimo. El jugador continúa sin entrada durante esa espera. Se conservan las transiciones y la integración de música/DeathSfx de la pasada anterior.

Verificación realizada antes de la instrucción de detener pruebas: scripts compilados en Unity 6000.5.6f1. Primera pasada: 25/26 PlayMode; el caso nuevo de identidad falló por comparar Color con el Color32 serializado de TextMesh y se corrigió la comparación. La segunda pasada completó la prueba de identidad, estados, HUD y cargas de M01–M04; resultado global **25/26**, con un error de E/S Win32 1224 al escribir `product/M04-spawn.png` en la prueba de mapas. No se presenta como una suite totalmente verde. Evidencia final en [playmode.xml](test-results/ui-redesign/playmode.xml) y capturas en [ui-redesign](test-results/ui-redesign). Se conservaron los XML/capturas históricos de la pasada de media. No se repitió EditMode ni se generó un ejecutable en esta entrega. Por petición explícita del usuario se detienen nuevas pruebas y se procede a commit/push.

## Fondos y audio de sesión — 25 de noviembre

Solicitud: integrar los recursos aportados en `Assets/_Project/05_ArtModels/Materials/Menu`, `04_Audio/Music` y `04_Audio/SFX/DeathSfx.mp3`. `PrincipalMenu.jfif` es el fondo del menú y de la carga de Boot; `level1`–`level4` se asignan explícitamente a M01–M04 mediante sus MapDefinition. La carga consulta el mapa de destino, no el mapa que se abandona. Se conservan barra, porcentaje y transiciones de páginas. Los fondos cubren la pantalla mediante recorte centrado proporcional, sin estirar. La capa oscura de esta pasada fue retirada el 26 de noviembre por petición del usuario.

Los JFIF originales se conservan y un importador de Editor los convierte en texturas de Unity con GUID estable. El prefab persistente LocalGameSession contiene SessionMusic: un solo AudioSource musical, PrincipalMenu en bucle, canción numerada una vez y después afterFirstSong en bucle. Pausa, opciones, muerte, respawn, resultados y reinicio del mismo mapa conservan la pista; entrar a un mapa diferente inicia su canción. La música anterior continúa durante la carga hasta activar el destino. El volumen maestro existente controla también música y muerte. Las seis canciones usan streaming para evitar descomprimirlas enteras en memoria.

CombatAudio usa el DeathSfx aportado mediante ProductTheme y un canal independiente. El evento de muerte reproduce una vez, con guardia reiniciada por Respawned. SessionMediaInstaller asigna referencias al prefab y assets existentes sin reconstruir mapas; el instalador general también conserva esta integración.

Verificación en Unity 6000.5.6f1: **44/44 EditMode y 25/25 PlayMode**, sin fallos. Evidencia en [editmode.xml](test-results/session-media/editmode.xml) y [playmode.xml](test-results/session-media/playmode.xml). La prueba nueva recorre M01–M04, comprueba fondos de destino, reproducción real de cada canción, final natural del clip (avanzando cerca del final), paso a afterFirstSong en bucle, continuidad al pausar/reiniciar, dos muertes separadas por respawn y retorno a la música del menú. También se comprueban proporciones a 1366×768, 1920×1080, 1024×768, 2560×1080 y 600×900. Las pruebas existentes de flujo, barra/progreso, Startup, puertas, navegación y combate siguen pasando. Capturas reales revisadas del menú y las cuatro cargas a 2560×1440 en [session-media](test-results/session-media). Esta pasada se validó en el Editor; no se generó un nuevo ejecutable. Sin cambios de Fusion, commits, merge ni push. Esta entrega no cierra el conjunto completo de producción/mezcla de audio ni el menú de red.

## Auditoría, interfaz y carga local — 24 de noviembre

**Base comprobada en Unity 6000.5.6f1.** La revisión 89dedc8 pasó 44/44 en EditMode y 19/19 en PlayMode desde una copia aislada. Los resultados de esta pasada se comparan con esa línea base. El [estado vigente](IMPLEMENTATION_STATUS_AUDIT.md) incluye el vínculo de escenas, prefabs y modelos con el GDD/TDD y la matriz funcional de la sección 21.1.

- **Tipografía**. Metal Mania en la marca, Barlow Semi Condensed en botones, encabezados y texto de lectura, IBM Plex Mono en cifras y texto de sistema. El HUD conserva su jerarquía para vida, armadura, reservas y potenciadores. Barlow industrial también se aplica a 18 rótulos de los cuatro mapas y a tres etiquetas de los prefabs de blancos, emparejando fuente y material. [Fuentes y avisos SIL OFL 1.1](../grinder/Assets/_Project/05_ArtModels/Resources/MeatgrinderFonts)
- **Interfaz**. Los botones uGUI tienen estados de foco, hover, pulsación y selección, con realce, desplazamiento corto y audio. Las páginas funden y desplazan su contenido en 0,19 s con tiempo independiente de la pausa. La página saliente deja de recibir entrada. El ajuste de movimiento reducido evita desplazamientos y transiciones. Las acciones de navegación siguen en el menú y los cambios de gameplay siguen a cargo de la sesión.
- **Carga**. Startup es el primer punto de entrada de la compilación y carga Boot de forma asíncrona. La misma pantalla acompaña la entrada a arena, el cambio por puerta, el reinicio y el retorno al menú. El fondo usa las texturas de metal del proyecto. La barra horizontal y el número reflejan la operación de escena de Unity. El 100 % aparece durante un cuadro tras completarse la operación. La sesión mantiene el input de juego bloqueado durante la carga.
- **Uso**. Abrir Startup.unity desde Unity para probar el arranque completo. Boot.unity sigue disponible para acceder al menú directamente desde el Editor. El build de Windows se genera con ProductSceneInstaller.BuildWindows.

La auditoría, la carga y la presentación se trabajaron con agentes de Codex mediante los handoffs de game-team-orchestrator. Se aplicaron AGENTS.md, las reglas de arquitectura, UI y QA, y la guía local ui/ui-ugui para conservar el flujo uGUI.

**Pruebas de Unity 6000.5.6f1.** EditMode **44/44** y PlayMode **23/23**. Los cuatro casos añadidos comprueban carga al iniciar, progreso ordenado hasta el 100 %, rechazo de un mapa inexistente y reinicio, navegación y raycast en pausa con movimiento reducido, y resolución de fuentes con caracteres del español. Las ampliaciones de los casos de producto cubren la barra al entrar por el menú, al cruzar la puerta M01→M02 y al volver a Boot. La revisión de una captura de 1366×768 encontró la marca cortada. Se ajustó su caja y se añadió una aserción de encaje con la resolución activa de PlayMode. También pasaron las pruebas previas de movimiento, combate, recursos y los cuatro mapas.

XML y logs de esta pasada en la copia local de QA `D:/MEATGRINDER_REFERENCE/MEATGRINDER-QA-20261124`, archivos `verified-editmode.xml` y `verified-playmode.xml`. La revisión visual de capturas de 1366×768 cubrió menú, carga de arranque, carga de arena, HUD de poderes y creación local. La inspección de FBX escribió `D:/MEATGRINDER_REFERENCE/Documentation/test-results/product/models.txt`.

**Windows Standalone x64 Development.** Build en 6000.5.6f1, salida 0, 0 errores y 193.727.340 bytes reportados. El ejecutable con `--meatgrinder-smoke` devolvió 0 y registró PASS en Startup → Boot → M01 con melee → pausa → resultados → Boot. La misma secuencia pasó también en el ejecutable con gráficos D3D12 en una Radeon RX 550X, en modo ventana con resolución solicitada de 1366×768, y devolvió 0. Las capturas inspeccionadas se tomaron en PlayMode. Se conserva F10-06 parcial hasta probar otra máquina sin Editor y crear/unir sesión de producto. Evidencia en `windows-build.log`, `windows-smoke.log` y `windows-smoke-graphics.log`, en la copia de QA.

**Checklist actualizado.** 28 VALIDATED, 31 INTEGRATED, 18 PARTIALLY_IMPLEMENTED, 4 BLOCKED y 49 NOT_STARTED. De 130 tareas, el **21,54 %** está validado y el **45,38 %** está integrado o validado. Las comprobaciones de práctica local no cuentan como validación en red. [Método y recuento por épica](IMPLEMENTATION_STATUS_AUDIT.md)

Siguen pendientes Host Mode en el flujo de juego, dos clientes, Versus, monstruos jugables, Ranked, recorridos completos, balance, rendimiento, rigs/LOD y producción de audio. Los ajustes de accesibilidad del GDD pendientes están en F7-09.

## Historial de integración local

### Integración de producto local y presentación — 22 de noviembre

Estado de esa pasada: Boot → menú uGUI → selección/carga de M01–M04 → combate local → pausa/muerte/respawn → salida con llave o fin de sesión → resultados → Boot. Escopeta, cohete y melee tienen presentación propia y recursos separados; seis power-ups tienen efectos, prefabs, símbolos y timers. Hay opciones de volumen, sensibilidad/FOV, pantalla y reducción de efectos. Los mapas reciben materiales texturados y una primera pasada de entorno; tres FBX aportados funcionan como blancos estáticos destructibles.

Uso: abrir `grinder/Assets/_Project/00_Scenes/Boot.unity` y entrar en Play, o ejecutar `grinder/Builds/LocalProduct/MEATGRINDER.exe` conservando sus carpetas adyacentes. «Jugar» abre el mapa elegido; «Crear partida local» permite revisar mapa/modo disponible, abrir ajustes y confirmar antes de cargar. Solo está disponible práctica local; no se simulan modos competitivos. Controles: WASD, ratón, Espacio, Shift sprint, Ctrl agacharse, V melee (también clic sin arma), R recarga, 1/2 selección de arma recogida, Escape pausa. Empiezas solo con melee: las armas y munición se recogen en el escenario.

Correcciones encontradas en las escenas reales: ajustes de vida/vacío sin asignar, un pickup sin definición, un componente PlayerLife duplicado, sprites importados sin sprite individual y pulso de pedestal que sobrescribía su escala. Se conectaron los assets reales y se añadieron pruebas de flujo; no bastaba la cobertura de clases de la pasada anterior.

PlayMode: **19/19**, `test-results/product/playmode.xml`. Comprueba menú/mapa/pausa/resultados, cuatro escenas con tuning y pickups, melee con preparación y cobertura, llave/puertas/transición M01→M02, recuperación del loot sin crear munición, origen real del cohete sin atravesar cobertura y muerte por vacío ≤5 s incluso con poderes, seguida de respawn melee-only. Capturas reales de juego/HUD/muerte y primera aparición de los cuatro mapas en la misma carpeta. Las pruebas existentes de movimiento y combate también pasan. EditMode: **44/44**, `test-results/product/editmode.xml`, incluidos los enlaces serializados a tuning, sprites, arma y punto de salida del cohete.

Windows Standalone x64 Development compiló sin errores, tamaño reportado 193.924.628 bytes (`test-results/product/windows-build.txt`). Se ejecutó el binario en la UHD 630 con `--meatgrinder-smoke`: **PASS**, código de salida 0, Boot → M01 melee-only → pausa → resultados → Boot (`test-results/product/windows-smoke.log`). Es una comprobación automática del flujo fuera del editor; no es medición de FPS ni sesión manual completa. El probe solo se habilita en Editor/Development y requiere ese argumento explícito.

La suite final PlayMode también recorre Crear partida → mapa → ajustes → volver → confirmación → carga. La comprobación enfocada de UI pasó **1/1** (`test-results/product/ui-flow.xml`), capturando menús/resultados tras finalizar su transición para verificar legibilidad. Unity reserializó ajustes auxiliares al compilar; se restituyeron únicamente esos cambios automáticos en GraphicsSettings, PlayerSettings y UnityConnectSettings tras verificar el diff, conservando la configuración original y el cambio preexistente de ProBuilder.

Los números de daño/movimiento siguen siendo el balance inicial configurable aprobado. El tiempo de apertura de puertas (1,2 s), preparación melee (0,12 s) y ventana de impacto (0,1 s) son tiempos de presentación/prueba configurables, pendientes de ajuste definitivo. La última puerta cierra la sesión local; no define la victoria Versus.

Checklist: **130 tareas** (el recuento anterior de 128 omitía F1-12 y F2-15); 24 VALIDATED, 33 INTEGRATED, 16 PARTIALLY_IMPLEMENTED, 4 BLOCKED y 53 NOT_STARTED. No equivalen a porcentaje de producto: las tareas tienen complejidad distinta y varias integraciones solo están acreditadas localmente.

Pendiente: autoridad Fusion y dos clientes reales, modos/equipos/victoria/roles, monstruos jugables, ranked, arsenal ampliado, rigs/LOD y arte/audio final, recorrido humano completo y perfiles de rendimiento. No se reclama juego completo ni multiplayer terminado. Base mínima de hardware, no objetivo definitivo de FPS: i5-10400/UHD630/16 GB a 1366×768.

Se conserva `ProjectSettings/Packages/com.unity.probuilder/Settings.json` preexistente. Único cambio de configuración de esta pasada: registrar Boot en Build Settings. Sin cambios de paquetes, versiones ni Fusion; sin commits/merge/push. Referencias estudiadas, comportamientos reproducidos con código propio; ver `REFERENCE_REPOSITORIES_AUDIT.md`.

## Fotografía previa (histórica, sustituida por el estado anterior)

Nota del 22 de noviembre de 2026: decisiones de diseño confirmadas por David registradas en `DECISIONS.md`, sección «Resoluciones de David». Esta pasada integró y probó localmente armadura, daño de vacío, sprint/agacharse, melee con cobertura, respawn melee-only, loot de armas/munición, rocket self-damage/rocket jump, HUD reactivo y timers de pickups. Unity EditMode: 43/43 (`Documentation/test-results/local-integration-editmode-final-2.xml`); PlayMode: 12/12 (`Documentation/test-results/local-integration-playmode-final.xml`). El cambio local preexistente en `grinder/ProjectSettings/Packages/com.unity.probuilder/Settings.json` se conserva. Fusion, Boot, menú, monstruos, autoridad de red y validación manual de mapas siguen pendientes.

Estimación sobre el checklist redefinido (~128 tareas REQUIRED): **~16–18 %** con estado ≥ `INTEGRATED` / `VALIDATED`.
Método y detalle: `IMPLEMENTATION_STATUS_AUDIT.md`. No confundir con «% de la demo antigua».

## Hecho y comprobado (evidencia histórica)

| Evidencia | Qué demuestra |
|---|---|
| Unity `6000.5.6f1` | Editor declarado abre `grinder/` |
| Fusion import 3 nov 2026 | SDK 2.1.4 Stable build 2500 compiló (T-22) |
| Sonda Host/Client 3 nov | F3-02 VALIDATED como sonda de cubos, no gameplay |
| Movimiento local 3–5 nov | EditMode/PlayMode movimiento; F1-* VALIDATED (salvo F1-03 / F1-11) |
| Combate local 4–5 nov | Escopeta + daño + HUD; F2-01/02/06/09/11/12 VALIDATED; F2-07 INTEGRATED |
| Arsenal/pickups commit `992bb92` | Scaffolding: Rocket asset/código, Pickups, armadura hooks — **no** INTEGRATED completo |

## Estado por eje (9 nov 2026)

| Eje | Resumen |
|---|---|
| Movimiento local | VALIDATED (tests). Play manual vivo pendiente (F1-03). |
| Escopeta | INTEGRATED local. VALIDATED manual pendiente. |
| Lanzacohetes | INTEGRATED local. Proyectil, explosión, autodaño y rocket jump; red pendiente. |
| Melee | INTEGRATED local. Cobertura y Berserk/Quad vía DamageResolver. |
| Salud / muerte | PARTIALLY_IMPLEMENTED. Respawn local 4 s, protección 1 s, loot y vacío; red/manual pendiente. |
| Armadura | INTEGRATED local. 100 de máximo, 50 % de absorción. |
| Pickups / power-ups | INTEGRATED local en código y kit de mapas; escenas M01–M04 regeneradas con definiciones. |
| HUD | INTEGRATED local (salud/armadura/munición/retícula/potenciadores), curación y respawn notifican. |
| Fusion gameplay | NOT_STARTED. Solo sonda. |
| Boot / menús / monstruos / ranked | NOT_STARTED |
| M01–M04 | PARTIALLY_IMPLEMENTED. Escenas ProBuilder con rutas mixtas, coberturas, spawns y los ocho power-ups configurados. Falta recorrido manual, puertas/llaves y Host Mode. T-27 |
| Lanzacohetes, melee, pickups, respawn local | INTEGRATED en código con balance provisional aprobado. Sin red |

## Checklist (recuento aproximado)

| Estado | ~Tareas |
|---|---|
| VALIDATED | ~22 |
| INTEGRATED | ~3 |
| PARTIALLY_IMPLEMENTED | ~15 |
| BLOCKED | ~12 |
| NOT_STARTED | resto (~76+) |

## Cambios locales sin commit (no descartar)

- `IDamageModifiers` + `DamageResolver` + `PowerUpManager` + asmdef Combat (rotura de ciclo).
- ProBuilder 6.1.4 (T-26). Cuatro escenas de mapa ampliadas o creadas el 9 de noviembre. La rampa de M01 que David recorrió a mano se conservó. T-27 registra los nombres de escena frente a los del GDD.
- Gameplay local: cohete en el loadout, pickups, quad, haste, melee, berserk, respawn y protección de spawn. Cifras iniciales configurables aprobadas; el balance definitivo sigue abierto.
- Diffs menores en `RocketLauncher.asset` y `EditorBuildSettings.asset`.

## No comprobado / no afirmar

- Play manual Greybox con teclado/ratón reales.
- Power-ups o cohete como «terminados» por existir clases.
- Multijugador de partida.
- Ranked y monstruos. M01–M04 no están validados: el humo del 10 nov 2026 comprueba apertura, colisión, un rayo de spawn, la rampa de M01 y que M04 no tiene suelo continuo. M02 se regeneró después y su humo de suelo volvió a pasar. No recorre escaleras, pickups ni Host Mode. El material sigue siendo el shader de color de vértice de ProBuilder.
- Balance definitivo más allá del conjunto inicial aprobado. El código lee los valores desde assets y tuning configurables.

## Historial de pasadas (conservado)

Ver secciones de movimiento, escopeta, presentación y feel en commits previos y XML en `Documentation/test-results/`. La pasada de feel del 5 nov está en el historial de git (`0b361cd` y siguientes). HEAD tipico de auditoría: `992bb92`.
