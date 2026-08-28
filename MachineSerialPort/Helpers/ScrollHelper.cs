using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace MachineSerialPort.Helpers
{
    public static class ScrollHelper
    {
        public static readonly DependencyProperty ScrollToItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollToItem",
                typeof(object),
                typeof(ScrollHelper),
                new PropertyMetadata(null, OnScrollToItemChanged));

        public static object GetScrollToItem(DependencyObject obj)
        {
            return obj.GetValue(ScrollToItemProperty);
        }

        public static void SetScrollToItem(DependencyObject obj, object value)
        {
            obj.SetValue(ScrollToItemProperty, value);
        }

        private static void OnScrollToItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid && e.NewValue != null)
            {
                // Call the ScrollIntoView method on the DataGrid, passing the new item.
                dataGrid.ScrollIntoView(e.NewValue);
            }
        }
    }
}
