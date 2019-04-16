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

            if (category == null)
            {
                ViewModel = new SetCategoryViewModel();
            }
            else
            {
                ViewModel = new SetCategoryViewModel(category);
            }

            ConfirmCommand = new Command(async () => await ConfirmAsync());

            var item = Resources["ConfirmButton"] as ToolbarItem;
            item.Command = ConfirmCommand;

            ToolbarItems.Add(item);

            if (ViewModel.IsNewCategory)
            {
                CategoryNameEntry.Placeholder = "New category";
                Title = "New category";
            }
            else
            {
                Title = ViewModel.Category.CategoryName;
                ViewModel.SetStatesCommand.Execute(null);
            }

            CategoryNameEntry.ReturnCommand = ViewModel.SetCategoryNameCommand;

            BindingContext = ViewModel;
		}

        private async Task ConfirmAsync()
        {
            if (CanConfirm())
            {
                ViewModel.SaveCategoryCommand.Execute(null);
                await Navigation.PopAsync();
            }
        }

        private bool CanConfirm()
        {
            return ViewModel.SaveCategoryCommand.CanExecute(null);
        }

        #region NewStateEntry
        private void NewStateEntry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;

            if (ViewModel.CreateStateCommand.CanExecute(entry.Text))
            {
                ViewModel.CreateStateCommand.Execute(entry.Text);
                entry.Text = "";
            }
        }

        private void NewStateEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            
            if (ViewModel.CreateStateCommand.CanExecute(entry.Text))
            {
                ViewModel.CreateStateCommand.Execute(entry.Text);
                entry.Text = "";
            }
            else if(entry.Text != "")
            {
                entry.Focus();
            }
        }
        #endregion

        #region StateEntry
        private void StateEntry_Focused(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            var state = entry.BindingContext as StateModel;

            ViewModel.EditState = ViewModel.Category.States.FirstOrDefault(x => x.StateID == state.StateID);
        }

        private void StateEntry_Unfocused(object sender, EventArgs e)
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

        private void StateEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!entry.ReturnCommand.CanExecute(entry.BindingContext))
            {
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = Color.Default;
            }
        }
        #endregion

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

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!entry.ReturnCommand.CanExecute(entry.Text))
            {
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = Color.Default;
            }
        }
    }
}