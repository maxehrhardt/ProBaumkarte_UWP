using ProBaumkarte_UWP.Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ProBaumkarte_UWP.Services.Dialog
{
    public class DialogService:IDialogService
    {
        public async Task<bool> ShowGeoDataQuestionDialog()
        {
            //Open the dialog to enter the geoinformation of the map
            ContentDialog geoDialog = new ContentDialog
            {
                Title = "Geoinformationen eingeben?",
                CloseButtonText = "Nein",
                PrimaryButtonText = "Ja"

            };
            ContentDialogResult result = await geoDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<MapGeoData> ShowGeoDataEntryDialog()
        {

            GeoContentDialog geoDataDialog = new GeoContentDialog();
            await geoDataDialog.ShowAsync();

            return geoDataDialog.mapGeoData;
        }

        public async void ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Fehler!",
                Content = message,
                CloseButtonText="Ok"         
            };

            await errorDialog.ShowAsync();
        }
    }
}
