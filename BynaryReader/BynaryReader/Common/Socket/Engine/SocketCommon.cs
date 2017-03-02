using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using CGEngine.Memory;

namespace CGEngine.SocketTool 
{
	//delegate   
	public delegate void TNetMessageProc (ByteArrayBuffer msg); //執行 協定
	public delegate void NetEvent (object sender, EventArgs e); //事件 Socket

	//EventArgs
	public class ConnectEventArgs : EventArgs
	{

	}

	public class DisConnectEventArgs : EventArgs
	{
		
	}

	public class ReceiveEventArgs : EventArgs
	{
		
	}

	public class SendMsgEventArgs : EventArgs
	{
		
	}

	public class ErrorEventArgs : EventArgs
	{
		
	}

	public class ProtocolAnalyzerEventArgs : EventArgs
	{
		public byte mainno;
		public byte subno;
	}

    /// <summary>
    /// 封包檔頭
    /// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PACKAGEHEAD
    {
        public ushort Version;    //版本號
        public uint DataLen;      //封包長度
        public byte MainNo;       //協定編號1    
        public byte SubNo;        //協定編號2 
        [MarshalAs(UnmanagedType.U1)]
        public bool PackCompress; //是否壓縮
        public uint PlayerID;     //
		public uint SessionID;    //SessionID
		public byte SerialID;     //協定序號
    }

    /// <summary>
    /// 接收封包的結構
    /// </summary>
    class SocketPacket
    {
        public Socket workSocket;
        public byte[] dataBuffer = new byte[1024];  //定太小, 封包會接不完整
    }

    /// <summary>
    /// 封包池
    /// </summary>
    class PacketPool : CacheBufPool<SocketPacket>
    {
        public PacketPool() : base(1024)
        {
        }
    }
}