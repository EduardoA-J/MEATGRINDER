# Contexto del proyecto MEATGRINDER

Fecha de este contexto: 9 de noviembre de 2026.
Fuente de diseño: [`MEATGRINDER_GDD_TDD.docx`](../MEATGRINDER_GDD_TDD.docx) (v1.0, 27 de octubre de 2026).
Adenda de alcance: [`GDD_SCOPE_ADDENDUM.md`](GDD_SCOPE_ADDENDUM.md).
Plan: [`FULL_GAME_DEVELOPMENT_PLAN.md`](FULL_GAME_DEVELOPMENT_PLAN.md).
Estado de código: [`IMPLEMENTATION_STATUS_AUDIT.md`](IMPLEMENTATION_STATUS_AUDIT.md).

## Qué estamos haciendo

MEATGRINDER es un arena FPS multijugador **completo** para Windows. Un equipo de humanos armados juega contra un equipo de monstruos controlados por personas. Los mapas de combate se encadenan con puertas. El conocimiento del escenario, la puntería y el temporizador de los ítems deciden la partida.

La fantasía de juego es desplazarse rápido, mantener el control en el aire, pegar tiros legibles y elegir entre alcance (humano) o cerrar distancia (monstruo).

No se desarrolla una demo descartable que luego haya que convertir en juego: se implementan sistemas definitivos, integrados y usables en partidas reales. Las etiquetas «demo / MVP / futuro» del GDD son **prioridad**, no exclusión.

## Dónde vive cada capa de contexto

| Regla | Siempre activa | Contenido |
|---|---|---|
| `00-project-overview.mdc` | Sí | Juego, alcance completo, hechos del repo |
| `01-game-design.mdc` | No | Pilares, modos, mapas, ítems |
| `02-technical-architecture.mdc` | No | Carpetas, componentes, datos |
| `03-gameplay-and-combat.mdc` | No | Movimiento, armas, melee, daño |
| `04-multiplayer-photon.mdc` | No | Fusion, autoridad, pruebas |
| `05-art-animation-and-audio.mdc` | No | Arte, gore, clips, audio |
| `06-ui-ux.mdc` | No | Menús, HUD, accesibilidad |
| `07-code-standards.mdc` | No | C# y ScriptableObjects |
| `08-testing-and-qa.mdc` | No | Definition of Done |
| `09-workflow-and-agents.mdc` | Sí | Flujo y límites de herramientas |
| `10-project-roadmap.mdc` | Sí | Fases P0–P4 del juego completo |

## Lectura recomendada

1. [GDD_SCOPE_ADDENDUM.md](GDD_SCOPE_ADDENDUM.md) — política de alcance.
2. [IMPLEMENTATION_STATUS_AUDIT.md](IMPLEMENTATION_STATUS_AUDIT.md) — qué hay en código.
3. [FULL_GAME_DEVELOPMENT_PLAN.md](FULL_GAME_DEVELOPMENT_PLAN.md) — plan inmediato.
4. [GDD_REQUIREMENTS.md](GDD_REQUIREMENTS.md) — requisitos y contradicciones.
5. [DECISIONS.md](DECISIONS.md) — cerrado frente a pendiente.
6. [MASTER_CHECKLIST.md](MASTER_CHECKLIST.md) — tareas ejecutables.
7. [DEPENDENCY_GRAPH.md](DEPENDENCY_GRAPH.md) — qué bloquea a qué.
8. [PROGRESS.md](PROGRESS.md) — avance con evidencia.
9. [RISKS.md](RISKS.md) — riesgos.
10. [TOOLS_AND_REFERENCES.md](TOOLS_AND_REFERENCES.md) — ProBuilder 6.1.4 instalado (T-26). Cuatro escenas de mapa en blockout (T-27). Ninguna está VALIDATED. Luna UI no está comprada.

## Estado en una frase

HEAD `992bb92`: movimiento local Quake-like, escopeta local, daño/salud, armadura parcial, scaffolding de pickups/power-ups y lanzacohetes, HUD local, sonda Fusion de cubos. Sin monstruos, sin Boot de producto, sin ranked. Hay cambios locales sin commit (combate, pickups y las cuatro escenas de mapa): no descartarlos. M01–M04 existen como blockout ProBuilder y no están validados.

## Producto objetivo (no MVP excluyente)

El producto es el juego completo: Boot, menús, Host Mode con gameplay, arsenal (incluidas armas ampliadas), melee, power-ups, monstruos jugables, M01–M04, modos versus, ranked, audio/VFX/UI. El orden de construcción está en `FULL_GAME_DEVELOPMENT_PLAN.md` y en la regla `10-project-roadmap.mdc`. Greybox es laboratorio.
