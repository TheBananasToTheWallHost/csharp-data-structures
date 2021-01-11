using System;
using System.Collections;
using System.Collections.Generic;

namespace BananaTurtles.CSharp.DataStructures.Heaps{
    public abstract class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
    {
        protected T[] _heapArray;
        protected readonly int DEFAULT_SIZE = 16;

        protected int _count;
        public virtual int Count {
            get => _count;
            private set => _count = value;
        }

        public virtual bool IsReadOnly => throw new NotImplementedException();

        public virtual void Add(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}