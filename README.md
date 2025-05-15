# 2D Platformer Game – Multi-Character System in Unity

This is a 2D platformer project built in Unity (C#), featuring a modular character system with unique abilities, object interaction, and full game state management. The game is designed to demonstrate advanced gameplay logic, component-based architecture, and system-level coordination between multiple mechanics.

## 🔧 Features

- 🔁 Character switching system (runtime, UI-integrated)
- 🧊 Ice transformation ability (disables movement, interacts with traps)
- 🕳️ Burrow mechanic (DirtMound character enters underground state)
- 🧱 Wall climbing & sliding (Cat character with wall jump)
- 💥 Centralized death handling (spikes, falling, abilities)
- 🎯 Item interaction system (e.g. pick-up, carry, deliver)
- 📍 Checkpoint & respawn system (per character and per item)
- 🎬 Cinematic camera sequences (pre-game, scripted movement)
- 🧠 Game state control: win/lose conditions, collectible tracking, UI updates

## 🛠 Technologies

- **Engine:** Unity 2022+
- **Language:** C# (OOP, component-based)
- **Physics:** Rigidbody2D, custom movement logic
- **UI:** Unity Canvas system (buttons, icons, dynamic display)
- **Scene Management:** Prefab instantiation, PlayerPrefs, serialized references
