using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    //public GameObject player;
    private GameObject destination;
    public bool PlayerIsSelected = false;
    bool DestinationIsSelected = false;
    void Update(){
            if(Input.GetMouseButtonDown(0) && PlayerIsSelected == false && OtherIsSelected() == false && ChoosedAsDestination() == false){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit)){
                    //need add other chess_is_jm
                    if(hit.collider.gameObject == this.gameObject){
                        //player = hit.collider.gameObject; 
                        PlayerIsSelected = true;
                        //string objName = player.name;
                        Change_shader();
                        Debug.Log("player " + this.gameObject.name);
                        //Debug.Log(this.gameObject.transform.position);
                
                        //GetLocalPosition();
                        //Debug.Log(PlayerIsSelected);
                        //Debug.Log(DestinationIsSelected);
                    }
                }
                //移動先の判定以外を無視する
            }else if(Input.GetMouseButtonDown(0) && PlayerIsSelected == true && DestinationIsSelected == false && OtherIsSelected() == false){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit)){
                    //need add other chess_is_jm
                    if(hit.collider.gameObject == this.gameObject){
                        //player = hit.collider.gameObject; 
                        PlayerIsSelected = false;
                        //string objName = player.name;
                        Return_shader();
                        Debug.Log("player " + this.gameObject.name + "canceled");
                    }else if((hit.collider.gameObject.tag == "player_white" || hit.collider.gameObject.tag == "player_black" || hit.collider.gameObject.tag == "ClickPlace" ) && hit.collider.gameObject != this.gameObject){
                        destination = hit.collider.gameObject;
                        DestinationIsSelected = true;
                        Debug.Log("destination " + destination.name);
                        Move();
                        Return_shader();   
                    }
                }
                /*
            }else if(Input.GetMouseButtonDown(1) && PlayerIsSelected == true && DestinationIsSelected == false){
                //Debug.Log("1");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "player_white" || hit.collider.gameObject.tag == "player_black" || hit.collider.gameObject.tag == "ClickPlace" ) && hit.collider.gameObject != this.gameObject){
                    destination = hit.collider.gameObject;
                    DestinationIsSelected = true;
                    Debug.Log("destination " + destination.name);
                    Move();
                    Return_shader();         
                }
                */
            }
    }

    #region Move
    void Move(){
        //二段目からの判定が変わる
        Vector3 targetPosition;
        if(destination.tag == "ClickPlace" ){
            targetPosition = new Vector3(destination.transform.position.x, destination.transform.position.y  + 0.5f, destination.transform.position.z);
        }else{
            targetPosition = new Vector3(destination.transform.position.x, destination.transform.position.y  + destination.transform.localScale.y + 0.05f, destination.transform.position.z);
            
        }
        
        transform.position = targetPosition;
        PlayerIsSelected = false;
        DestinationIsSelected = false;
    }
    #endregion

    #region Change_shader
    public Material changed_shader;
    void Change_shader(){
        this.gameObject.GetComponent<MeshRenderer>().material = changed_shader;
    }
    #endregion

    #region Return_shader
    public Material returned_shader;
    void Return_shader(){
        this.gameObject.GetComponent<MeshRenderer>().material = returned_shader;
    }
    #endregion

    #region OtherIsSelected & ChoosedAsDestination
    GameObject[] player_black_chesses;
    GameObject[] player_white_chesses;
    bool OtherIsSelected(){
        player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
        player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
        for(int i = 0; i < player_black_chesses.Length; i++){
            if(player_black_chesses[i] != this.gameObject && player_black_chesses[i].GetComponent<PlayerMove>().PlayerIsSelected == true){    
                return true;
            }
            
        }
        for(int i = 0; i < player_white_chesses.Length; i++){
            if(player_white_chesses[i] != this.gameObject && player_white_chesses[i].GetComponent<PlayerMove>().PlayerIsSelected == true){
                return true;
            }
        }
        return false;
    }

    bool ChoosedAsDestination(){
        player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
        player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
        for(int i = 0; i < player_black_chesses.Length; i++){
            if(player_black_chesses[i] != this.gameObject && player_black_chesses[i].GetComponent<PlayerMove>().destination == this.gameObject){    
                return true;
            }
            
        }
        for(int i = 0; i < player_white_chesses.Length; i++){
            if(player_white_chesses[i] != this.gameObject && player_white_chesses[i].GetComponent<PlayerMove>().destination == this.gameObject){
                return true;
            }
        }
        return false;
    }
    #endregion
    /*
    #region Rule
    void GetLocalPosition(){
        private int localPosition;
        localPosition = GameMain.GetComponent<GameMainScript>().board_state[this.gameObject.transform.position.x, this.gameObject.transform.position.z];
        Debug.Log(localPosition);
    }
    

    #endregion
*/
}




