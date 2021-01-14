using System;
using System.Collections.Generic;
using System.Linq;
using BananaTurtles.CSharp.Extensions;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    public class MinBinaryHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MinBinaryHeap{T}"/> class that is empty.
        /// </summary>
        public MinBinaryHeap()
        {
            _heapArray = new T[DEFAULT_SIZE];
            Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinBinaryHeap{T}"/> class that is filled with the values in 
        /// <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">An IEnumerable whose values will be added to the BinaryHeap.</param>
        public MinBinaryHeap(IEnumerable<T> enumerable){
            Count = enumerable.Count();
            InitializeUnderlyingArray(Count);
            
            var enumerator = enumerable.GetEnumerator();

            int i = 0;
            while(enumerator.MoveNext()){
                _heapArray[i++] = enumerator.Current;
            }

            BuildHeap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="span"/>. 
        /// </summary>
        /// <param name="span">A Span whoe values will be added to the BinaryHeap.</param>
        public MinBinaryHeap(Span<T> span)
        {
            Count = span.Length;
            InitializeUnderlyingArray(Count);
            for (int i = 0; i < span.Length; i++)
            {
                _heapArray[i] = span[i];
            }

            BuildHeap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="array"/>.  
        /// </summary>
        /// <param name="array">An Array whose values will be added to the BinaryHeap.</param>
        public MinBinaryHeap(T[] array)
        {
            Count = array.Length;
            InitializeUnderlyingArray(Count);
            array.CopyTo(_heapArray, 0);

            BuildHeap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinBinaryHeap{T}"> class that is filled with the values in 
        /// <paramref name="list"/>.  
        /// </summary>
        /// <param name="list">An IList whose values will be added to the BinaryHeap.</param>
        public MinBinaryHeap(IList<T> list){
            Count = list.Count;
            InitializeUnderlyingArray(Count);
            list.CopyTo(_heapArray, 0);

            BuildHeap();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Changes the value of the item at <paramref name="index"/> to <paramref name="newValue"/>.
        /// </summary>
        /// <param name="index">The index to change the value of.</param>
        /// <param name="newValue">The new value for the heap item at <paramref name="index"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public override void ChangeValue(int index, T newValue)
        {
            if (!IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException($"The value:[{index}] of {nameof(index)} does not point to a valid heap item.");
            }

            T oldValue = _heapArray[index];

            if (newValue.CompareTo(oldValue) < 0)
            {
                DecreaseValue(index, newValue);
            }
            else if (newValue.CompareTo(oldValue) > 0)
            {
                IncreaseValue(index, newValue);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Changes the value of the first occurence of <paramref name="oldValue"/> to <paramref name="newValue"/>.
        /// </summary>
        /// <param name="oldValue">The value currently in the heap that will be updated to <paramref name="newValue"/></param>
        /// <param name="newValue">The new value for the first occurence of <paramref name="oldValue"/>.</param>
        /// <exception cref="ArgumentException"/>
        public override void ChangeValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (valueIndex == -1)
            {
                throw new ArgumentException($"No item matching {oldValue} was found.");
            }

            ChangeValue(valueIndex, newValue);
        }

        /// <summary>
        /// Decreases the value of the item at <paramref name="index"/> to <paramref name="newValue"/>. 
        /// The <paramref name="newValue"/> must be smaller than the value currently at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to change the value of.</param>
        /// <param name="newValue">The new value for the heap item at <paramref name="index"/>. This value must be smaller than 
        /// the value currently at <paramref name="index"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public override void DecreaseValue(int index, T newValue)
        {
            if (!IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException($"The value:[{index}] of {nameof(index)} does not point to a valid heap item.");
            }

            T oldValue = _heapArray[index];

            if (newValue.CompareTo(oldValue) >= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not smaller than the value at {index}");
            }

            _heapArray[index] = newValue;
            BubbleUp(index);
        }

        /// <summary>
        /// Decreases the value of the first occurrence of <paramref name="oldValue"/> to <paramref name="newValue"/>.
        /// The <paramref name="newValue"/> must be smaller than the <paramref name="oldValue"/>.
        /// </summary>
        /// <param name="oldValue">The value currently in the heap that will be updated to <paramref name="newValue"/></param>
        /// <param name="newValue">The new value for the first occurence of <paramref name="oldValue"/>. This value must be smaller than 
        /// <paramref name="oldValue"/>.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public override void DecreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (newValue.CompareTo(oldValue) >= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not smaller than the value of {nameof(oldValue)}");
            }

            if (valueIndex == -1)
            {
                throw new InvalidOperationException($"No item matching {oldValue} was found.");
            }

            DecreaseValue(valueIndex, newValue);
        }

        /// <summary>
        /// Increases the value of the item at <paramref name="index"/> to <paramref name="newValue"/>.
        /// The <paramref name="newValue"/> must be larger than the value currently at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to change the value of.</param>
        /// <param name="newValue">The new value for the heap item at <paramref name="index"/>. This value must be larger than 
        /// the value currently at <paramref name="index"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public override void IncreaseValue(int index, T newValue)
        {
            if (!IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException($"The value:[{index}] of {nameof(index)} does not point to a valid heap item.");
            }

            T oldValue = _heapArray[index];

            if (newValue.CompareTo(oldValue) <= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not larger than the value at {index}");
            }

            _heapArray[index] = newValue;
            Heapify(index);
        }

        /// <summary>
        /// Increases the value of the first occurrence of <paramref name="oldValue"/> to <paramref name="newValue"/>.
        /// The <paramref name="newValue"/> must be smaller than the <paramref name="oldValue"/>.
        /// </summary>
        /// <param name="oldValue">The value currently in the heap that will be updated to <paramref name="newValue"/></param>
        /// <param name="newValue">The new value for the first occurence of <paramref name="oldValue"/>. This value must be larger than 
        /// <paramref name="oldValue"/></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public override void IncreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (newValue.CompareTo(oldValue) <= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not larger than the value of {nameof(oldValue)}");
            }

            if (valueIndex == -1)
            {
                throw new InvalidOperationException($"No item matching {oldValue} was found.");
            }

            IncreaseValue(valueIndex, newValue);
        }

        /// <summary>
        /// Changes the value of the item at <paramref name="index"/> to <paramref name="newValue"/>. The return value indicates
        /// whether the change operation succeeded.
        /// </summary>
        /// <param name="index">The index to change the value of.</param>
        /// <param name="newValue">The new value for the heap item at <paramref name="index"/>.</param>
        /// <returns>True if the value at <paramref name="index"/> was successfully changed, false otherwise.</returns>
        public override bool TryChangeValue(int index, T newValue)
        {
            try
            {
                ChangeValue(index, newValue);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the value of the first occurrence of <paramref name="oldValue"/> to <paramref name="newValue"/>. 
        /// The return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="oldValue">The value currently in the heap that will be updated to <paramref name="newValue"/></param>
        /// <param name="newValue">The new value for the first occurence of <paramref name="oldValue"/>.</param>
        /// <returns>True if <paramref name="oldValue"/> was successfully found and changed, false otherwise.</returns>
        public override bool TryChangeValue(T oldValue, T newValue)
        {
            try
            {
                ChangeValue(oldValue, newValue);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        protected override void BubbleUp(int currentIndex)
        {
            int parentIndex;
            while ((parentIndex = Parent(currentIndex)) >= 0)
            {

                T parentValue = _heapArray[parentIndex];
                T value = _heapArray[currentIndex];

                if (value.CompareTo(parentValue) >= 0)
                {
                    break;
                }

                _heapArray.Swap(currentIndex, parentIndex);
                currentIndex = parentIndex;
            }
        }

        protected override void Heapify(int currentIndex)
        {
            int leftChild = LeftChild(currentIndex);
            int rightChild = RightChild(currentIndex);

            if (leftChild == -1 && rightChild == -1)
            {
                return;
            }

            int smallestValueIndex = currentIndex;

            if (leftChild != -1 && _heapArray[leftChild].CompareTo(_heapArray[smallestValueIndex]) < 0)
            {
                smallestValueIndex = leftChild;
            }

            if (rightChild != -1 && _heapArray[rightChild].CompareTo(_heapArray[smallestValueIndex]) < 0)
            {
                smallestValueIndex = rightChild;
            }

            if (smallestValueIndex == currentIndex)
            {
                return;
            }

            _heapArray.Swap(currentIndex, smallestValueIndex);
            Heapify(smallestValueIndex);
        }

        public override T AddAndPop(T value)
        {
            T top;
            if(Count < 1 || value.CompareTo(top = _heapArray[0]) <= 0){
                return value;
            }
            
            _heapArray[0] = value;
            Heapify(0);
            return top;
        }

        public override T PopAndAdd(T value)
        {
            if(Count < 1){
                throw new InvalidOperationException("There are no items to pop.");
            }

            T top = _heapArray[0];
            _heapArray[0] = value;
            Heapify(0);
            return top;
        }
        #endregion
    }
}