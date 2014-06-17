using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Youtube_Playlist_Music
{
    class Worker_Local
    {
        public String rootFolder { get; set; }
        public List<String> files { get; set; }
        public List<Dictionary<string, string>> database { get; set; }

        public bool generateDataBase()
        {
            if (files == null || files.Count == 0)
            {
                return false;
            }
            this.database = new List<Dictionary<string, string>>();
            files.ForEach(delegate(String path)
            {
                TagLib.File file = TagLib.File.Create(path);
                Dictionary<string, string> media = new Dictionary<string, string>();
                string artist = file.Tag.FirstPerformer != null ? file.Tag.FirstPerformer : "";
                string title = file.Tag.Title != null ? file.Tag.Title : "";
                media.Add(artist, title);
                this.database.Add(media);
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
