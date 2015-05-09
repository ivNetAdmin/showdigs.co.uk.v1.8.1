
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace ivNet.Listing.Helpers
{
    public static class ImageHelper
    {        
        public static void UploadImage(HttpPostedFileBase file, string fileName, string filePath, int width, int height)
        {
            var bitmapImage = Image.FromStream(file.InputStream, true, true);
            var resizeHeight = height - 1;
            var cropWidth = width - 1;

            // Encoder parameter for image quality
            var qualityParam = new EncoderParameter(Encoder.Quality, 100);

            // Jpeg image codec
            var jpegCodec = ImageCodecInfo.GetImageEncoders().First(codecInfo => codecInfo.MimeType == "image/jpeg");

            if (jpegCodec == null)
                return;

            using (var encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = qualityParam;
               
                var isExists = Directory.Exists(filePath);
                if (!isExists)
                {
                    Directory.CreateDirectory(filePath);
                }

                var fullPath = string.Format("{0}\\{1}", filePath, fileName);

                var imageWidth = bitmapImage.Width;
                var imageHeight = bitmapImage.Height;
                var resizeWidth = (resizeHeight*imageWidth/imageHeight) - 1;

                var resizeImage = ResizeImage(bitmapImage, new Size(resizeWidth, resizeHeight));

                if (resizeWidth > width)
                {                 
                    var x = (resizeWidth - width) / 2;
                    var rectangle = new Rectangle(x, 0, cropWidth, resizeHeight);
                    var cropImage = CropImage(resizeImage, rectangle);
                    cropImage.Save(fullPath);
                }
                else
                {
                    resizeImage.Save(fullPath);
                }
            }
        }

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            //float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            //nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destHeight = (int) (size.Height);
            var destWidth = (int) (size.Height*sourceWidth/sourceHeight);
            
            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private static Image CropImage(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }
    }
}