using UnityEngine;

public class RoomPresetManager : MonoBehaviour
{
    public void LoadRoomPresets()
    {
        if (DungeonAssets.hasLoadedAssets)
        {
            return;
        }

        foreach (var item in FindObjectsByType<RoomPreset>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Debug.Log($"Loading dungeon of room {item.type}");
            DungeonAssets.RoomPresets.Add(new Room { roomGameObject = item.gameObject, type = item.type });
        }

        DungeonAssets.hasLoadedAssets = true;
    }
}
