using UnityEngine;

[CreateAssetMenu(fileName = "EntityParams", menuName = "Scriptable Objects/EntityParams")]
public class EntityParams : ScriptableObject
{
    public float baseHealth;
    public float baseDamageIgnoreTime;
    public float baseMoveSpeed;
    public float baseAttackDamage;
    public float baseAttackRange;

    public float baseAttackPre;
    public float baseAttackPost;
    public float baseAttackTime;
    public float baseAttackCooldown;

    // move speed on attack
    public float moveSpeedMultOnAttack;

    // attack
    public float initAttackDamageMult;

    // health
    public float initHealthMult;
    
    // speed
    public float initMoveSpeedMult;

    // health + attack
    public float initLifeStealMult;
    
    // attack + speed
    public float initAttackSpeedMult;

    // health + speed
    public float initHealthRegenSpeedMult;

    // health + health
    // won't use, maybe for enemies?
    public float initDamageReduceMult;
}
