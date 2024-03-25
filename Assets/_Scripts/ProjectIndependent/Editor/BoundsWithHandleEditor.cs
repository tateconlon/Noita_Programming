using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(BoundsWithHandle))]
public class MyBoundsClassEditor : Editor
{
    private BoxBoundsHandle _boundsHandle;

    private void OnSceneGUI()
    {
        BoundsWithHandle myBounds = (BoundsWithHandle)target;

        // Set up the BoxBoundsHandle with the bounds from the target object
        _boundsHandle.center = myBounds.GetCenter();
        _boundsHandle.size = myBounds.GetSize();

        // Draw the BoxBoundsHandle in the scene view
        EditorGUI.BeginChangeCheck();

        _boundsHandle.handleColor = myBounds._handleColor;
        _boundsHandle.wireframeColor = myBounds._wireframeColor;
        _boundsHandle.DrawHandle();
        
        if (EditorGUI.EndChangeCheck())
        {
            //NOTE: DO NOT UPDATE THE BOUNDS USING THE HANDLE. IT IS BROKEN.
            //ONLY USE THE INSPECTOR FIELDS AND USE HANDLE FOR VISUALIZATION
            //I'm sure there's a way but not enough time to fix for so little functionality
            //Taking into account local pos and scale is making the handle fucky.
            
            // Update the bounds of the target object when the BoxBoundsHandle is modified
            //Undo.RecordObject(myBounds, "Modify Bounds");
            //myBounds.SetLocalCenter(_boundsHandle.center - myBounds.transform.position);
            //myBounds.SetLocalSize(MultiplyComponents(_boundsHandle.size, myBounds.transform.localScale));
            //EditorUtility.SetDirty(myBounds);
        }
    }
    
    //Copy-Pasted from CodeHelpers,which isn't transferring.
    private Vector3 MultiplyComponents(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    private void OnEnable()
    {
        // Initialize the BoxBoundsHandle
        _boundsHandle = new BoxBoundsHandle();
    }
}