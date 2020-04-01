using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input_Handler : MonoBehaviour
{
    [SerializeField] private float m_lookSensitivity = 100f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Lock the cursor to the game.
    }

    public bool CanInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public bool GetLeftMousePressed()
    {
        if (CanInput())
        {
            if (Input.GetButton(GameConstants.BUTTON_FIRE))
            {
                return true;
            }
        }
        return false;
    }

    //-------------------------------------------------------------
    //Mouse Controls. 
    //-------------------------------------------------------------

    public bool GetLeftMouseHeld()
    {
        if (CanInput())
        {
            if (Input.GetButtonDown(GameConstants.BUTTON_FIRE))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetRightMousePressed()
    {
        if (CanInput())
        {
            if (Input.GetButton(GameConstants.BUTTON_AIM))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetRightMouseHeld()
    {
        if (CanInput())
        {
            if (Input.GetButtonDown(GameConstants.BUTTON_AIM))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetRightMouseReleased()
    {
        if (CanInput())
        {
            if (Input.GetButtonUp(GameConstants.BUTTON_AIM))
            {
                return true;
            }
        }
        return false;
    }

    //---------------------------------------------------------------
    //Keyboard Buttons. 
    //---------------------------------------------------------------
    public bool GetInteractPressed()
    {
        if(CanInput())
        {
            if(Input.GetButtonDown(GameConstants.BUTTON_INTERACT))
            {
                Debug.Log("[INFO] Interact button pressed.");
                return true; 
            }
        }
        return false; 
    }

    public bool GetReloadPressed()
    {
        if(CanInput())
        {
            if (Input.GetButtonDown(GameConstants.BUTTON_RELOAD))
            {
                Debug.Log("[INFO] Button pressed: Reload");
                return true;
            }
        }
        return false;
    }

    public bool GetSwitchWeapon()
    {
        if(CanInput())
        {
            if (Input.GetButtonDown(GameConstants.BUTTON_SWITCH_WEAPON))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public Vector3 GetMoveInput()
    {
        if (CanInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.AXIS_HORIZONTAL), 0f, Input.GetAxisRaw(GameConstants.AXIS_VERTICAL));

            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    //Check if sprint button is helf down. 
    public bool GetSprintInput()
    {    
        if (CanInput())
        {
            return Input.GetButton(GameConstants.BUTTON_SPRINT);
        }

        return false;
    }

    //Check if jump is pressed.
    public bool GetJumpInputDown()
    {
        if (CanInput())
        {
            return Input.GetButtonDown(GameConstants.BUTTON_JUMP);
        }

        return false;
    }

    //Check if jump is held down.
    public bool GetJumpInputHeld()
    {
        if (CanInput())
        {
            return Input.GetButton(GameConstants.BUTTON_JUMP);
        }

        return false;
    }

    public float GetVerticalLookInput()
    {
        if (CanInput())
        {
            return GetLookAxis(GameConstants.MOUSE_AXIS_VERTICAL, GameConstants.GAMEPAD_AXIS_VERTICAL);
        }
        return 0; 
    }

    public float GetHorizontalLookInput()
    {
        if (CanInput())
        {
            return GetLookAxis(GameConstants.MOUSE_AXIS_HORIZONTAL, GameConstants.GAMEPAD_AXIS_HORIZONTAL);
        }
        return 0; 
    }

    public float GetLookAxis(string mouseInputName, string stickInputName)
    {
        if (CanInput())
        {
            bool isGamePad = Input.GetAxis(stickInputName) != 0f;
            float i = isGamePad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

            i *= m_lookSensitivity;

            if (isGamePad)
            {
                i *= Time.deltaTime;
            }
            else
            {
                i *= 0.01f;
            }
        
            return i;
        }
        else
        {
            return 0f; 
        }
  
    }

    public bool GetInventoryPressed()
    {       
        if (Input.GetButtonDown(GameConstants.BUTTON_INVENTORY))
        {
            return true;
        }
        return false;         
    }
}
