// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

namespace ESRI.ArcGIS.Client.Toolkit.Utilities
{
    internal static class DataSourceCreator
    {
        private static readonly Regex improperNameRegex =
                new Regex(@"[^A-Z^a-z^0-9^_^ ]", RegexOptions.Singleline);
        private static readonly Dictionary<string, Type> _typeBySignature = new Dictionary<string, Type>();
        private static Dictionary<string, string> _fieldMapping = null;
		
        private static Type GetTypeByTypeSignature(string typeSignature)
        {
            Type type;
            return _typeBySignature.TryGetValue(typeSignature, out type) ? type : null;
        }

        private static Type GetValueType(string key, object value, Dictionary<string, Type> fieldInfo)
        {
            if (fieldInfo != null && fieldInfo.ContainsKey(key))
                return fieldInfo[key];
			else if (value != null)
			{
				if (value.GetType() == typeof(string))
					return typeof(string);
				if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
					return typeof(DateTime?);
				if (value.GetType() == typeof(bool) || value.GetType() == typeof(bool?))
					return typeof(bool?);
				if (value.GetType() == typeof(byte) || value.GetType() == typeof(byte?))
					return typeof(byte?);
				if (value.GetType() == typeof(short) || value.GetType() == typeof(short?))
					return typeof(short?);
				if (value.GetType() == typeof(int) || value.GetType() == typeof(int?))
					return typeof(int?);
				if (value.GetType() == typeof(long) || value.GetType() == typeof(long?))
					return typeof(long?);
				if (value.GetType() == typeof(float) || value.GetType() == typeof(float?))
					return typeof(float?);
				if (value.GetType() == typeof(double) || value.GetType() == typeof(double?))
					return typeof(double?);
				else
				    return value.GetType();		// User-defined data types
			}
			return typeof(object);
        }

        private static string GetTypeSignature(IDictionary firstDict, Dictionary<string, Type> fieldInfo)
        {
            StringBuilder sb = new StringBuilder();
            if (fieldInfo != null)
            {
                foreach (string key in fieldInfo.Keys)
                {
                    sb.AppendFormat("_{0}_{1}", key, fieldInfo[key]);
                }
            }
            else
            {
                foreach (DictionaryEntry pair in firstDict)
                {
                    sb.AppendFormat("_{0}_{1}", pair.Key, GetValueType(pair.Key.ToString(), pair.Value, fieldInfo));
                }
            }
            return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
        }

        private static IEnumerable GenerateEnumerable(Type objectType, IEnumerable<Graphic> graphics,
                                                      string[] types, IEnumerable<Graphic> filter)
        {
			IEnumerable graphicsAfterFilter = filter != null ? graphics.Intersect<Graphic>(filter) : graphics;
            var listType = typeof(ObservableCollection<>).MakeGenericType(new[] { objectType });
            var listOfCustom = Activator.CreateInstance(listType);

			foreach (Graphic g in graphicsAfterFilter)
            {
				var row = AttributeListToObject(objectType, g, null, types);
                listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
            }
            return listOfCustom as IEnumerable;
        }

        private static TypeBuilder GetTypeBuilder(string typeSignature)
        {
            AssemblyName an = new AssemblyName("ESRI.ArcGIS.Client.Toolkit.FeatureDataGrid." + typeSignature);
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
			
			TypeBuilder tb = moduleBuilder.DefineType("ESRI.ArcGIS.Client.Toolkit.DataSource.TempType" + typeSignature
								, TypeAttributes.Public |
								TypeAttributes.Class |
								TypeAttributes.AutoClass |
								TypeAttributes.AnsiClass |
								TypeAttributes.BeforeFieldInit |
								TypeAttributes.AutoLayout
								, typeof(object));
			return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType, 
                                           Dictionary<string, object[]> rangeDomainInfo, Field field, int order, bool isKey)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName,
                                                        propertyType,
                                                        FieldAttributes.Private);
            PropertyBuilder propertyBuilder =
                    tb.DefineProperty(
                        propertyName, PropertyAttributes.HasDefault, propertyType, null);

            string displayName = null;
            if (field != null)
                displayName = field.Alias;
			if (string.IsNullOrEmpty(displayName))
				displayName = _fieldMapping.KeyOfValue(propertyName);
            if (!string.IsNullOrEmpty(displayName))
            {
                // Add DisplayAttribute to property [DisplayAttribute(Name = displayName)]
                Type displayAttribute = typeof(DisplayAttribute);
                PropertyInfo info = displayAttribute.GetProperty("Name");
                PropertyInfo info2 = displayAttribute.GetProperty("AutoGenerateField");
                PropertyInfo info3 = displayAttribute.GetProperty("Order");
                CustomAttributeBuilder cabuilder = new CustomAttributeBuilder(
                    displayAttribute.GetConstructor(new Type[] { }), 
                    new object[] { },
                    new PropertyInfo[] { info, info2, info3 }, new object[] { displayName, !isKey, order });
                propertyBuilder.SetCustomAttribute(cabuilder);
            }
            if (isKey)
            {
                // Add KeyAttribute to property [KeyAttribute]
                CustomAttributeBuilder cabuilder = new CustomAttributeBuilder(
                    typeof(KeyAttribute).GetConstructor(new Type[] { }),
                    new object[] { });
                propertyBuilder.SetCustomAttribute(cabuilder);
            }

            if (field != null)
            {
                if (!field.Nullable && field.Type != Field.FieldType.GlobalID)
                {
                    // Add Required to property using standard validation error [Required()]
                    CustomAttributeBuilder requiredCABuilder = new CustomAttributeBuilder(
                        typeof(System.ComponentModel.DataAnnotations.RequiredAttribute).GetConstructor(new Type[] { }),
                        new object[] { });
                    propertyBuilder.SetCustomAttribute(requiredCABuilder);
                }

                // Add AllowEdit to property and allow an initial value
                PropertyInfo allowEditInfo = typeof(EditableAttribute).GetProperty("AllowEdit");
                PropertyInfo allowInitialValueInfo = typeof(EditableAttribute).GetProperty("AllowInitialValue");
                CustomAttributeBuilder allowEditCABuilder = new CustomAttributeBuilder(
                    typeof(EditableAttribute).GetConstructor(new Type[] { typeof(bool) }),
                    new object[] { field.Editable },
                    new PropertyInfo[] { allowEditInfo, allowInitialValueInfo }, new object[] { field.Editable, true });
                propertyBuilder.SetCustomAttribute(allowEditCABuilder);


                // Add MaximumLength to property
                if (field.Type == Field.FieldType.String && field.Length > 0)
                {
                    PropertyInfo maximumLengthInfo = typeof(StringLengthAttribute).GetProperty("MaximumLength");
                    CustomAttributeBuilder cabuilder = new CustomAttributeBuilder(
                        typeof(StringLengthAttribute).GetConstructor(new Type[] { typeof(int) }),
                        new object[] { field.Length },
                        new PropertyInfo[] { maximumLengthInfo }, new object[] { field.Length });
                    propertyBuilder.SetCustomAttribute(cabuilder);
                }
            }

            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                // Add DisplayAttribute to property [DisplayFormatAttribute(DataFormatString = "m/dd/YYYY")]
                PropertyInfo info = typeof(DisplayFormatAttribute).GetProperty("DataFormatString");
                CustomAttributeBuilder cabuilder = new CustomAttributeBuilder(
                    typeof(DisplayFormatAttribute).GetConstructor(new Type[] { }),
                    new object[] { },
                    new PropertyInfo[] { info }, new object[] { System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern });
                propertyBuilder.SetCustomAttribute(cabuilder);
            }

            MethodBuilder getPropMthdBldr =
                tb.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    propertyType, Type.EmptyTypes);

            ILGenerator getIL = getPropMthdBldr.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                    tb.DefineMethod("set_" + propertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, new Type[] { propertyType });

            ILGenerator setIL = setPropMthdBldr.GetILGenerator();

            if (rangeDomainInfo != null)
            {
                if (rangeDomainInfo.ContainsKey(propertyName))
                {
                    object[] rangeValues = rangeDomainInfo[propertyName];
                    if (rangeValues.Length == 2)
                    {
						string min = rangeValues[0].ToString();
						string max = rangeValues[1].ToString();
                        MethodInfo methodValidator = typeof(RangeDomainValidator).GetMethod("IsInValidRange",
                                                            BindingFlags.Public | BindingFlags.Static,
                                                            null, CallingConventions.Standard,
															new Type[] { propertyType, typeof(string), typeof(string)}, null);

                        setIL.Emit(OpCodes.Ldarg_1);                            // Pushing the value parameter
						setIL.Emit(OpCodes.Ldstr, min);							// Push string literal onto the top of the stack
						setIL.Emit(OpCodes.Ldstr, max);							// Push string literal onto the top of the stack
                        setIL.EmitCall(OpCodes.Call, methodValidator, null);    // Invoking IsInValidRange() method of the RangeDomainValidator class
                    }
                }
            }

            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
		
        private static FieldInfo[] GetFieldInfo(Type objectType)
        {
            return objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        private static void AddToEnumerable(Type objectType, Graphic graphic, IEnumerable itemsSource)
        {
            var listType = typeof(ObservableCollection<>).MakeGenericType(new[] { objectType });
            var row = AttributeListToObject(objectType, graphic, graphic.Attributes.Keys, null);			
            
			listType.GetMethod("Add").Invoke((itemsSource as ICollectionView).SourceCollection, new[] { row });	
        }

        private static string GetUniqueValue(Dictionary<string, string> dictionary, string valueToBeInserted)
        {
            int i = 0;
            string prefix = "";
            while (dictionary.ContainsValue(valueToBeInserted + prefix))
            {
                prefix = string.Format("_{0}", ++i);
            }
            return string.Format("{0}{1}", valueToBeInserted, prefix);
        }

        private static string GetMappedKey(string key)
        {
            string mappedKey = key;
            MatchCollection improperNames = improperNameRegex.Matches(key, 0);

            if (improperNames.Count > 0)
            {
                foreach (Match improperName in improperNames)
                    mappedKey = mappedKey.Replace(improperName.Value, "_");
            }
            if (_fieldMapping.ContainsValue(mappedKey))
                mappedKey = GetUniqueValue(_fieldMapping, mappedKey);
            _fieldMapping.Add(key, mappedKey);

            return mappedKey;
        }

        private static string KeyOfValue(this Dictionary<string, string> dictionary, string value)
        {
            if (dictionary.ContainsValue(value))
            {
                foreach (string key in dictionary.Keys)
                {
                    if (dictionary[key] == value)
                        return key;
                }
            }

            return null;
        }

		private static object AttributeListToObject(Type objectType, Graphic graphic, 
													IEnumerable<string> attributeKeys, IEnumerable<string> propertyNames)
        {
			var row = Activator.CreateInstance(objectType, new object[] { graphic });
            foreach (string type in propertyNames ?? attributeKeys)
            {
                IDictionary<string, object> currentDict = graphic.Attributes;
                string propertyName = type;
				string attributeKey = type;
				if (propertyNames == null)
				{
					if (_fieldMapping.ContainsKey(attributeKey))
						propertyName = _fieldMapping[attributeKey];
				}
				else
					attributeKey = _fieldMapping.KeyOfValue(propertyName);
				if (propertyName != null && currentDict != null)
                {
                    if (currentDict.ContainsKey(attributeKey))
                        SetProperty(currentDict[attributeKey], row, objectType.GetProperty(propertyName));
                }
            }
			return row;
        }

        private static IEnumerable<IDictionary<string, object>> GetGraphicsEnumerable(IEnumerable<Graphic> graphics)
        {
			if (graphics != null)
				return (from a in graphics select a.Attributes).AsEnumerable<IDictionary<string, object>>();
			return null;
        }
        
        private static bool AllAttributesMatch(this Graphic graphic, Type objectType)
        {
            if (objectType != null)
            {
                FieldInfo[] fieldInfo = GetFieldInfo(objectType);
                foreach (FieldInfo fldInfo in fieldInfo)
                {
                    Type fldType = fldInfo.FieldType;
                    string fldName = _fieldMapping.KeyOfValue(fldInfo.Name.Substring(1));
                    if (fldName != null && !graphic.Attributes.ContainsKey(fldName))
                        return false;
                }
            }

            return true;
        }

        internal static void SetProperty(object value, object instance, PropertyInfo property)
        {
            try
            {
				if (value != null)
				{
					Type valueType = value.GetType();
					if (property.PropertyType.IsValueType && property.PropertyType.IsGenericType
						&& property.PropertyType.FullName.StartsWith("System.Nullable`1[["))    // Nullable value type
					{
						Type[] args = property.PropertyType.GetGenericArguments();
						if (args != null && args.Length > 0 && args[0] != valueType)
						{
							value = Convert.ChangeType(value, args[0], null);
						}
					}
					else if (valueType != property.PropertyType)
					{
						value = Convert.ChangeType(value, property.PropertyType, null);
					}
					property.SetValue(instance, value, null);
				}
				else
				{
					// if supports nullable types then set value to null.
					if (property.PropertyType.IsValueType && property.PropertyType.IsGenericType
					   && property.PropertyType.FullName.StartsWith("System.Nullable`1[["))    
					{
						property.SetValue(instance, value, null);
					}				
					else if (property.PropertyType == typeof(string))						
						property.SetValue(instance, value, null);					
				}
            }
            catch
            {
            }
        }

		internal static bool IsOfType(this Type type, Type typeToCompare)
		{
			if (type == typeof(object) || typeToCompare == typeof(object))
				return true;

			Type type1 = type;
			Type type2 = typeToCompare;
			if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				type1 = type.GetGenericArguments()[0];
			if (typeToCompare.IsGenericType && typeToCompare.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				type2 = typeToCompare.GetGenericArguments()[0];
			return (type1 == type2);
		}

		internal static IEnumerable ToDataSource(this IEnumerable<Graphic> graphics, 
                                                 Dictionary<string, Type> fieldInfo, 
                                                 Dictionary<string, object[]> rangeDomainInfo,
                                                 Dictionary<string, Field> fieldProps,
                                                 string uniqueID,
												 IEnumerable<Graphic> filterGraphics,
                                                 out Type objectType)
        {
            objectType = null;
            IDictionary firstDict = null;
            IEnumerable<IDictionary<string, object>> list = GetGraphicsEnumerable(graphics);
            if (fieldInfo == null)
            {
                if (list != null)
                {
                    IEnumerator enumList = list.GetEnumerator();
                    if (enumList != null && enumList.MoveNext())
                    {
                        firstDict = enumList.Current as IDictionary;
						if (firstDict != null)
                        {
                            while (enumList.MoveNext())
                            {
                                IDictionary nextDict = enumList.Current as IDictionary;
								foreach (DictionaryEntry pair in nextDict)
                                {
									Type t = GetValueType(pair.Key as string, pair.Value, fieldInfo);
									if (!firstDict.Contains(pair.Key))	// Attribute doesn't exist => add it
									{
										object defaultValue = t.IsValueType ? Activator.CreateInstance(t) : null;
										firstDict.Add(pair.Key, defaultValue);
									}
									else   // Attribute exists => check for data type compatibility
									{
										Type typeInFirstDict = GetValueType(pair.Key as string, firstDict[pair.Key], fieldInfo);
										if (typeInFirstDict != typeof(object) &&		// object type in firstDict covers all data types
											pair.Value != null &&	// null values in nextDict match the nullable data type associated with the attribute in firstDict
											!t.IsOfType(typeInFirstDict))	// attribute data types don't match
											throw new InvalidCastException(string.Format(Properties.Resources.FeatureDataGrid_MixedAttributeTypesNotAllowed, 
																						 firstDict[pair.Key].GetType(), pair.Key));
									}
                                }
                            }
                        }
                    }
                }
            }
            if(firstDict == null && fieldInfo == null)
                return new object[] { };

            string typeSignature = GetTypeSignature(firstDict, fieldInfo);
            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            _fieldMapping = new Dictionary<string, string>();
            if (fieldInfo != null)  // FeatureLayer
            {
                foreach (string key in fieldInfo.Keys)
                {
                    string keyToUse = GetMappedKey(key);
                    Type t = fieldInfo[key];
                    properties.Add(keyToUse, t);
                }
            }
            else                    // GraphicsLayer
            {
                foreach (DictionaryEntry pair in firstDict)
                {
                   string keyToUse = GetMappedKey(pair.Key.ToString());
                   Type t = GetValueType(keyToUse, pair.Value, fieldInfo);
                   properties.Add(keyToUse, t);
                }
            }
            objectType = GetTypeByTypeSignature(typeSignature);
           
            if (objectType == null)
            {
                TypeBuilder tb = GetTypeBuilder(typeSignature);

				FieldBuilder fieldBuilder = tb.DefineField("_graphicSibling", typeof(Graphic), FieldAttributes.Private);
				ConstructorBuilder constructorBuilder = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(Graphic) });
				ILGenerator cbIL = constructorBuilder.GetILGenerator();
				cbIL.Emit(OpCodes.Ldarg_0);
				cbIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
				cbIL.Emit(OpCodes.Ldarg_0);
				cbIL.Emit(OpCodes.Ldarg_1);
				cbIL.Emit(OpCodes.Stfld, fieldBuilder);
				cbIL.Emit(OpCodes.Ret);

				MethodBuilder methodBuilder = tb.DefineMethod("GetGraphicSibling", MethodAttributes.Public, typeof(Graphic), Type.EmptyTypes);
				ILGenerator mbIL = methodBuilder.GetILGenerator();
				mbIL.Emit(OpCodes.Ldarg_0);
				mbIL.Emit(OpCodes.Ldfld, fieldBuilder);
				mbIL.Emit(OpCodes.Ret);

                int order = 0;
                foreach (string key in properties.Keys)
                {
                    if (fieldProps != null) // FeatureLayer
                    {
                        if (_fieldMapping.KeyOfValue(key) != null)
                        {
                            string mappedKey = _fieldMapping.KeyOfValue(key);
                            if (fieldProps.ContainsKey(mappedKey))    // Only create the property if the key is contained in field properties
                            {
                                Field fld = fieldProps[mappedKey];
                                if (IsViewableAttribute(fld))
                                    CreateProperty(tb, key, properties[key], rangeDomainInfo, fld, order++, uniqueID == key);
                            }
                        }
                    }
                    else    // GraphicsLayer
                        CreateProperty(tb, key, properties[key], rangeDomainInfo, null, order++, uniqueID == key);
                }
               
                objectType = tb.CreateType();

                _typeBySignature.Add(typeSignature, objectType);
            }
            return GenerateEnumerable(objectType, graphics, properties.Keys.ToArray(),filterGraphics);
        }

        internal static IList AsList(this IEnumerable itemsSource)
        {
			ICollectionView collectionView = itemsSource as ICollectionView;
			if (collectionView == null)		// The SourceCollection of the ICollectionView is passed, i.e. IEnumerable.
				return (IList)itemsSource;
			else     // The ICollectionView is passed.
			{
				IList retVal = new List<object>();
				foreach (object obj in collectionView)
					retVal.Add(obj);
				return retVal as IList;
        	}
        }

        internal static void AddToDataSource(this IEnumerable itemsSource, Graphic graphic, Type objectType)
        {
            if (objectType != null)
                AddToEnumerable(objectType, graphic, itemsSource);
        }

        internal static void RemoveFromDataSource(this IEnumerable itemsSource, int indexToRemove, Type objectType)
        {
            var listType = typeof(ObservableCollection<>).MakeGenericType(new[] { objectType });
			listType.GetMethod("RemoveAt").Invoke((itemsSource as ICollectionView).SourceCollection, new[] { (object)indexToRemove });
        }

        internal static void RefreshGraphic(this DataGridRow dataGridRow, Graphic graphic, Type objectType)
        {
            object data = dataGridRow.DataContext;
            data.RefreshGraphic(graphic, objectType);
        }

        internal static void RefreshGraphic(this object currentItem, Graphic graphic, Type objectType)
        {
            if (objectType != null && currentItem != null)
            {
                FieldInfo[] fieldInfo = GetFieldInfo(objectType);
                foreach (FieldInfo fldInfo in fieldInfo)
                {
                    string key = fldInfo.Name.Substring(1);
                    try
                    {
						if (_fieldMapping.KeyOfValue(key) != null)
						{
							if (graphic.Attributes.ContainsKey(_fieldMapping.KeyOfValue(key)))
							{
								object valueInDataGridRow = currentItem.GetType().GetProperty(key).GetValue(currentItem, null);
								object valueInGraphic = graphic.Attributes[_fieldMapping.KeyOfValue(key)];
								if (valueInDataGridRow != null)
								{
									if (!valueInDataGridRow.Equals(valueInGraphic))
									{
										try
										{
											graphic.Attributes[_fieldMapping.KeyOfValue(key)] = currentItem.GetType().GetProperty(key).GetValue(currentItem, null);
										}
										catch (Exception ex)
										{
											ArgumentNullException argumentNullException = ex as ArgumentNullException;
											if (argumentNullException == null)
												throw new Exception(ex.Message);
										}
									}
								}
								else if (valueInGraphic != null)
									graphic.Attributes[_fieldMapping.KeyOfValue(key)] = currentItem.GetType().GetProperty(key).GetValue(currentItem, null);
							}
							else
							{
								//Only add attribute to graphic if the new value is not null
								var value = currentItem.GetType().GetProperty(key).GetValue(currentItem, null);
								if(value != null)
									graphic.Attributes[_fieldMapping.KeyOfValue(key)] = value;
							}
						}
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(string.Format(Properties.Resources.FeatureDataGrid_InvalidObjectTypeForKey, key), ex);
                    }
                }
            }
            else
            {
                throw new NullReferenceException(Properties.Resources.FeatureDataGrid_ObjectIsNullOrInvalidType);
            }
        }

        internal static void RefreshRow(this Graphic graphic, IEnumerable itemsSource, int itemIndex, Type objectType)
        {
            if (objectType != null)
                itemsSource.AsList()[itemIndex] = AttributeListToObject(objectType, graphic, graphic.Attributes.Keys, null);
        }

        internal static string GetDisplayName(this Type objectType, string propertyPath)
        {
            if (objectType != null)
            {
                System.Reflection.PropertyInfo property = objectType.GetProperty(propertyPath);
                if (property != null)
                {
                    object[] customAttributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        DisplayAttribute attribute = customAttributes[0] as DisplayAttribute;
                        if (attribute != null)
                            return attribute.GetShortName();
                    }
                }
            }
            return propertyPath;
        }

        internal static bool IsViewableAttribute(Field field)
        {
            if (field.Type != Field.FieldType.Blob && field.Type != Field.FieldType.Geometry &&
                field.Type != Field.FieldType.Raster && field.Type != Field.FieldType.Unknown)
                return true;

            return false;
        }

		internal static Graphic GetGraphicSibling(object item)
		{
			if (item != null)
			{
				MethodInfo mi = item.GetType().GetMethod("GetGraphicSibling");
				if (mi != null)
					return mi.Invoke(item, null) as Graphic;
			}
			return null;
		}

		internal static string MappedKey(this string propertyName)
		{
			return _fieldMapping != null ? _fieldMapping.KeyOfValue(propertyName) : propertyName;
		}
	}
}