using System;
using System.Threading.Tasks;
using System.Windows;
using ERDT.Core;

namespace ERDT.MVVM.ViewModel
{
    internal class LoginViewModel : ObservableObject    
    {
        public RelayCommand LoginButtonClickHandler { get; set; }

        private bool _isLoginButtonEnabled = false;
        public Boolean IsLoginButtonEnabled
        {
            get => _isLoginButtonEnabled;
            set
            {
                _isLoginButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private void OnSupabaseInitialized(object sender, EventArgs e)
        {
            IsLoginButtonEnabled = true;
        }

        private void LoginButtonClickHandler_Execute(object commandParameter)
        {
            Console.WriteLine("In Login Button Click");
            var mainWindow = Application.Current.MainWindow;
            Task.Run(() => SupabaseHelper.SignInWithGoogle(mainWindow));
        }

        public LoginViewModel() 
        {
            LoginButtonClickHandler = new RelayCommand(LoginButtonClickHandler_Execute);
            SupabaseHelper.supabaseInitialized += OnSupabaseInitialized;

            SupabaseHelper.supabase.Auth.AddStateChangedListener((sender, state) =>
            {
                switch (state)
                {
                    case Supabase.Gotrue.Constants.AuthState.SignedIn:
                        IsLoginButtonEnabled = false;
                        break;

                    case Supabase.Gotrue.Constants.AuthState.SignedOut:
                        IsLoginButtonEnabled = true; 
                        break;

                    case Supabase.Gotrue.Constants.AuthState.UserUpdated:
                        IsLoginButtonEnabled = false;
                        Console.WriteLine("Login Button Disabled");
                        break;
                }
            });
        }

    }

}
