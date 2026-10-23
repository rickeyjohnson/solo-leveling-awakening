# Skills & Shadow Extraction (`SKL`)

| | |
| --- | --- |
| Owner | Combat engineer |
| GDD sections | [Skills & abilities](../GDD.md#skills--abilities), [Shadow extraction](../GDD.md#shadow-extraction--shadow-army), [Skill tree](../GDD.md#skill-tree) |
| Code | `SLA.Skills` (to create); gesture detection lives here too |
| Data | `SkillData` assets, `CombatConfig` (Dash) |
| Status | Draft |

## Summary

Skills fire from a simple body gesture **plus a held button**, so they feel like techniques but never fire by accident. Up to four active skills are equipped. The skill tree upgrades skills; it doesn't unlock the base actives (those come from level and story). Shadow Extraction ("Arise") is a stretch goal.

## Rules

### Skills

- **SKL-1 Dash** (start): hold grip and lunge, meaning the controller moves **0.25 m in under 0.2 s**; fallback is left stick + grip. Blinks **3 m** that way with a brief comfort fade. Costs **15 Fatigue**; cooldown **4 s**, reduced 1% per Agility point above 10, floored at 40%.
- **SKL-2 Tenacity** (passive, Level 5): below 30% HP, damage taken −50%.
- **SKL-3 Dagger Rush** (Level 8): throw a held dagger; squeeze grip to recall it. Thrown hit = 1.5× damage. 5 MP, 2 s cooldown.
- **SKL-4 Bloodlust** (Level 12, story): hold both triggers and pull both fists down to your sides. Enemies within 6 m slowed 40% for 4 s; weak enemies flee. 15 MP, 30 s.
- **SKL-5 Stealth** (Job Change): hold both grips and crouch below 70% of standing height. Invisible 8 s or until you attack; the first strike from stealth crits ×3. 20 MP, 25 s.
- **SKL-6 Shadow Step** (Job Change): point the weapon at an enemy and press A. Blink to behind the target, facing it. 10 MP, 8 s.
- **SKL-7 Ruler's Authority** (Level 25): empty hand, hold grip, push or pull the palm at **2 m/s or more**. Telekinetic push (knockback) or pull (weapon or enemy toward you). 25 MP, 12 s.

### Resources

- **SKL-8** MP regenerates 1/s out of combat, 0.3/s in combat. Mana potions restore 30.
- **SKL-9** Fatigue fills from dashing, blocking and sprinting and drains when the player stops. At 100, Dash is locked until Fatigue falls below 50.
- **SKL-10** Cooldowns show as rings on the back of each hand and on the wrist panel ([UI-4](system-ui.md)).

### Gesture rules

- **SKL-11** Every gesture is armed by a held button (grip or trigger). No gesture alone fires a skill.
- **SKL-12** Detection is rule-based (velocity, position, headset height), not machine learning.
- **SKL-13** No two gestures share a starting pose; motions are big and distinct.
- **SKL-14** A new skill opens a System training window with a ghost-hand demo in the hub before the player needs it in a gate.
- **SKL-15** Every gesture has a button-only alternative in settings (CMF-9).
- **SKL-16** Accidental triggers must stay under **2%** of normal combat swings (measured in telemetry).

### Skill tree

- **SKL-17** 1 skill point per level gained above Level 10 (40 by Level 50). Node costs: first node 1, second 2, capstone 3; the full tree is 18 points. Respecs are free in the hub.
- **SKL-18** Branches: **Blade** (opens at D): Sharp Edge +10% blade damage → Dual Wield off-hand +20% → **Vital Strike** weak points ×3. **Shadow** (opens at Job Change): Long Shadow Stealth +4 s → Far Step Shadow Step +50% range → **Shadow Dance** chain 3 steps. **Body** (opens at C): Iron Body +10% max HP → Second Wind Fatigue regen +30% → **Ruler's Grip** push and pull ×2. Capstones need A-rank.

### Shadow Extraction (stretch)

- **SKL-19** For 6 s after a kill, the body emits purple mist and *[Extraction available]*.
- **SKL-20** Gesture: open hand over the body, squeeze grip + trigger, lift the hand at least 0.4 m in under 0.5 s.
- **SKL-21** Three attempts per corpse. Base chance: lower rank 90%, same rank 60%, one rank higher 30%, boss 15% (guaranteed on the third try for story bosses).
- **SKL-22** Shadows reuse their enemy AI with the team flipped. Max 3 active. Summoning a stored shadow costs 10 MP (30 elite).
- **SKL-23** Commands (arm with grip): point + trigger = attack; raised fist = regroup; palm down push = hold.
- **SKL-24** If Arise ships, it replaces Shadow Dance as the Shadow capstone and the Job is renamed Shadow Summoner.

## Data

`SkillData` per skill: `unlockLevel`, `mpCost`, `fatigueCost`, `cooldownSeconds`, gesture and effect text. Dash numbers live in `CombatConfig` (`dashDistance`, `dashCooldown`, `dashFatigueCost`, `dashCooldownReductionPerAgility`, `dashMinCooldownFraction`, `lungeDistance`, `lungeSeconds`).

## Acceptance criteria

- [ ] Each gesture fires reliably in 9 of 10 deliberate attempts by a new player after the training window
- [ ] Normal sword play never fires a skill (SKL-16 telemetry under 2%)
- [ ] Every skill works in button-only mode
- [ ] Dash never moves the player through walls

## Open questions

- Does Dash's lunge detection conflict with fast stabs? Spike it in week 3–4.
