using System;
using System.Windows;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	internal class ResourceData
	{
		private static ResourceDictionary dictionary;

		public static ResourceDictionary Dictionary
		{
			get
			{
				if (dictionary == null)
				{
					dictionary = new ResourceDictionary();
                    dictionary.MergedDictionaries.Add(LoadDictionary("/ESRI.ArcGIS.Client.Toolkit.DataSources;component/Kml/KmlPlaceMarkerSymbol.xaml"));
#if WINDOWS_PHONE || !SILVERLIGHT && !NET35
					dictionary.MergedDictionaries.Add(LoadDictionary("/ESRI.ArcGIS.Client.Toolkit.DataSources;component/GpsLayer/GpsSymbolTemplate.xaml"));
#endif
				}
				return dictionary;
			}
		}

		private static ResourceDictionary LoadDictionary(string key)
		{
			return new ResourceDictionary() { Source = new Uri(key, UriKind.Relative) };
		}
	}
}
