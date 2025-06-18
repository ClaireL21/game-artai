using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
//using UnityUtils;

public class DialogueSpawner : MonoBehaviour
{
    [SerializeField] WorldSpaceUI uiDocumentPrefab;
    [SerializeField] float positionRandomness = 0.2f;

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

    public void SpawnDialogue(List<int> items, Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos;

        WorldSpaceUI instance = uiDocumentPool.Get();
       // instance.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);
        instance.transform.position = spawnPos;
        instance.transform.localScale = new Vector3(2.0f, 2.0f, 0.0f);

        this.gameObject.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);

        // turn items into respective emojis
        // gotta rename the emojis based on our names

        // <sprite="emojiAsset" index=4>

        instance.SetLabelText(k_labelName, "Hello! <sprite=\"emojiAsset\" index=4>");
    }

    public void SpawnDialogue(Tuple<int, int> items, Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos;

        WorldSpaceUI instance = uiDocumentPool.Get();
        instance.transform.position = spawnPos;
        instance.transform.localScale = new Vector3(2.0f, 2.0f, 0.0f);

        this.gameObject.transform.SetPositionAndRotation(spawnPos, Camera.main.transform.rotation);

        instance.SetLabelText(k_labelName, $" <sprite=\"emojiAsset\" index={items.Item1}> <sprite=\"emojiAsset\" index={items.Item2}>");
    }

}
