//using UnityEngine;
using System;
using System.Collections;

public enum eLang
{
	NONE,
    EN,
	TW,
	CN,
    JP
}

//聲音列舉
public enum eAudio
{
	NONE,  //無
	MUSIC, //音樂
	SOUND  //音效
}

//CRC結構
public struct RCRC
{
	public string FileName;
	public uint CRC;
	
    public RCRC (string vFileName, uint vCRC)
	{
		FileName = vFileName;
		CRC = vCRC;
	}
}

//資訊結構
public struct RHTTPInfo
{
	public string Name;
	public string IP;
	public string Version;
	
    public RHTTPInfo (string vName, string vIP, string vVersion)
	{
		Name = vName;
		IP = vIP;
		Version = vVersion;
	}
}

//貼圖結構
//public struct RTex
//{
//	public string Name; //名稱
//	public Texture Tex; //貼圖
	
//    public RTex (string vName, Texture vTex)
//	{
//		Name = vName;
//		Tex = vTex;
//	}
//}

//遊戲設定結構
public struct RSetting
{
    public eLang Lang;
    public bool IsMusic;    //音樂開關
    public bool IsSound;    //音效開關
    public bool IsDice;
    public bool IsProduct;
    public bool IsActivity;

    public RSetting (eLang vLang, bool vValue)
    {
        Lang = vLang;
        IsMusic = vValue;
        IsSound = vValue;
        IsDice = vValue;
        IsProduct = vValue;
        IsActivity = vValue;
    }
}

public struct RCharacter
{
    public bool IsSelect;
    public ushort ChessID;
    public string ChessName;
    public byte SkillLevel;
    public string SkillName;
    public string SkillInfo;

    public RCharacter (bool vIsSelect, ushort vChessID, string vChessName, byte vSkillLevel, string vSkillName, string vSkillInfo)
    {
        IsSelect = vIsSelect;
        ChessID = vChessID;
        ChessName = vChessName;
        SkillLevel = vSkillLevel;
        SkillName = vSkillName;
        SkillInfo = vSkillInfo;
    }
}

public struct RUseInfo
{
    public uint PlayerID;
    public string NickName;

    public RUseInfo (uint vPlayerID, string vNickName)
    {
        PlayerID = vPlayerID;
        NickName = vNickName;
    }
}

public class StructureAttribute : Attribute
{
    public int TotalSize;

    public StructureAttribute (int vTotalSize)
    {
        TotalSize = vTotalSize;
    }
}

public class FieldAttribute : Attribute
{
    public int Size;
    public int Length;

    public FieldAttribute (int vSize, int vLength = 1)
    {
        Size = vSize;
        Length = vLength;
    }
}

//通用常數
public static class Const_Common : object
{
    public const string VERSION = "1.0";

	//密鑰
	public const int KEY = 0;

    public const string WebPath1 = "http://partyweb.jway.me/";
    public const string WebPath2 = "http://partyinhouse.jway.me/";

	//語系路徑
    public const string LANG_EN = "EN/";
	public const string LANG_TW = "TW/";
    public const string LANG_CN = "CN/";
    public const string LANG_JP = "JP/";

	//檔案名稱常數
	public const string FILE_VERSION = "Version.res";
	public const string FILE_HTTP = "HTTP.dat";
	public const string FILE_SERVER = "ServerIP.txt";
	public const string FILE_ACCOUNT = "Account.txt";
    public const string FILE_SETTING = "Setting.txt";

	//檔案路徑常數
	public const string Path_Assets = "Assets/";
	public const string Path_UI = "UI/";
	public const string Path_UploadAssets = "UploadAssets/";
	public const string Path_DownloadAssets = "DownloadAssets/";
	public const string Path_StreamingAssets = "StreamingAssets/";
	public const string Path_Log = "Log/";
	public const string Path_Versions = "Versions/";

	public const string Path_Games = "Games/";
    public const string Path_Common = "Games/Common/";
    public const string Path_Create = "Games/Create/";
	public const string Path_AmusementPark = "Games/AmusementPark/";
	public const string Path_Monopoly = "Games/Monopoly/";
	public const string Path_GuessFood = "Games/GuessFood/";
    public const string Path_LightMap = "Games/LightMap/";
	public const string Path_TestGame = "Games/TestGame/";
    public const string Path_Bigmap = "Games/Bigmap/";
	public const string Path_Lobby = "Lobby/";

	public const string Path_Audios = "Audios/";
	public const string Path_Datas = "Datas/";
	public const string Path_Mats = "Mats/";
	public const string Path_Models = "Models/";
	public const string Path_Models_Items = "Models/Items/";
	public const string Path_Models_Scenes = "Models/Scenes/";
	public const string Path_Models_Cameras = "Models/Cameras/";
    public const string Path_Models_Chesses = "Models/Chesses/";
	public const string Path_Particles = "Particles/";
	public const string Path_UIs = "UIs/";
	public const string Path_UIs_Items = "UIs/Items/";
	public const string Path_UIs_Panels = "UIs/Panels/";
	public const string Path_Words = "Words/";
    public const string Path_ScreenShot = "ScreenShot/";
    public const string Path_Save  = "Save/";
}