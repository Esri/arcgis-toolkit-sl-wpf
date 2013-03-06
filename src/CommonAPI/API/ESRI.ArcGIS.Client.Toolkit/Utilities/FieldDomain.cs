// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using ESRI.ArcGIS.Client.FeatureService;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ESRI.ArcGIS.Client.Toolkit.Utilities
{
	#region Coded Value Domain
	/// <summary>
	/// *FOR INTERNAL USE ONLY* The CodedValueSource class.
	/// </summary>
	/// <remarks>Used to populate each entry in the coded value domain.</remarks>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class CodedValueSource
	{
		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		public object Code { get; set; }
		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get; set; }
		/// <summary>
		/// Gets or sets a true or false value indicating if this item is a 
		/// temporary place holder item.
		/// </summary>		
		internal bool Temp { get; set; }
 
	}
	/// <summary>
	/// *FOR INTERNAL USE ONLY* The CodedValueSources class.
	/// </summary>
	/// <remarks>Used to maintain collection of coded value domains.</remarks>	
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class CodedValueSources : List<CodedValueSource>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodedValueSources"/> class.
		/// </summary>
		public CodedValueSources() : base() { }

		internal static string CodedValueNameLookup(string field, object generic, CodedValueSources codedValueSources)
		{
#if SILVERLIGHT
			PropertyInfo fieldProperty = generic.GetType().GetProperty(field);
			if (fieldProperty == null)
				return "";

			var code = fieldProperty.GetValue(generic, null);
#else
			var code =  generic is Graphic ? (generic as Graphic).Attributes[field] : null;
#endif
			if (code == null)
				return "";

			foreach (CodedValueSource codedValueSource in codedValueSources)
			{
				if (codedValueSource.Code != null && code != null)
				{
					if (codedValueSource.Code.ToString() == code.ToString())
						return codedValueSource.DisplayName;
				}
				else if (codedValueSource.Code == null && code == null)
					return codedValueSource.DisplayName;
			}
			return code.ToString();
		}

		internal static object CodedValueCodeLookup(string field, object generic, CodedValueSources codedValueSources)
		{
#if SILVERLIGHT
			PropertyInfo fieldProperty = generic.GetType().GetProperty(field);
			if (fieldProperty == null)
				return null;

			var code = fieldProperty.GetValue(generic, null);
#else
			var code = generic is Graphic ? (generic as Graphic).Attributes[field] : null;
#endif
			if (code == null)
				return null;
			
			return code;
		}
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* DynamicCodedValueSource is used to hold 
	/// CodedValueSources that change depending on the a value of another graphic attribute. 
	/// </summary>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DynamicCodedValueSource : Dictionary<object, CodedValueSources>
	{

		internal static CodedValueSources GetCodedValueSources(string lookupField,Field field, object generic, DynamicCodedValueSource dynamicCodedValueSource, CodedValueSources nullableSources)
		{
			if (dynamicCodedValueSource != null && generic != null && !string.IsNullOrEmpty(lookupField))
			{
#if SILVERLIGHT
				PropertyInfo lookupFieldProperty = generic.GetType().GetProperty(lookupField);
				if (lookupFieldProperty == null)
					return null;

				var key = lookupFieldProperty.GetValue(generic, null);
#else
				var key = (generic is Graphic) ? (generic as Graphic).Attributes[lookupField] : null;
#endif
				if (key == null)
				{
					object code = CodedValueSources.CodedValueCodeLookup(field.Name, generic, new CodedValueSources());
					nullableSources.Clear();				
					if (field.Nullable)
					{																		
						CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };						
						nullableSources.Add(nullableSource);						
					}
					if (code != null)
					{
						CodedValueSource currentSource = new CodedValueSource() { Code = code, DisplayName = code.ToString() };
						nullableSources.Add(currentSource);
					}
					return nullableSources;
				}

				CodedValueSources codedValueSoruces = null;
				if (dynamicCodedValueSource.ContainsKey(key))
					codedValueSoruces = dynamicCodedValueSource[key];
				else if (dynamicCodedValueSource.ContainsKey(key.ToString()))
					codedValueSoruces = dynamicCodedValueSource[key.ToString()];

				CodedValueSource tempSource = codedValueSoruces.FirstOrDefault(x => x.Temp == true);
				if (tempSource != null)
					codedValueSoruces.Remove(tempSource);
#if SILVERLIGHT
				object value = CodedValueSources.CodedValueNameLookup(field.Name, generic, new CodedValueSources());
				if (string.IsNullOrEmpty(value as string))
					value = null;
#else
				object value = (generic as Graphic).Attributes[field.Name];
#endif
				if (value != null)
				{
					CodedValueSource source = codedValueSoruces.FirstOrDefault(x => x.Code != null && x.Code.ToString() == value.ToString());
					if (source == null)
					{
						CodedValueSource currentSource = new CodedValueSource() { Code = value, DisplayName = value.ToString(), Temp = true };
						if (field.Nullable)
							codedValueSoruces.Insert(1, currentSource);
						else
							codedValueSoruces.Insert(0, currentSource);
					}
				}
				return codedValueSoruces;
			}
			return null;
		}

		internal static string CodedValueNameLookup(string lookupField, string field, object generic, DynamicCodedValueSource dynamicCodedValueSource)
		{
			if (dynamicCodedValueSource != null && generic != null && !string.IsNullOrEmpty(field))
			{
#if SILVERLIGHT
				PropertyInfo lookupFieldProperty = generic.GetType().GetProperty(lookupField);
				if (lookupFieldProperty == null)
					return null;

				var key = lookupFieldProperty.GetValue(generic, null);
#else				
				var key = generic is Graphic ? (generic as Graphic).Attributes[lookupField] : null;
#endif
				if (key == null)
					return CodedValueSources.CodedValueNameLookup(field, generic, new CodedValueSources());				

				if (dynamicCodedValueSource.ContainsKey(key))
				{
					CodedValueSources codedValueSources = dynamicCodedValueSource[key];
					if (codedValueSources != null)
					{
						string name = CodedValueSources.CodedValueNameLookup(field, generic, codedValueSources);
						if (!string.IsNullOrEmpty(name))
							return name;
					}
				}
				else if (dynamicCodedValueSource.ContainsKey(key.ToString()))
				{
					CodedValueSources codedValueSources = dynamicCodedValueSource[key.ToString()];
					if (codedValueSources != null)
					{
						string name = CodedValueSources.CodedValueNameLookup(field, generic, codedValueSources);
						if (!string.IsNullOrEmpty(name))
							return name;
					}
				}
			}
			return null;
		}

	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* The CodedValueDomainConverter class.
	/// </summary>
	/// <remarks>Converts codes/values in the given coded value domain.</remarks>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class CodedValueDomainConverter : IValueConverter
	{
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null || parameter.ToString() == "")
				return "";

			CodedValueSources codedValueSources = parameter as CodedValueSources;
			if (codedValueSources != null)
			{
				foreach (CodedValueSource codedValueSource in codedValueSources)
				{
					if (codedValueSource.Code != null && value != null)
					{
						if (codedValueSource.Code.ToString() == value.ToString())
							return codedValueSource.DisplayName;
					}
					else if (codedValueSource.Code == null && value == null)
						return codedValueSource.DisplayName;
				}
			}

			return "";
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The CodedValueSourceConverter class.
	/// </summary>
	/// <remarks>Converts codes/values in the given coded value domain.</remarks>
	internal sealed class CodedValueSourceConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the field. The graphic attribute field that should be 
		/// used for the conversion. 
		/// </summary>
		/// <value>The field.</value>
		public string Field { get; set; }

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null || string.IsNullOrEmpty(Field) || value == null)
				return "";

#if SILVERLIGHT
			PropertyInfo fieldProperty = value.GetType().GetProperty(Field);
			if (fieldProperty == null)
				return "";

			var code = fieldProperty.GetValue(value, null);
#else			
			var code = value;
#endif
			if (code == null)
				return "";

			CodedValueSources codedValueSources = parameter as CodedValueSources;
			if (codedValueSources != null)
			{
				CodedValueSource codedValueSource = codedValueSources.FirstOrDefault(x => x.Code != null && x.Code.ToString() == code.ToString());
				if (codedValueSource != null)
					return codedValueSource.DisplayName;
			}

			return "";
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* The DynamicCodedValueSourceConverter class.
	/// </summary>
	/// <remarks>Converts codes/values in the given coded value domain based on 
	/// the current value of the lookup field.</remarks>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DynamicCodedValueSourceConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the lookup field.
		/// </summary>
		/// <value>The lookup field.</value>
		public string LookupField { get; set; }
		/// <summary>
		/// Gets or sets the field. this field is dependent on the lookup field.
		/// </summary>
		/// <value>The field.</value>
		public string Field { get; set; }

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null || parameter.ToString() == ""
				|| string.IsNullOrEmpty(LookupField) || string.IsNullOrEmpty(Field))
				return "";
#if SILVERLIGHT
			DynamicCodedValueSource dynamicCodedValueSource = parameter as DynamicCodedValueSource;
#else
			object[] array = parameter as object[];
			DynamicCodedValueSource dynamicCodedValueSource = array[0] as DynamicCodedValueSource;
			value = (array[1] as FrameworkElement).DataContext;
#endif
			string name = DynamicCodedValueSource.CodedValueNameLookup(LookupField, Field, value, dynamicCodedValueSource);

			if (!string.IsNullOrEmpty(name))
				return name;

			return "";
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The CodedValueSourceConverter class.
	/// </summary>
	/// <remarks>Converts codes/values in the given coded value domain.</remarks>
	internal sealed class CodedValueSourceLookupConverter : IValueConverter
	{
		public FrameworkElement element { get; set; }
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				return null;

			CodedValueSources codedValueSources = parameter as CodedValueSources;
			if (codedValueSources != null)
				return codedValueSources;

			return "";
		}


		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* The DynamicCodedValueSourceConverter class.
	/// </summary>
	/// <remarks>Converts codes/values in the given coded value domain based on 
	/// the current value of the lookup field.</remarks>
	/// <exclude/>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DynamicCodedValueSourceLookupConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the lookup field.
		/// </summary>
		/// <value>The lookup field.</value>
		public string LookupField { get; set; }

		/// <summary>
		/// Gets or sets the Field information
		/// </summary>
		/// <value>The Field information</value>
		public Field Field { get; set; }

		/// <summary>
		/// Gets or sets the nullable sources.
		/// </summary>
		/// <value>The nullable sources.</value>
		internal CodedValueSources NullableSources { get; set; }

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null || parameter.ToString() == "" || string.IsNullOrEmpty(LookupField))
				return value;

			DynamicCodedValueSource dynamicCodedValueSource = parameter as DynamicCodedValueSource;
			return DynamicCodedValueSource.GetCodedValueSources(LookupField, Field, value, dynamicCodedValueSource, NullableSources);						
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion
}