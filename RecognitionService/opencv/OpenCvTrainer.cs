using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;

namespace RecognitionService.opencv
{
    public static class OpenCvTrainer
    {
        public static Dictionary<int, string> Subjects { get; set; }
        public static CascadeClassifier DetectionClassifier { get; set; }

        public static LBPHFaceRecognizer FaceRecognizer { get; set; }

        static OpenCvTrainer()
        {
            Subjects = new Dictionary<int, string>();
            var haarPath = @"haarcascade_frontalface_default.xml";
            DetectionClassifier = new CascadeClassifier(haarPath);
            FaceRecognizer = new LBPHFaceRecognizer();
        }

        public static dynamic Predict(string imageData)
        {
            var rawBytes = Convert.FromBase64String(imageData.Replace("data:image/png;base64,", ""));
            MemoryStream stream = new MemoryStream();
            stream.Write(rawBytes, 0, rawBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            Bitmap bitmap = new Bitmap(stream);
            var original = new Image<Bgr, byte>(bitmap);
            Image<Gray, byte> grey = new Image<Gray, byte>(400, 300);
            CvInvoke.CvtColor(original, grey, ColorConversion.Bgr2Gray);
            var rects = OpenCvTrainer.DetectionClassifier.DetectMultiScale(grey);
            if (rects.Length > 0)
            {
                var rect = rects[0];
                var facialFeatures = grey.GetSubRect(rect);
                var resized = facialFeatures.Resize(100, 100, Inter.Cubic);
                var prediction = FaceRecognizer.Predict(resized);
                var name = Subjects[prediction.Label];
                original.Draw(rect, new Bgr(Color.Pink));
                CvInvoke.PutText(original, name, new Point(rect.Left, rect.Top), FontFace.HersheyPlain,2,new MCvScalar(255,255,255));
                return new { Result = "Success", Data = getImageString(original.Bitmap) };
            }
            else
            {
                return new { Result = "Failure" };
            }
        }

        private static string getImageString(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes));
        }

        public static bool Train(string subjectName, out string message, params string[] imagesData)
        {
            try
            {
                var subjectId = -1;
                if (Subjects.Count(x => x.Value.ToLower().Equals(subjectName.ToLower())) == 0)
                    Subjects.Add(Subjects.Count, subjectName.ToLower());
                else
                    subjectId = Subjects.FirstOrDefault(x => x.Value.ToLower().Equals(subjectName.ToLower())).Key;
                List<Image<Gray, byte>> samples = new List<Image<Gray, byte>>();
                List<int> labels = new List<int>();
                foreach (var item in imagesData)
                {
                    var rawBytes = Convert.FromBase64String(item.Replace("data:image/png;base64,", ""));
                    MemoryStream stream = new MemoryStream();
                    stream.Write(rawBytes, 0, rawBytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    Bitmap bitmap = new Bitmap(stream);
                    Image<Gray, byte> grey = new Image<Gray, byte>(400, 300);
                    CvInvoke.CvtColor(new Image<Bgr, byte>(bitmap), grey, ColorConversion.Bgr2Gray);
                    var rects = OpenCvTrainer.DetectionClassifier.DetectMultiScale(grey);
                    if (rects.Length > 0)
                    {
                        var rect = rects[0];
                        var facialFeatures = grey.GetSubRect(rect);
                        var resized = facialFeatures.Resize(100, 100, Inter.Cubic);
                        samples.Add(resized);
                        labels.Add(subjectId);
                    }
                }
                if (samples.Count > 0)
                {
                    //now train the recognizer
                    OpenCvTrainer.FaceRecognizer.Train(samples.ToArray(), labels.ToArray());
                    message = "Success";
                }
                else
                {
                    message = "Failed";
                    return false;
                }

            }
            catch (Exception ex)
            {
                message = string.Format("Error: {0}\nMessage:{1}", ex, ex.Message);
                return false;
            }
            return true;
        }
    }
}
