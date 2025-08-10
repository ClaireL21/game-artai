using System.Collections.Generic;
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
    public static float greyAmount = 1.0f;

    public static bool firstPlay = true;

    public static Dictionary<int, List<GameObject>> spriteDict = new Dictionary<int, List<GameObject>>();

    public static bool goodEnding = false;

    public static bool sketchBookSubmission = false;

    public static bool pauseTimer = false;

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

    private void Start()
    {
        if (firstPlay)
        {
            GameManager.instance.grayscaleMaterial.SetFloat("_Amount", 1.0f);
            firstPlay = false;
        }
    }

}
