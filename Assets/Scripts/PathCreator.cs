using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public Pathh path;
    public void CreatePath()
    {
        path = new Pathh(transform.position);
    }

}