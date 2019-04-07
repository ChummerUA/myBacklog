using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myBacklog.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SetCategoryPage : ContentPage
	{
		public SetCategoryPage ()
		{
			InitializeComponent ();
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
        }

        private void ColorsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                var color = e.SelectedItem as NamedColor;

                ViewModel.SelectedState.Color = color.Color;

                MainContent.IsVisible = true;
                ColorsListView.IsVisible = false;

                if(ViewModel.CategoryName != "")
                {
                    Title = "New category";
                }
                else
                {
                    Title = ViewModel.CategoryName;
                }

                ViewModel.States[ViewModel.SelectedState.ID] = ViewModel.SelectedState;
                
                //ViewModel.OnPropertyChanged("States");
            }
            ColorsListView.SelectedItem = null;
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

                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}