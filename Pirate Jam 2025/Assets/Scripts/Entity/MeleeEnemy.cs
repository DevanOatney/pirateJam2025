using UnityEngine;

public class MeleeEnemy : Enemy
{
    public GameObject hitbox;

    protected override void Start()
    {
        base.Start();
        hitbox.SetActive(false);
        UpdateAttackHitbox();
    }

    private float lastRange;

    protected override void Update()
    {
        base.Update();

        if (lastRange != attributes.baseAttackRange)
        {
            UpdateAttackHitbox();
        }
    }

    protected virtual void UpdateAttackHitbox()
    {
        hitbox.transform.localScale = new Vector3(2.5f * attributes.baseAttackRange, 2.5f * attributes.baseAttackRange, 2.5f * attributes.baseAttackRange);
        hitbox.GetComponent<SphereCollider>().radius = 0.25f * attributes.baseAttackRange;
        Debug.Log($"[MeleeEnemy] Updated attack hitbox from {lastRange} to {attributes.baseAttackRange}");
        lastRange = attributes.baseAttackRange;
    }

    protected override void OnAttackStart()
    {
        hitbox.SetActive(true);
    }

    protected override void OnAttackStop()
    {
        hitbox.SetActive(false);
    }

    protected override void OnAttackCancel()
    {
        hitbox.SetActive(false);
    }
}
