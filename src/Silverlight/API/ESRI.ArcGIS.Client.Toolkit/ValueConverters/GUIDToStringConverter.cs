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
    /// *FOR INTERNAL USE ONLY* Value converter for FeatureDataGrid class.
    /// </summary>
    /// <remarks>Used to convert between string and guid type for FeatureDataGrid</remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class GUIDToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert from GUID to string
        /// </summary>
        /// <param name="value">GUID</param>
        /// <param name="targetType">string</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value is Guid || value is Guid?) && targetType == typeof(string))
            {
                return string.Format("{{{0}}}", value);
            }
            return value;
        }

        /// <summary>
        /// Convert from string to GUID.
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="targetType">GUID</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }       
    }
}
