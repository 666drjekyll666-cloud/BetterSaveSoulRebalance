# Better Save Soul Rebalance — Test Build Log

Every handed DLL is immutable and tied to exact committed source plus build artifact identity.

## 1.0.0 — legacy handed balance candidate

- Legacy repository: `SoulDLCRebalance-legacy-private`.
- Exact runtime source: `ea5e62a7894e5959ce706c4497452c934159a6c2`.
- CI run: `34508205848`; artifact `SoulDLCRebalance-1.0.0` (`10164695855`).
- Raw DLL SHA-256: `04e7556214c80d5b91f2478edf73c2f350bf5e9c54ed78031bc805435f04dfe3`.
- Runtime scope: approved Spiritualism technology prices, grave-recipe/material changes, local manual-crafting Soul Gratitude surcharge, and combined Remote Craft Gratitude totals.
- Canonical legacy status: handed for player playtest; stable acceptance was not recorded.

## 1.1.0 — public migration candidate

- Date: 2026-09-11.
- Branch: `dev/1.1.0`.
- Runtime base: exact 1.0.0 production logic and `docs/BALANCE_SPEC.md`.
- Intended runtime balance changes relative to 1.0.0: none.
- Identity changes: public plugin name `Better Save Soul Rebalance`, assembly/DLL `BetterSaveSoulRebalance`, version `1.1.0`; BepInEx GUID and runtime namespace/class are preserved.
- Exact candidate/build source: `f9e47c07f2fbb2afd4687c9385ce7dd6a002f76a`.
- Frozen candidate ref: `candidate/1.1.0` at the same source commit.
- Clean CI: run `34642110816`, job `103404110517`, success with 0 warnings / 0 errors.
- Artifact: `BetterSaveSoulRebalance-1.1.0` (`10280322500`), archive digest `sha256:4873d4cad1b02cfa427f9276fd316c9a6e494cc5ec84d48411f51977782e7820`, retention 7 days.
- Raw DLL SHA-256: `6f45c4710e6f4557129bfdf5335d10d6252698d7a2d80ed066c662d555f41317`.
- A preceding pre-handoff build from `3eb3e554553e21f7d30a6c31510bfc56e4e4a8aa` compiled with one CS0108 warning because the private logging helper name `Info` hid an inherited `BaseUnityPlugin.Info` member. It was not handed to the player. The helper was renamed to `InfoOnce` with no intended runtime behavior change, then rebuilt cleanly as the candidate above.
- Requested player test:
  1. confirm the plugin loads as `Better Save Soul Rebalance 1.1.0`;
  2. inspect selected Spiritualism technology prices and confirm the expected Soul Gratitude/blue-point values;
  3. inspect at least one affected grave recipe and confirm its materials plus local Gratitude presentation;
  4. if convenient, complete one affected manual craft and confirm Gratitude is charged once on completion;
  5. report overall balance acceptance or any regression.
- Player result: **rejected, 2026-09-11**. With 1.1.0 loaded, the normal `CraftGUI` at Stone Cutter II became visibly corrupted and closing the crafting window produced a `CraftItemGUI.OnOut()` `NullReferenceException`. Repeating the same save/mod-stack test with only Better Save Soul Rebalance removed restored normal crafting UI behavior and produced no corresponding crafting exception.
- Root cause: the 1.1.0 local-Gratitude display prefix temporarily appended `gratitude_as_item` to the shared `CraftDefinition.needs` during `CraftItemGUI.Redraw()`. Verified game IL builds `_multiquality_ids` one entry per original need and later passes both lists together to `BaseItemCellGUI.DrawIngredients(...)`; the temporary extra need broke that parallel-list invariant. It also exposed display-only pseudo-data to other installed `CraftItemGUI.Redraw()` prefixes such as Queue Everything.
- Status: **rejected / immutable; do not publish or move `candidate/1.1.0`**.

## 1.1.1 — crafting UI safety fix

- Date: 2026-09-11.
- Branch: `dev/1.1.1`.
- Runtime base: frozen 1.1.0 balance behavior; no approved balance numbers changed.
- Fix: remove shared `CraftDefinition.needs` mutation from the local-Gratitude display path. Inject `gratitude_as_item` only into copied renderer arguments at `BaseItemCellGUI.DrawIngredients(...)`, extend the copied `_multiquality_ids` list in lockstep, and fail closed if the available ingredient-cell shape is insufficient or unexpected.
- Expected compatibility effect: Queue Everything / Max Buttons Redux and vanilla crafting logic continue to see the real physical recipe only; the pseudo-item exists only for the final ingredient renderer call.
- Exact candidate/build source: `d684b783408d23de212ccc1d91ac51add72edf93`.
- Frozen candidate ref: `candidate/1.1.1` at the exact build source above.
- Clean CI: run `34644829243`, job `103412980183`, successful Release build with 0 compiler warnings / 0 errors.
- Artifact: `BetterSaveSoulRebalance-1.1.1` (`10282001244`), archive digest `sha256:f154df6836a5bef026079f3a3062292a330f057ce3ec1fb64f26781d48d8aea2`, retention 7 days.
- Raw DLL SHA-256: `c17de0a66e590d93863ff47defd937ffefe82cf47bb2721749177025e5545cee`.
- Requested player test:
  1. reproduce the Stone Cutter II path that broke under 1.1.0 and confirm normal layout/navigation/close behavior;
  2. after unlocking an affected BSS grave recipe if necessary, confirm the local Soul Gratitude icon/value still appears with the physical ingredients;
  3. if convenient, complete one affected manual craft and confirm Gratitude is charged exactly once on completion;
  4. provide `LogOutput.log` if any crafting UI error or warning appears.
- Player result: pending.
- Status: **candidate / not accepted; do not release**.
