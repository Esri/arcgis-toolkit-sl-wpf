// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see https://opensource.org/licenses/ms-pl for details.
// All other rights reserved.

using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Toolkit.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace ESRI.ArcGIS.Client.Toolkit
{
	internal interface IKeyValue : INotifyPropertyChanged
	{
		string Key { get; }
		object Value { get; }
	}

    /// <summary>
    /// *FOR INTERNAL USE ONLY* The FeatureDataField class. Used by FeatureDataForm to create values corresponding to each graphic attribute. Beside 
    /// data validation since FeatureDataField implements INotifyPropertyChanged interface it will notify FeatureDataForm about 
    /// any change in an attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <exclude/>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class FeatureDataField<T> : IKeyValue
    {
        private FeatureDataForm _featureDataForm;
        private Field _field;
		private FeatureLayerInfo _layerInfo;
        private Type _propertyType;
        private T _propertyValue;
        private CodedValueDomain _codedValueDomain;
        private RangeDomain<DateTime> _dateRangeDomain;
        private RangeDomain<double> _doubleRangeDomain;
        private RangeDomain<float> _floatRangeDomain;
        private RangeDomain<int> _intRangeDomain;
        private RangeDomain<short> _shortRangeDomain;
        private RangeDomain<long> _longRangeDomain;
        private RangeDomain<byte> _byteRangeDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureDataField&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="featureDataForm">The feature data form.</param>
        /// <param name="layerInfo">The feature layer info.</param>
        /// <param name="field">The field.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        internal FeatureDataField(FeatureDataForm featureDataForm, FeatureLayerInfo layerInfo, Field field, Type propertyType, T propertyValue)
        {
            this._featureDataForm = featureDataForm;
			this._field = field;
			this._layerInfo = layerInfo;
            this._codedValueDomain = null;
            Domain domain = field.Domain;
            if (domain != null && !Toolkit.Utilities.FieldDomainUtils.IsDynamicDomain(_field, _layerInfo))
            {
                this._codedValueDomain = domain as CodedValueDomain;

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    this._dateRangeDomain = domain as RangeDomain<DateTime>;
                else if (propertyType == typeof(double) || propertyType == typeof(double?))
                    this._doubleRangeDomain = domain as RangeDomain<double>;
                else if (propertyType == typeof(float) || propertyType == typeof(float?))
                    this._floatRangeDomain = domain as RangeDomain<float>;
                else if (propertyType == typeof(int) || propertyType == typeof(int?))
                    this._intRangeDomain = domain as RangeDomain<int>;
                else if (propertyType == typeof(short) || propertyType == typeof(short?))
                    this._shortRangeDomain = domain as RangeDomain<short>;
                else if (propertyType == typeof(long) || propertyType == typeof(long?))
                    this._longRangeDomain = domain as RangeDomain<long>;
                else if (propertyType == typeof(byte) || propertyType == typeof(byte?))
                    this._byteRangeDomain = domain as RangeDomain<byte>;
            }
            this._propertyType = propertyType;
            this._propertyValue = propertyValue;
        }

        /// <summary>
        /// Gets the key (the attribute name).
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return this._field.Name; }
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return this._propertyValue; }
            set
            {
                string propertyName = this._field.Name;
                string typeFriendlyName = "";
                if (FeatureDataForm.Utilities.IsNotOfTypeSystemNullable(this._propertyType))
                    typeFriendlyName = this._propertyType.ToString();
                else
                    typeFriendlyName = System.Nullable.GetUnderlyingType(this._propertyType).ToString();
                
                if (this._codedValueDomain != null)
                {
                    try
                    {
                        this._propertyValue = (T)value;
                        NotifyPropertyChanged(propertyName);
                    }
                    catch
                    {
                        this._featureDataForm.IsValid = false;
                        if ((value as CodedValueSource).Code == null)
                            throw new ArgumentException(string.Format(Properties.Resources.Validation_ValueCannotBeNull, propertyName));
                        else
							throw new ArgumentException(string.Format(Properties.Resources.Validation_InvalidValue, value, typeFriendlyName, propertyName));
                    }
                }
                else
                {
                    // Checking whether the attribute is allowed to be NULL:
                    if (value == null && !this._field.Nullable)
                    {
                        this._featureDataForm.IsValid = false;
						throw new ArgumentException(string.Format(Properties.Resources.Validation_ValueCannotBeNull, propertyName));
                    }

                    if (value == null || (value != null && string.IsNullOrEmpty(value.ToString().Trim())))
                    {
                        try
                        {
                            this._propertyValue = value;
                            NotifyPropertyChanged(propertyName);
                        }
                        catch
                        {
                            this._featureDataForm.IsValid = false;
                            throw new ArgumentException(string.Format(Properties.Resources.Validation_ValueCannotBeNull, propertyName));
                        }
                    }
                    else
                    {
                        // First verify type of the value:
                        try
                        {
                            if (value != null)
                            {
                                object verifyType = null;
                                if (FeatureDataForm.Utilities.IsNotOfTypeSystemNullable(this._propertyType))
                                    verifyType = System.Convert.ChangeType(value, this._propertyType, new CultureInfo(_featureDataForm.Language.IetfLanguageTag));
                                else
									verifyType = System.Convert.ChangeType(value, System.Nullable.GetUnderlyingType(this._propertyType), new CultureInfo(_featureDataForm.Language.IetfLanguageTag));
                            }
                        }
                        catch
                        {
                            this._featureDataForm.IsValid = false;
							throw new ArgumentException(string.Format(Properties.Resources.Validation_InvalidType, propertyName, typeFriendlyName));
                        }

                        // Type of the value is valid; first check for a range domain and verify the value over its min and max:
                        try
                        {
							if(Toolkit.Utilities.FieldDomainUtils.IsDynamicDomain(_field, _layerInfo))
							{
								if (_featureDataForm._attributeFrameworkElements != null && _layerInfo != null && _featureDataForm._attributeFrameworkElements.ContainsKey(_layerInfo.TypeIdField))
								{
									var selectedItem = ((ComboBox)_featureDataForm._attributeFrameworkElements[_layerInfo.TypeIdField]).SelectedItem;
									if (selectedItem != null)
									{
										object code = ((CodedValueSource)selectedItem).Code;
										switch (_field.Type)
										{
											case ESRI.ArcGIS.Client.Field.FieldType.Double:
												var dynamicRangeDoubleDomains = FieldDomainUtils.BuildDynamicRangeDomain<double>(_field, _layerInfo);
												if (dynamicRangeDoubleDomains != null && code != null)
												{
													var domains = dynamicRangeDoubleDomains.Where(kvp => kvp.Key.Equals(code));
													if (domains != null && domains.Any())
														this._doubleRangeDomain = domains.First().Value;
												}
												break;
											case ESRI.ArcGIS.Client.Field.FieldType.Integer:
												var dynamicRangeIntegerDomains = FieldDomainUtils.BuildDynamicRangeDomain<int>(_field, _layerInfo);
												if (dynamicRangeIntegerDomains != null && code != null)
												{
													var domains = dynamicRangeIntegerDomains.Where(kvp => kvp.Key.Equals(code));
													if (domains != null && domains.Any())
														this._intRangeDomain = domains.First().Value;
												}
												break;
											case ESRI.ArcGIS.Client.Field.FieldType.Single:
												var dynamicRangeSingleDomains = FieldDomainUtils.BuildDynamicRangeDomain<float>(_field, _layerInfo);
												if (dynamicRangeSingleDomains != null && code != null)
												{
													var domains = dynamicRangeSingleDomains.Where(kvp => kvp.Key.Equals(code));
													if (domains != null && domains.Any())
														this._floatRangeDomain = domains.First().Value;
												}
												break;
											case ESRI.ArcGIS.Client.Field.FieldType.SmallInteger:
												var dynamicRangeShortDomains = FieldDomainUtils.BuildDynamicRangeDomain<short>(_field, _layerInfo);
												if (dynamicRangeShortDomains != null && code != null)
												{
													var domains = dynamicRangeShortDomains.Where(kvp => kvp.Key.Equals(code));
													if (domains != null && domains.Any())
														this._shortRangeDomain = domains.First().Value;
												}
												break;
											case ESRI.ArcGIS.Client.Field.FieldType.Date:
												var dynamicRangeDateDomains = FieldDomainUtils.BuildDynamicRangeDomain<DateTime>(_field, _layerInfo);
												if (dynamicRangeDateDomains != null && code != null)
												{
													var domains = dynamicRangeDateDomains.Where(kvp => kvp.Key.Equals(code));
													if (domains != null && domains.Any())
														this._dateRangeDomain = domains.First().Value;
												}
												break;
										}
									}
								}
							}
												
                            if (this._dateRangeDomain != null)
                            {
                                DateTime parsedValue = DateTime.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._dateRangeDomain, parsedValue);
                            }
                            else if (this._doubleRangeDomain != null)
                            {
                                double parsedValue = double.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._doubleRangeDomain, parsedValue);
                            }
                            else if (this._floatRangeDomain != null)
                            {
                                float parsedValue = float.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._floatRangeDomain, parsedValue);
                            }
                            else if (this._intRangeDomain != null)
                            {
                                int parsedValue = int.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._intRangeDomain, parsedValue);
                            }
                            else if (this._shortRangeDomain != null)
                            {
                                short parsedValue = short.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._shortRangeDomain, parsedValue);
                            }
                            else if (this._longRangeDomain != null)
                            {
                                long parsedValue = long.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._longRangeDomain, parsedValue);
                            }
                            else if (this._byteRangeDomain != null)
                            {
                                byte parsedValue = byte.Parse(value.ToString());
                                FeatureDataForm.Utilities.IsValidRange(this._byteRangeDomain, parsedValue);
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            this._featureDataForm.IsValid = false;
							_featureDataForm.UpdateDictionary(ref _featureDataForm._attributeValidationStatus, propertyName, false);						
									throw new ArgumentException(ex.Message);
                        }

                        this._propertyValue = value;
                        NotifyPropertyChanged(propertyName);

                        this._featureDataForm.IsValid = true;
                    }
                }
            }
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

		object IKeyValue.Value
		{
			get { return this.Value; }
		}
	}
}