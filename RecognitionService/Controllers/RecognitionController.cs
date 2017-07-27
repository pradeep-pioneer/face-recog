using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RecognitionService.Controllers
{
    public class RecognitionController : ApiController
    {
        [HttpPost]
        [Route("api/Recognition")]
        public IHttpActionResult Post(PostedData postedData)
        {
            return Json(opencv.OpenCvTrainer.Predict(postedData.Image1Data));
        }
    }
}
