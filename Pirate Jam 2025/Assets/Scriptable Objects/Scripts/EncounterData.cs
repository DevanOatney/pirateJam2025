using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "LevelNodeData/EncounterData")]
public class EncounterData : ScriptableObject
{
    public string combatName;
    public List<EnemyWave> enemyWaves;
    [Range(0f, 1f)] public float probability = 1f; 
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemyData> enemies;
}

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public int health;
    public int damage;
}
