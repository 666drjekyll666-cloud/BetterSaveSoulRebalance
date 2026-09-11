# Verified Runtime Data — Graveyard Keeper 1.407 / Better Save Soul

This document records runtime facts the production mod is allowed to depend on. Unknown or materially different states fail closed.

## Initialization

- Better Save Soul availability is exposed through `DLCEngine.IsDLCSoulsAvailable()`.
- Runtime balance data is available from `GameBalance.me`.
- Technology definitions are read from `GameBalance.techs_data`.
- Craft definitions are read from `GameBalance.craft_data`.
- `TechDefinition.LinkTechs()` is a verified re-link/reload point; production reapplies guarded mutations after it.

## Technology prices

Technology prices use a game resource object with the verified keys used by this mod:

- `r`
- `g`
- `b`
- `v`
- `gratitude_points`

Production replaces the price only when the relevant target matches either the verified stock baseline or the already-applied desired state.

## Grave craft definitions

Affected Better Save Soul grave crafts expose:

- `needs` for physical ingredients;
- `gratitude_points_craft_cost` for the game's Gratitude-based Remote Craft cost.

Production changes only the approved craft IDs and leaves a target unchanged if its current recipe/cost is neither the verified stock state nor the desired state.

## Manual local Gratitude

Verified production patch points:

- `BaseCraftGUI.CanCraft(...)` for manual eligibility;
- `CraftComponent.CraftReally(...)` for manual start validation;
- `CraftComponent.FinishCurrentCraft()` for completion-time charge;
- `CraftItemGUI.Redraw()` for temporary native Gratitude requirement presentation.

The game-recognized pseudo-item `gratitude_as_item` is used only as a temporary UI entry and is removed after redraw.

The player's current Gratitude is exposed through `player.gratitude_points`.

## Remote Craft separation

`GlobalCraftControlGUI.is_global_control_active` distinguishes the Remote Craft UI/path. When it is active, the manual local-Gratitude patches do not add a second completion charge; the craft definition's combined `gratitude_points_craft_cost` remains authoritative for Remote Craft.

## Persistence and performance

- The mod does not write custom save data.
- Balance mutations are one-time/event-bound rather than per-frame work.
- Reflection is used against verified game types/members so the production assembly does not need copied game binaries or implementation stubs.
