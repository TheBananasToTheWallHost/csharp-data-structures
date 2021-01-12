using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Utils;
using BananaTurtles.CSharp.Extensions;
using System.Linq;
using System.Collections;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    /// <summary>
    /// Represents a binary heap structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlexBinaryHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private int _count;

        /// <summary>
        /// Gets the number of values in the BinaryHeap.
        /// </summary>
        public int Count{
            get => _count;
            private set => _count = value;
        }

        public bool IsReadOnly {
            get => false;
        }

        private T[] _heapArray;
        private readonly int DEFAULT_SIZE = 16;
        private HeapType _heapType;

        private delegate int ChildSelectionOpDelegate(int x, int y);
        private ChildSelectionOpDelegate SelectionOp;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexBinaryHeap{T}"/> class that is empty.
        /// </summary>
        /// <param name="heapType">The type of the heap. This can be either Min or Max. <see cref="HeapType"/></param>
        public FlexBinaryHeap(HeapType heapType = HeapType.Min)
        {
            _heapArray = new T[DEFAULT_SIZE];
            _heapType = heapType;
            List<int> list = new List<int>();
            SetChildSelectionOp(heapType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexBinaryHeap{T}"/> class that is filled with the values in 
        /// <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">An IEnumerable whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public FlexBinaryHeap(IEnumerable<T> enumerable, HeapType heapType = HeapType.Min) : this(enumerable.ToArray(), heapType){
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="span"/>. 
        /// </summary>
        /// <param name="span">A Span whoe values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public FlexBinaryHeap(Span<T> span, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            InitializeUnderlyingArray(span.Length);
            Count = span.Length;
            SetChildSelectionOp(heapType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="array"/>.  
        /// </summary>
        /// <param name="array">An Array whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public FlexBinaryHeap(T[] array, HeapType heapType = HeapType.Min)
        {
            _heapType = heapType;
            InitializeUnderlyingArray(array.Length);
            Count = array.Length;
            SetChildSelectionOp(heapType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="list"/>.  
        /// </summary>
        /// <param name="list">An IList whose values will be added to the BinaryHeap.</param>
        /// <param name="heapType">The type of the heap. This can be either Min or Max.</param>
        public FlexBinaryHeap(IList<T> list, HeapType heapType = HeapType.Min){
            _heapType = heapType;
            InitializeUnderlyingArray(list.Count);
            Count = list.Count;
            SetChildSelectionOp(heapType);
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
            Count++;

            int parentIndex;
            while((parentIndex = Parent(insertionIndex)) >= 0){
                // Comparison values here are dependent on whether the heap is a MinHeap or a MaxHeap.
                // In a max heap we swap if the parent is less than the child, in a min heap we swap if the child is
                // less than the parent. Here, insertionIndex is the child.
                T comparisonLeft = _heapType == HeapType.Max ? _heapArray[parentIndex] : _heapArray[insertionIndex];
                T comparisonRight = _heapType == HeapType.Max ? _heapArray[insertionIndex] : _heapArray[parentIndex];

                if(comparisonLeft.CompareTo(comparisonRight) < 0){
                    _heapArray.Swap(insertionIndex, parentIndex);
                    insertionIndex = parentIndex;
                    continue;
                }

                // If this point is reached, the value is in the correct position and the heap property is maintained
                break;
                
            }
            return true;
        }

        public T Pop(){
            if(Count < 1){
                throw new InvalidOperationException("There are no items to pop.");
            }

            Pop(out T value);
            return value;
        }

        public bool Pop(out T topValue){
            if(Count < 1){
                topValue = default(T);
                return false;
            }

            T value = _heapArray[0];

            int insertionIndex = 0;
            _heapArray[insertionIndex] = _heapArray[Count - 1];
            Count--;

            while(true){
                int leftChild = LeftChild(insertionIndex);
                int rightChild = RightChild(insertionIndex);

                // Comparison values here are dependent on whether the heap is a MinHeap or a MaxHeap.
                // In a max heap we swap if the parent is less than the child, in a min heap we swap if the child is
                // less than the parent. Here, insertionIndex is the parent.
                T comparisonLeft = _heapType == HeapType.Max ? _heapArray[insertionIndex] : _heapArray[leftChild];
                T comparisonRight = _heapType == HeapType.Max ? _heapArray[leftChild] : _heapArray[insertionIndex];

                // Test left child and parent
                if(comparisonLeft.CompareTo(comparisonRight) < 0){
                    _heapArray.Swap(leftChild, insertionIndex);
                    insertionIndex = leftChild;
                    continue;
                }

                // Need to recalculate the comparison values in order to test the parent and right child.
                comparisonLeft = _heapType == HeapType.Max ? _heapArray[insertionIndex] : _heapArray[rightChild];
                comparisonRight = _heapType == HeapType.Max ? _heapArray[rightChild] : _heapArray[insertionIndex];

                if(comparisonLeft.CompareTo(comparisonRight) < 0){
                    _heapArray.Swap(rightChild, insertionIndex);
                    insertionIndex = rightChild;
                    continue;
                }

                // If this point is reached, the value that was pulled up from the bottom of the heap is in the correct 
                // position and the heap property is maintained
                break;
            }

            topValue = value;
            return true;
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
            return Contains(value, out int _);
        }

        public bool Contains(T value, out int index){
            index = Array.IndexOf(_heapArray, value);

            return index == -1 ? false : true;
        }

        public void CopyTo(T[] array, int arrayIndex){
            if(array is null){
                throw new ArgumentNullException($"{nameof(array)} is null");
            }
            
            if(array.Length - arrayIndex < Count){
                throw new ArgumentException(
                    $"Not enough space in {nameof(array)} to copy the values in the Binary Heap. " +
                    $"Available space ({array.Length - arrayIndex}, needed space ({Count}))"
                    );
            }

            if(arrayIndex < 0){
                throw new ArgumentOutOfRangeException($"Negative value used for {nameof(arrayIndex)}.");
            }

            _heapArray.CopyTo(array, arrayIndex);
        }

        public void Clear(){
            _heapArray = new T[DEFAULT_SIZE];
            Count = 0;
        }

        public bool Remove(T value){
            int index = Array.IndexOf(_heapArray, value);
            return Remove(index);
        }

        public bool Remove(int index){

        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> for the <see cref="FlexBinaryHeap{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="FlexBinaryHeap{T}"/></returns>
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
        /// <see cref="FlexBinaryHeap{T}"/>. The underlying array is grown when there is over 90% usage. 
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

        /// <summary>
        /// Returns the usage ratio for the underlying array given <paramref name="itemCount"/> values are added.
        /// </summary>
        /// <param name="itemCount">The number of items that are theoretically being added.</param>
        /// <returns>The usage ratio for the underlying array given <paramref name="itemCount"/> values are added.</returns>
        private decimal Usage(int itemCount){
            return itemCount / _heapArray.Length;
        }

        /// <summary>
        /// Returns the usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.
        /// </summary>
        /// <param name="itemCount">The number of items that are theoretically being added.</param>
        /// <param name="size">The size of the theoretical collection.</param>
        /// <returns>The usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.</returns>
        private decimal Usage(int itemCount, int size){
            return itemCount/size;
        }

        /// <summary>
        /// Initializes the underlying array <see cref="_heapArray"/> to an appropriate size given that
        /// <paramref name="inputCollectionSize"/> items are going to be added to it. 
        /// </summary>
        /// <param name="inputCollectionSize">The number of items potentially being added to the <see cref="FlexBinaryHeap{T}"/></param>
        private void InitializeUnderlyingArray(int inputCollectionSize){
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(inputCollectionSize, strict: false));
            if(Usage(inputCollectionSize, size) > .9m && size != int.MaxValue){
                size = MathUtils.CeilingToPowerOfTwo(size + 1, strict: false);
            }
            _heapArray = new T[size];
        }

        private void SetChildSelectionOp(HeapType heapType){
            switch(heapType){
                case HeapType.Max:
                    SelectionOp = Math.Max;
                    break;
                case HeapType.Min:
                    SelectionOp = Math.Min;
                    break;
            }
        }

        private void BubbleUp(){

        }

        private void Heapify(){

        }
        #endregion

        #region Enums
        /// <summary>
        /// Defines constants for types of heaps: Min and Max.
        /// </summary>
        public enum HeapType{
            Max,
            Min
        }
        #endregion
    }
}
