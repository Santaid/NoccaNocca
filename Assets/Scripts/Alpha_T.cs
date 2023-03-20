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
		Debug.Log("NBX: "+tuginotebit[0]+" AFy: "+tuginotebit[1]+" AITX: "+tuginotebit[2]+" AITY: "+tuginotebit[3]+"Score:"+ENDscorebit);
		// Debug.Log("current score: "+evaluate(board_stateAI));
	}
    int[] tuginotebit;int ENDscorebit;int edabit;
	int findinglimit = 7;//何手先まで読むか　最大７くらい
    public void abAIways(int[,] board_state,int turn){
	
		negaalphabit(-50000000, 50000000, findinglimit,downsizeab(board_state), turn);
		//negaalpha(int.MinValue,int.MaxValue...)とすると、-1*int.MinValueがオーバーフローする
		int[] finalscore = maxlevelab(int.MinValue,int.MaxValue,findinglimit,findinglimit,board_state,turn,-1,-1,-1,-1);
		from_x = finalscore[0];
		from_z = finalscore[1];
		to_x = finalscore[2];
		to_z = finalscore[3];
		score = finalscore[4];
	}
	//nega-alpha法
	int negaalphabit(int alpha,int beta ,int limit,/*int[,] board_state,*/int[] board_bit,int turn) {//bitの方
//int[] board_bit = downsizeab(board_state);
//int [,] board_state2 = upsizeab(downsizeab(board_state));
//Console.WriteLine("\n\n\n");
     //   prunt(board_state,-1,-1);prunt(board_state2,-1,-1);
//if(!onaji2d(board_state,upsizeab(board_bit))){Console.Write(onaji2d(board_state,upsizeab(board_bit)));}
	//	Console.Write(onaji2d(board_state,board_state2));
	//prunt(board_state,-1,-1);
      
        bool endJudge = DecideJudgebit(board_bit,3-turn);//Opponent(turn)demoyoshi
     /*
	   bool han = DecideJudge(Opponent(turn),Newboard_tok(board_state));//黒1白2
        if (han != endJudge) { prunt(board_state, -1, -1); //showboards(changetops(board_bit));
            Console.WriteLine("han:"+han+"endjudge"+endJudge);
            Environment.Exit(0); 
			}
			*/

        //		Console.WriteLine(turn);
        //          Console.WriteLine("s:{0} b:{1}ERror",han,endJudge); }
        //   Console.WriteLine(han == endJudge);
        //Console.WriteLine(limit == 0 || han);
        if(limit == 0||endJudge){edabit++;
        //    int n=evaluatenega(board_state, turn);
        //    int s = evaluatenegabit(changetops(board_bit), turn);//-(endJudge==true?64:0);//本番では後ろを消す
        //   if (n != s) {prunt(board_state, -1, -1);//showboards(board_bit);
        //        Console.WriteLine("turn:"+turn+" nega:" + n + " bit:" + s); }
            return (endJudge==true?(-1000000):0)+evaluatenegabit(changetops(board_bit), turn);
	//		 return (endJudge==true?(-1000000):0)+evaluatenega(board_state, turn);
  }

/*
 List<int[]> Selectways =new List<int[]>();
Canmovepieces = MovePieces(Newboard_tok(board_state),turn);	
	for(int d=0;d<Canmovepieces.Count;d++){
		piecemoveways = canMove(Canmovepieces[d][0],Canmovepieces[d][1],turn,board_state);
		for(int f = 0;f<piecemoveways.Count;f++){
		Selectways.Add(new int[4] {Canmovepieces[d][0],Canmovepieces[d][1],piecemoveways[f][0],piecemoveways[f][1]});
		}
	}

*/

        int score_max = int.MinValue;

//List<int[]> Selectwaysdash =new List<int[]>();
 
              int maskdata1 = changetops(board_bit)[turn - 1];
                    while ( maskdata1 != 0)		
                    {
                        int m = Findminbit(maskdata1);
      //                  Console.Write("point:"+m);
            int maskdata2 = canMovebit(~(board_bit[2] | board_bit[5]),m,turn);
			while( maskdata2 != 0){
				int f = Findminbit(maskdata2);
	//			  Console.Write(" direction:"+f);
// Console.WriteLine("{0},{1},{2},{3}, point:{4} direction:{5}",m/6+1, m%6+1,(m/6+1)+(f/3-1),(m%6+1)+(f%3-1),m,f);
//	Selectwaysdash.Add(new int[4] {m/6+1, m%6+1,(m/6+1)+(f/3-1),(m%6+1)+(f%3-1)});

// 	int[,] board_statecop = new int[row+2,line+2];
//		Array.Copy(board_state, board_statecop, board_state.Length);	
//   move(m/6+1, m%6+1,(m/6+1)+(f/3-1),(m%6+1)+(f%3-1),board_statecop,turn);
      int [] newboard_bit = movebit(m,f,board_bit,turn);
          int score = -1*negaalphabit(-beta,-alpha,limit - 1,/*board_statecop,*/newboard_bit,Opponent(turn));//次の相手の手
  //ここを削ればnegamaxになる
      if (score >= beta) {
     // beta値を上回ったら探索を中止
      return score;
    }
if(score>score_max){
  //より良い手が見つかった
   score_max = score;	  
	alpha = Math.Max(alpha, score_max);//α値を更新
	if (findinglimit == limit){//Console.WriteLine("tansakutyuu"+ss[0]+" "+ss[1]+" "+ss[2]+" "+ss[3]);
	tuginotebit = new int[4]{m/6+1, m%6+1,(m/6+1)+(f/3-1),(m%6+1)+(f%3-1)};ENDscorebit = score;
	}

}

				   maskdata2 -= (1 << f);
			}
                        maskdata1 -= (1 << m);
                    }    
           
    //  prunt(board_state, -1, -1);
      //  prunt(upsizeab(board_bit), -1, -1);
      //  showboards(board_bit);
      //Console.WriteLine("Ncount:{0}Dcount:{1}",Selectways.Count,Selectwaysdash.Count);
     //   for (int k = 0; k < Selectways.Count;k++){
	//		Console.WriteLine("N:{0},{1},{2},{3}",Selectways[k][0],Selectways[k][1],Selectways[k][2],Selectways[k][3]);
	//		Console.WriteLine("D:{0},{1},{2},{3}\n",Selectwaysdash[k][0],Selectwaysdash[k][1],Selectwaysdash[k][2],Selectwaysdash[k][3]);
		//}
        //if (Selectways.Count!=Selectwaysdash.Count) { Environment.Exit(0); }

/*
        foreach(int[] ss in Selectwaysdash){	 
	int[,] board_statecop = new int[row+2,line+2];
		Array.Copy(board_state, board_statecop, board_state.Length);	
   move(ss[0],ss[1],ss[2],ss[3],board_statecop,turn);
            //   Console.WriteLine("tansakutyuu"+ss[0]+" "+ss[1]+" "+ss[2]+" "+ss[3]);
            //          Console.WriteLine("point:"+(6*(ss[0]-1)+ss[1]-1)+"direction"+3*(ss[2]-ss[0]+1)+(ss[3]-ss[1]+1));
            
           int [] newboard_bit = movebit(6*(ss[0]-1)+ss[1]-1,3*(ss[2]-ss[0]+1)+(ss[3]-ss[1]+1),board_bit,turn);
      //      prunt(upsizeab(board_bit), -1, -1);
	//		 Console.WriteLine("tansakutyuu"+ss[0]+" "+ss[1]+" "+ss[2]+" "+ss[3]);
    //        Console.WriteLine("point"+(6*(ss[0]-1)+ss[1]-1)+"dedfa"+(3*(ss[2]-ss[0]+1)+(ss[3]-ss[1]+1)));
  //          showboards(board_bit);
//	  showboards(newboard_bit);
	  
     //      if (!onaji2d(board_statecop, upsizeab(newboard_bit))) {
	//			prunt(board_state,-1,-1);prunt(board_statecop,-1,-1);
	//			prunt(upsizeab(newboard_bit),-1,-1);
	//		Console.WriteLine("ERROD!");Environment.Exit(0);
      //    }



            int score = -1*negaalphabit(-beta,-alpha,limit - 1,board_statecop,newboard_bit,Opponent(turn));//次の相手の手
  //ここを削ればnegamaxになる
      if (score >= beta) {
     // beta値を上回ったら探索を中止
      return score;
    }
if(score>score_max){
  //より良い手が見つかった
   score_max = score;	  
	alpha = Math.Max(alpha, score_max);//α値を更新
	if (findinglimit == limit){//Console.WriteLine("tansakutyuu"+ss[0]+" "+ss[1]+" "+ss[2]+" "+ss[3]);
	tuginotebit = ss;ENDscorebit = score;
	}

}
  }

*/


  return score_max;
  
}

public int[] downsizeab(int[,] board_2d){
	int[] board_1d=new int[6];
	for(int f=1;f<row+1;f++){
		for (int g=1;g<line+1;g++){
	int help1,help2;
		if(board_2d[f,g]>= 11){help1 = White;board_1d[5]+=1<<(6*(f-1)+g-1);}
		else if(board_2d[f,g] >= 7){help1 = Black;board_1d[2]+=1<<(6*(f-1)+g-1);}
		else help1 = 0;//一番高い
		if(board_2d[f,g]-(4*help1)>=5){help2 = White;board_1d[4]+=1<<(6*(f-1)+g-1);}
		else if (board_2d[f,g]-(4*help1)>=3){help2 = Black;board_1d[1]+=1<<(6*(f-1)+g-1);}
		else help2 = 0;//二番目
		if(board_2d[f,g]-(4*help1)-(2*help2)>=2){board_1d[3]+=1<<(6*(f-1)+g-1);}
		else if (board_2d[f,g]-(4*help1)-(2*help2)>=1){board_1d[0]+=1<<(6*(f-1)+g-1);}
		//三番目
		}
	}
	return board_1d;
}

public int[] changetops(int[] board_6){
int[] board_2 = new int[2];
//board_2[0] = board_6[2]|(board_6[1] & ~board_6[5])|(board_6[0] & ~board_6[4] & ~board_6[5]);//変形
board_2[0] = board_6[2]| ~board_6[5]&(board_6[1]|board_6[0] & ~board_6[4]);
//board_2[1] =  board_6[5]|(board_6[4] & ~board_6[2])|(board_6[3] & ~board_6[1] & ~board_6[2]);//変形
board_2[1] = board_6[5]| ~board_6[2]&(board_6[4]|board_6[3] & ~board_6[1]);
return board_2; 
}

public int bitcount(int x){
        x = x - ((x >> 1) & 0x55555555);
        x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
        x = (x + (x >> 4)) & 0x0f0f0f0f;
        x = x + (x >> 8);
        x = x + (x >> 16);
        return x & 0x003f;
    }

bool DecideJudgebit(int[] board_bit,int turn){
    //    bool outzone = row!=bitcount(board_bit[3*(turn-1)])+bitcount(board_bit[3*(turn-1)+1])+bitcount(board_bit[3*(turn-1)+2]);
      //  bool pressed = changetops(board_bit)[2-turn] == 0;// 1 -> 1 2 -> 0  0 ->1 1->0//もともとturn-1
    

	
	  //  Console.WriteLine("turn:"+turn);
       // showboards(changetops(board_bit));
        //   showbits(changetops(board_bit)[turn-1] );
        //     Console.WriteLine(turn);//1 -> 1 2 -> 0
        //    if (pressed == true) { Console.WriteLine("Emee"); Environment.Exit(0); }
        //     showboards(board_bittop);
        //      Console.WriteLine(bitcount(board_bittop[turn-1])+"dddes");
        //  showboards(board_bit);
        //  Console.WriteLine(turn);
        //  Console.WriteLine("1段目:{0}",bitcount(board_bit[3*(turn-1)]));
        //	 Console.WriteLine("2段目:{0}",bitcount(board_bit[3*(turn-1)+1]));	
        //	  Console.WriteLine("3段目:{0}",bitcount(board_bit[3*(turn-1)+2]));	
		

        return (row!=bitcount(board_bit[3*(turn-1)])+bitcount(board_bit[3*(turn-1)+1])+bitcount(board_bit[3*(turn-1)+2])) || (changetops(board_bit)[2-turn] == 0);
        //return outzone || pressed;
    }

 	int evaluatenegabit(int[] board_bittop,int turn){
		//初めから入力をトップにする　返値はとりあえずint
		// 1 2 4 8 16 32 kuro
		// -32 -16 -6 -4 -2  -1 shiro 
		//int x0 =  0b00111111000000111111000000000000//1057222656
		//int y0 =  0b00000000111111000000111111111111;//16519167
        board_bittop[0] = ((board_bittop[0] & 1057222656) >> 6) +(board_bittop[0] & 16519167);//1列目、2と3列目の合計, 4と5列目の合計
        board_bittop[0] = (board_bittop[0] & 0b111111) + (board_bittop[0] >> 6);//1列目と2列目の合計
		board_bittop[0] += (board_bittop[0] >> 12);//合計
    	board_bittop[0] &= 0b111111111111;
        //int x1 = 0b00000111000111000111000111000111;//119304647
        //int x2 = 0b00111000111000111000111000111000;//954437176
		//int x3 = 0b00001001001001001001001001001001;//153391689
		//int x4 = 0b00010010010010010010010010010010;//306783378
		//int x5 = 0b00100100100100100100100100100100;//613566756
        //交換始め
		board_bittop[1] = (board_bittop[1] & 119304647)<<3 | (board_bittop[1] & 954437176) >> 3;//123456 -> 456123
        board_bittop[1] = (board_bittop[1] & 153391689) << 2|board_bittop[1] & 306783378|(board_bittop[1] & 613566756) >> 2;;//456123 -> 654321
		//交換終了
	    board_bittop[1] = ((board_bittop[1] & 1057222656) >> 6) +(board_bittop[1] & 16519167);//1列目、2と3列目の合計, 4と5列目の合計
        board_bittop[1] = (board_bittop[1] & 0b111111) + (board_bittop[1] >> 6);//1列目と2列目の合計
		board_bittop[1] += (board_bittop[1] >> 12);//合計
    	board_bittop[1] &= 0b111111111111;
        return (board_bittop[0]-board_bittop[1])*(3-2*turn);//1->1 2->-1
    }

    int canMovebit(int board_basebit,int point,int turn){//~(board_bit[2] | board_bit[5])を必ず入力	
        //int bas = 0b111000101000111;//28999
        //int mask1 = 0b000100000100000100;//16644
        //int mask2 = 0b000001000001000001;//4161
        //int mask3 = 0b000111000000000000;//28672
        if (point <= 7) {board_basebit <<= 7 - point;} 
		 else {	board_basebit >>= point - 7;
		 }	
        board_basebit &= 28999;
		

        if ((point % line) == 0 && turn == Black) {//Console.WriteLine("AD"); 
		board_basebit &= ~4161;}//左端から左へいかないように
		if ((point % line) == line - 1 && turn == White) {//Console.WriteLine("##2");
		board_basebit &= ~16644;}//右端から右へ行かないように
	 	if ((point % line) == line-1 && turn == Black) {//Console.WriteLine("RRFF"); 
		board_basebit |= 16644;}//右端から右へいけるように
		if ((point % line) == 0 && turn == White) {//Console.WriteLine("$DDD");
		board_basebit |= 4161;}//左端から左へ行けるように
    
        if(point <= 5){ //Console.WriteLine("ppp");
		board_basebit &= ~(0b111);}
		 if (point >= 24) {// Console.WriteLine("ddd");
		 board_basebit &= ~28672;}


        board_basebit |= (board_basebit >>3);
        board_basebit = (board_basebit & 0b111111) | ((board_basebit >>3)&0b111000000) ;
        return board_basebit;
    }

		int[] movebit(int point,int direction,int[] board_bit,int turn){
		     direction += 3*(direction / 3) - 7;
   
	//    Console.WriteLine("po:"+point+"di:"+direction+"turn:"+turn);
		int[] board_bitcop = new int[6];
		Array.Copy(board_bit, board_bitcop, board_bit.Length);	
	//	board_bitcop[Opponent(turn) * 1 - 1] = board_bit[Opponent(turn) * 1 - 1];
      //  board_bitcop[Opponent(turn) * 2 - 1] = board_bit[Opponent(turn) * 2 - 1];
        //board_bitcop[Opponent(turn) * 3 - 1] = board_bit[Opponent(turn) * 3 - 1];
       // showboards(board_bitcop);
        // 0 1 2 3 4 5 
        // 6 7 8 91011
        //0 1 2     -7 -6 -5   
        //3 4 5  -> -1  0  1   
        //6 7 8      5  6  7   
        //if ((changetops(board_bit)[turn -1]>>point & 1) != 1) { Console.WriteLine("Error: NO PIECE {0}",turn); }//求める駒がない　減らない
        //if ((((~(board_bit[2] | board_bit[5])) >> (point + direction)) & 1) == 0) { Console.WriteLine("Error: CAN'T PLACE");}//満杯　増やせない
        //0にすることで1を0にする 
            board_bitcop[3*(turn-1)] &= (~board_bit[3*(turn-1)] | board_bit[3*(turn-1)+1] |board_bit[3*(turn-1)+2]|~(1<<point));
        // 110 -> 1 101 -> 1 100 -> 0 111-> 1
            board_bitcop[3*(turn-1)+1] &= (~board_bit[3*(turn-1)+1] | board_bit[3*(turn-1)+2]|~(1<<point));
        // 10 -> 0 11 -> 1
            board_bitcop[3*(turn-1)+2] &= ~(1<<point);
        // 必ず0

		     int add=1 << point + direction;
        if((point %6 == 0 &&  (point + direction +6) % 6 == 5 || point %6 == 5 && ( point + direction+6) % 6 == 0 )){//そのうち改良
            add = 0;//(point,direction) = (0,-1)のような場合に備えて+6されている さもなくばシフトが-1になって大変
        }
//        Console.WriteLine("BAKAKAKA"+3*(turn-1));showbits( board_bitcop[3*(turn-1)]);showbits(add);
        /*
		      board_bit[turn * 1 - 1] &= (~board_bit[turn * 1 - 1] | board_bit[turn * 2 - 1] |board_bit[turn * 3 - 1]|~(1 << point));
		// 110 -> 1 101 -> 1 100 -> 0 111-> 1
        board_bit[turn * 2 - 1] &= (~board_bit[turn * 2 - 1] | board_bit[turn * 3 - 1]|~(1 << point));
        // 10 -> 0 11 -> 1
        board_bit[turn * 3 - 1] &= ~(1 << point);
		*/
        //      showbits(add);
        //    showbits((board_bit[1] | board_bit[4]) & ~(board_bit[2] | board_bit[5]) & (1 << (point + direction)));
        //  showbits(((board_bit[0] | board_bit[3])& ~(board_bit[1] | board_bit[4]) & (1 << (point + direction))));
        //showbits( ~(board_bit[0] |board_bit[3]) & (1 << (point + direction)));
        //1にすることで0を1にする 
        board_bitcop[3*(turn-1)+2] |= ((board_bit[1] | board_bit[4]) & ~(board_bit[2] | board_bit[5]) & add);
        //2段目が埋まっており、3段目が埋まっていない
        board_bitcop[3*(turn-1)+1] |=((board_bit[0] | board_bit[3])& ~(board_bit[1] | board_bit[4]) & add);
        //1段目が埋まっており、2段目が埋まっていない
            board_bitcop[3*(turn-1)] |= ~(board_bit[0] |board_bit[3]) & add;
        //1段目が埋まっていない
	//	  Console.WriteLine("BKAKA\n\n"+3*(turn-1));
	//	  showbits(~(board_bit[0] |board_bit[3]) & add);
	//	   showbits(~(board_bit[0] |board_bit[3]));
		  //   Console.WriteLine("point:{0} direction:{1} add:{2} divide:{3}",point,direction,point+direction,(point+direction)%6);showbits(add);
		//  showbits( board_bitcop[3*(turn-1)]);
        return board_bitcop;
    }

		int Findminbit(int x){
        int n=0;
        if ((x & 0xffff) == 0) { n += 16;x >>= 16; }
        if ((x & 0x00ff) == 0) { n += 8; x >>= 8; }
        if ((x & 0x000f) == 0) { n += 4; x >>= 4; }
        if ((x & 0x0003) == 0) { n += 2; x >>= 2; }
        if ((x & 0x0001) == 0) { n += 1;}
        return n;
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
			// 勝敗が確定する盤面
			if(board[r,1]==9){
				// 実質的に白の駒に２個を抑えられてる状態
				score-=10000;
			}
			if(board[r,line]==12){
				// 実質的に白の駒２個を抑えてる状態
				score+=10000;
			}

			for(int c=1; c<=line; c++){
				int board_val=board[r,c];
				if(board_val==0){// 空白のマスは何もしない
				}else if(board_val==1|| board_val==3 || board_val==4 || (board_val>=7 && board_val<=10)){
					score+=(1<<c); //黒のとき
					if(board_val==10){
						// 黒が白を2個押さえたら勝ち確
						score+=10000;
					}
				}else{
					score-=(1<<(line+1-c)); //白のとき
					if(board_val==11){
						// 白が黒を2個押さえたら負け確
						score-=10000;
					}
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