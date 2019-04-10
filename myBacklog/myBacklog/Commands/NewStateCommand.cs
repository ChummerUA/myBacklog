using myBacklog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace myBacklog.Commands
{
    class NewStateCommand : ICommand
    {
        SetCategoryViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var regex = new Regex(@"[\w\s]+");
            var match = regex.Match(parameter as string);

            if (!match.Success)
            {
                return false;
            }

            if(viewModel.Category.States.FirstOrDefault(x => x.StateName == parameter as string) != null)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            viewModel.CreateNewState(parameter as string);
        }

        public NewStateCommand(SetCategoryViewModel vm)
        {
            viewModel = vm;
        }
    }
}
