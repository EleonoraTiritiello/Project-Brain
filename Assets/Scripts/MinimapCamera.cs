using UnityEngine;
using redd096;

public class MinimapCamera : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] string minimapLayer = "Minimap";

    void Awake()
    {
        //remove minimap layer from main camera
        Camera cam = Camera.main;
        cam.cullingMask = LayerUtility.RemoveLayer(cam.cullingMask, LayerUtility.NameToLayer(minimapLayer));

        //set layer to this camera
        GetComponent<Camera>().cullingMask = CreateLayer.LayerOnly(minimapLayer);
    }

    void LateUpdate()
    {
        if (GameManager.instance.levelManager.currentRoom)
        {
            //current room position + up 
            transform.position = GameManager.instance.levelManager.currentRoom.transform.position + Vector3.up * 20;
        }
    }
}
