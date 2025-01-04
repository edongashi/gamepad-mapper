using System.Windows;
using System.Windows.Controls;
using GamepadMapper.Configuration;
using GamepadMapper.Menus;

namespace GamepadMapper.Wpf
{
    public class RadialMenu : Control
    {
        public static readonly DependencyProperty ArcWidthProperty =
            DependencyProperty.RegisterAttached("ArcWidth",
                typeof(double),
                typeof(RadialMenu),
                new PropertyMetadata(0d));

        public static double GetArcWidth(DependencyObject d)
        {
            return (double)d.GetValue(ArcWidthProperty);
        }

        public static void SetArcWidth(DependencyObject d, double value)
        {
            d.SetValue(ArcWidthProperty, value);
        }

        public static readonly DependencyProperty MaxArcWidthProperty =
            DependencyProperty.RegisterAttached("MaxArcWidth",
                typeof(double),
                typeof(RadialMenu),
                new PropertyMetadata(180d));

        public static double GetMaxArcWidth(DependencyObject d)
        {
            return (double)d.GetValue(ArcWidthProperty);
        }

        public static void SetMaxArcWidth(DependencyObject d, double value)
        {
            d.SetValue(ArcWidthProperty, value);
        }

        static RadialMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialMenu), new FrameworkPropertyMetadata(typeof(RadialMenu)));
        }

        public static readonly DependencyProperty MenuPositionProperty =
            DependencyProperty.Register("MenuPosition", 
                typeof(MenuPosition), 
                typeof(RadialMenu), 
                new PropertyMetadata(MenuPosition.BottomRight));

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", 
                typeof(MenuPage), 
                typeof(RadialMenu), 
                new PropertyMetadata(null));

        public static readonly DependencyProperty ItemFocusedProperty =
            DependencyProperty.Register("ItemFocused",
                typeof(bool), 
                typeof(RadialMenu),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsPointerVisibleProperty =
            DependencyProperty.Register("IsPointerVisible",
                typeof(bool),
                typeof(RadialMenu),
                new PropertyMetadata(false));

        public static readonly DependencyProperty PointerWidthProperty =
            DependencyProperty.Register("PointerWidth",
                typeof(double),
                typeof(RadialMenu),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen",
                typeof(bool),
                typeof(RadialMenu),
                new PropertyMetadata(false));

        public static readonly DependencyProperty PointerAngleProperty =
            DependencyProperty.Register("PointerAngle",
                typeof(double),
                typeof(RadialMenu),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty HelpScreenProperty =
            DependencyProperty.Register("HelpScreen",
                typeof(HelpConfiguration),
                typeof(RadialMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HelpScreen2Property =
            DependencyProperty.Register("HelpScreen2",
                typeof(HelpConfiguration),
                typeof(RadialMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem",
                typeof(PageItem),
                typeof(RadialMenu),
                new PropertyMetadata(null));

        public MenuPosition MenuPosition
        {
            get => (MenuPosition)GetValue(MenuPositionProperty);
            set => SetValue(MenuPositionProperty, value);
        }

        public MenuPage CurrentPage
        {
            get => (MenuPage)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public PageItem CurrentItem
        {
            get => (PageItem)GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        public bool ItemFocused
        {
            get => (bool)GetValue(ItemFocusedProperty);
            set => SetValue(ItemFocusedProperty, value);
        }

        public HelpConfiguration HelpScreen
        {
            get => (HelpConfiguration)GetValue(HelpScreenProperty);
            set => SetValue(HelpScreenProperty, value);
        }

        public HelpConfiguration HelpScreen2
        {
            get => (HelpConfiguration)GetValue(HelpScreen2Property);
            set => SetValue(HelpScreen2Property, value);
        }

        public bool IsPointerVisible
        {
            get => (bool)GetValue(IsPointerVisibleProperty);
            set => SetValue(IsPointerVisibleProperty, value);
        }

        public double PointerAngle
        {
            get => (double)GetValue(PointerAngleProperty);
            set => SetValue(PointerAngleProperty, value);
        }

        public double PointerWidth
        {
            get => (double)GetValue(PointerWidthProperty);
            set => SetValue(PointerWidthProperty, value);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
    }
}
