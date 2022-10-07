using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
	//public GameObject player;
	public GameObject destination;
	public bool PlayerIsSelected = false;
	public bool DestinationIsSelected = false;
	void Update(){       
		if(GameMainScript.instance.End){
			return;
		}
        
		if(Input.GetMouseButtonDown(0) && PlayerIsSelected == false && OtherIsSelected() == false && ChoosedAsDestination() == false && isTurn() == true){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				//need add other chess_is_jm
				if(hit.collider.gameObject == this.gameObject){
					//player = hit.collider.gameObject; 
					PlayerIsSelected = true;
					//string objName = player.name;
					IfCanBeChoosedAsDestination();
                    //Debug.Log("player " + this.gameObject.name);
					Change_shader();
				    
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
					Array.Clear(moveableObjs, 0, movePlace);
					movePlace = 0;
					//Debug.Log("player " + this.gameObject.name + "canceled");
				}else if(/*(hit.collider.gameObject.tag == "ClickPlace" || hit.collider.gameObject.tag == "player_black" || hit.collider.gameObject.tag =="player_white")*/ IsInMoveableObjs(hit.collider.gameObject) && hit.collider.gameObject != this.gameObject){ // 移動先が合法かどうか
					destination = hit.collider.gameObject;
					DestinationIsSelected = true;
					//Debug.Log("destination " + destination.name);
					Move();
					Return_shader();
					Array.Clear(moveableObjs, 0, movePlace);
					movePlace = 0;
					GameMainScript.instance.changeTurn();
					GameMainScript.instance.isEnd();
					PlayerIsSelected=false;
				}
			}
		}
	}

	#region Move
	public void Move(){
		//二段目からの判定が変わる
		Vector3 targetPosition;
		if(destination.tag == "ClickPlace" ){
			targetPosition = new Vector3(destination.transform.position.x, destination.transform.position.y  + 0.5f, destination.transform.position.z);
		}else{
			targetPosition = new Vector3(destination.transform.position.x, destination.transform.position.y  + destination.transform.localScale.y + 0.05f, destination.transform.position.z);
		}
		// ボード情報の書き換え
		GameMainScript.instance.move((int)transform.position.x,(int)transform.position.z,(int)destination.transform.position.x,(int)destination.transform.position.z);
		transform.position = targetPosition;
		PlayerIsSelected = false;
		DestinationIsSelected = false;
	}
	#endregion

	#region Change_shader
	public Material changed_shader;
    public Material destination_changed_shader;
	public void Change_shader(){
		// 選択した駒を色変え
		this.gameObject.GetComponent<MeshRenderer>().material = changed_shader;
		// 動ける場所を色変え
        
        for(int i = 0; i < movePlace; i++){
            moveableObjs[i].GetComponent<MeshRenderer>().material = destination_changed_shader;
        }
        //moveableObjs = null;
        

	}
	#endregion

	#region Return_shader
	public Material returned_shader;
    public Material white;
    public Material black;
    public Material clickPlace;

	public void Return_shader(){
		if(this.gameObject.tag == "player_black"){
            this.gameObject.GetComponent<MeshRenderer>().material = black;
        }else if(this.gameObject.tag == "player_white"){
            this.gameObject.GetComponent<MeshRenderer>().material = white;
        }

        for(int i = 0; i < movePlace; i++){
            if(moveableObjs[i].tag == "player_black"){
                moveableObjs[i].GetComponent<MeshRenderer>().material = black;
            }else if(moveableObjs[i].tag == "player_white"){
                moveableObjs[i].GetComponent<MeshRenderer>().material = white;
            }else if(moveableObjs[i].tag == "ClickPlace"){
                moveableObjs[i].GetComponent<MeshRenderer>().material = clickPlace;
            }
        }
	}
	#endregion

	#region OtherIsSelected & ChoosedAsDestination
	GameObject[] player_black_chesses;
	GameObject[] player_white_chesses;
	public bool OtherIsSelected(){
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

	public bool ChoosedAsDestination(){
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

	#region Rule
	// 選択した駒がターンと一致するか
	public bool isTurn(){
		int turn=0;
		if(this.gameObject.tag=="player_black"){
			turn=GameMainScript.instance.Black;
		}
		if(this.gameObject.tag=="player_white"){
			turn=GameMainScript.instance.White;
		}
		return (turn==GameMainScript.instance.Turn);
	}

	#endregion

    #region Moveable
    public GameObject[] moveableObjs = new GameObject[8];
    GameObject[] ClickPlace;

    public int movePlace = 0;
    public void IfCanBeChoosedAsDestination(){
        player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
        ClickPlace =  GameObject.FindGameObjectsWithTag("ClickPlace");
		for(int i=-1; i<=1; i++){ //行
			for(int j=-1; j<=1; j++){ //列
                if(!(GameMainScript.instance.Turn == GameMainScript.instance.Black && this.gameObject.transform.position.z + j == 0) && !(GameMainScript.instance.Turn == GameMainScript.instance.White && this.gameObject.transform.position.z + j == GameMainScript.instance.line + 1)){
				    if(!(i==0 && j==0)){
						if(GameMainScript.instance.board_state[(int)this.gameObject.transform.position.x + i, (int)this.gameObject.transform.position.z + j] == 0){
                            foreach(GameObject obj in ClickPlace){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    moveableObjs[movePlace] = obj;
                                    movePlace++;
                                }
                            }
                        }else if(GameMainScript.instance.board_state[(int)this.gameObject.transform.position.x + i, (int)this.gameObject.transform.position.z + j] <= 2){
                            foreach(GameObject obj in player_black_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.0f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    moveableObjs[movePlace] = obj;
                                    movePlace++;
                                }
                            }
                            foreach(GameObject obj in player_white_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.0f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    moveableObjs[movePlace] = obj;
                                    movePlace++;
                                }
                            }
                        }else if(GameMainScript.instance.board_state[(int)this.gameObject.transform.position.x + i, (int)this.gameObject.transform.position.z + j] <= 6){
                            foreach(GameObject obj in player_black_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.85f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    moveableObjs[movePlace] = obj;
                                    movePlace++;
                                }
                            }
                            foreach(GameObject obj in player_white_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.85f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    moveableObjs[movePlace] = obj;
                                    movePlace++;
                                }
                            }
                        }
					}
				}
			}
		}
		/*
		for(int i = movePlace; i < 8; i++){
			moveableObjs[i] = null;
		}
		*/
        //Debug.Log(movePlace);
        //movePlace = 0;
	}
    #endregion

	#region IsInMoveableObjs
	public bool IsInMoveableObjs(GameObject obj){
		for(int i = 0; i < movePlace; i++){
			if(obj == moveableObjs[i]){
				return true;
			}
		}
		return false;
	}
	#endregion
}