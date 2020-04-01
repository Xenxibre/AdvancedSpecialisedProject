using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    //Input. 
    private Player_Input_Handler inputHandler;
    private CharacterController characterController;

    //Rotation. 
    private GameObject rotationHelper;

    //Jumping.
    [SerializeField] private float jumpForce;

    private float gravity = -16.0f; 
    private float groundDistance = 0.4f;

    private Transform groundCheck;

    public LayerMask groundMask;

    private bool isGrounded; 

    //Movement variables.
    private float moveSpeed;
    private Vector3 velocity;

    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 4f;

    private float yMouseInput;
    private float xMouseInput; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<Player_Input_Handler>();
    }

    void Update()
    {
        HandleCharacterMovement(); 
    }

    //-------------------------------------------------------
    //Movement Handling.
    //-------------------------------------------------------
    void HandleCharacterMovement()
    {   
        xMouseInput += inputHandler.GetHorizontalLookInput();
        transform.localRotation = Quaternion.Euler(0f, xMouseInput, 0f); //Rotate character with horizontal mouse movement. 

        if (rotationHelper != null) //Rotate camera with vertical mouse movement. 
        {
            yMouseInput += -inputHandler.GetVerticalLookInput();
            yMouseInput = Mathf.Clamp(yMouseInput, -90f, 90f);
            rotationHelper.transform.localRotation = Quaternion.Euler(yMouseInput, 0f, 0f);
        }
        
        Vector3 worldSpaceMovement = transform.TransformVector(inputHandler.GetMoveInput()); //Create world space vector.
      
        if (characterController != null)  //Move player.
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            Debug.Log(isGrounded); 

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; 
            }

            characterController.Move(worldSpaceMovement * moveSpeed * Time.deltaTime); //Move player according to Input. 

            JumpInput(); //Test for jump input. 

            velocity.y += gravity * Time.deltaTime; //Apply gravity. 

            characterController.Move(velocity * Time.deltaTime);

            transform.position = characterController.transform.position; 
        }

        SetMovementSpeed();
    }

    void SetMovementSpeed() //Check if sprinting and adjust movement speed.
    {
        if (inputHandler.GetSprintInput())
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime * acceleration);
        }
    }

    void JumpInput() //Check if jumping. 
    {
        if (inputHandler.GetJumpInputDown() && isGrounded)
        {
            velocity.y = jumpForce; 
        }
    }

    //-----------------------------------------------------
    //Public Functions.
    //-----------------------------------------------------
    public void SetupForMovement()
    {
        rotationHelper = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper").gameObject;
        groundCheck = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/GroundCheck").gameObject.transform;
    }
}
