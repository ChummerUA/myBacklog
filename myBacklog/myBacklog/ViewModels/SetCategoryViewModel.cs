using myBacklog.Commands;
using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public ICommand NewStateCommand { get; }
        public ICommand SetStateNameCommand { get; }
        public ICommand RemoveCommand { get; }
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
                    ID = 0,
                    CategoryName = "",
                    States = new ObservableCollection<StateModel>()
                };
                IsNewCategory = true;
            }

            NewStateCommand = new NewStateCommand(this);
            SetStateNameCommand = new SetStateNameCommand(this);
            RemoveCommand = new Command<StateModel>(RemoveState);
            ChooseColorCommand = new Command<StateModel>(ChooseColor);
            ConfirmColorCommand = new Command<System.Drawing.Color>(ConfirmColor);
        }

        public void CreateNewState(string name)
        {
            StateModel newState = new StateModel
            {
                StateName = name,
                ID = Category.States.Count,
                Color = Color.Gray
            };
            Category.States.Add(newState);
        }

        public void SetStateName(StateModel state)
        {
            Category.States[state.ID] = state;
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

        private void ConfirmColor(System.Drawing.Color color)
        {
            EditState.Color = color;
            Category.States[EditState.ID] = EditState;
            EditState = null;
        }
    }
}
