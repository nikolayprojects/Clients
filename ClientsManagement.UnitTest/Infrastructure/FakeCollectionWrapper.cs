using System.Collections.Generic;
using ClientsManagement.Util;

namespace ClientsManagement.UnitTest.Infrastructure
{
    class FakeCollectionWrapperFactory : ICollectionWrapperFactory
    {
        public ICollectionWrapper<T> Create<T>()
        {
            return new FakeCollectionWrapper<T>();
        }
    }

    class FakeCollectionWrapper<T> : ICollectionWrapper<T>
    {
        public IList<T> Collection { get; }

        public FakeCollectionWrapper()
        {
            Collection = new List<T>();
        }

        public void Add(T item)
        {
            Collection.Add(item);
        }

        public void Remove(T item)
        {
            Collection.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
        }
    }
}