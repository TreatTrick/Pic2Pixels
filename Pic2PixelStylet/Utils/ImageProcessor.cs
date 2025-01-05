using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace Pic2PixelStylet.Utils
{
    internal static class ImageProcessor
    {
        public static BitmapSource ResizeBitmapSource(
            BitmapSource originalBitmap,
            double targetWidth,
            double targetHeight
        )
        {
            var scaleTransform = new ScaleTransform(
                targetWidth / originalBitmap.PixelWidth,
                targetHeight / originalBitmap.PixelHeight
            );
            return new TransformedBitmap(originalBitmap, scaleTransform);
        }

        public static BitmapSource CropImage(
            BitmapSource originalImage,
            double startX,
            double startY,
            double cropWidth,
            double cropHeight,
            double scale
        )
        {
            double scaledWidth = originalImage.PixelWidth * scale;
            double scaledHeight = originalImage.PixelHeight * scale;

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(0, 0, cropWidth, cropHeight)
                );

                Rect destRect = new Rect(startX, startY, scaledWidth, scaledHeight);
                drawingContext.DrawImage(originalImage, destRect);
            }

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)cropWidth,
                (int)cropHeight,
                originalImage.DpiX,
                originalImage.DpiY,
                PixelFormats.Pbgra32
            );

            renderBitmap.Render(drawingVisual);
            return renderBitmap;
        }

        public static BitmapImage ConvertTo96DpiBitmapImage(string imagePath, out bool success)
        {
            success = true;
            if (
                !imagePath.ToLower().EndsWith("jpg")
                && !imagePath.ToLower().EndsWith("png")
                && !imagePath.ToLower().EndsWith("jpeg")
            )
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
