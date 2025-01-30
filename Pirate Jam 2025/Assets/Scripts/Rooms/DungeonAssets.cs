using System.Collections.Generic;
using System.Linq;

public static class DungeonAssets
{
    public static readonly HashSet<Room> RoomPresets = new HashSet<Room>
    {
        new Room { type = RoomType.Start },
        new Room { type = RoomType.Main },
        new Room { type = RoomType.Boss },
        new Room { type = RoomType.Barrier } // i don't think this needs to be here
    };

    public static Room GetRandomRoomWithType(RoomType type)
    {
        return RNG.Choose(RoomPresets.Where(x => x.type == type));
    }

    public static Room GetRandomRoomWithTag(string tag)
    {
        return RNG.Choose(RoomPresets.Where(x => x.tags.Contains(tag)));
    }
}
