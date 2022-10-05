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
	[SerializeField] private GameObject ClickPlace_Tile;
	[SerializeField] private GameObject ClickPlace;
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
				}else if((hit.collider.gameObject.CompareTag("ClickPlace") || hit.collider.gameObject.CompareTag("ClickPlace_Tile")) && hit.collider.gameObject != this.gameObject){ // 移動先が合法かどうか
					destination = hit.collider.gameObject;
					DestinationIsSelected = true;
					// Debug.Log("destination " + destination.name);
					Move();
					Return_shader();
					GameMainScript.instance.changeTurn();
					GameMainScript.instance.isEnd();
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
		if(destination.tag == "ClickPlace_Tile" ){
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
		foreach(int[] item in canPut){
			int val=GameMainScript.instance.board_state[item[0],item[1]];
			if(val==0){ // 空白
				Instantiate(ClickPlace_Tile,new Vector3(item[0],0.5f,item[1]),Quaternion.identity);
			}else if(val<=2){ // 一段
				Instantiate(ClickPlace,new Vector3(item[0],1,item[1]),Quaternion.identity);
			}else{ //二段
				Instantiate(ClickPlace,new Vector3(item[0],1.85f,item[1]),Quaternion.identity);
			}
		}
	}
	#endregion

	#region Return_shader
	public Material returned_shader;
	void Return_shader(){
		this.gameObject.GetComponent<MeshRenderer>().material = returned_shader;
		GameObject[] clickPlaces=GameObject.FindGameObjectsWithTag("ClickPlace");
		GameObject[] clickPlaces_Tile=GameObject.FindGameObjectsWithTag("ClickPlace_Tile");
		foreach (GameObject gameObject in clickPlaces){
			Destroy(gameObject);
		}
		foreach (GameObject gameObject in clickPlaces_Tile){
			Destroy(gameObject);
		}
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