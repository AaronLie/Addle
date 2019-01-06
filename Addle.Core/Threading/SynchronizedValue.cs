using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Addle.Core.Threading
{
    public class SynchronizedValue<T>
        where T : class
    {
        public class Accessor : IDisposable
        {
            object _lockObject;

            public Accessor(object lockObject, T value)
            {
                _lockObject = lockObject;
                Value = value;

                Monitor.Enter(_lockObject);
            }

            ~Accessor()
            {
                Debug.Assert(_lockObject == null);
            }

            public T Value { get; }

            public void Dispose()
            {
                var lockObject = _lockObject;
                _lockObject = null;
                Monitor.Exit(lockObject);
            }
        }

        readonly object _lock = new object();
        readonly T _value;

        public SynchronizedValue(T value)
        {
            _value = value;
        }

        public Accessor Lock()
        {
            return new Accessor(_lock, _value);
        }
    }

    public static class SynchronizedValueExtensions
    {
        public static void WithLock<T>(this SynchronizedValue<T> @this, Action<T> action)
            where T : class
        {
            using (var synchronized = @this.Lock())
            {
                action(synchronized.Value);
            }
        }

        public static TResult WithLock<T, TResult>(this SynchronizedValue<T> @this, Func<T, TResult> func)
            where T : class
        {
            using (var synchronized = @this.Lock())
            {
                return func(synchronized.Value);
            }
        }
    }
}
