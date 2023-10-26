using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ERDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        SavefileWatcher savefileWatcher;
        SavefileProcessor savefileProcessor;
        string saveFilePath;

        private CharacterData selectedCharacterData;
        public CharacterData SelectedCharacterData
        {
            get => selectedCharacterData;
            set
            {
                if (selectedCharacterData != value)
                {
                    selectedCharacterData = value;
                    OnPropertyChanged(nameof(SelectedCharacterData));
                }
            }
        }

        private int previouslySelectedIndex = -2;
        public int PreviouslySelectedIndex
        {
            get => previouslySelectedIndex;
            set
            {
                if (previouslySelectedIndex != value && value != -1) // Disregard ListView's autoupdate
                {
                    previouslySelectedIndex = value;
                    OnPropertyChanged(nameof(PreviouslySelectedIndex));
                }
            }
        }


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

                            savefileProcessor = new SavefileProcessor(selectedFilePath);
                            savefileProcessor.charDataPopulated += onCharDataPopulated;
                            savefileProcessor.populateCharDataAsync();

                        } else
                        {
                            trackBox.IsEnabled = false;
                            trackBox.IsChecked = false;
                            characterListView.IsEnabled = false;
                            characterListView.Visibility = Visibility.Collapsed;
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
                trackBox.IsEnabled = true;

                characterListView.Visibility = Visibility.Visible;
                characterListView.IsEnabled = true;
                characterListView.ItemsSource = savefileProcessor.getCharacterDataList();

                if (PreviouslySelectedIndex != -2)
                {
                    characterListView.SelectedIndex = PreviouslySelectedIndex;
                }
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

        private void characterListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectedCharacterData = (CharacterData) (sender as ListView).SelectedItem;
            PreviouslySelectedIndex = characterListView.SelectedIndex;
        }
    }
}
