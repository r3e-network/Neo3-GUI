using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Common.Storage.LevelDBModules
{
    public class WriteBatch : IDisposable
    {
        private bool _disposed;
        internal readonly IntPtr handle = Native.leveldb_writebatch_create();

        ~WriteBatch()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            Native.leveldb_writebatch_destroy(handle);
            _disposed = true;
        }

        public void Clear()
        {
            Native.leveldb_writebatch_clear(handle);
        }

        public void Delete(byte[] key)
        {
            Native.leveldb_writebatch_delete(handle, key, (UIntPtr)key.Length);
        }

        public void Put(byte[] key, byte[] value)
        {
            if (value != null)
            {
                Native.leveldb_writebatch_put(handle, key, (UIntPtr)key.Length, value, (UIntPtr)value.Length);
            }
        }
    }
}
