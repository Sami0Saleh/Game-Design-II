using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Transform player;
    public float minYPosition = -10f; // Minimum Y position to trigger respawn
    private Vector3 respawnPosition;
    public List<GameObject> checkpoints = new List<GameObject>(); // List of all checkpoints
    public GameObject currentCheckpoint; // The last checkpoint the player collided with

    private void Start()
    {
        //currentCheckpoint = GetComponent<GameObject>();
    }

    private void Update()
    {
        // Check if player falls below a certain Y position
        if (player.position.y < minYPosition && currentCheckpoint != null)
        {
            RespawnPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            UpdateRespawnPosition(other.gameObject);
        }
    }

    private void UpdateRespawnPosition(GameObject checkpoint)
    {
        // Update the respawn position to the checkpoint's position
        respawnPosition = checkpoint.transform.position;
        currentCheckpoint = checkpoint;
    }

    private void RespawnPlayer()
    {
        // Respawn the player at the last checkpoint's position
        player.position = new Vector3(respawnPosition.x, respawnPosition.y, player.position.z);
        // You may want to add additional logic for resetting player state here
    }
}
