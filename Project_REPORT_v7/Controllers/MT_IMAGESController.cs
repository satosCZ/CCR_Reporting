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
    /// <summary>
    /// MT_IMAGESController class
    /// </summary>
    public class MT_IMAGESController : Controller
    {
        // Private variable declaration for database
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: Images by MainTaskID for Gallery
        /// </summary>
        /// <param name="id">Guid: Id of MainTask data</param>
        /// <returns>Modal window with all the matching images</returns>
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

        /// <summary>
        /// POST: Upload image to server and save path to database
        /// </summary>
        /// <param name="mainTaskID">Guid: Id of MainTask where images are uploaded</param>
        /// <returns>Json return of success or failure</returns>
        [HttpPost]
        public ActionResult UploadImage( Guid mainTaskID )
        {
            // Get the uploaded image from the Files collection
            var mt_images = db.MT_IMAGES.Where(x => x.ReportID == mainTaskID).ToList();

            // Declare image folder path
            var _imageFolder = new DirectoryInfo(string.Format("{0}Images\\", Server.MapPath(@"\")));

            // Declare database image path
            string dbImagePath = "/Images/";

            // Boolean variable for success or failure of saving image
            bool isSavedSuccessfully = true;

            // Declare file name
            string fName = "";
            try
            {
                // Loop through all the files in Files collection for uploading
                foreach ( string fileName in Request.Files )
                {
                    // Get the uploaded file
                    HttpPostedFileBase file = Request.Files[fileName];

                    // Set file name
                    fName = file.FileName;

                    // Declare MT_IMAGES object
                    MT_IMAGES mT_IMAGES = new MT_IMAGES();

                    // Check if file is not null and has content
                    if ( file != null && file.ContentLength > 0 )
                    {
                        // Set image Guid and MainTaskID
                        mT_IMAGES.ImageID = Guid.NewGuid();
                        mT_IMAGES.ReportID = mainTaskID;

                        // Creating path to save image in server with current date as folder name
                        var path = Path.Combine(_imageFolder.ToString(), DateTime.UtcNow.ToString("yyyyMMdd"));

                        // Creating path to save image in database with current date as folder name
                        var dbPath = Path.Combine(dbImagePath, DateTime.UtcNow.ToString("yyyyMMdd"));

                        // Adding to path, folder name
                        var folder = Path.Combine(path, mT_IMAGES.ReportID.ToString());

                        // Adding to path for db, folder name
                        var dbFolder = Path.Combine(dbPath, mT_IMAGES.ReportID.ToString());

                        // Getting file name
                        var fileName1 = Path.GetFileName(file.FileName);

                        // Checking if folder exists, if not create folder
                        bool isExists = System.IO.Directory.Exists(folder);

                        // If folder does not exists, create folder
                        if ( !isExists )
                            System.IO.Directory.CreateDirectory( folder );

                        // Creating path to save image in server
                        var uploadFilePath = string.Format("{0}\\{1}", folder, fileName1);

                        // Calling CompressImage method to compress image with save path and file name
                        CompressImage( file, uploadFilePath, fileName1 );

                        // Creating path to save image in database
                        string dbFinalPath = string.Format("{0}/{1}", dbFolder, fileName1);

                        // Setting image path to database
                        mT_IMAGES.ImagePath = dbFinalPath;

                        // Adding image to database
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
            // Error in Try Catch
            catch ( Exception ex )
            {
                isSavedSuccessfully = false;
                if ( int.TryParse( Session ["UserID"].ToString(), out int userID ) )
                {
                    LogHelper.AddLog( DateTime.UtcNow, "MT_IMAGES | ADD | ERROR", $" ERROR MSG: {ex.Message}", userID );
                }
            }
            // If image is saved successfully return success message else return error message
            if ( isSavedSuccessfully )
            {
                return Json( new { Message = fName } );
            }
            else
            {
                return Json( new { Message = "Error in saving file" } );
            }
        }

        /// <summary>
        /// POST: Delete image from server and database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete( Guid? id )
        {
            // Check if id is null
            if ( id == null )
            {
                return new HttpStatusCodeResult( System.Net.HttpStatusCode.BadRequest );
            }

            // Get image from database
            MT_IMAGES mT_IMAGES = db.MT_IMAGES.Find(id);

            // Check if image is null
            if ( mT_IMAGES == null )
            {
                return HttpNotFound();
            }

            // Declare image path
            var relPath = mT_IMAGES.ImagePath.Replace("/","\\");
            // Declare full path
            var fullPath = Server.MapPath(mT_IMAGES.ImagePath);
            try
            {
                // Check if file exists, if exists delete file
                if ( System.IO.File.Exists( fullPath ) )
                {
                    System.IO.File.Delete( fullPath );
                }
                // Delete image from database
                db.MT_IMAGES.Remove( mT_IMAGES );

                // Save changes to database
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

        /// <summary>
        /// Compress image method
        /// 
        /// Helped with StackOverflow
        /// </summary>
        /// <param name="file">Uploaded file</param>
        /// <param name="uploadPath">Path of file</param>
        /// <param name="fileName">Name of file</param>
        public void CompressImage( HttpPostedFileBase file, string uploadPath, string fileName )
        {
            try
            {
                using ( var image = Image.FromStream( file.InputStream, true, true ) )
                {
                    // Set maximum of Height or Width for compression
                    float maxHeight = 1100.0f;
                    float maxWidth = 1100.0f;

                    // Declare variables for calculated new dimensions
                    int newWidth, newHeight;

                    // Save extension of uploaded file
                    string extension = Path.GetExtension(fileName);

                    // Create bitmap from uploaded image
                    Bitmap originalBMP = new Bitmap(file.InputStream);

                    // Get Width and Height of image
                    int originalWidth = originalBMP.Width;
                    int originalHeight = originalBMP.Height;

                    // Check if image is bigger than max allowed resolution
                    if ( originalWidth > maxWidth || originalHeight > maxHeight )
                    {
                        // Calculate ratio so then image will have same dimension/orientation/proportion but at lower size
                        float ratioX = (float)maxWidth / (float)originalWidth;
                        float ratioY = (float)maxHeight / (float)originalHeight;
                        float ratio = Math.Min(ratioX, ratioY);

                        // Calculate new Width and Height by ratio
                        newWidth = ( int )(originalWidth * ratio);
                        newHeight = ( int )(originalHeight * ratio);
                    }
                    // If not then use original width and height
                    else
                    {
                        newWidth = originalWidth;
                        newHeight = originalHeight;
                    }

                    // Declare new bitmap with uploaded image and new height and width
                    Bitmap bitmap = new Bitmap(originalBMP, newWidth, newHeight);

                    Graphics g = Graphics.FromImage(bitmap);

                    // Compress by extensions
                    if ( extension == ".png" || extension == ".gif" || extension == ".PNG" || extension == ".GIF" )
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage( originalBMP, 0, 0, newWidth, newHeight );

                        bitmap.Save( uploadPath, image.RawFormat );

                        bitmap.Dispose();
                        g.Dispose();
                        originalBMP.Dispose();
                    }
                    else if ( extension == ".jpg" || extension == ".JPG" || extension == ".jpeg" || extension == ".JPEG" )
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

        /// <summary>
        /// Method for encoding jpeg images
        /// </summary>
        /// <param name="jpeg"></param>
        /// <returns></returns>
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