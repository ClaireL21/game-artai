using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CrowdManager : MonoBehaviour
{
    public static CrowdManager CM;
    [Header("Crowd Settings")]

    // Agent parameters
    [Range(0.1f, 5.0f)]
    public float minSpeed = 0.1f;
    [Range(0.1f, 10.0f)]
    public float maxSpeed = 5.0f;

    [Range(0.1f, 10.0f)]
    public float protestRadius = 5.0f;

    /* // Flocking Rule parameters
     [Range(0.1f, 10.0f)]
     public float neighborDistance = 5.0f;
     [Range(0.1f, 10.0f)]
     public float rotationSpeed = 5.0f;
 */
    public GameObject[] allAgents;
    public int protestorsCnt;
    public GameObject protestGoal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CM = this;
        allAgents = GameObject.FindGameObjectsWithTag("Agent");
        protestGoal = GameObject.FindGameObjectWithTag("ProtestGoal");
        protestorsCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector3 worldMousePos = mousePosition;
        worldMousePos.z = 50f;
        Vector3 origin = Camera.main.transform.position;
        worldMousePos = Camera.main.ScreenToWorldPoint(worldMousePos);
        Vector3 direction = worldMousePos - origin;
        
        Debug.DrawRay(origin, worldMousePos - origin, Color.blue);

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Create a Ray
            /*Vector3 origin = Camera.main.transform.position;
            Vector3 direction = worldMousePos - origin;*/
            direction = direction.normalized;
            Ray ray = new Ray(origin, direction);
            Debug.Log("sample t = 0: " + ray.GetPoint(0));
            Debug.Log("sample t = 10: " + ray.GetPoint(10));
            Debug.Log("direction: " + direction);

            // Find the desired distance along ray (t) using desired y coordinate (py)
            GameObject firstAgent = allAgents[0];
            float py = firstAgent.transform.position.y;
            Debug.Log("origin y position: " + origin.y);
            Debug.Log("py position: " + py);
            Debug.Log("vector y position: " + direction.y);

            float t = (py - origin.y) / direction.y;
            Debug.Log("float t: " + t);


            // Find the world point of mouse click using t
            Vector3 worldPoint = ray.GetPoint(t);
            Debug.Log("World position: " + worldPoint);



            // 3. Define a Plane (e.g., the ground)

            /*Plane groundPlane = new Plane(Vector3.up, pos);

            // 4. Raycast
            if (groundPlane.Raycast(ray, out float hit))
            {
                Vector3 worldPos = hit.
            }*/
            /*//*RaycastHit hit;
            if (groundPlane.Raycast(ray, out hit))
            {
                // 5. Get World Position
                Vector3 worldPosition = hit.point;

                // Use the world position as needed
                Debug.Log("World position: " + worldPosition);
            }*/
        }
    }
}
