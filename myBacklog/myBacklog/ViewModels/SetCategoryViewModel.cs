using myBacklog.Models;
using myBacklog.Services;
using Prism.Mvvm;
using Prism.Navigation;
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

namespace myBacklog.ViewModels
{
    public class SetCategoryViewModel : BaseViewModel, INotifyPropertyChanged
    {
        CategoryModel category;
        CategoryModel savedCategory;
        ObservableCollection<StateModel> states;
        ObservableCollection<StateModel> savedStates;
        string title;

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            var changed = this.PropertyChanged;
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
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        public CategoryModel SavedCategory
        {
            get
            {
                return savedCategory;
            }
            set
            {
                if(savedCategory != value)
                {
                    savedCategory = value;
                    OnPropertyChanged("SavedCategory");
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

        public ObservableCollection<StateModel> SavedStates
        {
            get
            {
                return savedStates;
            }
            set
            {
                if(value != savedStates)
                {
                    savedStates = value;
                    OnPropertyChanged("SavedStates");
                }
            }
        }

        private List<StateModel> DeletedStates { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public StateModel EditState { get; set; }

        public bool IsNewCategory { get; set; } = true;

        public bool IsUpdated { get; set; } = false;
        #endregion

        #region ICommand
        public ICommand CancelChangesCommand { get; private set; }

        public ICommand SetCategoryNameCommand { get; private set; }
        public ICommand SaveCategoryCommand { get; private set; }
        public ICommand DeleteCategoryCommand { get; private set; }

        public ICommand GetCategoryCommand { get; private set; }

        public ICommand CreateStateCommand { get; private set; }
        public ICommand SetStateNameCommand { get; private set; }
        public ICommand RemoveStateCommand { get; private set; }

        public ICommand ChooseColorCommand { get; private set; }
        public ICommand ConfirmColorCommand { get; private set; }
        #endregion

        public SetCategoryViewModel(INavigationService navigationService, IDialog dialogService, IFirebase databaseService) : base(navigationService, dialogService, databaseService)
        {
            DeletedStates = new List<StateModel>();
            SetCommands();
        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            await DialogService.PopAsync();
            if (parameters.ContainsKey("color"))
            {
                var color = parameters["color"] as NamedColor;
                ConfirmColorCommand.Execute(color);
            }
            else if(parameters.ContainsKey("categoryID"))
            {

                IsNewCategory = false;
                var categoryID = parameters["categoryID"] as string;
                GetCategoryCommand.Execute(categoryID);
            }
            else
            {
                Category = new CategoryModel
                {
                    CategoryName = "",
                };
                SavedCategory = new CategoryModel
                {
                    CategoryName = "",
                };

                States = new ObservableCollection<StateModel>();
                SavedStates = new ObservableCollection<StateModel>();

                IsNewCategory = true;
                Title = "New category";
                IsUpdated = true;
            }
        }

        public void SetCommands()
        {
            SaveCategoryCommand = new Command(execute: async () => await SaveCategoryAsync());
            SetCategoryNameCommand = new Command(execute: async () => await SetCategoryNameAsync());
            GetCategoryCommand = new Command<string>(execute: async (parameter) => await GetCategoryAsync(parameter));
            DeleteCategoryCommand = new Command(execute: async () => await DeleteCategoryAsync());

            CreateStateCommand = new Command(execute: CreateState);
            SetStateNameCommand = new Command<StateModel>(execute: (parameter) => SetStateName(parameter),
                canExecute: (parameter) => CanSetStateName(parameter));
            RemoveStateCommand = new Command<StateModel>((parameter) => RemoveState(parameter));
            CancelChangesCommand = new Command(CancelChanges);

            ChooseColorCommand = new Command<StateModel>(ChooseColor);
            ConfirmColorCommand = new Command<NamedColor>((parameter) => ConfirmColor(parameter));
        }

        #region Command.Execute()
        private async Task SaveCategoryAsync()
        {
            if (!await CanSaveCategoryAsync())
            {
                return;
            }

            await DialogService.DisplayPopupAsync();
            var parameters = new NavigationParameters();
            if (IsUpdated)
            {
                parameters.Add("IsUpdated", true);
            }

            if (IsNewCategory)
            {
                var categoryID = await FirebaseService.InsertCategoryAsync(Category);
                for(int i = States.Count - 1; i >= 0; i--)
                {
                    States[i].CategoryID = categoryID;
                    await FirebaseService.InsertStateAsync(States[i]);
                }
            }
            else
            {
                await FirebaseService.UpdateCategoryAsync(Category);
                foreach(var state in States)
                {
                    if(state.StateID == null)
                    {
                        await FirebaseService.InsertStateAsync(state);
                    }
                    else
                    {
                        await FirebaseService.UpdateStateAsync(state);
                    }
                }
                foreach(var state in DeletedStates)
                {
                    await FirebaseService.DeleteStateAsync(state);
                }
            }
            await DialogService.PopAsync();
            await NavigationService.GoBackAsync(parameters);
        }

        private async Task GetCategoryAsync(string categoryID)
        {
            if (!IsNewCategory)
            {
                await DialogService.DisplayPopupAsync();
                Category = await FirebaseService.GetCategoryAsync(categoryID);
                States = new ObservableCollection<StateModel>(await FirebaseService.GetStatesAsync(categoryID));
                await DialogService.PopAsync();
            }
        }

        private async Task SetCategoryNameAsync()
        {
            if (!await CanSetCategoryNameAsync())
            {
                return;
            }

            if (!IsNewCategory)
            {
                IsUpdated = true;
            }

            SaveChanges();
        }
        
        private async Task DeleteCategoryAsync()
        {
            var delete = await DialogService.DisplayAlert("Delete category", "Are you sure you want to delete category", "Ok", "Cancel");
            if (delete)
            {
                await DialogService.DisplayPopupAsync();
                await FirebaseService.DeleteCategoryAsync(Category);
                IsUpdated = true;

                var parameters = new NavigationParameters
                {
                    { "IsUpdated", IsUpdated }
                };

                await DialogService.DisplayPopupAsync();
                await NavigationService.GoBackToRootAsync(parameters);
            }
        }

        private void CreateState()
        {
            if (!CanCreateState())
            {
                return;
            }

            StateModel state = new StateModel
            {
                StateName = "",
                NamedColor = NamedColor.All.FirstOrDefault(x => x.Name == "Gray"),
            };
            States.Add(state);

            if (!IsNewCategory)
            {
                state.CategoryID = Category.CategoryID;
                IsUpdated = true;
            }

            SaveChanges();
        }

        private void SetStateName(StateModel state)
        {
            if (!CanSetStateName(state))
            {
                return;
            }

            if (!IsNewCategory)
            {
                IsUpdated = true;
            }

            SaveChanges();
        }

        private void RemoveState(StateModel state)
        {
            if (!IsNewCategory)
            {
                IsUpdated = true;
            }

            States.Remove(state);
            if(state.StateID != null)
            {
                DeletedStates.Add(state);
            }

            SaveChanges();
        }

        private void ChooseColor(StateModel state)
        {
            EditState = state;

            var parameters = new NavigationParameters
            {
                { "state", state }
            };

            NavigationService.NavigateAsync("SetColorPage", parameters);
        }

        private void ConfirmColor(NamedColor color)
        {
            var i = States.IndexOf(EditState);
            States[i].NamedColor = color;

            if (!IsNewCategory)
            {
                IsUpdated = true;
            }

            EditState = null;
            SaveChanges();
        }

        private void CancelChanges()
        {
            Category = new CategoryModel
            {
                CategoryName = SavedCategory.CategoryName
            };
            if(SavedCategory.CategoryID != null)
            {
                Category.CategoryID = SavedCategory.CategoryID;
            }

            States = new ObservableCollection<StateModel>(SavedStates.ToList());
        }

        private void SaveChanges()
        {
            SavedCategory = new CategoryModel
            {
                CategoryName = Category.CategoryName
            };
            if(Category.CategoryID != null)
            {
                SavedCategory.CategoryID = Category.CategoryID;
            }

            SavedStates = new ObservableCollection<StateModel>(States.ToList());
        }
        #endregion

        #region Command.CanExecute()
        private async Task<bool> CanSaveCategoryAsync()
        {
            if (Category == null)
            {
                return false;
            }

            if(Category.CategoryName == "" || Category.CategoryName == null)
            {
                return false;
            }

            if (States.Count == 0)
            {
                return false;
            }

            if(States.FirstOrDefault(x => x.StateName == "") != null)
            {
                return false;
            }
            if(!CanCreateState())
            {
                return false;
            }

            return await FirebaseService.IsCategoryNameAwailableAsync(Category);
        }

        private async Task<bool> CanSetCategoryNameAsync()
        {
            if(Category.CategoryName == "" || Category.CategoryName == null)
            {
                return false;
            }

            return await FirebaseService.IsCategoryNameAwailableAsync(Category);
        }

        private bool CanCreateState()
        {
            if(States.FirstOrDefault(x => x.StateName == "New state") != null)
            {
                return false;
            }
            return true;
        }

        private bool CanSetStateName(StateModel state)
        {
            var name = state.StateName;
            if(name == "" || name == null)
            {
                return false;
            }

            if (States.Count(x => x.StateName == name) > 1)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
