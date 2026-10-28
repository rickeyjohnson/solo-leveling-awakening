using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// Fades the headset view to black and back when SceneLoader asks (FadeRequested). The fade is a
    /// small sphere around the camera, drawn on top of everything, with its renderer off while clear so it
    /// costs nothing. Fading never moves the camera (CMF-1). Lives in Bootstrap.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private Renderer _fadeRenderer;
        [SerializeField] private SceneLoader _sceneLoader;
        [Tooltip("Start black, so the first scene fades in instead of popping in.")]
        [SerializeField] private bool _startBlack = true;

        private Material _material;
        private Color _color;
        private float _alpha;
        private float _target;
        private float _durationSeconds;

        /// <summary>Current fade: 0 clear, 1 black.</summary>
        public float Alpha => _alpha;

        private void Awake()
        {
            if (_fadeRenderer != null)
            {
                _material = _fadeRenderer.material; // a per-fader copy, so the shared asset never changes
                _color = _material.GetColor(ColorId);
            }

            // Without a loader nothing would ever fade back in, so never start black then.
            bool startBlack = _startBlack && _sceneLoader != null;
            if (_sceneLoader == null)
            {
                Debug.LogError("[ScreenFader] No SceneLoader assigned; fades are off.");
            }

            _alpha = _target = startBlack ? 1f : 0f;
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

        private void Update()
        {
            if (_alpha == _target)
            {
                return;
            }

            // Unscaled, so fades still run while the game is paused.
            _alpha = FadeMath.Step(_alpha, _target, _durationSeconds, Time.unscaledDeltaTime);
            Apply();
        }

        private void OnFadeRequested(float targetAlpha, float durationSeconds)
        {
            _target = Mathf.Clamp01(targetAlpha);
            _durationSeconds = durationSeconds;
        }

        private void Apply()
        {
            if (_fadeRenderer == null)
            {
                return;
            }

            _fadeRenderer.enabled = _alpha > 0f;
            if (_material != null)
            {
                _color.a = _alpha;
                _material.SetColor(ColorId, _color);
            }
        }
    }
}
