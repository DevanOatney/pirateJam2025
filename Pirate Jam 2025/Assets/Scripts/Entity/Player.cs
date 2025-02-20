using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Player : Entity
{
    public GameObject hitbox;
    public Vector2 look;

    
    public Vector3 currentVelocity;

    public UnityEvent PauseRequested;

    // INPUT
    public InputSystem_Actions controls;
    public CharacterController character;
    public Image healthFillUi;

    public bool canPause;
    private bool isPauseHeld;
    private bool hasStoppedPause = true;
    
    private void Awake()
    {
        character = GetComponent<CharacterController>();
        controls = new InputSystem_Actions();
        controls.Player.Move.Enable();
        controls.Player.Attack.Enable();
        controls.UI.Pause.Enable();
    }

    protected override void Start()
    {
        base.Start();
        hitbox.SetActive(false);
        UpdateAttackHitbox();
        canAttack = true;
    }

    private float lastRange;

    protected override void Update()
    {
        base.Update();

        if (lastRange != attributes.baseAttackRange)
        {
            UpdateAttackHitbox();
        }

        ReadPauseInput();
        ReadMoveInput();
        character.Move(currentVelocity);
        ReadAttackInput();
    }

    protected virtual void UpdateAttackHitbox()
    {
        hitbox.transform.localScale = new Vector3(2.5f * attributes.baseAttackRange, 2.5f * attributes.baseAttackRange, 2.5f * attributes.baseAttackRange);
        hitbox.GetComponent<SphereCollider>().radius = 0.25f * attributes.baseAttackRange;

        Debug.Log($"Updated attack hitbox from {lastRange} to {attributes.baseAttackRange}");
        lastRange = attributes.baseAttackRange;
    }

    private void ReadPauseInput()
    {
        if (controls.UI.Pause.IsPressed())
        {
            isPauseHeld = true;
        }
        else
        {
            isPauseHeld = false;
            hasStoppedPause = true;
        }

        
        if (isPauseHeld && hasStoppedPause)
        {
            // prevent reading lingering input
            hasStoppedPause = false;
            PauseRequested.Invoke();
        }
    }

    private void ReadAttackInput()
    {
        if (controls.Player.Attack.IsPressed())
        {
            TryAttack();
        }
    }

    private void ReadMoveInput()
    {
        //Gets movement input data and turns it into a vector2 to apply towards the characters movement animation
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (!canMove)
        {
            moveInput = Vector2.zero;
        }

        currentVelocity = moveSpeed * Time.deltaTime * new Vector3(moveInput.x, 0, moveInput.y);
        isMoving = moveInput != Vector2.zero;

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

    protected override void OnDeath()
    {
        // respawn, game over, etc....
        canMove = false;
        canAttack = false;
    }

    protected override void OnHealthUpdated(float previous, float current)
    {
        float percent = current / maxHealth;
        healthFillUi.fillAmount = percent;
        Debug.Log("Health bar updated");
    }

    protected override void OnAttackStart()
    {
        hitbox.SetActive(true);
    }

    protected override void OnAttackStop()
    {
        hitbox.SetActive(false);
    }
}
