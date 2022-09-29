using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainScript : MonoBehaviour{
	public int row = 5;
	public int line = 6;
	public GameObject player_cube_one;
	public GameObject player_cube_two;
	public GameObject clickPlace;
	List<GameObject> cubes = new List<GameObject>();
	List<GameObject> clickPlaceList = new List<GameObject>();
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
				board_state[0,j] = 15;
				board_state[row + 1,j] = 15;
			}
			for(int i = 1; i < row + 1; i ++){//0で埋める
				for(int j = 0; j < line + 2 ; j++){
					board_state[i, j] = 0;
				}
			}
			for(int i = 1; i <= row; i++){//初期配置
				board_state[i, 1] = 1;
				board_state[i, line] = 2;
			}
			for(int i = 1; i < row + 1; i++){//クリックの当たり判定
				for(int j = 0; j < line + 2; j++){
					Vector3 pos = new Vector3(i, 1, j);
					clickPlaceList.Add(Instantiate(clickPlace, pos, Quaternion.identity));
				}
			}
			printBoard();

			// move(1,1,1,2,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }
			// move(1,2,1,3,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }
			// move(1,3,1,4,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }
			// move(1,4,1,5,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }
			// move(1,5,1,6,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }
			// move(1,6,1,7,1);
			// if(isEnd(board_state) == 1){
			// 		Debug.Log("黒の勝ち");
			// }

			// printBoard();
    }

    // Update is called once per frame
		public int clickCount = 0;
		public int x;
		public int y;
		int from_x_store;
		int from_y_store;
		int now_color = 1;
    void Update(){
			if(clickCount == 1){
				from_x_store = x;
				from_y_store = y;
			}
			if(clickCount == 2){
				move(from_x_store, from_y_store, x, y, now_color);
				if(isEnd(board_state) == 1){
					Debug.Log("黒の勝ち");
				}
				printBoard();
				clickCount = 0;
				if(now_color == 1){
					now_color = 2;
				} else {
					now_color = 1;
				}
			}
    }

		void move(int from_x, int from_y, int to_x, int to_y, int color){
			switch(board_state[to_x, to_y]){
						case 0:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color;
							}
							break;
						case 1:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 2;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 2;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 2;
							}
							break;
						case 2:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 2;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 2;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 2;
							}
							break;
						case 3:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 4;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 4;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 4;
							}
							break;
						case 4:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 4;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 4;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 4;
							}
							break;
						case 5:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 4;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 4;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 4;
							}
							break;
						case 6:
							if(board_state[from_x, from_y] <= 2){
								board_state[from_x, from_y] -= color;
								board_state[to_x, to_y] += color * 4;
							} else if(board_state[from_x, from_y] <= 6){
								board_state[from_x, from_y] -= color * 2;
								board_state[to_x, to_y] += color * 4;
							} else {
								board_state[from_x, from_y] -= color * 4;
								board_state[to_x, to_y] += color * 4;
							}
							break;
			}
			Debug.Log("move");
			Debug.Log(board_state[from_x, from_y]);
			Debug.Log(board_state[to_x, to_y]);
		}

		public void printBoard(){
			for(int i = 0; i < cubes.Count; i++){
				Destroy(cubes[i]);
			}
			for(int i = 0; i < row + 2; i ++){
				for(int j = 0; j < line + 2 ; j++){
						Vector3 pos1 = new Vector3(i, 1, j);
						Vector3 pos2 = new Vector3(i, 2, j);
						Vector3 pos3 = new Vector3(i, 3, j);
						switch(board_state[i, j]){
						case 0:
							break;
						case 1:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							break;
						case 2:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							break;
						case 3:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							break;
						case 4:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							break;
						case 5:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
            	cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							break;
						case 6:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							break;
						case 7:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos3, Quaternion.identity));
							break;
						case 8:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos3, Quaternion.identity));      	
							break;
						case 9:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos3, Quaternion.identity));
							break;
						case 10:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos3, Quaternion.identity));
							break;
						case 11:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos3, Quaternion.identity));
							break;
						case 12:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_one, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos3, Quaternion.identity));
							break;
						case 13:
							cubes.Add(Instantiate(player_cube_one, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos3, Quaternion.identity));
							break;
						case 14:
							cubes.Add(Instantiate(player_cube_two, pos1, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos2, Quaternion.identity));
							cubes.Add(Instantiate(player_cube_two, pos3, Quaternion.identity));
							break;
						case 15:
							break;
					}
				}
			}
		}

		int isEnd(int[,] board_state){//続行0 黒1 白2 引き分け3
			int count_b = 0;
			int count_w = 0;
			// if(former_boards.Count == 4){//千日手の判定
			// 	if(former_boards[0] == board_state){
			// 		Debug.Log("千日手");
			// 		return 0;
			// 	}//引き分け
			// 	former_boards[0] = former_boards[1];//手を記録
			// 	former_boards[1] = former_boards[2];
			// 	former_boards[2] = former_boards[3];
			// 	former_boards[3] = board_state;
			// } else {
			// 	former_boards.Add(board_state);
			// 	Debug.Log("続行中！");
			// }
			for(int i = 1; i < row + 1; i++){//端に到達した場合
				if(board_state[i,line + 1] == 1) return 1;
				if(board_state[i, 0] == 2) return 2;
			}
			for(int i = 1; i < row + 1; i++){//相手のコマの上に乗って勝利(黒)
				for(int j = 1; j < line + 1; j++){
					int store = board_state[i, j];
					if(store == 2 || store == 5 || store == 6 || store == 11 || store == 12 || store == 13 || store == 14)count_w++;
				}
				if(count_w == 0)return 1;//白の駒が0ということで黒の勝利
			}
			for(int i = 1; i < row + 1; i++){//相手のコマの上に乗って勝利(白)
				for(int j = 1; j < line + 1; j++){
					int store = board_state[i, j];
					if(store == 1 || store == 3 || store == 4 || store == 7 || store == 8 || store == 9 || store == 10)count_b++;
				}
				if(count_b == 0)return 2;//黒の駒が0ということで白の勝利
			}
			return 0;//続行
		}
}
