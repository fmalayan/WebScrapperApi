using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WebScrapper.Services;
using Microsoft.Extensions.Configuration;
using Google.Cloud.Vision.V1;

namespace WebScrapper.Controllers
{
    [ApiController]
    public class ScrappingController : ControllerBase
    {
        private readonly string _API_KEY;
        private readonly string _BUSINESS_ID;

        public ScrappingController(IConfiguration config)
        {
            _API_KEY = config["API_KEY"];
            _BUSINESS_ID = config["BUSINESS_ID"];
        }

        [Route("GetScrapResult")]
        [AcceptVerbs("GET")]

        public async Task<string> GetScrapResult()
        {
            HttpClient hc = new HttpClient();
            hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _API_KEY);
            var response = await hc.GetAsync($"https://api.yelp.com/v3/businesses/{_BUSINESS_ID}/reviews");
            string responseBody = await response.Content.ReadAsStringAsync(); //Json result
            var rootobject = JsonConvert.DeserializeObject<Root>(responseBody); //Convert to object

            foreach(var listval in rootobject.reviews)
            {
                //var imageurl = listval.user.image_url.Trim();
                //var client = ImageAnnotatorClient.Create();
                //var image = Image.FromUri("gs://cloud-vision-codelab/" + imageurl);
                //var face_response = client.DetectFaces(image);
                //foreach (var annotation in face_response)
                //{
                //    listval.user.joyLikelihood = annotation.SurpriseLikelihood.ToString().Trim();
                //}
                //commented due to GOOGLE_APPLICATION_CREDENTIALS error

                listval.user.joyLikelihood = "Likely"; //Test insert for user new parameter
            }

            var json = JsonConvert.SerializeObject(rootobject); //Convert back to Json format

            return json;
        }

        public class Review
        {
            public string id { get; set; }
            public string url { get; set; }
            public string text { get; set; }
            public int rating { get; set; }
            public string time_created { get; set; }
            public User user { get; set; }
        }

        public class Root
        {
            public List<Review> reviews { get; set; }
            public int total { get; set; }
            public List<string> possible_languages { get; set; }
        }

        public class User
        {
            public string id { get; set; }
            public string profile_url { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string joyLikelihood { get; set; }
        }
    }
}
