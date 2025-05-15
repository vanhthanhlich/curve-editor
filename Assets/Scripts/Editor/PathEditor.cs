using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    private PathCreator creator;
    private Pathh path;

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

            Handles.color = Color.black;
            Handles.DrawLine(segment[0], segment[1]);
            Handles.DrawLine(segment[2], segment[3]);

            Handles.color = Color.green;
            Handles.DrawBezier(segment[0], segment[3], segment[1], segment[2], Color.green, null, 2f);
        }

        for(int i = 0; i < path.numPoints; i++)
        {
            Handles.color = Color.red;
            Vector2 new_pos = Handles.FreeMoveHandle(path[i], 0.08f, Vector2.zero, Handles.CylinderHandleCap);
           
            if(new_pos != path[i])
            {
                Undo.RecordObject(creator, "move point");
                path.MovePoint(i, new_pos, creator.LockAnchor);
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