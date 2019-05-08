using myBacklog.Models;
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
    public class ItemsViewModel : INotifyPropertyChanged
    {
        #region Variables
        CategoryModel category;
        ItemModel newItem;
        ItemModel editItem;
        ItemModel searchItem;
        ObservableCollection<StateModel> states;
        ObservableCollection<StateModel> selectableStates;
        ObservableCollection<ItemModel> searchResults;
        StateModel visibleState;
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

        public ObservableCollection<ItemModel> SearchResults
        {
            get
            {
                return searchResults;
            }
            set
            {
                if (value != searchResults)
                {
                    searchResults = value;
                    OnPropertyChanged("SearchResults");
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
        #endregion

        #region ICommand
        public ICommand UpdateModelCommand { get; }
        public ICommand SetEditItemCommand { get; }
        public ICommand CreateItemCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand ResetPanelItemsCommand { get; }
        public ICommand ShowStateCommand { get; }
        public ICommand LoadMoreCommand { get; }
        public ICommand SetItemNameCommand { get; }
        #endregion

        public ItemsViewModel(CategoryModel c)
        {
            Category = c;

            UpdateModelCommand = new Command(async () => await UpdateModelAsync());
            SetEditItemCommand = new Command<ItemModel>((parameter) => SetEditItem(parameter));
            CreateItemCommand = new Command(execute: async () => await CreateItemAsync(),
                canExecute: () => CanCreateItem());
            SaveItemCommand = new Command(execute: async () => await SaveItemAsync(),
                canExecute: CanSaveItem);
            ResetPanelItemsCommand = new Command(ResetPanelItems);
            ShowStateCommand = new Command(async () => await ShowStateAsync());
            LoadMoreCommand = new Command(execute: async () => await LoadMoreAsync());
            SetItemNameCommand = new Command<ItemModel>(execute:(parameter) => SetItemName(parameter),
                canExecute: (parameter) => CanSetItemName(parameter));

            ResetPanelItems();
        }

        #region Execute()
        private async Task UpdateModelAsync()
        {
            await SetSelectableStatesAsync();
            ShowStateCommand.Execute(null);
        }

        private async Task SetSelectableStatesAsync()
        {
            States = new ObservableCollection<StateModel>
            {
                new StateModel
                {
                    StateName = "All"
                }
            };

            var states = await App.Database.GetStatesAsync((int)Category.CategoryID);
            SelectableStates = new ObservableCollection<StateModel>(states);

            foreach (var state in states)
            {
                States.Add(state);
            }

            VisibleState = States[0];
        }

        private void ResetPanelItems()
        {
            EditItem = new ItemModel
            {
                ItemName = "",
                CategoryID = Category.CategoryID
            };
            NewItem = new ItemModel
            {
                ItemName = "",
                CategoryID = Category.CategoryID
            };
            SearchItem = new ItemModel
            {
                ItemName = "",
                CategoryID = Category.CategoryID,
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
                StateID = item.StateID
            };
        }

        private async Task SaveItemAsync()
        {
            if (!CanSaveItem())
            {
                return;
            }
            await App.Database.UpdateItemAsync(EditItem);

            var i = Category.Items.IndexOf(Category.Items.FirstOrDefault(x => x.ItemID == EditItem.ItemID));
            Category.Items[i] = EditItem;
            ResetPanelItemsCommand.Execute(null);
        }

        private async Task CreateItemAsync()
        {
            if (!CanCreateItem())
            {
                return;
            }
            await App.Database.CreateItemAsync(NewItem);
            await ShowStateAsync();
            ResetPanelItemsCommand.Execute(null);
        }

        private void SetItemName(ItemModel target)
        {
            //
        }

        private async Task ShowStateAsync()
        {
            List<ItemModel> items;
            if (VisibleState == States[0])
            {
                items = await App.Database.GetCategoryItemsAsync((int)Category.CategoryID);                
            }
            else
            {
                items = await App.Database.GetStateItemsAsync((int)VisibleState.StateID);
            }

            foreach (var item in items)
            {
                item.State = States.FirstOrDefault(x => x.StateID == item.StateID);
            }
            var source = items.OrderBy(x => x.StateID)
                .ThenByDescending(x => x.ItemID)
                .ToList();

            Category.Items = new ObservableCollection<ItemModel>(source);
        }

        private async Task LoadMoreAsync()
        {
            if (!CanLoadMore())
            {
                return;
            }

            List<ItemModel> items;

            if(SearchItem != null)
            {
                items = await App.Database.GetSearchResultsAsync(SearchItem, (int)SearchResults.Last().ItemID);
                foreach(var item in items)
                {
                    SearchResults.Add(item);
                }

                var source = SearchResults.ToList()
                    .OrderBy(x => x.StateID)
                    .ThenByDescending(x => x.ItemID)
                    .ToList();

                SearchResults = new ObservableCollection<ItemModel>(source);
            }
            else if (VisibleState.StateName != "All")
            {
                items = await App.Database.GetStateItemsAsync((int)VisibleState.StateID, (int)Category.Items.Last().ItemID);

                foreach(var item in items)
                {
                    Category.Items.Add(item);
                }

                var source = Category.Items.ToList()
                    .OrderBy(x => x.StateID)
                    .ThenByDescending(x => x.ItemID)
                    .ToList();

                Category.Items = new ObservableCollection<ItemModel>(source);
            }
            else
            {
                items = await App.Database.GetCategoryItemsAsync((int)Category.CategoryID, (int)Category.Items.Last().ItemID);

                foreach(var item in items)
                {
                    Category.Items.Add(item);
                }

                var source = Category.Items.ToList()
                    .OrderBy(x => x.StateID)
                    .ThenByDescending(x => x.ItemID)
                    .ToList();

                Category.Items = new ObservableCollection<ItemModel>(source);
            }
        }
        #endregion

        #region CanExecute()
        private bool CanSaveItem()
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

            return App.Database.IsItemNameAwailable(EditItem);
        }

        private bool CanCreateItem()
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

            return App.Database.IsItemNameAwailable(NewItem);
        }

        private bool CanSetItemName(ItemModel target)
        {
            if(target == null)
            {
                return false;
            }
            if(target.ItemName == "")
            {
                return false;
            }
            return App.Database.IsItemNameAwailable(target);
        }

        private bool CanLoadMore()
        {
            if(SearchItem != null)
            {
                if(SearchItem.ItemName == "")
                {
                    return false;
                }
                else if (App.Database.GetSearchResultsCount(SearchItem) == SearchResults.Count)
                {
                    return false;
                }
            }
            else if (VisibleState.StateName == "All")
            {
                if (App.Database.GetCategoryItemsCount((int)Category.CategoryID) == Category.Items.Count)
                {
                    return false;
                }
            }
            else
            {
                if (App.Database.GetStateItemsCount((int)VisibleState.StateID) == Category.Items.Count)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
