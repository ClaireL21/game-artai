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

    public void SpawnDialogue(Tuple<int, int> items, Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos;

        WorldSpaceUI instance = uiDocumentPool.Get();
        Vector3 offset = uiOffset;
        instance.transform.position = spawnPos + offset;
        instance.transform.localScale = new Vector3(uiScale, uiScale, 0.0f);
       // instance.GetComponent<WorldSpaceUI>().GetComponent<UIDocument>().sortingOrder = 1;
        this.gameObject.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);
        Vector3 currScale = dialogueBG.transform.localScale;

        int testNumEmojis = 2;

        if (testNumEmojis == 1)
        {
            currScale.x = 0.5f;
            instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}>");
        }
        else if (testNumEmojis == 2)
        {
            currScale.x = 0.75f;
            instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}> <sprite=\"emojiAsset\" index={items.Item2}>");

        }
        else
        {
            currScale.x = 1.0f;
            instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}> <sprite=\"emojiAsset\" index={items.Item2}> <sprite=\"emojiAsset\" index={items.Item2}>");
        }
        dialogueBG.transform.localScale = currScale;
    }

}
