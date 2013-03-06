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

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	internal class Utilities
	{
		internal static Uri PrefixProxy(string proxyUrl, string url)
		{
			if (string.IsNullOrEmpty(proxyUrl))
				return new Uri(url, UriKind.RelativeOrAbsolute);
			string _proxyUrl = proxyUrl;
			if (!_proxyUrl.Contains("?"))
			{
				if (!_proxyUrl.EndsWith("?"))
					_proxyUrl = proxyUrl + "?";
			}
			else
			{
				if (!_proxyUrl.EndsWith("&"))
					_proxyUrl = proxyUrl + "&";
			}
#if SILVERLIGHT && !WINDOWS_PHONE
			if (proxyUrl.StartsWith("~") || proxyUrl.StartsWith("../")) //relative to xap root
			{
				string uri = Application.Current.Host.Source.AbsoluteUri;
				int count = _proxyUrl.Split(new string[] { "../" }, StringSplitOptions.None).Length;
				for (int i = 0; i < count; i++)
				{
					uri = uri.Substring(0, uri.LastIndexOf("/"));
				}
				if (!uri.EndsWith("/"))
					uri += "/";
				_proxyUrl = uri + _proxyUrl.Replace("~", "").Replace("../", "");
			}
			else if (proxyUrl.StartsWith("/")) //relative to domain root
			{
				_proxyUrl = string.Format("{0}://{1}:{2}{3}",
					Application.Current.Host.Source.Scheme,
					Application.Current.Host.Source.Host,
					Application.Current.Host.Source.Port, _proxyUrl);
			}
#endif
			UriBuilder b = new UriBuilder(_proxyUrl);
			b.Query = url;
			return b.Uri;
		}

		internal static WebClient CreateWebClient()
		{
#if !SILVERLIGHT
			return new CompressResponseWebClient() { Encoding = System.Text.Encoding.UTF8 };
#else 
			return new WebClient();
#endif
		}
	}

#if !SILVERLIGHT
	internal class CompressResponseWebClient : WebClient
	{
		public System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate { get; set; }
		protected override System.Net.WebRequest GetWebRequest(Uri address)
		{
			System.Net.WebRequest wr = base.GetWebRequest(address);
			//Support GZIP and Deflate compressed responses from server
			//Silverlight automatically does this for us
			((HttpWebRequest)wr).AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			if (ClientCertificate != null)
				((HttpWebRequest)wr).ClientCertificates.Add(ClientCertificate);
			return wr;
		}
	}
#endif
}
