# Progreso

Fecha: 5 de noviembre de 2026.

## Hecho y comprobado

| Evidencia | Qué demuestra |
|---|---|
| `F:\Unity\Hub\Editor\6000.5.6f1\Editor\Unity.exe` | El editor declarado está instalado. Abrió el proyecto (PID 14408) y después se cerró para la importación. |
| `grinder/Logs/Editor.log` | `Initialize engine version: 6000.5.6f1 (0e0577a1a2ac)`. `ProjectVersion.txt` sigue en esa versión. |
| Misma log, compilación | `Tundra build success (51.84 seconds)`. Búsqueda de `error CS`: ninguna coincidencia. |
| Misma log, import | `Asset Pipeline Refresh` inicial terminó en 311.844 segundos. |
| `grinder/Logs/upm.log` | `project:resolve-packages` respondió 200. Tres warnings de versión mínima: searcher 4.9.3→4.9.4, burst 1.8.14→1.8.29, performance testing 3.2.0→3.5.0. |
| `EditorSettings.asset` | Serialización Force Text, que es un requisito de Fusion. |
| Importación de Fusion, 3 de noviembre de 2026 | `photon-fusion-2.1.4-stable-2500.unitypackage` importado. `fusion-import.log`: cuatro `Tundra build success`, la última de 20.60 segundos, salida con código 0. Realtime 5.1.20. Detalle en `DECISIONS.md`, T-22. |
| Sonda Host/Client, 3 de noviembre de 2026 | Editor batchmode como Host y `MeatgrinderProbe.exe` como Client en la sesión `MeatgrinderProbe-20261103`. `network-test-host.txt` y `network-test-client.txt`: `START_OK`, dos avatares, despawn al salir el Client. F3-02. |
| Movimiento local, 3 de noviembre de 2026 | EditMode 17/17 y PlayMode 5/5 con Unity `6000.5.6f1`. Resultados en `grinder/Logs/movement-editmode.xml` y `movement-playmode.xml`. F0-07, F1-01, F1-02, F1-04 a F1-10 y F5-01. |
| Combate local, 4 de noviembre de 2026 | Unity `6000.5.6f1`. EditMode 31/31 (combate 14, movimiento 17) y PlayMode 7/7 (combate 2, movimiento 5). XML en `Documentation/test-results/combat-editmode.xml` y `combat-playmode.xml`. F2-01, F2-02, F2-06, F2-09, F2-11 y F2-12. |

## No comprobado

- Consola visual del Editor. La evidencia es el log, no una captura de la ventana Console.
- El vector de Move y Look leído en una ventana de Play con teclado y ratón reales. En batchmode `InputAction.ReadValue` quedó en cero aunque la tecla del dispositivo de prueba estaba pulsada. F0-09 y F1-03 siguen en REVIEW.
- La pasada manual de Greybox: abrir la escena, pulsar Play, ver la escopeta y la retícula, disparar y ver morir un blanco. Las pruebas automáticas pasaron. F2-07, F7-04 y F7-05 siguen sin cerrarse por esa pasada. F7-04 además espera armadura y red.
- La pasada manual de la pista oeste: recta contra strafe, saltos pulsando espacio, rampa y plataformas. El 5 de noviembre solo corrieron las pruebas automáticas.
- Personajes de juego, el mapa definitivo y el HUD de partida. La sonda de red solo conecta y muestra cubos estáticos. El combate nuevo es local.
- Certificación oficial de Photon para Unity `6000.5.6f1`. La matriz de descarga sigue sin nombrar ese editor.

## Checklist

| Estado | Tareas |
|---|---|
| DONE | 24. Las 18 anteriores más F2-01, F2-02, F2-06, F2-09, F2-11 y F2-12. |
| READY | 2. F0-04 y F0-11. |
| BLOCKED | 13. F3-01 sigue bloqueada por la escena Boot. F1-11 sigue bloqueada por D-06. F2-03 sigue bloqueada por D-07. |
| BACKLOG | 77. Incluye F2-04 (falta el respawn), F2-05, F2-08, F2-10 y el combate en red. |
| IN PROGRESS | 0. |
| REVIEW | 3. F0-09, F1-03 y F2-07. La escopeta está en Greybox; falta pulsar Play en el Editor. |

13 epics. 119 tareas. 248 subtareas. Hay movimiento local y combate local de escopeta probados en automático. La única tarea de red cerrada es la sonda F3-02.

## Movimiento local

Referencia inspeccionada: `rogerli2020/SourceEngineMovementUnity`, clonada en `D:\MEATGRINDER_REFERENCE\SourceEngineMovementUnity`. El repositorio no tiene archivo de licencia (la API de GitHub responde 404). No se copió ningún archivo. El integrador es código nuevo en `MovementSimulator`.

Escena: `grinder/Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity`. Prefab: `Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab`. Parámetros: `Assets/_Project/02_ScriptableObjects/Movement/MovementTuning.asset`. Todos los números son PENDIENTE.

Prototipo inicial: GroundSpeed 8, GroundAcceleration 15, Friction 6, StopSpeed 2, AirAcceleration 40, AirControl 3, MaxSpeed 22, JumpVelocity 7,5, Gravity 22, ventana de bunny hop 0,15 s, retención 1, bloqueo de suelo tras el salto 0,1 s, StepOffset 0,4, altura 1,8, radio 0,4. `DashCooldown` existe y ningún script lo lee (D-06). `Gravity` del asset no toca la gravedad PhysX `{0, -9.81, 0}` (D-18 sigue abierta).

La simulación corre en `FixedUpdate` con `Time.fixedDeltaTime`. Esos números y el salto automático al mantener espacio son el prototipo del 3 de noviembre. La sección del 5 de noviembre los sustituye. Escape suelta el cursor.

### Preparación para Fusion

`MovementSimulator.Tick` no lee dispositivos. Recibe `MovementIntent`, orientación, `MovementTuning` y el resultado de colisión. Más adelante esa llamada pasa a `FixedUpdateNetwork`. Habrá que sincronizar posición, yaw y `MovementState.Velocity`. La cámara y el readout son presentación local. No hay `NetworkTransform` ni `NetworkCharacterController` en el prefab local. La reconciliación y su tolerancia siguen en D-19.

## Escopeta y salud, local

`LocalPlayer` lleva `WeaponController` con un solo hueco, `Shotgun.asset`, `PlayerCombat`, `HealthComponent`, `DamageResolverHost`, `LocalCombatAuthority` y `CombatReadout`. `PlayerMovement` y `MovementSimulator` no se tocaron. No hay `NetworkObject` ni `NetworkRunner` en este prefab.

Números de prototipo, marcados `PENDIENTE` en `WeaponDefinition` y `HealthProfile`. No son cifras del GDD. Escopeta: 8 perdigones, 12 de daño, dispersión 5°, alcance 36, caída de 6 a 28 hasta el 30 %, cadencia 0,7 s, cargador 6, reserva 24, recarga 1,4 s, retroceso 1,5°. Salud: humano 100, frágil 40, estándar 80, duro 200.

Greybox, bajo `CombatRange`: `Shotgun_Close` y `Shotgun_Far` (40), `Shotgun_Blocked` (80) detrás de `Wall_ShotgunBlock`, y `Target_Tough` (200). El jugador aparece en `(0, 0.05, -18)`. Los blancos están hacia +X, alrededor de x = 18.

El clic izquierdo dispara, R recarga y las teclas 1 y 2 piden cambio de hueco. Con una sola arma, el hueco 2 no cambia nada. El lanzacohetes no está en la escena. Su código y `RocketMovesAndSparesTheCoveredTarget` siguen en el repo; F2-08 sigue en BACKLOG.

`DamageResolver` sigue rechazando la misma confirmación, el disparo sin autoridad, al propio atacante y el fuego amigo. `Died` se emite una vez. No hay respawn (F2-04) ni armadura (D-07).

### Preparación para Fusion

El disparo local sigue en `PlayerCombat.FixedUpdate`. El comentario del componente dice qué tendrá que viajar al host: origen, dirección, id de arma y tick. `DamageResolver.Apply` no lo llama ningún `NetworkRunner`. Los proyectiles locales no se replican.

## Presentación local, 4 de noviembre de 2026

El viewmodel es un hijo de la cámara, hecho con cubos propios. No tiene collider y no mueve el origen del rayo. La retícula y el HUD son uGUI y leen `HealthComponent` y `WeaponController`. Los blancos arman su silueta al pulsar Play. Greybox tiene suelo, muros y cinco letreros de zona. Nada de esto salió de los clones de `F:\MAPAUNI\references`: no tienen licencia. Detalle en `Documentation/REFERENCE_REPOSITORIES_AUDIT.md`.

EditMode 32/32 y PlayMode 7/7. XML en `Documentation/test-results/presentation-editmode.xml` y `presentation-playmode.xml`. En esa pasada `MovementSimulator` no cambió.

## Sensación de movimiento y armas, 5 de noviembre de 2026

El análisis está en `Documentation/MOVEMENT_WEAPON_REFERENCE_ANALYSIS.md`. No se copió código de `F:\MAPAUNI\references`.

El wish de aire ya no es la carrera: `AirWishSpeed` 2,8. `MaxSpeed` 32 es tope de seguridad. `GroundAcceleration` 10. `AirAcceleration` 70. `AirControl` 0, porque el slerp a 3 cerraba el ángulo y un strafe de prueba solo ganaba 0,46 m/s. `BunnyHopWindow` 0,12 s. Campo de visión 100. Todos `PENDIENTE`.

`PlayerMovement` llena un buffer al pulsar espacio y no lo rellena mientras la tecla sigue pulsada. El simulador no cambió el significado de `Jump`.

La escopeta gasta lo mismo que antes (12 por perdigón, 8 perdigones, dispersión 5°, cadencia 0,7 s, cargador 6, reserva 24, recarga 1,4 s). Nuevo: `DrawDuration` 0,18 s. Durante ese tiempo no dispara ni recarga. El viewmodel baja en local. El rayo no se mueve. No hay rifle ni lanzacohetes en la escena.

Greybox tiene `MovementCourse` al oeste de la galería: recta, saltos, strafe, rampa, escalones y dos plataformas. Los blancos siguen en su sitio. El readout de la esquina es de depuración.

Pruebas dentro del Editor abierto, Unity `6000.5.6f1`: EditMode 36/36 y PlayMode 7/7. XML en `Documentation/test-results/feel-editmode.xml` y `feel-playmode.xml`. La repetición en batchmode, después de quitar el registro de depuración, dio el mismo resultado: `review-editmode.xml` 36/36 y `review-playmode.xml` 7/7, ambas con código de salida 0. La pasada con ratón en la ventana de Play no se hizo. F1 sigue DONE por las pruebas automáticas anteriores y estas. F2-07 sigue en REVIEW.

## Siguiente paso válido

Abrir `Greybox`, pulsar Play y, en la pista del oeste, comparar una recta con un strafe y encadenar saltos pulsando espacio. En la misma pasada, matar `Shotgun_Close` y comprobar que la tecla 2 no cambia de arma (F2-07, F0-09, F1-03). Después, la escena Boot (F0-08, F3-01). No importar la demo Shared. No commitear `PhotonAppSettings.asset`. El dash sigue bloqueado en D-06. El lanzacohetes espera su propia tarea.
