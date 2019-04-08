using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace myBacklog.ViewModels
{
    public class CategoriesViewModel : INotifyPropertyChanged
    {
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

        public ObservableCollection<CategoryModel> Categories { get; set; }

        public CategoriesViewModel(ObservableCollection<CategoryModel> categories = null)
        {
            if(categories == null)
            {
                categories = new ObservableCollection<CategoryModel>();
            }
            Categories = categories;
        }
    }
}
