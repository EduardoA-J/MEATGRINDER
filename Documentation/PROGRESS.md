# Progreso

Fecha: 3 de noviembre de 2026.

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

## No comprobado

- Consola visual del Editor. La evidencia es el log, no una captura de la ventana Console.
- El vector de Move y Look leído en una ventana de Play con teclado y ratón reales. En batchmode `InputAction.ReadValue` quedó en cero aunque la tecla del dispositivo de prueba estaba pulsada. F0-09 y F1-03 siguen en REVIEW.
- Armas, daño, personajes de juego, el mapa definitivo y el HUD de partida. La sonda de red solo conecta y muestra cubos estáticos. El controlador nuevo es local.
- Certificación oficial de Photon para Unity `6000.5.6f1`. La matriz de descarga sigue sin nombrar ese editor.

## Checklist

| Estado | Tareas |
|---|---|
| DONE | 18. Las 7 anteriores más F0-07, F1-01, F1-02, F1-04, F1-05, F1-06, F1-07, F1-08, F1-09, F1-10 y F5-01. |
| READY | 2. F0-04 y F0-11. |
| BLOCKED | 13. F3-01 sigue bloqueada por la escena Boot. F1-11 sigue bloqueada por D-06. |
| BACKLOG | 84. |
| IN PROGRESS | 0. |
| REVIEW | 2. F0-09 y F1-03. El asset de input está creado; falta ver el vector en la ventana de Play. |

13 epics. 119 tareas. 248 subtareas. Hay movimiento local probado. No hay combate. La única tarea de red cerrada es la sonda F3-02.

## Movimiento local

Referencia inspeccionada: `rogerli2020/SourceEngineMovementUnity`, clonada en `D:\MEATGRINDER_REFERENCE\SourceEngineMovementUnity`. El repositorio no tiene archivo de licencia (la API de GitHub responde 404). No se copió ningún archivo. El integrador es código nuevo en `MovementSimulator`.

Escena: `grinder/Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity`. Prefab: `Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab`. Parámetros: `Assets/_Project/02_ScriptableObjects/Movement/MovementTuning.asset`. Todos los números son PENDIENTE.

Prototipo inicial: GroundSpeed 8, GroundAcceleration 15, Friction 6, StopSpeed 2, AirAcceleration 40, AirControl 3, MaxSpeed 22, JumpVelocity 7,5, Gravity 22, ventana de bunny hop 0,15 s, retención 1, bloqueo de suelo tras el salto 0,1 s, StepOffset 0,4, altura 1,8, radio 0,4. `DashCooldown` existe y ningún script lo lee (D-06). `Gravity` del asset no toca la gravedad PhysX `{0, -9.81, 0}` (D-18 sigue abierta).

La simulación corre en `FixedUpdate` con `Time.fixedDeltaTime`. Mantener espacio vuelve a saltar al aterrizar dentro de la ventana. Escape suelta el cursor.

### Preparación para Fusion

`MovementSimulator.Tick` no lee dispositivos. Recibe `MovementIntent`, orientación, `MovementTuning` y el resultado de colisión. Más adelante esa llamada pasa a `FixedUpdateNetwork`. Habrá que sincronizar posición, yaw y `MovementState.Velocity`. La cámara y el readout son presentación local. No hay `NetworkTransform` ni `NetworkCharacterController` en el prefab local. La reconciliación y su tolerancia siguen en D-19.

## Siguiente paso válido

Abrir `Greybox` en el Editor, pulsar Play y confirmar con teclado y ratón el vector que batchmode no pudo leer (F0-09, F1-03). Después, la escena Boot (F0-08, F3-01). No importar la demo Shared. No commitear `PhotonAppSettings.asset`. El dash sigue bloqueado en D-06.
