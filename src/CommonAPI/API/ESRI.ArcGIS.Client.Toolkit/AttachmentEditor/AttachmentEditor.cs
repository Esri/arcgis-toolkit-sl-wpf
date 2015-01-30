// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ESRI.ArcGIS.Client.FeatureService;
using System.Windows.Media.Animation;
#if !SILVERLIGHT
using Microsoft.Win32;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{
        /// <summary>
        /// The AttachmentEditor Control allows for uploading, downloading, and deleting of files associated with 
        /// Graphic features in a FeatureLayer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The AttachmentEditor is a User Interface (UI) control that allows clients to upload, download, and delete 
        /// files associated with <see cref="ESRI.ArcGIS.Client.Graphic">Graphic</see> features in a 
        /// <see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see> of a back-end ArcGIS Server <b>FeatureServer</b> 
        /// database. The AttachmentEditor Control can be created in at design time in XAML or in dynamically at runtime 
        /// in the code-behind. The AttachmentEditor Control is one of several controls available in the Toolbox in 
        /// Visual Studio when the 
        /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
        /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
        /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
        ///  is installed, see the following screen shot:
        /// </para>
        /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the AttachmentEditor Control on the XAML design surface of a Silverlight application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor.png"/></para></ESRISILVERLIGHT>
        /// <ESRIWPF><para><img border="0" alt="Example of the AttachmentEditor Control on the XAML design surface of a WPF application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor_VS_WPF.png"/></para></ESRIWPF>
        /// <para>
        /// The default appearance of the AttachmentEditor Control can be modified using numerous inherited Properties 
        /// from System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. An example of 
        /// some of these Properties include: .Height, .Width, .BackGround, .BorderBrush, .BorderThickness, .FontFamily, 
        /// .FontSize, .FontStretch, .FontStyle, .Foreground, .HorizontalAlignment, .VerticalAlignment, .Margin, .Opacity, 
        /// .Visibility, etc. 
        /// </para>
        /// <para>
        /// Note: you cannot change the core behavior of the sub-components (i.e. Button, ListBox, etc.) of the 
        /// AttachmentEditor Control using standard Properties or Methods alone. To change the core behavior of the 
        /// sub-components and their appearance of the Control, developers can modify the Control Template in XAML and 
        /// the associated code-behind file. The easiest way to modify the UI sub-components is using Microsoft 
        /// Expression Blend. Then developers can delete/modify existing or add new sub-components in Visual Studio to 
        /// create a truly customized experience. A general approach to customizing a Control Template is discussed in 
        /// the ESRI blog entitled: 
        /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>.
        /// </para>
        /// <para>
        /// Additionally, changing the default behavior (or functionality) and appearance of the ListBox sub-component 
        /// that displays information about attachments in the AttachmentEditor Control can be modified using a DataTemplate 
        /// via the <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.ItemTemplate">AttachmentEditor.ItemTemplate</see> 
        /// Property and writing you own custom code when necessary. When defining the DataTemplate, the Binding Properties 
        /// that developers want to use to control the behavior of the AttachmentEditor are based upon those in the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo">ESRI.ArcGIS.Client.FeatureService.AttachmentInfo</see> 
        /// Class.
        /// </para>
        /// <para>
        /// To use the AttachmentEditor Control the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.FeatureLayer">AttachmentEditor.FeatureLayer</see> 
        /// Property must be set to a valid FeatureLayer that has 
        /// <a href="http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001t000003vt000000.htm" target="_blank">attachments enabled</a> 
        /// in an ArcGIS Server 
        /// <a href="http://help.arcgis.com/en/arcgisserver/10.0/help/arcgis_server_dotnet_help/index.html#//009300000021000000.htm" target="_blank">FeatureServer</a>. 
        /// Developers who do not have direct access to management of ArcGIS Server but need to know if An ArcGIS Server 
        /// FeatureLayer has attachments enabled can specify the FeatureLayer.Url in an web browser and look for the 
        /// <b>Has Attachments</b> section to see if the value is <b>True</b>, see the following screen shot:
        /// </para>
        /// <img border="0" alt="Using the FeatureLayer.Url Property to see if 'Has Attachments' is True." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor2.png"/> 
        /// <para>
        /// Additionally, developers can programmatically discover if a FeatureLayer <b>Has Attachments</b> by using the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureLayer.LayerInfo">FeatureLayer.LayerInfo</see> Property to return an 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo">ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo</see> 
        /// Class. Use the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo.HasAttachments">ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo.HasAttachments</see> 
        /// Property to obtain a Boolean value if attachments are enabled for the FeatureService. A code example is provided 
        /// for programmatically discovering if a FeatureLayer <b>Has Attachments</b> in the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.FeatureLayer">AttachmentEditor.FeatureLayer</see> 
        /// Property documentation.
        /// </para>
        /// <para>
        /// If the AttachmentEditor.FeatureLayer Property is not set to a valid ArcGIS Server <b>FeatureServer</b> service 
        /// with attachments enabled, a Visual Studio NotSupportedException error containing syntax similar to the following 
        /// will occur: "Layer does not support attachments" or "Layer does not support adding attachments". It should be 
        /// noted the FeatureLayer.Url Property of an ArcGIS Server <b>MapServer</b> service looks very similar to 
        /// <b>FeatureServer</b> and might even display a <b>Has Attributes</b> value equal True when viewed in a web 
        /// browser but this should not be used for the AttachmentEditor Control. A valid <b>FeatureServer</b> will have 
        /// the words "FeatureServer" in the URL, not "MapServer" which will cause errors.   
        /// </para>
        /// <para>
        /// It is the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.GraphicSource">AttachmentEditor.GraphicSource</see> 
        /// Property that causes the AttachmentEditor to make a call to the ArcGIS Server <b>FeatureServer</b> to see if any 
        /// existing attachments are associated with a specific Graphic in the FeatureLayer stored in the backend database. 
        /// If any attachments are found to be associated with a Graphic in the FeatureLayer, they will be listed in the 
        /// ListBox portion of the control. For each attachment that is listed in the ListBox of the AttachmentEditor, users 
        /// can click on the hyperlink with the attachment filename to use the Microsoft File Download dialog to save the 
        /// file on the local machine. Attachments listed in the ListBox can also be deleted from the backend ArcGIS Server 
        /// FeatureService by clicking the red X next to the attachment name. 
        /// </para>
        /// <para>
        /// When a Graphic is associated with the AttachmentEditor.GraphicSource Property the <b>Add</b> button of the 
        /// AttachmentEditor becomes enabled. The <b>Add</b> button allows users on the client machine to upload files on 
        /// the local computer to the ArcGIS Server <b>FeatureServer</b> backend database via the Microsoft OpenFileDialog 
        /// dialog. The <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.Filter">AttachmentEditor.Filter</see>, 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.FilterIndex">AttachmentEditor.FilterIndex</see>, and 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.Multiselect">AttachmentEditor.Multiselect</see> Properties 
        /// control the behavior of the Microsoft OpenFileDialog, see the following screen shot: 
        /// </para>
        /// <img border="0" alt="Demonstrating what areas of the Microsoft OpenFileDialog is controlled by the AttachmentEditor’s Filter, FilterIndex, and Multiselect Properties." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor3.png"/>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// When the Map loads click on a point feature in the FeatureLayer to select the Graphic. Then use the 
        /// AttachmentEditor to add, delete, and download files.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Using the AttachmentEditor to download, upload, and delete files." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor4.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="64" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///              Width="512" TextWrapping="Wrap" Margin="12,12,0,0" 
        ///              Text="When the Map loads click on a point feature in the FeatureLayer to select
        ///                    the Graphic. Then use the AttachmentEditor to add, delete, and download files." /&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Map control with an ArcGISTiledMapeServiceLayer and a FeatureLayer. It is important to 
        ///   provide a Name for the Map Control as this will be used by the AttachmentEditor Control.
        ///   Define and initial Extent for the map.
        ///   --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,100,0,0" Name="MyMap" 
        ///             VerticalAlignment="Top" Height="272" Width="400" 
        ///             Extent="-13625087,4547888,-13623976,4548643"&gt;
        ///   
        ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer 
        ///               Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///     
        ///     &lt;!-- 
        ///     The FeatureLayer is based upon an ArcGIS Server 'FeatureServer' service not a 'MapServer' service.
        ///     It is important to provide an ID value as this will be used for Binding in the AttachementEditor.
        ///     The MouseLeftButtonUp Event handler has been defined for the FeatureLayer. 
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="IncidentsLayer"
        ///            Url="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"
        ///            MouseLeftButtonUp="FeatureLayer_MouseLeftButtonUp" AutoSave="False"                  
        ///            DisableClientCaching="True" OutFields="*" Mode="OnDemand" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Wrap the Attachment Editor inside of a Grid and other controls to provide a stylistic look 
        ///   and to provide instructions to the user.
        ///   --&gt;
        ///   &lt;Grid HorizontalAlignment="Left" VerticalAlignment="Top" Margin="450,100,15,0" &gt;
        ///     &lt;Rectangle Fill="BurlyWood" Stroke="Green"  RadiusX="15" RadiusY="15" Margin="0,0,0,5" &gt;
        ///       &lt;Rectangle.Effect&gt;
        ///         &lt;DropShadowEffect/&gt;
        ///       &lt;/Rectangle.Effect&gt;
        ///     &lt;/Rectangle&gt;
        ///     &lt;Rectangle Fill="#FFFFFFFF" Stroke="Aqua" RadiusX="5" RadiusY="5" Margin="10,10,10,15" /&gt;
        ///     &lt;StackPanel Orientation="Vertical" HorizontalAlignment="Left"&gt;
        ///       &lt;TextBlock Text="Click on a point feature to select it, and click the 'Add' button to attach a file." 
        ///                      Width="275" TextAlignment="Left" Margin="20,20,20,5" TextWrapping="Wrap" FontWeight="Bold"/&gt;
        ///     
        ///       &lt;!--
        ///       Add an AttachmentEditor. Provide a x:Name for the AttachmentEditor so that it can be used in the 
        ///       code-behind file. The FeatureLayer Property is bound to the 'IncidentLayer' of the Map Control.
        ///       Two groups of Filter types are used (you can have multiple file extensions in a group) to drive the
        ///       OpenFileDialog. The FilterIndex shows the first group of Filter Types in the OpenFileDialog 
        ///       dropdown list. Only allow the user to upload (i.e. Add) one file at a time using the OpenFileDialog
        ///       with the Multiselect set to False. An UploadFailed Event has been defined for the code-behind.
        ///       --&gt;
        ///       &lt;esri:AttachmentEditor x:Name="MyAttachmentEditor" VerticalAlignment="Top" Margin="20,5,20,20" 
        ///                            Background="LightGoldenrodYellow" Width="280" Height="190" HorizontalAlignment="Right"                              
        ///                            FeatureLayer="{Binding Layers[IncidentsLayer], ElementName=MyMap}" 
        ///                            Filter="Image Files|*.jpg;*.gif;*.png;*.bmp|Adobe PDF Files (.pdf)|*.pdf" 
        ///                            FilterIndex="1" Multiselect="False"
        ///                            UploadFailed="MyAttachmentEditor_UploadFailed"&gt;
        ///       &lt;/esri:AttachmentEditor&gt;
        ///     &lt;/StackPanel&gt;
        ///   &lt;/Grid&gt;
        ///       
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void FeatureLayer_MouseLeftButtonUp(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
        /// {
        ///   // This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   // un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   // it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   // downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   // Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = sender as ESRI.ArcGIS.Client.FeatureLayer;
        ///   
        ///   // Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   for (int i = 0; i &lt; myFeatureLayer.SelectionCount; i++)
        ///   {
        ///     myFeatureLayer.SelectedGraphics.ToList()[i].UnSelect();
        ///   }
        ///   
        ///   // Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select();
        ///   
        ///   // Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   // to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic;
        /// }
        /// 
        /// private void MyAttachmentEditor_UploadFailed(object sender, ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.UploadFailedEventArgs e)
        /// {
        ///   // Display any error messages that may occur if the AttachmentEditor fails to upload a file.
        ///   MessageBox.Show(e.Result.Message);
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub FeatureLayer_MouseLeftButtonUp(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs)
        ///   
        ///   ' This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   ' un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   ' it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   ' downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   ' Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = TryCast(sender, ESRI.ArcGIS.Client.FeatureLayer)
        ///   
        ///   ' Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   For i As Integer = 0 To myFeatureLayer.SelectionCount - 1
        ///     myFeatureLayer.SelectedGraphics.ToList()(i).UnSelect()
        ///   Next i
        ///   
        ///   ' Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select()
        ///   
        ///   ' Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   ' to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic
        ///   
        /// End Sub
        /// 
        /// Private Sub MyAttachmentEditor_UploadFailed(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.UploadFailedEventArgs)
        ///   ' Display any error messages that may occur if the AttachmentEditor fails to upload a file.
        ///    MessageBox.Show(e.Result.Message)
        /// End Sub
        /// </code>
        /// </example>
    [TemplatePart(Name = "AddNewButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "AttachmentList", Type = typeof(ItemsControl))]
    [StyleTypedProperty(Property = "ItemTemplate", StyleTargetType = typeof(FrameworkElement))]
    [TemplateVisualState(GroupName = "BusyStates", Name = "Loaded")]
    [TemplateVisualState(GroupName = "BusyStates", Name = "Busy")]
    public class AttachmentEditor : Control
    {
        #region Private Fields
        private ButtonBase _addNewButton = null;
        private ItemsControl _attachmentList = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentEditor"/> class.
        /// </summary>
        public AttachmentEditor()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(AttachmentEditor);
#endif
            this.Drop += AttachmentEditor_Drop;
            this.DragOver += AttachmentEditor_DragOver;
        }

        static AttachmentEditor()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AttachmentEditor), 
                    new FrameworkPropertyMetadata(typeof(AttachmentEditor)));
#endif
        }
        #endregion

        #region Dependency Properties

        /// <summary> 
        /// Gets or sets the FeatureLayer for which the AttachmentEditor will use as its data source to display attachment 
        /// information.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To use the AttachmentEditor Control the AttachmentEditor.FeatureLayer Property must be set to a valid 
        /// FeatureLayer that has 
        /// <a href="http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//001t000003vt000000.htm" target="_blank">attachments enabled</a> 
        /// in an ArcGIS Server 
        /// <a href="http://help.arcgis.com/en/arcgisserver/10.0/help/arcgis_server_dotnet_help/index.html#//009300000021000000.htm" target="_blank">FeatureServer</a>. 
        /// Developers who do not have direct access to management of ArcGIS Server but need to know if An ArcGIS Server 
        /// FeatureLayer has attachments enabled can do so programmatically by using the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureLayer.LayerInfo">FeatureLayer.LayerInfo</see> Property to return an 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo">ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo</see> 
        /// object. Use the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo.HasAttachments">ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo.HasAttachments</see> 
        /// Property to obtain a Boolean value to know if attachments are enabled for the FeatureService. A code example is 
        /// provided in this document for programmatically discovering if a FeatureLayer <b>Has Attachments</b>.
        /// </para>
        /// <para>
        /// Alternatively, developers can specify the FeatureLayer.Url in an web browser and look for the 
        /// <b>Has Attachments</b> section to see if the value is <b>True</b>, see the following screen shot:
        /// </para>
        /// <img border="0" alt="Using the FeatureLayer.Url Property to see if ‘Has Attachments’ is True." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor2.png"/>
        /// <para>
        /// If the AttachmentEditor.FeatureLayer Property is not set to a valid ArcGIS Server <b>FeatureServer</b> service 
        /// with attachments enabled, a Visual Studio NotSupportedException error containing syntax similar to the following 
        /// will occur: "Layer does not support attachments" or "Layer does not support adding attachments". It should be 
        /// noted the FeatureLayer.Url Property of an ArcGIS Server <b>MapServer</b> service looks very similar to 
        /// <b>FeatureServer</b> and might even display a <b>Has Attributes</b> value equal True when viewed in a web 
        /// browser but this should not be used for the AttachmentEditor Control. A valid <b>FeatureServer</b> will have 
        /// the words “FeatureServer” in the URL, not “MapServer” which will cause errors.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the button to add the FeatureLayer to the Map Control and wire-up an AttachmentEditor if applicable. The 
        /// code example demonstrates adding a FeatureLayer dynamically at run-time. If the FeatureLayer has attachments 
        /// enabled (i.e. True) in the FeatureService, dynamically add the functionality to associate an AttachmentEditor 
        /// with the FeatureLayer for uploading/downloading/deleting attachments associated with the Graphic features in the 
        /// FeatureLayer.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Check if a FeatureLayer has attachments enabled in the FeatureService and dynamically associate the AttachmentEditor if True." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor.FeatureLayer.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        ///   &lt;!--
        ///   This code example demonstrates adding a FeatureLayer dynamically at run-time. If the FeatureLayer 
        ///   has attachments enabled (i.e. True) in the FeatureService, dynamically add the functionality
        ///   to associate an AttachmentEditor with the FeatureLayer for uploading/downloading/deleting
        ///   attachments associated with the Graphic features in the FeatureLayer.
        ///   --&gt;
        ///   &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///     &lt;!-- Add a button with instructions. The Click Event is enabled to for code-behind functionality. --&gt;
        ///     &lt;Button Content="Add the FeatureLayer to the Map and wire-up an AttachmentEditor if applicable." 
        ///             Height="23" HorizontalAlignment="Left" Margin="12,6,0,0" Name="Button1" 
        ///             VerticalAlignment="Top" Width="616" Click="Button1_Click"/&gt;
        ///     
        ///     &lt;!-- Add a Label telling the user to supply a FeatureLayer.Url --&gt;
        ///     &lt;sdk:Label Height="28" HorizontalAlignment="Left" Name="Label1" VerticalAlignment="Top" 
        ///                Width="182"  Content="Enter a Url for a FeatureLayer:" Margin="12,41,0,0" /&gt;
        ///     
        ///     &lt;!-- 
        ///     Add a default text string for a FeatureLayer.Url where attachements are enabled in the
        ///     FeatureServer.
        ///     
        ///     A FeatureLayer.Url WITH Attachments: 
        ///     "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0" 
        ///     
        ///     A FeatureLayer.Url WITHOUT Attachments: 
        ///     "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Fire/Sheep/FeatureServer/0"
        ///     --&gt;
        ///     &lt;TextBox Height="23" HorizontalAlignment="Left" Margin="12,58,0,0" Name="TextBox_FeatureLayerUrl" 
        ///              VerticalAlignment="Top" Width="628" 
        ///              Text="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"/&gt;
        ///     
        ///     &lt;!-- Add a Label informing the user if FeatureLayer supports attachents. --&gt;
        ///     &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="12,98,0,0" Name="Label2" 
        ///                VerticalAlignment="Top" Width="300" 
        ///                Content="Does the FeatureLayer support having attachments?"/&gt;
        ///     
        ///     &lt;!-- 
        ///     This is the TexBlock that will provide a True or False answer. A True means the FeatureLayer
        ///     supports having attachments. A False means the FeatureLayer does not support adding attachments. 
        ///     The TextBlock is invisible by default until the user clicks the Button1.
        ///     --&gt;
        ///     &lt;TextBlock Height="23" HorizontalAlignment="Left" Margin="307,98,0,0" 
        ///                Name="TextBlock_FeatureLayerLayerInfoHasAttachments" 
        ///                Text="TextBlock" VerticalAlignment="Top" Visibility="Collapsed"/&gt;
        ///     
        ///     &lt;!-- Add an ESRI Map Control. --&gt;
        ///     &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,124,0,0" Name="Map1" 
        ///               VerticalAlignment="Top" Height="344" Width="320"&gt;
        ///     
        ///       &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
        ///       &lt;esri:ArcGISTiledMapServiceLayer 
        ///               Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a default AttachmentEditor Control and set it to be invisible by default. Only if the 
        ///   supplied FeatureLayer has attachments = True will it be displayed. 
        ///   --&gt;      
        ///   &lt;esri:AttachmentEditor HorizontalAlignment="Left" Margin="338,124,0,0" Name="AttachmentEditor1" 
        ///                          VerticalAlignment="Top" Height="344" Width="290" Visibility="Collapsed"/&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function is the main driver for the sample code. It creates a FeatureLayer dynamically
        ///   // and adds it to the Map Control. The FeatureLayer.Initialized Event handler is dynamically 
        ///   // wired-up and activated when the FeatureLayer is added to the Map.
        ///   
        ///   // Create a new FeatureLayer using the Url text string supplied by the user.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = new ESRI.ArcGIS.Client.FeatureLayer();
        ///   myFeatureLayer.Url = TextBox_FeatureLayerUrl.Text;
        ///   
        ///   // Add the FeatureLayer to the Map.
        ///   Map1.Layers.Add(myFeatureLayer);
        ///   
        ///   // Create the FeatureLayer.Intialized Event handler. It will be invoked automatically
        ///   // when the FeatureLayer is added to the Map Control.
        ///   myFeatureLayer.Initialized += myFeatureLayer_Initialized;
        /// }
        /// 
        /// private void myFeatureLayer_Initialized(object sender, EventArgs e)
        /// {
        ///   // This function executes as a result of the FeatureLayer being initialized for the first time.
        ///   // If FeatureLayer suppports having attachments in the FeatureService, set the properties on
        ///   // the AttachmentEditor Control and wire-up the FeatureLayer.MouseLeftButtonUp Event handler
        ///   // which will allow users to click on a Graphic and display any attachment information.
        ///   
        ///   // Obtain the FeatureLayer from the sender argument.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = sender as ESRI.ArcGIS.Client.FeatureLayer;
        ///   
        ///   // Get a FeatureService.FeatureLayerInfo object from the FeatureLayer.
        ///   ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo myFeatureLayerInfo = myFeatureLayer.LayerInfo;
        ///   
        ///   // Display if attachments have been enabled in the TextBlock.
        ///   TextBlock_FeatureLayerLayerInfoHasAttachments.Text = myFeatureLayerInfo.HasAttachments.ToString();
        ///   TextBlock_FeatureLayerLayerInfoHasAttachments.Visibility = Windows.Visibility.Visible;
        ///   
        ///   if (myFeatureLayerInfo.HasAttachments == true)
        ///   {
        ///     // If attachments are enabled in the FeatureLayer's FeatureService, set up the properties
        ///     // of the AttachmentEditor.
        ///     AttachmentEditor1.FeatureLayer = myFeatureLayer;
        ///     AttachmentEditor1.Filter = "Image Files|*.jpg;*.gif;*.png;*.bmp|Adobe PDF Files (.pdf)|*.pdf";
        ///     AttachmentEditor1.FilterIndex = 1;
        ///     AttachmentEditor1.Multiselect = true;
        ///     AttachmentEditor1.Visibility = Windows.Visibility.Visible;
        ///     
        ///     // Add the FeatureLayer.MouseLeftButtonUp Event handler to allow users to click on a 
        ///     // Graphic and display any attachment information.
        ///     myFeatureLayer.MouseLeftButtonUp += myFeatureLayer_MouseLeftButtonUp;
        ///   }
        /// }
        /// 
        /// private void myFeatureLayer_MouseLeftButtonUp(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
        /// {
        ///   // This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   // un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   // it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   // downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   // Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = sender as ESRI.ArcGIS.Client.FeatureLayer;
        ///   
        ///   // Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   for (int i = 0; i &lt; myFeatureLayer.SelectionCount; i++)
        ///   {
        ///     myFeatureLayer.SelectedGraphics.ToList()[i].UnSelect();
        ///   }
        ///   
        ///   // Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select();
        ///   
        ///   // Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   // to update with any attachments currently associated with the Graphic.
        ///   AttachmentEditor1.GraphicSource = e.Graphic;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function is the main driver for the sample code. It creates a FeatureLayer dynamically
        ///   ' and adds it to the Map Control. The FeatureLayer.Initialized Event handler is dynamically 
        ///   ' wired-up and activated when the FeatureLayer is added to the Map.
        ///   
        ///   ' Create a new FeatureLayer using the Url text string supplied by the user.
        ///   Dim myFeatureLayer As New ESRI.ArcGIS.Client.FeatureLayer
        ///   myFeatureLayer.Url = TextBox_FeatureLayerUrl.Text
        ///   
        ///   ' Add the FeatureLayer to the Map.
        ///   Map1.Layers.Add(myFeatureLayer)
        ///   
        ///   ' Create the FeatureLayer.Intialized Event handler. It will be invoked automatically
        ///   ' when the FeatureLayer is added to the Map Control.
        ///   AddHandler myFeatureLayer.Initialized, AddressOf myFeatureLayer_Initialized
        /// End Sub
        /// 
        /// Private Sub myFeatureLayer_Initialized(ByVal sender As Object, ByVal e As EventArgs)
        ///   
        ///   ' This function executes as a result of the FeatureLayer being initialized for the first time.
        ///   ' If FeatureLayer suppports having attachments in the FeatureService, set the properties on
        ///   ' the AttachmentEditor Control and wire-up the FeatureLayer.MouseLeftButtonUp Event handler
        ///   ' which will allow users to click on a Graphic and display any attachment information.
        ///   
        ///   ' Obtain the FeatureLayer from the sender argument.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = TryCast(sender, ESRI.ArcGIS.Client.FeatureLayer)
        ///   
        ///   ' Get a FeatureService.FeatureLayerInfo object from the FeatureLayer.
        ///   Dim myFeatureLayerInfo As ESRI.ArcGIS.Client.FeatureService.FeatureLayerInfo = myFeatureLayer.LayerInfo
        ///   
        ///   ' Display if attachments have been enabled in the TextBlock.
        ///   TextBlock_FeatureLayerLayerInfoHasAttachments.Text = myFeatureLayerInfo.HasAttachments.ToString
        ///   TextBlock_FeatureLayerLayerInfoHasAttachments.Visibility = Windows.Visibility.Visible
        ///   
        ///   If myFeatureLayerInfo.HasAttachments = True Then
        ///     
        ///     ' If attachments are enabled in the FeatureLayer's FeatureService, set up the properties
        ///     ' of the AttachmentEditor.
        ///     AttachmentEditor1.FeatureLayer = myFeatureLayer
        ///     AttachmentEditor1.Filter = "Image Files|*.jpg;*.gif;*.png;*.bmp|Adobe PDF Files (.pdf)|*.pdf"
        ///     AttachmentEditor1.FilterIndex = 1
        ///     AttachmentEditor1.Multiselect = True
        ///     AttachmentEditor1.Visibility = Windows.Visibility.Visible
        ///     
        ///     ' Add the FeatureLayer.MouseLeftButtonUp Event handler to allow users to click on a 
        ///     ' Graphic and display any attachment information.
        ///     AddHandler myFeatureLayer.MouseLeftButtonUp, AddressOf myFeatureLayer_MouseLeftButtonUp
        ///     
        ///   End If
        ///   
        /// End Sub
        ///   
        /// Private Sub myFeatureLayer_MouseLeftButtonUp(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs)
        ///   
        ///   ' This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   ' un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   ' it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   ' downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   ' Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = TryCast(sender, ESRI.ArcGIS.Client.FeatureLayer)
        ///   
        ///   ' Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   For i As Integer = 0 To myFeatureLayer.SelectionCount - 1
        ///     myFeatureLayer.SelectedGraphics.ToList()(i).UnSelect()
        ///   Next i
        ///   
        ///   ' Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select()
        ///   
        ///   ' Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   ' to update with any attachments currently associated with the Graphic.
        ///   AttachmentEditor1.GraphicSource = e.Graphic
        ///   
        /// End Sub
        /// </code>
        /// </example>
        public FeatureLayer FeatureLayer
        {
            get { return (FeatureLayer)GetValue(FeatureLayerProperty); }
            set { SetValue(FeatureLayerProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FeatureLayer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FeatureLayerProperty =
            DependencyProperty.Register("FeatureLayer", typeof(FeatureLayer), typeof(AttachmentEditor), new PropertyMetadata(OnFeatureLayerPropertyChanged));
        private static void OnFeatureLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AttachmentEditor AttachmentEditor = d as AttachmentEditor;
            FeatureLayer oldValue = e.OldValue as FeatureLayer;
            FeatureLayer newValue = e.NewValue as FeatureLayer;
            if (oldValue != null)
                oldValue.Initialized -= AttachmentEditor.FeatureLayer_Initialized;
            if (newValue != null)
            {
                if (newValue.IsInitialized)
                    AttachmentEditor.LoadAttachments();
                else
                    newValue.Initialized += AttachmentEditor.FeatureLayer_Initialized;
            }
            else
                AttachmentEditor.LoadAttachments();
        }

        /// <summary>
        /// Gets or sets the Graphic source of the FeatureLayer for the AttachmentEditor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is the AttachmentEditor.GraphicSource Property that causes the AttachmentEditor to make a call to the ArcGIS 
        /// Server <b>FeatureServer</b> to see if any existing attachments are associated with a specific Graphic in the 
        /// FeatureLayer stored in the backend database. If any attachments are found to be associated with a Graphic in 
        /// the FeatureLayer, they will be listed in the ListBox portion of the control. For each attachment that is listed 
        /// in the ListBox of the AttachmentEditor, users can click on the hyperlink with the attachment filename to use the 
        /// Microsoft File Download dialog to save the file on the local machine. Attachments listed in the ListBox can also 
        /// be deleted from the backend ArcGIS Server FeatureService by clicking the red X next to the attachment name. When 
        /// a Graphic is associated with the AttachmentEditor.GraphicSource Property the <b>Add</b> button of the 
        /// AttachmentEditor becomes enabled. The <b>Add</b> button allows users on the client machine to upload files on the 
        /// local computer to the ArcGIS Server <b>FeatureServer</b> backend database via the Microsoft OpenFileDialog dialog.
        /// </para>
        /// <para>
        /// Obtaining a Graphic feature from the FeatureLayer to be used as the AttachmentEditor.GraphicSource is typically 
        /// obtained by a Property on the FeatureLayer such as: 
        /// <list type="bullet">
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.Graphics">FeatureLayer.Graphics</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.SelectedGraphics">FeatureLayer.SelectedGraphics</see></item>
        /// </list>
        /// or via an Event on the FeatureLayer such as:
        /// <list type="bullet">
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseEnter">FeatureLayer.MouseEnter</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseLeave">FeatureLayer.MouseLeave</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseLeftButtonDown">FeatureLayer.MouseLeftButtonDown</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseLeftButtonUp">FeatureLayer.MouseLeftButtonUp</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseMove">FeatureLayer.MouseMove</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseRightButtonDown">FeatureLayer.MouseRightButtonDown</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.GraphicsLayer.MouseRightButtonUp">FeatureLayer.MouseRightButtonUp</see></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// This sample code shows setting the AttachmentEditor.GraphicSource two different ways. One way uses the 
        /// ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs input argument of the FeatureLayer.MouseLeftButtonUp Event 
        /// to attach the Graphic to the AttachmentEditor.GraphicSource. The other way obtains the Graphic by iterating 
        /// over the GraphicCollection from a user choice of selecting the objectid value in a ListBox to set the 
        /// AttachmentEditor.GraphicSource:
        /// <list type="number">
        ///   <item>
        ///   Click on the Map to select a Graphic and add a few Attachments using the AttachmentEditor Control. Repeat 
        ///   this step for several Graphics.
        ///   </item>
        ///   <item>
        ///   Then Click the 'Load the objectid of Graphic with Attachments in the ListBox' button to list all of the 
        ///   Graphic 'objectid' values that have Attachments in the ListBox for all Graphics in the current Map's Extent.
        ///   </item>
        ///   <item>
        ///   Finally, Click on the 'objectid' string in the ListBox and the associated Graphic will be selected in the 
        ///   Map and will automatically display it's Attachment information in the AttachmentEditor Control.
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating setting the AttachmentEditor.GraphicSource using two alternative ways." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor.GraphicSource.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="189" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///          Width="760" TextWrapping="Wrap" Margin="12,12,0,0" 
        ///          Text="This sample code shows setting the AttachmentEditor.GraphicSource two different ways. One way uses 
        ///          the ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs input argument of the FeatureLayer.MouseLeftButtonUp 
        ///          Event to attach the Graphic to the AttachmentEditor.GraphicSource. The other way obtains the Graphic by 
        ///          iterating over the GraphicCollection from a user choice of selecting the objectid value in a ListBox to 
        ///          set the AttachmentEditor.GraphicSource:
        ///          1. Click on the Map to select a Graphic and add a few Attachments using the AttachmentEditor Control. 
        ///          Repeat this step for several Graphics.
        ///          2. Then Click the 'Load the objectid of Graphic with Attachments in the ListBox' button to list all of 
        ///          the Graphic 'objectid' values that have Attachments in the ListBox for all Graphics in the current 
        ///          Map's Extent.
        ///          3. Finally, Click on the 'objectid' string in the ListBox and the associated Graphic will be selected in 
        ///          the Map and will automatically display it's Attachment information in the AttachmentEditor Control." /&gt;
        ///          
        ///   &lt;!-- 
        ///   Add a Map control with an ArcGISTiledMapeServiceLayer and a FeatureLayer. It is important to 
        ///   provide a Name for the Map Control as this will be used by the AttachmentEditor Control.
        ///   Define and initial Extent for the map.
        ///   --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,225,0,0" Name="MyMap" 
        ///           VerticalAlignment="Top" Height="375" Width="418" 
        ///             Extent="-13629929,4545159,-13627707,4547153"&gt;
        ///   
        ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer 
        ///               Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///     
        ///     &lt;!-- 
        ///     The FeatureLayer is based upon an ArcGIS Server 'FeatureServer' service not a 'MapServer' service.
        ///     It is important to provide an ID value as this will be used for Binding in the AttachementEditor.
        ///     The MouseLeftButtonUp Event handler has been defined for the FeatureLayer. In order to improve
        ///     performance of the example code a Where clause was used to restrict the amount of Graphic features
        ///     returned from the FeatureLayer FeatureService. Setting the Mode Property equal to OnDemand also 
        ///     helps to improve performance by further restricting the returned Graphc features to only those that
        ///     are visible in the current Map.Extent.
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="IncidentsLayer"
        ///            Url="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"
        ///            MouseLeftButtonUp="FeatureLayer_MouseLeftButtonUp" AutoSave="False"                  
        ///            DisableClientCaching="True" OutFields="*" Mode="OnDemand" 
        ///            Where="district = '8' AND req_type = 'Damaged Property'"/&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="440,207,0,0" Name="Label2" 
        ///              VerticalAlignment="Top" Width="290" Content="The Attachment Editor:"/&gt;
        ///   
        ///   &lt;!--
        ///   Add an AttachmentEditor. Provide a x:Name for the AttachmentEditor so that it can be used in the 
        ///   code-behind file. The FeatureLayer Property is bound to the 'IncidentLayer' of the Map Control.
        ///   Only one group of Filter types are used (you can have multiple file extensions in a group) to drive 
        ///   the OpenFileDialog. This AttachmentEditor will only allow the uploading of Image files. The 
        ///   FilterIndex shows the first group of Filter Types in the OpenFileDialog dropdown list. Only allow 
        ///   the user to upload (i.e. Add) one file at a time using the OpenFileDialog with the Multiselect set 
        ///   to False. 
        ///   --&gt;
        ///   &lt;esri:AttachmentEditor x:Name="MyAttachmentEditor" VerticalAlignment="Top" Margin="436,225,0,0" 
        ///                            Background="LightGoldenrodYellow" Width="352" Height="110" HorizontalAlignment="Left"                              
        ///                            FeatureLayer="{Binding Layers[IncidentsLayer], ElementName=MyMap}" 
        ///                            Filter="Image Files|*.jpg;*.gif;*.png;*.bmp" 
        ///                            FilterIndex="1" Multiselect="False"&gt;
        ///   &lt;/esri:AttachmentEditor&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Button to perform the work of looping through all of the Graphics in the current Map.Extent, 
        ///   obtaining which Graphics have associated Attachments, and display the 'objectid' values of those
        ///   Graphics in the ListBox. The Click Event handler is wired up for use in the code-behind. 
        ///   --&gt;
        ///   &lt;Button  Content="Load the objectid of Graphic with Attachments in the ListBox" Height="35" 
        ///           HorizontalAlignment="Left" Margin="436,361,0,0" Name="Button1" 
        ///           VerticalAlignment="Top" Width="352" Click="Button1_Click" &gt;
        ///   &lt;/Button&gt;
        ///   
        ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="436,419,0,0" Name="Label1" 
        ///              VerticalAlignment="Top" Width="336"
        ///              Content="Graphics object in the Map with Attachments:"/&gt;
        ///   
        ///   &lt;!--
        ///   Add a ListBox to display any Graphic 'objectid' values in the current Map.Extent that have
        ///   Attachments associated with them. If no Graphic features in the current Map.Extent have 
        ///   Attachments nothing will be displayed. You may need to add some Attachments to the Graphics
        ///   in order to see the full functionality of the example code. The SelectionChanged Event handler
        ///   is wired up for use in the code-behind.
        ///   --&gt;
        ///   &lt;ListBox Height="145" HorizontalAlignment="Left" Margin="436,443,0,0" Name="ListBox1" 
        ///            VerticalAlignment="Top" Width="352"
        ///            SelectionChanged="ListBox1_SelectionChanged" /&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // The purpose of this example code is to show that the AttachmentEditor.GraphicSource can be set a few
        /// // different ways: 
        /// // (1) via user interaction with the Map 
        /// // (2) via user interaction with non-Map controls
        /// // The important thing to learn is that a valid Graphic needs to be set for the 
        /// // AttachmentEditor.GraphicSource, irrespective of how it is obtained.
        ///   
        /// private void ListBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        /// {
        ///   // This function loops through all of the Graphics in the FeatureLayer and unselects them. Then
        ///   // for the 'objectid' value that was selected in the ListBox, select it to show visually on the
        ///   // Map and set the AttachmentEditor.GraphicSource Property.
        ///   
        ///   // Get the FeatureLayer from the XAML.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = (ESRI.ArcGIS.Client.FeatureLayer)MyMap.Layers["IncidentsLayer"];
        ///   
        ///   // Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   for (int i = 0; i &lt; myFeatureLayer.SelectionCount; i++)
        ///   {
        ///     myFeatureLayer.SelectedGraphics.ToList()[i].UnSelect();
        ///   }
        ///   
        ///   // Obtain the GraphicsCollection from the FeatureLayer.
        ///   ESRI.ArcGIS.Client.GraphicCollection myGraphicCollection = myFeatureLayer.Graphics;
        ///   
        ///   // Another way to loop through all of the Graphics. 
        ///   foreach (ESRI.ArcGIS.Client.Graphic myGraphic in myGraphicCollection)
        ///   {
        ///     // This time find the specific Graphic that has a matching 'objectid' value as chosen by 
        ///     // the user in the ListBox, select it for visual display in the Map and set the 
        ///     // AttachmentEditor.GraphicSource Property.
        ///     String myString = myGraphic.Attributes["objectid"].ToString();
        ///     if (myString == (ListBox1.SelectedItem.ToString()))
        ///     {
        ///       myGraphic.Select();
        ///       MyAttachmentEditor.GraphicSource = myGraphic;
        ///       break;
        ///     }
        ///   }
        /// }
        ///   
        /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function scans through all of the Graphics of the FeatureLayer in the current Map.Extent
        ///   // and uses an Action Delegate to perform a FeatureLayer.QueryAttachmentInfos Method call to list
        ///   // all of the Graphic 'objectid' values in a ListBox for with there are associated Attachments.
        ///   
        ///   // Clear out the ListBox for multiple runs
        ///   ListBox1.Items.Clear();
        ///   
        ///   // Get the FeatureLayer that was defined in XAML.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = (ESRI.ArcGIS.Client.FeatureLayer)MyMap.Layers["IncidentsLayer"];
        ///   
        ///   // Get the GraphicCollection from the FeatureLayer
        ///   ESRI.ArcGIS.Client.GraphicCollection myGraphicCollection = myFeatureLayer.Graphics;
        ///   
        ///   // Loop through each Graphic in the GraphicCollection
        ///   foreach (ESRI.ArcGIS.Client.Graphic myGraphic in myGraphicCollection)
        ///   {
        ///     // Define the Action Delegate
        ///     Action&lt;System.Collections.Generic.IEnumerable&lt;ESRI.ArcGIS.Client.FeatureService.AttachmentInfo&gt;&gt; myAction = null;
        ///     myAction = PopulateTheListBoxWith_objectid_Values;
        ///     
        ///     // Use the FeatureLayer.QueryAttachmentInfos Method to call the PopulateTheListBoxWith_objectid_Values
        ///     // Function.
        ///     // SPECIAL NOTE: 
        ///     // By using an Action Delegate, the processing of the request does not occur until this Button1_Click
        ///     // Function has completely finished processing (this means the For/Each loop will completely process 
        ///     // BEFORE any calls to the PopulateTheListBoxWith_objectid_Values Function begin). Set some breakpoints
        ///     // to see how this operation occurs. As a result of the 'queuing up' of all the 
        ///     // PopulateTheListBoxWith_objectid_Values Function calls, there is no way to know exactly when the 
        ///     // PopulateTheListBoxWith_objectid_Values Function calls are going to finish processing since they 
        ///     // do not fire sequentially from within the For/Each loop. 
        ///     myFeatureLayer.QueryAttachmentInfos(myGraphic, myAction, null);
        ///   }
        /// }
        /// 
        /// public void PopulateTheListBoxWith_objectid_Values(System.Collections.Generic.IEnumerable&lt;ESRI.ArcGIS.Client.FeatureService.AttachmentInfo&gt; myAttachmentsInfos)
        /// {
        ///   // This function gets called as a result of an Action Delegate. An 
        ///   // IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo) objects are available to be 
        ///   // interrogated. If there is a valid AttachmentInfo object use the .Split String operation on
        ///   // the AttachmentInfo.Uri Property to extract out the 'objectid' value of the Graphic in the 
        ///   // FeatureLayer. For more information on the specific Uri structure see the ArcGIS Server REST 
        ///   // API document called "Attachment - Feature Service":
        ///   // http://sampleserver3.arcgisonline.com/ArcGIS/SDK/REST/fsattachment.html
        ///   // Once the 'objectid' value is obtained, add it to the ListBox so that users can click on it
        ///   // to highlight the Graphic feature in the Map and set the AttachmentEditor.GraphicSource Property.
        ///   
        ///   // Example of information of an AttachmentsInfo object:
        ///   // ContentType = "image/jpeg"
        ///   // Delete = {ESRI.ArCGIS.Client.DelegateCommand}
        ///   // ID = 467
        ///   // Name = "Chrysanthemum.jpg"
        ///   // Size = 87396
        ///   // Uri = {http://sampleserver3/arcgisonline/com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0/121979/attachments/467}
        ///   
        ///   // Loop through the IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo) objects
        ///   foreach (ESRI.ArcGIS.Client.FeatureService.AttachmentInfo oneAttchmentInfo in myAttachmentsInfos)
        ///   {
        ///     // Tokenize the AttachmentInfo.Uri value into an array of String objects.
        ///     string[] parts = oneAttchmentInfo.Uri.ToString().Split('/');
        ///     
        ///     // Get the 'objectid' value for a Graphic Feature from the AttachmentInfo.Uri.
        ///     string my_objectid = parts[parts.Length - 3].ToString();
        ///     
        ///     // Only add unique occurrences of the 'objectid' value to the ListBox. There could me
        ///     // multiple attachments per Graphic Feature.
        ///     if (!(ListBox1.Items.Contains(my_objectid)))
        ///     {
        ///       ListBox1.Items.Add(my_objectid);
        ///     }
        ///   }
        /// }
        /// 
        /// private void FeatureLayer_MouseLeftButtonUp(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
        /// {
        ///   // This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   // un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   // it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   // downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   // Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = sender as ESRI.ArcGIS.Client.FeatureLayer;
        ///   
        ///   // Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   for (int i = 0; i &lt; myFeatureLayer.SelectionCount; i++)
        ///   {
        ///     myFeatureLayer.SelectedGraphics.ToList()[i].UnSelect();
        ///   }
        ///   
        ///   // Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select();
        ///   
        ///   // Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   // to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// ' The purpose of this example code is to show that the AttachmentEditor.GraphicSource can be set a few
        /// ' different ways: 
        /// ' (1) via user interaction with the Map 
        /// ' (2) via user interaction with non-Map controls
        /// ' The important thing to learn is that a valid Graphic needs to be set for the 
        /// ' AttachmentEditor.GraphicSource, irrespective of how it is obtained.
        /// 
        /// Private Sub ListBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        ///   
        ///   ' This function loops through all of the Graphics in the FeatureLayer and unselects them. Then
        ///   ' for the 'objectid' value that was selected in the ListBox, select it to show visually on the
        ///   ' Map and set the AttachmentEditor.GraphicSource Property.
        ///   
        ///   ' Get the FeatureLayer from the XAML.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = MyMap.Layers("IncidentsLayer")
        ///   
        ///   ' Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   For i As Integer = 0 To myFeatureLayer.SelectionCount - 1
        ///     myFeatureLayer.SelectedGraphics.ToList()(i).UnSelect()
        ///   Next i
        ///   
        ///   ' Obtain the GraphicsCollection from the FeatureLayer.
        ///   Dim myGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = myFeatureLayer.Graphics
        ///   
        ///   ' Another way to loop through all of the Graphics. 
        ///   For Each myGraphic As ESRI.ArcGIS.Client.Graphic In myGraphicCollection
        ///     
        ///     ' This time find the specific Graphic that has a matching 'objectid' value as chosen by 
        ///     ' the user in the ListBox, select it for visual display in the Map and set the 
        ///     ' AttachmentEditor.GraphicSource Property.
        ///     If myGraphic.Attributes.Item("objectid") = ListBox1.SelectedItem.ToString Then
        ///       myGraphic.Select()
        ///       MyAttachmentEditor.GraphicSource = myGraphic
        ///       Exit For
        ///     End If
        ///   Next
        ///   
        /// End Sub
        /// 
        /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function scans through all of the Graphics of the FeatureLayer in the current Map.Extent
        ///   ' and uses an Action Delegate to perform a FeatureLayer.QueryAttachmentInfos Method call to list
        ///   ' all of the Graphic 'objectid' values in a ListBox for with there are associated Attachments.
        ///   
        ///   ' Clear out the ListBox for multiple runs
        ///   ListBox1.Items.Clear()
        ///   
        ///   ' Get the FeatureLayer that was defined in XAML.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = MyMap.Layers("IncidentsLayer")
        ///   
        ///   ' Get the GraphicCollection from the FeatureLayer
        ///   Dim myGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = myFeatureLayer.Graphics
        ///   
        ///   ' Loop through each Graphic in the GraphicCollection
        ///   For Each myGraphic As ESRI.ArcGIS.Client.Graphic In myGraphicCollection
        ///     
        ///     ' Define the Action Delegate
        ///     Dim myAction As Action(Of System.Collections.Generic.IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo))
        ///     myAction = AddressOf PopulateTheListBoxWith_objectid_Values
        ///     
        ///     ' Use the FeatureLayer.QueryAttachmentInfos Method to call the PopulateTheListBoxWith_objectid_Values
        ///     ' Function.
        ///     ' SPECIAL NOTE: 
        ///     ' By using an Action Delegate, the processing of the request does not occur until this Button1_Click
        ///     ' Function has completely finished processing (this means the For/Each loop will completely process 
        ///     ' BEFORE any calls to the PopulateTheListBoxWith_objectid_Values Function begin). Set some breakpoints
        ///     ' to see how this operation occurs. As a result of the 'queuing up' of all the 
        ///     ' PopulateTheListBoxWith_objectid_Values Function calls, there is no way to know exactly when the 
        ///     ' PopulateTheListBoxWith_objectid_Values Functoion calls are going to finish processing since they 
        ///     ' do not fire sequentially from within the For/Each loop. 
        ///     myFeatureLayer.QueryAttachmentInfos(myGraphic, myAction, Nothing)
        ///     
        ///   Next
        ///   
        /// End Sub
        /// 
        /// Public Sub PopulateTheListBoxWith_objectid_Values(ByVal myAttachmentsInfos As System.Collections.Generic.IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo))
        ///   
        ///   ' This function gets called as a result of an Action Delegate. An 
        ///   ' IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo) objects are available to be 
        ///   ' interrogated. If there is a valid AttachmentInfo object use the .Split String operation on
        ///   ' the AttachmentInfo.Uri Property to extract out the 'objectid' value of the Graphic in the 
        ///   ' FeatureLayer. For more information on the specific Uri structure see the ArcGIS Server REST 
        ///   ' API document called "Attachment - Feature Service":
        ///   ' http://sampleserver3.arcgisonline.com/ArcGIS/SDK/REST/fsattachment.html
        ///   ' Once the 'objectid' value is obtained, add it to the ListBox so that users can click on it
        ///   ' to highlight the Graphic feature in the Map and set the AttachmentEditor.GraphicSource Property.
        ///   
        ///   ' Example of information of an AttachmentsInfo object:
        ///   ' ContentType = "image/jpeg"
        ///   ' Delete = {ESRI.ArCGIS.Client.DelegateCommand}
        ///   ' ID = 467
        ///   ' Name = "Chrysanthemum.jpg"
        ///   ' Size = 87396
        ///   ' Uri = {http://sampleserver3/arcgisonline/com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0/121979/attachments/467}
        ///   
        ///   ' Loop through the IEnumerable(Of ESRI.ArcGIS.Client.FeatureService.AttachmentInfo) objects
        ///   Dim oneAttchmentInfo As ESRI.ArcGIS.Client.FeatureService.AttachmentInfo
        ///   For Each oneAttchmentInfo In myAttachmentsInfos
        ///     
        ///     ' Tokenize the AttachmentInfo.Uri value into an array of String objects.
        ///     Dim parts As String() = oneAttchmentInfo.Uri.ToString.Split("/")
        ///     
        ///     ' Get the 'objectid' value for a Graphic Feature from the AttachmentInfo.Uri.
        ///     Dim my_objectid As String = parts(parts.Length - 3).ToString
        ///     
        ///     ' Only add unique occurrences of the 'objectid' value to the ListBox. There could me
        ///     ' multiple attachments per Graphic Feature.
        ///     If Not ListBox1.Items.Contains(my_objectid) Then
        ///       ListBox1.Items.Add(my_objectid)
        ///     End If
        ///   Next
        ///   
        /// End Sub
        /// 
        /// Private Sub FeatureLayer_MouseLeftButtonUp(ByVal sender As System.Object, ByVal e As ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs)
        ///   
        ///   ' This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   ' un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   ' it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   ' downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   ' Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = TryCast(sender, ESRI.ArcGIS.Client.FeatureLayer)
        ///   
        ///   ' Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   For i As Integer = 0 To myFeatureLayer.SelectionCount - 1
        ///     myFeatureLayer.SelectedGraphics.ToList()(i).UnSelect()
        ///   Next i
        ///   
        ///   ' Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select()
        /// 
        ///   ' Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   ' to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic
        /// 
        /// End Sub
        /// </code>
        /// </example>
        public Graphic GraphicSource
        {
            get { return (Graphic)GetValue(GraphicSourceProperty); }
            set { SetValue(GraphicSourceProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="GraphicSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GraphicSourceProperty =
            DependencyProperty.Register("GraphicSource", typeof(Graphic), typeof(AttachmentEditor), new PropertyMetadata(null, OnGraphicSourcePropertyChanged));
        private static void OnGraphicSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AttachmentEditor AttachmentEditor = d as AttachmentEditor;
            AttachmentEditor.LoadAttachments();    
        }

        /// <summary> 
        /// Gets or sets the 
        /// <a href="http://msdn.microsoft.com/en-us/library/system.windows.datatemplate(v=VS.95).aspx" target="_blank">DataTemplate</a> 
        /// used for the ListBox sub-component UI Element of the AttachmentEditor control. The ListBox sub-component UI 
        /// Element typically has implicit Binding to the Properties of the 
        /// <see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo">ESRI.ArcGIS.Client.FeatureService.AttachmentInfo</see> 
        /// Class to control its behavior. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Changing the core default behavior and appearance of the ListBox sub-component of the AttachmentEditor Control 
        /// can be modified using a DataTemplate via the AttachmentEditor.ItemTemplate Property and writing your own custom 
        /// code when necessary. When defining the DataTemplate, the Binding Properties that developers usually want to use 
        /// to control the behavior of the ListBox sub-component of the AttachmentEditor are based upon those in the 
        /// ESRI.ArcGIS.Client.FeatureService.AttachmentInfo Class.
        /// </para>
        /// <para>
        /// If only the superficial appearance of the AttachmentEditor Control is desired to be modified (i.e. Height, 
        /// Width, Background, FontSize, etc.) consider using the numerous inherited Properties from 
        /// System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. 
        /// </para>
        /// <para>
        /// Note: you cannot change the core behavior of the sub-components (i.e. Button, ListBox, etc.) of the 
        /// AttachmentEditor Control using standard Properties or Methods alone. To change the core behavior of the 
        /// sub-components and their appearance of the Control, developers can modify the Control Template in XAML and the 
        /// associated code-behind file. The easiest way to modify the UI sub-components is using Microsoft Expression 
        /// Blend. Then developers can delete/modify existing or add new sub-components in Visual Studio to create a truly 
        /// customized experience. A general approach to customizing a Control Template is discussed in the ESRI blog 
        /// entitled: 
        /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>.
        /// </para>
        /// <para>
        /// The AttachmentEditor is essentially made of two core parts: an Add button and a ListBox that displays 
        /// information about attachments associated with a Graphic element of a FeatureLayer. The attachments are stored 
        /// in the backend database of a FeatureLayer on an ArcGIS Server FeatureServer. The default AttachmentEditor will 
        /// display a hyperlink and a delete button (via a red X) for each attachment associated with Graphic feature in 
        /// the FeatureLayer as a ListBox item. See the following screen shot that depicts the two core parts of the 
        /// AttachmentEditor Control.
        /// </para>
        /// <img border="0" alt="Core functionality parts of the Attachment Editor." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor.ItemTemplate.png"/>
        /// <para>
        /// The power of using a DataTemplate via XAML to control the behavior and appearance the ListBox sub-component of 
        /// the AttachmentEditor is great but comes at the price of understanding the complexity of Binding and having an 
        /// artistic flair to make a nice UI design. Developers can control the contents of the ListBox sub-component UI 
        /// Element via a Datatemplate of the AttachmentEditor using the AttachmentEditor.ItemTempalte Property. Developers 
        /// cannot control the Add button of the AttachmentEditor using a DataTemplate. In most circumstances the ListBox 
        /// sub-component UI Element that is customized in the DataTemplate will have Binding to the AttachmentEditors 
        /// internal usage of the ESRI.ArcGIS.Client.FeatureService.AttachmentInfo Class Properties. There are six 
        /// AttachmentInfo Properties:
        /// <list type="bullet">
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.ContentType">ContentType</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.Delete">Delete</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.ID">ID</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.Name">Name</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.Size">Size</see></item>
        ///   <item><see cref="ESRI.ArcGIS.Client.FeatureService.AttachmentInfo.Uri">Uri</see></item>
        /// </list>
        /// The Binding syntax for the ListBox sub-component UI Element of the AttachmentEditor typically follows the 
        /// pattern of:
        /// <code>
        /// "{Binding &lt;PropertyName&gt;}"
        /// </code> 
        /// Where &lt;PropertyName&gt; is the name of a FeatureService.AttachmentInfo Property (ex: Name or Uri). Note 
        /// when no source is specified for the Binding, it is implicitly implied that it is the FeatureService.AttachmentInfo 
        /// Class. If developers desire to use 
        /// <a href="http://msdn.microsoft.com/en-us/library/cc278072(v=VS.95).aspx" target="_blank">Data Binding</a> to 
        /// other objects they will be responsible for specifying the correct binding source. The code example in this 
        /// document provides an example of Binding the ListBox sub-component UI Element of the AttachmentEditor to both 
        /// FeatureService.AttachmentInfo Properties and user defined static sources.
        /// </para>
        /// <para>
        /// The FeatureService.AttachmentInfo.Delete Property is a unique in that it is not a simple data Type (or does 
        /// not easily translate to a simple data Type via an internal converter). The FeatureService.AttachmentInfo.Delete 
        /// Property returns a 
        /// <a href="http://msdn.microsoft.com/en-us/library/ms616869(v=VS.95).aspx" target="_blank">System.Windows.Input.ICommand</a> 
        /// Object. The trick to Binding to this Property is to Bind it to the 
        /// <a href="http://msdn.microsoft.com/en-us/library/system.windows.controls.primitives.buttonbase.command(v=VS.95).aspx" target="_blank">ButtonBase.Command</a> 
        /// Property of the various UI Elements (ex: Button, HyperlinkButton, RepeatButton, ToggleButton, etc) to the 
        /// desired UI Element. The code example in this document demonstrates Binding a simple Button’s Command Property 
        /// to the FeatureService.AttachmentInfo.Delete Property to delete attachments from the AttachmentEditor and the 
        /// FeatureLayers FeatureService backend database. 
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// When the Map loads click on a point feature in the FeatureLayer to select the Graphic. Then use the 
        /// AttachmentEditor to add, delete, and download files. Hover the mouse cursor over the attachment image to 
        /// display a Microsoft ToolTip containing metadata about the attachment.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating an AttachmentEditor that has been customized via a DataTemplate." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.AttachmentEditor.ItemTemplate1.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!--
        ///   The Grid.Resources section is a great place to define a DataTemplate used by the AttachmentEditor
        ///   Control. The x:Key is needed to reference the DataTemplate in the AttachmentEditor.ItemTemplate
        ///   Property. SPECIAL NOTE: The binding that occurs to the UI Elements in the DataTemplate use 
        ///   Properties from the ESRI.ArcGIS.Client.FeatureService.AttachmentInfo Class (additional comments
        ///   will be added to point out which Properties are used).
        ///   --&gt;
        ///   &lt;Grid.Resources&gt;
        ///     
        ///     &lt;!--
        ///     Define a simple StaticResource that will be displayed as a visual UI Element of the 
        ///     ListBox portion of the AttachmentEditor. This will help illustrate one way to perform
        ///     Binding. You will need to add the following XML namespace to use 'sys' variables:
        ///     xmlns:sys="clr-namespace:System;assembly=mscorlib"
        ///     --&gt;
        ///     &lt;sys:String x:Key="SingleString"&gt;Visual appearance of the attachment:&lt;/sys:String&gt;
        /// 
        ///     &lt;!-- Define a DataTemplate and use the x:Key to reference it in other parts of XAML. --&gt;
        ///     &lt;DataTemplate x:Key="AEItemTemplate"&gt;
        ///       &lt;Grid Margin="3"&gt;
        ///         &lt;StackPanel&gt;
        ///         
        ///           &lt;!--
        ///           Create a TextBlock that informs the user they are seeing a visual depiction of the
        ///           attachment associated with the Graphic in the FeatureLayer. Note how the 
        ///           Binding.Source Property needs to be set in order to use a custom object. This 
        ///           syntax is quite different from the other Bindings that occur in which it is
        ///           automatically implied in the Binding that we are accessing a Property of the
        ///           FeatureService.AttachmentInfo Class.
        ///           --&gt;
        ///           &lt;TextBlock Margin="0,0" Text="{Binding Source={StaticResource SingleString}}" /&gt;
        ///             
        ///           &lt;StackPanel Orientation="Horizontal"&gt;
        ///             
        ///             &lt;!-- 
        ///             Attachments that are images will be displayed in the ListBox of the AttachmentEditor. 
        ///             Note: The Image.Source Property use Binding to the AttachmentInfo.Uri Property. 
        ///             --&gt;
        ///             &lt;Image Source="{Binding Uri}" MaxWidth="75" MaxHeight="75" Stretch="Uniform"&gt;
        ///               
        ///               &lt;!--
        ///               A Microsoft ToolTip Control will be used such that when the user hovers the Mouse
        ///               Cursor over the image, additional metadata information from the AttachmentInfo Class
        ///               associated internally with the AttachmentEditor will be displayed.
        ///               --&gt;
        ///               &lt;ToolTipService.ToolTip&gt;
        ///                 &lt;Grid&gt;
        ///                   &lt;Grid.RowDefinitions&gt;
        ///                     &lt;RowDefinition Height="Auto" /&gt;
        ///                     &lt;RowDefinition Height="Auto" /&gt;
        ///                     &lt;RowDefinition Height="Auto" /&gt;
        ///                     &lt;RowDefinition Height="Auto" /&gt;
        ///                   &lt;/Grid.RowDefinitions&gt;
        ///                   &lt;Grid.ColumnDefinitions&gt;
        ///                     &lt;ColumnDefinition Width="Auto" /&gt;
        ///                     &lt;ColumnDefinition Width="Auto" /&gt;
        ///                   &lt;/Grid.ColumnDefinitions&gt;
        ///                   
        ///                   &lt;!-- 
        ///                   Note: The TextBlock.Text Property use Binding to the AttachmentInfo.Name Property to
        ///                   enhance the metadata information about the image attachment displayed in the ToolTip. 
        ///                   --&gt;
        ///                   &lt;TextBlock Text="Name: " Margin="10,0,0,0" 
        ///                              HorizontalAlignment="Right" VerticalAlignment="Center" 
        ///                              Grid.Row="0" Grid.Column="0" /&gt;
        ///                   &lt;TextBlock Text="{Binding Name}" Margin="5,0" MaxWidth="250" TextWrapping="Wrap" 
        ///                              Grid.Row="0" Grid.Column="1" VerticalAlignment="Center" /&gt;
        ///                   
        ///                   &lt;!-- 
        ///                   Note: The TextBlock.Text Property use Binding to the AttachmentInfo.Size Property to
        ///                   enhance the metadata information about the image attachment displayed in the ToolTip. 
        ///                   --&gt;
        ///                   &lt;TextBlock Text="Size: " Margin="10,0,0,0" 
        ///                              HorizontalAlignment="Right" 
        ///                              Grid.Row="1" Grid.Column="0" /&gt;
        ///                   &lt;TextBlock Text="{Binding Size}" Margin="5,0" MaxWidth="250" 
        ///                              TextWrapping="Wrap" Grid.Row="1" Grid.Column="1" /&gt;
        ///           
        ///                   &lt;!-- 
        ///                   Note: The TextBlock.Text Property use Binding to the AttachmentInfo.Uri Property to
        ///                   enhance the metadata information about the image attachment displayed in the ToolTip. 
        ///                   --&gt;
        ///                   &lt;TextBlock Text="Uri: " Margin="10,0,0,0" 
        ///                              HorizontalAlignment="Right" 
        ///                              Grid.Row="2" Grid.Column="0" /&gt;
        ///                   &lt;TextBlock Text="{Binding Uri}" Margin="5,0" MaxWidth="250" 
        ///                              TextWrapping="Wrap" Grid.Row="2" Grid.Column="1" /&gt;
        ///         
        ///                   &lt;!-- 
        ///                   Note: The TextBlock.Text Property use Binding to the AttachmentInfo.ContentType Property to
        ///                   enhance the metadata information about the image attachment displayed in the ToolTip. 
        ///                   --&gt;
        ///                   &lt;TextBlock Text="Content Type: " Margin="10,0,0,0" 
        ///                              HorizontalAlignment="Right" 
        ///                              Grid.Row="3" Grid.Column="0" /&gt;
        ///                   &lt;TextBlock Text="{Binding ContentType}" Margin="5,0" MaxWidth="250" 
        ///                              TextWrapping="Wrap" Grid.Row="3" Grid.Column="1" /&gt;
        ///                 &lt;/Grid&gt;
        ///               &lt;/ToolTipService.ToolTip&gt;
        ///             &lt;/Image&gt;
        ///           &lt;/StackPanel&gt;
        ///           
        ///           &lt;StackPanel Orientation="Horizontal"&gt;
        ///           
        ///             &lt;!-- 
        ///             Display the filename of the image attachment just below the picture of the image in 
        ///             the ListBox of the AttachmentEditor. Note: The Hyperlink.Content uses Binding to the 
        ///             AttachmentInfo.Name Property and the Hyperlink.NavigateUri uses Binding to the 
        ///             AttachmentInfo.Uri Property. 
        ///             --&gt;
        ///             &lt;HyperlinkButton Content="{Binding Name}" NavigateUri="{Binding Uri}" TargetName="_blank" 
        ///                              HorizontalAlignment="Center" VerticalAlignment="Center" Margin="0,3,0,0" 
        ///                              FontFamily="Arial Black" FontSize="10" FontStyle="Italic" /&gt;
        ///               
        ///             &lt;!--
        ///             Provide the ability to Delete an image attachment from the AttachmentEditor and the 
        ///             FeatureLayer FeatureServer. It is the Button.Command Property that will have Binding
        ///             to the AttachmentInfo.Delete Property (which is a Type of ICommand).
        ///             --&gt;
        ///             &lt;Button Content="Delete" Command="{Binding Delete}" Width="60" Margin="5,5,5,5" 
        ///                             HorizontalAlignment="Center" VerticalAlignment="Center"/&gt;
        ///                         
        ///           &lt;/StackPanel&gt;
        ///           &lt;Border BorderThickness="1" BorderBrush="Black"&gt;&lt;/Border&gt;
        ///         &lt;/StackPanel&gt;
        ///       &lt;/Grid&gt;
        ///     &lt;/DataTemplate&gt;
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="64" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///              Width="512" TextWrapping="Wrap" Margin="12,12,0,0" 
        ///              Text="When the Map loads click on a point feature in the FeatureLayer to select
        ///                   the Graphic. Then use the AttachmentEditor to add, delete, and download files.
        ///                   Hover the mouse cursor over the attachment image to display a Microsoft
        ///                   ToolTip containing metadata about the attachment." /&gt;
        ///     
        ///   &lt;!-- 
        ///   Add a Map control with an ArcGISTiledMapeServiceLayer and a FeatureLayer. It is important to 
        ///   provide a Name for the Map Control as this will be used by the AttachmentEditor Control.
        ///   Define and initial Extent for the map.
        ///   --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,100,0,0" Name="MyMap" 
        ///             VerticalAlignment="Top" Height="375" Width="400" 
        ///             Extent="-13625087,4547888,-13623976,4548643"&gt;
        ///     
        ///     &lt;!-- Add a backdrop ArcGISTiledMapServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer 
        ///               Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///     
        ///     &lt;!-- 
        ///     The FeatureLayer is based upon an ArcGIS Server 'FeatureServer' service not a 'MapServer' service.
        ///     It is important to provide an ID value as this will be used for Binding in the AttachementEditor.
        ///     The MouseLeftButtonUp Event handler has been defined for the FeatureLayer. 
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="IncidentsLayer"
        ///            Url="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"
        ///            MouseLeftButtonUp="FeatureLayer_MouseLeftButtonUp" AutoSave="False"                  
        ///            DisableClientCaching="True" OutFields="*" Mode="OnDemand" /&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Wrap the Attachment Editor inside of a Grid and other controls to provide a stylistic look 
        ///   and to provide instructions to the user.
        ///   --&gt;
        ///   &lt;Grid HorizontalAlignment="Left" VerticalAlignment="Top" Margin="450,100,15,0" &gt;
        ///     &lt;Rectangle Fill="BurlyWood" Stroke="Green"  RadiusX="15" RadiusY="15" Margin="0,0,0,5" &gt;
        ///       &lt;Rectangle.Effect&gt;
        ///         &lt;DropShadowEffect/&gt;
        ///       &lt;/Rectangle.Effect&gt;
        ///     &lt;/Rectangle&gt;
        ///     &lt;Rectangle Fill="#FFFFFFFF" Stroke="Aqua" RadiusX="5" RadiusY="5" Margin="10,10,10,15" /&gt;
        ///     &lt;StackPanel Orientation="Vertical" HorizontalAlignment="Left"&gt;
        ///       &lt;TextBlock Text="Click on a point feature to select it, and click the 'Add' button to attach a file." 
        ///                      Width="275" TextAlignment="Left" Margin="20,20,20,5" TextWrapping="Wrap" FontWeight="Bold"/&gt;
        ///       
        ///       &lt;!--
        ///       Add an AttachmentEditor. Provide a x:Name for the AttachmentEditor so that it can be used in the 
        ///       code-behind file. The FeatureLayer Property is bound to the 'IncidentLayer' of the Map Control.
        ///       Only one group of Filter types are used (you can have multiple file extensions in a group) to drive the
        ///       OpenFileDialog. This AttachmentEditor will only allow the uploading of Image files. This is necessary 
        ///       because as part of the DataTemplate that will control the appearance and core behavior, the actual 
        ///       images will be displayed. The FilterIndex shows the first group of Filter Types in the OpenFileDialog 
        ///       dropdown list. Only allow the user to upload (i.e. Add) one file at a time using the OpenFileDialog
        ///       with the Multiselect set to False. An UploadFailed Event has been defined for the code-behind.
        ///       The appearance and core behavior of the ListBox sub-component of the AttachmentEditor is controlled 
        ///       by defining a DataTemplate (see above) and using it for the ItemTemplate Property. SPECIAL NOTE: The 
        ///       binding that occurs to the UI Elements in the DataTemplate use Properties from the 
        ///       ESRI.ArcGIS.Client.FeatureService.AttachmentInfo Class.
        ///       --&gt;
        ///       &lt;esri:AttachmentEditor x:Name="MyAttachmentEditor" VerticalAlignment="Top" Margin="20,5,20,20" 
        ///                            Background="LightGoldenrodYellow" Width="280" Height="300" HorizontalAlignment="Right"                              
        ///                            FeatureLayer="{Binding Layers[IncidentsLayer], ElementName=MyMap}" 
        ///                            Filter="Image Files|*.jpg;*.gif;*.png;*.bmp" 
        ///                            FilterIndex="1" Multiselect="False"
        ///                            UploadFailed="MyAttachmentEditor_UploadFailed"
        ///                            ItemTemplate="{StaticResource AEItemTemplate}"&gt;
        ///       &lt;/esri:AttachmentEditor&gt;
        ///     &lt;/StackPanel&gt;
        ///   &lt;/Grid&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void FeatureLayer_MouseLeftButtonUp(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
        /// {
        ///   // This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   // un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   // it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   // downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   // Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = sender as ESRI.ArcGIS.Client.FeatureLayer;
        ///   
        ///   // Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   for (int i = 0; i &lt; myFeatureLayer.SelectionCount; i++)
        ///   {
        ///     myFeatureLayer.SelectedGraphics.ToList()[i].UnSelect();
        ///   }
        ///   
        ///   // Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select();
        ///   
        ///   // Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   // to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic;
        /// }
        /// 
        /// private void MyAttachmentEditor_UploadFailed(object sender, ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.UploadFailedEventArgs e)
        /// {
        ///   // Display any error messages that may occur if the AttachmentEditor fails to upload a file.
        ///   MessageBox.Show(e.Result.Message);
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub FeatureLayer_MouseLeftButtonUp(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs)
        ///   
        ///   ' This function obtains the FeatureLayer, loops through all of the Graphic features and
        ///   ' un-selects them. Then for the Graphic for which the user clicked with the mouse select
        ///   ' it and use that Graphic to set the AttachmentEditor.GraphicSource to enable uploading,
        ///   ' downloading, and deleting attachment files on the ArcGIS Server backend database.
        ///   
        ///   ' Get the FeatureLayer from the sender object of the FeatureLayer.MouseLeftButtonUp Event.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = TryCast(sender, ESRI.ArcGIS.Client.FeatureLayer)
        ///   
        ///   ' Loop through all of the Graphics in the FeatureLayer and UnSelect them.
        ///   For i As Integer = 0 To myFeatureLayer.SelectionCount - 1
        ///     myFeatureLayer.SelectedGraphics.ToList()(i).UnSelect()
        ///   Next i
        ///   
        ///   ' Select the Graphic from the e object which will change visibly on the Map via highlighting.
        ///   e.Graphic.Select()
        ///   
        ///   ' Setting the AttachmentEditor.GraphicSource immediately causes the AttachmentEditor Control 
        ///   ' to update with any attachments currently associated with the Graphic.
        ///   MyAttachmentEditor.GraphicSource = e.Graphic
        ///   
        /// End Sub
        ///   
        /// Private Sub MyAttachmentEditor_UploadFailed(ByVal sender As Object, ByVal e As ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.UploadFailedEventArgs)
        ///   ' Display any error messages that may occur if the AttachmentEditor fails to upload a file.
        ///   MessageBox.Show(e.Result.Message)
        /// End Sub
        /// </code>
        /// </example>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AttachmentEditor), null);
        #endregion

        #region Overridden Methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes 
        /// (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. 
        /// In simplest terms, this means the method is called just before a UI element displays in an application. 
        /// For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._addNewButton != null)
                this._addNewButton.Click -= AddNewButton_Click;
            this._addNewButton = GetTemplateChild("AddNewButton") as ButtonBase;
            if (this._addNewButton != null)
                this._addNewButton.Click += AddNewButton_Click;

            this._attachmentList = GetTemplateChild("AttachmentList") as ItemsControl;
            if (this._attachmentList != null && System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                LoadAttachments();
			// Forcing the AttachmentEditor to update its "Add" button enable state in cases where it has been added as 
			// the content of a ChildWindow control:
			UpdateStates();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a filter string that specifies the file types and descriptions
        /// to display in the System.Windows.Controls.OpenFileDialog.
        /// </summary>
        /// <value>
        /// A filter string that specifies the file types and descriptions to display in the OpenFileDialog.
        /// The default is <see cref="System.String.Empty"/>.
        /// </value>
        /// <exception cref="System.ArgumentException">
        /// The filter string does not contain at least one vertical bar (|).
        /// </exception>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item in the System.Windows.Controls.OpenFileDialog 
        /// filter drop-down list.
        /// </summary>
        /// <value>
        /// The index of the selected item in the System.Windows.Controls.OpenFileDialog 
        /// filter drop-down list. The default is 1.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The filter index is less than 1.
        /// </exception>
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the System.Windows.Controls.OpenFileDialog 
        /// allows users to select multiple files.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple selections are allowed; otherwise, <c>false</c>. The default is 
        /// </value>
        public bool Multiselect { get; set; }
        #endregion

        #region Private Methods
        private void FeatureLayer_Initialized(object sender, EventArgs e)
        {
            this.LoadAttachments();
        }

        private bool CheckGraphicParent()
        {
            if (this.FeatureLayer != null && this.GraphicSource != null) 
            { 
                return this.FeatureLayer.Graphics.Contains(this.GraphicSource);
            }
            return false;
        }

        private void LoadAttachments()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this._attachmentList != null)
            {
                DesignTimeDataSource[] source = new DesignTimeDataSource[] {
                    new DesignTimeDataSource() { Name="TextFileAttachment.txt", ID="0", ContentType="text/txt", Size=255, Uri=new Uri("http://www.esri.com") },
                    new DesignTimeDataSource() { Name="ImageAttachment.jpg", ID="1", ContentType="image/jpeg", Size=65535, Uri=new Uri("http://www.esri.com") }
                };

                this._attachmentList.ItemsSource = source;
                return;
            }

            if (this.FeatureLayer == null || this.GraphicSource == null)
                PopulateAttachmentList(null);
            else
            {
                if (CheckGraphicParent())
                {
                    string oidField = GetGraphicSourceOIdValue();
                    if (oidField != null && !string.IsNullOrEmpty(oidField.Trim()))
                    {
                        this.FeatureLayer.QueryAttachmentInfos(oidField, (Action<IEnumerable<AttachmentInfo>>)PopulateAttachmentList,
                            (Action<Exception>)delegate(Exception ex) { VisualStateManager.GoToState(this, "Loaded", true); });
                        VisualStateManager.GoToState(this, "Busy", true);
                    }
                    else
                        PopulateAttachmentList(null);
                }
                else
                    PopulateAttachmentList(null);
            }

            UpdateStates();
        }

        private string GetGraphicSourceOIdValue()
        {
            if (this.FeatureLayer != null && this.FeatureLayer.LayerInfo != null)
            {
                string oidField = this.FeatureLayer.LayerInfo.ObjectIdField;
                if (oidField != null && !string.IsNullOrEmpty(oidField.Trim()) && 
                    this.GraphicSource != null && this.GraphicSource.Attributes.ContainsKey(oidField))
                    return this.GraphicSource.Attributes[oidField].ToString();
            }

            return null;
        }

        private void PopulateAttachmentList(IEnumerable<AttachmentInfo> attachmentInfoCollection)
        {
            VisualStateManager.GoToState(this, "Loaded", true);
            if (this._attachmentList != null)
                this._attachmentList.ItemsSource = attachmentInfoCollection;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FeatureLayer == null || this.GraphicSource == null)
                return;

            string oidField = GetGraphicSourceOIdValue();
            if (oidField != null && !string.IsNullOrEmpty(oidField.Trim()))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() {
                    Filter = this.Filter, 
                    FilterIndex = this.FilterIndex < 1 ? 1 : this.FilterIndex, 
                    Multiselect = this.Multiselect
                };
                if (openFileDialog.ShowDialog() == true)
                {
#if SILVERLIGHT
                    AddFiles(oidField, openFileDialog.Files);
#else
                    AddFiles(oidField, GetFileInfos(openFileDialog.FileNames));
#endif
                }
            }
        }

#if !SILVERLIGHT
        private IEnumerable<FileInfo> GetFileInfos(string[] fileNames)
        {
            List<FileInfo> fileInfos = new List<FileInfo>(fileNames.Length);
            foreach (string fileName in fileNames)
            {
                fileInfos.Add(new FileInfo(fileName));
            }

            return fileInfos;
        }
#endif

        private void AddFiles(string oid, IEnumerable<FileInfo> fileInfos)
        {
            List<FileInfo> fileList = new List<FileInfo>(fileInfos);
            BeginUploadEventArgs beginUploadEventArgs = new BeginUploadEventArgs(fileList);
            OnBeginUpload(beginUploadEventArgs);
            if (beginUploadEventArgs.Cancel)
                return;

            Queue<FileInfo> files = new Queue<FileInfo>(fileList);
            if (files.Count > 0)
            {
                VisualStateManager.GoToState(this, "Busy", true);

                FileInfo fileInfo = null;
                Stream stream = null;

                Action<AttachmentResult> add = null;
                Action<Exception> error = (Action<Exception>)delegate(Exception result)
                {
                    OnUploadFailed(new UploadFailedEventArgs(result));  // Raise UploadFailed error event
                    add(null);  // Upload next file
                };
                add = delegate(AttachmentResult result)
                {
                    if (stream != null)
                        stream.Dispose(); // Dispose previous attachment
                    if (result != null && !result.Success)
                    {
                        OnUploadFailed(new UploadFailedEventArgs(new Exception(Properties.Resources.AttachmentEditor_FileUploadFailed)));  // Raise UploadFailed error event
                    }   
                    if (files.Count > 0)
                    {
                        fileInfo = files.Dequeue();
                        stream = fileInfo.OpenRead();
                            this.FeatureLayer.AddAttachment(oid, stream, fileInfo.Name, add, error);
                    }
                    else
                    {
                        OnEndUpload(EventArgs.Empty);
                        LoadAttachments();
                    }
                };
                add(null);
            }
        }

        private void AttachmentEditor_Drop(object sender, DragEventArgs e)
        {
            if (FeatureLayer == null || GraphicSource == null) { e.Handled = true; return; }

            if (e.Data != null)
            {
                string oidField = GetGraphicSourceOIdValue();
                if (oidField != null && !string.IsNullOrEmpty(oidField.Trim()))
                {
#if SILVERLIGHT
                    AddFiles(oidField, e.Data.GetData(DataFormats.FileDrop) as FileInfo[]);
#else
                    AddFiles(oidField, GetFileInfos(e.Data.GetData(DataFormats.FileDrop) as string[]));
#endif
                }
            }
        }

        private void AttachmentEditor_DragOver(object sender, DragEventArgs e)
        {
            if (FeatureLayer == null || GraphicSource == null)
                e.Handled = true;
        }

        private void UpdateStates()
        {
            if (this._addNewButton != null)
            {
                if (this.FeatureLayer != null && FeatureLayer.LayerInfo != null && CheckGraphicParent())
                {
					this._addNewButton.IsEnabled = this.AllowDrop = this.FeatureLayer.LayerInfo.HasAttachments 
						&& this.FeatureLayer.IsAddAttachmentAllowed(this.GraphicSource);					
                }
                else
                    this._addNewButton.IsEnabled = false;
            }
        }
        #endregion

        #region Events
        #region UploadFailed Event
        /// <summary>
        /// Occurs when uploading an attachment to the server failed.
        /// </summary>
        public event EventHandler<UploadFailedEventArgs> UploadFailed;
        /// <summary>
        /// Event argument for the UploadFailed event.
        /// </summary>
        public sealed class UploadFailedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the result of upload failure.
            /// </summary>
            /// <value>The result.</value>
            public Exception Result { get; private set; }
            internal UploadFailedEventArgs(Exception result) { Result = result; }
        }
        private void OnUploadFailed(UploadFailedEventArgs e)
        {
            if (UploadFailed != null)
                UploadFailed(this, e);
        }
        #endregion

        #region EndUpload Event
        /// <summary>
        /// Occurs when uploading of an attachment to the server ends.
        /// </summary>
        public event EventHandler EndUpload;
       
        private void OnEndUpload(EventArgs e)
        {
            if (EndUpload != null)
                EndUpload(this, e);
        }
        #endregion

        #region BeginUpload Event
        /// <summary>
        /// Occurs when uploading of an attachment begins.
        /// </summary>
        public event EventHandler<BeginUploadEventArgs> BeginUpload;
        /// <summary>
        /// Event argument for the BeginUpload event.
        /// </summary>
        public sealed class BeginUploadEventArgs : EventArgs
        {
            /// <summary>
            /// Gets information about the files being uploaded.
            /// </summary>
            /// <value>The file infos.</value>
            public IList<FileInfo> FileInfos { get; private set; }
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="BeginUploadEventArgs"/> should be cancelled.
            /// </summary>
            /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
            public bool Cancel { get; set; }
            internal BeginUploadEventArgs(IList<FileInfo> fileInfos) { FileInfos = fileInfos; }
        }
        private void OnBeginUpload(BeginUploadEventArgs e)
        {
            if (BeginUpload != null)
                BeginUpload(this, e);
        }
        #endregion
        #endregion

        #region Private Classes
        // DataSource used to visualize the AttachmentEditor control inside Expression Blend:
        private class DesignTimeDataSource
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string ContentType { get; set; }
            public long Size { get; set; }
            public Uri Uri { get; set; }
        }
        #endregion
    }
}