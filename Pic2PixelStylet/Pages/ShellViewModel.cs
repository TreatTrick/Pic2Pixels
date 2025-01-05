using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Pic2PixelStylet.Utils;
using Stylet;
using Point = System.Windows.Point;

namespace Pic2PixelStylet.Pages
{
    public class ShellViewModel : Screen
    {
        #region PrivateClass
        private struct CellInfo
        {
            public bool IsBlue;
            public Border CellBorder;
            public int Row;
            public int Column;

            public override string ToString()
            {
                return $"{Row + 1}行，{Column + 1}列";
            }
        }
        #endregion

        #region Fields
        private CellInfo[,] _cells;
        private BitmapImage _origianlImage;
        private string _originalImagePath;
        private double _canvasContainerWidth;
        private double _canvasContainerHeight;
        private double _cropAreaWidth;
        private double _cropAreaHeight;
        #endregion

        #region properties
        public bool IsImagedLoaded => _origianlImage != null;
        public double ScaleFactor { get; set; }
        public double ImageLeft { get; set; }
        public double ImageTop { get; set; }
        public double CropLeft { get; set; }
        public double CropTop { get; set; }
        public bool IsCropped { get; set; } = false;
        public int PixelCols { get; set; }
        public int PixelRows { get; set; }
        public int Threshold { get; set; }
        public Rect CropRect { get; set; }
        public Rect ContainerRect { get; set; }

        public BitmapImage OrigianlImage
        {
            get => _origianlImage;
            set { _origianlImage = value; }
        }

        public string OriginalImagePath
        {
            get => _originalImagePath;
            set
            {
                OnOriginalImagePathChanged(value);
                _originalImagePath = value;
            }
        }

        public double CropAreaWidth
        {
            get => _cropAreaWidth;
            set { _cropAreaWidth = value; }
        }

        public double CropAreaHeight
        {
            get => _cropAreaHeight;
            set { _cropAreaHeight = value; }
        }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            PixelRows = 20;
            PixelCols = 51;
            _cells = new CellInfo[PixelRows, PixelCols];
            Threshold = 128;
            ScaleFactor = 1;
        }
        #endregion

        #region PublicMethods
        public void ResizeCanvasCommand(System.Windows.Size size)
        {
            _canvasContainerWidth = size.Width;
            _canvasContainerHeight = size.Height;
            ResizeCropArea();
            ResizeImage();
        }

        public void ZoomCommand(int Delta)
        {
            double zoomFactor = Delta > 0 ? 1.1 : 0.9;
            ScaleFactor *= zoomFactor;
        }

        public void DragImage(System.Windows.Point offsetPoint)
        {
            ImageLeft += offsetPoint.X;
            ImageTop += offsetPoint.Y;
        }
        #endregion

        #region PrivateMethods
        private void OnOriginalImagePathChanged(string path)
        {
            if (path == _originalImagePath)
                return;
            _originalImagePath = path;
            OrigianlImage = ImageProcessor.ConvertTo96DpiBitmapImage(path, out bool success);
            if (!success)
            {
                return;
            }
            ResizeImage();
            NotifyOfPropertyChange(nameof(IsImagedLoaded));
        }

        private void ResizeImage()
        {
            if (OrigianlImage == null)
                return;
            double imageX = OrigianlImage.Width;
            double imageY = OrigianlImage.Height;
            if (imageX > imageY)
            {
                ScaleFactor = _canvasContainerWidth / imageX;
            }
            else
            {
                ScaleFactor = _canvasContainerHeight / imageY;
            }
            ImageLeft = (_canvasContainerWidth - imageX * ScaleFactor) / 2;
            ImageTop = (_canvasContainerHeight - imageY * ScaleFactor) / 2;
        }

        private void ResizeCropArea()
        {
            if (PixelRows > PixelCols)
            {
                CropAreaHeight = _canvasContainerHeight;
                CropAreaWidth = CropAreaHeight * PixelCols / PixelRows;
            }
            else
            {
                CropAreaWidth = _canvasContainerWidth;
                CropAreaHeight = CropAreaWidth * PixelRows / PixelCols;
            }

            CropLeft = (_canvasContainerWidth - CropAreaWidth) / 2;
            CropTop = (_canvasContainerHeight - CropAreaHeight) / 2;
            ContainerRect = new Rect(0, 0, _canvasContainerWidth, _canvasContainerHeight);
            CropRect = new Rect(CropLeft, CropTop, _cropAreaWidth, _cropAreaHeight);
        }
        #endregion
    }
}
