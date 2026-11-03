# Auditoría técnica

Fecha: 2 de noviembre de 2026.
Alcance: inspección de disco. No se modificó el proyecto Unity, no se instaló ni se quitó ningún paquete, skill o plugin, y no se ejecutó el Editor.

## Entorno

| Hecho | Valor observado |
|---|---|
| Editor declarado | `6000.5.6f1` (`0e0577a1a2ac`) en `grinder/ProjectSettings/ProjectVersion.txt` |
| Editor que abrió el proyecto | `F:\Unity\Hub\Editor\6000.5.6f1\Editor\Unity.exe`, revisión `0e0577a1a2ac`. Verificado en `Editor.log` el 3 de noviembre de 2026 |
| Otro Hub en Program Files | Sigue teniendo solo `2020.3.48f1` y `2020.3.49f1`. No fue el editor que abrió `grinder/` |
| CLI `unity` | Sigue sin estar en el PATH. El Editor gráfico sí está abierto |
| `grinder/Library` y `grinder/Logs` | Existen. El primer import terminó |
| Compilación | `Tundra build success`. Cero `error CS`. UPM respondió 200. Hay tres warnings de versión mínima, listados en F0-03 |
| Producto | `productName: grinder`, `companyName: DefaultCompany`, `bundleVersion: 0.1.0` |
| Color | Lineal (`m_ActiveColorSpace: 1`) |
| Pipeline | URP `com.unity.render-pipelines.universal` 17.5.0. Asset activo: `PC_RPAsset` (guid `4b83569d67af61e458304325a23e5dfd`). Renderer universal (`m_RendererType: 1`), SRP Batcher activo, sombras de luz principal a 2048 |
| Calidad | Índice actual 1 = PC. Standalone también usa PC. Android e iPhone usan Mobile. vSync 0. MSAA del asset PC: 1 |
| Input | `activeInputHandler: 1` (solo Input System). Paquete 1.20.0. Asset `Assets/InputSystem_Actions.inputactions` con mapa Player: Move, Look, Attack, Interact, Crouch, Jump, Previous, Next, Sprint |
| Scripting | `apiCompatibilityLevel: 6`. El log del editor 6.5.6 carga `BCLExtensions/runtime/netstandard2.1`. `allowUnsafeCode: 0`. Incremental GC activo. Backend explícito solo para Android (IL2CPP = 1) |
| Física | Gravedad PhysX `{0, -9.81, 0}`, solver iterations 6, queries hit triggers |
| Escena en build | Solo `Assets/Scenes/SampleScene.unity` |
| Contenido de SampleScene | Main Camera, Directional Light, Global Volume. `m_NavMeshData` nulo |
| Scripts | `Readme.cs` y `ReadmeEditor.cs` de la plantilla. Cero scripts de juego |
| Prefabs, modelos, animaciones, materiales de juego | No hay |
| Photon / Fusion | No aparece en `manifest.json` ni en `packages-lock.json`. El 3 de noviembre de 2026 se contrastó con la documentación oficial y no se instaló. Ver `DECISIONS.md`, sección «Verificación de Fusion» |
| Git | Rama `main` alineada con `origin/main`. El GDD `MEATGRINDER_GDD_TDD.docx` está versionado. No hay política de ramas escrita |
| Nube Unity | `cloudEnabled: 0`. Hay `cloudProjectId` y `organizationId` en ProjectSettings |

## Paquetes directos del manifiesto

| Paquete | Versión | Relación con el GDD |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.5.0 | Pedido |
| `com.unity.inputsystem` | 1.20.0 | Pedido |
| `com.unity.ugui` | 2.5.0 | Útil para HUD. El framework no está decidido (D-09) |
| `com.unity.test-framework` | 1.7.0 | Útil para las pruebas de la sección 21 |
| `com.unity.timeline` | 1.8.12 | No lo pide el GDD |
| `com.unity.visualscripting` | 1.9.11 | No lo pide el GDD |
| `com.unity.ai.navigation` | 2.0.14 | Los monstruos son jugadores. No hay NavMesh |
| `com.unity.multiplayer.center` | 1.0.1 | Hub de multiplayer de Unity. No es Photon |
| `com.unity.collab-proxy` | 2.13.3 | Versionado de Unity. El repo usa Git |
| `com.unity.ide.rider` / `visualstudio` | 3.0.38 / 2.0.26 | IDEs |
| Módulos `com.unity.modules.*` | 1.0.0 | Plantilla |

Transitivos relevantes: `com.unity.render-pipelines.core` 17.5.0, `com.unity.shadergraph` 17.5.0, `com.unity.burst` 1.8.29, `com.unity.collections` 6.5.0, `com.unity.mathematics` (dependencia de Burst).

El 3 de noviembre de 2026 `upm.log` cerró `project:resolve-packages` con HTTP 200. Fusion no es una entrada de UPM: vive en `Assets/Photon`. La importación añadió `com.unity.nuget.mono-cecil` como dependencia directa en la versión ya resuelta, 1.11.6. URP sigue en 17.5.0.

## Diferencias entre código y GDD

| GDD | Repositorio |
|---|---|
| `Assets/_Project/` con escenas Boot, Menus y Maps | `Assets/Scenes/SampleScene.unity` y `Assets/Settings/` |
| Fusion 2, sesión, autoridad | SDK 2.1.4 build 2500 en `Assets/Photon`. App ID en el asset local, fuera de git. Sin sesión de juego |
| Controlador, armas, salud, monstruos | Ausentes |
| Nombre de producto MEATGRINDER | `grinder` |
| Gravedad de movimiento configurable | Solo la gravedad por defecto de PhysX |
| Input de FPS de arena | Asset de plantilla, con Crouch y Sprint |

## Skills, plugins y herramientas

Criterio: una skill instalada es un paquete de instrucciones. Se considera ejecutable solo si esta sesión puede leerla y si la herramienta que necesita está presente. El 3 de noviembre de 2026 David pidió borrar del repositorio las skills que no aplican a este juego. Esas carpetas ya no están en `.cursor/skills`.

Ubicación de las skills de Unity y del orquestador: `f:\MAPAUNI\MEATGRINDER\.cursor\skills\<nombre>\SKILL.md`.
Skills de producto Cursor: `C:\Users\pc\.cursor\skills-cursor\`.
Skills de Codex: `C:\Users\pc\.agents\skills\`.
Plugins de Cursor en caché: Miro, Trello, Figma y Prisma. En esta sesión hay MCP de Miro, Trello y Figma, más las herramientas nativas de Cursor. No hay MCP de Unity.

### Tabla

| Herramienta | Función | Compatibilidad | Utilidad | Justificación |
|---|---|---|---|---|
| `unity-cli` | Conducir el Editor, builds, tests y el MCP de Unity | Instrucciones de Unity 6. El binario no está instalado y falta `com.unity.pipeline` | ÚTIL | Sirve cuando existan el editor 6.5.6 y el CLI. Hoy no ejecuta nada |
| `unity-package-management` | Añadir y comprobar paquetes UPM | Compatible con el manifiesto actual | ÚTIL | Sirve para paquetes del registro. Fusion ya entró como `.unitypackage` |
| `physics-3d-collision` | Diagnosticar colisiones PhysX 3D | Compatible con Unity 6. Pide editar sin preguntar | ÚTIL | El juego es 3D. La regla de flujo limita las ediciones al alcance aprobado |
| `ui` | Elegir uGUI, UI Toolkit o IMGUI | Compatible | ÚTIL | El GDD no fija framework. Esta skill evita mezclarlos |
| `ui-ugui` | Canvas, RectTransform, prefabs de UI | Compatible. uGUI 2.5.0 instalado | ÚTIL | HUD y menús propuestos en D-09 |
| `ui-uitk` | UXML y USS | Compatible con el módulo UIElements | ÚTIL | Reservada a herramientas de editor |
| `ui-imgui` | Inspectores y ventanas IMGUI | Compatible | ÚTIL | Solo si se mantiene código IMGUI. El proyecto nuevo de editor debería ir a UI Toolkit |
| `urp-postprocessing` | Volumes: bloom, color, viñeta | URP 17 ya está | ÚTIL | Fase de look. Hay `DefaultVolumeProfile` y `SampleSceneProfile` |
| `validate-urp-render-graph-renderer-feature` | Revisar un Renderer Feature de Unity 6 | Compatible con URP 17 | ÚTIL | Solo cuando exista un feature propio. Hoy no hay |
| `shader-graph-create-custom-node` | Nodos HLSL para Shader Graph | Shader Graph 17.5.0 está como transitivo | ÚTIL | Fase de materiales. No hace falta para el MVP |
| `audio-setup-mixers` | Enrutar fuentes a grupos de mixer | Compatible. No hay mixer en el proyecto | ÚTIL | La mezcla está pedida en la sección 17 |
| `optimize-audio` | Memoria, load type, mono | Compatible | ÚTIL | Fase 8 y 10, cuando haya clips |
| `optimize-text-mesh-pro` | Fuentes TMP y atlases | TMP llega con uGUI 2 | ÚTIL | HUD. No hay fuente de juego todavía |
| `generate-editor-search-query` | Búsquedas dentro del Editor | Requiere Editor abierto | ÚTIL | Cuando 6.5.6 esté abierto |
| `game-team-orchestrator` | Coordinar varios especialistas de GameForge | El archivo se lee. `disable-model-invocation: true`. Invoca skills y docs que no están | ÚTIL | Referencia de handoffs. No es un equipo ejecutable |
| `initialize-ai-navigation` | NavMesh, agentes, patrullas | El paquete 2.0.14 está. El GDD no pide IA | NO APLICABLE | Los monstruos los manejan jugadores |
| `setup-multiplayer-services` | Sesiones y red de Unity Gaming Services | Empuja otro stack distinto de Photon | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `setup-vivox-voice-chat` | Voz y texto Vivox | Paquete ausente. El GDD no pide voz | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `build-live-game` | Backend UGS: cuentas, Cloud Code, remote config, battle pass | Otro stack. Ranked está fuera de la demo | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `localization` | Tablas de idiomas | Paquete de Localization ausente. El GDD no pide i18n | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `implement-in-app-purchases` | IAP, Stripe, Coda | El GDD prohíbe pay-to-win | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `levelplay-unity-integration` | Anuncios LevelPlay | No está en el GDD | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `optimize-web` | WebGL y WebGPU | Objetivo Windows | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `2d-pixel-perfect` | Cámara 2D pixel perfect | Juego 3D | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `create-2d-physics` | Física 2D | Juego 3D | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `create-tile-palette-or-rule-tiles` | Tilemaps 2D | No hay tilemap de juego | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `sprite-editor` | Recortes de sprites | No hay sprites de juego | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `sprite-segment-3x3grid` | Analizar una textura 3x3 | Utilidad de sprite 2D | NO APLICABLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `manage-sprite-atlas` | Atlases de sprites | Útil si el HUD usa muchos sprites | POR VERIFICAR | Depende de D-09 y del volumen de iconos. No bloquea el MVP |
| `migrate-birp-to-urp` | Pasar Built-in a URP | El proyecto ya es URP | NO APLICABLE | Migrar otra vez no aporta |
| `new-unity-project` | Crear un proyecto nuevo | El proyecto ya existe | NO APLICABLE | Seguirlo crearía un segundo proyecto |
| `build-gtk` | Grafos de editor con Graph Toolkit | `required_editor_version: >=6000.6`. El proyecto es 6.5.6 | INCOMPATIBLE | Borrada del repositorio el 3 de noviembre de 2026 |
| `asset-transformer-toolkit` | Importación Pixyz / Industry Toolkit | Exige `com.unity.industry.toolkit` >= 4. No está instalado. El skill ordena detenerse | POR VERIFICAR | Solo si más adelante hay licencia y CAD. No instalarlo ahora |
| Unity Agent Plugin | Supuesto plugin de agente Unity | No está en `.cursor`, ni en los plugins de la sesión, ni como MCP | POR VERIFICAR | No hay evidencia de instalación. Lo más cercano es `unity-cli`, inactivo |
| Photon Fusion 2 | Red del juego. Topología cerrada: Host Mode | 2.1.4 Stable, build 2500, compiló en `6000.5.6f1` el 3 de noviembre de 2026. La página de requisitos sigue en 6.0.x y 6.3.x y no nombra este editor | ÚTIL | Prueba autorizada, no certificación oficial. App ID en el asset local, fuera de git. El paquete oficial es un `.unitypackage` en `Assets/Photon` |
| MCP Miro | Tableros y diagramas | Presente en la sesión | ÚTIL | Opcional para un grafo visual. La fuente sigue siendo el repo |
| MCP Trello | Tableros de tareas | Presente en la sesión | ÚTIL | Opcional. El checklist maestro está en git |
| MCP Figma | Diseño de interfaz | Presente en la sesión. No hay archivo de Figma en el repo | POR VERIFICAR | Útil si David diseña el HUD ahí. El GDD no lo exige |
| Plugin Prisma | ORM de base de datos | Instalado en la caché de Cursor | NO APLICABLE | No hay base de datos en el stack |
| Skills de Cursor (`automate`, `canvas`, `create-rule`, `create-hook`, `create-skill`, `loop`, `origin`, `new-repo`, `sdk`, `share`, `split-to-prs`, `autopilot`, `review-bugbot`, `review-security`, `statusline`, `update-cursor-settings`) | Operar el IDE y el flujo git | Ejecutables como skills de Cursor | ÚTIL | Sirven al proceso. No implementan el juego. `create-rule` es el mecanismo usado para `.mdc` |
| Skills de Codex en `C:\Users\pc\.agents\skills` | Las mismas de producto más Vercel, shell, review, goal, subagentes | Existen en el perfil de Codex. Este repositorio no las referencia | REDUNDANTE | Duplican las skills de Cursor. `deploy-with-vercel` no aplica a un FPS de Unity |

`ui-imgui` y `ui-uitk` no duplican a `ui-ugui`: `ui` es el router y las otras tres son el destino. `optimize-audio` y `audio-setup-mixers` se reparten mezcla y coste. Eso no es redundancia.

## Qué usar y qué dejar fuera

Usar en el flujo de MEATGRINDER:

- Reglas de `.cursor/rules/` y `AGENTS.md`.
- `unity-package-management` y, cuando el CLI exista, `unity-cli`.
- `physics-3d-collision` para diagnósticos 3D.
- `ui` y después `ui-ugui` para la interfaz de jugador.
- `urp-postprocessing`, `audio-setup-mixers`, `optimize-audio` y `optimize-text-mesh-pro` en sus fases.
- `game-team-orchestrator` solo como mapa de responsabilidades y de handoff.
- Documentación oficial de Photon citada en el apéndice B del GDD. No hay skill local de Fusion.

Borradas del repositorio el 3 de noviembre de 2026, a petición de David:

- `setup-multiplayer-services`, `build-live-game`, `setup-vivox-voice-chat`.
- `levelplay-unity-integration`, `implement-in-app-purchases`, `localization`.
- `2d-pixel-perfect`, `create-2d-physics`, `sprite-editor`, `sprite-segment-3x3grid`, `optimize-web`, `create-tile-palette-or-rule-tiles`, `build-gtk`.

Siguen en el repositorio y quedan fuera del flujo de juego: `migrate-birp-to-urp`, `new-unity-project`, `initialize-ai-navigation`.
- Pipelines del orquestador (`game-brainstorm`, `game-start`, y el resto): no están instalados.
- Prisma y Vercel.
- Anuncios, IAP, voz y ranked.

`asset-transformer-toolkit`, `manage-sprite-atlas` y Figma se consultan solo si la tarea correspondiente aparece y sus dependencias están realmente instaladas.
