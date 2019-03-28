using ProBaumkarte_UWP.Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBaumkarte_UWP.Services.Dialog
{
    public interface IDialogService
    {
        Task<bool> ShowGeoDataQuestionDialog();
        Task<MapGeoData> ShowGeoDataEntryDialog();
        void ShowErrorDialog(string message);
    }
}
