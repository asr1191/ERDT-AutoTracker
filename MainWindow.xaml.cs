using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ERDT.services;
using Microsoft.Win32;

namespace ERDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SavefileWatcher savefileWatcher;
        SavefileProcessor savefileProcessor;
        string saveFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectSave_Click(object sender, RoutedEventArgs e)
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
                saveFilePath = ofd.FileName; 
                savefilePathTextBox.Text = saveFilePath;
            }
        }

        private void Ofd_VerifySaveFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var selectedFilePath = (sender as OpenFileDialog).FileName;

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

                            savefileProcessor = new SavefileProcessor(selectedFilePath);
                            savefileProcessor.charDataPopulated += onCharDataPopulated;
                            savefileProcessor.populateCharDataAsync();

                        } else
                        {
                            trackBox.IsEnabled = false;
                            trackBox.IsChecked = false;
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
                nameTextBox.Text = savefileProcessor.characterDataArray[1].name;
                deathCountTextBox.Text = savefileProcessor.characterDataArray[1].deathTotal.ToString();
                trackBox.IsEnabled = true;
            });
        }

        private void TrackBox_Checked(object sender, RoutedEventArgs e)
        {
            savefileProcessor.populateCharDataAsync();
            savefileWatcher = new SavefileWatcher(saveFilePath);
            savefileWatcher.SavefileChanged += onSaveFileChanged;
        }

        private void TrackBox_Unchecked(object sender, RoutedEventArgs e)
        {
            savefileWatcher.SavefileChanged -= onSaveFileChanged;
            savefileWatcher.stopWatching();
        }

        private void onSaveFileChanged(object sender, EventArgs e)
        {
            savefileProcessor.populateCharDataAsync();
        }
    }
}
