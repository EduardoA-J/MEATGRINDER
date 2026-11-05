# Decisiones

Fecha: 2 de noviembre de 2026.
Una decisión «tomada» está escrita en el GDD sin contradicción interna, o está materializada en el proyecto. Una decisión «pendiente» necesita la aprobación de David antes de programarla. No se cierra nada por inferencia.

## Tomadas

| ID | Decisión | Evidencia |
|---|---|---|
| T-01 | El juego es un arena FPS asimétrico humano contra monstruo, sin extracción, puzzles ni habilidades de héroe. | GDD 1.1 y 7.2 |
| T-02 | Plataforma de la demo: Windows PC. Pipeline: URP. Lenguaje: C#. | GDD portada; `PC_RPAsset.asset` asignado en GraphicsSettings |
| T-03 | Editor declarado: Unity `6000.5.6f1`. | `grinder/ProjectSettings/ProjectVersion.txt` y respuesta de la sección 25 |
| T-04 | Red del prototipo: Photon Fusion 2. La demo acepta sesión de prueba. El dedicado de producción se evalúa después. | GDD 13.1 y sección 25 |
| T-21 | Topología: Host Mode (Client Host). El jugador host es la autoridad de estado sobre daño, disparos, salud, pickups y puntuación. El servidor dedicado queda descartado en esta fase. El presupuesto es el plan gratuito de Photon. El objetivo de red es una partida de 2 jugadores y la arquitectura preparada para 2v2. Shared Mode no se adopta sin una justificación técnica y una aprobación nueva. | Decisión de David, 3 de noviembre de 2026 |
| T-22 | Prueba controlada: Fusion SDK 2.1.4 Stable, build 2500, importado en Unity `6000.5.6f1`. Compiló en batchmode con código 0. Photon Realtime 5.1.20. Esta prueba no es una certificación oficial de Photon para ese editor. El App ID quedó después en el asset local, fuera de git. | `grinder/Logs/fusion-import.log` y `Assets/Photon/Fusion/release_history.txt`, 3 de noviembre de 2026 |
| T-05 | El resultado competitivo lo confirma la autoridad: daño, salud, muerte, pickup, puntuación, transición. | GDD 5.3, 13.2, 19 |
| T-06 | Fuego amigo desactivado por defecto y configurable por modo. | GDD 5.3 |
| T-07 | Armas de la demo: escopeta y lanzacohetes. Ametralladora, superescopeta y rayo son futuras. | GDD sección 6 |
| T-08 | Salud y armadura son ítems de demo. Invulnerabilidad, berserk y regeneración son futuros. | GDD sección 8 |
| T-09 | Los humanos empiezan con un arma básica. Las demás se recogen del mapa. | GDD 6.2 |
| T-10 | Al cambiar de mapa no se conservan las armas. Salud, armas e ítems se reinician según el modo. | GDD 10.4 y sección 25 |
| T-11 | Los monstruos pueden recoger todos los potenciadores. | GDD sección 25 |
| T-12 | En la demo, una desconexión no pausa la partida ni crea un bot. Se continúa y se marca. | GDD 11.4 |
| T-13 | Ranked no es requisito de la demo de 8 semanas. | GDD 12 y 22 |
| T-14 | Si hay retraso, se recorta contenido visual antes que las pruebas de red. | GDD sección 22 |
| T-15 | Código en inglés. ScriptableObjects para configuración. Estado de partida en la autoridad. | GDD apéndice A y 14.3 |
| T-16 | Input System nuevo, no el Input Manager clásico. | `activeInputHandler: 1` y paquete 1.20.0 |
| T-17 | M01 The Slaughterhouse es el mapa de demo. M03 y M04 son posteriores. | GDD 10.2 |
| T-18 | El daño no se duplica por una misma confirmación. La muerte se registra una vez. Las asistencias quedan para después. | GDD 5.3 |
| T-19 | Los efectos cosméticos no deciden daño ni resultado. | GDD 9.2 |
| T-20 | Assets originales, con licencia registrada o dominio público verificado. | GDD 2 |

## Pendientes — necesitan aprobación

| ID | Pregunta | Por qué no se puede cerrar sola |
|---|---|---|
| D-03 | Política de lag compensation para hitscan. | El GDD la condiciona a que la arquitectura la soporte. |
| D-04 | Modo de la demo: Versus Campaign (cambio de roles) o Versus Continuous. | Sección 3.2 marca los dos como alta prioridad. Sección 25 escribe «se intercambian roles». Sección 22 no entrega el intercambio en una semana concreta. |
| D-05 | Tamaño exigido: 2v2 de prototipo o 4v4 obligatorio. | 3.3 y 22 dicen 2v2 primero y 4v4 condicionado. La sección 25 escribe «4v4». |
| D-06 | ¿El dash entra en la demo? | 4.1 lo deja opcional, con cooldown, y no es un poder. No hay respuesta en la sección 25. |
| D-07 | Fórmula de armadura: porcentaje, tope, reducción por golpe. | 8 dice «absorbe parte del daño» y no da números. |
| D-08 | Segundos de respawn y de protección anti spawn-kill. | 3.3 dice temporizador corto y protección mínima. Sin cifras. |
| D-09 | uGUI o UI Toolkit para el HUD y los menús. | No está en el GDD. Propuesta de trabajo, no decisión de diseño: uGUI, porque `com.unity.ugui` 2.5.0 ya está. Hace falta un sí. |
| D-10 | Qué hace Quad Damage y cuánto dura. | Semana 5 y checklist lo incluyen. La tabla de la sección 8 no lo define. «Tres potenciadores» (1.4) no nombra los tres. |
| D-11 | Qué ítems usan temporizador fijo y qué pasa si se recoge el mismo efecto otra vez. | Sección 25: «sí, algunos». Sección 8.1 pide definir reinicio, extensión o ignorar. |
| D-12 | Estadísticas del demonio ligero y del demonio pesado. | El checklist pide dos modelos. La sección 7.2 solo describe un Demonio. |
| D-13 | ¿Los monstruos recogen armas de fuego o solo potenciadores? | 6.2 dice que también pueden agarrar armas. La sección 25 solo afirma los potenciadores. |
| D-14 | Victoria: progreso del recorrido, bajas, o una combinación. | 11.2 da puntos por bajas y por recorrido. La sección 25 dice «progreso recorrido». |
| D-15 | PC de referencia para el objetivo de 60 FPS. | 20.1 lo pide «acordado» y no está descrito. |
| D-16 | Fecha de inicio de las 8 semanas. | El plan no tiene calendario. Hoy el código de juego no existe. |
| D-17 | Crouch y Sprint del asset de input de plantilla. | Existen en `InputSystem_Actions.inputactions`. El GDD no los pide. |
| D-18 | Gravedad de diseño frente a PhysX `-9.81`. | El movimiento lleva su propio parámetro `Gravity` en ScriptableObject. |
| D-19 | Tolerancia visual de la reconciliación de movimiento. | 4.3 pide coincidencia «dentro de la tolerancia definida» y no la define. |
| D-20 | Ragdoll simple o solo animación de muerte en la demo. | 9.1 acepta cualquiera de los dos. |
| D-21 | Renombrar producto `grinder` / compañía `DefaultCompany` a MEATGRINDER. | Hecho de proyecto, ausente del GDD. |
| D-22 | Quitar del flujo de trabajo, sin desinstalar todavía, los paquetes de plantilla que el GDD no usa. | Navigation, Visual Scripting, Timeline y Collab Proxy están instalados. Esta fase no los toca. |

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

Estas filas describen código que ya está en el proyecto. Los números siguen en `PENDIENTE`. No cierran D-06, D-07, D-09 ni D-18.

| ID | Decisión | Evidencia |
|---|---|---|
| T-23 | En aire, el wish es `AirWishSpeed` y no la carrera. `MaxSpeed` es un tope de seguridad, no la velocidad de correr. `AirControl` es un slerp que solo redirige. Su valor de sensación es 0, porque un valor alto cierra el ángulo del strafe. | `MovementSimulator.TickAir`, `MovementTuning.asset` |
| T-24 | El jugador llena un buffer al pulsar salto y no lo rellena mientras la tecla sigue pulsada. `MovementSimulator` sigue tratando `Jump` como «salta en este tick». | `PlayerMovement.SampleJump` |
| T-25 | Cada hueco guarda su munición. Sacar el arma bloquea el disparo durante `DrawDuration`. El valor por defecto del tipo es 0. El retroceso del viewmodel no mueve la cámara ni el rayo. El HUD escucha eventos y no escribe daño. No se añade un rifle. El lanzacohetes no entra en Greybox. | `WeaponController`, `ShotgunView`, `CombatHud`, `Shotgun.asset` |

## Propuestas que no son decisiones

Quedan en el epic E12 del checklist: menú de pausa, bots, barriles, desmembramiento, armas futuras, asistencias y medidor de estilo.
