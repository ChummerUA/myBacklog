using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace myBacklog.ViewModels
{
    public class ItemsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CategoryModel Category { get; set; }

        public ItemsViewModel(CategoryModel category)
        {
            Category = category;
        }
    }
}
