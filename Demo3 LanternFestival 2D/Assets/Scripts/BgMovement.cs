using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D) )]

public class BgMovement : MonoBehaviour {
    float colliderHeight;
    BoxCollider2D m_BoxCollider;

	// Use this for initialization
	void Start () {
        m_BoxCollider = GetComponent<BoxCollider2D>();
        colliderHeight = m_BoxCollider.size.y;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(0,-1f,0)*Time.deltaTime);
        if (transform.position.y < -colliderHeight)
        {
            transform.position = (Vector2)transform.position + new Vector2(0, 2*colliderHeight);
        }
    }
}
