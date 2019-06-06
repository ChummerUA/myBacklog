using Plugin.CloudFirestore.Attributes;
using Prism.Mvvm;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace myBacklog.Models
{
    public class ItemModel : BindableBase
    {
        #region Variables
        private string itemID;
        private string itemName;
        private string categoryID;
        private string stateID;
        private StateModel state;
        private int id;
        #endregion

        #region ID
        public string ItemID
        {
            get => itemID;
            set => SetProperty(ref itemID, value);
        }

        public string CategoryID
        {
            get => categoryID;
            set => SetProperty(ref categoryID, value);
        }

        public string StateID
        {
            get => categoryID;
            set => SetProperty(ref stateID, value);
        }

        public int ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        #endregion

        public string ItemName
        {
            get => itemName;
            set => SetProperty(ref itemName, value);
        }

        [Ignore]
        public StateModel State
        {
            get => state;
            set => SetProperty(ref state, value);
        }
    }
}
