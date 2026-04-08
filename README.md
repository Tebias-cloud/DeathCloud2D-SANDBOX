# 🌩️ DeathCloud (Skybound Drifters) - Sandbox v0.1

Este repositorio contiene el entorno de pruebas técnico para el proyecto **DeathCloud**. El objetivo de este branch es validar las mecánicas de desplazamiento y físicas de alta velocidad antes de la integración con el mundo principal.

## 🕹️ Mecánicas Implementadas
- **Character Controller 2D:** Movimiento basado en físicas con fricción cero para evitar atascos en pendientes.
- **Sistema de Salto Dinámico:** Salto base con preservación de inercia.
- **Wall Mechanics:** Wall Slide y Wall Jump funcional en superficies verticales.
- **Látigo de Elitio (Grappling Hook):** - Sistema de balanceo mediante `DistanceJoint2D`.
  - Mecánica de tensión: La cuerda parpadea y se rompe tras 5-12 segundos de estrés máximo.
  - Salto "Spider-Man": Impulso vertical y horizontal extra si se salta en el momento de máxima velocidad.

## 🛠️ Detalles Técnicos
- **Tilemaps Múltiples:** Separación física entre capas de suelo (`Ground`) y puntos de anclaje (`Grapple`).
- **Renderizado:** Uso de `LineRenderer` para la visualización del gancho con efectos de color dinámicos.
- **PPU (Pixels Per Unit):** Configurado a 16 para los assets de la isla y 32 para el Caballero, manteniendo la escala 1:1 en el mundo.

## 👥 Colaboradores
- **Backend & Physics:** [Tebias-cloud](https://github.com/Tebias-cloud) (Esteban)
- **Equipo:** Diego, Seba

---
*Gráficos creados por Penzilla Design (Licencia Estándar)*
