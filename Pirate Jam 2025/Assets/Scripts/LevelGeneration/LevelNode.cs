using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Start, Encounter, Boss, Treasure, Mystery, End }

[System.Serializable]
public class LevelNode
{
    public NodeType nodeType;
    public Vector2 position;
    [System.NonSerialized]
    public List<LevelNode> connectedNodes = new List<LevelNode>();
}
