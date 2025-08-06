using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAIControl : AIControlTarget
{
    public GameObject requestPrefab;
    public GameObject dialoguePrefab;

    private GameObject request;
    public bool hasRequest;
    public bool madeRequest;
    private float chanceWillRequest = 0.5f;
    public float remainingDistance;

    private RequestObject requestDetails;

    public static bool deleteReq = false;

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
        else if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 2)
        {
            if (hasRequest && this.goal.Equals("machine")) // current agent is a customer
            {
                // Make request if request hasn't been made it yet
                if (!madeRequest && CrowdManager.CM.requestsCnt < CrowdManager.CM.maxCustomers)   // max requests at a time
                {
                    //Debug.Log("Made Request!");
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

        if (deleteReq && madeRequest)
        {
            CompletedRequest();
            deleteReq = false;
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
        requestDetails = RequestsManager.RM.GetRequest();
        request.GetComponent<DialoguePicker>().SetDialogue(requestDetails);
        madeRequest = true;
        CrowdManager.CM.requestsCnt++;
        SendRequest();
        CrowdManager.CM.currCustomer = this.gameObject;
        //RequestsManager.currRequest = requestDetails;
    }

    public void SendRequest()
    {
        var reqArr = RequestsManager.requestArray;
        if (reqArr.Count > 0 && !reqArr.Contains(-1))
        {
            reqArr.Clear();
        }

        if (requestDetails.getColorIndex() >= 0)
        {
            reqArr.Add(requestDetails.getColorIndex());
        }
        if (requestDetails.getPatternIndex() >= 0)
        {
            reqArr.Add(requestDetails.getPatternIndex() + requestDetails.getMats());
        }
        if (requestDetails.getThirdIndex() >= 0)
        {
            reqArr.Add(requestDetails.getThirdIndex() + requestDetails.getMats() + requestDetails.getMats());
        }

        reqArr.RemoveAll(item => item  == -1);

        RequestsManager.requestReference = new List<int>(reqArr);

        foreach (var item in reqArr)
        {
            Debug.Log($"reqArr: {item}");
        }

    }

    public void CompletedRequest()
    {
        hasRequest = false;
        madeRequest = false;
        CrowdManager.CM.requestsCnt--;
        DeleteRequest();
        SetDestinationNormal();

        GameManager.instance.allRequestsCnt++;
        //SpawnCustomerDialogue();
        //dialoguePrefab.transform.position = this.transform.position + offset;
    }

    public void SpawnCustomerDialogue(bool incorrect)
    {
        //Debug.Log("Spanwed dialogue??");
        Vector3 offset = new Vector3(0, this.transform.localScale.y * 0.5f + 2, 0);
        string text;
        if (incorrect)
        {
            text = "this isn't what I asked for...";
        }
        else
        {
            int diaChoice = UnityEngine.Random.Range(0, 6);

            switch (diaChoice)
            {
                case 0:
                    text = "thanks!";
                    break;
                case 1:
                    text = "looks just like the real thing";
                    break;
                case 2:
                    text = "wow that was fast";
                    break;
                case 3:
                    text = "looks a little funny...but thanks";
                    break;
                case 4:
                    text = "this looks nice";
                    break;
                default:
                    text = "thanks, this isn't bad";
                    break;
            }
        }
        dialoguePrefab.GetComponent<DialogueText>().requestText = text;
        GameObject dialogue = Instantiate(dialoguePrefab, this.transform.position + offset, Quaternion.identity);

        dialogue.GetComponent<TrackPosition>().InitializeTrack(this.transform, offset);
        Destroy(dialogue, 2f);
    }

    private void DeleteRequest()
    {
        Destroy(request);
    }
}
