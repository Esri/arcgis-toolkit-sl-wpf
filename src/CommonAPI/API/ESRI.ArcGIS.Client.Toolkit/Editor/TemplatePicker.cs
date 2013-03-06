// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using ESRI.ArcGIS.Client.FeatureService;
using System.Linq;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// A template picker control enables selecting feature types to add 
	/// when editing a feature layer that is based on a feature service.
	/// </summary>
	[StyleTypedProperty(Property = "ItemTemplate", StyleTargetType = typeof(FrameworkElement))]
	public class TemplatePicker : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemplatePicker"/> class.
		/// </summary>
		public TemplatePicker()
		{
#if SILVERLIGHT
			this.DefaultStyleKey = typeof(TemplatePicker);
#endif
		}

		/// <summary>
		/// Static initialization for the <see cref="TemplatePicker"/> control.
		/// </summary>
		static TemplatePicker()
		{
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplatePicker), new FrameworkPropertyMetadata(typeof(TemplatePicker)));
#endif
		}

		#region Private Members
		private bool mapLayersInitialized;

		private static IEnumerable<FeatureLayer> GetLayers(IEnumerable<string> ids, LayerCollection layers)
		{
			if (layers != null)
			{
				if (ids != null)
				{
					foreach (string item in ids)
					{
						FeatureLayer featureLayer = GetFeatureLayerWithID(item, layers);
						if (featureLayer != null)
							yield return featureLayer;
					}
				}
				else
				{
					foreach (Layer layer in layers)
					{
						if (layer is FeatureLayer && !string.IsNullOrEmpty(layer.ID))
						{
							yield return layer as FeatureLayer;
						}
					}
				}
				}
			}
		#endregion

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
			DependencyProperty.Register("GeometryServiceClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(TemplatePicker), new PropertyMetadata(OnGeometryServiceClientCertificatePropertyChanged));

		private static void OnGeometryServiceClientCertificatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var newValue = (System.Security.Cryptography.X509Certificates.X509Certificate)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.GeometryServiceClientCertificate = newValue;
			}
		}
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
            DependencyProperty.Register("GeometryServiceCredentials", typeof(System.Net.ICredentials), typeof(TemplatePicker), new PropertyMetadata(OnCredentialsPropertyChanged));

        private static void OnCredentialsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
            System.Net.ICredentials newValue = (System.Net.ICredentials)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
                foreach (SymbolTemplate template in picker.Templates)
                    template.Editor.GeometryServiceCredentials = newValue;
			}
		}
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
			DependencyProperty.Register("AlwaysDisplayDefaultTemplates", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false));
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
			DependencyProperty.Register("AutoSelect", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false, OnAutoSelectPropertyChanged));

		private static void OnAutoSelectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool newValue = (bool)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.AutoSelect = newValue;
			}
		}
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
			DependencyProperty.Register("AutoComplete", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false, OnAutoCompletePropertyChanged));

		private static void OnAutoCompletePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.AutoComplete = newValue;
			}
		}
		#endregion

		#region Continuous
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TemplatePicker"/> is continuous.
		/// </summary>
		/// <value><c>true</c> if continuous; otherwise, <c>false</c>.</value>
		/// <remarks>
		/// The template selected will remain active until a different template is selected.
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
			DependencyProperty.Register("Continuous", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false, OnContinuousPropertyChanged));

		private static void OnContinuousPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool newValue = (bool)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.ContinuousMode = newValue;
			}
		}
		#endregion

		#region Freehand
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TemplatePicker"/> 
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
			DependencyProperty.Register("Freehand", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false, OnFreehandPropertyChanged));

		private static void OnFreehandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.Freehand = newValue;
			}
		}
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
			DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(TemplatePicker), null);
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
			DependencyProperty.Register("GeometryServiceToken", typeof(string), typeof(TemplatePicker), new PropertyMetadata(OnGeometryServiceTokenPropertyChanged));

		private static void OnGeometryServiceTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var newValue = e.NewValue as string;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null && picker.Templates != null)
			{
				foreach (SymbolTemplate template in picker.Templates)
					template.Editor.GeometryServiceToken= newValue;
			}
		}
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
			DependencyProperty.Register("ProxyUrl", typeof(string), typeof(TemplatePicker), null);
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
		public static readonly DependencyProperty LayerIDsProperty =
			DependencyProperty.Register("LayerIDs", typeof(string[]), typeof(TemplatePicker), new PropertyMetadata(OnLayerIDsPropertyChanged));

		private static void OnLayerIDsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IEnumerable<string> oldLayerIds = (IEnumerable<string>)e.OldValue;
			IEnumerable<string> newLayerIds = (IEnumerable<string>)e.NewValue;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null)
			{
				if (oldLayerIds != null && picker.Map != null)
					picker.DetachLayerEventHandler(GetLayers(oldLayerIds, picker.Map.Layers));
				if (newLayerIds != null && picker.Map != null)
					picker.AttachLayerEventHandler(GetLayers(newLayerIds, picker.Map.Layers));
				picker.setTemplates();
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
		public static readonly DependencyProperty ShowAttributesOnAddProperty = DependencyProperty.Register("ShowAttributesOnAdd", typeof(bool), typeof(TemplatePicker), new PropertyMetadata(false));
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
		public static readonly DependencyProperty MapProperty =
			DependencyProperty.Register("Map", typeof(Map), typeof(TemplatePicker), new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Map oldMap = e.OldValue as Map;
			Map newMap = e.NewValue as Map;
			TemplatePicker picker = (TemplatePicker)d;
			if (picker != null)
			{
				if (oldMap != null && oldMap.Layers != null)
				{
					oldMap.PropertyChanged -= picker.Map_PropertyChanged;
					picker.DetachMapLayerCollection(oldMap.Layers);
				}
				if (newMap != null && newMap.Layers != null)
				{
					newMap.PropertyChanged += picker.Map_PropertyChanged;
					picker.AttachMapLayerCollection(newMap.Layers);
				}
			}
		}

		LayerCollection layerCollection = null;
		private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Map != sender) return;
			if (e.PropertyName == "Layers")
			{
				if(layerCollection!=null)
					DetachMapLayerCollection(layerCollection);
				AttachMapLayerCollection(Map.Layers);
			}
		}

		private void DetachMapLayerCollection(LayerCollection layers)
		{
			layers.LayersInitialized -= MapView_LayersInitialized;
			layers.CollectionChanged -= Layers_CollectionChanged;
			DetachLayerEventHandler(GetLayers(LayerIDs, layers));
		}

		private void AttachMapLayerCollection(LayerCollection layers)
		{
			if (layers == null) return;
			layerCollection = layers;
			layers.LayersInitialized += MapView_LayersInitialized;
			layers.CollectionChanged += Layers_CollectionChanged;
			AttachLayerEventHandler(GetLayers(LayerIDs, layers));
			var initializedLayers = layers.Where(l => l is FeatureLayer && l.IsInitialized);
			if (initializedLayers.Count() > 0)
				setTemplates();
		}
		#endregion

		#region Templates
		/// <summary>
		/// Gets or sets the templates for all of the layers that the Template Picker is working with.
		/// </summary>
		/// <value>The templates.</value>
		public IEnumerable<SymbolTemplate> Templates
		{
			get { return (IEnumerable<SymbolTemplate>)GetValue(TemplatesProperty); }
			set { SetValue(TemplatesProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Templates"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TemplatesProperty =
			DependencyProperty.Register("Templates", typeof(IEnumerable<SymbolTemplate>), typeof(TemplatePicker), null);
		#endregion

		#region TemplateGroups
		/// <summary>Gets or sets the template groups.</summary>
		/// <remarks>Each template group has the templates for a layer</remarks>		
		/// <value>The template groups.</value>
		public IEnumerable<TemplateGroup> TemplateGroups
		{
			get { return (IEnumerable<TemplateGroup>)GetValue(TemplateGroupsProperty); }
			set { SetValue(TemplateGroupsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="TemplateGroups"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TemplateGroupsProperty =
			DependencyProperty.Register("TemplateGroups", typeof(IEnumerable<TemplateGroup>), typeof(TemplatePicker), new PropertyMetadata(OnTemplateGroupsPropertyChanged));


		private static void OnTemplateGroupsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TemplatePicker picker = (d as TemplatePicker);
			picker.Templates = GetTemplates(e.NewValue as IEnumerable<TemplateGroup>);
		}
		#endregion

		#region ItemTemplate
		/// <summary>
		/// Gets or sets the data template for TemplatePicker
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
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TemplatePicker), null);
		#endregion
		#endregion


		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code
		/// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			setTemplates();
		}

		#region Event Handlers
		Dictionary<FeatureLayer, ILegendSupport> rendererLookup = new Dictionary<FeatureLayer, ILegendSupport>();
		private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (string.IsNullOrEmpty((sender as Layer).ID) || (LayerIDs != null && !LayerIDs.Contains((sender as Layer).ID)))
				return;
			if (e.PropertyName == "Visible" || e.PropertyName == "Renderer")
			{
				var featureLayer = sender as FeatureLayer;
				if (featureLayer == null) return;
				var renderer = (sender as FeatureLayer).Renderer as ILegendSupport;
				if (e.PropertyName == "Renderer" && renderer != null)
				{
					if (rendererLookup.ContainsKey(featureLayer))
					{
						var savedRenderer = rendererLookup[featureLayer];
						if (savedRenderer != renderer)
							savedRenderer.LegendChanged -= Renderer_LegendChanged;
					}
					else
						rendererLookup.Add(featureLayer, renderer);
					renderer.LegendChanged += Renderer_LegendChanged;
				}
				setTemplates();
			}
		}

		void Renderer_LegendChanged(object sender, EventArgs e)
		{
			setTemplates();
		}

		private void MapView_LayersInitialized(object sender, EventArgs e)
		{
			if (sender != null && !mapLayersInitialized)
			{
				setTemplates();
				mapLayersInitialized = true;
			}
		}

		private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			bool featureLayerUpdated = (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
			if (e.OldItems != null)
			{
				var featureLayers = new List<FeatureLayer>();								
				foreach (object layer in e.OldItems)
				{
					if (layer is FeatureLayer && !string.IsNullOrEmpty((layer as FeatureLayer).ID))
							featureLayers.Add(layer as FeatureLayer);
				}
				featureLayerUpdated = featureLayers.Count > 0;
				DetachLayerEventHandler(featureLayers);
			}
			if (e.NewItems != null)
			{
				var hookToInitialized = new LayerCollection();
				var hookToPropertyChangedOnly = new LayerCollection();
				foreach (object layer in e.NewItems)
				{
					if (layer is FeatureLayer && !string.IsNullOrEmpty((layer as FeatureLayer).ID))
					{	
						if (mapLayersInitialized && !(layer as FeatureLayer).IsInitialized)
							hookToInitialized.Add(layer as FeatureLayer);
						else
							hookToPropertyChangedOnly.Add(layer as FeatureLayer);
					}
				}
				featureLayerUpdated = hookToPropertyChangedOnly.Count > 0;
				AttachLayerEventHandler(GetLayers(LayerIDs, hookToInitialized), true);
				AttachLayerEventHandler(GetLayers(LayerIDs, hookToPropertyChangedOnly), false);
			}
			if (this.mapLayersInitialized && featureLayerUpdated)
				this.setTemplates();
		}

		private void Editor_EditorActivated(object sender, Editor.CommandEventArgs e)
		{
			OnEditorActivated(e);
		}

		private void Editor_EditCompleted(object sender, Editor.EditEventArgs e)
		{
			if (this.ShowAttributesOnAdd && e.Action == Editor.EditAction.Add)
			{
				foreach (Editor.Change change in e.Edits)
				{
					if (change.Layer != null && change.Layer is FeatureLayer && change.Graphic != null)
					{
						FeatureLayer featureLayer = change.Layer as FeatureLayer;
						ShowAttributeForm(featureLayer, change.Graphic);
						break;
					}
				}
			}
			OnEditCompleted(e);
		}
		#endregion

		#region Helper Methods
		private static bool IsTemplatable(FeatureLayer featureLayer)
		{
			return (featureLayer != null && featureLayer.Visible
				&& featureLayer.LayerInfo != null && featureLayer.LayerInfo.IsAddAllowed);
		}

		private static FeatureLayer GetFeatureLayerWithID(string layerID, LayerCollection layers)
		{
			if (layers != null && !string.IsNullOrEmpty(layerID) && layers[layerID] is FeatureLayer)
				return layers[layerID] as FeatureLayer;
			return null;
		}

		private void AttachLayerEventHandler(IEnumerable<FeatureLayer> layers, bool hookToInitialized = false)
		{
			foreach (FeatureLayer layer in layers)
				AttachLayerEventHandler(layer, hookToInitialized);
		}

		List<FeatureLayer> attachedLayers = new List<FeatureLayer>();
		private void AttachLayerEventHandler(FeatureLayer layer, bool hookToInitialized)
		{
			if (layer != null)
			{
				if(attachedLayers.Contains(layer))
					return;
				attachedLayers.Add(layer);
				if (hookToInitialized && !layer.IsInitialized)
					layer.Initialized += Layer_Initialized;
				layer.PropertyChanged += Layer_PropertyChanged;
			}
		}

		private void Layer_Initialized(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty((sender as Layer).ID) || LayerIDs != null && !LayerIDs.Contains((sender as Layer).ID))
				return;
			Layer layer = sender as Layer;
			layer.Initialized -= Layer_Initialized;
			setTemplates();
		}

		private void DetachLayerEventHandler(IEnumerable<FeatureLayer> layers)
		{
			foreach (FeatureLayer layer in layers)
				DetachLayerEventHandler(layer);
		}

		private void DetachLayerEventHandler(FeatureLayer layer)
		{
			if (layer != null)
			{
				attachedLayers.Remove(layer);
				layer.Initialized -= Layer_Initialized;
				layer.PropertyChanged -= Layer_PropertyChanged;
			}
		}

		private void setTemplates()
		{
			List<TemplateGroup> templateGroups = new List<TemplateGroup>();
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				TemplateGroup group = new TemplateGroup()
				{
					Name = "LayerID",
					Templates = new List<SymbolTemplate>()
				};
				SimpleFillSymbol fillSymbol = new SimpleFillSymbol()
				{
					Fill = new SolidColorBrush(Colors.Black)
				};
				group.Templates.Add(GetSymbolTemplate(new Editor(), fillSymbol, null, null, "Template Name", string.Empty));
				templateGroups.Add(group);
			}
			else
			{
				if (Map == null) return;
				foreach (FeatureLayer layer in GetLayers(LayerIDs, Map.Layers))
				{
					if (!IsTemplatable(layer)) continue;
					if (layer.LayerInfo.HasM || layer.LayerInfo.HasZ && !layer.LayerInfo.EnableZDefaults) continue;
					Editor editor = new Editor()
					{
						AutoComplete = AutoComplete,
						AutoSelect = AutoSelect,
						ContinuousMode = Continuous,
						Freehand = Freehand,
						GeometryServiceUrl = GeometryServiceUrl,
						GeometryServiceToken = GeometryServiceToken,
						ProxyUrl = ProxyUrl,
						LayerIDs = new string[] { layer.ID },
						Map = Map,
					};
					editor.EditorActivated += Editor_EditorActivated;
					editor.EditCompleted += Editor_EditCompleted;
					TemplateGroup group = new TemplateGroup();
					group.Name = layer.ID;
					group.Layer = layer;
					group.Templates = new List<SymbolTemplate>();

					if (layer.Renderer != null)
					{
						Symbol defaultSymbol = layer.Renderer.GetSymbol(null);
						if (layer.LayerInfo.FeatureTypes != null && layer.LayerInfo.FeatureTypes.Count > 0)
						{
							foreach (KeyValuePair<object, FeatureType> type in layer.LayerInfo.FeatureTypes)
							{
								if (type.Value != null && type.Value.Templates != null && type.Value.Templates.Count > 0)
								{
									foreach (KeyValuePair<string, FeatureTemplate> featureTemplate in type.Value.Templates)
									{
										string name = type.Value.Name;
										if (type.Value.Templates.Count > 1)
											name = string.Format("{0}-{1}", type.Value.Name, featureTemplate.Value.Name);
										Symbol symbol = featureTemplate.Value.GetSymbol(layer.Renderer) ?? defaultSymbol;
										SymbolTemplate symbolTemplate = GetSymbolTemplate(editor, symbol,
											type.Value.Id, featureTemplate.Value,
											name, featureTemplate.Value.Description);
										if (symbol != null)
											group.Templates.Add(symbolTemplate);
									}
								}
							}
							if (AlwaysDisplayDefaultTemplates)
							{
								if (defaultSymbol != null)
								{
									var defaultLabel = layer.Renderer is UniqueValueRenderer ? (layer.Renderer as UniqueValueRenderer).DefaultLabel :
										(layer.Renderer is UniqueValueMultipleFieldsRenderer) ? (layer.Renderer as UniqueValueMultipleFieldsRenderer).DefaultLabel : null;
									group.Templates.Add(GetSymbolTemplate(editor, defaultSymbol, null, null, defaultLabel ?? layer.ID, null));
								}
							}
						}
						else if (layer.LayerInfo.Templates != null && layer.LayerInfo.Templates.Count > 0)
						{
							foreach (KeyValuePair<string, FeatureTemplate> featureTemplate in layer.LayerInfo.Templates)
							{
								SymbolTemplate symbolTemplate = GetSymbolTemplate(editor, defaultSymbol,
											null, featureTemplate.Value, featureTemplate.Value.Name, featureTemplate.Value.Description);
								if (defaultSymbol != null)
									group.Templates.Add(symbolTemplate);
							}
						}
						else if (layer.Renderer is UniqueValueRenderer || layer.Renderer is UniqueValueMultipleFieldsRenderer)
						{
							var uvr = layer.Renderer as UniqueValueRenderer;
							if (uvr != null)
							{
								foreach (var info in uvr.Infos)
								{
									var prototypeAttributes = new Dictionary<string, object>();
									prototypeAttributes[uvr.Field] = info.Value;
									var featureTemplate = new FeatureTemplate(info.Label, info.Description, null, prototypeAttributes, FeatureEditTool.None);
									group.Templates.Add(GetSymbolTemplate(editor, info.Symbol, info.Value, featureTemplate, info.Label, info.Description));
								}
								if (AlwaysDisplayDefaultTemplates && uvr.DefaultSymbol != null)
										group.Templates.Add(GetSymbolTemplate(editor, uvr.DefaultSymbol, null, null, uvr.DefaultLabel ?? layer.ID, null));
							}
							else
							{
								var uvmfr = layer.Renderer as UniqueValueMultipleFieldsRenderer;
								foreach (var info in uvmfr.Infos)
								{
									var prototypeAttributes = new Dictionary<string, object>();
									if (uvmfr.Fields != null)
									{
										int i = 0;
										foreach (var field in uvmfr.Fields)
										{
											prototypeAttributes[field] = info.Values[i];
											i++;
										}
									}
									var featureTemplate = new FeatureTemplate(info.Label, info.Description, null, prototypeAttributes, FeatureEditTool.None);
									group.Templates.Add(GetSymbolTemplate(editor, info.Symbol, null, featureTemplate, info.Label, info.Description));
								}
								if (AlwaysDisplayDefaultTemplates && uvmfr.DefaultSymbol != null)
									group.Templates.Add(GetSymbolTemplate(editor, uvmfr.DefaultSymbol, null, null, uvmfr.DefaultLabel ?? layer.ID, null));
							}
						}
						else
						{
							if (defaultSymbol != null)
								group.Templates.Add(GetSymbolTemplate(editor, defaultSymbol, null, null, layer.ID, null));
							else if (layer.Renderer is ClassBreaksRenderer)
							{
								var cbr = layer.Renderer as ClassBreaksRenderer;
								if (AlwaysDisplayDefaultTemplates && cbr.DefaultSymbol != null)
									group.Templates.Add(GetSymbolTemplate(editor, cbr.DefaultSymbol, null, null, layer.ID, null));
							}
						}
						if (group.Templates.Count > 0)
							templateGroups.Add(group);
					}
				}
			}
			TemplateGroups = templateGroups;
		}

		private static SymbolTemplate GetSymbolTemplate(Editor editor, Symbol symbol, object featureTypeID, FeatureTemplate template,
			string displayName, string description)
		{
			object type = null;
			if (template != null)
				type = template;
			else if (featureTypeID != null)
				type = featureTypeID;			
			SymbolTemplate symbolTemplate = new SymbolTemplate()
			{
				TypeID = type,
				FeatureTemplate = template,
				Symbol = symbol,
				Name = displayName,
				Description = description,
				Editor = editor
			};
			return symbolTemplate;
		}

		private static IEnumerable<SymbolTemplate> GetTemplates(IEnumerable<TemplateGroup> groups)
		{
			foreach (TemplateGroup item in groups)
			{
				foreach (SymbolTemplate template in item.Templates)
				{
					yield return template;
				}
			}
		}

		internal static void ShowAttributeForm(FeatureLayer featureLayer, Graphic graphic)
		{
#if SILVERLIGHT
			ChildWindow window = new ChildWindow();
#else
            Window window = new Window();
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
#endif
			FeatureDataForm form = new FeatureDataForm()
			{
				GraphicSource = graphic,
				FeatureLayer = featureLayer,
				IsReadOnly = !featureLayer.IsUpdateAllowed(graphic),
				MaxWidth = 500,
				MaxHeight = 500
			};
			window.Content = form;
			form.EditEnded += (s, e) => { window.Close(); };
#if SILVERLIGHT
			window.Show();
#else           
            window.ShowDialog();
#endif
		}
		#endregion

		private void OnEditCompleted(ESRI.ArcGIS.Client.Editor.EditEventArgs args)
		{
			EventHandler<ESRI.ArcGIS.Client.Editor.EditEventArgs> handler = EditCompleted;
			if (handler != null)
				handler(this, args);
		}

		/// <summary>
		/// Occurs when an edit has completed.
		/// </summary>
		public event EventHandler<ESRI.ArcGIS.Client.Editor.EditEventArgs> EditCompleted;

		private void OnEditorActivated(ESRI.ArcGIS.Client.Editor.CommandEventArgs args)
		{
			EventHandler<ESRI.ArcGIS.Client.Editor.CommandEventArgs> handler = EditorActivated;
			if (handler != null)
				handler(this, args);
		}

		/// <summary>
		/// Occurs when an editor has been activated.
		/// </summary>
		public event EventHandler<ESRI.ArcGIS.Client.Editor.CommandEventArgs> EditorActivated;
	}

	/// <summary>
	/// A group of templates representing a layer in the template picker
	/// </summary>
	public sealed class TemplateGroup
	{
		internal TemplateGroup() { }

		/// <summary>
		/// Layer name.
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// Layer.
		/// </summary>
		public Layer Layer { get; internal set; }

		/// <summary>
		/// The templates in a layer.
		/// </summary>
		public IList<SymbolTemplate> Templates { get; internal set; }

		/// <summary>
		/// Returns the name of the layer.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// The template in a template picker layer
	/// </summary>
	public sealed class SymbolTemplate : INotifyPropertyChanged
	{
		Editor editor;
		internal SymbolTemplate() { }

		/// <summary>
		/// The name of the template 
		/// </summary>
		public string Name { get; internal set; }
		/// <summary>
		/// The feature type id of the template
		/// </summary>
		public object TypeID { get; internal set; }
		/// <summary>
		/// The feature template.
		/// </summary>
		public FeatureTemplate FeatureTemplate { get; internal set; }
		/// <summary>
		/// The symbol for the template
		/// </summary>
		public Symbol Symbol { get; internal set; }
		/// <summary>
		/// The editor that executes the Add command
		/// </summary>			
		public Editor Editor
		{
			get { return editor; }
			internal set
			{
				editor = value;
				RaisePropertyChanged("Editor");
			}
		}
		/// <summary>
		/// The symbol description
		/// </summary>			
		public string Description { get; internal set; }
		/// <summary>
		/// Returns the name of the symbolTemplate.
		/// </summary>
		/// <returns>The name of the symbolTemplate</returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		internal void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
