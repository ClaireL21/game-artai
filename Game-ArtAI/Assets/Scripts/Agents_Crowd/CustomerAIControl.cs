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
        request = Instantiate(requestPrefab, this.transform);
        request.transform.localPosition = new Vector3(0, this.transform.localScale.y + 1, 0);
        isRequesting = true;
        CrowdManager.CM.requestsCnt++;
    }
}
