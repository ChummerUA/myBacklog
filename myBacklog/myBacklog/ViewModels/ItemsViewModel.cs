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
        ItemModel editableItem;
        ItemModel searchParameters;
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

        public ItemModel EditableItem
        {
            get
            {
                return editableItem;
            }
            set
            {
                if (value != editableItem)
                {
                    editableItem = value;
                    OnPropertyChanged("EditableItem");
                }
            }
        }

        public ItemModel SearchParameters
        {
            get
            {
                return searchParameters;
            }
            set
            {
                if (value != searchParameters)
                {
                    searchParameters = value;
                    OnPropertyChanged("SearchParameters");
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
                if(value != visibleState)
                {
                    visibleState = value;
                    OnPropertyChanged("VisibleState");
                }
            }
        }
        #endregion

        #region ICommand
        public ICommand UpdateModelCommand { get; }
        public ICommand SetEditableItemCommand { get; }
        public ICommand ClearEditableItemCommand { get; }
        public ICommand SaveItemCommand { get; }
        public ICommand ShowStateCommand { get; }
        public ICommand SetItemNameCommand { get; }
        public ICommand LoadMoreCommand { get; }
        #endregion

        public ItemsViewModel(CategoryModel c)
        {
            Category = c;

            UpdateModelCommand = new Command(async () => await UpdateModelAsync());
            SetEditableItemCommand = new Command<ItemModel>((parameter) => SetEditableItem(parameter));
            SaveItemCommand = new Command(execute: async () => await SaveItemAsync(),
                canExecute: CanSaveItem);
            ShowStateCommand = new Command(async (paramete) => await ShowStateAsync());
            ClearEditableItemCommand = new Command(ClearEditableItem);
            SetItemNameCommand = new Command(execute: SetItemName,
                canExecute: CanSetItemName);
            LoadMoreCommand = new Command(execute: async () => await LoadMoreAsync());
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

        private void SetEditableItem(ItemModel item = null)
        {
            if (item == null)
            {
                item = new ItemModel
                {
                    ItemName = "",
                    CategoryID = Category.CategoryID
                };
            }
            EditableItem = new ItemModel
            {
                CategoryID = item.CategoryID,
                StateID = item.StateID,
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                State = item.State
            };
        }

        private void ClearEditableItem()
        {
            EditableItem = null;
        }

        private async Task SaveItemAsync()
        {
            if (!CanSaveItem())
            {
                return;
            }


            if (EditableItem.ItemID != null)
            {
                await App.Database.UpdateItemAsync(EditableItem);
                VisibleState = States[0];
                ShowStateCommand.Execute(null);
            }
            else
            {
                var item = EditableItem;
                await App.Database.CreateItemAsync(item);
                VisibleState = States[0];
                ShowStateCommand.Execute(null);
            }
        }

        private void SetItemName()
        {
            if (!CanSetItemName() && EditableItem.ItemID != null)
            {
                EditableItem.ItemName = Category.Items.FirstOrDefault(x => x.ItemID == EditableItem.ItemID).ItemName;
            }
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

            if(SearchParameters != null)
            {
                items = await App.Database.GetSearchResultsAsync(SearchParameters, (int)SearchResults.Last().ItemID);
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
            if (EditableItem.ItemName == "")
            {
                return false;
            }
            if(EditableItem.StateID == null)
            {
                return false;
            }

            return App.Database.IsItemNameAwailable(EditableItem);
        }

        private bool CanSetItemName()
        {
            if(EditableItem == null)
            {
                return false;
            }
            if(EditableItem.ItemName == "")
            {
                return false;
            }
            return App.Database.IsItemNameAwailable(EditableItem);
        }

        private bool CanLoadMore()
        {
            if(SearchParameters != null)
            {
                if(SearchParameters.ItemName == "")
                {
                    return false;
                }
                else if (App.Database.GetSearchResultsCount(SearchParameters) == SearchResults.Count)
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
