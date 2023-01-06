using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class AI_Yuzurihara : MonoBehaviour
{
	private GameObject AIComponent = null; //GameMainにAIをつける駒を設定必要ある
	[SerializeField] private GameObject AIComponentW; //GameMainにAIをつける駒を設定必要ある
	[SerializeField] private GameObject AIComponentB; //GameMainにAIをつける駒を設定必要ある
	private GameObject[] AIPieces; //AIがコントロールする駒の配列
	private int AIColor; //AI駒の色
	private System.Random rnd=new System.Random();
	private bool illigal=false;
	[SerializeField] private GameObject InputField;

	private bool agent_predicting = false;
	public bool White;
	public bool Black;

	void Start(){
		if(White == true){
			AIComponent = AIComponentW;
		}else if(Black == true){
			AIComponent = AIComponentB;
		}
		AIPieces = GameObject.FindGameObjectsWithTag(AIComponent.tag);
		if(AIComponent.tag == "player_black"){
			AIColor = GameMainScript.instance.Black;
		}else if(AIComponent.tag == "player_white"){
			AIColor = GameMainScript.instance.White;
		}
       
	}

	// Update is called once per frame
	async void Update(){
		if(GameMainScript.instance.End || illigal){
			return;
		}

		AIPieces = GameObject.FindGameObjectsWithTag(AIComponent.tag);
		if(AIColor == GameMainScript.instance.Turn){
			if(!agent_predicting){
				InputField.SetActive(true);
				agent_predicting = true;
				agent_predicting =await Task<bool>.Run<bool>(()=>{
					MCTS.instance.MAI();
					return false;
				});
				if(!agent_predicting){
					AIMove(MCTS.instance.from_x,MCTS.instance.from_z,MCTS.instance.to_x,MCTS.instance.to_z);
					InputField.SetActive(false);
				}
			}		
		}
	}

	#region ObjectsInit
	GameObject[] black_chesses;
	GameObject[] white_chesses;
	GameObject[] ClickPlace;
	void ObjectsInit(){
		black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		white_chesses = GameObject.FindGameObjectsWithTag("player_white");
		ClickPlace =  GameObject.FindGameObjectsWithTag("ClickPlace");
	}
	#endregion

	#region SelectPiece
	public GameObject SelectPiece(int from_x, int from_z){
		ObjectsInit();
		int layer;
		int boardVal=GameMainScript.instance.board_state[from_x,from_z];
		if(boardVal<=2){
			layer=1;
		}else if(boardVal<=6){
			layer=2;
		}else{
			layer=3;
		}
		//Debug.Log("boardVal " + boardVal);
		//Debug.Log("layer" + layer);

		switch(layer){
			case 1:
				foreach(GameObject obj in AIPieces){
					if(obj.transform.position == new Vector3(from_x, 1.0f,from_z)){
						return obj;
					}
				}
				break;
			case 2:
				foreach(GameObject obj in AIPieces){
					if(obj.transform.position == new Vector3(from_x, 1.85f,from_z)){
						return obj;
					}
				}
				break;
			case 3:
				foreach(GameObject obj in AIPieces){
					if(obj.transform.position == new Vector3(from_x, 2.70f,from_z)){
						return obj;
					}
				}
				break;
		}
		return null;
	}
	#endregion

	#region SelectDestination
	// 行先のlayerはタッチするオブジェクトの位置
	public GameObject SelectDestination(int to_x, int to_z){
		ObjectsInit();
		int layer;
		int boardVal=GameMainScript.instance.board_state[to_x,to_z];
		if(boardVal==0){
			layer=0;
		}else if(boardVal<=2){
			layer=1;
		}else{
			layer=2;
		}

		switch(layer){
			case 0:
				foreach(GameObject obj in ClickPlace){
					if(obj.transform.position == new Vector3(to_x, 0.5f,to_z)){
						return obj;
					}
				}
				break;
			case 1:
				foreach(GameObject obj in black_chesses){
					if(obj.transform.position == new Vector3(to_x, 1.0f,to_z)){
						return obj;
					}
				}
				foreach(GameObject obj in white_chesses){
					if(obj.transform.position == new Vector3(to_x, 1.0f,to_z)){
						return obj;
					}
				}
				break;
			case 2:
				foreach(GameObject obj in black_chesses){
					if(obj.transform.position == new Vector3(to_x, 1.85f,to_z)){
						return obj;
					}
				}
				foreach(GameObject obj in white_chesses){
					if(obj.transform.position == new Vector3(to_x, 1.85f,to_z)){
						return obj;
					}
				}
				break;
		}
		return null;
	}
	#endregion

	#region AIMove
	private void AIMove(int from_x,int from_z,int to_x,int to_z){
		// 駒の選択
		GameObject selectedPiece=SelectPiece(from_x,from_z);

		if(selectedPiece==null){
			illigal=true;
			Debug.Log("SelectedPiece Error");
			return;
		}

		PlayerMove instance_selected=selectedPiece.GetComponent<PlayerMove>();
		instance_selected.PlayerIsSelected=true;
		instance_selected.ExtractDestination();
		// 行先の選択
		GameObject destination=SelectDestination(to_x,to_z);

		if(destination==null){
			illigal=true;
			Debug.Log("SelectedDestination Error");
			return;
		}
		if(!instance_selected.destinations.Contains(destination)){
			illigal=true;
			Debug.Log("ルールに反する移動先の選択");
			return;
		}else{
			instance_selected.destination=destination;
			instance_selected.DestinationIsSelected=true;
			instance_selected.Move();
			instance_selected.PlayerIsSelected=false;
			instance_selected.destinations.Clear();
			GameMainScript.instance.changeTurn();
			GameMainScript.instance.isEnd();
			agent_predicting = false;
		}
	}
	#endregion

    

}