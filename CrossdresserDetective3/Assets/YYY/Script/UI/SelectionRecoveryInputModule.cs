using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class SelectionRecoveryInputModule : InputSystemUIInputModule
{
    private Selectable selectionBeforePointerDeselect;
    private InputAction navigationAction;
    private int lastVerticalDirection;
    private bool pressedVertical;

    protected override void OnEnable()
    {
        base.OnEnable();
        navigationAction = move?.action;
        if (navigationAction != null)
        {
            navigationAction.performed += OnNavigation;
            navigationAction.canceled += OnNavigation;
        }
    }

    private void OnNavigation(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        int verticalDirection = 0;
        if (Mathf.Abs(movement.y) >= 0.5f && Mathf.Abs(movement.y) >= Mathf.Abs(movement.x))
            verticalDirection = movement.y > 0 ? 1 : -1;

        if (verticalDirection != 0 && verticalDirection != lastVerticalDirection)
            pressedVertical = true;

        lastVerticalDirection = verticalDirection;
    }

    public override void Process()
    {
        bool restoreOnMove = pressedVertical;
        pressedVertical = false;
        GameObject selectedBefore = eventSystem.currentSelectedGameObject;
        bool pressedPointer = leftClick?.action?.WasPressedThisFrame() == true
            || rightClick?.action?.WasPressedThisFrame() == true
            || middleClick?.action?.WasPressedThisFrame() == true;

        base.Process();

        if (eventSystem.currentSelectedGameObject != null)
        {
            selectionBeforePointerDeselect = null;
            return;
        }

        if (selectedBefore != null)
        {
            selectionBeforePointerDeselect = pressedPointer
                ? selectedBefore.GetComponent<Selectable>()
                : null;
            return;
        }

        if (selectionBeforePointerDeselect == null || !selectionBeforePointerDeselect.IsActive()
            || !selectionBeforePointerDeselect.IsInteractable())
        {
            selectionBeforePointerDeselect = null;
            return;
        }

        // Restore after processing the move so the first press only restores focus.
        if (restoreOnMove && eventSystem.sendNavigationEvents)
        {
            GameObject target = selectionBeforePointerDeselect.gameObject;
            selectionBeforePointerDeselect = null;
            eventSystem.SetSelectedGameObject(target);
        }
    }

    protected override void OnDisable()
    {
        if (navigationAction != null)
        {
            navigationAction.performed -= OnNavigation;
            navigationAction.canceled -= OnNavigation;
            navigationAction = null;
        }

        selectionBeforePointerDeselect = null;
        lastVerticalDirection = 0;
        pressedVertical = false;
        base.OnDisable();
    }
}
