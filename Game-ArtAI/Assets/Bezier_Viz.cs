using System.Collections.Generic;
using UnityEngine;

public class Bezier_Viz : MonoBehaviour
{

    public List<Vector2> ControlPoints = new List<Vector2>
    {
        new Vector2(-5.0f, -5.0f),
        new Vector2(0.0f, 2.0f), 
        new Vector2(5.0f, -2.0f)
    };

    public GameObject Point;

    LineRenderer[] lineRenderer = null;

    List<GameObject> pointGameObjs = new List<GameObject>();

    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

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
        lineRenderer = new LineRenderer[2];
        lineRenderer[0] = CreateLine();
        lineRenderer[1] = CreateLine();

        lineRenderer[0].gameObject.name = "line_obj_0";
        lineRenderer[1].gameObject.name = "line_obj_1";

        for (int i = 0; i < ControlPoints.Count; i++)
        {
            GameObject obj = Instantiate(Point, ControlPoints[i], Quaternion.identity);
            obj.name = "CtrlPt_" + i.ToString();
            pointGameObjs.Add(obj);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
