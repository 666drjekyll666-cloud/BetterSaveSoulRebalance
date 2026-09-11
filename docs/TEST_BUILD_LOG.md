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
- Player result: pending.
- Status: **candidate / not accepted yet**.
