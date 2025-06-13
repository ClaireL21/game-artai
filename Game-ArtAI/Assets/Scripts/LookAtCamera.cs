using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Camera targetCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}
