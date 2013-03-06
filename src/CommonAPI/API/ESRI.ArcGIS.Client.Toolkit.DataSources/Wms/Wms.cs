// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	/// <summary>
	/// A layer for OGC Web Map Services.
	/// </summary>
	public class WmsLayer : DynamicMapServiceLayer, ILegendSupport, ISublayerVisibilitySupport, IAttribution
	{
		private string[] _layersArray;
		private string _proxyUrl;
		private static Version highestSupportedVersion = new Version(1, 3);
		// Coordinate system WKIDs in WMS 1.3 where X,Y (Long,Lat) is switched to Y,X (Lat,Long)
		private static int[,] LatLongCRSRanges = new int[,] { { 4001, 4999 },
						{2044, 2045},   {2081, 2083},   {2085, 2086},   {2093, 2093},
						{2096, 2098},   {2105, 2132},   {2169, 2170},   {2176, 2180},
						{2193, 2193},   {2200, 2200},   {2206, 2212},   {2319, 2319},
						{2320, 2462},   {2523, 2549},   {2551, 2735},   {2738, 2758},
						{2935, 2941},   {2953, 2953},   {3006, 3030},   {3034, 3035},
						{3058, 3059},   {3068, 3068},   {3114, 3118},   {3126, 3138},
						{3300, 3301},   {3328, 3335},   {3346, 3346},   {3350, 3352},
						{3366, 3366},   {3416, 3416},   {20004, 20032}, {20064, 20092},
						{21413, 21423}, {21473, 21483}, {21896, 21899}, {22171, 22177},
						{22181, 22187}, {22191, 22197}, {25884, 25884}, {27205, 27232},
						{27391, 27398}, {27492, 27492}, {28402, 28432}, {28462, 28492},
						{30161, 30179}, {30800, 30800}, {31251, 31259}, {31275, 31279},
						{31281, 31290}, {31466, 31700} };

		/// <summary>
		/// Initializes a new instance of the <see cref="WmsLayer"/> class.
		/// </summary>
		public WmsLayer()
			: base()
		{
		}

		static WmsLayer()
		{
			CreateAttributionTemplate(null);
		}

		#region Public Properties
		/// <summary>
		/// Required.  Gets or sets the URL to a WMS service endpoint.  
		/// For example, 
		/// http://sampleserver1.arcgisonline.com/ArcGIS/services/Specialty/ESRI_StatesCitiesRivers_USA/MapServer/WMSServer,
		/// http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }
		
		/// <summary>
		/// Gets or sets the image format being used by the service.
		/// </summary>
		/// <remarks>
		/// The image format must be a supported MimeType name, supported by the service and the framework.
		/// </remarks>
		/// <example>
		/// <code>
		/// myWmsLayer.ImageFormat = "image/png";
		/// </code>
		/// </example>
		public string ImageFormat
		{
			get { return (string)GetValue(ImageFormatProperty); }
			set { SetValue(ImageFormatProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ImageFormat"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ImageFormatProperty =
			DependencyProperty.Register("ImageFormat", typeof(string), typeof(WmsLayer), null);

		/// <summary>
		/// Gets a collection of image formats supported by the WMS service.
		/// </summary>
		/// <remarks>
		/// This property is only set after layer initialization completes and 
		/// <see cref="SkipGetCapabilities"/> is <c>false</c>.
		/// </remarks>
		public ReadOnlyCollection<string> SupportedImageFormats { get; private set; }

		/// <summary>
		/// Required. Gets or sets the unique layer ids in a WMS service.  
		/// Each id is a string value.  At least one layer id must be defined.   
		/// </summary>
		/// <value>A string array of layer ids.</value>
		[System.ComponentModel.TypeConverter(typeof(StringToStringArrayConverter))]
		public string[] Layers
		{
			get { return _layersArray; }
			set
			{
				_layersArray = value;
				OnLayerChanged();
				SetVisibleLayers();
				OnVisibilityChanged();
			}
		}

		/// <summary>
		/// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the client application and a 
  /// WMS service. Use a proxy service when the WMS service is not hosted on a site that provides a cross domain 
  /// policy file (clientaccesspolicy.xml or crossdomain.xml). 
  /// <ESRISILVERLIGHT>You can also use a proxy to convert png images to a bit-depth that supports transparency in Silverlight.</ESRISILVERLIGHT>
		/// </summary>
		/// <value>The proxy URL string.</value>
		public string ProxyUrl
		{
			get { return _proxyUrl; }
			set { _proxyUrl = value; }
		}
		/// <summary>
		/// Optional. Gets or sets the WMS version.  If SkipGetCapabilities property is set to true, this value determines version requested.  
		/// If SkipGetCapabilities is false, this value determines version to retrieve.  If no value specified, default value returned from 
		/// the site will be used.
		/// </summary>
		/// <value>The version string.</value>
		public string Version { get; set; }
		/// <summary>
		/// Optional. Gets or sets a value indicating whether to skip a request to get capabilities. 
		/// Default value is false.  Set SkipGetCapabilities if the site hosting the WMS service does not provide a
		/// cross domain policy file and you do not have a proxy page.  In this case, you must set the WMS service version.
		/// If true, the initial and full extent of the WMS layer will not be defined.
		/// </summary>
		public bool SkipGetCapabilities { get; set; }

		/// <summary>
		/// Optional. Gets or sets the map URL.		
		/// </summary>
		/// <value>The map URL.</value>
		public string MapUrl { get; set; }

		/// <summary>
		/// Gets or sets the title metadata for this service.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Gets the abstract metadata for this service.
		/// </summary>
		public string Abstract { get; private set; }
		/// <summary>
		/// Gets a list of layers available in this service.
		/// </summary>
		public IList<LayerInfo> LayerList
		{
			get
			{
				if (rootLayer == null)
				{
					rootLayer = new LayerInfo()
					{
						ChildLayers = new ObservableCollection<LayerInfo>()
					};
				}
				return rootLayer.ChildLayers;
			}
		}
		private LayerInfo rootLayer = null;

		private List<string> copyrightText = new List<string>();

		/// <summary>
		/// Gets or sets the supported spatial reference IDs.
		/// </summary>
		/// <value>The supported spatial reference IDs.</value>
		[System.ComponentModel.TypeConverter(typeof(StringToInt32ArrayConverter))]
		public int[] SupportedSpatialReferenceIDs { get; set; }

		#endregion

		#region IAttribution Members

		private static DataTemplate _attributionTemplate;
		private static void CreateAttributionTemplate(WmsLayer layer)
		{
			if (layer != null)
			{
				string template = string.Format(@"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
			<TextBlock Text=""{0}"" TextWrapping=""Wrap""/></DataTemplate>", string.Join(" ", layer.copyrightText.ToArray()));
#if SILVERLIGHT

				_attributionTemplate = System.Windows.Markup.XamlReader.Load(template) as DataTemplate;
#else
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(template)))
			{
				_attributionTemplate = System.Windows.Markup.XamlReader.Load(stream) as DataTemplate;
			}
#endif
				layer.OnPropertyChanged("AttributionTemplate");
			}
		}

		/// <summary>		
		/// Gets the attribution template of an WMS GetCapabilities XML scheme.		
		/// </summary>
		/// <value>The attribution template.</value>
		public DataTemplate AttributionTemplate
		{
			get { return _attributionTemplate; }
		}

		#endregion

		private bool initializing;
		/// <summary>
		/// Initializes this a WMS layer.  Calls GetCapabilities if SkipGetCapabilities is false. 
		/// </summary>
		public override void Initialize()
		{
			if (initializing || IsInitialized) return;
			initializing = true;

			if (SkipGetCapabilities)
			{
				// Give a subLayerID to all layers
				int subLayerID = 0;
				foreach (var layerInfo in Descendants(LayerList))
					layerInfo.SubLayerID = subLayerID++;

				// Init Visibility from the list of Layers
				SetVisibleLayers();

				if (ImageFormat == null) ImageFormat = "image/png";
				base.Initialize();
			}
			else
			{
				string wmsUrl = CreateUrl(Url,
						string.Format("service=WMS&request=GetCapabilities&version={0}",
						GetValidVersionNumber()));

				WebClient client = Utilities.CreateWebClient();
#if !SILVERLIGHT || WINDOWS_PHONE
                if (Credentials != null)
                    client.Credentials = Credentials;
#endif
#if !SILVERLIGHT
				if (ClientCertificate != null)
					(client as CompressResponseWebClient).ClientCertificate = ClientCertificate;
#endif
				client.DownloadStringCompleted += client_DownloadStringCompleted;
				client.DownloadStringAsync(Utilities.PrefixProxy(ProxyUrl, wmsUrl));
			}
		}

		private static string CreateUrl(string url, string querystring)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(url);
			if (!url.Contains('?'))
				sb.Append('?');
			else if (!url.EndsWith("&"))
				sb.Append('&');
			sb.Append(querystring);
			return sb.ToString();
		}


		private string GetValidVersionNumber()
		{
			try
			{
				Version providedVersion = new Version(Version);
				if (providedVersion <= highestSupportedVersion)
					return Version;
			}
			catch { }
			return "1.3.0";
		}

		private bool LowerThan13Version()
		{
			try
			{
				Version providedVersion = new Version(Version);
				return (providedVersion < highestSupportedVersion);
			}
			catch { }
			return true;
		}
		
		/// <summary>
		/// WMS LayerInfo
		/// </summary>
		public sealed class LayerInfo
		{
			/// <summary>
			/// Gets the name of the layer.
			/// </summary>
			/// <value>The name.</value>
			public string Name { get; internal set; }
			/// <summary>
			/// Gets the title of the layer.
			/// </summary>
			/// <value>The title.</value>
			public string Title { get; internal set; }
			/// <summary>
			/// Gets the abstract for the layer.
			/// </summary>
			/// <value>The abstract.</value>
			public string Abstract { get; internal set; }
			/// <summary>
			/// Gets or sets the attribution text for the layer.
			/// </summary>
			/// <value>The attribution text.</value>
			public AttributionInfo Attribution { get; internal set; }
			/// <summary>
			/// Gets the extent of the layer.
			/// </summary>
			/// <value>The extent.</value>
			public Envelope Extent { get; internal set; }
			/// <summary>
			/// Gets the child layers.
			/// </summary>
			/// <value>The child layers.</value>
			public IList<LayerInfo> ChildLayers { get; internal set; }

			internal int SubLayerID { get; set; }
			internal bool Visible { get; set; }
			internal double MaximumScale { get; set; }
			internal double MinimumScale { get; set; }
			internal string LegendUrl { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="LayerInfo"/> class.
			/// </summary>
			public LayerInfo() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="LayerInfo"/> class.
			/// </summary>
			/// <param name="name">The name.</param>
			/// <param name="title">The title.</param>
			/// <param name="legendUrl">The legend URL.</param>
			public LayerInfo(string name, string title, string legendUrl)
			{
				Name = name;
				Title = title;
				LegendUrl = legendUrl;
			}
		}

		internal void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (!CheckForError(e))
			{
				// Process capabilities file
#if WINDOWS_PHONE
				System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings(); 
				settings.DtdProcessing = System.Xml.DtdProcessing.Ignore;
				XDocument xDoc = null;
				using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(
					new System.IO.StringReader(e.Result), settings))
					xDoc = XDocument.Load(reader);
#else
				XDocument xDoc = XDocument.Parse(e.Result);
#endif
				ParseCapabilities(xDoc);
			}

			// Call initialize regardless of error
			base.Initialize();
		}

		/// <summary>
		/// Creates the list of layer infos from the layers XElement.
		/// </summary>
		/// <param name="layers">The layers XElement.</param>
		/// <param name="ns">The namespace name.</param>
		/// <param name="inheritedAttribution">The inherited attribution.</param>
		/// <returns></returns>
		private IList<WmsLayer.LayerInfo> CreateLayerInfos(XElement layers, string ns, string inheritedAttribution = "")
		{
			if (layers == null)
				return null;

			return (layers.Elements(XName.Get("Layer", ns)).Select(layer => CreateLayerInfo(layer, ns, inheritedAttribution)).ToList());
		}

		/// <summary>
		/// Creates the layer info from the layer XElement.
		/// </summary>
		/// <param name="layer">The layer XElement.</param>
		/// <param name="ns">The namespace name.</param>
		/// <param name="inheritedAttribution">The inherited attribution.</param>
		/// <returns></returns>
		private LayerInfo CreateLayerInfo(XElement layer, string ns, string inheritedAttribution = "")
		{
			LayerInfo layerInfo = new LayerInfo();
			layerInfo.Name = layer.Element(XName.Get("Name", ns)) == null ? null : layer.Element(XName.Get("Name", ns)).Value;
			layerInfo.Title = layer.Element(XName.Get("Title", ns)) == null ? null : layer.Element(XName.Get("Title", ns)).Value;
			layerInfo.Abstract = layer.Element(XName.Get("Abstract", ns)) == null ? null : layer.Element(XName.Get("Abstract", ns)).Value;

			layerInfo.Attribution = new AttributionInfo();
			var attribution = layer.Element(XName.Get("Attribution", ns)) == null ? null : layer.Element(XName.Get("Attribution", ns));
			if (attribution != null)
				layerInfo.Attribution.Title = attribution.Element(XName.Get("Title", ns)) == null ? inheritedAttribution : attribution.Element(XName.Get("Title", ns)).Value;
			else
				layerInfo.Attribution.Title = inheritedAttribution;

			layerInfo.ChildLayers = CreateLayerInfos(layer, ns, layerInfo.Attribution.Title); // recursive call for sublayers

			var style = layer.Element(XName.Get("Style", ns));
			if (style != null)
			{
				foreach (var legendUrl in style.Elements(XName.Get("LegendURL", ns)))
				{
					var format = legendUrl.Element(XName.Get("Format", ns));
					var onlineResource = legendUrl.Element(XName.Get("OnlineResource", ns));
					if (format != null && onlineResource != null)
					{
#if SILVERLIGHT
						if (format.Value != "image/png" && format.Value != "image/jpeg" && format.Value != "image/jpeg")
							continue;
#endif
						var href = onlineResource.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink"));
						if (href != null)
						{
							layerInfo.LegendUrl = href.Value;
							break;
						}
					}
				}
			}

			if (LowerThan13Version())
			{
				// Deal with ScaleHint
				var scaleHint = layer.Element(XName.Get("ScaleHint", ns));
				if (scaleHint != null)
				{
					var attribute = scaleHint.Attribute(XName.Get("max", ns));
					if (attribute != null)
					{
						double value;
						double.TryParse(attribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
						layerInfo.MinimumScale = ScaleHintToScale(value);
					}

					attribute = scaleHint.Attribute(XName.Get("min", ns));
					if (attribute != null)
					{
						double value;
						double.TryParse(attribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
						layerInfo.MaximumScale = ScaleHintToScale(value);
					}
				}
			}
			else
			{
				// deal with ScaleDenominator from 1.3.0
				var minScaleDenominator = layer.Element(XName.Get("MinScaleDenominator", ns));
				if (minScaleDenominator != null)
				{
					double value;
					double.TryParse(minScaleDenominator.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
					layerInfo.MaximumScale = value;
				}
				var maxScaleDenominator = layer.Element(XName.Get("MaxScaleDenominator", ns));
				if (maxScaleDenominator != null)
				{
					double value;
					double.TryParse(maxScaleDenominator.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
					layerInfo.MinimumScale = value;
				}
			}
			return layerInfo;
		}


		private void ParseCapabilities(XDocument xDoc)
		{
			string ns = xDoc.Root.Name.NamespaceName;
			if (xDoc.Root.Attribute("version") != null)
				Version = xDoc.Root.Attribute("version").Value;

			//Get service info
			var info = (from Service in xDoc.Descendants(XName.Get("Service", ns))
						select new
						{
							Title = Service.Element(XName.Get("Title", ns)) == null ? null : Service.Element(XName.Get("Title", ns)).Value,
							Abstract = Service.Element(XName.Get("Abstract", ns)) == null ? null : Service.Element(XName.Get("Abstract", ns)).Value
						}).First();
			if (info != null)
			{
				this.Title = info.Title;
				//OnPropertyChanged("Title");
				this.Abstract = info.Abstract;
				//OnPropertyChanged("Abstract");
			}
			//Get a list of layers
			var capabilities = xDoc.Descendants(XName.Get("Capability", ns)).FirstOrDefault();
			var layerList = CreateLayerInfos(capabilities, ns, null);
			rootLayer = layerList.FirstOrDefault();

			// Give a subLayerID to all layers
			int subLayerID = 0;
			foreach (var layerInfo in Descendants(LayerList))
				layerInfo.SubLayerID = subLayerID++;

			// Init Visibility from the list of Layers
			SetVisibleLayers();

			try
			{
				//Get endpoint for GetMap requests
				var request = (from c in capabilities.Descendants(XName.Get("Request", ns)) select c).First();
				var GetMaps = (from c in request.Descendants(XName.Get("GetMap", ns)) select c);
				var GetMap = (from c in GetMaps
							  where c.Descendants(XName.Get("Format", ns)).Select(t => (t.Value == "image/png" ||
								  t.Value == "image/jpeg")).Count() > 0
							  select c).First();
				var formats = (from c in GetMaps.Descendants(XName.Get("Format", ns)) select c);
				//Silverlight only supports png and jpg. Prefer PNG, then JPEG
				SupportedImageFormats = new ReadOnlyCollection<string>(new List<string>(from c in formats where c.Value != null select c.Value));
				OnPropertyChanged("SupportedImageFormats");
				if (ImageFormat == null)
				{
					foreach (string f in new string[] { "image/png", "image/jpeg", "image/jpg"
#if !SILVERLIGHT
						//for WPF after PNG and JPEG, Prefer GIF, then any image, then whatever is supported
						,"image/gif","image/"
#endif
					})
					{
						ImageFormat = (from c in SupportedImageFormats where c.StartsWith(f) select c).FirstOrDefault();
						if (ImageFormat != null) break;
					}
#if !SILVERLIGHT
					if(ImageFormat == null)
						ImageFormat = (from c in formats where c.Value != null select c.Value).FirstOrDefault();
#endif
				}
				var DCPType = (from c in GetMap.Descendants(XName.Get("DCPType", ns)) select c).First();
				var HTTP = (from c in DCPType.Descendants(XName.Get("HTTP", ns)) select c).First();
				var Get = (from c in HTTP.Descendants(XName.Get("Get", ns)) select c).First();
				var OnlineResource = (from c in Get.Descendants(XName.Get("OnlineResource", ns)) select c).First();
				var href = OnlineResource.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink"));				
				if (this.MapUrl == null)
					this.MapUrl = href.Value;
			}
			catch
			{   //Default to WMS url
				if (this.MapUrl == null)
					this.MapUrl = this.Url;
			}

			bool lowerThan13 = LowerThan13Version();
			List<int> supportedIDs = new List<int>();
			string key = lowerThan13 ? "SRS" : "CRS";
			IEnumerable<XElement> SRSs = xDoc.Descendants(XName.Get(key, ns));
			foreach (var element in SRSs)
			{
				if (element.Value != null && element.Value.StartsWith("EPSG:"))
				{
					try
					{
						int srid = int.Parse(element.Value.Replace("EPSG:", ""), CultureInfo.InvariantCulture);
						if (!supportedIDs.Contains(srid))
							supportedIDs.Add(srid);
					}
					catch { }
				}
			}
			SupportedSpatialReferenceIDs = supportedIDs.ToArray();

			//Get the full extent of all layers
			IEnumerable<XElement> elements = xDoc.Descendants(XName.Get("BoundingBox", ns));
			if (!elements.GetEnumerator().MoveNext() && lowerThan13)
			{
				var element = xDoc.Descendants(XName.Get("LatLonBoundingBox", ns)).First();
				this.SpatialReference = new Geometry.SpatialReference(4326);
				this.FullExtent = GetEnvelope(element, this.SpatialReference, true);
			}
			if (this.SpatialReference == null)
			{
				foreach (var element in elements)
				{
					if (element.Attribute(key) != null && element.Attribute(key).Value.StartsWith("EPSG:"))
					{
						try
						{
							int srid = int.Parse(element.Attribute(key).Value.Replace("EPSG:", ""), CultureInfo.InvariantCulture);
							this.SpatialReference = new Geometry.SpatialReference(srid);
						}
						catch { }
						this.FullExtent = GetEnvelope(element, this.SpatialReference, lowerThan13);
						break;
					}
				}
				if (this.FullExtent == null) //EPSG code not found. Default to first CRS
				{
					var element = elements.First();
					if (element.Attribute(key) != null)
					{
						string value = element.Attribute(key).Value;
						int idx = value.LastIndexOf(":");
						if (idx > -1) value = value.Substring(idx);
						try
						{
							int srid = int.Parse(value, CultureInfo.InvariantCulture);
							this.SpatialReference = new SpatialReference(srid);
						}
						catch { }
					}
					this.FullExtent = GetEnvelope(element, this.SpatialReference, lowerThan13);
				}
			}
		}

		private static Envelope GetEnvelope(XElement element, SpatialReference sref, bool lowerThan13)
		{
			bool useLatLon = !lowerThan13 && sref != null && UseLatLon(sref.WKID);
			Envelope extent = new Envelope(
			   element.Attribute("minx") == null ? double.MinValue : double.Parse(element.Attribute("minx").Value, CultureInfo.InvariantCulture),
			   element.Attribute("miny") == null ? double.MinValue : double.Parse(element.Attribute("miny").Value, CultureInfo.InvariantCulture),
			   element.Attribute("maxx") == null ? double.MaxValue : double.Parse(element.Attribute("maxx").Value, CultureInfo.InvariantCulture),
			   element.Attribute("maxy") == null ? double.MaxValue : double.Parse(element.Attribute("maxy").Value, CultureInfo.InvariantCulture)
			) { SpatialReference = sref };

			if (useLatLon)
				return new Envelope(extent.YMin, extent.XMin, extent.YMax, extent.XMax) { SpatialReference = sref };
			else
				return extent;
		}

		private bool CheckForError(DownloadStringCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				InitializationFailure = new Exception(Properties.Resources.WebRequest_Canceled);
				return true;
			}
			if (e.Error != null)
			{
				Exception ex = e.Error;
				if (ex is System.Security.SecurityException)
				{
					ex = new System.Security.SecurityException(
						string.Format(Properties.Resources.MapService_SecurityException, "WMS"),
						ex);
				}
				InitializationFailure = ex;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the URL. Override from DynamicMapServiceLayer
		/// </summary>
		/// <param name="extent">The extent.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="onComplete">OnUrlComplete delegate.</param>
		/// <remarks>
		/// The Map has a private method loadLayerInView which calls Layer.Draw.   
		/// The DynamicMapServiceLayer abstract class overrides the Draw method and calls 
		/// DynamicMapServiceLayer.GetUrl which must be implemented in a subclass.   
		/// The last parameter is the OnUrlComplete delegate, which is used to pass the appropriate values 
		/// (url, width, height, envelope) to the private DynamicMapServiceLayer.getUrlComplete method.
		/// </remarks>
		public override void GetUrl(ESRI.ArcGIS.Client.Geometry.Envelope extent, int width, int height,
			DynamicMapServiceLayer.OnUrlComplete onComplete)
		{
			int extentWKID = (extent.SpatialReference != null) ? extent.SpatialReference.WKID : 0;
			string baseUrl = MapUrl ?? Url;
			StringBuilder mapURL = new StringBuilder(baseUrl);

			if (!baseUrl.Contains("?"))
				mapURL.Append("?");
			else if (!baseUrl.EndsWith("&"))
				mapURL.Append("&");
			mapURL.Append("SERVICE=WMS&REQUEST=GetMap");
			mapURL.AppendFormat("&WIDTH={0}", width);
			mapURL.AppendFormat("&HEIGHT={0}", height);
			mapURL.AppendFormat("&FORMAT={0}", ImageFormat);
			mapURL.AppendFormat("&LAYERS={0}", Layers == null ? "" : String.Join(",", Layers));
			mapURL.Append("&STYLES=");
			mapURL.AppendFormat("&BGCOLOR={0}", "0xFFFFFF");
			mapURL.AppendFormat("&TRANSPARENT={0}", "TRUE");

			mapURL.AppendFormat("&VERSION={0}", GetValidVersionNumber());
			//If one of the WebMercator codes, change to a WKID supported by the service
			if (SupportedSpatialReferenceIDs != null &&
				(extentWKID == 102100 || extentWKID == 102113 || extentWKID == 3857 || extentWKID == 900913))
			{
				if (!SupportedSpatialReferenceIDs.Contains(extentWKID))
				{
					if (SupportedSpatialReferenceIDs.Contains(3857))
						extentWKID = 3857;
					else if (SupportedSpatialReferenceIDs.Contains(102100))
						extentWKID = 102100;
					else if (SupportedSpatialReferenceIDs.Contains(102113))
						extentWKID = 102113;
					else if (SupportedSpatialReferenceIDs.Contains(900913))
						extentWKID = 900913;
				}
			}
			if (LowerThan13Version())
			{
				mapURL.AppendFormat("&SRS=EPSG:{0}", extentWKID);
				mapURL.AppendFormat(CultureInfo.InvariantCulture,
						"&bbox={0},{1},{2},{3}", extent.XMin, extent.YMin, extent.XMax, extent.YMax);
			}
			else
			{
				mapURL.AppendFormat("&CRS=EPSG:{0}", extentWKID);
				if (UseLatLon(extentWKID))
					mapURL.AppendFormat(CultureInfo.InvariantCulture,
						"&BBOX={0},{1},{2},{3}", extent.YMin, extent.XMin, extent.YMax, extent.XMax);
				else
					mapURL.AppendFormat(CultureInfo.InvariantCulture,
						"&BBOX={0},{1},{2},{3}", extent.XMin, extent.YMin, extent.XMax, extent.YMax);
			}

			onComplete(Utilities.PrefixProxy(ProxyUrl, mapURL.ToString()).AbsoluteUri, width, height, new ESRI.ArcGIS.Client.Geometry.Envelope()
			{
				XMin = extent.XMin,
				YMin = extent.YMin,
				XMax = extent.XMax,
				YMax = extent.YMax
			});
		}

		private static bool UseLatLon(int extentWKID)
		{
			int length = LatLongCRSRanges.Length / 2;
			for (int count = 0; count < length; count++)
			{
				if (extentWKID >= LatLongCRSRanges[count, 0] && extentWKID <= LatLongCRSRanges[count, 1])
					return true;
			}
			return false;
		}


		#region ILegendSupport Members
		LayerLegendInfo _layerLegendInfo = null; // save info to avoid new web request

		/// <summary>
		/// Queries for the legend infos of a layer.
		/// </summary>
		/// <remarks>
		/// The returned result is encapsulated in a <see cref="LayerLegendInfo" /> object.
		/// This object represents the legend of the map service layer and contains a collection of LayerLegendInfos (one by sublayer)
		/// </remarks>
		/// <param name="callback">The method to call on completion.</param>
		/// <param name="errorCallback">The method to call in the event of an error.</param>
		public void QueryLegendInfos(Action<LayerLegendInfo> callback, Action<Exception> errorCallback)
		{
			if (callback == null)
				return;

			if (_layerLegendInfo == null && IsInitialized)
			{
				// Create legend tree from the LayerList
				_layerLegendInfo = new LayerLegendInfo
				{
					LayerLegendInfos = CreateLegendInfos(LayerList),
					LayerName = Title,
					LayerDescription = Abstract,
					LayerType = "WMS Layer",
					SubLayerID = 0
				};
			}

			callback(_layerLegendInfo);
		}

		/// <summary>
		/// Creates the list of legend infos from the layer infos.
		/// </summary>
		/// <param name="layerInfos">The layer infos.</param>
		/// <returns></returns>
		private static IEnumerable<LayerLegendInfo> CreateLegendInfos(IEnumerable<WmsLayer.LayerInfo> layerInfos)
		{
			if (layerInfos == null)
				return null;

			return layerInfos.Select(CreateLegendInfo).ToList();
		}

		/// <summary>
		/// Creates the legend info from a layer info.
		/// </summary>
		/// <param name="layerInfo">The layer info.</param>
		/// <returns></returns>
		private static LayerLegendInfo CreateLegendInfo(WmsLayer.LayerInfo layerInfo)
		{
			List<LegendItemInfo> legendItemInfos = null;

			// If there is a legend url, create a legend item with the image
			if (!string.IsNullOrEmpty(layerInfo.LegendUrl))
			{
				legendItemInfos = new List<LegendItemInfo>
				{
					new LegendItemInfo
					{
						ImageSource = new BitmapImage(new Uri(layerInfo.LegendUrl))
					}
				};
			}

			return new LayerLegendInfo
			{
				LayerDescription = layerInfo.Abstract,
				LayerName = layerInfo.Title,
				SubLayerID = layerInfo.SubLayerID,
				LayerLegendInfos = CreateLegendInfos(layerInfo.ChildLayers),
				MaximumScale = layerInfo.MaximumScale,
				MinimumScale = layerInfo.MinimumScale,
				LegendItemInfos = legendItemInfos
			};
		}

#pragma warning disable 0067 // never used but needs to be implemented due to interface
		/// <summary>
		/// Occurs when the legend of the layer changed.
		/// </summary>
		/// <remarks>
		/// Actually, for this kind of layer, the legend never changes after initialization.
		/// </remarks>
		public event EventHandler<EventArgs> LegendChanged;
#pragma warning restore 0067

		#endregion

		#region ISupportLayerSubVisibility
		/// <summary>
		/// Gets the sub-layer visibility.
		/// </summary>
		/// <param name="layerID">The sub-layer ID.</param>
		/// <returns>The sub-layer visibility</returns>
		public bool GetLayerVisibility(int layerID)
		{
			return Descendants(LayerList).Where(info => info.SubLayerID == layerID).Select(info => info.Visible).FirstOrDefault();
		}

		/// <summary>
		/// Sets the sublayer visibility.
		/// </summary>
		/// <param name="layerID">The sublayer ID.</param>
		/// <param name="visible">The sublayer visibility.</param>
		public void SetLayerVisibility(int layerID, bool visible)
		{
			var layerInfo = Descendants(LayerList).Where(info => info.SubLayerID == layerID).FirstOrDefault();
			if (layerInfo != null && layerInfo.Visible != visible)
			{
				layerInfo.Visible = visible;

				_layersArray = GetVisibleLayers(LayerList).ToArray();
				SetAttribution();
				OnLayerChanged();
				OnVisibilityChanged();
			}
		}

		private static IEnumerable<string> GetVisibleLayers(IEnumerable<WmsLayer.LayerInfo> layerInfos)
		{
			foreach (var info in layerInfos.Where(info => info.Visible))
			{
				if (info.ChildLayers == null || info.ChildLayers.Count == 0)
					yield return info.Name;
				else
					foreach (var i in GetVisibleLayers(info.ChildLayers))
						yield return i;
			}
		}

		/// <summary>
		/// Occurs when the visibility of sublayers changed.
		/// </summary>
		public event EventHandler<EventArgs> VisibilityChanged;

		private void OnVisibilityChanged()
		{
			EventHandler<EventArgs> visibilityChanged = VisibilityChanged;
			if (visibilityChanged != null)
				visibilityChanged(this, EventArgs.Empty);
		}

		#endregion

		#region SetVisibleLayers

		/// <summary>
		/// Init the visibility of the layers from the visibleLayers array
		/// When a layer is in the array visibleLayers, it is visible (whatever the visibility of its parent) and all its descendants are visible
		/// so in the LayerTree we have to set the visibility for all ascendants and all descendants.
		/// </summary>
		internal void SetVisibleLayers()
		{
			// First pass : set all layers invisible
			foreach (var info in Descendants(LayerList))
				info.Visible = false;

			// Second pass : foreach layer in visibleLayers, set visible flag to all parents and all children
			foreach (var info in LayerList)
			{
				SetVisibleLayers(info);
			}
			SetAttribution();
		}

		private void SetAttribution()
		{
			copyrightText.Clear();
			BuildLayersAttribution(rootLayer);
			CreateAttributionTemplate(this);
		}

		private void BuildLayersAttribution(LayerInfo layerInfo)
		{
			if (Layers == null)
				return;

			if (Layers.Contains(layerInfo.Name))
			{
				string parentAttribution = layerInfo.Attribution == null ? null : layerInfo.Attribution.Title;
				foreach (var child in Descendants(layerInfo.ChildLayers))
				{
					string attribution = child.Attribution == null ? null : child.Attribution.Title;
					if (!string.IsNullOrEmpty(attribution) && !copyrightText.Contains(attribution))
						copyrightText.Add(attribution);
				}
				if (!string.IsNullOrEmpty(parentAttribution) && !copyrightText.Contains(parentAttribution))
					copyrightText.Add(parentAttribution);
			}
			else if (layerInfo.ChildLayers != null)
			{
				foreach (var child in layerInfo.ChildLayers)
				{
					BuildLayersAttribution(child);
				}
			}

		}

		private void SetVisibleLayers(WmsLayer.LayerInfo layerInfo)
		{
			if (Layers == null)
				return;

			bool visible = false;
			if (Layers.Contains(layerInfo.Name))
			{
				// the layer and all its children is visible
				visible = true;
				foreach (var child in Descendants(layerInfo.ChildLayers))
					child.Visible = true;
			}
			else if (layerInfo.ChildLayers != null)
			{
				foreach (var child in layerInfo.ChildLayers)
				{
					SetVisibleLayers(child);
					visible |= child.Visible; // if a child is visible, all ascendants must be visible
				}
			}
			layerInfo.Visible = visible;
		}

		private static IEnumerable<LayerInfo> Descendants(IEnumerable<LayerInfo> layerInfos)
		{
			if (layerInfos == null)
				yield break;

			foreach (var info in layerInfos)
			{
				yield return info;

				foreach (var child in Descendants(info.ChildLayers))
					yield return child;
			}
		}

		private static double ScaleHintToScale(double scaleHint)
		{
			const double inchesPerMeter = 10000.0 / 254.0; // = 39.37
			const double sqrt2 = 1.4142; // =Math.Sqrt(2.0)
			const int dpi = 96;
			const double ratio = dpi * inchesPerMeter / sqrt2;

			return scaleHint * ratio;
		}

		#endregion

	}

	/// <summary>
	/// Attribution class used to store copyright information.
	/// </summary>
	public class AttributionInfo
	{
		internal AttributionInfo() { }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; internal set; }

		// TODO : In the future we may need to support logo and resource link		
	}
}
