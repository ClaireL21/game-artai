using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Bezier_Viz : MonoBehaviour
{

    public List<Vector2> ControlPoints = new List<Vector2>
    {
        new Vector2(-5.0f, 3.0f),
        new Vector2(0.0f, 10.0f),
        new Vector2(5.0f, 6.0f)

    };

    public GameObject PointPrefab;

    LineRenderer[] lineRenderers = null;
    List<GameObject> pointGameObjs = new List<GameObject>();

    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    //public GameObject panel;

    private LineRenderer CreateLine()
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = LineColor;
        lr.endColor = LineColor;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;
        return lr;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderers = new LineRenderer[2];
        lineRenderers[0] = CreateLine();
        lineRenderers[1] = CreateLine();

        lineRenderers[0].gameObject.name = "line_obj_0";
        lineRenderers[1].gameObject.name = "line_obj_1";

        //float uiScale = 100f;

        for (int i = 0; i < ControlPoints.Count; i++)
        {
            //Vector3 worldPos = new Vector3(ControlPoints[i].x, ControlPoints[i].y, 0f);
            GameObject obj = Instantiate(PointPrefab, ControlPoints[i], Quaternion.identity);
            obj.name = "CtrlPt_" + i.ToString();
            pointGameObjs.Add(obj);

            //GameObject obj = Instantiate(Point, panel.transform); // Parent it to the panel

            //RectTransform rt = obj.GetComponent<RectTransform>();
            //if (rt != null)
            //{
            //    rt.anchoredPosition = ControlPoints[i] * uiScale; // Set UI-local position
            //}

            //obj.name = "CtrlPt_" + i.ToString();
            //pointGameObjs.Add(obj);
        }

    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = lineRenderers[0];
        LineRenderer curveRenderer = lineRenderers[1];

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < pointGameObjs.Count; i++)
        {
            pts.Add(pointGameObjs[i].transform.position);
        }

        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; i++)
        {
            lineRenderer.SetPosition(i, pts[i]);
        }

        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        curveRenderer.startColor = BezierCurveColor;
        curveRenderer.endColor = BezierCurveColor;
        curveRenderer.positionCount = curve.Count;
        curveRenderer.startWidth = LineWidthBezier;
        curveRenderer.endWidth = LineWidthBezier;

        for (int i = 0; i < curve.Count; i++)
        {
            curveRenderer.SetPosition(i, curve[i]); 
        }
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector2 rayPos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                InsertNewControlPoint(rayPos);
            }
        }
    }

    void InsertNewControlPoint(Vector2 p)
    {
        if (pointGameObjs.Count >= 18)
        {
            //Debug.Log("Cannot create any new control points. Max number is 18");
            return;
        }
        GameObject obj = Instantiate(PointPrefab, p, Quaternion.identity);
        obj.name = "ControlPoint_" + pointGameObjs.Count.ToString();
        pointGameObjs.Add(obj);
    }

}
