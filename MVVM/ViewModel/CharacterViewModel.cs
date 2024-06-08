
using ERDT.Core;
using System;
using System.Collections.Generic;

namespace ERDT.MVVM.ViewModel
{
    public class CharacterViewModel : ObservableObject
    {
        private CharacterData PrevSelectedCharacterData;
        private CharacterData _selectedCharacterData;
        public CharacterData SelectedCharacterData
        {
            get => _selectedCharacterData;
            private set
            {
                _selectedCharacterData = value;
                OnPropertyChanged();
            }
        }

        private int _selectedCharacterIndex;
        public int SelectedCharacterIndex
        {
            get => _selectedCharacterIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedCharacterIndex = value;
                    Console.WriteLine("Selected character index: " + value);
                    SelectedCharacterData = SharedDataService.SavefileProcessor.CharacterDataArrayList[value];
                    OnPropertyChanged();
                }
            }
        }

        public CharacterViewModel()
        {
            Console.WriteLine("Constructing CharacterViewModel");

        }

        public void OnCharDataPopulated(object o, EventArgs e)
        {
            Console.WriteLine("CharacterViewModel: OnCharDataPopulated");
            List<CharacterData> charDataList = SharedDataService.SavefileProcessor.CharacterDataArrayList;
            SelectedCharacterIndex = 0;

            SavefileWatcher savefileWatcher = SharedDataService.InitializeSavefileWatcher();
            savefileWatcher.SavefileChanged += OnSavefileChanged;
            SharedDataService.SavefileProcessor.charDataModified += HandleNewCharData;
        }

        private void OnSavefileChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Savefile change detected!");
            Console.WriteLine("Repopulating char data");

            PrevSelectedCharacterData = SharedDataService.SavefileProcessor.CharacterDataArrayList[SelectedCharacterIndex];
            SharedDataService.SavefileProcessor.populateCharDataAsync();
        }

        private void HandleNewCharData(object sender, EventArgs e)
        {
            Console.WriteLine("CharacterViewModel: HandleNewCharData: Char data repopulated");
            if (SharedDataService.SavefileProcessor.CharacterDataArrayList[PrevSelectedCharacterData.slotIndex].Name == PrevSelectedCharacterData.Name)
            {
                SelectedCharacterIndex = PrevSelectedCharacterData.slotIndex;
                Console.WriteLine("Set SelectedCharacterData");
            } else
            {
                throw new Exception("Major Savefile Change Detected!");
            }
        }
    }
}
