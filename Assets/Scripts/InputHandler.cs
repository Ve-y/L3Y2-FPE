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
    private InputAction sprintAction;
    private InputAction scrollAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 MouseInput { get; private set; }
    public Vector2 ScrollInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool crouchTriggered { get; private set; }
    public bool sprintTriggered { get; private set; }
    public bool interactionTriggered { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InputActionMap mapReference = InputAsset.FindActionMap("Player");

        moveAction = mapReference.FindAction("Move");
        lookAction = mapReference.FindAction("Look");
        jumpAction = mapReference.FindAction("Jump");
        crouchAction = mapReference.FindAction("Crouch");
        sprintAction = mapReference.FindAction("Sprint");
        interactionAction = mapReference.FindAction("Interact");
        scrollAction = mapReference.FindAction("Zoom");

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        moveAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        moveAction.canceled += inputInfo => MovementInput = Vector2.zero;

        lookAction.performed += inputInfo => MouseInput = inputInfo.ReadValue<Vector2>();
        lookAction.canceled += inputInfo => MouseInput = Vector2.zero;

        scrollAction.performed += inputInfo => ScrollInput = inputInfo.ReadValue<Vector2>();
        scrollAction.canceled += inputInfo => ScrollInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;

        crouchAction.performed += inputInfo => crouchTriggered = true;
        crouchAction.canceled += inputInfo => crouchTriggered = false;

        interactionAction.performed += inputInfo => interactionTriggered = true;
        interactionAction.canceled += inputInfo => interactionTriggered = false;

        sprintAction.performed += inputInfo => sprintTriggered = true;
        sprintAction.canceled += inputInfo => sprintTriggered = false;
    }
}
