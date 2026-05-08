# CONTEXTO MAESTRO - DeathCloud 2D (MVP Académico)

Este archivo sirve como puente de contexto para mantener la coherencia y estabilidad del proyecto en futuras iteraciones.

## 1. Estado Actual del Proyecto
- **Motor:** Unity 6 (C#)
- **Arquitectura Multijugador:** Netcode for GameObjects (NGO)
- **Cámara:** Cinemachine 3.1.6 (Usando `CinemachineCamera` y `CameraTarget` offset)
- **Objetivo Principal:** Cumplir estrictamente una Pauta de Evaluación académica de 89 puntos. Trabajando de forma modular, paso a paso y priorizando código limpio (cero espagueti).

## 2. REGLA DE ORO (¡INQUEBRANTABLE!)
- **NO MODIFICAR** el script `PlayerStateMachine` ni las clases de los estados de movimiento (ej. `GroundedState.cs`, `AirborneState.cs`) a menos que el usuario lo solicite explícitamente.
- La base de red está **CONGELADA y es ESTABLE**. El movimiento, el flip visual (respetando la escala original del Prefab mediante `Mathf.Abs`) y el seguimiento de la cámara funcionan perfectamente aislando los inputs locales con `if (!IsOwner) return;`.

## 3. Sprint Actual: Sprint 1 (Infraestructura y Persistencia)
- **Último logro (Completado):** Se implementó `AudioManager.cs` (`Assets/_Project/Scripts/Core/Audio/AudioManager.cs`) utilizando el patrón de diseño Singleton y `DontDestroyOnLoad`. Expone dos `AudioSource` (Music y SFX) y cuenta con métodos para `SetMusicVolume`, `SetSFXVolume`, `MuteAll` y `PlaySFX`.
- **Enfoque Inmediato:** Validar la transición entre la "Escena Menú" y la "Escena Juego" para confirmar la persistencia del audio y continuar con las siguientes mecánicas solicitadas en la rúbrica.

## Notas para el Agente:
Lee este archivo siempre antes de proponer cambios estructurales. La prioridad es la **modularidad**, respetando la base ya estabilizada.
