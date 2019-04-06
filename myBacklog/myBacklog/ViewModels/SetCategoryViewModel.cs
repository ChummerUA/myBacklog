﻿using myBacklog.Models;
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

        private void OnPropertyChanged(string name)
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
        #endregion

        #region ICommand
        public ICommand NewStateCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ChooseColorCommand { get; }
        #endregion

        public SetCategoryViewModel( )
        {
            ID = 0;
            CategoryName = "New category";
            States = new ObservableCollection<StateModel>();

            NewStateCommand = new Command<string>(CreateNewState);
            RemoveCommand = new Command<StateModel>(RemoveState);
            ChooseColorCommand = new Command<StateModel>(ChooseColor);
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
            OnPropertyChanged("StatesObservable");
        }

        private void RemoveState(StateModel state)
        {
            States.Remove(state);
        }

        private void ChooseColor(StateModel state)
        {
            var colors = NamedColor.All;
            var picker = new Picker
            {
                ItemsSource = colors,
            };
            picker.Focus();
            
        }
    }
}
