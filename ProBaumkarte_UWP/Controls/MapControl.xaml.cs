using GalaSoft.MvvmLight.Messaging;

using ProBaumkarte_UWP.Models.Map;
using ProBaumkarte_UWP.Services.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using ProBaumkarte_UWP.Models.Baum;
using System.Collections.ObjectModel;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProBaumkarte_UWP.Controls
{
    public sealed partial class MapControl : UserControl
    {

        // For this example, we track simultaneous contacts in case the 
        // number of contacts has reached the maximum supported by the device.
        // Depending on the device, additional contacts might be ignored 
        // (PointerPressed not fired). 
        uint numActiveContacts;
        Windows.Devices.Input.TouchCapabilities touchCapabilities = new Windows.Devices.Input.TouchCapabilities();

        // Dictionary to maintain information about each active contact. 
        // An entry is added during PointerPressed/PointerEntered events and removed 
        // during PointerReleased/PointerCaptureLost/PointerCanceled/PointerExited events.
        Dictionary<uint, Windows.UI.Xaml.Input.Pointer> contacts;

        public MapControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            //Messenger.Default.Register<RenderImageMessage>(this, message =>
            //{
            //    RenderImage();
            //});

            Messenger.Default.Register<DrawOnImageMessage>(this, message =>
            {
                DrawOnImage();
            });

            Messenger.Default.Register<DeleteBaumMessage>(this, message =>
            {
                DeleteTree();
            });

            Messenger.Default.Register<NotificationMessageAction<SoftwareBitmap>>(this, RenderImage);

            //Handling Touch input
            numActiveContacts = 0;
            // Initialize the dictionary.
            contacts = new Dictionary<uint, Windows.UI.Xaml.Input.Pointer>((int)touchCapabilities.Contacts);
            // Declare the pointer event handlers.
            //MapCanvas.PointerPressed += new PointerEventHandler(MapCanvas_PointerPressed);
            //MapCanvas.PointerEntered += new PointerEventHandler(MapCanvas_PointerEntered);
            //MapCanvas.PointerReleased += new PointerEventHandler(MapCanvas_PointerReleased);
            //MapCanvas.PointerExited += new PointerEventHandler(MapCanvas_PointerExited);
            //MapCanvas.PointerCanceled += new PointerEventHandler(MapCanvas_PointerCanceled);
            //MapCanvas.PointerCaptureLost += new PointerEventHandler(MapCanvas_PointerCaptureLost);
            //MapCanvas.PointerMoved += new PointerEventHandler(MapCanvas_PointerMoved);
            //MapCanvas.PointerWheelChanged += new PointerEventHandler(MapCanvas_PointerWheelChanged);


           
            treeMarkerSize = 10;

            
        }

        //https://docs.microsoft.com/de-de/windows/uwp/input-and-devices/handle-pointer-input
        // PointerPressed and PointerReleased events do not always occur in pairs. 
        // Your app should listen for and handle any event that might conclude a pointer down action 
        // (such as PointerExited, PointerCanceled, and PointerCaptureLost).
        // For this example, we track the number of contacts in case the 
        // number of contacts has reached the maximum supported by the device.
        // Depending on the device, additional contacts might be ignored 
        // (PointerPressed not fired). 
        void MapCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            // Lock the pointer to the MapCanvas.
            MapCanvas.CapturePointer(e.Pointer);

            if ((MapModeEnum)MapMode==MapModeEnum.Set)
            {
                SetTree(e.GetCurrentPoint(mapImage),e.GetCurrentPoint(MapCanvas));
            }

            if ((MapModeEnum)MapMode == MapModeEnum.Move)
            {
                MoveTree(e.GetCurrentPoint(mapImage), e.GetCurrentPoint(MapCanvas));
            }
            // Check if pointer already exists (for example, enter occurred prior to press).
            if (contacts.ContainsKey(ptr.PointerId))
            {
                return;
            }
            // Add contact to dictionary.
            contacts[ptr.PointerId] = ptr;
            ++numActiveContacts;

            
        }

        void MapCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            // If event source is mouse or touchpad and the pointer is still 
            // over the MapCanvas, retain pointer and pointer details.
            // Return without removing pointer from pointers dictionary.
            // For this example, we assume a maximum of one mouse pointer.
            if (ptr.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                // Update MapCanvas UI.


                // Remove contact from dictionary.
                if (contacts.ContainsKey(ptr.PointerId))
                {
                    contacts[ptr.PointerId] = null;
                    contacts.Remove(ptr.PointerId);
                    --numActiveContacts;
                }

                // Release the pointer from the MapCanvas.
                MapCanvas.ReleasePointerCapture(e.Pointer);


                // Prevent most handlers along the event route from handling the same event again.
                e.Handled = true;
            }
            else
            {
                
            }

        }

        private void MapCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            // Multiple, simultaneous mouse button inputs are processed here.
            // Mouse input is associated with a single pointer assigned when 
            // mouse input is first detected. 
            // Clicking additional mouse buttons (left, wheel, or right) during 
            // the interaction creates secondary associations between those buttons 
            // and the pointer through the pointer pressed event. 
            // The pointer released event is fired only when the last mouse button 
            // associated with the interaction (not necessarily the initial button) 
            // is released. 
            // Because of this exclusive association, other mouse button clicks are 
            // routed through the pointer move event.          
            if (ptr.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                // To get mouse state, we need extended pointer details.
                // We get the pointer info through the getCurrentPoint method
                // of the event argument. 
                Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(MapCanvas);
                if (ptrPt.Properties.IsLeftButtonPressed)
                {
                    
                }
                if (ptrPt.Properties.IsMiddleButtonPressed)
                {
                    
                }
                if (ptrPt.Properties.IsRightButtonPressed)
                {
                    
                }
            }

            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;
        }

        private void MapCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            if (contacts.Count == 0)
            {

            }

            // Check if pointer already exists (if enter occurred prior to down).
            if (contacts.ContainsKey(ptr.PointerId))
            {
                return;
            }

            // Add contact to dictionary.
            contacts[ptr.PointerId] = ptr;
            ++numActiveContacts;

            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

        }

        private void MapCanvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            

            // Check if pointer already exists (for example, enter occurred prior to wheel).
            if (contacts.ContainsKey(ptr.PointerId))
            {
                return;
            }

            // Add contact to dictionary.
            contacts[ptr.PointerId] = ptr;
            ++numActiveContacts;

            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;
        }

        // Fires for for various reasons, including: 
        //    - User interactions
        //    - Programmatic capture of another pointer
        //    - Captured pointer was deliberately released
        // PointerCaptureLost can fire instead of PointerReleased. 
        private void MapCanvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            // Remove contact from dictionary.
            if (contacts.ContainsKey(ptr.PointerId))
            {
                contacts[ptr.PointerId] = null;
                contacts.Remove(ptr.PointerId);
                --numActiveContacts;
            }

            if (contacts.Count == 0)
            {
                
            }
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;
        }

        // Fires for for various reasons, including: 
        //    - Touch contact canceled by pen coming into range of the surface.
        //    - The device doesn&#39;t report an active contact for more than 100ms.
        //    - The desktop is locked or the user logged off. 
        //    - The number of simultaneous contacts exceeded the number supported by the device.
        private void MapCanvas_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;


            // Remove contact from dictionary.
            if (contacts.ContainsKey(ptr.PointerId))
            {
                contacts[ptr.PointerId] = null;
                contacts.Remove(ptr.PointerId);
                --numActiveContacts;
            }


            if (contacts.Count == 0)
            {
                
            }
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;
        }

        private void MapCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;

            // Remove contact from dictionary.
            if (contacts.ContainsKey(ptr.PointerId))
            {
                contacts[ptr.PointerId] = null;
                contacts.Remove(ptr.PointerId);
                --numActiveContacts;
            }


            if (contacts.Count == 0)
            {
                
            }

            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;
        }



        private void DrawOnImage()
        {
        }

        
        private async void RenderImage(NotificationMessageAction<SoftwareBitmap> notificationMessageAction)
        {
            if (BaumCollection.Count > 0)
            {
                if (BaumCollection.Where(x => x.IsMarked == true).Count() > 0)
                {
                    SelectTree(BaumCollection.Where(x => x.IsMarked == true).First());
                }

                //_selectedBaum = _BaumCollection.Where(x => x.IsMarked = true).First();
                //RaisePropertyChanged(() => SelectedBaum);
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            
            await renderTargetBitmap.RenderAsync(mapGrid);

            SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight);
            softwareBitmap.DpiX = 600;
            softwareBitmap.DpiY = 600;
            softwareBitmap.CopyFromBuffer(await renderTargetBitmap.GetPixelsAsync());

            notificationMessageAction.Execute(softwareBitmap);




            //BitmapDecoder imagedecoder;
            //using (var imagestream = await Map.OpenAsync(FileAccessMode.Read))
            //{
            //    imagedecoder = await BitmapDecoder.CreateAsync(imagestream);

            //    CanvasDevice device = CanvasDevice.GetSharedDevice();
            //    CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, imagedecoder.PixelWidth, imagedecoder.PixelHeight, 96);
            //    using (var ds = renderTarget.CreateDrawingSession())
            //    {
            //        ds.Clear(Colors.White);
            //        CanvasBitmap image = await CanvasBitmap.LoadAsync(device, imagestream);
            //        ds.DrawImage(image);
            //        //ds.DrawText(lblName.Text, new System.Numerics.Vector2(150, 150), Colors.Black);
            //    }

            //    await renderTarget.SaveAsync(imagestream, CanvasBitmapFileFormat.Jpeg);

            //    BitmapImage bitmap = new BitmapImage();
            //    bitmap.SetSource(imagestream);

            //    MapR = bitmap;
            //}
        }

        public static readonly DependencyProperty MapSourceProperty = DependencyProperty.Register("MapSource", typeof(MapFile), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnMapSourceChanged)));

        private async static void OnMapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapFile mapFile = e.NewValue as MapFile;

            if (mapFile.IsSvg)
            {
                MapControl mapcontrol = d as MapControl;
                mapcontrol.mapImage.Visibility = Visibility.Collapsed;
                mapcontrol.webView.Visibility = Visibility.Visible;
                mapcontrol.webView.NavigateToString(mapFile.MapSvgSource);
                await Task.Delay(500);
                //mapcontrol.RedrawMap();
                //mapcontrol.webView.LoadCompleted += async (s, args) => await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });

                //await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });

                //var innerText = await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });
                //var innerText = await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.clientHeight.ToString()" });
                //var innerText = await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "windwow.innerHeight.ToString()" });


                mapcontrol.webView.Refresh();

                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(mapcontrol.webView);
                mapcontrol.mapImage.Source = renderTargetBitmap;


                //WebViewBrush b = new WebViewBrush();
                //b.SetSource(mapcontrol.webView);

                //b.Redraw();
                ////mapcontrol.webRect.Fill = b;
                //mapcontrol.webRect.Background = b;

                mapcontrol.webView.Visibility = Visibility.Collapsed;

                mapcontrol.mapImage.Visibility = Visibility.Visible;

                //mapcontrol.webRect.Visibility = Visibility.Visible;
            }
            else
            {
                MapControl mapcontrol = d as MapControl;
                mapcontrol.mapImage.Source = mapFile.MapImageSource;
                mapcontrol.webView.Visibility = Visibility.Collapsed;
                mapcontrol.mapImage.Visibility = Visibility.Visible;
            }
        }

        public MapFile MapSource
        {
            get { return (MapFile)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }

        //public static readonly DependencyProperty MapBitmapSourceProperty = DependencyProperty.Register("MapBitmapSource", typeof(ImageSource), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnMapBitmapSourceChanged)));

        //private static void OnMapBitmapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    MapControl mapcontrol = d as MapControl;
        //    mapcontrol.mapImage.Source = mapcontrol.MapBitmapSource;
        //    mapcontrol.webView.Visibility = Visibility.Collapsed;
        //    mapcontrol.mapImage.Visibility = Visibility.Visible;

            
            
        //}

        //public ImageSource MapBitmapSource
        //{
        //    get { return (ImageSource)GetValue(MapBitmapSourceProperty); }
        //    set { SetValue(MapBitmapSourceProperty, value);}
        //}



        //public static readonly DependencyProperty MapSvgSourceProperty = DependencyProperty.Register("MapSvgSource", typeof(string), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnMapSvgSourceChanged)));

        //private async static void OnMapSvgSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    MapControl mapcontrol = d as MapControl;
        //    mapcontrol.mapImage.Visibility = Visibility.Collapsed;
        //    mapcontrol.webView.Visibility = Visibility.Visible;
        //    mapcontrol.webView.NavigateToString(mapcontrol.MapSvgSource);
        //    await Task.Delay(500);
        //    //mapcontrol.RedrawMap();
        //    //mapcontrol.webView.LoadCompleted += async (s, args) => await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });

        //    //await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });

        //    var innerText= await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.style.width='100px'" });
        //    //var innerText = await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "document.body.clientHeight.ToString()" });
        //    //var innerText = await mapcontrol.webView.InvokeScriptAsync("eval", new string[] { "windwow.innerHeight.ToString()" });


        //    mapcontrol.webView.Refresh();

        //    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
        //    await renderTargetBitmap.RenderAsync(mapcontrol.webView);
        //    mapcontrol.mapImage.Source = renderTargetBitmap;


        //    //WebViewBrush b = new WebViewBrush();
        //    //b.SetSource(mapcontrol.webView);
            
        //    //b.Redraw();
        //    ////mapcontrol.webRect.Fill = b;
        //    //mapcontrol.webRect.Background = b;
            
        //    mapcontrol.webView.Visibility = Visibility.Collapsed;

        //    mapcontrol.mapImage.Visibility = Visibility.Visible;

        //    //mapcontrol.webRect.Visibility = Visibility.Visible;
        //}

        //public string MapSvgSource
        //{
        //    get { return (string)GetValue(MapSvgSourceProperty); }
        //    set { SetValue(MapSvgSourceProperty, value); }
        //}

        public static readonly DependencyProperty RenderedMapBitmapSourceProperty = DependencyProperty.Register("RenderedMapBitmapSource", typeof(SoftwareBitmap), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnRenderedMapBitmapSourceChanged)));

        private static void OnRenderedMapBitmapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public SoftwareBitmap RenderedMapBitmapSource
        {
            get { return (SoftwareBitmap)GetValue(RenderedMapBitmapSourceProperty); }
            set { SetValue(RenderedMapBitmapSourceProperty, value); }
        }

        public static readonly DependencyProperty MapModeProperty = DependencyProperty.Register("MapMode", typeof(int), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnMapModeChanged)));

        private static void OnMapModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
       
        public int MapMode
        {
            get { return (int)GetValue(MapModeProperty); }
            set { SetValue(MapModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentBaumProperty = DependencyProperty.Register("CurrentBaum", typeof(Baum), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentBaumChanged)));

        private static void OnCurrentBaumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public Baum CurrentBaum
        {
            get { return (Baum)GetValue(CurrentBaumProperty); }
            set { SetValue(CurrentBaumProperty, value); }
        }

        public static readonly DependencyProperty BaumCollectionProperty = DependencyProperty.Register("BaumCollection", typeof(ObservableCollection<Baum>), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnBaumCollectionChanged)));

        private static void OnBaumCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public ObservableCollection<Baum> BaumCollection
        {
            get { return (ObservableCollection<Baum>)GetValue(BaumCollectionProperty); }
            set { SetValue(BaumCollectionProperty, value); }
        }

        public static readonly DependencyProperty TreeMarkerSizeProperty = DependencyProperty.Register("TreeMarkerSize", typeof(double), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnTreeMarkerSizeChanged)));

        private static void OnTreeMarkerSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public double TreeMarkerSize
        {
            get { return (double)GetValue(TreeMarkerSizeProperty); }
            set { SetValue(TreeMarkerSizeProperty, value); }
        }

        public double treeMarkerSize;

        private void SetTree(PointerPoint imagePoint, PointerPoint canvasPoint)
        {
            //var ellipse1 = new Ellipse();
            //ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.ForestGreen);
            //ellipse1.Width = TreeMarkerSize;
            //ellipse1.Height = TreeMarkerSize;
            //ellipse1.Opacity = 0.5;

            //ellipse1.PointerPressed += OnTreePointerPressed;

            //Canvas.SetLeft(ellipse1, pointerPoint.Position.X - (ellipse1.Width / 2));
            //Canvas.SetTop(ellipse1, pointerPoint.Position.Y - (ellipse1.Height / 2));
            //MapCanvas.Children.Add(ellipse1);

            //var textBlock = new TextBlock();
            //textBlock.IsHitTestVisible = false;
            //textBlock.FontStretch = Windows.UI.Text.FontStretch.UltraCondensed;
            //textBlock.Text = CurrentBaum.BaumNr.ToString();
            //textBlock.Opacity = 0.5;
            //textBlock.FontSize = 0.9*TreeMarkerSize;
            //textBlock.TextAlignment = TextAlignment.Center;

            ////textBlock.Height = textBlock.FontSize*1.1;
            ////textBlock.Width = textBlock.Text.Length * textBlock.FontSize*1.1;
            //textBlock.Height = TreeMarkerSize;
            //textBlock.Width = textBlock.Text.Length * TreeMarkerSize;
            //Canvas.SetLeft(textBlock, pointerPoint.Position.X - (textBlock.Width/2));
            //Canvas.SetTop(textBlock, pointerPoint.Position.Y - (textBlock.Height*1.2/2));

            //MapCanvas.Children.Add(textBlock);
            
            Baum neuerBaum = new Baum();
            neuerBaum.CanvasPosition = canvasPoint.Position;
            neuerBaum.ImagePosition = imagePoint.Position;
    
            neuerBaum.BaumNr = CurrentBaum.BaumNr;
            BaumCollection.Add(neuerBaum);
            SelectTree(neuerBaum);

            double i = mapImage.DesiredSize.Height;
            

            Messenger.Default.Send<SetBaumMessage>(new SetBaumMessage());
        }

        private void MoveTree(PointerPoint imagePoint,PointerPoint canvasPoint)
        {
            if (BaumCollection!=null)
            {
                Baum selectedTree = (Baum)BaumCollection.Where(x => x.IsMarked == true).FirstOrDefault();

                if (selectedTree != null)
                {
                    selectedTree.CanvasPosition = canvasPoint.Position;
                    selectedTree.ImagePosition = imagePoint.Position;

                    BaumCollection.Remove(selectedTree);
                    BaumCollection.Add(selectedTree);
                }
            }
 
        }

        private void SelectTree(Baum tree)
        {
            
            ObservableCollection<Baum> localBaumCollection = new ObservableCollection<Baum>();



            if (!tree.IsMarked)
            {
                foreach (var item in BaumCollection)
                {
                    item.IsMarked = false;
                    localBaumCollection.Add(item);
                }

                tree.IsMarked = true;
                localBaumCollection.Remove(tree);
                localBaumCollection.Add(tree);
            }
            else
            {
                foreach (var item in BaumCollection)
                {
                    item.IsMarked = false;
                    localBaumCollection.Add(item);
                }
            }
            
           

            BaumCollection = localBaumCollection;
        }

        private void DeleteTree()
        {
            Baum baumToDelete = BaumCollection.Where(x => x.IsMarked == true).FirstOrDefault();
            if (baumToDelete!=null)
            {
                BaumCollection.Remove(baumToDelete);
            }
            else
            {

            }
  
        }

        private void OnTreePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if ((MapModeEnum)MapMode==MapModeEnum.Select|| (MapModeEnum)MapMode == MapModeEnum.Move)
            {               
                Border treeMarker = sender as Border;
                Baum tree = treeMarker.DataContext as Baum;
                SelectTree(tree);
            }
        }

        public  double CorrectTreePosition(double point)
        {
            return point - treeMarkerSize / 2;
        }

        private void Scroll_ViewChanged(object sender, ScrollViewerViewChangingEventArgs e)
        {
   
        }
    }
}
