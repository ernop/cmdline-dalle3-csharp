//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//using System;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;

//namespace Ideogram
//{
//    public class IdeogramClient
//    {
//        private string userId;
//        private string channelId;
//        private string sessionCookieToken;
//        private string prompt;
//        private string aspectRatio;
//        private string outputDir;
//        private bool enableLogging;

//        public IdeogramClient(
//            string sessionCookieToken,
//            string prompt,
//            string userId = "-xnquyqCVSFOYTomOeUchbw",
//            string channelId = "LbF4xfurTryl5MUEZ73bDw",
//            string aspectRatio = "square",
//            string outputDir = "images",
//            bool enableLogging = false)
//        {
//            if (string.IsNullOrEmpty(sessionCookieToken))
//                throw new ArgumentException("Session cookie token is not defined.");
//            if (string.IsNullOrEmpty(prompt))
//                throw new ArgumentException("Prompt is not defined.");

//            this.userId = userId;
//            this.channelId = channelId;
//            this.sessionCookieToken = sessionCookieToken;
//            this.prompt = prompt;
//            this.aspectRatio = aspectRatio;
//            this.outputDir = outputDir;
//            this.enableLogging = enableLogging;

//            if (this.enableLogging)
//            {
//                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]: IdeogramWrapper initialized.");
//            }
//        }

//        private async Task<JObject> FetchGenerationMetadataAsync(string requestId)
//        {
//            string url = $"{Statics.BaseUrl}/retrieve_metadata_request_id/{requestId}";
//            (string headers, string cookies) = GetRequestParams();

//            try
//            {
//                using (var client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Add("Accept", headers);
//                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//                    client.DefaultRequestHeaders.Add("Cookie", cookies);

//                    var response = await client.GetAsync(url);
//                    response.EnsureSuccessStatusCode();

//                    var json = await response.Content.ReadAsStringAsync();
//                    var data = JObject.Parse(json);

//                    if (data["resolution"].Value<int>() == 1024)
//                    {
//                        if (enableLogging)
//                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]: Receiving image data...");
//                        return data;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR]: An error occurred: {ex.Message}");
//            }

//            return null;
//        }

//        public async Task InferenceAsync()
//        {
//            string url = $"{Statics.BaseUrl}/sample";
//            (string headers, string cookies) = GetRequestParams();

//            var payload = new
//            {
//                aspect_ratio = aspectRatio,
//                channel_id = channelId,
//                prompt = prompt,
//                raw_or_fun = "raw",
//                speed = "slow",
//                style = "photo",
//                user_id = userId
//            };

//            try
//            {
//                using (var client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Add("Accept", headers);
//                    client.DefaultRequestHeaders.Add("Content-Type", "application/json");
//                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//                    client.DefaultRequestHeaders.Add("Cookie", cookies);

//                    var response = await client.PostAsJsonAsync(url, payload);
//                    response.EnsureSuccessStatusCode();

//                    var json = await response.Content.ReadAsStringAsync();
//                    var data = JObject.Parse(json);
//                    string requestId = data["request_id"].Value<string>();

//                    if (enableLogging)
//                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]: Generation request sent. Waiting for response...");

//                    await MakeGetRequestAsync(requestId);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR]: An error occurred: {ex.Message}");
//            }
//        }

//        private async Task MakeGetRequestAsync(string requestId)
//        {
//            DateTime endTime = DateTime.Now.AddMinutes(5);

//            while (DateTime.Now < endTime)
//            {
//                var imageData = await FetchGenerationMetadataAsync(requestId);
//                if (imageData != null)
//                {
//                    await DownloadImagesAsync(imageData["responses"].Values<JObject>().ToArray());
//                    return;
//                }
//                await Task.Delay(1000);
//            }
//        }

//        private async Task DownloadImagesAsync(JObject[] responses)
//        {
//            (string headers, string cookies) = GetRequestParams();

//            for (int i = 0; i < responses.Length; i++)
//            {
//                string imageUrl = $"{Statics.BASE_URL}/direct/{responses[i]["response_id"]}";
//                string filePath = await DownloadImageAsync(imageUrl, headers, cookies, i);
//            }

//            if (!string.IsNullOrEmpty(filePath) && enableLogging)
//            {
//                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]: Successfully downloaded {responses.Length} images to {outputDir}.");
//            }
//        }

//        private async Task<string> DownloadImageAsync(string imageUrl, string headers, string cookies, int index)
//        {
//            Directory.CreateDirectory(outputDir);

//            string sanitizedPrompt = Regex.Replace(prompt, @"[^\w\s\'-]", "").Replace(" ", "_");
//            string filePath = Path.Combine(outputDir, $"{sanitizedPrompt}_{index}.jpeg");

//            try
//            {
//                using (var client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Add("Accept", headers);
//                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
//                    client.DefaultRequestHeaders.Add("Cookie", cookies);

//                    var response = await client.GetAsync(imageUrl);
//                    response.EnsureSuccessStatusCode();

//                    using (var stream = await response.Content.ReadAsStreamAsync())
//                    using (var fileStream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await stream.CopyToAsync(fileStream);
//                    }
//                }
//                return filePath;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR]: An error occurred: {ex.Message}");
//                return null;
//            }
//        }

//        private (string, string) GetRequestParams()
//        {
//            string headers = "Accept: */*\r\nUser-Agent: Mozilla/5.0";
//            string cookies = $"session_cookie={sessionCookieToken}";
//            return (headers, cookies);
//        }
//    }
//}
