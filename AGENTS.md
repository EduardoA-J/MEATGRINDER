# MEATGRINDER — instrucciones para agentes

Fuente de diseño: `MEATGRINDER_GDD_TDD.docx` (raíz del repositorio, versión 1.0, 27 de octubre de 2026).
Adenda de alcance: `Documentation/GDD_SCOPE_ADDENDUM.md` (9 de noviembre de 2026).
Contexto operativo: `Documentation/PROJECT_CONTEXT.md`.
Plan: `Documentation/FULL_GAME_DEVELOPMENT_PLAN.md`.
Estado real: `Documentation/IMPLEMENTATION_STATUS_AUDIT.md`.
Reglas de Cursor: `.cursor/rules/`.

## Qué es el juego

MEATGRINDER es un arena FPS multijugador completo para Windows PC. Humanos armados se enfrentan a monstruos controlados por jugadores. El movimiento sigue la lógica de Quake. El arsenal y los mapas siguen a DOOM clásico. El feedback de combate se inspira en ULTRAKILL. El formato Versus (equipos, monstruos jugables, intercambio de roles) se inspira en Left 4 Dead Versus. Ranked, arsenal ampliado, power-ups, melee, mapas M01–M04 y el resto del GDD forman parte del producto.

No hay extracción, puzzles obligatorios, narrativa compleja, habilidades activas de héroe, árboles de talentos ni pay-to-win.

## Política de alcance (obligatoria)

1. Todas las funcionalidades documentadas son `REQUIRED`. Prioridad ≠ exclusión.
2. No desarrollar demos descartables ni sistemas que haya que reescribir para la partida real.
3. Greybox es herramienta de prueba, no el producto. No gastes el ciclo en dioramas por mecánica.
4. Implementa arquitecturas definitivas, integradas al flujo de partida (Boot → sesión → combate → resultados).
5. Host Mode y autoridad de red se consideran desde el diseño de sistemas competitivos.
6. Separar lógica, presentación y autoridad. Una sola resolución de daño (`DamageResolver`). La UI no escribe estado autoritativo.
7. No afirmes terminado lo que solo es una clase, un ScriptableObject o un test aislado. Hace falta integración y prueba.
8. No inventes integración Fusion. No marques multijugador DONE sin dos clientes reales en el flujo de juego.
9. Si un número no está en el GDD, ScriptableObject + `PENDIENTE`; no lo presentes como definitivo.
10. Tras cada implementación: actualizar `MASTER_CHECKLIST.md` y `PROGRESS.md`. Si David cierra una decisión, `DECISIONS.md`.

## Stack

- Unity `6000.5.6f1`, URP 17.5.0, Input System 1.20.0.
- Photon Fusion 2.1.4 Stable, Host Mode. App ID solo en asset local gitignored.
- No sustituir el stack de red. No cambiar Unity/URP/Fusion sin tarea y aprobación.

## Estado real (9 de noviembre de 2026)

HEAD tipico: movimiento local Quake-like, escopeta local, daño/salud/armadura parcial, HUD local, sonda Fusion de cubos. Lanzacohetes y pickups/power-ups en scaffolding. Sin melee jugable, sin monstruos, sin Boot de producto, sin M01, sin ranked. Detalle: `IMPLEMENTATION_STATUS_AUDIT.md`.

Pueden existir cambios locales sin commit (p. ej. rotura de ciclo Combat↔Pickups). No los descartes.

## Orden de lectura

1. Esta página y `GDD_SCOPE_ADDENDUM.md`.
2. Regla `.cursor/rules/` del dominio.
3. Tarea en `MASTER_CHECKLIST.md`.
4. `FULL_GAME_DEVELOPMENT_PLAN.md` para prioridad.
5. `DECISIONS.md` si hay decisión abierta.
6. Apartado del GDD citado.

## Conducta

- Implementa el alcance de la tarea; el producto es el juego completo.
- Si el código contradice el GDD, detén y documenta.
- Nombres en inglés. PascalCase público, camelCase privado.
- No añadas paquetes ni actualices Fusion sin tarea.
- Estados de checklist: ver `MASTER_CHECKLIST.md` (ya no se usa DONE/BACKLOG como exclusión de alcance).

## Perfiles

`Documentation/AGENT_WORKFLOW.md` describe roles. El agente activo es Cursor en este IDE.
