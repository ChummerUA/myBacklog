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
        public CategoryModel Category { get; set; }

        public StateModel EditState { get; set; }

        public bool IsNewCategory { get; set; }
        #endregion

        #region ICommand
        public ICommand SetCategoryNameCommand { get; }
        public ICommand SaveCategoryCommand { get; }

        public ICommand SetStatesCommand { get; }
        public ICommand CreateStateCommand { get; }
        public ICommand SetStateNameCommand { get; }
        public ICommand RemoveStateCommand { get; }

        public ICommand ChooseColorCommand { get; }
        public ICommand ConfirmColorCommand { get; }
        #endregion

        public SetCategoryViewModel(CategoryModel category = null)
        {
            if(category != null)
            {
                Category = category;
                IsNewCategory = false;
            }
            else
            {
                Category = new CategoryModel
                {
                    CategoryName = "",
                    States = new ObservableCollection<StateModel>()
                };
                IsNewCategory = true;
            }

            SaveCategoryCommand = new Command(execute: async() => await SaveCategoryAsync(),
                canExecute: CanSaveCategory);
            SetCategoryNameCommand = new Command<string>(execute: async (parameter) => await SetCategoryNameAsync(parameter),
                canExecute: CanSetCategoryName);

            SetStatesCommand = new Command(async () => await SetStatesAsync());
            CreateStateCommand = new Command<string>(execute: async (parameter) => await CreateStateAsync(parameter),
                canExecute: (parameter) => CanCreateState(parameter));
            SetStateNameCommand = new Command<StateModel>(execute: async(parameter) => await SetStateNameAsync(parameter),
                canExecute: (parameter) => CanSetStateName(parameter));
            RemoveStateCommand = new Command<StateModel>(async (parameter) => await RemoveStateAsync(parameter));

            ChooseColorCommand = new Command<StateModel>(ChooseColor);
            ConfirmColorCommand = new Command<System.Drawing.Color>(async (parameter) => await ConfirmColorAsync(parameter));
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

        private async Task SetCategoryNameAsync(string name)
        {
            if (!CanSetCategoryName(name))
            {
                return;
            }
        }

        private async Task SetStatesAsync()
        {
            if (!IsNewCategory)
            {
                var states = await App.Database.GetStatesAsync(Category.CategoryID);
                
                Category.States = new ObservableCollection<StateModel>(states);
            }
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
                StateID = Category.States.Count,
                Color = Color.Gray,
            };

            if (!IsNewCategory)
            {
                state.CategoryID = Category.CategoryID;
            }
            else
            {
                await App.Database.CreateStateAsync(state);
            }

            Category.States.Add(state);
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

            EditState = null;
        }

        private async Task RemoveStateAsync(StateModel state)
        {
            if (!IsNewCategory)
            {
                await App.Database.DeleteStateAsync(state);
            }

            Category.States.Remove(state);
        }

        private void ChooseColor(StateModel state)
        {
            EditState = state;
        }

        private async Task ConfirmColorAsync(Color color)
        {
            if (!IsNewCategory)
            {
                await App.Database.UpdateStateAsync(EditState);
            }

            EditState.Color = color;

            var state = Category.States.FirstOrDefault(x => x.StateID == EditState.StateID);
            var i = Category.States.IndexOf(state);

            Category.States[i] = EditState;
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

            return true;
        }

        private bool CanSetCategoryName(string parameter)
        {
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(parameter);

            if (match.Groups.Count != 1 || match.Groups[0].Length != parameter.Length)
            {
                return false;
            }

            return true;
        }

        private bool CanCreateState(string parameter)
        {
            var pattern = @"[\w\s]+";
            var regex = new Regex(pattern);

            var match = regex.Match(parameter);

            if (match.Groups.Count != 1 || match.Groups[0].Length != parameter.Length)
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

            if (match.Groups.Count != 1 || match.Groups[0].Length != name.Length)
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
