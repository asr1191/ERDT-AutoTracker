using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using static Supabase.Gotrue.Constants;
using Supabase.Gotrue;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace ERDT.Core
{
    internal static class SupabaseHelper
    {
        private const string PROJECT_URL = "https://yjdfxteaqjixjnqydiut.supabase.co";
        private const string PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlqZGZ4dGVhcWppeGpucXlkaXV0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTcyMTMwNzcsImV4cCI6MjAzMjc4OTA3N30.RY80K44EDMW68tazaRRwf0UDcmFziTNmeVcb_zFAW8E";

        public static Supabase.Client supabase { get; set; }
        public static EventHandler supabaseInitialized;
        private static HttpListener httpListener;

        private static void output(string message)
        {
            Console.WriteLine(message);
        }

        private static void output(ICollection<string> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }

        public static void PreInitSupabase()
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
                Schema = "public",
            };

            supabase = new Supabase.Client(PROJECT_URL, PUBLIC_KEY, options);
            supabase.Auth.SetPersistence(new SupabaseSessionPersistence());
       
        }

        public static async Task InitSupabaseAsync()
        {
            await supabase.InitializeAsync();
            await setupNetworkStatus();

            supabaseInitialized.Invoke(typeof(SupabaseHelper), EventArgs.Empty);

            supabase.Auth.LoadSession();
            await supabase.Auth.RetrieveSessionAsync();

            Console.WriteLine("Supabase initialized.");
        }


        private static async Task setupNetworkStatus()
        {
            var status = new NetworkStatus();
            status.Client = supabase.Auth;
            var testURL = "https://8.8.8.8";
            supabase.Auth.Online = await status.StartAsync(testURL);
        }

        public static async Task<Boolean> SignInWithGoogle(Window mainWindow)
        {
            var redirectURI = GetRedirectURI();
            httpListener = CreateRedirectListener(redirectURI);

            // Close HTTPListener when application closes
            mainWindow.Closed += CloseHTTPListenerOnWindowClose;

            var ProviderAuthState = await supabase.Auth.SignIn(Provider.Google, new SignInOptions { RedirectTo = redirectURI, FlowType = OAuthFlowType.PKCE, Scopes = "openid profile email" });
            output($"Auth URL for Browser: {ProviderAuthState.Uri}");

            // Opens request in the browser.
            System.Diagnostics.Process.Start(ProviderAuthState.Uri.ToString());

            var session = await ListenToCallback(httpListener, ProviderAuthState, mainWindow);

            /////////

            if (session == null)
            {
                return false;
            }

            return true;

        }

        private static async Task<Session> ListenToCallback(HttpListener http, ProviderAuthState providerAuthState, Window mainWindow) 
        {
            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app to foreground
            Application.Current.Dispatcher.Invoke(() => mainWindow.Activate());

            var response = context.Response;
            var request = context.Request;

            if (request.HttpMethod == "GET")
            {
                var responseString = string.Format("<html><body>Please return to the app.</body></html>");
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                var session = await supabase.Auth.ExchangeCodeForSession(providerAuthState.PKCEVerifier, request.QueryString.Get("code"));
                return session;
            }
            else
            {
                response.StatusCode = 404;
                response.Close();

                return null;
            }
        }

        private static void CloseHTTPListenerOnWindowClose(object sender, EventArgs e) 
        {
            httpListener.Stop();
            httpListener.Close();
        }

        private static string GetRedirectURI()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            var redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, port);
            output("redirect URI: " + redirectURI);
            return redirectURI;
        }

        private static HttpListener CreateRedirectListener(string redirectURI)
        {
            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            output("Listening..");
            http.Start();

            return http;
        }

        
    }
}
