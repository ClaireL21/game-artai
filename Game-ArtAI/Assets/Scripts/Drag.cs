using UnityEngine;

public class Drag : MonoBehaviour
{
    private Transform dragObj = null;
    private Vector3 offset;
    private float yVal; // keeping fixed y val so no issues w/ scaling
    [SerializeField] private LayerMask movableLayers;


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
                int parseVal;
                if (hit.transform.gameObject.name == "flippedSprite")
                {
                    dragObj = hit.transform.parent;
                    dragObj.GetComponent<Artwork>().artClicked = true;
                }
                else if (int.TryParse(hit.transform.gameObject.name, out parseVal))
                {
                    dragObj = hit.transform;
                    dragObj.GetComponent<Artwork>().artClicked = true;
                }

                //dragObj = hit.transform;
                offset = dragObj.position - hit.point;
                yVal = dragObj.position.y;

                Debug.Log($"prior rot: {dragObj.rotation}");
                dragObj.rotation = Quaternion.identity;
                Debug.Log($"curr rot: {dragObj.rotation}");

                //if (dragObj == null)
                //{
                //int parseVal;
                //if (dragObj.gameObject.name == "flippedSprite")
                //{
                //    var parent = dragObj.transform.parent;
                //    parent.GetComponent<Artwork>().artClicked = true;
                //}
                //else if (int.TryParse(dragObj.gameObject.name, out parseVal))
                //{
                //    dragObj.GetComponent<Artwork>().artClicked = true;
                //}
                //}

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

                int parseVal;
                if (dragObj.gameObject.name == "flippedSprite")
                {
                    var parent = dragObj.transform.parent;
                    parent.GetComponent<Artwork>().artClicked = false;
                }
                else if (int.TryParse(dragObj.gameObject.name, out parseVal))
                {
                    dragObj.GetComponent<Artwork>().artClicked = false;
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
