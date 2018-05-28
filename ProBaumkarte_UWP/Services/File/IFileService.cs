using Microsoft.Graphics.Canvas;
using ProBaumkarte_UWP.Models.Baum;
using ProBaumkarte_UWP.Models.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace ProBaumkarte_UWP.Services.File
{
    public interface IFileService
    {
        Task<MapFile> GetFile();
        void SaveImage(SoftwareBitmap softwareBitmap);
        void SaveMap(MapFile mapFile, ObservableCollection<Baum> baumCollection);

    }
}
