namespace ERDT.Core
{
    public class SharedDataService : ObservableObject
    {
        private static SharedDataService _instance;
        public static SharedDataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    
                    _instance = new SharedDataService();
                    return _instance;
                }
                return _instance;
            }
        }

        //public string SavefilePath { get; set; } = "Please select your Elden Ring savefile!";

        private string _savefilePath = "Please select your Elden Ring savefile!";
        public string SavefilePath
        {
            get => _savefilePath;
            set
            {
                if (_savefilePath != value)
                {
                    _savefilePath = value;
                    OnPropertyChanged();
                }

            }

        }


        public static SavefileProcessor SavefileProcessor
        {
            get; private set;
        }

        public SharedDataService()
        {

        }

        public static SavefileProcessor InitalizeSavefileProcessor(string savefilePath)
        {
            SavefileProcessor = new SavefileProcessor(savefilePath);
            return SavefileProcessor;
        }

    }
}
