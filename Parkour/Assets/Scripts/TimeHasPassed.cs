using UnityEngine;

public class TimeHasPassed : MonoBehaviour
{
    public BoxCollider triggerCollider;
    public GameObject objectToRotate; // GameObject to rotate
    public GameObject objectToDisable; // GameObject to disable
    public Vector3 targetRotationAngles; // New RotatePosition
    public float rotationDuration = 1.0f; // Rotation duration in seconds

    private bool hasRotated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerCollider != null && !hasRotated)
        {
            if (other.bounds.Intersects(triggerCollider.bounds))
            {
                StartCoroutine(RotateObject());
                DisableObject();
            }
        }
    }

    private System.Collections.IEnumerator RotateObject()
    {
        if (objectToRotate != null)
        {
            Quaternion startRotation = objectToRotate.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(targetRotationAngles);
            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                objectToRotate.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / rotationDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the object rotates to the exact target rotation
            objectToRotate.transform.rotation = targetRotation;

            hasRotated = true;
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to rotate!");
        }
    }

    private void DisableObject()
    {
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to disable!");
        }
    }
}
