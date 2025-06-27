using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject spherePrefab;

    [Header("Settings")]
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private float gridScale = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateGrid()
    {
        Vector3 startPos = Vector2.left * (gridScale * columns) / 2 + Vector2.down * (gridScale * rows) / 2;
        startPos.x += 0.5f * gridScale;
        startPos.y += 0.5f * gridScale;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 spawnPosition = startPos + new Vector3(x, y) * gridScale;
                GameObject sphereInstance = Instantiate(spherePrefab, spawnPosition, Quaternion.identity, transform);
                sphereInstance.transform.localScale = Vector3.one * gridScale;
            }
        }
    }
}
