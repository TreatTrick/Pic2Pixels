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
    /// QuestionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class QuestionDialog : Window
    {
        public QuestionDialog()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowContentProperty = DependencyProperty.Register(
            nameof(ShowContent),
            typeof(string),
            typeof(QuestionDialog),
            new PropertyMetadata(string.Empty, OnShowContentChanged)
        );

        public string ShowContent
        {
            get { return (string)GetValue(ShowContentProperty); }
            set { SetValue(ShowContentProperty, value); }
        }

        private static void OnShowContentChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var dialog = d as QuestionDialog;
            if (dialog == null)
            {
                return;
            }
            dialog.ContentTextBlock.Text = dialog.ShowContent;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
