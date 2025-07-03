using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public static class MeshGenerator
{

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
       // return indices.ToArray();
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

    private static void AddTabTB(List<Vector3> vertices, List<int> triangles, float radius, int segments, float hh)
    {
        int centerIndex = vertices.Count;
        Vector3 center = new Vector3(0, hh, 0);

        int direction = hh < 0 ? -1 : 1;

        // Vertices
        vertices.Add(center); // Center of top edge for fan triangles
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

    private static void AddTabLR(List<Vector3> vertices, List<int> triangles, float radius, int segments, float hw)
    {
        int centerIndex = vertices.Count;
        Vector3 center = new Vector3(hw, 0, 0);

        int direction = hw < 0 ? 1 : -1;

        // Vertices
        vertices.Add(center); // Center of top edge for fan triangles
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


    private static Vector2[] verticesToWorld(Vector3[] vertices, Vector3 spawnPosition, float gridScale)
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
    private static Vector2[] worldToUV(Vector2[] screenVertices, int rows, int columns, float gridScale)
    {
        float w = columns * gridScale;
        float h = rows * gridScale;

        Vector2[] uvs = new Vector2[screenVertices.Length];
        for (int i = 0; i < uvs.Length; i++)
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
    public static GameObject MakeDummyTabMesh(Vector3 position, float width, float height, float radius, int segments, 
                                              Transform transform, float gridScale, int rows, int cols, Material mat)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * gridScale;
        obj.transform.parent = transform;

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

        // Step 3: triangles for rectangle
        triangles.AddRange(new int[] { 0, 2, 1, 0, 3, 2 });

        AddTabTB(vertices, triangles, radius, segments, hh);    // up
        AddTabTB(vertices, triangles, radius, segments, -hh);   // down
        AddTabLR(vertices, triangles, radius, segments, hw);    // right
        AddTabLR(vertices, triangles, radius, segments, -hw);   // left

        Vector2[] verts = verticesToWorld(vertices.ToArray(), Vector3.zero, gridScale);
        Vector2[] uvs = worldToUV(verts, rows, cols, gridScale);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs;
        mf.mesh = mesh;
        /*Material defaultMat = new Material(Shader.Find("Unlit/Color"));
        defaultMat.color = Color.white;*/

        mr.material = new Material(mat);

        return obj;
    }

    public static GameObject MakeDummyIndentMesh(Vector3 position, float width, float height, float radius, int segments, 
                                                 PuzzlePiece piece, Transform transform, float gridScale, int rows, int cols, Material mat)
    {
        GameObject obj = new GameObject("Piece");
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * gridScale;
        obj.transform.parent = transform;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> verts2D = new List<Vector2>();

        float hw = width / 2f;
        float hh = height / 2f;

        int tempB = piece.bottom;
        int tempR = piece.right;
        int tempT = piece.top;
        int tempL = piece.left;

        // bottom edge
        vertices.Add(new Vector3(-hw, -hh, 0));

        if (tempB == -1)
        {
            // add vertices for semicircle indent on top edge, clockwise from right to left
            for (int i = 0; i <= segments; i++)
            {
                float theta = Mathf.PI * (i / (float)segments); // 0
                float x = -radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta); // indent goes downward
                vertices.Add(new Vector3(x, -hh + y, 0));
            }
        }

        // add bottom-right vertex
        vertices.Add(new Vector3(hw, -hh, 0));

        // right edge
        if (tempR == -1)
        {
            // add vertices for semicircle indent on top edge, clockwise from right to left
            for (int i = 0; i <= segments; i++)
            {
                float theta = Mathf.PI * (i / (float)segments) + Mathf.PI * 1.5f; // 0
                float x = -(radius * Mathf.Cos(theta));
                float y = radius * Mathf.Sin(theta); // indent goes downward
                vertices.Add(new Vector3(hw + x, y, 0));
            }
        }

        // add top-right vertex
        vertices.Add(new Vector3(hw, hh, 0));

        // top edge
        if (tempT == -1)
        {
            // add vertices for semicircle indent on top edge, clockwise from right to left
            for (int i = 0; i <= segments; i++)
            {
                float theta = Mathf.PI * (i / (float)segments); // 0
                float x = radius * Mathf.Cos(theta);
                float y = -radius * Mathf.Sin(theta); // indent goes downward
                vertices.Add(new Vector3(x, hh + y, 0));
            }
        }


        // add top-left vertex
        vertices.Add(new Vector3(-hw, hh, 0));

        // left edge
        if (tempL == -1)
        {
            // add vertices for semicircle indent on top edge, clockwise from right to left
            for (int i = 0; i <= segments; i++)
            {
                float theta = Mathf.PI * (i / (float)segments) + Mathf.PI / 2; // 0
                float x = -radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta); // indent goes downward
                vertices.Add(new Vector3(-hw + x, y, 0));
            }
        }
        

        // Prepare 2D list for triangulation
        foreach (var v in vertices)
            verts2D.Add(new Vector2(v.x, v.y));

        // Triangulate using helper
        List<int> triangles = Triangulate(verts2D);

        if (tempT == 1)
        {
            AddTabTB(vertices, triangles, radius, segments, hh);    // up
        }
        if (tempB == 1)
        {
            AddTabTB(vertices, triangles, radius, segments, -hh);   // down
        }
        if (tempL == 1)
        {
            AddTabLR(vertices, triangles, radius, segments, -hw);   // left
        }
        if (tempR == 1)
        {
            AddTabLR(vertices, triangles, radius, segments, hw);    // right
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Vector2[] uvs = worldToUV(verticesToWorld(vertices.ToArray(), position, gridScale), rows, cols, gridScale);
        mesh.uv = uvs;

        mf.mesh = mesh;
        mr.material = new Material(mat);

        return obj;
    }
}
