using System;
using ERDT.Core;

namespace ERDT.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SavefileViewCommand { get; set; }
        public RelayCommand CharacterViewCommand { get; set; }

        public LoginViewModel LoginVM { get; set; }
        public SavefileViewModel SavefileVM { get; set; }
        public CharacterViewModel CharacterVM { get; set; }
       
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

        public void onCharDataPopulated_SavefileVM(object o, EventArgs e)
        {
            CurrentView = CharacterVM;
        }

        public MainViewModel()
        {
            LoginVM = new LoginViewModel();
            CharacterVM = new CharacterViewModel();

            SavefileVM = new SavefileViewModel();
            SavefileVM.CharDataPopulated += onCharDataPopulated_SavefileVM;


            // Setting CurrentView
            CurrentView = SavefileVM; // For now

            // Setting up Commands for UI
            HomeViewCommand = new RelayCommand(o => { CurrentView = LoginVM; });
            SavefileViewCommand = new RelayCommand(o => { CurrentView = SavefileVM; });
            CharacterViewCommand = new RelayCommand(o => { CurrentView = CharacterVM; });
        }

        
    }
}
