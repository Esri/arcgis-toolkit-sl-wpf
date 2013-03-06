// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client.Symbols;
using System.ComponentModel;
using System;
using System.Windows.Markup;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
	/// <summary>
 /// *FOR INTERNAL USE ONLY* A control that displays a symbol presenter scaled to fit.
 /// </summary>
 /// <exclude/>
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class SymbolDisplay : Control
	{
		DataBinder data = new DataBinder();
		private ContentPresenter ChildElement { get; set; }
		private ScaleTransform Scale { get; set; }
		SymbolPresenter presenter = new SymbolPresenter();
		static ControlTemplate _template;
        /// <summary>
        /// The symbol presenter
        /// </summary>
		public SymbolPresenter SymbolContent { get { return presenter; } }
		
        /// <summary>
        /// Constructor.
        /// </summary>
		public SymbolDisplay()
		{
			if (_template == null)
			{
				string temp = "<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><ContentPresenter x:Name=\"Child\" HorizontalAlignment=\"{TemplateBinding HorizontalAlignment}\" VerticalAlignment=\"{TemplateBinding VerticalAlignment}\"/></ControlTemplate>";
#if SILVERLIGHT
				_template = System.Windows.Markup.XamlReader.Load(temp) as ControlTemplate;
#else
				System.IO.MemoryStream templateStream = new System.IO.MemoryStream(System.Text.UTF8Encoding.Default.GetBytes(temp));
				_template = System.Windows.Markup.XamlReader.Load(templateStream) as ControlTemplate;
#endif
			}
			Template = _template;
			ApplyTemplate();
			base.IsTabStop = false;
		}

        /// <summary>
        /// Overrides OnApplyTemplate
        /// </summary>
		public override void OnApplyTemplate()
		{
			this.ChildElement = base.GetTemplateChild("Child") as ContentPresenter;
			this.ChildElement.RenderTransform = this.Scale = new ScaleTransform();
			this.ChildElement.Content = presenter;
			this.SymbolContent.DataContext = data;
		}

        /// <summary>
        /// The symbol to display
        /// </summary>
		public Symbol Symbol
		{
			get { return (Symbol)GetValue(SymbolProperty); }
			set { SetValue(SymbolProperty, value); }
		}

        /// <summary>
        /// Overrides MeasureOverride
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			Size size = new Size();
			if (this.ChildElement != null)
			{
				this.ChildElement.Measure((Symbol is MarkerSymbol)
					? new Size(double.PositiveInfinity, double.PositiveInfinity) : availableSize);
				Size desiredSize = this.ChildElement.DesiredSize;
				double factor = ComputeScaleFactor(availableSize, desiredSize);
				size.Width = desiredSize.Width * factor;
				size.Height = desiredSize.Height * factor;
			}
			return size;
		}

		private static double ComputeScaleFactor(Size availableSize, Size desiredSize)
		{
			if (desiredSize.Width == 0 || desiredSize.Height == 0)
				return 1;
			return Math.Min(1, Math.Min(availableSize.Width / desiredSize.Width,
									availableSize.Height / desiredSize.Height));
		}

        /// <summary>
        /// Overrides ArrangeOverride
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (this.ChildElement != null)
			{
				Size desiredSize = this.ChildElement.DesiredSize;
				double factor = ComputeScaleFactor(finalSize, desiredSize);
				this.Scale.ScaleX = factor;
				this.Scale.ScaleY = factor;
				Rect finalRect = new Rect(0.0, 0.0, desiredSize.Width, desiredSize.Height);
				this.ChildElement.Arrange(finalRect);
				finalSize.Width = factor * desiredSize.Width;
				finalSize.Height = factor * desiredSize.Height;
			}
			return finalSize;
		}

        /// <summary>
        /// Identifies the <see cref="Symbol"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
			DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SymbolDisplay), new PropertyMetadata(OnSymbolPropertyChanged));

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolDisplay obj = (SymbolDisplay)d;
            obj.SymbolContent.Symbol = obj.data.Symbol = e.NewValue as Symbol;
        }

        /// <summary>
        /// The attributes.  Required for data binding
        /// </summary>
		public Dictionary<string, object> Attributes
		{
			get { return (Dictionary<string, object>)GetValue(AttributesProperty); }
			set { SetValue(AttributesProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="Attributes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributesProperty =
			DependencyProperty.Register("Attributes", typeof(IDictionary<string, object>), typeof(SymbolDisplay), new PropertyMetadata(null, OnAttributesPropertyChanged));

		private static void OnAttributesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SymbolDisplay obj = (SymbolDisplay)d;
			obj.data.Attributes = (IDictionary<string, object>)e.NewValue;
		}

        /// <summary>
        /// The data binder.  Used for data binding.
        /// </summary>
		public class DataBinder : INotifyPropertyChanged
		{
            IDictionary<string, object> attributes;
            /// <summary>
            /// Attributes
            /// </summary>
			public IDictionary<string, object> Attributes
            {
                get { return attributes; }
                set
                {
                    attributes = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Attributes"));
                }
            }


            Symbol symbol;
            /// <summary>
            /// Symbol
            /// </summary>
			public Symbol Symbol {
                get { return symbol; }
                set
                {
                    symbol = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Symbol"));
                }
            }

            #region INotifyPropertyChanged Members
            /// <summary>
            /// Property changed event.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }
	}	
}