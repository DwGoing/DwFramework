using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using OpenCvSharp;
using Tesseract;

namespace DwFramework.Media
{
    public static class Recognizer
    {
        #region 身份证
        /// <summary>
        /// 识别身份证
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dataPath"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static object RecognizeIdCard(Mat input, string dataPath = "Resources", string lang = "chi_sim+eng")
        {
            using var resizedMat = input.Resize(new Size(856, 540));
            var nameRect = new OpenCvSharp.Rect(20, 30, 500, 110);
            using var nameMat = resizedMat[nameRect];
            var sexRect = new OpenCvSharp.Rect(20, 105, 230, 105);
            using var sexMat = resizedMat[sexRect];
            var nationRect = new OpenCvSharp.Rect(210, 105, 310, 105);
            using var nationMat = resizedMat[nationRect];
            var birthdayRect = new OpenCvSharp.Rect(20, 175, 500, 100);
            using var birthdayMat = resizedMat[birthdayRect];
            var addressRect = new OpenCvSharp.Rect(20, 235, 550, 160);
            using var addressMat = resizedMat[addressRect];
            var numberRect = new OpenCvSharp.Rect(20, 400, 810, 130);
            using var numberMat = resizedMat[numberRect];

            var result = new
            {
                Name = RecognizeIdCardContext(nameMat, dataPath: dataPath, lang: lang),
                Sex = RecognizeIdCardContext(sexMat, mode: PageSegMode.SingleChar, dataPath: dataPath, lang: lang),
                Nation = RecognizeIdCardContext(nationMat, dataPath: dataPath, lang: lang),
                Birthday = RecognizeIdCardContext(birthdayMat, true, PageSegMode.SingleChar, dataPath: dataPath, lang: lang),
                Address = RecognizeIdCardContext(addressMat, dataPath: dataPath, lang: lang),
                Number = RecognizeIdCardContext(numberMat, dataPath: dataPath, lang: lang)
            };
            return result;
        }

        /// <summary>
        /// 识别身份证内容
        /// </summary>
        /// <param name="input"></param>
        /// <param name="multi"></param>
        /// <param name="mode"></param>
        /// <param name="dataPath"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        private static string RecognizeIdCardContext(Mat input, bool multi = false, PageSegMode mode = PageSegMode.Auto, string dataPath = "Resources", string lang = "chi_sim+eng")
        {
            using var rChannelMat = Cv2.Split(input)[0];
            using var gaussianBlurMat = rChannelMat.GaussianBlur(new Size(3, 3), 0);
            using var thresholdMat = gaussianBlurMat.Threshold(0, 255, ThresholdTypes.Otsu | ThresholdTypes.BinaryInv);
            using var medianBlurMat = thresholdMat.MedianBlur(5);
            using var dilateKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(20, 20));
            using var dilateMat = medianBlurMat.Dilate(dilateKernel, iterations: 1);
            dilateMat.FindContours(out var points, out var hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
            if (hierarchy.Length <= 0) return null;
            using var engine = new TesseractEngine(dataPath, lang, EngineMode.Default);
            if (multi)
            {
                var mats = new List<(int X, Mat Mat)>();
                for (var i = 0; i >= 0; i = hierarchy[i].Next)
                {
                    var contour = points[i];
                    var boundingRect = Cv2.BoundingRect(contour);
                    mats.Add((boundingRect.TopLeft.X, rChannelMat[boundingRect]));
                }
                var builder = new StringBuilder();
                foreach (var item in mats.OrderBy(item => item.X))
                {
                    using var pix = Pix.LoadFromMemory(item.Mat.ToBytes());
                    using var page = engine.Process(pix, mode);
                    builder.Append(page.GetText());
                }
                return builder.ToString();
            }
            else
            {
                OpenCvSharp.Rect? mainRect = null;
                for (var i = 0; i >= 0; i = hierarchy[i].Next)
                {
                    var contour = points[i];
                    var boundingRect = Cv2.BoundingRect(contour);
                    if (mainRect == null || mainRect?.Width * mainRect?.Height < boundingRect.Width * boundingRect.Height)
                        mainRect = boundingRect;
                }
                if (mainRect == null) return null;
                using var pix = Pix.LoadFromMemory(rChannelMat[mainRect.Value].ToBytes());
                using var page = engine.Process(pix, mode);
                return page.GetText();
            }
        }
        #endregion
    }
}
