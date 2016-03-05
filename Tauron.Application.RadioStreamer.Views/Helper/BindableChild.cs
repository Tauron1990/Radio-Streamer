using System.Windows;
using System.Windows.Controls;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public static class BindableChild
    {
        [NotNull, UsedImplicitly]
        public static UIElement GetBindableChild([NotNull] DependencyObject obj)
        {
            return (UIElement) obj.GetValue(BindableChildProperty);
        }

        [UsedImplicitly]
        public static void SetBindableChild([NotNull] DependencyObject obj, [NotNull] UIElement value)
        {
            obj.SetValue(BindableChildProperty, value);
        }

        public static readonly DependencyProperty BindableChildProperty =
            DependencyProperty.RegisterAttached("BindableChild", typeof(UIElement), typeof(BindableChild), new UIPropertyMetadata(null, BindableChildPropertyChanged));

        private static void BindableChildPropertyChanged([NotNull] DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var ele = sender as Decorator;
            if (ele != null && !ReferenceEquals(ele, e.NewValue))
            {
                ele.Child = (UIElement) e.NewValue;
            }
        }
    }
}
