using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public Vector3 movement;
    public InputSystem_Actions controls;
    public bool moving = false;
    public Vector2 facingVector;
    public FacingDirection direction = FacingDirection.Down;

    public CharacterController characterController;

    public enum FacingDirection
    {
        Left,
        Right,
        Up,
        Down,
        UpRight,
        DownRight,
        UpLeft,
        DownLeft
    }

    void Awake()
    {
        //Fetching character controller
        characterController = GetComponent<CharacterController>();

        //Sets controls to a new input master
        controls = new InputSystem_Actions();

        //You must enable an action before it is used
        controls.FindAction("Move").Enable();
    }

    void FixedUpdate()
    {
        Vector3 move;

        move = movement;

        Movement();

        characterController.Move(movement);
    }

    public void Movement()
    {
        Keyboard kb = InputSystem.GetDevice<Keyboard>();

        //Gets movement input data and turns it into a vector2 to apply towards the characters movement animation
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        //8 directional movement
        movement = new Vector3(moveInput.x, 0, moveInput.y) * (moveSpeed * 10) * Time.fixedDeltaTime;

        //Uses input data to determine direction.
        /*
         * Up = (0, 1)
         * Down = (0, -1)
         * Left = (-1, 0)
         * Right =  (1, 0)
         * Up + Right = (1, 1)
         * Down + Right = (1, -1)
         * Up + Left = (-1, 1)
         * Down + Left = (-1, -1)
        */
        //(0,1)
        if (moveInput == new Vector2(0, 1))
        {
            direction = FacingDirection.Up;
            facingVector = new Vector2(0, 1);
        }
        //(0,-1)
        else if (moveInput == new Vector2(0, -1))
        {
            direction = FacingDirection.Down;
            facingVector = new Vector2(0, -1);
        }
        //(-1,0)
        else if (moveInput == new Vector2(-1, 0))
        {
            direction = FacingDirection.Left;
            facingVector = new Vector2(-1, 0);
        }
        //(1,0)
        else if (moveInput == new Vector2(1, 0))
        {
            direction = FacingDirection.Right;
            facingVector = new Vector2(1, 0);
        }
        //Diagonals
        //(1,1)
        else if (moveInput.x > 0 && moveInput.y > 0)
        {
            direction = FacingDirection.UpRight;
            facingVector = new Vector2(1, 1);
        }
        //(1, -1)
        else if (moveInput.x > 0 && moveInput.y < 0)
        {
            direction = FacingDirection.DownRight;
            facingVector = new Vector2(1, -1);
        }
        //(-1, 1)
        else if (moveInput.x < 0 && moveInput.y > 0)
        {
            direction = FacingDirection.UpLeft;
            facingVector = new Vector2(-1, 1);

        }
        //(-1, -1)
        else if (moveInput.x < 0 && moveInput.y < 0)
        {
            direction = FacingDirection.DownLeft;
            facingVector = new Vector2(-1, -1);
        }

        //If there is no move input, you are not running, otherwise, you are
        if (moveInput == new Vector2(0, 0))
        {
            moving = false;
        }
        else
        {
            moving = true;
        }
    }
}
