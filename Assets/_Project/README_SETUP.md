# 🌩️ DeathCloud Sandbox: Guía de Configuración y Diseño

Esta guía detalla cómo configurar tu escena para que el nuevo sistema modular funcione perfectamente y cómo diseñar niveles de prueba efectivos.

## 🛠️ Configuración Técnica (Requerido)

Para que las físicas y la detección funcionen, configura estos 3 pilares en Unity:

### 1. Capas (Layers)
Crea y asigna las siguientes capas en el Inspector:
- **Player**: Capa exclusiva para el jugador.
- **Ground**: Para suelos, paredes y techos sólidos.
- **Grapple**: Para puntos específicos donde el gancho puede anclarse (puedes usar la misma capa Ground si quieres que todo sea enganchable).

### 2. Colisionadores y Físicas
- **Personaje**: Usa un `CapsuleCollider2D`.
- **Fricción Cero**: Crea un `PhysicsMaterial2D` con `Friction = 0` y asígnalo al Collider del jugador. Esto evita que el personaje se quede "pegado" a las paredes al saltar.
- **Puntos de Gancho**: Deben tener un `Collider2D` (Box o Circle) para que el Raycast pueda detectarlos.

### 3. El Nuevo Prefab del Jugador
En el objeto de tu personaje, ahora debes tener estos componentes configurados:
1.  **Rigidbody2D**: Body Type = Dynamic, Collision Detection = Continuous.
2.  **PlayerStateMachine**: Arrastra el `InputReader` y el `PlayerStats` (ScriptableObjects) a sus ranuras.
3.  **DistanceJoint2D**: Desactívalo por defecto (`enabled = false`). Configure `Max Distance Only = True`.
4.  **LineRenderer**: Para el cable del gancho.

---

## 🕹️ Diseño del Sandbox (Modo de Prueba)

El objetivo de este Sandbox es validar el **Momentum** y la **Agilidad**. Sugiero diseñar el nivel con estos "test de estrés":

### ZONA A: El Pozo de Agilidad
- Un pasillo vertical estrecho.
- **Objetivo**: El jugador debe subir haciendo *Wall Jumps* en zig-zag.
- **Validación**: ¿Se siente fluido el despegue de la pared?

### ZONA B: El Péndulo de Elitio
- Una fosa grande con un único punto de anclaje (`Grapple`) en el techo.
- **Objetivo**: Saltar al vacío, engancharse, balancearse y soltarse en el punto más alto.
- **Validación**: Con el "Salto Spider-Man", el jugador debería salir disparado con mucha más fuerza que un salto normal si lo hace con el timing correcto.

### ZONA C: La Trampa de Tensión
- Un obstáculo que requiere balancearse por mucho tiempo.
- **Objetivo**: Cruzar un área enganchado a un punto lejano.
- **Validación**: ¿El parpadeo rojo de la cuerda le avisa al jugador que está a punto de romperse?

---

## 📦 Resumen del Refactor Modular
Hemos pasado de un script gigante a un sistema organizado:
- **InputReader**: Traduce tus teclas a eventos.
- **PlayerStats**: Cambia la velocidad del juego sin tocar código.
- **States**: La lógica de Caminar, Saltar y Gancho están separadas, facilitando encontrar errores.

*¡Ahora puedes probar el sistema y ajustar los valores en el PlayerStatsSO hasta que se sienta perfecto!*
