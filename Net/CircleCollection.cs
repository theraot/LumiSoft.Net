#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Collections.Generic;

namespace LumiSoft.Net
{
    /// <summary>
    /// Circle collection. Elements will be circled clockwise.
    /// </summary>
    public class CircleCollection<T>
    {
        private int _index;
        private readonly List<T> _items;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CircleCollection()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets item at the specified index.
        /// </summary>
        /// <param name="index">Item zero based index.</param>
        /// <returns>Returns item at the specified index.</returns>
        public T this[int index] => _items[index];

        /// <summary>
        /// Adds specified items to the collection.
        /// </summary>
        /// <param name="items">Items to add.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>items</b> is null.</exception>
        public void Add(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds specified item to the collection.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>item</b> is null.</exception>
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Add(item);

            // Reset loop index.
            _index = 0;
        }

        /// <summary>
        /// Clears all items from collection.
        /// </summary>
        public void Clear()
        {
            _items.Clear();

            // Reset loop index.
            _index = 0;
        }

        /// <summary>
        /// Gets if the collection contain the specified item.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Returns true if the collection contain the specified item, otherwise false.</returns>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Gets next item from the collection. This method is thread-safe.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is raised when there is no items in the collection.</exception>
        public T Next()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("There is no items in the collection.");
            }

            lock (_items)
            {
                var item = _items[_index];

                _index++;
                if (_index >= _items.Count)
                {
                    _index = 0;
                }

                return item;
            }
        }

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>item</b> is null.</exception>
        public void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items.Remove(item);

            // Reset loop index.
            _index = 0;
        }

        /// <summary>
        /// Copies all elements to new array, all elements will be in order they added. This method is thread-safe.
        /// </summary>
        /// <returns>Returns elements in a new array.</returns>
        public T[] ToArray()
        {
            lock (_items)
            {
                return _items.ToArray();
            }
        }

        /// <summary>
        /// Copies all elements to new array, all elements will be in current circle order. This method is thread-safe.
        /// </summary>
        /// <returns>Returns elements in a new array.</returns>
        public T[] ToCurrentOrderArray()
        {
            lock (_items)
            {
                var index = _index;
                var result = new T[_items.Count];
                for (var i = 0; i < _items.Count; i++)
                {
                    result[i] = _items[index];

                    index++;
                    if (index >= _items.Count)
                    {
                        index = 0;
                    }
                }

                return result;
            }
        }
    }
}
