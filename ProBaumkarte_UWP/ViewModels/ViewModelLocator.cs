using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ProBaumkarte_UWP.Services.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBaumkarte_UWP.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary> 
    /// //http://www.joeljoseph.net/navigation-between-pages-using-mvvm-light-in-universal-windows-platform-uwp/
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            var dialog = new ProBaumkarte_UWP.Services.Dialog.DialogService();
            var file = new FileService(dialog);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            //Register your services used here
            SimpleIoc.Default.Register<INavigationService,NavigationService>();
            SimpleIoc.Default.Register<StartPageViewModel>();
            SimpleIoc.Default.Unregister<IFileService>();
            SimpleIoc.Default.Register<IFileService>(() => file);
            SimpleIoc.Default.Unregister<ProBaumkarte_UWP.Services.Dialog.IDialogService>();
            SimpleIoc.Default.Register<ProBaumkarte_UWP.Services.Dialog.IDialogService>(() => dialog);
        }


        // <summary>
        // Gets the StartPage view model.
        // </summary>
        // <value>
        // The StartPage view model.
        // </value>
        public StartPageViewModel StartPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartPageViewModel>();
            }
        }

        // <summary>
        // The cleanup.
        // </summary>
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }

}
