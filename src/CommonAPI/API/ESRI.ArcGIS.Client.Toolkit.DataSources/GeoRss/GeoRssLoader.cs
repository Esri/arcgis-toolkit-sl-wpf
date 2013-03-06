// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using ESRI.ArcGIS.Client.Geometry;
using System.Globalization;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	internal class GeoRssLoader
	{
		private const string _W3CGEONAMESPACE_ = "http://www.w3.org/2003/01/geo/wgs84_pos#";
		private const string _GEORSSNAMESPACE_ = "http://www.georss.org/georss";
		private const double _EARTHCIRCUMFERENCE_ = 6378137;

		public class RssLoadedEventArgs : EventArgs
		{
			public object UserState { get; set; }
			public System.Collections.Generic.IEnumerable<Graphic> Graphics { get; set; }
		}
		public class RssLoadFailedEventArgs : EventArgs
		{
			public object UserState { get; set; }
			public Exception ex { get; set; }
		}
		public event EventHandler<RssLoadedEventArgs> LoadCompleted;
		public event EventHandler<RssLoadFailedEventArgs> LoadFailed;

		public void LoadRss(Uri feedUri, ICredentials credentials = null, object userToken = null, System.Security.Cryptography.X509Certificates.X509Certificate clientCertificate = null)
		{
			WebClient wc = Utilities.CreateWebClient();
            if (credentials != null)
                wc.Credentials = credentials;
#if !SILVERLIGHT
			if (clientCertificate != null)
				(wc as CompressResponseWebClient).ClientCertificate = clientCertificate;
#endif
			wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
			wc.OpenReadAsync(feedUri, userToken);
		}

		private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (LoadFailed != null)
					LoadFailed(this, new RssLoadFailedEventArgs()
					{
						ex = new Exception(Properties.Resources.GeoRss_ReadingFeedFailed, e.Error),
						UserState = e.UserState
					});
				return;
			}

			ESRI.ArcGIS.Client.GraphicCollection graphics = new ESRI.ArcGIS.Client.GraphicCollection();

			//Add symbols from each entry read from the feed to the graphics object of the layer
			using (Stream s = e.Result)
			{
				SyndicationFeed feed;
				List<SyndicationItem> feedItems = new List<SyndicationItem>();

				using (XmlReader reader = XmlReader.Create(s))
				{
					feed = SyndicationFeed.Load(reader);
					foreach (SyndicationItem feedItem in feed.Items)
					{
						SyndicationElementExtensionCollection ec = feedItem.ElementExtensions;

						string slong = "";
						string slat = "";
						Geometry.Geometry g = null;
						IDictionary<string, object> attributes = new Dictionary<string,object>();
						foreach (SyndicationElementExtension ee in ec)
						{
							if (ee.OuterNamespace.Equals(_W3CGEONAMESPACE_, StringComparison.OrdinalIgnoreCase))
							{
								//This is not part of the georss-simple spec, but this makes it support a common
								//use-case with geo:lat/geo:long coordinate pairs, as described at
								//http://www.w3.org/2003/01/geo/
								XmlReader xr = ee.GetReader();
								switch (ee.OuterName)
								{
									case ("lat"):
										{
											slat = xr.ReadElementContentAsString();
											break;
										}
									case ("long"):
										{
											slong = xr.ReadElementContentAsString();
											break;
										}
									case ("Point"):
										{
											XmlReader xmlPoint = xr.ReadSubtree();
											while (xmlPoint.Read())
											{
												if (xmlPoint.LocalName == "lat" && xmlPoint.NamespaceURI == _W3CGEONAMESPACE_)
												{
													slat = xmlPoint.ReadElementContentAsString();
												}
												else if (xmlPoint.LocalName == "long" && xmlPoint.NamespaceURI == _W3CGEONAMESPACE_)
												{
													slong = xmlPoint.ReadElementContentAsString();
												}
											}
											break;
										}
								}
							}
							else if (ee.OuterNamespace.Equals(_GEORSSNAMESPACE_, StringComparison.OrdinalIgnoreCase))
							{
								XmlReader xr = ee.GetReader();
								switch (ee.OuterName)
								{
									case ("point"):
										{
											string sp = xr.ReadElementContentAsString();
											string[] sxsy = sp.Split(new char[] { ' ' });
											slong = sxsy[1];
											slat = sxsy[0];
											break;
										}
									case ("line"):
										{
											string sp = xr.ReadElementContentAsString();
											PointCollection pnts = StringToPoints(sp);
											if (pnts != null)
											{
												Polyline line = new Polyline() { SpatialReference = new SpatialReference(4326) };
												line.Paths.Add(pnts);
												g = line;
											}
											break;
										}
									case ("polygon"):
										{
											string sp = xr.ReadElementContentAsString();
											PointCollection pnts = StringToPoints(sp);
											if (pnts != null)
											{
												Polygon line = new Polygon() { SpatialReference = new SpatialReference(4326) };
												line.Rings.Add(pnts);
												g = line;
											}
											break;
										}
									case ("box"):
										{
											string sp = xr.ReadElementContentAsString();
											PointCollection pnts = StringToPoints(sp);
											if (pnts != null && pnts.Count == 2)
											{
												g = new Envelope(pnts[0], pnts[1]) { SpatialReference = new SpatialReference(4326) };
											}
											break;
										}
									case ("circle"):
										{
											string sp = xr.ReadElementContentAsString();
											string[] sxsy = sp.Split(new char[] { ' ' });
											if (sxsy.Length == 3)
											{
												double x = double.NaN, y = double.NaN, r = double.NaN;
												string stX = sxsy[1];
												string stY = sxsy[0];
												string stR = sxsy[2];
												if (double.TryParse(stY, NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
													double.TryParse(stX, NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
													double.TryParse(stR, NumberStyles.Any, CultureInfo.InvariantCulture, out r))
												{
													g = GetRadiusAsPolygonGeodesic(new MapPoint(x, y), r, 360);
												}
											}
											break;
										}

									case ("where"): //GeoRSS-GML
										{
											//GML geometry parsing goes here. However this is not
											//part of GeoRSS-simple and not supported for this datasource
											//We'll just ignore these entries
											break;
										}
									#region Attributes
									case ("elev"):
										{
											string sp = xr.ReadElementContentAsString();
											double elevation = 0;
											if (double.TryParse(sp, NumberStyles.Any, CultureInfo.InvariantCulture, out elevation))
												attributes.Add("elev", elevation);
											break;
										}
									case ("floor"):
										{
											string sp = xr.ReadElementContentAsString();
											int floor = 0;
											if (int.TryParse(sp, NumberStyles.Any, CultureInfo.InvariantCulture, out floor))
												attributes.Add("floor", floor);
											break;
										}
									case ("radius"):
										{
											string sp = xr.ReadElementContentAsString();
											double radius = 0;
											if (double.TryParse(sp, NumberStyles.Any, CultureInfo.InvariantCulture, out radius))
												attributes.Add("radius", radius);
											break;
										}
									//case ("featuretypetag"):
									//case ("relationshiptag"):
									//case ("featurename"):
									default:
										{
											string sp = xr.ReadElementContentAsString();
											attributes.Add(ee.OuterName, sp);
											break;
										}
									#endregion
								}
							}
						}

						if (!string.IsNullOrEmpty(slong) && !string.IsNullOrEmpty(slat))
						{
							double x = double.NaN;
							double y = double.NaN;
							if (double.TryParse(slat, NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
								double.TryParse(slong, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
								g = new MapPoint(x, y, new SpatialReference(4326));
						}
						if (g != null)
						{
							Graphic graphic = new Graphic() { Geometry = g };
							
							if(feedItem.Title != null)
								graphic.Attributes.Add("Title", feedItem.Title.Text);
							if (feedItem.Summary != null)
								graphic.Attributes.Add("Summary", feedItem.Summary.Text);
							if (feedItem.PublishDate != null)
							{
								graphic.Attributes.Add("PublishDate", feedItem.PublishDate);
								graphic.TimeExtent = new TimeExtent(feedItem.PublishDate.DateTime);
							}
							if (feedItem.Links.Count > 0)
								graphic.Attributes.Add("Link", feedItem.Links[0].Uri);
							graphic.Attributes.Add("FeedItem", feedItem);
							graphic.Attributes.Add("Id", feedItem.Id);
							foreach(var val in attributes)
								if(!graphic.Attributes.ContainsKey(val.Key))
								graphic.Attributes.Add(val.Key, val.Value);
							
							graphics.Add(graphic);
						}
					}
				}
			}

			//Invoking the initialize method of the base class to finish the initialization of the graphics layer:
			if (LoadCompleted != null)
				LoadCompleted(this, new RssLoadedEventArgs()
				{
					Graphics = graphics,
					UserState = e.UserState
				}
				);
		}

		private static PointCollection StringToPoints(string str)
		{
			string[] sxsy = str.Split(new char[] { ' ' });
			PointCollection pnts = new PointCollection();
			for (int i = 0; i < sxsy.Length - 1; i+=2)
			{
				string slat = sxsy[i];
				string slong = sxsy[i + 1];
				double x = double.NaN;
				double y = double.NaN;
				if (double.TryParse(slat, NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
					double.TryParse(slong, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
					pnts.Add(new MapPoint(x, y));
			}
			if (pnts.Count > 0)
				return pnts;
			else return null;
		}

		#region Generate geodetic circle based on radius and long/lat point
		public static Polygon GetRadiusAsPolygonGeodesic(MapPoint center, double distance, int pointCount)
		{
			Polyline line = GetRadiusGeodesicAsPolyline(center, distance, pointCount);
			Polygon poly = new Polygon() { SpatialReference = new SpatialReference(4326) };

			if (line.Paths.Count > 1)
			{
				PointCollection ring = line.Paths[0];
				MapPoint last = ring[ring.Count - 1];
				for (int i = 1; i < line.Paths.Count; i++)
				{
					PointCollection pnts = line.Paths[i];
					ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y)));
					last = pnts[0];
					ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y)));
					foreach (MapPoint p in pnts)
						ring.Add(p);
					last = pnts[pnts.Count - 1];
				}
				poly.Rings.Add(ring);
			}
			else
			{
				poly.Rings.Add(line.Paths[0]);
			}
			if (distance > _EARTHCIRCUMFERENCE_ * Math.PI / 2 && line.Paths.Count != 2)
			{
				PointCollection pnts = new PointCollection();
				pnts.Add(new MapPoint(-180, -90));
				pnts.Add(new MapPoint(180, -90));
				pnts.Add(new MapPoint(180, 90));
				pnts.Add(new MapPoint(-180, 90));
				pnts.Add(new MapPoint(-180, -90));
				poly.Rings.Add(pnts); //Exterior
			}
			return poly;
		}


		private static Polyline GetRadiusGeodesicAsPolyline(MapPoint center, double distance, int pointCount)
		{
			Polyline line = new Polyline() { SpatialReference = new SpatialReference(4326) };
			PointCollection pnts = new PointCollection();
			line.Paths.Add(pnts);
			for (int i = 0; i < pointCount; i++)
			{
				//double angle = i / 180.0 * Math.PI;
				MapPoint p = GetPointFromHeadingGeodesic(center, distance, i);
				if (pnts.Count > 0)
				{
					MapPoint lastPoint = pnts[pnts.Count - 1];
					int sign = Math.Sign(p.X);
					if (Math.Abs(p.X - lastPoint.X) > 180)
					{   //We crossed the date line
						double lat = LatitudeAtLongitude(lastPoint, p, sign * -180);
						pnts.Add(new MapPoint(sign * -180, lat));
						pnts = new PointCollection();
						line.Paths.Add(pnts);
						pnts.Add(new MapPoint(sign * 180, lat));
					}
				}
				pnts.Add(p);
			}
			pnts.Add(line.Paths[0][0]);
			return line;
		}

		private static double LatitudeAtLongitude(MapPoint p1, MapPoint p2, double lon)
		{
			double lon1 = p1.X / 180 * Math.PI;
			double lon2 = p2.X / 180 * Math.PI;
			double lat1 = p1.Y / 180 * Math.PI;
			double lat2 = p2.Y / 180 * Math.PI;
			lon = lon / 180 * Math.PI;
			return Math.Atan((Math.Sin(lat1) * Math.Cos(lat2) * Math.Sin(lon - lon2)
	 - Math.Sin(lat2) * Math.Cos(lat1) * Math.Sin(lon - lon1)) / (Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(lon1 - lon2)))
			/ Math.PI * 180;
		}

		private static MapPoint GetPointFromHeadingGeodesic(MapPoint start, double distance, double heading)
		{
			double brng = heading / 180 * Math.PI;
			double lon1 = start.X / 180 * Math.PI;
			double lat1 = start.Y / 180 * Math.PI;
			double dR = distance / 6378137; //Angular distance in radians
			double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
			double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
			double lon = lon2 / Math.PI * 180;
			double lat = lat2 / Math.PI * 180;
			while (lon < -180) lon += 360;
			while (lat < -90) lat += 180;
			while (lon > 180) lon -= 360;
			while (lat > 90) lat -= 180;
			return new MapPoint(lon, lat);
		}
		#endregion
	}
}