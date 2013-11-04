// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Net;
using System.IO;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.Utils;
using ESRI.ArcGIS.Client.Geometry;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client;
using System.Globalization;
using System.Windows.Resources;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{

 /// <summary>
 /// A <see cref="ESRI.ArcGIS.Client.Geometry.MapPoint">MapPoint</see> type of custom 
 /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see> based on the 
 /// <a href="http://office.microsoft.com/en-us/excel-help/import-or-export-text-txt-or-csv-files-HP010099725.aspx" target="_blank">Comma Separated Value</a> 
 /// (CSV) file format.
 /// </summary>
 /// <remarks>
 /// <para>
 /// The CSV file format is tabular data in plain text. The data in the CSV file consists of fields of data separated by 
 /// a delimiting character (typically a comma) for a record. The first record in the CSV file is known as the header and 
 /// defines the names of each field of the tabular data. The second through last row of records in the CSV file is the 
 /// actual tabular data. When the delimiter (typically a comma) is embedded in the tabular data for a particular field, 
 /// that value should be encased in quotes to avoid parsing errors. Each record in the CSV file should contain the same 
 /// number of fields. Numerous applications including the Microsoft Excel Office product can export and import CSV files. 
 /// It is not required that a CSV source file contain the extension .csv; the file can contain any extension (ex: .txt) 
 /// or none at all.
 /// </para>
 /// <para>
 /// In order to make use of CSV files in the 
 /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
 /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
 /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
 ///  there should be point based spatial locational coordinate information for each record. This spatial information 
 ///  defines the <see cref="ESRI.ArcGIS.Client.Geometry.MapPoint">MapPoint</see> that will be used to construct a 
 ///  custom <see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see>. Other geography types like Polyline or 
 ///  Polygon are not supported for constructing a CsvLayer. Unless specified otherwise in the 
 ///  <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SourceSpatialReference">SourceSpatialReference</see> 
 ///  Property, it is assumed that the <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> 
 ///  of the CsvLayer has a WKID value of 4326. During the parsing process of reading the header record of the CSV file 
 ///  to construct the CsvLayer, any of the following names can be used to automatically detect the spatial location 
 ///  coordinate information:
 /// </para>
 /// <list type="table">
 ///   <listheader><term>Coordinate Type</term><description>Automatically detected Field names</description></listheader>
 ///   <item><term>LATITUDE</term><description>"lat", "latitude", "y", "ycenter", "latitude83", "latdecdeg", "point-y"</description></item>
 ///   <item><term>LONGITUDE</term><description>"lon", "lng", "long", "longitude", "x", "xcenter", "longitude83", "longdecdeg", "point-x"</description></item>
 /// </list>
 /// <para>
 /// NOTE: The CsvLayer parsing algorithm for the Field names listed in the table above is case insensitive.
 /// </para>
 /// <para>
 /// If the above spatial location coordinate field names are not specified in the header record, then it will be required 
 /// to specify them explicitly. Use the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.XFieldName">XFieldName</see> 
 /// Property to explicitly specify the Longitude coordinate and the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.YFieldName">YFieldName</see> Property to explicitly specify 
 /// the Latitude coordinate.
 /// </para>
 /// <para>
 /// By default it is assumed that the delimiter for the CSV file in parsing data values for between fields is the comma 
 /// (,). If another delimiter is used (for example a tab or dash) it is required to specify the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.ColumnDelimiter">ColumnDelimiter</see> Property. If the 
 /// comma is used as the delimiter in the CSV file the ColumnDelimiter Property does not need to be set.
 /// </para>
 /// <para>
 /// If it is not desired to convert all of the fields of information in the CSV records into a CsvLayer use the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SourceFields">SourceFields</see> Property to set exactly 
 /// which fields will become attributes in the CsvLayer. If the SourceFields Property is not specified then all fields 
 /// of information in the CSV file will be used to populate the attributes in the CsvLayer. To restrict which fields 
 /// are generated in the CsvLayer using the SourceFields Property, create a new 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection">CsvLayer.FieldCollection</see> object 
 /// and add the specific <see cref="ESRI.ArcGIS.Client.Field">Field</see> objects with the minimum Properties of 
 /// <see cref="ESRI.ArcGIS.Client.Field.FieldName">Field.FieldName</see> and 
 /// <see cref="ESRI.ArcGIS.Client.Field.Type">Field.Type</see> being set. The Field.FieldName should match the header 
 /// name contained inside the CSV file. If Field.Type is not provided field type will default to string.
 /// </para>
 /// <para>
 /// If it is desired to obtain the CSV data from a 
 /// <a href="http://msdn.microsoft.com/en-us/library/system.io.stream(v=vs.100).aspx" target="_blank">Stream</a> rather 
 /// than a Url use the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SetSource">SetSource</see> Method.
 /// </para>
 /// <para>
 /// Because the CsvLayer inherits all of the GraphicsLayer functionality (which also inherits from Layer), this means 
 /// that all of the things you can do a GraphicsLayer can also be done on a CsvLayer. For example 
 /// <ESRISILVERLIGHT>you can set up a <b>GraphicsLayer.MapTip</b> to display popup information when the mouse is hovered over a particular MapPoint Graphic;</ESRISILVERLIGHT>
 /// <ESRIWPF>you can set up a <b>GraphicsLayer.MapTip</b> to display popup information when the mouse is hovered over a particular MapPoint Graphic;</ESRIWPF>
 /// you can <see cref="ESRI.ArcGIS.Client.Graphic.Select">select</see> a particular Graphic in the CsvLayer; 
 /// or you can even define when particular Graphic displays in the Map based upon a 
 /// <see cref="ESRI.ArcGIS.Client.Layer.VisibleTimeExtent">VisibleTimeExtent</see>. The possibilities are only limited 
 /// to your ideas and programming experience.
 /// </para>
 /// <ESRISILVERLIGHT><para>Although ArcGIS Server is not required to host a CSV file web service you may experience the similar hosting issues of accessing the web service as described in the ArcGIS Resource Center blog entitled <a href="http://blogs.esri.com/esri/arcgis/2009/08/24/troubleshooting-blank-layers/" target="_blank">Troubleshooting blank layers</a>. Specifically, you may need to make sure that a correct <a href="http://msdn.microsoft.com/EN-US/LIBRARY/CC197955(VS.95).ASPX" target="_blank">cilentaccesspolicy.xml or crossdomain.xml</a> file is in place on the web servers root. If a clientaccesspolicy.xml or crossdomainpolicy.xml file cannot be used on your web server for situations like <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/index.html#/Secure_services/016600000022000000/" target="_blank">secure services</a> you may need to use a <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/0166/other/SLProxyPage.zip">proxy page</a> on your web server and make use of the <b>CsvLayer.ProxyUrl</b> Property.</para></ESRISILVERLIGHT>
 /// <para>
 /// The bare minimum settings that need to be specified to create and display a CsvLayer in a Map are the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.Url">Url</see> and 
 /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Url methadology) OR the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SetSource">SetSource</see> and 
 /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Stream methodology). 
 /// NOTE: This assumes that default spatial coordinate information field names are used and the delimiter for the 
 /// CSV file is a comma.
 /// </para>
 /// <para>
 /// There are several methods to construct a Url for accessing data in a CSV layer. The example code in the 
 /// <b>CsvLayer</b> Class documentation shows how to access 
 /// a CSV file via on a web server and uses the 'http://' keyword to construct a Url. The example code in the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.Url">CsvLayer.Url</see> Property documentation shows 
 /// how to access a CSV file as a resource on the local disk in a Visual Studio project using the 
 /// '[Visual_Studio_Project_Name]' and the 'component' keywords. Even more options are available such as constructing 
 /// a Url using the 'file://' or 'pack://' keywords. The development platform you are coding in will determine which 
 /// style of Url is appropriate. See the documentation for your particular development platform to decide which type 
 /// of string can be used in the the Url construction.
 /// </para>
 /// </remarks>
 /// <example>
 /// <para>
 /// <b>How to use:</b>
 /// </para>
 /// <para>
 /// When the application loads a CsvLayer will automatically be added to the Map (it was specified in XAML). 
 /// Click the Button to add another CsvLayer to the Map (it will be added via code-behind). The ID of each 
 /// layer will displayed in the TextBox. 
 /// </para>
 /// <para>
 /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
 /// the functionality.
 /// </para>
 /// <para>
 /// The following is an example of the ASCII contents for the file named US_Cities_Top_5.csv:<br/>
 /// ID,Lat,Long,CityName,Population<br/>
 /// 1,40.714,-74.006,New York City,8244910<br/>
 /// 2,34.0522,-118.244,Los Angeles,3819702<br/>
 /// 3,41.878,-87.636,Chicago,2708120<br/>
 /// 4,29.763,-95.363,Houston,2099451<br/>
 /// 5,39.952,-75.168,Philadelphia,1526006<br/>
 /// </para>
 /// <para>
 /// The following is an example of the ASCII contents for the file named US_Cities_6_to_10.csv:<br/>
 /// ID,Lat,Long,CityName,Population<br/>
 /// 6,29.423,-98.493,San Antonio,1327407<br/>
 /// 7,32.715,-117.156,San Diego,1326179<br/>
 /// 8,32.782,-96.815,Dallas,1223229<br/>
 /// 9,37.228,-119.228,San Jose,945942<br/>
 /// 10,30.331,-81.655,Jacksonville,821784<br/>
 /// </para>
 /// <para>
 /// The following screen shot corresponds to the code example in this page.
 /// </para>
 /// <para>
 /// <img border="0" alt="Adding a CsvLayer in XAML and code-behind." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.png"/>
 /// </para>
 /// <code title="Example XAML1" description="" lang="XAML">
 /// &lt;Grid x:Name="LayoutRoot" &gt;
 ///   
 ///   &lt;!-- Add local Resources to define a SimpleRender to display Red circles as a SimpleMarkerSymbol 
 ///   for the CsvLayer. --&gt;
 ///   &lt;Grid.Resources&gt;
 ///     &lt;esri:SimpleRenderer x:Key="myRenderer"&gt;
 ///       &lt;esri:SimpleRenderer.Symbol&gt;
 ///         &lt;esri:SimpleMarkerSymbol Color="Red" Size="12" Style="Circle" /&gt;
 ///       &lt;/esri:SimpleRenderer.Symbol&gt;
 ///     &lt;/esri:SimpleRenderer&gt;
 ///   &lt;/Grid.Resources&gt;
 ///   
 ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
 ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
 ///         Margin="0,238,0,0" Height="350" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
 ///   
 ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
 ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
 ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
 ///     
 ///     &lt;!-- Add a CsvLayer. The renderer symbology will be based upon the local static resource defined above.
 ///     NOTE: you need to adjust the Url to a .csv file served up on your test web server. --&gt;
 ///     &lt;esri:CsvLayer ID="US_Cities_Top_5" Url="http://www.yourserver.com/CSV_Files/US_Cities_Top_5.csv"
 ///           Renderer="{StaticResource myRenderer}" Initialized="CsvLayer_Initialized"/&gt;
 ///   &lt;/esri:Map&gt;
 ///   
 ///   &lt;!-- Add a Button that will allow the user to add another CsvLayer via code-behind. --&gt;
 ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,209,0,0"  VerticalAlignment="Top" 
 ///           Width="706" Content="Add another CsvLayer (via code-behind) for the specified Url."
 ///           Click="Button1_Click" /&gt;
 ///   
 ///   &lt;!-- TextBox to display information about about the CsvLayerLayers added to the Map. --&gt;
 ///   &lt;TextBox Height="350" HorizontalAlignment="Left" Margin="421,238,0,0" Name="TextBox1" VerticalAlignment="Top" 
 ///            Width="285" /&gt;
 ///   
 ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
 ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
 ///              TextWrapping="Wrap" Text="When the application loads a CsvLayer will automatically be added 
 ///              to the Map (it was specified in XAML). Click the Button to add another CsvLayer to the Map 
 ///              (it will be added via code-behind). The ID of each layer will displayed in the TextBox." /&gt;
 /// &lt;/Grid&gt;
 /// </code>
 /// <code title="Example CS1" description="" lang="CS">
 /// private void CsvLayer_Initialized(object sender, System.EventArgs e)
 /// {
 ///   // This function will execute as a result of the CsvLayer that was defined in XAML being Initialized.
 ///   
 ///   // Get the CsvLayer.
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
 ///   
 ///   // Get the ID of the CsvLayer.
 ///   string myID = myCsvLayer.ID;
 ///   
 ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
 ///   myStringBuilder.Append("The 1st CsvLayer was" + Environment.NewLine);
 ///   myStringBuilder.Append("added via XAML and it's ID is: " + Environment.NewLine);
 ///   myStringBuilder.Append(myID);
 ///   
 ///   // Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString();
 /// }
 /// 
 /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
 /// {
 ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
 ///   
 ///   // Create a CsvLayer. 
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer2 = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
 ///   
 ///   // Set the Url of the CsvLayer to a public service. 
 ///   // NOTE: you need to adjust the Url to a .csv file served up on your test web server.
 ///   myCsvLayer2.Url = "http://www.yourserver.com/CSV_Files/US_Cities_6_to_10.csv";
 ///   
 ///   // Set the ID of the CsvLayer.
 ///   myCsvLayer2.ID = "US_Cities_6_to_10";
 ///   
 ///   // Create a SimpleMarkerSymbol (a green circle) for the CsvLayer that will be added.
 ///   ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol theSimpleMarkerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol();
 ///   theSimpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
 ///   theSimpleMarkerSymbol.Size = 12;
 ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
 ///   
 ///   // Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
 ///   ESRI.ArcGIS.Client.SimpleRenderer theSimpleRenderer = new ESRI.ArcGIS.Client.SimpleRenderer();
 ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol;
 ///   
 ///   // Define the Renderer for the CsvLayer.
 ///   myCsvLayer2.Renderer = theSimpleRenderer;
 ///   
 ///   // Wire-up the Initialized Event of the CsvLayer. Note how a different Initilized Event is being used verses 
 ///   // the one defined in XAML. They could share the same Initialized Event but we created two seperate ones 
 ///   // for demonstration purposes.
 ///   myCsvLayer2.Initialized += CsvLayer_Initialized2;
 ///   
 ///   // Add the CsvLayer to the Map.
 ///   Map1.Layers.Add(myCsvLayer2);
 /// }
 /// 
 /// private void CsvLayer_Initialized2(object sender, EventArgs e)
 /// {
 ///   // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
 ///   
 ///   // Get the CsvLayer.
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer2 = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
 ///   
 ///   // Get the ID of the CsvLayer.
 ///   string myID2 = myCsvLayer2.ID;
 ///   
 ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
 ///   myStringBuilder.Append(TextBox1.Text + Environment.NewLine);
 ///   myStringBuilder.Append(Environment.NewLine);
 ///   myStringBuilder.Append("The 2nd CsvLayer was" + Environment.NewLine);
 ///   myStringBuilder.Append("added via code-behind and it's ID is: " + Environment.NewLine);
 ///   myStringBuilder.Append(myID2);
 ///   
 ///   // Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString();
 /// }
 /// </code>
 /// <code title="Example VB1" description="" lang="VB.NET">
 /// Private Sub CsvLayer_Initialized(sender As System.Object, e As System.EventArgs)
 ///   
 ///   ' This function will execute as a result of the CsvLayer that was defined in XAML being Initialized.
 ///   
 ///   ' Get the CsvLayer.
 ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
 ///   
 ///   ' Get the ID of the CsvLayer.
 ///   Dim myID As String = myCsvLayer.ID
 ///   
 ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   Dim myStringBuilder As New Text.StringBuilder
 ///   myStringBuilder.Append("The 1st CsvLayer was" + vbCrLf)
 ///   myStringBuilder.Append("added via XAML and it's ID is: " + vbCrLf)
 ///   myStringBuilder.Append(myID)
 ///   
 ///   ' Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString
 ///   
 /// End Sub
 /// 
 /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
 ///   
 ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
 ///   
 ///   ' Create a CsvLayer. 
 ///   Dim myCsvLayer2 As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
 ///   
 ///   ' Set the Url of the CsvLayer to a public service. 
 ///   ' NOTE: you need to adjust the Url to a .csv file served up on your test web server.
 ///   myCsvLayer2.Url = "http://www.yourserver.com/CSV_Files/US_Cities_6_to_10.csv"
 ///   
 ///   ' Set the ID of the CsvLayer.
 ///   myCsvLayer2.ID = "US_Cities_6_to_10"
 ///   
 ///   ' Create a SimpleMarkerSymbol (a green circle) for the CsvLayer that will be added.
 ///   Dim theSimpleMarkerSymbol As ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol = New ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
 ///   theSimpleMarkerSymbol.Color = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green)
 ///   theSimpleMarkerSymbol.Size = 12
 ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
 ///   
 ///   ' Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
 ///   Dim theSimpleRenderer As ESRI.ArcGIS.Client.SimpleRenderer = New ESRI.ArcGIS.Client.SimpleRenderer
 ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol
 ///   
 ///   ' Define the Renderer for the CsvLayer.
 ///   myCsvLayer2.Renderer = theSimpleRenderer
 ///   
 ///   ' Wire-up the Initialized Event of the CsvLayer. Note how a different Initilized Event is being used verses 
 ///   ' the one defined in XAML. They could share the same Initialized Event but we created two seperate ones 
 ///   ' for demonstration purposes.
 ///   AddHandler myCsvLayer2.Initialized, AddressOf CsvLayer_Initialized2
 ///   
 ///   ' Add the CsvLayer to the Map.
 ///   Map1.Layers.Add(myCsvLayer2)
 ///   
 /// End Sub
 /// 
 /// Private Sub CsvLayer_Initialized2(sender As Object, e As EventArgs)
 ///   
 ///   ' This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
 ///   
 ///   ' Get the CsvLayer.
 ///   Dim myCsvLayer2 As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
 ///   
 ///   ' Get the ID of the CsvLayer.
 ///   Dim myID2 As String = myCsvLayer2.ID
 ///   
 ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   Dim myStringBuilder As New Text.StringBuilder
 ///   myStringBuilder.Append(TextBox1.Text + vbCrLf)
 ///   myStringBuilder.Append(vbCrLf)
 ///   myStringBuilder.Append("The 2nd CsvLayer was" + vbCrLf)
 ///   myStringBuilder.Append("added via code-behind and it's ID is: " + vbCrLf)
 ///   myStringBuilder.Append(myID2)
 ///   
 ///   ' Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString
 ///   
 /// End Sub
 /// </code>
 /// </example>
	public class CsvLayer : GraphicsLayer
	{
        /// <summary>
        /// Initalizes and instance of <see cref="CsvLayer"/>.
        /// </summary>
		public CsvLayer()
		{
            SourceFields = new FieldCollection();
            SourceSpatialReference = new SpatialReference(4326);
		}

		#region Private Members

		private static string[] LAT_FIELDS = new string[] { "lat", "latitude", "y", "ycenter", "latitude83", "latdecdeg", "point-y" };
		private static string[] LON_FIELDS = new string[] { "lon", "lng", "long", "longitude", "x", "xcenter", "longitude83", "longdecdeg", "point-x" };		
		private bool initializing = false;
		private static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private CultureInfo numericCulture = CultureInfo.InvariantCulture;

		#endregion Private Members

		#region Public Members

        
  /// <summary>
  /// The fields that will be added as attribute of the graphic. The FieldName should match the header name contained 
  /// inside the CSV file. The FieldType will be used to convert column into that specific data type. If FieldSource 
  /// is empty or null then all fields will be returned as attributes. If FieldType is not provided field type will 
  /// default to string.
  /// </summary>
  /// <remarks>
  /// <para>
  /// If it is not desired to convert all of the fields of information in the CSV records into a CsvLayer use the 
  /// <b>SourceFields</b> Property to set exactly which fields will become attributes in the CsvLayer. If the <b>SourceFields</b> 
  /// Property is not specified then all fields of information in the CSV file will be used to populate the attributes 
  /// in the CsvLayer. To restrict which fields are generated in the CsvLayer using the <b>SourceFields</b> Property, create 
  /// a new the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection">CsvLayer.FieldCollection</see> 
  /// object and add the specific <see cref="ESRI.ArcGIS.Client.Field">Field</see> objects with the minimum Properties 
  /// of <see cref="ESRI.ArcGIS.Client.Field.FieldName">Field.FieldName</see> and 
  /// <see cref="ESRI.ArcGIS.Client.Field.Type">Field.Type</see> being set. The Field.FieldName should match the header 
  /// name contained inside the CSV file. If Field.Type is not provided field type will default to string.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// Click the Button to add a CsvLayer to the Map. The ID of the layer and its attribute information will displayed 
  /// in the TextBox.
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following is an example of the ASCII contents for the file named US_Cities_Top_3_ManyFields.csv:<br/>
  /// ID,Y,X,City,Useless,Population,Clueless,What,MyDate<br/>
  /// 1,40.714,-74.006,NYC,XXX,8244910,QQQ,GGG,12/1/2012<br/>
  /// 2,34.0522,-118.244,LA,YYY,3819702,AAA,TTT,12/2/2012<br/>
  /// 3,41.878,-87.636,Chicago,ZZZ,2708120,ZZZ,UUU,12/3/2012<br/>
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Specifying which attributes fields to load into a CsvLayer using SourceFields." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.SourceFields.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
  ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
  ///         Margin="0,212,0,0" Height="376" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
  ///   
  ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
  ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
  ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
  ///   
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- Add a Button that will allow the user to add a CsvLayer via code-behind. --&gt;
  ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,183,0,0"  VerticalAlignment="Top" 
  ///           Width="706" Content="Add a CsvLayer (via code-behind) for the specified Url."
  ///           Click="Button1_Click" /&gt;
  ///   
  ///   &lt;!-- TextBox to display attribute information about about the CsvLayer added to the Map. --&gt;
  ///   &lt;TextBox Height="376" HorizontalAlignment="Left" Margin="421,212,0,0" Name="TextBox1" VerticalAlignment="Top" 
  ///            Width="285" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
  ///              TextWrapping="Wrap" Text="Click the Button to add a CsvLayer to the Map. The ID of the layer and 
  ///              its attribute information will displayed in the TextBox." /&gt;
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   // Create a CsvLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
  ///   
  ///   // Set the Url of the CsvLayer to a public service. 
  ///   // NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3_ManyFields.csv";
  ///   
  ///   // Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3_ManyFields";
  ///   
  ///   // Create a FieldCollection object to define what Fields from the CSV file will be imported into the CsvLayer.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection theSourceFields = null;
  ///   theSourceFields = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection();
  ///   
  ///   // Define the Field objects that will be read in from the CSV file. 
  ///   // NOTE: not all of the attribute fields that are in the CSV file will be converted to Fields in the CsvLayer.
  ///   ESRI.ArcGIS.Client.Field theIDfield = new ESRI.ArcGIS.Client.Field();
  ///   theIDfield.FieldName = "ID";
  ///   theIDfield.Type = ESRI.ArcGIS.Client.Field.FieldType.OID;
  ///   ESRI.ArcGIS.Client.Field theXfield = new ESRI.ArcGIS.Client.Field();
  ///   theXfield.FieldName = "X";
  ///   theXfield.Type = ESRI.ArcGIS.Client.Field.FieldType.Double;
  ///   ESRI.ArcGIS.Client.Field theYfield = new ESRI.ArcGIS.Client.Field();
  ///   theYfield.FieldName = "Y";
  ///   theYfield.Type = ESRI.ArcGIS.Client.Field.FieldType.Double;
  ///   ESRI.ArcGIS.Client.Field theCity = new ESRI.ArcGIS.Client.Field();
  ///   theCity.FieldName = "City";
  ///   theCity.Type = ESRI.ArcGIS.Client.Field.FieldType.String;
  ///   ESRI.ArcGIS.Client.Field thePopulation = new ESRI.ArcGIS.Client.Field();
  ///   thePopulation.FieldName = "Population";
  ///   thePopulation.Type = ESRI.ArcGIS.Client.Field.FieldType.Integer;
  ///   ESRI.ArcGIS.Client.Field theMyDate = new ESRI.ArcGIS.Client.Field();
  ///   theMyDate.FieldName = "MyDate";
  ///   theMyDate.Type = ESRI.ArcGIS.Client.Field.FieldType.Date;
  ///   
  ///   // Add the defined Fields into the FieldCollection object.
  ///   theSourceFields.Add(theIDfield);
  ///   theSourceFields.Add(theXfield);
  ///   theSourceFields.Add(theYfield);
  ///   theSourceFields.Add(theCity);
  ///   theSourceFields.Add(thePopulation);
  ///   theSourceFields.Add(theMyDate);
  ///   
  ///   // Set the custom FieldCollection to the CsvLayer.SourceFields Property.
  ///   myCsvLayer.SourceFields = theSourceFields;
  ///   
  ///   // Create a SimpleMarkerSymbol (a red traingle) for the CsvLayer that will be added.
  ///   ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol theSimpleMarkerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol();
  ///   theSimpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
  ///   theSimpleMarkerSymbol.Size = 18;
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Triangle;
  ///   
  ///   // Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   ESRI.ArcGIS.Client.SimpleRenderer theSimpleRenderer = new ESRI.ArcGIS.Client.SimpleRenderer();
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol;
  ///   
  ///   // Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer;
  ///   
  ///   // Wire-up the Initialized Event of the CsvLayer.
  ///   myCsvLayer.Initialized += CsvLayer_Initialized;
  ///   
  ///   // Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer);
  /// }
  /// 
  /// private void CsvLayer_Initialized(object sender, EventArgs e)
  /// {
  ///   // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   // Get the CsvLayer.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
  ///   
  ///   // Get the ID of the CsvLayer.
  ///   string myID = myCsvLayer.ID;
  ///   
  ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + Environment.NewLine);
  ///   myStringBuilder.Append(myID + Environment.NewLine);
  ///   myStringBuilder.Append("====================================" + Environment.NewLine);
  ///   
  ///   // Get the GraphicCollection from the CsvLayer.
  ///   ESRI.ArcGIS.Client.GraphicCollection theGraphicCollection = myCsvLayer.Graphics;
  ///   if (theGraphicCollection != null)
  ///   {
  ///     // Loop through each Graphic.
  ///     foreach (ESRI.ArcGIS.Client.Graphic oneGraphic in theGraphicCollection)
  ///     {
  ///       // Get the Attribute Keys. 
  ///       System.Collections.Generic.ICollection&lt;string&gt; theFieldNameKeys = oneGraphic.Attributes.Keys;
  ///       
  ///       // Loop through each Attribute.
  ///       foreach (var oneKey in theFieldNameKeys)
  ///       {
  ///         // Get the value of the Attribute Field.
  ///         string theValue = oneGraphic.Attributes[oneKey].ToString();
  ///         
  ///         // Get the Type of the Attribute Field.
  ///         string oneFieldType = oneGraphic.Attributes[oneKey].GetType().ToString();
  ///         
  ///         // Add the Attribute Field name, OS data type, and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString() + "(" + oneFieldType + "): " + theValue + Environment.NewLine);
  ///       }
  ///       myStringBuilder.Append(Environment.NewLine);
  ///     }
  ///   }
  ///   
  ///   // Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString();
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  /// 
  ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   ' Create a CsvLayer. 
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
  ///   
  ///   ' Set the Url of the CsvLayer to a public service. 
  ///   ' NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3_ManyFields.csv"
  ///   
  ///   ' Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3_ManyFields"
  ///   
  ///   ' Create a FieldCollection object to define what Fields from the CSV file will be imported into the CsvLayer.
  ///   Dim theSourceFields As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection
  ///   theSourceFields = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.FieldCollection
  ///   
  ///   ' Define the Field objects that will be read in from the CSV file. 
  ///   ' NOTE: not all of the attribute fields that are in the CSV file will be converted to Fields in the CsvLayer.
  ///   Dim theIDfield As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   theIDfield.FieldName = "ID"
  ///   theIDfield.Type = ESRI.ArcGIS.Client.Field.FieldType.OID
  ///   Dim theXfield As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   theXfield.FieldName = "X"
  ///   theXfield.Type = ESRI.ArcGIS.Client.Field.FieldType.Double
  ///   Dim theYfield As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   theYfield.FieldName = "Y"
  ///   theYfield.Type = ESRI.ArcGIS.Client.Field.FieldType.Double
  ///   Dim theCity As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   theCity.FieldName = "City"
  ///   theCity.Type = ESRI.ArcGIS.Client.Field.FieldType.String
  ///   Dim thePopulation As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   thePopulation.FieldName = "Population"
  ///   thePopulation.Type = ESRI.ArcGIS.Client.Field.FieldType.Integer
  ///   Dim theMyDate As ESRI.ArcGIS.Client.Field = New ESRI.ArcGIS.Client.Field
  ///   theMyDate.FieldName = "MyDate"
  ///   theMyDate.Type = ESRI.ArcGIS.Client.Field.FieldType.Date
  ///   
  ///   ' Add the defined Fields into the FieldCollection object.
  ///   theSourceFields.Add(theIDfield)
  ///   theSourceFields.Add(theXfield)
  ///   theSourceFields.Add(theYfield)
  ///   theSourceFields.Add(theCity)
  ///   theSourceFields.Add(thePopulation)
  ///   theSourceFields.Add(theMyDate)
  ///   
  ///   ' Set the custom FieldCollection to the CsvLayer.SourceFields Property.
  ///   myCsvLayer.SourceFields = theSourceFields
  ///   
  ///   ' Create a SimpleMarkerSymbol (a red traingle) for the CsvLayer that will be added.
  ///   Dim theSimpleMarkerSymbol As ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol = New ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
  ///   theSimpleMarkerSymbol.Color = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red)
  ///   theSimpleMarkerSymbol.Size = 18
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Triangle
  ///   
  ///   ' Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   Dim theSimpleRenderer As ESRI.ArcGIS.Client.SimpleRenderer = New ESRI.ArcGIS.Client.SimpleRenderer
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol
  ///   
  ///   ' Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer
  ///   
  ///   ' Wire-up the Initialized Event of the CsvLayer.
  ///   AddHandler myCsvLayer.Initialized, AddressOf CsvLayer_Initialized
  ///   
  ///   ' Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer)
  ///   
  /// End Sub
  /// 
  /// Private Sub CsvLayer_Initialized(sender As Object, e As EventArgs)
  ///   
  ///   ' This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   ' Get the CsvLayer.
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
  ///   
  ///   ' Get the ID of the CsvLayer.
  ///   Dim myID As String = myCsvLayer.ID
  ///   
  ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   Dim myStringBuilder As New Text.StringBuilder
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + vbCrLf)
  ///   myStringBuilder.Append(myID + vbCrLf)
  ///   myStringBuilder.Append("====================================" + vbCrLf)
  ///   
  ///   ' Get the GraphicCollection from the CsvLayer.
  ///   Dim theGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = myCsvLayer.Graphics
  ///   If theGraphicCollection IsNot Nothing Then
  ///     
  ///     ' Loop through each Graphic.
  ///     For Each oneGraphic As ESRI.ArcGIS.Client.Graphic In theGraphicCollection
  ///       
  ///       ' Get the Attribute Keys. 
  ///       Dim theFieldNameKeys As System.Collections.Generic.ICollection(Of String) = oneGraphic.Attributes.Keys
  ///       
  ///       ' Loop through each Attribute.
  ///       For Each oneKey In theFieldNameKeys
  ///         
  ///         ' Get the value of the Attribute Field.
  ///         Dim theValue As String = oneGraphic.Attributes(oneKey)
  ///         
  ///         ' Get the Type of the Attribute Field.
  ///         Dim oneFieldType As String = oneGraphic.Attributes(oneKey).GetType.ToString
  ///         
  ///         ' Add the Attribute Field name, OS data type, and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString + "(" + oneFieldType + "): " + theValue + vbCrLf)
  ///         
  ///       Next
  ///       myStringBuilder.Append(vbCrLf)
  ///     Next
  ///     
  ///   End If
  ///   
  ///   ' Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString
  ///   
  /// End Sub
  /// </code>
  /// </example>
		public FieldCollection SourceFields
		{
            get { return (FieldCollection)GetValue(SourceFieldsProperty); }
			set { SetValue(SourceFieldsProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="SourceFields"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty SourceFieldsProperty =
            DependencyProperty.Register("SourceFields", typeof(FieldCollection), typeof(CsvLayer), new PropertyMetadata(null, OnSourceFieldsPropertyChanged));

        private static void OnSourceFieldsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset();
        }

  /// <summary>
  /// The name of the column header in the CSV file that contains the data used for the X coordinate.
  /// </summary>
  /// <remarks>
  /// <para>
  /// In order to make use of CSV files in the 
  /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
  /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
  ///  there should be point based spatial locational coordinate information for each record. This spatial information 
  ///  defines the <see cref="ESRI.ArcGIS.Client.Geometry.MapPoint">MapPoint</see> that will be used to construct a 
  ///  custom <see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see>. Other geography types like Polyline or 
  ///  Polygon are not supported for constructing a CsvLayer. Unless specified otherwise in the 
  ///  <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SourceSpatialReference">SourceSpatialReference</see> 
  ///  Property, it is assumed that the <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> 
  ///  of the CsvLayer has a WKID value of 4326. During the parsing process of reading the header record of the CSV file 
  ///  to construct the CsvLayer, any of the following names can be used to automatically detect the spatial location 
  ///  coordinate information:
  /// </para>
  /// <list type="table">  
  /// <listheader><term>Coordinate Type</term><description>Automatically detected Field names</description></listheader>  
  /// <item><term>LATITUDE</term><description>"lat", "latitude", "y", "ycenter", "latitude83", "latdecdeg", "point-y"</description></item>
  /// <item><term>LONGITUDE</term><description>"lon", "lng", "long", "longitude", "x", "xcenter", "longitude83", "longdecdeg", "point-x"</description></item>
  /// </list>
  /// <para>
  /// NOTE: The CsvLayer parsing algorithm for the Field names listed in the table above is case insensitive. 
  /// </para>
  /// <para>
  /// If the above spatial location coordinate field names are not specified in the header record, then it will be 
  /// required to specify them explicitly. Use the <b>XFieldName</b> Property to explicitly specify the Longitude coordinate
  /// and the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.YFieldName">YFieldName</see> Property to 
  /// explicitly specify the Latitude coordinate.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// Click the Button to add a CsvLayer to the Map. The ID of the layer and its attribute information will displayed 
  /// in the TextBox.
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following is an example of the ASCII contents for the file named US_Cities_Top_3.csv:<br/>
  /// ID,MySuperY,MySuperX,CityNameStateAbbr,Population<br/>
  /// 1,40.714,-74.006,"New York City, NY",8244910<br/>
  /// 2,34.0522,-118.244,"Los Angeles, CA",3819702<br/>
  /// 3,41.878,-87.636,"Chicago, IL",2708120<br/>
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Adding a CSV layer and displaying its attribute information." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.XFieldNameYFieldName.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
  ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
  ///         Margin="0,238,0,0" Height="350" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
  ///     
  ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
  ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
  ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
  ///   
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- Add a Button that will allow the user to add a CsvLayer via code-behind. --&gt;
  ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,209,0,0"  VerticalAlignment="Top" 
  ///           Width="706" Content="Add a CsvLayer (via code-behind) for the specified Url."
  ///           Click="Button1_Click" /&gt;
  ///   
  ///   &lt;!-- TextBox to display attribute information about about the CsvLayer added to the Map. --&gt;
  ///   &lt;TextBox Height="350" HorizontalAlignment="Left" Margin="421,238,0,0" Name="TextBox1" VerticalAlignment="Top" 
  ///            Width="285" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
  ///              TextWrapping="Wrap" Text="Click the Button to add a CsvLayer to the Map. The ID of the layer and 
  ///              its attribute information will displayed in the TextBox." /&gt;
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   // Create a CsvLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
  ///   
  ///   // Set the Url of the CsvLayer to a public service. 
  ///   // NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3.csv";
  ///   
  ///   // Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3";
  ///   
  ///   // Set the XFieldName and YFieldName Properties. This CSV file does not use standard X &amp; Y field names.
  ///   myCsvLayer.XFieldName = "MySuperX";
  ///   myCsvLayer.YFieldName = "MySuperY";
  ///   
  ///   // Create a SimpleMarkerSymbol (a black square) for the CsvLayer that will be added.
  ///   ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol theSimpleMarkerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol();
  ///   theSimpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
  ///   theSimpleMarkerSymbol.Size = 12;
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Square;
  ///   
  ///   // Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   ESRI.ArcGIS.Client.SimpleRenderer theSimpleRenderer = new ESRI.ArcGIS.Client.SimpleRenderer();
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol;
  ///   
  ///   // Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer;
  ///   
  ///   // Wire-up the Initialized Event of the CsvLayer. 
  ///   myCsvLayer.Initialized += CsvLayer_Initialized;
  ///   
  ///   // Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer);
  /// }
  /// 
  /// private void CsvLayer_Initialized(object sender, EventArgs e)
  /// {
  ///   // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   // Get the CsvLayer.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
  ///   
  ///   // Get the ID of the CsvLayer.
  ///   string myID = myCsvLayer.ID;
  ///   
  ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + Environment.NewLine);
  ///   myStringBuilder.Append(myID + Environment.NewLine);
  ///   myStringBuilder.Append("====================================" + Environment.NewLine);
  ///   
  ///   // Get the GraphicCollection from the CsvLayer.
  ///   ESRI.ArcGIS.Client.GraphicCollection theGraphicCollection = myCsvLayer.Graphics;
  ///   if (theGraphicCollection != null)
  ///   {
  ///     // Loop through each Graphic.
  ///     foreach (ESRI.ArcGIS.Client.Graphic oneGraphic in theGraphicCollection)
  ///     {
  ///       // Get the Attribute Keys. 
  ///       System.Collections.Generic.ICollection&lt;string&gt; theFieldNameKeys = oneGraphic.Attributes.Keys;
  ///       
  ///       // Loop through each Attribute.
  ///       foreach (var oneKey in theFieldNameKeys)
  ///       {
  ///         // Get the value of the Attribute Field.
  ///         string theValue = (string)oneGraphic.Attributes[oneKey];
  ///         
  ///         // Add the Attribute Field name and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString() + ": " + theValue + Environment.NewLine);
  ///       }
  ///     }
  ///   }
  ///   
  ///   // Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString();
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   ' Create a CsvLayer. 
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
  ///   
  ///   ' Set the Url of the CsvLayer to a public service. 
  ///   ' NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3.csv"
  ///   
  ///   ' Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3"
  ///   
  ///   ' Set the XFieldName and YFieldName Properties. This CSV file does not use standard X &amp; Y field names.
  ///   myCsvLayer.XFieldName = "MySuperX"
  ///   myCsvLayer.YFieldName = "MySuperY"
  ///   
  ///   ' Create a SimpleMarkerSymbol (a black square) for the CsvLayer that will be added.
  ///   Dim theSimpleMarkerSymbol As ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol = New ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
  ///   theSimpleMarkerSymbol.Color = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)
  ///   theSimpleMarkerSymbol.Size = 12
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Square
  ///   
  ///   ' Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   Dim theSimpleRenderer As ESRI.ArcGIS.Client.SimpleRenderer = New ESRI.ArcGIS.Client.SimpleRenderer
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol
  ///   
  ///   ' Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer
  ///   
  ///   ' Wire-up the Initialized Event of the CsvLayer.
  ///   AddHandler myCsvLayer.Initialized, AddressOf CsvLayer_Initialized
  ///   
  ///   ' Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer)
  ///   
  /// End Sub
  /// 
  /// Private Sub CsvLayer_Initialized(sender As Object, e As EventArgs)
  ///   
  ///   ' This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   ' Get the CsvLayer.
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
  ///   
  ///   ' Get the ID of the CsvLayer.
  ///   Dim myID As String = myCsvLayer.ID
  ///   
  ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   Dim myStringBuilder As New Text.StringBuilder
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + vbCrLf)
  ///   myStringBuilder.Append(myID + vbCrLf)
  ///   myStringBuilder.Append("====================================" + vbCrLf)
  ///   
  ///   ' Get the GraphicCollection from the CsvLayer.
  ///   Dim theGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = myCsvLayer.Graphics
  ///   If theGraphicCollection IsNot Nothing Then
  ///     
  ///     ' Loop through each Graphic.
  ///     For Each oneGraphic As ESRI.ArcGIS.Client.Graphic In theGraphicCollection
  ///       
  ///       ' Get the Attribute Keys. 
  ///       Dim theFieldNameKeys As System.Collections.Generic.ICollection(Of String) = oneGraphic.Attributes.Keys
  ///       
  ///       ' Loop through each Attribute.
  ///       For Each oneKey In theFieldNameKeys
  ///         
  ///         ' Get the value of the Attribute Field.
  ///         Dim theValue As String = oneGraphic.Attributes(oneKey)
  ///         
  ///         ' Add the Attribute Field name and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString + ": " + theValue + vbCrLf)
  ///         
  ///       Next
  ///       
  ///     Next
  ///     
  ///   End If
  ///   
  ///   ' Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString
  ///   
  /// End Sub
  /// </code>
  /// </example>
  public string XFieldName
		{
			get { return (string)GetValue(XFieldNameProperty); }
			set { SetValue(XFieldNameProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="XFieldName"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty XFieldNameProperty =
			DependencyProperty.Register("XFieldName", typeof(string), typeof(CsvLayer), new PropertyMetadata(null,OnXFieldNamePropertyChanged));

        private static void OnXFieldNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset(); 
        }


  /// <summary>
  /// The name of the column header in the CSV file that contains the data used for the Y coordinate.
  /// </summary>
  /// <remarks>
  /// <para>
  /// In order to make use of CSV files in the 
  /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
  /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
  ///  there should be point based spatial locational coordinate information for each record. This spatial information 
  ///  defines the <see cref="ESRI.ArcGIS.Client.Geometry.MapPoint">MapPoint</see> that will be used to construct a 
  ///  custom <see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see>. Other geography types like Polyline or 
  ///  Polygon are not supported for constructing a CsvLayer. Unless specified otherwise in the 
  ///  <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SourceSpatialReference">SourceSpatialReference</see> 
  ///  Property, it is assumed that the <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> 
  ///  of the CsvLayer has a WKID value of 4326. During the parsing process of reading the header record of the CSV file 
  ///  to construct the CsvLayer, any of the following names can be used to automatically detect the spatial location 
  ///  coordinate information:
  /// </para>
  /// <list type="table">  
  /// <listheader><term>Coordinate Type</term><description>Automatically detected Field names</description></listheader>  
  /// <item><term>LATITUDE</term><description>"lat", "latitude", "y", "ycenter", "latitude83", "latdecdeg", "point-y"</description></item>
  /// <item><term>LONGITUDE</term><description>"lon", "lng", "long", "longitude", "x", "xcenter", "longitude83", "longdecdeg", "point-x"</description></item>
  /// </list>
  /// <para>
  /// NOTE: The CsvLayer parsing algorithm for the Field names listed in the table above is case insensitive. 
  /// </para>
  /// <para>
  /// If the above spatial location coordinate field names are not specified in the header record, then it will be 
  /// required to specify them explicitly. Use the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.XFieldName">XFieldName</see> Property to explicitly 
  /// specify the Longitude coordinate and the <b>YFieldName</b> Property to explicitly specify the Latitude coordinate.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// Click the Button to add a CsvLayer to the Map. The ID of the layer and its attribute information will displayed 
  /// in the TextBox.
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following is an example of the ASCII contents for the file named US_Cities_Top_3.csv:<br/>
  /// ID,MySuperY,MySuperX,CityNameStateAbbr,Population<br/>
  /// 1,40.714,-74.006,"New York City, NY",8244910<br/>
  /// 2,34.0522,-118.244,"Los Angeles, CA",3819702<br/>
  /// 3,41.878,-87.636,"Chicago, IL",2708120<br/>
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Adding a CSV layer and displaying its attribute information." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.XFieldNameYFieldName.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
  ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
  ///         Margin="0,238,0,0" Height="350" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
  ///     
  ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
  ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
  ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
  ///   
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- Add a Button that will allow the user to add a CsvLayer via code-behind. --&gt;
  ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,209,0,0"  VerticalAlignment="Top" 
  ///           Width="706" Content="Add a CsvLayer (via code-behind) for the specified Url."
  ///           Click="Button1_Click" /&gt;
  ///   
  ///   &lt;!-- TextBox to display attribute information about about the CsvLayer added to the Map. --&gt;
  ///   &lt;TextBox Height="350" HorizontalAlignment="Left" Margin="421,238,0,0" Name="TextBox1" VerticalAlignment="Top" 
  ///            Width="285" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
  ///              TextWrapping="Wrap" Text="Click the Button to add a CsvLayer to the Map. The ID of the layer and 
  ///              its attribute information will displayed in the TextBox." /&gt;
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   // Create a CsvLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
  ///   
  ///   // Set the Url of the CsvLayer to a public service. 
  ///   // NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3.csv";
  ///   
  ///   // Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3";
  ///   
  ///   // Set the XFieldName and YFieldName Properties. This CSV file does not use standard X &amp; Y field names.
  ///   myCsvLayer.XFieldName = "MySuperX";
  ///   myCsvLayer.YFieldName = "MySuperY";
  ///   
  ///   // Create a SimpleMarkerSymbol (a black square) for the CsvLayer that will be added.
  ///   ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol theSimpleMarkerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol();
  ///   theSimpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
  ///   theSimpleMarkerSymbol.Size = 12;
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Square;
  ///   
  ///   // Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   ESRI.ArcGIS.Client.SimpleRenderer theSimpleRenderer = new ESRI.ArcGIS.Client.SimpleRenderer();
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol;
  ///   
  ///   // Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer;
  ///   
  ///   // Wire-up the Initialized Event of the CsvLayer. 
  ///   myCsvLayer.Initialized += CsvLayer_Initialized;
  ///   
  ///   // Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer);
  /// }
  /// 
  /// private void CsvLayer_Initialized(object sender, EventArgs e)
  /// {
  ///   // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   // Get the CsvLayer.
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
  ///   
  ///   // Get the ID of the CsvLayer.
  ///   string myID = myCsvLayer.ID;
  ///   
  ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + Environment.NewLine);
  ///   myStringBuilder.Append(myID + Environment.NewLine);
  ///   myStringBuilder.Append("====================================" + Environment.NewLine);
  ///   
  ///   // Get the GraphicCollection from the CsvLayer.
  ///   ESRI.ArcGIS.Client.GraphicCollection theGraphicCollection = myCsvLayer.Graphics;
  ///   if (theGraphicCollection != null)
  ///   {
  ///     // Loop through each Graphic.
  ///     foreach (ESRI.ArcGIS.Client.Graphic oneGraphic in theGraphicCollection)
  ///     {
  ///       // Get the Attribute Keys. 
  ///       System.Collections.Generic.ICollection&lt;string&gt; theFieldNameKeys = oneGraphic.Attributes.Keys;
  ///       
  ///       // Loop through each Attribute.
  ///       foreach (var oneKey in theFieldNameKeys)
  ///       {
  ///         // Get the value of the Attribute Field.
  ///         string theValue = (string)oneGraphic.Attributes[oneKey];
  ///         
  ///         // Add the Attribute Field name and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString() + ": " + theValue + Environment.NewLine);
  ///       }
  ///     }
  ///   }
  ///   
  ///   // Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString();
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   ' Create a CsvLayer. 
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
  ///   
  ///   ' Set the Url of the CsvLayer to a public service. 
  ///   ' NOTE: you need to adjust the Url to a .csv file served up on you test web server.
  ///   myCsvLayer.Url = "http://www.yourserver.com/CSV_Files/US_Cities_Top_3.csv"
  ///   
  ///   ' Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_3"
  ///   
  ///   ' Set the XFieldName and YFieldName Properties. This CSV file does not use standard X &amp; Y field names.
  ///   myCsvLayer.XFieldName = "MySuperX"
  ///   myCsvLayer.YFieldName = "MySuperY"
  ///   
  ///   ' Create a SimpleMarkerSymbol (a black square) for the CsvLayer that will be added.
  ///   Dim theSimpleMarkerSymbol As ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol = New ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
  ///   theSimpleMarkerSymbol.Color = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)
  ///   theSimpleMarkerSymbol.Size = 12
  ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Square
  ///   
  ///   ' Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
  ///   Dim theSimpleRenderer As ESRI.ArcGIS.Client.SimpleRenderer = New ESRI.ArcGIS.Client.SimpleRenderer
  ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol
  ///   
  ///   ' Define the Renderer for the CsvLayer.
  ///   myCsvLayer.Renderer = theSimpleRenderer
  ///   
  ///   ' Wire-up the Initialized Event of the CsvLayer.
  ///   AddHandler myCsvLayer.Initialized, AddressOf CsvLayer_Initialized
  ///   
  ///   ' Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer)
  ///   
  /// End Sub
  /// 
  /// Private Sub CsvLayer_Initialized(sender As Object, e As EventArgs)
  ///   
  ///   ' This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
  ///   
  ///   ' Get the CsvLayer.
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
  ///   
  ///   ' Get the ID of the CsvLayer.
  ///   Dim myID As String = myCsvLayer.ID
  ///   
  ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
  ///   Dim myStringBuilder As New Text.StringBuilder
  ///   myStringBuilder.Append("The CsvLayer and it's ID is: " + vbCrLf)
  ///   myStringBuilder.Append(myID + vbCrLf)
  ///   myStringBuilder.Append("====================================" + vbCrLf)
  ///   
  ///   ' Get the GraphicCollection from the CsvLayer.
  ///   Dim theGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = myCsvLayer.Graphics
  ///   If theGraphicCollection IsNot Nothing Then
  ///     
  ///     ' Loop through each Graphic.
  ///     For Each oneGraphic As ESRI.ArcGIS.Client.Graphic In theGraphicCollection
  ///       
  ///       ' Get the Attribute Keys. 
  ///       Dim theFieldNameKeys As System.Collections.Generic.ICollection(Of String) = oneGraphic.Attributes.Keys
  ///       
  ///       ' Loop through each Attribute.
  ///       For Each oneKey In theFieldNameKeys
  ///         
  ///         ' Get the value of the Attribute Field.
  ///         Dim theValue As String = oneGraphic.Attributes(oneKey)
  ///         
  ///         ' Add the Attribute Field name and Attribute value to the StringBuilder object.
  ///         myStringBuilder.Append(oneKey.ToString + ": " + theValue + vbCrLf)
  ///         
  ///       Next
  ///       
  ///     Next
  ///     
  ///   End If
  ///   
  ///   ' Display the results of the StringBuilder text to the user.
  ///   TextBox1.Text = myStringBuilder.ToString
  ///   
  /// End Sub
  /// </code>
  /// </example>
  public string YFieldName
		{
			get { return (string)GetValue(YFieldNameProperty); }
			set { SetValue(YFieldNameProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="YFieldName"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty YFieldNameProperty =
			DependencyProperty.Register("YFieldName", typeof(string), typeof(CsvLayer), new PropertyMetadata(null,OnYFieldNamePropertyChanged));

        private static void OnYFieldNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset(); 
        }


  /// <summary>
  /// The column delimiter used to split the columns of the CSV file. The default value is comma.
  /// </summary>
  /// <remarks>
  /// <para>
  /// By default it is assumed that the delimiter for the CSV file in parsing data values for between fields is the 
  /// comma (,). If another delimiter is used (for example a tab or dash) it is required to specify the ColumnDelimiter
  /// Property. If the comma is used as the delimiter in the CSV file the <b>ColumnDelimiter</b> Property does not need to 
  /// be set.
  /// </para>
  /// </remarks>
		public string ColumnDelimiter
		{
			get { return (string)GetValue(ColumnDelimiterProperty); }
			set { SetValue(ColumnDelimiterProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="ColumnDelimiter"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty ColumnDelimiterProperty =
			DependencyProperty.Register("ColumnDelimiter", typeof(string), typeof(CsvLayer), new PropertyMetadata(",",OnColumnDelimiterPropertyChanged));

        private static void OnColumnDelimiterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset(); 
        }


  /// <summary>
  /// Url location to CSV file.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The CSV file format is tabular data in plain text. The data in the CSV file consists of fields of data separated 
  /// by a delimiting character (typically a comma) for a record. The first record in the CSV file is known as the 
  /// header and defines the names of each field of the tabular data. The second through last row of records in the 
  /// CSV file is the actual tabular data. When the delimiter (typically a comma) is embedded in the tabular data 
  /// for a particular field, that value should be encased in quotes to avoid parsing errors. Each record in the 
  /// CSV file should contain the same number of fields. Numerous applications including the Microsoft Excel Office 
  /// product can export and import CSV files. It is not required that a CSV source file contain the extension .csv; 
  /// the file can contain any extension (ex: .txt) or none at all.
  /// </para>
  /// <para>
  /// If it is desired to obtain the CSV data from a 
  /// <a href="http://msdn.microsoft.com/en-us/library/system.io.stream(v=vs.100).aspx" target="_blank">Stream</a> 
  /// rather than the <b>Url</b> Property use the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SetSource">SetSource</see> 
  /// Method.
  /// </para>
  /// <ESRISILVERLIGHT><para>Although ArcGIS Server is not required to host a CSV file web service you may experience the similar hosting issues of accessing the web service as described in the ArcGIS Resource Center blog entitled <a href="http://blogs.esri.com/esri/arcgis/2009/08/24/troubleshooting-blank-layers/">Troubleshooting blank layers</a>. Specifically, you may need to make sure that a correct <a href="http://msdn.microsoft.com/EN-US/LIBRARY/CC197955(VS.95).ASPX" target="_blank">cilentaccesspolicy.xml or crossdomain.xml</a> file is in place on the web servers root. If a clientaccesspolicy.xml or crossdomainpolicy.xml file cannot be used on your web server for situations like <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/index.html#/Secure_services/016600000022000000/">secure services</a> you may need to use a <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/0166/other/SLProxyPage.zip">proxy page</a> on your web server and make use of the <b>CsvLayer.ProxyUrl</b> Property.</para></ESRISILVERLIGHT>
  /// <para>
  /// The bare minimum settings that need to be specified to create and display a CsvLayer in a Map are the 
  /// <b>Url</b> and <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Url methadology) OR 
  /// the <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.SetSource">SetSource</see> and 
  /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Stream methodology). 
  /// NOTE: This assumes that default spatial coordinate information field names are used and the delimiter for 
  /// the CSV file is a comma.
  /// </para>
  /// <para>
  /// There are several methods to construct a Url for accessing data in a CSV layer. The example code in the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer">CsvLayer</see> Class documentation shows how to access 
  /// a CSV file via on a web server and uses the 'http://' keyword to construct a Url. The example code in the 
  /// <b>CsvLayer.Url</b> Property documentation shows 
  /// how to access a CSV file as a resource on the local disk in a Visual Studio project using the 
  /// '[Visual_Studio_Project_Name]' and the 'component' keywords. Even more options are available such as constructing 
  /// a Url using the 'file://' or 'pack://' keywords. The development platform you are coding in will determine which 
  /// style of Url is appropriate. See the documentation for your particular development platform to decide which type 
  /// of string can be used in the the Url construction.
  /// </para>
  /// </remarks>
 /// <example>
 /// <para>
 /// <b>How to use:</b>
 /// </para>
 /// <para>
 /// When the application loads a CsvLayer will automatically be added to the Map (it was specified in XAML). 
 /// Click the Button to add another CsvLayer to the Map (it will be added via code-behind). The ID of each 
 /// layer will displayed in the TextBox. NOTE: the CSV files are accessed via Resource files on disk in the 
 /// Visual Studio project rather that an http:// web service.
 /// </para>
 /// <para>
 /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
 /// the functionality.
 /// </para>
 /// <para>
 /// SPECIAL INSTRUCTIONS: The name of the sample Visual Studio project in this code example is "TestProject".
 /// Additionally a folder named "myFolder" was added to "TestProject". Place the "US_Cities_Top_5.csv" and 
 /// the "US_Cities_6_to_10.csv" files in the "myFolder" location. Make sure that the for the Properties of the 
 /// "US_Cities_Top_5.csv" and "US_Cities_6_to_10.csv" files that the 'Build Action' is set to 'Resource'.
 /// </para>
 /// <para>
 /// <img border="0" alt="Adding the US_Cities_Top_5.csv and US_Cities_6_to_10.csv files as a Resources to the Visual Studio Proect named 'TestProject' in the 'myFolder' location." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.Url1.png"/>
 /// </para>
 /// <para>
 /// The following is an example of the ASCII contents for the file named US_Cities_Top_5.csv:<br/>
 /// ID,Lat,Long,CityName,Population<br/>
 /// 1,40.714,-74.006,New York City,8244910<br/>
 /// 2,34.0522,-118.244,Los Angeles,3819702<br/>
 /// 3,41.878,-87.636,Chicago,2708120<br/>
 /// 4,29.763,-95.363,Houston,2099451<br/>
 /// 5,39.952,-75.168,Philadelphia,1526006<br/>
 /// </para>
 /// <para>
 /// The following is an example of the ASCII contents for the file named US_Cities_6_to_10.csv:<br/>
 /// ID,Lat,Long,CityName,Population<br/>
 /// 6,29.423,-98.493,San Antonio,1327407<br/>
 /// 7,32.715,-117.156,San Diego,1326179<br/>
 /// 8,32.782,-96.815,Dallas,1223229<br/>
 /// 9,37.228,-119.228,San Jose,945942<br/>
 /// 10,30.331,-81.655,Jacksonville,821784<br/>
 /// </para>
 /// <para>
 /// The following screen shot corresponds to the code example in this page.
 /// </para>
 /// <para>
 /// <img border="0" alt="Adding a CsvLayer in XAML and code-behind when the CSV files are resources in the Visual Studio project." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.Url.png"/>
 /// </para>
 /// <code title="Example XAML1" description="" lang="XAML">
 /// &lt;Grid x:Name="LayoutRoot" &gt;
 ///   
 ///   &lt;!-- Add local Resources to define a SimpleRender to display aqua diamonds as a SimpleMarkerSymbol 
 ///   for the CsvLayer. --&gt;
 ///   &lt;Grid.Resources&gt;
 ///     &lt;esri:SimpleRenderer x:Key="myRenderer"&gt;
 ///       &lt;esri:SimpleRenderer.Symbol&gt;
 ///         &lt;esri:SimpleMarkerSymbol Color="Red" Size="12" Style="Circle" /&gt;
 ///       &lt;/esri:SimpleRenderer.Symbol&gt;
 ///     &lt;/esri:SimpleRenderer&gt;
 ///   &lt;/Grid.Resources&gt;
 ///   
 ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
 ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
 ///         Margin="0,238,0,0" Height="350" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
 ///   
 ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
 ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
 ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
 ///     
 ///     &lt;!-- Add a CsvLayer. The renderer symbology will be based upon the local static resource defined above.
 ///          VERY IMPORTANT: 
 ///          Replace the first parameter argument of the Url with the correct string to the location of the CSV 
 ///          file relative to your test project. In this example the Visual Studio project name is: "TestProject".
 ///          Additionally, a folder was added to "TestProject" called "myFolder" and this is where the 
 ///          US_Cities_Top_5.csv file is located. Finally, make sure that the for the Properties of the 
 ///          US_Cities_Top_5.csv file that the 'Build Action' is set to 'Resource'.
 ///     --&gt;
 ///     &lt;esri:CsvLayer ID="US_Cities_Top_5" Url="/TestProject;component/myFolder/US_Cities_Top_5.csv"
 ///           Renderer="{StaticResource myRenderer}" Initialized="CsvLayer_Initialized"/&gt;
 ///   &lt;/esri:Map&gt;
 ///   
 ///   &lt;!-- Add a Button that will allow the user to add another CsvLayer via code-behind. --&gt;
 ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,209,0,0"  VerticalAlignment="Top" 
 ///           Width="706" Content="Add another CsvLayer (via code-behind) for the specified Url."
 ///           Click="Button1_Click" /&gt;
 ///   
 ///   &lt;!-- TextBox to display information about about the CsvLayerLayers added to the Map. --&gt;
 ///   &lt;TextBox Height="350" HorizontalAlignment="Left" Margin="421,238,0,0" Name="TextBox1" VerticalAlignment="Top" 
 ///            Width="285" /&gt;
 ///   
 ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
 ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
 ///              TextWrapping="Wrap" Text="When the application loads a CsvLayer will automatically be added 
 ///              to the Map (it was specified in XAML). Click the Button to add another CsvLayer to the Map 
 ///              (it will be added via code-behind). The ID of each layer will displayed in the TextBox. 
 ///              NOTE: the CSV files are accessed via Resource files on disk in the Visual Studio project 
 ///              rather that an http:// web service." /&gt;
 /// &lt;/Grid&gt;
 /// </code>
 /// <code title="Example CS1" description="" lang="CS">
 /// private void CsvLayer_Initialized(object sender, System.EventArgs e)
 /// {
 ///   // This function will execute as a result of the CsvLayer that was defined in XAML being Initialized.
 ///   
 ///   // Get the CsvLayer.
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
 ///   
 ///   // Get the ID of the CsvLayer.
 ///   string myID = myCsvLayer.ID;
 ///   
 ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
 ///   myStringBuilder.Append("The 1st CsvLayer was" + Environment.NewLine);
 ///   myStringBuilder.Append("added via XAML and it's ID is: " + Environment.NewLine);
 ///   myStringBuilder.Append(myID);
 ///   
 ///   // Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString();
 /// }
 /// 
 /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
 /// {
 ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
 ///   
 ///   // Create a CsvLayer. 
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer2 = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
 ///   
 ///   // Set the Url of the CsvLayer to a public service. 
 ///   // VERY IMPORTANT: 
 ///   // Replace the first parameter argument of the Url with the correct string to the location of the CSV 
 ///   // file relative to your test project. In this example the Visual Studio project name is: "TestProject".
 ///   // Additionally, a folder was added to "TestProject" called "myFolder" and this is where the US_Cities_6_to_10.csv 
 ///   // file is located. Finally, make sure that the for the Properties of the US_Cities_6_to_10.csv file that the 
 ///   // 'Build Action' is set to 'Resource'.
 ///   myCsvLayer2.Url = "/TestProject;component/myFolder/US_Cities_6_to_10.csv";
 ///   
 ///   // Set the ID of the CsvLayer.
 ///   myCsvLayer2.ID = "US_Cities_6_to_10";
 ///   
 ///   // Create a SimpleMarkerSymbol (a purple diamond) for the CsvLayer that will be added.
 ///   ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol theSimpleMarkerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol();
 ///   theSimpleMarkerSymbol.Color = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Purple);
 ///   theSimpleMarkerSymbol.Size = 12;
 ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Diamond;
 ///   
 ///   // Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
 ///   ESRI.ArcGIS.Client.SimpleRenderer theSimpleRenderer = new ESRI.ArcGIS.Client.SimpleRenderer();
 ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol;
 ///   
 ///   // Define the Renderer for the CsvLayer.
 ///   myCsvLayer2.Renderer = theSimpleRenderer;
 ///   
 ///   // Wire-up the Initialized Event of the CsvLayer. Note how a different Initilized Event is being used verses 
 ///   // the one defined in XAML. They could share the same Initialized Event but we created two seperate ones 
 ///   // for demonstration purposes.
 ///   myCsvLayer2.Initialized += CsvLayer_Initialized2;
 ///   
 ///   // Add the CsvLayer to the Map.
 ///   Map1.Layers.Add(myCsvLayer2);
 /// }
 /// 
 /// private void CsvLayer_Initialized2(object sender, EventArgs e)
 /// {
 ///   // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
 ///   
 ///   // Get the CsvLayer.
 ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer2 = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;
 ///   
 ///   // Get the ID of the CsvLayer.
 ///   string myID2 = myCsvLayer2.ID;
 ///   
 ///   // Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   System.Text.StringBuilder myStringBuilder = new System.Text.StringBuilder();
 ///   myStringBuilder.Append(TextBox1.Text + Environment.NewLine);
 ///   myStringBuilder.Append(Environment.NewLine);
 ///   myStringBuilder.Append("The 2nd CsvLayer was" + Environment.NewLine);
 ///   myStringBuilder.Append("added via code-behind and it's ID is: " + Environment.NewLine);
 ///   myStringBuilder.Append(myID2);
 ///   
 ///   // Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString();
 /// }
 /// </code>
 /// <code title="Example VB1" description="" lang="VB.NET">
 /// Private Sub CsvLayer_Initialized(sender As System.Object, e As System.EventArgs)
 ///   
 ///   ' This function will execute as a result of the CsvLayer that was defined in XAML being Initialized.
 ///   
 ///   ' Get the CsvLayer.
 ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
 ///   
 ///   ' Get the ID of the CsvLayer.
 ///   Dim myID As String = myCsvLayer.ID
 ///   
 ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   Dim myStringBuilder As New Text.StringBuilder
 ///   myStringBuilder.Append("The 1st CsvLayer was" + vbCrLf)
 ///   myStringBuilder.Append("added via XAML and it's ID is: " + vbCrLf)
 ///   myStringBuilder.Append(myID)
 ///   
 ///   ' Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString
 ///   
 /// End Sub
 /// 
 /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
 ///   
 ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
 ///   
 ///   ' Create a CsvLayer. 
 ///   Dim myCsvLayer2 As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
 ///   
 ///   ' Set the Url of the CsvLayer to a public service. 
 ///   ' VERY IMPORTANT: 
 ///   ' Replace the first parameter argument of the Url with the correct string to the location of the CSV 
 ///   ' file relative to your test project. In this example the Visual Studio project name is: "TestProject".
 ///   ' Additionally, a folder was added to "TestProject" called "myFolder" and this is where the US_Cities_6_to_10.csv 
 ///   ' file is located. Finally, make sure that the for the Properties of the US_Cities_6_to_10.csv file that the 
 ///   ' 'Build Action' is set to 'Resource'.
 ///   myCsvLayer2.Url = "/TestProject;component/myFolder/US_Cities_6_to_10.csv"
 ///   
 ///   ' Set the ID of the CsvLayer.
 ///   myCsvLayer2.ID = "US_Cities_6_to_10"
 ///   
 ///   ' Create a SimpleMarkerSymbol (a purple diamond) for the CsvLayer that will be added.
 ///   Dim theSimpleMarkerSymbol As ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol = New ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
 ///   theSimpleMarkerSymbol.Color = New System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green)
 ///   theSimpleMarkerSymbol.Size = 12
 ///   theSimpleMarkerSymbol.Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
 ///   
 ///   ' Define a SimpleRenderer and set the Symbol to the SimpleMarkerSymbol.
 ///   Dim theSimpleRenderer As ESRI.ArcGIS.Client.SimpleRenderer = New ESRI.ArcGIS.Client.SimpleRenderer
 ///   theSimpleRenderer.Symbol = theSimpleMarkerSymbol
 ///   
 ///   ' Define the Renderer for the CsvLayer.
 ///   myCsvLayer2.Renderer = theSimpleRenderer
 ///   
 ///   ' Wire-up the Initialized Event of the CsvLayer. Note how a different Initilized Event is being used verses 
 ///   ' the one defined in XAML. They could share the same Initialized Event but we created two seperate ones 
 ///   ' for demonstration purposes.
 ///   AddHandler myCsvLayer2.Initialized, AddressOf CsvLayer_Initialized2
 ///   
 ///   ' Add the CsvLayer to the Map.
 ///   Map1.Layers.Add(myCsvLayer2)
 ///   
 /// End Sub
 /// 
 /// Private Sub CsvLayer_Initialized2(sender As Object, e As EventArgs)
 ///   
 ///   ' This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.
 ///   
 ///   ' Get the CsvLayer.
 ///   Dim myCsvLayer2 As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = CType(sender, ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)
 ///   
 ///   ' Get the ID of the CsvLayer.
 ///   Dim myID2 As String = myCsvLayer2.ID
 ///   
 ///   ' Create a StringBuilder object to hold information about the CsvLayer and add some useful information to it.
 ///   Dim myStringBuilder As New Text.StringBuilder
 ///   myStringBuilder.Append(TextBox1.Text + vbCrLf)
 ///   myStringBuilder.Append(vbCrLf)
 ///   myStringBuilder.Append("The 2nd CsvLayer was" + vbCrLf)
 ///   myStringBuilder.Append("added via code-behind and it's ID is: " + vbCrLf)
 ///   myStringBuilder.Append(myID2)
 ///   
 ///   ' Display the results of the StringBuilder text to the user.
 ///   TextBox1.Text = myStringBuilder.ToString
 ///   
 /// End Sub
 /// </code>
 /// </example>
 public string Url
		{
			get { return (string)GetValue(UrlProperty); }
			set { SetValue(UrlProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="Url"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty UrlProperty =
			DependencyProperty.Register("Url", typeof(string), typeof(CsvLayer), new PropertyMetadata(null,OnUrlPropertyChanged));

        private static void OnUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset();            
        }


  /// <summary>
  /// The spatial reference of the CSV data.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Unless specified otherwise in the <b>SouceSpatialReference</b> Property, it is assumed that the 
  /// <see cref="ESRI.ArcGIS.Client.Geometry.SpatialReference">SpatialReference</see> of the CsvLayer 
  /// has a WKID value of 4326.
  /// </para>
  /// </remarks>
		public SpatialReference SourceSpatialReference
		{
			get { return (SpatialReference)GetValue(SourceSpatialReferenceProperty); }
			set { SetValue(SourceSpatialReferenceProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="SourceSpatialReference"/> dependency property. The default value is WKID (4326).
        /// </summary>
		public static readonly DependencyProperty SourceSpatialReferenceProperty =
			DependencyProperty.Register("SourceSpatialReference", typeof(SpatialReference), typeof(CsvLayer), new PropertyMetadata(null,OnSourceSpatialReferenceChanged));

        private static void OnSourceSpatialReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset(); 
        }

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
            System.Windows.DependencyProperty.Register("Credentials", typeof(System.Net.ICredentials), typeof(CsvLayer), new PropertyMetadata(OnCredentialsPropertyChanged));

        private static void OnCredentialsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset();             
        }
#endif
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
            DependencyProperty.Register("ClientCertificate", typeof(System.Security.Cryptography.X509Certificates.X509Certificate), typeof(CsvLayer), null);
#endif

#if SILVERLIGHT && !WINDOWS_PHONE

  /// <summary>
  /// The proxy url location.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Although ArcGIS Server is not required to host a CSV file web service you may experience the similar hosting 
  /// issues of accessing the web service as described in the ArcGIS Resource Center blog entitled 
  /// <a href="http://blogs.esri.com/esri/arcgis/2009/08/24/troubleshooting-blank-layers/">Troubleshooting blank layers</a>. 
  /// Specifically, you may need to make sure that a correct 
  /// <a href="http://msdn.microsoft.com/EN-US/LIBRARY/CC197955(VS.95).ASPX" target="_blank">cilentaccesspolicy.xml or crossdomain.xml</a> 
  /// file is in place on the web servers root. If a clientaccesspolicy.xml or crossdomainpolicy.xml file cannot be 
  /// used on your web server for situations like 
  /// <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/index.html#/Secure_services/016600000022000000/">secure services</a> 
  /// you may need to use a 
  /// <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/0166/other/SLProxyPage.zip">proxy page</a> 
  /// on your web server and make use of the <b>ProxyUrl</b> Property.
  /// </para>
  /// </remarks>
		public string ProxyUrl
		{
			get { return (string)GetValue(ProxyUrlProperty); }
			set { SetValue(ProxyUrlProperty, value); }
		}

        /// <summary>
        /// Identifies the <see cref="ProxyUrl"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty ProxyUrlProperty =
			DependencyProperty.Register("ProxyUrl", typeof(string), typeof(CsvLayer), new PropertyMetadata(null,OnProxyUrlPropertyChanged));

        private static void OnProxyUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CsvLayer csvLayer = d as CsvLayer;
            if (csvLayer.IsInitialized)
                csvLayer.Reset();   
        }
#endif

		#endregion Public Members

		#region Private Methods

        /// <summary>
        /// Clears the graphics and reloads the CSV data.
        /// </summary>
        private void Reset()
        {
            Action<GraphicCollection> callback;
            callback = (collection) =>  
            {                            
                this.Graphics = collection; 
            };
            GetGraphics(callback);
        }

        /// <summary>
        /// This will start the process to find the CSV data, process the data, and convert into graphics.
        /// </summary>
        /// <param name="onCompleted">The callback handler that will return a collection of graphics for the layer.</param>
		private void GetGraphics(Action<GraphicCollection> onCompleted)
		{
			Action<List<string[]>> callback = new Action<List<string[]>>((csvData) =>
			{
				CreateGraphics(csvData, onCompleted);
			});
			CsvParser(callback);
		}

        /// <summary>
        /// Creates a GraphicCollection from a list of string array containing rows of CSV data.
        /// </summary>
        /// <param name="csvData">list of string array containing multiple rows of CSV data.</param>
        /// <param name="onCompleted">Callback that will return the GraphicCollection after all the CSV data is parsed into Graphic objects</param>
		private void CreateGraphics(List<string[]> csvData, Action<GraphicCollection> onCompleted)
		{
			if (csvData == null && onCompleted != null)
				onCompleted(null);

            numericCulture = GetCsvNumericCulture(csvData);

			GraphicCollection graphics = new GraphicCollection();
			if (csvData != null && csvData.Count > 0)
			{
				try
				{
					string[] headers = csvData[0];
					for (int i = 1; i < csvData.Count(); i++)
					{
						double x = double.NaN, y = double.NaN;
						string[] row = csvData[i];
						Graphic g = new Graphic();
						for (int j = 0; j < row.Count(); j++)
						{
							if ((!string.IsNullOrEmpty(row[j])) && double.IsNaN(x) && ((!string.IsNullOrEmpty(XFieldName) && XFieldName == headers[j]) ||
								(string.IsNullOrEmpty(XFieldName) && LON_FIELDS.Contains(headers[j].ToLowerInvariant()))))
							{
                	            double.TryParse(row[j], NumberStyles.Number | NumberStyles.AllowExponent, numericCulture, out x);                            
							}
							else if ((!string.IsNullOrEmpty(row[j])) && double.IsNaN(y) && ((!string.IsNullOrEmpty(YFieldName) && YFieldName == headers[j]) ||
								(string.IsNullOrEmpty(YFieldName) && LAT_FIELDS.Contains(headers[j].ToLowerInvariant()))))
							{
        	                    double.TryParse(row[j], NumberStyles.Number | NumberStyles.AllowExponent, numericCulture, out y);                            
							}

							object value = row[j];
                    	    if (SourceFields == null || SourceFields.Count == 0)
                        	    g.Attributes[headers[j]] = row[j];
	                        else
    	                    {
        	                    var field = SourceFields != null ? SourceFields.FirstOrDefault(f => f.Name == headers[j]) : null;
            	                if (field != null && field.Type != Field.FieldType.String)
                	                value = ConvertToExpectedType(row[j], field.Type);
                    	        if (field != null)
                        	        g.Attributes[field.FieldName] = value;
	                        }
						}
						if (!double.IsNaN(x) && !double.IsNaN(y))
							g.Geometry = new MapPoint(x, y, SourceSpatialReference);
						graphics.Add(g);
					}
				}
				catch (Exception ex)
                {
                    InitializationFailure = new FormatException(Properties.Resources.CsvLayer_Parsing_FormatException, ex);
                    graphics = null;
                }
			}
			if (onCompleted != null)
				onCompleted.Invoke(graphics);
		}

        /// <summary>
        /// This will attempt to download or retrieve the CSV file from various locations based on what the platform supports. The if CSV data
        /// is found it will be read and returned as a list of string array.
        /// </summary>
        /// <param name="callback">Calback that will return list of string array containing parsed CSV data</param>
		private void CsvParser(Action<List<string[]>> callback)
		{
            WebClient client = Utilities.CreateWebClient();
#if !SILVERLIGHT
            if (ClientCertificate != null)
                (client as CompressResponseWebClient).ClientCertificate = ClientCertificate;
#endif
#if !SILVERLIGHT || WINDOWS_PHONE
            client.Credentials = Credentials;
#endif
			client.OpenReadCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					SetStream(e.Result, callback);
				}
				else
				{
                    if(InitializationFailure == null)
                        InitializationFailure = e.Error;
					if (callback != null)
						callback(null);
				}
			};
			try
			{
                if (string.IsNullOrEmpty(Url))
                {
                    if (InitializationFailure == null)
                        InitializationFailure = new ArgumentNullException("Url", Properties.Resources.Generic_UrlNotSet);
                    if (callback != null)
                        callback(null);
                    return;
                }

#if SILVERLIGHT && !WINDOWS_PHONE
				// http && build action = Resource
				Uri uri;

				if (!string.IsNullOrEmpty(ProxyUrl))
					uri = Utilities.PrefixProxy(ProxyUrl, Url);
				else
					uri = new Uri(Url,UriKind.RelativeOrAbsolute);				
				
#else
				Uri uri = new Uri(Url,UriKind.RelativeOrAbsolute);
#endif			    
				if (uri.IsAbsoluteUri)
				{
					string scheme = uri.Scheme.ToLower();
					if (scheme == "http" || scheme == "https") // Web 						
						client.OpenReadAsync(uri);
#if !SILVERLIGHT
					else if (scheme == "pack") // Local project
					{
						StreamResourceInfo info = Application.GetResourceStream(uri); // Build Action = Resource
						if (info == null)
							info = Application.GetContentStream(uri); // Build Action = Content
						if (info == null)
							info = Application.GetRemoteStream(uri); // site-origin

						if (info != null)
							SetStream(info.Stream, callback);
						else
						{
                            if (InitializationFailure == null)
                                InitializationFailure = new ArgumentException(Properties.Resources.CsvLayer_ResourceNotFound, "Url");
							if (callback != null)
								callback(null);
						}
					}
					else if (scheme == "file") // Local machine
					{
						if (File.Exists(uri.OriginalString))
						{
							using (StreamReader reader = File.OpenText(uri.OriginalString))
							{
								SetStream(reader.BaseStream, callback);
							}
						}
						else
						{
                            if (InitializationFailure == null)
                                InitializationFailure = new ArgumentException(Properties.Resources.CsvLayer_ResourceNotFound, "Url");
							if (callback != null)
								callback(null);
						}
					}
#endif
                    else // unsupported / Invalid url path
					{
                        if (InitializationFailure == null)
                            InitializationFailure = new ArgumentException(Properties.Resources.CsvLayer_ResourceNotFound, "Url");
						if (callback != null)
							callback(null);
					}
				}
				else
				{
#if !SILVERLIGHT
					if (File.Exists(uri.OriginalString)) // relative local machine 
					{
						using (StreamReader reader = File.OpenText(uri.OriginalString))
						{
							SetStream(reader.BaseStream, callback);
						}
					}
					else
					{
#endif
						StreamResourceInfo info = Application.GetResourceStream(uri); // resource (&& content - silverlight only)                        
#if !SILVERLIGHT
						if (info == null)
							info = Application.GetContentStream(uri); // content - Wpf
						if (info == null)
							info = Application.GetRemoteStream(uri); // site-origin
#endif
						if (info != null)
							SetStream(info.Stream, callback);
						else
						{
                            if (InitializationFailure == null)
                                InitializationFailure = new ArgumentException(Properties.Resources.CsvLayer_ResourceNotFound, "Url");
							if (callback != null)
								callback(null);
						}
#if !SILVERLIGHT
					}
#endif
				}		
			}
			catch (Exception ex)
			{
				InitializationFailure = ex;
				if (callback != null)
					callback(null);
			}					
		}

        /// <summary>
        /// Attempts to convert and object into a exact data type based on FieldType metadata.
        /// </summary>
        /// <param name="valueObject">object value to try and convert into a specific type</param>
        /// <param name="expectedValueType">type that the value object will try to be converted to.</param>
        /// <returns>The converted value as an object</returns>
		private object ConvertToExpectedType(object valueObject, Field.FieldType expectedValueType)
		{
			if (valueObject == null) return null;
			object result = valueObject;
			switch (expectedValueType)
			{
				case Field.FieldType.Date:
					if (!(valueObject is DateTime))
					{
						try
						{
							if (valueObject is string)
							{      
                                string str_date = ((string)valueObject).ToLower();
                                if (str_date.Contains("utc"))
                                {
                                    str_date = str_date.Replace("utc", "");
                                    result = DateTime.Parse((string)str_date, numericCulture, DateTimeStyles.AssumeUniversal);
                                }
                                else
                                    result = DateTime.Parse((string)valueObject, numericCulture);
							}
							else
							{
								long time = 0;
								if (valueObject.GetType() == typeof(long)) time = (long)valueObject;
								else if (valueObject.GetType() == typeof(int)) time = (int)valueObject;
								else time = Convert.ToInt64(valueObject, numericCulture);
								result = Epoch.AddMilliseconds((double)time);
							}
						}
						catch { }
					}
					break;
				case Field.FieldType.Double:
					if (valueObject.GetType() != typeof(double))
					{
						try { result = Convert.ToDouble(valueObject, numericCulture); }
						catch { }
					}
					break;
				case Field.FieldType.Single:
					if (valueObject.GetType() != typeof(float))
					{
						try { result = Convert.ToSingle(valueObject, numericCulture); }
						catch { }
					}
					break;
				case Field.FieldType.Integer:
					if (valueObject.GetType() != typeof(int))
					{
						try { result = Convert.ToInt32(valueObject, numericCulture); }
						catch { }
					}
					break;
				case Field.FieldType.SmallInteger:
					if (valueObject.GetType() != typeof(short))
					{
						try { result = Convert.ToInt16(valueObject, numericCulture); }
						catch { }
					}
					break;
				case Field.FieldType.GUID:
					if (valueObject.GetType() != typeof(Guid))
					{
						Guid output;
						var valueStr = Convert.ToString(valueObject, numericCulture);
						if (!string.IsNullOrEmpty(valueStr))
						{
							if (Guid.TryParse(valueStr, out output))
								result = output;
						}
					}
					break;
			}
			return result;
		}

        /// <summary>
        /// Samples the X and Y field to check the decimal character to determine numeric culture of to use for parsing numeric numbers.
        /// There is an assumption that all numeric columns will have the same culture.
        /// </summary>
        /// <param name="csvData">Csv data</param>        
        /// <returns></returns>
        private CultureInfo GetCsvNumericCulture(List<string[]> csvData)
        {
            int xCol = -1, yCol = -1;
            if (csvData != null && csvData.Count > 0)
            {
                string[] headers = csvData[0];
                for (int i = 0; i < headers.Length; i++)
                {
                    if ((!string.IsNullOrEmpty(XFieldName) && XFieldName == headers[i]) ||
                        (string.IsNullOrEmpty(XFieldName) && LON_FIELDS.Contains(headers[i].ToLowerInvariant())))
                    {
                        if (xCol == -1)
                            xCol = i;
                    }
                    else if ((!string.IsNullOrEmpty(YFieldName) && YFieldName == headers[i]) ||
                                (string.IsNullOrEmpty(YFieldName) && LAT_FIELDS.Contains(headers[i].ToLowerInvariant())))
                    {
                        if(yCol == -1)
                            yCol = i;
                    }
                    if (xCol != -1 && yCol != -1)
                        break;
                }
                for (int i = 1; i < csvData.Count; i++)
                {
                    string[] row = csvData[i];
                    if (xCol != -1 && xCol < row.Length) // Make sure the x column is found and this row contains it 
                    {
                        string lon = row[xCol];
                        if (lon.Contains(',') && !lon.Contains('.'))
                            return new CultureInfo("fr-FR");
                        else if (lon.Contains('.') && !lon.Contains(','))
                            return CultureInfo.InvariantCulture;
                    }

                    if (yCol != -1 && yCol < row.Length) // Make sure the y column is found and this row contains it
                    {
                        string lat = row[yCol];
                        if (lat.Contains(',') && !lat.Contains('.'))
                            return new CultureInfo("fr-FR");
                        else if (lat.Contains('.') && !lat.Contains(','))
                            return CultureInfo.InvariantCulture;
                    }
                }
            }
            return CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// This will parse the Stream of CSV data and return a list of string arrays containing the parsed CSV data.
        /// </summary>
        /// <param name="stream">Stream of CSV data</param>
        /// <param name="callback">Callback that will return a list of string arrays of the parsed CSV data from the stream.</param>
		private void SetStream(Stream stream, Action<List<string[]>> callback)
		{
			using (StreamReader sr = new StreamReader(stream))
			{
                bool newLine = true;
				bool lineHasQuotes = false;
				bool sectionHasQuote = false;
				string[] separatedLine = null;
				List<string> workingLine = null;
				List<string[]> csvData = new List<string[]>();
				StringBuilder sb = new StringBuilder();
				string line = string.Empty;
                bool skip_comma = false;

				do
				{
					line = sr.ReadLine();
                    newLine = true;
					if (line == null) continue;

					// indicates quotes are in line, which may indicate that extra non-delimiters may be detected during split operation.
					if (!lineHasQuotes)
						lineHasQuotes = line.Contains('"');
                  
					separatedLine = line.Split(new string[] { ColumnDelimiter }, StringSplitOptions.None);

					if (!lineHasQuotes)
					{
						csvData.Add(separatedLine);
						continue;
					}


					if (workingLine == null)
						workingLine = new List<string>();
					for (int i = 0; i < separatedLine.Count(); i++)
					{
						string col = separatedLine[i].Trim();
                        if (col.StartsWith("\"", StringComparison.InvariantCultureIgnoreCase))
                        {
                            sectionHasQuote = true;
                            skip_comma = true;
                        }

                        if (sectionHasQuote)
                        {
                            if (!newLine && !skip_comma)
                                sb.Append(ColumnDelimiter);
                            else
                            {
                                newLine = false;
                                skip_comma = false;
                            }
                            sb.Append(separatedLine[i]);                            
                        }
                        else
                            workingLine.Add(separatedLine[i]);

						if (col.EndsWith("\"", StringComparison.InvariantCultureIgnoreCase) && !col.EndsWith("\\\"", StringComparison.InvariantCultureIgnoreCase) && col.Length != 1)
						{
							workingLine.Add(sb.ToString());
							sb.Clear();
							sectionHasQuote = false;
						}
					}

					if (!sectionHasQuote)
					{
						csvData.Add(workingLine.ToArray());
						workingLine = null;
						lineHasQuotes = false;
					}
				}
				while (line != null);
                if (csvData != null && csvData.Count > 0)
                {
                    for (int i = 0; i < csvData.Count; i++)
                    {
                        string[] row = csvData[i];                        
                        for (int j = 0; j < row.Length; j++)
                        {
                            if (row[j].StartsWith("\""))
                                row[j] = row[j].Substring(1);
                            if (row[j].EndsWith("\""))
                                row[j] = row[j].Substring(0, row[j].Length - 1);
                        }
                    }
                }

				if (callback != null)
					callback(csvData);
			}
		}

		#endregion Private Methods

		#region Public Methods

		/// <summary>
		/// Set stream is used to add graphics to the layer from a stream that contains CSV data.
		/// </summary>
		/// <param name="stream">Stream containing CSV data</param>
  /// <remarks>
  /// <para>
  /// Using the <b>SetSource</b> Method is an alternate way to create A CsvLayer using a 
  /// <a href="http://msdn.microsoft.com/en-us/library/system.io.stream(v=vs.100).aspx" target="_blank">Stream</a> 
  /// to get the data from a CSV file rather than using the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.Url">Url</see> Property. 
  /// </para>
  /// <para>
  /// The CSV file format is tabular data in plain text. The data in the CSV file consists of fields of data separated 
  /// by a delimiting character (typically a comma) for a record. The first record in the CSV file is known as the 
  /// header and defines the names of each field of the tabular data. The second through last row of records in the 
  /// CSV file is the actual tabular data. When the delimiter (typically a comma) is embedded in the tabular data 
  /// for a particular field, that value should be encased in quotes to avoid parsing errors. Each record in the 
  /// CSV file should contain the same number of fields. Numerous applications including the Microsoft Excel Office 
  /// product can export and import CSV files. It is not required that a CSV source file contain the extension .csv; 
  /// the file can contain any extension (ex: .txt) or none at all.
  /// </para>
  /// <ESRISILVERLIGHT><para>Although ArcGIS Server is not required to host a CSV file web service you may experience the similar hosting issues of accessing the web service as described in the ArcGIS Resource Center blog entitled <a href="http://blogs.esri.com/esri/arcgis/2009/08/24/troubleshooting-blank-layers/">Troubleshooting blank layers</a>. Specifically, you may need to make sure that a correct <a href="http://msdn.microsoft.com/EN-US/LIBRARY/CC197955(VS.95).ASPX" target="_blank">cilentaccesspolicy.xml or crossdomain.xml</a> file is in place on the web servers root. If a clientaccesspolicy.xml or crossdomainpolicy.xml file cannot be used on your web server for situations like <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/index.html#/Secure_services/016600000022000000/">secure services</a> you may need to use a <a href="http://resources.arcgis.com/en/help/silverlight-api/concepts/0166/other/SLProxyPage.zip">proxy page</a> on your web server and make use of the <b>CsvLayer.ProxyUrl</b> Property.</para></ESRISILVERLIGHT>
  /// <para>
  /// The bare minimum settings that need to be specified to create and display a CsvLayer in a Map are the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer.Url">Url</see> and 
  /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Url methadology) OR 
  /// the <b>SetSource</b> and 
  /// <see cref="ESRI.ArcGIS.Client.GraphicsLayer.Renderer">Renderer</see> Properties (Stream methodology). 
  /// NOTE: This assumes that default spatial coordinate information field names are used and the delimiter for 
  /// the CSV file is a comma.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// Click the Button to add a CsvLayer to the Map. The CsvLayer will be created using a local Resouce in the 
  /// Visual Studio project. The CsvLayer.SetSource Method is used to get the local Resource as a Stream.
  /// </para>
  /// <para>
  /// SPECIAL INSTRUCTIONS: The name of the sample Visual Studio project in this code example is "TestProject".
  /// Additionally a folder named "myFolder" was added to "TestProject". Place the "US_Cities_Top_5.csv" file in
  /// the "myFolder" location. Make sure that the for the Properties of the "US_Cities_Top_5.csv" file that the
  /// 'Build Action' is set to 'Resource'.
  /// </para>
  /// <para>
  /// <img border="0" alt="Adding the US_Cities_Top_5.csv file as a Resource to the Visual Studio Proect named 'TestProject' in the 'myFolder' location." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.SetSource1.png"/>
  /// </para>
  /// <para>
  /// The following is an example of the ASCII contents for the file named US_Cities_Top_5.csv:<br/>
  /// ID,Lat,Long,CityName,Population<br/>
  /// 1,40.714,-74.006,New York City,8244910<br/>
  /// 2,34.0522,-118.244,Los Angeles,3819702<br/>
  /// 3,41.878,-87.636,Chicago,2708120<br/>
  /// 4,29.763,-95.363,Houston,2099451<br/>
  /// 5,39.952,-75.168,Philadelphia,1526006<br/>
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Adding a CsvLayer via the .SetSource Method to a Map." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.ToolkitDataSources.CsvLayer.SetSource.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" &gt;
  ///   
  ///   &lt;!-- Define a SimpleRenderer with a SimpleMarkerSymbol as Resource that can be used in other locations of the project. --&gt;
  ///   &lt;Grid.Resources&gt;
  ///     &lt;esri:SimpleRenderer x:Key="renderer"&gt;
  ///       &lt;esri:SimpleRenderer.Symbol&gt;
  ///         &lt;esri:SimpleMarkerSymbol Color="Yellow" Size="20" Style="Circle" /&gt;
  ///       &lt;/esri:SimpleRenderer.Symbol&gt;
  ///     &lt;/esri:SimpleRenderer&gt;
  ///   &lt;/Grid.Resources&gt;
  ///   
  ///   &lt;!-- Add a Map Control to the application. Set the Extent to North America. --&gt;
  ///   &lt;esri:Map x:Name="Map1" HorizontalAlignment="Left" VerticalAlignment="Top" 
  ///         Margin="0,212,0,0" Height="376" Width="415" Extent="-15219969,2609636,-6232883,6485365"&gt;
  ///     
  ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
  ///     &lt;esri:ArcGISTiledMapServiceLayer  ID="World_Topo_Map" 
  ///           Url="http://services.arcgisonline.com/arcgis/rest/services/world_topo_map/MapServer" /&gt;
  ///     
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- Add a Button that will allow the user to add a CsvLayer via code-behind. --&gt;
  ///   &lt;Button Name="Button1" Height="23" HorizontalAlignment="Left" Margin="0,183,0,0"  VerticalAlignment="Top" 
  ///           Width="415" Content="Add a CsvLayer (via code-behind) for the specified Url."
  ///           Click="Button1_Click" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="174" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="788" 
  ///              TextWrapping="Wrap" Text="Click the Button to add a CsvLayer to the Map. The CsvLayer will be 
  ///              created using a local Resouce in the Visual Studio project. The CsvLayer.SetSource Method is
  ///              used to get the local Resource as a Stream." /&gt;
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   // Create a CsvLayer. 
  ///   ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();
  ///   
  ///   // Create a new Uri for the CSV file located on the hard drive. 
  ///   // VERY IMPORTANT: 
  ///   // Replace the first parameter argument of the Uri constructor with the correct string to the location of the CSV 
  ///   // file relative to your test project. In this example the Visual Studio project name is: "TestProject".
  ///   // Additionally, a folder was added to "TestProject" called "myFolder" and this is where the "US_Cities_Top_5.csv 
  ///   // file is located. Finally, make sure that the for the Properties of the US_Cities_Top_5.csv file that the 
  ///   // 'Build Action' is set to 'Resource'.
  ///   Uri myUri = new Uri("/TestProject;component/myFolder/US_Cities_Top_5.csv", UriKind.RelativeOrAbsolute);
  ///   
  ///   // Create a StreamResourceInfo object using the static/Shared Application.GetResourceStream function.
  ///   System.Windows.Resources.StreamResourceInfo myStreamResourceInfo = Application.GetResourceStream(myUri);
  ///   
  ///   // Use the StreamResourceInfo.Stream in the CsvLayer.SetSource Method 
  ///   myCsvLayer.SetSource(myStreamResourceInfo.Stream);
  ///   
  ///   // Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_5";
  ///   
  ///   // Use the SimpleRenderer with a SimpleMarkerSymbol that was defined in XAML for the CsvLayer.Render Property.
  ///   myCsvLayer.Renderer = (ESRI.ArcGIS.Client.IRenderer)LayoutRoot.Resources["renderer"];
  ///   
  ///   // Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer);
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function executes as a result of the user clicking the Button. It adds a CsvLayer using code-behind.
  ///   
  ///   ' Create a CsvLayer. 
  ///   Dim myCsvLayer As ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer = New ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer
  ///   
  ///   ' Create a new Uri for the CSV file located on the hard drive. 
  ///   ' VERY IMPORTANT: 
  ///   ' Replace the first parameter argument of the Uri constructor with the correct string to the location of the CSV 
  ///   ' file relative to your test project. In this example the Visual Studio project name is: "TestProject".
  ///   ' Additionally, a folder was added to "TestProject" called "myFolder" and this is where the "US_Cities_Top_5.csv 
  ///   ' file is located. Finally, make sure that the for the Properties of the US_Cities_Top_5.csv file that the
  ///   ' 'Build Action' is set to 'Resource'.
  ///   Dim myUri As Uri = New Uri("/TestProject;component/myFolder/US_Cities_Top_5.csv", UriKind.RelativeOrAbsolute)
  ///   
  ///   ' Create a StreamResourceInfo object using the static/Shared Application.GetResourceStream function.
  ///   Dim myStreamResourceInfo As Windows.Resources.StreamResourceInfo = Application.GetResourceStream(myUri)
  ///   
  ///   ' Use the StreamResourceInfo.Stream in the CsvLayer.SetSource Method 
  ///   myCsvLayer.SetSource(myStreamResourceInfo.Stream)
  ///   
  ///   ' Set the ID of the CsvLayer.
  ///   myCsvLayer.ID = "US_Cities_Top_5"
  ///   
  ///   ' Use the SimpleRenderer with a SimpleMarkerSymbol that was defined in XAML for the CsvLayer.Render Property.
  ///   myCsvLayer.Renderer = LayoutRoot.Resources("renderer")
  ///   
  ///   ' Add the CsvLayer to the Map.
  ///   Map1.Layers.Add(myCsvLayer)
  ///   
  /// End Sub
  /// </code>
  /// </example>
  public void SetSource(Stream stream)
		{
			SetStream(stream, new Action<List<string[]>>((csvData) =>
			{
				CreateGraphics(csvData, new Action<GraphicCollection>((gc) =>
				{					
					this.Graphics = gc;
				}));
			}));
		}
       
		#endregion PUblic Methods

		#region Override

        /// <summary>
        /// Initializes the layer.
        /// </summary>
        /// <seealso cref="ESRI.ArcGIS.Client.Layer.Initialized"/>
        /// <seealso cref="ESRI.ArcGIS.Client.Layer.InitializationFailure"/>
		public override void Initialize()
		{
			if (initializing || IsInitialized || DesignerProperties.GetIsInDesignMode(this))
				return;
			if (Url == null)
				base.Initialize();
			else
			{
				initializing = true;
				Action<GraphicCollection> callback;
				callback = (collection) =>
				{
					this.Graphics = collection;
					base.Initialize();                    
                    initializing = false;
				};
				GetGraphics(callback);
			}
		}

		#endregion

        /// <summary>
        /// Holds a collection of <see cref="ESRI.ArcGIS.Client.Field"/>.
        /// </summary>
        public class FieldCollection : List<Field>{}
	}
}