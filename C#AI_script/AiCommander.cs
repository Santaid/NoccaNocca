using System;
using System.Collections.Generic;

public class AiCommander{

	public static void Main(){
		int[,] start_board = new int[,] {
			{ 15, 15, 15, 15, 15, 15, 15, 15},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{ 15, 15, 15, 15, 15, 15, 15, 15}
		};

		int[,] start_board_top = new int[,] {
			{  0,  0,  0,  0,  0,  0,  0,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  1,  0,  0,  0,  0,  2,  0},
			{  0,  0,  0,  0,  0,  0,  0,  0}
		};
		Board board = new Board(start_board, start_board_top, 1, false);
		NoccaFunction ruleObj = new NoccaFunction();
		ruleObj.printBoard(board);
		
		// int black_count = 0;
		// int white_count = 0;
		while(ruleObj.isEnd(board) == 0){
			AlphaBetaAI aiObject1 = new AlphaBetaAI(board);
			int[] tmp1 = aiObject1.selectHand();
			board = ruleObj.move(board, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
			ruleObj.printBoard(board);
			Console.WriteLine();
			if(ruleObj.isEnd(board) == 0){
				RandomAI aiObject2 = new RandomAI(board);
				int[] tmp2 = aiObject2.selectHand();
				board = ruleObj.move(board, tmp2[0], tmp2[1], tmp2[2], tmp2[3]);
				ruleObj.printBoard(board);
				Console.WriteLine();
			}
		}
		// ruleObj.printBoard(board);
		// for(int i = 0; i < 100; i++){
		// 	while(ruleObj.isEnd(board) == 0){
		// 		AlphaBetaAI aiObject1 = new AlphaBetaAI(board);
		// 		int[] tmp1 = aiObject1.selectHand();
		// 		board = ruleObj.move(board, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		// 		if(ruleObj.isEnd(board) == 1)black_count++;
		// 		if(ruleObj.isEnd(board) == 2)white_count++;
		// 		// ruleObj.printBoard(board);
		// 		// Console.WriteLine("hand_x={0} hand_y={1}", tmp1[0], tmp1[1]);
		// 		// Console.WriteLine("hand_x={0} hand_y={1}", tmp1[2], tmp1[3]);
		// 		// ruleObj.move(board, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		// 		// ruleObj.printBoard(board);
		// 		if(ruleObj.isEnd(board) == 0){
		// 			RandomAI aiObject2 = new RandomAI(board);
		// 			int[] tmp2 = aiObject2.selectHand();
		// 			board = ruleObj.move(board, tmp2[0], tmp2[1], tmp2[2], tmp2[3]);
		// 			if(ruleObj.isEnd(board) == 1)black_count++;
		// 			if(ruleObj.isEnd(board) == 2)white_count++;
		// 		}
		// 	}
		// 	board = new Board(start_board, start_board_top, 1, false);
		// }
		// Console.WriteLine("black = {0}   white = {1}", black_count, white_count);
	}
}


public class AlphaBetaAI{
	Board board;
	List<int[]> hands;
	NoccaFunction ruleObj = new NoccaFunction();
	int[] reply = new int[]{-1,-1,-1,-1};

	public AlphaBetaAI(Board board){
		this.board = board;
		hands = ruleObj.makeHands(board);
	}

	public int[] selectHand(){
		negaAlphaBeta_search(5, board);
		return reply;
	}

	int negaAlphaBeta(int depth, Board board, int alpha, int beta){
		int max = -10000;
		if(depth == 0){
			if(board.Turn == 1)return (ruleObj.evaluate_point_black(board) + ruleObj.evaluate_climb_black(board));
			else return (ruleObj.evaluate_point_white(board) + ruleObj.evaluate_climb_white(board));
		}
		for(int i = 0; i < hands.Count; i++){
			int negaAlphaBeta_tmp = -negaAlphaBeta(depth - 1, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]), -beta, -alpha);
			if(negaAlphaBeta_tmp >= beta)return negaAlphaBeta_tmp;
			alpha = Math.Max(alpha, negaAlphaBeta_tmp);
			max = Math.Max(max, negaAlphaBeta_tmp);
		}
		// Console.WriteLine("depth : {0}     max : {1}", depth, alpha);
		return max;
	}

	void negaAlphaBeta_search(int depth, Board board){
		int alpha = -10000;
		int beta = 10000;
		int score;
		for(int i = 0; i < hands.Count; i++){
			score = - negaAlphaBeta(depth, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]), - beta, -alpha);
			if(alpha < score){
				alpha = score;
				reply = hands[i];
			}
		}
		// Console.WriteLine("depth : {0}     max : {1}", depth, alpha);
	}
}

public class NegaMaxAI{
	Board board;
	List<int[]> hands;
	NoccaFunction ruleObj = new NoccaFunction();
	int[] reply = new int[]{-1,-1,-1,-1};

	public NegaMaxAI(Board board){
		this.board = board;
		hands = ruleObj.makeHands(board);
	}

	public int[] selectHand(){
		negaMax_search(4, board);
		return reply;
	}

	int negaMax(int depth, Board board){
		int max = -10000;
		if(depth == 0){
			if(board.Turn == 1)return (ruleObj.evaluate_point_black(board) + ruleObj.evaluate_climb_black(board));
			else return (ruleObj.evaluate_point_white(board) + ruleObj.evaluate_climb_white(board));
		}

		for(int i = 0; i < hands.Count; i++){
			int negaAlphaBeta_tmp = -negaMax(depth - 1, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]));
			max = Math.Max(max, negaAlphaBeta_tmp);
		}

		return max;
	}

	void negaMax_search(int depth, Board board){
		int max = -10000;
		for(int i = 0; i < hands.Count; i++){
			int tmp = - negaMax(depth, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]));
			if(max < tmp){
				max = tmp;
				reply = hands[i];
			}
		}
		// Console.WriteLine("depth : {0}     max : {1}", depth, alpha);
	}

}

public class RandomAI{
	Board board;
	NoccaFunction ruleObj = new NoccaFunction();

	public RandomAI(Board board){
		this.board = board;
	}

	public int[] selectHand(){
		var rnd = new Random();
		List<int[]> tmp = ruleObj.makeHands(board);
		return tmp[rnd.Next(0, tmp.Count)];
	}
}

public class MonteAI{
	Board board;
	NoccaFunction ruleObj = new NoccaFunction();
	List<int[]> hands;
	int[] reply = new int[]{-1,-1,-1,-1};

	public MonteAI(Board board){
		this.board = board;
		hands = ruleObj.makeHands(board);
	}

	public int[] selectHand(){
		monte_search(board);
		return reply;
	}

	public int one_play(int[] first_hand, Board board){//Boardはcloneしたものを渡す(値参照のため)
		Board tmp = ruleObj.move(board, first_hand[0], first_hand[1], first_hand[2], first_hand[3]);//自分側が打ってからスタート
		while(ruleObj.isEnd(tmp) == 0){
			RandomAI aiObject1 = new RandomAI(tmp);
			int[] tmp1 = aiObject1.selectHand();
			tmp = ruleObj.move(tmp, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		}
		return ruleObj.isEnd(tmp);//勝った色が返る
	}

	public void monte_search(Board board){
		int loop = 100;
		int max = 0;
		int[] winCount = new int[hands.Count];
		for(int i = 0; i < hands.Count; i++){
			winCount[i] = 0;
		}
		for(int j = 0; j < loop; j++){
			for(int i = 0; i < hands.Count; i++){
				int[,] cloned_board_state = new int[board.board_state.GetLength(0), board.board_state.GetLength(1)];
				Array.Copy(board.board_state, cloned_board_state, board.board_state.Length);
				int[,] cloned_board_top = new int[board.board_top.GetLength(0), board.board_top.GetLength(1)];
				Array.Copy(board.board_top, cloned_board_top, board.board_top.Length);
				Board cloned_board = new Board(cloned_board_state, cloned_board_top, board.Turn, board.End);
				if(one_play(hands[i], cloned_board) == board.Turn){
					winCount[i]++;
				}
			}
		}
		for(int i = 0; i < hands.Count; i++){
			if(max < winCount[i]){
				reply = hands[i];
				max = winCount[i];
			}
			// Console.WriteLine("hand : {0}{1}{2}{3} => win count {4}", hands[i][0],hands[i][1],hands[i][2],hands[i][3],winCount[i]);
		}
	}

}



public class Board{
	public int Turn;
	public bool End;
	// public int Black = 1;
	// public int White = 2;
	// public int Wall = 15;
	// public int row = 5;
	// public int line = 6;
	public int[,] board_state;
	public int[,] board_top;

	public Board(int[,] board_state, int[,] board_top, int Turn, bool End){
		this.board_state = board_state;
		this.board_top = board_top;
		this.Turn = Turn;
		this.End = End;
		// Turn = Black;
		// board_state = new int[row + 2, line + 2];
		// board_top = new int[row + 2, line + 2];
		// for(int j = 0; j < line + 2; j++){//壁を上下に作る
		// 	board_state[0,j] = Wall;
		// 	board_state[row + 1,j] = Wall;
		// 	board_top[0,j]=0;
		// 	board_top[row+1,j]=0;
		// }
		// for(int i = 1; i < row + 1; i ++){//0で埋める
		// 	for(int j = 0; j < line + 2 ; j++){
		// 		board_state[i, j] = 0;
		// 		board_top[i,j]=0;
		// 	}
		// }
		// for(int i = 1; i <= row; i++){//初期配置
		// 	board_state[i, 1] = Black;
		// 	board_state[i, line] = White;
		// 	board_top[i,1]=Black;
		// 	board_top[i,line]=White;
		// }
	}
}

public class NoccaFunction{
	public int Black = 1;
	public int White = 2;
	public int Wall = 15;
	public int row = 5;
	public int line = 6;

	int[,] black_point_board = new int[,] {
		{ 0, 0,  0,  0,  0,  0,  0,   0},
		{-5, 0, 10, 20, 30, 40, 50, 500},
		{-5, 0, 10, 20, 30, 40, 50, 500},
		{-5, 0, 10, 20, 30, 40, 50, 500},
		{-5, 0, 10, 20, 30, 40, 50, 500},
		{-5, 0, 10, 20, 30, 40, 50, 500},
		{ 0, 0,  0,  0,  0,  0,  0,   0}
	};
	int[,] white_point_board = new int[,] {
		{ 0, 0,  0,  0,  0,  0,  0,   0},
		{500, 50, 40, 30, 20, 10, 0, -5},
		{500, 50, 40, 30, 20, 10, 0, -5},
		{500, 50, 40, 30, 20, 10, 0, -5},
		{500, 50, 40, 30, 20, 10, 0, -5},
		{500, 50, 40, 30, 20, 10, 0, -5},
		{ 0, 0,  0,  0,  0,  0,  0,   0}
	};

	public void make_start_board(Board board){
		board.Turn = Black;
		board.board_state = new int[row + 2, line + 2];
		board.board_top = new int[row + 2, line + 2];
		for(int j = 0; j < line + 2; j++){//壁を上下に作る
			board.board_state[0,j] = Wall;
			board.board_state[row + 1,j] = Wall;
			board.board_top[0,j]=0;
			board.board_top[row+1,j]=0;
		}
		for(int i = 1; i < row + 1; i ++){//0で埋める
			for(int j = 0; j < line + 2 ; j++){
				board.board_state[i, j] = 0;
				board.board_top[i,j]=0;
			}
		}
		for(int i = 1; i <= row; i++){//初期配置
			board.board_state[i, 1] = Black;
			board.board_state[i, line] = White;
			board.board_top[i,1]=Black;
			board.board_top[i,line]=White;
		}
	}

	public Board move(Board board, int from_x,int from_z,int to_x,int to_z){
		int[,] return_board_state = new int[board.board_state.GetLength(0), board.board_state.GetLength(1)];
		Array.Copy(board.board_state, return_board_state, board.board_state.Length);
		int[,] return_board_top = new int[board.board_top.GetLength(0), board.board_top.GetLength(1)];
		Array.Copy(board.board_top, return_board_top, board.board_top.Length);
		// int[,] return_board_state = board.board_state.Clone() as int[,];
		// int[,] return_board_top = board.board_top.Clone() as int[,];
		int originalPos=board.board_state[from_x,from_z];
		int movedPos=board.board_state[to_x,to_z];
		// 移動元の処理
		if(originalPos<=2){ // 一段
			return_board_state[from_x,from_z]=0;
			return_board_top[from_x,from_z]=0;
		}else if(originalPos<=6){ // 二段
			return_board_state[from_x,from_z]-=board.Turn<<1;
			return_board_top[from_x,from_z]=return_board_state[from_x,from_z];
		}else if(originalPos<15){ // 三段
			return_board_state[from_x,from_z]-=board.Turn<<2;
			if(return_board_state[from_x,from_z]<=4){ // 3or4->black
				return_board_top[from_x,from_z]=Black;
			}else{
				return_board_top[from_x,from_z]=White;
			}
		}
		// 移動先の処理
		if(movedPos==0){ // 空白
			return_board_state[to_x,to_z]+=board.Turn;
		}else if(movedPos<=2){ // 一段
			return_board_state[to_x,to_z]+=board.Turn<<1;
		}else if(movedPos<=6){ // 二段
			return_board_state[to_x,to_z]+=board.Turn<<2;
		}
		return_board_top[to_x,to_z]=board.Turn;
		Board tmp = new Board(return_board_state, return_board_top, changeTurn(board), board.End);
		return tmp;
	}

	public void printBoard(Board board){
		for(int i = 0; i < row + 2; i++){
			for(int j = 0; j < line + 2; j++){
				Console.Write("{0,3:d}",board.board_state[i,j]);
			}
			Console.WriteLine("\n");
		}
	}

	public int evaluate_point_black(Board board){
		int score = 0;
		for(int i = 0; i < row + 2; i++){
			for(int j = 0; j < line + 2; j++){
				if(board.board_top[i, j] == Black){
					score += black_point_board[i, j];
				}
			}
		}
		return score;
	}

	public int evaluate_point_white(Board board){
		int score = 0;
		for(int i = 0; i < row + 2; i++){
			for(int j = 0; j < line + 2; j++){
				if(board.board_top[i, j] == White){
					score += white_point_board[i, j];
				}
			}
		}
		return score;
	}

	public int evaluate_climb_black(Board board){
		int top_count_black = 0;
		int top_count_white = 0;
		for(int i = 0; i < row + 2; i++){
			for(int j = 0; j < line + 2; j++){
				if(board.board_top[i, j] == Black){
					top_count_black++;
				} else if(board.board_top[i, j] == White){
					top_count_white++;
				}
			}
		}
				return (top_count_black - top_count_white) * 100;
	}

	public int evaluate_climb_white(Board board){
		int top_count_black = 0;
		int top_count_white = 0;
		for(int i = 0; i < row + 2; i++){
			for(int j = 0; j < line + 2; j++){
				if(board.board_top[i, j] == Black){
					top_count_black++;
				} else if(board.board_top[i, j] == White){
					top_count_white++;
				}
			}
		}
				return (top_count_white - top_count_black) * 100;
	}

	public List<int[]> canMove(Board board, int row, int line){
		List<int[]> res = new List<int[]>();
		for(int i = -1; i <= 1; i++){
			for(int j = -1; j <= 1; j++){
				if(board.board_state[row + i, line + j] <= 6 && !(i == 0 && j == 0)){
					if(!(board.Turn == Black && line + j == 0) && !(board.Turn == White && line + j == line + 1)){
						res.Add(new int[2] {row + i, line + j});
					}
				}
			}
		}
		return res;
	}

	public int changeTurn(Board board){
		if(board.Turn==Black){
			return White;
		}else{
			return Black;
		}
	}

	public List<int[]> makeHands(Board board){
		List<int[]> res = new List<int[]>();
		List<int[]> to;
		for(int i = 1; i <= row; i++){
			for(int j = 1; j <= line; j++){
				if(board.board_top[i,j] == board.Turn){
					to = canMove(board, i, j);
					for(int a = 0; a < to.Count; a++){
						int[] hand = new int[4]{i, j, to[a][0], to[a][1]};
						res.Add(hand);
					}
				}
			}
		}
		return res;//moveの引数と同じ順番の配列がリストになっている
	}

	public int isEnd(Board board){
		bool white_top=true,black_top=true;
		// ゴールしたか
		for(int i=1; i<=row; i++){
			if(board.board_state[i,0]==White){
				// Console.WriteLine("White won");
				board.End=true;
				return Black;
			}else if(board.board_state[i,line+1]==Black){
				// Console.WriteLine("Black won");
				board.End=true;
				return White;
			}
			// 駒が動けないか
			for(int j=1; j<=line; j++){
				if(board.board_top[i,j]==Black){
					white_top=false;
				}
				if(board.board_top[i,j]==White){
					black_top=false;
				}
			}
		}
		if(black_top){
			// Console.WriteLine("Black won");
			board.End=true;
			return Black;
		}
		if(white_top){
			// Console.WriteLine("White won");
			board.End=true;
			return White;
		}
		return 0;
	}
}