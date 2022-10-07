using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class AI : MonoBehaviour
{
    public GameObject AIComponent;　//GameMainにAIをつける駒を設定必要ある
    private GameObject[] AI_chesses;　//AIがコントロールする駒の配列
    private int AIsColor;　//AI駒の色


    // Update is called once per frame
    void Update(){
        AI_chesses = GameObject.FindGameObjectsWithTag(AIComponent.tag);
        if(AIComponent.tag == "player_black"){
            AIsColor = GameMainScript.instance.Black;
        }else if(AIComponent.tag == "player_white"){
            AIsColor = GameMainScript.instance.White;
        }


        if(GameMainScript.instance.End){
			return;
		}

        if(AIsColor == GameMainScript.instance.Turn){
            //調整できるところSTART
            //移動する駒をランダム選ぶ
            int num = UnityEngine.Random.Range(0, AI_chesses.Length - 1);
            GameObject go = AI_chesses[num];
            //調整できるところEND
		    if(go.GetComponent<PlayerMove>().PlayerIsSelected == false && go.GetComponent<PlayerMove>().OtherIsSelected() == false && go.GetComponent<PlayerMove>().ChoosedAsDestination() == false && go.GetComponent<PlayerMove>().isTurn() == true){
			    go.GetComponent<PlayerMove>().PlayerIsSelected = true;
			    go.GetComponent<PlayerMove>().IfCanBeChoosedAsDestination();
                //Debug.Log("playerAI " + go.name);
			    go.GetComponent<PlayerMove>().Change_shader();
		    }else if(go.GetComponent<PlayerMove>().PlayerIsSelected == true && go.GetComponent<PlayerMove>().DestinationIsSelected == false && go.GetComponent<PlayerMove>().OtherIsSelected() == false){
                //調整できるところSTART
                //移動先を選ぶ
                //moveableObjsは移動できるgameObjectsの配列
                int num_destination = UnityEngine.Random.Range(0, go.GetComponent<PlayerMove>().movePlace - 1);
                GameObject destinationAI = go.GetComponent<PlayerMove>().moveableObjs[num_destination];
                //調整できるところEND
                go.GetComponent<PlayerMove>().destination = destinationAI;
			    go.GetComponent<PlayerMove>().DestinationIsSelected = true;
			    go.GetComponent<PlayerMove>().Move();
			    go.GetComponent<PlayerMove>().Return_shader();
			    Array.Clear(go.GetComponent<PlayerMove>().moveableObjs, 0, go.GetComponent<PlayerMove>().movePlace);
			    go.GetComponent<PlayerMove>().movePlace = 0;
			    GameMainScript.instance.changeTurn();
			    GameMainScript.instance.isEnd();
			    go.GetComponent<PlayerMove>().PlayerIsSelected=false;    
	        }
        }
    }

    #region ChessAndClickPlace_Init
    GameObject[] black_chesses;
	GameObject[] white_chesses;
    GameObject[] ClickPlace;
    void ChessAndClickPlace_Init(){
        black_chesses = GameObject.FindGameObjectsWithTag("player_black");
		white_chesses = GameObject.FindGameObjectsWithTag("player_white");
        ClickPlace =  GameObject.FindGameObjectsWithTag("ClickPlace");
    }
    #endregion

    //SelectChess&SelectDestinationの使い方：引数を渡って移動したい駒/目的地を返す、３０行を　GameObject go = SelectChess(1,２,３)(１行2列三階の駒);に入れ替える。４２行も同じやり方

    #region SelectChess
    public GameObject SelectChess(int x, int y, int layer){　//x:行　ｙ：列　layer：階層
        ChessAndClickPlace_Init();
        switch(layer){
            case 1:
                foreach(GameObject obj in black_chesses){
                    if(obj.transform.position == new Vector3(x, 1.0f,y)){
                        return obj;
                    }
                }
                foreach(GameObject obj in white_chesses){
                    if(obj.transform.position == new Vector3(x, 1.0f,y)){
                        return obj;
                    }
                }
                break;
            case 2:
                foreach(GameObject obj in black_chesses){
                    if(obj.transform.position == new Vector3(x, 1.85f,y)){
                        return obj;
                    }
                }
                foreach(GameObject obj in white_chesses){
                    if(obj.transform.position == new Vector3(x, 1.85f,y)){
                        return obj;
                    }
                }
                break;
            case 3:
                foreach(GameObject obj in black_chesses){
                    if(obj.transform.position == new Vector3(x, 2.70f,y)){
                        return obj;
                    }
                }
                foreach(GameObject obj in white_chesses){
                    if(obj.transform.position == new Vector3(x, 2.70f,y)){
                        return obj;
                    }
                }
                break;
        }
        return null;
    }
    #endregion

    #region SelectDestination
    public GameObject SelectDestination(int x, int y, int layer){　//x:行　ｙ：列　layer：階層（0は地面）
        ChessAndClickPlace_Init();
        switch(layer){
            case 0:
                foreach(GameObject obj in ClickPlace){
                    if(obj.transform.position == new Vector3(x, 0.5f,y)){
                        return obj;
                    }
                }
                break;
            case 1:
                foreach(GameObject obj in black_chesses){
                    if(obj.transform.position == new Vector3(x, 1.0f,y)){
                        return obj;
                    }
                }
                foreach(GameObject obj in white_chesses){
                    if(obj.transform.position == new Vector3(x, 1.0f,y)){
                        return obj;
                    }
                }
                break;
            case 2:
                foreach(GameObject obj in black_chesses){
                    if(obj.transform.position == new Vector3(x, 1.85f,y)){
                        return obj;
                    }
                }
                foreach(GameObject obj in white_chesses){
                    if(obj.transform.position == new Vector3(x, 1.85f,y)){
                        return obj;
                    }
                }
                break;
        }
        return null;
    }
    #endregion
}