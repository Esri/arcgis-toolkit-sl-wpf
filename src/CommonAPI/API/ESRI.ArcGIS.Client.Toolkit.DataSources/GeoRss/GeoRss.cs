// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using ESRI.ArcGIS.Client.Symbols;
using System.Collections.Generic;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	/// <summary>
	/// GeoRSS Layer.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Only <a href="http://www.georss.org/simple">GeoRSS-simple</a> feeds are supported.
	/// Geometries are returned in Geographic WGS84. If you are displaying the feed
	/// on top of a map in a different projection, they must be reprojected manually 
	/// when the graphics collection gets features added.
	/// </para>
	/// <para>
	/// The graphic will not have a symbol associated with them. You should specify
	/// a renderer on this layer, or manually assign symbols to the graphics when
	/// the graphics collection gets features added.
	/// </para>
	/// <para>
	/// Recent earthquake's greater than M2.5 with map tips:<br/>
	/// <code Lang="XAML">
	/// &lt;esri:GeoRssLayer Source=&quot;http://earthquake.usgs.gov/earthquakes/catalogs/1day-M2.5.xml&quot; &gt;
	///	  &lt;esri:GeoRssLayer.Renderer&gt;
	///	    &lt;esri:SimpleRenderer Brush=&quot;Red&quot; /&gt;
	///	  &lt;/esri:GeoRssLayer.Renderer&gt;
	///	  &lt;esri:GeoRssLayer.MapTip&gt;
	///	    &lt;Border Padding=&quot;5&quot; Background=&quot;White&quot; esri:GraphicsLayer.MapTipHideDelay=&quot;0:0:0.5&quot;&gt;
	///	      &lt;StackPanel&gt;
	///	        &lt;TextBlock Text=&quot;{Binding [Title]}&quot; FontWeight=&quot;Bold&quot; FontSize=&quot;12&quot; /&gt;
	///	        &lt;TextBlock Text=&quot;{Binding [Summary]}&quot; FontSize=&quot;10&quot; /&gt;
	///	        &lt;HyperlinkButton Content=&quot;Link&quot; NavigateUri=&quot;{Binding [Link]}&quot; Opacity=&quot;.5&quot; FontSize=&quot;10&quot; TargetName=&quot;_blank&quot; /&gt;
	///	      &lt;/StackPanel&gt;
	///	    &lt;/Border&gt;
	///	  &lt;/esri:GeoRssLayer.MapTip&gt;
	/// &lt;/esri:GeoRssLayer&gt;
	/// </code>
	/// </para>
	/// <para>
	/// If you require a proxy, simply prefix the layer URI with a proxy prefix:<br/>
	/// <code Lang="XAML">
	/// &lt;esri:GeoRssLayer Source=&quot;../proxy.ashx?url=http://earthquake.usgs.gov/earthquakes/catalogs/1day-M2.5.xml&quot; /&gt;
	/// </code>
	/// </para>
	/// <para>
	/// The following attributes will be associated with each graphic:
    /// </para>
    /// <list type="bullet">
	/// 	<item>Title (<see cref="String"/>)</item>
	/// 	<item>Summary (<see cref="String"/>)</item> 
	/// 	<item>PublishDate (<see cref="DateTime"/>)</item>
	/// 	<item>Id (<see cref="String"/>)</item>
	/// 	<item>Link (<see cref="System.Uri"/>)</item>
	/// 	<item>FeedItem (<see cref="System.ServiceModel.Syndication.SyndicationItem"/>)</item>
	/// </list>
    /// <para>
	/// Optionally, if the item is using any of the simple-georss extensions,
	/// these will also be included:
    /// </para>
	/// <list type="bullet">
	///		<item>elev (<see cref="double"/>)</item>
	/// 	<item>floor (<see cref="System.Int32"/>)</item>
	/// 	<item>radius (<see cref="double"/>)</item>
	/// 	<item>featuretypetag (<see cref="string"/>)</item> 
	/// 	<item>relationshiptag (<see cref="string"/>)</item>
	/// 	<item>featurename (<see cref="string"/>)</item>
	/// </list>
    /// <para>
	/// The Graphic's <see cref="ESRI.ArcGIS.Client.Graphic.TimeExtent"/> property 
	/// will be set to a time instance matching the PublishDate.
    /// </para>
	/// </remarks>
	public sealed class GeoRssLayer : GraphicsLayer
    {
		GeoRssLoader loader;

        #region Constructor:
		/// <summary>
		/// Initializes a new instance of the <see cref="GeoRssLayer"/> class.
		/// </summary>
        public GeoRssLayer() : base()
        {
			loader = new GeoRssLoader();
			loader.LoadCompleted += loader_LoadCompleted;
			loader.LoadFailed += loader_LoadFailed;
        }

		private void loader_LoadFailed(object sender, GeoRssLoader.RssLoadFailedEventArgs e)
		{
			this.InitializationFailure = e.ex;
			if (!IsInitialized)
				base.Initialize();
		}

		private void loader_LoadCompleted(object sender, GeoRssLoader.RssLoadedEventArgs e)
		{
			this.Graphics = new GraphicCollection(e.Graphics);
			// GeoRSS-Simple requires geometries in WGS84 hence; setting layer Spatial Reference to 4326:
			this.SpatialReference = new Geometry.SpatialReference(4326);
			if(!IsInitialized)
				base.Initialize();
		}
        #endregion

        #region Overriden Methods:

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
            Update();
        }

		/// <summary>
		/// Called when the GraphicsSource property changes.
		/// </summary>
		/// <param name="oldValue">Old value of the GraphicsSource property.</param>
		/// <param name="newValue">New value of the GraphicsSource property.</param>
		/// <exception cref="InvalidOperationException">Thrown when <see cref="GraphicsLayer.GraphicsSource"/>property is changed on a <see cref="GeoRssLayer"/>.</exception>
		protected override void OnGraphicsSourceChanged(IEnumerable<Graphic> oldValue, IEnumerable<Graphic> newValue)
		{
			throw new InvalidOperationException(Properties.Resources.GraphicsLayer_GraphicsSourceCannotBeSetOnLayer);
		}

        #endregion

        #region Dependency Properties:
#if !SILVERLIGHT
  /// <summary>
  /// Gets or sets the client certificate that is sent to the host and used to authenticate the request.
  /// </summary>
  /// <value>The client certificate used for authentication.</value>
  /// <remarks>
  /// <para>
  /// A client certificate is an electronic document which uses a digital signature to bind a public key with an identity. 
  /// A client certificate is used to verify that a public key belongs to an individual or an organization. When a client 
  /// certificate is valid, access to secured content over the https:// is enabled. Client certificates fall under the 
  /// technology umbrella known as a Public-Key Infrastructure (PKI). PKI is a large complex body of standards, policies, 
  /// protocols, and practices that are beyond the scope this documentation. The following Microsoft document should give 
  /// the developer a starting point to understand PKI: 
  /// <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb427432(v=vs.85).aspx" target="_blank">Public Key Infrastructure</a>.
  /// </para>
  /// <para>
  /// ArcGIS Server version 10.1 and higher has the ability to leverage PKI solutions in 'Commercial Off the Shelf' (COTS) Web servers 
  /// such as: Microsoft Internet Information Server (IIS), Oracle WebLogic, IBM WebSphere, etc. through the use of the 
  /// ArcGIS Web Adaptor. The requirements for using PKI in ArcGIS Server include:
  /// </para>
  /// <list type="number">
  ///   <item>The ArcGIS Web Adaptor must be setup as the gateway to ArcGIS Server.</item>
  ///   <item>The Web Server hosting the ArcGIS Web Adaptor must be configured to require client certificates for user authentication.</item>
  ///   <item>ArcGIS Server Site must be configured to: (a)	Delegate user authentication to the Web Tier and (b) Use an identity store (LDAP, Windows Active Directory, etc.) supported by the Web Server.</item>
  /// </list>
  /// <para>
  /// When a request is made for a resource on ArcGIS Server, the Web Server will authenticate the user by validating the 
  /// client certificate provided. The request (along with the user name) is then forwarded to ArcGIS Server via the Web 
  /// Adaptor. ArcGIS Server will verify that the specified user has access to the requested resource before sending back 
  /// the appropriate response. For more information on using PKI techniques to set up and use client certificates, see 
  /// the ArcGIS Server documentation.
  /// </para>
  /// <para>
  /// The ArcGIS Runtime for WPF requires supplying a valid 
  /// <a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&amp;l=EN-US&amp;k=k(System.Security.Cryptography.X509Certificates.X509Certificate)&amp;rd=true" target="_blank">Microsoft System.Security.Cryptography.X509Certificates.X509Certificate</a> 
  /// object as the .ClientCertificate Property in order to gain access to a secured (https://) ArcGIS Server web service 
  /// based upon PKI. The Microsoft 
  /// <a href="http://msdn.microsoft.com/en-us/library/ztkw6e67" target="_blank">System.Security.Cryptography.X509Certificates Namespace</a> 
  /// API documentation provides a starting point for developers to learn how to programmatically access X509Certificate objects. 
  /// If no client certificates have been set up on a client machine and a user tries to access using an X509Certificate from 
  /// your custom ArcGIS WPF application, a Windows Security dialog stating "No certificate available. No certificates meet the 
  /// application. Click OK to continue" will appear:
  /// </para>
  /// <para>
  /// <img border="0" alt="Try to access an X509Certificate when none are installed on the client computer." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.NoPKI_CertificateAvailable.png"/>
  /// </para>
  /// <para>
  /// Whenever an ArcGIS Runtime for WPF based application uses PKI to secure web services, it is important that error checking 
  /// be added to the application to ensure that the correct X509Certificate is used to access those secured web services. If 
  /// a user of your ArcGIS WPF client application provides/uses an X509Certificate that is not accepted by the PKI security 
  /// set up on the ArcGIS Server machine, then an error will be thrown. The following are a couple of different error messages 
  /// that could occur:
  /// </para>
  /// <para>
  /// "Error initializing layer: The remote server returned an error: (403) Forbidden.":
  /// </para>
  /// <para>
  /// <img border="0" alt="Using an incorrect X509Certificate for the .ClientProperty return 403 Forbidden error." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.403ForbiddenAccessDenied.png"/>
  /// </para>
  /// <para>
  /// "Error initializing layer: The remote server returned an error: (401) Unauthorized.":
  /// </para>
  /// <para>
  /// <img border="0" alt="Using an incorrect X509Certificate for the .ClientProperty return 401 Unauthorized error." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.401Unauthorized.png"/>
  /// </para>
  /// <para>
  /// Depending on the particular ArcGIS Runtime for WPF object that is used, the developer will need to write code in the 
  /// appropriate error handling event. For example: an ArcGISDynmaicMapServiceLayer should have error trapping code in 
  /// the InitializationFailed Event; a QueryTask should have error trapping code in the Failed Event, a PrintTask should 
  /// have error trapping code in the ExecuteCompleted Event (via the PrintEventArgs), etc.
  /// </para>
  /// <para>
  /// The .ClientCertificate Property has been added to numerous ArcGIS Runtime for WPF objects. Accessing and using an 
  /// X509Certificate is basically the same for each of the ArcGIS Runtime for WPF objects with  a .ClientCertificate 
  /// Property. There are code examples of using the X509Certificate in the 
  /// <see cref="ESRI.ArcGIS.Client.DynamicMapServiceLayer.ClientCertificate">DynamicMapServiceLayer.ClientCertificate</see> 
  /// Property, 
  /// <see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer.ClientCertificate">ArcGISTiledMapServiceLayer.ClientCertificate</see> 
  /// Property, 
  /// <see cref="M:ESRI.ArcGIS.Client.Printing.PrintTask.ClientCertificate">Printing.PrintTask.ClientCertificate</see> 
  /// Property (code-behind only options) and 
  /// <see cref="ESRI.ArcGIS.Client.FeatureLayer.ClientCertificate">FeatureLayer.ClientCertificate</see> Property 
  /// (Model-View-View-Model (MVVM) pattern using XAML and code-behind). Remember the key to accessing a PKI based 
  /// secured ArcGIS Server web service is to first provide the appropriate .ClientCertificate Property credentials 
  /// during construction of the object and prior to using (i.e Set/Write) any of the other properties/methods of 
  /// the ArcGIS Runtime for WPF object, otherwise an error accessing that object will result.
  /// </para>
  /// </remarks>
  public System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate
		{
			get { return (System.Security.Cryptography.X509Certificates.X509Certificate)GetValue(ClientCertificateProperty); }
			set { SetValue(ClientCertificateProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ClientCertificate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ClientCertificateProperty =
			DependencyProperty.Register("ClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(GeoRssLayer), null);
#endif

#if !SILVERLIGHT || WINDOWS_PHONE
		/// <summary>
		/// Gets or sets the network credentials that are sent to the host and used to authenticate the request.
		/// </summary>
		/// <value>The credentials used for authentication.</value>
		public System.Net.ICredentials Credentials
		{
			get { return (System.Net.ICredentials)GetValue(CredentialsProperty); }
			set { SetValue(CredentialsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Credentials"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CredentialsProperty =
			DependencyProperty.Register("Credentials", typeof(System.Net.ICredentials), typeof(GeoRssLayer), null);
#endif

		/// <summary>
		/// Gets or sets the URI for the RSS feed.
		/// </summary>
		public Uri Source
        {
			get { return ((Uri)GetValue(SourceProperty)); }
			set { SetValue(SourceProperty, value); }
        }

		/// <summary>
		/// Identifies the <see cref="Source"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(Uri), typeof(GeoRssLayer), null);
        
        #endregion

		/// <summary>
		/// Reloads the RSS feed from the endpoint.
		/// </summary>
		public void Update()
		{
			if (Source != null)
			{
#if !SILVERLIGHT
				loader.LoadRss(Source, Credentials, ClientCertificate);
#else
                loader.LoadRss(Source);
#endif
			}
		}
	}
}
