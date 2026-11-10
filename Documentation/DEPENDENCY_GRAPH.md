# Grafo de dependencias — juego completo

Fecha: 9 de noviembre de 2026.
Detalle por tarea: [MASTER_CHECKLIST.md](MASTER_CHECKLIST.md). Plan: [FULL_GAME_DEVELOPMENT_PLAN.md](FULL_GAME_DEVELOPMENT_PLAN.md).

Las flechas significan «hace falta antes». Ranked y ampliaciones están **en el grafo** con prioridad baja; no están fuera del producto.

## Cadena de sistemas

```mermaid
flowchart TD
  audit[F0 editor y estructura]
  fusionSdk[Fusion SDK Host Mode]
  input[Input]
  greybox[Greybox laboratorio]
  move[Movimiento]
  combat[Combate escopeta cohete melee]
  pickups[Pickups y powerups]
  boot[Boot y menus]
  netPlayer[NetworkPlayer gameplay]
  sync[Sync movimiento dano pickups]
  monsters[Monstruos]
  m01[M01]
  modes[Modos versus]
  mapsRest[M02 M03 M04]
  ranked[Ranked]
  qa[QA y build]

  audit --> input
  audit --> greybox
  audit --> fusionSdk
  input --> move
  greybox --> move
  move --> combat
  combat --> pickups
  pickups --> combat
  fusionSdk --> boot
  boot --> netPlayer
  move --> sync
  combat --> sync
  pickups --> sync
  netPlayer --> sync
  sync --> monsters
  greybox --> m01
  sync --> m01
  m01 --> modes
  monsters --> modes
  modes --> mapsRest
  modes --> qa
  mapsRest --> qa
  qa --> ranked
```

## Camino crítico P0 → P1

1. Asmdefs limpios (`IDamageModifiers` / sin ciclo Combat↔Pickups).
2. F2-08 lanzacohetes cableado al jugador.
3. F6-01/02/03 pickups con efecto real; Quad → DamageResolver.
4. F2-13 melee; F2-14 Berserk.
5. F2-04 respawn local.
6. F0-08 Boot → F7-01/02 menú → F3-01 bootstrap producto.
7. F3-03 NetworkPlayer → F3-04/06/07 sync → F3-12 dos clientes gameplay.
8. F5-02 M01 blockout en paralelo seguro (escena nueva).

## Ensamblados (objetivo tras fix local)

```mermaid
flowchart LR
  movement[Meatgrinder.Movement]
  combat[Meatgrinder.Combat]
  weapons[Meatgrinder.Weapons]
  pickups[Meatgrinder.Pickups]
  players[Meatgrinder.Players]
  presentation[Meatgrinder.Presentation]
  movement --> players
  combat --> weapons
  combat --> players
  weapons --> players
  pickups --> combat
  pickups --> players
  combat --> presentation
  weapons --> presentation
  players --> presentation
  pickups --> presentation
```

`Combat` no debe referenciar `Pickups`. Los modificadores de daño se inyectan vía `IDamageModifiers`.

## Paralelo seguro vs coordinación

| Paralelo | Coordinar |
|---|---|
| Fichas monstruo (docs), audio placeholder, UI menú sin combate, blockout M01 en escena nueva | DamageResolver, PowerUpManager, LocalPlayer prefab, asmdefs, Fusion spawn |

## Ranked y ampliaciones

E11 (R-01…R-04) cuelga del núcleo jugable + autoridad + decisión de backend. Sigue REQUIRED (P4).
E12 (P-01…P-07) REQUIRED con P3–P4. No bloquean P0.
M02–M04 y 4v4: REQUIRED; orden tras M01 y matriz de red.
