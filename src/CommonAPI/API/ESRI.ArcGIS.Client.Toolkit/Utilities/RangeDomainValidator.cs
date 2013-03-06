using System;

namespace ESRI.ArcGIS.Client.Toolkit.Utilities
{	
	/// <summary>
	/// *FOR INTERNAL USE ONLY* The RangeDomainValidator class.
	/// </summary>
	/// <remarks>Used by property setters of the fields having range domain information.</remarks>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public static class RangeDomainValidator
	{
		private static string errorMessage = Properties.Resources.Validation_InvalidRangeDomain;   // Error message shown after validation fails 

		/// <summary>
		/// Determines whether the nullable byte value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(byte? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the byte value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(byte value, string min, string max)
		{
			byte lowerBound;
			byte upperBound;
			if (byte.TryParse(min, out lowerBound) &&
				byte.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable short integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(short? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the short integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(short value, string min, string max)
		{
			short lowerBound;
			short upperBound;
			if (short.TryParse(min, out lowerBound) &&
				short.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(int? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(int value, string min, string max)
		{
			int lowerBound;
			int upperBound;
			if (int.TryParse(min.ToString(), out lowerBound) &&
				int.TryParse(max.ToString(), out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable long integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(long? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the long integer value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(long value, string min, string max)
		{
			long lowerBound;
			long upperBound;
			if (long.TryParse(min, out lowerBound) &&
				long.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable DateTime value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(DateTime? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value,min,max);
		}
		/// <summary>
		/// Determines whether the DateTime value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(DateTime value, string min, string max)
		{
			DateTime lowerBound;
			DateTime upperBound;
			if (DateTime.TryParse(min, out lowerBound) &&
			    DateTime.TryParse(max, out upperBound))
			{
				if (value.Ticks < lowerBound.Ticks || value.Ticks > upperBound.Ticks)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable single value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(Single? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value,min, max);
		}
		/// <summary>
		/// Determines whether the single value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(Single value, string min, string max)
		{
			Single lowerBound;
			Single upperBound;
			if (Single.TryParse(min, out lowerBound) &&
				Single.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable decimal value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(decimal? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the decimal value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(decimal value, string min, string max)
		{
			decimal lowerBound;
			decimal upperBound;
			if (decimal.TryParse(min, out lowerBound) &&
				decimal.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
		/// <summary>
		/// Determines whether the nullable double value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(double? value, string min, string max)
		{
			if (!value.HasValue) return;
			IsInValidRange(value.Value, min, max);
		}
		/// <summary>
		/// Determines whether the double value is in valid range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min value.</param>
		/// <param name="max">The max value.</param>
		public static void IsInValidRange(double value, string min, string max)
		{
			double lowerBound;
			double upperBound;
			if (double.TryParse(min, out lowerBound) &&
				double.TryParse(max, out upperBound))
			{
				if (value < lowerBound || value > upperBound)
					throw new ArgumentException(string.Format(errorMessage, lowerBound, upperBound));
			}
		}
	}
}
