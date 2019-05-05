using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace myBacklog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        #region ICommand
        public ICommand CategorySettingsCommand { get; }
        public ICommand NewItemCommand { get; }
        public ICommand CancelItemCommand { get; }
        public ICommand ConfirmItemCommand { get; }
        public ICommand StatesCommand { get; }
        #endregion

        public ItemsViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if (value != viewModel)
                {
                    viewModel = value;
                    OnPropertyChanged("ViewModel");
                }
            }
        }

        public ItemsPage(ItemsViewModel vm)
        {
            InitializeComponent();

            CategorySettingsCommand = new Command(async () => await CategorySettingsAsync());
            NewItemCommand = new Command(CreateNewItem);
            CancelItemCommand = new Command(CancelItem);
            ConfirmItemCommand = new Command(ConfirmItem);
            StatesCommand = new Command(ShowState);

            SaveItemButton.Command = ConfirmItemCommand;
            NewItemButton.Command = NewItemCommand;
            CancelButton.Command = CancelItemCommand;
            StatesButton.Command = StatesCommand;

            ViewModel = vm;
            BindingContext = ViewModel;

            SetToolbar();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.UpdateModelCommand.Execute(null);

            StatePicker.SelectedIndex = 0;
            StatePicker.SelectedIndexChanged += StatePicker_SelectedIndexChanged;

            SetItemStack.Layout(new Rectangle
            {
                Width = SetItemStack.Width,
                Height = SetItemStack.Height,
                X = SetItemStack.X,
                Y = Height
            });
        }

        protected override bool OnBackButtonPressed()
        {
            if (!BottomPanel.IsVisible)
            {
                HideSetItemStack();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetToolbar()
        {
            var settings = Resources["SettingsButton"] as ToolbarItem;
            settings.Command = CategorySettingsCommand;
            ToolbarItems.Add(settings);
        }

        private void HideToolbar()
        {
            ToolbarItems.Clear();
        }

        private void CancelItem()
        {
            HideSetItemStack();
        }

        private async Task CategorySettingsAsync()
        {
            var states = ViewModel.States.ToList();
            states.RemoveAt(0);

            var viewModel = new SetCategoryViewModel(ViewModel.Category, states);
            var page = new SetCategoryPage(viewModel);

            await Navigation.PushAsync(page);
        }

        private void CreateNewItem()
        {
            ViewModel.SetEditableItemCommand.Execute(null);
            SetItemHeader.Text = "New";
            ShowSetItemStack();
        }

        private void ConfirmItem()
        {
            if (!ViewModel.SaveItemCommand.CanExecute(null))
            {
                return;
            }

            ViewModel.SaveItemCommand.Execute(null);

            StatePicker.SelectedIndex = 0;

            HideSetItemStack();
        }

        private async void ShowSetItemStack()
        {
            HideToolbar();
            BottomPanel.IsVisible = false;
            await SetItemStack.LayoutTo(new Rectangle
            {
                Width = SetItemStack.Width,
                Height = SetItemStack.Height,
                X = SetItemStack.X,
                Y = SetItemStack.Y - SetItemStack.Height
            }, 100);
        }

        private async void HideSetItemStack()
        {
            BottomPanel.IsVisible = true;
            await SetItemStack.LayoutTo(new Rectangle
            {
                Width = SetItemStack.Width,
                Height = SetItemStack.Height,
                X = SetItemStack.X,
                Y = Height
            }, 100);

            SetToolbar();
            ViewModel.ClearEditableItemCommand.Execute(null);
            SetItemStatePicker.SelectedItem = null;
        }

        private void SetItemStatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (picker.SelectedItem == null || ViewModel.EditableItem == null)
            {
                return;
            }

            ViewModel.EditableItem.StateID = ViewModel.EditableItem.State.StateID;
        }

        private void StatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (!(picker.SelectedItem is StateModel state))
            {
                return;
            }

            ViewModel.ShowStateCommand.Execute(null);
        }

        private void ShowState()
        {
            StatePicker.Focus();
        }

        private void ItemNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!entry.ReturnCommand.CanExecute(null) && entry.Text != null && entry.Text != "")
            {
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = Color.Default;
            }
        }

        private void ItemsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            if(e.SelectedItem != null)
            {
                var item = e.SelectedItem as ItemModel;
                listView.SelectedItem = null;

                ViewModel.SetEditableItemCommand.Execute(item);
                SetItemHeader.Text = "Edit";

                ShowSetItemStack();
            }
        }

        private void ItemListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var listview = sender as ListView;
            var source = listview.ItemsSource as ObservableCollection<ItemModel>;
            if (source.IndexOf((e.Item as ItemModel)) >= source.Count - 15)
            {
                ViewModel.LoadMoreCommand.Execute(null);
            }
        }
    }
}