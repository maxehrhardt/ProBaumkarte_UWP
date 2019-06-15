using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ProBaumkarte_UWP.Models.Map
{
    public class MapFile
    {
        public bool IsSvg { get; set; }
        public BitmapImage MapImageSource { get; set; }
        public WriteableBitmap MapSoftwareImageSource { get; set; }
        public byte[] MapBytes { get; set; }
        public string MapSvgSource { get; set; }

        public double TreeMarkersize { get; set; }
        public Color TreeMarkerColor{ get; set; }
        public SvgImageSource svgImageSource { get; set; }

        public bool IsGeoReferenced { get; set; }
        public double LatitudeTop { get; set; }
        public double LatitudeBottom { get; set; }
        public double LongitudeLeft { get; set; }
        public double LongitudeRight { get; set; }
    }
}
