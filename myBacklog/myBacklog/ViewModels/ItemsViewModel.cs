using myBacklog.Models;
using myBacklog.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace myBacklog.ViewModels
{
    public class ItemsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region Variables
        CategoryModel category;
        ItemModel newItem;
        ItemModel editItem;
        ItemModel searchItem;
        ObservableCollection<StateModel> states;
        ObservableCollection<StateModel> selectableStates;
        StateModel visibleState;
        bool isItemsLoading;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Properties
        public CategoryModel Category
        {
            get
            {
                return category;
            }
            set
            {
                if (value != category)
                {
                    category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        public ItemModel EditItem
        {
            get
            {
                return editItem;
            }
            set
            {
                if (value != editItem)
                {
                    editItem = value;
                    OnPropertyChanged("EditItem");
                }
            }
        }

        public ItemModel SearchItem
        {
            get
            {
                return searchItem;
            }
            set
            {
                if (value != searchItem)
                {
                    searchItem = value;
                    OnPropertyChanged("SearchItem");
                }
            }
        }

        public ItemModel NewItem
        {
            get
            {
                return newItem;
            }
            set
            {
                if (value != newItem)
                {
                    newItem = value;
                    OnPropertyChanged("NewItem");
                }
            }
        }

        public ObservableCollection<StateModel> States
        {
            get
            {
                return states;
            }
            set
            {
                if (value != states)
                {
                    states = value;
                    OnPropertyChanged("States");
                }
            }
        }

        public ObservableCollection<StateModel> SelectableStates
        {
            get
            {
                return selectableStates;
            }
            set
            {
                if (value != selectableStates)
                {
                    selectableStates = value;
                    OnPropertyChanged("SelectableStates");
                }
            }
        }

        public StateModel VisibleState
        {
            get
            {
                return visibleState;
            }
            set
            {
                if (value != visibleState)
                {
                    visibleState = value;
                    OnPropertyChanged("VisibleState");
                }
            }
        }

        public bool IsItemsLoading
        {
            get
            {
                return isItemsLoading;
            }
            set
            {
                if (value != isItemsLoading)
                {
                    isItemsLoading = value;
                    OnPropertyChanged("IsItemsLoading");
                }
            }
        }
        #endregion

        #region ICommand
        public ICommand UpdateModelCommand { get; }
        public ICommand SetEditItemCommand { get; }
        public ICommand CreateItemCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand ResetNewItemCommand { get; }
        public ICommand ResetSearchItemCommand { get; }
        public ICommand ResetPanelItemsCommand { get; }
        public ICommand ShowItemsCommand { get; }
        public ICommand LoadMoreCommand { get; }
        public ICommand SetItemNameCommand { get; }
        public ICommand CategorySettingsCommand { get; }
        #endregion

        public ItemsViewModel(INavigationService navigationService, IDialog dialogService, IFirebase databaseService) : base(navigationService, dialogService, databaseService)
        {

            UpdateModelCommand = new Command(async () => await UpdateModelAsync());
            SetEditItemCommand = new Command<ItemModel>((parameter) => SetEditItem(parameter));
            CreateItemCommand = new Command(execute: async () => await CreateItemAsync());
            SaveItemCommand = new Command(execute: async () => await SaveItemAsync());
            ResetNewItemCommand = new Command(ResetNewItem);
            ResetSearchItemCommand = new Command(ResetSearchItem);
            ShowItemsCommand = new Command(async () => await ShowItemsAsync());
            LoadMoreCommand = new Command(execute: async () => await LoadMoreAsync());
            SetItemNameCommand = new Command<ItemModel>(execute: async(parameter) => await SetItemNameAsync(parameter));
            CategorySettingsCommand = new Command(async () => await OpenCategorySettingsAsync());
            ResetPanelItemsCommand = new Command(ResetPanelItems);
        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            await DialogService.PopAsync();
            if (parameters.ContainsKey("category"))
            {
                var c = parameters.GetValue<CategoryModel>("category");
                Category = c;
                UpdateModelCommand.Execute(null);
            }
            else if(parameters.ContainsKey("IsUpdated"))
            {
                UpdateModelCommand.Execute(null);
            }
        }

        #region Execute()
        private async Task UpdateModelAsync()
        {
            await DialogService.DisplayPopupAsync();
            await SetSelectableStatesAsync();
            ResetNewItemCommand.Execute(null);
            ResetSearchItem();
            ShowItemsCommand.Execute(null);
            await DialogService.PopAsync();
        }

        private async Task SetSelectableStatesAsync()
        {
            States = new ObservableCollection<StateModel>
            {
                new StateModel
                {
                    StateName = "All",
                    NamedColor = NamedColor.Transparent
                }
            };

            var states = await FirebaseService.GetStatesAsync(Category.CategoryID);
            SelectableStates = new ObservableCollection<StateModel>(states);

            foreach (var state in states)
            {
                States.Add(state);
            }

            VisibleState = States[0];
        }

        private void ResetNewItem()
        {
            NewItem = new ItemModel
            {
                ItemName = "",
                CategoryID = Category.CategoryID
            };
        }

        private void ResetSearchItem()
        {
            SearchItem = new ItemModel
            {
                ItemName = "",
                CategoryID = Category.CategoryID,
                State = States[0]
            };
        }

        private void SetEditItem(ItemModel item = null)
        {
            EditItem = new ItemModel
            {
                CategoryID = item.CategoryID,
                ItemName = item.ItemName,
                ItemID = item.ItemID,
                State = item.State,
                StateID = item.StateID,
                ID = item.ID
            };
        }

        private async Task SaveItemAsync()
        {
            if (!await CanSaveItemAsync())
            {
                return;
            }

            await DialogService.DisplayPopupAsync();
            await FirebaseService.UpdateItemAsync(EditItem);

            var i = Category.Items.IndexOf(Category.Items.FirstOrDefault(x => x.ItemID == EditItem.ItemID));
            Category.Items[i] = EditItem;
            ResetNewItemCommand.Execute(null);
            await DialogService.DisplayPopupAsync();
        }

        private async Task CreateItemAsync()
        {
            if (!await CanCreateItemAsync())
            {
                return;
            }

            await DialogService.DisplayPopupAsync();
            await FirebaseService.InsertItemAsync(NewItem);
            ResetNewItemCommand.Execute(null);

            await ShowItemsAsync();
            await DialogService.PopAsync();
        }

        private async Task SetItemNameAsync(ItemModel target)
        {
            if(!await CanSetItemNameAsync(target))
            {

            }
        }

        private async Task ShowItemsAsync()
        {
            List<ItemModel> items = new List<ItemModel>();
            Category.Items = new ObservableCollection<ItemModel>();
            if (SearchItem.ItemName != "")
            {
                items = await FirebaseService.GetItemsAsync(SearchItem, 20, null);
            }
            else if (VisibleState == States[0])
            {
                items = await FirebaseService.GetItemsAsync(Category, 20, null);
            }
            else
            {
                items = await FirebaseService.GetItemsAsync(VisibleState, 20, null);
            }

            foreach (var item in items)
            {
                var state = States.FirstOrDefault(x => x.StateID == item.StateID);
                item.State = state;
            }

            Category.Items = new ObservableCollection<ItemModel>(items);
        }

        private async Task LoadMoreAsync()
        {
            if (!await CanLoadMoreAsync())
            {
                return;
            }

            IsItemsLoading = true;

            if (SearchItem.ItemName != "")
            {
                var items = await FirebaseService.GetItemsAsync(SearchItem, 20, Category.Items.Last().ID);
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }
            else if (VisibleState == States[0] && Category.Items.Count != 0)
            {
                var items = await FirebaseService.GetItemsAsync(Category, 20, Category.Items.Last().ID);
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }
            else if (VisibleState.StateID != null)
            {
                var items = await FirebaseService.GetItemsAsync(VisibleState, 20, Category.Items.Last().ID);
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }

            IsItemsLoading = false;
        }

        private async Task OpenCategorySettingsAsync()
        {
            await DialogService.DisplayPopupAsync();
            var parameters = new NavigationParameters
            {
                { "categoryID", Category.CategoryID }
            };

            await DialogService.DisplayPopupAsync();
            await NavigationService.NavigateAsync("SetCategoryPage", parameters);
        }

        private void ResetPanelItems()
        {
            ResetNewItemCommand.Execute(null);
            ResetSearchItemCommand.Execute(null);
        }
        #endregion

        #region CanExecute()
        private async Task<bool> CanSaveItemAsync()
        {
            if(EditItem == null)
            {
                return false;
            }

            if(EditItem.State == null)
            {
                return false;
            }

            if(EditItem.ItemName == "")
            {
                return false;
            }

            return await FirebaseService.IsItemNameAwailableAsync(EditItem);
        }

        private async Task<bool> CanCreateItemAsync()
        {
            if (NewItem == null)
            {
                return false;
            }

            if (NewItem.State == null)
            {
                return false;
            }

            if (NewItem.ItemName == "")
            {
                return false;
            }

            return await FirebaseService.IsItemNameAwailableAsync(NewItem);
        }

        private async Task<bool> CanSetItemNameAsync(ItemModel target)
        {
            if(target == null)
            {
                return false;
            }
            if(target.ItemName == "")
            {
                return false;
            }
            return await FirebaseService.IsItemNameAwailableAsync(target);
        }

        private async Task<bool> CanLoadMoreAsync()
        {
            if (IsItemsLoading)
            {
                return false;
            }
            IsItemsLoading = true;

            if (Category.Items?.Any() != true)
            {
                IsItemsLoading = false;
                return false;
            }

            if (VisibleState == States?[0])
            {
                var count = await FirebaseService.GetItemsCountAsync(Category);

                if (Category.Items.Count >= count)
                {
                    IsItemsLoading = false;
                    return false;
                }
            }
            else if (VisibleState?.StateName == "SearchResults")
            {
                var count = await FirebaseService.GetItemsCountAsync(SearchItem);

                if (Category.Items.Count >= count)
                {
                    IsItemsLoading = false;
                    return false;
                }
            }
            else if (VisibleState.StateID != null)
            {
                var count = await FirebaseService.GetItemsCountAsync(VisibleState);

                if (Category.Items.Count >= count)
                {
                    IsItemsLoading = false;
                    return false;
                }
            }

            IsItemsLoading = false;
            return true;
        }
        #endregion
    }
}
