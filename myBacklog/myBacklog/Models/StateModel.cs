using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SQLite;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace myBacklog.Models
{
    public class StateModel : INotifyPropertyChanged
    {
        #region Variables
        int stateID;
        string stateName;
        NamedColor color;
        ObservableCollection<ItemModel> items;
        int categoryID;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        [PrimaryKey, AutoIncrement]
        public int StateID
        {
            get
            {
                return stateID;
            }
            set
            {
                if (value != stateID)
                {
                    stateID = value;
                    OnPropertyChanged("StateID");
                }
            }
        }

        public string StateName
        {
            get
            {
                return stateName;
            }
            set
            {
                if (value != stateName)
                {
                    stateName = value;
                    OnPropertyChanged("StateName");
                }
            }
        }

        [Ignore]
        public NamedColor NamedColor { get; set; }

        public string ColorName
        {
            get
            {
                return NamedColor.Name;
            }
            set
            {
                NamedColor = NamedColor.All.FirstOrDefault(x => x.Name == value);
            }
        }

        public Color Color
        {
            get
            {
                return NamedColor.Color;
            }
        }

        [Ignore]
        public ObservableCollection<ItemModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                if (value != items)
                {
                    items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        [Ignore]
        public bool IsShown
        {
            get
            {
                if(Items.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public int CategoryID
        {
            get
            {
                return categoryID;
            }
            set
            {
                if(categoryID != value)
                {
                    categoryID = value;
                    OnPropertyChanged("CategoryID");
                }
            }
        }
    }
}
