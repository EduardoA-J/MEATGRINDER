# Contexto del proyecto MEATGRINDER

Fecha de este contexto: 2 de noviembre de 2026.
Fuente de diseño: [`MEATGRINDER_GDD_TDD.docx`](../MEATGRINDER_GDD_TDD.docx) (v1.0, 27 de octubre de 2026).
El documento declara que el diseño está propuesto y que nada está implementado hasta superar sus pruebas.

## Qué estamos haciendo

MEATGRINDER es un arena FPS multijugador para Windows. Un equipo de humanos armados juega contra un equipo de monstruos controlados por personas. Los mapas de combate se encadenan con puertas. El conocimiento del escenario, la puntería y el temporizador de los ítems deciden la partida.

La fantasía de juego es desplazarse rápido, mantener el control en el aire, pegar tiros legibles y elegir entre alcance (humano) o cerrar distancia (monstruo).

## Dónde vive cada capa de contexto

Cursor reconoce de forma nativa, en esta versión, dos mecanismos usados aquí:

- `AGENTS.md` en la raíz, para instrucciones compartidas con el agente.
- `.cursor/rules/*.mdc`, con frontmatter `description`, `globs` y `alwaysApply`.

No hay un archivo `.context`. El detalle largo está en `Documentation/` para no duplicar el GDD dentro de cada regla.

| Regla | Siempre activa | Contenido |
|---|---|---|
| `00-project-overview.mdc` | Sí | Juego, alcance, hechos del repo |
| `01-game-design.mdc` | No | Pilares, modos, mapas, ítems |
| `02-technical-architecture.mdc` | No | Carpetas, componentes, datos |
| `03-gameplay-and-combat.mdc` | No | Movimiento, armas, daño |
| `04-multiplayer-photon.mdc` | No | Fusion, autoridad, pruebas |
| `05-art-animation-and-audio.mdc` | No | Arte, gore, clips, audio |
| `06-ui-ux.mdc` | No | Menús, HUD, accesibilidad |
| `07-code-standards.mdc` | No | C# y ScriptableObjects |
| `08-testing-and-qa.mdc` | No | Definition of Done |
| `09-workflow-and-agents.mdc` | Sí | Flujo y límites de herramientas |
| `10-project-roadmap.mdc` | Sí | MVP, demo de 8 semanas, después |

## Lectura recomendada

1. [TECHNICAL_AUDIT.md](TECHNICAL_AUDIT.md) — entorno, skills, plugins, huecos.
2. [GDD_REQUIREMENTS.md](GDD_REQUIREMENTS.md) — requisitos y contradicciones.
3. [DECISIONS.md](DECISIONS.md) — cerrado frente a pendiente.
4. [MASTER_CHECKLIST.md](MASTER_CHECKLIST.md) — tareas ejecutables.
5. [DEPENDENCY_GRAPH.md](DEPENDENCY_GRAPH.md) — qué bloquea a qué.
6. [AGENT_WORKFLOW.md](AGENT_WORKFLOW.md) — perfiles y flujo.
7. [RISKS.md](RISKS.md) — riesgos de producción.
8. [PROGRESS.md](PROGRESS.md) — avance con evidencia.

## Estado en una frase

El repositorio tiene la plantilla URP de Unity 6.5.6, el GDD y Photon Fusion 2.1.4 Stable, build 2500, en `grinder/Assets/Photon`. El 3 de noviembre de 2026 esa importación compiló en `6000.5.6f1`. El juego todavía no tiene sistemas. El App ID está en el asset local, fuera de git. La topología cerrada es Host Mode. La compilación no es una certificación oficial de Photon para este editor.

## MVP

El MVP demuestra movimiento rápido, una arena gris, cámara, input, escopeta, lanzacohetes, daño, salud, dos jugadores en Fusion, muerte, respawn, un HUD mínimo y una pasada de estabilidad. El resto de la demo de ocho semanas (monstruos, M01, puertas, armadura, Quad Damage, marcador, gore, build) viene después. Ranked queda fuera. Detalle en la regla `10-project-roadmap.mdc`.
