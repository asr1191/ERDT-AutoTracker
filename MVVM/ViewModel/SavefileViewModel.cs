using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ERDT.Core;
using Microsoft.Win32;

namespace ERDT.MVVM.ViewModel
{
    internal class SavefileViewModel : ObservableObject
    {
        public RelayCommand SelectSavefileCommand
        {
            get; set;
        }

        private int PreviouslySelectedIndex
        {
            get; set;
        }

        private void SelectSavefileExecute(object commandParameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Elden Ring Savefile (*.sl2)|*.sl2";
            ofd.FileOk += Ofd_VerifySaveFile;

            try
            {
                var appDataRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var eldenRingPath = Path.Combine(appDataRoamingPath, "EldenRing");
                DirectoryInfo eldenRingDirectory = new DirectoryInfo(eldenRingPath);

                var mostRecentDirectory = eldenRingDirectory.GetDirectories()
                    .OrderBy(dir => dir.LastWriteTime)
                    .LastOrDefault();

                if (mostRecentDirectory != null)
                {
                    ofd.InitialDirectory = mostRecentDirectory.FullName;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (ofd.ShowDialog() == true)
            {
                SharedDataService.Instance.SavefilePath = ofd.FileName;
            }
        }

        private void Ofd_VerifySaveFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var selectedFilePath = (sender as OpenFileDialog).FileName;
            PreviouslySelectedIndex = -2;

            try
            {
                using (FileStream fileStream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        var magicBytes = reader.ReadBytes(4);
                        var magicString = Encoding.UTF8.GetString(magicBytes);
                        if (magicString.Equals("BND4"))
                        {
                            Console.WriteLine("Magic string found (BND4: " + selectedFilePath + ")");

                            SavefileProcessor sfProcessor = SharedDataService.InitalizeSavefileProcessor(selectedFilePath);
                            sfProcessor.charDataPopulated += onCharDataPopulated;
                            sfProcessor.populateCharDataAsync();
                        }
                        else
                        {
                            //trackBox.IsEnabled = false;
                            //trackBox.IsChecked = false;
                            //characterListView.IsEnabled = false;
                            //characterListView.Visibility = Visibility.Collapsed;
                            Console.WriteLine("Not a valid file format");
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void onCharDataPopulated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //trackBox.IsEnabled = true;

                //characterListView.Visibility = Visibility.Visible;
                //characterListView.IsEnabled = true;
                //characterListView.ItemsSource = savefileProcessor.getCharacterDataList();

                //if (PreviouslySelectedIndex != -2)
                //{
                //    characterListView.SelectedIndex = PreviouslySelectedIndex;
                //}
            });
        }

        public SavefileViewModel()
        {
            SelectSavefileCommand = new RelayCommand(SelectSavefileExecute);
        }
    }
}
