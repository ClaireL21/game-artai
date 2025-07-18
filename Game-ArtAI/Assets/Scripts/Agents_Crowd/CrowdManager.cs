using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    [Range(0.1f, 10.0f)]
    public float grabRadius = 3.0f;

    /* Artist variables */
    public GameObject[] allArtAgents;
    public List<GameObject> grabbedAgents;
    public List<Vector3> offsetAgents;

    public int protestorsCnt;
    public GameObject protestGoal;

    // Dragging logic
    private Vector3 initialMousePosition;
    private bool isDragging;

    // Drawing
    public int maxArtists;
    public int drawingsCnt;

    /* Customer variables */
    public int maxCustomers;
    public GameObject customerGoal;
    public int requestsCnt;

    private void Awake()
    {
        CM = this;
        isDragging = false;
        customerGoal = GameObject.FindGameObjectWithTag("CustomerGoal");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //CM = this;
        allArtAgents = GameObject.FindGameObjectsWithTag("ArtistAgent");
        protestGoal = GameObject.FindGameObjectWithTag("ProtestGoal");
        grabbedAgents = new List<GameObject>();
        offsetAgents = new List<Vector3>();
        protestorsCnt = 0;
        drawingsCnt = 0;
        requestsCnt = 0;
        maxArtists = 4;
        maxCustomers = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            initialMousePosition = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector3.Distance(Input.mousePosition, initialMousePosition) > 1)
            {
                isDragging = true;
                SetAgentsInRadius();
            }

            if (isDragging)
            {
                // Dragging logic here
                // Debug.Log("Mouse Dragged!");
                FindAgentsInRadius(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                // End drag logic here
                isDragging = false;
                ResetGrabbedAgents();
                // Debug.Log("End Dragged!");
            }
        }
    }

    Vector3 ScreenToWorld(Vector3 mousePos)
    {
        // Get world point
        Vector3 worldMousePos = mousePos;
        worldMousePos.z = 50f;
        Vector3 origin = Camera.main.transform.position;
        worldMousePos = Camera.main.ScreenToWorldPoint(worldMousePos);
        Vector3 direction = worldMousePos - origin;

        // Debugging -- Visualize Ray
        //Debug.DrawRay(origin, worldMousePos - origin, Color.blue);

        // Create a Ray
        direction = direction.normalized;
        Ray ray = new Ray(origin, direction);

        // Find the desired distance along ray (t) using desired y coordinate (py)
        GameObject firstAgent = allArtAgents[0];
        float py = firstAgent.transform.position.y;
        float t = (py - origin.y) / direction.y;

        // Find the world point of mouse click using t
        Vector3 worldPoint = ray.GetPoint(t);

        return worldPoint;
    }

    void SetAgentsInRadius()
    {
        Vector3 worldPoint = ScreenToWorld(initialMousePosition);
        // Loop through game objects
        foreach (GameObject gameObj in allArtAgents)
        {
            if (Vector3.Distance(worldPoint, gameObj.transform.position) <= grabRadius &&         // can only grab within certain radius
                Vector3.Distance(worldPoint, protestGoal.transform.position) <= protestRadius)  // can only grab protestors
            {
                // Debug.Log("Within grab radius");
                grabbedAgents.Add(gameObj);
                Vector3 offset = gameObj.transform.position - worldPoint;
                offsetAgents.Add(offset);
            }
        }
        //prevWorldPoint = worldPoint;
    }
    void FindAgentsInRadius(Vector3 inputMousePos)
    {
        // Get world point
        Vector3 newWorldPoint = ScreenToWorld(inputMousePos);

        // Loop through game objects
        int i = 0;
        foreach (GameObject gameObj in grabbedAgents)
        {
            AIControlTarget agent = gameObj.GetComponent<AIControlTarget>();
            if (agent.protestorCounted)
            {
                // Debug.Log("in protestor uncounted");
               // gameObj.transform.position = newWorldPoint + offsetAgents[i];

                Vector3 targetPosition = newWorldPoint + offsetAgents[i];
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPosition, out hit, 5.0f, NavMesh.AllAreas))
                {
                    // Debug.Log("On nav mesh");
                    NavMeshAgent nav = gameObj.GetComponent<NavMeshAgent>();
                    Vector3 finalPos = hit.position;
                    finalPos.y = targetPosition.y;
                    nav.Warp(finalPos);  // Instantly moves the agent to valid NavMesh position
                } 
                /*else
                {
                    gameObj.transform.position = newWorldPoint + offsetAgents[i];
                }*/
            }
            i++;
        }
        //prevWorldPoint = newWorldPoint;
    }

    public void ResetGrabbedAgents()
    {
        foreach (GameObject gameObj in grabbedAgents)
        {
            AIControlTarget agent = gameObj.GetComponent<AIControlTarget>();
            if (agent.protestorCounted)
            {
                agent.SetDestinationNormal();
                agent.protestorCounted = false;
                agent.isProtesting = false;
                agent.GetComponent<NavMeshAgent>().speed = agent.agentSpeed;
                CM.protestorsCnt--;
            }

        }
        grabbedAgents.Clear();
        offsetAgents.Clear();
    }

}
