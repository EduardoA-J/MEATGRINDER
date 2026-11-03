# MEATGRINDER — instrucciones para agentes

Fuente de diseño: `MEATGRINDER_GDD_TDD.docx` (raíz del repositorio, versión 1.0, 27 de octubre de 2026).
Contexto operativo: `Documentation/PROJECT_CONTEXT.md`.
Reglas de Cursor: `.cursor/rules/`.

## Qué es el juego

MEATGRINDER es un arena FPS para Windows PC. Humanos armados se enfrentan a monstruos controlados por jugadores. El movimiento sigue la lógica de Quake (aceleración, control aéreo, strafe jumping, bunny hopping). El arsenal y los mapas siguen a DOOM clásico (armas contundentes, pasillos, puertas de transición, sin puzzles). El feedback de combate se inspira en ULTRAKILL. El formato de dos equipos, monstruos jugables e intercambio de roles se inspira en Left 4 Dead Versus.

No hay extracción, puzzles obligatorios, narrativa compleja, habilidades activas de héroe, árboles de talentos ni pay-to-win.

## Stack que hay que respetar

- Unity `6000.5.6f1` (6.5.6), ya declarada en `grinder/ProjectSettings/ProjectVersion.txt`.
- URP 17.5.0. El asset de pipeline activo es `grinder/Assets/Settings/PC_RPAsset.asset`.
- C#, Input System 1.20.0 (`activeInputHandler: 1`).
- Photon Fusion 2 es la red elegida. La topología cerrada es Host Mode: el jugador host tiene la autoridad. Shared Mode queda fuera hasta una justificación técnica y una aprobación nueva. El SDK 2.1.4 Stable, build 2500, está en `grinder/Assets/Photon` y compiló en `6000.5.6f1` (T-22). Esa compilación no es una certificación oficial de Photon. El App ID está en el asset local y ese archivo está en `.gitignore`. No sustituirlo por Netcode for GameObjects, Unity Multiplayer Services ni otro stack.
- Plataforma de la demo: Windows PC. Objetivo de rendimiento del GDD: 60 FPS en un PC de referencia todavía no definido.

## Estado real

El proyecto de juego está en `grinder/`. Contiene la plantilla URP (`SampleScene`, el asset de input por defecto y los scripts de Readme) y Photon Fusion 2.1.4 en `Assets/Photon`. No hay controlador, armas, monstruos ni mapas de juego. El 3 de noviembre de 2026 Unity `6000.5.6f1` compiló la plantilla y, después, la importación de Fusion, ambas sin `error CS`. La sesión de red sigue sin verificar: el App ID ya está en el asset local y todavía no hay dos clientes conectados.

## Orden de lectura

1. Esta página.
2. La regla `.cursor/rules/` del dominio que vas a tocar.
3. La tarea concreta en `Documentation/MASTER_CHECKLIST.md`.
4. `Documentation/DECISIONS.md` si la tarea depende de una decisión.
5. El apartado del GDD citado por la tarea, si la decisión afecta al diseño.

## Conducta

- Implementa solo el alcance de la tarea aprobada.
- Si el código contradice el GDD, detén el cambio y registra la diferencia. El GDD manda en diseño.
- Si un valor numérico no está en el GDD, déjalo en un ScriptableObject con un comentario `PENDIENTE` y no lo presentes como definitivo.
- El estado competitivo (daño confirmado, salud, muerte, recogida, puntuación, transición de mapa) lo resuelve la autoridad de la sesión. El cliente puede predecir presentación, no el resultado.
- Nombres de código en inglés. PascalCase en tipos y métodos públicos. camelCase en campos privados. Los ScriptableObjects guardan configuración, no el estado mutable de la partida.
- Separa lógica de juego, presentación y red. Una sola resolución de daño (`DamageResolver`). La UI no escribe estado autoritativo.
- No añadas paquetes, no cambies la versión de Unity y no actualices Fusion sin una tarea que lo pida y sin aprobación.
- No marques una tarea `DONE` sin una prueba repetible y, si afecta a multijugador, sin dos clientes.
- Actualiza `Documentation/PROGRESS.md` y, si la decisión queda cerrada por David, `Documentation/DECISIONS.md`.

## Perfiles

Los perfiles de `Documentation/AGENT_WORKFLOW.md` son roles de trabajo. Los únicos agentes realmente presentes son el agente de Cursor en este IDE y, en el perfil de la máquina, skills de Codex que no están conectadas a este repositorio. `game-team-orchestrator` es una referencia de coordinación; no tiene instalados los especialistas que intenta invocar.
