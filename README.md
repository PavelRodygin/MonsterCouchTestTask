# Unity 2D Game — Test Task

A 2D game prototype built as a test assignment. The focus is on modular architecture, DI, input abstraction, and pragmatic performance optimization.

> Project Status: Work in Progress
>
> Most of the tasks from the assignment are implemented and playable. The project may still contain bugs or rough edges. Priority was given to architecture, input, optimization, and code quality over full gameplay polish.

## 📋 Task Requirements

### Main Menu
- 3 buttons: Play, Settings, Exit
- Navigation: Mouse and Keyboard
- Exit closes the application
- Play starts the 2D game
- Settings opens the settings screen

### Settings Screen
- Two checkboxes (no persistence required)
- Back button returns to Main Menu
- ESC returns to Main Menu

### Play (2D Game)
- Camera: green solid background at position (0, 0, -10)
- Player: keyboard controls, default Circle sprite, scale (0.2, 0.2, 0.2)
- Enemies: 1000 enemies that flee from the player
- Collision: enemies stop when touching the player (turn red)
- Boundaries: player and enemies stay within camera bounds
- ESC key returns to Main Menu

### Additional Requirements
- Full navigation using mouse OR keyboard alone
- UI implemented with uGUI (Unity UI)

---

## ✅ Implementation Status

### 🏗️ Architecture & Foundation
- Modular architecture; modules isolated
- MVP pattern across screens
- Dependency Injection with VContainer
- Input System abstraction (service layer) for gameplay/UI
- Factory pattern for Player/Enemy creation with DI
- Component-based Player: `Player`, `PlayerMoveController`, `PlayerGfx`, `PlayerSfx`
- Clean code structure and naming

### 🎮 Gameplay (Game Module)
- Camera: Orthographic, green background, (0, 0, -10)
- Player: 2D movement, screen clamping, camera injected via DI
- Enemies (1000): flee behavior, screen clamping
- Collision: Enemy turns red and stops on contact with Player
- ESC key: returns to Main Menu
- Prefab creator tool for Player/Enemy with correct 2D setup

### ⚙️ Performance
- Centralized, batched enemy updates in `EnemyManager` (limits per-frame work)
- Proximity-based collider activation (triggers enabled only near player)
- Orthographic bounds math (no per-frame ScreenToWorldPoint)

### 🧠 Async & Reactive
- UniTask-based async flows for module lifecycle and scene logic
- Reactive streams (R3/UniRx-style) for input/UI events
- Cancellation tokens + PlayerLoop integration for safe, frame-accurate operations

### 🐛 Known Issues
- UI/flow edge cases may exist
- Visuals are minimal by design

---

## 🎮 Gameplay Details

### Player
- Movement: WASD or Arrow keys
- Visual: Blue Circle sprite
- Physics: Kinematic 2D, constrained to screen bounds
- Scale: (0.2, 0.2, 0.2)

### Enemies
- AI: Constantly flee from player
- Count: 1000 (batched updates)
- Visual: White Circle; turns Red when stopped (on touch)
- Collision: Trigger-based detection vs Player
- Scale: (0.2, 0.2, 0.2)

### Camera
- Position: (0, 0, -10)
- Background: Green
- Projection: Orthographic

---

## 🏗️ Architecture

The project follows clean modular principles with separation of concerns.

### Core Principles
- Modular design: MainMenu (with state machine), Game
- MVP: Model–View–Presenter separation
- FSM via Stateless library for complex modules
- DI via VContainer
- Factories for decoupled object creation
- Component-based entities

### Module Structure
```
Simple Module
├── ModuleController      # Lifecycle management & state coordination
├── ModulePresenter       # Business logic & view updates
├── ModuleView            # UI components & user input
├── ModuleModel           # Data & state management
└── ModuleInstaller       # Dependency injection setup

Complex Module (with FSM)
├── ModuleController      # Lifecycle & state machine coordination
├── ModuleModel           # Data & StateMachine configuration (Stateless)
├── State1/
│   ├── State1Presenter   # State-specific business logic
│   └── State1View        # State-specific UI
├── State2/
│   ├── State2Presenter   # State-specific business logic
│   └── State2View        # State-specific UI
└── ModuleInstaller       # Dependency injection setup
```

### MainMenu Module Architecture (with FSM)
```
MainMenuModule
├── MainMenuModuleController    # Module lifecycle & state machine coordination
├── MainMenuModuleModel         # StateMachine (Stateless) configuration
├── MainMenuState/
│   ├── MainMenuStatePresenter  # Main menu logic (navigate to Game/Settings)
│   └── MainMenuStateView       # Main menu UI (buttons: Game, Settings)
├── SettingsState/
│   ├── SettingsStatePresenter  # Settings logic (audio controls, back)
│   └── SettingsStateView       # Settings UI (toggle, back button, ESC support)
└── MainMenuInstaller           # DI setup for all states

States: MainMenu ⇄ Settings
Triggers: OpenSettings, BackToMainMenu
```

### Game Module Architecture
```
GameModule
├── GameModuleController     # Module lifecycle
├── GamePresenter            # Presentation logic
├── GameView                 # UI components
├── GameManager              # Game flow & entity management
├── Player                   # Main player component
│   ├── PlayerMoveController # 2D movement logic
│   ├── PlayerGfx            # Visual effects
│   └── PlayerSfx            # Sound effects
├── Enemy                    # Enemy AI & behavior
└── EnemyManager             # Spawning & batched updates for 1000 enemies
```

### Dependency Injection Flow
```
VContainer (Scene Installer)
├── InputSystemService → gameplay & UI
├── Camera → Player & Enemy (bounds)
├── PlayerFactory → GameManager → creates Player with DI
└── EnemyFactory → EnemyManager → spawns Enemies with DI
```

---

## 📦 Project Structure (Simplified)
```
Assets/
├── CodeBase/
│   ├── Core/
│   │   ├── Infrastructure/      # Module system
│   │   ├── Patterns/            # MVP interfaces
│   │   └── UI/                  # Base UI components
│   ├── Services/
│   │   └── Input/               # Input system abstraction
│   └── Editor/
│       └── Tools/               # Dev tools
│
├── Modules/
│   └── Base/
│       ├── MainMenu/
│       │   ├── Scripts/
│       │   │   ├── MainMenuState/      # Main menu state (play, settings buttons)
│       │   │   ├── SettingsState/      # Settings state (audio, back)
│       │   │   ├── MainMenuModuleController.cs
│       │   │   ├── MainMenuModuleModel.cs  # StateMachine config
│       │   │   └── MainMenuInstaller.cs
│       │   └── Scenes/
│       └── Game/
│           ├── Scripts/
│           │   ├── Gameplay/
│           │   │   ├── Player/
│           │   │   ├── Enemy/
│           │   │   └── GameManager.cs
│           │   ├── GameModuleController.cs
│           │   ├── GamePresenter.cs
│           │   └── GameView.cs
│           ├── Scenes/
│           │   └── Game.unity
│           └── Prefabs/
│               ├── Player2D.prefab
│               └── Enemy2D.prefab
│
└── Resources/
```

---

## 🚀 Quick Start

### Prerequisites
- Unity 2022.3 LTS or newer
- .NET Standard 2.1

### Setup
1. Clone the repository and open in Unity
2. Create prefabs (if missing):
   - Tools → Game Module → Create 2D Prefabs → Create Both Prefabs
   - Prefabs saved to `Assets/Modules/Base/Game/Prefabs/`
3. Assign references (if needed) in scene installers:
   - GameModuleInstaller → Player Prefab, Enemy Prefab
4. Open the entry scene and press Play

### Controls
- Menu: Mouse or Arrow Keys + Enter/Space
- Player: WASD or Arrow Keys
- Return to Menu: ESC

---

## 🔧 Technologies & Libraries

### Core
- Unity 2022.3 LTS, C# (.NET Standard 2.1)
- uGUI (Unity UI)

### Architecture & Patterns
- VContainer (Dependency Injection)
- MVP (Model–View–Presenter)
- Stateless (Finite State Machine library for complex modules)
- Factory pattern
- Component-based entities

### Async & Reactive
- UniTask (async/await in Unity loops)
- Reactive streams (R3/UniRx-style) for input/UI events

---

## 🎯 Key Implementation Details (Snippets)

### Player Movement (2D)
```csharp
// Keyboard input → 2D movement with screen bounds
Vector2 input = inputService.InputActions.Player.Move.ReadValue<Vector2>();
Vector3 delta = new Vector3(input.x, input.y, 0) * speed * Time.deltaTime;
transform.position = ClampToScreen(transform.position + delta);
```

### Enemy Flee Behavior
```csharp
// Flee away from player, with fallback for zero vector
Vector2 dir = ((Vector2)transform.position - playerPos).normalized;
if (dir == Vector2.zero) dir = Random.insideUnitCircle.normalized;
transform.position += (Vector3)(dir * fleeSpeed * dt);
```

### Orthographic Screen Bounds
```csharp
float h = camera.orthographicSize;
float w = h * camera.aspect;
Vector2 bounds = new(w, h);
position.x = Mathf.Clamp(position.x, -bounds.x + margin, bounds.x - margin);
position.y = Mathf.Clamp(position.y, -bounds.y + margin, bounds.y - margin);
```

### EnemyManager Batched Tick
```csharp
// Update only a subset per frame
int toUpdate = Mathf.Min(maxEnemiesUpdatedPerFrame, enemies.Count);
for (int i = 0; i < toUpdate; i++)
  enemies[(cursor + i) % enemies.Count].Tick(playerPos, dt, screenBounds);
cursor = (cursor + toUpdate) % enemies.Count;
```

---

## 🧩 Editor Tools
- Prefab Creator: Tools → Game Module → Create 2D Prefabs
  - Creates Player2D and Enemy2D with proper 2D components (SpriteRenderer, CircleCollider2D, Rigidbody2D, scripts)

---

## 📌 Notes
- This repository primarily demonstrates modularity, input abstraction, DI, async/reactive patterns, and performance techniques for many simple agents (1000 enemies).
