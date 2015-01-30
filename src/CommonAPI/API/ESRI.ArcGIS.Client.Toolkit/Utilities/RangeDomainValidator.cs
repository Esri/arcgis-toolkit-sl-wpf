using ESRI.ArcGIS.Client.FeatureService;
using System;
using System.Linq;

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
        /// Validates subtype fields that have range domain defined.
        /// </summary>
        /// <param name="layerInfo">FeatureLayerInfo that contains the subtype information.</param>
        /// <param name="field">The field to validate.</param>
        /// <param name="typeIdValue">The current TypeIDField value to uses as lookup value for SubType</param>
        /// <param name="value">The current value to validate.</param>
        internal static void ValidateRange(FeatureLayerInfo layerInfo, Field field, object typeIdValue, object value)
        {
            if (layerInfo == null || field == null || typeIdValue == null || value == null)
                return;

            switch (field.Type)
            {
                case Field.FieldType.Date:
                    if (value is DateTime)
                    {
                        var dynamicRangeDomains = FieldDomainUtils.BuildDynamicRangeDomain<DateTime>(field, layerInfo);
                        if (dynamicRangeDomains != null && typeIdValue != null)
                        {
                            var domains = dynamicRangeDomains.Where(kvp => kvp.Key.Equals(typeIdValue));
                            if (domains != null && domains.Any())
                            {
                                var domain = domains.First().Value;
                                if (((DateTime)value) < domain.MinimumValue || ((DateTime)value) > domain.MaximumValue)
                                    throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                            }
                        }
                    }
                    break;
                case Field.FieldType.Double:
                    if (value is Double)
                    {
                        var dynamicRangeDomains = FieldDomainUtils.BuildDynamicRangeDomain<Double>(field, layerInfo);
                        if (dynamicRangeDomains != null && typeIdValue != null)
                        {
                            var domains = dynamicRangeDomains.Where(kvp => kvp.Key.Equals(typeIdValue));
                            if (domains != null && domains.Any())
                            {
                                var domain = domains.First().Value;
                                if (((Double)value) < domain.MinimumValue || ((Double)value) > domain.MaximumValue)
                                    throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                            }
                        }
                    }
                    break;
                case Field.FieldType.Integer:
                    if (value is Int32)
                    {
                        var dynamicRangeDomains = FieldDomainUtils.BuildDynamicRangeDomain<Int32>(field, layerInfo);
                        if (dynamicRangeDomains != null && typeIdValue != null)
                        {
                            var domains = dynamicRangeDomains.Where(kvp => kvp.Key.Equals(typeIdValue));
                            if (domains != null && domains.Any())
                            {
                                var domain = domains.First().Value;
                                if (((Int32)value) < domain.MinimumValue || ((Int32)value) > domain.MaximumValue)
                                    throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                            }
                        }
                    }
                    break;
                case Field.FieldType.Single:
                    if (value is Single)
                    {
                        var dynamicRangeDomains = FieldDomainUtils.BuildDynamicRangeDomain<Single>(field, layerInfo);
                        if (dynamicRangeDomains != null && typeIdValue != null)
                        {
                            var domains = dynamicRangeDomains.Where(kvp => kvp.Key.Equals(typeIdValue));
                            if (domains != null && domains.Any())
                            {
                                var domain = domains.First().Value;
                                if (((Single)value) < domain.MinimumValue || ((Single)value) > domain.MaximumValue)
                                    throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                            }
                        }
                    }
                    break;
                case Field.FieldType.SmallInteger:
                    if (value is Int16)
                    {
                        var dynamicRangeDomains = FieldDomainUtils.BuildDynamicRangeDomain<Int16>(field, layerInfo);
                        if (dynamicRangeDomains != null && typeIdValue != null)
                        {
                            var domains = dynamicRangeDomains.Where(kvp => kvp.Key.Equals(typeIdValue));
                            if (domains != null && domains.Any())
                            {
                                var domain = domains.First().Value;
                                if (((Int16)value) < domain.MinimumValue || ((Int16)value) > domain.MaximumValue)
                                    throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        internal static void ValidateRange(Field field, object value)
        {
            if (field == null || value == null || field.Domain == null || field.Domain is CodedValueDomain)
                return;

            switch (field.Type)
            {
                case Field.FieldType.Date:
                    if (value is DateTime && field.Domain is RangeDomain<DateTime>)
                    {
                        var domain = (RangeDomain<DateTime>)field.Domain;
                        if (((DateTime)value) < domain.MinimumValue || ((DateTime)value) > domain.MaximumValue)
                            throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                    }
                    break;
                case Field.FieldType.Double:
                    if (value is Double && field.Domain is RangeDomain<Double>)
                    {
                        var domain = (RangeDomain<Double>)field.Domain;
                        if (((Double)value) < domain.MinimumValue || ((Double)value) > domain.MaximumValue)
                            throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                    }
                    break;
                case Field.FieldType.Integer:
                    if (value is Int32 && field.Domain is RangeDomain<Int32>)
                    {
                        var domain = (RangeDomain<Int32>)field.Domain;
                        if (((Int32)value) < domain.MinimumValue || ((Int32)value) > domain.MaximumValue)
                            throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                    }
                    break;
                case Field.FieldType.Single:
                    if (value is Single && field.Domain is RangeDomain<Single>)
                    {
                        var domain = (RangeDomain<Single>)field.Domain;
                        if (((Single)value) < domain.MinimumValue || ((Single)value) > domain.MaximumValue)
                            throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                    }
                    break;
                case Field.FieldType.SmallInteger:
                    if (value is Int16 && field.Domain is RangeDomain<Int16>)
                    {
                        var domain = (RangeDomain<Int16>)field.Domain;
                        if (((Int16)value) < domain.MinimumValue || ((Int16)value) > domain.MaximumValue)
                            throw new ArgumentException(string.Format(errorMessage, domain.MinimumValue, domain.MaximumValue));
                    }
                    break;
            }
        }


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
