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
        else if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 2)   // set new random destination
        {
            // Set destination normal or create drawing
            SetDestinationNormal();
            if (!isDrawing && CrowdManager.CM.drawingsCnt < CrowdManager.CM.maxArtists)
            {
                CreateDrawing();
            } 
            /*else
            {
                CheckArtworkStatus();
            }*/
        }

        // check if drawing has been taken
        if (isDrawing)
        {
            CheckArtworkStatus();
        }
    }

    private void CreateDrawing()
    {
        art = Instantiate(artPrefab, this.transform);
        art.transform.localPosition = new Vector3(0, this.transform.localScale.y * 0.85f, 0);

        isDrawing = true;
        CrowdManager.CM.drawingsCnt++;
        int artIndex = RequestsManager.RM.GetArt();
        art.GetComponent<Artwork>().Setup(artIndex);
    }
    private void CheckArtworkStatus()   // need to fix?
    {
        if (art.GetComponent<Artwork>().isMoved)
        {
            isDrawing = false;
            CrowdManager.CM.drawingsCnt--;
        }
    }
}
