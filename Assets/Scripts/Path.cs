using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pathh
{
    [SerializeField, HideInInspector]
    List<Vector2> Points;

    public bool isClosed = false;
    public bool autoSetControlPointMode = false;

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

        if(autoSetControlPointMode) AutoSetAllAffectedPoint(Points.Count - 1);
    }

    public Vector2[] GetPointsInSegment(int id)
    {
        Vector2[] vec = new Vector2[4];
        for(int i = 0; i < 4; i ++) vec[i] = Points[LoopIndex(id * 3 + i)];

        return vec;
    }

    public void MovePoint(int id, Vector2 pos)
    {
        if(autoSetControlPointMode)
        {
            if(id % 3 == 0)
            {
                Points[id] = pos;
                AutoSetAllAffectedPoint(id);
            }
            return;
        }

        if (id % 3 == 0)
        {
            Vector2 change = pos - Points[id];

            if (InsideBound(id - 1)) Points[LoopIndex(id - 1)] += change;
            if (InsideBound(id + 1)) Points[LoopIndex(id + 1)] += change;
            
            Points[id] = pos;
        }
        else
        {
            int anchorPoint = (id % 3 == 1) ? id - 1 : id + 1;
            int controlPoint = (id % 3 == 1) ? id - 2 : id + 2;

            if(isClosed)
            {
                anchorPoint = LoopIndex(anchorPoint);
                controlPoint = LoopIndex(controlPoint);
            }

            if (0 <= controlPoint && controlPoint < Points.Count) Points[controlPoint] = 2 * Points[anchorPoint] - pos;
            Points[id] = pos;
        }

    }

    #region Auto Set Mode
    private void AutoSetAnchorControlPoint(int id)
    {
        Vector2 dir = Vector2.zero;
        float[] dist = new float[2];

        if(InsideBound(id - 3))
        {
            Vector2 v = Points[LoopIndex(id - 3)] - Points[id];
            dist[0] = v.magnitude;
            dir += v.normalized;
        }

        if(InsideBound(id + 3))
        {
            Vector2 v = Points[LoopIndex(id + 3)] - Points[id];
            dist[1] = -v.magnitude;
            dir -= v.normalized;
        }

        dir.Normalize();

        for(int i = 0; i < 2; i++)
        {
            int controlPoint = id + i * 2 - 1; //magic :)) [0, 1] -> [-1, 1]
            if(InsideBound(controlPoint))
            {
                Points[LoopIndex(controlPoint)] = Points[id] + 0.5f * dir * dist[i];
            }
        }
    }

    private void AutoSetAllAffectedPoint(int id)
    {
        for(int i = id - 3; i <= id + 3; i += 3)
        {
            if(InsideBound(i)) AutoSetAnchorControlPoint(LoopIndex(i));
        }
        AutoSetStartAndEndControls();
    }

    private void AutoSetAllPoint()
    {
        for(int i = 0; i < Points.Count; i += 3)
        {
            AutoSetAnchorControlPoint(i);
        }
        AutoSetStartAndEndControls();
    }

    void AutoSetStartAndEndControls()
    {
        if (!isClosed)
        {
            Points[Points.Count - 2] = (Points[Points.Count - 1] + Points[Points.Count - 3]) * .5f;
            Points[1] = (Points[0] + Points[2]) * .5f;
        }
    }

    #endregion
    
    public void ToggleAutoSetMode()
    {
        autoSetControlPointMode = !autoSetControlPointMode;
        if(autoSetControlPointMode) AutoSetAllPoint();
    }

    public void ToggleClosedMode()
    {
        isClosed = !isClosed;

        if (isClosed)
        {
            Points.Add(2 * Points[Points.Count - 1] - Points[Points.Count - 2]);
            Points.Add(2 * Points[0] - Points[1]);

            if(autoSetControlPointMode) AutoSetAllPoint();
        }
        else
        {
            Points.RemoveRange(Points.Count - 2, 2);
            if(autoSetControlPointMode) AutoSetAllPoint();
        }
    }
    
    private int LoopIndex(int id) => (id + Points.Count) % Points.Count;

    private bool InsideBound(int id) => 0 <= id && id < Points.Count || isClosed;
}