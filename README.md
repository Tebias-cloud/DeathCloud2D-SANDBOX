# DeathCloud 2D - Professional Sandbox

A high-performance 2D platformer framework built with Unity 6, utilizing modern architectural patterns for scalable game development.

## 🏗 Architecture Overview

This project follows a **Decoupled Modular Architecture** to ensure clean separation of concerns:

- **State Machine Pattern**: The player movement and mechanics are managed via a robust, interface-driven state machine. This allows for complex behaviors (Grappling, Dashing, Airborne states) to be easily extended without spaghetti code.
- **Input System (Modern)**: Fully integrated with the new Unity Input System, using an `InputReader` ScriptableObject as a bridge to decouple input logic from actor behavior.
- **ScriptableObject-Driven Data**: Player statistics, tuning, and configurations are stored in ScriptableObjects, enabling real-time balancing and clear data/logic separation.

## 📂 Project Structure

Following industry standards, the project is organized under a root `_Project` directory to isolate game assets from external packages:

- `_Project/Scripts/Core`: fundamental systems (Input, Audio, Global Managers).
- `_Project/Scripts/Features`: self-contained gameplay modules (Player, Enemies, World).
- `_Project/Settings/`: configurations for Input, Rendering (URP), and Physiscs.
- `_Project/Legacy/`: archived components for reference during migration.

## 🚀 Getting Started

1. **Unity Version**: Ensure you are using Unity 6 (6000.x) or newer.
2. **Setup**: The project uses URP. If materials appear pink, go to `Window > Rendering > Render Pipeline Converter`.
3. **Input**: Input settings are located in `_Project/Settings/Input`. Controls are mapped for Keyboard (WASD/Space/E) and Gamepads.

## 🛠 Features

- [x] Advanced 2D State Machine
- [x] Grappling Hook with tension physics
- [x] Coyote Time & Jump Buffer
- [x] Dash mechanics with gravity control
- [x] Professional project organization
