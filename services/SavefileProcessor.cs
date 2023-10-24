using System;
using System.IO;
using System.Threading.Tasks;

namespace ERDT
{
    internal class SavefileProcessor
    {
        private readonly string _filePath;
        private readonly int _charManifestAddress = 0x1901D04;

        public CharacterData[] characterDataArray;

        public EventHandler charDataPopulated;


        public SavefileProcessor(string filePath)
        {
            _filePath = filePath;
            characterDataArray = new CharacterData[10];
        }

        public void populateCharDataAsync()
        {
            Task.Run(() =>
            {
                populateCharData();
                charDataPopulated?.Invoke(this, EventArgs.Empty);
            });
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
        }
    }
}
