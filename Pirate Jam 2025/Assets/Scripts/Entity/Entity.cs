using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class Entity : MonoBehaviour
{
    public delegate void HealthUpdatedParams(float previous, float current);

    // what is the name of this entity?
    public NavTarget navType;

    public EntityParams attributes;

    public UnityEvent<float, float> HealthUpdated;

    [SerializeField]
    public bool isDead { get; protected set; }

    public bool isAttacking => st_attack > 0 && st_attack < AttackState.AttackCooldown;
    public bool isAttackOnCooldown => st_attack == AttackState.AttackCooldown;

    public bool isMoving = false;

    // used to prevent update checks
    public bool canAttack = true;
    public bool canMove = true;

    public float maxHealth => Mathf.Max(0, attributes.baseHealth * healthMult);

    public float attackDamage => Mathf.Max(0, attributes.baseAttackDamage * attackDamageMult);
    public float attackCooldown => Mathf.Max(0, attributes.baseAttackCooldown * attackSpeedMult);

    public float moveSpeed => Mathf.Max(0, CalculateMoveSpeed(attributes.baseMoveSpeed));

    public float healthRegen => Mathf.Max(0, CalculateHealthRegen(attributes.baseHealth));


    protected virtual float CalculateMaxHealth(float baseHealth)
    {
        float health = baseHealth * healthMult;
        return health;
    }

    protected virtual float CalculateAttackDamage(float baseDamage)
    {
        float damage = baseDamage * attackDamageMult;
        return damage;
    }

    protected virtual float CalculateAttackCooldown(float baseCooldown)
    {
        float cooldown = baseCooldown * attackSpeedMult;
        return cooldown;
    }

    protected virtual float CalculateMoveSpeed(float baseSpeed)
    {
        float speed = baseSpeed * moveSpeedMult;
        return speed;
    }

    protected virtual float CalculateHealthRegen(float baseRegen)
    {
        float regen = baseRegen * healthRegenMult;
        return regen;
    }

    
    public float currentHealth { get; protected set; }
    public float timeSinceLastHurt { get; protected set; }

    // used to determine visuals
    public int powerPoints { get; set; }
    public int defensePoints { get; set; }
    public int speedPoints { get; set; }

    // used for balancing purposes
    public int totalPoints => powerPoints + defensePoints + speedPoints;

    // multipliers, we can conversely use these to make enemies stronger
    public float attackDamageMult { get; set; } = 1; // attack
    public float healthMult { get; set; } = 1; // health
    public float moveSpeedMult { get; set; } = 1; // speed
    public float lifeStealMult { get; set; } = 0; // attack + health
    public float attackSpeedMult { get; set; } = 1; // attack + speed
    public float healthRegenMult { get; set; } = 0; // health + speed

    // defenseMult // health + health
    // criticalChance // attack + attack
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        attackDamageMult = attributes.initAttackDamageMult;
        healthMult = attributes.initHealthMult;
        moveSpeedMult = attributes.initMoveSpeedMult;
        lifeStealMult = attributes.initLifeStealMult;
        attackSpeedMult = attributes.initAttackSpeedMult;
        healthRegenMult = attributes.initHealthRegenSpeedMult;
    }

    private float t_damageIgnore;

    protected virtual void Update()
    {
        UpdateAttack();
        RegenHealth();
        UpdateDamageGraceTime();
    }

    public bool CanTakeDamage()
    {
        return t_damageIgnore <= 0;
    }

    public void TakeDamage(float amount)
    {
        // avoid taking damage again if it just happened
        if (!CanTakeDamage())
        {
            return;
        }

        t_damageIgnore = attributes.baseDamageIgnoreTime;
        float previous = currentHealth;
        currentHealth -= amount;
        Debug.Log($"[{navType}] {gameObject.name} took {amount} damage. (Health: {currentHealth})");

        if (Mathf.Abs(currentHealth - previous) > 0)
        {
            HealthUpdated.Invoke(previous, currentHealth);
            OnHealthUpdated(previous, currentHealth);
        }

        if (currentHealth <= 0)
        {
            // THEY FREAKIGN DIED!!!!!!
            Die();
        }
    }

    public void Heal(float amount)
    {
        float previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"[{navType}] {gameObject.name} recovered {amount}. (Health: {currentHealth})");

        if (Mathf.Abs(currentHealth - previous) > 0)
        {
            HealthUpdated.Invoke(previous, currentHealth);
            OnHealthUpdated(previous, currentHealth);
        }
    }

    public void RegenHealth()
    {
        if (isDead)
        {
            return;
        }

        if (healthRegen > 0)
        {
            Heal(healthRegen * Time.deltaTime);
        }
    }

    public void TryHealFromDamage(float damageDealt)
    {
        if (lifeStealMult > 0)
        {
            Heal(damageDealt * lifeStealMult);
        }
    }

    public void Die()
    {
        Debug.Log($"[{navType}] {gameObject.name} has died.");
        isDead = true;
        OnDeath();
    }

    protected void TryAttack()
    {
        if (isDead || !canAttack || isAttacking || isAttackOnCooldown)
        {
            return;
        }

        st_attack++; // move to ready state
        UpdateAttack();
    }

    private void UpdateDamageGraceTime()
    {
        if (t_damageIgnore > 0)
        {
            t_damageIgnore -= Time.deltaTime;

            if (t_damageIgnore <= 0)
            {
                t_damageIgnore = 0;
            }
        }
    }

    private enum AttackState
    {
        AttackReady = 1,
        AttackPre = 2,
        Attack = 3,
        AttackPost = 4,
        AttackCooldown = 5
    }

    [SerializeField]
    private AttackState st_attack;
    [SerializeField]
    private float t_attack;
    [SerializeField]
    private float t_attackEnd;

    private void UpdateAttack()
    {
        // don't update if we're not attacking
        if (st_attack <= 0)
        {
            return;
        }

        // if the entity is dead, stop attacking
        if (isDead)
        {
            st_attack = 0;
            t_attack = 0;
            t_attackEnd = 0;
            OnAttackCancel();
            return;
        }

        // if we passed our target time
        if (t_attack >= t_attackEnd)
        {
            // reset timer
            t_attack = 0;
            t_attackEnd = 0;
            // CALLED BEFORE WE SET NEXT STATE
            // stop our active attack as soon as we hit their timer
            if (st_attack == AttackState.Attack)
            {
                OnAttackStop();
            }

            // stop if the state of this attack was on cooldown
            if (st_attack == AttackState.AttackCooldown)
            {
                st_attack = 0;
                return;
            }

            

            // skip states until we hit cooldown or we have a time specified
            while (t_attackEnd <= 0 && st_attack < AttackState.AttackCooldown)
            {
                st_attack++;

                // set new target for timer
                t_attackEnd = st_attack switch
                {
                    AttackState.AttackPre => attributes.baseAttackPre,
                    AttackState.Attack => attributes.baseAttackTime,
                    AttackState.AttackPost => attributes.baseAttackPost,
                    AttackState.AttackCooldown => attackCooldown,
                    _ => 0
                };


                Debug.Log($"Moving to state {st_attack} with time {t_attackEnd}");
            }

            // CALLED AFTER WE SET NEXT STATE
            // call our next method based on the new state provided
            switch (st_attack)
            {
                case AttackState.AttackPre:
                    OnAttackPre();
                    break;

                case AttackState.Attack:
                    OnAttackStart();
                    break;

                case AttackState.AttackPost:
                    OnAttackPost();
                    break;
            }
        }

        // update attack timer
        t_attack += Time.deltaTime;
    }

    public void EquipMod(Mod mod)
    {
        if (mod.onEquip != null)
        {
            float previousMax = maxHealth;
            mod.onEquip(this);

            powerPoints += mod.powerPoints;
            defensePoints += mod.defensePoints;
            speedPoints += mod.speedPoints;

            OnMaxHealthUpdated(previousMax, maxHealth);
            OnModEquipped(mod);
        }
    }

    protected virtual void OnHealthUpdated(float previous, float current) { }

    protected virtual void OnMaxHealthUpdated(float previous, float current)
    {
        // we lied
        if (previous == current)
        {
            return;
        }

        // kill if max health is 0 or less
        if (current <= 0)
        {
            Die();
            return;
        }


        if (current > currentHealth)
        {
            // raise current health to max if they haven't taken damage
            if (currentHealth == previous)
            {
                currentHealth = maxHealth;
            }
        }
        else
        {
            // lower current health to match max health
            currentHealth = maxHealth;
        }


    }

    protected virtual void OnDeath() { }

    protected virtual void OnModEquipped(Mod mod) { }

    protected virtual void OnAttackPre() { }
    protected virtual void OnAttackPost() { }
    protected virtual void OnAttackStart() { }
    protected virtual void OnAttackCancel() { }
    protected virtual void OnAttackStop() { }
}
