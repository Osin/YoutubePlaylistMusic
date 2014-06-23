using Google.Apis;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Youtube_Playlist_Music
{
    class Worker_Google
    {
        private Uri accessAuthEndPoint = new Uri("https://accounts.google.com/o/oauth2/");
        private Uri accessConfirmAuthEndPoint = new Uri("https://accounts.google.com/o/oauth2/auth");
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
            UriBuilder uri = previousUri == null? new UriBuilder(this.accessAuthEndPoint) : new UriBuilder(previousUri);
            string queryToAppend = parameterName + "=" + parameterValue;
            if (uri.Query != null && uri.Query.Length > 1)
                uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
            else
                uri.Query = queryToAppend;
            return uri.Uri;
        }

        public Uri getAuthRequestUri()
        {
            Uri uri = new Uri(this.accessAuthEndPoint + "auth");
            uri = this.addParameterToEndPoint("scope", Properties.Settings.Default.Scope, uri);
            uri = this.addParameterToEndPoint("redirect_uri", Properties.Settings.Default.RedirectUri, uri);
            uri = this.addParameterToEndPoint("response_type", this.responseType, uri);
            uri = this.addParameterToEndPoint("client_id", Properties.Settings.Default.ClientId, uri);
            uri = this.addParameterToEndPoint("access_type", Properties.Settings.Default.AccessType, uri);
            return uri;
        }

        public void completeAuthRequestUri(string code)
        {
            Uri uri = new Uri(this.accessAuthEndPoint + "token");
            string parameters = "code=" + code
                              + "&client_id=" + Properties.Settings.Default.ClientId
                              + "&client_secret=" + Properties.Settings.Default.ClientSecret
                              + "&redirect_uri=" + Properties.Settings.Default.RedirectUri
                              + "&grant_type=authorization_code"
                              ;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = parameters.Length;
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(parameters);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var definition = new { Name = "access_token", Expire = "expires_in", Token = "token_type" };
            var unserializedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            Console.WriteLine(responseString);
            var matches = Regex.Match(responseString, "(: \\\")(.*?)\\\",\\n  \\\"t");
            var matche = Regex.Match(matches.ToString(), "\\\"(.*)\"");
            string token = matche.Value.Substring(1, matche.Length - 2);
            this.setToken(token);
        }

        public void setToken(string code)
        {
            this.accessToken = code;
        }

    }
}
