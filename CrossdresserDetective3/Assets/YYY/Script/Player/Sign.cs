using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    public PlayerInputControl playerInput;

    public Transform playerTrans;
    public GameObject signSprite;

    private Animator anim;
    private SpriteRenderer signRenderer;

    private bool canPress;
    private IInteractable targetItem;

    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        signRenderer = signSprite.GetComponent<SpriteRenderer>();

        playerInput = new PlayerInputControl();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        playerInput.Gameplay.Confirm.started -= OnConfirm;

        playerInput.Disable();
    }

    private void OnDestroy()
    {
        InputSystem.onActionChange -= OnActionChange;
        playerInput.Gameplay.Confirm.started -= OnConfirm;
    }

    private void Update()
    {
        if (signRenderer != null)
            signRenderer.enabled = canPress;

        if (signSprite != null && playerTrans != null)
            signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress && targetItem != null)
        {
            targetItem.TriggerAction();
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (anim == null) return;

        if (actionChange == InputActionChange.ActionStarted)
        {
            InputAction action = obj as InputAction;
            if (action == null || action.activeControl == null) return;

            var device = action.activeControl.device;

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
            else if (device is Gamepad)
            {
                anim.Play("xbox");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable item = other.GetComponent<IInteractable>();

            if (item != null)
            {
                canPress = true;
                targetItem = item;
            }
        }
        else
        {
            canPress = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = false;
            targetItem = null;
        }
    }
}