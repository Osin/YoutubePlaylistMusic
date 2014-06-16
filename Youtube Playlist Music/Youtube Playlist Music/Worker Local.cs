using System;
using System.Collections.Generic;
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
    }
}
