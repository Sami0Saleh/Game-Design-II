using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    private Transform ropeAttachPoint;
    private bool _isRopeLocated = false;
    private Collider _ropeCollider;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 swingDirection;
    private Vector3 hangPosition;
    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed;
    private bool _isJumpingFromRope;
    private Vector3 oldMoveDirection;

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

    public bool isSprinting = false;
    public bool isWallRunning = false;
    public bool isHanging = false;
    public bool isOnRope = false;


    void Update()
    {
        moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (!characterController.isGrounded && !isWallRunning && !isHanging && !isOnRope)
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
        else if (isOnRope)
        {
            characterController.Move(Vector3.zero); // reset move
                                                    // characterController.Move(swingDirection * Time.deltaTime);
            moveDirection = Vector3.zero;
            verticalInput = Input.GetAxis("Vertical");
            Vector3 climbDirection = transform.up * verticalInput * climbSpeed * Time.deltaTime;
            climbDirection.x = 0;
            climbDirection.z = 0;
            characterController.Move(climbDirection);
            if (Input.GetButtonDown("Jump"))
            {
                _ropeCollider.enabled = false;
                horizontalInput = Input.GetAxis("Horizontal");

                moveDirection = transform.forward * horizontalInput * moveSpeed * Time.deltaTime;
                characterController.Move(moveDirection);
                isOnRope = false;
                _isJumpingFromRope = true;
                moveDirection = oldMoveDirection * 3;

            }
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
            oldMoveDirection = moveDirection;
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
                moveDirection.y -= 5f * Time.deltaTime;
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
        Vector3 currentPos = transform.position;
        Debug.Log("Detected a edge");
        // Perform raycast to detect edges below the character
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, edgeDetectionDistance, edgeLayer);
        Debug.Log("Entered RayCast");
        // Position character at the edge with slight offset
        Vector3 hangPosition = hit.point + transform.up * edgeHangOffset;
        characterController.Move(/*hangPosition*/ currentPos - transform.position);

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

            Debug.Log("let Go from Edge");
            JumpFromEdge();
        }
        /*if (*//*Physics.Raycast(transform.position, -transform.up, out hit, edgeDetectionDistance, edgeLayer)*//* true)
        {
        }
        else
        {
            // Stop hanging if no edge is detected
            isHanging = false;
        }*/
    }

    void JumpFromEdge()
    {
        // Apply jump force away from the edge
        moveDirection = transform.up * jumpHeight;
        moveDirection.y = jumpHeight;
        moveDirection.z = 10;
        isHanging = false;
    }
    /*void StartSwinging(Transform ropeTransform)
    {
        Vector3 ropeDirection = ropeTransform.position - ropeAttachPoint.position;
        swingDirection = Vector3.Cross(ropeDirection, Vector3.up).normalized * swingForce;
        if (Input.GetButtonDown("Jump"))
        {
            isHanging = false;
            _isRopeLocated = false;
        }
    }*/
    void HangOnRope(Transform ropeTransform) // Where is it being called from?
    {
        Debug.Log("hanging on!");
        isOnRope = true;
        if (!_isRopeLocated)
        {
            ropeAttachPoint = ropeTransform.transform; // Assign the transform when hitting the rope

            // Position the player slightly below the rope attach point
            hangPosition = ropeTransform.position + ropeAttachPoint.localPosition;
            hangPosition.y = transform.position.y; // sets the player on the height he connected to the rope
            characterController.Move(hangPosition - transform.position);
            _isRopeLocated = true;
        }

        // Check for climbing input
        /* float verticalInput = Input.GetAxis("Vertical");
         Vector3 climbDirection = transform.up * verticalInput * climbSpeed * Time.deltaTime;
         characterController.Move(climbDirection);*/
        // Check if the player wants to release the rope

        //  StartSwinging(ropeTransform);
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
            _ropeCollider = hit.collider;
        }
    }
}