using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using CGEngine.Memory;

namespace CGEngine.SocketTool 
{
	public class SockectClient : IDisposable
	{
        private PacketPool _PacketPool; //封包池

        private object m_synRec;  //同步对象
        private object m_synSend; //同步对象     
   
        private RingMemoryStream m_RecBuf;  //存放接收封包資訊
        private RingMemoryStream m_SendBuf; //存放發送封包資訊    

        private IAsyncResult m_rs;

        //解析封包頭使用
        private PACKAGEHEAD m_MsgHead; //head 資訊
        private int m_MsgHeadSize;     //head 大小                
        private byte[] m_TmpHeadBuf;   //對應  head 的 byte[] 暫存
        private ByteArrayBuffer m_Buf; //暫存的封包buf

        private uint m_PlayerID;
		private uint m_SessionID;

		private ushort m_Version;                 //協定版號
		private ushort m_PackCompressSize;        //封包壓縮判斷 0表不使用
		private TNetMessageProc[,] m_NetMsgProcs; //對應協定號要執行的命令		
		
		//debug        
        private Action<string> DoLog;
                
        private Socket m_tcpSsocket; //socket 元件
        private IPEndPoint m_ipEnd;  //IP 資訊
		
		public bool Connected {get{return (m_tcpSsocket != null) ? m_tcpSsocket.Connected : false;}}
        public uint PlayerID {get{return m_PlayerID;} set{m_PlayerID = value;}}
		public uint SessionID {get{return m_SessionID;} set{m_SessionID = value;}}

        // Events
        protected NetEvent _OnReceive;          //接收協定資料
        protected NetEvent _OnSendMsg;          //發送協定資料
        protected NetEvent _OnConnect;          //進行連線        
        protected NetEvent _OnDisConnect;       //進行斷線
        protected NetEvent _OnError;            //發生錯誤
		protected NetEvent _OnProtocolAnalyzer; //進行協定分析
		
		//發生錯誤
        public event NetEvent OnError {add{_OnError += value;} remove{_OnError -= value;}}
		
		//接收協定資料
        public event NetEvent OnReceive {add{_OnReceive += value;} remove{_OnReceive -= value;}}
		
		//發送協定資料
        public event NetEvent OnSendMsg {add{_OnSendMsg += value;} remove{_OnSendMsg -= value;}}
		
		//進行連線      
        public event NetEvent OnConnect {add{_OnConnect += value;} remove{_OnConnect -= value;}}
		
		//進行斷線
        public event NetEvent OnDisConnect {add{_OnDisConnect += value;} remove{_OnDisConnect -= value;}}
		
		//進行協定分析
        public event NetEvent OnProtocolAnalyzer {add{_OnProtocolAnalyzer += value;} remove{_OnProtocolAnalyzer -= value;}}

		//建構子
        public SockectClient (ushort version, ushort packcompresssize, Action<string> logevent)
        {
            try
            {
				//建立封包池
				_PacketPool = new PacketPool();

				m_synRec = new object();
				m_synSend = new object();

				//建立Buffer
				m_RecBuf = new RingMemoryStream();
				m_SendBuf = new RingMemoryStream();

				//建立封包相關
				m_MsgHead = new PACKAGEHEAD();

                m_MsgHeadSize = Marshal.SizeOf(m_MsgHead);
				m_TmpHeadBuf = new byte[m_MsgHeadSize];
				m_Buf = new ByteArrayBuffer();

                m_PlayerID = 0;
				m_SessionID = 0;

                //版本號
                m_Version = version;
                m_PackCompressSize = packcompresssize;

				//對應協定號要執行的命令
				m_NetMsgProcs = new TNetMessageProc[256, 256];

				DoLog += logevent;
            }
            catch (Exception se)
            {
				//UnityEngine.Debug.Log(se.Message);
            }	
        }

		public void Dispose ()
        {
			DoDisConnect();

			_PacketPool = null;
			m_synRec = null;
			m_synSend = null;
			m_RecBuf = null;
			m_SendBuf = null;
			m_TmpHeadBuf = null;
			m_Buf = null;
			m_NetMsgProcs = null;
			DoLog = null;
			m_tcpSsocket = null;
			m_ipEnd = null;
		}

		//建立連線
        public void Connect (string ipstring, int port)
        {
            //log
            AddLog("start connect ip [" + ipstring + "]");

            //safe code
            DoDisConnect();

            // Create the socket instance
            m_tcpSsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //m_tcpSsocket.ReceiveBufferSize = 10240;
            //m_tcpSsocket.ReceiveTimeout = 1000 * 30;

            // Create the end point 
            m_ipEnd = new IPEndPoint(IPAddress.Parse(ipstring), port);
            
            //異步連線
            m_rs = m_tcpSsocket.BeginConnect(m_ipEnd, new AsyncCallback(Callback_ConnectResult), null);
        }

		//連線結果
        private void Callback_ConnectResult (IAsyncResult asyn)
        {
            //連線成功才觸發
            if (m_tcpSsocket.Connected)
            {
                DoConnect();
            }
        }

		//連線
        private void DoConnect ()
        {
            //log
            AddLog("Connect OK!");

            //Wait for data asynchronously 
            WaitForData();

            if (_OnConnect != null)
              _OnConnect(this, new ConnectEventArgs());
        }

		//斷線
        private void DoDisConnect ()
        {
            if (m_tcpSsocket == null)
                return;

            if (!m_tcpSsocket.Connected)
                return;

            m_rs = null;

            //log
            AddLog("Disconnect!");
			
			try
            {
            	m_tcpSsocket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception e)
            {
				AddLog(e.ToString());
			}
			
            m_tcpSsocket.Close();

            if (_OnDisConnect != null)
                _OnDisConnect(this, new DisConnectEventArgs());
        }

		//分析封包資料
        private void DoReceive (ref byte[] msg, int count)
        {
            lock (m_synRec)
            {
                //接收協定字串內容
                if (count != 0)
                {
                    m_RecBuf.Write(msg, 0, count);
					AddLog(string.Format("write {0} Byte to m_RecBuf", count));
                }
            }
        }

        public void Update ()
        {
            Update_Connect();

            Update_Receive();
        }

        private void Update_Connect ()
        {
            if (m_rs != null)
            {
                if (!m_rs.AsyncWaitHandle.WaitOne(6000, false))
                {
                    //log
                    AddLog("connect timeout fail");

                    //連線超時  
                    DoDisConnect();

                    throw new TimeoutException();
                }
            }
        }

		//更新接收封包資料
        private void Update_Receive ()
        {
            lock (m_synRec)
            {
				long vIntoTick = DateTime.Now.Ticks;

                try
                {
					//長度大於協定頭大小,才能進行封包解析
                    while (m_RecBuf.Length >= m_MsgHeadSize)    
                    {
						//取出封包頭                        
                        m_RecBuf.ReadData(m_TmpHeadBuf, 0, m_MsgHeadSize, false);

						//清空內容
						m_Buf.Clear();

						//寫入資料
						m_Buf.WriteByteS(m_TmpHeadBuf);

						//清除內容
						m_MsgHead = new PACKAGEHEAD();

						//寫入檔頭封包資料
						m_MsgHead.Version = m_Buf.ReadUShort();
						m_MsgHead.DataLen = m_Buf.ReadUInt();
						m_MsgHead.MainNo = m_Buf.ReadByte();
						m_MsgHead.SubNo = m_Buf.ReadByte();
						m_MsgHead.PackCompress = m_Buf.ReadBool();
                        m_MsgHead.PlayerID = m_Buf.ReadUInt();
                        m_MsgHead.SessionID = m_Buf.ReadUInt();
						m_MsgHead.SerialID = m_Buf.ReadByte();

						AddLog(string.Format("DoReceive {0} {1} {2} {3}", m_MsgHead.Version, m_MsgHead.MainNo, m_MsgHead.SubNo, m_MsgHead.DataLen));

                        //檢查協定版本
						if (m_MsgHead.Version != m_Version)
                            throw new System.Exception("Version Error");

                        //協定長度不對
                        if (m_MsgHead.DataLen < m_MsgHeadSize)
                            throw new System.Exception("Length Error");

                        //協定是否接收完畢
                        if (m_MsgHead.DataLen > m_RecBuf.Length)
                            break;

                        //再讀取一次檔頭, 移動指針
                        m_RecBuf.ReadData(m_TmpHeadBuf, 0, m_MsgHeadSize, true);

                        byte[] _Command = new byte[m_MsgHead.DataLen - m_MsgHeadSize];
                        m_RecBuf.ReadData(_Command, 0, _Command.Length, true);
                        ByteArrayBuffer _buf = new ByteArrayBuffer();
                        _buf.Write(_Command);
						
						//是否有進行壓縮
                        if (m_MsgHead.PackCompress)
                            _buf.DecompressAndMark();

                        //執行命令
                        if (m_NetMsgProcs[m_MsgHead.MainNo, m_MsgHead.SubNo] != null)
                        {
                            try
                            {
                                m_NetMsgProcs[m_MsgHead.MainNo, m_MsgHead.SubNo](_buf);
                            }
                            catch (Exception e)
                            {
                                AddLog(string.Format("m_NetMsgProcs[{0},{1}] error -> {0}", m_MsgHead.MainNo, m_MsgHead.SubNo, e.Message));
                            }

                            //event
                            if (_OnProtocolAnalyzer != null) _OnProtocolAnalyzer(this, new ProtocolAnalyzerEventArgs() { mainno = m_MsgHead.MainNo, subno = m_MsgHead.SubNo });                             
                        }
                        else
                        {
                            //log
                            AddLog(string.Format("messgae {0} - {1} not create function", m_MsgHead.MainNo, m_MsgHead.SubNo));
                        }

						if ((DateTime.Now.Ticks - vIntoTick) > 20000000)
                        {
							AddLog("Update_Receive too long");
							break;
						}
                    }
                }
                catch (Exception e)
                {
                    if (m_RecBuf != null)
                        m_RecBuf.Clear();

                    AddLog(string.Format("Update_Receive Error -> {0}", e.Message));
                }

                //event
                if (_OnReceive != null)
                    _OnReceive(this, new ReceiveEventArgs());
            }
        }

		//等待封包資料
        public void WaitForData ()
        {
            try
            {                 
                SocketPacket _Pkt = _PacketPool.NewNode();
                _Pkt.workSocket = m_tcpSsocket;

                // Start listening to the data asynchronously
                m_tcpSsocket.BeginReceive(_Pkt.dataBuffer, 0, _Pkt.dataBuffer.Length, SocketFlags.None, new AsyncCallback(Callback_Received), _Pkt);
            }
            catch (SocketException se)
            {
				//UnityEngine.Debug.Log(se.Message);
            }
        }

		//接收封包
        public void Callback_Received (IAsyncResult asyn)
        {
            try
            {
                //接數數據, 並呼叫Socket 等待下一次接收   
                SocketPacket _Pkt = (SocketPacket)asyn.AsyncState;
                SocketError socketError = SocketError.TypeNotFound;
                int iRx = 0;

                if (_Pkt.workSocket.Connected)
                    iRx = _Pkt.workSocket.EndReceive(asyn, out socketError);
                else
                    return;

                if (iRx == 0)
                {
                    DoDisConnect();
                    return;
                }

                //msg
                AddLog(string.Format("Callback_Received() rcv {0} Bytes", iRx));
                
                //解析數據
                DoReceive(ref _Pkt.dataBuffer, iRx);

                //封包歸還
                _PacketPool.DisposeNode(_Pkt);

                //等待接收Next 封包, 必須是在最後, 封包才不會亂掉
                WaitForData();
            }
            catch (Exception e)
            {
                AddLog(e.ToString());
            }
        }

		//送出訊息
        public void SendMsg (byte mainno, byte subno, ByteArrayBuffer msg, byte serialId)
        {
            if (m_tcpSsocket == null)
                return;

            if (!m_tcpSsocket.Connected)
                return;

            lock (m_synSend)
            {
                try
                {
                    //log
					AddLog(string.Format("send messgae {0} - {1} serialId {2} !!", mainno, subno, serialId));
					
                    bool _packcompress = false;
                    uint _PlayerID = m_PlayerID;
					uint _SessionID = m_SessionID;
					byte _SerialID = serialId;

                    if (m_PackCompressSize > 0) //檢查封包壓縮
                    {
                        if (msg.Available > m_PackCompressSize)
                        {
                            _packcompress = true;
                            msg.CompressAndMark();
                        }
                    }

                    ByteArrayBuffer _head = new ByteArrayBuffer();
                    _head.WriteUShort(m_Version);
					_head.WriteUInt((uint)(m_MsgHeadSize + msg.Available));
                    _head.WriteByte(mainno);
                    _head.WriteByte(subno);
                    _head.WriteBool(_packcompress);
                    _head.WriteUInt(_PlayerID);
                    _head.WriteUInt(_SessionID);
					_head.WriteByte(_SerialID);

                    int _headlen = _head.Available;
                    m_SendBuf.Write(_head.ReadAllData(), 0, _headlen);
                    
                    //write msg
                    int _msglen = msg.Available;
                    m_SendBuf.Write(msg.ReadAllData(), 0, _msglen);

                    //socket send 
                    byte[] _buf = new byte[m_SendBuf.Length];
                    m_SendBuf.Read(_buf, 0, _buf.Length);
                    m_tcpSsocket.Send(_buf);

                    //event
                    if (_OnSendMsg != null)
                        _OnSendMsg(this, new SendMsgEventArgs());
                }
                catch (SocketException se)
                {
					//UnityEngine.Debug.Log(se.Message);
                }
            }
        }

		//關閉連線
        public void Close ()
        {
            DoDisConnect();
        }

		//加入協定事件
        public void AddEvent (int Kind1, int Kind2, TNetMessageProc Evt)
        {
            m_NetMsgProcs[Kind1, Kind2] = Evt;
        }

		//加入Debug訊息
        private void AddLog (string msg)
        {
            if (DoLog != null)
                DoLog(msg);
        }
    }
}