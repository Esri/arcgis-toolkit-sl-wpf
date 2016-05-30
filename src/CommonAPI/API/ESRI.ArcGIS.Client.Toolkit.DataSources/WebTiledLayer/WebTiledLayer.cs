// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see https://opensource.org/licenses/ms-pl for details.
// All other rights reserved.

using System;
using System.Globalization;
using ESRI.ArcGIS.Client.Geometry;
using System.Windows;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{

	/// <summary>
	/// A WebTiledLayer is a tiled layer where the tiles are obtained directly from the provided URL, as opposed to requests made against a service.
	/// Typically, Web Tiled Layers are used as a basemap.
	/// <para>
	/// As a minimum, the property <seealso cref="TemplateUrl"/> must be defined.
	/// If the tileInfo is not specified then the spatial reference and tiling scheme of the layer is assumed to be in the web mercator projection and google/ms/esri web mercator tiling scheme.
	/// </para>
	/// </summary>
	/// <remarks>
	/// Tile layers are added to a web map as layer type: WebTiledLayer (exposed in the UI as a Tile Layer).
	/// </remarks>
	/// <seealso cref="OpenStreetMapLayer"/>
	public class WebTiledLayer : TiledMapServiceLayer, IAttribution
#if WINDOWS_PHONE
		, ITileCache
#endif
	{
		/// <summary>Simple constant used for full extent and tile origin specific to this projection.</summary>
		private const double CornerCoordinate = 20037508.3427892;
		/// <summary>ESRI Spatial Reference ID for Web Mercator.</summary>
		private const int Wkid = 102100;

		private string _templateUrl; // for being able to access it from a background thread
		private string[] _subDomains; // for being able to access it from a background thread

		/// <summary>
		/// Initializes a new instance of the <see cref="WebTiledLayer"/> class.
		/// </summary>
		public WebTiledLayer()
		{
			//This default layer's spatial reference
			SpatialReference = new SpatialReference(Wkid);
			//Full extent fo the layer
			base.FullExtent = new Envelope(-CornerCoordinate, -CornerCoordinate, CornerCoordinate, CornerCoordinate)
			{
				SpatialReference = SpatialReference
			};
			//Set up tile information. Each tile is 256x256px, 19 levels.
			TileInfo = new TileInfo
			{
				Height = 256,
				Width = 256,
				Origin = new MapPoint(-CornerCoordinate, CornerCoordinate) { SpatialReference = SpatialReference },
				SpatialReference = SpatialReference,
				Lods = new Lod[19]
			};
			//Set the resolutions for each level. Each level is half the resolution of the previous one.
			double resolution = CornerCoordinate * 2 / 256;
			for (int i = 0; i < TileInfo.Lods.Length; i++)
			{
				TileInfo.Lods[i] = new Lod { Resolution = resolution };
				resolution /= 2;
			}
		}

		/// <summary>
		/// Initializes the <see cref="WebTiledLayer"/> class.
		/// </summary>
		static WebTiledLayer()
		{
			CreateAttributionTemplate();
		}

		/// <summary>
		/// Initializes the resource.
		/// </summary>
		/// <remarks>
		/// 	<para>Override this method if your resource requires asyncronous requests to initialize,
		/// and call the base method when initialization is completed.</para>
		/// 	<para>Upon completion of initialization, check the <see cref="ESRI.ArcGIS.Client.Layer.InitializationFailure"/> for any possible errors.</para>
		/// </remarks>
		/// <seealso cref="ESRI.ArcGIS.Client.Layer.Initialized"/>
		/// <seealso cref="ESRI.ArcGIS.Client.Layer.InitializationFailure"/>
		public override void Initialize()
		{
			if (string.IsNullOrEmpty(TemplateUrl))
				InitializationFailure = new Exception("TemplateUrl must be set.");

			//Call base initialize to raise the initialization event
			base.Initialize();
		}
		
		/// <summary>
		/// Returns a URL to the specified tile in an WebTiledLayer.
		/// </summary>
		/// <param name="level">Layer level</param>
		/// <param name="row">Tile row</param>
		/// <param name="col">Tile column</param>
		/// <returns>URL to the tile image</returns>
		public override string GetTileUrl(int level, int row, int col)
		{
			// Select a subdomain based on level/row/column so that it will always
			// be the same for a specific tile. Multiple subdomains allows the user
			// to load more tiles simultaneously. To take advantage of the browser cache
			// the following expression also makes sure that a specific tile will always 
			// hit the same subdomain.
			if (level + col + row < 0 || _templateUrl == null) return null;

			string levelValue = null;
			if (LevelValues != null && LevelValues.Length > level)
				levelValue = LevelValues[level];
			if (string.IsNullOrEmpty(levelValue))
				levelValue = level.ToString(CultureInfo.InvariantCulture);

			string subDomain = _subDomains != null && _subDomains.Length > 0
				                   ? _subDomains[(level + col + row)%_subDomains.Length]
				                   : null;

			string url = _templateUrl.Replace("{level}", levelValue)
			                        .Replace("{row}", row.ToString(CultureInfo.InvariantCulture))
			                        .Replace("{col}", col.ToString(CultureInfo.InvariantCulture));

			if (subDomain != null)
				url = url.Replace("{subDomain}", subDomain);

			return url;
		}

		/// <summary>
		/// Gets or sets the template URL to the Web Tiled Layer.
		/// The template url contains a parameterized url.
		/// The template can contain the following templated parameters: {subDomain}, {level}, {row} and {col}.
		/// <para>
		/// Example of OpenStreetMap template URL: http://{subDomain}.tile.opencyclemap.org/cycle/{level}/{col}/{row}.png</para>
		/// </summary>
		/// <value>
		/// The template URL.
		/// </value>
		public string TemplateUrl
		{
			get { return (string)GetValue(TemplateUrlProperty); }
			set { SetValue(TemplateUrlProperty, value); }
		}

		/// <summary>
		/// Dependency property for <see cref="TemplateUrl"/>.
		/// </summary>
		public static readonly DependencyProperty TemplateUrlProperty =
			DependencyProperty.Register("TemplateUrl", typeof(string), typeof(WebTiledLayer), new PropertyMetadata(OnTemplateUrlPropertyChanged));

		private static void OnTemplateUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (WebTiledLayer)d;
			obj._templateUrl = e.NewValue as string;
			if (obj.IsInitialized)
				obj.Refresh();
		}


		/// <summary>
		/// Gets or sets the sub domains.
		/// The sub domain values are used to replace the {subDomain} parameter in the <see cref="TemplateUrl"/>.
		/// </summary>
		/// <value>
		/// The sub domains.
		/// </value>
		[System.ComponentModel.TypeConverter(typeof(StringToStringArrayConverter))]
		public string[] SubDomains
		{
			get { return (string[])GetValue(SubDomainsProperty); }
			set { SetValue(SubDomainsProperty, value); }
		}

		/// <summary>
		/// Dependency property for <see cref="SubDomains"/>.
		/// </summary>
		public static readonly DependencyProperty SubDomainsProperty =
			DependencyProperty.Register("SubDomains", typeof(string[]), typeof(WebTiledLayer), new PropertyMetadata(OnSubDomainsPropertyChanged));

		private static void OnSubDomainsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (WebTiledLayer)d;
			obj._subDomains = e.NewValue as string[];
			if (obj.IsInitialized)
				obj.Refresh();
		}


		/// <summary>
		/// Gets or sets the attribution to the Web Tiled Layer provider.
		/// </summary>
		/// <value>
		/// The copyright.
		/// </value>
		public string CopyrightText
		{
			get { return (string)GetValue(CopyrightTextProperty); }
			set { SetValue(CopyrightTextProperty, value); }
		}

		/// <summary>
		/// Dependency property for <see cref="CopyrightText"/>.
		/// </summary>
		public static readonly DependencyProperty CopyrightTextProperty =
			DependencyProperty.Register("CopyrightText", typeof(string), typeof(WebTiledLayer), null);

		/// <summary>
		/// Gets or sets the level values to use when replacing the {level} parameter in the TemplateUrl.
		/// This optional property is useful when the TemplateUrl is expecting non numeric level values.
		/// This is maily used for the WMTS layers that are converted to WebTiledLayers when added to a webmap. The WMTS layers may use non numeric level values.
		/// </summary>
		/// <value>
		/// The level values.
		/// </value>
		public string[] LevelValues { get; set; }

		/// <summary>
		/// Sets the full extent of the layer.
		/// </summary>
		/// <param name="extent">The extent.</param>
		public void SetFullExtent(Envelope extent)
		{
			FullExtent = extent;
		}


		/// <summary>
		/// Sets the tile scheme of the tile layer.
		/// If not set then assume standard web mercator tiling scheme.
		/// </summary>
		/// <param name="tileInfo">The tile info.</param>
		public void SetTileInfo(TileInfo tileInfo)
		{
			TileInfo = tileInfo;
			if (tileInfo.SpatialReference != null)
				SpatialReference = tileInfo.SpatialReference;
		}
		
		#region IAttribution Members
		private const string Template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
			<TextBlock Text=""{Binding CopyrightText}"" TextWrapping=""Wrap""/></DataTemplate>";

		private static DataTemplate _attributionTemplate;
		private static void CreateAttributionTemplate()
		{
#if SILVERLIGHT
			_attributionTemplate = System.Windows.Markup.XamlReader.Load(Template) as DataTemplate;
#else
			using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(Template)))
			{
				_attributionTemplate = System.Windows.Markup.XamlReader.Load(stream) as DataTemplate;
			}
#endif
		}

		/// <summary>
		/// Gets the attribution template of the layer.
		/// </summary>
		/// <value>The attribution template.</value>
		public DataTemplate AttributionTemplate
		{
			get { return _attributionTemplate; }
		}

		#endregion


#if WINDOWS_PHONE
		#region ITileCache Members

		bool ITileCache.PersistCacheAcrossSessions
		{
			get { return true; }
		}

		string ITileCache.CacheUid
		{
			get
			{
				return string.Format("ESRI.ArcGIS.Client.Toolkit.DataSources.WebTiledLayer_{0}", TemplateUrl);
			}
		}

		#endregion
#endif
	}
}
