using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{

    Animator animator;
    CharacterController characterController;

    
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float jumpButtonGracePeriod;
    [SerializeField] private float jumpHorizontalSpeed;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    // Internal State Variables
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }
    void Update()
    {
        handleMovement();
       
        
    }
    void handleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputMagnitude /= 2;
        }

        animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
         
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        HandleJump();

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        if (isGrounded == false)
        {
            Vector3 velocity = movementDirection * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);
        }
    }
    void HandleJump()
    {
        float gravity = Physics.gravity.y * gravityMultiplier;

        if (isJumping && ySpeed > 0 && Input.GetButton("Jump") == false)
        {
            gravity *= 2;
        }
        ySpeed += gravity * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
                animator.SetBool("IsJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("IsFalling", true);
            }
        }
    }
    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y = ySpeed * Time.deltaTime;
            characterController.Move(velocity);
        }
    }
    public void LoadData(GameData data)
    {
        //Debug.Log("Loading player position: " + data.playerPosition.ToString());
        this.transform.position = data.playerPosition;
    }
    public void SaveData(GameData data)
    {
        // Debug.Log("Saving player position: " + this.transform.position.ToString());
        data.playerPosition = this.transform.position;
    }
    /*
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    */
}