// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using ESRI.ArcGIS.Client.Geometry;
using System.Security.Cryptography.X509Certificates;
#if !SILVERLIGHT
using ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip;
using System.Xml;
#endif

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{

	internal class ContainerInfo
	{
		public XElement Element { get; set; }
		public string Name { get; set; }
		public bool Visible { get; set; }
		public string AtomAuthor { get; set; }
		public Uri AtomHref { get; set; }
		public double RefreshInterval { get; set; }
		public RegionInfo RegionInfo {get; set;}
		public string Url { get; set; } // Only for NetworkLinks
		public ViewRefreshMode ViewRefreshMode { get; set; } // Only for NetworkLinks
		public int FolderId { get; set; } // internal folderId useful for the webmaps
		public bool HideChildren { get; set;} // Flag indicating whether the children should be hidden in the legend
		public TimeExtent TimeExtent { get; set; }
	}

	internal enum ViewRefreshMode
	{
		Never,
		OnStop,
		OnRequest,
		OnRegion // Only OnRegion used at this time
	}

	internal class RegionInfo
	{
		public RegionInfo()
		{
			MinLodPixels = MaxLodPixels = double.NaN;
		}
		public Envelope Envelope { get; set; }
		public double MinLodPixels { get; set; }
		public double MaxLodPixels { get; set; }

		public bool HasLods() { return !(double.IsNaN(MinLodPixels) && (double.IsNaN(MaxLodPixels) || MaxLodPixels == -1)); }
		
		private Envelope _projectedEnvelope; // last projected envelope

		public void GetRegionAsync(SpatialReference spatialReference, IProjectionService projectionService, Action<Envelope> callback)
		{
			Envelope region = null;
			if (spatialReference != null && Envelope != null)
			{
				if (spatialReference.Equals(Envelope.SpatialReference))
					region = Envelope;
				else if (_projectedEnvelope != null && spatialReference.Equals(_projectedEnvelope.SpatialReference))
					region = _projectedEnvelope;
				
				if (region == null && projectionService != null)
				{
					EventHandler<Tasks.GraphicsEventArgs> handler = null;
					if (projectionService.IsBusy)
					{
						// ProjectionService is busy --> Wait for the end of the current projection and retry
						handler = (s, e) =>
						{
							projectionService.ProjectCompleted -= handler;
							GetRegionAsync(spatialReference, projectionService, callback);
						};
						projectionService.ProjectCompleted += handler;
					}
					else
					{
						// Project the envelope asynchronously
						handler = (s, e) =>
						          	{
						          		projectionService.ProjectCompleted -= handler;
						          		if (e.Results.Any())
						          		{
						          			var envelope = e.Results.First().Geometry.Extent;
						          			// Check the SR since the projection service can return the geometry unchanged
						          			_projectedEnvelope = envelope != null && spatialReference.Equals(envelope.SpatialReference)
						          			                     	? envelope
						          			                     	: null;
						          			callback(_projectedEnvelope);
						          		}
						          		else
						          		{
						          			callback(null);
						          		}
						          	};

						projectionService.ProjectCompleted += handler;
						Graphic g = new Graphic {Geometry = Envelope};
						projectionService.ProjectAsync(new [] { g }, spatialReference);
					}
					return;
				}
			}
			callback(region);
		}
	}

    /// <summary>
    /// Converts a KML document into a FeatureDefinition.
    /// </summary>
    internal class KmlToFeatureDefinition
    {
        #region Private Classes
        /// <summary>
        /// This stores the state of the currently processed KML feature while its style information is
        /// downloaded from an external source.
        /// </summary>
        private class DownloadStyleState
        {
            public string StyleId { get; set; }
#if !SILVERLIGHT
			public X509Certificate ClientCertificate { get; set; }
#endif
            public System.Net.ICredentials Credentials { get; set; }
			public Action<KMLStyle> Callback { get; set; }

			public DownloadStyleState(string styleId, System.Net.ICredentials credentials, Action<KMLStyle> callback, X509Certificate clientCertificate = null)
			{
#if !SILVERLIGHT
				ClientCertificate = clientCertificate;
#endif
				StyleId = styleId;
				Credentials = credentials;
				Callback = callback;
			}
		}

		/// <summary>
		/// Helper class to wait for the end of styles downloads
		/// </summary>
		private class WaitHelper
		{
			private int _nbDownloading;
			private ManualResetEvent _waitHandle;

			public void Reset()
			{
				_nbDownloading = 0;
				_waitHandle = null;
			}

			public void AddOne()
			{
				_nbDownloading++;
				if (_waitHandle == null)
					_waitHandle = new ManualResetEvent(false);
				else
					_waitHandle.Reset();
			}

			public void OneDone()
			{
				_nbDownloading--;
				Debug.Assert(_nbDownloading >= 0 && _waitHandle != null);
				if (_nbDownloading == 0)
					_waitHandle.Set();
			}

			/// <summary>
			/// Waits for the styles.
			/// </summary>
			public void Wait()
			{
				if (_waitHandle != null)
					_waitHandle.WaitOne();
			}

		}
		#endregion

        #region Private Members
        private static readonly XNamespace atomNS = "http://www.w3.org/2005/Atom";

        // The output of the conversion process is this object. It will contain the metadata for all
        // graphic features and will later be used to convert that information into elements that are
        // added to a graphics layer. This separation of effort was done to facilitate running the code
        // in this process on a background worker thread while the creation of graphic elements and their
        // associated brushes and UI components to be done on the UI thread.
        internal FeatureDefinition featureDefs;

        /// <summary>
        /// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the Silverlight 
        /// client and a KML file.  Use a proxy service when the KML file is not hosted on a site that provides
        /// a cross domain policy file (clientaccesspolicy.xml or crossdomain.xml).
        /// </summary>
        /// <value>The Proxy URL string.</value>
        private string ProxyUrl { get; set; }

    	private readonly WaitHelper _waitHelper = new WaitHelper();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="KmlToFeatureDefinition"/> class.
        /// </summary>
        public KmlToFeatureDefinition(Uri baseUri, string proxyUrl)
        {
            // Instantiate feature definition object that will contain metadata for all supported element
            // types in the KML
			featureDefs = new FeatureDefinition(baseUri);

            ProxyUrl = proxyUrl;
        }
        #endregion

		#region Convert
		/// <summary>
        /// Takes features in the KML element and converts them into equivalent features
        /// and adds them to the FeatureDefinition.
        /// Only the direct children of the KML element are converted.
        /// </summary>
		/// <param name="context">Context containing the XElement with the KML definition to be converted.</param>
		/// <returns></returns>
        public FeatureDefinition Convert(KmlLayerContext context)
		{
			ICredentials credentials = context.Credentials;
#if !SILVERLIGHT
			var clientCertificate = context.ClientCertificate;
#endif
			XElement xElement = context.Element;
			XNamespace kmlNS = xElement.Name.Namespace;
			_waitHelper.Reset();

            // Remove any existing features so only those contained in the input KML element file are stored
            featureDefs.Clear();

			// Process the styles if they are not already known (the styles are shared by all folders/documents, so process them only once)
			if (context.Styles == null)
			{
				featureDefs.styles = new Dictionary<string, KMLStyle>();

				// Find all Style elements that have an ID and can thus be referenced by other styleURLs.
				IEnumerable<XElement> styles = xElement.Descendants().Where(e => e.Name.LocalName == "Style" && (string)e.Attribute("id") != null);
				foreach (XElement style in styles)
				{
					KMLStyle kmlStyle = new KMLStyle();
					GetStyle(style, kmlStyle);
					featureDefs.AddStyle(kmlStyle.StyleId, kmlStyle);
				}

				// Find all StyleMap elements that have an ID and can thus be referenced by other styleURLs.
				IEnumerable<XElement> styleMaps = xElement.Descendants().Where(e => e.Name.LocalName == "StyleMap" && (string)e.Attribute("id") != null);
				foreach (XElement map in styleMaps)
				{
					// A style map may need to download styles in other documents.
					// Need to use asynchronous pattern.
					GetStyleMapAsync(map, null, credentials
					                 , kmlStyle =>
					                   	{
					                   		if (kmlStyle != null)
					                   			featureDefs.AddStyle(kmlStyle.StyleId, kmlStyle);
					                   	}
#if !SILVERLIGHT
, clientCertificate
#endif
);
				}

				// Wait for getting all styles before creating the feature definition
				_waitHelper.Wait();
			}
			else
			{
				foreach (var style in context.Styles)
					featureDefs.AddStyle(style.Key, style.Value);
			}

			// Process the optional NetworkLinkControl
			XElement networkLinkControl = xElement.Element(kmlNS + "NetworkLinkControl");
			if (networkLinkControl != null)
			{
				featureDefs.networkLinkControl = new NetworkLinkControl();
				XElement minRefreshPeriod = networkLinkControl.Element(kmlNS + "minRefreshPeriod");
				if (minRefreshPeriod != null)
					featureDefs.networkLinkControl.MinRefreshPeriod = GetDoubleValue(minRefreshPeriod);
			}

			// Find the containers which will be represented by a sublayer i.e Document/Folder/NetworkLink
			foreach (XElement container in xElement.Elements().Where(element => element.Name.LocalName == "Folder" || element.Name.LocalName == "Document" || element.Name.LocalName == "NetworkLink"))
			{
				ContainerInfo containerInfo = new ContainerInfo
				                         	{
				                         		Element = container,
				                         		Url = null, // only for networklink
				                         		Visible = true,
				                         		AtomAuthor = context.AtomAuthor, // Use parent value by default
				                         		AtomHref = context.AtomHref  // Use parent value by default
				                         	};

				XNamespace kmlContainerNS = container.Name.Namespace;
				if (container.Name.LocalName == "NetworkLink")
				{
					string hrefValue = "";
					string composite = "";
					string layerids = "";

					// Link takes precedence over Url from KML version 2.1 and later:
					XElement url = container.Element(kmlContainerNS + "Link") ?? container.Element(kmlContainerNS + "Url");
					if (url != null)
					{
						XElement href = url.Element(kmlContainerNS + "href");
						if (href != null)
						{
							hrefValue = href.Value;
						}

						// This next section is to parse special elements that only occur when an ArcGIS Server KML 
						// is to be processed.
						XElement view = url.Element(kmlContainerNS + "viewFormat");
						if (view != null)
						{
							int begIdx = view.Value.IndexOf("Composite");
							if (begIdx != -1)
							{
								int endIdx = view.Value.IndexOf("&", begIdx);
								if (endIdx != -1)
									composite = view.Value.Substring(begIdx, endIdx - begIdx);
							}

							begIdx = view.Value.IndexOf("LayerIDs");
							if (begIdx != -1)
							{
								int endIdx = view.Value.IndexOf("&", begIdx);
								if (endIdx != -1)
									layerids = view.Value.Substring(begIdx, endIdx - begIdx);
							}
						}

						// If network link URL is successfully extracted, then add to container list
						if (!String.IsNullOrEmpty(hrefValue))
						{
							// extract refreshInterval
							XElement refreshMode = url.Element(kmlContainerNS + "refreshMode");
							if (refreshMode != null && refreshMode.Value == "onInterval")
							{
								XElement refreshInterval = url.Element(kmlContainerNS + "refreshInterval");
								if (refreshInterval != null)
									containerInfo.RefreshInterval = GetDoubleValue(refreshInterval);
								else
									containerInfo.RefreshInterval = 4; // default value 
							}

							XElement viewRefreshMode = url.Element(kmlContainerNS + "viewRefreshMode");
							if (viewRefreshMode != null)
							{
								ViewRefreshMode viewRefreshModeEnum;

								try // Enum.TryParse doesn't exist in 3.5
								{
									viewRefreshModeEnum = (ViewRefreshMode)Enum.Parse(typeof(ViewRefreshMode), viewRefreshMode.Value, true);
									containerInfo.ViewRefreshMode = viewRefreshModeEnum;
								}
								catch{}
							}

							// the following values are for processing specialized ArcGIS Server KML links
							// generated from REST endpoints.
							if (!String.IsNullOrEmpty(composite))
								hrefValue += "?" + composite;

							if (!String.IsNullOrEmpty(layerids))
							{
								if (!String.IsNullOrEmpty(hrefValue))
									hrefValue += "&" + layerids;
								else
									hrefValue += "?" + layerids;
							}
							containerInfo.Url = hrefValue;

						}
						else
							containerInfo = null; // Link without href. Should not happen. Skip it.
					}
					else
						containerInfo = null; // NetworkLink without Link/Url. Should not happen. Skip it.
				}
				else
				{
					// Folder or Document XElement 
					XElement linkElement = container.Elements(atomNS + "link").Where(element => element.HasAttributes).FirstOrDefault();
					if (linkElement != null)
					{
						// Overwrite global default value only upon successful extraction from element
						string tempHref = GetAtomHref(linkElement);
						if (!String.IsNullOrEmpty(tempHref))
							containerInfo.AtomHref = new Uri(tempHref);
					}

					XElement authorElement = container.Element(atomNS + "author");
					if (authorElement != null)
					{
						// Overwrite global default value only upon successful extraction from element
						string tempAuthor = GetAtomAuthor(authorElement);
						if (!String.IsNullOrEmpty(tempAuthor))
							containerInfo.AtomAuthor = tempAuthor;
					}
				}

				if (containerInfo != null)
				{
					XElement visibilityElement = container.Element(kmlContainerNS + "visibility");
					if (visibilityElement != null)
					{
						containerInfo.Visible = GetBooleanValue(visibilityElement);
					}

					XElement nameElement = container.Element(kmlContainerNS + "name");
					if (nameElement != null)
					{
						containerInfo.Name = nameElement.Value.Trim();
					}

					containerInfo.RegionInfo = ExtractRegion(container);

					// Look for a listItemType element that can be set to 'checkHideChildren' to prevent cildren to be seen in the legend
					XElement listItemTypeElement = container.XPathSelectElement("Style/ListStyle/listItemType", kmlContainerNS);
					if (listItemTypeElement != null)
					{
						if (listItemTypeElement.Value == "checkHideChildren")
							containerInfo.HideChildren = true;
					}

					if (container.HasAttributes && container.Attribute(KmlLayer.FolderIdAttributeName) != null)
					{
						containerInfo.FolderId = (int)container.Attribute(KmlLayer.FolderIdAttributeName);
					}

					containerInfo.TimeExtent = ExtractTimeExtent(container);
					featureDefs.AddContainer(containerInfo);
				}
			}


            // Process all children placemarks or groundoverlays
			foreach (XElement element in xElement.Elements().Where(element => element.Name == kmlNS + "Placemark" || element.Name == kmlNS + "GroundOverlay" ))
            {
                // Establish baseline style if a "styleUrl" setting is present
                XElement styleElement = element.Element(kmlNS + "styleUrl");
				if (styleElement != null)
				{
					// get the style asynchronously and create the feature definition as soon as the style is there
					XElement featureElement = element;
					GetStyleUrlAsync(styleElement.Value, null, credentials, kmlStyle => CreateFeatureDefinition(kmlStyle, featureElement, null, context)
#if !SILVERLIGHT
, clientCertificate
#endif
);
				}
				else
				{
					// Create feature definition synchronously using default KML style, meta data and placemark information
					CreateFeatureDefinition(null, element, null, context);
				}
            }

			// Get the name of the XElement
			XElement nameXElement = xElement.Element(kmlNS + "name");
			if (nameXElement != null && string.IsNullOrEmpty(featureDefs.name))
			{
				featureDefs.name = nameXElement.Value.Trim();
			}

			// At this point, some inner styles are possibly on the way to being downloaded and so the feature definitions are not created yet
			// Wait for all downloads to be sure all feature definitions are created before terminating the background worker
        	_waitHelper.Wait();

        	int folderId = 0;
			if (xElement.HasAttributes && xElement.Attribute(KmlLayer.FolderIdAttributeName) != null)
			{
				folderId = (int)xElement.Attribute(KmlLayer.FolderIdAttributeName);
			}

			ContainerInfo singleContainer = featureDefs.containers.Count() == 1 ? featureDefs.containers.First() : null;
			if (!featureDefs.groundOverlays.Any() && !featureDefs.placemarks.Any() && singleContainer != null && folderId == 0
				&& (singleContainer.RegionInfo == null || !singleContainer.RegionInfo.HasLods()) 
				&& string.IsNullOrEmpty(singleContainer.Url) && singleContainer.TimeExtent ==  null)
			{
				// Avoid useless level when there is no groundoverlay, no placemark and only one folder or document at the root level without any lod info
				Dictionary<string, KMLStyle> styles = featureDefs.styles.ToDictionary(style => style.Key, style => style.Value);

				KmlLayerContext childContext = new KmlLayerContext
				                               	{
				                               		Element = singleContainer.Element, // The XElement that the KML layer has to process
				                               		Styles = styles,
				                               		Images = context.Images,
				                               		AtomAuthor = singleContainer.AtomAuthor,
				                               		AtomHref = singleContainer.AtomHref,
				                               		Credentials = context.Credentials
#if !SILVERLIGHT
													,ClientCertificate = context.ClientCertificate
#endif
				                               	};

				featureDefs.hasRootContainer = true;
				return Convert(childContext);
			}

			return featureDefs;
        }
        #endregion

        #region Private Methods

		/// <summary>
    	/// Downloads KML file containing style, extracts style and creates feature definitions.
    	/// </summary>
    	/// <param name="styleUrl">Style id to locate in file.</param>
    	/// <param name="credentials">The credentials.</param>
    	/// <param name="callback">Callback to execture with the downloaded style</param>
		/// <param name="clientCertificate">The client certificate.</param>
    	/// <returns></returns>
    	private void DownloadStyleAsync(string styleUrl, System.Net.ICredentials credentials, Action<KMLStyle> callback, X509Certificate clientCertificate = null)
		{
			// We can only download KML/KMZ files that are stored remotely, not on the local file system
			if (styleUrl.StartsWith("http://") || styleUrl.StartsWith("https://"))
			{
				// Split style into file URL and style id
				string[] tokens = styleUrl.Split('#');
				if (tokens.Length == 2)
				{
					// Store current state so event handler can resume
					DownloadStyleState state = new DownloadStyleState('#' + tokens[1], credentials, callback
#if !SILVERLIGHT
, clientCertificate
#endif
);
					WebClient webClient = Utilities.CreateWebClient();

					if (credentials != null)
						webClient.Credentials = credentials;
#if !SILVERLIGHT
					if (clientCertificate != null)
						(webClient as CompressResponseWebClient).ClientCertificate = clientCertificate;
#endif

					webClient.OpenReadCompleted += StyleDownloaded;
					webClient.OpenReadAsync(Utilities.PrefixProxy(ProxyUrl, tokens[0]), state);
					_waitHelper.AddOne();
					return;
				}
			}

			// Incorrect styleUrl : execute the callback with a null style
			// This will use the default style to process the placemark.
    		callback(null); 
		}

        /// <summary>
        /// Event handler invoked when an external KML file containing a style definition has been downloaded.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments including error information and the input stream.</param>
		private void StyleDownloaded(object sender, OpenReadCompletedEventArgs e)
		{
			DownloadStyleState state = (DownloadStyleState)e.UserState;

			if (sender is WebClient)
				((WebClient) sender).OpenReadCompleted -= StyleDownloaded; // free the webclient

			// If there is no error downloading the KML/KMZ file, then load it into an XDocument and find
			// the style id within.
			XDocument xDoc = null;
			ZipFile zipFile = null;
			if (e.Error == null)
			{
				Stream seekableStream = KmlLayer.ConvertToSeekable(e.Result);
				if (seekableStream != null)
				{
					if (KmlLayer.IsStreamCompressed(seekableStream))
					{
						// Feed result into ZIP library
						// Extract KML file embedded within KMZ file
						zipFile = ZipFile.Read(seekableStream);
						xDoc = GetKmzContents(zipFile);
					}
					else
					{
						xDoc = KmlLayer.LoadDocument(seekableStream);
					}
				}
			}

			if (xDoc == null)
			{
				// Error while getting the style : execute the callback with a null style
				if (zipFile != null)
				   zipFile.Dispose();
				state.Callback(null);
			}
			else
			{
				// Look for the style in the downloaded document
				// This may need to download other documents recursively. 
				GetStyleUrlAsync(state.StyleId, xDoc, state.Credentials, kmlStyle => StoreZipfileAndCallback(kmlStyle, state.Callback, zipFile)
#if !SILVERLIGHT
, state.ClientCertificate
#endif
);
			}

			_waitHelper.OneDone();
		}

		private static void StoreZipfileAndCallback(KMLStyle kmlStyle, Action<KMLStyle> callback, ZipFile zipFile)
		{
			if (zipFile != null)
			{
				if (kmlStyle.ZipFile == null && !String.IsNullOrEmpty(kmlStyle.IconHref))
				{
					kmlStyle.ZipFile = zipFile;
				}
				else
				{
					zipFile.Dispose();
				}
			}
			callback(kmlStyle);
		}

		/// <summary>
		/// Processes each file in the ZIP stream, storing images in a dictionary and load the KML contents
		/// into an XDocument.
		/// </summary>
		/// <param name="zipFile">Decompressed stream from KMZ.</param>
		/// <returns>XDocument containing KML content from the KMZ source.</returns>
		private static XDocument GetKmzContents(ZipFile zipFile)
		{
			XDocument xDoc = null;

			// Process each file in the archive
			foreach (string filename in zipFile.EntryFileNames)
			{
				// Determine where the last "." character exists in the filename and if is does not appear
				// at all, then skip the file.
				int lastPeriod = filename.LastIndexOf(".");
				if (lastPeriod == -1)
					continue;

#if SILVERLIGHT
				Stream ms = zipFile.GetFileStream(filename);
#else
				MemoryStream ms = new MemoryStream();
				zipFile.Extract(filename, ms);
#endif
				if (ms == null) continue;
				ms.Seek(0, SeekOrigin.Begin);

				switch (filename.Substring(lastPeriod).ToLower())
				{
					case ".kml":
						// Create the XDocument object from the input stream
						xDoc = KmlLayer.LoadDocument(ms);
						break;
				}
			}

			return xDoc;
		}

		private void CreateFeatureDefinition(KMLStyle kmlStyle, XElement feature, XElement geometry, KmlLayerContext context)
        {
			if (feature == null)
				return; // should not happen

			XNamespace kmlNS = feature.Name.Namespace;
			if (feature.Name.LocalName == "Placemark")
			{
				// kmlStyle is null when the placemark doesn't reference any shared style (or a shared style that we are not able to download)
				// in this case, use a default style
				if (kmlStyle == null)
					kmlStyle = new KMLStyle();

				// Determine what kind of feature is present in the placemark. If an input geometry is present, then the
				// style has already been determined and this method is being called recursively for each child element
				// of a multi-geometry placemarker.
				XElement geomElement = null;
				if (geometry != null)
				{
					geomElement = geometry;
				}
				else
				{
					geomElement = GetFeatureType(feature);

					// Override any settings from the inline style "Style" node
					XElement styleElement = feature.Element(kmlNS + "Style");
					if (styleElement != null)
					{
						GetStyle(styleElement, kmlStyle);
					}
				}

				PlacemarkDescriptor fd = null;

				if (geomElement != null && geomElement.Name != null)
				{
					switch (geomElement.Name.LocalName)
					{
						case "Point":
							fd = ExtractPoint(kmlStyle, geomElement);
							break;

						case "LineString":
							fd = ExtractPolyLine(kmlStyle, geomElement);
							break;

						case "LinearRing":
							fd = ExtractLinearRing(kmlStyle, geomElement);
							break;

						case "Polygon":
							fd = ExtractPolygon(kmlStyle, geomElement);
							break;

						case "MultiGeometry":
							foreach (XElement item in geomElement.Elements())
							{
								// Use recursion to walk the hierarchy of embedded definitions
								CreateFeatureDefinition(kmlStyle, feature, item, context);
							}
							break;

						case "LatLonBox":
							ExtractFeatureStyleInfo(kmlStyle, feature);
							fd = ExtractLatLonBox(kmlStyle, geomElement);
							break;
					}

					// If a feature definition was created, then assign timeextent, attributes and add to collection
					if (fd != null)
					{
						fd.TimeExtent = ExtractTimeExtent(feature, fd.Attributes);

						if (fd.Geometry != null)
							fd.Geometry.SpatialReference = new SpatialReference(4326);

						XElement descElement = feature.Element(kmlNS + "description");
						if (descElement != null)
							fd.Attributes.Add("description", descElement.Value);

						XElement nameElement = feature.Element(kmlNS + "name");
						if (nameElement != null)
							fd.Attributes.Add("name", nameElement.Value);

						if (atomNS != null)
						{
							// Initialize to parent value
							Uri atomHrefValue = context.AtomHref;

							// If node exists, has attributes, and can be successfully extracted, then extract
							// this value.
							XElement atomHrefElement = feature.Element(atomNS + "link");
							if (atomHrefElement != null && atomHrefElement.HasAttributes)
							{
								string tempHref = GetAtomHref(atomHrefElement);
								if (!String.IsNullOrEmpty(tempHref))
									atomHrefValue = new Uri(tempHref);
							}

							// If a value was extracted or assigned from a parent, then add to attributes
							if (atomHrefValue != null)
								fd.Attributes.Add("atomHref", atomHrefValue);

							// AtomAuthor : Initialize to parent value
							string atomValue = context.AtomAuthor;

							// If node exists, has attributes, and can be successfully extracted, then extract
							// this value.
							XElement atomAuthorElement = feature.Element(atomNS + "author");
							if (atomAuthorElement != null)
							{
								string tempAuthor = GetAtomAuthor(atomAuthorElement);
								if (!String.IsNullOrEmpty(tempAuthor))
									atomValue = tempAuthor;
							}

							// If a value was extracted or assigned from a parent, then add to attributes
							if (!String.IsNullOrEmpty(atomValue))
								fd.Attributes.Add("atomAuthor", atomValue);
						}

						// Extract extended information
						XElement extendedDataElement = feature.Element(kmlNS + "ExtendedData");
						if (extendedDataElement != null)
						{
							List<KmlExtendedData> extendedList = new List<KmlExtendedData>();
							IEnumerable<XElement> dataElements =
								from e in extendedDataElement.Descendants(kmlNS + "Data")
								select e;
							foreach (XElement data in dataElements)
							{
								XAttribute name = data.Attribute("name");
								if (name != null)
								{
									KmlExtendedData listItem = new KmlExtendedData();
									listItem.Name = name.Value;

									foreach (XElement dataChild in data.Descendants())
									{
										if (dataChild.Name == kmlNS + "displayName")
											listItem.DisplayName = dataChild.Value;
										else if (dataChild.Name == kmlNS + "value")
											listItem.Value = dataChild.Value;
									}

									extendedList.Add(listItem);
								}
							}

							if (extendedList.Count > 0)
								fd.Attributes.Add("extendedData", extendedList);
						}

						featureDefs.AddPlacemark(fd);
					}
				}
			}
			else if (feature.Name.LocalName == "GroundOverlay")
			{
				XElement latLonBoxElement = feature.Element(kmlNS + "LatLonBox");

				if (latLonBoxElement != null)
				{
					GroundOverlayDescriptor fd = new GroundOverlayDescriptor
					                             	{
					                             		Envelope = ExtractEnvelope(latLonBoxElement),
					                             		TimeExtent = ExtractTimeExtent(feature)
					                             	};

					XElement rotationElement = latLonBoxElement.Element(kmlNS + "rotation");
					if (rotationElement != null)
						fd.Rotation = GetDoubleValue(rotationElement);

					XElement colorElement = feature.Element(kmlNS + "color");
					if (colorElement != null)
						fd.Color = GetColorFromHexString(colorElement.Value);
					else
						fd.Color = System.Windows.Media.Colors.White; // Default = white

					XElement iconElement = feature.Element(kmlNS + "Icon");
					if (iconElement != null)
					{
						XElement href = iconElement.Element(kmlNS + "href");
						if (href != null)
						{
							fd.IconHref = href.Value;
						}
					}

					featureDefs.AddGroundOverlay(fd);
				}
			}
        }

        /// <summary>
        /// Extracts the feature element from the Placemark.
        /// </summary>
        /// <param name="element">Placemark node that may contain a supported feature type node.</param>
        /// <returns>XElement node containing a supported feature type definition.</returns>
        private static XElement GetFeatureType(XElement element)
        {
            string[] featureTypes = { "Point", "LineString", "LinearRing", "Polygon", "MultiGeometry", "LatLonBox" };

        	return element.Elements().FirstOrDefault(e => featureTypes.Contains(e.Name.LocalName));
        }

        private static void ExtractFeatureStyleInfo(KMLStyle kmlStyle, XElement placemark)
        {
        	XNamespace kmlNS = placemark.Name.Namespace;
            XElement colorElement = placemark.Element(kmlNS + "color");
            if (colorElement != null)
                kmlStyle.PolyFillColor = GetColorFromHexString(colorElement.Value);

            XElement iconElement = placemark.Element(kmlNS + "Icon");
            if (iconElement != null)
                kmlStyle.IconHref = iconElement.Value.Trim();
        }

        /// <summary>
        /// Extracts a polygon from the input element and applies style information to the placemark descriptor.
        /// </summary>
        /// <param name="kmlStyle">KML Style information.</param>
        /// <param name="geomElement">Polygon geometry information.</param>
		/// <returns>A PlacemarkDescriptor object representing the feature.</returns>
        private static PlacemarkDescriptor ExtractLatLonBox(KMLStyle kmlStyle, XElement geomElement)
        {
			XNamespace kmlNS = geomElement.Name.Namespace;
			ESRI.ArcGIS.Client.Geometry.Polygon polygon = new Polygon();
            double? north = null, south = null, east = null, west = null;
            double temp;
            XElement boundary;

            // Extract box values
            boundary = geomElement.Element(kmlNS + "north");
            if (boundary != null)
            {
                if (double.TryParse(boundary.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                    north = temp;
            }
            boundary = geomElement.Element(kmlNS + "south");
            if (boundary != null)
            {
                if (double.TryParse(boundary.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                    south = temp;
            }
            boundary = geomElement.Element(kmlNS + "east");
            if (boundary != null)
            {
                if (double.TryParse(boundary.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                    east = temp;
            }
            boundary = geomElement.Element(kmlNS + "west");
            if (boundary != null)
            {
                if (double.TryParse(boundary.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                    west = temp;
            }

            if (north.HasValue && south.HasValue && east.HasValue && west.HasValue)
            {
                ESRI.ArcGIS.Client.Geometry.PointCollection pts = new PointCollection();
                MapPoint mp1 = new MapPoint(west.Value, north.Value);
                pts.Add(mp1);
                MapPoint mp2 = new MapPoint(east.Value, north.Value);
                pts.Add(mp2);
                MapPoint mp3 = new MapPoint(east.Value, south.Value);
                pts.Add(mp3);
                MapPoint mp4 = new MapPoint(west.Value, south.Value);
                pts.Add(mp4);

                polygon.Rings.Add(pts);

                // Create symbol and use style information
                PolygonSymbolDescriptor sym = new PolygonSymbolDescriptor();
                sym.style = kmlStyle;

                // Create feature descriptor from geometry and other information
                return new PlacemarkDescriptor()
                {
                    Geometry = polygon,
                    Symbol = sym
                };
            }

            return null;
        }

		/// <summary>
		/// Extracts an envelope from the input element.
		/// </summary>
		/// <param name="geomElement">LatLonBox geometry information.</param>
		/// <returns>An envelope.</returns>
		private static Envelope ExtractEnvelope(XElement geomElement)
		{
			if (geomElement == null)
				return null;

			XNamespace kmlNS = geomElement.Name.Namespace;
			double? north = null, south = null, east = null, west = null;
			double temp;
			XElement boundary;

			// Extract box values
			boundary = geomElement.Element(kmlNS + "north");
			if (boundary != null)
			{
				if (double.TryParse(boundary.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
					north = temp;
			}
			boundary = geomElement.Element(kmlNS + "south");
			if (boundary != null)
			{
				if (double.TryParse(boundary.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
					south = temp;
			}
			boundary = geomElement.Element(kmlNS + "east");
			if (boundary != null)
			{
				if (double.TryParse(boundary.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
					east = temp;
			}
			boundary = geomElement.Element(kmlNS + "west");
			if (boundary != null)
			{
				if (double.TryParse(boundary.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
					west = temp;
			}

			return north.HasValue && south.HasValue && east.HasValue && west.HasValue
			       	? new Envelope(west.Value, south.Value, east.Value, north.Value) { SpatialReference = new SpatialReference(4326)}
			       	: null;
		}

        /// <summary>
        /// Extracts a polygon from the input element and applies style information to the placemark descriptor.
        /// </summary>
        /// <param name="kmlStyle">KML Style information.</param>
        /// <param name="geomElement">Polygon geometry information.</param>
        /// <returns>A PlacemarkDescriptor object representing the feature.</returns>
        private static PlacemarkDescriptor ExtractPolygon(KMLStyle kmlStyle, XElement geomElement)
        {
			XNamespace kmlNS = geomElement.Name.Namespace;
			ESRI.ArcGIS.Client.Geometry.Polygon polygon = new Polygon();

            // Extract outer polygon boundary
            XElement boundary;
            boundary = geomElement.Element(kmlNS + "outerBoundaryIs");
            if (boundary != null)
            {
                ESRI.ArcGIS.Client.Geometry.PointCollection pts = ExtractRing(boundary);
                if (pts != null && pts.Count > 0)
                {
                    polygon.Rings.Add(pts);
                }
            }

            // Extract holes (if any)
            IEnumerable<XElement> holes =
                from e in geomElement.Descendants(kmlNS + "innerBoundaryIs")
                select e;
            foreach (XElement hole in holes)
            {
                ESRI.ArcGIS.Client.Geometry.PointCollection pts = ExtractRing(hole);
                if (pts != null && pts.Count > 0)
                {
                    polygon.Rings.Add(pts);
                }
            }
            
            // Create symbol and use style information
            PolygonSymbolDescriptor sym = new PolygonSymbolDescriptor();
            sym.style = kmlStyle;

            if (polygon.Rings.Count > 0)
            {
                // Create feature descriptor from geometry and other information
                return new PlacemarkDescriptor()
                {
                    Geometry = polygon,
                    Symbol = sym
                };
            }

            return null;
        }

        /// <summary>
        /// Extracts a linear ring from the input element and applies style information to the placemark descriptor.
        /// </summary>
        /// <param name="kmlStyle">KML Style information.</param>
        /// <param name="geomElement">Linear ring geometry information.</param>
        /// <returns>A PlacemarkDescriptor object representing the feature.</returns>
        private static PlacemarkDescriptor ExtractLinearRing(KMLStyle kmlStyle, XElement geomElement)
        {
			XNamespace kmlNS = geomElement.Name.Namespace;
			XElement coord = geomElement.Element(kmlNS + "coordinates");
            if (coord != null)
            {
                // Extract coordinates and build geometry
                ESRI.ArcGIS.Client.Geometry.PointCollection pts = ExtractCoordinates(coord);
                if (pts != null && pts.Count > 0)
                {
                    var polyline = new Polyline();
                    polyline.Paths.Add(pts);

                    // Create symbol and use style information
                    LineSymbolDescriptor sym = new LineSymbolDescriptor();
                    sym.style = kmlStyle;

                    // Create feature descriptor from geometry and other information
                    return new PlacemarkDescriptor()
                    {
                        Geometry = polyline,
                        Symbol = sym
                    };
                }
            }
            
            return null;
        }

        /// <summary>
        /// Extracts a polyline from the input element and applies style information to the placemark descriptor.
        /// </summary>
        /// <param name="kmlStyle">KML Style information.</param>
        /// <param name="line">Polyline geometry information.</param>
		/// <returns>A PlacemarkDescriptor object representing the feature.</returns>
        private static PlacemarkDescriptor ExtractPolyLine(KMLStyle kmlStyle, XElement line)
        {
			XNamespace kmlNS = line.Name.Namespace;
			XElement coord = line.Element(kmlNS + "coordinates");
            if (coord != null)
            {
                // Extract coordinates and build geometry
                ESRI.ArcGIS.Client.Geometry.PointCollection pts = ExtractCoordinates(coord);
                if (pts != null && pts.Count > 0)
                {
                    ESRI.ArcGIS.Client.Geometry.Polyline polyline = new ESRI.ArcGIS.Client.Geometry.Polyline();
                    polyline.Paths.Add(pts);

                    // Create symbol and use style information
                    LineSymbolDescriptor sym = new LineSymbolDescriptor();
                    sym.style = kmlStyle;

                    // Create feature descriptor from geometry and other information
                    return new PlacemarkDescriptor()
                    {
                        Geometry = polyline,
                        Symbol = sym
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts a point from the input element and applies style information to the placemark descriptor.
        /// </summary>
        /// <param name="kmlStyle">KML Style information.</param>
        /// <param name="point">Point geometry information.</param>
		/// <returns>A PlacemarkDescriptor object representing the feature.</returns>
        private static PlacemarkDescriptor ExtractPoint(KMLStyle kmlStyle, XElement point)
        {
			XNamespace kmlNS = point.Name.Namespace;
			XElement coord = point.Element(kmlNS + "coordinates");
            if (coord != null)
            {
                // Extract geometry
                ESRI.ArcGIS.Client.Geometry.Geometry geom = ExtractCoordinate(coord.Value);
                if (geom != null)
                {
                    // Create symbol and use style information
                    PointSymbolDescriptor sym = new PointSymbolDescriptor();
                    sym.style = kmlStyle;

                    // Create feature descriptor from geometry and other information
                    PlacemarkDescriptor fd = new PlacemarkDescriptor()
                    {
                        Geometry = geom,
                        Symbol = sym
                    };

                    return fd;
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts a collection of points from a LinearRing definition.
        /// </summary>
        /// <param name="boundary">Outer or Inner boundary XElement object.</param>
        /// <returns>A PointCollection containing MapPoint objects.</returns>
        private static PointCollection ExtractRing(XElement boundary)
        {
			XNamespace kmlNS = boundary.Name.Namespace;
			// Ensure there is a LinearRing element within the boundary
            XElement ring = boundary.Element(kmlNS + "LinearRing");
            if (ring != null)
            {
                // Ensure there is a coordinates element within the linear ring
                XElement coord = ring.Element(kmlNS + "coordinates");
                if (coord != null)
                {
                    return ExtractCoordinates(coord);
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts the X and Y values from a comma delimited string containing multiple coordinates.
        /// </summary>
        /// <param name="coordinates">Comma delimited string containing multiple coordinate groups.</param>
        /// <returns>A PointCollection containing MapPoint objects.</returns>
        private static PointCollection ExtractCoordinates(XElement coordinates)
        {
			IList<MapPoint> pointsList = new List<MapPoint>();

            // Break collection into individual coordinates
            string[] paths = coordinates.Value.Trim().Split(new [] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string coordinate in paths)
            {
                MapPoint mapPoint = ExtractCoordinate(coordinate);
                if (mapPoint != null)
					pointsList.Add(mapPoint);
            }

			return new ESRI.ArcGIS.Client.Geometry.PointCollection(pointsList);
        }

        /// <summary>
        /// Extracts the X and Y values from a comma delimited string containing a single coordinate.
        /// </summary>
        /// <param name="coordinate">Comma delimited string containing X, Y and Z values.</param>
        /// <returns>A MapPoint object with X and Y coordinate values assigned.</returns>
        private static MapPoint ExtractCoordinate(string coordinate)
        {
            MapPoint mp = null;

            // Ensure string coordinate is intact
            if (!String.IsNullOrEmpty(coordinate))
            {
                // Split input string into an array of strings using comma as the delimiter
                string[] xy = coordinate.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                // Make sure X and Y coordinate strings are available
                if (xy.Length >= 2)
                {
                    double x, y;

                    // Create new MapPoint object passing in X and Y values to constructor
                    if (double.TryParse(xy[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out x) && double.TryParse(xy[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                    {
                        mp = new MapPoint(x, y);
                    }
                }
            }

            return mp;
        }

		/// <summary>
		/// Extracts the region from a KML feature (folder, document, networklink, placemark or groundoverlay)
		/// </summary>
		/// <param name="feature">The feature.</param>
		/// <returns></returns>
		private static RegionInfo ExtractRegion(XElement feature)
		{
			XNamespace kmlNS = feature.Name.Namespace;
			XElement region = feature.Element(kmlNS + "Region");
			RegionInfo regionInfo;
			if (region != null)
			{
				regionInfo = new RegionInfo();
				regionInfo.Envelope = ExtractEnvelope(region.Element(kmlNS + "LatLonAltBox"));

				XElement lod = (region.Element(kmlNS + "Lod"));
				if (lod != null)
				{
					regionInfo.MinLodPixels = ExtractDouble(lod.Element(kmlNS + "minLodPixels"));
					regionInfo.MaxLodPixels = ExtractDouble(lod.Element(kmlNS + "maxLodPixels"));
				}
			}
			else
			{
				regionInfo = null;
			}
			return regionInfo;
		}

		private static double ExtractDouble(XElement element)
		{
			double result = double.NaN;
			if (element != null)
				double.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
			return result;

		}

		/// <summary>
		/// Gets the 'normal' style of a style map.
		/// Getting this style may need recursive download.
		/// When the style is ready -> execute the callback.
		/// </summary>
		/// <remarks>
		/// The 'highlight' style is not used by the KmlLayer.
		/// </remarks>
		/// <param name="styleMap">The style map element to parse.</param>
		/// <param name="xDoc">The xDocument the style map is part of.</param>
		/// <param name="credentials">The credentials.</param>
		/// <param name="callback">The callback to call when the style is downloaded (if needed).</param>
		/// <param name="clientCertificate">The client certificate.</param>
		private void  GetStyleMapAsync(XElement styleMap, XDocument xDoc, ICredentials credentials, Action<KMLStyle> callback, X509Certificate clientCertificate = null)
		{
			XNamespace kmlNS = styleMap.Name.Namespace;
			KMLStyle kmlStyle = null;
			foreach (XElement pair in styleMap.Descendants(kmlNS + "Pair"))
			{
				XElement key = pair.Element(kmlNS + "key");
				if (key != null)
				{
					if (key.Value == "normal")
					{
						XElement style = pair.Element(kmlNS + "Style");
						if (style != null)
						{
							kmlStyle = new KMLStyle();
							GetStyle(style, kmlStyle);
						}
						else
						{
							XElement styleUrl = pair.Element(kmlNS + "styleUrl");
							if (styleUrl != null)
							{
								XAttribute styleIdAttribute = styleMap.Attribute("id");
								string styleId = styleIdAttribute == null ? null : styleIdAttribute.Value;

								// Get the style from the styleUrl. This may need to downloading an external KML file
								GetStyleUrlAsync(styleUrl.Value, xDoc, credentials
									, kmlstyle =>
										{
											//// After obtaining the style (which may have involved recursion and downloading external KML files
											//// to resolve style URLs) be sure to always overwrite the styleId with the name given to this StyleMap.
											if (styleId != null && kmlstyle != null)
												kmlstyle.StyleId = styleId;
											callback(kmlstyle);
										}, clientCertificate);
								return;
							}
						}
					}
				}
			}

			// execute the callback with the found style (or null if not found)
			callback(kmlStyle);
		}


		private void GetStyleUrlAsync(string styleUrl, XDocument xDoc, System.Net.ICredentials credentials, Action<KMLStyle> callback, X509Certificate clientCertificate = null)
		{
			KMLStyle kmlStyle = new KMLStyle();
			styleUrl = styleUrl.Trim();

			if (!String.IsNullOrEmpty(styleUrl))
			{
				// If the style url begins with a # symbol, then it is a reference to a style
				// defined within the current KML file. Otherwise it is a reference to a style
				// in an external file which must be downloaded and processed.
				// If there is no starting # and no KML doc referenced, we search also in current KML file (may happen that the # is missing in some KML doc)
				if (styleUrl.StartsWith("#") || (xDoc == null && featureDefs.styles.ContainsKey(styleUrl)))
				{
					// Remove first character (which is "#")
					string styleId = styleUrl.StartsWith("#") ? styleUrl.Substring(1) : styleUrl;

					if (xDoc == null)
					{
						if (featureDefs.styles.ContainsKey(styleId))
							kmlStyle.CopyFrom(featureDefs.styles[styleId]);
					}
					else
					{
						XElement style = xDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Style" && (string)e.Attribute("id") == styleId);

						// Make sure the style was found and use first element
						if (style != null)
						{
							GetStyle(style, kmlStyle);
						}
						else
						{
							// See if the styleURL value is associated with a StyleMap node
							style = xDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "StyleMap" && (string)e.Attribute("id") == styleId);

							// Make sure the style map was found and use first element
							if (style != null)
							{
								GetStyleMapAsync(style, xDoc, credentials, callback, clientCertificate);
								return;
							}
						}
					}
				}
				else
				{
					DownloadStyleAsync(styleUrl, credentials, callback, clientCertificate);
					return;
				}
			}

			callback(kmlStyle);
		}

        /// <summary>
        /// Constructs a KMLStyle object that represents KML style contained in the input XElement.
        /// </summary>
        /// <param name="style">XElement containing KML style definition.</param>
        /// <param name="kmlStyle">KMLStyle object representing input style.</param>
        private static void GetStyle(XElement style, KMLStyle kmlStyle)
        {
        	XNamespace kmlNS = style.Name.Namespace;
            XAttribute styleId = style.Attribute("id");
            if (styleId != null)
            {
                kmlStyle.StyleId = styleId.Value;
            }

            // If style contains an BalloonStyle, then extract that information
            XElement balloonStyle = style.Element(kmlNS + "BalloonStyle");
            if (balloonStyle != null)
            {
                XElement text = balloonStyle.Element(kmlNS + "text");
                if (text != null)
                {
                    kmlStyle.BalloonText = text.Value;
                }
            }

            // If style contains an IconStyle, then extract that information
            XElement iconStyle = style.Element(kmlNS + "IconStyle");
            if (iconStyle != null)
            {
                XElement icon = iconStyle.Element(kmlNS + "Icon");
                if (icon != null)
                {
                    XElement href = icon.Element(kmlNS + "href");
                    if (href != null)
                    {
                        string iconUrl = href.Value;
                        const string googlePal = "root://icons/palette-";
                        if(iconUrl.StartsWith(googlePal, StringComparison.OrdinalIgnoreCase))
                        {
                            // Replace Google earth built-in palette URL by the real URL
                            int x = 0;
                            int y = 0;
                            int numPalette = 0;
                            XElement xElement = icon.Element(kmlNS + "x");
                            if (xElement != null)
                                int.TryParse(xElement.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out x);
                            XElement yElement = icon.Element(kmlNS + "y");
                            if (yElement != null)
                                int.TryParse(yElement.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out y);
                            string pal = iconUrl.Substring(googlePal.Length, 1);
                            int.TryParse(pal, NumberStyles.Integer, CultureInfo.InvariantCulture, out numPalette);
                            if (numPalette > 0)
                            {
                                int numIcon = 8 * (7 - y/32) + x/32;
                                iconUrl = string.Format("http://maps.google.com/mapfiles/kml/pal{0}/icon{1}.png", numPalette, numIcon);
                            }
                        }

                        kmlStyle.IconHref = iconUrl;

                    }
                }

                // Extract IconColor
                XElement iconColor = iconStyle.Element(kmlNS + "color");
                if (iconColor != null)
                {
                    kmlStyle.IconColor = GetColorFromHexString(iconColor.Value);
                }

                // If the hotspot element is present, make use of it
                XElement hotspot = iconStyle.Element(kmlNS + "hotSpot");
                if (hotspot != null)
                {
                    XAttribute units;
                    XAttribute val;

                    units = hotspot.Attribute("xunits");
                    if (units != null)
                    {
                        try
                        {
                            kmlStyle.IconHotspotUnitsX = (HotSpotUnitType)Enum.Parse(typeof(HotSpotUnitType), units.Value, true);
                            val = hotspot.Attribute("x");
                            if (val != null)
                            {
                                double x;
                                if (double.TryParse(val.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                                    kmlStyle.IconHotspotX = x;
                            }
                        }
                        catch { }
                    }

                    units = hotspot.Attribute("yunits");
                    if (units != null)
                    {
                        try
                        {
                            kmlStyle.IconHotspotUnitsY = (HotSpotUnitType)Enum.Parse(typeof(HotSpotUnitType), units.Value, true);
                            val = hotspot.Attribute("y");
                            if (val != null)
                            {
                                double y;
                                if (double.TryParse(val.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                                    kmlStyle.IconHotspotY = y;
                            }
                        }
                        catch { }
                    }
                }

                // If the heading element is present, make use of it
                XElement heading = iconStyle.Element(kmlNS + "heading");
                if (heading != null)
                {
                    double degrees;
                    if (double.TryParse(heading.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out degrees))
                        kmlStyle.IconHeading = degrees;
                }

                // If the scale element is present, make use of it
                XElement scale = iconStyle.Element(kmlNS + "scale");
                if (scale != null)
                {
                    double scaleAmount;
                    if (double.TryParse(scale.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out scaleAmount))
                        kmlStyle.IconScale = scaleAmount;
                }
            }

            // If style contains a LineStyle, then extract that information
            XElement lineStyle = style.Element(kmlNS + "LineStyle");
            if (lineStyle != null)
            {
                XElement color = lineStyle.Element(kmlNS + "color");
                if (color != null)
                {
                    kmlStyle.LineColor = GetColorFromHexString(color.Value);
                }
                XElement width = lineStyle.Element(kmlNS + "width");
                if (width != null)
                {
                    double widthVal;
                    if (double.TryParse(width.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out widthVal))
                        kmlStyle.LineWidth = widthVal;
                }
            }

            // If style contains a PolyStyle, then extract that information
            XElement polyStyle = style.Element(kmlNS + "PolyStyle");
            if (polyStyle != null)
            {
                XElement color = polyStyle.Element(kmlNS + "color");
                if (color != null)
                {
                    kmlStyle.PolyFillColor = GetColorFromHexString(color.Value);
                }
                XElement fill = polyStyle.Element(kmlNS + "fill");
                if (fill != null)
                {
                    kmlStyle.PolyFill = StringToBool(fill.Value);
                }
                XElement outline = polyStyle.Element(kmlNS + "outline");
                if (outline != null)
                {
                    kmlStyle.PolyOutline = StringToBool(outline.Value);
                }
            }
        }

        /// <summary>
        /// Converts a string containing an integer value into a boolean.
        /// </summary>
        /// <param name="s">String containing boolean in numeric format.</param>
        /// <returns>Boolean value extracted from the input string.</returns>
        private static bool StringToBool(string s)
        {
            int intVal;

            // Try to parse the string into an integer. If successful, then treat 0 as false and
            // all other values as true.
            if (int.TryParse(s, out intVal))
            {
                return (intVal == 0) ? false : true;
            }

            // If parsing failed, then return default value for boolean type
            return false;
        }

		private static bool GetBooleanValue(XElement element)
		{
			bool visible;
			if (Boolean.TryParse(element.Value, out visible))
				return visible;
			return StringToBool(element.Value);
		}

        /// <summary>
        /// Converts hexadecimal color notation into equivalent Silverlight Color.
        /// </summary>
        /// <param name="s">Input color string in hexadecimal format.</param>
        /// <returns>Color object representing input string.</returns>
        private static System.Windows.Media.Color GetColorFromHexString(string s)
        {
            if (s.Length == 8)
            {
                // Be advised that the values are not ARGB, but instead ABGR.
                byte a = System.Convert.ToByte(s.Substring(0, 2), 16);
                byte b = System.Convert.ToByte(s.Substring(2, 2), 16);
                byte g = System.Convert.ToByte(s.Substring(4, 2), 16);
                byte r = System.Convert.ToByte(s.Substring(6, 2), 16);
                return System.Windows.Media.Color.FromArgb(a, r, g, b);
            }
            else
            {
                byte b = System.Convert.ToByte(s.Substring(0, 2), 16);
                byte g = System.Convert.ToByte(s.Substring(2, 2), 16);
                byte r = System.Convert.ToByte(s.Substring(4, 2), 16);
                return System.Windows.Media.Color.FromArgb(255, r, g, b);
            }
        }

        private static string GetAtomHref(XElement element)
        {
            XAttribute atomHrefAttr = element.Attribute("href");
            if (atomHrefAttr != null)
            {
                return atomHrefAttr.Value;
            }

            return null;
        }

        private static string GetAtomAuthor(XElement element)
        {
            XElement name = element.Element(atomNS + "name");
            if (name != null)
            {
                return name.Value;
            }

            return null;
        }

		private static double GetDoubleValue(XElement element)
		{
			double ret = 0.0;
			if (element != null)
				double.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out ret);

			return ret;
		}

    	#endregion

		#region Time private methods

		private static TimeExtent ExtractTimeExtent(XElement element, IDictionary<string, object> attributes = null)
		{
			if (element == null)
				return null;
			XNamespace ns = element.Name.Namespace;

			return ExtractTimeExtentFromTimeSpan(element.Element(ns + "TimeSpan"), attributes) ??
			       ExtractTimeExtentFromTimeStamp(element.Element(ns + "TimeStamp"), attributes);
		}

		private static TimeExtent ExtractTimeExtentFromTimeSpan(XElement element, IDictionary<string, object> attributes)
		{
			if (element == null)
				return null;

			XElement beginElement = element.Element(element.Name.Namespace + "begin");
			string strBegin = beginElement == null ? null : beginElement.Value;
			if (attributes != null && !string.IsNullOrEmpty(strBegin))
				attributes["TimeSpan_Begin"] = strBegin;

			XElement endElement = element.Element(element.Name.Namespace + "end");
			string strEnd = endElement == null ? null : endElement.Value;
			if (attributes != null && !string.IsNullOrEmpty(strEnd))
				attributes["TimeSpan_End"] = strEnd;


			TimeExtent startExtent = ExtractTimeExtentFromDate(strBegin);
			TimeExtent endExtent = ExtractTimeExtentFromDate(strEnd);

			DateTime startDate = startExtent == null ? DateTime.MinValue : startExtent.Start;
			DateTime endDate = endExtent == null ? DateTime.MaxValue : endExtent.End;
			return new TimeExtent(startDate, endDate);
		}

		private static TimeExtent ExtractTimeExtentFromTimeStamp(XElement element, IDictionary<string, object> attributes)
		{
			if (element == null)
				return null;
			XElement whenElement = element.Element(element.Name.Namespace + "when");
			string strWhen = whenElement == null ? null : whenElement.Value;
			if (attributes != null && !string.IsNullOrEmpty(strWhen))
				attributes["TimeStamp"] = strWhen;
			return ExtractTimeExtentFromDate(strWhen);
		}

		// Example : 1997-07-16T10:30:15+03:00
		private static TimeExtent ExtractTimeExtentFromDate(string strDateTime)
		{
			if (string.IsNullOrEmpty(strDateTime))
				return null;

			string[] strsDateTime = strDateTime.Split('T'); // Separator between date and time
			string strDate = strsDateTime[0];
			string strTime = strsDateTime.Length > 1 ? strsDateTime[1] : null;

			// Decode the date
			string[] strsDate = strDate.Split('-'); // Y,M,D separator

			int year, month = 0, day = 0;
			if (!int.TryParse(strsDate[0], out year))
				return null;
			if (strsDate.Length > 1 && !int.TryParse(strsDate[1], out month))
				return null;
			if (strsDate.Length > 2 && !int.TryParse(strsDate[2], out day))
				return null;
			
			DateTime startTime = new DateTime(year, month > 0 ? month : 1, day > 0 ? day : 1);
			startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);

			if (string.IsNullOrEmpty(strTime))
			{
				// There is no time part --> calculate the end time to get an extent corresponding to the year, the month or the day
				DateTime endTime;
				if (month == 0)
					endTime= startTime.AddYears(1); // TimeExtent is the whole year
				else if (day == 0)
					endTime = startTime.AddMonths(1); // TimeExtent is the whole month
				else
					endTime = startTime.AddDays(1); // TimeExtent is the whole day
				endTime = endTime.AddSeconds(-1); // so the first second of the next year, month or day is not in the extent

				return new TimeExtent(startTime, endTime);
			}
			else
			{
				// There is a time part
				// Separate time part and offset part ('Z' at the end means UTC i.e. no offset)
				int index = strTime.IndexOfAny(new char[] {'Z', '+', '-'});

				TimeSpan timeOffsetSpan = new TimeSpan(0);
				if (index > 0)
				{
					if (strTime[index] == '+' || strTime[index] == '-')
					{
						// Use time offset
						string strOffset = strTime.Substring(index + 1);
						TimeSpan.TryParse(strOffset, out timeOffsetSpan);
						if (strTime[index] == '-')
							timeOffsetSpan = -timeOffsetSpan;
					}
					strTime = strTime.Substring(0, index);
				}

				TimeSpan timeSpan;
				if (!TimeSpan.TryParse(strTime, out timeSpan))
					return null;

				startTime += timeSpan;
				startTime += timeOffsetSpan;
				return new TimeExtent(startTime);
			}
		}

		#endregion
	}
}
