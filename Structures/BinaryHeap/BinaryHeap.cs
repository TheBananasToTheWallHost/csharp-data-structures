using System;
using System.Collections;
using System.Collections.Generic;
using BananaTurtles.CSharp.Extensions;
using BananaTurtles.CSharp.Utils;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    public abstract class BinaryHeap<T> : ICollection<T>
    {
        protected T[] _heapArray;
        protected readonly int DEFAULT_SIZE = 16;

        protected int _count;

        /// <summary>
        /// Gets the number of values in the BinaryHeap.
        /// </summary>
        public virtual int Count
        {
            get => _count;
            protected set => _count = value;
        }

        protected bool _isReadOnly;
        public virtual bool IsReadOnly => false;

        #region ICollection Functionality

        /// <summary>
        /// Clears the contents of the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        public virtual void Clear()
        {
            _heapArray = new T[DEFAULT_SIZE];
            Count = 0;
        }

        /// <summary>
        /// Copies all values in the heap into <paramref name="array"/> starting at <paramref name="arrayIndex"/>. Be aware that
        /// this only creates a shallow copy when using reference types.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to begin copying at.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException($"{nameof(array)} is null");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(
                    $"Not enough space in {nameof(array)} to copy the values in the Binary Heap. " +
                    $"Available space ({array.Length - arrayIndex}, needed space ({Count}))"
                    );
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException($"Negative value used for {nameof(arrayIndex)}.");
            }

            for(int i = 0, target = arrayIndex; i < Count; i++, target++){
                array[target] = _heapArray[i];
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> for the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="BinaryHeap{T}"/>.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _heapArray[i];
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> using the generic <see cref="GetEnumerator()" implementation/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion


        #region Overlapping ICollection/Heap functionality

        /// <summary>
        /// Removes the first occurence of <paramref name="item"/>. The first occurence in this case refers to the shallowest, and 
        /// leftmost occurence. The return value indicates whether <paramref name="item"/> was successfully removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if <paramref name="item"/> is removed, false otherwise.</returns>
        public virtual bool Remove(T item)
        {
            int index = Array.IndexOf(_heapArray, item);

            if(index == -1){
                return false;
            }

            return Remove(index);
        }

        /// <summary>
        /// Adds the given value to the BinaryHeap. No values are added once the heap reaches its maximum size.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public virtual void Add(T value)
        {
            TryAddValue(value);
        }

        /// <summary>
        /// Checks if <paramref name="value"/> is found within the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <returns>True if the <see cref="BinaryHeap{T}"/> contains <paramref name="value"/>, false otherwise.</returns>
        public virtual bool Contains(T value)
        {
            return Contains(value, out int _);
        }

        #endregion


        #region Heap functionality
        #region Concrete Methods
        /// <summary>
        /// Gets the value at the top of the heap and removes it from the heap.
        /// </summary>
        /// <returns>The value at the top of the heap.</returns>
        /// <exception cref="InvalidOperationException"/>
        public virtual T Pop()
        {
            if (Count < 1)
            {
                throw new InvalidOperationException("There are no items to pop.");
            }

            Pop(out T value);
            return value;
        }

        /// <summary>
        /// Gets the value at the top of the heap and removes it from the heap. The return value indicates whether or not the 
        /// operation succeeded.
        /// </summary>
        /// <param name="topValue">Contains the value at the top of the heap if the operation succeeded, otherwise it will contain
        /// the default value for the type held by the heap. The operation fails if the heap is empty. The passed in value will be 
        /// overwritten.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public virtual bool Pop(out T topValue)
        {
            if (Count < 1)
            {
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

        /// <summary>
        /// Removes the value at <paramref name="index"/>. The return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="index">The index whose value will be removed.</param>
        /// <returns>True if the value at <paramref name="index"/> is removed, false otherwise.</returns>
        public virtual bool Remove(int index)
        {
            if (index < 0)
            {
                return false;
            }
            if (index >= Count)
            {
                return false;
            }

            if (index == Count - 1)
            {
                Count--;
                return true;
            }

            if (index == 0)
            {
                return Pop(out T _);
            }

            T removedValue = _heapArray[index];
            T replacementValue = _heapArray[Count - 1];
            Count--;

            ChangeValue(index, replacementValue);
            ShrinkUnderlyingArray();
            return true;
        }

        /// <summary>
        /// Adds the given value to the <see cref="BinaryHeap{T}"/>. The return value indicates whether the operation
        /// succeeeded.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>Returns true if <paramref name="value"/> was successfully added, false otherwise.</returns>
        public virtual bool TryAddValue(T value)
        {
            if (Count == Int32.MaxValue)
            {
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
        /// Checks if <paramref name="value"/> is found within the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <param name="index">Contains the index of the first occurrence of <paramref name="value"/> if found,
        /// -1 otherwise.</param>
        /// <returns>True if the <see cref="BinaryHeap{T}"/> contains <paramref name="value"/>, false otherwise.</returns>
        public virtual bool Contains(T value, out int index)
        {
            index = Array.IndexOf(_heapArray, value);

            return index == -1 ? false : true;
        }

        /// <summary>
        /// Gets the left child's index for the given <paramref name="parentIndex"/>. 
        /// </summary>
        /// <param name="parentIndex">The index of the item you want the left child of.</param>
        /// <returns>The index of the left child or -1 if no child exists.</returns>
        public virtual int LeftChild(int parentIndex)
        {
            if (parentIndex < 0)
            {
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
        public virtual int RightChild(int parentIndex)
        {
            if (parentIndex < 0)
            {
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
        public virtual int Parent(int childIndex)
        {
            if (childIndex <= 0)
            {
                return -1;
            }

            int parentIndex = (childIndex - 1) / 2;

            return parentIndex >= Count ? -1 : parentIndex;
        }

        /// <summary>
        /// "Builds" a heap by ensuring the heap property is respected in all subtrees found within <see cref="_heapArray"/>.
        /// </summary>
        protected virtual void BuildHeap(){
            for (int i = _heapArray.Length/2; i >= 0; i--)
            {
                Heapify(i);
            }
        }
        #endregion

        #region Abstract Methods
    
        /// <summary>
        /// This operation is also known as bubble-up, percolate-up, sift-up, trickle-up, swim-up, heapify-up, or cascade-up.
        /// This operation maintains heap order by moving values up to their appropriate places within the heap.
        /// </summary>
        /// <param name="currentIndex">The index of the value currently being considered for moving up.</param>
        protected abstract void BubbleUp(int currentIndex);

        /// <summary>
        /// This operation is also known as bubble-down, percolate-down, sift-down, sink-down, trickle down, heapify-down, 
        /// or cascade-down. This operation maintains heap order by moving values down to their appropriate places within the heap.  
        /// </summary>
        /// <param name="currentIndex">The index of the value currently being considered for moving down.</param>
        protected abstract void Heapify(int currentIndex);

        /// <summary>
        /// Pops the topmost item in the heap and then adds <paramref name="value"/>. 
        /// </summary>
        /// <param name="value">The new value to add to the heap.</param>
        /// <returns>The topmost item in the heap.</returns>
        /// <exception cref="InvalidOperationException"/>
        public abstract T PopAndAdd(T value);

        /// <summary>
        /// Adds <paramref name="value"/> to the heap and pops the topmost item.
        /// </summary>
        /// <param name="value">The new value to add to the heap.</param>
        /// <returns>The topmost item in the heap.</returns>
        public abstract T AddAndPop(T value);

        public abstract void IncreaseValue(int index, T newValue);
        public abstract void IncreaseValue(T oldValue, T newValue);
        public abstract void DecreaseValue(int index, T newValue);
        public abstract void DecreaseValue(T oldValue, T newValue);
        public abstract void ChangeValue(int index, T newValue);
        public abstract void ChangeValue(T oldValue, T newValue);
        public abstract bool TryChangeValue(int index, T newValue);
        public abstract bool TryChangeValue(T oldValue, T newValue);
        #endregion
        #endregion


        #region Helpers
        /// <summary>
        /// Grows the size of the underlying array <see cref="_heapArray"/> in order to make sure items can always be added to the 
        /// <see cref="BinaryHeap{T}"/>. The underlying array is grown when there is over 90% usage. 
        /// </summary>
        /// <returns>True if <see cref="_heapArray"/> was grown, false otherwise.</returns>
        protected virtual bool GrowUnderlyingArray()
        {
            if (Usage(Count) < .9m || _heapArray.Length == Int32.MaxValue)
            {
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
        protected decimal Usage(int itemCount)
        {
            return itemCount / _heapArray.Length;
        }

        /// <summary>
        /// Returns the usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.
        /// </summary>
        /// <param name="itemCount">The number of items that are theoretically being added.</param>
        /// <param name="size">The size of the theoretical collection.</param>
        /// <returns>The usage ratio for some <paramref name="size"/> collection given <paramref name="itemCount"/> values are added.</returns>
        protected decimal Usage(int itemCount, int size)
        {
            return itemCount / size;
        }

        /// <summary>
        /// Initializes the underlying array <see cref="_heapArray"/> to an appropriate size given that
        /// <paramref name="inputCollectionSize"/> items are going to be added to it. 
        /// </summary>
        /// <param name="inputCollectionSize">The number of items potentially being added to the <see cref="FlexBinaryHeap{T}"/></param>
        protected virtual void InitializeUnderlyingArray(int inputCollectionSize)
        {
            int size = Math.Max(DEFAULT_SIZE, MathUtils.CeilingToPowerOfTwo(inputCollectionSize, strict: false));
            if (Usage(inputCollectionSize, size) > .9m && size != int.MaxValue)
            {
                size = MathUtils.CeilingToPowerOfTwo(size + 1, strict: false);
            }
            _heapArray = new T[size];
        }

        /// <summary>
        /// Shrinks the size of the underlying array <see cref="_heapArray"/> in order to make sure space isn't being used
        /// unnecessarily. The underlying array is shrunk when there is usage less than or equal to 25%. 
        /// </summary>
        /// <returns>True if <see cref="_heapArray"/> was shrunk, false otherwise.</returns>
        protected virtual bool ShrinkUnderlyingArray()
        {
            if (Usage(Count) > .25m || _heapArray.Length == DEFAULT_SIZE)
            {
                return false;
            }

            int newSize = MathUtils.FloorToPowerOfTwo(_heapArray.Length - 1);
            T[] newHeapArray = new T[newSize];
            _heapArray.CopyTo(newHeapArray, 0);
            _heapArray = newHeapArray;

            return true;
        }

        /// <summary>
        /// Checks if the input index is valid given the state of the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        protected virtual bool IsValidIndex(int index)
        {
            return (index < Count && index >= 0);
        }
        #endregion
    }
}