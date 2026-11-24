# Contexto del proyecto MEATGRINDER

Contexto actualizado el 24 de noviembre de 2026.
Fuente de diseño: [`MEATGRINDER_GDD_TDD.docx`](../MEATGRINDER_GDD_TDD.docx) (v1.0, 27 de octubre de 2026).
Adenda de alcance: [`GDD_SCOPE_ADDENDUM.md`](GDD_SCOPE_ADDENDUM.md).
Plan: [`FULL_GAME_DEVELOPMENT_PLAN.md`](FULL_GAME_DEVELOPMENT_PLAN.md).
Estado de código: [`IMPLEMENTATION_STATUS_AUDIT.md`](IMPLEMENTATION_STATUS_AUDIT.md).

## Qué estamos haciendo

La pasada del 24 de noviembre auditó el código, las escenas, los prefabs y los recursos de la base `89dedc8` junto al GDD/TDD. El estado vigente y las comprobaciones están en [IMPLEMENTATION_STATUS_AUDIT.md](IMPLEMENTATION_STATUS_AUDIT.md) y [PROGRESS.md](PROGRESS.md). Las decisiones de diseño permanecen en [DECISIONS.md](DECISIONS.md).

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

Startup carga Boot, que lleva a una partida de práctica local en M01–M04 con movimiento Quake-like, escopeta, cohete, melee, seis potenciadores, llaves y puertas, muerte y respawn. Fusion se usa en la sonda `NetworkTest`. El gameplay de red, los monstruos jugables, los modos Versus y Ranked siguen pendientes. Los mapas necesitan recorrido completo, balance y perfiles de rendimiento.

## Producto objetivo (no MVP excluyente)

El producto es el juego completo: Boot, menús, Host Mode con gameplay, arsenal (incluidas armas ampliadas), melee, power-ups, monstruos jugables, M01–M04, modos versus, ranked, audio/VFX/UI. El orden de construcción está en `FULL_GAME_DEVELOPMENT_PLAN.md` y en la regla `10-project-roadmap.mdc`. Greybox es laboratorio.
