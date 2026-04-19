using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Contacts.View.Converters;

/// <summary>
/// Конвертер, преобразующий значение <see cref="bool"/> в <see cref="Visibility"/>.
/// true → Visible, false → Collapsed.
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Преобразует bool в Visibility.
    /// </summary>
    /// <param name="value">Значение типа bool.</param>
    /// <param name="targetType">Целевой тип (не используется).</param>
    /// <param name="parameter">Дополнительный параметр (не используется).</param>
    /// <param name="culture">Культура (не используется).</param>
    /// <returns>Visibility.Visible если true, иначе Visibility.Collapsed.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    /// <summary>
    /// Преобразует Visibility обратно в bool.
    /// </summary>
    /// <param name="value">Значение типа Visibility.</param>
    /// <param name="targetType">Целевой тип (не используется).</param>
    /// <param name="parameter">Дополнительный параметр (не используется).</param>
    /// <param name="culture">Культура (не используется).</param>
    /// <returns>true если Visible, иначе false.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}