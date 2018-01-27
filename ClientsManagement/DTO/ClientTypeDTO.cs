using System.ComponentModel.DataAnnotations;
using MugenMvvmToolkit.Models;

namespace ClientsManagement.DTO
{
    public class ClientTypeDTO : NotifyPropertyChangedBase
    {
        string name;

        public int ID { get; set; }

        [Required]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ClientTypeDTO() { }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            ClientTypeDTO clientTypeDTO = obj as ClientTypeDTO;

            if (obj == null || clientTypeDTO == null)
                return false;

            return ID == clientTypeDTO.ID && name == clientTypeDTO.name;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ (Name != null ? Name.GetHashCode() : 0);
        }
    }
}