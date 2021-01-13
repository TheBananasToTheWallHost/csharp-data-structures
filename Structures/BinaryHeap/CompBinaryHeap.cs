using System;
using System.Collections.Generic;
using BananaTurtles.CSharp.Extensions;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    public class CompBinaryHeap<T> : BinaryHeap<T>
    {
        private IComparer<T> _comparer;

        public override void ChangeValue(int index, T newValue)
        {
            if (!IsValidIndex(index))
            {
                throw new ArgumentOutOfRangeException($"The value:[{index}] of {nameof(index)} does not point to a valid heap item.");
            }

            T oldValue = _heapArray[index];

            if (_comparer.Compare(newValue, oldValue) < 0)
            {
                DecreaseValue(index, newValue);
            }
            else if (_comparer.Compare(newValue, oldValue) > 0)
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

            if (_comparer.Compare(newValue, oldValue) >= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not smaller than the value at {index}");
            }

            _heapArray[index] = newValue;
            BubbleUp(index);
        }

        public override void DecreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (_comparer.Compare(newValue, oldValue) >= 0)
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

            if (_comparer.Compare(newValue, oldValue) <= 0)
            {
                throw new ArgumentException($"The value of {nameof(newValue)} is not larger than the value at {index}");
            }

            _heapArray[index] = newValue;
            Heapify(index);
        }

        public override void IncreaseValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if (_comparer.Compare(newValue, oldValue) <= 0)
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

                if (_comparer.Compare(value, parentValue) >= 0)
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

            if (leftChild != -1 && _comparer.Compare(_heapArray[leftChild], _heapArray[smallestValueIndex]) < 0)
            {
                smallestValueIndex = leftChild;
            }

            if (rightChild != -1 && _comparer.Compare(_heapArray[leftChild], _heapArray[smallestValueIndex]) < 0)
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
    }
}