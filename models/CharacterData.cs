using System.IO;
using System.Text;

namespace ERDT
{
    public class CharacterData
    {
        private static readonly int _slotStart = 0x300;
        private static readonly int _slotLength = 0x280010;
        private readonly int _startAddress;
        private readonly string _filepath;

        private int _version;
        public string name;
        public int playtime;
        public int runeLevel;
        public int deathTotal;

        public CharacterData(int index, string filepath)
        {
            _startAddress = _slotStart + (index * _slotLength);
            _filepath = filepath;
            getData();
        }

        private void SkipLookupEntry(FileStream fileStream, BinaryReader reader)
        {
            var bytes = reader.ReadBytes(0x8);
            fileStream.Seek(-0x8, SeekOrigin.Current);

            if (bytes[3] == 0xC0 || bytes[3] == 0x80 || bytes[3] == 0x90)
            {
                fileStream.Seek(0x8, SeekOrigin.Current);

                if (bytes[3] == 0x80)
                {
                    fileStream.Seek(0xD, SeekOrigin.Current);
                }
                else if (bytes[3] == 0x90)
                {
                    fileStream.Seek(0x8, SeekOrigin.Current);
                }
                else if (bytes[3] == 0xC0)
                {
                    // No bytes to seek.
                }
            }
            else
            {
                fileStream.Seek(0x08, SeekOrigin.Current);
            }

        }

        public void getData()
        {
            using (FileStream fileStream = new FileStream(_filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    fileStream.Seek(_startAddress, SeekOrigin.Begin);

                    fileStream.Seek(0x10, SeekOrigin.Current);
                    _version = reader.ReadInt32();

                    fileStream.Seek(0x04, SeekOrigin.Current);
                    playtime = reader.ReadInt32();

                    fileStream.Seek(0x04, SeekOrigin.Current);
                    if (_version > 0x51)
                    {
                        fileStream.Seek(0x10, SeekOrigin.Current);
                    }

                    for (var lookupIndex = 0; lookupIndex < 0x1400; lookupIndex++)
                    {
                        SkipLookupEntry(fileStream, reader);
                    }

                    fileStream.Seek(0x94, SeekOrigin.Current);
                    var nameBytes = reader.ReadBytes(0x20);
                    var nameString = Encoding.Unicode.GetString(nameBytes);
                    name = nameString.IndexOf("\0") == -1 ? nameString : nameString.Substring(0, nameString.IndexOf("\0"));

                    fileStream.Seek(0x9418, SeekOrigin.Current);

                    var unkBlockCount = reader.ReadInt32();
                    for (var index = 0; index < unkBlockCount; index++)
                    {
                        fileStream.Seek(0x08, SeekOrigin.Current);
                    }

                    fileStream.Seek(0x62E7, SeekOrigin.Current);

                    var playRegionCount = reader.ReadInt32();
                    for (var index = 0; index < playRegionCount; index++)
                    {
                        fileStream.Seek(0x04, SeekOrigin.Current);
                    }

                    fileStream.Seek(0x1CA44, SeekOrigin.Current);
                    deathTotal = (int)reader.ReadUInt32();
                }
            }
        }
    }
}