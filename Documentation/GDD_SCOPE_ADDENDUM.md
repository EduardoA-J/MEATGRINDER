# Adenda de alcance — juego completo

Fecha: 9 de noviembre de 2026.
Autoridad: decisión de producto de David.
Fuente de diseño: `MEATGRINDER_GDD_TDD.docx` (v1.0). Esta adenda **no borra** el GDD: redefine cómo se lee su calendario y sus etiquetas de fase.

## Política oficial

Todas las funcionalidades definidas en los documentos oficiales de MEATGRINDER pertenecen al alcance del videojuego completo y deben desarrollarse como implementaciones reales, integradas y utilizables en partidas.

No existen categorías que permitan descartar una funcionalidad solo porque el GDD o un documento intermedio la llamara Demo, Futuro, Prototipo, MVP, Después de la demo o Prueba inicial.

Las etiquetas temporales del GDD (semanas 1–8, demo, futuro) pasan a ser **orden de priorización**, no exclusiones del producto.

## Tres ejes que no se mezclan

| Eje | Valores |
|---|---|
| Alcance del producto | `REQUIRED` para todo lo documentado en GDD/TDD/requisitos/propuestas aceptadas |
| Estado de implementación | `NOT_STARTED`, `IN_PROGRESS`, `PARTIALLY_IMPLEMENTED`, `IMPLEMENTED`, `INTEGRATED`, `VALIDATED`, `BLOCKED` |
| Prioridad de desarrollo | `P0` núcleo jugable · `P1` primer ciclo jugable · `P2` sistemas del juego · `P3` contenido y pulido · `P4` optimización y ampliaciones |

## Lectura del GDD a partir de ahora

| Antes en el GDD / docs | Ahora |
|---|---|
| Demo de 8 semanas | Fase de entrega temprana; el alcance sigue completo |
| Fuera de la demo / Futuro | REQUIRED, prioridad según dependencias |
| Ranked Versus visión futura | REQUIRED (epic E11 / R-01…R-04) |
| Ametralladora, superescopeta, rayo | REQUIRED (P-05) |
| Invulnerabilidad, berserk, regeneración | REQUIRED (ítems / combate) |
| Melee de monstruo | REQUIRED; además melee humano si Berserk lo exige |
| M02 / M03 / M04 | REQUIRED; orden M01 → M02 → M03 → M04 |
| 2v2 vs 4v4 | Ambos REQUIRED; 2v2 primero por dependencias de red, 4v4 no se descarta |
| Sistema de estilo ULTRAKILL | REQUIRED (P-07); no se copia UI ajena, se implementa el medidor propio |

## Qué no cambia

- Diseño mecánico del GDD (pilares, asimetría, Host Mode, autoridad).
- Valores numéricos no definidos: siguen `PENDIENTE` en ScriptableObjects y en `DECISIONS.md`.
- Stack: Unity `6000.5.6f1`, URP 17.5.0, Fusion 2 Host Mode.

## Documentos que aplican esta adenda

`AGENTS.md`, `.cursor/rules/`, `GDD_REQUIREMENTS.md`, `MASTER_CHECKLIST.md`, `PROJECT_CONTEXT.md`, `PROGRESS.md`, `DEPENDENCY_GRAPH.md`, `DECISIONS.md`, `RISKS.md`, `FULL_GAME_DEVELOPMENT_PLAN.md`, `IMPLEMENTATION_STATUS_AUDIT.md`.
