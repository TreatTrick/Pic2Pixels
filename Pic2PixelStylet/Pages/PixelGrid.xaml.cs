using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pic2PixelStylet.Pages
{
    public struct CellInfo
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
            pixelGrid.CreatePixelGrid(width, height, pixelGrid.InverseColor);
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
            pixelGrid.CreatePixelGrid(width, height, pixelGrid.InverseColor);
        }

        private void CreatePixelGrid(int width, int height, bool isInverse)
        {
            PixelUniformGrid.Children.Clear();
            PixelUniformGrid.Rows = height;
            PixelUniformGrid.Columns = width;
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
