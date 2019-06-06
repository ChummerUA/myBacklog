using Plugin.CloudFirestore.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace myBacklog.Models
{
    public class CategoryModel : BindableBase
    {
        #region Variables
        private string categoryId;
        private string categoryName;
        private ObservableCollection<ItemModel> items;
        private int id;
        #endregion

        #region ID
        public string CategoryID
        {
            get => categoryId;
            set => SetProperty(ref categoryId, value);
        }

        public int ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        #endregion

        public string CategoryName
        {
            get => categoryName;
            set => SetProperty(ref categoryName, value);
        }
        
        [Ignored]
        public ObservableCollection<ItemModel> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
    }
}
