using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Graphics.Canvas;
using ProBaumkarte_UWP.Models.Baum;
using ProBaumkarte_UWP.Models.Map;
using ProBaumkarte_UWP.Services.Dialog;
using ProBaumkarte_UWP.Services.File;
using ProBaumkarte_UWP.Services.Messages;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ProBaumkarte_UWP.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {

        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        

        public RelayCommand OpenMapCommand { get; private set; }
        public RelayCommand SaveMapCommand { get; private set; }
        public RelayCommand ExportMapCommand { get; private set; }
        public RelayCommand ImportMapCommand { get; private set; }
        public RelayCommand NavigateCommand { get; private set; }
        public RelayCommand SetBaumCommand { get; private set; }
        public RelayCommand MoveBaumCommand { get; private set; }
        public RelayCommand SelectBaumCommand { get; private set; }
        public RelayCommand DeleteBaumCommand { get; private set; }
        public RelayCommand PrintCommand { get; private set; }


        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");

            }
        }
        private string _title;
        public string Title
        {

            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }


        private MapFile _mapSource;
        public MapFile MapSource
        {
            get { return _mapSource; }
            set
            {
                if (value != _mapSource)
                {
                    _mapSource = value;
                    _mapSource.TreeMarkersize = _treeMarkerSize;
                    RaisePropertyChanged("MapSource");
                    
                }
            }
        }

        private ImageSource _mapBitmapSource;
        public ImageSource MapBitmapSource
        {
            get { return _mapBitmapSource; }
            set
            {
                if (value != _mapBitmapSource)
                {
                    _mapBitmapSource = value;
                    RaisePropertyChanged("MapBitmapSource");
                }
            }
        }

        private string _mapSvgSource;
        public string MapSvgSource
        {
            get { return _mapSvgSource; }
            set
            {
                if (value != _mapSvgSource)
                {
                    _mapSvgSource = value;
                    RaisePropertyChanged("MapSvgSource");
                }
            }
        }

        private SoftwareBitmap _renderedMapBitmapSource;
        public SoftwareBitmap RenderedMapBitmapSource
        {
            get { return _renderedMapBitmapSource; }
            set
            {
                if (value != _renderedMapBitmapSource)
                {
                    _renderedMapBitmapSource = value;
                    RaisePropertyChanged("RenderedMapBitmapSource");
                }
            }
        }

        private int _mapMode;
        public int MapMode
        {
            get { return _mapMode; }
            set
            {
                if (value != _mapMode)
                {
                    _mapMode = value;
                    RaisePropertyChanged(()=>MapMode);
                }
            }
        }

        private bool _settingsOpen;
        public bool SettingsOpen
        {
            get { return _settingsOpen; }
            set
            {
                if (value != _settingsOpen)
                {
                    _settingsOpen = value;
                    RaisePropertyChanged(() => SettingsOpen);
                }
            }
        }

        private bool _test;
        public bool Test
        {
            get { return _test; }
            set
            {
                if (value != _test)
                {
                    _test = value;
                    RaisePropertyChanged("Test");
                }
            }
        }

        private Baum _currentBaum;
        public Baum CurrentBaum
        {
            get { return _currentBaum; }
            set
            {
                if (value != _currentBaum)
                {
                    _currentBaum = value;
                    RaisePropertyChanged(() => CurrentBaum);
                }
            }
        }

        private Baum _selectedBaum;
        public Baum SelectedBaum
        {
            get { return _selectedBaum; }
            set
            {
                if (value != _selectedBaum)
                {
                    _selectedBaum = value;
                    RaisePropertyChanged(() => SelectedBaum);
                }
            }
        }

        private ObservableCollection<Baum> _BaumCollection;
        public ObservableCollection<Baum> BaumCollection
        {
            get { return _BaumCollection; }
            set
            {
                
                if (value != _BaumCollection)
                {
                    _BaumCollection = value;
                    if (BaumCollection.Count > 0)
                    {
                        if (BaumCollection.Where(x => x.IsMarked == true).Count()>0)
                        {
                            SelectedBaum = BaumCollection.Where(x => x.IsMarked == true).First();
                        }
                        
                        //_selectedBaum = _BaumCollection.Where(x => x.IsMarked = true).First();
                        //RaisePropertyChanged(() => SelectedBaum);
                    }

                    RaisePropertyChanged(() => BaumCollection);
                }
            }
        }

        private double _treeMarkerSize;
        public double TreeMarkerSize
        {
            get { return _treeMarkerSize; }
            set
            {
                if (value != _treeMarkerSize)
                {
                    _treeMarkerSize = value;
                    if (MapSource!=null)
                    {
                        MapSource.TreeMarkersize = value;
                        RaisePropertyChanged(() => MapSource);
                    }
                    RaisePropertyChanged(() => TreeMarkerSize);
                }
            }
        }
        


        public StartPageViewModel(IFileService fileService,IDialogService dialogService)
        {
            Title = "ProBaumkarte";

            _fileService = fileService;
            _dialogService = dialogService;
            
            MapMode = 3;

            BaumCollection = new ObservableCollection<Baum>();
            CurrentBaum = new Baum { BaumNr = 1 };
            TreeMarkerSize = 10;
            RaisePropertyChanged(() => BaumCollection);



            OpenMapCommand = new RelayCommand(OpenMapCommandAction);
            SaveMapCommand = new RelayCommand(SaveMapCommandAction);
            ExportMapCommand = new RelayCommand(ExportMapCommandAction);
            ImportMapCommand = new RelayCommand(ImportMapCommandAction);
            NavigateCommand = new RelayCommand(NavigateCommandAction);
            SetBaumCommand = new RelayCommand(SetBaumCommandAction);
            MoveBaumCommand = new RelayCommand(MoveBaumCommandAction);
            SelectBaumCommand = new RelayCommand(SelectBaumCommandAction);
            DeleteBaumCommand = new RelayCommand(DeleteBaumCommandAction);



            Messenger.Default.Register<SetBaumMessage>(this, message =>
            {
                UpdateCurrentBaum();
            });
        }







        //https://stackoverflow.com/questions/37179815/displaying-a-background-image-on-a-uwp-ink-canvas

        private async void OpenMapCommandAction()
        {
            

            //Open the Map open dialog
            try
            {
                MapFile mapFile = await _fileService.GetFile();
                MapSource = mapFile;

                //if(await _dialogService.ShowGeoDataQuestionDialog())
                //{
                //    MapGeoData mapGeoData= await _dialogService.ShowGeoDataEntryDialog();
                //    mapFile.LatitudeTop = mapGeoData.LatitudeTop;
                //    mapFile.LatitudeBottom = mapGeoData.LatitudeBottom;
                //    mapFile.LongitudeLeft = mapGeoData.LongitudeLeft;
                //    mapFile.LongitudeRight = mapGeoData.LongitudeRight;
                //    mapFile.IsGeoReferenced = true;
                //}
                mapFile.IsGeoReferenced = false;
                BaumCollection.Clear();
                CurrentBaum.BaumNr = 1;
                RaisePropertyChanged("CurrentBaum");
                SelectedBaum = null;
                
            }
            catch (NullReferenceException)
            {


            }


        }

        private void ProcessSavedImage(SoftwareBitmap softwareBitmap)
        {
            _fileService.SaveImage(softwareBitmap);

        }
        private void ExportMapCommandAction()
        {
            //if (BaumCollection.Count > 0)
            //{
            //    if (BaumCollection.Where(x => x.IsMarked == true).Count() > 0)
            //    {
            //       BaumCollection.Where(x => x.IsMarked == true).First().IsMarked=false;
            //    }

            //    //_selectedBaum = _BaumCollection.Where(x => x.IsMarked = true).First();
            //    //RaisePropertyChanged(() => SelectedBaum);
            //}

            Messenger.Default.Send<NotificationMessageAction<SoftwareBitmap>>(new NotificationMessageAction<SoftwareBitmap>("",ProcessSavedImage));

        }

        private async void ImportMapCommandAction()
        {
            try
            {
                MapAndTrees mapAndTrees = await _fileService.ImportMap();
                if (mapAndTrees != null)
                {
                    BaumCollection = mapAndTrees.baumListe;
                    MapSource = mapAndTrees.map;
                }
            }
            catch (Exception)
            {
                _dialogService.ShowErrorDialog("Etwas ist beim importieren schief gelaufen.");
                
            }
           


        }

        private async void SaveMapCommandAction()
        {
            _fileService.SaveMap(MapSource, BaumCollection);
        }

        private void NavigateCommandAction()
        {
            MapMode = 3;
            RaisePropertyChanged(() => MapMode);
        }

        private void SelectBaumCommandAction()
        {
            MapMode = 0;
            RaisePropertyChanged(() => MapMode);
        }

        private void SetBaumCommandAction()
        {
            MapMode = 1;
            RaisePropertyChanged(() => MapMode);
        }

        private async void MoveBaumCommandAction()
        {
            MapMode = 2;
            RaisePropertyChanged(() => MapMode);
            
        }

        private async void DeleteBaumCommandAction()
        {
            Messenger.Default.Send<DeleteBaumMessage>(new DeleteBaumMessage());
        }


        private void UpdateCurrentBaum()
        {
            //_BaumCollection.Add(CurrentBaum);
            CurrentBaum = new Baum { BaumNr = CurrentBaum.BaumNr+1 };           
            RaisePropertyChanged("CurrentBaum");
        }




    }
}
