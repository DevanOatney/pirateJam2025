using System.Collections.Generic;
using System.Linq;

public static class DungeonAssets
{
    public static bool hasLoadedAssets = false;

    public static List<Room> RoomPresets = new List<Room> { };

    public static Room GetRandomRoomWithType(RoomType type)
    {
        return RNG.Choose(RoomPresets.Where(x => x.type == type));
    }

    public static Room GetRandomRoomWithTag(string tag)
    {
        return RNG.Choose(RoomPresets.Where(x => x.tags.Contains(tag)));
    }
}
