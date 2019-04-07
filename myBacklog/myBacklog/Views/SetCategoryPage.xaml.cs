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
	public partial class SetCategoryPage : ContentPage
	{
        public ICommand ConfirmCommand { get; }

        public SetCategoryViewModel ViewModel { get; set; }

        public SetCategoryPage(CategoryModel category = null)
		{
			InitializeComponent ();

            if(category != null)
            {
                ViewModel = new SetCategoryViewModel(category);
            }
            else
            {
                ViewModel = new SetCategoryViewModel();
            }
            BindingContext = ViewModel;

            ConfirmCommand = new Command(ConfirmCategory);

            ToolbarItems.Add(Resources["ConfirmButton"] as ToolbarItem);
		}

        private void ColorBox_Tapped(object sender, EventArgs e)
        {
            MainContent.IsVisible = false;
            ColorsListView.IsVisible = true;

            var selectedColor = ViewModel.SelectedState.Color;
            var color = ViewModel.Colors.FirstOrDefault(x => x.Color.A == selectedColor.A
            && x.Color.B == selectedColor.B
            && x.Color.G == selectedColor.G
            && x.Color.R == selectedColor.R);
            var gray = System.Drawing.Color.Gray;

            if (!Color.Equals(color.Color, gray))
            {
                ColorsListView.ScrollTo(color, ScrollToPosition.Center, false);
            }
            Title = "Choose color";
            ToolbarItems.Remove(Resources["ConfirmButton"] as ToolbarItem);
        }

        private void ColorsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                var color = e.SelectedItem as NamedColor;

                ViewModel.SelectedState.Color = color.Color;

                MainContent.IsVisible = true;
                ColorsListView.IsVisible = false;
                
                Title = ViewModel.CategoryName;

                ViewModel.States[ViewModel.SelectedState.ID] = ViewModel.SelectedState;

                ToolbarItems.Add(Resources["ConfirmButton"] as ToolbarItem);
            }
            ColorsListView.SelectedItem = null;
        }

        private void ConfirmCategory()
        {
            //To do
        }

        protected override bool OnBackButtonPressed()
        {
            if(Title == "Choose color")
            {
                MainContent.IsVisible = true;
                ColorsListView.IsVisible = false;

                ViewModel.SelectedState = null;

                if (ViewModel.CategoryName != "")
                {
                    Title = "New category";
                }
                else
                {
                    Title = ViewModel.CategoryName;
                }

                ToolbarItems.Add(Resources["ConfirmButton"] as ToolbarItem);

                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}