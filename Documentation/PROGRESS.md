# Progreso

Actualización vigente: 22 de noviembre de 2026. Se conserva el historial inferior.
Alcance: juego completo (`GDD_SCOPE_ADDENDUM.md`). Estados: `MASTER_CHECKLIST.md`.

## Integración de producto local y presentación — 22 noviembre

Estado vigente: Boot → menú uGUI → selección/carga de M01–M04 → combate local → pausa/muerte/respawn → salida con llave o fin de sesión → resultados → Boot. Escopeta, cohete y melee tienen presentación propia y recursos separados; seis power-ups tienen efectos, prefabs, símbolos y timers. Hay opciones de volumen, sensibilidad/FOV, pantalla y reducción de efectos. Los mapas reciben materiales texturados y una primera pasada de entorno; tres FBX aportados funcionan como blancos estáticos destructibles.

Uso: abrir `grinder/Assets/_Project/00_Scenes/Boot.unity` y entrar en Play, o ejecutar `grinder/Builds/LocalProduct/MEATGRINDER.exe` conservando sus carpetas adyacentes. «Jugar» abre el mapa elegido; «Crear partida local» permite revisar mapa/modo disponible, abrir ajustes y confirmar antes de cargar. Solo está disponible práctica local; no se simulan modos competitivos. Controles: WASD, ratón, Espacio, Shift sprint, Ctrl agacharse, V melee (también clic sin arma), R recarga, 1/2 selección de arma recogida, Escape pausa. Empiezas solo con melee: las armas y munición se recogen en el escenario.

Correcciones encontradas en las escenas reales: ajustes de vida/vacío sin asignar, un pickup sin definición, un componente PlayerLife duplicado, sprites importados sin sprite individual y pulso de pedestal que sobrescribía su escala. Se conectaron los assets reales y se añadieron pruebas de flujo; no bastaba la cobertura de clases de la pasada anterior.

PlayMode: **19/19**, `test-results/product/playmode.xml`. Comprueba menú/mapa/pausa/resultados, cuatro escenas con tuning y pickups, melee con preparación y cobertura, llave/puertas/transición M01→M02, recuperación del loot sin crear munición, origen real del cohete sin atravesar cobertura y muerte por vacío ≤5 s incluso con poderes, seguida de respawn melee-only. Capturas reales de juego/HUD/muerte y primera aparición de los cuatro mapas en la misma carpeta. Las pruebas existentes de movimiento y combate también pasan. EditMode: **44/44**, `test-results/product/editmode.xml`, incluidos los enlaces serializados a tuning, sprites, arma y punto de salida del cohete.

Windows Standalone x64 Development compiló sin errores, tamaño reportado 193.924.628 bytes (`test-results/product/windows-build.txt`). Se ejecutó el binario en la UHD 630 con `--meatgrinder-smoke`: **PASS**, código de salida 0, Boot → M01 melee-only → pausa → resultados → Boot (`test-results/product/windows-smoke.log`). Es una comprobación automática del flujo fuera del editor; no es medición de FPS ni sesión manual completa. El probe solo se habilita en Editor/Development y requiere ese argumento explícito.

La suite final PlayMode también recorre Crear partida → mapa → ajustes → volver → confirmación → carga. La comprobación enfocada de UI pasó **1/1** (`test-results/product/ui-flow.xml`), capturando menús/resultados tras finalizar su transición para verificar legibilidad. Unity reserializó ajustes auxiliares al compilar; se restituyeron únicamente esos cambios automáticos en GraphicsSettings, PlayerSettings y UnityConnectSettings tras verificar el diff, conservando la configuración original y el cambio preexistente de ProBuilder.

Los números de daño/movimiento siguen siendo el balance inicial configurable aprobado. El tiempo de apertura de puertas (1,2 s), preparación melee (0,12 s) y ventana de impacto (0,1 s) son tiempos de presentación/prueba configurables, pendientes de ajuste definitivo. La última puerta cierra la sesión local; no define la victoria Versus.

Checklist: **130 tareas** (el recuento anterior de 128 omitía F1-12 y F2-15); 24 VALIDATED, 33 INTEGRATED, 16 PARTIALLY_IMPLEMENTED, 4 BLOCKED y 53 NOT_STARTED. No equivalen a porcentaje de producto: las tareas tienen complejidad distinta y varias integraciones solo están acreditadas localmente.

Pendiente: autoridad Fusion y dos clientes reales, modos/equipos/victoria/roles, monstruos jugables, ranked, arsenal ampliado, rigs/LOD y arte/audio final, recorrido humano completo y perfiles de rendimiento. No se reclama juego completo ni multiplayer terminado. Base mínima de hardware, no objetivo definitivo de FPS: i5-10400/UHD630/16 GB a 1366×768.

Se conserva `ProjectSettings/Packages/com.unity.probuilder/Settings.json` preexistente. Único cambio de configuración de esta pasada: registrar Boot en Build Settings. Sin cambios de paquetes, versiones ni Fusion; sin commits/merge/push. Referencias estudiadas, comportamientos reproducidos con código propio; ver `REFERENCE_REPOSITORIES_AUDIT.md`.

## Fotografía previa (histórica, sustituida por el estado anterior)

Nota del 22 de noviembre de 2026: decisiones de diseño confirmadas por David registradas en `DECISIONS.md`, sección «Resoluciones de David». Esta pasada integró y probó localmente armadura, daño de vacío, sprint/agacharse, melee con cobertura, respawn melee-only, loot de armas/munición, rocket self-damage/rocket jump, HUD reactivo y timers de pickups. Unity EditMode: 43/43 (`Documentation/test-results/local-integration-editmode-final-2.xml`); PlayMode: 12/12 (`Documentation/test-results/local-integration-playmode-final.xml`). El cambio local preexistente en `grinder/ProjectSettings/Packages/com.unity.probuilder/Settings.json` se conserva. Fusion, Boot, menú, monstruos, autoridad de red y validación manual de mapas siguen pendientes.

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
| Lanzacohetes | INTEGRATED local. Proyectil, explosión, autodaño y rocket jump; red pendiente. |
| Melee | INTEGRATED local. Cobertura y Berserk/Quad vía DamageResolver. |
| Salud / muerte | PARTIALLY_IMPLEMENTED. Respawn local 4 s, protección 1 s, loot y vacío; red/manual pendiente. |
| Armadura | INTEGRATED local. 100 de máximo, 50 % de absorción. |
| Pickups / power-ups | INTEGRATED local en código y kit de mapas; escenas M01–M04 regeneradas con definiciones. |
| HUD | INTEGRATED local (salud/armadura/munición/retícula/potenciadores), curación y respawn notifican. |
| Fusion gameplay | NOT_STARTED. Solo sonda. |
| Boot / menús / monstruos / ranked | NOT_STARTED |
| M01–M04 | PARTIALLY_IMPLEMENTED. Escenas ProBuilder con rutas mixtas, coberturas, spawns y los ocho power-ups configurados. Falta recorrido manual, puertas/llaves y Host Mode. T-27 |
| Lanzacohetes, melee, pickups, respawn local | INTEGRATED en código con balance provisional aprobado. Sin red |

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
- Gameplay local: cohete en el loadout, pickups, quad, haste, melee, berserk, respawn y protección de spawn. Cifras iniciales configurables aprobadas; el balance definitivo sigue abierto.
- Diffs menores en `RocketLauncher.asset` y `EditorBuildSettings.asset`.

## No comprobado / no afirmar

- Play manual Greybox con teclado/ratón reales.
- Power-ups o cohete como «terminados» por existir clases.
- Multijugador de partida.
- Ranked y monstruos. M01–M04 no están validados: el humo del 10 nov 2026 comprueba apertura, colisión, un rayo de spawn, la rampa de M01 y que M04 no tiene suelo continuo. M02 se regeneró después y su humo de suelo volvió a pasar. No recorre escaleras, pickups ni Host Mode. El material sigue siendo el shader de color de vértice de ProBuilder.
- Balance definitivo más allá del conjunto inicial aprobado. El código lee los valores desde assets y tuning configurables.

## Historial de pasadas (conservado)

Ver secciones de movimiento, escopeta, presentación y feel en commits previos y XML en `Documentation/test-results/`. La pasada de feel del 5 nov está en el historial de git (`0b361cd` y siguientes). HEAD tipico de auditoría: `992bb92`.
