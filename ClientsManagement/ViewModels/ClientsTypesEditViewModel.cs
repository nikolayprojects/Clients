using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;

namespace ClientsManagement.ViewModels
{
    public class ClientsTypesEditViewModel : ViewModelBase
    {
        bool isBusy;

        ClientsModel clientsModel;
        CustomBindingList<ClientTypeDTO> bindingList;

        public CustomBindingList<ClientTypeDTO> ClientsTypes => bindingList;

        public void Initialize(ClientsModel clientsModel)
        {
            bindingList = new CustomBindingList<ClientTypeDTO>(clientsModel.ClientsTypesList);
            bindingList.ChangeItems += BindingList_ChangeItems;

            this.clientsModel = clientsModel;
        }

        private async Task BindingList_ChangeItems(CustomBindingListEventArgs<ClientTypeDTO> e)
        {
            IBusyToken token = null;

            try
            {
                token = BeginBusy();

                switch (e.ItemsChangedType)
                {
                    case ItemsChangedType.Added:
                        await clientsModel.AddClientTypeAsync(e.Item);
                        break;
                    case ItemsChangedType.Changed:
                        await clientsModel.ChangedClientTypeAsync(e.Item);
                        break;
                    case ItemsChangedType.Deleted:
                        await clientsModel.RemoveClientTypeAsync(e.Item);
                        break;
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
            finally
            {
                token.Dispose();
            }
        }
    }

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