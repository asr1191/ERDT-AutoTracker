using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ERDT.Core;

namespace ERDT
{
    public class SavefileProcessor : ObservableObject
    {
        private readonly string _filePath;
        public string FilePath => _filePath;
        private readonly int _charManifestAddress = 0x1901D04;

        public CharacterData[] characterDataArray;

        private List<CharacterData> _characterDataArrayList;
        public List<CharacterData> CharacterDataArrayList
        {
            get => _characterDataArrayList;
            set
            {
                if (value != _characterDataArrayList)
                {
                    _characterDataArrayList = value;
                    OnPropertyChanged();
                }
            }
        }

        public EventHandler charDataPopulated;
        public Boolean WasPopulatedBefore = false;
        public EventHandler charDataModified;

        public SavefileProcessor(string filePath)
        {
            _filePath = filePath;
            characterDataArray = new CharacterData[10];
        }

        public void populateCharDataAsync()
        {
            Task.Run(() =>
            {
                Console.WriteLine("Populating character data..");
                populateCharData();
                Console.WriteLine("Done populating character data.");
                if (WasPopulatedBefore)
                {
                    charDataModified?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    charDataPopulated?.Invoke(this, EventArgs.Empty);
                    WasPopulatedBefore = true;
                }
            });
        }
        private List<CharacterData> getCharacterDataList()
        {
            return characterDataArray.Select((charData, index) => charData ?? CharacterData.getEmptyCharacterData(index)).ToList();
        }

        private void populateCharData()
        {
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Seek(_charManifestAddress, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    for (var index = 0; index < characterDataArray.Length; index++)
                    {
                        if (reader.ReadByte() == 1)
                        {
                            characterDataArray[index] = new CharacterData(index, _filePath);
                        }
                    }
                }
            }
            CharacterDataArrayList = getCharacterDataList();
        }
    }
}
