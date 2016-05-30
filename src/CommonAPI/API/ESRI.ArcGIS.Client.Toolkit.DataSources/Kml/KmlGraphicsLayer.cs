// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see https://opensource.org/licenses/ms-pl for details.
// All other rights reserved.

using System;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
	/// <summary>
	/// KML GraphicsLayer subclass for allowing legend based on the styles
	/// </summary>
	internal class KmlGraphicsLayer : GraphicsLayer
	{
		#region ILegendSupport Members

		private bool _hasLegendChanged; // indicates whether the renderer changed during call to QueryLegendInfos
		internal bool IsHidden { get; set; } // Flag indicating that this layer should be hidden in the legend

		internal LayerLegendInfo LegendInfo { get; set; } // legend based on the style. 

		private IRenderer _rendererBasedOnStyle;
		internal IRenderer RendererBasedOnStyle
		{
			get { return _rendererBasedOnStyle; }
			set 
			{
				if (_rendererBasedOnStyle != value)
				{
					if (_rendererBasedOnStyle is ILegendSupport)
						((ILegendSupport)_rendererBasedOnStyle).LegendChanged -= RendererLegendChanged;
					_rendererBasedOnStyle = value;
					if (_rendererBasedOnStyle is ILegendSupport)
						((ILegendSupport)_rendererBasedOnStyle).LegendChanged += RendererLegendChanged;
					RendererLegendChanged(null, null);
				}
			}
		}

		void RendererLegendChanged(object sender, EventArgs e)
		{
			LegendInfo = null;
			_hasLegendChanged = true;
			OnLegendChanged();
		}

		/// <summary>
		/// Queries for the legend infos of the layer.
		/// </summary>
		/// <remarks>
		/// The returned result is encapsulated in a <see cref="LayerLegendInfo" /> object.
		/// </remarks>
		/// <param name="callback">The method to call on completion.</param>
		/// <param name="errorCallback">The method to call in the event of an error.</param>
		public override void QueryLegendInfos(Action<LayerLegendInfo> callback, Action<Exception> errorCallback)
		{
			if (callback == null)
				return;

			// If a renderer has been set, use it for the legend
			if (Renderer != null)
				base.QueryLegendInfos(callback, errorCallback);
			else if (LegendInfo != null) // legend based on style has already been created
				callback(LegendInfo);
			else if (RendererBasedOnStyle is ILegendSupport) // first call ->legend needs to be created
			{
				_hasLegendChanged = false;
				((ILegendSupport) RendererBasedOnStyle).QueryLegendInfos(
					legendInfo =>
						{
							if (_hasLegendChanged)
							{
								// Meanwhile the renderer has changed so the current result is meaningless
								QueryLegendInfos(callback, errorCallback);
							}
							else
							{
								LegendInfo = legendInfo; // keep it for further calls
								LegendInfo.IsHidden = IsHidden;
								callback(LegendInfo);
							}
						},
					error =>
						{
							if (_hasLegendChanged)
							{
								// Meanwhile the renderer has changed so the current result is meaningless
								QueryLegendInfos(callback, errorCallback);
							}
							else
							{
								if (errorCallback != null)
									errorCallback(error);
							}
						});
			}
			else
			{
				callback(null);
			}
		}

		#endregion
	}
}
