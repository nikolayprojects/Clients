using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ClientsManagement.Util
{
    public class CustomBindingList<T> : BindingList<T>
    {
        bool isAdd;
        bool noRaiseDeletedChangeEvent;

        public event Func<CustomBindingListEventArgs<T>, Task> ChangeItems;

        public CustomBindingList(IList<T> list) : base(list)
        {
            noRaiseDeletedChangeEvent = false;
        }

        async Task<bool> OnChangeItems(T item, ItemsChangedType itemsChangedType)
        {
            var e = Volatile.Read(ref ChangeItems);

            if (e != null)
            {
                CustomBindingListEventArgs<T> arg = new CustomBindingListEventArgs<T>(item, itemsChangedType);

                await e(arg);

                return arg.Cancel;
            }

            return false;
        }

        protected override async void RemoveItem(int index)
        {
            if (noRaiseDeletedChangeEvent || !await OnChangeItems(this[index], ItemsChangedType.Deleted))
                base.RemoveItem(index);
        }

        protected override async void OnListChanged(ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        isAdd = true;
                    }
                    break;

                case ListChangedType.ItemChanged:
                    {
                        if (isAdd)
                        {
                            isAdd = false;

                            base.OnListChanged(e);

                            noRaiseDeletedChangeEvent = true;

                            if (await OnChangeItems(this[e.NewIndex], ItemsChangedType.Added))
                                RemoveItem(e.NewIndex);

                            noRaiseDeletedChangeEvent = false;

                            return;
                        }
                        else
                        {
                            await OnChangeItems(this[e.NewIndex], ItemsChangedType.Changed);
                        }
                    }
                    break;
            }

            base.OnListChanged(e);
        }
    }

    public class CustomBindingListEventArgs<T> : EventArgs
    {
        public T Item { get; }
        public ItemsChangedType ItemsChangedType { get; }
        public bool Cancel { get; set; }

        public CustomBindingListEventArgs(T item, ItemsChangedType itemsChangedType)
        {
            Item = item;
            ItemsChangedType = itemsChangedType;
        }
    }

    public enum ItemsChangedType { Added, Deleted, Changed }
}