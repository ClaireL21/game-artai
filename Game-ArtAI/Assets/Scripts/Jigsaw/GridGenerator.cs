using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    //[SerializeField] private GameObject quadPrefab;
    [SerializeField] private Material puzzleMaterial;

    [Header("Settings")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 5;
    [SerializeField] private float gridScale = 1.0f;

    private float[] columnWidths;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeColumnWidths();
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeColumnWidths()
    {
        columnWidths = new float[columns];
        for (int i = 0; i < columnWidths.Length; i++)
        {
            columnWidths[i] = 1.0f;
        }

        int iters = (columns / 2) * 2;
        for (int i = 0; i < iters; i++)
        {
            int multiplier = 1;
            if (i % 2 == 0)
            {
                multiplier = -1;
            }
            columnWidths[i] += multiplier * 0.2f;
            Debug.Log(columnWidths[i]);

        }

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
                float colOffset = Mathf.Abs(1 - columnWidths[x]) * 0.5f;
                Vector3 spawnPosition = startPos + new Vector3(x - colOffset, y) * gridScale;
                //GameObject pieceInstance = Instantiate(quadPrefab, spawnPosition, Quaternion.identity, transform);
                //pieceInstance.transform.localScale = Vector3.one * gridScale;

                //GameObject pieceInstance = CreateQuadMesh(spawnPosition, gridScale);
                float width = columnWidths[x];
                GameObject pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, 0.5f);


                Vector2 tiling = new Vector2(1f / columns, 1f / rows);
                Vector2 offset = new Vector2((float)x / columns, (float)y / rows);

                pieceInstance.GetComponent<Renderer>().material.mainTextureScale = tiling;
                pieceInstance.GetComponent<Renderer>().material.mainTextureOffset = offset;
            }
        }
    }

    // Creates a rectangular puzzle piece of specified width and height
    // halfwidth, halfheight --> [0, 0.5]
    private GameObject CreatePuzzlePieceMesh(Vector3 position, float scale, float halfwidth, float halfheight)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
        new Vector3(-halfwidth, -halfheight, 0),
        new Vector3(halfwidth, -halfheight, 0),
        new Vector3(halfwidth, halfheight, 0),
        new Vector3(-halfwidth, halfheight, 0),
        };

        int[] triangles = new int[]
        {
        0, 2, 1,
        0, 3, 2
        };

        Vector2[] uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Material should be assigned externally or reused
        mr.material = new Material(puzzleMaterial); // Temporary
        return obj;
    }

    private GameObject CreateQuadMesh(Vector3 position, float size)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * size;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
        new Vector3(-0.5f, -0.5f, 0),
        new Vector3(0.5f, -0.5f, 0),
        new Vector3(0.5f, 0.5f, 0),
        new Vector3(-0.5f, 0.5f, 0),
        };

        int[] triangles = new int[]
        {
        0, 2, 1,
        0, 3, 2
        };

        Vector2[] uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Material should be assigned externally or reused
        mr.material = new Material(puzzleMaterial); // Temporary
        return obj;
    }
}
