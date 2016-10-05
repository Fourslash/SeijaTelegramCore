using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
//using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace SeijaTelegram.Twitter
{
    public class TwitAuthenticateResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }

    class TwitterConnector
    {
        const string AUTH_URL = @"https://api.twitter.com/oauth2/token";
        const string TIMELINE_URL = @"https://api.twitter.com/1.1/statuses/user_timeline.json";


        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        TwitAuthenticateResponse Token { get; set; }


        public TwitterConnector()
        {
            Authorize();
        }

        async public Task<Tweet[]> LoadTweets(string userId)
        {
            if (Token == null)
            {
                Authorize();
            }
            /*return*/
            return await loadTweets(userId);
        }

        async Task<Tweet[]> loadTweets(string userId)
        {
            var timelineFormat = TIMELINE_URL + "?user_id={0}&exclude_replies=true&include_rts=false";
            var timelineUrl = string.Format(timelineFormat, userId);
            HttpWebRequest timeLineRequest = (HttpWebRequest)WebRequest.Create(timelineUrl);
            var timelineHeaderFormat = "{0} {1}";
            timeLineRequest.Headers["Authorization"] = string.Format(timelineHeaderFormat, Token.token_type, Token.access_token);
            timeLineRequest.Method = "Get";
            WebResponse timeLineResponse = await timeLineRequest.GetResponseAsync();
            var timeLineJson = string.Empty;
            using (timeLineResponse)
            {
                using (var reader = new StreamReader(timeLineResponse.GetResponseStream()))
                {
                    timeLineJson = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Tweet[]>(timeLineJson);
                }
            }
        }


        void LoadCredentials()
        {
            ConsumerKey = SeijaTelegram.Main.Brain.settings.twitterConsumerKey;
            ConsumerSecret = SeijaTelegram.Main.Brain.settings.twitterConsumerSecret;
        }

        async void Authorize()
        {
            LoadCredentials();

            var authHeaderFormat = "Basic {0}";

            var authHeader = string.Format(authHeaderFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(ConsumerKey) + ":" +
                Uri.EscapeDataString((ConsumerSecret)))
            ));

            var postBody = "grant_type=client_credentials";

            HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(AUTH_URL);
            authRequest.Headers["Authorization"] = authHeader;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            //authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (Stream stream = await authRequest.GetRequestStreamAsync())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            authRequest.Headers["Accept-Encoding"] = "gzip";

            using (var authResponse = await authRequest.GetResponseAsync())
            {                
                using (var responseStream = new GZipStream(authResponse.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        var objectText = reader.ReadToEnd();
                        Token = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                    }
                }                
            }
        }
    }
}
