using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.FeatureService;

namespace ESRI.ArcGIS.Client.Toolkit.Utilities
{
	internal static partial class FieldDomainUtils
	{
		/// <summary>
		/// Sets the field information in the feature layer.
		/// </summary>
		/// <param name="featureLayerInfo">The feature layer info.</param>
		/// <param name="rangeDomainInfo">The range domain info.</param>
		/// <param name="fieldProps">The properties associated with the field.</param>
		/// <returns>Dictionary of field types keyed by their actual names. Also, populates range domain information and field properties if any.</returns>
		internal static Dictionary<string, Type> SetFieldInfo(FeatureLayerInfo featureLayerInfo, out Dictionary<string, object[]> rangeDomainInfo, out Dictionary<string, Field> fieldProps)
		{
			Dictionary<string, Type> fieldInfo = null;
			rangeDomainInfo = null;
			fieldProps = new Dictionary<string, Field>();
			if (featureLayerInfo != null)
			{
				fieldInfo = new Dictionary<string, Type>();
				foreach (Field field in featureLayerInfo.Fields)
				{
					Type fieldType = typeof(object);
					switch (field.Type)
					{
						case Field.FieldType.Date:
							fieldType = typeof(DateTime?);
							break;
						case Field.FieldType.Double:
							fieldType = typeof(double?);
							break;
						case Field.FieldType.Integer:
							fieldType = typeof(int?);
							break;
						case Field.FieldType.OID:
							fieldType = typeof(int);
							break;
						case Field.FieldType.Geometry:
						case Field.FieldType.GUID:
						case Field.FieldType.Blob:
						case Field.FieldType.Raster:
						case Field.FieldType.Unknown:
							fieldType = typeof(object);
							break;
						case Field.FieldType.Single:
							fieldType = typeof(float?);
							break;
						case Field.FieldType.SmallInteger:
							fieldType = typeof(short?);
							break; ;
						case Field.FieldType.GlobalID:
						case Field.FieldType.String:
						case Field.FieldType.XML:
							fieldType = typeof(string);
							break;
						default:
							throw new NotSupportedException(string.Format(Properties.Resources.FieldDomain_FieldTypeNotSupported, fieldType.GetType()));
					}
					fieldInfo.Add(field.Name, fieldType);
					fieldProps.Add(field.Name, field);
					// Populating the range domain info if any:
					if (field.Domain != null)
					{
						switch (field.Type)
						{
							case Field.FieldType.Date:
								RangeDomain<DateTime> dateTimeRangeDomain = field.Domain as RangeDomain<DateTime>;
								if (dateTimeRangeDomain != null)
								{
									if (rangeDomainInfo == null)
										rangeDomainInfo = new Dictionary<string, object[]>();
									rangeDomainInfo.Add(field.Name, new object[] { dateTimeRangeDomain.MinimumValue, dateTimeRangeDomain.MaximumValue });
								}
								break;
							case Field.FieldType.Double:
								RangeDomain<double> doubleRangeDomain = field.Domain as RangeDomain<double>;
								if (doubleRangeDomain != null)
								{
									if (rangeDomainInfo == null)
										rangeDomainInfo = new Dictionary<string, object[]>();
									rangeDomainInfo.Add(field.Name, new object[] { doubleRangeDomain.MinimumValue, doubleRangeDomain.MaximumValue });
								}
								break;
							case Field.FieldType.Integer:
								RangeDomain<int> intRangeDomain = field.Domain as RangeDomain<int>;
								if (intRangeDomain != null)
								{
									if (rangeDomainInfo == null)
										rangeDomainInfo = new Dictionary<string, object[]>();
									rangeDomainInfo.Add(field.Name, new object[] { intRangeDomain.MinimumValue, intRangeDomain.MaximumValue });
								}
								break;
							case Field.FieldType.Single:
								RangeDomain<Single> singleRangeDomain = field.Domain as RangeDomain<Single>;
								if (singleRangeDomain != null)
								{
									if (rangeDomainInfo == null)
										rangeDomainInfo = new Dictionary<string, object[]>();
									rangeDomainInfo.Add(field.Name, new object[] { singleRangeDomain.MinimumValue, singleRangeDomain.MaximumValue });
								}
								break;
							case Field.FieldType.SmallInteger:
								RangeDomain<short> shortRangeDomain = field.Domain as RangeDomain<short>;
								if (shortRangeDomain != null)
								{
									if (rangeDomainInfo == null)
										rangeDomainInfo = new Dictionary<string, object[]>();
									rangeDomainInfo.Add(field.Name, new object[] { shortRangeDomain.MinimumValue, shortRangeDomain.MaximumValue });
								}
								break;
						}
					}
				}
			}
			return fieldInfo;
		}
	}
}
