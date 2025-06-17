using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManage;
    [SerializeField] private RectTransform bottomUI;
    [SerializeField] private BoxCollider machineUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManage = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsinBottomUI (Vector2 currPos)
    {
        Vector3[] corners = new Vector3[4];
        bottomUI.GetWorldCorners(corners);
        foreach (var cor in corners) {
            Debug.Log("corner: " + cor);

        }

        return (currPos.x > corners[0].x && currPos.x < corners[2].x && 
                currPos.y > corners[0].y && currPos.y < corners[2].y);

    }
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
