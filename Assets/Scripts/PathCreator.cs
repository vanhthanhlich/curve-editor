using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public Pathh path;

    public bool LockAnchor = true;

    public void CreatePath()
    {
        path = new Pathh(transform.position);
    }

}