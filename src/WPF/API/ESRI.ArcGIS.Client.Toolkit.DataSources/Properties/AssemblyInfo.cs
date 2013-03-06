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
[assembly: AssemblyTitle("ESRI.ArcGIS.Client.Toolkit.DataSources")]
[assembly: AssemblyDescription("ArcGIS WPF API (Toolkit.DataSources) for the Microsoft WPF Platform")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ESRI")]
[assembly: AssemblyProduct("ArcGIS WPF API")]
[assembly: AssemblyCopyright("Copyright © ESRI 2010")]
[assembly: AssemblyTrademark("ESRI")]
[assembly: AssemblyCulture("")]

//Default prefixes
//toolkit:
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.esri.com/arcgis/client/2009", "ESRI.ArcGIS.Client.Toolkit.DataSources")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.esri.com/arcgis/client/2009", "esri")]

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
[assembly: NeutralResourcesLanguageAttribute("en-US")]
