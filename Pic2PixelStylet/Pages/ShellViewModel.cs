using Stylet;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

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
        #endregion

        #region properties
        public string TestText
        {
            get => _testText;
            set
            {
                _testText = value;
            }
        }
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

        #region Methods
        public void TEST()
        {
            TestText = "TEST";
        }
        #endregion      
    }
}
