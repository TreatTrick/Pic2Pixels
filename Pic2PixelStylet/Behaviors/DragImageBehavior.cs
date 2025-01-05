using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Pic2PixelStylet.Behaviors
{
    class DragImageBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty DragImageCommanProperty =
            DependencyProperty.Register(
                nameof(DragImageCommand),
                typeof(ICommand),
                typeof(DragImageBehavior),
                new PropertyMetadata(null)
            );

        public ICommand DragImageCommand
        {
            get { return (ICommand)GetValue(DragImageCommanProperty); }
            set { SetValue(DragImageCommanProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        }

        private void AssociatedObject_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            var DraggedImage = sender as UIElement;
            _dragStartPoint = e.GetPosition(DraggedImage);
            _isDragging = true;
            DraggedImage.CaptureMouse();
        }

        private void AssociatedObject_MouseMove(
            object sender,
            System.Windows.Input.MouseEventArgs e
        )
        {
            if (_isDragging)
            {
                var DraggedImage = sender as UIElement;
                var currentPosition = e.GetPosition(DraggedImage);
                double offsetX = currentPosition.X - _dragStartPoint.X;
                double offsetY = currentPosition.Y - _dragStartPoint.Y;
                DragImageCommand.Execute(new Point(offsetX, offsetY));
                _dragStartPoint = currentPosition;
            }
        }

        private void AssociatedObject_MouseLeftButtonUp(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            var DraggedImage = sender as UIElement;
            _isDragging = false;
            DraggedImage.ReleaseMouseCapture();
        }

        private System.Windows.Point _dragStartPoint;
        private bool _isDragging;
    }
}
