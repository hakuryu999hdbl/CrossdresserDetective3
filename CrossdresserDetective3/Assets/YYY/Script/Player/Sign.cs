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

        // 第一优先读取上次设备
        if (GameFlowData.LastInputDevice >= 0)
        {
            RefreshPrompt(GameFlowData.LastInputDevice);
        }
        else
        {
            RefreshPrompt(Keyboard.current != null ? 0 : 2);
        }
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

    private bool lastCaptured;

    private void Update()
    {
        bool captured = playerController != null && playerController.isCaptured;

        // 普通状态：附近有可交互物才显示
        // 被抓状态：强制显示攻击/挣扎提示
        if (signRenderer != null)
        {
            signRenderer.enabled = captured || canPress;
        }

        if (signSprite != null && playerTrans != null)
        {
            signSprite.transform.localScale = playerTrans.localScale / 2;
        }

        // 抓取状态发生变化时，立刻换提示
        if (captured != lastCaptured)
        {
            lastCaptured = captured;

            int deviceType =
                GameFlowData.LastInputDevice >= 0
                ? GameFlowData.LastInputDevice
                : 0;

            RefreshPrompt(deviceType);
        }

        // 防止目标隐藏、销毁或改变Tag后提示残留
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

        if (actionChange != InputActionChange.ActionStarted)
            return;

        InputAction action = obj as InputAction;

        if (action == null || action.activeControl == null)
            return;

        var device = action.activeControl.device;

        int deviceType = -1;

        if (device is Keyboard)
        {
            deviceType = 0;
        }
        else if (device is DualShockGamepad)
        {
            deviceType = 1;
        }
        else if (device is XInputController)
        {
            deviceType = 2;
        }
        else if (device is Gamepad)
        {
            deviceType = 2;
        }

        if (deviceType < 0)
            return;

        // Sign自己检测到切换时也更新全局记录
        GameFlowData.LastInputDevice = deviceType;

        RefreshPrompt(deviceType);
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
            signRenderer.enabled =
                playerController != null &&
                playerController.isCaptured;
        }
    }//防止万一目标被消耗等，E残留


    private void RefreshPrompt(int deviceType)
    {
        if (anim == null)
            return;

        bool captured = playerController != null && playerController.isCaptured;

        switch (deviceType)
        {
            case 0:
                anim.Play(captured ? "keyboard_attack" : "keyboard");
                break;

            case 1:
                anim.Play(captured ? "ps_attack" : "ps");
                break;

            case 2:
                anim.Play(captured ? "xbox_attack" : "xbox");
                break;
        }
    }
}