using UnityEngine;

public class Material_Inputs : MonoBehaviour
{
    private Material mat;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mat.SetColor("_BaseB", Color.blue);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            mat.SetColor("_BaseW", Color.red);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            mat.SetInt("_isAnimated", 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            mat.SetInt("_isAnimated", 1);
        }

    }
}
