using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Room
{
    public Room()
    {
        type = RoomType.Empty;
        x = -1;
        y = -1;
        tags = new List<string>();
        canUseTunnels = true;
        allowNewConnections = false;
        minTunnels = 1;
        maxTunnels = 3;
        minTunnelLength = 2;
        maxTunnelLength = 3;
    }

    public Room(RoomType t, int rx, int ry, bool allowTunnels, string roomTag = null) : this()
    {
        type = t;
        x = rx;
        y = ry;
        canUseTunnels = allowTunnels;
        tag = roomTag;

        if (type == RoomType.Main || type == RoomType.Start)
        {
            allowNewConnections = true;
        }
    }

    public void AddRoomToCurrentScene()
    {
        Debug.Log($"Getting room preset for type {type}...");
        Room target = DungeonAssets.GetRandomRoomWithType(type);
        
    }

    public void CreateTunnelBuilder()
    {
        if (!canUseTunnels)
        {
            Debug.Log($"[Room] Room ({type} at X:{x} Y:{y}) does not allow tunnels...");
            return;
        }

        tunnel = new TunnelBuilder(startX, startY, defaultType, defaultTag, this)
        {
            currentTunnelCount = Random.Range(minTunnels, maxTunnels),
            minLength = minTunnelLength,
            maxLength = maxTunnelLength,
            defaultType = defaultType
        };

        Debug.Log($"[Room] Created tunnel builder at X:{startX} Y:{startY}");
    }

    // position of room relative to its current dungeon
    public int x;
    public int y;

    public GameObject roomGameObject;

    
    public RoomType type;
    public RoomType defaultType;
    public string tag;
    public string defaultTag;
    public List<string> tags;
    
    // the tunnel that this room was sourced from, if any
    public TunnelBuilder tunnel;

    public bool required;
    public int queuePriority;
    // if true, the next update step for building dungeons will include this room
    public bool allowNewConnections;
    public bool blockAdjacentRooms;

    public bool canUseTunnels;
    public int minTunnels;
    public int maxTunnels;
    public int minTunnelLength;
    public int maxTunnelLength;
    public int startX;
    public int startY;
}
