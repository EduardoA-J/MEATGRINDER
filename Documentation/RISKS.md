# Riesgos

Fecha: 2 de noviembre de 2026. Impacto y mitigación salen del GDD cuando existen, y de la auditoría cuando el repositorio aporta un hecho nuevo.

| ID | Riesgo | Impacto | Estado | Mitigación |
|---|---|---|---|---|
| K-01 | Alcance de 8 semanas con el juego a cero. | La demo completa no cabe si el arranque se retrasa. | Abierto | MVP de las semanas 1–3. Recortar arte antes que las pruebas de red (T-14). |
| K-02 | El editor 6.5.6 ya abrió el proyecto. El riesgo de usar 2020.3 queda cerrado. | El import inicial compiló sin `error CS`. | Cerrado el 3 de noviembre de 2026 | F0-02 y F0-03 en DONE. |
| K-03 | Unity CLI no está en el PATH. El paquete Pipeline no está en el manifiesto. | `unity-cli` no puede conducir el Editor desde el agente. | Abierto | Instalar el CLI cuando David lo apruebe. No instalar Pipeline en esta fase. |
| K-04 | Fusion 2.1.4 compiló en Unity 6000.5.6f1. La matriz oficial sigue sin listar ese editor. | La compilación de la prueba no cubre ejecución, weaver en Play Mode ni certificación de Photon. | Abierto | T-22. Probar dos clientes cuando exista App ID. Si falla en ejecución, revertir solo la importación. |
| K-05 | Movimiento tipo Quake difícil de sincronizar. | Combate injusto. | Abierto | Prototipo de movimiento y de red en las primeras tareas. Valores en ScriptableObject. |
| K-06 | Contradicción 2v2 / 4v4 y Campaign / Continuous (D-04, D-05). | Se construye el modo equivocado. | Abierto | No implementar F9 hasta la aprobación. El MVP usa dos clientes, sin cerrar el modo. |
| K-07 | Quad Damage, armadura, respawn y dash sin números (D-06 a D-11). | Balance y código con constantes inventadas. | Abierto | Bloquear esas tareas. Prototipar con valores marcados `PENDIENTE`. |
| K-08 | Asimetría humanos / monstruos. | Un equipo gana siempre. | Abierto | Probar 2v2 y medir. Estadísticas en `MonsterDefinition`. |
| K-09 | Gore caro en FPS y en tiempo de arte. | La demo se va en efectos. | Abierto | Pool, topes, ragdoll limitado. Recorte visual primero. |
| K-10 | Ranked prematuro. | Resultados manipulables. | Contenido | Epic E11 fuera de la demo. MMR solo con backend de confianza. |
| K-11 | Población baja. | Colas vacías. | Aceptado para la demo | Salas privadas. Sin matchmaking. |
| K-12 | Licencias de assets. | No se puede distribuir el build. | Abierto | F0-11 antes de importar un pack. |
| K-13 | Desconexiones. | Partidas corruptas. | Regla simple tomada | Continuar y marcar (T-12). Reconexión sin duplicar recompensa: hay que diseñarla cuando exista sesión. |
| K-14 | Las copias de `setup-multiplayer-services`, `build-live-game`, Vivox, IAP, anuncios, localización, 2D, web, tiles y `build-gtk` se borraron de `.cursor/skills`. | Un perfil global de Cursor todavía puede tener esas skills. | Mitigado en este repo | Regla `04`. No reinstalarlas para la red de este juego. |
| K-15 | `game-team-orchestrator` invoca agentes que no existen. | Planes de orquestación que nadie puede ejecutar. | Abierto | Usarlo como referencia de handoff. No lanzar sus pipelines. |
| K-16 | `physics-3d-collision` ordena aplicar el arreglo sin pedir permiso. | Ediciones fuera de la tarea. | Abierto | La regla de flujo gana: diagnosticar sí, editar solo el alcance aprobado. |
| K-17 | `com.unity.multiplayer.center` sigue instalado junto a Fusion. | El asistente de Unity puede empujar otro stack. | Abierto | La red de juego es Fusion Host Mode. No seguir ese asistente. |
| K-18 | Input de plantilla con Crouch y Sprint (D-17). | Un controlador genérico se cuela como si fuera el de Quake. | Abierto | F0-09. El controlador de juego no reutiliza el Character Controller de muestra porque no hay muestra de jugador. |
| K-19 | Calidad Mobile y renderer móvil en un juego de PC. | Un agente optimiza el objetivo equivocado. | Bajo | Standalone apunta a la calidad PC. Mobile se ignora hasta que exista una plataforma móvil, que el GDD no pide. |
| K-20 | El GDD está en el repositorio, pero el código futuro puede divergir en silencio. | Dos fuentes de verdad. | Abierto | Si el código contradice el GDD, se documenta y se para. |

## Limitaciones de esta auditoría

- La auditoría inicial no abrió el Editor. El 3 de noviembre de 2026 el editor `6000.5.6f1` sí abrió el proyecto y, en un segundo paso, importó Fusion en batchmode. La evidencia de esa importación es `grinder/Logs/fusion-import.log`, no una captura de la consola.
- No se ejecutó una sesión de Photon, ni una prueba de rendimiento, ni una de input.
- El texto del GDD se extrajo del XML de Word. Las tablas salieron como líneas sucesivas. El contenido usado es el de ese texto; no se rellenó ninguna celda vacía.
- La skill `game-team-orchestrator` se leyó completa. El resto de skills de Unity se leyeron por su cabecera y, cuando hacía falta, por las restricciones de versión (`build-gtk`, `asset-transformer-toolkit`, `unity-cli`, `setup-multiplayer-services`, `physics-3d-collision`, `ui`).
