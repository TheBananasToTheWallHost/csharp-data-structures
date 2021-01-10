using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Utils;
using BananaTurtles.CSharp.Extensions;
using System.Linq;
using System.Collections;

namespace BananaTurtles.CSharp.DataStructures
{
    public class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private int _count;
        public int Count{
            get => _count;
            set => _count = value;
        }

        int ICollection<T>.Count => throw new NotImplementedException();

        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

        private T[] _heapArray;
        private readonly int DEFAULT_SIZE = 16;
        private HeapType _heapType;

        #region Constructors
        public BinaryHeap(HeapType heapType = HeapType.Min)
        {
            _heapArray = new T[DEFAULT_SIZE];
            _heapType = heapType;
        }

        public BinaryHeap(IEnumerable<T> enumerable, HeapType heapType = HeapType.Min) : this(enumerable.ToArray(), heapType){
        
        }

        public BinaryHeap(Span<T> span, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(span.Length, strict: false));

            _heapArray = new T[size];
            Count = span.Length;
        }

        public BinaryHeap(T[] array, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(array.Length, strict: false));

            _heapArray = new T[size];
            Count = array.Length;
        }

        public BinaryHeap(IList<T> list, HeapType heapType = HeapType.Min){
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(list.Count, strict: false));

            T[] heapArray = new T[size];
            Count = list.Count;
        }
        #endregion

        #region Heap Functionality
        public int LeftChild(int parentIndex){
            int childIndex = (parentIndex * 2) + 1;

            return childIndex >= Count ? -1 : childIndex;
        }

        public int RightChild(int parentIndex){
            int childIndex = (parentIndex * 2) + 2;

            return childIndex >= Count ? -1 : childIndex;
        }

        public int Parent(int childIndex){
            return (childIndex - 1)/2;
        }

        public void Add(T value){
            TryAddValue(value);
        }

        public bool TryAddValue(T value){
            if(Count == Int32.MaxValue){
                return false;   
            }

            GrowUnderlyingArray();

            int insertionIndex = Count;
            _heapArray[insertionIndex] = value;

            int parentIndex;
            while((parentIndex = Parent(insertionIndex)) >= 0){
                // We change the comparison values here to allow the BinaryHeap to support MinHeaps and MaxHeaps.
                // In a max heap we swap if the parent is less than the child, in a min heap we swap if the child is
                // less than the parent. 
                T leftVal = _heapType == HeapType.Max ? _heapArray[parentIndex] : _heapArray[insertionIndex];
                T rightVal = _heapType == HeapType.Max ? _heapArray[insertionIndex] : _heapArray[parentIndex];

                if(leftVal.CompareTo(rightVal) < 0){
                    _heapArray.Swap(insertionIndex, parentIndex);
                    insertionIndex = parentIndex;
                }
                else{
                    break;
                }
            }
            Count++;
            return true;
        }

        public T Pop(){

        }

        public T GetValue(int index){
            return index >= Count ? default(T) : _heapArray[index];
        }

        public bool Contains(T value){

        }

        public bool Contains(T value, out int index){

        }

        public void CopyTo(T[] array, int arrayIndex){

        }

        public void Clear(){

        }

        public bool Remove(T value){

        }

        public bool Remove(int index){

        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; i++){
                yield return _heapArray[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Helpers
        private bool GrowUnderlyingArray(){
            if(_heapArray.Length - Count > 3 || _heapArray.Length == Int32.MaxValue){
                return false;
            }

            int newSize = MathUtils.CeilingToPowerOfTwo(_heapArray.Length + 1); 
            T[] newHeapArray = new T[newSize];
            _heapArray.CopyTo(newHeapArray, 0);
            _heapArray = newHeapArray;

            return true;
        }

        private void Heapify(){

        }

        private void SiftDown(){

        }
        #endregion

        public enum HeapType{
            Max,
            Min
        }
    }
}
