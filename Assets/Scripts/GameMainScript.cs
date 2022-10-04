using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainScript : MonoBehaviour{
	private int White=1;
	private int Black=2;
	private int Wall=15;
	[SerializeField] private int row = 5;
	[SerializeField] private int line = 6;
	[SerializeField] private GameObject player_cube_one;
	[SerializeField] private GameObject player_cube_two;
	[SerializeField] private GameObject click_place;
	int[,] board_state; 
	List<int[,]> former_boards = new List<int[,]>();

	public static GameMainScript instance;
	public void Awake(){
		if(instance == null){
			instance = this;
		}
	}

    // Start is called before the first frame update
    void Start(){
		board_state = new int[row + 2, line + 2];
		for(int j = 0; j < line + 2; j++){//壁を上下に作る
			board_state[0,j] = Wall;
			board_state[row + 1,j] = Wall;
		}
		for(int i = 1; i < row + 1; i ++){//0で埋める
			for(int j = 0; j < line + 2 ; j++){
				board_state[i, j] = 0;
			}
		}
		for(int i = 1; i <= row; i++){//初期配置
			board_state[i, 1] = Black;
			board_state[i, line] = White;
			// Vector3(row方向, 高さ, line方向)
			Vector3 pos_white = new Vector3(i, 1, 1);
			Vector3 pos_black = new Vector3(i, 1, line);
			Instantiate(player_cube_one, pos_white, Quaternion.identity);
			Instantiate(player_cube_two, pos_black, Quaternion.identity);
			// 移動先のタイル生成
			for(int j=0; j<line+2; j++){
				Instantiate(click_place,new Vector3(i,0.5f,j),Quaternion.identity);
			}
		}
    }

	int isEnd(){
		// ゴールしたか
		for(int i=1; i<=row; i++){
			if(board_state[i,0]==Black){
				return Black;
			}else if(board_state[i,line+1]==White){
				return White;
			}
		}

		// 駒が動けないか
		bool white_top=true,black_top=true;
		for(int i = 1; i <= row; i++){
			for(int j = 1; j <= line; j++){
				// 黒が上
				if(board_state[i,j]==1 || board_state[i,j]==3 || board_state[i,j]==4 || board_state[i,j]>=7 || board_state[i,j]<=10){
					white_top=false;
				}else if(board_state[i,j]!=0){ //白が上
					black_top=false;
				}
			}
		}
		if(white_top){
			Debug.Log("白の勝ち");
			return White;
		}else if(black_top){
			Debug.Log("黒の勝ち");
		}
		return 0;
	}
}
