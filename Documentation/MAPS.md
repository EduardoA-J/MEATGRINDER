# Mapas

Fecha: 9 de noviembre de 2026.
Herramienta: ProBuilder 6.1.4. Material compartido: `Assets/_Project/00_Scenes/Maps/M01/M01_Blockout.mat` (`ProBuilder6/Standard Vertex Color`). El color de cada pieza es color de vértice. Las piezas siguen siendo mallas editables.

Estado de las cuatro escenas: `PARTIALLY_IMPLEMENTED`. Ninguna es `VALIDATED`. No hay `MapDefinition`, ni `SpawnManager`, ni prueba con dos clientes. Host Mode queda para una fase posterior.

Nombres: T-27. El GDD sigue escribiendo M02 Iron Cathedral, M03 The Pit y M04 Blood Foundry. Las escenas pedidas el 9 de noviembre son las de esta página. El GDD no se reescribió.

## Escala

El salto del jugador es de unos 1,28 m (`jumpVelocity` 7,5 y `gravity` 22). El escalón automático es 0,4 m y la pendiente límite 46°. Las escaleras de estos mapas suben 0,3 m por peldaño. Los enlaces obligatorios son losas o escaleras, no huecos que exijan strafe jump.

Los colliders nuevos son `BoxCollider` del tamaño del cubo ProBuilder. La rampa `PB_Ramp` no se regeneró.

## Cifras de ítems — PENDIENTE

Los assets de `Assets/_Project/02_ScriptableObjects/Pickups/` usan los valores que ya tenía el código, no cifras del GDD:

| Asset | Tipo | Valor | Pendiente |
|---|---|---|---|
| `Health_Small` | Salud | 25 | D-11, cantidad |
| `Armor_Small` | Armadura | 25 | D-07 y D-11 |
| `Quad` | Quad Damage | duración 30, efecto 2 | D-10 y D-11 |
| `Berserk` | Berserk | duración 30, efecto 2 | D-10 y D-11 |
| `Rocket_Ammo` | Arma / munición | 4 cohetes | Cantidad no está en el GDD |

Recoger el mismo power-up reinicia el efecto. No suma duraciones. Eso es la regla provisional de D-11.

El respawn del jugador usa `PlayerLife` ya existente. El retardo de 3 s y la protección de 1 s siguen siendo D-08. Cada escena apunta `spawnPoint` del jugador de esa escena a `Spawn_Human_A`. No hay un gestor de spawns ni un teletransporte de caída.

## M01 — The Slaughterhouse

Escena: `Assets/_Project/00_Scenes/Maps/M01/M01_TheSlaughterhouse.unity`.

La ampliación vive bajo `M01_Expansion`. `Meatgrinder/Maps/Create M01 Shell` se niega a recrear la escena. `Meatgrinder/Maps/Expand M01` solo sustituye `M01_Expansion`.

Se conservan `PB_Floor`, `PB_Step`, `PB_Wall` y `PB_Ramp`. La rampa sigue en `(3.5, 1.218, 4)` con la rotación que David recorrió a mano (yaw 180, inclinación 18°). La prueba automática antigua no la subió. Esta pasada no la volvió a intentar.

Alrededor del pad original:

- Flancos este y oeste, bahías sur y una arena al norte de unos 40 × 32 m.
- Dos entradas hacia la arena, una por cada flanco.
- Escaleras de 0,3 m hasta una pasarela a 3 m, con descansillos que la unen a los flancos.
- Columnas, vigas, tuberías y bloques de cobertura en la arena.
- El muro norte del pad original sigue cerrado. El paso al norte sale por los flancos.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Bahías sur, x ±22, z −12 | Fuera de la arena. Un muro de la arena corta la vista hacia el spawn de monstruo A |
| `Spawn_Monster_A` / `B` | Fondo de la arena, x ±12, z 56 | Detrás de columnas y del muro sur de la arena |
| Salud | Flanco oeste | Ruta lateral, no el centro |
| Berserk | Flanco oeste, más al norte | Hay que comprometerse al pasillo |
| Cohetes | Flanco este | La otra ruta |
| Quad | Centro de la arena | Disputable y a la vista |
| Armadura | Pasarela, y = 3,4 | Hay que subir |

## M02 — Castle of the Damned

Escena: `Assets/_Project/00_Scenes/Maps/M02/M02_CastleOfTheDamned.unity`.

Patio de 36 × 36 m, sala norte, patio sur, cortinas con hueco central, cuatro torres con piso intermedio y alto, puente a 3 m entre las torres del norte, altillo y cubierta de la sala. Los arcos son piezas ProBuilder en eje, no un primitivo de arco.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Torres suroeste y sureste | El muro norte del patio corta la vista hacia la sala |
| `Spawn_Monster_A` / `B` | Sala norte, x ±6, z 30 | Sobre el suelo de la sala |
| Quad | Centro del patio | Disputable |
| Armadura | Puente, y = 3,5 | Control de altura |
| Berserk | Fondo de la sala | Hay que cruzar el arco |
| Salud y cohetes | Patio sur | Ruta exterior, lejos del quad |

No hay puerta de recorrido. F5-06 sigue sin empezar.

## M03 — Ruins of the Forgotten

Escena: `Assets/_Project/00_Scenes/Maps/M03/M03_RuinsOfTheForgotten.unity`.

Plaza abierta de unos 56 m, sin cubierta general. El centro es un cuenco a −1,6 m con escaleras al norte y al sur. El templo está a 3 m. Las terrazas laterales están a 2,4 m. Un puente estrecho cruza el templo a 6,2 m. Hay columnas, dos arcos y muros del templo con un hueco central.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` | Terraza oeste | El muro frontal izquierdo del templo corta la vista al monstruo A |
| `Spawn_Human_B` | Terraza este | Lado opuesto |
| `Spawn_Monster_A` / `B` | Suelo del templo, z 36 | Detrás del muro frontal |
| Salud | Fondo del cuenco | Hay que bajar |
| Armadura | Templo | Altura media |
| Quad | Puente del templo, y = 6,7 | El punto alto disputable |
| Berserk | Terraza oeste | Control de esa altura |
| Cohetes | Terraza este | La otra altura |

## M04 — The Longest Void

Escena: `Assets/_Project/00_Scenes/Maps/M04/M04_TheLongestVoid.unity`.

No hay suelo continuo ni plano bajo las plataformas. El cielo de la escena está vacío y la cámara de esta instancia usa color sólido oscuro. La referencia es la filosofía de Q3DM17, no una copia.

Plataforma central a y = 4 (16 × 16 m). Cuatro islas a la misma altura, unidas por puentes de 3,2 m que pisan las plataformas. Dos cubiertas altas a 8,5 m, al norte y al este, con escaleras de 0,3 m. Una plataforma baja al noroeste de la isla oeste, unida por escaleras. Caer fuera de esas piezas es el vacío. No se añadió teletransporte: `PlayerLife` reaparece en `Spawn_Human_A` con las reglas ya existentes.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Plataforma central | El muro de la cubierta norte corta la vista al monstruo A |
| `Spawn_Monster_A` | Cubierta norte, y = 8,6 | Altura, detrás de ese muro |
| `Spawn_Monster_B` | Cubierta este, y = 8,6 | La otra altura |
| Salud | Centro | Cruce |
| Quad | Isla este | Hay que cruzar el puente |
| Berserk | Isla oeste | La ruta opuesta |
| Armadura | Cubierta norte | El punto alto |
| Cohetes | Isla sur | Otra salida del centro |

## Prueba de esta pasada

`Documentation/test-results/maps-playmode.xml`, 10 de noviembre de 2026, 4/4 en PlayMode:

- Cada escena abre.
- Hay mallas ProBuilder con collider y el material de bloqueo, sin shader de error.
- El jugador, andando unos 6 m hacia delante, no cae por debajo del suelo de salida.
- Un rayo de `Spawn_Human_A` a `Spawn_Monster_A` choca con geometría.
- M01 sigue teniendo `PB_Ramp`.
- M04 no tiene un objeto `PB_Ground`.

Eso no recorre escaleras, no recoge ítems y no prueba dos clientes. Por eso las tareas F5-02, F5-03, F5-04, F5-05, F5-09, F5-10 y F5-11 quedan en `PARTIALLY_IMPLEMENTED`.

## Cómo seguir editando

Abrir la escena. `Tools → ProBuilder → ProBuilder Window`. Las piezas nuevas cuelgan de `M01_Expansion`, `M02_Castle`, `M03_Ruins` o `M04_Void`. Duplicar o extruir esas mallas. Asignar `M01_Blockout` si una pieza nueva sale sin material. En sólidos gruesos, `BoxCollider` del tamaño de la pieza. No lanzar `Meatgrinder/Maps/Create M01 Shell`. Greybox no forma parte de estos mapas.
