using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
	/// <summary>
	/// Internal class encapsulating a layer item representing the virtual root item for the legend tree.
	/// The LayerItems collection of this item is the collection of map layer item displayed at the first level of the TOC.
	/// This class manages the events coming from the map, from the map layers and from the map layer items.
	/// </summary>
	internal sealed class LegendTree : LayerItemViewModel
	{
		#region Constructor
		public LegendTree()
		{
			LayerItemsOptions = new LayerItemsOpts(returnMapLayerItems: false, returnGroupLayerItems: false,
			                                       returnLegendItems: false,
			                                       showOnlyVisibleLayers: true, reverseLayersOrder: false);
			Attach(this);
		}
		~LegendTree()
		{
			Detach();
		} 
		#endregion

		#region LegendItemTemplate
		/// <summary>
		/// Gets or sets the legend item template.
		/// </summary>
		/// <value>The legend item template.</value>
		private DataTemplate _legendItemTemplate;
		internal DataTemplate LegendItemTemplate
		{
			get
			{
				return _legendItemTemplate;
			}
			set
			{
 				if (_legendItemTemplate != value)
				{
					_legendItemTemplate = value;
					PropagateTemplate();
					UpdateLayerItemsOptions();
				}
			}
		}
		#endregion

		#region LayerTemplate
		private DataTemplate _layerTemplate;
		/// <summary>
		/// Gets or sets the layer template i.e. the template used to display a layer in the legend.
		/// </summary>
		/// <value>The layer template.</value>
		internal DataTemplate LayerTemplate
		{
			get
			{
				return _layerTemplate;
			}
			set
			{
 				if (_layerTemplate != value)
				{
					_layerTemplate = value;
					PropagateTemplate();
				}
			}
		}
		#endregion

		#region MapLayerTemplate
		private DataTemplate _mapLayerTemplate;
		/// <summary>
		/// Gets or sets the map layer template.
		/// </summary>
		/// <value>The map layer template.</value>
		internal DataTemplate MapLayerTemplate
		{
			get
			{
				return _mapLayerTemplate;
			}
			set
			{
 				if (_mapLayerTemplate != value)
				{
					_mapLayerTemplate = value;
					PropagateTemplate();
				}
			}
		}
		#endregion

		#region Map
		private Map _map;
		private LayerCollection _oldLayers = null; // to be able to unhook when Layers changes
		/// <summary>
		/// Gets or sets the map that the legend control is buddied to.
		/// </summary>
		/// <value>The map.</value>
		internal Map Map
		{
			get
			{
				return _map;
			}
			set
			{
				if (_map != value)
				{
					if (_map != null)
					{
						_map.PropertyChanged -= Map_PropertyChanged;
						_map.ExtentChanged -= Map_ExtentChanged;
						if (_map.Layers != null)
						{
							_map.Layers.CollectionChanged -= Layers_CollectionChanged;
							foreach (var l in _map.Layers)
								l.PropertyChanged -= Layer_PropertyChanged;
						}
					}

					_map = value;

					if (_map != null)
					{
						_map.PropertyChanged += Map_PropertyChanged;
						_map.ExtentChanged += Map_ExtentChanged;
						if (_map.Layers != null)
						{
							_map.Layers.CollectionChanged += Layers_CollectionChanged;
							foreach (var l in _map.Layers)
								l.PropertyChanged += Layer_PropertyChanged;
						}
					}
					UpdateMapLayerItems();
				}
			}
		}

		#endregion

		#region LayerIDs
		private IEnumerable<string> _layerIDs = null;

		/// <summary>
		/// Gets or sets the layer IDs of the layers participating in the legend.
		/// </summary>
		/// <remarks>
		/// Specified in XAML and in Blend as a comma-delimited string: If a layer 
		/// name contains a comma, please use &#44; instead of the comma.
		/// If null/empty, legend from all layers is generated. Order of 
		/// the layer ids is respected in generating the legend.
		/// </remarks>
		/// <value>The layer IDs.</value>
		internal IEnumerable<string> LayerIDs
		{
			get
			{
				return _layerIDs;
			}

			set
			{
				if (_layerIDs != value)
				{
					_layerIDs = value;
					UpdateMapLayerItems();
				}
			}
		}
		#endregion

		#region ShowOnlyVisibleLayers
		private bool _showOnlyVisibleLayers = true;
		/// <summary>
		/// Gets or sets a value indicating whether only the visible layers are participating to the legend.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if only the visible layers are participating to the legend; otherwise, <c>false</c>.
		/// </value>
		internal bool ShowOnlyVisibleLayers
		{
			get
			{
				return _showOnlyVisibleLayers;
			}
			set
			{
				_showOnlyVisibleLayers = value;
				LayerItemsOpts mode = LayerItemsOptions;
				mode.ShowOnlyVisibleLayers = value;
				PropagateLayerItemsOptions(mode);
			}
		}
		#endregion

		#region ReverseLayersOrder
		private bool _reverseLayersOrder = false;
		/// <summary>
		/// Gets or sets a value indicating whether only the visible layers are participating to the legend.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if only the visible layers are participating to the legend; otherwise, <c>false</c>.
		/// </value>
		internal bool ReverseLayersOrder
		{
			get
			{
				return _reverseLayersOrder;
			}
			set
			{
				_reverseLayersOrder = value;
				LayerItemsOpts mode = LayerItemsOptions;
				mode.ReverseLayersOrder = value;
				PropagateLayerItemsOptions(mode);
			}
		}
		#endregion

		#region Refresh
		/// <summary>
		/// Refreshes the legend control.
		/// </summary>
		/// <remarks>Note : In most cases, the control is always up to date without calling the refresh method.</remarks>
		internal void Refresh()
		{
			// refresh all map layer items (due to group layers we have to go through the legend hierarchy
			LayerItems.Descendants(item => item.LayerItems).OfType<MapLayerItem>().ForEach(mapLayerItem => mapLayerItem.Refresh());
		}
		#endregion

		#region Event Refreshed
		/// <summary>
		/// Occurs when the legend is refreshed. 
		/// Give the opportunity for an application to add or remove legend items.
		/// </summary>
		internal event EventHandler<Legend.RefreshedEventArgs> Refreshed;

		internal void OnRefreshed(object sender, Legend.RefreshedEventArgs args)
		{
			EventHandler<Legend.RefreshedEventArgs> refreshed = Refreshed;

			if (refreshed != null)
			{
				refreshed(sender, args);
			}
		}
		#endregion

		#region Map Event Handlers

		private void Map_ExtentChanged(object sender, ExtentEventArgs e)
		{
			if (e.NewExtent != null)
			{
				if (e.OldExtent == null || e.NewExtent.Height != e.OldExtent.Height || e.NewExtent.Width != e.OldExtent.Width)
				{
					UpdateLayerVisibilities();
				}
			}
		}

		private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Map map = sender as Map;
			if (map == null)
				return;

			if (e.PropertyName == "Layers")
			{
				if (_oldLayers != null)
					_oldLayers.CollectionChanged -= Layers_CollectionChanged;

				_oldLayers = map.Layers;
				if (_oldLayers != null)
					_oldLayers.CollectionChanged += Layers_CollectionChanged;

				UpdateMapLayerItems();
			}
			else if (e.PropertyName == "TimeExtent")
			{
				// May change some layer visibilities for layer managing TimeExtent
				UpdateLayerVisibilities();
			}
		}

		private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var l = item as Layer;
					l.PropertyChanged -= Layer_PropertyChanged;
				}
			}
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var l = item as Layer;
					l.PropertyChanged += Layer_PropertyChanged;
				}
			}
			UpdateMapLayerItems();
		}

		void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ShowLegend")
				UpdateMapLayerItems();
		}

		#endregion

		#region Propagate methods propagating a property to all legend items of the legend tree
		private void PropagateTemplate()
		{
			// set the template on all descendants including the legend items
			LayerItems.Descendants(item => item.LayerItems).ForEach(item =>
			{
				item.Template = item.GetTemplate();
				item.LegendItems.ForEach(legendItem => legendItem.Template = legendItem.GetTemplate());
			});
		}

		private void PropagateLayerItemsOptions(LayerItemsOpts layerItemsOptions)
		{

			if (!LayerItemsOptions.Equals(layerItemsOptions))
			{
				DeferLayerItemsSourceChanged = true;
				LayerItemsOptions = layerItemsOptions;
				// set value on all descendants
				LayerItems.Descendants(layerItem => layerItem.LayerItems).ForEach(layerItem => layerItem.LayerItemsOptions = layerItemsOptions);
				DeferLayerItemsSourceChanged = false;
			}
		} 
		#endregion

		#region Private Methods


		/// <summary>
		/// Gets the layers at the first level of the legend control.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <param name="map">The map.</param>
		/// <returns></returns>
		private static IEnumerable<Layer> GetLayers(IEnumerable<string> ids, Map map)
		{
			if (map != null && map.Layers != null)
			{
				if (ids != null)
				{
					foreach (string item in ids)
					{
						if (!string.IsNullOrEmpty(item) && map.Layers[item] != null)
							yield return map.Layers[item];
					}
				}
				else
				{
					foreach (Layer layer in map.Layers)
					{
						yield return layer;
					}
				}
			}
		}

		private void UpdateMapLayerItems()
		{
			ObservableCollection<LayerItemViewModel> mapLayerItems = new ObservableCollection<LayerItemViewModel>();

			foreach (Layer layer in GetLayers(LayerIDs, Map))
			{
				if (layer.ShowLegend)
				{
					MapLayerItem mapLayerItem = FindMapLayerItem(layer);

					if (mapLayerItem == null) // else reuse existing map layer item to avoid query again the legend and to keep the current state (selected, expansed, ..)
					{
						// Create a new map layer item
						mapLayerItem = new MapLayerItem(layer) { LegendTree = this };
						mapLayerItem.Refresh();
					}

					mapLayerItems.Add(mapLayerItem);
				}
			}
			LayerItems = mapLayerItems;
		}

		private IEnumerable<MapLayerItem> MapLayerItems
		{
			get
			{
				if (LayerItems == null)
					return null;

				return LayerItems.OfType<MapLayerItem>();
			}
		}

		private MapLayerItem FindMapLayerItem(Layer layer)
		{
			return MapLayerItems == null ? null : MapLayerItems.FirstOrDefault(mapLayerItem => mapLayerItem.Layer == layer);
		}

		internal void UpdateLayerVisibilities()
		{
			LayerItems.ForEach(layerItem =>
				{
					layerItem.DeferLayerItemsSourceChanged = true;
					layerItem.UpdateLayerVisibilities(true, true, true);
					layerItem.DeferLayerItemsSourceChanged = false;
				}
			);
		}
		#endregion

		#region LayerItemsMode

		private Legend.Mode _layerItemsMode = Legend.Mode.Flat;
		internal Legend.Mode LayerItemsMode
		{
			get
			{
				return _layerItemsMode;
			}
			set
			{
				if (value != _layerItemsMode)
				{
					_layerItemsMode = value;
					UpdateLayerItemsOptions();
				}
			}
		}

		private void UpdateLayerItemsOptions()
		{
			LayerItemsOpts layerItemsOptions;
			bool returnsLegendItems = (LegendItemTemplate != null);

			switch (LayerItemsMode)
			{
				case Legend.Mode.Tree:
					layerItemsOptions = new LayerItemsOpts(true, true, returnsLegendItems, ShowOnlyVisibleLayers, ReverseLayersOrder);
					break;

				default:
					layerItemsOptions = new LayerItemsOpts(false, false, returnsLegendItems, ShowOnlyVisibleLayers, ReverseLayersOrder);
					break;
			}

			PropagateLayerItemsOptions(layerItemsOptions);
		}

		#endregion

	}
}
