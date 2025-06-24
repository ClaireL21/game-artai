using UnityEngine;

public class TrackPosition : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    public void InitializeTrack(Transform target, Vector3 offset)
    {
        this.target = target;
        this.offset = offset;
    }
    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset; // optional offset
        }
    }

    
}
