using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    private float dayLength = 10.0f;
    [SerializeField] private float timer = 0f;

    [SerializeField] private TextMeshProUGUI dayText;

    private string endDayScene = "EndDay";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameManager.instance.dayCounter = 1;
        dayText.text = GameManager.instance.dayCounter.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= dayLength)
        {
            SceneManager.LoadScene(endDayScene);
            GameManager.instance.dayCounter += 1;
        }
    }
}
