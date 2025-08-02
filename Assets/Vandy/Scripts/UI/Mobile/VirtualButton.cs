using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Reflection;

public class VirtualButton : MonoBehaviour
{
    public UnityEvent onPress;
    public UnityEvent onRelease;

    public InputActionReference actionReference;
    [Tooltip("Specify the exact binding index (0-based)")]
    public int bindingIndex = 0;

    private bool isPressed = false;

    private void Update()
    {
        // Detect touch input (Mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, touchPosition))
            {
                if (touch.phase == UnityEngine.TouchPhase.Began)
                {
                    PressButton();
                }
                else if (touch.phase == UnityEngine.TouchPhase.Ended)
                {
                    ReleaseButton();
                }
            }
        }

        // Detect mouse input (PC - useful for testing)
        if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition))
        {
            PressButton();
        }
        if (Input.GetMouseButtonUp(0) && isPressed)
        {
            ReleaseButton();
        }
    }

    private void PressButton()
    {
        if (!isPressed)
        {
            isPressed = true;
            onPress?.Invoke();

            Debug.Log($"Button {gameObject.name} Pressed");

            if (actionReference != null && actionReference.action != null)
            {
                actionReference.action.Enable();
                TriggerBinding(true);
            }
        }
    }

    private void ReleaseButton()
    {
        isPressed = false;
        onRelease?.Invoke();

        Debug.Log($"Button {gameObject.name} Released");

        if (actionReference != null && actionReference.action != null)
        {
            TriggerBinding(false);
            actionReference.action.Disable();
        }
    }

    private void TriggerBinding(bool isPressed)
    {
        if (actionReference == null || actionReference.action == null)
            return;

        var action = actionReference.action;

        if (bindingIndex >= 0 && bindingIndex < action.bindings.Count)
        {
            Debug.Log($"Triggered binding: {action.bindings[bindingIndex].path}");

            // Use Reflection to call the internal Invoke method
            MethodInfo invokeMethod = typeof(InputAction).GetMethod("InvokeCallbacks",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (invokeMethod != null)
            {
                invokeMethod.Invoke(action, new object[] { new InputAction.CallbackContext() });
                Debug.Log($"Simulated input for {action.name}");
            }
            else
            {
                Debug.LogWarning($"Could not trigger input action {action.name} due to missing InvokeCallbacks method.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid binding index {bindingIndex} for action {action.name}");
        }
    }
}
