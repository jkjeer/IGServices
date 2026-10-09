using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace IGServices.Common.Types
{
    [DefaultProperty("items")]
    [CollectionDataContract(Name = "CardinalCollectionOf{0}", Namespace = "")]
    public class CardinalCollection<T> : IList<T>
    {
        private List<T> items;

        public CardinalCollection()
        {
            items = new List<T>();
        }

        public CardinalCollection(CardinalCollection<T> _items)
        {
            items = _items.ToList();
        }

        public virtual int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)items).IsReadOnly;
            }
        }

        public void Add(T item)
        {
            this.items.Add(item);
        }

        public T Get(int index) { return items[index]; }

        public T this[int index]
        {
            get
            {
                return items[index];
            }

            set
            {
                items[index] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int counter = 0;

            while (counter < Count)
            {
                yield return items[counter];
                counter++;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            int counter = 0;

            while (counter < Count)
            {
                yield return items[counter];
                counter++;
            }
        }

        public void Clear()
        {
            ((ICollection<T>)items).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)items).Remove(item);
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)items).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)items).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)items).RemoveAt(index);
        }
    }
}
