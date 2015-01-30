// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ESRI.ArcGIS.Client.Geometry;
using System.Windows.Media;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// Magnifying control for the <see cref="ESRI.ArcGIS.Client.Map"/> using a <see cref="TiledMapServiceLayer"/>.
	/// </summary>
	/// <remarks>
	/// If you require multiple layer or dynamic layers in your magnifier, 
	/// use the <see cref="Magnifier"/> control instead.
	/// </remarks>
	[TemplatePart(Name = "bigMap", Type = typeof(Map))]
	[System.Windows.Markup.ContentProperty("Layer")]
	public class MagnifyingGlass : Control
	{
		Map bigMap;
		Cursor cursor;
		double lastResolution = double.NaN;
		Point beginP;
		Point currentP;
		bool dragOn = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="MagnifyingGlass"/> class.
		/// </summary>
		public MagnifyingGlass()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(MagnifyingGlass);
#endif
			this.Opacity = 0.0;
			ZoomFactor = 5;
		}

		/// <summary>
		/// Static initialization for the <see cref="MagnifyingGlass"/> control.
		/// </summary>
		static MagnifyingGlass()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MagnifyingGlass),
				new FrameworkPropertyMetadata(typeof(MagnifyingGlass)));
#endif
		}
		/// <summary>
		/// Provides the behavior for the "Arrange" pass of Silverlight layout.
		/// Classes can override this method to define their own arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that
		/// this object should use to arrange itself and its children.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (this.Visibility == Visibility.Visible)
			{
				//Visibility has changed. Update extent
				Dispatcher.BeginInvoke((Action) delegate()
				{
					SetMagnifyResolution();
					UpdateMagnifyMapCenter();
				});
			}
			return base.ArrangeOverride(finalSize);
		}

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.Layer != null && bigMap != null)
			{
				bigMap.Layers.Remove(this.Layer);
			}
			bigMap = GetTemplateChild("bigMap") as Map;
			if (bigMap == null)
			{
				throw new ArgumentNullException(Properties.Resources.MagnifyingGlass_BigMapNotFoundInTemplate);
			}
			bigMap.Layers.LayersInitialized += Layers_LayersInitialized;
			if(Layer != null)
				bigMap.Layers.Add(Layer);

			bigMap.MinimumResolution = double.Epsilon;
			bigMap.MaximumResolution = double.MaxValue;

			this.MouseLeftButtonDown += MagnifyBox_MouseLeftButtonDown;
			this.MouseMove += MagnifyBox_MouseMove;
			this.MouseLeftButtonUp += MagnifyBox_MouseLeftButtonUp;
			this.Opacity = 1;
			if ((this.Visibility == Visibility.Visible) && Map != null)
			{
				Dispatcher.BeginInvoke((Action)delegate()
				{
					SetMagnifyResolution();					
					UpdateMagnifyMapCenter();
				});
			}
		}

		private void Layers_LayersInitialized(object sender, EventArgs args)
		{
			SetMagnifyResolution();
			UpdateMagnifyMapCenter();
		}

		private void MagnifyBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (dragOn)
			{
				this.ReleaseMouseCapture();
				dragOn = false;
			}
			this.Cursor = cursor;
		}

		private void MagnifyBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragOn)
			{
				currentP = e.GetPosition(null);
				double x0 = System.Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
				double y0 = System.Convert.ToDouble(this.GetValue(Canvas.TopProperty));
				ApplyTranslationTransform(currentP.X - beginP.X, currentP.Y - beginP.Y);

				beginP = currentP;
				UpdateMagnifyMapCenter();
			}
		}
		private void ApplyTranslationTransform(double x, double y)
		{
			if (this.FlowDirection == System.Windows.FlowDirection.RightToLeft)
				x = -x;
			Transform renderTransform = this.RenderTransform;
			TransformGroup group = renderTransform as TransformGroup;
			MatrixTransform transform2 = renderTransform as MatrixTransform;
			TranslateTransform transform3 = renderTransform as TranslateTransform;
			if (transform3 == null)
			{
				if (group != null)
				{
					if (group.Children.Count > 0)
					{
						transform3 = group.Children[group.Children.Count - 1] as TranslateTransform;
					}
					if (transform3 == null)
					{
						transform3 = new TranslateTransform();
						group.Children.Add(transform3);
					}
				}
				else
				{
					if (transform2 != null)
					{
						Matrix matrix = transform2.Matrix;
						matrix.OffsetX += x;
						matrix.OffsetY += y;
						MatrixTransform transform4 = new MatrixTransform();
						transform4.Matrix = matrix;
						this.RenderTransform = transform4;
						return;
					}
					TransformGroup group2 = new TransformGroup();
					transform3 = new TranslateTransform();
					if (renderTransform != null)
					{
						group2.Children.Add(renderTransform);
					}
					group2.Children.Add(transform3);
					this.RenderTransform = group2;
				}
			}
			transform3.X += x;
			transform3.Y += y;
		}


		private void MagnifyBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			cursor = this.Cursor;
			this.Cursor = Cursors.None;
			dragOn = true;
			beginP = e.GetPosition(null);
			this.CaptureMouse();
		}

		private void map_ExtentChanging(object sender, ExtentEventArgs e)
		{
			UpdateMagnifyMapCenter();
		}

		private void map_ExtentChanged(object sender, ExtentEventArgs e)
		{
			if (this.Map.Resolution != lastResolution)
				SetMagnifyResolution();
			UpdateMagnifyMapCenter();
		}

		private void SetMagnifyResolution()
		{
			if (this.Map == null || this.Map.Extent == null) return;
			double mapRes = this.Map.Resolution;
			double magRes = mapRes / ZoomFactor;
			if (bigMap != null && bigMap.Resolution != magRes)
			{
				if (bigMap.Extent == null)
				{
					MapPoint center = this.Map.Extent.GetCenter();
					bigMap.Extent = new Envelope(center.X - bigMap.ActualWidth * mapRes,
						center.Y - bigMap.ActualWidth * mapRes,
						center.X + bigMap.ActualWidth * mapRes,
						center.Y + bigMap.ActualWidth * mapRes);

				}
				bigMap.Rotation = Map.Rotation;
				bigMap.ZoomToResolution(magRes);
			}
		}

		private void UpdateMagnifyMapCenter()
		{
			if (this.Visibility == Visibility.Collapsed) return;
			if (bigMap != null && Map != null)
			{
				try
				{
					Point p = TransformToVisual(this.Map).Transform(new Point((this.RenderSize.Width * .5), (this.RenderSize.Height * .5)));
					MapPoint center = this.Map.ScreenToMap(p);
					if (center != null)
						bigMap.PanTo(center);
				}
				catch (ArgumentException) //Resizing elements at design time can cause errors
				{ 
				}
			}
		}


		/// <summary>
		/// Identifies the <see cref="Layer"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerProperty = DependencyProperty.Register("Layer", typeof(TiledMapServiceLayer), typeof(MagnifyingGlass), new PropertyMetadata(OnLayerPropertyChanged));

		private static void OnLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MagnifyingGlass mag = d as MagnifyingGlass;
			if (mag.bigMap != null)
			{
				mag.bigMap.Layers.Clear();
				if (mag.Layer != null)
					mag.bigMap.Layers.Add(mag.Layer);
			}
		}

		/// <summary>
		/// Gets or sets the layer used in the overview map.
		/// </summary>
		/// <value>The layer.</value>
		public TiledMapServiceLayer Layer
		{
			get { return (TiledMapServiceLayer)GetValue(LayerProperty); }
			set { SetValue(LayerProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(Map), typeof(MagnifyingGlass), new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MagnifyingGlass mag = d as MagnifyingGlass;
			Map old = e.OldValue as Map;
			if (old != null) //clean up
			{
				if (mag.bigMap != null)
					mag.bigMap.Layers.Clear();
				old.RotationChanged -= mag.map_RotationChanged;
				old.ExtentChanged -= mag.map_ExtentChanged;
				old.ExtentChanging -= mag.map_ExtentChanging;
			}
			Map newMap = e.NewValue as Map;
			if (newMap != null)
			{
				if (mag.bigMap != null)
				{
					if (mag.Layer != null && !mag.bigMap.Layers.Any()) // the layer may have laready b een added OnLayerChanegd or OnApplyTemplate events
						mag.bigMap.Layers.Add(mag.Layer);					
					mag.bigMap.Rotation = newMap.Rotation;
				}
				newMap.RotationChanged += mag.map_RotationChanged;
				newMap.ExtentChanged += mag.map_ExtentChanged;
				newMap.ExtentChanging += mag.map_ExtentChanging;
			}
		}

		private void map_RotationChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (bigMap == null) return;
			bigMap.Rotation = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? (double)e.NewValue : -(double)e.NewValue;
			UpdateMagnifyMapCenter();
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
		/// Identifies the <see cref="ZoomFactor"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(MagnifyingGlass), new PropertyMetadata(OnZoomFactorPropertyChanged));

		private static void OnZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MagnifyingGlass mag = d as MagnifyingGlass;
			mag.SetMagnifyResolution();
		}
		
		/// <summary>
		/// Gets or sets the zoom factor.
		/// </summary>
		public double ZoomFactor
		{
			get { return (double)GetValue(ZoomFactorProperty); }
			set { SetValue(ZoomFactorProperty, value);  }
		}
	}
}