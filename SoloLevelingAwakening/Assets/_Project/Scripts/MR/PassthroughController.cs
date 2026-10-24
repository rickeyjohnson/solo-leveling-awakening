using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace SLA.MR
{
    /// <summary>
    /// Switches between the real room (passthrough) and a fully virtual world. Passthrough needs a
    /// transparent camera background plus an enabled ARCameraManager (Unity OpenXR: Meta).
    /// </summary>
    public class PassthroughController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private ARCameraManager _arCameraManager;

        private CameraClearFlags _vrClearFlags = CameraClearFlags.Skybox;
        private Color _vrBackground = Color.black;

        public bool IsOn { get; private set; }

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            if (_arCameraManager == null && _camera != null)
            {
                _arCameraManager = _camera.GetComponent<ARCameraManager>();
            }
            if (_camera != null)
            {
                _vrClearFlags = _camera.clearFlags;
                _vrBackground = _camera.backgroundColor;
            }
        }

        public void SetPassthrough(bool on)
        {
            if (_camera == null)
            {
                return;
            }

            if (on)
            {
                _camera.clearFlags = CameraClearFlags.SolidColor;
                _camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                _camera.clearFlags = _vrClearFlags;
                _camera.backgroundColor = _vrBackground;
            }

            if (_arCameraManager != null)
            {
                _arCameraManager.enabled = on;
            }
            IsOn = on;
        }
    }
}
