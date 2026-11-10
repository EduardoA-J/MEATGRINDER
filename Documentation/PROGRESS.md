# Progreso

Fecha: 9 de noviembre de 2026.
Alcance: juego completo (`GDD_SCOPE_ADDENDUM.md`). Estados: `MASTER_CHECKLIST.md`.

## Porcentaje real

Estimación sobre el checklist redefinido (~128 tareas REQUIRED): **~16–18 %** con estado ≥ `INTEGRATED` / `VALIDATED`.
Método y detalle: `IMPLEMENTATION_STATUS_AUDIT.md`. No confundir con «% de la demo antigua».

## Hecho y comprobado (evidencia histórica)

| Evidencia | Qué demuestra |
|---|---|
| Unity `6000.5.6f1` | Editor declarado abre `grinder/` |
| Fusion import 3 nov 2026 | SDK 2.1.4 Stable build 2500 compiló (T-22) |
| Sonda Host/Client 3 nov | F3-02 VALIDATED como sonda de cubos, no gameplay |
| Movimiento local 3–5 nov | EditMode/PlayMode movimiento; F1-* VALIDATED (salvo F1-03 / F1-11) |
| Combate local 4–5 nov | Escopeta + daño + HUD; F2-01/02/06/09/11/12 VALIDATED; F2-07 INTEGRATED |
| Arsenal/pickups commit `992bb92` | Scaffolding: Rocket asset/código, Pickups, armadura hooks — **no** INTEGRATED completo |

## Estado por eje (9 nov 2026)

| Eje | Resumen |
|---|---|
| Movimiento local | VALIDATED (tests). Play manual vivo pendiente (F1-03). |
| Escopeta | INTEGRATED local. VALIDATED manual pendiente. |
| Lanzacohetes | PARTIALLY_IMPLEMENTED. No en loadout; spawner null. |
| Melee | NOT_STARTED (solo enum). |
| Salud / muerte | INTEGRATED local. Respawn NOT_STARTED. |
| Armadura | PARTIALLY_IMPLEMENTED. D-07 abierta. |
| Pickups / power-ups | PARTIALLY_IMPLEMENTED. Sin wiring efectivo a consumidores. |
| HUD | INTEGRATED local (salud/munición/retícula). Power-ups UI parcial. |
| Fusion gameplay | NOT_STARTED. Solo sonda. |
| Boot / menús / monstruos / ranked | NOT_STARTED |
| M01–M04 | PARTIALLY_IMPLEMENTED. M02 regenerado (~540 × 500 m, torres, ruinas y plataformas) el 10 nov 2026. Humo de suelo de M02 pasado. Escaleras, ítems y Host Mode sin validar. T-27 |
| Lanzacohetes, melee, pickups, respawn local | INTEGRATED en código. Cifras D-07/D-08/D-10/D-11 abiertas. Sin red |

## Checklist (recuento aproximado)

| Estado | ~Tareas |
|---|---|
| VALIDATED | ~22 |
| INTEGRATED | ~3 |
| PARTIALLY_IMPLEMENTED | ~15 |
| BLOCKED | ~12 |
| NOT_STARTED | resto (~76+) |

## Cambios locales sin commit (no descartar)

- `IDamageModifiers` + `DamageResolver` + `PowerUpManager` + asmdef Combat (rotura de ciclo).
- ProBuilder 6.1.4 (T-26). Cuatro escenas de mapa ampliadas o creadas el 9 de noviembre. La rampa de M01 que David recorrió a mano se conservó. T-27 registra los nombres de escena frente a los del GDD.
- Gameplay local: cohete en el loadout, pickups, quad, haste, melee, berserk, respawn y protección de spawn. Cifras PENDIENTE.
- Diffs menores en `RocketLauncher.asset` y `EditorBuildSettings.asset`.

## No comprobado / no afirmar

- Play manual Greybox con teclado/ratón reales.
- Power-ups o cohete como «terminados» por existir clases.
- Multijugador de partida.
- Ranked y monstruos. M01–M04 no están validados: el humo del 10 nov 2026 comprueba apertura, colisión, un rayo de spawn, la rampa de M01 y que M04 no tiene suelo continuo. M02 se regeneró después y su humo de suelo volvió a pasar. No recorre escaleras, pickups ni Host Mode. El material sigue siendo el shader de color de vértice de ProBuilder.
- Cifras de D-07, D-08, D-10 y D-11. El código las lee de campos marcados PENDIENTE.

## Historial de pasadas (conservado)

Ver secciones de movimiento, escopeta, presentación y feel en commits previos y XML en `Documentation/test-results/`. La pasada de feel del 5 nov está en el historial de git (`0b361cd` y siguientes). HEAD tipico de auditoría: `992bb92`.
