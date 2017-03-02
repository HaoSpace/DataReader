using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using System.Net.NetworkInformation;

namespace CGEngine.Memory
{
    public class EngineTools
    {
        /// <summary>
        /// for 解壓流緩衝使用 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private static void CopyStream (Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int aLen = 0;
            while ((aLen = input.Read(buffer, 0, 2000)) != 0)
            {
                output.Write(buffer, 0, aLen);
            }
            output.Flush();
        }

        //// <summary>
        /// 解壓流
        /// </summary>
        /// <param name="SourceStream">需要被解縮的流數據</param>
        /// <returns></returns>
        public static Stream DecompressStream (Stream SourceStream)
        {
            try
            {
                MemoryStream stmOutput = new MemoryStream();
                ZOutputStream outZStream = new ZOutputStream(stmOutput);
                CopyStream(SourceStream, outZStream);
                outZStream.finish();
                // error			
                return stmOutput;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解壓縮數據資料
        /// </summary>
        /// <param name="SourceByte"></param>
        /// <returns></returns>
        public static byte[] DeCompressBytes (byte[] SourceByte)
        {
            try
            {
                MemoryStream stmInput = new MemoryStream(SourceByte);
                Stream stmOutPut = DecompressStream(stmInput);
                byte[] bytOutPut = new byte[stmOutPut.Length];
                stmOutPut.Position = 0;
                stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
                return bytOutPut;
            }
            catch
            {
                return null;
            }
        }

        //// <summary>
        /// 壓縮流
        /// </summary>
        /// <param name="SourceStream">需要被壓縮的流數據</param>
        /// <returns></returns>
        public static Stream CompressStream (Stream SourceStream)
        {
            try
            {
                MemoryStream stmOutTemp = new MemoryStream();
                ZOutputStream outZStream = new ZOutputStream(stmOutTemp, zlibConst.Z_DEFAULT_COMPRESSION);
                CopyStream(SourceStream, outZStream);
                outZStream.finish();
                return stmOutTemp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 壓縮byte數組數據
        /// </summary>
        /// <param name="SourceByte">需要被壓縮的Byte數組數據</param>
        /// <returns></returns>
        public static byte[] CompressBytes (byte[] SourceByte)
        {
            try
            {
                MemoryStream stmInput = new MemoryStream(SourceByte);
                Stream stmOutPut = CompressStream(stmInput);
                byte[] bytOutPut = new byte[stmOutPut.Length];
                stmOutPut.Position = 0;
                stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
                return bytOutPut;
            }
            catch
            {
                return null;
            }
        }

        //設置數據某一位值
        public static byte SetBitsBoolean (byte Value, byte Bits, bool Flag)
        {
            int tempValue = (int)Value;
            if (Flag)
            {
                tempValue |= (0x1 << Bits);
            }
            else
            {
                tempValue &= ~(0x1 << Bits);
            }
            Value = (byte)tempValue;
            return Value;
        }

        //得到數據某一位值
        public static bool GetBitsBoolean (byte Value, byte Bits)
        {
            int tmpResult = Value >> Bits & 1;
            if (tmpResult == 1)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// 取得Macaddress, 若多張網卡則取其中一張
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress ()
        {
            var allInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in allInterfaces)
            {
                if (adapter.OperationalStatus != OperationalStatus.Up)
                    continue;
                //if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                //    continue;
                                
                var address = adapter.GetPhysicalAddress();                
                if (address.ToString() != "")
                {   
                    byte[] bytes = address.GetAddressBytes();
                    string[] mm = new string[bytes.Length];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        mm[i] = bytes[i].ToString("X2");
                    }
                    return string.Join("-", mm);
                }
            }

            return "00-00-00-00-00-00";
        }

        /*檢查某一個 function 所使用的效能時間
        public static void CallFunctionWithTimeout<T>(Action<T> function, T arg, int timeout, Action callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            CallFunctionWithTimeout(new Action(() => function(arg)), timeout, callback, timeoutCallback);
        }

        public static void CallFunctionWithTimeout<T1, T2>(Action<T1, T2> function, T1 arg1, T2 arg2, int timeout, Action callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            CallFunctionWithTimeout(new Action(() => function(arg1, arg2)), timeout, callback, timeoutCallback);
        }

        public static void CallFunctionWithTimeout(Action function, int timeout, Action callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            Action action = new Action(() =>
            {
                IAsyncResult ar = function.BeginInvoke(null, null);
                if (ar.AsyncWaitHandle.WaitOne(timeout, false))
                {
                    if (callback != null)
                    {
                        callback();
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Call function {0} is timeout.", function.Method.Name));
                    if (timeoutCallback != null)
                    {
                        timeoutCallback();
                    }
                }
            });

            action.BeginInvoke(null, null);
        }

        public static void CallFunctionWithTimeout<T, TResult>(Func<T, TResult> function, T arg, int timeout, Action<TResult> callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            CallFunctionWithTimeout(new Func<TResult>(() => { return function(arg); }), timeout, callback, timeoutCallback);
        }

        public static void CallFunctionWithTimeout<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2, int timeout, Action<TResult> callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            CallFunctionWithTimeout(new Func<TResult>(() => { return function(arg1, arg2); }), timeout, callback, timeoutCallback);
        }

        public static void CallFunctionWithTimeout<TResult>(Func<TResult> function, int timeout, Action<TResult> callback, Action timeoutCallback)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }

            Action action = new Action(() =>
            {
                IAsyncResult ar = function.BeginInvoke(null, null);
                if (ar.AsyncWaitHandle.WaitOne(timeout, false))
                {
                    if (callback != null)
                    {
                        callback(function.EndInvoke(ar));
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Call function {0} is timeout.", function.Method.Name));
                    if (timeoutCallback != null)
                    {
                        timeoutCallback();
                    }
                }
            });

            action.BeginInvoke(null, null);
        }		
	*/
    }
}