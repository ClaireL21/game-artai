using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Artwork : MonoBehaviour
{
    [SerializeField] Sprite process;
    [SerializeField] Sprite circle; 

    float timer = 0.0f;
    Sprite spriteImg;
    bool runLogic = false; 

    public bool isMoved;
    public int debugLogArtIndex = -1;
    public int debugLogCategory = -1;

    private Material mat;
    private string spriteType;

    // time sprite 

    // circle sprite

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
        //this.GetComponent<SpriteRenderer>().sprite = spriteImg;

        // setting sprite & material
        this.GetComponent<SpriteRenderer>().sprite = circle;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = circle;

        // getting material based on category 
        if (mat != null)
        {
            this.GetComponent<SpriteRenderer>().sharedMaterial = mat;
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().sharedMaterial = mat;
        }

    }

    public void Setup(int spriteInfo)
    {
       /* int artIndex = RequestsManager.RM.GetArt();
        debugLogArtIndex = artIndex;*/

        int category = spriteInfo / 12;
        int index = spriteInfo % 12;
        debugLogArtIndex = index;
        debugLogCategory = category;
        string spriteSheet;

        // change this to use materials 
        switch (category)
        {
            case 0:
                spriteSheet = "emoji";
                spriteType = "color1";
                break;
            case 1:
                spriteSheet = "animals";
                spriteType = "texture";
                break;
            default:
                spriteSheet = "shapes";
                spriteType = "color2";
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

        Material[] mats;

        switch (spriteType)
        {
            case "color1":
                mats = Resources.LoadAll<Material>("Colors");
                mat = mats[index];

                break;
            case "texture":
                mats = Resources.LoadAll<Material>("JigsawMats");
                mat = mats[index];

                break;
            case "color2":
                mats = Resources.LoadAll<Material>("Colors");
                mat = mats[index];

                break;
            default:
                break;
        }

        this.GetComponent<SpriteRenderer>().sprite = process;

        // picking a random time for how long it takes the artist to create art 
        timer = Random.Range(0.0f, 7.0f);
        runLogic = true; 
    }

    public void OnMouseDrag()
    {
        isMoved = true;
        this.transform.parent = null;
    }
}
