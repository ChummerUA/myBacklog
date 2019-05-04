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
        int? itemID;
        string itemName;
        int? categoryID;
        int? stateID;
        StateModel state;
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
        public int? ItemID
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

        public int? StateID
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
        public StateModel State
        {
            get
            {
                return state;
            }
            set
            {
                if(value != state)
                {
                    state = value;
                    OnPropertyChanged("State");
                }
            }
        }
    }
}
