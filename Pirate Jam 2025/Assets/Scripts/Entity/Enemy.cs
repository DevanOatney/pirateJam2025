using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public EnemyNavParams navParams;

    protected Transform target;
    //protected Transform target;
    protected NavMeshAgent agent;

    public float retreatMoveSpeedMult { get; protected set; }

    protected override void Start()
    {
        st_nav = NavState.Idle;
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        retreatMoveSpeedMult = 1.0f;
        agent.speed = moveSpeed;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        UpdateNavAgent();
        ReadAttack();
    }

    [SerializeField]
    private NavState st_nav;

    private enum NavState
    {
        Idle = 1,
        Detect = 2,
        Attack = 3,
        Retreat = 4
    }

    protected virtual void UpdateNavAgent()
    {
        if (target == null || !canMove) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        retreatMoveSpeedMult = 1.0f;
        st_nav = NavState.Idle;

        // If player is within detection range
        if (distanceToPlayer <= navParams.detectionRange)
        {
            st_nav = NavState.Detect;
            agent.SetDestination(target.position);
            agent.isStopped = false;
            isMoving = true;
        }
        else
        {
            // otherwise, we don't see them, adjust idle behavior here and move on
            agent.isStopped = true;
            isMoving = false;
            return;
        }

        // If player is within attack range
        if (distanceToPlayer <= attributes.baseAttackRange)
        {
            st_nav = NavState.Attack;
            agent.isStopped = true;
            isMoving = false;
        }

        // If player is within min distance
        if (distanceToPlayer <= navParams.minDistance)
        {
            st_nav = NavState.Retreat;
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            agent.SetDestination(transform.position + (-directionToPlayer * navParams.minDistance));
            retreatMoveSpeedMult = navParams.retreatMoveSpeedMult;
            canAttack = false;
            agent.isStopped = false;
            isMoving = true;
        }
        else
        {
            canAttack = true;
        }

        agent.speed = moveSpeed;
    }

    protected override float CalculateMoveSpeed(float baseSpeed)
    {
        float affectMult = isAttacking ? attributes.moveSpeedMultOnAttack : retreatMoveSpeedMult;
        float affectSpeed = baseSpeed * affectMult;
        float finalSpeed = base.CalculateMoveSpeed(affectSpeed);

        return finalSpeed;
    }

    protected virtual void ReadAttack()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // If the player is within attack range, attempt an attack
        if (distanceToPlayer <= navParams.detectionRange && distanceToPlayer <= attributes.baseAttackRange)
        {
            TryAttack();
        }
    }

    protected override void OnDeath()
    {
        canMove = false;
        canAttack = false;
    }
}
