# Riesgos

Fecha base: 2 de noviembre de 2026. Actualización: 9 de noviembre de 2026 (alcance juego completo).

| ID | Riesgo | Impacto | Estado | Mitigación |
|---|---|---|---|---|
| K-01 | Alcance completo con mucho núcleo aún no integrado. | Retraso si se intenta avanzar en Ranked/arte antes del combate/red. | Abierto | Prioridad P0–P4 en `FULL_GAME_DEVELOPMENT_PLAN.md`. No borrar alcance. |
| K-02 | El editor 6.5.6 ya abrió el proyecto. | — | Cerrado 3 nov 2026 | F0-02 / F0-03 VALIDATED. |
| K-03 | Unity CLI no está en el PATH. | El agente no conduce el Editor fácilmente. | Abierto | Instalar CLI cuando David lo apruebe. |
| K-04 | Fusion 2.1.4 compiló; matriz oficial sin 6000.5.6f1. | Riesgo en ejecución/weaver. | Abierto | T-22. Probar gameplay net con dos clientes reales. |
| K-05 | Movimiento Quake difícil de sincronizar. | Combate injusto. | Abierto | Diseñar `FixedUpdateNetwork` desde MovementSimulator. D-19. |
| K-06 | Contradicción 2v2/4v4 y Campaign/Continuous (D-04, D-05). | Se construye el modo equivocado primero. | Abierto | Ambos REQUIRED; orden por dependencias tras decisión de David. |
| K-07 | Quad, armadura, respawn, dash sin números (D-06…D-11). | Constantes inventadas. | Abierto | `PENDIENTE` + decisión; no VALIDATED hasta cifras. |
| K-08 | Asimetría humanos / monstruos. | Un equipo gana siempre. | Abierto | Probar 2v2; stats en MonsterDefinition. |
| K-09 | Gore caro. | Se gasta el ciclo en VFX. | Abierto | Pool, topes; retrasar pulido antes que red. |
| K-10 | Ranked prematuro sobre núcleo incompleto. | Resultados manipulables / trabajo tirado. | Abierto | Ranked REQUIRED (P4); no empezar R-* hasta núcleo + autoridad. |
| K-11 | Población baja. | Colas vacías. | Aceptado temprano | Salas privadas primero; matchmaking en R-03. |
| K-12 | Licencias de assets. | Build no distribuible. | Abierto | F0-11 antes de importar packs. |
| K-13 | Desconexiones. | Partidas corruptas. | Abierto | Continuar y marcar (T-12); bots P-02 después. |
| K-14 | Skills globales que empujan otro stack. | Red equivocada. | Mitigado en repo | Regla `04`. |
| K-15 | `game-team-orchestrator` invoca agentes inexistentes. | Planes no ejecutables. | Abierto | Solo handoff de referencia. |
| K-16 | Skills que editan sin permiso. | Diff fuera de alcance. | Abierto | Regla de flujo: solo tarea aprobada. |
| K-17 | `com.unity.multiplayer.center` junto a Fusion. | Otro stack. | Abierto | Red = Fusion Host Mode. |
| K-18 | Input de plantilla con Crouch/Sprint (D-17). | Controlador genérico. | Abierto | F0-09. |
| K-19 | Calidad Mobile en proyecto PC. | Optimización equivocada. | Bajo | Ignorar Mobile. |
| K-20 | Código diverge del GDD en silencio. | Dos verdades. | Abierto | Parar y documentar. |
| K-21 | Docs de arsenal (`ARSENAL_*`) sobreestiman integración. | Priorización falsa. | Abierto | `IMPLEMENTATION_STATUS_AUDIT.md` manda sobre informes optimistas. |
| K-22 | Agentes paralelos en Combat/Pickups/prefabs. | Conflictos y ciclos asmdef. | Abierto | Coordinar; un agente por prefab crítico. |
| K-23 | Tratar Greybox como producto. | Retraso de Boot/M01. | Abierto | Regla Greybox en AGENTS y `09`/`10`. |
| K-24 | Ciclo de ensamblados Combat↔Pickups. | Compilación rota. | Mitigación local | `IDamageModifiers` (cambios locales sin commit el 9 nov). |

## Limitaciones de auditoría

- El estado de implementación del 9 nov se contrastó con HEAD `992bb92` + cambios locales visibles.
- No se reejecutó la suite EditMode/PlayMode en esa fecha.
- El texto del GDD se extrajo del XML de Word; no se inventaron celdas vacías.
