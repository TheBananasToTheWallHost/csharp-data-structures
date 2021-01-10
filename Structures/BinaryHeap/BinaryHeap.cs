using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Utils;
using BananaTurtles.CSharp.Extensions;
using System.Linq;
using System.Collections;

namespace BananaTurtles.CSharp.DataStructures
{
    /// <summary>
    /// Represents a binary heap structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private int _count;

        /// <summary>
        /// Gets the number of values in the BinaryHeap.
        /// </summary>
        public int Count{
            get => _count;
            private set => _count = value;
        }

        int ICollection<T>.Count => throw new NotImplementedException();

        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

        private T[] _heapArray;
        private readonly int DEFAULT_SIZE = 16;
        private HeapType _heapType;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class that is empty.
        /// </summary>
        /// <param name="heapType">The type of the heap. This can be either Min or Max. <see cref="HeapType"/></param>
        public BinaryHeap(HeapType heapType = HeapType.Min)
        {
            _heapArray = new T[DEFAULT_SIZE];
            _heapType = heapType;
            List<int> list = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class that is filled with the values in 
        /// <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">An IEnumerable whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public BinaryHeap(IEnumerable<T> enumerable, HeapType heapType = HeapType.Min) : this(enumerable.ToArray(), heapType){
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="span"/>. 
        /// </summary>
        /// <param name="span">A Span whoe values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public BinaryHeap(Span<T> span, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(span.Length, strict: false));

            _heapArray = new T[size];
            Count = span.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="array"/>.  
        /// </summary>
        /// <param name="array">An Array whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public BinaryHeap(T[] array, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(array.Length, strict: false));

            _heapArray = new T[size];
            Count = array.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="list"/>.  
        /// </summary>
        /// <param name="list">An IList whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public BinaryHeap(IList<T> list, HeapType heapType = HeapType.Min){
            _heapType = heapType;
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(list.Count, strict: false));

            T[] heapArray = new T[size];
            Count = list.Count;
        }
        #endregion

        #region Heap Functionality

        /// <summary>
        /// Gets the left child's index for the given <paramref name="parentIndex"/>. 
        /// </summary>
        /// <param name="parentIndex">The index of the item you want the left child of.</param>
        /// <returns>The index of the left child or -1 if no child exists.</returns>
        public int LeftChild(int parentIndex){
            if(parentIndex < 0){
                return -1;
            }

            int childIndex = (parentIndex * 2) + 1;

            return childIndex >= Count ? -1 : childIndex;
        }

        /// <summary>
        /// Gets the right child's index for the given <paramref name="parentIndex"/>.
        /// </summary>
        /// <param name="parentIndex">The index of the item you want the right child of.</param>
        /// <returns>The index of the right child or -1 if no child exists.</returns>
        public int RightChild(int parentIndex){
            if(parentIndex < 0){
                return -1;
            }

            int childIndex = (parentIndex * 2) + 2;

            return childIndex >= Count ? -1 : childIndex;
        }

        /// <summary>
        /// Gets the parents index for the given <paramref name="childIndex"/>
        /// </summary>
        /// <param name="childIndex">The index of the item you want the parent of.</param>
        /// <returns>The index of the parent or -1 if no parent exists</returns>
        public int Parent(int childIndex){
            if(childIndex <= 0){
                return -1;
            }

            int parentIndex = (childIndex - 1)/2;

            return parentIndex >= Count ? -1 : parentIndex; 
        }

        /// <summary>
        /// Adds the given value to the BinaryHeap.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(T value){
            TryAddValue(value);
        }

        /// <summary>
        /// Adds the given value to the BinaryHeap. The return value indicates whether the operation
        /// succeeeded.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Returns true if <paramref name="value"/> was successfully added, false otherwise.</returns>
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

        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index">The index of the value to get.</param>
        /// <returns>The value at the given <paramref name="index"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public T GetValue(int index){
            if(index < 0 || index >= Count){
                throw new ArgumentOutOfRangeException(
                    $"The input index \"{index}\" does not currently exist in the BinaryHeap, " +
                    "or is otherwise not a valid index."
                    );
            }

            return _heapArray[index];
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

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> for the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="BinaryHeap{T}"/></returns>
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

        /// <summary>
        /// Grows the size of the underlying array <see cref="_heapArray"/> in order to make sure items can always be added to the 
        /// <see cref="BinaryHeap{T}"/>. The underlying array is grown when there is over 90% usage. 
        /// </summary>
        /// <returns></returns>
        private bool GrowUnderlyingArray(){
            
            if(Usage(Count) < .9m || _heapArray.Length == Int32.MaxValue){
                return false;
            }

            int newSize = MathUtils.CeilingToPowerOfTwo(_heapArray.Length + 1); 
            T[] newHeapArray = new T[newSize];
            _heapArray.CopyTo(newHeapArray, 0);
            _heapArray = newHeapArray;

            return true;
        }

        private decimal Usage(int items){
            return items / _heapArray.Length;
        }

        private decimal Usage(int items, int size){
            return items/size;
        }

        private void Heapify(){

        }

        private void SiftDown(){

        }
        #endregion

        /// <summary>
        /// Defines constants for types of heaps: Min and Max.
        /// </summary>
        public enum HeapType{
            Max,
            Min
        }
    }
}
