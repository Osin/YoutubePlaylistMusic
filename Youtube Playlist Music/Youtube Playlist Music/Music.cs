
namespace Youtube_Playlist_Music
{
    class Music
    {
        public string artist { get; private set; }
        public string title { get; private set; }

        public Music(string artist, string title)
        {
            this.artist = artist;
            this.title = title;
        }

    }
}
