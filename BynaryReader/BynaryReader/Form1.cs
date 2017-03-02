using CGEngine.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BynaryReader
{
    [StructureAttribute(273)]
    public struct RtestType
    {
        [FieldAttribute(4)]
        public int ID;
        [FieldAttribute(4)]
        public ushort LV;
        [FieldAttribute(16 * 2)]
        public WideChar16 Name;
        [FieldAttribute(16 * 2)]
        public WideChar16 Password;
        [FieldAttribute(100 * 2)]
        public WideChar100 Content;
        [FieldAttribute(1)]
        public byte Status;
    }


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog vOpenFileDialog = new OpenFileDialog();

            if (vOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(vOpenFileDialog.FileName);

                //慢 安全
                ByteArrayBuffer vBuffer = new ByteArrayBuffer();
                if (WriteArrayBufferData<RtestType>(sr.BaseStream, ref vBuffer) == true)
                {
                    RtestType vData = new RtestType();

                    vBuffer.ReadStruct<RtestType>(ref vData);

                    MessageBox.Show(vData.Password);
                }

                //快 不穩定
                //RtestType vStruct = ReadStructData<RtestType>(sr.BaseStream);

                sr.Close();
            }
        }

        //寫入資料
        public bool WriteArrayBufferData<T>(Stream vStream, ref ByteArrayBuffer vBuffer)
        {
            try
            {
                StructureAttribute vStructAttribute = typeof(T).GetCustomAttribute<StructureAttribute>();
                
                var sz = vStructAttribute.TotalSize;
                var vByteAry = new byte[sz];
                vStream.Read(vByteAry, 0, sz);

                vBuffer = new ByteArrayBuffer();
                vBuffer.Write(vByteAry, 0, sz);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public T ReadStructData<T>(Stream vStream)
        {
            var sz = Marshal.SizeOf(typeof(T));
            var buffer = new byte[sz];
            vStream.Read(buffer, 0, sz);
            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(T));
            pinnedBuffer.Free();
            return structure;
        }

        //讀取架構檔案
        //public T ReadStructData<T>(string vData)
        //{
        //    IntPtr valPoint = Marshal.StringToBSTR(vData);

        //    T ret = (T)Marshal.PtrToStructure(valPoint, typeof(T));
        //    Marshal.FreeBSTR(valPoint);

        //    return ret;
        //}
    }
}
