using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Transform[] checkpoints;
    public BoxCollider gameOverCollider; // Box Collider that triggers game over
    private Transform currentCheckpoint;

    private GameObject player; // Reference to the player GameObject

    void Start()
    {
        currentCheckpoint = checkpoints[0]; // Assign the first checkpoint as the initial checkpoint
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player GameObject
    }

    // Function to respawn the player at the current checkpoint
    public void RespawnPlayer()
    {
        if (player != null && currentCheckpoint != null)
        {
            player.transform.position = currentCheckpoint.position;
        }
        else
        {
            Debug.LogError("Player or CurrentCheckpoint is null.");
        }
    }

    // Function to update the current checkpoint
    public void UpdateCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    // Check if the player collides with a checkpoint or game over collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            UpdateCheckpoint(other.transform);
        }
        else if (other == gameOverCollider)
        {
            StartCoroutine(RespawnAfterDelay(3f)); // Respawn after 3 seconds
        }
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }
}
