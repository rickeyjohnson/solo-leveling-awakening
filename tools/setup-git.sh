#!/usr/bin/env bash
# One-time setup for each clone of this repo.
#   1. Enables Git LFS for this repo.
#   2. Registers Unity's Smart Merge (UnityYAMLMerge) as the merge driver for scenes and prefabs.
#
# Run from anywhere:  bash tools/setup-git.sh   (or the full path to this file)
# On Windows, run it from Git Bash.
set -euo pipefail

# Find the repo from this script's location, not the current folder.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if ! REPO_ROOT="$(git -C "$SCRIPT_DIR" rev-parse --show-toplevel 2>/dev/null)"; then
  echo "This script must stay inside the solo-leveling-awakening repo (tools/setup-git.sh)."
  exit 1
fi
cd "$REPO_ROOT"

# --- 1. Git LFS ------------------------------------------------------------
if ! git lfs version >/dev/null 2>&1; then
  echo "Git LFS is not installed."
  echo "  macOS:   brew install git-lfs"
  echo "  Windows: it ships with Git for Windows; reinstall Git and tick 'Git LFS'"
  exit 1
fi
git lfs install --local
echo "Git LFS enabled."

# --- 2. Unity Smart Merge --------------------------------------------------
# The Unity version the project uses (read from ProjectVersion.txt once the project exists).
VERSION_FILE="SoloLevelingAwakening/ProjectSettings/ProjectVersion.txt"
if [[ -f "$VERSION_FILE" ]]; then
  UNITY_VERSION="$(sed -n 's/^m_EditorVersion: //p' "$VERSION_FILE" | tr -d '\r')"
else
  UNITY_VERSION="${UNITY_VERSION:-}"
fi

MERGE_TOOL=""
case "$(uname -s)" in
  Darwin)
    if [[ -n "$UNITY_VERSION" ]]; then
      MERGE_TOOL="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/Tools/UnityYAMLMerge"
    else
      MERGE_TOOL="$(ls -d /Applications/Unity/Hub/Editor/*/Unity.app/Contents/Tools/UnityYAMLMerge 2>/dev/null | tail -1 || true)"
    fi
    ;;
  MINGW*|MSYS*|CYGWIN*)
    BASE="/c/Program Files/Unity/Hub/Editor"
    if [[ -n "$UNITY_VERSION" ]]; then
      MERGE_TOOL="$BASE/$UNITY_VERSION/Editor/Data/Tools/UnityYAMLMerge.exe"
    else
      MERGE_TOOL="$(ls -d "$BASE"/*/Editor/Data/Tools/UnityYAMLMerge.exe 2>/dev/null | tail -1 || true)"
    fi
    ;;
esac

if [[ -z "$MERGE_TOOL" || ! -e "$MERGE_TOOL" ]]; then
  echo "Could not find UnityYAMLMerge. Install the project's Unity version with Unity Hub, then rerun."
  echo "(Or set it by hand: UNITY_VERSION=6000.3.x bash tools/setup-git.sh)"
  exit 1
fi

git config --local merge.unityyamlmerge.name "Unity SmartMerge (UnityYAMLMerge)"
git config --local merge.unityyamlmerge.driver "\"$MERGE_TOOL\" merge -p %O %B %A %A"
git config --local merge.unityyamlmerge.recursive binary
echo "Unity Smart Merge registered: $MERGE_TOOL"
echo "Done."
