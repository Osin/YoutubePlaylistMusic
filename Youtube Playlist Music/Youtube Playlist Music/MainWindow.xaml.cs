using System.Windows;
using Ookii.Dialogs.Wpf;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Youtube_Playlist_Music
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Worker_Local local_worker = new Worker_Local();
        private Worker_Youtube youtubeService = new Worker_Youtube();
        private string playlistTitle { get; set; }
        private string playlistVisibility { get; set; }
        private string playlistDescription { get; set; }

        private BackgroundWorker bw = new BackgroundWorker();


        public MainWindow()
        {
            //Activer cela si vous voulez sauter la connection à Youtube pour l'authentification
            InitializeComponent();
            this.bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
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
            tblResult.Text = files.Count + " musiques détectés, " + this.local_worker.database.Count + " musiques reconnues.\n";
            this.setLayoutToYoutubeJob();
        }

        private void setLayoutToYoutubeJob()
        {
            tblResult.Text += "Cliquer sur le bouton pour créer votre playlist";
            btChoseDir.Visibility = Visibility.Hidden;
            lTitle.Visibility = Visibility.Visible;
            tbTitle.Visibility = Visibility.Visible;
            lVisibility.Visibility = Visibility.Visible;
            lVisibility.Visibility = Visibility.Visible;
            rbPrivate.Visibility = Visibility.Visible;
            rbPublic.Visibility = Visibility.Visible;
            tblResult.Visibility = Visibility.Visible;
            btResult.Visibility = Visibility.Visible;
            btReset.Visibility = Visibility.Visible;
            tbDescription.Visibility = Visibility.Visible;
        }

        private void resetLayout()
        {
            btChoseDir.Visibility = Visibility.Visible;
            lTitle.Visibility = Visibility.Hidden;
            tbTitle.Visibility = Visibility.Hidden;
            lVisibility.Visibility = Visibility.Hidden;
            lVisibility.Visibility = Visibility.Hidden;
            rbPrivate.Visibility = Visibility.Hidden;
            rbPublic.Visibility = Visibility.Hidden;
            tblResult.Visibility = Visibility.Hidden;
            btResult.Visibility = Visibility.Hidden;
            btReset.Visibility = Visibility.Hidden;
            tbDescription.Visibility = Visibility.Hidden;
            pbProgress.Visibility = Visibility.Hidden;
            lProgress.Visibility = Visibility.Hidden;
            tblResult.Visibility = Visibility.Hidden;
        }

        private void setLayoutToJobWorking()
        {
            tblResult.Text = "Taches en cours...";
            pbProgress.Visibility = Visibility.Visible;
            lProgress.Visibility = Visibility.Visible;
            tblResult.Visibility = Visibility.Visible;
            btChoseDir.Visibility = Visibility.Hidden;
            lTitle.Visibility = Visibility.Hidden;
            tbTitle.Visibility = Visibility.Hidden;
            lVisibility.Visibility = Visibility.Hidden;
            lVisibility.Visibility = Visibility.Hidden;
            rbPrivate.Visibility = Visibility.Hidden;
            rbPublic.Visibility = Visibility.Hidden;
            btResult.Visibility = Visibility.Hidden;
            tbDescription.Visibility = Visibility.Hidden;
            btReset.Visibility = Visibility.Hidden;
        }
        private void btResult_Click(object sender, RoutedEventArgs e)
        {
            if (this.playlistVisibility == null)
                this.playlistVisibility = Worker_Youtube.privacyPrivate;
            if (this.playlistTitle == null)
                this.playlistTitle = "Playlist";
            if (this.playlistDescription == null)
                this.playlistDescription= "Description";
            this.setLayoutToJobWorking();
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            this.playlistVisibility = null ;
            this.playlistTitle = null;
            this.playlistDescription = null;
            this.local_worker = new Worker_Local();
            this.youtubeService = new Worker_Youtube();
            this.resetLayout();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int totalElements = this.local_worker.database.Count + 1; //On garde 1 car créer une liste est une action a conserver dans le compteur
            int index = 0;
            try
            {
                this.youtubeService.init().Wait();
                var playlist = youtubeService.createPlaylist(this.playlistTitle, this.playlistDescription, this.playlistVisibility);
                index++;
                worker.ReportProgress((int) (100 / (totalElements)) * index);            
                foreach (Music music in this.local_worker.database)
	            {
                    Console.WriteLine(music.artist + " || " + music.title);
                    var videos = this.youtubeService.searchVideo(music.artist + " " + music.title);
                    foreach (var video in videos)
                    {
                        this.youtubeService.addVideoToPlaylist(playlist, video);
                    }
                    Console.WriteLine(index);
                    index++;
                    int percent = (200 * index + 1) / (totalElements * 2);
                    worker.ReportProgress(percent);
	            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return;
            }
        }

        private void tbDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbDescription.Text == "")
            {
                tbDescription.Text = "Description de votre playlist";
            }
            else
            {
                this.playlistDescription = tbDescription.Text;
            }
        }

        private void tbDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbDescription.Text == "Description de votre playlist")
            {
                tbDescription.Text = ""; 
            }
        }

        private void RadioButtonPublic_Checked(object sender, RoutedEventArgs e)
        {
            rbPrivate.IsChecked = false;
            this.playlistVisibility = Worker_Youtube.privacyPublic;
        }

        private void RadioButtonPrivate_Checked(object sender, RoutedEventArgs e)
        {
            rbPrivate.IsChecked = true;
            this.playlistVisibility = Worker_Youtube.privacyPrivate;
        }

        private void tbTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.playlistTitle = tbTitle.Text;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.lProgress.Content = (e.ProgressPercentage.ToString() + "%");
            this.pbProgress.Value = e.ProgressPercentage;
            Console.WriteLine(e.ProgressPercentage.ToString());
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tblResult.Text = "Terminée";
            btReset.Visibility = Visibility.Visible;
        }
    }
}