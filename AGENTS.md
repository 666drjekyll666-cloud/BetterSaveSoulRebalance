# Better Save Soul Rebalance — Working Rules

Read `666drjekyll666-cloud/DevRules` before substantive work. `ENGINEERING_RULES.md`, `CI_POLICY.md`, `GIT_WORKFLOW.md`, and `PROJECT_BOOTSTRAP.md` apply here; this file adds project-specific constraints.

## Project identity

- Public mod: **Better Save Soul Rebalance**
- Repository: `666drjekyll666-cloud/BetterSaveSoulRebalance`
- Project / assembly / DLL: `BetterSaveSoulRebalance`
- Game: Graveyard Keeper 1.407
- Required DLC: Better Save Soul
- Stable BepInEx GUID: `nikich.graveyardkeeper.souldlcrebalance`
- The legacy namespace/class name `SoulDLCRebalance` / `SoulDLCRebalancePlugin` is retained intentionally; do not change it for cosmetic reasons.

## Product scope

The purpose is to integrate Better Save Soul rewards and progression into the base game's progression without making the DLC either a shortcut pack or a grind wall.

Do not turn this into a general Graveyard Keeper rebalance without explicit scope expansion. Game of Crone may be relevant for comparisons but is not a hard dependency.

## Balance contract

Use:

`audit -> verify -> propose -> approve -> implement narrowly -> test -> accept`

- Preserve meaningful vanilla progression.
- Preserve Better Save Soul's usefulness and identity.
- Fix the narrowest balance cause instead of broadly nerfing symptoms.
- Do not add grind merely because Soul Gratitude, Sin Shards, or technology points exist.
- Material balance changes require verified before/after values and user approval.
- `docs/BALANCE_SPEC.md` is the canonical approved balance contract for the current production line.

## Runtime contract

- Prefer deterministic one-time/event-bound runtime-data changes.
- Keep mutations idempotent and guarded against verified baseline states.
- If a target is neither in the verified stock state nor the already-applied desired state, leave it unchanged and log a concise warning.
- Avoid per-frame work, broad recurring searches, background workers, and excessive logging.
- Do not modify persistent save data unless explicitly approved.
- Do not add configuration UI unless configurable balance is explicitly requested.
- Local Soul Gratitude is a currency surcharge, not a physical inventory item.
- Manual-crafting Gratitude is charged only on successful completion; Remote Craft uses the game's own combined/proportional Gratitude path and must not receive the manual charge as well.

## Version and release workflow

- `main` is accepted stable public state only.
- Runtime work belongs in `dev/X.Y.Z` until explicit player acceptance.
- Research-only work belongs in clearly named research branches.
- Numbered DLLs are immutable and tied to exact source SHA plus CI artifact identity.
- GitHub Releases is the canonical download surface for accepted stable DLLs.
- The exact accepted artifact must be promoted to Releases without rebuilding different bytes under the same version.
- The user prefers a raw versioned DLL rather than a ZIP.

The legacy 1.0.0 DLL was already handed under the old project identity, so the clean public candidate uses **1.1.0** rather than reusing 1.0.0.

## Public documentation

`README.md` and GitHub Release notes must remain user-facing: describe what the mod does, requirements, installation, compatibility, and release behavior. Do not put private-development history, migration narrative, repository-cleanliness commentary, or old private-build upgrade instructions there.

Keep migration/provenance and acceptance details in engineering documents such as `docs/MIGRATION_PROVENANCE.md` and `docs/TEST_BUILD_LOG.md`.

## CI

- Hosted CI is a candidate/handoff gate, not a per-commit service.
- The verified managed-code toolchain builds on `ubuntu-latest`; keep it unless a concrete dependency requires Windows.
- Documentation-only changes do not justify hosted CI.
- Candidate artifacts use short retention.

## 1.1.0 acceptance gate

1.1.0 preserves the approved legacy 1.0.0 runtime balance logic and changes only public identity/version/build metadata plus the ready log string. Before stable release:

- clean Release build must succeed from the exact candidate source;
- the candidate must load under the preserved BepInEx GUID;
- selected Spiritualism prices and at least one affected grave recipe/local-Gratitude path must behave as expected;
- the player must explicitly accept the runtime/balance result.
