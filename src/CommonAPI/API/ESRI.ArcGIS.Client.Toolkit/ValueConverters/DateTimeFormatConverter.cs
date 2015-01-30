using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace ESRI.ArcGIS.Client.Toolkit.ValueConverters
{
	/// <summary>
 /// *FOR INTERNAL USE ONLY* DateTimeFormat converter is used as a DateTime.ToString() formatter. A DateTimeFormat
	/// and a DateTimeKind that will be used in the DateTime.ToString(). All outputs are presented
	/// in Culture defined by System.Globalization.CultureInfo.CurrentCulture of the executing application.
	/// </summary>
 /// <exclude/>
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DateTimeFormatConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the date time format used to format the DateTime string result.
		/// </summary>		
		public string DateTimeFormat { get; set; }
		/// <summary>
		/// Gets or sets the DateTimeKind that will be used to format the DateTime string result.
		/// </summary>		
		public DateTimeKind DateTimeKind { get; set; }

		#region IValueConverter Members

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is DateTime)
				return DateTimeToString((DateTime)value, DateTimeKind, DateTimeFormat, culture);
			return "";
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// Gets the ToString() result of a DateTime object. 
		/// </summary>
		/// <param name="date">Date to call ToString() method on.</param>
		/// <param name="kind">DateTimeKind to present the string result in.</param>
		/// <param name="dateTimeFormat">String format used to format the ToString() method with.</param>
		/// <param name="culture">The culture</param>
		/// <returns></returns>
		public static string DateTimeToString(DateTime? date, DateTimeKind kind, string dateTimeFormat, CultureInfo culture)
		{
			DateTime dt = new DateTime(date.Value.Ticks, date.Value.Kind);
			if (dt.Kind != System.DateTimeKind.Unspecified)
			{
				switch (kind)
				{
					case System.DateTimeKind.Local:
						dt = dt.ToLocalTime();
						break;
					case System.DateTimeKind.Utc:
						dt = dt.ToUniversalTime();
						break;
				}
			}
			return dt.ToString(dateTimeFormat, culture);
		}
	}
}
