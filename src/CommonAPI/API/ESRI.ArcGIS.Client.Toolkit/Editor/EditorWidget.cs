// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace ESRI.ArcGIS.Client.Toolkit
{
    /// <summary>
    /// Editor widget.
    /// </summary>
    /// <seealso cref="TemplatePicker"/>
    /// <seealso cref="ESRI.ArcGIS.Client.Editor"/>
    [TemplatePart(Name = "TemplatePicker", Type = typeof(TemplatePicker))]
    [TemplatePart(Name = "EditGeometry", Type = typeof(UIElement))]
    [TemplatePart(Name = "Reshape", Type = typeof(UIElement))]
    [TemplatePart(Name = "NewSelect", Type = typeof(UIElement))]
    [TemplatePart(Name = "AddSelect", Type = typeof(UIElement))]
    [TemplatePart(Name = "RemoveSelect", Type = typeof(UIElement))]
    [TemplatePart(Name = "ClearSelect", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "DeleteSelect", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "Union", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "Cut", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "Save", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "DisplayAttribute", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "Options", Type = typeof(ButtonBase))]    
    public partial class EditorWidget : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorWidget"/> class.
        /// </summary>
        public EditorWidget()
        {
            this.editor = new Editor();
            this.editor.EditorActivated += Editor_EditorActivated;
			this.editor.EditCompleted += Editor_EditCompleted;
            this.displayAttribute = false;
            this.DataContext = editor;
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(EditorWidget);
#endif
            SetEditorBinding("AutoComplete", Editor.AutoCompleteProperty);
            SetEditorBinding("AutoSelect", Editor.AutoSelectProperty);
            SetEditorBinding("Continuous", Editor.ContinuousModeProperty);
            SetEditorBinding("Freehand", Editor.FreehandProperty);
            SetEditorBinding("GeometryServiceUrl", Editor.GeometryServiceUrlProperty);
			SetEditorBinding("GeometryServiceToken", Editor.GeometryServiceTokenProperty);
			SetEditorBinding("ProxyUrl", Editor.ProxyUrlProperty);
			SetEditorBinding("MoveEnabled", Editor.MoveEnabledProperty);
			SetEditorBinding("EditVerticesEnabled", Editor.EditVerticesEnabledProperty);
			SetEditorBinding("RotateEnabled", Editor.RotateEnabledProperty);
			SetEditorBinding("ScaleEnabled", Editor.ScaleEnabledProperty);
			SetEditorBinding("MaintainAspectRatioEnabled", Editor.MaintainAspectRatioProperty);
#if!SILVERLIGHT
            SetEditorBinding("GeometryServiceCredentials", Editor.GeometryServiceCredentialsProperty);
			SetEditorBinding("GeometryServiceClientCertificate", Editor.GeometryServiceClientCertificateProperty);
#endif
        }

        /// <summary>
        /// Static initialization for the <see cref="EditorWidget"/> control.
        /// </summary>
        static EditorWidget()
        {
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EditorWidget), new FrameworkPropertyMetadata(typeof(EditorWidget)));
#endif
        }

        private Editor editor;
        private bool displayAttribute;

        #region Child controls
        UIElement EditGeometry;
        UIElement Reshape;
        UIElement NewSelect;
        UIElement AddSelect;
        UIElement RemoveSelect;
        ButtonBase ClearSelect;
        ButtonBase DeleteSelect;
        ButtonBase Cut;
        ButtonBase Union;
        ButtonBase Save;
        ButtonBase DisplayAttribute;
        ButtonBase Options;
        TemplatePicker TemplatePicker;
        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EditGeometry = GetTemplateChild("EditGeometry") as UIElement;
            Reshape = GetTemplateChild("Reshape") as UIElement;
            NewSelect = GetTemplateChild("NewSelect") as UIElement;
            AddSelect = GetTemplateChild("AddSelect") as UIElement;
            RemoveSelect = GetTemplateChild("RemoveSelect") as UIElement;
            ClearSelect = GetTemplateChild("ClearSelect") as ButtonBase;
            DeleteSelect = GetTemplateChild("DeleteSelect") as ButtonBase;
            Union = GetTemplateChild("Union") as ButtonBase;
            Cut = GetTemplateChild("Cut") as ButtonBase;
            Save = GetTemplateChild("Save") as ButtonBase;
            Options = GetTemplateChild("Options") as ButtonBase;
            DisplayAttribute = GetTemplateChild("DisplayAttribute") as ButtonBase;
            if (DisplayAttribute != null)
                DisplayAttribute.Click += DisplayAttribute_Click;
			TemplatePicker = GetTemplateChild("TemplatePicker") as TemplatePicker;
			if (TemplatePicker != null)
			{
				TemplatePicker.EditorActivated += Editor_EditorActivated;
				TemplatePicker.EditCompleted += Editor_EditCompleted;
			}
            if (!this.editor.GraphicsLayers.GetEnumerator().MoveNext())
                this.UpdateVisibleButtons();
        }

        #region Dependency Properties
#if !SILVERLIGHT
		/// <summary>
		/// Gets or sets the client certificate that is sent to the host and used to authenticate the request.
		/// </summary>
		/// <value>The client certificate used for authentication.</value>
		public System.Security.Cryptography.X509Certificates.X509Certificate GeometryServiceClientCertificate
		{
			get { return (System.Security.Cryptography.X509Certificates.X509Certificate)GetValue(GeometryServiceClientCertificateProperty); }
			set { SetValue(GeometryServiceClientCertificateProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GeometryServiceClientCertificate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GeometryServiceClientCertificateProperty =
			DependencyProperty.Register("GeometryServiceClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(EditorWidget), null);
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the network credentials that are sent to the host and used to authenticate the request.
        /// </summary>
        /// <value>The credentials used for authentication.</value>
        public System.Net.ICredentials GeometryServiceCredentials
        {
            get { return (System.Net.ICredentials)GetValue(GeometryServiceCredentialsProperty); }
            set { SetValue(GeometryServiceCredentialsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GeometryServiceCredentialsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceCredentialsProperty =
            DependencyProperty.Register("GeometryServiceCredentials", typeof(System.Net.ICredentials), typeof(EditorWidget), null);
#endif

        #region AlwaysDisplayDefaultTemplates
        /// <summary>
        /// Gets or sets a value indicating whether default templates should 
        /// always be displayed.
        /// </summary>
        /// <remarks>
        /// Default templates are displayed when no other templates exist.
        /// </remarks>
        /// <value>
        /// 	<c>true</c> if [always display default templates]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysDisplayDefaultTemplates
        {
            get { return (bool)GetValue(AlwaysDisplayDefaultTemplatesProperty); }
            set { SetValue(AlwaysDisplayDefaultTemplatesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AlwaysDisplayDefaultTemplates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlwaysDisplayDefaultTemplatesProperty =
            DependencyProperty.Register("AlwaysDisplayDefaultTemplates", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
        #endregion

        #region AutoSelect
        /// <summary>
        /// Gets or sets a value indicating whether selection is automatic 
        /// for tools that require it.
        /// </summary>
        /// <remarks>
        /// Tools that use auto selection: Cut, Reshape, Union, and AutoComplete Add
        /// </remarks>
        /// <value><c>true</c> if [auto select]; otherwise, <c>false</c>.</value>
        public bool AutoSelect
        {
            get { return (bool)GetValue(AutoSelectProperty); }
            set { SetValue(AutoSelectProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AutoSelect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSelectProperty =
            DependencyProperty.Register("AutoSelect", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
        #endregion

        #region AutoComplete
        /// <summary>
        /// Gets or sets a value indicating whether auto completion is enabled 
        /// when adding polygons.
        /// </summary>
        /// <remarks>
        /// When AutoComplete is enabled, a line is drawn instead of a polygon 
        /// and the rest of the polygon is completed based on snapping to nearby features.
        /// </remarks>
        /// <value><c>true</c> if [auto complete]; otherwise, <c>false</c>.</value>
        public bool AutoComplete
        {
            get { return (bool)GetValue(AutoCompleteProperty); }
            set { SetValue(AutoCompleteProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AutoComplete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoCompleteProperty =
            DependencyProperty.Register("AutoComplete", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
        #endregion

        #region Continuous
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EditorWidget"/> is continuous.
        /// </summary>
        /// <value><c>true</c> if continuous; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Tools contained in the EditorWidget like the TemplatePicker, EditVertices, Reshape, 
        /// Cut, Selection, etc. will remain active until a different tool is selected.
        /// </remarks>
        public bool Continuous
        {
            get { return (bool)GetValue(ContinuousProperty); }
            set { SetValue(ContinuousProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Continuous"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContinuousProperty =
            DependencyProperty.Register("Continuous", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
        #endregion

        #region Freehand
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EditorWidget"/> 
        /// is using freehand draw mode when using Add, Reshape, Union and Cut.
        /// </summary>
        /// <value><c>true</c> if freehand; otherwise, <c>false</c>.</value>
        public bool Freehand
        {
            get { return (bool)GetValue(FreehandProperty); }
            set { SetValue(FreehandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Freehand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FreehandProperty =
            DependencyProperty.Register("Freehand", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
        #endregion

		#region MaintainAspectRatio
		/// <summary>
		/// Gets or sets a value indicating whether aspect ratio 
		/// need to be maintained when geometry is scaled.
		/// </summary>
		public bool MaintainAspectRatio
		{
			get { return (bool)GetValue(MaintainAspectRatioProperty); }
			set { SetValue(MaintainAspectRatioProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MaintainAspectRatio"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MaintainAspectRatioProperty =
			DependencyProperty.Register("MaintainAspectRatio", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false));
		#endregion

		#region ScaleEnabled
		/// <summary>
		/// Gets or sets a value indicating whether scale is enabled.
		/// </summary>
		/// <value><c>true</c> if scale is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>If ScaleEnabled is true and command EditVertices is active,
		/// the selected geometry can be scaled until this value is set to false.</remarks>
		public bool ScaleEnabled
		{
			get { return (bool)GetValue(ScaleEnabledProperty); }
			set { SetValue(ScaleEnabledProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ScaleEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ScaleEnabledProperty =
			DependencyProperty.Register("ScaleEnabled", typeof(bool), typeof(EditorWidget), new PropertyMetadata(true));
		#endregion

		#region RotateEnabled
		/// <summary>
		/// Gets or sets a value indicating whether rotate is enabled.
		/// </summary>
		/// <value><c>true</c> if rotate is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>If RotateEnabled is true and command EditVertices is active,
		/// the selected geometry can be rotated until this value is set to false.</remarks>
		public bool RotateEnabled
		{
			get { return (bool)GetValue(RotateEnabledProperty); }
			set { SetValue(RotateEnabledProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RotateEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RotateEnabledProperty =
			DependencyProperty.Register("RotateEnabled", typeof(bool), typeof(EditorWidget), new PropertyMetadata(true));
		#endregion

		#region MoveEnabled
		/// <summary>
		/// Gets or sets a value indicating whether move is enabled.
		/// </summary>
		/// <value><c>true</c> if move is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>If MoveEnabled is true and command EditVertices is active,
		/// the selected geometry can be moved until this value is set to false.</remarks>
		public bool MoveEnabled
		{
			get { return (bool)GetValue(MoveEnabledProperty); }
			set { SetValue(MoveEnabledProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MoveEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MoveEnabledProperty =
			DependencyProperty.Register("MoveEnabled", typeof(bool), typeof(EditorWidget), new PropertyMetadata(true));
		#endregion

		#region EditVerticesEnabled
		/// <summary>
		/// Gets or sets a value indicating whether edit vertices is enabled.
		/// </summary>
		/// <value><c>true</c> if edit vertices is enabled; otherwise, <c>false</c>.</value>
		/// <remarks>If EditVerticesEnabled is true and command EditVertices is active,
		/// the selected geometry's vertices can be edited until this value is set to false.</remarks>
		public bool EditVerticesEnabled
		{
			get { return (bool)GetValue(EditVerticesEnabledProperty); }
			set { SetValue(EditVerticesEnabledProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="EditVerticesEnabled"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty EditVerticesEnabledProperty =
			DependencyProperty.Register("EditVerticesEnabled", typeof(bool), typeof(EditorWidget), new PropertyMetadata(true));
		#endregion

        #region GeometryService
        /// <summary>
        /// Gets or sets the geometry service URL.
        /// </summary>
        /// <value>The geometry service URL.</value>
        public string GeometryServiceUrl
        {
            get { return (string)GetValue(GeometryServiceUrlProperty); }
            set { SetValue(GeometryServiceUrlProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GeometryServiceUrl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceUrlProperty =
            DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(EditorWidget), null);
        #endregion

		#region GeometryServiceToken
		/// <summary>
		/// Gets or sets the token used for geometry service.
		/// </summary>
		/// <value>The token used for geometry service.</value>
		public string GeometryServiceToken
		{
			get { return (string)GetValue(GeometryServiceTokenProperty); }
			set { SetValue(GeometryServiceTokenProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GeometryServiceToken"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GeometryServiceTokenProperty =
			DependencyProperty.Register("GeometryServiceToken", typeof(string), typeof(EditorWidget), null);
		#endregion

		#region ProxyUrl
		/// <summary>
		/// Gets or sets the proxy URL for geometry service.
		/// </summary>
		/// <value>The proxy URL for geometry service.</value>
		public string ProxyUrl
		{
			get { return (string)GetValue(ProxyUrlProperty); }
			set { SetValue(ProxyUrlProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ProxyUrl"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ProxyUrlProperty =
			DependencyProperty.Register("ProxyUrl", typeof(string), typeof(EditorWidget), null);
		#endregion

		#region Layers
		/// <summary>
        /// Gets or sets the layer IDs of the layers for which templates are displayed.
        /// </summary>
        /// <remarks>
        /// Specified in XAML and in Blend as a comma-delimited string: If a layer 
        /// name contains a comma, please use &#44; instead of the comma.
        /// If null/empty, templates from all feature layers are used. Order of 
        /// the layer ids is respected in generating templates.
        /// </remarks>
        /// <value>The layer IDs.</value>
        [System.ComponentModel.TypeConverter(typeof(StringToStringArrayConverter))]
		public string[] LayerIDs
        {
			get { return (string[])GetValue(LayerIDsProperty); }
            set { SetValue(LayerIDsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LayerIDs"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LayerIDsProperty = DependencyProperty.Register("LayerIDs",
            typeof(string[]), typeof(EditorWidget), new PropertyMetadata(OnLayerIdsPropertyChanged));

        private static void OnLayerIdsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditorWidget widget = (d as EditorWidget);
            if (widget != null)
            {
                widget.DetachLayerEventHandlers(widget.editor.GraphicsLayers);
                widget.editor.LayerIDs = widget.LayerIDs;				
                widget.AttachLayerEventHandlers(widget.editor.GraphicsLayers);
            }
        }
        #endregion

        #region Map
        /// <summary>
        /// Gets or sets the map that the template picker is buddied to.
        /// </summary>
        /// <value>The map.</value>
        public ESRI.ArcGIS.Client.Map Map
        {
            get { return GetValue(MapProperty) as Map; }
            set { SetValue(MapProperty, value); }
        }

        /// /// <summary>
        /// Identifies the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map",
              typeof(Map), typeof(EditorWidget), new PropertyMetadata(OnMapPropertyChanged));

        private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Map oldMap = e.OldValue as Map;
            Map newMap = e.NewValue as Map;
            EditorWidget widget = (EditorWidget)d;
            if (widget != null)
            {
                widget.editor.Map = newMap;
                if (oldMap != null && oldMap.Layers != null)
                {
                    List<GraphicsLayer> oldGraphicsLayers = new List<GraphicsLayer>();
                    foreach (Layer layer in oldMap.Layers)
                    {
                        if (layer is GraphicsLayer)
                            oldGraphicsLayers.Add(layer as GraphicsLayer);
                    }
                    widget.DetachLayerEventHandlers(oldGraphicsLayers);
                    oldMap.Layers.CollectionChanged -= widget.Layers_CollectionChanged;
                }
                if (newMap != null && newMap.Layers != null)
                {
                    newMap.Layers.CollectionChanged += widget.Layers_CollectionChanged;
                    widget.AttachLayerEventHandlers(widget.editor.GraphicsLayers);
                }
            }
        }
        #endregion

        #region ShowAttributesOnAdd
        /// <summary>
        /// Gets or sets a value indicating whether the attributes are shown
        /// when a graphic is added to the feature layer
        /// </summary>
        public bool ShowAttributesOnAdd
        {
            get { return (bool)GetValue(ShowAttributesOnAddProperty); }
            set { SetValue(ShowAttributesOnAddProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowAttributesOnAdd"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowAttributesOnAddProperty = DependencyProperty.Register("ShowAttributesOnAdd", typeof(bool), typeof(EditorWidget), new PropertyMetadata(false, OnShowAttributesOnAddPropertyChanged));

        private static void OnShowAttributesOnAddPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            EditorWidget widget = (EditorWidget)d;
            if (widget != null && widget.TemplatePicker != null)
                widget.TemplatePicker.ShowAttributesOnAdd = newValue;
        }
        #endregion

        #endregion

        #region Event Handlers
        private void AttachLayerEventHandlers(IEnumerable<GraphicsLayer> layers)
        {
			if (layers != null && layers.GetEnumerator().MoveNext())
			{
				foreach (GraphicsLayer layer in layers)
					AttachLayerEventHandlers(layer);
			}
			else
				UpdateVisibleButtons();
        }

        private void AttachLayerEventHandlers(GraphicsLayer layer)
        {
            if (layer != null)
            {
				if (!layer.IsInitialized)
					layer.Initialized += Layer_Initialized;
				else
					UpdateVisibleButtons();
                layer.PropertyChanged += Layer_PropertyChanged;
                if (layer is FeatureLayer)
                    layer.MouseLeftButtonDown += Layer_MouseLeftButtonDown;
            }
        }

        private void DetachLayerEventHandlers(IEnumerable<GraphicsLayer> layers)
        {
            foreach (GraphicsLayer layer in layers)
                DetachLayerEventHandlers(layer);
        }

        private void DetachLayerEventHandlers(GraphicsLayer layer)
        {
            if (layer != null)
            {
                layer.PropertyChanged -= Layer_PropertyChanged;
                if (!layer.IsInitialized)
                    layer.Initialized -= Layer_Initialized;
                if (layer is FeatureLayer)
                    layer.MouseLeftButtonDown -= Layer_MouseLeftButtonDown;
            }
        }

        private void Layer_Initialized(object sender, EventArgs e)
        {
            Layer layer = sender as Layer;
            layer.Initialized -= Layer_Initialized;
            UpdateVisibleButtons();
        }

        private void Layer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoSave" || e.PropertyName == "Visible")
                UpdateVisibleButtons();
        }

        private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DetachLayerEventHandlers(this.editor.GraphicsLayers);
            if (e.OldItems != null)
                foreach (object layer in e.OldItems)
                {
                    if (layer is GraphicsLayer)
                        DetachLayerEventHandlers(layer as GraphicsLayer);
                }
            AttachLayerEventHandlers(this.editor.GraphicsLayers);
            UpdateVisibleButtons();
        }

        private void Layer_MouseLeftButtonDown(object sender, GraphicMouseButtonEventArgs args)
        {
            if (this.displayAttribute && sender is FeatureLayer)
            {
                FeatureLayer featureLayer = sender as FeatureLayer;
                TemplatePicker.ShowAttributeForm(featureLayer, args.Graphic);
                this.displayAttribute = this.Continuous;
                args.Handled = true;
            }
        }
		
		private void Editor_EditCompleted(object sender, Editor.EditEventArgs e)
		{
			EventHandler<ESRI.ArcGIS.Client.Editor.EditEventArgs> handler = EditCompleted;
			if (handler != null)
				handler(this, e);
		}

		private void Editor_EditorActivated(object sender, Editor.CommandEventArgs e)
		{
			this.displayAttribute = false;
			EventHandler<ESRI.ArcGIS.Client.Editor.CommandEventArgs> handler = EditorActivated;
			if (handler != null)
				handler(this, e);
		}

        private void DisplayAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (this.editor.CancelActive.CanExecute(null))
                this.editor.CancelActive.Execute(null);
            this.displayAttribute = true;
        }

		/// <summary>
		/// Occurs when an edit has completed.
		/// </summary>
		public event EventHandler<ESRI.ArcGIS.Client.Editor.EditEventArgs> EditCompleted;
		
		/// <summary>
		/// Occurs when an editor has been activated.
		/// </summary>
		public event EventHandler<ESRI.ArcGIS.Client.Editor.CommandEventArgs> EditorActivated;
        #endregion

        #region Helper Methods

		private void UpdateVisibleButtons()
		{
			bool isVisible = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
			bool hasPolygonOrPolyline = isVisible;
			bool addPolygonOrPolyline = isVisible;
			bool atLeastOneLayerNotAutoSave = isVisible;
			bool atLeastOneLayerCanAdd = isVisible;
			bool atLeastOneLayerCanUpdate = isVisible;
			bool atLeastOneLayerCanDelete = isVisible;
			bool atLeastOneLayer = isVisible;
			bool atLeastOneFeatureLayer = isVisible;

			foreach (GraphicsLayer layer in this.editor.GraphicsLayers)
			{
				if (layer == null || string.IsNullOrEmpty(layer.ID))
					continue;
				if (!atLeastOneLayer && layer.Visible)
					atLeastOneLayer = true;
				if (layer is FeatureLayer)
				{
					if (!atLeastOneFeatureLayer && layer.Visible)
						atLeastOneFeatureLayer = true;
					FeatureLayer flayer = layer as FeatureLayer;
					if (flayer.LayerInfo == null) continue;
					if ((!hasPolygonOrPolyline || !addPolygonOrPolyline) && (flayer.LayerInfo.IsAddAllowed || flayer.LayerInfo.IsUpdateAllowed) && flayer.Visible && flayer.LayerInfo != null &&
						(flayer.LayerInfo.GeometryType == GeometryType.Polygon || flayer.LayerInfo.GeometryType == GeometryType.Polyline))
					{
						if (!hasPolygonOrPolyline && (flayer.LayerInfo.IsUpdateAllowed || (!flayer.AutoSave && flayer.LayerInfo.IsAddAllowed)))
							hasPolygonOrPolyline = true;
						if (!addPolygonOrPolyline && flayer.LayerInfo.IsAddAllowed)
							addPolygonOrPolyline = true;
					}
					if (!atLeastOneLayerNotAutoSave && !flayer.AutoSave && !flayer.IsReadOnly)
						atLeastOneLayerNotAutoSave = true;
					if (!atLeastOneLayerCanAdd && flayer.LayerInfo.IsAddAllowed && flayer.Visible)
						atLeastOneLayerCanAdd = true;
					if (!atLeastOneLayerCanUpdate && (flayer.LayerInfo.IsUpdateAllowed || (!flayer.AutoSave && flayer.LayerInfo.IsAddAllowed)) && flayer.Visible)
						atLeastOneLayerCanUpdate = true;
					if (!atLeastOneLayerCanDelete && (flayer.LayerInfo.IsDeleteAllowed || (!flayer.AutoSave && flayer.LayerInfo.IsAddAllowed)) && flayer.Visible)
						atLeastOneLayerCanDelete = true;
				}
				else if (layer.Visible)
					hasPolygonOrPolyline = true; //GraphicsLayer may contain polygon or polyline
			}
			bool allButtonsDisabled = !hasPolygonOrPolyline && !atLeastOneLayerNotAutoSave && !addPolygonOrPolyline && 
				!atLeastOneLayerCanAdd && !atLeastOneLayerCanUpdate && atLeastOneLayerCanDelete
				&& !atLeastOneLayer && !atLeastOneFeatureLayer;
			DisableOrHideElement(TemplatePicker, atLeastOneLayerCanAdd, allButtonsDisabled);
			DisableOrHideElement(EditGeometry, hasPolygonOrPolyline || atLeastOneLayerCanUpdate, allButtonsDisabled);
			DisableOrHideElement(Reshape, hasPolygonOrPolyline, allButtonsDisabled);
			DisableOrHideElement(NewSelect, atLeastOneLayer, allButtonsDisabled);
			DisableOrHideElement(AddSelect, atLeastOneLayer, allButtonsDisabled);
			DisableOrHideElement(RemoveSelect, atLeastOneLayer, allButtonsDisabled);
			DisableOrHideElement(ClearSelect, atLeastOneLayer, allButtonsDisabled);
			DisableOrHideElement(DeleteSelect, atLeastOneLayerCanDelete, allButtonsDisabled);
			DisableOrHideElement(Cut, hasPolygonOrPolyline, allButtonsDisabled);
			DisableOrHideElement(Union, hasPolygonOrPolyline, allButtonsDisabled);
			DisableOrHideElement(Save, atLeastOneLayerNotAutoSave, allButtonsDisabled);
			DisableOrHideElement(DisplayAttribute, atLeastOneFeatureLayer, allButtonsDisabled);
			DisableOrHideElement(Options, addPolygonOrPolyline, allButtonsDisabled);
		}

        private void DisableOrHideElement(UIElement element, bool isVisible, bool allButtonsDisabled)
        {
			if (element != null)
			{
				if (allButtonsDisabled && element is Control)
				{
					(element as Control).IsEnabled = false;
					if (element == TemplatePicker)
						element.Visibility = Visibility.Collapsed;
					else
						element.Visibility = Visibility.Visible;
				}
				else
				{
					if (element is Control)
						(element as Control).IsEnabled = isVisible;
					element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
				}
			}
        }

        private void SetEditorBinding(string path, DependencyProperty dependencyProperty)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(path);
            binding.Source = this;
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            System.Windows.Data.BindingOperations.SetBinding(editor, dependencyProperty, binding);
        }
        #endregion
    }
}
