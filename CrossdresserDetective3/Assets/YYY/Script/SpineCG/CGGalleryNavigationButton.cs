using UnityEngine;
using UnityEngine.UI;

public class CGGalleryNavigationButton : Button
{
    [SerializeField] private Selectable leftTarget;
    [SerializeField] private Selectable rightTarget;

    public override Selectable FindSelectableOnLeft()
    {
        return CanSelect(leftTarget) ? leftTarget : base.FindSelectableOnLeft();
    }

    public override Selectable FindSelectableOnRight()
    {
        return CanSelect(rightTarget) ? rightTarget : base.FindSelectableOnRight();
    }

    private static bool CanSelect(Selectable target)
    {
        return target != null && target.IsActive() && target.IsInteractable();
    }
}
