# Flujo de agentes

Fecha: 2 de noviembre de 2026.

## Qué existe de verdad

| Pieza | Estado |
|---|---|
| Agente de Cursor en este repositorio | Activo. Lee `AGENTS.md` y `.cursor/rules/`. |
| Skills de Unity del proyecto | Archivos en `.cursor/skills/`. Cursor puede cargarlas por su descripción. No arrancan el Editor por sí solas. |
| Codex | Skills de usuario en `C:\Users\pc\.agents\skills`. No hay `AGENTS.md` previo ni configuración de Codex dentro de `grinder/`. Si David abre el mismo repo en Codex, `AGENTS.md` es la instrucción compartida. |
| Game Team Orchestrator | Una skill. No es un equipo. Sus agentes (`game-designer`, `game-technical-director`, `game-unity-specialist`, `game-qa-lead`, y el resto) no están instalados. |
| Unity Agent Plugin / MCP de Unity | No detectado. |
| Photon | No instalado. |

Los perfiles de abajo son roles de trabajo para repartir tareas. Una misma persona, o el agente de Cursor, puede llevar varios roles en serie. No son procesos autónomos lanzados a la vez.

## Perfiles

### Game Director

- Responsabilidades: pilares, alcance de la demo, contradicciones D-04, D-05, D-10, D-12, D-13 y D-14. Acepta o rechaza recortes.
- Consulta: `01-game-design.mdc`, GDD, `DECISIONS.md`.
- Herramientas: lectura del GDD. No necesita Editor.
- Puede modificar: `Documentation/DECISIONS.md` cuando David confirme, y el estado de tareas de diseño.
- Depende de: nadie para documentar. El resto espera sus cierres.
- Entrega: decisión escrita con ID, sin números inventados.

### Technical Architect

- Responsabilidades: Unity 6.5.6, carpetas, ensamblados, Host Mode ya aprobado, límites de autoridad.
- Consulta: `02`, `04`, `unity-package-management`, `unity-cli` si el binario existe.
- Herramientas: manifiesto, ProjectSettings, documentación de Photon.
- Puede modificar: `Packages/manifest.json` solo con tarea de instalación aprobada; `Assets/_Project` de estructura; asmdefs. No toca el diseño de armas.
- Depende de: T-21 y T-22 para Fusion. De F0-02 para cualquier escena.
- Entrega: proyecto que abre en 6.5.6, sin un segundo stack de red, con la decisión de modo escrita.

### Gameplay Programmer

- Responsabilidades: movimiento, cámara, input, daño local, armas, salud, pickups en simulación local.
- Consulta: `03`, `07`, `physics-3d-collision` si hay un fallo de colisión.
- Herramientas: Editor 6.5.6, Test Framework.
- Puede modificar: `01_Scripts/Movement`, `Combat`, `Weapons`, `Pickups` y sus ScriptableObjects. No escribe autoridad de red dentro del arma.
- Depende de: F0-07 y F1-01. La armadura espera D-07. El dash espera D-06.
- Entrega: prueba repetible de la tarea y parámetros fuera del código.

### Multiplayer Engineer

- Responsabilidades: sesión Fusion, spawn, predicción, daño autoritativo, proyectiles, desconexión.
- Consulta: `04`, apéndice B del GDD. Las skills de UGS se borraron del repositorio.
- Herramientas: Fusion 2.1.4 en `Assets/Photon`, dos clientes cuando exista App ID.
- Puede modificar: `01_Scripts/Networking` y los puntos de extensión ya previstos en salud y armas. No cambia la fórmula de diseño.
- Depende de: T-21, T-22, el App ID, F1-10 para sincronizar movimiento, F2-01 para daño.
- Entrega: dos clientes, un solo registro de daño, salida de sesión limpia.

### UI/UX Developer

- Responsabilidades: menús, HUD, ajustes, feed, resultados. La UI lee eventos.
- Consulta: `06`, skill `ui`, skill `ui-ugui`.
- Herramientas: uGUI ya instalado.
- Puede modificar: `01_Scripts/UI` y `03_Prefabs/UI`. No modifica `HealthComponent` para «arreglar» el HUD.
- Depende de: D-09 y de los eventos que gameplay y red expongan.
- Entrega: cada dato del HUD coincide con el estado autoritativo en los dos clientes.

### Technical Artist

- Responsabilidades: materiales URP, volumes, presupuesto de luces, pools de VFX, import settings.
- Consulta: `05`, `urp-postprocessing`, `validate-urp-render-graph-renderer-feature` solo si hay un feature.
- Herramientas: URP 17.5. `asset-transformer-toolkit` no, mientras falte el paquete.
- Puede modificar: `05_ArtModels/Materials`, profiles de Volume, prefabs de VFX. No cambia colisionadores de juego para que el material encaje.
- Depende de: F5-02 para vestir M01. El MVP puede ir en gris.
- Entrega: el enemigo se distingue y el efecto tiene tope.

### 3D Artist / Animator

- Responsabilidades: modelos, rigs, clips, Animator de presentación.
- Consulta: `05`. No hay skill de rigging instalada; el trabajo es de DCC y de importación Unity.
- Herramientas: el DCC que use David. Unity para importar cuando el editor exista.
- Puede modificar: `05_ArtModels/Characters`, animators, avatares. No aplica daño en eventos de animación.
- Depende de: F4-02 para el tamaño del hitbox. La malla no define la salud.
- Entrega: prefab con rig, clips mínimos de la tarea y licencia si el modelo es externo.

### QA Engineer

- Responsabilidades: matrices de la sección 21, no cerrar tareas, bugs reproducibles.
- Consulta: `08`, checklist, `game-team-orchestrator` receta 1 solo como orden de prueba (diseño → técnica → casos).
- Herramientas: dos clientes, Test Framework, más adelante Profiler.
- Puede modificar: `Tests/` y el estado de la tarea. No «arregla de paso» el sistema que falla.
- Depende de: criterio de aceptación de la tarea bajo prueba.
- Entrega: resultado observado, pasos, y estado REVIEW o bloqueo. `DONE` solo con la prueba cumplida.

## Flujo de una tarea

1. Leer `AGENTS.md` y la regla del dominio.
2. Tomar un ID del checklist en `READY`.
3. Mirar [DEPENDENCY_GRAPH.md](DEPENDENCY_GRAPH.md). Si una dependencia no está `DONE`, la tarea sigue bloqueada.
4. Leer el código del sistema. Si no existe, la tarea lo crea. Si existe, se amplía.
5. Escribir el plan corto: archivos, datos pendientes, prueba.
6. Implementar solo ese ID.
7. Ejecutar la prueba de la ficha. Guardar el resultado en `PROGRESS.md` sin adornarlo.
8. Revisar el diff frente al alcance.
9. Pasar la tarea a `REVIEW` o dejarla `BLOCKED` con el motivo.
10. Si David cierra una decisión, actualizar `DECISIONS.md` y la regla que la citaba.

## Handoff mínimo

Cuando el trabajo cambia de perfil, una nota en la tarea basta:

- Contexto de lo que quedó hecho.
- Decisiones que ya no se reabren.
- Restricción que el siguiente perfil no puede romper.
- Pregunta abierta.
- Prueba que debe repetir.

El orquestador pide ese documento entre agentes. Aquí el documento es la ficha de la tarea más la entrada de `PROGRESS.md`. No se crea `production/session-state/`: esa ruta pertenece a pipelines que no están instalados.

## Conflictos

| Choque | Quién corta |
|---|---|
| Sensación de juego frente a una implementación más simple | Game Director, dentro de los pilares |
| La feature no sostiene 60 FPS en el PC acordado | Technical Architect recorta la feature |
| El HUD no se lee en combate | UI/UX, sin quitar un dato obligatorio del GDD |
| Fecha frente a alcance | David. La regla de corte del GDD es quitar arte antes que pruebas de red |
| Una skill pide un stack distinto | Se ignora la skill. Manda T-04 |
