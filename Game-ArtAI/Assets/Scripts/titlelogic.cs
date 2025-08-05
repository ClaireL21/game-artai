using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class titlelogic : MonoBehaviour
{
    public Button startBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startBtn.onClick.AddListener(onStartClick);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onStartClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

}
