// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// OverviewMap Control
	/// </summary>
	[TemplatePart(Name = "OVMapImage", Type = typeof(Map))]
	[TemplatePart(Name = "AOI", Type = typeof(Grid))]
	[System.Windows.Markup.ContentProperty("Layer")]
	public partial class OverviewMap : Control
	{	
		private Envelope mapExtent;
		private Envelope fullExtent;
		private Envelope lastMapExtent = new Envelope();
		private Point startPoint;
		private RotateTransform rotateTransform;
		double offsetLeft = 0;
		double offsetTop = 0;
		private bool dragOn = false;
		private double maxWidth = 0;
		private double maxHeight = 0;
		private bool isInitialized = false;		

		#region Template items
		Map OVMapImage;
		Grid AOI;		
		#endregion
		
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="OverviewMap"/> class.
		/// </summary>
		public OverviewMap()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(OverviewMap);
#endif
			this.Loaded += (s, e) =>
			{				
				UpdateOVMap(); 
#if !SILVERLIGHT
				UpdateAOI();
#endif
			};
		}
		/// <summary>
		/// Static initialization for the <see cref="OverviewMap"/> control.
		/// </summary>
		static OverviewMap()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(OverviewMap),
				new FrameworkPropertyMetadata(typeof(OverviewMap)));
#endif
		}
		#endregion

		#region Overrides
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code 
		/// or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.Layer != null && OVMapImage != null)
			{
				OVMapImage.Layers.Remove(this.Layer);
			}
			OVMapImage = GetTemplateChild("OVMapImage") as Map;
			if (OVMapImage != null)
			{
				OVMapImage.ExtentChanged += (s, e) => { UpdateAOI(); };
				OVMapImage.Layers.LayersInitialized += (s, e) => 
				{
					isInitialized = true;
					if(MaximumExtent != null)
						UpdateExtentToMaixumExtent();
					ZoomToNewExtent(); 
				};
				if (this.Layer != null)
					this.OVMapImage.Layers.Add(this.Layer);				
			}

			AOI = GetTemplateChild("AOI") as Grid;
			if (AOI != null)
				AOI.MouseLeftButtonDown += AOI_MouseLeftButtonDown;			

			UpdateAOI();
		}		

		/// <summary>
		/// Provides the behavior for the "Arrange" pass of Silverlight layout.
		/// Classes can override this method to define their own arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this
		/// object should use to arrange itself and its children.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			this.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
			return base.ArrangeOverride(finalSize);
		}
		#endregion

		#region Dependency Properties

		/// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(Map), typeof(OverviewMap), new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OverviewMap ovmap = d as OverviewMap;
			Map oldMap = e.OldValue as Map;
			if (oldMap != null) //clean up
			{
				if (ovmap.OVMapImage != null)
					ovmap.OVMapImage.Layers.Clear();
				oldMap.ExtentChanged -= ovmap.UpdateOVMap;
				oldMap.RotationChanged -= ovmap.map_RotationChanged;
			}
			Map newMap = e.NewValue as Map;
			if (newMap != null)
			{
				newMap.ExtentChanged += ovmap.UpdateOVMap;
				newMap.RotationChanged += ovmap.map_RotationChanged;
				if (ovmap.Layer != null && ovmap.OVMapImage != null)
					ovmap.OVMapImage.Layers.Add(ovmap.Layer);
				ovmap.UpdateOVMap();
			}			
		}

		private void map_RotationChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (AOI == null) return;
			double angle = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? -(double)e.NewValue : (double)e.NewValue;

			if (rotateTransform == null)
			{
				rotateTransform = new RotateTransform();
				AOI.RenderTransform = rotateTransform;
				AOI.RenderTransformOrigin = new Point(0.5, 0.5);
			}						
			rotateTransform.Angle = angle;			
		}

		/// <summary>
		/// Sets or gets the Map control associated with the OverviewMap.
		/// </summary>
		public Map Map
		{
			get { return (Map)GetValue(MapProperty); }
			set { SetValue(MapProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MaximumExtent"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MaximumExtentProperty = DependencyProperty.Register("MaximumExtent", typeof(Envelope), typeof(OverviewMap), new PropertyMetadata(OnMaximumExtentPropertyChanged));

		private static void OnMaximumExtentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OverviewMap ov = d as OverviewMap;
			Envelope newExtent = e.NewValue as Envelope;
			if (newExtent != null)
			{
				if (ov.OVMapImage == null || ov.OVMapImage.Layers == null) return;								
				ov.maxWidth = newExtent.Width;
				ov.maxHeight = newExtent.Height;				
				ov.UpdateOVMap();
				if (ov.IsStatic)
				{
					if (ov.isInitialized)
						ov.UpdateExtentToMaixumExtent();
					else
						ov.OVMapImage.Layers.LayersInitialized += (s, arg) => { ov.UpdateExtentToMaixumExtent(); };
				}
				else
				{
					if (ov.isInitialized)
					{
						ov.ZoomToNewExtent();
					}
				}
			}
			else
			{
				if (ov.isInitialized)
				{
					ov.fullExtent = ov.OVMapImage.Layers.GetFullExtent();
					if (ov.fullExtent != null)
					{
						ov.maxWidth = ov.fullExtent.Width;
						ov.maxHeight = ov.fullExtent.Height;
					}
					ov.UpdateOVMap();
				}
			}
		}

		private void UpdateExtentToMaixumExtent()
		{
			if(OVMapImage != null && isInitialized)
			{
				OVMapImage.Extent = MaximumExtent;
			}
		}

		/// <summary>
		/// Gets or sets  the maximum map extent of the overview map. 
		/// If undefined, the maximum extent is derived from the layer.
		/// </summary>
		/// <value>The maximum extent.</value>
		public Envelope MaximumExtent
		{
			get { return (Envelope)GetValue(MaximumExtentProperty); }
			set { SetValue(MaximumExtentProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="IsStatic"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsStaticProperty = DependencyProperty.Register("IsStatic", typeof(bool), typeof(OverviewMap), new PropertyMetadata(false, OnIsStaticPropertyChanged));

		/// <summary>
		/// Gets or sets a value indicating whether the overview map extent is static.
		/// </summary>
		/// <value>If true the extent of the overview map will never change. </value>
		/// <remarks>If true the extent will remain at <see cref="MaximumExtent"/>. If 
		/// MaximumExtent is not set the extent will remain at the full extent 
		/// of the layer.</remarks>
		public bool IsStatic
		{
			get { return (bool)GetValue(IsStaticProperty); }
			set { SetValue(IsStaticProperty, value); }
		}
		private static void OnIsStaticPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
				(d as OverviewMap).ZoomFullExtent();
			else
				(d as OverviewMap).ZoomToNewExtent();
			(d as OverviewMap).UpdateOVMap();
		}

		/// <summary>
		/// Identifies the <see cref="Layer"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerProperty = DependencyProperty.Register("Layer", typeof(Layer), typeof(OverviewMap), new PropertyMetadata(OnLayerPropertyChanged));

		private static void OnLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			OverviewMap ovmap = d as OverviewMap;
			if (ovmap.OVMapImage != null)
			{
				ovmap.OVMapImage.Layers.Clear();
				if (ovmap.Layer != null)
					ovmap.OVMapImage.Layers.Add(ovmap.Layer);
			}
			if (ovmap.Layer != null)
			{
				bool isInit = ovmap.Layer.IsInitialized;
				if (isInit)
					ovmap.Layer_LayersInitialized(ovmap.Layer, null);
				else
					ovmap.Layer.Initialized += ovmap.Layer_LayersInitialized;
			}
		}

		/// <summary>
		/// Gets or sets the layer used in the overview map.
		/// </summary>
		/// <value>The layer.</value>
		public Layer Layer
		{
			get { return (Layer)GetValue(LayerProperty); }
			set { SetValue(LayerProperty, value); }
		}

		#endregion		

		#region Private Methods

		/// <summary>
		/// Sets extents, limits, and events after layers have been initialized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void Layer_LayersInitialized(object sender, EventArgs args)
		{
			if (OVMapImage != null)
			{				
				if (MaximumExtent != null)
					fullExtent = MaximumExtent.Clone();
				else
					fullExtent = OVMapImage.Layers.GetFullExtent();
				
				OVMapImage.MinimumResolution = double.Epsilon;
				OVMapImage.MaximumResolution = double.MaxValue;
				if(fullExtent != null)
				{
					maxWidth = fullExtent.Width;
					maxHeight = fullExtent.Height;
				}
				UpdateOVMap();
			}
		}

		#region Methods for setting extent of OverviewMap

		/// <summary>
		/// Determines if the OverviewMap extent should be changed. If so, set new 
		/// extent and call ZoomTo or PanTo. If not, send to UpdateAOI
		/// </summary>
		private void UpdateOVMap()
		{
			if (Map == null || OVMapImage == null || OVMapImage.Extent == null || Map.Extent == null)
			{
				if(AOI!=null)
					AOI.Visibility = Visibility.Collapsed;
				return;
			}

			Envelope ovExtent = NormalizeExtent(OVMapImage.Extent);
			Envelope mapExtent = NormalizeExtent(Map.Extent);			

			// update ov extent if necessary
			double mapWidth = mapExtent.Width;
			double mapHeight = mapExtent.Height;
			double ovWidth = ovExtent.Width;
			double ovHeight = ovExtent.Height;
			
			bool sameWidthHeight = (mapWidth == lastMapExtent.Width && mapHeight == lastMapExtent.Height);

			if (mapExtent.Equals(lastMapExtent))
			{
				UpdateAOI();
			}
			else if (sameWidthHeight || IsStatic)
			{				
				double halfWidth = ovWidth / 2;
				double halfHeight = ovHeight / 2;
				MapPoint newCenter = mapExtent.GetCenter();
				
				if (MaximumExtent != null && !IsStatic)
				{
					if (newCenter.X - halfWidth < MaximumExtent.XMin) newCenter.X = MaximumExtent.XMin + halfWidth;
					if (newCenter.X + halfWidth > MaximumExtent.XMax) newCenter.X = MaximumExtent.XMax - halfWidth;
					if (newCenter.Y - halfHeight < MaximumExtent.YMin) newCenter.Y = MaximumExtent.YMin + halfHeight;
					if (newCenter.Y + halfHeight > MaximumExtent.YMax) newCenter.Y = MaximumExtent.YMax - halfHeight;
				}
				if (ovWidth >= maxWidth && !Map.WrapAroundIsActive)
					UpdateAOI();
				else
				{
					if (AOI != null) 
						AOI.Visibility = Visibility.Collapsed;

					if (NeedUpdate(newCenter,ovExtent.GetCenter(),OVMapImage.Resolution) && !IsStatic)
						OVMapImage.PanTo(newCenter);
					else
						UpdateAOI();
				}
			}
			else if (mapWidth >= maxWidth && !Map.WrapAroundIsActive)
				ZoomFullExtent();
			else
			{
				ZoomToNewExtent();
			}
		}

		private void ZoomToNewExtent()
		{
			if (OVMapImage == null || OVMapImage.Extent == null || 
				Map == null || Map.Extent == null) return;
			Envelope ovExtent = NormalizeExtent(OVMapImage.Extent);
			if (ovExtent == null) return;
			Envelope mapExtent = NormalizeExtent(Map.Extent);
			if (mapExtent == null) return;
			const double minRatio = 0.15;
			const double maxRatio = 0.8;
			double mapWidth = mapExtent.Width;
			double mapHeight = mapExtent.Height;			
			double ovWidth = ovExtent.Width;
			double ovHeight = ovExtent.Height;
			double widthRatio = mapWidth / ovWidth;
			double heightRatio = mapHeight / ovHeight;

			bool isMapWithinOV = mapExtent.XMin >= ovExtent.XMin && mapExtent.XMax <= ovExtent.XMax &&
								  mapExtent.YMin >= ovExtent.YMin && mapExtent.YMax <= ovExtent.YMax;

			Envelope extent;
			if (!isMapWithinOV || widthRatio <= minRatio || heightRatio <= minRatio || widthRatio >= maxRatio || heightRatio >= maxRatio)
			{
				//set new size around new mapextent
				if (AOI != null)
					AOI.Visibility = Visibility.Collapsed;
				if (maxWidth / 3 > mapWidth || Map.WrapAroundIsActive)
				{
					if (!IsStatic)
					{
						extent = new Envelope()
						{
							XMin = mapExtent.XMin - mapWidth,
							XMax = mapExtent.XMax + mapWidth,
							YMin = mapExtent.YMin - mapHeight,
							YMax = mapExtent.YMax + mapHeight
						};
						if (MaximumExtent != null)
						{
							if (extent.XMin < MaximumExtent.XMin) extent.XMin = MaximumExtent.XMin;
							if (extent.XMax > MaximumExtent.XMax) extent.XMax = MaximumExtent.XMax;
							if (extent.YMin < MaximumExtent.YMin) extent.YMin = MaximumExtent.YMin;
							if (extent.YMax > MaximumExtent.YMax) extent.YMax = MaximumExtent.YMax;
						}
						OVMapImage.ZoomTo(extent);
					}
					else
						UpdateAOI();
				}
				else
					ZoomFullExtent();
			}
			else
				UpdateAOI();
		}		

		/// <summary>
		/// Overload of UpdateOVMap - ExtentEventHandler version
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateOVMap(object sender, ESRI.ArcGIS.Client.ExtentEventArgs e)
		{
			UpdateOVMap();
		}

		private void ZoomFullExtent()
		{
			if (OVMapImage != null)
			{
				if (MaximumExtent != null)
					OVMapImage.ZoomTo(MaximumExtent);
				else
					OVMapImage.ZoomTo(fullExtent);
				UpdateAOI();
			}
		}

		#endregion

		#region Methods for setting size and position of AOI Box

		/// <summary>
		/// Sets size and position of AOI Box
		/// </summary>
		private void UpdateAOI()
		{
			if (Map == null || OVMapImage == null || OVMapImage.Extent == null || AOI == null) return;
						
			Envelope mapExtent = NormalizeExtent(Map.Extent);			

			if (mapExtent == null)
			{
				AOI.Visibility = Visibility.Collapsed;
				return;
			}
			MapPoint pt1 = new MapPoint(mapExtent.XMin, mapExtent.YMax);
			MapPoint pt2 = new MapPoint(mapExtent.XMax, mapExtent.YMin);
			Point topLeft = OVMapImage.MapToScreen(pt1);
			Point bottomRight = OVMapImage.MapToScreen(pt2);
			if (!double.IsNaN(topLeft.X) && !double.IsNaN(topLeft.Y) &&
				!double.IsNaN(bottomRight.X) && !double.IsNaN(bottomRight.Y))
			{
				// Get absolute value of (bottomRight.X - topLeft.X) to avoid negative width when the 
				// control FlowDirection is set to RTL:
				AOI.Width = Math.Max(3, Math.Abs(bottomRight.X - topLeft.X));
				AOI.Height = Math.Max(3, bottomRight.Y - topLeft.Y);
				// Setting the correct value for AOI width when the control FlowDirection is set to RTL:
				AOI.Margin = new Thickness((this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? topLeft.X
																				: topLeft.X - AOI.Width, topLeft.Y, 0, 0);
				// Set rotation of AOI according to map roatation
				if (rotateTransform == null)
				{
					rotateTransform = new RotateTransform();					
					AOI.RenderTransform = rotateTransform;
					AOI.RenderTransformOrigin = new Point(0.5, 0.5);
				}
				
				rotateTransform.Angle = (Map.Rotation * -1);
			
				AOI.Visibility = Visibility.Visible;
			}
			else
				AOI.Visibility = Visibility.Collapsed;
			lastMapExtent = mapExtent;
		}

		#endregion

		#region Method for setting extent of Map

		/// <summary>
		/// Set new map extent of main map control. Called after AOI
		/// Box has been repositioned by user
		/// </summary>
		private void UpdateMap()
		{
			if (AOI == null) return;
			mapExtent = Map.Extent;
			double aoiLeft = AOI.Margin.Left;
			double aoiTop = AOI.Margin.Top;
			MapPoint pt = OVMapImage.ScreenToMap(new Point(aoiLeft, aoiTop));
			double mapHalfWidth = mapExtent.Width / 2;
			double mapHalfHeight = mapExtent.Height / 2;
			MapPoint pnt = new MapPoint(pt.X + mapHalfWidth, pt.Y - mapHalfHeight);
			Map.PanTo(pnt);
		}

		#endregion

		#region AOI Box Mouse handlers
		private void AOI_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			dragOn = true;
			startPoint = e.GetPosition(this);
			offsetLeft = startPoint.X - AOI.Margin.Left;
			offsetTop = startPoint.Y - AOI.Margin.Top;
			AOI.MouseMove += AOI_MouseMove;
			AOI.MouseLeftButtonUp += AOI_MouseLeftButtonUp;
			AOI.CaptureMouse();
		}

		private void AOI_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (dragOn)
			{
				AOI.MouseMove -= AOI_MouseMove;
				AOI.MouseLeftButtonUp -= AOI_MouseLeftButtonUp;
				UpdateMap();
				dragOn = false;
				AOI.ReleaseMouseCapture();
			}
		}

		private void AOI_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragOn)
			{
				Point pos = e.GetPosition(this);
				AOI.Margin = new Thickness(pos.X - offsetLeft, pos.Y - offsetTop, 0, 0);
			}
		}

		#endregion

		#endregion

		// Checks to see if the base map pan has change enough that a pan is needed
		// for the overview map. A very small pan on the base map may cause a pan of less 
		// than 1 pixel on the overview map. if the pan is less than 1 pixel then 
		// then the UpdateAOI should be called because no pan less than 1 pixel is valid.
		bool NeedUpdate(MapPoint newCenter, MapPoint currentCenter, double resolution )
		{
			int x = (int)Math.Round((currentCenter.X - newCenter.X) / resolution);
			int y = (int)Math.Round((currentCenter.Y - newCenter.Y) / resolution);
			return (x != 0 || y != 0);				
		}

		// this function will check to see if the current extent needs to be normalized 
		// because wraping is turned on and maximum extent has been set.
		private Envelope NormalizeExtent(Envelope extent)
		{
			// If overmap has an MaximumExtent and the base map is panned into 
			// another frames then extent needs to be normalized in order to determine 
			// if the current extent of the base map on other frames is within 
			// the maximum extent. If there is no MaximumExtent or WrapAround is not
			// present in the overview map then there is no need to normalize extent.
			if (MaximumExtent != null && Map.WrapAroundIsActive)
			{
				Geometry.Geometry normExtent = Envelope.NormalizeCentralMeridian(extent);
				
				// if the entire extent is in another frame then an envelope will be returned.
				if (normExtent is Envelope)
					return normExtent as Envelope;

				// if the extent crosses the between two frames then half of the 
				// extent exists in one frame andthe other half exists in the 
				// other frame. Polygon with two rings is returned to reprsent each 
				// side of the extent in each frame.
				else if (normExtent is Polygon)
					return CreateDateLineExtent((Polygon)normExtent);				
			}
			// if normalizing is not needed then the extent will be 
			// returned unchanged.
			return extent;
		}

		// Merges two polygon rings into a single extent based on which width is 
		// bigger. e.g. if an extent crosses the dateline then the half that is most
		// visible should be represented on the overview map. 
		private static Envelope CreateDateLineExtent(Polygon polygon)
		{
			// left side
			Polygon ring1 = new Polygon();
			ring1.Rings.Add(polygon.Rings[0]);

			// right side
			Polygon ring2 = new Polygon();
			ring2.Rings.Add(polygon.Rings[1]);

			// if left side is bigger than right side. 
			// merge the right side width onto the left side.
			if (ring1.Extent.Width > ring2.Extent.Width)
			{
				ring1.Extent.XMax += ring2.Extent.Width;
				return ring1.Extent;
			}
			// if the right side is bigger than the left side.
			// merge the left side width onto the right side.
			else
			{
				ring2.Extent.XMin -= ring1.Extent.Width;
				return ring2.Extent;
			}
		}
	}
}
