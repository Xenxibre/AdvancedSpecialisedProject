using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{ 
    
    enum Guns
    {
        Deagle,
        AK47
    }

    //Input Manager. 
    [SerializeField] private Player_Input_Handler m_inputHandler;

    private GameObject m_rotationHelper;
    private CharacterController m_characterController;

    //Movement variables.
    private float m_moveSpeed;

    [SerializeField] private float m_walkSpeed = 4f;
    [SerializeField] private float m_sprintSpeed = 10f;
    [SerializeField] private float m_acceleration = 4f;

    private bool m_isJumping;

    [SerializeField] private float m_jumpMultiplier;
    [SerializeField] private AnimationCurve m_jumpFalloff;

    private Vector3 characterVelocity; 

    //Gun Variables.
    [SerializeField] List<GameObject> m_gunList;
    [SerializeField] List<GameObject> m_equippedGunList;

    private bool m_primaryChanged;
    private bool m_secondaryChanged;

    private int m_currentlyEquipped;

    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>(); 

        m_primaryChanged = false;
        m_secondaryChanged = false;

        m_currentlyEquipped = 1; //Equip secondary pistol by default.
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterMovement(); 
    }

    void HandleCharacterMovement()
    {
        //Rotate character with horizontal mouse movement. 
        transform.Rotate(new Vector3(0f, (m_inputHandler.GetHorizontalLookInput()), 0f), Space.Self);

        //Rotate camera with vertical mouse movement. 
        if (m_rotationHelper != null)
        {
            m_rotationHelper.transform.Rotate(new Vector3((-m_inputHandler.GetVerticalLookInput()), 0f, 0f), Space.Self);
        }

        //Check if player is sprinting. 
        bool isSprinting = m_inputHandler.GetSprintInput();

        //Create world space vector.
        Vector3 worldSpaceMovement = transform.TransformVector(m_inputHandler.GetMoveInput());

        //Move player.
        if(m_characterController != null)
        {
            m_characterController.SimpleMove(worldSpaceMovement * m_walkSpeed);
            transform.position = m_characterController.transform.position; 
        }

        SetMovementSpeed();
        JumpInput(); 
    }

    //Check if sprinting and adjust movement speed.
    void SetMovementSpeed()
    {
        if (m_inputHandler.GetSprintInput())
        {
            m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_sprintSpeed, Time.deltaTime * m_acceleration);
        }
        else
        {
            m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_walkSpeed, Time.deltaTime * m_acceleration);
        }
    }

    void JumpInput()
    {
        if (m_inputHandler.GetJumpInputHeld() && !m_isJumping)
        {
            m_isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private bool IsOnSlope()
    {
        if (m_isJumping)
        {
            return false;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_characterController.height / 2 * 1.3f))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator JumpEvent()
    {
        float timeInAir = 0;

        do
        {
            float jumpForce = m_jumpFalloff.Evaluate(timeInAir);
            m_characterController.Move(Vector3.up * jumpForce * m_jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (m_characterController.isGrounded && m_characterController.collisionFlags != CollisionFlags.Above);
        m_isJumping = false; 
    }

    public void SetupForMovement()
    {
        m_rotationHelper = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper").gameObject;
    }
}
