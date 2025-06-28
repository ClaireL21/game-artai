using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    //[SerializeField] private GameObject quadPrefab;
    [SerializeField] private GameObject spherePrefab;

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

    // Unit column widths, where each column by default is 1 unit wide
    // Vary column width by offsetting in the negative or positive direction by 1/(column + 1) of column width)
    private void InitializeColumnWidths()
    {
        columnWidths = new float[columns];
        for (int i = 0; i < columnWidths.Length; i++)
        {
            columnWidths[i] = 1.0f;
        }

        float totalWidth = columns;
        float offset = 1.0f / (1.0f + columns);
        for (int i = 0; i < columnWidths.Length; i++)
        {
            if (i == columnWidths.Length - 1)
            {
                columnWidths[i] = totalWidth;
            } else
            {
                float offsetVal = (float)Math.Round(UnityEngine.Random.Range(-offset, offset), 1, MidpointRounding.AwayFromZero);
                columnWidths[i] += offsetVal;
                totalWidth -= columnWidths[i];
            }
            //Debug.Log(columnWidths[i]);
        }
        
        /*int iters = (columns / 2) * 2;
        for (int i = 0; i < iters; i++)
        {
            int multiplier = 1;
            if (i % 2 == 0)
            {
                multiplier = -1;
            }
            columnWidths[i] += multiplier * 0.2f;
            Debug.Log(columnWidths[i]);

        }*/

    }
    private void GenerateGrid()
    {
        Vector3 startPos = Vector2.left * (gridScale * columns) / 2 + Vector2.down * (gridScale * rows) / 2;
        startPos.x += 0.5f * gridScale;
        startPos.y += 0.5f * gridScale;

        float currWidth = 0;
        float halfWidth = gridScale * columns * 0.5f;
        
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                //float colOffset = (currWidth + columnWidths[x] - (x + 1)) * 0.5f;
                Vector3 spawnPosition = new Vector2((currWidth + columnWidths[x] * 0.5f) * gridScale - halfWidth, startPos.y +  y * gridScale);
                Vector2 uvs = new Vector2(spawnPosition.x, spawnPosition.y) + new Vector2(columns, rows) * 0.5f;
                uvs.x /= (gridScale * columns);
                uvs.y /= (gridScale * rows);
                /*float colOffset = Mathf.Abs(1 - columnWidths[x]) * 0.5f;
                Vector3 spawnPosition = startPos + new Vector3(x - colOffset, y) * gridScale;*/

                //GameObject pieceInstance = Instantiate(quadPrefab, spawnPosition, Quaternion.identity, transform);
                //pieceInstance.transform.localScale = Vector3.one * gridScale;

                //GameObject pieceInstance = CreateQuadMesh(spawnPosition, gridScale);
                float width = columnWidths[x];
               // GameObject pieceInstance = CreatePuzzlePieceMeshUV(spawnPosition, gridScale, width * 0.5f, 0.5f, spawnPosition);

                if (y % 2 == 0)
                {
                    GameObject pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, 0.7f, 1.3f, 0);
                } else
                {
                    GameObject pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, 1.3f, 0.7f, 1);

                }


                /*Vector2 tiling = new Vector2(1f / columns, 1f / rows);
                Vector2 offset = new Vector2((float)x / columns, (float)y / rows);*/

                /*pieceInstance.GetComponent<Renderer>().material.mainTextureScale = tiling;
                pieceInstance.GetComponent<Renderer>().material.mainTextureOffset = offset;*/
            }
            currWidth += columnWidths[x];
        }
    }
    private Vector2[] verticesToWorld(Vector3[] vertices, Vector3 spawnPosition)
    {
        Vector2[] worldVertices = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = new Vector2(vertices[i][0], vertices[i][1]);   // need bottom left corner to be at origin?
            worldVertices[i] *= gridScale;
            worldVertices[i] += new Vector2(spawnPosition[0], spawnPosition[1]);

            Vector3 debug = worldVertices[i];
            debug.z = spawnPosition[2];
            //GameObject pieceInstance = Instantiate(spherePrefab, debug, Quaternion.identity, transform);
        }
        //worldToUV(worldVertices);
        return worldVertices;
    }
    private Vector2[] worldToUV(Vector2[] screenVertices)
    {
        float w = columns * gridScale;
        float h = rows * gridScale;

        Vector2[] uvs = new Vector2[screenVertices.Length];
        for (int i = 0; i < uvs.Length; i ++)
        {
            float x = (screenVertices[i][0] + w * 0.5f) / w;
            float y = (screenVertices[i][1] + h * 0.5f) / h;
            uvs[i] = new Vector2(x, y);

            Vector3 debug = uvs[i];
            debug.z = 0;
            //GameObject pieceInstance = Instantiate(spherePrefab, debug, Quaternion.identity, transform);

        }

        return uvs;
    }
    // Creates a rectangular puzzle piece of specified width and height
    // halfwidth, halfheight --> [0, 0.5]
    private GameObject CreatePuzzlePieceMeshUV(Vector3 position, float scale, float halfwidth, float halfheight, Vector3 spawnPosition)
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

        Vector2[] verts = verticesToWorld(vertices, spawnPosition);
        Vector2[] uvs = worldToUV(verts);

        /*Vector2[] uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };*/

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Material should be assigned externally or reused
        mr.material = new Material(puzzleMaterial); // Temporary
        return obj;
    }

    // Creates a trapezoidal puzzle piece of specified width and heightA and heightB
    // halfwidth, halfheight --> [0, 0.5]
    // type = 0 --> top edge is skewed; type = 1 --> bottom edge is skewed
    private GameObject CreateUnevenPuzzlePieceMesh(Vector3 position, float scale, float halfwidth, 
                                                    float heightA, float heightB, int type)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices;
        //Vector2[] uvs;

        if (type == 0)
        {

            vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -0.5f, 0),
            new Vector3(halfwidth, -0.5f, 0),
            new Vector3(halfwidth, heightB - 0.5f, 0),
            new Vector3(-halfwidth, heightA - 0.5f, 0),
            };

            /*uvs = new Vector2[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 0.54f),
            };*/
        } else
        {
            vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -heightA + 0.5f, 0),
            new Vector3(halfwidth, -heightB + 0.5f, 0),
            new Vector3(halfwidth, 0.5f, 0),
            new Vector3(-halfwidth, 0.5f, 0),
            };

            /*uvs = new Vector2[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0.46f),
            new Vector2(1, 1),
            new Vector2(0, 1),
            };*/
        }

        int[] triangles = new int[]
        {
        0, 2, 1,
        0, 3, 2
        };
        Vector2[] verts = verticesToWorld(vertices, position);
        Vector2[] uvs = worldToUV(verts);
        /* float height = Mathf.Max(heightA, heightB);
         Vector2[] uvs = new Vector2[]
         {
         new Vector2(0, 0),
         new Vector2(1, 0),
         new Vector2(1, height),
         new Vector2(0, height)
         };
 */
        /* Vector2[] uvs = new Vector2[vertices.Length];

         float minX = vertices.Min(v => v.x);
         float maxX = vertices.Max(v => v.x);
         float minY = vertices.Min(v => v.y);
         float maxY = vertices.Max(v => v.y);

         for (int i = 0; i < vertices.Length; i++)
         {
             float u = Mathf.InverseLerp(minX, maxX, vertices[i].x);
             float v = Mathf.InverseLerp(minY, maxY, vertices[i].y);
             uvs[i] = new Vector2(u, v);
             Debug.Log("x, y = " + x + ", " + y + "; " + uvs[i]);
         }*/

        // UV offset and size for this tile
        /*float uvWidth = 1f / columns;
        float uvHeight = 1f / rows;

        Vector2 uvOffset = new Vector2((float)x / columns, (float)y / rows);

        // Map UVs to the full image, not the shape
        Vector2[] uvs;
        if (type == 0)
        {
            uvs = new Vector2[]
            {
            uvOffset + new Vector2(0, 0),
            uvOffset + new Vector2(uvWidth, 0),
            uvOffset + new Vector2(uvWidth, uvHeight),
            uvOffset + new Vector2(0, uvHeight)
            };
        }
        else
        {
            uvs = new Vector2[]
            {
            uvOffset + new Vector2(0, 0),
            uvOffset + new Vector2(uvWidth, 0),
            uvOffset + new Vector2(uvWidth, uvHeight),
            uvOffset + new Vector2(0, uvHeight)
            };
        }*/

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Material should be assigned externally or reused
        mr.material = new Material(puzzleMaterial); // Temporary
        return obj;
    }

    

    // Creates a rectangular puzzle piece of specified width and height
    // halfwidth, halfheight --> [0, 0.5]
    /*private GameObject CreatePuzzlePieceMesh(Vector3 position, float scale, float halfwidth, float halfheight)
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
    }*/

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
