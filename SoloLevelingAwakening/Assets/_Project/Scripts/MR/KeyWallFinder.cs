using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SLA.MR
{
    /// <summary>
    /// Finds the room's "key wall": the largest vertical plane from the headset's room scan
    /// (Space Setup), read through AR Foundation. The gate portal opens there.
    /// This is the week-3 spike target; if AR Foundation proves unreliable, swap in Meta's MRUK.
    /// </summary>
    public class KeyWallFinder : MonoBehaviour
    {
        [SerializeField] private ARPlaneManager _planeManager;
        [SerializeField] private float _minWallWidth = 1.2f;
        [SerializeField] private float _minWallHeight = 1.8f;

        public ARPlaneManager PlaneManager
        {
            get => _planeManager;
            set => _planeManager = value;
        }

        /// <summary>
        /// Pose at the wall's center whose forward points INTO the wall, so a quad placed with it
        /// faces the room (Unity's quad is visible from its -Z side).
        /// </summary>
        public bool TryFindKeyWall(out Pose pose, out Vector2 size)
        {
            pose = default;
            size = default;
            if (_planeManager == null)
            {
                return false;
            }

            ARPlane best = null;
            float bestArea = 0f;
            foreach (var plane in _planeManager.trackables)
            {
                if (plane.alignment != PlaneAlignment.Vertical)
                {
                    continue;
                }

                Vector2 s = plane.size;
                if (s.x < _minWallWidth || s.y < _minWallHeight)
                {
                    continue;
                }

                float area = s.x * s.y;
                if (area > bestArea)
                {
                    best = plane;
                    bestArea = area;
                }
            }

            if (best == null)
            {
                return false;
            }

            pose = new Pose(best.center, Quaternion.LookRotation(-best.normal, Vector3.up));
            size = best.size;
            return true;
        }
    }
}
