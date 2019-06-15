using Microsoft.Graphics.Canvas;
using ProBaumkarte_UWP.Models.Baum;
using ProBaumkarte_UWP.Models.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ProBaumkarte_UWP.Services.Dialog;

namespace ProBaumkarte_UWP.Services.File
{
    public class FileService : IFileService
    {
        private readonly IDialogService _dialogService;

        public FileService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }
        public async Task<MapFile> GetFile()
        {
            //https://github.com/Microsoft/Windows-universal-samples/blob/e13cf5dca497ad661706d150a154830666913be4/Samples/PdfDocument/cs/Scenario1_Render.xaml.cs
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".svg");
            StorageFile file = await picker.PickSingleFileAsync();

            //return file;


            MapFile mapFile = new MapFile();

            if (file.FileType == ".pdf")
            {
                PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(file);


                using (PdfPage page = pdfDocument.GetPage(0))
                {

                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        await page.RenderToStreamAsync(stream);
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);





                        using (var stream2 = new InMemoryRandomAccessStream())
                        {
                            PdfPageRenderOptions options;

                            await page.RenderToStreamAsync(stream2);
                            //mapFile.MapBytes = new byte[bitmap.PixelHeight*bitmap.PixelWidth];
                            mapFile.MapBytes = new byte[(uint)stream2.AsStream().Length];
                            await stream2.AsStreamForRead().ReadAsync(mapFile.MapBytes, 0, mapFile.MapBytes.Length);
                            //WriteableBitmap writableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
                            //await writableBitmap.SetSourceAsync(stream2);
                            //mapFile.MapSoftwareImageSource = writableBitmap;
                        }


                        //
                        //mapFile.MapSoftwareImageSource.SetSource(stream2);
                        //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        //mapFile.MapSoftwareImageSource = await decoder.GetSoftwareBitmapAsync();



                        mapFile.MapImageSource = bitmap;
                    }


                }
            }
            //http://igrali.com/2015/12/24/how-to-render-svg-in-xaml-windows-10-uwp/
            if (file.FileType == ".svg")
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    //SvgImageSource svgImage = new SvgImageSource();
                    //await svgImage.SetSourceAsync(stream);
                    //mapFile.svgImageSource = svgImage;

                    string svgString = await Windows.Storage.FileIO.ReadTextAsync(file);


                    //mapFile.MapImageSource = svgImage;
                    mapFile.MapSvgSource = svgString;
                    mapFile.IsSvg = true;

                }
            }

            if (file.FileType == ".jpg" || file.FileType == ".jpeg" || file.FileType == ".png")
            {

                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    BitmapImage bitmap = new BitmapImage();

                    await bitmap.SetSourceAsync(stream);
                    mapFile.MapImageSource = bitmap;


                }
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    mapFile.MapBytes = new byte[(uint)stream.AsStream().Length];
                    await stream.AsStreamForRead().ReadAsync(mapFile.MapBytes, 0, mapFile.MapBytes.Length);
                }

            }

            return mapFile;
        }

        public async void SaveImage(SoftwareBitmap softwareBitmap)
        {
            //var picker=new Windows.Storage.Pickers.FileSavePicker();
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //fileSavePicker.FileTypeChoices.Add("JPEG Dateien", new List<string>() { ".jpg" });
            fileSavePicker.FileTypeChoices.Add("PNG Dateien", new List<string>() { ".png" });
            fileSavePicker.SuggestedFileName = "Karte_" + DateTime.Now.ToString("dd_MM_yyyy");
            var outputFile = await fileSavePicker.PickSaveFileAsync();

            if (outputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }
            else
            {
                SaveSoftwareBitmapToFile(softwareBitmap, outputFile);
            }

        }

        private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {

                // Create an encoder with the desired format
                //BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                // Set additional encoding parameters, if needed
                //encoder.BitmapTransform.ScaledWidth = 320;
                //encoder.BitmapTransform.ScaledHeight = 240;
                //encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.Clockwise90Degrees;
                //encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    switch (err.HResult)
                    {
                        case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
                                                         // If the encoder does not support writing a thumbnail, then try again
                                                         // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw err;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }


            }
        }


        public async void SaveMap(MapFile mapFile, ObservableCollection<Baum> baumCollection)
        {
            string image = "data:image/png;base64, ";
            try
            {
                image = image + Convert.ToBase64String(mapFile.MapBytes);
            }
            catch (System.NullReferenceException)
            {
                //TODO: Message anzeigen!!!
                return;

            }

            XDocument map = new XDocument();

            XNamespace svg_ns = "http://www.w3.org/2000/svg";
            XNamespace xlink_ns = "http://www.w3.org/1999/xlink";
            XNamespace baum_ns = "http://probaumkontrollen.org/probaumkontrollen";
            XElement root = new XElement(svg_ns + "svg", new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"), new XAttribute(XNamespace.Xmlns + "probaumkontrollen", "http://probaumkontrollen.org/probaumkontrollen"), new XAttribute("width", mapFile.MapImageSource.PixelWidth.ToString()), new XAttribute("height", mapFile.MapImageSource.PixelHeight.ToString()));


            ////Add Map Background
            XElement background_group = new XElement(svg_ns + "g");
            XElement background = new XElement(svg_ns + "image");
            background.Add(new XAttribute(xlink_ns + "href", image));
            background_group.Add(background);

            ////Add trees to the XML
            XElement tree_group = new XElement(svg_ns + "g");
            tree_group.Add(new XAttribute("id", "Baum_Liste"));

            foreach (var baum in baumCollection)
            {
                XElement Baum_Element = new XElement(svg_ns + "g");
                Baum_Element.Add(new XAttribute(baum_ns + "BaumNr", baum.BaumNr));


                XElement baumMarker = new XElement(svg_ns + "circle");
                baumMarker.Add(new XAttribute("cx", baum.ImagePosition.X));
                baumMarker.Add(new XAttribute("cy", baum.ImagePosition.Y));
                baumMarker.Add(new XAttribute("r", mapFile.TreeMarkersize / 2));
                //baumMarker.Add(new XAttribute("fill", "green"));
                string color = "rgb(" +
                    mapFile.TreeMarkerColor.R.ToString() + "," +
                    mapFile.TreeMarkerColor.G.ToString() + "," +
                    mapFile.TreeMarkerColor.B.ToString() + ")";

                baumMarker.Add(new XAttribute("fill", color));
                baumMarker.Add(new XAttribute("fill-opacity", "0.5"));

                Baum_Element.Add(baumMarker);

                XElement baumNr = new XElement(svg_ns + "text");
                baumNr.Add(new XAttribute("x", baum.ImagePosition.X));
                baumNr.Add(new XAttribute("y", baum.ImagePosition.Y));
                baumNr.Add(new XAttribute("font-size", 0.8 * mapFile.TreeMarkersize));
                baumNr.Add(new XAttribute("text-anchor", "middle"));
                baumNr.Add(new XAttribute("alignment-baseline", "central"));
                baumNr.Add(new XText(baum.BaumNr.ToString()));
                Baum_Element.Add(baumNr);


                tree_group.Add(Baum_Element);
            }


            root.Add(background_group);
            root.Add(tree_group);

            map.Add(root);


            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("SVG Dateien", new List<string>() { ".svg" });
            fileSavePicker.SuggestedFileName = "Karte_" + DateTime.Now.ToString("dd_MM_yyyy");

            var file = await fileSavePicker.PickSaveFileAsync();


            if (file != null)
            {
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    fileStream.SetLength(0);
                    map.Save(fileStream);
                }
            }

        }

        public async Task<MapAndTrees> ImportMap()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".svg");
            StorageFile file = await picker.PickSingleFileAsync();

            MapFile mapFile = new MapFile();
            ObservableCollection<Baum> baumCollection = new ObservableCollection<Baum>();
            MapAndTrees mapAndTrees = new MapAndTrees();

            if (file.FileType == ".svg")
            {


                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    XNamespace baum_ns = "http://probaumkontrollen.org/probaumkontrollen";
                    XNamespace svg_ns = "http://www.w3.org/2000/svg";
                    XNamespace xlink_ns = "http://www.w3.org/1999/xlink";

                    string svgString = await Windows.Storage.FileIO.ReadTextAsync(file);
                    try
                    {
                        XDocument xDocument = XDocument.Parse(svgString);

                        //Geting the map
                        XElement imageElement = xDocument.Descendants(svg_ns + "image").FirstOrDefault();
                        string mapString = imageElement.Attribute(xlink_ns + "href").Value;
                        mapString = mapString.Substring(mapString.IndexOf(",") + 1);
                        byte[] mapBytes = Convert.FromBase64String(mapString);

                        using (InMemoryRandomAccessStream imageStream = new InMemoryRandomAccessStream())
                        {
                            await imageStream.WriteAsync(mapBytes.AsBuffer());
                            BitmapImage bitmapImage = new BitmapImage();
                            imageStream.Seek(0);
                            try
                            {
                                await bitmapImage.SetSourceAsync(imageStream);
                            }
                            catch (Exception)
                            {

                                _dialogService.ShowErrorDialog("Die importierte Datei enthält keine gültige Karte.");
                                return null;
                            }

                            mapFile.MapImageSource = bitmapImage;
                            mapFile.MapBytes = mapBytes;
                        }




                        mapFile.IsSvg = false;
                        mapAndTrees.map = mapFile;


                        // Getting the list of trees
                        XElement xBaumliste = xDocument.Descendants().Where(el => el.HasAttributes && el.FirstAttribute.Name.ToString() == "id").FirstOrDefault();
                        ObservableCollection<Baum> baumliste = new ObservableCollection<Baum>();
                        if (!xBaumliste.IsEmpty)
                        {
                            foreach (var node in xBaumliste.Elements().ToList())
                            {
                                XElement xNode = node;
                                Baum baum = new Baum();
                                baum.BaumNr = Convert.ToInt16(node.Attribute(baum_ns + "BaumNr").Value);

                                XElement circleNode = node.Elements(svg_ns + "circle").FirstOrDefault();

                                Windows.Foundation.Point imagePosition;
                                Windows.Foundation.Point canvasPosition;
                                imagePosition.X = double.Parse(circleNode.Attribute("cx").Value, CultureInfo.InvariantCulture);
                                imagePosition.Y = double.Parse(circleNode.Attribute("cy").Value, CultureInfo.InvariantCulture);
                                canvasPosition.X = double.Parse(circleNode.Attribute("cx").Value, CultureInfo.InvariantCulture);
                                canvasPosition.Y = double.Parse(circleNode.Attribute("cy").Value, CultureInfo.InvariantCulture);

                                baum.ImagePosition = imagePosition;
                                baum.CanvasPosition = canvasPosition;
                                baumliste.Add(baum);

                                mapAndTrees.baumListe = baumliste;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        _dialogService.ShowErrorDialog("Die importierte Datei entspricht nicht dem benötigten Format.");
                        return null;
                    }


                }
            }
            return mapAndTrees;

        }
    }
}
