# Validación de red para Windows

La build de desarrollo Win64 del 26 de noviembre de 2026 se generó en Unity 6000.5.6f1 con cero errores. El App ID de Fusion se leyó del asset local ignorado por Git.

## Resultados

| Caso | Resultado |
|---|---|
| Host y cliente en dos procesos, transporte Direct | PASS en ambos |
| Host y cliente en dos procesos, relay de Photon | PASS en ambos, con simulación de 120 ms y 4 % de pérdida. Fusion registró RTT de juego de 378 ms al salir. |
| Movimiento remoto | La intención llegó por Fusion y movió al avatar autoritativo. |
| Contención de pickup | Dos avatares conectados intentaron recoger la misma armadura en el mismo frame del host. Se concedió una vez por la ruta de recogida del juego. |
| Daño, muerte y reaparición | Se inyectó un impacto en el resolvedor autoritativo. El host registró una sola baja y el cliente observó muerte y respawn. |
| Rotación y abandono | El cliente cargó M01–M04 en Continuous, recibió resultados y vio el reinicio en la revancha. El host confirmó el resultado por abandono al salir el cliente. |

El primer intento descubrió que el clon heredaba el identificador de salud generado en `Awake` por la plantilla de escena. La sesión ahora asigna un identificador nuevo a cada combatiente.

## Reproducción

Compilar con el App ID local y ejecutar desde la carpeta Unity del proyecto:

```powershell
./Tools/Test-VersusWindows.ps1 -Executable <ruta-al-exe>
./Tools/Test-VersusWindows.ps1 -Executable <ruta-al-exe> -Relay -Impaired
```

Para probar desde equipos distintos, ejecutar `-Role Host` en uno y `-Role Client` en otro con el mismo valor de `-Room`. Usar un código nuevo por pasada. El script guarda los logs separados y falla si falta el marcador PASS de un proceso.

La pasada usó un solo equipo físico. Quedan por comprobar LAN y redes de internet distintas con dos equipos, una recogida simultánea mediante colisiones reales y muertes simultáneas.

## Demonios en 2v2 y 4v4

La sonda valida la preferencia de clase con Fusion, rechaza una opción inválida, comprueba perfiles de salud, hitbox y melee en el host, y confirma el cambio de clase al reaparecer. El caso privado decide por bajas rivales. Campaign comprueba los cuatro mapas en ambas mitades, los roles invertidos y el marcador por salidas. El host usa bajas y salidas controladas para reproducir los resultados. Todos los clientes confirman cada escena y el marcador antes de que la sonda pase al siguiente paso.

```powershell
./Tools/Test-VersusWindows.ps1 -Executable <ruta-al-exe> -Players 4
./Tools/Test-VersusWindows.ps1 -Executable <ruta-al-exe> -Players 8 -Campaign
```

Todos los procesos deben usar la misma región de Photon. El script usa `cae` por defecto y admite `-Region`. Estas comprobaciones cubren la réplica y la lógica de las reglas; el playtest con personas, balance y rendimiento en equipos separados quedan pendientes.

La sonda de dos procesos también acciona los botones del lobby para cambiar el cliente a Marines y el host a Demonios. Comprueba la asignación replicada y el mismo panel de salud en ambos procesos, con máximos respectivos de 100 y 90.
