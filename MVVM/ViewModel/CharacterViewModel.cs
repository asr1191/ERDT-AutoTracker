
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

        }
    }
}
