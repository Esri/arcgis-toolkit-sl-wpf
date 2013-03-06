using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ESRI.ArcGIS.Client.Symbols;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
    /// <summary>
    /// Point symbol using a bitmap image for symbol
    /// </summary>
    public class KmlPlaceMarkerSymbol : MarkerSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KmlPlaceMarkerSymbol"/> class.
        /// </summary>
        public KmlPlaceMarkerSymbol() : base()
        {
            this.ControlTemplate = ResourceData.Dictionary["KmlPlaceMarkerSymbol"] as ControlTemplate;
        }

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(OnFillPropertyChanged));

        /// <summary>
        /// Gets or sets Fill.
        /// </summary>
        public ImageBrush Fill
        {
            get
            {
                return (ImageBrush)GetValue(FillProperty);
            }

            set
            {
                SetValue(FillProperty, value);
            }
        }

        private static void OnFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Fill");
        }

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(Double.NaN, OnWidthPropertyChanged));

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Width");
        }

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(Double.NaN, OnHeightPropertyChanged));

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }

        private static void OnHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Height");
        }

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(1.0, OnOpacityPropertyChanged));

        /// <summary>
        /// Gets or sets the opacity of the image.
        /// </summary>
        public double Opacity
        {
            get
            {
                return (double)GetValue(OpacityProperty);
            }

            set
            {
                SetValue(OpacityProperty, value);
            }
        }

        private static void OnOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Opacity");
        }

        /// <summary>
        /// Identifies the <see cref="Heading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(0.0, OnHeadingPropertyChanged));

        /// <summary>
        /// Gets or sets the heading of the image.
        /// </summary>
        public double Heading
        {
            get
            {
                return (double)GetValue(HeadingProperty);
            }

            set
            {
                SetValue(HeadingProperty, value);
            }
        }

        private static void OnHeadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Heading");
        }

        /// <summary>
        /// Identifies the <see cref="Scale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(1.0, OnScalePropertyChanged));

        /// <summary>
        /// Gets or sets the scale of the image.
        /// </summary>
        public double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }

            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("Scale");
        }

        /// <summary>
        /// Identifies the <see cref="TranslateX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty = DependencyProperty.Register("TranslateX", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(0.0, OnTranslateXPropertyChanged));

        /// <summary>
        /// Gets or sets the translation along the X axis of the image.
        /// </summary>
        public double TranslateX
        {
            get
            {
                return (double)GetValue(TranslateXProperty);
            }

            set
            {
                SetValue(TranslateXProperty, value);
            }
        }

        private static void OnTranslateXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("TranslateX");
        }

        /// <summary>
        /// Identifies the <see cref="TranslateY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register("TranslateY", typeof(double), typeof(KmlPlaceMarkerSymbol), new PropertyMetadata(0.0, OnTranslateYPropertyChanged));

        /// <summary>
        /// Gets or sets the translation along the X axis of the image.
        /// </summary>
        public double TranslateY
        {
            get
            {
                return (double)GetValue(TranslateYProperty);
            }

            set
            {
                SetValue(TranslateYProperty, value);
            }
        }

        private static void OnTranslateYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KmlPlaceMarkerSymbol dp = d as KmlPlaceMarkerSymbol;
            dp.OnPropertyChanged("TranslateY");
        }
    }
}
