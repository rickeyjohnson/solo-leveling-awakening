using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace SLA.Player
{
    /// <summary>
    /// A teleport target that only accepts points on the baked NavMesh (walkable floor). Aiming anywhere
    /// else can't be selected, so the arc shows its blocked (grey) style and releasing doesn't teleport.
    /// The player lands on the NavMesh point, never inside props or walls. Put it on the floor colliders'
    /// root (Environment layer, interaction layer Teleport) in any scene with a NavMeshSurface (CMF-7).
    /// </summary>
    [AddComponentMenu("SLA/NavMesh Teleportation Area")]
    public class NavMeshTeleportationArea : TeleportationArea
    {
        [Tooltip("How far the aimed point may be from the baked NavMesh and still count as walkable floor (m).")]
        [SerializeField] private float _maxDistanceFromNavMeshMeters = 0.25f;

        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            if (!base.IsSelectableBy(interactor))
            {
                return false;
            }

            // Only ray interactors aim at a point; anything else is checked in GenerateTeleportRequest.
            if (!(interactor is XRRayInteractor ray) || ray == null)
            {
                return true;
            }

            return ray.TryGetCurrent3DRaycastHit(out RaycastHit hit) && TryFindNavMeshPoint(hit.point, out _);
        }

        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (!base.GenerateTeleportRequest(interactor, raycastHit, ref teleportRequest)
                || !TryFindNavMeshPoint(teleportRequest.destinationPosition, out Vector3 onNavMesh))
            {
                return false;
            }

            // x/z from the NavMesh, height from the floor that was hit: the baked NavMesh sits a few centimetres
            // above the floor, and with gravity off the player would hover there.
            onNavMesh.y = teleportRequest.destinationPosition.y;
            teleportRequest.destinationPosition = onNavMesh;
            return true;
        }

        private bool TryFindNavMeshPoint(Vector3 point, out Vector3 onNavMesh)
        {
            if (NavMesh.SamplePosition(point, out NavMeshHit hit, _maxDistanceFromNavMeshMeters, NavMesh.AllAreas))
            {
                onNavMesh = hit.position;
                return true;
            }

            onNavMesh = point;
            return false;
        }
    }
}
