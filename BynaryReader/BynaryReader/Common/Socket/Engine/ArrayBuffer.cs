using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CGEngine.Memory
{
    /**
     * 缓冲区的可能状态:
     * <code>
     * (1)
     * ----====== data ======-----rspace----
     *     |                 |             |
     *     rd_nxt            wr_nxt        capacity-1
     * (2)
     * ==ldata==-------------==== rdata ====
     *          |            |             |
     *          wr_nxt       rd_nxt        capacity-1
     * (3)
     * ===ldata=============rdata===========(full of data)
     *             |
     *             wr_nxt(rd_nxt)
     * (4)
     * -------------------------------------(empty)
     *           |
     *           wr_nxt(rd_nxt)
     * </code>
     */

    /// <summary>
    /// 使用字节数组来实现的缓冲区. 该缓冲区把该数组看作是一个环,
    /// 支持在一块固定的数组上的无限次读和写, 数组的大小不会自动变化.
    /// ideawu
    /// </summary>
    /// <typeparam name="T">所缓冲的数据类型.</typeparam>
    public class ArrayBuffer<T>
    {
        /// <summary>
        /// 默认大小.
        /// </summary>
        private const int DFLT_SIZE = 512 * 1024;

        /// <summary>
        /// 缓冲区还能容纳的元素数目.
        /// </summary>
        private int space = 0;

        /// <summary>
        /// 缓冲区中的数据元素数目.
        /// </summary>
        private int available = 0;

        /// <summary>
        /// 缓冲区的容量.
        /// </summary>
        private int capacity = DFLT_SIZE;
        // 注意 capacity 和 buf.Length 可以不相同, 前者小于或者等于后者.

        /// <summary>
        /// 下一次要将数据写入缓冲区的开始下标.
        /// </summary>
        private int wr_nxt = 0;

        /// <summary>
        /// 下一次读取接收缓冲区的开始下标.
        /// </summary>
        private int rd_nxt = 0;

        /// <summary>
        /// 缓冲区所使用的数组.
        /// </summary>
        protected T[] dataBuf;

        private object bufLock = new object();
        
        /// <summary>
        /// 创建一个具体默认容量的缓冲区.
        /// </summary>
        public ArrayBuffer () : this(DFLT_SIZE)
        {
        }

        /// <summary>
        /// 创建一个指定容量的缓冲区.
        /// </summary>
        /// <param name="capacity">缓冲区的容量.</param>
        public ArrayBuffer (int capacity) : this(new T[capacity])
        {
        }

        /// <summary>
        /// 使用指定的数组来创建一个缓冲区.
        /// </summary>
        /// <param name="buf">缓冲区将要使用的数组.</param>
        public ArrayBuffer (T[] buf) : this(buf, 0, 0)
        {
        }

        protected void CreateBuf (T[] buf, int offset, int size)
        {
            Clear();

            this.dataBuf = buf;
            capacity = buf.Length;
            available = size;
            space = capacity - available;
            rd_nxt = offset;
            wr_nxt = offset + size;
        }

        /// <summary>
        /// 使用指定的数组来创建一个缓冲区, 且该数组已经包含数据.
        /// </summary>
        /// <param name="buf">缓冲区将要使用的数组.</param>
        /// <param name="offset">数据在数组中的偏移.</param>
        /// <param name="size">数据的字节数.</param>
        public ArrayBuffer (T[] buf, int offset, int size)
        {
            CreateBuf(buf, offset, size);
        }

        /// <summary>
        /// 缓冲区还能容纳的元素数目.
        /// </summary>
        public int Space
        {
            get
            {
                return space;
            }
        }

        /// <summary>
        /// 缓冲区中可供读取的数据的元素数目
        /// </summary>
        public int Available
        {
            get
            {
                return available;
            }
        }

        /// <summary>
        /// get, set 接收缓冲区的大小(元素数目). 默认值为 512K.
        /// Capacity 不能设置为小于 Available 的值(实现会忽略这样的值).
        /// </summary>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                lock (bufLock)
                {
                    if (value < available || value == 0)
                    {
                        return;
                        //throw new ApplicationException("Capacity must be larger than Available.");
                    }
                    if (value == capacity)
                    {
                        return;
                    }

                    T[] buf = new T[value];
                    if (available > 0)
                    {
                        available = ReadData(buf, 0, buf.Length);
                        // 下面的用法是错误的!
                        //available = Read(buf, 0, buf.Length);
                    }
                    dataBuf = buf;
                    capacity = value;
                    space = capacity - available;
                    rd_nxt = 0;
                    // 当容量缩小时, 可能导致变化后可写空间为0, 这时wr_nxt=0.
                    wr_nxt = (space == 0) ? 0 : available;
                }
            }
        }

        /// <summary>
        /// 清空本缓冲区.
        /// </summary>
        public void Clear ()
        {
            lock (bufLock)
            {
                available = 0;
                space = capacity;
                rd_nxt = 0;
                wr_nxt = 0;
            }
        }

        /*
        /// <summary>
        /// 将读指针向前移动 num 个单元. 如果 num 大于 Avalable,
        /// 将抛出异常.
        /// </summary>
        /// <param name="num">读指针要向前的单元个数.</param>
        /// <exception cref="ApplicationException">num 大于 Avalable.</exception>
        public void Seek(int num) {
        }
        */

        /// <summary>
        /// 未实现.
        /// </summary>
        /// <returns></returns>
        public T ReadOne ()
        {
            throw new Exception("Not supported.");
        }

        /// <summary>
        /// 从缓冲区中读取数据. 读取的字节数一定是 buf.Length 和 Available 的较小者.
        /// </summary>
        /// <param name="buf">存储接收到的数据的缓冲区.</param>
        /// <returns>已经读取的字节数. 一定是 size 和 Available 的较小者.</returns>
        public int Read (T[] buf)
        {
            return Read(buf, 0, buf.Length);
        }

        /// <summary>
        /// 从缓冲区中读取数据. 读取的字节数一定是 size 和 Available 的较小者.
        /// 本方法是线程安全的.
        /// </summary>
        /// <param name="buf">存储接收到的数据的缓冲区.</param>
        /// <param name="offset">buf 中存储所接收数据的位置.</param>
        /// <param name="size">要读取的字节数.</param>
        /// <returns>已经读取的字节数. 一定是 size 和 Available 的较小者.</returns>
        public int Read (T[] buf, int offset, int size)
        {
            lock (bufLock)
            {
                int nread = ReadData(buf, offset, size);
                
                space += nread;
                available -= nread;

                return nread;
            }
        }
		
		/// <summary>
        /// 把本缓冲区的数据复制指定的数组中, 不移动读指针.
        /// </summary>
		public T[] CopyBuff ()
		{
			T[] buf = new T[available];
            lock (bufLock)
            {
				int now_rd_nxt = rd_nxt;
                ReadData(buf, rd_nxt, available);                
               	rd_nxt = now_rd_nxt;
                return buf;
            }
		}

        /// <summary>
        /// 把本缓冲区的数据复制指定的数组中, 并移动读指针.
        /// </summary>
        private int ReadData (T[] buf, int offset, int size)
        {
            int nread = (available >= size) ? size : available;
            // 当 rd_nxt 在 wr_nxt 的左边时, 缓冲的右边包含的网络字节数.
            int rdata = capacity - rd_nxt;
            if (rd_nxt < wr_nxt || rdata >= nread/*隐含rd_nxt >= wr_nxt*/)
            {
                Array.Copy(dataBuf, rd_nxt, buf, offset, nread);
                rd_nxt += nread;
            }
            else
            {
                // 两次拷贝.
                Array.Copy(dataBuf, rd_nxt, buf, offset, rdata);
                rd_nxt = nread - rdata;
                Array.Copy(dataBuf, 0, buf, offset + rdata, rd_nxt);
            }
            return nread;
        }

        /// <summary>
        /// 写入数据到缓冲区.
        /// </summary>
        /// <param name="buf">要写入的数据的缓冲区.</param>
        //public void Write(byte[] buf)
        public void Write (T[] buf)
        {
            Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// 写入数据到缓冲区. 注意: 本方法不是线程安全的.
        /// </summary>
        /// <param name="buf">要写入的数据的缓冲区.</param>
        /// <param name="offset">数据缓冲区中要写入数据的起始位置.</param>
        /// <param name="size">要写入的字节数.</param>
        /// <exception cref="ApplicationException">如果空间不足, 会抛出异常.</exception>
        //public void Write(byte[] buf, int offset, int size)
        public void Write (T[] buf, int offset, int size)
        {
            int n_left = size;
            int n_offset = offset;
            int nwrite;
            int rspace;

            //20130805 poyu modify auto grow 
            if (size > space)
            {
                int needLen = Capacity * 2;
                while (needLen < (available + size))
                {
                    needLen += Capacity;
                }
                T[] tmp = new T[needLen];
				
				int _srclen = available;
                Read(tmp, 0, _srclen);        //先取出資料
                CreateBuf(tmp, 0, _srclen);   //重建資料             
            }

            while (n_left > 0)
            { 
                lock (bufLock)
                {
                    nwrite = (space >= n_left) ? n_left : space;
                    // 当 rd_nxt 在 wr_nxt 的左边时, 缓冲的右边可以放置的网络字节数.
                    rspace = capacity - wr_nxt;
                    if (wr_nxt < rd_nxt || rspace >= nwrite/*隐含wr_nxt >= rd_nxt*/)
                    {
                        //Buffer.BlockCopy(buf, n_offset, dataBuf, wr_nxt, nwrite);
                        Array.Copy(buf, n_offset, dataBuf, wr_nxt, nwrite);
                        wr_nxt += nwrite;
                        if (wr_nxt == capacity)
                        {
                            wr_nxt = 0;
                        }
                    }
                    else
                    {
                        // 两次拷贝.
                        Array.Copy(buf, n_offset, dataBuf, wr_nxt, rspace);
                        wr_nxt = nwrite - rspace; // 是调用下一句之后的 wr_nxt值.
                        Array.Copy(buf, n_offset + rspace, dataBuf, 0, wr_nxt);
                    }

                    space -= nwrite;
                    available += nwrite;

                    n_offset += nwrite;
                    n_left -= nwrite;
                }
            } // end while
        }
    }
}