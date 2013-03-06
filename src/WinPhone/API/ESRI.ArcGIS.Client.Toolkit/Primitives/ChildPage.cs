using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
	/// <summary>
	/// ChildPage control that simulates a page navigation without leaving the page.
	/// This is useful when you want to display details about the map in the entire 
	/// view, but don't want to navigate away from the map, which would unload the map
	/// state.
	/// </summary>
	[TemplatePart(Name = "Popup", Type = typeof(Popup))]
	public sealed class ChildPage : ContentControl
	{
		private static List<ChildPage> PageStack = new List<ChildPage>();
		Microsoft.Phone.Controls.PhoneApplicationPage page;
		Storyboard anim;
		DoubleAnimation da;
		Popup Popup;

		/// <summary>
		/// Initializes a new instance of the <see cref="ChildPage"/> class.
		/// </summary>
		public ChildPage()
		{
			DefaultStyleKey = typeof(ChildPage);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChildPage"/> class.
		/// </summary>
		/// <param name="parent">The parent page to react to back button and rotation on.</param>
		/// <remarks>
		/// Only use this constructor if you don't add the ChildPage to the page tree.</remarks>
		public ChildPage(Microsoft.Phone.Controls.PhoneApplicationPage parent) : this()
		{
			page = parent;
		}

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
			Popup = GetTemplateChild("Popup") as Popup;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the child page is visible or not.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="IsOpen"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(ChildPage), 
			new PropertyMetadata(false, OnIsOpenPropertyChanged));

		private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var page = d as ChildPage;
			if (page.IsOpen)
				page.Show();
			else
				page.Hide();
		}

		private void Show()
		{
			if (Popup != null && !Popup.IsOpen)
			{
				var page = GetPage();
				if (page != null)
				{
					var corner = this.TransformToVisual(page).Transform(new Point(0, 0));
					if (anim == null)
					{
						anim = new Storyboard();
						da = new DoubleAnimation();
						da.To = 0;
						da.EasingFunction = new System.Windows.Media.Animation.CircleEase() { EasingMode = EasingMode.EaseInOut };
						da.Duration = System.TimeSpan.FromMilliseconds(400);
						da.FillBehavior = FillBehavior.HoldEnd;
						anim.Children.Add(da);
						Storyboard.SetTarget(da, Popup);
						Storyboard.SetTargetProperty(da, new PropertyPath("HorizontalOffset"));
					}
					da.To = -corner.X;
					da.From = -corner.X + 800;
					page.BackKeyPress += page_BackKeyPress;
					page.SizeChanged += page_SizeChanged;
					Popup.VerticalOffset = -corner.Y;
					Popup.IsOpen = true;
					(Popup.Child as FrameworkElement).Width = page.ActualWidth;
					(Popup.Child as FrameworkElement).Height = page.ActualHeight;
					anim.Begin();
					PageStack.Add(this);
				}
			}
		}

		private void page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			(Popup.Child as FrameworkElement).Width = (sender as FrameworkElement).ActualWidth;
			(Popup.Child as FrameworkElement).Height = (sender as FrameworkElement).ActualHeight;
		}

		private void Hide()
		{
			if (Popup != null && Popup.IsOpen)
			{
				var page = GetPage();
				if (page != null)
				{
					page.BackKeyPress -= page_BackKeyPress;
					page.SizeChanged -= page_SizeChanged;
				}
				Popup.IsOpen = false;
				if(PageStack.Contains(this))
					PageStack.Remove(this);
			}
		}

		private void page_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (PageStack.Contains(this) && PageStack[PageStack.Count-1] == this && IsOpen)
			{
				IsOpen = false;
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Looks for the page that this control belongs to.
		/// </summary>
		/// <returns></returns>
		private Microsoft.Phone.Controls.PhoneApplicationPage GetPage()
		{
			if(page != null) return this.page;
			
			FrameworkElement parent = this.Parent as FrameworkElement;
			while (parent != null && !(parent is Microsoft.Phone.Controls.PhoneApplicationPage))
			{
				parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
			}
			return parent as Microsoft.Phone.Controls.PhoneApplicationPage;
		}
	}
}
