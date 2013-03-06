// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client.Geometry;
using System.Linq;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>The ScaleLine Control generates a line representing a certain distance on the map in both Metric and US units.
	/// </summary>
	/// <remarks>
	/// 	<para>
	/// 		<strong>
	/// 			<u>Setting the MapUnit Property:</u>
	/// 		</strong>
	/// 	</para>
	/// 	<para>
	/// 	For the ScaleLine to function correctly, it is possible to set the <see cref="MapUnit"></see> to 
	/// 	whatever unit the Map's <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> is using. 
	/// 	For example: if the Map's SpatialReference is based on a Geographic coordinate system use DecimalDegrees 
	/// 	(aka. Longitude/Latitude) units; if it is a UTM or WebMercator (SRID=102100) projection use Meters.
	/// 	</para>
	/// 	<para>
	/// 	When the Map is using Geographic units (ie. DecimalDegrees) or WebMercator projection, the approximate scale will be calculated at the 
	/// 	center of the map. If any other units are used, a direct conversion between MapUnit's and Metric/US units 
	/// 	is used and scale distortion is not taken into account.
	/// 	</para>
	/// 	<para>
	/// 		<strong>
	/// 			<u>Default MapUnit:</u>
	/// 		</strong>
	/// 	</para>
	/// 	<para>
	/// 	If the <see cref="MapUnit"></see> is  not set manually, the scale line control will use a default map unit which is calculated from the spatial reference of the map
	/// 	or from the <see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer.Units"></see> of the layers inside the map.
	///		If the spatial reference is Geographic WGS84 (SRID=4326) or WebMercator (SRID=102100), the default MapUnit will be DecimalDegrees and Meters respectively.
	///		For others spatial references, the scale line will look at the Units property of the layers having the same spatial reference than the map.
	///		If no layers are found, the default DecimalDegrees will be used.
	/// 	</para>
	///     <para>
	/// 		<strong>
	/// 			<u>Controling the text on the ScaleLine:</u>
	/// 		</strong>
	///     </para>
	///     <para>
	///     It is not possibly to directly control the values of the text on the ScaleLine as a Property that can be set. The 
	///     text as part of the ScaleLine that displays is automatically adjusted as the scale of the map changes when the 
	///     ScaleLine is bound to a Map Control. This also means that it is not possible to control the scale (ie. zoom level) 
	///     of the Map Control via a Property of the ScaleLine Control.
	///     </para>
	///     <para>
	/// 		<strong>
	/// 			<u>ScaleBar style</u>
	/// 		</strong>
	///     </para>
	///     <para>
	///     The ScaleLine control is replacing the ScaleBar control which is now deprecated.
	///     Nevertheless it's possible to template the ScaleLine control in order to get a look close of the old ScaleBar.
	///     Below there is a sample of a scaleline style allowing to get a scale bar with metric units.
	///     </para>
	/// </remarks>
	/// <example>
	/// 	<code title="Example XAML1" description="" lang="XAML">
	/// 	&lt;esri:ScaleLine Name="ScaleLine1" Map="{Binding ElementName=Map1}"
	///       HorizontalAlignment="Left" VerticalAlignment="Top"  
	///       TargetWidth="400" 
	///       Foreground="Black" FontFamily="Courier New" FontSize="18" /&gt;
	///     </code>
	///     
	/// 	<code title="Example CS1" description="" lang="CS">
	///     //Create a new ScaleLine Control and add it to the LayoutRoot (a Grid in the XAML)
	///     ESRI.ArcGIS.Client.Toolkit.ScaleLine ScaleLine1 = new ESRI.ArcGIS.Client.Toolkit.ScaleLine();
	///     LayoutRoot.Children.Add(ScaleLine1);
	/// 
	///     //Associate the ScaleLine with Map Control (analagous to a OneTime Binding). Most common coding pattern.
	///     ScaleLine1.Map = Map1;
	/// 
	///     //Alternative Binding Method. Useful if the ScaleLine's Properties will dynamically impact other objects.
	///     //System.Windows.Data.Binding myBinding = new System.Windows.Data.Binding();
	///     //myBinding.ElementName = "Map1";
	///     //ScaleLine1.SetBinding(ESRI.ArcGIS.Client.Toolkit.ScaleLine.MapProperty, myBinding);
	///
	///     //Set the alignment properties relative the hosting Grid Control
	///     ScaleLine1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
	///     ScaleLine1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
	/// 
	///     //Set the Map units for the ScaleLine
	///     ScaleLine1.MapUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees;
	/// 
	///     //Set the target width for the ScaleLine
	///     ScaleLine1.TargetWidth = 400;
	/// 
	///     //Set ScaleLine color and related Font information
	///     System.Windows.Media.Color myScaleLineColor = Color.FromArgb(255, 0, 0, 0);
	///     ScaleLine1.Foreground = new System.Windows.Media.SolidColorBrush(myScaleLineColor);
	///     ScaleLine1.FontFamily = new FontFamily("Courier New");
	///     ScaleLine1.FontSize = 18;
	///     </code>
	///     
	/// 	<code title="Example VB1" description="" lang="VB.NET">
	/// 	'Create a new ScaleLine Control and add it to the LayoutRoot (a Grid in the XAML)
	///     Dim ScaleLine1 As New ESRI.ArcGIS.Client.Toolkit.ScaleLine
	///     LayoutRoot.Children.Add(ScaleLine1)
	/// 
	///     'Associate the ScaleLine with Map Control (analagous to a OneTime Binding). Most common coding pattern.
	///     ScaleLine1.Map = Map1
	/// 
	///     'Alternative Binding Method. Useful if the ScaleLine's Properties will dynamically impact other objects.
	///     'Dim myBinding As System.Windows.Data.Binding = New System.Windows.Data.Binding()
	///     'myBinding.ElementName = "Map1"
	///     'ScaleLine1.SetBinding(ESRI.ArcGIS.Client.Toolkit.ScaleLine.MapProperty, myBinding)
	///
	///     'Set the alignment properties relative the hosting Grid Control
	///     ScaleLine1.HorizontalAlignment = Windows.HorizontalAlignment.Left
	///     ScaleLine1.VerticalAlignment = Windows.VerticalAlignment.Top
	/// 
	///     'Set the Map units for the ScaleLine
	///     ScaleLine1.MapUnit = ESRI.ArcGIS.Client.Toolkit.ScaleLine.ScaleLineUnit.DecimalDegrees
	/// 
	///     'Set the target width for the ScaleLine
	///     ScaleLine1.TargetWidth = 400
	/// 
	///     'Set ScaleLine color and related Font information
	///     Dim myScaleLineColor As System.Windows.Media.Color = Color.FromArgb(255, 0, 0, 0)
	///     ScaleLine1.Foreground = New System.Windows.Media.SolidColorBrush(myScaleLineColor)
	///     ScaleLine1.FontFamily = New FontFamily("Courier New")
	///     ScaleLine1.FontSize = 18
	///     </code>
	/// 
	///     <code title="ScaleBar style" description="" lang="XAML">
	///     &lt;Style x:Key="ScaleBarStyle" TargetType="esri:ScaleLine"&gt;
	///         &lt;Setter Property="Background" Value="White" /&gt;
	///         &lt;Setter Property="TargetWidth" Value="150.0" /&gt;
	///         &lt;Setter Property="FontSize" Value="10.0" /&gt;
	///         &lt;Setter Property="Foreground" Value="Black" /&gt;
	///         &lt;Setter Property="Template"&gt;
	///             &lt;Setter.Value&gt;
	///                 &lt;ControlTemplate TargetType="esri:ScaleLine"&gt;
	///                     &lt;StackPanel Name="LayoutRoot" Orientation="Horizontal"&gt;
	///                         &lt;Grid VerticalAlignment="Center" Height="10" Width="{Binding MetricSize, RelativeSource={RelativeSource TemplatedParent}}"&gt;
	///                             &lt;Grid.ColumnDefinitions&gt;
	///                                 &lt;ColumnDefinition Width="1*" /&gt;
	///                                 &lt;ColumnDefinition Width="1*" /&gt;
	///                                 &lt;ColumnDefinition Width="1*" /&gt;
	///                                 &lt;ColumnDefinition Width="2*" /&gt;
	///                                 &lt;ColumnDefinition Width="5*" /&gt;
	///                             &lt;/Grid.ColumnDefinitions&gt;
	///                             &lt;Grid.RowDefinitions&gt;
	///                                 &lt;RowDefinition Height="1*" /&gt;
	///                                 &lt;RowDefinition Height="1*" /&gt;
	///                             &lt;/Grid.RowDefinitions&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Foreground}" Grid.Row="0" Grid.Column="0" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Background}" Grid.Row="0" Grid.Column="1" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Foreground}" Grid.Row="0" Grid.Column="2" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Background}" Grid.Row="0" Grid.Column="3" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Foreground}" Grid.Row="0" Grid.Column="4" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Background}" Grid.Row="1" Grid.Column="0" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Foreground}" Grid.Row="1" Grid.Column="1" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Background}" Grid.Row="1" Grid.Column="2" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Foreground}" Grid.Row="1" Grid.Column="3" /&gt;
	///                             &lt;Rectangle Fill="{TemplateBinding Background}" Grid.Row="1" Grid.Column="4" /&gt;
	///                         &lt;/Grid&gt;
	///                         &lt;TextBlock Text="{Binding MetricValue, RelativeSource={RelativeSource TemplatedParent}}" VerticalAlignment="Center" Margin="2,0"/&gt;
	///                         &lt;TextBlock Text="{Binding MetricUnit, RelativeSource={RelativeSource TemplatedParent}}" VerticalAlignment="Center" /&gt;
	///                     &lt;/StackPanel&gt;
	///                 &lt;/ControlTemplate&gt;
	///             &lt;/Setter.Value&gt;
	///         &lt;/Setter&gt;
	///     &lt;/Style&gt;
	///     </code>
	/// </example>
	
	[TemplatePart(Name = "LayoutRoot", Type = typeof(FrameworkElement))]
	public partial class ScaleLine : Control, INotifyPropertyChanged
	{
		#region Private fields
		private static readonly SpatialReference _webMercSref = new SpatialReference(102100);
		private const double toRadians = 0.017453292519943295769236907684886; //Conversion factor from degrees to radians
		private const double earthRadius = 6378137; //Earth radius in meters (defaults to WGS84 / GRS80)
		private const double degreeDist = 111319.49079327357264771338267052; //earthRadius * toRadians;
		private FrameworkElement _layoutRoot;
		#endregion

		#region Constructors
				/// <summary>
		/// Initializes a new instance of the <see cref="ScaleLine"/> control.
		/// </summary>
		public ScaleLine()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(ScaleLine);
#endif
		}

		/// <summary>
		/// Static initialization for the <see cref="ScaleLine"/> control.
		/// </summary>
		static ScaleLine()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScaleLine),
				new FrameworkPropertyMetadata(typeof(ScaleLine)));
#endif
		} 
		#endregion

		#region OnApplyTemplate
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call 
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In
		/// simplest terms, this means the method is called just before a UI
		/// element displays in an application.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			_layoutRoot = GetTemplateChild("LayoutRoot") as FrameworkElement;
			RefreshScaleline();
			SetDesignValues();
		}

		/// <summary>
		/// Sets the design values.
		/// </summary>
		private void SetDesignValues()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				// set values for design
				USSize = TargetWidth * 0.8;
				USUnit = ScaleLineUnit.Miles;
				USValue = 5;
				MetricSize = TargetWidth;
				MetricUnit = ScaleLineUnit.Kilometers;
				MetricValue = 10;
			}
		}

		#endregion

		#region DependencyProperty Map
		/// <summary>
		/// Gets or sets the map that the scale line is buddied to.
		/// </summary>
		public Map Map
		{
			get { return GetValue(MapProperty) as Map; }
			set { SetValue(MapProperty, value); }
		}

		/// <summary>
		/// Identifies the Map dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty =
			DependencyProperty.Register(
			  "Map",
			  typeof(Map),
			  typeof(ScaleLine),
			  new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScaleLine scaleLine = d as ScaleLine;
			if (scaleLine != null)
				scaleLine.OnMapPropertyChanged(e.OldValue as Map, e.NewValue as Map);
		}

		private void OnMapPropertyChanged(Map oldMap, Map newMap)
		{
			if (oldMap != null)
			{
				oldMap.ExtentChanged -= map_ExtentChanged;
				oldMap.ExtentChanging -= map_ExtentChanged;
				oldMap.PropertyChanged -= Map_PropertyChanged;
			}
			if (newMap != null)
			{
				newMap.ExtentChanged += map_ExtentChanged;
				newMap.ExtentChanging += map_ExtentChanged;
				newMap.PropertyChanged += Map_PropertyChanged;
			}
			InitializeMapUnit();
			RefreshScaleline();
		}

		private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SpatialReference")
			{
				InitializeMapUnit();
				RefreshScaleline();
			}
		} 
		#endregion

		#region DependencyProperty TargetWith

		/// <summary>
		/// Gets or sets the target width of the scale line.
		/// </summary>
		/// <remarks>The actual width of the scale line changes when values are rounded.</remarks>
		public double TargetWidth
		{
			get { return (double)GetValue(TargetWidthProperty); }
			set { SetValue(TargetWidthProperty, value); }
		}


		/// <summary>
		/// Identifies the <see cref="TargetWidth"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register("TargetWidth", typeof(double), typeof(ScaleLine), new PropertyMetadata(150.0, OnTargetWidthPropertyChanged));

		private static void OnTargetWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScaleLine scaleLine = d as ScaleLine;
			if (scaleLine != null)
			{
				scaleLine.RefreshScaleline();
				scaleLine.SetDesignValues();
			}
		}

		#endregion

		#region DependencyProperty MapUnit

		/// <summary>
		/// Gets or sets the map unit.
		/// </summary>
		/// <remarks>
		/// 	If the MapUnit is  not set manually, the scale line control will use a default map unit which is calculated from the spatial reference of the map
		/// 	or from the <see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer.Units"></see> of the layers inside the map.
		/// </remarks>
		public ScaleLineUnit MapUnit
		{
			get { return (ScaleLineUnit)GetValue(MapUnitProperty); }
			set { SetValue(MapUnitProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MapUnit"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapUnitProperty = DependencyProperty.Register("MapUnit", typeof(ScaleLineUnit), typeof(ScaleLine), new PropertyMetadata(ScaleLineUnit.DecimalDegrees, OnMapUnitPropertyChanged));

		private static void OnMapUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScaleLine scaleLine = d as ScaleLine;
			if (scaleLine != null)
			{
				scaleLine.RefreshScaleline();
			}
		}

		#endregion

		#region Properties

		#region USValue
		private double _usValue;

		/// <summary>
		/// The text displayed on the ScaleLine that denotes the value of the length of the ScaleLine in US Units (Miles or Feet).
		/// <remarks>This is a rounded value.</remarks>
		/// </summary>
		public double USValue
		{
			get { return _usValue;}
			private set
			{
				_usValue = value;
				OnPropertyChanged("USValue");
			}
		} 
		#endregion

		#region USUnit
		private ScaleLineUnit _usUnit;

		/// <summary>
		/// The US unit (Miles or Feet).
		/// </summary>
		/// <value>The US unit.</value>
		public ScaleLineUnit USUnit
		{
			get { return _usUnit; }
			private set
			{
				_usUnit = value;
				OnPropertyChanged("USUnit");
			}
		} 
		#endregion

		#region USSize
		private double _usSize;

		/// <summary>
		/// The width of the US ScaleLine in pixels on the display screen.
		/// </summary>
		public double USSize
		{
			get { return _usSize; }
			private set
			{
				_usSize = value;
				OnPropertyChanged("USSize");
			}
		} 
		#endregion

		#region MetricValue
		private double _metricValue;

		/// <summary>
		/// The text displayed on the ScaleLine that denotes the value of the length of the ScaleLine in Metric Units (Kilometers or Meters).
		/// <remarks>This is a rounded value.</remarks>
		/// </summary>
		public double MetricValue
		{
			get { return _metricValue; }
			private set
			{
				_metricValue = value;
				OnPropertyChanged("MetricValue");
			}
		}
		#endregion

		#region MetricUnit
		private ScaleLineUnit _metricUnit;

		/// <summary>
		/// The metric unit (Kilometers or Meters).
		/// </summary>
		/// <value>The metric unit.</value>
		public ScaleLineUnit MetricUnit
		{
			get { return _metricUnit; }
			private set
			{
				_metricUnit = value;
				OnPropertyChanged("MetricUnit");
			}
		}
		#endregion

		#region MetricSize
		private double _metricSize;

		/// <summary>
		/// The width of the metric ScaleLine in pixels on the display screen.
		/// </summary>
		public double MetricSize
		{
			get { return _metricSize; }
			private set
			{
				_metricSize = value;
				OnPropertyChanged("MetricSize");
			}
		}
		#endregion
		#endregion

		#region INotifyPropertyChanged Members
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		internal void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		#region Helper Functions

		/// <summary>
		/// Handles the ExtentChanged and ExtentChanging event of the map control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="ESRI.ArcGIS.Client.ExtentEventArgs"/> 
		/// instance containing the event data.</param>
		private void map_ExtentChanged(object sender, ExtentEventArgs args)
		{
			RefreshScaleline();
		}

		/// <summary>
		/// Refreshes the scaleline when the map extent changes.
		/// </summary>
		private void RefreshScaleline()
		{
			if (Map == null || double.IsNaN(Map.Resolution) ||
				MapUnit == ScaleLineUnit.DecimalDegrees && Math.Abs(Map.Extent.GetCenter().Y) >= 90)
			{
				if (!DesignerProperties.GetIsInDesignMode(this) && _layoutRoot != null)
					_layoutRoot.Visibility = Visibility.Collapsed;
				return;
			}

			if (_layoutRoot != null)
				_layoutRoot.Visibility = Visibility.Visible;

			ScaleLineUnit outUnit;
			double outResolution;

			#region KiloMeters/Meters
			double roundedKiloMeters = GetBestEstimateOfValue(Map.Resolution, ScaleLineUnit.Kilometers, out outUnit, out outResolution);
			double widthMeters = roundedKiloMeters / outResolution;

			MetricValue = roundedKiloMeters;
			MetricSize = widthMeters;
			MetricUnit = outUnit;
			#endregion

			#region Miles
			double roundedMiles = GetBestEstimateOfValue(Map.Resolution, ScaleLineUnit.Miles, out outUnit, out outResolution);
			double widthMiles = roundedMiles / outResolution;

			USValue = roundedMiles;
			USSize = widthMiles;
			USUnit = outUnit;
			#endregion
		}

		private double GetBestEstimateOfValue(double resolution, ScaleLineUnit displayUnit, out ScaleLineUnit unit, out double outResolution)
		{
			unit = displayUnit;
			double rounded = 0;
			double originalRes = resolution;
			while (rounded < 0.5)
			{
				resolution = originalRes;
				if (MapUnit == ScaleLineUnit.DecimalDegrees)
				{
					resolution = GetResolutionForGeographic(Map.Extent.GetCenter(), resolution);
					resolution = resolution * (int)ScaleLineUnit.Meters / (int)unit;
				}
				else if (_webMercSref.Equals(Map.SpatialReference))
				{
					//WebMercator
					double mercatorStretch = 1 / Math.Cosh(Map.Extent.GetCenter().Y / earthRadius); // = Cos(lat)
					resolution = mercatorStretch * originalRes * (int)ScaleLineUnit.Meters / (int)unit;
				}
				else if (MapUnit != ScaleLineUnit.Undefined)
				{
					resolution = resolution * (int)MapUnit / (int)unit;
				}

				double val = TargetWidth * resolution;
				val = RoundToSignificant(val, resolution);
				double noFrac = Math.Round(val); // to get rid of the fraction
				if (val < 0.5)
				{
					ScaleLineUnit newUnit = ScaleLineUnit.Undefined;
					// Automatically switch unit to a lower one
					if (unit == ScaleLineUnit.Kilometers)
						newUnit = ScaleLineUnit.Meters;
					else if (unit == ScaleLineUnit.Miles)
						newUnit = ScaleLineUnit.Feet;
					if (newUnit == ScaleLineUnit.Undefined) { break; } //no lower unit
					unit = newUnit;
				}
				else if (noFrac > 1)
				{
					rounded = noFrac;
					var len = noFrac.ToString().Length;
					if (len <= 2)
					{
						// single/double digits ... make it a multiple of 5 ..or 1,2,3,4
						if (noFrac > 5)
						{
							rounded -= noFrac % 5;
						}
						while (rounded > 1 && (rounded / resolution) > TargetWidth)
						{
							// exceeded maxWidth .. decrement by 1 or by 5
							double decr = noFrac > 5 ? 5 : 1;
							rounded = rounded - decr;
						}
					}
					else if (len > 2)
					{
						rounded = Math.Round(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1);
						if ((rounded / resolution) > TargetWidth)
						{
							// exceeded maxWidth .. use the lower bound instead
							rounded = Math.Floor(noFrac / Math.Pow(10, len - 1)) * Math.Pow(10, len - 1);
						}
					}
				}
				else
				{ // anything between 0.5 and 1
					rounded = Math.Floor(val);
					if (rounded == 0)
					{
						//val >= 0.5 but < 1 so round up
						rounded = (val == 0.5) ? 0.5 : 1;
						if ((rounded / resolution) > TargetWidth)
						{
							// exceeded maxWidth .. re-try by switching to lower unit 
							rounded = 0;
							ScaleLineUnit newUnit = ScaleLineUnit.Undefined;
							// Automatically switch unit to a lower one
							if (unit == ScaleLineUnit.Kilometers)
								newUnit = ScaleLineUnit.Meters;
							else if (unit == ScaleLineUnit.Miles)
								newUnit = ScaleLineUnit.Feet;
							if (newUnit == ScaleLineUnit.Undefined) { break; } //no lower unit
							unit = newUnit;
						}
					}
				}
			}
			outResolution = resolution;
			return rounded;
		}

		/// <summary>
		/// Rounds to a value to the significant number of digits.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="resolution">The resolution.</param>
		/// <returns></returns>
		private static double RoundToSignificant(double value, double resolution)
		{
			var round = Math.Floor(-Math.Log(resolution));
			if (round > 0)
			{
				round = Math.Pow(10, round);
				return Math.Round(value * round) / round;
			}
			return Math.Round(value);
		}


		/// <summary>
		/// Calculates horizontal scale at center of extent
		/// for geographic / Plate Carrée projection.
		/// Horizontal scale is 0 at the poles.
		/// </summary>
		private static double GetResolutionForGeographic(MapPoint center, double resolution)
		{
			double y = center.Y;
			if (Math.Abs(y) > 90) { return 0; }
			return Math.Cos(y * toRadians) * resolution * degreeDist;
		}

		/// <summary>
		/// Try to initialize the map units
		/// </summary>
		private void InitializeMapUnit()
		{
			if (Map == null || Map.SpatialReference == null)
				return;

			// First test the well know spatial references
			if (Map.SpatialReference.WKID == 4326)
				MapUnit = ScaleLineUnit.DecimalDegrees;
			else if (_webMercSref.Equals(Map.SpatialReference))
				MapUnit = ScaleLineUnit.Meters;
			else
			{
				Layer layer = Map.Layers == null ? null :
					Map.Layers.FirstOrDefault(l => l.SpatialReference != null && l.SpatialReference.Equals(Map.SpatialReference));

				string layerUnits;
				if (layer is ArcGISDynamicMapServiceLayer)
				{
					layerUnits = ((ArcGISDynamicMapServiceLayer)layer).Units;
				}
				else if (layer is ArcGISTiledMapServiceLayer)
				{
					layerUnits = ((ArcGISTiledMapServiceLayer)layer).Units;
				}
				else
					layerUnits = null;

				if (!string.IsNullOrEmpty(layerUnits))
				{
					// Remove leading 'esri' to layerUnits
					if (layerUnits.StartsWith("esri"))
						layerUnits = layerUnits.Substring(4);

					try
					{
						ScaleLineUnit unit = (ScaleLineUnit)Enum.Parse(typeof(ScaleLineUnit), layerUnits, true);
						MapUnit = unit;
					}
					catch (ArgumentException) // layersUnits is not one of the named constants defined for the enumeration
					{
					}
				}
			}
		}
		#endregion

	}
}