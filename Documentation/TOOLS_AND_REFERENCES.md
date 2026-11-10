# Herramientas y referencias — evaluación

Fecha: 9 de noviembre de 2026.
Alcance: ProBuilder quedó instalado (T-26). Luna sigue sin comprar.

HEAD del juego: `992bb92` en `main`, más cambios locales sin commit. Proyecto Unity: `F:\MAPAUNI\MEATGRINDER\grinder` (`6000.5.6f1`, URP 17.5.0, Fusion 2.1.4 Host Mode). `com.unity.probuilder` **6.1.4** está en `Packages/manifest.json`.

## Dónde viven los clones

`F:\MAPAUNI\references\` (fuera de `grinder/`, fuera de git del juego, sin submódulos):

| Carpeta | Origen | Esta pasada |
|---|---|---|
| `salvations-descent` | ya estaba | no se volvió a clonar |
| `SourceEngineMovementUnity` | ya estaba | no se volvió a clonar |
| `doom` | `https://github.com/paul8483/doom` | clon shallow nuevo |

## ProBuilder — instalado (T-26)

| Dato | Valor verificado |
|---|---|
| Paquete | `com.unity.probuilder` **6.1.4** |
| Licencia | paquete oficial de Unity, sin compra |
| Dependencias resueltas | `com.unity.settings-manager` 1.0.3 (añadida por UPM). Shader Graph 17.5.0 ya estaba vía URP. Módulos `imgui` y `physics` |
| URP | 6.1.4 no trae un sample de shaders URP. El material de M01 usa `ProBuilder6/Standard Vertex Color` (Shader Graph del paquete). No es el shader de error |
| Escenas | `M01_TheSlaughterhouse` (ampliada), `M02_CastleOfTheDamned`, `M03_RuinsOfTheForgotten`, `M04_TheLongestVoid`. Detalle en `MAPS.md` |
| Rampa de M01 | La prueba automática antigua no la subió. David la recorrió a mano. `PB_Ramp` se conserva: posición `(3.5, 1.218, 4)`, rotación yaw 180. `Create M01 Shell` se niega si la escena existe |
| Mapas | PARTIALLY_IMPLEMENTED. Humo `maps-playmode.xml` 4/4. Ninguno VALIDATED |
| Greybox | no se modificó |

Uso previsto: geometría editable de M01–M04. Greybox sigue siendo laboratorio.

## Luna UI — bloqueada por compra (D-24)

Producto vigente: **Luna v2 — Complete UI Solution** (CupkekGames). La ficha 1.4.0 (id 299710) está retirada; la documentación actual es la 2.1.5.

| Dato | Valor |
|---|---|
| Motor | **UI Toolkit**. El propio asset dice que no es para uGUI |
| Editor | Unity **6000.5 o posterior**. `6000.5.6f1` entra |
| URP | compatible a nivel de panel UI; no renderiza el mundo |
| Red | no habla con Fusion. Los botones solo pueden llamar a nuestra API de sesión |
| Dependencia declarada | Input System (ya tenemos 1.20.0). El resto viaja dentro del paquete (`Packages/com.cupkekgames.luna/Dependencies/`) |
| Precio | de pago, licencia por asiento, EULA estándar de Asset Store. Un agregador mostraba 54,99 USD / 27,49 USD en oferta. **El precio de caja hay que confirmarlo en el checkout; no se ha comprado** |
| Riesgo | no añadir el registro git de CupkekGames junto al paquete de la tienda (ensamblados duplicados). No importar el sample de inventario/RPG como si fuera el juego |

El HUD de combate actual es uGUI (`CombatHud`). Propuesta, no decisión: menús/pausa/lobby en Luna; HUD de partida sigue en uGUI hasta una migración aparte. D-09 queda abierta hasta el sí de D-24.

## Licencias de referencias

| Repo | Licencia del código de juego | Qué se puede hacer |
|---|---|---|
| `doom` | **No hay LICENSE de proyecto.** Terceros: LibTess SGI, Nuked OPL LGPL-2.1, Super-xBR MIT, Freedoom BSD-like en `Distribution/FREEDOOM-LICENSE.txt` | Estudiar. No copiar scripts, shaders, texturas ni el WAD |
| `salvations-descent` | Sin LICENSE. Incluye Odin, Aura 2 y Retro FX (plugins de pago) | Estudiar ideas. No copiar ni importar plugins |
| `SourceEngineMovementUnity` | Sin LICENSE | Consulta. No sustituye `MovementSimulator` |

Editor del clon DOOM: `6000.4.8f1`. No abrir ese proyecto con `6000.5.6f1` si no hace falta: Unity reescribiría sus ProjectSettings.

## Matriz (código verificado)

| Sistema | Referencia | MEATGRINDER ahora | Aprovechar | Acción |
|---|---|---|---|---|
| Movimiento | Source `PlayerMovementUtil` | `MovementSimulator` VALIDATED local. Strafe propio; no se copió el clamp de Source | Solo comparar. Escaleras del clon no están en el GDD | No reemplazar |
| Armas | Salvation `WeaponSystem` / `WeaponBase`; DOOM `WeaponLoadout` | `WeaponController` + escopeta INTEGRATED. Cohete PARTIALLY_IMPLEMENTED | Idea ya usada: `DrawDuration`. DOOM es tabla de armas de WAD, no nuestro `WeaponDefinition` | No portar. Cablear el cohete propio |
| Melee | Salvation `MeleeWeapon` (Odin + `Input.GetMouseButton`) | `DamageType.Melee` solamente. F2-13 NOT_STARTED | Ningún archivo portable | Implementar melee propio sobre `DamageResolver` |
| Enemigos | DOOM `MonsterDef` + IA de WAD; Salvation `BehaviorStateManager` | Sin monstruos | Máquina de estados simple como idea, no el código (Odin / WAD) | Diseñar `MonsterDefinition` propio cuando toque F4 |
| Salud / armadura | DOOM `HealthModel` (1/3 y 1/2) | `HealthComponent` + armadura parcial. D-07 abierta | No adoptar las fracciones de DOOM: cerrarían D-07 en silencio | Esperar D-07 |
| Pickups | DOOM `ThingPickup`; Salvation `WeaponPickup` | `PickupController` PARTIALLY_IMPLEMENTED. Power-ups sin efecto real | Trigger + respawn ya están esbozados | Completar el nuestro |
| Puertas / elevadores | DOOM `SectorMover`; Salvation `TriggerDoorBehavior`, `Elevator` | No hay puertas | Patrón: offset o dos puntos, collider que se mueve. El código de DOOM depende del mapa WAD | Escribir un mover propio en F5-06 |
| Barriles | DOOM `BarrelRules` (20 HP, explosión en frame) | P-03 NOT_STARTED | Idea de objeto dañable que llama al resolver | Más tarde, código propio |
| Mapas | ProBuilder 6.1.4 | Cuatro blockouts. Rampa de M01 conservada | Geometría editable | Recorrer rutas a mano. No regenerar `PB_Ramp` |
| UI menús | Luna v2 | Sin Boot/menú. HUD uGUI local | Navegación de pantallas | Comprar solo si D-24 |
| Audio / VFX | DOOM (WAD, OPL3 LGPL) | Sin set de producción | No reutilizar el pipeline de audio DOOM | F8 propio |

## Plan por prioridad

| P | Bloque | Hecho en esta pasada | Siguiente acción | Prueba |
|---|---|---|---|---|
| P0 | ProBuilder 6.1.4 | Instalado. Shader de paquete, no magenta | No sustituir la rampa ya recorrida | Recorrido manual de David |
| P0 | No portar movimiento ni daño ajenos | Confirmado | Seguir `MovementSimulator` y `DamageResolver` | Tests de movimiento existentes |
| P1 | M01–M04 ProBuilder | Blockouts en escena. Humo 4/4 | Recorrido completo y MapDefinition | Caminar rutas, escaleras y pickups |
| P1 | Luna v2 | Evaluado, no comprado | David confirma precio y D-24 | Una pantalla de menú que no escribe salud ni sesión |
| P2 | Puertas propias (idea Salvation/DOOM) | No | F5-06 sobre el mover propio | La puerta cambia collider y no teletransporta |
| P3 | Barriles, IA de monstruo | No | Después de daño y melee | Un barril daña vía resolver |

## Próximo bloque

1. D-23 está cerrada (T-26). ProBuilder 6.1.4 ya está. La rampa de M01 que David recorrió no se sustituye. Los cuatro mapas están en `MAPS.md` y no están validados.
2. **D-24** queda en espera. No comprar Luna.
4. El núcleo de combate (cohete en loadout, pickups con efecto, melee) sigue siendo trabajo de código propio, en paralelo solo si no toca la misma escena M01.

## Riesgos

- Abrir el clon DOOM con nuestro editor y subir su versión sin querer.
- Copiar `HealthModel` y dar por cerrada D-07.
- Instalar Luna y el registro CupkekGames a la vez.
- Mezclar Luna (UI Toolkit) con el HUD uGUI sin definir orden de dibujo.
- Gastar el ciclo vistiendo Greybox en vez de M01.
- Tratar esta evaluación como integración. No lo es.
