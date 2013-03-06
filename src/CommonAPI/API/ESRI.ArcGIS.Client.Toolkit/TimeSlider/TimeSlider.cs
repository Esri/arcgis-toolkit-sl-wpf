// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace ESRI.ArcGIS.Client.Toolkit
{
 /// <summary>
 /// The TimeSlider is a utility Control that emits TimeExtent values typically for use with the Map Control 
 /// to enhance the viewing of geographic features that have attributes based upon Date/Time information.
 /// </summary>
 /// <remarks>
 /// <para>
 /// The TimeSlider Control is a User Interface (UI) control that looks and acts very similar to the buttons 
 /// that one would find on a DVD or MP3 player. The TimeSlider emits 
 /// <see cref="ESRI.ArcGIS.Client.TimeExtent">TimeExtent</see> values that are typically used in conjunction 
 /// with the Map Control to enhance the viewing of geographic features that have attributes based upon Date/Time 
 /// information. In most circumstances, the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> Property is bound to the 
 /// <see cref="ESRI.ArcGIS.Client.Map.TimeExtent">Map.TimeExtent</see> Property to control the visible geographic 
 /// features in a Layer that have temporal data. 
 /// </para>
 /// <para>
 /// <b>Note:</b> When a geographic feature has a TimeExtent that falls within the Map.TimeExtent, that feature 
 /// will be displayed. Conversely, when the TimeExtent of the geographic feature is no longer within the 
 /// Map.TimeExtent it is no longer displayed in the Map. 
 /// </para>
 /// <para>
 /// The TimeSlider Control can be created at design time in XAML or dynamically at runtime in the code-behind. 
 /// The TimeSlider Control is one of several controls available in the Visual Studio Toolbox when the 
 /// <ESRISILVERLIGHT>ArcGIS API for Silverlight</ESRISILVERLIGHT>
 /// <ESRIWPF>ArcGIS Runtime SDK for WPF</ESRIWPF>
 /// <ESRIWINPHONE>ArcGIS Runtime SDK for Windows Phone</ESRIWINPHONE>
 ///  is installed, see the following screen shot:
 /// </para>
 /// <ESRISILVERLIGHT><para><img border="0" alt="Example of the TimeSlider Control on the XAML design surface of a Silverlight application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.png"/></para></ESRISILVERLIGHT>
 /// <ESRIWPF><para><img border="0" alt="Example of the TimeSlider Control on the XAML design surface of a WPF application." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider_VS_WPF.png"/></para></ESRIWPF>
 /// <para>
 /// The default appearance of the TimeSlider Control can be modified using numerous inherited Properties from 
 /// System.Windows.FrameworkElement, System.Windows.UIElement, and System.Windows.Controls. An example of some 
 /// of these Properties include: .Height, .Width, .BackGround, .BorderBrush, .BorderThickness, .Foreground, 
 /// .HorizontalAlignment, .VerticalAlignment, .Margin, .Opacity, .Visibility, etc. 
 /// </para>
 /// <para>
 /// <b>Note:</b> you cannot change the core behavior of the sub-components (i.e. ToggleButton, Rectangle, Grid, Thumb, 
 /// Border, RepeatButton, Button, Path, etc.) of the TimeSlider Control using standard Properties or Methods 
 /// alone. To change the core behavior of the sub-components and their appearance in the Control, developers can 
 /// modify the Control Template in XAML and the associated code-behind file. The easiest way to modify the UI 
 /// sub-components is using Microsoft Expression Blend. Then developers can delete/modify existing or add new 
 /// sub-components in Visual Studio to create a truly customized experience. A general approach to customizing 
 /// a Control Template is discussed in the ESRI blog entitled: 
 /// <a href="http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx" target="_blank">Use control templates to customize the look and feel of ArcGIS controls</a>. 
 /// Specific code examples of modifying the Control Template of the TimeSlider Control to can be found in the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.IsPlaying">IsPlaying</see> and 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Loop">Loop</see> Properties documents.
 /// </para>
 /// <para>
 /// The TimeSlider Control is comprised of the following sub-components:
 /// </para>
 /// <para>
 /// <list type="bullet">
 /// <item>
 /// A TickBar with TickMarks to display the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Intervals">Intervals</see> of TimeExtent values
 /// </item>
 /// <item>
 /// A PlayPauseButton to start and stop the graphic animations 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.IsPlaying">playing</see> in the TickBar
 /// </item>
 /// <item>
 /// A MinimumThumb and MaximumThumb to denote the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">range</see> of 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeExtent values</see> being viewed along the 
 /// TickBar
 /// </item>
 /// <item>
 /// A NextButton to <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Next">advance</see> the thumb(s) forward 
 /// along the  intervals of TickBar
 /// </item>
 /// <item>
 /// A PreviousButton to <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Previous">reverse</see> the thumb(s) 
 /// back along the intervals of the TickBar
 /// </item>
 /// </list>
 /// </para>
 /// <para>
 /// The following image shows a typical TimeSlider Control and its various sub-component parts:
 /// </para>
 /// <para>
 /// <img border="0" alt="Describing the various parts of the TimeSlider Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider2.png"/>
 /// </para>
 /// <para>
 /// Clients have the ability to use the various Play, Pause, Next, and Previous buttons on the TimeSlider to 
 /// control the animation of which geographic features are displayed very similar to one watching a movie 
 /// on a DVD player or listening to music on and MP3 player. Users can also drag/slide the thumb(s) on the 
 /// TimeSlider to move to an exact position in the timeline of the TimeExtent for more control over feature 
 /// viewing.
 /// </para>
 /// <para>
 /// The bare minimum Properties that need to be set on the TimeSlider in order to have functioning interaction 
 /// between the TimeSlider and Map Controls are 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MinimumValue">MinimumValue</see>, 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MaximumValue">MaximumValue</see>, and 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">Value</see> (as well as binding the Map.TimeExtent 
 /// to the TimeSlider.Value). This will give a TimeSlider where the user can move the thumb(s) and have the Map 
 /// Control respond accordingly. But having only this bare minimum will not provide the full functionality of 
 /// the PlayPauseButton, NextButton, and PreviousButton being visible. In order to have the full functionality 
 /// of the TimeSlider the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Intervals">Intervals</see> Property 
 /// must also be set.
 /// </para>
 /// <para>
 /// The setting of the initial <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">Value</see> Property for 
 /// end users typically corresponds to the left-most TickMark position on the TickBar of the TimeSlider. This enables end 
 /// users to click the Play icon of the PlayPauseButton to begin an animation from the beginning of the series 
 /// of temporal geographic observations. There are several ways that a developer could determine what the 
 /// <a href="http://msdn.microsoft.com/en-us/library/03ybds8y(v=VS.95).aspx" target="_blank">Date/Time</a> of the 
 /// left-most TickMark position is; the following is a list of some common ways to determine this left-most 
 /// Date value:
 /// </para>
 /// <list type="bullet">
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MinimumValue">TimeSlider.MinimumValue</see> Property
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval">TimeSlider.CreateTimeStopsByTimeInterval.First</see> 
 ///   Property of the Static Function
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByCount">TimeSlider.CreateTimeStopsByCount.First</see> 
 ///   Property of the Static Function
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.FeatureLayer.TimeExtent">FeatureLayer.TimeExtent.Start</see> Property
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer.TimeExtent">ArcGISDynamicMapServiceLayer.TimeExtent.Start</see> 
 ///   Property
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer.TimeExtent">ArcGISImageServiceLayer.TimeExtent.Start</see> 
 ///   Property
 ///   </item>
 ///   <item>
 ///   Set it manually in XAML or code-behind to a known Date/Time
 ///   </item>
 ///   <item>
 ///   Obtain the initial start Date/Time of the TimeExtent from your own custom function (see the code example in 
 ///   the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">TimeSlider.TimeMode</see> Property documentation for 
 ///   obtaining the initial TimeSlider.Value from a subset of Graphics in a FeatureLayer rather that the 
 ///   FeatureLayer’s published TimeExtent.Start)
 ///   </item>
 /// </list>
 /// <para>
 /// <b>NOTE:</b> you use Date/Time value(s) to construct a TimeExtent object.)
 /// </para>
 /// <para>
 /// The TimeExtent object that is used by the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> 
 /// Property can be essentially constructed two ways: 
 /// (1) as an 'instance-in-time' or (2) as a 'period-of-time'. Depending on how the TimeExtent object is 
 /// constructed will have implications for how the TimeSlider behaves when the TimeSlider.Value is 
 /// set. It is important to remember that a Map Control that is bound to a TimeSlider will only display 
 /// geographic features that match the TimeSlider.Value Property. 
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">TimeSlider.TimeMode</see> Property also
 /// plays crucial role in how the TimeSlider behaves and setting the correct TimeExtent for the TimeSlider.Value
 /// will impact the TimeSlider.TimeMode setting.
 /// When using the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeMode">TimeMode</see> Enumerations of 
 /// <b>CumulativeFromStart</b> and <b>TimeExtent</b>, it is best 
 /// that you construct the TimeSlider.Value using a 'period-of-time' for the TimeExtent object. When using the 
 /// TimeMode Enumeration of <b>TimeInstant</b>, it is best that you construct the TimeSlider.Value using an 
 /// 'instance-in-time' for the TimeExtent object. By default the TimeSlider uses the TimeMode.CumulativeFromStart 
 /// as its setting. The code example in the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> 
 /// document demonstrates the subtle differences you may want to explore. 
 /// </para>
 /// <para>
 /// The following code examples demonstrate manually creating an 'instance-in-time' and 'period-of-time' for a 
 /// TimeExtent object:
 /// </para>
 /// <para>XAML:
 /// </para>
 /// <code>
 /// &lt;esri:TimeSlider Name="TimeSlider1" Value="2000/08/04 12:30:00 UTC" /&gt; &lt;!-- instance-in-time --&gt; 
 /// &lt;esri:TimeSlider Name="TimeSlider1" Value="2000/08/04 12:30:00 UTC, 2000/08/05 12:30:00 UTC" /&gt; &lt;!-- period-of-time --&gt;
 /// </code>
 /// <para>C#:
 /// </para>
 /// <code>
 /// TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(new DateTime(2000, 8, 4, 12, 30, 0)); // instance-in-time
 /// TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(new DateTime(2000, 8, 4, 12, 30, 0), new DateTime(2000, 8, 5, 12, 30, 0)); // period-of-time
 /// </code>
 /// <para>VB.NET:
 /// </para>
 /// <code>
 /// TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(New Date(2000, 8, 4, 12, 30, 0)) ' instance-in-time
 /// TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(New Date(2000, 8, 4, 12, 30, 0), New Date(2000, 8, 5, 12, 30, 0)) ' period-of-time
 /// </code>
 /// <para>
 /// A word of caution about using the code-behind TimeSlider.Loaded Event if you are trying to obtain the 
 /// TimeExtent Property of a Layer which contains temporal observations to calculate the TimeSlider.Value and 
 /// TimeSlider.Intervals; this will not work. The TimeSlider.Loaded event occurs before the various Layer 
 /// <b>.Initialized</b> Events occur so the Layer's <b>.TimeExtent</b> values will always be 
 /// Nothing/null. Using the TimeSlider.Loaded Event to determine the TimeSlider.Value and TimeSlider.Intervals 
 /// Properties is best when the TimeExtent values are known at design-time and the TimeSlider.Value and 
 /// TimeSlider.Intervals just need constructing in the code-behind.
 /// </para>
 /// <para>
 /// It is possible to set the TimeSlider.Value Property in XAML using Binding to another object. Just make sure 
 /// the object you are binding to returns a TimeExtent object. The following is a simplified XAML fragment where 
 /// the TimeSlider.Value is set to the TimeExtent of a FeatureLayer named 'EarthquakesLayer':
 /// <code>
 /// &lt;esri:TimeSlider Name="TimeSlider1" Value="{Binding ElementName=MyMap, Path=Layers[EarthquakesLayer].TimeExtent, Mode=OneWay}" /&gt;
 /// </code>
 /// </para>
 /// <para>
 /// The types of ArcGIS Server Layers that support Time Info (i.e. they have TimeExtent information) that are 
 /// useful with the TimeSlider are:
 /// <list type="bullet">
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer">ArcGISDynamicMapServiceLayer</see>
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer">ArcGISImageServiceLayer</see>
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.FeatureLayer">FeatureLayer</see>
 ///   </item>
 ///   <item>
 ///   <see cref="ESRI.ArcGIS.Client.GraphicsLayer">GraphicsLayer</see> (which are based upon 
 ///   <see cref="ESRI.ArcGIS.Client.Graphic">Graphic</see> elements)
 ///   </item>
 /// </list>
 /// </para>
 /// <para>
 /// <b>TIP: </b>For the ArcGISDynamicMapServiceLayer, ArcGISImageServiceLayer, and FeatureLayer you can tell 
 /// if TimeExtent information is available for the web service by copying the Url into the address bar of a web 
 /// browser and scrolling through the ArcGIS Server web service description and look for a 'Time Info' section. 
 /// See the following screen shot for a sample FeatureLayer:  
 /// </para>
 /// <para>
 /// <img border="0" alt="How to determine if an ArcGIS Server web service has temporal information." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.Value.png"/>
 /// </para>
 /// <para>
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.IsPlaying">IsPlaying</see> Property gets or set as 
 /// Boolean indicating whether the TimeSlider is playing through the Intervals of the TickBar. A value of True 
 /// means the animating of the thumb(s) is occurring. A value of False means no animation is occurring. The 
 /// default TimeSlider Control uses a ToggleButton for the PlayPauseButton to control the animations. When the 
 /// Pause icon is visible that means animation is occurring (i.e. IsPlaying=True). When the Play icon is visible, 
 /// that means the animation has stopped (i.e. IsPlaying=False). See the following screen shot for a visual 
 /// depiction:
 /// </para>
 /// <para>
 /// <img border="0" alt="The PlayPauseButton and the IsPlaying Property of the TimeSlider Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.IsPlaying.png"/>
 /// </para>
 /// <para>
 /// Under most circumstances it is not necessary to set the IsPlaying Property via code as this Property is 
 /// automatically managed as part of the default TimeSlider Control. It is possible to modify the default 
 /// behavior of the PlayPauseButton sub-component of the TimeSlider by editing it’s Control Template. The 
 /// example code in the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.IsPlaying">IsPlaying</see> Property 
 /// demonstrates a code example of using the set operation if customization is desired.  
 /// </para>
 /// <para>The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.PlaySpeed">TimeSlider.PlaySpeed</see> Property 
 /// uses a <a href="http://msdn.microsoft.com/en-us/library/system.timespan(v=VS.95).aspx" target="_blank">TimeSpan</a> 
 /// Structure to define how quickly the thumb moves across the tick marks on the TimeSlider. The default 
 /// PlaySpeed is 1 second.
 /// </para>
 /// <para>
 /// The PlaySpeed Attribute in XAML follows the format of "hh:mm:ss" where, hh = hours (0 to 24), mm = minutes 
 /// (0 to 60), and ss = seconds (0 to 60). You can have fractions of a second by using decimal values. In the 
 /// following XAML code fragment the PlaySpeed increments the time intervals every tenth of a second (i.e. 0.1).
 /// </para>
 /// <para>
 /// <code lang="XAML">
 /// &lt;esri:TimeSlider Name="TimeSlider1" PlaySpeed="00:00:00.1"/&gt;
 /// </code>
 /// </para>
 /// <para>
 /// In the code-behind there are several ways to make a TimeSpan using the constructor. The following code 
 /// example(s) provides a few different examples that also produce the same time intervals every tenth of a 
 /// second (i.e. 0.1):
 /// </para>
 /// <para>
 /// VB.NET:
 /// </para>
 /// <para>
 /// <code lang="VB.NET">
 /// TimeSlider1.PlaySpeed = New TimeSpan(100000) ' ticks
 /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0.1) ' hours, minutes, seconds
 /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0.1) ' days, hours, minutes, seconds
 /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0, 10) ' days, hours, minutes, seconds, milliseconds
 /// </code>
 /// </para>
 /// <para>
 /// C#:
 /// </para>
 /// <para>
 /// <code lang="CS">
 /// TimeSlider1.PlaySpeed = new TimeSpan(100000); // ticks
 /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0.1); // hours, minutes, seconds
 /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0, 0.1); // days, hours, minutes, seconds
 /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0, 0, 10); // days, hours, minutes, seconds, milliseconds
 /// </code>
 /// </para>
 /// <para>
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">TimeSlider.TimeMode</see> Property determines 
 /// how the TimeSlider Control will represent a TimeExtent as the thumb(s) moves across the tick marks. 
 /// </para>
 /// <para>
 /// The TimeExtent that is shown visually in the TimeSlider Control is corresponds to the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> Property. The TimeMode 
 /// Property is set to one of three Enumerations:
 /// </para>
 /// <para>
 /// <list type="bullet">
 /// <item>
 /// <b>TimeMode.CumulativeFromStart</b>: This TimeMode setting will draw all observations on the Map from the 
 /// oldest to the current as the MaximumThumb crosses a tic mark. It draws observations in a 'cumulative' effect. 
 /// As the animation occurs, the TimeSlider.Value.Start will remain static but the TimeSlider.Value.End will 
 /// constantly change. The MinimumThumb is not shown and is implied to be the same as the TimeSlider.Value.Start. 
 /// The color in the Foreground of the TimeSlider will shade from the TimeSlider.Value.Start through the 
 /// TimeSlider.Value.End as animation occurs. This is the default TimeMode setting on the TimeSlider. See the 
 /// following screen shot:
 /// <para>
 /// <img border="0" alt="The TimeSlider TimeMode.CumulativeFromStart." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.CumulativeFromStart.png"/>
 /// </para>
 /// </item>
 /// <item>
 /// <b>TimeMode.TimeExtent</b>: This TimeMode setting will draw only those observations on the Map that fall in 
 /// a 'window of time' as defined by the TimeSlider.Value. As the thumbs (MinimumThumb and MaximumThumb) of the 
 /// 'window of time' move across the tick marks the older observations will disappear and newer ones will appear. 
 /// As the animation occurs, the TimeSlider.Value.Start (analogous to the MinimumThumb) will constantly change and 
 /// the TimeSlider.Value.End (analogous to the MaximumThumb) will constantly change but the two difference between 
 /// the Start and End times will be fixed. The Foreground shading of the TimeSlider will display in between the 
 /// two thumbs as the animation occurs. See the following screen shot:
 /// <para>
 /// <img border="0" alt="The TimeSlider TimeMode.TimeExtent." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.TimeExtent.png"/>
 /// </para>
 /// </item>
 /// <item>
 /// <b>TimeMode.TimeInstant</b>: This TimeMode setting will draw only the most recent observations on the Map as 
 /// the thumb(s) (MinimumThumb and MaximumThumb) crosses a tic mark. It draws observations in a 'snapshot' effect. 
 /// As the animation occurs, the TimeSlider.Value.Start and the TimeSlider.Value.End will be exactly the same but 
 /// values will change continuously. The MinimumThumb and MaximumThumb overlap each other exactly and give the 
 /// visual appearance of only one thumb being show to demonstrate the snapshot effect. No Foreground shading 
 /// occurs in the TimeSlider in the TimeMode.TimeInstant setting. See the following screen shot:
 /// <para>
 /// <img border="0" alt="The TimeSlider TimeMode.TimeInstant." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.TimeInstant.png"/>
 /// </para>
 /// </item>
 /// </list>
 /// </para>
 /// <para>
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Loop">TimeSlider.Loop</see> Property gets or sets a value 
 /// indicating whether the animating of the TimeSlider thumb(s) will restart playing when the end of the TickBar 
 /// is reached. The Loop Property of True allows continuous playing of the thumb(s) across the TickBar of the 
 /// TimeSlider. A False value means the thumb(s) will stop at the last Interval (i.e. the 
 /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MaximumValue">MaximumValue</see>) in the TickBar when it 
 /// is reached. The default Loop value of the TimeSlider is False.
 /// </para>
 /// <para>
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MinimumValue">TimeSlider.MinimumValue</see> Property 
 /// specifies the starting Date/Time of the TimeSlider track.
 /// </para>
 /// <para>
 /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MaximumValue">TimeSlider.MaximumValue</see> Property 
 /// specifies the ending Date/Time of the TimeSlider track.
 /// </para>
 /// </remarks>
 /// <example>
 /// <para>
 /// <b>How to use:</b>
 /// </para>
 /// <para>
 /// Click the 'Initialize the TimeSlider' button to initialize the TimeSlider functions (PlayPause, Next, 
 /// Previous) to display the observed oil spill and projected oil spill plume trajectory. Then click the play 
 /// button to watch the animation. The black polygons represent actual oil spill observations by day. The gray 
 /// polygons represent the area of the oil spill plume trajectory. As time moves forward in the example, the 
 /// gray polygons will merge into one large polygon that is the cumulative trajectory over the course of the 
 /// oil spill.
 /// </para>
 /// <para>
 /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
 /// the functionality.
 /// </para>
 /// <para>
 /// The following screen shot corresponds to the code example in this page.
 /// </para>
 /// <para>
 /// <img border="0" alt="Using the TimeSlider Control to display oil spill observations and trajectory of the oil plume." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider3.png"/>
 /// </para>
 /// <code title="Example XAML1" description="" lang="XAML">
 /// &lt;Grid x:Name="LayoutRoot"&gt;
 /// 
 ///   &lt;!--
 ///   Add a Map control with an ArcGISTiledMapServiceLayer and a FeatureLayer. The Map.TimeExtent is
 ///   bound to the MyTimeSlider (TimeSlider) control. The MyTimeSlider will control what TimeExtent (i.e.
 ///   slices of time) can be viewed in the Map control.
 ///           
 ///   The FeatureLayer will contain several observations based upon Polygon geometries which have various 
 ///   TimeExtent values set. When the specific features have a TimeExtent that falls within the 
 ///   Map.TimeExtent will they be displayed based upon the MyTimeSlider settings.
 ///   --&gt;
 ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,150,0,0" Name="Map1" 
 ///             VerticalAlignment="Top" Height="318" Width="341" 
 ///             Extent="-92.65,25.64,-85.53,32.28"
 ///             TimeExtent="{Binding ElementName=MyTimeSlider, Path=Value}"&gt;
 ///   
 ///     &lt;esri:ArcGISTiledMapServiceLayer ID="PhysicalTiledLayer" 
 ///                  Url="http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer" /&gt;
 ///     
 ///     &lt;!-- The FeatureLayer displays oil slick information in the Gulf of Mexico by day from the NOAA. --&gt;
 ///     &lt;esri:FeatureLayer ID="MyFeatureLayer"
 ///                      Url="http://servicesbeta.esri.com/ArcGIS/rest/services/GulfOilSpill/Gulf_Oil_Spill/MapServer/0"
 ///                      OutFields="*"&gt;
 ///         
 ///       &lt;esri:FeatureLayer.Renderer&gt;
 ///         &lt;esri:TemporalRenderer&gt;
 ///           
 ///           &lt;!-- 
 ///           The Gray polygons display the projected trajectory of the oil slick.
 ///           The Black polygons display actual observed oil on the water.
 ///           --&gt;
 ///             
 ///           &lt;esri:TemporalRenderer.ObservationRenderer&gt;
 ///             &lt;esri:SimpleRenderer&gt;
 ///               &lt;esri:FillSymbol Fill="Gray" BorderBrush="Gray"/&gt;
 ///             &lt;/esri:SimpleRenderer&gt;
 ///           &lt;/esri:TemporalRenderer.ObservationRenderer&gt;
 ///                         
 ///           &lt;esri:TemporalRenderer.LatestObservationRenderer&gt;
 ///             &lt;esri:SimpleRenderer&gt;
 ///               &lt;esri:FillSymbol Fill="Black" BorderBrush="Black"/&gt;
 ///             &lt;/esri:SimpleRenderer&gt;
 ///           &lt;/esri:TemporalRenderer.LatestObservationRenderer&gt;
 ///             
 ///         &lt;/esri:TemporalRenderer&gt;
 ///       &lt;/esri:FeatureLayer.Renderer&gt;
 ///     &lt;/esri:FeatureLayer&gt;
 /// 
 ///   &lt;/esri:Map&gt;
 /// 
 ///   &lt;!-- 
 ///   Add a TimeSlider to control the display of what geographic features are displayed in the Map Control
 ///   based upon a specified TimeExtent. In the case of this sample code, when the specific features 
 ///   have a TimeExtent that falls within the Map.TimeExtent will they be displayed based upon the MyTimeSlider 
 ///   settings.
 ///     
 ///   Tip: It is the x:Name Attribute that allows you to access the TimeSlider Control in the code-behind file. 
 ///  
 ///   The PlaySpeed Attribute follows the format of "hh:mm:ss" where, hh = hours (0 to 24), mm = minutes (0 to 60),
 ///   and ss = seconds (0 to 60). In this example the PlaySpeed increments the time intervals every half a second
 ///   (i.e. 0.5).
 ///       
 ///   The TimeMode Attribute of CumulativeFromStart means there is a fixed start date/time (2010/05/24 20:00:00 UTC
 ///   in this example) that does not change and an end date/time that adjusts as the specified Interval (one day 
 ///   in this example which is set in the code-behind) increases. As the 'thumb' of the TimeSlider control moves
 ///   to the right along the slider track the TimeExtent Interval of the TimeSlider increases.
 ///       
 ///   The MinimumValue Attribute specifies the starting date/time of the TimeSlider track.
 ///       
 ///   The MaximumValue Attribute specifies the ending date/time of the TimeSlider track.
 ///     
 ///   The Value Attribute specifies the date/time location of the thumb along the TimeSlider track. The thumb can 
 ///   have a start date/time and end date/time set for a TimeExtent which will display a window of time as the
 ///   thumb moves along the TimeSlider track but this is best for TimeMode Attribute of 'TimeExtent'. Since
 ///   this example is showing a TimeMode of 'CumulativeFromStart' it is best to have the thumb just use a single
 ///   date/time specified for the Value set to the same date/time as the MinimumValue.
 ///     
 ///   The last thing needed to enable the full capabilities of a TimeSlider (i.e. having the PlayPause, Next, 
 ///   and Previous buttons) is to set the Intervals Property. In Silverlight, this can only be done in code-behind to 
 ///   construct the Collection Type of IEnumerable(Of Date). Without the TimeSlider.Intervals being set the user 
 ///   has to manually move the thumb across the TimeSlider track to change the Map.TimeExtent and thereby see what 
 ///  Graphics can be displayed for that date/time window.
 ///   --&gt;
 ///   &lt;esri:TimeSlider x:Name="MyTimeSlider" 
 ///                     MinimumValue="2010/05/24 20:00:00 UTC"
 ///                     MaximumValue="2010/08/05 20:00:00 UTC"
 ///                     Height="22" Margin="12,122,0,0" 
 ///                     Value="2010/05/24 20:00:00 UTC"
 ///                     TimeMode="CumulativeFromStart"
 ///                     PlaySpeed="0:0:0.5"
 ///                     HorizontalAlignment="Left" VerticalAlignment="Top" Width="341"/&gt;
 /// 
 ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
 ///   &lt;TextBlock Height="75" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="455" 
 ///            TextWrapping="Wrap" Margin="12,12,0,0" 
 ///            Text="Click the 'Initialize the TImeSlider' button to initialize the TimeSlider functions 
 ///              (PlayPause, Next, Previous) to display the observed oil spill and projected oil spill plume 
 ///              trajectory. Then click the play button to watch the animation. The black polygons represent 
 ///              actual oil spill observations by day. The gray polygons represent the area of the oil spill 
 ///              plume trajectory. As time moves forward in the example the gray polygons will merge into one
 ///              large polygon that is the cumulative trajectory over the course of the oil spill." /&gt;
 /// 
 ///   &lt;!-- Add a button to perform the work. --&gt;
 ///   &lt;Button Content="Initialize the TimeSlider" Height="23" HorizontalAlignment="Left" 
 ///         Margin="77,93,0,0" Name="Button1" VerticalAlignment="Top" Width="185" Click="Button1_Click"/&gt;
 ///     
 /// &lt;/Grid&gt;
 /// </code>
 /// <code title="Example CS1" description="" lang="CS">
 /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
 /// {
 ///   // This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
 ///   // are a Collection of IEnumerable&lt;DateTime&gt; objects. When the TimeSlider.Intervals Property is set along with 
 ///   // the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
 ///   // functionality includes buttons for Play, Pause, Forward, and Back.
 ///   
 ///   // Obtain the start and end DateTime values from the TimeSlider named MyTimeSlider that was defined in XAML.
 ///   DateTime myMinimumDate = MyTimeSlider.MinimumValue;
 ///   DateTime myMaximumDate = MyTimeSlider.MaximumValue;
 ///   
 ///   // Create a TimeExtent based upon the start and end date/times.
 ///   ESRI.ArcGIS.Client.TimeExtent myTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate);
 ///   
 ///   // Create a new TimeSpan (1 day in our case).
 ///   TimeSpan myTimeSpan = new TimeSpan(1, 0, 0, 0);
 ///   
 ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
 ///   System.Collections.Generic.IEnumerable&lt;DateTime&gt; myIEnumerableDates = null;
 ///   
 ///   // Load all of Dates into the Collection of IEnumerable&lt;DateTime&gt; objects using the 
 ///   // TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
 ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan);
 ///   
 ///   // Set the TimeSlider.Intervals which define the tick marks along the TimSlider track to the IEnumerable&lt;DateTime&gt; 
 ///   // objects.
 ///   MyTimeSlider.Intervals = myIEnumerableDates;
 /// }
 /// </code>
 /// <code title="Example VB1" description="" lang="VB.NET">
 /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
 ///   
 ///   ' This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
 ///   ' are a Collection of IEnumerable(Of Date) objects. When the TimeSlider.Intervals Property is set along with 
 ///   ' the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
 ///   ' functionality includes buttons for Play, Pause, Forward, and Back.
 ///   
 ///   ' Obtain the start and end Date/Time values from the TimeSlider named MyTimeSlider that was defined in XAML.
 ///   Dim myMinimumDate As Date = MyTimeSlider.MinimumValue
 ///   Dim myMaximumDate As Date = MyTimeSlider.MaximumValue
 ///   
 ///   ' Create a TimeExtent based upon the start and end date/times.
 ///   Dim myTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate)
 ///   
 ///   ' Create a new TimeSpan (1 day in our case).
 ///   Dim myTimeSpan As New TimeSpan(1, 0, 0, 0)
 ///   
 ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
 ///   Dim myIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
 ///   
 ///   ' Load all of Dates into the Collection of IEnumerable(Of Date) objects using the 
 ///   ' TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
 ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan)
 ///   
 ///   ' Set the TimeSlider.Intervals which define the tick marks along the TimSlider track to the IEnumerable(Of Date) 
 ///   ' objects.
 ///   MyTimeSlider.Intervals = myIEnumerableDates
 ///   
 /// End Sub
 /// </code>
 /// </example>
	[TemplatePart(Name = "HorizontalTrack", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "HorizontalTrackThumb", Type = typeof(Thumb))]
	[TemplatePart(Name = "MinimumThumb", Type = typeof(Thumb))]
	[TemplatePart(Name = "MaximumThumb", Type = typeof(Thumb))]
	[TemplatePart(Name = "TickMarks", Type = typeof(ESRI.ArcGIS.Client.Toolkit.Primitives.TickBar))]
	[TemplatePart(Name = "HorizontalTrackLargeChangeDecreaseRepeatButton", Type = typeof(RepeatButton))]
	[TemplatePart(Name = "HorizontalTrackLargeChangeIncreaseRepeatButton", Type = typeof(RepeatButton))]
	[TemplatePart(Name = "PlayPauseButton", Type = typeof(ToggleButton))]
	[TemplatePart(Name = "NextButton", Type = typeof(ButtonBase))]
	[TemplatePart(Name = "PreviousButton", Type = typeof(ButtonBase))]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
	public class TimeSlider : Control
	{
		#region Fields
		FrameworkElement SliderTrack;
		Thumb MinimumThumb;
		Thumb MaximumThumb;
		Thumb HorizontalTrackThumb;
		RepeatButton ElementHorizontalLargeDecrease;
		RepeatButton ElementHorizontalLargeIncrease;
		ToggleButton PlayPauseButton;
		ButtonBase NextButton;
		ButtonBase PreviousButton;
		ESRI.ArcGIS.Client.Toolkit.Primitives.TickBar TickMarks;
		DispatcherTimer playTimer;
		private TimeExtent currentValue;
		private bool isFocused;
		private bool isMouseOver;
		private double totalHorizontalChange;
		private TimeExtent HorizontalChangeExtent;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSlider"/> class.
		/// </summary>
		public TimeSlider()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(TimeSlider);
#endif
			Intervals = new ObservableCollection<DateTime>();
			playTimer = new DispatcherTimer() { Interval = this.PlaySpeed };
			playTimer.Tick += playTimer_Tick;
			SizeChanged += TimeSlider_SizeChanged;
		}

		private void TimeSlider_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateTrackLayout(ValidValue);
		}
		/// <summary>
		/// Static initialization for the <see cref="TimeSlider"/> control.
		/// </summary>
		static TimeSlider()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSlider),
				new FrameworkPropertyMetadata(typeof(TimeSlider)));
#endif
		}
		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			SliderTrack = GetTemplateChild("HorizontalTrack") as FrameworkElement;
			if(SliderTrack != null)
				SliderTrack.SizeChanged +=  TimeSlider_SizeChanged;
			HorizontalTrackThumb = GetTemplateChild("HorizontalTrackThumb") as Thumb;
			MinimumThumb = GetTemplateChild("MinimumThumb") as Thumb;
			MaximumThumb = GetTemplateChild("MaximumThumb") as Thumb;
			ElementHorizontalLargeDecrease = GetTemplateChild("HorizontalTrackLargeChangeDecreaseRepeatButton") as RepeatButton;
			ElementHorizontalLargeIncrease = GetTemplateChild("HorizontalTrackLargeChangeIncreaseRepeatButton") as RepeatButton;
			PlayPauseButton = GetTemplateChild("PlayPauseButton") as ToggleButton;
			NextButton = GetTemplateChild("NextButton") as ButtonBase;
			PreviousButton = GetTemplateChild("PreviousButton") as ButtonBase;
			TickMarks = GetTemplateChild("TickMarks") as ESRI.ArcGIS.Client.Toolkit.Primitives.TickBar;
			if (MinimumThumb != null)
			{
				MinimumThumb.DragDelta += MinimumThumb_DragDelta;
				MinimumThumb.DragCompleted += DragCompleted;
				MinimumThumb.DragStarted += (s, e) => { Focus(); };
			}
			if (MaximumThumb != null)
			{
				MaximumThumb.DragDelta += MaximumThumb_DragDelta;
				MaximumThumb.DragCompleted += DragCompleted;
				MaximumThumb.DragStarted += (s, e) => { Focus(); };
			}
			if (HorizontalTrackThumb != null)
			{
				HorizontalTrackThumb.DragDelta += HorizontalTrackThumb_DragDelta;
				HorizontalTrackThumb.DragCompleted += DragCompleted;
				HorizontalTrackThumb.DragStarted += (s, e) => {	Focus(); };				
			}
			if (ElementHorizontalLargeDecrease != null)
			{
				ElementHorizontalLargeDecrease.Click += (s, e) => { Focus(); if (IsPlaying) { IsPlaying = false; } Previous(); };
			}
			if (ElementHorizontalLargeIncrease != null)
			{
				ElementHorizontalLargeIncrease.Click += (s, e) => { Focus(); if (IsPlaying) { IsPlaying = false; } Next(); };
			}
			if (PlayPauseButton!=null)
			{
				this.IsPlaying = PlayPauseButton.IsChecked.Value;
				PlayPauseButton.Checked += (s,e) => { this.IsPlaying = true; };
				PlayPauseButton.Unchecked += (s, e) => { this.IsPlaying = false; };
			}
			if (NextButton != null)
			{
				NextButton.Click += (s, e) => { this.Next(); };
			}
			if (PreviousButton != null)
			{
				PreviousButton.Click += (s, e) => { this.Previous(); };
			}
			CreateTickmarks();
			SetButtonVisibility();			
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			isFocused = false;
			ChangeVisualState(true);
		}
		
		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			isFocused = true;
			ChangeVisualState(true);
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			isMouseOver = true;
			if (this.MinimumThumb != null && !this.MinimumThumb.IsDragging ||
				this.MaximumThumb != null && !this.MaximumThumb.IsDragging ||
				this.MinimumThumb == null && this.MaximumThumb == null)
			{
				ChangeVisualState(true);
			}
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			isMouseOver = false;
			if (this.MinimumThumb != null && !this.MinimumThumb.IsDragging ||
				this.MaximumThumb != null && !this.MaximumThumb.IsDragging ||
				this.MinimumThumb == null && this.MaximumThumb == null)
			{
				ChangeVisualState(true);
			}
		}
		
		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if(!e.Handled && IsEnabled)
			{
				Focus();
			}
		}
		
		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && IsEnabled)
			{
				if ((e.Key == Key.Left))
				{
					Previous();
				}
				else if ((e.Key == Key.Right))
				{
					Next();
				}
			}
		}

		#endregion

		private void ChangeVisualState(bool useTransitions)
		{
			//CommonStates
			if (!base.IsEnabled)
			{
				VisualStateManager.GoToState(this, "Disabled", useTransitions);
			}
			else if (this.isMouseOver)
			{
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}
			//FocusStates
			if (this.isFocused && base.IsEnabled)
			{
				VisualStateManager.GoToState(this, "Focused", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Unfocused", useTransitions);
			}
		}

		private void UpdateTrackLayout(TimeExtent extent)
		{						
			if (extent == null || extent.Start < MinimumValue || extent.End > MaximumValue ||
				MinimumThumb == null || MaximumThumb == null || MaximumValue <=
				MinimumValue || SliderTrack == null) 
				return;

			TimeMode timeMode = this.TimeMode;			
			double sliderWidth = SliderTrack.ActualWidth;
			double minimum = MinimumValue.Ticks;
			double maximum = MaximumValue.Ticks;

			// time			
			TimeExtent snapped = Snap(extent,false);
			double start = snapped.Start.Ticks;
			double end = snapped.End.Ticks;
				
			//margins 
			double left = 0;
			double right = 0;
			bool hasIntervals = (Intervals == null) ? false : Intervals.GetEnumerator().MoveNext();
			
			// rate = (distance) / (time)				
			double rate =  GetTrackWidth() / (maximum - minimum);

			if (timeMode == TimeMode.TimeExtent && !hasIntervals)
			{												
				//left repeater	
				right = Math.Min(sliderWidth,((maximum - start) * rate) + MinimumThumb.ActualWidth + MaximumThumb.ActualWidth);
				ElementHorizontalLargeDecrease.Margin = new Thickness(0, 0, right, 0);												

				//minimum thumb
				left = Math.Min(sliderWidth, (start - minimum) * rate);
				right = Math.Min(sliderWidth, ((maximum - start) * rate) + MaximumThumb.ActualWidth);
				MinimumThumb.Margin = new Thickness(left, 0, right, 0);

				//middle thumb
				left = Math.Min(sliderWidth, ((start - minimum) * rate) + MinimumThumb.ActualWidth);
				right = Math.Min(sliderWidth, (maximum - end) * rate + MaximumThumb.ActualWidth);
				HorizontalTrackThumb.Margin = new Thickness(left, 0, right, 0);
				HorizontalTrackThumb.Width = Math.Max(0, (sliderWidth - right - left));

				//maximum thumb
				left = Math.Min(sliderWidth, (end - minimum) * rate + MinimumThumb.ActualWidth);
				right = Math.Min(sliderWidth, ((maximum - end) * rate));
				MaximumThumb.Margin = new Thickness(left, 0, right, 0);

				//right repeater
				left = Math.Min(sliderWidth, ((end - minimum) * rate) + MinimumThumb.ActualWidth + MaximumThumb.ActualWidth);
				ElementHorizontalLargeIncrease.Margin = new Thickness(left, 0, 0, 0);
			}
			else if (hasIntervals) //one or two thumbs
			{				
				//left repeater								
				right = Math.Min(sliderWidth, ((maximum - start) * rate) + MaximumThumb.ActualWidth);
				ElementHorizontalLargeDecrease.Margin = new Thickness(0, 0, right, 0);

				//minimum thumb
				if (timeMode == TimeMode.TimeExtent)
				{
					left = Math.Min(sliderWidth, (start - minimum) * rate);
					right = Math.Min(sliderWidth, ((maximum - start) * rate));
					MinimumThumb.Margin = new Thickness(left, 0, right, 0);
				}
				else
				{
					MinimumThumb.Margin = new Thickness(0, 0, sliderWidth, 0);
				}

				//middle thumb
				if (timeMode == TimeMode.TimeInstant)
				{
					HorizontalTrackThumb.Margin = new Thickness(0, 0, sliderWidth, 0);
					HorizontalTrackThumb.Width = 0;
				}
				else if(timeMode == TimeMode.TimeExtent)
				{
					left = Math.Min(sliderWidth, ((start - minimum) * rate) + MinimumThumb.ActualWidth);
					right = Math.Min(sliderWidth, (maximum - end) * rate + MaximumThumb.ActualWidth);
					HorizontalTrackThumb.Margin = new Thickness(left, 0, right, 0);
					HorizontalTrackThumb.Width = Math.Max(0, (sliderWidth - right - left));
					HorizontalTrackThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				}
				else
				{
					right = Math.Min(sliderWidth, ((maximum - end) * rate) + MaximumThumb.ActualWidth);
					HorizontalTrackThumb.Margin = new Thickness(0, 0, right, 0);
					HorizontalTrackThumb.Width = (sliderWidth - right);
					HorizontalTrackThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				}

				//maximum thumb
				left = Math.Min(sliderWidth,(end - minimum) * rate);
				right = Math.Min(sliderWidth,((maximum - end) * rate));
				MaximumThumb.Margin = new Thickness(left, 0, right, 0);

				//right repeater
				left = Math.Min(sliderWidth,((end - minimum) * rate) + MaximumThumb.ActualWidth);
				ElementHorizontalLargeIncrease.Margin = new Thickness(left, 0, 0, 0);				
			}
			else //no intervals, one thumb or two thumbs where start==end
			{				
				//left repeater				
				right = Math.Min(sliderWidth,((maximum - end) * rate) + MaximumThumb.ActualWidth);
				ElementHorizontalLargeDecrease.Margin = new Thickness(0, 0, right, 0);

				//minimum thumb
				MinimumThumb.Margin = new Thickness(0, 0, sliderWidth, 0);

				//middle thumb
				if (timeMode == TimeMode.TimeInstant)
				{
					HorizontalTrackThumb.Margin = new Thickness(0, 0, sliderWidth, 0);
					HorizontalTrackThumb.Width = 0;
				}
				else
				{
					HorizontalTrackThumb.Margin = new Thickness(0, 0, right, 0);
					HorizontalTrackThumb.Width = (sliderWidth - right);
					HorizontalTrackThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				}

				//maximum thumb
				left = Math.Min(sliderWidth,(end - minimum) * rate);
				right = Math.Min(sliderWidth,((maximum - end) * rate));
				MaximumThumb.Margin = new Thickness(left, 0, right, 0);

				//right repeater
				left = Math.Min(sliderWidth,((end - minimum) * rate) + MaximumThumb.ActualWidth);
				ElementHorizontalLargeIncrease.Margin = new Thickness(left, 0, 0, 0);
			}
		}

		#region Drag event handlers
		
		private void HorizontalTrackThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			if (IsPlaying) IsPlaying = false;
			if (e.HorizontalChange == 0 || TimeMode == TimeMode.CumulativeFromStart) return;
			if (currentValue == null) currentValue = ValidValue;
#if SILVERLIGHT
			totalHorizontalChange += e.HorizontalChange;
			if (HorizontalChangeExtent == null)
				HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#else
			totalHorizontalChange = e.HorizontalChange;
			HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#endif
			// time ratio 
			long TimeRate = (MaximumValue.Ticks - MinimumValue.Ticks) / (long)GetTrackWidth();

			// time change
			long TimeChange = (long)(TimeRate * totalHorizontalChange);			
			
			TimeSpan difference = new TimeSpan(currentValue.End.Ticks - currentValue.Start.Ticks);

			TimeExtent tempChange = null;					
			try
			{
				tempChange = new TimeExtent(HorizontalChangeExtent.Start.AddTicks(TimeChange),
					HorizontalChangeExtent.End.AddTicks(TimeChange));
			}
			catch (ArgumentOutOfRangeException)
			{
				if (totalHorizontalChange < 0)
					tempChange = new TimeExtent(MinimumValue, MinimumValue.Add(difference));
				else if (totalHorizontalChange > 0 )
					tempChange = new TimeExtent(MaximumValue.Subtract(difference),MaximumValue);
			}

			if (tempChange.Start.Ticks < MinimumValue.Ticks)
				currentValue = Snap(new TimeExtent(MinimumValue, MinimumValue.Add(difference)), true);
			else if (tempChange.End.Ticks > MaximumValue.Ticks)
				currentValue = Snap(new TimeExtent(MaximumValue.Subtract(difference), MaximumValue), true);
			else
				currentValue = Snap(new TimeExtent(tempChange.Start, tempChange.End), true);

			UpdateTrackLayout(currentValue);
		}

		private void MinimumThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			if (IsPlaying) IsPlaying = false;
			if (e.HorizontalChange == 0) return;
			if (currentValue == null) currentValue = ValidValue;
#if SILVERLIGHT
			totalHorizontalChange += e.HorizontalChange;
			if (HorizontalChangeExtent == null)
				HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#else
			totalHorizontalChange = e.HorizontalChange;
			HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#endif
			// time ratio 
			long TimeRate = (MaximumValue.Ticks - MinimumValue.Ticks) / (long)GetTrackWidth();

			// time change
			long TimeChange = (long)(TimeRate * totalHorizontalChange);
			
			TimeExtent tempChange = null;
			try
			{
				tempChange = new TimeExtent(HorizontalChangeExtent.Start.AddTicks(TimeChange), HorizontalChangeExtent.End);
			}
			catch (ArgumentOutOfRangeException)
			{
				if(totalHorizontalChange < 0) 
					tempChange = new TimeExtent(MinimumValue,currentValue.End); 
				else if(totalHorizontalChange > 0) 
					tempChange = new TimeExtent(currentValue.End);
			}

			if (tempChange.Start.Ticks < MinimumValue.Ticks)
				currentValue = Snap(new TimeExtent(MinimumValue, currentValue.End), false);
			else if (tempChange.Start >= currentValue.End)
				currentValue = Snap(new TimeExtent(currentValue.End), false);
			else
				currentValue = Snap(new TimeExtent(tempChange.Start, tempChange.End), false);	

			UpdateTrackLayout(currentValue);
		}

		private void MaximumThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			if (IsPlaying) IsPlaying = false;
			if (e.HorizontalChange == 0) return;
			if (currentValue == null) currentValue = ValidValue;
#if SILVERLIGHT
			totalHorizontalChange += e.HorizontalChange;
			if (HorizontalChangeExtent == null)
				HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#else
			totalHorizontalChange = e.HorizontalChange;			
			HorizontalChangeExtent = new TimeExtent(currentValue.Start, currentValue.End);
#endif
			// time ratio 
			long TimeRate = (MaximumValue.Ticks - MinimumValue.Ticks) / (long)GetTrackWidth();
			
			// time change
			long TimeChange = (long)(TimeRate * totalHorizontalChange);

			TimeExtent tempChange = null;
			if (TimeMode == TimeMode.TimeInstant)
			{
				try
				{
					// If the mouse drag creates a date thats year is before 
					// 1/1/0001 or after 12/31/9999 then an out of renge 
					// exception will be trown.
					tempChange = new TimeExtent(HorizontalChangeExtent.End.AddTicks(TimeChange));
				}
				catch (ArgumentOutOfRangeException)
				{
					if (totalHorizontalChange > 0) // date is after 12/31/9999
						tempChange = new TimeExtent(MaximumValue);
					else if (totalHorizontalChange < 0) // date is before 1/1/0001
						tempChange = new TimeExtent(MinimumValue);					
				}
			}
			else
			{
				try
				{
					// If the mouse drag creates a date thats year is before 
					// 1/1/0001 or after 12/31/9999 then an out of renge 
					// exception will be trown.
					tempChange = new TimeExtent(HorizontalChangeExtent.Start, HorizontalChangeExtent.End.AddTicks(TimeChange));
				}
				catch (ArgumentOutOfRangeException)
				{
					if (totalHorizontalChange > 0) // date is after 12/31/9999
						tempChange = new TimeExtent(currentValue.Start, MaximumValue);
					else if (totalHorizontalChange < 0) // date is before 1/1/0001
 						tempChange = new TimeExtent(currentValue.Start);					
				}
			}

			// validate change
			if (TimeMode == TimeMode.TimeInstant)
			{
				if (tempChange.End.Ticks > MaximumValue.Ticks)
					currentValue = Snap(new TimeExtent(MaximumValue), false);
				else if (tempChange.End.Ticks < MinimumValue.Ticks)
					currentValue = Snap(new TimeExtent(MinimumValue), false);
				else
					currentValue = Snap(new TimeExtent(tempChange.End), false);
			}
			else
			{
				if (tempChange.End.Ticks > MaximumValue.Ticks)
					currentValue = Snap(new TimeExtent(currentValue.Start, MaximumValue), false);
				else if (tempChange.End <= currentValue.Start && TimeMode == TimeMode.TimeExtent )
					currentValue = Snap(new TimeExtent(currentValue.Start, currentValue.Start.AddMilliseconds(1)), false);
				else if (tempChange.End.Ticks < MinimumValue.Ticks)
					currentValue = Snap(new TimeExtent(MinimumValue), false);
				else
					currentValue = Snap(new TimeExtent(currentValue.Start, tempChange.End), false);
			}

			UpdateTrackLayout(currentValue);
		}
		
		private double GetTrackWidth()
		{
			if (SliderTrack == null) return 0;
			bool hasIntervals = (Intervals == null) ? false : Intervals.GetEnumerator().MoveNext();
			double trackWidth;
			if (TimeMode == TimeMode.TimeExtent && !hasIntervals)
				trackWidth = SliderTrack.ActualWidth - (MinimumThumb == null ? 0 : MinimumThumb.ActualWidth) - (MaximumThumb == null ? 0 : MaximumThumb.ActualWidth);			
			else
				trackWidth = SliderTrack.ActualWidth - (MaximumThumb == null ? 0 : MaximumThumb.ActualWidth);
			return trackWidth;
		}

		private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
#if SILVERLIGHT
			totalHorizontalChange = 0;
			HorizontalChangeExtent = null;
#endif		
			if (currentValue == null) return;
			if ((sender as Thumb).Name == "HorizontalTrackThumb")
				Value = Snap(new TimeExtent(currentValue.Start, currentValue.End), true);
			else if (TimeMode == TimeMode.CumulativeFromStart)
				Value = Snap(new TimeExtent(MinimumValue, currentValue.End), false);
			else
				Value = Snap(new TimeExtent(currentValue.Start, currentValue.End), false);			
		}

		#endregion

		private TimeExtent Snap(TimeExtent extent, bool preserveSpan)
		{
			if (extent == null) return null;
			if (Intervals != null && Intervals.GetEnumerator().MoveNext())
			{				
				DateTime start = (extent.Start < MinimumValue ? MinimumValue : extent.Start);
				DateTime end = (extent.End > MaximumValue ? MaximumValue : extent.End);
				start = (start > end ? end : start);
				end = (end < start ? start : end);
				TimeExtent result = new TimeExtent(start, end);
				
				//snap min thumb.								
				long d0 = long.MaxValue;
				foreach (DateTime d in Intervals)
				{
					long delta = Math.Abs((d - start).Ticks);
					if (delta < d0)
					{
						if (TimeMode == TimeMode.CumulativeFromStart || TimeMode == TimeMode.TimeInstant || (TimeMode == TimeMode.TimeExtent && d < end))
						{
							d0 = delta;
							result.Start = d;
						}
					}
				}				

				if (preserveSpan)
				{
					//check interval difference between min and max.
					int intervalDifference = 0;
					bool count = false;
					if (TimeMode == TimeMode.TimeExtent)
					{
						foreach (DateTime d in Intervals)
						{
							count = (d >= ValidValue.Start && d < ValidValue.End) ? true : false;
							if (count) intervalDifference++;
						}
					}

					//snap max thumb.
					long d1 = long.MaxValue;
					count = false;
					int diff = 0;
					foreach (DateTime d in Intervals)
					{
						long delta = Math.Abs((d - end).Ticks);
						if (delta < d1)
						{
							if (TimeMode == TimeMode.CumulativeFromStart || TimeMode == TimeMode.TimeInstant || (TimeMode == TimeMode.TimeExtent && d > result.Start))
							{
								if (intervalDifference != 0)
								{
									count = (d >= result.Start) ? true : false;
									if (count) diff++;
									if (diff == intervalDifference)
									{
										result.End = d;
										return result;
									}
								}
								else
								{
									d1 = delta;
									result.End = d;
								}
							}
						}
					}
				}
				else
				{
					long d1 = long.MaxValue;
					foreach (DateTime d in Intervals)
					{
						long delta = Math.Abs((d - end).Ticks);
						if (delta < d1)
						{
							if (TimeMode == TimeMode.CumulativeFromStart || TimeMode == TimeMode.TimeInstant || (TimeMode == TimeMode.TimeExtent && d > result.Start))
							{																
								d1 = delta;
								result.End = d;								
							}
						}
					}
				}
				//return snaped extent
				return result;
			}
			else return extent;
			
		}

		private void CreateTickmarks()
		{			
			if (TickMarks == null || MinimumValue >= MaximumValue) return;
			
			TickMarks.TickMarkPositions = null;
			if (Intervals != null && Intervals.GetEnumerator().MoveNext())
			{														
				long span = MaximumValue.Ticks - MinimumValue.Ticks;
				List<double> intervals = new List<double>();
				foreach (DateTime d in Intervals)
					intervals.Add((d.Ticks - MinimumValue.Ticks) / (double)span);
				TickMarks.TickMarkPositions = intervals.ToArray();
			}			
		}
	
		#region Properties

		private TimeExtent ValidValue
		{
			get 
			{
				if (Value == null) return new TimeExtent(MinimumValue, MaximumValue);
				TimeExtent value = Value;
				if (Intervals != null && Intervals.GetEnumerator().MoveNext())
					value = Snap(Value, false);
				else if (Value.Start < MinimumValue && Value.End > MaximumValue)
					value = new TimeExtent(MinimumValue, MaximumValue);
				else if (Value.Start < MinimumValue)
					value = new TimeExtent(MinimumValue, Value.End);
				else if (Value.End > MaximumValue)
					value = new TimeExtent(Value.Start, MaximumValue);

				if (TimeMode == Toolkit.TimeMode.TimeInstant)
					value = new TimeExtent(value.End);
				else if(TimeMode == Toolkit.TimeMode.CumulativeFromStart)
					value = new TimeExtent(MinimumValue, value.End);				
				return value;
			}

		}

		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		[System.ComponentModel.TypeConverter(typeof(DateTimeTypeConverter))]
		public DateTime MinimumValue
		{
			get { return (DateTime)GetValue(MinimumValueProperty); }
			set { SetValue(MinimumValueProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MinimumValue"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MinimumValueProperty =
			DependencyProperty.Register("MinimumValue", typeof(DateTime), typeof(TimeSlider), new PropertyMetadata(OnMinimumValuePropertyChanged));

		private static void OnMinimumValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;
			obj.CreateTickmarks();
			obj.UpdateTrackLayout(obj.ValidValue);
		}

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		[System.ComponentModel.TypeConverter(typeof(DateTimeTypeConverter))]
		public DateTime MaximumValue
		{
			get { return (DateTime)GetValue(MaximumValueProperty); }
			set { SetValue(MaximumValueProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="MaximumValue"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MaximumValueProperty =
			DependencyProperty.Register("MaximumValue", typeof(DateTime), typeof(TimeSlider), new PropertyMetadata(OnMaximumValuePropertyChanged));

		private static void OnMaximumValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;			
			obj.CreateTickmarks();
			obj.UpdateTrackLayout(obj.ValidValue);			
		}

        /// <summary>
        /// Gets or sets the <see cref="ESRI.ArcGIS.Client.TimeExtent">TimeExtent</see> value(s) associated with the 
        /// visual thumbs(s) displayed on the TickBar of the TimeSlider Control. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The TimeSlider Value Property emits <see cref="ESRI.ArcGIS.Client.TimeExtent">TimeExtent</see> value(s) that 
        /// are typically used in conjunction with the Map Control to enhance the viewing of geographic features that 
        /// have attributes based upon Date/Time (aka. Time Info in ArcGIS Server terminology) information. In most 
        /// circumstances, the Value Property is bound to the 
        /// <see cref="ESRI.ArcGIS.Client.Map.TimeExtent">Map.TimeExtent</see> Property to control the visible 
        /// geographic features in a Layer that have temporal data. 
        /// </para>
        /// <para>
        /// <b>Note: </b>When a geographic feature has a TimeExtent that falls within the Map.TimeExtent, that feature 
        /// will be displayed. Conversely, when the TimeExtent of the geographic feature is no longer within the 
        /// Map.TimeExtent it is no longer displayed. 
        /// </para>
        /// <para>
        /// Setting the Value Property is one of the three necessary properties that must be set in order to have a 
        /// functioning TimeSlider. The other two necessary properties that must be set are the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MinimumValue">TimeSlider.MinimumValue</see> and 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MaximumValue">TimeSlider.MaximumValue</see>. 
        /// Under most circumstances the TimeSlider.Value Property is bound to the 
        /// <see cref="ESRI.ArcGIS.Client.Map.TimeExtent">Map.TimeExtent</see> Property to have the TimeSlider 
        /// manage the animation of temporal data in the Map Control. Setting these minimum requirements will enable 
        /// the TimeSlider to allow the user to manually move the thumb(s) across the TockBar causing an automatic 
        /// update of what is 
        /// emitted by the TimeSlider.Value Property to the bound Map.Value Property and thus producing the animation 
        /// effect on the Map. However, having only the three bare minimum properties set on the TimeSlider will not 
        /// provide the full functionality of the PlayPauseButton, NextButton, and PreviousButton being visible. In 
        /// order to have the full functionality of the TimeSlider, the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Intervals">TimeSlider.Intervals</see> Property must also 
        /// be set.
        /// </para>
        /// <para>
        /// The setting of the initial Value Property for 
        /// end users typically corresponds to the left-most TickMark position on the TickBar of the TimeSlider. This enables end 
        /// users to click the Play icon of the PlayPauseButton to begin an animation from the beginning of the series 
        /// of temporal geographic observations. There are several ways that a developer could determine what the 
        /// <a href="http://msdn.microsoft.com/en-us/library/03ybds8y(v=VS.95).aspx" target="_blank">Date/Time</a> of the 
        /// left-most TickMark position is. The following is a list of some common ways to determine this left-most 
        /// Date value:
        /// </para>
        /// <list type="bullet">
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MinimumValue">TimeSlider.MinimumValue</see> Property
        ///   </item>
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval">TimeSlider.CreateTimeStopsByTimeInterval.First</see> 
        ///   Property of the Static Function
        ///   </item>
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByCount">TimeSlider.CreateTimeStopsByCount.First</see> 
        ///   Property of the Static Function
        ///   </item>
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.FeatureLayer.TimeExtent">FeatureLayer.TimeExtent.Start</see> Property
        ///   </item>
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer.TimeExtent">ArcGISDynamicMapServiceLayer.TimeExtent.Start</see> 
        ///   Property
        ///   </item>
        ///   <item>
        ///   <see cref="ESRI.ArcGIS.Client.ArcGISImageServiceLayer.TimeExtent">ArcGISImageServiceLayer.TimeExtent.Start</see> 
        ///   Property
        ///   </item>
        ///   <item>
        ///   Set it manually in XAML or code-behind to a known Date/Time
        ///   </item>
        ///   <item>
        ///   Obtain the initial start Date/Time of the TimeExtent from your own custom function (see the code example in 
        ///   the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">TimeSlider.TimeMode</see> Property documentation for 
        ///   obtaining the initial TimeSlider.Value from a subset of Graphics in a FeatureLayer rather that the 
        ///   FeatureLayer’s published TimeExtent.Start)
        ///   </item>
        /// </list>
        /// <para>
        /// <b>NOTE:</b> you use Date/Time value(s) to construct the TimeExtent object.
        /// </para>
        /// <para>
        /// The TimeExtent object that is used by the TimeSlider.Value 
        /// Property can be essentially constructed two ways: 
        /// (1) as an 'instance-in-time' or (2) as a 'period-of-time'. Depending on how the TimeExtent object is 
        /// constructed will have implications for how the TimeSlider behaves when the TimeSlider.Value is 
        /// set. It is important to remember that a Map Control that is bound to a TimeSlider will only display 
        /// geographic features that match the TimeSlider.Value Property. 
        /// The <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode">TimeSlider.TimeMode</see> Property also
        /// plays crucial role in how the TimeSlider behaves and setting the correct TimeExtent for the TimeSlider.Value
        /// will impact the TimeSlider.TimeMode setting.
        /// When using the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeMode">TimeMode</see> Enumerations of 
        /// <b>CumulativeFromStart</b> and <b>TimeExtent</b>, it is best 
        /// that you construct the TimeSlider.Value using a 'period-of-time' for the TimeExtent object. When using the 
        /// TimeMode Enumeration of <b>TimeInstant</b>, it is best that you construct the TimeSlider.Value using an 
        /// 'instance-in-time' for the TimeExtent object. By default the TimeSlider uses the TimeMode.CumulativeFromStart 
        /// as its setting. The code example in the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> 
        /// document demonstrates the subtle differences you may want to explore. 
        /// </para>
        /// <para>
        /// The following code examples demonstrate manually creating an 'instance-in-time' and 'period-of-time' for a 
        /// TimeExtent object:
        /// </para>
        /// <para>XAML:
        /// </para>
        /// <code>
        /// &lt;esri:TimeSlider Name="TimeSlider1" Value="2000/08/04 12:30:00 UTC" /&gt; &lt;!-- instance-in-time --&gt; 
        /// &lt;esri:TimeSlider Name="TimeSlider1" Value="2000/08/04 12:30:00 UTC, 2000/08/05 12:30:00 UTC" /&gt; &lt;!-- period-of-time --&gt;
        /// </code>
        /// <para>C#:
        /// </para>
        /// <code>
        /// TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(new DateTime(2000, 8, 4, 12, 30, 0)); // instance-in-time
        /// TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(new DateTime(2000, 8, 4, 12, 30, 0), new DateTime(2000, 8, 5, 12, 30, 0)); // period-of-time
        /// </code>
        /// <para>VB.NET:
        /// </para>
        /// <code>
        /// TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(New Date(2000, 8, 4, 12, 30, 0)) ' instance-in-time
        /// TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(New Date(2000, 8, 4, 12, 30, 0), New Date(2000, 8, 5, 12, 30, 0)) ' period-of-time
        /// </code>
        /// <para>
        /// A word of caution about using the code-behind TimeSlider.Loaded Event if you are trying to obtain the 
        /// TimeExtent Property of a Layer which contains temporal observations to calculate the TimeSlider.Value and 
        /// TimeSlider.Intervals; this will not work. The TimeSlider.Loaded event occurs before the various Layer 
        /// <b>.Initialized</b> Events occur so the Layer's <b>.TimeExtent</b> values will always be 
        /// Nothing/null. Using the TimeSlider.Loaded Event to determine the TimeSlider.Value and TimeSlider.Intervals 
        /// Properties is best when the TimeExtent values are known at design-time and the TimeSlider.Value and 
        /// TimeSlider.Intervals just need constructing in the code-behind.
        /// </para>
        /// <para>
        /// It is possible to set the TimeSlider.Value Property in XAML using Binding to another object. Just make sure 
        /// the object you are binding to returns a TimeExtent object. The following is a simple XAML fragment where 
        /// the TimeSlider.Value is set to the TimeExtent of a FeatureLayer named ‘EarthquakesLayer’:
        /// <code>
        /// &lt;esri:TimeSlider Name="TimeSlider1" Value="{Binding ElementName=MyMap, Path=Layers[EarthquakesLayer].TimeExtent, Mode=OneWay}" /&gt;
        /// </code>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the various buttons for each <b>TimeMode</b> setting to see what the effect is on running the 
        /// TimeSlider and the output that is produced in the Map Control. The buttons in Green are those that use 
        /// best practices in the setting the TimeSlider <b>.Value</b> and <b>.TimeMode</b> appropriately in 
        /// conjunction with each other. Click the bottom button ('Get the TimeSliderExtent.Value') before and 
        /// after using the PlayPauseButton on the TimeSlider control to see how the various TimeSlider.Value's 
        /// change.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Comparing the various TimeSlider.Value settings for the different TimeSlider.TimeMode options." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.Value2.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="63" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///            Width="628" TextWrapping="Wrap" Margin="12,0,0,0" 
        ///            Text="Click the various buttons for each TimeMode setting to see what the effect is on running 
        ///            the TimeSlider and the output that is produced in the Map Control. The buttons in Green are 
        ///            those that use best practices in the setting the TimeSlider .Value and .TimeMode appropriately 
        ///            in conjunction with each other. Click the bottom button ('Get the TimeSliderExtent.Value') 
        ///            before and after using the PlayPauseButton on the TimeSlider control to see how the various 
        ///            TimeSlider.Value's change." /&gt;
        ///     
        ///   &lt;!--
        ///   Add a Map Control with an ArcGISTiledMapServiceLayer and a FeatureLayer. The ArcGISTiledMapsServiceLayer 
        ///   is the first Layer in Map.LayerCollection and is drawn on the bottom. The FeatureLayer is then added and 
        ///   draws on top. Set the Map.Extent to zoom to the middle of the Atlantic Ocean.
        ///       
        ///   Setting the Map.TimeExtent acts like a Where clause in that only those features/records that fall
        ///   within the set TimeSlider.Value (which is a narrowed TimeExtent) will then be shown. By Binding the 
        ///   Map.TimeExtent to the TimeSlider.Value Property, the Map will automatically update as the thumb on 
        ///   the TimeSlider moves.
        ///   --&gt;
        ///   &lt;esri:Map Name="Map1" Background="White" Height="375" Width="375" Margin="12,93,0,0" 
        ///             VerticalAlignment="Top" HorizontalAlignment="Left" Extent="-96.81,-57.36,-26.16,13.28"
        ///             TimeExtent="{Binding ElementName=TimeSlider1, Path=Value}"&gt;
        ///   
        ///     &lt;!-- Add an ArcGISTiledMapsServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="StreetMapLayer"
        ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer"/&gt;
        ///     
        ///     &lt;!--
        ///     The FeatureLayer contains earthquake dats that is restricted to only those with a Magnitude &gt; 7.
        ///     --&gt;
        ///          
        ///     &lt;esri:FeatureLayer ID="MyFeatureLayer"
        ///           Url="http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/Since_1970/MapServer/0" 
        ///           OutFields="*" Where="Magnitude &gt; 7" Initialized="FeatureLayer_Initialized"/&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a TimeSlider. The initialization of the TimeSlider Properties (.MinimumValue, .MaximumValue, 
        ///   .Intervals, and .Value) will be handled in the code-behind in the FeatureLayer_Initialized Event.
        ///   --&gt;
        ///   &lt;esri:TimeSlider Name="TimeSlider1" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///                    Height="25" Width="375" Margin="12,67,0,0"/&gt;
        ///     
        ///   &lt;!-- TimeSlider.TimeMode.CumulativeFromStart section. --&gt;
        ///   &lt;sdk:Label Height="19" HorizontalAlignment="Left" Margin="393,73,0,0" Name="Label1" VerticalAlignment="Top" Width="241" 
        ///              Content="TimeMode.CumulativeFromStart:" FontWeight="Bold"/&gt;
        ///   &lt;Button Content="5th position via 'instance-in-time'" Height="23" HorizontalAlignment="Left" Margin="393,93,0,0" 
        ///           Name="btn1" VerticalAlignment="Top" Width="235" Click="Button_CumulativeFromStart_InstanceInTime"&gt;&lt;/Button&gt;
        ///   &lt;Button Content="5th to 9th position via 'period-of-time'" Height="23" HorizontalAlignment="Left" Margin="393,123,0,0" 
        ///           Name="btn2" VerticalAlignment="Top" Width="235"  Click="Button_CumulativeFromStart_PeriodOfTime" Background="Lime" AllowDrop="True" /&gt;
        ///     
        ///   &lt;!-- TimeSlider.TimeMode.TimeExtent section. --&gt;
        ///   &lt;sdk:Label Height="19" HorizontalAlignment="Left" Margin="393,169,0,0" Name="Label2" VerticalAlignment="Top" Width="235" 
        ///              Content="TimeMode.TimeExtent:" FontWeight="Bold"/&gt;
        ///   &lt;Button Content="5th position via 'instance-in-time'" Height="23" HorizontalAlignment="Left" Margin="393,186,0,0" 
        ///           Name="btn3" VerticalAlignment="Top" Width="235" Click="Button_TimeExtent_InstanceInTime"/&gt;
        ///   &lt;Button Content="5th to 9th position via 'period-of-time'" Height="23" HorizontalAlignment="Left" Margin="393,218,0,0" 
        ///           Name="btn4" VerticalAlignment="Top" Width="235" Click="Button_TimeExtent_PeriodOfTime" Background="Lime" /&gt;
        ///     
        ///   &lt;!-- TimeSlider.TimeMode.TimeInstant section. --&gt;
        ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="403,262,0,0" Name="Label3" VerticalAlignment="Top" Width="212" 
        ///              Content="TimeMode.TimeInstant:" FontWeight="Bold"/&gt;
        ///   &lt;Button Content="5th position via 'instance-in-time'" Height="23" HorizontalAlignment="Left" Margin="397,280,0,0" 
        ///           Name="btn5" VerticalAlignment="Top" Width="231" Click="Button_TimeInstant_InstanceInTime" Background="Lime" /&gt;
        ///   &lt;Button Content="5th to 9th position via 'period-of-time'" Height="23" HorizontalAlignment="Left" Margin="397,309,0,0" 
        ///           Name="btn6" VerticalAlignment="Top" Width="231" Click="Button_TimeInstant_PeriodOfTime"/&gt;
        ///   
        ///   &lt;!-- 
        ///   Useful button to determine the active TimeSlider.Value settings. Use before and after clicking the PlayPauseButton
        ///   on the TimeSlider.
        ///   --&gt;
        ///   &lt;Button Content="Get the TimeSlider.Value" Height="23" HorizontalAlignment="Left" Margin="397,403,0,0" Name="btn7" 
        ///           VerticalAlignment="Top" Width="231" Click="Button_Get_TimeSliderValue"/&gt;
        ///    
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void FeatureLayer_Initialized(object sender, System.EventArgs e)
        /// {
        ///   // This function sets up TimeSlider .MinimumValue, .MaximumValue, .Intervals, and the initial starting 
        ///   // .Value that define how the TimeSlider will behave. 
        ///   
        ///   // Get the FeatureLayer with TimeExtent information.
        ///   ESRI.ArcGIS.Client.FeatureLayer myFeatureLayer = (ESRI.ArcGIS.Client.FeatureLayer)sender;
        ///   
        ///   // Obtain the start and end Date/Time values from the TimeSlider.
        ///   DateTime myMinimumDate = myFeatureLayer.TimeExtent.Start;
        ///   DateTime myMaximumDate = myFeatureLayer.TimeExtent.End;
        ///   
        ///   // If you don't set the .MinimumValue and .MaximumValue no TicksMarks get set on the TickBar!
        ///   TimeSlider1.MinimumValue = myMinimumDate;
        ///   TimeSlider1.MaximumValue = myMaximumDate;
        ///   
        ///   // Create a TimeExtent based upon the start and end Date/Times.
        ///   ESRI.ArcGIS.Client.TimeExtent myTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate);
        ///   
        ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
        ///   System.Collections.Generic.IEnumerable&lt;DateTime&gt; myIEnumerableDates = null;
        ///   
        ///   // Load all of Dates into the Collection of IEnumerable&lt;DateTime&gt; objects using the 
        ///   // TimeSlider.CreateTimeStopsByCount Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByCount(myTimeExtent, 20);
        ///   
        ///   // Set the TimeSlider.Intervals which define the Tick marks along the TimeSlider track to the 
        ///   // IEnumerable&lt;DateTime&gt; objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates;
        ///   
        ///   // Define the initial starting position of the thumb along the TimeSlider.
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First);
        /// }
        /// 
        /// 
        /// // When using ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        /// // ------------------------------------------------------------------
        /// private void Button_CumulativeFromStart_InstanceInTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // No observations will initially be will be shown in the Map upon the button click. Once the user clicks 
        ///   // the Play icon of the PlayPauseButton, the TimeSlider1.Value will automatically adjust to a 
        ///   // 'period-of-time' and all observations up to the 6th TickMark of the TickBar will suddenly appear and 
        ///   // then continue to cumulatively display the animation as the thumb moves across the Ticks. If you want 
        ///   // to track a single instance in time use the TimeSlider.TimeMode.Instance enumeration instead.
        ///   //
        ///   // Using this option is not considered a best practice.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5));
        /// }
        /// 
        /// private void Button_CumulativeFromStart_PeriodOfTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   // Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   // 'period-of-time' as the thumb moves across the Ticks.
        ///   //
        ///   // **********************************************************************************************************
        ///   // It is a best practice to use a 'period-of-time' (i.e. a TimeExtent defined with a Start and End Date/Time)
        ///   // TimeSlider.Value setting when using the TimeSlider.TimeMode.CumulativeFromStart.
        ///   // **********************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9));
        /// }
        /// 
        /// 
        /// // When using ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent
        /// // ---------------------------------------------------------
        /// private void Button_TimeExtent_InstanceInTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // No observations will initially be will be shown in the Map upon the button click. Once the user clicks the 
        ///   // Play icon of the PlayPauseButton, the TimeSlider1.Value will automatically adjust to a 'period-of-time' that 
        ///   // covers the span of two Tick marks. As the animation occurs of the thumbs moving across the Ticks there will
        ///   // always be a span for the 'period-of-time' that covers two Ticks; this can not be changed. If you want to track
        ///   // a single instance in time use the TimeSlider.TimeMode.Instance enumeration instead.
        ///   //
        ///   // Using this option is not considered a best practice.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5));
        /// }
        /// 
        /// private void Button_TimeExtent_PeriodOfTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   // Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   // 'period-of-time' as the thumb moves across the Ticks.
        ///   //
        ///   // *********************************************************************************************************
        ///   // It is a best practice to use a 'period-of-time' (i.e. a TimeExtent defined with a Start and End Date/Time)
        ///   // TimeSlider.Value setting when using the TimeSlider.TimeMode.TimeExtent.
        ///   // *********************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9));
        /// }
        /// 
        /// 
        /// // When we have ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant
        /// // ------------------------------------------------------------
        /// private void Button_TimeInstant_InstanceInTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // No observations will be initially will be shown in the Map upon the button click. This is because the 
        ///   // 'instance-in-time' is a single Date/Time ("1980/05/17 13:53:41 UTC" in XAML) observation for which there
        ///   // is no earthquake event occuring. Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value 
        ///   // will continue to use the 'instance-in-time' (i.e. a TimeExtent with a single Start or End Date/Time) to display
        ///   // earthquake events. Unfortunately, there will never be any earthquake events that display in this option even
        ///   // though it is the correct one to use for the TimeMode.Instant setting. This is because the calculated TickMark
        ///   // intervals from using the TimeSlider.CreateTimeStopsByCount Shared/Static function never coincide with an
        ///   // actual earthquake event. When using the TimeMode.TimeInstant make sure and use data and/or an algorithm
        ///   // that will produce TickMark intervals that coincide with the data values being displayed.
        ///   //
        ///   // ***************************************************************************************************************
        ///   // It is a best practice to use a 'instance-of-time' (i.e. a TimeExtent defined with just a Start or End Date/Time)
        ///   // TimeSlider.Value setting when using the TimeSlider.TimeMode.TimeInstant
        ///   // ***************************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5));
        /// }
        /// 
        /// private void Button_TimeInstant_PeriodOfTime(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   // Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   // 'instance-in-time' (i.e. a TimeExtent with a single Start or End Date/Time) to display
        ///   // earthquake events. Unfortunately, there will never be any earthquake events that display in this option 
        ///   // because the calculated TickMark intervals from using the TimeSlider.CreateTimeStopsByCount Shared/Static 
        ///   // function never coincide with an actual earthquake event. When using the TimeMode.TimeInstant make sure and 
        ///   // use data and/or an algorithm that will produce TickMark intervals that coincide with the data values being 
        ///   // displayed.
        ///   //
        ///   // Using this option is not considered a best practice as the TimeSlider will automatically convert the
        ///   // TimeSlider.Value from a 'period-of-time' to an 'instance-in-time' for the TimeExtent values being emmited.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant;
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9));
        /// }
        /// 
        /// private void Button_Get_TimeSliderValue(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Use this function after hitting each button to the the acutal TimeSlider.Value output. It is also
        ///   // helpful to start and then pause the TimeSlider and then use this function to see how the TimeSlider.Value
        ///   // output data changes.
        /// 
        ///   MessageBox.Show(TimeSlider1.Value.ToString());
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub FeatureLayer_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ///   
        ///   ' This function sets up TimeSlider .MinimumValue, .MaximumValue, .Intervals, and initial starting .Value 
        ///   ' that define how the TimeSlider will behave. 
        ///   
        ///   ' Get the FeatureLayer with TimeExtent information.
        ///   Dim myFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = sender
        ///   
        ///   ' Obtain the start and end Date/Time values from the TimeSlider.
        ///   Dim myMinimumDate As Date = myFeatureLayer.TimeExtent.Start
        ///   Dim myMaximumDate As Date = myFeatureLayer.TimeExtent.End
        ///   
        ///   ' If you don't set the .MinimumValue and .MaximumValue no TicksMarks get set on the TickBar!
        ///   TimeSlider1.MinimumValue = myMinimumDate
        ///   TimeSlider1.MaximumValue = myMaximumDate
        ///   
        ///   ' Create a TimeExtent based upon the start and end Date/Times.
        ///   Dim myTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate)
        ///   
        ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
        ///   Dim myIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
        ///   
        ///   ' Load all of Dates into the Collection of IEnumerable(Of Date) objects using the 
        ///   ' TimeSlider.CreateTimeStopsByCount Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByCount(myTimeExtent, 20)
        ///   
        ///   ' Set the TimeSlider.Intervals which define the Tick marks along the TimeSlider track to the 
        ///   ' IEnumerable(Of Date) objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates
        ///   
        ///   ' Define the initial starting position of the thumb along the TimeSlider.
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First)
        ///   
        /// End Sub
        ///   
        /// 
        /// ' When using ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        /// ' ------------------------------------------------------------------
        /// Private Sub Button_CumulativeFromStart_InstanceInTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' No observations will initially be will be shown in the Map upon the button click. Once the user clicks the 
        ///   ' Play icon of the PlayPauseButton, the TimeSlider1.Value will automatically adjust to a 'period-of-time' and 
        ///   ' all observations up to the 6th TickMark of the TickBar will suddenly appear and then continue to cumulatively
        ///   ' display the animation as the thumb moves across the Ticks. If you want to track a single instance in time 
        ///   ' use the TimeSlider.TimeMode.Instance enumeration instead.
        ///   '
        ///   ' Using this option is not considered a best practice.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5))
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_CumulativeFromStart_PeriodOfTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   ' Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   ' 'period-of-time' as the thumb moves across the Ticks.
        ///   '
        ///   ' **********************************************************************************************************
        ///   ' It is a best practice to use a 'period-of-time' (i.e. a TimeExtent defined with a Start and End Date/Time)
        ///   ' TimeSlider.Value setting when using the TimeSlider.TimeMode.CumulativeFromStart.
        ///   ' **********************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9))
        ///   
        /// End Sub
        /// 
        /// 
        /// ' When using ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent
        /// ' ---------------------------------------------------------
        /// Private Sub Button_TimeExtent_InstanceInTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' No observations will initially be will be shown in the Map upon the button click. Once the user clicks the 
        ///   ' Play icon of the PlayPauseButton, the TimeSlider1.Value will automatically adjust to a 'period-of-time' that 
        ///   ' covers the span of two Tick marks. As the animation occurs of the thumbs moving across the Ticks there will
        ///   ' always be a span for the 'period-of-time' that covers two Ticks; this can not be changed. If you want to track
        ///   ' a single instance in time use the TimeSlider.TimeMode.Instance enumeration instead.
        ///   '
        ///   ' Using this option is not considered a best practice.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5))
        /// 
        /// End Sub
        /// 
        /// Private Sub Button_TimeExtent_PeriodOfTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   ' Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   ' 'period-of-time' as the thumb moves across the Ticks.
        ///   '
        ///   ' *********************************************************************************************************
        ///   ' It is a best practice to use a 'period-of-time' (i.e. a TimeExtent defined with a Start and End Date/Time)
        ///   ' TimeSlider.Value setting when using the TimeSlider.TimeMode.TimeExtent.
        ///   ' *********************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9))
        ///   
        /// End Sub
        /// 
        /// 
        /// ' When we have ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant
        /// ' ------------------------------------------------------------
        /// Private Sub Button_TimeInstant_InstanceInTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' No observations will be initially will be shown in the Map upon the button click. This is because the 
        ///   ' 'instance-in-time' is a single Date/Time ("1980/05/17 13:53:41 UTC" in XAML) observation for which there
        ///   ' is no earthquake event occuring. Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value 
        ///   ' will continue to use the 'instance-in-time' (i.e. a TimeExtent with a single Start or End Date/Time) to display
        ///   ' earthquake events. Unfortunately, there will never be any earthquake events that display in this option even
        ///   ' though it is the correct one to use for the TimeMode.Instant setting. This is because the calculated TickMark
        ///   ' intervals from using the TimeSlider.CreateTimeStopsByCount Shared/Static function never coincide with an
        ///   ' actual earthquake event. When using the TimeMode.TimeInstant make sure and use data and/or an algorithm
        ///   ' that will produce TickMark intervals that coincide with the data values being displayed.
        ///   '
        ///   ' ***************************************************************************************************************
        ///   ' It is a best practice to use a 'instance-of-time' (i.e. a TimeExtent defined with just a Start or End Date/Time)
        ///   ' TimeSlider.Value setting when using the TimeSlider.TimeMode.TimeInstant
        ///   ' ***************************************************************************************************************
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5))
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_TimeInstant_PeriodOfTime(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' All observations that fall within the TimeExtent will initially be shown in the Map upon the button click.
        ///   ' Once the user clicks the Play icon of the PlayPauseButton, the TimeSlider1.Value will continue to use the 
        ///   ' 'instance-in-time' (i.e. a TimeExtent with a single Start or End Date/Time) to display
        ///   ' earthquake events. Unfortunately, there will never be any earthquake events that display in this option 
        ///   ' because the calculated TickMark intervals from using the TimeSlider.CreateTimeStopsByCount Shared/Static 
        ///   ' function never coincide with an actual earthquake event. When using the TimeMode.TimeInstant make sure and 
        ///   ' use data and/or an algorithm that will produce TickMark intervals that coincide with the data values being 
        ///   ' displayed.
        ///   '
        ///   ' Using this option is not considered a best practice as the TimeSlider will automatically convert the
        ///   ' TimeSlider.Value from a 'period-of-time' to an 'instance-in-time' for the TimeExtent values being emmited.
        ///   
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.ElementAt(5), TimeSlider1.Intervals.ElementAt(9))
        ///   
        /// End Sub
        /// 
        /// Private Sub Button_Get_TimeSliderValue(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Use this function after hitting each button to the the acutal TimeSlider.Value output. It is also
        ///   ' helpful to start and then pause the TimeSlider and then use this function to see how the TimeSlider.Value
        ///   ' output data changes.
        ///   
        ///   MessageBox.Show(TimeSlider1.Value.ToString)
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public TimeExtent Value
		{
			get { return (TimeExtent)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(TimeExtent), typeof(TimeSlider), new PropertyMetadata(OnValuePropertyChanged));

		private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;
			TimeExtent newValue = e.NewValue as TimeExtent;

            obj.currentValue = newValue;
			obj.UpdateTrackLayout(obj.ValidValue);
			EventHandler<ValueChangedEventArgs> handler = obj.ValueChanged;
			if (handler != null)
			{
				handler(obj, new ValueChangedEventArgs(newValue, e.OldValue as TimeExtent));
			}
			
		}

		/// <summary>
		/// Gets or sets the time intervals for the tickmarks.
		/// </summary>
		/// <value>The intervals.</value>
		public IEnumerable<DateTime> Intervals
		{
			get { return (IEnumerable<DateTime>)GetValue(IntervalsProperty); }
			set { SetValue(IntervalsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Intervals"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IntervalsProperty =
			DependencyProperty.Register("Intervals", typeof(IEnumerable<DateTime>), typeof(TimeSlider), new PropertyMetadata(OnIntervalsPropertyChanged));

		private static void OnIntervalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;
			if (obj.IsPlaying && (obj.Intervals == null || !obj.Intervals.GetEnumerator().MoveNext()))
				obj.IsPlaying = false;
			obj.CreateTickmarks();
			if(e.OldValue is ObservableCollection<DateTime>)
			{
				(e.OldValue as ObservableCollection<DateTime>).CollectionChanged -= obj.TimeSlider_CollectionChanged;
			}
			if(e.NewValue is ObservableCollection<DateTime>)
			{
				(e.NewValue as ObservableCollection<DateTime>).CollectionChanged += obj.TimeSlider_CollectionChanged;
			}
			obj.SetButtonVisibility();
			obj.UpdateTrackLayout(obj.ValidValue);
		}

        /// <summary>
        /// Gets or sets the how fast the thumb(s) moves across the Tick Marks of the TimeSlider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The PlaySpeed Property uses a 
        /// <a href="http://msdn.microsoft.com/en-us/library/system.timespan(v=VS.95).aspx" target="_blank">TimeSpan</a> Structure 
        /// to define how quickly the thumb moves across the tick marks on the TimeSlider.
        /// </para>
        /// <para>
        /// The PlaySpeed Attribute in XAML follows the format of "hh:mm:ss" where, hh = hours (0 to 24), mm = minutes (0 to 60), 
        /// and ss = seconds (0 to 60). You can have fractions of a second by using decimal values. In the following XAML code 
        /// fragment the PlaySpeed increments the time intervals every tenth of a second (i.e. 0.1). The default PlaySpeed is 
        /// 1 second.
        /// </para>
        /// <para>
        /// <code lang="XAML">
        /// &lt;esri:TimeSlider Name="TimeSlider1" PlaySpeed="00:00:00.1"/&gt;
        /// </code>
        /// </para>
        /// <para>
        /// In the code-behind there are several ways to make a TimeSpan using the constructor. The following code examples 
        /// provides a few different examples that also produce the same time intervals every tenth of a second (i.e. 0.1):
        /// </para>
        /// <para>
        /// VB.NET:
        /// </para>
        /// <para>
        /// <code lang="VB.NET">
        /// TimeSlider1.PlaySpeed = New TimeSpan(100000) ' ticks
        /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0.1) ' hours, minutes, seconds
        /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0.1) ' days, hours, minutes, seconds
        /// TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0, 10) ' days, hours, minutes, seconds, milliseconds
        /// </code>
        /// </para>
        /// <para>
        /// C#:
        /// </para>
        /// <para>
        /// <code lang="CS">
        /// TimeSlider1.PlaySpeed = new TimeSpan(100000); // ticks
        /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0.1); // hours, minutes, seconds
        /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0, 0.1); // days, hours, minutes, seconds
        /// TimeSlider1.PlaySpeed = new TimeSpan(0, 0, 0, 0, 10); // days, hours, minutes, seconds, milliseconds
        /// </code>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the play button on the TimeSlider to start the animation. Then use the regular Microsoft Slider Control to 
        /// adjust the speed of the animation (i.e. PlaySpeed) on the TimeSlider." 
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating how to adjust the TimeSlider.PlaySpeed Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.PlaySpeed.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="63" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///            Width="537" TextWrapping="Wrap" Margin="12,0,0,0" 
        ///            Text="Click the play button on the TimeSlider to start the animation. Then use the regular Microsoft
        ///              Slider Control to adjust the speed of the animation (i.e. PlaySpeed) on the TimeSlider." /&gt;
        ///   
        ///   &lt;!--
        ///   Add a Map Control with an ArcGISTiledMapServiceLayer and a FeatureLayer. The ArcGISTiledMapsServiceLayer 
        ///   is the first Layer in Map.LayerCollection and is drawn on the bottom. The FeatureLayer is then added and 
        ///   draws on top. Set the Map.Extent to zoom to the middle of the Atlantic Ocean.
        ///     
        ///   Setting the Map.TimeExtent acts like a Where clause in that only those features/records that fall
        ///   within the set TimeSlider.Value (which is a narrowed TimeExtent) will then be shown. By Binding the 
        ///   Map.TimeExtent to the TimeSlider.Value Property, the Map will automatically update as the thumb(s) on 
        ///   the TimeSlider moves.
        ///   --&gt;
        ///   &lt;esri:Map Name="Map1" Background="White" Height="375" Width="375" Margin="12,93,0,0" 
        ///             VerticalAlignment="Top" HorizontalAlignment="Left" Extent="-103.04,-13.82,2.08,91.30"
        ///             TimeExtent="{Binding ElementName=TimeSlider1, Path=Value}"&gt;
        ///     
        ///     &lt;!-- Add an ArcGISTiledMapsServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="StreetMapLayer"
        ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer"/&gt;
        ///       
        ///     &lt;!--
        ///     The FeatureLayer contains Hurricane data from NOAA as Markers (aka. Points). The Where clause is 
        ///     optional. It is necessary when more that 500/1000 records returned. In ArcGIS Server 9.3.1 and prior, 
        ///     the default maximum is 500 records returned per FeatureLayer. In ArcGIS Server 10 the default is 1000. 
        ///     This setting is configurable per map service using ArcCatalog or ArcGIS Server Manager (on the Parameters
        ///     tab). 
        ///           
        ///     The Where clause gets only hurricane data for Alberto (a much smaller subset of the entire service).
        ///     
        ///     Specify the Outfields Property to specify which Fields are returned. Specifying the wildcard (*) 
        ///     character will return all Fields. 
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="MyFeatureLayer" OutFields="*" Where="EVENTID = 'Alberto'"
        ///           Url="http://servicesbeta.esri.com/ArcGIS/rest/services/Hurricanes/Hurricanes/MapServer/0" /&gt;
        ///       
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a TimeSlider. The initialization of the TimeSlider .Intervals and .Value Properties will be 
        ///   handled in the code-behind when the TimeSlider initializes. 
        ///   
        ///   The remainder of the initialization of the TimeSlider Properties will be handled here in the XAML.
        ///   --&gt;
        ///   &lt;esri:TimeSlider Name="TimeSlider1" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///                    Height="25" Width="375" Margin="12,67,0,0"
        ///                    MinimumValue="2000/08/04 00:00:01 UTC" MaximumValue="2000/08/24 06:00:01 UTC"
        ///                    Value = "2000/08/04 00:00:01 UTC,2000/08/04 00:00:01 UTC"
        ///                    TimeMode="CumulativeFromStart" Loop="True" 
        ///                    Loaded="TimeSlider1_Loaded"/&gt;
        ///     
        ///   &lt;!-- Add a rectangle and a label for a nice layout. --&gt;
        ///   &lt;Rectangle Height="176" HorizontalAlignment="Left" Margin="396,83,0,0" Name="Rectangle_NiceFormat" 
        ///              Stroke="Black" StrokeThickness="1" VerticalAlignment="Top" Width="225" /&gt;
        ///   &lt;sdk:Label Height="28" HorizontalAlignment="Left" Margin="444,89,0,0" Name="Label_PlaySpeed" 
        ///              VerticalAlignment="Top" Width="88" Content="PlaySpeed" FontSize="14" /&gt;
        ///     
        ///   &lt;!-- 
        ///   Add a regular Microsoft Slider Control and a few explanatory labels to adjust the speed
        ///   of the TimeSlider Control. Each time the regular Slider has a different value, call the
        ///   ValueChanged event with some code-behind logic to adjust the TimeSlider.PlaySpeed.
        ///   --&gt;
        ///   &lt;sdk:Label HorizontalAlignment="Left" VerticalAlignment="Top" Margin="418,114,0,346" 
        ///              Name="Label_Slow" Width="36" Content="Slow"/&gt;
        ///   &lt;Slider HorizontalAlignment="Left" Name="Slider1" Width="48" Orientation="Vertical"
        ///           Minimum="100000" Maximum="20000000" Value="10000000" Height="80" Margin="406,140,0,0" 
        ///           VerticalAlignment="Top" ValueChanged="Slider1_ValueChanged"/&gt;
        ///   &lt;sdk:Label Height="17" HorizontalAlignment="Left" Margin="422,225,0,0" Name="Label_Fast" 
        ///              VerticalAlignment="Top" Width="41" Content="Fast"/&gt;
        ///     
        ///   &lt;!-- 
        ///   The output of the regular Microsoft Slider is in Ticks. Show this to the user. Note the use of
        ///   the StringFormat function on the displayed text in the TextBlock_Ticks. We only need whole number
        ///   values for Tics, not decimal values.
        ///   --&gt;
        ///   &lt;sdk:Label HorizontalAlignment="Left" VerticalAlignment="Top" Margin="476,138,0,0" 
        ///              Name="Label_Ticks" Width="66" Content="Ticks:"/&gt;
        ///   &lt;TextBlock Height="23" HorizontalAlignment="Left" Margin="476,153,0,0" Name="TextBlock_Ticks" 
        ///              Text="{Binding ElementName=Slider1,Path=Value,StringFormat=F0}" VerticalAlignment="Top" /&gt;
        ///   
        ///   &lt;!-- 
        ///   The desired input of the TimeSlider.PlaySpeed is a TimeSpan object. Show how the TimeSpan
        ///   string compares to a Tick version. Remember: The TimeSlider.PlaySpeed is displayed in the TimeSpan 
        ///   format of: 'days:hours:seconds'
        ///   --&gt;
        ///   &lt;TextBlock Height="23" HorizontalAlignment="Left" Margin="476,197,0,0" Name="TextBlock_TimeSpan" 
        ///              Text="{Binding  ElementName=TimeSlider1, Path=PlaySpeed}" VerticalAlignment="Top" Width="107" /&gt;
        ///   &lt;sdk:Label Height="19" HorizontalAlignment="Left" Margin="476,182,0,0" Name="Label_TimeSpan" 
        ///              VerticalAlignment="Top" Width="66" Content="TimeSpan:" /&gt;
        ///     
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// // A word of caution about using the TimeSlider.Loaded Event. If you are trying to obtain the 
        /// // TimeExtent from a FeatureLayer, this will not work. The TimeSlider.Loaded occurs before the 
        /// // FeatureLayer.Intialized Event so the FeatureLayer.TimeExtent will always be Nothing/null. Using the 
        /// // TimeSlider.Loaded Event is best when the TimeExtent values are known but the TimeSlider.Intervals 
        /// // just need constructing.
        ///   
        /// private void TimeSlider1_Loaded(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   // are a Collection of IEnumerable&lt;DateTime&gt;) objects. When the TimeSlider.Intervals Property is set along with 
        ///   // the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
        ///   // functionality includes buttons for Play, Pause, Forward, and Back.
        ///   
        ///   // Obtain the start and end DateTime values from the TimeSlider named TimeSlider1 that was defined in XAML.
        ///   DateTime myMinimumDate = TimeSlider1.MinimumValue;
        ///   DateTime myMaximumDate = TimeSlider1.MaximumValue;
        ///   
        ///   // Create a TimeExtent based upon the start and end DateTimes.
        ///   ESRI.ArcGIS.Client.TimeExtent myTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate);
        ///   
        ///   // Create a new TimeSpan (1 day in our case).
        ///   TimeSpan myTimeSpan = new TimeSpan(1, 0, 0, 0);
        ///   
        ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
        ///   System.Collections.Generic.IEnumerable&lt;DateTime&gt; myIEnumerableDates = null;
        ///   
        ///   // Load all of Dates into the Collection of IEnumerable&lt;DateTime&gt; objects using the 
        ///   // TimeSlider.CreateTimeStopsByTimeInterval Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan);
        ///   
        ///   // Set the TimeSlider.Intervals which define the tick marks along the TimeSlider track to the IEnumerable&lt;DateTime&gt; 
        ///   // objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates;
        ///   
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First);
        ///   
        /// }
        /// 
        /// private void Slider1_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt; e)
        /// {
        ///   // This function converts the output from the regular Microsoft Slider Control in Ticks into a the
        ///   // TimeSpan object which the ESRI TimeSlider control likes.
        ///   
        ///   TimeSpan aTimeSpan = new TimeSpan(sender.Value);
        ///   
        ///   // Make sure the TimeSlider has been completely created before we attempt to set a PlaySpeed value.
        ///   if (TimeSlider1 != null)
        ///   {
        ///     // Set the PlaySpeed to the TimeSpan.
        ///     TimeSlider1.PlaySpeed = aTimeSpan;
        ///   }
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// ' A word of caution about using the TimeSlider.Loaded Event. If you are trying to obtain the 
        /// ' TimeExtent from a FeatureLayer, this will not work. The TimeSlider.Loaded occurs before the 
        /// ' FeatureLayer.Intialized Event so the FeatureLayer.TimeExtent will always be Nothing/null. Using the 
        /// ' TimeSlider.Loaded Event is best when the TimeExtent values are known but the TimeSlider.Intervals 
        /// ' just need constructing.
        ///   
        /// Private Sub TimeSlider1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   ' are a Collection of IEnumerable(Of Date) objects. When the TimeSlider.Intervals Property is set along with 
        ///   ' the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
        ///   ' functionality includes buttons for Play, Pause, Forward, and Back.
        ///   
        ///   ' Obtain the start and end Date/Time values from the TimeSlider named TimeSlider1 that was defined in XAML.
        ///   Dim myMinimumDate As Date = TimeSlider1.MinimumValue
        ///   Dim myMaximumDate As Date = TimeSlider1.MaximumValue
        ///   
        ///   ' Create a TimeExtent based upon the start and end date/times.
        ///   Dim myTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate)
        ///   
        ///   ' Create a new TimeSpan (1 day in our case).
        ///   Dim myTimeSpan As New TimeSpan(1, 0, 0, 0)
        ///   
        ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
        ///   Dim myIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
        ///   
        ///   ' Load all of Dates into the Collection of IEnumerable(Of Date) objects using the 
        ///   ' TimeSlider.CreateTimeStopsByTimeInterval Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan)
        ///   
        ///   ' Set the TimeSlider.Intervals which define the tic marks along the TimeSlider track to the IEnumerable(Of Date) 
        ///   ' objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates
        ///   
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First)
        ///   
        /// End Sub
        /// 
        /// Private Sub Slider1_ValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        ///   
        ///   ' This function converts the output from the regular Microsoft Slider Control in Ticks into a the
        ///   ' TimeSpan object which the ESRI TimeSlider control likes.
        ///   
        ///   Dim aTimeSpan As New TimeSpan(sender.Value)
        ///   
        ///   ' Make sure the TimeSlider has been completely created before we attempt to set a PlaySpeed value.
        ///   If TimeSlider1 IsNot Nothing Then
        ///     
        ///     ' Set the PlaySpeed to the TimeSpan.
        ///     TimeSlider1.PlaySpeed = aTimeSpan
        ///     
        ///   End If
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public TimeSpan PlaySpeed
		{
			get { return (TimeSpan)GetValue(PlaySpeedProperty); }
			set { SetValue(PlaySpeedProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="PlaySpeed"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PlaySpeedProperty =
			DependencyProperty.Register("PlaySpeed", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(TimeSpan.FromSeconds(1), OnPlaySpeedPropertyChanged));

		private static void OnPlaySpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;
			TimeSpan newValue = (TimeSpan)e.NewValue;
			obj.playTimer.Interval = newValue;
		}

		private void SetButtonVisibility()
		{
			Visibility viz = (Intervals != null && Intervals.GetEnumerator().MoveNext()) ? Visibility.Visible : Visibility.Collapsed;
			
			//Play Button
			if ( PlayPauseButton != null ) PlayPauseButton.Visibility = viz;
			
			//Next Button
			if ( NextButton != null ) NextButton.Visibility = viz;
			
			//Previous Button
			if (PreviousButton != null ) PreviousButton.Visibility = viz;		
		}

		private void TimeSlider_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			CreateTickmarks();
			UpdateTrackLayout(ValidValue);
		}

        /// <summary>
        /// Determines how the TimeSlider Control will represent a TimeExtent as the thumb(s) moves across the tick marks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The TimeExtent that is shown visually in the TimeSlider Control is corresponds to the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Value">TimeSlider.Value</see> Property. The TimeMode Property is 
        /// set to one of three Enumerations:
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// <b>TimeMode.CumulativeFromStart</b>: This TimeMode setting will draw all observations on the Map from the oldest 
        /// to the current as the MaximumThumb crosses a tick mark. It draws observations in a 'cumulative' effect. As the 
        /// animation occurs, the TimeSlider.Value.Start will remain static but the TimeSlider.Value.End will constantly 
        /// change. The MinimumThumb is not shown and is implied to be the same as the TimeSlider.Value.Start. The color in 
        /// the Foreground of the TimeSlider will shade from the TimeSlider.Value.Start through the TimeSlider.Value.End as 
        /// animation occurs. This is the default TimeMode setting on the TimeSlider. See the following screen shot:
        /// <para>
        /// <img border="0" alt="The TimeSlider TimeMode.CumulativeFromStart." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.CumulativeFromStart.png"/>
        /// </para>
        /// </item>
        /// <item>
        /// <b>TimeMode.TimeExtent</b>: This TimeMode setting will draw only those observations on the Map that fall in a 
        /// 'window of time' as defined by the TimeSlider.Value. As the thumbs (MinimumThumb and MaximumThumb) of the 
        /// 'window of time' move across the tick marks the older observations will disappear and newer ones will appear. 
        /// As the animation occurs, the TimeSlider.Value.Start (analogous to the MinimumThumb) will constantly change and 
        /// the TimeSlider.Value.End (analogous to the MaximumThumb) will constantly change but the two difference between 
        /// the Start and End times will be fixed. The Foreground shading of the TimeSlider will display in between the two 
        /// thumbs as the animation occurs. See the following screen shot:
        /// <para>
        /// <img border="0" alt="The TimeSlider TimeMode.TimeExtent." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.TimeExtent.png"/>
        /// </para>
        /// </item>
        /// <item>
        /// <b>TimeMode.TimeInstant</b>: This TimeMode setting will draw only the most recent observations on the Map as the 
        /// thumb(s) (MinimumThumb and MaximumThumb) crosses a tick mark. It draws observations in a 'snapshot' effect. As the 
        /// animation occurs, the TimeSlider.Value.Start and the TimeSlider.Value.End will be exactly the same but values will 
        /// change continuously. The MinimumThumb and MaximumThumb overlap each other exactly and give the visual appearance of 
        /// only one thumb being show to demonstrate the snapshot effect. No Foreground shading occurs in the TimeSlider in the 
        /// TimeMode.TimeInstant setting. See the following screen shot:
        /// <para>
        /// <img border="0" alt="The TimeSlider TimeMode.TimeInstant." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.TimeInstant.png"/>
        /// </para>
        /// </item>
        /// </list>
        /// </para>        
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the 'Initialize TimeSlider' button to set up the TimeSlider's functionality. Then click the play button on 
        /// the TimeSlider to view the animation. Click the different TimeMode RadioButtons to see the visual effect on the Map 
        /// and the TimeSlider.Value Property (aka. TimeExtent).
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating the various TimeMode Enumerations of the TimeSlider." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.TimeMode.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="63" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///            Width="537" TextWrapping="Wrap" Margin="12,0,0,0" 
        ///            Text="Click the 'Initialize TimeSlider' button to set up the TimeSlider's functionality. Then 
        ///              click the play button on the TimeSlider to view the animation. Click the different 
        ///              TimeMode RadioButtons to see the visual effect on the Map and the TimeSlider.Value 
        ///              Property (aka. TimeExtent)." /&gt;
        ///   
        ///   &lt;!--
        ///   Add a Map Control with an ArcGISTiledMapServiceLayer and a FeatureLayer. The ArcGISTiledMapsServiceLayer 
        ///   is the first Layer in Map.LayerCollection and is drawn on the bottom. The FeatureLayer is then added and 
        ///   draws on top. Set the Map.Extent to zoom to the middle of the Atlantic Ocean.
        ///       
        ///   Setting the Map.TimeExtent acts like a Where clause in that only those features/records that fall
        ///   within the set TimeSlider.Value (which is a narrowed TimeExtent) will then be shown. By Binding the 
        ///   Map.TimeExtent to the TimeSlider.Value Property, the Map will automatically update as the thumb on 
        ///   the TimeSlider moves.
        ///   --&gt;
        ///   &lt;esri:Map Name="Map1" Background="White" Height="375" Width="375" Margin="12,93,0,0" 
        ///             VerticalAlignment="Top" HorizontalAlignment="Left" Extent="-69.78,0.52,10.01,80.33"
        ///             TimeExtent="{Binding ElementName=TimeSlider1, Path=Value}"&gt;
        ///     
        ///     &lt;!-- Add an ArcGISTiledMapsServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="StreetMapLayer"
        ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer"/&gt;
        ///       
        ///     &lt;!--
        ///     The FeatureLayer contains Hurricane data from NOAA as Markers (aka. Points). The Where clause is 
        ///     optional. It is necessary when more that 500/1000 records returned. In ArcGIS Server 9.3.1 and prior, 
        ///     the default maximum is 500 records returned per FeatureLayer. In ArcGIS Server 10 the default is 1000. 
        ///     This setting is configurable per map service using ArcCatalog or ArcGIS Server Manager (on the Parameters
        ///     tab). 
        ///       
        ///     The Where clause gets only hurricane data for Alberto (a much smaller subset of the entire service).
        ///       
        ///     Specify the Outfields Property to specify which Fields are returned. Specifying the wildcard (*) 
        ///     character will return all Fields. 
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="MyFeatureLayer" Where="EVENTID = 'Alberto'" OutFields="*"
        ///           Url="http://servicesbeta.esri.com/ArcGIS/rest/services/Hurricanes/Hurricanes/MapServer/0"/&gt;
        ///       
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a TimeSlider. The initialization of the TimeSlider will be handled in the code-behind when the
        ///   user clicks the Button_InitializeTimeSlider. The initialization of the TimeSlider will handle setting
        ///   the Properties of .MinimumValue, .MaximumValue, .Intervals, .PlaySpeed, .Loop, and .TimeMode.
        ///   --&gt;
        ///   &lt;esri:TimeSlider Name="TimeSlider1" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///                    Height="25" Width="375" Margin="12,67,0,0"/&gt;
        ///     
        ///   &lt;!-- Add a Button to initialize the TimeSlider. The Click Event has associated code-behind. --&gt;
        ///   &lt;Button Name="Button_InitializeTimeSlider" Content="Initialize TimeSlider" Height="23" Width="170" 
        ///           HorizontalAlignment="Left" VerticalAlignment="Top" Margin="401,69,0,0" 
        ///           Click="Button_InitializeTimeSlider_Click" /&gt;
        ///     
        ///   &lt;!--
        ///   Add three RadioButtons to control the TimeMode Property on the TimeSlider. Each RadioButton has
        ///   a Click event with associated code-behind. 
        ///   --&gt;
        ///   &lt;sdk:Label Name="Label_TimeMode" Content="TimeMode:" Height="28" Width="120" Margin="401,117,0,0" 
        ///              VerticalAlignment="Top" HorizontalAlignment="Left" /&gt;
        ///   &lt;RadioButton Name="RadioButton_CumulativeFromStart" Content="CumulativeFromStart" Height="16" 
        ///                HorizontalAlignment="Left" VerticalAlignment="Top" Margin="401,136,0,0" 
        ///                Click="RadioButton_CumulativeFromStart_Click"/&gt;
        ///   &lt;RadioButton Name="RadioButton_TimeExtent" Content="TimeExtent" Height="16" 
        ///                HorizontalAlignment="Left" VerticalAlignment="Top" Margin="401,158,0,0" 
        ///                Click="RadioButton_TimeExtent_Click"/&gt;
        ///   &lt;RadioButton Name="RadioButton_TimeInstant" Content="TimeInstant" Height="16" 
        ///                HorizontalAlignment="Left" VerticalAlignment="Top" Margin="401,180,0,0" 
        ///                Click="RadioButton_TimeInstant_Click"/&gt;
        ///   
        ///   &lt;!--
        ///   Add a TextBlock to display the TimeSlider.Value.Start. The TextBlock.Content Property uses Binding
        ///   to automatically update any changes from the TimeSlider.Value.Start as the thumb of the TimeSlider
        ///   moves. Depending on the TimeSlider.TimeMode setting you will notice different behavior of what
        ///   gets displayed in the TextBlock.
        ///   --&gt;
        ///   &lt;sdk:Label Name="Label_TimeSliderValueStart" Content="TimeSlider.Value.Start:" Margin="403,229,0,0" 
        ///              Height="28" Width="131" HorizontalAlignment="Left" VerticalAlignment="Top" /&gt;
        ///   &lt;TextBlock Name="TextBlock_StartTime" Text="{Binding ElementName=TimeSlider1, Path=Value.Start}" 
        ///              HorizontalAlignment="Left" VerticalAlignment="Top" Width="175" Height="23" Margin="401,248,0,0" /&gt;
        ///   
        ///   &lt;!--
        ///   Add a TextBlock to display the TimeSlider.Value.End. The TextBlock.Content Property uses Binding
        ///   to automatically update any changes from the TimeSlider.Value.End as the thumb of the TimeSlider
        ///   moves. Depending on the TimeSlider.TimeMode setting you will notice different behavior of what
        ///   gets displayed in the TextBlock.
        ///   --&gt;
        ///   &lt;sdk:Label Name="Label_TimeSliderValueEnd" Content="TimeSlider.Value.End:" Margin="401,279,0,0" 
        ///              Height="29" Width="120" HorizontalAlignment="Left" VerticalAlignment="Top" /&gt;
        ///   &lt;TextBlock Name="TextBlock_EndTime" Text="{Binding ElementName=TimeSlider1, Path=Value.End}" 
        ///              HorizontalAlignment="Left" VerticalAlignment="Top" Width="175" Height="23" Margin="401,295,0,0" /&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button_InitializeTimeSlider_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function sets up the various initialization Properties for the TimeSlider (.MinimumValue, 
        ///   // .MaximumValue, .Intervals, .PlaySpeed, .Loop and .TimeMode). 
        ///   
        ///   // NOTE: 
        ///   // The bare minimum Properties that need to be set on the TimeSlider in order to have functioning 
        ///   // interaction between the TimeSlider and Map Control's are .MimimumValue, .MaximumValue, and 
        ///   // .Value (as well as binding the Map.TimeExtent to the TimeSlider.Value). This will give a 
        ///   // TimeSlider where the user can move the thumb(s) and have the Map Control respond accordingly. 
        ///   // But having only this bare minimum will not provide the full functionality of the PlayPauseButton, 
        ///   // NextButton, and PreviousButton being visible. In order to have the full functionality of the 
        ///   // TimeSlider the .Intervals Property must also be set.
        ///      
        ///   // Obtain the FeatureLayer that has the temporal data in the Map Control.
        ///   ESRI.ArcGIS.Client.FeatureLayer theFeatureLayer = Map1.Layers["MyFeatureLayer"];
        ///   
        ///   // Using the custom GetTimeExtentFromGraphicCollection function, obtain the a TimeExtent (i.e. a span of time) 
        ///   // from the GraphicCollection of the FeatureLayer. The FeatureLayer.TimeExtent Property could have been used 
        ///   // but that would have returned the entire TimeExtent of the FeatureLayer map service. Since our FeatureLayer
        ///   // was defined in XAML to have a Where clause to restrict the data returned to be only those temporal events
        ///   // associated with hurricane Alberto, we only want the TimeExtent associated with hurricane Alberto. The 
        ///   // GetTimeExtentFromGraphicCollection function obtains the TimeExtent for just the features in the 
        ///   // GraphicCollection associated with hurricane Alberto and not the entire FeatureLayer.TimeExtent.
        ///   ESRI.ArcGIS.Client.GraphicCollection theGraphicCollection = theFeatureLayer.Graphics;
        ///   ESRI.ArcGIS.Client.TimeExtent theTimeExtent = GetTimeExtentFromGraphicCollection(theGraphicCollection);
        ///   
        ///   // Set the TimeSlider's .MinimumValue and .MaximumValue Properties from Alberto's TimeExtent.
        ///   DateTime theStartDate = theTimeExtent.Start;
        ///   DateTime theEndDate = theTimeExtent.End;
        ///   TimeSlider1.MinimumValue = theStartDate;
        ///   TimeSlider1.MaximumValue = theEndDate;
        ///   
        ///   // Create a new TimeSpan (6 hours in our case). 
        ///   TimeSpan theTimeSpan = new TimeSpan(0, 6, 0, 0);
        ///   
        ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
        ///   System.Collections.Generic.IEnumerable&lt;Date&gt; theIEnumerableDates = null;
        ///   
        ///   // Load all of DateTimes into the IEnumerable&lt;DateTime&gt; object using the Shared/Static function
        ///   // TimeSlider.CreateTimeStopsByTimeInterval.
        ///   theIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(theTimeExtent, theTimeSpan);
        ///   
        ///   // Set the TimeSlider.Intervals to define the tick marks along the TimeSlider track from the IEnumerable&lt;DateTime&gt; 
        ///   // object. Each tick mark will be 6 hours apart starting from theTimeExtent.Start and ending with theTimeExtent.End.
        ///   TimeSlider1.Intervals = theIEnumerableDates;
        ///   
        ///   // Initialize the thumb's initial start position of the TimeSlider to be the leftmost. The individual DateTime objects 
        ///   // within the IEnumerable&lt;DateTime&gt; are ordered in the from oldest (.First) to newest (.Last). You could have also
        ///   // used theTimeExtent.Start.
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(theIEnumerableDates.First);
        ///   
        ///   // Set the TimeSlider.TimeMode to CumulativeFromStart and set the RadioButton_CumulativeFromStart as being
        ///   // checked. Users can interactively change the TimeMode and see the different effects once the TimeSlider is 
        ///   // initialized.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart;
        ///   RadioButton_CumulativeFromStart.IsChecked = true;
        ///   
        ///   // The default TimeSlider.Play Speed is 1 second but we want a faster redraw of the Graphics, so we are setting
        ///   // it to 1/2 of a second. The following 4 constructors all produce the same results: 
        ///   TimeSlider1.PlaySpeed = new TimeSpan(500000); // ticks
        ///   //TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0.5); // hours, minutes, seconds
        ///   //TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0.5); // days, hours, minutes, seconds
        ///   //TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0, 50); // days, hours, minutes, seconds, milliseconds
        ///   
        ///   // Enable continuous playback of the TimeSlider.
        ///   TimeSlider1.Loop = true;
        /// }
        ///   
        /// public ESRI.ArcGIS.Client.TimeExtent GetTimeExtentFromGraphicCollection(ESRI.ArcGIS.Client.GraphicCollection myGraphicCollection)
        /// {
        ///   // This function obtains the a TimeExtent (i.e. a span of time) from a GraphicCollection. An assumption is made
        ///   // specific to the FeatureLayer in this example that an Attribute called 'Date_Time' is present (which is the
        ///   // Date of the temporal observation of the hurricane). It is also assumed that ALL of the temporal observations
        ///   // have valid DateTime information. If there are any temporal observations that have Nothing/null values, the logic
        ///   // of this function will need modification.
        ///   
        ///   // Define new variables to hold the start and end Dates of all the temporal observations in the GraphicCollection.
        ///   DateTime startDate = new DateTime();
        ///   DateTime endDate = new DateTime();
        ///   
        ///   // Loop through each Graphic in the GraphicCollection.
        ///   foreach (ESRI.ArcGIS.Client.Graphic theGraphic in myGraphicCollection)
        ///   {
        ///     // Obtain the DateTime of the temporal observation from the Graphic.Attribute. If you use your own map service 
        ///     // dataset, you will need to change the Attribute field name accordingly.
        ///     DateTime aDate = theGraphic.Attributes["Date_Time"];
        ///     
        ///     // Seed the initial startDate with the first observation in the GraphicCollection.
        ///     if (startDate == new DateTime())
        ///     {
        ///   	startDate = aDate;
        ///     }
        ///     
        ///     // Seed the initial endDate with the first observation in the GraphicCollection.
        ///     if (endDate == new DateTime())
        ///     {
        ///   	endDate = aDate;
        ///     }
        ///   
        ///     // If the aDate is earlier than startDate then set it to the startDate. 
        ///     if (aDate &lt; startDate)
        ///     {
        ///   	startDate = aDate;
        ///     }
        ///     
        ///     // If the aDate is later than endDate then set it to the endDate. 
        ///     if (aDate &gt; endDate)
        ///     {
        ///   	endDate = aDate;
        ///     }
        ///   }
        ///   
        ///   // Construct the TimeExtent from the startDate and endDate.
        ///   ESRI.ArcGIS.Client.TimeExtent theTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(startDate, endDate);
        ///   
        ///   // Return the TimeExtent to the caller.
        ///   return theTimeExtent;
        /// }
        ///   
        /// private void RadioButton_CumulativeFromStart_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This TimeMode setting will draw all points on the Map from the oldest observation to the current
        ///   // observation as the thumb crosses a tick mark. It draws observations in a 'cumulative' effect. As 
        ///   // the animation occurs, the TimeSlider.Value.Start will remain static but the TimeSlider.Value.End
        ///   // will constantly change.
        ///   
        ///   // Set the TimeSlider.TimeMode to the TimeMode.CumulativeFromStart enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart;
        ///   
        ///   // Move the thumb of the TimeSlider to the leftmost position.
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.First);
        /// }
        ///   
        /// private void RadioButton_TimeExtent_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This TimeMode setting will draw only those points on the Map that fall in a 'window of time' as defined
        ///   // by the TimeSlider.Value. As the thumb of the 'window of time' moves across the tick marks the older 
        ///   // observations will disappear and newer ones will appear. As the animation occurs, the TimeSlider.Value.Start 
        ///   // will constantly change and the TimeSlider.Value.End will constantly change but the two difference between 
        ///   // the Start and End times will be fixed.
        ///   
        ///   // Set the TimeSlider.TimeMode to the TimeMode.Time enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent;
        ///   
        ///   // Get the IEnumerable&lt;DateTime&gt; from the TimeSlider.Intervals Property.
        ///   IEnumerable&lt;DateTime&gt; myIntervals = TimeSlider1.Intervals;
        ///   
        ///   // Obtain the first DateTime observation in the IEnumerable&lt;DateTime&gt;, which is the oldest DateTime. 
        ///   DateTime theStartDate = myIntervals.First();
        ///   
        ///   // Set the thumb of the TimeSlider to be a 'window of time' that covers a 2 day (48 hours) period. The 
        ///   // thumb that represents a 'window of time' will be moved to the leftmost position. 
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(theStartDate, theStartDate.AddHours(48));
        /// }
        ///   
        /// private void RadioButton_TimeInstant_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This TimeMode setting will draw only the most recent observations on the Map as the thumb crosses 
        ///   // a tick mark. It draws observations in a 'snapshot' effect. As the animation occurs, the 
        ///   // TimeSlider.Value.Start and the TimeSlider.Value.End will be exactly the same but values will change
        ///   // continuously.
        ///   
        ///   // Set the TimeSlider.TimeMode to the TimeMode.TimeInstant enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant;
        ///   
        ///   // Move the thumb of the TimeSlider to the leftmost position.
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.First);
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button_InitializeTimeSlider_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This function sets up the various initialization Properties for the TimeSlider (.MinimumValue, 
        ///   ' .MaximumValue, .Intervals, .PlaySpeed, .Loop and .TimeMode). 
        ///   
        ///   ' NOTE: 
        ///   ' The bare minimum Properties that need to be set on the TimeSlider in order to have functioning 
        ///   ' interaction between the TimeSlider and Map Control's are .MimimumValue, .MaximumValue, and 
        ///   ' .Value (as well as binding the Map.TimeExtent to the TimeSlider.Value). This will give a 
        ///   ' TimeSlider where the user can move the thumb(s) and have the Map Control respond accordingly. 
        ///   ' But having only this bare minimum will not provide the full functionality of the PlayPauseButton, 
        ///   ' NextButton, and PreviousButton being visible. In order to have the full functionality of the 
        ///   ' TimeSlider the .Intervals Property must also be set.
        ///   
        ///   ' Obtain the FeatureLayer that has the temporal data in the Map Control.
        ///   Dim theFeatureLayer As ESRI.ArcGIS.Client.FeatureLayer = Map1.Layers.Item("MyFeatureLayer")
        ///   
        ///   ' Using the custom GetTimeExtentFromGraphicCollection function, obtain the a TimeExtent (i.e. a span of time) 
        ///   ' from the GraphicCollection of the FeatureLayer. The FeatureLayer.TimeExtent Property could have been used 
        ///   ' but that would have returned the entire TimeExtent of the FeatureLayer map service. Since our FeatureLayer
        ///   ' was defined in XAML to have a Where clause to restrict the data returned to be only those temporal events
        ///   ' associated with hurricane Alberto, we only want the TimeExtent associated with hurricane Alberto. The 
        ///   ' GetTimeExtentFromGraphicCollection function obtains the TimeExtent for just the features in the 
        ///   ' GraphicCollection associated with hurricane Alberto and not the entire FeatureLayer.TimeExtent.
        ///   Dim theGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection = theFeatureLayer.Graphics
        ///   Dim theTimeExtent As ESRI.ArcGIS.Client.TimeExtent = GetTimeExtentFromGraphicCollection(theGraphicCollection)
        ///   
        ///   ' Set the TimeSlider's .MinimumValue and .MaximumValue Properties from Alberto's TimeExtent.
        ///   Dim theStartDate As Date = theTimeExtent.Start
        ///   Dim theEndDate As Date = theTimeExtent.End
        ///   TimeSlider1.MinimumValue = theStartDate
        ///   TimeSlider1.MaximumValue = theEndDate
        ///   
        ///   ' Create a new TimeSpan (6 hours in our case). 
        ///   Dim theTimeSpan As New TimeSpan(0, 6, 0, 0)
        ///   
        ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
        ///   Dim theIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
        ///   
        ///   ' Load all of Dates into the IEnumerable(Of Date) object using the Shared/Static function
        ///   ' TimeSlider.CreateTimeStopsByTimeInterval.
        ///   theIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(theTimeExtent, theTimeSpan)
        ///    
        ///   ' Set the TimeSlider.Intervals to define the tick marks along the TimeSlider track from the IEnumerable(Of Date) 
        ///   ' object. Each tick mark will be 6 hours apart starting from theTimeExtent.Start and ending with theTimeExtent.End.
        ///   TimeSlider1.Intervals = theIEnumerableDates
        ///   
        ///   ' Initialize the thumb's initial start position of the TimeSlider to be the leftmost. The individual Date objects 
        ///   ' within the IEnumerable(Of Date) are ordered in the from oldest (.First) to newest (.Last). You could have also
        ///   ' used theTimeExtent.Start.
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(theIEnumerableDates.First)
        /// 
        ///   ' Set the TimeSlider.TimeMode to CumulativeFromStart and set the RadioButton_CumulativeFromStart as being
        ///   ' checked. Users can interactively change the TimeMode and see the different effects once the TimeSlider is 
        ///   ' initialized.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        ///   RadioButton_CumulativeFromStart.IsChecked = True
        ///   
        ///   ' The default TimeSlider.Play Speed is 1 second but we want a faster redraw of the Graphics, so we are setting
        ///   ' it to 1/2 of a second. The following 4 constructors all produce the same results: 
        ///   TimeSlider1.PlaySpeed = New TimeSpan(500000) ' ticks
        ///   'TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0.5) ' hours, minutes, seconds
        ///   'TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0.5) ' days, hours, minutes, seconds
        ///   'TimeSlider1.PlaySpeed = New TimeSpan(0, 0, 0, 0, 50) ' days, hours, minutes, seconds, milliseconds
        ///   
        ///   ' Enable continuous playback of the TimeSlider.
        ///   TimeSlider1.Loop = True
        ///   
        /// End Sub
        /// 
        /// Public Function GetTimeExtentFromGraphicCollection(ByVal myGraphicCollection As ESRI.ArcGIS.Client.GraphicCollection) As ESRI.ArcGIS.Client.TimeExtent
        ///   
        ///   ' This function obtains the a TimeExtent (i.e. a span of time) from a GraphicCollection. An assumption is made
        ///   ' specific to the FeatureLayer in this example that an Attribute called 'Date_Time' is present (which is the
        ///   ' Date of the temporal observation of the hurricane). It is also assumed that ALL of the temporal observations
        ///   ' have valid Date information. If there are any temporal observations that have Nothing/null values, the logic
        ///   ' of this function will need modification.
        ///    
        ///   ' Define new variables to hold the start and end Dates of all the temporal observations in the GraphicCollection.
        ///   Dim startDate As New Date
        ///   Dim endDate As New Date
        ///   
        ///   ' Loop through each Graphic in the GraphicCollection.
        ///   For Each theGraphic As ESRI.ArcGIS.Client.Graphic In myGraphicCollection
        ///   
        ///     ' Obtain the Date of the temporal observation from the Graphic.Attribute. If you use your own map service 
        ///     ' dataset, you will need to change the Attribute field name accordingly.
        ///     Dim aDate As Date = theGraphic.Attributes.Item("Date_Time")
        ///     
        ///     ' Seed the initial startDate with the first observation in the GraphicCollection.
        ///     If startDate = Nothing Then
        ///       startDate = aDate
        ///     End If
        ///     
        ///     ' Seed the initial endDate with the first observation in the GraphicCollection.
        ///     If endDate = Nothing Then
        ///       endDate = aDate
        ///     End If
        ///     
        ///     ' If the aDate is earlier than startDate then set it to the startDate. 
        ///     If aDate &lt; startDate Then
        ///       startDate = aDate
        ///     End If
        ///     
        ///     ' If the aDate is later than endDate then set it to the endDate. 
        ///     If aDate &gt; endDate Then
        ///       endDate = aDate
        ///     End If
        ///      
        ///   Next
        ///    
        ///   ' Construct the TimeExtent from the startDate and endDate.
        ///   Dim theTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(startDate, endDate)
        ///      
        ///   ' Return the TimeExtent to the caller.
        ///   Return theTimeExtent
        ///    
        /// End Function
        ///  
        /// Private Sub RadioButton_CumulativeFromStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///    
        ///   ' This TimeMode setting will draw all points on the Map from the oldest observation to the current
        ///   ' observation as the thumb crosses a tick mark. It draws observations in a 'cumulative' effect. As 
        ///   ' the animation occurs, the TimeSlider.Value.Start will remain static but the TimeSlider.Value.End
        ///   ' will constantly change.
        ///   
        ///   ' Set the TimeSlider.TimeMode to the TimeMode.CumulativeFromStart enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.CumulativeFromStart
        ///   
        ///   ' Move the thumb of the TimeSlider to the leftmost position.
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.First)
        ///   
        /// End Sub
        ///   
        /// Private Sub RadioButton_TimeExtent_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This TimeMode setting will draw only those points on the Map that fall in a 'window of time' as defined
        ///   ' by the TimeSlider.Value. As the thumb of the 'window of time' moves across the tic marks the older 
        ///   ' observations will disappear and newer ones will appear. As the animation occurs, the TimeSlider.Value.Start 
        ///   ' will constantly change and the TimeSlider.Value.End will constantly change but the two difference between 
        ///   ' the Start and End times will be fixed.
        ///   
        ///   ' Set the TimeSlider.TimeMode to the TimeMode.Time enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeExtent
        ///   
        ///   ' Get the IEnumerable(Of Date) from the TimeSlider.Intervals Property.
        ///   Dim myIntervals As IEnumerable(Of Date) = TimeSlider1.Intervals
        ///   
        ///   ' Obtain the first Date observation in the IEnumerable(Of Date), which is the oldest Date. 
        ///   Dim theStartDate As Date = myIntervals.First()
        ///   
        ///   ' Set the thumb of the TimeSlider to be a 'window of time' that covers a 2 day (48 hours) period. The 
        ///   ' thumb that represents a 'window of time' will be moved to the leftmost position. 
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(theStartDate, theStartDate.AddHours(48))
        ///   
        /// End Sub
        ///   
        /// Private Sub RadioButton_TimeInstant_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' This TimeMode setting will draw only the most recent observations on the Map as the thumb crosses 
        ///   ' a tick mark. It draws observations in a 'snapshot' effect. As the animation occurs, the 
        ///   ' TimeSlider.Value.Start and the TimeSlider.Value.End will be exactly the same but values will change
        ///   ' continuously.
        ///   
        ///   ' Set the TimeSlider.TimeMode to the TimeMode.TimeInstant enumeration.
        ///   TimeSlider1.TimeMode = ESRI.ArcGIS.Client.Toolkit.TimeMode.TimeInstant
        ///   
        ///   ' Move the thumb of the TimeSlider to the leftmost position.
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(TimeSlider1.Intervals.First)
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public TimeMode TimeMode
		{
			get { return (TimeMode)GetValue(TimeModeProperty); }
			set { SetValue(TimeModeProperty, value); }
		}
		
		/// <summary>
		/// Identifies the <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode"/> dependancy property.
		/// </summary>
		public static readonly DependencyProperty TimeModeProperty =
			DependencyProperty.Register("TimeMode", typeof(TimeMode), typeof(TimeSlider), new PropertyMetadata(TimeMode.CumulativeFromStart, OnTimeModePropertyChange));

		private static void OnTimeModePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;									
			obj.UpdateTrackLayout(obj.ValidValue);	
		}

		/// <summary>
		/// Identifies the <see cref="IsPlaying"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsPlayingProperty =
			DependencyProperty.Register("IsPlaying", typeof(bool), typeof(TimeSlider), new PropertyMetadata(false, OnIsPlayingPropertyChanged));

		private static void OnIsPlayingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider obj = (TimeSlider)d;
			bool newValue = (bool)e.NewValue;
			if (newValue && obj.Intervals != null && obj.Intervals.GetEnumerator().MoveNext())
				obj.playTimer.Start();
			else
				obj.playTimer.Stop();
			if(obj.PlayPauseButton != null)
				obj.PlayPauseButton.IsChecked = newValue;
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the selected time extent has changed.
		/// </summary>
		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		/// <summary>
		/// <see cref="RoutedEventArgs"/> used when raising the <see cref="ValueChanged"/> event.
		/// </summary>
		public sealed class ValueChangedEventArgs : RoutedEventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
			/// </summary>
			/// <param name="newValue">The new <see cref="TimeExtent"/> value.</param>
			/// <param name="oldValue">The old <see cref="TimeExtent"/> value.</param>
			internal ValueChangedEventArgs(TimeExtent newValue, TimeExtent oldValue)
			{
				NewValue = newValue;
				OldValue = oldValue;
			}
			/// <summary>
			/// Gets the new <see cref="TimeExtent"/> value.
			/// </summary>
			/// <value>The new value.</value>
			public TimeExtent NewValue { get; private set; }
			/// <summary>
			/// Gets the old <see cref="TimeExtent"/> value.
			/// </summary>
			/// <value>The old value.</value>
			public TimeExtent OldValue { get; private set; }
		}

		#endregion

		#region Static helper methods

		/// <summary>
		/// Creates the specified number of time stops evenly distributed in the time extent.
		/// </summary>
		/// <param name="extent">The time extent.</param>
		/// <param name="count">Number of stops.</param>
		/// <returns>IEnumerable of time stops.</returns>
		public static IEnumerable<DateTime> CreateTimeStopsByCount(TimeExtent extent, int count)
		{			
			long span = (extent.End - extent.Start).Ticks / (count - 1);
			DateTime d = extent.Start;
			for (int i = 0; i < count-1; i++)
			{
				yield return d;				
				try { d = d.AddTicks(span); }
				catch (ArgumentOutOfRangeException) { }				
			}
			yield return extent.End;
		}

		/// <summary>
		/// Creates time stops within an interval dispersed with by specified <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="extent">The time extent.</param>
		/// <param name="interval">Interval between each time stop.</param>
		/// <returns>IEnumerable of time stops.</returns>
		public static IEnumerable<DateTime> CreateTimeStopsByTimeInterval(TimeExtent extent, TimeSpan interval)
		{
			DateTime d = extent.Start;
			while (d <= extent.End)
			{
				yield return d;
				try { d = d.Add(interval); }
				catch (ArgumentOutOfRangeException) { }
			}			
		}

		#endregion

		#region Play methods

		/// <summary>
		/// Jumps to the next position in the intervals.
		/// </summary>
		/// <returns><c>true</c> if succeeded, <c>false</c> if there are no <see cref="Intervals"/> 
		/// or the <see cref="MaximumValue"/> was reached.</returns>
		/// <see cref="Intervals"/>
		public bool Next()
		{
			if (Intervals == null || ValidValue == null) return false;
			DateTime nextA = ValidValue.Start;
			DateTime nextB = ValidValue.End;
			IEnumerator<DateTime> enumerator = Intervals.GetEnumerator();
			bool next = false;
			bool hasMore = enumerator.MoveNext();
			while (hasMore)
			{
				if (next)
				{					
					nextB = enumerator.Current;
					break;
				}
				if (enumerator.Current == ValidValue.End)
					next = true;
				hasMore = enumerator.MoveNext();
			}
			if (!hasMore) return false; //reached the end
			next = false; 
			if (TimeMode == TimeMode.TimeExtent || TimeMode == TimeMode.TimeInstant)
			{
				enumerator = Intervals.GetEnumerator();
				hasMore = enumerator.MoveNext();
				while (hasMore)
				{
					if (next)
					{
						nextA = enumerator.Current;
						break;
					}
					if (enumerator.Current == ValidValue.Start)
						next = true;
					hasMore = enumerator.MoveNext();
				}
			}
			
			Value = new TimeExtent(nextA, nextB);
			return true;
		}

		/// <summary>
		/// Jumps to the previous position in the intervals.
		/// </summary>
		/// <returns><c>true</c> if succeeded, <c>false</c> if there are no <see cref="Intervals"/> 
		/// or the <see cref="MinimumValue"/> was reached.</returns>
		/// <see cref="Intervals"/>
		public bool Previous()
		{
			if (Intervals == null || ValidValue == null) return false;
			DateTime nextA = ValidValue.Start;
			DateTime nextB = ValidValue.End;
			IEnumerator<DateTime> enumerator = Intervals.GetEnumerator();
			bool hasMore = enumerator.MoveNext();
			if (!hasMore) return false;
			DateTime temp;
			if (TimeMode == TimeMode.TimeExtent || TimeMode == TimeMode.TimeInstant)
			{
				temp = enumerator.Current;
				if (temp == ValidValue.Start)
					return false;
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ValidValue.Start)
					{
						nextA = temp;
						break;
					}
					temp = enumerator.Current;
				}
			}
			if (ValidValue.Start == ValidValue.End)
			{
				Value = new TimeExtent(nextA);
			}
			else
			{
				temp = nextB;
				while (hasMore)
				{
					if (enumerator.Current == ValidValue.End)
					{
						nextB = temp;
						break;
					}
					temp = enumerator.Current;
					hasMore = enumerator.MoveNext();
				}
				if (!hasMore) return false; //reached the end
				Value = new TimeExtent(nextA, nextB);
			}
			return true;
		}

        /// <summary>
        /// Gets or set as Boolean indicating whether the TimeSlider is playing through the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.Intervals">Intervals</see> of the TickBar.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A value of True means the animating of the thumb(s) is occurring. A value of False means no animation is 
        /// occurring. The default TimeSlider Control uses a ToggleButton for the PlayPauseButton to control the 
        /// animations. When the Pause icon is visible that means animation is occurring (i.e. .IsPlaying=True). When 
        /// the Play icon is visible, that means the animation has stopped (i.e. .IsPlaying=False). See the following 
        /// screen shot for a visual depiction:
        /// </para>
        /// <para>
        /// <img border="0" alt="The PlayPauseButton and the .IsPlaying Property of the TimeSlider Control." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.IsPlaying.png"/>
        /// </para>
        /// <para>
        /// Under most circumstances it is not necessary to set the IsPlaying Property via code as this Property is 
        /// automatically managed as part of the default TimeSlider Control. It is possible to modify the default behavior 
        /// of the PlayPauseButton sub-component of the TimeSlider by editing its Control Template; see the code example in 
        /// this document.  
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Once the FeatureLayer has initialized the customized TimeSlider will have all of the necessary Properties 
        /// set in order to run. The normal PlayPauseButton has been removed as part of the Control Template of the 
        /// TimeSlider. Use the 'Start' and 'Stop' buttons to control the functionality of the TimeSlider's animation 
        /// via the .IsPlaying Property.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Demonstrating the TimeSlider.IsPlaying Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.IsPlaying2.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;!-- 
        /// Don't forget to add the xml namespace reference:
        /// xmlns:esriToolkitPrimitives="clr-namespace:ESRI.ArcGIS.Client.Toolkit.Primitives;assembly=ESRI.ArcGIS.Client.Toolkit" 
        /// --&gt;
        ///   
        /// &lt;Grid x:Name="LayoutRoot" Background="White"&gt;
        ///   &lt;Grid.Resources&gt;
        ///   
        ///     &lt;!--
        ///     The majority of the XAML that defines the ControlTemplate for the Bookmark Control was obtained
        ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
        ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
        ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
        ///     --&gt;
        ///     &lt;Style x:Key="TimeSliderStyle1" TargetType="esri:TimeSlider"&gt;
        ///       &lt;Setter Property="IsTabStop" Value="True"/&gt;
        ///       &lt;Setter Property="Foreground"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0"&gt;
        ///             &lt;GradientStop Color="#00ffffff"/&gt;
        ///             &lt;GradientStop Color="#FF326FC0" Offset="0.5"/&gt;
        ///             &lt;GradientStop Color="#00ffffff" Offset="1"/&gt;
        ///           &lt;/LinearGradientBrush&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///       &lt;Setter Property="Background" Value="White"/&gt;
        ///       &lt;Setter Property="BorderBrush" Value="Black"/&gt;
        ///       &lt;Setter Property="BorderThickness" Value="1"/&gt;
        ///       &lt;Setter Property="Template"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;ControlTemplate TargetType="esri:TimeSlider"&gt;
        ///             &lt;Grid&gt;
        ///               &lt;Grid.Resources&gt;
        ///                 
        ///                 &lt;!-- 
        ///                 Comment out the entire 'Style' section of the PlayPauseToggleButtonStyle since it is not used 
        ///                 in our example!
        ///                 --&gt;
        ///                 &lt;!--
        ///                 &lt;Style x:Key="PlayPauseToggleButtonStyle" TargetType="ToggleButton"&gt;
        ///                   &lt;Setter Property="Background" Value="#FF1F3B53"/&gt;
        ///                   &lt;Setter Property="Foreground" Value="#FF000000"/&gt;
        ///                   &lt;Setter Property="Padding" Value="3"/&gt;
        ///                   &lt;Setter Property="BorderThickness" Value="1"/&gt;
        ///                   &lt;Setter Property="BorderBrush"&gt;
        ///                     &lt;Setter.Value&gt;
        ///                       &lt;LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0"&gt;
        ///                         &lt;GradientStop Color="#FFA3AEB9" Offset="0"/&gt;
        ///                         &lt;GradientStop Color="#FF8399A9" Offset="0.375"/&gt;
        ///                         &lt;GradientStop Color="#FF718597" Offset="0.375"/&gt;
        ///                         &lt;GradientStop Color="#FF617584" Offset="1"/&gt;
        ///                       &lt;/LinearGradientBrush&gt;
        ///                     &lt;/Setter.Value&gt;
        ///                   &lt;/Setter&gt;
        ///                   &lt;Setter Property="Template"&gt;
        ///                     &lt;Setter.Value&gt;
        ///                       &lt;ControlTemplate TargetType="ToggleButton"&gt;
        ///                         &lt;Grid&gt;
        ///                           &lt;VisualStateManager.VisualStateGroups&gt;
        ///                             &lt;VisualStateGroup x:Name="CommonStates"&gt;
        ///                               &lt;VisualState x:Name="Normal"/&gt;
        ///                               &lt;VisualState x:Name="MouseOver"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="BackgroundAnimation"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#F2FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[2].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#CCFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[3].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#7FFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Pressed"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" Storyboard.TargetName="Background"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#FF6DBDD1"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="BackgroundAnimation"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[0].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#D8FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#C6FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[2].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#8CFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[3].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#3FFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Disabled"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="DisabledVisualElement"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value=".55"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                             &lt;VisualStateGroup x:Name="CheckStates"&gt;
        ///                               &lt;VisualState x:Name="Checked"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PlayContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Collapsed&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PauseContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Unchecked"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PlayContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PauseContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Collapsed&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                             &lt;VisualStateGroup x:Name="FocusStates"&gt;
        ///                               &lt;VisualState x:Name="Focused"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="FocusVisualElement"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                  &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Unfocused"/&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                           &lt;/VisualStateManager.VisualStateGroups&gt;
        ///                           &lt;Border x:Name="Background" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" Background="White" CornerRadius="3"&gt;
        ///                             &lt;Grid Background="{TemplateBinding Background}" Margin="1"&gt;
        ///                               &lt;Border x:Name="BackgroundAnimation" Background="#FF448DCA" Opacity="0"/&gt;
        ///                               &lt;Rectangle x:Name="BackgroundGradient"&gt;
        ///                                 &lt;Rectangle.Fill&gt;
        ///                                   &lt;LinearGradientBrush EndPoint=".7,1" StartPoint=".7,0"&gt;
        ///                                     &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt;
        ///                                     &lt;GradientStop Color="#F9FFFFFF" Offset="0.375"/&gt;
        ///                                     &lt;GradientStop Color="#E5FFFFFF" Offset="0.625"/&gt;
        ///                                     &lt;GradientStop Color="#C6FFFFFF" Offset="1"/&gt;
        ///                                   &lt;/LinearGradientBrush&gt;
        ///                                 &lt;/Rectangle.Fill&gt;
        ///                               &lt;/Rectangle&gt;
        ///                             &lt;/Grid&gt;
        ///                           &lt;/Border&gt;
        ///                           &lt;Grid HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" Margin="{TemplateBinding Padding}" VerticalAlignment="{TemplateBinding VerticalContentAlignment}"&gt;
        ///                             &lt;Grid x:Name="PlayContent" FlowDirection="LeftToRight" Height="9" Margin="1" Width="9"&gt;
        ///                               &lt;Path Data="M0,0 L0,9 9,4.5 0,0" Fill="{TemplateBinding Foreground}"/&gt;
        ///                             &lt;/Grid&gt;
        ///                             &lt;Grid x:Name="PauseContent" Height="9" Margin="1" Visibility="Collapsed" Width="9"&gt;
        ///                               &lt;Rectangle Fill="{TemplateBinding Foreground}" HorizontalAlignment="Left" Width="3"/&gt;
        ///                               &lt;Rectangle Fill="{TemplateBinding Foreground}" HorizontalAlignment="Right" Width="3"/&gt;
        ///                             &lt;/Grid&gt;
        ///                           &lt;/Grid&gt;
        ///                           &lt;Rectangle x:Name="DisabledVisualElement" Fill="#FFFFFFFF" IsHitTestVisible="false" Opacity="0" RadiusY="3" RadiusX="3"/&gt;
        ///                           &lt;Rectangle x:Name="FocusVisualElement" IsHitTestVisible="false" Margin="1" Opacity="0" RadiusY="2" RadiusX="2" Stroke="#FF6DBDD1" StrokeThickness="1"/&gt;
        ///                         &lt;/Grid&gt;
        ///                       &lt;/ControlTemplate&gt;
        ///                     &lt;/Setter.Value&gt;
        ///                   &lt;/Setter&gt;
        ///                 &lt;/Style&gt;
        ///                 --&gt;
        ///                 
        ///               &lt;/Grid.Resources&gt;
        ///               &lt;Grid.ColumnDefinitions&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                 &lt;ColumnDefinition Width="*"/&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///               &lt;/Grid.ColumnDefinitions&gt;
        ///               &lt;VisualStateManager.VisualStateGroups&gt;
        ///                 &lt;VisualStateGroup x:Name="CommonStates"&gt;
        ///                   &lt;VisualState x:Name="Normal"/&gt;
        ///                   &lt;VisualState x:Name="MouseOver"/&gt;
        ///                   &lt;VisualState x:Name="Disabled"&gt;
        ///                     &lt;Storyboard&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="HorizontalTrackRectangleDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="MinimumThumbDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="MaximumThumbDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                     &lt;/Storyboard&gt;
        ///                   &lt;/VisualState&gt;
        ///                 &lt;/VisualStateGroup&gt;
        ///                 &lt;VisualStateGroup x:Name="FocusStates"&gt;
        ///                   &lt;VisualState x:Name="Focused"&gt;
        ///                     &lt;Storyboard&gt;
        ///                       &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="FocusVisualElement"&gt;
        ///                         &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                       &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                     &lt;/Storyboard&gt;
        ///                   &lt;/VisualState&gt;
        ///                   &lt;VisualState x:Name="Unfocused"/&gt;
        ///                 &lt;/VisualStateGroup&gt;
        ///               &lt;/VisualStateManager.VisualStateGroups&gt;
        ///               
        ///               &lt;!-- 
        ///               Comment out the 'PlayPauseButton' since we will control the functionality of the TimeSlider via
        ///               our own custom buttons (Button_Start and Button_Stop) using the TimeSlider.IsPlaying Property
        ///               in the code-behind.
        ///               --&gt;
        ///               &lt;!--
        ///               &lt;ToggleButton x:Name="PlayPauseButton" Grid.Column="0" Height="17" Style="{StaticResource PlayPauseToggleButtonStyle}" VerticalAlignment="Center" Width="17"/&gt;
        ///               --&gt;
        ///                   
        ///               &lt;Rectangle Grid.Column="1" Fill="{TemplateBinding Background}" Margin="0,2,0,2" RadiusY="2" RadiusX="2" Stroke="{TemplateBinding BorderBrush}" StrokeThickness="{TemplateBinding BorderThickness}"/&gt;
        ///               &lt;Grid x:Name="HorizontalTrack" Grid.Column="1"&gt;
        ///                 &lt;esriToolkitPrimitives:TickBar x:Name="TickMarks" Grid.Column="0" IsHitTestVisible="False" Margin="5,0,5,0"&gt;
        ///                   &lt;esriToolkitPrimitives:TickBar.TickMarkTemplate&gt;
        ///                     &lt;DataTemplate&gt;
        ///                       &lt;Rectangle Height="{TemplateBinding Height}" Opacity="0.5" Stroke="Black" StrokeThickness="0.5" Width="1"/&gt;
        ///                     &lt;/DataTemplate&gt;
        ///                   &lt;/esriToolkitPrimitives:TickBar.TickMarkTemplate&gt;
        ///                 &lt;/esriToolkitPrimitives:TickBar&gt;
        ///                 &lt;RepeatButton x:Name="HorizontalTrackLargeChangeDecreaseRepeatButton" HorizontalAlignment="Stretch" IsTabStop="False" Opacity="0"/&gt;
        ///                 &lt;Thumb x:Name="MinimumThumb" Cursor="Hand" DataContext="{TemplateBinding Value}" HorizontalAlignment="Left" IsTabStop="False" ToolTipService.Placement="Top" ToolTipService.ToolTip="{Binding Start}" Width="10"/&gt;
        ///                 &lt;Rectangle x:Name="MinimumThumbDisabledOverlay" Fill="White" HorizontalAlignment="Left" Opacity=".55" RadiusY="2" RadiusX="2" Visibility="Collapsed" Width="10"/&gt;
        ///                 &lt;Thumb x:Name="HorizontalTrackThumb" Foreground="{TemplateBinding Foreground}" HorizontalAlignment="Left" IsTabStop="False"&gt;
        ///                   &lt;Thumb.Template&gt;
        ///                     &lt;ControlTemplate&gt;
        ///                       &lt;Rectangle Fill="{TemplateBinding Foreground}"/&gt;
        ///                     &lt;/ControlTemplate&gt;
        ///                   &lt;/Thumb.Template&gt;
        ///                 &lt;/Thumb&gt;
        ///                 &lt;Border x:Name="HorizontalTrackRectangleDisabledOverlay" Background="Red" CornerRadius="5" HorizontalAlignment="Left" Opacity="1" Visibility="Collapsed"/&gt;
        ///                 &lt;Thumb x:Name="MaximumThumb" Cursor="Hand" DataContext="{TemplateBinding Value}" HorizontalAlignment="Left" IsTabStop="False" ToolTipService.Placement="Top" ToolTipService.ToolTip="{Binding End}" Width="10"/&gt;
        ///                 &lt;Rectangle x:Name="MaximumThumbDisabledOverlay" Fill="White" HorizontalAlignment="Left" Height="Auto" Opacity=".55" RadiusY="2" RadiusX="2" Visibility="Collapsed" Width="10"/&gt;
        ///                 &lt;RepeatButton x:Name="HorizontalTrackLargeChangeIncreaseRepeatButton" HorizontalAlignment="Stretch" IsTabStop="False" Opacity="0"/&gt;
        ///               &lt;/Grid&gt;
        ///               &lt;Rectangle x:Name="FocusVisualElement" Grid.Column="1" IsHitTestVisible="false" Margin="1" Opacity="0" RadiusY="2" RadiusX="2" Stroke="#FF6DBDD1" StrokeThickness="1"/&gt;
        ///               &lt;Button x:Name="PreviousButton" Grid.Column="2" Height="17" Padding="3,0" VerticalAlignment="Center" Width="17"&gt;
        ///                 &lt;Grid&gt;
        ///                   &lt;Path Data="M 5,0 L 5,9 0,4.5 5,0" Fill="Black"/&gt;
        ///                   &lt;Rectangle Fill="Black" HorizontalAlignment="Left" Height="9" Margin="6,0,0,0" VerticalAlignment="Top" Width="1"/&gt;
        ///                 &lt;/Grid&gt;
        ///               &lt;/Button&gt;
        ///               &lt;Button x:Name="NextButton" Grid.Column="3" Height="17" Padding="3,0" VerticalAlignment="Center" Width="17"&gt;
        ///                 &lt;Grid&gt;
        ///                   &lt;Path Data="M 0,0 L 0,9 5,4.5 0,0" Fill="Black" Margin="1.5,0,0,0"/&gt;
        ///                   &lt;Rectangle Fill="Black" HorizontalAlignment="Left" Height="9" VerticalAlignment="Top" Width="1"/&gt;
        ///                 &lt;/Grid&gt;
        ///               &lt;/Button&gt;
        ///             &lt;/Grid&gt;
        ///           &lt;/ControlTemplate&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///     &lt;/Style&gt;
        ///     
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="64" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" 
        ///            Width="616" TextWrapping="Wrap" Margin="12,0,0,0" 
        ///            Text="Once the FeatureLayer has initialized the customized TimeSlider will have all of the necessary
        ///              Properties set in order to run. The normal PlayPauseButton has been removed as part of the Control 
        ///              Template of the TimeSlider. Use the 'Start' and 'Stop' buttons to control the functionality
        ///              of the TimeSlider's animation via the .IsPlaying Property." /&gt;
        ///   
        ///   &lt;!--
        ///   Add a Map Control with an ArcGISTiledMapServiceLayer and a FeatureLayer. The ArcGISTiledMapsServiceLayer 
        ///   is the first Layer in Map.LayerCollection and is drawn on the bottom. The FeatureLayer is then added and 
        ///   draws on top. Set the Map.Extent to zoom to the middle of the Atlantic Ocean.
        ///   
        ///   Setting the Map.TimeExtent acts like a Where clause in that only those features/records that fall
        ///   within the set TimeSlider.Value (which is a narrowed TimeExtent) will then be shown. By Binding the 
        ///   Map.TimeExtent to the TimeSlider.Value Property, the Map will automatically update as the thumb on 
        ///   the TimeSlider moves.
        ///   --&gt;
        ///   &lt;esri:Map Name="Map1" Background="White" Height="375" Width="375" Margin="12,93,0,0" 
        ///             VerticalAlignment="Top" HorizontalAlignment="Left" Extent="-103.04,-13.82,2.08,91.30"
        ///             TimeExtent="{Binding ElementName=TimeSlider1, Path=Value}"&gt;
        ///   
        ///     &lt;!-- Add an ArcGISTiledMapsServiceLayer. --&gt;
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="StreetMapLayer"
        ///           Url="http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer"/&gt;
        ///     
        ///     &lt;!--
        ///     The FeatureLayer contains Hurricane data from NOAA as Markers (aka. Points). Adding a Where clause is 
        ///     optional. It is necessary when more that 500/1000 records returned. In ArcGIS Server 9.3.1 and prior, 
        ///     the default maximum is 500 records returned per FeatureLayer. In ArcGIS Server 10 the default is 1000. 
        ///     This setting is configurable per map service using ArcCatalog or ArcGIS Server Manager (on the Parameters
        ///     tab). 
        ///     
        ///     Specify the Outfields Property to specify which Fields are returned. Specifying the wildcard (*) 
        ///     character will return all Fields. 
        ///     --&gt;
        ///     &lt;esri:FeatureLayer ID="MyFeatureLayer"  OutFields="*"
        ///           Url="http://servicesbeta.esri.com/ArcGIS/rest/services/Hurricanes/Hurricanes/MapServer/0"
        ///           Initialized="FeatureLayer_Initialized"/&gt;
        ///     
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!--
        ///   Add a TimeSlider. The initialization of the TimeSlider .Intervals and .Values Properties will be handled in
        ///   the code-behind as part of the FeatureLayer_Initilized Event.
        ///       
        ///   The other TimeSlider initialization values of .MinimumValue, .MaximumValue, .TimeMode, Loop and .PlaySpeed
        ///   will be handled here in the XAML.
        ///       
        ///   PlaySpeed uses the format: "days:hours:seconds".
        ///       
        ///   The Style Property makes use of the StaticResource 'TimeSliderStyle1' which was defined earlier in the XAML
        ///   to customize the look and feel via a ControlTemplate.
        ///   --&gt;
        ///   &lt;esri:TimeSlider Name="TimeSlider1" HorizontalAlignment="Left" VerticalAlignment="Top" 
        ///                    Height="25" Width="375" Margin="12,67,0,0"
        ///                    MinimumValue="{Binding ElementName=Map1,Path=Layers[1].TimeExtent.Start}"
        ///                    MaximumValue="{Binding ElementName=Map1,Path=Layers[1].TimeExtent.End}"
        ///                    TimeMode="CumulativeFromStart" Loop="True" PlaySpeed="00:00:00.1"
        ///                    Style="{StaticResource TimeSliderStyle1}"/&gt;
        ///   
        ///   &lt;!-- 
        ///   TimeSlider.IsPlaying is a Dependency Property meaning that we can use Binding to automatically 
        ///   display the state of .IsPlaying in a TextBox.
        ///   --&gt;
        ///   &lt;sdk:Label Content="TimeSlider.IsPlaying:" Name="Label_TimeSliderIsPlaying" Margin="402,70,0,0"
        ///              HorizontalAlignment="Left" VerticalAlignment="Top" Height="28" Width="120" /&gt;
        ///   &lt;TextBlock Height="23" HorizontalAlignment="Left" Margin="401,85,0,0" Name="TextBlock_TimeSlider_IsPlaying" 
        ///              Text="{Binding ElementName=TimeSlider1,Path=IsPlaying}" VerticalAlignment="Top" Width="133" /&gt;
        ///   
        ///   &lt;!--
        ///   Add a button to control the start of the animation in the TimeSlider. The logic of the 
        ///   button is in the code-behind file. 
        ///   --&gt;
        ///   &lt;Button Name="Button_Start" Content="Start" Height="23" Width="121" Margin="401,125,0,0" 
        ///           HorizontalAlignment="Left" VerticalAlignment="Top" Click="Button_Start_Click"
        ///           IsEnabled="False"/&gt;
        ///   
        ///   &lt;!--
        ///   Add a button to control the stop of the animation in the TimeSlider. The logic of the 
        ///   button is in the code-behind file. 
        ///   --&gt;
        ///   &lt;Button Name="Button_Stop" Content="Stop" Height="23"  Width="121" Margin="401,154,0,0" 
        ///           HorizontalAlignment="Left" VerticalAlignment="Top" Click="Button_Stop_Click"
        ///           IsEnabled="False"/&gt;
        ///   
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void FeatureLayer_Initialized(object sender, System.EventArgs e)
        /// {
        ///   // This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   // are a Collection of IEnumerable&lt;DateTime&gt; objects. When the TimeSlider.Intervals Property is set along with 
        ///   // the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. 
        ///   
        ///   // Obtain the start and end DateTime values from the TimeSlider named TimeSlider1 that was defined in XAML.
        ///   DateTime myMinimumDate = TimeSlider1.MinimumValue;
        ///   DateTime myMaximumDate = TimeSlider1.MaximumValue;
        ///   
        ///   // Create a TimeExtent based upon the start and end DateTimes.
        ///   ESRI.ArcGIS.Client.TimeExtent myTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate);
        ///   
        ///   // Create a new TimeSpan (1 day in our case).
        ///   TimeSpan myTimeSpan = new TimeSpan(1, 0, 0, 0);
        ///   
        ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
        ///   System.Collections.Generic.IEnumerable&lt;DateTime&gt; myIEnumerableDates = null;
        ///   
        ///   // Load all of DateTimes into the Collection of IEnumerable&lt;DateTime&gt; objects using the 
        ///   // TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan);
        ///   
        ///   // Set the TimeSlider.Intervals which define the tick marks along the TimeSlider track to the IEnumerable&lt;DateTime&gt; 
        ///   // objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates;
        ///   
        ///   // Define the initial starting position of the thumb along the TimeSlider.
        ///   TimeSlider1.Value = new ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First);
        ///   
        ///   // Enable the Button_Start.
        ///   Button_Start.IsEnabled = true;
        /// }
        /// 
        /// private void Button_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Start the animation of the TimeSlider.
        ///   TimeSlider1.IsPlaying = true;
        ///   Button_Start.IsEnabled = false;
        ///   Button_Stop.IsEnabled = true;
        /// }
        /// 
        /// private void Button_Stop_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Stop the animation of the TimeSlider.
        ///   TimeSlider1.IsPlaying = false;
        ///   Button_Start.IsEnabled = true;
        ///   Button_Stop.IsEnabled = false;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub FeatureLayer_Initialized(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ///   
        ///   ' This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   ' are a Collection of IEnumerable(Of Date) objects. When the TimeSlider.Intervals Property is set along with 
        ///   ' the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. 
        ///   
        ///   ' Obtain the start and end Date/Time values from the TimeSlider named TimeSlider1 that was defined in XAML.
        ///   Dim myMinimumDate As Date = TimeSlider1.MinimumValue
        ///   Dim myMaximumDate As Date = TimeSlider1.MaximumValue
        ///   
        ///   ' Create a TimeExtent based upon the start and end date/times.
        ///   Dim myTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate)
        ///   
        ///   ' Create a new TimeSpan (1 day in our case).
        ///   Dim myTimeSpan As New TimeSpan(1, 0, 0, 0)
        ///   
        ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
        ///   Dim myIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
        ///   
        ///   ' Load all of Dates into the Collection of IEnumerable(Of Date) objects using the 
        ///   ' TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan)
        ///   
        ///   ' Set the TimeSlider.Intervals which define the tick marks along the TimeSlider track to the IEnumerable(Of Date) 
        ///   ' objects.
        ///   TimeSlider1.Intervals = myIEnumerableDates
        ///   
        ///   ' Define the initial starting position of the thumb along the TimeSlider.
        ///   TimeSlider1.Value = New ESRI.ArcGIS.Client.TimeExtent(myIEnumerableDates.First)
        ///   
        ///   ' Enable the Button_Start.
        ///   Button_Start.IsEnabled = True
        ///   
        /// End Sub
        ///   
        /// Private Sub Button_Start_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Start the animation of the TimeSlider.
        ///   TimeSlider1.IsPlaying = True
        ///   Button_Start.IsEnabled = False
        ///   Button_Stop.IsEnabled = True
        ///   
        /// End Sub
        ///   
        /// Private Sub Button_Stop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   
        ///   ' Stop the animation of the TimeSlider.
        ///   TimeSlider1.IsPlaying = False
        ///   Button_Start.IsEnabled = True
        ///   Button_Stop.IsEnabled = False
        ///   
        /// End Sub
        /// </code>
        /// </example>
		public bool IsPlaying
		{
			get { return (bool)GetValue(IsPlayingProperty); }
			set { SetValue(IsPlayingProperty, value); }
		}

        /// <summary>
        /// Gets or sets a value indicating whether the animating of the TimeSlider thumb(s) will restart playing 
        /// when the end of the TickBar is reached.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Loop Property of True allows continuous playing of the thumb(s) across the TickBar of the TimeSlider. 
        /// A False value means the thumb(s) will stop at the last Interval (i.e. the 
        /// <see cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.MaximumValue">MaximumValue</see>) in the TickBar when 
        /// it is reached. 
        /// </para>
        /// <para>
        /// The default Loop value of the TimeSlider is False.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// <b>How to use:</b>
        /// </para>
        /// <para>
        /// Click the 'Initialize the TimeSlider' button to initialize all the TimeSlider functions (PlayPauseButton, 
        /// NextButton, PreviousButton). Then push the play button to start the animations. Check on/off the Loop 
        /// CheckBox to see the different effects of the Loop Property. The code example demonstrates a modified 
        /// Control Template of the TimeSlider.
        /// </para>
        /// <para>
        /// The XAML code in this example is used in conjunction with the code-behind (C# or VB.NET) to demonstrate
        /// the functionality.
        /// </para>
        /// <para>
        /// The following screen shot corresponds to the code example in this page.
        /// </para>
        /// <para>
        /// <img border="0" alt="Modifying the Control Template of a TimeSlider to have a CheckBox to control the TimeSlider.Loop Property." src="C:\ArcGIS\dotNET\API SDK\Main\ArcGISSilverlightSDK\LibraryReference\images\Client.Toolkit.TimeSlider.Loop.png"/>
        /// </para>
        /// <code title="Example XAML1" description="" lang="XAML">
        /// &lt;!-- 
        /// Don't forget to add the xml namespace reference:
        /// xmlns:esriToolkitPrimitives="clr-namespace:ESRI.ArcGIS.Client.Toolkit.Primitives;assembly=ESRI.ArcGIS.Client.Toolkit" 
        /// --&gt;
        /// &lt;Grid x:Name="LayoutRoot"&gt;
        ///   &lt;Grid.Resources&gt;
        ///   
        ///     &lt;!-- 
        ///     Tip: Use x:Key Attribute and not x:Name Attribute for defining the name of the Resources so that
        ///     you can access the Resource in the code-behind file. 
        ///     --&gt;
        ///     
        ///     &lt;!-- The 'RedMarkerSymbol' will be used as the default symbol for the Graphics. --&gt;
        ///     &lt;esri:SimpleMarkerSymbol x:Key="RedMarkerSymbol" Color="Red" Size="12" Style="Circle" /&gt;
        ///     
        ///     &lt;!-- 
        ///     Define a SpatialReference object that has the same WKID as the ArcGISTiledMapServiceLayer in
        ///     the Map. This will allow for the Graphics in the GraphicsLayer to line up properly. 
        ///     --&gt;
        ///     &lt;esri:SpatialReference x:Key="theSpatialReference" WKID="102100"/&gt;
        ///     
        ///     &lt;!--
        ///     The majority of the XAML that defines the ControlTemplate for the Bookmark Control was obtained
        ///     by using Microsoft Blend. See the blog post entitled: 'Use control templates to customize the 
        ///     look and feel of ArcGIS controls' at the following Url for general How-To background:
        ///     http://blogs.esri.com/Dev/blogs/silverlightwpf/archive/2010/05/20/Use-control-templates-to-customize-the-look-and-feel-of-ArcGIS-controls.aspx
        ///     --&gt;
        ///     &lt;Style x:Key="TimeSliderStyle1" TargetType="esri:TimeSlider"&gt;
        ///       &lt;Setter Property="IsTabStop" Value="True"/&gt;
        ///       &lt;Setter Property="Foreground"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0"&gt;
        ///             &lt;GradientStop Color="#00ffffff"/&gt;
        ///             &lt;GradientStop Color="#FF326FC0" Offset="0.5"/&gt;
        ///             &lt;GradientStop Color="#00ffffff" Offset="1"/&gt;
        ///           &lt;/LinearGradientBrush&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///       &lt;Setter Property="Background" Value="White"/&gt;
        ///       &lt;Setter Property="BorderBrush" Value="Black"/&gt;
        ///       &lt;Setter Property="BorderThickness" Value="1"/&gt;
        ///       &lt;Setter Property="Template"&gt;
        ///         &lt;Setter.Value&gt;
        ///           &lt;ControlTemplate TargetType="esri:TimeSlider"&gt;
        ///             &lt;Grid&gt;
        ///               &lt;Grid.Resources&gt;
        ///                 &lt;Style x:Key="PlayPauseToggleButtonStyle" TargetType="ToggleButton"&gt;
        ///                   &lt;Setter Property="Background" Value="#FF1F3B53"/&gt;
        ///                   &lt;Setter Property="Foreground" Value="#FF000000"/&gt;
        ///                   &lt;Setter Property="Padding" Value="3"/&gt;
        ///                   &lt;Setter Property="BorderThickness" Value="1"/&gt;
        ///                   &lt;Setter Property="BorderBrush"&gt;
        ///                     &lt;Setter.Value&gt;
        ///                       &lt;LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0"&gt;
        ///                         &lt;GradientStop Color="#FFA3AEB9" Offset="0"/&gt;
        ///                         &lt;GradientStop Color="#FF8399A9" Offset="0.375"/&gt;
        ///                         &lt;GradientStop Color="#FF718597" Offset="0.375"/&gt;
        ///                         &lt;GradientStop Color="#FF617584" Offset="1"/&gt;
        ///                       &lt;/LinearGradientBrush&gt;
        ///                     &lt;/Setter.Value&gt;
        ///                   &lt;/Setter&gt;
        ///                   &lt;Setter Property="Template"&gt;
        ///                     &lt;Setter.Value&gt;
        ///                       &lt;ControlTemplate TargetType="ToggleButton"&gt;
        ///                         &lt;Grid&gt;
        ///                           &lt;VisualStateManager.VisualStateGroups&gt;
        ///                             &lt;VisualStateGroup x:Name="CommonStates"&gt;
        ///                               &lt;VisualState x:Name="Normal"/&gt;
        ///                               &lt;VisualState x:Name="MouseOver"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="BackgroundAnimation"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#F2FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[2].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#CCFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[3].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#7FFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Pressed"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)" Storyboard.TargetName="Background"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#FF6DBDD1"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="BackgroundAnimation"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[0].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#D8FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#C6FFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[2].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#8CFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                   &lt;ColorAnimationUsingKeyFrames Storyboard.TargetProperty="(Rectangle.Fill).(GradientBrush.GradientStops)[3].(GradientStop.Color)" Storyboard.TargetName="BackgroundGradient"&gt;
        ///                                     &lt;SplineColorKeyFrame KeyTime="0" Value="#3FFFFFFF"/&gt;
        ///                                   &lt;/ColorAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Disabled"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="DisabledVisualElement"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value=".55"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                             &lt;VisualStateGroup x:Name="CheckStates"&gt;
        ///                               &lt;VisualState x:Name="Checked"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PlayContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Collapsed&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PauseContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Unchecked"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PlayContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                   &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="PauseContent"&gt;
        ///                                     &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                                       &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                                         &lt;Visibility&gt;Collapsed&lt;/Visibility&gt;
        ///                                       &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                                     &lt;/DiscreteObjectKeyFrame&gt;
        ///                                   &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                             &lt;VisualStateGroup x:Name="FocusStates"&gt;
        ///                               &lt;VisualState x:Name="Focused"&gt;
        ///                                 &lt;Storyboard&gt;
        ///                                   &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="FocusVisualElement"&gt;
        ///                                     &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                                   &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                                 &lt;/Storyboard&gt;
        ///                               &lt;/VisualState&gt;
        ///                               &lt;VisualState x:Name="Unfocused"/&gt;
        ///                             &lt;/VisualStateGroup&gt;
        ///                           &lt;/VisualStateManager.VisualStateGroups&gt;
        ///                           &lt;Border x:Name="Background" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" Background="White" CornerRadius="3"&gt;
        ///                             &lt;Grid Background="{TemplateBinding Background}" Margin="1"&gt;
        ///                               &lt;Border x:Name="BackgroundAnimation" Background="#FF448DCA" Opacity="0"/&gt;
        ///                               &lt;Rectangle x:Name="BackgroundGradient"&gt;
        ///                                 &lt;Rectangle.Fill&gt;
        ///                                   &lt;LinearGradientBrush EndPoint=".7,1" StartPoint=".7,0"&gt;
        ///                                     &lt;GradientStop Color="#FFFFFFFF" Offset="0"/&gt;
        ///                                     &lt;GradientStop Color="#F9FFFFFF" Offset="0.375"/&gt;
        ///                                     &lt;GradientStop Color="#E5FFFFFF" Offset="0.625"/&gt;
        ///                                     &lt;GradientStop Color="#C6FFFFFF" Offset="1"/&gt;
        ///                                   &lt;/LinearGradientBrush&gt;
        ///                                 &lt;/Rectangle.Fill&gt;
        ///                               &lt;/Rectangle&gt;
        ///                             &lt;/Grid&gt;
        ///                           &lt;/Border&gt;
        ///                           &lt;Grid HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" Margin="{TemplateBinding Padding}" VerticalAlignment="{TemplateBinding VerticalContentAlignment}"&gt;
        ///                             &lt;Grid x:Name="PlayContent" FlowDirection="LeftToRight" Height="9" Margin="1" Width="9"&gt;
        ///                               &lt;Path Data="M0,0 L0,9 9,4.5 0,0" Fill="{TemplateBinding Foreground}"/&gt;
        ///                             &lt;/Grid&gt;
        ///                             &lt;Grid x:Name="PauseContent" Height="9" Margin="1" Visibility="Collapsed" Width="9"&gt;
        ///                               &lt;Rectangle Fill="{TemplateBinding Foreground}" HorizontalAlignment="Left" Width="3"/&gt;
        ///                               &lt;Rectangle Fill="{TemplateBinding Foreground}" HorizontalAlignment="Right" Width="3"/&gt;
        ///                             &lt;/Grid&gt;
        ///                           &lt;/Grid&gt;
        ///                           &lt;Rectangle x:Name="DisabledVisualElement" Fill="#FFFFFFFF" IsHitTestVisible="false" Opacity="0" RadiusY="3" RadiusX="3"/&gt;
        ///                           &lt;Rectangle x:Name="FocusVisualElement" IsHitTestVisible="false" Margin="1" Opacity="0" RadiusY="2" RadiusX="2" Stroke="#FF6DBDD1" StrokeThickness="1"/&gt;
        ///                         &lt;/Grid&gt;
        ///                       &lt;/ControlTemplate&gt;
        ///                     &lt;/Setter.Value&gt;
        ///                   &lt;/Setter&gt;
        ///                 &lt;/Style&gt;
        ///               &lt;/Grid.Resources&gt;
        ///               &lt;Grid.ColumnDefinitions&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                 &lt;ColumnDefinition Width="*"/&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                          
        ///                 &lt;!-- Add a placeholder for our Loop Checkbox. --&gt;
        ///                 &lt;ColumnDefinition Width="Auto"/&gt;
        ///                  
        ///               &lt;/Grid.ColumnDefinitions&gt;
        ///               &lt;VisualStateManager.VisualStateGroups&gt;
        ///                 &lt;VisualStateGroup x:Name="CommonStates"&gt;
        ///                   &lt;VisualState x:Name="Normal"/&gt;
        ///                   &lt;VisualState x:Name="MouseOver"/&gt;
        ///                   &lt;VisualState x:Name="Disabled"&gt;
        ///                     &lt;Storyboard&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="HorizontalTrackRectangleDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="MinimumThumbDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                       &lt;ObjectAnimationUsingKeyFrames Duration="0" Storyboard.TargetProperty="Visibility" Storyboard.TargetName="MaximumThumbDisabledOverlay"&gt;
        ///                         &lt;DiscreteObjectKeyFrame KeyTime="0"&gt;
        ///                           &lt;DiscreteObjectKeyFrame.Value&gt;
        ///                             &lt;Visibility&gt;Visible&lt;/Visibility&gt;
        ///                           &lt;/DiscreteObjectKeyFrame.Value&gt;
        ///                         &lt;/DiscreteObjectKeyFrame&gt;
        ///                       &lt;/ObjectAnimationUsingKeyFrames&gt;
        ///                     &lt;/Storyboard&gt;
        ///                   &lt;/VisualState&gt;
        ///                 &lt;/VisualStateGroup&gt;
        ///                 &lt;VisualStateGroup x:Name="FocusStates"&gt;
        ///                   &lt;VisualState x:Name="Focused"&gt;
        ///                     &lt;Storyboard&gt;
        ///                       &lt;DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="Opacity" Storyboard.TargetName="FocusVisualElement"&gt;
        ///                         &lt;SplineDoubleKeyFrame KeyTime="0" Value="1"/&gt;
        ///                       &lt;/DoubleAnimationUsingKeyFrames&gt;
        ///                     &lt;/Storyboard&gt;
        ///                   &lt;/VisualState&gt;
        ///                   &lt;VisualState x:Name="Unfocused"/&gt;
        ///                 &lt;/VisualStateGroup&gt;
        ///               &lt;/VisualStateManager.VisualStateGroups&gt;
        ///               &lt;ToggleButton x:Name="PlayPauseButton" Grid.Column="0" Height="17" Style="{StaticResource PlayPauseToggleButtonStyle}" VerticalAlignment="Center" Width="17"/&gt;
        ///               &lt;Rectangle Grid.Column="1" Fill="{TemplateBinding Background}" Margin="0,2,0,2" RadiusY="2" RadiusX="2" Stroke="{TemplateBinding BorderBrush}" StrokeThickness="{TemplateBinding BorderThickness}"/&gt;
        ///               &lt;Grid x:Name="HorizontalTrack" Grid.Column="1"&gt;
        ///                 &lt;esriToolkitPrimitives:TickBar x:Name="TickMarks" Grid.Column="0" IsHitTestVisible="False" Margin="5,0,5,0"&gt;
        ///                   &lt;esriToolkitPrimitives:TickBar.TickMarkTemplate&gt;
        ///                     &lt;DataTemplate&gt;
        ///                       &lt;Rectangle Height="{TemplateBinding Height}" Opacity="0.5" Stroke="Black" StrokeThickness="0.5" Width="1"/&gt;
        ///                     &lt;/DataTemplate&gt;
        ///                   &lt;/esriToolkitPrimitives:TickBar.TickMarkTemplate&gt;
        ///                 &lt;/esriToolkitPrimitives:TickBar&gt;
        ///                 &lt;RepeatButton x:Name="HorizontalTrackLargeChangeDecreaseRepeatButton" HorizontalAlignment="Stretch" IsTabStop="False" Opacity="0"/&gt;
        ///                 &lt;Thumb x:Name="MinimumThumb" Cursor="Hand" DataContext="{TemplateBinding Value}" HorizontalAlignment="Left" IsTabStop="False" ToolTipService.Placement="Top" ToolTipService.ToolTip="{Binding Start}" Width="10"/&gt;
        ///                 &lt;Rectangle x:Name="MinimumThumbDisabledOverlay" Fill="White" HorizontalAlignment="Left" Opacity=".55" RadiusY="2" RadiusX="2" Visibility="Collapsed" Width="10"/&gt;
        ///                 &lt;Thumb x:Name="HorizontalTrackThumb" Foreground="{TemplateBinding Foreground}" HorizontalAlignment="Left" IsTabStop="False"&gt;
        ///                   &lt;Thumb.Template&gt;
        ///                     &lt;ControlTemplate&gt;
        ///                       &lt;Rectangle Fill="{TemplateBinding Foreground}"/&gt;
        ///                     &lt;/ControlTemplate&gt;
        ///                   &lt;/Thumb.Template&gt;
        ///                 &lt;/Thumb&gt;
        ///                 &lt;Border x:Name="HorizontalTrackRectangleDisabledOverlay" Background="Red" CornerRadius="5" HorizontalAlignment="Left" Opacity="1" Visibility="Collapsed"/&gt;
        ///                 &lt;Thumb x:Name="MaximumThumb" Cursor="Hand" DataContext="{TemplateBinding Value}" HorizontalAlignment="Left" IsTabStop="False" ToolTipService.Placement="Top" ToolTipService.ToolTip="{Binding End}" Width="10"/&gt;
        ///                 &lt;Rectangle x:Name="MaximumThumbDisabledOverlay" Fill="White" HorizontalAlignment="Left" Height="Auto" Opacity=".55" RadiusY="2" RadiusX="2" Visibility="Collapsed" Width="10"/&gt;
        ///                 &lt;RepeatButton x:Name="HorizontalTrackLargeChangeIncreaseRepeatButton" HorizontalAlignment="Stretch" IsTabStop="False" Opacity="0"/&gt;
        ///               &lt;/Grid&gt;
        ///               &lt;Rectangle x:Name="FocusVisualElement" Grid.Column="1" IsHitTestVisible="false" Margin="1" Opacity="0" RadiusY="2" RadiusX="2" Stroke="#FF6DBDD1" StrokeThickness="1"/&gt;
        ///               &lt;Button x:Name="PreviousButton" Grid.Column="2" Height="17" Padding="3,0" VerticalAlignment="Center" Width="17"&gt;
        ///                 &lt;Grid&gt;
        ///                   &lt;Path Data="M 5,0 L 5,9 0,4.5 5,0" Fill="Black"/&gt;
        ///                  &lt;Rectangle Fill="Black" HorizontalAlignment="Left" Height="9" Margin="6,0,0,0" VerticalAlignment="Top" Width="1"/&gt;
        ///                 &lt;/Grid&gt;
        ///               &lt;/Button&gt;
        ///               &lt;Button x:Name="NextButton" Grid.Column="3" Height="17" Padding="3,0" VerticalAlignment="Center" Width="17"&gt;
        ///                 &lt;Grid&gt;
        ///                   &lt;Path Data="M 0,0 L 0,9 5,4.5 0,0" Fill="Black" Margin="1.5,0,0,0"/&gt;
        ///                   &lt;Rectangle Fill="Black" HorizontalAlignment="Left" Height="9" VerticalAlignment="Top" Width="1"/&gt;
        ///                 &lt;/Grid&gt;
        ///               &lt;/Button&gt;
        ///               
        ///               &lt;!-- 
        ///               Add a CheckBox that will provide the ability for the user to turn on/off the Loop
        ///               Property of the TimeSlider. Code-behind functions were added for the Checked and
        ///               Unchecked states of the CheckBox.
        ///               --&gt;
        ///               &lt;CheckBox x:Name="MyLooper" Grid.Column="4" Height="17" Width="50" Padding="3,0" Margin="2"
        ///                         VerticalAlignment="Center" Content="Loop" 
        ///                         Checked="MyLooper_Checked" Unchecked="MyLooper_Unchecked"
        ///                         IsChecked="True"/&gt;
        ///         
        ///             &lt;/Grid&gt;
        ///           &lt;/ControlTemplate&gt;
        ///         &lt;/Setter.Value&gt;
        ///       &lt;/Setter&gt;
        ///     &lt;/Style&gt;
        ///     
        ///   &lt;/Grid.Resources&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a Map control with an ArcGISTiledMapServiceLayer and a GraphicsLayer. The Map.TimeExtent is
        ///   bound to the MyTimeSlider (TimeSlider) control. The MyTimeSlider will control what TimeExtent (i.e.
        ///   slices of time) can be viewed in the Map control.
        ///         
        ///   The GraphicsLayer will contain several Graphics based upon MapPoint geometries (which use the 
        ///   defined SpatialReference) which use the RedMarkerSymbol as the default symbolization, and have various 
        ///   TimeExtent values set. When the specific Graphic elements have a TimeExtent that falls within the 
        ///   Map.TimeExtent will they be displayed based upon the MyTimeSlider settings.
        ///   --&gt;
        ///   &lt;esri:Map Background="White" HorizontalAlignment="Left" Margin="12,150,0,0" Name="Map1" 
        ///           VerticalAlignment="Top" Height="318" Width="341" 
        ///           TimeExtent="{Binding ElementName=MyTimeSlider, Path=Value}"&gt;
        ///   
        ///     &lt;esri:ArcGISTiledMapServiceLayer ID="PhysicalTiledLayer" 
        ///                  Url="http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer" /&gt;
        ///     
        ///     &lt;!-- 
        ///     It is important to provide the GraphicsLayer with an 'ID' Attribute so to be able to access it
        ///     in the code-behind file. 
        ///     --&gt;
        ///     &lt;esri:GraphicsLayer ID="MyGraphicsLayer" &gt;
        ///     
        ///       &lt;esri:GraphicsLayer.Graphics&gt;
        ///       
        ///         &lt;!-- 
        ///         Each Graphic added to the GraphicsLayer will have it's symbology, TimeExtent and geometry defined. 
        ///         --&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/04 12:00:01 UTC" &gt;
        ///           &lt;esri:MapPoint X="-7356594.25" Y="4752385.95" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/07 06:30:00 UTC"&gt;
        ///           &lt;esri:MapPoint X="5654893.89" Y="3718746.02" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/10 05:15:15 UTC"&gt;
        ///           &lt;esri:MapPoint X="-13654893.89" Y="-1718746.02" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/14 03:03:00 UTC"&gt;
        ///           &lt;esri:MapPoint X="3654893.89" Y="7718746.02" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/18 09:11:51 UTC"&gt;
        ///           &lt;esri:MapPoint X="6801033.36" Y="10325547.30" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/20 01:00:03 UTC"&gt;
        ///           &lt;esri:MapPoint X="-5468910.57" Y="1741081.03" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         &lt;esri:Graphic Symbol="{StaticResource RedMarkerSymbol}" TimeExtent="2000/08/22 18:23:43 UTC"&gt;
        ///           &lt;esri:MapPoint X="-4614958.43" Y="-326382.05" SpatialReference="{StaticResource theSpatialReference}"/&gt;
        ///         &lt;/esri:Graphic&gt;
        ///         
        ///       &lt;/esri:GraphicsLayer.Graphics&gt;
        ///     &lt;/esri:GraphicsLayer&gt;
        ///   &lt;/esri:Map&gt;
        ///   
        ///   &lt;!-- 
        ///   Add a TimeSlider to control the display of what geographic features are displayed in the Map Control
        ///   based upon a specified TimeExtent. In the case of this sample code, when the specific Graphic elements 
        ///   have a TimeExtent that falls within the Map.TimeExtent will they be displayed based upon the MyTimeSlider 
        ///   settings.
        ///   
        ///   Tip: It is the x:Name Attribute that allows you to access the TimeSlider Control in the code-behind file. 
        ///     
        ///   The Loop Attribute of True allows continuous playing of the TimeSlider once it initialized in the code-behind.
        ///       
        ///   The PlaySpeed Attribute follows the format of "hh:mm:ss" where, hh = hours (0 to 24), mm = minutes (0 to 60),
        ///   and ss = seconds (0 to 60). In this example the PlaySpeed increments the time intervals every tenth of a second
        ///   (i.e. 0.1).
        ///       
        ///   The TimeMode Attribute of CumulativeFromStart means there is a fixed start date/time (2000/08/04 00:00:00 UTC
        ///   in this example) that does not change and an end date/time that adjusts as the specified Interval (one day 
        ///   in this example which is set in the code-behind) increases. As the 'thumb' of the TimeSlider control moves
        ///   to the right along the slider track the TimeExtent Interval of the TimeSlider increases.
        ///       
        ///   The MinimumValue Attribute specifies the starting date/time of the TimeSlider track.
        ///       
        ///   The MaximumValue Attribute specifies the ending date/time of the TimeSlider track.
        ///     
        ///   The Value Attribute specifies the date/time location of the thumb along the TimeSlider track. The thumb can 
        ///   have a start date/time and end date/time set for a TimeExtent which will display a window of time as the
        ///   thumb moves along the TimeSlider track but this is best for TimeMode Attribute of 'TimeExtent'. Since
        ///   this example is showing a TimeMode of 'CumulativeFromStart' it is best to have the thumb just use a single
        ///   date/time specified for the Value set to the same date/time as the MinimumValue.
        ///     
        ///   The last thing needed to enable the full capabilities of a TimeSlider (i.e. having the PlayPause, Next, 
        ///   and Previous buttons) is to set the Intervals Property. In Silverlight, this can only be done in code-behind to 
        ///   construct the Collection Type of IEnumerable(Of Date). Without the TimeSlider.Intervals being set the user 
        ///   has to manually move the thumb across the TimeSlider track to change the Map.TimeExtent and thereby see what 
        ///   Graphics can be displayed for that date/time window.
        ///   --&gt;
        ///   &lt;esri:TimeSlider x:Name="MyTimeSlider" 
        ///                     Loop="True" PlaySpeed="0:0:0.1"
        ///                     TimeMode="CumulativeFromStart"                 
        ///                     MinimumValue="2000/08/04 00:00:00 UTC"
        ///                     MaximumValue="2000/08/24 00:00:00 UTC"
        ///                     Height="22" Margin="12,122,0,0" 
        ///                     Value="2000/08/04 00:00:00 UTC"
        ///                     HorizontalAlignment="Left" VerticalAlignment="Top" Width="341" 
        ///                     Style="{StaticResource TimeSliderStyle1}"/&gt;
        /// 
        ///   &lt;!-- Provide the instructions on how to use the sample code. --&gt;
        ///   &lt;TextBlock Height="87" HorizontalAlignment="Left" Name="TextBlock1" VerticalAlignment="Top" Width="455" 
        ///            TextWrapping="Wrap" Margin="12,0,0,0" 
        ///            Text="Click the 'Initialize the TimeSlider' button to initialize all the TimeSlider functions 
        ///              (PlayPauseButton, NextButton, PreviousButton). Then push the play button to start the animations. 
        ///              Check on/off the Loop CheckBox to see the different effects of the Loop Property. The code example 
        ///              demonstrates a modified Control Template of the TimeSlider." /&gt;
        /// 
        ///   &lt;!-- Add a button to perform the work of fully initiaizing the TimeSlider. --&gt;
        ///   &lt;Button Content="Initialize the TimeSlider" Height="23" HorizontalAlignment="Left" 
        ///         Margin="12,93,0,0" Name="Button1" VerticalAlignment="Top" Width="341" Click="Button1_Click"/&gt;
        /// 
        /// &lt;/Grid&gt;
        /// </code>
        /// <code title="Example CS1" description="" lang="CS">
        /// private void Button1_Click(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   // are a Collection of IEnumerable&lt;DateTime&gt; objects. When the TimeSlider.Intervals Property is set along with 
        ///   // the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
        ///   // functionality includes buttons for PlayPause, Next, and Previous.
        ///   
        ///   // Obtain the start and end DateTime values from the TimeSlider named MyTimeSlider that was defined in XAML.
        ///   DateTime myMinimumDate = MyTimeSlider.MinimumValue;
        ///   DateTime myMaximumDate = MyTimeSlider.MaximumValue;
        ///   
        ///   // Create a TimeExtent based upon the start and end date/times.
        ///   ESRI.ArcGIS.Client.TimeExtent myTimeExtent = new ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate);
        ///   
        ///   // Create a new TimeSpan (1 day in our case).
        ///   TimeSpan myTimeSpan = new TimeSpan(1, 0, 0, 0);
        ///   
        ///   // Create an empty Collection of IEnumerable&lt;DateTime&gt; objects.
        ///   System.Collections.Generic.IEnumerable&lt;Date&gt; myIEnumerableDates = null;
        ///   
        ///   // Load all of DateTimes into the Collection of IEnumerable&lt;DateTime&gt; objects using the 
        ///   // TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan);
        ///   
        ///   // Set the TimeSlider.Intervals which define the tick marks along the TimeSlider track to the IEnumerable&lt;DateTime&gt; 
        ///   // objects.
        ///   MyTimeSlider.Intervals = myIEnumerableDates;
        /// }
        ///   
        /// private void MyLooper_Checked(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Enabling Looping.
        ///   MyTimeSlider.Loop = true;
        /// }
        ///   
        /// private void MyLooper_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        /// {
        ///   // Disable Looping.
        ///   MyTimeSlider.Loop = false;
        /// }
        /// </code>
        /// <code title="Example VB1" description="" lang="VB.NET">
        /// Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        /// 
        ///   ' This function sets up TimeSlider Intervals that define the tick marks along the TimeSlider track. Intervals 
        ///   ' are a Collection of IEnumerable(Of Date) objects. When the TimeSlider.Intervals Property is set along with 
        ///   ' the other necessary TimeSlider Properties, the full functionality of the TimeSlider is enabled. This full 
        ///   ' functionality includes buttons for PlayPause, Next, and Previous.
        ///   
        ///   ' Obtain the start and end Date/Time values from the TimeSlider named MyTimeSlider that was defined in XAML.
        ///   Dim myMinimumDate As Date = MyTimeSlider.MinimumValue
        ///   Dim myMaximumDate As Date = MyTimeSlider.MaximumValue
        ///   
        ///   ' Create a TimeExtent based upon the start and end date/times.
        ///   Dim myTimeExtent As New ESRI.ArcGIS.Client.TimeExtent(myMinimumDate, myMaximumDate)
        ///   
        ///   ' Create a new TimeSpan (1 day in our case).
        ///   Dim myTimeSpan As New TimeSpan(1, 0, 0, 0)
        ///   
        ///   ' Create an empty Collection of IEnumerable(Of Date) objects.
        ///   Dim myIEnumerableDates As System.Collections.Generic.IEnumerable(Of Date)
        ///   
        ///   ' Load all of Dates into the Collection of IEnumerable(Of Date) objects using the 
        ///   ' TimeSlider.CreateTimeStopsByTimeInterval is a Shared/Static function.
        ///   myIEnumerableDates = ESRI.ArcGIS.Client.Toolkit.TimeSlider.CreateTimeStopsByTimeInterval(myTimeExtent, myTimeSpan)
        ///   
        ///   ' Set the TimeSlider.Intervals which define the tick marks along the TimeSlider track to the IEnumerable(Of Date) 
        ///   ' objects.
        ///   MyTimeSlider.Intervals = myIEnumerableDates
        ///   
        /// End Sub
        /// 
        /// Private Sub MyLooper_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   ' Enabling Looping.
        ///   MyTimeSlider.Loop = True
        /// End Sub
        /// 
        /// Private Sub MyLooper_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ///   ' Disable Looping.
        ///   MyTimeSlider.Loop = False
        /// End Sub
        /// </code>
        /// </example>
		public bool Loop
		{
			get { return (bool)GetValue(LoopProperty); }
			set { SetValue(LoopProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Loop"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LoopProperty =
			DependencyProperty.Register("Loop", typeof(bool), typeof(TimeSlider), new PropertyMetadata(false));

		private void playTimer_Tick(object sender, EventArgs e)
		{
			bool isFinished = !Next();
			if (isFinished)
			{
				if (!Loop)
					IsPlaying = false;
				else
				{
					Rewind();
				}
			}
		}

		private void Rewind()
		{
			if (ValidValue == null) return;
			if (TimeMode == TimeMode.CumulativeFromStart)
				Value = new TimeExtent(ValidValue.Start);
			else
			{
				IEnumerator<DateTime> enumerator = Intervals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (ValidValue.Start == ValidValue.End)
						break;
					if (enumerator.Current == ValidValue.Start)
						break;
				}
				if (ValidValue.Start == ValidValue.End)				
					Value = new TimeExtent(enumerator.Current, enumerator.Current);				
				else
				{
					int i = 0;
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == ValidValue.End)
							break;
						i++;
					}
					enumerator = Intervals.GetEnumerator();
					enumerator.MoveNext();
					DateTime start = enumerator.Current;
					for (int j = 0; j <= i; j++) enumerator.MoveNext();
					DateTime end = enumerator.Current;
					Value = new TimeExtent(start, end);
				}
			}
		}
		#endregion
		
	}

	/// <summary>
	/// TimeMode represents the way dates are measured over time for the 
	/// <see cref="TimeSlider"/> control.
	/// </summary>
	/// <seealso cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider"/>
	/// <seealso cref="ESRI.ArcGIS.Client.Toolkit.TimeSlider.TimeMode"/>
	/// <seealso cref="ESRI.ArcGIS.Client.TimeExtent"/>
	public enum TimeMode
	{
		/// <summary>
		/// CumulativeFromStart mode represents a fixed start date that does not change 
		/// and a end date that can change.
		/// </summary>
		CumulativeFromStart,

		/// <summary>
		/// TimeExtent mode represents a start date that can change and a end 
		/// date that can also change.
		/// </summary>
		TimeExtent,

		/// <summary>
		/// TimeInstant mode represents a start date and end date that are always 
		/// the same date.
		/// </summary>
		TimeInstant
	}	
}
