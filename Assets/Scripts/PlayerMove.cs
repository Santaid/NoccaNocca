using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
	//public GameObject player;
	public static PlayerMove instance;
	public GameObject destination=null;
	public bool PlayerIsSelected = false;
	public bool DestinationIsSelected = false;
	public bool AIing = false;
	
	public void Awake(){
		if(instance == null){
			instance = this;
		}
	}
	void Update(){     
		if(GameMainScript.instance.End){
			return;
		}
        
		if(Input.GetMouseButtonDown(0) && !PlayerIsSelected && !OtherIsSelected() && !IsDestination() && isTurn() && isTop(this.gameObject) && !AIing){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				//need add other chess_is_jm
				if(hit.collider.gameObject == this.gameObject){
					Debug.Log(this.tag);
					PlayerIsSelected = true;
					ExtractDestination();
					Change_shader();
				}
			}
		//移動先の判定以外を無視する
		}else if(Input.GetMouseButtonDown(0) && PlayerIsSelected && !DestinationIsSelected && !OtherIsSelected() && !AIing){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				if(destinations.Contains(hit.collider.gameObject) && hit.collider.gameObject != this.gameObject){ //移動先が合法かどうか
					destination = hit.collider.gameObject;
					DestinationIsSelected = true;
					Move();
					Return_shader();
					destinations.Clear();
					GameMainScript.instance.changeTurn();
					GameMainScript.instance.isEnd();
					PlayerIsSelected=false;
				}else if(hit.collider.gameObject==this.gameObject){
					PlayerIsSelected = false;
					Return_shader();
					destinations.Clear();
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
		foreach(GameObject obj in destinations){
			obj.GetComponent<MeshRenderer>().material = destination_changed_shader;
		}
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

		foreach(GameObject obj in destinations){
			if(obj.CompareTag("player_black")){
				obj.GetComponent<MeshRenderer>().material=black;
			}else if(obj.CompareTag("player_white")){
				obj.GetComponent<MeshRenderer>().material=white;
			}else if(obj.CompareTag("ClickPlace")){
				obj.GetComponent<MeshRenderer>().material=clickPlace;
			}
		}
	}
	#endregion

	#region OtherIsSelected & Selected
	GameObject[] player_black_chesses;
	GameObject[] player_white_chesses;
	public bool OtherIsSelected(){
		player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
		foreach(GameObject obj in player_black_chesses){
			if(obj != this.gameObject && obj.GetComponent<PlayerMove>().PlayerIsSelected){    
				return true;
			}
		}
		foreach(GameObject obj in player_white_chesses){
			if(obj != this.gameObject && obj.GetComponent<PlayerMove>().PlayerIsSelected){
				return true;
			}
		}
		return false;
	}

	public bool IsDestination(){
		player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
		foreach(GameObject obj in player_black_chesses){
			if(obj != this.gameObject && obj.GetComponent<PlayerMove>().destination == this.gameObject){    
				return true;
			}
		}
		foreach(GameObject obj in player_white_chesses){
			if(obj != this.gameObject && obj.GetComponent<PlayerMove>().destination == this.gameObject){
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
    public List<GameObject> destinations=new List<GameObject>();
    GameObject[] ClickPlace;

    public void ExtractDestination(){
        player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
        ClickPlace =  GameObject.FindGameObjectsWithTag("ClickPlace");
		int boardVal;
		for(int i=-1; i<=1; i++){ //行
			for(int j=-1; j<=1; j++){ //列
                if(!(GameMainScript.instance.Turn == GameMainScript.instance.Black && this.gameObject.transform.position.z + j == 0) && !(GameMainScript.instance.Turn == GameMainScript.instance.White && this.gameObject.transform.position.z + j == GameMainScript.instance.line + 1)){
					if(!(i==0 && j==0)){
						boardVal=GameMainScript.instance.board_state[(int)this.gameObject.transform.position.x + i, (int)this.gameObject.transform.position.z + j];
						if(boardVal == 0){
                            foreach(GameObject obj in ClickPlace){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    destinations.Add(obj);
                                }
                            }
                        }else if(boardVal <= 2){
                            foreach(GameObject obj in player_black_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.0f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    destinations.Add(obj);
                                }
                            }
                            foreach(GameObject obj in player_white_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.0f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    destinations.Add(obj);
                                }
                            }
                        }else if(boardVal <= 6){
                            foreach(GameObject obj in player_black_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.85f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    destinations.Add(obj);
                                }
                            }
                            foreach(GameObject obj in player_white_chesses){
                                if(obj.transform.position.x == this.gameObject.transform.position.x + i && obj.transform.position.y == 1.85f && obj.transform.position.z == this.gameObject.transform.position.z + j){
                                    destinations.Add(obj);
                                }
                            }
                        }
					}
				}
			}
		}
	}
    #endregion

	#region isTop
	// オブジェクトが一番上にあるか
	public bool isTop(GameObject obj){
		int boardVal=GameMainScript.instance.board_state[(int)obj.transform.position.x,(int)obj.transform.position.z];
		if(boardVal<=2){ // 一段以下
			return true;
		}else if(boardVal<=6){ // 二段
			return (obj.transform.position.y==1.85f);
		}else{ //三段
			return (obj.transform.position.y==2.7f);
		}
	}
	#endregion
}