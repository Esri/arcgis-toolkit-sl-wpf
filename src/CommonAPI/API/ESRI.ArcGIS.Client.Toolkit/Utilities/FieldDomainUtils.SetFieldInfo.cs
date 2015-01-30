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
		/// <param name="fieldProps">The properties associated with the field.</param>
		/// <returns>Dictionary of field types keyed by their actual names. Also, populates range domain information and field properties if any.</returns>
		internal static Dictionary<string, Type> SetFieldInfo(FeatureLayerInfo featureLayerInfo, out Dictionary<string, Field> fieldProps)
		{
			Dictionary<string, Type> fieldInfo = null;
			fieldProps = new Dictionary<string, Field>();
			if (featureLayerInfo != null)
			{
				fieldInfo = new Dictionary<string, Type>();
				foreach (Field field in featureLayerInfo.Fields)
				{
                    if (fieldInfo.ContainsKey(field.Name))
                        continue;

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
                        case Field.FieldType.GUID:
                            fieldType = typeof(Guid);
                            break;
						default:
							throw new NotSupportedException(string.Format(Properties.Resources.FieldDomain_FieldTypeNotSupported, fieldType.GetType()));
					}
					fieldInfo.Add(field.Name, fieldType);
					fieldProps.Add(field.Name, field);
				}
			}
			return fieldInfo;
		}
	}
}
