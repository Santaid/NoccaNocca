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
	private List<int[]> canPut;
	void Update(){
		if(GameMainScript.instance.End){
			return;
		}

		if(Input.GetMouseButtonDown(0) && !PlayerIsSelected && !OtherIsSelected() && !ChoosedAsDestination() && isTurn()){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				//need add other chess_is_jm
				if(hit.collider.gameObject == this.gameObject){
					//player = hit.collider.gameObject; 
					PlayerIsSelected = true;
					//string objName = player.name;
					Change_shader();
					// Debug.Log("player " + this.gameObject.name);
					//Debug.Log(this.gameObject.transform.position);	
					//GetLocalPosition();
					//Debug.Log(PlayerIsSelected);
					//Debug.Log(DestinationIsSelected);
				}
			}
		//移動先の判定以外を無視する
		}else if(Input.GetMouseButtonDown(0) && PlayerIsSelected && !DestinationIsSelected && !OtherIsSelected()){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				//need add other chess_is_jm
				if(hit.collider.gameObject == this.gameObject){
					//player = hit.collider.gameObject; 
					PlayerIsSelected = false;
					//string objName = player.name;
					Return_shader();
					// Debug.Log("player " + this.gameObject.name + "canceled");
				}else if((hit.collider.gameObject.tag == "player_white" || hit.collider.gameObject.tag == "player_black" || hit.collider.gameObject.tag == "ClickPlace" ) && hit.collider.gameObject != this.gameObject){
					destination = hit.collider.gameObject;
					// 移動先が合法かどうか
					foreach (int[] item in canPut){
						// Debug.Log("canput: "+item[0]+","+item[1]);
						if(item[0]==(int)destination.transform.position.x && item[1]==(int)destination.transform.position.z){
							DestinationIsSelected = true;
							// Debug.Log("destination " + destination.name);
							Move();
							Return_shader();
							GameMainScript.instance.changeTurn();
							GameMainScript.instance.isEnd();
							return;
						}
					}
					PlayerIsSelected=false;
					Return_shader();
				}
			}
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
		// ボード情報の書き換え
		GameMainScript.instance.move((int)transform.position.x,(int)transform.position.z,(int)destination.transform.position.x,(int)destination.transform.position.z);
		transform.position = targetPosition;
		PlayerIsSelected = false;
		DestinationIsSelected = false;
	}
	#endregion

	#region Change_shader
	public Material changed_shader;
	void Change_shader(){
		// 選択した駒を色変え
		this.gameObject.GetComponent<MeshRenderer>().material = changed_shader;
		// 動ける場所を色変え
		canPut=GameMainScript.instance.canMove((int)this.transform.position.x,(int)this.transform.position.z);
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

	#region Rule
	// 選択した駒がターンと一致するか
	bool isTurn(){
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
}




