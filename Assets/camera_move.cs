using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_move : MonoBehaviour
{
    public enum MouseState{
        None,
        MidMouseBtn,
        LeftMouseBtn
    }

    private MouseState mMouseState = MouseState.None;
    private Camera mCamera;
    
    private void Awake(){
        mCamera = GetComponent<Camera>();
        if(mCamera == null){
            Debug.LogError(GetType() + "camera get error ...");
        }
        GetDefaultFov();
    }

    private void LateUpdate(){
        CameraRotate();
        CameraFov();
        CameraMove();
    }

    #region Camera Rotation

    //Rotate MAX angle
    //public int yRotationMinLimit = -20;
    //public int yRotationMaxLimit = 80;
    //rotate speed
    //public float xRotationSpeed = 250.0f;
    //public float yRotationSpeed = 120.0f;
    public float zRotationSpeed = 250.0f;
    //rotate angle
    //private float xRotation = 0.0f;
    //private float yRotation = 0.0f;
    private float zRotation = 0.0f;

    void CameraRotate(){
        if(Input.GetMouseButton(0)){ //左クリックを押してる間
            //Input.GetAxis("Mouse X") Mouseがｘ軸に移動する距離
            //xRotation -= Input.GetAxis("Mouse X") * xRotationSpeed * 0.02f;
            //yRotation += Input.GetAxis("Mouse Y") * yRotationSpeed * 0.02f;
            zRotation -= Input.GetAxis("Mouse X") * zRotationSpeed * 0.02f;
            //yRotation = ClampValue(yRotation, yRotationMinLimit,yRotationMaxLimit);　//最後に書いてある関数
                       //オイラー角　-＞ 四元数
            Quaternion rotation = Quaternion.Euler(90, 0, -zRotation);
            transform.rotation = rotation;
        }else if(Input.GetMouseButtonDown(0)){　//左クリックをクリック時
            mMouseState = MouseState.LeftMouseBtn; //mMouseState をLeftMouseBtnに変更
            //Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }else if(Input.GetMouseButtonUp(0)){ //左クリックから離れたときに
            mMouseState = MouseState.None; //mMouseState -> None
            //Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }
    }
    #endregion

    #region Camera fov
    //fov最大最小角度
    public int fovMinLimit = 25;
    public int fovMaxLimit = 100;
    //fov変化速度
    public float fovSpeed = 50.0f;
    //fov角度
    private float fov = 0.0f;

    void GetDefaultFov(){
        fov = mCamera.fieldOfView;
    }

    //scrollでfovを変更
    public void CameraFov(){
        fov -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100 * fovSpeed;

        //fov制限修正
        fov = ClampValue(fov, fovMinLimit, fovMaxLimit);

        //cameraのfovの変更
        mCamera.fieldOfView = (fov);
    }
    #endregion


    #region Camera Move
    float _mouseX = 0;
    float _mouseY = 0;
    public float moveSpeed = 5;
    //scrollで引っ張る
    public void CameraMove(){
        if(Input.GetMouseButton(2)) { //scrollをクリックしている間に
            _mouseX = Input.GetAxis("Mouse X");
            _mouseY = Input.GetAxis("Mouse Y");

        
            //camera位置のoffset
            Vector3 moveDir = (_mouseX * - transform.right + _mouseY * - transform.up);

            //y軸のoffsetを制限
            moveDir.y = 0;
            transform.position += moveDir * 0.5f * moveSpeed; 
        }else if(Input.GetMouseButtonDown(2)){　//scrollをクリック時
            mMouseState = MouseState.MidMouseBtn; //mMouseState をmidMouseBtnに変更
            //Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }else if(Input.GetMouseButtonUp(2)){ //scrollから離れたときに
            mMouseState = MouseState.None; //mMouseState -> None
            //Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }
    }
    #endregion

    #region tools ClampValue

    //値の範囲限定
    float ClampValue(float value, float min, float max){ // rotation角度をコントロール
        if(value < -360){
            value += 360;
        }
        if(value > 360){
            value -= 360;
        }
        return Mathf.Clamp(value, min, max); //valuの値をminとmaxの間に限定。もしvalue < min -> return min;もしvaue > max -> return max.
    }
    #endregion


        
}
