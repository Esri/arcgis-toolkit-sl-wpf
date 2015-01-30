using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Threading;
using ESRI.ArcGIS.Client.Toolkit.ValueConverters;
#if NET35
using Microsoft.Windows.Controls;
#endif

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
	/// <summary>
 /// *FOR INTERNAL USE ONLY* DateTimePicker is an internal primitive control used in the FeatureDataGrid 
	/// and FeatureDataForm for Editing DateTime field types.s
	/// </summary>
 /// <exclude/>
 [TemplatePart(Name = "Root", Type = typeof(Grid))]
	[TemplatePart(Name = "TextBox", Type = typeof(TextBox))]
	[TemplatePart(Name = "Button", Type = typeof(Button))]
	[TemplatePart(Name = "Popup", Type = typeof(Popup))]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DateTimePicker : Control
	{
		#region Private Members
		
		const string DEFAULT_DATETIME_FORMAT = "G";
		bool _userChangeDetected = false;
		Grid _root;
		TextBox _textbox;
		Button _button;
		Calendar _calendar;
		Popup _popup;
#if SILVERLIGHT
		Canvas _outsideCanvas, _outsidePopupCanvas;		
#endif
		
		#endregion Private members

		#region Constructor

		static DateTimePicker()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker),
				new FrameworkPropertyMetadata(typeof(DateTimePicker)));
#endif
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimePicker"/> class.
		/// </summary>
		public DateTimePicker()
		{
#if SILVERLIGHT
			this.DefaultStyleKey = typeof(DateTimePicker);
#endif
			base.IsEnabledChanged += DateTimePickerControl_IsEnabledChanged;

			_calendar = new Calendar();
			_calendar.IsTabStop = false;
			_calendar.SelectionMode = CalendarSelectionMode.SingleDate;
			_calendar.SelectedDatesChanged  += _calendar_SelectedDatesChanged;		
#if SILVERLIGHT
			_calendar.SizeChanged += _calendar_SizeChanged;			
#endif
		}		

		#endregion Constructor

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application 
		/// code or internal processes (such as a rebuilding layout pass) call 
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In 
		/// simplest terms, this means the method is called just before a UI 
		/// element displays in an application. For more information, see Remarks.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// Root
			_root = GetTemplateChild("Root") as Grid;

			// Textbox
			_textbox = GetTemplateChild("TextBox") as TextBox;
			if (_textbox != null)
			{
				_textbox.KeyDown += new KeyEventHandler(_textbox_KeyDown);
				_textbox.GotFocus += new RoutedEventHandler(_textbox_GotFocus);
				_textbox.LostFocus += new RoutedEventHandler(_textbox_LostFocus);
			}

			// Button
			_button = GetTemplateChild("Button") as Button;
			if (_button != null)
				_button.Click += _button_Click;			

			// Popup
			if (this._popup != null)
			{
				this._popup.Child = null;
			}
			this._popup = base.GetTemplateChild("Popup") as Popup;
			if (this._popup != null)
			{	
#if SILVERLIGHT
				if (this._outsideCanvas == null)
				{
					this._outsideCanvas = new Canvas();
					this._outsidePopupCanvas = new Canvas();
					this._outsidePopupCanvas.Background = new SolidColorBrush(Colors.Transparent);
					this._outsideCanvas.Children.Add(this._outsidePopupCanvas);
					this._outsideCanvas.Children.Add(this._calendar);
					this._outsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.OutsidePopupCanvas_MouseLeftButtonDown);
				}
				this._popup.Child = this._outsideCanvas;	
#else
                this._popup.Child = _calendar;
                this._popup.AllowsTransparency = true;
				this._popup.StaysOpen = false;
                if (_textbox != null)
                    this._popup.PlacementTarget = _textbox;
#endif
			}

			UpdateDisabledVisual();
			UpdateText();
		}		

		#endregion Overrides

		#region Public Members

		#region SelectedDate

		/// <summary>
		/// Gets or sets the selected date.
		/// </summary>		
		public DateTime? SelectedDate
		{
			get { return (DateTime?)GetValue(SelectedDateProperty); }
			set { SetValue(SelectedDateProperty, value); }
		}

		/// <summary>
		/// The dependency property for SelectedDate property.
		/// </summary>
		public static readonly DependencyProperty SelectedDateProperty =
			DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata(null, OnSelectedDatePropertyChanged));

		private static void OnSelectedDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dtp = d as DateTimePicker;
			if (dtp._calendar != null)
			{
				if (e.NewValue != null)
				{
					// Update the calendar control to show the same date as the date time picker control.
					dtp._calendar.DisplayDate = new DateTime(((DateTime)e.NewValue).Ticks, ((DateTime)e.NewValue).Kind);
					dtp._calendar.SelectedDate = new DateTime(((DateTime)e.NewValue).Ticks, ((DateTime)e.NewValue).Kind);
				}
				else
					dtp._calendar.SelectedDate = null;

				if (dtp.SelectedDateChanged != null)
					dtp.SelectedDateChanged(dtp, new SelectedDateEventArgs(e.OldValue as DateTime?, e.NewValue as DateTime?));
			}			
			dtp.UpdateText();			
		}

		#endregion SelectedDate

		#region DateTimeFormat

		/// <summary>
		/// Gets or sets the date time format that will be displayed for the SelectedDate property.
		/// </summary>		
		public string DateTimeFormat
		{
			get { return (string)GetValue(DateTimeFormatProperty); }
			set { SetValue(DateTimeFormatProperty, value); }
		}

		/// <summary>
		/// The dependency property used for DateTimeFormat.
		/// </summary>
		public static readonly DependencyProperty DateTimeFormatProperty =
			DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DateTimePicker), new PropertyMetadata(DEFAULT_DATETIME_FORMAT, OnDateTimeFormatPropertyChanged));

		private static void OnDateTimeFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dtp = d as DateTimePicker;		
			dtp.UpdateText();			
		}

		#endregion DateTimeFormat		

		#region DateTimeKind

		/// <summary>
		/// Gets or sets the DateTimeKind that will be used to creating new DateTime entries
		/// and will also be used to display the current SelectedDate.
		/// </summary>		
		public DateTimeKind DateTimeKind
		{
			get { return (DateTimeKind)GetValue(DateTimeKindProperty); }
			set { SetValue(DateTimeKindProperty, value); }
		}

		/// <summary>
		/// The dependency property used for the DateTimeKind property.
		/// </summary>
		public static readonly DependencyProperty DateTimeKindProperty =
			DependencyProperty.Register("DateTimeKind", typeof(DateTimeKind), typeof(DateTimePicker), new PropertyMetadata(DateTimeKind.Local, OnDateTimeKindPropertyChanged));

		private static void OnDateTimeKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dtp = d as DateTimePicker;
			dtp.UpdateText();			
		}

		#endregion DateTimeKind

		#endregion Public Members

		#region Private Events

		void _textbox_KeyDown(object sender, KeyEventArgs e)
		{
			_userChangeDetected = true;
			if (e.Key == Key.Enter)
			{
				SetText();
				// keep control in focus after textbox loses focus. this is 
				// important for FDG to prevent cell edit from ending.				
				this.Focus(); 
			}
		}

		void _textbox_LostFocus(object sender, RoutedEventArgs e)
		{
			SetText();
		}

		void _textbox_GotFocus(object sender, RoutedEventArgs e)
		{
			_textbox.SelectAll();
		}
#if SILVERLIGHT
		void _calendar_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SetPopUpPosition();
		}
#endif
		void _button_Click(object sender, RoutedEventArgs e)
		{
			_calendar.Language = Language;
			_popup.IsOpen = true;
#if SILVERLIGHT
			SetPopUpPosition();
#endif
		}
#if SILVERLIGHT
		private void OutsidePopupCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_popup.IsOpen = false;
		}
#endif
		void _calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{					
			if (_calendar != null && _popup != null && _popup.IsOpen)
			{
				_popup.IsOpen = false;
				_userChangeDetected = true;
				if(!_calendar.SelectedDate.HasValue)
					SelectedDate = null;
				else if (!SelectedDate.HasValue) 
					SelectedDate =  new DateTime(_calendar.SelectedDate.Value.Ticks, DateTimeKind);
				else
				{
					// keep existing time but override the date with the new calendar
					// selected date.
					DateTime time = new DateTime(SelectedDate.Value.Ticks);
					DateTime date = new DateTime(_calendar.SelectedDate.Value.Ticks);
					SelectedDate = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind);
				}
				// keep control in focus after calendar closes. this is important
				// for FDG to prevent cell edit from ending.
				this.Focus();
			}
		}

		void DateTimePickerControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateDisabledVisual();
		}

		#endregion Private Events

		#region Public Events

		/// <summary>
		/// Event that is raised when the SelectedDate property changes.
		/// </summary>
		public event EventHandler<SelectedDateEventArgs> SelectedDateChanged;

		/// <summary>
		/// Event arguments used for the SelectedDateChanged event.
		/// </summary>
		/// <exclude/>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public sealed class SelectedDateEventArgs : EventArgs
		{
			internal SelectedDateEventArgs(DateTime? oldValue, DateTime? newValue)
				: base()
			{
				this.OldValue = oldValue;
				this.NewValue = newValue;
			}
			/// <summary>
			/// Gets the old value that is being replaced.
			/// </summary>
			public DateTime? OldValue { get; private set; }

			/// <summary>
			/// Gets the new value that is replacing the old value.
			/// </summary>
			public DateTime? NewValue { get; private set; }
		}
		#endregion		

		#region Helper Methods

		private void SetText()		
		{
			// if empty string gets set to the textbox it may override original
			// date time value. To prevent losing data all valid changes will 
			// be flagged in order to validate textbox entries.
			if (!_userChangeDetected) return; // important security check do not delete
			_userChangeDetected = false;

			string text = _textbox.Text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				SelectedDate = null;
				return;
			}
			DateTime dt;
			if (DateTime.TryParse(text, new System.Globalization.CultureInfo(Language.IetfLanguageTag), System.Globalization.DateTimeStyles.None, out dt))
			{
				if (_calendar != null)
				{
					SelectedDate = new DateTime(dt.Ticks, DateTimeKind);
				}
			}
			else							
				UpdateText();
			
		}
#if SILVERLIGHT
		private void SetPopUpPosition()
		{	
			if (((this._calendar != null) && (Application.Current != null)) &&

			((Application.Current.Host != null) && (Application.Current.Host.Content != null)))
			{
				double actualHeight = Application.Current.Host.Content.ActualHeight;
				double actualWidth = Application.Current.Host.Content.ActualWidth;
#if !SILVERLIGHT4
				if (Application.Current.IsRunningOutOfBrowser)
				{
					Window window = Window.GetWindow(this);
					if ((window != null) && (window != Application.Current.MainWindow) && window.Content != null)
					{
						actualHeight = window.Content.ActualHeight;
						actualWidth = window.Content.ActualWidth;
					}
				}
#endif
				double num3 = this._calendar.ActualHeight;
				double num4 = base.ActualHeight;
				if (this._root != null)
				{
					GeneralTransform transform = this._root.TransformToVisual(null);
					if (transform != null)
					{
						Point point = new Point(0.0, 0.0);
						Point point2 = new Point(1.0, 0.0);
						Point point3 = new Point(0.0, 1.0);
						Point point4 = transform.Transform(point);
						Point point5 = transform.Transform(point2);
						Point point6 = transform.Transform(point3);
						double x = point4.X;
						double y = point4.Y;
						double num7 = x;
						double num8 = y + num4;
						if (actualHeight < (num8 + num3))
						{
							num8 = y - num3;
						}
						this._popup.HorizontalOffset = 0.0;
						this._popup.VerticalOffset = 0.0;
						this._outsidePopupCanvas.Width = actualWidth;
						this._outsidePopupCanvas.Height = actualHeight;
						this._calendar.HorizontalAlignment = HorizontalAlignment.Left;
						this._calendar.VerticalAlignment = VerticalAlignment.Top;
						Canvas.SetLeft(this._calendar, num7 - x);
						Canvas.SetTop(this._calendar, num8 - y);
						Matrix identity = Matrix.Identity;
						identity.M11 = point5.X - point4.X;
						identity.M12 = point5.Y - point4.Y;
						identity.M21 = point6.X - point4.X;
						identity.M22 = point6.Y - point4.Y;
						identity.OffsetX = point4.X;
						identity.OffsetY = point4.Y;
						MatrixTransform transform2 = new MatrixTransform();
						InvertMatrix(ref identity);
						transform2.Matrix = identity;
						this._outsidePopupCanvas.RenderTransform = transform2;
					}
				}
			}
		}

		private static bool InvertMatrix(ref Matrix matrix)
		{
			double num = (matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21);
			if (num == 0.0)
			{
				return false;
			}
			Matrix matrix2 = matrix;
			matrix.M11 = matrix2.M22 / num;
			matrix.M12 = (-1.0 * matrix2.M12) / num;
			matrix.M21 = (-1.0 * matrix2.M21) / num;
			matrix.M22 = matrix2.M11 / num;
			matrix.OffsetX = ((matrix2.OffsetY * matrix2.M21) - (matrix2.OffsetX * matrix2.M22)) / num;
			matrix.OffsetY = ((matrix2.OffsetX * matrix2.M12) - (matrix2.OffsetY * matrix2.M11)) / num;
			return true;
		}
#endif
		private void UpdateText()
		{
			if (_textbox != null)
			{
				if (!SelectedDate.HasValue)
					_textbox.Text = "";
				else
				{
					string date = DateTimeFormatConverter.DateTimeToString(SelectedDate, DateTimeKind, DateTimeFormat, new System.Globalization.CultureInfo(Language.IetfLanguageTag));
					// WARNING //////////////////////////////////////////////////////////////////////
					// Do not set empty string conversion to Textbox.Text doing so may override the//
 					// original value saving null to the selected date when that was not intended. //
					/////////////////////////////////////////////////////////////////////////////////
					if (!string.IsNullOrEmpty(date))
						_textbox.Text = date;
				}
			}
		}

		private void UpdateDisabledVisual()
		{
			if (!base.IsEnabled)						
				VisualStateManager.GoToState(this, "Disabled", true);							
			else
				VisualStateManager.GoToState(this, "Normal", true);							
		}


		#endregion Helper Methods
	}
}
