﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        // 计算 BitmapSource 的 SHA256 哈希
        public static string GetImageHash(BitmapSource bitmapSource)
        {
            if (bitmapSource == null)
                throw new ArgumentNullException(nameof(bitmapSource));

            // 使用 PNG 编码器将 BitmapSource 转为内存流（也可改用其他编码器如 BmpBitmapEncoder）
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Position = 0; // 重置流位置

                // 计算哈希
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(memoryStream);
                    return BytesToHexString(hashBytes);
                }
            }
        }

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
                return BitmapToBitmapImage(bitmap);
            }
        }

        public static BitmapImage ConvertTo96DpiBitmapImage(
            BitmapSource bitmapSource,
            out bool success
        )
        {
            success = true;
            if (bitmapSource == null)
            {
                success = false;
                return new BitmapImage();
            }
            using (Bitmap bitmap = BitmapSourceToBitmap(bitmapSource))
            {
                return BitmapToBitmapImage(bitmap);
            }
        }

        #region Private Methods
        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
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

        private static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            if (bitmapSource == null)
                throw new ArgumentNullException(nameof(bitmapSource));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);
                using (Bitmap bitmap = new Bitmap(memoryStream))
                {
                    return new Bitmap(bitmap);
                }
            }
        }

        // 辅助方法：将字节数组转为十六进制字符串
        private static string BytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2")); // "x2" 表示两位小写十六进制
            }
            return sb.ToString();
        }
        #endregion
    }
}
