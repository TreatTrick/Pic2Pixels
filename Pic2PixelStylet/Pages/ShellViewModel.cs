using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Pic2PixelStylet.Utils;
using Stylet;

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
        private int _gridWidth;
        private int _gridHeight;
        private CellInfo[,] _cells;
        private string _testText;
        private BitmapImage _origianlImage;
        private string _originalImagePath;
        private int _pixelCols;
        private int _pixelRows;
        #endregion

        #region properties
        public string TestText
        {
            get => _testText;
            set { _testText = value; }
        }

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
                _originalImagePath = value;
                OnOriginalImagePathChanged(value);
            }
        }

        public bool IsImagedLoaded => _origianlImage != null;
        public double ScaleFactor { get; set; }
        public double ImageLeft { get; set; }
        public double ImageTop { get; set; }
        public double CropLeft { get; set; }
        public double CropTop { get; set; }
        public double CanvasContainerWidth { get; set; }
        public double CanvasContainerHeight { get; set; }
        public double CropAreaWidth { get; set; }
        public double CropAreaHeight { get; set; }
        public string PixelCols
        {
            get => _pixelCols.ToString();
            set
            {
                if (int.TryParse(value, out int result))
                    _pixelCols = result;
            }
        }
        public string PixelRows
        {
            get => _pixelRows.ToString();
            set
            {
                if (int.TryParse(value, out int result))
                    _pixelRows = result;
            }
        }
        public int Threshhold { get; set; }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            _gridWidth = 51;
            _gridHeight = 20;
            _testText = "100";
            _cells = new CellInfo[_gridWidth, _gridHeight];
        }
        #endregion

        #region PublicMethods

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
            NotifyOfPropertyChange(nameof(IsImagedLoaded));
        }
        #endregion
    }
}
