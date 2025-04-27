# Detailed Snake Game Implementation Plan

---

# Todos

1.  ~~Create main scenes: (`MainMenu`, `Game`, `GameOver`) and add them to build settings.~~ ✓
2.  ~~Create `SnakeSegment` prefab.~~ ✓
3.  ~~Create `Food` prefab.~~ ✓
4.  Create score system: (Requires `GameManager.cs`). ⚠️ Partially Done (Needs UI)
5.  Implement Snake Body/Tail Sprite Logic. ⚠️ Partially Done (Core logic implemented, needs gameplay verification)
6.  Implement UI (Menu, HUD, Game Over). ❌
7.  Add Sounds. ❌
8.  Gameplay Testing & Polish. ❌

---

## 1. Initial Setup (30 minutes)

**Create project structure:** ✓

- In Unity, right-click in Project window
- Create folders: "Scenes", "Scripts", "Prefabs", "Materials", "Sprites"

**Create main scenes:** ✓

- Create "MainMenu" scene (File > New Scene, save as "MainMenu" in Scenes folder) ✓
- Create "Game" scene (main gameplay) ✓
- Create "GameOver" scene ✓
- Add all scenes to build settings (`File > Build Settings` > `Add Open Scenes`) ✓

## 2. Core Game Elements (2 hours)

**Create snake head:** ✓

- Create empty GameObject named "Snake"
- Add SpriteRenderer component ✓
- Add BoxCollider2D component ✓
- Create C# script "SnakeController.cs" with: ✓
  ```csharp
  // Basic movement in 4 directions
  // Input handling (arrow keys)
  // Speed variable
  // Head sprite updates based on direction ✓
  ```

**Create snake body segment:** ✓

- Create empty GameObject named "SnakeSegment"
- Add SpriteRenderer component ✓
- Add BoxCollider2D component ✓
- Create as prefab ✓
- Create C# script "SnakeSegment.cs" with: ✓
  ```csharp
  // Store previous position
  // Follow logic
  // Needs logic for sprite changes (turns/tail) ⚠️ Partially Done
  ```

**Implement food system:** ✓

- Create empty GameObject named "Food"
- Add SpriteRenderer ✓
- Add CircleCollider2D component ✓
- Create as prefab ✓
- Create C# script "FoodSpawner.cs" with: ✓
  ```csharp
  // Random position generation within grid
  // Spawn method
  ```

**Create game grid:** ✓

- Create C# script "GridManager.cs" with: ✓
  ```csharp
  // Define grid size and cell size
  // Convert world position to grid position and vice versa
  // Check if position is valid
  ```
- Add visual background (Checkerboard) ✓

**Create score system:** ⚠️ Partially Done

- Create C# script "GameManager.cs": ✓
  ```csharp
  // Score variable and methods ✓
  // Game state management (Basic scene loading added) ✓
  // Scene transitions (Basic scene loading added) ✓
  // Needs UI connection ❌
  ```

## 3. Game Mechanics (1.5 hours)

**Implement snake growth:** ✓ (Core logic implemented)

- Modify SnakeController.cs:
  ```csharp
  // Add method to grow snake ✓
  // Create list to store body segments ✓
  // Instantiate new segment when food is collected ✓
  // Needs logic for updating body/tail sprites ⚠️ Partially Done
  ```

**Implement collision detection:** ✓ (Refined)

- Add to SnakeController.cs:
  ```csharp
  // OnTriggerEnter2D for food collection ✓ (Refined)
  // Check for self-collision ✓
  // Check for wall collision ✓
  ```
  _(Needs thorough gameplay testing)_

**Implement movement mechanics:** ✓ (Refined)

- Finish SnakeController.cs movement:
  ```csharp
  // Grid-based movement ✓ (Refined)
  // Direction change logic ✓
  // Update position on timer ✓
  // Input uses Input System ✓
  ```

## 4. UI Implementation (1 hour)

**Create start menu:** ❌

- Create Canvas in MainMenu scene
- Add title text
- Add "Play" button with event to load Game scene
- Style with simple colors

**Create game over screen:** ❌

- Create Canvas in GameOver scene
- Add "Game Over" text
- Add score display text
- Add "Restart" and "Main Menu" buttons with events
- Style with simple colors

**Create HUD:** ❌

- Create Canvas in Game scene
- Add score text in corner
- Style with simple colors
- Connect to GameManager score variable

**Add animations (optional):** ❌

- Simple food spinning animation
- Snake head direction indication
- Button hover effects

## 5. Polish and Testing (1 hour)

**Add simple sounds:** ❌

- Import free sound assets
- Add AudioSource components
- Play sounds for:
  - Food collection
  - Game over
  - Movement direction change
  - Button clicks

**Test gameplay:** ❌

- Check collision detection
- Verify score increments correctly
- Test game over conditions
- Verify UI functionality
- Test scene transitions

**Code cleanup:** ❌

- Add comments to all scripts
- Organize namespaces
- Remove debug code
- Ensure consistent code style
