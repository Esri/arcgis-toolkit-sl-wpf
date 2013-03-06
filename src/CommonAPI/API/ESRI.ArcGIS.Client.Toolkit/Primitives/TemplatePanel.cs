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
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
    /// <summary>
    /// *FOR INTERNAL USE ONLY* Lays out templates in a grid whose one dimension is specified.
    /// </summary>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class TemplatePanel : Control
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TemplatePanel()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(TemplatePanel);
#endif
        }
        /// <summary>
        /// Static initialization for the <see cref="TemplatePanel"/> control.
        /// </summary>
        static TemplatePanel()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplatePanel), new FrameworkPropertyMetadata(typeof(TemplatePanel)));
#endif
        }
        #region Properties
        #region ItemTemplate
        /// <summary>
        /// Gets or sets the data template for TemplatePanel
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TemplatePanel), null);
        #endregion

        #region TemplateGroups
        /// <summary>
        /// The templates to be displayed in the template panel.
        /// </summary>
        public IEnumerable<SymbolTemplate> Templates
        {
            get { return (IEnumerable<SymbolTemplate>)GetValue(TemplatesProperty); }
            set { SetValue(TemplatesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Templates"/> dependency property
        /// </summary>
        public static readonly DependencyProperty TemplatesProperty =
            DependencyProperty.Register("Templates",
            typeof(IEnumerable<SymbolTemplate>), typeof(TemplatePanel), new PropertyMetadata(OnTemplatePropertyChanged));

        private static void OnTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemplatePanel)
                (d as TemplatePanel).layoutTemplates();
        }
        #endregion

        #region StackDirection, StackCount
        /// <summary>
        /// Identifies the <see cref="StackDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StackDirectionProperty = DependencyProperty.Register("StackDirection",
            typeof(Orientation), typeof(TemplatePanel), new PropertyMetadata(OnStackDirectionPropertyChanged));

        private static void OnStackDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TemplatePanel)
                (d as TemplatePanel).layoutTemplates();
        }

        /// <summary>
        /// Gets or sets the StackDirection.
        /// When <see cref="StackDirection"/> is Horizontal, the number of rows equals the <see cref="StackCount"/>.  
        /// When <see cref="StackDirection"/> is Vertical, the number of columns equals the <see cref="StackCount"/>.
        /// <see cref="StackCount"/> has to be 1 or more.
        /// </summary>
        public Orientation StackDirection
        {
            get { return (Orientation)GetValue(StackDirectionProperty); }
            set { SetValue(StackDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="StackCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StackCountProperty = DependencyProperty.Register("StackCount",
            typeof(int), typeof(TemplatePanel), new PropertyMetadata(1, OnStackCountPropertyChanged));

        /// <summary>
        /// Gets or sets the number of stacks.
        /// When <see cref="StackDirection"/> is Horizontal, the number of rows equals the <see cref="StackCount"/>.  
        /// When <see cref="StackDirection"/> is Vertical, the number of columns equals the <see cref="StackCount"/>.
        /// <see cref="StackCount"/> has to be 1 or more.
        /// </summary>
        public int StackCount
        {
            get { return (int)GetValue(StackCountProperty); }
            set { SetValue(StackCountProperty, value); }
        }

        private static void OnStackCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 1)
                throw new Exception(Properties.Resources.TemplatePanel_StackCountLessThanOne);
            if (d is TemplatePanel)
                (d as TemplatePanel).layoutTemplates();
        }
        #endregion

        #endregion

        Grid palette;
        /// <summary>
        /// Overrides apply template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            palette = (Grid)GetTemplateChild("Palette");
            layoutTemplates();
        }

        #region arrangement methods
        void layoutTemplates()
        {
            if (palette == null || Templates == null)
                return;
            palette.Children.Clear();
            palette.ColumnDefinitions.Clear();
            palette.RowDefinitions.Clear();

            if (StackDirection == Orientation.Vertical) //StackCount = ColumnCount
            {
                if (palette.ColumnDefinitions == null || palette.ColumnDefinitions.Count != StackCount)
                {
                    palette.ColumnDefinitions.Clear();
                    palette.Children.Clear();
                    for (int i = 0; i < StackCount; i++)
                        palette.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
            else //StackCount = RowCount
            {
                if (palette.RowDefinitions == null || palette.RowDefinitions.Count != StackCount)
                {
                    palette.RowDefinitions.Clear();
                    palette.Children.Clear();
                    for (int i = 0; i < StackCount; i++)
                        palette.RowDefinitions.Add(new RowDefinition());
                }
            }

            int row = 0, col = 0;
            foreach (SymbolTemplate type in Templates)
            {
                FrameworkElement element = getElement(type);
                if (element != null)
                {
                    int total = palette.Children.Count;
                    if (StackDirection == Orientation.Vertical)//StackCount = ColumnCount
                    {
                        col = total % StackCount;
                        row = total / StackCount;
                        if (row >= palette.RowDefinitions.Count)
                            palette.RowDefinitions.Add(new RowDefinition());
                    }
                    else
                    {
                        row = total % StackCount;
                        col = total / StackCount;
                        if (col >= palette.ColumnDefinitions.Count)
                            palette.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    element.SetValue(Grid.ColumnProperty, col);
                    element.SetValue(Grid.RowProperty, row);
                    palette.Children.Add(element);
                }

            }
        }

        FrameworkElement getElement(SymbolTemplate type)
        {
			if (type != null)
			{
				DataTemplate template = this.ItemTemplate;
				FrameworkElement obj = template.LoadContent() as FrameworkElement;
#if !SILVERLIGHT
				//This code forces re-evaluation of Command in WPF
				//CanExecute is evaluated once Command is set and does not get called again once 
				//CommandParameter has resolved its binding statement.
				RoutedEventHandler handler = null;
				handler = (s, a) =>
				{
					obj.Loaded -= handler;
					var editor = type.Editor;
					type.Editor = null;
					type.Editor = editor;
				};
				obj.Loaded += handler;
#endif
				obj.DataContext = type;
				return obj;
			}
            return null;
        }

        #endregion
    }
}
