# Decisiones

Fecha base: 2 de noviembre de 2026. Actualización de alcance: 9 de noviembre de 2026.
Una decisión «tomada» está escrita en el GDD sin contradicción interna, o está materializada en el proyecto. Una decisión «pendiente» necesita la aprobación de David antes de programarla. No se cierra nada por inferencia.
Alcance del producto: [GDD_SCOPE_ADDENDUM.md](GDD_SCOPE_ADDENDUM.md).

## Identidad de interfaz y fondos — 26 de noviembre de 2026

El usuario solicita una identidad industrial/SYSTEM/death metal legible para toda la UI, con controles oscuros, metal, rojo sangre y blanco envejecido. La instrucción posterior prevalece para los fondos: conservar las imágenes originales, sin recoloreado rojo/gris ni oscurecimiento añadido. La paleta de controles no restringe los colores de las ilustraciones ni del mundo. Reutilizar Metal Mania, Barlow Semi Condensed y IBM Plex Mono existentes, con sus licencias SIL OFL 1.1; no importar recursos de Luna.

También solicita reducir textos, botones y HUD y hacer más lentas las cargas. Ajuste adoptado: referencia común 1920×1080 (antes 1600×900, reducción lineal del 16,7 % a igual pantalla) y mínimo configurable de 3 segundos por carga, conservando el progreso real y el bloqueo de entrada. El usuario autoriza finalizar, commitear y subir a la rama actual; sustituye la restricción anterior de no commit/push para esta entrega.

## Recursos de menú y música — 25 de noviembre de 2026

Decisión explícita del usuario: usar PrincipalMenu como imagen y música del menú; fondos level1–level4 y canciones homónimas para M01–M04; al terminar la primera canción de cualquier nivel, reproducir afterFirstSong como música posterior. DeathSfx debe sonar una sola vez por muerte. Conservar las transiciones y barra de carga. Implementación: afterFirstSong se repite en bucle; pausa/respawn/reinicio del mismo mapa no reinician música y cambiar de mapa sí selecciona su canción. Los recursos originales están en Assets/_Project; los nombres reales son PrincipalMenu, afterFirstSong y DeathSfx.

## Tomadas

### Resoluciones de David — 22 de noviembre de 2026

Esta sección prevalece sobre las filas históricas que contradiga. Registra decisiones de diseño, no implementación ni validación. Encargo actual: integración local y mapas; conservar Fusion sin migrar gameplay, sin instalar paquetes ni assets de pago y sin commits, merges o push.

- **D-07 cerrada:** armadura máxima 100; absorbe 50 % del daño, consumiendo un punto de armadura por punto absorbido. El daño que no puede absorber pasa a salud.
- **D-08 cerrada:** respawn tras 4 segundos, con 100 de vida, 1 segundo de protección, sin armadura ni potenciadores y solo melee. La primera aparición también es solo melee. La protección termina inmediatamente al atacar. Se elige otro punto seguro, nunca el punto de la muerte.
- **Loot de muerte:** soltar las armas y la munición que el jugador tenía, nada más, en el lugar de muerte; el jugador no recibe munición nueva al recuperarlo. El lote caduca a los 90 s. En el vacío cae con el jugador y se pierde.
- **Vacío (corrección expresa de David, 22 de noviembre):** muerte como máximo 5 segundos después de detectar la caída fuera de la zona jugable, con pérdida progresiva de vida, vibración de cámara y oscurecimiento de la vista. Se conserva la gracia configurable de 1 s y la pérdida del 25 % de vida inicial por segundo, pero al alcanzar el límite se aplica el daño letal restante. Ignora armadura, invulnerabilidad y regeneración. Respawn normal de 4 s; no hay retorno gratuito a una plataforma. `LifeTuning.MaximumVoidDuration` limita el intervalo; la opción de reducir movimiento reduce el efecto de cámara.
- **Cohetes:** autodaño y rocket jump. El autodaño usa el 25 % del daño de explosión antes de armadura; el impulso máximo provisional es 10 m/s con caída por distancia. Quad no multiplica el autodaño.
- **D-10 cerrada:** Quad multiplica por 4 el daño de armas y melee durante 30 segundos.
- **D-11 cerrada en modelo y cifras provisionales:** recoger el mismo potenciador reinicia su duración sin sumar potencia; efectos diferentes coexisten. Quad y Berserk multiplican conjuntamente el melee. Quad ×4/30 s; Berserk ×2/20 s; Invulnerabilidad 10 s; Haste ×1,3/20 s; Regeneración 10 HP/s durante 10 s; Megahealth +100 hasta 200, espera 5 s y degrada a 5 HP/s. Los valores permanecen configurables para balance.
- **D-06/D-17 cerrada:** incorporar sprint y agacharse; no incorporar dash en esta fase. Sprint 12 m/s, agacharse 4 m/s y altura reducida configurable, sin stamina; se conserva la gravedad y el salto existentes.
- **Mapas y llaves:** combinar zonas de exploración con zonas de combate puro. Las llaves abren rutas de avance y accesos opcionales a power-ups; las puertas desbloqueadas quedan abiertas para todos. Esto sustituye la prohibición anterior de llaves de recorrido. No autoriza añadir puzzles complejos.
- **D-24 cerrada:** no comprar Luna. Implementar un menú propio inspirado en Quake 2, con animaciones, pantallas de carga, ajustes y funciones equivalentes pertinentes. No se autoriza importar código ni assets de Luna. El framework de UI no fue confirmado expresamente.
- **Balance provisional aprobado:** se adopta el conjunto inicial configurable basado en las referencias estudiadas; no se presenta como balance definitivo. Escopeta 8 perdigones ×12, intervalo 0,7 s, cargador 6 y recarga 1,4 s; cohete 120 de explosión/4 m/24 m/s, cargador 4, reserva 12 y recarga 2 s; melee 20/2,2 m/0,55 s; salud y armadura +25; munición +6 cartuchos o +4 cohetes; recursos 20 s y poderes 60 s.
- **D-15 resuelta como base mínima:** el equipo i5-10400, Intel UHD Graphics 630, aproximadamente 16 GB de RAM y salida 1366 × 768 es la base mínima de pruebas. El gameplay principal se probará también en equipos superiores; su configuración y objetivo de FPS final siguen pendientes.

Las preguntas históricas de abajo permanecen como trazabilidad. No reabrir las partes resueltas en esta sección ni dar por resueltas sus cifras pendientes.

| ID | Decisión | Evidencia |
|---|---|---|
| T-01 | El juego es un arena FPS asimétrico humano contra monstruo, sin extracción, puzzles ni habilidades de héroe. | GDD 1.1 y 7.2 |
| T-02 | Plataforma del producto: Windows PC. Pipeline: URP. Lenguaje: C#. | GDD portada; `PC_RPAsset.asset` |
| T-03 | Editor declarado: Unity `6000.5.6f1`. | `grinder/ProjectSettings/ProjectVersion.txt` |
| T-04 | Red: Photon Fusion 2. Sesión de producto (Boot + menú). El dedicado de producción se evalúa cuando latencia/coste lo pidan. | GDD 13.1 y sección 25 |
| T-21 | Topología: Host Mode (Client Host). El jugador host es la autoridad de estado sobre daño, disparos, salud, pickups y puntuación. Shared Mode no se adopta sin justificación y aprobación nueva. Arquitectura preparada para 2v2 y después 4v4 (ambos REQUIRED). | Decisión de David, 3 de noviembre de 2026 |
| T-23 | Política de alcance juego completo: toda funcionalidad documentada es REQUIRED; prioridad ≠ exclusión; no demos descartables; Greybox es laboratorio. | David, 9 de noviembre de 2026; `GDD_SCOPE_ADDENDUM.md` |
| T-22 | Prueba controlada: Fusion SDK 2.1.4 Stable, build 2500, importado en Unity `6000.5.6f1`. Compiló en batchmode con código 0. Photon Realtime 5.1.20. Esta prueba no es una certificación oficial de Photon para ese editor. El App ID quedó después en el asset local, fuera de git. | `grinder/Logs/fusion-import.log` y `Assets/Photon/Fusion/release_history.txt`, 3 de noviembre de 2026 |
| T-05 | El resultado competitivo lo confirma la autoridad: daño, salud, muerte, pickup, puntuación, transición. | GDD 5.3, 13.2, 19 |
| T-06 | Fuego amigo desactivado por defecto y configurable por modo. | GDD 5.3 |
| T-07 | Arsenal REQUIRED: escopeta y lanzacohetes primero (P0); ametralladora, superescopeta y rayo REQUIRED (P-05, prioridad P3). | GDD sección 6 + T-23 |
| T-08 | Ítems REQUIRED: salud, armadura, Quad Damage, invulnerabilidad, berserk, regeneración, haste, megahealth. Las cifras iniciales configurables están fijadas en la resolución del 22 de noviembre. | GDD sección 8 + T-23 |
| T-09 | Primera aparición y respawn solo con melee; armas de fuego y munición se recogen del mapa. Sustituye la formulación inicial de «arma básica». | Resolución expresa de David, 22 de noviembre de 2026; GDD 6.2 histórico |
| T-10 | Al cambiar de mapa no se conservan las armas. Salud, armas e ítems se reinician según el modo. | GDD 10.4 y sección 25 |
| T-11 | Los monstruos pueden recoger todos los potenciadores. | GDD sección 25 |
| T-12 | Una desconexión no pausa la partida: se continúa y se marca. Bots de desconexión son REQUIRED (P-02) con prioridad posterior. | GDD 11.4 + T-23 |
| T-13 | Ranked es REQUIRED (E11 / R-01…R-04). Prioridad P4 por dependencias de backend y núcleo jugable; no está excluido del producto. | GDD 12 + T-23 |
| T-14 | Si hay retraso, se retrasa pulido visual antes que autoridad de red o combate. El alcance del checklist no se borra. | GDD sección 22 + T-23 |
| T-15 | Código en inglés. ScriptableObjects para configuración. Estado de partida en la autoridad. | GDD apéndice A y 14.3 |
| T-16 | Input System nuevo, no el Input Manager clásico. | `activeInputHandler: 1` y paquete 1.20.0 |
| T-17 | Mapas REQUIRED: M01 → M02 → M03 → M04. Orden de construcción por dependencias, no exclusión. | GDD 10.2 + T-23 |
| T-18 | El daño no se duplica por una misma confirmación. La muerte se registra una vez. Asistencias REQUIRED (P-06). | GDD 5.3 + T-23 |
| T-19 | Los efectos cosméticos no deciden daño ni resultado. | GDD 9.2 |
| T-20 | Assets originales, con licencia registrada o dominio público verificado. | GDD 2 |
| T-26 | ProBuilder `6.1.4` es la herramienta de construcción de mapas. D-23 cerrada. No hay sample URP en esa versión: el shader de bloque es `ProBuilder6/Standard Vertex Color`, dentro del paquete. La subida automática de la rampa de M01 falló; David la recorrió a mano y esa pieza se conserva. M01 no está validado. | David, 9 de noviembre de 2026. `Packages/manifest.json`, `Documentation/test-results/m01-shell-playmode.xml` |
| T-27 | Las escenas de mapa pedidas el 9 de noviembre de 2026 son `M01_TheSlaughterhouse`, `M02_CastleOfTheDamned`, `M03_RuinsOfTheForgotten` y `M04_TheLongestVoid`. Los nombres escritos del GDD para M02–M04 siguen siendo Iron Cathedral, The Pit y Blood Foundry. Esta instrucción no edita el GDD. | David, 9 de noviembre de 2026. `Documentation/MAPS.md` |

## Pendientes — necesitan aprobación

| ID | Pregunta | Por qué no se puede cerrar sola |
|---|---|---|
| D-03 | Política de lag compensation para hitscan. | El GDD la condiciona a que la arquitectura la soporte. |
| D-04 | ¿Qué modo versus se implementa primero: Campaign (cambio de roles) o Continuous? Ambos son REQUIRED. | Sección 3.2 marca los dos como alta prioridad. Sección 25 escribe «se intercambian roles». |
| D-05 | Orden de capacidad: ¿validar 2v2 antes que 4v4? Ambos son REQUIRED. | 3.3/22 vs sección 25 «4v4». Propuesta técnica: 2v2 primero por dependencias de red. |
| D-06 | Resuelta para esta fase: incorporar sprint y agacharse; excluir dash. | Resolución de David, 22 de noviembre de 2026. |
| D-07 | Resuelta arriba: armadura 100, absorción 50 %. | Resolución de David, 22 de noviembre de 2026. |
| D-08 | Resuelta arriba: respawn 4 s, protección 1 s y punto seguro distinto. | Resolución de David, 22 de noviembre de 2026. |
| D-09 | uGUI para HUD y menú propio inspirado en Quake 2. | Resolución de David, 22 de noviembre de 2026. |
| D-10 | Resuelta arriba: Quad ×4 durante 30 s. | Resolución de David, 22 de noviembre de 2026. |
| D-11 | Resuelta arriba: reinicio de duración, coexistencia y cifras provisionales. | Resolución de David, 22 de noviembre de 2026. |
| D-12 | Ligero móvil y de ataque rápido; pesado robusto y de ataque lento y amplio. | Estadísticas en los perfiles DemonLight/DemonHeavy. Cifras de balance PENDIENTES de playtest. |
| D-13 | Ambos demonios pueden recoger salud, armadura y todos los potenciadores. | Armas de fuego, munición, botín de armas y llaves de recorrido reservados a humanos. |
| D-14 | Campaign se decide por salidas completadas; Continuous y Private Match, por bajas rivales confirmadas. | Campaign compara M01–M04 tras intercambiar roles. Continuous suma las bajas de cuatro mapas; Private Match usa un mapa. Empate si hay igualdad. Suicidio y fuego amigo no puntúan. Un equipo vacío concede victoria por abandono. |
| D-15 | Base mínima resuelta; objetivo y equipo principal siguen pendientes. | Resolución de David, 22 de noviembre de 2026. |
| D-16 | Fecha de inicio de las 8 semanas. | El plan no tiene calendario. Hoy el código de juego no existe. |
| D-17 | Resuelta arriba: Sprint y Crouch entran; dash no. | Resolución de David, 22 de noviembre de 2026. |
| D-18 | Gravedad de diseño frente a PhysX `-9.81`. | El movimiento lleva su propio parámetro `Gravity` en ScriptableObject. |
| D-19 | Tolerancia visual de la reconciliación de movimiento. | 4.3 pide coincidencia «dentro de la tolerancia definida» y no la define. |
| D-20 | Ragdoll simple o solo animación de muerte en la demo. | 9.1 acepta cualquiera de los dos. |
| D-21 | Renombrar producto `grinder` / compañía `DefaultCompany` a MEATGRINDER. | Hecho de proyecto, ausente del GDD. |
| D-22 | Quitar del flujo de trabajo, sin desinstalar todavía, los paquetes de plantilla que el GDD no usa. | Navigation, Visual Scripting, Timeline y Collab Proxy están instalados. Esta fase no los toca. |
| D-24 | No comprar Luna v2. El menú propio se implementará con el framework UI ya disponible y cubrirá sus funciones pertinentes. | Resolución de David, 22 de noviembre de 2026. |

## Verificación de Fusion — 3 de noviembre de 2026

D-02 pasó a T-21. La instalación autorizada pasó a T-22. El 3 de noviembre de 2026 se importó `photon-fusion-2.1.4-stable-2500.unitypackage` con Unity `6000.5.6f1` en batchmode. El log `grinder/Logs/fusion-import.log` cierra con `Exiting batchmode successfully now!` y código 0. La última línea de compilación es `Tundra build success (20.60 seconds), 448 items updated, 977 evaluated`. Las tres anteriores también son `Tundra build success`. No hay `error CS`.

Photon Realtime dentro del SDK es 5.1.20. El 3 de noviembre de 2026 David pasó el App ID y quedó en `AppIdFusion` de `PhotonAppSettings.asset`. El valor no se copia aquí. El asset y su `.meta` están en `.gitignore`. El resto de App ID siguen vacíos. El paquete anidado `Assets/Photon/FusionDemos/Photon-Fusion-Intro-Shared-2.1.4-Stable-2500.unitypackage` no se expandió. Host Mode se mantiene.

URP sigue en 17.5.0. `PC_RPAsset.asset`, GraphicsSettings, SampleScene y `ProjectVersion.txt` no cambiaron de hash respecto a la instantánea previa. `FusionInstaller` puso `runInBackground` en 1 y escribió los defines `FUSION_2_1_4` y `FUSION_WEAVER` en PlayerSettings. El mismo instalador pide `com.unity.nuget.mono-cecil@1.10.2` si el manifiesto no contiene `nuget.mono-cecil`. El proyecto ya resolvía 1.11.6 por `com.unity.collections` 6.5.0, y el asmdef de Fusion declara `FUSION_CECIL_1_11_OR_NEWER` a partir de 1.11. El manifiesto quedó en 1.11.6, la versión ya resuelta, para no bajar el paquete que Collections exige. Ningún otro paquete del manifiesto cambió de versión.

El log avisa que `Packages/com.unity.render-pipelines.core/Runtime/Debugging/Runtime UI Resources/RuntimeDebugWindow_PanelSettings.asset` se alteró dentro de un paquete inmutable. Ese archivo no está en git. El asset de URP del proyecto no cambió.

Esta compilación no certifica `6000.5.6f1`. La matriz oficial sigue sin nombrar ese editor.

| Dato | Resultado | Fuente |
|---|---|---|
| Última estable | Fusion SDK 2.1.4 Stable, build 2500, 26 de octubre de 2026 | [SDK download](https://doc.photonengine.com/fusion/v2/getting-started/sdk-download) |
| Candidata que se había propuesto | 2.1.3 Stable, build 2390, 17 de septiembre de 2026. Queda detrás de 2.1.4 | La misma página |
| Matriz de editores | Unity 2021.3.45, 2022.3.45, 6.0.x y 6.3.x. No aparece 6.5.x ni `6000.5.6f1` | La misma página, sección Requirements |
| Changelog | Build 2159 (25 de junio de 2026): «Fixed: SDK compilation and compatibility with Unity 6000.5». Build 2174: arreglo de shaders magenta del sample intro en Unity 6.5 con URP. 2.1.3 y 2.1.4 son posteriores e incluyen esos builds | [Release notes 2.1](https://doc.photonengine.com/fusion/v2/getting-started/release-notes/release-notes-2-1) |
| Parche exacto | Photon no nombra `6000.5.6f1` | Las dos fuentes anteriores |
| Instalación oficial | El SDK se descarga con sesión en el dashboard y se importa como `.unitypackage`. No es un paquete del registro de Unity | SDK download, sección Updating the SDK |
| Serialización | El proyecto ya está en Force Text (`m_SerializationMode: 2`). Fusion lo exige | `EditorSettings.asset` |
| App ID | Presente solo en el asset local `PhotonAppSettings.asset`, ignorado por git. El valor no está en este archivo | David, 3 de noviembre de 2026 |
| Compilación de la prueba | Código 0. Último `Tundra build success (20.60 seconds)` | `grinder/Logs/fusion-import.log` |

Cambios de Fusion 2.0 a 2.1 que afectan a este juego, según [What's New in 2.1](https://doc.photonengine.com/fusion/v2/getting-started/release-notes/whats-new-2-1): Forecast Physics en Host, Server y Shared; prioridad de envío por objeto, y en Host la fija el host; RPCs grandes por canal fiable; input delay en Host y Server; tick rate de 8 Hz a 256 Hz en modos cliente-servidor; identificador estable de jugador; el hub nuevo. Fusion 2.1 exige Photon Realtime 5. Voice de 2.0 no sirve. Este proyecto no usa voz.

Plan gratuito, leído en [photonengine.com/fusion/pricing](https://www.photonengine.com/fusion/pricing) y en la documentación de precios de Photon:

- 20 CCU gratis: solo desarrollo y uso no comercial. 60 GB de tráfico al mes. Tope duro, sin burst. Publicar el juego en este plan no está cubierto.
- 100 CCU gratis: una aplicación Fusion o Quantum por cliente, solo juegos, uso comercial incluido. Unos 0,3 TB de tráfico al mes. Tope duro. Photon estima unos 40.000 MAU como regla orientativa, no como contrato.
- Al pasar el tope de esos planes, Photon rechaza clientes (`MaxCcuReached`).
- El tráfico cuenta mensajes de entrada y de salida hacia Photon Cloud.
- A partir de 500 CCU el servicio es de pago (500 CCU a 125 USD al mes, 1000 a 250, 2000 a 500, con burst). El excedente de tráfico se factura por GB: 0,05 USD en regiones de América y Europa listadas, 0,10 USD en el resto de las regiones publicadas.
- El SDK gratuito durante el desarrollo no vuelve ilimitado el servicio online.

## Implementación local — 5 de noviembre de 2026

Estas filas describen código que ya está en el proyecto. Los valores de balance aprobados siguen siendo configurables para pruebas; no cierran D-03, D-04, D-05, D-06, D-12, D-13, D-14 ni D-18.

| ID | Decisión | Evidencia |
|---|---|---|
| T-23 | En aire, el wish es `AirWishSpeed` y no la carrera. `MaxSpeed` es un tope de seguridad, no la velocidad de correr. `AirControl` es un slerp que solo redirige. Su valor de sensación es 0, porque un valor alto cierra el ángulo del strafe. | `MovementSimulator.TickAir`, `MovementTuning.asset` |
| T-24 | El jugador llena un buffer al pulsar salto y no lo rellena mientras la tecla sigue pulsada. `MovementSimulator` sigue tratando `Jump` como «salta en este tick». | `PlayerMovement.SampleJump` |
| T-25 | Cada hueco guarda su munición. Sacar el arma bloquea el disparo durante `DrawDuration`. El valor por defecto del tipo es 0. El retroceso del viewmodel no mueve la cámara ni el rayo. El HUD escucha eventos y no escribe daño. No se añade un rifle. El lanzacohetes no entra en Greybox. | `WeaponController`, `ShotgunView`, `CombatHud`, `Shotgun.asset` |

## Propuestas que no son decisiones

Quedan en el epic E12 del checklist: menú de pausa, bots, barriles, desmembramiento, armas futuras, asistencias y medidor de estilo.
