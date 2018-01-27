using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace ClientsManagement.Util
{
    public interface ICollectionWrapper<T>
    {
        IList<T> Collection { get; }
        void Add(T item);
        void Remove(T item);
        void RemoveAt(int index);
    }

    public interface ICollectionWrapperFactory
    {
        ICollectionWrapper<T> Create<T>();
    }

    public class CollectionWrapperFactory : ICollectionWrapperFactory
    {
        public ICollectionWrapper<T> Create<T>()
        {
            return new CollectionWrapper<T>();
        }
    }

    public class CollectionWrapper<T> : ICollectionWrapper<T>
    {
        readonly IList<T> collection;
        readonly Dispatcher dispatcher;

        public IList<T> Collection => collection;

        public CollectionWrapper()
        {
            collection = new ObservableCollection<T>();
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Add(T item)
        {
            dispatcher.Invoke(() => collection.Add(item));
        }

        public void RemoveAt(int index)
        {
            dispatcher.Invoke(() => collection.RemoveAt(index));
        }

        public void Remove(T item)
        {
            dispatcher.Invoke(() => collection.Remove(item));
        }
    }
}
