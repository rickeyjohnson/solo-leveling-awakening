using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SLA.Core
{
    /// <summary>
    /// Loads one content scene (title, hub, gate, penalty zone...) additively on top of Bootstrap,
    /// unloading the previous one, with a fade to black around the swap to hide loading hitches.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>The loader in Bootstrap; null when Bootstrap isn't loaded.</summary>
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private float _fadeSeconds = 0.25f;

        private string _currentContentScene;

        /// <summary>Asks for a fade to a target alpha (1 = black, 0 = clear) over some seconds. ScreenFader listens.</summary>
        public event Action<float, float> FadeRequested;

        /// <summary>True from the start of a fade-out until the new scene is fading in.</summary>
        public bool IsLoading { get; private set; }

        /// <summary>The content scene loaded now, or null before the first load.</summary>
        public string CurrentContentScene => _currentContentScene;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>True if <paramref name="sceneName"/> can be loaded now: no load running, and it's in the build.</summary>
        public bool CanLoad(string sceneName)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] Already loading; ignored request for {sceneName}.");
                return false;
            }

            // Checked before fading out: failing mid-swap would leave the player on a black screen.
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[SceneLoader] {sceneName} is not in the build profile's scene list.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Fades out, swaps in <paramref name="sceneName"/>, then fades in. Returns false if it can't load now
        /// (see <see cref="CanLoad"/>). <paramref name="onLoaded"/> runs once the new scene is active.
        /// Use GameFlow.GoTo when the game state changes too.
        /// </summary>
        public bool LoadContentScene(string sceneName, Action onLoaded = null)
        {
            if (!CanLoad(sceneName))
            {
                return false;
            }

            StartCoroutine(SwapRoutine(sceneName, onLoaded));
            return true;
        }

        private IEnumerator SwapRoutine(string sceneName, Action onLoaded)
        {
            IsLoading = true;
            FadeRequested?.Invoke(1f, _fadeSeconds);
            yield return new WaitForSecondsRealtime(_fadeSeconds);

            if (!string.IsNullOrEmpty(_currentContentScene))
            {
                yield return SceneManager.UnloadSceneAsync(_currentContentScene);
            }

            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            _currentContentScene = sceneName;

            // One frame so the new scene's Start runs (PlayerSpawn moves the rig) while the view is
            // still black: the player never sees the move (MRH-8, CMF-1).
            yield return null;

            FadeRequested?.Invoke(0f, _fadeSeconds);
            IsLoading = false;
            onLoaded?.Invoke();
        }
    }
}
