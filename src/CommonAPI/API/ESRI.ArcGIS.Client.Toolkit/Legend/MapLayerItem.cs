// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ESRI.ArcGIS.Client.Toolkit.Primitives;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// Internal class encapsulating a layer item representing a map layer.
	/// This class manages the events coming from the layer and the legend refresh process.
	/// </summary>
	internal sealed class MapLayerItem : LayerItemViewModel
	{
		#region Constructors
		internal const string c_mapLayerType = "MapLayer Layer";
		private bool _isQuerying = false;
		private Dispatcher _dispatcher = null;
		private double maxServiceResolution = 0.0; // Max and Min resolution coming from the service : to combine with max and min resolution coming from the layer
		private double minServiceResolution = 0.0;

		/// <summary>
		/// Initializes a new instance of the <see cref="MapLayerItem"/> class.
		/// </summary>
		/// <param name="layer">The layer.</param>
		internal MapLayerItem(Layer layer)
			: base(layer)
		{
			LayerType = c_mapLayerType;

			Label = layer.DisplayName ?? layer.ID;
			MinimumResolution = layer.MinimumResolution;
			MaximumResolution = layer.MaximumResolution;
			VisibleTimeExtent = layer.VisibleTimeExtent;
			IsVisible = layer != null ? layer.Visible : true;

			if (Application.Current != null
#if SILVERLIGHT
				&& Application.Current.RootVisual != null)
					_dispatcher = Application.Current.RootVisual.Dispatcher;
#else
				)
					_dispatcher = Application.Current.Dispatcher;
#endif
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MapLayerItem"/> class. Only useful in Design.
		/// </summary>
		internal MapLayerItem()
		{
			LayerType = c_mapLayerType;
		}

		#endregion

		#region Events Handler
		private void AttachLayerEventHandler(Layer layer)
		{
			Debug.Assert(layer != null);
			if (layer is ILegendSupport)
				(layer as ILegendSupport).LegendChanged += new EventHandler<EventArgs>(Layer_LegendChanged);

			if (layer is ISublayerVisibilitySupport)
				(layer as ISublayerVisibilitySupport).VisibilityChanged += new EventHandler<EventArgs>(Layer_VisibilityChanged);

			layer.PropertyChanged += new PropertyChangedEventHandler(Layer_PropertyChanged);

			layer.Initialized += new EventHandler<EventArgs>(Layer_Initialized);
		}

		private void DetachLayerEventHandler(Layer layer)
		{
			if (layer != null)
			{
				if (layer is ILegendSupport)
					(layer as ILegendSupport).LegendChanged -= new EventHandler<EventArgs>(Layer_LegendChanged);

				if (layer is ISublayerVisibilitySupport)
					(layer as ISublayerVisibilitySupport).VisibilityChanged -= new EventHandler<EventArgs>(Layer_VisibilityChanged);

				layer.PropertyChanged -= new PropertyChangedEventHandler(Layer_PropertyChanged);
				layer.Initialized -= new EventHandler<EventArgs>(Layer_Initialized);
			}
		}

		void Layer_Initialized(object sender, EventArgs e)
		{
			if (!(sender is GroupLayerBase)) // For group layers, we don't wait for initialized event 
				Refresh();
		}

		private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Layer layer = sender as Layer;
			if (layer == null)
				return;

			if (e.PropertyName == "MinimumResolution")
			{
				MinimumResolution = Math.Max(layer.MinimumResolution, minServiceResolution);
				if (LegendTree != null)
					LegendTree.UpdateLayerVisibilities();
			}
			else if (e.PropertyName == "MaximumResolution")
			{
				MaximumResolution = Math.Min(layer.MaximumResolution, maxServiceResolution);
				if (LegendTree != null)
					LegendTree.UpdateLayerVisibilities();
			}
			else if (e.PropertyName == "VisibleTimeExtent")
			{
				VisibleTimeExtent = layer.VisibleTimeExtent;
				if (LegendTree != null)
					LegendTree.UpdateLayerVisibilities();
			}
			else if (e.PropertyName == "Visible")
			{
				if (LegendTree != null)
					LegendTree.UpdateLayerVisibilities();
			}
			else if (e.PropertyName == "ID")
			{
				if (!string.IsNullOrEmpty(layer.ID))
					Label = layer.ID;
			}
		}

		private void Layer_LegendChanged(object sender, EventArgs e)
		{
			// Structure of legend items has changed -> refresh
			Refresh();
		}

		private void Layer_VisibilityChanged(object sender, EventArgs e)
		{
			// Visibility of sublayers has changed --> update layer visibility of legend items
			if (LegendTree != null)
				LegendTree.UpdateLayerVisibilities();
		}

		#endregion

		#region Refresh
		/// <summary>
		/// Refreshes the legend from infos coming from the map layer.
		/// </summary>
		internal void Refresh()
		{
			if (_isQuerying || Layer == null)
				return; // already querying

			if (!(Layer is GroupLayerBase)) // GroupLayer : don't wait for layer intialized, so the user will see the layer hierarchy even if the group layer is not initialized yet (else would need to wait for all sublayers initialized)
			{
				if (!Layer.IsInitialized) 
				{
					IsBusy = true; // set busy indicator waiting for layer initialized
					return; // Refresh will be done on event Initialized
				}

				LayerItems = null;
			}
			LegendItems = null;

			if (Layer is ILegendSupport)
			{
				IsBusy = true;
				_isQuerying = true;
				ILegendSupport legendSupport = Layer as ILegendSupport;

				Action queryLegendInfosAction = delegate()
				{
					legendSupport.QueryLegendInfos(
						// callback
						(result) =>
						{
							if (LegendTree == null)
							{
								// The legend item has been detached ==> result no more needed
								IsBusy = _isQuerying = false;
								return;
							}

							if (result != null)
							{
								Description = result.LayerDescription;
								if (string.IsNullOrEmpty(Label)) // Label is set with LayerID : keep it if not null
									Label = result.LayerName;

								Map map = LegendTree.Map;

								// Convert scale to resolution
								maxServiceResolution = result.MinimumScale == 0.0 ? double.PositiveInfinity : ConvertToResolution(result.MinimumScale, map);
								minServiceResolution = ConvertToResolution(result.MaximumScale, map);

								// Combine Layer and Service resolution
								MinimumResolution = Math.Max(Layer.MinimumResolution, minServiceResolution);
								MaximumResolution = Math.Min(Layer.MaximumResolution, maxServiceResolution);

								IsHidden = result.IsHidden;

								if (result.LayerLegendInfos != null)
								{
									LayerItems = result.LayerLegendInfos.Select(info => new LayerItemViewModel(Layer, info, Description, map)).ToObservableCollection();
								}

								if (result.LegendItemInfos != null)
								{
									LegendItems = result.LegendItemInfos.Select(info => new LegendItemViewModel(info)).ToObservableCollection();
								}
							}

							// If groupLayer -> add the child layers 
							AddGroupChildLayers();

							// Kml layer particular case : if a KML layer has only a child which is not another KML layer ==> set the child item as transparent so it doesn't appear in the legend
							ProcessKmlLayer();

							LegendTree.UpdateLayerVisibilities();
							_isQuerying = false;
							// Fire event Refreshed without exception
							LegendTree.OnRefreshed(this, new Legend.RefreshedEventArgs(this, null));

							if (_dispatcher != null)
								_dispatcher.BeginInvoke(new Action(() => IsBusy = false));
							else
								IsBusy = false;
						},

						// errorCallback
						(ex) =>
						{
							// Fire event Refreshed with an exception
							if (LegendTree != null)
								LegendTree.OnRefreshed(this, new Legend.RefreshedEventArgs(this, ex));

							_isQuerying = false;
							IsBusy = false;
						}
					);
				};

				if (_dispatcher != null)
					_dispatcher.BeginInvoke(queryLegendInfosAction);
				else
					queryLegendInfosAction();
			}
			else
			{
				IsBusy = false;
				// Fire event Refreshed
				if (LegendTree != null)
					LegendTree.OnRefreshed(this, new Legend.RefreshedEventArgs(this, null));

			}
		}
		
		private void AddGroupChildLayers()
		{
			if (Layer is GroupLayerBase)
			{
				ObservableCollection<LayerItemViewModel> mapLayerItems = new ObservableCollection<LayerItemViewModel>();
				if ((Layer as GroupLayerBase).ChildLayers != null)
				{
					foreach (Layer layer in (Layer as GroupLayerBase).ChildLayers)
					{
						Layer layerToFind = layer;
						MapLayerItem mapLayerItem = LayerItems == null ? null : LayerItems.FirstOrDefault(item => item.Layer == layerToFind) as MapLayerItem;

						if (mapLayerItem == null || mapLayerItems.Contains(mapLayerItem)) // else reuse existing map layer item to avoid querying again the legend and lose the item states (note : contains test if for the degenerated case where a layer is twice or more in a group layer)
						{
							// Create a new map layer item
							mapLayerItem = new MapLayerItem(layer) { LegendTree = this.LegendTree };
							if (_dispatcher != null)
								_dispatcher.BeginInvoke(new Action(() => mapLayerItem.Refresh()));
							else
								mapLayerItem.Refresh();
						}
						mapLayerItems.Add(mapLayerItem);
					}
				}
				LayerItems = mapLayerItems;
#if !SILVERLIGHT
				// Don't display the AcceleratedDisplayLayers root node
				if (Layer is AcceleratedDisplayLayers)
					IsTransparent = true;
#endif
			}
		}

		// KML Layer case : if a KML layer has only a child which is not another KML layer ==> set the child item as transparent so it doesn't appear in the legend
		private void ProcessKmlLayer()
		{
			// Must be a group layer (KmlLayer inherits from GroupLayer)
			if (!(Layer is GroupLayer))
				return;

			// Must have only one child
			LayerCollection layers = (Layer as GroupLayer).ChildLayers;
			if (layers == null || layers.Count() != 1)
				return;

			// The child must not be a KMLLayer i.e. not a group layer (sub folder and sub document must not be removed from the legend)
			Layer childLayer = layers.FirstOrDefault();
 			if (childLayer is GroupLayer)
				return;

			// The layer must be a KmlLayer
			if (!IsKmlLayer())
				return;

			// Set the child as transparent
			LayerItemViewModel childLayerItem = LayerItems == null ? null : LayerItems.FirstOrDefault();
			if (childLayerItem != null)
				childLayerItem.IsTransparent = true;
		}

		// check that the layer inherits from kmllayer by using name in order to avoid a reference to DataSources project
		private bool IsKmlLayer()
		{
			Type type = Layer.GetType();
			while (type != null)
			{
				if ( type.Name == "KmlLayer")
					return true;
				type = type.BaseType;
			}
			return false;
		}

		#endregion

		#region Attach/Detach

		internal override void Attach(LegendTree legendTree)
		{
			if (legendTree == null)
				return;

			base.Attach(legendTree);

			AttachLayerEventHandler(Layer);
		}

		internal override void Detach()
		{
			DetachLayerEventHandler(Layer);

			base.Detach();
		}
		#endregion
	}
}
