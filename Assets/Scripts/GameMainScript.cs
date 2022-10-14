using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainScript : MonoBehaviour{
	public bool End=false;
	public int Turn;
	public int White=2;
	public int Black=1;
	public int Wall=15;
	[SerializeField] public int row;
	[SerializeField] public int line;
	[SerializeField] private GameObject board;
	[SerializeField] private GameObject myCamera;
	[SerializeField] private GameObject playerWhite;
	[SerializeField] private GameObject playerBlack;
	[SerializeField] private GameObject click_place;
	public int[,] board_state;
	public int[,] board_top;
	public List<int[,]> former_boards = new List<int[,]>();

	public static GameMainScript instance;
	public void Awake(){
		if(instance == null){
			instance = this;
		}
	}

    // Start is called before the first frame update
    void Start(){
		myCamera.transform.position = new Vector3(0.5f + 0.5f * row, 2.8f, -(0.5f + 0.5f * line));
		board.transform.position = new Vector3(0.5f + 0.5f * row, 0, 0.5f + 0.5f * line);
		board.transform.localScale = new Vector3(row + 2, 1, line + 2);
		Turn=Black;
		board_state = new int[row + 2, line + 2];
		board_top=new int[row+2,line+2];
		for(int j = 0; j < line + 2; j++){//壁を上下に作る
			board_state[0,j] = Wall;
			board_state[row + 1,j] = Wall;
			board_top[0,j]=0;
			board_top[row+1,j]=0;
		}
		for(int i = 1; i < row + 1; i ++){//0で埋める
			for(int j = 0; j < line + 2 ; j++){
				board_state[i, j] = 0;
				board_top[i,j]=0;
			}
		}

		for(int i = 1; i <= row; i++){//初期配置
			board_state[i, 1] = Black;
			board_state[i, line] = White;
			board_top[i,1]=Black;
			board_top[i,line]=White;
			// Vector3(row方向, 高さ, line方向)
			Vector3 pos_white = new Vector3(i, 1, line);
			Vector3 pos_black = new Vector3(i, 1, 1);
			Instantiate(playerWhite, pos_white, Quaternion.identity);
			Instantiate(playerBlack, pos_black, Quaternion.identity);
			// 移動先のタイル生成
			for(int j=0; j<line+2; j++){
				Instantiate(click_place,new Vector3(i,0.5f,j),Quaternion.identity);
			}
		}
    }

	[SerializeField] private GameObject WhiteWin;
	[SerializeField] private GameObject BlackWin;
	void Update(){
		if(isEnd() == Black){
			WhiteWin.SetActive(true);
		}else if(isEnd() == White){
			BlackWin.SetActive(true);
		}
	}

	public void changeTurn(){
		if(Turn==Black){
			Turn=White;
			Debug.Log("white turn");
		}else{
			Turn=Black;
			Debug.Log("black turn");
		}
	}

	public List<int[]> canMove(int r,int l){
		List<int[]> res=new List<int[]>();
		for(int i=-1; i<=1; i++){
			for(int j=-1; j<=1; j++){
				if(board_state[r+i,l+j]<=6 && !(i==0 && j==0)){
					if(!(Turn==Black && l+j==0) && !(Turn==White && l+j==line+1)){
						res.Add(new int[2] {r+i,l+j});
					}
				}
			}
		}
		return res;
	}

	// move自体はルールによる移動制限はない
	public void move(int from_x,int from_z,int to_x,int to_z){
		int originalPos=board_state[from_x,from_z];
		int movedPos=board_state[to_x,to_z];

		// 移動元の処理
		if(originalPos<=2){ //　一段
			board_state[from_x,from_z]=0;
			board_top[from_x,from_z]=0;
		}else if(originalPos<=6){ // 二段
			board_state[from_x,from_z]-=Turn<<1;
			board_top[from_x,from_z]=board_state[from_x,from_z];
		}else if(originalPos<15){ // 三段
			board_state[from_x,from_z]-=Turn<<2;
			if(board_state[from_x,from_z]<=4){ // 3or4->black
				board_top[from_x,from_z]=Black;
			}else{
				board_top[from_x,from_z]=White;
			}
		}
		// 移動先の処理
		if(movedPos==0){ // 空白
			board_state[to_x,to_z]+=Turn;
		}else if(movedPos<=2){ // 一段
			board_state[to_x,to_z]+=Turn<<1;
		}else if(movedPos<=6){ // 二段
			board_state[to_x,to_z]+=Turn<<2;
		}
		board_top[to_x,to_z]=Turn;
		// Debug.Log("board : (from)"+board_state[from_x,from_z]+", (to)"+board_state[to_x,to_z]);
		// Debug.Log("board_top : (from)"+board_top[from_x,from_z]+", (to)"+board_top[to_x,to_z]);
	}

	public int isEnd(){
		bool white_top=true,black_top=true;
		// ゴールしたか
		for(int i=1; i<=row; i++){
			if(board_state[i,0]==White){
				Debug.Log("White won");
				End=true;
				return Black;
			}else if(board_state[i,line+1]==Black){
				Debug.Log("Black won");
				End=true;
				return White;
			}
			// 駒が動けないか
			for(int j=1; j<=line; j++){
				if(board_top[i,j]==Black){
					white_top=false;
				}
				if(board_top[i,j]==White){
					black_top=false;
				}
			}
		}
		if(black_top && !white_top){
			Debug.Log("Black won");
			End=true;
			return Black;
		}
		if(white_top && !black_top){
			Debug.Log("White won");
			End=true;
			return White;
		}
		return 0;
	}
}
