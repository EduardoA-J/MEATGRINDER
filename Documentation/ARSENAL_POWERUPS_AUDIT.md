# Auditoría del Arsenal y Power-ups

Fecha: 5 de noviembre de 2026.
Fuente: Inspección del código en `d:\git\jogo\grinder` tras la sesión de movimiento y armas del 5 de noviembre.
Objetivo: Estado real antes de implementar el lanzacohetes completo, pickups y potenciadores del GDD.

## Resumen ejecutivo

**Estado del arsenal:** Escopeta funcional en local. Lanzacohetes parcialmente implementado (proyectil y explosión existen, pero no está en la escena ni en el equipamiento de juego). `WeaponController` gestiona múltiples huecos con munición independiente y cambio de armas. Sistema de proyectiles existe.

**Estado de pickups:** Carpeta vacía. No existe sistema de recogida.

**Estado de potenciadores:** No implementados. No existe sistema de efectos temporales.

**Estado de armadura:** `HealthComponent` solo gestiona salud. Sin absorción de daño.

**Red:** Todo local. `DamageResolver` centraliza aplicación de daño pero no tiene autoridad de Fusion. `NetworkObject` ausente del `LocalPlayer`.

## 1. Estado del arsenal

### 1.1. WeaponDefinition (ScriptableObject)

**Archivo:** `Assets/_Project/01_Scripts/Weapons/WeaponDefinition.cs`

**Estado:** ✅ IMPLEMENTADO Y COMPLETO para armas base

**Campos configurables:**
- ✅ `weaponId` (string) - Identificador estable
- ✅ `displayName` (string) - Nombre para HUD
- ✅ `delivery` (enum: Hitscan/Projectile) - Tipo de disparo
- ✅ `damage` - Daño base por perdigón/impacto directo
- ✅ `pelletCount` - Perdigones (escopeta)
- ✅ `spreadDegrees` - Dispersión
- ✅ `maxRange` - Alcance máximo
- ✅ `falloffStart` / `falloffEnd` / `minimumDamageScale` - Caída de daño con distancia
- ✅ `fireInterval` - Cadencia
- ✅ `magazineSize` / `startingLoaded` / `startingReserve` / `ammoPerShot` - Munición
- ✅ `reloadDuration` - Tiempo de recarga
- ✅ `projectileSpeed` / `projectileRadius` / `projectileLifetime` - Proyectiles
- ✅ `blastRadius` / `blastDamage` / `blastEdgeScale` - Explosiones
- ✅ `recoilPitch` - Retroceso
- ✅ `drawDuration` - Tiempo de sacar el arma (bloquea disparo)
- ✅ `projectilePrefab` - Prefab del proyectil
- ✅ `shotSound` - Audio
- ✅ `impactVfx` - Efectos visuales

**Métodos útiles:**
- ✅ `DamageAt(float distance)` - Calcula daño con falloff
- ✅ `BlastAmount(float distance)` - Calcula daño de explosión por distancia

**Pendientes:**
- ❌ No hay campo para reglas de pickup de arma (si se recoge, si reemplaza, etc.)
- ❌ Todos los valores están marcados `PENDIENTE` - no son definitivos

### 1.2. WeaponController

**Archivo:** `Assets/_Project/01_Scripts/Weapons/WeaponController.cs`

**Estado:** ✅ IMPLEMENTADO Y FUNCIONAL

**Funcionalidad implementada:**
- ✅ Sistema de slots (múltiples armas)
- ✅ Munición independiente por slot (cargador + reserva)
- ✅ Estado de recarga independiente por arma
- ✅ Estado de cooldown (cadencia) independiente
- ✅ Sistema de `DrawDuration` - no dispara mientras saca el arma
- ✅ Cambio entre armas con `Select(int index)`
- ✅ Evento `StateChanged` para actualizar HUD
- ✅ Validaciones de disparo: `CanFire()`, `Inspect()`
- ✅ Razones de rechazo: `Unequipped`, `Drawing`, `Reloading`, `CoolingDown`, `Empty`
- ✅ `TryBeginShot()` devuelve `ShotTicket` con confirmación
- ✅ `TryBeginReload()` gestiona recarga automática si no hay munición en reserva
- ✅ `Equip()` / `Unequip()` para controlar disponibilidad
- ✅ Munición se conserva al cambiar de arma

**Limitaciones actuales:**
- ⚠️ El loadout es un array serializado - no hay sistema dinámico de "recoger arma"
- ⚠️ No hay validación de "arma disponible" para equiparla
- ⚠️ El cambio de arma es instantáneo (no hay transición visual)

**Preparado para:**
- ✅ Múltiples armas (código soporta N slots)
- ✅ Equipamiento runtime (método `LoadForTests` muestra cómo cargar armas)

### 1.3. Escopeta

**Asset:** `Assets/_Project/02_ScriptableObjects/Weapons/Shotgun.asset`

**Estado:** ✅ FUNCIONAL Y PROBADO

**Configuración actual (PENDIENTE de balance):**
- `weaponId`: "shotgun"
- `displayName`: "Escopeta"
- `delivery`: Hitscan
- `damage`: 12 por perdigón
- `pelletCount`: 8
- `spreadDegrees`: 5°
- `maxRange`: 36 m
- `falloffStart`: 6 m
- `falloffEnd`: 28 m
- `minimumDamageScale`: 0.3 (30% del daño al final del alcance)
- `fireInterval`: 0.7 s
- `magazineSize`: 6
- `startingLoaded`: 6
- `startingReserve`: 24
- `ammoPerShot`: 1
- `reloadDuration`: 1.4 s
- `recoilPitch`: 1.5°
- `drawDuration`: 0.18 s (añadido el 5 de noviembre)

**Sistema de disparo:**
- ✅ Hitscan con 8 perdigones
- ✅ Dispersión configurable
- ✅ Daño con caída por distancia
- ✅ Cobertura funciona (raycast detecta obstáculos)
- ✅ Feedback de impacto
- ✅ Retroceso de cámara y viewmodel separados

**Pruebas:**
- ✅ EditMode: daño, dispersión, alcance, cobertura
- ✅ PlayMode: `ShotgunDamagesTheOpenTargetAndNotTheHiddenOne`
- ⚠️ Pasada manual en Play Mode: NO REALIZADA (F2-07 en REVIEW)

### 1.4. Lanzacohetes

**Estado:** ⚠️ PARCIALMENTE IMPLEMENTADO - NO EN ESCENA

**Lo que EXISTE:**

#### RocketProjectile.cs
✅ **Implementado y funcional**
- Movimiento físico del proyectil con `SphereCast`
- Velocidad configurable desde `WeaponDefinition`
- Radio de colisión configurable
- Detección de impacto con mundo y objetivos
- Exclusión del owner (no colisiona con quien lo disparó)
- Vida máxima (`ProjectileLifetime`) para evitar proyectiles perdidos
- Llama a `ExplosionQuery.Apply()` al explotar
- Se destruye después de explotar
- ✅ Prueba PlayMode: `RocketMovesAndSparesTheCoveredTarget`

#### ExplosionQuery.cs
✅ **Implementado y funcional**
- Daño de área real con `Physics.OverlapSphere`
- Radio configurable desde `WeaponDefinition`
- Daño por distancia con `BlastAmount(distance)`
- Múltiples objetivos simultáneos (no solo el primero)
- Evita daño duplicado al mismo objetivo (HashSet por `StableId`)
- Detección de cobertura mediante raycast
- Excluye al atacante si corresponde
- Usa `DamageResolver.Apply` - centralizado
- ✅ Prueba EditMode: explosión daña múltiples objetivos

#### LocalProjectileSpawner.cs
✅ **Implementado**
- Instancia proyectiles desde prefab o crea fallback (esfera primitiva)
- Comentario indica preparación para `Runner.Spawn` en red
- No está replicado (local)

#### PlayerCombat.LaunchRocket()
✅ **Implementado**
- Crea `ProjectileLaunch` con todos los datos necesarios
- Llama a `LocalProjectileSpawner.Spawn(shot)`
- Calcula origen con `muzzleClearance`
- Pasa resolver, presenter, owner, attacker ID, team, confirmation

**Lo que FALTA:**

- ❌ **WeaponDefinition asset** para lanzacohetes (Shotgun.asset existe, pero no Rocket.asset)
- ❌ **Lanzacohetes en el loadout** del `LocalPlayer`
- ❌ **Prefab de proyectil** visual (actualmente usa fallback de esfera primitiva)
- ❌ **Viewmodel** del lanzacohetes (ShotgunView existe)
- ❌ **Pickup** de lanzacohetes en Greybox
- ❌ **Zona de prueba** de explosiones en Greybox
- ❌ **VFX** de lanzamiento, vuelo y explosión
- ❌ **Audio** de lanzamiento y explosión
- ❌ **Configuración de valores** (velocidad, radio explosión, daño) - pendientes de balance

**Conclusión lanzacohetes:**
La arquitectura y el código están COMPLETOS. Solo falta configuración (asset), assets visuales (modelo, VFX, audio) y colocación en la escena.

### 1.5. Sistema de cambio de armas

**Estado:** ✅ FUNCIONAL pero sin múltiples armas en escena

**Implementado:**
- ✅ Tecla 1, 2, etc. llaman a `WeaponController.Select(index)`
- ✅ `Select` valida índice y no cambia si ya está activa
- ✅ Inicia `BeginDraw()` que bloquea disparo durante `DrawDuration`
- ✅ Emite evento `StateChanged` para actualizar HUD
- ✅ `ShotgunView` responde a cambios con animación de bajada
- ✅ Munición se conserva por slot

**Falta:**
- ❌ Transición visual más elaborada entre armas (actualmente solo bajada rápida)
- ❌ Segunda arma en el loadout para probar cambio real
- ❌ Scroll del mouse para cambio (no prioritario)
- ❌ Indicador visual de armas disponibles vs no disponibles

### 1.6. HUD de armas

**Archivo:** `Assets/_Project/01_Scripts/Presentation/CombatHud.cs`

**Estado:** ✅ IMPLEMENTADO para escopeta

**Muestra actualmente:**
- ✅ Nombre del arma activa (`weaponText`)
- ✅ Munición: cargador / reserva (`ammoText`)
- ✅ Estado: "saca" si está en `DrawDuration`, "recarga" si está recargando
- ✅ Salud actual / máxima
- ✅ Barra de salud visual
- ✅ Retícula central
- ✅ Marcas de impacto (hit markers) - amarillo si daña, gris si pega en mundo
- ✅ Viñeta de daño recibido

**Falta para arsenal completo:**
- ❌ Armadura actual / máxima
- ❌ Indicadores de potenciadores activos (Quad Damage, etc.)
- ❌ Temporizadores de efectos temporales
- ❌ Icono del arma actual
- ❌ Indicador de armas disponibles

**Arquitectura:**
- ✅ Escucha eventos: `WeaponController.StateChanged`, `HealthComponent.Damaged/Died`, `PlayerImpactFeedback.Presented`
- ✅ No modifica estado de juego - solo lectura
- ✅ Construido con uGUI (no OnGUI)

## 2. Estado de salud y daño

### 2.1. HealthComponent

**Archivo:** `Assets/_Project/01_Scripts/Combat/HealthComponent.cs`

**Estado:** ✅ FUNCIONAL para salud simple

**Implementado:**
- ✅ Salud actual y máxima
- ✅ `IsAlive` property
- ✅ `StableId` para identificación única
- ✅ `TeamId` para fuego amigo
- ✅ `ReceiveConfirmed(float amount)` - aplica daño
- ✅ Eventos `Damaged` y `Died`
- ✅ `ResetHealth()` para respawn
- ✅ Soporte de `HealthProfile` (ScriptableObject)

**Assets existentes:**
- `HealthHuman.asset` - 100 HP
- `HealthFragile.asset` - 40 HP
- `HealthStandard.asset` - 80 HP
- `HealthTough.asset` - 200 HP

**Falta:**
- ❌ **Sistema de armadura** (absorción de daño)
- ❌ Armadura actual / máxima
- ❌ Fórmula de mitigación (bloqueada por D-07)
- ❌ Regeneración de salud (potenciador futuro)
- ❌ Megahealth con degradación

### 2.2. DamageResolver

**Archivo:** `Assets/_Project/01_Scripts/Combat/DamageResolver.cs`

**Estado:** ✅ FUNCIONAL Y CENTRALIZADO

**Implementado:**
- ✅ Único punto de aplicación de daño
- ✅ Evita daño duplicado (HashSet de confirmations)
- ✅ Valida autoridad (`HasAuthority`)
- ✅ Fuego amigo configurable (`FriendlyFire`)
- ✅ Excluye atacante si `ExcludeAttacker`
- ✅ Valida equipo (`TeamId`)
- ✅ Solo daña si objetivo está vivo y cantidad > 0

**Preparado para:**
- ✅ Autoridad de Fusion (campo `HasAuthority` ya existe)
- ✅ Integración con armadura (solo falta modificar el flujo)

**Limitaciones actuales:**
- ⚠️ No hay sistema de armadura integrado
- ⚠️ Sin modificadores de daño (Quad Damage, etc.)

### 2.3. DamageInfo

**Archivo:** `Assets/_Project/01_Scripts/Combat/DamageInfo.cs`

**Estado:** ✅ COMPLETO

**Campos:**
- AttackerId, AttackerTeam
- Target (IDamageable)
- Amount (float)
- Type (enum: Hitscan / Explosion)
- Point (Vector3)
- ConfirmationId (ulong) - evita duplicados
- HasAuthority (bool) - preparado para red
- ExcludeAttacker (bool)
- WeaponId (string)

## 3. Estado de pickups

**Carpeta:** `Assets/_Project/01_Scripts/Pickups/`
**Estado:** ❌ VACÍA (solo .gitkeep)

**No existe:**
- ❌ Sistema de detección de jugador
- ❌ Validación de recogida
- ❌ Evitar doble recogida
- ❌ Temporizadores de reaparición
- ❌ Feedback visual/audio de pickup
- ❌ `PickupDefinition` ScriptableObject
- ❌ Ningún prefab de pickup

**ScriptableObjects Pickups:**
`Assets/_Project/02_ScriptableObjects/Pickups/` - Vacía

**Necesario implementar:**
1. `PickupDefinition` ScriptableObject
2. `PickupController` o componente similar
3. Sistema de trigger/colisión
4. Validación autoritativa
5. Temporizador de respawn
6. Eventos para HUD/feedback
7. Pool de pickups

## 4. Estado de potenciadores

**Estado:** ❌ NO IMPLEMENTADO

**GDD define 7 potenciadores para implementar:**

### 4.1. Health Pickup (Prioridad Demo)
- ❌ No existe
- Requisitos: recupera salud hasta límite normal, no recoge si ya está lleno

### 4.2. Armor Pickup (Prioridad Demo)
- ❌ No existe
- ❌ Sistema de armadura no existe en `HealthComponent`
- ⚠️ Bloqueado por D-07 (fórmula de absorción)

### 4.3. Quad Damage (Prioridad Demo - Semana 5)
- ❌ No existe
- ⚠️ Bloqueado por D-10 (efecto no definido en GDD)
- Requisitos: multiplicador temporal de daño, afecta todas las armas, duración configurable, HUD muestra icono y tiempo

### 4.4. Megahealth (Prioridad Futura)
- ❌ No existe
- Requisitos: salud temporal por encima del límite, degradación gradual configurable

### 4.5. Haste (Prioridad Futura)
- ❌ No existe
- Requisitos: aumenta velocidad temporalmente, modifica `MovementTuning`, restaura valores al expirar, no romper movimiento Quake

### 4.6. Invulnerability (Prioridad Futura)
- ❌ No existe
- Requisitos: reduce/anula daño temporalmente, integrar con `DamageResolver`, duración configurable

### 4.7. Berserk (Prioridad Futura)
- ❌ No existe
- Requisitos: potencia ataques melee (aún no existen), infraestructura de modificadores

### 4.8. Regeneration (Prioridad Futura)
- ❌ No existe
- Requisitos: recuperación gradual de salud, duración configurable, no supera límite (o límite actual con Megahealth)

**Sistema de efectos temporales:**
- ❌ No existe gestor de efectos activos
- ❌ No existe sistema de temporizadores
- ❌ No existen reglas de acumulación (reiniciar/extender/ignorar)
- ❌ No hay eventos para HUD de potenciadores

## 5. Estado de Greybox

**Escena:** `Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity`

**Zonas existentes (5 de noviembre):**

### Zona Movimiento (oeste)
✅ `MovementCourse`
- Recta para velocidad base
- Zona strafe
- Saltos encadenados
- Rampa
- Escalones
- Plataformas
- Letreros de zona

### Zona Combate (este)
✅ `CombatRange`
- `Shotgun_Close` (40 HP) - cerca
- `Shotgun_Far` (40 HP) - lejos (~x=40)
- `Shotgun_Blocked` (80 HP) - detrás de `Wall_ShotgunBlock`
- `Target_Tough` (200 HP)
- Galería de tiro

**Falta añadir:**
- ❌ Zona lanzacohetes / explosiones
- ❌ Objetivos agrupados para probar daño de área
- ❌ Objetivos tras cobertura para explosiones
- ❌ Punto de spawn de pickup de lanzacohetes
- ❌ Puntos de spawn de pickups de salud
- ❌ Puntos de spawn de pickups de armadura
- ❌ Zona de cada potenciador (7 zonas nuevas)
- ❌ Señalización para nuevas zonas
- ❌ Geometría de prueba para cobertura de explosiones

**Objetos de prueba actuales:**
- ✅ `CombatTargetLabel` - construye silueta al arrancar
- ✅ `HealthComponent` - responde a daño
- ✅ Muros y coberturas funcionan

## 6. Arquitectura reutilizable

### 6.1. Sistemas que funcionan bien

✅ **ScriptableObject para configuración**
- `WeaponDefinition` es modelo a seguir
- Separación clara de configuración vs estado mutable

✅ **Sistema de eventos**
- `WeaponController.StateChanged`
- `HealthComponent.Damaged` / `Died`
- `PlayerImpactFeedback.Presented`
- HUD escucha sin modificar estado

✅ **Validación centralizada**
- `DamageResolver` único punto de aplicación
- `WeaponController.Inspect()` validaciones de disparo
- Sistema de confirmaciones contra duplicados

✅ **Separación de responsabilidades**
- Lógica (simulador/controller)
- Datos (ScriptableObject)
- Presentación (view/HUD)
- Ensamblados separados

✅ **Preparado para red**
- Campos `HasAuthority` en lugares correctos
- Comentarios indican qué mover a `FixedUpdateNetwork`
- `ConfirmationId` para evitar duplicados
- Sistema de equipos

### 6.2. Patrones a replicar

Para pickups y potenciadores:

1. **Definición ScriptableObject**
   - Similar a `WeaponDefinition`
   - Campos de configuración (duración, efecto, cooldown)
   - Métodos de consulta (no estado mutable)

2. **Componente de escena**
   - Trigger para detección
   - Referencia a Definition
   - Timer local (estado mutable aquí, no en SO)
   - Eventos para feedback

3. **Gestor de efectos**
   - Similar a `WeaponController` (gestiona slots)
   - Temporizadores de efectos activos
   - Reglas de acumulación
   - Eventos para HUD

4. **Integración con DamageResolver**
   - Modificadores se aplican en `DamageResolver.Apply`
   - No duplicar lógica en cada arma

## 7. Sistemas que necesitan ampliación

### 7.1. HealthComponent → HealthComponent + Armor

**Cambios necesarios:**
- Añadir `Armor` actual y máxima
- Añadir método `SetArmor(float value)`
- Modificar `ReceiveConfirmed` para aplicar absorción ANTES de dañar salud
- Fórmula de absorción pendiente de D-07
- Eventos: `ArmorChanged`

**Alternativa:** Componente separado `ArmorComponent`
- Pros: Single Responsibility
- Contras: Requiere coordinación entre componentes
- **Recomendación:** Añadir a `HealthComponent` - están conceptualmente unidos

### 7.2. DamageResolver → DamageResolver + Modifiers

**Cambios necesarios:**
- Sistema de modificadores temporales
- Lista de efectos activos que modifican daño
- `ApplyModifiers(ref float damage, DamageInfo info)` antes de aplicar
- Quad Damage verifica si el atacante tiene el efecto
- No duplicar en cada arma

### 7.3. WeaponController → mantener sin cambios

**No necesita cambios estructurales**
- Ya soporta múltiples slots
- Solo falta popular con más armas
- Método `LoadForTests` muestra cómo añadir runtime

### 7.4. PlayerCombat → mantener sin cambios

**Ya tiene el flujo de proyectiles**
- `LaunchRocket` está implementado
- `FireHitscan` está implementado
- Solo falta que el lanzacohetes esté en el loadout

### 7.5. CombatHud → ampliar con nuevos paneles

**Añadir:**
- Panel de armadura (similar a salud)
- Panel de potenciadores activos (lista de iconos + timers)
- Cambiar buildeo para acomodar más elementos
- Mantener el patrón de escuchar eventos

## 8. Archivos a crear

### Código nuevo (C#)

**Pickups:**
1. `PickupDefinition.cs` (ScriptableObject)
2. `PickupType.cs` (enum)
3. `PickupController.cs` (MonoBehaviour)
4. `PickupSpawner.cs` o sistema de pool
5. Implementaciones específicas si necesario

**Potenciadores:**
6. `PowerUpDefinition.cs` (ScriptableObject) - podría ser subclase de Pickup
7. `PowerUpType.cs` (enum)
8. `PowerUpManager.cs` - gestiona efectos activos
9. `PowerUpEffect.cs` (clase base abstracta)
10. `QuadDamageEffect.cs`
11. `MegahealthEffect.cs`
12. `HasteEffect.cs`
13. `InvulnerabilityEffect.cs`
14. `BerserkEffect.cs`
15. `RegenerationEffect.cs`

**Armadura:**
16. Modificar `HealthComponent.cs` (añadir armor)
17. `ArmorProfile.cs` (ScriptableObject) - opcional

**Lanzacohetes:**
18. `RocketView.cs` (viewmodel, similar a ShotgunView)

**Greybox:**
19. Modificar `CombatGreyboxBuilder.cs` para añadir zonas

**Tests:**
20. `PickupTests.cs` (EditMode)
21. `PowerUpTests.cs` (EditMode)
22. `ArmorTests.cs` (EditMode)
23. `RocketLauncherTests.cs` (EditMode + PlayMode)
24. `ExplosionDamageTests.cs` (ya existe parcialmente)

### Assets (ScriptableObjects)

**Armas:**
1. `RocketLauncher.asset` (WeaponDefinition)

**Salud:**
(ya existen HealthProfile assets)

**Pickups:**
2. `HealthPickup_Small.asset`
3. `HealthPickup_Large.asset`
4. `ArmorPickup.asset`

**Potenciadores:**
5. `QuadDamage.asset`
6. `Megahealth.asset`
7. `Haste.asset`
8. `Invulnerability.asset`
9. `Berserk.asset`
10. `Regeneration.asset`

### Prefabs

**Armas:**
1. `RocketProjectile.prefab` (visual)
2. `RocketLauncher_ViewModel.prefab`
3. `RocketLauncher_World.prefab` (futuro)

**Pickups:**
4. `HealthPickup.prefab`
5. `ArmorPickup.prefab`
6. `WeaponPickup.prefab` (genérico)

**Potenciadores:**
7. `QuadDamagePickup.prefab`
8. `MegahealthPickup.prefab`
9. `HastePickup.prefab`
10. `InvulnerabilityPickup.prefab`
11. `BerserkPickup.prefab`
12. `RegenerationPickup.prefab`

**VFX:**
13. `RocketExplosion_VFX.prefab`
14. `RocketTrail_VFX.prefab`
15. `PickupGlow_VFX.prefab`
16. `PowerUpActivated_VFX.prefab`

### Escenas

- Modificar `Greybox.unity` - añadir zonas nuevas

## 9. Archivos a modificar

### Modificaciones mínimas necesarias

1. **HealthComponent.cs**
   - Añadir campos de armadura
   - Modificar `ReceiveConfirmed` para absorción
   - Añadir evento `ArmorChanged`
   - NO cambiar interfaz pública existente innecesariamente

2. **DamageResolver.cs**
   - Añadir campo opcional `PowerUpManager` para modificadores
   - Modificar `Apply` para consultar multiplicadores antes de `ReceiveConfirmed`
   - NO duplicar lógica de modificadores en cada arma

3. **CombatHud.cs**
   - Añadir panel de armadura
   - Añadir lista de potenciadores activos
   - Extender `BuildCorner` o añadir `BuildPowerUpPanel`
   - Mantener el patrón de eventos

4. **LocalPlayer.prefab**
   - Añadir `PowerUpManager` component
   - Añadir lanzacohetes al loadout de `WeaponController`
   - Añadir `RocketView` como hijo de cámara (desactivado inicialmente)

5. **Greybox.unity**
   - Añadir geometría de prueba nueva
   - Añadir puntos de spawn de pickups
   - Añadir señalización de zonas

6. **Meatgrinder.Combat.asmdef**
   - Posiblemente añadir referencia si pickups necesitan acceso a armas

7. **Meatgrinder.Weapons.asmdef**
   - Verificar referencias si hay dependencia circular

### Modificaciones NO permitidas

❌ NO cambiar `MovementSimulator` para Haste hasta tener clara la integración
❌ NO romper la interfaz de `DamageResolver` existente
❌ NO duplicar lógica de daño en `RocketProjectile`
❌ NO mover `WeaponController` a otro ensamblado
❌ NO cambiar valores de `Shotgun.asset` sin justificación
❌ NO introducir una segunda `DamageResolver` instance
❌ NO añadir NetworkObject hasta tarea específica de red

## 10. Decisiones bloqueantes

Según `DECISIONS.md`:

- **D-06:** Dash - NO implementar hasta cerrada
- **D-07:** Fórmula de armadura - NECESARIA para implementar absorción
- **D-09:** uGUI vs UI Toolkit - Ya implícitamente cerrada (uGUI en uso), formalizar
- **D-10:** Quad Damage efecto - NECESARIA para implementar correctamente
- **D-11:** Reglas de acumulación de ítems - NECESARIA para sistema completo
- **D-15:** PC de referencia - No bloquea implementación, bloquea validación de rendimiento

## 11. Pendientes de balance

Todos los valores numéricos actuales están marcados `PENDIENTE`:

**Escopeta:**
- Daño por perdigón: 12
- Perdigones: 8
- Dispersión: 5°
- Alcance: 36 m
- Cadencia: 0.7 s
- Munición: 6 / 24

**Lanzacohetes (valores a definir):**
- Daño directo: ¿?
- Daño explosión: ¿?
- Radio explosión: ¿? (campo existe en WeaponDefinition)
- Velocidad proyectil: ¿?
- Cadencia: ¿?
- Munición: ¿?

**Salud:**
- Humano: 100 HP
- Frágil: 40 HP
- Estándar: 80 HP
- Duro: 200 HP

**Armadura:**
- Valor máximo: ¿?
- Porcentaje de absorción: ¿? (D-07)

**Potenciadores:**
- Quad Damage multiplicador: ¿? (D-10)
- Quad Damage duración: ¿?
- Megahealth cantidad: ¿?
- Megahealth degradación: ¿?
- Haste multiplicador velocidad: ¿?
- Haste duración: ¿?
- Invulnerability reducción: ¿?
- Invulnerability duración: ¿?
- Berserk multiplicador: ¿?
- Berserk duración: ¿?
- Regeneration HP/s: ¿?
- Regeneration duración: ¿?

**Pickups:**
- Health Small cantidad: ¿?
- Health Large cantidad: ¿?
- Armor cantidad: ¿?
- Tiempos de reaparición: ¿? (D-11)

## 12. Pendientes de red (multiplayer)

**Todo el arsenal actual es LOCAL:**

- `DamageResolver` no tiene autoridad de Fusion
- `WeaponController` no sincroniza estado
- Proyectiles no se replican (`LocalProjectileSpawner`)
- Pickups no tienen validación autoritativa
- Efectos temporales no se sincronizan

**Preparado para red:**
- ✅ `HasAuthority` existe en `DamageInfo`
- ✅ `ConfirmationId` evita duplicados
- ✅ Comentarios indican qué mover a `FixedUpdateNetwork`
- ✅ `ProjectileLaunch` estructura lista
- ✅ `LocalProjectileSpawner` tiene comentario sobre `Runner.Spawn`

**Tareas de red bloqueadas:**
- F3-01 (Boot scene)
- F3-03 (Spawn autorizado)
- F3-06 (Sincronizar arma, salud, armadura)
- F3-07 (Daño autoritativo)
- F3-08 (Proyectiles autoritativos)
- F6-05 (Pickups autoritativos)
- F6-07 (Réplica de pickups)

## 13. Pruebas existentes

**EditMode - 36/36 pasando (5 de noviembre):**
- Movimiento: 17 tests
- Combate: 14 tests
- Armas: incluidos en combate
- Presentación: 5 tests

**PlayMode - 7/7 pasando:**
- Movimiento: 5 tests
- Combate: 2 tests
  - `ShotgunDamagesTheOpenTargetAndNotTheHiddenOne`
  - `RocketMovesAndSparesTheCoveredTarget`

**Pruebas manuales pendientes:**
- ⚠️ F2-07 (Escopeta en Play Mode con ratón real)
- ⚠️ F0-09 (Input de Move y Look con ratón real)
- ⚠️ F1-03 (Input en ventana de Play)

## 14. Plan de implementación propuesto

### Fase 1: Lanzacohetes completo (P0)
1. Crear `RocketLauncher.asset` con valores PENDIENTE
2. Crear prefab visual del proyectil (o usar fallback mejorado)
3. Crear `RocketView.cs` para viewmodel
4. Añadir lanzacohetes al loadout de LocalPlayer
5. Añadir zona de prueba en Greybox
6. Validar cambio entre escopeta y lanzacohetes
7. Pruebas EditMode y PlayMode

### Fase 2: Sistema de armadura (P0 - bloqueado por D-07)
1. **ESPERAR cierre de D-07**
2. Extender `HealthComponent` con armadura
3. Crear `ArmorProfile.asset` si necesario
4. Modificar `DamageResolver` o `HealthComponent.ReceiveConfirmed` para absorción
5. Actualizar HUD con panel de armadura
6. Pruebas de absorción

### Fase 3: Sistema de pickups base (P0)
1. Crear `PickupDefinition.cs`
2. Crear `PickupController.cs` con trigger y timer
3. Implementar validación básica (no red)
4. Implementar feedback visual/audio básico
5. Pruebas EditMode

### Fase 4: Health y Armor pickups (P0)
1. Crear `HealthPickup.asset`
2. Crear `ArmorPickup.asset` (requiere Fase 2)
3. Crear prefabs de pickups
4. Añadir a Greybox
5. Pruebas de recogida
6. Pruebas de timer de respawn

### Fase 5: Pickup de armas (P1)
1. Crear `WeaponPickup` componente
2. Añadir lanzacohetes como pickup en Greybox
3. Sistema de equipamiento dinámico (ampliar `WeaponController`)
4. Pruebas de recoger arma

### Fase 6: Sistema de potenciadores (P1)
1. Crear `PowerUpManager.cs`
2. Crear `PowerUpEffect.cs` base abstracta
3. Implementar sistema de temporizadores
4. Integrar con `DamageResolver` para modificadores
5. Eventos para HUD

### Fase 7: Quad Damage (P1 - bloqueado por D-10)
1. **ESPERAR cierre de D-10**
2. Implementar `QuadDamageEffect.cs`
3. Crear `QuadDamage.asset`
4. Pruebas: verificar que multiplica daño de TODAS las armas
5. Verificar que no aplica dos veces al mismo ataque

### Fase 8: Potenciadores futuros (P2)
1. Megahealth (con degradación configurable)
2. Haste (cuidado con integración en MovementSimulator)
3. Invulnerability
4. Berserk (requiere melee)
5. Regeneration

### Fase 9: HUD completo (P1)
1. Panel de armadura
2. Lista de potenciadores activos con iconos y timers
3. Pulido visual

### Fase 10: Greybox completo (P1)
1. Zona de explosiones
2. 7 zonas de potenciadores
3. Señalización
4. Geometría de cobertura para explosiones

### Fase 11: Reglas de acumulación (P1 - bloqueado por D-11)
1. **ESPERAR cierre de D-11**
2. Implementar reinicio/extensión/ignorar según decisión
3. Pruebas de cada caso

## 15. Riesgos identificados

### Técnicos

**R-01: Fórmula de armadura sin definir (D-07)**
- **Impacto:** ALTO - bloquea implementación completa de armadura
- **Mitigación:** Implementar con valores configurables, documentar como PENDIENTE
- **Propuesta temporal:** Absorción lineal 50%, máximo 100, configurable en `ArmorProfile`

**R-02: Quad Damage sin especificación (D-10)**
- **Impacto:** MEDIO - se puede implementar infraestructura sin valores
- **Mitigación:** Multiplicador configurable en `PowerUpDefinition`, defecto 2x, TODO claramente marcado
- **Propuesta temporal:** 2x durante 30s, no acumula (reinicia duración)

**R-03: Modificadores de daño pueden aplicarse dos veces**
- **Impacto:** ALTO - bug crítico de gameplay
- **Mitigación:** Aplicar modificadores SOLO en `DamageResolver.Apply`, ANTES de `ReceiveConfirmed`
- **Test:** Verificar que un disparo con Quad activo registra el multiplicador una sola vez

**R-04: Haste puede romper MovementSimulator**
- **Impacto:** ALTO - movimiento Quake es core
- **Mitigación:** Modificador temporal sobre `MovementTuning`, NO cambiar algoritmo del simulador
- **Test:** Verificar que al expirar Haste, strafe jumping sigue funcionando

**R-05: Temporizadores pueden desincronizarse entre clientes**
- **Impacto:** MEDIO - en fase local no aplica, pero preparar
- **Mitigación:** Estructura de timer preparada para tick de red
- **Futuro:** Usar `NetworkObject` + `TickTimer` de Fusion

**R-06: Pickup simultáneo sin autoridad**
- **Impacto:** ALTO en red - dos jugadores obtienen el mismo ítem
- **Mitigación:** En fase local, solo un jugador; preparar validación autoritativa
- **Futuro:** F6-05 implementa anti-doble-recogida

### De alcance

**R-07: Valores PENDIENTE tratados como finales**
- **Impacto:** MEDIO - perder tiempo balanceando
- **Mitigación:** Comentarios `PENDIENTE` en cada valor provisional
- **Regla:** No invertir tiempo en balance hasta F10-03

**R-08: Implementar sistemas no pedidos**
- **Impacto:** BAJO - desperdicio de tiempo
- **Mitigación:** Solo implementar lo listado en GDD secciones 5, 6, 8
- **Prohibido:** Medidores de combo, rage meters, habilidades de héroe, etc.

**R-09: Añadir dependencias nuevas sin justificación**
- **Impacto:** MEDIO - complejidad innecesaria
- **Mitigación:** Solo ScriptableObjects + componentes, seguir arquitectura actual

### De integración

**R-10: Romper pruebas existentes 36/36 + 7/7**
- **Impacto:** ALTO - regresión
- **Mitigación:** Ejecutar tests después de CADA cambio
- **Regla:** No commitear si algún test falla

**R-11: Cambiar interfaz de `DamageResolver`**
- **Impacto:** ALTO - rompe todo el combate
- **Mitigación:** Extender, NO cambiar firma de `Apply`

**R-12: Introducir dependencia circular entre ensamblados**
- **Impacto:** MEDIO - no compila
- **Mitigación:** Revisar asmdefs, mantener separación: Combat ← Weapons ← Pickups

## 16. Conclusiones y recomendaciones

### Estado general: ✅ BASE SÓLIDA

El proyecto tiene:
- ✅ Arquitectura clara y separada
- ✅ Sistema de armas funcional
- ✅ Sistema de daño centralizado
- ✅ Proyectiles y explosiones implementados
- ✅ HUD con eventos
- ✅ 43/43 tests pasando
- ✅ Código preparado para red

### Trabajo pendiente: IMPLEMENTACIÓN Y CONFIGURACIÓN

**NO es refactoring masivo**, es:
1. Completar lanzacohetes (ya 80% hecho)
2. Crear sistema de pickups (nuevo)
3. Extender salud con armadura (ampliar)
4. Crear sistema de potenciadores (nuevo)
5. Ampliar HUD (agregar paneles)
6. Ampliar Greybox (añadir zonas)

### Recomendación de orden

**Prioridad ALTA (MVP blocker):**
1. Lanzacohetes completable → solo assets y configuración
2. Armadura → esperar D-07, luego implementar
3. Sistema pickups → base nueva
4. Health/Armor pickups → usa base de pickups

**Prioridad MEDIA (demo 8 semanas):**
5. Weapon pickups
6. Sistema potenciadores
7. Quad Damage → esperar D-10
8. HUD completo

**Prioridad BAJA (post-demo):**
9. Potenciadores futuros (Megahealth, Haste, etc.)
10. Balance de valores

### Decisiones urgentes a cerrar

1. **D-07 (Armadura)** - bloquea pickups de armadura
2. **D-10 (Quad Damage)** - bloquea implementación correcta
3. **D-11 (Acumulación)** - bloquea sistema completo de timers

### Estimación de complejidad

**Lanzacohetes:** S - solo configuración, código ya existe
**Sistema Pickups:** M - estructura nueva pero clara
**Armadura:** S-M - modificar componente existente, fórmula por definir
**Potenciadores:** L - 7 efectos + sistema de gestión + HUD
**Greybox:** M - geometría y colocación

**Total estimado:** L-XL (acorde con alcance del hito)

### Siguiente paso inmediato

Empezar con **lanzacohetes completo**, porque:
- ✅ Código ya existe y funciona
- ✅ No tiene decisiones bloqueantes
- ✅ Solo requiere assets y configuración
- ✅ Valida el cambio de armas end-to-end
- ✅ Quick win para momentum

Después: sistema de pickups (base) → health pickup → armadura (si D-07 cierra) → resto.

---

**Auditoría completada. Sistema listo para implementación.**
