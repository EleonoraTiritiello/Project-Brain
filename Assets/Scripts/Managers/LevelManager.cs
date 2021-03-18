using UnityEngine;
using redd096;

public class LevelManager : MonoBehaviour
{
    [Header("Rope")]
    public LineRenderer RopePrefab = default;
    public RopeColliderInteraction ColliderPrefab = default;
    public float RopeLength = 12;

    [Header("Minimap")]
    [SerializeField] GameObject MinimapIcon = default;

    [Header("Debug")]
    [ReadOnly] public RoomGame currentRoom;

    Transform minimapIconsParent;

    void Awake()
    {
        //create parent icons
        minimapIconsParent = new GameObject("Minimap Icons").transform;
    }

    public GameObject CreateIcon(Vector3 position, float tileSize, int width, int height)
    {
        //instantiate minimap icon
        GameObject minimapIcon = Instantiate(GameManager.instance.levelManager.MinimapIcon, minimapIconsParent);
        minimapIcon.transform.position = position;
        minimapIcon.transform.localScale = new Vector3(width * tileSize, height * tileSize, 1);

        //deactive by default
        minimapIcon.SetActive(false);

        return minimapIcon;
    }
}
