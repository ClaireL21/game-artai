using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PuzzleDrag : MonoBehaviour
{
    private Transform dragObj = null;
    private Vector3 offset;
    private float yVal; // keeping fixed y val so no issues w/ scaling
    [SerializeField] private LayerMask movableLayers;
    [SerializeField] public int zMax = 10;     // number of moves before resetting, should be greater than the number of pieces
    private MeshRenderer[] pieceObjects;
    private int pieceIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pieceObjects = new MeshRenderer[zMax];
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
                
                UpdateOrder();
                
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (dragObj != null)
            {
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

    void UpdateOrder()
    {
        //Debug.Log("Update:");
        MeshRenderer mr = dragObj.gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            if (pieceIndex >= zMax)
            {
                //Debug.Log("Reset:");
                // Reset pieces order
                int newStartIndex = zMax - 1;
                int search = -1;
                for (int i = 0; i < zMax; i++)
                {
                    if (pieceObjects[i] == null)
                    {
                        search = Mathf.Max(search, i + 1);
                        for (int j = search; j < zMax; j++)
                        {
                            if (pieceObjects[j] != null)
                            {
                                pieceObjects[i] = pieceObjects[j];
                                pieceObjects[i].sortingOrder = i;
                                pieceObjects[j] = null;
                                newStartIndex = j;
                                search = j + 1;
                                break;
                            }
                        }
                    }
                }
                pieceIndex = newStartIndex;
            }
            //Debug.Log("Assign:");
            int temp = mr.sortingOrder;
            mr.sortingOrder = pieceIndex;
            pieceObjects[pieceIndex] = mr;
            pieceIndex++;
            if (temp >= 0)
            {
                pieceObjects[temp] = null;
            }
            //toString();
        }
        else
        {
            Debug.Log("No mesh renderer");
        }
        
    }

    private void toString()
    {
        for (int i = 0;i < pieceObjects.Length;i++)
        {
            if (pieceObjects[i] == null)
            {
                Debug.Log("i = " + i + "; NULL");

            } else
            {
                Debug.Log("i = " + i + "; " + pieceObjects[i].name);

            }
        }
    }

}
