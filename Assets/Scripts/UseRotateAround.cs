using UnityEngine;

/// <summary>
/// Transform.RotateAroundを用いた円運動
/// </summary>
public class UseRotateAround : MonoBehaviour
{
    // 中心点
    [SerializeField] private Vector3 center = new Vector3(0, 0, 0);

    // 回転軸
    [SerializeField] Vector3 axis1 = Vector3.up;
    [SerializeField] Vector3 axis2;

    // 円運動周期
    [SerializeField] private float period = 2;

		void Start() {
		}
		
    void Update(){
				if (Input.GetKey (KeyCode.A)) {
            this.transform.RotateAround(center, axis1, 360 / period * Time.deltaTime);
        }
				if (Input.GetKey (KeyCode.D)) {
            this.transform.RotateAround(center, axis1, - 360 / period * Time.deltaTime);
        }
				if (Input.GetKey (KeyCode.W)) {
						axis2 = transform.right;
            this.transform.RotateAround(center, axis2, 360 / period * Time.deltaTime);
        }
				if (Input.GetKey (KeyCode.S)) {
						axis2 = transform.right;
            this.transform.RotateAround(center, axis2, - 360 / period * Time.deltaTime);
        }
    }
}
