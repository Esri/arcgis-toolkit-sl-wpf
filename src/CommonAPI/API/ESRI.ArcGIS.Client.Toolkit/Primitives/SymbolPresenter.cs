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
using System.ComponentModel;
using System;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
 /// <summary>
 /// *FOR INTERNAL USE ONLY* A control that displays a symbol.
 /// </summary>
 /// <exclude/>
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class SymbolPresenter : Control
    {
        bool mouseOver = false;
        bool mouseDown = false;

        /// <summary>
        /// Overrides MeasureOverride
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Symbol is ESRI.ArcGIS.Client.Symbols.FillSymbol || Symbol is ESRI.ArcGIS.Client.Symbols.LineSymbol)
            {
                SetPath(availableSize);
                return base.MeasureOverride(availableSize);
            }
			else if (Symbol is ESRI.ArcGIS.Client.Symbols.MarkerSymbol)
			{
				// Set the margin to null else the desiredsize would be impacted by this margin
				// For example with a negative margin, the desired size is going towards 0 which can't be the right size of the swatch
				UIElement child = GetChild();

				if (child is FrameworkElement)
					((FrameworkElement)child).Margin = new Thickness(0.0);

				// Now measure without the margin
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Size size = child.DesiredSize;

				// Take care of the child transform to define the size of the swatch
				// For example with a scale of 2, the swatch size must be twice the desired size
				//             with a rotation, the swatch size must take care of this rotation
				Transform childTransform = child == null ? null : child.RenderTransform;
				if (childTransform != null)
				{
					Rect rect = childTransform.TransformBounds(new Rect(new Point(0, 0), size));
					size.Height = rect.Height;
					size.Width = rect.Width;
				}
				return size;
			}
			else return base.MeasureOverride(availableSize);
        }

		/// <summary>
		/// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
		/// <returns>
		/// The actual size that is used after the element is arranged in layout.
		/// </returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (Symbol is ESRI.ArcGIS.Client.Symbols.MarkerSymbol)
			{
				UIElement child = GetChild();
				Transform childTransform = child == null ? null : child.RenderTransform;

				if (childTransform != null)
				{
					// Arrange the symbol following it's desired size, the final size which is taking care of the transform is not the good one
					Size size = child.DesiredSize.Height == 0 && child.DesiredSize.Width == 0 ? finalSize : child.DesiredSize;
					child.Arrange(new Rect(new Point(0, 0), size));

					// Now we need to translate the symbol in order that the visual part of the symbol be in the snapshot
					Rect rect = childTransform.TransformBounds(new Rect(new Point(0, 0), child.DesiredSize)); // visual position of the symbol after transformation
					child.RenderTransformOrigin = new Point(0, 0); // for the swatch, the transform origin must be (0,0) whatever the transform origin used in the map
					if (rect.X != 0 || rect.Y != 0)
						RenderTransform = new TranslateTransform { X = -rect.X, Y = -rect.Y }; // Center the symbol

					return finalSize;
				}

			}
			return base.ArrangeOverride(finalSize);
		}

		private UIElement GetChild()
		{
			UIElement child = null;
			if (VisualTreeHelper.GetChildrenCount(this) > 0)
				child = VisualTreeHelper.GetChild(this, 0) as UIElement;

			return child;
		}

		private void SetPath(Size s)
		{
			FrameworkElement elm = GetTemplateChild("Element") as FrameworkElement;
			double width = Math.Min(s.Width, 35); //Maximum path size
			double height = Math.Min(s.Height, 35); //Maximum path size

			if (Symbol is ESRI.ArcGIS.Client.Symbols.LineSymbol && elm is Path)
			{
				Path p = elm as Path;
				PathGeometry geom = new PathGeometry();
				PathFigure figure = new PathFigure() { StartPoint = new Point(0, height / 2) };
				figure.Segments.Add(new LineSegment() { Point = new Point(width, height / 2) });
				geom.Figures.Add(figure);
				p.Data = geom;
			}
			else if (Symbol is ESRI.ArcGIS.Client.Symbols.FillSymbol)
			{
				if (elm is Path)
					(elm as Path).Data = new RectangleGeometry() { Rect = new Rect(0, 0, width, height) };
				else
				{
#if !WINDOWS_PHONE
					if(Symbol is FeatureService.Symbols.PictureFillSymbol)
					{
						var pfs = Symbol as FeatureService.Symbols.PictureFillSymbol;
						width = pfs.Width * 5;
						height = pfs.Height * 5;
					}
#endif
					elm.Width = width;
					elm.Height = height;
				}
			}
		}

        /// <summary>
        /// The symbol to display.
        /// </summary>
        public ESRI.ArcGIS.Client.Symbols.Symbol Symbol
        {
            get { return (ESRI.ArcGIS.Client.Symbols.Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Symbol"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
          DependencyProperty.Register("Symbol", typeof(ESRI.ArcGIS.Client.Symbols.Symbol), typeof(SymbolPresenter), new PropertyMetadata(OnSymbolPropertyChanged));

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolPresenter obj = (SymbolPresenter)d;
            if (e.OldValue != null)
                (e.OldValue as ESRI.ArcGIS.Client.Symbols.Symbol).PropertyChanged -= obj.symbol_PropertyChanged;
            ESRI.ArcGIS.Client.Symbols.Symbol newValue = (ESRI.ArcGIS.Client.Symbols.Symbol)e.NewValue;
            obj.Template = newValue.ControlTemplate;
            newValue.PropertyChanged += obj.symbol_PropertyChanged;
        }

        private void symbol_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ControlTemplate")
            {
                Template = Symbol.ControlTemplate;
            }
        }

        /// <summary>
        /// Overrides OnMouseEnter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            mouseOver = true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Overrides OnMouseLeftButtonDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            mouseDown = true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Overrides OnMouseLeftButtonUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            mouseDown = false;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Overrides OnMouseLeave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            mouseOver = false;
            mouseDown = false;
            ChangeVisualState(true);
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
            if (mouseDown)
            {
                GoToState(useTransitions, "Selected");
            }
            else
            {
                GoToState(useTransitions, "Unselected");
            }
        }

        private bool GoToState(bool useTransitions, string stateName)
        {
            return VisualStateManager.GoToState(this, stateName, useTransitions);
        }
    }
}