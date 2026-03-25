using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonState : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private bool isSelected = false;
    private bool isPressed = false;

    // Called when the button becomes the currently selected UI element
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        Debug.Log(this.gameObject.name + " was selected");
    }

    // Called when the button stops being the selected UI element
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        // Ensure pressed state is reset when deselected
        isPressed = false;
        Debug.Log(this.gameObject.name + " was deselected");
    }

    // Called when a pointer (mouse/touch) is pressed down on the button
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    // Called when a pointer is released from the button
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    // Public properties to check the states from other scripts
    public bool IsSelected { get { return isSelected; } }
    public bool IsPressed { get { return isPressed; } }
}
