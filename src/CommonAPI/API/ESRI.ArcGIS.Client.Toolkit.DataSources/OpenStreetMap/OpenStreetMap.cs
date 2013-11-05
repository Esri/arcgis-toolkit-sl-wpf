// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Collections.Generic;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{

/// <summary>
/// A layer that conforms to the <a href="http://www.openstreetmap.org">OpenStreetMap</a> standard. An 
/// OpenStreetMapLayer is a cached service that accesses pre-created tiles from a cache on a server's hard 
/// drive instead of dynamically rendering images.
/// </summary>
/// <remarks>
/// <para>
/// <a href="http://www.openstreetmap.org">OpenStreetMap</a> is an organization that provides free world-wide 
/// tiled based map services to the public. OpenStreetMapLayers are specialized layers that consume web 
/// services using the OpenStreetMap specification. A tiled map service means that map images are pre-created 
/// on a server to improve drawing performance in a client application. Because the images are pre-created 
/// cached tiles, re-projection on-the-fly to a different 
/// <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> is not possible.
/// </para>
/// <para>
/// By generating pre-created cached tiles and storing them in an optimized file structure on a server using 
/// the OpenStreetMap specification, client applications can see improved draw times of geographic phenomena. 
/// This performance increase comes as a trade-off of not having the flexibility to dynamically change the 
/// drawing of geographic phenomena on-the-fly. This means that an OpenStreetMapLayer does not have Properties 
/// like dynamic map service layers such as the 
/// <see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer">ArcGISDynamicMapServiceLayer</see> or a 
/// <see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see> have for things such as: LayerDefinition, 
/// Where, TimeExtent, and/or VisibleLayers which restrict which features are returned.
/// </para>
/// <para>
/// A call to the OpenStreetMapLayer is Asynchronous. As a result, this means that obtaining information 
/// (i.e. get/Read) for the various Properties of an OpenStreetMapLayer should occur in the 
/// <see cref="ESRI.ArcGIS.Client.Layer.Initialized">Initialized</see> Event or any time after the 
/// Initialized Event occurs. This ensures that information retrieved about the OpenStreetMapLayer has been 
/// obtained after a complete round trip from the server. Do not be tempted to try and access 
/// OpenStreetMapLayer Property information from generic application Events like: 
/// <ESRISILVERLIGHT>MainPage.Loaded</ESRISILVERLIGHT>
/// <ESRIWPF>MainWindows.Loaded</ESRIWPF>
/// <ESRIWINPHONE>MainPage.Loaded</ESRIWINPHONE>
/// or the Constructor, etc. as the OpenStreetMapLayer has not been Initialized and erroneous information 
/// will be returned. Likewise, OpenStreetMapLayer Methods should not be invoked until after the 
/// OpenStreetMapLayer Initialized Event has fired or from within the Initialized Event to avoid erroneous 
/// results.
/// </para>
/// <para>
/// It is highly recommended to use the OpenStreetMapLayer 
/// <see cref="ESRI.ArcGIS.Client.Layer.InitializationFailed">InitializationFailed</see> Event to test for 
/// valid data being returned from the Server. Some common reasons for an OpenStreetMapLayer failing to 
/// initialize include the server being down or an incorrect Url was specified in the 
/// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServers">OpenStreetMapLayer.TileServers</see> 
/// Property. If proper error handling is not done in the OpenStreetMapLayer InitializationFailed Event, an 
/// 'Unhandled Exception' error message will be thrown by Visual Studio causing undesirable application 
/// termination.
/// </para>
/// <para>
/// It is only required to create a new instance of an OpenStreetMapLayer and then add it to the 
/// <see cref="ESRI.ArcGIS.Client.Map.Layers">Map.Layers</see> Property to display a default 
/// OpenStreetMapLayer. The reason for this is that the internals of the OpenStreetMapLayer constructor 
/// automatically uses an internal Url to a web service provided by the OpenStreetMap organization. So 
/// the following is an XAML example of all that is needed to create a default OpenStreetMapLayer:
/// </para>
/// <code language="XAML">
/// &lt;esri:Map x:Name="MyMap"&gt;
///   &lt;esri:OpenStreetMapLayer /&gt;
/// &lt;/esri:Map&gt;
/// </code>
/// <para>
/// The OpenStreetMap organization hosts several types of maps that can be used as OpenStreetMapLayer's. 
/// To change which type of map is used, specify the 
/// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.Style">OpenStreetMapLayer.Style</see> 
/// Property to any one of several 
/// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.MapStyle">OpenStreetMapLayer.MapStyle</see> 
/// Enumerations. The default OpenStreetMap.MapStyle Property is <b>OpenStreetMapLayer.MapStyle.Mapnik</b> meaning 
/// that if an OpenStreetMapLayer.Style is not specified in constructing an OpenStreetMapLayer, the 
/// <b>OpenStreetMapLayer.MapStyle.Mapnik</b> style will be used by default. The following is an XAML example of 
/// specifying a specific OpenStreetMapLayer.Style when defining a new OpenStreetMapLayer:
/// </para>
/// <code language="XAML">
/// &lt;esri:Map x:Name="MyMap"&gt;
///  &lt;esri:OpenStreetMapLayer Style=”CycleMap”/&gt;
/// &lt;/esri:Map&gt;
/// </code>
/// <para>
/// If it is not desired to use the map services provided directly by the OpenStreetMap organization or if you 
/// discover that additional map services are provided for which Esri has not provided an explicit 
/// OpenStreetMapLayer.Style, developers can explicitly provide their own Url's for an OpenStreetMapLayer 
/// using the 
/// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServers">OpenStreetMapLayer.TileServers</see> 
/// Property. When using the OpenStreetMapLayer.TileServers Property, the OpenStreetMapLayer.Style Property is 
/// ignored. The 
/// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList">OpenStreetMapLayer.TileServerList</see> 
/// Class is a List Collection of Strings that are the Url's to various OpenStreetMap map based servers. The 
/// following is an XAML example of specifying specific Urls using the OpenStreetMapLayer.TileServers Property 
/// when defining a new OpenStreetMapLayer:
/// </para>
/// <code language="XAML">
/// &lt;esri:Map x:Name="MyMap"&gt;
///   &lt;esri:OpenStreetMapLayer ID="osmLayer"&gt;
///     &lt;esri:OpenStreetMapLayer.TileServers&gt;
///       &lt;sys:String&gt;http://otile1.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
///       &lt;sys:String&gt;http://otile2.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
///       &lt;sys:String&gt;http://otile3.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
///     &lt;/esri:OpenStreetMapLayer.TileServers&gt;
///   &lt;/esri:OpenStreetMapLayer&gt;
/// &lt;/esri:Map&gt;
/// </code>
/// <para>
/// It is important to understand that only one Url is needed in the OpenStreetMapLayer.TileServerList. If multiple 
/// Urls are included in the OpenStreetMapLayer.TileServerList, all of the map servers should be serving up the same 
/// base data. The reason for having the ability to specify multiple Urls in the OpenStreetMapLayer.TileServerList 
/// is to improve performance by load balancing the requests the client application uses across multiple servers. 
/// If different OpenStreetMap based map services are specified in the OpenStreetMapLayer.TileServerList, the tiles 
/// that are placed together in the Esri Map control will yield unexpected results. For example, assume that three 
/// different OpenStreetMap map based services are used for the the OpenStreetMapLayer.TileServers Property in the 
/// following XAML example code:
/// </para>
/// <code language="XAML">
/// &lt;esri:Map x:Name="MyMap"&gt;
///   &lt;esri:OpenStreetMapLayer ID="osmLayer"&gt;
///     &lt;esri:OpenStreetMapLayer.TileServers&gt;
///       &lt;sys:String&gt;http://otile1.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
///       &lt;sys:String&gt;http://a.tile.openstreetmap.org&lt;/sys:String&gt;
///       &lt;sys:String&gt;http://a.tile.opencyclemap.org/cycle&lt;/sys:String&gt;
///     &lt;/esri:OpenStreetMapLayer.TileServers&gt;
///   &lt;/esri:OpenStreetMapLayer&gt;
/// &lt;/esri:Map&gt;   
/// </code>
/// <para>
/// The following is a screen shot of the previous XAML code fragment showing the application's undesirable 
/// results appearing if different map based services were used in the the OpenStreetMap.TileServers Property:
/// </para>
/// <para>
/// <img border="0" alt="Using different base map services as the OpenStreetMap.TileServer Property yields strange results." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.OpenStreetMap.TileServersStrange.png"/>
/// </para>
/// <para>
/// Whenever an OpenStreetMapLayer is used in a production application based on services from the OpenStreetMap 
/// organization, it is required by their license agreement to provide the 
/// <a href="http://www.openstreetmap.org/copyright">appropriate credit</a> for using their data. The credit 
/// information for maps provided by the OpenStreetMap organization is stored in the 
/// <see cref="WebTiledLayer.CopyrightText"/> 
/// Property. To display the credit information in your application is most easily accomplished by adding an Esri 
/// <see cref="T:ESRI.ArcGIS.Client.Toolkit.Attribution">Attribution</see> control to your client application 
/// and binding the <see cref="M:ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see> Property 
/// to the OpenStreetMapLayer. The following is an XAML example of how to accomplish this:
/// </para>
/// <code language="XAML">
/// &lt;esri:Map x:Name="MyMap"&gt;
///   &lt;esri:OpenStreetMapLayer Style="Mapnik" /&gt;
/// &lt;/esri:Map&gt;
/// &lt;esri:Attribution Layers="{Binding ElementName=MyMap, Path=Layers}" /&gt;
/// </code>
/// <para>
/// OpenStreetMap is released under the Create Commons "Attribution-Share Alike 2.0 Generic" license.
/// </para>
/// </remarks>
/// <example>
/// <para>
/// <b>How to use:</b>
/// </para>
/// <para>
/// Click the Button to add an OpenStreetMapLayer to the Map (it will be added via code-behind). The credit information 
/// about the dataset will displayed in the Attribution Control.
/// </para>
/// <para>
/// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
/// the functionality.
/// </para>
/// <para>
/// The following screen shot corresponds to the code example in this page.
/// </para>
/// <para>
/// <img border="0" alt="Example of loading an OpenStreetMapLayer via code-behind." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.OpenStreetMap.png"/>
/// </para>
/// <code title="Example XAML1" description="" lang="XAML">
/// &lt;Grid x:Name="LayoutRoot" &gt;
///   
///   &lt;!-- Add a Map Control to the application. --&gt;
///   &lt;esri:Map x:Name="Map1" WrapAround="True" HorizontalAlignment="Left" VerticalAlignment="Top" 
///         Margin="0,156,0,0" Height="350" Width="550" /&gt;
///   
///   &lt;!-- Add an Attribution Control. --&gt;
///   &lt;esri:Attribution x:Name="Attribution1" Width="550" Height="50" HorizontalAlignment="Left" Margin="0,512,0,38" /&gt;
///   
///   &lt;!-- Add a Button that will allow the user to add an OpenStreetMapLayer via code-behind. --&gt;
///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,128,0,0"  VerticalAlignment="Top" 
///           Width="550" Content="Add an OpenStreetMapLayer (via code-behind)."
///           Click="Button1_Click" /&gt;
///   
///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
///   &lt;TextBlock Height="122" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="572" 
///              TextWrapping="Wrap" Text="Click the Button to add an OpenStreetMapLayer to the Map (it will be added 
///              via code-behind). The credit information about the dataset will displayed in the Attribution Control." /&gt;
///   
/// &lt;/Grid&gt;
/// </code>
/// <code title="Example CS1" description="" lang="CS">
/// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
/// {
///   // Create a new instance of an OpenStreetMapLayer.
///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer();
///   
///   // Set the OpenStreetMapLayer.Style to that of MapStyle.CycleMap
///   myOpenStreetMapLayer.Style = ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.MapStyle.CycleMap;
///   
///   // Wire up an Initialized Event handler for the OpenStreetMapLayer.
///   myOpenStreetMapLayer.Initialized += myOpenStreetMapLayer_Initialized;
///   
///   // Add the OpenStreetMapLayer to the Map's Layer Collection. This will cause the OpenStreetMapLayer.Initialized Event to fire.
///   Map1.Layers.Add(myOpenStreetMapLayer);
/// }
/// 
/// private void myOpenStreetMapLayer_Initialized(object sender, EventArgs e)
/// {
///   // Get the OpenStreetMapLayer.
///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer)sender;
///   
///   // Display the OpenStreetMapLayer credit information via the ESRI Attribution Control.
///   Attribution1.Layers = Map1.Layers;
/// }
/// </code>
/// <code title="Example VB1" description="" lang="VB.NET">
/// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
///   
///   ' Create a new instance of an OpenStreetMapLayer.
///   Dim myOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer
///   
///   ' Set the OpenStreetMapLayer.Style to that of MapStyle.CycleMap
///   myOpenStreetMapLayer.Style = ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.MapStyle.CycleMap
///   
///   ' Wire up an Initialized Event handler for the OpenStreetMapLayer.
///   AddHandler myOpenStreetMapLayer.Initialized, AddressOf myOpenStreetMapLayer_Initialized
///   
///   ' Add the OpenStreetMapLayer to the Map's Layer Collection. This will cause the OpenStreetMapLayer.Initialized Event to fire.
///   Map1.Layers.Add(myOpenStreetMapLayer)
///   
/// End Sub
/// 
/// Private Sub myOpenStreetMapLayer_Initialized(sender As Object, e As EventArgs)
///   
///   ' Get the OpenStreetMapLayer.
///   Dim myOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = sender
///   
///   ' Display the OpenStreetMapLayer credit information via the ESRI Attribution Control.
///   Attribution1.Layers = Map1.Layers
///   
/// End Sub
/// </code>
/// </example>	
	public class OpenStreetMapLayer : WebTiledLayer
	{
		/// <summary>Available subdomains for tiles.</summary>
		private static readonly string[] subDomains = { "a", "b", "c" };
		/// <summary>Base URL used in GetTileUrl.</summary>
		private static readonly string[] baseUrl =
		{
			"http://{subDomain}.tile.openstreetmap.org/{level}/{col}/{row}.png",
			"http://{subDomain}.tile.opencyclemap.org/cycle/{level}/{col}/{row}.png",
			"http://{subDomain}.tile.cloudmade.com/fd093e52f0965d46bb1c6c6281022199/3/256/{level}/{col}/{row}.png"
		};

		private TileServerList _tileServers; // for being able to access it from a background thread

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenStreetMapLayer"/> class.
		/// </summary>
		public OpenStreetMapLayer()
		{            
			SubDomains = subDomains;
			Style = MapStyle.Mapnik;
			TemplateUrl = baseUrl[(int)Style];
#pragma warning disable 0618
			TileServers = new TileServerList();
#pragma warning restore 0618
			CopyrightText = "Map data © OpenStreetMap contributors, CC-BY-SA";
		}
		
		/// <summary>
		/// Returns a URL to the specified tile in an OpenStreetMapLayer.
		/// </summary>
		/// <param name="level">Layer level</param>
		/// <param name="row">Tile row</param>
		/// <param name="col">Tile column</param>
		/// <returns>URL to the tile image</returns>
  /// <remarks>
  /// <para>
  /// An OpenStreetMapLayer is made up of multiple tiles (or images) that are automatically put together in 
  /// a mosaic for display in a Map Control. The tiles are pre-generated on a web server and can 
  /// be accessed individually via a URL. In order to access the URL for a specific tile it is 
  /// required to know the Level, Row, and Column information. 
  /// </para>
  /// <para>
  /// A programmatic way to determine the various Level, Row, and Column information can be obtained by writing some 
  /// code-behind logic in the 
  /// <see cref="ESRI.ArcGIS.Client.TiledLayer.TileLoading">OpenStreetMapLayer.TileLoading</see> Event 
  /// (see the code example in this document).
  /// </para>  
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// After the OpenStreetMapLayer loads in the Map Control, the ListBox will be populated with all the 
  /// combinations of 'Level, Row, and Column' tiles that make up the initial extent of the OpenStreetMapLayer 
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
  /// <img border="0" alt="Displaying individual tile images and their Url values for an OpenStreetMapLayer." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.OpenStreetMap.GetTileIUrl.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="78" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="640" 
  ///     TextWrapping="Wrap" Text="After the OpenStreetMapLayer loads in the Map Control, the ListBox will be 
  ///     populated with all the combinations of 'Level, Row, and Column' tiles that make up the initial extent of the 
  ///     OpenStreetMapLayer image service. Click on any of the combinations in the Listbox and that particular 
  ///     tile will be displayed in an Image Control as well as the Url for that image." /&gt;
  ///   
  ///   &lt;!-- The Map Control. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="33,160,0,0" Name="Label_MapControl" 
  ///        VerticalAlignment="Top" Width="120" Content="Map Control:"/&gt;
  ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="32,180,0,0" Name="Map1" 
  ///        VerticalAlignment="Top" WrapAround="True" Height="320" Width="600"&gt;
  ///     &lt;esri:Map.Layers&gt;
  ///       &lt;esri:LayerCollection&gt;
  ///       
  ///         &lt;!-- 
  ///         Add an OpenStreetMapLayer. The InitializationFailed Event is used to notify the user in case the 
  ///         OpenStreetMapLayer service is down. The TileLoading Event provides details about individual tiles 
  ///         in the OpenStreetMapLayer service that is necessary to get the input parameters (Level, Row, Column) 
  ///         of the OpenStreetMapLayer.GetTileUrl Method. 
  ///         --&gt;
  ///         &lt;esri:OpenStreetMapLayer ID="myOpenStreetMapLayer" Style="CycleMap"
  ///               InitializationFailed="OpenStreetMapLayer_InitializationFailed"
  ///               TileLoading="OpenStreetMapLayer_TileLoading"/&gt;
  ///       
  ///       &lt;/esri:LayerCollection&gt;
  ///     &lt;/esri:Map.Layers&gt;
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- ListBox results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="33,512,0,0" Name="Label_ListBox1" 
  ///        VerticalAlignment="Top" Width="194" Content="ListBox Control:"/&gt;
  ///   &lt;ListBox Height="93" HorizontalAlignment="Left" Margin="33,526,0,0" Name="ListBox1" 
  ///            VerticalAlignment="Top" Width="194" SelectionChanged="ListBox1_SelectionChanged"/&gt;
  ///     
  ///   &lt;!-- TiledLayer.TileLoadEventsArgs. Level, Row, and Column. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,510,0,0" Name="Label_TileLoadEventArgs" 
  ///        VerticalAlignment="Top" Width="120" Content="TileLoadEventArgs:"/&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,542,0,0" Name="Label_Level" 
  ///        VerticalAlignment="Top" Width="48" Content="Level:"/&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,536,0,0" Name="TextBox_Level" 
  ///            VerticalAlignment="Top" Width="52" /&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,569,0,0" Name="Label_Row" 
  ///        VerticalAlignment="Top" Width="48" Content="Row:"/&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,566,0,0" Name="TextBox_Row" 
  ///            VerticalAlignment="Top" Width="51" /&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="239,602,0,0" Name="Label_Column" 
  ///        VerticalAlignment="Top" Width="48" Content="Column:" /&gt;
  ///   &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="293,596,0,0" Name="TextBox_Column" 
  ///            VerticalAlignment="Top" Width="52" /&gt;
  ///   
  ///   &lt;!-- OpenStreetMapLayer.GetTileUrl results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="32,631,0,0" Name="Label_GetTileUrl" 
  ///        VerticalAlignment="Top" Width="344" Content="OpenStreetMapLayer.GetTileUrl:"/&gt;
  ///   &lt;TextBox Height="124" HorizontalAlignment="Left" Margin="32,648,0,0" Name="TextBox_GetTileUrl" 
  ///            VerticalAlignment="Top" Width="344" TextWrapping="Wrap"/&gt;
  ///   
  ///   &lt;!-- Image Control results. --&gt;
  ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="384,508,0,0" Name="Label_ImageControl1" 
  ///        VerticalAlignment="Top" Width="198" Content="Image Control:"/&gt;
  ///   &lt;Image Height="250" HorizontalAlignment="Left" Margin="382,522,0,0" Name="Image1" 
  ///          Stretch="Fill" VerticalAlignment="Top" Width="250" /&gt;
  ///   
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void OpenStreetMapLayer_InitializationFailed(object sender, System.EventArgs e)
  /// {
  ///   // Notify the user if there is a failure with the OpenStreetMapLayer service.
  ///   ESRI.ArcGIS.Client.Layer aLayer = (ESRI.ArcGIS.Client.Layer)sender;
  ///   MessageBox.Show(aLayer.InitializationFailure.Message);
  /// }
  /// 
  /// private void OpenStreetMapLayer_TileLoading(object sender, ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs e)
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
  ///   if (!(ListBox1.Items.Contains(myString)))
  ///   {
  ///     ListBox1.Items.Add(myString);
  ///   }
  /// }
  /// 
  /// private void ListBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  /// {
  ///   // Get the SelectedItem from the Listbox and parse out the Level, Row, and Column arguments necessary to 
  ///   // obtain the Url for a specific tile.
  ///   string theConcatenatedString = (string)ListBox1.SelectedItem;
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
  ///   // Get the OpenStreetMapLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer theOpenStreetMapLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer)Map1.Layers["myOpenStreetMapLayer"];
  ///   
  ///   // Obtain a specific tile Url from the OpenStreetMapLayer using the three arguments for the GetTileUrl Method.
  ///   string theGetTileUrl = theOpenStreetMapLayer.GetTileUrl(theLevel, theRow, theColumn);
  ///   TextBox_GetTileUrl.Text = theGetTileUrl;
  ///   
  ///   // Only process valid restults. 
  ///   if (theGetTileUrl != null)
  ///   {
  ///     // Set the specific tile's Url as the Image's Source. 
  ///     Uri myUri = new Uri(theGetTileUrl);
  ///     Image1.Source = new System.Windows.Media.Imaging.BitmapImage(myUri);
  ///   }
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub OpenStreetMapLayer_TileLoading(sender As System.Object, e As ESRI.ArcGIS.Client.TiledLayer.TileLoadEventArgs)
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
  ///   ' Get the OpenStreetMapLayer. 
  ///   Dim theOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = Map1.Layers("myOpenStreetMapLayer")
  ///   
  ///   ' Obtain a specific tile Url from the OpenStreetMapLayer using the three arguments for the GetTileUrl Method.
  ///   Dim theGetTileUrl As String = theOpenStreetMapLayer.GetTileUrl(theLevel, theRow, theColumn)
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
		public override string GetTileUrl(int level, int row, int col)
		{
			if (level + col + row < 0) return null;
			if (_tileServers != null && _tileServers.Count > 0)
            {
				string tileServer = _tileServers[(level + col + row) % _tileServers.Count];                
                string tileServerPathFormat = (tileServer.EndsWith("/")) ? "{0}{1}/{2}/{3}.png" : "{0}/{1}/{2}/{3}.png";
                return string.Format(tileServerPathFormat, tileServer, level, col, row);
            }
			return base.GetTileUrl(level, row, col);
		}

        /// <summary>
        /// Gets or Sets the tile servers to use when requesting tiles. If the TileServers Property is set 
        /// the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.Style"/> Property will be ignored.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is only required to create a new instance of an OpenStreetMapLayer and then add it to the 
        /// <see cref="ESRI.ArcGIS.Client.Map.Layers">Map.Layers</see> Property to display a default 
        /// OpenStreetMapLayer. The reason for this is that the internals of the OpenStreetMapLayer constructor 
        /// automatically uses an internal Url to a web service provided by the OpenStreetMap organization. So 
        /// the following is an XAML example of all that is needed to create a default OpenStreetMapLayer:
        /// </para>
        /// <code language="XAML">
        /// &lt;esri:Map x:Name="MyMap"&gt;
        ///   &lt;esri:OpenStreetMapLayer /&gt;
        /// &lt;/esri:Map&gt;
        /// </code>
        /// <para>
        /// The OpenStreetMap organization hosts several types of maps that can be used as OpenStreetMapLayer's. 
        /// To change which type of map is used, specify the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.Style">OpenStreetMapLayer.Style</see> 
        /// Property to any one of several 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.MapStyle">OpenStreetMapLayer.MapStyle</see> 
        /// Enumerations. The default OpenStreetMap.MapStyle Property is <b>OpenStreetMapLayer.MapStyle.Mapnik</b> meaning 
        /// that if an OpenStreetMapLayer.Style is not specified in constructing an OpenStreetMapLayer, the 
        /// <b>OpenStreetMapLayer.MapStyle.Mapnik</b> style will be used by default. The following is an XAML example of 
        /// specifying a specific OpenStreetMapLayer.Style when defining a new OpenStreetMapLayer:
        /// </para>
        /// <code language="XAML">
        /// &lt;esri:Map x:Name="MyMap"&gt;
        ///  &lt;esri:OpenStreetMapLayer Style=”CycleMap”/&gt;
        /// &lt;/esri:Map&gt;
        /// </code>
        /// <para>
        /// If it is not desired to use the map services provided directly by the OpenStreetMap organization or if you 
        /// discover that additional map services are provided for which Esri has not provided an explicit 
        /// OpenStreetMapLayer.Style, developers can explicitly provide their own Url's for an OpenStreetMapLayer 
        /// using the OpenStreetMapLayer.TileServers Property. When using the OpenStreetMapLayer.TileServers Property, 
        /// the OpenStreetMapLayer.Style Property is ignored. The 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList">OpenStreetMapLayer.TileServerList</see> 
        /// Class is a List Collection of Strings that are the Url's to various OpenStreetMap map based servers. The 
        /// following is an XAML example of specifying specific Urls using the OpenStreetMapLayer.TileServers Property 
        /// when defining a new OpenStreetMapLayer:
        /// </para>
        /// <code language="XAML">
        /// &lt;esri:Map x:Name="MyMap"&gt;
        ///   &lt;esri:OpenStreetMapLayer ID="osmLayer"&gt;
        ///     &lt;esri:OpenStreetMapLayer.TileServers&gt;
        ///       &lt;sys:String&gt;http://otile1.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
        ///       &lt;sys:String&gt;http://otile2.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
        ///       &lt;sys:String&gt;http://otile3.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
        ///     &lt;/esri:OpenStreetMapLayer.TileServers&gt;
        ///   &lt;/esri:OpenStreetMapLayer&gt;
        /// &lt;/esri:Map&gt;
        /// </code>
        /// <para>
        /// It is important to understand that only one Url is needed in the OpenStreetMapLayer.TileServerList. If multiple 
        /// Urls are included in the OpenStreetMapLayer.TileServerList, all of the map servers should be serving up the same 
        /// base data. The reason for having the ability to specify multiple Urls in the OpenStreetMapLayer.TileServerList 
        /// is to improve performance by load balancing the requests the client application uses across multiple servers. 
        /// If different OpenStreetMap based map services are specified in the OpenStreetMapLayer.TileServerList, the tiles 
        /// that are placed together in the Esri Map control will yield unexpected results. For example, assume that three 
        /// different OpenStreetMap map based services are used for the the OpenStreetMapLayer.TileServers Property in the 
        /// following XAML example code:
        /// </para>
        /// <code language="XAML">
        /// &lt;esri:Map x:Name="MyMap"&gt;
        ///   &lt;esri:OpenStreetMapLayer ID="osmLayer"&gt;
        ///     &lt;esri:OpenStreetMapLayer.TileServers&gt;
        ///       &lt;sys:String&gt;http://otile1.mqcdn.com/tiles/1.0.0/osm&lt;/sys:String&gt;
        ///       &lt;sys:String&gt;http://a.tile.openstreetmap.org&lt;/sys:String&gt;
        ///       &lt;sys:String&gt;http://a.tile.opencyclemap.org/cycle&lt;/sys:String&gt;
        ///     &lt;/esri:OpenStreetMapLayer.TileServers&gt;
        ///   &lt;/esri:OpenStreetMapLayer&gt;
        /// &lt;/esri:Map&gt;   
        /// </code>
        /// <para>
        /// The following is a screen shot of the previous XAML code fragment showing the application's undesirable 
        /// results appearing if different map based services were used in the the OpenStreetMap.TileServers Property:
        /// </para>
        /// <para>
        /// <img border="0" alt="Using different base map services as the OpenStreetMap.TileServer Property yields strange results." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.OpenStreetMap.TileServersStrange.png"/>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the various buttons (on left) to add OpenStreetMapLayers to the Map. Click the 'Clear all layers' 
        /// button in-between adding the layers to clear out the Map. Notice the effect of adding the various 
        /// OpenStreetMapLayers - read the comments in the code-behind for more info.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Using the OpenStreetMapLayer.TileServers Property to add different layers." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.DataSources.OpenStreetMap.TileServers.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" &gt;
        ///   
        ///   &lt;!-- Add a Map Control to the application. --&gt;
        ///   &lt;esri:Map x:Name="Map1" WrapAround="True" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///         Margin="0,250,0,0" Height="350" Width="550" /&gt;
        ///   
        ///   &lt;!-- Button to add multiple OpenStreetMapLayers all from the same base data. --&gt;
        ///   &lt;Button Name="Button_MultipleSameBase" Height="23" HorizontalAlignment="Left" Margin="0,165,0,0"  
        ///           Width="395" Content="Add multiple OpenStreetMapLayer's all using the same base data."
        ///           VerticalAlignment="Top" Click="Button_MultipleSameBase_Click" /&gt;
        ///   
        ///   &lt;!-- Button to add multiple OpenStreetMapLayers each using different base data. --&gt;
        ///   &lt;Button Content="Add multiple OpenStreetMapLayers' each using different base data." Height="23"  
        ///           Margin="0,193,0,0" Name="ButtonMultipleDifferentBase" VerticalAlignment="Top" Width="395" 
        ///           HorizontalAlignment="Left" Click="ButtonMultipleDifferentBase_Click"/&gt;
        ///     
        ///   &lt;!-- Button to add one OpenStreetMapLayer. --&gt;
        ///   &lt;Button Content="Add one OpenStreetMapLayer." Height="23" HorizontalAlignment="Left" Margin="0,222,0,0" 
        ///           Name="Button_OneLayer" VerticalAlignment="Top" Width="394" Click="Button_OneLayer_Click"/&gt;
        ///     
        ///   &lt;!-- Clear all of the Layers in the Map. --&gt;
        ///   &lt;Button Content="Clear all layers." Height="80" HorizontalAlignment="Left" Margin="401,165,0,0" 
        ///           Name="Button_ClearAllLayers" VerticalAlignment="Top" Width="149" Click="Button_ClearAllLayers_Click"/&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="77" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="572" 
        ///              TextWrapping="Wrap" Text="Click the various buttons (on left) to add OpenStreetMapLayers to the Map. 
        ///              Click the 'Clear all layers' button in-between adding the layers to clear out the Map. Notice the 
        ///              effect of adding the various OpenStreetMapLayers - read the comments in the code-behind for more info." /&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button_MultipleSameBase_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Create a new instance of an OpenStreetMapLayer.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer();
        ///   
        ///   // Create a new instance of the TileServerList object.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList myTileServers = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList();
        ///   
        ///   // Add Urls (for the same base data) to the TileServerList. This is a great way to have a performance increase on the 
        ///   // client side as you are getting data from multiple servers and not taxing any one server too much.
        ///   myTileServers.Add("http://otile1.mqcdn.com/tiles/1.0.0/osm");
        ///   myTileServers.Add("http://otile2.mqcdn.com/tiles/1.0.0/osm");
        ///   myTileServers.Add("http://otile3.mqcdn.com/tiles/1.0.0/osm");
        ///   
        ///   // Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers;
        ///   
        ///   // Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer);
        /// }
        ///   
        /// private void ButtonMultipleDifferentBase_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Create a new instance of an OpenStreetMapLayer.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer();
        ///   
        ///   // Create a new instance of the TileServerList object.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList myTileServers = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList();
        ///   
        ///   // This is not a realistic scenario. If different OpenStreetMap based map services are specified in the 
        ///   // OpenStreetMapLayer.TileServerList, the tiles that are placed together in the Esri Map control will yield 
        ///   // unexpected results.
        ///   myTileServers.Add("http://otile1.mqcdn.com/tiles/1.0.0/osm");
        ///   myTileServers.Add("http://a.tile.openstreetmap.org");
        ///   myTileServers.Add("http://a.tile.opencyclemap.org/cycle");
        ///   
        ///   // Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers;
        ///   
        ///   // Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer);
        /// }
        ///   
        /// private void Button_OneLayer_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Create a new instance of an OpenStreetMapLayer.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer myOpenStreetMapLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer();
        ///   
        ///   // Create a new instance of the TileServerList object.
        ///   ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList myTileServers = new ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList();
        ///   
        ///   // If you only have one server to access, then just add one Url to the TileServerList.
        ///   myTileServers.Add("http://a.tile.cloudmade.com/fd093e52f0965d46bb1c6c6281022199/3/256");
        ///   
        ///   // Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers;
        ///   
        ///   // Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer);
        /// }
        ///   
        /// private void Button_ClearAllLayers_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Clear all the layers in the Map. 
        ///   // Note: Do this before clicking the other buttons so you can see the newly added OpenStreetMapLayer(s).
        ///   Map1.Layers.Clear();
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_MultipleSameBase_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Create a new instance of an OpenStreetMapLayer.
        ///   Dim myOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer
        ///   
        ///   ' Create a new instance of the TileServerList object.
        ///   Dim myTileServers As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList
        ///   
        ///   ' Add Urls (for the same base data) to the TileServerList. This is a great way to have a performance increase on the 
        ///   ' client side as you are getting data from multiple servers and not taxing any one server too much.
        ///   myTileServers.Add("http://otile1.mqcdn.com/tiles/1.0.0/osm")
        ///   myTileServers.Add("http://otile2.mqcdn.com/tiles/1.0.0/osm")
        ///   myTileServers.Add("http://otile3.mqcdn.com/tiles/1.0.0/osm")
        ///   
        ///   ' Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers
        ///   
        ///   ' Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer)
        ///   
        /// End Sub
        ///   
        /// Private Sub ButtonMultipleDifferentBase_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Create a new instance of an OpenStreetMapLayer.
        ///   Dim myOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer
        ///   
        ///   ' Create a new instance of the TileServerList object.
        ///   Dim myTileServers As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList
        ///   
        ///   ' This is not a realistic scenario. If different OpenStreetMap based map services are specified in the 
        ///   ' OpenStreetMapLayer.TileServerList, the tiles that are placed together in the Esri Map control will yield 
        ///   ' unexpected results.
        ///   myTileServers.Add("http://otile1.mqcdn.com/tiles/1.0.0/osm")
        ///   myTileServers.Add("http://a.tile.openstreetmap.org")
        ///   myTileServers.Add("http://a.tile.opencyclemap.org/cycle")
        ///   
        ///   ' Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers
        ///   
        ///   ' Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer)
        ///   
        /// End Sub
        ///   
        /// Private Sub Button_OneLayer_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Create a new instance of an OpenStreetMapLayer.
        ///   Dim myOpenStreetMapLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer
        ///   
        ///   ' Create a new instance of the TileServerList object.
        ///   Dim myTileServers As ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList = New ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServerList
        ///   
        ///   ' If you only have one server to access, then just add one Url to the TileServerList.
        ///   myTileServers.Add("http://a.tile.cloudmade.com/fd093e52f0965d46bb1c6c6281022199/3/256")
        ///   
        ///   ' Set the OpenStreetMap.TileServer Property.
        ///   myOpenStreetMapLayer.TileServers = myTileServers
        ///   
        ///   ' Add the OpenStreetMapLayer to the Map's Layer Collection. This will refresh the map with the new layers.
        ///   Map1.Layers.Add(myOpenStreetMapLayer)
        ///   
        /// End Sub
        ///   
        /// Private Sub Button_ClearAllLayers_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Clear all the layers in the Map. 
        ///   ' Note: Do this before clicking the other buttons so you can see the newly added OpenStreetMapLayer(s).
        ///   Map1.Layers.Clear()
        ///   
        /// End Sub
        /// </code>
        /// </example>
		[Obsolete("Use the TemplateUrl property")]
        public TileServerList TileServers
        {
            get { return (TileServerList)GetValue(TileServersProperty); }
            set { SetValue(TileServersProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServers"/>.
        /// </summary>
		[Obsolete("Use the TemplateUrlProperty")]
        public static readonly DependencyProperty TileServersProperty =
            DependencyProperty.Register("TileServers", typeof(TileServerList), typeof(OpenStreetMapLayer), new PropertyMetadata(OnTileServersPropertyChanged));
      
        
        private static void OnTileServersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OpenStreetMapLayer obj = (OpenStreetMapLayer)d;
	        obj._tileServers = e.NewValue as TileServerList;
            if (obj.IsInitialized)
                obj.Refresh();
        }

		/// <summary>
  /// Gets or sets the map style. This property is ignored if the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.TileServers"/> Property is set.
		/// </summary>
  /// <remarks>
  /// <para>
  /// The OpenStreetMap organization hosts several types of maps that can be used as OpenStreetMapLayer's. 
  /// To change which type of map is used, specify the OpenStreetMapLayer.Style Property to any one of several 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer.MapStyle">OpenStreetMapLayer.MapStyle</see> 
  /// Enumerations. The default OpenStreetMap.MapStyle Property is <b>OpenStreetMapLayer.MapStyle.Mapnik</b> meaning 
  /// that if an OpenStreetMapLayer.Style is not specified in constructing an OpenStreetMapLayer, the 
  /// <b>OpenStreetMapLayer.MapStyle.Mapnik</b> style will be used by default. The following is an XAML example of 
  /// specifying a specific OpenStreetMapLayer.Style when defining a new OpenStreetMapLayer:
  /// </para>
  /// <code language="XAML">
  /// &lt;esri:Map x:Name="MyMap"&gt;
  ///  &lt;esri:OpenStreetMapLayer Style=”CycleMap”/&gt;
  /// &lt;/esri:Map&gt;
  /// </code>
  /// </remarks>
		public MapStyle Style
		{
			get { return (MapStyle)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Style"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty StyleProperty =
			DependencyProperty.Register("Style", typeof(MapStyle), typeof(OpenStreetMapLayer), new PropertyMetadata(MapStyle.Mapnik, OnStylePropertyChanged));

		private static void OnStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (OpenStreetMapLayer)d;
			obj.TemplateUrl = baseUrl[(int)obj.Style];
		}

		/// <summary>
		/// MapStyle
		/// </summary>
		public enum MapStyle : int
		{
			/// <summary>
			/// Mapnik
			/// </summary>
			Mapnik = 0,			
			/// <summary>
			/// Cycle Map
			/// </summary>
			CycleMap = 1,
			/// <summary>
			/// No Name
			/// </summary>
			NoName = 2
		}


        /// <summary>
        /// Holds a list of string urls to tile servers.
        /// </summary>
        public class TileServerList : List<string>
        {            
        }
	}
}
