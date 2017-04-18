using System;
using System.Collections;
using System.Collections.Generic;
using SpotifyPlaylistMixer.Converter;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class CustomList<T> : ICollection<T>, IComparable<T>
    {
        private readonly ICollection<T> _items;

        public CustomList()
        {
            _items = new List<T>();
        }

        public CustomList(ICollection<T> collection)
        {
            _items = collection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public int CompareTo(T other)
        {
            var otherValue = ListStringToStringConverter.ConnectedString(other);
            var baseValue = ListStringToStringConverter.ConnectedString(_items);
            return string.Compare(baseValue, otherValue, StringComparison.CurrentCulture);
        }

        public void AddRange(ICollection<T> collection)
        {
            foreach (var item in collection)
            {
                _items.Add(item);
            }
        }

        public override string ToString()
        {
            var baseValue = ListStringToStringConverter.ConnectedString(_items);
            return baseValue;
        }
    }
}