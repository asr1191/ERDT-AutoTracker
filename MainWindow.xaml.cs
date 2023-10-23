using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace ERDT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                    .FirstOrDefault();

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
                var selectedFilePath = ofd.FileName;
                savefilePathTextBox.Text = selectedFilePath;
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
                        } else
                        {
                            Console.WriteLine("Not a valid file format");
                        }

                    }
                }


            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
