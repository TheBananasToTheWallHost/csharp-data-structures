using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Utils;

namespace BananaTurtles.CSharp.DataStructures
{
    public class BinaryHeap<T> where T : IComparable<T>
    {
        private int _count;
        public int Count{
            get => _count;
            set => _count = value;
        }

        private T[] heapArray;
        private readonly int DEFAULT_SIZE = 16;

        public BinaryHeap()
        {
            heapArray = new T[DEFAULT_SIZE];
        }

        public BinaryHeap(Span<T> span)
        {
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(span.Length, strict: false));

            T[] heapArray = new T[size];
        }

        public BinaryHeap(T[] array)
        {
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(array.Length, strict: false));

            T[] heapArray = new T[size];
        }

        public BinaryHeap(List<T> list){
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(list.Count, strict: false));

            T[] heapArray = new T[size];
        }

        private void GrowUnderlyingArray(){

        }
    }
}
