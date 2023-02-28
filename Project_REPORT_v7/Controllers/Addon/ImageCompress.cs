using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class ImageCompress
    {
        //**************************************//
        // Function for compressing image       //
        //  by bipulmgr - codeproject.com       //
        //**************************************//


        /// <summary>
        /// Compressing image to 800.0f while maintaining ratio.
        /// </summary>
        /// <param name="srcImgStream">source image</param>
        /// <param name="targetPath">path to save compressed image</param>
        public static void CompressImage(Stream srcImgStream, string targetPath)
        {
            try
            {
                var image = Image.FromStream(srcImgStream);

                float maxHeight = 800.0f;
                float maxWidth = 800.0f;
                int newWidth, newHeight;

                var originalBMP = new Bitmap(srcImgStream);
                int originalWidth = originalBMP.Width;
                int originalHeight = originalBMP.Height;

                if (originalWidth > maxWidth || originalHeight > maxHeight)
                {
                    float ratioX = (float)maxWidth / originalWidth;
                    float ratioY = (float)maxHeight / originalHeight;
                    float ratio = Math.Min(ratioX, ratioY);
                    newWidth = (int)(ratio * originalWidth);
                    newHeight = (int)(ratio * originalHeight);
                }
                else
                {
                    newWidth = originalWidth;
                    newHeight = originalHeight;
                }

                var bitmap = new Bitmap(originalBMP, newWidth, newHeight);
                var imgGraph = Graphics.FromImage(bitmap);

                imgGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                imgGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

                var extension = Path.GetExtension(targetPath).ToLower();

                if (extension == ".png" || extension == ".gif")
                {
                    bitmap.Save(targetPath, image.RawFormat);
                }
                else if(extension == ".jpg" || extension == ".jpeg")
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    Encoder encoder = Encoder.Quality;
                    var encoderParameters = new EncoderParameters();
                    var parameter = new EncoderParameter(encoder, 50L);
                    encoderParameters.Param[0] = parameter;

                    bitmap.Save(targetPath, jpgEncoder, encoderParameters);
                }

                // Disposing
                bitmap.Dispose();
                imgGraph.Dispose();
                originalBMP.Dispose();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally 
            {
                
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}