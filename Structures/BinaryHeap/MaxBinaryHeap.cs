using System;
using BananaTurtles.CSharp.Extensions;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    public class MaxBinaryHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
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

        public override void ChangeValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (valueIndex == -1)
            {
                throw new ArgumentException($"No item matching {oldValue} was found.");
            }

            ChangeValue(valueIndex, newValue);
        }

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
            Heapify(index);
        }

        public override void DecreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (newValue.CompareTo(oldValue) >= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not smaller than the value of {nameof(oldValue)}");
            }

            if (valueIndex == -1)
            {
                throw new ArgumentException($"No item matching {oldValue} was found.");
            }

            DecreaseValue(valueIndex, newValue);
        }

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
            BubbleUp(index);
        }

        public override void IncreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (newValue.CompareTo(oldValue) <= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not larger than the value of {nameof(oldValue)}");
            }

            if (valueIndex == -1)
            {
                throw new ArgumentException($"No item matching {oldValue} was found.");
            }

            IncreaseValue(valueIndex, newValue);
        }

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

                if (value.CompareTo(parentValue) <= 0)
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

            int largestValueIndex = currentIndex;

            if (leftChild != -1 && _heapArray[leftChild].CompareTo(_heapArray[largestValueIndex]) > 0)
            {
                largestValueIndex = leftChild;
            }

            if (rightChild != -1 && _heapArray[rightChild].CompareTo(_heapArray[largestValueIndex]) > 0)
            {
                largestValueIndex = rightChild;
            }

            if (largestValueIndex == currentIndex)
            {
                return;
            }

            _heapArray.Swap(currentIndex, largestValueIndex);
            Heapify(largestValueIndex);
        }

    }
}