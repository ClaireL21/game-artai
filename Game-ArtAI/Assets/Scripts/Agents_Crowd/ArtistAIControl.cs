using UnityEngine;

public class ArtistAIControl : AIControlTarget
{
    public GameObject artPrefab;
    private bool isDrawing;
    private GameObject art;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAgent();
        SetDestinationNormal();
        isDrawing = false;
        art = null;
    }

    // Update is called once per frame
    void Update()
    {
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
            // Set destination normal or create drawing
            SetDestinationNormal();
            if (!isDrawing)
            {
                CreateDrawing();
            } else
            {
                CheckArtworkStatus();
            }
        }
    }

    private void CreateDrawing()
    {
        art = Instantiate(artPrefab, this.transform);
        art.transform.localPosition = new Vector3(0, this.transform.localScale.y + 1, 0);

        /*Quaternion spawnRotation = Quaternion.Euler(20, 0, 0);
        Vector3 spawnPos = this.transform.position + new Vector3(0, this.transform.localScale.y * 0.5f + 2, 0);
        art = Instantiate(artPrefab, spawnPos, spawnRotation);
        art.transform.localScale *= 2;
        art.transform.SetParent(this.transform, worldPositionStays: true);*/


        isDrawing = true;
        art.GetComponent<Artwork>().Setup(0);
    }
    private void CheckArtworkStatus()
    {
        if (art.GetComponent<Artwork>().isMoved)
        {
            isDrawing = false;
        }
    }
}
