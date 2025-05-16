using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    private PathCreator creator;
    private Pathh path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        if(GUILayout.Button("Create new path"))
        {
            Undo.RecordObject(creator, "create new path");
            creator.CreatePath();
            path = creator.path;
        }


        if(path.isClosed != GUILayout.Toggle(path.isClosed, "Enable closed mode"))
        {
            Undo.RecordObject(creator, "enable closed mode");
            path.ToggleClosedMode();
        }

        if(path.autoSetControlPointMode != GUILayout.Toggle(path.autoSetControlPointMode, "Enable auto set control point mode"))
        {
            Undo.RecordObject(creator, "enable auto set control point mode");
            path.ToggleAutoSetMode();
        }

        if(EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }

    }
    private void OnSceneGUI()
    {
        Inp();
        Draw();
    }

    private void Inp()
    {
        Event guiEvent = Event.current;
        if(guiEvent.type == EventType.MouseDown &&  guiEvent.button == 0 && guiEvent.shift)
        {
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            Undo.RecordObject(creator, "add point");
            path.AddSegment(mousePos);
        }
    }

    private void Draw()
    {
        for(int i = 0; i < path.numSegments; i++)
        {
            Vector2[] segment = path.GetPointsInSegment(i);

            if(!path.autoSetControlPointMode)
            {
                Handles.color = Color.black;
                Handles.DrawLine(segment[0], segment[1], 2.5f);
                Handles.DrawLine(segment[2], segment[3], 2.5f);
            }

            Handles.color = Color.green;
            Handles.DrawBezier(segment[0], segment[3], segment[1], segment[2], Color.green, null, 3.5f);
        }

        for(int i = 0; i < path.numPoints; i++)
        {
            if (path.autoSetControlPointMode && i % 3 != 0) continue;

            Handles.color = Color.red;
            Vector2 new_pos = Handles.FreeMoveHandle(path[i], 0.08f, Vector2.zero, Handles.CylinderHandleCap);
           
            if(new_pos != path[i])
            {
                Undo.RecordObject(creator, "move point");
                path.MovePoint(i, new_pos);
            }
        }

    }

    private void OnEnable()
    {
        creator = (PathCreator)target;
        creator.CreatePath();
        path = creator.path;
    }


}