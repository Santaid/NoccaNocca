using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class Alpha_T1 : MonoBehaviour//MonoBehaviourやUnity系は必要か不明
{
	public GameObject AIComponent = null; //GameMainにAIをつける駒を設定する必要ある
	public GameObject AIComponentW; //GameMainにAIをつける駒を設定必要ある
	public GameObject AIComponentB; //GameMainにAIをつける駒を設定必要ある
	public static Alpha_T1 instance;
	private GameObject[] AIPieces; //AIがコントロールする駒の配列
	private int AIColor; //AI駒の色
	private int row,line,Black,White;
	public int from_x,from_z,to_x,to_z,score;
	public void Awake(){
		if(instance == null){
			instance = this;
		}
	}
	void Start(){
		if(AI_Tozaki1.instance.White == true){
			AIComponent = AIComponentW;
		}else if(AI_Tozaki1.instance.Black == true){
			AIComponent = AIComponentB;
		}
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
	//bitboardはint[6] 　黒一段目　黒二段目　黒三段目 白一段目 白二段目　白三段目 それぞれ6*5の30ビット　駒が存在していると1、していないと0　最初の2ビットは使わない
	public void AIScript(int[,] board_stateAI,int AIColor){
		abAIways(board_stateAI,AIColor);
		Debug.Log("from_x"+from_x+"from_z"+from_z+"to_x"+to_x+"to_z"+to_z+"score"+score);
	}
	int findinglimit = 7;//何手先まで読むか　最大７くらい
    public void abAIways(int[,] board_state,int turn){
		negaalphabit(-50000000, 50000000, findinglimit,downsizeab(board_state), turn);
		//negaalpha(int.MinValue,int.MaxValue...)とすると、-1*int.MinValueがオーバーフローする
	}
	int negaalphabit(int alpha,int beta ,int limit,int[] board_bit,int turn) {//negaalphaを行う
        bool endJudge = DecideJudgebit(board_bit,3-turn);//Opponent(turn)としてもよし 1->2 2->1
        if(limit == 0||endJudge){
            return (endJudge==true?(-1000000):0)+evaluatenegabit(changetops(board_bit), turn);
		}
        int score_max = int.MinValue;
        int maskdata1 = changetops(board_bit)[turn - 1];
        while ( maskdata1 != 0){			
            int m = Findminbit(maskdata1);
            int maskdata2 = canMovebit(~(board_bit[2] | board_bit[5]),m,turn);
			while( maskdata2 != 0){
				int f = Findminbit(maskdata2);
				int [] newboard_bit = movebit(m,f,board_bit,turn);
        		int score = -1*negaalphabit(-beta,-alpha,limit - 1,newboard_bit,3-turn);//次の相手の手 3-turnは相手 1->2 2->1
  				//ここを削ればnegamaxになる
      			if (score >= beta) {// beta値を上回ったら探索を中止
      				return score;
    			}
				if(score>score_max){//より良い手が見つかった
			    	score_max = score;	  
					alpha = Math.Max(alpha, score_max);//α値を更新
					if (findinglimit == limit){
						from_x = m/6+1;from_z = m%6+1;to_x = (m/6+1)+(f/3-1);to_z = (m%6+1)+(f%3-1);//動かす手を更新
						this.score = score;//スコアを更新
					}
				}
				maskdata2 -= (1 << f);
			}
            maskdata1 -= (1 << m);
        }    
		return score_max;
	}

	public int[] downsizeab(int[,] board_2d){//送られた盤面を変換
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

	public int[] changetops(int[] board_6){//盤面のそれぞれ黒が打てる場所、白が打てる場所を表す
		int[] board_2 = new int[2];
		//board_2[0] = board_6[2]|(board_6[1] & ~board_6[5])|(board_6[0] & ~board_6[4] & ~board_6[5]);//変形
		board_2[0] = board_6[2]| ~board_6[5]&(board_6[1]|board_6[0] & ~board_6[4]);
		//board_2[1] =  board_6[5]|(board_6[4] & ~board_6[2])|(board_6[3] & ~board_6[1] & ~board_6[2]);//変形
		board_2[1] = board_6[5]| ~board_6[2]&(board_6[4]|board_6[3] & ~board_6[1]);
		return board_2; 
	}

	public int bitcount(int x){//ビット上で1の数を数える
 	    x = x - ((x >> 1) & 0x55555555);
    	x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
    	x = (x + (x >> 4)) & 0x0f0f0f0f;
    	x = x + (x >> 8);
    	x = x + (x >> 16);
    	return x & 0x003f;
	}

	bool DecideJudgebit(int[] board_bit,int turn){//ゲームが終わったか判定
	    //bool outzone = row!=bitcount(board_bit[3*(turn-1)])+bitcount(board_bit[3*(turn-1)+1])+bitcount(board_bit[3*(turn-1)+2]);//ゴールに到達し、駒の数が減っていないか
    	//bool pressed = changetops(board_bit)[2-turn] == 0;//動けるコマが無くなっていないか
		return (row!=bitcount(board_bit[3*(turn-1)])+bitcount(board_bit[3*(turn-1)+1])+bitcount(board_bit[3*(turn-1)+2])) || (changetops(board_bit)[2-turn] == 0);
	}

 	int evaluatenegabit(int[] board_bittop,int turn){//盤面のスコアを求める　初めから入力をトップにする
		// 黒は　　1 　2 　4  8 16 32 
		// 白は　-32 -16 -6 -4 -2 -1 
		//int x0 =  0b00111111000000111111000000000000//1057222656
		//int y0 =  0b00000000111111000000111111111111;//16519167
        board_bittop[0] = ((board_bittop[0] & 1057222656) >> 6) +(board_bittop[0] & 16519167);//1列目、2と3列目の合計, 4と5列目の合計
		// aaaaaa       aaaaaa 
		// bbbbbb       (b+cの	  
		// cccccc   -> 	  合計)
		// dddddd       (d+eの 
		// eeeeee         合計)
        board_bittop[0] = (board_bittop[0] & 0b111111) + (board_bittop[0] >> 6);//1列目と2列目の合計
		// aaaaaa 		(a+b+c
		// (b+cの	  	の合計)
		//   合計)   -> (d+eの
		// (d+eの 		  合計)
		//   合計)      000000
		board_bittop[0] += (board_bittop[0] >> 12);//合計
		// (a+b+c			(a+b+c+d+e
		// 	の合計)			の合計)
		// (d+eの      -> 	(d+eの
		//    合計)			合計)
		//  000000			000000
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

    int canMovebit(int board_basebit,int point,int turn){//動かせる方向を9ビットで示す　盤面全体における動かせる場所を示す~(board_bit[2] | board_bit[5])を入力	
        //int bas = 0b111000101000111;//28999
        //int mask1 = 0b000100000100000100;//16644
        //int mask2 = 0b000001000001000001;//4161
        //int mask3 = 0b000111000000000000;//28672
        if (point <= 7) {board_basebit <<= 7 - point;}else {board_basebit >>= point - 7;}//求める場所が来るように調整
        board_basebit &= 28999;
        if ((point % line) == 0 && turn == Black) {board_basebit &= ~4161;}//左端から左へいかないように
		if ((point % line) == line - 1 && turn == White) {board_basebit &= ~16644;}//右端から右へ行かないように
	 	if ((point % line) == line-1 && turn == Black) {board_basebit |= 16644;}//右端から右へいけるように
		if ((point % line) == 0 && turn == White) {board_basebit |= 4161;}//左端から左へ行けるように
        if(point <= 5){board_basebit &= ~(0b111);}//上端から上に行かないように
		 if (point >= 24) {board_basebit &= ~28672;}//下端から下に行かないように
        board_basebit |= (board_basebit >>3);//値の合成 1段目と2段目の左3つが順番通りになるように
        //xxx000     xxxyyy  
        //yyy000  -> yyyzzz   
        //zzz000     zzz000		
        board_basebit = (board_basebit & 0b111111) | ((board_basebit >>3)&0b111000000) ;//値の合成2　1段目6つとずらした2段目右3つが順番通りになるように
        //xxxyyy     xxxyyy  
        //yyyzzz  -> zzz000   
        //zzz000     000000	
        return board_basebit;
    }

	int[] movebit(int point,int direction,int[] board_bit,int turn){//駒を動かす
	    //if ((changetops(board_bit)[turn -1]>>point & 1) != 1) { Console.WriteLine("Error: NO PIECE {0}",turn); }//求める駒がない　減らない
        //if ((((~(board_bit[2] | board_bit[5])) >> (point + direction)) & 1) == 0) { Console.WriteLine("Error: CAN'T PLACE");}//満杯　増やせない
		// 0 1 2 3 4 5  pointの番号と盤面の対応表
    	// 6 7 8 91011
	    direction += 3*(direction / 3) - 7;//方向を調整
        //0 1 2     -7 -6 -5   
        //3 4 5  -> -1  0  1   
        //6 7 8      5  6  7     
		int[] board_bitcop = new int[6];
		Array.Copy(board_bit, board_bitcop, board_bit.Length);	 

        //駒を減らす　0にすることで1を0にする 
        board_bitcop[3*(turn-1)] &= (board_bit[3*(turn-1)+1] |board_bit[3*(turn-1)+2]|~(1<<point));//一段目
        // 110 -> 1 101 -> 1 100 -> 0 111-> 1　となるような値を取る
        board_bitcop[3*(turn-1)+1] &= (board_bit[3*(turn-1)+2]|~(1<<point));//二段目
        // 10 -> 0 11 -> 1　となるような値を取る
        board_bitcop[3*(turn-1)+2] &= ~(1<<point);//三段目
        // 必ず0

		//駒を増やす　1にすることで0を1にする 
		 int add=1 << point + direction;
        if((point %6 == 0 &&  (point + direction +6) % 6 == 5 || point %6 == 5 && ( point + direction+6) % 6 == 0 )){
        	add = 0;//右端より右や左端より左に駒を動かす場合、前や次の行に行くことを防ぐ
		//(point,direction) = (0,-1)のような場合に備えて+6されている さもなくばシフトが-1になってややこしいことになる
        }
        board_bitcop[3*(turn-1)+2] |= ((board_bit[1] | board_bit[4]) & ~(board_bit[2] | board_bit[5]) & add);
        //2段目が埋まっており、3段目が埋まっていない
        board_bitcop[3*(turn-1)+1] |=((board_bit[0] | board_bit[3])& ~(board_bit[1] | board_bit[4]) & add);
        //1段目が埋まっており、2段目が埋まっていない
        board_bitcop[3*(turn-1)] |= ~(board_bit[0] |board_bit[3]) & add;
        //1段目が埋まっていない
        return board_bitcop;
    }

	int Findminbit(int x){//1である最下位ビットを求める 例　00101 -> 0 10100 -> 2 
    	int n=0;
     	if ((x & 0xffff) == 0) { n += 16;x >>= 16; }
        if ((x & 0x00ff) == 0) { n += 8; x >>= 8; }
        if ((x & 0x000f) == 0) { n += 4; x >>= 4; }
        if ((x & 0x0003) == 0) { n += 2; x >>= 2; }
        if ((x & 0x0001) == 0) { n += 1;}
        return n;
    }
}

