using System.Windows;
using Ookii.Dialogs.Wpf;
using System.IO;
using System;

namespace Youtube_Playlist_Music
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Worker_Local local_worker = new Worker_Local();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == true)
            {
                if (Directory.Exists(dlg.SelectedPath))
                {
                    this.local_worker.rootFolder = dlg.SelectedPath;
                    System.Collections.Generic.List<String> files = new System.Collections.Generic.List<String>();
                    files.AddRange(Directory.GetFileSystemEntries(this.local_worker.rootFolder, "*", SearchOption.AllDirectories));
                    this.local_worker.files = new System.Collections.Generic.List<String>();
                    this.local_worker.files.AddRange(files.FindAll(s => s.Contains(".mp3")));
                    this.local_worker.files.AddRange(files.FindAll(s => s.Contains(".mp4")));
                    this.local_worker.files.AddRange(files.FindAll(s => s.Contains(".m4a")));
                    this.local_worker.files.AddRange(files.FindAll(s => s.Contains(".flac")));
                    this.local_worker.generateDataBase();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Erreur sur le dossier",
                        "Il semblerait que le dossier choisi n'existe pas", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.local_worker.rootFolder = null;
                    this.local_worker.files = null;
                }
                
            }
        }
    }
}
