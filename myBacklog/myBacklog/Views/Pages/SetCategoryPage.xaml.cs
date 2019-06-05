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
        public ICommand DeleteCommand { get; }

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

        public SetCategoryPage()
        {
            InitializeComponent();

            ViewModel = BindingContext as SetCategoryViewModel;

            if (ViewModel.IsNewCategory)
            {
                ConfirmCommand = ViewModel.SaveCategoryCommand;

                var item = Resources["ConfirmCategory"] as ToolbarItem;
                item.Command = ConfirmCommand;

                ToolbarItems.Add(item);
            }
            else
            {
                DeleteCommand = new Command(execute: async () => await DeleteAsync());

                var item = Resources["DeleteCategory"] as ToolbarItem;
                item.Command = DeleteCommand;

                ToolbarItems.Add(item);
            }
        }

        private async Task DeleteAsync()
        {
            bool delete = await DisplayAlert("Delete", "If you delete category, all items from it will be lost", "Yes", "No");

            if (delete)
            {
                ViewModel.DeleteCategoryCommand.Execute(null);
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

            if (!entry.ReturnCommand.CanExecute(e.NewTextValue) && e.NewTextValue != "")
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
    }
}