# Workflow: Playtest report

Input: raw notes from a playtest session (typed or transcribed), SSQ scores, and any telemetry CSV summary. **Never include participant names or identifying details**; use P1, P2 ...

Produce `docs/playtests/YYYY-MM-DD-<round>.md` with:

1. **Session:** date, build (commit), headset(s), number of testers, how many were new to VR, comfort preset used.
2. **Headline:** the three most important findings, one line each.
3. **Comfort:** SSQ before/after (median and range), anyone who stopped early and why. Any CMF rule broken → file an `S1` issue.
4. **Findings table:**

   | # | What happened | How many testers | Severity (S1/S2/S3) | Spec rule | Suggested fix |
   | --- | --- | --- | --- | --- | --- |

5. **Quotes:** up to five short, anonymized quotes that explain a finding.
6. **Numbers:** completion times, deaths, squat/punch accuracy if measured, frame-rate dips.
7. **Issues filed:** links to the GitHub issues created from the findings.

Keep it factual: what testers did and said, separate from what we think it means.
