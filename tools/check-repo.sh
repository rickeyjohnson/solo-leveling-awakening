#!/usr/bin/env bash
# Repo hygiene checks. CI runs this on every pull request; run it yourself before pushing.
#   1. No forbidden files (Unity Library/Temp/builds, raw source art, secrets, study data).
#   2. Every asset under SoloLevelingAwakening/Assets has its .meta, and no .meta is orphaned.
#   3. No large files outside Git LFS, and no LFS-type file committed without LFS.
#   4. No leftover merge-conflict markers.
#
# Checks tracked and staged files (git ls-files), so `git add` first.
# Run from anywhere:  bash tools/check-repo.sh
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if ! REPO_ROOT="$(git -C "$SCRIPT_DIR" rev-parse --show-toplevel 2>/dev/null)"; then
  echo "This script must stay inside the solo-leveling-awakening repo (tools/check-repo.sh)."
  exit 1
fi
cd "$REPO_ROOT"

MAX_NON_LFS_BYTES=$((5 * 1024 * 1024))
ASSETS_DIR="SoloLevelingAwakening/Assets"
FAILURES=0

fail() {
  echo "  ✗ $1"
  FAILURES=$((FAILURES + 1))
}

# --- 1. Forbidden files ----------------------------------------------------
echo "Checking for forbidden files..."
FORBIDDEN_REGEX='^(SoloLevelingAwakening/)?([Ll]ibrary|[Tt]emp|[Oo]bj|[Bb]uilds?|[Ll]ogs|[Uu]ser[Ss]ettings)/|\.(blend|blend1|psd|spp|apk|aab|keystore|jks)$|(^|/)\.env(\..*)?$|\.participant\.csv$'
while IFS= read -r path; do
  fail "$path (never commit this; see AGENTS.md rule 9)"
done < <(git -c core.quotePath=false ls-files | grep -E "$FORBIDDEN_REGEX" || true)

# --- 2. Unity .meta pairs --------------------------------------------------
# Unity skips names starting with "." or ending with "~", so those have no .meta.
if [[ -d "$ASSETS_DIR" ]]; then
  echo "Checking .meta files..."
  TRACKED="$(git -c core.quotePath=false ls-files -- "$ASSETS_DIR" | grep -vE '(^|/)\.|~(/|$)' || true)"

  # Every file below Assets/, and every folder between it and Assets/, needs a .meta.
  EXPECTED="$(
    echo "$TRACKED" | grep -v '\.meta$' | awk -v root="$ASSETS_DIR" '
      NF { print $0 ".meta"
           n = split($0, part, "/"); dir = part[1]
           for (i = 2; i < n; i++) { dir = dir "/" part[i]; if (length(dir) > length(root)) print dir ".meta" } }' |
      sort -u
  )"
  METAS="$(echo "$TRACKED" | grep '\.meta$' | sort -u || true)"

  while IFS= read -r m; do
    fail "${m%.meta} has no .meta (commit the .meta Unity created next to it)"
  done < <(comm -23 <(echo "$EXPECTED") <(echo "$METAS") | grep . || true)

  while IFS= read -r m; do
    fail "$m has no asset (delete the .meta, or commit the missing asset or folder contents)"
  done < <(comm -13 <(echo "$EXPECTED") <(echo "$METAS") | grep . || true)
else
  echo "Skipping .meta checks ($ASSETS_DIR doesn't exist yet)."
fi

# --- 3. Large files and Git LFS --------------------------------------------
# Uses the staged blobs, so it checks exactly what will be committed.
# An LFS pointer is ~130 bytes; an LFS-type blob much bigger than that is the real file.
echo "Checking file sizes and Git LFS..."
INDEX="$(git -c core.quotePath=false ls-files -s | grep -v '^160000' || true)"
if [[ -n "$INDEX" ]]; then
  while IFS=$'\t' read -r size filter path; do
    if [[ "$filter" == "lfs" ]]; then
      if (( size > 1024 )); then
        fail "$path is an LFS type but was committed without LFS (run tools/setup-git.sh, then: git rm --cached '$path' && git add '$path')"
      fi
    elif (( size > MAX_NON_LFS_BYTES )); then
      fail "$path is over $((MAX_NON_LFS_BYTES / 1024 / 1024)) MB and not in Git LFS (add its extension to .gitattributes)"
    fi
  done < <(
    paste \
      <(echo "$INDEX" | cut -d' ' -f2 | git cat-file --batch-check='%(objectsize)') \
      <(echo "$INDEX" | cut -f2 | git check-attr --stdin filter | sed 's/.*: filter: //') \
      <(echo "$INDEX" | cut -f2)
  )
fi

# --- 4. Merge-conflict markers ---------------------------------------------
echo "Checking for merge-conflict markers..."
while IFS= read -r hit; do
  fail "conflict marker at $hit"
done < <(git grep -n -I -E '^(<{7}|>{7})( |$)' -- . ':!tools/check-repo.sh' | cut -d: -f1,2 || true)

# --- Result ----------------------------------------------------------------
if (( FAILURES > 0 )); then
  echo "$FAILURES problem(s) found."
  exit 1
fi
echo "All repo checks passed."
