using System.Windows;
using Ookii.Dialogs.Wpf;
using System.IO;
using System;
using System.Windows.Navigation;
using System.Collections.Generic;

namespace Youtube_Playlist_Music
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Worker_Local local_worker = new Worker_Local();
        private Worker_Google google_worker = new Worker_Google();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            if (dlg.ShowDialog() == false)
            {
                return;
            }
            if (!Directory.Exists(dlg.SelectedPath))
            {
                MessageBoxResult result = MessageBox.Show("Erreur sur le dossier",
                    "Il semblerait que le dossier choisi n'existe pas", MessageBoxButton.OK, MessageBoxImage.Error);
                this.local_worker.rootFolder = null;
                this.local_worker.files = null;
                return;
            }

            this.local_worker.rootFolder = dlg.SelectedPath;
            List<String> files = this.local_worker.scanDirectoryForMedia();
            bool databaseIsGenerate = this.local_worker.generateDataBase();
            if (databaseIsGenerate == false)
            {
                MessageBoxResult result = MessageBox.Show("Erreur lors du scan du dossier",
                "Je ne sais pas quoi faire pour vous aider, désolé!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            tblResult.Text = files.Count + " éléments détectés. Veuillez vous connecter à Youtube pour créer votre playlist";
            btChoseDir.Visibility = Visibility.Hidden;

            tblResult.Visibility = Visibility.Visible;
            btResult.Visibility = Visibility.Visible;
            
        }

        private void btResult_Click(object sender, RoutedEventArgs e)
        {
            wbBrowser.Source = this.google_worker.getAuthRequestUri();
            
            btChoseDir.Visibility = Visibility.Hidden;
            lbInfos.Visibility = Visibility.Hidden;
            btResult.Visibility = Visibility.Hidden;
            tblResult.Visibility = Visibility.Hidden;

            wbBrowser.Visibility = Visibility.Visible;
            tbCode.Visibility = Visibility.Visible;
            btValidateCode.Visibility = Visibility.Visible;
        }

        private void btValidateCode_Click(object sender, RoutedEventArgs e)
        {
            string code = tbCode.Text.Trim();
            if (code.Length == 0) {
                MessageBoxResult result = MessageBox.Show("Code non specifié", "Veuillez à bien mettre le code fourni par google après la connexion à votre compte", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.google_worker.setToken(code);
            Console.WriteLine(code);
        }

        private void tbCode_GotFocus(object sender, RoutedEventArgs e)
        {
            tbCode.Text = "";
        }

        private void tbCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbCode.Text == "")
                tbCode.Text = "Veuillez spécifier le code qui vous est donné par Google une fois que vous vous êtes authentifié.";
        }

    }
}
