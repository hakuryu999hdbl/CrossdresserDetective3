using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
//using UnityEngine.InputSystem.Android;

public class Sign : MonoBehaviour
{
    public PlayerInputControl playerInput;//多端输入

    public Transform playerTrans;
    private Animator anim;
    public GameObject signSprite;
    private bool canPress;


    private IInteractable targetItem;//当前正在互动的物体


    private void Awake()
    {
        // anim = GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;//通过当前不同的输入设备，显示不同的提示
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }
   

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
        }
    }


    /// <summary>
    /// 切换设备不同动画按键提示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="actionChange"></param>
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            // Debug.Log(((InputAction)obj).activeControl.device);

            var device = ((InputAction)obj).activeControl.device;

            if (device is Keyboard)
            {
                anim.Play("keyboard");
            }
            else if (device is DualShockGamepad)
            {
                anim.Play("ps");
            }
            else if (device is XInputController)
            {
                anim.Play("xbox");
            }
            //else if (device is Gamepad)
            //{
            //    anim.Play("gamepad");
            //}
            //else if (device is Touchscreen)
            //{
            //    anim.Play("android");
            //}
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();//获得交互物体
        }
        else 
        {
            canPress = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }

 
}
