namespace ERDT.Core
{
    public class SharedDataService : ObservableObject
    {
        private static readonly SharedDataService _instance;
        public static SharedDataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    return new SharedDataService();
                }
                return _instance;
            }
        }

        public static string SavefilePath { get; set; } = "Please select your Elden Ring savefile!";

        //private static string _savefilePath = "Please select your Elden Ring savefile!";
        //public static string SavefilePath
        //{
        //    get => _savefilePath;
        //    set
        //    {
        //        if (_savefilePath != value)
        //        {
        //            _savefilePath = value;
        //            OnPropertyChanged();
        //        }

        //    }

        //}


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
