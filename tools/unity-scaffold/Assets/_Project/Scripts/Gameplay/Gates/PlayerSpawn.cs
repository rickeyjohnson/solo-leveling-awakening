using UnityEngine;

namespace SLA.Gates
{
    /// <summary>
    /// Where the player stands when a content scene loads. The XR Origin lives in Bootstrap,
    /// so each gate moves it here (position and facing) instead of spawning a new rig.
    /// </summary>
    public class PlayerSpawn : MonoBehaviour
    {
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                return;
            }

            var cc = player.GetComponent<CharacterController>();
            bool hadController = cc != null && cc.enabled;
            if (hadController) cc.enabled = false; // CharacterController fights teleports otherwise

            player.transform.SetPositionAndRotation(
                transform.position,
                Quaternion.Euler(0f, transform.eulerAngles.y, 0f));

            if (hadController) cc.enabled = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.9f, 0.3f);
            Gizmos.DrawLine(transform.position + Vector3.up * 0.9f, transform.position + Vector3.up * 0.9f + transform.forward * 0.6f);
        }
    }
}
