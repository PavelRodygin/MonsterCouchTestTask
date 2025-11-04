# Unity 2D Game - Test Task

A small 2D game built with Unity as a test assignment, featuring modular architecture with MVP pattern, dependency injection, and clean code principles.

> ⚠️ **Project Status**: Work in Progress
> 
> The game is **not fully complete** and contains bugs. Due to time constraints, the focus was placed on:
> - ✅ **Modular Architecture** - Clean, scalable module system
> - ✅ **Input System** - Proper input handling and abstraction
> - ✅ **Performance Optimization** - Boundary calculations, object management
> - ✅ **Code Quality** - Clean code, DI, component-based design
> 
> The gameplay functionality is partially implemented and requires additional polish and bug fixes.

## 📋 Task Requirements

### Main Menu
- 3 buttons: **Play**, **Settings**, **Exit**
- Navigation support: Mouse and Keyboard
- Exit button shuts down the game
- Play button starts the 2D game
- Settings button opens settings screen

### Settings Screen
- 2 checkboxes: Checkbox 1 and Checkbox 2
- Back button to return to Main Menu
- ESC key returns to Main Menu
- No data persistence required

### Play (2D Game)
- **Camera**: Green solid background at position (0, 0, -10)
- **Player**: Controlled with keyboard, rendered as Circle sprite at scale (0.2, 0.2, 0.2)
- **Enemies**: 1000 enemies running away from the player
- **Collision**: Enemies stop moving when touched by player
- **Boundaries**: Player and enemies stay within screen bounds
- **Exit**: ESC key returns to Main Menu

### Additional Requirements
- Full navigation using mouse OR keyboard alone
- UI implemented with uGUI (Unity UI)

---

## ✅ Implementation Status

### 🏗️ Architecture & Foundation (Main Focus)
- ✅ **Modular architecture** with complete isolation between modules
- ✅ **MVP pattern** implementation across all modules
- ✅ **Dependency Injection** with VContainer
- ✅ **Input System** abstraction and proper handling
- ✅ **Factory pattern** for entity creation with DI integration
- ✅ **Component-based** player architecture (Move, Gfx, Sfx)
- ✅ **Screen bounds optimization** using camera orthographic size
- ✅ **Clean code** structure and naming conventions

### ⚠️ Gameplay Features (Partially Implemented)

#### Main Menu Module
- ⚠️ Main menu UI structure created
- ⚠️ Basic navigation (may have bugs)
- ⚠️ Module switching implemented

#### Game Module (2D Gameplay)
- ✅ Camera setup with green background at (0, 0, -10)
- ✅ Player controller with 2D movement system
- ✅ Enemy AI with flee behavior logic
- ✅ Screen boundary constraints (optimized calculations)
- ✅ Keyboard input handling (WASD/Arrow keys)
- ⚠️ 1000 enemies spawning (may have performance issues)
- ⚠️ Collision detection (implemented but needs testing)
- ⚠️ Visual feedback (color changes)
- ⚠️ ESC key to return to menu (may not work reliably)

### 🐛 Known Issues
- Game may have bugs in enemy spawning and behavior
- UI navigation might not work correctly in all cases
- Performance optimization needed for 1000 enemies
- Collision detection requires additional testing
- Module transitions may have edge cases

### 🎮 Gameplay Features

#### Player
- **Movement**: WASD or Arrow keys
- **Visual**: Blue Circle sprite
- **Physics**: 2D Kinematic with screen bounds
- **Scale**: (0.2, 0.2, 0.2)

#### Enemies
- **AI**: Flee behavior - always run away from player
- **Count**: 1000 enemies
- **Visual**: White Circle sprite (turns Red when stopped)
- **Collision**: Trigger-based detection
- **Boundaries**: Stay within screen bounds
- **Scale**: (0.2, 0.2, 0.2)

#### Camera
- **Position**: (0, 0, -10)
- **Background**: Green solid color
- **Projection**: Orthographic (2D)
- **Size**: Adjustable (default 5-10 units)

---

## 🏗️ Architecture

The project follows **clean modular architecture** principles with clear separation of concerns.

### Core Principles
- **Modular Design**: Each module (MainMenu, Settings, Game) is isolated
- **MVP Pattern**: Model-View-Presenter separation
- **Dependency Injection**: VContainer for loose coupling
- **Factory Pattern**: Player creation with DI integration
- **Component-Based**: Player components (Move, Gfx, Sfx)

### Module Structure

```
Module
├── ModuleController      # Lifecycle management & state coordination
├── ModulePresenter       # Business logic & view updates
├── ModuleView           # UI components & user input
├── ModuleModel          # Data & state management
└── ModuleInstaller      # Dependency injection setup
```

### Game Module Architecture

```
GameModule
├── GameModuleController     # Module lifecycle
├── GamePresenter           # Presentation logic
├── GameView               # UI components
├── GameManager            # Game flow & entity management
├── Player                 # Main player component
│   ├── PlayerMoveController    # 2D movement logic
│   ├── PlayerGfx               # Visual effects
│   └── PlayerSfx               # Sound effects
├── Enemy                  # Enemy AI & behavior
└── EnemyManager          # Spawning & managing 1000 enemies
```

### Dependency Injection Flow

```
VContainer (Scene Installer)
├── InputSystemService → Player & Enemy
├── Camera → Player & Enemy (boundary calculations)
└── PlayerFactory → GameManager
    └── Creates Player with all dependencies injected
```

---

## 📦 Project Structure

```
Assets/
├── CodeBase/                    # Core framework
│   ├── Core/
│   │   ├── Infrastructure/      # Module system
│   │   ├── Patterns/           # MVP interfaces
│   │   └── UI/                 # Base UI components
│   ├── Services/
│   │   ├── Input/              # Input system
│   │   └── SceneInstaller/     # DI installers
│   └── Editor/
│       └── Tools/              # Development tools
│
├── Modules/
│   └── Base/
│       ├── Bootstrap/          # Entry point
│       ├── MainMenu/          # Main menu module
│       ├── Settings/          # Settings module (if separate)
│       └── Game/              # 2D Game module
│           ├── Scripts/
│           │   ├── Gameplay/
│           │   │   ├── Player/        # Player components
│           │   │   ├── Enemy/         # Enemy AI
│           │   │   └── GameManager.cs # Game flow
│           │   ├── GameModuleController.cs
│           │   ├── GamePresenter.cs
│           │   └── GameView.cs
│           ├── Scenes/
│           │   └── Game.unity         # Game scene
│           └── Prefabs/
│               ├── Player2D.prefab    # Player prefab
│               └── Enemy2D.prefab     # Enemy prefab
│
└── Resources/
```

---

## 🚀 Quick Start

### Prerequisites
- **Unity Version**: 2022.3 LTS or newer
- **.NET Standard**: 2.1
- **Platform**: Windows, macOS, Linux, WebGL

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Add project from disk
   - Select Unity 2022.3 LTS or newer

3. **Create Prefabs** (if not created)
   - In Unity Editor: `Tools → Game Module → Create 2D Prefabs`
   - Click "Create Both Prefabs"
   - Prefabs will be created in `Assets/Modules/Base/Game/Prefabs/`

4. **Setup Scene** (if needed)
   - Open `Assets/Modules/Base/Game/Scenes/Game.unity`
   - Assign prefabs in Inspector:
     - GameModuleInstaller → Player Prefab
     - EnemyManager → Enemy Prefab

5. **Play the Game**
   - Open the main entry scene in Unity Editor
   - Press Play button
   - Navigate through menus (if working)
   
   > **Note**: Due to incomplete state, some features may not work as expected

### Controls
- **Menu Navigation**: Mouse or Arrow Keys + Enter/Space
- **Player Movement**: WASD or Arrow Keys
- **Return to Menu**: ESC key

---

## 🔧 Technologies & Libraries

### Core Technologies
- **Unity 2022.3 LTS**: Game engine
- **C# (.NET Standard 2.1)**: Programming language
- **URP 2D**: Universal Render Pipeline for 2D

### Architecture & Patterns
- **VContainer**: Dependency Injection framework
- **UniTask**: Async/await operations
- **R3**: Reactive Extensions (event-driven architecture)
- **MVP Pattern**: Model-View-Presenter separation

### Additional Libraries
- **DOTween** (optional): Smooth UI animations
- **TextMeshPro**: Advanced text rendering
- **Unity Input System**: Modern input handling

---

## 🎯 Key Implementation Details

### Player Movement (2D)
```csharp
// Keyboard input → 2D movement with screen bounds
Vector2 input = InputSystem.Move.ReadValue<Vector2>();
Vector3 movement = new Vector3(input.x, input.y, 0) * speed * Time.deltaTime;
position = ClampToScreen(position + movement);
```

### Enemy AI (Flee Behavior)
```csharp
// Always flee from player
Vector2 fleeDirection = (enemyPos - playerPos).normalized;
newPosition = enemyPos + fleeDirection * fleeSpeed * Time.deltaTime;
```

### Screen Boundaries
```csharp
// Calculate bounds from orthographic camera
float height = Camera.orthographicSize;
float width = height * Camera.aspect;
Vector2 bounds = new Vector2(width, height);

// Clamp with margin
position.x = Mathf.Clamp(position.x, -bounds.x + margin, bounds.x - margin);
position.y = Mathf.Clamp(position.y, -bounds.y + margin, bounds.y - margin);
```

### Collision Detection
```csharp
// Enemy detects player collision
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Stop(); // Enemy stops and changes color to red
    }
}
```

---

## 📝 Development Notes

### Editor Tools
- **Prefab Creator**: `Tools → Game Module → Create 2D Prefabs`
  - Automatically creates Player2D and Enemy2D prefabs
  - Uses Unity's default Circle sprite (Knob)
  - Sets up all required components (Rigidbody2D, Colliders, Scripts)

### Scene Setup
- **GameManager**: Handles game flow and player spawning
- **EnemyManager**: Spawns and manages 1000 enemies
- **Camera Setup**: Configures camera for 2D gameplay

### Performance Considerations
- **1000 Enemies**: All active simultaneously
- **Kinematic Physics**: No gravity or complex physics calculations
- **Trigger Collisions**: Lightweight collision detection
- **Simple AI**: Minimal calculations per enemy

---

## 🎨 Visual Design

### Color Scheme
- **Background**: Green (as per requirements)
- **Player**: Blue Circle
- **Enemy (active)**: White Circle
- **Enemy (stopped)**: Red Circle

### Sprites
- **Circle Sprite**: Unity's built-in Knob sprite (`UI/Skin/Knob.psd`)
- **Scale**: (0.2, 0.2, 0.2) for both player and enemies

---

## 📊 Code Quality

### Best Practices Followed
- ✅ Clean code principles
- ✅ SOLID principles
- ✅ Dependency Injection
- ✅ Component-based architecture
- ✅ Clear naming conventions
- ✅ Code documentation
- ✅ Modular structure

### Code Comments
- All comments and logs in English
- Concise and meaningful
- Not excessive (as per project rules)

---

## 🎯 Development Focus & Priorities

### What Was Prioritized
This project demonstrates a **strong architectural foundation** rather than complete gameplay:

1. **Modular Architecture** (Primary Focus)
   - Complete module isolation and independence
   - Clean separation of concerns (MVP pattern)
   - Scalable structure for future development

2. **Input System** (Primary Focus)
   - Proper abstraction layer
   - Support for multiple input methods
   - Clean integration with game systems

3. **Performance & Optimization** (Primary Focus)
   - Efficient boundary calculations
   - Optimized camera-to-world space conversions
   - Component-based architecture for reusability

4. **Code Quality** (Primary Focus)
   - Clean code principles
   - Dependency Injection throughout
   - Proper use of patterns (Factory, MVP, Component)

### What Was Not Completed
Due to time constraints, gameplay implementation was not finished:
- Full UI flow and navigation
- Complete enemy AI behavior
- Thorough testing and bug fixes
- Visual polish and feedback
- Edge case handling

### Takeaway
This project showcases **software engineering skills** and **architectural thinking** rather than a polished game. The foundation is solid and extensible, making it easy to complete the gameplay features with additional time.

## 🤝 About

Built as a test assignment to demonstrate:
- ✅ **Architectural skills**: Clean, modular, scalable structure
- ✅ **Unity knowledge**: Proper patterns and systems usage
- ✅ **Code quality**: Professional-level code organization
- ✅ **Problem-solving**: Input abstraction, optimization, DI integration
- ⚠️ **Time management**: Prioritized foundation over feature completion

---

## 📄 License

[Specify license]

---

**Unity 2D Game Test Task** - Clean architecture demonstration with 2D gameplay mechanics.
