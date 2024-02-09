using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public Transform ropeAttachPoint;
    public float swingForce = 10f;
    public float climbSpeed = 5f;
    public LayerMask ropeLayer;

    [SerializeField] CharacterController characterController;
    [SerializeField] PlayerController playerController;
    private bool isSwinging = false;
    private Vector3 swingDirection;

    void Update()
    {
        if (isSwinging)
        {
            // Apply swinging force to the character
            characterController.Move(swingDirection * Time.deltaTime);

            // Check for climbing input
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 climbDirection = transform.up * verticalInput * climbSpeed * Time.deltaTime;
            characterController.Move(climbDirection);
            if (Input.GetButtonDown("Jump"))
            {
                isSwinging = true;
                playerController.isSwinging = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with a rope
        if (other.gameObject.layer == ropeLayer)
        {
            StartSwinging(other.transform);
        }
    }

    void StartSwinging(Transform ropeTransform)
    {
        isSwinging = true;
        playerController.isSwinging = true;

        // Calculate the direction from the attach point to the player
        Vector3 ropeDirection = ropeTransform.position - ropeAttachPoint.position;
        swingDirection = Vector3.Cross(ropeDirection, Vector3.up).normalized * swingForce;
    }
}
