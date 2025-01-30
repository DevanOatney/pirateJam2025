using UnityEngine;

public class DummyDungeonTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        



        Debug.Log("Opening the DungeonManager...");
        
        DungeonManager manager = new DungeonManager();
        
        Debug.Log(manager.state);

        



        // wait for 3 seconds
        new WaitForSeconds(3.0f);
        Debug.Log("Now loading the simple dungeon...");
        DungeonManager.Instance.LoadQueueFromDungeonTag(DungeonTag.Simple);
    }

    // Update is called once per frame
    void Update()
    {
        DungeonManager.Instance.Update();
    }
}
