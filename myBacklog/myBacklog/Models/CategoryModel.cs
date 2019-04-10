using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace myBacklog.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        private int id;
        private string categoryName;
        private ObservableCollection<StateModel> states;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if(changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if(value != id)
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        public string CategoryName
        {
            get
            {
                return categoryName;
            }
            set
            {
                if(value != categoryName)
                {
                    categoryName = value;
                    OnPropertyChanged("CategoryName");
                }
            }
        }
        
        public ObservableCollection<StateModel> States
        {
            get
            {
                return states;
            }
            set
            {
                if(value != states)
                {
                    states = value;
                    OnPropertyChanged("States");
                }
            }
        }

    }
}
