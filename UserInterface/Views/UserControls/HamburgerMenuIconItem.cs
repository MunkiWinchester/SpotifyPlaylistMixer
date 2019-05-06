using System.Windows;
using MahApps.Metro.Controls;

namespace UserInterface.Views.UserControls
{
    public class CustomHamburgerMenuIconItem : HamburgerMenuIconItem
    {
        public new static readonly DependencyProperty ToolTipProperty
            = DependencyProperty.Register(nameof(ToolTip),
                typeof(object),
                typeof(CustomHamburgerMenuIconItem),
                new PropertyMetadata(null));

        public new object ToolTip
        {
            get => GetValue(ToolTipProperty);
            set => SetValue(ToolTipProperty, value);
        }
    }
}