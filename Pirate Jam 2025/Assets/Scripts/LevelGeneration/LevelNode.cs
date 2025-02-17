using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Start, Encounter, Elite, Treasure, Mystery, Boss, End }

[System.Serializable]
public class LevelNode
{
    public NodeType nodeType;
    public Vector2 position;
    [System.NonSerialized]
    public List<LevelNode> connectedNodes = new List<LevelNode>();

    // Node-specific data
    public TreasuryData treasuryData;
    public MysteryData mysteryData;
    public EncounterData combatData;
}
