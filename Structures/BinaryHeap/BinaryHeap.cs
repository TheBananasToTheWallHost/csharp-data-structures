using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Utils;
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

        private T[] heapArray;
        private readonly int DEFAULT_SIZE = 16;

        #region Constructors
        public BinaryHeap(HeapType heapType = HeapType.Min)
        {
            heapArray = new T[DEFAULT_SIZE];
        }

        public BinaryHeap(IEnumerable<T> enumerable, HeapType heapType = HeapType.Min) : this(enumerable.ToArray(), heapType){
        
        }

        public BinaryHeap(Span<T> span, HeapType heapType = HeapType.Min)
        {
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(span.Length, strict: false));

            T[] heapArray = new T[size];
            Count = span.Length;
        }

        public BinaryHeap(T[] array, HeapType heapType = HeapType.Min)
        {
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(array.Length, strict: false));

            T[] heapArray = new T[size];
            Count = array.Length;
        }

        public BinaryHeap(IList<T> list, HeapType heapType = HeapType.Min){
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
            return true;
        }

        public T Pop(){

        }

        public T GetValue(int index){
            return index >= Count ? default(T) : heapArray[index];
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

        #endregion

        #region Helpers
        private bool GrowUnderlyingArray(){
            if(heapArray.Length - Count > 3 || heapArray.Length == Int32.MaxValue){
                return false;
            }

            int newSize = MathUtils.CeilingToPowerOfTwo(heapArray.Length + 1); 
            T[] newHeapArray = new T[newSize];
            heapArray.CopyTo(newHeapArray, 0);
            heapArray = newHeapArray;

            return true;
        }

        private void Heapify(){

        }

        private void SiftDown(){

        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; i++){
                yield return heapArray[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        public enum HeapType{
            Max,
            Min
        }
    }
}
