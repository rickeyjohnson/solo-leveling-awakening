# Testing

Three tracks: weekly headset checks keep the game running, playtests keep it fun and comfortable, and one user study produces the report's data. **Everything below is measured on a Quest, never only in the editor.** Full rationale: the GDD's [Testing & evaluation plan](GDD.md#testing--evaluation-plan).

## Acceptance targets

| Area | Target | How it's measured |
| --- | --- | --- |
| Frame rate | 72 Hz held in every gate; under 1% dropped frames in a 10-minute run | OVR Metrics Tool, Unity Profiler |
| Squat counting | 95%+ of reps counted; at most 1 false count per 50 | Video-recorded sets, counted by a human vs. the game |
| Punch counting | 95%+ of pad hits registered | Same |
| Gesture skills | Accidental triggers on under 2% of normal combat swings | Telemetry |
| Comfort | Median SSQ total-score increase under 20 after a 20-minute session | SSQ before and after |
| Usability | SUS 68 or higher | SUS after the study session |
| Stability | No crash on the 30-minute expo path, 5 runs in a row | Manual runs on the expo build |
| Length | Main path takes about 2 hours for a first-time player | Playtest timing |

## Weekly headset check (every Friday)

1. Producer builds `main` to a Quest.
2. Each person plays for about 15 minutes, focusing on what changed this week.
3. Record frame rate in the hub and the newest gate (OVR Metrics overlay).
4. Every problem becomes an issue with a severity label before the weekend.

## Playtest rounds

| Round | When | Build | Testers | Focus |
| --- | --- | --- | --- | --- |
| 1 | Dec 7–11, 2026 | Vertical slice | 5 classmates outside the team | Combat feel, sickness, the portal moment |
| 2 | Feb 22 – Mar 5, 2027 | Content alpha (E–C) | 8 | Pacing, difficulty, onboarding, daily-quest length |
| Study | Mar 15 – Apr 2, 2027 | Beta | 20–30 | The research questions |
| Expo rehearsal | Apr 12–16, 2027 | Release candidate | 5 strangers | The 10-minute expo path, headset hygiene |

### Running a playtest

1. Clear the play area; run the headset's boundary setup.
2. Read the tester the consent and safety script (below). Tell them they can stop at any time.
3. SSQ before.
4. Let them play. **Don't help** unless they're stuck for more than 2 minutes or unsafe. Take notes on where they hesitate, what they say, what they miss.
5. SSQ after, plus 3 questions: what was best, what was confusing, did anything feel bad physically?
6. Wipe the headset face pad and controllers.

**Safety script:** "This game asks you to move: squat, punch, swing. Stop any time you feel dizzy, sick or tired; just say stop or take the headset off. Keep inside the boundary. Do you have any injuries or conditions that make squatting or fast arm movement a problem?"

## The user study

| | |
| --- | --- |
| Questions | RQ1 exercise + RPG progression; RQ2 diegetic UI vs. HUD (primary). RQ3 physics vs. arcade damage; RQ4 MR vs. virtual hub (secondary) |
| Design | Within-subjects (each person tries both conditions), counterbalanced order |
| Participants | 20–30 adult students |
| Session | About 45 minutes |
| Ethics | **IRB review before recruiting.** Submit by Jan 22, 2027 |
| Analysis | Paired t-test, or Wilcoxon signed-rank when data aren't normal; report effect sizes and confidence intervals |

### Instruments

| Instrument | Measures | Items |
| --- | --- | --- |
| Simulator Sickness Questionnaire (SSQ) | Nausea, eye strain, disorientation | 16 |
| System Usability Scale (SUS) | Usability | 10 |
| Igroup Presence Questionnaire (IPQ) | Presence | 14 |
| Physical Activity Enjoyment Scale (PACES) | Exercise enjoyment | 8–18 |
| Borg Rating of Perceived Exertion | Effort | 1 (6–20 scale) |
| Intrinsic Motivation Inventory, interest/enjoyment | Motivation | 7 |
| Player Experience Inventory (PXI) | Mastery, meaning, immersion | 30 |

### Study data rules

- **Participant data never goes in the repo.** `StudyData/` and `*.participant.csv` are git-ignored.
- Store it on the university drive the IRB approves, keyed by participant number, not name.
- Telemetry from the game (reps, swing speeds, deaths, time per room, UI events) is written to a local CSV per session and copied off the headset after each session.

## Automated tests

- **Unit tests** (Unity Test Framework, Edit Mode): damage formula, XP curve, drop tables, save/load round-trips.
- **Recorded-motion tests:** headset and controller poses from real squat and punch sets, saved as CSV in `Tests/Recordings/`, replayed through the detectors. Every change to a detector must pass all recordings.
- **Soak test:** a gate running with god mode and auto-spawning enemies for 30 minutes, watching memory and frame time.

## Bug severity

| Label | Meaning | Rule |
| --- | --- | --- |
| `S1 crash/sickness` | Crash, or anything that makes players sick (camera jolts, frame drops below 72 Hz) | Blocks the weekly build; fixed before new features |
| `S2 broken feature` | A feature doesn't work | Fixed within the sprint |
| `S3 minor` | Cosmetic or minor | Fixed when convenient |
