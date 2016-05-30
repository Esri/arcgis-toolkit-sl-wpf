# arcgis-toolkit-sl-wpf

This project contains the source code and libraries for the ArcGIS Runtime SDK Toolkit for Silverlight/WPF. The library is an extension of the ArcGIS API for Silverlight™ and the ArcGIS Runtime SDK for WPF™. Included are a number of controls, classes, and data sources you can use to enhance your applications. 

NOTE: This library is only an extension of the ArcGIS API for Silverlight™ or the ArcGIS Runtime SDK for WPF™.   In order to build a complete mapping application, you will also need to install the respective products on which they are built.  See the instructions below on how to get started.

[View it live in Silverlight](http://resources.arcgis.com/en/help/silverlight-api/samples/start.htm#Attribution)

[![Example toolkit contents](https://raw.github.com/Esri/arcgis-toolkit-sl-wpf/master/arcgis-toolkit-sl-wpf.png "Example toolkit contents")](http://resources.arcgis.com/en/help/silverlight-api/samples/start.htm#Attribution)

## Features
- Controls and classes
 * AttachmentEditor
 * Attribution
 * Bookmark
 * EditorWidget
 * FeatureDataForm
 * FeatureDataGrid
 * InfoWindow
 * Legend
 * Magnifier
 * MagnifyingGlass
 * MapProgressBar
 * MapTip
 * Navigation
 * OAuthAuthorize
 * OverviewMap
 * SignInDialog 
 * TemplatePicker
 * TimeSlider
- Data sources
 * CsvLayer
 * GeoRssLayer
 * GpsLayer
 * HeatMapLayer
 * KmlLayer
 * OpenStreetMapLayer
 * WebTiledLayer
 * WmsLayer
 * WmtsLayer

## Instructions

1. Fork and then clone the repo or download the .zip file. 
2. Download and install the ArcGIS API for Silverlight or ArcGIS Runtime SDK for WPF.  Login requires an Esri Global account, which can be created for free.   
 * [ArcGIS API for Silverlight](http://www.esri.com/apps/products/download/index.cfm?fuseaction=download.main&downloadid=876) 
 * [ArcGIS Runtime SDK for WPF](http://www.esri.com/apps/products/download/index.cfm?fuseaction=download.main&downloadid=1079)
3. In Visual Studio, open the solution for Silverlight (ESRI_Silverlight_Toolkit.sln) or WPF (ESRI_WPF_Toolkit.sln).  You may need to update the reference to the location of the core ArcGIS Silverlight or WPF assembly ESRI.ArcGIS.Client.dll.     
4. Build the ESRI.ArcGIS.Client.Toolkit and ESRI.ArcGIS.Client.Toolkit.DataSources projects and reference the dlls in your application.  

## Requirements

* [ArcGIS API for Silverlight system requirements](http://resources.arcgis.com/en/help/silverlight-api/concepts/#/System_requirements/01660000000t000000/) 
* [ArcGIS Runtime SDK for WPF system requirements](http://resources.arcgis.com/en/help/runtime-wpf/concepts/index.html#/System_requirements/0170000000p3000000/)

## Resources

* [ArcGIS API for Silverlight resource center](http://resources.arcgis.com/en/communities/silverlight-api/) 
* [ArcGIS Runtime SDK for WPF resource center](http://resources.arcgis.com/en/communities/runtime-wpf/)

## Issues

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Anyone and everyone is welcome to [contribute](CONTRIBUTING.md). 

## Licensing
Copyright 2013 Esri

Licensed under Ms-PL (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   https://opensource.org/licenses/ms-pl

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's [license.txt](https://raw.github.com/Esri/arcgis-toolkit-sl-wpf/master/license.txt) file.

[](Esri Tags: ArcGIS toolkit wpf silverlight c-sharp)
[](Esri Language: Silverlight)
