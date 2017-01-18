using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

namespace PhilipsHueIoTGateway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static DeviceClient m_DeviceClient;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private static void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //var connStr = "TODO";

            //m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr);

            //runMethodListener();
        }

        //private static Task<MethodResponse> routeRequest(MethodRequest request, object args)
        //{
        //    string jsonResp = JsonConvert.SerializeObject(new
        //    {
        //        DeviceStatus = "on",
        //    });

        //    MethodResponse resp = new MethodResponse(Encoding.UTF8.GetBytes(jsonResp), 200);

        //    return Task.FromResult<MethodResponse>(resp);
        //}

        //private async static void runMethodListener()
        //{
        //    MethodCallback methodCallback = new MethodCallback(routeRequest);

        //    await m_DeviceClient.OpenAsync();

        //    m_DeviceClient.SetMethodHandler("RouteRequest", methodCallback, "context");
        //}
    }
}
