using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurve : MonoBehaviour {
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector3> pointsList;
	private Vector3 mousePos;

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
		//		renderer.material.SetTextureOffset(
	}
	//	-----------------------------------	
	void Update () {
		// If mouse button down, remove old line and set its color to green
		if(Input.GetMouseButtonDown(0)) {
			isMousePressed = true;
			line.SetVertexCount(0);
			pointsList.RemoveRange(0,pointsList.Count);
			line.SetColors(Color.red, Color.red);
		}
		else if(Input.GetMouseButtonUp(0)) {
			isMousePressed = false;
			for(int i = 0; i < pointsList.Count; i++)
			{
				Debug.Log("pointsList: " + pointsList[i]);
			}
		}
		// Drawing line when mouse is moving(presses)
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
}