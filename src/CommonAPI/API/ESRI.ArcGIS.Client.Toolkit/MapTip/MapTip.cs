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
using System.Collections.Generic;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// Graphics Layer MapTip control
	/// </summary>
	[TemplateVisualState(Name = "Collapsed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
	public class MapTip : Control
	{
		private System.Windows.Threading.DispatcherTimer timer;
		private TimeSpan maptipDelay = new TimeSpan(0, 0, 0, 0, 500);
		/// <summary>
		/// Initializes a new instance of the <see cref="MapTip"/> class.
		/// </summary>
		public MapTip()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(MapTip);
#endif
			this.Visibility = Visibility.Collapsed;
			HorizontalOffset = 20;
			VerticalOffset = 30;
			enterHandler = graphicsLayer_MouseEnter;
			leaveHandler = graphicsLayer_MouseLeave;
			moveHandler = graphicsLayer_MouseMove;
			timer = new System.Windows.Threading.DispatcherTimer()
			{
				Interval = maptipDelay
			};
			timer.Tick += new EventHandler(timer_Tick);
			this.MouseEnter += new MouseEventHandler(MapTip_MouseEnter);
			this.MouseLeave += new MouseEventHandler(MapTip_MouseLeave);
			this.MouseLeftButtonUp += new MouseButtonEventHandler(MapTip_MouseLeftButtonUp);
		}
		/// <summary>
		/// Static initialization for the <see cref="MapTip"/> control.
		/// </summary>
		static MapTip()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MapTip),
				new FrameworkPropertyMetadata(typeof(MapTip)));
#endif
		}
		private void MapTip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            if (!this.expanded)
            {
                this.expanded = true;
                ChangeVisualState(true);
            }
		}

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ChangeVisualState(false);
		}

		ESRI.ArcGIS.Client.GraphicsLayer.MouseEventHandler enterHandler;
		ESRI.ArcGIS.Client.GraphicsLayer.MouseEventHandler leaveHandler;
		ESRI.ArcGIS.Client.GraphicsLayer.MouseEventHandler moveHandler;
		private bool expanded = false;

		private void ChangeVisualState(bool useTransitions)
		{
			if (expanded)
			{
				GoToState(useTransitions, "Expanded");
			}
			else
			{
				GoToState(useTransitions, "Collapsed");
			}
		}

		private bool GoToState(bool useTransitions, string stateName)
		{
			return VisualStateManager.GoToState(this, stateName, useTransitions);
		}

		/// <summary>
		/// Expands the maptip
		/// </summary>
		/// <param name="useTransitions">if set to <c>true</c> will use transitions.</param>
		public void Expand(bool useTransitions)
		{
			expanded = true;
			ChangeVisualState(useTransitions);
		}

		/// <summary>
		/// Collapses the maptip.
		/// </summary>
		/// <param name="useTransitions">if set to <c>true</c> will use transitions.</param>
		public void Collapse(bool useTransitions)
		{
			expanded = false;
			ChangeVisualState(useTransitions);
		}

		#region Properties


		/// <summary>
		/// Identifies the <see cref="GraphicsLayer"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GraphicsLayerProperty = DependencyProperty.Register("GraphicsLayer", typeof(GraphicsLayer), typeof(MapTip), new PropertyMetadata(OnGraphicsLayerPropertyChanged));

		private static void OnGraphicsLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapTip maptip = d as MapTip;
			GraphicsLayer newLayer = e.NewValue as GraphicsLayer;
			GraphicsLayer oldLayer = e.OldValue as GraphicsLayer;
			if (oldLayer != null)
			{
				oldLayer.MouseEnter -= maptip.enterHandler;
				oldLayer.MouseLeave -= maptip.leaveHandler;
				oldLayer.MouseMove -= maptip.moveHandler;
			}
            maptip.Collapse(false);
            maptip.Visibility = Visibility.Collapsed;
            maptip.currentFeature = null;
            maptip.ItemsSource = null;
            maptip.DataContext = null;
			if (newLayer != null)
			{
				newLayer.MouseEnter += maptip.enterHandler;
				newLayer.MouseLeave += maptip.leaveHandler;
				newLayer.MouseMove += maptip.moveHandler;
			}
		}

		/// <summary>
		/// Gets or sets the graphics layer that the maptip is associated with.
		/// </summary>
		public ESRI.ArcGIS.Client.GraphicsLayer GraphicsLayer
		{
			get { return GetValue(GraphicsLayerProperty) as GraphicsLayer; }
			set { SetValue(GraphicsLayerProperty, value); }
		}

		

		/// <summary>
		/// Identifies the <see cref="TitleMember"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TitleMemberProperty = DependencyProperty.Register("TitleMember", typeof(string), typeof(MapTip), null);

		/// <summary>
		/// Gets or sets the Graphic Attribute Key used for setting the title 
		/// on the MapTip. This is overridden if the <see cref="Title"/> property is set.
		/// </summary>
		/// <value>The map tip title member.</value>
		public string TitleMember
		{
			get { return (string)GetValue(TitleMemberProperty); }
			set { SetValue(TitleMemberProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Title"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(MapTip), null);

		/// <summary>
		/// Gets or sets the title displayed in the MapTip. It can be a either a string or a FrameworkElement.
		/// It will override any settings set using <see cref="TitleMember"/>.
		/// </summary>
		/// <value>The map tip title.</value>
		public object Title
		{
			get { return (object)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="HorizontalOffset"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty HorizontalOffsetProperty = 
            DependencyProperty.Register("HorizontalOffset", typeof(int), typeof(MapTip), 
            new PropertyMetadata(0, OnHorizontalOffsetPropertyChanged));

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
              MapTip dp = d as MapTip;
              int dX = (int)e.NewValue - (int)e.OldValue;
              dp.SetValue(Canvas.LeftProperty, Canvas.GetLeft(dp) + dX);
        }

        /// <summary>
		/// Identifies the <see cref="VerticalOffset"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty VerticalOffsetProperty = 
            DependencyProperty.Register("VerticalOffset", typeof(int), typeof(MapTip), 
            new PropertyMetadata(0, OnVerticalOffsetPropertyChanged));

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
              MapTip dp = d as MapTip;
              int dY = (int)e.NewValue - (int)e.OldValue;
              dp.SetValue(Canvas.TopProperty, Canvas.GetTop(dp) + dY);
        }

		/// <summary>
		/// Gets or sets the horizontal offset.
		/// </summary>
		/// <value>The horizontal offset.</value>
		public int HorizontalOffset
		{
			get { return (int)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}
		/// <summary>
		/// Gets or sets the vertical offset.
		/// </summary>
		/// <value>The vertical offset.</value>
		public int VerticalOffset
		{
			get { return (int)GetValue(VerticalOffsetProperty); }
			set { SetValue(VerticalOffsetProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ItemsSource"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(System.Collections.IEnumerable), typeof(MapTip), null);
		/// <summary>
		/// Gets or sets the items source used for data binding.
		/// </summary>
		/// <value>The items source.</value>
		public System.Collections.IEnumerable ItemsSource
		{
			get
			{
				return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty);
			}
			private set
			{
				SetValue(ItemsSourceProperty, value);
			}
		}

		#endregion
		
		#region Mouse Graphic Events

		/// <summary>The current active feature</summary>
		private ESRI.ArcGIS.Client.Graphic currentFeature;
		/// <summary>True if the mouse is over either the graphic or the maptip</summary>
		private bool mouseIsOver = false;
		
		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			if (mouseIsOver)
			{
				Visibility = Visibility.Visible;
			}
			else
			{
				Visibility = Visibility.Collapsed;
			}
		}

		private void graphicsLayer_MouseMove(object sender, GraphicMouseEventArgs e)
		{
			//Recenter the maptip as long as the maptip isn't displaying yet
			if (Visibility == Visibility.Collapsed)
			{
				Point p = e.GetPosition(Parent as UIElement);
				SetValue(Canvas.LeftProperty, p.X + HorizontalOffset);
				SetValue(Canvas.TopProperty, p.Y + VerticalOffset);
				timer.Start();
			}
		}

		private void MapTip_MouseLeave(object sender, MouseEventArgs e)
		{
			mouseIsOver = false;
			if (timer.IsEnabled) { timer.Stop(); } //Cancel the show timer
			if (Visibility == Visibility.Visible)
			{
				timer.Start(); //Delay hiding maptip 
			}
		}

		private void MapTip_MouseEnter(object sender, MouseEventArgs e)
		{
			mouseIsOver = true;
			if (timer.IsEnabled) { timer.Stop(); } //Cancel the hide timer
			if (Visibility == Visibility.Collapsed)
			{
				timer.Start(); //Delay showing maptip
			}
		}

		private void graphicsLayer_MouseLeave(object sender, GraphicMouseEventArgs args)
		{
			if (timer.IsEnabled) { timer.Stop(); }
			if (Visibility == Visibility.Visible)
			{
				timer.Start(); //Delay hiding maptip 
			}
			mouseIsOver = false;
		}

		private void graphicsLayer_MouseEnter(object sender, GraphicMouseEventArgs args)
		{
			mouseIsOver = true;
			Graphic graphic = args.Graphic;
			if (currentFeature != graphic) //Mouse entered a new feature
			{
				this.expanded = false;
				currentFeature = graphic;
				Point p = args.GetPosition(Parent as UIElement);
				SetValue(Canvas.LeftProperty, p.X + HorizontalOffset);
				SetValue(Canvas.TopProperty, p.Y + VerticalOffset);
				this.DataContext = this.ItemsSource = graphic.Attributes;
				if (!string.IsNullOrEmpty(TitleMember))
				{
					object title = null;
					if (graphic.Attributes.ContainsKey(TitleMember))
						title = string.Format("{0}", graphic.Attributes[TitleMember]);
					else
					{
						string firstKey = null;
						foreach (string key in graphic.Attributes.Keys)
						{
							if (firstKey == null) firstKey = key;
							if (graphic.Attributes[key].GetType() == typeof(string))
							{
								title = graphic.Attributes[key] as string;
								break;
							}
						}
						if (title == null && !string.IsNullOrEmpty(firstKey))
							title = string.Format("{0}", graphic.Attributes[firstKey]);
					}
					this.Title = title;
				}
				ChangeVisualState(false);
				Visibility = Visibility.Collapsed;
			}
			if (Visibility == Visibility.Collapsed)
			{
				timer.Start(); //Delay showing maptip 
			}
		}
		
		#endregion
	}
}
