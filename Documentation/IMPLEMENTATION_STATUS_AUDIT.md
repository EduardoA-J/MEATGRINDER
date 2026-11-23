# Auditoría de estado de implementación

## Estado vigente — producto local, 22 noviembre 2026

Esta sección sustituye las afirmaciones de ausencia de Boot, melee, puertas y UI de las fotografías históricas inferiores. Ver resultados y alcance en `PROGRESS.md`; evidencia actual en `test-results/product/`.

**Integrado localmente:** `GameBootstrap`/`LocalGameSession` persistente; Boot y cuatro MapDefinition; menú/selección/carga/opciones/pausa/resultados uGUI; primera aparición melee-only; equipamiento por pickups, cargadores y reservas independientes, loot exacto; melee con fases y cobertura, cohete con autodaño/impulso; seis poderes efectivos con prefabs/iconos/timers; muerte por vacío en máximo 5 s con cámara y oscurecimiento, respawn de 4 s en otro punto; llaves y dos puertas por mapa con estado compartido local. Animaciones de armas/cámara/puertas y audio/VFX propios conectados a eventos.

**Contenido:** materiales URP texturados originales, vestido industrial/gótico/ruinas/vacío según mapa y tres modelos aportados como blancos destructibles. Se preservaron las escenas existentes, rampa y llegada de M01. El modelo de 1,98 millones de vértices no está en escenas. Falta rig/LOD, sombreado/acabado final, enemigos jugables y recorrido manual completo; la existencia de un blanco no acredita un monstruo.

**Defectos de integración corregidos:** LifeTuning ausente impedía daño de vacío; un pickup M02 carecía de definición; PlayerLife duplicado podía ejecutar dos ciclos; importación de iconos no generaba Sprite; pulso visual deformaba pedestales. Las pruebas anteriores de lógica no detectaban esos problemas de serialización/presentación. Las nuevas pruebas abren las escenas de producto y sus assets.

**Límites:** solo autoridad local. El bootstrap Fusion y la sonda NetworkTest conservan su estado previo; no hay gameplay replicado ni prueba de dos clientes. Resultados locales no son puntuación/victoria Versus. Modos, monstruos, ranked y arsenal ampliado siguen REQUIRED. Las cifras aprobadas son provisionales; tiempos visuales nuevos quedan configurables y PENDIENTE de balance. No se declara 60 FPS ni mapa VALIDATED por una captura.

«Crear partida local» tiene selección de mapa y confirmación explícita, modo práctica identificado y acceso a los ajustes soportados. Versus/Ranked aparecen como no disponibles. El proyectil usa un Transform de boca de arma asignado, con prueba de obstrucción entre cámara y boca; sustituir el modelo requiere conservar o reasignar ese socket, sin cambiar la lógica.

Arquitectura actual y procedencia: `DEPENDENCY_GRAPH.md`, `ARSENAL_POWERUPS_IMPLEMENTATION.md`, `REFERENCE_REPOSITORIES_AUDIT.md`. El índice de `MASTER_CHECKLIST.md` prevalece sobre sus fichas históricas.

## Fotografía histórica — 9 noviembre

Fecha: 9 de noviembre de 2026.
HEAD verificado: `992bb92dfce9bae03ee06b0a34a58405e851e3ce` (`Add local armor, pickups, and the rocket launcher presentation.`).
Cambios locales sin commit (no descartar): fix de ciclo `IDamageModifiers` / `Meatgrinder.Combat.asmdef` / `PowerUpManager`; diffs menores en `RocketLauncher.asset` y `EditorBuildSettings.asset`.
No se reejecutó la suite histórica en esta fecha. Últimos XML de revisión: `Documentation/test-results/review-*.xml` (5 nov). El humo de las cuatro escenas de mapa está en `Documentation/test-results/maps-playmode.xml` (4/4, 10 nov 2026) y no valida recorridos ni Host Mode.

## Addendum de implementación — 22 de noviembre de 2026

La auditoría anterior es una fotografía histórica. Desde entonces se implementó una pasada local, sin modificar Fusion: armadura 100/50 %, identificadores de daño contextualizados, melee con comprobación de cobertura, cohete con autodaño reducido y rocket jump, HUD reactivo a curación, sprint/agacharse, respawn de 4 s con protección cancelable al atacar, loot de armas/munición y caída al vacío con daño progresivo. Las definiciones de pickups de recursos y poderes se centralizaron en el kit de `MapAuthor` y se regeneraron las cuatro escenas. EditMode pasó 43/43 en `Documentation/test-results/local-integration-editmode-final-2.xml` y PlayMode pasó 12/12 en `Documentation/test-results/local-integration-playmode-final.xml`.

Estos sistemas son `INTEGRATED` solo en el flujo local. No se elevan a `VALIDATED` de producto: falta prueba manual completa, Boot, puertas/llaves, autoridad Host Mode y dos clientes reales. El vacío y el loot requieren todavía una prueba PlayMode dedicada.

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
| Lanzacohetes | INTEGRATED (local) | Segundo hueco del `LocalPlayer` + `TryGrant` + `LocalProjectileSpawner` | Valores provisionales aprobados. Sin red |
| WeaponController multi-slot | INTEGRATED (local) | Escopeta y cohete, munición por hueco | |
| Melee | INTEGRATED (local) | `PlayerMelee`, tecla V provisional | Valores provisionales aprobados |
| Salud / muerte / respawn | PARTIALLY_IMPLEMENTED (local) | `PlayerLife`, `DeathLoot`, `PlayerVoidFall` | 4 s, melee-only, punto seguro; sin red/manual |
| Armadura | INTEGRATED (local) | Absorción y pickup local | Máximo 100, absorción 50 % |
| Pickups salud/armadura/arma | INTEGRATED (local) | `PickupController` en las cuatro escenas | +25, recursos 20 s, poderes 60 s |
| Power-ups | INTEGRATED (local) | `PowerUpManager.Grant` crea el efecto | D-11: reinicia duración, no potencia; tipos distintos coexisten |
| Quad → daño | INTEGRATED (local) | `DamageResolver` lee `IDamageModifiers` | ×4 durante 30 s; configurable para balance |
| Haste → movimiento | INTEGRATED (local) | `ISpeedScale` en `PlayerMovement` | ×1,3 durante 20 s; configurable |
| Berserk → melee | INTEGRATED (local) | Solo `DamageType.Melee` | ×2 durante 20 s; configurable |
| HUD salud/munición/retícula | INTEGRATED (local) | `CombatHud` | |
| HUD power-ups | INTEGRATED (local) | Busca `PowerUpManager` en el jugador | |
| Fusion sesión sonda | VALIDATED (probe) | F3-02 | Cubos, no gameplay |
| Jugador NetworkObject | NOT_STARTED | LocalPlayer offline | |
| Sync combate/pickups | NOT_STARTED | — | |
| Boot / menús producto | NOT_STARTED | — | |
| Monstruos jugables | NOT_STARTED | `.gitkeep` | |
| M01 | PARTIALLY_IMPLEMENTED | `M01_TheSlaughterhouse` complejo industrial, 10 nov 2026 | Rampa original conservada. Sin MapDefinition. Humo 4/4 no recorre escaleras. No VALIDATED |
| M02 Castle of the Damned | PARTIALLY_IMPLEMENTED | `M02_CastleOfTheDamned` recinto ~540 m, torres, ruinas y plataformas | GDD: Iron Cathedral. T-27. No VALIDATED |
| M03 Ruins of the Forgotten | PARTIALLY_IMPLEMENTED | `M03_RuinsOfTheForgotten` plaza, templo, terrazas | GDD: The Pit. T-27. No VALIDATED |
| M04 The Longest Void | PARTIALLY_IMPLEMENTED | `M04_TheLongestVoid` islas a varias alturas | GDD: Blood Foundry. Sin suelo continuo. No VALIDATED |
| Ranked | NOT_STARTED | R-01…04 | |
| Arsenal ampliado | NOT_STARTED | P-05 | |
| Audio / VFX producción | NOT_STARTED | — | |
| Estilo / asistencias / barriles | NOT_STARTED | P-* | |

## Porcentaje

Método: tareas del checklist con estado ≥ INTEGRATED o VALIDATED / 119+ (IDs históricos + nuevas F2-13… si se añaden).

Estimación 9 nov 2026: **~16–18 %** del juego completo (misma orden de magnitud que la auditoría crítica; el alcance ya no se restringe al MVP).
