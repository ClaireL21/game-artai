using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManage;
    //[SerializeField] private RectTransform bottomUI;
    [SerializeField] private BoxCollider machineUI;
    [SerializeField] private BoxCollider generateButtonUI;
    [SerializeField] private BoxCollider sketchBookUI;
    [SerializeField] private BoxCollider exitUI;

    [SerializeField] private GameObject progressBarUI;
    [SerializeField] private GameObject sketchBook;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManage = this;
        progressBarUI.GetComponent<Image>().fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // check if it hit generate art button
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // getting obj that is hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.collider != null && hit.collider == generateButtonUI)
                {
                    Debug.Log("Sprite clicked: " + gameObject.name);
                    RemoveArtInMachineUI();
                }

                else if (hit.collider != null && hit.collider == sketchBookUI)
                {
                    OpenBook();
                }

                else if (hit.collider != null && hit.collider == exitUI)
                {
                    CloseBook();
                }
            }
        }
    }

    /*public bool IsinBottomUI (Vector2 currPos)
    {
        Vector3[] corners = new Vector3[4];
        bottomUI.GetWorldCorners(corners);
        foreach (var cor in corners) {
            Debug.Log("corner: " + cor);

        }

        return (currPos.x > corners[0].x && currPos.x < corners[2].x && 
                currPos.y > corners[0].y && currPos.y < corners[2].y);

    }*/

    /*public bool ClickedGenerateButton(Vector2 currPos)
    {
        Vector3[] corners = new Vector3[4];
        generateButtonUI.GetWorldCorners(corners);
        foreach (var cor in corners)
        {
            Debug.Log("corner: " + cor);

        }

        return (currPos.x > corners[0].x && currPos.x < corners[2].x &&
                currPos.y > corners[0].y && currPos.y < corners[2].y);

    }*/
    public bool IsInMachineUI(GameObject dragged)
    {
        //Debug.Log("machine ui position: " + machineUI.transform.position + "; machine size: " + machineUI.transform.localScale / 2);
        
        Collider[] colliders = Physics.OverlapBox(machineUI.transform.position, machineUI.transform.localScale / 2, Quaternion.identity);
        //Gizmos.DrawWireCube(transform.position, transform.localScale);

        foreach (Collider c in colliders)
        {
            if (c.gameObject == dragged)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveArtInMachineUI()
    {
        bool processSprite = false;

        if (RequestsManager.requestArray.Count == 0)
        {
            Debug.Log("unable to process");
        }
        else
        {
            processSprite = true;
        }

        if (processSprite)
        {
            Collider[] colliders = Physics.OverlapBox(machineUI.transform.position, machineUI.transform.localScale / 2, Quaternion.identity);

            foreach (Collider c in colliders)
            {
                if (c.gameObject.tag == "Art")
                {
                    if (RequestsManager.requestArray.Contains(int.Parse(c.gameObject.name)))
                    {
                        Debug.Log("correct art");
                    }
                    else if (RequestsManager.requestArray.Contains(int.Parse(c.gameObject.name)))
                    {
                        Debug.Log("wrong art");
                    }

                    Debug.Log($"artwork inputted: {c.gameObject.name}");
                    c.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OpenBook()
    {
        sketchBookUI.gameObject.SetActive(false);
        sketchBook.SetActive(true);
    }

    public void CloseBook()
    {
        // clear canvas
        sketchBook.GetComponent<Sketchbook>().resetCanvas();

        sketchBookUI.gameObject.SetActive(true);
        sketchBook.SetActive(false);
    }

    public void progressBar()
    {
        var progressSprite = progressBarUI.GetComponent<Image>();

        progressSprite.fillAmount += 0.2f;
        if (progressSprite.fillAmount == 1.0f)
        {
            Debug.Log("Progress bar full!");
        }

    }


    // debug for GUI 
    //void OnGUI()
    //{
    //    Vector3[] corners = new Vector3[4];
    //    bottomUI.GetWorldCorners(corners);

    //    Vector2 min = new Vector2(corners[0].x, corners[0].y);
    //    Vector2 max = new Vector2(corners[2].x, corners[2].y);

    //    Rect r = new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);
    //    GUI.color = Color.red;
    //    GUI.DrawTexture(r, Texture2D.whiteTexture);
    //}
}
