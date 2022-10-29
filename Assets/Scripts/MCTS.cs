using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTS : MonoBehaviour {
    
    public static MCTS instance;
    public MonteAI mAI;
    public int from_x;
    public int from_z;
    public int to_x;
    public int to_z;
    void Update() {
        uint[] board_info = downsize(GameMainScript.instance.board_state);
        mAI = new MonteAI(board_info, (uint)GameMainScript.instance.Turn);
        from_x = mAI.from_row;
        from_z = mAI.from_col;
        to_x = mAI.to_row;
        to_z = mAI.to_col;
    }
    
    public uint[] downsize(int[,] board){
        uint[] res=new uint[GameMainScript.instance.row+2];
        res[0]=0xffffffff;
        res[GameMainScript.instance.row+1]=0xffffffff;
        for(int i=1; i<=GameMainScript.instance.row; i++){
            for(int j=0; j<GameMainScript.instance.line+2; j++){
                res[i]|=(uint)board[i,j]<<(4*j);
            }
        }
        return res;
    }
}

class NoccaGameSystem{
    public static uint Turn;
    public static uint Black=1;
    public static uint White=2;
    public static int row=5;
    public static int col=6;
    public static uint[] board=new uint[row+2];
    public static List<uint[]> former_boards=new List<uint[]>();

    public static NoccaGameSystem instance;

    public static void Main(string[] args){
        Turn=Black;
        // 壁
        board[0]=0xffffffff;
        board[row+1]=0xffffffff;
        // 駒の初期配置
        for(int r=1; r<=row; r++){
            board[r]=Black<<4|White<<(col*4);
        }

        former_boards.Add(board.Clone() as uint[]);
        Console.WriteLine("初期状態");
        printBoard(board.Clone() as uint[]);

        // // 以下プレイの処理
        uint win=isEnd();
        while(win==0){
            Console.WriteLine("count: "+former_boards.Count);
            // 手を決める
            int from_row,from_col,to_row,to_col;
            if(Turn==Black){
                RandomAI randAI=new RandomAI(board.Clone() as uint[],Turn);
                from_row=randAI.from_row;
                from_col=randAI.from_col;
                to_row=randAI.to_row;
                to_col=randAI.to_col;
            }else{
                MonteAI monte=new MonteAI(board.Clone() as uint[],Turn);
                from_row=monte.from_row;
                from_col=monte.from_col;
                to_row=monte.to_row;
                to_col=monte.to_col;
            }

            Console.WriteLine(Turn+": ("+from_row+", "+from_col+") -> ("+to_row+", "+to_col+")");
            if(!move(from_row,from_col,to_row,to_col)){
                return;
            }
            printBoard(board.Clone() as uint[]);
            if(Turn==White){
                Turn=Black;
            }else if(Turn==Black){
                Turn=White;
            }
            win=isEnd();
        }
        Console.WriteLine(win==3? "draw":win+" win");
    }

    public static uint getTop(uint borad_val){
        if(borad_val<=0 || borad_val>=15){
            return 0;
        }else if(borad_val==1 || borad_val==3 || borad_val==4 || (7<=borad_val && borad_val<=10)){
            return Black;
        }else{
            return White;
        }
    }

    public static bool isLegalDst(int from_row,int from_col,int to_row,int to_col){
        if(from_row==to_row && from_col==to_col){
            return false;
        }
        if((to_col==0 && Turn==Black) || (to_col==col+1 && Turn==White)){
            return false;
        }
        if(Math.Abs(from_row-to_row)>1 && Math.Abs(from_col-to_col)>1){
            return false;
        }

        uint to_val=board[to_row]>>(to_col*4)&0xf;
        if(to_val>6){
            return false;
        }

        return true;
    }

    // 試行用
    public static bool isLegalDst(uint[] board,uint Turn,int from_row,int from_col,int to_row,int to_col){
        if(from_row==to_row && from_col==to_col){
            return false;
        }
        if((to_col==0 && Turn==Black) || (to_col==col+1 && Turn==White)){
            return false;
        }
        if(Math.Abs(from_row-to_row)>1 && Math.Abs(from_col-to_col)>1){
            return false;
        }

        uint to_val=board[to_row]>>(to_col*4)&0xf;
        if(to_val>6){
            return false;
        }

        return true;
    }

    public static bool move(int from_row,int from_col,int to_row,int to_col){
        uint from_val=board[from_row]>>(from_col*4)&0xf;
        if(getTop(from_val)!=Turn){
            Console.WriteLine("駒の選択ミス");
            return false;
        }
        if(!isLegalDst(from_row,from_col,to_row,to_col)){
            Console.WriteLine("移動先の選択ミス");
            return false;
        }

        // 以下合法、移動元の処理
        if(from_val<=2){
            board[from_row]^=(from_val<<(from_col*4));
        }else if(from_val<=6){
            board[from_row]-=(2*Turn << (from_col*4));
        }else{
            board[from_row]-=(4*Turn << (from_col*4));
        }

        // 移動先の処理
        uint to_val=board[to_row]>>(to_col*4)&0xf;
        if(to_val==0){
            board[to_row]|=(Turn << (to_col*4));
        }else if(to_val<=2){
            board[to_row]+=(Turn*2 << (to_col*4));
        }else{
            board[to_row]+=(Turn*4 << (to_col*4));
        }
        former_boards.Add(board.Clone() as uint[]);
        return true;
    }

    public static uint isEnd(){
        // ドロー
        for(int i=0; i<former_boards.Count-1; i++){
            if(former_boards[i].SequenceEqual(board)){
                Console.WriteLine(i+"手目と同じ盤面");
                return 3;
            }
        }

        // ゴールしたか
        uint b=Black<<((col+1)*4);
        for(int r=1; r<=row; r++){
            if((board[r]&White)!=0){
                return White;
            }else if((board[r]&b)!=0){
                return Black;
            }
        }

        // 上に乗ったか
        bool blackTop=true,whiteTop=true;
        for(int r=1; r<=row; r++){
            uint board_r=board[r];
            for(int c=1; c<=col; c++){
                uint top=getTop((board_r>>(c*4))&0xf);
                // Console.WriteLine("("+r+", "+c+") top: "+top);
                if(top==White){
                    blackTop=false;
                }else if(top==Black){
                    whiteTop=false;
                }
                if(!blackTop && !whiteTop){
                    return 0;
                }
            }
        }
        if(blackTop){
            return Black;
        }else if(whiteTop){
            return White;
        }else{
            return 0;
        }
    }

    // 試行用、ドローは判定しない
    public static uint isEnd(uint[] board){
        // ゴールしたか
        uint b=Black<<((col+1)*4);
        for(int r=1; r<=row; r++){
            if((board[r]&White)!=0){
                return White;
            }else if((board[r]&b)!=0){
                return Black;
            }
        }

        // 上に乗ったか
        bool blackTop=true,whiteTop=true;
        for(int r=1; r<=row; r++){
            uint board_r=board[r];
            for(int c=1; c<=col; c++){
                uint top=getTop((board_r>>(c*4))&0xf);
                // Console.WriteLine("("+r+", "+c+") top: "+top);
                if(top==White){
                    blackTop=false;
                }else if(top==Black){
                    whiteTop=false;
                }
                if(!blackTop && !whiteTop){
                    return 0;
                }
            }
        }
        if(blackTop){
            return Black;
        }else if(whiteTop){
            return White;
        }else{
            return 0;
        }
    }

    public static void printBoard(uint[] board){
        for(int c=col+1; c>=0; c--){
            for(int r=1; r<=row; r++){
                string txt=string.Format(" {0,2}",board[r]>>(c*4)&0xf);
                Console.Write(txt);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}

class RandomAI{
    private System.Random rnd=new System.Random();
    private uint Turn;
    private uint[] board;
    public int from_row,from_col,to_row,to_col;

    public RandomAI(uint[] board,uint Turn){
        this.Turn=Turn;
        this.board=board;
        List<int[]> srcList=getSrc();
        int[] src=srcList[rnd.Next(srcList.Count)];
        List<int[]> dstList=getDst(src[0],src[1]);
        int[] dst=dstList[rnd.Next(dstList.Count)];
        from_row=src[0];
        from_col=src[1];
        to_row=dst[0];
        to_col=dst[1];
    }

    private List<int[]> getSrc(){
        List<int[]> src=new List<int[]>();
        for(int r=1; r<=NoccaGameSystem.row; r++){
            uint board_r=board[r];
            for(int c=1; c<=NoccaGameSystem.col; c++){
                if(NoccaGameSystem.getTop(board_r>>(c*4)&0xf)==Turn){
                    src.Add(new int[]{r,c});
                    // Console.WriteLine("src: ("+r+", "+c+")");
                }
            }
        }
        return src;
    }

    private List<int[]> getDst(int r,int c){
        List<int[]> dst=new List<int[]>();
        for(int i=-1; i<=1; i++){
            for(int j=-1; j<=1; j++){
                if(NoccaGameSystem.isLegalDst(board,Turn,r,c,r+i,c+j)){
                    dst.Add(new int[]{r+i,c+j});
                    // Console.WriteLine("dst: ("+r+", "+c+")");
                }
            }
        }
        return dst;
    }
}

class GameTreeNode{
    public uint Turn;
    public uint[] board;
    public int move; //現在の局面からの合法手(from_row,from_col,to_row,to_col)
    public int simCount; //このノードでのシミュレーション回数
    public int win;

    public GameTreeNode(uint Turn,uint[] board,int move,int simCount,int win){
        this.Turn=Turn;
        this.board=board;
        this.move=move;
        this.simCount=simCount;
        this.win=win;
    }
}

public class MonteAI{
    public int from_row,from_col,to_row,to_col;
    private uint Black=NoccaGameSystem.Black;
    private uint White=NoccaGameSystem.White;
    private int allSimCount=0;
    Dictionary<int,int[]> dict=new Dictionary<int,int[]>(); //moveをキーにrootからの手の試行回数と勝ち数{simCount,win}を保存(backup)

    // ハイパーパラメタ
    private int step=1000; //step数
    private int simTimes=20; //1stepの試行回数
    private int threshold=10; //展開時の閾値
    private double c_squared=2.0f; //uctの定数c^2

    // 展開したゲーム木の末端ノードのリスト
    private List<GameTreeNode> nodeList=new List<GameTreeNode>();

    public MonteAI(uint[] board,uint Turn){
        // nodeListに着手候補を追加
        foreach(int[] item in getMove(board,Turn)){
            int _move=(item[0]<<12)|(item[1]<<8)|(item[2]<<4)|item[3];
            uint[] newBoard=move(board.Clone() as uint[],Turn,item[0],item[1],item[2],item[3]).Clone() as uint[];
            nodeList.Add(new GameTreeNode(changeTurn(Turn),newBoard,_move,0,0));
            dict.Add(_move,new int[2]);
        }
        // uctを元に木を探索、拡張
        search(step,Turn);
        // test();

        double rate=0.0;
        int k=0;
        foreach(int key in dict.Keys){
            double _rate=(double)dict[key][1]/(double)dict[key][0];
            // test
            // Console.Write("("+((key>>12)&0xf)+","+((key>>8)&0xf)+") -> ");
            // Console.Write("("+((key>>4)&0xf)+","+(key&0xf)+")");
            // Console.WriteLine(": "+dict[key][1]+"/"+dict[key][0]);
            if(_rate>rate){
                rate=_rate;
                from_row=(key>>12)&0xf;
                from_col=(key>>8)&0xf;
                to_row=(key>>4)&0xf;
                to_col=key&0xf;
                k=key;
            }
        }
        Console.WriteLine("rate: "+dict[k][1]+"/"+dict[k][0]);
        // Console.WriteLine("max rate: "+rate);
    }

    private double upperConfidenceBound(int simCount,int allSimCount,int win){
        double uct=(double)win/(double)simCount+Math.Sqrt(c_squared*Math.Log(allSimCount)/simCount);
        return uct;
    }

    private void search(int step,uint Turn){
        if(step==0){
            return;
        }else{
            select();
            expand(Turn);
            search(step-1,Turn);
        }
    }

    // 1stepのselection
    private void select(){
        int limit=simTimes;
        while(limit>0){
            double uct_selected=-999999;
            GameTreeNode selected=null;
            foreach(GameTreeNode node in nodeList){
                if(node.simCount==0){
                    // ランダムプレイ
                    if(node.Turn==rollout(node.board.Clone() as uint[],node.Turn)){
                        node.win++;
                    }
                    allSimCount++;
                    node.simCount++;
                    limit--;
                }else{
                    double uct=upperConfidenceBound(node.simCount,allSimCount,node.win);
                    // uctで選択するノードを更新
                    if(uct_selected<uct){
                        uct_selected=uct;
                        selected=node;
                    }
                }
            }
            // ランダムプレイ
            if(selected!=null){
                if(selected.Turn==rollout(selected.board.Clone() as uint[],selected.Turn)){
                    selected.win++;
                }
                allSimCount++;
                selected.simCount++;
                limit--;
            }else{
                // 新しいノードの試行だけで終わったとき、ここに来る
                // Console.WriteLine("uct unused");
            }
        }
    }

    // expnasion
    private void expand(uint Turn){
        //Turnは現在のターン
        // foreachの中で母集合を操作するとエラーになる: https://qiita.com/nori0__/items/58d97201b479c3556e39
        // ->tmpNodeに保存して後で結合する
        List<GameTreeNode> tmpNode=new List<GameTreeNode>();

        foreach(GameTreeNode node in nodeList){
            if(node.simCount>=threshold){
                foreach(int[] item in getMove(node.board,node.Turn)){
                    uint[] newBoard=move(node.board.Clone() as uint[],node.Turn,item[0],item[1],item[2],item[3]).Clone() as uint[];
                    tmpNode.Add(new GameTreeNode(changeTurn(node.Turn),newBoard,node.move,0,0));
                }
                allSimCount-=node.simCount;
            }else{
                // backup
                dict[node.move][0]+=node.simCount;
                if(node.Turn==Turn){
                    dict[node.move][1]+=node.win;
                }else{
                    dict[node.move][1]+=(node.simCount-node.win);
                }
            }
        }
        // 削除もforeachの後
        nodeList.RemoveAll(node=>node.simCount>=threshold);
        nodeList.AddRange(tmpNode);
    }

    private uint changeTurn(uint Turn){
        if(Turn==Black){
            return White;
        }else{
            return Black;
        }
    }

    private List<int[]> getMove(uint[] board,uint Turn){
        List<int[]> moveList=new List<int[]>();
        // ゲームが終了していたら空のリストを返す
        if(NoccaGameSystem.isEnd(board)!=0){
            return moveList;
        }

        for(int r=1; r<=NoccaGameSystem.row; r++){
            uint board_r=board[r];
            for(int c=1; c<=NoccaGameSystem.col; c++){
                if(NoccaGameSystem.getTop(board_r>>(c*4)&0xf)==Turn){
                    // 行先の取得
                    for(int i=-1; i<=1; i++){
                        for(int j=-1; j<=1; j++){
                            if(NoccaGameSystem.isLegalDst(board,Turn,r,c,r+i,c+j)){
                                moveList.Add(new int[]{r,c,r+i,c+j});
                            }
                        }
                    }
                }
            }
        }
        return moveList;
    }

    private uint[] move(uint[] board,uint Turn,int from_row,int from_col,int to_row,int to_col){
        // 移動元の処理
        uint from_val=board[from_row]>>(from_col*4)&0xf;
        if(from_val<=2){
            board[from_row]^=(from_val<<(from_col*4));
        }else if(from_val<=6){
            board[from_row]-=(2*Turn << (from_col*4));
        }else{
            board[from_row]-=(4*Turn << (from_col*4));
        }

        // 移動先の処理
        uint to_val=board[to_row]>>(to_col*4)&0xf;
        if(to_val==0){
            board[to_row]|=(Turn << (to_col*4));
        }else if(to_val<=2){
            board[to_row]+=(Turn*2 << (to_col*4));
        }else{
            board[to_row]+=(Turn*4 << (to_col*4));
        }
        return board;
    }

    // 勝敗を返す
    private uint rollout(uint[] board,uint Turn){
        uint end=NoccaGameSystem.isEnd(board);
        if(end!=0){
            return end;
        }else{
            RandomAI rndAI=new RandomAI(board.Clone() as uint[],Turn);
            uint[] newBoard=move(board,Turn,rndAI.from_row,rndAI.from_col,rndAI.to_row,rndAI.to_col).Clone() as uint[];
            return rollout(newBoard.Clone() as uint[],changeTurn(Turn));
        }
    }

    private void checkNodeList(){
        foreach(GameTreeNode node in nodeList){
            string str_from="("+((node.move>>12)&0xf)+","+((node.move>>8)&0xf)+")";
            string str_to=" -> ("+((node.move>>4)&0xf)+","+(node.move&0xf)+")";
            Console.WriteLine(str_from+str_to+": "+node.win+"/"+node.simCount);
        }
    }
}