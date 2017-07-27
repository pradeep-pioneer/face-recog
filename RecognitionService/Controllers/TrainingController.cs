using RecognitionService.opencv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RecognitionService.Controllers
{
    public class TrainingController : ApiController
    {
        [HttpPost]
        [Route("api/Training")]
        public IHttpActionResult Post(PostedData postedData)
        {
            try
            {
                string resultMessage = null;
                if (OpenCvTrainer.Train(postedData.SubjectName, out resultMessage, postedData.Image1Data, postedData.Image2Data, postedData.Image3Data, postedData.Image4Data, postedData.Image5Data))
                    return Json(new { Result = "Success", Message = resultMessage ?? "Unknown" });
                else
                    return Json(new { Result = "Failure", Message = resultMessage ?? "Unknown" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Failure", Message = ex.Message });
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
