using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParams", menuName = "Scriptable Objects/PlayerParams")]
public class PlayerParams : ScriptableObject
{
    public float baseMoveSpeed;
    public float baseAttack;
    public float baseAttackSpeed;
    public float baseHealth;
}
