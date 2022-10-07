using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove2 : MonoBehaviour
{
	//public GameObject player;
	private GameObject destination;
	private bool PlayerIsSelected = false;
	private bool DestinationIsSelected = false;
	private List<int[]> canPut;
	void Update(){
		if(GameMainScript.instance.End){
			return;
		}

		if(Input.GetMouseButtonDown(0) && !PlayerIsSelected && !OtherIsSelected() && !ChoosedAsDestination() && isTurn() && isTop()){
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
				}else if((hit.collider.gameObject.CompareTag("ClickPlace_Black") || hit.collider.gameObject.CompareTag("ClickPlace_White") || hit.collider.gameObject.CompareTag("ClickPlace_Tile")) && hit.collider.gameObject != this.gameObject){ // 移動先が合法かどうか
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
		if(destination.CompareTag("ClickPlace_Tile") ){
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
	[SerializeField] private Material changed_shader;
	[SerializeField] private Material clickPlace;
	private GameObject[] tiles;
	void Change_shader(){
		// 選択した駒を色変え
		this.gameObject.GetComponent<MeshRenderer>().material = changed_shader;
		// 動ける場所を色変え
		canPut=GameMainScript.instance.canMove((int)this.transform.position.x,(int)this.transform.position.z);
		tiles=GameObject.FindGameObjectsWithTag("Tile");
		foreach(int[] item in canPut){
			foreach(GameObject obj in tiles){
				if((int)obj.transform.position.x==item[0] && (int)obj.transform.position.z==item[1] && GameMainScript.instance.board_top[item[0],item[1]]==0){
					obj.GetComponent<MeshRenderer>().material=clickPlace;
					obj.tag="ClickPlace_Tile";
					break;
				}
			}
			foreach(GameObject obj in player_black_chesses){
				if((int)obj.transform.position.x==item[0] && (int)obj.transform.position.z==item[1] && isTop(obj)){
					obj.GetComponent<MeshRenderer>().material=clickPlace;
					obj.tag="ClickPlace_Black";
					break;
				}
			}
			foreach(GameObject obj in player_white_chesses){
				if((int)obj.transform.position.x==item[0] && (int)obj.transform.position.z==item[1] && isTop(obj)){
					obj.GetComponent<MeshRenderer>().material=clickPlace;
					obj.tag="ClickPlace_White";
					break;
				}
			}
		}
	}
	#endregion

	#region Return_shader
	[SerializeField] private Material returned_shader;
	[SerializeField] private Material blackPiece;
	[SerializeField] private Material whitePiece;
	[SerializeField] private Material tile;
	void Return_shader(){
		this.gameObject.GetComponent<MeshRenderer>().material = returned_shader;
		GameObject[] clickPlaces_black=GameObject.FindGameObjectsWithTag("ClickPlace_Black");
		GameObject[] clickPlaces_white=GameObject.FindGameObjectsWithTag("ClickPlace_White");
		GameObject[] clickPlaces_tile=GameObject.FindGameObjectsWithTag("ClickPlace_Tile");
		foreach(GameObject obj in clickPlaces_black){
			obj.GetComponent<MeshRenderer>().material=blackPiece;
			obj.tag="player_black";
		}
		foreach(GameObject obj in clickPlaces_white){
			obj.GetComponent<MeshRenderer>().material=whitePiece;
			obj.tag="player_white";
		}
		foreach(GameObject obj in clickPlaces_tile){
			obj.GetComponent<MeshRenderer>().material=tile;
			obj.tag="Tile";
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
			if(player_black_chesses[i] != this.gameObject && player_black_chesses[i].GetComponent<PlayerMove2>().PlayerIsSelected == true){    
				return true;
			}
			
		}
		for(int i = 0; i < player_white_chesses.Length; i++){
			if(player_white_chesses[i] != this.gameObject && player_white_chesses[i].GetComponent<PlayerMove2>().PlayerIsSelected == true){
				return true;
			}
		}
		return false;
	}

	bool ChoosedAsDestination(){
		player_black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		player_white_chesses = GameObject.FindGameObjectsWithTag("player_white");
		for(int i = 0; i < player_black_chesses.Length; i++){
			if(player_black_chesses[i] != this.gameObject && player_black_chesses[i].GetComponent<PlayerMove2>().destination == this.gameObject){    
				return true;
			}
			
		}
		for(int i = 0; i < player_white_chesses.Length; i++){
			if(player_white_chesses[i] != this.gameObject && player_white_chesses[i].GetComponent<PlayerMove2>().destination == this.gameObject){
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
		if(this.gameObject.CompareTag("player_black")){
			turn=GameMainScript.instance.Black;
		}
		if(this.gameObject.CompareTag("player_white")){
			turn=GameMainScript.instance.White;
		}
		return (turn==GameMainScript.instance.Turn);
	}

	// オブジェクトが一番上にあるかチェック
	bool isTop(){
		int boardVal=GameMainScript.instance.board_state[(int)this.transform.position.x,(int)this.transform.position.z];
		if(boardVal<=2){ // 一段以下
			return true;
		}else if(boardVal<=6){ // 二段
			return (this.transform.position.y==1.85f);
		}else{ //三段
			return (this.transform.position.y==2.7f);
		}
	}

	bool isTop(GameObject obj){
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