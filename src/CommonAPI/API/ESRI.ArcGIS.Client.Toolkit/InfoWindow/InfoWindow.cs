// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client.Geometry;
using System.Linq;

namespace ESRI.ArcGIS.Client.Toolkit
{
  /// <summary>
  /// Creates an instance of the InfoWindow that positions itself on top of the map
  /// </summary>
  [TemplatePart(Name = "BorderPath", Type = typeof(Path))]
  [TemplateVisualState(Name = "Show", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Hide", GroupName = "CommonStates")]
  public class InfoWindow : ContentControl
  {
    #region Private fields

    /// <summary>
    /// Describes how the InfoWindow will be displayed;    
    /// </summary>
    public enum PlacementMode
    {
      /// <summary>
      /// InfoWindow is placed on top of anchor location
      /// </summary>
      Top = 0,
      /// <summary>
      /// InfoWindow is placed to the left of anchor location
      /// </summary>
      Left = 1,
      /// <summary>
      /// InfoWindow is placed to the Right of anchor location
      /// </summary>
      Right = 2,
      /// <summary>
      /// InfoWindow is placed at the bottom of the anchor location
      /// </summary>
      Bottom = 3,
      /// <summary>
      /// InfoWindow is placed at the TopLeft of the anchor location
      /// </summary>      
      TopLeft = 4,
      /// <summary>
      /// InfoWindow is placed at the TopRight of the anchor location
      /// </summary>      
      TopRight = 5,
      /// <summary>
      /// InfoWindow is placed at the BottomLeft of the anchor location
      /// </summary>      
      BottomLeft = 6,
      /// <summary>
      /// InfoWindow is placed at the BottomRight of the anchor location
      /// </summary>      
      BottomRight = 7,
      /// <summary>
      /// InfoWindow is auto placed 
      /// </summary>      
      Auto = 8,
    }

    private const double ArrowHeight =
#if WINDOWS_PHONE
			20;
#else
 10;
#endif
    private TranslateTransform translate;
    private Path borderPath;
    private bool isDesignMode = false;
    private PathGeometry borderGeometry;
    private bool realizeGeometryScheduled;
    private PlacementMode currentPlacementMode;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoWindow"/> class.
    /// </summary>
    public InfoWindow()
    {
#if SILVERLIGHT
      DefaultStyleKey = typeof(InfoWindow);
#endif
      isDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);//Note: returns true in Blend but not VS
      Loaded += InfoWindow_Loaded;
    }

    private void InfoWindow_Loaded(object sender, RoutedEventArgs e)
    {
      CheckPosition();
    }

    /// <summary>
    /// Static initialization for the <see cref="InfoWindow"/> control.
    /// </summary>
    static InfoWindow()
    {
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoWindow), new FrameworkPropertyMetadata(typeof(InfoWindow)));
#endif
    }

    /// <summary>
    /// Provides the behavior for the Arrange pass of the layout.
    /// Classes can override this method to define their own Arrange pass 
    /// behavior.
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this
    /// object should use to arrange itself and its children.</param>
    /// <returns>
    /// The actual size that is used after the element is arranged in layout.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      var size = base.ArrangeOverride(finalSize);

      if (!realizeGeometryScheduled && BuildBorderPath(size, ArrowHeight, CornerRadius))
      {
        realizeGeometryScheduled = true;
        LayoutUpdated += OnLayoutUpdated;
      }

      if (Map == null && Anchor == null && !isDesignMode) return size;
      CheckPosition();
      if (!isDesignMode)
      {
        Point p2 = Map.MapToScreen(Anchor, true);
        Point p = Map.TransformToVisual(this.Parent as UIElement).Transform(p2);
        double sum_Height_CR_AH = size.Height + CornerRadius + ArrowHeight;
        switch (currentPlacementMode)
        {
          case PlacementMode.Top:
            translate.X = p.X - size.Width * .5;
            translate.Y = p.Y - size.Height - ArrowHeight - CornerRadius;
            RenderTransformOrigin = new Point(0.5, sum_Height_CR_AH / size.Height);
            break;
          case PlacementMode.Left:
            translate.X = p.X - size.Width - CornerRadius - ArrowHeight;
            translate.Y = p.Y - size.Height * .5;
            RenderTransformOrigin = new Point((size.Width + CornerRadius + ArrowHeight) / size.Width, 0.5);
            break;
          case PlacementMode.Right:
            translate.X = p.X + ArrowHeight + CornerRadius;
            translate.Y = p.Y - size.Height * .5;
            RenderTransformOrigin = new Point(-(CornerRadius / size.Width), 0.5);
            break;
          case PlacementMode.Bottom:
            translate.X = p.X - (size.Width * .5);
            translate.Y = p.Y + ArrowHeight + CornerRadius;
            RenderTransformOrigin = new Point(0.5, -(CornerRadius / size.Height));
            break;
          case PlacementMode.BottomRight:
            translate.X = p.X;
            translate.Y = p.Y + ArrowHeight + CornerRadius;
            RenderTransformOrigin = new Point(0, -(CornerRadius / size.Height));
            break;
          case PlacementMode.TopRight:
            translate.X = p.X;
            translate.Y = p.Y - (size.Height + ArrowHeight + CornerRadius);
            RenderTransformOrigin = new Point(0, sum_Height_CR_AH / size.Height);
            break;
          case PlacementMode.TopLeft:
            translate.X = p.X - (size.Width);
            translate.Y = p.Y - (size.Height + ArrowHeight + CornerRadius);
            RenderTransformOrigin = new Point(1, sum_Height_CR_AH / size.Height);
            break;
          case PlacementMode.BottomLeft:
            translate.X = p.X - (size.Width);
            translate.Y = p.Y + (ArrowHeight + CornerRadius);
            RenderTransformOrigin = new Point(1, -(CornerRadius / size.Height));
            break;
        }

      }
      return size;
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
      realizeGeometryScheduled = false;
      LayoutUpdated -= OnLayoutUpdated;
      borderPath.Data = borderGeometry;
      borderPath.Margin = new Thickness(0, 0, -CornerRadius * 2 - StrokeThickness - ArrowHeight, -CornerRadius * 2 - ArrowHeight - StrokeThickness);
    }

    //This method will determine where the placement of InfoWindow should be based on location of the point.
    private PlacementMode FindPlacement(Size size, double arrowSize, double cornerRadius, Point pT)
    {
      double sumArrowCR = arrowSize + cornerRadius;

      Point translatedTop = new Point(pT.X - size.Width * .5, pT.Y - (size.Height + sumArrowCR)); //Top
      Point translatedLeft = new Point(pT.X - (size.Width + sumArrowCR), pT.Y - size.Height * .5); //Left
      Point translatedRight = new Point(pT.X + sumArrowCR, pT.Y - size.Height * .5);//Right
      Point translatedBottom = new Point(pT.X - (size.Width * .5), pT.Y + sumArrowCR);  //Bottom
      Point translatedTopLeft = new Point(pT.X, pT.Y + sumArrowCR); //TopLeft
      Point translatedTopRight = new Point(pT.X - size.Width, pT.Y + sumArrowCR); //TopRight
      Point translatedBottomLeft = new Point(pT.X, pT.Y - (size.Height + sumArrowCR));//BottomLeft
      Point translatedBottomRight = new Point(pT.X - size.Width, pT.Y - (size.Height + sumArrowCR));  //BottomRight

      if (((Map.ActualWidth - translatedTop.X) < Map.ActualWidth) && ((Map.ActualWidth - translatedTop.X) > size.Width) &&
       ((Map.ActualHeight - translatedTop.Y) < Map.ActualHeight) && ((Map.ActualHeight - translatedTop.Y) > size.Height))
      {
        return PlacementMode.Top;
      }

      if (((Map.ActualWidth - translatedBottom.X) < Map.ActualWidth) && ((Map.ActualWidth - translatedBottom.X) > size.Width) &&
        ((Map.ActualHeight - translatedBottom.Y) < Map.ActualHeight) && ((Map.ActualHeight - translatedBottom.Y) > size.Height))
      {
        return PlacementMode.Bottom;
      }

      if (((Map.ActualWidth - translatedLeft.X) < Map.ActualWidth) && ((Map.ActualWidth - translatedLeft.X) > size.Width) &&
       ((Map.ActualHeight - translatedLeft.Y) < Map.ActualHeight) && ((Map.ActualHeight - translatedLeft.Y) > size.Height))
      {
        return PlacementMode.Left;
      }

      if (((Map.ActualWidth - translatedRight.X) < Map.ActualWidth) && ((Map.ActualWidth - translatedRight.X) > size.Width) &&
       ((Map.ActualHeight - translatedRight.Y) < Map.ActualHeight) && ((Map.ActualHeight - translatedRight.Y) > size.Height))
      {
        return PlacementMode.Right;
      }

      if (((Map.ActualWidth - translatedTopLeft.X) <= Map.ActualWidth) && ((Map.ActualWidth - translatedTopLeft.X) > size.Width) &&
      ((Map.ActualHeight - translatedTopLeft.Y) <= Map.ActualHeight) && ((Map.ActualHeight - translatedTopLeft.Y) > size.Height))
      {
        return PlacementMode.BottomRight;
      }

      if (((Map.ActualWidth - translatedTopRight.X) <= Map.ActualWidth) && ((Map.ActualWidth - translatedTopRight.X) > size.Width) &&
        ((Map.ActualHeight - translatedTopRight.Y) <= Map.ActualHeight) && ((Map.ActualHeight - translatedTopRight.Y) > size.Height))
      {
        return PlacementMode.BottomLeft;
      }

      if (((Map.ActualWidth - translatedBottomLeft.X) <= Map.ActualWidth) && ((Map.ActualWidth - translatedBottomLeft.X) > size.Width) &&
        ((Map.ActualHeight - translatedBottomLeft.Y) <= Map.ActualHeight) && ((Map.ActualHeight - translatedBottomLeft.Y) > size.Height))
      {
        return PlacementMode.TopRight;
      }

      if (((Map.ActualWidth - translatedBottomRight.X) <= Map.ActualWidth) && ((Map.ActualWidth - translatedBottomRight.X) > size.Width) &&
        ((Map.ActualHeight - translatedBottomRight.Y) <= Map.ActualHeight) && ((Map.ActualHeight - translatedBottomRight.Y) > size.Height))
      {
        return PlacementMode.TopLeft;
      }
      //if doesn't satisfy any condition default to Top
      return PlacementMode.Top;
    }

    private bool BuildBorderPath(Size size, double arrowSize, double cornerRadius)
    {
      Point pT = new Point(0, 0);
      if (Map != null)
      {
        Point p2 = Map.MapToScreen(Anchor, true);
        pT = Map.TransformToVisual(this.Parent as UIElement).Transform(p2);
      }
      else
      { return false; }
      
      if (Placement != PlacementMode.Auto)
      {
        currentPlacementMode = Placement;
      }
      else //auto
      {
        currentPlacementMode = FindPlacement(size, arrowSize, cornerRadius, pT);
      }

      //Start building the path
      PathGeometry pg = new PathGeometry();
      PathFigure p = new PathFigure();

      double sumArrowCR = arrowSize + cornerRadius;
      double sumHeightCR = size.Height + cornerRadius;
      double sumWidthCR = size.Width + cornerRadius;
      double halfWidth = size.Width * .5;
      double halfArrow = arrowSize * .5;
      double halfHeight = size.Height * .5;

      if (currentPlacementMode == PlacementMode.Top || currentPlacementMode == PlacementMode.TopLeft || currentPlacementMode == PlacementMode.TopRight)
      {

        p.StartPoint = new Point(0, -cornerRadius);

        p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) }); //Top line
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(sumWidthCR, 0)
        }); //UR

        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, size.Height) }); //Right side
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(size.Width, sumHeightCR)
        }); //BR

        //Miter
        if (currentPlacementMode == PlacementMode.Top)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth + halfArrow, sumHeightCR) }); //Bottom line, Right of miter
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth, size.Height + sumArrowCR) }); //Right side of Miter down
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth - halfArrow, sumHeightCR) }); //Left side of Miter up
          p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); //line after miter
        }

        if (currentPlacementMode == PlacementMode.TopLeft)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width - halfArrow, sumHeightCR) });//line before Mitter line 1
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width, size.Height + sumArrowCR) });//Mitter line 1
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width - arrowSize * 2, sumHeightCR) }); //Mitter line 2
          p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); //line after mitter
        }

        if (currentPlacementMode == PlacementMode.TopRight)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(arrowSize * 2, sumHeightCR) });//bottom line
          p.Segments.Add(new LineSegment() { Point = new Point(0, size.Height + sumArrowCR) }); //Miter line 1
          p.Segments.Add(new LineSegment() { Point = new Point(halfArrow, sumHeightCR) }); //Mitter line 2
          p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); // line after Mitter line 2
        }

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(-cornerRadius, size.Height)
        }); //LL

        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, 0) }); //Left side
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(0, -cornerRadius)
        }); //UR
      }
      if (currentPlacementMode == PlacementMode.Left)
      {

        p.StartPoint = new Point(0, -cornerRadius);
        p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) }); //Top line
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(sumWidthCR, 0)
        }); //UR

        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, halfHeight - halfArrow) }); //Left of mitter
        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR + arrowSize, halfHeight) }); //mitter up
        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, halfHeight + halfArrow) }); //mitter down
        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, size.Height) }); //right of mitter

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(size.Width, sumHeightCR)
        }); //BR Arc

        p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); //bottom line		

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(-cornerRadius, size.Height)
        }); //BL Arc

        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, 0) });//Left line

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(0, -cornerRadius)
        }); //TL Arc
      }
      if (currentPlacementMode == PlacementMode.Right)
      {

        p.StartPoint = new Point(0, -cornerRadius);
        p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) }); //Top line
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(sumWidthCR, 0)
        }); //UR

        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, size.Height) }); //Right side
        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(size.Width, sumHeightCR)
        }); //LR


        p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); //bottom line		

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(-cornerRadius, size.Height)
        }); //BL Arc

        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, halfArrow + halfHeight) });//Left up b4 mitter
        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius - arrowSize, halfHeight) });// mitter line up
        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, halfHeight - halfArrow) });// mitter line down
        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, 0) });// line up after mitter

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(0, -cornerRadius)
        }); //TL Arc
      }
      if (currentPlacementMode == PlacementMode.Bottom || currentPlacementMode == PlacementMode.BottomLeft || currentPlacementMode == PlacementMode.BottomRight)
      {
        p.StartPoint = new Point(0, -cornerRadius);

        if (currentPlacementMode == PlacementMode.Bottom)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth - halfArrow, -cornerRadius) }); //top line, before miter
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth, -cornerRadius - arrowSize) }); //mitter up 
          p.Segments.Add(new LineSegment() { Point = new Point(halfWidth + halfArrow, -cornerRadius) }); //Miter down
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) }); //line after miter
        }

        if (currentPlacementMode == PlacementMode.BottomRight)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(halfArrow, -cornerRadius) });//Line before Mitter
          p.Segments.Add(new LineSegment() { Point = new Point(0, -sumArrowCR) }); //Miter line 1
          p.Segments.Add(new LineSegment() { Point = new Point(arrowSize * 2, -cornerRadius) });//Mitter line 2
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) });//line after miter
        }

        if (currentPlacementMode == PlacementMode.BottomLeft)
        {
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width - (arrowSize * 2), -CornerRadius) }); //Line before Mitter		
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -sumArrowCR) }); //Mitter line 1
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width - halfArrow, -cornerRadius) });//Mitter line 2
          p.Segments.Add(new LineSegment() { Point = new Point(size.Width, -cornerRadius) });//line after mitter
        }

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(sumWidthCR, 0)
        }); //TR Arc

        p.Segments.Add(new LineSegment() { Point = new Point(sumWidthCR, size.Height) });//right line

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(size.Width, sumHeightCR)
        }); //BR Arc

        p.Segments.Add(new LineSegment() { Point = new Point(0, sumHeightCR) }); //bottom line		

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(-cornerRadius, size.Height)
        }); //BL Arc

        p.Segments.Add(new LineSegment() { Point = new Point(-cornerRadius, 0) });//Left line

        p.Segments.Add(new ArcSegment()
        {
          Size = new Size(cornerRadius, cornerRadius),
          SweepDirection = System.Windows.Media.SweepDirection.Clockwise,
          Point = new Point(0, -cornerRadius)
        }); //TL Arc
      }
      pg.Figures.Add(p);
      borderGeometry = pg;
      return true;
    }

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
      borderPath = GetTemplateChild("BorderPath") as Path;
      RenderTransform = translate = new TranslateTransform();
      InvalidateArrange();
      ChangeVisualState(false);
    }

    private void ChangeVisualState(bool useTransitions)
    {
      InvalidateMeasure();
      if (IsOpen && (Anchor != null && Map != null || isDesignMode))
        GoToState(useTransitions, "Show");
      else
        GoToState(useTransitions, "Hide");
    }

    private bool GoToState(bool useTransitions, string stateName)
    {
      return VisualStateManager.GoToState(this, stateName, useTransitions);
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the InfoWindow is open.
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
      DependencyProperty.Register("IsOpen", typeof(bool), typeof(InfoWindow), new PropertyMetadata(false, OnIsOpenPropertyChanged));

    private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      InfoWindow obj = (InfoWindow)d;
      obj.ChangeVisualState(true);
    }

    /// <summary>
    /// Gets or sets the map.
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
      DependencyProperty.Register("Map", typeof(Map), typeof(InfoWindow), new PropertyMetadata(OnMapPropertyChanged));

    private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      InfoWindow obj = (InfoWindow)d;
      Map newValue = (Map)e.NewValue;
      Map oldValue = (Map)e.OldValue;
      if (oldValue != null)
      {
				oldValue.RotationChanged -= obj.map_RotationChanged;
        oldValue.ExtentChanging -= obj.map_ExtentChanging;
        oldValue.ExtentChanged -= obj.map_ExtentChanging;
      }
      if (newValue != null)
      {
				newValue.RotationChanged += obj.map_RotationChanged;
        newValue.ExtentChanging += obj.map_ExtentChanging;
        newValue.ExtentChanged += obj.map_ExtentChanging;
      }
      obj.CheckPosition();
      obj.InvalidateArrange();
    }

    /// <summary>
    /// Gets or sets the anchor point.
    /// </summary>
    /// <value>The anchor point.</value>
    public MapPoint Anchor
    {
      get { return (MapPoint)GetValue(AnchorProperty); }
      set { SetValue(AnchorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Anchor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnchorProperty =
      DependencyProperty.Register("Anchor", typeof(MapPoint), typeof(InfoWindow), new PropertyMetadata(OnAnchorPropertyChanged));

    private static void OnAnchorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      InfoWindow obj = (InfoWindow)d;
      obj.CheckPosition();
      obj.InvalidateArrange();
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>The corner radius.</value>
    public double CornerRadius
    {
      get { return (double)GetValue(CornerRadiusProperty); }
      set { SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
      DependencyProperty.Register("CornerRadius", typeof(double), typeof(InfoWindow), new PropertyMetadata(5d, OnCornerRadiusPropertyChanged));

    private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (((double)e.NewValue) < 0)
        throw new ArgumentOutOfRangeException("CornerRadius");
      (d as InfoWindow).InvalidateArrange();
    }

    ///<summary>
    /// Sets the Placement of the InfoWindow
    ///</summary>
    ///<value>Top,Left,Right,Bottom, Auto</value>

    public PlacementMode Placement
    {
      get { return (PlacementMode)GetValue(PlacementProperty); }
      set { SetValue(PlacementProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Placement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PlacementProperty =
      DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(InfoWindow), new PropertyMetadata(PlacementMode.Top, OnPlacementPropertyChanged));


    private static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as InfoWindow).InvalidateArrange();
    }

    /// <summary>
    /// Gets or sets the stroke thickness.
    /// </summary>
    /// <value>The stroke thickness.</value>
    public double StrokeThickness
    {
      get { return (double)GetValue(StrokeThicknessProperty); }
      set { SetValue(StrokeThicknessProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StrokeThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StrokeThicknessProperty =
      DependencyProperty.Register("StrokeThickness", typeof(double), typeof(InfoWindow), new PropertyMetadata(2d));

    #endregion

		private void map_RotationChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			CheckPosition();
			InvalidateArrange();
		}

    private void map_ExtentChanging(object sender, ExtentEventArgs e)
    {
      CheckPosition();
      InvalidateArrange();
    }

    private void CheckPosition()
    {

      if (Map != null && Anchor != null && this.Parent != null && Map.Extent != null)
      {
		  var pt = Map.MapToScreen(Anchor, true);
		  bool isOutside = pt.X < 0 || pt.Y < 0 || pt.X > Map.ActualWidth || pt.Y > Map.ActualHeight;
		  this.Visibility = isOutside ? Visibility.Collapsed : Visibility.Visible;
      }
      else if (!isDesignMode)
        this.Visibility = System.Windows.Visibility.Collapsed;
    }
  }
}