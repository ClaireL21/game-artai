using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndDayScript : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI jobPerfText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI worldText;

    private string beginDayScene = "Gameplay";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load in end screen 
        // change to maxRequests
        if (GameManager.instance.allRequestsCnt >= 1)
        {
            SceneManager.LoadScene("GameEndScreen");
        }

        nextButton.onClick.AddListener(OnButtonClicked);

        // job performance text
        // TODO: make it based on progress bar?
        if (GameManager.instance.currDayReqRight < 2)
        {
            jobPerfText.text = "Poor performance today. You didn't meet Arthur's Intelligence standards of excellence";
        }
        else if (GameManager.instance.currDayReqRight < 4)
        {
            jobPerfText.text = "Okay performance today. You made just enough artworks to pass by... but your manager is keeping a close eye on you";
        }
        else if (GameManager.instance.currDayReqRight < 6)
        {
            jobPerfText.text = "Good performance today. Your manager is pleased with the artworks you were able to create today";
        }
        else
        {
            jobPerfText.text = "Outstanding performance today. You're a star employee at Arthur's Intelligence and you've got quite a knack for making art";
        }

        // life & world text -- TODO: separate world to be based on art protests/stuff?
        if (GameManager.instance.allRequestsCnt < 5)
        {
            int choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0: 
                    lifeText.text = "You have enough to pay rent for now.";
                    break;
                case 1:
                    lifeText.text = "Money is tight, but you're making just enough for rent and groceries";
                    break;
                default:
                    lifeText.text = "You're able to pay for rent and groceries, but a car would be nice";
                    break;
            }

            choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                    worldText.text = "The days feel long but at least the sky is blue and the weather's nice";
                    break;
                case 1:
                    worldText.text = "Working at a machine all day isn't ideal but it's nice to be outside in nature";
                    break;
                default:
                    worldText.text = "The customers you work with seem friendly";
                    break;
            }
        }
        else if (GameManager.instance.allRequestsCnt < 10)
        {
            int choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                    lifeText.text = "You've been working hard and your manager's given you a bonus.";
                    break;
                case 1:
                    lifeText.text = "You're making a little extra cash these days. ";
                    break;
                default:
                    lifeText.text = "You bought a pet! It's nice having a companion at home";
                    break;
            }

            choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                    worldText.text = "Maybe you're just imagining it, but the skies feel gloomier than usual";
                    break;
                case 1:
                    worldText.text = "The customers seemed pleased with your work but sometimes they want more from it";
                    break;
                default:
                    worldText.text = "Work has been going well, but you've been noticing some cold shoulders from artists on the street";
                    break;
            }
        } else if (GameManager.instance.allRequestsCnt < 15)
        {
            int choice = UnityEngine.Random.Range(0, 2);
            switch (choice)
            {
                case 0:
                    lifeText.text = "You just received a promotion and your manager's given you a raise.";
                    break;
                default:
                    lifeText.text = "You bought a car! This will make life a lot easier";
                    break;
            }
            choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                    worldText.text = "Maybe you're just imagining it, but the skies feel gloomier than usual";
                    break;
                case 1:
                    worldText.text = "Artists take to protesting on the streets, upset over stolen art";
                    break;
                default:
                    worldText.text = "Artists on the street don't seem too happy about the work you're doing";
                    break;
            }
        } else
        {
            int choice = UnityEngine.Random.Range(0, 2);
            switch (choice)
            {
                case 0:
                    lifeText.text = "You bought a house! Life is nice and comfortable now.";
                    break;
                default:
                    lifeText.text = "You bought a house! Life is nice and comfortable now.";
                    break;
            }
            choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                    worldText.text = "The skies feel gloomier than usual";
                    break;
                case 1:
                    worldText.text = "Artists take to protesting on the streets, upset over stolen art";
                    break;
                default:
                    worldText.text = "Artists on the street don't seem too happy about the work you're doing";
                    break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClicked()
    {
        SceneManager.LoadScene(beginDayScene);

        //Debug.Log("Button was clicked!");
        // your logic here
    }
}
