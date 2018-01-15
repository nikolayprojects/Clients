using System;

namespace ClientsManagement.DTO
{
    public class ClientDTO : DTOBase
    {
        string inn;
        string name;
        ClientTypeDTO type;
        DateTime dateContract;
        string contacts;

        public int ID { get; set; }

        public string INN
        {
            get { return inn; }
            set
            {
                inn = value;
                OnPropertyChanged(nameof(INN));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ClientTypeDTO Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

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