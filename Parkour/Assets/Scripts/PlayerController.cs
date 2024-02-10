using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform ropeAttachPoint;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 swingDirection;
    private Vector3 hangPosition;


    public float swingForce = 10f;
    public float climbSpeed = 5f;
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float jumpHeight = 8.0f;
    public float gravity = 20.0f;
    public float hangMovementSpeed = 3f;
    public float wallRunSpeed = 3f;
    public float maxWallAngle = 91f;
    public float wallRunDistance = 20f;
    public float edgeDetectionDistance = 20f;
    public float edgeHangOffset = 0.5f;
    public LayerMask wallLayer;
    public LayerMask edgeLayer;
    public LayerMask ropeLayer;

    private bool isSprinting = false;
    public bool isWallRunning = false;
    public bool isHanging = false;
    public bool isSwinging = false;

    void Update()
    {
        float moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (!characterController.isGrounded && !isWallRunning && !isHanging && !isSwinging)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else if (isWallRunning)
        {
            WallRun();
        }
        else if (isHanging)
        {
            HangFromEdge();
        }
        else if (isSwinging)
        {
            characterController.Move(swingDirection * Time.deltaTime);

            verticalInput = Input.GetAxis("Vertical");
            Vector3 climbDirection = transform.up * verticalInput * climbSpeed * Time.deltaTime;
            characterController.Move(climbDirection);
        }
        else if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else
        {
            moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
            
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
        characterController.Move(moveDirection * Time.deltaTime);

    }

    void WallRun()
    {
        // Perform raycast to detect nearby walls
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallRunDistance, wallLayer))
        {
            // Check if the wall is within the acceptable angle range for wall running
            if (Vector3.Angle(hit.normal, Vector3.up) < maxWallAngle)
            {
                // Calculate move direction parallel to the wall
                Vector3 wallRunDirection = Vector3.Cross(hit.normal, Vector3.up);
                moveDirection = wallRunDirection * wallRunSpeed;

                // Apply gravity to stick to the wall
                moveDirection.y -= 60f * Time.deltaTime;
                characterController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                // Stop wall running if the angle is too steep
                isWallRunning = false;
            }
        }
        else
        {
            // Stop wall running if no wall is detected
            isWallRunning = false;
        }
    }

    void HangFromEdge()
    {
        // Perform raycast to detect edges below the character
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, edgeDetectionDistance, edgeLayer))
        {
            // Position character at the edge with slight offset
            Vector3 hangPosition = hit.point + transform.up * edgeHangOffset;
            characterController.Move(hangPosition - transform.position);

            // Disable movement along y-axis
            moveDirection.y = 0;

            // Check for lateral movement input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 lateralMovement = new Vector3(horizontalInput, 0, verticalInput).normalized * hangMovementSpeed;
            characterController.Move(lateralMovement * Time.deltaTime);

            // Check for jump input to vault from edge
            if (Input.GetButtonDown("Jump"))
            {
                JumpFromEdge();
            }
        }
        else
        {
            // Stop hanging if no edge is detected
            isHanging = false;
        }
    }

    void JumpFromEdge()
    {
        // Apply jump force away from the edge
        moveDirection = transform.up * jumpHeight;
        moveDirection.y = jumpHeight;
        isHanging = false;
    }
    void StartSwinging(Transform ropeTransform)
    {
        Vector3 ropeDirection = ropeTransform.position - ropeAttachPoint.position;
        swingDirection = Vector3.Cross(ropeDirection, Vector3.up).normalized * swingForce;
        if (Input.GetButtonDown("Jump"))
        {
            isHanging = false;
        }
    }
    void HangOnRope(Transform ropeTransform)
    {
        isSwinging = true;
        // Position the player slightly below the rope attach point
        hangPosition = ropeTransform.position + ropeAttachPoint.localPosition;
        characterController.Move(hangPosition - transform.position);

        // Check for climbing input
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 climbDirection = transform.up * verticalInput * climbSpeed * Time.deltaTime;
        characterController.Move(climbDirection);

        // Check if the player wants to release the rope
        
        StartSwinging(ropeTransform);
    }
    void Jump()
    {
        moveDirection.y = jumpHeight;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("sidewall") && Input.GetKey(KeyCode.LeftShift))
        {
            isWallRunning = true;

        }
        else if (hit.gameObject.CompareTag("edge"))
        {
            isHanging = true;
        }
        else if (hit.gameObject.CompareTag("rope"))
        {
            HangOnRope(hit.transform);
        }
    }
    

}
