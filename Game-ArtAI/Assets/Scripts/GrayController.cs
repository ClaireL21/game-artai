using UnityEngine;

public class GrayController : MonoBehaviour
{
    public Material grayscaleMaterial;
    private float amount;

    private void Start()
    {
        amount = 1;
        grayscaleMaterial.SetFloat("_Amount", amount);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            amount = Mathf.Clamp01(amount - 0.1f);
            grayscaleMaterial.SetFloat("_Amount", amount);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            amount = Mathf.Clamp01(amount + 0.1f);
            grayscaleMaterial.SetFloat("_Amount", amount);
        }

    }
}
