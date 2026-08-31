using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    public PlayerController playerController;//玩家不落地不能进行交互

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
            signSprite.transform.localScale = playerTrans.localScale/2;


        // 防止目标隐藏、销毁或改变Tag后，E提示残留
        if (targetItem != null)
        {
            MonoBehaviour targetBehaviour =
                targetItem as MonoBehaviour;

            bool targetInvalid =
                targetBehaviour == null ||
                !targetBehaviour.gameObject.activeInHierarchy ||
                !targetBehaviour.CompareTag("Interactable");

            if (targetInvalid)
            {
                ClearInteraction();
            }
        }

    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {

        if (canPress && targetItem != null)
        {



            if (!canPress || targetItem == null|| playerController.isCaptured)//被抓住无法进行交互
                return;
            IInteractable currentTarget = targetItem;


            targetItem.TriggerAction();

            // 交互后检查目标是否已经失效、隐藏或取消交互Tag
            MonoBehaviour targetBehaviour =
                currentTarget as MonoBehaviour;

            if (targetBehaviour == null ||
                !targetBehaviour.gameObject.activeInHierarchy ||
                !targetBehaviour.CompareTag("Interactable"))
            {
                ClearInteraction();
            }
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
        if (other.CompareTag("Interactable")&& playerController.physicsCheck.isGround)
        {
            IInteractable item = other.GetComponent<IInteractable>();

            if (item != null)
            {
                canPress = true;
                targetItem = item;
            }
        }
        //else
        //{
        //    canPress = false;
        //}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            //canPress = false;
            //targetItem = null;

            ClearInteraction();
        }
    }


    public void ClearInteraction()
    {
        canPress = false;
        targetItem = null;

        if (signRenderer != null)
        {
            signRenderer.enabled = false;
        }
    }//防止万一目标被消耗等，E残留
}