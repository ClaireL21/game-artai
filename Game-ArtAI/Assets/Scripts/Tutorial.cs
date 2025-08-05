using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject text;
    public Button nextBtn;
    public int dialogueCount = 0; 

    public List<string> dialogue = new(); 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        nextBtn.onClick.AddListener(nextOnClick);

        dialogue.Add("Here at Arthur's Intelligence, customers are our top priority and your job is to fulfill their requests.");
        dialogue.Add("Customers will ask for artwork with certain criteria which you must find and turn into their desired artwork.");
        dialogue.Add("Make sure to work as fast as possible-your salary depends on it.");
        dialogue.Add("I'm counting on you to do your best!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void nextOnClick()
    {
        if (dialogueCount == dialogue.Count-1)
        {

            // clicking button opens gameplay scene 
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            text.GetComponent<TextMeshProUGUI>().text = dialogue[dialogueCount];
            dialogueCount++;
        }

        if (dialogueCount == dialogue.Count - 1)
        {
            // change button text to start 
            nextBtn.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Start!";

        }


    }

}
