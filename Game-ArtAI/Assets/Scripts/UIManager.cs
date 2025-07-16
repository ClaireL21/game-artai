using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
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
    [SerializeField] private GameObject sketchBook;
    [SerializeField] private GameObject progressBarUI;

    [SerializeField] private GameObject BottomUISprites;
    [SerializeField] private GameObject JigsawHelper;
    [SerializeField] private GameObject SketchbookHelpText; // remove later!

    [SerializeField] private BoxCollider puzzleDoneUI;
    [SerializeField] private SpriteRenderer puzzleDoneColor;
    private Color gray = new Color(162f / 255f, 181f / 255f, 184f / 255f);
    private Color green = new Color(157f / 255f, 222f / 255f, 101f / 255f);

    [SerializeField] private GameObject puzzlePrefab;
    private GameObject puzzle;
    private GridGenerator puzzleGrid;

    private bool incorrectReq = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManage = this;
        progressBarUI.GetComponent<Image>().fillAmount = 0f;
        puzzleDoneColor.color = gray;
        puzzle = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ProgressBar();
        }

        if (RequestsManager.requestArray.Count == 0)
        {
            ProgressBar();
            RequestsManager.requestArray.Add(-1);
            CustomerAIControl.deleteReq = true;

            // customer feedback
            if (incorrectReq)
            {
                Debug.Log("fulfilled req: wrong");
                incorrectReq = false;
            }
            else
            {
                Debug.Log("fulfilled req: correct");
            }

        }

        // check if it hit generate art button
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // getting obj that is hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.collider != null && hit.collider == generateButtonUI && puzzle == null)
                {
                    Debug.Log("Sprite clicked: " + gameObject.name);
                    RemoveArtInMachineUI();
                    puzzle = Instantiate(puzzlePrefab);
                    puzzleGrid = puzzle.GetComponent<GridGenerator>();
                    puzzleGrid.SetupPuzzle(UnityEngine.Random.Range(2, 4), UnityEngine.Random.Range(2, 5));

                }

                else if (hit.collider != null && hit.collider == sketchBookUI)
                {
                    OpenBook();
                }

                else if (hit.collider != null && hit.collider == exitUI)
                {
                    CloseBook();
                }

                else if (hit.collider != null && hit.collider == puzzleDoneUI)
                {
                    if (puzzle != null && puzzleGrid.puzzleInitialized())
                    {
                        if (puzzleGrid.isFinished)
                        {
                            Destroy(puzzle);
                            puzzle = null;
                            puzzleDoneColor.color = gray;
                        }
                    }
                }
            }
        }

        if (puzzle != null && puzzleGrid.puzzleInitialized())
        {
            if (puzzleGrid.isFinished)
            {
                puzzleDoneColor.color = green;
            } else
            {
                puzzleDoneColor.color = gray;
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
                if (c.gameObject.CompareTag("Art"))
                {
                    if (RequestsManager.requestArray.Contains(int.Parse(c.gameObject.name)))
                    {
                        Debug.Log("correct art");
                        RequestsManager.requestArray.Remove(int.Parse(c.gameObject.name));
                    }
                    else
                    {
                        Debug.Log("wrong art");

                        RequestsManager.requestArray.Remove(0);
                        incorrectReq = true;
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

        SketchbookHelpText.gameObject.SetActive(false);
    }

    public void CloseBook()
    {
        // clear canvas
        sketchBook.GetComponent<Sketchbook>().resetCanvas();

        sketchBookUI.gameObject.SetActive(true);
        sketchBook.SetActive(false);

        // nice to have helper text maybe?
        SketchbookHelpText.gameObject.SetActive(true);
    }

    public void ProgressBar()
    {
        var progressSprite = progressBarUI.GetComponent<Image>();

        progressSprite.fillAmount += 0.2f;
        if (progressSprite.fillAmount == 1.0f)
        {
            Debug.Log("Progress bar full!");
        }
        else if (progressSprite.fillAmount >= 0.5f)
        {
            // initiate drawing mode
            HideUI();
            SketchbookHelpText.SetActive(true);
        }

    }

    // For prototype purposes 
    public void HideUI()
    {
        // hide bottom sprites 
        BottomUISprites.SetActive(false);
        SketchbookHelpText.SetActive(false);

        // hide jigsaw 
        JigsawHelper.SetActive(false);

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
