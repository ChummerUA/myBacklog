using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	public partial class SetCategoryPage : ContentPage, INotifyPropertyChanged
	{
        SetCategoryViewModel viewModel;

        public ICommand ConfirmCommand { get; }

        public SetCategoryViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if(value != viewModel)
                {
                    viewModel = value;
                    OnPropertyChanged("ViewModel");
                }
            }
        }

        public SetCategoryPage(SetCategoryViewModel vm)
		{
			InitializeComponent ();

            ViewModel = vm;
            BindingContext = ViewModel;

            if (ViewModel.IsNewCategory)
            {
                CategoryNameEntry.Placeholder = "New category";
                Title = "New category";
            }
            else
            {
                ViewModel.GetCategoryCommand.Execute(null);
            }

            ConfirmCommand = new Command(execute: async () => await SaveAsync());

            if (ViewModel.IsNewCategory)
            {
                var item = Resources["ToolbarItem"] as ToolbarItem;
                item.Command = ConfirmCommand;

                ToolbarItems.Add(item);
            }
        }

        private async Task SaveAsync()
        {
            if (ViewModel.SaveCategoryCommand.CanExecute(null))
            {
                ViewModel.SaveCategoryCommand.Execute(null);
                await Navigation.PopAsync();
            }
        }

        #region NewStateEntry
        private void NewStateEntry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;

            if (entry.ReturnCommand.CanExecute(entry.Text))
            {
                entry.Text = "";
            }
        }

        private void NewStateEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            
            entry.ReturnCommand.Execute(entry.Text);
            entry.Text = "";
        }

        private void NewStateEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if(entry.ReturnCommand == null)
            {
                return;
            }

            if (!entry.ReturnCommand.CanExecute(e.NewTextValue))
            {
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = Color.Default;
            }
        }
        #endregion

        #region StateEntry
        private void StateEntry_Focused(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            var state = entry.BindingContext as StateModel;

            ViewModel.EditState = new StateModel
            {
                StateName = state.StateName,
                NamedColor = state.NamedColor,
            };
            if(state.CategoryID != null)
            {
                ViewModel.EditState.CategoryID = state.CategoryID;
            }
            if(state.StateID != null)
            {
                ViewModel.EditState.StateID = state.StateID;
            }
        }

        private void StateEntry_Unfocused(object sender, EventArgs e)
        {
            var entry = sender as Entry;

            if (entry.ReturnCommand.CanExecute(entry.BindingContext))
            {
                entry.ReturnCommand.Execute(entry.BindingContext);
            }
            else
            {
                ViewModel.CancelChangesCommand.Execute(null);
            }
        }

        private void StateEntry_Completed(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (!entry.ReturnCommand.CanExecute(entry.BindingContext))
            {
                entry.Focus();
            }
        }

        private void StateEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if(entry.ReturnCommand == null)
            {
                return;
            }

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

        #region Category
        private void CategoryNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if(entry.ReturnCommand == null)
            {
                return;
            }

            if (!entry.ReturnCommand.CanExecute(null))
            {
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = Color.Default;
            }
        }

        private void CategoryNameEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;

            if (!entry.ReturnCommand.CanExecute(null))
            {
                ViewModel.CancelChangesCommand.Execute(null);
            }
        }
        #endregion

        private async void ColorBox_Tapped(object sender, EventArgs e)
        {
            var colors = new List<System.Drawing.Color>();
            foreach(var state in ViewModel.States)
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

            var selectedColor = ViewModel.EditState.NamedColor;

            var viewModel = new SetColorViewModel(namedColors, selectedColor);

            await Navigation.PushAsync(new SetColorPage(viewModel));
        }
    }
}