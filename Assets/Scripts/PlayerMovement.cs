using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float MovementVelocity;
    private float Gravity = 10.14f;
    private float FallingSpeed = 0f;

    private float JumpTime = 0f;

    private Vector3 SavedVelocity;

    private float MaxVelocity=5.0f;
    private float Acceleration=6f;
    private float Deceleration=7f;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private InputHandler PlayerInputHandler;

    Vector2 MovementSpeed;
    bool IsJumping;

    private void Jump()
    {
        IsJumping = PlayerInputHandler.JumpTriggered;

        if (JumpTime>Time.time)
        {
            FallingSpeed = Mathf.Lerp(FallingSpeed,-Gravity,8.0f*Time.deltaTime);
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
                }
            }
            else
            {
                FallingSpeed = Mathf.Lerp(FallingSpeed,Gravity,4.0f*Time.deltaTime);
                Controller.Move(new Vector3(0,-FallingSpeed*Time.deltaTime,0));
            }
        }
    }

    private void Movement()
    {
        MovementSpeed = PlayerInputHandler.MovementInput;
        Vector3 MoveDirection = ((transform.right*MovementSpeed.x)+(transform.forward*MovementSpeed.y)).normalized*MovementVelocity;

        if (MovementSpeed.magnitude>0)
        {
            MovementVelocity=Mathf.Min(MovementVelocity+(Acceleration*Time.deltaTime),MaxVelocity);
            SavedVelocity = Vector3.Lerp(SavedVelocity,MoveDirection,Acceleration*Time.deltaTime);
        }
        else
        {
            MovementVelocity=Mathf.Max(MovementVelocity-(Deceleration*Time.deltaTime),0);
            SavedVelocity = Vector3.Lerp(SavedVelocity,Vector3.zero,Deceleration*Time.deltaTime);
        }

        Controller.Move(SavedVelocity*Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
    }
}
