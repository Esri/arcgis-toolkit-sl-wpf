// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using ESRI.ArcGIS.Client.Symbols;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Gps
{
	/// <summary>
	/// *FOR INTERNAL USE ONLY* GPS Symbol used as default symbology by the <see cref="GpsLayer"/>.
	/// </summary>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class GpsSymbol : MarkerSymbol
	{
		internal GpsSymbol()
		{
			ControlTemplate = ResourceData.Dictionary["GpsSymbolTemplate"] as ControlTemplate;
#if WINDOWS_PHONE
			Size = 30;
#else
			Size = 20;
#endif
		}

		private double _Size;

		/// <summary>
		/// [INTERNAL USE ONLY] Gets or sets the symbol size.
		/// </summary>
		public double Size
		{
			get { return _Size; }
			set
			{
				if (_Size != value)
				{
					_Size = value;
					OffsetX = OffsetY = value * .5;
					base.OnPropertyChanged("Size");
				}
			}
		}

		private double course;

		/// <summary>
		/// Gets or sets the heading/course.
		/// </summary>
		public double Course
		{
			get
			{
				return course;
			}
			set
			{
				if (value != course)
				{
					course = value;
					base.OnPropertyChanged("Course"); //Triggers rebinding
				}
			}
		}

		private double speed;

		/// <summary>
		/// [INTERNAL USE ONLY] Gets or sets the speed.
		/// </summary>
		public double Speed
		{
			get
			{
				return speed;
			}
			set
			{
				if (value != speed)
				{
					speed = value;
					base.OnPropertyChanged("Speed"); //Triggers rebinding
				}
			}
		}
#if WINDOWS_PHONE
		/// <summary>
		/// [INTERNAL USE ONLY] Identifies the Angle attached dependency property.
		/// </summary>
		/// <remarks>
		/// Silverlight 3 doesn't support binding into transforms, so we use this
		/// surrogate binding as described here:
		/// http://www.sharpgis.net/post/2009/05/04/Using-surrogate-binders-in-Silverlight.aspx
		/// </remarks>
		public static readonly DependencyProperty AngleProperty =
			DependencyProperty.RegisterAttached("Angle", typeof(double),
			typeof(GpsSymbol),
			new PropertyMetadata(OnAngleChanged));

		/// <summary>
		/// Gets the angle.
		/// </summary>
		/// <param name="d">The dependency object.</param>
		/// <returns>Angle value</returns>
		public static double GetAngle(DependencyObject d)
		{
			return (double)d.GetValue(AngleProperty);
		}

		/// <summary>
		/// Sets the angle.
		/// </summary>
		/// <param name="d">The dependency object.</param>
		/// <param name="value">The angle.</param>
		public static void SetAngle(DependencyObject d, double value)
		{
			d.SetValue(AngleProperty, value);
		}

		private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is UIElement)
			{
				if (e.NewValue is double)
				{
					double c = (double)e.NewValue;
					if (!double.IsNaN(c))
					{
						UIElement b = d as UIElement;
						if (b.RenderTransform is RotateTransform)
							(b.RenderTransform as RotateTransform).Angle = c;
						else
							b.RenderTransform = new RotateTransform() { Angle = c };
					}
				}
			}
		}
#endif
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* Converter used to turn off the direction marker when the speed drops below a certain point
	/// </summary>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class VisibleWhenAboveConverter : System.Windows.Data.IValueConverter
	{
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
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double && parameter != null)
			{
				var val = (double)value;
				if (double.IsNaN(val)) return Visibility.Collapsed;
				var param = double.Parse(parameter.ToString(), System.Globalization.CultureInfo.InvariantCulture);
				return val < param ? Visibility.Collapsed : Visibility.Visible;
			}
			return value;
		}

		/// <summary>
		/// Not Implemented
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		/// <exception cref="System.NotImplementedException" />
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}