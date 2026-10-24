using System.Collections;
using SLA.Core;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace SLA.MR
{
    /// <summary>
    /// Runs the mixed-reality hub: turns passthrough and plane detection on, finds the key wall,
    /// and opens the gate portal there. If no wall is found in time (no room scan, or an expo
    /// hall), it falls back to the virtual apartment.
    /// </summary>
    public class MRHubController : MonoBehaviour
    {
        [SerializeField] private PassthroughController _passthrough;
        [SerializeField] private KeyWallFinder _wallFinder;
        [Tooltip("The gate portal; hidden until it has a wall.")]
        [SerializeField] private Transform _portal;
        [SerializeField] private float _searchSeconds = 6f;
        [Tooltip("Portal center height above the floor, meters.")]
        [SerializeField] private float _portalCenterHeight = 1.05f;
        [Tooltip("Gap between the portal and the wall, meters.")]
        [SerializeField] private float _wallOffset = 0.02f;

        private ARSession _session;
        private ARPlaneManager _planes;

        private void OnEnable()
        {
            _session = FindAnyObjectByType<ARSession>(FindObjectsInactive.Include);
            _planes = FindAnyObjectByType<ARPlaneManager>(FindObjectsInactive.Include);

            if (_wallFinder != null && _wallFinder.PlaneManager == null)
            {
                _wallFinder.PlaneManager = _planes;
            }

            SetMixedReality(true);
            if (_portal != null)
            {
                _portal.gameObject.SetActive(false);
            }

            GameStateManager.Instance?.ChangeState(GameState.Hub);
            StartCoroutine(PlacePortalRoutine());
        }

        private void OnDisable()
        {
            SetMixedReality(false);
        }

        private void SetMixedReality(bool on)
        {
            if (_session != null) _session.enabled = on;
            if (_planes != null) _planes.enabled = on;
            if (_passthrough != null) _passthrough.SetPassthrough(on);
        }

        private IEnumerator PlacePortalRoutine()
        {
            float deadline = Time.time + _searchSeconds;
            while (Time.time < deadline)
            {
                if (_wallFinder != null && _wallFinder.TryFindKeyWall(out Pose wall, out _))
                {
                    PlacePortal(wall);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            Debug.LogWarning("[MRHubController] No key wall found; loading the virtual hub.");
            SceneLoader.Instance?.LoadContentScene(SceneNames.HubVirtual);
        }

        private void PlacePortal(Pose wall)
        {
            if (_portal == null)
            {
                return;
            }

            float floorY = _portal.root == _portal ? 0f : _portal.root.position.y;
            Vector3 position = wall.position - wall.forward * _wallOffset;
            position.y = floorY + _portalCenterHeight;

            _portal.SetPositionAndRotation(position, wall.rotation);
            _portal.gameObject.SetActive(true);
        }
    }
}
