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
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
    /// <summary>
    /// A layer that conforms to the Web Map Tiling Service (WMTS)  
    /// <a href="http://www.opengeospatial.org" target="_blank">Open GIS Consortium (OGC)</a> standard. WMTS is a 
    /// cached service that accesses pre-created tiles from a cache on a server’s hard drive instead of 
    /// dynamically rendering images.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The full WMTS standard can be found on the 
    /// <a href="http://www.opengeospatial.org/standards/wmts" target="_blank">OpenGIS Web Map Tile Service Implementation Standard</a> 
    /// web page. As of ArcGIS Server version 10.1 serving WMTS layers as a native REST service is supported.
    /// </para>
    /// <para>
    /// In order to consume a WmtsLayer successfully the following Properties must be set:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.Url">Url</see> (The WMTS service endpoint.)
    ///   </item>
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.ServiceMode">ServiceMode</see> (The WMTS 
    ///   service communication protocol.)
    ///   </item>
    /// </list>
    /// <para>
    /// There are however several other optional Properties/Methods/Events that should also be set in order to 
    /// avoid problems consuming a WmtsLayer, they are:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Layer.Initialized">Initialized</see> Method (Method raised when 
    ///   WmtsLayer is being created on the client; useful to get or change information about the layer.)
    ///   </item>
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Layer.InitializationFailed">InitializationFailed</see> Event (Method 
    ///   raised if the WmtsLayer has a problem being created.)
    ///   </item>
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.ProxyUrl">ProxyUrl</see> Property (A proxy 
    ///   service to broker web requests between the WMTS service and the web client.)
    ///   </item>
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.Layer">Layer</see> Property (Name of a 
    ///   single layer in a WMTS service; there can be multiple layers per service.)
    ///   </item>
    ///   <item>
    ///   <see cref="ESRI.ArcGIS.Client.Layer.ID">ID</see> Property (The unique ID of the layer in the Map control.)
    ///   </item>
    /// </list>
    /// <para>
    /// A single WMTS service can have multiple layers. For each WmtsLayer instance, only one layer is drawn at a 
    /// time. In order to discover what layers are available in a WMTS service, append the string 
    /// '?request=GetCapabilities&amp;service=WMTS&amp;version=1.0.0' to what would be supplied for the WmtsLayer.Url Property 
    /// in the address bar of a web browser and the full details will be provided in an XML document. A few example 
    /// strings of obtaining the capabilities of a WMTS service are:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///   http://v2.suite.opengeo.org/geoserver/gwc/service/wmts?request=GetCapabilities&amp;service=WMTS&amp;version=1.0.0
    ///   </item>
    ///   <item>
    ///   http://MyTestServer:6080/arcgis/rest/services/cachedservices/MyTestWMTSService/MapServer/WMTS?request=GetCapabilities&amp;service=WMTS&amp;version=1.0.0
    ///   </item>
    /// </list>
    /// <para>
    /// By default if the WmtsLayer.Layer Property is not specified, the first layer respecting the 
    /// WmtsLayer.TileMatrixSet will be used. When specifying the WmtsLayer.Layer Property at runtime in the 
    /// code-behind, the WmtsLayer’s Refresh Method fires which in turn invokes the WmtsLayer’s Initialized Event 
    /// which will cause a re-draw of the layer in the Map Control.
    /// </para>
    /// <para>
    /// It is important to specify the correct 
    /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsServiceMode">WmtsLayer.WmtsServiceMode</see> 
    /// Enumeration in the WmtsLayer.ServiceMode Property or the WmtsLayer may not be created properly. The two 
    /// available WmtsLayer.WmtsServiceMode Enumeration options available are <b>KVP</b> and <b>RESTful</b>. WMTS 
    /// services provided by ArcGIS Server produce <b>RESTful</b> WmtsLayer.WmtsServiceMode services. The 
    /// <b>KVP</b> WmtsLayer.WmtsServiceMode means HTTP 'Key/Value Pair' encoding; details on this OGC specification 
    /// can found by downloading the 
    /// <a href="http://portal.opengeospatial.org/files/?artifact_id=36263&amp;version=2format=pdf" target="_blank">WCS Extension -- KVP Protocol</a> 
    /// document.
    /// </para>
    /// <para>
    /// A call to the WmtsLayer is Asynchronous. As a result, this means that you cannot obtain valid Read (VB.NET) 
    /// or get (C#) Property values until information has been returned from a WMTS server to the Client application. 
    /// You can safely obtain valid Read/get Property information in the Initialized, PropertyChanged, 
    /// TileLoaded, and TileLoading Events or from a function/sub/method that occurs after these Events fire. If you 
    /// try to obtain Read/get Property information before these Events fire you will obtain invalid or null/Nothing 
    /// information for the particular Property in question. 
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// <b>How to use:</b>
    /// </para>
    /// <para>
    /// Select a layer name in the ListBox and click the 'Get WMTSLayer information' button to display various 
    /// WMTSLayer and WMTSLayer.LayerInfo Property information. The Map will automatically zoom to the Extent 
    /// of the layer. Properties with the words '[Count]' gives the count of the number of items in an 
    /// IEnumerable; more coding could be done in the code-behind to dig deeper into the collection.
    /// </para>
    /// <para>
    /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
    /// the functionality.
    /// </para>
    /// <para>
    /// The following screen shot corresponds to the code example in this page.
    /// </para>
    /// <para>
    /// <img border="0" alt="Example of displaying a WmtsLayer and detailed information about the service." src=" C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.WmtsLayer.png"/>
    /// </para>
    /// <code title="Example XAML1" description="" lang="XAML">
    /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
    ///   
    ///   &lt;!-- Add a Map Control. --&gt;
    ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Name="Map1" 
    ///             VerticalAlignment="Top" WrapAround="True" Height="400" Width="775" Margin="12,80,0,0"&gt;
    ///     &lt;esri:Map.Layers&gt;
    ///       &lt;esri:LayerCollection&gt;
    ///           
    ///         &lt;!-- 
    ///         Add a sample WmtsLayer. Setting the 'ID' Property is good if you want to access the WmtsLayer in code-behind.
    ///         It is mandatory that you set the correct 'ServiceMode' Property. Setting the 'InitializationFailed' Event is useful
    ///         to troubleshoot if the WmtsLayer fails to load. A 'ProxyUrl' is needed to test this particular service; it is not
    ///         always necessary depending on your configuration (i.e. local web service internal to your network). Use the 
    ///         'Initialized' Method to gain access to various Properties/Methods of the WmtsLayer.
    ///         --&gt;
    ///         &lt;esri:WmtsLayer ID="WMTS1"
    ///                         Url="http://v2.suite.opengeo.org/geoserver/gwc/service/wmts"
    ///                         Initialized="WmtsLayer_Initialized"
    ///                         ProxyUrl="http://servicesbeta3.esri.com/SilverlightDemos/ProxyPage/proxy.ashx"
    ///                         InitializationFailed="WmtsLayer_InitializationFailed"
    ///                         ServiceMode="KVP"
    ///                         /&gt;
    ///       &lt;/esri:LayerCollection&gt;
    ///     &lt;/esri:Map.Layers&gt;
    ///   &lt;/esri:Map&gt;
    ///   
    ///   &lt;!-- 
    ///   Add a Button to allow the user to change the sub-Layer that is displaying in the Map along with information
    ///   about the WmtsLayer service.
    ///   --&gt;
    ///   &lt;Button Content="Get WMTSLayer information" Height="23" HorizontalAlignment="Left" Margin="518,484,0,0" 
    ///           Name="Button1" VerticalAlignment="Top" Width="269" Click="Button1_Click"/&gt;
    ///   &lt;ListBox Height="103" HorizontalAlignment="Left" Margin="519,513,0,0" Name="ListBox1" 
    ///            VerticalAlignment="Top" Width="269" /&gt;
    ///   
    ///   &lt;!-- Display various WMTSLayer Property information. --&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="170,479,0,0" Name="Label_WMTSLayerProperties" 
    ///              VerticalAlignment="Top" Width="199" Content="WMTSLayer Properties" FontSize="14" FontWeight="Bold"/&gt;
    ///   
    ///   &lt;!-- FullExtent--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,508,0,0" Name="Label_FullExtent" 
    ///              VerticalAlignment="Top" Width="120" Content="FullExtent:" /&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,508,0,0" 
    ///            Name="TextBox_FullExtent" VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!--Description--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,540,0,0" Name="Label_Description" 
    ///              VerticalAlignment="Top" Width="88" Content="Description:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,536,0,0" Name="TextBox_Description" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!--ImageFormat--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,569,0,0" Name="Label_ImageFormat" 
    ///              VerticalAlignment="Top" Width="88" Content="ImageFormat:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,565,0,0" Name="TextBox_ImageFormat" 
    ///            VerticalAlignment="Top" Width="161" /&gt;
    ///   
    ///   &lt;!--Layer--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="268,565,0,0" Name="Label_Layer" 
    ///              VerticalAlignment="Top" Width="88" Content="Layer:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="309,565,0,0" Name="TextBox_Layer" 
    ///            VerticalAlignment="Top" Width="192" /&gt;
    ///   
    ///   &lt;!--LayerInfos--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,596,0,0" Name="Label_LayerInfos" 
    ///              VerticalAlignment="Top" Width="140" Content="LayerInfos [Count]:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="119,593,0,0" Name="TextBox_LayerInfos" 
    ///            VerticalAlignment="Top" Width="89" /&gt;
    ///   
    ///   &lt;!--ProxyUrl--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,625,0,0" Name="Label_ProxyUrl" 
    ///              VerticalAlignment="Top" Width="88" Content="ProxyUrl:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,622,0,0" Name="TextBox_ProxyUrl" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!-- ServiceMode--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="225,597,0,0" Name="Label_ServiceMode" 
    ///              VerticalAlignment="Top" Width="88" Content="ServiceMode:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="309,593,0,0" Name="TextBox_ServiceMode" 
    ///            VerticalAlignment="Top" Width="192" /&gt;
    ///   
    ///   &lt;!--Style--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,650,0,0" Name="Label_Style" 
    ///              VerticalAlignment="Top" Width="88" Content="Style:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,650,0,0" Name="TextBox_Style" 
    ///            VerticalAlignment="Top" Width="161" /&gt;
    ///   
    ///   &lt;!--TileMatrixSet--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="268,654,0,0" Name="Label_TileMatrixSet" 
    ///              VerticalAlignment="Top" Width="88" Content="TileMatrixSet:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="352,654,0,0" Name="TextBox_TileMatrixSet" 
    ///            VerticalAlignment="Top" Width="149" /&gt;
    ///   
    ///   &lt;!--Title--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,688,0,0" Name="Label_Title" 
    ///              VerticalAlignment="Top" Width="120" Content="Title:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,684,0,0" Name="TextBox_Title" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!--Token--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,717,0,0" Name="Label_Token" 
    ///              VerticalAlignment="Top" Width="120" Content="Token:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,713,0,0" Name="TextBox_Token" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!--Url--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,746,0,0" Name="Label_Url" 
    ///              VerticalAlignment="Top" Width="120" Content="Url:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,742,0,0" Name="TextBox_Url" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   &lt;!--Version--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="7,775,0,0" Name="Label_Version" 
    ///              VerticalAlignment="Top" Width="120" Content="Version:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="101,771,0,0" Name="TextBox_Version" 
    ///            VerticalAlignment="Top" Width="400" /&gt;
    ///   
    ///   
    ///   &lt;!--Display detailed WMTSLayer.LayerInfo about the specific sub-Layer that is being displayed. --&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="543,622,0,0" Name="Label_WMTSLayerLayerInfo" 
    ///              VerticalAlignment="Top" Width="187" Content="WMTSLayer.LayerInfo" FontSize="14" FontWeight="Bold" /&gt;
    ///   
    ///   &lt;!--Abstract--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="511,656,0,0" Name="Label_Abstract" 
    ///              VerticalAlignment="Top" Width="50" Content="Abstract:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="569,652,0,0" Name="TextBox_Abstract" 
    ///            VerticalAlignment="Top" Width="67" /&gt;
    ///   
    ///   &lt;!--Formats--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="642,654,0,0" Name="Label_Formats" 
    ///              VerticalAlignment="Top" Width="97" Content="Formats [Count]:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="743,652,0,0" Name="TextBox_Formats" 
    ///            VerticalAlignment="Top" Width="44" /&gt;
    ///   
    ///   &lt;!--Identifier--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="508,683,0,0" Name="Label_Identifier" 
    ///              VerticalAlignment="Top" Width="60" Content="Identifier:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="571,684,0,0" Name="TextBox_Identifier" 
    ///            VerticalAlignment="Top" Width="216" /&gt;
    ///   
    ///   &lt;!--Styles--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="511,713,0,0" Name="Label_Styles" 
    ///              VerticalAlignment="Top" Width="86" Content="Styles [Count]:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="603,713,0,0" Name="TextBox_Styles" 
    ///            VerticalAlignment="Top" Width="184" /&gt;
    ///   
    ///   &lt;!--TileMatrixSets--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="507,742,0,0" Name="Label_TileMatrixSets" 
    ///              VerticalAlignment="Top" Width="129" Content="TileMatrixSets [Count]:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="642,740,0,0" Name="TextBox_TileMatrixSets" 
    ///            VerticalAlignment="Top" Width="145" /&gt;
    ///   
    ///   &lt;!--Title--&gt;
    ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="513,768,0,0" Name="Label_Title2" 
    ///              VerticalAlignment="Top" Width="48" Content="Title:"/&gt;
    ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="554,769,0,0" Name="TextBox_Title2" 
    ///            VerticalAlignment="Top" Width="236" /&gt;
    ///   
    ///   
    ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
    ///   &lt;TextBlock Height="74" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
    ///          TextWrapping="Wrap" Margin="12,12,0,0" 
    ///          Text="Select a layer name in the ListBox and click the 'Get WMTSLayer information' button to display
    ///          various WMTSLayer and WMTSLayer.LayerInfo Property information. The Map will automatically zoom to the
    ///          Extent of the layer. Properties with the words '[Count]' gives the count of the number of items in 
    ///          an IEnumerable; more coding could be done in the code-behind to dig deeper into the collection." /&gt;
    ///   
    /// &lt;/Grid&gt;
    /// </code>
    /// <code title="Example CS1" description="" lang="CS">
    /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
    /// {
    ///   // This function takes the user choice for a specific WmtsLayer.Layer from a ListBox and refreshes 
    ///   // the Map using that layer.
    ///   
    ///   // Get the user choice for the name of the WmtsLayer.Layer.
    ///   string theTitle = ListBox1.SelectedItem.ToString();
    ///   
    ///   // Get the WmtsLayer that was defined in XAML.
    ///   ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer theWMTSLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer)Map1.Layers["WMTS1"];
    ///   
    ///   // Set the specific layer to display in the WmtsLayer. This will internally cause a WmtsLayer.Refresh
    ///   // which in turn causes the WmtsLayer.Initialized Method to execute.
    ///   theWMTSLayer.Layer = theTitle;
    /// }
    /// 
    /// private void WmtsLayer_Initialized(object sender, System.EventArgs e)
    /// {
    ///   // This function loops through all of the sub-Layers in a WmtsLayer service and displays information
    ///   // about the WmtsLayer service in general and details about a particular WmtsLayer.Layer specified
    ///   // by the user choice. This function initiates whenever the WmtsLayer first initializes or as a result 
    ///   // of specifying a different WmtsLayer.Layer. 
    ///   
    ///   // Clear out all of the sub-Layer names of the WmtsLayer service.
    ///   ListBox1.Items.Clear();
    ///   
    ///   // Get the WmtsLayer that was defined in XAML.
    ///   ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer theWMTSLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer)Map1.Layers["WMTS1"];
    ///   
    ///   // Get all of the sub-Layer information from the WmtsLayer.
    ///   IEnumerable&lt;ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo&gt; theLayers = theWMTSLayer.LayerInfos;
    ///   if (theLayers != null)
    ///   {
    ///     foreach (ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo oneWMTSLayerInfo in theLayers)
    ///     {
    ///   	//Display the name of the WmtsLayer sub-Layer's in the ListBox. 
    ///   	ListBox1.Items.Add(oneWMTSLayerInfo.Title);
    ///     }
    ///   }
    ///   
    ///   // Set the Map.Extent to the WmtsLayer sub-Layer and display the numerical extent in a TextBox.
    ///   if (theWMTSLayer.FullExtent != null)
    ///   {
    ///     Map1.Extent = theWMTSLayer.FullExtent;
    ///     TextBox_FullExtent.Text = theWMTSLayer.FullExtent.ToString();
    ///   }
    ///   
    ///   // Display the Description of the WmtsLayer service.
    ///   if (theWMTSLayer.Description != null)
    ///   {
    ///     TextBox_Description.Text = theWMTSLayer.Description;
    ///   }
    ///   
    ///   // Display the ImageFormat of the WmtsLayer service.
    ///   if (theWMTSLayer.ImageFormat != null)
    ///   {
    ///     TextBox_ImageFormat.Text = theWMTSLayer.ImageFormat;
    ///   }
    ///   
    ///   // Display the currently displaying sub-Layer name of the WmtsLayer service.
    ///   if (theWMTSLayer.Layer != null)
    ///   {
    ///     TextBox_Layer.Text = theWMTSLayer.Layer;
    ///   }
    ///   
    ///   // Display the LayerInfos.Count of the WmtsLayer service. More information is available if you
    ///   // care to loop through the IEnumerable&lt;WmtsLayer.WmtsLayerInfo&gt; objects.
    ///   if (theWMTSLayer.LayerInfos != null)
    ///   {
    ///     TextBox_LayerInfos.Text = theWMTSLayer.LayerInfos.Count.ToString();
    ///   }
    ///   
    ///   // Display the ProxyUrl of the WmtsLayer service.
    ///   if (theWMTSLayer.ProxyUrl != null)
    ///   {
    ///     TextBox_ProxyUrl.Text = theWMTSLayer.ProxyUrl;
    ///   }
    ///   
    ///   // Display the ServiceMode Enumeration of the WmtsLayer service.
    ///   TextBox_ServiceMode.Text = theWMTSLayer.ServiceMode.ToString(); //An Enumeration.
    ///   
    ///   // Display the Style of the WmtsLayer service.
    ///   if (theWMTSLayer.Style != null)
    ///   {
    ///     TextBox_Style.Text = theWMTSLayer.Style;
    ///   }
    ///   
    ///   // Display the TileMatrix of the WmtsLayer service.
    ///   if (theWMTSLayer.TileMatrixSet != null)
    ///   {
    ///     TextBox_TileMatrixSet.Text = theWMTSLayer.TileMatrixSet;
    ///   }
    ///   
    ///   // Display the Title of the WmtsLayer service. Select the name of the Title in the ListBox to
    ///   // show which sub-Layer is currently being displayed in the Map.
    ///   if (theWMTSLayer.Title != null)
    ///   {
    ///     TextBox_Title.Text = theWMTSLayer.Title;
    ///     ListBox1.SelectedItem = theWMTSLayer.Title;
    ///   }
    ///   
    ///   // Display the Token of the WmtsLayer service.
    ///   if (theWMTSLayer.Token != null)
    ///   {
    ///     TextBox_Token.Text = theWMTSLayer.Token;
    ///   }
    ///   
    ///   // Display the Url of the WmtsLayer service.
    ///   if (theWMTSLayer.Url != null)
    ///   {
    ///     TextBox_Url.Text = theWMTSLayer.Url;
    ///   }
    ///   
    ///   // Display the Version of the WmtsLayer service.
    ///   if (theWMTSLayer.Version != null)
    ///   {
    ///     TextBox_Version.Text = theWMTSLayer.Version;
    ///   }
    ///   
    ///   // Get the WmtsLayer.Title from the user choice in the ListBox. This will be used to display detailed 
    ///   // information (WmtsLayer.WmtsLayerInfo) about the specific WmtsLayer.Layer currently being shown in the map.
    ///   string theTitle = null;
    ///   if (ListBox1.SelectedItem != null)
    ///   {
    ///     theTitle = ListBox1.SelectedItem.ToString();
    ///   }
    ///   
    ///   // Loop through all of the sub-Layers of the WmtsLayer and display WmtsLayer.WmtsLayerInfo Property
    ///   // information that match the user choice in the ListBox.
    ///   if (theLayers != null)
    ///   {
    ///     foreach (ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo oneWMTSLayerInfo in theLayers)
    ///     {
    ///       if (oneWMTSLayerInfo.Title == theTitle)
    ///       {
    ///         // Display the Abstract of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         if (oneWMTSLayerInfo.Abstract != null)
    ///         {
    ///           TextBox_Abstract.Text = oneWMTSLayerInfo.Abstract;
    ///         }
    ///         
    ///         // Display the Formats of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         // care to loop through the IEnumerable&lt;String&gt; objects.
    ///         if (oneWMTSLayerInfo.Formats != null)
    ///         {
    ///           TextBox_Formats.Text = oneWMTSLayerInfo.Formats.Count.ToString();
    ///         }
    ///         
    ///         // Display the Identifier of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         if (oneWMTSLayerInfo.Identifier != null)
    ///         {
    ///           TextBox_Identifier.Text = oneWMTSLayerInfo.Identifier;
    ///         }
    ///         
    ///         // Display the Styles of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         // care to loop through the IEnumerable&lt;String&gt; objects.
    ///         if (oneWMTSLayerInfo.Styles != null)
    ///         {
    ///           TextBox_Styles.Text = oneWMTSLayerInfo.Styles.Count.ToString();
    ///         }
    ///         
    ///         // Display the TileMatrixSets of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         // care to loop through the IEnumerable&lt;String&gt; objects.
    ///         if (oneWMTSLayerInfo.TileMatrixSets != null)
    ///         {
    ///           TextBox_TileMatrixSets.Text = oneWMTSLayerInfo.TileMatrixSets.Count.ToString();
    ///         }
    ///         
    ///         // Display the Title of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         if (oneWMTSLayerInfo.Title != null)
    ///         {
    ///           TextBox_Title2.Text = oneWMTSLayerInfo.Title;
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// 
    /// private void WmtsLayer_InitializationFailed(object sender, System.EventArgs e)
    /// {
    ///   // This function displays any error information of the WmtsLayer fails to load.
    ///   
    ///   ESRI.ArcGIS.Client.Layer aLayer = (ESRI.ArcGIS.Client.Layer)sender;
    ///   MessageBox.Show(aLayer.InitializationFailure.Message);
    /// }
    /// </code>
    /// <code title="Example VB1" description="" lang="VB.NET">
    /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    ///   
    ///   ' This function takes the user choice for a specific WmtsLayer.Layer from a ListBox and refreshes 
    ///   ' the Map using that layer.
    ///   
    ///   ' Get the user choice for the name of the WmtsLayer.Layer.
    ///   Dim theTitle As String = ListBox1.SelectedItem.ToString
    ///   
    ///   ' Get the WmtsLayer that was defined in XAML.
    ///   Dim theWMTSLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer = Map1.Layers("WMTS1")
    ///   
    ///   ' Set the specific layer to display in the WmtsLayer. This will internally cause a WmtsLayer.Refresh
    ///   ' which in turn causes the WmtsLayer.Initialized Method to execute.
    ///   theWMTSLayer.Layer = theTitle
    ///   
    /// End Sub
    ///   
    /// Private Sub WmtsLayer_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs)
    ///   
    ///   ' This function loops through all of the sub-Layers in a WmtsLayer service and displays information
    ///   ' about the WmtsLayer service in general and details about a particular WmtsLayer.Layer specified
    ///   ' by the user choice. This function initiates whenever the WmtsLayer first initializes or as a result 
    ///   ' of specifying a different WmtsLayer.Layer. 
    ///   
    ///   ' Clear out all of the sub-Layer names of the WmtsLayer service.
    ///   ListBox1.Items.Clear()
    ///   
    ///   ' Get the WmtsLayer that was defined in XAML.
    ///   Dim theWMTSLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer = Map1.Layers("WMTS1")
    ///   
    ///   ' Get all of the sub-Layer information from the WmtsLayer.
    ///   Dim theLayers As IEnumerable(Of ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo) = theWMTSLayer.LayerInfos
    ///   If theLayers IsNot Nothing Then
    ///     For Each oneWMTSLayerInfo As ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo In theLayers
    ///       'Display the name of the WmtsLayer sub-Layer's in the ListBox. 
    ///       ListBox1.Items.Add(oneWMTSLayerInfo.Title)
    ///     Next
    ///   End If
    ///   
    ///   ' Set the Map.Extent to the WmtsLayer sub-Layer and display the numerical extent in a TextBox.
    ///   If theWMTSLayer.FullExtent IsNot Nothing Then
    ///     Map1.Extent = theWMTSLayer.FullExtent
    ///     TextBox_FullExtent.Text = theWMTSLayer.FullExtent.ToString
    ///   End If
    ///   
    ///   ' Display the Description of the WmtsLayer service.
    ///   If theWMTSLayer.Description IsNot Nothing Then
    ///     TextBox_Description.Text = theWMTSLayer.Description
    ///   End If
    ///   
    ///   ' Display the ImageFormat of the WmtsLayer service.
    ///   If theWMTSLayer.ImageFormat IsNot Nothing Then
    ///     TextBox_ImageFormat.Text = theWMTSLayer.ImageFormat
    ///   End If
    ///   
    ///   ' Display the currently displaying sub-Layer name of the WmtsLayer service.
    ///   If theWMTSLayer.Layer IsNot Nothing Then
    ///     TextBox_Layer.Text = theWMTSLayer.Layer
    ///   End If
    ///   
    ///   ' Display the LayerInfos.Count of the WmtsLayer service. More information is available if you
    ///   ' care to loop through the IEnumerable(Of WmtsLayer.WmtsLayerInfo) objects.
    ///   If theWMTSLayer.LayerInfos IsNot Nothing Then
    ///     TextBox_LayerInfos.Text = theWMTSLayer.LayerInfos.Count.ToString
    ///   End If
    ///   
    ///   ' Display the ProxyUrl of the WmtsLayer service.
    ///   If theWMTSLayer.ProxyUrl IsNot Nothing Then
    ///     TextBox_ProxyUrl.Text = theWMTSLayer.ProxyUrl
    ///   End If
    ///   
    ///   ' Display the ServiceMode Enumeration of the WmtsLayer service.
    ///   TextBox_ServiceMode.Text = theWMTSLayer.ServiceMode.ToString 'An Enumeration.
    ///   
    ///   ' Display the Style of the WmtsLayer service.
    ///   If theWMTSLayer.Style IsNot Nothing Then
    ///     TextBox_Style.Text = theWMTSLayer.Style
    ///   End If
    ///   
    ///   ' Display the TileMatrix of the WmtsLayer service.
    ///   If theWMTSLayer.TileMatrixSet IsNot Nothing Then
    ///     TextBox_TileMatrixSet.Text = theWMTSLayer.TileMatrixSet
    ///   End If
    ///   
    ///   ' Display the Title of the WmtsLayer service. Select the name of the Title in the ListBox to
    ///   ' show which sub-Layer is currently being displayed in the Map.
    ///   If theWMTSLayer.Title IsNot Nothing Then
    ///     TextBox_Title.Text = theWMTSLayer.Title
    ///     ListBox1.SelectedItem = theWMTSLayer.Title
    ///   End If
    ///   
    ///   ' Display the Token of the WmtsLayer service.
    ///   If theWMTSLayer.Token IsNot Nothing Then
    ///     TextBox_Token.Text = theWMTSLayer.Token
    ///   End If
    ///   
    ///   ' Display the Url of the WmtsLayer service.
    ///   If theWMTSLayer.Url IsNot Nothing Then
    ///     TextBox_Url.Text = theWMTSLayer.Url
    ///   End If
    ///   
    ///   ' Display the Version of the WmtsLayer service.
    ///   If theWMTSLayer.Version IsNot Nothing Then
    ///     TextBox_Version.Text = theWMTSLayer.Version
    ///   End If
    ///   
    ///   ' Get the WmtsLayer.Title from the user choice in the ListBox. This will be used to display detailed 
    ///   ' information (WmtsLayer.WmtsLayerInfo) about the specific WmtsLayer.Layer currently being shown in the map.
    ///   Dim theTitle As String = Nothing
    ///   If ListBox1.SelectedItem IsNot Nothing Then
    ///     theTitle = ListBox1.SelectedItem.ToString
    ///   End If
    ///   
    ///   ' Loop through all of the sub-Layers of the WmtsLayer and display WmtsLayer.WmtsLayerInfo Property
    ///   ' information that match the user choice in the ListBox.
    ///   If theLayers IsNot Nothing Then
    ///     For Each oneWMTSLayerInfo As ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.WmtsLayerInfo In theLayers
    ///       If oneWMTSLayerInfo.Title = theTitle Then
    ///         
    ///         ' Display the Abstract of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         If oneWMTSLayerInfo.Abstract IsNot Nothing Then
    ///           TextBox_Abstract.Text = oneWMTSLayerInfo.Abstract
    ///         End If
    ///         
    ///         ' Display the Formats of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         ' care to loop through the IEnumerable(Of String) objects.
    ///         If oneWMTSLayerInfo.Formats IsNot Nothing Then
    ///           TextBox_Formats.Text = oneWMTSLayerInfo.Formats.Count.ToString
    ///         End If
    ///         
    ///         ' Display the Identifier of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         If oneWMTSLayerInfo.Identifier IsNot Nothing Then
    ///           TextBox_Identifier.Text = oneWMTSLayerInfo.Identifier
    ///         End If
    ///         
    ///         ' Display the Styles of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         ' care to loop through the IEnumerable(Of String) objects.
    ///         If oneWMTSLayerInfo.Styles IsNot Nothing Then
    ///           TextBox_Styles.Text = oneWMTSLayerInfo.Styles.Count.ToString
    ///         End If
    ///         
    ///         ' Display the TileMatrixSets of the WmtsLayer.WmtsLayerInfo sub-Layer. More information is available if you
    ///         ' care to loop through the IEnumerable(Of String) objects.
    ///         If oneWMTSLayerInfo.TileMatrixSets IsNot Nothing Then
    ///           TextBox_TileMatrixSets.Text = oneWMTSLayerInfo.TileMatrixSets.Count.ToString
    ///         End If
    ///         
    ///         ' Display the Title of the WmtsLayer.WmtsLayerInfo sub-Layer.
    ///         If oneWMTSLayerInfo.Title IsNot Nothing Then
    ///           TextBox_Title2.Text = oneWMTSLayerInfo.Title
    ///         End If
    ///         
    ///       End If
    ///     Next
    ///   End If
    ///   
    /// End Sub
    ///   
    /// Private Sub WmtsLayer_InitializationFailed(ByVal sender As System.Object, ByVal e As System.EventArgs)
    ///   
    ///   ' This function displays any error information of the WmtsLayer fails to load.
    ///   
    ///   Dim aLayer As ESRI.ArcGIS.Client.Layer = sender
    ///   MessageBox.Show(aLayer.InitializationFailure.Message)
    ///   
    /// End Sub
    /// </code>
    /// </example>
	public class WmtsLayer : TiledMapServiceLayer
	{
		#region Private fields

		private string _httpGetKVPTileResource;

		private static readonly Version _highestSupportedVersion = new Version(1, 0);

		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="WmtsLayer"/> class.
		/// </summary>
		public WmtsLayer()
		{
			Version = "1.0.0";
		}
		
		#endregion

		// Connection properties
		////////////////////////
		#region Url Dependency Property
		/// <summary>
		/// Required.  Gets or sets the URL to a WMTS service endpoint.  
		/// For example,
		/// http://v2.suite.opengeo.org/geoserver/gwc/service/wmts
		/// </summary>
		/// <value>The URL.</value>
		public string Url
		{
			get { return (string)GetValue(UrlProperty); }
			set { SetValue(UrlProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Url"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty UrlProperty =
			DependencyProperty.Register("Url", typeof(string), typeof(WmtsLayer), new PropertyMetadata(OnUrlPropertyChanged));

		private static void OnUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((WmtsLayer) d).OnUrlPropertyChanged(e.NewValue as string);
		}

		private void OnUrlPropertyChanged(string newUrl)
		{
			if (!SkipGetCapabilities && (IsInitialized || _initializing) && !string.IsNullOrEmpty(newUrl))
			{
				// reinitialize the layer to take care of the new url
				IsInitialized = false;
				_initializing = false;
				InitializationFailure = null;
				LayerInfos = null;
				Refresh();
				Initialize();
			}
		}
		#endregion

		#region Token Dependency Property
		/// <summary>Gets or sets the token for accessing a secure ArcGIS service.</summary>
		/// <value>The token.</value>
		/// <remarks>
		/// ArcGIS Server services may be secured using token authentication. Use 
        /// <a href="javascript:ApiToConcept('discovering-services', '01n700000004000000', '011v00000007000000')" target="_top">Services Directory</a> 
		/// to determine if a service requires a
		/// token to be used. A token is an encrypted string generated by a token service on the
		/// same ArcGIS Server site and the secured service. The token service can be accessed in a
		/// browser via the Get Token link in Services Explorer or by navigating to the main token
		/// service page (e.g. <a href="http://www.example.com/ArcGIS/tokens/gettoken.html">http://www.example.com/ArcGIS/tokens/gettoken.html</a>).
		/// See the discussion topic on 
        /// <a href="javascript:ApiToConcept('secure-services', '01n700000022000000', '011v0000000n000000')" target="_top">secure services</a> 
		/// for more info.
		/// </remarks>
		/// <example>
		/// 	<code lang="XAML">
		/// 		<![CDATA[
		/// <esri:WmtsLayer ID="WmtsLayer"
		///     Url="http://serverapps.esri.com/ArcGIS/rest/services/California/MapServer/WMTS" 
		///     Token="T2ILopZdSMylbhKIysHa-8YgBVNPjRHsK-Kw3VoQS2RUQ0UpUAj30vGfT92YlEue" />]]>
		/// 	</code>
		/// </example>
		public string Token
		{
			get { return (string)GetValue(TokenProperty); }
			set { SetValue(TokenProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Token"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TokenProperty =
			DependencyProperty.Register("Token", typeof(string), typeof(WmtsLayer), new PropertyMetadata(null, OnTokenPropertyChanged));

		private static void OnTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is WmtsLayer)
				(d as WmtsLayer).OnTokenPropertyChanged();
		}

		private void OnTokenPropertyChanged()
		{
			// if the token has changed and if the initialization is on the way or has failed --> initialize again the layer with the new token
			// if the initialization was OK with the previous token --> nothing to do since we assume that the token doesn't change the metadata
			if (_initializing || (IsInitialized && InitializationFailure != null))
			{
				IsInitialized = false;
				_initializing = false;
				InitializationFailure = null;
				Initialize();
			}
		} 
		#endregion

		#region ProxyUrl
		/// <summary>
		/// Optional. Gets or sets the URL to a proxy service that brokers Web requests between the client application 
  /// and a WMTS service. Use a proxy service when the WMTS service is not hosted on a site that provides
		/// a cross domain policy file (clientaccesspolicy.xml or crossdomain.xml). 
  /// <ESRISILVERLIGHT>You can also use a proxy to convert png images to a bit-depth that supports transparency in Silverlight.</ESRISILVERLIGHT>
		/// </summary>
		/// <value>The proxy URL string.</value>
		public string ProxyUrl { get; set; }

		#endregion

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
			DependencyProperty.Register("ClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(WmtsLayer), null);
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
			DependencyProperty.Register("Credentials", typeof(System.Net.ICredentials), typeof(WmtsLayer), null);
#endif

		#region ServiceMode

		/// <summary>
		/// Gets or sets the service mode i.e. "KVP" or "RESTful". By default, it's "RESTful".
		/// </summary>
		/// <remarks>
		/// If the mode doesn't match the service, it won't create the WMTS layer successfully. </remarks>
		/// <value>The service mode.</value>
		public WmtsServiceMode ServiceMode
		{
			get { return (WmtsServiceMode)GetValue(ServiceModeProperty); }
			set { SetValue(ServiceModeProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ServiceMode"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ServiceModeProperty = 
			DependencyProperty.Register("ServiceMode", typeof(WmtsServiceMode), typeof(WmtsLayer), new PropertyMetadata(WmtsServiceMode.KVP, OnServiceModePropertyChanged));

		private static void OnServiceModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			WmtsLayer wmtsLayer = ((WmtsLayer)d);
			if (wmtsLayer._initializing)
			{
				wmtsLayer._initializing = false;
				wmtsLayer.Initialize();
			}
		}


		/// <summary>
		/// Indicates which architecture style is using the server : procedure oriented (KVP) or resource oriented (RESTFul)
		/// </summary>
		/// <remarks>The procedure oriented architecture style with SOAP encodings is not supported.</remarks>
		public enum WmtsServiceMode
		{
			/// <summary>
			/// The WMTS server operates in a procedure oriented architecture style.
			/// HTTP GET Transfer of operation requests is using KVP (Key-Value-Pair) encodings.
			/// </summary>
			KVP,
			/// <summary>
			/// The WMTS server operates in a resource oriented architecture style (REST).
			/// </summary>
			RESTful
		}
		
		#endregion

		#region Version
		/// <summary>
		/// Optional. Gets or sets the WMTS version.
		/// The default value is 1.0.0
		/// </summary>
		/// <remarks>At this time, the only possible value is 1.0.0 </remarks>
		/// <value>The version string.</value>
		public string Version { get; set; }
		#endregion


		// Optional properties allowing to select a layer/a style/a tile matrixset/ a format
		////////////////////////////////////////////////////////////////////////////////////
		#region Layer
		private string _layer;
		/// <summary>
		/// Gets or sets the layer identifier displayed by the WMTS service.
		/// </summary>
		/// <remarks>
		/// If the layer is not set explicitly, the first layer respecting the TileMatrixSet will be used.</remarks>
		/// <value>The layer.</value>
		public string Layer
		{
			get { return _layer; }
			set
			{
				if (_layer != value)
				{
					_layer = value;
					SetCurrentLayer();
					OnPropertyChanged("Layer");
					if (IsInitialized)
						Refresh();
				}
			}
		} 
		#endregion

		#region TileMatrixSet
		private string _tileMatrixSet;

		/// <summary>
		/// Gets or sets the tile matrix set.
		/// It defines the tileMatrixSet the layer will use. 
		/// </summary>
		/// <remarks>
		/// If the tile matrix set is not set explicitly, the first tile matrix set supported by the WMTS will be used.</remarks>
		/// <value>The tile matrix set.</value>
		public string TileMatrixSet
		{
			get { return _tileMatrixSet; }
			set
			{
				if (_tileMatrixSet != value)
				{
					_tileMatrixSet = value;
					SetCurrentTileMatrixSet();
					OnPropertyChanged("TileMatrixSet");
					if (IsInitialized)
						Refresh();
				}
			}
		}
		#endregion

		#region Style
		private string _style;
		/// <summary>
		/// Gets or sets the style to apply to the WMTS layer.
		/// </summary>
		/// <value>The style.</value>
		public string Style
		{
			get { return _style; }
			set
			{
				if (_style != value)
				{
					_style = value;
					SetCurrentStyle();
					OnPropertyChanged("Style");
					if (IsInitialized)
						Refresh();
				}
			}
		}
		
		#endregion

		#region ImgeFormat
		private string _imageFormat;

		/// <summary>
		/// Gets or sets the image format used by the WMTS layer.
		/// </summary>
		/// <value>The image format.</value>
		public string ImageFormat
		{
			get { return _imageFormat; }
			set
			{
				if (_imageFormat != value)
				{
					_imageFormat = value;
					SetCurrentFormat();
					OnPropertyChanged("ImageFormat");
					if (IsInitialized)
						Refresh();
				}
			}
		}
		
		#endregion

		// Other properties
		///////////////////
		#region SkipGetCapabilities

		private bool _skipGetCapabilities;

		/// <summary>
		/// Optional. Gets or sets a value indicating whether to skip a request to get capabilities. 
		/// Default value is false.  Set SkipGetCapabilities if the site hosting the WMTS service does not provide a
		/// cross domain policy file and you do not have a proxy page.  In this case, you must set the WMTS service version.
		/// If true, the initial and full extent of the WMTS Silverlight layer will not be defined.
		/// </summary>
		internal bool SkipGetCapabilities // TODO should be public later
		{
			get { return _skipGetCapabilities; }
			set
			{
				if (_skipGetCapabilities != value)
				{
					_skipGetCapabilities = value;
					if (_skipGetCapabilities)
					{
						// reset useless internal fields
						LayerInfos = null;
						CurrentLayer = null;
						CurrentTileMatrixSet = null;
						CurrentTileMatrixSetLink = null;
					}

					if (IsInitialized)
					{
						// Reinitialize the layer for taking care of this change
						IsInitialized = _initializing = false;
						InitializationFailure = null;
						Refresh();
						Initialize();
					}
					OnPropertyChanged("SkipGetCapabilities");
				}
			}
		}

		#endregion

		#region Title
		private string _title;

		/// <summary>
		/// The title of the currently active layer.
		/// </summary>
		public string Title
		{
			get
			{
				return CurrentLayer == null ? _title : CurrentLayer.Title;
			}
			private set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
		
		#endregion

		#region Description
		/// <summary>
		/// Gets or sets the description of the currently active layer.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get
			{
				return CurrentLayer == null ? null : CurrentLayer.Abstract;
			}
		} 
		#endregion

		#region LayerInfos
		private IEnumerable<WmtsLayerInfo> _layerInfos;
		/// <summary>
		/// An enumeration of WmtsLayerInfo describing the layers available for this WMTS service.
		/// </summary>
		/// <remarks>
		/// This enumeration is only available if SkipGetCapabilities is false and after the layer is initialized.
		/// </remarks>
		/// <value>The layer infos.</value>
		public IEnumerable<WmtsLayerInfo> LayerInfos
		{
			get { return _layerInfos; }
			internal set
			{
				_layerInfos = value;
				OnPropertyChanged("LayerInfos");
			}
		} 
		#endregion

		#region Dimensions

		private WmtsDimensionValueCollection _dimensionValues;

		/// <summary>
		///  Gets or sets the dimensional value(s) used by the WMTS layer to request the tiles.
		/// </summary>
		/// <para>Examples of dimensions are Time, Elevation and Band but the service can define any other dimension property that exists in the multidimensional layer collection being served.</para>
		/// <para>If the DimensionValues collection doesn't specify the value for a dimension supported by the service, the following value will be used by order of priority:
		/// <list type="bullet">
		/// <item>the <see cref="WmtsDimensionInfo.Default"/> value if not null.</item>
		/// <item>the 'current' value if the dimension <see cref="WmtsDimensionInfo.SupportsCurrent">supports a current value</see>.</item>
		/// <item>the first value in <see cref="WmtsDimensionInfo.Values"/>.</item>
		/// </list>
		/// </para>
		public WmtsDimensionValueCollection DimensionValues
		{
			get { return _dimensionValues; }
			set
			{
				if (_dimensionValues != value)
				{
					if (_dimensionValues != null)
						_dimensionValues.CollectionChanged -= DimensionValuesOnCollectionChanged;
					_dimensionValues = value;
					OnPropertyChanged("DimensionValues");
					SetCurrentDimensionValues();
					if (IsInitialized)
						Refresh(); 
					if (_dimensionValues != null)
						_dimensionValues.CollectionChanged += DimensionValuesOnCollectionChanged;
				}
			}
		}

		void DimensionValuesOnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (IsInitialized)
				Refresh();
		}

		#endregion

		#region WmtsLayerInfo class
		/// <summary>
		/// Information about a WMTS layer.
		/// </summary>
		public sealed class WmtsLayerInfo
		{
			/// <summary>
			/// Gets the identifier of the layer.
			/// </summary>
			/// <value>The identifier.</value>
			public string Identifier { get; internal set; }

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
			/// Gets the enumeration of image formats supported by the layer.
			/// </summary>
			/// <value>The formats.</value>
			public IEnumerable<string> Formats { get; internal set; }

			/// <summary>
			/// Gets the enumeration of styles supported by the layer.
			/// </summary>
			/// <value>The styles.</value>
			public IEnumerable<string> Styles { get; internal set; }

			/// <summary>
			/// Gets the enumeration of tile matrix sets supported by the layer.
			/// </summary>
			/// <value>The tile matrix sets.</value>
			public IEnumerable<string> TileMatrixSets
			{
				get { return TileMatrixSetLinks.Select(tmsl => tmsl.TileMatrixSet); }
			}

			/// <summary>
			/// Gets the extra dimensions for a tile request.
			/// </summary>
			/// <value>
			/// The extra dimensions supported by the layer.
			/// </value>
			public IEnumerable<WmtsDimensionInfo> DimensionInfos { get; internal set; } 

			/// <summary>
			/// Gets the extent of the layer.
			/// </summary>
			/// <value>The extent.</value>
			internal Envelope Extent { get; set; }
			internal IEnumerable<TileMatrixSetLinkInfo> TileMatrixSetLinks { get; set; }
			internal IEnumerable<ResourceUrlInfo> ResourceUrls { get; set; }
		} 
		#endregion

		#region Override Initialize
		private bool _initializing;
		/// <summary>
		/// Initializes this WMTS layer.  Calls GetCapabilities if SkipGetCapabilities is false. 
		/// </summary>
		public override void Initialize()
		{
			if (_initializing || IsInitialized || System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
				return;

			if (string.IsNullOrEmpty(Url))
			{
				InitializationFailure = new ArgumentNullException("Url");
			}
			else
			{

				if (SkipGetCapabilities)
				{
					if (TileInfo == null)
					{
						InitializationFailure = new ArgumentNullException("TileInfo");
					}
					// TODO need the fullextent?
				}
				else
				{
					if (LayerInfos == null)
					{
						// Get the capabilities
						_initializing = true;
						string wmtsUrl;
						string queryString;
						if (ServiceMode == WmtsServiceMode.RESTful)
						{
							wmtsUrl = Url + "/" + Version + "/WMTSCapabilities.xml";
							queryString = string.IsNullOrEmpty(Token) ? string.Empty : string.Format("token={0}", Token);
						}
						else
						{
							wmtsUrl = Url;
							queryString = string.Format("service=WMTS&request=GetCapabilities&version={0}", GetValidVersionNumber());
							if (!string.IsNullOrEmpty(Token))
								queryString += string.Format("&token={0}", Token);
						}
						if (!string.IsNullOrEmpty(queryString))
							wmtsUrl = CreateUrl(wmtsUrl, queryString);

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
						client.DownloadStringAsync(Utilities.PrefixProxy(ProxyUrl, wmtsUrl));
						return;
					}
				}
			}
			base.Initialize();
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
				if (providedVersion <= _highestSupportedVersion)
					return Version;
			}
			catch { }
			return "1.0.0";
		}

		internal void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (!CheckForError(e))
			{
				try
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
				catch (Exception ex)
				{
					InitializationFailure = ex;
				}

				SetCurrentLayer();
			}

			// Call initialize regardless of error
			base.Initialize();
			Refresh();
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
						string.Format(Properties.Resources.MapService_SecurityException, "WMTS"),
						ex);
				}
				InitializationFailure = ex;
				return true;
			}
			return false;
		}


		#endregion

		#region Override GetTileUrl

  /// <summary>
  /// Returns a URL to the specific tile in a WmtsLayer.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A WmtsLayer is made up of multiple tiles (or images) that are automatically put together in 
  /// a mosaic for display in a Map Control. The tiles are pre-generated on a WMTS server and can 
  /// be accessed individually via a URL. In order to access the URL for a specific tile it is 
  /// required to know the Level, Row, and Column information. As of ArcGIS Server version 10.1, 
  /// serving WMTS layers as a native REST service is supported.
  /// </para>
  /// <para>
  /// It is possible to obtain the complete list of various Level, Row, and Column input parameter 
  /// values that can be used by the GetTileUrl Method by interrogating the XML information 
  /// returned from a GetCapabilities request in the address bar of a web browser but this is 
  /// tedious (see the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer">WmtsLayer</see> 
  /// documentation for a few examples of using the GetCapabilities request). A programmatic way 
  /// to determine the various Level, Row, and Column information can be obtained by writing some 
  /// code-behind logic in the 
  /// <see cref="ESRI.ArcGIS.Client.TiledLayer.TileLoading">WmtsLayer.TileLoading</see> Event 
  /// (see the code example in this document).
  /// </para>
  /// <para>
  /// If the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer.ProxyUrl">ProxyUrl</see> 
  /// Property has been set to create the WmtsLayer, then the output return string of the GetTileUrl 
  /// Method will have that ProxyUrl value inserted at the beginning.
  /// </para>
  /// <para>
  /// <b>Note:</b> Using Methods are only available in code-behind. You cannot use a Method via XAML.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// After the WmtsLayer loads in the Map Control, the ListBox will be populated with all the 
  /// combinations of 'Level, Row, and Column' tiles that make up the initial extent of the WmtsLayer 
  /// image service. Click on any of the combinations in the Listbox and that particular tile will be 
  /// displayed in an Image Control as well as the Url for that image.
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Displaying individual tile images and their URL values for a WmtsLayer." src=" C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.WmtsLayer.GetTileUrl.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="78" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="640" 
  ///        TextWrapping="Wrap" Text="After the WmtsLayer loads in the Map Control, the ListBox will be populated with
  ///        all the combinations of 'Level, Row, and Column' tiles that make up the initial extent of the WmtsLayer 
  ///        image service. Click on any of the combinations in the Listbox and that particular tile will be 
  ///        displayed in an Image Control as well as the Url for that image." /&gt;
  ///   
  ///   &lt;!-- The Map Control. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="33,160,0,0" Name="Label_MapControl" 
  ///              VerticalAlignment="Top" Width="120" Content="Map Control:"/&gt;
  ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="32,180,0,0" Name="Map1" 
  ///             VerticalAlignment="Top" WrapAround="True" Height="320" Width="600"&gt;
  ///     &lt;esri:Map.Layers&gt;
  ///       &lt;esri:LayerCollection&gt;
  ///                   
  ///         &lt;!-- 
  ///         Add a WmtsLayer. The use of a ProxyUrl is needed in addition to the regular Url property because the 
  ///         WMTS service is not hosted on a site that provides a cross domain policy file (clientaccesspolicy.xml
  ///         or crossdomain.xml). The InitializationFailed Event is used to notify the user in case the WMTS 
  ///         service is down. The TileLoading Event provides details about individual tiles in the WMTS service
  ///         that is necessary to get the input parameters (Level, Row, Column) of the WmtsLayer.GetTileUrl Method. 
  ///         --&gt;
  ///         &lt;esri:WmtsLayer ID="WMTS1" ServiceMode="KVP" Layer="usa:states"
  ///                         Url="http://v2.suite.opengeo.org/geoserver/gwc/service/wmts"
  ///                         ProxyUrl="http://servicesbeta3.esri.com/SilverlightDemos/ProxyPage/proxy.ashx"
  ///                         InitializationFailed="WmtsLayer_InitializationFailed"
  ///                         TileLoading="WmtsLayer_TileLoading"/&gt;
  ///       &lt;/esri:LayerCollection&gt;
  ///     &lt;/esri:Map.Layers&gt;
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- ListBox results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="33,512,0,0" Name="Label_ListBox1" 
  ///              VerticalAlignment="Top" Width="194" Content="ListBox Control:"/&gt;
  ///   &lt;ListBox Height="93" HorizontalAlignment="Left" Margin="33,526,0,0" Name="ListBox1" 
  ///            VerticalAlignment="Top" Width="194" SelectionChanged="ListBox1_SelectionChanged"/&gt;
  ///   
  ///   &lt;!-- TiledLayer.TileLoadEventsArgs. Level, Row, and Column. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,510,0,0" Name="Label_TileLoadEventArgs" 
  ///              VerticalAlignment="Top" Width="120" Content="TileLoadEventArgs:"/&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,542,0,0" Name="Label_Level" 
  ///              VerticalAlignment="Top" Width="48" Content="Level:"/&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,536,0,0" Name="TextBox_Level" 
  ///            VerticalAlignment="Top" Width="52" /&gt;
  ///   &lt;sdk:Label HorizontalAlignment="Left" Margin="239,564,0,208" Name="Label_Row" Width="48" Content="Row:"/&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,566,0,0" Name="TextBox_Row" 
  ///            VerticalAlignment="Top" Width="51" /&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,602,0,0" Name="Label_Column" 
  ///              VerticalAlignment="Top" Width="48" Content="Column:" /&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,596,0,0" Name="TextBox_Column" 
  ///            VerticalAlignment="Top" Width="52" /&gt;
  ///   
  ///   &lt;!-- WmtsLayer.GetTileUrl results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="32,631,0,0" Name="Label_GetTileUrl" 
  ///              VerticalAlignment="Top" Width="344" Content="WmtsLayer.GetTileUrl:"/&gt;
  ///   &lt;TextBox Height="124" HorizontalAlignment="Left" Margin="32,648,0,0" Name="TextBox_GetTileUrl" 
  ///            VerticalAlignment="Top" Width="344" TextWrapping="Wrap"/&gt;
  ///   
  ///   &lt;!-- Image Control results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="384,508,0,0" Name="Label_ImageControl1" 
  ///              VerticalAlignment="Top" Width="198" Content="Image Control:"/&gt;
  ///   &lt;Image Height="250" HorizontalAlignment="Left" Margin="382,522,0,0" Name="Image1" 
  ///          Stretch="Fill" VerticalAlignment="Top" Width="250" /&gt;
  ///   
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void WmtsLayer_InitializationFailed(object sender, System.EventArgs e)
  /// {
  ///   // Notify the user if there is a failure with the WMTS service.
  ///   ESRI.ArcGIS.Client.Layer aLayer = (ESRI.ArcGIS.Client.Layer)sender;
  ///   MessageBox.Show(aLayer.InitializationFailure.Message);
  /// }
  /// 
  /// private void WmtsLayer_TileLoading(object sender, ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs e)
  /// {
  ///   // This Event will fire for each tile that is loaded in the Map Control. For instance, if it takes 4 tiled images
  ///   // to render the Map Control completely, then this Event will fire 4 times. As you Zoom In or Pan around to other
  ///   // geographic areas in the Map, this Event will continue to fire until all of the tiles have been processed. 
  ///   
  ///   // The e argument of the Event returns a TileLoadEventArgs object.
  ///   ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs myTileLoadEventArgs = e;
  ///   
  ///   // Get the Tile's Level, Row, and Column Properties
  ///   int myLevel = myTileLoadEventArgs.Level;
  ///   int myRow = myTileLoadEventArgs.Row;
  ///   int myColumn = myTileLoadEventArgs.Column;
  ///   
  ///   // Generate a string that is comma delimited with the Level, Row, and Column values and add them to a Listbox.
  ///   string myString = myLevel.ToString() + "," + myRow.ToString() + "," + myColumn.ToString();
  ///   
  ///   // Do not add any duplicates.
  ///   if (! (ListBox1.Items.Contains(myString)))
  ///   {
  ///     ListBox1.Items.Add(myString);
  ///   }
  /// }
  /// 
  /// private void ListBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  /// {
  ///   // Get the SelectedItem from the Listbox and parse out the Level, Row, and Column arguments necessary to 
  ///   // obtain the Url for a specific tile.
  ///   string theConcatenatedString = ListBox1.SelectedItem;
  ///   string[] theParts = theConcatenatedString.Split(',');
  ///   int theLevel = Convert.ToInt32(theParts[0]);
  ///   int theRow = Convert.ToInt32(theParts[1]);
  ///   int theColumn = Convert.ToInt32(theParts[2]);
  ///   
  ///   // Update the Level, Row, and Column information in the TextBoxes for ease of viewing.
  ///   TextBox_Level.Text = theLevel.ToString();
  ///   TextBox_Row.Text = theRow.ToString();
  ///   TextBox_Column.Text = theColumn.ToString();
  ///   
  ///   // Get the WmtsLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer theWMTSLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer)Map1.Layers["WMTS1"];
  ///   
  ///   // Obtain a specific tile Url from the WmtsLayer using the three arguments for the GetTileUrl Method.
  ///   string theGetTileUrl = theWMTSLayer.GetTileUrl(theLevel, theRow, theColumn);
  ///   TextBox_GetTileUrl.Text = theGetTileUrl;
  ///   
  ///   // Only process valid restults. 
  ///   if (theGetTileUrl != null)
  ///   {
  ///     // Set the specific tile's Url as the Image's Source. 
  ///     Uri myUri = new Uri(theGetTileUrl);
  ///     Image1.Source = new Imaging.BitmapImage(myUri);
  ///   }
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub WmtsLayer_InitializationFailed(ByVal sender As System.Object, ByVal e As System.EventArgs)
  ///   
  ///   ' Notify the user if there is a failure with the WMTS service.
  ///   Dim aLayer As ESRI.ArcGIS.Client.Layer = sender
  ///   MessageBox.Show(aLayer.InitializationFailure.Message)
  ///   
  /// End Sub
  ///   
  /// Private Sub WmtsLayer_TileLoading(ByVal sender As System.Object, ByVal e As ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs)
  ///   
  ///   ' This Event will fire for each tile that is loaded in the Map Control. For instance, if it takes 4 tiled images
  ///   ' to render the Map Control completely, then this Event will fire 4 times. As you Zoom In or Pan around to other
  ///   ' geographic areas in the Map, this Event will continue to fire until all of the tiles have been processed. 
  ///   
  ///   ' The e argument of the Event returns a TileLoadEventArgs object.
  ///   Dim myTileLoadEventArgs As ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs = e
  ///   
  ///   ' Get the Tile's Level, Row, and Column Properties
  ///   Dim myLevel As Integer = myTileLoadEventArgs.Level
  ///   Dim myRow As Integer = myTileLoadEventArgs.Row
  ///   Dim myColumn As Integer = myTileLoadEventArgs.Column
  ///   
  ///   ' Generate a string that is comma delimited with the Level, Row, and Column values and add them to a Listbox.
  ///   Dim myString As String = myLevel.ToString + "," + myRow.ToString + "," + myColumn.ToString
  ///   
  ///   ' Do not add any duplicates.
  ///   If Not ListBox1.Items.Contains(myString) Then
  ///     ListBox1.Items.Add(myString)
  ///   End If
  ///   
  /// End Sub
  ///   
  /// Private Sub ListBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
  ///   
  ///   ' Get the SelectedItem from the Listbox and parse out the Level, Row, and Column arguments necessary to 
  ///   ' obtain the Url for a specific tile.
  ///   Dim theConcatenatedString As String = ListBox1.SelectedItem
  ///   Dim theParts As String() = Split(theConcatenatedString, ",")
  ///   Dim theLevel As Integer = CInt(theParts(0))
  ///   Dim theRow As Integer = CInt(theParts(1))
  ///   Dim theColumn As Integer = CInt(theParts(2))
  ///   
  ///   ' Update the Level, Row, and Column information in the TextBoxes for ease of viewing.
  ///   TextBox_Level.Text = theLevel.ToString
  ///   TextBox_Row.Text = theRow.ToString
  ///   TextBox_Column.Text = theColumn.ToString
  ///   
  ///   ' Get the WmtsLayer. 
  ///   Dim theWMTSLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.WmtsLayer = Map1.Layers("WMTS1")
  ///   
  ///   ' Obtain a specific tile Url from the WmtsLayer using the three arguments for the GetTileUrl Method.
  ///   Dim theGetTileUrl As String = theWMTSLayer.GetTileUrl(theLevel, theRow, theColumn)
  ///   TextBox_GetTileUrl.Text = theGetTileUrl
  ///   
  ///   ' Only process valid restults. 
  ///   If theGetTileUrl IsNot Nothing Then
  ///     
  ///     ' Set the specific tile's Url as the Image's Source. 
  ///     Dim myUri As New Uri(theGetTileUrl)
  ///     Image1.Source = New Imaging.BitmapImage(myUri)
  ///       
  ///   End If
  ///   
  /// End Sub
  /// </code>
  /// </example>
		/// <param name="level">Layer level</param>
		/// <param name="row">Tile row</param>
		/// <param name="col">Tile column</param>
		/// <returns>URL to the tile image</returns>
		public override string GetTileUrl(int level, int row, int col)
		{
			if (!IsInitialized)
				return null;

			string matrixIdentifier;
			if (!SkipGetCapabilities)
			{
				// Check the limits of the layer

				if (CurrentTileMatrixSet == null || CurrentLayer == null || CurrentTileMatrixSet.Matrices == null ||
					level < 0 || CurrentTileMatrixSet.Matrices.Count() <= level)
					return null;

				// Check that the row/col are in the limits for this level
				var matrix = CurrentTileMatrixSet.Matrices.ElementAt(level);
				if (matrix == null)
					return null;

				TileMatrixLimitsInfo tileMatrixLimits = CurrentTileMatrixSetLink == null ? null : CurrentTileMatrixSetLink.TileMatrixLimits.FirstOrDefault(tml => tml.TileMatrix == matrix.Identifier);

				int maxTileRow, minTileRow = 0, maxTileCol, minTileCol = 0;
				if (tileMatrixLimits != null)
				{
					// Use the specific limits for this layer
					maxTileRow = tileMatrixLimits.MaxTileRow;
					maxTileCol = tileMatrixLimits.MaxTileCol;
					minTileRow = tileMatrixLimits.MinTileRow;
					minTileCol = tileMatrixLimits.MinTileCol;

					// Hack : looks like a well known map server has a bug and add 1 to the minTileRow and maxTileRow (but ok with columns)
					// so substract 1 to the minTileRow else this tile is never displayed.
					// For server without this bug, this will end up by failing to load an inexisting tile but this is managed
					if (minTileRow > 0)
						minTileRow--;
				}
				else
				{
					if (CurrentTileMatrixSetLink != null && CurrentTileMatrixSetLink.TileMatrixLimits.Any()) // some limits are specified --> no limits for one level means no tile for this level
						return null;

					// Use the generic limits of the TileMatrix
					maxTileCol = matrix.MatrixWidth - 1;
					maxTileRow = matrix.MatrixHeight - 1;
				}

				if (row < minTileRow || row > maxTileRow || col < minTileCol || col > maxTileCol)
					return null; // out of limit (avoid useless GetTile request)

				matrixIdentifier = matrix.Identifier;
			}
			else
			{
				// Hack to generate the matrix identifier : would be better to extend the Lod class
				// !!!! there is no guarantee that the matrix identifier is based on the matrixset indentifier
				matrixIdentifier = string.Format("{0}:{1}", TileMatrixSet, level);
			}


			// Build the getTile url
			String url;
			if (ServiceMode == WmtsServiceMode.KVP)
			{
				url = string.IsNullOrEmpty(_httpGetKVPTileResource) ? Url : _httpGetKVPTileResource;

				url = CreateUrl(url, string.Format("service=WMTS&request=GetTile&version={0}", GetValidVersionNumber()));

				StringBuilder sb = new StringBuilder(url);
				sb.AppendFormat("&layer={0}", Layer);
				sb.AppendFormat("&style={0}", Style);
				sb.AppendFormat("&format={0}", ImageFormat);
				sb.AppendFormat("&tileMatrixSet={0}", TileMatrixSet);
				sb.AppendFormat("&tileMatrix={0}", matrixIdentifier);
				sb.AppendFormat("&tileRow={0}", row);
				sb.AppendFormat("&tileCol={0}", col);
				if (!string.IsNullOrEmpty(Token))
					sb.AppendFormat("&token={0}", Token);

				// Add the dimensional parameters
				if (CurrentDimensionValues != null)
				{
					foreach (var dimensionValue in CurrentDimensionValues)
						sb.AppendFormat("&{0}={1}", dimensionValue.Key, dimensionValue.Value);
				}

				url = sb.ToString();
			}
			else
			{
				// In Restful mode, a resource template Url must be provided
				string resourceTemplateUrl = CurrentLayer.ResourceUrls.Where(r => r.Format == ImageFormat && r.ResourceType == "tile").Select(r => r.Template).FirstOrDefault();
				if (string.IsNullOrEmpty(resourceTemplateUrl))
					return null;

				url = TemplateProcessor(resourceTemplateUrl, matrixIdentifier, row, col);
				if (!string.IsNullOrEmpty(Token))
					url = CreateUrl(url, string.Format("token={0}", Token));
			}
			
			return Utilities.PrefixProxy(ProxyUrl, url).ToString();
		}


		private string TemplateProcessor(string template, string matrixIdentifier, int row, int col)
		{
			string url = template;

			url = ReplaceParameter(url, "style", Style);
			url = ReplaceParameter(url, "TileMatrixSet", TileMatrixSet);
			url = ReplaceParameter(url, "TileMatrix", matrixIdentifier);
			url = ReplaceParameter(url, "TileRow", row.ToString());
			url = ReplaceParameter(url, "TileCol", col.ToString());
			// Note : the template is depending on the layer and on the format, so they are not parameters

			// Add the dimensional parameters
			if (CurrentDimensionValues != null)
			{
				foreach (var dimensionValue in CurrentDimensionValues)
					url = ReplaceParameter(url, dimensionValue.Key, dimensionValue.Value);
			}

			return url;
		}

		/// <summary>
		/// Replaces the parameter by its value. Make an insensitive replacement since I am not sure about the WMTS specifications.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static string ReplaceParameter(string input, string parameter, string value)
		{
			string output;
			string oldValue = "{" + parameter + "}";
			int index = input.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);

			if (index >= 0)
				output = input.Substring(0, index) + value + input.Substring(index + oldValue.Length);
			else
				output = input;

			return output;
		}

		#endregion


		#region Private properties managing the current state of the layer
		// private fields describing the current state
		private WmtsLayerInfo _currentLayer;
		private WmtsLayerInfo CurrentLayer
		{
			get { return _currentLayer; }
			set
			{
				if (_currentLayer != value)
				{
					_currentLayer = value;
					if (_currentLayer != null)
						Layer = _currentLayer.Identifier;

					// Changing the layer can change the format, the style, the tileMatrixSet and the dimensions values
					SetCurrentTileMatrixSet();
					SetCurrentFormat();
					SetCurrentStyle();
					SetCurrentDimensionValues();

					OnPropertyChanged("Description");
					OnPropertyChanged("Title");
				}
			}
		}

		private TileMatrixSetInfo CurrentTileMatrixSet { get; set; }
		private TileMatrixSetLinkInfo CurrentTileMatrixSetLink { get; set; }


		private void SetCurrentLayer()
		{
			if (LayerInfos == null)
			{
				// Layer not initialied yet or SkipGetCapabilities true
				CurrentLayer = null;
				return;
			}

			WmtsLayerInfo layerInfo = null;

			// First look for the layer with the specified name
			if (!string.IsNullOrEmpty(Layer))
				layerInfo = LayerInfos.FirstOrDefault(l => l.Identifier == Layer);

			// look for a layer being able to respect the TileMatrixSet
			if (layerInfo == null &&!string.IsNullOrEmpty(TileMatrixSet))
				layerInfo = LayerInfos.FirstOrDefault(l => l.TileMatrixSetLinks.Any(tmsl => tmsl.TileMatrixSet == TileMatrixSet));

			// finally take the first one
			if (layerInfo == null)
				layerInfo = LayerInfos.First();

			CurrentLayer = layerInfo;
		}

		private void SetCurrentTileMatrixSet()
		{
			if (TileMatrixSets == null || CurrentLayer == null)
			{
				// Layer not initialied yet or SkipGetCapabilities true
				CurrentTileMatrixSetLink = null;
				CurrentTileMatrixSet = null;
				return;
			}

			TileMatrixSetLinkInfo currentTileMatrixSetLink = null;
			TileMatrixSetInfo currentTileMatrixSet = null;

			if (!string.IsNullOrEmpty(TileMatrixSet)) // try first with forced TileMatrixSet
				currentTileMatrixSetLink = CurrentLayer.TileMatrixSetLinks.FirstOrDefault(tmsl => tmsl.TileMatrixSet == TileMatrixSet);

			if (currentTileMatrixSetLink == null) // take now the first one associated with the layer
				currentTileMatrixSetLink = CurrentLayer.TileMatrixSetLinks.FirstOrDefault();

			if (currentTileMatrixSetLink != null) // should always be the case
			{
				currentTileMatrixSet = TileMatrixSets.FirstOrDefault(tms => tms.Identifier == currentTileMatrixSetLink.TileMatrixSet);
			}


			bool needReinitialize = CurrentTileMatrixSetLink != currentTileMatrixSetLink ||
			                        CurrentTileMatrixSet != currentTileMatrixSet;

			CurrentTileMatrixSetLink = currentTileMatrixSetLink;
			CurrentTileMatrixSet = currentTileMatrixSet;
			if (currentTileMatrixSet != null)
				TileMatrixSet = currentTileMatrixSet.Identifier;

			if (needReinitialize)
			{
				// Reinitialize TileInfo/FullExtent/SpatialReference which may have changed
				SetTileInfo();
				SetFullExtent();
				SetSpatialReference();

				// Reinitialize the layer to take care of the new tiling scheme
				if (IsInitialized)
				{
					// Reinitialize the layer for taking care of this change
					IsInitialized = _initializing = false;
					InitializationFailure = null;
					Refresh();
					Initialize();
				}
				
			}

		}

		private void SetCurrentStyle()
		{
			if (CurrentLayer == null)
				return;

			string currentStyle;

			// Check that the specified style is associated to the layer
			if (!string.IsNullOrEmpty(Style) && CurrentLayer.Styles.Contains(Style))
			{
				currentStyle = Style;
			}
			else
			{
				// use default style (i.e. first style )
				currentStyle = CurrentLayer.Styles.FirstOrDefault();
			}
			Style = currentStyle;
		}

		private IDictionary<string, string> CurrentDimensionValues { get; set; }

		private void SetCurrentDimensionValues()
		{
			// Add the default values and the 'current' keywords to the values provided as DimensionValues
			if (CurrentLayer == null)
				return;

			var currentDimensionValues = new Dictionary<string, string>();

			if (CurrentLayer.DimensionInfos != null)
			{
				foreach (WmtsDimensionInfo dimensionInfo in CurrentLayer.DimensionInfos)
				{
					string value = null;
					WmtsDimensionValue wmtsDimensionValue = DimensionValues == null ? null : DimensionValues.FirstOrDefault(dv => dv.Identifier == dimensionInfo.Identifier);
					if (wmtsDimensionValue != null) // value specified
						value = wmtsDimensionValue.Value;
					else // Use default or current value or first value
					{
						if (!string.IsNullOrEmpty(dimensionInfo.Default))
							value = dimensionInfo.Default;
						else if (dimensionInfo.SupportsCurrent)
							value = "current"; // keywords
						else if (dimensionInfo.Values != null)
							value = dimensionInfo.Values.FirstOrDefault();
					}
					if (!string.IsNullOrEmpty(value))
						currentDimensionValues.Add(dimensionInfo.Identifier, value);
				}
			}
			CurrentDimensionValues = currentDimensionValues;
		}

		private void SetCurrentFormat()
		{
			if (CurrentLayer == null)
				return;

			string currentFormat;

			// Check that the specified Format is supported by the layer
			if (!string.IsNullOrEmpty(ImageFormat) && CurrentLayer.Formats.Contains(ImageFormat))
			{
				currentFormat = ImageFormat;
			}
			else
			{
				// Look for the most appropriate format
				currentFormat = BestFormat(CurrentLayer.Formats);
			}
			ImageFormat = currentFormat;
		}

		private static string BestFormat(IEnumerable<string> formats)
		{
			//Silverlight only supports png and jpg. Prefer PNG, then JPEG
			var format = formats.FirstOrDefault(f => f.StartsWith("image/png")) ??
						 formats.FirstOrDefault(f => f.StartsWith("image/jpg") || f.StartsWith("image/jpeg"));
#if !SILVERLIGHT
			//for WPF after PNG and JPEG, Prefer GIF, then any image, then whatever is supported
			format = format ??
					 formats.FirstOrDefault(f => f.StartsWith("image/gif")) ??
					 formats.FirstOrDefault(f => f.StartsWith("image/")) ??
					 formats.FirstOrDefault();
#endif
			return format;
		}

		private void SetSpatialReference()
		{
			if (CurrentTileMatrixSet == null)
				return;

			// Set SR of the layer
			var spatialReference = CurrentTileMatrixSet.SpatialReference;
			if (!SpatialReference.AreEqual(SpatialReference, spatialReference, false))
			{
				SpatialReference = spatialReference;
				OnPropertyChanged("SpatialReference");
			}
		}

		private void SetTileInfo()
		{
			if (CurrentTileMatrixSet == null)
				return;

			// Set the tiling scheme (from the first matrice since we assume all are supposed to be equivalent)
			var topLeftCorner = CurrentTileMatrixSet.Matrices.First().TopLeftCorner;
			var tileWidth = CurrentTileMatrixSet.Matrices.First().TileWidth;
			var tileHeight = CurrentTileMatrixSet.Matrices.First().TileHeight;

			// Check that all matrices use the same origin, tileWidth and tileHeight
			// else it's not supported
			if (CurrentTileMatrixSet.Matrices.Any(tm => tm.TileWidth != tileWidth || tm.TileHeight != tileHeight || !tm.TopLeftCorner.Equals(topLeftCorner)))
				throw new Exception("Tiling scheme not supported at this time");

			IEnumerable<TileMatrixInfo> matrices;
			if (CurrentTileMatrixSetLink != null && CurrentTileMatrixSetLink.TileMatrixLimits.Any())
			{
				// at least one limit is specified ==> the lods are coming from the TileMatrixLimits
				matrices = CurrentTileMatrixSetLink.TileMatrixLimits.Join(CurrentTileMatrixSet.Matrices, tmsl => tmsl.TileMatrix, m => m.Identifier, (tmsl, m) => m);
			}
			else
			{
				matrices = CurrentTileMatrixSet.Matrices;
			}

			IEnumerable<Lod> lods = matrices.OrderByDescending(m => m.ScaleDenominator)
											.Select(m => new Lod { Resolution = ConvertToResolution(m.ScaleDenominator, CurrentTileMatrixSet.SpatialReference) });

			base.TileInfo = new TileInfo
			{
				Origin = topLeftCorner,
				Lods = lods.ToArray(),
				Width = tileWidth,
				Height = tileHeight,
				SpatialReference = CurrentTileMatrixSet.SpatialReference
			};

		}

		///<summary>
		///</summary>
		internal new TileInfo TileInfo // TODO :waiting for decision concerning SkipGetCapabilities
		{
			get { return base.TileInfo; }
			set
			{
				if (TileInfo != value)
				{
					base.TileInfo = value;
					if (TileInfo != null && SkipGetCapabilities)
					{
						SpatialReference = TileInfo.SpatialReference;
						OnPropertyChanged("SpatialReference");
					}
					OnPropertyChanged("TileInfo");
				}
			}
		}


		//// Init Full Extent
		private void SetFullExtent()
		{
			if (CurrentTileMatrixSet == null || CurrentLayer == null)
				return;

			// The layer can provide an extent (optional)
			// The tileMatrixset can also provide an extent (optional as well)
			Envelope extent = CurrentLayer.Extent ?? CurrentTileMatrixSet.Extent;

			if (extent == null)
			{
				// Extent is not provided by layer nor by matrixset ==> calculate it from the matrices
				TileMatrixInfo matrix;
				double maxTileRow , minTileRow, maxTileCol, minTileCol; // double needed

				if (CurrentTileMatrixSetLink != null && CurrentTileMatrixSetLink.TileMatrixLimits.Any())
				{
					// Calculate from the matrix limits which should be more ajusted to the full extent than the global matrixset
					var tileMatrixLimits = CurrentTileMatrixSetLink.TileMatrixLimits.Last();
					matrix = CurrentTileMatrixSet.Matrices.FirstOrDefault(m => m.Identifier == tileMatrixLimits.TileMatrix);

					maxTileRow = tileMatrixLimits.MaxTileRow;
					maxTileCol = tileMatrixLimits.MaxTileCol;
					minTileRow = tileMatrixLimits.MinTileRow;
					minTileCol = tileMatrixLimits.MinTileCol;

					// Hack : looks like a well known map server has a bug and add 1 to the minTileRow and maxTileRow (but ok with columns)
					// so substract 1 to the minTileRow
					if (minTileRow > 0)
						minTileRow--;
				}
				else
				{
					matrix = CurrentTileMatrixSet.Matrices.Last();
					maxTileCol = matrix.MatrixWidth - 1;
					maxTileRow = matrix.MatrixHeight - 1;
					minTileCol = minTileRow = 0;
				}

				double resolution = ConvertToResolution(matrix.ScaleDenominator, CurrentTileMatrixSet.SpatialReference);
				var topLeft = matrix.TopLeftCorner;

				double xmin = topLeft.X + matrix.TileWidth * minTileCol * resolution;
				double xmax = topLeft.X + matrix.TileWidth *  (maxTileCol + 1) * resolution;
				double ymin = topLeft.Y - matrix.TileHeight * (maxTileRow + 1) * resolution;
				double ymax = topLeft.Y - matrix.TileHeight * minTileRow * resolution;

				extent = new Envelope(xmin, ymin, xmax, ymax);
			}

			extent.SpatialReference = CurrentTileMatrixSet.SpatialReference;
			FullExtent = extent;
			OnPropertyChanged("FullExtent");
		}
		#endregion

		#region Private methods

		private static readonly int[,] _latLongCrsRanges = new[,]
		                                                   	{
		                                                   		{4001, 4999},
		                                                   		{2044, 2045}, {2081, 2083}, {2085, 2086}, {2093, 2093},
		                                                   		{2096, 2098}, {2105, 2132}, {2169, 2170}, {2176, 2180},
		                                                   		{2193, 2193}, {2200, 2200}, {2206, 2212}, {2319, 2319},
		                                                   		{2320, 2462}, {2523, 2549}, {2551, 2735}, {2738, 2758},
		                                                   		{2935, 2941}, {2953, 2953}, {3006, 3030}, {3034, 3035},
		                                                   		{3058, 3059}, {3068, 3068}, {3114, 3118}, {3126, 3138},
		                                                   		{3300, 3301}, {3328, 3335}, {3346, 3346}, {3350, 3352},
		                                                   		{3366, 3366}, {3416, 3416}, {20004, 20032}, {20064, 20092},
		                                                   		{21413, 21423}, {21473, 21483}, {21896, 21899}, {22171, 22177},
		                                                   		{22181, 22187}, {22191, 22197}, {25884, 25884}, {27205, 27232},
		                                                   		{27391, 27398}, {27492, 27492}, {28402, 28432}, {28462, 28492},
		                                                   		{30161, 30179}, {30800, 30800}, {31251, 31259}, {31275, 31279},
		                                                   		{31281, 31290}, {31466, 31700}
		                                                   	};
		
		private static bool UseLatLon(XElement crsElement)
		{
			bool useLatLon = false;
			string crs = crsElement.GetValue();
			if (crs != null && crs.Contains("EPSG:")) // else if SR is defined by CRS84, CRS83 or CRS27 --> use x,y
			{
				useLatLon = UseLatLon(crsElement.GetSpatialReference());
			}
			return useLatLon;
		}

		private static bool UseLatLon(SpatialReference spatialReference)
		{
			if (spatialReference == null)
				return false;

			int wkid = spatialReference.WKID;
			int length = _latLongCrsRanges.Length / 2;
			for (int count = 0; count < length; count++)
			{
				if (wkid >= _latLongCrsRanges[count, 0] && wkid <= _latLongCrsRanges[count, 1])
					return true;
			}
			return false;
		}

		private const double INCHES_PER_METER = 39.37;
		private const double PIXEL_SIZE = 0.00028; // WMTS specifications
		private const double DPI = 1.0/INCHES_PER_METER/PIXEL_SIZE; //  ~90.7
		private static double ConvertToResolution(double scaleDenominator, SpatialReference spatialReference)
		{
			// GetScale for a map resolution of 1 (i.e one map unit by pixel) gives the number of pixels by map unit of the specified SR
			double pixelsPerUnit = Geometry.Geometry.GetScale(1.0, spatialReference, DPI);
			return scaleDenominator / pixelsPerUnit;
		}

		private IEnumerable<TileMatrixSetInfo> TileMatrixSets { get; set; }

		#endregion

		#region XML Parsers
		private void ParseCapabilities(XDocument xDoc)
		{
			XElement root = xDoc.Root;
			if (root == null)
				return;

			string ns = root.Name.NamespaceName;
			XNamespace owsns = root.GetNamespaceOfPrefix("ows") ?? "http://www.opengis.net/ows/1.1";

			XAttribute att;
			att = root.Attribute("version");

			if (att != null)
				Version = att.Value;

			//Get Title from ServiceIdentification
			Title = root.XPathSelectElement("ServiceIdentification/Title", owsns).GetValue();

			// Get the URL for requesting the tiles in KVP mode
			_httpGetKVPTileResource = null;
			XElement httpElement = root.XPathSelectElement("OperationsMetadata/Operation[@name='GetTile']/DCP/HTTP", owsns);
			if (httpElement != null)
			{
				foreach(var e in httpElement.Elements(XName.Get("Get", owsns.NamespaceName)))
				{
					XElement val = e.XPathSelectElement("Constraint/AllowedValues/Value", owsns);
					if (val != null && val.GetValue() == "KVP")
					{
						_httpGetKVPTileResource = e.GetAttributeValue(XName.Get("href", "http://www.w3.org/1999/xlink"));
						break;
					}
				}
			}

			var contents = root.Element(XName.Get("Contents", ns));
			if (contents != null)
			{
				// parse the layers
				LayerInfos = contents.Elements(XName.Get("Layer", ns)).Select(layerElement => ParseLayer(layerElement, owsns.NamespaceName, ns)).ToList();

				// parse the MatrixSet
				TileMatrixSets = contents.Elements(XName.Get("TileMatrixSet", ns)).Select(elt => ParseTileMatrixSet(elt, owsns.NamespaceName, ns)).ToList();
			}
		}

		private static WmtsLayerInfo ParseLayer(XElement layerElement, string owsnsname, string nsname)
		{
			return new WmtsLayerInfo
			{
				// Description
				Title = layerElement.Element(XName.Get("Title", owsnsname)).GetValue(),
				Abstract = layerElement.Element(XName.Get("Abstract", owsnsname)).GetValue(),

				// Dataset Description
				Identifier = layerElement.Element(XName.Get("Identifier", owsnsname)).GetValue(),
				Extent = ParseEnvelope(layerElement.Element(XName.Get("BoundingBox", owsnsname)), null, owsnsname),

				// Styles
				Styles = layerElement.Elements(XName.Get("Style", nsname))
				                     .Select(styleElement => ParseStyle(styleElement, owsnsname))
				                     .OrderByDescending(styleInfo => styleInfo.IsDefault)
				                     .Select(styleInfo => styleInfo.Identifier)
				                     .ToList(),

				// Formats
				Formats = layerElement.Elements(XName.Get("Format", nsname)).Select(formatElement => formatElement.GetValue()).ToList(),

				// TileMatrixSetLink
				TileMatrixSetLinks = layerElement.Elements(XName.Get("TileMatrixSetLink", nsname)).Select(element => ParseTileMatrixSetLink(element, nsname)).ToList(),

				// Dimensions
				DimensionInfos = layerElement.Elements(XName.Get("Dimension", nsname))
								.Select(element => ParseDimension(element, owsnsname, nsname))
								.ToList(),

				// Resource URLs
				ResourceUrls = layerElement.Elements(XName.Get("ResourceURL", nsname)).Select(ParseResourceUrl).ToList()
			};
		}

		private static StyleInfo ParseStyle(XElement styleElement, string owsnsname)
		{
			if (styleElement == null)
				return null;

			bool isDefault;
			Boolean.TryParse(styleElement.GetAttributeValue("isDefault"), out isDefault);

			return new StyleInfo
			{
				Identifier = styleElement.Element(XName.Get("Identifier", owsnsname)).GetValue(),
				IsDefault = isDefault
			};
		}

		private static TileMatrixSetLinkInfo ParseTileMatrixSetLink(XElement element, string nsname)
		{
			if (element == null)
				return null;

			XElement tileMatrixSetLimitsElement = element.Element(XName.Get("TileMatrixSetLimits", nsname));
			IEnumerable<TileMatrixLimitsInfo> tileMatrixLimits = tileMatrixSetLimitsElement == null
			                                                     	? Enumerable.Empty<TileMatrixLimitsInfo>()
			                                                     	: tileMatrixSetLimitsElement.Elements(XName.Get("TileMatrixLimits", nsname)).Select(elt => ParseTileMatrixLimits(elt, nsname));

			return new TileMatrixSetLinkInfo
			{
				TileMatrixSet = element.Element(XName.Get("TileMatrixSet", nsname)).GetValue(),
				TileMatrixLimits = tileMatrixLimits.ToList()
			};
		}

		private static TileMatrixLimitsInfo ParseTileMatrixLimits(XElement element, string nsname)
		{
			if (element == null)
				return null;

			return new TileMatrixLimitsInfo
			{
				TileMatrix = element.Element(XName.Get("TileMatrix", nsname)).GetValue(),
				MinTileRow = element.Element(XName.Get("MinTileRow", nsname)).GetIntValue(),
				MaxTileRow = element.Element(XName.Get("MaxTileRow", nsname)).GetIntValue(),
				MinTileCol = element.Element(XName.Get("MinTileCol", nsname)).GetIntValue(),
				MaxTileCol = element.Element(XName.Get("MaxTileCol", nsname)).GetIntValue()
			};
		}

		private static TileMatrixSetInfo ParseTileMatrixSet(XElement element, string owsnsname, string nsname)
		{
			var crsElement = element.Element(XName.Get("SupportedCRS", owsnsname));
			var tileMatrixSet = new TileMatrixSetInfo
			{
				Identifier = element.Element(XName.Get("Identifier", owsnsname)).GetValue(),
				Title = element.Element(XName.Get("Title", owsnsname)).GetValue(),
				SpatialReference = crsElement.GetSpatialReference(),
				Matrices = element.Elements(XName.Get("TileMatrix", nsname)).Select(elt => ParseTileMatrix(elt, owsnsname, nsname)).ToList()
			};
			tileMatrixSet.Extent = ParseEnvelope(element.Element(XName.Get("BoundingBox", owsnsname)), tileMatrixSet.SpatialReference, owsnsname);

			bool useLatLon = UseLatLon(crsElement);

			// Hack for 900913 : looks like sometimes it's inversed, sometimes not : waiting for more infos
			if (tileMatrixSet.SpatialReference.WKID == 3857)
			{
				useLatLon = tileMatrixSet.Matrices.First().TopLeftCorner.Y < -2E7;
			}

			// If Y coordinate of the topLeftCorner was specified before X, we have to reverse it
			if (useLatLon)
			{
				foreach (var matrix in tileMatrixSet.Matrices)
				{
					double x = matrix.TopLeftCorner.X;
					matrix.TopLeftCorner.X = matrix.TopLeftCorner.Y;
					matrix.TopLeftCorner.Y = x;
				}
			}

			return tileMatrixSet;
		}

		private static TileMatrixInfo ParseTileMatrix(XElement element, string owsnsname, string nsname)
		{
			return new TileMatrixInfo
			{
				Identifier = element.Element(XName.Get("Identifier", owsnsname)).GetValue(),
				ScaleDenominator = element.Element(XName.Get("ScaleDenominator", nsname)).GetDoubleValue(),
				TopLeftCorner = element.Element(XName.Get("TopLeftCorner", nsname)).GetMapPoint(),
				TileHeight = element.Element(XName.Get("TileHeight", nsname)).GetIntValue(),
				TileWidth = element.Element(XName.Get("TileWidth", nsname)).GetIntValue(),
				MatrixHeight = element.Element(XName.Get("MatrixHeight", nsname)).GetIntValue(),
				MatrixWidth = element.Element(XName.Get("MatrixWidth", nsname)).GetIntValue(),
			};
		}

		private static ResourceUrlInfo ParseResourceUrl(XElement element)
		{
			return new ResourceUrlInfo
			{
				Format = element.GetAttributeValue("format"),
				Template = element.GetAttributeValue("template"),
				ResourceType = element.GetAttributeValue("resourceType")
			};
		}

		private static Envelope ParseEnvelope(XElement element, SpatialReference sref, string nsname)
		{
			if (element == null)
				return null;

			var lowerCorner = element.Element(XName.Get("LowerCorner", nsname)).GetMapPoint();
			var upperCorner = element.Element(XName.Get("UpperCorner", nsname)).GetMapPoint();

			if (upperCorner == null || lowerCorner == null)
				return null;

			return new Envelope(lowerCorner, upperCorner) { SpatialReference = sref };
		}

		private static WmtsDimensionInfo ParseDimension(XElement element, string owsnsname, string nsname)
		{
			if (element == null)
				return null;

			return new WmtsDimensionInfo
			{
				Identifier = element.Element(XName.Get("Identifier", owsnsname)).GetValue(),
				Abstract = element.Element(XName.Get("Abstract", owsnsname)).GetValue(),
				Title = element.Element(XName.Get("Title", owsnsname)).GetValue(),
				Default = element.Element(XName.Get("Default", nsname)).GetValue(),
				SupportsCurrent = element.Element(XName.Get("Current", nsname)).GetBoolValue(),
				Values = element.Elements(XName.Get("Value", nsname)).Select(elt => elt.GetValue()).ToList()
			};
		}

		#endregion

		#region WmtsDimensionInfo Class
		/// <summary>
		/// WMTS Metadata about a particular dimension that the tiles of a layer are available.
		/// </summary>
		/// <seealso cref="WmtsLayerInfo.DimensionInfos"/>>
		public sealed class WmtsDimensionInfo
		{
			/// <summary>
			/// Gets the name of dimensional axis.
			/// </summary>
			public string Identifier { get; internal set; }

			/// <summary>
			/// Gets the title of this dimension.
			/// </summary>
			public string Title { get; internal set; }

			/// <summary>
			/// Gets the brief narrative description of this dimension.
			/// </summary>
			public string Abstract { get; internal set; }

			/// <summary>
			/// Gets the Default value that will be used if a tile request does not specify a value or uses the keyword 'default'.
			/// </summary>
			public string Default { get; internal set; }

			/// <summary>
			/// Gets a flag indicating whether that temporal data are normally kept current and that the request value of this dimension accepts the keyword 'current'.
			/// </summary>
			public Boolean  SupportsCurrent { get; internal set; }

			/// <summary>
			/// Gets the available value for this dimension..
			/// </summary>
			public IEnumerable<string> Values { get; internal set; } 
		}

		#endregion

		#region Internal Infos Classes

		internal sealed class StyleInfo
		{
			/// <summary>
			/// Gets the identifier of the style.
			/// </summary>
			/// <value>The identifier.</value>
			public string Identifier { get; internal set; }

			public bool IsDefault { get; internal set; }
		}

		internal sealed class TileMatrixSetInfo
		{
			public string Title { get; set; }
			public string Identifier { get; set; }

			public SpatialReference SpatialReference { get; set; }

			public Envelope Extent { get; set; }

			public IEnumerable<TileMatrixInfo> Matrices { get; set; }
		}

		internal sealed class TileMatrixInfo
		{
			public string Identifier { get; set; }

			public double ScaleDenominator { get; set; }

			public MapPoint TopLeftCorner { get; set; }

			public int TileWidth { get; set; }
			public int TileHeight { get; set; }
			public int MatrixWidth { get; set; }
			public int MatrixHeight { get; set; }
		}

		internal sealed class TileMatrixSetLinkInfo
		{
			public string TileMatrixSet { get; set; }

			public IEnumerable<TileMatrixLimitsInfo> TileMatrixLimits { get; set; }
		}

		internal sealed class TileMatrixLimitsInfo
		{
			public string TileMatrix { get; set; }
			public int MinTileRow { get; set; }
			public int MaxTileRow { get; set; }
			public int MinTileCol { get; set; }
			public int MaxTileCol { get; set; }
		} 

		internal sealed class ResourceUrlInfo
		{
			public string Template { get; set; }
			public string ResourceType {get; set;}
			public string Format { get; set; }
		}
		#endregion

	}


	/// <summary>
	/// Represents one dimensional value used by the WMTS layer for requesting the tiles.
	/// </summary>
	/// <para>Examples of dimensions are Time, Elevation and Band but the service can define any other dimension property that exists in the multidimensional layer collection being served.</para>
	/// <seealso cref="WmtsLayer.DimensionValues"/>
	/// <seealso cref="WmtsDimensionValueCollection"/>
	public sealed class WmtsDimensionValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WmtsDimensionValue"/> class.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <param name="value">The value.</param>
		public WmtsDimensionValue(string identifier, string value)
		{
			Identifier = identifier;
			Value = value;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="WmtsDimensionValue"/> class.
		/// </summary>
		public WmtsDimensionValue()
		{
		}

		/// <summary>
		/// Gets or sets the dimension identifier.
		/// </summary>
		/// <value>
		/// The dimension identifier.
		/// </value>
		public string Identifier { get; set; }

		/// <summary>
		/// Gets or sets the value for the dimension.
		/// </summary>
		/// <value>
		/// The dimension value.
		/// </value>
		public string Value { get; set; }
	}

	/// <summary>
	/// Holds a collection of <see cref="WmtsDimensionValue"/>.
	/// </summary>
	/// <seealso cref="WmtsLayer.DimensionValues"/>
	public class WmtsDimensionValueCollection : ObservableCollection<WmtsDimensionValue>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WmtsDimensionValueCollection"/> class.
		/// </summary>
		public WmtsDimensionValueCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WmtsDimensionValueCollection"/> class.
		/// </summary>
		/// <param name="dimensions">The dimension values.</param>
		public WmtsDimensionValueCollection(IEnumerable<WmtsDimensionValue> dimensions)
			: base(dimensions)
		{
		}
	}

	#region internal static class XmlExtension
	/// <summary>
	/// Helper XML extensions
	/// </summary>
	internal static class XmlExtension
	{
		/// <summary>
		/// Very simplified version of querying an element by XPath.
		/// Note : System.Xml.Xpath extensions doesn't exist for WP7
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="path">The path.</param>
		/// <param name="ns">The ns.</param>
		/// <returns></returns>
		public static XElement XPathSelectElement(this XElement element, string path, XNamespace ns)
		{
			foreach (var name in path.Split('/'))
			{
				if (element == null)
					break;

				if (name.Contains("[@"))
				{
					// filter on attribute name
					var filter = name.Split(new[] { "[@" }, StringSplitOptions.None).ElementAt(1);
					var eltName = name.Split(new[] { "[@" }, StringSplitOptions.None).First();
					var attName = filter.Split('=').First();
					var value = filter.Split('=').ElementAt(1).Trim(new[] { '\'', ']' });

					element = element.Elements(XName.Get(eltName, ns.NamespaceName)).Where(elt => elt.GetAttributeValue(XName.Get(attName)) == value).FirstOrDefault();
				}
				else
					element = element.Element(XName.Get(name, ns.NamespaceName));
			}
			return element;
		}

		public static string GetAttributeValue(this XElement element, XName attName)
		{
			if (element == null)
				return null;
			var att = element.Attribute(attName);
			return att == null ? null : att.Value;
		}

		public static string GetValue(this XElement element)
		{
			if (element == null)
				return null;
			return element.Value;
		}

		public static MapPoint GetMapPoint(this XElement element)
		{
			if (element == null)
				return null;

			var points = element.GetValue().Split(' ');

			if (points.Count() != 2)
				return null;

			return new MapPoint(double.Parse(points.First(), CultureInfo.InvariantCulture),
								double.Parse(points.Last(), CultureInfo.InvariantCulture));
		}

		public static double GetDoubleValue(this XElement element)
		{
			double ret = 0.0;
			if (element != null)
				double.TryParse(element.GetValue(), NumberStyles.Any, CultureInfo.InvariantCulture, out ret);

			return ret;
		}

		public static int GetIntValue(this XElement element)
		{
			int ret = 0;
			if (element != null)
				int.TryParse(element.GetValue(), NumberStyles.None, CultureInfo.InvariantCulture, out ret);

			return ret;
		}

		public static bool GetBoolValue(this XElement element)
		{
			string val = GetValue(element);
			return !string.IsNullOrEmpty(val) && (val.Equals("true", StringComparison.OrdinalIgnoreCase) || val == "1");
		}

		public static SpatialReference GetSpatialReference(this XElement element)
		{
			int WKID = 0;
			string CRS = element.GetValue();
			if (CRS != null)
			{
				if (CRS.Contains("EPSG:"))
				{
					int.TryParse(CRS.Split(':').Last(), NumberStyles.None, CultureInfo.InvariantCulture, out WKID);
				} else if (CRS.EndsWith("CRS84")) // WGS84
					WKID = 4326;
				else if (CRS.EndsWith("CRS83")) // NAD83
					WKID = 4269;
				else if (CRS.EndsWith("CRS27")) // NAD27
					WKID = 4267;

				if (WKID == 900913)
					WKID = 3857; // change google ID to the normalized one (EPSG:900913 is not supposed to exist)

				if (WKID == 0)
					throw new Exception("Unrecognized SR : " + CRS);
			}

			return WKID > 0 ? new SpatialReference(WKID) : null;
		}

	}
	#endregion
}



