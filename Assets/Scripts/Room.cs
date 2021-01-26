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
}

[AddComponentMenu("Project Brain/Room")]
[SelectionBase]
public class Room : MonoBehaviour
{
    #region variables

    [Header("Important")]
    [SerializeField] float tileSize = 1f;   //size of every tile which compose this room
    [SerializeField] int width = 1;         //int because the size will be exspressed in tiles
    [SerializeField] int height = 1;        //int because the size will be exspressed in tiles
    [SerializeField] List<DoorStruct> doors = new List<DoorStruct>();

    float HalfWidth => width * tileSize * 0.5f;
    float HalfHeight => height * tileSize * 0.5f;
    Vector3 UpRight => new Vector3(transform.position.x + HalfWidth, transform.position.y + HalfHeight, 0);
    Vector3 DownLeft => new Vector3(transform.position.x - HalfWidth, transform.position.y - HalfHeight, 0);

    [Header("DEBUG")]
    [SerializeField] TextMesh textID = default;
    [SerializeField] int id = 0;

    [Header("DEBUG adjancent room (necessary not public)")]
    [SerializeField] DoorStruct adjacentDoor = default;
    [SerializeField] Room adjacentRoom = default;
    [SerializeField] DoorStruct door = default;

    #endregion

    #region public API

    public void Init(int id, bool teleported)
    {
        this.id = id;

        //debug
        if (textID)
        {
            textID.text = teleported ? "tp: " + id.ToString() : id.ToString();
        }
        else
        {
            Debug.Log("La room " + name + " non ha un Text per mostrare il suo ID in scena");
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

    #endregion

    #region private API

    bool CheckDoors()
    {
        //if moving left, check doors on right, else check doors on left
        if (adjacentDoor.direction == CardinalDirection.left || adjacentDoor.direction == CardinalDirection.right)
        {
            door.direction = adjacentDoor.direction == CardinalDirection.left ? CardinalDirection.right : CardinalDirection.left;
        }
        //if moving up, check doors on bottom, else check doors on top
        else
        {
            door.direction = adjacentDoor.direction == CardinalDirection.up ? CardinalDirection.down : CardinalDirection.up;
        }

        List<DoorStruct> possibleDoors = new List<DoorStruct>();

        //add every possible door (on left or right side)
        foreach (DoorStruct d in doors)
        {
            if (d.direction == door.direction)
            {
                possibleDoors.Add(d);
            }
        }

        //if no possible doors, return
        if (possibleDoors.Count <= 0)
            return false;

        //else get a random door between possibles
        door.door = possibleDoors[Random.Range(0, possibleDoors.Count)].door;

        //calculate distance and move
        Vector3 fromDoorToAdjacentDoor = adjacentDoor.door.position - door.door.position;
        transform.position += fromDoorToAdjacentDoor;

        return true;
    }

    bool OldCheckDoors()
    {
        List<DoorStruct> possibleDoors = new List<DoorStruct>();
        Vector3 newPosition = Vector3.zero;

        //moving left or right
        if (adjacentDoor.direction == CardinalDirection.left || adjacentDoor.direction == CardinalDirection.right)
        {
            //if moving left, check doors on right, else check doors on left
            door.direction = adjacentDoor.direction == CardinalDirection.left ? CardinalDirection.right : CardinalDirection.left;

            //add every possible door (on left or right side)
            foreach (DoorStruct v in doors)
            {
                if (v.direction == door.direction)
                {
                    possibleDoors.Add(v);
                }
            }

            //if no possible doors, return
            if (possibleDoors.Count <= 0)
                return false;

            //get a random door between possibles
            door.door = possibleDoors[Random.Range(0, possibleDoors.Count)].door;

            //move right or left to adjacent room
            Vector3 shift = adjacentDoor.direction == CardinalDirection.left ? Vector3.left * (adjacentRoom.HalfWidth + HalfWidth) : Vector3.right * (adjacentRoom.HalfWidth + HalfWidth);
            newPosition = adjacentRoom.transform.position + shift;

            //move to adjacent door
            newPosition.y = adjacentDoor.door.position.y;

            //now align our door, not center of the room
            float doorYPosition = door.door.position.y;
            float fromDoorToAdjacentDoor = newPosition.y - doorYPosition;

            //move from center by distance found to align doors
            newPosition.y = transform.position.y + fromDoorToAdjacentDoor;
        }
        //moving up or down
        else
        {
            //if moving down, check doors on top, else check doors on bottom
            door.direction = adjacentDoor.direction == CardinalDirection.down ? CardinalDirection.up : CardinalDirection.down;

            //add every possible door (on top or bottom side)
            foreach (DoorStruct v in doors)
            {
                if (v.direction == door.direction)
                {
                    possibleDoors.Add(v);
                }
            }

            //if no possible doors, return
            if (possibleDoors.Count <= 0)
                return false;

            //get a random door between possibles
            door.door = possibleDoors[Random.Range(0, possibleDoors.Count)].door;

            //move up or down to adjacent room
            Vector3 shift = adjacentDoor.direction == CardinalDirection.down ? Vector3.down * (adjacentRoom.HalfHeight + HalfHeight) : Vector3.up * (adjacentRoom.HalfHeight + HalfHeight);
            newPosition = adjacentRoom.transform.position + shift;

            //move to adjacent door
            newPosition.x = adjacentDoor.door.position.x;

            //now align our door, not center of the room
            float doorXPosition = door.door.position.x;
            float fromDoorToAdjacentDoor = newPosition.x - doorXPosition;

            //move from center by distance found to align doors
            newPosition.x = transform.position.x + fromDoorToAdjacentDoor;
        }

        transform.position = newPosition;

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
                Vector3 directionGap = new Vector3(x >= roomToCheck.width ? -1 : 1, y >= roomToCheck.height ? -1 : 1, 0);

                Vector3 point = roomToCheck.DownLeft + (Vector3.right * x * roomToCheck.tileSize) + (Vector3.up * y * roomToCheck.tileSize)     //down left of every tile
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
        Vector3 center = roomToCheck.transform.position;
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
