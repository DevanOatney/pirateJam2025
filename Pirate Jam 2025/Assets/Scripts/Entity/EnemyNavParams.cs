using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNavParams", menuName = "Scriptable Objects/EnemyNavParams")]
public class EnemyNavParams : ScriptableObject
{
    // how close to spot a player
    public float detectionRange;


    // how fast does the enemy move when the player is too close?
    public float retreatMoveSpeedMult;

    // how close can the target be?
    public float minDistance;
}
