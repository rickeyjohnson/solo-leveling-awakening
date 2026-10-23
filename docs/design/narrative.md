# Narrative (`NAR`)

| | |
| --- | --- |
| Owner | Whole team (one writer per act) |
| GDD section | [Narrative & story](../GDD.md#narrative--story) |
| Data | Dialogue and System lines (format to decide: ScriptableObject or CSV) |
| Status | Draft |

## Summary

An original side story in canon-era Seoul. The player is themselves: a broke E-rank hunter whom the System chooses as its second Player. Five acts take them from E-rank to S-rank. Canon characters appear as cameos; Jinwoo is never playable.

## Rules

- **NAR-1** The player is never named, seen or voiced. NPCs say "you" or "Hunter".
- **NAR-2** The System never explains why the player was chosen. A title card frames the game: *"An original story set in the world of Solo Leveling."* Hidden quests may leave hints.
- **NAR-3** No camera-moving cutscenes. Story happens in front of the player in real time (CMF-1).
- **NAR-4** In the hub, story arrives by phone calls and texts; in gates, by System messages and short guild radio chatter.
- **NAR-5** About 150 NPC lines, voiced by volunteers with subtitles always on. Fallback: text with sound cues. The System is never voiced.
- **NAR-6** No anime footage, soundtrack, voice clips, manhwa art or official logo (fan-project rules).

## Acts

| Act | Gates | Beat | Ends with |
| --- | --- | --- | --- |
| 1. The Weakest Hunter | Prologue | A cheap raid finds a hidden temple; Kang dies; the player is left for dead | *[You have qualified to become a Player.]*, waking up in the real room |
| 2. Leveling Up | Daily quest, Goblin Warren, Kasaka's Nest | Training, solo gates, the Association notices | Re-ranked to D; Go Gunhee wants a meeting |
| 3. Job Change | Castle of the Fallen Knight, Hive | The sealed trial, then the first C-rank gate | *[You are now a Shadow Assassin.]* |
| 4. Gates of Seoul | Frost Gate, Seoul Subway | A guild raid turns red; a dungeon break in the city | Re-ranked to A |
| 5. The Strongest | Spire Gate | A gate above the Han River no instrument can measure | The device breaks; S-rank; *[A new quest has arrived.]* |

## Cast

| Character | Canon? | Role |
| --- | --- | --- |
| The Player | Original | You |
| The System | Canon | Cold, bracketed voice; quest giver |
| Min Seo-yeon | Original | C-rank healer who leads the prologue raid; later your contact |
| Kang Dae-sik | Original | Veteran D-rank; dies in the temple |
| Woo Jinchul | Canon | Association inspector; phone calls; re-assessments |
| Go Gunhee | Canon | Association president; re-assessments and the ending |
| Cha Hae-In | Canon | Hunters Guild; invites you to the Frost Gate raid |
| Baek Yoonho | Canon | White Tiger Guild; holds the line in the Subway |

## System voice library (starter)

| Moment | Line |
| --- | --- |
| Awakening | [You have qualified to become a Player.] |
| Daily quest | [Daily Quest: Preparation to Become Strong has arrived.] |
| Calibration | [Stand still.] |
| Skill | [You have acquired a new skill: Dash.] |
| Level-up | [Level up!] |
| Abandon | [You have failed to complete the Daily Quest. You will now be moved to the Penalty Zone.] |
| Red Gate | [Warning: This gate cannot be exited until the boss is defeated.] |
| Boss down | [The dungeon's boss has been defeated. The gate will close shortly.] |
| Job Change | [Job Change complete. You are now a Shadow Assassin.] |
| Puzzle hint | [Hint: The plate requires weight.] |
| Ending | [A new quest has arrived.] |

## Open questions

- Dialogue data format (ScriptableObject per conversation, or one CSV for localization later).
- Who writes each act?
