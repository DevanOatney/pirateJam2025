using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public Vector3 velocity;
    public InputSystem_Actions controls;
    public bool moving = false;
    public Vector2 look;

    public CharacterController characterController;

    // TODO: Switch to event-based input detection
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

        move = velocity;

        ReadMoveInput();

        characterController.Move(velocity);
    }

    public void ReadMoveInput()
    {
        //Gets movement input data and turns it into a vector2 to apply towards the characters movement animation
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        velocity = new Vector3(moveInput.x, 0, moveInput.y) * (moveSpeed * 10) * Time.fixedDeltaTime;
        moving = moveInput != Vector2.zero;

        look = Vector2.zero;

        // Clamp movement to 8 directions
        if (moveInput.x > 0)
        {
            look += Vector2.right;
        }
        else if (moveInput.x < 0)
        {
            look += Vector2.left;
        }

        if (moveInput.y > 0)
        {
            look += Vector2.up;
        }
        else if (moveInput.y < 0)
        {
            look += Vector2.down;
        }
    }
}
