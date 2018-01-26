using System;
using System.ComponentModel.DataAnnotations;
using MugenMvvmToolkit.Models;

namespace ClientsManagement.DTO
{
    public class ClientDTO : NotifyPropertyChangedBase
    {
        string inn;
        string name;
        ClientTypeDTO type;
        DateTime dateContract;
        string contacts;

        public int ID { get; set; }

        [Required, Display(Name = "ИНН"), StringLength(12, MinimumLength = 12)]
        public string INN
        {
            get { return inn; }
            set
            {
                inn = value;
                OnPropertyChanged(nameof(INN));
            }
        }

        [Required, Display(Name = "Наименование"), StringLength(150, MinimumLength = 1)]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [Required, Display(Name = "Тип клиента")]
        public ClientTypeDTO Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        [Required, Display(Name = "Дата заключения контракта")]
        public DateTime DateContract
        {
            get { return dateContract; }
            set
            {
                dateContract = value;
                OnPropertyChanged(nameof(DateContract));
            }
        }

        public int PartnershipDuration
        {
            get { return (DateTime.Now - DateContract).Days; }
        }

        [Required, Display(Name = "Контактные данные")]
        public string Contacts
        {
            get { return contacts; }
            set
            {
                contacts = value;
                OnPropertyChanged(nameof(Contacts));
            }
        }
    }
}