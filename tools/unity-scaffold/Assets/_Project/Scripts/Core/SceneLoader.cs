using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SLA.Core
{
    /// <summary>
    /// Loads one content scene (hub, gate, penalty zone...) additively on top of Bootstrap,
    /// unloading the previous one, with a fade to black around the swap to hide loading hitches.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private float _fadeSeconds = 0.25f;

        private string _currentContentScene;

        /// <summary>Target fade alpha (1 = black, 0 = clear). A screen fader in Bootstrap listens.</summary>
        public event Action<float, float> FadeRequested;

        public bool IsLoading { get; private set; }

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

        public void LoadContentScene(string sceneName, Action onLoaded = null)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] Already loading; ignored request for {sceneName}.");
                return;
            }

            StartCoroutine(SwapRoutine(sceneName, onLoaded));
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

            FadeRequested?.Invoke(0f, _fadeSeconds);
            IsLoading = false;
            onLoaded?.Invoke();
        }
    }
}
