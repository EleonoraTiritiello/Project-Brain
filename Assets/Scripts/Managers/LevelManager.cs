using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Rope")]
    public LineRenderer RopePrefab = default;
    public RopeColliderInteraction ColliderPrefab = default;
    public float RopeLength = 12;
}
