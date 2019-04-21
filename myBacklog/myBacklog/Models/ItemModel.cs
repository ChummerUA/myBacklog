using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace myBacklog.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        #region Variables
        int itemID;
        string itemName;
        int categoryID;
        int stateID;
        NamedColor namedColor;
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
        public int ItemID
        {
            get
            {
                return itemID;
            }
            set
            {
                if(itemID != value)
                {
                    itemID = value;
                    OnPropertyChanged("ItemID");
                }
            }
        }

        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                if(itemName != value)
                {
                    itemName = value;
                    OnPropertyChanged("ItemName");
                }
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

        public int StateID
        {
            get
            {
                return stateID;
            }
            set
            {
                if(stateID != value)
                {
                    stateID = value;
                    OnPropertyChanged("StateID");
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
                if(namedColor != value)
                {
                    namedColor = value;
                    OnPropertyChanged("NamedColor");
                }
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
    }
}
