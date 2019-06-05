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
        #region variables
        ItemsViewModel viewModel;
        #endregion

        #region ICommand
        public ICommand CategorySettingsCommand { get; }
        public ICommand NewItemPanelCommand { get; }
        public ICommand SearchItemPanelCommand { get; }
        public ICommand HidePanelCommand { get; }
        public ICommand StatesCommand { get; }
        #endregion

        #region Properties
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
        #endregion

        public ItemsPage()
        {
            InitializeComponent();

            ViewModel = BindingContext as ItemsViewModel;

            NewItemPanelCommand = new Command(ShowNewItemPanel);
            SearchItemPanelCommand = new Command(ShowSearchItemPanel);
            HidePanelCommand = new Command(() => HideBottomPanel());
            StatesCommand = new Command(ShowState);

            NewItemButton.Command = NewItemPanelCommand;
            SearchButton.Command = SearchItemPanelCommand;
            StatesButton.Command = StatesCommand;

            SetToolbar();
        }

        #region override Page events
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            StatePicker.SelectedIndex = 0;
            StatePicker.SelectedIndexChanged += StatePicker_SelectedIndexChanged;

            if (BottomPanel.IsVisible)
            {
                BottomPanel.MinimumHeightRequest = BottomPanel.Height;
                ButtonsPanel.MinimumHeightRequest = ButtonsPanel.Height;

                await BottomPanel.Hide(0);

                NewItemStack.IsVisible = false;
                EditItemStack.IsVisible = false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (!ButtonsPanel.IsVisible)
            {
                HideBottomPanel();
                return true;
            }
            else if(Title == "Search")
            {
                Title = ViewModel.Category.CategoryName;
                ViewModel.ResetSearchItemCommand.Execute(null);
                ViewModel.ShowItemsCommand.Execute(null);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDisappearing()
        {
            if (!ButtonsPanel.IsVisible)
            {
                HideBottomPanel();
            }
            base.OnDisappearing();
        }
        #endregion

        private async Task CategorySettingsAsync()
        {
            var states = ViewModel.States.ToList();
            states.RemoveAt(0);

            //var viewModel = new SetCategoryViewModel(ViewModel.Category, states);

            //await Navigation.PushAsync(page);
        }

        #region Toolbar
        private void SetToolbar()
        {
            var settings = Resources["SettingsButton"] as ToolbarItem;
            ToolbarItems.Add(settings);
        }

        private void HideToolbar()
        {
            ToolbarItems.Clear();
        }
        #endregion

        #region Bottom panel
        private void ShowNewItemPanel()
        {
            ShowBottomPanel(NewItemStack);
            BottomHeader.Text = "New";
        }

        private void ShowSearchItemPanel()
        {
            ShowBottomPanel(SearchItemStack);
            SearchItemStatePicker.SelectedIndex = 0;
            BottomHeader.Text = "Search";
        }

        private async void ShowBottomPanel(StackLayout target, uint length = 100)
        {
            HideToolbar();
            await ButtonsPanel.Hide(length);

            target.IsVisible = true;
            await BottomPanel.Show(length);
        }

        private async void HideBottomPanel(uint length = 100)
        {
            await BottomPanel.Hide(length);

            await ButtonsPanel.Show(length);

            NewItemStack.IsVisible = false;
            EditItemStack.IsVisible = false;
            SearchItemStack.IsVisible = false;

            SetToolbar();
        }

        private void NewItemStatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (picker.SelectedItem == null || ViewModel.NewItem == null)
            {
                return;
            }

            ViewModel.NewItem.StateID = ViewModel.NewItem.State.StateID;
        }

        private void EditItemStatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (picker.SelectedItem == null || ViewModel.EditItem == null)
            {
                return;
            }

            ViewModel.EditItem.StateID = ViewModel.EditItem.State.StateID;
        }

        private void SearchItemStatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (picker.SelectedItem == null || ViewModel.SearchItem.State == null)
            {
                return;
            }

            var state = picker.SelectedItem as StateModel;
            if (state.StateID == null)
            {
                return;
            }

            ViewModel.SearchItem.StateID = ViewModel.SearchItem.State.StateID;
        }

        private void ItemNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if(entry.ReturnCommand == null)
            {
                return;
            }

            if (!entry.ReturnCommand.CanExecute(entry.BindingContext) && entry.Text != null && entry.Text != "")
            {
                entry.TextColor = Color.Red;
                entry.PlaceholderColor = Color.Red;
            }
            else
            {
                entry.PlaceholderColor = Color.Default;
                entry.TextColor = Color.Default;
            }
        }

        private void HidePanelButton_Clicked(object sender, EventArgs e)
        {
            HideBottomPanel();
        }

        private void CreateItemButton_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.CreateItemCommand.CanExecute(null))
            {
                ViewModel.CreateItemCommand.Execute(null);
                HideBottomPanel();
                StatePicker.SelectedIndex = 0;
            }
            else
            {
                NewItemNameEntry.PlaceholderColor = Color.Red;
            }
        }

        private void SearchItemButton_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.SearchItem.ItemName != "")
            {
                Title = "Search";
                ViewModel.ShowItemsCommand.Execute(null);
                HideBottomPanel();
            }
        }

        private void SaveItemButton_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.SaveItemCommand.CanExecute(null))
            {
                ViewModel.SaveItemCommand.Execute(null);
                HideBottomPanel();
            }
            else
            {
                EditItemNameEntry.PlaceholderColor = Color.Red;
            }
        }
        #endregion

        #region DisplayedState
        private void StatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;

            if (!(picker.SelectedItem is StateModel state))
            {
                return;
            }

            var items = ItemsListView.ItemsSource as ObservableCollection<ItemModel>;

            if(items != null && items.Count > 0)
            {
                ItemsListView.ScrollTo(items[0], ScrollToPosition.Start, false);
            }

            ViewModel.ShowItemsCommand.Execute(null);
        }

        private void ShowState()
        {
            StatePicker.Focus();
        }
        #endregion

        #region ItemsListView
        private void ItemsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (e.SelectedItem != null && ButtonsPanel.IsVisible)
            {
                BottomHeader.Text = "Edit";

                var item = e.SelectedItem as ItemModel;
                listView.SelectedItem = null;

                ViewModel.SetEditItemCommand.Execute(item);
                ShowBottomPanel(EditItemStack);
            }
            else
            {
                listView.SelectedItem = null;
            }
        }

        private void ItemListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var listview = sender as ListView;
            var source = listview.ItemsSource as ObservableCollection<ItemModel>;
            if (source.IndexOf((e.Item as ItemModel)) >= source.Count - 5 && !listview.IsRefreshing)
            {
                ViewModel.LoadMoreCommand.Execute(null);
            }
        }
        #endregion
    }

    public static class Extension
    {
        public static async Task Hide(this View obj, uint length)
        {
            await obj.LayoutTo(new Rectangle
            {
                Width = obj.Width,
                Height = 0,
                X = obj.X,
                Y = obj.Y + obj.Height
            }, length);
            obj.IsVisible = false;
        }

        public static async Task Show(this View obj, uint length)
        {
            obj.IsVisible = true;
            await obj.LayoutTo(new Rectangle
            {
                Width = obj.Width,
                Height = obj.MinimumHeightRequest,
                X = obj.X,
                Y = obj.Y - obj.MinimumHeightRequest
            }, length);
        }
    }
}