using System.Windows;
using System.Windows.Media;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
    /// <summary>
    /// *FOR INTERNAL USE ONLY*
    /// </summary>
    /// <remarks>
    /// Surrogate binder class for setting the angle/heading.
    /// </remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class SurrogateBindRotate
    {
        /// <summary>
        /// Dependency property to assign the heading/angle of the place marker symbol.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.RegisterAttached("Angle", typeof(double),
            typeof(SurrogateBindRotate),
            new PropertyMetadata(OnAngleChanged));

        /// <summary>
        /// Get function for angle property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <returns>The current angle/heading value.</returns>
        public static double GetAngle(DependencyObject d)
        {
            return (double)d.GetValue(AngleProperty);
        }

        /// <summary>
        /// Set function for angle property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="value">The value to assign as the angle/heading.</param>
        public static void SetAngle(DependencyObject d, double value)
        {
            d.SetValue(AngleProperty, value);
        }

        /// <summary>
        /// Called when the attached property is bound/changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                UIElement b = d as UIElement;
                if (e.NewValue is double)
                {
                    double c = (double)e.NewValue;
                    if (!double.IsNaN(c))
                    {
                        if (b.RenderTransform is TransformGroup)
                        {
                            TransformGroup tg = b.RenderTransform as TransformGroup;
                            foreach (Transform t in tg.Children)
                            {
                                RotateTransform rt = t as RotateTransform;
                                if (rt != null)
                                {
                                    rt.Angle = c;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// *FOR INTERNAL USE ONLY*
    /// </summary>
    /// <remarks>
    /// Surrogate binder class for setting the scale.
    /// </remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class SurrogateBindScale
    {
        /// <summary>
        /// Dependency property to assign the scale of the place marker symbol.
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.RegisterAttached("Scale", typeof(double),
            typeof(SurrogateBindScale),
            new PropertyMetadata(OnScaleChanged));

        /// <summary>
        /// Get function for scale property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <returns>The current scale value.</returns>
        public static double GetScale(DependencyObject d)
        {
            return (double)d.GetValue(ScaleProperty);
        }

        /// <summary>
        /// Set function for scale property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="value">The value to assign as the scale.</param>
        public static void SetScale(DependencyObject d, double value)
        {
            d.SetValue(ScaleProperty, value);
        }

        /// <summary>
        /// Called when the attached property is bound/changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                UIElement b = d as UIElement;
                if (e.NewValue is double)
                {
                    double c = (double)e.NewValue;
                    if (!double.IsNaN(c))
                    {
                        if (b.RenderTransform is TransformGroup)
                        {
                            TransformGroup tg = b.RenderTransform as TransformGroup;
                            foreach (Transform t in tg.Children)
                            {
                                ScaleTransform st = t as ScaleTransform;
                                if (st != null)
                                {
                                    st.ScaleX = c;
                                    st.ScaleY = c;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// *FOR INTERNAL USE ONLY*
    /// </summary>
    /// <remarks>
    /// Surrogate binder class for setting the translation along the X axis.
    /// </remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class SurrogateBindTranslateX
    {
        /// <summary>
        /// Dependency property to assign the translation along the X axis of the place marker symbol.
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.RegisterAttached("TranslateX", typeof(double),
            typeof(SurrogateBindTranslateX),
            new PropertyMetadata(OnTranslateXChanged));

        /// <summary>
        /// Get function for translate X property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <returns>The current translation value.</returns>
        public static double GetTranslateX(DependencyObject d)
        {
            return (double)d.GetValue(TranslateXProperty);
        }

        /// <summary>
        /// Set function for translate X property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="value">The value to assign as the translation.</param>
        public static void SetTranslateX(DependencyObject d, double value)
        {
            d.SetValue(TranslateXProperty, value);
        }

        /// <summary>
        /// Called when the attached property is bound/changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTranslateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                UIElement b = d as UIElement;
                if (e.NewValue is double)
                {
                    double c = (double)e.NewValue;
                    if (!double.IsNaN(c))
                    {
                        if (b.RenderTransform is TransformGroup)
                        {
                            TransformGroup tg = b.RenderTransform as TransformGroup;
                            foreach (Transform t in tg.Children)
                            {
                                TranslateTransform st = t as TranslateTransform;
                                if (st != null)
                                {
                                    st.X = c;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// *FOR INTERNAL USE ONLY*
    /// </summary>
    /// <remarks>
    /// Surrogate binder class for setting the translation along the X axis.
    /// </remarks>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class SurrogateBindTranslateY
    {
        /// <summary>
        /// Dependency property to assign the translation along the Y axis of the place marker symbol.
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.RegisterAttached("TranslateY", typeof(double),
            typeof(SurrogateBindTranslateY),
            new PropertyMetadata(OnTranslateYChanged));

        /// <summary>
        /// Get function for translate Y property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <returns>The current translation value.</returns>
        public static double GetTranslateY(DependencyObject d)
        {
            return (double)d.GetValue(TranslateYProperty);
        }

        /// <summary>
        /// Set function for translate Y property.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="value">The value to assign as the translation.</param>
        public static void SetTranslateY(DependencyObject d, double value)
        {
            d.SetValue(TranslateYProperty, value);
        }

        /// <summary>
        /// Called when the attached property is bound/changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTranslateYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                UIElement b = d as UIElement;
                if (e.NewValue is double)
                {
                    double c = (double)e.NewValue;
                    if (!double.IsNaN(c))
                    {
                        if (b.RenderTransform is TransformGroup)
                        {
                            TransformGroup tg = b.RenderTransform as TransformGroup;
                            foreach (Transform t in tg.Children)
                            {
                                TranslateTransform st = t as TranslateTransform;
                                if (st != null)
                                {
                                    st.Y = c;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
