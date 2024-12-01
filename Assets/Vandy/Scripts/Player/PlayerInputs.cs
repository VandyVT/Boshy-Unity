using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction shootAction;
    [HideInInspector] public InputAction killAction;
    [HideInInspector] public InputAction resetAction;
    [HideInInspector] public InputAction optionsAction;

    public Vector2 movementVector;

    public PlayerInput playerInput;

    public static PlayerInputs instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        killAction = playerInput.actions["Kill"];
        resetAction = playerInput.actions["Reset"];
        optionsAction = playerInput.actions["Pause"];
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        shootAction.Disable();
        killAction.Disable();
        resetAction.Disable();
        optionsAction.Disable();
    }

    void OnMove(InputValue value)
    {
        movementVector = value.Get<Vector2>();

        Gamepad gamepad = Gamepad.current;

        if (gamepad != null)
        {
            Vector2 leftStickInput = gamepad.leftStick.ReadValue();
        }

        if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Handheld)
        {
            /*if (MobileControls.instance.fixedJoystick.Direction.magnitude >= 0f)
            {
                movementVector = MobileControls.instance.fixedJoystick.Direction;
            }

            else if (gamepad != null)
            {
                Vector2 leftStickInput = gamepad.leftStick.ReadValue();
                movementVector = leftStickInput;
            }*/
        }

    }
}
