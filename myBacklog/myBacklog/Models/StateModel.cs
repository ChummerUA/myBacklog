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
        int? stateID;
        string stateName;
        NamedColor namedColor;
        int? categoryID;
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
        public int? StateID
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
        public NamedColor NamedColor
        {
            get
            {
                return namedColor;
            }
            set
            {
                if(value != namedColor)
                {
                    namedColor = value;
                    OnPropertyChanged("NamedColor");
                    OnPropertyChanged("Color");
                }
            }
        }

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

        [Ignore]
        public Color Color
        {
            get
            {
                return NamedColor.Color;
            }
        }

        public int? CategoryID
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
