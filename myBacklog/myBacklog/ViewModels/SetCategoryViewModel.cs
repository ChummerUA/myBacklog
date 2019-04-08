using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public int ID { get; set; }

        public string CategoryName { get; set; }

        public ObservableCollection<StateModel> States { get; set; }

        public StateModel SelectedState { get; set; }

        public List<NamedColor> Colors { get; set; }

        public bool IsNewCategory { get; set; }
        #endregion

        #region ICommand
        public ICommand NewStateCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ChooseColorCommand { get; }
        public ICommand ConfirmColorCommand { get; }
        #endregion

        public SetCategoryViewModel(CategoryModel category = null)
        {
            if(category != null)
            {
                ID = category.ID;
                CategoryName = category.CategoryName;
                
                foreach(var state in category.States)
                {
                    States.Add(state);
                }
                IsNewCategory = false;
            }
            else
            {
                ID = 0;
                CategoryName = "";
                States = new ObservableCollection<StateModel>();
                IsNewCategory = true;
            }


            NewStateCommand = new Command<string>(CreateNewState);
            RemoveCommand = new Command<StateModel>(RemoveState);
            ChooseColorCommand = new Command<StateModel>(ChooseColor);
            ConfirmColorCommand = new Command<Color>(ConfirmColor);

            Colors = NamedColor.All;
        }

        private void CreateNewState(string name)
        {
            StateModel state = new StateModel
            {
                StateName = name,
                ID = States.Count,
                Color = Color.Gray
            };
            States.Add(state);
            OnPropertyChanged("States");
        }

        private void RemoveState(StateModel state)
        {
            States.Remove(state);
        }

        private void ChooseColor(StateModel state)
        {
            SelectedState = state;
        }

        private void ConfirmColor(Color color)
        {
            SelectedState.Color = color;
            SelectedState = null;
        }
    }
}
