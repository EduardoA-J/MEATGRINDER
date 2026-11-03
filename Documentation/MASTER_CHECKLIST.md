# Checklist maestro

Fecha: 2 de noviembre de 2026.
Fuente: `MEATGRINDER_GDD_TDD.docx`. Requisitos: [GDD_REQUIREMENTS.md](GDD_REQUIREMENTS.md). Decisiones: [DECISIONS.md](DECISIONS.md).

Estados usados: `BACKLOG`, `READY`, `IN PROGRESS`, `BLOCKED`, `REVIEW`, `DONE`.
`DONE` solo con prueba realizada. En este documento están en `DONE` la auditoría escrita (F0-01, F0-12), la apertura verificada del editor (F0-02, F0-03) y la prueba de importación de Fusion 2.1.4 (F0-05, F0-06). Ninguna función de juego está hecha. El App ID de Photon está en el asset local y fuera de git.

Prioridad: P0 bloquea el MVP, P1 entra en la demo de 8 semanas, P2 es demo si el núcleo aguanta, P3 queda fuera.
Complejidad: S, M, L, XL. Es orden de magnitud, no una estimación en días. El GDD no da velocidades de equipo.

Recuento: 13 epics, 119 tareas, 248 subtareas.

## Epics

| Epic | Fase | Tareas |
|---|---|---|
| E00 Preparación | 0 | 12 |
| E01 Movimiento | 1 | 11 |
| E02 Combate | 2 | 12 |
| E03 Red | 3 | 17 |
| E04 Monstruos | 4 | 8 |
| E05 Mapas | 5 | 9 |
| E06 Ítems | 6 | 7 |
| E07 UI | 7 | 10 |
| E08 Arte y audio | 8 | 8 |
| E09 Modos | 9 | 6 |
| E10 QA | 10 | 8 |
| E11 Ranked | Posterior | 4 |
| E12 Propuestas | Propuesta | 7 |

## Índice

| ID | Tarea | Estado | Depende de |
|---|---|---|---|
| F0-01 | Auditoría de repositorio, GDD y herramientas | DONE | Ninguna |
| F0-02 | Instalar Unity 6000.5.6f1 y abrir grinder | DONE | Ninguna. Requiere acción de David en Unity Hub. |
| F0-03 | Registrar la primera compilación | DONE | F0-02 |
| F0-04 | Política de ramas Git | READY | Ninguna |
| F0-05 | Elegir la versión de Photon Fusion 2 y el modo de sesión | DONE | David autorizó la prueba de 2.1.4 el 3 de noviembre de 2026 |
| F0-06 | Instalar Photon Fusion 2 | DONE | F0-02, F0-05 |
| F0-07 | Crear Assets/_Project y ensamblados vacíos | BACKLOG | F0-03 |
| F0-08 | Escena Boot vacía | BACKLOG | F0-07 |
| F0-09 | Mapa de input de juego | BACKLOG | F0-03, decisión D-17 si se van a borrar acciones |
| F0-10 | Acordar el PC de referencia | BLOCKED | David |
| F0-11 | Registro de licencias | READY | Ninguna |
| F0-12 | Inventario de paquetes de plantilla no pedidos | DONE | F0-01 |
| F1-01 | MovementTuning | BACKLOG | F0-07 |
| F1-02 | Cámara FPS | BACKLOG | F0-09 |
| F1-03 | Input de Move, Look y Jump | BACKLOG | F0-09 |
| F1-04 | Aceleración en suelo y fricción | BACKLOG | F1-01, F1-03, F5-01 |
| F1-05 | Salto y gravedad de diseño | BACKLOG | F1-04 |
| F1-06 | Control aéreo | BACKLOG | F1-05 |
| F1-07 | Strafe jumping | BACKLOG | F1-02, F1-06 |
| F1-08 | Bunny hopping | BACKLOG | F1-05, F1-07 |
| F1-09 | Rampas, escalones y plataformas | BACKLOG | F1-04, F5-01 |
| F1-10 | Prueba repetible de movimiento | BACKLOG | F1-04, F1-05, F1-06, F1-07, F1-08, F1-09 |
| F1-11 | Decidir el dash | BLOCKED | David |
| F2-01 | DamageInfo y DamageResolver | BACKLOG | F0-07 |
| F2-02 | HealthComponent | BACKLOG | F2-01 |
| F2-03 | Mitigación de armadura | BLOCKED | D-07, F2-01 |
| F2-04 | Muerte única y respawn | BACKLOG | F2-02, F3-03 para el respawn en red; en local puede probarse antes |
| F2-05 | Protección de spawn | BACKLOG | F2-04 |
| F2-06 | WeaponDefinition y WeaponController | BACKLOG | F0-07, F2-01 |
| F2-07 | Escopeta | BACKLOG | F2-06, F1-02 |
| F2-08 | Lanzacohetes y explosión | BACKLOG | F2-06 |
| F2-09 | Munición y cambio de arma | BACKLOG | F2-06 |
| F2-10 | Recogida de arma en el mapa | BACKLOG | F2-06, F6-05 |
| F2-11 | Feedback local de impacto | BACKLOG | F2-07 |
| F2-12 | Pruebas EditMode de daño | BACKLOG | F2-01, F2-02, F2-04, F2-09 |
| F3-01 | Bootstrap de Fusion | BLOCKED | F0-08. El SDK y el App ID local ya están. Falta la escena Boot |
| F3-02 | Crear, unir y salir de sesión | DONE | F3-01. La prueba usó NetworkTest; la escena Boot sigue pendiente |
| F3-03 | Spawn autorizado | BLOCKED | F3-02, F5-04 |
| F3-04 | Movimiento replicado con predicción | BACKLOG | F1-10, F3-03, D-19 puede seguir abierta si la prueba solo registra el error observado |
| F3-05 | Equipos | BACKLOG | F3-03 |
| F3-06 | Sincronizar arma, salud y armadura | BACKLOG | F2-02, F2-06, F3-05 |
| F3-07 | Daño autoritativo | BACKLOG | F2-01, F2-07, F3-06 |
| F3-08 | Proyectiles autoritativos | BACKLOG | F2-08, F3-07 |
| F3-09 | Política de lag compensation | BLOCKED | D-03, F3-07 |
| F3-10 | Muerte y respawn en red | BACKLOG | F2-04, F2-05, F3-07 |
| F3-11 | Desconexión de la demo | BACKLOG | F3-02 |
| F3-12 | Dos clientes en la misma máquina | BACKLOG | F3-02, F3-04 |
| F3-13 | LAN e internet | BACKLOG | F3-12, F3-07 |
| F3-14 | Latencia y pérdida de paquetes | BACKLOG | F3-04, F3-07 |
| F3-15 | Transición de mapa en red | BACKLOG | F5-07, F3-02 |
| F3-16 | Muertes casi simultáneas | BACKLOG | F3-10 |
| F3-17 | Recogida simultánea | BACKLOG | F6-05, F3-06 |
| F4-01 | MonsterDefinition | BACKLOG | F0-07 |
| F4-02 | Ficha del demonio ligero y del pesado | BLOCKED | David, F4-01 |
| F4-03 | Selección validada por la autoridad | BACKLOG | F3-05, F4-02 |
| F4-04 | Hitboxes | BACKLOG | F4-02 |
| F4-05 | Ataque melee | BACKLOG | F2-01, F4-04 |
| F4-06 | Armas y potenciadores para el monstruo | BLOCKED | D-13, F2-10, F6-05, F4-03 |
| F4-07 | Identidad visual en red | BACKLOG | F4-03, F3-06, F8-03 |
| F4-08 | Prueba de asimetría 2v2 | BACKLOG | F4-05, F3-13, F5-02 |
| F5-01 | Arena gris | BACKLOG | F0-07 |
| F5-02 | Blockout de M01 The Slaughterhouse | BACKLOG | F5-01, F1-10 |
| F5-03 | Dos rutas y verticalidad | BACKLOG | F5-02 |
| F5-04 | Spawns separados | BACKLOG | F5-02 |
| F5-05 | Layout de ítems y armas | BACKLOG | F5-02 |
| F5-06 | Puertas de recorrido | BACKLOG | F5-02 |
| F5-07 | Carga del siguiente estado | BACKLOG | F5-06, F9-01 |
| F5-08 | Pruebas de spawn-kill y de bloqueo | BACKLOG | F5-03, F5-04, F1-09 |
| F5-09 | M02 Iron Cathedral | BACKLOG | F10-02, F5-08 |
| F6-01 | Pickup de salud | BACKLOG | F2-02, F6-05 |
| F6-02 | Pickup de armadura | BLOCKED | F2-03, F6-05 |
| F6-03 | Definir e implementar Quad Damage | BLOCKED | D-10, F6-06 |
| F6-04 | Temporizadores de reaparición | BLOCKED | D-11, F6-01 |
| F6-05 | Autoridad y anti doble recogida | BACKLOG | F2-01, F3-07 |
| F6-06 | Acumulación de efectos | BLOCKED | D-11 |
| F6-07 | Réplica de pickups | BACKLOG | F6-05, F3-06 |
| F7-01 | Menú principal | BACKLOG | F0-08, D-09 |
| F7-02 | Crear y unirse | BACKLOG | F3-02, F7-01 |
| F7-03 | Selección de equipo y de monstruo | BACKLOG | F4-03, F7-02 |
| F7-04 | HUD de salud, armadura y munición | BACKLOG | F2-02, F2-09, F3-06 |
| F7-05 | Retícula | BACKLOG | F1-02 |
| F7-06 | Indicador de potenciador | BACKLOG | F6-03, F6-07 |
| F7-07 | Marcador y feed | BACKLOG | F3-07, F3-10, F9-03 |
| F7-08 | Resultados y revancha | BACKLOG | F9-05, F7-07 |
| F7-09 | Ajustes | BACKLOG | F7-01, F1-02 |
| F7-10 | Errores de conexión | BACKLOG | F3-02, F3-11, F7-02 |
| F8-01 | Materiales base y volumen | BACKLOG | F5-02 |
| F8-02 | Humano, manos FPS y silueta remota | BACKLOG | F0-11, F1-02 |
| F8-03 | Dos monstruos | BACKLOG | F4-02, F0-11 |
| F8-04 | Modelos de escopeta y lanzacohetes | BACKLOG | F2-06, F0-11 |
| F8-05 | Kit de M01 | BACKLOG | F5-02, F8-01 |
| F8-06 | VFX de disparo, sangre y decals con tope | BACKLOG | F2-11, F7-09 |
| F8-07 | Set de audio de combate | BACKLOG | F2-07, F2-08, F0-11 |
| F8-08 | Animación mínima de presentación | BACKLOG | F8-02, F8-03, F4-05 |
| F9-01 | Cerrar el modo de la demo | BLOCKED | David |
| F9-02 | Reglas de equipo de la demo | BACKLOG | F3-05, F9-01 |
| F9-03 | Puntuación | BACKLOG | F9-01, F3-10, F5-07 |
| F9-04 | Intercambio de roles | BACKLOG | F9-01, F9-03, F5-07 |
| F9-05 | Fin de partida y revancha | BACKLOG | F9-03, F3-02 |
| F9-06 | Validación 4v4 | BACKLOG | F3-13, F4-08, F10-04, D-05 |
| F10-01 | Matriz funcional | BACKLOG | F1-10, F2-12, F6-07, F5-08, F7-10 |
| F10-02 | Matriz de red | BACKLOG | F3-12, F3-13, F3-14, F3-15, F3-16, F3-17 |
| F10-03 | Balance | BACKLOG | F4-08, F5-08, F6-03 |
| F10-04 | Rendimiento | BACKLOG | F0-10, F8-06, F3-13 |
| F10-05 | Build de Windows | BACKLOG | F10-01 |
| F10-06 | Arranque fuera del Editor | BACKLOG | F10-05, F7-01 |
| F10-07 | README de instalación y controles | BACKLOG | F1-03, F7-02, F10-05 |
| F10-08 | Cierre por Definition of Done | BACKLOG | F10-01, F10-02, F10-06 |
| R-01 | Fórmula de MMR y reglas congeladas | BACKLOG | F10-08, David |
| R-02 | Identidad y backend de resultados | BACKLOG | R-01 |
| R-03 | Matchmaking y abandonos | BACKLOG | R-02 |
| R-04 | Temporadas, rangos y recompensas cosméticas | BACKLOG | R-02 |
| P-01 | Menú de pausa | BACKLOG | David |
| P-02 | Bots de desconexión | BACKLOG | F10-08 |
| P-03 | Barriles explosivos | BACKLOG | F10-08 |
| P-04 | Desmembramiento por piezas | BACKLOG | F8-06, F10-04 |
| P-05 | Ametralladora, superescopeta y rayo | BACKLOG | F10-08 |
| P-06 | Asistencias | BACKLOG | F7-07 |
| P-07 | Medidor de estilo | BACKLOG | F10-08 |

## Fichas

### F0-01 Auditoría de repositorio, GDD y herramientas

- **ID:** F0-01
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Auditoría de repositorio, GDD y herramientas
- **Descripción:** Dejar escrito el estado real del proyecto, del GDD y de las skills.
- **Objetivo:** Tener una base verificable antes de programar.
- **Justificación:** El GDD pide preproducción y prohíbe dar por hechas funciones no probadas.
- **Dependencias:** Ninguna
- **Archivos o sistemas:** Documentation/, AGENTS.md, .cursor/rules/
- **Skills o plugins:** create-rule
- **Responsable:** Technical Architect
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Leer el docx por XML puede aplanar tablas. Quedó registrado en RISKS.
- **Criterios de aceptación:**
  - Existe TECHNICAL_AUDIT.md con la versión de Unity declarada y la ausencia de Fusion.
  - El GDD usado es MEATGRINDER_GDD_TDD.docx.
  - No se modificó grinder/Assets ni Packages.
- **Pruebas:** Relectura de ProjectVersion.txt, manifest.json y del texto extraído del docx.
- **Estado:** DONE
- **Subtareas:**
  - F0-01a Inventariar paquetes y escenas.
  - F0-01b Extraer el GDD sin completar huecos.
  - F0-01c Clasificar skills sin desinstalarlas.

### F0-02 Instalar Unity 6000.5.6f1 y abrir grinder

- **ID:** F0-02
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Instalar Unity 6000.5.6f1 y abrir grinder
- **Descripción:** Editor verificado en `F:\Unity\Hub\Editor\6000.5.6f1`. El proyecto `grinder/` se abrió con ese binario.
- **Objetivo:** Poder importar el proyecto con el editor del GDD.
- **Justificación:** RT-01. Abrirlo con 2020.3 sería otra versión.
- **Dependencias:** Ninguna. Requiere acción de David en Unity Hub.
- **Archivos o sistemas:** grinder/ProjectSettings/ProjectVersion.txt
- **Skills o plugins:** unity-cli, cuando el binario exista
- **Responsable:** Technical Architect
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** K-02. Un editor distinto reescribe ProjectSettings.
- **Criterios de aceptación:**
  - Unity Hub muestra 6000.5.6f1.
  - El proyecto abre con ese editor.
  - ProjectVersion.txt sigue en 6000.5.6f1.
- **Pruebas:** `grinder/Logs/Editor.log` línea de versión `6000.5.6f1 (0e0577a1a2ac)` y proceso `F:\Unity\Hub\Editor\6000.5.6f1\Editor\Unity.exe`. `ProjectVersion.txt` no cambió.
- **Estado:** DONE
- **Subtareas:**
  - F0-02a Instalar el editor exacto.
  - F0-02b Abrir grinder/ y esperar el import.

### F0-03 Registrar la primera compilación

- **ID:** F0-03
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Registrar la primera compilación
- **Descripción:** Anotar errores y warnings reales de consola tras el primer import.
- **Objetivo:** Saber si la plantilla compila antes de añadir sistemas.
- **Justificación:** El primer import ya generó `grinder/Logs/Editor.log` y `grinder/Library`.
- **Dependencias:** F0-02
- **Archivos o sistemas:** grinder/Logs/, Documentation/PROGRESS.md
- **Skills o plugins:** unity-cli
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Safe Mode si el primer import falla.
- **Criterios de aceptación:**
  - La consola del editor 6.5.6 queda copiada en PROGRESS.md.
  - Cada error tiene archivo y mensaje.
  - Si no hay errores, se dice explícitamente que la consola estaba limpia.
- **Pruebas:** Leído `Editor.log` el 3 de noviembre de 2026. Una compilación de scripts: `Tundra build success (51.84 seconds)`. Cero coincidencias de `error CS`. El refresco inicial de assets terminó (`Asset Pipeline Refresh`, 311.844 s). `upm.log` resolvió paquetes con HTTP 200. Warnings de UPM, no de C#: `com.unity.searcher` 4.9.3 elevado a 4.9.4, `com.unity.burst` 1.8.14 elevado a 1.8.29, `com.unity.test-framework.performance` 3.2.0 elevado a 3.5.0.
- **Estado:** DONE
- **Subtareas:**
  - F0-03a Importar.
  - F0-03b Copiar errores y warnings literales.

### F0-04 Política de ramas Git

- **ID:** F0-04
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Política de ramas Git
- **Descripción:** Definir rama de trabajo, qué no se commitea y cómo se versiona una build.
- **Objetivo:** Cumplir el ítem de preproducción del GDD.
- **Justificación:** Solo existe main, alineada con origin.
- **Dependencias:** Ninguna
- **Archivos o sistemas:** .gitignore, Documentation/DECISIONS.md
- **Skills o plugins:** Ninguna skill de Unity
- **Responsable:** Technical Architect
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Reescribir historia de main.
- **Criterios de aceptación:**
  - Queda escrito el nombre de la rama de desarrollo.
  - PhotonAppId.local sigue ignorado.
  - No se hace force push.
- **Pruebas:** git status después de documentar la política, sin commits no pedidos.
- **Estado:** READY
- **Subtareas:**
  - F0-04a Escribir la política en DECISIONS cuando David la confirme.
  - F0-04b Comprobar que el gitignore cubre Library, Logs y secretos.

### F0-05 Elegir la versión de Photon Fusion 2 y el modo de sesión

- **ID:** F0-05
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Elegir la versión de Photon Fusion 2 y el modo de sesión
- **Descripción:** Host Mode y Fusion 2.1.4 Stable, build 2500, quedaron elegidos. David autorizó la prueba controlada el 3 de noviembre de 2026.
- **Objetivo:** Evitar implementar movimiento de red sobre un modo supuesto.
- **Justificación:** David fijó Host Mode, plan gratuito y partidas de 2 jugadores hacia 2v2. La página oficial de requisitos sigue listando Unity 6.0.x y 6.3.x. Esta elección es una prueba autorizada, no una certificación de Photon para `6000.5.6f1`.
- **Dependencias:** Aprobación de David, dada el 3 de noviembre de 2026.
- **Archivos o sistemas:** Documentation/DECISIONS.md
- **Skills o plugins:** Documentación oficial de Fusion.
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** K-04. La matriz oficial sigue sin nombrar `6000.5.6f1`.
- **Criterios de aceptación:**
  - T-21 está tomada: Host Mode. El host tiene la autoridad de estado.
  - T-22 registra la versión importada y el límite de la prueba.
  - Shared Mode queda fuera hasta una justificación técnica y una aprobación nueva.
- **Pruebas:** Notas en DECISIONS.md con las URLs consultadas el 3 de noviembre de 2026. `grinder/Assets/Photon/Fusion/release_history.txt` abre con 2.1.4 Stable, build 2500.
- **Estado:** DONE
- **Subtareas:**
  - F0-05a Consultar compatibilidad Fusion 2 con 6000.5. Hecho: el changelog 2.1, build 2159, declara un arreglo de compilación para Unity 6000.5. La matriz de descarga no lista 6.5.
  - F0-05b Elegir modo y escribir qué peer tiene autoridad. Hecho: Host Mode; el jugador host es la autoridad.
  - F0-05c Autorización de David para la prueba de 2.1.4. Hecho el 3 de noviembre de 2026.

### F0-06 Instalar Photon Fusion 2

- **ID:** F0-06
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Instalar Photon Fusion 2
- **Descripción:** Importar el `.unitypackage` oficial de Fusion 2.1.4 Stable, build 2500. El SDK vive en `Assets/Photon`, no en el registro UPM. El App ID queda vacío.
- **Objetivo:** Tener el SDK compilando antes de la prueba de dos clientes.
- **Justificación:** RN-01.
- **Dependencias:** F0-02, F0-05
- **Archivos o sistemas:** grinder/Assets/Photon, grinder/Logs/fusion-import.log
- **Skills o plugins:** Importación con `Unity.exe -importPackage`. El paquete oficial no entra por `unity-package-management`.
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** App Id filtrado al repositorio. K-04 sigue abierto en ejecución: la compilación no certifica el editor.
- **Criterios de aceptación:**
  - `release_history.txt` identifica 2.1.4 Stable, build 2500.
  - El batchmode de `6000.5.6f1` termina con código 0 y la última compilación es `Tundra build success`.
  - `AppIdFusion` está en el asset local. `PhotonAppSettings.asset` y su `.meta` están en `.gitignore`. El valor no aparece en documentación.
- **Pruebas:** `grinder/Logs/fusion-import.log`, 3 de noviembre de 2026. Cuatro `Tundra build success`; la última, 20.60 segundos, 448 ítems. `Exiting batchmode successfully now!` y código de retorno 0. Búsqueda de `error CS`: ninguna coincidencia. Photon Realtime 5.1.20. El `.unitypackage` anidado `Photon-Fusion-Intro-Shared-2.1.4-Stable-2500.unitypackage` no se importó.
- **Estado:** DONE
- **Subtareas:**
  - F0-06a Importar el `.unitypackage`. Hecho.
  - F0-06b Pegar el App ID. Hecho en el asset local el 3 de noviembre de 2026. El archivo está ignorado por git.
  - F0-06c Compilar. Hecho en batchmode.

### F0-07 Crear Assets/_Project y ensamblados vacíos

- **ID:** F0-07
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Crear Assets/_Project y ensamblados vacíos
- **Descripción:** Crear la estructura de la sección 14.1 sin gameplay.
- **Objetivo:** Tener un lugar único para el código nuevo.
- **Justificación:** RT-03. Hoy solo existe la plantilla.
- **Dependencias:** F0-03
- **Archivos o sistemas:** grinder/Assets/_Project/**
- **Skills o plugins:** Ninguna
- **Responsable:** Technical Architect
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Mover SampleScene rompería el build settings sin necesidad.
- **Criterios de aceptación:**
  - Existen las carpetas 00 a 05 y Tests.
  - Hay asmdefs de runtime y de tests sin referencias circulares.
  - SampleScene sigue en el build hasta que Boot la reemplace en su propia tarea.
- **Pruebas:** El Editor compila los ensamblados vacíos.
- **Estado:** BACKLOG
- **Subtareas:**
  - F0-07a Crear carpetas.
  - F0-07b Crear asmdefs.
  - F0-07c No mover assets de URP.

### F0-08 Escena Boot vacía

- **ID:** F0-08
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Escena Boot vacía
- **Descripción:** Añadir una escena de arranque que todavía no carga gameplay.
- **Objetivo:** Separar menú, lobby y mapas, como pide el apéndice A.
- **Justificación:** El build solo contiene SampleScene.
- **Dependencias:** F0-07
- **Archivos o sistemas:** grinder/Assets/_Project/00_Scenes/Boot/
- **Skills o plugins:** unity-cli si el Editor está abierto
- **Responsable:** Technical Architect
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Dejar el juego sin escena cargable.
- **Criterios de aceptación:**
  - Boot está en Build Settings.
  - Entra en Play y no instancia jugador, arma ni red.
  - SampleScene puede quedar desactivada del build solo en esta tarea si Boot ya carga.
- **Pruebas:** Play Mode de Boot sin errores de consola nuevos.
- **Estado:** BACKLOG
- **Subtareas:**
  - F0-08a Crear la escena.
  - F0-08b Registrarla en EditorBuildSettings.

### F0-09 Mapa de input de juego

- **ID:** F0-09
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Mapa de input de juego
- **Descripción:** Separar las acciones que el GDD pide de Crouch y Sprint de la plantilla.
- **Objetivo:** RT-02 y D-17.
- **Justificación:** El asset actual es el de la plantilla URP.
- **Dependencias:** F0-03, decisión D-17 si se van a borrar acciones
- **Archivos o sistemas:** grinder/Assets/InputSystem_Actions.inputactions o un asset nuevo en _Project
- **Skills o plugins:** Ninguna skill específica de input; el paquete ya está
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Romper el binding de la plantilla que nadie usa todavía.
- **Criterios de aceptación:**
  - Existen acciones Move, Look, Jump y Attack enlazadas a teclado y ratón.
  - Crouch y Sprint no alimentan el controlador de MEATGRINDER mientras D-17 siga abierta.
  - El juego no lee el Input Manager antiguo.
- **Pruebas:** En Play, el log de una escena de prueba muestra el vector de Move y el eje de Look. Esa escena pertenece a F1, no a esta tarea si aún no hay controlador.
- **Estado:** BACKLOG
- **Subtareas:**
  - F0-09a Listar acciones del GDD.
  - F0-09b Crear el asset de juego sin reutilizar Crouch como mecánica.

### F0-10 Acordar el PC de referencia

- **ID:** F0-10
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Acordar el PC de referencia
- **Descripción:** Escribir CPU, GPU, RAM y resolución del objetivo de 60 FPS.
- **Objetivo:** RNF-01. Sin máquina de referencia el presupuesto no se puede comprobar.
- **Justificación:** El GDD dice que el PC debe estar acordado y no lo describe.
- **Dependencias:** David
- **Archivos o sistemas:** Documentation/DECISIONS.md
- **Skills o plugins:** Ninguna
- **Responsable:** Technical Architect
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Optimizar contra el PC del desarrollador y fallar en el objetivo.
- **Criterios de aceptación:**
  - D-15 tiene un equipo concreto.
  - F10-04 usa ese equipo y no otro.
- **Pruebas:** El texto de D-15 queda cerrado por David.
- **Estado:** BLOCKED
- **Subtareas:**
  - F0-10a David describe el equipo.
  - F0-10b Anotarlo como decisión tomada.

### F0-11 Registro de licencias

- **ID:** F0-11
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Registro de licencias
- **Descripción:** Abrir una tabla de assets externos con fuente, licencia, atribución y cambios.
- **Objetivo:** RT-07 y K-12.
- **Justificación:** Todavía no hay assets de juego. El registro puede nacer vacío.
- **Dependencias:** Ninguna
- **Archivos o sistemas:** Documentation/ASSET_LICENSES.md se crea en esta tarea, no antes
- **Skills o plugins:** Ninguna
- **Responsable:** Technical Artist
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Importar un pack antes de registrarlo.
- **Criterios de aceptación:**
  - La tabla existe y hoy no contiene filas inventadas.
  - Cada asset que se importe después añade una fila antes de usarse en una escena.
- **Pruebas:** Revisión del archivo. No hay binarios nuevos en Assets.
- **Estado:** READY
- **Subtareas:**
  - F0-11a Crear la tabla vacía.
  - F0-11b Enlazarla desde PROJECT_CONTEXT.

### F0-12 Inventario de paquetes de plantilla no pedidos

- **ID:** F0-12
- **Fase:** 0
- **Epic:** E00 Preparación
- **Nombre:** Inventario de paquetes de plantilla no pedidos
- **Descripción:** Dejar constancia de Navigation, Visual Scripting, Timeline, Collab Proxy y Multiplayer Center.
- **Objetivo:** Evitar que un agente los use como si fueran diseño.
- **Justificación:** D-22. Esta fase no desinstala.
- **Dependencias:** F0-01
- **Archivos o sistemas:** Documentation/TECHNICAL_AUDIT.md
- **Skills o plugins:** unity-package-management en modo lectura
- **Responsable:** Technical Architect
- **Prioridad:** P2
- **Complejidad:** S
- **Riesgos:** Desinstalar y romper la plantilla.
- **Criterios de aceptación:**
  - La auditoría nombra esos paquetes y su versión.
  - Ninguno fue quitado del manifiesto en esta fase.
- **Pruebas:** Diff de manifest.json vacío respecto del inventario.
- **Estado:** DONE
- **Subtareas:**
  - F0-12a Leer manifest.json.
  - F0-12b Clasificar cada paquete directo frente al GDD.

### F1-01 MovementTuning

- **ID:** F1-01
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** MovementTuning
- **Descripción:** ScriptableObject con GroundSpeed, GroundAcceleration, AirAcceleration, JumpVelocity, Gravity, Friction, MaxSpeed y DashCooldown.
- **Objetivo:** Ajustar el movimiento sin recompilar.
- **Justificación:** GDD 4.2. Los números no existen.
- **Dependencias:** F0-07
- **Archivos o sistemas:** Assets/_Project/02_ScriptableObjects/Movement/
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Tratar el primer valor de prueba como definitivo.
- **Criterios de aceptación:**
  - El asset expone los ocho campos del GDD.
  - Cada valor de prototipo lleva comentario PENDIENTE.
  - No hay constantes de velocidad duplicadas en el controlador.
- **Pruebas:** EditMode: el asset se carga y los campos existen.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-01a Crear el tipo.
  - F1-01b Crear el asset con valores marcados.

### F1-02 Cámara FPS

- **ID:** F1-02
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Cámara FPS
- **Descripción:** Cámara anclada a la cabeza, Look con ratón, pitch limitado, FOV leído de un dato configurable.
- **Objetivo:** Mirar y apuntar antes de disparar.
- **Justificación:** El GDD pide primera persona con teclado y ratón. El FOV numérico inicial queda PENDIENTE.
- **Dependencias:** F0-09
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Players/
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Giro que atraviesa el cuello o mueve el cuerpo en Y con el ratón.
- **Criterios de aceptación:**
  - El yaw gira el cuerpo y el pitch gira solo la cámara.
  - El pitch no pasa del tope configurado.
  - Sin input de Look la cámara no deriva.
- **Pruebas:** Escena de movimiento: mover el ratón y comprobar los tres criterios.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-02a Componente de mirada.
  - F1-02b Límite de pitch en el mismo asset o en uno de cámara.

### F1-03 Input de Move, Look y Jump

- **ID:** F1-03
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Input de Move, Look y Jump
- **Descripción:** Leer el asset de juego con el Input System y exponer un frame de intención.
- **Objetivo:** Una sola puerta de entrada para el controlador.
- **Justificación:** RT-02.
- **Dependencias:** F0-09
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Players/
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Leer Input.GetAxis clásico con el backend nuevo, que no responde.
- **Criterios de aceptación:**
  - Move, Look y Jump salen del Input System.
  - No hay llamadas a UnityEngine.Input en el controlador nuevo.
- **Pruebas:** Play Mode con el asset de acciones de juego.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-03a Envolver el asset.
  - F1-03b Prueba manual de cada acción.

### F1-04 Aceleración en suelo y fricción

- **ID:** F1-04
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Aceleración en suelo y fricción
- **Descripción:** En suelo, la velocidad horizontal se acerca a la deseada según GroundAcceleration y cae según Friction al soltar la dirección.
- **Objetivo:** RJ-01.
- **Justificación:** GDD 4.1.
- **Dependencias:** F1-01, F1-03, F5-01
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Movement/PlayerMovement.cs
- **Skills o plugins:** physics-3d-collision si el cuerpo atraviesa el suelo
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Usar el Character Controller de un tutorial genérico y perder el modelo de Quake.
- **Criterios de aceptación:**
  - Mantener la dirección aumenta la velocidad hasta el objetivo de GroundSpeed.
  - Soltar la dirección reduce la velocidad por Friction.
  - El cambio de esos dos campos altera el resultado sin editar el script.
- **Pruebas:** Recorrido recto cronometrado, repetido dos veces con los mismos campos.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-04a Integración de velocidad en suelo.
  - F1-04b Fricción al no haber input.

### F1-05 Salto y gravedad de diseño

- **ID:** F1-05
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Salto y gravedad de diseño
- **Descripción:** Impulso JumpVelocity y aceleración Gravity del ScriptableObject, distinta de la gravedad PhysX salvo que se decida unificarlas en D-18.
- **Objetivo:** Saltar sin perder el modelo de movimiento.
- **Justificación:** GDD 4.1 y 4.2.
- **Dependencias:** F1-04
- **Archivos o sistemas:** PlayerMovement.cs, MovementTuning
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Aplicar las dos gravedades y obtener un salto impredecible.
- **Criterios de aceptación:**
  - Con Jump, la velocidad vertical pasa a JumpVelocity.
  - En el aire, la velocidad vertical cambia según Gravity del asset.
  - El jugador no puede saltar otra vez hasta volver a Grounded, salvo la ventana que defina el bunny hop en F1-08.
- **Pruebas:** Salto en plano, medido dos veces.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-05a Impulso.
  - F1-05b Integrar Gravity del asset y dejar PhysX documentado en D-18.

### F1-06 Control aéreo

- **ID:** F1-06
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Control aéreo
- **Descripción:** En el aire, AirAcceleration modifica la velocidad horizontal sin ponerla a cero.
- **Objetivo:** El jugador no pierde el control al saltar.
- **Justificación:** GDD 4.3.
- **Dependencias:** F1-05
- **Archivos o sistemas:** PlayerMovement.cs
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Copiar la aceleración de suelo y anular el strafe.
- **Criterios de aceptación:**
  - Durante el salto, una dirección lateral cambia la velocidad horizontal.
  - Al dejar de pulsar, la velocidad horizontal no cae por Friction de suelo.
  - El aterrizaje no teletransporta al jugador.
- **Pruebas:** Salto con corrección lateral, repetido.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-06a Rama de aire.
  - F1-06b Caso de aterrizaje.

### F1-07 Strafe jumping

- **ID:** F1-07
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Strafe jumping
- **Descripción:** Combinar mirada y dirección lateral en el aire puede aumentar la velocidad respecto del salto anterior, respetando MaxSpeed.
- **Objetivo:** La habilidad de movimiento decide encuentros, como pide la referencia a Quake.
- **Justificación:** GDD 4.1. El incremento exacto sale del prototipo.
- **Dependencias:** F1-02, F1-06
- **Archivos o sistemas:** PlayerMovement.cs
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Velocidad infinita si MaxSpeed no actúa.
- **Criterios de aceptación:**
  - Una secuencia escrita de strafe produce más velocidad horizontal que un salto recto con los mismos campos.
  - La velocidad no supera la regla de MaxSpeed del asset.
  - La secuencia está descrita en la prueba para poder repetirla.
- **Pruebas:** Prueba de movimiento F1-10, caso strafe.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-07a Acoplar yaw al wishdir.
  - F1-07b Aplicar MaxSpeed.

### F1-08 Bunny hopping

- **ID:** F1-08
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Bunny hopping
- **Descripción:** Saltar al tocar suelo conserva parte de la velocidad horizontal.
- **Objetivo:** GDD 4.1.
- **Justificación:** La fracción conservada no está escrita. Vive en el asset como PENDIENTE.
- **Dependencias:** F1-05, F1-07
- **Archivos o sistemas:** PlayerMovement.cs
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Ventana de salto tan ancha que el suelo deja de existir.
- **Criterios de aceptación:**
  - Un salto en el frame de aterrizaje conserva velocidad horizontal por encima de GroundSpeed si se venía más rápido.
  - Un aterrizaje largo sin nuevo salto aplica Friction.
  - La ventana, si existe, es un campo del asset.
- **Pruebas:** Caso de la prueba F1-10.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-08a Detectar aterrizaje.
  - F1-08b Conservar la fracción configurada.

### F1-09 Rampas, escalones y plataformas

- **ID:** F1-09
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Rampas, escalones y plataformas
- **Descripción:** El cuerpo sube escalones bajos y rampas sin impulsos raros ni quedarse enganchado de forma habitual.
- **Objetivo:** GDD 4.1 y 4.3.
- **Justificación:** La altura de escalón no está en el GDD. Campo PENDIENTE.
- **Dependencias:** F1-04, F5-01
- **Archivos o sistemas:** PlayerMovement.cs, geometría de la arena gris
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Kinematic versus malla cóncava. Atravesar suelos finos a alta velocidad.
- **Criterios de aceptación:**
  - El jugador sube la rampa de la arena gris y se queda sobre ella.
  - Sube el escalón marcado y no lo trata como muro.
  - En diez pasadas no atraviesa el suelo.
- **Pruebas:** Recorrido de geometría de F5-01.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-09a Step offset configurable.
  - F1-09b Caso de rampa y caso de escalón.

### F1-10 Prueba repetible de movimiento

- **ID:** F1-10
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Prueba repetible de movimiento
- **Descripción:** Una escena o test con recorrido fijo: recta, salto, strafe, bhop, rampa.
- **Objetivo:** Poder comparar cambios de MovementTuning.
- **Justificación:** GDD 4.3 y 21.1. También a otra tasa de fotogramas, sin exigir todavía un número de Hz concreto más allá del objetivo global.
- **Dependencias:** F1-04, F1-05, F1-06, F1-07, F1-08, F1-09
- **Archivos o sistemas:** Assets/_Project/Tests/PlayMode/
- **Skills o plugins:** unity-cli cuando exista
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Una prueba manual imposible de repetir.
- **Criterios de aceptación:**
  - El caso recta, el caso salto, el caso strafe, el caso bhop y el caso rampa tienen pasos escritos.
  - Dos ejecuciones con el mismo asset dan la misma conclusión de cumple o no cumple.
  - Cambiar GroundAcceleration cambia el caso recta.
- **Pruebas:** Ejecutar la prueba y pegar el resultado en PROGRESS.md.
- **Estado:** BACKLOG
- **Subtareas:**
  - F1-10a Escribir los pasos.
  - F1-10b Ejecutarlos en el Editor.

### F1-11 Decidir el dash

- **ID:** F1-11
- **Fase:** 1
- **Epic:** E01 Movimiento
- **Nombre:** Decidir el dash
- **Descripción:** Cerrar D-06. Si entra, tendrá cooldown en el asset y no será un poder de monstruo.
- **Objetivo:** No construir una mecánica opcional por defecto.
- **Justificación:** GDD 4.1.
- **Dependencias:** David
- **Archivos o sistemas:** Documentation/DECISIONS.md
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P2
- **Complejidad:** S
- **Riesgos:** Añadir un dash que rompa el tiempo de las armas.
- **Criterios de aceptación:**
  - D-06 dice sí o no.
  - Si es no, DashCooldown permanece sin uso.
  - Si es sí, hay una tarea de implementación nueva antes de codificarlo.
- **Pruebas:** Decisión escrita.
- **Estado:** BLOCKED
- **Subtareas:**
  - F1-11a David elige.
  - F1-11b Actualizar la regla 03.

### F2-01 DamageInfo y DamageResolver

- **ID:** F2-01
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** DamageInfo y DamageResolver
- **Descripción:** Un solo sitio aplica daño: atacante, objetivo, cantidad, tipo, punto e id de red.
- **Objetivo:** RNF-03 y RF-06.
- **Justificación:** GDD 5.3. Prohíbe que cada arma tenga su regla.
- **Dependencias:** F0-07
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Combat/
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Aplicar daño en el cliente y otra vez en el servidor.
- **Criterios de aceptación:**
  - Dos llamadas con la misma confirmación no restan vida dos veces.
  - Una llamada sin autoridad no modifica la salud cuando la red exista.
  - El tipo de daño distingue hitscan, proyectil, melee y explosión.
- **Pruebas:** EditMode F2-12.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-01a Tipo DamageInfo.
  - F2-01b Resolver con id de confirmación.

### F2-02 HealthComponent

- **ID:** F2-02
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** HealthComponent
- **Descripción:** Salud actual, tope y evento de daño ya confirmado.
- **Objetivo:** La UI y la muerte escuchan; no calculan.
- **Justificación:** GDD 5.1 y 14.2.
- **Dependencias:** F2-01
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Combat/HealthComponent.cs
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Tope numérico presentado como definitivo.
- **Criterios de aceptación:**
  - La salud no baja de cero.
  - El evento se dispara una vez por daño confirmado.
  - El tope del humano es un campo de configuración marcado PENDIENTE.
- **Pruebas:** EditMode: daño parcial y daño mayor que la salud.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-02a Estado.
  - F2-02b Evento.

### F2-03 Mitigación de armadura

- **ID:** F2-03
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Mitigación de armadura
- **Descripción:** Aplicar la fórmula de armadura cuando D-07 exista.
- **Objetivo:** RF-07.
- **Justificación:** El GDD no da porcentaje ni tope.
- **Dependencias:** D-07, F2-01
- **Archivos o sistemas:** DamageResolver
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director y Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Inventar el modelo de DOOM y cerrarlo como diseño.
- **Criterios de aceptación:**
  - Con armadura 0, el daño llega entero a la salud.
  - Con armadura mayor que 0, la salud recibe menos según la fórmula escrita en D-07.
  - La armadura no se vuelve negativa.
- **Pruebas:** EditMode con la fórmula ya aprobada.
- **Estado:** BLOCKED
- **Subtareas:**
  - F2-03a Cerrar D-07.
  - F2-03b Implementar solo esa fórmula.

### F2-04 Muerte única y respawn

- **ID:** F2-04
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Muerte única y respawn
- **Descripción:** Al llegar a cero se muere una vez y se reaparece tras el temporizador.
- **Objetivo:** RF-06.
- **Justificación:** La duración es D-08 y sigue pendiente. El campo existe igual.
- **Dependencias:** F2-02, F3-03 para el respawn en red; en local puede probarse antes
- **Archivos o sistemas:** HealthComponent, SpawnManager
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Doble evento de muerte que duplica el feed.
- **Criterios de aceptación:**
  - Dos daños que cruzan cero en el mismo paso generan una sola muerte.
  - El jugador reaparece con la salud al tope.
  - El tiempo de espera es el campo PENDIENTE, no un número mágico en el script.
- **Pruebas:** EditMode de muerte duplicada y Play Mode de un respawn.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-04a Flag de muerte.
  - F2-04b Temporizador configurable.

### F2-05 Protección de spawn

- **ID:** F2-05
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Protección de spawn
- **Descripción:** Durante el tiempo mínimo configurado, el jugador recién aparecido no recibe daño.
- **Objetivo:** RJ-05.
- **Justificación:** D-08 no tiene segundos. El criterio se prueba con el valor del asset.
- **Dependencias:** F2-04
- **Archivos o sistemas:** SpawnManager, DamageResolver
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Protección eterna si el temporizador no arranca.
- **Criterios de aceptación:**
  - Un daño dentro de la ventana no baja la salud.
  - Un daño después de la ventana sí la baja.
  - La ventana termina sola.
- **Pruebas:** EditMode con escala de tiempo controlada o ticks manuales.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-05a Ventana.
  - F2-05b Rechazo en el resolver.

### F2-06 WeaponDefinition y WeaponController

- **ID:** F2-06
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** WeaponDefinition y WeaponController
- **Descripción:** Datos de arma y equipo activo. El controlador pide disparar; no resuelve el daño final si la autoridad existe.
- **Objetivo:** RF-05.
- **Justificación:** GDD 6.1.
- **Dependencias:** F0-07, F2-01
- **Archivos o sistemas:** Weapons/WeaponDefinition.cs, WeaponController.cs
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Cada arma con su propia clase de daño.
- **Criterios de aceptación:**
  - Un asset describe id, daño, cadencia, dispersión, proyectil, radio, munición, recarga, prefab, VFX y audio.
  - Cambiar de arma activa el otro definition.
  - El cambio no dispara un tiro extra.
- **Pruebas:** EditMode de cambio de arma y de cadencia.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-06a ScriptableObject.
  - F2-06b Controlador de equipo.

### F2-07 Escopeta

- **ID:** F2-07
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Escopeta
- **Descripción:** Disparo hitscan o cono de rayos, más daño de cerca que de lejos.
- **Objetivo:** Arma de demo.
- **Justificación:** GDD sección 6. La curva de caída no tiene números.
- **Dependencias:** F2-06, F1-02
- **Archivos o sistemas:** Weapons/, un asset Shotgun
- **Skills o plugins:** physics-3d-collision si el rayo falla
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Raycast que ignora al tirador o atraviesa paredes.
- **Criterios de aceptación:**
  - Un disparo a un objetivo cercano aplica daño.
  - El mismo disparo a mayor distancia aplica menos, según el campo del asset.
  - Un muro entre ambos impide el daño.
  - La cadencia del asset rechaza un segundo disparo inmediato.
- **Pruebas:** Escena gris con un blanco y un muro.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-07a Rayos.
  - F2-07b Caída por distancia configurable.

### F2-08 Lanzacohetes y explosión

- **ID:** F2-08
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Lanzacohetes y explosión
- **Descripción:** Proyectil con velocidad y daño en radio.
- **Objetivo:** Presión de área de la demo.
- **Justificación:** GDD 5.2 y 6. Radio y daño sin cifras.
- **Dependencias:** F2-06
- **Archivos o sistemas:** Weapons/, Projectile
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Explosión que daña dos veces o empuja sin regla de diseño. El GDD deja el desplazamiento de explosión como opcional futuro en barriles, no como regla del cohete.
- **Criterios de aceptación:**
  - El proyectil viaja a la velocidad del asset.
  - Al impactar, daña objetivos dentro del radio una sola vez.
  - Un objetivo detrás de un muro sólido no recibe ese daño, salvo que D de penetración se apruebe; hoy no existe esa decisión, así que el muro bloquea.
- **Pruebas:** Play Mode con dos blancos, uno tapado.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-08a Proyectil local.
  - F2-08b Radio y oclusión.

### F2-09 Munición y cambio de arma

- **ID:** F2-09
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Munición y cambio de arma
- **Descripción:** La munición baja al disparar y el arma vacía no daña.
- **Objetivo:** RF-05.
- **Justificación:** GDD 6.2. La recarga existe como campo; el GDD no obliga un modelo de cargador frente a reserva.
- **Dependencias:** F2-06
- **Archivos o sistemas:** WeaponController
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Cerrar un modelo de recarga no escrito.
- **Criterios de aceptación:**
  - Cada disparo válido consume la cantidad del asset.
  - Con munición 0 no hay DamageInfo.
  - El arma inicial del humano tiene la munición de su definition.
- **Pruebas:** EditMode de consumo y de arma vacía.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-09a Contador.
  - F2-09b Rechazo en vacío. El modelo de recarga queda PENDIENTE si hace falta otro campo.

### F2-10 Recogida de arma en el mapa

- **ID:** F2-10
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Recogida de arma en el mapa
- **Descripción:** El arma del escenario pasa al equipo del jugador cuando la autoridad lo confirma.
- **Objetivo:** Los humanos no empiezan con el arsenal completo.
- **Justificación:** GDD 6.2.
- **Dependencias:** F2-06, F6-05
- **Archivos o sistemas:** Pickups/, WeaponController
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Duplicar el arma en dos clientes.
- **Criterios de aceptación:**
  - Tras la confirmación, el arma está en el equipo y desaparece del suelo para todos.
  - Un segundo jugador no la recibe.
  - En local, sin red, la misma regla vale con una autoridad simulada.
- **Pruebas:** Dos recogidas seguidas en la prueba de F6-05.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-10a Pickup de arma.
  - F2-10b Alta en el equipo.

### F2-11 Feedback local de impacto

- **ID:** F2-11
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Feedback local de impacto
- **Descripción:** Retroceso de cámara configurable, marca de impacto y confirmación de que el tiro conectó.
- **Objetivo:** RJ-02.
- **Justificación:** GDD 5.1. Las curvas de shake no tienen números.
- **Dependencias:** F2-07
- **Archivos o sistemas:** Players/, VFX local
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** El shake tapa al objetivo.
- **Criterios de aceptación:**
  - Un impacto en un blanco muestra confirmación.
  - Un impacto en el mundo no usa la confirmación de jugador.
  - La amplitud del retroceso es un campo y puede ponerse a cero.
- **Pruebas:** Play Mode de la escopeta contra un blanco.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-11a Confirmación.
  - F2-11b Retroceso configurable.

### F2-12 Pruebas EditMode de daño

- **ID:** F2-12
- **Fase:** 2
- **Epic:** E02 Combate
- **Nombre:** Pruebas EditMode de daño
- **Descripción:** Cubrir doble aplicación, muerte única, cadencia y arma vacía.
- **Objetivo:** RT-05.
- **Justificación:** GDD 21.1 y 23.
- **Dependencias:** F2-01, F2-02, F2-04, F2-09
- **Archivos o sistemas:** Assets/_Project/Tests/EditMode/
- **Skills o plugins:** Test Framework ya instalado
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Tests que dependen de Play Mode y no se pueden repetir en CI.
- **Criterios de aceptación:**
  - El test de doble confirmación falla si el daño se aplica dos veces.
  - El test de muerte espera un solo evento.
  - Los tests no cargan SampleScene.
- **Pruebas:** Ejecutar EditMode y registrar el recuento real.
- **Estado:** BACKLOG
- **Subtareas:**
  - F2-12a Casos.
  - F2-12b Correrlos en el Editor 6.5.6.

### F3-01 Bootstrap de Fusion

- **ID:** F3-01
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Bootstrap de Fusion
- **Descripción:** Arranque de red, sesión y lectura del App Id local.
- **Objetivo:** RN-01.
- **Justificación:** GDD 13.4.
- **Dependencias:** F0-08. F0-06 está hecha y el App ID ya está en el asset local. Falta la escena Boot.
- **Archivos o sistemas:** Assets/_Project/01_Scripts/Networking/FusionSessionManager.cs
- **Skills o plugins:** Documentación de Fusion. No setup-multiplayer-services.
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** K-04. Dejar el App Id en un asset versionado.
- **Criterios de aceptación:**
  - El juego entra a Boot sin red y no lanza excepciones.
  - Con App Id local, el runner de Fusion inicia en el modo de D-02.
  - git no contiene el App Id.
- **Pruebas:** Play de Boot y revisión de git status.
- **Estado:** BLOCKED
- **Subtareas:**
  - F3-01a Runner.
  - F3-01b Secreto local.

### F3-02 Crear, unir y salir de sesión

- **ID:** F3-02
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Crear, unir y salir de sesión
- **Descripción:** Una sesión con identificador. Un segundo cliente entra. Ambos pueden salir.
- **Objetivo:** RF-01.
- **Justificación:** GDD 13.1.
- **Dependencias:** F3-01
- **Archivos o sistemas:** `Assets/_Project/01_Scripts/Networking/NetworkSessionController.cs`
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Sesión huérfana que impide volver a crear.
- **Criterios de aceptación:**
  - El cliente A crea y obtiene un identificador.
  - El cliente B entra con ese identificador.
  - Tras salir, ninguno sigue recibiendo estado de esa sesión.
- **Pruebas:** 3 de noviembre de 2026. Editor `6000.5.6f1` en batchmode como Host y `Builds/NetworkTest/MeatgrinderProbe.exe` como Client, sesión `MeatgrinderProbe-20261103`. `grinder/Logs/network-test-host.txt`: `START_OK Host`, jugador 2 entra (`total=2`) y sale (`total=1`), `HOST_SAW_DESPAWN`, runner cerrado `Ok`. `network-test-client.txt`: `START_OK Client`, `LocalPlayer=2`, `CLIENT_SAW_TWO` (dos `NetworkTestAvatar` en el cliente), runner cerrado `Ok`.
- **Estado:** DONE
- **Subtareas:**
  - F3-02a Create.
  - F3-02b Join.
  - F3-02c Leave.

### F3-03 Spawn autorizado

- **ID:** F3-03
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Spawn autorizado
- **Descripción:** Solo la autoridad instancia al jugador en un punto libre.
- **Objetivo:** RF-03.
- **Justificación:** GDD 13.4.
- **Dependencias:** F3-02, F5-04
- **Archivos o sistemas:** Networking/, SpawnManager
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Los dos clientes crean su propia copia autoritativa.
- **Criterios de aceptación:**
  - Cada jugador conectado aparece una vez.
  - El punto lo elige la autoridad.
  - Un cliente no puede forzar las coordenadas de spawn por RPC sin validación.
- **Pruebas:** La sonda del 3 de noviembre de 2026 spawneó un avatar estático por jugador y el Host lo despawneó al salir el Client. Sigue faltando la arena gris y la lista de puntos de F5-04.
- **Estado:** BLOCKED
- **Subtareas:**
  - F3-03a Lista de puntos.
  - F3-03b Spawn de red.

### F3-04 Movimiento replicado con predicción

- **ID:** F3-04
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Movimiento replicado con predicción
- **Descripción:** El local responde el mismo frame. El remoto ve la postura. Hay reconciliación.
- **Objetivo:** RN-02 y RN-03.
- **Justificación:** GDD 4.1 y 13.3. La tolerancia es D-19.
- **Dependencias:** F1-10, F3-03, D-19 puede seguir abierta si la prueba solo registra el error observado
- **Archivos o sistemas:** PlayerMovement, NetworkPlayerState
- **Skills o plugins:** Documentación de Fusion sobre predicción
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** XL
- **Riesgos:** K-05. El modelo Quake no encaja en un motor de personaje genérico.
- **Criterios de aceptación:**
  - El local gira y se mueve sin esperar un eco de red.
  - El otro cliente ve el desplazamiento.
  - Tras un salto con strafe, el remoto no queda en una pose de suelo si el autoritativo está en el aire.
  - La prueba anota el error visual observado, sin declararlo dentro de tolerancia mientras D-19 siga abierta.
- **Pruebas:** Dos clientes, caso de F1-10 en red.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-04a Input predicho.
  - F3-04b Estado replicado.
  - F3-04c Corrección.

### F3-05 Equipos

- **ID:** F3-05
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Equipos
- **Descripción:** Cada jugador tiene equipo humano o monstruo, asignado por la autoridad.
- **Objetivo:** RF-02.
- **Justificación:** GDD 11.1.
- **Dependencias:** F3-03
- **Archivos o sistemas:** NetworkPlayerState, VersusManager
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Un cliente se cambia de equipo escribiendo el estado.
- **Criterios de aceptación:**
  - Los dos clientes muestran el mismo equipo para el mismo jugador.
  - Una petición ilegal no cambia el equipo.
- **Pruebas:** Dos clientes, uno por equipo.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-05a Campo de equipo.
  - F3-05b Rechazo de cambio ilegal.

### F3-06 Sincronizar arma, salud y armadura

- **ID:** F3-06
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Sincronizar arma, salud y armadura
- **Descripción:** El estado visible sale del estado autoritativo.
- **Objetivo:** RN-02.
- **Justificación:** GDD 13.1.
- **Dependencias:** F2-02, F2-06, F3-05
- **Archivos o sistemas:** NetworkPlayerState
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Dos contadores, el local y el de red, que divergen.
- **Criterios de aceptación:**
  - Un cambio de salud en la autoridad se ve en el otro cliente.
  - El arma activa coincide.
  - La armadura coincide cuando F2-03 exista; hasta entonces el campo viaja en cero.
- **Pruebas:** Dos clientes y un daño de prueba.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-06a Propiedades de red.
  - F3-06b Lectura en el remoto.

### F3-07 Daño autoritativo

- **ID:** F3-07
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Daño autoritativo
- **Descripción:** El cliente envía la intención. La autoridad valida cadencia, munición y línea de tiro, y emite un solo daño.
- **Objetivo:** RNF-03, RN-04, RN-07.
- **Justificación:** GDD 13.2 y 19.
- **Dependencias:** F2-01, F2-07, F3-06
- **Archivos o sistemas:** PlayerCombat, DamageResolver
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** XL
- **Riesgos:** Confiar en el daño que manda el cliente.
- **Criterios de aceptación:**
  - Un disparo válido resta salud una vez en ambos clientes.
  - Un disparo más rápido que la cadencia no resta.
  - Con munición 0 no resta.
  - El feed, cuando exista, recibe un solo evento.
- **Pruebas:** Dos clientes y el test de doble confirmación adaptado a red.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-07a Intención de disparo.
  - F3-07b Validación.
  - F3-07c Aplicación única.

### F3-08 Proyectiles autoritativos

- **ID:** F3-08
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Proyectiles autoritativos
- **Descripción:** El cohete tiene trayectoria y explosión consistentes.
- **Objetivo:** RN-02.
- **Justificación:** GDD 13.1 y 13.3.
- **Dependencias:** F2-08, F3-07
- **Archivos o sistemas:** ProjectileNetwork
- **Skills o plugins:** physics-3d-collision, documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Un proyectil visual y otro autoritativo que explotan en sitios distintos y dañan dos veces.
- **Criterios de aceptación:**
  - Ambos clientes ven el impacto en el mismo obstáculo.
  - El daño de área llega una vez.
  - El proyectil no es una partícula de gore en red.
- **Pruebas:** Dos clientes y un muro.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-08a Estado del proyectil.
  - F3-08b Explosión única.

### F3-09 Política de lag compensation

- **ID:** F3-09
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Política de lag compensation
- **Descripción:** Cerrar D-03 antes de rebobinar hitboxes.
- **Objetivo:** RN-05.
- **Justificación:** El GDD no la da por activada.
- **Dependencias:** D-03, F3-07
- **Archivos o sistemas:** Documentation/DECISIONS.md
- **Skills o plugins:** Documentación de Fusion sobre lag compensation
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Rewind activado sin presupuesto de tiempo ni de distancia.
- **Criterios de aceptación:**
  - D-03 dice si hay compensación, y con qué límite.
  - Si es no, el hitscan usa la pose actual autoritativa.
  - Si es sí, la prueba con latencia artificial está escrita.
- **Pruebas:** Decisión y, solo entonces, una prueba.
- **Estado:** BLOCKED
- **Subtareas:**
  - F3-09a David elige.
  - F3-09b Documentar el límite.

### F3-10 Muerte y respawn en red

- **ID:** F3-10
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Muerte y respawn en red
- **Descripción:** La muerte y el nuevo spawn coinciden en todos los clientes.
- **Objetivo:** RF-06.
- **Justificación:** GDD 13.1.
- **Dependencias:** F2-04, F2-05, F3-07
- **Archivos o sistemas:** HealthComponent, SpawnManager
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** El remoto sigue disparando con el cadáver.
- **Criterios de aceptación:**
  - Al morir, los dos clientes dejan de mostrar al jugador como vivo.
  - Reaparece en un punto autorizado con salud al tope.
  - Durante la protección no recibe daño en ninguno de los dos.
- **Pruebas:** Dos clientes, un asesinato y un respawn.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-10a Estado muerto.
  - F3-10b Respawn autorizado.

### F3-11 Desconexión de la demo

- **ID:** F3-11
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Desconexión de la demo
- **Descripción:** Si un jugador se va, la sesión sigue y el abandono queda marcado.
- **Objetivo:** RF-16 y T-12.
- **Justificación:** GDD 11.4.
- **Dependencias:** F3-02
- **Archivos o sistemas:** NetworkGameManager
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Cerrar la sesión y borrar la puntuación.
- **Criterios de aceptación:**
  - Al cerrar un cliente, el otro permanece en la partida.
  - El jugador ausente no sigue infligiendo daño.
  - La puntuación ya sumada no se pone a cero.
- **Pruebas:** Dos clientes y cierre de uno a media partida.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-11a Detectar salida.
  - F3-11b Marcar y continuar.

### F3-12 Dos clientes en la misma máquina

- **ID:** F3-12
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Dos clientes en la misma máquina
- **Descripción:** Primera prueba de red del GDD, sección 13.5.
- **Objetivo:** Ver spawn, movimiento y un disparo.
- **Justificación:** Semana 1: el jugador se mueve y dos clientes se conectan.
- **Dependencias:** F3-02, F3-04
- **Archivos o sistemas:** Escena gris
- **Skills o plugins:** unity-cli si permite dos instancias; si no, el flujo manual de Fusion
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Dar por buena la red solo con esta prueba.
- **Criterios de aceptación:**
  - Dos ventanas entran en la misma sesión.
  - Se ven moverse.
  - El resultado queda en PROGRESS con fecha.
- **Pruebas:** Ejecución real. Esta ficha no se cierra con un test de un solo proceso.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-12a Levantar dos clientes.
  - F3-12b Anotar lo observado.

### F3-13 LAN e internet

- **ID:** F3-13
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** LAN e internet
- **Descripción:** Repetir la sesión en dos máquinas y, después, fuera de la LAN.
- **Objetivo:** RNF-04.
- **Justificación:** GDD 13.5 y semana 3.
- **Dependencias:** F3-12, F3-07
- **Archivos o sistemas:** Build de desarrollo
- **Skills o plugins:** Ninguna skill local de Photon
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Firewall o App Id de desarrollo que solo acepta LAN.
- **Criterios de aceptación:**
  - Dos PCs de la misma red juegan un disparo y una muerte.
  - Una prueba por internet hace lo mismo o deja el fallo concreto registrado.
- **Pruebas:** Checklist de la sección 13.5, filas de LAN e internet.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-13a LAN.
  - F3-13b Internet.

### F3-14 Latencia y pérdida de paquetes

- **ID:** F3-14
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Latencia y pérdida de paquetes
- **Descripción:** Simular latencia, jitter y pérdida, y anotar el efecto en movimiento y disparos.
- **Objetivo:** RN-06. La demo final exige al menos una prueba con latencia simulada.
- **Justificación:** GDD 13.3 y 13.5. No hay umbral numérico de aceptación todavía.
- **Dependencias:** F3-04, F3-07
- **Archivos o sistemas:** Herramienta de simulación de Fusion o del sistema
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Declarar jugable un caso que no se midió.
- **Criterios de aceptación:**
  - Queda escrito el valor de latencia aplicado y la herramienta usada.
  - El movimiento local sigue respondiendo al instante.
  - Se anota si el disparo hitscan se sintió adelantado o atrasado, sin activar lag compensation mientras F3-09 esté bloqueada.
- **Pruebas:** Una pasada con condiciones artificiales y nota en PROGRESS.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-14a Configurar la simulación.
  - F3-14b Observar movimiento y escopeta.

### F3-15 Transición de mapa en red

- **ID:** F3-15
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Transición de mapa en red
- **Descripción:** La autoridad ordena la carga. Nadie se queda en la escena anterior. Hay timeout.
- **Objetivo:** RF-11.
- **Justificación:** GDD 10.4.
- **Dependencias:** F5-07, F3-02
- **Archivos o sistemas:** MapFlowManager, Fusion
- **Skills o plugins:** Documentación de Fusion sobre cambio de escena
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Cliente desconectado a medias que vuelve con armas del mapa anterior. T-10 prohíbe conservarlas.
- **Criterios de aceptación:**
  - Todos los clientes conectados acaban en la escena indicada.
  - Un cliente que no carga dentro del timeout queda marcado.
  - La salud y las armas vuelven al estado inicial del modo.
- **Pruebas:** Dos clientes y una puerta de prueba.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-15a Orden de carga.
  - F3-15b Timeout.
  - F3-15c Reinicio de carga.

### F3-16 Muertes casi simultáneas

- **ID:** F3-16
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Muertes casi simultáneas
- **Descripción:** Dos bajas en el mismo instante generan dos eventos, no uno ni cuatro.
- **Objetivo:** GDD 13.5 y checklist de combate.
- **Justificación:** Sección 21.2.
- **Dependencias:** F3-10
- **Archivos o sistemas:** DamageResolver
- **Skills o plugins:** QA Engineer con el ingeniero de red
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** El feed pierde una baja o duplica las dos.
- **Criterios de aceptación:**
  - Cada víctima tiene una muerte.
  - Cada atacante conserva su baja.
  - La puntuación no suma de más.
- **Pruebas:** Dos clientes que se eliminan en la misma ventana.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-16a Caso de prueba.
  - F3-16b Comprobar feed y marcador.

### F3-17 Recogida simultánea

- **ID:** F3-17
- **Fase:** 3
- **Epic:** E03 Red
- **Nombre:** Recogida simultánea
- **Descripción:** Dos jugadores tocan el mismo ítem. Solo uno lo recibe.
- **Objetivo:** RN-06.
- **Justificación:** GDD 8.1 y 13.5.
- **Dependencias:** F6-05, F3-06
- **Archivos o sistemas:** PickupSpawner
- **Skills o plugins:** QA Engineer
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Los dos reciben Quad Damage o la escopeta.
- **Criterios de aceptación:**
  - Un solo cliente obtiene el efecto.
  - El objeto desaparece en los dos.
  - El segundo intento no genera otro efecto.
- **Pruebas:** Dos clientes sobre el mismo pickup.
- **Estado:** BACKLOG
- **Subtareas:**
  - F3-17a Preparar el caso.
  - F3-17b Registrar quién lo recibió.

### F4-01 MonsterDefinition

- **ID:** F4-01
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** MonsterDefinition
- **Descripción:** ScriptableObject de salud, velocidad, tamaño, ataque y prefab.
- **Objetivo:** RT-04.
- **Justificación:** GDD 14.3. Sin cifras de balance.
- **Dependencias:** F0-07
- **Archivos o sistemas:** 02_ScriptableObjects/Monsters/
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Un prefab distinto por cliente.
- **Criterios de aceptación:**
  - El asset tiene los campos de la sección 14.3.
  - Los valores llevan PENDIENTE.
  - El ataque básico no tiene munición.
- **Pruebas:** EditMode de carga del asset.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-01a Tipo.
  - F4-01b Asset vacío de cifras definitivas.

### F4-02 Ficha del demonio ligero y del pesado

- **ID:** F4-02
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Ficha del demonio ligero y del pesado
- **Descripción:** Escribir salud, velocidad, tamaño y ataque de cada uno cuando David los defina.
- **Objetivo:** RJ-07 y D-12.
- **Justificación:** La sección 7.2 solo describe un Demonio. El checklist pide dos.
- **Dependencias:** David, F4-01
- **Archivos o sistemas:** Documentation/DECISIONS.md, assets de monstruo
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Inventar un tanque y un asesino y cerrar el balance.
- **Criterios de aceptación:**
  - Hay dos fichas, o una decisión de que la demo lleva un solo monstruo.
  - Ninguna ficha obliga una única elección por un dato no probado.
- **Pruebas:** Revisión de diseño, sin playtest todavía.
- **Estado:** BLOCKED
- **Subtareas:**
  - F4-02a Cerrar D-12.
  - F4-02b Crear los assets con esos datos.

### F4-03 Selección validada por la autoridad

- **ID:** F4-03
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Selección validada por la autoridad
- **Descripción:** El jugador elige un monstruo legal al iniciar o al reaparecer.
- **Objetivo:** RF-02 y RF-10.
- **Justificación:** GDD 7.3.
- **Dependencias:** F3-05, F4-02
- **Archivos o sistemas:** MonsterSelectionUI, VersusManager
- **Skills o plugins:** ui, ui-ugui
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Un humano elige demonio y conserva la escopeta inicial del humano.
- **Criterios de aceptación:**
  - El equipo monstruo solo puede elegir definiciones de monstruo.
  - El equipo humano no pasa por esa lista.
  - Los dos clientes ven el mismo prefab para ese jugador.
- **Pruebas:** Dos clientes y un intento de selección ilegal.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-03a Petición.
  - F4-03b Validación y réplica.

### F4-04 Hitboxes

- **ID:** F4-04
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Hitboxes
- **Descripción:** El collider coincide con el tamaño del definition, no con la malla decorativa.
- **Objetivo:** RF-10.
- **Justificación:** GDD 7.3.
- **Dependencias:** F4-02
- **Archivos o sistemas:** Prefabs de Monsters
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Disparar a la cola decorativa y contar un impacto, o lo contrario.
- **Criterios de aceptación:**
  - Un rayo al volumen definido impacta.
  - Un rayo que solo toca decoración fuera del volumen no impacta.
  - El remoto usa el mismo volumen.
- **Pruebas:** Escopeta contra el prefab en la arena gris.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-04a Collider de juego.
  - F4-04b Separar malla visual.

### F4-05 Ataque melee

- **ID:** F4-05
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Ataque melee
- **Descripción:** Garras o mordisco a corta distancia, sin munición y sin cooldown de héroe.
- **Objetivo:** RJ-04.
- **Justificación:** GDD 5.2 y 7.2. Alcance y daño en el asset, PENDIENTE.
- **Dependencias:** F2-01, F4-04
- **Archivos o sistemas:** 01_Scripts/Monsters/
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Convertir el melee en un dash de personaje.
- **Criterios de aceptación:**
  - Dentro del alcance del asset hay DamageInfo de tipo melee.
  - Fuera del alcance no lo hay.
  - No se consume munición.
  - No existe un botón de habilidad extra.
- **Pruebas:** Play Mode contra un blanco quieto.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-05a Volumen de golpe.
  - F4-05b Daño vía resolver.

### F4-06 Armas y potenciadores para el monstruo

- **ID:** F4-06
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Armas y potenciadores para el monstruo
- **Descripción:** Aplicar T-11. Las armas de fuego solo si D-13 dice que sí.
- **Objetivo:** Cerrar la frase doble de la sección 6.2.
- **Justificación:** D-13.
- **Dependencias:** D-13, F2-10, F6-05, F4-03
- **Archivos o sistemas:** PickupSpawner, WeaponController
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Dar la escopeta al monstruo y borrar la asimetría sin querer.
- **Criterios de aceptación:**
  - Un monstruo recoge salud y armadura si la autoridad lo confirma.
  - Quad Damage sigue la misma regla cuando F6-03 exista.
  - Las armas de fuego se recogen solo si D-13 es afirmativa.
- **Pruebas:** Dos clientes y un pickup de salud en el equipo monstruo.
- **Estado:** BLOCKED
- **Subtareas:**
  - F4-06a Cerrar D-13.
  - F4-06b Implementar la rama aprobada.

### F4-07 Identidad visual en red

- **ID:** F4-07
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Identidad visual en red
- **Descripción:** Modelo, hitbox y estadísticas coinciden en todos los clientes.
- **Objetivo:** RF-10.
- **Justificación:** GDD 7.3.
- **Dependencias:** F4-03, F3-06, F8-03
- **Archivos o sistemas:** NetworkPlayerState
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** El remoto muestra el humano mientras el autoritativo es un demonio.
- **Criterios de aceptación:**
  - Cambiar la selección actualiza el prefab remoto.
  - La salud máxima remota es la del definition.
  - El ataque melee del remoto no se representa como disparo de escopeta.
- **Pruebas:** Dos clientes y un cambio de monstruo al respawn.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-07a Id de monstruo en red.
  - F4-07b Swap de presentación.

### F4-08 Prueba de asimetría 2v2

- **ID:** F4-08
- **Fase:** 4
- **Epic:** E04 Monstruos
- **Nombre:** Prueba de asimetría 2v2
- **Descripción:** Observar si el monstruo puede acercarse y si el humano no sostiene una posición sin riesgo.
- **Objetivo:** RJ-04 y RJ-07. GDD 21.3.
- **Justificación:** Semana 4.
- **Dependencias:** F4-05, F3-13, F5-02
- **Archivos o sistemas:** M01 en blockout
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Declarar balance con una sola partida.
- **Criterios de aceptación:**
  - Hay al menos tres partidas anotadas.
  - Cada una dice si el monstruo llegó a melee y si el humano pudo negar el mapa entero.
  - Los cambios de cifras se hacen en assets, no en código.
- **Pruebas:** Partidas 2v2 reales, resultado en PROGRESS.
- **Estado:** BACKLOG
- **Subtareas:**
  - F4-08a Protocolo de observación.
  - F4-08b Tres partidas.

### F5-01 Arena gris

- **ID:** F5-01
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Arena gris
- **Descripción:** Suelo, rampa, escalón, plataforma y un muro. Sin arte final.
- **Objetivo:** RJ-01 y la semana 1.
- **Justificación:** El GDD pide arena gris en la semana 1, antes que M01.
- **Dependencias:** F0-07
- **Archivos o sistemas:** 00_Scenes/Maps/Greybox
- **Skills o plugins:** unity-cli si el Editor está abierto
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Construir M01 definitivo en lugar del blockout.
- **Criterios de aceptación:**
  - La escena contiene suelo, rampa, escalón, plataforma y muro con colisión.
  - No tiene luces decorativas ni mallas finales.
  - Está en Build Settings.
- **Pruebas:** Recorrido a pie cuando exista F1-04. Hasta entonces, los colliders se seleccionan en el Editor.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-01a Geometría.
  - F5-01b Colliders.

### F5-02 Blockout de M01 The Slaughterhouse

- **ID:** F5-02
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Blockout de M01 The Slaughterhouse
- **Descripción:** Arena central, pasillos, rampas y puertas en blockout.
- **Objetivo:** Mapa de demo.
- **Justificación:** GDD 10.2.
- **Dependencias:** F5-01, F1-10
- **Archivos o sistemas:** 00_Scenes/Maps/M01, MapDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Cerrar el arte antes de saber si las rutas se juegan.
- **Criterios de aceptación:**
  - Hay una arena y pasillos reconocibles en el layout.
  - MapDefinition apunta a esa escena.
  - El documento de layout de F5-05 existe o esta tarea no se marca hecha.
- **Pruebas:** Recorrer las dos rutas con el controlador.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-02a Bloquear volúmenes.
  - F5-02b MapDefinition.

### F5-03 Dos rutas y verticalidad

- **ID:** F5-03
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Dos rutas y verticalidad
- **Descripción:** M01 ofrece al menos dos caminos y una ruta vertical.
- **Objetivo:** RJ-06.
- **Justificación:** GDD 10.1.
- **Dependencias:** F5-02
- **Archivos o sistemas:** M01
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Un pasillo único.
- **Criterios de aceptación:**
  - Desde el spawn humano se alcanzan dos entradas distintas al área central.
  - Hay una plataforma o rampa que cambia de altura.
  - Un jugador no puede sellar las dos rutas a la vez con su cuerpo.
- **Pruebas:** F5-08.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-03a Segunda ruta.
  - F5-03b Altura.

### F5-04 Spawns separados

- **ID:** F5-04
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Spawns separados
- **Descripción:** Puntos de humanos y de monstruos fuera de la línea de tiro directa.
- **Objetivo:** RF-03 y RJ-05.
- **Justificación:** GDD 10.1.
- **Dependencias:** F5-02
- **Archivos o sistemas:** SpawnManager, M01
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Spawn con vista directa al otro equipo.
- **Criterios de aceptación:**
  - Un rayo desde cada spawn enemigo hasta el punto de aparición choca con geometría.
  - Hay puntos suficientes para dos humanos y dos monstruos.
- **Pruebas:** F5-08.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-04a Colocar puntos.
  - F5-04b Rayos de comprobación.

### F5-05 Layout de ítems y armas

- **ID:** F5-05
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Layout de ítems y armas
- **Descripción:** Documento y marcadores de armas, munición, salud, armadura, Quad Damage, puertas y estrangulamientos.
- **Objetivo:** RJ-03.
- **Justificación:** GDD 10.3. El temporizador del ítem fuerte es corto; el número es D-11.
- **Dependencias:** F5-02
- **Archivos o sistemas:** MapDefinition, Documentation de layout de M01
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Esconder el ítem fuerte en una sala sin disputa.
- **Criterios de aceptación:**
  - Cada marcador tiene tipo y posición.
  - El Quad Damage, aunque su efecto siga pendiente, tiene una ubicación disputable.
  - El layout está en el repositorio.
- **Pruebas:** Revisión del documento contra la escena.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-05a Marcadores.
  - F5-05b Nota de disputa.

### F5-06 Puertas de recorrido

- **ID:** F5-06
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Puertas de recorrido
- **Descripción:** La puerta avanza. No pide llave ni puzzle.
- **Objetivo:** RF-11.
- **Justificación:** GDD 3.3 y 10.1.
- **Dependencias:** F5-02
- **Archivos o sistemas:** Maps/MapFlowManager
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Una puerta que se abre con un objeto de inventario.
- **Criterios de aceptación:**
  - Cruzar o activar la puerta marca el recorrido como listo para transicionar.
  - No hay inventario de llaves.
  - Un jugador no puede dejar la puerta cerrada para siempre.
- **Pruebas:** Play Mode en M01.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-06a Volumen de puerta.
  - F5-06b Señal de transición.

### F5-07 Carga del siguiente estado

- **ID:** F5-07
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Carga del siguiente estado
- **Descripción:** Al completar la puerta se carga la siguiente escena o el estado de fin si no hay segundo mapa.
- **Objetivo:** RF-11 y RF-17.
- **Justificación:** M02 es condicional. La demo de un mapa necesita un estado siguiente aunque sea la pantalla de resultados.
- **Dependencias:** F5-06, F9-01
- **Archivos o sistemas:** MapFlowManager, MapDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Cargar M02 inexistente y dejar la partida colgada.
- **Criterios de aceptación:**
  - Con un solo mapa, la puerta lleva al fin de recorrido definido por el modo.
  - Las armas no se conservan.
  - La puntuación de campaña se conserva.
- **Pruebas:** Play Mode de una partida que llega a la puerta.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-07a Orden de mapas.
  - F5-07b Reinicio de carga del jugador.

### F5-08 Pruebas de spawn-kill y de bloqueo

- **ID:** F5-08
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** Pruebas de spawn-kill y de bloqueo
- **Descripción:** Comprobar línea de tiro y que el cuerpo de un jugador no sella el avance.
- **Objetivo:** RJ-05 y RJ-06.
- **Justificación:** GDD 21.3.
- **Dependencias:** F5-03, F5-04, F1-09
- **Archivos o sistemas:** M01
- **Skills o plugins:** QA Engineer
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Probarlo solo en la arena gris y dar por bueno M01.
- **Criterios de aceptación:**
  - El rayo de F5-04 falla contra geometría en todos los spawns de M01.
  - Un jugador quieto en un pasillo deja la otra ruta usable.
  - El resultado queda anotado.
- **Pruebas:** Pasada en el Editor con el controlador.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-08a Rayos.
  - F5-08b Bloqueo con un jugador.

### F5-09 M02 Iron Cathedral

- **ID:** F5-09
- **Fase:** 5
- **Epic:** E05 Mapas
- **Nombre:** M02 Iron Cathedral
- **Descripción:** Segundo mapa, solo si el núcleo de red y M01 ya cumplen sus pruebas.
- **Objetivo:** Objetivo condicionado de la sección 22.
- **Justificación:** GDD 10.2: «si hay tiempo».
- **Dependencias:** F10-02, F5-08
- **Archivos o sistemas:** 00_Scenes/Maps/M02
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Empezar M02 con el movimiento todavía roto.
- **Criterios de aceptación:**
  - M01 y la prueba de dos máquinas están en DONE.
  - M02 tiene dos rutas, spawns cubiertos y una puerta.
  - Si no hay tiempo, la tarea pasa a una nota de corte, no a DONE.
- **Pruebas:** El mismo protocolo de F5-08.
- **Estado:** BACKLOG
- **Subtareas:**
  - F5-09a Decidir si entra en la demo.
  - F5-09b Blockout solo después del corte.

### F6-01 Pickup de salud

- **ID:** F6-01
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Pickup de salud
- **Descripción:** Restaura salud hasta el tope normal. No la supera.
- **Objetivo:** RF-08.
- **Justificación:** GDD sección 8.
- **Dependencias:** F2-02, F6-05
- **Archivos o sistemas:** Pickups/, PickupDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Curar por encima del tope y crear un segundo mega.
- **Criterios de aceptación:**
  - Con salud por debajo del tope, recoger salud la sube.
  - No pasa del tope del definition.
  - El objeto desaparece al confirmarse.
- **Pruebas:** EditMode del efecto y Play Mode del objeto.
- **Estado:** BACKLOG
- **Subtareas:**
  - F6-01a Definición.
  - F6-01b Aplicación en autoridad.

### F6-02 Pickup de armadura

- **ID:** F6-02
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Pickup de armadura
- **Descripción:** Otorga armadura según la regla de D-07.
- **Objetivo:** RF-07.
- **Justificación:** Sin fórmula no se puede afirmar cuánto absorbe.
- **Dependencias:** F2-03, F6-05
- **Archivos o sistemas:** PickupDefinition de armadura
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Sumar armadura infinita.
- **Criterios de aceptación:**
  - Recoger armadura aumenta el valor hasta el tope escrito en D-07.
  - El daño siguiente usa F2-03.
- **Pruebas:** EditMode encadenado con F2-03.
- **Estado:** BLOCKED
- **Subtareas:**
  - F6-02a Esperar D-07.
  - F6-02b Pickup.

### F6-03 Definir e implementar Quad Damage

- **ID:** F6-03
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Definir e implementar Quad Damage
- **Descripción:** Cerrar D-10 y solo entonces aplicar el efecto.
- **Objetivo:** RF-09. Está en la semana 5 y no en la tabla de la sección 8.
- **Justificación:** No hay multiplicador ni duración.
- **Dependencias:** D-10, F6-06
- **Archivos o sistemas:** PowerupEffectController
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Copiar el Quad Damage de Quake como si el GDD lo hubiera cuantificado.
- **Criterios de aceptación:**
  - D-10 describe el efecto, la duración y si reinicia la duración.
  - Durante la ventana, el daño de salida cambia como dice D-10.
  - Al expirar, el daño vuelve al valor del arma.
- **Pruebas:** EditMode de inicio y fin del efecto.
- **Estado:** BLOCKED
- **Subtareas:**
  - F6-03a Escribir D-10.
  - F6-03b Implementar esa regla.

### F6-04 Temporizadores de reaparición

- **ID:** F6-04
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Temporizadores de reaparición
- **Descripción:** Cada pickup reaparece según su campo. Cuáles son fijos lo dice D-11.
- **Objetivo:** RJ-03.
- **Justificación:** GDD 8.1 y sección 25.
- **Dependencias:** D-11, F6-01
- **Archivos o sistemas:** PickupSpawner
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Reaparición instantánea que anula la disputa.
- **Criterios de aceptación:**
  - Tras recogerse, el objeto no está.
  - Vuelve cuando pasa el tiempo de su definition.
  - Un ítem marcado sin temporizador no reaparece.
- **Pruebas:** Play Mode con el reloj del asset.
- **Estado:** BLOCKED
- **Subtareas:**
  - F6-04a Cerrar qué ítems temporizan.
  - F6-04b Spawner.

### F6-05 Autoridad y anti doble recogida

- **ID:** F6-05
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Autoridad y anti doble recogida
- **Descripción:** La autoridad concede el objeto una vez.
- **Objetivo:** RN-07.
- **Justificación:** GDD 8.1.
- **Dependencias:** F2-01, F3-07
- **Archivos o sistemas:** PickupSpawner
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Predicción visual que deja el objeto en el inventario aunque la autoridad lo niegue.
- **Criterios de aceptación:**
  - Dos peticiones del mismo objeto producen un solo efecto.
  - Si la autoridad niega, el cliente deshace la predicción visual.
  - El estado final coincide en los dos clientes.
- **Pruebas:** F3-17.
- **Estado:** BACKLOG
- **Subtareas:**
  - F6-05a Petición.
  - F6-05b Concesión única.

### F6-06 Acumulación de efectos

- **ID:** F6-06
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Acumulación de efectos
- **Descripción:** Definir si un segundo efecto igual reinicia, extiende o se ignora.
- **Objetivo:** GDD 8.1.
- **Justificación:** D-11.
- **Dependencias:** D-11
- **Archivos o sistemas:** PowerupEffectController
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Elegir una política dentro del código y olvidar escribirla.
- **Criterios de aceptación:**
  - D-11 nombra la política.
  - El segundo pickup se comporta como esa frase.
  - La UI de duración usa la misma hora de fin.
- **Pruebas:** EditMode de dos recogidas seguidas.
- **Estado:** BLOCKED
- **Subtareas:**
  - F6-06a Decisión.
  - F6-06b Caso de prueba.

### F6-07 Réplica de pickups

- **ID:** F6-07
- **Fase:** 6
- **Epic:** E06 Ítems
- **Nombre:** Réplica de pickups
- **Descripción:** Aparición, desaparición y fin de efecto iguales en todos los clientes.
- **Objetivo:** RN-02.
- **Justificación:** GDD 13.1.
- **Dependencias:** F6-05, F3-06
- **Archivos o sistemas:** PickupSpawner, PowerupEffectController
- **Skills o plugins:** Documentación de Fusion
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** El efecto sigue en un cliente después de expirar en la autoridad.
- **Criterios de aceptación:**
  - Al recoger, desaparece en los dos.
  - Al reaparecer, vuelve en los dos.
  - El temporizador de efecto llega a cero en los dos.
- **Pruebas:** Dos clientes y un ciclo completo de un pickup de salud.
- **Estado:** BACKLOG
- **Subtareas:**
  - F6-07a Estado del spawner.
  - F6-07b Fin de efecto.

### F7-01 Menú principal

- **ID:** F7-01
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Menú principal
- **Descripción:** Play, Private Match, Settings, Credits y Quit.
- **Objetivo:** RF-14.
- **Justificación:** GDD 18.1.
- **Dependencias:** F0-08, D-09
- **Archivos o sistemas:** 01_Scripts/UI, 03_Prefabs/UI
- **Skills o plugins:** ui, ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Botones que llaman a UGS.
- **Criterios de aceptación:**
  - Las cinco entradas existen.
  - Quit cierra el build.
  - Credits no promete ranked ni un arsenal futuro como si ya estuviera.
- **Pruebas:** Click de cada botón en Play Mode.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-01a Canvas.
  - F7-01b Navegación.

### F7-02 Crear y unirse

- **ID:** F7-02
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Crear y unirse
- **Descripción:** Pantalla de sesión con identificador y estado de conexión.
- **Objetivo:** RF-01.
- **Justificación:** GDD 18.1. Quick Match espera a que exista matchmaking, fuera de la demo.
- **Dependencias:** F3-02, F7-01
- **Archivos o sistemas:** UI de sesión
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Mostrar Quick Match como cola real.
- **Criterios de aceptación:**
  - Se puede crear sesión y ver el identificador.
  - Se puede pegar un identificador y unirse.
  - Un fallo de conexión muestra un estado, no una excepción vacía.
- **Pruebas:** Prueba de F3-02 desde la UI.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-02a Formulario.
  - F7-02b Estados de espera y error.

### F7-03 Selección de equipo y de monstruo

- **ID:** F7-03
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Selección de equipo y de monstruo
- **Descripción:** Elegir bando. El bando monstruo ve modelo y cifras del definition.
- **Objetivo:** RF-02.
- **Justificación:** GDD 18.1.
- **Dependencias:** F4-03, F7-02
- **Archivos o sistemas:** MonsterSelectionUI
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** La UI escribe el equipo saltándose la autoridad.
- **Criterios de aceptación:**
  - La elección llama a la petición de F4-03.
  - Si la autoridad rechaza, la UI vuelve al valor anterior.
  - Las cifras mostradas son las del asset.
- **Pruebas:** Intento legal e intento ilegal con dos clientes.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-03a Equipo.
  - F7-03b Lista de monstruos.

### F7-04 HUD de salud, armadura y munición

- **ID:** F7-04
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** HUD de salud, armadura y munición
- **Descripción:** Valores actuales, sin escribir el estado.
- **Objetivo:** RF-14.
- **Justificación:** GDD 18.2.
- **Dependencias:** F2-02, F2-09, F3-06
- **Archivos o sistemas:** HUD
- **Skills o plugins:** ui-ugui, optimize-text-mesh-pro si la fuente lo pide
- **Responsable:** UI/UX Developer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** El HUD resta vida en local y desincroniza el servidor.
- **Criterios de aceptación:**
  - El número de salud coincide con HealthComponent.
  - La munición coincide con el arma activa.
  - La armadura muestra el valor de red, aunque sea 0 hasta F2-03.
- **Pruebas:** Dañar y disparar en dos clientes y comparar cifras.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-04a Bind de eventos.
  - F7-04b Presentación.

### F7-05 Retícula

- **ID:** F7-05
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Retícula
- **Descripción:** Visible, legible y con tamaño configurable.
- **Objetivo:** GDD 18.2.
- **Justificación:** No hay tamaño canónico.
- **Dependencias:** F1-02
- **Archivos o sistemas:** HUD
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Retícula que se confunde con el entorno gris.
- **Criterios de aceptación:**
  - Se ve en el centro.
  - Un campo cambia su escala.
  - No bloquea el rayo de la escopeta.
- **Pruebas:** Play Mode de la arena gris.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-05a Elemento.
  - F7-05b Escala.

### F7-06 Indicador de potenciador

- **ID:** F7-06
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Indicador de potenciador
- **Descripción:** Icono y tiempo restante del efecto activo.
- **Objetivo:** RF-09.
- **Justificación:** GDD 18.2.
- **Dependencias:** F6-03, F6-07
- **Archivos o sistemas:** HUD
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Mostrar un temporizador local distinto del de la autoridad.
- **Criterios de aceptación:**
  - Con efecto activo se ve el tiempo que dicta la autoridad.
  - Al expirar, el icono desaparece en los dos clientes.
- **Pruebas:** Caso de F6-07 mirando el HUD.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-06a Icono.
  - F7-06b Tiempo restante.

### F7-07 Marcador y feed

- **ID:** F7-07
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Marcador y feed
- **Descripción:** Bajas, muertes, puntuación, tiempo y feed de atacante, víctima y arma.
- **Objetivo:** RF-06 y RF-12.
- **Justificación:** GDD 11.2 y 18.2. Latencia en el marcador solo si el dato existe.
- **Dependencias:** F3-07, F3-10, F9-03
- **Archivos o sistemas:** ScoreboardUI
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** El cliente incrementa las bajas por su cuenta.
- **Criterios de aceptación:**
  - Una muerte autoritativa añade una línea de feed.
  - Los dos clientes muestran la misma puntuación.
  - No aparece una segunda línea por el mismo evento.
- **Pruebas:** F3-16.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-07a Marcador.
  - F7-07b Feed.

### F7-08 Resultados y revancha

- **ID:** F7-08
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Resultados y revancha
- **Descripción:** Ganador, puntuación, duración y acción de revancha.
- **Objetivo:** RF-12.
- **Justificación:** GDD 18.1.
- **Dependencias:** F9-05, F7-07
- **Archivos o sistemas:** UI de resultados
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Cada cliente declara un ganador distinto.
- **Criterios de aceptación:**
  - El ganador mostrado es el calculado por la autoridad.
  - Revancha crea o reutiliza la sesión según la regla de F9-05.
  - Empate, si las reglas lo permiten, no muestra un ganador falso.
- **Pruebas:** Forzar fin de partida en dos clientes.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-08a Panel.
  - F7-08b Revancha.

### F7-09 Ajustes

- **ID:** F7-09
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Ajustes
- **Descripción:** Sensibilidad, invertir eje, FOV, volúmenes, calidad, gore y reducción de flashes y de movimiento de cámara.
- **Objetivo:** RF-15 y RA-02.
- **Justificación:** GDD 18.1 y 18.3. Los subtítulos de avisos entran aquí.
- **Dependencias:** F7-01, F1-02
- **Archivos o sistemas:** UI de Settings, opciones de VFX
- **Skills o plugins:** ui-ugui, audio-setup-mixers cuando haya mixer
- **Responsable:** UI/UX Developer
- **Prioridad:** P2
- **Complejidad:** M
- **Riesgos:** Guardar opciones en un sitio que pise el estado de la partida.
- **Criterios de aceptación:**
  - Cambiar la sensibilidad altera Look.
  - Invertir el eje lo invierte.
  - El FOV de la cámara cambia con el control.
  - Bajar gore oculta decals nuevos sin tocar el daño.
- **Pruebas:** Play Mode de cada control.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-09a Controles.
  - F7-09b Persistencia local de opciones, separada del estado de red.

### F7-10 Errores de conexión

- **ID:** F7-10
- **Fase:** 7
- **Epic:** E07 UI
- **Nombre:** Errores de conexión
- **Descripción:** Fallo al crear, fallo al unir, timeout de carga y desconexión tienen mensaje.
- **Objetivo:** RF-14.
- **Justificación:** GDD 23: los errores previsibles tienen respuesta.
- **Dependencias:** F3-02, F3-11, F7-02
- **Archivos o sistemas:** UI de sesión
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Solo un log de consola.
- **Criterios de aceptación:**
  - Un identificador rechazado muestra error en pantalla.
  - La desconexión del remoto muestra el estado de F3-11.
  - El timeout de F3-15 tiene texto.
- **Pruebas:** Provocar cada fallo en una prueba de dos clientes.
- **Estado:** BACKLOG
- **Subtareas:**
  - F7-10a Mensajes.
  - F7-10b Enlace con los eventos de sesión.

### F8-01 Materiales base y volumen

- **ID:** F8-01
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Materiales base y volumen
- **Descripción:** Metal, contraste y luz roja con pocos materiales compartidos.
- **Objetivo:** RV-01.
- **Justificación:** GDD 15.1 y 15.3. El MVP puede seguir en gris.
- **Dependencias:** F5-02
- **Archivos o sistemas:** 05_ArtModels/Materials, Volume de M01
- **Skills o plugins:** urp-postprocessing
- **Responsable:** Technical Artist
- **Prioridad:** P2
- **Complejidad:** M
- **Riesgos:** Una luz dinámica por prop.
- **Criterios de aceptación:**
  - M01 usa un conjunto pequeño de materiales reutilizados.
  - Hay una dirección de luz roja legible en movimiento.
  - El blockout gris de F5-01 no se borra.
- **Pruebas:** Recorrer M01 y contar materiales en el frame debugger cuando el Editor exista.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-01a Materiales.
  - F8-01b Volume.

### F8-02 Humano, manos FPS y silueta remota

- **ID:** F8-02
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Humano, manos FPS y silueta remota
- **Descripción:** Un modelo con manos en primera persona y cuerpo para el resto.
- **Objetivo:** RV-03 y RV-04.
- **Justificación:** GDD 15.2.
- **Dependencias:** F0-11, F1-02
- **Archivos o sistemas:** 05_ArtModels/Characters
- **Skills o plugins:** Ninguna de rigging. asset-transformer-toolkit solo si el paquete existe.
- **Responsable:** 3D Artist / Animator
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Manos que se separan de la cámara al disparar.
- **Criterios de aceptación:**
  - El local ve manos y arma, no el cuerpo completo en su cámara.
  - El remoto ve el cuerpo.
  - La licencia está en el registro si el modelo no es propio.
- **Pruebas:** Dos clientes cuando F3-04 exista; hasta entonces, una vista local y un prefab remoto en escena.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-02a Manos.
  - F8-02b Cuerpo remoto.

### F8-03 Dos monstruos

- **ID:** F8-03
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Dos monstruos
- **Descripción:** Modelos del ligero y del pesado, o uno solo si D-12 lo reduce.
- **Objetivo:** RV-04.
- **Justificación:** GDD 15.2 y checklist de monstruos.
- **Dependencias:** F4-02, F0-11
- **Archivos o sistemas:** 05_ArtModels/Characters
- **Skills o plugins:** Ninguna
- **Responsable:** 3D Artist / Animator
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Dos mallas con el mismo hitbox sin ficha.
- **Criterios de aceptación:**
  - Cada definition aprobado tiene prefab.
  - La silueta se distingue del metal del mapa a distancia de combate.
- **Pruebas:** Colocarlos en M01 y mirarlos desde un spawn.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-03a Modelo.
  - F8-03b Prefab.

### F8-04 Modelos de escopeta y lanzacohetes

- **ID:** F8-04
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Modelos de escopeta y lanzacohetes
- **Descripción:** Vista FPS y modelo de mundo.
- **Objetivo:** RV-04.
- **Justificación:** GDD 15.2.
- **Dependencias:** F2-06, F0-11
- **Archivos o sistemas:** 05_ArtModels y prefabs de Weapons
- **Skills o plugins:** Ninguna
- **Responsable:** 3D Artist / Animator
- **Prioridad:** P2
- **Complejidad:** M
- **Riesgos:** El modelo de mundo tapa la retícula.
- **Criterios de aceptación:**
  - Cada arma de demo tiene vista y mundo.
  - La vista permanece estable con la cámara quieta.
- **Pruebas:** Inspección en primera persona.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-04a Vista.
  - F8-04b Mundo.

### F8-05 Kit de M01

- **ID:** F8-05
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Kit de M01
- **Descripción:** Paredes, suelo, puertas, columnas, plataformas, rampas y tuberías.
- **Objetivo:** RV-01.
- **Justificación:** GDD 15.2. La decoración de cadenas y cadáveres es prioridad media y puede recortarse.
- **Dependencias:** F5-02, F8-01
- **Archivos o sistemas:** 05_ArtModels/Environment
- **Skills o plugins:** urp-postprocessing
- **Responsable:** Technical Artist
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Sustituir colliders buenos por mallas cóncavas caras.
- **Criterios de aceptación:**
  - La lectura de la ruta sobrevive al cambio de malla.
  - Los colliders de juego no se sustituyen por la malla visual si eso rompe F1-09.
- **Pruebas:** Repetir F5-08 después del arte.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-05a Kit modular.
  - F8-05b Conservar colisión.

### F8-06 VFX de disparo, sangre y decals con tope

- **ID:** F8-06
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** VFX de disparo, sangre y decals con tope
- **Descripción:** Pool, límite de decals y opción de reducción. Sin desmembramiento.
- **Objetivo:** RV-02 y RNF-02.
- **Justificación:** GDD 9.
- **Dependencias:** F2-11, F7-09
- **Archivos o sistemas:** VFXPool
- **Skills o plugins:** urp-postprocessing
- **Responsable:** Technical Artist
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** K-09. Efectos en red como NetworkObject.
- **Criterios de aceptación:**
  - Disparo, impacto, explosión y sangre salen del pool.
  - Al pasar el tope, el decal más viejo se recicla.
  - Con gore al mínimo no se crean decals nuevos.
  - El daño no depende de la partícula.
- **Pruebas:** Ráfaga de disparos y comprobación del contador del pool.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-06a Pool.
  - F8-06b Topes.
  - F8-06c Evento de red, no objeto de red por gota.

### F8-07 Set de audio de combate

- **ID:** F8-07
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Set de audio de combate
- **Descripción:** Disparo, explosión, pasos, salto, impacto, muerte, pickup, UI, ambiente y una pieza musical.
- **Objetivo:** RA-01, RA-02, RA-03.
- **Justificación:** GDD sección 17. La lista completa puede recortarse; los disparos y los pasos no, porque son señales.
- **Dependencias:** F2-07, F2-08, F0-11
- **Archivos o sistemas:** 04_Audio, AudioManager
- **Skills o plugins:** audio-setup-mixers, optimize-audio
- **Responsable:** Technical Artist
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Un sonido de ambiente que se confunde con un disparo.
- **Criterios de aceptación:**
  - La escopeta y el cohete suenan distinto.
  - El salto suena al dejar el suelo.
  - Los tres volúmenes de F7-09 mueven sus grupos.
  - Dos disparos seguidos no usan exactamente el mismo tono si hay variación configurada.
- **Pruebas:** Pasada con los ojos cerrados no es un criterio. El criterio es identificar arma y salto en una grabación de la prueba.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-07a Clips con licencia.
  - F8-07b Mixer.
  - F8-07c Disparos enlazados al evento, no al fin de la animación.

### F8-08 Animación mínima de presentación

- **ID:** F8-08
- **Fase:** 8
- **Epic:** E08 Arte y audio
- **Nombre:** Animación mínima de presentación
- **Descripción:** Idle, movimiento, salto, disparo, melee y muerte. El resto de la sección 16 queda listado como corte si no cabe.
- **Objetivo:** RT-08.
- **Justificación:** El GDD separa la lista completa del presupuesto. La sección 22 recorta visual antes que red.
- **Dependencias:** F8-02, F8-03, F4-05
- **Archivos o sistemas:** Animators
- **Skills o plugins:** Ninguna
- **Responsable:** 3D Artist / Animator
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Aplicar daño en un animation event.
- **Criterios de aceptación:**
  - El remoto cambia de idle a movimiento con la velocidad de red.
  - Fire y Attack no modifican la salud.
  - Dead se activa con el estado autoritativo.
  - Los clips ausentes de la sección 16 quedan nombrados en PROGRESS como corte, no como hechos.
- **Pruebas:** Dos clientes y una muerte.
- **Estado:** BACKLOG
- **Subtareas:**
  - F8-08a Parámetros del GDD.
  - F8-08b Clips mínimos.
  - F8-08c Lista de cortes.

### F9-01 Cerrar el modo de la demo

- **ID:** F9-01
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Cerrar el modo de la demo
- **Descripción:** Elegir Versus Campaign, Versus Continuous, o los dos en secuencia.
- **Objetivo:** D-04 y D-14.
- **Justificación:** Las secciones 3.2, 22 y 25 no dicen lo mismo.
- **Dependencias:** David
- **Archivos o sistemas:** Documentation/DECISIONS.md, GameModeDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Implementar los dos modos completos y no terminar ninguno.
- **Criterios de aceptación:**
  - D-04 y D-14 quedan tomadas.
  - GameModeDefinition refleja solo lo elegido.
  - El MVP puede seguir en una sesión libre mientras esto no esté, sin llamarlo modo final.
- **Pruebas:** Texto de decisión.
- **Estado:** BLOCKED
- **Subtareas:**
  - F9-01a David elige.
  - F9-01b Asset de modo.

### F9-02 Reglas de equipo de la demo

- **ID:** F9-02
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Reglas de equipo de la demo
- **Descripción:** Tamaño 2 contra 2 para el prototipo. 4 contra 4 no se exige aquí.
- **Objetivo:** RF-02.
- **Justificación:** GDD 3.3. D-05 sigue abierto para el objetivo final.
- **Dependencias:** F3-05, F9-01
- **Archivos o sistemas:** VersusManager, GameModeDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Empezar exigiendo ocho jugadores.
- **Criterios de aceptación:**
  - La partida de prueba arranca con dos y dos, o con menos si alguien falta, según F3-11.
  - No exige cuatro y cuatro.
  - Las armas se reinician al cambiar de mapa.
- **Pruebas:** Sesión 2v2.
- **Estado:** BACKLOG
- **Subtareas:**
  - F9-02a Cupo.
  - F9-02b Reinicio de carga.

### F9-03 Puntuación

- **ID:** F9-03
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Puntuación
- **Descripción:** La autoridad suma bajas y, si el modo lo dice, progreso de recorrido.
- **Objetivo:** RF-12.
- **Justificación:** GDD 11.2. La combinación exacta espera D-14.
- **Dependencias:** F9-01, F3-10, F5-07
- **Archivos o sistemas:** VersusManager
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Puntos por daño infligido. El GDD de ranked desaconseja ese incentivo y la demo tampoco lo pide.
- **Criterios de aceptación:**
  - Una muerte confirmada suma la baja que diga el modo.
  - Llegar a la puerta suma el progreso solo si D-14 lo incluye.
  - El suicidio sigue la regla escrita; si no hay regla, no suma y queda anotado como hueco.
- **Pruebas:** EditMode de dos eventos de puntuación.
- **Estado:** BACKLOG
- **Subtareas:**
  - F9-03a Eventos.
  - F9-03b Totales autoritativos.

### F9-04 Intercambio de roles

- **ID:** F9-04
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Intercambio de roles
- **Descripción:** Los equipos recorren otra vez con el rol contrario y se comparan.
- **Objetivo:** RF-13.
- **Justificación:** Solo si F9-01 elige campaña.
- **Dependencias:** F9-01, F9-03, F5-07
- **Archivos o sistemas:** VersusManager
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P2
- **Complejidad:** L
- **Riesgos:** Construir el intercambio cuando el modo elegido sea el continuo.
- **Criterios de aceptación:**
  - Si el modo no intercambia roles, esta tarea se cierra como fuera de alcance con la cita de F9-01, no como DONE de funcionalidad.
  - Si sí intercambia, el segundo recorrido usa los equipos contrarios y conserva la comparación.
- **Pruebas:** Partida de dos recorridos o nota de alcance.
- **Estado:** BACKLOG
- **Subtareas:**
  - F9-04a Depender de F9-01.
  - F9-04b Segundo recorrido solo si aplica.

### F9-05 Fin de partida y revancha

- **ID:** F9-05
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Fin de partida y revancha
- **Descripción:** La autoridad declara el final y ofrece otra partida.
- **Objetivo:** RF-12.
- **Justificación:** GDD 18.1.
- **Dependencias:** F9-03, F3-02
- **Archivos o sistemas:** VersusManager, MatchResultsService
- **Skills o plugins:** Ninguna
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** MatchResultsService guardando MMR.
- **Criterios de aceptación:**
  - Ambos clientes reciben el mismo resultado.
  - Revancha no duplica la puntuación anterior.
  - No se escribe MMR.
- **Pruebas:** Dos clientes hasta el fin.
- **Estado:** BACKLOG
- **Subtareas:**
  - F9-05a Condición de fin.
  - F9-05b Revancha limpia.

### F9-06 Validación 4v4

- **ID:** F9-06
- **Fase:** 9
- **Epic:** E09 Modos
- **Nombre:** Validación 4v4
- **Descripción:** Probar ocho jugadores solo después de que 2v2 sea estable.
- **Objetivo:** D-05 y sección 22.
- **Justificación:** Es objetivo condicionado, no parte del MVP.
- **Dependencias:** F3-13, F4-08, F10-04, D-05
- **Archivos o sistemas:** VersusManager
- **Skills o plugins:** Profiler de Unity y herramientas de Fusion
- **Responsable:** QA Engineer
- **Prioridad:** P3
- **Complejidad:** XL
- **Riesgos:** Romper el tick de red por duplicar jugadores antes de tiempo.
- **Criterios de aceptación:**
  - Hay una sesión de ocho jugadores o una nota de que D-05 la deja fuera.
  - Si se juega, quedan FPS, tráfico y el fallo concreto si no llega a 60.
- **Pruebas:** Sesión real de ocho, o decisión de corte.
- **Estado:** BACKLOG
- **Subtareas:**
  - F9-06a Confirmar D-05.
  - F9-06b Sesión solo si sigue dentro.

### F10-01 Matriz funcional

- **ID:** F10-01
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Matriz funcional
- **Descripción:** Movimiento, armas, salud, ítems, puertas y UI según la sección 21.1.
- **Objetivo:** RC-01.
- **Justificación:** GDD 21.1.
- **Dependencias:** F1-10, F2-12, F6-07, F5-08, F7-10
- **Archivos o sistemas:** Documentation/PROGRESS.md
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Marcar casillas sin ejecutarlas.
- **Criterios de aceptación:**
  - Cada fila de 21.1 tiene resultado observado o bloqueo.
  - No hay filas en verde por ausencia de bugs conocidos.
- **Pruebas:** Ejecución de las pruebas de las tareas citadas.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-01a Plantilla.
  - F10-01b Pasada.

### F10-02 Matriz de red

- **ID:** F10-02
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Matriz de red
- **Descripción:** Las ocho situaciones de la sección 21.2.
- **Objetivo:** RN-06.
- **Justificación:** GDD 21.2.
- **Dependencias:** F3-12, F3-13, F3-14, F3-15, F3-16, F3-17
- **Archivos o sistemas:** PROGRESS.md
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** L
- **Riesgos:** Sustituir la matriz por la prueba de dos ventanas locales.
- **Criterios de aceptación:**
  - Cada fila tiene fecha, máquinas y resultado.
  - La fila de latencia cita la herramienta y el valor.
- **Pruebas:** Las pruebas F3 correspondientes.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-02a Filas.
  - F10-02b Huecos explícitos.

### F10-03 Balance

- **ID:** F10-03
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Balance
- **Descripción:** Las seis observaciones de la sección 21.3.
- **Objetivo:** RJ-04, RJ-05, RJ-08.
- **Justificación:** GDD 21.3.
- **Dependencias:** F4-08, F5-08, F6-03
- **Archivos o sistemas:** Assets de tuning
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Cambiar cifras durante la partida y perder la referencia.
- **Criterios de aceptación:**
  - Cada observación tiene una nota de partida.
  - Los ajustes posteriores quedan en los ScriptableObjects con el valor anterior anotado.
- **Pruebas:** Partidas 2v2.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-03a Observar.
  - F10-03b Ajustar assets.

### F10-04 Rendimiento

- **ID:** F10-04
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Rendimiento
- **Descripción:** Medir CPU, GPU, memoria y tráfico en el PC de F0-10 con el máximo de jugadores de la demo.
- **Objetivo:** RNF-01.
- **Justificación:** GDD 20. No optimizar antes de medir.
- **Dependencias:** F0-10, F8-06, F3-13
- **Archivos o sistemas:** Profiler
- **Skills o plugins:** unity-cli cuando pueda lanzar el perfilado
- **Responsable:** Technical Architect
- **Prioridad:** P1
- **Complejidad:** L
- **Riesgos:** Recortar el movimiento para ganar FPS antes de mirar el profiler.
- **Criterios de aceptación:**
  - Hay una captura con el PC de D-15, la escena y el número de jugadores.
  - Se anota si se sostiene 60 FPS.
  - Si no se sostiene, el recorte propuesto es visual, salvo que el movimiento sea el coste medido.
- **Pruebas:** Profiler en una partida real.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-04a Medir.
  - F10-04b Decidir recorte con el dato.

### F10-05 Build de Windows

- **ID:** F10-05
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Build de Windows
- **Descripción:** Player de desarrollo o release que no depende del Editor para existir.
- **Objetivo:** RNF-05.
- **Justificación:** GDD sección 26.
- **Dependencias:** F10-01
- **Archivos o sistemas:** grinder/Builds/
- **Skills o plugins:** unity-cli
- **Responsable:** Technical Architect
- **Prioridad:** P1
- **Complejidad:** M
- **Riesgos:** Build de 2020.3 o de otra versión de editor.
- **Criterios de aceptación:**
  - El player se genera con 6000.5.6f1.
  - La carpeta de build no se versiona, según .gitignore.
- **Pruebas:** El comando de build termina sin error. El log se guarda en PROGRESS.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-05a Configurar la escena de arranque.
  - F10-05b Construir.

### F10-06 Arranque fuera del Editor

- **ID:** F10-06
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Arranque fuera del Editor
- **Descripción:** El player abre el menú en una máquina sin Unity Editor.
- **Objetivo:** Sección 26.
- **Justificación:** GDD 26.
- **Dependencias:** F10-05, F7-01
- **Archivos o sistemas:** Build
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** S
- **Riesgos:** Probar el exe en la misma máquina y creer que el Editor no interviene.
- **Criterios de aceptación:**
  - El exe muestra el menú.
  - Se puede crear o unir sesión desde ese exe.
  - El fallo, si lo hay, queda con texto.
- **Pruebas:** Ejecutar el player y anotarlo.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-06a Copiar el build.
  - F10-06b Arrancarlo.

### F10-07 README de instalación y controles

- **ID:** F10-07
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** README de instalación y controles
- **Descripción:** Cómo abrir el proyecto, qué editor hace falta, cómo entrar en una sesión y qué teclas usa el movimiento.
- **Objetivo:** Sección 26.
- **Justificación:** El repositorio no tiene README de juego.
- **Dependencias:** F1-03, F7-02, F10-05
- **Archivos o sistemas:** README.md
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P1
- **Complejidad:** S
- **Riesgos:** Documentar controles que el input no tiene.
- **Criterios de aceptación:**
  - El README cita 6000.5.6f1 y la versión de Fusion realmente instalada.
  - Lista Move, Look, Jump y disparo con las teclas del asset.
  - Lista las limitaciones conocidas abiertas.
- **Pruebas:** Seguir el README en limpio hasta el menú.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-07a Instalación.
  - F10-07b Controles y límites.

### F10-08 Cierre por Definition of Done

- **ID:** F10-08
- **Fase:** 10
- **Epic:** E10 QA
- **Nombre:** Cierre por Definition of Done
- **Descripción:** Cada tarea DONE de juego cumple la sección 23.
- **Objetivo:** RC-01.
- **Justificación:** GDD 23 y 26.
- **Dependencias:** F10-01, F10-02, F10-06
- **Archivos o sistemas:** MASTER_CHECKLIST.md, PROGRESS.md
- **Skills o plugins:** Ninguna
- **Responsable:** QA Engineer
- **Prioridad:** P0
- **Complejidad:** M
- **Riesgos:** Cerrar la demo con tareas en REVIEW.
- **Criterios de aceptación:**
  - No queda ninguna tarea de la demo en DONE sin prueba citada.
  - Los errores críticos están corregidos o listados.
  - La latencia simulada tiene una pasada.
- **Pruebas:** Lectura del checklist contra PROGRESS.
- **Estado:** BACKLOG
- **Subtareas:**
  - F10-08a Auditar estados.
  - F10-08b Lista de errores abiertos.

### R-01 Fórmula de MMR y reglas congeladas

- **ID:** R-01
- **Fase:** Posterior
- **Epic:** E11 Ranked
- **Nombre:** Fórmula de MMR y reglas congeladas
- **Descripción:** Documentar el algoritmo solo cuando la demo haya terminado y David abra ranked.
- **Objetivo:** Evitar ranked prematuro.
- **Justificación:** GDD 12. K-10.
- **Dependencias:** F10-08, David
- **Archivos o sistemas:** Documentation/DECISIONS.md
- **Skills o plugins:** No build-live-game salvo aprobación explícita de backend
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Guardar MMR en PlayerPrefs.
- **Criterios de aceptación:**
  - La tarea no se implementa durante la demo.
  - Si se abre, la fórmula vive en servidor y tiene versión.
- **Pruebas:** Revisión de diseño. Sin código en la demo.
- **Estado:** BACKLOG
- **Subtareas:**
  - R-01a Mantenerla fuera del sprint de demo.
  - R-01b Especificar solo con aprobación.

### R-02 Identidad y backend de resultados

- **ID:** R-02
- **Fase:** Posterior
- **Epic:** E11 Ranked
- **Nombre:** Identidad y backend de resultados
- **Descripción:** Persistencia de jugador, historial y resultados firmados por la autoridad.
- **Objetivo:** GDD 12.4.
- **Justificación:** Sin backend de confianza, ranked sigue fuera.
- **Dependencias:** R-01
- **Archivos o sistemas:** Servicio externo todavía no elegido
- **Skills o plugins:** build-live-game solo si se elige UGS por escrito
- **Responsable:** Technical Architect
- **Prioridad:** P3
- **Complejidad:** XL
- **Riesgos:** Elegir UGS por inercia de una skill.
- **Criterios de aceptación:**
  - El backend está nombrado por David.
  - Un cliente no puede enviarse su propio MMR.
- **Pruebas:** Prueba de manipulación descrita en el GDD, cuando exista el servicio.
- **Estado:** BACKLOG
- **Subtareas:**
  - R-02a Elegir backend.
  - R-02b Rechazar escrituras de cliente.

### R-03 Matchmaking y abandonos

- **ID:** R-03
- **Fase:** Posterior
- **Epic:** E11 Ranked
- **Nombre:** Matchmaking y abandonos
- **Descripción:** Cola por MMR, límite de latencia y partidas inválidas por abandono.
- **Objetivo:** GDD 12.4.
- **Justificación:** La demo usa sala privada.
- **Dependencias:** R-02
- **Archivos o sistemas:** Servicio de cola
- **Skills o plugins:** No setup-multiplayer-services mientras la red de juego sea Fusion, salvo una decisión nueva
- **Responsable:** Multiplayer Engineer
- **Prioridad:** P3
- **Complejidad:** XL
- **Riesgos:** Mezclar el matchmaking de UGS con una sesión Fusion sin diseño.
- **Criterios de aceptación:**
  - La cola no entra en el botón Play de la demo.
  - Un abandono marca la partida según la regla que se escriba entonces.
- **Pruebas:** Fuera de la demo.
- **Estado:** BACKLOG
- **Subtareas:**
  - R-03a Dejar Quick Match desactivado.
  - R-03b Especificar la cola en su fase.

### R-04 Temporadas, rangos y recompensas cosméticas

- **ID:** R-04
- **Fase:** Posterior
- **Epic:** E11 Ranked
- **Nombre:** Temporadas, rangos y recompensas cosméticas
- **Descripción:** Bronce a Infernal, reinicio parcial y recompensas sin ventaja de combate.
- **Objetivo:** GDD 12.1 y 12.3.
- **Justificación:** Pay-to-win está fuera.
- **Dependencias:** R-02
- **Archivos o sistemas:** Datos de temporada
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Una recompensa que suba el daño.
- **Criterios de aceptación:**
  - Las recompensas no modifican WeaponDefinition ni MonsterDefinition.
  - La temporada tiene duración configurable.
- **Pruebas:** Revisión de datos. Sin implementación en la demo.
- **Estado:** BACKLOG
- **Subtareas:**
  - R-04a Tabla de rangos ya escrita en el GDD.
  - R-04b Prohibir ventaja de combate.

### P-01 Menú de pausa

- **ID:** P-01
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Menú de pausa
- **Descripción:** Pausa local de la simulación. No está en la sección 18.
- **Objetivo:** Cubrir un hueco de UX si David lo pide.
- **Justificación:** El GDD no lo lista. En red, pausar el mundo afecta a otros jugadores.
- **Dependencias:** David
- **Archivos o sistemas:** UI
- **Skills o plugins:** ui-ugui
- **Responsable:** UI/UX Developer
- **Prioridad:** P3
- **Complejidad:** M
- **Riesgos:** Pausar el servidor desde un cliente.
- **Criterios de aceptación:**
  - Sigue fuera de la demo mientras no haya decisión.
  - Si se aprueba, solo puede pausar una partida local de un jugador, no una sesión con otra persona.
- **Pruebas:** No se prueba hasta que exista la decisión.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-01a No implementarla.
  - P-01b Pedir decisión si hace falta.

### P-02 Bots de desconexión

- **ID:** P-02
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Bots de desconexión
- **Descripción:** El GDD cita bots como alternativa y elige no implementarlos en la demo.
- **Objetivo:** Mantener la regla simple de T-12.
- **Justificación:** GDD 11.4.
- **Dependencias:** F10-08
- **Archivos o sistemas:** Ninguno en la demo
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Un bot que juega mejor que una persona y altera el resultado.
- **Criterios de aceptación:**
  - La demo no contiene bots.
  - Cualquier bot futuro no suma puntuación sin una regla nueva.
- **Pruebas:** Inspección de que no hay agente automático.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-02a Confirmar ausencia.
  - P-02b Dejar la tarea fuera del MVP.

### P-03 Barriles explosivos

- **ID:** P-03
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Barriles explosivos
- **Descripción:** Daño de área de entorno, marcado como futuro en la sección 5.2.
- **Objetivo:** No meterlos en M01 por ambientación.
- **Justificación:** GDD 5.2.
- **Dependencias:** F10-08
- **Archivos o sistemas:** Ninguno
- **Skills o plugins:** physics-3d-collision
- **Responsable:** Gameplay Programmer
- **Prioridad:** P3
- **Complejidad:** M
- **Riesgos:** Un barril que empuja al jugador sin regla de desplazamiento.
- **Criterios de aceptación:**
  - No hay barriles en la demo.
  - Si se aprueban, el daño pasa por DamageResolver.
- **Pruebas:** No se prueba en la demo.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-03a Mantenerlos fuera.
  - P-03b Reusar el resolver si algún día entran.

### P-04 Desmembramiento por piezas

- **ID:** P-04
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Desmembramiento por piezas
- **Descripción:** Expansión de gore, no simulación anatómica.
- **Objetivo:** Reservar el coste.
- **Justificación:** GDD 9.1.
- **Dependencias:** F8-06, F10-04
- **Archivos o sistemas:** VFX
- **Skills o plugins:** Ninguna
- **Responsable:** Technical Artist
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** K-09.
- **Criterios de aceptación:**
  - La demo usa muerte animada o ragdoll simple, no piezas.
  - Si se aprueba, tiene pool y no decide el daño.
- **Pruebas:** No forma parte de F10-01.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-04a Fuera de la demo.
  - P-04b Presupuesto propio si se reabre.

### P-05 Ametralladora, superescopeta y rayo

- **ID:** P-05
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Ametralladora, superescopeta y rayo
- **Descripción:** Arsenal futuro de la sección 6.
- **Objetivo:** No adelantar armas.
- **Justificación:** T-07.
- **Dependencias:** F10-08
- **Archivos o sistemas:** WeaponDefinition
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Una tercera arma que rompe el tiempo de la demo.
- **Criterios de aceptación:**
  - La demo solo equipa escopeta y lanzacohetes.
  - Un arma futura sería otro WeaponDefinition, sin otra clase de daño.
- **Pruebas:** Inspección del catálogo de armas.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-05a Catálogo de demo con dos armas.
  - P-05b Nuevas fichas solo tras la demo.

### P-06 Asistencias

- **ID:** P-06
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Asistencias
- **Descripción:** El GDD las deja para una iteración posterior.
- **Objetivo:** No complicar el feed.
- **Justificación:** GDD 5.3 y 11.2.
- **Dependencias:** F7-07
- **Archivos o sistemas:** Scoreboard
- **Skills o plugins:** Ninguna
- **Responsable:** Gameplay Programmer
- **Prioridad:** P3
- **Complejidad:** M
- **Riesgos:** Doble conteo de una ayuda y una baja.
- **Criterios de aceptación:**
  - El feed de la demo no muestra asistencias.
  - Si se añaden, no sustituyen a la baja.
- **Pruebas:** Revisión del feed.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-06a Fuera del feed inicial.
  - P-06b Regla propia si se aprueba.

### P-07 Medidor de estilo

- **ID:** P-07
- **Fase:** Propuesta
- **Epic:** E12 Propuestas
- **Nombre:** Medidor de estilo
- **Descripción:** ULTRAKILL inspira el feedback, no su sistema de estilo completo.
- **Objetivo:** No convertir la demo en otro juego.
- **Justificación:** GDD sección 2.
- **Dependencias:** F10-08
- **Archivos o sistemas:** Ninguno
- **Skills o plugins:** Ninguna
- **Responsable:** Game Director
- **Prioridad:** P3
- **Complejidad:** L
- **Riesgos:** Un multiplicador de puntuación que cambie el modo antes de congelar las reglas.
- **Criterios de aceptación:**
  - No hay medidor de estilo en el HUD de la demo.
  - Cualquier versión futura no modifica el daño base.
- **Pruebas:** Revisión del HUD.
- **Estado:** BACKLOG
- **Subtareas:**
  - P-07a Fuera del HUD.
  - P-07b Reabrir solo con el núcleo validado.

