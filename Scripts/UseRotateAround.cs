using UnityEngine;

/// <summary>
/// Transform.RotateAroundを用いた円運動
/// </summary>
public class UseRotateAround : MonoBehaviour{
	// 中心対象のオブジェクト
	[SerializeField] private GameObject centerObject;

	// 回転軸
	Vector3 axis1 = Vector3.up;
	Vector3 axis2;

	// 円運動周期
	[SerializeField] private float period = 2;

	void Update(){
		if (Input.GetKey (KeyCode.A)) {
			this.transform.RotateAround(centerObject.transform.position, axis1, 360 / period * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			this.transform.RotateAround(centerObject.transform.position, axis1, - 360 / period * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.W)) {
			axis2 = transform.right;
			this.transform.RotateAround(centerObject.transform.position, axis2, 360 / period * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.S)) {
			axis2 = transform.right;
			this.transform.RotateAround(centerObject.transform.position, axis2, - 360 / period * Time.deltaTime);
		}
	}
}
