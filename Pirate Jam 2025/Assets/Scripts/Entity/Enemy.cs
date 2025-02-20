using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public EnemyNavParams navParams;

    protected Entity target;
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
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
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

    // this method is used to determine what navigation target is most viable
    protected virtual void FindBestNavTarget()
    {
        // var entities = 
        // for each entity in the level
    }

    // protected virtual bool CheckLineOfSightForPosition(Vector3 position, float range, bool)

    protected virtual void UpdateNavAgent()
    {
        /*
        if (!canMove) return;

        // if we have no target, try looking for an available one.
        if (target == null)
        {
            // FindBestNavTarget();
            // return;
        }
        */

        if (target == null || !canMove) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        retreatMoveSpeedMult = 1.0f;
        st_nav = NavState.Idle;

        // If player is within detection range
        if (distanceToTarget <= navParams.detectionRange)
        {
            st_nav = NavState.Detect;
            agent.SetDestination(target.transform.position);
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
        if (distanceToTarget <= attributes.baseAttackRange)
        {
            st_nav = NavState.Attack;
            agent.isStopped = true;
            isMoving = false;
        }

        // If player is within min distance
        if (distanceToTarget <= navParams.minDistance)
        {
            st_nav = NavState.Retreat;
            Vector3 directionToPlayer = (target.transform.position - transform.position).normalized;
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

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // If the player is within attack range, attempt an attack
        if (distanceToTarget <= navParams.detectionRange && distanceToTarget <= attributes.baseAttackRange)
        {
            Debug.Log($"Distance to {target.navType}: {distanceToTarget} <= {attributes.baseAttackRange} and {navParams.detectionRange}");
            TryAttack();
        }
    }

    protected override void OnDeath()
    {
        canMove = false;
        canAttack = false;
    }
}
