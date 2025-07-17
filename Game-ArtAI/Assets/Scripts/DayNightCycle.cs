using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    private float dayLength = 10.0f;
    private string endDayScene = "EndDay";

    [SerializeField] private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= dayLength)
        {
            SceneManager.LoadScene(endDayScene);
        }
    }
}
