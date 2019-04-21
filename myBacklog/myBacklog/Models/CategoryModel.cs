using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace myBacklog.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        #region Variables
        private int id;
        private string categoryName;
        private ObservableCollection<StateModel> states;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if(changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        [PrimaryKey, AutoIncrement]
        public int CategoryID
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
        
        [Ignore]
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
