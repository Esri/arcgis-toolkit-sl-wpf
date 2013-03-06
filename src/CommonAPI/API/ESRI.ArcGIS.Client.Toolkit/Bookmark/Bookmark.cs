// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
#if NET35
using Microsoft.Windows.Controls;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{

        /// <summary>
        /// The Bookmark Control allows for setting pre-defined Map.Extent values using a name for speedy navigation 
        /// of the Map. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Bookmark is a User Interface (UI) control that allows clients to define preset 
        /// <see cref="ESRI.ArcGIS.Client.Map.Extent">Map.Extent</see> values using a name for quick navigation in the 
        /// Map Control. The Bookmark Control can be created at design time in XAML or dynamically at runtime in the 
        /// code-behind. The Bookmark Control is one of several controls available in the Toolbox in Visual Studio when 
        /// the 
        /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
        /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
        /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
        ///  is installed, see the following screen shot:
        /// </para>
        /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the Bookmark Control on the XAML design surface of a Silverlight application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark.png"/></para></ESRISILVERLIGHT>
        /// <ESRIWPF><para><img border="0" alt="Example of the Bookmark Control on the XAML design surface of a WPF application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark_VS_WPF.png"/></para></ESRIWPF>
        /// <para>
        /// The default appearance of the Bookmark Control can be modified using numerous inherited Properties from 
        /// System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. An example of some 
        /// of these Properties include: .Height, .Width, .BackGround, .BorderBrush, .BorderThickness, .FontFamily, 
        /// .FontSize, .FontStretch, .FontStyle, .Foreground, .HorizontalAlignment, .VerticalAlignment, .Margin, 
        /// .Opacity, .Visibility, etc. 
        /// </para>
        /// <para>
        /// Note: you cannot change the core behavior of the sub-components (i.e. Button, DataGrid, etc.) of the 
        /// Bookmark Control using standard Properties or Methods alone. To change the core behavior of the 
        /// sub-components and their appearance of the Control, developers can modify the Control Template in XAML 
        /// and the associated code-behind file. The easiest way to modify the UI sub-components is using Microsoft 
        /// Expression Blend. Then developers can delete/modify existing or add new sub-components in Visual Studio 
        /// to create a truly customized experience. A general approach to customizing a Control Template is discussed 
        /// in the ESRI blog entitled: 
        /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
        /// Specific code examples of modifying the Control Template of the Bookmark control to can be found in the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.DeleteBookmarkAt">Bookmark.DeleteBookmarkAt</see> Method 
        /// and <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.AddBookmark">Bookmark.AddBookmark</see> Method 
        /// documents.
        /// </para>
        /// <para>
        /// In order to use the Bookmark Control it is mandatory that the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Map">Bookmark.Map</see> Property be set to a Map Control. 
        /// This can be done via Binding in the XAML or by setting the Map Property in the code-behind file.
        /// </para>
        /// <para>
        /// The Bookmark Control is comprised of mainly five sub-components: 
        /// <list type="bullet">
        ///   <item>
        ///   A TextBlock for displaying the <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Title">Bookmark.Title</see>
        ///   </item>
        ///   <item>
        ///   A TextBox to provide a Name for the <see cref="ESRI.ArcGIS.Client.Map.Extent">Map.Extent</see> of the 
        ///   <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark">MapBookmark</see> item
        ///   </item>
        ///   <item>
        ///   A Button to <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.AddBookmark">add</see> the named Map.Extent 
        ///   (aka. a bookmark) to the DataGrid
        ///   </item>
        ///   <item>
        ///   A DataGrid to display the <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Bookmarks">ObservableCollection(Of Bookmark.MapBookmark)</see> 
        ///   that have been added by the user
        ///   </item>
        ///   <item>
        ///   A Button to <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.ClearBookmarks">clear</see> out (or remove) 
        ///   all of the MapBookmark items from the DataGrid
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The following image shows a typical Bookmark Control and its various sub-component parts:
        /// </para>
        /// <para>
        /// <img border="0" alt="Describing the various parts of the Bookmark Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark2.png"/>
        /// </para>
        /// <para>
        /// To use the Bookmark Control, clients use the various pan/zoom functions on the Map Control to select a 
        /// visual Map.Extent and then type in a name for in the TextBox. Then they click the <b>+</b> button to add 
        /// that named Map.Extent (aka. a bookmark) into the DataGrid. Later when the user decides to go back to a 
        /// previous Map.Extent, they click on the name in the DataGrid and the Map will automatically pan/zoom to 
        /// that named Map.Extent (aka. a bookmark). Users click the Clear button to remove all of the named Map.Extent 
        /// (aka. a bookmark) values from the DataGrid.
        /// </para>
        /// <para>
        /// If a portion of the current Map.Extent is in the named Map.Extent (aka. a bookmark) when the user clicks on 
        /// a bookmark item in the DatGrid, the map will pan/zoom using an animation to the clicked bookmark. If the 
        /// current Map.Extent does not contain any spatial overlap with the named Map.Extent (aka. a bookmark) that 
        /// the user clicks on in the DataGrid, there will be not be any animation and the Map will immediately go to 
        /// the new Bookmark location.
        /// </para>
        /// <para>
        /// By default the <b>ESRI.ArcGIS.Client.Toolkit.Bookmark.UseIsolatedStorage</b> 
        /// value equals True. This means that the application will remember from one session to another the named 
        /// Map.Extent (aka. a bookmark) values previously entered. If the Bookmark.UseIsolatedStorage is set to False, 
        /// the next time a user begins another client session of the application the named Map.Extent (aka. a bookmark) 
        /// settings will be lost. <b>Note:</b> There is only one <b>IsolatedStorage</b> container per application. 
        /// This may have implications if you have multiple Maps, each with their own set of Bookmarks. This means you 
        /// may need to make use of the <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Bookmarks">Bookmark.Bookmarks</see> 
        /// Property which returns an ObservableCollection(Of Bookmark.MapBookmark) object to persist across the various 
        /// Maps.
        /// </para>
        /// <para>
        /// When a Bookmark Control is created the ObservableCollection (Of MapBookmark) object is also created that is 
        /// accessible from the Bookmark.Bookmarks Property. Although the Bookmark.Bookmarks Property is ReadOnly, 
        /// meaning you can only get the ObservableCollection (Of MapBookmark) object, you can use its Members to like: 
        /// Add, Clear, Remove, etc. to define how the Bookmark Control behaves.
        /// </para>
        /// <para>
        /// To change the text of a previously entered named Map.Extent (aka. a bookmark) in the DataGrid, click on the 
        /// text portion inside of the selected DataGrid item and the bounding box around the text will turn Black 
        /// (meaning it is ready to accept the cursor for text changes). Then type the desired edits and when done making 
        /// text changes for the named Map.Extent (aka. a bookmark) click with the mouse cursor somewhere outside of the 
        /// editiable portion of the DataGrid. It may take some practice of first selecting a DataGrid item and then 
        /// clicking on the text inside of the DataGrid to get the Black editable background (two clicks in the same spot 
        /// but not a double-click).
        /// </para>
        /// <para>
        /// When the user types in a name for the current Map.Extent to add to the DataGrid by clicking the <b>+</b> 
        /// button, the text value that is typed will remain after the user clicks the <b>+</b> button. The text value 
        /// does not automatically clear when the <b>+</b> button is clicked.
        /// </para>
        /// <para>
        /// It is possible to programmatically control the removal of one named Map.Extent (aka. a bookmark) from the 
        /// DataGrid in the Bookmark Control rather than removing all of them via the Clear button. Use the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.DeleteBookmarkAt">Bookmark.DeleteBookmarkAt</see> Method to 
        /// supply an index number for the correct named Map.Extent (aka. a bookmark) in the DatGrid to be removed. The 
        /// visually top most named Map.Extent (aka. a bookmark) in the DataGrid is the 0 index value.
        /// </para>
        /// <para>
        /// The text of the Title for the Bookmark can be controlled via the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Title">Title</see> Property in XAML or code-behind.
        /// </para>
        /// <para>
        /// Controlling the input text for the TextBox that provides a name for the Map.Extent is done via client 
        /// interaction. Developers can programmatically assign a text value by modifying the sub-component 
        /// <b>AddBookmarkName</b> TextBox of the Bookmark Control using a custom Control Template.
        /// </para>
        /// <para>
        /// Controlling the behavior and appearance of the <b>+</b> button to add a named Map.Extent (aka. a bookmark) 
        /// to the DataGrid can be programmatically controlled by modifying the sub-component <b>AddBookmark</b> Button 
        /// of the Bookmark Control using a custom Control Template.
        /// </para>
        /// <para>
        /// Controlling the behavior and appearance of the DataGrid that contains the ObservableCollection of named 
        /// Map.Extent (aka. a bookmark) values can be programmatically controlled by modifying the sub-component 
        /// <b>BookmarkList</b> DataGrid of the Bookmark Control using a custom Control Template.
        /// </para>
        /// <para>
        /// Controlling the behavior and appearance of the Clear button to remove all named Map.Extent (aka. a bookmark) 
        /// values from the DataGrid can be programmatically controlled by modifying the sub-component 
        /// <b>ClearBookmarks</b> Button of the Bookmark Control using a custom Control Template.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// This example provides a default Bookmark Control that is bound to a Map Control to experiment with its 
        /// functionality. Additionally, there are several Buttons that can be clicked to see how some of the behavior 
        /// and appearance of the Bookmark Control can be modified via the code-behind.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating a default Bookmark Control and usage of several Properties and Methods." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark3.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="70" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
        ///              TextWrapping="Wrap" Margin="2,9,0,0" 
        ///              Text="This example provides a default Bookmark Control that is bound to a Map Control to 
        ///                   experiment with its functionality. Additionally, there are several Buttons that can be 
        ///                   clicked to see how some of the behavior and appearance of the Bookmark Control can be 
        ///                   modified via the code-behind." /&gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,85,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="360" Width="401" &gt;
        ///   
        ///     &lt;!-- Add a default ArcGISTiledMapServiceLayer for visual display in the Map. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="PhysicalTiledLayer" 
        ///                  Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a default Bookmark Control. The Bookmark.Map Property has been bound to the Map Control. 
        ///   The Bookmark.IsolatedStorage Property have been set to False (the default is True) to clear
        ///   out the MapBookmark items each time the application runs. --&gt;
        ///   &lt;esri:Bookmark HorizontalAlignment="Left" Margin="419,278,0,0" Name="Bookmark1" 
        ///                  VerticalAlignment="Top" Width="208" Height="168" 
        ///                  Map="{Binding ElementName=Map1}" UseIsolatedStorage="False"/&gt;
        ///   
        ///   &lt;!-- 
        ///   A Button with code-behind to show adding a MapBookmark to the Bookmark Control. This version
        ///   uses an Envelope.Extent via String X,Y coordinate pairs to populate the MapBookmark.Extent. 
        ///   --&gt;
        ///   &lt;Button Content="AddBookmark Method" HorizontalAlignment="Left" Margin="420,86,0,0" 
        ///           Name="Button_AddBookmark" VerticalAlignment="Top" Width="209" Click="Button_AddBookmark_Click"/&gt;
        ///   
        ///   &lt;!-- 
        ///   A Button with code-behind to show adding a MapBookmark to the Bookmark Control. This version
        ///   uses the current Map.Extent for the MapBookmark.Extent value.  
        ///   --&gt;
        ///   &lt;Button Content="AddBookmark v2 Method" Height="23" HorizontalAlignment="Left" Margin="421,115,0,0" 
        ///           Name="Button_AddBookmarkv2" VerticalAlignment="Top" Width="208" Click="Button_AddBookmarkv2_Click"/&gt;
        ///   
        ///   &lt;!-- A button with code-behind to clear out all items in the Bookmark Control. --&gt;
        ///   &lt;Button Content="ClearBookmarks Method" Height="23" HorizontalAlignment="Left" Margin="421,144,0,0" 
        ///           Name="Button_ClearBookmarks" VerticalAlignment="Top" Width="209" Click="Button_ClearBookmarks_Click"/&gt;
        ///   
        ///   &lt;!-- A button with code-behind to clear out just the first MapBookmark item in the Bookmark Control. --&gt;
        ///   &lt;Button Content="DeleteBookmarkAt Method" Height="23" HorizontalAlignment="Left" Margin="420,173,0,0"
        ///           Name="Button_DeleteBookmarkAt" VerticalAlignment="Top" Width="209" Click="Button_DeleteBookmarkAt_Click" /&gt;
        ///   
        ///   &lt;!-- 
        ///   A button with code-behind to display all of the MapBookmark.Name and MapBookmark.Extent values in 
        ///   the Bookmark Control via a MessageBox. --&gt;
        ///   &lt;Button Content="Bookmarks Property" Height="23" HorizontalAlignment="Left" Margin="420,202,0,0" 
        ///           Name="Button_Bookmarks" VerticalAlignment="Top" Width="209" Click="Button_Bookmarks_Click"/&gt;
        ///   
        ///   &lt;!-- A button with code-behind to change the default Title of the Bookmark Control. --&gt;
        ///   &lt;Button Content="Title Property" Height="23" HorizontalAlignment="Left" Margin="420,231,0,0" 
        ///           Name="Button_Title" VerticalAlignment="Top" Width="209" Click="Button_Title_Click"/&gt;
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button_AddBookmark_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function add a new named Map.Extent (aka. a bookmark) to the ObservableCollection&lt;MapBookmark&gt;.
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtent = new ESRI.ArcGIS.Client.Geometry.Envelope(-1837672.03060728, 3202886.78616195, 4708662.7690822, 9079895.58388817);
        ///   Bookmark1.AddBookmark("Europe", myExtent);
        /// }
        /// 
        /// private void Button_AddBookmarkv2_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // An alternate method of adding a named MapExtent (aka. a bookmark) to the ObservableCollection&lt;MapBookmark&gt;.
        ///   Bookmark1.AddBookmark("A custom location", Map1.Extent);
        /// }
        /// 
        /// private void Button_ClearBookmarks_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This Clears ALL of the Bookmarks. Same functionality as the 'Clear' button in the default GUI for the control.
        ///   Bookmark1.ClearBookmarks();
        /// }
        /// 
        /// private void Button_DeleteBookmarkAt_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function deletes the first occurrence of a MapBookmark from the Bookmark.Control.
        ///   
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; myBookmarks = null;
        ///   myBookmarks = Bookmark1.Bookmarks;
        ///   if (myBookmarks.Count &gt; 0)
        ///   {
        ///     // Delete the top most bookmark
        ///     Bookmark1.DeleteBookmarkAt(0);
        ///   }
        /// }
        /// 
        /// private void Button_Bookmarks_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function loops through all of the MapBookmark items in the Bookmark.Bookmarks 
        ///   // ObservableCollection&lt;MapBookmark&gt; to display the MapBookmark.Name and MapBookmark.Extent.
        /// 
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; myBookmarks = null;
        ///   myBookmarks = Bookmark1.Bookmarks;
        ///   
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark oneMapBookmark in myBookmarks)
        ///   {
        ///     MessageBox.Show("The MapBookmark: " + oneMapBookmark.Name + Environment.NewLine + "Has the Extent: " + oneMapBookmark.Extent.ToString());
        ///   }
        /// }
        /// 
        /// private void Button_Title_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Modify the default title of the Bookmark Control.
        ///   Bookmark1.Title = "My Custom Title";
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_AddBookmark_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function add a new named Map.Extent (aka. a bookmark) to the ObservableCollection(Of MapBookmark).
        ///   Dim myExtent As New ESRI.ArcGIS.Client.Geometry.Envelope(-1837672.03060728, 3202886.78616195, 4708662.7690822, 9079895.58388817)
        ///   Bookmark1.AddBookmark("Europe", myExtent)
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_AddBookmarkv2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' An alternate method of adding a named MapExtent (aka. a bookmark) to the ObservableCollection(Of MapBookmark).
        ///   Bookmark1.AddBookmark("A custom location", Map1.Extent)
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_ClearBookmarks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This Clears ALL of the Bookmarks. Same functionality as the 'Clear' button in the default GUI for the control.
        ///   Bookmark1.ClearBookmarks()
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_DeleteBookmarkAt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function deletes the first occurrence of a MapBookmark from the Bookmark.Control.
        ///   
        ///   Dim myBookmarks As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
        ///   myBookmarks = Bookmark1.Bookmarks
        ///   If myBookmarks.Count > 0 Then
        ///     
        ///     ' Delete the top most bookmark
        ///     Bookmark1.DeleteBookmarkAt(0)
        ///     
        ///   End If
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_Bookmarks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function loops through all of the MapBookmark items in the Bookmark.Bookmarks 
        ///   ' ObservableCollection(Of MapBookmark) to display the MapBookmark.Name and MapBookmark.Extent.
        ///   
        ///   Dim myBookmarks As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
        ///   myBookmarks = Bookmark1.Bookmarks
        ///   
        ///   For Each oneMapBookmark As ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark In myBookmarks
        ///     MessageBox.Show("The MapBookmark: " + oneMapBookmark.Name + vbCrLf +
        ///                     "Has the Extent: " + oneMapBookmark.Extent.ToString)
        ///   Next
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_Title_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Modify the default title of the Bookmark Control.
        ///   Bookmark1.Title = "My Custom Title"
        ///   
        /// End Sub
        /// </code>
        /// </example>
	[TemplatePart(Name = "AddBookmark", Type = typeof(Button))]
	[TemplatePart(Name = "ClearBookmarks", Type = typeof(Button))]
	[TemplatePart(Name = "BookmarkList", Type = typeof(DataGrid))]
	[TemplatePart(Name = "AddBookmarkName", Type = typeof(TextBox))]
	public class Bookmark : Control
	{
#if !SILVERLIGHT
		private int current_index = -1;
		private int previous_index = -1;
#endif
		private bool is_editing = false;

		/// <summary>
		/// Bookmark class for storing named extents
		/// </summary>
		public class MapBookmark
		{
			/// <summary>
			/// Gets or sets the name.
			/// </summary>
			/// <value>The name.</value>
			public string Name { get; set; }
			/// <summary>
			/// Gets or sets the extent.
			/// </summary>
			/// <value>The extent.</value>
			public ESRI.ArcGIS.Client.Geometry.Envelope Extent { get; set; }
			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
			/// </returns>
			public override string ToString()
			{
				return Name;
			}
		}		

		private const string isolatedStorageKey = "ESRI.ArcGIS.Client.Toolkit.BookMarks";
		private string Key
		{
			get { return isolatedStorageKey + "_" + this.Name; }
		}

        /// <summary>
        /// Gets the ObservableCollection containing 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark">Bookmark.MapBookmark</see> objects that are 
        /// displayed in the DataGrid sub-component of the Bookmark Control.   
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Bookmark.Bookmarks Property gets a ReadOnly ObservableCollection(Of 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark">Bookmark.MapBookmark</see>) object. This 
        /// object is used to store the named Map.Extent pairs (aka. bookmarks) that are displayed in the DataGrid 
        /// sub-component of the Bookmark Control. When a Bookmark Control is created, the ObservableCollection 
        /// (Of MapBookmark) object is also created. Although the MapBookmark.Bookmarks Property is ReadOnly, meaning 
        /// you can only get the ObservableCollection (Of MapBookmark) object, you can use the regular 
        /// ObservableCollection Members to like: 
        /// <a href="http://msdn.microsoft.com/en-us/library/ms132404(v=VS.95).aspx" target="_blank">Add</a>, 
        /// <a href="http://msdn.microsoft.com/en-us/library/ms132405(v=VS.95).aspx" target="_blank">Clear</a>, 
        /// <a href="http://msdn.microsoft.com/en-us/library/ms132413(v=VS.95).aspx" target="_blank">Remove</a>, 
        /// etc. to define how the Bookmark Control behaves. <b>NOTE:</b> You cannot create an new instance of the 
        /// ObservableCollection (Of MapBookmark) object and set it to the Bookmark.Bookmarks Property, use the 
        /// ObservableCollection.Add Property instead. 
        /// </para>
        /// <para>
        /// You can also control the behavior of the ObservableCollection (Of MapBookmark) object by using the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.AddBookmark">AddBookmark</see>, 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.ClearBookmarks">ClearBookmarks</see>, and 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.DeleteBookmarkAt">DeleteBookmarkAt</see> Methods. 
        /// </para>
        /// <para>
        /// By default the 
        /// <b>ESRI.ArcGIS.Client.Toolkit.Bookmark.UseIsolatedStorage</b> value 
        /// equals True. This means that the application will remember from one session to another the named Map.Extent 
        /// (aka. a bookmark) values previously entered. If the Bookmark.UseIsolatedStorage is set to False, the next 
        /// time a user begins another client session of the application the named Map.Extent (aka. a bookmark) settings 
        /// will be lost. <b>Note:</b> There is only one <b>IsolatedStorage</b> container per application. This may have 
        /// implications if you have multiple Maps, each with their own set of bookmarks. This means you may need to make 
        /// use of the Bookmark.Bookmarks Property to persist across the various Maps.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the different RadioButtons to change the items in the Bookmark Control. Then click the various 
        /// Bookmark.MapBookmark items in the Bookmark Control to zoom to the different Extents.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="An example of swapping different items in the ObservableCollection of Bookmark.Bookmarks." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark.Bookmarks.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="52" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
        ///            TextWrapping="Wrap" Margin="2,9,0,0" 
        ///            Text="Click the different RadioButtons to change the items in the Bookmark Control. Then click the
        ///                 various Bookmark.MapBookmark items in the Bookmark Control to zoom to the different Extents." /&gt;
        ///   
        ///   &lt;!-- Add a Map Control. --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,85,0,0" Name="Map1" 
        ///             VerticalAlignment="Top" Height="360" Width="401" &gt;
        ///         
        ///     &lt;!-- Add an ArcGISTiledMapServiceLayer for a data source. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="PhysicalTiledLayer" 
        ///                  Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer"/&gt;
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- Add a Bookmark Control. Bind the Bookmark.Map Property to the Map Control. --&gt;
        ///   &lt;esri:Bookmark HorizontalAlignment="Left" Margin="426,85,0,0" Name="Bookmark1" 
        ///                  VerticalAlignment="Top" Width="179" 
        ///                  Map="{Binding ElementName=Map1}"/&gt;
        ///       
        ///   &lt;!-- 
        ///   Add a RadioButton that will correspond to State level predefined Map.Extents. The Click Event is
        ///   Wired-up for use with the code-behind. Note how it uses the same Event handler as the other
        ///   Radio Buttons. It is the use of the RadioButton.Tag Property that will help the code-behind
        ///   functions know which RadioButton was chosen.
        ///   --&gt;
        ///   &lt;RadioButton Content="State (FL and GA)" Height="16" HorizontalAlignment="Left" Margin="12,67,0,0"
        ///                Name="RadioButton_State" VerticalAlignment="Top" Click="RadioButtons"
        ///                Tag="State"/&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a RadioButton that will correspond to local level predefined Map.Extents. The Click Event is
        ///   Wired-up for use with the code-behind. Note how it uses the same Event handler as the other
        ///   Radio Buttons. It is the use of the RadioButton.Tag Property that will help the code-behind
        ///   functions know which RadioButton was chosen.
        ///   --&gt;
        ///   &lt;RadioButton Content="Florida (local)" Height="16" HorizontalAlignment="Left" Margin="154,67,0,0" 
        ///                Name="RadioButton_Local_FL" VerticalAlignment="Top" Click="RadioButtons" 
        ///                Tag="FL"/&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a RadioButton that will correspond to local level predefined Map.Extents. The Click Event is
        ///   Wired-up for use with the code-behind. Note how it uses the same Event handler as the other
        ///   Radio Buttons. It is the use of the RadioButton.Tag Property that will help the code-behind
        ///   functions know which RadioButton was chosen.
        ///   --&gt;
        ///   &lt;RadioButton Content="Georgia (local)" Height="16" HorizontalAlignment="Left" Margin="288,67,0,0" 
        ///                Name="RadioButton_Local_GA" VerticalAlignment="Top" Click="RadioButtons"
        ///                Tag="GA"/&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void RadioButtons(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function serves as the Click Event handler for the three RadioButtons. Get the .Tag 
        ///   // Property from the RadioButton to construct an ObservableCollecton of MapBookmark objects. 
        ///   // Then add the MapBookmark items to the Bookmark Control.
        ///   
        ///   // Get the RadioButton from the sender.
        ///   RadioButton theRadioButton = (RadioButton)sender;
        ///   
        ///   // Get the String identifying which RadioButton we have from the RadioButton.Tag Property.
        ///   string theTag = (string)theRadioButton.Tag;
        ///   
        ///   // Get the ObservableCollection&lt;Of MapBookmark&gt; object based upon which RadioButton the user chose.
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; myObservableCollectionOfMapBookmark = null;
        ///   myObservableCollectionOfMapBookmark = GenerateObservableCollectionOfMapBookmark(theTag);
        ///   
        ///   // Clear out any existing MapBookmark objects from the Bookmark Control.
        ///   Bookmark1.ClearBookmarks();
        ///   
        ///   // Loop through the ObservableCollection&lt;MapBookmark&gt; object and add each MapBookmark to 
        ///   // the Bookmark Control using the .Bookmarks Property.
        ///   foreach (ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark oneMapBookmark in myObservableCollectionOfMapBookmark)
        ///   {
        ///     // The Bookmark.Bookmarks Property returns a ReadOnly ObservableCollection&lt;MapBookmark&gt; 
        ///     // object. So to get items in the ObservableCollection, use the Add Property.
        ///     Bookmark1.Bookmarks.Add(oneMapBookmark);
        ///   
        ///     // This could be an alternative way to add a MapBookmark to the ObservableCollection!
        ///     // Bookmark1.AddBookmark(oneMapBookmark.Name, oneMapBookmark.Extent);
        ///   }
        ///   
        ///   // NOTE: Because the Bookmark.Bookmarks Property returns a Read Only 
        ///   // ObservableCollection&lt;MapBookmark&gt;, you CANNOT do this:
        ///   // Bookmark1.Bookmarks = myObservableCollectionOfMapBookmark;
        /// }
        /// 
        /// public Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; GenerateObservableCollectionOfMapBookmark(string theTag)
        /// {
        ///   // This the main function that generates the correct set of MapBookmark items to be added to the
        ///   // Bookmark Control. Depending on which RadioButton the user chose will determine what gets added to
        ///   // the ObservableCollection&lt;MapBookmark&gt;.
        ///   
        ///   // Create a new instance of the ObservableCollection&lt;MapBookmark&gt;.
        ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; theBookmarks = new Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt;();
        ///   
        ///   if (theTag == "State")
        ///   {
        ///     // Generate MapBookmark's for a State level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Florida and Georgia", new ESRI.ArcGIS.Client.Geometry.Envelope(-10126037, 2750924, -8487426, 4221996)));
        ///     theBookmarks.Add(MakeMapBookmark("Florida", new ESRI.ArcGIS.Client.Geometry.Envelope(-9808676, 2779124, -8812475, 3673469)));
        ///     theBookmarks.Add(MakeMapBookmark("Georgia", new ESRI.ArcGIS.Client.Geometry.Envelope(-9633752, 3534963, -8898290, 4195229)));
        ///   }
        ///   else if (theTag == "FL")
        ///   {
        ///     // Generate MapBookmark's for a local level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Jacksonville", new ESRI.ArcGIS.Client.Geometry.Envelope(-9105796, 3532477, -9075555, 3559626)));
        ///     theBookmarks.Add(MakeMapBookmark("Miami", new ESRI.ArcGIS.Client.Geometry.Envelope(-8936736, 2965084, -8918792, 2981193)));
        ///     theBookmarks.Add(MakeMapBookmark("Tampa", new ESRI.ArcGIS.Client.Geometry.Envelope(-9194994, 3225279, -9165864, 3251430)));
        ///   }
        ///   else if (theTag == "GA")
        ///   {
        ///     // Generate MapBookmark's for a local level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Atlanta", new ESRI.ArcGIS.Client.Geometry.Envelope(-9421000, 3975108, -9367812, 4022858)));
        ///     theBookmarks.Add(MakeMapBookmark("Columbus", new ESRI.ArcGIS.Client.Geometry.Envelope(-9464616, 3820843, -9455555, 3828978)));
        ///     theBookmarks.Add(MakeMapBookmark("Savannah", new ESRI.ArcGIS.Client.Geometry.Envelope(-9043472, 3757220, -9013688, 3783958)));
        ///   }
        ///   
        ///   // Return the ObservableCollection&lt;MapBookmark&gt; to the caller.
        ///   return theBookmarks;
        /// }
        /// 
        /// public ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark MakeMapBookmark(string aName, ESRI.ArcGIS.Client.Geometry.Envelope anExtent)
        /// {
        ///   // A helper function to create a single MapBookmark based upon an input Name and Extent.
        ///   ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark aMapBookmark = new ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark();
        ///   aMapBookmark.Name = aName;
        ///   aMapBookmark.Extent = anExtent;
        ///   
        ///   // Return the MapBookmark to the caller.
        ///   return aMapBookmark;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub RadioButtons(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function serves as the Click Event handler for the three RadioButtons. Get the .Tag 
        ///   ' Property from the RadioButton to construct an ObservableCollecton of MapBookmark objects. 
        ///   ' Then add the MapBookmark items to the Bookmark Control.
        ///   
        ///   ' Get the RadioButton from the sender.
        ///   Dim theRadioButton As RadioButton = sender
        ///   
        ///   ' Get the String identifying which RadioButton we have from the RadioButton.Tag Property.
        ///   Dim theTag As String = theRadioButton.Tag
        ///   
        ///   ' Get the ObservableCollection(Of MapBookmark) object based upon which RadioButton the user chose.
        ///   Dim myObservableCollectionOfMapBookmark As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
        ///   myObservableCollectionOfMapBookmark = GenerateObservableCollectionOfMapBookmark(theTag)
        ///   
        ///   ' Clear out any existing MapBookmark objects from the Bookmark Control.
        ///   Bookmark1.ClearBookmarks()
        ///   
        ///   ' Loop through the ObservableCollection(Of MapBookmark) object and add each MapBookmark to 
        ///   ' the Bookmark Control using the .Bookmarks Property.
        ///   For Each oneMapBookmark As ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark In myObservableCollectionOfMapBookmark
        ///   
        ///     ' The Bookmark.Bookmarks Property returns a ReadOnly ObservableCollection(Of MapBookmark) 
        ///     ' object. So to get items in the ObservableCollection, use the Add Property.
        ///     Bookmark1.Bookmarks.Add(oneMapBookmark)
        ///     
        ///     ' This could be an alternative way to add a MapBookmark to the ObservableCollection!
        ///     'Bookmark1.AddBookmark(oneMapBookmark.Name, oneMapBookmark.Extent)
        ///     
        ///   Next
        ///   
        ///   ' NOTE: Because the Bookmark.Bookmarks Property returns a Read Only 
        ///   ' ObservableCollection(Of MapBookmark), you CANNOT do this:
        ///   'Bookmark1.Bookmarks = myObservableCollectionOfMapBookmark
        ///   
        /// End Sub
        /// 
        /// Public Function GenerateObservableCollectionOfMapBookmark(ByVal theTag As String) As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
        ///   
        ///   ' This the main function that generates the correct set of MapBookmark items to be added to the
        ///   ' Bookmark Control. Depending on which RadioButton the user chose will determine what gets added to
        ///   ' the ObservableCollection(Of MapBookmark).
        ///   
        ///   ' Create a new instance of the ObservableCollection(Of MapBookmark).
        ///   Dim theBookmarks As New Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
        ///   
        ///   If theTag = "State" Then
        ///     
        ///     ' Generate MapBookmark's for a State level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Florida and Georgia", New ESRI.ArcGIS.Client.Geometry.Envelope(-10126037, 2750924, -8487426, 4221996)))
        ///     theBookmarks.Add(MakeMapBookmark("Florida", New ESRI.ArcGIS.Client.Geometry.Envelope(-9808676, 2779124, -8812475, 3673469)))
        ///     theBookmarks.Add(MakeMapBookmark("Georgia", New ESRI.ArcGIS.Client.Geometry.Envelope(-9633752, 3534963, -8898290, 4195229)))
        ///     
        ///   ElseIf theTag = "FL" Then
        ///     
        ///     ' Generate MapBookmark's for a local level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Jacksonville", New ESRI.ArcGIS.Client.Geometry.Envelope(-9105796, 3532477, -9075555, 3559626)))
        ///     theBookmarks.Add(MakeMapBookmark("Miami", New ESRI.ArcGIS.Client.Geometry.Envelope(-8936736, 2965084, -8918792, 2981193)))
        ///     theBookmarks.Add(MakeMapBookmark("Tampa", New ESRI.ArcGIS.Client.Geometry.Envelope(-9194994, 3225279, -9165864, 3251430)))
        ///     
        ///   ElseIf theTag = "GA" Then
        ///     
        ///     ' Generate MapBookmark's for a local level of viewing.
        ///     theBookmarks.Add(MakeMapBookmark("Atlanta", New ESRI.ArcGIS.Client.Geometry.Envelope(-9421000, 3975108, -9367812, 4022858)))
        ///     theBookmarks.Add(MakeMapBookmark("Columbus", New ESRI.ArcGIS.Client.Geometry.Envelope(-9464616, 3820843, -9455555, 3828978)))
        ///     theBookmarks.Add(MakeMapBookmark("Savannah", New ESRI.ArcGIS.Client.Geometry.Envelope(-9043472, 3757220, -9013688, 3783958)))
        ///     
        ///   End If
        ///   
        ///   ' Return the ObservableCollection(Of MapBookmark) to the caller.
        ///   Return theBookmarks
        ///   
        /// End Function
        /// 
        /// Public Function MakeMapBookmark(ByVal aName As String, ByVal anExtent As ESRI.ArcGIS.Client.Geometry.Envelope) As ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark
        ///   
        ///   ' A helper function to create a single MapBookmark based upon an input Name and Extent.
        ///   Dim aMapBookmark As New ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark
        ///   aMapBookmark.Name = aName
        ///   aMapBookmark.Extent = anExtent
        ///   
        ///   ' Return the MapBookmark to the caller.
        ///   Return aMapBookmark
        ///   
        /// End Function
        /// </code>
        /// </example>
		public System.Collections.ObjectModel.ObservableCollection<MapBookmark> Bookmarks { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Bookmark"/> class.
		/// </summary>
		public Bookmark()
		{
			Bookmarks = new System.Collections.ObjectModel.ObservableCollection<MapBookmark>();
#if SILVERLIGHT
			DefaultStyleKey = typeof(Bookmark);
			UseIsolatedStorage = true;		
			this.Loaded += new RoutedEventHandler(Bookmark_Loaded);
#endif
		}

		/// <summary>
		/// Static initialization for the <see cref="Bookmark"/> control.
		/// </summary>
		static Bookmark()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Bookmark), new FrameworkPropertyMetadata(typeof(Bookmark)));
#endif
		}

#if SILVERLIGHT
		private void Bookmark_Loaded(object sender, RoutedEventArgs e)
		{
			LoadBookmarks();
		}

		private void SaveBookmarks()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;

			try
			{
				if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains(Key))
					System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Remove(Key);

				if (UseIsolatedStorage && Bookmarks != null && Bookmarks.Count > 0)
				{
                    System.Collections.ObjectModel.ObservableCollection<MapBookmark> bookmarkClone = new System.Collections.ObjectModel.ObservableCollection<MapBookmark>();
                    foreach (MapBookmark item in Bookmarks)
                        bookmarkClone.Add(item);
					System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Add(Key, bookmarkClone);
				}
				System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Save();
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(Properties.Resources.Bookmark_SaveFailed + ex.Message);
			}
		}

		private void LoadBookmarks()
		{
			if (UseIsolatedStorage && !DesignerProperties.GetIsInDesignMode(this))
			{
				if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains(Key))
				{
					System.Collections.ObjectModel.ObservableCollection<MapBookmark> storedMarks = 
						System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[Key] as System.Collections.ObjectModel.ObservableCollection<MapBookmark>;
                    Bookmarks.Clear();
					if (storedMarks != null)
						foreach (MapBookmark marks in storedMarks)
							Bookmarks.Add(marks);
				}
			}
		}
#endif

		TextBox AddBookmarkName;
		Button AddBookmarkButton;
		DataGrid BookmarkList;
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code
		/// or internal processes (such as a rebuilding layout pass) call 
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			AddBookmarkName = GetTemplateChild("AddBookmarkName") as TextBox;
			AddBookmarkButton = GetTemplateChild("AddBookmark") as Button;
			BookmarkList = GetTemplateChild("BookmarkList") as DataGrid;
			
			if(AddBookmarkButton!=null)
				AddBookmarkButton.Click += AddBookmarkButton_Click;
			if (BookmarkList != null)
			{
#if !SILVERLIGHT
				BookmarkList.CanUserAddRows = false;
#endif
				BookmarkList.ItemsSource = Bookmarks;
				BookmarkList.SelectionChanged += BookmarkList_SelectionChanged;
				BookmarkList.BeginningEdit += BookmarkList_BeginningEdit;
				BookmarkList.CellEditEnding += BookmarkList_CellEditEnding;

			}
			Button ClearBookmarksButton = GetTemplateChild("ClearBookmarks") as Button;
			if (ClearBookmarksButton != null)
			{
				ClearBookmarksButton.Click += (o, e) => { ClearBookmarks(); };
			}
		}

		void BookmarkList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			is_editing = false;
		}

		void BookmarkList_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			is_editing = true;
		}

		private void BookmarkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
#if !SILVERLIGHT
			current_index = BookmarkList.SelectedIndex;
#endif
			MapBookmark bookmark = BookmarkList.SelectedItem as MapBookmark;
			if (bookmark!=null && Map != null && !Double.IsNaN(Map.Resolution))
			{
				Map.ZoomTo(bookmark.Extent);
			}
		}

		private void AddBookmarkButton_Click(object sender, RoutedEventArgs e)
		{
			if (Map == null) return;

			string name = null;
			if (AddBookmarkName != null)
				name = AddBookmarkName.Text;
			AddBookmark(name, Map.Extent);
		}

		/// <summary>
		/// Identifies the <see cref="Map"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(Map), typeof(Bookmark), new PropertyMetadata(OnMapPropertyChanged));

		private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Bookmark bookmark = d as Bookmark;
			Map oldValue = e.OldValue as Map;
			Map newValue = e.NewValue as Map;
			if (oldValue != null)
				oldValue.ExtentChanged -= bookmark.map_ExtentChanged;
			if (newValue != null)
				newValue.ExtentChanged += bookmark.map_ExtentChanged;
		}

		/// <summary>
		/// Gets or sets the map that the <see cref="Bookmark"/> is buddied to.
		/// </summary>
		public Map Map
		{
			get { return GetValue(MapProperty) as Map; }
			set { SetValue(MapProperty, value); }
		}

		private void map_ExtentChanged(object sender, ExtentEventArgs args)
		{		
#if !SILVERLIGHT
			// the data grid in wpf requires two mouse clicks to enter edit mode. 
			// the silverlight data grid only requires one mouse click to enter edit mode.
			// current index and previous index manage the history for the wpf version 
			// so the index is not lost after first click.
			if (current_index != -1 && current_index == previous_index || is_editing)
			{
				current_index = -1;
				return;
			}
			previous_index = current_index;			
#else
			if(is_editing) return;
#endif
			if (BookmarkList != null)
				BookmarkList.SelectedIndex = -1;
		}

        /// <summary>
        /// Adds a named Map.Extent (aka. a bookmark) into the DataGrid sub-component of the Bookmark Control. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The AddBookmark Method adds a named Map.Extent (aka. a bookmark) into the 
        /// ObservableCollection(Of Bookmark.MapBookmark) object. The ObservableCollection(Of Bookmark.MapBookmark) 
        /// object is internally bound to the DataGrid sub-component in the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark">Bookmark</see> Control. Use the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Bookmarks">Bookmark.Bookmarks</see> Property to obtain the 
        /// ObservableCollection(Of Bookmark.MapBookmark) object.
        /// </para>
        /// <para>
        /// There are two input parameters for the AddBookmark Method: <b>name</b> and <b>extent</b>.The <b>name</b> 
        /// is the human readable string that defines a geographic extent on the Map. The <b>extent</b> is the 
        /// <see cref="ESRI.ArcGIS.Client.Map.Extent">Map.Extent</see> that corresponds to item <b>name</b> in the 
        /// ObservableCollection.
        /// </para>
        /// <para>
        /// The functionality of the AddBookmark Method is comparable to when a user of a client application types a 
        /// string into the Textbox sub-component of the Bookmark Control and clicks the <b>+</b> button, causing the 
        /// named Map.Extent (aka. a bookmark) to appear in the DataGrid. It should be noted that the AddBookmark 
        /// Method IS NOT the Event for the <b>+</b> button of the Bookmark Control. To change the core behavior of 
        /// any of the sub-components and their appearance of the Bookmark Control (including the <b>+</b> button), 
        /// developers must modify the Control Template in XAML and the associated code-behind file. The easiest way 
        /// to modify the UI sub-components is using Microsoft Expression Blend. Then developers can delete/modify 
        /// existing or add new sub-components in Visual Studio to create a truly customized experience. A general 
        /// approach to customizing a Control Template is discussed in the ESRI blog entitled: 
        /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
        /// An example of modifying the Control Template of the Bookmark control to provide an alternate set of 
        /// functionality that automatically populates several named Map.Extent (aka. a bookmark) values via the 
        /// AddBookmark Method is provided in the code example section of this document.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// This example code shows disabling some of the features of the Bookmark Control to create a kiosk type of 
        /// application. Users are not allowed to make any editable changes to the Bookmark Control. To use: Click the 
        /// various preset bookmarks to see the Four Corners area of the United States.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Example of overriding the default behavior of the Bookmark Control to auto-populate several bookmarks via the AddBookmark Method." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark.AddBookmark.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
        ///   
        ///   &lt;!-- 
        ///   Use the Resources section to hold a Style for setting the appearance and behavior of the Bookmark Control. 
        ///   Don't forget to add following XAML Namespace definitions to the correct location in your code:
        ///   xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk" 
        ///   xmlns:esri="http://schemas.esri.com/arcgis/client/2009" 
        ///   --&gt;
        ///   &lt;Grid.Resources&gt;
        ///   
        ///     &lt;!--
        ///     The majority of the XAML that defines the ControlTemplate for the Bookmark Control was obtained
        ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
        ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
        ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
        ///     --&gt;
        ///     &lt;Style x:Key="BookmarkStyle1" TargetType="esri:Bookmark"&gt;
        ///       &lt;Setter Property="MaxHeight" Value="200"/&gt;
        ///       &lt;Setter Property="Width" Value="120"/&gt;
        ///       &lt;Setter Property="Background" Value="#99000000"/&gt;
        ///     
        ///       &lt;!-- The Title.Value was modified. --&gt;
        ///       &lt;Setter Property="Title" Value="Four Corners Kiosk"/&gt;
        ///       
        ///       &lt;Setter Property="BorderThickness" Value="1"/&gt;
        ///       &lt;Setter Property="BorderBrush" Value="White"/&gt;
        ///       &lt;Setter Property="Template"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;ControlTemplate TargetType="esri:Bookmark"&gt;
        ///             &lt;Grid Background="Yellow"&gt;
        ///               &lt;Grid.RowDefinitions&gt;
        ///                 &lt;RowDefinition Height="25"/&gt;
        ///                   
        ///                 &lt;!-- Changed the Height for better visual appeal. --&gt;
        ///                 &lt;RowDefinition Height="1"/&gt;
        ///                                  
        ///                 &lt;RowDefinition Height="*"/&gt;
        ///                 &lt;RowDefinition Height="25"/&gt;
        ///               &lt;/Grid.RowDefinitions&gt;
        ///               &lt;Border BorderBrush="{TemplateBinding BorderBrush}" 
        ///                       BorderThickness="{TemplateBinding BorderThickness}" 
        ///                       Background="{TemplateBinding Background}" 
        ///                       CornerRadius="5" 
        ///                       Grid.RowSpan="4"/&gt;
        ///               &lt;TextBlock Foreground="Black" FontWeight="Bold" FontSize="12" 
        ///                          FontFamily="Verdana" Margin="5,5,5,0" 
        ///                          Grid.Row="0" Text="{TemplateBinding Title}"/&gt;
        ///                  
        ///               &lt;!-- Comment out the entire section that allows users to add bookmarks. Since 
        ///               this is a Kiosk Application, we do not want users to make any changes.
        ///               --&gt;
        ///               &lt;!--
        ///               &lt;Grid Margin="5,0,5,0" Grid.Row="1"&gt;
        ///                 &lt;Grid.ColumnDefinitions&gt;
        ///                   &lt;ColumnDefinition Width="*"/&gt;
        ///                   &lt;ColumnDefinition Width="30"/&gt;
        ///                 &lt;/Grid.ColumnDefinitions&gt;
        ///                 &lt;TextBox x:Name="AddBookmarkName" Grid.Column="0" Width="200"/&gt;
        ///                 &lt;Button x:Name="AddBookmark" Content="+" Grid.Column="1" /&gt;
        ///               &lt;/Grid&gt;
        ///               --&gt;
        ///                             
        ///               &lt;!-- Add the IsReadOnly Attribute to disable the user from editing the bookmark names. --&gt;
        ///               &lt;sdk:DataGrid x:Name="BookmarkList" AutoGenerateColumns="False" CanUserResizeColumns="False" 
        ///                             CanUserReorderColumns="False" HeadersVisibility="None" Margin="5,0,5,0" 
        ///                             Grid.Row="2" RowHeight="16" RowDetailsVisibilityMode="Collapsed" 
        ///                             TabNavigation="Local" Visibility="Visible" IsReadOnly="True" &gt;
        ///                 &lt;sdk:DataGrid.Columns&gt;
        ///                 &lt;sdk:DataGridTextColumn Binding="{Binding Name, Mode=TwoWay}" Foreground="Black" 
        ///                                         FontSize="10" FontFamily="Times" Header="Bookmark" 
        ///                                         IsReadOnly="False" Width="{TemplateBinding Width}"/&gt;
        ///                 &lt;/sdk:DataGrid.Columns&gt;
        ///               &lt;/sdk:DataGrid&gt;
        ///               
        ///               &lt;!-- Comment out the entire section that allows users to delete bookmarks. Since 
        ///               this is a Kiosk Application, we do not want users to make any changes.
        ///               --&gt;
        ///               &lt;!--
        ///               &lt;Grid Margin="5,0,5,5" Grid.Row="3"&gt;
        ///                 &lt;Grid.ColumnDefinitions&gt;
        ///                   &lt;ColumnDefinition Width="*"/&gt;
        ///                   &lt;ColumnDefinition Width="*"/&gt;
        ///                 &lt;/Grid.ColumnDefinitions&gt;
        ///                 &lt;Button x:Name="ClearBookmarks" Content="Clear All" Grid.Column="0"/&gt; 
        ///               &lt;/Grid&gt;
        ///               --&gt;
        ///                               
        ///             &lt;/Grid&gt;
        ///           &lt;/ControlTemplate&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///     &lt;/Style&gt;
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="70" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
        ///              TextWrapping="Wrap" Margin="2,9,0,0" 
        ///              Text="This example code shows disabling some of the features of the Bookmark Control to create
        ///                   a kiosk type of application. Users are not allowed to make any editable changes to the Bookmark
        ///                   Control. To use: Click the various preset bookmarks to see the Four Corners area of the 
        ///                   United States." /&gt;
        ///   
        ///   &lt;!-- Add a Map Control to the application and define an intial Map.Extent to the Four Corners states. --&gt;
        ///   &lt;esri:Map x:Name="MyMap" Extent="-12948716, 3633316, -11207358, 5196630"
        ///             Background="White" HorizontalAlignment="Left" Margin="12,85,0,0"  
        ///             VerticalAlignment="Top" Height="360" Width="401"&gt;
        ///   
        ///     &lt;!-- Add an ArcGISTiledMapServiceLayer for some quick data display. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer 
        ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a Bookmark Control. It is important to provide an x:Name attribute so it can be referenced
        ///   in the code-behind. Define the Style of the Bookmark to use the Control Template that was generated
        ///   in Blend and modified here in Visual Studio. The Map Property uses the default two-way Binding to 
        ///   associate the Bookmark Control with the Map Control. The Loaded Event handler was added so that in
        ///   the code-behind some predefined bookmarks could be added.
        ///   --&gt;
        ///   &lt;esri:Bookmark x:Name="MyBookmarks" Width="200" HorizontalAlignment="Right" VerticalAlignment="Top" 
        ///                  Margin="0,85,12,0" Background="#CC919191" BorderBrush="#FF92a8b3" Foreground="Black"   
        ///                  Style="{StaticResource BookmarkStyle1}"
        ///                  Map="{Binding ElementName=MyMap}" Loaded="MyBookmarks_Loaded"/&gt;
        ///       
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void MyBookmarks_Loaded(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function clears out any existing bookmarks and added several new ones to provide predefined
        ///   // functionality for zooming around the Four Corners states of the United States.
        ///   
        ///   // Clear out any existing bookmarks. 
        ///   MyBookmarks.ClearBookmarks();
        ///   
        ///   // Add bookmarks for the various areas of the Four Corners region of the United States.
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtentTheFourCorners = new ESRI.ArcGIS.Client.Geometry.Envelope(-12948716, 3633316, -11207358, 5196630);
        ///   MyBookmarks.AddBookmark("The Four Corners", myExtentTheFourCorners);
        ///   
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtentArizona = new ESRI.ArcGIS.Client.Geometry.Envelope(-12921190, 3642001, -11973117, 4493139);
        ///   MyBookmarks.AddBookmark("Arizona", myExtentArizona);
        ///   
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtentColorado = new ESRI.ArcGIS.Client.Geometry.Envelope(-12190630, 4330502, -11325307, 5107351);
        ///   MyBookmarks.AddBookmark("Colorado", myExtentColorado);
        ///   
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtentNewMexico = new ESRI.ArcGIS.Client.Geometry.Envelope(-12289052, 3647640, -11371077, 4471758);
        ///   MyBookmarks.AddBookmark("New Mexico", myExtentNewMexico);
        ///   
        ///   ESRI.ArcGIS.Client.Geometry.Envelope myExtentUtah = new ESRI.ArcGIS.Client.Geometry.Envelope(-12850560, 4409389, -11995243, 5177256);
        ///   MyBookmarks.AddBookmark("Utah", myExtentUtah);
        ///  }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub MyBookmarks_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function clears out any existing bookmarks and added several new ones to provide predefined
        ///   ' functionality for zooming around the Four Corners states of the United States.
        ///   
        ///   ' Clear out any existing bookmarks. 
        ///   MyBookmarks.ClearBookmarks()
        ///   
        ///   ' Add bookmarks for the various areas of the Four Corners region of the United States.
        ///   Dim myExtentTheFourCorners As New ESRI.ArcGIS.Client.Geometry.Envelope(-12948716, 3633316, -11207358, 5196630)
        ///   MyBookmarks.AddBookmark("The Four Corners", myExtentTheFourCorners)
        ///   
        ///   Dim myExtentArizona As New ESRI.ArcGIS.Client.Geometry.Envelope(-12921190, 3642001, -11973117, 4493139)
        ///   MyBookmarks.AddBookmark("Arizona", myExtentArizona)
        ///   
        ///   Dim myExtentColorado As New ESRI.ArcGIS.Client.Geometry.Envelope(-12190630, 4330502, -11325307, 5107351)
        ///   MyBookmarks.AddBookmark("Colorado", myExtentColorado)
        ///   
        ///   Dim myExtentNewMexico As New ESRI.ArcGIS.Client.Geometry.Envelope(-12289052, 3647640, -11371077, 4471758)
        ///   MyBookmarks.AddBookmark("New Mexico", myExtentNewMexico)
        ///   
        ///   Dim myExtentUtah As New ESRI.ArcGIS.Client.Geometry.Envelope(-12850560, 4409389, -11995243, 5177256)
        ///   MyBookmarks.AddBookmark("Utah", myExtentUtah)
        ///   
        /// End Sub
        /// </code>
        /// </example>
		/// <param name="name">Display name (aka. named Map.Extent or bookmark).</param>
		/// <param name="extent">The Map.Extent of the bookmark.</param>
		public void AddBookmark(string name, ESRI.ArcGIS.Client.Geometry.Envelope extent)
		{
			name = name.Trim();
			if (string.IsNullOrEmpty(name))
			{
				name = Properties.Resources.Bookmark_Name + (Bookmarks.Count + 1);
			}
			MapBookmark bookmark = new MapBookmark()
			{
				Name = name,
				Extent = extent
			};
			Bookmarks.Add(bookmark);
			if (BookmarkList != null)
			{
				BookmarkList.InvalidateMeasure();
				if (BookmarkList.Columns.Count > 0)
					BookmarkList.ScrollIntoView(bookmark, BookmarkList.Columns[0]);
			}
#if SILVERLIGHT
			SaveBookmarks();
#endif
		}

  /// <summary>
  /// Deletes a bookmark from the DataGrid sub-component of the Bookmark Control at a specified index.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The DeleteBookmark Method deletes a single Bookmark from the ObservableCollection(Of Bookmark.MapBookmark) 
  /// object. The ObservableCollection(Of Bookmark.MapBookmark) object is internally bound to the DataGrid 
  /// sub-component in the <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark">Bookmark</see> Control. Use the 
  /// <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.Bookmarks">Bookmark.Bookmarks</see> Property to obtain the 
  /// ObservableCollection(Of Bookmark.MapBookmark) object. 
  /// </para>
  /// <para>
  /// Unlike the <see cref="ESRI.ArcGIS.Client.Toolkit.Bookmark.AddBookmark">Bookmark.AddBookmark</see> Method 
  /// which takes a <b>name</b> and a <b>Map.Extent</b> as parameters to add items into the ObservableCollection, 
  /// the DeleteBookmark Method requires an Integer <b>index</b> value to remove an item from the 
  /// ObservableCollection. No Method exists in the 
  /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
  /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
  ///  to remove an item from the ObservableCollection by its name. 
  /// </para>
  /// <para>
  /// The top-most Bookmark listed in the DataGrid sub-component of the Bookmark Control is the zero (0) item index 
  /// value.
  /// </para>
  /// <para>
  /// There is no Property/Method in the 
  /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
  /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
  /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
  /// for the Bookmark Control to 
  /// obtain the selected index value(s) of the DataGrid sub-component from client user interaction. However, 
  /// developers can change the core behavior of any of the sub-components and their appearance of the Bookmark 
  /// Control (including the obtaining selection index values of the DataGrid sub-component) by modifying the 
  /// Control Template in XAML and the associated code-behind file. The easiest way to modify the UI sub-components 
  /// is using Microsoft Expression Blend. Then developers can delete/modify existing or add new sub-components in 
  /// Visual Studio to create a truly customized experience. A general approach to customizing a Control Template 
  /// is discussed in the ESRI blog entitled: 
  /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
  /// An example of modifying the Control Template of the Bookmark control to obtain the selection index value 
  /// of the DataGrid sub-component in the Bookmark Control is provided in the code example section of this 
  /// document.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// <b>How to use:</b>
  /// </para>
  /// <para>
  /// Pan/Zoom to a desired Map.Extent then type in some text for the name of the Bookmark and click the Add button. 
  /// Repeat several times at other extents to create more Bookmarks. The 'Clear All' button removes all the 
  /// Bookmarks. The 'Clear Selected' button only removes the Bookmark for which is chosen in the DataGrid using 
  /// the DeleteBookmarkAt Method. The 'Add' button has the behavior of adding a named Map.Extent (aka. a bookmark) 
  /// to the DataGrid with the additionally functionality of clearing the TextBox once the 'Add' button has been 
  /// clicked.
  /// </para>
  /// <para>
  /// The following screen shot corresponds to the code example in this page.
  /// </para>
  /// <para>
  /// <img border="0" alt="A customized Bookmark Control via Control Template that demonstrates using the DeleteBookmarkAt Method." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.Bookmark.DeleteBookmarkAt.png"/>
  /// </para>
  /// <code title="Example XAML1" description="" lang="XAML">
  /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
  ///   
  ///   &lt;!-- 
  ///   Use the Resources section to hold a Style for setting the appearance and behavior of the Bookmark Control. 
  ///   Don't forget to add following XAML Namespace definitions to the correct location in your code:
  ///   xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk" 
  ///   xmlns:esri="http://schemas.esri.com/arcgis/client/2009" 
  ///   --&gt;
  ///   &lt;Grid.Resources&gt;
  ///         
  ///     &lt;!--
  ///     The majority of the XAML that defines the ControlTemplate for the Bookmark Control was obtained
  ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
  ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
  ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
  ///     --&gt;  
  ///     &lt;Style x:Key="BookmarkStyle1" TargetType="esri:Bookmark"&gt;
  ///       &lt;Setter Property="MaxHeight" Value="200"/&gt;
  ///       &lt;Setter Property="Width" Value="120"/&gt;
  ///       &lt;Setter Property="Background" Value="#99000000"/&gt;
  ///       
  ///       &lt;!-- The Title.Value was modified. --&gt;
  ///       &lt;Setter Property="Title" Value="Custom Bookmarks"/&gt; 
  ///               
  ///       &lt;Setter Property="BorderThickness" Value="1"/&gt;
  ///       &lt;Setter Property="BorderBrush" Value="White"/&gt;
  ///       &lt;Setter Property="Template"&gt;
  ///         &lt;Setter.Value&gt;
  ///           &lt;ControlTemplate TargetType="esri:Bookmark"&gt;
  ///             &lt;Grid Background="Yellow"&gt;
  ///               &lt;Grid.RowDefinitions&gt;
  ///                 &lt;RowDefinition Height="25"/&gt;
  ///                 &lt;RowDefinition Height="20"/&gt;
  ///                 &lt;RowDefinition Height="*"/&gt;
  ///                 &lt;RowDefinition Height="25"/&gt;
  ///               &lt;/Grid.RowDefinitions&gt;
  ///               &lt;Border BorderBrush="{TemplateBinding BorderBrush}" 
  ///                       BorderThickness="{TemplateBinding BorderThickness}" 
  ///                       Background="{TemplateBinding Background}" 
  ///                       CornerRadius="5" 
  ///                       Grid.RowSpan="4"/&gt;
  ///               &lt;TextBlock Foreground="Black" FontWeight="Bold" FontSize="12" 
  ///                          FontFamily="Verdana" Margin="5,5,5,0" 
  ///                          Grid.Row="0" Text="{TemplateBinding Title}"/&gt;
  ///               &lt;Grid Margin="5,0,5,0" Grid.Row="1"&gt;
  ///                 &lt;Grid.ColumnDefinitions&gt;
  ///                   &lt;ColumnDefinition Width="*"/&gt;
  ///                   &lt;ColumnDefinition Width="30"/&gt;
  ///                 &lt;/Grid.ColumnDefinitions&gt;
  ///                 &lt;TextBox x:Name="AddBookmarkName" Grid.Column="0" Width="200"/&gt;
  ///                   
  ///                 &lt;!-- 
  ///                 Comment out the default Button with its default behavior. We will out our own
  ///                 button and wire-up our own functionality.
  ///                 --&gt;
  ///                 &lt;!-- &lt;Button x:Name="AddBookmark" Content="+" Grid.Column="1" /&gt; --&gt;
  ///                   
  ///                 &lt;!--
  ///                 This Button is new and has different functionality from the default + (i.e. AddBookmark) 
  ///                 Button provided with the Bookmark Control. The new Button will have a different
  ///                 Content value and will automatically erase what was in the TextBox once a new
  ///                 bookmark is added. Note that the Click Event is wired up.
  ///                 --&gt;
  ///                 &lt;Button x:Name="AddBookmarkNEW" Content="Add" Grid.Column="1" Click="AddBookmarkNEW_Click" /&gt;
  ///               
  ///               &lt;/Grid&gt;
  ///                 
  ///               &lt;!--
  ///               Changed the default behavior of the DataGrid sub-component of the Bookmark Control.
  ///               In particular we added a SelectionChanged Event handler and changed the SelectionMode
  ///               to Single so that only one bookmark could be selected at a time for the 
  ///               ClearSelectedBookmark Button to work appropriately.
  ///               --&gt;
  ///               &lt;sdk:DataGrid x:Name="BookmarkList" AutoGenerateColumns="False" CanUserResizeColumns="False" 
  ///                             CanUserReorderColumns="False" HeadersVisibility="None" Margin="5,0,5,0" 
  ///                             Grid.Row="2" RowHeight="16" RowDetailsVisibilityMode="Collapsed" 
  ///                             TabNavigation="Local" Visibility="Visible"
  ///                             SelectionChanged="BookmarkList_SelectionChanged"
  ///                             SelectionMode="Single"&gt;
  ///                 
  ///                 &lt;sdk:DataGrid.Columns&gt;
  ///                   &lt;sdk:DataGridTextColumn Binding="{Binding Name, Mode=TwoWay}" Foreground="Black" 
  ///                                          FontSize="10" FontFamily="Times" Header="Bookmark" 
  ///                                           IsReadOnly="False" Width="{TemplateBinding Width}"/&gt;
  ///                 &lt;/sdk:DataGrid.Columns&gt;
  ///               &lt;/sdk:DataGrid&gt;
  ///               &lt;Grid Margin="5,0,5,5" Grid.Row="3"&gt;
  ///                 &lt;Grid.ColumnDefinitions&gt;
  ///                   &lt;ColumnDefinition Width="*"/&gt;
  ///                   &lt;ColumnDefinition Width="*"/&gt;
  ///                 &lt;/Grid.ColumnDefinitions&gt;
  ///                     
  ///                 &lt;!-- Modified the default Content of the Button to be more explicit. --&gt;
  ///                 &lt;Button x:Name="ClearBookmarks" Content="Clear All" Grid.Column="0"/&gt;
  ///                 
  ///                 &lt;!--
  ///                 This Button is new and adds the ability to Delete just the selected bookmark from
  ///                 the DataGrid sub-control. Notice that the Click Event is wired up for use in the
  ///                 code-behind.
  ///                 --&gt;
  ///                 &lt;Button x:Name="ClearSelectedBookmark" Content="Clear Selected" Grid.Column="1" 
  ///                         Click="ClearSelectedBookmark_Click"/&gt;
  ///                   
  ///               &lt;/Grid&gt;
  ///             &lt;/Grid&gt;
  ///           &lt;/ControlTemplate&gt;
  ///         &lt;/Setter.Value&gt;
  ///       &lt;/Setter&gt;
  ///     &lt;/Style&gt;
  ///   &lt;/Grid.Resources&gt;
  ///   
  ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
  ///   &lt;TextBlock Height="70" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="626" 
  ///            TextWrapping="Wrap" Margin="2,9,0,0" 
  ///            Text="Pan/Zoom to a desired Map.Extent then type in some text for the name of the Bookmark and click
  ///                 the Add button. Repeat several times at other extents to create more Bookmarks. The 'Clear All' 
  ///                 button removes all the Bookmarks. The 'Clear Selected' button only removes the Bookmark for 
  ///                 which is chosen in the DataGrid using the DeleteBookmarkAt Method. The ‘Add’ button has the 
  ///                 behavior of adding a named Map.Extent (aka. a bookmark) to the DataGrid with the additionally 
  ///                 functionality of clearing the TextBox once the ‘Add’ button has been clicked." /&gt;
  ///   
  ///   &lt;!-- Add a Map Control to the application and define an intial Map.Extent. --&gt;
  ///   &lt;esri:Map x:Name="MyMap" Extent="-15000000,2000000,-7000000,8000000"
  ///             Background="White" HorizontalAlignment="Left" Margin="12,85,0,0"  
  ///             VerticalAlignment="Top" Height="360" Width="401"&gt;
  ///       
  ///     &lt;!-- Add an ArcGISTiledMapServiceLayer for some quick data display. --&gt;
  ///     &lt;esri:ArcGISTiledMapServiceLayer 
  ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer" /&gt;
  ///   &lt;/esri:Map&gt;
  ///   
  ///   &lt;!--
  ///   Add a Bookmark Control. It is important to provide an x:Name attribute so it can be referenced
  ///   in the code-behind. Define the Style of the Bookmark to use the Control Template that was generated
  ///   in Blend and modified here in Visual Studio. The Map Property uses the default two-way Binding to 
  ///   associate the Bookmark Control with the Map Control.
  ///   --&gt;
  ///   &lt;esri:Bookmark x:Name="MyBookmarks" Width="200" HorizontalAlignment="Right" VerticalAlignment="Top" 
  ///                  Margin="0,85,12,0" Background="#CC919191" BorderBrush="#FF92a8b3" Foreground="Black"   
  ///                  Style="{StaticResource BookmarkStyle1}" Map="{Binding ElementName=MyMap}" /&gt;
  /// 
  /// &lt;/Grid&gt;
  /// </code>
  /// <code title="Example CS1" description="" lang="CS">
  /// // This is a Member (aka. Global) variable that will contain the currently selected index value
  /// // for the DataGrid sub-component in the Bookmark Control.
  /// public int _SelectedIndex = -1;
  ///   
  /// private void BookmarkList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  /// {
  ///   // This function obtains the index location of the last cell a user clicked in the DataGrid
  ///   // sub-component in the Bookmark Control. It stores that value in the Global variable _SelectedIndex.
  ///   
  ///   // Note: There is a bug in the Bookmark.SelectionChanged Event where it always fires twice when a
  ///   // DataGrid cell is selected. The first time the Bookmark.SelectionChanged Event fires the correct
  ///   // DataGrid.SelectedIndex value is obtained but then the Event fires a second time and always
  ///   // set the DataGrid.SelectedIndex value to -1. So this code is a workaround.
  ///   
  ///   // Get the DataGrid sub-component of the Bookmark Control.
  ///   DataGrid myDataGrid = (DataGrid)sender;
  ///   
  ///   // Get the ObservableCollection&lt;MapBookmark&gt; objects (aka. the named Map.Extent values).
  ///   Collections.ObjectModel.ObservableCollection&lt;ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark&gt; theBookmarks = null;
  ///   theBookmarks = MyBookmarks.Bookmarks;
  ///   //theBookmarks = myDataGrid.ItemsSource; // This would work too!
  ///   
  ///   // Only perform this operation if there at least one named Map.Extent (aka. a bookmark) value.
  ///   if (theBookmarks.Count > 0)
  ///   {
  ///     // Screen out the second firing of this Event.
  ///     if (myDataGrid.SelectedIndex != -1)
  ///     {
  ///       // Set the Member variable to the currently selected cell in the DataGrid sub-component of the BookMark 
  ///       // Control. NOTE: the DataGrid.SelectionMode was set to 'Single' in XAML so that only one bookmark could 
  ///       // be selected at a time for the ClearSelectedBookmark Button to work appropriately.
  ///       _SelectedIndex = myDataGrid.SelectedIndex;
  ///     }
  ///   }
  /// }
  /// 
  /// private void ClearSelectedBookmark_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function deletes a named Map.Extent (aka. a bookmark) from the DataGrid portion of the 
  ///   // Bookmark Control via its index location. The _SelectedIndex object is a global (aka. Member) 
  ///   // variable and hold the current value of the DataGrid.SelectedItem.
  ///   
  ///   // Only process _SelectedIndex values not equal to -1.
  ///   if (_SelectedIndex != -1)
  ///   {
  ///     // Delete the specific named Map.Extent (aka. a bookmark) at the specified _SelectedIndex value.
  ///     MyBookmarks.DeleteBookmarkAt(_SelectedIndex);
  ///   }
  /// }
  /// 
  /// private void AddBookmarkNEW_Click(object sender, System.Windows.RoutedEventArgs e)
  /// {
  ///   // This function obtains the sub-component TextBox that of the Bookmark Control where the user 
  ///   // enters a name for the Map.Extent from the XAML hierarchy defined in the ControlTemplate. In
  ///   // the case of the XAML code you need to go up two levels (i.e. Parent) in order to use the
  ///   // .FindName() function to obtain the desired TextBox by its name. Then add a named Map.Extent
  ///   // (aka. a bookmark) to the DataGrid sub-components of the Bookmark Control. Finally, clear out 
  ///   // the text of the TextBox after the newly added Bookmark was added.
  ///   
  ///   // Traverse through the ControlTemplate hierarchy of controls to find the desired TextBox.
  ///   Button myButton = (Button)sender;
  ///   Grid myGrid1 = (Grid)myButton.Parent;
  ///   Grid mygrid2 = (Grid)myGrid1.Parent;
  ///   TextBox myTextBox = (TextBox)mygrid2.FindName("AddBookmarkName");
  ///   
  ///   // Add a bookmark for the current Map.Extent using the name in the TextBox provided by the user.
  ///   MyBookmarks.AddBookmark(myTextBox.Text, MyMap.Extent);
  ///   
  ///   // Clear out the text in the TextBox for the next round.
  ///   myTextBox.Text = "";
  /// }
  /// </code>
  /// <code title="Example VB1" description="" lang="VB.NET">
  /// ' This is a Member (aka. Global) variable that will contain the currently selected index value
  /// ' for the DataGrid sub-componet in the Bookmark Control.
  /// Public _SelectedIndex As Integer = -1
  ///   
  /// Private Sub BookmarkList_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
  ///   
  ///   ' This function obtains the index location of the last cell a user clicked in the DataGrid
  ///   ' sub-component in the Bookmark Control. It stores that value in the Global variable _SelectedIndex.
  ///   
  ///   ' Note: There is a bug in the Bookmark.SelectionChanged Event where it always fires twice when a
  ///   ' DataGrid cell is selected. The first time the Bookmark.SelectionChanged Event fires the correct
  ///   ' DataGrid.SelectedIndex value is obtained but then the Event fires a second time and always
  ///   ' set the DataGrid.SelectedIndex value to -1. So this code is a workaround.
  ///   
  ///   ' Get the DataGrid sub-component of the Bookmark Control.
  ///   Dim myDataGrid As DataGrid = sender
  ///   
  ///   ' Get the ObservableCollection(Of MapBookmark) objects (aka. the named Map.Extent values).
  ///   Dim theBookmarks As Collections.ObjectModel.ObservableCollection(Of ESRI.ArcGIS.Client.Toolkit.Bookmark.MapBookmark)
  ///   theBookmarks = MyBookmarks.Bookmarks
  ///   'theBookmarks = myDataGrid.ItemsSource 'This would work too!
  ///   
  ///   ' Only perform this operation if there at least one named Map.Extent (aka. a bookmark) value.
  ///   If theBookmarks.Count &gt; 0 Then
  ///     
  ///     ' Screen out the second firing of this Event.
  ///     If myDataGrid.SelectedIndex &lt;&gt; -1 Then
  ///       
  ///       ' Set the Member variable to the currently selected cell in the DataGrid sub-component of the BookMark 
  ///       ' Control. NOTE: the DataGrid.SelectionMode was set to 'Single' in XAML so that only one bookmark could 
  ///       ' be selected at a time for the ClearSelectedBookmark Button to work appropriately.
  ///       _SelectedIndex = myDataGrid.SelectedIndex
  ///       
  ///     End If
  ///     
  ///   End If
  ///   
  /// End Sub
  /// 
  /// Private Sub ClearSelectedBookmark_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function deletes a named Map.Extent (aka. a bookmark) from the DataGrid portion of the 
  ///   ' Bookmark Control via its index location. The _SelectedIndex object is a global (aka. Member) 
  ///   ' variable and hold the current value of the DataGrid.SelectedItem.
  ///   
  ///   ' Only process _SelectedIndex values not equal to -1.
  ///   If _SelectedIndex &lt;&gt; -1 Then
  ///   
  ///     ' Delete the specific named Map.Extent (aka. a bookmark) at the specified _SelectedIndex value.
  ///     MyBookmarks.DeleteBookmarkAt(_SelectedIndex)
  ///   
  ///   End If
  ///   
  /// End Sub
  /// 
  /// Private Sub AddBookmarkNEW_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
  ///   
  ///   ' This function obtains the sub-component TextBox that of the Bookmark Control where the user 
  ///   ' enters a name for the Map.Extent from the XAML hierarchy defined in the ControlTemplate. In
  ///   ' the case of the XAML code you need to go up two levels (i.e. Parent) in order to use the
  ///   ' .FindName() function to obtain the desired TextBox by its name. Then add a named Map.Extent
  ///   ' (aka. a bookmark) to the DataGrid sub-components of the Bookmark Control. Finally, clear out 
  ///   ' the text of the TextBox after the newly added Bookmark was added.
  ///   
  ///   ' Traverse through the ControlTemplate hierarchy of controls to find the desired TextBox.
  ///   Dim myButton As Button = sender
  ///   Dim myGrid1 As Grid = myButton.Parent
  ///   Dim mygrid2 As Grid = myGrid1.Parent
  ///   Dim myTextBox As TextBox = mygrid2.FindName("AddBookmarkName")
  ///   
  ///   ' Add a bookmark for the current Map.Extent using the name in the TextBox provided by the user.
  ///   MyBookmarks.AddBookmark(myTextBox.Text, MyMap.Extent)
  ///   
  ///   ' Clear out the text in the TextBox for the next round.
  ///   myTextBox.Text = ""
  ///   
  /// End Sub
  /// </code>
  /// </example>
		/// <param name="index">The index value of the bookmark to remove from the ObservableCollection.</param>
		public void DeleteBookmarkAt(int index)
		{
			Bookmarks.RemoveAt(index);
#if SILVERLIGHT
			SaveBookmarks();
#endif
		}
		/// <summary>
		/// Clears the bookmarks.
		/// </summary>
		public void ClearBookmarks()
		{
			Bookmarks.Clear();
#if SILVERLIGHT
			SaveBookmarks();
#endif
		}

#if SILVERLIGHT

		/// <summary>
		/// Gets or sets a value indicating whether to store the booksmarks in the isolated storage.
		/// </summary>
		/// <value><c>true</c> if bookmarks will be stored between sessions.</value>
		public bool UseIsolatedStorage { get; set; }
#endif

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get
			{
				return GetValue(TitleProperty) as string;
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		/// <summary>
		/// Identifies the <see cref="Title"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Bookmark), null);

	}
}
