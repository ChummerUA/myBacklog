using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            if (ViewModel.IsNewCategory)
            {
                CategoryNameEntry.Placeholder = "New category";
                Title = "New category";
                ToolbarItems.Add(Resources["ConfirmButton"] as ToolbarItem);
            }
            else
            {
                Title = ViewModel.Category.CategoryName;
            }

            BindingContext = ViewModel;

            ConfirmCommand = new Command(async () => await ConfirmCategoryAsync());

            var item = Resources["ConfirmButton"] as ToolbarItem;
            item.Command = ConfirmCommand;
            Resources["ConfirmButton"] = item;
		}
        
        private async Task ConfirmCategoryAsync()
        {
            var category = ViewModel.Category;

            Regex regex = new Regex(@"[\w\s]+");
            if (!regex.Match(category.CategoryName).Success)
            {
                return;
            }

            if(category.States.Count == 0)
            {
                return;
            }

            var categoriesPage = Navigation.NavigationStack[0] as CategoriesPage;

            if (ViewModel.IsNewCategory)
            {
                if(categoriesPage.ViewModel.Categories.FirstOrDefault(x => x.CategoryName == category.CategoryName) != null)
                {
                    return;
                }

                categoriesPage.ViewModel.Categories.Add(category);
            }
            else
            {
                if(categoriesPage.ViewModel.Categories.FirstOrDefault(x => x.CategoryName == category.CategoryName && x.ID != category.ID) != null)
                {
                    return;
                }

                categoriesPage.ViewModel.Categories[category.ID] = category;
            }
            await Navigation.PopAsync();
        }

        private void NewStateEntry_Completed(object sender, EventArgs e)
        {
            NewStateEntry.Text = "";
        }

        private void NewStateEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (ViewModel.NewStateCommand.CanExecute(entry.Text))
            {
                ViewModel.NewStateCommand.Execute(entry.Text);
                entry.Text = "";
            }
            else if(entry.Text != "")
            {
                entry.Focus();
            }
        }

        private void StateEntry_Focused(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            var state = entry.BindingContext as StateModel;

            ViewModel.EditState = ViewModel.Category.States.FirstOrDefault(x => x.ID == state.ID);
        }

        private void StateEntry_Unfocused(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (ViewModel.SetStateNameCommand.CanExecute(entry.Text))
            {
                ViewModel.EditState = null;
            }
            else
            {
                entry.Focus();
            }
        }

        private void StateEntry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (ViewModel.SetStateNameCommand.CanExecute(entry.Text))
            {
                ViewModel.SetStateNameCommand.Execute(entry.Text);
            }
            else
            {
                entry.Focus();
            }
        }

        private async void ColorBox_Tapped(object sender, EventArgs e)
        {
            var colors = new List<System.Drawing.Color>();
            foreach(var state in ViewModel.Category.States)
            {
                colors.Add(state.Color);
            }

            var namedColors = new ObservableCollection<NamedColor>();
            foreach(var color in colors)
            {
                namedColors.Add(NamedColor.All.FirstOrDefault(x => x.Color.A == color.A &&
                x.Color.B == color.B &&
                x.Color.G == color.B &&
                x.Color.R == color.R));
            }

            var selectedColor = ViewModel.EditState.Color;
            var namedColor = NamedColor.All.FirstOrDefault(x => x.Color.A == selectedColor.A &&
            x.Color.B == selectedColor.B &&
            x.Color.G == selectedColor.G && 
            x.Color.R == selectedColor.R);

            var viewModel = new SetColorViewModel(namedColors, namedColor);

            await Navigation.PushAsync(new SetColorPage(viewModel));
        }
    }
}