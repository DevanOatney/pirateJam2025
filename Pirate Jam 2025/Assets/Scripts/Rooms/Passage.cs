using UnityEngine;

public class Passage : MonoBehaviour
{
    public RoomDirection direction;
    public bool canUse;

    public Room target;


    public void GetBestTarget()
    {
        // use dungeon manager to get best target
    }

}
