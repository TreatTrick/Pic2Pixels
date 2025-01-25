using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Pic2PixelStylet.DbConnection
{
    public class PixelsHistory
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public string ProjectName { get; set; }

        [SugarColumn(IsNullable = false)]
        public string PictureHash { get; set; }

        [SugarColumn(IsNullable = false)]
        public string DataFilePath { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
