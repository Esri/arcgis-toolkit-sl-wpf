// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ESRI.ArcGIS.Client.Toolkit")]
[assembly: AssemblyDescription("ArcGIS Silverlight API (Toolkit) for the Microsoft Silverlight Platform")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ESRI")]
[assembly: AssemblyProduct("ArcGIS Silverlight API")]
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

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8534fdac-e5aa-4240-b8be-f790e02d57c1")]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
