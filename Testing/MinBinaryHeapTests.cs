using Microsoft.VisualStudio.TestTools.UnitTesting;
using BananaTurtles.CSharp.DataStructures.Heaps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Testing
{
    [TestClass]
    public class MinBinaryHeapTests
    {
        MinBinaryHeap<int> _minHeap;
        int[] _intArray;

        [TestInitialize]
        public void Initialize(){
            _intArray = new int[]{
                6,2,8,6,9,4,5,1,0,-3,10,7,20,-1
            };
        }

        [TestMethod]
        public void CreateFromSpanTest(){
            Span<int> span = _intArray.AsSpan();
            _minHeap = new MinBinaryHeap<int>(span);
            int[] heapCopy = new int[_minHeap.Count];
            _minHeap.CopyTo(heapCopy, 0);
            CollectionAssert.AreEquivalent(_intArray, heapCopy);
            Assert.AreEqual(_intArray.Length, _minHeap.Count);
        }

        [TestMethod]
        public void CreateFromArrayTest()
        {
            _minHeap = new MinBinaryHeap<int>(_intArray);
            int[] heapCopy = new int[_minHeap.Count];
            _minHeap.CopyTo(heapCopy, 0);
            CollectionAssert.AreEquivalent(_intArray, heapCopy);
            Assert.AreEqual(_intArray.Length, _minHeap.Count);
        }

        [TestMethod]
        public void CreateFromListTest(){
            List<int> list = new List<int>(_intArray);
            _minHeap = new MinBinaryHeap<int>(list);
            int[] heapCopy = new int[_minHeap.Count];
            _minHeap.CopyTo(heapCopy, 0);
            CollectionAssert.AreEquivalent(_intArray, heapCopy);
            Assert.AreEqual(_intArray.Length, _minHeap.Count);
        }

        [TestMethod]
        public void CreateFromIEnumerableTest(){
            IEnumerable<int> enumerable = _intArray.AsEnumerable();
            _minHeap = new MinBinaryHeap<int>(enumerable);
            int[] heapCopy = new int[_minHeap.Count];
            _minHeap.CopyTo(heapCopy, 0);
            CollectionAssert.AreEquivalent(_intArray, heapCopy);
            Assert.AreEqual(_intArray.Length, _minHeap.Count);
        }

        [TestMethod]
        public void PopTest(){
            _minHeap = new MinBinaryHeap<int>(_intArray);

            int top1 = _minHeap.Pop();
            Assert.AreEqual(top1, -3);

            int top2 = _minHeap.Pop();
            Assert.AreEqual(top2, -1);

            int top3 = _minHeap.Pop();
            Assert.AreEqual(top3, 0);

            int top4 = _minHeap.Pop();
            Assert.AreEqual(top4, 1);

            int top5 = _minHeap.Pop();
            Assert.AreEqual(top5, 2);
        }

        [TestMethod]
        public void PopExceptionTest(){
            _minHeap = new MinBinaryHeap<int>();
            Assert.ThrowsException<InvalidOperationException>(() => _minHeap.Pop());
        }

        [TestMethod]
        public void AddTest(){
            _minHeap = new MinBinaryHeap<int>();

            for(int i = 0; i < _intArray.Length; i++){
                _minHeap.Add(_intArray[i]);
                Assert.AreEqual(_minHeap.Count, i + 1);
            }
        }

        [TestMethod]
        public void AddFollowedByPopTest(){
            _minHeap = new MinBinaryHeap<int>();

            for(int i = 0; i < _intArray.Length; i++){
                _minHeap.Add(_intArray[i]);
                Assert.IsTrue(_minHeap.Count == 1);
                Assert.IsTrue(_minHeap.Pop(out int top));
                Assert.AreEqual(top, _intArray[i]);
                Assert.IsTrue(_minHeap.Count == 0);
            }
        }
    }
}
