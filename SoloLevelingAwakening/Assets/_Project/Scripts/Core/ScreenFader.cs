using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// Fades the headset view to black and back. The fade is a small sphere around the camera, drawn on
    /// top of everything, with its renderer off while clear so it costs nothing. Two sources share it:
    /// scene loads (SceneLoader.FadeRequested) and short overlay fades such as teleports
    /// (<see cref="FadeOverlay"/>); the darker of the two is shown. Fading never moves the camera (CMF-1).
    /// Lives on the PlayerRig prefab, so it also works in sandboxes, which have no SceneLoader.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private Renderer _fadeRenderer;
        [Tooltip("Bootstrap's SceneLoader. Found in the scene when empty; sandboxes have none, so only overlay fades run there.")]
        [SerializeField] private SceneLoader _sceneLoader;
        [Tooltip("Start black, so the first scene fades in instead of popping in. Ignored without a SceneLoader.")]
        [SerializeField] private bool _startBlack = true;

        private Material _material;
        private Color _color;
        private float _sceneAlpha;
        private float _sceneTarget;
        private float _sceneSeconds;
        private float _overlayAlpha;
        private float _overlayTarget;
        private float _overlaySeconds;

        /// <summary>Current fade: 0 clear, 1 black.</summary>
        public float Alpha => Mathf.Max(_sceneAlpha, _overlayAlpha);

        private void Awake()
        {
            if (_fadeRenderer != null)
            {
                _material = _fadeRenderer.material; // a per-fader copy, so the shared asset never changes
                _color = _material.GetColor(ColorId);
            }

            if (_sceneLoader == null)
            {
                _sceneLoader = FindAnyObjectByType<SceneLoader>();
            }

            // Without a loader nothing would ever fade back in, so never start black then.
            bool startBlack = _startBlack && _sceneLoader != null;
            _sceneAlpha = _sceneTarget = startBlack ? 1f : 0f;
            Apply();
        }

        private void OnEnable()
        {
            if (_sceneLoader != null)
            {
                _sceneLoader.FadeRequested += OnFadeRequested;
            }
        }

        private void OnDisable()
        {
            if (_sceneLoader != null)
            {
                _sceneLoader.FadeRequested -= OnFadeRequested;
            }
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }

        /// <summary>
        /// Fades the overlay layer (teleports) to <paramref name="targetAlpha"/> (1 black, 0 clear) so a full
        /// fade takes <paramref name="durationSeconds"/>. It never clears a scene-load fade.
        /// </summary>
        public void FadeOverlay(float targetAlpha, float durationSeconds)
        {
            _overlayTarget = Mathf.Clamp01(targetAlpha);
            _overlaySeconds = durationSeconds;
        }

        private void Update()
        {
            if (_sceneAlpha == _sceneTarget && _overlayAlpha == _overlayTarget)
            {
                return;
            }

            // Unscaled, so fades still run while the game is paused.
            float delta = Time.unscaledDeltaTime;
            _sceneAlpha = FadeMath.Step(_sceneAlpha, _sceneTarget, _sceneSeconds, delta);
            _overlayAlpha = FadeMath.Step(_overlayAlpha, _overlayTarget, _overlaySeconds, delta);
            Apply();
        }

        private void OnFadeRequested(float targetAlpha, float durationSeconds)
        {
            // A load that starts while a teleport fade is clearing starts from what's on screen, so the view only
            // gets darker instead of pulsing light then dark again.
            if (targetAlpha >= 1f)
            {
                _sceneAlpha = Mathf.Max(_sceneAlpha, _overlayAlpha);
            }

            _sceneTarget = Mathf.Clamp01(targetAlpha);
            _sceneSeconds = durationSeconds;
        }

        private void Apply()
        {
            if (_fadeRenderer == null)
            {
                return;
            }

            float alpha = Alpha;
            _fadeRenderer.enabled = alpha > 0f;
            if (_material != null)
            {
                _color.a = alpha;
                _material.SetColor(ColorId, _color);
            }
        }
    }
}
