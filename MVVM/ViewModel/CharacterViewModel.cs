
using ERDT.Core;
using System;
using System.Collections.Generic;

namespace ERDT.MVVM.ViewModel
{
    public class CharacterViewModel : ObservableObject
    {
        private CharacterData _selectedCharacterData;
        public CharacterData SelectedCharacterData
        {
            get => _selectedCharacterData;
            set
            {
                if (_selectedCharacterData != value)
                {
                    _selectedCharacterData = value;
                    Console.WriteLine("Selected character: " +  _selectedCharacterData.Name);
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
            List<CharacterData> charDataList = SharedDataService.SavefileProcessor.getCharacterDataList();
            SelectedCharacterData = charDataList[0];

            SavefileWatcher savefileWatcher = SharedDataService.InitializeSavefileWatcher();
            savefileWatcher.SavefileChanged += OnSavefileChanged;
        }

        private void OnSavefileChanged(object sender, EventArgs e)
        {
            SharedDataService.SavefileProcessor.charDataModified += HandleNewCharData;
            SharedDataService.SavefileProcessor.populateCharDataAsync();
        }

        private void HandleNewCharData(object sender, EventArgs e)
        {
            List<CharacterData> newCharDataList = SharedDataService.SavefileProcessor.getCharacterDataList();
            if (newCharDataList[SelectedCharacterData.slotIndex].Name != SelectedCharacterData.Name)
            {
                // Pop-up dialog saying "Major Change in Savefile Detected"?
                throw new Exception("Huge Change in Savefile?");
            }
            else
            {
                SelectedCharacterData = newCharDataList[SelectedCharacterData.slotIndex];
            }
        }
    }
}
