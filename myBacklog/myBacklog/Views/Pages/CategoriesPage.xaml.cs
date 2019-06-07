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

		public CategoriesPage ()
		{
			InitializeComponent ();
            ViewModel = BindingContext as CategoriesViewModel;
        }

        private void OpenCategory(object sender, SelectedItemChangedEventArgs e) 
        {
            if(e.SelectedItem != null)
            {
                ViewModel.OpenCategoryCommand.Execute(e.SelectedItem as CategoryModel);
                CategoriesListView.SelectedItem = null;
            }
        }

        private void CategoriesListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var source = CategoriesListView.ItemsSource as ObservableCollection<CategoryModel>;
            if(e.ItemIndex >= source.Count - 10)
            {
                ViewModel.LoadMoreCategoriesCommand.Execute(null);
            }
        }
    }
}