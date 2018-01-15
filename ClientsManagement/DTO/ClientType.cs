namespace ClientsManagement.DTO
{
    public class ClientTypeDTO : DTOBase
    {
        string name;

        public int ID { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}