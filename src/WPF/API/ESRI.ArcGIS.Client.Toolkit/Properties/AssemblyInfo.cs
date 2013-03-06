// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ESRI.ArcGIS.Client.Toolkit")]
[assembly: AssemblyDescription("ArcGIS Client API (Toolkit) for Microsoft WPF")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ESRI")]
[assembly: AssemblyProduct("ArcGIS WPF API")]
[assembly: AssemblyCopyright("Copyright © ESRI 2009")]
[assembly: AssemblyTrademark("ESRI")]
[assembly: AssemblyCulture("")]

//Default prefixes
//toolkit:
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.esri.com/arcgis/client/2009", "ESRI.ArcGIS.Client.Toolkit")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.esri.com/arcgis/client/2009", "esri")]
//primitives:
[assembly: System.Windows.Markup.XmlnsDefinition("clr-namespace:ESRI.ArcGIS.Client.Toolkit.Primitives;assembly=ESRI.ArcGIS.Client.Toolkit", "ESRI.ArcGIS.Client.Toolkit.Primitives")]
[assembly: System.Windows.Markup.XmlnsPrefix("clr-namespace:ESRI.ArcGIS.Client.Toolkit.Primitives;assembly=ESRI.ArcGIS.Client.Toolkit", "esriToolkitPrimitives")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
