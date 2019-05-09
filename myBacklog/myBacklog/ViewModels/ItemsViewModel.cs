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
        public ICommand ShowItemsCommand { get; }
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
            ResetNewItemCommand = new Command(ResetNewItem);
            ResetSearchItemCommand = new Command(ResetSearchItem);
            ShowItemsCommand = new Command(async () => await ShowItemsAsync());
            LoadMoreCommand = new Command(execute: async () => await LoadMoreAsync());
            SetItemNameCommand = new Command<ItemModel>(execute:(parameter) => SetItemName(parameter),
                canExecute: (parameter) => CanSetItemName(parameter));
        }

        #region Execute()
        private async Task UpdateModelAsync()
        {
            await SetSelectableStatesAsync();
            ResetNewItemCommand.Execute(null);
            ResetSearchItem();
            ShowItemsCommand.Execute(null);
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

            var states = await App.Database.GetStatesAsync((int)Category.CategoryID);
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
            ResetNewItemCommand.Execute(null);
        }

        private async Task CreateItemAsync()
        {
            if (!CanCreateItem())
            {
                return;
            }
            await App.Database.CreateItemAsync(NewItem);
            await ShowItemsAsync();
            ResetNewItemCommand.Execute(null);
        }

        private void SetItemName(ItemModel target)
        {
            //
        }

        private async Task ShowItemsAsync()
        {
            List<ItemModel> items = new List<ItemModel>();
            Category.Items = new ObservableCollection<ItemModel>();
            if (SearchItem.ItemName != "")
            {
                items = await App.Database.GetSearchResultsAsync(SearchItem);
            }
            else if (VisibleState == States[0])
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

            IsItemsLoading = true;

            if (SearchItem.ItemName != "")
            {
                var items = await App.Database.GetSearchResultsAsync(SearchItem, (int)Category.Items.Last().ItemID);
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }
            else if (VisibleState == States[0])
            {
                var items = await App.Database.GetCategoryItemsAsync((int)Category.CategoryID, (int)Category.Items.Last().ItemID);
                for(int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }
            else if (VisibleState.StateID != null)
            {
                var items = await App.Database.GetStateItemsAsync((int)VisibleState.StateID, (int)Category.Items.Last().ItemID);
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].State = SelectableStates.FirstOrDefault(x => x.StateID == items[i].StateID);
                    Category.Items.Add(items[i]);
                }
            }

            IsItemsLoading = false;
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
            if (IsItemsLoading)
            {
                return false;
            }
            if (VisibleState == States[0])
            {
                var count = App.Database.GetCategoryItemsCount((int)Category.CategoryID);

                if (Category.Items.Count >= count)
                {
                    return false;
                }
            }
            else if (VisibleState.StateName == "SearchResults")
            {
                var count = App.Database.GetSearchResultsCount(SearchItem);

                if (Category.Items.Count >= count)
                {
                    return false;
                }
            }
            else if (VisibleState.StateID != null)
            {
                var count = App.Database.GetStateItemsCount((int)VisibleState.StateID);

                if (Category.Items.Count >= count)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
