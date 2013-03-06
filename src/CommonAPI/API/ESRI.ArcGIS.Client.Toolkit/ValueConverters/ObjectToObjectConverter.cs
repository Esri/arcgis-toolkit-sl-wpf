// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ESRI.ArcGIS.Client.Toolkit.ValueConverters
{
	/// <summary>
	/// *FOR INTERNAL USE ONLY* Convert an object to an object defined in a dictionary (the key being the string representation of the value).
	/// <remarks>
	/// If the key doesn't exist un the dictionary, the converter returns the string representation of the value.
	/// </remarks>
	/// </summary>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[ContentProperty("ResourceDictionary")]
	public sealed class ObjectToObjectConverter : IValueConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectToObjectConverter"/> class.
		/// </summary>
		public ObjectToObjectConverter()
		{
			ResourceDictionary = new ResourceDictionary();
		}

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
			string key = value.ToString();
			return ResourceDictionary.Contains(key) ? ResourceDictionary[key] : key;
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
			throw new NotSupportedException();
		}

		/// <summary>
		/// The resource dictionary used for the conversion.
		/// <remarks>
		/// This is the content property of the class.
		/// </remarks>
		/// </summary>
		/// <value>The resource dictionary.</value>
		public ResourceDictionary ResourceDictionary { get; set; }
	}
}
