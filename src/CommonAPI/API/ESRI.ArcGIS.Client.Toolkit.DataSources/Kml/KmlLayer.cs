// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit.DataSources.Kml;
using System.Windows.Media;
using System.Windows;
#if !SILVERLIGHT
using ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip;
#endif

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{

	/// <summary>
  /// A <see cref="ESRI.ArcGIS.Client.GroupLayer">GroupLayer</see> based upon the 
  /// <a href="http://en.wikipedia.org/wiki/KML" target="_blank">Keyhole Markup Language</a> (KML) specification that targets 
  /// the Google Maps level of support.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Enhanced functionality to support more KML elements was added in the 
  /// <ESRISILVERLIGHT>ArcGIS API for Silverlight.</ESRISILVERLIGHT>
  /// <ESRIWPF>ArcGIS Runtime SDK for WPF.</ESRIWPF>
  /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone.</ESRIWINPHONE>
  ///  This brings the KML level of functionality very close to that of the KML specification used in 
  /// <a href="http://en.wikipedia.org/wiki/Google_maps" target="_blank">Google Maps</a>.
  /// </para>
  /// <para>
  /// KML is an XML tag based syntax that holds geographic information (geometric shapes, symbology, and attributes) in a 
  /// file with the .kml extension. The KmlLayer also support the KMZ specification which is essentially a set of zipped 
  /// KML files, folders, and other documents (e.g. images, icons, html documents, etc. ) into a single file with the .kmz 
  /// extension. To view the contents of a KMZ file, rename the file with the extension .zip and use a ZIP program to explode 
  /// the contents onto a hard drive location.
  /// </para>
  /// <para><big>KmlLayer is a GroupLayer</big></para>
  /// <para>
  /// The KmlLayer is of Type <see cref="ESRI.ArcGIS.Client.GroupLayer">GroupLayer</see> meaning that certain KML elements 
  /// are parsed out into individual Layers in a <see cref="ESRI.ArcGIS.Client.LayerCollection">LayerCollection</see>. 
  /// The following KML elements are parsed into the ArcGIS Layer types of:
  /// </para>
  /// <list type="table">
  /// <listheader><term>KML Element(s)</term><description>ArcGIS Layer Type</description></listheader>
  /// <item><term>&lt;Placemark&gt;&lt;Point&gt;</term><description><see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see> based upon <see cref="ESRI.ArcGIS.Client.Geometry.MapPoint">MapPoint</see> Geometries</description></item>
  /// <item><term>&lt;Placemark&gt;&lt;LineString&gt;</term><description><see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see> based upon <see cref="ESRI.ArcGIS.Client.Geometry.Polyline">Polyline</see> Geometries</description></item>
  /// <item><term>&lt;Placemark&gt;&lt;Polygon&gt;</term><description><see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see> based upon <see cref="ESRI.ArcGIS.Client.Geometry.Polygon">Polygon</see> Geometries</description></item>
  /// <item><term>&lt;GroundOverlay&gt;</term><description><see cref="ESRI.ArcGIS.Client.ElementLayer">ElementLayer</see></description></item>
  /// </list>
  /// <para>
  /// <b>Note:</b> The KML elements of &lt;Folder&gt;, &lt;Document&gt;, and &lt;NetworkLink&gt; become child KmlLayers (which 
  /// can be recursive in nature depending on the KML/KMZ file) and are parsed into the ArcGIS Layer types noted in the prior table.
  /// Additionally, a KmlLayer can contain other KmlLayers (which again can be recursive). 
  /// </para>
  /// <para>
  /// Drilling into individual Layers and their subsequent atomic level Types of a KmlLayer GroupLayer is accomplished 
  /// via the <see cref="ESRI.ArcGIS.Client.GroupLayerBase.ChildLayers">KmlLayer.ChildLayers</see> Property. See the code 
  /// example in the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.Name">KmlLayer.Name</see> Property 
  /// documentation for one example of drilling into the KmlLayer GroupLayer to obtain detailed information (like: 
  /// Graphic Attributes, Graphic Symbology, Graphic Geometry, ElementLayer ElementType, ElementLayer Opacity, 
  /// ElementLayer Envelope, etc.) that may be useful in creating an application.
  /// </para>
  /// <ESRIWINPHONE><para><b>TIP:</b> For Windows Phone applications, when using a Legend Control buddied with a Map Control that contains a KmlLayer, use the <see cref="M:ESRI.ArcGIS.Client.Toolkit.Legend.LayerItemsMode">Legend.LayerItemsMode</see> = "Flat" in order to see the graphical symbols in the Legend. This is not a KmlLayer specific issue, rather a function of the default Legend template not taking advantage of the TreeView Control hierarchy for GroupLayers.</para></ESRIWINPHONE>
  /// <ESRIWPF><para>There is a known issue of KmlLayer symbols briefly flickering in the upper left corner of the Windows Desktop when a WPF application starts and stops.</para></ESRIWPF>
  /// <para><big>Accessing KML/KMZ on the local hard drive</big></para>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view the geographic information on the client. 
  /// Since the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server is however the recommended server to host KML/KMZ files via geographic web services as 
  /// it generates KML/KMZ files natively as part of the web service publishing process from an ArcMap .mxd. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the following options:
  /// </para>
  /// <ESRISILVERLIGHT><para><b>Option #1:</b> Developers can place the KML/KMZ file in the ClientBin directory of the test web site that is generated when creating a Silverlight application using Visual Studios built in web server (i.e. the directory that has the  path: ..\[APPLICATION_NAME].Web\ClientBin). This option is the easiest method for testing KML/KMZ files when there  is no web browser security issues because all of the KML/KMZ functionality is self contained. See the code example in the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.Url">KmlLayer.Url</see> Property for one common workflow example of Option #1.</para></ESRISILVERLIGHT>
  /// <ESRISILVERLIGHT><para><b>NOTE:</b> If the KML/KMZ file has hyperlinks (i.e. they begin with http://) to resources (such as other KML/KMZ files) to locations outside of your local network, <b>IT WILL BE REQUIRED</b> to use Option #2 for local testing of KML/KMZ files. Some common KML tags (or nested sub-tags) that can use external hyperlinks outside of your local network include the following: &lt;href&gt;, &lt;Style&gt;, &lt;Icon&gt;, &lt;IconStyle&gt;, &lt;StyleMap&gt;, &lt;NetworkLink&gt;, and &lt;styleUrl&gt;. Additionally, if you get a Security Exception or unhandled exception 4004 errors in Visual Studio during your Silverlight application debugging, you will most likely need to use Option #2 instead.</para></ESRISILVERLIGHT>
  /// <ESRISILVERLIGHT><para><b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory of their local web server (i.e. http://localhost). Using this option has the additional requirements of:</para></ESRISILVERLIGHT>
  /// <ESRISILVERLIGHT><list type="bullet"><item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item><item>Adding a crossdomain.xml file to the root of the web server</item><item>Making use of a proxy to avoid Security Exception error messages</item></list></ESRISILVERLIGHT>
  /// <ESRISILVERLIGHT><para>See the code example in the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.ProxyUrl">KmlLayer.ProxyUrl</see> Property for one common workflow example of Option #2.</para></ESRISILVERLIGHT>
  /// <ESRIWPF><para><b>Option #1:</b> Developers can place a KML/KMZ file anywhere on the local hard drive and provide the file path as the KmlLayer.Url (example: Url="C:\TEST_KML_FILES\Test.kml").</para></ESRIWPF>
  /// <ESRIWPF><para><b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory of their local web server (i.e. http://localhost). Using this option has the additional requirements of:</para></ESRIWPF>
  /// <ESRIWPF><list type="bullet"><item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item><item>Adding a crossdomain.xml file to the root of the web server</item><item>Making use of a proxy to avoid Security Exception error messages</item></list></ESRIWPF>
  /// <ESRIWPF><para><b>NOTE:</b> WPF does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though the local KML file may have resource links (i.e. http://) to locations outside of the local network).</para></ESRIWPF>
  /// <ESRIWINPHONE><para><b>Option #1:</b> If the developer has installed a web server on the same computer as Visual Studio (for example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory of their local web server (i.e. http://localhost). Using this option has the additional requirements of:</para></ESRIWINPHONE>
  /// <ESRIWINPHONE><list type="bullet"><item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item><item>Adding a crossdomain.xml file to the root of the web server</item><item>Making use of a proxy to avoid Security Exception error messages</item></list></ESRIWINPHONE>
  /// <ESRIWINPHONE><para><b>NOTE:</b> Windows Phone does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though the local KML file may have resource links (i.e. http://) to locations outside of the local network).</para></ESRIWINPHONE>
  /// <para><big>Supported KML Tags</big></para>
  /// <para>
  /// The following table lists the KML elements supported by the KmlLayer class, and provides additional notes for elements that 
  /// are conditionally supported.
  /// </para>
  /// <list type="table">
  /// <listheader><term>Supported KML Element</term><description>Supportability Notes</description></listheader>
  /// <item>
  ///   <term>&lt;altitudeMode&gt;</term>
  ///   <description>Only 2D supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;atom:author&gt;</term>
  ///   <description>The &lt;atom:author&gt; 'name' attribute is stored in the ESRI.ArcGIS.Client.Graphic as an 
  ///   <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> (where the key = 'name' in the key/value 
  ///   pairs of the IDictionary(Of String, Object)). When this value is defined on a container 
  ///   (&lt;Folder&gt; or &lt;Document&gt;), all &lt;Placemark&gt;'s in the hierarchy inherits from this 
  ///   value.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;atom:link&gt;</term>
  ///   <description>The &lt;atom:link&gt; 'href' attribute is stored in the ESRI.ArcGIS.Client.Graphic as an 
  ///   <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> (where the key = 'atomHRef' in the 
  ///   key/value pairs of the IDictionary(Of String, Object)). When this value is defined on a container 
  ///   (&lt;Folder&gt; or &lt;Document&gt;), all &lt;Placemark&gt;'s in the hierarchy inherits from this 
  ///   value.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;atom:name&gt;</term>
  ///   <description>The &lt;atom:name&gt; 'href' attribute is stored in the ESRI.ArcGIS.Client.Graphic as an 
  ///   <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> (where the key = 'atomHRef' in the key/value 
  ///   pairs of the IDictionary(Of String, Object)). When this value is defined on a container (&lt;Folder&gt; or 
  ///   &lt;Document&gt;), all &lt;Placemark&gt;'s in the hierarchy inherits from this value.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;BalloonStyle&gt;</term>
  ///   <description>For the &lt;BalloonStyle&gt;, the nested &lt;text&gt; tag information is stored in the 
  ///   ESRI.ArcGIS.Client.Graphic as an <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> 
  ///   (where the key = 'balloonText' in the key/value pairs of the IDictionary(Of String, Object)). NOTE: 
  ///   Whatever is specified as information in the &lt;text &gt; tag is used, there is no entity replacement.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;color&gt;</term>
  ///   <description>Includes #AABBGGRR and #BBGGRR.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;colorMode&gt;</term>
  ///   <description>Random mode not supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;coordinates&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;Data&gt;</term>
  ///   <description>Multiple &lt;Data&gt; tags can be nested within the &lt;ExtendedData&gt; tag. The &lt;ExtendedData&gt; 
  ///   tag corresponds to a System.Collections.Generic.List(Of 
  ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData">ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData</see>).
  ///   An &lt;ExtendedData&gt; tag is stored as a single key/value pair in the IDictionary(Of String, Object)) 
  ///   of the <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Graphic.Attributes</see> Property (where the 
  ///   key = 'extendedData' in the key/value pairs of the IDictionary(Of String, Object)). Each &lt;Data&gt; tag 
  ///   holds three attributes: 'name', 'displayname', and 'value' and is stored as a KmlExtendedData object of the 
  ///   System.Collections.Generic.List(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData). Thus, the 
  ///   &lt;Data&gt; 'name' attribute maps to a KmlExtendedData.Name Property; the &lt;Data&gt; 'displayname' attribute 
  ///   maps to the KmlExtendedData.DisplayName Property; and the &lt;Data&gt; 'value' attribute maps to the 
  ///   KmlExtendedData.Value Property.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;description&gt;</term>
  ///   <description>For the &lt;description&gt; tag the information is stored in the ESRI.ArcGIS.Client.Graphic as an 
  ///   <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> (where the key = 'description' in the key/value 
  ///   pairs of the IDictionary(Of String, Object)). NOTE: Whatever is specified as information in the &lt;description&gt; 
  ///   tag is used, there is no entity replacement. The HTML content is allowed but is sanitized to protect from 
  ///   cross-browser attacks; entity replacements of the form $[dataName] are unsupported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;Document&gt;</term>
  ///   <description>From v2.3, the &lt;Document&gt; tag becomes ESRI.ArcGIS.Client.GroupLayers.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;east&gt;</term>
  ///   <description>Part of &lt;LatLonBox&gt;.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;ExtendedData&gt;</term>
  ///   <description>Multiple &lt;Data&gt; tags can be nested within the &lt;ExtendedData&gt; tag. The &lt;ExtendedData&gt; 
  ///   tag corresponds to a System.Collections.Generic.List(Of 
  ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData">ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData</see>).
  ///   An &lt;ExtendedData&gt; tag is stored as a single key/value pair in the IDictionary(Of String, Object)) 
  ///   of the <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Graphic.Attributes</see> Property (where the 
  ///   key = 'extendedData' in the key/value pairs of the IDictionary(Of String, Object)). Each &lt;Data&gt; tag 
  ///   holds three attributes: 'name', 'displayname', and 'value' and is stored as a KmlExtendedData object of the 
  ///   System.Collections.Generic.List(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData). Thus, the 
  ///   &lt;Data&gt; 'name' attribute maps to a KmlExtendedData.Name Property; the &lt;Data&gt; 'displayname' attribute 
  ///   maps to the KmlExtendedData.DisplayName Property; and the &lt;Data&gt; 'value' attribute maps to the 
  ///   KmlExtendedData.Value Property. No support for SchemaData.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;fill&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;Folder&gt;</term>
  ///   <description>From v2.3, the &lt;Folder&gt; tag becomes ESRI.ArcGIS.Client.GroupLayers.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;GroundOverlay&gt;</term>
  ///   <description>From v2.3, an ESRI.ArcGIS.Client.ElementLayer is created by &lt;Folder&gt; and &lt;Document&gt; tags 
  ///   to contain all the &lt;GroundOverlay&gt; tags of the container. Nested tags of &lt;LatLongBox&gt;, &lt;rotation&gt;, 
  ///   &lt;color&gt;, and &lt;icon&gt; are used.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;heading&gt;</term>
  ///   <description>Supported when part of an &lt;IconStyle&gt; element for proper rotation of a point's image.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;hotSpot&gt;</term>
  ///   <description>Supported for Symbol creation.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;href&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;Icon&gt;</term>
  ///   <description>Only the 'href' element of this complex element is supported. Rotation and scaling are supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;IconStyle&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;innerBoundaryIs&gt;</term>
  ///   <description>Only single interior ring supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;kml&gt;</term>
  ///   <description>It's the root element of any KML document.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;LatLonBox&gt;</term>
  ///   <description>Support for &lt;Placemark&gt; tags. Supported from v2.3 for &lt;GroundOverlay&gt; tags.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;LinearRing&gt;</term>
  ///   <description>Supported, but only makes use of the &lt;coordinates&gt; sub element.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;LineString&gt;</term>
  ///   <description>Supported, but only makes use of the &lt;coordinates&gt; sub element.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;LineStyle&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;Link&gt;</term>
  ///   <description>Supported, but only makes use of the &lt;href&gt; sub element.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;MultiGeometry&gt;</term>
  ///   <description>Rendered but displayed as separate features in left side panel.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;name&gt;</term>
  ///   <description>For the &lt;name&gt; tag the information is stored in the ESRI.ArcGIS.Client.Graphic as an 
  ///   <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Attribute</see> (where the key = 'name' in the key/value 
  ///   pairs of the IDictionary(Of String, Object)). NOTE: Whatever is specified as information in the &lt;description&gt; 
  ///   tag is used, there is no entity replacement.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;NetworkLink&gt;</term>
  ///   <description>From v2.3 support for &lt;refreshInterval&gt; tag and 'OnInterval' RefreshMode. Becomes a 
  ///   sub-layer in the ESRI.ArcGIS.Client.GroupLayer.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;NetworkLinkControl&gt;</term>
  ///   <description>'MinRefreshPeriod' supported from v2.3.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;north&gt;</term>
  ///   <description>Supported as part of &lt;LatLonBox&gt;.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;outerBoundaryIs&gt;</term>
  ///   <description>Implicitly from &lt;LinearRing&gt; order.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;outline&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;Placemark&gt;</term>
  ///   <description>Becomes an ESRI.ArcGIS.Client.Graphic in an ESRI.ArcGIS.Client.GraphicsLayer.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;Point&gt;</term>
  ///   <description>Supported, but only makes use of the &lt;coordinates&gt; sub element.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;Polygon&gt;</term>
  ///   <description>Supported, but only makes use of the &lt;OuterBoundaryIs&gt; and &lt;InnerBoundaryIs&gt; 
  ///   sub elements.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;PolyStyle&gt;</term>
  ///   <description>Makes use of the tags &lt;color&gt;, &lt;fill&gt;, and &lt;outline&gt;.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;refreshInterval&gt;</term>
  ///   <description>Supported from v2.3.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;refreshMode&gt;</term>
  ///   <description>'OnInterval' mode supported from v2.3.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;south&gt;</term>
  ///   <description>Supported as part of &lt;LatLonBox&gt;.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;Style&gt;</term>
  ///   <description>Supported, but only &lt;IconStyle&gt;, &lt;LineStyle&gt;, and &lt;PolyStyle&gt; 
  ///   sub-elements are supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;StyleMap&gt;</term>
  ///   <description>Only 'Normal' style supported. 'highlight' style not supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;text&gt;</term>
  ///   <description></description>
  /// </item>
  /// <item>
  ///   <term>&lt;value&gt;</term>
  ///   <description>Replacement of $[geDirections] is not supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;visibility&gt;</term>
  ///   <description>Visibility of containers (&lt;Folder&gt;/&lt;Document&gt;/&lt;NetworkLink&gt;) supported 
  ///   from v2.3. Visibility of features not supported.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;west&gt;</term>
  ///   <description>Supported as part of &lt;LatLonBox&gt;.</description>
  /// </item>
  /// <item>
  ///   <term>&lt;width&gt;</term>
  ///   <description></description>
  /// </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// When the Map initially loads a KmlLayer (Earth Quakes in the last 7 days) will be added that was defined in 
  /// XAML. Click the button to add another KmlLayer (Volcanoes of the World) using code-behind logic.
  /// <para>
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Adding KmlLayers to the Map using XAML and code-behind." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot"&gt;
  /// 
  ///   &lt;!-- Add a Map Control. Zoom to the Central Europe/Mediterranean area. --&gt;
  ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="0,171,0,0" Name="Map1" VerticalAlignment="Top" 
  ///             WrapAround="True" Height="419" Width="405" Extent="-692043,2479159,4226170,7567385"&gt;
  ///     &lt;esri:Map.Layers&gt;
  ///       &lt;esri:LayerCollection&gt;
  ///                   
  ///         &lt;!-- Add a background ArcGISTiledMapServiceLayer for visual reference. --&gt;
  ///         &lt;esri:ArcGISTiledMapServiceLayer 
  ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
  ///           
  ///         &lt;!-- Add a KmlLayer that shows Earthquake occurrences over the last 7 days. Need to use a ProxyUrl. --&gt;
  ///         &lt;esri:KmlLayer ID="Earth Quakes" 
  ///                        Url="http://earthquake.usgs.gov/earthquakes/catalogs/eqs7day-age_src.kmz"
  ///                        ProxyUrl="http://serverapps.esri.com/SilverlightDemos/ProxyPage/proxy.ashx"&gt;
  ///         &lt;/esri:KmlLayer&gt;
  ///         
  ///       &lt;/esri:LayerCollection&gt;
  ///     &lt;/esri:Map.Layers&gt;
  ///   &lt;/esri:Map&gt;
  ///       
  ///   &lt;!-- Add a Legend Control to show the symbology of the Layers in the Map. --&gt;
  ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="403,171,0,0" Name="Legend1" VerticalAlignment="Top" 
  ///                Height="419" Width="237" Map="{Binding ElementName=Map1}" 
  ///                ShowOnlyVisibleLayers="True" LayerItemsMode="Tree" /&gt;
  ///         
  ///   &lt;!-- Add a button that has the Click event wired up. The button will add another KmlLayer via the code-behind. --&gt;
  ///   &lt;Button Content="Add KmlLayer 'Volcanoes of the World' to the Map." Height="23" HorizontalAlignment="Left" 
  ///           Margin="0,144,0,0" Name="Button1" VerticalAlignment="Top" Width="640" Click="Button1_Click"/&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="138" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="640" 
  ///      TextWrapping="Wrap" Text="When the Map initially loads a KmlLayer (Earth Quakes in the last 7 days) will 
  ///              be added that was defined in XAML. Click the button to add another KmlLayer (Volcanoes of the World)
  ///              using code-behind logic." /&gt;
  ///   
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // Add another KmlLayer to the Map.
  ///   
  ///   // Create a new KmlLayer object. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer theKmlLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer();
  ///   
  ///   // Set the KmlLayer's ID.
  ///   theKmlLayer.ID = "Volcanoes of the world";
  ///   
  ///   // Set the Url of the KmlLayer. Note the Url takes a Uri object!
  ///   theKmlLayer.Url = new Uri("http://sites.google.com/site/geined13/tours/Volcanoes_of_the_World.kmz?attredirects=0&amp;d=1");
  ///   
  ///   // Need to use a ProxyUrl on the KmlLayer since the service is not hosted locally or on a local network.
  ///   theKmlLayer.ProxyUrl = "http://serverapps.esri.com/SilverlightDemos/ProxyPage/proxy.ashx";
  ///   
  ///   // Add the KmlLayer to the Map. An automaic refresh of the Map and Legend Controls will occur.
  ///   Map1.Layers.Add(theKmlLayer);
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' Add another KmlLayer to the Map.
  ///   
  ///   ' Create a new KmlLayer object. 
  ///   Dim theKmlLayer As New ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer
  ///   
  ///   ' Set the KmlLayer's ID.
  ///   theKmlLayer.ID = "Volcanoes of the world"
  ///   
  ///   ' Set the Url of the KmlLayer. Note the Url takes a Uri object!
  ///   theKmlLayer.Url = New Uri("http://sites.google.com/site/geined13/tours/Volcanoes_of_the_World.kmz?attredirects=0&amp;d=1")
  ///   
  ///   ' Need to use a ProxyUrl on the KmlLayer since the service is not hosted locally or on a local network.
  ///   theKmlLayer.ProxyUrl = "http://serverapps.esri.com/SilverlightDemos/ProxyPage/proxy.ashx"
  ///   
  ///   ' Add the KmlLayer to the Map. An automaic refresh of the Map and Legend Controls will occur.
  ///   Map1.Layers.Add(theKmlLayer)
  ///   
  /// End Sub
  /// </code>
  /// </example>
	public class KmlLayer : GroupLayer
	{

		// Public Members
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
		/// Identifies the <see cref="ClientCertificateProperty"/> dependency property.
		/// </summary>
		public static readonly System.Windows.DependencyProperty ClientCertificateProperty =
			System.Windows.DependencyProperty.Register("ClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(KmlLayer), new PropertyMetadata(OnClientCertificatePropertyChanged));

		private static void OnClientCertificatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var newValue = (System.Security.Cryptography.X509Certificates.X509Certificate)e.NewValue;
			KmlLayer kmlLayer = (KmlLayer)d;
			if (kmlLayer != null && kmlLayer.ChildLayers != null)
			{
				foreach (KmlLayer layer in kmlLayer.ChildLayers.OfType<KmlLayer>())
				{
					layer.ClientCertificate = newValue;
				}
			}
		}
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
		public static readonly System.Windows.DependencyProperty CredentialsProperty =
			System.Windows.DependencyProperty.Register("Credentials", typeof(System.Net.ICredentials), typeof(KmlLayer), new PropertyMetadata(OnCredentialsPropertyChanged));

		private static void OnCredentialsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			System.Net.ICredentials newValue = (System.Net.ICredentials)e.NewValue;
			KmlLayer kmlLayer = (KmlLayer)d;
			if (kmlLayer != null && kmlLayer.ChildLayers != null)
			{
				foreach (KmlLayer layer in kmlLayer.ChildLayers.OfType<KmlLayer>())
				{
					layer.Credentials = newValue;
				}
			}
		}
#endif

		#region Url

		private Uri _url;

#if SILVERLIGHT && !WINDOWS_PHONE
  /// <summary>
  /// Gets or sets the Url to the KML document.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the followings options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> Developers can place the KML/KMZ file in the ClientBin directory of the test web site that 
  /// is generated when creating a Silverlight application using Visual Studios built in web server (i.e. the directory 
  /// that has the path: ..\[APPLICATION_NAME].Web\ClientBin). This option is the easiest method for testing KML/KMZ files 
  /// when there  is no web browser security issues because all of the KML/KMZ functionality is self contained. See the 
  /// code example in this document for one common workflow example for Option #1.
  /// </para>
  /// <para>
  /// <b>NOTE:</b> If the KML/KMZ file has hyperlinks (i.e. they begin with http://) to resources (such as other KML/KMZ 
  /// files) to locations outside of your local network, <b>IT WILL BE REQUIRED</b> to use Option #2 for local 
  /// testing of KML/KMZ files. Some common KML tags (or nested sub-tags) that can use external hyperlinks outside 
  /// of your local network include the following: &lt;href&gt;, &lt;Style&gt;, &lt;Icon&gt;, &lt;IconStyle&gt;, 
  /// &lt;StyleMap&gt;, &lt;NetworkLink&gt;, and &lt;styleUrl&gt;. Additionally, if you get a Security Exception 
  /// or unhandled exception 4004 errors in Visual Studio during your Silverlight application debugging, you will 
  /// most likely need to use Option #2 instead. 
  /// </para>
  /// <para>
  /// <b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for example: 
  /// Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory of their 
  /// local web server (i.e. http://localhost).  Using this option has the additional requirements of: 
  /// </para>
  /// <list type="bullet">
  /// <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  /// <item>Adding a crossdomain.xml file to the root of the web server</item>
  /// <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para> See the code example in the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.ProxyUrl">KmlLayer.ProxyUrl</see> Property
  /// for one common workflow example of Option #2.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// The following steps show one example of how a developer could test a simple KML file (with no external 
  /// hyperlink dependencies) using Visual Studio’s built-in web server (Cassini):
  /// </para>
  /// <list type="bullet">
  /// <item>Launch Visual Studio 2010</item>
  /// <item>Choose <b>File</b> | <b>New Project</b> from the Visual Studio menus.</item>
  /// <item>
  /// In the <b>New Project</b> dialog, expand .NET Language of your choice (Visual Basic shown in this 
  /// example), click on the <b>Silverlight Template</b>, choose <b>Silverlight Application</b>, and specify the 
  /// following information in the textboxes: 
  /// <list type="bullet">
  /// <item>Name: <b>SilverlightApplication1</b></item>
  /// <item>Location: <b>C:\KML_Test\</b></item>
  /// <item>Solution name: <b>SilverlightApplication1</b></item>
  /// </list>
  /// See the following screen shot:<br/>
  /// <img border="0" alt="Choosing a Silverlight Application in Visual Studio." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.Url.VS_KML_Test_1.png"/>
  /// </item>
  /// <item>
  /// In the <b>New Silverlight Application</b> dialog, accept the defaults (make sure the <b>Host the Silverlight 
  /// application in a new Web site</b> is checked). This will use the Visual Studio built-in web server (Cassini) for 
  /// launching your Silverlight application (see the following screen shot):<br/>
  /// <img border="0" alt="Accepting the defaults in the New Silverlight Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.Url.VS_KML_Test_2.png"/>
  /// </item>
  /// <item>Drag an ESRI Silverlight API <b>Map Control</b> onto the <b>MainPage.xaml</b> design surface.</item>
  /// <item>Add the following additional Reference to the Visual Studio Project: <b>ESRI.ArcGIS.Client.Toolkit.DataSources</b>.</item>
  /// <item>Replace the XAML code in the <b>MainPage.xaml</b> with the following:
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;UserControl x:Class="SilverlightApplication1.MainPage"
  ///              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  ///              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  ///              xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
  ///              xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
  ///              mc:Ignorable="d"
  ///              d:DesignHeight="300" d:DesignWidth="400" 
  ///              xmlns:esri="http://schemas.esri.com/arcgis/client/2009"&gt;
  /// 
  ///   &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
  ///     &lt;esri:Map Background="White" HorizontalAlignment="Left" Name="Map1" VerticalAlignment="Top"
  ///           WrapAround="True" Height="276" Width="376" Margin="12,12,0,0"&gt;
  ///       &lt;esri:Map.Layers&gt;
  ///         &lt;esri:LayerCollection&gt;
  ///           &lt;esri:ArcGISTiledMapServiceLayer 
  ///                 Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
  ///           &lt;esri:KmlLayer Url="Test.kml"/&gt;
  ///         &lt;/esri:LayerCollection&gt;
  ///       &lt;/esri:Map.Layers&gt;
  ///     &lt;/esri:Map&gt;
  ///   &lt;/Grid&gt;
  /// &lt;/UserControl&gt;
  /// </code>
  /// </item>
  /// <item>
  /// Choose <b>Build</b> | <b>Build Solution</b> from the Visual Studio menus (you should not have any 
  /// compiler Errors/Warnings).
  /// </item>
  /// <item>
  /// Using the text editor application, <b>Notepad</b>, copy the following KML syntax and save the file as 
  /// <b>C:\KML_Test\SilverlightApplication1\SilverlightApplication1.Web\ClientBin\Test.kml</b> (this is 
  /// the  same location as where the <b>SilverlightApplication1.xap</b> file gets created when the Visual 
  /// Studio Project gets built).
  /// <code title="Example XAML2" description="" lang="XAML">
  /// &lt;?xml version="1.0" encoding="UTF-8"?&gt;
  /// &lt;kml xmlns="http://www.opengis.net/kml/2.2"&gt;
  ///   &lt;Document&gt;
  ///     &lt;name&gt;Simple Placemark KML&lt;/name&gt;
  ///     &lt;visibility&gt;1&lt;/visibility&gt;
  ///     &lt;open&gt;1&lt;/open&gt;
  ///     &lt;Placemark&gt;
  ///       &lt;name&gt;Simple placemark&lt;/name&gt;
  ///       &lt;visibility&gt;true&lt;/visibility&gt;
  ///       &lt;open&gt;0&lt;/open&gt;
  ///       &lt;Point&gt;
  ///         &lt;coordinates&gt;-122.0822035425683,37.42228990140251,0&lt;/coordinates&gt;
  ///       &lt;/Point&gt;
  ///     &lt;/Placemark&gt;
  ///   &lt;/Document&gt;
  /// &lt;/kml&gt;
  /// </code>
  /// </item>
  /// <item>
  /// Use <b>Windows Explorer</b> to confirm you have a the 
  /// <b>C:\KML_TEST\SilverlightApplication1\SilverlightApplication1.Web\ClientBin\Test.kml</b> file created (see the 
  /// following screen shot):<br/>
  /// <img border="0" alt="Location of the .kml file on disk." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.Url.VS_KML_Test_4.png"/>
  /// </item>
  /// <item>
  /// Hit <b>F5</b> (or click the Start Debugging button) in Visual Studio to launch the Silverlight application in 
  /// Internet Explorer. You should get an image that appears like the following screenshot (a single red point in San 
  /// Francisco):<br/>
  /// <img border="0" alt="Displaying the KML in a Map Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.Url.VS_KML_Test_3.png"/>
  /// </item>
  /// </list>
  /// </example>		
  /// <seealso cref="SetSource"/>
#endif
#if !SILVERLIGHT
  /// <summary>
  /// Gets or sets the Url to the KML document.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the followings options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> Developers can place a KML/KMZ file anywhere on the local hard drive and provide the file 
  /// path as the KmlLayer.Url (example: Url="C:\TEST_KML_FILES\Test.kml").
  /// </para>
  /// <para>
  /// <b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for 
  /// example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application 
  /// directory of their local web server (i.e. http://localhost). Using this option has the additional requirements of:
  /// </para>
  /// <list type="bullet">
  ///   <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  ///   <item>Adding a crossdomain.xml file to the root of the web server</item>
  ///   <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para>
  /// <b>NOTE:</b> WPF does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though the local KML 
  /// file may have resource links (i.e. http://) to locations outside of the local network).
  /// </para>
  /// </remarks>
  /// <seealso cref="SetSource"/>
#endif
#if WINDOWS_PHONE
  /// <summary>
  /// Gets or sets the Url to the KML document.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the followings options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> If the developer has installed a web server on the same computer as Visual Studio (for 
  /// example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory 
  /// of their local web server (i.e. http://localhost). Using this option has the additional requirements of:
  /// </para>
  /// <list type="bullet">
  ///   <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  ///   <item>Adding a crossdomain.xml file to the root of the web server</item>
  ///   <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para>
  /// <b>NOTE:</b> Windows Phone does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though 
  /// the local KML file may have resource links (i.e. http://) to locations outside of the local network).
  /// </para>
  /// </remarks>
  /// <seealso cref="SetSource"/>
#endif
  public Uri Url
		{
			get { return _url; }
			set
			{
				if (_url != value)
				{
					_url = value;
					_isLoading = _isLoaded = false;
					if (IsInitialized)
					{
						IsInitialized = false; // will raise again Initialized event
						if (ChildLayers.Any())
							ChildLayers.Clear();
						Name = null;
						Refresh();
					}
					OnPropertyChanged("Url");
				}
			}
		}

		#endregion

		#region ProxyUrl
		private string _proxyUrl;

#if SILVERLIGHT && !WINDOWS_PHONE
  /// <summary>
  /// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the Silverlight 
  /// client and a KML file.  Use a proxy service when the KML file is not hosted on a site that provides
  /// a cross domain policy file (clientaccesspolicy.xml or crossdomain.xml).
  /// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the following options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> Developers can place the KML/KMZ file in the ClientBin directory of the test web site that 
  /// is generated when creating a Silverlight application using Visual Studios built in web server (i.e. the directory 
  /// that has the path: ..\[APPLICATION_NAME].Web\ClientBin). This option is the easiest method for testing KML/KMZ files 
  /// when there is no web browser security issues because all of the KML/KMZ functionality is self contained. See the 
  /// code example in the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.Url">KmlLayer.Url</see> Property
  /// for one common workflow example of Option #1.
  /// </para>
  /// <para>
  /// <b>NOTE:</b> If the KML/KMZ file has hyperlinks (i.e. they begin with http://) to resources (such as other KML/KMZ 
  /// files) to locations outside of your local network, <b>IT WILL BE REQUIRED</b> to use Option #2 for local 
  /// testing of KML/KMZ files. Some common KML tags (or nested sub-tags) that can use external hyperlinks outside 
  /// of your local network include the following: &lt;href&gt;, &lt;Style&gt;, &lt;Icon&gt;, &lt;IconStyle&gt;, 
  /// &lt;StyleMap&gt;, &lt;NetworkLink&gt;, and &lt;styleUrl&gt;. Additionally, if you get a Security Exception 
  /// or unhandled exception 4004 errors in Visual Studio during your Silverlight application debugging, you will 
  /// most likely need to use Option #2 instead. 
  /// </para>
  /// <para>
  /// <b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for example: 
  /// Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory of their 
  /// local web server (i.e. http://localhost).  Using this option has the additional requirements of: 
  /// </para>
  /// <list type="bullet">
  ///  <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  ///  <item>Adding a crossdomain.xml file to the root of the web server</item>
  ///  <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para> See the code example in this document for one common workflow example of Option #2.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// The following steps show one example of how a developer could test a KML file that is on the local development computer 
  /// with external hyperlink dependencies to another KML file outside of the local network using Microsoft IIS web server on 
  /// a Windows 7 Operating System (it is assumed that IIS is installed):
  /// </para>
  /// <para>
  /// Step 1: Setting up the correct MIME type on the web server to handle KML/KMZ files
  /// </para>
  /// <list type="bullet">
  ///  <item>
  ///  Type <b>InteMgr.exe</b> in the Windows 7 <b>Search programs and files</b> taskbar and hit Enter to launch 
  ///  <b>Internet Information Service (IIS) Manager</b> application (see the following screen shots):<br/>
  ///  <img border="0" alt="Finding and opening the InetMgr.exe application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_1.png"/><br/>
  ///  <img border="0" alt="The Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_2.png"/>
  ///  </item>
  ///  <item>
  ///  Click on the <b>Features View</b> tab and scroll down to the <b>IIS</b> section. Select the <b>MIME Types</b> 
  ///  icon and in the <b>Actions</b> area click the <b>Open Feature</b> hyperlink (see the following screen shot):<br/>
  ///  <img border="0" alt="Add a MIME Type in the Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_3.png"/>
  ///  </item>
  ///  <item>
  ///  Click the <b>Add...</b> hyperlink to launch the <b>Add MIME Type</b> dialog (see the following screen shot):<br/>
  ///  <img border="0" alt="The Add MIME Type dialog in the Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_4.png"/>
  ///  </item>
  ///  <item>
  ///  Add the correct <b>MIME Type</b> for both the KML and KMZ file types and click <b>OK</b> in the dialog for each entry 
  ///  using the values:
  ///  <para><u>KML:</u></para>
  ///   File name extension: <b>.kml</b><br/>
  ///   MIME type: <b>application/vnd.google-earth.kml+xml</b><br/>
  ///  <para/>
  ///  <para><u>KMZ:</u></para>
  ///   File name extension: <b>.kmz</b><br/>
  ///   MIME type: <b>application/vnd.google-earth.kmz</b><br/>
  ///   <para/><para/>
  ///  The <b>Internet Information Service (IIS) Manager</b> application should look like the following screen shot when completed:<br/>
  ///  <img border="0" alt="The Internet Information Services (IIS) Manager application after the KML/KMZ MIME Types are added." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_5.png"/>
  ///  </item>
  ///  <item>Close the <b>Internet Information Service (IIS) Manager</b> application.</item>
  ///  <item>
  ///  <b>NOTE:</b> Refer to the following Google and Microsoft documents for more information on KML/KMZ MIME types in 
  ///  IIS: <a href="http://code.google.com/apis/kml/documentation/kml_tut.html#kml_server" target="_blank">KML Tutorial</a> 
  ///  and <a href="http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/eb5556e2-f6e1-4871-b9ca-b8cf6f9c8134.mspx?mfr=true" target="_blank">Working with MIME Types (IIS 6.0)</a>.
  ///  </item>
  /// </list>
  /// <para>
  /// Step 2: Adding a <b>clientaccesspolicy.xml</b> file to the root of the web server
  /// </para>
  /// <para>
  /// For security reasons, the Silverlight runtime restricts access to data and services for specific classes across 
  /// schemes, domains, and zones. These restrictions impact the ArcGIS Silverlight API when the KML/KMZ file has 
  /// hyperlinks (i.e. they begin with http://) to resources (such as other KML/KMZ files) to locations outside of 
  /// your local network. You need to create a client access policy file on the local web server to enable access 
  /// to resources outside of your local network. Perform the following actions to set up using a 
  /// <b>clientaccesspolicy.xml</b> file:
  /// </para>
  /// <list type="bullet">
  ///  <item>
  ///  Right click on the following hyperlink: 
  ///  <a href="http://services.arcgisonline.com/clientaccesspolicy.xml" target="_blank">http://services.arcgisonline.com/clientaccesspolicy.xml</a> 
  ///  and choose <b>Save Target As...</b> to save a local copy of the <b>clientaccesspolicy.xml</b> file on your local 
  ///  hard drive. This file was created for use on ArcGIS Online but is sufficient for http://locahost testing on 
  ///  a development computer.
  ///  </item>
  ///  <item>
  ///  Copy the <b>clientaccesspolicy.xml</b> file into the <b>C:\inetpub\wwwroot</b> folder of the development 
  ///  machine that has IIS installed. You may need Administrator privileges to copy the file. If you get the 
  ///  <b>Destination Folder Access Denied</b> dialog, just click the <b>Continue</b> button to copy the file 
  ///  (see the following screen shots):<br/>
  ///  <img border="0" alt="Destination Folder Access Denied dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_7.png"/><br/>
  ///  <img border="0" alt="The Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_8.png"/><br/>
  ///  <b>NOTE:</b> For more information on the necessity and use of <b>clientaccesspolicy.xml</b> files refer to 
  ///  the ArcGIS Resource Center Blog document 
  ///  <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2009/08/31/Using-services-across-schemes.aspx" target="_blank">Using services across schemes</a>.
  ///  </item> 
  /// </list>
  /// <para>
  /// Step 3: Making use of a proxy to avoid Security Exception error messages
  /// </para>
  /// <para>
  /// Again as part of the Silverlight runtime security restrictions developers need a proxy service to broker Web 
  /// requests between the Silverlight client and the KML/KMZ files. The proxy adds credential information that 
  /// allows the web server to access the KML/KMZ files. Perform the following actions to set up using a proxy on 
  /// the local web server:
  /// </para>
  /// <list type="bullet">
  ///   <item>Using Windows Explorer, create a <b>C:\ProxyDownload</b> directory on the development computer.</item>
  ///   <item>
  ///   Right click on the following hyperlink: 
  ///   <a href="http://help.arcgis.com/en/webapi/silverlight/help/SLProxyPage.zip" target="_blank">http://help.arcgis.com/en/webapi/silverlight/help/SLProxyPage.zip</a> 
  ///   and choose <b>Save Target As...</b> to save a local copy of the <b>SLProxyPage.zip</b> file to the 
  ///   <b>C:\ProxyDownload</b> directory on your local hard drive. This file was created by ESRI as a starting 
  ///   point for customization in using proxies.
  ///   </item>
  ///   <item>
  ///   Using a ZIP/UNZIP application (like <b>7 Zip</b> or <b>WinZip</b>) extract the compressed <b>SLProxyPage.zip</b> file onto 
  ///   your development computer’s hard drive. This will explode out the folder <b>C:\ProxyDownload\SLProxyPage</b> 
  ///   with three files: <b>proxy.ashx</b>, <b>proxy.config</b>, and <b>ReadMe.txt</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Using Windows Explorer to see where the SLProxyPage.zip was unzipped." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_9.png"/>
  ///   </item>
  ///   <item>
  ///   In <b>Windows Explorer</b>, navigate to the <b>C:\ProxyDownload\SLProxyPage</b> directory, right click on 
  ///   the <b>proxy.config</b> file and choose <b>Properties</b> (see the following screen shot):<br/> 
  ///   <img border="0" alt="Accessing the Properties of the proxy.config file in Using Windows Explorer." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_17.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>proxy.config Properties</b> dialog, uncheck the <b>Read-only</b> attribute and click <b>Apply</b> (see the 
  ///   following screen shot):<br/>
  ///   <img border="0" alt="Removed the Read-only Attribute from the proxy.config file." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_27.png"/>
  ///   </item>
  ///   <item>
  ///   Then click <b>OK</b> to close the <b>proxy.config Properties</b> dialog.
  ///   </item>
  ///   <item>
  ///   In <b>Windows Explorer</b>, right click on the <b>C:\ProxyDownload\SLProxyPage\proxy.config</b> file and 
  ///   choose <b>Open with Notepad</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Using Windows Explorer to open the proxy.config file in Notepad." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_15.png"/>
  ///   </item>
  ///   <item>
  ///   Change the 4th line from: <b>&lt;ProxyConfig mustMatch="true"&gt;</b> to: <b>&lt;ProxyConfig mustMatch="false"&gt;</b>, 
  ///   save the file and close it (see the following screen shot):<br/>
  ///   <img border="0" alt="Using Windows Explorer to open the proxy.config file in Notepad." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_16.png"/>
  ///   </item>
  ///   <item>
  ///   Copy the <b>C:\ProxyDownload\SLProxyPage</b> folder into the <b>C:\InetPub\wwwroot</b> folder of the 
  ///   development machine that has IIS installed. You may need Administrator privileges to copy the file. If you 
  ///   get the <b>Destination Folder Access Denied</b> dialog, just click the <b>Continue</b> button to copy the 
  ///   file (see the following screen shots):<br/>
  ///   <img border="0" alt="Destination Folder Access Denied dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_7.png"/><br/>
  ///   <img border="0" alt="Confirming that the SLPRoxyPage directory was added to the C:\inetpub\wwwroot directoryin Windows Explorer." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_10.png"/>
  ///   </item>
  ///   <item>
  ///   Type <b>InteMgr.exe</b> in the Windows 7 <b>Search programs and files taskbar</b> and hit Enter to launch the
  ///   <b>Internet Information Service (IIS) Manager</b> application (see the following screen shots):<br/>
  ///   <img border="0" alt="Starting the InetMgr.exe application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_1.png"/><br/>
  ///   <img border="0" alt="Viewing the SLProxyPage folder in the Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_11.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>Internet Information Services (IIS) Manager</b> application, right click on the <b>SLProxyPage</b> folder and 
  ///   choose <b>Convert to Application</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Using the Internet Information Services (IIS) Manager application to convert the SLProxyPage folder into an Application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_12.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>Add Application</b> dialog, accept the defaults and click <b>OK</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Accept the defaults in the Add Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_13.png"/>
  ///   </item>
  ///   <item>
  ///   This will change the icon for the <b>SLProxyPage</b> folder to be a web application in IIS (see the following screen shot):<br/>
  ///   <img border="0" alt="Accept the defaults in the Add Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_14.png"/>
  ///   </item>
  ///   <item>
  ///   Close the <b>Internet Information Services (IIS) Manager</b> application.
  ///   </item>
  ///   <item>
  ///   <b>NOTE:</b> To learn more about how proxies work refer to the <b>ArcGIS Resource Center</b> document 
  ///   <a href="http://help.arcgis.com/en/webapi/silverlight/help/index.html#/Secure_services/016600000022000000/" target="_blank">Secure Services</a>. 
  ///   </item>
  /// </list>
  /// <para>
  /// Step 4: Create the KML file that has resource links to external sources outside of the local network and add to IIS as an application folder
  /// </para>
  /// <list type="bullet">
  ///   <item>
  ///   Using <b>Windows Explorer</b>, create a new folder on the hard drive of the development computer. Name the folder 
  ///   <b>C:\TEST_KML_FILES</b>.
  ///   </item>
  ///   <item>Using the text editor application, <b>Notepad</b>, copy the following KML syntax and save the file as 
  ///   <b>C:\ TEST_KML_FILES \Test_w_http_link.kml</b>:
  ///   <code title="Example XAML1" description="" lang="XAML">
  ///   &lt;?xml version="1.0" encoding="UTF-8"?&gt;
  ///   &lt;kml xmlns="http://www.opengis.net/kml/2.2"&gt;
  ///     &lt;Document&gt;
  ///       &lt;name&gt;TEST WITH http LINK&lt;/name&gt;
  ///       &lt;visibility&gt;1&lt;/visibility&gt;
  ///       &lt;open&gt;1&lt;/open&gt;
  ///       &lt;StyleMap id="styleMap"&gt;
  ///         &lt;Pair&gt;
  ///           &lt;key&gt;normal&lt;/key&gt;
  ///           &lt;styleUrl&gt;http://code.google.com/apis/kml/documentation/KML_Samples.kml#normalPlacemark&lt;/styleUrl&gt;
  ///         &lt;/Pair&gt;
  ///         &lt;Pair&gt;
  ///           &lt;key&gt;highlight&lt;/key&gt;
  ///           &lt;styleUrl&gt;http://code.google.com/apis/kml/documentation/KML_Samples.kml#highlightPlacemark&lt;/styleUrl&gt;
  ///         &lt;/Pair&gt;
  ///       &lt;/StyleMap&gt;
  ///         &lt;Placemark&gt;
  ///           &lt;name&gt;Simple placemark&lt;/name&gt;
  ///           &lt;visibility&gt;true&lt;/visibility&gt;
  ///           &lt;open&gt;0&lt;/open&gt;
  ///           &lt;styleUrl&gt;#styleMap&lt;/styleUrl&gt;
  ///           &lt;Point&gt;
  ///             &lt;coordinates&gt;-122.0822035425683,37.42228990140251,0&lt;/coordinates&gt;
  ///           &lt;/Point&gt;
  ///         &lt;/Placemark&gt;
  ///     &lt;/Document&gt;
  ///   &lt;/kml&gt;
  ///   </code>
  ///   </item>
  ///   <item>
  ///   Use <b>Windows Explorer</b> to confirm you have a the <b>C:\KML_TEST_FILES\Test_w_http_link.kml</b> file created (see the following 
  ///   screen shot):<br/>
  ///   <img border="0" alt="Confirming the C:\KML_TEST_FILES\Test_w_http_link.kml file exists in Windows Explorer." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_21.png"/>
  ///   </item>
  ///   <item>
  ///   Copy the <b>C:\TEST_KML_FILES</b> directory to the <b>C:\inetpub\wwwroot</b> folder (see the following screen shot):<br/>
  ///   <img border="0" alt="Using Windows Explorer to copy C:\TEST_KML_FILES to C:\inetpub\wwwroot." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_22.png"/>
  ///   </item>
  ///   <item>
  ///   Type <b>InteMgr.exe</b> in the Windows 7 <b>Search programs and files</b> taskbar and hit Enter to launch <b>Internet Information 
  ///   Service (IIS) Manager</b> application (see the following screen shots):<br/>
  ///   <img border="0" alt="Starting the InetMgr.exe application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_1.png"/><br/>
  ///   <img border="0" alt="Viewing the KML_TEST_FILES folder in the Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_23.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>Internet Information Services (IIS) Manager</b> application, right click on the <b>KML_TEST_FILES</b> folder and 
  ///   choose <b>Convert to Application</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Converting the KML_TEST_FILES folder into an Application in the Internet Information Services (IIS) Manager application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_24.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>Add Application</b> dialog, accept the defaults and click <b>OK</b> (see the following screen shot):<br/>
  ///   <img border="0" alt="Accept the defaults in the Add Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_25.png"/>
  ///   </item>
  ///   <item>
  ///   This will change the icon for the <b>KML_TEST_FILES</b> folder to be a web application in IIS (see the following screen shot):<br/>
  ///   <img border="0" alt="Accept the defaults in the Add Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_26.png"/>
  ///   </item>
  ///   <item>
  ///   Close the <b>Internet Information Services (IIS) Manager</b> application.
  ///   </item>
  /// </list>
  /// <para>
  /// Step 5: Create a Visual Studio application to test the KML file that has references to resources (i.e. other KML files) on different networks
  /// </para>
  /// <list type="bullet">
  ///   <item>Launch Visual Studio 2010.</item>
  ///   <item>Choose <b>File</b> | <b>New Project</b> from the Visual Studio menus.</item>
  ///   <item>
  ///   In the <b>New Project</b> dialog, expand .NET Language of your choice (Visual Basic shown in this 
  ///   example), click on the <b>Silverlight Template</b>, choose <b>Silverlight Application</b>, and specify the 
  ///   following information in the textboxes: 
  ///   <list type="bullet">
  ///     <item>Name: <b>SilverlightApplication2</b></item>
  ///     <item>Location: <b>C:\KML_Test\</b></item>
  ///     <item>Solution name: <b>SilverlightApplication2</b></item>
  ///   </list>
  ///   See the following screen shot:<br/>
  ///   <img border="0" alt="Choosing a Silverlight Application in Visual Studio." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_28.png"/>
  ///   </item>
  ///   <item>
  ///   In the <b>New Silverlight Application</b> dialog, accept the defaults (make sure the <b>Host the Silverlight 
  ///   application in a new Web site</b> is checked). This will use the Visual Studio built-in web server (Cassini) for 
  ///   launching your Silverlight application (see the following screen shot):<br/>
  ///   <img border="0" alt="Accepting the defaults in the New Silverlight Application dialog." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_29.png"/>
  ///   </item>
  ///   <item>Drag an ESRI Silverlight API <b>Map Control</b> onto the <b>MainPage.xaml</b> design surface.</item>
  ///   <item>Add the following additional Reference to the Visual Studio Project: <b>ESRI.ArcGIS.Client.Toolkit.DataSources</b>.</item>
  ///   <item>Replace the XAML code in the <b>MainPage.xaml</b> with the following:
  ///   <code title="Example XAML1" description="" lang="XAML">
  ///   &lt;UserControl x:Class="SilverlightApplication2.MainPage"
  ///       xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  ///       xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  ///       xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
  ///       xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
  ///       mc:Ignorable="d"
  ///       d:DesignHeight="300" d:DesignWidth="400" 
  ///       xmlns:esri="http://schemas.esri.com/arcgis/client/2009"&gt;
  ///   
  ///       &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
  ///       &lt;esri:Map Background="White" HorizontalAlignment="Left" Name="Map1" VerticalAlignment="Top" 
  ///                 WrapAround="True" Height="276" Width="376" Margin="12,12,0,0"&gt;
  ///         &lt;esri:Map.Layers&gt;
  ///           &lt;esri:LayerCollection&gt;
  ///             &lt;esri:ArcGISTiledMapServiceLayer 
  ///               Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
  ///             &lt;esri:KmlLayer Url="http://localhost/KML_TEST_FILES/Test_w_http_link.kml" 
  ///                            ProxyUrl="http://localhost/SLProxyPage/proxy.ashx"/&gt;
  ///           &lt;/esri:LayerCollection&gt;
  ///         &lt;/esri:Map.Layers&gt;
  ///       &lt;/esri:Map&gt;
  ///     &lt;/Grid&gt;
  ///   &lt;/UserControl&gt;
  ///   </code>
  ///   </item>
  ///   <item>
  ///   Choose <b>Build</b> | <b>Build Solution</b> from the Visual Studio menus (you should not have any 
  ///   compiler Errors/Warnings).
  ///   </item>
  ///   <item>
  ///   Hit <b>F5</b> (or click the Start Debugging button) in Visual Studio to launch the Silverlight application in Internet 
  ///   Explorer. You should get an image that appears like the following (a single upside down white tear drop in San Francisco). 
  ///   Although the KML file is hosted locally on the same development machine as the Silverlight application, the 
  ///   <b>Test_w_http_link.kml</b> has a <b>http://</b> link to another KML document outside of our local network (i.e. 
  ///   http://code.google.com/apis/kml/documentation/KML_Samples.kml) (see the following screen shot):<br/>
  ///   <img border="0" alt="Displaying the KML in a Map Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.ProxyUrl.VS_KML_Test_20.png"/>
  ///   </item>
  /// </list>
  /// </example>		
  /// <value>The Proxy URL string.</value>
#endif
#if !SILVERLIGHT
		/// <summary>
		/// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the client application 
		/// and a KML file.  Use a proxy service when the KML file is not hosted on a site that provides
		/// a cross domain policy file (clientaccesspolicy.xml or crossdomain.xml).
		/// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the following options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> Developers can place a KML/KMZ file anywhere on the local hard drive and provide the file 
  /// path as the KmlLayer.Url (example: Url="C:\TEST_KML_FILES\Test.kml").
  /// </para>
  /// <para>
  /// <b>Option #2:</b> If the developer has installed a web server on the same computer as Visual Studio (for 
  /// example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application 
  /// directory of their local web server (i.e. http://localhost). Using this option has the additional requirements of:
  /// </para>
  /// <list type="bullet">
  ///   <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  ///   <item>Adding a crossdomain.xml file to the root of the web server</item>
  ///   <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para>
  /// <b>NOTE:</b> WPF does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though the local KML 
  /// file may have resource links (i.e. http://) to locations outside of the local network).
  /// </para>
  /// </remarks>
  /// <value>The Proxy URL string.</value>
#endif
#if WINDOWS_PHONE
		/// <summary>
		/// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the client application 
  /// and a KML file.  Use a proxy service when the KML file is not hosted on a site that provides
		/// a cross domain policy file (clientaccesspolicy.xml or crossdomain.xml).
		/// </summary>
  /// <remarks>
  /// <para>
  /// Accessing a KmlLayer is unique in that ArcGIS Server is not required to view geographic information. Since 
  /// the KmlLayer is based upon a KML or KMZ file, all that is required is a web server to host the KML/KMZ file. 
  /// <b>NOTE:</b> ArcGIS Server has the ability to host geographic web services created in ArcMap as native 
  /// KML/KMZ files. 
  /// </para>
  /// <para>
  /// Developers who wish to test the KmlLayer functionality using KML/KMZ files locally on their 
  /// development machine have the following options:
  /// </para>
  /// <para>
  /// <b>Option #1:</b> If the developer has installed a web server on the same computer as Visual Studio (for 
  /// example: Internet Information Server (IIS)), then they can place the KML/KMZ file in an application directory 
  /// of their local web server (i.e. http://localhost). Using this option has the additional requirements of:
  /// </para>
  /// <list type="bullet">
  ///   <item>Setting up the correct MIME type on the web server to handle KML/KMZ files</item>
  ///   <item>Adding a crossdomain.xml file to the root of the web server</item>
  ///   <item>Making use of a proxy to avoid Security Exception error messages</item>
  /// </list>
  /// <para>
  /// <b>NOTE:</b> Windows Phone does not use proxies so the use of KmlLayer.ProxyUrl is not necessary (even though 
  /// the local KML file may have resource links (i.e. http://) to locations outside of the local network).
  /// </para>
  /// </remarks>
  /// <value>The Proxy URL string.</value>
#endif
		public string ProxyUrl
		{
			get { return _proxyUrl; }
			set
			{
				if (_proxyUrl != value)
				{
					_proxyUrl = value;
					foreach (KmlLayer layer in ChildLayers.OfType<KmlLayer>())
					{
						layer.ProxyUrl = _proxyUrl;
					}
					OnPropertyChanged("ProxyUrl");
				}
			}
		} 
		#endregion

		#region DisableClientCaching
		private bool _disableClientCaching;

		/// <summary>Disables caching an KLM document on the client.</summary>
		/// <remarks>
		/// <para>
		/// The default value is false. If true, adds a timestamp parameter ("_ts") to the request to prevent 
		/// loading a KML document from the browser's cache.
		/// </para>
		/// </remarks>
		public bool DisableClientCaching
		{
			get { return _disableClientCaching; }
			set
			{
				if (_disableClientCaching != value)
				{
					_disableClientCaching = value;
					foreach (KmlLayer layer in ChildLayers.OfType<KmlLayer>())
					{
						layer.DisableClientCaching = _disableClientCaching;
					}

					OnPropertyChanged("DisableClientCaching");
				}
			}
		}
		
		#endregion

		#region MapTip
#if !WINDOWS_PHONE
		private FrameworkElement _mapTip;

#if SILVERLIGHT && !WINDOWS_PHONE
  /// <summary>
  /// Gets or sets the MapTip displayed when the mouse hovers on a 
  /// <see cref="ESRI.ArcGIS.Client.Graphic">Graphic</see> in the KmlLayer (or its sub-layers).
  /// </summary>
  /// <seealso cref="ESRI.ArcGIS.Client.GraphicsLayer.MapTip"/>
  /// <remarks>
  /// <para>
  /// A KmlLayer.MapTip is a FrameworkElement that displays a visual popup containing information associated 
  /// with a Graphic. Defining the User Interface (UI) look of the FrameworkElement for a KmlLayer.MapTip can 
  /// be done in either XAML (see the code example in this document) or code-behind.
  /// </para>
  /// <para>        
  /// There are several sources of where the information that is displayed in a KmlLayer.MapTip can come from:
  /// <list type="bullet">
  /// <item>
  /// The information is stored in the 
  /// <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Graphic.Attributes</see> of the KmlLayer.
  /// </item>
  /// <item>The information is hard coded</item>
  /// <item>The information is generated on the fly based upon user interaction with the Map</item>
  /// </list>
  /// </para>
  /// <para>
  /// You can use a binding expression in XAML to bind the Attributes of the GraphicsLayer (embedded in the KmlLayer) to the 
  /// <a href="http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.datacontext(v=vs.95).aspx" target="_blank">DataContext</a> 
  /// Property of the Graphic. The general usage syntax follows the pattern:<br></br>
  /// <code lang="XAML">
  /// &lt;esri:KmlLayer&gt;
  ///   &lt;esri:KmlLayer.MapTip&gt;
  ///     &lt;StackPanel Orientation="Horizontal" Background="White"&gt;
  ///       &lt;TextBlock Text="KML Placemark Name:" /&gt;
  ///       &lt;TextBlock Text="{Binding [SomeAttributeName]}" /&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/esri:KmlLayer.MapTip&gt;
  /// &lt;esri:KmlLayer&gt;
  /// </code>
  /// </para>
  /// <para>
  /// <b>Tip:</b> Developers can perform DataContext binding directly to 
  /// Dictionary Keys by specifying the Key name in brackets. Therefore when binding the DataContext of 
  /// a KmlLayer.MapTip to a specific attribute name in the Graphic.Attributes (which is a Dictionary), 
  /// encase the attribute name in square brackets (i.e. []). Example: 
  /// <b>&lt;TextBlock Text="{Binding [name]}" /&gt;</b> or the slightly more verbose version 
  /// <b>&lt;TextBlock Text="{Binding Path=[name]}" /&gt;</b>).
  /// </para>
  /// <para>
  /// The following KML tags map 
  /// to Attributes in a GraphicsLayer that can be used for binding to a KmlLayer.MapTip: &lt;atom:author&gt; 'name' 
  /// attribute, &lt;atom:link&gt; 'href' attribute, ' &lt;atom:name&gt; 'href' attribute, &lt;BalloonStyle&gt;&lt;text&gt; 
  /// information, &lt;description&gt; information, &lt;name&gt;, and &lt;ExtendedData&gt;.
  /// </para>
  /// <para>
  /// The &lt;ExtendedData&gt; tag maps internally to the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData">ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData</see> 
  /// Class. Each KmlExtendedData object has three Properties: 'DisplayName', 'Name', and 'Value' that can 
  /// have attribute information. In order to use binding from a KmlLayer.MapTip to a KmlExtendedData object, 
  /// developers must create their own custom converter. See the code example in this document for one possible 
  /// way to use a custom converter for the KmlExtendedData Class. Additional discussion on the use of the 
  /// KML &lt;ExtendedData&gt; tags can also be found in <b>ArcGIS Resource Center</b> in the Forum thread entitled: 
  /// <a href="http://forums.arcgis.com/threads/27927-KMLLayer-use-of-identify-or-maptips" target="_blank">KMLLayer use of identify or maptips</a>.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// When the application loads the KmlLayer.MapTip will be wired up automatically in XAML. Move your cursor over the 
  /// the red circles to see MapTip information about that large city. Follow the <b>SPECIAL INSTRUCTIONS</b> to create
  /// this intermediate level of sample application.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Displaying a KmlLayer with MapTips in a Map Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.MapTip2.png"/>
  /// </para>
  /// <para>
  /// <b>SPECIAL INSTRUCTIONS:</b>
  /// </para>
  /// <para>
  /// The following steps show one example of how a developer could test a KML file (with no external 
  /// hyperlink dependencies) using Visual Studio’s built-in web server (Cassini) to take advantage of the KmlLayer.MapTip Property:
  /// </para>
  /// <list type="bullet">
  /// <item>Launch Visual Studio 2010</item>
  /// <item>Choose <b>File</b> | <b>New Project</b> from the Visual Studio menus.</item>
  /// <item>
  /// In the <b>New Project</b> dialog, expand .NET Language of your choice (Visual Basic shown in this 
  /// example), click on the <b>Silverlight Template</b>, choose <b>Silverlight Application</b>, and specify the 
  /// following information in the textboxes: 
  /// <list type="bullet">
  /// <item>Name: <b>KmlLayer_MapTip_Test</b></item>
  /// <item>Location: <b>C:\KML_Test\</b></item>
  /// <item>Solution name: <b>KmlLayer_MapTip_Test</b></item>
  /// </list>
  /// See the following screen shot:<br/>
  /// <img border="0" alt="Choosing a Silverlight Application in Visual Studio." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.MapTip1.png"/>
  /// </item>
  /// <item>
  /// In the <b>New Silverlight Application</b> dialog, accept the defaults (make sure the <b>Host the Silverlight 
  /// application in a new Web site</b> is checked). This will use the Visual Studio built-in web server (Cassini) for 
  /// launching your Silverlight application.
  /// </item>
  /// <item>
  /// Drag an ESRI Silverlight API <b>Map Control</b> onto the <b>MainPage.xaml</b> design surface.
  /// </item>
  /// <item>
  /// Add the following additional Reference to the Visual Studio Project: <b>ESRI.ArcGIS.Client.Toolkit.DataSources</b>.
  /// </item>
  /// <item>
  /// Choose <b>Project</b> | <b>Add Class</b> from the Visual Studio menus. In the 
  /// <b>Add new Item – KmlLayer_MapTip_Test</b> dialog, specify <b>ExtendedDataConverter.vb</b> or <b>ExtendedDataConverter.cs</b> 
  /// for the <b>Name:</b> depending on the .NET Language used and click the <b>Add</b> button (see the following screen shot):<br/>
  /// <img border="0" alt="Adding the ExtendedDataConverter Class to the Visual Studio Project." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.MapTip1a.png"/>
  /// </item>
  /// <item>
  /// Replace the <b>ExtendedDataConverter.vb</b> or <b>ExtendedDataConverter.cs</b> with the following:<br/>
  /// <b><u>C# code:</u></b><br/>
  /// <code title="Example CS1" description="" lang="CS">
  /// // This is a custom Class used to convert the complex KmlExtendedData object with it's three Properties 
  /// // (DisplayName, Name, and Value) into a simple string that can be used for binding to the KmlLayer.MapTip.
  /// // In your application add a new class called 'ExtendedDataConverter.vb' and replace the contents of that
  /// // Class with this code.
  /// 
  /// public class ExtendedDataConverter : Data.IValueConverter
  /// {
  ///   // This is the function that does the work of the conversion.
  ///   public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
  ///   {
  ///   	 // Create a variable that will be used to return something.
  ///   	 string theReturnValue = null;
  ///     
  ///   	 // Ensure we have the KmlExtendedData in an IList.
  ///   	 if (value is IList&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt;)
  ///   	 {
  ///   	   // Cast the input 'value' object of the converter to the correct Type.
  ///   	   IList&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt; theIList = (IList&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt;)value;
  ///       
  ///   	   // Obtain the first KmlExtendedData object from the IList.
  ///   	   ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData theKmlExtendedData = theIList.FirstOrDefault();
  ///       
  ///   	   // Depending on what passed as the ConverterParameter (which is the input argument 'parameter') in XAML will 
  ///   	   // determine what we Return back. The options are: 'Value', 'DisplayName', and 'Name'.
  ///   	   if (parameter.ToString() == "Value")
  ///   	   {
  ///      		 theReturnValue = theKmlExtendedData.Value;
  ///   	   }
  ///   	   else if (parameter.ToString() == "DisplayName")
  ///   	   {
  ///      	 	theReturnValue = theKmlExtendedData.DisplayName;
  ///   	   }
  ///   	   else if (parameter.ToString() == "Name")
  ///   	   {
  ///     		  theReturnValue = theKmlExtendedData.Name;
  /// 	     }
  /// 	   }  
  /// 	   // Return something back.
  /// 	   return theReturnValue;
  ///   }
  /// 
  ///   // This function is necessary because we implement the Data.IValueConverter Interface. Hence we must have the signature
  ///   // defined even though we will not really be doing any ConvertBack operations in this example.
  ///   public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
  ///   {
  ///     throw new System.NotImplementedException();
  ///   }
  /// }
  /// </code>
  /// <b><u>VB.NET code:</u></b><br/>
  /// <code title="ExtendedDataConverter.vb" description="ExtendedDataConverter.vb" lang="VB.NET">
  /// ' This is a custom Class used to convert the complex KmlExtendedData object with it's three Properties 
  /// ' (DisplayName, Name, and Value) into a simple string that can be used for binding to the KmlLayer.MapTip.
  /// ' In your application add a new class called 'ExtendedDataConverter.vb' and replace the contents of that
  /// ' Class with this code.
  ///   
  /// Public Class ExtendedDataConverter
  ///   Implements Data.IValueConverter
  ///   
  ///   ' This is the function that does the work of the conversion.
  ///   Public Function Convert(ByVal value As Object,
  ///                           ByVal targetType As System.Type,
  ///                           ByVal parameter As Object,
  ///                           ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
  ///     
  ///     ' Create a variable that will be used to return something.
  ///     Dim theReturnValue As String = Nothing
  ///     
  ///     ' Ensure we have the KmlExtendedData in an IList.
  ///     If TypeOf value Is IList(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData) Then
  ///       
  ///       ' Cast the input 'value' object of the converter to the correct Type.
  ///       Dim theIList As IList(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData) =
  ///        CType(value, IList(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData))
  ///       
  ///       ' Obtain the first KmlExtendedData object from the IList.
  ///       Dim theKmlExtendedData As ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData = theIList.FirstOrDefault()
  ///       
  ///       ' Depending on what passed as the ConverterParameter (which is the input argument 'parameter') in XAML will 
  ///       ' determine what we Return back. The options are: 'Value', 'DisplayName', and 'Name'.
  ///       If parameter.ToString = "Value" Then
  ///         theReturnValue = theKmlExtendedData.Value
  ///       ElseIf parameter.ToString = "DisplayName" Then
  ///         theReturnValue = theKmlExtendedData.DisplayName
  ///       ElseIf parameter.ToString = "Name" Then
  ///         theReturnValue = theKmlExtendedData.Name
  ///       End If
  /// 
  ///     End If
  ///     
  ///     ' Return something back.
  ///     Return theReturnValue
  ///     
  ///   End Function
  /// 
  ///   ' This function is necessary because we implement the Data.IValueConverter Interface. Hence we must have the signature
  ///   ' defined even though we will not really be doing any ConvertBack operations in this example.
  ///   Public Function ConvertBack(value As Object,
  ///                               targetType As System.Type,
  ///                               parameter As Object,
  ///                               culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
  ///     Throw New System.NotImplementedException()
  ///   End Function
  /// 
  /// End Class
  /// </code>
  /// </item>
  /// <item>Replace the XAML code in the <b>MainPage.xaml</b> with the following:
  /// <code title="MainPage.xaml" description="MainPage.xaml" lang="XAML">
  /// &lt;UserControl x:Class="KmlLayer_MapTip_Test.MainPage"
  ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  ///     xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
  ///     xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
  ///     mc:Ignorable="d"
  ///     d:DesignHeight="480" d:DesignWidth="640" 
  ///     xmlns:esri="http://schemas.esri.com/arcgis/client/2009"
  ///     xmlns:local="clr-namespace:KmlLayer_MapTip_Test"&gt;
  ///     
  ///   &lt;Grid x:Name="LayoutRoot"&gt;
  ///     
  ///     &lt;!-- 
  ///     Add some resources that will used by our XAML page. 
  ///         
  ///     You will need to add the correct 'local' namespace reference to the top of this XAML page that makes use of 
  ///     custom 'ExtendedDataConverter' Class. In this code example, we used: 
  ///     xmlns:local="clr-namespace:KmlLayer_MapTip_Test" 
  ///     near the top of the XAML file because our Visual Studio project was named 'KmlLayer_MapTip_Test'. 
  ///     --&gt;
  ///     &lt;Grid.Resources&gt;
  /// 
  ///       &lt;!-- 
  ///       The x:Key of "extendedDataConverter" is what will be used as the StaticResource when trying  bind a 
  ///       KmlLayer.MapTip to the KmlExtendedData Type in the .kml file.
  ///       --&gt;
  ///       &lt;local:ExtendedDataConverter x:Key="extendedDataConverter" /&gt;
  ///     &lt;/Grid.Resources&gt;
  ///     
  ///     &lt;!-- Add a Map Control. Zoom to the Continental US. --&gt;
  ///     &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="0,169,0,0" Name="Map1" VerticalAlignment="Top" 
  ///               WrapAround="True" Height="299" Width="628" Extent="-128.59,22.28,-66.71,51.75"&gt;
  ///       &lt;esri:Map.Layers&gt;
  ///         &lt;esri:LayerCollection&gt;
  ///           
  ///           &lt;!-- Add a background ArcGISTiledMapServiceLayer for visual reference. --&gt;
  ///           &lt;esri:ArcGISTiledMapServiceLayer 
  ///             Url="http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer" /&gt;
  /// 
  ///           &lt;!-- 
  ///           Add a KmlLayer to the Map.  In this Silverlight Application example, we are testing against a .kml
  ///           file that is on the local hard drive in the path: 
  ///           C:\KML_Test\KmlLayer_MapTip_Test\KmlLayer_MapTip_Test.Web\ClientBin. 
  ///           Review the API documentation for the ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer Class for the 
  ///           various options of how you can Access KML/KMZ files on the local hard drive. See the example code 
  ///           documentation in the ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer.MapTip Property for the structure 
  ///           of the .kml file.
  ///           --&gt;
  ///           &lt;esri:KmlLayer ID="BIG US CITIES" Url="BIG_US_CITIES.kml"&gt;
  ///             
  ///             &lt;!-- Set up using a MapTip on the KmlLayer. --&gt;
  ///             &lt;esri:KmlLayer.MapTip&gt;
  ///               &lt;Grid Background="White"&gt;
  ///                 &lt;Border BorderBrush="SeaGreen" BorderThickness="10" Name="Border1"&gt;
  ///                   &lt;StackPanel&gt;
  ///                     &lt;StackPanel Orientation="Horizontal" Name="StackPanel_name"&gt;
  ///                       &lt;TextBlock Text="Name: " FontWeight="Bold" FontSize="10" VerticalAlignment="Center" /&gt;
  ///                         
  ///                       &lt;!-- For each KML Placemark bind to the &lt;name&gt; tag. --&gt;
  ///                       &lt;TextBlock Text="{Binding [name]}" HorizontalAlignment="Left" VerticalAlignment="Center"/&gt;
  ///                     &lt;/StackPanel&gt;
  ///                     &lt;StackPanel Orientation="Horizontal" Name="StackPanel_description"&gt;
  ///                       &lt;TextBlock Text="Nickname: " FontWeight="Bold" FontSize="10" VerticalAlignment="Center"/&gt;
  ///                         
  ///                       &lt;!-- For each KML Placemark bind to the &lt;description&gt; tag. --&gt;
  ///                       &lt;TextBlock Text="{Binding [description]}" HorizontalAlignment="Left" VerticalAlignment="Center"/&gt;
  ///                     &lt;/StackPanel&gt;
  ///                     &lt;StackPanel Orientation="Horizontal" Name="StackPanel_value"&gt;
  ///                     
  ///                       &lt;!-- 
  ///                       For each KML Placemark bind to the &lt;ExtendedData&gt; tag. 
  ///  
  ///                       This option is more complicated because the &lt;ExtendedData&gt; tag maps internally to the 
  ///                       ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData Class. Each KmlExtendedData object
  ///                       has three Properties: 'DisplayName', 'Name', and 'Value' that can have attribute information.
  ///                                             
  ///                       A custom converter (ExtendedDataConverter.vb or ExtendedDataConverter.cs) was added to this
  ///                       project that helps to convert one of the three attributes in the KmlExtendedData object into
  ///                       something that is bindable for display in the MapTip.
  ///                       
  ///                       When looking at the binding syntax for the Text="" Property the following applies:
  ///                       
  ///                       (1) Binding [extendedData] ==&gt; extendedData is the internal attribute name of the KmlExtenedData object. 
  ///                       Hence you are binding to this Attribute just as you would a normal String but in this case you are 
  ///                       binding to an object that can have any of three attribute values ('DisplayName', 'Name', and 'Value').
  ///                                             
  ///                       (2) Converter={StaticResource extendedDataConverter} ==&gt; extendedDataConverter is the StaticResource
  ///                       that was defined in the &lt;Grid.Resources&gt; above.
  ///                       
  ///                       (3) ConverterParameter=Name or ConverterParameter=Value ==&gt; this is the 'parameter' argument passed into 
  ///                       the Convert function of the custom converter ExtendedDataConverter Class. For example: giving the 'Name' 
  ///                       parameter will retrieve the KmlExtendedData.Name Property as a String and giving the 'Value' parameter
  ///                       will retrieve the KmlExtendedData.Value Property as a String.
  ///                       --&gt;
  ///                       
  ///                       &lt;TextBlock 
  ///                         Text="{Binding [extendedData], Converter={StaticResource extendedDataConverter}, ConverterParameter=Name}" 
  ///                         FontWeight="Bold" FontSize="10" VerticalAlignment="Center"/&gt;
  ///                       &lt;TextBlock Text=": " FontWeight="Bold" FontSize="10" VerticalAlignment="Center"/&gt;
  ///                       &lt;TextBlock 
  ///                         Text="{Binding [extendedData], Converter={StaticResource extendedDataConverter}, ConverterParameter=Value}" 
  ///                         HorizontalAlignment="Left" VerticalAlignment="Center"/&gt;
  ///                         
  ///                     &lt;/StackPanel&gt;
  ///                   &lt;/StackPanel&gt;
  ///                 &lt;/Border&gt;
  ///               &lt;/Grid&gt;
  ///             &lt;/esri:KmlLayer.MapTip&gt;
  ///           &lt;/esri:KmlLayer&gt;
  ///           
  ///         &lt;/esri:LayerCollection&gt;
  ///       &lt;/esri:Map.Layers&gt;
  ///     &lt;/esri:Map&gt;
  /// 
  ///     &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///     &lt;TextBlock Height="138" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="628" 
  ///        TextWrapping="Wrap" Text="When the application loads the KmlLayer.MapTip will be wired up automatically in 
  ///        XAML. Move your cursor over the the red circles to see MapTip information about that large city. Follow the 
  ///        SPECIAL INSTRUCTIONS to create this intermediate level of sample application." /&gt;
  ///   &lt;/Grid&gt;
  ///   
  /// &lt;/UserControl&gt;
  /// </code>
  /// </item>
  /// <item>
  /// Choose <b>Build</b> | <b>Build Solution</b> from the Visual Studio menus (you should not have any 
  /// compiler Errors/Warnings).
  /// </item>
  /// <item>
  /// Using the text editor application, <b>Notepad</b>, copy the following KML syntax and save the file as 
  /// <b> C:\KML_Test\KmlLayer_MapTip_Test\KmlLayer_MapTip_Test.Web\ClientBin\BIG_US_CITIES.kml</b> (this is 
  /// the same location as where the <b>SilverlightApplication1.xap</b> file gets created when the Visual 
  /// Studio Project gets built).
  /// <code title="BIG_US_CITIES.kml" description="BIG_US_CITIES.kml" lang="KML">
  /// &lt;?xml version="1.0" encoding="UTF-8"?&gt;
  /// &lt;kml xmlns="http://www.opengis.net/kml/2.2"&gt;
  ///   &lt;Document&gt;
  ///     &lt;name&gt;Big US Cities&lt;/name&gt;
  ///     &lt;visibility&gt;1&lt;/visibility&gt;
  ///     &lt;open&gt;1&lt;/open&gt;
  ///     &lt;Placemark&gt;
  ///       &lt;name&gt;New York City&lt;/name&gt;
  ///       &lt;description&gt;The Big Apple&lt;/description&gt;
  ///       &lt;ExtendedData&gt;
  ///         &lt;Data name="Population"&gt;
  ///           &lt;value&gt;8,175,133&lt;/value&gt;
  ///         &lt;/Data&gt;
  ///       &lt;/ExtendedData&gt;
  ///       &lt;visibility&gt;true&lt;/visibility&gt;
  ///         &lt;open&gt;0&lt;/open&gt;
  ///           &lt;Point&gt;
  ///             &lt;coordinates&gt;-74.006,40.714,0&lt;/coordinates&gt;
  ///           &lt;/Point&gt;
  ///     &lt;/Placemark&gt;
  ///     &lt;Placemark&gt;
  ///       &lt;name&gt;Los Angeles&lt;/name&gt;
  ///       &lt;description&gt;City of Angles&lt;/description&gt;
  ///       &lt;ExtendedData&gt;
  ///         &lt;Data name="Population"&gt;
  ///           &lt;value&gt;3,792,621&lt;/value&gt;
  ///         &lt;/Data&gt;
  ///       &lt;/ExtendedData&gt;
  ///       &lt;visibility&gt;true&lt;/visibility&gt;
  ///         &lt;open&gt;0&lt;/open&gt;
  ///           &lt;Point&gt;
  ///             &lt;coordinates&gt;-118.243,34.052,0&lt;/coordinates&gt;
  ///           &lt;/Point&gt;
  ///     &lt;/Placemark&gt;
  ///     &lt;Placemark&gt;
  ///       &lt;name&gt;Chicago&lt;/name&gt;
  ///       &lt;description&gt;The Windy City&lt;/description&gt;
  ///       &lt;ExtendedData&gt;
  ///         &lt;Data name="Population"&gt;
  ///           &lt;value&gt;2,695,598&lt;/value&gt;
  ///         &lt;/Data&gt;
  ///       &lt;/ExtendedData&gt;
  ///       &lt;visibility&gt;true&lt;/visibility&gt;
  ///         &lt;open&gt;0&lt;/open&gt;
  ///           &lt;Point&gt;
  ///             &lt;coordinates&gt;-87.636,41.878,0&lt;/coordinates&gt;
  ///           &lt;/Point&gt;
  ///     &lt;/Placemark&gt;
  ///   &lt;/Document&gt;
  /// &lt;/kml&gt;
  /// </code>
  /// </item>
  /// <item>
  /// Use <b>Windows Explorer</b> to confirm you have a the 
  /// <b>C:\KML_TEST\SilverlightApplication1\SilverlightApplication1.Web\ClientBin\BIG_US_CITIES.kml</b> file created 
  /// (see the following screen shot):<br/>
  /// <img border="0" alt="Confirming the KML file has been created in the correct location." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.MapTip1b.png"/>.
  /// </item>
  /// <item>
  /// Hit <b>F5</b> (or click the Start Debugging button) in Visual Studio to launch the Silverlight application in 
  /// Internet Explorer. Move you mouse cursor over the various red circles (i.e. Big US Cities) to display the MapTip 
  /// information.
  /// </item>
  /// </list>
  /// </example>
#endif
#if !SILVERLIGHT
  /// <summary>
  /// Gets or sets the MapTip displayed when the mouse hovers on a 
  /// <see cref="ESRI.ArcGIS.Client.Graphic">Graphic</see> in the KmlLayer (or its sub-layers).
  /// </summary>
  /// <seealso cref="ESRI.ArcGIS.Client.GraphicsLayer.MapTip"/>
  /// <remarks>
  /// <para>
  /// A KmlLayer.MapTip is a FrameworkElement that displays a visual popup containing information associated 
  /// with a Graphic. Defining the User Interface (UI) look of the FrameworkElement for a KmlLayer.MapTip can 
  /// be done in either XAML (see the code example in this document) or code-behind.
  /// </para>
  /// <para>        
  /// There are several sources of where the information that is displayed in a KmlLayer.MapTip can come from:
  /// <list type="bullet">
  /// <item>
  /// The information is stored in the 
  /// <see cref="ESRI.ArcGIS.Client.Graphic.Attributes">Graphic.Attributes</see> of the KmlLayer.
  /// </item>
  /// <item>The information is hard coded</item>
  /// <item>The information is generated on the fly based upon user interaction with the Map</item>
  /// </list>
  /// </para>
  /// <para>
  /// You can use a binding expression in XAML to bind the Attributes of the GraphicsLayer (embedded in the KmlLayer) to the 
  /// <a href="http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.datacontext(v=vs.95).aspx" target="_blank">DataContext</a> 
  /// Property of the Graphic. The general usage syntax follows the pattern:<br></br>
  /// <code lang="XAML">
  /// &lt;esri:KmlLayer&gt;
  ///   &lt;esri:KmlLayer.MapTip&gt;
  ///     &lt;StackPanel Orientation="Horizontal" Background="White"&gt;
  ///       &lt;TextBlock Text="KML Placemark Name:" /&gt;
  ///       &lt;TextBlock Text="{Binding [SomeAttributeName]}" /&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/esri:KmlLayer.MapTip&gt;
  /// &lt;esri:KmlLayer&gt;
  /// </code>
  /// </para>
  /// <para>
  /// <b>Tip:</b> Developers can perform DataContext binding directly to 
  /// Dictionary Keys by specifying the Key name in brackets. Therefore when binding the DataContext of 
  /// a KmlLayer.MapTip to a specific attribute name in the Graphic.Attributes (which is a Dictionary), 
  /// encase the attribute name in square brackets (i.e. []). Example: 
  /// <b>&lt;TextBlock Text="{Binding [name]}" /&gt;</b> or the slightly more verbose version 
  /// <b>&lt;TextBlock Text="{Binding Path=[name]}" /&gt;</b>).
  /// </para>
  /// <para>
  /// The following KML tags map 
  /// to Attributes in a GraphicsLayer that can be used for binding to a KmlLayer.MapTip: &lt;atom:author&gt; 'name' 
  /// attribute, &lt;atom:link&gt; 'href' attribute, ' &lt;atom:name&gt; 'href' attribute, &lt;BalloonStyle&gt;&lt;text&gt; 
  /// information, &lt;description&gt; information, &lt;name&gt;, and &lt;ExtendedData&gt;.
  /// </para>
  /// <para>
  /// The &lt;ExtendedData&gt; tag maps internally to the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData">ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData</see> 
  /// Class. Each KmlExtendedData object has three Properties: 'DisplayName', 'Name', and 'Value' that can 
  /// have attribute information. In order to use binding from a KmlLayer.MapTip to a KmlExtendedData object, 
  /// developers must create their own custom converter. See the code example in this document for one possible 
  /// way to use a custom converter for the KmlExtendedData Class. Additional discussion on the use of the 
  /// KML &lt;ExtendedData&gt; tags can also be found in <b>ArcGIS Resource Center</b> in the Forum thread entitled: 
  /// <a href="http://forums.arcgis.com/threads/27927-KMLLayer-use-of-identify-or-maptips" target="_blank">KMLLayer use of identify or maptips</a>.
  /// </para>
  /// </remarks>
#endif
		public FrameworkElement MapTip
		{
			get { return _mapTip; }
			set
			{
				if (_mapTip != value)
				{
					_mapTip = value;

					// Set the maptip recursively
					foreach (Layer layer in ChildLayers)
					{
						if (layer is GraphicsLayer)
							(layer as GraphicsLayer).MapTip = _mapTip;
						else if (layer is KmlLayer)
							(layer as KmlLayer).MapTip = _mapTip;
					}

					OnPropertyChanged("MapTip");
				}
			}
		}
#endif 
		#endregion

		#region Graphics
		/// <summary>
		/// Gets the graphics from all sublayers of this KML layer.
		/// </summary>
		/// <remarks>Only the graphics already downloaded are returned.</remarks>
		/// <value>The graphics.</value>
		public IEnumerable<Graphic> Graphics
		{
			get
			{
				return ChildLayers.SelectMany(LayerGraphics);
			}
		}

		private static IEnumerable<Graphic> LayerGraphics(Layer layer)
		{
			if (layer is GraphicsLayer)
				return ((GraphicsLayer)layer).Graphics;
			if (layer is KmlLayer)
				return ((KmlLayer)layer).Graphics;
			return Enumerable.Empty<Graphic>();
		}

		#endregion

		#region FullExtent
		/// <summary>
		/// The full extent of the layer
		/// </summary>
		/// <value></value>
		public override Envelope FullExtent
		{
			get
			{
				// Union of full extent of child layers
				return ChildLayers.Select(layer => layer.FullExtent).Where(extent => extent != null).Aggregate<Envelope, Envelope>(null, (current, extent) => extent.Union(current));
			}
			protected set
			{
				throw new NotSupportedException();
			}
		} 
		#endregion

		#region Name
		private string _name;

  /// <summary>
  /// Gets the name of the KML document.
  /// </summary>
  /// <value>The name.</value>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// When the application starts, click the Button to load a KmlLayer in the Map. Then click on any child 
  /// KmlLayer.Name in the ListBox (Note: there could be more than one depending on the KmlLayer loaded by the 
  /// Button). This will cause the Map to zoom to the general area of the child KmlLayer and display detailed 
  /// information (e.g. geometry type, symbology, and any attribute information).
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Digging deep into the KmlLayer sub-layers to get detailed information like: geometry, symbology, and attributes." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.KmlLayer.Name.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot"&gt;
  ///   
  ///   &lt;!-- Add a Map Control. --&gt;
  ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="356,144,0,0" Name="Map1" VerticalAlignment="Top" 
  ///             WrapAround="True" Height="204" Width="350" &gt;
  ///     &lt;esri:Map.Layers&gt;
  ///       &lt;esri:LayerCollection&gt;
  ///       
  ///         &lt;!-- Add a background ArcGISTiledMapServiceLayer for visual reference. --&gt;
  ///         &lt;esri:ArcGISTiledMapServiceLayer 
  ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
  ///           
  ///       &lt;/esri:LayerCollection&gt;
  ///     &lt;/esri:Map.Layers&gt;
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- Add a Button that has the Click event wired up. The button will add a KmlLayer via the code-behind. --&gt;
  ///   &lt;Button Content="Step 1 - Add a KmlLayer to the Map." Height="23" HorizontalAlignment="Left" 
  ///           Margin="0,144,0,0" Name="Button1" VerticalAlignment="Top" Width="350" Click="Button1_Click"/&gt;
  ///   
  ///   &lt;!-- Add a TextBlock to give the user instructions to click on the KmlLayer.Name to see detailed information. --&gt;
  ///   &lt;TextBlock Height="67" HorizontalAlignment="Left" Margin="0,173,0,0" Name="TextBlock2" VerticalAlignment="Top" 
  ///              Width="350" TextWrapping="Wrap"
  ///              Text="Step 2 - Click on a sub KmlLayer.Name in the ListBox (below) to zoom to the extent of that sub KmlLayer and display it's detailed information like: geometry, symbology, and attributes." /&gt;
  ///   
  ///   &lt;!-- Add a ListBox. --&gt;
  ///   &lt;ListBox Height="102" HorizontalAlignment="Left" Margin="0,246,0,0" Name="ListBox1" 
  ///            VerticalAlignment="Top" Width="350" SelectionChanged="ListBox1_SelectionChanged"/&gt;
  ///   
  ///   &lt;!-- Add a Textbox to display the KmlLayer sub-layers. --&gt;
  ///   &lt;TextBox Height="246" HorizontalAlignment="Left" Margin="0,354,0,0" Name="TextBox1" VerticalAlignment="Top" 
  ///            Width="706" HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="138" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="706" 
  ///      TextWrapping="Wrap" Text="When the application starts, click the Button to load a KmlLayer in the Map.
  ///      Then click on any child KmlLayer.Name in the ListBox (Note: there could be more than one depending on the 
  ///      KmlLayer loaded by the Button). This will cause the Map to zoom to the general area of the child KmlLayer
  ///      and display detailed information (e.g. geometry type, symbology, and any attribute information)." /&gt;
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function loads a KmlLayer.
  ///   
  ///   // Create a new KmlLayer object.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer theKmlLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer();
  ///   
  ///   // Set an initial ID.
  ///   theKmlLayer.ID = "KML Sample Data";
  ///   
  ///   // Provide a Url for the KML/KMZ files to test.
  ///   theKmlLayer.Url = new Uri("http://kml-samples.googlecode.com/svn/trunk/kml/ExtendedData/lincoln-park-gc-style.kml");
  ///   
  ///   // A few other public KML/KMZ layers to try too!
  ///   //theKmlLayer.Url = new Uri("http://earthquake.usgs.gov/earthquakes/catalogs/eqs7day-age_src.kmz");
  ///   //theKmlLayer.Url = new Uri("http://earthquake.usgs.gov/regional/nca/bayarea/kml/quads.kmz");
  ///   
  ///   // Need to use a ProxyUrl to access the data since it is not in our local network.
  ///   theKmlLayer.ProxyUrl = "http://serverapps.esri.com/SilverlightDemos/ProxyPage/proxy.ashx";
  ///   
  ///   // Wire up an InitializationFailed Event Handler to catch any problems.
  ///   theKmlLayer.InitializationFailed += kmlLayer1_InitializationFailed;
  ///   
  ///   // Wire up the Initialized Event Handler that will list all of the visible sub-layers.
  ///   theKmlLayer.Initialized += kmlLayer1_Initialized;
  ///   
  ///   // Add the KmlLayer to the Map Control.
  ///   Map1.Layers.Add(theKmlLayer);
  /// }
  ///   
  /// private void kmlLayer1_InitializationFailed(object sender, EventArgs e)
  /// {
  ///   // Display a MessageBox with Error information if there is a problem loading the KmlLayer. 
  ///   ESRI.ArcGIS.Client.Layer theLayer = (ESRI.ArcGIS.Client.Layer)sender;
  ///   MessageBox.Show("Error initializing layer: " + theLayer.InitializationFailure.Message);
  /// }
  ///   
  /// private void kmlLayer1_Initialized(object sender, EventArgs e)
  /// {
  ///   // Get the KmlLayer.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer theKmlLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)sender;
  ///   
  ///   // Obtain the ChildLayers (i.e. GroupLayer) from the KmlLayer.
  ///   ESRI.ArcGIS.Client.LayerCollection theLayerCollection = theKmlLayer.ChildLayers;
  ///   
  ///   // Loop through the ChildLayers and display the Layer.ID in the ListBox.
  ///   foreach (ESRI.ArcGIS.Client.Layer theLayer in theLayerCollection)
  ///   {
  ///     string theLayerID = theLayer.ID;
  ///     ListBox1.Items.Add(theLayerID);
  ///     
  ///     // TODO: You could add more logic here to recursively turn on the Visibility of the sub-Layers
  ///     // so that they could be interrogated as well. Sometimes KML/KMZ authors intentionally turn off
  ///     // the Visibility but they could have goody information that can be explored.
  ///   }
  /// }
  ///   
  /// private void ListBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  /// {
  ///   // This function runs when the user clicks on the name of one of the visible sub-layers of the KmlLayer in the ListBox.
  ///   
  ///   // Clear out any previous information in the TextBox.
  ///   TextBox1.Text = "";
  ///   
  ///   // Get the name of the KmlLayer sub-layer.
  ///   string listbox_LayerName = ListBox1.SelectedItem.ToString();
  ///   
  ///   // Create a new StringBuilder to display information about the KmlLayer back to the user.
  ///   System.Text.StringBuilder sb = new System.Text.StringBuilder();
  ///   
  ///   // Set an initial level in the KmlLayer sub-layer hierarchy.
  ///   int level = 0;
  ///   
  ///   // Get the KmlLayer in the in the Map.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer theKmlLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)(Map1.Layers[1]);
  ///   
  ///   // Add the top  level (Parent) information about the KmlLayer to the StringBuilder. 
  ///   sb.Append("KmlLayer Name (Parent - Level " + level.ToString() + "): " + theKmlLayer.Name + Environment.NewLine);
  ///   sb.Append("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" + Environment.NewLine);
  ///   sb.Append(Environment.NewLine);
  ///   
  ///   // Get the child sub-layers (i.e. GroupLayer) of the KmlLayer.
  ///   ESRI.ArcGIS.Client.LayerCollection theLayerCollection = theKmlLayer.ChildLayers;
  ///   
  ///   // Loop through all of the child sub-layers.
  ///   foreach (ESRI.ArcGIS.Client.Layer theLayer in theLayerCollection)
  ///   {
  ///     // Get the Layer.ID
  ///     string theLayerID = theLayer.ID;
  ///     
  ///     // If we have a match with the what was chosen in the ListBox.
  ///     if (theLayerID == listbox_LayerName)
  ///     {
  ///       // Zoom to the FullExtent (and then expanded by 50%) of the Layer chosen in the Listbox .
  ///       // User TODO: Watch out for single point layers. Their extent will return an Envelope 
  ///       // that is a Point and not an area! More coding on your own.
  ///       Map1.Extent = theLayer.FullExtent.Expand(1.5);
  ///       
  ///       // Go into a recursive function that gets details about the sub-layer.
  ///       sb.Append(GoDeep(theLayer, 1)); // 1 is the intial level
  ///     }
  ///   }
  ///   
  ///   // Put the StringBuilder information into the TextBox. 
  ///   TextBox1.Text = sb.ToString();
  /// }
  ///   
  /// // A helper object to track which sub-layer object we are working on.
  /// public class SuperObject
  /// {
  ///   public object theObjects;
  ///   public int theLevel;
  /// }
  ///   
  /// public string GoDeep(object theObject, int theLevel)
  /// {
  ///   // This is a recursive function that gets details about a specific sub-layer.
  ///   
  ///   // If we go more than one time in this recursive function, the SuperObject helps
  ///   // delinate which sub-layer we are operating on.
  ///   if (theObject is SuperObject)
  ///   {
  ///     SuperObject theSuperObject = (SuperObject)theObject;
  ///     theObject = theSuperObject.theObjects;
  ///     theLevel = theSuperObject.theLevel;
  ///   }
  ///   
  ///   // Create a new StringBuilder object to hold the detailed information about the sub-layer.
  ///   Text.StringBuilder sb = new Text.StringBuilder();
  ///   
  ///   // The sub-layer that is passed into this recursive function could be any number of
  ///   // Layer types. Branch into the correct If statement depending on what type of
  ///   // object we are dealing with.
  ///   
  ///   if (theObject is ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)
  ///   {
  ///     // We have a KmlLayer type of object. Will need to recursively dig into more sub-layers 
  ///     // in order to display details that we are interested in.
  ///     
  ///     // Get the KmlLayer.
  ///     ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer theKmlLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)theObject;
  ///     
  ///     // Get the KmlLayer's child sub-layers.
  ///     ESRI.ArcGIS.Client.LayerCollection theLayerCollection = theKmlLayer.ChildLayers;
  ///     
  ///     // Add some KmlLayer information into the StringBuilder.
  ///     sb.Append("KmlLayer Name (Level " + theLevel.ToString() + "): " + theKmlLayer.ID + Environment.NewLine);
  ///     sb.Append("##############################################" + Environment.NewLine);
  ///     sb.Append(Environment.NewLine);
  ///     
  ///     // Loop through all of the sub-layers in the KmlLayer.
  ///     foreach (object theObject2 in theLayerCollection)
  ///     {
  ///       // Create a SuperObject to perform the recursive analysis.
  ///       SuperObject theRecursive_SuperObject = new SuperObject();
  ///       theRecursive_SuperObject.theObjects = theObject2;
  ///       theRecursive_SuperObject.theLevel = theLevel + 1;
  ///       
  ///       // Add detailed sub-layer information to the StringBuilder as a result of a recursive operation.
  ///       sb.Append(GoDeep(theRecursive_SuperObject, theLevel));
  ///     }
  ///   }
  ///   else if (theObject is ESRI.ArcGIS.Client.GraphicsLayer)
  ///   {
  ///     // We have a GraphicsLayer type of object. 
  ///     
  ///     // Display detailed information about each Graphic in the GraphicsLayer
  ///     ESRI.ArcGIS.Client.GraphicsLayer theGraphicsLayer = (ESRI.ArcGIS.Client.GraphicsLayer)theObject;
  ///     string theGraphicsLayerID = theGraphicsLayer.ID;
  ///     
  ///     // Display overall information on the number of Graphics in the GraphicsLayer.
  ///     sb.Append("GraphicsLayer (Level " + theLevel.ToString() + "): " + theGraphicsLayerID + Environment.NewLine);
  ///     sb.Append("================================================" + Environment.NewLine);
  ///     sb.Append("Number of Graphics: " + theGraphicsLayer.Graphics.Count.ToString() + Environment.NewLine);
  ///     sb.Append(Environment.NewLine);
  ///     
  ///     // A counter for the number of Graphics in the GraphicsLayer.
  ///     int graphicsCount = 0;
  ///     
  ///     // Loop through each Graphic in the GraphicsLayer.
  ///     foreach (ESRI.ArcGIS.Client.Graphic theGraphic in theGraphicsLayer)
  ///     {
  ///       // Append which Graphic we are operating on in the StringBuilder.
  ///       sb.Append("Graphic #" + graphicsCount.ToString() + Environment.NewLine);
  ///       
  ///       // Incriment the Graphics counter.
  ///       graphicsCount = graphicsCount + 1;
  ///       
  ///       // ---------------------------------------------------------------------------------------
  ///       
  ///       // Append the Geometry Type of the Graphic to the StringBuilder.
  ///       sb.Append("Geometry Type: " + theGraphic.Geometry.GetType().ToString() + Environment.NewLine);
  ///       
  ///       // Interrogate the specific Geometry Type of the Graphic to display it's coordinate information.
  ///       if (theGraphic.Geometry is ESRI.ArcGIS.Client.Geometry.MapPoint)
  ///       {
  ///         // We have a MapPoint. Display its coordinate information in the StringBuilder.
  ///         ESRI.ArcGIS.Client.Geometry.MapPoint theMapPoint = (ESRI.ArcGIS.Client.Geometry.MapPoint)theGraphic.Geometry;
  ///         sb.Append("Coordinates: " + theMapPoint.ToString() + Environment.NewLine);
  ///       }
  ///       else if (theGraphic.Geometry is ESRI.ArcGIS.Client.Geometry.Polyline)
  ///       {
  ///         // We have a Polyline. Display its coordinate information in the StringBuilder.
  ///         ESRI.ArcGIS.Client.Geometry.Polyline thePolyline = (ESRI.ArcGIS.Client.Geometry.Polyline)theGraphic.Geometry;
  ///         string polylineString = "";
  ///         System.Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Geometry.PointCollection&gt; theObservableCollection_PointCollection = thePolyline.Paths;
  ///         foreach (ESRI.ArcGIS.Client.Geometry.PointCollection thePointCollection in theObservableCollection_PointCollection)
  ///         {
  ///           foreach (ESRI.ArcGIS.Client.Geometry.MapPoint theMapPoint in thePointCollection)
  ///           {
  ///             polylineString = polylineString + theMapPoint.ToString() + ", ";
  ///           }
  ///         }
  ///         sb.Append("Coordinates: " + polylineString + Environment.NewLine);
  ///       }
  ///       else if (theGraphic.Geometry is ESRI.ArcGIS.Client.Geometry.Polygon)
  ///       {
  ///         // We have a Polygon. Display its coordinate information in the StringBuilder.
  ///         ESRI.ArcGIS.Client.Geometry.Polygon thePolygon = (ESRI.ArcGIS.Client.Geometry.Polygon)theGraphic.Geometry;
  ///         string polygonString = "";
  ///         System.Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Geometry.PointCollection&gt; theObservableCollection_PointCollection = thePolygon.Rings;
  ///         foreach (ESRI.ArcGIS.Client.Geometry.PointCollection thePointCollection in theObservableCollection_PointCollection)
  ///         {
  ///           foreach (ESRI.ArcGIS.Client.Geometry.MapPoint theMapPoint in thePointCollection)
  ///           {
  ///             polygonString = polygonString + theMapPoint.ToString() + ", ";
  ///           }
  ///         }
  ///         sb.Append("Coordinates: " + polygonString + Environment.NewLine);
  ///       }
  ///       
  ///       // -------------------------------------------------------------------------------------
  ///       
  ///       // Append the symbology of the Graphic into the StringBuilder.
  ///       ESRI.ArcGIS.Client.Symbols.Symbol theSymbol = theGraphic.Symbol;
  ///       sb.Append("Symbol Type: " + theSymbol.GetType().ToString() + Environment.NewLine);
  ///       
  ///       // -------------------------------------------------------------------------------------
  ///       
  ///       // Interrogate the Attributes of the Graphic.
  ///       
  ///       // Get the Dictionary of Attributes.
  ///       System.Collections.Generic.IDictionary&lt;string, object&gt; theAttributeDictionary = theGraphic.Attributes;
  ///       
  ///       // Get the Keys for the Dictionary.
  ///       System.Collections.Generic.ICollection&lt;string&gt; theAttributeKeys = theAttributeDictionary.Keys;
  ///       
  ///       // Loop through each Key in the Dictionary of Attributes. 
  ///       foreach (string theKey in theAttributeKeys)
  ///       {
  ///         // Get the Value of one Attribute. It could be any number of Types! 
  ///         object theValue = theAttributeDictionary[theKey];
  ///         
  ///         // Get the Type of the Value of the Attribute (we could have a more complex object than a String).
  ///         System.Type theType = theValue.GetType();
  ///         
  ///         // Interrogate the Value Type.
  ///         if (theValue is System.Collections.Generic.List&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt;)
  ///         {
  ///           // We have a List&lt;KmlExtendedData&gt; objects (most likely a result of the &lt;ExtendedData&gt; tag in KML).
  ///           
  ///           // Get the List&lt;KmlExtendedData&gt; object.
  ///           System.Collections.Generic.List&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt; theList = null;
  ///           theList = (System.Collections.Generic.List&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData&gt;)theValue;
  ///           
  ///           // Loop through each KmlExtendedData object in the List&lt;KmlExtendedData&gt;.
  ///           foreach (ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData oneKmlExtendedData in theList)
  ///           {
  ///             // Append the Attribute information of the KmlExtendedData into the StringBuilder.
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.DisplayName" + ", AttributeValue: " + oneKmlExtendedData.DisplayName + Environment.NewLine);
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.Name" + ", AttributeValue: " + oneKmlExtendedData.Name + Environment.NewLine);
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.Value" + ", AttributeValue: " + oneKmlExtendedData.Value + Environment.NewLine);
  ///             sb.Append("----------------------------------------------------" + Environment.NewLine);
  ///           }
  ///         }
  ///         else if (theValue is string)
  ///         {
  ///           // We have a String object.
  ///           
  ///           // This could come from a number of KML tags (ex: &lt;atom:author&gt; 'name' attribute; &lt;atom:link&gt; 'href' attribute;
  ///           // &lt;atom:name&gt; 'href' attribute, &lt;BalloonStyle&gt;&lt;text&gt; information, &lt;description&gt; information; and &lt;name&gt; information).
  ///           sb.Append("AttributeKey: " + theKey + ", AttributeValue: " + theValue.ToString() + Environment.NewLine);
  ///           sb.Append("----------------------------------------------------" + Environment.NewLine);
  ///          }
  ///          else
  ///          {
  ///           // We have some other Type of object. TODO: User to interrogate further!
  ///          }
  ///       }  
  ///       sb.Append(Environment.NewLine);
  ///     }
  ///   }
  ///   else if (theObject is ESRI.ArcGIS.Client.ElementLayer)
  ///   {
  ///     // We have an ElementLayer type of object. 
  ///     
  ///     // Display detailed information about each ElementLayer
  ///     ESRI.ArcGIS.Client.ElementLayer theElementLayer = (ESRI.ArcGIS.Client.ElementLayer)theObject;
  ///     
  ///     // Append the overall information about the ElementLayer in the StringBuilder.
  ///     string theElementLayerID = theElementLayer.ID;
  ///     sb.Append("ElementLayer (Level " + theLevel.ToString() + "): " + theElementLayerID + Environment.NewLine);
  ///     sb.Append("=============================================" + Environment.NewLine);
  ///     sb.Append("Number of Elements: " + theElementLayer.Children.Count.ToString() + Environment.NewLine);
  ///     
  ///     // Append the Extent information about the ElementLayer int the Stirng Builder.
  ///     ESRI.ArcGIS.Client.Geometry.Envelope elementLayerFullExtent = theElementLayer.FullExtent;
  ///     sb.Append("FullExtent: " + elementLayerFullExtent.ToString() + Environment.NewLine);
  ///     sb.Append(Environment.NewLine);
  ///     
  ///     // Loop through each UIElement in the ElementLayer.
  ///     foreach (System.Windows.UIElement oneElement in theElementLayer.Children)
  ///     {
  ///       // Append information about each UIElement in the StringBulder.
  ///       sb.Append("ElementType: " + oneElement.GetType().ToString() + Environment.NewLine);
  ///       sb.Append("Opacity: " + oneElement.Opacity.ToString() + Environment.NewLine);
  ///       sb.Append("Visibility: " + oneElement.Visibility.ToString() + Environment.NewLine);
  ///       sb.Append("Envelope: " + ESRI.ArcGIS.Client.ElementLayer.GetEnvelope(oneElement).ToString() + Environment.NewLine);
  ///       sb.Append(Environment.NewLine);
  ///     }
  ///   }
  ///   // Return the StringBuilder information back to the caller.
  ///   return sb.ToString();
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function loads a KmlLayer.
  ///   
  ///   ' Create a new KmlLayer object.
  ///   Dim theKmlLayer As New ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer
  ///   
  ///   ' Set an initial ID.
  ///   theKmlLayer.ID = "KML Sample Data"
  ///   
  ///   ' Provide a Url for the KML/KMZ files to test.
  ///   theKmlLayer.Url = New Uri("http://kml-samples.googlecode.com/svn/trunk/kml/ExtendedData/lincoln-park-gc-style.kml")
  ///   
  ///   ' A few other public KML/KMZ layers to try too!
  ///   'theKmlLayer.Url = New Uri("http://earthquake.usgs.gov/earthquakes/catalogs/eqs7day-age_src.kmz")
  ///   'theKmlLayer.Url = New Uri("http://earthquake.usgs.gov/regional/nca/bayarea/kml/quads.kmz")
  ///   
  ///   ' Need to use a ProxyUrl to access the data since it is not in our local network.
  ///   theKmlLayer.ProxyUrl = "http://serverapps.esri.com/SilverlightDemos/ProxyPage/proxy.ashx"
  ///   
  ///   ' Wire up an InitializationFailed Event Handler to catch any problems.
  ///   AddHandler theKmlLayer.InitializationFailed, AddressOf kmlLayer1_InitializationFailed
  ///   
  ///   ' Wire up the Initialized Event Handler that will list all of the visible sub-layers.
  ///   AddHandler theKmlLayer.Initialized, AddressOf kmlLayer1_Initialized
  ///   
  ///   ' Add the KmlLayer to the Map Control.
  ///   Map1.Layers.Add(theKmlLayer)
  ///   
  /// End Sub
  /// 
  /// Private Sub kmlLayer1_InitializationFailed(sender As Object, e As EventArgs)
  ///   
  ///   ' Display a MessageBox with Error information if there is a problem loading the KmlLayer. 
  ///   Dim theLayer As ESRI.ArcGIS.Client.Layer = CType(sender, ESRI.ArcGIS.Client.Layer)
  ///   MessageBox.Show("Error initializing layer: " + theLayer.InitializationFailure.Message)
  ///   
  /// End Sub
  ///   
  /// Private Sub kmlLayer1_Initialized(sender As Object, e As EventArgs)
  ///   
  ///   ' Get the KmlLayer.
  ///   Dim theKmlLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)
  ///   
  ///   ' Obtain the ChildLayers (i.e. GroupLayer) from the KmlLayer.
  ///   Dim theLayerCollection As ESRI.ArcGIS.Client.LayerCollection = theKmlLayer.ChildLayers
  ///   
  ///   ' Loop through the ChildLayers and display the Layer.ID in the ListBox.
  ///   For Each theLayer As ESRI.ArcGIS.Client.Layer In theLayerCollection
  ///     Dim theLayerID As String = theLayer.ID
  ///     ListBox1.Items.Add(theLayerID)
  ///     
  ///     ' TODO: You could add more logic here to recursively turn on the Visibility of the sub-Layers
  ///     ' so that they could be interrogated as well. Sometimes KML/KMZ authors intentionally turn off
  ///     ' the Visibility but they could have goody information that can be explored.
  ///   Next
  ///   
  /// End Sub
  /// 
  /// Private Sub ListBox1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)
  ///   
  ///   ' This function runs when the user clicks on the name of one of the visible sub-layers of the KmlLayer in the ListBox.
  ///   
  ///   ' Clear out any previous information in the TextBox.
  ///   TextBox1.Text = ""
  ///   
  ///   ' Get the name of the KmlLayer sub-layer.
  ///   Dim listbox_LayerName As String = ListBox1.SelectedItem.ToString
  ///   
  ///   ' Create a new StringBuilder to display information about the KmlLayer back to the user.
  ///   Dim sb As New Text.StringBuilder
  ///   
  ///   ' Set an initial level in the KmlLayer sub-layer hierarchy.
  ///   Dim level As Integer = 0
  ///   
  ///   ' Get the KmlLayer in the in the Map.
  ///   Dim theKmlLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer = CType(Map1.Layers(1), ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)
  ///   
  ///   ' Add the top  level (Parent) information about the KmlLayer to the StringBuilder. 
  ///   sb.Append("KmlLayer Name (Parent - Level " + level.ToString + "): " + theKmlLayer.Name + vbCrLf)
  ///   sb.Append("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" + vbCrLf)
  ///   sb.Append(vbCrLf)
  ///   
  ///   ' Get the child sub-layers (i.e. GroupLayer) of the KmlLayer.
  ///   Dim theLayerCollection As ESRI.ArcGIS.Client.LayerCollection = theKmlLayer.ChildLayers
  ///   
  ///   ' Loop through all of the child sub-layers.
  ///   For Each theLayer As ESRI.ArcGIS.Client.Layer In theLayerCollection
  ///     
  ///     ' Get the Layer.ID
  ///     Dim theLayerID As String = theLayer.ID
  ///     
  ///     ' If we have a match with the what was chosen in the ListBox.
  ///     If theLayerID = listbox_LayerName Then
  ///       
  ///       ' Zoom to the FullExtent (and then expanded by 50%) of the Layer chosen in the Listbox .
  ///       ' User TODO: Watch out for single point layers. Their extent will return an Envelope 
  ///       ' that is a Point and not an area! More coding on your own.
  ///       Map1.Extent = theLayer.FullExtent.Expand(1.5)
  ///       
  ///       ' Go into a recursive function that gets details about the sub-layer.
  ///       sb.Append(GoDeep(theLayer, 1)) ' 1 is the intial level
  ///       
  ///     End If
  ///   Next
  ///   
  ///   ' Put the StringBuilder information into the TextBox. 
  ///   TextBox1.Text = sb.ToString
  ///   
  /// End Sub
  ///   
  /// ' A helper object to track which sub-layer object we are working on.
  /// Public Class SuperObject
  ///   Public theObjects As Object
  ///   Public theLevel As Integer
  /// End Class
  ///   
  /// Public Function GoDeep(ByVal theObject As Object, ByVal theLevel As Integer) As String
  ///   
  ///   ' This is a recursive function that gets details about a specific sub-layer.
  ///   
  ///   ' If we go more than one time in this recursive function, the SuperObject helps
  ///   ' delinate which sub-layer we are operating on.
  ///   If TypeOf theObject Is SuperObject Then
  ///     Dim theSuperObject As SuperObject = CType(theObject, SuperObject)
  ///     theObject = theSuperObject.theObjects
  ///     theLevel = theSuperObject.theLevel
  ///   End If
  ///   
  ///   ' Create a new StringBuilder object to hold the detailed information about the sub-layer.
  ///   Dim sb As New Text.StringBuilder
  ///   
  ///   ' The sub-layer that is passed into this recursive function could be any number of
  ///   ' Layer types. Branch into the correct If statement depending on what type of
  ///   ' object we are dealing with.
  ///   
  ///   If TypeOf theObject Is ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer Then
  ///     
  ///     ' We have a KmlLayer type of object. Will need to recursively dig into more sub-layers 
  ///     ' in order to display details that we are interested in.
  ///     
  ///     ' Get the KmlLayer.
  ///     Dim theKmlLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer = CType(theObject, ESRI.ArcGIS.Client.Toolkit.DataSources.KmlLayer)
  ///     
  ///     ' Get the KmlLayer's child sub-layers.
  ///     Dim theLayerCollection As ESRI.ArcGIS.Client.LayerCollection = theKmlLayer.ChildLayers
  ///     
  ///     ' Add some KmlLayer information into the StringBuilder.
  ///     sb.Append("KmlLayer Name (Level " + theLevel.ToString + "): " + theKmlLayer.ID + vbCrLf)
  ///     sb.Append("##############################################" + vbCrLf)
  ///     sb.Append(vbCrLf)
  ///     
  ///     ' Loop through all of the sub-layers in the KmlLayer.
  ///     For Each theObject2 As Object In theLayerCollection
  ///       
  ///       ' Create a SuperObject to perform the recursive analysis.
  ///       Dim theRecursive_SuperObject As New SuperObject
  ///       theRecursive_SuperObject.theObjects = theObject2
  ///       theRecursive_SuperObject.theLevel = theLevel + 1
  ///       
  ///       ' Add detailed sub-layer information to the StringBuilder as a result of a recursive operation.
  ///       sb.Append(GoDeep(theRecursive_SuperObject, theLevel))
  ///       
  ///     Next
  ///     
  ///   ElseIf TypeOf theObject Is ESRI.ArcGIS.Client.GraphicsLayer Then
  ///     
  ///     ' We have a GraphicsLayer type of object. 
  ///       
  ///     ' Display detailed information about each Graphic in the GraphicsLayer
  ///     Dim theGraphicsLayer As ESRI.ArcGIS.Client.GraphicsLayer = CType(theObject, ESRI.ArcGIS.Client.GraphicsLayer)
  ///     Dim theGraphicsLayerID As String = theGraphicsLayer.ID
  ///     
  ///     ' Display overall information on the number of Graphics in the GraphicsLayer.
  ///     sb.Append("GraphicsLayer (Level " + theLevel.ToString + "): " + theGraphicsLayerID + vbCrLf)
  ///     sb.Append("================================================" + vbCrLf)
  ///     sb.Append("Number of Graphics: " + theGraphicsLayer.Graphics.Count.ToString + vbCrLf)
  ///     sb.Append(vbCrLf)
  ///     
  ///     ' A counter for the number of Graphics in the GraphicsLayer.
  ///     Dim graphicsCount As Integer = 0
  ///     
  ///     ' Loop through each Graphic in the GraphicsLayer.
  ///     For Each theGraphic As ESRI.ArcGIS.Client.Graphic In theGraphicsLayer
  ///       
  ///       ' Append which Graphic we are operating on in the StringBuilder.
  ///       sb.Append("Graphic #" + graphicsCount.ToString + vbCrLf)
  ///       
  ///       ' Incriment the Graphics counter.
  ///       graphicsCount = graphicsCount + 1
  ///       
  ///       ' ---------------------------------------------------------------------------------------
  ///       
  ///       ' Append the Geometry Type of the Graphic to the StringBuilder.
  ///       sb.Append("Geometry Type: " + theGraphic.Geometry.GetType.ToString + vbCrLf)
  ///       
  ///       ' Interrogate the specific Geometry Type of the Graphic to display it's coordinate information.
  ///       If TypeOf theGraphic.Geometry Is ESRI.ArcGIS.Client.Geometry.MapPoint Then
  ///         
  ///         ' We have a MapPoint. Display its coordinate information in the StringBuilder.
  ///         Dim theMapPoint As ESRI.ArcGIS.Client.Geometry.MapPoint = CType(theGraphic.Geometry, ESRI.ArcGIS.Client.Geometry.MapPoint)
  ///         sb.Append("Coordinates: " + theMapPoint.ToString + vbCrLf)
  ///         
  ///       ElseIf TypeOf theGraphic.Geometry Is ESRI.ArcGIS.Client.Geometry.Polyline Then
  ///         
  ///         ' We have a Polyline. Display its coordinate information in the StringBuilder.
  ///         Dim thePolyline As ESRI.ArcGIS.Client.Geometry.Polyline = CType(theGraphic.Geometry, ESRI.ArcGIS.Client.Geometry.Polyline)
  ///         Dim polylineString As String = ""
  ///         Dim theObservableCollection_PointCollection As System.Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Geometry.PointCollection) = thePolyline.Paths
  ///         For Each thePointCollection As ESRI.ArcGIS.Client.Geometry.PointCollection In theObservableCollection_PointCollection
  ///           For Each theMapPoint As ESRI.ArcGIS.Client.Geometry.MapPoint In thePointCollection
  ///             polylineString = polylineString + theMapPoint.ToString + ", "
  ///           Next
  ///         Next
  ///         sb.Append("Coordinates: " + polylineString + vbCrLf)
  ///         
  ///       ElseIf TypeOf theGraphic.Geometry Is ESRI.ArcGIS.Client.Geometry.Polygon Then
  ///         
  ///         ' We have a Polygon. Display its coordinate information in the StringBuilder.
  ///         Dim thePolygon As ESRI.ArcGIS.Client.Geometry.Polygon = CType(theGraphic.Geometry, ESRI.ArcGIS.Client.Geometry.Polygon)
  ///         Dim polygonString As String = ""
  ///         Dim theObservableCollection_PointCollection As System.Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Geometry.PointCollection) = thePolygon.Rings
  ///         For Each thePointCollection As ESRI.ArcGIS.Client.Geometry.PointCollection In theObservableCollection_PointCollection
  ///           For Each theMapPoint As ESRI.ArcGIS.Client.Geometry.MapPoint In thePointCollection
  ///             polygonString = polygonString + theMapPoint.ToString + ", "
  ///           Next
  ///         Next
  ///         sb.Append("Coordinates: " + polygonString + vbCrLf)
  ///         
  ///       End If
  ///       
  ///       ' -------------------------------------------------------------------------------------
  ///       
  ///       ' Append the symbology of the Graphic into the StringBuilder.
  ///       Dim theSymbol As ESRI.ArcGIS.Client.Symbols.Symbol = theGraphic.Symbol
  ///       sb.Append("Symbol Type: " + theSymbol.GetType.ToString + vbCrLf)
  ///       
  ///       ' -------------------------------------------------------------------------------------
  ///       
  ///       ' Interrogate the Attributes of the Graphic.
  ///       
  ///       ' Get the Dictionary of Attributes.
  ///       Dim theAttributeDictionary As System.Collections.Generic.IDictionary(Of String, Object) = theGraphic.Attributes
  ///       
  ///       ' Get the Keys for the Dictionary.
  ///       Dim theAttributeKeys As System.Collections.Generic.ICollection(Of String) = theAttributeDictionary.Keys
  ///       
  ///       ' Loop through each Key in the Dictionary of Attributes. 
  ///       For Each theKey As String In theAttributeKeys
  ///         
  ///         ' Get the Value of one Attribute. It could be any number of Types! 
  ///         Dim theValue As Object = theAttributeDictionary.Item(theKey)
  ///          ' Get the Type of the Value of the Attribute (we could have a more complex object than a String).
  ///         Dim theType As System.Type = theValue.GetType
  ///           
  ///         ' Interrogate the Value Type.
  ///         If TypeOf theValue Is System.Collections.Generic.List(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData) Then
  ///           
  ///           ' We have a List(Of KmlExtendedData) objects (most likely a result of the &lt;ExtendedData&gt; tag in KML).
  ///           
  ///           ' Get the List(Of KmlExtendedData) object.
  ///           Dim theList As System.Collections.Generic.List(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData)
  ///           theList = CType(theValue, System.Collections.Generic.List(Of ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData))
  ///           
  ///           ' Loop through each KmlExtendedData object in the List(Of KmlExtendedData).
  ///           For Each oneKmlExtendedData As ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.KmlExtendedData In theList
  ///           
  ///             ' Append the Attribute information of the KmlExtendedData into the StringBuilder.
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.DisplayName" + ", AttributeValue: " + oneKmlExtendedData.DisplayName + vbCrLf)
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.Name" + ", AttributeValue: " + oneKmlExtendedData.Name + vbCrLf)
  ///             sb.Append("AttributeKey: " + "KmlExtendedData.Value" + ", AttributeValue: " + oneKmlExtendedData.Value + vbCrLf)
  ///             sb.Append("----------------------------------------------------" + vbCrLf)
  ///             
  ///           Next
  ///           
  ///         ElseIf TypeOf theValue Is String Then
  ///           
  ///           ' We have a String object.
  ///           
  ///           ' This could come from a number of KML tags (ex: &lt;atom:author&gt; 'name' attribute; &lt;atom:link&gt; 'href' attribute;
  ///           ' &lt;atom:name&gt; 'href' attribute, &lt;BalloonStyle&gt;&lt;text&gt; information, &lt;description&gt; information; and &lt;name&gt; information).
  ///           sb.Append("AttributeKey: " + theKey + ", AttributeValue: " + theValue.ToString + vbCrLf)
  ///           sb.Append("----------------------------------------------------" + vbCrLf)
  ///           
  ///         Else
  ///           
  ///           ' We have some other Type of object. TODO: User to interrogate further!
  ///           
  ///         End If
  ///       Next
  ///         
  ///       sb.Append(vbCrLf)
  ///       
  ///     Next
  ///     
  ///   ElseIf TypeOf theObject Is ESRI.ArcGIS.Client.ElementLayer Then
  ///     
  ///     ' We have an ElementLayer type of object. 
  ///     
  ///     ' Display detailed information about each ElementLayer
  ///     Dim theElementLayer As ESRI.ArcGIS.Client.ElementLayer = CType(theObject, ESRI.ArcGIS.Client.ElementLayer)
  ///     
  ///     ' Append the overall information about the ElementLayer in the StringBuilder.
  ///     Dim theElementLayerID As String = theElementLayer.ID
  ///     sb.Append("ElementLayer (Level " + theLevel.ToString + "): " + theElementLayerID + vbCrLf)
  ///     sb.Append("=============================================" + vbCrLf)
  ///     sb.Append("Number of Elements: " + theElementLayer.Children.Count.ToString + vbCrLf)
  ///     
  ///     ' Append the Extent information about the ElementLayer int the Stirng Builder.
  ///     Dim elementLayerFullExtent As ESRI.ArcGIS.Client.Geometry.Envelope = theElementLayer.FullExtent
  ///     sb.Append("FullExtent: " + elementLayerFullExtent.ToString + vbCrLf)
  ///     sb.Append(vbCrLf)
  ///     
  ///     ' Loop through each UIElement in the ElementLayer.
  ///     For Each oneElement As System.Windows.UIElement In theElementLayer.Children
  ///       
  ///       ' Append information about each UIElement in the StringBulder.
  ///       sb.Append("ElementType: " + oneElement.GetType.ToString + vbCrLf)
  ///       sb.Append("Opacity: " + oneElement.Opacity.ToString + vbCrLf)
  ///       sb.Append("Visibility: " + oneElement.Visibility.ToString + vbCrLf)
  ///       sb.Append("Envelope: " + ESRI.ArcGIS.Client.ElementLayer.GetEnvelope(oneElement).ToString + vbCrLf)
  ///       sb.Append(vbCrLf)
  ///       
  ///     Next
  /// 
  ///   End If
  /// 
  ///   ' Return the StringBuilder information back to the caller.
  ///   Return sb.ToString
  /// 
  /// End Function
  /// </code>
  /// </example>
		public string Name
		{
			get { return _name; }
			private set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		} 
		#endregion

		#region ProjectionService
		/// <summary>
		/// Gets or sets the projection service used for projecting geometry in the data source
		/// to the map's spatial reference. 
		/// </summary>
		/// <remarks>
		/// If you are projecting between WGS84 Geographic coordinates (SRID=4326) and WebMercator
		/// or your source data is in the same projection as the map it's being displayed on, 
		/// there is no need to set this property.
		/// </remarks>
		/// <value>The projection service.</value>
		public IProjectionService ProjectionService
		{
			get { return (IProjectionService)GetValue(ProjectionServiceProperty); }
			set { SetValue(ProjectionServiceProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ProjectionService"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ProjectionServiceProperty =
			DependencyProperty.Register("ProjectionService", typeof(IProjectionService), typeof(KmlLayer), new PropertyMetadata(null, OnProjectionServicePropertyChanged));

		private static void OnProjectionServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			KmlLayer obj = (KmlLayer)d;
			obj.SetProjectionService(obj.ChildLayers);
		}

		private void SetProjectionService(IEnumerable<Layer> layerCollection)
		{
			foreach (var layer in layerCollection)
			{
				if (layer is GraphicsLayer)
					(layer as GraphicsLayer).ProjectionService = ProjectionService;
				else
				{
					if (layer is KmlLayer)
						(layer as KmlLayer).ProjectionService = ProjectionService;
					else if (layer is GroupLayer)
						SetProjectionService((layer as GroupLayer).ChildLayers);
				}
			}
		}
		#endregion

		#region VisibleLayers

		private IEnumerable<string> _visibleLayers;

		/// <summary>
		/// Gets or sets the names of the KML sublayers that have to be initialized as visible.
		/// If this property is not set, the visibility defined in the KML document is used.
		/// </summary>
		/// <remarks>The wildcard "*" allows to set all descendant layers as visible.
		/// <para>
		/// For examples:
		/// <list type="bullet">
		/// <item>
		/// VisibleLayers="*" sets all sublayers as visible</item>
		/// <item>
		/// VisibleLayers="folder/*" set all sublayers under 'folder' as visible</item>
		/// <item>
		/// VisibleLayers="folder,folder/*" sets 'folder' and all its sublayers as visible</item>
		/// </list>
		/// </para>
		/// <para>Note that this list is a configuration property and doesn't return the sublayers that are currently visible.
		/// To know whether a sublayer is currently visible, you have to go through the layers hierarchy and get the current visibility of the layer.
		/// </para>
		/// </remarks>
		/// <value>The visible layers enumeration.</value>
		[TypeConverter(typeof(StringToStringArrayConverter))]
		public IEnumerable<string> VisibleLayers
		{
			get { return _visibleLayers; }
			set
			{
				_visibleLayers = value;
				if (IsInitialized)
				{
					_isLoading = false;
					Refresh();
				}
				OnPropertyChanged("VisibleLayers");
			}
		}
		#endregion

		#region RefreshInterval
		private TimeSpan _refreshInterval;
		/// <summary>
		/// Gets or sets the refresh interval.
		/// </summary>
		/// <value>The refresh interval.</value>
		public TimeSpan RefreshInterval
		{
			get { return _refreshInterval; }
			set
			{
				if (_refreshInterval != value)
				{
					_refreshInterval = value;
					InitRefreshTimer();
					OnPropertyChanged("RefreshInterval");
				}
			}
		}

		#endregion


		#region Private Members
		// Context contains :
		//  - the KML XElement to process
		//  - the images extracted from KMZ file and used for point features
		//  - the styles parsed only by the root layer
		private KmlLayerContext _context;

		// Loading status
		private bool _isLoaded;
		private bool _isLoading;
		WebClient _webClient;

		// Parent KML Layer (or null if root)
		private readonly KmlLayer _parentLayer;

		// Full path useful to define the layer visibility from VisibleLayers property
		private string _fullPath; // for root kmllayer the fullpath is null, for first level the fullpath equals "<FolderName>", for second level the fullpath equals "<FolderName>/<SubFolderName>"....

		private bool _isRoot = false; // root layer from which the Url or the stream has been set (note that NetworkLinks is root as well)
		private bool _hasRootContainer = false; // the root layer contains only a document or a folder at the root level. This container is not shown as group layer but needs to be taken in care for generating folderIDS
		private int _folderId; // WebMap folder ID
		internal bool IsHidden { get; set; } // Flag indicating whether this layer should be hidden in the legend
		private bool _hideChildren; // Flag indicating whether the children of this KML layer should be hidden in the legend
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="KmlLayer"/> class.
		/// </summary>
		public KmlLayer()
		{
			ChildLayers = new LayerCollection();
			_isLoading = false;
			_isLoaded = false;
			NeedRefreshOnRegion = false;
			_context = new KmlLayerContext { Images = new Dictionary<string,ImageBrush>()};
			PropertyChanged += KmlLayer_PropertyChanged;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KmlLayer"/> class which will be a child of another <see cref="KmlLayer"/>.
		/// </summary>
		/// <param name="parentLayer">The parent layer.</param>
		internal KmlLayer(KmlLayer parentLayer) : this()
		{
			_parentLayer = parentLayer;
			ProjectionService = parentLayer.ProjectionService;
			ProxyUrl = parentLayer.ProxyUrl;
			VisibleLayers = parentLayer.VisibleLayers;
			InitializationFailed += KmlLayer_InitializationFailed; // To avoid crash and report error to parent
#if !WINDOWS_PHONE
			MapTip = parentLayer.MapTip;
#endif
#if !SILVERLIGHT || WINDOWS_PHONE
			Credentials = parentLayer.Credentials;
#endif
		}

		void KmlLayer_InitializationFailed(object sender, EventArgs e)
		{
			if (_parentLayer != null && sender is KmlLayer)
				_parentLayer.InitializationFailure = ((KmlLayer) sender).InitializationFailure;
		}

		#endregion

		#region override void OnMapChanged
		/// <summary>
		/// Override to know when a layer's <see cref="Map"/> property changes.
		/// </summary>
		/// <param name="oldValue">Old map</param>
		/// <param name="newValue">New map</param>
		protected override void OnMapChanged(Map oldValue, Map newValue)
		{
			InitRefreshTimer(); // stopping the timer when newmap is null allows to avoid memory leak when the layer is removed from the map

			if (oldValue != null)
			{
				if (NeedRefreshOnRegion)
					oldValue.ExtentChanged -= RefreshOnRegionAsync;
				oldValue.PropertyChanged -= Map_PropertyChanged;
			}
			if (newValue != null)
			{
				if (NeedRefreshOnRegion)
					newValue.ExtentChanged += RefreshOnRegionAsync;
				newValue.PropertyChanged += Map_PropertyChanged;
				SetResolutionRange();
			}
			base.OnMapChanged(oldValue, newValue);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Initializes the resource.
		/// </summary>
		/// <seealso cref="ESRI.ArcGIS.Client.Layer.Initialized"/>
		/// <seealso cref="ESRI.ArcGIS.Client.Layer.InitializationFailure"/>
		public override void Initialize()
		{
			if (IsInitialized)
				return;

			if (_visibleLayerIds != null)
			{
				// VisibleLayerIds is set --> layer created from a web map --> check that the layer must stay visible
				if (!_visibleLayerIds.Contains(0)) // FolderIds 0 is the layer itself (Note : we can't test here the top folder visibility (folderid=1) since the kml document has not been parsed yet, so _hasRootContainer is not initialized yet)
					Visible = false;
			}

			Refresh();
		}


		/// <summary>
		/// Sets the KML stream source.
		/// </summary>
		/// <param name="stream">A stream to a KML or KMZ file.</param>
		/// <remarks>
		/// Use this method if you need to load KML from a local file or
		/// file stream.
		/// </remarks>
		public void SetSource(Stream stream)
		{
			_url = null; 
			_context = new KmlLayerContext { Images = new Dictionary<string, ImageBrush>() }; // reset the context
			SetSourceInternal(stream);
			if (IsInitialized) // else wait for Initialize to parse the stream
			{
				IsInitialized = false; // will raise again Initialized event
				if (ChildLayers.Any())
					ChildLayers.Clear();
				Name = null;
				Refresh();
			}
		}

    	private void SetSourceInternal(Stream stream)
		{
			XDocument xLocalDoc;
			using (Stream seekableStream = ConvertToSeekable(stream))
			{
				if (IsStreamCompressed(seekableStream))
				{
					// Feed result into ZIP library
					// Extract KML file embedded within KMZ file and extract point image file names
					ZipFile zipFile = ZipFile.Read(seekableStream);
					xLocalDoc = GetKmzContents(zipFile);
				}
				else
				{
					try
					{
						xLocalDoc = XDocument.Load(XmlReader.Create(new StreamReader(seekableStream)), LoadOptions.None);
					}
					catch
					{
						xLocalDoc = null;
					}
				}

				if (xLocalDoc != null)
				{
					_isRoot = true;

					// Assign folder IDS for web maps
					AssignFolderIDs(xLocalDoc.Root);

					// keep root element for future parse (in background)
					_context.Element = xLocalDoc.Root;
				}
			}
		}

		/// <summary>
		/// Refreshes the KML layer by downloading and parsing the KML document.
		/// </summary>
		public void Refresh()
		{
			if (_isLoading)
				return; // refresh already on the way

			_isLoaded = false;
			if (!Visible) // delay the refresh until the layer is visible
			{
				if (!IsInitialized)
					base.Initialize(); // call Initialize though else could wait indefinitively for it
				return;
			}

			InitializationFailure = null;
			if (Url != null)
			{
				// If the ViewRefreshMode is set to OnRegion, the refresh is depending on the region to draw and may be done later
				if (ViewRefreshMode == ViewRefreshMode.OnRegion && RegionInfo != null && RegionInfo.Envelope != null)
				{
					RegionInfo.GetRegionAsync(Map.SpatialReference, ProjectionService, ConditionalRefresh);
					return;
				}

				// Download again the url
				ConditionalRefresh(null);
			}
			else if (_context != null && _context.Element != null)
			{
				// SetSource may have been called
				_isLoading = true;
				ParseKmlDocument();
			}
			else
			{
				InitializationFailure = new ArgumentException(Properties.Resources.Generic_UrlNotSet, "Url");
				if (ChildLayers.Any())
					ChildLayers.Clear();
				if (!IsInitialized)
					base.Initialize();
			}
		}

		/// <summary>
		/// If the region matches the map extent, download the URL immediatly
		/// else set the flag NeedRefreshOnRegion for further refresh
		/// </summary>
		/// <param name="region">The region.</param>
		private void ConditionalRefresh(Envelope region)
		{
			if (IsInRegion(Map, region))
			{
				// Download the url
				_context = new KmlLayerContext { Images = new Dictionary<string, ImageBrush>() }; // reset the context
				_isLoading = true;
				NeedRefreshOnRegion = false;
				if (Url != null)
					DownloadContent(Url);
			}
			else
			{
				// Delay refresh until the map fits the KML region
				NeedRefreshOnRegion = true;
				if (ChildLayers.Any())
					ChildLayers.Clear();
				if (!IsInitialized)
					base.Initialize(); // call Initialize though else could wait indefinitively for it
			}
		}

		#region Visibility by IDs
		private int[] _visibleLayerIds;

		///<summary>
		/// Set the folders visibility from an enumeration of Ids.
		/// These Ids are those used by the webmaps.
		///</summary>
		/// <param name="IDs">Enumeration of visible folder ID.</param>
		/// <remarks>This method is mainly useful for the webmap serializer</remarks>
		public void SetVisibilityByIDs(IEnumerable<int> IDs)
		{
			_visibleLayerIds = IDs.ToArray();

			if (IsInitialized)
			{
				// FolderIds 0 and 1 are the layer itself
				if (!_visibleLayerIds.Contains(0) || (_hasRootContainer && !_visibleLayerIds.Contains(1)))
					Visible = false;

				_isLoading = false;
				Refresh();
			}
		}

		///<summary>
		/// Generate the IDs of the folder that are currently visible.
		///</summary>
		///<returns>The enumeration of visible folder IDs</returns>
		/// <remarks>This method is mainly useful for the webmap serializer</remarks>
		public IEnumerable<int> GenerateVisibilityIDs()
		{
			// First create an enumeration for the current layer
			IList<int> ownIDs = new List<int>();

			if (Visible)
			{
				if (_isRoot)
				{
					ownIDs.Add(0); // ID for the root layer itself
					if (_hasRootContainer)
						ownIDs.Add(1); // for root container which is not shown in SL
				}
				else
					ownIDs.Add(_folderId); // ID of the layer
			}

			// Then go recursively through the sublayers and  concat with  the visible IDs of the sublayers (but don't go through the hierarchy for networklinks (i.e. _isRoot))
			return ownIDs.Concat(ChildLayers.OfType<KmlLayer>().SelectMany(InternalGenerateVisibilityIDs)).ToArray(); // ToArray freezes the result
		}

		private static IEnumerable<int> InternalGenerateVisibilityIDs(KmlLayer layer)
		{
			// For NetworkLinks (i.e. _isRoot), don't go through the hierarchy and return an empty enumeration (Network links visibility is not managed by arcgis.com)
			if (layer._isRoot)
				return Enumerable.Empty<int>();

			// Create an enumeration either empty (if the layer is not visible) or containing the current ID (if the layer is visible)
			IEnumerable<int> ownIDs = layer.Visible ? new[] { layer._folderId } : Enumerable.Empty<int>();

			// Go recursively through the sublayers and  concat with  the visible IDs of the sublayers
			return ownIDs.Concat(layer.ChildLayers.OfType<KmlLayer>().SelectMany(InternalGenerateVisibilityIDs));
		}

		#endregion

    	#endregion

        #region Private Methods

        /// <summary>
        /// Download content using URL. This method may be called recursively if the KML file contains
        /// a network link.
        /// </summary>
        /// <param name="url">Location of KML content.</param>
        private void DownloadContent(Uri url)
        {
#if !SILVERLIGHT
			if (url.IsFile)
			{
				if (!System.IO.File.Exists(url.OriginalString))
				{
					InitializationFailure = new FileNotFoundException(url.OriginalString);
					if (ChildLayers.Any())
						ChildLayers.Clear();

					if (!IsInitialized)
					{
						base.Initialize();
					}
					_isLoaded = true;
					_isLoading = false;
				}
				else
				{
					using (Stream s = System.IO.File.Open(url.OriginalString, FileMode.Open))
					{
						SetSourceInternal(s);
					}
					if (_context == null || _context.Element == null)
					{
						this.InitializationFailure = new ArgumentException(Properties.Resources.KmlLayer_XDocumentReadFailed);
						if (ChildLayers.Any())
							ChildLayers.Clear();
						if (!IsInitialized)
						{
							base.Initialize();
						}
						_isLoaded = true;
						_isLoading = false;
						return;
					}
					ParseKmlDocument();
				}
			}
			else
#endif
			{
				// Use web client to download KML document and get notified when read is complete
				if (_webClient != null)
				{
					_webClient.OpenReadCompleted -= webclient_OpenReadCompleted;
					if (_webClient.IsBusy)
						_webClient.CancelAsync();
				}
				_webClient = Utilities.CreateWebClient();
#if !SILVERLIGHT || WINDOWS_PHONE
				if (Credentials != null)
                    _webClient.Credentials = Credentials;
#endif
#if !SILVERLIGHT
				if (ClientCertificate != null)
					(_webClient as CompressResponseWebClient).ClientCertificate = ClientCertificate;
#endif
        		string urlString = url.OriginalString;

				if (DisableClientCaching)
				{
					// Add a timestamp to avoid browser caching
					if (!urlString.Contains("?"))
					{
						if (!urlString.EndsWith("?"))
							urlString += "?";
					}
					else
					{
						if (!urlString.EndsWith("&"))
							urlString += "&";
					}
					urlString += string.Format("_ts={0}", DateTime.Now.Ticks);
				}
				_webClient.OpenReadCompleted += webclient_OpenReadCompleted;
				_webClient.OpenReadAsync(Utilities.PrefixProxy(ProxyUrl, urlString));
			}
        }

        /// <summary>
        /// Event handler for when the KML file is completely downloaded. If the KML content contains a
        /// network link, then the linked content will be downloaded recursively. If no network link is
        /// detected, then the KML content is converted into a feature definition using the engine and
        /// eventually rendered as graphic g.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Stream containing KML content.</param>
		private void webclient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
		{
			if (sender is WebClient)
				((WebClient) sender).OpenReadCompleted -= webclient_OpenReadCompleted;

			// Proceed only if not cancelled
			if (!e.Cancelled)
			{
				// If an error occurred, then report using the argument error information
				if (e.Error != null)
				{
					// Indicate that this layer is now initialized
					InitializationFailure = e.Error;
					if (ChildLayers.Any())
						ChildLayers.Clear();
					if (!IsInitialized)
					{
						base.Initialize();
					}
					_isLoaded = true;
					_isLoading = false;
					return;
				}
				SetSourceInternal(e.Result);
				if (_context == null || _context.Element == null)
				{
					InitializationFailure = new ArgumentException(Properties.Resources.KmlLayer_XDocumentReadFailed);
					if (ChildLayers.Any())
						ChildLayers.Clear();
					if (!IsInitialized)
					{
						base.Initialize();
					}
					_isLoaded = true;
					_isLoading = false;
					return;
				}
				ParseKmlDocument();
			}
		}

        private void ParseKmlDocument()
        {
			// Create a background worker thread to process the KML content into a list of feature
			// definitions that can then be easily added to the graphics layer. This process is done
			// in the background for performance reasons (to keep the application interactive) but
			// actual creation of features require the UI thread which is done after this thread
			// completes.
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
#if !SILVERLIGHT || WINDOWS_PHONE
			_context.Credentials = Credentials;
#endif

			// Invoke the background thread and pass the info about the XElement to parse via event arguments
			backgroundWorker.RunWorkerAsync(_context);
		}

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Instantiate KML To Graphics converter engine, and
            // feed XDocument. Store resulting feature definition in the result object of the event
            // arguments so it is transmitted to the Work Completed handler for subsequent processing.
            KmlToFeatureDefinition ktg = new KmlToFeatureDefinition(GetBaseUri(), ProxyUrl);

			e.Result = ktg.Convert((KmlLayerContext)e.Argument);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			if (ChildLayers.Any())
				ChildLayers.Clear(); // for refresh case
 
            if (e.Error != null)
            {
                this.InitializationFailure = new ArgumentException(Properties.Resources.KmlLayer_DocumentParsingFailed);
            }
            else
            {
                // Create graphic features from definitions -- this code requires the UI thread
                FeatureDefinition fd = (FeatureDefinition)e.Result;

				// Initialize the layer name with the name info coming from the KML document
				Name = fd.name;

				_hasRootContainer = _isRoot && fd.hasRootContainer; // the root container has been collapsed (info needed to generate internal folderIDs)

				if (_visibleLayerIds != null && !IsInitialized)
				{
					// VisibleLayerIds is set --> layer created from a web map --> check that the layer must stay visible (_hasRootContainer is set only after the file has been parsed)
					if (_hasRootContainer && !_visibleLayerIds.Contains(1)) // FolderIds 1 is the top level folder that may not be visible in SL
					{
						Visible = false;
						_isLoading = false;
						base.Initialize();
						return;
					}
				}

				// Store the parsed styles to be able to pass them to children
				_context.Styles = fd.styles;

				// Create ground overlays and add to element layer
				if (fd.groundOverlays.Any())
				{
					ElementLayer elementLayer = new ElementLayer { ID = Properties.Resources.KmlLayer_GroundOverlaysSublayer };
					fd.CreateGroundOverlays(elementLayer, _context.Images, Map);
					ChildLayers.Add(elementLayer);
					if (IsInitialized)
						elementLayer.Initialize(); // should be done by the group layer (to remove when Bug#2718 is fixed)
				}
				
				// Create graphics and add to graphics layer
				if (fd.placemarks.Any())
				{
					KmlGraphicsLayer kmlGraphicsLayer = new KmlGraphicsLayer
					{
						ID = Properties.Resources.KmlLayer_PlacemarksSublayer,
						ProjectionService = ProjectionService,
						IsHidden = _hideChildren
					};
					fd.CreateGraphics(kmlGraphicsLayer, _context.Images);
#if !WINDOWS_PHONE
					kmlGraphicsLayer.MapTip = MapTip;
#endif
					ChildLayers.Add(kmlGraphicsLayer);
					if (IsInitialized)
						kmlGraphicsLayer.Initialize(); // should be done by the group layer  (to remove when Bug#2718 is fixed)

					// Setting the Spatial Reference of the KML layer to 4326:
					if (this.SpatialReference == null)
					{
					    this.SpatialReference = new Geometry.SpatialReference(4326);
					}
				}

				// Create a sub KML layer for each container
				foreach (ContainerInfo container in fd.containers)
				{
					string fullPath = _fullPath == null ? (container.Name ?? string.Empty) : string.Concat(_fullPath, "/", container.Name);
					// Note : use internal constructor, so properties such as MapTip, ProxyUrl, VisibleLayers.. are reported to the children
					var kmlLayer = new KmlLayer(this)
					               	{
					               		ID = container.Name,
					               		Name = container.Name,
					               		_fullPath = fullPath,
					               		RefreshInterval = TimeSpan.FromSeconds(container.RefreshInterval),
					               		VisibleTimeExtent = container.TimeExtent,
					               		RegionInfo = container.RegionInfo,
					               		_folderId = container.FolderId,
					               		_hideChildren = container.HideChildren,
					               		IsHidden = _hideChildren
					               	};

					bool isOk = true;
					if (string.IsNullOrEmpty(container.Url))
					{
						// Set the visibility of the layer
						// There are 3 ways to define the visibility of a folder or document, by priority order:
						//    - by the internal VisibleLayerIds property (for the layers inside a web map)
						//    - by the public VisibleLayers property
						//    - by the visibility defined in the KML document
						kmlLayer.Visible = _visibleLayerIds != null ? _visibleLayerIds.Contains(kmlLayer._folderId) : IsContainerVisible(fullPath, container.Visible);
						kmlLayer._visibleLayerIds = _visibleLayerIds;

						// Subfolder : Create a context object and initialize a KmlLayer with it
						kmlLayer._context = new KmlLayerContext
						                      	{
						                      		Element = container.Element, // The XElement that the KML layer has to process
						                      		Styles = _context.Styles,
						                      		Images = _context.Images,
						                      		AtomAuthor = container.AtomAuthor,
						                      		AtomHref = container.AtomHref
						                      	};
					}
					else
					{
						// NetworkLink : initialize the Url
						Uri containerUri = GetUri(container.Url, GetBaseUri());
						if (containerUri != null)
						{
							kmlLayer.Url = containerUri;
							kmlLayer.ViewRefreshMode = container.ViewRefreshMode;
						}
						else
							isOk = false; // bad url, don't create the child layer

						// Set the visibility of the layer
						// For a network link, the internal VisibleLayerIds property is not used.
						kmlLayer.Visible = IsContainerVisible(fullPath, container.Visible);
					}

					if (isOk)
					{
						ChildLayers.Add(kmlLayer);
						if (IsInitialized)
							kmlLayer.Initialize(); // should be done by the group layer --> to remove later (after or with CR2718)
					}
				}

				// Check that the layer refresh interval is compatible with infos coming from NetworkLinkControl
				if (fd.networkLinkControl != null)
				{
					if (RefreshInterval != TimeSpan.Zero && fd.networkLinkControl.MinRefreshPeriod > 0.0)
						RefreshInterval = TimeSpan.FromSeconds(Math.Max(RefreshInterval.Seconds, fd.networkLinkControl.MinRefreshPeriod));
				}
				
				// Set resolution range from the Region/Lods info of the parent
				SetResolutionRange();

			}

			if (!IsInitialized)
				base.Initialize();
			_isLoading = false;
			_isLoaded = true;
		}

		// Return an Uri from an url by taking care that the url may be relative
		internal static Uri GetUri(string url, Uri baseUri)
		{
			if (string.IsNullOrEmpty(url))
				return null;

			Uri uri;

			// Try first with an absolute URI
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri) && baseUri != null)
			{
				// Try with Relative Uri 
				Uri.TryCreate(baseUri, url, out uri);
			}
			return uri;
		}

		// Base Uri to use for relative paths
		private Uri GetBaseUri()
		{
			return GetParentsAndSelf(this).Select(l => l.Url).FirstOrDefault(u => u != null); // Go up the hierarchy and return the first Url
		}

		private static IEnumerable<KmlLayer> GetParentsAndSelf(KmlLayer kmlLayer)
		{
			while (kmlLayer != null)
			{
				yield return kmlLayer;
				kmlLayer = kmlLayer._parentLayer;
			}
		}

		private bool IsContainerVisible(string path, bool defaultVisibility)
		{
			// VisibleLayers not set --> use the default visibility defined in the KML document
			if (VisibleLayers == null || !VisibleLayers.Any())
				return defaultVisibility;

			// Check whether VisibleLayers contains the full path
			if (path == null || VisibleLayers.Contains(path))
				return true;

			// Global wildcard to set all layers as visible
			if (VisibleLayers.Contains("*"))
				return true;

			// look for a wildcard defined at a sublevel  (e.g. VisibleLayers="myFolder/*" makes visible all paths beginning by myFolder/)
			IEnumerable<string> subpaths = path.Select((c, ind) => c == '/' ? path.Substring(0, ind) : null).Where(subpath => subpath != null);
			return subpaths.Any(subpath => VisibleLayers.Contains(subpath + "/*"));
		}

        /// <summary>
        /// Processes each file in the ZIP stream, storing images in a dictionary and load the KML contents
        /// into an XDocument.
        /// </summary>
        /// <param name="zipFile">Decompressed stream from KMZ.</param>
        /// <returns>XDocument containing KML content from the KMZ source.</returns>
        private XDocument GetKmzContents(ZipFile zipFile)
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
				System.IO.Stream ms = zipFile.GetFileStream(filename);
#else
				MemoryStream ms = new MemoryStream();
				zipFile.Extract(filename, ms);
#endif
				if (ms == null) continue;
				ms.Seek(0, SeekOrigin.Begin);

                switch (filename.Substring(lastPeriod).ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
#if !SILVERLIGHT
                    case ".bmp":
                    case ".gif":
#endif
						// If the file is an image, then add it to the dictionary of images and use
                        // its filename as the key since this will match the subsequent KML style
                        // information for point features.
						try
						{
                            BitmapImage thumbnailBitmap = new BitmapImage();
#if SILVERLIGHT
                            thumbnailBitmap.SetSource(ms);
#else
                            thumbnailBitmap.BeginInit();
                            thumbnailBitmap.StreamSource = ms;
                            thumbnailBitmap.EndInit();
#endif
                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = thumbnailBitmap;
                            _context.Images.Add(filename.ToLower(), ib);
						}
						catch { }

                        break;

                    case ".kml":
                        // Create the XDocument object from the input stream
                        try
                        {
							XDocument doc = XDocument.Load(XmlReader.Create(new StreamReader(ms)));
						    xDoc = doc;
                        }
                        catch { }
                        break;
                }
            }

            return xDoc;
        }

        internal static bool IsStreamCompressed(Stream inputStream)
        {
            bool isCompressed = false;

            BinaryReader reader = new BinaryReader(inputStream);
            inputStream.Seek(0, SeekOrigin.Begin);
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int headerSignature = reader.ReadInt32();
                if (headerSignature == 67324752) //PKZIP
                    isCompressed = true;

                // Reset stream back to beginning
                inputStream.Seek(0, SeekOrigin.Begin);
            }

            return isCompressed;
        }

        internal static Stream ConvertToSeekable(Stream inputStream)
        {
            // Under Silverlight this will be true and thus the input stream is returned and utilized
            if (inputStream.CanSeek)
                return inputStream;
            
            // Under WPF, however, the stream is not seekable and thus we convert it to a memory stream
            // which can then be used to determine if the stream contains compressed (ZIP) content or not
            int b;
            MemoryStream ms = new MemoryStream();

            while (true)
            {
                b = inputStream.ReadByte();
                if (b < 0) break;
                ms.WriteByte((byte)b);
            }

            inputStream.Close();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

		private DispatcherTimer _refreshTimer; // Timer to manage the autorefresh

		/// <summary>
		/// Init the timer to refresh the layer
		/// </summary>
		private void InitRefreshTimer()
		{
			// Cancel the previous timer
			StopRefreshTimer();

			if (Map != null && RefreshInterval != TimeSpan.Zero)
			{
				// Create a new timer to refresh the layer
				_refreshTimer = new DispatcherTimer() { Interval = RefreshInterval };
				_refreshTimer.Tick += (s, e) => Refresh();
				_refreshTimer.Start();
			}
		}

		private void StopRefreshTimer()
		{
			if (_refreshTimer != null)
			{
				_refreshTimer.Stop();
				_refreshTimer = null;
			}
		}

		/// <summary>
		/// Handles the PropertyChanged event of the KmlLayer.
		/// Loads and parses the KML source as soon as the layer is visible.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		void KmlLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				if (Visible && !_isLoaded)
					Refresh();
				else if (!Visible)
					NeedRefreshOnRegion = false;
			}
		}

		internal const string FolderIdAttributeName = "__folderid__";

		// Assign folder IDS for WebMap.
		// Warning : These IDs have to be in sync with arcgis.com KML servlet.
		private static void AssignFolderIDs(XContainer xContainer)
		{
			int folderIDs = 0;
			foreach(var element in xContainer.Descendants().Where(NeedNumericId))
			{
				folderIDs++;
				// store the folderID as attribute of the in memory XML document, so the info is available when parsing the documents or folders
				if (element.Name.LocalName == "Folder" || element.Name.LocalName == "Document")
					element.SetAttributeValue(FolderIdAttributeName, folderIDs);
			}
		}

		private static bool NeedNumericId(XElement element)
		{
			string name = element.Name.LocalName;
			// This list must be in sync with the kml servlet utility behavior

			// List of Elements that need a numeric ID whatever their real ID
			bool needNumericId = name == "Folder" || name == "Placemark" || name == "Document"
				|| name == "ScreenOverlay" || name == "GroundOverlay" || name == "NetworkLink"
				|| name == "PhotoOverlay" || name == "Camera" || name == "MultiTrack" || name == "Track";

			if (!needNumericId)
			{
				// These elements need a numeric ID only if they have no ID already set
				bool isCandidate = name == "BalloonStyle" || name == "LineStyle" || name == "IconStyle" || name == "Icon" || name == "LookAt"
					|| name == "LineString" || name == "LinearRing" || name == "LabelStyle" || name == "Style" || name == "StyleMap"
					|| name == "LinkNode" || name == "ListStyle" || name == "Model" || name == "Schema" || name == "Url";

				needNumericId = isCandidate && !(element.HasAttributes && element.Attribute("id") != null);
			}
			return needNumericId;
		}

		#endregion

		#region Region management
		private ViewRefreshMode ViewRefreshMode { get; set; }

		private RegionInfo _regionInfo;
		private RegionInfo RegionInfo
		{
			get { return _regionInfo; }
			set
			{
				if (_regionInfo != value)
				{
					_regionInfo = value;
					SetResolutionRange();
				}
			}
		}

		private bool _needRefreshOnRegion;
		private bool NeedRefreshOnRegion
		{
			get { return _needRefreshOnRegion; }
			set
			{
				if (_needRefreshOnRegion != value)
				{
					_needRefreshOnRegion = value;
					if (Map != null)
					{
						if (_needRefreshOnRegion)
							Map.ExtentChanged += RefreshOnRegionAsync;
						else
							Map.ExtentChanged -= RefreshOnRegionAsync;
					}
				}
			}
		}

		void RefreshOnRegionAsync(object sender, ExtentEventArgs e)
		{
			Map map = sender as Map;
			if (map == null || !NeedRefreshOnRegion || _isLoading || _isLoaded)
				return;

			// Check visibility of this layer and all parents
			bool visible = GetParentsAndSelf(this).All(l => l.Visible);
			if (!visible)
				return;

			if (RegionInfo == null)
				RefreshOnRegion(null);
			else
				RegionInfo.GetRegionAsync(map.SpatialReference, ProjectionService, RefreshOnRegion);
		}

		void RefreshOnRegion(Envelope region)
		{
			if (NeedRefreshOnRegion && IsInRegion(Map, region))
			{
				// refresh the KML layer
				_context = new KmlLayerContext { Images = new Dictionary<string, ImageBrush>() }; // reset the context
				_isLoading = true;
				NeedRefreshOnRegion = false;
				if (Url != null)
					DownloadContent(Url);
			}
		}

		private bool IsInRegion(Map map, Envelope region)
		{
			if (region == null)
				return true; // region not defined --> we can load the layer without waiting for map extent

			if (map == null)
				return false;

			Envelope extent = map.Extent;
			if (extent == null || extent.SpatialReference == null)
				return false;

			// KML LODS are based on the diagonal size
			double regionSize = Math.Sqrt(region.Width * region.Width + region.Height * region.Height);

			// Test that current map extent intersects the KML region
			if (!region.Intersects(extent))
				return false;

			// Test the LOD level
			double lod = regionSize / map.Resolution;

			return !(lod < RegionInfo.MinLodPixels || (lod > RegionInfo.MaxLodPixels && RegionInfo.MaxLodPixels != -1)); // Keep ! to take care of NaN
		}


		/// <summary>
		/// Sets the min and max resolution from the Region.
		/// If the layer is a networklink, the resolution is set on the layer itself
		/// If the layer is not a networklink, the resolution is set on the children that are not networklink.
		/// </summary>
		private void SetResolutionRange()
		{
			if (RegionInfo == null || RegionInfo.Envelope == null || Map == null)
				return;

			if (!RegionInfo.HasLods())
				return;

			if (Map.SpatialReference == null)
			{
				SetResolutionRange(null);
			}
			else
			{
				IProjectionService projectionService = GetProjectionService();

				RegionInfo.GetRegionAsync(Map.SpatialReference, projectionService, SetResolutionRange);
			}
		}

		private void SetResolutionRange(Envelope envelope)
		{
			double minimumResolution = 0.0;
			double maximumResolution = double.PositiveInfinity;

			if (envelope != null) // else not able to project the region --> give up
			{
				// KML LODS are based on the diagonal size
				double regionSize = Math.Sqrt(envelope.Width * envelope.Width + envelope.Height * envelope.Height);

				maximumResolution = RegionInfo.MinLodPixels > 0 ? regionSize/RegionInfo.MinLodPixels : double.PositiveInfinity;
				minimumResolution = RegionInfo.MaxLodPixels != -1 && !double.IsNaN(RegionInfo.MaxLodPixels)
				                    	? regionSize/RegionInfo.MaxLodPixels
				                    	: 0.0;
			}

			if (Url == null)
			{
				foreach (var layer in ChildLayers.Where(l => !(l is KmlLayer) || (l as KmlLayer).Url == null)) // Eliminate the networklinks from the list
				{
					layer.MinimumResolution = minimumResolution;
					layer.MaximumResolution = maximumResolution;
				}
			}
			else
			{
				MinimumResolution = minimumResolution;
				MaximumResolution = maximumResolution;
			}
		}

		void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Map map = sender as Map;
			if (map == null)
				return;

			if (e.PropertyName == "SpatialReference")
				SetResolutionRange();
		}

		private IProjectionService GetProjectionService()
		{
			return ProjectionService ?? new WebMercatorProjectionService();
		}

		#endregion

		#region ILegendSupport Members

		/// <summary>
		/// Queries for the legend infos of the layer.
		/// </summary>
		/// <remarks>
		/// The returned result is encapsulated in a <see cref="LayerLegendInfo" /> object.
		/// A group layer returns only one item describing the group layer (the legends of the sublayers are not returned by this method) 
		/// </remarks>
		/// <param name="callback">The method to call on completion.</param>
		/// <param name="errorCallback">The method to call in the event of an error (cant' happen with a group layer).</param>
		public override void QueryLegendInfos(Action<LayerLegendInfo> callback, Action<Exception> errorCallback)
		{
			if (callback != null)
			{
				if (IsInitialized)
				{
					// create one default layerLegendInfo item.
					LayerLegendInfo layerLegendInfo = new LayerLegendInfo
					                                  	{
					                                  		LegendItemInfos = null,
					                                  		LayerName = Name,
					                                  		LayerDescription = Name,
					                                  		IsHidden = IsHidden
					                                  	};
					callback(layerLegendInfo);
				}
				else
				{
					// delay the answer until initialization
					EventHandler<EventArgs> handler = null;
					handler = delegate(object s, EventArgs e)
								{
									KmlLayer kmlLayer = s as KmlLayer;
									if (kmlLayer == null)
										return;
									kmlLayer.Initialized -= handler;

									// create one default layerLegendInfo item.
									LayerLegendInfo layerLegendInfo = new LayerLegendInfo
									                                  	{
									                                  		LegendItemInfos = null,
									                                  		LayerName = kmlLayer.Name,
									                                  		LayerDescription = kmlLayer.Name,
									                                  		IsHidden = kmlLayer.IsHidden
									                                  	};
									callback(layerLegendInfo);
								};
					Initialized += handler;
				}
			}
		}

		#endregion

	}

	/// <summary>
	/// Class with infos needed to be able to refresh and reparse a KML layer
	/// </summary>
	internal sealed class KmlLayerContext
	{
		/// <summary>
		/// Gets or sets the element (document, folder or NetworkLink) corresponding to the KML layer..
		/// </summary>
		/// <value>The element.</value>
		public XElement Element { get; set; }

		/// <summary>
		/// Gets or sets the atom author of the XElement or inherited from parents.
		/// </summary>
		/// <value>The atom author.</value>
		public string AtomAuthor { get; set; }

		/// <summary>
		/// Gets or sets the atom href of the XElement or inherited from parents.
		/// </summary>
		/// <value>The atom href.</value>
		public Uri AtomHref { get; set; }

		// Styles parsed only once by the root KML layer
		public Dictionary<string, KMLStyle> Styles { get; set; }

		// Images extracted from KMZ file, used for point features or groundoverlays
		public Dictionary<string, ImageBrush> Images { get; set; }

#if !SILVERLIGHT
		internal System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate { get; set; } // workaround for DP not accessible in backgroud thread
#endif

		internal ICredentials Credentials { get; set; } // workaround for DP not accessible in backgroud thread
	}

	internal class WebMercatorProjectionService : IProjectionService
	{
		private static readonly SpatialReference MercatorSref = new SpatialReference(102113);

		public void ProjectAsync(IEnumerable<Graphic> graphics, SpatialReference outSpatialReference)
		{
			if (outSpatialReference == null ||
				outSpatialReference.WKID != 4326 &&
				!outSpatialReference.Equals(MercatorSref))
			{
				//This projector doesn't support this out sref -> Return geometry untouched
				ProjectCompleted(this, new Tasks.GraphicsEventArgs(graphics.ToList(), null));
			}
			else
			{
				//Perform projection
				var result = graphics.Where(g => g != null).Select(g => new Graphic {Geometry = Project(g.Geometry, outSpatialReference)});

				ProjectCompleted(this, new Tasks.GraphicsEventArgs(result.ToList(), null));
			}
		}

		/// <summary>
		/// Projects the specified geometry.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="outSpatialReference">The out spatial reference.</param>
		/// <returns></returns>
		private static Geometry.Geometry Project(Geometry.Geometry geometry, SpatialReference outSpatialReference)
		{
			var toMercator = outSpatialReference.WKID != 4326;
			if (geometry != null && geometry.SpatialReference != null &&
				!geometry.SpatialReference.Equals(outSpatialReference))
			{
				Projection.WebMercator projector = new Projection.WebMercator();
				if (toMercator && geometry.SpatialReference.WKID == 4326)
					//Data is 4326 and must projected to webmercator
					return projector.FromGeographic(geometry);
				if (!toMercator && MercatorSref.Equals(geometry.SpatialReference))
					//Data is in webmercator and must be projected to 4326
					return projector.ToGeographic(geometry);
			}
			// Other cases : geometry without SR, geometry already in the right SR, unsupported SR -> return the input geometry
			return geometry;
		}

		public bool IsBusy { get { return false; } }
		public event EventHandler<Tasks.GraphicsEventArgs> ProjectCompleted;
	}

}
