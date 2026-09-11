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
- Exact candidate/build source: pending bootstrap commit.
- Candidate ref: pending.
- CI run/artifact/hash: pending.
- Requested player test:
  1. confirm the plugin loads as `Better Save Soul Rebalance 1.1.0`;
  2. inspect selected Spiritualism technology prices and confirm the expected Soul Gratitude/blue-point values;
  3. inspect at least one affected grave recipe and confirm its materials plus local Gratitude presentation;
  4. if convenient, complete one affected manual craft and confirm Gratitude is charged once on completion;
  5. report overall balance acceptance or any regression.
- Player result: pending.
- Status: **candidate / not accepted yet**.
