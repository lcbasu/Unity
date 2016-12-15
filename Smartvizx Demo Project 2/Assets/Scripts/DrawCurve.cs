using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurve : MonoBehaviour {
	private LineRenderer line;
    private bool isMousePressed;
	private Vector3 mousePos;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 middlePoint;
    private List<Vector3> pointsList;
    int count;
    float a, b, c, d, l, m, n, k, p, D, q, r, s, x, y, z;

	//	---------------------- -------------	
	void Awake() 
    {
		// Create line renderer component and set its property
		line = gameObject.AddComponent<LineRenderer>();
		line.material =  new Material(Shader.Find("Particles/Additive"));
		line.SetVertexCount(0);
		line.SetWidth(.2f,0.2f);
		line.SetColors(Color.red, Color.red);
		line.useWorldSpace = true;
        pointsList = new List<Vector3>();
		isMousePressed = false;
        count = 0;
	}
	//	-----------------------------------	
	void Update () {
		// If mouse button down, remove old line and set its color to green
		if(Input.GetMouseButtonDown(0)) 
        {
			isMousePressed = true;
		}
		else if(Input.GetMouseButtonUp(0)) 
        {
			isMousePressed = false;
            count += 1;
		}
		// Drawing line when mouse is moving(presses)
		// The points are pushed each frame
		if(isMousePressed) {
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z=0;
            Debug.Log("count: " + count);

            if(count == 0)
            {
                startPoint = mousePos;
            }

            if (count == 1)
            {
                endPoint = mousePos;
            }

            if (count > 1)
            {
                middlePoint = mousePos;
                // check if points lie on the same line
                if(liesOnTheSameLine())
                {
                    Debug.Log("Lies on the same line");
                }
                else
                {
                    Debug.Log("Do not lie on the same line");
                }

                pointsList.RemoveRange(0, pointsList.Count);

                pointsList.Add(startPoint);

                for (float t = 0; t <= 1; t += 0.1f)
                { // only  run the loop till (count - 4) element 
                    Vector3 pointOnCurve = findPointOnParametricCurve(t);
                    pointsList.Add(pointOnCurve);
                    Debug.Log("t: " + t + " -> pointOnCurve: " + pointOnCurve);
                }

                pointsList.Add(endPoint);

                line.SetVertexCount(pointsList.Count);
                line.SetPositions(pointsList.ToArray());
            }
            Debug.Log("startPoint: " + startPoint + " middlePoint: " + middlePoint + " endPoint: " + endPoint + " a, b, c: " + x + " " + y + " " + " " + z);
		}
	}

    Vector3 findPointOnCurve(float t)
    {

        Vector3 P1 = startPoint;
        Vector3 P2 = middlePoint;
        Vector3 P3 = endPoint;

        findConstants();

        // y = A*x^2 + B*x + C
        float A = x;
        float B = y;
        float C = z;

        Vector3 point = new Vector3();

        point.x = (P3.x - P2.x) * t + P1.x;

        point.y = A * (point.x * point.x) + B * point.x + C;

        point.z = 0;

        return point;
    }

    void findConstants()
    {
        Vector3 P1 = startPoint;
        Vector3 P2 = middlePoint;
        Vector3 P3 = endPoint;

        a = P1.x * P1.x;
        b = P1.x;
        c = 1;
        d = -P1.y;

        l = P2.x * P2.x;
        m = P2.x;
        n = 1;
        k = -P2.y;

        p = P3.x * P3.x;
        q = P3.x;
        r = 1;
        s = -P3.y;

        D = (a * m * r + b * p * n + c * l * q) - (a * n * q + b * l * r + c * m * p);
        x = ((b * r * k + c * m * s + d * n * q) - (b * n * s + c * q * k + d * m * r)) / D;
        y = ((a * n * s + c * p * k + d * l * r) - (a * r * k + c * l * s + d * n * p)) / D;
        z = ((a * q * k + b * l * s + d * m * p) - (a * m * s + b * p * k + d * l * q)) / D;
    }

    bool liesOnTheSameLine()
    {
        Vector3 A = startPoint;
        Vector3 B = middlePoint;
        Vector3 C = endPoint;

        float areaDouble = A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y);

        if(areaDouble == 0)
        {
            return true;
        }
        return false;
    }


    Vector3 findPointOnParametricCurve(float t)
    {
        Vector3 P0 = startPoint;
        Vector3 P1 = middlePoint;
        Vector3 P2 = endPoint;

        Vector3 a0 = P0;
        Vector3 a2 = -1*(1 / 0.25f) * ((P1 - P0) - 0.5f * (P2 - P0)); //(1 / (2 * t * (t - 1) + 0.5f)) * ((P2 - P0) + 2 * P0);
        Vector3 a1 = P2 - P0 - a2;

        Vector3 point = a2 * (t * t) + a1 * t + a0;

        return point;
    }
}