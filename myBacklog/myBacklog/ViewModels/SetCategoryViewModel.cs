using myBacklog.Models;
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
    public class SetCategoryViewModel : INotifyPropertyChanged
    {
        CategoryModel category;
        CategoryModel savedCategory;
        ObservableCollection<StateModel> states;
        ObservableCollection<StateModel> savedStates;

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

        public StateModel EditState { get; set; }

        public bool IsNewCategory { get; set; }
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

        public SetCategoryViewModel(CategoryModel c, List<StateModel> s)
        {
            SetCommands();

            Category = c;
            SavedCategory = c;

            States = new ObservableCollection<StateModel>(s);
            SavedStates = new ObservableCollection<StateModel>(s);

            IsNewCategory = false;
        }

        public SetCategoryViewModel()
        {
            SetCommands();
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
        }

        public void SetCommands()
        {
            SaveCategoryCommand = new Command(execute: async () => await SaveCategoryAsync(),
                canExecute: CanSaveCategory);
            SetCategoryNameCommand = new Command(execute: async () => await SetCategoryNameAsync(),
                canExecute: CanSetCategoryName);
            GetCategoryCommand = new Command(execute: async () => await GetCategoryAsync());
            DeleteCategoryCommand = new Command(execute: async () => await DeleteCategoryAsync());

            CreateStateCommand = new Command<string>(execute: async (parameter) => await CreateStateAsync(parameter),
                canExecute: (parameter) => CanCreateState(parameter));
            SetStateNameCommand = new Command<StateModel>(execute: async (parameter) => await SetStateNameAsync(parameter),
                canExecute: (parameter) => CanSetStateName(parameter));
            RemoveStateCommand = new Command<StateModel>(async (parameter) => await RemoveStateAsync(parameter),
                canExecute: (parameter) => CanRemoveState(parameter));
            CancelChangesCommand = new Command(CancelChanges);

            ChooseColorCommand = new Command<StateModel>(ChooseColor);
            ConfirmColorCommand = new Command<NamedColor>(async (parameter) => await ConfirmColorAsync(parameter));
        }

        #region Command.Execute()
        private async Task SaveCategoryAsync()
        {
            if (!CanSaveCategory())
            {
                return;
            }
            if (IsNewCategory)
            {
                var categoryID = await App.Database.CreateCategoryAsync(Category);
                foreach(var state in States)
                {
                    state.CategoryID = categoryID;
                    await App.Database.CreateStateAsync(state);
                }
            }
            else
            {
                await App.Database.UpdateCategoryAsync(Category);
            }
        }

        private async Task GetCategoryAsync()
        {
            if (!IsNewCategory)
            {
                Category = await App.Database.GetCategoryAsync((int)Category.CategoryID);
            }
        }

        private async Task SetCategoryNameAsync()
        {
            if (!CanSetCategoryName())
            {
                return;
            }

            if (!IsNewCategory)
            {
                await App.Database.UpdateCategoryAsync(Category);
            }

            SaveChanges();
        }
        
        private async Task DeleteCategoryAsync()
        {
            await App.Database.DeleteCategoryAsync(Category);
        }

        private async Task CreateStateAsync(string name)
        {
            if (!CanCreateState(name))
            {
                return;
            }

            StateModel state = new StateModel
            {
                StateName = name,
                NamedColor = NamedColor.All.FirstOrDefault(x => x.Name == "Gray"),
            };
            States.Add(state);

            if (!IsNewCategory)
            {
                state.CategoryID = Category.CategoryID;
                await App.Database.CreateStateAsync(state);
            }

            SaveChanges();
        }

        private async Task SetStateNameAsync(StateModel state)
        {
            if (!CanSetStateName(state))
            {
                return;
            }

            if (!IsNewCategory)
            {
                await App.Database.UpdateStateAsync(state);
            }

            SaveChanges();
        }

        private async Task RemoveStateAsync(StateModel state)
        {
            if (!CanRemoveState(state))
            {
                return;
            }

            await App.Database.DeleteStateAsync(state);

            States.Remove(state);
            SaveChanges();
        }

        private void ChooseColor(StateModel state)
        {
            EditState = state;
        }

        private async Task ConfirmColorAsync(NamedColor color)
        {
            var i = States.IndexOf(EditState);
            States[i].NamedColor = color;

            if (!IsNewCategory)
            {
                await App.Database.UpdateStateAsync(States[i]);
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
        private bool CanSaveCategory()
        {
            if(Category.CategoryName == "" || Category.CategoryName == null)
            {
                return false;
            }

            if (States.Count == 0)
            {
                return false;
            }

            var isAwailable = App.Database.IsCategoryNameAwailable(Category);

            return isAwailable;
        }

        private bool CanSetCategoryName()
        {
            if(Category.CategoryName == "" || Category.CategoryName == null)
            {
                return false;
            }

            var isAwailable = App.Database.IsCategoryNameAwailable(Category);

            return isAwailable;
        }

        private bool CanCreateState(string parameter)
        {
            if(parameter == "" || parameter == null)
            {
                return false;
            }

            if (States.FirstOrDefault(x => x.StateName == parameter as string) != null)
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

        private bool CanRemoveState(StateModel state)
        {
            if (!IsNewCategory && States.Count == 1)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
