using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myBacklog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        #region PropertyChanged
        public PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        ItemsViewModel viewModel;

        public ICommand CategorySettingsCommand { get; }
        public ICommand NewItemCommand { get; }
        public ICommand CancelItemCommand { get; }

        public ItemsViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if (value != viewModel)
                {
                    viewModel = value;
                    OnPropertyChanged("ViewModel");
                }
            }
        }

        public ItemsPage(ItemsViewModel viewModel)
        {
            InitializeComponent();

            //CategorySettingsCommand = new Command(async () => await CategorySettingsAsync());
            //NewItemCommand = new Command(CreateNewItem);
            //CancelItemCommand = new Command(CancelItem);

            //NewItemButton.Command = NewItemCommand;
            //ViewModel = viewModel;

            //Title = ViewModel.Category.CategoryName;
            //BindingContext = ViewModel;
            //SetToolbar();
        }

        //private void SetToolbar()
        //{
        //    var search = Resources["SearchButton"] as ToolbarItem;

        //    var settings = Resources["SettingsButton"] as ToolbarItem;
        //    settings.Command = CategorySettingsCommand;

        //    ToolbarItems.Add(search);
        //    ToolbarItems.Add(settings);
        //}

        //private void HideToolbar()
        //{
        //    ToolbarItems.Clear();
        //}

        //private void CancelItem()
        //{
        //    SetToolbar();
        //}

        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //}

        //private async Task CategorySettingsAsync()
        //{
        //    var viewModel = new SetCategoryViewModel(ViewModel.Category);
        //    var page = new SetCategoryPage(viewModel);

        //    await Navigation.PushAsync(page);
        //}

        //private void CreateNewItem()
        //{
        //    NewItemStack.IsVisible = true;
        //    NewItemButton.IsVisible = false;
        //    HideToolbar();
        //}
    }
}