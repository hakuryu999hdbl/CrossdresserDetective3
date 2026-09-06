using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InputPromptManager : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    [Header("对应图片（编号一致）")]
    public Sprite keyboardSprites;
    public Sprite psSprites;
    public Sprite xboxSprites;



    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;

        // =========================
        // 第一优先：读取上一个场景记录
        // =========================
        if (GameFlowData.LastInputDevice >= 0)
        {
            RefreshIcon(GameFlowData.LastInputDevice);
        }
        else
        {
            // 完全没有记录时才使用默认
            int defaultDevice = Keyboard.current != null ? 0 : 2;

            GameFlowData.LastInputDevice = defaultDevice;
            RefreshIcon(defaultDevice);
        }
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnDestroy()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange != InputActionChange.ActionStarted)
            return;

        InputAction action = obj as InputAction;

        if (action == null || action.activeControl == null)
            return;

        var device = action.activeControl.device;

        if (device is Keyboard)
        {
            RefreshIcon(0);
        }
        else if (device is DualShockGamepad)
        {
            RefreshIcon(1);
        }
        else if (device is XInputController)
        {
            RefreshIcon(2);
        }
        else if (device is Gamepad)
        {
            // 其它手柄默认走Xbox
            RefreshIcon(2);
        }
    }

    /// <summary>
    /// 0=Keyboard 1=PS 2=Xbox
    /// </summary>
    private void RefreshIcon(int deviceType)
    {
        if (icon == null)
            return;

        switch (deviceType)
        {
            case 0:
                icon.sprite = keyboardSprites;
                break;

            case 1:
                icon.sprite = psSprites;
                break;

            case 2:

                icon.sprite = xboxSprites;
                break;
        }
    }


}