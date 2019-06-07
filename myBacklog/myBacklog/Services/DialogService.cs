using myBacklog.Views.Pages;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Services
{
    public class DialogService : IDialog
    {
        protected readonly IPopupNavigation Popup = PopupNavigation.Instance;

        public async Task<bool> DisplayAlert(string title, string message, string OKButton = "OK", string CancelButton = null)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, OKButton, CancelButton);
        }

        public async Task DisplayPopupAsync()
        {
            if(Popup.PopupStack.Count == 0)
                await Popup.PushAsync(new ActivityPopupPage());
        }

        public async Task PopAsync()
        {
            await Popup.PopAllAsync();
        }
    }
}
