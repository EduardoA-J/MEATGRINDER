# Plan de desarrollo — MEATGRINDER juego completo

Fecha: 9 de noviembre de 2026.
Complementa: `GDD_SCOPE_ADDENDUM.md`, `MASTER_CHECKLIST.md`, `IMPLEMENTATION_STATUS_AUDIT.md`.

## 1. Alcance completo

Todo lo documentado en GDD/TDD y documentos oficiales es `REQUIRED`: movimiento, combate, melee, arsenal (incluidas armas ampliadas), pickups y power-ups, monstruos jugables, mapas M01–M04, Boot/menús/sesiones, Host Mode con gameplay real, modos versus, ranked, HUD, audio, VFX, gore, marcador, estilo, bots de desconexión y el resto de propuestas del checklist.

## 2. Principios

1. Sistemas definitivos, no demos descartables.
2. Greybox = laboratorio; el producto es Boot + partida + mapas.
3. Autoridad Host Mode desde el diseño de estado competitivo.
4. Una resolución de daño; UI solo lee.
5. Prioridad ordena; no borra.
6. Cifras desconocidas → `PENDIENTE` + decisión abierta.
7. Terminado = integrado + prueba (y dos clientes si es net).

## 3. Prioridades P0–P4

| P | Objetivo |
|---|---|
| P0 | Compilar limpio; combate usable (escopeta+cohete); daño/salud/armadura; pickups con efecto real; melee base; respawn local |
| P1 | Boot, menú sesión, jugador en red, sync núcleo, HUD cableado |
| P2 | Monstruos, M01, power-ups restantes, modos, marcador |
| P3 | M02–M04, arsenal ampliado, audio/arte, UI completa, estilo |
| P4 | Ranked, 4v4, optimización, propuestas restantes |

## 4. Dependencias críticas

```text
IDamageModifiers / asmdefs limpios
  → cablear Rocket + spawner
  → Pickups → PowerUpManager → DamageResolver / Movement / Melee
  → Respawn local
  → Boot + menú
  → NetworkPlayer (LocalPlayer + NetworkObject)
  → sync movimiento/salud/arma/daño
  → M01 + spawns + layout
  → Monstruos + melee monstruo
  → modos / marcador
  → Ranked (backend + reglas)
```

## 5. Plan inmediato

### A. Bloqueantes inmediatos (P0)

| # | Tarea | Criterio | Prueba |
|---|---|---|---|
| A1 | Cerrar ciclo Combat↔Pickups (`IDamageModifiers`) | Compila sin error de asmdef | Compilación Editor |
| A2 | No afirmar power-ups DONE en docs | Docs alineados | Revisión |
| A3 | Inventario local sin commit | Decisión: commit o conservar | `git status` |

### B. Primer ciclo de implementación (P0)

| # | Objetivo | Sistemas | Criterio de aceptación |
|---|---|---|---|
| B1 | Lanzacohetes jugable | Weapons, Players, Presentation | Tecla 2 dispara; munición propia; explosión daña; cobertura |
| B2 | Pickup salud + armadura | Pickups, Combat | Trigger aplica; respawn de ítem |
| B3 | Quad Damage real | Pickups, DamageResolver | Durante el efecto el daño de arma ×N medible en test |
| B4 | Melee humano mínimo | Combat, Players | Golpe aplica `DamageType.Melee` vía resolver |
| B5 | Berserk × melee | Pickups | Con berserk el melee escala; sin melee no cerrar |
| B6 | Respawn local | Players, Combat | Tras `Died`, reaparece con reglas D-08 provisionales |

### C. Primer ciclo jugable real (P1)

Boot → menú crear/unir → spawn humano → mover/disparar/recoger → morir/respawn → salir limpio. Dos clientes cuando el NetworkPlayer exista.

### D. M01 (P2)

Blockout, spawns, layout armas/ítems, puertas, verticalidad. Contenido en el nivel real, no demos Greybox.

### E. Resto del GDD (P2–P4)

Monstruos, modos, M02–M04, arsenal ampliado, audio/arte, ranked, estilo, 4v4.

## 6. Paralelismo entre agentes

| Paralelo seguro | Requiere coordinación |
|---|---|
| Audio clips / materiales placeholder | `DamageResolver` + Pickups + asmdefs |
| Documentación de fichas de monstruo | `LocalPlayer` prefab / loadout |
| UI menú (sin tocar combate) | Movement + Haste |
| Blockout M01 en escena nueva | Fusion spawn + PlayerCombat net |

Evitar dos agentes en el mismo prefab o en Combat+Pickups a la vez.

## 7. Validación

- EditMode/PlayMode por sistema.
- Multijugador: dos procesos, escena de producto.
- No VALIDATED sin evidencia en `PROGRESS.md` + XML o log.

## 8. Riesgos de integración

- Docs `ARSENAL_*` optimistas → priorización falsa.
- D-07/D-10/D-11 sin cifras → provisional marcado.
- Fusion solo probe → no construir ranked encima.
- Agentes paralelos en prefabs → conflictos.
