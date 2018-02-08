using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public float turnSpeed = 3f;  
    public Transform target;
    private Vector3 mousePos;

	// Use this for initialization
	void Start () {
        /*
        Vector3 to = target.position - transform.position;
        to.Normalize();
        Vector3 from = transform.right;
        float angle = Vector3.Angle(from, to);
        transform.localEulerAngles = new Vector3(0, 0, angle); 
        */
	}
	

	void Update () {
        /*

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-Camera.main.transform.position.z));  
        Vector3 direction = worldPos-transform.position;  
        direction.z=0f;  
        direction.Normalize();  
  
        float targetAngle = Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;  
        transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.Euler( 0, 0, targetAngle ), turnSpeed * Time.deltaTime );  
        */

        //Vector3 direction = vecB - vecA;                                    ///< 终点减去起点（AB方向与X轴的夹角）  
        Vector3 direction = target.position - transform.position;                                  ///< （BA方向与X轴的夹角）  
        float angle = Vector3.Angle(direction, Vector3.right);              ///< 计算旋转角度  
        direction = Vector3.Normalize(direction);                           ///< 向量规范化  
        float dot = Vector3.Dot(direction, Vector3.up);                  ///< 判断是否Vector3.right在同一方向  
        if (dot < 0)  
            angle = 360 - angle;

        transform.localEulerAngles = new Vector3(0, 0, angle);
	}
}
