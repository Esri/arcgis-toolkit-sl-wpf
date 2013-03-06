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
	/// Map Magnifying glass
	/// </summary>
	public partial class Magnifier : UserControl
	{
		FrameworkElement rootElement;
		bool isActive;		

		/// <summary>
		/// Initializes a new instance of the <see cref="Magnifier"/> class.
		/// </summary>
		public Magnifier()
		{
			InitializeComponent();
			Layers = bigMap.Layers;
			ZoomFactor = 2;
			this.Cursor = Cursors.None;
			bool isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);//Note: returns true in Blend but not VS
			if(!isDesignMode)
				this.LayoutRoot.Visibility = Visibility.Collapsed;
			this.MouseLeftButtonUp += new MouseButtonEventHandler(Magnifier_MouseLeftButtonUp);
		}

		void Magnifier_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Enabled = false;
		}

		private void updateExtent(Envelope parentMapExtent)
		{			
			bigMap.Extent = parentMapExtent;
		}
		
		private void dragGlass(object sender, MouseEventArgs e)
		{			
			double x = e.GetPosition(null).X;
			double y = e.GetPosition(null).Y;

			Point offset = e.GetPosition(this);
			
			ApplyTranslationTransform(offset.X - 85,offset.Y -85);
			
			bigMap.SetValue(Canvas.LeftProperty, (- x * ZoomFactor) + 94 );
			bigMap.SetValue(Canvas.TopProperty,  (- y * ZoomFactor) + 95 );
		}

		private void ApplyTranslationTransform(double x, double y)
		{
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
			transform3.X = x;
			transform3.Y = y;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Magnifier"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get { return isActive; }
			set
			{
				if (value)
					StartDrag();
				else
				{
					StopDrag();
				}
			}
		}
		private void StopDrag()
		{
			isActive = false;
			this.LayoutRoot.Visibility = Visibility.Collapsed;
		}

        private void StartDrag()
		{
			isActive = true;
			this.LayoutRoot.Visibility = Visibility.Visible;			
			SetVisualSize();
			magGlass.Opacity = 1;
			bigScene.Opacity = 1;

			Dispatcher.BeginInvoke((Action)delegate() { updateExtent(Map.Extent); });
		}

		private void SetVisualSize()
		{			 		
			if (this.Visibility == Visibility.Collapsed) return;
			if (bigMap != null && Map != null)
			{

				bool DesignMode;
#if SILVERLIGHT 
				DesignMode = System.ComponentModel.DesignerProperties.IsInDesignTool;
#else
				var prop = System.ComponentModel.DesignerProperties.IsInDesignModeProperty;
				DesignMode = 
					(bool)System.ComponentModel.DependencyPropertyDescriptor.FromProperty(prop, 
					typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
				if(!DesignMode)
				{
					var RootVisual =
#if SILVERLIGHT
					Application.Current.RootVisual;
#else				
					GetRootVisual(Map);					
#endif
					// Get top left screen positon of base map after any scale 
					// transforms have been applied to the base map.				
					Point TopLeft = Map.TransformToVisual(RootVisual).Transform(new Point());
					Point BottomRight = Map.TransformToVisual(RootVisual).Transform(new Point(Map.ActualWidth, Map.ActualHeight));

					double left = TopLeft.X;
					double top = TopLeft.Y;
					double right = BottomRight.X;
					double bottom = BottomRight.Y;

					double width = (right - left) * ZoomFactor;
					double height = (bottom - top) * ZoomFactor;

					// offset the magnifiers map to align map top left corner 
					// with the base maps top left corner	
					//double left_margin = ((left * ZoomFactor) + Map.Margin.Left);
					//double top_margin = ((top * ZoomFactor) + Map.Margin.Top);

					double left_margin = (left * ZoomFactor);
					double top_margin = (top * ZoomFactor);

					bigMap.Margin = new Thickness(left_margin, top_margin, 0, 0);

					// set the width and height of the magnifiers map to the 
					// visual equal of the base maps height after any scale 
					// transform has been applied
					bigMap.Height = height;
					bigMap.Width = width;
				}
			}
		}
#if !SILVERLIGHT
		static Visual rootVisual;
		private static Visual GetRootVisual(UIElement element)
		{
			if (Application.Current != null && Application.Current.MainWindow != null)
			{
				if (element.IsDescendantOf(Application.Current.MainWindow))
					return Application.Current.MainWindow.Content as Visual;
			}
			if (rootVisual != null && element.IsDescendantOf(rootVisual))
				return rootVisual;
			DependencyObject dependencyObject = VisualTreeHelper.GetParent(element);
			if (dependencyObject == null)
				return element;
			while (VisualTreeHelper.GetParent(dependencyObject) != null)			
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			rootVisual = dependencyObject as Visual;
			return rootVisual;
		}
#endif
		/// <summary>
		/// Identifies the <see cref="Layers"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayersProperty = DependencyProperty.RegisterAttached("Layers", typeof(ESRI.ArcGIS.Client.LayerCollection), typeof(Magnifier), new PropertyMetadata(OnLayersPropertyChanged));
		/// <summary>
		/// Gets or sets the layers.
		/// </summary>
		public ESRI.ArcGIS.Client.LayerCollection Layers
		{
			get { return (ESRI.ArcGIS.Client.LayerCollection)GetValue(LayersProperty); }
			set { SetValue(LayersProperty, value); }
		}

		private static void OnLayersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Magnifier m = d as Magnifier;
			if (m.bigMap != null)
				m.bigMap.Layers = e.NewValue as LayerCollection;
			m.SetVisualSize();
		}

		/// <summary>
		/// Gets or sets the zoom factor.
		/// </summary>
		public double ZoomFactor { get; set; }

		/// <summary>
		/// Gets or sets the map that this magnifier should magnify on.
		/// </summary>
		/// <value>The map.</value>
		public Map Map
		{
			get { return (Map)GetValue(MapProperty); }
			set { SetValue(MapProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty =
			DependencyProperty.Register("Map", typeof(Map), typeof(Magnifier), new PropertyMetadata(null, OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Magnifier obj = (Magnifier)d;
			Map newValue = (Map)e.NewValue;
			Map oldValue = (Map)e.OldValue;
			if (oldValue != null)
			{
				oldValue.RotationChanged -= obj.Map_RotationChanged;
				obj.rootElement.MouseMove -= obj.dragGlass;
				obj.rootElement = null;
			}
			if (newValue != null)
			{
				newValue.RotationChanged += obj.Map_RotationChanged;				
				if(obj.bigMap != null)
					obj.bigMap.WrapAround = newValue.WrapAround;
				obj.rootElement = newValue.Parent as FrameworkElement;				
				if (obj.rootElement != null)
					obj.rootElement.MouseMove += obj.dragGlass;
				obj.SetVisualSize();
			}
		}

		private void Map_RotationChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (bigMap == null) return;
		    bigMap.Rotation = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? (double)e.NewValue : -(double)e.NewValue;			
		}
	}
}
