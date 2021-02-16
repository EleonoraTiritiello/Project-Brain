using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rigidbody))]
[CanEditMultipleObjects]
public class RigidbodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //do only if game is running
        if (Application.isPlaying == false)
            return;

        GUILayout.Space(10);

        //back forward
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Push Back"))
        {
            Push(Vector3.back);
        }

        if (GUILayout.Button("Push Forward"))
        {
            Push(Vector3.forward);
        }
        GUILayout.EndHorizontal();

        //left right
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Push Left"))
        {
            Push(Vector3.left);
        }

        if (GUILayout.Button("Push Right"))
        {
            Push(Vector3.right);
        }
        GUILayout.EndHorizontal();

        //down up
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Push Down"))
        {
            Push(Vector3.down);
        }

        if (GUILayout.Button("Push Up"))
        {
            Push(Vector3.up);
        }
        GUILayout.EndHorizontal();
    }

    void Push(Vector3 direction)
    {
        //foreach rigidbody, push in direction
        foreach (Object obj in targets)
        {
            ((Rigidbody)obj).AddForce(direction * 10, ForceMode.Impulse);
        }
    }
}
