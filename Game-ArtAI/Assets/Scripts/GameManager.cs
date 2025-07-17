using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public int dayCounter = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep between scenes
        }
        /*else
        {
            Destroy(gameObject); // Prevent duplicates
        }*/
    }

}
