using System;
using System.Threading.Tasks;
using System.Windows;
using ERDT.Core;

namespace ERDT.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public EventHandler UserSignedIn;
        public EventHandler UserUpdated;

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SavefileViewCommand { get; set; }
        public RelayCommand CharacterViewCommand { get; set; }

        public LoginViewModel LoginVM { get; set; }
        public SavefileViewModel SavefileVM { get; set; }
        public CharacterViewModel CharacterVM { get; set; }

        private enum PageViews: int
        {
            LoginView,
            SavefileView,
            CharacterView,
        }
       
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private int _selectedRadioIndex = 0;
        public int SelectedRadioIndex
        {
            get => _selectedRadioIndex;
            set
            {
                if (_selectedRadioIndex != value)
                {
                    _selectedRadioIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _welcomeMessage = "Please log in to continue...";
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged();
            }
        }

        public void onCharDataPopulated_SavefileVM(object o, EventArgs e)
        {
            SwitchToTab(PageViews.CharacterView);
            CharacterVM.OnCharDataPopulated(o, e);
        }

        private void SwitchToTab(PageViews view)
        {
            switch (view)
            {
                case PageViews.LoginView:
                    CurrentView = LoginVM;
                    SelectedRadioIndex = 0;
                    Console.WriteLine("Advanced to Login Tab");
                    break;
                case PageViews.SavefileView:
                    CurrentView = SavefileVM;
                    SelectedRadioIndex = 1;
                    Console.WriteLine("Advanced to Savefile Tab");
                    break;
                case PageViews.CharacterView:
                    CurrentView = CharacterVM;
                    SelectedRadioIndex = 2;
                    Console.WriteLine("Advanced to Character Tab");
                    break;
            }
        }

        public MainViewModel()
        {
            SupabaseHelper.PreInitSupabase();
            SupabaseHelper.supabase.Auth.AddStateChangedListener((sender, state) =>
            {
                switch (state)
                {
                    case Supabase.Gotrue.Constants.AuthState.SignedIn:
                        WelcomeMessage = $"Welcome back, {SupabaseHelper.supabase.Auth.CurrentUser.Email}";
                        SwitchToTab(PageViews.SavefileView);
                        Console.WriteLine("Signed In");
                        break;

                    case Supabase.Gotrue.Constants.AuthState.SignedOut:
                        WelcomeMessage = "Please log in to continue...";
                        SwitchToTab(PageViews.LoginView);
                        Console.WriteLine("Signed Out");
                        break;

                    case Supabase.Gotrue.Constants.AuthState.UserUpdated:
                        WelcomeMessage = $"Welcome back, {SupabaseHelper.supabase.Auth.CurrentUser.Email}";
                        SwitchToTab(PageViews.SavefileView);
                        Console.WriteLine("UserUpdated");
                        break;

                    case Supabase.Gotrue.Constants.AuthState.TokenRefreshed:
                        Console.WriteLine("TokenRefreshed");
                        break;
                }
            });            
            Task.Run(() => SupabaseHelper.InitSupabaseAsync()); // This is async

            LoginVM = new LoginViewModel();
            CharacterVM = new CharacterViewModel();
            SavefileVM = new SavefileViewModel();
            SavefileVM.CharDataPopulated += onCharDataPopulated_SavefileVM;

            // Setting CurrentView
            CurrentView = LoginVM; // For now

            // Setting up Commands for UI
            HomeViewCommand = new RelayCommand(o => { CurrentView = LoginVM; });
            SavefileViewCommand = new RelayCommand(o => { CurrentView = SavefileVM; });
            CharacterViewCommand = new RelayCommand(o => { CurrentView = CharacterVM; });
        }
    }
}
