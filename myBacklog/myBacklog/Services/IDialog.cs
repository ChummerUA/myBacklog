using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace myBacklog.Services
{
    public interface IDialog
    {
        Task<bool> DisplayAlert(string title, string message, string OKButton = "OK", string CancelButton = null);
    }
}
