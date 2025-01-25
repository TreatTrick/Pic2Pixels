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
using System.Windows.Shapes;

namespace Pic2PixelStylet.Pages
{
    /// <summary>
    /// InputProjectNameDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputProjectNameDialog : Window
    {
        public string ProjectName { get; private set; }

        public InputProjectNameDialog()
        {
            InitializeComponent();
            ProjectNameTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(ProjectNameTextBox.Text.Trim()))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            ProjectName = ProjectNameTextBox.Text.Trim();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ProjectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectNameTextBox.Text.Trim()))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                OkButton.IsEnabled = false;
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;
                OkButton.IsEnabled = true;
            }
        }
    }
}
