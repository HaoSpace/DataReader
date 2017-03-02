using System;
using System.Collections.Generic;
using System.Text;

namespace CGEngine.Memory
{
    public class CacheBufPool<T> where T : class , new()
    {
        private T[] _Caches;                // 庫存 buffer        
        private int _CacheCount;            // buffer 庫存量        
        private int _CacheMax;              // 最大 buffer 快取數
        private int _LeapCount;             // 實際向系統配置 buffer 次數
        
        public CacheBufPool (int max)
        {
            _Caches = new T[max];
            _CacheMax = max;
        }
        
        public T NewNode ()
        {
            if (_CacheCount > 0)
            {
                _CacheCount--;
                return _Caches[_CacheCount];

            }
            else
            {
                T tmp = new T();
                return tmp;
            }
        }

        public void DisposeNode (T P1)
        {
            if (_CacheCount >= _CacheMax)
            {
                //不加入管理
                _LeapCount--;

            }
            else
            {
                _Caches[_CacheCount] = P1;
                _CacheCount++;

            }
        }
    }
}