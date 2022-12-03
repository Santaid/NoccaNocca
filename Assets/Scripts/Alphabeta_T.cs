// See https://aka.ms/new-console-template for more information


using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
//abの本格的なのはwayforab2
public class Alphabeta_T : MonoBehaviour {
    
    public static Alphabeta_T instance;
    public Alphabeta abAI;
	int[] result;
    public int from_x;
    public int from_z;
    public int to_x;
    public int to_z;
    public void Awake(){
		if(instance == null){
			instance = this;
		}
	}
    public void ABAI() {
        int[,] board_info = GameMainScript.instance.board_state;
		int turn = GameMainScript.instance.Turn;
        result = abAI.abAIways(board_info,turn);
        from_x = result[0];
        from_z = result[1];
        to_x = result[2];
        to_z = result[3];
    }
}
public class Alphabeta{
public const int White=2;
	public const int Black=1;
	public int Wall=15;
	public int row;
	public int line;
	public int turn = Black;
	private int AIColor;
    public int[,] board_state;
	public int eda;
	[SerializeField] private GameObject AIComponent;
	private GameObject[] AIPieces;
	//public int[,] board_top;		board_top = Newboard(board_state);
	//Random r = new Random();
		void Start(){
		AIPieces = GameObject.FindGameObjectsWithTag(AIComponent.tag);
		if(AIComponent.tag == "player_black"){
			AIColor = GameMainScript.instance.Black;
		}else if(AIComponent.tag == "player_white"){
			AIColor = GameMainScript.instance.White;
		}
	}
	void Update(){

	}


/*
GameMainScript(int row,int line){
        this.row = row;
        this.line = line;
        		board_state = new int[row + 2, line + 2];
		board_top=new int[row+2,line+2];
    }*/

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
	List<int[]> MovePieces(int[,] board_top,int turn){
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
	List<int[]> canMove(int r,int l,int turn){
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
	//盤面を表示
void prunt(int[,] board_state,int Fromwaytate,int Fromwayyoko){

	for(int f=0;f<row+2;f++){
		for (int g=0;g<line+2;g++){
	int help1,help2;
	if(f==Fromwaytate&&g==Fromwayyoko){Console.Write("[");}	else {Console.Write(" ");}
	if(board_state[f,g]==15){Console.Write("wal");}
	else {
		if(board_state[f,g]>= 11){help1 = White;Console.Write("W");}
		else if(board_state[f,g] >= 7){help1 = Black;Console.Write("B");}
		else {help1 = 0;Console.Write(" ");}//一番高い
		if(board_state[f,g]-(4*help1)>=5){help2 = White;Console.Write("W");}
		else if (board_state[f,g]-(4*help1)>=3){help2 = Black;Console.Write("B");}
		else {help2 = 0;Console.Write(" ");}//二番目
		if(board_state[f,g]-(4*help1)-(2*help2)>=2){Console.Write("W");}
		else if (board_state[f,g]-(4*help1)-(2*help2)>=1){Console.Write("B");}
		else {Console.Write(" ");}//三番目
	}
	if(f==Fromwaytate&&g==Fromwayyoko){Console.Write("]");}else {Console.Write(" ");}
		}
	Console.WriteLine("");
	}
}

 int Fromwaytate=0,Fromwayyoko=0;
 int Towaytate=0,Towayyoko=0;
 List<int[]> Canmovepieces = new List<int[]>();
  List<int[]> piecemoveways = new List<int[]>();
//全体を動かす



public int[] abAIways(int[,] board_state,int turn){
//Console.WriteLine("ddddd"+turn);
List<int[]> Selectways =new List<int[]>();	
Canmovepieces = MovePieces(Newboard_tok(board_state),turn);	
	for(int d=0;d<Canmovepieces.Count;d++){
		piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn);
		for(int f = 0;f<piecemoveways.Count;f++){
		Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
		}
	}
int findinglimit = 7;//なんて先まで読むか　最大７くらい
var sw = new System.Diagnostics.Stopwatch();
eda = 0;
sw.Start();
int[] Scorll = maxlevelab(int.MinValue,int.MaxValue,findinglimit,findinglimit,board_state,turn,-1,-1,-1,-1);
sw.Stop();
Console.WriteLine("かかった時間");
TimeSpan ts = sw.Elapsed;
Console.WriteLine($"{ts}");
Console.WriteLine($"{ts.Hours}時間 {ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");

Console.WriteLine($"枝の数:{eda}");
Console.WriteLine("IFX: "+Scorll[0]+" AFy: "+Scorll[1]+" AITX: "+Scorll[2]+" AITY: "+Scorll[3]+"Score:"+Scorll[4]);
return Scorll;
}

int[] maxlevelab(int alpha,int beta ,int limit,int Flimit,int[,] board_state,int turn,int fft,int ffy,int ftt,int fty) {
List<int[]> Selectways =new List<int[]>();	
bool han = DecideJudge(Opponent(turn),Newboard_tok(board_state));//黒1白2
  if(limit == 0||han){eda++;//ここのdecidejudgeは負け確
// if(han==true){Console.WriteLine(limit+"ss"+turn);prunt(board_top,-1,-1);}
  return new int[5]{fft,ffy,ftt,fty,han==true?(-1000000):evaluate(board_state)/*Score_imp(board_state,board_top,turn)*/};
 /* 
  if(limit ==0){return new int[5]{fft,ffy,ftt,fty,Score_imp(board_state,board_top,turn)};}
  else /*Console.WriteLine("まけ");prunt(board_state,-1,-1);return new int[5]{fft,ffy,ftt,fty,int.MinValue};
  */
  }//  limit==0?Score_imp(board_state,board_top,turn):int.MinValue
 	

Canmovepieces = MovePieces(Newboard_tok(board_state),turn);	
	for(int d=0;d<Canmovepieces.Count;d++){
		piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn);
	//	Console.WriteLine("  cd ");
		for(int f = 0;f<piecemoveways.Count;f++){
		Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
		}
	}
/*
prunt(board_state,-1,-1);
	for(int f = 0;f<Selectways.Count;f++){
	Console.WriteLine("FromXmax:"+Selectways[f][0]+"FROMy:"+Selectways[f][1]+"toX"+Selectways[f][2]+"toY"+Selectways[f][3]+"turn:"+turn);
		}
*/

   int[] score_max = {-1,-1,-1,-1,int.MinValue};
  foreach(int[] ss in Selectways){	 
	int[,] board_statecop = new int[row+2,line+2];
//	int[,] board_topcop = new int[row+2,line+2];
		Array.Copy(board_state, board_statecop, board_state.Length);
//		Array.Copy(board_top, board_topcop, board_top.Length);	
   move(ss[0],ss[1],ss[2],ss[3],board_statecop,turn);
 //  prunt(board_statecop,-1,-1);
  int[] score;
  if (Flimit == limit){
	/*
	prunt(board_statecop,-1,-1);
	for(int f = 0;f<Selectways.Count;f++){
	Console.WriteLine("FromXmax:"+Selectways[f][0]+"FROMy:"+Selectways[f][1]+"toX"+Selectways[f][2]+"toY"+Selectways[f][3]+"turn:"+turn);
		}
	Console.WriteLine("dfa"+ss[0]+ss[1]+ss[2]+ss[3]);*/
   score = minlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),ss[0],ss[1],ss[2],ss[3]);//次の相手の手
  }else {
	 score = minlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),fft,ffy,ftt,fty);
  }
      if (score[4] >= beta) {
	//		 Console.WriteLine("FREEZE");
     // beta値を上回ったら探索を中止
      return score;
    }

	/*//テスト用　ランダム性
  if (score[4] >= score_max[4]) {
if(!(score[4]==score_max[4]&&r.Next(1,11)>5)){
   score_max = score;
	  
	   alpha = Math.Max(alpha, score_max[4]);//α値を更新
}
  }
*/

 // if(score[4]==score_max[4]){Console.Write("THE SCNE!");}
    if (score[4] > score_max[4]) {
	//	 Console.WriteLine("CHANGE");
	//  Console.WriteLine("FromX"+ss[0]+"FromY"+ss[1]+"ToX"+ss[2]+"ToX"+ss[3]+"Score"+score[4]);	
  //    より良い手が見つかった
      score_max = score;
	  
	   alpha = Math.Max(alpha, score_max[4]); //α値を更新
    }
  }
//  Console.WriteLine("Total Score:"+score_max[4]);
  return score_max;
  
}
//相手の手を調べる
int[] minlevelab(int alpha,int beta,int limit,int Flimit,int[,] board_state,int turn,int fft,int ffy,int ftt,int fty) {
List<int[]> Selectways =new List<int[]>();
bool han =DecideJudge(Opponent(turn),Newboard_tok(board_state));//黒が1白が2
  if(limit == 0||han){eda++;//ここのdecidejudgeは勝ち確
//  if(han==true){Console.WriteLine(limit+"dd"+turn);prunt(board_top,-1,-1);}
 return new int[5]{fft,ffy,ftt,fty,(han==true)?(1000000):evaluate(board_state)/*Score_imp(board_state,board_top,turn)*/};
/*  
  if(han==true){Console.Write("かち");prunt(board_state,-1,-1);return new int[5]{fft,ffy,ftt,fty,int.MaxValue};}
  else{/*Console.Write("えええ"+han+Opponent(turn));prunt(board_state,-1,-1);prunt(board_top,-1,-1);return new int[5]{fft,ffy,ftt,fty,Score_imp(board_state,board_top,turn)};}
//  limit==0?Score_imp(board_state,board_top,turn):int.MaxValue
*/
  }
 	
Canmovepieces = MovePieces(Newboard_tok(board_state),turn);	
	for(int d=0;d<Canmovepieces.Count;d++){
	//	piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],(turn == Black?White:Black));
	piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn);

		for(int f = 0;f<piecemoveways.Count;f++){
		Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
		}
	}
/*
prunt(board_state,-1,-1);
	for(int f = 0;f<Selectways.Count;f++){
	Console.WriteLine("FromXmin:"+Selectways[f][0]+"FROMy:"+Selectways[f][1]+"toX"+Selectways[f][2]+"toY"+Selectways[f][3]+"turn:"+turn);
		}
*/
//   Console.WriteLine("dd"+Selectways.Count);
//  可能な手を全て生成;
 int[] score_min = {-1,-1,-1,-1,int.MaxValue};
  foreach(int[] mmd in Selectways) {
	int[,] board_statecop = new int[row+2,line+2];
	//int[,] board_topcop = new int[row+2,line+2];
		Array.Copy(board_state, board_statecop, board_state.Length);
	//	Array.Copy(board_top, board_topcop, board_top.Length);	
// move(mmd[0],mmd[1],mmd[2],mmd[3],board_statecop,board_topcop,(turn == Black?White:Black));
 move(mmd[0],mmd[1],mmd[2],mmd[3],board_statecop,turn);	
    //手を打つ;
     int[] score = maxlevelab(alpha,beta,limit - 1,limit,board_statecop,Opponent(turn),fft,ffy,ftt,fty);//次の自分の手
	  if (score[4] <= alpha) {
     // alpha値を上回ったら探索を中止
      return score;
    }

/*//テスト　同じ値の時ランダムにする
  if (score[4] <= score_min[4]) {
if(!(score[4]==score_min[4]&&r.Next(1,11)>5)){
      score_min = score;
	    beta = Math.Min(beta,score_min[4]);// β値を更新
}
  }
*/
    if (score[4] < score_min[4]) {
    // より悪い手（相手にとって良い手）が見つかった
      score_min = score;
	    beta = Math.Min(beta,score_min[4]);// β値を更新
    }
  }
  return score_min;
}

//対戦相手
int Opponent(int turn){
return turn == Black?White:Black;
}


// 自分が黒であるとして計算
  int evaluate(int[,] board){
   int score=0;
   for(int r=1; r<=row; r++){
    for(int c=1; c<=line; c++){
	//	Console.WriteLine("row"+r+"line"+c);
     int board_val=board[r,c];
     if(board_val==0){
      // 空白のマスは何もしない
     }else if(board_val==1|| board_val==3 || board_val==4 || (board_val>=7 && board_val<=10)){
      score+=(1<<c); //黒のとき
	//  Console.WriteLine("df"+(1<<(line+1-c)));
     }else{
      score-=(1<<(line+1-c)); //白のとき
	 // Console.WriteLine("dhew"+(1<<(c)));
     }
    }
   }

   return (this.turn==Black)?score:-score;
  }


/*
int Score_imp(int[,] board_state,int[,] board_top,int turn){
//	prunt(board_state,-1,-1);//tttest
	int point=0;
	int nowturn = turn;
	int JIBUN = this.turn;
	int AITE = Opponent(this.turn);
	string f = (nowturn==Black)?"Black":"white";
	string f2 = (JIBUN==Black)?"Black":"white";
	//prunt(board_state,-1,-1);
	//Console.WriteLine("今のターンは:"+f+" 自分のAIは:"+f2);
//	if(DecideJudge(JIBUN,board_top)){Console.WriteLine("今のターンは:"+f+" 自分のAIは:"+f2);prunt(board_state,-1,-1);Console.Write("ENA");return int.MaxValue;}//まさか逆?現在これがよし
//	if(DecideJudge(AITE,board_top)){Console.WriteLine("今のターンは:"+f+" 自分のAIは:"+f2);prunt(board_state,-1,-1);Console.Write("ENdA");return int.MinValue;}
	//Console.WriteLine("AIT"+JIBUN+"line:"+nowturn);
	//自身と自身の手が同じの際は近づく(finginhlimitが偶数)野はいい、逆の場合はわるい(finginhlimitが奇数)
	//jibunがwhite、nowturnがblackの場合は悪い
for(int a=1;a<row+1;a++){
	for(int b=1;b<line+1;b++){
	if(board_top[a,b]==JIBUN){point = point+Math.Abs(b-(JIBUN==Black?0:line))*2;//先に進むほどよい、用改良
	if(JIBUN==Black){//AIが黒
	if(3<=board_state[a,b]&&board_state[a,b]<=4){point += 3;}
	if(7<=board_state[a,b]&&board_state[a,b]<=10){point += 5;}//一段より二段、二段より散弾
	}
	else{//AIが白
	if(5<=board_state[a,b]&&board_state[a,b]<=6){point += 3;}
	if(11<=board_state[a,b]&&board_state[a,b]<=14){point += 5;}	
	} 
	foreach(int[] mixa in canMove(a,b,JIBUN)){
		if(board_top[mixa[0],mixa[1]] == AITE){
			if(JIBUN==nowturn){if(board_state[mixa[0],mixa[1]]<=6)point += 1;}else{point -= 1;}//近くにいた場合、同じならよく違うなら悪い
			}//自分単体の時、相手の進路上にいると得点をもらえるようにするか考え中
	}
	}
	}
}

return point;
}
*/
/*
bool Judge(List<int[]> t,int waytate,int wayyoko){//waytateとwayyokoの一致を当てる
	foreach(int[] ss in t){
			if(ss.SequenceEqual(new int[2] {waytate,wayyoko})){return false;}
		}
	return true;
}*/

 bool DecideJudge(int turn,int[,] board_top){
	bool j = true;
for(int a=1;a<row+1;a++){
	for(int b=1;b<line+1;b++){
	if (board_top[a,b] == Opponent(turn)){j = false;}
	}
//if(board_state[a,0]!=0||board_state[a,line+1]!=0){return true;}
if(board_top[a,turn ==Black?line+1:0]==turn){return true;}
}
return j;
	}
int[,] Newboard_tok(int[,] board_state){
	int[,] newboard = new int[row+2,line+2];
for(int a=1;a<row+1;a++){
	for(int b=1;b<line+1;b++){
			if((7<=board_state[a,b]&&board_state[a,b]<=10)||(3<=board_state[a,b]&&board_state[a,b]<=4)||(board_state[a,b]==1)){newboard[a,b] = Black;}
		else if((11<=board_state[a,b]&&board_state[a,b]<=14)||(5<=board_state[a,b]&&board_state[a,b]<=6)||(board_state[a,b]==2)){newboard[a,b] = White;}
	}
}
return newboard;
}


uint[] downsize(int[,] board_2d){
	uint[] board_1d=new uint[row+2];
	board_1d[0]=0xffffffff;
	board_1d[row+1]=0xffffffff;

	for(int i=1; i<=row; i++){
		for(int j=0; j<=line+1; j++){
	//		Console.Write((uint)board_2d[i,j]<<(4*j));
	//		Console.Write(" ");
			board_1d[i] += (uint)(board_2d[i,j]<<(4*j));
		//	board_1d[i] = board_1d[i]<<(4*j);
		//	int a = 4|=2;
		}
	//	Console.WriteLine();
	}
	return board_1d;
}



}