using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	public Transform point1;
	public Transform point2;
	public Transform point3;
	public LineRenderer testLine;

	// Use this for initialization
	void Start () {


		
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < 100; i++) {
			Vector3 point = beisaier (point1.position,point2.position,point3.position,(float)(i*0.01));
			testLine.SetPosition (i, point);
		}
	}

	private Vector3 beisaier(Vector3 p1, Vector3 p2,Vector3 p3,float t){
		Vector3 f = Vector3.zero;
		float t1 = (1 - t) * (1 - t);
		float t2 = t * (1 - t);
		float t3 = t * t;
		f = p1 * t1 + 2 * t2 * p2 + t3 * p3;
		return f;
	}


}
