using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static readonly int MapWidth = 20;
    public static readonly int MapHeight = 20;

    public static DungeonManager Instance { get; private set; }

    public DungeonManager()
    {
        currentMap = new Grid<Room>(20, 20);
        ClearMap();
        queuedRooms = new List<Room>();
        currentDungeonTag = DungeonTag.Simple;
        state = DungeonState.QueueProcess;
        currentX = GetMapOriginX();
        currentY = GetMapOriginY();
        generatedDungeon = false;
        roomGenX = 0;
        roomGenY = 0;
        hasRequestedLoad = false;
        updateRequired = true;
        allowedToContinue = true;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadQueueFromDungeonTag(currentDungeonTag);
    }

    public DungeonState state;
    
    public Grid<Room> currentMap;
    public string currentDungeonTag;
    public string targetDungeonTag; // where to go?

    public List<Room> queuedRooms;
    public Room queuedRoom;

    // reference points
    public int startX;
    public int startY;

    public int bossX;
    public int bossY;

    public int centerPointX;
    public int centerPointY;

    public int currentX;
    public int currentY;

    public int roomGenX;
    public int roomGenY;

    public int minTunnelLength;
    public int maxTunnelLength;

    public int minDeadEnds;
    public int maxDeadEnds;

    public int minTunnels;
    public int maxTunnels;

    public int minRooms;
    public int maxRooms;

    public bool hasRequestedLoad;
    public bool updateRequired;

    public bool generatedDungeon;
    public bool allowedToContinue;
    public bool completedGeneration;

    // lazy approach, but we can just specify dungeon tag types here
    // and determine what is queue with each type
    // every time this is method is called, we have to clear and reset everything
    public void LoadQueueFromDungeonTag(string tag)
    {
        // clear and reset everything
        ClearMap();
        Reset();
        state = DungeonState.QueueProcess;

        currentDungeonTag = tag;
        
        AddStartRoomToQueue();


        Debug.Log("Now iterating through the current dungeon tag set...");
        switch (tag)
        {
            case "simple":
            default:

                Debug.Log("Now preparing dungeon tunnel parameters...");
                SetDungeonTunnelParams(4, 12, 1, 3, 3, 3, 2, 3);
                Debug.Log("Now adding barrier border to queue...");
                AddBarrierBorder(1);
                Debug.Log("Now adding barrier corridors to queue...");
                AddBarrierCorridors(2, 1, 1);
                Debug.Log("Now adding boss room to queue...");
                AddBossRoomToQueue();
                break;
        }

        hasRequestedLoad = true;
    }

    // builder functions
    public void AddBarrier(int x, int y, bool replaceExistingRoom = false)
    {
        if (!IsInMapRange(x, y))
        {
            return;
        }

        if (GetRoomType(x, y) == RoomType.Empty || replaceExistingRoom)
        {
            AddRoom(RoomType.Barrier, x, y, false);
        }
    }

    public void AddAdjacentBarriers(int x, int y, bool replaceExistingRooms = false)
    {
        AddBarrier(x - 1, y, replaceExistingRooms);
        AddBarrier(x + 1, y, replaceExistingRooms);
        AddBarrier(x, y - 1, replaceExistingRooms);
        AddBarrier(x, y + 1, replaceExistingRooms);
    }

    public void AddBarrierCorridors(int offset, int gap, float chance = 1)
    {
        currentMap.ForEachValue((Room r, int x, int y) =>
        {
            if ((x % gap) == offset && (y % gap) == offset && RNG.Chance(chance))
            {
                AddBarrier(x, y, true);
            }
        });
    }

    public void AddBarrierBorder(int thickness)
    {
        currentMap.ForEachValue((Room r, int x, int y) =>
        {
            if (!(x == Mathf.Clamp(x, thickness, currentMap.Width - (1 + thickness)) && y == Mathf.Clamp(y, thickness, currentMap.Height - (1 + thickness))))
            {
                AddBarrier(x, y, true);
            }
        });
    }

    public void AddRoomToQueue(Room room, int priority, bool canUseTunnels, bool blockAdjacentRooms)
    {
        if (room != null)
        {
            room.canUseTunnels = canUseTunnels;
            room.blockAdjacentRooms = blockAdjacentRooms;
        }

        AddRoomToQueue(room, priority);
    }

    public void AddRoomToQueue(Room room, int priority)
    {
        // add room to queue
        room.queuePriority = priority - queuedRooms.Count;
        queuedRooms.Add(room);
    }

    private void AddSpecialRoomToQueue(RoomType type, int priority, bool canUseTunnels, float maxTunnelScale, float maxTunnelLengthScale, int maxConnections, bool blockAdjacentRooms)
    {
        Room connectedRoom;

        Room room = new Room
        {
            type = type,
            defaultType = RoomType.Main,
            canUseTunnels = canUseTunnels,
            blockAdjacentRooms = blockAdjacentRooms
        };

        // create a constructor with the specified types for ease of use

        int startX = GetMapOriginX();
        int startY = GetMapOriginY();
        bool findConnection = false;
        int minConnectionCount = 0;
        int maxConnectionCount = 1;

        if (maxConnections > 0)
        {
            findConnection = true;
            maxConnectionCount = maxConnections;
        }

        if (findConnection)
        {
            List<Room> availableRooms = new List<Room>();

            while (availableRooms.Count <= 0)
            {
                if (maxConnectionCount >= 4)
                {
                    Debug.LogWarning("[Manager] Unable to find any rooms with available connections...");
                    break;
                    // can't find any connections here?
                }

                availableRooms = GetRoomsWithAvailableConnections(minConnectionCount, maxConnectionCount);
                maxConnectionCount++;
            }

            connectedRoom = RNG.Choose(availableRooms);
            startX = connectedRoom.x;
            startY = connectedRoom.y;
        }

        room.canUseTunnels = true;
        room.minTunnels = minTunnels;
        room.maxTunnels = (int)(maxTunnels * maxTunnelScale);
        room.minTunnelLength = minTunnelLength;
        room.maxTunnelLength = (int)(maxTunnelLength * maxTunnelLengthScale);
        room.startX = startX;
        room.startY = startY;

        // add resulting screen to queue
        AddRoomToQueue(room, priority);
    }

    public int GetMapOriginX()
    {
        return currentMap.Width / 2;
    }

    public int GetMapOriginY()
    {
        return currentMap.Height / 2;
    }


    public void ClearMap()
    {
        currentMap.SetEachValue(() => { return new Room(); });
        currentMap.ForEachValue((Room r, int x, int y) => {   r.x = x; r.y = y; /* Debug.Log($"[Room] New room initialized at X: {x}, Y: {y}"); */ });
    }

    public bool IsInMapRange(int x, int y)
    {
        return x <= currentMap.Width - 1 && y <= currentMap.Height - 1;
    }


    public Room GetRoom(int x, int y)
    {
        if (!IsInMapRange(x, y))
        {
            return null;
        }

        return currentMap.GetValue(x, y);
    }

    public RoomType GetRoomType(int x, int y)
    {
        Room target = GetRoom(x, y);

        if (target == null)
            return RoomType.Empty;

        return target.type;
    }

    public RoomType GetCurrentRoomType()
    {
        return GetRoomType(currentX, currentY);
    }

    public bool RoomExists(int x, int y)
    {
        return GetRoomType(x, y) > RoomType.Empty;
    }



    public int GetAdjacentRoomCount(int x, int y, bool ignoreRoomType = false)
    {
        int count = 0;

        // Check around the specified position for adjacent rooms
        for (int ax = -1; ax <= 1; ax++)
        {
            for (int ay = -1; ay <= 1; ay++)
            {
                int cx = x + ax;
                int cy = y + ay;

                if (IsInMapRange(cx, cy) && !(ax == 0 && ay == 0))
                {
                    RoomType type = GetRoomType(cx, cy);

                    if (type == RoomType.Empty)
                    {
                        continue;
                    }

                    if (ignoreRoomType || type == RoomType.Start || type == RoomType.Main)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    public int GetConnectionCount(int x, int y)
    {
        int count = 0;

        if (RoomExists(x + 1, y)) count++;
        if (RoomExists(x - 1, y)) count++;
        if (RoomExists(x, y + 1)) count++;
        if (RoomExists(x, y - 1)) count++;

        return count;
    }

    public List<Room> GetRoomsWithAvailableConnections(int minConnectionsAllowed = 0, int maxConnectionsAllowed = 4, bool ignoreRoomType = false)
    {
        List<Room> availableRooms = new List<Room>();

        currentMap.ForEachValue((room, x, y) =>
        {
            if (room == null)
            {
                Debug.Log("[Manager] Room does not exist, continuing...");
                return;
            }

            if (!room.allowNewConnections)
            {
                // Debug.Log($"[Manager] Room ({room.type} at X: {room.x}, Y: {room.y}) does not allow new connections, continuing...");
                return;
            }

            if (ignoreRoomType || room.type == RoomType.Start || room.type == RoomType.Main)
            {
                int connectionCount = GetConnectionCount(room.x, room.y);

                // Debug.Log($"[Manager] Room has {connectionCount} occupied connections...");

                if (connectionCount == Mathf.Clamp(connectionCount, minConnectionsAllowed, maxConnectionsAllowed))
                {
                    Debug.Log($"[Manager] Found a room ({room.type} at X: {room.x} Y: {room.y}) with {connectionCount} occupied connection(s)!");
                    availableRooms.Add(room);
                }
            }
        });

        return availableRooms;
    }

    public RoomType GetNearestRoomType(RoomDirection direction)
    {
        int checkX = currentX + Calc.GetXFromAngle((int) direction);
        int checkY = currentY + Calc.GetYFromAngle((int)direction);

        if (IsInMapRange(checkX, checkY))
        {
            return GetRoomType(checkX, checkY);
        }

        return RoomType.Empty;
    }

    public void SetRoom(Room room, int x, int y)
    {
        currentMap.SetValue(room, x, y);
        Debug.Log($"[Map] Set room of type {room.type} at X:{room.x} Y:{room.y}");
    }

    public void AddRoom(RoomType type, int x, int y, bool canUseTunnels, string tag = null)
    {
        if (!IsInMapRange(x, y))
        {
            Debug.LogWarning("[Room] Position of room out of range, ignoring action...");
            // Position of room out of range, ignoring action
            return;
        }

        Room room = new Room(type, x, y, canUseTunnels, tag);

        room.startX = GetMapOriginX();
        room.startY = GetMapOriginY();

        currentMap.SetValue(room, x, y);
        Debug.Log($"[Map] Added room ({room.type} at X:{room.x} Y:{room.y})");

        if (completedGeneration)
        {
            Debug.Log("[Room] Placing room as game object since dungeon generation is finished...");
            // room.AddRoomToCurrentScene();
        }
    }

    public string GetMapAsString()
    {
        StringBuilder m = new StringBuilder();

        m.AppendLine("Dungeon Map Preview\n");

        for (int x = 0; x < currentMap.Width; x++)
        {
            for (int y = 0; y < currentMap.Height; y++)
            {
                Room r = GetRoom(x, y);

                if (r == null || r.type == RoomType.Empty)
                {
                    m.Append(" ");
                    continue;
                }

                switch (r.type)
                {
                    case RoomType.Barrier:
                        m.Append("X");
                        break;
                    case RoomType.Main:
                        m.Append("O");
                        break;
                    case RoomType.Start:
                        m.Append("S");
                        break;
                    case RoomType.Boss:
                        m.Append("B");
                        break;
                }
            }

            m.AppendLine();
        }

        return m.ToString();
    }

    // This function is called to set all tunnel parameters for dungeon generation
    public void Update()
    {
        // this is just for the test, there could be a better way to do this
        if (!hasRequestedLoad || !updateRequired)
        {
            return;
        }

        switch (state)
        {
            case DungeonState.QueueProcess:
                ProcessRoomQueue();
                break;
            case DungeonState.TunnelGenerate:
                GenerateTunnel();
                break;
            case DungeonState.RoomGenerate:
                GenerateRoom();
                break;
            case DungeonState.DungeonStart:
                StartDungeon();
                break;
            case DungeonState.DungeonActive:
                updateRequired = false;
                Debug.Log("Dungeon has finished generating...");
                Debug.Log(GetMapAsString());
                break;
        }
    }

    // Reset this dungeon manager to not have any queued rooms
    public void Reset()
    {
        queuedRooms.Clear();
    }

    private void ProcessRoomQueue()
    {
        int roomCount = GetRoomCount(RoomType.Main);
        if (queuedRooms.Count <= 0 || roomCount > maxRooms)
        {
            if (queuedRooms.Count <= 0)
            {
                Debug.Log("Queue is empty, continuing...");
                // Queue is empty, continuing...
            }

            Debug.Log($"[Process] {roomCount} rooms in queue, {minRooms}-{maxRooms} rooms required...");

            // roomCount rooms in queue
            // minRooms-maxRooms rooms required

            if (roomCount > maxRooms)
            {
                // Too many rooms, restarting dungeon generation...
                Debug.Log("[Process] Too many rooms, restarting dungeon generation...");
                LoadQueueFromDungeonTag(currentDungeonTag);
                return;
            }
            else if (roomCount < minRooms)
            {
                Debug.Log("[Process] Not enough rooms, adding dead end...");
                float mult = 0.5f;
                AddSpecialRoomToQueue(RoomType.Main, 1000, true, mult, mult, 3, false);
            }
            else
            {
                Debug.Log("[Process] All rooms queued, now generating tunnels...");
                state = DungeonState.RoomGenerate;
            }
        }
        else
        {
            queuedRoom = queuedRooms.OrderByDescending(x => x.queuePriority).First();

            if (queuedRoom != null)
            {
                // Room in queue found, moving to tunnel generation
                Debug.Log($"[Process] Room ({queuedRoom.type} at X:{queuedRoom.x} Y:{queuedRoom.y}) in queue found, moving to tunnel generation...");
                queuedRoom.CreateTunnelBuilder();
                state = DungeonState.TunnelGenerate;
            }
            else
            {
                // Room in queue does not exist, going back to queue
                Debug.Log("[Process] Room in queue does not exist, going back to queue...");
                state = DungeonState.QueueProcess;
            }

            // Remove room from queue
            queuedRooms.Remove(queuedRoom);
        }
    }

    private void GenerateTunnel()
    {
        if  (queuedRoom == null)
        {
            // Room in queue no longer exists, returning to queue
            Debug.Log("Room in queue no longer exists, returning...");
            state = DungeonState.QueueProcess;
            return;
        }

        TunnelBuilder tunnel = queuedRoom.tunnel;

        if (tunnel == null)
        {
            // Tunnel does not exist for room, returning to queue
            Debug.Log("Tunnel does not exist for room, returning to queue...");
            queuedRoom = null;
            state = DungeonState.QueueProcess;
            return;
        }

        if (tunnel.complete)
        {
            bool failed = tunnel.failed;
            tunnel = null;
            queuedRoom.tunnel = null;

            if (failed)
            {
                // Tunnel failed generation
                Debug.Log("Tunnel failed generation...");
                queuedRoom = null;
            }
        }
        else
        {
            tunnel.Update();
        }
    }

    private void GenerateRoom()
    {
        if (IsInMapRange(roomGenX, roomGenY))
        {
            Room target = GetRoom(roomGenX, roomGenY);
            if (target != null && target.type > 0)
            {
                target.AddRoomToCurrentScene();
            }
        }

        roomGenX++;

        if (roomGenX >= currentMap.Width)
        {
            roomGenX = 0;
            roomGenY++;

            if (roomGenY >= currentMap.Height)
            {

                // All rooms generated. Now starting dungeon
                Debug.Log("All rooms generated. Now starting dungeon...");
                roomGenY = 0;
                state = DungeonState.DungeonStart;
            }
        }
    }

    private void StartDungeon()
    {
        if (allowedToContinue)
        {
            completedGeneration = true;
            Debug.Log("Prepared everything for the dungeon, now moving to active updates...");
            state = DungeonState.DungeonActive;
        }
    }


    public int GetRoomCount()
    {
        int count = 0;
        
        currentMap.ForEachValue(x =>
        {
            if (x != null && x.type > RoomType.Empty)
            {
                count++;
            }
        });

        return count;
    }

    public int GetRoomCount(RoomType type)
    {
        int count = 0;

        currentMap.ForEachValue(x =>
        {
            if (x != null && x.type == type)
            {
                count++;
            }
        });

        return count;
    }



















    // queue functions

    private void SetDungeonTunnelParams(int minimumRooms, int maximumRooms, int minimumTunnels, int maximumTunnels, int minimumTunnelLength, int maximumTunnelLength, int minimumDeadEnds, int maximumDeadEnds, bool addDeadEndsToQueue = true)
    {
        minRooms = Mathf.Max(1, minimumRooms);
        maxRooms = Mathf.Max(1, maximumRooms);
        minTunnels = Mathf.Max(1, minimumTunnels);
        maxTunnels = Mathf.Max(1, maximumTunnels);
        minTunnelLength = Mathf.Max(1, minimumTunnelLength);
        maxTunnelLength = Mathf.Max(1, maximumTunnelLength);
        minDeadEnds = Mathf.Max(1, minimumDeadEnds);
        maxDeadEnds = Mathf.Max(1, maximumDeadEnds);

        if (addDeadEndsToQueue)
        {
            AddDeadEndsToQueue(minDeadEnds, maxDeadEnds);
        }
    }

    private void AddStartRoomToQueue()
    {
        Debug.Log("[Queue] Adding starting room to queue...");
        AddRoom(RoomType.Start, GetMapOriginX(), GetMapOriginY(), false);
    }

    private void AddDeadEndToQueue()
    {
        AddSpecialRoomToQueue(RoomType.Main, 1010, true, 1, 1, 3, false);
    }

    private void AddDeadEndsToQueue(int min, int max)
    {
        for (int i = 0; i < Random.Range(min, max); i++)
        {
            Debug.Log("Adding new dead end...");
            AddDeadEndToQueue();
        }
    }

    private void AddBossRoomToQueue()
    {
        AddSpecialRoomToQueue(RoomType.Boss, 1000, true, 1, 2, 3, true);
    }
}
