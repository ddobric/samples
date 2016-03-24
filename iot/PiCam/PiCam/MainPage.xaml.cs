using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiCam
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PiCam myCamApi;

        public MainPage()
        {
            this.InitializeComponent();

            try
            {
                Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Run()
        {

            myCamApi = new PiCam(captureImage, previewElement, playbackElement);
            await myCamApi.Init();
            await myCamApi.TakePhoto();

        }
    }
}
