using UnityEngine;
using UnityEngine.UI;

public class LevelNodeUIController : MonoBehaviour
{
    public LevelNode levelNode; // The associated logical LevelNode

    // Initialize this UI node with the associated logical node
    public void Initialize(LevelNode node)
    {
        levelNode = node;
    }
}
