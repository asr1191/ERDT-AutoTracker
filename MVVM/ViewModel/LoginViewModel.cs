using System;
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


        private async void LoginButtonClickHandler_Execute(object commandParameter)
        {
            Console.WriteLine("In Login Button Click");
            IsLoginButtonEnabled = false;

            IsLoginButtonEnabled = !await SupabaseHelper.SignInWithGoogle();
        }

        public LoginViewModel() 
        {
            LoginButtonClickHandler = new RelayCommand(LoginButtonClickHandler_Execute);
            SupabaseHelper.supabaseInitialized += OnSupabaseInitialized;
        }

        private void OnSupabaseInitialized(object sender, EventArgs e)
        {
            IsLoginButtonEnabled = true;
        }
    }

}
