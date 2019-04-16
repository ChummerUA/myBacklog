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
        public ICommand SetCategoryCommand { get; }

		public CategoriesPage ()
		{
			InitializeComponent ();

            ViewModel = new CategoriesViewModel();
            BindingContext = ViewModel;

            CreateCategoryCommand = new Command(async () => await CreateCategoryAsync());
            SetCategoryCommand = new Command<CategoryModel>(async (parameter) => await SetCategoryAsync(parameter));

            AddCategoryButton.Command = CreateCategoryCommand;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.UpdateCategoriesCommand.Execute(null);
        }

        private async Task CreateCategoryAsync()
        {
            await Navigation.PushAsync(new SetCategoryPage(null));
        }

        private async Task SetCategoryAsync(CategoryModel model)
        {
            await Navigation.PushAsync(new SetCategoryPage(model));
        }
	}
}