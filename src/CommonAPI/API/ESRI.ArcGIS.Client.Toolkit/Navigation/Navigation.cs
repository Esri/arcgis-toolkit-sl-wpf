// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using System.Linq;

namespace ESRI.ArcGIS.Client.Toolkit
{
    /// <summary>
    /// Navigation control supporting pan, zoom and rotation.
    /// </summary>
    /// <remarks>
    /// 	<para>The Navigation control contains a slider to zoom in and out, 
    /// 	interactive elements in a ring to rotate the map, and a a set of buttons 
    /// 	to zoom, pan, zoom to full extent, and reset rotation. The behavior 
    /// 	of the Navigation control at runtime depends on the content and 
    /// 	properties of the <see cref="Map"/> control to which it is bound.</para>
    /// 	<para>The zoom in\out buttons will zoom using the <see cref="P:Map.ZoomFactor"/>.
    /// 	The zoom factor must be greater than 1 for the zoom in\out buttons to 
    /// 	function properly.</para>
    /// 	<para>The zoom slider will only be displayed if the <see cref="P:Map.MinimumResolution" /> 
    /// 	and <see cref="P:Map.MaximumResolution"/> resolution on the Map control 
    /// 	have been defined. If a <see cref="TiledMapServiceLayer" /> is present 
    /// 	in the Map's layer collection, in most cases the minimum and maximum 
    /// 	resolution will be set for you. If a Map only contains <see cref="DynamicLayer" />,
    ///     you must set the minimum and maximum resolution explicitly. If 
    ///     <see cref="P:Map.SnapToLevels"/> is true and the Map contains a tiled 
    ///     map layer, the Map will zoom in\out only when the zoom slider bar 
    ///     represents a resolution closer to a different level of detail. This 
    ///     means small changes in the location of the slider bar may not cause 
    ///     the Map to zoom in\out. If <see cref="P:Map.SnapToLevels"/> is false 
    ///     (the default) any change to the slider bar will cause the Map to zoom
    ///     in\out.</para>
    /// </remarks>
    [TemplatePart(Name = "RotateRing", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PanLeft", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PanRight", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PanUp", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PanDown", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ZoomSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "TransformRotate", Type = typeof(RotateTransform))]
    [TemplatePart(Name = "ZoomFullExtent", Type = typeof(Button))]
    [TemplatePart(Name = "ResetRotation", Type = typeof(Button))]
    [TemplatePart(Name = "ZoomInButton", Type = typeof(Button))]
    [TemplatePart(Name = "ZoomOutButton", Type = typeof(Button))]
    public class Navigation : Control
    {
        #region private fields

        FrameworkElement RotateRing;
        FrameworkElement PanLeft;
        FrameworkElement PanRight;
        FrameworkElement PanUp;
        FrameworkElement PanDown;
        RotateTransform TransformRotate;
        Button ZoomFullExtent;
        Button ResetRotation;
        Button ZoomInButton;
        Button ZoomOutButton;
        Slider ZoomSlider;

        private double _panFactor = 0.5;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigation"/> class.
        /// </summary>
        public Navigation()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(Navigation);
#endif
        }
        /// <summary>
        /// Static initialization for the <see cref="Navigation"/> control.
        /// </summary>
        static Navigation()
        {
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Navigation),
				new FrameworkPropertyMetadata(typeof(Navigation)));
#endif
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RotateRing = GetTemplateChild("RotateRing") as FrameworkElement;
            TransformRotate = GetTemplateChild("TransformRotate") as RotateTransform;
            if (TransformRotate != null && Map != null)
                TransformRotate.Angle = Map.Rotation;
            PanLeft = GetTemplateChild("PanLeft") as FrameworkElement;
            PanRight = GetTemplateChild("PanRight") as FrameworkElement;
            PanUp = GetTemplateChild("PanUp") as FrameworkElement;
            PanDown = GetTemplateChild("PanDown") as FrameworkElement;
            ZoomSlider = GetTemplateChild("ZoomSlider") as Slider;
            ZoomFullExtent = GetTemplateChild("ZoomFullExtent") as Button;
            ResetRotation = GetTemplateChild("ResetRotation") as Button;
            ZoomInButton = GetTemplateChild("ZoomInButton") as Button;
            ZoomOutButton = GetTemplateChild("ZoomOutButton") as Button;

            enablePanElement(PanLeft);
            enablePanElement(PanRight);
            enablePanElement(PanUp);
            enablePanElement(PanDown);

            // Set control's flow direction to LTR to avoid flipping East and West keys:
            this.FlowDirection = System.Windows.FlowDirection.LeftToRight;

            if (ZoomSlider != null)
            {
                if (Map != null)
                {
                    SetupZoom();
                }
                ZoomSlider.Minimum = 0;
                ZoomSlider.Maximum = 1;
                ZoomSlider.SmallChange = .01;
                ZoomSlider.LargeChange = .1;
                ZoomSlider.LostMouseCapture += ZoomSlider_LostMouseCapture;
                ZoomSlider.LostFocus += ZoomSlider_LostMouseCapture;
            }
            if (ZoomInButton != null)
                ZoomInButton.Click += ZoomInButton_Click;
            if (ZoomOutButton != null)
                ZoomOutButton.Click += ZoomOutButton_Click;
            if (RotateRing != null)
            {
                RotateRing.MouseLeftButtonDown += RotateRing_MouseLeftButtonDown;
                RotateRing.LostMouseCapture += RotateRing_OnLostCapture;
            }
            if (ZoomFullExtent != null)
                ZoomFullExtent.Click += ZoomFullExtent_Click;
            if (ResetRotation != null)
                ResetRotation.Click += ResetRotation_Click;

            bool isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (isDesignMode)
                mouseOver = isDesignMode;
            ChangeVisualState(false);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            Map.Zoom(1 / Map.ZoomFactor);
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Map.Zoom(Map.ZoomFactor);
        }

        private void Map_ExtentChanged(object sender, ExtentEventArgs args)
        {
            if (!double.IsNaN(Map.Resolution) && ZoomSlider != null)
                ZoomSlider.Value = ResolutionToValue(Map.Resolution);
        }

        private void ResetRotation_Click(object sender, RoutedEventArgs e)
        {
            Storyboard s = new Storyboard();
            s.Duration = TimeSpan.FromMilliseconds(500);
            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            SplineDoubleKeyFrame spline = new SplineDoubleKeyFrame() { KeyTime = s.Duration.TimeSpan, Value = 0, KeySpline = new KeySpline() { ControlPoint1 = new System.Windows.Point(0, 0.1), ControlPoint2 = new System.Windows.Point(0.1, 1) } };
            anim.KeyFrames.Add(spline);
            spline.Value = 0;
            anim.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Rotation"));
            s.Children.Add(anim);
            Storyboard.SetTarget(anim, Map);
            s.Completed += (sender2, e2) =>
            {
                s.Stop();
                Map.Rotation = 0;
            };
            s.Begin();
        }

        private void ZoomFullExtent_Click(object sender, RoutedEventArgs e)
        {
            if (Map != null)
                Map.ZoomTo(Map.Layers.GetFullExtent());
        }

        #region State management

        private bool mouseOver = false;

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            mouseOver = false;
            if (!trackingRotation)
                ChangeVisualState(true);
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            mouseOver = true;
            ChangeVisualState(true);
            base.OnMouseEnter(e);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            if (mouseOver)
            {
                GoToState(useTransitions, "MouseOver");
            }
            else
            {
                GoToState(useTransitions, "Normal");
            }
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        #endregion

        #region Rotation

		Point startMousePos;
        private double angle = 0;
        private bool trackingRotation = false;

        private void RotateRing_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            startAppMouseTracking();
			startMousePos = e.GetPosition(this);
        }

        private void startAppMouseTracking()
        {
            trackingRotation = true;
            RotateRing.MouseMove += RotateRing_MouseMove;
            RotateRing.MouseLeftButtonUp += RotateRing_OnLostCapture;
            RotateRing.CaptureMouse();
        }

        private void stopAppMouseTracking()
        {
            RotateRing.ReleaseMouseCapture();
            RotateRing.MouseLeftButtonUp -= RotateRing_OnLostCapture;
            RotateRing.MouseMove -= RotateRing_MouseMove;
            trackingRotation = false;
            ChangeVisualState(true);
        }
		
		private void RotateRing_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.Map == null) return;
			FrameworkElement s = sender as FrameworkElement;
			if (TransformRotate == null || s == null) return;
			//Find the parent the TransformRotate is applied to:
			while (s.RenderTransform != TransformRotate)
			{
				s = s.Parent as FrameworkElement;
				if (s == null) return;
			}
			Point p = e.GetPosition(this);
			Point centerOfRotation = s.RenderTransformOrigin;
			centerOfRotation = new Point(s.ActualWidth * centerOfRotation.X, s.ActualHeight * centerOfRotation.Y);
			centerOfRotation = s.TransformToVisual(this).Transform(centerOfRotation);
			double previousAngle = GetAngle(centerOfRotation, startMousePos);
			double currentAngle = GetAngle(centerOfRotation, p);
			angle = this.Map.Rotation + (currentAngle - previousAngle);
			startMousePos = p;
			SetMapRotation(angle);
		}

		private double GetAngle(Point a, Point b)
		{
			if (a == null || b == null) return 0;
			return Math.Atan2((b.X - a.X), (a.Y - b.Y)) / Math.PI * 180;
		}

        private void RotateRing_OnLostCapture(object sender, EventArgs e)
        {
            stopAppMouseTracking();
        }

        private void SetMapRotation(double angle)
        {
            // In RightToLeft FlowDirection rotating the navigation control performs reverse rotation of the map, negating 
            // the angle value resolves this issue:
			if (TransformRotate != null)
				TransformRotate.Angle = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? angle : -angle;

            if (this.Map != null)
            {
                this.Map.Rotation = angle;
            }
        }

        #endregion

        #region Zoom

        private void ZoomSlider_LostMouseCapture(object sender, EventArgs e)
        {
            Map.ZoomToResolution(ValueToResolution(ZoomSlider.Value));
        }

        #endregion

        private void enablePanElement(FrameworkElement element)
        {
            if (element == null) return;
            element.MouseLeave += panElement_MouseLeftButtonUp;
            element.MouseLeftButtonDown += panElement_MouseLeftButtonDown;
            element.MouseLeftButtonUp += panElement_MouseLeftButtonUp;
        }

        private void panElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Map == null || sender == null) return;

            Envelope env = Map.Extent;
            if (env == null) return;
            double x = 0, y = 0;
            MapPoint oldCenter = env.GetCenter();
            MapPoint newCenter = null;
            var height = env.Height * _panFactor;
            var width = env.Width * _panFactor;
            // if units are degrees (the default), limit or alter panning to the lat/lon limits
            if (sender == PanUp) // North
            {
                y = oldCenter.Y + height;
                newCenter = new MapPoint(oldCenter.X, y);
            }
            else if (sender == PanRight) // East
            {
                x = oldCenter.X + width;
                newCenter = new MapPoint(x, oldCenter.Y);
            }
            else if (sender == PanLeft) // West
            {
                x = oldCenter.X - width;
                newCenter = new MapPoint(x, oldCenter.Y);
            }
            else if (sender == PanDown) // South
            {
                y = oldCenter.Y - height;
                newCenter = new MapPoint(oldCenter.X, y);
            }

            if (newCenter != null)
                Map.PanTo(newCenter);

        }

        private void panElement_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (Map == null) return;
        }

        /// <summary>
        /// Maps the slider values 0..1 to the map's resolution.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private double ValueToResolution(double value)
        {
            double max = Math.Log10(Map.MaximumResolution);
            double min = Math.Log10(Map.MinimumResolution);
            double resLog = (1 - value) * (max - min) + min;
            return Math.Pow(10, resLog);
        }
        /// <summary>
        /// Maps the map's resolution to a logirithmic scale between 
        /// 0 and 1 which is used on the slider.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        /// <returns></returns>
        private double ResolutionToValue(double resolution)
        {
            double max = Math.Log10(Map.MaximumResolution);
            double min = Math.Log10(Map.MinimumResolution);
            double value = 1 - ((Math.Log10(resolution) - min) / (max - min));
            return Math.Min(1, Math.Max(value, 0)); //cap values between 0..1
        }
        /// <summary>
        /// Sets up the parameters of the ZoomSlider
        /// </summary>
        public void SetupZoom()
        {

            if (ZoomSlider != null && Map != null)
            {
                if (!double.IsNaN(Map.MinimumResolution) && !double.IsNaN(Map.MaximumResolution) &&
                    Map.MaximumResolution != double.MaxValue &&
                    Map.MinimumResolution != double.Epsilon &&
                    !double.IsNaN(Map.Resolution))
                {
                    ZoomSlider.Value = ResolutionToValue(Map.Resolution);
                    ZoomSlider.Visibility = Visibility.Visible;
                }
                else
                {
                    bool isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);//Note: returns true in Blend but not VS
                    if (!isDesignMode)
                        ZoomSlider.Visibility = Visibility.Collapsed;
                }
            }
        }

        #region Properties

        /// <summary>
        /// Identifies the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(Map), typeof(Navigation), new PropertyMetadata(OnMapPropertyChanged));

        private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Navigation nav = d as Navigation;
            Map map = e.NewValue as Map;
            Map oldmap = e.OldValue as Map;

            if (oldmap != null)
            {
                oldmap.RotationChanged -= nav.Map_RotationChanged;
                oldmap.ExtentChanged -= nav.Map_ExtentChanged;
                oldmap.ExtentChanging -= nav.Map_ExtentChanged;
                if (oldmap.Layers != null)
                    oldmap.Layers.LayersInitialized -= nav.Layers_LayersInitialized;
            }
            if (map != null)
            {
                map.RotationChanged += nav.Map_RotationChanged;
                map.ExtentChanged += nav.Map_ExtentChanged;
                map.ExtentChanging += nav.Map_ExtentChanged;
                if (map.Layers != null)
                    map.Layers.LayersInitialized += nav.Layers_LayersInitialized;
                if (nav.TransformRotate != null)
                    nav.TransformRotate.Angle = map.Rotation;				
				nav.SetupZoom();
            }
        }

        private void Layers_LayersInitialized(object sender, EventArgs args)
        {
            SetupZoom();
        }

        /// <summary>
        /// Gets or sets the map that the scale bar is buddied to.
        /// </summary>
        public Map Map
        {
            get { return GetValue(MapProperty) as Map; }
            set { SetValue(MapProperty, value); }
        }

        /// <summary>
        ///  Factor used in panning map. The factor is used as a portion of current width and height of map extent. Default is 0.5.
        /// </summary>
        public Double PanFactor { get { return _panFactor; } set { _panFactor = value; } }

        private void Map_RotationChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // In RightToLeft FlowDirection rotating the navigation control performs reverse rotation of the map, negating 
            // the angle value while maintaining its original value in global variable of the class (angle):
            double value = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight) ? (double)e.NewValue : -(double)e.NewValue;
            if (TransformRotate != null && TransformRotate.Angle != value)
                TransformRotate.Angle = value;
            angle = (double)e.NewValue;
        }

        #endregion
    }
}
