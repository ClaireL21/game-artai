using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
//using UnityUtils;

public class DialogueSpawner : MonoBehaviour
{
    [SerializeField] WorldSpaceUI uiDocumentPrefab;
    [SerializeField] float positionRandomness = 0.2f;
    [SerializeField] float uiScale = 1.2f;
    [SerializeField] GameObject dialogueBG;
    [SerializeField] Vector2 uiOffset = Vector2.zero;

    IObjectPool<WorldSpaceUI> uiDocumentPool;
    const string k_labelName = "TestLabel";

    public int debugSize = -1;
    public float debugScale = -1;
    public string debugText = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

    public void SpawnDialogue(RequestObject request, Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos;

        WorldSpaceUI instance = uiDocumentPool.Get();
        Vector3 offset = uiOffset;
        instance.transform.position = spawnPos + offset;
        instance.transform.localScale = new Vector3(uiScale, uiScale, 0.0f);
       // instance.GetComponent<WorldSpaceUI>().GetComponent<UIDocument>().sortingOrder = 1;
        this.gameObject.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);
        Vector3 currScale = dialogueBG.transform.localScale;

        currScale.x = 0.25f * (request.getSize() - 1) + 0.5f;
        debugScale = currScale.x;
        debugSize = request.getSize();
        string requestText = "";
        debugText = "";

        Material[] mats;
        int indx;

        if (request.getColorIndex() >= 0)
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
        }

       /* requestText = "";
        
        requestText += $" <sprite=\"emojiAsset\" index={3}>";
        requestText += $" <sprite=\"animalsAsset\" index={11}>";*/
        //requestText += $" <sprite=\"shapesAsset\" index={15}>";

        //currScale.x = 0.75f;
        //Debug.Log("Request text: " +  requestText);
        //Debug.Log("Request toString: " + request.toString());
        instance.SetLabelText(k_labelName, requestText);
        dialogueBG.transform.localScale = currScale;

        /* if (request.getSize() == 1)
         {
             currScale.x = 0.5f;
             instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}>");
         }
         else if (request.getSize() == 2)
         {
             currScale.x = 0.75f;
             instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}> <sprite=\"emojiAsset\" index={items.Item2}>");

         }
         else
         {
             currScale.x = 1.0f;
             instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}> <sprite=\"emojiAsset\" index={items.Item2}> <sprite=\"emojiAsset\" index={items.Item2}>");
         }*/

    }

}
