// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Toolkit.Primitives;

namespace ESRI.ArcGIS.Client.Toolkit
{
 /// <summary>
 /// The Legend is a utility control that displays the symbology of Layers and their description for map 
 /// interpretation. By altering its various DataTemplates, the Legend Control functionality can be altered 
 /// to behave like an interactive Table of Contents (TOC).
 /// </summary>
 /// <remarks>
 /// <para>
 /// The Legend Control is a User Interface (UI) control which provides useful narrative and graphic descriptions for 
 /// understanding what is being viewed in the Map. It displays the 
 /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> name, sub-layer name(s) (via the 
 /// <see cref="ESRI.ArcGIS.Client.LayerInfo.Name">LayerInfo.Name</see> Property), and any associated graphical 
 /// symbols with their Label(s) (using the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel.LegendItems">LayerItemViewModel.LegendItems</see> 
 /// Collection). Additionally, 
 /// ToolTip information can obtained revealing detailed Layer information (such as: 
 /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see>, 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel.SubLayerID">LayerItemViewModel.SubLayerID</see>, 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see>,
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Description">LegendItemViewModel.Description</see>,
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel.MinimumResolution">LayerItemViewModel.MinimumResolution</see>, 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel.MaximumResolution">LayerItemViewModel.MaximumResolution</see>, 
 /// etc.) when the user hovers the cursor over a Layer/sub-Layer name in the Legend. In its default mode of operation, 
 /// end-users interact with the Legend Control to expand and collapse the nodes to view as much information as desired 
 /// about a particular Layer. 
 /// </para>
 /// <para>
 /// In most use cases, developers typically only need to set a few core Properties in order to have a Legend that 
 /// displays useful information about the Layers in the Map. The core properties are:
 /// </para>
 /// <list type="bullet">
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.Map">Legend.Map</see> - This Property binds the Legend to the Map 
 ///   Control. It is required that this Property be set or else there will be no communication between the Map and the
 ///   Legend.
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerItemsMode">Legend.LayerItemsMode</see> - This Property 
 ///   controls whether the Legend displays information in an expanded <b>Tree</b> hierarchy for all Legend items or 
 ///   in a <b>Flat</b> structure which shows only the lowest Layer item leaves (The Layer.ID, 
 ///   LayerItemViewModel.SubLayerID, and group-layer labels hierarchy information will not be displayed). The 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.Mode">Legend.Mode</see> Enumeration is used to define the
 ///   <b>Tree</b> or <b>Flat</b> options. The default Enumeration value for the Legend.LayerItemsMode is 
 ///   <b>Flat</b>.
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.ShowOnlyVisibleLayers">Legend.ShowOnlyVisibleLayers</see> - This
 ///   Property controls the display of items in the Legend if they are visible in the Map. It accepts a
 ///   Boolean. The default is <b>True</b>, meaning that only visible layers in the Map will display in the Legend. A Layer
 ///   may have scale dependency rendering (i.e. the 
 ///   <see cref="ESRI.ArcGIS.Client.Layer.MinimumResolution">Layer.MinimumResolution</see> and/or 
 ///   <see cref="ESRI.ArcGIS.Client.Layer.MaximumResolution">Layer.MaximumResolution</see>) values which 
 ///   set which limits when a Layer can be viewed in the Map at a particular zoom level. A Legend.ShowOnlyVisibleLayers
 ///   value of <b>False</b> means that even if the Layer is not currently displaying in the Map it will still be listed in 
 ///   the Legend.
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerIDs">Legend.LayerIDs</see> - This Property defines which Layers
 ///   are participating in the Legend Control. By default this value is set to null/Nothing; which means that all Layers
 ///   in the Map Control participate in the Legend Control. Developers can limit which Layers are listed in the Legend
 ///   Control (even though they are still viewable in the Map Control) by specifying a comma-delimmited string using 
 ///   <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> values. <b>Note:</b> Do not use Layer.ID values that 
 ///   contain embedded commas as this will cause problems tokenizing in the Legend.LayerIDs Property.
 ///   </item>
 /// </list>
 /// <para>
 /// The Legend Control is highly customizable such that developers can override the default behavior of the 
 /// various DataTemplates to alter the behavior of the control to make it behave like a TOC. A TOC provides the 
 /// same useful narrative and graphical information as a Legend but adds the extra functionality of allowing users 
 /// to turn on/off the visibility of individual Layers (and their sub-Layers where applicable). Depending on the 
 /// developers creativity even other specialized functions can be programmed into the DataTemplates to perform 
 /// functions like: controlling the opacity of individual Layers (and their sub-Layers where applicable), setting 
 /// thresholds at which scale a particular Layer is visible, allowing for a specific Layer to be selected and 
 /// highlighted in the Map, and more. Additional details on what parts of the Legend Control can be customized 
 /// by the various DataTemplate Properties will be discussed later in this document. 
 /// </para>
 /// <para>
 /// The Legend Control can be created at design time in XAML or dynamically at runtime in the code-behind. 
 /// <ESRISILVERLIGHT>The Legend Control is one of several controls available in the Toolbox in Visual Studio when the ArcGIS API for Silverlight is installed, </ESRISILVERLIGHT>
 /// <ESRIWPF>The Legend Control is one of several controls available in the Toolbox in Visual Studio when the ArcGIS Runtime SDK for WPF is installed, </ESRIWPF>
 /// <ESRIWINPHONE>There are no controls that can be dragged into the XAML design surface from the Toolbox as part of the ArcGIS Runtime SDK for Windows Phone installation. Developers need to type the correct syntax in the XAML to have the controls appear in the visual designer, </ESRIWINPHONE>
 /// see the following screen shot:
 /// </para>
 /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the Legend Control on the XAML design surface of a Silverlight application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend1.png"/></para></ESRISILVERLIGHT>
 /// <ESRIWPF><para><img border="0" alt="Example of the Legend Control on the XAML design surface of a WPF application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend_VS_WPF.png"/></para></ESRIWPF>
 /// <ESRIWINPHONE><para><img border="0" alt="Example of the Legend Control on the XAML design surface of a Windows Phone application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend_VS_WinPhone.png"/></para></ESRIWINPHONE>
 /// <para>
 /// The default appearance of the Legend Control can be modified using numerous inherited Properties from 
 /// System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. An example of some of 
 /// these Properties include: .Height, .Width, .BackGround, .BorderBrush, .BorderThickness, .Foreground, 
 /// .HorizontalAlignment, .VerticalAlignment, .Margin, .Opacity, .Visibility, etc. 
 /// </para>
 /// <para>
 /// <b>Note:</b> You cannot change the core behavior of the sub-components (i.e. Image, ToolTipService, StackPanel, 
 /// TextBlock, DataTemplate, Grid, ContentControl, ContentPresenter, 
 /// ESRI.ArcGIS.Client.Toolkit.Primitives.TreeViewExtended, etc.) of the Legend Control using standard Properties 
 /// or Methods alone. To change the core behavior of the sub-components and their appearance of the Control, 
 /// developers can modify the Control Template in XAML and the associated code-behind file. The easiest way to 
 /// modify the UI sub-components is using Microsoft Expression Blend. Then developers can delete/modify existing 
 /// or add new sub-components in Visual Studio to create a truly customized experience. A general approach to 
 /// customizing a Control Template is discussed in the ESRI blog entitled: 
 /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
 /// A specific code example of modifying the Control Template of the Legend Control to can be found in the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.Map">Legend.Map</see> Property document.
 /// </para>
 /// <para>
 /// Rather than edit the full Control Template of the Legend Control, ESRI has exposed three DataTemplates 
 /// Properties where developers can target the UI customization to a more limited scope. Each DataTemplate 
 /// and a description of what part of the Legend Control it has bearing over is listed here:
 /// </para>
 /// <list type="bullet">
 ///   <item>
 ///   <para>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.MapLayerTemplate">Legend.MapLayerTemplate</see>
 ///   </para>
 ///   <para>
 ///   This DataTemplate controls what is viewed in the Legend for the highest level of information about a 
 ///   particular Layer. It presents each Layer name (via the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see> Property) 
 ///   shown in the Map in a ContentControl with a node to expand any sub-Layer information. When a user hovers the 
 ///   cursor over the Legend.MapLayerTemplate section, a ToolTip appears displaying the additional information about the 
 ///   Layer including: Layer.Copyright, LegendItemViewModel.Description, LayerItemViewModel.MinimumResolution, 
 ///   and LayerItemViewModel.MaximumResolution. The MapLayerTemplate value is optional; by default the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see> is used. 
 ///   </para>
 ///   <para>
 ///   <b>Note:</b> The <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerItemsMode">Legend.LayerItemsMode</see> 
 ///   Property has great impact on what is displayed in the Legend. For 
 ///   users who want to see the most information available in the Legend, developers should set the LayerItemsMode 
 ///   to <b>Tree</b>. If customizations are made to the MapLayerTemplate and they seem to be overridden at 
 ///   runtime back to a default setting, it is most likely that the LayerItemsMode is set to the default of 
 ///   <b>Flat</b> and it should be set to <b>Tree</b>.
 ///   </para>
 ///   <para>
 ///   The objects that have Binding occur in the MapLayerTemplate are implied to be the Properties of the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel">LayerItemViewModel</see> Class.
 ///   </para>
 ///   <para>
 ///   At the MapLayerTemplate level, one common customization technique would be add a TOC style of interaction. 
 ///   Developers could add a CheckBox to manage the visibility of a Layer (including its sub-Layers) or add a 
 ///   Slider to control the Opacity of the Layer (including its sub-Layers). Code examples of modifying the 
 ///   MapLayerTemplate can be found in the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.MapLayerTemplate">Legend.MapLayerTemplate</see> and 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see> documents.
 ///   </para>
 ///   </item>
 ///   <item>
 ///   <para>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see>
 ///   </para>  
 ///   <para>
 ///   This DataTemplate controls what is viewed in the Legend for the middle level of information about a 
 ///   particular Layer. It presents each sub-Layer name (via the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see> 
 ///   Property) in a ContentControl with a node to expand any LegendItem information or any other nested 
 ///   sub-Layer (aka. group layer) information. When a user hovers the cursor over the Legend.LayerTemplate 
 ///   section, a ToolTip appears displaying the additional information about the sub-layer including: Layer.ID, 
 ///   LegendItemViewModel.Label, LegendItemViewModel.Description, LayerItemViewModel.SubLayerID, 
 ///   LayerItemViewModel.MinimumResolution, and LayerItemViewModel.MaximumResolution. This template is used 
 ///   by default for all Layers except the LegendItems as the lowest level. 
 ///   </para>
 ///   <para>
 ///   The objects that have Binding occur in the LayerTemplate are implied to be the Properties of the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel">LayerItemViewModel</see> Class.
 ///   </para>
 ///   <para>
 ///   At the Legend.LayerTemplate level, one common customization technique would be to add TOC style of interaction. 
 ///   Developers could add a CheckBox to manage the visibility of sub-Layer elements. You could add a Slider 
 ///   similar to the Legend.MapLayerTemplate and you will get Sliders appearing for each sub-Layer but the behavior 
 ///   may not be as you would expect. Adjusting a Slider on one sub-Layer would also control the Opacity of 
 ///   the other sub-Layers simultaneously. <b>Note:</b> FeatureLayers do not have the ability to control the 
 ///   visibility or opacity of sub-Layer elements. A code examples of modifying the Legend.LayerTemplate can be 
 ///   found in the
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see> document.
 ///   </para>
 ///   </item>
 ///   <item>
 ///   <para>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LegendItemTemplate">Legend.LegendItemTemplate</see>
 ///   </para>
 ///   <para>
 ///   This DataTemplate controls what is viewed in the Legend for the lowest level of information about a 
 ///   particular Layer. It presents each LegendItem via its image 
 ///   (<see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.ImageSource">LegendItemViewModel.ImageSource</see>) 
 ///   and label description 
 ///   (<see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see>). 
 ///   No ToolTip information is provided for an individual LegendItem by default. 
 ///   </para>
 ///   <para>
 ///   The objects that have Binding occur in the LegendItemTemplate are implied to be the Properties of the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel">LegendItemViewModel</see> Class.
 ///   </para>
 ///   <para>
 ///   At the Legend.LegendItemTemplate level, the customization options become more limited due to the map service 
 ///   information being passed back from the ArcGIS Server REST end point. The LegendItemViewModel class 
 ///   contains the most atomic level of information about a particular LegendItem. Turning on/off individual 
 ///   LegendItems or changing their opacity is not possible on any Layer type. It is possible to get creative 
 ///   and perform TOC style user interactions at the Legend.LegendItemtemplate level by discovering the parent objects 
 ///   of individual LegendItems; see the code example in the 
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LegendItemTemplate">Legend.LegendItemTemplate</see> 
 ///   for one such customization.
 /// </para>
 /// </item>
 /// </list>
 /// <para>
 /// It should be noted that it is possible to customize one or more of the Legend Control DataTemplates at the 
 /// same time. There are established default values for each DataTemplate, so setting one will not necessarily 
 /// override the others that have been set by default. Significant testing should be done on all of the Layers 
 /// in the customized application to ensure that the desired behavior has been achieved. Some Layers have 
 /// different behavior in rendering items in the Legend and should be tested thoroughly.
 /// </para>
 /// <para>
 /// The following screen shot demonstrates which part of the Legend Control corresponds to the three 
 /// DataTemplates. The Layer (ID = "United States") that is being displayed is an ArcGISDynamicMapServiceLayer 
 /// with three sub-Layers (ushigh, states, and counties). The information found in the ArcGIS Services Directory 
 /// about the ArcGISDynamicMapServiceLayer corresponds to what is shown in the Map and Legend Controls.
 /// </para>
 /// <para>
 /// <img border="0" alt="Using the ArcGIS Services Directory to view information about an ArcGISDynamicMapServiceLayer and how that corresponds to what is displayed in the Map and Legend Control. The DataTemplate parts of the Legend Control are specified." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.png"/>
 /// </para>
 /// <para>
 /// <b>TIP:</b> It is typically necessary for developers to specify the 
 /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> name for Layers in the Map Control. This can be done
 /// in XAML or code-behind. If a Layer.ID value is not specified, then a default Layer.ID value is provided
 /// based upon the URL of the ArcGIS Server map service.
 /// </para>
 /// </remarks>
 /// <example>
 /// <para>
 /// <b>How to use:</b>
 /// </para>
 /// <para>
 /// Click the 'Initialize the Legend' button to wire up the Legend Control with the Map Control. The core Legend 
 /// Control Properties of LayerItemsMode and ShowOnlyVisibleLayers will also have values set. Note: these same 
 /// functions could also be performed in XAML (see the XAML for details) rather than in code-behind.
 /// </para>
 /// <para>
 /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
 /// the functionality.
 /// </para>
 /// <para>
 /// The following screen shot corresponds to the code example in this page.
 /// </para>
 /// <para>
 /// <img border="0" alt="Example of binding a Map Control to the Legend Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend2.png"/>
 /// </para>
 /// <code title="Example XAML1" description="" lang="XAML">
 /// &lt;Grid x:Name="LayoutRoot"&gt;
 ///   
 ///   &lt;!-- Add a Map Control. --&gt;
 ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
 ///             VerticalAlignment="Top" Height="400" Width="400" 
 ///             Extent="-13874971,5811127,-13383551,6302547"&gt;
 ///   
 ///     &lt;!-- Add an ArcGISTiledMapServiceLayer. --&gt;
 ///     &lt;esri:ArcGISTiledMapServiceLayer ID="Topo" 
 ///             Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer"/&gt;
 ///         
 ///     &lt;!-- Add a FeatureLayer. --&gt;
 ///     &lt;esri:FeatureLayer ID="Congressional Districts" Opacity="0.4"
 ///                    Url="http://maps1.arcgisonline.com/ArcGIS/rest/services/USA_Congressional_Districts/MapServer/1"
 ///                    OutFields="*"/&gt;
 ///     
 ///     &lt;!-- Add an ArcGISDynamicMapServiceLayer. --&gt;
 ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="Amtrack"  
 ///            Url="http://maps1.arcgisonline.com/ArcGIS/rest/services/FRA_Amtrak_Stations/MapServer"/&gt;
 ///     
 ///   &lt;/esri:Map&gt;
 ///   
 ///   &lt;!--
 ///   Add a Legend Control to the application. Only the basic Properties have been set to place the
 ///   control on the page. The Properties to hook up the Legend with the Map, along with setting the
 ///   behavior of the control are done in code-behind when the user clicks Button1.
 ///   --&gt;
 ///   &lt;esri:Legend Name="Legend1" HorizontalAlignment="Left" VerticalAlignment="Top" 
 ///                Margin="418,188,0,0" Width="350" Height="400" /&gt;
 ///   
 ///   &lt;!-- 
 ///   Rather than use code-behind to set up the remainder of the core Properties on the Legend Control,
 ///   the Properties could have been specified at design-time in XAML. To see this, comment out the above
 ///   Legend Control (Legend1) and uncomment the below Legend Control (Legend2). 
 ///   --&gt;
 ///   &lt;!--
 ///   &lt;esri:Legend Name="Legend2" HorizontalAlignment="Left" VerticalAlignment="Top" 
 ///                Margin="418,188,0,0" Width="350" Height="400" 
 ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree" ShowOnlyVisibleLayers="False" /&gt;
 ///   --&gt;
 ///   
 ///   &lt;!--
 ///   This Button initializes the remainder of the core Propeties on the Legend Control.
 ///   --&gt;
 ///   &lt;Button Content="Initialize the Legend" Height="23" HorizontalAlignment="Left" Margin="12,159,0,0" 
 ///           Name="Button1" VerticalAlignment="Top" Width="756" 
 ///           Click="Button1_Click"/&gt;
 ///   
 ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
 ///   &lt;TextBlock Height="95" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
 ///          TextWrapping="Wrap" Margin="12,12,0,0" 
 ///          Text="Click the 'Initialize the Legend' button to wire up the Legend Control with the Map Control.
 ///              The core Legend Control Properties of LayerItemsMode and ShowOnlyVisibleLayers will also
 ///              have values set. Note: these same functions could also be performed in XAML (see the XAML
 ///              for details) rather than in code-behind." /&gt;
 ///       
 /// &lt;/Grid&gt;
 /// </code>
 /// <code title="Example CS1" description="" lang="CS">
 /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
 /// {
 ///   // Bind the Map to the Legend.
 ///   Legend1.Map = Map1;
 ///   
 ///   // Show the complete layer/item hierarchy.
 ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Tree;
 ///   
 ///   // All layers regardless of their visibility wil be shown in the Legend.
 ///   Legend1.ShowOnlyVisibleLayers = false;
 /// }
 /// </code>
 /// <code title="Example VB1" description="" lang="VB.NET">
 /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
 ///   
 ///   ' Bind the Map to the Legend.
 ///   Legend1.Map = Map1
 ///   
 ///   ' Show the complete layer/item hierarchy.
 ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Tree
 ///   
 ///   ' All layers regardless of their visibility wil be shown in the Legend.
 ///   Legend1.ShowOnlyVisibleLayers = False
 ///   
 /// End Sub
 /// </code>
 /// </example>
	public partial class Legend : Control
	{
		#region Constructor
		private bool _isLoaded = false;
		private LegendTree _legendTree;

		/// <summary>
		/// Initializes a new instance of the <see cref="Legend"/> class.
		/// </summary>
		public Legend()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(Legend);
#endif
			_legendTree = new LegendTree();
			_legendTree.Refreshed += new EventHandler<RefreshedEventArgs>(OnRefreshed);
			_legendTree.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(LegendTree_PropertyChanged);
		}

		void LegendTree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LayerItemsSource")
				LayerItemsSource = _legendTree.LayerItemsSource;
			else if (e.PropertyName == "LayerItems")
				LayerItems = _legendTree.LayerItems;
		}

		/// <summary>
		/// Static initialization for the <see cref="Legend"/> control.
		/// </summary>
		static Legend()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend),
				new FrameworkPropertyMetadata(typeof(Legend)));
#endif
		}

		#endregion

		#region Map
  /// <summary>
  /// Gets or sets the Map that the Legend Control is buddied to.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Setting this Property is required to have the Legend Control display information about the Layers in the Map.
  /// </para>
  /// <para>
  /// In the following XAML example, Binding is used to associate a Map Control named 'Map1' to the Legend Control’s 
  /// Map Property:<br/>
  /// <code lang="XAML">
  /// &lt;esri:Legend Name="Legend1" Map="{Binding ElementName=Map1}" /&gt;
  /// </code>
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// A customized Control Template is used to override the default Style of the Legend Control. Click the 
  /// 'Customized Layer info' button to see various pieces of information about a Layer. The ToolTip for 
  /// the Layer that is normally displayed in the Legend in the MapLayerTemplate was disabled.
  /// </para>
  /// <para>
  /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
  /// the functionality.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="Example of a customized Control Template for the Legend Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.Map.png"/>
  /// </para>
  /// <para>
  /// <b>SPECIAL INSTRUCTIONS: </b>This code example uses two 
  /// <ESRISILVERLIGHT>Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>WPF</ESRIWPF>
  /// <ESRIWINPHONE>Windows Phone</ESRIWINPHONE>
  ///  pages. The first page 
  /// should be named 'TheInfo.xaml' (with corresponding .vb and .cs code-behind files). The second page can use 
  /// whatever name you choose. Follow these instructions:  
  /// </para>
  /// <para>
  /// <b>First: </b>Add the 
  /// <ESRISILVERLIGHT>Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>WPF</ESRIWPF>
  /// <ESRIWINPHONE>Windows Phone</ESRIWINPHONE>
  ///  page named 'TheInfo.xaml' into your Visual Studio 
  /// project. Replace the full contents of the code-behind with the provided example code (depending on your 
  /// programming language). Note: there is no need to modify the .xaml file.
  /// </para>
  /// <code title="Example CS1" description="TheInfo.xaml.cs" lang="CS">
  /// // This file is the 'TheInfo.xaml.cs' file!
  /// using System;
  /// public partial class TheInfo : Page
  /// {
  ///   public TheInfo()
  ///   {
  ///     InitializeComponent();
  ///   }
  ///   
  ///   // Create several Properties (with Dependency Properties) that will be used to hold information about a Layer.
  ///   // The purpose of this Class is to store information via Binding in XAML to an Object. Then pass that Object 
  ///   // to the code-behind so that you can get access to all of the information.
  ///   
  ///   public string MyCopyrightText
  ///   {
  ///     get { return (string)GetValue(MyCopyrightTextProperty); }
  ///     set { SetValue(MyCopyrightTextProperty, value); }
  ///   }
  ///   public static readonly DependencyProperty MyCopyrightTextProperty = DependencyProperty.Register("MyCopyrightText", typeof(string), typeof(TheInfo), new PropertyMetadata(null));
  ///   
  ///   public string MyUrl
  ///   {
  ///     get { return (string)GetValue(MyUrlProperty); }
  ///     set { SetValue(MyUrlProperty, value); }
  ///   }
  ///   public static readonly DependencyProperty MyUrlProperty = DependencyProperty.Register("MyUrl", typeof(string), typeof(TheInfo), new PropertyMetadata(null));
  ///   
  ///   public string MyIsVisible
  ///   {
  ///     get { return (string)GetValue(MyIsVisibleProperty); }
  ///     set { SetValue(MyIsVisibleProperty, value); }
  ///   }
  ///   public static readonly DependencyProperty MyIsVisibleProperty = DependencyProperty.Register("MyIsVisible", typeof(string), typeof(TheInfo), new PropertyMetadata(null));
  ///   
  ///   public Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; MyLayerItems
  ///   {
  ///     get { return GetValue(MyLayerItemsProperty); }
  ///     set { SetValue(MyLayerItemsProperty, value); }
  ///   }
  ///   public static readonly DependencyProperty MyLayerItemsProperty = DependencyProperty.Register("MyLayerItems", typeof(Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt;), typeof(TheInfo), new PropertyMetadata(null));
  /// }
  /// </code>
  /// <code title="Example VB1" description="TheInfo.xaml.vb" lang="VB.NET">
  /// ' This file is the 'TheInfo.xaml.vb' file!
  /// 
  /// Partial Public Class TheInfo
  ///   Inherits Page
  ///   
  ///   Public Sub New()
  ///     InitializeComponent()
  ///   End Sub
  ///   
  ///   ' Create several Properties (with Dependency Properties) that will be used to hold information about a Layer.
  ///   ' The purpose of this Class is to store information via Binding in XAML to an Object. Then pass that Object 
  ///   ' to the code-behind so that you can get access to all of the information.
  ///   
  ///   Public Property MyCopyrightText() As String
  ///     Get
  ///       Return CStr(GetValue(MyCopyrightTextProperty))
  ///     End Get
  ///     Set(ByVal value As String)
  ///       SetValue(MyCopyrightTextProperty, value)
  ///     End Set
  ///   End Property
  ///   Public Shared ReadOnly MyCopyrightTextProperty As DependencyProperty = DependencyProperty.Register("MyCopyrightText", GetType(String), GetType(TheInfo), New PropertyMetadata(Nothing))
  ///   
  ///   Public Property MyUrl() As String
  ///     Get
  ///       Return CStr(GetValue(MyUrlProperty))
  ///     End Get
  ///     Set(ByVal value As String)
  ///       SetValue(MyUrlProperty, value)
  ///     End Set
  ///   End Property
  ///   Public Shared ReadOnly MyUrlProperty As DependencyProperty = DependencyProperty.Register("MyUrl", GetType(String), GetType(TheInfo), New PropertyMetadata(Nothing))
  ///   
  ///   Public Property MyIsVisible() As String
  ///     Get
  ///       Return CStr(GetValue(MyIsVisibleProperty))
  ///     End Get
  ///     Set(ByVal value As String)
  ///       SetValue(MyIsVisibleProperty, value)
  ///     End Set
  ///   End Property
  ///   Public Shared ReadOnly MyIsVisibleProperty As DependencyProperty = DependencyProperty.Register("MyIsVisible", GetType(String), GetType(TheInfo), New PropertyMetadata(Nothing))
  ///   
  ///   Public Property MyLayerItems() As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel)
  ///     Get
  ///       Return GetValue(MyLayerItemsProperty)
  ///     End Get
  ///     Set(ByVal value As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel))
  ///       SetValue(MyLayerItemsProperty, value)
  ///     End Set
  ///   End Property
  ///   Public Shared ReadOnly MyLayerItemsProperty As DependencyProperty = DependencyProperty.Register("MyLayerItems", GetType(Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel)), GetType(TheInfo), New PropertyMetadata(Nothing))
  ///   
  /// End Class
  /// </code>
  /// <para>
  /// <b>Second: </b>Add another 
  /// <ESRISILVERLIGHT>Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>WPF</ESRIWPF>
  /// <ESRIWINPHONE>Windows Phone</ESRIWINPHONE>
  /// corresponding code-behind (.cs or .vb) will be the main driver for the application sample code. 
  /// </para>
  /// <code title="Example XAML2" description="Main driver XAML page." lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot"&gt;
  ///   
  ///   &lt;!-- 
  ///   Use the Resources section to hold a Style for setting the appearance and behavior of the Legend Control. 
  ///   Don't forget to add following XAML Namespace definitions to the correct location in your code:
  ///   xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk" 
  ///   xmlns:esri="http://schemas.esri.com/arcgis/client/2009"
  ///   xmlns:esriToolkitPrimitives="clr-namespace:ESRI.ArcGIS.Client.Toolkit.Primitives;assembly=ESRI.ArcGIS.Client.Toolkit"
  ///   xmlns:local="clr-namespace:TestLegend"
  ///       
  ///   NOTE: You will need to modify your code for the XAML Namespace definition for using local resources of 
  ///   (xmlns:local="clr-namespace:TestLegend") to match the name of your Project file. For example if your project 
  ///   was called 'BillyBob', the XAML Namespace definition would need to be changed to: 
  ///   xmlns:local="clr-namespace:BillyBob"
  ///   --&gt;
  ///   
  ///   &lt;Grid.Resources&gt;
  ///     
  ///     &lt;!--
  ///     The majority of the XAML that defines the ControlTemplate for the Legend Control was obtained
  ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
  ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
  ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
  ///     --&gt;
  ///     &lt;Style x:Key="LegendStyle1" TargetType="esri:Legend"&gt;
  ///       &lt;Setter Property="Foreground" Value="Black"/&gt;
  ///       &lt;Setter Property="Background"&gt;
  ///         &lt;Setter.Value&gt;
  ///         
  ///           &lt;!-- 
  ///           Change the default visual appearance of the Legend Control. Rather going from a White to 
  ///           Gray Background (top to bottom), mix things up by going from a Green to Yellow to Magenta
  ///           Background. 
  ///           --&gt;
  ///           &lt;LinearGradientBrush EndPoint=".7,1" StartPoint=".7,0"&gt;
  ///             &lt;GradientStop Color="Green" Offset="0"/&gt;
  ///             &lt;GradientStop Color="GreenYellow" Offset="0.375"/&gt;
  ///             &lt;GradientStop Color="Salmon" Offset="0.625"/&gt;
  ///             &lt;GradientStop Color="Magenta" Offset="1"/&gt;
  ///           &lt;/LinearGradientBrush&gt;
  ///           
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///       &lt;Setter Property="BorderBrush"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0"&gt;
  ///             &lt;GradientStop Color="#FFA3AEB9" Offset="0"/&gt;
  ///             &lt;GradientStop Color="#FF8399A9" Offset="0.375"/&gt;
  ///             &lt;GradientStop Color="#FF718597" Offset="0.375"/&gt;
  ///             &lt;GradientStop Color="#FF617584" Offset="1"/&gt;
  ///           &lt;/LinearGradientBrush&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///       &lt;Setter Property="HorizontalContentAlignment" Value="Left"/&gt;
  ///       &lt;Setter Property="VerticalContentAlignment" Value="Top"/&gt;
  ///       &lt;Setter Property="Cursor" Value="Arrow"/&gt;
  ///       &lt;Setter Property="BorderThickness" Value="1"/&gt;
  ///       &lt;Setter Property="Padding" Value="0"/&gt;
  ///       &lt;Setter Property="Margin" Value="0"/&gt;
  ///       &lt;Setter Property="IsTabStop" Value="False"/&gt;
  ///       &lt;Setter Property="TabNavigation" Value="Once"/&gt;
  ///       &lt;Setter Property="LayerItemsMode" Value="Flat"/&gt;
  ///       &lt;Setter Property="LegendItemTemplate"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;DataTemplate&gt;
  ///             &lt;StackPanel Margin="0,-1" Orientation="Horizontal"&gt;
  ///               &lt;Image HorizontalAlignment="Center" MaxWidth="55" MaxHeight="55" Margin="0,-1" MinWidth="20" Source="{Binding ImageSource}" Stretch="None" VerticalAlignment="Center"/&gt;
  ///               &lt;TextBlock Margin="5,0,0,0" Text="{Binding Label}" VerticalAlignment="Center"/&gt;
  ///             &lt;/StackPanel&gt;
  ///           &lt;/DataTemplate&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///       &lt;Setter Property="LayerTemplate"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;DataTemplate&gt;
  ///             &lt;StackPanel Margin="0,-1" Orientation="Horizontal"&gt;
  ///               &lt;ToolTipService.ToolTip&gt;
  ///                 &lt;StackPanel MaxWidth="400"&gt;
  ///                   &lt;TextBlock FontWeight="Bold" TextWrapping="Wrap" Text="{Binding Layer.ID}"/&gt;
  ///                   &lt;TextBlock FontWeight="Bold" TextWrapping="Wrap" Text="{Binding Label}"/&gt;
  ///                   &lt;TextBlock TextWrapping="Wrap" Text="{Binding Description}"/&gt;
  ///                   &lt;TextBlock Text="{Binding SubLayerID, StringFormat=SubLayer ID : \{0\}}"/&gt;
  ///                   &lt;TextBlock Text="{Binding MinimumResolution, StringFormat=Minimum Resolution : \{0:F6\}}"/&gt;
  ///                   &lt;TextBlock Text="{Binding MaximumResolution, StringFormat=Maximum Resolution : \{0:F6\}}"/&gt;
  ///                 &lt;/StackPanel&gt;
  ///               &lt;/ToolTipService.ToolTip&gt;
  ///               &lt;TextBlock Text="{Binding Label}" VerticalAlignment="Center"/&gt;
  ///             &lt;/StackPanel&gt;
  ///           &lt;/DataTemplate&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///       &lt;Setter Property="MapLayerTemplate"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;DataTemplate&gt;
  ///             &lt;StackPanel Margin="0,-1"&gt;
  ///               &lt;StackPanel.Resources&gt;
  ///                 &lt;DataTemplate x:Key="BusyIndicatorTemplate"&gt;
  ///                   &lt;Grid x:Name="BusyIndicator" Background="Transparent" HorizontalAlignment="Left" Margin="3,0" RenderTransformOrigin="0.5,0.5" VerticalAlignment="Center"&gt;
  ///                     &lt;Grid.RenderTransform&gt;
  ///                       &lt;RotateTransform/&gt;
  ///                     &lt;/Grid.RenderTransform&gt;
  ///                     &lt;Grid.Triggers&gt;
  ///                       &lt;EventTrigger RoutedEvent="Canvas.Loaded"&gt;
  ///                         &lt;BeginStoryboard&gt;
  ///                           &lt;Storyboard&gt;
  ///                             &lt;DoubleAnimation Duration="0:0:1" RepeatBehavior="Forever" To="360" Storyboard.TargetProperty="(UIElement.RenderTransform).(RotateTransform.Angle)" Storyboard.TargetName="BusyIndicator"/&gt;
  ///                           &lt;/Storyboard&gt;
  ///                         &lt;/BeginStoryboard&gt;
  ///                       &lt;/EventTrigger&gt;
  ///                     &lt;/Grid.Triggers&gt;
  ///                     &lt;Ellipse Fill="#1E525252" Height="2" Margin="11,2,11,20" Width="2"/&gt;
  ///                     &lt;Ellipse Fill="#3F525252" HorizontalAlignment="Right" Height="3" Margin="0,4,5,0" VerticalAlignment="Top" Width="3"/&gt;
  ///                     &lt;Ellipse Fill="#7F525252" HorizontalAlignment="Right" Height="4" Margin="0,9,1,0" VerticalAlignment="Top" Width="4"/&gt;
  ///                     &lt;Ellipse Fill="#BF525252" HorizontalAlignment="Right" Height="5" Margin="0,0,3,3" VerticalAlignment="Bottom" Width="5"/&gt;
  ///                     &lt;Ellipse Fill="#FF525252" Height="6" Margin="9,0" VerticalAlignment="Bottom" Width="6"/&gt;
  ///                   &lt;/Grid&gt;
  ///                 &lt;/DataTemplate&gt;
  ///               &lt;/StackPanel.Resources&gt;
  ///                             
  ///               &lt;!-- 
  ///               Comment out the default ability to do a ToolTip for the MapLayerTemplate. We will replace with a 
  ///               Button that displays some customized Layer information.
  ///               --&gt;
  ///               &lt;!--
  ///               &lt;ToolTipService.ToolTip&gt;
  ///                 &lt;StackPanel MaxWidth="400"&gt;
  ///                   &lt;TextBlock FontWeight="Bold" TextWrapping="Wrap" Text="{Binding Layer.CopyrightText}"/&gt;
  ///                   &lt;TextBlock TextWrapping="Wrap" Text="{Binding Description}"/&gt;
  ///                   &lt;TextBlock Text="{Binding MinimumResolution, StringFormat=Minimum Resolution : \{0:F6\}}"/&gt;
  ///                   &lt;TextBlock Text="{Binding MaximumResolution, StringFormat=Maximum Resolution : \{0:F6\}}"/&gt;
  ///                 &lt;/StackPanel&gt;
  ///               &lt;/ToolTipService.ToolTip&gt;
  ///               --&gt;
  ///                 
  ///               &lt;StackPanel Orientation="Horizontal"&gt;
  ///                 &lt;ContentControl ContentTemplate="{StaticResource BusyIndicatorTemplate}" Visibility="{Binding BusyIndicatorVisibility}"/&gt;
  ///                 &lt;TextBlock FontWeight="Bold" Text="{Binding Label}" VerticalAlignment="Center"/&gt;
  ///                   
  ///                                   
  ///                 &lt;!-- 
  ///                 IMPORTANT INFORMATION:
  ///                 Add a Button that displays customized Layer information next to the Layer.Label. The key to making 
  ///                 this logic work is to add a custom .xaml page (called TheInfo.xaml with the code-behind file 
  ///                 TheInfo.xaml.vb/TheInfo.xaml.cs) that contains the ability to set/get some Properties (with 
  ///                 Dependency Properties). The 'TheInfo' object is then bound to the Button.Tag Property so that we 
  ///                 can use the 'TheInfo' object's information in the code-behind. Remember that you need to add the 
  ///                 correct XAML Namespace definition at the top of your XAML code in order to use the local 
  ///                 'TheInfo' object.
  ///                   
  ///                 The binding that is occuring to our custom 'TheInfo' object follows the same pattern that is
  ///                 used in other parts of this Style. For the MapLayerTemplate is it implied that Binding is
  ///                 occuring to the LayerItemViewModel object. 
  ///                 --&gt;
  ///                 &lt;local:TheInfo x:Name="MyTheInfo" 
  ///                                MyCopyrightText="{Binding Layer.CopyrightText}" 
  ///                                MyUrl="{Binding Layer.Url}"
  ///                                MyIsVisible="{Binding IsVisible}"
  ///                                MyLayerItems="{Binding LayerItems}"/&gt;
  ///                 
  ///                 &lt;Button x:Name="TheButton" Content="Customized Layer info" Click="TheButton_Click" 
  ///                         Tag="{Binding ElementName=MyTheInfo}"/&gt;
  ///                    
  ///               &lt;/StackPanel&gt;
  ///             &lt;/StackPanel&gt;
  ///           &lt;/DataTemplate&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///       &lt;Setter Property="Template"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;ControlTemplate TargetType="esri:Legend"&gt;
  ///             &lt;esriToolkitPrimitives:TreeViewExtended BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" Background="{TemplateBinding Background}" Foreground="{TemplateBinding Foreground}" HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}" ItemsSource="{TemplateBinding LayerItemsSource}" Padding="{TemplateBinding Padding}" VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"&gt;
  ///               &lt;esriToolkitPrimitives:TreeViewExtended.ItemTemplate&gt;
  ///                 &lt;sdk:HierarchicalDataTemplate ItemsSource="{Binding LayerItemsSource}"&gt;
  ///                   &lt;ContentPresenter ContentTemplate="{Binding Template}" Content="{Binding}"/&gt;
  ///                 &lt;/sdk:HierarchicalDataTemplate&gt;
  ///               &lt;/esriToolkitPrimitives:TreeViewExtended.ItemTemplate&gt;
  ///             &lt;/esriToolkitPrimitives:TreeViewExtended&gt;
  ///           &lt;/ControlTemplate&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///     &lt;/Style&gt;
  ///   &lt;/Grid.Resources&gt;
  ///   
  ///   &lt;!-- Add a Map control with a couple of Layers. --&gt;
  ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
  ///             VerticalAlignment="Top" Height="400" Width="400" &gt;
  ///   
  ///     &lt;esri:ArcGISTiledMapServiceLayer ID="Street Map" 
  ///             Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer"/&gt;
  ///     
  ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="United States" Opacity="0.6" 
  ///            Url="http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer"/&gt;
  ///     
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!-- 
  ///   Add a Legend Control. Bind the Legend.Map Property to a Map Control. Define the Style of the Legend to use the 
  ///   Control Template that was generated in Blend and modified here in Visual Studio (see above).
  ///   --&gt;
  ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="418,188,0,0" Name="Legend1" 
  ///                VerticalAlignment="Top" Width="350" Height="400" 
  ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree"
  ///                Style="{StaticResource LegendStyle1}" /&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="70" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="616" 
  ///            TextWrapping="Wrap" Margin="12,12,0,0" 
  ///            Text="A customized Control Template is used to override the default Style of the Legend Control.
  ///              Click the 'Customized Layer info' button to see various pieces of information about a Layer.
  ///              The ToolTip for the Layer that is normally displayed in the Legend in the MapLayerTemplate
  ///              was disabled." /&gt;
  ///   
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS2" description="Main driver code-behind (.cs) page." lang="CS">
  /// private void TheButton_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function occurs when the user clicks the Button with the text 'Customized Layer info' next to 
  ///   // each Layer in the Legend. An object based on custom TheInfo Class is passed into this function via
  ///   // the Button.Tag Property. We extract out information from the TheInfo object to construct an
  ///   // informational string to display to the user via a MessageBox.
  ///   
  ///   // Get the Button.Tag information (i.e. a TheInfo Class object).
  ///   TheInfo myTheInfo = sender.Tag;
  ///   
  ///   // Construct the informational string (using the StringBuilder Class).
  ///   Text.StringBuilder myDisplayString = new Text.StringBuilder();
  ///   
  ///   // Add information into the StringBuilder.
  ///   myDisplayString.Append("Selected information about the Layer:" + Environment.NewLine);
  ///   myDisplayString.Append("Copyright: " + myTheInfo.MyCopyrightText + Environment.NewLine);
  ///   myDisplayString.Append("Url: " + myTheInfo.MyUrl + Environment.NewLine);
  ///   myDisplayString.Append("IsVisible: " + myTheInfo.MyIsVisible + Environment.NewLine);
  ///   
  ///   // The TheInfo.MyLayerItems Property is a bit more complex that a simple String. It actually contains
  ///   // the information from the Layer.LayerItems Property. Use this object to display how many sub-Layers
  ///   // are associated with this particular Layer.
  ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; myLayerItems = null;
  ///   myLayerItems = myTheInfo.MyLayerItems;
  ///   myDisplayString.Append("Number of sub-Layers: " + myLayerItems.Count.ToString());
  ///   
  ///   // Display the information to the user.
  ///   MessageBox.Show(myDisplayString.ToString());
  /// }
  /// </code>
  /// <code title="Example VB2" description="Main driver code-behind (.vb) page." lang="VB.NET">
  /// Private Sub TheButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function occurs when the user clicks the Button with the text 'Customized Layer info' next to 
  ///   ' each Layer in the Legend. An object based on custom TheInfo Class is passed into this function via
  ///   ' the Button.Tag Property. We extract out information from the TheInfo object to construct an
  ///   ' informational string to display to the user via a MessageBox.
  ///   
  ///   ' Get the Button.Tag information (i.e. a TheInfo Class object).
  ///   Dim myTheInfo As TheInfo = sender.Tag
  ///   
  ///   ' Construct the informational string (using the StringBuilder Class).
  ///   Dim myDisplayString As New Text.StringBuilder
  ///   
  ///   ' Add information into the StringBuilder.
  ///   myDisplayString.Append("Selected information about the Layer:" + vbCrLf)
  ///   myDisplayString.Append("Copyright: " + myTheInfo.MyCopyrightText + vbCrLf)
  ///   myDisplayString.Append("Url: " + myTheInfo.MyUrl + vbCrLf)
  ///   myDisplayString.Append("IsVisible: " + myTheInfo.MyIsVisible + vbCrLf)
  ///   
  ///   ' The TheInfo.MyLayerItems Property is a bit more complex that a simple String. It actually contains
  ///   ' the information from the Layer.LayerItems Property. Use this object to display how many sub-Layers
  ///   ' are associated with this particular Layer.
  ///   Dim myLayerItems As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel)
  ///   myLayerItems = myTheInfo.MyLayerItems
  ///   myDisplayString.Append("Number of sub-Layers: " + myLayerItems.Count.ToString)
  ///   
  ///   ' Display the information to the user.
  ///   MessageBox.Show(myDisplayString.ToString)
  ///   
  /// End Sub
  /// </code>
  /// </example>
		public ESRI.ArcGIS.Client.Map Map
		{
			get { return GetValue(MapProperty) as Map; }
			set { SetValue(MapProperty, value); }
		}

		/// /// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty =
			DependencyProperty.Register("Map", typeof(Map), typeof(Legend), new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnMapPropertyChanged(e.OldValue as Map, e.NewValue as Map);
		}

		private void OnMapPropertyChanged(Map oldMap, Map newMap)
		{
			if (!_isLoaded)
				return; // defer initialization until all parameters are well known

			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)
				&& (newMap == null || newMap.Layers == null || newMap.Layers.Count == 0))
			{
				// Keep the basic hierarchy for design
			}
			else
				_legendTree.Map = newMap;
		}

		#endregion

		#region LayerIDs

		/// <summary>
		/// Gets or sets the layer IDs of the layers participating in the legend.
		/// </summary>
		/// <remarks>
		/// Specified in XAML and in Blend as a comma-delimited string: If a layer 
		/// name contains a comma, please use &#44; instead of the comma.
		/// If null/empty, legend from all layers is generated. Order of 
		/// the layer ids is respected in generating the legend.
		/// </remarks>
		/// <value>The layer IDs.</value>
		[System.ComponentModel.TypeConverter(typeof(StringToStringArrayConverter))]
		public string[] LayerIDs
		{
			get { return (string[])GetValue(LayerIDsProperty); }
			set { SetValue(LayerIDsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LayerIDs"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerIDsProperty =
				DependencyProperty.Register("LayerIDs", typeof(string[]), typeof(Legend), new PropertyMetadata(OnLayerIDsPropertyChanged));

		private static void OnLayerIDsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnLayerIDsPropertyChanged((string[])e.OldValue, (string[])e.NewValue);
		}

		private void OnLayerIDsPropertyChanged(string[] oldLayerIDs, string[] newLayerIDs)
		{
			_legendTree.LayerIDs = newLayerIDs;
		}

		#endregion

		#region LayerItemsMode

		#region Mode
		/// <summary>
		/// LayerItems mode enumeration defines the structure of the legend : Flat or Tree.
		/// </summary>
		public enum Mode
		{
			/// <summary>
			/// Flat structure : LayerItemsSource returns the LayerItems leaves (i.e not the group layers nor the map layers with sub layers) and the LegendItems. 
			/// <remarks>This is the default value.</remarks>
			/// </summary>
			Flat,
			/// <summary>
			/// Tree structure : LayerItemsSource returns a hierarchy of legend items taking care of the map layers and of the group layers. 
			/// </summary>
			Tree
		};
		#endregion


		/// <summary>
        /// Gets or sets the mode of display that defines the structure of the Legend Control: <b>Flat</b> or <b>Tree</b>.
		/// </summary>
        /// <remarks>
        /// <para>
        /// This Property controls whether the Legend displays information in an expanded <b>Tree</b> hierarchy for all 
        /// Legend items or in a <b>Flat</b> structure which shows only the lowest LayerItem leaves (The Layer.ID, 
        /// LayerItemViewModel.SubLayerID, and group-layer labels hierarchy information will not be displayed). The 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.Mode">Legend.Mode</see> Enumeration is used to define the
        /// <b>Tree</b> or <b>Flat</b> options. The default Enumeration value for the Legend.LayerItemsMode is <b>Flat</b>.
        /// </para>
        /// <para>
        /// The following is a visual depiction of the two different Legend.LayerItemsMode Enumerations for an 
        /// ArcGISDynamicMapServiceLayer:
        /// </para>
        /// <para>
        /// <img border="0" alt="Legend.LayerItemsMode comparison of 'Tree' and 'Flat' options." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.LayerItemsMode.png"/>
        /// </para>
        /// <para>
        /// Depending on the Legend.LayerItemsMode value, the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerItemsSource">Legend.LayerItemsSource</see> and 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel.LayerItemsSource">LayerItemViewModel.LayerItemsSource</see> 
        /// will return either only the Layer items leaves (i.e. <b>Flat</b>) or a complete Layer item hierarchy taking 
        /// care of the Map Layers and of the group layers (i.e. <b>Tree</b>).
        /// </para>
        /// <para>
        /// <b>Note:</b> The Legend.LayerItemsMode Property has great impact on what is displayed in the Legend. For users 
        /// who want to see the most information available in the Legend, developers should set the LayerItemsMode to 
        /// <b>Tree</b>. If customizations are made to the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.MapLayerTemplate">Legend.MapLayerTemplate</see> and they seem to be 
        /// overridden at runtime back to a default setting, it is most likely that the LayerItemsMode is set to the default of 
        /// <b>Flat</b> and it should be set to <b>Tree</b>.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the two buttons 'LayerItemsMode: Tree' and 'LayerItemsMode: Flat' to see how the Legend has different 
        /// appearances. The Legend_Refreshed Event is overridden to collapse the individual LegendItems (i.e. the map symbol 
        /// images) so that differences in the LayerItemsMode settings can more readily be seen.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating the different Legend.LayerItemsMode settings." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.LayerItemsMode1.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" &gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="350" Width="350" &gt;
        ///     
        ///     &lt;!-- Add an ArcGISDynamicMapServiceLayer. --&gt;
        ///     &lt;esri:ArcGISDynamicMapServiceLayer
        ///       Url="http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer"/&gt;
        ///       
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a Legend Control to the application. The Legend is bound to Map1. Set the ShowOnlyVisibleLayers=False 
        ///   to display all available sub-Layers and group-Layers from the ArcGISDynamicMapServiceLayer in the Legend 
        ///   even though some scale dependency thresholds have been set in the map service. 
        ///   --&gt;
        ///   &lt;esri:Legend Name="Legend1" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///                Margin="368,188,0,0" Width="350" Height="350" 
        ///                Map="{Binding ElementName=Map1}"
        ///                ShowOnlyVisibleLayers="False"/&gt;
        ///     
        ///   &lt;!--
        ///   Add two Buttons; one to set the Legend.LayerItemsMode = Tree and the other to set the 
        ///   Legend.LayerItemsMode = False. The Click Events for each button have code-behind functionality.
        ///   --&gt;
        ///   &lt;Button Content="LayerItemsMode: Tree" Height="23" HorizontalAlignment="Left" Margin="12,159,0,0" 
        ///           Name="Button_LayerItemsMode_Tree" VerticalAlignment="Top" Width="350" 
        ///           Click="Button_LayerItemsMode_Tree_Click"/&gt;
        ///   &lt;Button Content="LayerItemsMode: Flat" Height="23" HorizontalAlignment="Left" Margin="368,159,0,0" 
        ///           Name="Button2" VerticalAlignment="Top" Width="350" 
        ///           Click="Button_LayerItemsMode_Flat_Click"/&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="95" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
        ///          TextWrapping="Wrap" Margin="12,12,0,0" 
        ///          Text="Click the two buttons 'LayerItemsMode: Tree' and 'LayerItemsMode: Flat' to see how the 
        ///              Legend has different appearances. The Legend_Refreshed Event is overridden to collapse
        ///              the individual LegendItems (i.e. the map symbol images) so that differences in the 
        ///              LayerItemsMode settings can more readily be seen." /&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // NOTE: Don’t forget to insert the following event handler wireups at the end of the 'InitializeComponent' 
        /// // method for forms, 'Page_Init' for web pages, or into a constructor for other classes:
        /// Legend1.Refreshed += new ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventHandler(Legend1_Refreshed);
        ///   
        /// private void Button_LayerItemsMode_Tree_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Tree;
        /// }
        /// 
        /// private void Button_LayerItemsMode_Flat_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Flat;
        /// }
        /// 
        /// private void Legend1_Refreshed(object sender, ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs e)
        /// {
        ///   // The rendering of the Legend Control happens asynchronously. Use the Legend.Refreshed Event to demonstrate
        ///   // custom changes in what the Legend displays. This function overrides the default behavior of how LegendItems
        ///   // are displayed. Instead of showing each LegendItem (i.e. the map symbol images) individually, only the 
        ///   // Layer/sub-Layer/group-Layer leaves in the Legend are displayed to emphasize how the Legend.LayerItemsMode
        ///   // works.
        ///   
        ///   // Close all of the LegendItem leaves (i.e. the lowest level of leaves) 
        ///   IEnumerable&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel&gt; theIEnumerable = Legend1.LayerItemsSource;
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel theLegendItemViewModel in theIEnumerable)
        ///   {
        ///     theLegendItemViewModel.IsExpanded = false;
        ///   }
        ///   
        ///   // Expand the LayerItem leaves (i.e. the Layer/sub-Layer/group-Layer leaves)
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; theObservableCollection = Legend1.LayerItems;
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel theLayerItemViewModel in theObservableCollection)
        ///   {
        ///     theLayerItemViewModel.IsExpanded = true; //False
        ///   }
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_LayerItemsMode_Tree_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Tree
        /// End Sub
        /// 
        /// Private Sub Button_LayerItemsMode_Flat_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   Legend1.LayerItemsMode = ESRI.ArcGIS.Client.Toolkit.Legend.Mode.Flat
        /// End Sub
        /// 
        /// Private Sub Legend1_Refreshed(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs) Handles Legend1.Refreshed
        ///   
        ///   ' The rendering of the Legend Control happens asynchronously. Use the Legend.Refreshed Event to demonstrate
        ///   ' custom changes in what the Legend displays. This function overrides the default behavior of how LegendItems
        ///   ' are displayed. Instead of showing each LegendItem (i.e. the map symbol images) individually, only the 
        ///   ' Layer/sub-Layer/group-Layer leaves in the Legend are displayed to emphasize how the Legend.LayerItemsMode
        ///   ' works.
        ///   
        ///   ' Close all of the LegendItem leaves (i.e. the lowest level of leaves) 
        ///   Dim theIEnumerable As IEnumerable(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel) = Legend1.LayerItemsSource
        ///   For Each theLegendItemViewModel As ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel In theIEnumerable
        ///     theLegendItemViewModel.IsExpanded = False
        ///   Next
        /// 
        ///   ' Expand the LayerItem leaves (i.e. the Layer/sub-Layer/group-Layer leaves)
        ///   Dim theObservableCollection As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel) = Legend1.LayerItems
        ///   For Each theLayerItemViewModel As ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel In theObservableCollection
        ///     theLayerItemViewModel.IsExpanded = True 'False
        ///   Next
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public Mode LayerItemsMode
		{
			get { return (Mode)GetValue(LayerItemsModeProperty); }
			set { SetValue(LayerItemsModeProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LayerItemsModeProperty"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerItemsModeProperty =
			DependencyProperty.Register("LayerItemsMode", typeof(Mode), typeof(Legend), new PropertyMetadata(Mode.Flat, OnLayerItemsModePropertyChanged));


		private static void OnLayerItemsModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnLayerItemsModePropertyChanged((Mode)e.OldValue, (Mode)e.NewValue);
		}

		private void OnLayerItemsModePropertyChanged(Mode oldLayerItemsMode, Mode newLayerItemsMode)
		{
			_legendTree.LayerItemsMode = newLayerItemsMode;
		}
		#endregion

		#region ShowOnlyVisibleLayers

		/// <summary>
		/// Gets or sets a value indicating whether only the visible layers are participating to the legend.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if only the visible layers are participating to the legend; otherwise, <c>false</c>.
		/// </value>
		public bool ShowOnlyVisibleLayers
		{
			get { return (bool)GetValue(ShowOnlyVisibleLayersProperty); }
			set { SetValue(ShowOnlyVisibleLayersProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ShowOnlyVisibleLayers"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowOnlyVisibleLayersProperty =
				DependencyProperty.Register("ShowOnlyVisibleLayers", typeof(bool), typeof(Legend), new PropertyMetadata(true, OnShowOnlyVisibleLayersPropertyChanged));

		private static void OnShowOnlyVisibleLayersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnShowOnlyVisibleLayersPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		private void OnShowOnlyVisibleLayersPropertyChanged(bool oldValue, bool newValue)
		{
			_legendTree.ShowOnlyVisibleLayers = newValue;
		}

		#endregion

		#region LayerItems
		/// <summary>
		/// Gets the LayerItems for all layers that the legend control is working with.
		/// </summary>
		/// <value>The LayerItems.</value>
		public ObservableCollection<LayerItemViewModel> LayerItems
		{
			get { return (ObservableCollection<LayerItemViewModel>)GetValue(LayerItemsProperty); }
			internal set { SetValue(LayerItemsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LayerItems"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerItemsProperty =
			DependencyProperty.Register("LayerItems", typeof(ObservableCollection<LayerItemViewModel>), typeof(Legend), null);
		#endregion

		#region LayerItemsSource
		/// <summary>
		/// The enumeration of the legend items displayed at the first level of the legend control.
		/// This enumeration is depending on the <see cref="Legend.LayerItemsMode"/> property and on the <see cref="Legend.ShowOnlyVisibleLayers"/> property.
		/// </summary>
		/// <value>The layer items source.</value>
		public IEnumerable<LegendItemViewModel> LayerItemsSource
		{
			get { return (IEnumerable<LegendItemViewModel>)GetValue(LayerItemsSourceProperty); }
			internal set { SetValue(LayerItemsSourceProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LayerItemsSource"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerItemsSourceProperty =
			DependencyProperty.Register("LayerItemsSource", typeof(IEnumerable<LegendItemViewModel>), typeof(Legend), null);
		#endregion

		#region LegendItemTemplate
		/// <summary>
		/// Gets or sets the DataTemplate that controls how the Legend Control will display information about individual 
        /// LegendItems in a Layer. It is used at the lowest leaf level in the Legend hierachy and most generally contains 
        /// an image and a label. 
		/// </summary>
		/// <remarks>
        /// <para>
        /// This DataTemplate controls what is viewed in the Legend for the lowest level of information about a 
        /// particular Layer. It presents each LegendItem via its image 
        /// (<see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.ImageSource">LegendItemViewModel.ImageSource</see>) 
        /// and label description 
        /// (<see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see>). 
        /// No ToolTip information is provided for an individual LegendItem by default. 
        /// </para>
        /// <para>
        /// The objects that have Binding occur in the LegendItemTemplate are implied to be the Properties of the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel">LegendItemViewModel</see> Class.
        /// </para>
        /// <para>
        /// At the Legend.LegendItemTemplate level, the customization options become more limited due to the map service 
        /// information being passed back from the ArcGIS Server REST end point. The LegendItemViewModel class 
        /// contains the most atomic level of information about a particular LegendItem. Turning on/off individual 
        /// LegendItems or changing their opacity is not possible on any Layer type. It is possible to get creative 
        /// and perform TOC style user interactions at the Legend.LegendItemtemplate level by discovering the parent objects 
        /// of individual LegendItems; see the code example in the this document for one such customization.
        /// </para>
        /// <para>
        /// It should be noted that it is possible to customize one or more of the Legend Control DataTemplates at the 
        /// same time. There are established default values for each DataTemplate, so setting one will not necessarily 
        /// override the others that have been set by default. Significant testing should be done on all of the Layers 
        /// in the customized application to ensure that the desired behavior has been achieved. Some Layers have 
        /// different behavior in rendering items in the Legend and should be tested thoroughly.
        /// </para>
        /// <para>
        /// The following screen shot demonstrates which part of the Legend Control corresponds to the three 
        /// DataTemplates. The Layer (ID = "United States") that is being displayed is an ArcGISDynamicMapServiceLayer 
        /// with three sub-Layers (ushigh, states, and counties). The information found in the ArcGIS Services Directory 
        /// about the ArcGISDynamicMapServiceLayer corresponds to what is shown in the Map and Legend Controls.
        /// </para>
        /// <para>
        /// <img border="0" alt="Using the ArcGIS Services Directory to view information about an ArcGISDynamicMapServiceLayer and how that corresponds to what is displayed in the Map and Legend Control. The DataTemplate parts of the Legend Control are specified." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.png"/>
        /// </para>
        /// <para>
        /// <b>TIP:</b> It is typically necessary for developers to specify the 
        /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> name for Layers in the Map Control. This can be done
        /// in XAML or code-behind. If a Layer.ID value is not specified, then a default Layer.ID value is provided
        /// based upon the URL of the ArcGIS Server map service.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the 'Apply a custom LegendItemTemplate' button to change the default Legend into an interactive 
        /// Table of Contents (TOC). This example shows how to apply a custom DataTemplate for the 
        /// Legend.LegendItemTemplate. Move the cursor over an Image of any LegendItem and the features that correspond 
        /// to that LegendItem with be selected (in blue) in the Map. When the cursor is moved off of the Image in the 
        /// LegendItem the selection will clear.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Applying a custom LegendItemTemplate to the Legend Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.LegendItemTemplate.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///     
        ///   &lt;!-- 
        ///   NOTE: Don't forget to add the following xml namespace definition to the XAML file:
        ///   xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk"
        ///   --&gt;
        ///   
        ///   &lt;!-- Define a Resources section for use later in the XAML and code-behind --&gt;
        ///   &lt;Grid.Resources&gt;
        ///           
        ///     &lt;!--
        ///     Construct a DataTemplate that will be used to override the default behavior of the Legend.LegendItemTemplate.
        ///     Important things to note are:
        ///     ~ When the user moves the cursor over the Image of a LegendItem, the MouseEnter event will fire and custom coding 
        ///     will occur in the code-behind that performs a Query to display selected graphic that correspond to the LegendItem
        ///     in the Map.
        ///     ~ When the user moves the cursor off the Image of the LegendItem, the GraphicsLayer will be cleared.
        ///     ~ The Image.Tag Property is bound to the LegendItemViewModel.Tag Property. The LegendItemViewModel.Tag gets 
        ///     populated as part of the Legend.Refreshed event. The purpose of populating the LegendItemViewModel.Tag is to 
        ///     pass the parent object (i.e. the Layer/sub-Layer) associated with a specific LegendItem so that a Query can 
        ///     occur correctly.
        ///     ~ The objects that have Binding occur in the DataTemplate are implied to be the Properties of the 
        ///     ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel Class.
        ///     --&gt;
        ///     &lt;DataTemplate x:Key="My_LegendItemTemplate"&gt;
        ///       &lt;StackPanel Orientation="Horizontal"&gt;
        ///         &lt;Image Source="{Binding ImageSource}" MouseEnter="Image_MouseEnter" MouseLeave="Image_MouseLeave" Tag="{Binding Tag}"/&gt;
        ///         &lt;sdk:Label Content="{Binding Label}"  FontSize="14" FontWeight="Bold"/&gt;
        ///       &lt;/StackPanel&gt;
        ///     &lt;/DataTemplate&gt;
        ///     
        ///     &lt;!-- Construct some default SimpleFillSymbol and SimpleLineSymbol objects for use in the GraphicsLayer. --&gt;
        ///     &lt;esri:SimpleFillSymbol x:Key="DefaultFillSymbol" Fill="#500000FF" BorderBrush="Blue" BorderThickness="1" /&gt;
        ///     &lt;esri:SimpleLineSymbol x:Key="DefaultLineSymbol"  Color="Blue" /&gt;
        ///     
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="400" Width="400" &gt;
        ///   
        ///     &lt;!-- Add an ArcGISDynamicMapServiceLayer. It will have three sub-Layers: counties, states, and ushigh. --&gt;
        ///     &lt;esri:ArcGISDynamicMapServiceLayer  Opacity="0.6" ID="MyArcGISDynamicMapServiceLayer"
        ///            Url="http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer"/&gt;
        ///     
        ///     &lt;!-- Add an empty GraphicsLayer used for rendering selected features that correspond to LegendItems. --&gt;
        ///     &lt;esri:GraphicsLayer ID="MyGraphicsLayer" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Legend. It is bound to the Map Control. The LegendItemTemplate will get overridden in the code-behind 
        ///   using the DataTemplate defined in the Resources section of the XAML.
        ///   --&gt;
        ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="418,188,0,0" Name="Legend1" 
        ///                VerticalAlignment="Top" Width="350" Height="400" Background="#FF2180D4"
        ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree" ShowOnlyVisibleLayers="False"/&gt;
        ///   
        ///   &lt;!-- Add a button to apply the custom DataTemplate and thus allowing the other customized functionality. --&gt;
        ///   &lt;Button Content="Apply a custom LegendItemTemplate" Name="Button1"
        ///           Height="23" HorizontalAlignment="Left" Margin="12,159,0,0" 
        ///           VerticalAlignment="Top" Width="756" 
        ///           Click="Button1_Click"/&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="95" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
        ///          TextWrapping="Wrap" Margin="12,12,0,0" 
        ///          Text="Click the 'Apply a custom LegendItemTemplate' button to change the default Legend into an interactive
        ///              Table of Contents (TOC). This example shows how to apply a custom DataTemplate for the 
        ///              Legend.LegendItemTemplate. Move the cursor over an Image of any LegendItem and the features that
        ///              correspond to that LegendItem with be selected (in blue) in the Map. When the cursor is moved off
        ///              of the Image in the LegendItem the selection will clear." /&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // NOTE: Don't forget to insert the following event handler wireups at the end of the 'InitializeComponent' 
        /// // method for forms, 'Page_Init' for web pages, or into a constructor for other classes:
        /// Legend1.Refreshed += new ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventHandler(Legend1_Refreshed);
        /// 
        /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Apply the custom DataTemplate that was defined in XAML to the Legend's DataTemplate.
        ///   DataTemplate myDataTemplate = LayoutRoot.Resources["My_LegendItemTemplate"];
        ///   Legend1.LegendItemTemplate = myDataTemplate;
        ///   
        ///   // Only show the ArcGISDynamicMapServiceLayer in the Legend Control. Do not display the GraphicsLayer.
        ///   Legend1.LayerIDs = new string[] {"MyArcGISDynamicMapServiceLayer"};
        /// }
        /// 
        /// private void Legend1_Refreshed(object sender, ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs e)
        /// {
        ///   // The purpose of the code in this function is to associate (1) the Layer/sub-Layer name and (2) the Label associated 
        ///   // the image of a LegendItem to the LegendItemViewModel.Tag Property. This will enable the correct Query selection
        ///   // to occur when the user moves the cursor over the LegendItem image in the Legend Control.
        ///   
        ///   // Get the Legend.
        ///   ESRI.ArcGIS.Client.Toolkit.Legend theLegend = (ESRI.ArcGIS.Client.Toolkit.Legend)sender;
        ///   
        ///   // Get the LayerItems of the Legend Control.
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; theObservableCollection = theLegend.LayerItems;
        ///   
        ///   // Loop through the ObservableCollection&lt;LayerItemViewModel&gt; objects for each Layer. There are two Layers in this 
        ///   // sample (an ArcGISDynamicMapServiceLayer and an empty GraphicsLayer).
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel theLayerItemViewModel in theObservableCollection)
        ///   {
        ///     // Get the LayerItems for each Layer.
        ///     Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; theObservableCollection2 = theLayerItemViewModel.LayerItems;
        ///     
        ///     // Make sure that we have a valid ObservableCollection&lt;LayerItemViewModel&gt; object before continuing. The GraphicsLayer 
        ///     // does not have any LayerItems so it will return a Nothing/null. 
        ///     if (theObservableCollection2 != null)
        ///     {
        ///       // Loop through the ObservableCollection&lt;LayerItemViewModel&gt; objects for each sub-Layer. For the 
        ///       // ArcGISDynamicMapServiceLayer there will be three sub-Layers (i.e. counties, states, and ushigh).
        ///       foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel theLayerItemViewModel2 in theObservableCollection2)
        ///       {
        ///         // Get the LegendItems for each sub-Layer.
        ///         Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel&gt; theObservableCollection3 = theLayerItemViewModel2.LegendItems;
        ///         
        ///         // Loop through the ObservableCollection&lt;LegendItemViewModel&gt; objects for each LegendItem of the sub-Layer.
        ///         // The states and ushigh sub-layers only have one LegendItem (i.e. a symbol with a label). The counties sub-Layer
        ///         // however has numerous LegendItems.
        ///         foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel theLegendItemViewModel in theObservableCollection3)
        ///         {
        ///           // Create a List object to hold:
        ///           // (1) the name of the sub-Layer 
        ///           // (2) the string label associated with the image of the LegendItem
        ///           //
        ///           // The reason for obtaining this information is there is NOT a 'Parent' object property of the LegendItemViewModel 
        ///           // which makes it a little tricky to figure out what Layer/sub-Layer a particular LegendItem belongs to. Hence 
        ///           // it requires some more complex coding to determine this information and we will store it in the .Tag property
        ///           // of each individual LegendItem for use later in the application.
        ///           List&lt;string&gt; theList = new List&lt;string&gt;();
        ///           theList.Add(theLayerItemViewModel2.Label); // The Layer/sub-Layer name (i.e. "counties", "states", or "ushigh")
        ///           theList.Add(theLegendItemViewModel.Label); // The Label of the LegendItem.
        ///           theLegendItemViewModel.Tag = theList;
        ///         }
        ///       }
        ///     }
        ///   }
        /// }
        /// 
        /// private void Image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        /// {
        ///   // This function will perform a 'selection' type of operation each time the user moves the cursor over the image
        ///   // associated with a LegendItem in the Legend Control. The correct type of Query syntax will be obtained from the
        ///   // Image.Tag Property which contains a List with two strings. The first string gives the name of the Layer/sub-Layer
        ///   // and the second string is the Label of the particular LegendItem associated with the Image.
        ///   
        ///   // Create and initial QueryTask object. It will be fully instantiated later in the code.
        ///   ESRI.ArcGIS.Client.Tasks.QueryTask theQueryTask = null;
        ///   
        ///   // Create a new Query and set its initial Properties.
        ///   ESRI.ArcGIS.Client.Tasks.Query theQuery = new ESRI.ArcGIS.Client.Tasks.Query();
        ///   theQuery.OutSpatialReference = Map1.SpatialReference;
        ///   theQuery.OutFields.Add("*");
        ///   theQuery.ReturnGeometry = true;
        ///   
        ///   // Get the information necessary to construct the Query syntax from the Image.Tag Property.
        ///   List&lt;string&gt; theList = sender.Tag;
        ///   string theLayerName = theList[0];
        ///   string theLegendItem = theList[1];
        ///   
        ///   // Determine which sub-Layer of the ArcGISDynamicMapServiceLayer we will be constructing the Query for and
        ///   // use the appropriate syntax to construct the QueryTask. NOTE: this example is very much hard-coded for
        ///   // specific sample data; you will need to adjust the code according to your sample data.
        ///   if (theLayerName == "counties")
        ///   {
        ///     theQuery.Where = "STATE_NAME = '" + theLegendItem + "'";
        ///     theQueryTask = new ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/2");
        ///   }
        ///   else if (theLayerName == "states")
        ///   {
        ///     theQuery.Where = "1 = 1";
        ///     theQueryTask = new ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/1");
        ///   }
        ///   else if (theLayerName == "ushigh")
        ///   {
        ///     theQuery.Where = "1 = 1";
        ///     theQueryTask = new ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/0");
        ///   }
        ///   
        ///   // Add handlers for the Asynchronous processing.
        ///   theQueryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
        ///   theQueryTask.Failed += QueryTask_Failed;
        ///   
        ///   // Execute the QueryTask.
        ///   theQueryTask.ExecuteAsync(theQuery);
        /// }
        /// 
        /// private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        /// {
        ///   // This Asynchronous function will occur when the QueryTask has completed executing on the web server. It's 
        ///   // purpose is to add any features returned from the QueryTask and put them in the GraphicsLayer symbolized
        ///   // properly.
        ///   // Remember that depending on the ArcGIS Server settings only the first 500 or 1000 features will be returned. 
        ///   // In ArcGIS Server 9.3.1 and prior, the default maximum is 500 records returned per Query. In ArcGIS Server 10 
        ///   // the default is 1000. This setting is configurable per map service using ArcCatalog or ArcGIS Server Manager 
        ///   // (on the Parameters tab). 
        ///   
        ///   // Get the FeatureSet from the QueryTask operation.
        ///   ESRI.ArcGIS.Client.Tasks.FeatureSet theFeatureSet = args.FeatureSet;
        ///   
        ///   // Obtain the GraphicsLayer that was defined in XAML and clear out and Graphics that may have previously been there.
        ///   ESRI.ArcGIS.Client.GraphicsLayer theGraphicsLayer = Map1.Layers["MyGraphicsLayer"] as ESRI.ArcGIS.Client.GraphicsLayer;
        ///   theGraphicsLayer.ClearGraphics();
        ///   
        ///   // If we have 1 or more features returned from the QueryTask proceed.
        ///   if (theFeatureSet != null &amp;&amp; theFeatureSet.Features.Count > 0)
        ///   {
        ///     // Loop through each feature (i.e. a Graphic) in the FeatureSet
        ///     foreach (ESRI.ArcGIS.Client.Graphic theGraphic in theFeatureSet)
        ///     {
        ///       // Depending the GeometryType of the Graphic set the appropriate Symbol from what was
        ///       // defined in the LayoutRoot.Resources of XAML.
        ///       if (theFeatureSet.GeometryType == ESRI.ArcGIS.Client.Tasks.GeometryType.Polygon)
        ///       {
        ///         theGraphic.Symbol = LayoutRoot.Resources["DefaultFillSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
        ///       }
        ///       else if (theFeatureSet.GeometryType == ESRI.ArcGIS.Client.Tasks.GeometryType.Polyline)
        ///       {
        ///         theGraphic.Symbol = LayoutRoot.Resources["DefaultLineSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
        ///       }
        ///       
        ///       // Add the Graphic from the FeatureSet to the GraphicsLayer.
        ///       theGraphicsLayer.Graphics.Add(theGraphic);
        ///     }
        ///   }
        /// }
        /// 
        /// private void QueryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs args)
        /// {
        ///   // Display some error help if there is something wrong.
        ///   MessageBox.Show("Query failed: " + args.Error.ToString());
        /// }
        /// 
        /// private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        /// {
        ///   // If the user moves the cursor off of the Image in the LegendItem, clear the selected features in the GraphicsLayer.
        ///   ESRI.ArcGIS.Client.GraphicsLayer graphicsLayer = Map1.Layers["MyGraphicsLayer"] as ESRI.ArcGIS.Client.GraphicsLayer;
        ///   graphicsLayer.ClearGraphics();
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Apply the custom DataTemplate that was defined in XAML to the Legend's DataTemplate.
        ///   Dim myDataTemplate As DataTemplate = LayoutRoot.Resources("My_LegendItemTemplate")
        ///   Legend1.LegendItemTemplate = myDataTemplate
        ///   
        ///   ' Only show the ArcGISDynamicMapServiceLayer in the Legend Control. Do not display the GraphicsLayer.
        ///   Legend1.LayerIDs = New String() {"MyArcGISDynamicMapServiceLayer"}
        ///   
        /// End Sub
        ///   
        /// Private Sub Legend1_Refreshed(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs) Handles Legend1.Refreshed
        ///   
        ///   ' The purpose of the code in this function is to associate (1) the Layer/sub-Layer name and (2) the Label associated 
        ///   ' the image of a LegendItem to the LegendItemViewModel.Tag Property. This will enable the correct Query selection
        ///   ' to occur when the user moves the cursor over the LegendItem image in the Legend Control.
        ///   
        ///   ' Get the Legend.
        ///   Dim theLegend As ESRI.ArcGIS.Client.Toolkit.Legend = sender
        ///   
        ///   ' Get the LayerItems of the Legend Control.
        ///   Dim theObservableCollection As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel) = theLegend.LayerItems
        ///   
        ///   ' Loop through the ObservableCollection(Of LayerItemViewModel) objects for each Layer. There are two Layers in this 
        ///   ' sample (an ArcGISDynamicMapServiceLayer and an empty GraphicsLayer).
        ///   For Each theLayerItemViewModel As ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel In theObservableCollection
        ///     
        ///     ' Get the LayerItems for each Layer.
        ///     Dim theObservableCollection2 As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel) = theLayerItemViewModel.LayerItems
        ///         
        ///     ' Make sure that we have a valid ObservableCollection(Of LayerItemViewModel) object before continuing. The GraphicsLayer 
        ///     ' does not have any LayerItems so it will return a Nothing/null. 
        ///     If theObservableCollection2 IsNot Nothing Then
        ///           
        ///       ' Loop through the ObservableCollection(Of LayerItemViewModel) objects for each sub-Layer. For the 
        ///       ' ArcGISDynamicMapServiceLayer there will be three sub-Layers (i.e. counties, states, and ushigh).
        ///       For Each theLayerItemViewModel2 As ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel In theObservableCollection2
        ///         
        ///         ' Get the LegendItems for each sub-Layer.
        ///         Dim theObservableCollection3 As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel) = theLayerItemViewModel2.LegendItems
        ///         
        ///         ' Loop through the ObservableCollection(Of LegendItemViewModel) objects for each LegendItem of the sub-Layer.
        ///         ' The states and ushigh sub-layers only have one LegendItem (i.e. a symbol with a label). The counties sub-Layer
        ///         ' however has numerous LegendItems.
        ///         For Each theLegendItemViewModel As ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel In theObservableCollection3
        ///           
        ///           ' Create a List object to hold:
        ///           ' (1) the name of the sub-Layer 
        ///           ' (2) the string label associated with the image of the LegendItem
        ///           '
        ///           ' The reason for obtaining this information is there is NOT a 'Parent' object property of the LegendItemViewModel 
        ///           ' which makes it a little tricky to figure out what Layer/sub-Layer a particular LegendItem belongs to. Hence 
        ///           ' it requires some more complex coding to determine this information and we will store it in the .Tag property
        ///           ' of each individual LegendItem for use later in the application.
        ///           Dim theList As New List(Of String)
        ///           theList.Add(theLayerItemViewModel2.Label) ' The Layer/sub-Layer name (i.e. "counties", "states", or "ushigh")
        ///           theList.Add(theLegendItemViewModel.Label) ' The Label of the LegendItem.
        ///           theLegendItemViewModel.Tag = theList
        ///         Next
        ///       Next
        ///     End If
        ///   Next
        ///   
        /// End Sub
        /// 
        /// Private Sub Image_MouseEnter(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs)
        ///   
        ///   ' This function will perform a 'selection' type of operation each time the user moves the cursor over the image
        ///   ' associated with a LegendItem in the Legend Control. The correct type of Query syntax will be obtained from the
        ///   ' Image.Tag Property which contains a List with two strings. The first string gives the name of the Layer/sub-Layer
        ///   ' and the second string is the Label of the particular LegendItem associated with the Image.
        ///   
        ///   ' Create and initial QueryTask object. It will be fully instantiated later in the code.
        ///   Dim theQueryTask As ESRI.ArcGIS.Client.Tasks.QueryTask = Nothing
        ///   
        ///   ' Create a new Query and set its initial Properties.
        ///   Dim theQuery As New ESRI.ArcGIS.Client.Tasks.Query()
        ///   theQuery.OutSpatialReference = Map1.SpatialReference
        ///   theQuery.OutFields.Add("*")
        ///   theQuery.ReturnGeometry = True
        ///   
        ///   ' Get the information necessary to construct the Query syntax from the Image.Tag Property.
        ///   Dim theList As List(Of String) = sender.Tag
        ///   Dim theLayerName As String = theList.Item(0)
        ///   Dim theLegendItem As String = theList.Item(1)
        ///   
        ///   ' Determine which sub-Layer of the ArcGISDynamicMapServiceLayer we will be constructing the Query for and
        ///   ' use the appropriate syntax to construct the QueryTask. NOTE: this example is very much hard-coded for
        ///   ' specific sample data; you will need to adjust the code according to your sample data.
        ///   If theLayerName = "counties" Then
        ///     theQuery.Where = "STATE_NAME = '" + theLegendItem + "'"
        ///     theQueryTask = New ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/2")
        ///   ElseIf theLayerName = "states" Then
        ///     theQuery.Where = "1 = 1"
        ///     theQueryTask = New ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/1")
        ///   ElseIf theLayerName = "ushigh" Then
        ///     theQuery.Where = "1 = 1"
        ///     theQueryTask = New ESRI.ArcGIS.Client.Tasks.QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/0")
        ///   End If
        ///   
        ///   ' Add handlers for the Asynchronous processing.
        ///   AddHandler theQueryTask.ExecuteCompleted, AddressOf QueryTask_ExecuteCompleted
        ///   AddHandler theQueryTask.Failed, AddressOf QueryTask_Failed
        /// 
        ///   ' Execute the QueryTask.
        ///   theQueryTask.ExecuteAsync(theQuery)
        ///   
        /// End Sub
        /// 
        /// Private Sub QueryTask_ExecuteCompleted(ByVal sender As Object, ByVal args As ESRI.ArcGIS.Client.Tasks.QueryEventArgs)
        ///   
        ///   ' This Asynchronous function will occur when the QueryTask has completed executing on the web server. It's 
        ///   ' purpose is to add any features returned from the QueryTask and put them in the GraphicsLayer symbolized
        ///   ' properly.
        ///   ' Remember that depending on the ArcGIS Server settings only the first 500 or 1000 features will be returned. 
        ///   ' In ArcGIS Server 9.3.1 and prior, the default maximum is 500 records returned per Query. In ArcGIS Server 10 
        ///   ' the default is 1000. This setting is configurable per map service using ArcCatalog or ArcGIS Server Manager 
        ///   ' (on the Parameters tab). 
        ///   
        ///   ' Get the FeatureSet from the QueryTask operation.
        ///   Dim theFeatureSet As ESRI.ArcGIS.Client.Tasks.FeatureSet = args.FeatureSet
        ///   
        ///   ' Obtain the GraphicsLayer that was defined in XAML and clear out and Graphics that may have previously been there.
        ///   Dim theGraphicsLayer As ESRI.ArcGIS.Client.GraphicsLayer = TryCast(Map1.Layers("MyGraphicsLayer"), ESRI.ArcGIS.Client.GraphicsLayer)
        ///   theGraphicsLayer.ClearGraphics()
        ///   
        ///   ' If we have 1 or more features returned from the QueryTask proceed.
        ///   If theFeatureSet IsNot Nothing AndAlso theFeatureSet.Features.Count > 0 Then
        ///     
        ///     ' Loop through each feature (i.e. a Graphic) in the FeatureSet
        ///     For Each theGraphic As ESRI.ArcGIS.Client.Graphic In theFeatureSet
        ///       
        ///       ' Depending the GeometryType of the Graphic set the appropriate Symbol from what was
        ///       ' defined in the LayoutRoot.Resources of XAML.
        ///       If theFeatureSet.GeometryType = ESRI.ArcGIS.Client.Tasks.GeometryType.Polygon Then
        ///         theGraphic.Symbol = TryCast(LayoutRoot.Resources("DefaultFillSymbol"), ESRI.ArcGIS.Client.Symbols.Symbol)
        ///       ElseIf theFeatureSet.GeometryType = ESRI.ArcGIS.Client.Tasks.GeometryType.Polyline Then
        ///         theGraphic.Symbol = TryCast(LayoutRoot.Resources("DefaultLineSymbol"), ESRI.ArcGIS.Client.Symbols.Symbol)
        ///       End If
        ///       
        ///       ' Add the Graphic from the FeatureSet to the GraphicsLayer.
        ///       theGraphicsLayer.Graphics.Add(theGraphic)
        ///       
        ///     Next
        ///     
        ///   End If
        ///   
        /// End Sub
        /// 
        /// Private Sub QueryTask_Failed(ByVal sender As Object, ByVal args As ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs)
        ///   
        ///   ' Display some error help if there is something wrong.
        ///   MessageBox.Show("Query failed: " + args.Error.ToString())
        ///   
        /// End Sub
        ///   
        /// Private Sub Image_MouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs)
        ///   
        ///   ' If the user moves the cursor off of the Image in the LegendItem, clear the selected features in the GraphicsLayer.
        ///   Dim graphicsLayer As ESRI.ArcGIS.Client.GraphicsLayer = TryCast(Map1.Layers("MyGraphicsLayer"), ESRI.ArcGIS.Client.GraphicsLayer)
        ///   graphicsLayer.ClearGraphics()
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public DataTemplate LegendItemTemplate
		{
			get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
			set { SetValue(LegendItemTemplateProperty, value); }
		}

		/// /// <summary>
		/// Identifies the <see cref="LegendItemTemplate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LegendItemTemplateProperty =
			DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(Legend), new PropertyMetadata(OnLegendItemTemplateChanged));

		private static void OnLegendItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnLegendItemTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
		}

		private void OnLegendItemTemplateChanged(DataTemplate oldDataTemplate, DataTemplate newDataTemplate)
		{
			_legendTree.LegendItemTemplate = newDataTemplate;
		}

		#endregion

		#region LayerTemplate
		/// <summary>
        /// Gets or sets the DataTemplate that controls how the Legend Control will display information
        /// about a Layer. This DataTemplate is used by default for all Layer items (i.e all items except 
        /// the LegendItems at the lowest leaf level).
		/// </summary>
        /// <remarks>
        /// <para>
        /// This DataTemplate controls what is viewed in the Legend for the middle level of information about a 
        /// particular Layer. It presents each sub-Layer name (via the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see> 
        /// Property) in a ContentControl with a node to expand any LegendItem information or any other nested 
        /// sub-Layer (aka. group layer) information. When a user hovers the cursor over the Legend.LayerTemplate 
        /// section, a ToolTip appears displaying the additional information about the sub-layer including: Layer.ID, 
        /// LegendItemViewModel.Label, LegendItemViewModel.Description, LayerItemViewModel.SubLayerID, 
        /// LayerItemViewModel.MinimumResolution, and LayerItemViewModel.MaximumResolution. This template is used 
        /// by default for all Layers except the LegendItems as the lowest level. 
        /// </para>
        /// <para>
        /// The objects that have Binding occur in the LayerTemplate are implied to be the Properties of the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel">LayerItemViewModel</see> Class.
        /// </para>
        /// <para>
        /// At the Legend.LayerTemplate level, one common customization technique would be to add TOC style of interaction. 
        /// Developers could add a CheckBox to manage the visibility of sub-Layer elements. You could add a Slider 
        /// similar to the Legend.MapLayerTemplate and you will get Sliders appearing for each sub-Layer but the behavior 
        /// may not be as you would expect. Adjusting a Slider on one sub-Layer would also control the Opacity of 
        /// the other sub-Layers simultaneously. <b>Note:</b> FeatureLayers do not have the ability to control the 
        /// visibility or opacity of sub-Layer elements.
        /// </para>
        /// <para>
        /// It should be noted that it is possible to customize one or more of the Legend Control DataTemplates at the 
        /// same time. There are established default values for each DataTemplate, so setting one will not necessarily 
        /// override the others that have been set by default. Significant testing should be done on all of the Layers 
        /// in the customized application to ensure that the desired behavior has been achieved. Some Layers have 
        /// different behavior in rendering items in the Legend and should be tested thoroughly.
        /// </para>
        /// <para>
        /// The following screen shot demonstrates which part of the Legend Control corresponds to the three 
        /// DataTemplates. The Layer (ID = "United States") that is being displayed is an ArcGISDynamicMapServiceLayer 
        /// with three sub-Layers (ushigh, states, and counties). The information found in the ArcGIS Services Directory 
        /// about the ArcGISDynamicMapServiceLayer corresponds to what is shown in the Map and Legend Controls.
        /// </para>
        /// <para>
        /// <img border="0" alt="Using the ArcGIS Services Directory to view information about an ArcGISDynamicMapServiceLayer and how that corresponds to what is displayed in the Map and Legend Control. The DataTemplate parts of the Legend Control are specified." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.png"/>
        /// </para>
        /// <para>
        /// <b>TIP:</b> It is typically necessary for developers to specify the 
        /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> name for Layers in the Map Control. This can be done
        /// in XAML or code-behind. If a Layer.ID value is not specified, then a default Layer.ID value is provided
        /// based upon the URL of the ArcGIS Server map service.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click each of the three buttons to change the DataTemplates of the Legend Control. Experiment with 
        /// interacting with the Legend after clicking each button to see the behavior changes. Each time a 
        /// different DataTemplate (i.e. MapLayerTemplate, LayerTemplate, and LegendItemTemplate) is applied, the 
        /// other DataTemplates will revert back to their original state.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Changing the DataTemplates of a Legend Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.LayerTemplate.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- 
        ///   NOTE: Don't forget to add the following xml namespace definition to the XAML file:
        ///   xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk"
        ///   --&gt;
        ///   
        ///   &lt;!-- Define a Resources section for use later in the XAML and code-behind --&gt;
        ///   &lt;Grid.Resources&gt;
        ///   
        ///     &lt;!--
        ///     Construct the various DataTemplates that will be used to override the default behavior of the Legend.
        ///     Users click one of the three buttons to apply the various DataTemplates defined here using code-behind.
        ///     --&gt;
        ///     
        ///     &lt;!--
        ///     Define the MapLayerTemplate. The DataTemplate adds the ability to turn on/off the entire Layer (including
        ///     any sub-Layers) via a CheckBox. Additionally, a Slider is available that can control the Opacity (aka. 
        ///     the Visibility) of the Layer (including any sub-Layers). The objects that have Binding occur in this 
        ///     DataTemplate are implied to be the Properties of the ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel 
        ///     Class.
        ///     --&gt;
        ///     &lt;DataTemplate x:Key="My_MapLayerTemplate"&gt;
        ///       &lt;StackPanel Orientation="Horizontal"&gt;
        ///         &lt;CheckBox Content="{Binding Label}" IsChecked="{Binding IsEnabled, Mode=TwoWay}"/&gt;
        ///         &lt;Slider Maximum="1" Value="{Binding Layer.Opacity, Mode=TwoWay}" Width="50" /&gt;
        ///       &lt;/StackPanel&gt;
        ///     &lt;/DataTemplate&gt;
        ///     
        ///     &lt;!--
        ///     Define the LayerTemplate. The DataTemplate adds the ability to turn on/off the a sub-Layers via a 
        ///     CheckBox. The objects that have Binding occur in this DataTemplate are implied to be the Properties of 
        ///     the ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel Class.
        ///     NOTE: You could add a Slider similarly to the MapLayerTemplate and you will get Sliders appearing for
        ///     each sub-Layer but the behavior may not be as you would expect. Adjusting a Slider on one sub-Layer
        ///     would also control the Opacity of the other sub-Layers simultaneously.
        ///     --&gt;
        ///     &lt;DataTemplate x:Key="My_LayerTemplate"&gt;
        ///       &lt;StackPanel Orientation="Horizontal"&gt;
        ///         &lt;CheckBox Content="{Binding Label}" IsChecked="{Binding IsEnabled, Mode=TwoWay}"/&gt;
        ///         &lt;!--&lt;Slider Maximum="1" Value="{Binding Layer.Opacity, Mode=TwoWay}" Width="50" /&gt;--&gt;
        ///       &lt;/StackPanel&gt;
        ///     &lt;/DataTemplate&gt;
        ///     
        ///     &lt;!--
        ///     Define the LegendItemTemplate. The DataTemplate adds the ability to display information about 
        ///     individual LegendItems in the Legend Control. An image for each LegendItem is provided. Additionally,
        ///     a Label displays the text description for the image that corresponds to the Symbology in the Map.
        ///     The objects that have Binding occur in this DataTemplate are implied to be the Properties of 
        ///     the ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemItemViewModel Class.
        ///     NOTE: Adding controls like the CheckBox or Slider to manage the visibility of individual LegendItems
        ///     for a Layer/sub-Layer in the Map will not work. The REST web services do not provide the ability to 
        ///     control this level of granularity over how the Layers are displayed. Look closely to the Properties of
        ///     the LegendItemViewModel to see what type of TOC interactions could be created.
        ///     --&gt;
        ///     &lt;DataTemplate x:Key="My_LegendItemTemplate"&gt;
        ///       &lt;StackPanel Orientation="Horizontal"&gt;
        ///         &lt;Image Source="{Binding ImageSource}" /&gt;
        ///         &lt;sdk:Label Content="{Binding Label}" FontFamily="Comic Sans MS" FontSize="14" FontWeight="Bold"/&gt;
        ///       &lt;/StackPanel&gt;
        ///     &lt;/DataTemplate&gt;
        ///     
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="Black" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="400" Width="400" &gt;
        ///     
        ///     &lt;!-- Add an ArcGISDynamicMapServiceLayer. --&gt;
        ///     &lt;esri:ArcGISDynamicMapServiceLayer  Opacity="0.6" 
        ///         Url="http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Louisville/LOJIC_LandRecords_Louisville/MapServer"/&gt;
        ///   
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Legend. It is bound to the Map Control. The various DataTemplates will get overridden in the 
        ///   code-behind ones defined in the Resources section of the XAML.
        ///   --&gt;
        ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="418,188,0,0" Name="Legend1" 
        ///                VerticalAlignment="Top" Width="350" Height="400" Background="#FF2180D4"
        ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree"
        ///                ShowOnlyVisibleLayers="False"/&gt;
        ///   
        ///   &lt;!-- 
        ///   Add buttons to control the various DataTempaltes of the Legend Control (i.e. MapLayerTemplate, 
        ///   LayerTemplate, and LegendItemTemplate. Each Button has its Click event defined.
        ///   --&gt;
        ///   &lt;Button Content="MapLayerTemplate" Height="23" HorizontalAlignment="Left" Margin="12,159,0,0" 
        ///           Name="Button_MapLayerTempalte" VerticalAlignment="Top" Width="240" 
        ///           Click="Button_MapLayerTempalte_Click"/&gt;
        ///   &lt;Button Content="LayerTemplate" Height="23" HorizontalAlignment="Left" Margin="270,159,0,0" 
        ///           Name="Button_LayerTemplate" VerticalAlignment="Top" Width="240" 
        ///           Click="Button_LayerTemplate_Click"/&gt;
        ///   &lt;Button Content="LegendItemTemplate" Height="23" HorizontalAlignment="Left" Margin="528,159,0,0" 
        ///           Name="Button_LegendItemTemplate" VerticalAlignment="Top" Width="240" 
        ///           Click="Button_LegendItemTemplate_Click"/&gt;
        ///       
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="95" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
        ///          TextWrapping="Wrap" Margin="12,12,0,0" 
        ///          Text="Click each of the three buttons to change the DataTemplates of the Legend Control. 
        ///                Experiment with interacting with the Legend after clicking each button to see the behavior
        ///                changes. Each time a different DataTemplate (i.e. MapLayerTemplate, LayerTemplate, and
        ///                LegendItemTemplate is applied the other DataTemplates will revert back to their original
        ///                state." /&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // There is 'by default' some form of DataTemplate for each of the Legend Templates (MapLayerTemplate, 
        /// // LayerTemplate, and LegendItemTemplate). By using a global (i.e. Member) variable to store what 
        /// // those 'default' templates are, we can reset them to their initial setting when using our individual 
        /// // custom DataTemplates.
        /// 
        /// public DataTemplate _MapLayerTemplate;
        /// public DataTemplate _LayerTemplate;
        /// public DataTemplate _LegendItemTemplate;
        /// 
        /// // NOTE: Change the name of the class in this code-example ("Example_LayerTemplate") to the name of your
        /// // class when testing the code. 
        /// public &lt;class Example_LayerTemplate&gt;()
        /// {
        ///   InitializeComponent();
        ///   
        ///   // Populate the Member variables for the various DataTemplates of the Legend Control.
        ///   ESRI.ArcGIS.Client.Toolkit.Legend theLegend = Legend1;
        ///   _MapLayerTemplate = theLegend.MapLayerTemplate;
        ///   _LayerTemplate = theLegend.LayerTemplate;
        ///   _LegendItemTemplate = theLegend.LegendItemTemplate;
        /// }
        /// 
        /// private void Button_MapLayerTempalte_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Apply the custom DataTemplate for the MapLayerTemplate. Reset the other DataTemplates
        ///   // of the Legend Control back to their initial setting.
        ///   DataTemplate myDataTemplate = LayoutRoot.Resources["My_MapLayerTemplate"];
        ///   Legend1.MapLayerTemplate = myDataTemplate;
        ///   Legend1.LayerTemplate = _LayerTemplate;
        ///   Legend1.LegendItemTemplate = _LegendItemTemplate;
        /// }
        /// 
        /// private void Button_LayerTemplate_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Apply the custom DataTemplate for the LayerTemplate. Reset the other DataTemplates
        ///   // of the Legend Control back to their initial setting.
        ///   DataTemplate myDataTemplate = LayoutRoot.Resources["My_LayerTemplate"];
        ///   Legend1.MapLayerTemplate = _MapLayerTemplate;
        ///   Legend1.LayerTemplate = myDataTemplate;
        ///   Legend1.LegendItemTemplate = _LegendItemTemplate;
        /// }
        /// 
        /// private void Button_LegendItemTemplate_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Apply the custom DataTemplate for the LegendItemTemplate. Reset the other DataTemplates
        ///   // of the Legend Control back to their initial setting.
        ///   DataTemplate myDataTemplate = LayoutRoot.Resources["My_LegendItemTemplate"];
        ///   Legend1.MapLayerTemplate = _MapLayerTemplate;
        ///   Legend1.LayerTemplate = _LayerTemplate;
        ///   Legend1.LegendItemTemplate = myDataTemplate;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// ' There is 'by default' some form of DataTemplate for each of the Legend Templates (MapLayerTemplate, 
        /// ' LayerTemplate, and LegendItemTemplate). By using a global (i.e. Member) variable to store what 
        /// ' those 'default' templates are, we can reset them to their initial setting when using our individual 
        /// ' custom DataTemplates.
        /// 
        /// Public _MapLayerTemplate As DataTemplate
        /// Public _LayerTemplate As DataTemplate
        /// Public _LegendItemTemplate As DataTemplate
        /// 
        /// Public Sub New()
        ///   InitializeComponent()
        ///   
        ///   ' Populate the Member variables for the various DataTemplates of the Legend Control.
        ///   Dim theLegend As ESRI.ArcGIS.Client.Toolkit.Legend = Legend1
        ///   _MapLayerTemplate = theLegend.MapLayerTemplate
        ///   _LayerTemplate = theLegend.LayerTemplate
        ///   _LegendItemTemplate = theLegend.LegendItemTemplate
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_MapLayerTempalte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Apply the custom DataTemplate for the MapLayerTemplate. Reset the other DataTemplates
        ///   ' of the Legend Control back to their initial setting.
        ///   Dim myDataTemplate As DataTemplate = LayoutRoot.Resources("My_MapLayerTemplate")
        ///   Legend1.MapLayerTemplate = myDataTemplate
        ///   Legend1.LayerTemplate = _LayerTemplate
        ///   Legend1.LegendItemTemplate = _LegendItemTemplate
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_LayerTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Apply the custom DataTemplate for the LayerTemplate. Reset the other DataTemplates
        ///   ' of the Legend Control back to their initial setting.
        ///   Dim myDataTemplate As DataTemplate = LayoutRoot.Resources("My_LayerTemplate")
        ///   Legend1.MapLayerTemplate = _MapLayerTemplate
        ///   Legend1.LayerTemplate = myDataTemplate
        ///   Legend1.LegendItemTemplate = _LegendItemTemplate
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_LegendItemTemplate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Apply the custom DataTemplate for the LegendItemTemplate. Reset the other DataTemplates
        ///   ' of the Legend Control back to their initial setting.
        ///   Dim myDataTemplate As DataTemplate = LayoutRoot.Resources("My_LegendItemTemplate")
        ///   Legend1.MapLayerTemplate = _MapLayerTemplate
        ///   Legend1.LayerTemplate = _LayerTemplate
        ///   Legend1.LegendItemTemplate = myDataTemplate
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public DataTemplate LayerTemplate
		{
			get { return (DataTemplate)GetValue(LayerTemplateProperty); }
			set { SetValue(LayerTemplateProperty, value); }
		}

		/// /// <summary>
		/// Identifies the <see cref="LayerTemplate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LayerTemplateProperty =
			DependencyProperty.Register("LayerTemplate", typeof(DataTemplate), typeof(Legend), new PropertyMetadata(OnLayerTemplateChanged));

		private static void OnLayerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnLayerTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
		}

		private void OnLayerTemplateChanged(DataTemplate oldDataTemplate, DataTemplate newDataTemplate)
		{
			_legendTree.LayerTemplate = newDataTemplate;

		}
		#endregion

		#region ReverseLayersOrder
		/// <summary>
		/// Gets or sets a value indicating whether the legend displays the last declared layers as first legend items.
		/// By default the layers are returned in the order they are declared in the map or in the group layers.
		/// Setting this option allows to reverse the layers order returned by <see cref="LayerItemsSource"/> and <see cref="LayerItemViewModel.LayerItemsSource"/>.
		/// This option doesn't impact <see cref="LayerItems"/> nor <see cref="LayerItemViewModel.LayerItems"/> nor <see cref="LayerItemViewModel.LegendItems"/>.
		/// The order of the <see cref="ArcGISDynamicMapServiceLayer"/> sublayers is not impacted either.
		/// </summary>
		/// <value>
		///   <c>true</c> if <see cref="LayerItemsSource"/> returns reversed map layer items; otherwise, <c>false</c>.
		/// </value>
		public bool ReverseLayersOrder
		{
			get { return (bool)GetValue(ReverseLayersOrderProperty); }
			set { SetValue(ReverseLayersOrderProperty, value); }
		}

		/// /// <summary>
		/// Identifies the <see cref="ReverseLayersOrder"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ReverseLayersOrderProperty =
			DependencyProperty.Register("ReverseLayersOrder", typeof(bool), typeof(Legend), new PropertyMetadata(false, OnReverseLayersOrderChanged));

		private static void OnReverseLayersOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Legend) d).OnReverseLayersOrderChanged((bool)e.NewValue);
		}

		private void OnReverseLayersOrderChanged(bool newReverseLayersOrder)
		{
			_legendTree.ReverseLayersOrder = newReverseLayersOrder;

		}
		#endregion

		#region MapLayerTemplate
		/// <summary>
        /// Gets or sets the DataTemplate that controls how the Legend Control will display information about a Layer at 
        /// the hightest level in the Legend hierarchy. This DataTemplate is optional as the <see cref="LayerTemplate"/> 
        /// is used by default.
		/// </summary>
		/// <remarks>
        /// <para>
        /// This DataTemplate controls what is viewed in the Legend for the highest level of information about a 
        /// particular Layer. It presents each Layer name (via the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LegendItemViewModel.Label">LegendItemViewModel.Label</see> Property) 
        /// shown in the Map in a ContentControl with a node to expand any sub-Layer information. When a user hovers the 
        /// cursor over the Legend.MapLayerTemplate section, a ToolTip appears displaying the additional information about the 
        /// Layer including: Layer.Copyright, LegendItemViewModel.Description, LayerItemViewModel.MinimumResolution, 
        /// and LayerItemViewModel.MaximumResolution. The MapLayerTemplate value is optional; by default the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see> is used. 
        /// </para>
        /// <para>
        /// <b>Note:</b> The <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerItemsMode">Legend.LayerItemsMode</see> 
        /// Property has great impact on what is displayed in the Legend. For 
        /// users who want to see the most information available in the Legend, developers should set the LayerItemsMode 
        /// to <b>Tree</b>. If customizations are made to the MapLayerTemplate and they seem to be overridden at 
        /// runtime back to a default setting, it is most likely that the LayerItemsMode is set to the default of 
        /// <b>Flat</b> and it should be set to <b>Tree</b>.
        /// </para>
        /// <para>
        /// The objects that have Binding occur in the MapLayerTemplate are implied to be the Properties of the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel">LayerItemViewModel</see> Class.
        /// </para>
        /// <para>
        /// At the MapLayerTemplate level, one common customization technique would be add a TOC style of interaction. 
        /// Developers could add a CheckBox to manage the visibility of a Layer (including its sub-Layers) or add a 
        /// Slider to control the Opacity of the Layer (including its sub-Layers). Code examples of modifying the 
        /// MapLayerTemplate can be found in this document and the  
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Legend.LayerTemplate">Legend.LayerTemplate</see> document.
        /// </para>
        /// <para>
        /// It should be noted that it is possible to customize one or more of the Legend Control DataTemplates at the 
        /// same time. There are established default values for each DataTemplate, so setting one will not necessarily 
        /// override the others that have been set by default. Significant testing should be done on all of the Layers 
        /// in the customized application to ensure that the desired behavior has been achieved. Some Layers have 
        /// different behavior in rendering items in the Legend and should be tested thoroughly.
        /// </para>
        /// <para>
        /// The following screen shot demonstrates which part of the Legend Control corresponds to the three 
        /// DataTemplates. The Layer (ID = "United States") that is being displayed is an ArcGISDynamicMapServiceLayer 
        /// with three sub-Layers (ushigh, states, and counties). The information found in the ArcGIS Services Directory 
        /// about the ArcGISDynamicMapServiceLayer corresponds to what is shown in the Map and Legend Controls.
        /// </para>
        /// <para>
        /// <img border="0" alt="Using the ArcGIS Services Directory to view information about an ArcGISDynamicMapServiceLayer and how that corresponds to what is displayed in the Map and Legend Control. The DataTemplate parts of the Legend Control are specified." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.png"/>
        /// </para>
        /// <para>
        /// <b>TIP:</b> It is typically necessary for developers to specify the 
        /// <see cref="ESRI.ArcGIS.Client.Layer.ID">Layer.ID</see> name for Layers in the Map Control. This can be done
        /// in XAML or code-behind. If a Layer.ID value is not specified, then a default Layer.ID value is provided
        /// based upon the URL of the ArcGIS Server map service.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// This example code demonstrates setting the Legend.MapLayerTemplate to a custom DataTemplate defined in 
        /// the &lt;Resources/&gt; tags of XAML. The comments in the XAML code also show an option for setting the 
        /// Legend.MapLayerTemplate in-line of the Legend Control XAML. A code-behind function in the Legend_Refreshed 
        /// event demonstrates how to collapse all of the leaves to their highest level (the Layer level) in the Legend 
        /// Control. Use the Slider to change the Opacity of the Layers. Click the Hyperlink to see the ArcGIS Server 
        /// Directory metadata information about the REST web service.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt=" Applying a custom MapLayerTemplate to the Legend Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Legend.MapLayerTemplate.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Define a Resources section for use later in the XAML. --&gt;
        ///   &lt;Grid.Resources&gt;
        ///     
        ///     &lt;!--
        ///     Define the MapLayerTemplate. The DataTemplate adds the ability to control the Opacity (aka. the Visibility) of 
        ///     the various Layers (including any sub-Layers) via a Slider. Additionally, a HyperlinkButton provides Label
        ///     information for the various Layer names. The HyperlinkButton.Uri Property is set to the Url of the individual
        ///     Layers which will display the ArcGIS Server Directory metadata information about the REST web service. Setting 
        ///     the MapLayerTemplate does not override the default DataTemplate settings for the LayerTemplate and 
        ///     LegendItemTemplate. 
        ///     NOTE: The objects that have Binding occur in the MapLayerTemplate are implied to be the Properties of the 
        ///     ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel Class.
        ///     --&gt;
        ///     &lt;DataTemplate x:Key="MyMapLayerTemplate"&gt;
        ///       &lt;StackPanel Orientation="Horizontal"&gt;
        ///         &lt;Slider Maximum="1" Value="{Binding Layer.Opacity, Mode=TwoWay}" Width="50" /&gt;
        ///         &lt;HyperlinkButton  Content="{Binding Label}" NavigateUri="{Binding Layer.Url}"/&gt;
        ///       &lt;/StackPanel&gt;
        ///     &lt;/DataTemplate&gt;
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,188,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="400" Width="400" &gt;
        ///     
        ///     &lt;!-- Add several different types of Layers to the Map. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="Street Map" 
        ///             Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer"/&gt;
        ///       
        ///     &lt;esri:ArcGISDynamicMapServiceLayer ID="United States" Opacity="0.6" 
        ///            Url="http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer"/&gt;
        ///     
        ///     &lt;esri:FeatureLayer ID="H1N1 Flu Counts" Where="PLACE LIKE '%USA%'" OutFields="*"
        ///                    Url="http://servicesbeta.esri.com/ArcGIS/rest/services/Health/h1n1/MapServer/0" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Legend Control. It is bound to the Map Control. The MapLayerTemplate was defined in the &lt;Resources/&gt;
        ///   tags above. The LayerItemsMode of 'Tree' allows seeing of all of the Layer and sub-Layer items expandable by
        ///   leave nodes. 
        ///   NOTE: The Legend_Refreshed event has code-behind logic to cause all of the leaves in the Legend to be collapsed
        ///   to the highest level (i.e. the Layer level) when the application starts up. Had this code-behind logic not been 
        ///   used all of the leaves would have been expanded to their lowest level (this is the default behavior).
        ///   --&gt;
        ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="418,188,0,0" Name="Legend1" 
        ///                VerticalAlignment="Top" Width="350" Height="400" 
        ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree"
        ///                MapLayerTemplate="{StaticResource MyMapLayerTemplate}" 
        ///                Refreshed="Legend1_Refreshed"/&gt;
        ///     
        ///   &lt;!-- 
        ///   Interesting XAML Fact:
        ///   Rather than use the DataTemplate that was defined in a set of &lt;Resources/&gt; tags you could optionally
        ///   set the MapLayerTemplate in-line of the &lt;Legend/&gt; tags!
        ///   --&gt;
        ///   &lt;!--
        ///   &lt;esri:Legend HorizontalAlignment="Left" Margin="418,188,0,0" Name="Legend1" 
        ///                VerticalAlignment="Top" Width="350" Height="400" 
        ///                Map="{Binding ElementName=Map1}" LayerItemsMode="Tree"&gt;
        ///     &lt;esri:Legend.MapLayerTemplate&gt;
        ///       &lt;DataTemplate&gt;
        ///         &lt;StackPanel Orientation="Horizontal"&gt;
        ///           &lt;Slider Maximum="1" Value="{Binding Layer.Opacity, Mode=TwoWay}" Width="50" /&gt;
        ///           &lt;HyperlinkButton  Content="{Binding Label}" NavigateUri="{Binding Layer.Url}"/&gt;
        ///         &lt;/StackPanel&gt;
        ///       &lt;/DataTemplate&gt;
        ///     &lt;/esri:Legend.MapLayerTemplate&gt;
        ///   &lt;/esri:Legend&gt;
        ///   --&gt;
        /// 
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="123" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="756" 
        ///          TextWrapping="Wrap" Margin="12,12,0,0" 
        ///          Text="This example code demonstrates setting the Legend.MapLayerTemplate to a custom DataTemplate
        ///              defined in the &lt;Resources/&gt; tags of XAML. The comments in the XAML code also show an
        ///              option for setting the Legend.MapLayerTemplate in-line of the Legend Control XAML. A code-behind
        ///              function in the Legend_Refreshed event demonstrates how to collapse all of the leaves to their
        ///              highest level (the Layer level) in the Legend Control. Use the Slider to change the Opacity of
        ///              the Layers. Click the Hyperlink to see the ArcGIS Server Directory metadata information about 
        ///              the REST web service." /&gt;
        /// 
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // NOTE: Don’t forget to insert the following event handler wireups at the end of the 'InitializeComponent' 
        /// // method for forms, 'Page_Init' for web pages, or into a constructor for other classes:
        /// Legend1.Refreshed += new ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventHandler(Legend1_Refreshed);
        /// 
        /// private void Legend1_Refreshed(object sender, ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs e)
        /// {
        ///   // The purpose of the code in this function is to cause all of the leaves in the Legend to be collapsed
        ///   // to the highest level (i.e. the Layer level) when the application starts up. It is by setting the  
        ///   // LayerItemViewModel.IsExpandable = False for each Layer/sub-Layer that allows this behavior to occur.
        ///   // Had this code-behind logic not been used all of the leaves would have been expanded to their lowest level 
        ///   // in the Legend Control (this is the default behavior).
        ///   
        ///   // Get the Legend.
        ///   ESRI.ArcGIS.Client.Toolkit.Legend theLegend = (ESRI.ArcGIS.Client.Toolkit.Legend)sender;
        ///   
        ///   // Get the LayerItems of the Legend Control.
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel&gt; theObservableCollection = theLegend.LayerItems;
        ///   
        ///   // Loop through the ObservableCollection&lt;LayerItemViewModel&gt; objects for each Layer. 
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel theLayerItemViewModel in theObservableCollection)
        ///   {
        ///     // Close the leaves in the Legend Control for the particular Layer/sub-Layer.
        ///     theLayerItemViewModel.IsExpanded = false;
        ///   }
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Legend1_Refreshed(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs) Handles Legend1.Refreshed
        ///   
        ///   ' The purpose of the code in this function is to cause all of the leaves in the Legend to be collapsed
        ///   ' to the highest level (i.e. the Layer level) when the application starts up. It is by setting the  
        ///   ' LayerItemViewModel.IsExpandable = False for each Layer/sub-Layer that allows this behavior to occur.
        ///   ' Had this code-behind logic not been used all of the leaves would have been expanded to their lowest level 
        ///   ' in the Legend Control (this is the default behavior).
        ///   
        ///   ' Get the Legend.
        ///   Dim theLegend As ESRI.ArcGIS.Client.Toolkit.Legend = sender
        ///   
        ///   ' Get the LayerItems of the Legend Control.
        ///   Dim theObservableCollection As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel) = theLegend.LayerItems
        ///   
        ///   ' Loop through the ObservableCollection(Of LayerItemViewModel) objects for each Layer. 
        ///   For Each theLayerItemViewModel As ESRI.ArcGIS.Client.Toolkit.Primitives.LayerItemViewModel In theObservableCollection
        ///     
        ///     ' Close the leaves in the Legend Control for the particular Layer/sub-Layer.
        ///     theLayerItemViewModel.IsExpanded = False
        ///     
        ///   Next
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public DataTemplate MapLayerTemplate
		{
			get { return (DataTemplate)GetValue(MapLayerTemplateProperty); }
			set { SetValue(MapLayerTemplateProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="LegendItemTemplate"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapLayerTemplateProperty =
			DependencyProperty.Register("MapLayerTemplate", typeof(DataTemplate), typeof(Legend), new PropertyMetadata(OnMapLayerTemplateChanged));

		private static void OnMapLayerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Legend).OnMapLayerTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
		}

		private void OnMapLayerTemplateChanged(DataTemplate oldDataTemplate, DataTemplate newDataTemplate)
		{
			_legendTree.MapLayerTemplate = newDataTemplate;
		}
		#endregion

		#region Refresh
		/// <summary>
		/// Refreshes the legend control.
		/// </summary>
		/// <remarks>Note : In most cases, the control is always up to date without calling the refresh method.</remarks>
		public void Refresh()
		{
			_legendTree.Refresh();
		}
		#endregion

		#region public override void OnApplyTemplate()
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code 
		/// or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (!_isLoaded)
			{
				_isLoaded = true;

				if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && (Map == null || Map.Layers == null || Map.Layers.Count == 0))
				{
					if (_legendTree.LayerItems == null)
					{
						// Create a basic hierachy for design :  Map Layer -> SubLayer -> LegendItemViewModel
						LegendItemViewModel legendItem = new LegendItemViewModel()
						{
							Label = "LegendItem",
							ImageSource = new BitmapImage(new Uri("/ESRI.ArcGIS.Client.Toolkit;component/Images/legendItem.png", UriKind.Relative)),
						};

						LayerItemViewModel layerItem = new LayerItemViewModel()
						{
							Label = "LayerItem",
							LegendItems = new ObservableCollection<LegendItemViewModel>() { legendItem }
						};

						LayerItemViewModel mapLayerItem = new LayerItemViewModel()
						{
							Label = "MapLayerItem",
							LayerType = MapLayerItem.c_mapLayerType,
							LayerItems = new ObservableCollection<LayerItemViewModel>() { layerItem },
						};

						_legendTree.LayerItems = new ObservableCollection<LayerItemViewModel>() { mapLayerItem };
					}

				}
				else
				{
					// Initialize the Map now that all parameters are well known
					_legendTree.Map = Map;
				}
			}
		}

		#endregion

		#region Event Refreshed
		/// <summary>
		/// Occurs when the legend is refreshed. 
		/// Give the opportunity for an application to add or remove legend items.
		/// </summary>
		public event EventHandler<RefreshedEventArgs> Refreshed;

		private void OnRefreshed(object sender, RefreshedEventArgs args)
		{
			EventHandler<RefreshedEventArgs> refreshed = Refreshed;

			if (refreshed != null)
			{
				refreshed(this, args);
			}
		}
		#endregion

		#region class RefreshedEventArgs
		/// <summary>
		/// Legend Event Arguments used when the legend is refreshed.
		/// </summary>
		public sealed class RefreshedEventArgs : EventArgs
		{
			internal RefreshedEventArgs(LayerItemViewModel layerItem, Exception ex)
			{
				LayerItem = layerItem;
				Error = ex;
			}

			/// <summary>
			/// Gets the layer item being refreshed.
			/// </summary>
			/// <value>The layer item.</value>
			public LayerItemViewModel LayerItem { get; internal set; }

			/// <summary>
			/// Gets a value that indicates which error occurred during the legend refresh.
			/// </summary>
			/// <value>An System.Exception instance, if an error occurred during the refresh; otherwise null.</value>
			public Exception Error { get; internal set; }
		} 
		#endregion
	}
}
