// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// A Map ProgressBar control that automatically fades in and out when map loads tiles.
	/// </summary>
	[TemplatePart(Name = "Progress", Type = typeof(ProgressBar))]
	[TemplatePart(Name = "ValueText", Type = typeof(TextBlock))]
	[TemplateVisualState(Name = "Show", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Hide", GroupName = "CommonStates")]
	public class MapProgressBar : Control
	{
		private System.Windows.Controls.ProgressBar bar;
		private TextBlock text;
		private bool isVisible = false;
			
		/// <summary>
		/// Initializes a new instance of the <see cref="MapProgressBar"/> class.
		/// </summary>
		public MapProgressBar()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(MapProgressBar);
#endif
		}
		
		/// <summary>
		/// Static initialization for the <see cref="MapProgressBar"/> control.
		/// </summary>
		static MapProgressBar()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MapProgressBar), new FrameworkPropertyMetadata(typeof(MapProgressBar)));
#endif
		}

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			text = GetTemplateChild("ValueText") as TextBlock;
			bar = GetTemplateChild("Progress") as System.Windows.Controls.ProgressBar;
			bool isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
			if (isDesignMode)
			{
				isVisible = true;
				if (bar != null)
					bar.Value = 50;
				if (text != null)
					text.Text = string.Format(Properties.Resources.ProgressBar_ProgressPercentage, 50);
			}
			ChangeVisualState(false);
		}

		private void ChangeVisualState(bool useTransitions)
		{
			bool ok = false;
			if (isVisible || isVisible)
			{
				ok = GoToState(useTransitions, "Show");
			}
			else
			{
				ok = GoToState(useTransitions, "Hide");
			}
		}

		private bool GoToState(bool useTransitions, string stateName)
		{
			return VisualStateManager.GoToState(this, stateName, useTransitions);
		}

		/// <summary>
		/// Sets or gets the Map control associated with the <see cref="ProgressBar"/>.
		/// </summary>
		public ESRI.ArcGIS.Client.Map Map
		{
			get { return (ESRI.ArcGIS.Client.Map)GetValue(MapProperty); }
			set { SetValue(MapProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty =
			DependencyProperty.Register("Map", typeof(ESRI.ArcGIS.Client.Map), typeof(MapProgressBar), new PropertyMetadata(null, OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MapProgressBar mp = d as MapProgressBar;
			Map oldMap = e.OldValue as Map;
			Map newMap = e.NewValue as Map;
			if (oldMap != null)
			{
				oldMap.Progress -= mp.map_Progress;
			}
			if (newMap != null)
			{
				newMap.Progress += mp.map_Progress;
			}
		}

		private void map_Progress(object sender, ProgressEventArgs e)
		{
			if (bar != null)
				bar.Value = e.Progress;
			if (text != null)
				text.Text = string.Format(Properties.Resources.ProgressBar_ProgressPercentage, e.Progress);
			isVisible = (e.Progress < 99);
			ChangeVisualState(true);
		}

		/// <summary>
		/// Gets or sets the text brush.
		/// </summary>
		/// <value>The text brush.</value>
		public System.Windows.Media.Brush TextBrush
		{
			get { return (System.Windows.Media.Brush)GetValue(TextBrushProperty); }
			set { SetValue(TextBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="TextBrush"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TextBrushProperty =
			DependencyProperty.Register("TextBrush", typeof(System.Windows.Media.Brush), typeof(MapProgressBar), null);		
	}
}
