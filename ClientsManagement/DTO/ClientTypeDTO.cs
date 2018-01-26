﻿using System.ComponentModel.DataAnnotations;
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
    }
}