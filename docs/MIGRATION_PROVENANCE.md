# Public Migration Provenance

The public repository starts a clean Git history and does not import the legacy private development history.

## Legacy production candidate

- Legacy private repository: `666drjekyll666-cloud/SoulDLCRebalance-legacy-private`
- Legacy handed version: `1.0.0`
- Exact runtime source: `ea5e62a7894e5959ce706c4497452c934159a6c2`
- CI run: `34508205848`
- Artifact: `SoulDLCRebalance-1.0.0` (`10164695855`)
- Raw DLL SHA-256: `04e7556214c80d5b91f2478edf73c2f350bf5e9c54ed78031bc805435f04dfe3`
- Legacy repository status: handed for playtest; stable acceptance was not recorded in its canonical test log.

The later legacy `dev/1.0.0` head only changed workflow/rules/test-record bookkeeping relative to that frozen runtime source; production source and balance logic were unchanged.

## Public production line

The first clean public candidate is `1.1.0` because `1.0.0` already identified an immutable handed binary.

1.1.0 preserves the 1.0.0 runtime balance code and stable BepInEx GUID. Intentional production differences are limited to:

- public plugin name: `Better Save Soul Rebalance`;
- project / assembly / DLL name: `BetterSaveSoulRebalance`;
- version: `1.1.0`;
- ready log text uses current plugin name/version.

The runtime namespace/class name remains `SoulDLCRebalance` / `SoulDLCRebalancePlugin` to avoid cosmetic code churn.

The legacy runtime-audit archive and research/prototype branch history are intentionally excluded from the public production repository.
