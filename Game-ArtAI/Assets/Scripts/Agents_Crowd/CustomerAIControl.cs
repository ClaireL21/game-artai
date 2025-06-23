using UnityEngine;

public class CustomerAIControl : AIControlTarget
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAgent();
        SetDestinationCustomer();

    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 2)   // set new random destination
        {
            // Create request

        }
    }
}
