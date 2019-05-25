using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Csvier.Test
{
    public class Counter<T> : IEnumerable<T>
    {
        private IEnumerable<T> enm;
        private CounterEnum<T> counterEnum;
        
        public Counter(IEnumerable<T> enm)
        {
            this.enm = enm;
        }

        public IEnumerator<T> GetEnumerator()
        {
            counterEnum = new CounterEnum<T>(enm.GetEnumerator());
            return counterEnum;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int getCount()
        {
            return counterEnum.count;
        }
    }

    class CounterEnum<T> : IEnumerator<T>
    {
        private IEnumerator<T> enumerator;
        public int count
        {
            get { return count; }
            set { count = value; }
        }

        public CounterEnum(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool MoveNext()
        {
            count++;
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        public T Current => enumerator.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
        }
    }
}