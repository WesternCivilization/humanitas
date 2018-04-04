using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SqlCache
{
    internal static class ImageHelper
    {

        internal static string ResizeImage(string image, int maxWidth, int maxHeight, string destinationPath)
        {
            if (image == null || image.Length == 0) return string.Empty;
            byte[] rebin = Convert.FromBase64String(image);
            var tempFileName = Path.Combine(Path.GetDirectoryName(destinationPath), $"{Guid.NewGuid()}.jpg");
            using (FileStream fs2 = new FileStream(tempFileName, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs2))
                bw.Write(rebin);
            using (var img = Bitmap.FromFile(tempFileName))
            {
                using (var bitmap = new Bitmap(img))
                {
                    ResizeImage(bitmap, maxWidth, maxHeight, 100, destinationPath);
                }
            }
            if (File.Exists(tempFileName)) File.Delete(tempFileName);
            return BinaryHelper.FromImageToString(destinationPath);
        }

        internal static string ResizeImageW(string image, int maxWidth, string destinationPath)
        {
            if (string.IsNullOrEmpty(image)) return image;
            var result = string.Empty;
            byte[] rebin = Convert.FromBase64String(image);
            var tempFileName = Path.Combine(Path.GetDirectoryName(destinationPath), $"{Guid.NewGuid()}.jpg");
            using (FileStream fs2 = new FileStream(tempFileName, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs2))
                bw.Write(rebin);
            using (var img = Bitmap.FromFile(tempFileName))
            {
                using (var bitmap = new Bitmap(img))
                {
                    var imgPath = !string.IsNullOrEmpty(destinationPath) ?
                        destinationPath : Path.ChangeExtension(tempFileName, ".jpg");
                    ResizeImage(bitmap, maxWidth, 9999, 100, imgPath);
                    result = BinaryHelper.FromImageToString(imgPath);
                }
            }
            File.Delete(tempFileName);
            File.Delete(Path.ChangeExtension(tempFileName, ".jpg"));
            return result;
        }

        internal static string ResizeImageH(string image, int maxHeight, string destinationPath)
        {
            if (string.IsNullOrEmpty(image)) return image;
            var result = string.Empty;
            byte[] rebin = Convert.FromBase64String(image);
            var tempFileName = Path.Combine(Path.GetDirectoryName(destinationPath), $"{Guid.NewGuid()}.jpg");
            using (FileStream fs2 = new FileStream(tempFileName, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs2))
                bw.Write(rebin);
            using (var img = Bitmap.FromFile(tempFileName))
            {
                using (var bitmap = new Bitmap(img))
                {
                    var imgPath = !string.IsNullOrEmpty(destinationPath) ?
                        destinationPath : Path.ChangeExtension(tempFileName, ".jpg");
                    ResizeImage(bitmap, 9999, maxHeight, 100, imgPath);
                    result = BinaryHelper.FromImageToString(imgPath);
                }
            }
            File.Delete(tempFileName);
            File.Delete(Path.ChangeExtension(tempFileName, ".jpg"));
            return result;
        }

        internal static Size GetImageSize(string image)
        {
            if (image.EndsWith(".jpg"))
            {
                var result = new Size();
                using (var bitmap = new Bitmap(image))
                {
                    result.Width = bitmap.Width;
                    result.Height = bitmap.Height;
                }
                return result;
            }
            else
            {
                var result = new Size();
                if (string.IsNullOrEmpty(image)) return result;
                byte[] rebin = Convert.FromBase64String(image);
                var tempFileName = Path.GetTempFileName();
                using (FileStream fs2 = new FileStream(tempFileName, FileMode.Create))
                using (BinaryWriter bw = new BinaryWriter(fs2))
                    bw.Write(rebin);
                using (var img = Bitmap.FromFile(tempFileName))
                {
                    using (var bitmap = new Bitmap(img))
                    {
                        result.Width = bitmap.Width;
                        result.Height = bitmap.Height;
                    }
                }
                File.Delete(tempFileName);
                File.Delete(Path.ChangeExtension(tempFileName, ".jpg"));
                return result;
            }
        }

        static internal void ResizeImage(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            using (var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb))
            {
                // Draws the image in the specified size with quality mode set to HighQuality
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                // Get an ImageCodecInfo object that represents the JPEG codec.
                ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                // Create an Encoder object for the Quality parameter.
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object. 
                EncoderParameters encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.
                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter;
                newImage.Save(filePath, imageCodecInfo, encoderParameters);
            }
        }

        static internal ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

    }
}
