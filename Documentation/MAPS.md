# Mapas

## Integración visual y flujo local — 22 noviembre 2026

Esta actualización prevalece sobre las descripciones históricas inferiores. `ProductSceneInstaller` añade contenido a las escenas existentes; no ejecuta la regeneración de `MapAuthor`. Comprueba que el transform y collider de `PB_Ramp` y `PB_RampLanding` de M01 permanezcan iguales. La geometría histórica y los cambios locales anteriores se conservan.

- Las cuatro escenas tienen `MapDefinition`, `GameBootstrap`, puntos de aparición, ajustes de vida/melee/vacío, armas recogibles, munición independiente y seis power-ups con definición válida. Primera aparición y respawn solo melee.
- M01 incorpora tanques, tuberías/raíles de procesado, controles y señalización; M02 contrafuertes, ventanas y braseros; M03 columnas, piezas caídas y relieve; M04 bases suspendidas, anclajes y balizas. Materiales URP texturados propios. Son una primera pasada de arte, no escenarios finales certificados.
- Cada mapa tiene una llave junto a un recurso de salud, una puerta de acceso al Quad y otra de avance. `MapRouteState` abre ambas para el estado local compartido. Salidas: M01 `(0,0,75)`, M02 `(0,0,84)`, M03 `(0,3.6,78)`, M04 `(0,10.5,42)`. M01→M02→M03→M04; la última salida muestra resultados locales. La regla de victoria Versus sigue pendiente.
- Tres modelos aportados se integran como blancos locales destructibles cerca de la primera aparición. No son monstruos controlables ni IA. El cuarto modelo, de casi dos millones de vértices, permanece fuera de las escenas.
- Vacío detectado bajo Y=-8 en M01–M03 y Y=-4 en M04; muerte en máximo 5 s desde detección, con daño progresivo y feedback de cámara. Loot perdido; respawn normal tras 4 s en otro punto seguro.

Entrar por `Assets/_Project/00_Scenes/Boot.unity`. La selección permite abrir cualquiera de los cuatro mapas. No ejecutar los generadores históricos para conservar esta pasada de contenido. Quedan pendientes recorrido humano completo, análisis de spawn-kill, accesibilidad de todas las rutas y perfiles de rendimiento en equipos de gameplay.

Fecha: 22 de noviembre de 2026.
Herramienta: ProBuilder 6.1.4. Material compartido: `Assets/_Project/00_Scenes/Maps/M01/M01_Blockout.mat` (`ProBuilder6/Standard Vertex Color`). La identidad de cada mapa sale del color de vértice, de la niebla y de luces puntuales. No se añadieron paquetes ni texturas nuevas. Las piezas siguen siendo mallas editables.

Estado de las cuatro escenas: `PARTIALLY_IMPLEMENTED`. Ninguna es `VALIDATED`. Las escenas tienen pickups locales configurados con ScriptableObjects y spawns editables; no hay todavía `MapDefinition`, puertas/llaves, `SpawnManager` autoritativo ni prueba con dos clientes.

Nombres: T-27. El GDD sigue escribiendo M02 Iron Cathedral, M03 The Pit y M04 Blood Foundry. Las escenas de esta página no reescriben el GDD.

## Escala

El salto del jugador es de unos 1,28 m (`jumpVelocity` 7,5 y `gravity` 22). El escalón automático es 0,4 m y la pendiente límite 46°. Las escaleras de esta pasada suben 0,3 m por peldaño y avanzan 0,32–1,0 m según el tramo. Los enlaces obligatorios son losas que se solapan o escaleras. No hay huecos que exijan strafe jump.

Los colliders nuevos son `BoxCollider` del tamaño de la pieza ProBuilder, también en cilindros y dovelas. La rampa `PB_Ramp` se conserva en `(3.5, 1.218, 4)`, con inclinación normalizada de −18° y una plataforma de llegada que continúa la ruta.

La geometría de M02, M03 y M04 se vuelve a generar con `Meatgrinder/Maps/Build M02 Castle`, `Build M03 Ruins` y `Build M04 Void`. M01 solo sustituye `M01_Expansion`. `Meatgrinder/Maps/Create M01 Shell` sigue negándose a recrear la escena.

Conteos aproximados de piezas `PB_` tras la generación del 10 de noviembre: M01 ~132, M03 ~132, M04 ~91. M02 se regenera con el recinto ampliado; el humo exige al menos 350 mallas. Incluyen escalones, columnas (basa, fuste y capitel) y arcos de dovelas giradas.

## Cifras de ítems — balance provisional aprobado

Los assets de `Assets/_Project/02_ScriptableObjects/Pickups/` usan los valores que ya tenía el código, no cifras del GDD:

| Asset | Tipo | Valor | Pendiente |
|---|---|---|---|
| `Health_Small` | Salud | 25 | Configurable para balance |
| `Armor_Small` | Armadura | 25; máximo 100 y absorción 50 % | Configurable para balance |
| `Quad` | Quad Damage | duración 30, efecto ×4 | Configurable para balance |
| `Berserk` | Berserk | duración 20, efecto ×2 | Configurable para balance |
| `Rocket_Ammo` | Arma / munición | 4 cohetes | Configurable para balance |

También se generan `Invulnerability` (10 s), `Haste` (20 s, ×1,3), `Regeneration` (10 s, 10 HP/s) y `Megahealth` (+100 hasta 200, demora 5 s, decaimiento 5 HP/s). Los poderes reaparecen a 60 s; salud, armadura y munición a 20 s.

Recoger el mismo power-up reinicia el efecto. No suma duraciones. Eso es la regla provisional de D-11.

El respawn del jugador usa `PlayerLife`: 4 s, vida 100, sin armadura/poderes y solo melee, con protección de 1 s que se cancela al atacar. Se evita el punto de muerte mediante marcadores seguros; sigue siendo autoridad local y no hay gestor Fusion. La caída al vacío usa daño `DamageType.Void`, después de una gracia de 1 s, sin teletransporte gratuito.

## M01 — The Slaughterhouse

Escena: `Assets/_Project/00_Scenes/Maps/M01/M01_TheSlaughterhouse.unity`.

Se conservan `PB_Floor`, `PB_Step`, `PB_Wall`, `PB_Ramp` y `PB_Ramp (1)`. El pad original sigue cerrado al norte por `PB_Wall`. La ampliación vive bajo `M01_Expansion`.

Alrededor del pad, un complejo industrial de unos 150 × 130 m:

- Delantales este y oeste que salen del pad sin tapar la rampa.
- Muelles sur (`PB_SouthDock`) y un labio que los une al borde sur del pad.
- Nave de proceso al oeste, con prensas, tuberías y un foso a −1,2 m (`PB_WestPit`) unido por escaleras de 0,3 m.
- Cámaras frías al este, con muros partidos que dejan puertas.
- Sala de matanza al norte (`PB_KillFloor`, 80 × 40 m), columnas, pantallas que cortan la vista y una cubierta de grúa a 6 m.
- Pasarelas a 3,6 m por los dos flancos, unidas a un espinazo. La subida a la grúa sale de ese espinazo.

Luz direccional cálida con sombra suave, niebla ocre y cuatro luces puntuales (sala, frío, nave, muelles).

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Muelles sur | Fuera de la sala de matanza |
| `Spawn_Monster_A` / `B` | Fondo de `PB_KillFloor` | Detrás de `PB_ScreenWest` / `PB_ScreenEast` |
| `Pickup_Health_West` | Nave oeste | Ruta lateral |
| `Pickup_Berserk_West` | Foso oeste, y ≈ −0,75 | Hay que bajar |
| `Pickup_Rockets_East` | Cámaras frías | La otra ruta |
| `Pickup_Quad_Arena` | Centro de la sala de matanza | Disputable y a la vista |
| `Pickup_Armor_Catwalk` | Cubierta de la grúa, y ≈ 6,45 | Hay que subir |

## M02 — Castle of the Damned

Escena: `Assets/_Project/00_Scenes/Maps/M02/M02_CastleOfTheDamned.unity`.

Recinto de unos 540 × 500 m. El patio interior (`PB_Bailey`) sigue siendo 200 × 200 m y se abre a los recintos norte, este, oeste y sur. Sala norte (`PB_GreatHall`) con nártex, columnas, arbotantes, galerías laterales a 3,6 m y ábside. Capilla este elevada 0,6 m, con desván a 4,2 m. Cripta sur a −2,4 m, unida al patio por el arco sur y al barbacán por un cuello a ras de suelo. Una torre gigante por cuadrante (`PB_TowerNW`, `NE`, `SW`, `SE`), desplazada al perímetro: planta de 24 m, muros de 16 m y pilares de esquina de 22 m, cada una con un arco monumental. El adarve (`PB_Battlement`) une las torres del norte a 12 m. El torreón (`PB_Keep`) se une a la torre suroeste por un puente alto. Hay cuatro atalayas, seis torres en ruina, adarves este y oeste a 6 m, claustro a 3,6 m, cubierta norte a 4,2 m y un puente roto entre pilares a 5,4 m. Cortinas interiores rotas, muralla exterior con tres puertas de arco, arcadas y columnas caídas.

Luz de luna baja, niebla oscura y luces cálidas de antorcha en patio, sala, capilla y cripta.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Patio sur, fuera de las torres gigantes | `PB_HallScreen` corta la vista al monstruo A |
| `Spawn_Monster_A` / `B` | Sala norte | Detrás del biombo |
| `Pickup_Quad_Court` | Patio, desplazado del centro | Disputable |
| `Pickup_Armor_Bridge` | Adarve norte entre las torres gigantes, y ≈ 12,45 | Control de altura |
| `Pickup_Berserk_Hall` | Fondo de la sala | Hay que cruzar el nártex |
| `Pickup_Health_South` | Patio sur | Ruta exterior |
| `Pickup_Rockets_Yard` | Capilla | La altura de 0,6 m |

No hay puerta de recorrido. F5-06 sigue sin empezar.

## M03 — Ruins of the Forgotten

Escena: `Assets/_Project/00_Scenes/Maps/M03/M03_RuinsOfTheForgotten.unity`.

Plaza anillada de unos 108 m, sin cubierta general. El centro es un cuenco a −1,8 m con escaleras al norte y al sur. El templo está a 3,6 m, con columnas de unos 12 m, muros rotos y un dintel. La terraza oeste está a 2,4 m y la este, más corta y distinta, a 3,3 m. Un puente estrecho cruza el templo a 7,2 m, con dos subidas. Hay una columna caída, un pórtico partido y escombro.

Sol alto, niebla de polvo y luces de relleno en el cuenco y el templo.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` | Terraza oeste | `PB_TempleFrontL` corta la vista al monstruo A |
| `Spawn_Human_B` | Terraza este | Lado opuesto y más alto |
| `Spawn_Monster_A` / `B` | Suelo del templo | Detrás del muro frontal |
| `Pickup_Health_Bowl` | Fondo del cuenco | Hay que bajar |
| `Pickup_Armor_Temple` | Templo | Altura media |
| `Pickup_Quad_Bridge` | Puente del templo, y ≈ 7,65 | El punto alto disputable |
| `Pickup_Berserk_West` | Terraza oeste | Control de esa altura |
| `Pickup_Rockets_East` | Terraza este | La otra altura |

## M04 — The Longest Void

Escena: `Assets/_Project/00_Scenes/Maps/M04/M04_TheLongestVoid.unity`.

No hay suelo continuo ni un objeto `PB_Ground`. El cielo de la escena está vacío y la cámara de esta instancia usa color sólido oscuro. La referencia es la filosofía de Q3DM17, no una copia.

Plataforma central `PB_Core` a y = 6 (30 × 30 m). Isla norte y sur a la misma altura, unidas por puentes de 4,6 m que pisan las plataformas. Isla oeste a y = 3, con escaleras de vuelta al núcleo. Isla este y satélite noreste a y = 10,2. Corona norte a y = 10,5. Espolón bajo al noroeste a y = 1,5. Mátiles, una cercha y barandillas en los puentes. Caer fuera de esas piezas es el vacío. No se añadió teletransporte: `PlayerLife` reaparece en `Spawn_Human_A` con las reglas ya existentes.

Luz fría, niebla casi negra y cuatro luces puntuales cian y ámbar.

| Marcador | Lugar | Intención |
|---|---|---|
| `Spawn_Human_A` / `B` | Plataforma central | `PB_CrownBlind` corta la vista al monstruo A |
| `Spawn_Monster_A` | Corona norte, y ≈ 10,6 | Altura, detrás de ese muro |
| `Spawn_Monster_B` | Isla este, y ≈ 10,3 | La otra altura |
| `Pickup_Health_Core` | Núcleo, fuera del eje de salida | Cruce |
| `Pickup_Quad_East` | Isla este | Hay que subir |
| `Pickup_Berserk_West` | Isla oeste | La ruta baja |
| `Pickup_Armor_High` | Corona norte | El punto alto |
| `Pickup_Rockets_South` | Isla sur | Otra salida del centro |

## Prueba de esta pasada

`Documentation/test-results/maps-playmode.xml`, 10 de noviembre de 2026, PlayMode `MapLayoutPlayModeTests`, 4/4:

- Cada escena abre.
- Hay mallas ProBuilder con collider y el material de bloqueo, sin shader de error. El mínimo exigido es 100 / 180 / 90 / 60 mallas.
- M01 conserva `PB_Ramp` y tiene `PB_KillFloor`.
- M02 tiene `PB_GreatHall`. M03 tiene `PB_TempleFloor`. M04 tiene `PB_Core` y no tiene `PB_Ground`.
- El jugador, andando unos 6 m hacia delante desde su posición de escena, no cae por debajo del suelo de salida.
- Un rayo de `Spawn_Human_A` a `Spawn_Monster_A` choca con geometría.

Eso no recorre escaleras, pasarelas, foso, cripta ni puentes. No recoge ítems y no prueba dos clientes. Por eso las tareas F5-02, F5-03, F5-04, F5-05, F5-09, F5-10 y F5-11 quedan en `PARTIALLY_IMPLEMENTED`.

La pasada local adicional valida el flujo de movimiento y las escenas con PlayMode completo 12/12 (`local-integration-playmode-final.xml`), incluida la subida de la rampa M01. Eso no valida loot, vacío, recorrido completo, puertas/llaves ni dos clientes. Tampoco es un pase de arte de producción: un solo shader de color de vértice, sin kit modular texturizado.

## Cómo seguir editando

Abrir la escena. `Tools → ProBuilder → ProBuilder Window`. Las piezas cuelgan de `M01_Expansion`, `M02_Castle`, `M03_Ruins` o `M04_Void`. Duplicar o extruir esas mallas. Asignar `M01_Blockout` si una pieza nueva sale sin material. En sólidos, `BoxCollider` del tamaño de la pieza. No lanzar `Meatgrinder/Maps/Create M01 Shell`. Greybox no forma parte de estos mapas.

Volver a generar M02–M04 sustituye toda la escena. Volver a generar M01 solo sustituye `M01_Expansion` y vuelve a atar el `spawnPoint` del jugador que ya estaba en la escena.
