using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ProBaumkarte_UWP.Models.Baum
{
    public class Baum
    {
        public int BaumNr { get; set; }

        private Point _canvasPosition;
        public Point CanvasPosition {
            get { return _canvasPosition; }
            set { _canvasPosition = value; X = value.X; Y = value.Y; } }
        public double X { get; set; }
        public double Y { get; set; }
        public Point ImagePosition { get; set; }


        public bool IsMarked { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
