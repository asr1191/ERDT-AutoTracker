﻿namespace ERDT.Core
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

                    SavefileWatcher?.stopWatching();
                }
            }
        }


        public static SavefileProcessor SavefileProcessor
        {
            get; private set;
        }

        public static SavefileWatcher SavefileWatcher { get; private set; }

        public static SavefileProcessor InitalizeSavefileProcessor(string savefilePath)
        {
            SavefileProcessor = new SavefileProcessor(savefilePath);
            return SavefileProcessor;
        }

        public static SavefileWatcher InitializeSavefileWatcher()
        {
            SavefileWatcher = new SavefileWatcher(SavefileProcessor.FilePath);
            return SavefileWatcher;
        }

    }
}
