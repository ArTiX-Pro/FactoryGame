# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

FactoryGame is a Unity 6 (`6000.3.14f1`) grid-based factory-builder game (think Factorio/Satisfactory-lite): players place buildings on a grid that mine resources, convert them via recipes, and move items along conveyor belts. There is no README; this file is the primary orientation doc.

## Working with this repo

This is a Unity project — there is no CLI build/test/lint workflow. Changes are made by editing C# scripts and iterating in the Unity Editor.

- All gameplay code lives in `Assets/_PROJECT/`, compiled into the default `Assembly-CSharp` assembly (no custom `.asmdef` files split it up).
- `Assets/Plugins/` and the many root-level `*.csproj` files (TableForge, AssetInventory, Automator, Brain, VHierarchy, VFolders, AudioTool, Sirenix/Odin Inspector, Database.SQLite, etc.) are third-party editor tooling/asset-store packages — not part of gameplay code, generally not relevant unless a task explicitly touches them.
- Odin Inspector (`Sirenix.OdinInspector`) is used for editor UX on ScriptableObjects (e.g. `[Title]`, `[Button]` in `PlacedObjectSO`).
- To validate a change, open the project in the Unity Editor and Play — there are no automated tests for gameplay code (the `com.unity.test-framework` package is present but unused by `_PROJECT`).
- `.editorconfig` enforces explicit accessibility modifiers everywhere and disables `var` (`csharp_style_var_*` all `false`) — always use explicit types and explicit access modifiers.

## Code layout

```
Assets/_PROJECT/
  Features/
    GameManager.cs                 # top-level singleton; wires available buildings/recipes/resources into HUD
    GridBuildingSystem/
      Logic/                       # GridBuildingSystem, GridSO, Grid<T> integration, pathfinding wiring, states
        States/                    # BuildingState, DestroyState (GameState subclasses)
      Buildings/
        Logic/                     # PlacedObject / PlacedObjectSO base classes, ActiveBuilding, Ghost
        PlayerToBuild/              # concrete buildings: Convertor, Conveyor, Grabber, Miner, Warehouse
        ResourceNode/               # non-player-placed resource nodes mined by Miner
    Resources/Recipes/             # RecipeSO, RecipeButton (crafting recipes for Convertor)
    UI/                            # HUD, BuildingButton
  Shared/
    Grid.cs, Pathfinding.cs        # generic grid container + A*-style pathfinding
    TickSystem.cs                  # global fixed-interval tick (~0.2s), separate from Update
    Inventory/                     # Inventory, InventorySO, ItemSO, IItemHolder, InventoryWindow
    Interaction/                   # IInteractable, Interactor (raycast-based hover/interact + outline)
    Window/                        # BaseWindow, DragWindow, ObjectWindow<T> (generic UI window base)
    Effects/                       # MasterTween (custom tween system), ScreenShake, Shaker, Shockwave
    Utilities/                     # MasterTimer (pooled timer), SavingSystem (JSON save/load), InputManager
```

## Core architecture

**Namespace convention**: everything lives under `ArTiX.*` (e.g. `ArTiX.GridBuildingSystem`, `ArTiX.GridBuildingSystem.Buildings`, `ArTiX.GridBuildingSystem.Datas`, `ArTiX.Inventory`, `ArTiX.Interaction`, `ArTiX.Input`, `ArTiX.Utils`, `ArTiX.Utils.TickSystem`, `ArTiX.Utils.Window`, `ArTiX.FactoryGame.Buildings`, `ArTiX.FactoryGame.UI`, `ArTiX.FactoryGame.Datas`).

**Lazy-singleton MonoBehaviours**: `GridBuildingSystem`, `TickSystem`, `GameManager`, `Interactor`, `InputManager` all follow the same pattern — a static `Instance` getter that spawns a `GameObject` on first access, and `Awake()` self-destroys duplicate instances. When adding a new manager-style class, follow this pattern for consistency.

**Two clocks**: gameplay logic does *not* run on Unity's per-frame `Update` for simulation ticks. `TickSystem` (`Shared/TickSystem.cs`) fires `OnFirstTick` / `OnTick` / `OnLateTick` on a fixed interval (`tickInterval`, default 0.2s), and most building logic (conveyor movement, interaction raycasts, `ActiveBuilding` subclasses) subscribes to these rather than `Update`. `MasterTimer` (pooled, per-instance) is the other timing primitive, used for things like `Convertor`'s conversion progress.

**Grid & building placement** (`GridBuildingSystem.cs`):
- `Grid<T>` (`Shared/Grid.cs`) is a plain 2D array wrapper; `GridBuildingSystem` holds a `Grid<PlacedObject>` plus a `Pathfinding` instance kept in sync (a cell occupied by a building is also marked unwalkable).
- A `GameState` state machine (`BuildingState`, `DestroyState` in `Logic/States/`) governs placement mode, driven by `InputManager` events (`OnStartCreatingBuilding`, `OnFinishCreating`, `OnCancel`, `OnRotate`). `BuildingState` also supports drag-placement along a pathfound route (`Ghost` previews per cell).
- World↔grid conversion assumes the grid is centered at the origin (see `ConvertWorldPosToCellPos` / `ConvertCellPosToWorldPos` in `GridBuildingSystem.cs`) — don't assume a corner-anchored grid.

**PlacedObject hierarchy** (`Features/GridBuildingSystem/Buildings/Logic/`):
- `PlacedObject` (abstract `MonoBehaviour`, requires `BoxCollider`) is the base for anything placed on the grid; its `PlacedObjectSO` data asset defines the cell footprint (`Vector2Int[] cellArray`, rotatable) and prefab.
- `ActiveBuilding : PlacedObject` adds tick-driven behavior — subclasses implement `OnTickAction()` and are ticked every `TicksPerUpdate` ticks of the type (`First`/`Main`/`Late`) configured on their `ActiveBuildingSO`. `Miner` is the example: each tick it pulls one item from an adjacent `ResourceNode` into its `Inventory` if there's room, and deactivates itself when it has no remaining adjacent nodes.
- Concrete buildings live in `Buildings/PlayerToBuild/`: `Convertor` (recipe-based crafting via `RecipeSO`, opens `ConvertorWindow`), `Conveyor` (belt-linked item transport, see below), `Grabber`, `Miner`, `Warehouse`.

**Item transfer — `IItemHolder`**: `Inventory`, `Conveyor`, and other item-holding components implement `IItemHolder` (`Item`, `GiveItem`, `CanGiveItem`, `TakeItem`, `CanTakeItem`) as the common contract for moving a single item between two holders — this is the interface to implement when adding a new item-passing building.

**Conveyor belts**: `Conveyor.cs` has non-trivial internal logic — a private nested `ConveyorBelt` class tracks a doubly-linked chain of `Conveyor` instances (`previousConveyor`/`nextConveyor`, `FirstConveyor`/`LastConveyor`) and handles belt creation, appending, merging two belts when a placed conveyor bridges them, splitting a belt when one is removed from the middle, and reversing direction. Belt movement itself is driven by the belt's own tick subscription (`TICKS_PER_UPDATE = 5` main ticks) rather than each conveyor ticking independently. Read the full class before modifying belt topology logic — the merge/split/reverse cases are easy to get subtly wrong.

**Interaction**: `Interactor` (singleton) raycasts from the mouse every tick (not every frame) against an interaction layer mask, applies an `Outline` component to hovered `IInteractable`s, and calls `Interact()` on click via `InputManager`. Buildings that open a UI window (e.g. `Convertor`, `Miner`) implement `IInteractable`.

**Windows**: `BaseWindow` is the base UI panel (focus/close). `ObjectWindow<T>` is a generic base for windows bound to a specific object instance (e.g. `InventoryWindow : ObjectWindow<Inventory>`, `ConvertorWindow : ObjectWindow<Convertor>`), created via static `Create(...)` factory methods rather than `Instantiate` directly.

**Data-driven config**: gameplay tuning lives in `ScriptableObject`s (`*SO.cs`): `PlacedObjectSO`/`BuildingSO`/`ActiveBuildingSO` (footprint, prefab, tick rate), `RecipeSO` (crafting inputs/output/duration), `ItemSO` (stack size, prefab), `InventorySO` (max distinct item slots), `GridSO` (grid dimensions/visuals). `GameManager` holds the master lists of available buildings/recipes/resources exposed to the HUD.

**Saving**: `SavingSystem` (`Shared/Utilities/SavingSystem.cs`) is a minimal `JsonUtility`-based save/load to `Application.persistentDataPath` — currently only persists an array of `Vector2Int` cells (`SavedData`), i.e. it is not yet a full building-state save system.

## Conventions to follow

- Follow the lazy-singleton pattern shown above for any new global manager.
- Use `TickSystem` events for simulation logic instead of `Update()` unless something genuinely needs per-frame resolution (e.g. UI/camera smoothing).
- New item-passing components should implement `IItemHolder`.
- New tick-driven placed buildings should extend `ActiveBuilding` and implement `OnTickAction()` rather than hand-rolling tick subscriptions.
