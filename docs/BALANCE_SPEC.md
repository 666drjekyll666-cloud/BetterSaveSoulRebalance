# Better Save Soul Rebalance — Approved Balance Spec

Game: Graveyard Keeper 1.407  
Required DLC: Better Save Soul  
Production BepInEx GUID: `nikich.graveyardkeeper.souldlcrebalance`

## Scope

This production line implements the approved Better Save Soul progression/economy package. It does not change Better Save Soul grave-quality values, vanilla grave technologies, save data, Soul Gratitude capacity inflation from stored items, or the rejected cross-branch Theology prerequisite prototype.

## Spiritualism technology prices

Keep stock red/green/versatility prices unless explicitly changed below. Soul Gratitude values are calibrated against the intuitive one-container equipment ladder: 40 starter core -> 55 tier-II core -> 70 tier-III core. Extra containers, crematoria and pallets are optional capacity buffer rather than mandatory progression assumptions.

| Technology | Approved R/G/B/GP |
| --- | ---: |
| `soul_sins_1` | 0 / 0 / 0 / **0** |
| `soul_sins_2` | 10 / 15 / 0 / **10** |
| `soul_stone_fences` | 50 / 50 / **150** / **30** |
| `soul_buildings` | 20 / 15 / 0 / **40** |
| `soul_marble_fences` | 100 / 75 / **200** / **40** |
| `soul_sins_3` | 15 / 20 / 0 / **55** |
| `soul_stone_statues` | 80 / 150 / **250** / **45** |
| `soul_marble_statues` | 120 / 170 / **300** / **60** |
| `soul_buildings_2` | 30 / 20 / 0 / **55** |
| `soul_sins_4` | 25 / 25 / 0 / **70** |
| `soul_church_additions` | 25 / 50 / 0 / **20** |
| `soul_totem_tech` | 50 / 30 / 0 / **25** |
| `soul_writing_additions` | 40 / 35 / 10 / **35** |
| `soul_garden_additions` | 30 / 50 / 0 / **35** |

`soul_sins_1` and `soul_sins_2` retain their stock GP values. The four grave technologies add the approved 150/200/250/300 blue progression while retaining their stock red/green prices.

## Grave decoration recipes and recurring soul-resource costs

Grave quality/output values remain unchanged.

| Craft | Approved physical needs | Local GP | Sin Shards |
| --- | --- | ---: | ---: |
| `grave_bot_stn_6` | 1 `stone_plate_2` + 1 `stone_plate_3` | **5** | 0 |
| `grave_bot_stn_7` | 2 `stone_plate_2` + 1 `stone_plate_3` + 2 `detail_3` | **7** | 0 |
| `grave_bot_stn_8` | 2 `stone_plate_2` + 1 `stone_plate_3` + 1 `jewelry_detail_gold` | **10** | 0 |
| `grave_bot_mrb_6` | 1 `marble_plate_2` + 1 `marble_plate_3:1` | **10** | 0 |
| `grave_bot_mrb_7` | 2 `marble_plate_2` + 1 `marble_plate_3:2` + 2 `detail_3` | **12** | 0 |
| `grave_bot_mrb_8` | 2 `marble_plate_2` + 1 `marble_plate_3:3` + 1 `jewelry_detail_gold` | **15** | 0 |
| `grave_top_sculpt_stn_4` | 2 `stone_plate_2` + 1 `stone_plate_3` + 2 `detail_2` | **12** | 0 |
| `grave_top_sculpt_stn_5` | stock heavy recipe + 1 `sin_shard` | **15** | **1** |
| `grave_top_sculpt_mrb_4` | stock heavy recipe + 1 `sin_shard` | **15** | **1** |
| `grave_top_sculpt_mrb_5` | stock heavy recipe + 2 `sin_shard` | **15** | **2** |

Exact retained heavy physical recipes:

- `grave_top_sculpt_stn_5`: 2 `stone_plate_2` + 2 `stone_plate_3` + 2 `detail_2`;
- `grave_top_sculpt_mrb_4`: 3 `marble_plate_2` + 2 `marble_plate_3:3` + 2 `detail_2`;
- `grave_top_sculpt_mrb_5`: 3 `marble_plate_2` + 3 `marble_plate_3:3` + 2 `detail_2`.

## Local GP semantics

Local GP is a currency surcharge, not a physical inventory item.

Manual crafting:

- the recipe UI shows the local Soul Gratitude requirement using the game's native Gratitude pseudo-item presentation;
- eligibility requires enough current Gratitude for the selected craft amount;
- local GP is charged once for each successfully completed decoration;
- cancelling an unfinished manual craft charges no local GP;
- if Gratitude falls below the per-item local cost while a craft is already in progress, completion waits rather than allowing a free or negative-GP result.

Remote Craft Control:

- stock Remote Craft convenience GP remains in force;
- the remote GP definition is increased by the approved local surcharge so one total is paid through the stock Remote Craft path;
- stock proportional spend/refund behavior remains untouched;
- a Remote Craft must not also receive the manual completion charge.

Verified stock remote fees are 5 GP for the Better Save Soul fences, 10 GP for the two stone sculptures, and 15 GP for the two marble sculptures. Approved combined remote fees are therefore 10/12/15/15/17/20/22/25/30/30 GP in the table order above.

## Explicit exclusions

- No cross-branch Theology prerequisite or prerequisite badge/UI.
- No grave quality reduction.
- No changes to Soul Gratitude capacity definitions or legal room slots.
- No restriction of container-content capacity inflation.
- No unrelated vanilla or other-DLC rebalance.
- No save migration or persistent save-data rewrite.

## Runtime safety contract

Production mutations must be one-time/event-bound, idempotent and guarded against verified stock baselines. A target whose relevant baseline is neither verified stock nor this approved state must be left unchanged with a concise warning rather than blindly overwritten.
