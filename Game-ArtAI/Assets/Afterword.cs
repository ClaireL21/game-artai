using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Afterword : MonoBehaviour
{
    [SerializeField] private GameObject afterwordTxt;
    [SerializeField] private Button menuBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuBtn.onClick.AddListener(OnButtonClicked);

        if (GameManager.goodEnding)
        {
            afterwordTxt.GetComponent<TextMeshProUGUI>().text = "Color has been brought back to the world and you have contributed to that with art of your own. Pick up that paintbrush and continue spreading joy. \r\nYou may not have everything you wanted monetarily but with further hardwork and your own creativity, who can stop you?";
        }
        else
        {
            afterwordTxt.GetComponent<TextMeshProUGUI>().text = "The world has turned grey devoid of the art the brings joy and wonder.\r\nBut screw the consequences, you have what you wanted! You might still be a lowly cog in the wheel but you’ll rise in the ranks eventually. Surely your contributions are invaluable. \r\n\r\nThis is what you wanted though…isn’t it?";
        }

        GameManager.goodEnding = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClicked()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
