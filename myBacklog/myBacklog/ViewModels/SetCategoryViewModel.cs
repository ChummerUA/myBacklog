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

        public StateModel EditState { get; set; }

        public bool IsNewCategory { get; set; }
        #endregion

        #region ICommand
        public ICommand SetCategoryNameCommand { get; private set; }
        public ICommand SaveCategoryCommand { get; private set; }

        public ICommand GetCategoryCommand { get; private set; }

        public ICommand CreateStateCommand { get; private set; }
        public ICommand SetStateNameCommand { get; private set; }
        public ICommand RemoveStateCommand { get; private set; }

        public ICommand ChooseColorCommand { get; private set; }
        public ICommand ConfirmColorCommand { get; private set; }
        #endregion

        public SetCategoryViewModel(CategoryModel category)
        {
            SetCommands();
            Category = category;
            IsNewCategory = false;
        }

        public SetCategoryViewModel()
        {
            SetCommands();
            Category = new CategoryModel
            {
                CategoryName = "",
                States = new ObservableCollection<StateModel>()
            };
            IsNewCategory = true;
        }

        public void SetCommands()
        {
            SaveCategoryCommand = new Command(execute: async () => await SaveCategoryAsync(),
                canExecute: CanSaveCategory);
            SetCategoryNameCommand = new Command(execute: SetCategoryName,
                canExecute: CanSetCategoryName);
            GetCategoryCommand = new Command(execute: async () => await GetCategoryAsync());

            CreateStateCommand = new Command<string>(execute: (parameter) => CreateState(parameter),
                canExecute: (parameter) => CanCreateState(parameter));
            SetStateNameCommand = new Command<StateModel>(execute: (parameter) => SetStateName(parameter),
                canExecute: (parameter) => CanSetStateName(parameter));
            RemoveStateCommand = new Command<StateModel>( (parameter) =>  RemoveState(parameter));

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
                await App.Database.CreateCategoryAsync(Category);
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
                Category = await App.Database.GetCategoryAsync(Category.CategoryID);
            }
        }

        private void SetCategoryName()
        {
            if (!CanSetCategoryName())
            {
                return;
            }
        }

        private async Task GetStatesAsync()
        {
            if (!IsNewCategory)
            {
                var states = await App.Database.GetStatesAsync(Category.CategoryID);
                
                Category.States = new ObservableCollection<StateModel>(states);
            }
        }

        private void CreateState(string name)
        {
            if (!CanCreateState(name))
            {
                return;
            }

            StateModel state = new StateModel
            {
                StateName = name,
                StateID = Category.States.Count,
                NamedColor = NamedColor.All.FirstOrDefault(x => x.Name == "Gray"),
            };

            if (!IsNewCategory)
            {
                state.CategoryID = Category.CategoryID;
            }

            Category.States.Add(state);
        }

        private void SetStateName(StateModel state)
        {
            if (!CanSetStateName(state))
            {
                return;
            }

            EditState = null;
        }

        private void RemoveState(StateModel state)
        {
            Category.States.Remove(state);
        }

        private void ChooseColor(StateModel state)
        {
            EditState = state;
        }

        private async Task ConfirmColorAsync(NamedColor color)
        {
            if (!IsNewCategory)
            {
                await App.Database.UpdateStateAsync(EditState);
            }

            EditState.NamedColor = color;

            var state = Category.States.FirstOrDefault(x => x.StateID == EditState.StateID);
            var i = Category.States.IndexOf(state);

            Category.States[i] = EditState;

            await App.Database.UpdateStateAsync(EditState);

            EditState = null;
        }
        #endregion

        #region Command.CanExecute()
        private bool CanSaveCategory()
        {
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(Category.CategoryName);

            if (match.Groups.Count != 1 || match.Groups[0].Length != Category.CategoryName.Length)
            {
                return false;
            }

            if (Category.States.Count == 0)
            {
                return false;
            }

            var isAwailable = App.Database.IsCategoryNameAwailable(Category);

            return isAwailable;
        }

        private bool CanSetCategoryName()
        {
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(Category.CategoryName);

            if (match.Groups.Count != 1 || (match.Groups.Count == 1 && match.Groups[0].Length != Category.CategoryName.Length))
            {
                return false;
            }

            var isAwailable = App.Database.IsCategoryNameAwailable(Category);

            return isAwailable;
        }

        private bool CanCreateState(string parameter)
        {
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(parameter);

            if (match.Groups.Count != 1 || (match.Groups.Count == 1 && match.Groups[0].Length != parameter.Length))
            {
                return false;
            }

            if (Category.States.FirstOrDefault(x => x.StateName == parameter as string) != null)
            {
                return false;
            }
            return true;
        }

        private bool CanSetStateName(StateModel state)
        {
            var name = state.StateName;
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(name);

            if (match.Groups.Count != 1 || (match.Groups.Count == 1 && match.Groups[0].Length != name.Length))
            {
                return false;
            }
            if (Category.States.Count(x => x.StateName == name) > 1)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
