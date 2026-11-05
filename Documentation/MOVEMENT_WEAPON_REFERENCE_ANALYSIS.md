# Análisis de movimiento y armas

Fecha: 5 de noviembre de 2026.
Los clones siguen en `F:\MAPAUNI\references\`. No se copió código. La auditoría de licencias está en `REFERENCE_REPOSITORIES_AUDIT.md`.

## 1. Qué hace cada repositorio

**SourceEngineMovementUnity.** Un `CharacterController` lee un comando (avance, strafe, salto, yaw) y lo integra en `PlayerMovementUtil`. En suelo aplica fricción y acelera a lo largo del wishdir hasta un tope de wish speed. En aire usa la misma aceleración con otro valor. Cuando la velocidad ya llegó al máximo, el empuje nuevo no aumenta el módulo: lo redistribuye. Hay otro recorte horizontal al mismo máximo. El salto pone la velocidad vertical y no borra la horizontal. Los números viven en un ScriptableObject.

Ese tope duro impide el strafe jumping de Quake III, donde el límite del aire recorta el wish, no la velocidad que ya llevas.

**Salvation's Descent.** `WeaponSystem` tiene un arma activa. `WeaponBase` guarda cargador, reserva, cadencia, recarga y un retroceso que mueve el transform local del arma, no la cámara. Al activarse reproduce la animación de sacar y no dispara hasta que termina. El disparo puede ser por clic o por mantener, según el arma. La escopeta hereda el raycast y añade perdigones. El proyectil es otra subclase. Odin, audio y el animator son de ese proyecto.

## 2. Qué conviene reproducir

- Acelerar solo a lo largo del wishdir, con fricción solo en suelo.
- En aire, un wish corto. Ir recto no sube de la carrera. El strafe en ángulo sí puede pasar de esa carrera, hasta un tope de seguridad.
- Saltar conserva la horizontal. Encadenar saltos exige pulsar, no dejar espacio pulsado.
- Un arma activa, munición por hueco, cadencia, recarga y un tiempo de sacar el arma en el que no se dispara.
- El retroceso visual sigue en el viewmodel. La cámara tiene el suyo, aparte. El rayo no se mueve por ninguno de los dos.

## 3. Cómo estaba MEATGRINDER

`MovementSimulator.Tick` ya separaba suelo y aire, aplicaba fricción, aceleraba por wishdir y saltaba sin poner la horizontal a cero. Después recortaba el módulo a `MaxSpeed`. En aire el wish era `GroundSpeed` (8). Con el asset, la carrera llegaba a 8 en unas pocas décimas y el strafe moría en 22. `PlayerMovement` mandaba `Jump` mientras espacio seguía pulsado, así que al aterrizar volvía a saltar solo.

Había además un slerp de `AirControl` que giraba la velocidad hacia el wish sin cambiar el módulo. Con el valor 3, un giro de 4° por tick durante un salto solo subía de 8,0 a 8,46. El ángulo que construye velocidad se cerraba solo.

`WeaponController` ya tenía huecos, munición independiente, cadencia, recarga y cambio sin disparo extra. En Greybox solo está la escopeta. La tecla 2 no cambia nada. `ShotgunView` retrocedía en local y no tocaba el rayo. El cambio de arma era instantáneo.

## 4. Diferencias que se cerraron en código

| Principio | Referencia útil | Antes | Después |
|---|---|---|---|
| Wish de aire más corto que la carrera | Quake III | El wish de aire era la carrera | `AirWishSpeed` 2,8 |
| La velocidad de strafe puede superar la carrera | Quake III | El recorte a 22 llegaba enseguida y el slerp mataba el ángulo | El slerp queda en 0. El tope de seguridad es 32 |
| Tope que impide pasar del máximo | El repo de Source | `ClampMagnitude` a 22 | Sigue el recorte, ahora a 32 |
| Salto al mantener espacio | Se pidió que no sea automático | Se repetía al aterrizar | Un buffer por pulsación |
| Sacar el arma bloquea el disparo | Salvation's Descent | El cambio era instantáneo | `DrawDuration` 0,18 s en la escopeta |
| Varias armas en la escena | Salvation's Descent | Un solo hueco | Sigue un solo hueco. El código ya admite otro |

## 5. Qué quedó implementado

- `AirWishSpeed` recorta el wish del aire, no la velocidad ya ganada. Ir recto no pasa de la carrera.
- `MaxSpeed` es tope de seguridad.
- `GroundAcceleration` baja de 15 a 10 para que la carrera no sea un instante.
- `AirControl` pasa de 3 a 0. El parámetro sigue en el asset. Subirlo vuelve a cerrar el ángulo del strafe.
- El salto del jugador sale de un buffer al pulsar espacio. Mantenerlo no lo rellena. El simulador sigue tratando `Jump` como «salta este tick».
- La escopeta tiene `DrawDuration` 0,18 s. `TryBeginShot` responde `Drawing` y no gasta munición. El valor por defecto del tipo es 0, para no romper las pruebas que disparan al momento.
- El viewmodel baja mientras se saca y la corredera se mueve en local al disparar. El rayo sale de la cámara.
- Greybox tiene `MovementCourse` al oeste. Los blancos no se movieron.
- El readout de depuración muestra velocidad, máximo de la sesión, suelo o aire, delta y si el salto está en buffer.

No se añadió un rifle. La ametralladora está fuera de la demo. El lanzacohetes sigue fuera de la escena.

## 6. Archivos

Tocados: `MovementSimulator`, `MovementTuning`, `MovementTuning.asset`, `PlayerMovement`, `MovementProbeReadout`, `WeaponDefinition`, `WeaponController`, `ShotRejectReason`, `ShotgunView`, `CombatHud`, `Shotgun.asset`, `Greybox.unity`, pruebas EditMode.

Nuevos: `MovementCourseBuilder`, `FeelPassRunner`, esta nota.

## 7. Riesgos

- El tope de seguridad sigue existiendo. Un strafe muy largo no crece sin fin.
- Los números siguen siendo `PENDIENTE`. No son los de Quake ni los del GDD.
- La gravedad de diseño no toca PhysX (D-18).
- `AirControl` alto anula el strafe. El valor de sensación es 0.
- El buffer de salto cambia la sensación de mantener espacio. Las pruebas del simulador siguen mandando `Jump` explícito.
- La pasada en la ventana de Play, con ratón real, no se ha hecho.

## 8. Estudiado, implementado y pendiente

**Estudiado y no copiado.** Fricción y wishdir de Source. El recorte de Source que impide ganar módulo. El ciclo de sacar el arma, el cargador y el retroceso local de Salvation's Descent.

**Implementado.** Aceleración y fricción en suelo. Wish de aire más corto que la carrera. Strafe que puede pasar de 8 m/s hasta el tope 32. Salto que conserva la horizontal. Buffer de pulsación para encadenar saltos. Rampa, escalón y plataformas que ya estaban, más una pista nueva al oeste. Escopeta con perdigones, cadencia, recarga, munición propia, tiempo de sacar, retroceso de cámara y retroceso de viewmodel separados. HUD que lee eventos.

**Parcial.** El control aéreo de Quake es la aceleración por wish. El slerp de `AirControl` existe y está en 0, porque al subirlo anula el strafe. El cambio de arma funciona en código con dos definiciones de prueba. En la escena solo hay una escopeta, así que la tecla 2 no cambia nada. La corredera es un desplazamiento local, no una animación de recarga completa.

**Pendiente.** Rifle automático, no entra: la ametralladora está fuera de la demo. Lanzacohetes en la escena. Pasada manual con ratón. Red, armadura, respawn y dash.
