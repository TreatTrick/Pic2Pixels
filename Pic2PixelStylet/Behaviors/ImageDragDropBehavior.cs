using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Pic2PixelStylet.Behaviors
{
    internal class ImageDragDropBehavior : Behavior<FrameworkElement>
    {
        static DependencyProperty IsSuccessProperty = DependencyProperty.Register(
            nameof(IsSuccess),
            typeof(bool),
            typeof(ImageDragDropBehavior),
            new PropertyMetadata(false)
        );

        public bool IsSuccess
        {
            get { return (bool)GetValue(IsSuccessProperty); }
            set { SetValue(IsSuccessProperty, value); }
        }

        static DependencyProperty ImportImageCommandProperty = DependencyProperty.Register(
            nameof(ImportImageCommand),
            typeof(ICommand),
            typeof(ImageDragDropBehavior),
            new PropertyMetadata(null)
        );

        public ICommand ImportImageCommand
        {
            get { return (ICommand)GetValue(ImportImageCommandProperty); }
            set { SetValue(ImportImageCommandProperty, value); }
        }

        static DependencyProperty HasErrorChangeCommandProperty = DependencyProperty.Register(
            nameof(HasErrorChangeCommand),
            typeof(ICommand),
            typeof(ImageDragDropBehavior),
            new PropertyMetadata(null)
        );

        public ICommand HasErrorChangeCommand
        {
            get { return (ICommand)GetValue(HasErrorChangeCommandProperty); }
            set { SetValue(HasErrorChangeCommandProperty, value); }
        }

        static DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            nameof(ErrorMessage),
            typeof(string),
            typeof(ImageDragDropBehavior),
            new PropertyMetadata("只支持单张JPG或者PNG文件！")
        );

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.Drop += AssociatedObject_Drop;
            this.AssociatedObject.DragOver += AssociatedObject_DragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Drop -= AssociatedObject_Drop;
            this.AssociatedObject.DragOver -= AssociatedObject_DragOver;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (!IsDraggedFileValid(e))
                return;
            var imagePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            ImportImageCommand?.Execute(imagePath);
            IsSuccess = true;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            IsSuccess = false;
            if (!IsDraggedFileValid(e))
            {
                HasErrorChangeCommand?.Execute(true);
                e.Effects = DragDropEffects.None;
                return;
            }
            HasErrorChangeCommand?.Execute(false);
            e.Effects = DragDropEffects.Copy; // 允许拖动
        }

        private bool IsDraggedFileValid(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length != 1)
            {
                return false;
            }

            string fileExtension = System.IO.Path.GetExtension(files[0]).ToLower();
            Debug.WriteLine("fileExtension: " + fileExtension);

            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
            {
                return false;
            }
            return true;
        }
    }
}
