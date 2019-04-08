using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
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

		public CategoriesPage (CategoriesViewModel viewModel)
		{
			InitializeComponent ();

            ViewModel = viewModel;
            BindingContext = ViewModel;

            CreateCategoryCommand = new Command(async () => await CreateCategoryAsync(), () => true);

            AddCategoryButton.Command = CreateCategoryCommand;
        }

        private async Task CreateCategoryAsync()
        {
            await Navigation.PushAsync(new SetCategoryPage());
        }

        private async void CreateCategory_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetCategoryPage());
        }
	}
}