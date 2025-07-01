using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.HableCurve;

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
        // GenerateGrid();
        GenerateGridWithTabs();
    }

    private void AddTabTB(List<Vector3> vertices, List<int> triangles, float radius, int segments, float hh)
    {
        int centerIndex = vertices.Count;
        Vector3 center = new Vector3(0, hh, 0);

        int direction = hh < 0 ? -1 : 1;

        // Vertices
        vertices.Add(center); // Center of top edge — for fan triangles
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI * i / segments; // from 0 to PI
            float x = Mathf.Cos(angle) * radius * direction;
            float y = Mathf.Sin(angle) * radius * direction;
            vertices.Add(center + new Vector3(x, y, 0)); // semicircle verts
        }

        // Triangles
        for (int i = 0; i < segments; i++)
        {
            int start = centerIndex + 1; // semicircle vertices start at index 4
            triangles.Add(centerIndex);                  // triangle center = top right corner of rect
            triangles.Add(start + i + 1);      // outer point i+1
            triangles.Add(start + i);          // outer point i*/
        }
    }

    private void AddTabLR(List<Vector3> vertices, List<int> triangles, float radius, int segments, float hw)
    {
        int centerIndex = vertices.Count;
        Vector3 center = new Vector3(hw, 0, 0);

        int direction = hw < 0 ? 1 : -1;

        // Vertices
        vertices.Add(center); // Center of top edge — for fan triangles
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI * i / segments + Mathf.PI / 2; // from 0 to PI
            float x = Mathf.Cos(angle) * radius * direction;
            float y = Mathf.Sin(angle) * radius * direction;
            vertices.Add(center + new Vector3(x, y, 0)); // semicircle verts
        }

        // Triangles
        for (int i = 0; i < segments; i++)
        {
            int start = centerIndex + 1; // semicircle vertices start at index 4
            triangles.Add(centerIndex);                  // triangle center = top right corner of rect
            triangles.Add(start + i + 1);      // outer point i+1
            triangles.Add(start + i);          // outer point i*/
        }
    }

    private GameObject MakeDummyMesh(Vector3 position, float width, float height, float radius, int segments)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float hw = width / 2f;
        float hh = height / 2f;

        // Step 1: rectangle base (4 corners)
        vertices.Add(new Vector3(-hw, -hh, 0)); // 0 bottom left
        vertices.Add(new Vector3(hw, -hh, 0));  // 1 bottom right
        vertices.Add(new Vector3(hw, hh, 0));   // 2 top right
        vertices.Add(new Vector3(-hw, hh, 0));  // 3 top left

        // Step 2: top semicircle
        /*int centerIndex = vertices.Count;
        Vector3 center = new Vector3(0, hh, 0);
        vertices.Add(center); // Center of top edge — for fan triangles

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI * i / segments; // from 0 to PI
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices.Add(center + new Vector3(x, y, 0)); // semicircle verts
        }*/

        // Step 3: triangles for rectangle
        triangles.AddRange(new int[] { 0, 2, 1, 0, 3, 2 });

        // Step 4: triangles for semicircle
        /*for (int i = 0; i < segments; i++)
        {
            int start = centerIndex + 1; // semicircle vertices start at index 4
            triangles.Add(centerIndex);                  // triangle center = top right corner of rect
            triangles.Add(start + i + 1);      // outer point i+1
            triangles.Add(start + i);          // outer point
        }*/
        AddTabTB(vertices, triangles, radius, segments, hh);    // up
        AddTabTB(vertices, triangles, radius, segments, -hh);   // down
        AddTabLR(vertices, triangles, radius, segments, hw);    // right
        AddTabLR(vertices, triangles, radius, segments, -hw);

        Vector2[] verts = verticesToWorld(vertices.ToArray(), Vector3.zero);
        Vector2[] uvs = worldToUV(verts);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs;
        mf.mesh = mesh;
        /*Material defaultMat = new Material(Shader.Find("Unlit/Color"));
        defaultMat.color = Color.white;*/

        mr.material = new Material(puzzleMaterial);

//        Instantiate(mesh, Vector3.zero, Quaternion.identity, transform);
        return obj;
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

                acc += height;
                puzzlePieceHeights[lineIndex][i] = height;
                puzzleAccHeights[lineIndex][i] = acc;
                //UnityEngine.Debug.Log("Puzzle Acc: col line: " + lineIndex + "; row: " + i + "; acc: " + acc);
            }
        }
    }

    private void GenerateGridWithTabs()
    {
        Vector3 startPos = Vector2.left * (gridScale * columns) / 2 + Vector2.down * (gridScale * rows) / 2;
        startPos.x += 0.5f * gridScale;
        startPos.y += 0.5f * gridScale;

        float currWidth = 0;
        float halfWidth = gridScale * columns * 0.5f;

        GameObject obj = MakeDummyMesh(new Vector3(15, 0, 0), 2, 2, 0.2f, 5);


        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                Vector3 spawnPosition = new Vector2((currWidth + columnWidths[x] * 0.5f) * gridScale - halfWidth, startPos.y + y * gridScale);
                float width = columnWidths[x];

                float heightA = puzzlePieceHeights[x][y];       // left side
                float heightB = puzzlePieceHeights[x + 1][y];   // right side

                GameObject pieceInstance;

                // top or bottom edge piece
                if (y == 0)
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "bottom");
                }
                else if (y == rows - 1)
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "top");
                }
                else
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "middle");
                }

            }
            currWidth += columnWidths[x];
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

                Vector3 spawnPosition = new Vector2((currWidth + columnWidths[x] * 0.5f) * gridScale - halfWidth, startPos.y + y * gridScale);
                float width = columnWidths[x];

                float heightA = puzzlePieceHeights[x][y];       // left side
                float heightB = puzzlePieceHeights[x + 1][y];   // right side

                GameObject pieceInstance;

                // top or bottom edge piece
                if (y == 0)
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "bottom");
                } else if (y == rows - 1)
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "top");
                } else
                {
                    pieceInstance = CreatePuzzlePieceMesh(spawnPosition, gridScale, width * 0.5f, heightA, heightB, y, x, "middle");
                }

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

    private Vector3[] FindCoordsBottomPiece(int row, int col, float halfwidth, float heightA, float heightB)
    {
        Vector3[] vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -0.5f, 0),
            new Vector3(halfwidth, -0.5f, 0),
            new Vector3(halfwidth, heightB - 0.5f, 0),
            new Vector3(-halfwidth, heightA - 0.5f, 0),
            };

        return vertices;
    }

    private Vector3[] FindCoordsTopPiece(int row, int col, float halfwidth, float heightA, float heightB)
    {
        Vector3[] vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -heightA + 0.5f, 0),
            new Vector3(halfwidth, -heightB + 0.5f, 0),
            new Vector3(halfwidth, 0.5f, 0),
            new Vector3(-halfwidth, 0.5f, 0),
            };

        return vertices;
    }

    // Find the unit coordinates for a piece in one of the middle rows
    // row must be > 0 and < num rows
    private Vector3[] FindCoordsMiddlePiece(int row, int col, float halfwidth, float heightA, float heightB)
    {
        if (!(row > 0 && row < this.rows))
        {
            return null;
        }

        Vector3[] coords;

        /*float heightA = puzzlePieceHeights[col][row];
        float heightB = puzzlePieceHeights[col + 1][row];*/

        float bLDist = puzzleAccHeights[col][row - 1];          // distance from bottom edge of puzzle to bottom left corner of current piece
        float bRDist = puzzleAccHeights[col + 1][row - 1];      // distance from bottom edge of puzzle to bottom right corner of current piece
        //float tRDist = rows - puzzleAccHeights[col + 1][row];   // distance from top edge of puzzle to top right corner of current piece
        //float tLDist = rows - puzzleAccHeights[col][row];       // distance from top edge of puzzle to top left corner of current piece

        Vector3 bL = new Vector3(-halfwidth, bLDist - row - 0.5f, 0);
        Vector3 bR = new Vector3(halfwidth, bRDist - row - 0.5f, 0);
        Vector3 tR = new Vector3(halfwidth, bR.y + heightB, 0);
        Vector3 tL = new Vector3(-halfwidth, bL.y + heightA, 0);
        //Vector3 tR = new Vector3(halfwidth, this.rows - (row + 1) - tRDist + 0.5f, 0);
        //Vector3 tL = new Vector3(-halfwidth, this.rows - (row + 1) - tLDist + 0.5f, 0);

        UnityEngine.Debug.Log("col, row = " + col + ", " + row + "; " + 
            "bL: " + bL + ", bR: " + bR + ", tR: " + tR + ", tL: " + tL);
        coords = new Vector3[] { bL, bR, tR, tL };

        return coords;
    }

    private GameObject CreatePuzzlePieceMesh(Vector3 position, float scale, float halfwidth, float heightA, float heightB, int row, int col, string type)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.parent = this.transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices;
        if (type.Equals("bottom"))
        {
            vertices = FindCoordsBottomPiece(row, col, halfwidth, heightA, heightB);
        } else if (type.Equals("top"))
        {
            vertices = FindCoordsTopPiece(row, col, halfwidth, heightA, heightB);
        } else
        {
            vertices = FindCoordsMiddlePiece(row, col, halfwidth, heightA, heightB);
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
}
