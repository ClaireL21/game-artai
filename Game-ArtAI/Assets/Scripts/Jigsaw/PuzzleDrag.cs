using UnityEngine;

public class PuzzleDrag : MonoBehaviour
{
    private Transform dragObj = null;
    private Vector3 offset;
    private float yVal; // keeping fixed y val so no issues w/ scaling
    [SerializeField] private LayerMask movableLayers;
    private int maxSortingOrder = 10;
    private int tempSortVal = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // getting obj that is hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, movableLayers))
            {
                dragObj = hit.transform;
                offset = dragObj.position - hit.point;
                yVal = dragObj.position.y;

                dragObj.rotation = Quaternion.identity;
                
                MeshRenderer mr = dragObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null )
                {
                    tempSortVal = mr.sortingOrder;
                    mr.sortingOrder = maxSortingOrder;
                } else
                {
                    Debug.Log("No mesh renderer");
                }
                
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            //dragObj = null; 
            if (dragObj != null)
            {
                Vector2 mousePos = Input.mousePosition;

                // Check if it's over the UI drop area
                /*if (UIManager.UIManage.IsInMachineUI(dragObj.gameObject))
                {
                     Debug.Log("Dropped into UI!");

                     // hiding when dropped
                     dragObj.gameObject.SetActive(false);
                }*/
                MeshRenderer mr = dragObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.sortingOrder = tempSortVal;
                    tempSortVal = 0;
                }
                dragObj = null;
            }
        }

        if (dragObj != null)
        {
            // need to use raycast to get pos of mouse 

            Plane dragPlane = new Plane(Vector3.up, new Vector3(0, yVal, 0));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                dragObj.position = worldPoint + offset;

                dragObj.position = new Vector3(dragObj.position.x, yVal, dragObj.position.z);

                dragObj.forward = Camera.main.transform.forward;
            }
        }

    }

}
