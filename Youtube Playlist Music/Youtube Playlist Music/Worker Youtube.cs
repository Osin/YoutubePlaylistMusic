using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Youtube_Playlist_Music
{
    internal class Worker_Youtube
    {
        public YouTubeService youtubeService { get; set; }
        public const string privacyPrivate = "private";
        public const string privacyPublic = "public";

        public async Task init()
        {
            UserCredential credential;
            using (var stream = new FileStream(@"client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            this.youtubeService = youtubeService;
        }

        public Playlist createPlaylist(string title, string description, string privacy)
        {
            if (this.youtubeService == null)
                throw new Exception("Youtube Service is cannot be null when you call this action");
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = title;
            newPlaylist.Snippet.Description = description;
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = privacy;
            newPlaylist = this.youtubeService.Playlists.Insert(newPlaylist, "snippet,status").Execute();
            return newPlaylist;
        }

        public void addVideoToPlaylist(Playlist playlist, string videoId ){
            if (this.youtubeService == null)
                throw new Exception("Youtube Service is cannot be null when you call this action");
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = playlist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = videoId;
            newPlaylistItem = this.youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").Execute();
            Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, playlist.Id);
        }

        public List<string> searchVideo(string query, int maxVideoToRetrieve = 1)
        {
            List<string> videos = new List<string>();
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query; // Replace with your search term.
            //searchListRequest.VideoDefinition = Google.Apis.YouTube.v3.SearchResource.ListRequest.VideoDefinitionEnum.Any; //mettre en HD, ou pas;
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = maxVideoToRetrieve;
            var searchListResponse = searchListRequest.Execute();
            foreach (var searchResult in searchListResponse.Items)
            {
                Console.WriteLine(searchResult.Snippet.ToString());
                videos.Add(searchResult.Id.VideoId);
            }
            return videos;
        }
    }
}
