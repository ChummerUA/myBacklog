using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myBacklog.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoriesPage : ContentPage
	{
        public CategoriesViewModel ViewModel { get; set; }

        public ICommand CreateCategoryCommand { get; }

		public CategoriesPage ()
		{
			InitializeComponent ();

            ViewModel = new CategoriesViewModel();
            BindingContext = ViewModel;

            CreateCategoryCommand = new Command(async () => await CreateCategoryAsync());

            AddCategoryButton.Command = CreateCategoryCommand;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.UpdateCategoriesCommand.Execute(null);
        }

        private async Task CreateCategoryAsync()
        {
            await Navigation.PushAsync(new SetCategoryPage(new SetCategoryViewModel()));
        }

        private async void OpenCategoryAsync(object sender, SelectedItemChangedEventArgs e) 
        {
            if(e.SelectedItem != null)
            {
                var category = e.SelectedItem as CategoryModel;
                (sender as ListView).SelectedItem = null;

                var viewModel = new ItemsViewModel(await App.Database.GetCategoryAsync((int)category.CategoryID));

                var page = new ItemsPage(viewModel);

                await Navigation.PushAsync(page);
            }
        }
	}
}