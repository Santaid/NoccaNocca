using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AI_K : MonoBehaviour {
    
    public static AI_K instance;
	public MonteAI_K mAI_k;
    public MonteTreeAI mtAI_k;
	public AlphaBetaAI abAI;
	public Board board;
    public int from_x;
    public int from_z;
    public int to_x;
    public int to_z;
	private int[] tmp1;

	public bool Monte = true;
	public bool MonteTree = false;
	public bool AlphaBeta = false;
    public void Awake(){
		if(instance == null){
			instance = this;
		}
	}

    public void MAI_K() {
        int[,] board_info = GameMainScript.instance.board_state;
		board = new Board(board_info, GameMainScript.instance.board_top, GameMainScript.instance.Turn, GameMainScript.instance.End);
		
		if(Monte == true){
			mAI_k = new MonteAI_K(board);
			tmp1 = mAI_k.selectHand();
		}else if(MonteTree == true){
			mtAI_k = new MonteTreeAI(board);
			tmp1 = mtAI_k.selectHand();
		}else if(AlphaBeta == true){
			abAI = new AlphaBetaAI(board);
			tmp1 = abAI.selectHand();
		}
        
        from_x = tmp1[0];
        from_z = tmp1[1];
        to_x = tmp1[2];
        to_z = tmp1[3];
    }
}

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
		NoccaFunction ruleObj = new NoccaFunction();

		// MonteAI_K aiObj = new MonteAI_K(board);
		// int[] tmp1 = aiObj.selectHand();
		// board = ruleObj.move(board, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		// ruleObj.printBoard(board);

		int win = 0;
		for(int _ = 0; _ < 10; _++){
			Board board = new Board(start_board, start_board_top, 1, false);
			// ruleObj.printBoard(board);
			while(ruleObj.isEnd(board) == 0){
				// RandomAI_K aiObject1 = new RandomAI_K(board);
				// NegaMaxAI aiObject1 = new NegaMaxAI(board);
				// AlphaBetaAI aiObject1 = new AlphaBetaAI(board);
				// MonteAI_K aiObject1 = new MonteAI_K(board);
				MonteTreeAI aiObject1 = new MonteTreeAI(board);
				int[] tmp1 = aiObject1.selectHand();
				board = ruleObj.move(board, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
				// ruleObj.printBoard(board);
				// Console.WriteLine();
				if(ruleObj.isEnd(board) == 1)win++;
				// if(ruleObj.isEnd(board) == 2)win--;
				if(ruleObj.isEnd(board) == 0){
					// RandomAI_K aiObject2 = new RandomAI_K(board);
					// NegaMaxAI aiObject2 = new NegaMaxAI(board);
					// AlphaBetaAI aiObject2 = new AlphaBetaAI(board);
					MonteAI_K aiObject2 = new MonteAI_K(board);
					// MonteTreeAI aiObject2 = new MonteTreeAI(board);
					int[] tmp2 = aiObject2.selectHand();
					board = ruleObj.move(board, tmp2[0], tmp2[1], tmp2[2], tmp2[3]);
					// ruleObj.printBoard(board);
					// Console.WriteLine();
					if(ruleObj.isEnd(board) == 1)win++;
					// if(ruleObj.isEnd(board) == 2)win--;
				}
			}
			ruleObj.printBoard(board);
			Console.WriteLine();
			Console.WriteLine("count = {0},   win count = {1}", _ + 1, win);
		}
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
		negaAlphaBeta_search(1, board);
		return reply;
	}

	int negaAlphaBeta(int depth, Board board, int alpha, int beta){
		int max = -1000000;
		if(depth == 0){
			int score = ruleObj.evaluate_board(board);
			Console.WriteLine("depth : {0}     max : {1}", depth, score);
			return score;
		}
		for(int i = 0; i < hands.Count; i++){
			int negaAlphaBeta_tmp = -negaAlphaBeta(depth - 1, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]), -beta, -alpha);
			if(negaAlphaBeta_tmp >= beta)return negaAlphaBeta_tmp;
			alpha = Math.Max(alpha, negaAlphaBeta_tmp);
			max = Math.Max(max, negaAlphaBeta_tmp);
			Console.WriteLine("depth : {0}     max : {1}", depth, alpha);
		}
		return max;
	}

	void negaAlphaBeta_search(int depth, Board board){
		int alpha = -1000000;
		int beta = 1000000;
		int score;
		for(int i = 0; i < hands.Count; i++){
			score = - negaAlphaBeta(depth, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]), - beta, -alpha);
			if(alpha < score){
				alpha = score;
				reply = hands[i];
			}
		}
		Console.WriteLine("depth : {0}     max : {1}", depth, alpha);
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
		int max = -1000000;
		if(depth == 0){
			return ruleObj.evaluate_board(board);
		}

		for(int i = 0; i < hands.Count; i++){
			int negaAlphaBeta_tmp = -negaMax(depth - 1, ruleObj.move(board, hands[i][0], hands[i][1], hands[i][2], hands[i][3]));
			max = Math.Max(max, negaAlphaBeta_tmp);
		}

		return max;
	}

	void negaMax_search(int depth, Board board){
		int max = -1000000;
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

public class RandomAI_K{
	Board board;
	NoccaFunction ruleObj = new NoccaFunction();

	public RandomAI_K(Board board){
		this.board = board;
	}

	public int[] selectHand(){
		var rnd = new System.Random();
		List<int[]> tmp = ruleObj.makeHands(board);
		// foreach(int[] a in tmp){
		// 	Console.WriteLine("{0}{1}{2}{3}", a[0],a[1],a[2],a[3]);
		// }
		int rndInt = rnd.Next(0, tmp.Count);
		// Console.WriteLine(tmp.Count);
		return tmp[rndInt];
	}
}

public class MonteAI_K{
	Board board;
	NoccaFunction ruleObj = new NoccaFunction();
	List<int[]> hands;
	int[] reply = new int[]{-1,-1,-1,-1};

	public MonteAI_K(Board board){
		this.board = board;
		hands = ruleObj.makeHands(board);
	}

	public int[] selectHand(){
		monte_search(board);
		return reply;
	}

	public int one_play(int[] first_hand, Board board){//Boardはcloneしたものを渡す(値参照のため)   moveするから関係ない
		Board tmp = ruleObj.move(board, first_hand[0], first_hand[1], first_hand[2], first_hand[3]);//自分側が打ってからスタート
		while(ruleObj.isEnd(tmp) == 0){
			RandomAI_K aiObject1 = new RandomAI_K(tmp);
			int[] tmp1 = aiObject1.selectHand();
			tmp = ruleObj.move(tmp, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		}
		return ruleObj.isEnd(tmp);//勝った色が返る
	}

	public void monte_search(Board board){
		int max = 0;
		int[] winCount = new int[hands.Count];
		for(int i = 0; i < hands.Count; i++){
			winCount[i] = 0;
		}

		Stopwatch sw = new Stopwatch();
		while(sw.ElapsedMilliseconds < 10 * 1000){
			sw.Start();
			for(int i = 0; i < hands.Count; i++){
				// int[,] cloned_board_state = new int[board.board_state.GetLength(0), board.board_state.GetLength(1)];
				// Array.Copy(board.board_state, cloned_board_state, board.board_state.Length);
				// int[,] cloned_board_top = new int[board.board_top.GetLength(0), board.board_top.GetLength(1)];
				// Array.Copy(board.board_top, cloned_board_top, board.board_top.Length);
				// Board cloned_board = new Board(cloned_board_state, cloned_board_top, board.Turn, board.End);

				// if(one_play(hands[i], cloned_board) == board.Turn){
				// 	winCount[i]++;
				// }
				if(one_play(hands[i], board) == board.Turn){
					winCount[i]++;
				}
			}
			sw.Stop();
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

public class MonteTreeAI{
	Node root;//現在のboardを含むNode
	List<Node> visits = new List<Node>();//訪れたノード(フィードバック用)
	List<int[]> hands;
	public int threshold = 200;//閾値
	public double c_value = 2.0;
	// int one_play_count = 1;//一回訪れた際のplayout数
	int[] reply = new int[]{-1,-1,-1,-1};
	public NoccaFunction ruleObj = new NoccaFunction();


	public MonteTreeAI(Board board){
		this.root = new Node(board);
		hands = ruleObj.makeHands(board);
	}

	public int[] selectHand(){
		monteTree_search();
		return reply;
	}

	public void monteTree_search(){
		Stopwatch sw = new Stopwatch();
		while(sw.ElapsedMilliseconds < 20 * 1000){
			sw.Start();
			visits.Clear();
			Node selected = select(root);
			if(selected.visit >= threshold){
				expand(selected);
			}else{
				if(ruleObj.isEnd(selected.board) == 0){//選ばれたのが終わっていない盤面
					RandomAI_K aiObject1 = new RandomAI_K(selected.board);
					int[] hand_tmp = aiObject1.selectHand();
					feedback(visits, one_play(hand_tmp, selected.board));
				}else if(ruleObj.isEnd(selected.board) == 1){
					feedback(visits, 1);
				}else{
					feedback(visits, 2);
				}
			}
			sw.Stop();
		}

		int max_visit = 0;
		for(int i = 0; i < hands.Count; i++){
			if(root.node_children[i].visit > max_visit){
				max_visit = root.node_children[i].visit;
				reply = hands[i];
			}
			// Console.WriteLine("hand : {0}{1}{2}{3} => visit count {4},   win rate {5}", hands[i][0],hands[i][1],hands[i][2],hands[i][3],root.node_children[i].visit, (float)root.node_children[i].win/(float)root.node_children[i].visit);
		}
	}

	public class Node{//サブクラスを用意した
		public int visit;//訪問回数
		public int win;//報酬回数
		public Board board;
		public double score = 0;
		public List<Node> node_children = new List<Node>();

		public Node(Board board){
			this.board = board;
			this.visit = 0;
			this.win = 0;
			node_children.Clear();
		}
	}

	public void expand(Node node){
		List<int[]> children_hands = ruleObj.makeHands(node.board);
		for(int i = 0; i < children_hands.Count; i++){
			node.node_children.Add(new Node(ruleObj.move(node.board, children_hands[i][0], children_hands[i][1], children_hands[i][2], children_hands[i][3])));
		}
	}


	public Node select(Node node){
		Node tmp = node;
		double max = 0;
		visits.Add(node);//feedback用に格納
		if(node.node_children.Count == 0){//子を持っていないならそのノードを返す
			return node;
		} else {//下を探索
			foreach( Node i in node.node_children){
				if(i.visit == 0){//訪問回数0は優先
					tmp = i;
					break;
				} else {
					double culc = ((double)i.win / (double)i.visit) + (Math.Sqrt(c_value) * Math.Sqrt(Math.Log(root.visit) / (double)i.visit));
					i.score = culc;
					if(culc > max){//maxを超えたらtmpに保存
						max = culc;
						tmp = i;
					}
				}
			}
			return select(tmp);
		}
	}

	public void feedback(List<Node> visits, int win_color){
		foreach(Node i in visits){
			if(i.board.Turn != win_color){//相手の盤面で考えるため
				i.visit++;
				i.win++;
			} else {
				i.visit++;
			}
		}
	}

	public int one_play(int[] first_hand, Board board){
		Board tmp = ruleObj.move(board, first_hand[0], first_hand[1], first_hand[2], first_hand[3]);//自分側が打ってからスタート
		while(ruleObj.isEnd(tmp) == 0){
			RandomAI_K aiObject1 = new RandomAI_K(tmp);
			int[] tmp1 = aiObject1.selectHand();
			tmp = ruleObj.move(tmp, tmp1[0], tmp1[1], tmp1[2], tmp1[3]);
		}
		return ruleObj.isEnd(tmp);//勝った色が返る黒1白2
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
		{-5, 0, 10, 30, 60, 120, 300, 500},
		{-5, 0, 10, 30, 60, 120, 300, 500},
		{-5, 0, 10, 30, 60, 120, 300, 500},
		{-5, 0, 10, 30, 60, 120, 300, 500},
		{-5, 0, 10, 30, 60, 120, 300, 500},
		{ 0, 0,  0,  0,  0,  0,  0,   0}
	};
	int[,] white_point_board = new int[,] {
		{ 0, 0,  0,  0,  0,  0,  0,   0},
		{500, 300, 120, 60, 30, 10, 0, -5},
		{500, 300, 120, 60, 30, 10, 0, -5},
		{500, 300, 120, 60, 30, 10, 0, -5},
		{500, 300, 120, 60, 30, 10, 0, -5},
		{500, 300, 120, 60, 30, 10, 0, -5},
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

	public Board move(Board board, int from_x,int from_z,int to_x,int to_z){//boardに変化はない
		int[,] return_board_state = new int[board.board_state.GetLength(0), board.board_state.GetLength(1)];//ボードをクローンしている(配列は値参照のため)
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

	public int evaluate_board(Board board){
		int _isEnd = isEnd(board);
		if(_isEnd == board.Turn){
			return 5000;
		} else if(_isEnd == 0){
			if(board.Turn == Black){
				return evaluate_point_black(board) + evaluate_climb_black(board);
			} else {
				return evaluate_point_white(board) + evaluate_climb_white(board);
			}
		} else {
			return -5000;
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
		if(top_count_black - top_count_white == 5){
			return 5000;
		} else {
			return (top_count_black - top_count_white) * 100;
		}
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
		if(top_count_white - top_count_black == 5){
			return 5000;
		} else {
			return (top_count_white - top_count_black) * 100;
		}
		
	}

	public List<int[]> canMove(Board board, int r, int l){
		List<int[]> res = new List<int[]>();
		for(int i = -1; i <= 1; i++){
			for(int j = -1; j <= 1; j++){
				if(board.board_state[r + i, l + j] <= 6 && !(i == 0 && j == 0)){
					if(!(board.Turn == Black && l + j == 0) && !(board.Turn == White && l + j == line + 1)){
						res.Add(new int[2] {r + i, l + j});
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
				return White;
			}else if(board.board_state[i,line+1]==Black){
				// Console.WriteLine("Black won");
				board.End=true;
				return Black;
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