using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Pic2PixelStylet.Pages
{
    public struct CellInfo
    {
        public bool IsBlue { get; set; }

        [JsonIgnore]
        public Border CellBorder;

        public int Row { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return $"{Row + 1}行，{Column + 1}列";
        }
    }

    /// <summary>
    /// PixelGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PixelGrid : UserControl
    {
        public PixelGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InverseColorProperty =
            DependencyProperty.Register(
                nameof(InverseColor),
                typeof(bool),
                typeof(PixelGrid),
                new PropertyMetadata(false, OnInverseColorChanged)
            );

        public bool InverseColor
        {
            get { return (bool)GetValue(InverseColorProperty); }
            set { SetValue(InverseColorProperty, value); }
        }

        public static readonly DependencyProperty CellsProperty = DependencyProperty.Register(
            nameof(Cells),
            typeof(CellInfo[,]),
            typeof(PixelGrid),
            new PropertyMetadata(null, OnCellsChanged)
        );

        public CellInfo[,] Cells
        {
            get { return (CellInfo[,])GetValue(CellsProperty); }
            set { SetValue(CellsProperty, value); }
        }

        private static void OnCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PixelGrid pixelGrid)
                return;
            if (
                pixelGrid.Cells is null
                || pixelGrid.Cells.GetLength(0) == 0
                || pixelGrid.Cells.GetLength(1) == 0
            )
                return;

            int height = pixelGrid.Cells.GetLength(0);
            int width = pixelGrid.Cells.GetLength(1);
            pixelGrid.CreateUniformGrids(width, height, pixelGrid.InverseColor);
        }

        private static void OnInverseColorChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is not PixelGrid pixelGrid)
                return;
            if (
                pixelGrid.Cells is null
                || pixelGrid.Cells.GetLength(0) == 0
                || pixelGrid.Cells.GetLength(1) == 0
            )
                return;

            int height = pixelGrid.Cells.GetLength(0);
            int width = pixelGrid.Cells.GetLength(1);
            pixelGrid.CreateUniformGrids(width, height, pixelGrid.InverseColor);
        }

        private void CreateUniformGrids(int width, int height, bool isInverse)
        {
            ClearUniformGrids(width, height);
            CreatePixelGridInner(isInverse, width, height);
            CreateTopBottomUniformGrid(width);
            CreateLeftRightUniformGrid(height);
        }

        private void CreateLeftRightUniformGrid(int height)
        {
            for (int row = 0; row < height; row++)
            {
                var realRow = row + 1;
                if (realRow % 5 == 0)
                {
                    LeftUniformGrid.Children.Add(CreateTextBlock(realRow, InverseColor));
                    RightUniformGrid.Children.Add(CreateTextBlock(realRow, InverseColor));
                }
                else
                {
                    LeftUniformGrid.Children.Add(CreateTransparentBorder());
                    RightUniformGrid.Children.Add(CreateTransparentBorder());
                }
            }
        }

        private void CreateTopBottomUniformGrid(int width)
        {
            for (int col = 0; col < width; col++)
            {
                var realCol = col + 1;
                if (realCol % 5 == 0)
                {
                    TopUniformGrid.Children.Add(CreateTextBlock(realCol, InverseColor));
                    BottomUniformGrid.Children.Add(CreateTextBlock(realCol, InverseColor));
                }
                else
                {
                    TopUniformGrid.Children.Add(CreateTransparentBorder());
                    BottomUniformGrid.Children.Add(CreateTransparentBorder());
                }
            }
        }

        private static Border CreateTransparentBorder()
        {
            return new Border
            {
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
            };
        }

        private static TextBlock CreateTextBlock(int num, bool isInverse)
        {
            return new TextBlock
            {
                Text = num.ToString(),
                Foreground = isInverse ? Brushes.Blue : Brushes.White,
                Background = isInverse ? Brushes.White : Brushes.Blue,
            };
        }

        private void CreatePixelGridInner(bool isInverse, int width, int height)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Border border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Background = GetColor(Cells[row, col], isInverse),
                        Tag = Cells[row, col],
                    };
                    Cells[row, col].CellBorder = border;
                    border.MouseLeftButtonDown += Pixel_Click;
                    border.MouseEnter += Pixel_MouseEnter;
                    PixelUniformGrid.Children.Add(border);
                }
            }
        }

        private void ClearUniformGrids(int width, int height)
        {
            PixelUniformGrid.Children.Clear();
            PixelUniformGrid.Rows = height;
            PixelUniformGrid.Columns = width;

            TopUniformGrid.Children.Clear();
            TopUniformGrid.Rows = 1;
            TopUniformGrid.Columns = width;

            RightUniformGrid.Children.Clear();
            RightUniformGrid.Rows = height;
            RightUniformGrid.Columns = 1;

            BottomUniformGrid.Children.Clear();
            BottomUniformGrid.Rows = 1;
            BottomUniformGrid.Columns = width;

            LeftUniformGrid.Children.Clear();
            LeftUniformGrid.Rows = height;
            LeftUniformGrid.Columns = 1;
        }

        private SolidColorBrush GetColor(CellInfo cell, bool isInverse)
        {
            if (isInverse)
            {
                return cell.IsBlue ? Brushes.White : Brushes.Blue;
            }
            else
            {
                return cell.IsBlue ? Brushes.Blue : Brushes.White;
            }
        }

        private void Pixel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.Tag is CellInfo cell)
            {
                border.ToolTip = border.Tag.ToString();
            }
        }

        private void Pixel_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CellInfo cell)
            {
                if (border.Background == Brushes.Blue)
                {
                    border.Background = Brushes.White;
                    Cells[cell.Row, cell.Column].IsBlue = false;
                }
                else
                {
                    border.Background = Brushes.Blue;
                    Cells[cell.Row, cell.Column].IsBlue = true;
                }
            }
        }
    }
}
