using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DialoguePicker : MonoBehaviour
{
    [SerializeField] DialogueSpawner dialogueSpawner;
    public int[] choices;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // replace this later w/ actual stuff!! - ising arr for now 
        // rnd picking for now? or keep? 

        //var rndIndx = UnityEngine.Random.Range(0, choices.Length);

        //List<int> inputToSpawn = new List<int>();
        //inputToSpawn.Add(rndIndx);

        //// child has txt box? -- if use tmp approach 
        //dialogueSpawner.SpawnDialogue(inputToSpawn, this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDialogue(Tuple<int, int> inputs)
    {
        dialogueSpawner.SpawnDialogue(inputs, this.transform.position);
    }

}
