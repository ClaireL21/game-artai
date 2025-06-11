using UnityEngine;

public class Drag : MonoBehaviour
{
    private Transform dragObj = null;
    private Vector3 offset;
    private float zVal; 
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
            //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
            //                                     Vector2.zero,
            //                                     float.PositiveInfinity,
            //                                     movableLayers);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, movableLayers))
            {
                dragObj = hit.transform;
                offset = dragObj.position - hit.point;
                //zVal = dragObj.position.z;

                //Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zVal));
                //offset = dragObj.position - mouseWorld;

                dragObj.rotation = Quaternion.identity;
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            //dragObj = null; 
            if (dragObj != null)
            {
                //Vector2 screenPos = Input.mousePosition;

                //// Check if it's over the UI drop area
                //if (UIManager.Instance.IsOverDropZone(screenPos)) // Make a UI manager for this
                //{
                //    Debug.Log("Dropped into UI!");

                //    // Optional: Snap to UI position
                //    // or hide/remove from world
                //    dragObj.gameObject.SetActive(false);
                //}

                dragObj = null;
            }
        }

        if (dragObj != null)
        {
            Plane dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, 0)); // flat XY plane
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);
                dragObj.position = worldPos + offset;

                // Keep Z = 0 (2D sprites usually live on Z = 0)
                dragObj.position = new Vector3(dragObj.position.x, dragObj.position.y, 0);

                // Optional: Keep sprite upright
                dragObj.rotation = Quaternion.identity;
            }
        }

        //if (dragObj != null)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    Plane plane = new Plane(Vector3.forward, dragObj.position); // assumes Z-forward drag plane

        //    if (plane.Raycast(ray, out float distance))
        //    {
        //        Vector3 worldPoint = ray.GetPoint(distance);
        //        dragObj.position = new Vector3(worldPoint.x, worldPoint.y, 0) + new Vector3(offset.x, offset.y, offset.z);
        //    }

        //    //dragObj.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        //}
    }

}
