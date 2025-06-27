using System;
using UnityEngine;

public class CustomerAIControl : AIControlTarget
{
    public GameObject requestPrefab;
    private GameObject request;
    public bool hasRequest;
    public bool madeRequest;
    private float chanceWillRequest = 0.5f;
    public float remainingDistance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAgent();
        SetDestinationNormal();
        hasRequest = false;
        madeRequest = false;
        request = null;
        remainingDistance = agent.remainingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        remainingDistance = agent.remainingDistance;
        // Request was received
        if (Input.GetKeyDown(KeyCode.R) && madeRequest)
        {
            hasRequest = false;
            madeRequest = false;
            CrowdManager.CM.requestsCnt--;
            DeleteRequest();
            SetDestinationNormal();
        } 
        // Check if agent is at its destination
        else if (agent.remainingDistance < 1)
        {
            if (hasRequest && this.goal.Equals("machine")) // current agent is a customer
            {
                // Make request if request hasn't been made it yet
                if (!madeRequest && CrowdManager.CM.requestsCnt < CrowdManager.CM.maxCustomers)   // max requests at a time
                {
                    Debug.Log("Made Request!");
                    MakeRequest();
                }
            }
            else // current agent is at a normal destination
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) > chanceWillRequest)
                {
                    hasRequest = true;
                    SetDestinationCustomer();
                } else
                {
                    SetDestinationNormal();
                }
            }
        }
    }
    private void MakeRequest()
    {
        Vector3 offset = new Vector3(0, this.transform.localScale.y * 0.5f + 2, 0);
        //Quaternion spawnRotation = Quaternion.Euler(0, 0, 0); 
        request = Instantiate(requestPrefab, this.transform.position + offset, Quaternion.identity);
        request.GetComponent<TrackPosition>().InitializeTrack(this.transform, offset);

        //request.transform.SetParent(this.transform, worldPositionStays: true);

        /*request = Instantiate(requestPrefab, this.transform);
        request.transform.localPosition = new Vector3(0, this.transform.localScale.y + 1, 0);*/

       // Tuple<int, int> requestDetails = Tuple.Create(0, 0);
        RequestObject requestDetails = RequestsManager.RM.GetRequest();
        request.GetComponent<DialoguePicker>().SetDialogue(requestDetails);
        madeRequest = true;
        CrowdManager.CM.requestsCnt++;
    }
    private void DeleteRequest()
    {
        Destroy(request);
    }
}
