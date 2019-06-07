using myBacklog.Models;
using myBacklog.Services;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public ICommand RefreshCategoriesCommand { get; }
        public ICommand LoadMoreCategoriesCommand { get; }
        public ICommand CreateCategoryCommand { get; }
        public ICommand OpenCategoryCommand { get; }
        #endregion

        public CategoriesViewModel(INavigationService navigationService, IDialog dialogService, IFirebase databaseService) : base(navigationService, dialogService, databaseService)
        {
            RefreshCategoriesCommand = new Command(async () => await RefreshCategoriesAsync());
            LoadMoreCategoriesCommand = new Command(async () => await LoadMoreCategoriesAsync());
            CreateCategoryCommand = new Command(async () => await CreateCategoryAsync());
            OpenCategoryCommand = new Command<CategoryModel>(async (param) => await OpenCategoryAsync(param));

            RefreshCategoriesCommand.Execute(null);
        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            await DialogService.PopAsync();
            if (parameters.ContainsKey("IsUpdated"))
            {
                RefreshCategoriesCommand.Execute(null);
            }
        }

        private async Task RefreshCategoriesAsync()
        {
            await DialogService.DisplayPopupAsync();
            var categoriesList = await FirebaseService.GetCategoriesAsync(20, null);

            if(categoriesList == null)
            {
                categoriesList = new List<CategoryModel>();
            }

            Categories = new ObservableCollection<CategoryModel>(categoriesList);
            await DialogService.PopAsync();
        }

        private async Task LoadMoreCategoriesAsync()
        {
            if (!await CanLoadMoreAsync())
            {
                return;
            }

            var categoriesList = await FirebaseService.GetCategoriesAsync(10, Categories.Last().ID);

            if(categoriesList == null)
            {
                return;
            }

            foreach(var category in categoriesList)
            {
                Categories.Add(category);
            }
        }

        private async Task CreateCategoryAsync()
        {
            await DialogService.DisplayPopupAsync();
            var parameters = new NavigationParameters();
            await NavigationService.NavigateAsync("SetCategoryPage", parameters);
            await DialogService.PopAsync();
        }

        private async Task OpenCategoryAsync(CategoryModel c)
        {
            var parameters = new NavigationParameters();
            parameters.Add("category", c);
            await NavigationService.NavigateAsync("ItemsPage", parameters);
        }

        private async Task<bool> CanLoadMoreAsync()
        {
            if(Categories == null)
            {
                return false;
            }
            if(Categories.Count == 0)
            {
                return false;
            }

            if(Categories.Count == await FirebaseService.GetCategoriesCountAsync())
            {
                return false;
            }

            return true;
        }
    }
}
