using UnityEngine;
using UnityEngine.AI;

public class AIControlTarget : MonoBehaviour
{
   //public int goalChosen;
    // todo: give agents a random priority? to prevent from blockages
        // but give protestors highest priority
    // todo: may need to set agents as moving obstacles to prevent from blockages as well
    public GameObject[] goals;
    NavMeshAgent agent;
    public float minSpeed;
    public float maxSpeed;
    public float agentSpeed;
    public bool isProtesting;
    public bool protestorCounted;
    //public Vector3 destination;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        goals = GameObject.FindGameObjectsWithTag("RedGoal");
        int i = Random.Range(0, goals.Length);
       // goalChosen = i;
        agent.SetDestination(goals[i].transform.position);
        minSpeed = CrowdManager.CM.minSpeed;
        maxSpeed = CrowdManager.CM.maxSpeed;
        agentSpeed = Random.Range(CrowdManager.CM.minSpeed, CrowdManager.CM.maxSpeed);
        agent.speed = agentSpeed;
        isProtesting = false;
        protestorCounted = false;
        //destination = agent.destination;

    }

    // Update is called once per frame
    void Update()
    {
       /* if (CrowdManager.CM.grabbedAgents.Count > 0)
        {
            Debug.Log("in crowd manage");
            foreach (GameObject gameObj in CrowdManager.CM.grabbedAgents)
            {
                Debug.Log("game obj: " +  gameObj.name + "; this game obj: " + this.gameObject.name);
                if (gameObj == this.gameObject && protestorCounted)
                {
                    Debug.Log("in protestor uncounted");
                    gameObj.transform.position = new Vector3(23, 1.67f, -10);
                    // drag to mouse position
                    SetDestinationNormal();
                    protestorCounted = false;
                    isProtesting = false;
                    CrowdManager.CM.protestorsCnt--;
                    
                }
            }
            CrowdManager.CM.ResetGrabbedAgents();
        }*/
        // if someone is protesting, other people within a range should also protest
        if (isProtesting)  
        {
            SetDestinationProtest();    // keep setting destination
        } 
        else 
        {
            /*if (Random.Range(0, 100) < 10)  // become a protestor if you are nearby a protest 10% of the time
            {*/
            BecomeProtestorIfNearby();
            /*}*/
        }

        // if destination is reached
        if (isProtesting && (agent.remainingDistance < 2 || agent.velocity.magnitude < 0.1f))    // check velocity or might need to make this radius count proportional to the number of protestors huddled?
        {
            // count protestor if not counted yet
            if (!protestorCounted)
            {
                protestorCounted = true;
                CrowdManager.CM.protestorsCnt++;
            }
            // yelling animation
        }
        else if (agent.remainingDistance < 2)   // set new random destination
        {
            int i = Random.Range(0, goals.Length);
            agent.SetDestination(goals[i].transform.position);
            //goalChosen = i;
        }
    }
    void SetDestinationProtest()
    {
        GameObject protestGoal = CrowdManager.CM.protestGoal;
        agent.SetDestination(protestGoal.transform.position);
    }
    public void SetDestinationNormal()
    {
        int i = Random.Range(0, goals.Length);
        agent.SetDestination(goals[i].transform.position);
    }

    void BecomeProtestorIfNearby()
    {
        if (CrowdManager.CM.protestorsCnt > 0)  // could add a < condition if we want a max # of protestors
        {
            GameObject protestGoal = CrowdManager.CM.protestGoal;
            float distance = Vector3.Distance(protestGoal.transform.position, this.transform.position);
            if (distance <= CrowdManager.CM.protestRadius)
            {
                agent.speed *= 2;
                //CrowdManager.CM.protestorsCnt++;
                SetDestinationProtest();
                isProtesting = true;
            }
        }
        
    }

    private void OnMouseOver()
    {
       /* if (isProtesting)
        {
            SetDestinationNormal();
        }*/
        if (Input.GetMouseButtonDown(1))
        {
            isProtesting = !isProtesting;
            if (isProtesting)
            {
                agent.speed *= 2;
                //CrowdManager.CM.protestorsCnt++;
                SetDestinationProtest();

            }
            else
            {
                agent.speed = agentSpeed;
                //CrowdManager.CM.protestorsCnt--;
                SetDestinationNormal();
            }
        }
        
    }

    /*// Movement rules for an agent
    void ApplyRules()
    {
        GameObject[] agents;
        agents = CrowdManager.CM.allAgents;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float groupSpeed = 0.01f;   // accumulated speed, summation
        float nDistance;
        int groupSize = 0;

        foreach (GameObject agent in agents)
        {
            // not this agent
            if (gameObject != this.gameObject)
            {
                nDistance = Vector3.Distance(agent.transform.position, this.transform.position);

                // check if current agent and this agent are within neighborDistance of each other 
                if (nDistance <= CrowdManager.CM.neighborDistance)
                {
                    vCenter += agent.transform.position;
                    groupSize++;

                    // if other agent is very close to current agent, then set avoid vector
                    if (nDistance < 1.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - agent.transform.position);
                    }
                    AIControlTarget anotherAgent = agent.GetComponent<AIControlTarget>();
                    groupSpeed = groupSpeed + anotherAgent.agentSpeed;
                }
            }

            if (groupSize > 0)
            {
                vCenter = vCenter / groupSize;
                agentSpeed = groupSpeed / groupSize;

                Vector3 direction = (vCenter + vAvoid) - transform.position;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                            Quaternion.LookRotation(direction),
                                                            CrowdManager.CM.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }*/
}
