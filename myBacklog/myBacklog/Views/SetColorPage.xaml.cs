using myBacklog.Models;
using myBacklog.ViewModels;
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
    public partial class SetColorPage : ContentPage
    {
        public SetColorViewModel ViewModel { get; set; }

        public SetColorPage(SetColorViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var selectedColor = ViewModel.SelectedColor.Color;

            if (ViewModel.SelectedColor != null)
            {
                var namedColor = ViewModel.Colors.FirstOrDefault(x => x.Color.A == selectedColor.A
                    && x.Color.B == selectedColor.B
                    && x.Color.G == selectedColor.G
                    && x.Color.R == selectedColor.R);
                var gray = System.Drawing.Color.Gray;

                if (!Color.Equals(namedColor.Color, gray))
                {
                    ColorsListView.ScrollTo(namedColor, ScrollToPosition.Center, false);
                }
            }
        }

        private async void ColorsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                var color = (e.SelectedItem as NamedColor).Color;
                var page = Navigation.NavigationStack.FirstOrDefault(x => x.GetType() == typeof(SetCategoryPage)) as SetCategoryPage;
                
                page.ViewModel.ConfirmColorCommand.Execute(color);
                
                await Navigation.PopAsync();
            }
        }
    }
}