using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Pic2PixelStylet.Behaviors
{
    internal class SliderMouseWheelBehavior : Behavior<Slider>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;
        }

        private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                AssociatedObject.Value += AssociatedObject.TickFrequency;
            }
            else
            {
                AssociatedObject.Value -= AssociatedObject.TickFrequency;
            }
        }
    }
}
