# Auditoría de repositorios de referencia

## Aplicación de referencias — 22 noviembre 2026

Se revisaron los tres clones locales (`salvations-descent`, `SourceEngineMovementUnity`, `doom`): separación de inventario/vista, cambio de arma, melee por fases, movimiento por parámetros, pickups, puertas y flujo de pantallas. Se reprodujeron esos comportamientos con código propio adaptado a `DamageResolver`, Input System y los ensamblados existentes. No se copiaron sus scripts, WAD, modelos, sonidos, shaders ni plugins comerciales. No se encontraron licencias de proyecto que autoricen copiar los tres juegos; las licencias de subdependencias no se extienden al proyecto completo.

Inventario de los cuatro FBX aportados en `models/`, importados sin modificar originales a `Assets/_Project/05_ArtModels/Imported/`:

- `20261110234906_b0c92559`: 116.370 vértices, modelo humanoide; integrado en `TrainingTarget_0`.
- `20261111001453_9a59cb6b`: 93.174 vértices, figura con túnica; integrado en `TrainingTarget_1`.
- `20261111002122_967857ff`: 115.869 vértices, humanoide con armadura; integrado en `TrainingTarget_2`.
- `20261110235955_ece0d8da`: 1.975.614 vértices; importado e inspeccionado, fuera de escenas por coste. No hay presupuesto de producción aprobado para esa densidad.

Los cuatro FBX tienen un renderer estático, sin SkinnedMeshRenderer, Animator ni clips importados (comprobado con Unity). Los tres targets usan `ModelSocket`, escala normalizada a dos metros, collider y salud propios; la caída de muerte es procedural. No constituyen monstruos jugables ni un rig de animación terminado. Métricas y previews: `test-results/product/models.txt` y PNG por modelo. Pendientes LOD, rig/anatomía/hitboxes de producto y mediciones de GPU.

Registro de procedencia de esta pasada: FBX = aportados por David (licencia de distribución no incluida, pendiente registrar fuente); geometría de armas, materiales/texturas, seis iconos y clips sintéticos = generados específicamente para MEATGRINDER; uGUI/URP/ProBuilder = paquetes ya presentes. No se añadieron paquetes ni recursos de pago. F0-11 permanece parcial hasta completar las licencias de todos los recursos externos.

Fecha: 4 de noviembre de 2026.
Clones, fuera del proyecto Unity: `F:\MAPAUNI\references\salvations-descent` y `F:\MAPAUNI\references\SourceEngineMovementUnity`. No se copió ningún archivo al repositorio de MEATGRINDER.

## Salvation's Descent

`laspencer91/salvations-descent`. README: prototipo FPS en Unity 2022.3.0f1. Movimiento tipo Quake, armas intercambiables, IA por estados, bob de cámara, visualizador de dispersión y pantalla de resultados. El arma vive bajo un Weapon Manager: al cambiar, se desactiva el modelo actual, se activa el otro y una animación corre en OnEnable y OnDisable. El disparo de la escopeta es un raycast propio (`ShotgunRaycastWeapon`).

Ideas que encajan aquí, hechas con código nuevo:

- El modelo de la escopeta es un hijo de la cámara, solo local.
- El retroceso visual y el fogonazo no mueven el origen del rayo.
- Equipar y recargar se leen como pose, sin un Animator todavía.
- La dispersión sigue saliendo de `HitscanSpread`, no de un gizmo de otro proyecto.

No se reutiliza nada de ese repo. No hay archivo `LICENSE` en la raíz ni en el README. Sin licencia explícita no hay permiso de copia. Además trae plugins que no entran en este proyecto: Aura 2, Sirenix (Odin) y QFSW. Importarlos metería dependencias comerciales y un stack de iluminación que el GDD no pide.

## SourceEngineMovementUnity

`rogerli2020/SourceEngineMovementUnity`. README: extensión mínima del Character Controller para bunny hop, surf y variables en un ScriptableObject `MoveVars`. Scripts en `Scripts/PlayerMovement` y `PlayerMovementInput`. Escenas, materiales y prefabs de prueba propios.

Ese patrón de datos ya está en `MovementTuning`. El readout de velocidad ya está en `MovementProbeReadout`. No se tocó `MovementSimulator` ni `PlayerMovement`.

No hay archivo `LICENSE`. La API de GitHub ya había respondido 404 para la licencia. No se copia código, materiales ni escenas.

## Qué se implementa aquí

Viewmodel de primitivas, HUD uGUI, retícula, marcas de impacto con un pool de 16 y el vestido de Greybox. Todo es código y geometría de este repositorio. Materiales duplicados del `Greybox.mat` de URP que ya estaba en el proyecto. No hay modelos, texturas ni audio de los clones.

## Actualización — 9 de noviembre de 2026

Clonado `https://github.com/paul8483/doom` en `F:\MAPAUNI\references\doom` (shallow). Fuera de `grinder/`. No es submódulo. El motor de ese repo no tiene licencia de proyecto: no se copia código ni el WAD. Evaluación y plan: `TOOLS_AND_REFERENCES.md`.

## Hallazgo del 5 de noviembre de 2026

La frase de arriba que dice que no se tocó `MovementSimulator` describe la pasada de presentación del 4 de noviembre. El 5 de noviembre el wish de aire dejó de ser la carrera y `AirControl` pasó a 0. El algoritmo sigue siendo propio. El repositorio de Source recorta el módulo cuando ya se llegó al máximo y eso impide el strafe. No se copió ese recorte ni ningún archivo. Salvation's Descent aportó la idea de bloquear el disparo mientras el arma entra. El tiempo vive en `WeaponDefinition.DrawDuration`. No se importó su animator ni Odin. El detalle está en `MOVEMENT_WEAPON_REFERENCE_ANALYSIS.md`.
