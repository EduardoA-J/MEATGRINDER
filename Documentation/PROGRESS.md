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

## No comprobado

- Consola visual del Editor. La evidencia es el log, no una captura de la ventana Console.
- Movimiento, armas, daño, personajes de juego, mapas y el HUD de partida. La sonda solo conecta y muestra cubos estáticos.
- Certificación oficial de Photon para Unity `6000.5.6f1`. La matriz de descarga sigue sin nombrar ese editor.

## Checklist

| Estado | Tareas |
|---|---|
| DONE | 7. F0-01, F0-02, F0-03, F0-05, F0-06, F0-12 y F3-02. |
| READY | 2. F0-04 y F0-11. |
| BLOCKED | 13. F3-01 sigue bloqueada por la escena Boot. F3-03 sigue bloqueada por la arena gris. |
| BACKLOG | 97. |
| IN PROGRESS | 0. |
| REVIEW | 0. |

13 epics. 119 tareas. 248 subtareas. En `DONE` no hay movimiento, combate ni mapas. La única tarea de red cerrada es la sonda F3-02.

## Siguiente paso válido

La sonda de red ya conectó. Sigue la escena Boot (F0-08, F3-01) y el movimiento local (F0-07, F1). F3-03 espera la arena gris. No importar la demo Shared. No commitear `PhotonAppSettings.asset`.
