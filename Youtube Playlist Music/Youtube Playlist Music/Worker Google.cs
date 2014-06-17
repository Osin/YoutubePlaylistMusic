using Google.Apis;
using System;

namespace Youtube_Playlist_Music
{
    class Worker_Google
    {
        private Uri endPoint = new Uri("https://accounts.google.com/o/oauth2/auth");
        private string responseType = "code";
        private string accessToken;
        public string redirectUri { get; set; }
        public string scope { get; set; }
        private string clientId { get; set; }

        public Worker_Google()
        {
            if (Properties.Settings.Default.ClientId == null)
                throw new Exception("Le client de l'api n'est pas correctement configuré");
            this.clientId = Properties.Settings.Default.ClientId;
            this.scope = scope;
        }

        private Uri addParameterToEndPoint(string parameterName, string parameterValue, Uri previousUri = null)
        {
            UriBuilder uri = previousUri == null? new UriBuilder(this.endPoint) : new UriBuilder(previousUri);
            string queryToAppend = parameterName + "=" + parameterValue;
            if (uri.Query != null && uri.Query.Length > 1)
                uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
            else
                uri.Query = queryToAppend;
            return uri.Uri;
        }

        public Uri getAuthRequestUri()
        {
            Uri uri = this.addParameterToEndPoint("scope", Properties.Settings.Default.Scope);
            uri = this.addParameterToEndPoint("redirect_uri", Properties.Settings.Default.RedirectUri, uri);
            uri = this.addParameterToEndPoint("response_type", this.responseType, uri);
            uri = this.addParameterToEndPoint("client_id", Properties.Settings.Default.ClientId, uri);
            uri = this.addParameterToEndPoint("access_type", Properties.Settings.Default.AccessType, uri);
            return uri;
        }

        public Uri completeAuthRequestUri(string code)
        {
            if (this.accessToken == null)
            {
                throw new Exception("access token cannot be null when you call this method");
            }
            //a terminer

        }

        public void setToken(string code)
        {
            this.accessToken = code;
        }

    }
}
