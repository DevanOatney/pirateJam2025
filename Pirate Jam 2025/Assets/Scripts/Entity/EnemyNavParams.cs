using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNavParams", menuName = "Scriptable Objects/EnemyNavParams")]
public class EnemyNavParams : ScriptableObject
{
    // who am i allowed to target?
    public NavTarget allowedTargets;

    // how to i determine the best target?
    public NavPriority targetPriority;

    // how long do i search for a target before giving up?
    public float targetSearchTime;

    // how close to spot a target?
    public float detectionRange;

    // how fast does the enemy move when the player is too close?
    public float retreatMoveSpeedMult;

    // how close can the target be?
    public float minDistance;
}
