using System;
using BananaTurtles.CSharp.Extensions;

namespace BananaTurtles.CSharp.DataStructures.Heaps
{
    public class MinBinaryHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        public override void ChangeValue(int index, T newValue)
        {
            if(!IsValidIndex(index)){
                throw new ArgumentOutOfRangeException();
            }
        }

        public override void ChangeValue(T oldValue, T newValue)
        {
            int valueIndex = Array.IndexOf(_heapArray, oldValue);

            if(valueIndex == -1){
                throw new ArgumentException($"No item matching {oldValue} was found.");
            }

            ChangeValue(valueIndex, newValue);
        }

        public override void DecreaseValue(int index, T newValue)
        {
            throw new NotImplementedException();
        }

        public override void DecreaseValue(T oldValue, T newValue)
        {
            throw new NotImplementedException();
        }

        public override void IncreaseValue(int index, T newValue)
        {
            throw new NotImplementedException();
        }

        public override void IncreaseValue(T oldValue, T newValue)
        {
            throw new NotImplementedException();
        }

        public override bool TryChangeValue(int index, T newValue)
        {
            throw new NotImplementedException();
        }

        public override bool TryChangeValue(T oldValue, T newValue)
        {
            throw new NotImplementedException();
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

            if(leftChild == -1 && rightChild == -1){
                return;
            }

            int smallestValueIndex = currentIndex;

            if(leftChild != -1 && _heapArray[leftChild].CompareTo(_heapArray[smallestValueIndex]) < 0){
                smallestValueIndex = leftChild;
            }

            if(rightChild != -1 && _heapArray[rightChild].CompareTo(_heapArray[smallestValueIndex]) < 0){
                smallestValueIndex = rightChild;
            }

            if(smallestValueIndex == currentIndex){
                return;
            }

            _heapArray.Swap(currentIndex, smallestValueIndex);
            Heapify(smallestValueIndex);
        }

        protected override void RestoreHeap(T replacementValue, T removedValue, int removalIndex)
        {
            throw new NotImplementedException();
        }
    }
}