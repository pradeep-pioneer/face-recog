using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LiveRecognitionTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Recognize()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Recognize(PostedData data)
        {
            var returnObject = JsonConvert.DeserializeObject<dynamic>(PostAndRecognize(data));
            if (returnObject.Result.ToString() == "Success")
                return Json(new { Result = returnObject.Result.ToString(), Data = returnObject.Data.ToString() });
            else
                return Json(new { Result = "Failure", Data = data.Image1Data });
        }

        private string PostAndRecognize(PostedData data)
        {
            var client = new HttpClient();
            var postTask = client.PostAsync("http://localhost:7860/api/Recognition", new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.Default, "application/json"));
            postTask.Wait();
            if (postTask.Result.IsSuccessStatusCode)
            {
                var responseReadTask = postTask.Result.Content.ReadAsStringAsync();
                responseReadTask.Wait();
                return responseReadTask.Result;
            }
            else
            {
                return "Error";
            }
        }

        [HttpPost]
        public ActionResult Index(PostedData postedData)
        {
            try
            {
                return Json(PostAndGetResult(postedData));
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Failure", Message = ex.Message });
            }
        }

        private dynamic PostAndGetResult(PostedData data)
        {
            var client = new HttpClient();
            var postTask = client.PostAsync("http://localhost:7860/api/Training", new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.Default, "application/json"));
            postTask.Wait();
            if (postTask.Result.IsSuccessStatusCode)
            {
                var responseReadTask = postTask.Result.Content.ReadAsStringAsync();
                responseReadTask.Wait();
                var sanitized = responseReadTask.Result.Replace("{", "").Replace("}", "").Replace("\"", "").Replace(",", "|").Replace(" ", "").Split('|');
                if (sanitized.Length > 1)
                {
                    var result = sanitized[0].Split(':')[1];
                    var message = sanitized[1].Split(':')[1];
                    return new { Result = result, Message = message };
                }
                else
                {
                    return new { Result = "Failure", Message = "Invalid Response" };
                }
            }
            else
            {
                return new { Result = "Failure", Message = "Unknown Error" };
            }
        }
    }

    public class PostedData
    {
        public string SubjectName { get; set; }
        public string Image1Data { get; set; }
        public string Image2Data { get; set; }
        public string Image3Data { get; set; }
        public string Image4Data { get; set; }
        public string Image5Data { get; set; }

    }
}