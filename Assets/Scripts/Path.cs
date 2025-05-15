using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pathh
{
    [SerializeField, HideInInspector]
    List<Vector2> Points;

    public int numPoints { get { return Points.Count; } }
    public int numSegments { get { return Points.Count / 3; } }
    public Vector2 this[int i] {  get { return Points[i]; } }



    public Pathh(Vector2 center)
    {
        Points = new List<Vector2>()
        {
            center + Vector2.left,
            center + Vector2.left + Vector2.up * .5f,
            center + Vector2.right + Vector2.up * .5f,
            center + Vector2.right
        };
    }

    public void AddSegment(Vector2 p)
    {
        Points.Add(2 * Points[Points.Count - 1] - Points[Points.Count - 2]);
        Points.Add((p + Points[Points.Count - 1]) * .5f);
        Points.Add(p);
    }

    public Vector2[] GetPointsInSegment(int id)
    {
        return new Vector2[]
        {
            Points[3 * id],
            Points[3 * id + 1],
            Points[3 * id + 2],
            Points[3 * id + 3]
        };
    }

    public void MovePoint(int id, Vector2 pos, bool isLock)
    {
        if(!isLock)
        {
            Points[id] = pos;
            return;
        }

        if (id % 3 == 0)
        {
            Vector2 change = pos - Points[id];
            if (id - 1 >= 0) Points[id - 1] += change;
            if (id + 1 < Points.Count) Points[id + 1] += change;

            Points[id] = pos;
        }
        else if (id % 3 == 1)
        {
            if (id - 2 >= 0) Points[id - 2] = 2 * Points[id - 1] - pos;
            Points[id] = pos;
        }
        else if(id % 3 == 2)
        {
            if (id + 2 < Points.Count) Points[id + 2] = 2 * Points[id + 1] - pos;
            Points[id] = pos;
        }

    }

}