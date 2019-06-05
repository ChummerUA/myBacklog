using myBacklog.Models;
using Prism.Mvvm;
using Prism.Navigation;
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
    public class CategoriesViewModel : BaseViewModel, INotifyPropertyChanged
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

        #region Property
        INavigationService NavigationService { get; set; }

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
        #endregion

        #region ICommand
        public ICommand UpdateCategoriesCommand { get; }
        public ICommand CreateCategoryCommand { get; }
        public ICommand OpenCategoryCommand { get; }
        #endregion

        public CategoriesViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            UpdateCategoriesCommand = new Command(async () => await UpdateCategoriesAsync());
            CreateCategoryCommand = new Command(async () => await CreateCategoryAsync());
            OpenCategoryCommand = new Command<CategoryModel>(async (param) => await OpenCategoryAsync(param));

            UpdateCategoriesCommand.Execute(null);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("IsUpdated"))
            {
                UpdateCategoriesCommand.Execute(null);
            }
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

        private async Task CreateCategoryAsync()
        {
            var parameters = new NavigationParameters();
            await NavigationService.NavigateAsync("SetCategoryPage", parameters);
        }

        private async Task OpenCategoryAsync(CategoryModel c)
        {
            var parameters = new NavigationParameters();
            parameters.Add("category", c);
            await NavigationService.NavigateAsync("ItemsPage", parameters);
        }
    }
}
