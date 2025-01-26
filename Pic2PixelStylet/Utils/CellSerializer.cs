using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using Pic2PixelStylet.Pages;

namespace Pic2PixelStylet.Utils
{
    public class CellsWrapper
    {
        public CellsWrapper(
            CellInfo[,] grid,
            double imageSizeToCropAreaSizeRatio,
            double imageLeftToCropAreaLeftRatio,
            double imageTopToCropAreaTopRatio
        )
        {
            Rows = grid.GetLength(0);
            Columns = grid.GetLength(1);
            Cells = new List<CellInfo>();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var cell = grid[i, j];
                    cell.Row = i; // 显式设置索引
                    cell.Column = j;
                    Cells.Add(cell);
                }
            }
            ImageSizeToCropAreaSizeRatio = imageSizeToCropAreaSizeRatio;
            ImageLeftToCropAreaLeftRatio = imageLeftToCropAreaLeftRatio;
            ImageTopToCropAreaTopRatio = imageTopToCropAreaTopRatio;
        }

        [JsonConstructor]
        private CellsWrapper()
        {
            Rows = 0;
            Columns = 0;
            Cells = new List<CellInfo>();
            ImageSizeToCropAreaSizeRatio = 1;
            ImageLeftToCropAreaLeftRatio = 0;
            ImageTopToCropAreaTopRatio = 0;
        }

        public CellInfo[,] GetCellInfos()
        {
            var grid = new CellInfo[Rows, Columns];

            foreach (var cell in Cells)
            {
                if (cell.Row < Rows && cell.Column < Columns)
                {
                    grid[cell.Row, cell.Column] = cell;
                }
            }
            return grid;
        }

        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<CellInfo> Cells { get; set; }
        public double ImageSizeToCropAreaSizeRatio { get; set; }
        public double ImageLeftToCropAreaLeftRatio { get; set; }
        public double ImageTopToCropAreaTopRatio { get; set; }
    }

    public static class CellSerializer
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static void SaveToFile(CellsWrapper wrapper, string filePath)
        {
            string json = JsonSerializer.Serialize(wrapper, Options);
            File.WriteAllText(filePath, json);
        }

        public static CellsWrapper LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var wrapper = JsonSerializer.Deserialize<CellsWrapper>(json, Options);
            return wrapper;
        }
    }
}
