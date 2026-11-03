# Requisitos extraídos del GDD

Fuente: `MEATGRINDER_GDD_TDD.docx`, leído completo el 2 de noviembre de 2026.
Trazabilidad: cada requisito apunta a tareas de [MASTER_CHECKLIST.md](MASTER_CHECKLIST.md). Un requisito con decisión abierta no autoriza a inventar el dato que falta.

## Funcionales

| ID | Requisito | Tareas |
|---|---|---|
| RF-01 | Crear sesión, unirse y salir. En la demo, Play es crear o unirse. Sala privada con mapa. | F3-02, F7-01, F7-02 |
| RF-02 | Asignar equipos humanos y monstruos. El servidor valida la selección. | F3-05, F4-03, F7-03, F9-02 |
| RF-03 | Spawn autorizado en zonas que no reciban disparo directo. | F3-03, F5-04, F5-08 |
| RF-04 | Movimiento en primera persona: aceleración, fricción, salto, gravedad, aire, strafe, bunny hop, rampas y escalones. | F1-01 a F1-10 |
| RF-05 | Escopeta y lanzacohetes configurados en `WeaponDefinition`. Munición limitada. Cambio de arma. Arma inicial y recogida. | F2-06 a F2-10 |
| RF-06 | `DamageInfo` único, salud, una muerte, respawn. Feed con atacante, víctima y arma. | F2-01, F2-02, F2-04, F2-12, F7-07 |
| RF-07 | Armadura que absorbe parte del daño. Fórmula sin definir. | F2-03, F6-02 |
| RF-08 | Pickups de salud y armadura en posición fija, con reaparición y sin doble recogida. | F6-01, F6-02, F6-04, F6-05 |
| RF-09 | Quad Damage en la semana 5. Efecto, duración y acumulación sin definir. | F6-03, F6-06, F7-06 |
| RF-10 | Monstruos seleccionables, hitbox coherente, ataque básico sin munición. | F4-01 a F4-07 |
| RF-11 | Puertas que cambian de mapa. El servidor ordena la carga. Se conserva la puntuación de campaña. Timeout y desconexión. | F5-06, F5-07, F3-15 |
| RF-12 | Puntuación por bajas y por recorrido. El servidor calcula el resultado. Revancha. | F9-03, F9-05, F7-07, F7-08 |
| RF-13 | Intercambio de roles para comparar recorridos. Contradice la prioridad de Versus Continuous. | F9-01, F9-04 |
| RF-14 | Menús de la sección 18 y HUD de retícula, salud, armadura, arma, potenciador, marcador y conexión. | F7-01 a F7-10 |
| RF-15 | Ajustes: sensibilidad, invertir eje, FOV, audio, calidad, gore, reducción de flashes y de movimiento de cámara, subtítulos de avisos, contraste. | F7-09 |
| RF-16 | Desconexión en demo: la partida sigue y queda marcada. Sin reiniciar la puntuación al reconectar. | F3-11 |
| RF-17 | Humanos no conservan armas al cambiar de mapa. | F5-07, F9-02 |

## No funcionales

| ID | Requisito | Tareas |
|---|---|---|
| RNF-01 | 60 FPS en el PC de referencia, que no está definido. | F0-10, F10-04 |
| RNF-02 | Pool de proyectiles, impactos, partículas y decals. Tope de ragdolls. | F8-06 |
| RNF-03 | El cliente no confirma el daño, la puntuación ni el MMR. | F2-01, F3-07, regla `04` |
| RNF-04 | Una función de red no se cierra en una sola máquina. | F3-12, F3-13, F10-02 |
| RNF-05 | Build de Windows que arranca sin Editor. | F10-05, F10-06 |
| RNF-06 | Secretos de Photon fuera de git. | F3-01 |
| RNF-07 | Sin frameworks nuevos si el sistema cabe en componentes y ScriptableObjects. | F0-07, regla `02` |

## De jugabilidad

| ID | Requisito | Tareas |
|---|---|---|
| RJ-01 | El movimiento se sostiene sin enemigos. | F1-10, F5-01 |
| RJ-02 | El impacto se entiende y no tapa al rival. | F2-11, F8-06 |
| RJ-03 | Los recursos del mapa tienen lugar y tiempo. | F5-05, F6-04 |
| RJ-04 | Asimetría de alcance contra cuerpo a cuerpo, sin habilidades de héroe. | F4-05, F4-08 |
| RJ-05 | El spawn no produce muertes inevitables. | F2-05, F5-08 |
| RJ-06 | Ninguna ruta se bloquea de forma permanente. Hay al menos dos rutas. | F5-03, F5-08 |
| RJ-07 | Ningún monstruo es la única elección viable. | F4-02, F4-08 |
| RJ-08 | El ítem fuerte se disputa y no garantiza la victoria por sí solo. | F6-03, F10-03 |

## Técnicos

| ID | Requisito | Tareas |
|---|---|---|
| RT-01 | Unity 6.5.6 y URP. | F0-02, F0-03 |
| RT-02 | Input System. | F0-09, F1-03 |
| RT-03 | Carpetas `_Project` y componentes de la sección 14.2. | F0-07 |
| RT-04 | ScriptableObjects de arma, monstruo, pickup, mapa, modo y movimiento. | F1-01, F2-06, F4-01, F6-01 |
| RT-05 | Pruebas de daño, pickups y puntuación. | F2-12, F6-05, F10-01 |
| RT-06 | Nombres de código en inglés. Separar lógica, presentación y red. | Regla `07` |
| RT-07 | Registrar licencia de cada asset externo. | F0-11 |
| RT-08 | Animación remota solo como presentación. | F8-08, regla `05` |

## Visuales

| ID | Requisito | Tareas |
|---|---|---|
| RV-01 | Look gótico industrial, metal, luz roja, contraste, low poly aceptable. | F8-01, F8-05 |
| RV-02 | Sangre, decals limitados, muerte animada o ragdoll simple. Desmembramiento fuera de la demo. | F8-06 |
| RV-03 | Silueta de enemigo distinta del escenario. | F8-02, F8-03 |
| RV-04 | Humanos: 1 modelo, manos FPS y silueta remota. Monstruos: 2. Armas: vista y mundo. | F8-02, F8-03, F8-04 |
| RV-05 | Opción de reducir partículas, decals, flashes y sangre. | F7-09, F8-06 |

## Sonoros

| ID | Requisito | Tareas |
|---|---|---|
| RA-01 | Disparos, explosiones, pasos, salto, impactos, muertes, pickup, UI, ambiente y música heavy. | F8-07 |
| RA-02 | Volúmenes separados de música, efectos y general. Variación para no repetir el mismo golpe. | F7-09, F8-07 |
| RA-03 | Pasos y disparos cercanos se distinguen. El sonido cosmético no imita una señal de partida. | F8-07 |

## De networking

| ID | Requisito | Tareas |
|---|---|---|
| RN-01 | Photon Fusion 2. Sesión de prueba en la demo. | F0-05, F0-06, F3-01 |
| RN-02 | Sincronizar spawn, movimiento, equipo, arma, salud, armadura, muerte, proyectiles, pickups, ronda, mapa y puntuación. | F3-04 a F3-08, F3-10, F6-07 |
| RN-03 | Respuesta local inmediata y reconciliación. | F3-04 |
| RN-04 | Un impacto no se cuenta dos veces. | F3-07, F2-12 |
| RN-05 | Lag compensation de hitscan sin política escrita. | F3-09 |
| RN-06 | Pruebas: dos clientes locales, dos máquinas, internet, latencia, pérdida de paquetes, abandono, pickup simultáneo, muertes simultáneas, transición. | F3-12 a F3-17, F10-02 |
| RN-07 | Validar cadencia, munición, posición de pickup y estado antes de aceptar la acción. | F3-07, F6-05 |

## De calidad

| ID | Requisito | Tareas |
|---|---|---|
| RC-01 | Definition of Done de la sección 23. | F10-08, regla `08` |
| RC-02 | La demo terminada exige conexión, partida de dos equipos, ciclo de vida del jugador, sincronización, monstruos, puerta, marcador coherente, desconexión segura, build, latencia simulada, errores documentados y README. | F10-05 a F10-07 |
| RC-03 | No dar por hecha una función que solo se vio en el Editor en solitario. | Regla `08` |

## Contradicciones y huecos

Las que siguen abiertas van de D-03 a D-22 en [DECISIONS.md](DECISIONS.md). La red quedó en T-21 (Host Mode) y T-22 (Fusion 2.1.4 compilado, sin certificación oficial del editor). Las que cambian el alcance son:

- 2v2 recomendado y 4v4 condicionado, frente a «4v4» en la sección 25.
- Versus Continuous «alta para demo» frente a «se intercambian roles».
- «Tres potenciadores» frente a una tabla que solo deja salud y armadura en demo, y un Quad Damage sin reglas.
- Un solo arquetipo de Demonio frente a dos modelos en el checklist.
- Monstruos que no dependen de munición y, en la misma sección, pueden recoger armas.
- La sección 11 salta de 11.2 a 11.4. No hay 11.3 en el documento.

## Fuera de los requisitos de la demo

Ranked completo (sección 12), MMR, temporadas, recompensas cosméticas, matchmaking, backend, ametralladora, superescopeta, rayo, invulnerabilidad, berserk, regeneración, barriles, desmembramiento, sistema de estilo, M03, M04, bots y anti-cheat avanzado. Están en el checklist como epic E11 o como propuestas E12, con estado BACKLOG y fuera del MVP.
