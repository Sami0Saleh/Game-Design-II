using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector3 respawnPosition;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Checkpoint"))
        {
            UpdateRespawnPosition(hit.transform);
        }
        else if (hit.gameObject.CompareTag("DeathGround"))
        {

            RespawnPlayer();
        }
    }

    private void UpdateRespawnPosition(Transform checkpoint)
    {
        Debug.Log("CP Updated");
        // Update the respawn position to the checkpoint's position
        respawnPosition = checkpoint.transform.position;
    }

    private void RespawnPlayer()
    {
        // Respawn the player at the last checkpoint's position
        transform.position = new Vector3(respawnPosition.x, respawnPosition.y, respawnPosition.z);
    }
}
