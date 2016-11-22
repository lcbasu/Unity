using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurve : MonoBehaviour {
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector3> pointsList;
	private Vector3 mousePos;

	private int numberOfPointsBetweenSplinePoints = 10;

	//Store points on the Catmull curve so we can visualize them
	List<Vector3> newPoints = new List<Vector3>();

	//Store all points on the Catmull curve
	List<Vector3> allSplinePoints = new List<Vector3>();

	//set from 0-1
	public float alpha = 0.5f;

	private Vector3 rotationAxis = new Vector3 (0.0f, 0.0f, 1.0f);
	private float rotationAngle = 45.0f;
	private int steps = 5;

	//	-----------------------------------	
	void Awake() {
		// Create line renderer component and set its property
		line = gameObject.AddComponent<LineRenderer>();
		line.material =  new Material(Shader.Find("Particles/Additive"));
		line.SetVertexCount(0);
		line.SetWidth(.2f,0.2f);
		line.SetColors(Color.red, Color.red);
		line.useWorldSpace = true;	
		isMousePressed = false;
		pointsList = new List<Vector3>();
	}
	//	-----------------------------------	
	void Update () {
		// If mouse button down, remove old line and set its color to green
		if(Input.GetMouseButtonDown(0)) {
			isMousePressed = true;
			line.SetVertexCount(0);
			pointsList.RemoveRange(0, pointsList.Count);
			allSplinePoints.RemoveRange(0, allSplinePoints.Count);
			line.SetColors(Color.red, Color.red);
		}
		else if(Input.GetMouseButtonUp(0)) {
			isMousePressed = false;
			if (pointsList.Count >= 4) {
				pointsList.Add (pointsList [0]);
				pointsList.Add (pointsList [1]);
				pointsList.Add (pointsList [2]);

				// Drawing Catmull curve between all the points
				for (int i = 0; i < pointsList.Count - 3; i++) { // only  run the loop till (count - 4) element 
					addPointsOnCatmullCurve (pointsList [i], pointsList [i + 1], pointsList [i + 2], pointsList [i + 3]);
				}

				line.SetVertexCount (allSplinePoints.Count - 1);
				for (int i = 0; i < allSplinePoints.Count - 1; i++) {
					line.SetPosition (i, allSplinePoints [i]);
				}
					
//				for (int i = 0; i < allSplinePoints.Count - 1; i++) {
//					Vector3 oldPointVector = allSplinePoints [i];
//					Vector3 newPointVector = Quaternion.AngleAxis(rotationAngle, rotationAxis) * oldPointVector;
//					line.SetPosition (i, newPointVector);
//				}
//
				// Mesh surface

				List<Vector3> pointsOnSurface = new List<Vector3>();

				for (int i = 0; i < allSplinePoints.Count; i++) {
					pointsOnSurface.Add (allSplinePoints[i]);
				}
					
				float incrementalAngle = rotationAngle/steps;//Time.deltaTime;
				while (incrementalAngle <= rotationAngle) {
					for (int i = 0; i < allSplinePoints.Count; i++) {
						Vector3 oldPointVector = allSplinePoints [i];
						Vector3 newPointVector = Quaternion.AngleAxis(rotationAngle*steps, rotationAxis) * oldPointVector;
						pointsOnSurface.Add (newPointVector);
					}
					incrementalAngle += rotationAngle/steps;//Time.deltaTime;
				}

				Debug.Log ("allSplinePoints.Count: " + allSplinePoints.Count);
				Debug.Log ("pointsOnSurface.Count: " + pointsOnSurface.Count);

				// Start drawing traingles
				gameObject.AddComponent<MeshFilter>();
				gameObject.AddComponent<MeshRenderer>();
				Mesh mesh = GetComponent<MeshFilter>().mesh;

				mesh.Clear();

//				// make changes to the Mesh by creating arrays which contain the new values
//				mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0)};
//				int[] triangles = {0, 1, 2, 2, 3, 0};
//				mesh.triangles = triangles;

				mesh.vertices = pointsOnSurface.ToArray();
				List<int> triangles = new List<int>();

				int totalPointOnSlpine = allSplinePoints.Count;
				int height = 0;
				while (height < steps) {
					for (int i = 0; i < totalPointOnSlpine-1; i++) {
						int index = i + totalPointOnSlpine * height;

						int indexP0 = index;
						int indexP1 = index + 1;
						int indexP2 = index + 1 + totalPointOnSlpine;
						int indexP3 = index + totalPointOnSlpine;
						// Traingle 1
						triangles.Add (indexP0);
						triangles.Add (indexP1);
						triangles.Add (indexP2);

						// Triangle 2
						triangles.Add (indexP2);
						triangles.Add (indexP3);
						triangles.Add (indexP0);
					}
					height += 1;
				}
				mesh.triangles = triangles.ToArray ();

			} else {
				Debug.Log ("Not enough point to draw catmull spline");
			}
		}
		// Drawing line when mouse is moving(presses)
		// The points are pushed each frame
		if(isMousePressed) {
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z=0;
			if (!pointsList.Contains (mousePos)) {
				pointsList.Add (mousePos);
				line.SetVertexCount (pointsList.Count);
				line.SetPosition (pointsList.Count - 1, (Vector3)pointsList [pointsList.Count - 1]);
			}
		}
	}

	void drawCatmullSpline() {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPositions(allSplinePoints.ToArray());
	}

	void addPointsOnCatmullCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {

		float t0 = 0.0f;
		float t1 = GetT(t0, p0, p1);
		float t2 = GetT(t1, p1, p2);
		float t3 = GetT(t2, p2, p3);

		float amountOfPoints = numberOfPointsBetweenSplinePoints;

		for(float t=t1; t<t2; t+=((t2-t1)/amountOfPoints))
		{
			Vector3 A1 = (t1-t)/(t1-t0)*p0 + (t-t0)/(t1-t0)*p1;
			Vector3 A2 = (t2-t)/(t2-t1)*p1 + (t-t1)/(t2-t1)*p2;
			Vector3 A3 = (t3-t)/(t3-t2)*p2 + (t-t2)/(t3-t2)*p3;

			Vector3 B1 = (t2-t)/(t2-t0)*A1 + (t-t0)/(t2-t0)*A2;
			Vector3 B2 = (t3-t)/(t3-t1)*A2 + (t-t1)/(t3-t1)*A3;

			Vector3 C = (t2-t)/(t2-t1)*B1 + (t-t1)/(t2-t1)*B2;

			allSplinePoints.Add(C);
		}
	}

	Vector3 centerOfCatmullSpline() {
		Vector3 center = new Vector3 (0.0f, 0.0f, 0.0f);
		float minX = float.MaxValue;
		float maxX = float.MinValue;
		float minY = float.MaxValue;
		float maxY = float.MinValue;

		for(int i = 0; i < allSplinePoints.Count; i++) {
			Vector3 temp = allSplinePoints [i];
			if (temp.x > maxX) {
				maxX = temp.x;
			}
			if (temp.x < minX) {
				minX = temp.x;
			}
			if (temp.y > maxY) {
				maxY = temp.y;
			}
			if (temp.y < minY) {
				minY = temp.y;
			}
		}

		center.x = (minX + maxX) / 2;
		center.y = (minY + maxY) / 2;

		return center;
	}

	void CatmulRom()
	{
		newPoints.Clear();

		Vector3 p0 = new Vector3(pointsList[0].x, pointsList[0].y, 0.0f);
		Vector3 p1 = new Vector3(pointsList[1].x, pointsList[1].y, 0.0f);
		Vector3 p2 = new Vector3(pointsList[2].x, pointsList[2].y, 0.0f);
		Vector3 p3 = new Vector3(pointsList[3].x, pointsList[3].y, 0.0f);

		float t0 = 0.0f;
		float t1 = GetT(t0, p0, p1);
		float t2 = GetT(t1, p1, p2);
		float t3 = GetT(t2, p2, p3);

		float amountOfPoints = numberOfPointsBetweenSplinePoints;

		for(float t=t1; t<t2; t+=((t2-t1)/amountOfPoints))
		{
			Vector3 A1 = (t1-t)/(t1-t0)*p0 + (t-t0)/(t1-t0)*p1;
			Vector3 A2 = (t2-t)/(t2-t1)*p1 + (t-t1)/(t2-t1)*p2;
			Vector3 A3 = (t3-t)/(t3-t2)*p2 + (t-t2)/(t3-t2)*p3;

			Vector3 B1 = (t2-t)/(t2-t0)*A1 + (t-t0)/(t2-t0)*A2;
			Vector3 B2 = (t3-t)/(t3-t1)*A2 + (t-t1)/(t3-t1)*A3;

			Vector3 C = (t2-t)/(t2-t1)*B1 + (t-t1)/(t2-t1)*B2;

			newPoints.Add(C);
		}
	}

	float GetT(float t, Vector3 p0, Vector3 p1)
	{
		float a = Mathf.Pow((p1.x-p0.x), 2.0f) + Mathf.Pow((p1.y-p0.y), 2.0f) + Mathf.Pow((p1.z-p0.z), 2.0f);
		float b = Mathf.Pow(a, 0.5f);
		float c = Mathf.Pow(b, alpha);

		return (c + t);
	}

	//Visualize the catmull spline points
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach(Vector3 temp in allSplinePoints)
		{
			Vector3 pos = new Vector3(temp.x, temp.y, 0);
			Gizmos.DrawSphere(pos, 0.3f);
		}
	}
}