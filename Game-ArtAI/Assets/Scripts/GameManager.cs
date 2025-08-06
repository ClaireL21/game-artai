using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // TODO: unserialize later -- just for debugging!
    [SerializeField] public int dayCounter = 1;
    [SerializeField] public int allRequestsCnt = 0;
    [SerializeField] public int currDayReqRight = 0;
    [SerializeField] public int currDayReqWrong = 0;

    [SerializeField] public int maxRequests = 20;

    public Material grayscaleMaterial;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep between scenes
            maxRequests = 20;
        }
        /*else
        {
            Destroy(gameObject); // Prevent duplicates
        }*/
    }

}
