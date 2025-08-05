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
        nextButton.onClick.AddListener(OnButtonClicked);

        // job performance text
        // TODO: make it based on progress bar?
        if (GameManager.instance.currDayReqRight < 2)
        {
            jobPerfText.text = "Poor performance";
        }
        else if (GameManager.instance.currDayReqRight < 4)
        {
            jobPerfText.text = "Okay performance";
        }
        else if (GameManager.instance.currDayReqRight < 6)
        {
            jobPerfText.text = "Good performance";
        }
        else
        {
            jobPerfText.text = "Outstanding performance";
        }

        // life & world text -- TODO: separate world to be based on art protests/stuff?
        if (GameManager.instance.allRequestsCnt < 5)
        {
            lifeText.text = "You have enought to pay rent";
            worldText.text = "The world is okay";
        }
        else if (GameManager.instance.allRequestsCnt < 10)
        {
            lifeText.text = "You bought a pet";
            worldText.text = "The world is meh";
        } else if (GameManager.instance.allRequestsCnt < 15)
        {
            lifeText.text = "You bought a car";
            worldText.text = "The world is bad";
        } else
        {
            lifeText.text = "You bought a house";
            worldText.text = "The world is really bad";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClicked()
    {
        SceneManager.LoadScene(beginDayScene);

        Debug.Log("Button was clicked!");
        // your logic here
    }
}
