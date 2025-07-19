using System;
using System.Collections.Generic;
using System.Drawing;
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
    private UnityEngine.Color gray = new UnityEngine.Color(162f / 255f, 181f / 255f, 184f / 255f);
    private UnityEngine.Color green = new UnityEngine.Color(157f / 255f, 222f / 255f, 101f / 255f);

    [SerializeField] private GameObject puzzlePrefab;
    private GameObject puzzle;
    private GridGenerator puzzleGrid;

    private bool incorrectReq = false;

    private List<int> inputMats;
    public static bool animateTexture = true;

    private Material proceduralTexture; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManage = this;
        progressBarUI.GetComponent<Image>().fillAmount = 0f;
        puzzleDoneColor.color = gray;
        puzzle = null;

        inputMats = new List<int>();
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

            PuzzleSetup();

            ProgressBar();
            RequestsManager.requestArray.Add(-1);

            // customer feedback
            if (incorrectReq)
            {
                Debug.Log("fulfilled req: wrong");
                incorrectReq = false;
                ManageIncorrect();
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

                            // reset procedural texture color 
                            proceduralTexture.SetColor("_BaseW", UnityEngine.Color.black);
                            proceduralTexture.SetColor("_BaseW", UnityEngine.Color.white);
                            proceduralTexture.SetInt("_isAnimated", 1);

                            CustomerAIControl.deleteReq = true;
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
                Debug.Log("c name" + c.gameObject.name);
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

                        RequestsManager.requestArray.RemoveAt(0);
                        incorrectReq = true;
                    }

                    inputMats.Add(int.Parse(c.gameObject.name));

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

    public void PuzzleSetup()
    {
        puzzle = Instantiate(puzzlePrefab);
        puzzleGrid = puzzle.GetComponent<GridGenerator>();

        // setting up jigsaw mats 
        
        Material jigsawMat = null;
        Material color1 = null; 
        Material color2 = null;

        Material[] mats;

        foreach (var m in inputMats) {
            int category = m / RequestsManager.numColors;
            int index = m % RequestsManager.numColors;

            switch (category)
            {
                case 0:
                    mats = Resources.LoadAll<Material>("Colors");
                    color1 = mats[index];

                    break;

                case 1:
                    mats = Resources.LoadAll<Material>("JigsawMats");
                    jigsawMat = mats[index];

                    break;

                default:
                    mats = Resources.LoadAll<Material>("Colors");
                    color2 = mats[index];

                    break;
            }
        }

        if (jigsawMat != null)
        {
            GridGenerator.puzzleMaterial = jigsawMat;
            proceduralTexture = jigsawMat;

            GridGenerator.puzzleMaterial.SetColor("_BaseB", color1.color);

            if (color2 != null)
            {
                GridGenerator.puzzleMaterial.SetColor("_BaseW", color2.color);
            }
            else
            {
                GridGenerator.puzzleMaterial.SetColor("_BaseW", UnityEngine.Color.white);
            }

            if (animateTexture)
            {
                GridGenerator.puzzleMaterial.SetInt("_isAnimated", 1);
            }
            else
            {
                GridGenerator.puzzleMaterial.SetInt("_isAnimated", 0);
            }

        }
        else
        {
            GridGenerator.puzzleMaterial = color1;
        }
        
        puzzleGrid.SetupPuzzle(UnityEngine.Random.Range(2, 4), UnityEngine.Random.Range(2, 5));
    }

    public void ManageIncorrect()
    {
        List<int> incorrect = new List<int>();

        // cycle through input
        foreach (var userIn in inputMats)
        {
            // check if in request manager 
            bool inReq = RequestsManager.requestReference.Contains(userIn);

            // sort based on this 
            if (inReq)
            {
                RequestsManager.requestReference.Remove(userIn);
            }
            else
            {
                incorrect.Add(userIn);
            }
        }

        // replace prev correct ones in scene w/ incorrect 
        for (int i = 0; i < incorrect.Count; i++)
        {
            GameObject obj = GameObject.Find($"{RequestsManager.requestReference[i]}");
            obj.name = incorrect[0].ToString();

            Material[] mats;
            int category = incorrect[0] / RequestsManager.numColors;
            int index = incorrect[0] % RequestsManager.numColors;
            Material mat;

            switch (category)
            {
                case 0:
                    mats = Resources.LoadAll<Material>("Colors");
                    mat = mats[index];
                    break;

                case 1:
                    mats = Resources.LoadAll<Material>("JigsawMats");
                    mat = mats[index];
                    break;

                default:
                    mats = Resources.LoadAll<Material>("Colors");
                    mat = mats[index];
                    break;
            }

            obj.GetComponent<SpriteRenderer>().sharedMaterial = mat;
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().sharedMaterial = mat;
        }

    }

}
