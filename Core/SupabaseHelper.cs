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

        public static async Task InitSupabase()
        {
            var url = "https://yjdfxteaqjixjnqydiut.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlqZGZ4dGVhcWppeGpucXlkaXV0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTcyMTMwNzcsImV4cCI6MjAzMjc4OTA3N30.RY80K44EDMW68tazaRRwf0UDcmFziTNmeVcb_zFAW8E";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
                Schema = "public",
            };

            supabase = new Supabase.Client(url, key, options);
            await supabase.InitializeAsync();

            supabaseInitialized.Invoke(typeof(SupabaseHelper), EventArgs.Empty);

            Console.WriteLine("Supabase initialized.");

        }

        public static async Task<Boolean> SignInWithGoogle()
        {
            var redirectURI = GetRedirectURI();
            httpListener = CreateRedirectListener(redirectURI);

            // Close HTTPListener when application closes
            Application.Current.MainWindow.Closed += CloseHTTPListenerOnWindowClose;

            var ProviderAuthState = await supabase.Auth.SignIn(Provider.Google, new SignInOptions { RedirectTo = redirectURI, FlowType = OAuthFlowType.PKCE, Scopes = "openid profile email" });
            output($"Auth URL for Browser: {ProviderAuthState.Uri.ToString()}");

            // Opens request in the browser.
            System.Diagnostics.Process.Start(ProviderAuthState.Uri.ToString());

            supabase.Auth.AddStateChangedListener((sender, changed) =>
            {
                switch (changed)
                {
                    case AuthState.SignedIn:
                        output("AuthState Changed: SignedIn");
                        break;
                    case AuthState.SignedOut:
                        output("AuthState Changed: SignedOut");
                        break;
                    case AuthState.UserUpdated:
                        output("AuthState Changed: UserUpdated");
                        break;
                    case AuthState.PasswordRecovery:
                        output("AuthState Changed: PasswordRecovery");
                        break;
                    case AuthState.TokenRefreshed:
                        output("AuthState Changed: SignedIn");
                        break;
                }
            });

            var session = await ListenToCallback(httpListener, ProviderAuthState);

            /////////

            if (session == null)
            {
                return false;
            }

            return true;

        }

        private static async Task<Session> ListenToCallback(HttpListener http, ProviderAuthState providerAuthState) 
        {
            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app to foreground
            Application.Current.MainWindow.Activate();

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

        //private static async Task HandleLogin(LoginResult result)
        //{
        //    var authResponse = await supabase.Auth.SetSession(result.AccessToken, result.RefreshToken);
        //    if (authResponse.User != null)
        //    {
        //        // Update UI with login status
        //        Console.WriteLine($"Logged in as: {authResponse.User.Email}");
        //        // Load the main content or update the UI as needed
        //    }
        //}

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
