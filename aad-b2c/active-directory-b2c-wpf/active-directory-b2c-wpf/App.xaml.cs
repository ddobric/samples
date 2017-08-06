using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Identity.Client;

namespace active_directory_b2c_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string Tenant = "fabrikamb2c.onmicrosoft.com";
        private static string ClientId = "1f20978a-0fec-46b3-9272-a0f70c3a50f3";
        public static string PolicySignUpSignIn = "b2c_1_susi";
       
        public static string PolicyEditProfile = "b2c_1_myedit";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] ApiScopes = { "https://TODO.onmicrosoft.com/apiapp/admin" };
        public static string ApiEndpoint = "https://TODO.azurewebsites.net/test";

        private static string BaseAuthority = "https://login.microsoftonline.com/tfp/{tenant}/{policy}/oauth2/v2.0/authorize";
        public static string Authority = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicySignUpSignIn);
        public static string AuthorityEditProfile = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyEditProfile);
        public static string AuthorityResetPassword = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyResetPassword);

        private static PublicClientApplication _clientApp = new PublicClientApplication(ClientId, Authority, TokenCacheHelper.GetUserCache());
        
        public static PublicClientApplication PublicClientApp { get { return _clientApp; } }
    }
}
