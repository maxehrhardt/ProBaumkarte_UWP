using ProBaumkarte_UWP.Models.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace ProBaumkarte_UWP.Services.Dialog
{
    public sealed partial class GeoContentDialog : ContentDialog
    {
        public MapGeoData mapGeoData { get; set; }

        public GeoContentDialog()
        {
            this.InitializeComponent();
            mapGeoData = new MapGeoData();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            mapGeoData.LatitudeTop = Convert.ToDouble( Top.Text);
            mapGeoData.LatitudeBottom = Convert.ToDouble(Bottom.Text);
            mapGeoData.LongitudeLeft = Convert.ToDouble(Left.Text);
            mapGeoData.LongitudeRight = Convert.ToDouble(Right.Text);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
