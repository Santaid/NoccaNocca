using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class Alpha_T : MonoBehaviour//MonoBehaviourやUnity系は必要か不明
{
	public GameObject AIComponent = null; //GameMainにAIをつける駒を設定する必要ある
	public GameObject AIComponentW; //GameMainにAIをつける駒を設定必要ある
	public GameObject AIComponentB; //GameMainにAIをつける駒を設定必要ある
	public static Alpha_T instance;
	private GameObject[] AIPieces; //AIがコントロールする駒の配列
	private int AIColor; //AI駒の色
	private int row,line,Black,White;
	public int from_x,from_z,to_x,to_z,score;
	List<int[]> Canmovepieces = new List<int[]>();//動かせる駒のリスト
  	List<int[]> piecemoveways = new List<int[]>();//駒が動ける方向のリスト
	public void Awake(){
		if(instance == null){
			instance = this;
		}
	}
	void Start(){
		if(AIComponent.tag == "player_black"){
			AIColor = GameMainScript.instance.Black;
		}else if(AIComponent.tag == "player_white"){
			AIColor = GameMainScript.instance.White;
		}
		AIPieces = GameObject.FindGameObjectsWithTag(AIComponent.tag);
		row = GameMainScript.instance.row;
		line = GameMainScript.instance.line;
		Black = GameMainScript.instance.Black;
		White = GameMainScript.instance.White;
		
	}
	public void AIScript(int[,] board_stateAI,int AIColor){
		abAIways(board_stateAI,AIColor);
		Debug.Log("from_x"+from_x+"from_z"+from_z+"to_x"+to_x+"to_z"+to_z+"score"+score);		
	}
	public void abAIways(int[,] board_state,int turn){
		int findinglimit = 7;//何手先まで読むか　最大７くらい
		List<int[]> Selectways =new List<int[]>();	
		Canmovepieces = MovePieces(Change_state_to_top(board_state),turn);	
		for(int d=0;d<Canmovepieces.Count;d++){
			piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn,board_state);
			for(int f = 0;f<piecemoveways.Count;f++){
				Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
			}
		}
		int[] finalscore = maxlevelab(int.MinValue,int.MaxValue,findinglimit,findinglimit,board_state,turn,-1,-1,-1,-1);
		from_x = finalscore[0];
		from_z = finalscore[1];
		to_x = finalscore[2];
		to_z = finalscore[3];
		score = finalscore[4];
	}
	//alpha-beta法
	//自分の手の最大を探る
	int[] maxlevelab(int alpha,int beta ,int limit,int Flimit,int[,] board_state,int turn,int fft,int ffy,int ftt,int fty) {
		List<int[]> Selectways =new List<int[]>();	
		bool gameend = DecideJudge(Opponent(turn),Change_state_to_top(board_state));
  		if(limit == 0||gameend){
  			return new int[5]{fft,ffy,ftt,fty,gameend==true?(-1000000):evaluate(board_state)};//負けたかどうか判定
  		}
		Canmovepieces = MovePieces(Change_state_to_top(board_state),turn);	
		for(int d=0;d<Canmovepieces.Count;d++){
			piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn,board_state);
			for(int f = 0;f<piecemoveways.Count;f++){
				Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
			}
		}//出せる手を検索
   		int[] score_max = {-1,-1,-1,-1,int.MinValue};
  		foreach(int[] sw in Selectways){	 
			int[,] board_statecop = new int[row+2,line+2];
			Array.Copy(board_state, board_statecop, board_state.Length);	
			move(sw[0],sw[1],sw[2],sw[3],board_statecop,turn);
  			int[] score;
  			if(Flimit == limit){
   				score = minlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),sw[0],sw[1],sw[2],sw[3]);//次の相手の手
  			}else{
		 		score = minlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),fft,ffy,ftt,fty);
  			}
  			//ここを削ればminimaxになる
		 	if (score[4] >= beta) { // beta値を上回ったら探索を中止
   	 			return score;
			}
			if(score[4]>score_max[4]){ //より良い手が見つかった
				score_max = score;	  
				alpha = Math.Max(alpha, score_max[4]);//α値を更新
			}
  		}
	  	return score_max; 
	}
	//相手の手を調べる
	int[] minlevelab(int alpha,int beta,int limit,int Flimit,int[,] board_state,int turn,int fft,int ffy,int ftt,int fty) {
		List<int[]> Selectways =new List<int[]>();
		bool gameend =DecideJudge(Opponent(turn),Change_state_to_top(board_state));
	  	if(limit == 0||gameend){
	 		return new int[5]{fft,ffy,ftt,fty,(gameend==true)?(1000000):evaluate(board_state)};//勝ったかどうか判定
	  	} 	
		Canmovepieces = MovePieces(Change_state_to_top(board_state),turn);	
		for(int d=0;d<Canmovepieces.Count;d++){
			piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn,board_state);
			for(int f = 0;f<piecemoveways.Count;f++){
				Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
			}
		}//出せる手を検索
 		int[] score_min = {-1,-1,-1,-1,int.MaxValue};
  		foreach(int[] sw in Selectways) {
			int[,] board_statecop = new int[row+2,line+2];
			Array.Copy(board_state, board_statecop, board_state.Length);
			move(sw[0],sw[1],sw[2],sw[3],board_statecop,turn);	
   			//手を打つ;
    		int[] score = maxlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),fft,ffy,ftt,fty);//次の自分の手
			//ここを削ればminimaxになる
	  		if (score[4] <= alpha) {
      			return score;     // alpha値を上回ったら探索を中止
    		}
			if (score[4] < score_min[4]) {	 // より悪い手（相手にとって良い手）が見つかった
      			score_min = score;
	    		beta = Math.Min(beta,score_min[4]);// β値を更新
			}
		}
  		return score_min;
	}
	//駒を動かす
	void move(int from_x,int from_z,int to_x,int to_z,int[,] board_state,int turn){
		int originalPos=board_state[from_x,from_z];
		int movedPos=board_state[to_x,to_z];
		// 移動元の処理
		if(originalPos<=2){ //　一段
			board_state[from_x,from_z]=0;		
		}else if(originalPos<=6){ // 二段
			board_state[from_x,from_z]-=turn<<1;
		}else if(originalPos<15){ // 三段
			board_state[from_x,from_z]-=turn<<2;
		}
		// 移動先の処理
		if(movedPos==0){ // 空白
			board_state[to_x,to_z]+=turn;
		}else if(movedPos<=2){ // 一段
			board_state[to_x,to_z]+=turn<<1;
		}else if(movedPos<=6){ // 二段
			board_state[to_x,to_z]+=turn<<2;
		}
	}
	//動けるコマ
	public List<int[]> MovePieces(int[,] board_top,int turn){
		List<int[]> res=new List<int[]>();					
		for(int a=1;a<row+1;a++){
			for(int b=1;b<line+1;b++){
				if(board_top[a,b]==turn){
					res.Add(new int[2] {a,b});		
				}
			}
		}
		return res;
	}
	//駒の移動できる範囲
	public List<int[]> canMove(int r,int l,int turn,int[,] board_state){
		List<int[]> res=new List<int[]>();
		for(int i=-1; i<=1; i++){
			for(int j=-1; j<=1; j++){
				if(board_state[r+i,l+j]<=6 && !(i==0 && j==0)){
					if(!(turn==Black && l+j==0) && !(turn==White && l+j==line+1)){
						res.Add(new int[2] {r+i,l+j});
					}
				}
			}
		}
		return res;
	}
	//対戦相手
	public int Opponent(int turn){
		return turn == Black?White:Black;
	}
 	int evaluate(int[,] board){
   		int score=0;
   		for(int r=1; r<=row; r++){
    		for(int c=1; c<=line; c++){
     			int board_val=board[r,c];
     			if(board_val==0){// 空白のマスは何もしない
     			}else if(board_val==1|| board_val==3 || board_val==4 || (board_val>=7 && board_val<=10)){
      				score+=(1<<c); //黒のとき
     			}else{
      				score-=(1<<(line+1-c)); //白のとき
     			}
    		}
   		}
   		return (AIColor==Black)?score:-score;
  	}
	public bool DecideJudge(int turn,int[,] board_top){
		bool j = true;
		for(int a=1;a<row+1;a++){
			for(int b=1;b<line+1;b++){
				if (board_top[a,b] == Opponent(turn)){j = false;}
			}
			if(board_top[a,turn ==Black?line+1:0]==turn){return true;}
		}
		return j;
	}
	public int[,] Change_state_to_top(int[,] board_state){
		int[,] newboard = new int[row+2,line+2];
		for(int a=1;a<row+1;a++){
			for(int b=0;b<line+2;b++){
				if((7<=board_state[a,b]&&board_state[a,b]<=10)||(3<=board_state[a,b]&&board_state[a,b]<=4)||(board_state[a,b]==1)){newboard[a,b] = Black;}
				else if((11<=board_state[a,b]&&board_state[a,b]<=14)||(5<=board_state[a,b]&&board_state[a,b]<=6)||(board_state[a,b]==2)){newboard[a,b] = White;}
			}
		}
		return newboard;
	}
}


