// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Threading;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	/// <summary>
	/// GPS Layer showing the current location and accuracy.
	/// </summary>
	public partial class GpsLayer : GraphicsLayer, INotifyPropertyChanged
	{
		#region Private Fields
		private MapPoint position;
		private Graphic location;
		private Graphic accuracyCircle;
		private DispatcherTimer animationTimerLocation;
		private DispatcherTimer animationTimerAccuracy;
		private static ESRI.ArcGIS.Client.Projection.WebMercator merc = new ESRI.ArcGIS.Client.Projection.WebMercator();
		private DateTime animateStartTime;
		private DateTime animateAccuracyStartTime;
		private MapPoint animateCenterFrom;
		private MapPoint animateCenterTo;
		private const int animationDuration = 500;
		private double animateRadiusFrom;
		private double animateRadiusTo;
		private static SpatialReference WebMercatorSR = new SpatialReference(102100);
		#endregion
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GpsLayer"/> class.
		/// </summary>
		public GpsLayer()
		{
			location = new Graphic();
			GeoPositionWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
			System.Windows.Media.RadialGradientBrush brush = new System.Windows.Media.RadialGradientBrush()
			{
				Center = new Point(.25, .25),
				GradientOrigin = new Point(.25, .25),
				RadiusX = 1,
				RadiusY = 1
			};

			LocationMarkerSymbol = new Gps.GpsSymbol();
			
			accuracyCircle = new Graphic {  Geometry = new Circle() };
			AccuracyCircleSymbol = new SimpleFillSymbol() {
					BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(90, 255, 255, 255)),
					Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(20, 65, 76, 249)),
					BorderThickness = 3
				};
			Graphics.Add(accuracyCircle);
			Graphics.Add(location);
			location.SetZIndex(1);

			IsHitTestVisible = false;
			
			animationTimerLocation = new DispatcherTimer();
			animationTimerLocation.Interval = TimeSpan.FromMilliseconds(33);
			animationTimerLocation.Tick += animationTimerLocation_Tick;

			animationTimerAccuracy = new DispatcherTimer();
			animationTimerAccuracy.Interval = TimeSpan.FromMilliseconds(33);
			animationTimerAccuracy.Tick += animationTimerAccuracy_Tick;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="GpsLayer"/> is reclaimed by garbage collection.
		/// </summary>
		~GpsLayer()
		{
			try
			{
				if (GeoPositionWatcher != null)
				{
					DetachListeners(GeoPositionWatcher);
					GeoPositionWatcher.Stop();
				}
			}
			catch { }
		}

		/// <summary>
		/// Initializes the resource.
		/// </summary>
		/// <remarks>
		/// 	<para>Override this method if your resource requires asyncronous requests to initialize,
		/// and call the base method when initialization is completed.</para>
		/// 	<para>Upon completion of initialization, check the 
		/// 	<see cref="P:ESRI.ArcGIS.Client.Layer.InitializationFailure"/> for any possible errors.</para>
		/// </remarks>
		/// <seealso cref="E:ESRI.ArcGIS.Client.Layer.Initialized"/>
		/// <seealso cref="P:ESRI.ArcGIS.Client.Layer.InitializationFailure"/>
		public override void Initialize()
		{
			base.Initialize();
			if (GeoPositionWatcher != null)
				this.TryStartGps();
		}

		#region GPS GeoWatcher

		/// <summary>
		/// Attempts the start the geowatcher.
		/// </summary>
        private void TryStartGps()
        {
            bool permission = true;
            if (GeoPositionWatcher is GeoCoordinateWatcher)
                permission = (GeoPositionWatcher as GeoCoordinateWatcher).Permission != GeoPositionPermission.Denied;
#if WINDOWS_PHONE
            if (permission && Visible && Map != null && IsEnabled && GeoPositionWatcher != null)
                GeoPositionWatcher.Start(true); //TryStart's behavior is not consistent across WP OS versions
            if (GeoPositionWatcher != null && GeoPositionWatcher.Position != null &&
                GeoPositionWatcher.Position.Location != null && !GeoPositionWatcher.Position.Location.IsUnknown)
                UpdatePosition(GeoPositionWatcher.Position);
#else
            bool result = false;
            if (permission && Visible && Map != null && IsEnabled && GeoPositionWatcher != null)
                result = GeoPositionWatcher.TryStart(true, TimeSpan.FromSeconds(10));
			if (result && GeoPositionWatcher != null && GeoPositionWatcher.Position != null &&
				GeoPositionWatcher.Position.Location != null && !GeoPositionWatcher.Position.Location.IsUnknown)
				UpdatePosition(GeoPositionWatcher.Position);
#endif
        }

		/// <summary>
		/// Handles the StatusChanged event of the watcher control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Device.Location.GeoPositionStatusChangedEventArgs"/> 
		/// instance containing the event data.</param>
		private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
		{
			switch (e.Status)
			{
				case GeoPositionStatus.Disabled:
				case GeoPositionStatus.Initializing:
				case GeoPositionStatus.NoData:
					UpdateLocation();
					UpdateAccuracyCircle();
					GeoCoordinate = null;
					Position = null;
					break;
				case GeoPositionStatus.Ready:
					//
					break;
			}
		}

		/// <summary>
		/// Handles the PositionChanged event of the watcher control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		/// The <see cref="System.Device.Location.GeoPositionChangedEventArgs{T}"/>
		/// instance containing the event data.
		/// </param>
		private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
		{
			UpdatePosition(e.Position);
		}

		private void UpdatePosition(GeoPosition<GeoCoordinate> position)
		{
			GeoCoordinate = position.Location;
			accuracyCircle.Attributes["position"] = position;
			location.Attributes["position"] = position;
			UpdateLocation();
			UpdateAccuracyCircle();
			if (location.Symbol is Gps.GpsSymbol)
			{
				//Triggers changing visual state which makes it strobe every time an update is received
				location.Selected = !location.Selected;

				var symbol = location.Symbol as Gps.GpsSymbol;
				if (GeoCoordinate == null || double.IsNaN(GeoCoordinate.Course) || double.IsNaN(GeoCoordinate.Speed))
				{
					symbol.Course = 0;
					symbol.Speed = 0;
				}
				else
				{
					symbol.Course = GeoCoordinate.Course;
					symbol.Speed = GeoCoordinate.Speed;
				}
			}
		}

		#endregion
		
		/// <summary>
		/// Starts animation of the circle
		/// </summary>
		private void UpdateAccuracyCircle()
		{
			if (Map != null && !double.IsNaN(Map.Resolution))
			{
				var c = accuracyCircle.Geometry as Circle;
				if (GeoCoordinate == null || double.IsNaN(GeoCoordinate.HorizontalAccuracy) || GeoCoordinate.HorizontalAccuracy < 15)
				{
					c.Radius = double.NaN;
					c.UpdateRing();
				}
				else
				{
					double size = GetAccuracyInMapUnits();
					if (c.Radius != size)
					{
						if (location.Geometry != null)
						{
							animateRadiusFrom = double.IsNaN(c.Radius) ? 0 : c.Radius;
							animateRadiusTo = size;
							if (AnimateUpdates)
							{
								animateAccuracyStartTime = DateTime.Now;
								animationTimerAccuracy.Start();
							}
							else 
							{
								c.Radius = size;
								c.UpdateRing();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Starts animating the location.
		/// </summary>
		private void UpdateLocation()
		{
			if (Map != null && Map.SpatialReference != null && GeoCoordinate != null && GeoCoordinate != GeoCoordinate.Unknown )
			{
				MapPoint newLocation = new ESRI.ArcGIS.Client.Geometry.MapPoint(GeoCoordinate.Longitude, GeoCoordinate.Latitude) { SpatialReference = new SpatialReference(4326) };
				if (!Map.SpatialReference.Equals(newLocation.SpatialReference))
				{
					if (WebMercatorSR.Equals(Map.SpatialReference))
						newLocation = merc.FromGeographic(newLocation) as MapPoint;
					else
					{
						if (ProjectionService != null)
						{
							if (!ProjectionService.IsBusy)
							{
								var geom = newLocation;
								EventHandler<Tasks.GraphicsEventArgs> handler = null;
								handler = (a, b) =>
									{
										(a as IProjectionService).ProjectCompleted -= handler;
										if (b.Results != null && b.Results.Count > 0 && b.Results[0].Geometry is MapPoint)
											BeginAnimateLocation(b.Results[0].Geometry as MapPoint);
									};
								ProjectionService.ProjectCompleted += handler;
								ProjectionService.ProjectAsync(new Graphic[] { new Graphic() { Geometry = geom } }, Map.SpatialReference);
							}
							else
							{
								EventHandler<Tasks.GraphicsEventArgs> handler = null;
								handler = (a, b) =>
									{
										ProjectionService.ProjectCompleted -= handler;
										UpdateLocation(); //Try again
									};
								ProjectionService.ProjectCompleted += handler; //Wait for task to complete
							}
						}
						return; //Wait for projection to complete
					}
				}
				BeginAnimateLocation(newLocation);
			}
		}

		private void BeginAnimateLocation(MapPoint newLocation)
		{
			if (location.Geometry == null || !AnimateUpdates)
			{
				var c = accuracyCircle.Geometry as Circle;
				location.Geometry = c.Center = animateCenterFrom = animateCenterTo = newLocation;
				c.UpdateRing();
			}
			else
			{
				animateCenterFrom = location.Geometry as MapPoint;
				animateCenterTo = newLocation;
				animateStartTime = DateTime.Now;
				animationTimerLocation.Start();
			}
			Position = newLocation;
		}

		/// <summary>
		/// Handles the Tick event of the animationTimerLocation timer
		/// and animates the location point and center of the accuracy circle.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void animationTimerLocation_Tick(object sender, EventArgs e)
		{
			double fraction = (DateTime.Now - animateStartTime).TotalMilliseconds / animationDuration;
			var c = accuracyCircle.Geometry as Circle;
			if (fraction >= 1)
			{
				location.Geometry = animateCenterFrom = animateCenterTo;
				c.Center = location.Geometry as MapPoint; 
				animationTimerLocation.Stop();
			}
			else
			{
				fraction = quinticEaseOut(fraction, 0, 1, 1);
				var x = (animateCenterTo.X - animateCenterFrom.X) * fraction + animateCenterFrom.X;
				var y = (animateCenterTo.Y - animateCenterFrom.Y) * fraction + animateCenterFrom.Y;
				location.Geometry = c.Center = new MapPoint(x, y);
				c.Center = location.Geometry as MapPoint;
			}
			c.UpdateRing();
		}

		/// <summary>
		/// Handles the Tick event of the animationTimerAccuracy timer
		/// and animates radius of the accuracy circle.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void animationTimerAccuracy_Tick(object sender, EventArgs e)
		{
			double fraction = (DateTime.Now - animateAccuracyStartTime).TotalMilliseconds / animationDuration;
			var c = accuracyCircle.Geometry as Circle;
			if (fraction >= 1)
			{
				c.Radius = animateRadiusFrom = animateRadiusTo;
				animationTimerAccuracy.Stop();
			}
			else
			{
				fraction = quinticEaseOut(fraction, 0, 1, 1);
				c.Radius = (animateRadiusTo - animateRadiusFrom) * fraction + animateRadiusFrom;
			}
			c.UpdateRing();
		}

		internal static double quinticEaseOut(double t, double b, double c, double d)
		{
			//quintic ease out
			t /= d;
			t--;
			return -c * (t * t * t * t - 1) + b;
		}

		#region Calculate accuracy

		/// <summary>
		/// Gets the horizontal accuracy in map units.
		/// </summary>
		/// <returns></returns>
		private double GetAccuracyInMapUnits()
		{
			if (Map != null && Map.SpatialReference != null && GeoCoordinate != null)
			{
				//From MSDN doc on GeoCoordinate.HorizontalAccuracy: 
				//The accuracy of the latitude and longitude, in meters.
				//The accuracy can be considered the radius of certainty of the 
				//latitude and longitude data. A circular area that is formed 
				//with the accuracy as the radius and the latitude and longitude
				//coordinates as the center contains the actual location.
				double acc = GeoCoordinate.HorizontalAccuracy;
				string unit = GetMapUnits();
				switch(unit)
				{
					case "esriDecimalDegrees":
						double brng = 0;
						double lon1 = GeoCoordinate.Longitude / 180 * Math.PI;
						double lat1 = GeoCoordinate.Latitude / 180 * Math.PI;
						double dR = acc / 6378137; //Angular distance in radians
						double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
						double lat = lat2 / Math.PI * 180;
						while (lat < -90) lat += 180;
						while (lat > 90) lat -= 180;
						return Math.Abs(lat - GeoCoordinate.Latitude);
					case "esriMeters":
						return acc;
					case "esriMillimeters":
						return acc * 1000;
					case "esriCentimeters":
						return acc * 100;
					case "esriDecimeters":
						return acc * 10;
					case "esriKilometers":
						return acc * 0.001;
					case "esriInches":
						return acc * 39.3700787;
					case "esriFeet":
						return acc * 3.2808399;
					case "esriYards":
						return acc * 1.0936133;
					case "esriMiles":
						return acc * 0.000621371192;
					case "esriNauticalMiles":
						return acc * 0.000539956803;
					default:
						return GetAccuracyInMapUnitsFromScale();
				}
			}
			else return double.NaN;
		}

		private double GetAccuracyInMapUnitsFromScale()
		{
			var scale = Map.Scale;
			var resolution = Map.Resolution;
			double acc = GeoCoordinate.HorizontalAccuracy;
			if (!double.IsNaN(scale) && !double.IsNaN(resolution) && !double.IsNaN(acc) )
			{
				var resInMeters = (96 * resolution) / scale * 39.3700787;
				return resInMeters * acc;
			}
			return double.NaN;
		}

		/// <summary>
		/// Gets the map units from the map.
		/// This requires the map to be in either WGS84 or WebMercator spatial reference,
		/// or have at least one ArcGIS layer whos default spatial reference matches that
		/// of the map.
		/// </summary>
		/// <returns></returns>
		private string GetMapUnits()
		{
			if (Map == null || Map.SpatialReference == null) return null;
			if (Map.SpatialReference.WKID == 4326) return "esriDecimalDegrees";
			if (Map.SpatialReference.Equals(new SpatialReference(102100))) return "esriMeters";
			foreach (Layer l in Map.Layers)
			{
				if (Map.SpatialReference.Equals(l.SpatialReference))
				{
					if (l is ArcGISTiledMapServiceLayer)
						return (l as ArcGISTiledMapServiceLayer).Units;
					if (l is ArcGISDynamicMapServiceLayer)
						return (l as ArcGISDynamicMapServiceLayer).Units;
				}
			}
			return null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the geographic coordinate from last known GPS position
		/// </summary>
		/// <remarks>
		/// The <see cref="GeoCoordinateChanged"/> event will be fired when this updates.
		/// </remarks>
		public GeoCoordinate GeoCoordinate
		{
			get { return (GeoCoordinate)GetValue(GeoCoordinateProperty); }
			private set { SetValue(GeoCoordinateProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GeoCoordinate"/> dependency property.
		/// </summary>
		private static readonly DependencyProperty GeoCoordinateProperty =
			DependencyProperty.Register("GeoCoordinate", typeof(GeoCoordinate), typeof(GpsLayer), new PropertyMetadata(null, OnGeoCoordinatePropertyChanged));

		private static void OnGeoCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GpsLayer obj = (GpsLayer)d;
			if (obj.GeoCoordinateChanged != null)
				obj.GeoCoordinateChanged(obj, EventArgs.Empty);
			obj.OnPropertyChanged("GeoCoordinate");
		}
		

#if !SILVERLIGHT
		private void GpsLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)delegate()
			{
				if (this.GeoPositionWatcher != null && this.GeoPositionWatcher.Position != null)
				{
					UpdatePosition(this.GeoPositionWatcher.Position);
				}
			});
		}
#endif

		/// <summary>
		/// Gets the coordinate in the Map's spatial reference from last known GPS position
		/// </summary>
		/// <remarks>
		/// The <see cref="PositionChanged"/> event will be fired when this updates.
		/// </remarks>
		public MapPoint Position
		{
			private set
			{
				if (position != value)
				{
					position = value;
					if (PositionChanged != null)
						PositionChanged(this, EventArgs.Empty);
					OnPropertyChanged("Position");
				}
			}
			get { return position; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the layer should animate GPS marker and accuracy circle on each update.
		/// </summary>
		/// <value><c>true</c> if [animate updates]; otherwise, <c>false</c>.</value>
		public bool AnimateUpdates
		{
			get { return (bool)GetValue(AnimateUpdatesProperty); }
			set { SetValue(AnimateUpdatesProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="AnimateUpdates"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty AnimateUpdatesProperty =
			DependencyProperty.Register("AnimateUpdates", typeof(bool), typeof(GpsLayer), new PropertyMetadata(true));

		#region Map

		/// <summary>
		/// Override to know when a layer's <see cref="Map"/> property changes.
		/// </summary>
		/// <param name="oldValue">Old map</param>
		/// <param name="newValue">New map</param>
		protected override void OnMapChanged(Map oldValue, Map newValue)
		{
			base.OnMapChanged(oldValue, newValue);
			if (oldValue != null)
				oldValue.PropertyChanged -= map_PropertyChanged;
			if (newValue != null)
			{
				newValue.PropertyChanged += map_PropertyChanged;
                if(GeoPositionWatcher != null)
                    TryStartGps();
			}
			else if (GeoPositionWatcher != null) 
				GeoPositionWatcher.Stop();
		}

		private void map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SpatialReference")
			{
				Dispatcher.BeginInvoke((Action)delegate()
				{
					UpdateLocation();
					UpdateAccuracyCircle();
				});
			}
		}

		#endregion

#if WINDOWS_PHONE

		#region GeometryServiceUrl
		/// <summary>
		/// Gets or sets the Url to the geometry service to use if the map is in a 
		/// spatial reference different than WGS84 or WebMercator.
		/// </summary>
		/// <value>The geometry service URL.</value>
		[Obsolete("Use the ProjectionService property")]
		public string GeometryServiceUrl
		{
			get { return (string)GetValue(GeometryServiceUrlProperty); }
			set { SetValue(GeometryServiceUrlProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GeometryServiceUrl"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GeometryServiceUrlProperty =
			DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(GpsLayer), new PropertyMetadata(OnGeometryServiceUrlPropertyChanged));

		private static void OnGeometryServiceUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var layer = d as GpsLayer;
			string newUrl = e.NewValue as string;
			if(layer.ProjectionService == null)
			{
				layer.ProjectionService = new ESRI.ArcGIS.Client.Tasks.GeometryService(newUrl);
			}	
		}
		#endregion
#endif

		#region LocationMarkerSymbol
		/// <summary>
		/// Gets or sets the symbol used for the location marker.
		/// </summary>
		/// <value>The location marker symbol.</value>
		public MarkerSymbol LocationMarkerSymbol
		{
			get { return (MarkerSymbol)GetValue(LocationMarkerSymbolProperty); }
			set { SetValue(LocationMarkerSymbolProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LocationMarkerSymbol"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LocationMarkerSymbolProperty =
			DependencyProperty.Register("LocationMarkerSymbol", typeof(MarkerSymbol), typeof(GpsLayer), new PropertyMetadata(OnLocationMarkerSymbolPropertyChanged));

		private static void OnLocationMarkerSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as GpsLayer).location.Symbol = e.NewValue as Symbol;
		}
		#endregion

		#region AccuracyCircleSymbol
		/// <summary>
		/// Gets or sets the symbol used on the accuracy circle.
		/// </summary>
		/// <value>The accuracy circle symbol.</value>
		public FillSymbol AccuracyCircleSymbol
		{
			get { return (FillSymbol)GetValue(AccuracyCircleSymbolProperty); }
			set { SetValue(AccuracyCircleSymbolProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="AccuracyCircleSymbol"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty AccuracyCircleSymbolProperty =
			DependencyProperty.Register("AccuracyCircleSymbol", typeof(FillSymbol), typeof(GpsLayer), new PropertyMetadata(OnAccuracyCircleSymbolPropertyChanged));

		private static void OnAccuracyCircleSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as GpsLayer).accuracyCircle.Symbol = e.NewValue as Symbol;
		}
		#endregion

		#region IsEnabled
		/// <summary>
		/// Toggles the GPS tracking on and off
		/// </summary>
		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="IsEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(GpsLayer), new PropertyMetadata(true, OnIsEnabledPropertyChanged));
		
		private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GpsLayer layer = d as GpsLayer;
			if (!(bool)e.NewValue)
			{
				if (layer.GeoPositionWatcher != null)
					layer.GeoPositionWatcher.Stop();
			}
			else
				layer.TryStartGps();
		}
		#endregion

		#region GeoPositionWatcher

		/// <summary>
		/// Gets or sets the GeoPositionWatcher used to track position.
		/// </summary>
		/// <remarks>
		/// By default the position watcher used is Windows Phone's built-in <see cref="GeoCoordinateWatcher"/>.
		/// Overriding this makes it possible to assign a simulator instead.
		/// </remarks>
		public IGeoPositionWatcher<GeoCoordinate> GeoPositionWatcher
		{
			get { return (IGeoPositionWatcher<GeoCoordinate>)GetValue(GeoPositionWatcherProperty); }
			set { SetValue(GeoPositionWatcherProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GeoPositionWatcher"/> attached dependency property.
		/// </summary>
		public static readonly DependencyProperty GeoPositionWatcherProperty =
			DependencyProperty.Register("GeoPositionWatcher", typeof(IGeoPositionWatcher<GeoCoordinate>), typeof(GpsLayer), new PropertyMetadata(GeoPositionWatcherPropertyChanged));

		private static void GeoPositionWatcherPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GpsLayer layer = d as GpsLayer;
			if (e.OldValue != null)
			{
				layer.DetachListeners(e.OldValue as IGeoPositionWatcher<GeoCoordinate>);
			}
			if (e.NewValue != null)
			{
				layer.AttachListeners(e.NewValue as IGeoPositionWatcher<GeoCoordinate>);
				if (layer.IsInitialized && layer.IsEnabled)
					layer.TryStartGps();
			}
		}

		private void AttachListeners(IGeoPositionWatcher<GeoCoordinate> geowatcher)
		{
			geowatcher.StatusChanged += watcher_StatusChanged;
			geowatcher.PositionChanged += watcher_PositionChanged;
#if !SILVERLIGHT
			//This is to work around a bug in .NET's GeoCoordinateWatcher, where CoordinateChanged doesn't 
			//fire when it should but instead rely on INotifyPropertyChanged
			if (geowatcher is INotifyPropertyChanged && geowatcher is GeoCoordinateWatcher)
			{
				(geowatcher as INotifyPropertyChanged).PropertyChanged += GpsLayer_PropertyChanged;
			}
#endif
		}

		private void DetachListeners(IGeoPositionWatcher<GeoCoordinate> geowatcher)
		{
			geowatcher.StatusChanged -= watcher_StatusChanged;
			geowatcher.PositionChanged -= watcher_PositionChanged;
#if !SILVERLIGHT
			if (geowatcher is INotifyPropertyChanged && geowatcher is GeoCoordinateWatcher)
			{
					(geowatcher as INotifyPropertyChanged).PropertyChanged -= GpsLayer_PropertyChanged;
			}
#endif
		}

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// Occurs when <see cref="Position"/> property has changed.
		/// </summary>
		public event EventHandler PositionChanged;
		/// <summary>
		/// Occurs when <see cref="GeoCoordinate"/> property has changed.
		/// </summary>
		public event EventHandler GeoCoordinateChanged;

		#endregion

		/// <summary>
		/// Polygon Helper class for describing a circle
		/// </summary>
		private class Circle : Polygon
		{
			public Circle()
			{
				PointCount = 90;
				Radius = double.NaN;
			}

			public double Radius;
			public MapPoint Center;
			public int PointCount;

			public void UpdateRing()
			{
				if (!double.IsNaN(Radius) && Radius > 0 && Center != null && PointCount > 2)
				{
					PointCollection pnts = new PointCollection();
					for (int i = PointCount; i >= 0; i--)
					{
						double rad = 2 * Math.PI / PointCount * i;
						double x = Math.Cos(rad) * Radius + Center.X;
						double y = Math.Sin(rad) * Radius + Center.Y;
						pnts.Add(new MapPoint(x, y));
					}
					if (Rings.Count == 0)
						Rings.Add(pnts);
					else
						Rings[0] = pnts;
				}
				else
                    Rings.Clear();
			}
		}
	}
}




