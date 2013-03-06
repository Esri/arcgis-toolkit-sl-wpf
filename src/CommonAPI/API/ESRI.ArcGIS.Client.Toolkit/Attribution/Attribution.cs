// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ESRI.ArcGIS.Client.Toolkit
{

    /// <summary>
    /// The Attribution Control displays Copyright information for Layers that have the IAttribution 
    /// Interface implemented.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Attribution Control is a User Interface (UI) control that displays <b>Copyright</b> information 
    /// in a list of Microsoft 
    /// <a href="http://msdn.microsoft.com/en-us/library/system.windows.controls.contentpresenter(v=VS.95).aspx" target="_blank">ContentPresenter</a> 
    /// Controls for each <see cref="ESRI.ArcGIS.Client.Layer">Layer</see> in a 
    /// <see cref="ESRI.ArcGIS.Client.LayerCollection">LayerCollection</see> that has the IAttribution Interface
    /// implemented. Layers that have the IAttribution Interface implemented have an additional 
    /// <b>.AttributionTemplate</b> Property which returns a 
    /// <a href="http://msdn.microsoft.com/en-us/library/ms589297(v=VS.95).aspx" target="_blank">DataTemplate</a>
    /// that allows for the display of the <b>Copyright</b> information in the ContentPresenter. As of version 2.2
    /// of the API, the <b>Copyright</b> information is returned as <b>CopyrightText</b> strings for the various
    /// Layers that implement the IAttribution interface.
    /// </para>
    /// <para>
    /// The Attribution Control can be created at design-time in XAML or dynamically at run-time in the 
    /// code-behind. 
    /// <ESRISILVERLIGHT>The Attribution Control is one of several controls available in the Toolbox for Visual Studio when the ArcGIS API for Silverlight is installed, </ESRISILVERLIGHT>
    /// <ESRIWPF>The Attribution Control is one of several controls available in the Toolbox for Visual Studio when the ArcGIS Runtime SDK for WPF is installed, </ESRIWPF>
    /// <ESRIWINPHONE>There are no controls that can be dragged into the XAML design surface from the Toolbox as part of the ArcGIS Runtime SDK for Windows Phone installation. Developers need to type the correct syntax in the XAML to have the controls appear in the visual designer, </ESRIWINPHONE>
    /// see the following screen shot:
    /// </para>
    /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the Attribution Control on the XAML design surface of a Silverlight application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution.png"/></para></ESRISILVERLIGHT>
    /// <ESRIWPF><para><img border="0" alt="Example of the Attribution Control on the XAML design surface of a WPF application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution_VS_WPF.png"/></para></ESRIWPF>
    /// <ESRIWINPHONE><para><img border="0" alt="Example of the Attribution Control on the XAML design surface of a Windows Phone application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution_VS_WinPhone.png"/></para></ESRIWINPHONE>
    /// <para>
    /// The default appearance of the Attribution Control can be modified using numerous inherited Properties 
    /// from System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. An example 
    /// of some of these Properties include: .Height, .Width, .BackGround, .BorderBrush, .BorderThickness, 
    /// .FontFamily, .FontSize, .FontStretch, .FontStyle, .Foreground, .HorizontalAlignment, .VerticalAlignment, 
    /// .Margin, .Opacity, .Visibility, etc. 
    /// </para>
    /// <para>
    /// Note: you cannot change the core behavior of the sub-components (i.e. ContentPresenter, etc.) of the 
    /// Attribution Control using standard Properties or Methods alone. To change the core behavior of the 
    /// sub-components and their appearance of the Control, developers can modify the Control Template in 
    /// XAML and the associated code-behind file. The easiest way to modify the UI sub-components is using 
    /// Microsoft Expression Blend. Then developers can delete/modify existing or add new sub-components in 
    /// Visual Studio to create a truly customized experience. A general approach to customizing a Control 
    /// Template is discussed in the ESRI blog entitled: 
    /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
    /// A specific code example of modifying the Control Template of the Attribution Control can be found 
    /// in the <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see> documentation.
    /// </para>
	   /// <para>
    /// The Attribution Control is comprised of mainly one sub-component, a ContentPresenter, that displays 
    /// <b>Copyright</b> information for each Layer that has the IAttribution Interface implemented. The 
    /// implied Binding that occurs in the ControlTemplate Style of the Attribution Control to the 
    /// ContentPresenter sub-component is to the 
    /// <see cref="ESRI.ArcGIS.Client.IAttribution">ESRI.ArcGIS.Client.IAttribution</see> Interface members. 
    /// The IAttribution Interface adds functionality to Layer objects. These special Layer Types have an 
    /// <b>.AttributionTemplate</b> Property. The <b>.AttributionTemplate</b> Property is the internal mechanism 
    /// for constructing the IAttribution object. Only those Layers which have a <b>.AttributionTemplate</b> 
    /// Property will get listed in the ObservabaleCollection(Of IAttribution) as a result of using the 
    /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Items">Attribution.Items</see> Property. The 
    /// following Layers are those that implement the IAttribution Interface and have an 
    /// <b>.AttributionTemplate</b> Property: 
    /// </para>
    /// <para>
    /// <list type="bullet">
    /// <item><see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer">ArcGISDynamicMapServiceLayer</see></item>
    /// <item><see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer">ArcGISImageServiceLayer</see></item>
    /// <item><see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer">ArcGISTiledMapServiceLayer</see></item>
    /// <item><see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see></item>
    /// <item><see cref="T:ESRI.ArcGIS.Client.Bing.TileLayer">TileLayer</see></item>
    /// <item><see cref="T:ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer">OpenStreetMapLayer</see></item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Note:</b> This also means that you can also use implied Binding in the ControlTemplate Style of the 
    /// Attribution Control for the various Properties of the special Types of Layers such as: .ID, .Url, 
    /// .Version, etc. 
	/// </para>
	/// <para>
    /// The <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Items">Attribution.Items</see> Property returns 
    /// a Read only ObservableCollection(Of IAttribution) object. While it is true that you can use the various 
    /// <a href="http://msdn.microsoft.com/en-us/library/ms668604(v=VS.95).aspx" target="_blank">ObservableCollection</a> 
    /// Propeties such as .Add, .Clear, .Remove, etc. to manipulate the contents of what is in the 
    /// ObservableCollection it is not recommended. The 
    /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.PropertyChanged">Attribution.PropertyChanged</see> 
    /// and <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Refreshed">Attribution.Refreshed</see> Events do 
    /// not fire as a result of adding/removing items in the ObservableCollection. Additionally, the Map Control does 
    /// not update by adding/removing items in the ObservableCollection. The correct programming practice to 
    /// see automatic updates to the Attributon.Items Collection is to add Layers to the Map Control which is 
    /// bound to the <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see>. When 
    /// Binding the Map.Layers to the Attribution.Layers is done any .Add, .Remove, or change to the 
    /// LayerCollection will result in update to Attribution.Items.
	/// </para>
    /// <para>
    /// Both the Attribution.PropertyChanged and Attribution.Refreshed Events fire as a result of the Layers 
    /// that Implement IAttribution in the LayerCollection being added or removed in the Attribution.Layers 
    /// Property. Use the Attribution.Refreshed Event if you want to add, delete, or modify the default 
    /// attribution items given by the framework. Use the Attribution.PropertyChanged Event on property 
    /// Attribution.Items, if you want to be aware of this change in order to hook up an Event fired by 
    /// Attribution.Items. <b>Note:</b> The various special Layer types that Implement the IAttribution 
    /// Interface have <b>Copyright</b> information Properties that are ReadOnly (meaning they can't be changed 
    /// by the client application) and hence it is impossible force the Attribution.PropertyChanged and 
    /// Attribution.Refreshed Events to update as a result of trying to modify the <b>Copyright</b> 
    /// information on the client side. 
    /// </para>
    /// <para>
    /// In order to use the Attribution Control it is mandatory that the 
    /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see> Property be set to 
    /// a valid <see cref="ESRI.ArcGIS.Client.LayerCollection">LayerCollection</see>. This can be done via 
    /// Binding in the XAML or by setting the Attribution.Layers Property in the code-behind file. Typically, 
    /// the LayerCollection object is obtained from the <see cref="ESRI.ArcGIS.Client.Map.Layers">Layers</see> 
    /// Property on the Map Control. 
    /// </para>
 	/// <para>
	/// In the following XAML example, Binding is used to associate a Map Control named 'Map1' with it’s 
    /// LayerCollection to the Attribution Control’s Layers Property:<br/>
	/// <code lang="XAML">
    /// &lt;esri:Attribution Name="Attribution1" Layers="{Binding ElementName=Map1,Path=Layers}" /&gt;
	/// </code>
    /// </para>
	   /// <para>
    /// The default visual appearance of the Attribution Control is minimal 
    /// <ESRISILVERLIGHT>when using drag-and-drop to place the control </ESRISILVERLIGHT>
    /// <ESRIWPF>when using drag-and-drop to place the control </ESRIWPF>
    /// on the design surface of a XAML page in Visual Studio; there are graphical selection handles 
    /// but nothing else to denote the visual appearance of the control. At design-time, it is not until the 
    /// Attribution.Layers Property is specified that placeholder text values will be populated in the control. 
    /// At design-time, if no <b>ID</b> value is specified for the Layer the placeholder information displayed 
    /// in the list of ContentPresenter sub-controls will be of the form: "<b>&lt;Type of Layer&gt; attribution.</b>". 
    /// Conversely, if there is an <b>ID</b> value specified for the Layer the placeholder information displayed 
    /// in the list of ContentPresenter sub-controls will be of the form: "<b>&lt;Layer ID&gt; attribution.</b>". See 
    /// the following screen shot to see how the design-time placeholder text appears in the Attribution Control 
    /// for the associated XAML:
    /// </para>
    /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the visual appearance of the Attribution Control at design-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution2.png"/></para></ESRISILVERLIGHT>
    /// <ESRIWPF><para><img border="0" alt="Example of the visual appearance of the Attribution Control at design-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution_VS2_WPF.png"/></para></ESRIWPF>
    /// <ESRIWINPHONE><para><img border="0" alt="Example of the visual appearance of the Attribution Control at design-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution_VS2_WinPhone.png"/></para></ESRIWINPHONE>
    /// <para>
    /// <b>NOTE:</b> It is not until run-time that the actual <b>Copyright</b> information about a Layer 
    /// will replace the placeholder text that is shown in the Attribution Control defined at design-time. 
    /// The following image shows the run-time display of the Map Control and the Attribution Control that 
    /// corresponds to the previous design-time screen shot where the actual <b>Copyright</b> information 
    /// about the Layers is displayed.
	/// </para>
    /// <para>
    /// <img border="0" alt="Example of the visual appearance of the Attribution Control at run-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution2a.png"/>
    /// </para>
    /// <para>
    /// <b>Note:</b> Setting the Layer.ID Property is typically done in XAML or in code-behind. The Layer.ID is 
    /// usually not populated in the map service.
    /// </para>
    /// <para>
    /// If length of the <b>Copyright</b> information string for a Layer exceeds the Attribution Controls Width, 
    /// the text will wrap across multiple lines to avoid truncation. 
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// <b>How to use:</b>
    /// </para>
    /// <para>
    /// By default two Layers have been added to the Map Control and the <b>Copyright</b> information for those 
    /// Layers are displayed in the Attribution Control. Click the 'Add Layer Dynamically' button to add 
    /// another Layer to the Map and see the newly added <b>Copyright</b> information added to the Attribution 
    /// Control.
    /// </para>
    /// <para>
    /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
    /// the functionality.
    /// </para>
    /// <para>
    /// The following screen shot corresponds to the code example in this page.
    /// </para>
    /// <para>
    /// <img border="0" alt="Visual example of the Attribution Control displaying CopyrightText information about various layers." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution3.png"/>
    /// </para>
    /// <code title="Example XAML1" description="" lang="XAML">
    /// &lt;Grid x:Name="LayoutRoot"&gt;
    ///   
    ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
    ///   &lt;TextBlock Height="65" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
    ///              TextWrapping="Wrap" Margin="2,2,0,0" 
    ///              Text="By default two Layers have been added to the Map Control and the Copyright information
    ///              for those Layers are displayed in the Attribution Control. Click the 'Add Layer Dynamically' 
    ///              button to add another Layer to the Map and see the newly added Copyright information
    ///              added to the Attribution Control." /&gt;
    ///   
    ///   &lt;!-- Add a Map Control zoomed to a specific Extent. --&gt;  
    ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,102,0,0" Name="Map1" 
    ///             VerticalAlignment="Top" Height="306" Width="545" 
    ///             Extent="-10723833,4627392,-10704398,4644168"&gt;
    ///   
    ///     &lt;!-- 
    ///     Add an ArcGISTiledMapServiceLayer. Note: No ID value was specified and hence the Layer can not
    ///     be accessed via code-behind. Also, since no ID value was specified the design-time will list the
    ///     Type of Layer in Attribution Control placeholder (Ex: ArcGISTiledMapServiceLayer attribution.).
    ///     --&gt;
    ///     &lt;esri:ArcGISTiledMapServiceLayer  
    ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"/&gt;
    ///     
    ///     &lt;!-- 
    ///     Add an ArcGISDynamicMapServiceLayer. Note: since an ID value is specified the design-time will 
    ///     list the Layer ID in Attribution Control placeholder (Ex: World Transportation attribution.).
    ///     --&gt;
    ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="World Transportation"
    ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer" /&gt;
    ///     
    ///   &lt;/esri:Map&gt;
    ///   
    ///   &lt;!--
    ///   Add an Attribution Control and Bind the Map.Layers to the Attribution.Layers. Change a few
    ///   visual appearance settings related to the Font for the text that is displayed in the Control. 
    ///   --&gt;
    ///   &lt;esri:Attribution HorizontalAlignment="Left" Margin="12,420,0,0" Name="Attribution1" 
    ///                     VerticalAlignment="Top" Layers="{Binding ElementName=Map1,Path=Layers}" 
    ///                     Width="545" Height="48" 
    ///                     FontSize="11" FontFamily="Arial" Foreground="Chocolate"/&gt;
    ///      
    ///   &lt;!-- 
    ///   Add a button to dynamically add a Layer to the Map which will also automatically update what is in
    ///   the Attribution Control. Note the Click event handler is wired up to use code-behind. 
    ///   --&gt;
    ///   &lt;Button Content="Add Layer Dynamically" Height="23" HorizontalAlignment="Left" Margin="12,73,0,0" 
    ///           Name="AddLayerDynamically" VerticalAlignment="Top" Width="545" Click="AddLayerDynamically_Click"/&gt;
    ///   
    /// &lt;/Grid&gt;
    /// </code>
    /// <code title="Example CS1" description="" lang="CS">
    /// private void AddLayerDynamically_Click(object sender, System.Windows.RoutedEventArgs e)
    /// {
    ///   // This function dynamically (i.e. at run-time) adds a FeatureLayer to the Map Control. As a result the 
    ///   // Attribution Control will also be updated with the CopyrightText information available on the Layer.
    ///   
    ///   // Create a new FeatureLayer and set a few Properties. 
    ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = new ESRI.ArcGIS.Client.FeatureLayer();
    ///   myFeatureLayer.Url = "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Hydrography/Watershed173811/FeatureServer/1";
    ///   myFeatureLayer.DisableClientCaching = true;
    ///   myFeatureLayer.Mode = ESRI.ArcGIS.Client.FeatureLayer.QueryMode.OnDemand;
    ///   
    ///   // Add the FeatureLayer to the Map Control.
    ///   Map1.Layers.Add(myFeatureLayer);
    /// }
    /// </code>
    /// <code title="Example VB1" description="" lang="VB.NET">
    /// Private Sub AddLayerDynamically_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    ///   
    ///   ' This function dynamically (i.e. at run-time) adds a FeatureLayer to the Map Control. As a result the 
    ///   ' Attribution Control will also be updated with the CopyrightText information available on the Layer.
    ///   
    ///   ' Create a new FeatureLayer and set a few Properties. 
    ///   Dim myFeatureLayer As New ESRI.ArcGIS.Client.FeatureLayer
    ///   myFeatureLayer.Url = "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Hydrography/Watershed173811/FeatureServer/1"
    ///   myFeatureLayer.DisableClientCaching = True
    ///   myFeatureLayer.Mode = ESRI.ArcGIS.Client.FeatureLayer.QueryMode.OnDemand
    ///   
    ///   ' Add the FeatureLayer to the Map Control.
    ///   Map1.Layers.Add(myFeatureLayer)
    ///   
    /// End Sub
    /// </code>
    /// </example>
	public class Attribution : Control, INotifyPropertyChanged
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Attribution"/> class.
		/// </summary>
		public Attribution()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(Attribution);
#endif
		}

		/// <summary>
		/// Static initialization for the <see cref="Attribution"/> control.
		/// </summary>
		static Attribution()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Attribution),
				new FrameworkPropertyMetadata(typeof(Attribution)));
#endif
		}
		#endregion

		#region DependencyProperty Layers

        /// <summary>
        /// Gets or sets the LayerCollection that the Attribution Control is buddied to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In order to use the Attribution Control it is mandatory that the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see> Property be set 
        /// to a valid <see cref="ESRI.ArcGIS.Client.LayerCollection">LayerCollection</see>. This can be done 
        /// via Binding in the XAML or by setting the Attribution.Layers Property in the code-behind file. 
        /// Typically, the LayerCollection object is obtained from the 
        /// <see cref="ESRI.ArcGIS.Client.Map.Layers">Layers</see> Property on the Map Control. 
        /// </para>
	    /// <para>
	    /// In the following XAML example, Binding is used to associate a Map Control named 'Map1' with it’s 
        /// LayerCollection to the Attribution Control’s Layers Property:<br/>
	    /// <code lang="XAML">
        /// &lt;esri:Attribution Name="Attribution1" Layers="{Binding ElementName=Map1,Path=Layers}" /&gt;
	    /// </code>
        /// </para>
	    /// <para>
        /// The default visual appearance of the Attribution Control is minimal when using drag-and-drop to place 
        /// the control on the design surface of a XAML page in Visual Studio; there are graphical selection 
        /// handles but nothing else to denote the visual appearance of the control. At design-time, it is not 
        /// until the Attribution.Layers Property is specified that placeholder text values will be populated in 
        /// the control. At design-time, if no <b>ID</b> value is specified for the Layer the placeholder 
        /// information displayed in the list of ContentPresenter sub-controls will be of the form: 
        /// "<b>&lt;Type of Layer&gt; attribution.</b>". Conversely, if there is an <b>ID</b> value specified for the 
        /// Layer the placeholder information displayed in the list of ContentPresenter sub-controls will be of 
        /// the form: "<b>&lt;Layer ID&gt; attribution.</b>". See the following screen shot to see how the design-time 
        /// placeholder text appears in the Attribution Control for the associated XAML:
	    /// </para>
        /// <para>
        /// <img border="0" alt="Example of the visual appearance of the Attribution Control at design-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution2.png"/>
        /// </para>
        /// <para>
        /// <b>NOTE:</b> It is not until run-time that the actual <b>Copyright</b> information about a Layer 
        /// will replace the placeholder text that is shown in the Attribution Control defined at design-time. 
        /// The following image shows the run-time display of the Map Control and the Attribution Control that 
        /// corresponds to the previous design-time screen shot where the actual <b>Copyright</b> information 
        /// about the Layers is displayed.
	    /// </para>
        /// <para>
        /// <img border="0" alt="Example of the visual appearance of the Attribution Control at run-time when the Attribution.Layers Property is bound to the Map.Layers Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution2a.png"/>
        /// </para>
        /// <para>
        /// <b>Note:</b> Setting the Layer.ID Property is typically done in XAML or in code-behind. The Layer.ID 
        /// is usually not populated in the map service.
        /// </para>
	    /// <para>
        /// When Binding the Map.Layers to the Attribution.Layers is done any .Add, .Remove, or change to the 
        /// LayerCollection will result in update to Attribution.Items.
	    /// </para>
	    /// <para>
        /// Only those Layers in the LayerCollection that Implement the 
        /// <see cref="ESRI.ArcGIS.Client.IAttribution">ESRI.ArcGIS.Client.IAttribution</see>. Interface will have 
        /// <b>Copyright</b> information displayed in the Attribution Control. You can tell if the IAttribution 
        /// Interface is implemented on the Layer if it has a <b>.AttributionTemplate</b> Property. The 
        /// following Layers are those that implement the IAttribution Interface and have an 
        /// <b>.AttributionTemplate</b> Property: 
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer">ArcGISDynamicMapServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer">ArcGISImageServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer">ArcGISTiledMapServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see></item>
        /// <item><see cref="M:ESRI.ArcGIS.Client.Bing.TileLayer">TileLayer</see></item>
        /// <item><see cref="M:ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer">OpenStreetMapLayer</see></item>
        /// </list>
        /// </para>
        /// <para>
        /// Both the <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.PropertyChanged">Attribution.PropertyChanged</see> 
        /// and <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Refreshed">Attribution.Refreshed</see> Events fire 
        /// as a result of the Layers that Implement IAttribution in the LayerCollection being added or removed in 
        /// the Attribution.Layers Property. Use the Attribution.Refreshed Event if you want to add, delete, or modify 
        /// the default attribution items given by the framework. Use the Attribution.PropertyChanged Event on 
        /// property <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Items">Attribution.Items</see>, if you want to 
        /// be aware of this change in order to hook up an Event fired by Attribution.Items. <b>Note:</b> The various 
        /// special Layer types that Implement the IAttribution Interface have <b>Copyright</b> infromation Properties 
        /// that are ReadOnly (meaning they can't be changed by the client application) and hence it is impossible force 
        /// the Attribution.PropertyChanged and Attribution.Refreshed Events to update as a result of trying to modify 
        /// the <b>Copyright</b> information on the client side. 
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// By default two Layers have been added to the Map Control and the Copyright information for those 
        /// Layers are displayed in the Attribution Control. Click the 'Apply a different ControlTemplate Style 
        /// to the Attribution Control' button to change the appearance of the text in the Attribution Control; it 
        /// will have the Layer ID in bold, a dash, and then the Copyright information displayed.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Applying a custom ControlTemplate as the Style of the Attribution Control for the various Attribution.Layers." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution.Layers.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- 
        ///   Use the Resources section to hold a Style for setting the appearance and behavior of the Attribution 
        ///   Control. 
        ///   --&gt;
        ///   &lt;Grid.Resources&gt;
        ///     
        ///     &lt;!--
        ///     The majority of the XAML that defines the ControlTemplate for the Attribution Control was obtained
        ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
        ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
        ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
        ///     --&gt;
        ///     &lt;Style x:Key="AttributionStyle1" TargetType="esri:Attribution"&gt;
        ///       &lt;Setter Property="Background" Value="Transparent"/&gt;
        ///       &lt;Setter Property="BorderBrush" Value="Transparent"/&gt;
        ///       &lt;Setter Property="BorderThickness" Value="0"/&gt;
        ///       &lt;Setter Property="IsTabStop" Value="False"/&gt;
        ///       &lt;Setter Property="IsHitTestVisible" Value="False"/&gt;
        ///       &lt;Setter Property="Template"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;ControlTemplate TargetType="esri:Attribution"&gt;
        ///             &lt;Border BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" 
        ///                     Background="{TemplateBinding Background}" CornerRadius="5"&gt;
        ///               &lt;ItemsControl ItemsSource="{Binding Items, RelativeSource={RelativeSource TemplatedParent}}"&gt;
        ///                 &lt;ItemsControl.ItemTemplate&gt;
        ///                   &lt;DataTemplate&gt;
        ///                     &lt;Grid Margin="3"&gt;
        ///                       &lt;StackPanel Orientation="Horizontal"&gt;
        ///                       
        ///                         &lt;!--
        ///                         Add a TextBlock to display the Layer ID value in bold along with a dash (-) for a separator.
        ///                         The StringFormat is used format the Layer ID value. There are several MSDN documents that are
        ///                         a good reference for creating your own custom formatting using the BindingBase.StringFormat Property:
        ///                         http://msdn.microsoft.com/en-us/library/system.windows.data.bindingbase.stringformat(v=VS.100).aspx
        ///                         http://msdn.microsoft.com/en-us/library/26etazsy.aspx
        ///                         http://msdn.microsoft.com/en-us/library/txafckwd.aspx
        ///                         http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx
        ///                         http://msdn.microsoft.com/en-us/library/0c899ak8.aspx
        ///                         http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
        ///                         http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
        ///                           
        ///                         It is imperative that a Layer ID value be specified in XAML for the different Layers that are
        ///                         added to the Map Control. If no ID value is specified, nothing will be displayed for the Layer
        ///                         ID in the Attribution Control.
        ///                         --&gt;
        ///                         &lt;TextBlock Text="{Binding ID, StringFormat='\{0\} - '}" FontWeight="Bold" 
        ///                                    FontFamily="Arial Black" FontSize="14"/&gt;
        ///                         
        ///                         &lt;ContentPresenter ContentTemplate="{Binding AttributionTemplate}" Content="{Binding}" /&gt;
        ///                       &lt;/StackPanel&gt;
        ///                     &lt;/Grid&gt;
        ///                   &lt;/DataTemplate&gt;
        ///                 &lt;/ItemsControl.ItemTemplate&gt;
        ///               &lt;/ItemsControl&gt;
        ///             &lt;/Border&gt;
        ///           &lt;/ControlTemplate&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///     &lt;/Style&gt;
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="67" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
        ///              TextWrapping="Wrap" Margin="2,2,0,0" 
        ///              Text="By default two Layers have been added to the Map Control and the CopyrightText information for those 
        ///              Layers are displayed in the Attribution Control. Click the 'Apply a different ControlTemplate Style to 
        ///              the Attribution Control' button to change the appearance of the text in the Attribution Control; it will 
        ///              have the Layer ID in bold, a dash, and then the CopyrightText information displayed." /&gt;
        ///       
        ///   &lt;!-- Add a Map Control and zoom to an initial Extent. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,113,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="295" Width="600" 
        ///             Extent="-15022410,2064855,-6201900,7017288" &gt;
        ///     
        ///     &lt;!-- 
        ///     Add an ArcGISTiledMapServiceLayer. Note that and ID value is specified in XAML. The Layer's ID value is not 
        ///     provided by the ArcGIS Server REST map service. 
        ///     --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="US Topography" 
        ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/USA_Topo_Maps/MapServer"/&gt;
        ///       
        ///     &lt;!-- 
        ///     Add an ArcGISDynamicMapServiceLayer. Note that and ID value is specified in XAML. The Layer's ID value is not 
        ///     provided by the ArcGIS Server REST map service. 
        ///     --&gt;
        ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="US Population Change 1990 to 2000" Opacity=".3"
        ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/Demographics/USA_1990-2000_Population_Change/MapServer" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add an Attribution Control and Bind the Map.Layers to the Attribution.Layers. It is when the user clicks the 
        ///   Button_ApplyStyle that a code-behind function will be used to apply the Style defined in the Grid.Resources
        ///   to change the appearance of the text in the Attribution Control.
        ///   --&gt;
        ///   &lt;esri:Attribution HorizontalAlignment="Left" Margin="12,420,0,0" Name="Attribution1" Width="600" Height="48"
        ///                     VerticalAlignment="Top" Layers="{Binding ElementName=Map1,Path=Layers}" /&gt;
        ///     
        ///   &lt;!-- 
        ///   If you prefer to have the new Style to change the appearance of the Attribution Control as soon as the
        ///   application starts rather than have the user do it manually via a button click, uncomment the next line of XAML
        ///   code. Make sure to comment out the above other Attribution Control to avoid having duplicates.
        ///   --&gt;
        ///   &lt;!--  &lt;esri:Attribution HorizontalAlignment="Left" Margin="12,420,0,0" Name="Attribution1" Width="600" Height="48" 
        ///                     VerticalAlignment="Top" Layers="{Binding ElementName=Map1,Path=Layers}" 
        ///                     Style="{StaticResource AttributionStyle1}"/&gt; --&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a button to dynamically change the text in the Attribution Control at run-time. Note the Click event 
        ///   handler is wired up to use code-behind. 
        ///   --&gt;
        ///   &lt;Button Content="Apply a different ControlTemplate Style to the Attribution Control" 
        ///           Height="25" HorizontalAlignment="Left" Margin="12,82,0,0" 
        ///           Name="Button_ApplyStyle" VerticalAlignment="Top" Width="600" Click="Button_ApplyStyle_Click" /&gt;
        ///       
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button_ApplyStyle_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Obtain the Style for the Resources section of the XAML file.
        ///   System.Windows.Style myStyle = (System.Windows.Style)(LayoutRoot.Resources["AttributionStyle1"]);
        ///   
        ///   // Apply the custom Style to the Attribution Control.
        ///   Attribution1.Style = myStyle;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_ApplyStyle_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Obtain the Style for the Resources section of the XAML file.
        ///   Dim myStyle As System.Windows.Style = CType(LayoutRoot.Resources("AttributionStyle1"), System.Windows.Style)
        ///   
        ///   ' Apply the custom Style to the Attribution Control.
        ///   Attribution1.Style = myStyle
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public LayerCollection Layers
		{
			get { return GetValue(LayersProperty) as LayerCollection; }
			set { SetValue(LayersProperty, value); }
		}

		/// /// <summary>
		/// Identifies the <see cref="Layers"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayersProperty =
			DependencyProperty.Register("Layers", typeof(LayerCollection), typeof(Attribution), new PropertyMetadata(null, OnLayersPropertyChanged));

		private static void OnLayersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Attribution)
				(d as Attribution).OnLayersPropertyChanged(e.OldValue as LayerCollection, e.NewValue as LayerCollection);
		}

		private void OnLayersPropertyChanged(LayerCollection oldLayers, LayerCollection newLayers)
		{
			DetachLayersHandler(oldLayers);
			AttachLayersHandler(newLayers);
			UpdateAttributionItems();
		}

		#endregion

		#region Items
		private ObservableCollection<IAttribution> _items;

        /// <summary>
        /// Gets the ObservableCollection of IAttribution Items displayed in the Attribution Control.
        /// </summary>
        /// <remarks>
	    /// <para>
        /// The Attribution Items are the Layers of the Map (or the Layers corresponding to LayerIDs) implementing 
        /// IAttribution and returning a non null AttributionTemplate.
	    /// </para>
        /// <para>
        /// The <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Items">Attribution.Items</see> Property returns 
        /// a Read only ObservableCollection(Of IAttribution) object. You can use the various 
        /// <a href="http://msdn.microsoft.com/en-us/library/ms668604(v=VS.95).aspx" target="_blank">ObservableCollection</a> 
        /// Propeties such as .Add, .Clear, .Remove, etc. to manipulate the contents of what is in the Collection. 
        /// However it should be noted that the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.PropertyChanged">Attribution.PropertyChanged</see> 
        /// and <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Refreshed">Attribution.Refreshed</see> Events do 
        /// not fire as a result of adding items to the ObservableCollection. Also, the Map Control does not 
        /// automatically by adding new items to the ObservableCollection. The correct programming practice to see 
        /// automatic updates to the Attributon.Items Collection is to add Layers to the Map Control which is bound to 
        /// the <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Layers">Attribution.Layers</see>. When Binding the 
        /// Map.Layers to the Attribution.Layers is done any .Add, .Remove, or change to the LayerCollection will 
        /// result in update to Attribution.Items.
        /// </para>
        /// <para>
        /// The Attribution Control is comprised of mainly one sub-component, a ContentPresenter, that displays 
        /// <b>Copyright</b> information for each Layer that has the IAttribution Interface implemented. The 
        /// implied Binding that occurs in the ControlTemplate Style of the Attribution Control to the 
        /// ContentPresenter sub-component is to the 
        /// <see cref="ESRI.ArcGIS.Client.IAttribution">ESRI.ArcGIS.Client.IAttribution</see> Interface members. 
        /// The IAttribution Interface adds functionality to some of the Layer objects; which also means that you 
        /// can cast to these special Layer types. These special Layer Types have an <b>.AttributionTemplate</b> 
        /// Property. The <b>.AttributionTemplate</b> Property is the internal mechanism for constructing the 
        /// IAttribution object. Only those Layers which have a <b>.AttributionTemplate</b> Property will get 
        /// listed in the ObservabaleCollection(Of IAttribution) as a result of using the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Attribution.Items">Attribution.Items</see> Property. The 
        /// following Layers are those that implement the IAttribution Interface and have an 
        /// <b>.AttributionTemplate</b> Property: 
        /// </para>para>
        /// <para>
        /// <list type="bullet">
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer">ArcGISDynamicMapServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer">ArcGISImageServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer">ArcGISTiledMapServiceLayer</see></item>
        /// <item><see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see></item>
        /// <item><see cref="M:ESRI.ArcGIS.Client.Bing.TileLayer">TileLayer</see></item>
        /// <item><see cref="M:ESRI.ArcGIS.Client.Toolkit.DataSources.OpenStreetMapLayer">OpenStreetMapLayer</see></item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Note:</b>This also means that you can also use implied Binding in the ControlTemplate Style of the 
        /// Attribution Control for the various Properties of the special Types of Layers such as: .ID, .Url, 
        /// .Version, etc. 
        /// </para>
        /// <para>
        /// Both the Attribution.PropertyChanged and Attribution.Refreshed Events fire as a result of the Layers 
        /// that Implement IAttribution in the LayerCollection being added or removed in the Attribution.Layers 
        /// Property. Use the Attribution.Refreshed Event if you want to add, delete, or modify the default 
        /// attribution items given by the framework. Use the Attribution.PropertyChanged Event on property 
        /// Attribution.Items, if you want to be aware of this change in order to hook up an Event fired by 
        /// Attribution.Items. <b>Note:</b> The various special Layer types that Implement the IAttribution 
        /// Interface have <b>Copyright</b> information Properties that are ReadOnly (meaning they can't be changed 
        /// by the client application) and hence it is impossible force the Attribution.PropertyChanged and 
        /// Attribution.Refreshed Events to update as a result of trying to modify the <b>Copyright</b> 
        /// information on the client side. 
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// By default three Layers have been added to the Map Control and the Copyright information for those 
        /// Layers are displayed in the Attribution Control. Click the 'Interrogate Attribution.Items' button to 
        /// loop through the ObservableCollection(Of IAttribution) object obtained by the Attribution.Items Property 
        /// to display various information about items in the Control.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Interrogating the Attribution.Items and displaying the results in a MessageBox." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Attribution.Items.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="65" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
        ///              TextWrapping="Wrap" Margin="2,2,0,0" 
        ///              Text="By default three Layers have been added to the Map Control and the CopyrightText information
        ///              for those Layers are displayed in the Attribution Control. Click the 'Interrogate Attribution.Items' 
        ///              button to loop through the ObservableCollection(Of IAttribution) object obtained by the 
        ///              Attribution.Items Property to display various information about items in the Control." /&gt;
        ///   
        ///   &lt;!-- Add a Map Control zoomed to a specific Extent. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,105,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="303" Width="600" 
        ///             Extent="-10723833,4627392,-10704398,4644168"&gt;
        ///   
        ///     &lt;!-- 
        ///     Add an ArcGISTiledMapServiceLayer. Note: since an ID value is specified the design-time will 
        ///     list the Layer ID in Attribution Control placeholder (Ex: World Imagery attribution.).
        ///     --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="World Imagery"
        ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"/&gt;
        ///       
        ///     &lt;!-- 
        ///     Add an ArcGISDynamicMapServiceLayer. Note: since an ID value is specified the design-time will 
        ///     list the Layer ID in Attribution Control placeholder (Ex: World Transportation attribution.).
        ///     --&gt;
        ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="World Transportation"
        ///           Url="http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer" /&gt;
        ///     
        ///     &lt;!-- 
        ///     Add a FeatureLayer. Note: since an ID value is specified the design-time will 
        ///     list the Layer ID in Attribution Control placeholder (Ex: Local Rivers attribution.).
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="Local Rivers"
        ///           Url="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Hydrography/Watershed173811/FeatureServer/1"
        ///           AutoSave="False" DisableClientCaching="True" OutFields="*" Mode="OnDemand" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add an Attribution Control and Bind the Map.Layers to the Attribution.Layers. Change a few
        ///   visual appearance settings related to the Font for the text that is displayed in the Control. 
        ///   --&gt;
        ///   &lt;esri:Attribution HorizontalAlignment="Left" VerticalAlignment="Top" Margin="12,420,0,0" Name="Attribution1" 
        ///                     Layers="{Binding ElementName=Map1,Path=Layers}" Width="600" Height="48" 
        ///                     FontSize="12" FontFamily="Courier New" Foreground="Green" /&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a button to dynamically interrogate each IAttribution object in the ObservableCollection using the 
        ///   Attribution.Items Property. Note the Click event handler is wired up to use code-behind. 
        ///   --&gt;
        ///   &lt;Button Content="Interrogate Attribution.Items" Height="23" HorizontalAlignment="Left" 
        ///           Margin="12,76,0,0" Name="Button_Interrogate_AttributeItems" 
        ///           VerticalAlignment="Top" Width="600" Click="Button_Interrogate_AttributeItems_Click"/&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button_Interrogate_AttributeItems_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function loops through all of the ObservabaleCollection&lt;IAttribution&gt; items of the Attribution
        ///   // Control to display various information in a MessageBox.
        ///   
        ///   // Get the ObservabaleCollection&lt;IAttribution&gt; object.
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.IAttribution&gt; theObservableCollectionOfIAttribution = null;
        ///   theObservableCollectionOfIAttribution = Attribution1.Items;
        ///   
        ///   // Get the count of the number of items in the ObservabaleCollection&lt;IAttribution&gt;.
        ///   int theCount = theObservableCollectionOfIAttribution.Count;
        ///   
        ///   // Define a StringBuilder to display information back to the user about the Attribution Control.
        ///   Text.StringBuilder myStringBuilder = new Text.StringBuilder();
        ///   
        ///   // Add the count of items to the StringBuilder.
        ///   myStringBuilder.Append("There are " + theCount.ToString() + " Layers in the Attribution Control." + Environment.NewLine);
        ///   
        ///   // Loop through each item in the ObservabaleCollection&lt;IAttribution&gt;.
        ///   foreach (ESRI.ArcGIS.Client.IAttribution oneIAttribution in theObservableCollectionOfIAttribution)
        ///   {
        ///     // Define some variables for CopyrightText and Layer.ID information.
        ///     string theCopyrightText = null;
        ///     string theLayerID = null;
        ///     
        ///     // FYI: The ESRI.ArcGIS.Client.IAttribution object are really various types of Layer objects!
        ///     
        ///     // Not every Type of Layer has a .AttributionTemplate Property which is the internal mechanism for 
        ///     // constructing the ESRI.ArcGIS.Client.IAttribution object. Only those Layers which have a
        ///     // .AttributionTemplate Property will get listed in the ObservabaleCollection&lt;IAttribution&gt; as 
        ///     // a result of the Attribution.Items Property. Some of theses special Types of Layers that have a 
        ///     // .AttributionTemplate Property are used below to obtain the various CopyrightText and Layer.ID 
        ///     // information
        ///     
        ///     if (oneIAttribution is ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)
        ///     {
        ///       ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer oneArcGISDynamicMapServiceLayer = null;
        ///       oneArcGISDynamicMapServiceLayer = oneIAttribution as ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer;
        ///       theCopyrightText = oneArcGISDynamicMapServiceLayer.CopyrightText;
        ///       theLayerID = oneArcGISDynamicMapServiceLayer.ID;
        ///     }
        ///     else if (oneIAttribution is ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer)
        ///     {
        ///       ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer oneArcGISTiledMapServiceLayer = null;
        ///       oneArcGISTiledMapServiceLayer = oneIAttribution as ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer;
        ///       theCopyrightText = oneArcGISTiledMapServiceLayer.CopyrightText;
        ///       theLayerID = oneArcGISTiledMapServiceLayer.ID;
        ///     }
        ///     else if (oneIAttribution is ESRI.ArcGIS.Client.FeatureLayer)
        ///     {
        ///       ESRI.ArcGIS.Client.FeatureLayer oneFeatureLayer = null;
        ///       oneFeatureLayer = oneIAttribution as ESRI.ArcGIS.Client.FeatureLayer;
        ///       ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo oneFeatureLayerInfo = oneFeatureLayer.LayerInfo;
        ///       theCopyrightText = oneFeatureLayerInfo.CopyrightText;
        ///       theLayerID = oneFeatureLayer.ID;
        ///     }
        ///     
        ///     // Append the CopyrightText and Layer.ID information about the Layer in the StringBuilder.
        ///     myStringBuilder.Append(Environment.NewLine);
        ///     myStringBuilder.Append("Layer: " + theLayerID + " has the CopyrightText: " + theCopyrightText + Environment.NewLine);
        ///   }
        ///   
        ///   // Display the information about the Attribute Control to the user. 
        ///   MessageBox.Show(myStringBuilder.ToString());
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_Interrogate_AttributeItems_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function loops through all of the ObservabaleCollection(Of IAttribution) items of the Attribution
        ///   ' Control to display various information in a MessageBox.
        ///   
        ///   ' Get the ObservabaleCollection(Of IAttribution) object.
        ///   Dim theObservableCollectionOfIAttribution As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.IAttribution)
        ///   theObservableCollectionOfIAttribution = Attribution1.Items
        ///   
        ///   ' Get the count of the number of items in the ObservabaleCollection(Of IAttribution).
        ///   Dim theCount As Integer = theObservableCollectionOfIAttribution.Count
        ///   
        ///   ' Define a StringBuilder to display information back to the user about the Attribution Control.
        ///   Dim myStringBuilder As New Text.StringBuilder
        ///   
        ///   ' Add the count of items to the StringBuilder.
        ///   myStringBuilder.Append("There are " + theCount.ToString + " Layers in the Attribution Control." + vbCrLf)
        ///   
        ///   ' Loop through each item in the ObservabaleCollection(Of IAttribution).
        ///   For Each oneIAttribution As ESRI.ArcGIS.Client.IAttribution In theObservableCollectionOfIAttribution
        ///     
        ///     ' Define some variables for CopyrightText and Layer.ID information.
        ///     Dim theCopyrightText As String = Nothing
        ///     Dim theLayerID As String = Nothing
        ///     
        ///     ' FYI: The ESRI.ArcGIS.Client.IAttribution object are really various types of Layer objects!
        ///     
        ///     ' Not every Type of Layer has a .AttributionTemplate Property which is the internal mechanism for 
        ///     ' constructing the ESRI.ArcGIS.Client.IAttribution object. Only those Layers which have a
        ///     ' .AttributionTemplate Property will get listed in the ObservabaleCollection(Of IAttribution) as 
        ///     ' a result of the Attribution.Items Property. Some of theses special Types of Layers that have a 
        ///     ' .AttributionTemplate Property are used below to obtain the various CopyrightText and Layer.ID 
        ///     ' information
        ///     
        ///     If TypeOf oneIAttribution Is ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer Then
        ///       Dim oneArcGISDynamicMapServiceLayer As ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer = Nothing
        ///       oneArcGISDynamicMapServiceLayer = TryCast(oneIAttribution, ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)
        ///       theCopyrightText = oneArcGISDynamicMapServiceLayer.CopyrightText
        ///       theLayerID = oneArcGISDynamicMapServiceLayer.ID
        ///     ElseIf TypeOf oneIAttribution Is ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer Then
        ///       Dim oneArcGISTiledMapServiceLayer As ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer = Nothing
        ///       oneArcGISTiledMapServiceLayer = TryCast(oneIAttribution, ESRI.ArcGIS.Client.ArcGISTiledMapServiceLayer)
        ///       theCopyrightText = oneArcGISTiledMapServiceLayer.CopyrightText
        ///       theLayerID = oneArcGISTiledMapServiceLayer.ID
        ///     ElseIf TypeOf oneIAttribution Is ESRI.ArcGIS.Client.FeatureLayer Then
        ///       Dim oneFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = Nothing
        ///       oneFeatureLayer = TryCast(oneIAttribution, ESRI.ArcGIS.Client.FeatureLayer)
        ///       Dim oneFeatureLayerInfo As ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo = oneFeatureLayer.LayerInfo
        ///       theCopyrightText = oneFeatureLayerInfo.CopyrightText
        ///       theLayerID = oneFeatureLayer.ID
        ///     End If
        ///     
        ///     ' Append the CopyrightText and Layer.ID information about the Layer in the StringBuilder.
        ///     myStringBuilder.Append(vbCrLf)
        ///     myStringBuilder.Append("Layer: " + theLayerID + " has the CopyrightText: " + theCopyrightText + vbCrLf)
        ///     
        ///   Next
        ///   
        ///   ' Display the information about the Attribute Control to the user. 
        ///   MessageBox.Show(myStringBuilder.ToString)
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public ObservableCollection<IAttribution> Items
		{
			get { return _items; }
			internal set
			{
				_items = value;
				OnRefreshed();
				OnPropertyChanged("Items");
			}
		} 
		#endregion

		#region Event Refreshed
		/// <summary>
		/// Occurs when the attribution items are refreshed. 
		/// Give the opportunity for an application to add or remove attribution items.
		/// </summary>
		public event EventHandler<EventArgs> Refreshed;

		private void OnRefreshed()
		{
			EventHandler<EventArgs> refreshed = Refreshed;

			if (refreshed != null)
			{
				refreshed(this, EventArgs.Empty);
			}
		}
		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Map Event Handlers


		private void DetachLayersHandler(LayerCollection layers)
		{
			if (layers != null)
			{
				layers.LayersInitialized -= Layers_LayersInitialized;
				layers.CollectionChanged -= Layers_CollectionChanged;
				foreach (Layer layer in layers)
					DetachLayerHandler(layer);
			}
		}

		private void AttachLayersHandler(LayerCollection layers)
		{
			if (layers != null)
			{
				layers.CollectionChanged += Layers_CollectionChanged;

				// if one layer is not initialized, subscribe to Layers_Initialize event in order to update attribution items
				// This update is only useful to avoid blank lines (for any reason, a TextBlock with a null string has a null height the first time and a non null height after setting again the text value to null)
				if (!layers.All(layer => layer.IsInitialized))
					layers.LayersInitialized += Layers_LayersInitialized;
				foreach (Layer layer in layers)
					AttachLayerHandler(layer);
			}
		}

		private void AttachLayerHandler(Layer layer)
		{
			layer.PropertyChanged += layer_PropertyChanged;
		}

		private void DetachLayerHandler(Layer layer)
		{
			layer.PropertyChanged -= layer_PropertyChanged;
		}

		private void layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible" || e.PropertyName == "MaximumResolution" || e.PropertyName == "MinimumResolution")
				UpdateAttributionItems();
		}

		void Layers_LayersInitialized(object sender, EventArgs args)
		{
			LayerCollection layers = sender as LayerCollection;
			if (layers == null)
				return;
			UpdateAttributionItems();
		}

		private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var oldItems = e.OldItems;
			var newItems = e.NewItems;
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
				newItems = Layers;
			if (oldItems != null)
				foreach (var item in oldItems)
					DetachLayerHandler(item as Layer);
			if (newItems != null)
				foreach (var item in newItems)
					AttachLayerHandler(item as Layer);
			UpdateAttributionItems();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the attribution items with the layers implementing IAttribution and returning non null AttributionTemplate.
		/// </summary>
		private void UpdateAttributionItems()
		{
			if (Layers == null) {
				Items = null;
				return;
			}

			if (DesignerProperties.GetIsInDesignMode(this))
				// Design mode : create a dummy entry by layer implementing IAttribution with non null AttributionTemplate
				Items = Layers.OfType<IAttribution>().Where(attrib => attrib.AttributionTemplate != null).Select(attribution => (IAttribution)new DesignAttribution((Layer)attribution)).ToObservableCollection();
			else {
				// Non design mode : filter on implementation of IAttribution and non null AttributionTemplate

				IEnumerable<Layer> visibleLayers = EnumerateLeafLayers(Layers);
				Items = visibleLayers.OfType<IAttribution>().Where(attrib => attrib.AttributionTemplate != null).ToObservableCollection();
			}
		}

		private IEnumerable<Layer> EnumerateLeafLayers(IEnumerable<Layer> layers)
		{
			foreach (var l in layers)
			{
				if (!l.Visible) continue;				
				if (l is GroupLayerBase && !(l is IAttribution))
				{
					foreach (var l2 in EnumerateLeafLayers((l as GroupLayerBase).ChildLayers))
						yield return l2;
				}
				else
					yield return l;
			}
		}
		#endregion

		#region internal class DesignAttribution
		/// <summary>
		/// Attribution item for design mode
		/// </summary>
		internal class DesignAttribution : IAttribution
		{
			public DesignAttribution(Layer layer)
			{
				Copyright = string.Format(Properties.Resources.Attribution_Copyright, layer.ID ?? layer.GetType().Name);
			}

			static DesignAttribution()
			{
				CreateAttributionTemplate();
			}

			public string Copyright { get; private set; } 

			public DataTemplate AttributionTemplate
			{
				get
				{
					return _attributionTemplate;
				}
			}

			private const string template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><TextBlock Text=""{Binding Copyright}""/></DataTemplate>";

			private static DataTemplate _attributionTemplate;
			private static void CreateAttributionTemplate()
			{
#if SILVERLIGHT
				_attributionTemplate = System.Windows.Markup.XamlReader.Load(template) as DataTemplate;
#else
				System.IO.MemoryStream stream = new System.IO.MemoryStream(
					System.Text.Encoding.UTF8.GetBytes(template));
				_attributionTemplate = System.Windows.Markup.XamlReader.Load(stream) as DataTemplate;
#endif
			}
		}
 
		#endregion
	}
}
