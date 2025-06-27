using System;
using System.Collections.Generic;
using System.Linq;
//using NUnit.Framework;
using UnityEngine;
public class RequestsManager : MonoBehaviour
{
    [Header("Request Settings")]

    [Range(0.0f, 1.0f)]
    public float freqReqSizeOne = 0.3f;

    [Range(0.0f, 1.0f)]
    public float freqReqSizeTwo = 0.4f;

    [Range(0.0f, 1.0f)]
    public float freqReqSizeThree = 0.3f;


    // requests in sprite sheets
    public static int numColors;
    public static int numPatterns;
    public static int numThird;

    public Queue<RequestObject> requests;
    // To decode an artInfo item:
    //      artInfoItem = rand(0, numColors + numPatterns + numThird)
    //      so...  artInfoItem / 10 -> sprite sheet
    //             artInfoItem % 10 -> index of corresponding sprite sheet
    //      Note: all spritesheets must be same size for this to work
    public Queue<int> artInfo;
    //public int currNumArt;
    //public int currNumRequests;     // number of requests visible in the scene
    public static RequestsManager RM;




    // gameobjects
    //public List<GameObject> requests;

    // private vars
    //float timer;
    //public bool notActive = true; // should alter this 

    private void Awake()
    {
        RM = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numColors = 16;
        numPatterns = 16;
        numThird = 16;
        requests = new Queue<RequestObject>();
        artInfo = new Queue<int>();

        if (freqReqSizeOne + freqReqSizeTwo + freqReqSizeThree > 1.0f)
        {
            freqReqSizeOne = 0.3f;
            freqReqSizeTwo = 0.4f;
            freqReqSizeThree = 0.3f;
        }

        AddToRequestArtQueues(5);

        //timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToRequestArtQueues(int numItems)
    {
        for (int i = 0; i < numItems; i++)
        {
            UpdateRequestArt();
        }
    }
    private void UpdateRequestArt()
    {
        float randSize = UnityEngine.Random.Range(0.0f, 1.0f);
        int colorIndex;
        int patternIndex;
        int thirdIndex;
        int size;

        if (randSize <= freqReqSizeOne)
        {
            colorIndex = UnityEngine.Random.Range(0, numColors);
            patternIndex = -1;
            thirdIndex = -1;
            size = 1;
        }
        else if (randSize <= freqReqSizeOne + freqReqSizeTwo)
        {
            colorIndex = UnityEngine.Random.Range(0, numColors);
            patternIndex = UnityEngine.Random.Range(0, numPatterns);
            thirdIndex = -1;
            size = 2;
        }
        else
        {
            colorIndex = UnityEngine.Random.Range(0, numColors);
            patternIndex = UnityEngine.Random.Range(0, numPatterns);
            thirdIndex = UnityEngine.Random.Range(0, numThird);
            size = 3;
        }

        RequestObject request = new RequestObject(colorIndex,
                                                  patternIndex,
                                                  thirdIndex,
                                                  size);
        Debug.Log("RM request: " + request.toString());

        requests.Enqueue(request);
        if (colorIndex >= 0)
        {
            artInfo.Enqueue(colorIndex);
        }
        if (patternIndex >= 0)
        {
            artInfo.Enqueue(patternIndex + numColors);
        }
        if (thirdIndex >= 0)
        {
            artInfo.Enqueue(thirdIndex + numColors + numPatterns);

        }
    }

    public RequestObject GetRequest()
    {
        if (requests.Count == 0)
        {
            AddToRequestArtQueues(5);
        }
        return requests.Dequeue();
    }
    
    public int GetArt()
    {
        if (artInfo.Count > 0)
        {
            return artInfo.Dequeue();
        } else
        {
            return UnityEngine.Random.Range(0, numColors + numPatterns + numThird); // numColors + numPatterns + numThird
        }
    }
    /*private void AddRandomArt()
    {
        while (artInfo.Count < currNumArt)
        {
            artInfo.Enqueue(UnityEngine.Random.Range(0, numColors + numPatterns + numThird));
        }
    }*/

}
