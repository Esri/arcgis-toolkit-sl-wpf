// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client.Geometry;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	/// <summary>
	/// Heat Map layer
	/// </summary>
	public class HeatMapLayer : DynamicLayer, ILegendSupport
#if !SILVERLIGHT
		, ISupportsDynamicImageByteRequests
#endif
	{
		private BackgroundWorker renderThread; //background thread used for generating the heat map
		private ESRI.ArcGIS.Client.Geometry.PointCollection heatMapPoints;
		private Envelope fullExtent; //cached value of the calculated full extent
		private Envelope enqueueExtent;
		private int enqueueWidth;
		private int enqueueHeight;
		private DynamicLayer.OnImageComplete enqueueOnComplete;
		
		private struct HeatPoint
		{
			public int X;
			public int Y;
		}

		private struct ThreadSafeGradientStop
		{
			public double Offset;
			public Color Color;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HeatMapLayer"/> class.
		/// </summary>
		public HeatMapLayer()
		{
			GradientStopCollection stops = new GradientStopCollection();
			stops.Add(new GradientStop() { Color = Colors.Transparent, Offset = 0 });
			stops.Add(new GradientStop() { Color = Colors.Blue, Offset = .5 });
			stops.Add(new GradientStop() { Color = Colors.Red, Offset = .75 });
			stops.Add(new GradientStop() { Color = Colors.Yellow, Offset = .8 });
			stops.Add(new GradientStop() { Color = Colors.White, Offset = 1 });
			Gradient = stops;
			HeatMapPoints = new ESRI.ArcGIS.Client.Geometry.PointCollection();
			//Create a separate thread for rendering the heatmap layer.
			renderThread = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
			renderThread.ProgressChanged += new ProgressChangedEventHandler(renderThread_ProgressChanged);
			renderThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(renderThread_RunWorkerCompleted);
			renderThread.DoWork += new DoWorkEventHandler(renderThread_DoWork);
		}

		/// <summary>
		/// The full extent of the layer.
		/// </summary>
		public override Envelope FullExtent
		{
			get
			{
				if (fullExtent == null && heatMapPoints != null && heatMapPoints.Count > 0)
				{
					fullExtent = new Envelope();
					foreach (MapPoint p in heatMapPoints)
					{
						fullExtent = fullExtent.Union(p.Extent);
					}
				}
				return fullExtent;
			}
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Identifies the <see cref="Intensity"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IntensityProperty =
						DependencyProperty.Register("Intensity", typeof(double), typeof(HeatMapLayer),
						new PropertyMetadata(10.0, OnIntensityPropertyChanged));

		/// <summary>
		/// Gets or sets the interval.
		/// </summary>
		public double Intensity
		{
			get { return (double)GetValue(IntensityProperty); }
			set { SetValue(IntensityProperty, value); }
		}

		/// <summary>
		/// IntervalProperty property changed handler. 
		/// </summary>
		/// <param name="d">HeatMapLayer that changed its Interval.</param>
		/// <param name="e">DependencyPropertyChangedEventArgs.</param> 
		private static void OnIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((double)e.NewValue < 1)
				throw new ArgumentOutOfRangeException("Intensity", Properties.Resources.HeatMapLayer_IntensityLessThanOne);
			HeatMapLayer dp = d as HeatMapLayer;
			if (dp.IsInitialized)
			{
				if (dp.timer == null)
				{
					dp.timer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(50) };
					dp.timer.Tick += (s, e2) =>
					{
						dp.timer.Stop();
						dp.OnLayerChanged();
					};
				}
				dp.timer.Stop();
				dp.timer.Start();
			}
		}
		System.Windows.Threading.DispatcherTimer timer;
		/// <summary>
		/// Gets or sets the heat map points.
		/// </summary>
		/// <value>The heat map points.</value>
		public ESRI.ArcGIS.Client.Geometry.PointCollection HeatMapPoints
		{
			get { return heatMapPoints; }
			set
			{
				if (heatMapPoints != null)
					heatMapPoints.CollectionChanged -= heatMapPoints_CollectionChanged;
				heatMapPoints = value;
				if (heatMapPoints != null)
					heatMapPoints.CollectionChanged += heatMapPoints_CollectionChanged;
				fullExtent = null;
				OnLayerChanged();
			}
		}

		/// <summary>
		/// Handles the CollectionChanged event of the heatMapPoints control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void heatMapPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			fullExtent = null;
			OnLayerChanged();
		}

		/// <summary>
		/// Identifies the <see cref="Gradient"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GradientProperty =
						DependencyProperty.Register("Gradient", typeof(GradientStopCollection), typeof(HeatMapLayer),
						new PropertyMetadata(null, OnGradientPropertyChanged));
		/// <summary>
		/// Gets or sets the heat map gradient.
		/// </summary>
		public GradientStopCollection Gradient
		{
			get { return (GradientStopCollection)GetValue(GradientProperty); }
			set { SetValue(GradientProperty, value); }
		}
		/// <summary>
		/// GradientProperty property changed handler. 
		/// </summary>
		/// <param name="d">HeatMapLayer that changed its Gradient.</param>
		/// <param name="e">DependencyPropertyChangedEventArgs.</param> 
		private static void OnGradientPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			HeatMapLayer dp = d as HeatMapLayer;
			dp.OnLayerChanged();
			dp.OnLegendChanged();
		}

		/// <summary>
		/// Gets the source image to display in the dynamic layer. Override this to generate
		/// or modify images.
		/// </summary>
		/// <param name="extent">The extent of the image being requested.</param>
		/// <param name="width">The width of the image being requested.</param>
		/// <param name="height">The height of the image being requested.</param>
		/// <param name="onComplete">The method to call when the image is ready.</param>
		/// <seealso cref="ESRI.ArcGIS.Client.DynamicLayer.OnProgress"/>
		protected override void GetSource(Envelope extent, int width, int height, DynamicLayer.OnImageComplete onComplete)
		{
			if (!IsInitialized || HeatMapPoints == null || HeatMapPoints.Count == 0)
			{
				onComplete(null, -1, -1, null);
				return;
			}

			if (renderThread != null && renderThread.IsBusy)
			{
				renderThread.CancelAsync(); //render already running. Cancel current process, and queue up new
				enqueueExtent = extent;
				enqueueWidth = width;
				enqueueHeight = height;
				enqueueOnComplete = onComplete;
				return;
			}

			//Accessing a GradientStop collection from a non-UI thread is not allowed,
			//so we used a private class gradient collection
			List<ThreadSafeGradientStop> stops = new List<ThreadSafeGradientStop>(Gradient.Count);
			foreach (GradientStop stop in Gradient)
			{
				stops.Add(new ThreadSafeGradientStop() { Color = stop.Color, Offset = stop.Offset });
			}
			//Gradients must be sorted by offset
			stops.Sort((ThreadSafeGradientStop g1, ThreadSafeGradientStop g2) => { return g1.Offset.CompareTo(g2.Offset); });

			List<HeatPoint> points = new List<HeatPoint>();
			double res = extent.Width / width;
			//adjust extent to include points slightly outside the view so pan won't affect the outcome
			Envelope extent2 = new Envelope(extent.XMin - Intensity * res, extent.YMin - Intensity * res,
				extent.XMax + Intensity * res, extent.YMax + Intensity * res);

			//get points within the extent and transform them to pixel space
			foreach (MapPoint p in HeatMapPoints) 
			{
				if (Map != null && Map.WrapAroundIsActive)
				{
					// Note : this should work even if WrapAround is not active but it's probably less performant
					if (p.Y >= extent2.YMin && p.Y <= extent2.YMax)
					{
						Point screenPoint = Map.MapToScreen(p, true);
						if (!double.IsNaN(width) && Map.FlowDirection == FlowDirection.RightToLeft)
							screenPoint.X = width - screenPoint.X; 

						if (screenPoint.X >= -Intensity && screenPoint.X <= width + Intensity)
						{
							points.Add(new HeatPoint()
							{
								X = (int)Math.Round(screenPoint.X),
								Y = (int)Math.Round(screenPoint.Y)
							});
						}
					}
				}
				else
				{
					if (p.X >= extent2.XMin && p.Y >= extent2.YMin &&
						p.X <= extent2.XMax && p.Y <= extent2.YMax)
					{
						points.Add(new HeatPoint()
						{
							X = (int)Math.Round((p.X - extent.XMin) / res),
							Y = (int)Math.Round((extent.YMax - p.Y) / res)
						});
					}
				}
			}
			//Start the render thread
			renderThread.RunWorkerAsync(
				new object[] { extent, width, height, (int)Math.Round(this.Intensity), stops, points, onComplete });
		}

		/// <summary>
		/// Stops loading of any pending images
		/// </summary>
		protected override void Cancel()
		{
			enqueueExtent = null;
			enqueueOnComplete = null;
			if (renderThread != null && renderThread.IsBusy)
			{
				renderThread.CancelAsync();
			}
			base.Cancel();
		}

		/// <summary>
		/// Handles the DoWork event of the renderThread control. This is where we
		/// render the heatmap outside the UI thread.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance 
		/// containing the event data.</param>
		private void renderThread_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = (BackgroundWorker)sender;
			object[] args = (object[])e.Argument;
			Envelope extent = (Envelope)args[0];
			int width = (int)args[1];
			int height = (int)args[2];
			int size = (int)args[3];
			List<ThreadSafeGradientStop> stops = (List<ThreadSafeGradientStop>)args[4];
			List<HeatPoint> points = (List<HeatPoint>)args[5];
			OnImageComplete onComplete = (OnImageComplete)args[6];

			size = size * 2 + 1;
			ushort[] matrix = CreateDistanceMatrix(size);
			int[] output = new int[width * height];
			foreach (HeatPoint p in points)
			{
				AddPoint(matrix, size, p.X, p.Y, output, width);
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					e.Result = null;
					return;
				}
			}
			matrix = null;
			int max = 0;
			foreach (int val in output) //find max - used for scaling the intensity
				if (max < val) max = val;

			//If we only have single points in the view, don't show them with too much intensity.
			if (max < 2) max = 2; 
#if SILVERLIGHT
			PngEncoder ei = new PngEncoder(width, height);
#else
			int[] pixels = new int[height*width];
#endif
			for (int idx = 0; idx < height; idx++)      // Height (y)
			{
#if SILVERLIGHT
				int rowstart = ei.GetRowStart(idx);
#endif
				for (int jdx = 0; jdx < width; jdx++)     // Width (x)
				{
					Color c = InterpolateColor(output[idx * width + jdx] / (float)max, stops);
#if SILVERLIGHT
					ei.SetPixelAtRowStart(jdx, rowstart, c.R, c.G, c.B, c.A);
#else
					int color = (c.A << 24) + (c.R << 16) + (c.G << 8) + c.B;
					pixels[idx * width + jdx] = color;
#endif			
				}
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					e.Result = null;
					output = null;
#if SILVERLIGHT
					ei = null;
#else
					pixels = null;
#endif
					return;
				}
				//Raise the progress event for each line rendered
				worker.ReportProgress((idx + 1) * 100 / height);
			}
			stops.Clear();
			output = null;

			// Get stream and set image source
#if SILVERLIGHT
			e.Result = new object[] { ei, width, height, extent, onComplete };
#else
			e.Result = new object[] { pixels, width, height, extent, onComplete };
#endif
		}

		/// <summary>
		/// Handles the RunWorkerCompleted event of the renderThread control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
		private void renderThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (enqueueExtent != null) //cancelled because new image was requested. Create new image
				{
					GetSource(enqueueExtent, enqueueWidth, enqueueHeight, enqueueOnComplete);
					enqueueExtent = null;
					enqueueOnComplete = null;
				}
				return;
			}
			enqueueExtent = null;
			enqueueOnComplete = null;
			if (e.Result == null)
				return;
			object[] result = (object[])e.Result;

			int width = (int)result[1];
			int height = (int)result[2];
			Envelope extent = (Envelope)result[3];
			OnImageComplete onComplete = (OnImageComplete)result[4];
#if SILVERLIGHT
			BitmapImage image = new BitmapImage();
			PngEncoder ei = (PngEncoder)result[0];
			image.SetSource(ei.GetImageStream());
#else
			List<Color> colors = new List<Color>();
			colors.Add(Colors.Violet);
			colors.Add(Colors.Yellow);
			BitmapPalette palette = new BitmapPalette(colors);
			System.Windows.Media.PixelFormat pf =
				System.Windows.Media.PixelFormats.Bgra32;
			int stride = width * (pf.BitsPerPixel / 8);
			BitmapSource image = BitmapSource.Create(width, height, 96, 96, pf, palette, (int[])result[0], stride);
#endif
			onComplete(image, width, height, extent);
		}

		/// <summary>
		/// Handles the ProgressChanged event of the renderThread control and fires the layer progress event.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
		private void renderThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//Raise the layer progress event
			OnProgress(e.ProgressPercentage);
		}

		/// <summary>
		/// Lienarly interpolates a color from a list of colors.
		/// </summary>
		/// <param name="value">The value relative to the gradient stop offsets.</param>
		/// <param name="stops">The color stops sorted by the offset.</param>
		/// <returns></returns>
		private static Color InterpolateColor(float value, List<ThreadSafeGradientStop> stops)
		{
			if (value < 1 / 255f)
				return Colors.Transparent;
			if (stops == null || stops.Count == 0)
				return Colors.Black;
			if (stops.Count == 1)
				return stops[0].Color;

			if (stops[0].Offset >= value) //clip to bottom
				return stops[0].Color;
			else if (stops[stops.Count - 1].Offset <= value) //clip to top
				return stops[stops.Count - 1].Color;
			int i = 0;
			for (i = 1; i < stops.Count; i++)
			{
				if (stops[i].Offset > value)
				{
					Color start = stops[i - 1].Color;
					Color end = stops[i].Color;

					double frac = (value - stops[i - 1].Offset) / (stops[i].Offset - stops[i - 1].Offset);
					byte R = (byte)Math.Round((start.R * (1 - frac) + end.R * frac));
					byte G = (byte)Math.Round((start.G * (1 - frac) + end.G * frac));
					byte B = (byte)Math.Round((start.B * (1 - frac) + end.B * frac));
					byte A = (byte)Math.Round((start.A * (1 - frac) + end.A * frac));
					return Color.FromArgb(A, R, G, B);
				}
			}
			return stops[stops.Count - 1].Color; //should never happen
		}

		/// <summary>
		/// Adds a heat map point to the intensity matrix.
		/// </summary>
		/// <param name="distanceMatrix">The distance matrix.</param>
		/// <param name="size">The size of the distance matrix.</param>
		/// <param name="x">x.</param>
		/// <param name="y">y</param>
		/// <param name="intensityMap">The intensity map.</param>
		/// <param name="width">The width of the intensity map..</param>
		private static void AddPoint(ushort[] distanceMatrix, int size, int x, int y, int[] intensityMap, int width)
		{
			for (int i = 0; i < size * 2 - 1; i++)
			{
				int start = (y - size + 1 + i) * width + x - size;
				for (int j = 0; j < size * 2 - 1; j++)
				{
					if (j + x - size < 0 || j + x - size >= width) continue;
					int idx = start + j;
					if (idx < 0 || idx >= intensityMap.Length)
						continue;
					intensityMap[idx] += distanceMatrix[i * (size * 2 - 1) + j];
				}
			}
		}

		/// <summary>
		/// Creates the distance matrix.
		/// </summary>
		/// <param name="size">The size of the matrix (must be and odd number).</param>
		/// <returns></returns>
		private static ushort[] CreateDistanceMatrix(int size)
		{
			int width = size * 2 - 1;
			ushort[] matrix = new ushort[(int)Math.Pow(width, 2)];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < width; j++)
				{
					matrix[i * width + j] = (ushort)Math.Max((size - (Math.Sqrt(Math.Pow(i - size + 1, 2) + Math.Pow(j - size + 1, 2)))), 0);
				}
			}
			return matrix;
		}

		#region ILegendSupport Members

		/// <summary>
		/// Queries for the legend infos of a layer.
		/// </summary>
		/// <remarks>
		/// The returned result is encapsulated in a <see cref="LayerLegendInfo" /> object containing one legend item showing the heat map gradient.
		/// </remarks>
		/// <param name="callback">The method to call on completion.</param>
		/// <param name="errorCallback">The method to call in the event of an error.</param>
		public void QueryLegendInfos(Action<LayerLegendInfo> callback, Action<Exception> errorCallback)
		{
			// Create one legend item with a radial brush using the heat map gradient (reversed)

			// Create the UI element
			RadialGradientBrush brush = new RadialGradientBrush()
			{
				Center = new Point(0.5, 0.5),
				RadiusX = 0.5,
				RadiusY = 0.5,
				GradientOrigin = new Point(0.5, 0.5),
				GradientStops = new GradientStopCollection()
			};

			if (Gradient != null)
			{
				foreach (GradientStop stop in Gradient)
					brush.GradientStops.Add(new GradientStop() { Color = stop.Color, Offset = 1 - stop.Offset });
			}
			Rectangle rect = new Rectangle() { Height = 20, Width = 20, Fill = brush };

			// Create the imagesource
			ImageSource imageSource;
#if SILVERLIGHT
			imageSource = new WriteableBitmap(rect, null);
#else
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(20, 20, 96, 96, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(rect);
			imageSource = renderTargetBitmap;
#endif
			// Create a layerItemInfo array (so enumerable) with one item
			LegendItemInfo legendItemInfo = new LegendItemInfo() {
				Label = ID,
				ImageSource = imageSource
			};
			LegendItemInfo[] legendItemInfos = new LegendItemInfo[] { legendItemInfo };

			// Create the returned layerLegendInfo
			LayerLegendInfo layerLegendInfo = new LayerLegendInfo() { LegendItemInfos = legendItemInfos };

			if (callback != null)
				callback(layerLegendInfo);
		}

		/// <summary>
		/// Occurs when the legend of the layer changed (i.e. when the the <see cref="Gradient"/> changed).
		/// </summary>
		public event EventHandler<EventArgs> LegendChanged;

		private void OnLegendChanged()
		{
			EventHandler<EventArgs> legendChanged = LegendChanged;
			if (legendChanged != null)
				legendChanged(this, EventArgs.Empty);
		}
		#endregion

#if !SILVERLIGHT

		void ISupportsDynamicImageByteRequests.GetImageData(Envelope extent, int width, int height, OnImageDataReceived onImageDataReceived)
		{
			OnImageComplete onImageComplete =
				(image, imageWidth, imageHeight, imageExtent) =>
					{
						BitmapSource bitmapSource = image as BitmapSource;

						MemoryStream stream = new MemoryStream();
						if (bitmapSource != null)
						{
							PngBitmapEncoder encoder = new PngBitmapEncoder();
							encoder.Interlace = PngInterlaceOption.Off;
							encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
							encoder.Save(stream);
							stream.Seek(0, SeekOrigin.Begin);
						}

						onImageDataReceived(stream, imageWidth, imageHeight, imageExtent);
					};

			GetSource(extent, width, height, onImageComplete);
		}

#endif

	}
}