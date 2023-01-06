// 参考:
// https://yotiky.hatenablog.com/entry/unity_mousedrag-camerarotation3rd

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	[SerializeField] private GameObject centerObject;
	[SerializeField] private Vector2 rotationSpeed=new Vector2(0.08f,0.08f);
	// [SerializeField] private float moveSpeed=5;
	private Vector2 lastMousePosition;

	void Update(){
		// ドラッグによる視点移動
		if (Input.GetMouseButtonDown(2)){
			lastMousePosition = Input.mousePosition;
		}else if (Input.GetMouseButton(2)){
			var newAngle = Vector3.zero;
			newAngle.x = (Input.mousePosition.x - lastMousePosition.x) * rotationSpeed.x;
			newAngle.y = (lastMousePosition.y - Input.mousePosition.y) * rotationSpeed.y;
			this.transform.RotateAround(centerObject.transform.position, Vector3.up, newAngle.x);
			this.transform.RotateAround(centerObject.transform.position, transform.right, newAngle.y);
			lastMousePosition = Input.mousePosition;
		}

		// キーボードによるカメラ移動
		// if(Input.GetKey(KeyCode.A)){
		//     this.transform.position -= this.transform.right * moveSpeed * Time.deltaTime;
		// }
		// if(Input.GetKey(KeyCode.D)){
		//     this.transform.position += this.transform.right * moveSpeed * Time.deltaTime;
		// }
		// if(Input.GetKey(KeyCode.W)){
		// 	this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
		// }
		// if(Input.GetKey(KeyCode.S)){
		// 	this.transform.position -= this.transform.forward * moveSpeed * Time.deltaTime;
		// }
		// if(Input.GetKey(KeyCode.LeftShift)){
		// 	this.transform.position -= this.transform.up * moveSpeed * Time.deltaTime;
		// }
		// if(Input.GetKey(KeyCode.Space)){
		// 	this.transform.position += this.transform.up * moveSpeed * Time.deltaTime;
		// }
	}
}