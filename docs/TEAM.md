# Team

Five people, six workstreams, two semesters. One owner per workstream; everyone tests and everyone writes part of the report.

## Members and roles

Pick primary roles in week 1 and update this table (and the scene owners table in [CONTRIBUTING](../CONTRIBUTING.md#scenes-and-prefabs)).

| Member | Primary role | Also helps with | GitHub |
| --- | --- | --- | --- |
| Ciara McLaughlin | TBD | | |
| Elijah Lewis | TBD | | |
| Rickey Johnson | TBD | | @rickeyjohnson |
| Samaii Hartley | TBD | | |
| Tiara McLaughlin | TBD | | |

With five people and six roles, the producer also covers one workstream (usually Systems).

## Roles

| Role | Owns | Learns first | Issue label |
| --- | --- | --- | --- |
| Producer / tech lead | Schedule, Git, builds, code review, report outline, weekly build | Unity basics, Git + LFS, Quest build pipeline | — |
| Combat engineer | Physics hands, weapons, damage, hit feedback | Rigidbodies, joints, XRI grabbing | `area: combat` |
| Systems engineer | Stats, XP, skill tree, quests, inventory, save/load, exercise tracking | C#, ScriptableObjects, JSON save | `area: systems` |
| MR & UI engineer | Passthrough hub, room scan, gate portal, System windows, wrist panel | AR Foundation planes, world-space UI | `area: mr-ui` |
| AI & level designer | Enemy AI, gates, encounters, bosses, puzzles and traps | NavMesh, Behavior graphs, ProBuilder | `area: ai-levels` |
| Tech artist & audio | Toon shading, VFX, animation retargeting, sound | URP Shader Graph, Mixamo, Blender, Substance Painter | `area: art-audio` |

A sixth or seventh member would split AI & levels in two, or become a dedicated research and playtest lead.

## Rituals

| What | When | Length | Who |
| --- | --- | --- | --- |
| Sprint planning | Every other Monday | 45 min | Everyone |
| Check-in | Twice a week (async in chat is fine) | 10 min | Everyone: done, next, blocked |
| Weekly build | Fridays | 30 min | Everyone plays `main` on a headset |
| Sprint demo | End of each sprint | 30 min | Everyone demos on a headset |
| Advisor meeting | As scheduled | — | Producer + anyone presenting |

## How decisions get made

- **Small, inside one workstream:** the owner decides.
- **Crosses workstreams or changes the GDD:** propose it in a GitHub issue or a GDD comment, discuss at the next check-in, record it in [DECISIONS](DECISIONS.md).
- **Scope cuts:** producer proposes, team agrees at sprint planning.

## Shared resources

| Resource | Where |
| --- | --- |
| Live GDD | Claude Docs (link pinned in team chat); Markdown copy in [GDD.md](GDD.md) |
| Code and docs | This repo |
| Raw source art (`.blend`, `.psd`, `.spp`) | Shared drive `ArtSource/` |
| Headset booking | Shared calendar (set up in week 1) |
| Study data | University drive approved by the IRB, never the repo |
