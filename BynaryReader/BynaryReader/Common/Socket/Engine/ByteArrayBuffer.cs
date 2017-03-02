using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ComponentAce.Compression.Libs.zlib;

namespace CGEngine.Memory
{
    public interface IByteArraySerialize 
    {
        void Serialize(ByteArrayBuffer iData);
        void Deserialize(ByteArrayBuffer iData);
    }

    public sealed  class ByteArrayBuffer : ArrayBuffer<Byte>
    {
        private const int DFLT_SIZE = 10 * 1024;      //預設的大小是 10K   
        private const ushort COMPRESSMARK = 35137;    //壓縮標記

        public ByteArrayBuffer () : base(DFLT_SIZE)
        {
        }

        public void WriteArray<T> (ref T[] data) where T : struct, IByteArraySerialize
        {
            WriteUShort((ushort)data.Length);
            foreach (T obj in data)
                obj.Serialize(this);
        }

        public void WriteByte (byte data)
        {
            byte[] tmp = new byte[1];
            tmp[0] = data;
            Write(tmp, 0, tmp.Length);
        }

        public void WriteSByte (sbyte data)
        {
            byte[] tmp = new byte[1];
            tmp[0] = (byte)data;
            Write(tmp, 0, tmp.Length);
        }

        public void WriteShort (short data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteUShort (ushort data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteInt (int data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteUInt (uint data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteLong (long data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteULong (ulong data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteFloat (float data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteDouble (double data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteChar (char data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteBool (bool data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            Write(tmp, 0, tmp.Length);
        }

        public void WriteStringByByte (string data)
        {
            byte[] _tmp = System.Text.Encoding.Unicode.GetBytes(data);
            byte _len = (byte)data.Length;
            WriteByte(_len);
            Write(_tmp, 0, _tmp.Length);
        }

        public void WriteStringByUShort (string data)
        {
            byte[] _tmp = System.Text.Encoding.Unicode.GetBytes(data);
            ushort _len = (ushort)data.Length;
            WriteUShort(_len);
            Write(_tmp, 0, _tmp.Length);
        }

        public void WriteStringByInt (string data)
        {
            byte[] _tmp = System.Text.Encoding.Unicode.GetBytes(data);
            int _len = (int)data.Length;
            WriteInt(_len);
            Write(_tmp, 0, _tmp.Length);
        }

        public void WriteString (string data)
        {
            byte[] _tmp = System.Text.Encoding.Unicode.GetBytes(data);            
            Write(_tmp, 0, _tmp.Length);
        }

        public void WriteDateTime (DateTime data)
        {
            //要知道這行為什麼是這個數值要先知道幾件事
            //Delphi中TDateTime初始時間為     1899/12/30 00:00:00
            //C#中DateTime的初始值為          0001/01/01 00:00:00
            //DateTime是初始日期(Ticks=0) + offset而成
            const double DAYS_BETWEEN_00010101_AND_18991230 = 693593;

            //24 * 60 *60 *1000 * 1000  *10
            const double TIME_UNIT = 864000000000;

            //C# DateTime 為 delphi中所存的日期 + C#與Delphi日期格式初始值的差異
            double doubleTime = data.Ticks / TIME_UNIT - DAYS_BETWEEN_00010101_AND_18991230;

            WriteDouble(doubleTime);
        }

       

        private void WriteValue (object value)
        {
            System.IConvertible convertible = value as System.IConvertible;
            if (convertible != null)
            {
                switch (convertible.GetTypeCode())
                {
                    case System.TypeCode.Boolean:
                        WriteBool((bool)value);
                        return;
                    case System.TypeCode.Char:
                        WriteChar((char)value);
                        return;
                    case System.TypeCode.SByte:
                        WriteSByte((sbyte)value);
                        return;
                    case System.TypeCode.Byte:
                        WriteByte((byte)value);
                        return;
                    case System.TypeCode.Int16:
                        WriteShort((short)value);
                        return;
                    case System.TypeCode.UInt16:
                        WriteUShort((ushort)value);
                        return;
                    case System.TypeCode.Int32:
                        WriteInt((int)value);
                        return;
                    case System.TypeCode.UInt32:
                        WriteUInt((uint)value);
                        return;
                    case System.TypeCode.Int64:
                        WriteLong((long)value);
                        return;
                    case System.TypeCode.UInt64:
                        WriteULong((ulong)value);
                        return;
                    case System.TypeCode.Single:
                        WriteFloat((float)value);
                        return;
                    case System.TypeCode.Double:
                        WriteDouble((double)value);
                        return;
                    case System.TypeCode.DateTime:
                        WriteDouble(((DateTime)value).ToOADate());
                        return;
                    case System.TypeCode.String:
                        return;
                }
            }

            FieldInfo[] f = value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            System.Array.Sort(f, (FieldInfo a, FieldInfo b) => {return (a.MetadataToken - b.MetadataToken);});

            for (int i = 0; i < f.Length; i++)
            {
                if (f[i].FieldType.IsArray == true)
                {
                    Array array = (Array)f[i].GetValue(value);

                    if (array != null)
                    {
                        for (int j = 0; j < array.Length; j++)
                        {
                            WriteValue(array.GetValue(j));
                        }
                    }
                }
                else if (!f[i].IsLiteral)
                {
                    WriteValue(f[i].GetValue(value));
                }
            }
        }

        public bool WriteStruct<T> (T value)
        {
            try
            {
                WriteValue(value);
                return true;
            }
            catch (Exception e)
            {
                //GameInfos.LogException(e);
                return false;
            }
        }

//        public void WriteStruct<T> (T vValue)
//        {
//            int size = Marshal.SizeOf(typeof(T));
//
//            byte[] bytes = new byte[size];
//
//            IntPtr buffer = Marshal.AllocHGlobal(size);
//
//            try
//            {
//                Marshal.StructureToPtr(vValue, buffer, true);
//
//                Marshal.Copy(buffer, bytes, 0, size);
//
//                Write(bytes, 0, bytes.Length);
//            }
//            finally
//            {
//                Marshal.FreeHGlobal(buffer);
//            }
//        }

        //public void WriteBufToJson<T> (T data)
        //{
        //    string str = Tools.SerializeObject(data);
        //    if (str.Length <= ushort.MaxValue / 2)
        //        WriteStringByInt(str);               
        //}

        public void WriteByteS (byte[] data)
        {
            Write(data, 0, data.Length);
        }

        public void WriteCharS (char[] data)
        {
            byte[] val = System.Text.Encoding.ASCII.GetBytes(data);
            Write(val, 0, val.Length);
        }

        public T[] ReadArray<T> () where T : struct, IByteArraySerialize
        {
            int len = ReadUShort();
            T[] data = new T[len];
            for (int i = 0; i< data.Length; i++)
                data[i].Deserialize(this);
            return data;
        }

        public byte ReadByte ()
        {
            byte[] tmp = new byte[1];
            tmp[0] = 0;            
            Read(tmp);
            return tmp[0];
        }

        public sbyte ReadSByte ()
        {
            byte[] tmp = new byte[1];
            tmp[0] = 0;
            Read(tmp);
            return (sbyte)tmp[0];
        }

        public short ReadShort ()
        {
            short data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToInt16(tmp, 0);
        }

        public ushort ReadUShort ()
        {
            ushort data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToUInt16(tmp, 0);
        }

        public int ReadInt ()
        {
            int data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToInt32(tmp, 0);            
        }

        public uint ReadUInt ()
        {
            uint data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToUInt32(tmp, 0);  
        }

        public long ReadLong ()
        {
            long data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToInt64(tmp, 0);  
        }

        public ulong ReadULong ()
        {
            ulong data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToUInt64(tmp, 0);  
        }

        public float ReadFloat ()
        {
            float data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToSingle(tmp, 0);  
        }

        public double ReadDouble ()
        {
            double data = 0;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToDouble(tmp, 0); 
        }

        public char ReadChar ()
        {
            char data = '0';
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToChar(tmp, 0); 
        }

        public bool ReadBool ()
        {
            bool data = false;
            byte[] tmp = BitConverter.GetBytes(data);
            Read(tmp);
            return BitConverter.ToBoolean(tmp, 0); 
        }

        public string ReadStringByByte ()
        {
            byte _len = ReadByte();
            byte[] tmp = new byte[_len * 2];
            Read(tmp);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }

        public string ReadStringByUShort ()
        {
            ushort _len = ReadUShort();
            byte[] tmp = new byte[_len * 2];
            Read(tmp);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }

        public string ReadStringByInt ()
        {
            int _len = ReadInt();
            byte[] tmp = new byte[_len * 2];
            Read(tmp);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }

        public string ReadString (int iLen)
        {
            byte[] tmp = new byte[iLen * 2];
            Read(tmp);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }

        public DateTime ReadDateTime ()
        {
            double doubleTime = ReadDouble();

            //要知道這行為什麼是這個數值要先知道幾件事
            //Delphi中TDateTime初始時間為     1899/12/30 00:00:00
            //C#中DateTime的初始值為          0001/01/01 00:00:00
            //DateTime是初始日期(Ticks=0) + offset而成
            const double DAYS_BETWEEN_00010101_AND_18991230 = 693593;

            //24 * 60 *60 *1000 * 1000  *10
            const double TIME_UNIT = 864000000000;
            
            //C# DateTime 為 delphi中所存的日期 + C#與Delphi日期格式初始值的差異
            return new DateTime((long)((doubleTime + DAYS_BETWEEN_00010101_AND_18991230) * TIME_UNIT));
        }

        private void ReadValue (ref object value)
        {
            System.IConvertible convertible = value as System.IConvertible;
            if (convertible != null)
            {
                switch (convertible.GetTypeCode())
                {
                    case System.TypeCode.Boolean:
                        value = ReadBool();
                        return;
                    case System.TypeCode.Char:
                        value = ReadChar();
                        return;
                    case System.TypeCode.SByte:
                        value = ReadSByte();
                        return;
                    case System.TypeCode.Byte:
                        value = ReadByte();
                        return;
                    case System.TypeCode.Int16:
                        value = ReadShort();
                        return;
                    case System.TypeCode.UInt16:
                        value = ReadUShort();
                        return;
                    case System.TypeCode.Int32:
                        value = ReadInt();
                        return;
                    case System.TypeCode.UInt32:
                        value = ReadUInt();
                        return;
                    case System.TypeCode.Int64:
                        value = ReadLong();
                        return;
                    case System.TypeCode.UInt64:
                        value = ReadULong();
                        return;
                    case System.TypeCode.Single:
                        value = ReadFloat();
                        return;
                    case System.TypeCode.Double:
                        value = ReadDouble();
                        return;
                    case System.TypeCode.DateTime:
                        value = DateTime.FromOADate(ReadDouble());
                        return;
                    case System.TypeCode.String:
                        return;
                }
            }

            FieldInfo[] f = value.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            System.Array.Sort(f, (FieldInfo a, FieldInfo b) => {return (a.MetadataToken - b.MetadataToken);});

            for (int i = 0; i < f.Length; i++)
            {
                if (f[i].FieldType.IsArray == true)
                {
                    Array array = (Array)f[i].GetValue(value);

                    if (array == null)
                    {
                        foreach (FieldAttribute attf in f[i].GetCustomAttributes(false))
                        {
                            int size = attf.Length;
                            array = System.Array.CreateInstance(f[i].FieldType.GetElementType(), size);
                            f[i].SetValue(value, array);
                            break;
                        }
                    }

                    if (array != null)
                    {
                        for (int j = 0; j < array.Length; j++)
                        {
                            object temp = array.GetValue(j);
                            ReadValue(ref temp);
                            array.SetValue(temp, j);
                        }
                    }
                }
                else if (!f[i].IsLiteral)
                {
                    object temp = f[i].GetValue(value);

                    if (temp == null)
                        temp = Activator.CreateInstance(temp.GetType());

                    ReadValue(ref temp);

                    f[i].SetValue(value, temp);
                }
            }
        }

        public bool ReadStruct<T> (ref T value)
        {
            try
            {
                object temp = value;
                ReadValue(ref temp);
                value = (T)temp;

                return true;
            }
            catch (Exception e)
            {
                //GameInfos.LogException(e);
                return false;
            }
        }

//        public bool ReadStruct<T> (ref T Data)
//        {
//            int size = Marshal.SizeOf(typeof(T));
//
//            IntPtr buffer = Marshal.AllocHGlobal(size);
//
//            try
//            {
//                byte[] bytes = new byte[size];
//
//                Read(bytes);
//
//                Marshal.Copy(bytes, 0, buffer, size);
//
//                Data = (T)Marshal.PtrToStructure(buffer, typeof(T));
//
//                Marshal.FreeHGlobal(buffer);
//
//                return true;
//            }
//            catch (Exception e)
//            {
//                GameInfos.ExceptionMsg(e);
//
//                Marshal.FreeHGlobal(buffer);
//
//                return false;
//            }
//        }

    //    public bool ReadBufToJSON<T> (ref T Data)
    //    {
    //        string str = ReadStringByInt();

    //        try
    //        {
				//Data = Tools.DeserializeObject<T>(str);
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            GameInfos.LogException(e);

    //            return false;
    //        }
    //    }

        public byte[] ReadByteS (int iLen)
        {
            byte[] _tmp = new byte[iLen];
            Read(_tmp, 0, _tmp.Length);
            return _tmp;        
        }

        public char[] ReadCharS (int iLen)
        {
            char[] _tmp = new char[iLen];
            for (int i = 0; i < _tmp.Length; i++)
                _tmp[i] = ReadChar();
            return _tmp;     
        }

        public byte[] ReadAllData ()
        {
            byte[] _tmp = new byte[Available];
            Read(_tmp, 0, _tmp.Length);
            return _tmp;            
        }

        /// <summary>
        /// 整包壓縮
        /// </summary>
        public void Compress ()
        {
            byte[] src = ReadAllData();
            byte[] compressData = EngineTools.CompressBytes(src);
            Write(compressData, 0, compressData.Length);
        }

        /// <summary>
        /// 整包解壓
        /// </summary>
        public void Decompress ()
        {
            byte[] src = ReadAllData();
            byte[] deccompressData = EngineTools.DeCompressBytes(src);

            if (deccompressData.Length > Capacity)
            {
                CreateBuf(deccompressData, 0, deccompressData.Length);
            }
            else
            {
                Write(deccompressData, 0, deccompressData.Length);
            }
        }

        /// <summary>
        /// 整包壓縮(有標記)
        /// </summary>
        public void CompressAndMark ()
        {
            byte[] src = ReadAllData();
            byte[] compressData = EngineTools.CompressBytes(src);
            ushort mark = COMPRESSMARK;
            WriteUShort(mark);
			WriteInt(compressData.Length);
            Write(compressData, 0, compressData.Length);
        }

        /// <summary>
        /// 整包解壓(有標記) 
        /// </summary>
        public void DecompressAndMark ()
        {
            ushort mark = ReadUShort();
            if (mark != COMPRESSMARK)
            {
                throw new Exception("Decompress Error Mark");                
            }

            int len = ReadInt();

            byte[] src = ReadByteS(len);
            byte[] deccompressData = EngineTools.DeCompressBytes(src);

            if (deccompressData.Length > Capacity)
            {
                CreateBuf(deccompressData, 0, deccompressData.Length);
            }
            else
            {
                Write(deccompressData, 0, deccompressData.Length);
            }
        }

        /// <summary>
        /// 整包加密
        /// </summary>
        /// <param name="xorbyte"></param>
        public void Encode (byte xorbyte)
        {
            byte[] src = ReadAllData();
            for (int i = 0 ; i < src.Length ; i++)
            {
                src[i] ^= xorbyte;
            }
        }

        /// <summary>
        /// 整包解密
        /// </summary>
        /// <param name="xorbyte"></param>
        public void Decode (byte xorbyte)
        {
            byte[] src = ReadAllData();
            for (int i = 0; i < src.Length; i++)
            {
                src[i] ^= xorbyte;
            }
        }
    }
}