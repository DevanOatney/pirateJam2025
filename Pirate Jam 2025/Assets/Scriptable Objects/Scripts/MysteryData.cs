using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MysteryData", menuName = "LevelNodeData/MysteryData")]
public class MysteryData : ScriptableObject
{
    public string mysteryName;
    public string dialogue;
    public Sprite icon;
    public List<BuffData> effects;
    [Range(0f, 1f)] public float probability = 1f;
}
