using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    up, down, left, right
}

[System.Serializable]
public struct DoorStruct
{
    public Transform door;
    public CardinalDirection direction;
    public bool isOnlyExit;
}

[AddComponentMenu("Project Brain/Room")]
[SelectionBase]
public abstract class Room : MonoBehaviour
{
    #region variables

    [Header("2D or 3D")]
    [SerializeField] bool is3D = true;

    [Header("Important")]
    [Tooltip("Size of every tile which compose this room")] [SerializeField] float tileSize = 1f;
    [Tooltip("Int because the size will be exspressed in tiles")] [SerializeField] int width = 1;
    [Tooltip("Int because the size will be exspressed in tiles")] [SerializeField] int height = 1;
    [SerializeField] protected List<DoorStruct> doors = new List<DoorStruct>();

    float HalfWidth => width * tileSize * 0.5f;
    float HalfHeight => height * tileSize * 0.5f;
    Vector2 UpRight => is3D ? new Vector3(transform.position.x + HalfWidth, transform.position.z + HalfHeight) : new Vector3(transform.position.x + HalfWidth, transform.position.y + HalfHeight);
    Vector2 DownLeft => is3D ? new Vector3(transform.position.x - HalfWidth, transform.position.z - HalfHeight) : new Vector3(transform.position.x - HalfWidth, transform.position.y - HalfHeight);

    [Header("DEBUG")]
    public bool showDebug = false;
    [CanShow("showDebug")] [SerializeField] TextMesh textID = default;
    [CanShow("showDebug")] [SerializeField] protected int id = 0;
    [CanShow("showDebug")] [SerializeField] protected bool teleported = false;

    [CanShow("showDebug")] [SerializeField] DoorStruct adjacentDoor = default;
    [CanShow("showDebug")] [SerializeField] Room adjacentRoom = default;
    [CanShow("showDebug")] [SerializeField] DoorStruct entranceDoor = default;
    [CanShow("showDebug")] [SerializeField] protected List<DoorStruct> usedDoors = new List<DoorStruct>();

    #endregion

    public abstract void CompleteRoom();

    #region public API

    public void SetPosition(Vector3 position)
    {
        //set position
        transform.position = position;
    }

    public bool SetPosition(DoorStruct adjacentDoor, Room adjacentRoom)
    {
        //set references
        this.adjacentDoor = adjacentDoor;
        this.adjacentRoom = adjacentRoom;

        if (adjacentRoom == null || adjacentDoor.door == null)
            Debug.Log("<color=red>Houston, abbiamo un problema</color>");

        //check this room has a door to adjacent room, and adjust position
        if (CheckDoors() == false)
            return false;

        return true;
    }

    public DoorStruct GetRandomDoor()
    {
        //return random door
        return doors[Random.Range(0, doors.Count)];
    }

    public bool CheckCanPlace(List<Room> rooms)
    {
        //foreach room
        foreach (Room room in rooms)
        {
            //check if there is a room inside this one
            if (PointsInsideRoom(room))
                return false;

            //check if this one is inside a room
            if (room.PointsInsideRoom(this))
                return false;
        }

        return true;
    }

    public void Register(int id, bool teleported)
    {
        this.id = id;
        this.teleported = teleported;

        //debug
        if (textID)
        {
            textID.text = teleported ? "tp: " + id.ToString() : id.ToString();
        }
        else
        {
            Debug.Log("La room " + name + " non ha un Text per mostrare il suo ID in scena");
        }

        //set entrance and exit doors used
        if (adjacentRoom)
        {
            usedDoors.Add(entranceDoor);
            adjacentRoom.usedDoors.Add(adjacentDoor);
        }

        //random color
        float h = Random.value;
        Color color = Color.HSVToRGB(h, 0.8f, 0.8f);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
            color = Color.HSVToRGB(h, 1f, 1f);
        }
    }

    #endregion

    #region private API

    bool CheckDoors()
    {
        //if moving left, check doors on right, else check doors on left
        if (adjacentDoor.direction == CardinalDirection.left || adjacentDoor.direction == CardinalDirection.right)
        {
            entranceDoor.direction = adjacentDoor.direction == CardinalDirection.left ? CardinalDirection.right : CardinalDirection.left;
        }
        //if moving up, check doors on bottom, else check doors on top
        else
        {
            entranceDoor.direction = adjacentDoor.direction == CardinalDirection.up ? CardinalDirection.down : CardinalDirection.up;
        }

        List<DoorStruct> possibleDoors = new List<DoorStruct>();

        //add every possible door (using direction setted before)
        foreach (DoorStruct possibleDoor in doors)
        {
            if (possibleDoor.direction == entranceDoor.direction
                && possibleDoor.isOnlyExit == false)                //be sure is not OnlyExit, because this one will be an entrance to this room
            {
                possibleDoors.Add(possibleDoor);
            }
        }

        //if no possible doors, return
        if (possibleDoors.Count <= 0)
            return false;

        //else get a random door between possibles
        entranceDoor = possibleDoors[Random.Range(0, possibleDoors.Count)];

        //calculate distance and move
        Vector3 fromDoorToAdjacentDoor = adjacentDoor.door.position - entranceDoor.door.position;
        transform.position += fromDoorToAdjacentDoor;

        return true;
    }

    bool PointsInsideRoom(Room roomToCheck)
    {
        //get every tile from downleft (<= so check also DownRight, UpLeft, UpRight)
        for (int x = 0; x <= roomToCheck.width; x++)
        {
            for (int y = 0; y <= roomToCheck.height; y++)
            {
                //direction right up, if reached limit go backward
                Vector2 directionGap = new Vector2(x >= roomToCheck.width ? -1 : 1, y >= roomToCheck.height ? -1 : 1);

                Vector2 point = roomToCheck.DownLeft + (Vector2.right * x * roomToCheck.tileSize) + (Vector2.up * y * roomToCheck.tileSize)     //down left of every tile
                    + (directionGap * roomToCheck.tileSize * 0.1f);                                                                             //little gap to no have half room inside another

                //check is inside this room
                if (point.x > DownLeft.x && point.x < UpRight.x)
                {
                    if (point.y > DownLeft.y && point.y < UpRight.y)
                    {
                        return true;
                    }
                }
            }
        }

        //check also center if inside this room
        Vector2 center = is3D ? new Vector2(roomToCheck.transform.position.x, roomToCheck.transform.position.z) : new Vector2(roomToCheck.transform.position.x, roomToCheck.transform.position.y);
        if (center.x > DownLeft.x && center.x < UpRight.x)
        {
            if (center.y > DownLeft.y && center.y < UpRight.y)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
