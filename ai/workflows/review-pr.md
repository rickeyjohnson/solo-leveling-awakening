# Workflow: Review a pull request

Input: a PR number or branch.

1. Get the diff (`gh pr diff <n>` or `git diff main...<branch>`) and the PR description; find the issue and rule IDs it claims.
2. Run the reviews that apply:
   - C# changed → `ai/reviews/unity-code-review.md`
   - camera, movement, combat feedback, VFX, scenes, UI or performance-sensitive code → `ai/reviews/vr-comfort-performance.md`
   - specs, GDD, ScriptableObject defaults or names changed → `ai/reviews/gdd-consistency.md`
3. Check the PR template boxes are honest (headset test done? frame rate noted? CREDITS updated?).
4. Merge the findings into one list ordered by severity, without duplicates.
5. Post or return the review. Finish with the exact headset checks the human reviewer should do.

An AI review never replaces the human headset test before merge.
