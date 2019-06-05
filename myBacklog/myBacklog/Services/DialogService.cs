using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Services
{
    public class DialogService : IDialog
    {
        public async Task<bool> DisplayAlert(string title, string message, string OKButton = "OK", string CancelButton = null)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, OKButton, CancelButton);
        }
    }
}
