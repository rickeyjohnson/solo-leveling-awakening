# Workflow: Weekly build (Fridays)

Run by the producer (or an assistant helping them). Produces a short note in the team chat and issues for anything broken.

1. `git switch main && git pull`. Note the commit hash.
2. `bash tools/check-repo.sh` and the EditMode tests (command in `AGENTS.md`). Stop and file `S1` issues if anything fails.
3. Build to a Quest 3 (File → Build Profiles → Android → Build and Run). Note build time and APK size.
4. Smoke test (15 minutes, everyone):
   - [ ] Boots to the title or setup screen within a few seconds, no errors in MQDH logs
   - [ ] Hub loads; portal appears on a wall (or the virtual apartment loads)
   - [ ] Daily quest counts squats
   - [ ] Enter the newest gate, fight, die once, respawn at the entrance
   - [ ] Frame rate (OVR Metrics) in the hub and the worst gate room
   - [ ] Every comfort preset still works
5. Check Git LFS usage (GitHub → Settings → Billing → Git LFS) against the 10 GiB limits.
6. Post the note: commit, pass/fail per check, frame rates, LFS usage, new issues.

Template:

```text
Weekly build <date> @ <commit>
Tests: ✅/❌  Repo checks: ✅/❌  Build: ✅/❌ (<size> MB)
Hub <fps> fps · <gate>/<room> <fps> fps
Smoke: boot ✅ hub ✅ daily ✅ gate ✅ respawn ✅ presets ✅
LFS: <x> GiB storage / <y> GiB bandwidth this month
New issues: #..
```
