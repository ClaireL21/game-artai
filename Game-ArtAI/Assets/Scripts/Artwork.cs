using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Artwork : MonoBehaviour
{
    [SerializeField] Sprite process; 

    float timer = 0.0f;
    Sprite spriteImg;
    bool runLogic = false; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (runLogic)
        {

            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }

            if (timer <= 0.0f)
            {
                StartCoroutine(AnimateSpeech(2.0f));
            }
        }
    }

    private IEnumerator AnimateSpeech(float waitTime)
    {
        // the time we want between finishing art & prancing around 
        yield return new WaitForSeconds(waitTime);

        // setting the sprite 
        this.GetComponent<SpriteRenderer>().sprite = spriteImg;

    }

    public void Setup(int spriteInfo)
    {

        // Assets/Resources/emoji.png

        Sprite[] all = Resources.LoadAll<Sprite>("emoji");

        foreach (var s in all)
        {
            if (s.name == $"emoji_{spriteInfo}")
            {
                spriteImg = s;
                break;
            }
        }


        //spriteImg = Resources.Load<Sprite>($"emoji_{spriteInfo}");
        //spriteImg = Resources.Load<Sprite>($"testimg");
        this.GetComponent<SpriteRenderer>().sprite = process;

        // picking a random time for how long it takes the artist to create art 
        timer = Random.Range(0.0f, 7.0f);
        runLogic = true; 
    }
    
}
