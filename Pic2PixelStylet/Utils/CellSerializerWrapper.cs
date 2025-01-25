using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pic2PixelStylet.Pages;

namespace Pic2PixelStylet.Utils
{
    public class CellGridWrapper
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<CellInfo> Cells { get; set; }
    }

    public static class CellSerializer
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static void SaveToFile(CellInfo[,] grid, string filePath)
        {
            var wrapper = new CellGridWrapper
            {
                Rows = grid.GetLength(0),
                Columns = grid.GetLength(1),
                Cells = new List<CellInfo>(),
            };

            for (int i = 0; i < wrapper.Rows; i++)
            {
                for (int j = 0; j < wrapper.Columns; j++)
                {
                    var cell = grid[i, j];
                    cell.Row = i; // 显式设置索引
                    cell.Column = j;
                    wrapper.Cells.Add(cell);
                }
            }
            string json = JsonSerializer.Serialize(wrapper, Options);
            File.WriteAllText(filePath, json);
        }

        public static CellInfo[,] LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var wrapper = JsonSerializer.Deserialize<CellGridWrapper>(json, Options);
            var grid = new CellInfo[wrapper.Rows, wrapper.Columns];

            foreach (var cell in wrapper.Cells)
            {
                if (cell.Row < wrapper.Rows && cell.Column < wrapper.Columns)
                {
                    grid[cell.Row, cell.Column] = cell;
                }
            }

            return grid;
        }
    }
}
