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

La pasada actual usó un solo equipo físico. Quedan por comprobar LAN y redes de internet distintas con dos equipos, una recogida simultánea mediante colisiones reales, muertes simultáneas y el flujo completo de Campaign y Private Match.
