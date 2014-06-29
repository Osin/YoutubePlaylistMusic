using System;
using System.Collections.Generic;
using System.IO;

namespace Youtube_Playlist_Music
{
    class Worker_Local
    {
        public String rootFolder { get; set; }
        public List<String> files { get; set; }
        public List<Music> database { get; set; }

        public bool generateDataBase()
        {
            if (files == null || files.Count == 0)
            {
                return false;
            }
            this.database = new List<Music>();
            files.ForEach(delegate(String path)
            {
                TagLib.File file = TagLib.File.Create(path);
                
                string artist = file.Tag.FirstPerformer != null ? file.Tag.FirstPerformer : "";
                string title = file.Tag.Title != null ? file.Tag.Title : "";
                if (String.Empty != artist || string.Empty != title) {
                    var song = new Music(artist, title);
                    this.database.Add(song);
                }
            });
            return true;
        }

        internal List<String> scanDirectoryForMedia()
        {
            List<String> files = new List<String>();
            files.AddRange(Directory.GetFileSystemEntries(this.rootFolder, "*", SearchOption.AllDirectories));
            this.files = new System.Collections.Generic.List<String>();
            this.files.AddRange(files.FindAll(s => s.Contains(".mp3")));
            this.files.AddRange(files.FindAll(s => s.Contains(".mp4")));
            this.files.AddRange(files.FindAll(s => s.Contains(".m4a")));
            this.files.AddRange(files.FindAll(s => s.Contains(".flac")));
            return this.files;
        }
    }
}
