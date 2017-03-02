//using UnityEngine;
using System.Collections;
using CGEngine.Memory;

//遊戲介面
public interface IGame
{
	void Rcv_AfterGameStart (ByteArrayBuffer vMsg);
	void Rcv_AfterGameOver (ByteArrayBuffer vMsg);
	void Rcv_AfterGamePause (ByteArrayBuffer vMsg);
	void Rcv_AfterGameContinue (ByteArrayBuffer vMsg);
	void Rcv_AfterGameBalance (ByteArrayBuffer vMsg);
	void Rcv_AfterRoundStart (ByteArrayBuffer vMsg);
	void Rcv_AfterRoundOver (ByteArrayBuffer vMsg);
	void Rcv_GameReady (ByteArrayBuffer vMsg);
	void Rcv_GameStart (ByteArrayBuffer vMsg);
	void Rcv_GameOver (ByteArrayBuffer vMsg);
	void Rcv_GamePause (ByteArrayBuffer vMsg);
	void Rcv_GameContinue (ByteArrayBuffer vMsg);
	void Rcv_GameBalance (ByteArrayBuffer vMsg);
	void Rcv_RoundStart (ByteArrayBuffer vMsg);
	void Rcv_RoundOver (ByteArrayBuffer vMsg);
}

//遊戲種類列舉
public enum eGame
{
	NONE = 0,
    LOBBY,
	MONOPOLY,
    GUESSFOOD,
    GOLF,
    WATERFRUIT,
    MEMORYCYCLE,
    BALLOON,
	TESTGAME
}

//競賽類型列舉
public enum eRace
{
	NONE = 0,
	ONESELF,
	TWOANDTWO,
	ONEANDTHREE
}

//遊戲狀態
public enum eGameStatus
{
    NONE = 0,
	WAIT,
	READY,
	RUN,
	BALANCE,
	OVER
}

//玩家狀態
public enum ePlayerStatus
{
	NONE = 0,
	WAIT,
	LOCK,
	READYING,
	READY,
	RUN,
	BALANCE
}

//遊戲定義結構
[StructureAttribute(10)]
public struct RGameDefine
{
    [FieldAttribute(1)]
	public byte GameType;  //遊戲種類
    [FieldAttribute(1)]
    public byte RaceType;  //競賽種類
    [FieldAttribute(4)]
	public int Round;      //回合數
    [FieldAttribute(4)]
	public int MaxCount;   //遊戲人數
}

[StructureAttribute(10)]
public struct RWaitTableInfo
{
    [FieldAttribute(4)]
    public uint TableID;          //房間編號
    [FieldAttribute(8)]
    public double Deadline;
    [FieldAttribute(10)]
	public RGameDefine Define;    //遊戲定義資料
    [FieldAttribute(41, 4)]
	public RWaitPlayer[] Players; //玩家資料
}

//等待配桌玩家結構
[StructureAttribute(41)]
public struct RWaitPlayer
{
    [FieldAttribute(1)]
    public bool IsLeader;       //室長標記
    [FieldAttribute(2)]
    public ushort ChessID;      //棋子編號
    [FieldAttribute(4)]
    public uint PlayerID;       //玩家編號
    [FieldAttribute(16 * 2)]
    public WideChar16 NickName; //玩家名稱
    [FieldAttribute(2)]
    public ushort LV;           //玩家等級
}

//桌次資訊結構
[StructureAttribute(9)]
public struct RTableInfo
{
    [FieldAttribute(4)]
	public int GameType;
    [FieldAttribute(1)]
	public byte RaceType;
    [FieldAttribute(2)]
	public ushort TableID;
    [FieldAttribute(1)]
	public byte Status;
    [FieldAttribute(1)]
	public byte Count;
	
	public RTableInfo (int vGameType, byte vRaceType, ushort vTableID, byte vStatus, byte vCount)
	{
		GameType = vGameType;
		RaceType = vRaceType;
		TableID = vTableID;
		Status = vStatus;
		Count = vCount;
	}
}

//玩家結算結構
[StructureAttribute(9)]
public struct RBalancePlayer
{
    [FieldAttribute(4)]
    public uint PlayerID; //玩家編號
    [FieldAttribute(1)]
    public byte Rank;     //排名
    [FieldAttribute(4)]
    public int Score;     //分數
}

//public static class Const_Game : object
//{
//	public const int MAX_PLAYER_COUNT = 4;

//	public static eUI GetUI(eGame vGameType)
//	{
//		switch (vGameType)
//		{
//		    case eGame.MONOPOLY: return eUI.Monopoly;
//		    case eGame.GUESSFOOD: return eUI.GuessFood;
//		    case eGame.TESTGAME: return eUI.TestGame;
//		}

//		return eUI.None;
//	}
//}
