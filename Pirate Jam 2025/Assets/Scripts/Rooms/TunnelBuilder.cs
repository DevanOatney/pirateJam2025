using System.Collections.Generic;
using UnityEngine;

public class TunnelBuilder
{
    public TunnelBuilder(int x, int y, RoomType type, string tag = null, Room target = null)
    {
        currentX = x;
        currentY = y;
        defaultType = type;
        defaultTag = tag;
        minLength = 2;
        maxLength = 3;
        currentLength = 0;
        currentDirection = RNG.ChooseFrom(0, 90, 180, 270);
        complete = false;
        finalRoom = target;
    }

    public static readonly int MaxConsecutiveFails = 25;
    public static readonly int MaxFailsBeforeError = 100;

    // default info for tunnel generation
    public RoomType defaultType;
    public string defaultTag;

    public int currentTunnelCount;
    public int currentLength;
    public int currentDirection;

    public int currentX;
    public int currentY;
    
    public int maxLength;
    public int minLength;

    public Room finalRoom;

    public bool complete;
    public List<Room> failedRooms;
    public int consecutiveFails;
    public bool failed;
    public bool createDefaultRooms;
    
    public void Update()
    {
        // Get the current map
        Grid<Room> m = DungeonManager.Instance.currentMap;

        if (m == null)
        {
            // There is no available map to reference, return
            return;
        }

        // Do not update if tunnel has completed
        if (complete)
        {
            return;
        }

        createDefaultRooms = true;

        if (currentLength <= 0)
        {
            // generate random parameters
            currentLength = Random.Range(minLength, maxLength);
            
            currentDirection += RNG.ChooseFrom(-90, 90);
            currentDirection = Calc.CorrectAngle(currentDirection);
        }

        int h = Calc.GetXFromAngle(currentDirection);
        int v = Calc.GetYFromAngle(currentDirection);

        // Check if new location is empty and is within map range
        if (DungeonManager.Instance.IsInMapRange(currentX + h, currentY + v) && DungeonManager.Instance.GetRoomType(currentX + h, currentY + v) == RoomType.Empty)
        {
            currentX += h;
            currentY += v;
            currentLength--;

            // Check if tunnel length has been emptied
            if (currentLength <= 0)
            {
                currentTunnelCount--;

                if (currentTunnelCount <= 0 && DungeonManager.Instance.GetConnectionCount(currentX, currentY) == 1)
                {
                    // If there is a final room specified, replace the default room
                    if (finalRoom != null)
                    {
                        Debug.Log($"[Tunnel] Final room specified, now placing...");

                        createDefaultRooms = false;

                        // Set new info
                        finalRoom.x = currentX;
                        finalRoom.y = currentY;

                        // Add to map
                        DungeonManager.Instance.SetRoom(finalRoom, currentX, currentY);

                        if (finalRoom.blockAdjacentRooms)
                        {
                            Debug.Log($"[Tunnel] Final room is blocking adjacent rooms...");
                            DungeonManager.Instance.AddAdjacentBarriers(finalRoom.x, finalRoom.y);
                        }
                    }

                    complete = true;
                }
            }

            if (createDefaultRooms)
            {
                consecutiveFails = 0;
                DungeonManager.Instance.AddRoom(defaultType, currentX, currentY, false, defaultTag);
            }
        }
        else
        {
            // Otherwise, this tunnel failed to find a suitable position
            currentLength = 0;
            consecutiveFails++;
            Debug.Log($"[Tunnel] This tunnel was unable to find a suitable position to place a room ({consecutiveFails} consecutive fail(s))");

            if (consecutiveFails >= MaxConsecutiveFails)
            {
                // If we hit the maximum fail count, allow some room types to add new connections if needed

                failedRooms = DungeonManager.Instance.GetRoomsWithAvailableConnections(1, 3, true);

                // If there are any failed rooms, randomly choose one and use the new position
                if (failedRooms.Count > 0)
                {
                    Debug.Log($"[Tunnel] There are {failedRooms.Count} rooms with available connections");
                    Room newTarget = RNG.Choose(failedRooms);
                    Debug.Log($"[Tunnel] Selected random room ({newTarget.type} at X:{newTarget.x} Y: {newTarget.y}) as new position of tunnel");
                    currentX = newTarget.x;
                    currentY = newTarget.y;
                }
                else
                {
                    Debug.Log($"[Tunnel] There are no more rooms with available connections...");
                    // There are no rooms with any available connections
                }

                if (consecutiveFails >= MaxFailsBeforeError)
                {
                    // If there's a final room that isn't required, we can still pass this tunnel generation
                    if (finalRoom != null && !finalRoom.required)
                    {
                        complete = true;
                        failed = true;
                        Debug.Log("[Tunnel] The final room is missing, but it isn't required");
                    }
                    else
                    {
                        // Too many errors have occurred while attempting to generate this tunnel
                        Debug.LogWarning("[Tunnel] Too many errors have occurred while attempting to build this tunnel");
                        Debug.LogWarning(DungeonManager.Instance.GetMapAsString());
                    }
                }
            }
        }
    }
    
}
