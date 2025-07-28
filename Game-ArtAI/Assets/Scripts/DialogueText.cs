using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
//using UnityUtils;

public class DialogueText : MonoBehaviour
{
    [SerializeField] WorldSpaceUI uiDocumentPrefab;
    [SerializeField] float positionRandomness = 0.2f;
    [SerializeField] float uiScale = 1.2f;
    //[SerializeField] GameObject dialogueBG;
    [SerializeField] Vector2 uiOffset = Vector2.zero;

    IObjectPool<WorldSpaceUI> uiDocumentPool;
    const string k_labelName = "TestLabel";

    public int debugSize = -1;
    public float debugScale = -1;
    public string debugText = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake() => uiDocumentPool = new ObjectPool<WorldSpaceUI>(
        Create,
        OnTake,
        OnReturn,
        OnDestroyObj
    );

    WorldSpaceUI Create() => Instantiate(uiDocumentPrefab, transform, true);
    void OnTake(WorldSpaceUI uiDoc) => uiDoc.gameObject.SetActive(true);
    void OnReturn(WorldSpaceUI uiDoc) => uiDoc.gameObject.SetActive(false);
    void OnDestroyObj(WorldSpaceUI uiDoc) => Destroy(uiDoc.gameObject);

    public void SpawnDialogue()
    {
        Vector3 spawnPos = this.transform.position;

        WorldSpaceUI instance = uiDocumentPool.Get();
        Vector3 offset = uiOffset;
        instance.transform.position = spawnPos + offset;
        instance.transform.localScale = new Vector3(uiScale, uiScale, 0.0f);
       // instance.GetComponent<WorldSpaceUI>().GetComponent<UIDocument>().sortingOrder = 1;
        this.gameObject.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);
        //Vector3 currScale = dialogueBG.transform.localScale;

        /*currScale.x = 0.25f * (request.getSize() - 1) + 0.5f;
        debugScale = currScale.x;*/
        string requestText = "helllllo";
       /* debugText = "";

        Material[] mats;
        int indx;*/
        instance.SetLabelText(k_labelName, requestText);


        /*if (request.getColorIndex() >= 0)
        {
            //requestText += $" <sprite=\"emojiAsset\" index={request.getColorIndex()}>";
            //debugText += " <sprite=\"emojiAsset\" index=" + request.getColorIndex();

            mats = Resources.LoadAll<Material>("Colors");
            indx = request.getColorIndex();

            var child = this.gameObject.transform.GetChild(0).transform.GetChild(0);
            child.gameObject.SetActive(true);
            child.GetComponent<SpriteRenderer>().sharedMaterial = mats[indx];

        }
        if (request.getPatternIndex() >= 0)
        {
            //requestText += $" <sprite=\"animalsAsset\" index={request.getPatternIndex()}>";
            //debugText += " <sprite=\"animalsAsset\" index=" + request.getPatternIndex();

            mats = Resources.LoadAll<Material>("JigsawMats");
            indx = request.getPatternIndex();

            var child = this.gameObject.transform.GetChild(0).transform.GetChild(1);
            child.gameObject.SetActive(true);
            child.GetComponent<SpriteRenderer>().sharedMaterial = mats[indx];

        }
        if (request.getThirdIndex() >= 0)
        {
            //requestText += $" <sprite=\"shapesAsset\" index={request.getThirdIndex()}>";
            //debugText += " <sprite=\"shapesAsset\" index=" + request.getThirdIndex();

            mats = Resources.LoadAll<Material>("Colors");
            indx = request.getThirdIndex();

            var child = this.gameObject.transform.GetChild(0).transform.GetChild(2);
            child.gameObject.SetActive(true);
            child.GetComponent<SpriteRenderer>().sharedMaterial = mats[indx];
        }*/

        //dialogueBG.transform.localScale = currScale;

    }

}
