using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Pic2PixelStylet.DbConnection;
using Pic2PixelStylet.Utils;
using Stylet;
using StyletIoC;
using Point = System.Windows.Point;

namespace Pic2PixelStylet.Pages
{
    public class ShellViewModel : Screen
    {
        #region Fields
        private CellInfo[,] _cells;
        private BitmapImage _originalImage;
        private string _imageHash;
        private double _canvasContainerWidth;
        private double _canvasContainerHeight;
        private double _cropAreaWidth;
        private double _cropAreaHeight;
        private int _threshold;
        private FormatConvertedBitmap _grayBitmap;
        private double _imageLeftToCropAreaLeftRatio = 0;
        private double _imageTopToCropAreaTopRatio = 0;
        private double _imageSizeToCropAreaSizeRatio = 1;
        private PixelsHistory _selectedCell;
        private BindableCollection<PixelsHistory> _historyCells;
        #endregion

        #region properties
        public string ErrorMessage { get; set; }
        public bool IsImagedLoaded => _originalImage != null;
        public double ScaleFactor { get; set; }
        public double ImageLeft { get; set; }
        public double ImageTop { get; set; }
        public double CropLeft { get; set; }
        public double CropTop { get; set; }
        public bool IsCropped { get; set; } = false;
        public int PixelCols { get; set; }
        public int PixelRows { get; set; }
        public Rect CropRect { get; set; }
        public Rect ContainerRect { get; set; }
        public bool HasError { get; set; }
        public bool HasImage { get; set; }

        public int Threshold
        {
            get => _threshold;
            set
            {
                _threshold = value;
                BuildCells(_grayBitmap, value);
            }
        }

        public BitmapImage OriginalImage
        {
            get => _originalImage;
            set
            {
                _originalImage = value;
                _imageHash = ImageProcessor.GetImageHash(value);
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

        public CellInfo[,] Cells
        {
            get => _cells;
            set { _cells = value; }
        }

        public PixelsHistory SelectedCell
        {
            get => _selectedCell;
            set { _selectedCell = value; }
        }

        public BindableCollection<PixelsHistory> HistoryCells
        {
            get => _historyCells;
            set { _historyCells = value; }
        }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 3)
            {
                PixelRows = int.Parse(args[2]);
                PixelCols = int.Parse(args[1]);
            }
            else
            {
                PixelRows = 21;
                PixelCols = 55;
            }
            _cells = new CellInfo[PixelRows, PixelCols];
            Threshold = 128;
            ScaleFactor = 1;
            ImageLeft = 0;
            ImageTop = 0;
        }
        #endregion

        #region PublicMethods

        public void SaveImage()
        {
            try
            {
                if (_cells == null || _cells.Length == 0)
                    return;
                var dialog = new InputProjectNameDialog();
                if (dialog.ShowDialog() != true)
                {
                    return;
                }
                string projectName = dialog.ProjectName;
                string dirName = "CellInfo";
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                string uuid = Guid.NewGuid().ToString();
                string fileName = Path.Combine(dirName, uuid + ".json");
                CellSerializer.SaveToFile(_cells, fileName);
                var pixelHistory = new PixelsHistory
                {
                    PictureHash = _imageHash,
                    DataFilePath = fileName,
                    ProjectName = projectName,
                };
                DbConnection.DbConnection.Db.Insertable(pixelHistory).ExecuteCommand();
                LoadHistoryCells();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void ImportImage(string path)
        {
            OriginalImage = ImageProcessor.ConvertTo96DpiBitmapImage(path, out bool success);
            if (!success)
            {
                return;
            }
            InitialImage();
            NotifyOfPropertyChange(nameof(IsImagedLoaded));
        }

        public void PasteCommand()
        {
            try
            {
                HasImage = false;
                if (Clipboard.ContainsImage())
                {
                    BitmapSource clipboardImage = Clipboard.GetImage();
                    if (clipboardImage != null)
                    {
                        OriginalImage = ImageProcessor.ConvertTo96DpiBitmapImage(
                            clipboardImage,
                            out bool success
                        );
                        if (!success)
                        {
                            return;
                        }
                        HasImage = true;
                    }
                }
                else if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText().Trim();
                    OriginalImage = ImageProcessor.ConvertTo96DpiBitmapImage(
                        clipboardText,
                        out bool success
                    );
                    if (!success)
                    {
                        return;
                    }
                    if (OriginalImage == null)
                    {
                        return;
                    }
                    HasImage = true;
                }
            }
            finally
            {
                if (!HasImage)
                {
                    HasError = true;
                    ErrorMessage = "只能粘贴JPG、PNG图片或者其所在的路径！";
                    NotifyOfPropertyChange(nameof(HasError));
                }
                else
                {
                    InitialImage();
                    NotifyOfPropertyChange(nameof(IsImagedLoaded));
                }
            }
        }

        public void RemoveHistoryCommand(PixelsHistory history)
        {
            try
            {
                File.Delete(history.DataFilePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            DbConnection.DbConnection.Db.Deleteable(history).ExecuteCommand();
            LoadHistoryCells();
        }

        public void DragOverHasErrorChangeCommand(bool hasError)
        {
            HasError = hasError;
            ErrorMessage = "只支持单张JPG或者PNG文件！";
        }

        public void ResizeCanvasCommand(System.Windows.Size size)
        {
            _canvasContainerWidth = size.Width;
            _canvasContainerHeight = size.Height;
            ResizeCropArea();
            ResizeImage();
        }

        public void ZoomCommand(MouseWheelEventArgs e)
        {
            if (IsCropped)
                return;
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            ScaleFactor *= zoomFactor;
            _imageSizeToCropAreaSizeRatio = OriginalImage.Width * ScaleFactor / (_cropAreaWidth);
            var oriPoint = e.GetPosition(View) - new Point(ImageLeft, ImageTop);
            var newPoint = new Point(oriPoint.X * zoomFactor, oriPoint.Y * zoomFactor);
            DragImage(new Point(oriPoint.X - newPoint.X, oriPoint.Y - newPoint.Y));
        }

        public void DragImage(System.Windows.Point offsetPoint)
        {
            if (IsCropped)
                return;
            ImageLeft += offsetPoint.X;
            ImageTop += offsetPoint.Y;
            _imageLeftToCropAreaLeftRatio = (ImageLeft - CropLeft) / _cropAreaWidth;
            _imageTopToCropAreaTopRatio = (ImageTop - CropTop) / _cropAreaHeight;
        }

        public void CropImage()
        {
            if (OriginalImage == null)
                return;

            if (IsCropped)
            {
                IsCropped = false;
                return;
            }
            InnerCropImage();
            IsCropped = true;
        }
        #endregion

        #region PrivateMethods
        private void LoadHistoryCells()
        {
            HistoryCells = new BindableCollection<PixelsHistory>();
            var history = DbConnection
                .DbConnection.Db.Queryable<PixelsHistory>()
                .Where(x => x.PictureHash == _imageHash)
                .OrderByDescending(x => x.DateTime)
                .ToList();
            foreach (var item in history)
            {
                HistoryCells.Add(item);
            }
        }

        private void InnerCropImage()
        {
            double startX = ImageLeft - CropLeft;
            double startY = ImageTop - CropTop;

            var croppedBitmap = ImageProcessor.CropImage(
                OriginalImage,
                startX,
                startY,
                _cropAreaWidth,
                _cropAreaHeight,
                ScaleFactor
            );
            var transformedPic = ImageProcessor.ResizeBitmapSource(
                croppedBitmap,
                PixelCols,
                PixelRows
            );
            ConvertToBinaryCells(transformedPic, Threshold);
        }

        private void ConvertToBinaryCells(BitmapSource bitmapSource, int threshold)
        {
            _grayBitmap = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray8, null, 0);
            BuildCells(_grayBitmap, threshold);
        }

        private void BuildCells(FormatConvertedBitmap grayBitmap, int threshold)
        {
            if (grayBitmap == null)
                return;
            int stride = (grayBitmap.PixelWidth * grayBitmap.Format.BitsPerPixel + 7) / 8;
            var pixelData = new byte[grayBitmap.PixelHeight * stride];
            grayBitmap.CopyPixels(pixelData, stride, 0);
            if (pixelData == null || pixelData.Length == 0)
                return;
            for (int i = 0; i < pixelData.Length; i++)
            {
                pixelData[i] = pixelData[i] > threshold ? (byte)255 : (byte)0; // 大于阈值为白色，小于等于为黑色
            }

            int w = grayBitmap.PixelWidth;
            int h = grayBitmap.PixelHeight;

            var newCells = new CellInfo[h, w];
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    newCells[row, col].IsBlue = pixelData[row * w + col] > threshold ? true : false;
                    newCells[row, col].Row = row;
                    newCells[row, col].Column = col;
                }
            }
            Cells = newCells;
        }

        private void ResizeImage()
        {
            if (OriginalImage == null)
                return;
            double imageX = OriginalImage.Width;
            double imageY = OriginalImage.Height;
            ScaleFactor = _imageSizeToCropAreaSizeRatio * _cropAreaWidth / OriginalImage.Width;
            ImageLeft = CropLeft + _imageLeftToCropAreaLeftRatio * _cropAreaWidth;
            ImageTop = CropTop + _imageTopToCropAreaTopRatio * _cropAreaHeight;
        }

        private void InitialImage()
        {
            if (OriginalImage == null)
                return;
            double imageX = OriginalImage.Width;
            double imageY = OriginalImage.Height;
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

            _imageSizeToCropAreaSizeRatio = OriginalImage.Width * ScaleFactor / (_cropAreaWidth);
            _imageLeftToCropAreaLeftRatio = (ImageLeft - CropLeft) / _cropAreaWidth;
            _imageTopToCropAreaTopRatio = (ImageTop - CropTop) / _cropAreaHeight;
            LoadHistoryCells();
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
