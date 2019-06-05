using myBacklog.Models;
using myBacklog.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace myBacklog.ViewModels
{
    public class SetColorViewModel : BaseViewModel
    {
        #region Properties
        public ObservableCollection<NamedColor> Colors { get; set; }

        public NamedColor SelectedColor { get; set; }
        #endregion

        public ICommand ConfirmColorCommand { get; }

        public SetColorViewModel(INavigationService navigationService, IDialog dialogService) : base(navigationService, dialogService)
        {
            Colors = new ObservableCollection<NamedColor>();
            var colors = NamedColor.All;

            foreach(var color in colors)
            {
                Colors.Add(color);
            }

            ConfirmColorCommand = new Command<NamedColor>(async (parameter) => await ConfirmColorAsync(parameter));
        }

        private async Task ConfirmColorAsync(NamedColor color)
        {
            var parameters = new NavigationParameters();
            parameters.Add("color", color);
            await NavigationService.GoBackAsync(parameters);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            var state = parameters.GetValue<StateModel>("state");
            SelectedColor = state.NamedColor;
        }
    }
}
