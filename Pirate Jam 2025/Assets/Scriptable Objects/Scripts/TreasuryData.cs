using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasuryData", menuName = "LevelNodeData/TreasuryData")]
public class TreasuryData : ScriptableObject
{
    public string treasuryName;
    public string description;
    public Sprite icon;
    public List<BuffData> buffChoices;
    [Range(0f, 1f)] public float probability = 1f;
}

[System.Serializable]
public class BuffData
{
    public string buffName;
    public string description;
    public Sprite icon;
    public int effectValue; // e.g., +10% damage
}
