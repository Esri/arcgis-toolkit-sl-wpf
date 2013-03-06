using System.Collections.Generic;
using ESRI.ArcGIS.Client.FeatureService;

namespace ESRI.ArcGIS.Client.Toolkit.Utilities
{
	internal static partial class FieldDomainUtils
	{
		/// <summary>
		/// Checks Fields and determines if Field is Dynamic Coded Value Domain (aka Sub Domain)
		/// </summary>
		/// <param name="field">The field that needs to be checked.</param>
		/// <param name="layerInfo">
		/// The FeatureLayerInfo that has the information to determine if the field
		/// is a dynamic coded value domain (aka Sub Domain).
		/// </param>
		/// <returns>Boolean value indicating if the field is a DynamicCodedValueDomain (aka Sub Domain)</returns>
		internal static bool IsDynamicDomain(Field field, FeatureLayerInfo layerInfo)
		{
			bool result = false;
			if (field.Domain == null && layerInfo != null
				&& layerInfo.FeatureTypes != null
				&& layerInfo.FeatureTypes.Count > 0)
			{
				foreach (object key in layerInfo.FeatureTypes.Keys)
				{
					FeatureType featureType = layerInfo.FeatureTypes[key];
					if (featureType.Domains.ContainsKey(field.Name))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Builds a DynamicCodedValueSource object used to lookup display value.
		/// </summary>
		/// <param name="field">The field to build DynamicCodedValueSource for</param>
		/// <param name="layerInfo">The FeatureLayerInfo used to lookup all the coded value domains </param>
		/// <returns>DynamicCodedValueSource which is a collection of coded value domain values for a field.</returns>
		internal static DynamicCodedValueSource BuildDynamicCodedValueSource(Field field, FeatureLayerInfo layerInfo)
		{
			DynamicCodedValueSource dynamicCodedValueSource = null;
			foreach (object key in layerInfo.FeatureTypes.Keys)
			{
				FeatureType featureType = layerInfo.FeatureTypes[key];
				if (featureType.Domains.ContainsKey(field.Name))
				{
					if (dynamicCodedValueSource == null)
						dynamicCodedValueSource = new DynamicCodedValueSource();

					CodedValueDomain codedValueDomain = featureType.Domains[field.Name] as CodedValueDomain;
					if (codedValueDomain != null)
					{
						CodedValueSources codedValueSources = null;
						foreach (KeyValuePair<object, string> kvp in codedValueDomain.CodedValues)
						{
							if (codedValueSources == null)
							{
								codedValueSources = new CodedValueSources();
								if (field.Nullable)
								{
									CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };
									codedValueSources.Add(nullableSource);
								}
							}

							codedValueSources.Add(new CodedValueSource() { Code = kvp.Key, DisplayName = kvp.Value });
						}
						if (codedValueSources != null)
						{
							if (dynamicCodedValueSource == null)
								dynamicCodedValueSource = new DynamicCodedValueSource();

							dynamicCodedValueSource.Add(featureType.Id, codedValueSources);
						}
					}
				}
			}
			return dynamicCodedValueSource;
		}
		/// <summary>
		/// Returns CodedValueSources that make up the display text of each value of the TypeIDField.
		/// </summary>
		/// <param name="field">TypeIDField</param>
		/// <param name="layerInfo">FeatureLayerInof used to construct the CodedValueSources from the FeatureTypes.</param>
		/// <returns>CodedValueSoruces that contain code an value matches to all possible TypeIDField values.</returns>
		internal static CodedValueSources BuildTypeIDCodedValueSource(Field field, FeatureLayerInfo layerInfo)
		{
			CodedValueSources codedValueSources = null;
			foreach (KeyValuePair<object, FeatureType> kvp in layerInfo.FeatureTypes)
			{
				if (kvp.Key == null) continue;
				string name = (kvp.Value != null && kvp.Value.Name != null) ? kvp.Value.Name : "";
				CodedValueSource codedValueSource = new CodedValueSource() { Code = kvp.Key, DisplayName = name };
				if (codedValueSources == null)
				{
					codedValueSources = new CodedValueSources();
					if (field.Nullable)
					{
						CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };
						codedValueSources.Add(nullableSource);
					}
				}
				codedValueSources.Add(codedValueSource);
			}
			return codedValueSources;
		}
		/// <summary>
		/// Returns a CodedValueSources object constructed form the CodedValueDomain value.
		/// </summary>
		/// <param name="field">The field to make a CodedValueSource from.</param>
		/// <returns>The CodedValueSources object used for a code to value lookup.</returns>
		internal static CodedValueSources BuildCodedValueSource(Field field)
		{
			CodedValueDomain codedValueDomain = field.Domain as CodedValueDomain;
			CodedValueSources codedValueSources = null;
			if (codedValueDomain != null)
			{
				foreach (KeyValuePair<object, string> codedValue in codedValueDomain.CodedValues)
				{
					if (codedValueSources == null)
					{
						codedValueSources = new CodedValueSources();
						if (field.Nullable)
						{
							CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };
							codedValueSources.Add(nullableSource);
						}
					}
					codedValueSources.Add(new CodedValueSource() { Code = codedValue.Key, DisplayName = codedValue.Value });
				}
			}
			return codedValueSources;
		}
	}
}
