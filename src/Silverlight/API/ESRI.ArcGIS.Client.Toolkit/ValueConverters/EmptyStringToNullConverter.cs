using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ESRI.ArcGIS.Client.Toolkit.ValueConverters
{
    /// <summary>
    /// *FOR INTERNAL USE ONLY* The FeatureDataGrid class.
    /// </summary>
    /// <remarks>Used by property setters of the fields having range domain information.</remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class EmptyStringToNullConverter : IValueConverter
    {
        /// <summary>
        /// Change empty strings into null values so they can be set to null-able values.
        /// </summary>
        /// <param name="value">the proposed value to convert.</param>
        /// <param name="targetType">they type to convert to.</param>
        /// <param name="parameter">binding parameters to evaluate.</param>
        /// <param name="culture">culture of the application.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EmptyStringToNullConverter.convert(value, targetType, parameter, culture);
        }

        /// <summary>
        /// Change null value to an empty string for display in textbox.
        /// </summary>
        /// <param name="value">the proposed value to convert.</param>
        /// <param name="targetType">they type to convert to.</param>
        /// <param name="parameter">binding parameters to evaluate.</param>
        /// <param name="culture">culture of the application.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EmptyStringToNullConverter.convert(value, targetType, parameter, culture);
        }

        private static object convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string && targetType != typeof(string) && string.IsNullOrEmpty((string)value))
                return null;
            if (value is Nullable && targetType == typeof(string))
                return string.Empty;
            return value;
        }
    }
}
