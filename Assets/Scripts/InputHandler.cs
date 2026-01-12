using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public InputActionAsset InputAsset;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction interactionAction;
    private InputAction crouchAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 MouseInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool crouchTriggered { get; private set; }
    public bool interactionTriggered { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InputActionMap mapReference = InputAsset.FindActionMap("Player");

        moveAction = mapReference.FindAction("Move");
        lookAction = mapReference.FindAction("Look");
        jumpAction = mapReference.FindAction("Jump");
        crouchAction = mapReference.FindAction("Crouch");
        interactionAction = mapReference.FindAction("Interact");

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        moveAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        moveAction.canceled += inputInfo => MovementInput = Vector2.zero;

        lookAction.performed += inputInfo => MouseInput = inputInfo.ReadValue<Vector2>();
        lookAction.canceled += inputInfo => MouseInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;

        crouchAction.performed += inputInfo => crouchTriggered = true;
        crouchAction.canceled += inputInfo => crouchTriggered = false;

        interactionAction.performed += inputInfo => interactionTriggered = true;
        interactionAction.canceled += inputInfo => interactionTriggered = false;
    }
}
