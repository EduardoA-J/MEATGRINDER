# Auditoría de implementación de MEATGRINDER

24 de noviembre de 2026. La revisión abarca Git 89dedc8, los cambios de interfaz y carga de esta pasada y los 130 IDs REQUIRED del [checklist maestro](MASTER_CHECKLIST.md).

Se leyó el [GDD/TDD v1.0](../MEATGRINDER_GDD_TDD.docx), incluidas sus tablas de requisitos, la [adenda de alcance](GDD_SCOPE_ADDENDUM.md), las [decisiones](DECISIONS.md), el plan y las reglas del repositorio. Se contrastaron con el manifiesto, los ensamblados, las escenas y los prefabs serializados, los ScriptableObjects, los recursos y las pruebas del proyecto. La adenda mantiene dentro del producto los objetivos que el GDD situaba después de la demo. Las decisiones posteriores de llaves, sprint y agacharse se aplican según su registro.

## Avance por tareas

<!-- AVANCE -->
**21,54 % de cierre acreditado (28/130).** La cobertura de tareas integradas o validadas llega al **45,38 % (59/130)**, incluidas las variantes locales identificadas en el índice.

| Épica | Total | VALIDATED | INTEGRATED | PARTIALLY_IMPLEMENTED | BLOCKED | NOT_STARTED |
|---|---:|---:|---:|---:|---:|---:|
| E00 Preparación | 12 | 8 | 1 | 2 | 0 | 1 |
| E01 Movimiento | 12 | 10 | 2 | 0 | 0 | 0 |
| E02 Combate | 15 | 6 | 9 | 0 | 0 | 0 |
| E03 Red | 17 | 1 | 0 | 0 | 1 | 15 |
| E04 Monstruos | 8 | 0 | 0 | 0 | 2 | 6 |
| E05 Mapas | 11 | 1 | 3 | 6 | 0 | 1 |
| E06 Ítems | 12 | 0 | 10 | 0 | 0 | 2 |
| E07 UI | 10 | 0 | 3 | 4 | 0 | 3 |
| E08 Arte y audio | 8 | 0 | 3 | 4 | 0 | 1 |
| E09 Modos | 6 | 0 | 0 | 0 | 1 | 5 |
| E10 QA | 8 | 1 | 0 | 2 | 0 | 5 |
| E11 Ranked | 4 | 0 | 0 | 0 | 0 | 4 |
| E12 Ampliaciones | 7 | 1 | 0 | 0 | 0 | 6 |
| **Total** | **130** | **28** | **31** | **18** | **4** | **49** |

No hay IDs en IMPLEMENTED ni IN_PROGRESS.
<!-- FIN_AVANCE -->

El denominador incluye preparación, QA, ampliaciones y Ranked. Cada ID del índice cuenta una vez. Los subítems históricos y los nombres de escena alternativos del GDD no crean tareas adicionales. El recuento se reproduce agrupando las filas del índice por la tercera columna (Estado) y comprobando que hay 130 IDs distintos. El porcentaje de **cierre acreditado** es VALIDATED / 130. Como dato de cobertura, se informa también (VALIDATED + INTEGRATED) / 130. El crédito de una tarea local no se transfiere a sus dependencias de red. Un recurso importado, la cobertura parcial o el trabajo en aislamiento no aportan a estos cocientes. Son porcentajes por número de tareas, sin estimación de esfuerzo, de calidad final ni fecha de lanzamiento.

Se conservan las validaciones históricas con resultado documentado y se indica cuándo la prueba pertenece a otra pasada. Los XML, capturas y el Windows build del 22 de noviembre citados en el historial no venían versionados en la copia inicial. Los resultados nuevos y sus límites están en [PROGRESS.md](PROGRESS.md).

## Escenas, prefabs y arquitectura

| Evidencia inspeccionada | Estado real | Trabajo faltante |
|---|---|---|
| Unity 6000.5.6f1, URP 17.5.0, Input System 1.20.0, uGUI 2.5.0 y ProBuilder 6.1.4. Fusion 2.1.4 instalado aparte | Startup ocupa el índice 0 y Boot sigue registrado. El constructor de Windows de producto selecciona Startup, Boot y M01–M04. SampleScene, Greybox y NetworkTest son escenas auxiliares. | Número de versión candidato, distribución y prueba en equipo sin Unity. |
| Startup.unity → GameBootstrap → LocalGameSession.prefab → Boot.unity, con SessionMenu y SessionLoadingScreen | La sesión persiste entre mapas. El menú uGUI presenta práctica, selección, ajustes, créditos, pausa y resultados. BuildSmokeProbe solo se activa por argumento explícito en Editor/Development. | Sala privada, matchmaking, selección de equipo/monstruo, conexión, feed y marcador de producto. Los resultados muestran contadores locales. |
| LocalPlayer.prefab → PlayerInputReader/PlayerLook/PlayerMovement y MovementTuning | Cápsula FPS, aceleración, aire, salto, sprint y agacharse. Se cablea Input System; existe prueba de bindings y colisión. F1-11 acredita la decisión de dash para esta fase. | Capturar valores con teclado/ratón físicos y probar movimiento remoto, reconciliación y latencia. |
| LocalPlayer.prefab → PlayerCombat/PlayerMelee → DamageResolver/HealthComponent; WeaponDefinition y prefabs ShotgunProduct, RocketProduct, RocketProjectile y CleaverProduct | Hitscan, proyectil, melee, mitigación, cobertura y feedback local. El cohete usa MuzzleSocket y autodaño ajustable. El jugador empieza y reaparece con melee. PlayerLife, PlayerVoidFall y DeathLoot coordinan daños de vacío, muerte, loot y respawn. | Autoridad de Host Mode, armas ampliadas P-05, asistencias P-06, gameplay de monstruos y balance. |
| Prefabs de pickups → PickupDefinition → PowerUpManager; IDamageModifiers e ISpeedScale; CombatHud | Salud, armadura, armas, reservas independientes, timers y seis poderes. Quad y Berserk llegan al mismo resolver y Haste al movimiento. El HUD refleja salud, armadura, munición, retícula y poderes. | Prueba de competencia y temporizadores en dos clientes, sincronización y antirrecogida doble. |
| M01–M04 → MapDefinition, GameBootstrap, LocalPlayer, LocalSpawnPoints, MapRouteState, RouteKey y KeyGate | Cada escena tiene spawns, recursos, puerta opcional y salida. Los marcadores de humano/monstruo están colocados. Rampa de M01, salas de M02, templo de M03 e islas de M04 son geometría navegable en las zonas verificadas. La última salida va a resultados locales y las transiciones reinician el equipo local. | Recorrer todas las rutas y puertas, validar spawns, spawn-kill, balance competitivo y perfiles. Falta conservar puntuación de campaña y sincronizar puertas/mapas. Los nombres de M02–M04 siguen T-27. |
| NetworkTest.unity → NetworkSessionController/NetworkPlayerSpawner y NetworkTestPlayer.prefab | Sonda de cubos Host/Client. La prueba de dos instancias registrada el 3 de noviembre acredita F3-02 únicamente. LocalPlayer usa LocalCombatAuthority y el flujo Boot no instancia un NetworkRunner. | F3-01 y spawn de jugador real, equipo, predicción, daño/hitscan, réplica de combate/pickups, transición, desconexión y pruebas LAN/internet. |
| Carpetas Monsters/GameModes y prefabs previstos; R-01…R-04 y P-02…P-07 | El bucle local no ofrece partidas asimétricas. Los blancos estáticos carecen de input de monstruo. Las carpetas reservadas no contienen implementaciones de MonsterDefinition, selección, modos ni Ranked. | Demonios jugables, reglas Continuous/Campaign, intercambio, victoria, 2v2/4v4, backend/MMR/temporadas, bots de desconexión, barriles, desmembramiento y medidor de estilo. |
| PresentationTheme, CombatAudio/CombatEffects, WeaponVisualRig y assets Product | Materiales y geometría propios de primera pasada, efectos con límites, audio sintético de combate y animaciones procedurales. Las vistas y el HUD consumen datos/eventos de gameplay. | Rig y animación remota, LOD, SFX de movimiento, ambiente/música y mezcla final, gore y perfiles de GPU. |

En el grafo de ensamblados, Combat y Movement son bases. Weapons toma Combat y Pickups usa Combat, Weapons y Movement. Players compone esos sistemas y el Input System. Core orquesta sesión y escenas sobre las dependencias de juego. Presentation lee sus estados y eventos. Combat consume la interfaz IDamageModifiers y no referencia Pickups. El subconjunto Networking está cableado a la sonda de Fusion. [Grafo de dependencias](DEPENDENCY_GRAPH.md)

Startup arranca la sesión persistente y solicita cargar el menú. El mismo flujo local asíncrono gobierna primera arena, siguiente mapa, reinicio y retorno a Boot. La pantalla uGUI vive en el prefab persistente y escucha el destino, la fase y los cambios del AsyncOperation. Los rótulos espaciales serializados de mapas y prefabs de blancos usan la fuente industrial y su material, incluidos los regenerados por las herramientas de Editor. El progreso se refiere a la carga de la escena que Unity está ejecutando. F3-15 sigue pendiente de sincronización entre clientes.

## Modelos

Se cotejó SHA-256 entre los cuatro archivos de models/ y sus copias en Assets/_Project/05_ArtModels/Imported. Las cuatro parejas coinciden. Los GUID enlazan tres FBX a TrainingTarget_0, _1 y _2. Los tres prefabs están instanciados en cada mapa de producto y usan collider, HealthComponent y TargetPresentation. El cuarto FBX carece de referencia en los mapas y los prefabs de juego inspeccionados.

| FBX | Vértices importados por Unity | Enlace en la escena |
|---|---:|---|
| 20261110234906_b0c92559 | 116.370 | TrainingTarget_0, M01–M04 |
| 20261111001453_9a59cb6b | 93.174 | TrainingTarget_1, M01–M04 |
| 20261111002122_967857ff | 115.869 | TrainingTarget_2, M01–M04 |
| 20261110235955_ece0d8da | 1.975.614 | Archivo importado de unos 42 MiB, fuera de los mapas y prefabs de juego inspeccionados |

La inspección de Unity 6000.5.6f1 confirmó un renderer estático por FBX, importación Generic, cero SkinnedMeshRenderer, cero Animator y cero clips. La vista previa D3D12 del FBX más denso registró una petición de unos 79 MB contra un límite de unos 17 MB del búfer de subida. Ese recurso queda fuera de los mapas. La verificación por GUID acredita uso de los otros tres como blancos locales. F0-11 sigue parcial porque falta registrar fuente y licencia de distribución de los cuatro FBX. F8-02 y F8-08 siguen parciales por rig, silueta remota, clips y LOD pendientes. F8-03 sigue sin iniciar como arte de monstruos jugables. [Registro de modelos y de procedencia](REFERENCE_REPOSITORIES_AUDIT.md)

## Matriz funcional del GDD/TDD, sección 21.1

| Área | Evidencia ejecutable | Límites para el cierre |
|---|---|---|
| Movimiento | MovementEditModeTests, MovementPlayModeTests y M01ShellPlayModeTests. Cubren aceleración, strafe/hop, colisión y rampa. | La prueba de entrada sintética confirma bindings y estado del dispositivo. Falta registrar la salida de movimiento y cámara con teclado/ratón físicos y la postura remota. |
| Armas | CombatEditModeTests/CombatPlayModeTests, ProductAssetsEditModeTests y ProductFlowPlayModeTests. Cubren cadencia, cambio, cobertura, proyectil y melee. | Balance y autoridad, arsenal ampliado, validación competitiva. |
| Salud | CombatEditModeTests, GameplayEditModeTests y ProductFlowPlayModeTests. Cubren daño, armadura preservada frente al vacío, muerte, loot y respawn. | Prueba dedicada de mitigación de armadura ante daño normal, muertes simultáneas, spawn-kill y respawn de red. |
| Ítems | GameplayEditModeTests y ProductFlowPlayModeTests. Cubren efecto, tuning y configuración en mapas reales. | Mismo pickup en dos clientes, expiración visible sincronizada y balance espacial. |
| Versus | Sonda de sesión independiente; el modo local de práctica carece de equipo, puntuación, intercambio y ganador. | D-04/D-12/D-13/D-14, integración Host Mode y pruebas 2v2 y 4v4. |
| Mapas | MapLayoutPlayModeTests y ProductFlowPlayModeTests. Cubren abrir los cuatro mapas y llave/puerta M01→M02. | Ruta íntegra de cada mapa, puntuación entre escenas y transición simultánea de clientes. |
| Interfaz | ProductFlowPlayModeTests. Cubre menú, selección, pausa, resultados y vuelta. El detalle de cargas, fuentes y feedback de esta pasada está en PROGRESS. | Navegación con dispositivos físicos, toda la matriz de accesibilidad y pantallas de producto pendientes. |
| Instalación y rendimiento | BuildWindows y BuildSmokeProbe. Historial local y resultados vigentes en PROGRESS. | Ejecutar en una máquina sin Editor, crear/unir sesión y medir CPU/GPU/memoria/tráfico con jugadores reales. |

## Prioridades pendientes

1. Cerrar las reglas y cifras de Versus y monstruos que siguen abiertas. F3-01/03/04/06/07/12 enlazan Boot con el primer gameplay en Host Mode y su prueba mínima de dos clientes.
2. Completar M01–M04, QA de spawns/rutas, puntuación entre mapas, selección de equipo/monstruo, marcador, feeds y errores de conexión.
3. Terminar arte, rig/LOD, mezcla, accesibilidad y arsenal ampliado. Perfilar el núcleo en hardware de referencia y comprobar el build en otra máquina antes del cierre.
4. Integrar Ranked, persistencia y ampliaciones sobre autoridad, modos y resultados ya verificados.

El [historial de progreso](PROGRESS.md) conserva las pasadas anteriores de movimiento, combate y presentación. Los datos de equilibrio, duraciones de animación y valores de mapa de la pasada local siguen sujetos a ajuste.
