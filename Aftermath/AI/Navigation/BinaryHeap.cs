using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.AI.Navigation
{
    /// <summary>
    /// Implementation of a binary min heap. Acts as a kind of insertable priority queue with 
    /// fast retrieval of the lowest item and fast insertion of new items into the right location.
    /// Also supports updating and reordering of items.
    /// </summary>
    class BinaryHeap<T>
    {
        int _count = 0;
        //heap is implemented as an array
        List<BinaryHeapItem> _items;
        //use dictionary for fast retrieval of the index of an item in the array
        Dictionary<T, int> _itemIndex = new Dictionary<T, int>();

        public BinaryHeap(int capacity = 100)
        {
            _items = new List<BinaryHeapItem>(capacity);
        }

        /// <summary>
        /// Add a new item to the heap with a specified value
        /// </summary>
        public void Add(T item, int value)
        {
            BinaryHeapItem newItem = new BinaryHeapItem();
            newItem.Value = value;
            newItem.Item = item;
            if (_items.Count > _count)
                _items[_count] = newItem;
            else
                _items.Add(newItem);
            _itemIndex[item] = _count;
            _count++;
            BubbleUp(_count - 1);
        }

        /// <summary>
        /// Removes and returns the top item of the heap. The top item will always be
        /// the item with the lowest value
        /// </summary>
        public T Remove()
        {
            T ret = _items[0].Item;
            _itemIndex.Remove(ret);
            int lastIndex = _count - 1;
            _count--;
            //if there are any other items then move the last item to the first position and bubble it down
            if (_count > 0)
            {
                _items[0].Item = _items[lastIndex].Item;
                _items[0].Value = _items[lastIndex].Value;
                _itemIndex[_items[0].Item] = 0;
                BubbleDown(0);
            }
            return ret;
        }

        /// <summary>
        /// Assigns an existing item in the heap a new value and repositions it into the correct place
        /// </summary>
        public void Reposition(T item, int newValue)
        {
            int index = IndexOf(item);
            _items[index].Value = newValue;

            HashSet<int> nic = new HashSet<int>();
            for (int i=0;i<_count;i++)
            {
                int nid = ((INavigatableNode)_items[i].Item).NodeUid;
                if (nic.Contains(nid))
                    throw new Exception();
                nic.Add(nid);
            }

            if (!BubbleUp(index))
                BubbleDown(index);
        }

        /// <summary>
        /// Returns the array index of an existing item in the heap
        /// </summary>
        int IndexOf(T item)
        {
            return _itemIndex[item];
        }

        /// <summary>
        /// Bubble up an item in the heap until it is in the correct position
        /// </summary>
        bool BubbleUp(int index)
        {
            bool swapped = false;
            int pos = index + 1;
            while (true)
            {
                if (pos == 1)
                    break;
                int parentPos = pos / 2;
                var posItem = _items[pos - 1];
                var parentItem = _items[parentPos - 1];
                if (parentItem.Value <= posItem.Value)
                    break;
                var item = posItem.Item;
                int value = posItem.Value;
                posItem.Item = parentItem.Item;
                posItem.Value = parentItem.Value;
                parentItem.Item = item;
                parentItem.Value = value;
                _itemIndex[posItem.Item] = pos - 1;
                _itemIndex[parentItem.Item] = parentPos - 1;
                swapped = true;
                pos = parentPos;
            }
            return swapped;
        }

        /// <summary>
        /// Bubble down an item in the heap until it is in the correct position
        /// </summary>
        void BubbleDown(int index)
        {
            int swapPos;
            int pos = index + 1;
            while (true)
            {
                BinaryHeapItem posItem = _items[pos - 1];

                int i1 = pos * 2;
                if (i1 > _count)
                    break;
                int i2 = pos * 2 + 1;
                if (i2 > _count)
                    swapPos = i1;
                else
                {
                    if (_items[i1 - 1].Value < _items[i2 - 1].Value)
                        swapPos = i1;
                    else
                        swapPos = i2;
                }
                BinaryHeapItem swapItem = _items[swapPos - 1];
                if (posItem.Value <= swapItem.Value)
                    break;
                var item = posItem.Item;
                int value = posItem.Value;
                posItem.Item = swapItem.Item;
                posItem.Value = swapItem.Value;
                swapItem.Item = item;
                swapItem.Value = value;
                _itemIndex[posItem.Item] = pos - 1;
                _itemIndex[swapItem.Item] = swapPos - 1;
                pos = swapPos;
            }
        }

        /// <summary>
        /// Internal representation of an item on the heap
        /// </summary>
        public class BinaryHeapItem    //MOBO remove public
        {
            public int Value;
            public T Item;

            public override string ToString()
            {
                return Item.ToString() + "   " + Value;
            }
        }

        /// <summary>
        /// Returns the number of items on the heap
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Clears all items from the heap
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _count = 0;
            _itemIndex.Clear();
        }
    }
}
