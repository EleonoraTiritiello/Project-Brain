using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private MapManager mapManager;

    private void OnEnable()
    {
        mapManager = target as MapManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Regen Map - DOESN'T WORK WITH FIXED ROOMS"))
        {
            mapManager.DestroyMap();
            mapManager.CreateNewMap();

            //set undo
            Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
        }

        if (GUILayout.Button("Destroy Map"))
        {
            mapManager.DestroyMap();

            //set undo
            Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");
        }
    }
}