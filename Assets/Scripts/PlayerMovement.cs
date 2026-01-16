using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    private float MovementVelocity;
    private float Gravity = 10.14f;
    private float FallingSpeed = 0f;

    private float JumpTime = 0f;

    private Vector3 SavedVelocity;

    private float MaxVelocity=2.0f;
    private float Acceleration=8f;
    private float Deceleration=10f;

    public float XSensitivity;
    public float YSensitivity;

    public float CurrentScroll=35f;

    private float yFacing;

    private bool IsCrouching;
    private bool IsSprinting;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private InputHandler PlayerInputHandler;
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject GlobalVolume;
    [SerializeField] private GameObject PlayerHitbox;
    [SerializeField] private GameObject PlayerModelHead;
    [SerializeField] private Animator AnimationController;

    Vector2 MovementSpeed;
    bool IsJumping;

    private void Crouch()
    {
        if (PlayerInputHandler.crouchTriggered && !IsCrouching && !IsSprinting)
        {
            IsCrouching = true;
            AnimationController.SetBool("Crouching",true);

            MaxVelocity -= 0.5f;

            PlayerHitbox.transform.localScale = new Vector3(PlayerHitbox.transform.localScale.x, PlayerHitbox.transform.localScale.y/1.5f, PlayerHitbox.transform.localScale.z);
        }
        if (!PlayerInputHandler.crouchTriggered && IsCrouching)
        {
            IsCrouching = false;
            AnimationController.SetBool("Crouching", false);

            MaxVelocity += 0.5f;

            PlayerHitbox.transform.localScale = new Vector3(PlayerHitbox.transform.localScale.x, PlayerHitbox.transform.localScale.y * 1.5f, PlayerHitbox.transform.localScale.z);
        }
    }

    private void Look()
    {
        Vector2 MouseInput = PlayerInputHandler.MouseInput;
        Vector2 ScrollInput = PlayerInputHandler.ScrollInput;

        Camera CameraObject = Camera.GetComponent<Camera>();

        float FOVModifier = CameraObject.fieldOfView / 30;
        float SpeedModifier = ((MovementVelocity / MaxVelocity) * MaxVelocity);

        float xRotation = (MouseInput.x * (XSensitivity*FOVModifier) ) * Time.deltaTime;
        float yRotation = (MouseInput.y * (YSensitivity * FOVModifier)) * Time.deltaTime;

        float ScrollSpeed = (ScrollInput.y * Time.deltaTime)*1200;
        CurrentScroll = Mathf.Clamp(CurrentScroll - ScrollSpeed, 1, 30);

        transform.Rotate(Vector3.up, xRotation);

        yFacing = Mathf.Clamp(yFacing-yRotation, -90, 90);

        Vector3 CameraRotation = new Vector3(Camera.transform.eulerAngles.x, Camera.transform.eulerAngles.y, Camera.transform.eulerAngles.z);

        Quaternion NewFacing = Quaternion.Euler(yFacing, CameraRotation.y, CameraRotation.z);
        Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation,NewFacing,15*Time.deltaTime);

        Vignette vg;
        GlobalVolume.GetComponent<Volume>().profile.TryGet(out vg);
        vg.intensity.value = (30 - CameraObject.fieldOfView)*0.01f;

        CameraObject.fieldOfView = Mathf.Lerp(CameraObject.fieldOfView, CurrentScroll + SpeedModifier, 10*Time.deltaTime);
        CameraObject.transform.position = PlayerModelHead.transform.position + (transform.forward*0.25f);

        if (CurrentScroll<2)
        {
            AnimationController.SetBool("ZoomedIn",true);
        }
        else
        {
            AnimationController.SetBool("ZoomedIn", false);
        }
    }

    private void Jump()
    {
        IsJumping = PlayerInputHandler.JumpTriggered;

        if (JumpTime>Time.time)
        {
            FallingSpeed = -Gravity;
            Controller.Move(new Vector3(0,-FallingSpeed*Time.deltaTime,0));
        }
        else
        {
            if (Controller.isGrounded)
            {
                FallingSpeed = Mathf.Lerp(FallingSpeed,0,2.0f*Time.deltaTime);
                Controller.Move(new Vector3(0,-3*Time.deltaTime,0));

                if (IsJumping)
                {
                    JumpTime=Time.time+0.2f;
                    AnimationController.SetTrigger("Jump");
                }
            }
            else
            {
                FallingSpeed = Mathf.Lerp(FallingSpeed, Gravity, 10*Time.deltaTime);
                Controller.Move(new Vector3(0,-FallingSpeed*Time.deltaTime,0));
            }
        }
    }

    private void Movement()
    {
        MovementSpeed = PlayerInputHandler.MovementInput;
        Vector3 MoveDirection = ((transform.right*MovementSpeed.x)+(transform.forward*MovementSpeed.y)).normalized*MovementVelocity;

        AnimationController.SetFloat("MovementSpeed", MovementVelocity/MaxVelocity);

        if (PlayerInputHandler.sprintTriggered)
        {
            if (!IsSprinting && !IsCrouching)
            {
                IsSprinting = true;
                MaxVelocity += 6;

                AnimationController.SetBool("Sprinting", true);
            }
        }
        if (!PlayerInputHandler.sprintTriggered)
        {
            if (IsSprinting)
            {
                IsSprinting = false;
                MaxVelocity -= 6;

                AnimationController.SetBool("Sprinting", false);
            }
        }

        if (MovementSpeed.magnitude>0)
        {
            MovementVelocity=Mathf.Min(MovementVelocity+(Acceleration*Time.deltaTime),MaxVelocity);
            SavedVelocity = Vector3.Lerp(SavedVelocity,MoveDirection,Acceleration*Time.deltaTime);

            AnimationController.SetBool("Walking",true);
        }
        else
        {
            MovementVelocity=Mathf.Max(MovementVelocity-(Deceleration*Time.deltaTime),0);
            SavedVelocity = Vector3.Lerp(SavedVelocity,Vector3.zero,Deceleration*Time.deltaTime);

            AnimationController.SetBool("Walking", false);
        }

        Controller.Move(SavedVelocity*Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Crouch();
        Jump();
        Look();
    }
}
