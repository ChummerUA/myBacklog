using myBacklog.Models;
using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace myBacklog.Commands
{
    class SetStateNameCommand : ICommand
    {
        SetCategoryViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var name = parameter as string;
            var regex = new Regex(@"[\w\s]+");
            var match = regex.Match(name);

            if (!match.Success)
            {
                return false;
            }
            if(viewModel.Category.States.Count(x => x.StateName == name) > 1)
            {
                return false;
            }

            return true;
        }

        public void Execute(object parameter)
        {
            var state = viewModel.EditState;
            state.StateName = parameter as string;
            viewModel.SetStateName(state);
        }

        public SetStateNameCommand(SetCategoryViewModel vm)
        {
            viewModel = vm;
        }
    }
}
