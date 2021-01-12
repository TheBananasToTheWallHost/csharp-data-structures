using System;
using System.Collections;
using System.Collections.Generic;
using BananaTurtles.CSharp.Extensions;
using BananaTurtles.CSharp.Utils;

namespace BananaTurtles.CSharp.DataStructures.Heaps{
    public abstract class BinaryHeap<T> : ICollection<T>
    {
        protected T[] _heapArray;
        protected readonly int DEFAULT_SIZE = 16;

        protected int _count;
        public virtual int Count {
            get => _count;
            private set => _count = value;
        }

        protected bool _isReadOnly;
        public virtual bool IsReadOnly => throw new NotImplementedException();

        /// <summary>
        /// Adds the given value to the BinaryHeap.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public virtual void Add(T value){
            TryAddValue(value);
        }

        /// <summary>
        /// Adds the given value to the BinaryHeap. The return value indicates whether the operation
        /// succeeeded.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Returns true if <paramref name="value"/> was successfully added, false otherwise.</returns>
        public virtual bool TryAddValue(T value){
            if(Count == Int32.MaxValue){
                return false;   
            }

            GrowUnderlyingArray();

            int insertionIndex = Count;
            _heapArray[insertionIndex] = value;
            Count++;

            BubbleUp(insertionIndex);

            return true;
        }

        /// <summary>
        /// Gets the left child's index for the given <paramref name="parentIndex"/>. 
        /// </summary>
        /// <param name="parentIndex">The index of the item you want the left child of.</param>
        /// <returns>The index of the left child or -1 if no child exists.</returns>
        public virtual int LeftChild(int parentIndex){
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
        public virtual int RightChild(int parentIndex){
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
        public virtual int Parent(int childIndex){
            if(childIndex <= 0){
                return -1;
            }

            int parentIndex = (childIndex - 1)/2;

            return parentIndex >= Count ? -1 : parentIndex; 
        }

        public virtual T Pop(){
            if(Count < 1){
                throw new InvalidOperationException("There are no items to pop.");
            }

            Pop(out T value);
            return value;
        }

        public virtual bool Pop(out T topValue){
            if(Count < 1){
                topValue = default(T);
                return false;
            }

            T value = _heapArray[0];

            int replacementIndex = 0;
            _heapArray[replacementIndex] = _heapArray[Count - 1];
            Count--;

            Heapify(replacementIndex);

            topValue = value;
            return true;
        }

        public virtual T GetValue(int index){
            if(index < 0 || index >= Count){
                throw new ArgumentOutOfRangeException(
                    $"The input index \"{index}\" does not currently exist in the BinaryHeap, " +
                    "or is otherwise not a valid index."
                    );
            }

            return _heapArray[index];
        }

        public virtual void Clear()
        {
            _heapArray = new T[DEFAULT_SIZE];
            Count = 0;
        }

        public virtual bool Contains(T value){
            return Contains(value, out int _);
        }

        public virtual bool Contains(T value, out int index){
            index = Array.IndexOf(_heapArray, value);

            return index == -1 ? false : true;
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
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

        public virtual bool Remove(T item)
        {
            int index = Array.IndexOf(_heapArray, item);
            return Remove(index);
        }

        public virtual bool Remove(int index){
            if(index < 0){
                throw new ArgumentOutOfRangeException($"Negative value used for {nameof(index)}.");
            }
            if(index >= Count){
                throw new ArgumentOutOfRangeException($"Value used for {nameof(index)} exceeds the number of items stored in the heap.");
            }

            if(index == Count - 1){
                Count--;
                return true;
            }

            if(index == 0){
                return Pop(out T _ );
            }

            T removedValue = _heapArray[index];
            T replacementValue = _heapArray[Count - 1];
            _heapArray.Swap(index, Count - 1);
            Count--;

            RestoreHeap(replacementValue, removedValue, index);
            return true;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; i++){
                yield return _heapArray[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract void IncreaseValue();

        public abstract void DecreaseValue();

        public abstract void ChangeValue();

        protected abstract void BubbleUp(int startingIndex);

        protected abstract void Heapify(int startingIndex);

        protected abstract void RestoreHeap(T replacementValue, T removedValue, int removalIndex);

        /// <summary>
        /// Grows the size of the underlying array <see cref="_heapArray"/> in order to make sure items can always be added to the 
        /// <see cref="FlexBinaryHeap{T}"/>. The underlying array is grown when there is over 90% usage. 
        /// </summary>
        /// <returns></returns>
        protected virtual bool GrowUnderlyingArray(){
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
        protected decimal Usage(int itemCount){
            return itemCount / _heapArray.Length;
        }

        /// <summary>
        /// Returns the usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.
        /// </summary>
        /// <param name="itemCount">The number of items that are theoretically being added.</param>
        /// <param name="size">The size of the theoretical collection.</param>
        /// <returns>The usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.</returns>
        protected decimal Usage(int itemCount, int size){
            return itemCount/size;
        }

        /// <summary>
        /// Initializes the underlying array <see cref="_heapArray"/> to an appropriate size given that
        /// <paramref name="inputCollectionSize"/> items are going to be added to it. 
        /// </summary>
        /// <param name="inputCollectionSize">The number of items potentially being added to the <see cref="FlexBinaryHeap{T}"/></param>
        protected virtual void InitializeUnderlyingArray(int inputCollectionSize){
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(inputCollectionSize, strict: false));
            if(Usage(inputCollectionSize, size) > .9m && size != int.MaxValue){
                size = MathUtils.CeilingToPowerOfTwo(size + 1, strict: false);
            }
            _heapArray = new T[size];
        }

        protected virtual bool ShrinkUnderlyingArray(){
            if(Usage(Count) > .25m || _heapArray.Length == DEFAULT_SIZE){
                return false;
            }
            
            int newSize = MathUtils.FloorToPowerOfTwo(_heapArray.Length - 1); 
            T[] newHeapArray = new T[newSize];
            _heapArray.CopyTo(newHeapArray, 0);
            _heapArray = newHeapArray;

            return true;
        }
    }
}