using ERDT.Core;

namespace ERDT.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SavefileViewCommand { get; set; }
        public RelayCommand CharacterViewCommand { get; set; }

        public LoginViewModel HomeVM { get; set; }
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


        public MainViewModel()
        {
            HomeVM = new LoginViewModel();
            SavefileVM = new SavefileViewModel();
            CharacterVM = new CharacterViewModel();

            CurrentView = SavefileVM; // For now

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            SavefileViewCommand = new RelayCommand(o =>
            {
                CurrentView = SavefileVM;
            });

            CharacterViewCommand = new RelayCommand(o =>
            {
                CurrentView = CharacterVM;
            });
        }
    }
}
