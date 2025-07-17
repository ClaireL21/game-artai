using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndDayScript : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    private string beginDayScene = "Prototype1";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.onClick.AddListener(OnButtonClicked);
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
