using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatApp.Converter;

public class BoolToVisibilityConverter<T> : IValueConverter
{
    private T True { get; }

    private T False { get; }

    public BoolToVisibilityConverter(T trueValue, T falseValue)
    {
        True = trueValue;
        False = falseValue;
    }

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool && (bool)value ? True : False;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
    }
}

public class BoolToVisibilityConverter : BoolToVisibilityConverter<Visibility>
{
    public BoolToVisibilityConverter()
        : base(Visibility.Visible, Visibility.Collapsed)
    {
    }
}