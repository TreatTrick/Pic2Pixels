using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pic2PixelStylet.Utils
{
    internal static class ImageProcessor
    {
        public static BitmapImage ConvertTo96DpiBitmapImage(string imagePath, out bool success)
        {
            success = true;
            if (!imagePath.ToLower().EndsWith("jpg")
                && !imagePath.ToLower().EndsWith("png")
                && !imagePath.ToLower().EndsWith("jpeg"))
            {
                success = false;
                return new BitmapImage();
            }
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                bitmap.SetResolution(96, 96);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
        }
    }
}
