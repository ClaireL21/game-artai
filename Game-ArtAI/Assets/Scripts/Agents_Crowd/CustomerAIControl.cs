using System;
using UnityEngine;

public class CustomerAIControl : AIControlTarget
{
    public GameObject requestPrefab;
    private GameObject request;
    private bool isRequesting;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAgent();
        SetDestinationCustomer();
        request = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 2)   // set new random destination
        {
            // Create request
            if (!isRequesting && CrowdManager.CM.requestsCnt < 1)   // max requests at a time
            {
                MakeRequest();
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

        Tuple<int, int> requestDetails = Tuple.Create(0, 0);
        request.GetComponent<DialoguePicker>().SetDialogue(requestDetails);
        isRequesting = true;
        CrowdManager.CM.requestsCnt++;
    }
}
