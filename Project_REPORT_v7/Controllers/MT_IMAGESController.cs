using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers
{
    public class MT_IMAGESController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        [HttpGet]
        public ActionResult Gallery( Guid? id )
        {
            if ( id == null )
            {
                return new HttpStatusCodeResult( System.Net.HttpStatusCode.BadRequest );
            }
            var mt_images = db.MT_IMAGES.Where(x => x.ReportID == id).ToList();
            if ( mt_images == null )
            {
                return HttpNotFound();
            }
            return PartialView( "Gallery", mt_images );
        }

        [HttpPost]
        public ActionResult UploadImage( Guid mainTaskID )
        {
            var mt_images = db.MT_IMAGES.Where(x => x.ReportID == mainTaskID).ToList();
            var _imageFolder = new DirectoryInfo(string.Format("{0}Images\\", Server.MapPath(@"\")));
            string dbImagePath = "/Images/";
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach ( string fileName in Request.Files )
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    fName = file.FileName;
                    MT_IMAGES mT_IMAGES = new MT_IMAGES();
                    if ( file != null && file.ContentLength > 0 )
                    {
                        mT_IMAGES.ImageID = Guid.NewGuid();
                        mT_IMAGES.ReportID = mainTaskID;
                        var path = Path.Combine(_imageFolder.ToString(), DateTime.UtcNow.ToString("yyyyMMdd"));
                        var dbPath = Path.Combine(dbImagePath, DateTime.UtcNow.ToString("yyyyMMdd"));
                        var folder = Path.Combine(path, mT_IMAGES.ReportID.ToString());
                        var dbFolder = Path.Combine(dbPath, mT_IMAGES.ReportID.ToString());
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = System.IO.Directory.Exists(folder);
                        if ( !isExists )
                            System.IO.Directory.CreateDirectory( folder );
                        var uploadFilePath = string.Format("{0}\\{1}", folder, fileName1);

                        CompressImage( file, uploadFilePath, fileName1 );
                        string dbFinalPath = string.Format("{0}/{1}", dbFolder, fileName1);

                        mT_IMAGES.ImagePath = dbFinalPath;
                        try
                        {
                            db.MT_IMAGES.Add( mT_IMAGES );
                            db.SaveChanges();
                        }
                        catch ( Exception ex )
                        {
                            if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                            {
                                LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | ADD | ERROR", $"MainTaskID: {mainTaskID} | IMAGE ID: {mT_IMAGES.ImageID} | PATH: {mT_IMAGES.ImagePath} | ERROR MSG: {ex.Message}", userID );
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                isSavedSuccessfully = false;
                if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                {
                    LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | ADD | ERROR", $" ERROR MSG: {ex.Message}", userID );
                }
            }
            if ( isSavedSuccessfully )
            {
                return Json( new { Message = fName } );
            }
            else
            {
                return Json( new { Message = "Error in saving file" } );
            }
        }

        public ActionResult Delete( Guid? id )
        {
            if ( id == null )
            {
                return new HttpStatusCodeResult( System.Net.HttpStatusCode.BadRequest );
            }

            MT_IMAGES mT_IMAGES = db.MT_IMAGES.Find(id);
            if ( mT_IMAGES == null )
            {
                return HttpNotFound();
            }

            var relPath = mT_IMAGES.ImagePath.Replace("/","\\");
            var fullPath = Server.MapPath(mT_IMAGES.ImagePath);
            try
            {
                if ( System.IO.File.Exists( fullPath ) )
                {
                    System.IO.File.Delete( fullPath );
                }
                db.MT_IMAGES.Remove( mT_IMAGES );
                db.SaveChanges();
            }
            catch ( Exception ex )
            {
                if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                {
                    LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | DELETE | ERROR", $"MainTaskID: {mT_IMAGES.ReportID} | IMAGE ID: {mT_IMAGES.ImageID} | PATH: {mT_IMAGES.ImagePath} | ERROR MSG: {ex.Message}", userID );
                }
            }
            finally
            {
                if ( mT_IMAGES != null )
                {
                    mT_IMAGES = null;
                }
                if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                {
                    LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | DELETE", $"MainTaskID: {mT_IMAGES.ReportID} | IMAGE ID: {mT_IMAGES.ImageID} | PATH: {mT_IMAGES.ImagePath}", userID );
                }
            }
            return Json( new { success = true }, JsonRequestBehavior.AllowGet );
        }

        public void CompressImage( HttpPostedFileBase file, string uploadPath, string fileName )
        {
            try
            {
                using ( var image = Image.FromStream( file.InputStream, true, true ) )
                {
                    float maxHeight = 1100.0f;
                    float maxWidth = 1100.0f;
                    int newWidth, newHeight;
                    string extension = Path.GetExtension(fileName);
                    Bitmap originalBMP = new Bitmap(file.InputStream);
                    int originalWidth = originalBMP.Width;
                    int originalHeight = originalBMP.Height;

                    if ( originalWidth > maxWidth || originalHeight > maxHeight )
                    {
                        float ratioX = (float)maxWidth / (float)originalWidth;
                        float ratioY = (float)maxHeight / (float)originalHeight;
                        float ratio = Math.Min(ratioX, ratioY);

                        newWidth = ( int )(originalWidth * ratio);
                        newHeight = ( int )(originalHeight * ratio);
                    }
                    else
                    {
                        newWidth = originalWidth;
                        newHeight = originalHeight;
                    }

                    Bitmap bitmap = new Bitmap(originalBMP, newWidth, newHeight);
                    Graphics g = Graphics.FromImage(bitmap);
                    if ( extension == ".png" || extension == ".gif" )
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage( originalBMP, 0, 0, newWidth, newHeight );

                        bitmap.Save( uploadPath, image.RawFormat );

                        bitmap.Dispose();
                        g.Dispose();
                        originalBMP.Dispose();
                    }
                    else if ( extension == ".jpg" )
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage( originalBMP, 0, 0, newWidth, newHeight );
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        Encoder encoder = Encoder.Quality;
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        EncoderParameter encoderParameter = new EncoderParameter(encoder, 50L);
                        encoderParameters.Param [0] = encoderParameter;
                        bitmap.Save( uploadPath, jpgEncoder, encoderParameters );

                        bitmap.Dispose();
                        g.Dispose();
                        originalBMP.Dispose();
                    }
                }
            }
            catch ( Exception ex )
            {
                if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                {
                    LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | COMPRESS IMAGE | ERROR", $"ERROR MSG: {ex.Message}", userID );
                }
            }
        }

        private ImageCodecInfo GetEncoder( ImageFormat jpeg )
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach ( ImageCodecInfo codec in codecs )
            {
                if ( codec.FormatID == jpeg.Guid )
                {
                    return codec;
                }
            }
            return null;
        }
    }
}