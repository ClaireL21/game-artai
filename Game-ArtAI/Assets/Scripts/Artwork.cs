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

    public bool isMoved;
    public int debugLogArtIndex = -1;
    public int debugLogCategory = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoved = false;
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
       /* int artIndex = RequestsManager.RM.GetArt();
        debugLogArtIndex = artIndex;*/

        int category = spriteInfo / 16;
        int index = spriteInfo % 16;
        debugLogArtIndex = index;
        debugLogCategory = category;
        string spriteSheet;

        switch (category)
        {
            case 0:
                spriteSheet = "emoji";
                break;
            case 1:
                spriteSheet = "animals";
                break;
            default:
                spriteSheet = "shapes";
                break;
        }

        Sprite[] all = Resources.LoadAll<Sprite>(spriteSheet);

        foreach (var s in all)
        {
            if (s.name == $"{spriteSheet}_{index}")
            {
                spriteImg = s;
                break;
            }
        }


        this.GetComponent<SpriteRenderer>().sprite = process;

        // picking a random time for how long it takes the artist to create art 
        timer = Random.Range(0.0f, 7.0f);
        runLogic = true; 
    }

    public void OnMouseDrag()
    {
        isMoved = true;
    }
}
