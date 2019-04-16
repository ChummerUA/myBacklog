using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace myBacklog.ViewModels
{
    public class CategoriesViewModel : INotifyPropertyChanged
    {
        ObservableCollection<CategoryModel> categories;

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

        public ObservableCollection<CategoryModel> Categories
        {
            get
            {
                return categories;
            }
            set
            {
                if(categories != value)
                {
                    categories = value;
                    OnPropertyChanged("Categories");
                }
            }
        }

        public ICommand UpdateCategoriesCommand { get; }

        public CategoriesViewModel()
        {
            UpdateCategoriesCommand = new Command(async () => await UpdateCategoriesAsync());
        }

        private async Task UpdateCategoriesAsync()
        {
            var categoriesList = await App.Database.GetCategoriesAsync();

            if(categoriesList == null)
            {
                categoriesList = new List<CategoryModel>();
            }

            Categories = new ObservableCollection<CategoryModel>(categoriesList);
        }
    }
}
