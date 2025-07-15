using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System;
using UnityEngine.SearchService;

public class MeshGenerator
{
    private Transform transform;
    private int rows;
    private int columns;
    private float radius;
    private int segments;
    private float gridScale;
    private Material material;
    private int sort;

    public MeshGenerator(Transform transform, int rows, int columns, float radius, int segments, float gridScale, Material material)
    {
        this.transform = transform;
        this.rows = rows;
        this.columns = columns;
        this.radius = radius;
        this.segments = segments;
        this.gridScale = gridScale;
        this.material = material;
        sort = -1;
    }
    public static List<int> Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();
        int n = points.Count;
        if (n < 3) return indices;

        int[] V = new int[n];
        if (Area(points) > 0)
        {
            for (int v = 0; v < n; v++) V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++) V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0) break;

            int u = v; if (nv <= u) u = 0;
            v = u + 1; if (nv <= v) v = 0;
            int w = v + 1; if (nv <= w) w = 0;

            if (Snip(points, u, v, w, nv, V))
            {
                int a = V[u], b = V[v], c = V[w];
                indices.Add(c);
                indices.Add(b);
                indices.Add(a);
                for (int s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                nv--; count = 2 * nv;
            }
        }
        return indices;
    }

    private static float Area(List<Vector2> points)
    {
        int n = points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = points[p], qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return A * 0.5f;
    }

    private static bool Snip(List<Vector2> points, int u, int v, int w, int n, int[] V)
    {
        Vector2 A = points[V[u]], B = points[V[v]], C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x)))) return false;
        for (int p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w)) continue;
            Vector2 P = points[V[p]];
            if (InsideTriangle(A, B, C, P)) return false;
        }
        return true;
    }

    private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax = C.x - B.x, ay = C.y - B.y;
        float bx = A.x - C.x, by = A.y - C.y;
        float cx = B.x - A.x, cy = B.y - A.y;
        float apx = P.x - A.x, apy = P.y - A.y;
        float bpx = P.x - B.x, bpy = P.y - B.y;
        float cpx = P.x - C.x, cpy = P.y - C.y;

        float aCrossBP = ax * bpy - ay * bpx;
        float bCrossCP = bx * cpy - by * cpx;
        float cCrossAP = cx * apy - cy * apx;

        return (aCrossBP >= 0.0f) && (bCrossCP >= 0.0f) && (cCrossAP >= 0.0f);
    }

    private Vector2[] verticesToWorld(Vector3[] vertices, Vector3 spawnPosition)
    {
        Vector2[] worldVertices = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = new Vector2(vertices[i][0], vertices[i][1]); 
            worldVertices[i] *= this.gridScale;
            worldVertices[i] += new Vector2(spawnPosition[0], spawnPosition[1]);
        }
        return worldVertices;
    }
    private Vector2[] worldToUV(Vector2[] screenVertices)
    {
        float w = this.columns * this.gridScale;
        float h = this.rows * this.gridScale;

        Vector2[] uvs = new Vector2[screenVertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            float x = (screenVertices[i][0] + w * 0.5f) / w;
            float y = (screenVertices[i][1] + h * 0.5f) / h;
            uvs[i] = new Vector2(x, y);
        }

        return uvs;
    }

    private Vector3[] FindCoordsBottomPiece(float halfwidth, float heightA, float heightB)
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

    private Vector3[] FindCoordsTopPiece(float halfwidth, float heightA, float heightB)
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
    private Vector3[] FindCoordsMiddlePiece(int row, int col, float halfwidth, float heightA, float heightB, float[][] puzzleAccHeights)
    {
        if (!(row > 0 && row < this.rows))
        {
            return null;
        }

        Vector3[] coords;

        float bLDist = puzzleAccHeights[col][row - 1];          // distance from bottom edge of puzzle to bottom left corner of current piece
        float bRDist = puzzleAccHeights[col + 1][row - 1];      // distance from bottom edge of puzzle to bottom right corner of current piece

        Vector3 bL = new Vector3(-halfwidth, bLDist - row - 0.5f, 0);
        Vector3 bR = new Vector3(halfwidth, bRDist - row - 0.5f, 0);
        Vector3 tR = new Vector3(halfwidth, bR.y + heightB, 0);
        Vector3 tL = new Vector3(-halfwidth, bL.y + heightA, 0);

        coords = new Vector3[] { bL, bR, tR, tL };

        return coords;
    }

    private Vector2 FindCenterPoint(Vector2 start, Vector2 end)
    {
        float xMid = start.x + Math.Abs((end.x - start.x) / 2.0f);
        float m = (end.y - start.y) / (end.x - start.x);
        Vector2 center = new Vector2(xMid, end.y + m * (xMid - end.x));

        return center;
    }


    // (x-a)^2 + (y-k)^2 = r^2  --> but we always want k = 0
    // y - y1 = m(x - x1)
    private float FindTheta(Vector2 start, Vector2 end)
    {
        float m = (end.y - start.y) / (end.x - start.x);

        float theta = Mathf.Atan(m);
        return theta;
    }

    public GameObject MakeTrapezoidMesh(Vector3 position, Vector3 randPos, float width, float heightA, float heightB, PuzzlePiece piece, int row, int col, float[][] puzzleAccHeights)
    {
        GameObject obj = new GameObject("Piece" + row + col);
        //obj.transform.position = randPos;
        obj.transform.localScale = Vector3.one * this.gridScale;
        obj.transform.parent = this.transform;
        obj.transform.rotation = obj.transform.parent.rotation;
        obj.transform.position = obj.transform.parent.position;
        obj.transform.localPosition = randPos;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        MeshCollider collider = obj.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> verts2D = new List<Vector2>();

        float halfwidth = width / 2f;

        Vector3[] corners;

        if (piece.bottom == 0)
        {
            corners = FindCoordsBottomPiece(halfwidth, heightA, heightB);
        }
        else if (piece.top == 0)
        {
            corners = FindCoordsTopPiece(halfwidth, heightA, heightB);
        }
        else
        {
            corners = FindCoordsMiddlePiece(row, col, halfwidth, heightA, heightB, puzzleAccHeights);
        }

        // bottom edge
        vertices.Add(corners[0]);

        //int direction;
        if (piece.bottom != 0)
        {
            //direction = -tempBottom;
            Vector2 circleCenter = FindCenterPoint(corners[0], corners[1]);
            float thetaOffset = Math.Abs(FindTheta(corners[0], corners[1]));

            if (piece.bottom == -1)
            {
                if (corners[0].y < corners[1].y) thetaOffset = -thetaOffset;

                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= this.segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + thetaOffset; // 0
                    float x = -this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(x, circleCenter.y + y, 0));
                    //Debug.Log("x " + x + "; y " + y);
                }
            } else
            {
                if (corners[0].y < corners[1].y) thetaOffset = -thetaOffset;

                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= this.segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + Mathf.PI - thetaOffset; // 0
                    float x = this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(x, circleCenter.y + y, 0));

                }
            }
            
        }

        // add bottom-right vertex
        vertices.Add(corners[1]);

        // right edge
        if (piece.right != 0)
        {
            if (piece.right == -1)
            {
                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + Mathf.PI * 1.5f; // 0
                    float x = -(this.radius * Mathf.Cos(theta));
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(halfwidth + x, y, 0));
                }
            } else
            {
                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + Mathf.PI * 1.5f; // 0
                    float x = this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(halfwidth + x, y, 0));
                }
            }
        }

        // add top-right vertex
        vertices.Add(corners[2]);

        // top edge
        if (piece.top != 0)
        {
            Vector2 circleCenter = FindCenterPoint(corners[3], corners[2]);
            float thetaOffset = Math.Abs(FindTheta(corners[3], corners[2]));

            if (piece.top == -1)
            {
                if (corners[2].y > corners[3].y) thetaOffset = -thetaOffset;

                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + thetaOffset; // 0
                    float x = this.radius * Mathf.Cos(theta);
                    float y = -this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(x, circleCenter.y + y, 0));
                }
            } else
            {
                if (corners[2].y < corners[3].y) thetaOffset = -thetaOffset;

                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + thetaOffset; // 0
                    float x = this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(x, circleCenter.y + y, 0));
                }
            }
            
        }

        // add top-left vertex
        vertices.Add(corners[3]);

        // left edge
        if (piece.left != 0)
        {
            if (piece.left == -1)
            {
                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + Mathf.PI / 2; // 0
                    float x = -this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(-halfwidth + x, y, 0));
                }
            } else
            {
                // add vertices for semicircle indent on top edge, clockwise from right to left
                for (int i = 0; i <= segments; i++)
                {
                    float theta = Mathf.PI * (i / (float)this.segments) + Mathf.PI / 2; // 0
                    float x = this.radius * Mathf.Cos(theta);
                    float y = this.radius * Mathf.Sin(theta); // indent goes downward
                    vertices.Add(new Vector3(-halfwidth + x, y, 0));
                }
            }
            
        }

        // Prepare 2D list for triangulation
        foreach (var v in vertices)
            verts2D.Add(new Vector2(v.x, v.y));

        // Triangulate using helper
        List<int> triangles = Triangulate(verts2D);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Vector2[] uvs = worldToUV(verticesToWorld(vertices.ToArray(), position));
        mesh.uv = uvs;

        mf.mesh = mesh;
        collider.sharedMesh = mesh;
        mr.material = new Material(this.material);
        mr.sortingOrder = sort;
        mr.sortingLayerName = "PuzzlePieces";
        //this.sort++;

        return obj;
    }

    public GameObject MakeBaseMesh(Vector3 position, float width, float height, Material mat)
    {
        GameObject obj = new GameObject("Base");
        obj.transform.localScale = Vector3.one * this.gridScale;
        obj.transform.parent = this.transform;
        obj.transform.rotation = obj.transform.parent.rotation;
        obj.transform.position = obj.transform.parent.position;
        obj.transform.localPosition = position;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        float halfwidth = width / 2f;
        float halfheight = height / 2f;

        Vector3[] vertices = new Vector3[]
            {
            new Vector3(-halfwidth, -halfheight, 0),
            new Vector3(halfwidth, -halfheight, 0),
            new Vector3(halfwidth, halfheight),
            new Vector3(-halfwidth, halfheight),
            };
        
        // Triangulate using helper
        int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2 
        };
        mesh.vertices = vertices;

        // Prepare 2D list for triangulation
        /*List<Vector2> verts2D = new List<Vector2>();

        foreach (var v in vertices)
            verts2D.Add(new Vector2(v.x, v.y));
        List<int> triangles = Triangulate(verts2D);*/

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        //Vector2[] uvs = worldToUV(verticesToWorld(vertices, position));
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0), // Bottom-left
            new Vector2(1, 0), // Bottom-right
            new Vector2(1, 1), // Top-right
            new Vector2(0, 1)  // Top-left
        };
        mesh.uv = uvs;

        mf.mesh = mesh;
        mr.material = new Material(mat);
        mr.sortingOrder = -2;
        mr.sortingLayerName = "PuzzlePieces";
        //this.sort++;

        return obj;
    }
}
