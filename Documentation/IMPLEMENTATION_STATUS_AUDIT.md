# Auditoría de estado de implementación

Fecha: 9 de noviembre de 2026.
HEAD verificado: `992bb92dfce9bae03ee06b0a34a58405e851e3ce` (`Add local armor, pickups, and the rocket launcher presentation.`).
Cambios locales sin commit (no descartar): fix de ciclo `IDamageModifiers` / `Meatgrinder.Combat.asmdef` / `PowerUpManager`; diffs menores en `RocketLauncher.asset` y `EditorBuildSettings.asset`.
No se reejecutó la suite histórica en esta fecha. Últimos XML de revisión: `Documentation/test-results/review-*.xml` (5 nov). El humo de las cuatro escenas de mapa está en `Documentation/test-results/maps-playmode.xml` (4/4, 10 nov 2026) y no valida recorridos ni Host Mode.

## Criterio

| Estado | Significado |
|---|---|
| NOT_STARTED | Sin código útil |
| PARTIALLY_IMPLEMENTED | Código o assets sin flujo jugable completo |
| IMPLEMENTED | Lógica completa en aislamiento |
| INTEGRATED | Cableado al jugador/escena/partida local |
| VALIDATED | INTEGRATED + pruebas repetibles (y net si aplica) |
| BLOCKED | Falta decisión o dependencia externa |

Alcance de todas las filas: `REQUIRED`.

## Sistemas

| Sistema | Estado | Evidencia | Notas |
|---|---|---|---|
| Movimiento Quake-like | VALIDATED (local) | `MovementSimulator`, tests | Sin red |
| Cámara / look | VALIDATED (local) | `PlayerLook` | |
| Input mapa | PARTIALLY_IMPLEMENTED | Asset + reader | Play manual vivo pendiente |
| Escopeta | INTEGRATED (local) | Prefab + Greybox + tests | VALIDATED manual pendiente |
| Lanzacohetes | INTEGRATED (local) | Segundo hueco del `LocalPlayer` + `TryGrant` + `LocalProjectileSpawner` | Cifras PENDIENTE. Sin red |
| WeaponController multi-slot | INTEGRATED (local) | Escopeta y cohete, munición por hueco | |
| Melee | INTEGRATED (local) | `PlayerMelee`, tecla V provisional | Alcance/daño/cooldown PENDIENTE |
| Salud / muerte / respawn | INTEGRATED (local) | `PlayerLife` | Segundos D-08. Sin red |
| Armadura | PARTIALLY_IMPLEMENTED | Absorción y pickup local | D-07 abierta |
| Pickups salud/armadura/arma | INTEGRATED (local) | `PickupController` en las cuatro escenas | Cantidades 25 / 30 s / 2 son PENDIENTE. D-11 |
| Power-ups | INTEGRATED (local) | `PowerUpManager.Grant` crea el efecto | Acumulación D-11 provisional: reinicia |
| Quad → daño | INTEGRATED (local) | `DamageResolver` lee `IDamageModifiers` | Qué hace y cuánto dura: D-10 |
| Haste → movimiento | INTEGRATED (local) | `ISpeedScale` en `PlayerMovement` | Cifra PENDIENTE |
| Berserk → melee | INTEGRATED (local) | Solo `DamageType.Melee` | Cifra PENDIENTE |
| HUD salud/munición/retícula | INTEGRATED (local) | `CombatHud` | |
| HUD power-ups | INTEGRATED (local) | Busca `PowerUpManager` en el jugador | |
| Fusion sesión sonda | VALIDATED (probe) | F3-02 | Cubos, no gameplay |
| Jugador NetworkObject | NOT_STARTED | LocalPlayer offline | |
| Sync combate/pickups | NOT_STARTED | — | |
| Boot / menús producto | NOT_STARTED | — | |
| Monstruos jugables | NOT_STARTED | `.gitkeep` | |
| M01 | PARTIALLY_IMPLEMENTED | `M01_TheSlaughterhouse` ampliado | Rampa original conservada. Sin MapDefinition. No VALIDATED |
| M02 Castle of the Damned | PARTIALLY_IMPLEMENTED | `M02_CastleOfTheDamned` | GDD: Iron Cathedral. T-27. No VALIDATED |
| M03 Ruins of the Forgotten | PARTIALLY_IMPLEMENTED | `M03_RuinsOfTheForgotten` | GDD: The Pit. T-27. No VALIDATED |
| M04 The Longest Void | PARTIALLY_IMPLEMENTED | `M04_TheLongestVoid` | GDD: Blood Foundry. Sin suelo continuo. No VALIDATED |
| Ranked | NOT_STARTED | R-01…04 | |
| Arsenal ampliado | NOT_STARTED | P-05 | |
| Audio / VFX producción | NOT_STARTED | — | |
| Estilo / asistencias / barriles | NOT_STARTED | P-* | |

## Porcentaje

Método: tareas del checklist con estado ≥ INTEGRATED o VALIDATED / 119+ (IDs históricos + nuevas F2-13… si se añaden).

Estimación 9 nov 2026: **~16–18 %** del juego completo (misma orden de magnitud que la auditoría crítica; el alcance ya no se restringe al MVP).
