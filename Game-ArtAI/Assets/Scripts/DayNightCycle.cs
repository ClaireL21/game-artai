using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float dayLength = 10.0f;
    private float timer = 0f;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image clockHand;

    private string endDayScene = "EndDay";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayText.text = GameManager.instance.dayCounter.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float degree = 360 - (timer / dayLength) * 360;
        clockHand.transform.rotation = Quaternion.Euler(0, 0, degree);

        if (timer >= dayLength)
        {
            SceneManager.LoadScene(endDayScene);
            GameManager.instance.dayCounter += 1;
        }
    }
}
