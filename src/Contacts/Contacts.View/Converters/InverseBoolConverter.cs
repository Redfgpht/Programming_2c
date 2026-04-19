using System;
using System.Globalization;
using System.Windows.Data;

namespace Contacts.View.Converters;

/// <summary>
/// Конвертер, инвертирующий булево значение (true -> false, false -> true).
/// Используется в XAML для привязки к свойствам, где требуется обратная логика.
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBoolConverter : IValueConverter
{
    /// <summary>
    /// Преобразует исходное булево значение в противоположное.
    /// </summary>
    /// <param name="value">Исходное значение (ожидается bool).</param>
    /// <param name="targetType">Тип целевого свойства (не используется).</param>
    /// <param name="parameter">Дополнительный параметр (не используется).</param>
    /// <param name="culture">Культура (не используется).</param>
    /// <returns>Инвертированное булево значение или false, если входное значение не bool.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
            return !boolean;
        return false;
    }

    /// <summary>
    /// Преобразует инвертированное значение обратно в исходное (для двусторонней привязки).
    /// </summary>
    /// <param name="value">Инвертированное значение (ожидается bool).</param>
    /// <param name="targetType">Тип исходного свойства (не используется).</param>
    /// <param name="parameter">Дополнительный параметр (не используется).</param>
    /// <param name="culture">Культура (не используется).</param>
    /// <returns>Исходное булево значение или false, если входное значение не bool.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
            return !boolean;
        return false;
    }
}