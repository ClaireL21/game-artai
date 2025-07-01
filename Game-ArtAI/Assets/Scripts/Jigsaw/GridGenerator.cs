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
    private float[][] puzzlePieceHeights;
    private float[][] puzzleAccHeights;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeColumnWidths();
        InitializePuzzlePieceHeights();
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
    }

    // Heights of the sides of each puzzle piece, organize by "column lines"
    // There are (column + 1) column lines
    // Store column lines to ensure that adjacent puzzle pieces have the same side height
    private void InitializePuzzlePieceHeights()
    {
        puzzlePieceHeights = new float[columns + 1][];
        puzzleAccHeights = new float[columns + 1][];
        for (int i = 0; i < puzzlePieceHeights.Length; i++)
        {
            puzzlePieceHeights[i] = new float[rows];
            puzzleAccHeights[i] = new float[rows];
        }

        
        float offset = 1.0f / (1.0f + this.rows);
        for (int lineIndex = 0; lineIndex < puzzlePieceHeights.Length; lineIndex++)
        {
            float totalHeight = this.rows;
            int rows = puzzlePieceHeights[lineIndex].Length;
            float acc = 0.0f;
            for (int i = 0; i < rows; i++)
            {
                float height;

                if (i == this.rows - 1)
                {
                    height = totalHeight;
                } else
                {
                    float offsetVal = (float)Math.Round(UnityEngine.Random.Range(-offset, offset), 1, MidpointRounding.AwayFromZero);
                    height = 1 + offsetVal;
                    totalHeight -= height;
                }


                // curr piece is not an edge piece
               /* if (i > 0 && i < rows - 1)
                {
                    height = 1.0f;
                } else
                {
                    // flat edge piece 
                    // on even column lines
                    if (lineIndex % 2 == 0)
                    {
                        if (i == 0)
                        {
                            height = 0.7f;
                        } else
                        {
                            height = 1.3f;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            height = 1.3f;
                        }
                        else
                        {
                            height = 0.7f;
                        }
                    }

                }*/
                acc += height;
                puzzlePieceHeights[lineIndex][i] = height;
                puzzleAccHeights[lineIndex][i] = acc;
                UnityEngine.Debug.Log("Puzzle Acc: col line: " + lineIndex + "; row: " + i + "; acc: " + acc);
            }
        }
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
                Vector3 spawnPosition = new Vector2((currWidth + columnWidths[x] * 0.5f) * gridScale - halfWidth, startPos.y + y * gridScale);
                /*Vector2 uvs = new Vector2(spawnPosition.x, spawnPosition.y) + new Vector2(columns, rows) * 0.5f;
                uvs.x /= (gridScale * columns);
                uvs.y /= (gridScale * rows);*/
                /*float colOffset = Mathf.Abs(1 - columnWidths[x]) * 0.5f;
                Vector3 spawnPosition = startPos + new Vector3(x - colOffset, y) * gridScale;*/

                //GameObject pieceInstance = Instantiate(quadPrefab, spawnPosition, Quaternion.identity, transform);
                //pieceInstance.transform.localScale = Vector3.one * gridScale;

                //GameObject pieceInstance = CreateQuadMesh(spawnPosition, gridScale);
                float width = columnWidths[x];
                // GameObject pieceInstance = CreatePuzzlePieceMeshUV(spawnPosition, gridScale, width * 0.5f, 0.5f, spawnPosition);

                float heightA = puzzlePieceHeights[x][y];       // left side
                float heightB = puzzlePieceHeights[x + 1][y];   // right side

                GameObject pieceInstance;
                // top or bottom edge piece
                if (y == 0)
                {
                    pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, 0);
                } else if (y == rows - 1)
                {
                    pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, 1);
                } else
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, y, x);
                }
                /*if (y % 2 == 0)
                {
                    GameObject pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, 1.3f, 0.7f, 0);

                }
                else
                {
                    GameObject pieceInstance = CreateUnevenPuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, 0.7f, 1.3f, 1);

                }*/


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

    // Find the unit coordinates for a piece in one of the middle rows
    // row must be > 0 and < num rows
    private Vector3[] FindCoordsMiddlePiece(int row, int col, float halfwidth)
    {
        if (!(row > 0 && row < this.rows))
        {
            return null;
        }

        Vector3[] coords;

        float heightA = puzzlePieceHeights[col][row];
        float heightB = puzzlePieceHeights[col + 1][row];

        float bLDist = puzzleAccHeights[col][row - 1];          // distance from bottom edge of puzzle to bottom left corner of current piece
        float bRDist = puzzleAccHeights[col + 1][row - 1];      // distance from bottom edge of puzzle to bottom right corner of current piece
        float tRDist = rows - puzzleAccHeights[col + 1][row];   // distance from top edge of puzzle to top right corner of current piece
        float tLDist = rows - puzzleAccHeights[col][row];       // distance from top edge of puzzle to top left corner of current piece

        Vector3 bL = new Vector3(-halfwidth, bLDist - row - 0.5f, 0);
        Vector3 bR = new Vector3(halfwidth, bRDist - row - 0.5f, 0);
        Vector3 tR = new Vector3(halfwidth, this.rows - (row + 1) - tRDist + 0.5f, 0);
        Vector3 tL = new Vector3(-halfwidth, this.rows - (row + 1) - tLDist + 0.5f, 0);

        UnityEngine.Debug.Log("col, row = " + col + ", " + row + "; " + 
            "bL: " + bL + ", bR: " + bR + ", tR: " + tR + ", tL: " + tL);
        coords = new Vector3[] { bL, bR, tR, tL };

        return coords;
    }

    private GameObject CreatePuzzlePieceMesh(Vector3 position, float scale, float halfwidth, int row, int col)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        // Middle piece
        Vector3[] vertices = FindCoordsMiddlePiece(row, col, halfwidth);

        int[] triangles = new int[]
        {
        0, 2, 1,
        0, 3, 2
        };
        Vector2[] verts = verticesToWorld(vertices, position);
        Vector2[] uvs = worldToUV(verts);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        mr.material = new Material(puzzleMaterial);
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
        }
        else
        {
            vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -heightA + 0.5f, 0),
            new Vector3(halfwidth, -heightB + 0.5f, 0),
            new Vector3(halfwidth, 0.5f, 0),
            new Vector3(-halfwidth, 0.5f, 0),
            };
        }

        int[] triangles = new int[]
        {
        0, 2, 1,
        0, 3, 2
        };
        Vector2[] verts = verticesToWorld(vertices, position);
        Vector2[] uvs = worldToUV(verts);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        mr.material = new Material(puzzleMaterial);
        return obj;
    }

    // Creates a rectangular puzzle piece of specified width and height
    // halfwidth, halfheight --> [0, 0.5]
    /*private GameObject CreatePuzzlePieceMeshUV(Vector3 position, float scale, float halfwidth, float halfheight, Vector3 spawnPosition)
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

        *//*Vector2[] uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };*//*

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Material should be assigned externally or reused
        mr.material = new Material(puzzleMaterial); // Temporary
        return obj;
    }*/





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

    /*private GameObject CreateQuadMesh(Vector3 position, float size)
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
    }*/
}
