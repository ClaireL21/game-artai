using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

public class Requests : MonoBehaviour
{
    // requests in sprite sheets
    public static int numPatterns; 
    public static int numColors;

    // gameobjects
    [SerializeField] GameObject artworks;
    [SerializeField] GameObject target;
    //public List<GameObject> requests;

    // private vars
    float timer;
    public bool notActive = true; // should alter this 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numPatterns = 10;
        numColors = 10;

        timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //timer -= Time.deltaTime;

        //if (timer <= 0)
        //{
        //    // req logic

        //    timer = 0;
        //    notActive = true;
        //}

        //if (notActive)
        //{
        //    SelectRequest();
        //}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectRequest();
        }

    }

    void SelectRequest()
    {
        List<int> requestInfos = new List<int>();

        var artTotal = artworks.transform.childCount;

        for (int i = 0; i < artTotal / 2; i++)
        {
            requestInfos.Add(UnityEngine.Random.Range(0, numColors));
        }

        for (int i = artTotal / 2; i < artTotal; i++)
        {
            requestInfos.Add(UnityEngine.Random.Range(0, numPatterns));
        }

        // pick request & set 

        Tuple<int, int> request = Tuple.Create(requestInfos[0], requestInfos[artTotal / 2]);
        target.GetComponent<DialoguePicker>().SetDialogue(request);

        requestInfos = requestInfos.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < artTotal; i++)
        {
            var child = artworks.transform.GetChild(i);
            child.GetComponent<Artwork>().Setup(requestInfos[i]); 
        }

        timer = 5.0f;
        notActive = false;
    }


}
