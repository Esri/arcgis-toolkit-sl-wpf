// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections;
using System.ComponentModel;
using ESRI.ArcGIS.Client.Toolkit.Utilities;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using ESRI.ArcGIS.Client.Toolkit.ValueConverters;
#if NET35
using Microsoft.Windows.Controls;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// The FeatureDataForm Control. Provides the ability to view/modify graphic attributes in Graphics/Feature layers.
	/// </summary>
	[TemplatePart(Name = "ContentPresenter", Type = typeof(ContentPresenter))]
	[TemplatePart(Name = "CommitButton", Type = typeof(ButtonBase))]
	[TemplatePart(Name = "DeleteButton", Type = typeof(ButtonBase))]
	[StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(ContentControl))]
	[StyleTypedProperty(Property = "TextBoxStyle", StyleTargetType = typeof(TextBox))]
	[StyleTypedProperty(Property = "ComboBoxStyle", StyleTargetType = typeof(ComboBox))]
	[StyleTypedProperty(Property = "DateTimePickerStyle", StyleTargetType = typeof(DateTimePicker))]
	public sealed class FeatureDataForm : Control, INotifyPropertyChanged
	{
		#region Private Fields
		private const int SIZEOF_SPACING_COLUMN = 8;                            // Constant for width of spacing columns in FeatureDataForm layout
		private const string DEFAULT_DATETIME_FORMAT = "G";						// The default date time format that will be used if user does not supply one.
		private const DateTimeKind DEFAULT_DATETIME_KIND = DateTimeKind.Local;	// The default date time kind that will be used if the user does not supply one.				
		private Thickness PANEL_MARGIN = new Thickness(15, 10, 15, 10);         // Used to set man panel of the FeatureDataForm corner margins
		private Thickness CONTROLS_MARGIN = new Thickness(0, 2.5, 0, 2.5);      // Used to set each field control margins
		private ContentPresenter _contentPresenter = null;                      // ContentPresenter object in FeatureDataForm
		private ButtonBase _commitButton = null;                                // Variable to hold the Commit button
		private ButtonBase _deleteButton = null;								// Variable to hold the Delete button
		private Dictionary<string, CodedValueDomain> _codedValueDomains = null; // Dictionary of all coded value domains with field names as keys
		internal Dictionary<string, Control> _attributeFrameworkElements = null; // Dictionary to hold appropriate controls for each field keyed by field names
		private bool _isUpdatedByCommitButton = false;                          // Indicates whether changes applied by pressing FeatureDataForm's Commit button
		internal Dictionary<string, bool> _attributeValidationStatus = null;    // Dictionary of each field with its validation status
		private bool _isBindingData = false;                                    // Variable indicating that FeatureDataForm is binding the data
		private bool _isVerifyingAfterLosingFocus = false;                      // Indicates the verification is taking place after a control loses its focus
		private bool _isValid;                                                  // field for IsValid property
		private bool _hasEdits;                                                 // field for HasEdits property
		private string _typeIdField = null;                                     // The TypeId field
		private IDictionary<object, FeatureType> _featureTypes = null;          // Feature types defined in the layer
		private Dictionary<string, IKeyValue> _dataFields = null;               // Collection of FeatureDataFields
		private int currentTabOrder = 0;										// Maintains field controls tab orders		
		private bool isFocusedInternally = false;								// Control is focused through the code vs. user
		private bool hasEditableControl = false;								// Flag to indicate whether form contains editable control.
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FeatureDataForm"/> class.
		/// </summary>
		public FeatureDataForm()
		{

#if SILVERLIGHT
			DefaultStyleKey = typeof(FeatureDataForm);
			Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.Name);
#endif
		}

		static FeatureDataForm()
		{
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FeatureDataForm), 
                    new FrameworkPropertyMetadata(typeof(FeatureDataForm)));
#endif
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether last update operation in this instance was valid.
		/// </summary>
		/// <value><c>true</c> if the last update operation was valid; otherwise, <c>false</c>.</value>
		public bool IsValid
		{
			get { return _isValid; }
			internal set
			{
				if (value != _isValid)
				{
					_isValid = value;
					OnPropertyChanged("IsValid");
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the instance of the FeatureDataForm has unapplied edit(s).
		/// </summary>
		/// <value><c>true</c> if the instance of the FeatureDataForm has unapplied edit(s); otherwise, <c>false</c>.</value>
		public bool HasEdits
		{
			get { return _hasEdits; }
			internal set
			{
				if (value != _hasEdits)
				{
					_hasEdits = value;
					OnPropertyChanged("HasEdits");
				}
			}
		}

		/// <summary>
		/// Gets or sets the date time format to use in <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.DateTimePicker"/> control.
		/// </summary>		
		public string DateTimeFormat
		{
			get { return (string)GetValue(DateTimeFormatProperty); }
			set { SetValue(DateTimeFormatProperty, value); }
		}

		/// <summary>
		/// The dependency property for <see cref="FeatureDataForm.DateTimeFormat"/> property.
		/// </summary>
		public static readonly DependencyProperty DateTimeFormatProperty =
			DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(FeatureDataForm), new PropertyMetadata(DEFAULT_DATETIME_FORMAT, OnDateTimeFormatPropertyChanged));

		private static void OnDateTimeFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the DateTimeKind used in <see cref="ESRI.ArcGIS.Client.Toolkit.Primitives.DateTimePicker"/> control.
		/// </summary>		
		public DateTimeKind DateTimeKind
		{
			get { return (DateTimeKind)GetValue(DateTimeKindProperty); }
			set { SetValue(DateTimeKindProperty, value); }
		}

		/// <summary>
		/// The dependency property for <see cref="FeatureDataForm.DateTimeKind"/> property.
		/// </summary>
		public static readonly DependencyProperty DateTimeKindProperty =
			DependencyProperty.Register("DateTimeKind", typeof(DateTimeKind), typeof(FeatureDataForm), new PropertyMetadata(DEFAULT_DATETIME_KIND, OnDateTimeKindPropertyChanged));

		private static void OnDateTimeKindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}
			
		#endregion

		#region Attached Properties
		private static readonly DependencyProperty AssociatedFieldNameProperty =
			DependencyProperty.RegisterAttached("AssociatedFieldName", typeof(string), typeof(FeatureDataForm), null);
		#endregion

		#region Dependency Properties
		/// <summary>
		/// Gets or sets the feature layer.
		/// </summary>
		/// <value>The feature layer.</value>
		public FeatureLayer FeatureLayer
		{
			get { return (FeatureLayer)GetValue(FeatureLayerProperty); }
			set { SetValue(FeatureLayerProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="FeatureLayer"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty FeatureLayerProperty =
			DependencyProperty.Register("FeatureLayer", typeof(FeatureLayer), typeof(FeatureDataForm), new PropertyMetadata(null, OnFeatureLayerPropertyChanged));
		private static void OnFeatureLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			FeatureLayer oldValue = e.OldValue as FeatureLayer;
			FeatureLayer newValue = e.NewValue as FeatureLayer;
			if (oldValue != null)
				oldValue.UpdateCompleted -= dataForm.FeatureLayer_UpdateCompleted;
			if (newValue != null)
			{
				if (newValue.IsInitialized)
				{					
					dataForm.GenerateUI(true);
				}
				newValue.UpdateCompleted += dataForm.FeatureLayer_UpdateCompleted;
			}
		}		

		/// <summary>
		/// Gets or sets the GraphicSource. This is the data source used by the FeatureDataForm.
		/// </summary>
		/// <value>The graphic source.</value>
		public Graphic GraphicSource
		{
			get { return (Graphic)GetValue(GraphicSourceProperty); }
			set { SetValue(GraphicSourceProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="GraphicSource"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GraphicSourceProperty =
			DependencyProperty.Register("GraphicSource", typeof(Graphic), typeof(FeatureDataForm), new PropertyMetadata(null, OnGraphicSourcePropertyChanged));
		private static void OnGraphicSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			Graphic oldValue = e.OldValue as Graphic;
			Graphic newValue = e.NewValue as Graphic;
			if (oldValue != null)
				oldValue.AttributeValueChanged -= dataForm.Graphic_AttributeValueChanged;
			if (newValue != null)
				newValue.AttributeValueChanged += dataForm.Graphic_AttributeValueChanged;

			dataForm.GenerateUI(true);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="IsReadOnly"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsReadOnlyProperty =
			DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(FeatureDataForm), new PropertyMetadata(false, OnIsReadOnlyPropertyChanged));
		private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(true);
		}		

		private bool CanUserEdit
		{
			get 
			{
				if (this.GraphicSource != null)
				{						
					if (this.FeatureLayer != null)
					{
						return FeatureLayer.IsUpdateAllowed(GraphicSource);						
					}											
				}
				return false;
			}			
		}

		/// <summary>
		/// Gets or sets the content of the commit button.
		/// </summary>
		/// <value>The content of the commit button.</value>
		public object CommitButtonContent
		{
			get { return (object)GetValue(CommitButtonContentProperty); }
			set { SetValue(CommitButtonContentProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="CommitButtonContent"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CommitButtonContentProperty =
			DependencyProperty.Register("CommitButtonContent", typeof(object), typeof(FeatureDataForm), new PropertyMetadata(Properties.Resources.FeatureDataForm_CommitButtonContent));		

		/// <summary>
		/// Gets or sets the content of the delete button.
		/// </summary>
		/// <value>The content of the delete button.</value>
		public object DeleteButtonContent
		{
			get { return (object)GetValue(DeleteButtonContentProperty); }
			set { SetValue(DeleteButtonContentProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="DeleteButtonContent"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty DeleteButtonContentProperty =
			DependencyProperty.Register("DeleteButtonContent", typeof(object), typeof(FeatureDataForm), new PropertyMetadata(Properties.Resources.FeatureDataForm_DeleteButtonContent));				

		/// <summary>
		/// Gets or sets the label positioning in FeatureDataForm.
		/// </summary>
		/// <value>The label position: Left or Top</value>
		public FeatureDataFormLabelPosition LabelPosition
		{
			get { return (FeatureDataFormLabelPosition)GetValue(LabelPositionProperty); }
			set { SetValue(LabelPositionProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="LabelPosition"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LabelPositionProperty =
			DependencyProperty.Register("LabelPosition", typeof(FeatureDataFormLabelPosition), typeof(FeatureDataForm), new PropertyMetadata(OnLabelPositionPropertyChanged));
		private static void OnLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the style used for the commit button.
		/// </summary>
		/// <value>The commit button style.</value>
		public Style CommitButtonStyle
		{
			get { return (Style)GetValue(CommitButtonStyleProperty); }
			set { SetValue(CommitButtonStyleProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="CommitButtonStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CommitButtonStyleProperty =
			DependencyProperty.Register("CommitButtonStyle", typeof(Style), typeof(FeatureDataForm), null);		

		/// <summary>
		/// Gets or sets the style used for the delete button.
		/// </summary>
		/// <value>The delete button style.</value>
		public Style DeleteButtonStyle
		{
			get { return (Style)GetValue(DeleteButtonStyleProperty); }
			set { SetValue(DeleteButtonStyleProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="DeleteButtonStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty DeleteButtonStyleProperty =
			DependencyProperty.Register("DeleteButtonStyle", typeof(Style), typeof(FeatureDataForm),null);		
		

		/// <summary>
		/// Gets or sets the style used for all TextBlock controls showing field names/aliases.
		/// </summary>
		/// <value>The label style.</value>
		public Style LabelStyle
		{
			get { return (Style)GetValue(LabelStyleProperty); }
			set { SetValue(LabelStyleProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="LabelStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LabelStyleProperty =
			DependencyProperty.Register("LabelStyle", typeof(Style), typeof(FeatureDataForm), new PropertyMetadata(OnLabelStylePropertyChanged));
		private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the style used for all TextBox controls showing field values.
		/// </summary>
		/// <value>The text box style.</value>
		public Style TextBoxStyle
		{
			get { return (Style)GetValue(TextBoxStyleProperty); }
			set { SetValue(TextBoxStyleProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="TextBoxStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TextBoxStyleProperty =
			DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(FeatureDataForm), new PropertyMetadata(OnTextBoxStylePropertyChanged));
		private static void OnTextBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the style used for all ComboBox controls showing field values.
		/// </summary>
		/// <value>The combo box style.</value>
		public Style ComboBoxStyle
		{
			get { return (Style)GetValue(ComboBoxStyleProperty); }
			set { SetValue(ComboBoxStyleProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="ComboBoxStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ComboBoxStyleProperty =
			DependencyProperty.Register("ComboBoxStyle", typeof(Style), typeof(FeatureDataForm), new PropertyMetadata(OnComboBoxStylePropertyChanged));
		private static void OnComboBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the style used for all DatePicker (calendar) controls showing field values.
		/// </summary>
		/// <value>The DatePicker style.</value>
		/// <remarks>This property has been marked obsolete because it was superseded by DateTimePickerStyle. </remarks>
		[Obsolete("The property has been replaced with a new property DateTimePickerStyle")]
		public Style DatePickerStyle
		{
#pragma warning disable 0618
			get { return (Style)GetValue(DatePickerStyleProperty); }
			set { SetValue(DatePickerStyleProperty, value); }
#pragma warning restore 0618
		}

		/// <summary>
		/// Identifies the <see cref="DatePickerStyle"/> dependency property.
		/// </summary>
		[Obsolete("The property has been replaced with a new property DateTimePickerStyleProperty")]
		public static readonly DependencyProperty DatePickerStyleProperty =
			DependencyProperty.Register("DatePickerStyle", typeof(Style), typeof(FeatureDataForm), new PropertyMetadata(OnDatePickerStylePropertyChanged));
		private static void OnDatePickerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		/// <summary>
		/// Gets or sets the style used for all DateTimePicker (calendar) controls showing field values.
		/// </summary>
		/// <value>The DateTimePicker style.</value>
		public Style DateTimePickerStyle
		{
			get { return (Style)GetValue(DateTimePickerStyleProperty); }
			set { SetValue(DateTimePickerStyleProperty, value); }
		}
		/// <summary>
		/// Identifies the <see cref="DateTimePickerStyle"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty DateTimePickerStyleProperty =
			DependencyProperty.Register("DateTimePickerStyle", typeof(Style), typeof(FeatureDataForm), new PropertyMetadata(OnDateTimePickerStylePropertyChanged));

		private static void OnDateTimePickerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataForm dataForm = d as FeatureDataForm;
			dataForm.GenerateUI(false);
		}

		#endregion

		#region Overridden Methods
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes 
		/// (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this._contentPresenter != null)
				this._contentPresenter.Content = null;
			this._contentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;

			if (this._commitButton != null)
				this._commitButton.Click -= CommitButton_Click;
			this._commitButton = GetTemplateChild("CommitButton") as ButtonBase;
			if (this._commitButton != null)							
				this._commitButton.Click += CommitButton_Click;			

			if (this._deleteButton != null)
				this._deleteButton.Click -= DeleteButton_Click;
			this._deleteButton = GetTemplateChild("DeleteButton") as ButtonBase;
			if (this._deleteButton != null)			
				this._deleteButton.Click += DeleteButton_Click;			
			this.GenerateUI(true);
		}
		#endregion

		#region Private Methods
		
		private void FeatureLayer_UpdateCompleted(object sender, EventArgs e)
		{			
			GenerateUI(true);
		}

		private bool CheckGraphicParent()
		{
			if (this.FeatureLayer != null && this.GraphicSource != null)
				return this.FeatureLayer.Graphics.Contains(this.GraphicSource);

			return false;
		}

		private void GenerateUI(bool resetInternalVariables)
		{
			if (resetInternalVariables)
			{
				this._codedValueDomains = null;
				this._isVerifyingAfterLosingFocus = false;
				this._attributeFrameworkElements = null;
				this._isUpdatedByCommitButton = false;
				this._attributeValidationStatus = null;
				this._isBindingData = false;
				IsValid = true;
				HasEdits = false;
				this._typeIdField = null;
				this._featureTypes = null;
				this._dataFields = null;
				this.hasEditableControl = false;
				if (!this.IsReadOnly && this.CanUserEdit)
				{
					if(this._commitButton != null)
						this._commitButton.IsEnabled = true;
					if (this._deleteButton != null)
						this._deleteButton.IsEnabled = false;
				}
				
			}

			if (this._deleteButton != null)
			{
				bool enabled = false;
				if (this.FeatureLayer != null)
				{
					if (this.FeatureLayer.Graphics.Contains(this.GraphicSource) && this.FeatureLayer.IsDeleteAllowed(this.GraphicSource))
						enabled = true;
				}
				this._deleteButton.IsEnabled = enabled;
			}

			if (this.FeatureLayer == null || this.GraphicSource == null ||
				(this.FeatureLayer != null && this.GraphicSource != null && !CheckGraphicParent()))
			{
				if (this._contentPresenter != null)
				{
					this._contentPresenter.DataContext = null;
					this._contentPresenter.Content = null;
				}
				UpdateStates();
				return;
			}

			this._typeIdField = this.FeatureLayer.LayerInfo.TypeIdField;
			this._featureTypes = this.FeatureLayer.LayerInfo.FeatureTypes;
			if ( this._featureTypes == null || this._featureTypes.Count == 0)
			{
				if (this.FeatureLayer.Renderer is UniqueValueRenderer || this.FeatureLayer.Renderer is UniqueValueMultipleFieldsRenderer)
				{
					var uvr = this.FeatureLayer.Renderer as UniqueValueRenderer;
					if (string.IsNullOrEmpty(_typeIdField))
						_typeIdField = uvr.Field;
					var types = new Dictionary<object, FeatureType>();
					if (uvr != null)
					{
						foreach (var info in uvr.Infos)
						{
							var featureType = new FeatureType(info.Value, info.Label, null, null);
							types[info.Value] = featureType;
						}
					}
					else
					{
						var uvmfr = this.FeatureLayer.Renderer as UniqueValueMultipleFieldsRenderer;
						if (string.IsNullOrEmpty(_typeIdField))
							_typeIdField = string.Join(uvmfr.FieldDelimiter, uvmfr.Fields);
						foreach (var info in uvmfr.Infos)
						{							
							var featureType = new FeatureType(info.Values, info.Label, null, null);
							types[info.Values] = featureType;
						}
					}
					_featureTypes = types;

				}
			}
			AutoGenerateColumns();
			UpdateStates();
		}

		private void UpdateDataForm(string key, object newValue)
		{
			if (this._contentPresenter != null && this._contentPresenter.Content != null)
			{
				if (this._attributeFrameworkElements.ContainsKey(key))
				{
					Control fieldControl = this._attributeFrameworkElements[key];
					if (fieldControl is ComboBox)
					{
						int newIndex = -1;
						if (newValue != null)
						{
							ComboBox comboBox = fieldControl as ComboBox;
							foreach (CodedValueSource codedValueSource in comboBox.ItemsSource)
							{
								newIndex++;
								Type type = Utilities.GetFieldType(key, this.FeatureLayer.LayerInfo.Fields);
								if (Utilities.AreEqual(codedValueSource.Code, newValue, type, new CultureInfo(Language.IetfLanguageTag)))
									break;
							}
						}
						(fieldControl as ComboBox).SelectionChanged -= FrameworkElement_PropertyChanged;
						(fieldControl as ComboBox).SetValue(ComboBox.SelectedIndexProperty, newIndex);

						if (key == _typeIdField)
						{
							// When TypeID value changes need to update all non inherited domain types
							// becasue the coded values from the non inherited domains changed based on the TypeID value.
							// All domain types will show up in LayerInfo.FeatureTypes, but only inherited types will not have
							// a Domain value for LayerInfo.Fields[FieldName].Domain.
							object code = newValue;
							if (newValue != null && _featureTypes.ContainsKey(code) && _featureTypes[code].Domains != null)
							{
								newValue = _featureTypes[code].Name;
								foreach (string fieldName in _featureTypes[code].Domains.Keys)
								{
									bool found = false;
									foreach (Field field in this.FeatureLayer.LayerInfo.Fields)
									{
										if (field.Name == fieldName && field.Domain != null)
										{
											found = true; // inherited domain found, no update needed.
											break;
										}
									}
									if (!found) // Refresh the dynamic domain control
									{
										RepopulateDomains(fieldControl);
										object value = (this.GraphicSource.Attributes.ContainsKey(fieldName)) ? this.GraphicSource.Attributes[fieldName] : null;
										UpdateDataForm(fieldName, value);
										break;
									}
								}
							}
						}

						// In SILVERLIGHT the ...Changed events won't raise right after setting SelectedIndex, SelectedDate, 
						// IsChecked, and Text properties in ComboBox, DateTimePicker and TextBox controls respectively. 
						// We need to hook up detached events after making sure that the above properties have been changed first:
#if SILVERLIGHT
						Dispatcher.BeginInvoke((Action)delegate()
						{
#endif							
							(fieldControl as ComboBox).SelectionChanged += FrameworkElement_PropertyChanged;
#if SILVERLIGHT
						});
#endif
					}
					else if (fieldControl is DateTimePicker)
					{
						(fieldControl as DateTimePicker).SelectedDateChanged -= FrameworkElement_PropertyChanged;
						(fieldControl as DateTimePicker).SetValue(DateTimePicker.SelectedDateProperty, newValue);
#if SILVERLIGHT
						Dispatcher.BeginInvoke((Action)delegate()
						{
#endif
							(fieldControl as DateTimePicker).SelectedDateChanged += FrameworkElement_PropertyChanged;
#if SILVERLIGHT
						});
#endif
					}
					else
					{
						(fieldControl as TextBox).TextChanged -= FrameworkElement_PropertyChanged;
						object typeIdFieldValue = (this._typeIdField != null && this.GraphicSource.Attributes.ContainsKey(this._typeIdField)) ? this.GraphicSource.Attributes[this._typeIdField] : null;
						Domain fieldDomain = GetDomain(key, typeIdFieldValue);
						if (key == _typeIdField)
						{
							// When TypeID value changes need to update all non inherited domain types
							// because the coded values from the non inherited domains changed based on the TypeID value.
							// All domain types will show up in LayerInfo.FeatureTypes, but only inherited types will not have
 							// a Domain value for LayerInfo.Fields[FieldName].Domain.
							object code = newValue;
							if (newValue != null && _featureTypes.ContainsKey(code))
							{
								newValue = _featureTypes[code].Name;
								foreach (string fieldName in _featureTypes[code].Domains.Keys)
								{
									bool found = false;
									foreach (Field field in this.FeatureLayer.LayerInfo.Fields)
									{
										if (field.Name == fieldName && field.Domain != null)
										{
											found = true; // inherited domain found, no update needed.
											break;
										}
									}
									if (!found) // Refresh the dynamic domain control
									{
										object value = (this.GraphicSource.Attributes.ContainsKey(fieldName)) ? this.GraphicSource.Attributes[fieldName] : null;
										UpdateDataForm(fieldName,value);
									}
								}
							}
						}
						else if (fieldDomain is CodedValueDomain)
						{
							if (newValue != null && ((CodedValueDomain)fieldDomain).CodedValues.ContainsKey(newValue))
								newValue = ((CodedValueDomain)fieldDomain).CodedValues[newValue];							
						}
						(fieldControl as TextBox).SetValue(TextBox.TextProperty, newValue == null ? "" : newValue.ToString());

#if SILVERLIGHT
						Dispatcher.BeginInvoke((Action)delegate()
						{
#endif
							(fieldControl as TextBox).TextChanged += FrameworkElement_PropertyChanged;
#if SILVERLIGHT
						});
#endif
					}

				}
				UpdateStates();
			}
		}

		private void Graphic_AttributeValueChanged(object sender, ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs e)
		{
			if ((IsEditFieldsInfo(e.Key) || !this._isUpdatedByCommitButton) &&
				e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				UpdateDataForm(e.Key, e.NewValue);
				if (FeatureLayer != null && FeatureLayer.LayerInfo != null)
				{
					string key = e.Key;
					if (FeatureLayer.LayerInfo.Fields != null)
					{
						Field field = FeatureLayer.LayerInfo.Fields.First(x => x.FieldName == key);
						if (FieldDomainUtils.IsDynamicDomain(field, FeatureLayer.LayerInfo))
							key = FeatureLayer.LayerInfo.TypeIdField;
					}
					if (FeatureLayer.LayerInfo.TypeIdField == key)
					{
						Control control = (_attributeFrameworkElements != null
							&& _attributeFrameworkElements.ContainsKey(key)) ? _attributeFrameworkElements[key] : null;
						RegenerateLayout(control, null);
					}					
				}
			}
		}

		#region Graphic update as a result of the INotifyPropertyChanged event invocation
		private void FeatureDataField_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (this._isVerifyingAfterLosingFocus)
				return;
		
			IKeyValue newValue = sender as IKeyValue;
			object valueInGraphics = this.GraphicSource.Attributes[newValue.Key];
			object value = newValue.Value;
			bool hasChange = (valueInGraphics == null && value != null) ||
							 (valueInGraphics != null && value == null) ||
							 (valueInGraphics != null && value != null && !valueInGraphics.Equals(value));
			if (hasChange)
				this.GraphicSource.Attributes[newValue.Key] = value;
		}
		#endregion

		private void FrameworkElement_PropertyChanged(object sender, EventArgs e)
		{

			string associatedField = (sender as FrameworkElement).GetValue(AssociatedFieldNameProperty) as string;


			if (this._isBindingData && associatedField != _typeIdField)
				return;

			if (associatedField != null && !string.IsNullOrEmpty(associatedField.ToString()))
			{
				if (FeatureLayer != null)
				{
					bool isChangeValid = true;
					if (sender is TextBox)
					{
						var value = (sender as TextBox).Text;
						Type type = Utilities.GetFieldType(associatedField, FeatureLayer.LayerInfo.Fields);
						try
						{
							Utilities.ConvertToType(value, type, new CultureInfo(Language.IetfLanguageTag));
						}
						catch
						{
							isChangeValid = false;
						}
					}
					UpdateDictionary(ref this._attributeValidationStatus, associatedField, isChangeValid);
					UpdateStates();
				}								
			}
			// If TypeId field is a CodedValueDomain re-generate control layout as the selection (subtype) has changed:
			if (sender is ComboBox && associatedField != null && !string.IsNullOrEmpty(associatedField) && associatedField == this._typeIdField)
				RegenerateLayout(sender as Control, sender as Control);
		}

		private Domain GetDomain(string fieldName, object typeIdFieldValue)
		{
			if (this._typeIdField != null && typeIdFieldValue != null && this._featureTypes != null &&
				this.GraphicSource.Attributes.ContainsKey(this._typeIdField))
			{
				if (this._featureTypes.ContainsKey(typeIdFieldValue))
				{
					FeatureType featureType = this._featureTypes[typeIdFieldValue];
					if (featureType != null && featureType.Domains != null &&
						featureType.Domains.ContainsKey(fieldName))
						return featureType.Domains[fieldName];
				}
			}

			return null;
		}

		private Dictionary<object, string> GetTypeIDCodedValueDomain()
		{
			Dictionary<object, string> typeIDValues = null;
			foreach (KeyValuePair<object, FeatureType> type in this._featureTypes)
			{
				if (typeIDValues == null)
					typeIDValues = new Dictionary<object, string>();

				object code = type.Key;
				string name = "";
				if (type.Value != null && type.Value.Name != null)
					name = type.Value.Name;
				typeIDValues.Add(code, name);
			}
			return typeIDValues;
		}

		private ComboBox GetTypeIDFieldControl(bool isNullable)
		{
			CodedValueSources codedValueSources = null;
			foreach (KeyValuePair<object, FeatureType> type in this._featureTypes)
			{
				if (codedValueSources == null)
				{
					codedValueSources = new CodedValueSources();
					if (isNullable)
					{
						CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };
						codedValueSources.Add(nullableSource);
					}
				}
				object code = type.Key;
				string name = "";
				if (type.Value != null && type.Value.Name != null)
					name = type.Value.Name;
				CodedValueSource codedValueSource = new CodedValueSource() { Code = code, DisplayName = name };
				codedValueSources.Add(codedValueSource);
			}
			ComboBox comboBox = new ComboBox();
			comboBox.VerticalAlignment = VerticalAlignment.Center;
			comboBox.Margin = CONTROLS_MARGIN;
			// Set ItemsSource:
			comboBox.ItemsSource = codedValueSources;
			comboBox.DisplayMemberPath = "DisplayName";

			// Applying Style if any:
			if (this.ComboBoxStyle != null)
				comboBox.Style = this.ComboBoxStyle;

			// Hookup the Loaded event of the ComboBox control:
			comboBox.Loaded += (sender, e) =>
			{
				(sender as ComboBox).SelectionChanged += FrameworkElement_PropertyChanged;
			};

			return comboBox;
		}

		private ComboBox GetCodedValueDomainFieldControl(CodedValueDomain codedValueDomain, Field field, object value)
		{
			CodedValueSources codedValueSources = new CodedValueSources();
			bool Found = false;
			if (codedValueDomain != null)
			{
				foreach (KeyValuePair<object, string> codeVal in codedValueDomain.CodedValues)
				{
					if (codedValueSources == null)
					{					
						codedValueSources = new CodedValueSources();					
					}
					if (value != null && codeVal.Key != null && codeVal.Key.ToString() == value.ToString())
						Found = true;
					CodedValueSource codedValueSource = new CodedValueSource() { Code = codeVal.Key, DisplayName = codeVal.Value == null ? "" : codeVal.Value };
					codedValueSources.Add(codedValueSource);
				}
			}			
			if (!Found && value != null)
			{
				CodedValueSource currentSource = new CodedValueSource() { Code = value, DisplayName = value.ToString(), Temp = true };
				codedValueSources.Insert(0,currentSource);
			}
			if (field.Nullable)
			{
				CodedValueSource nullableSource = new CodedValueSource() { Code = null, DisplayName = " " };
				codedValueSources.Insert(0,nullableSource);
			}								
			ComboBox comboBox = new ComboBox();
			comboBox.VerticalAlignment = VerticalAlignment.Center;
			comboBox.Margin = CONTROLS_MARGIN;
			// Set ItemsSource:
			comboBox.ItemsSource = codedValueSources;
			comboBox.DisplayMemberPath = "DisplayName";

			// Applying Style if any:
			if (this.ComboBoxStyle != null)
				comboBox.Style = this.ComboBoxStyle;			

			// Hookup the Loaded event of the ComboBox control:
			comboBox.Loaded += (sender, e) =>
			{
				(sender as ComboBox).SelectionChanged += FrameworkElement_PropertyChanged;
			};

			return comboBox;
		}
		private DateTimePicker GetDateFieldControl()
		{
			DateTimePicker dtp = new DateTimePicker();
			dtp.DateTimeFormat = this.DateTimeFormat;
			dtp.DateTimeKind = this.DateTimeKind;
			dtp.VerticalAlignment = VerticalAlignment.Center;
			dtp.Margin = CONTROLS_MARGIN;
			dtp.Language = this.Language;

			// Applying Style if any:
			if (this.DateTimePickerStyle != null)
				dtp.Style = this.DateTimePickerStyle;

			// Hookup the Loaded event of the DatePicker control:
			dtp.Loaded += (sender, e) =>
			{
				(sender as DateTimePicker).SelectedDateChanged += FrameworkElement_PropertyChanged;
			};

			return dtp;
		}
		private TextBox GetTextFieldControl()
		{
			TextBox textBox = new TextBox();
#if SILVERLIGHT
            textBox.Language = this.Language;
#endif
			textBox.VerticalAlignment = VerticalAlignment.Center;
			textBox.Margin = CONTROLS_MARGIN;

			// Applying Style if any:
			if (this.TextBoxStyle != null)
				textBox.Style = this.TextBoxStyle;

			// Hookup the Loaded event of the TextBox control:
			textBox.Loaded += (sender, e) =>
			{
				(sender as TextBox).TextChanged += FrameworkElement_PropertyChanged;
			};

			return textBox;
		}
		private int GetCodedValueIndex(CodedValueDomain codedValueDomain, object value)
		{
			int idx = -1;								
			if (codedValueDomain == null)
				return -1;
			var culture = new CultureInfo(Language.IetfLanguageTag);
			foreach (KeyValuePair<object, string> codeVal in codedValueDomain.CodedValues)
			{
				idx++;
				if (Utilities.AreEqual(codeVal.Key, value, codeVal.Key.GetType(), culture))
					return idx;
			}						

			return -1;
		}
		private Control GetControlFromType(FeatureLayerInfo layerInfo, Field field, BindingMode bindingMode, Domain fieldDomain)
		{
			string fieldName = field.Name;
			object fieldValue = this.GraphicSource.Attributes[fieldName];
			if (_attributeFrameworkElements.ContainsKey(field.Name))
			{
				Control previousControl = this._attributeFrameworkElements[field.Name];
				if (previousControl != null && previousControl is ComboBox && field.Domain == null)
				{
					var cvs = (previousControl as ComboBox).SelectedItem as CodedValueSource;
					fieldValue = (cvs != null) ? cvs.Code : null;						
				}
			}	 

			Control control = null;
			CodedValueDomain codedValueDomain = null;
			CodedValueSource codedValueSource = null;
			bool ReadOnlyMode = (this.IsReadOnly || !field.Editable || !this.CanUserEdit);
			if (ReadOnlyMode)
			{	// TextBox controls for all viewable fields if FeatureDataForm is in read-only mode				
				if (field.Name == _typeIdField)
				{
					if (fieldValue != null && _featureTypes.ContainsKey(fieldValue))
						fieldValue = _featureTypes[fieldValue].Name;
				}
				else if (fieldDomain is CodedValueDomain)
				{
					if (fieldValue != null && ((CodedValueDomain)fieldDomain).CodedValues.ContainsKey(fieldValue))
						fieldValue = ((CodedValueDomain)fieldDomain).CodedValues[fieldValue];				
				}				
				control = GetTextFieldControl();

				if (field.Type == Field.FieldType.Date)
				{
					if(fieldValue is DateTime)
						fieldValue = DateTimeFormatConverter.DateTimeToString((DateTime)fieldValue, DateTimeKind, DateTimeFormat, new CultureInfo(Language.IetfLanguageTag));
				}
									
				(control as TextBox).Text = (fieldValue == null) ? "" : fieldValue.ToString();
				return control;
			}
			else
			{
				hasEditableControl = true;
				if (field.Name == _typeIdField)
				{
					if (control == null)
						control = GetTypeIDFieldControl(field.Nullable);
					string name = (fieldValue != null && _featureTypes.ContainsKey(fieldValue)) ? _featureTypes[fieldValue].Name : "";
					codedValueSource = new CodedValueSource() { Code = fieldValue, DisplayName = name };
					control.Tag = fieldValue;
				}
				else if ((fieldDomain != null && fieldDomain is CodedValueDomain) || FieldDomainUtils.IsDynamicDomain(field,FeatureLayer.LayerInfo))
				{
					var dynamicCodedValueSource = FieldDomainUtils.BuildDynamicCodedValueSource(field, layerInfo);
					var nullableSources = new CodedValueSources();
					CodedValueSources codedValueSources = null;

					if (this._attributeFrameworkElements.ContainsKey(_typeIdField))
					{
						var selectedItem = ((ComboBox)this._attributeFrameworkElements[_typeIdField]).SelectedItem;
						if (selectedItem != null)
						{
							var code = ((CodedValueSource)selectedItem).Code;
							if (dynamicCodedValueSource != null && code != null && dynamicCodedValueSource.ContainsKey(code))
								codedValueSources = dynamicCodedValueSource[code];
						}
					}
					else
					{
						codedValueSources = Toolkit.Utilities.DynamicCodedValueSource.GetCodedValueSources(layerInfo.TypeIdField, field, GraphicSource, dynamicCodedValueSource, nullableSources);
					}

					if (codedValueSources != null)
					{
						codedValueDomain = fieldDomain as CodedValueDomain;
						if (codedValueDomain != null)   // Coded value domain
						{
							if (this._codedValueDomains == null)
								this._codedValueDomains = new Dictionary<string, CodedValueDomain>();
							if (!this._codedValueDomains.ContainsKey(fieldName))
								this._codedValueDomains.Add(fieldName, codedValueDomain);
							else
								this._codedValueDomains[fieldName] = codedValueDomain;


							if (field.Name != _typeIdField)
								control = GetCodedValueDomainFieldControl(codedValueDomain, field, fieldValue);

							foreach (KeyValuePair<object, string> codeVal in codedValueDomain.CodedValues)
							{
								if ((codeVal.Key != null && codeVal.Key.Equals(fieldValue)) || (fieldValue != null && fieldValue.Equals(codeVal.Key)))
								{
									codedValueSource = new CodedValueSource() { Code = codeVal.Key, DisplayName = codeVal.Value == null ? "" : codeVal.Value };
									break;
								}
							}
						}
						else
							control = GetCodedValueDomainFieldControl(null, field, fieldValue);

						if (codedValueSource == null && fieldValue != null)
							codedValueSource = new CodedValueSource() { Code = fieldValue, DisplayName = fieldValue == null ? " " : fieldValue.ToString(), Temp = true };
					}
					else
					{
						if (field.Type == Field.FieldType.Date)
							control = GetDateFieldControl();
						else
							control = GetTextFieldControl();
					}
				}			
				else   
				{
					if (field.Type == Field.FieldType.Date)
						control = GetDateFieldControl();
					else
						control = GetTextFieldControl();
				}
			}
			control.DataContext = field;

			// Set the binding:
			Binding binding = new Binding("Value");
			binding.Mode = bindingMode;			
			if (bindingMode == BindingMode.TwoWay)
			{
				binding.Converter = new FeatureDataFieldValueConverter(codedValueDomain);
				binding.ConverterParameter = fieldName;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
				binding.ValidatesOnExceptions = true;
				binding.NotifyOnValidationError = true;
			}
			if (codedValueDomain != null)
				binding.Source = codedValueSource;

			Type type = Utilities.GetFieldType(fieldName, this.FeatureLayer.LayerInfo.Fields);			
			IKeyValue datafield = null;
			if (type == typeof(int?) || type == typeof(int))
			{
				try { datafield = new FeatureDataField<int?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToInt32(fieldValue) : (int?)null); }
				catch { datafield = new FeatureDataField<int?>(this, layerInfo, field, type, (int?)null); }
			}
			else if (type == typeof(short?) || type == typeof(short))
			{
				try { datafield = new FeatureDataField<short?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToInt16(fieldValue) : (short?)null); }
				catch { datafield = new FeatureDataField<short?>(this, layerInfo, field, type, (short?)null); }
			}
			else if (type == typeof(double?) || type == typeof(double))
			{
				try { datafield = new FeatureDataField<double?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToDouble(fieldValue) : (double?)null); }
				catch { datafield = new FeatureDataField<double?>(this, layerInfo, field, type, (double?)null); }
			}
			else if (type == typeof(float?) || type == typeof(float))
			{
				try { datafield = new FeatureDataField<float?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToSingle(fieldValue) : (float?)null); }
				catch { datafield = new FeatureDataField<float?>(this, layerInfo, field, type, (float?)null); }
			}
			else if (type == typeof(DateTime?) || type == typeof(DateTime))
			{
				try { datafield = new FeatureDataField<DateTime?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToDateTime(fieldValue) : (DateTime?)null); }
				catch { datafield = new FeatureDataField<DateTime?>(this, layerInfo, field, type, (DateTime?)null); }
			}
			else if (type == typeof(long?) || type == typeof(long))
			{
				try { datafield = new FeatureDataField<long?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToInt64(fieldValue) : (long?)null); }
				catch { datafield = new FeatureDataField<long?>(this, layerInfo, field, type, (long?)null); }
			}
			else if (type == typeof(byte?) || type == typeof(byte))
			{
				try { datafield = new FeatureDataField<byte?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToByte(fieldValue) : (byte?)null); }
				catch { datafield = new FeatureDataField<byte?>(this, layerInfo, field, type, (byte?)null); }
			}
			else if (type == typeof(bool?) || type == typeof(bool))
			{
				try { datafield = new FeatureDataField<bool?>(this, layerInfo, field, type, fieldValue != null ? Convert.ToBoolean(fieldValue) : (bool?)null); }
				catch { datafield = new FeatureDataField<bool?>(this, layerInfo, field, type, (bool?)null); }
			}
			else if (type == typeof(string))
			{
				try { datafield = new FeatureDataField<string>(this, layerInfo, field, type, fieldValue != null ? Convert.ToString(fieldValue) : (string)null); }
				catch { datafield = new FeatureDataField<string>(this, layerInfo, field, type, (string)null); }
			}
            else if(type == typeof(Guid))
            {                
                if (fieldValue != null && fieldValue.GetType() != typeof(Guid))
                {                    
                    var valueStr = Convert.ToString(fieldValue, System.Globalization.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        Guid guid;
                        if (Guid.TryParse(valueStr, out guid))
                            fieldValue = guid;
                    }
                }
                try { datafield = new FeatureDataField<Guid>(this, layerInfo, field, type, (Guid)fieldValue); }
                catch { datafield = new FeatureDataField<object>(this, layerInfo, field, type, fieldValue); }
            }
			else if (type == typeof(object))
			{
				datafield = new FeatureDataField<object>(this, layerInfo, field, type, fieldValue);
			}			
			if (datafield != null)
			{
				if (this._dataFields == null)
					this._dataFields = new Dictionary<string, IKeyValue>();
				if (this._dataFields.ContainsKey(fieldName))
				{
					this._dataFields[fieldName].PropertyChanged -= FeatureDataField_PropertyChanged;
					this._dataFields[fieldName] = datafield;
				}
				else
					this._dataFields.Add(fieldName, datafield);

				if (field.Editable)
					datafield.PropertyChanged += FeatureDataField_PropertyChanged;
				binding.Source = datafield;
			}

			if (control is ComboBox)
			{
				int index = -1;
				if (field.Name == _typeIdField)
				{					
					foreach (FeatureType ft in _featureTypes.Values)
					{
						index++;
						if (ft.Id.Equals(fieldValue))
							break;
					}
				}
				else				
					index = GetCodedValueIndex(codedValueDomain, fieldValue);
				if (field.Nullable && index != -1)
					index++;
				if (fieldValue != null && index == -1)
				{
					CodedValueSources sources = (control as ComboBox).ItemsSource as CodedValueSources;
					if (sources != null)
					{
						CodedValueSource source = sources.FirstOrDefault(x => x.Code != null && x.Code.ToString() == fieldValue.ToString());
						if (sources != null)
							index = sources.IndexOf(source);
					}
				}				

				(control as ComboBox).SelectedIndex = index;
				control.SetBinding(ComboBox.SelectedItemProperty, binding);
			}
			else if (control is DateTimePicker)
				control.SetBinding(DateTimePicker.SelectedDateProperty, binding);
			else
				control.SetBinding(TextBox.TextProperty, binding);

			return control;
		}

		private void GenerateField(FeatureLayerInfo layerInfo, Field field, Grid grid, BindingMode bindingMode)
		{
			if (field.Type == Field.FieldType.OID && !field.Editable)
				return;

			int gridRow = grid.RowDefinitions.Count;
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

			// Field label:
			ContentControl fieldLabel = new ContentControl();

			// Applying Style if any:
			if (this.LabelStyle != null)
				fieldLabel.Style = this.LabelStyle;
			fieldLabel.Content = string.IsNullOrEmpty(field.Alias) ? field.Name : field.Alias;

			fieldLabel.VerticalAlignment = VerticalAlignment.Center;
			if (this.LabelPosition == FeatureDataFormLabelPosition.Top)
				fieldLabel.HorizontalAlignment = HorizontalAlignment.Left;
			else
				fieldLabel.HorizontalAlignment = HorizontalAlignment.Right;

			fieldLabel.DataContext = field;

			fieldLabel.SetValue(Grid.RowProperty, gridRow);
			if (this.LabelPosition == FeatureDataFormLabelPosition.Top)
			{
				fieldLabel.SetValue(Grid.ColumnProperty, 1);

				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
				gridRow++;
			}
			else
				fieldLabel.SetValue(Grid.ColumnProperty, 0);
			fieldLabel.IsTabStop = false;
			// Adding the label:
			grid.Children.Add(fieldLabel);

			object typeIdFieldValue = (this._typeIdField != null && this.GraphicSource.Attributes.ContainsKey(this._typeIdField)) ? this.GraphicSource.Attributes[this._typeIdField] : null;
			Domain fieldDomain = GetDomain(field.Name, typeIdFieldValue);
			if (fieldDomain == null)
				fieldDomain = field.Domain;
			PopulateFieldControl(layerInfo, field, grid, bindingMode, gridRow, fieldDomain);
		}

		private Control PopulateFieldControl(FeatureLayerInfo layerInfo, Field field, Grid grid, BindingMode bindingMode, int gridRow, Domain fieldDomain)
		{
			// Field control:
			Control fieldControl = GetControlFromType(layerInfo, field, bindingMode, fieldDomain);

			// Set user control's AssociatedFieldName attached property to identify the corresponding field:
			fieldControl.SetValue(AssociatedFieldNameProperty, field.Name);
			// Handling binding validation errors when the field control loses its focus:
			if (!this.IsReadOnly && this.CanUserEdit)
			{
				fieldControl.LostFocus += FieldControl_LostFocus;
				fieldControl.GotFocus += FieldControl_GotFocus;
			}

			// Setting the IsEnabled property of the TextBox controls based on IsReadOnly property of the FeatureDataForm or 
			// the Editable property of the corresponding field if FeatureDataForm is not in read-only mode.
			if (fieldControl is TextBox)
			{
				TextBox tb = (fieldControl as TextBox);
				tb.IsReadOnly = this.IsReadOnly || !field.Editable || !this.CanUserEdit;
				if (field.Length > 0 && !tb.IsReadOnly)
					tb.MaxLength = field.Length;
			}

			fieldControl.VerticalAlignment = VerticalAlignment.Center;
			fieldControl.SetValue(Grid.RowProperty, gridRow);
			if (this.LabelPosition == FeatureDataFormLabelPosition.Left)
				fieldControl.SetValue(Grid.ColumnProperty, 2);
			else
				fieldControl.SetValue(Grid.ColumnProperty, 1);
			// Adding or replacing the control:
			if (this._attributeFrameworkElements.ContainsKey(field.Name))
			{
				Control previousControl = this._attributeFrameworkElements[field.Name];
				int backupTabIndex = previousControl.TabIndex;
				if (previousControl != null && previousControl.Parent == grid)
					grid.Children.Remove(previousControl);
				grid.Children.Add(fieldControl);
				// Updating the _attributeFrameworkElements dictionary:
				this._attributeFrameworkElements[field.Name] = fieldControl;
				fieldControl.TabIndex = backupTabIndex;
			}
			else
			{
				grid.Children.Add(fieldControl);
				// Adding to the _attributeFrameworkElements dictionary:
				this._attributeFrameworkElements.Add(field.Name, fieldControl);
				fieldControl.TabIndex = currentTabOrder++;
			}
			return fieldControl;
		}

		private FrameworkElement GenerateFields()
		{
			if (this.FeatureLayer == null || this.GraphicSource == null || this._contentPresenter == null ||
				this.FeatureLayer.LayerInfo == null || this.FeatureLayer.LayerInfo.Fields == null ||
				!this.FeatureLayer.LayerInfo.Fields.GetEnumerator().MoveNext())
				return null;

			BindingMode bindingMode = this.IsReadOnly || !this.CanUserEdit || this.FeatureLayer.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

			this._contentPresenter.DataContext = this.GraphicSource;

			Grid grid = new Grid();
			grid.Margin = PANEL_MARGIN;
			// Label column:
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
			if (this.LabelPosition == FeatureDataFormLabelPosition.Left)
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SIZEOF_SPACING_COLUMN, GridUnitType.Pixel), MinWidth = SIZEOF_SPACING_COLUMN });
			// Content column:
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

			this._attributeFrameworkElements = new Dictionary<string, Control>();

			this._isBindingData = true;
			ESRI.ArcGIS.Client.Tasks.OutFields outFields = this.FeatureLayer.OutFields;
			if (outFields != null)
			{
				currentTabOrder = 0;
				foreach (Field field in this.FeatureLayer.LayerInfo.Fields)
				{
					if ((outFields.Contains("*") || outFields.Contains(field.Name)) &&	//Outfield requested
						this.GraphicSource.Attributes.ContainsKey(field.Name) && 		//Attribute exist in graphic
						IsViewableAttribute(field)) 					//Attribute not hidden
						GenerateField(this.FeatureLayer.LayerInfo, field, grid, bindingMode);
				}
				if (this._commitButton != null)
					this._commitButton.TabIndex = currentTabOrder;
				if (this._deleteButton != null)
					this._deleteButton.TabIndex = ++currentTabOrder;
			}
#if SILVERLIGHT
			Dispatcher.BeginInvoke((Action)delegate()
			{
#endif
				this._isBindingData = false;
#if SILVERLIGHT
			});
#endif

			return grid;
		}

		internal static bool IsViewableAttribute(Field field)
		{
			if (field.Type != Field.FieldType.Blob && field.Type != Field.FieldType.Geometry &&
				field.Type != Field.FieldType.Raster && field.Type != Field.FieldType.Unknown)
				return true;

			return false;
		}

		private void AutoGenerateColumns()
		{
			if (this._contentPresenter != null)
				this._contentPresenter.Content = GenerateFields();
		}

		private void UpdateStates()
		{
			if (this._contentPresenter != null && this._contentPresenter.Content != null)
			{
				if (!this.IsReadOnly && this.CanUserEdit)
				{
					bool oldValue = HasEdits;
					bool newValue = false;
					if (HasChange())
					{
						newValue = true;
						if (this._commitButton != null)
						{
							if (HasInvalidField() || !this.IsValid)
								this._commitButton.IsEnabled = false;
							else
								this._commitButton.IsEnabled = true;
						}
					}
					else
					{
						newValue = false;
						if (this._commitButton != null)
							this._commitButton.IsEnabled = true;
					}
					HasEdits = newValue;

					if (this._commitButton != null)
						this._commitButton.Visibility = Visibility.Visible;
					if (this._deleteButton != null)
						this._deleteButton.Visibility = Visibility.Visible;
				}
				else
				{
					IsValid = true;
					HasEdits = false;
					if (hasEditableControl)
					{
						AutoGenerateColumns();
						hasEditableControl = false;
					}
					if (this._commitButton != null)
						this._commitButton.Visibility = Visibility.Collapsed;
					if (this._deleteButton != null)
						this._deleteButton.Visibility = Visibility.Collapsed;
				}
			}
		}

		#region Methods related to _attributeValidationStatus Dictionary
		internal void UpdateDictionary(ref Dictionary<string, bool> dictionary, string key, bool value)
		{
			if (dictionary == null)
				dictionary = new Dictionary<string, bool>();
			if (!dictionary.ContainsKey(key))
				dictionary.Add(key, value);
			else
				dictionary[key] = value;
		}
		private void RemoveFromDictionary(ref Dictionary<string, bool> dictionary, string key)
		{
			if (dictionary != null && dictionary.ContainsKey(key))
				dictionary.Remove(key);
		}
		private bool HasChange()
		{
			bool changed = false;
			if (this._attributeValidationStatus == null || (this._attributeValidationStatus != null && this._attributeValidationStatus.Count == 0))
				return false; // If there are no entries there are no changes
			else
			{
				var culture = new CultureInfo(Language.IetfLanguageTag);
				// If there are entries verify that that at least one is different from graphic attribute value.				
				foreach (KeyValuePair<string, bool> ChangeLogItem in _attributeValidationStatus)
				{
					string fieldName = ChangeLogItem.Key;
					Control control = null;
					if (_attributeFrameworkElements.ContainsKey(fieldName)) 
						control = _attributeFrameworkElements[fieldName];
					if (control != null)
					{
						object GraphicValue = null;
						
						if (GraphicSource.Attributes.ContainsKey(fieldName))
							GraphicValue = GraphicSource.Attributes[fieldName];
						
						Type type = Utilities.GetFieldType(fieldName, FeatureLayer.LayerInfo.Fields);
						
						if (control is ComboBox)
						{
							var value = ((ComboBox)control).SelectedItem;							
							if (value is CodedValueSource)
							{								
								if (Utilities.AreEqual(((CodedValueSource)value).Code, GraphicValue,type, culture) == false)
									return true;
							}
							else if (value == null && GraphicValue != null)
								return true;
						}
						else if (control is TextBox)
						{
							var value = ((TextBox)control).Text;
							if (Utilities.AreEqual(value, GraphicValue, type, culture) == false)
								return true;

						}
						else if (control is DateTimePicker)
						{
							var value = ((DateTimePicker)control).SelectedDate;
							if (Utilities.AreEqual(value, GraphicValue, type, culture) == false)
								return true;
						}
					}
				}
			}
			return changed;
		}
		private bool HasInvalidField()
		{
			if (this._attributeValidationStatus == null ||
				(this._attributeValidationStatus != null && this._attributeValidationStatus.Count == 0))
				return false;
			else
			{
				if (this._attributeValidationStatus.ContainsValue(false))
					return true;
				else
					return false;
			}
		}
		#endregion

		private void CommitButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.IsReadOnly && this.CanUserEdit)
				ApplyChanges();
			else
			{
				if (hasEditableControl)
				{
					AutoGenerateColumns();
					hasEditableControl = false;
				}
				if (this._commitButton != null)
					this._commitButton.Visibility = Visibility.Collapsed;
			}
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.IsReadOnly && this.CanUserEdit)
			{
				if (FeatureLayer != null)
				{
					if (FeatureLayer.IsDeleteAllowed(this.GraphicSource))
					{
						FeatureLayer.Graphics.Remove(this.GraphicSource);
						OnEditEnded(EventArgs.Empty);
					}
				}
			}
			else 
			{
				if (hasEditableControl)
				{
					AutoGenerateColumns();
					hasEditableControl = false;
				}
				if (this._deleteButton != null)
					this._deleteButton.Visibility = Visibility.Collapsed;
			}
		}

		private bool ApplyChanges(Control fieldControl, bool shouldUpdateGraphicSource)
		{
			if (this.FeatureLayer == null || this.GraphicSource == null ||
				(this.FeatureLayer != null && this.GraphicSource != null && !CheckGraphicParent()))
				return false;

			BindingExpression bindingExpression = null;
			if (fieldControl is ComboBox)
				bindingExpression = (fieldControl as ComboBox).GetBindingExpression(ComboBox.SelectedItemProperty);
			else if (fieldControl is DateTimePicker)
			{
				DateTimePicker datePicker = fieldControl as DateTimePicker;
				bindingExpression = datePicker.GetBindingExpression(DateTimePicker.SelectedDateProperty);
				// Need to force DatePicker control to update its contents when SelectedDate is NULL:				
			}
			else if (fieldControl is TextBox)
			{
				// Do not validate read-only fields.
				if (((TextBox)fieldControl).IsReadOnly)
					return false;
				bindingExpression = (fieldControl as TextBox).GetBindingExpression(TextBox.TextProperty);
			}

			bool hasError = false;

			if (bindingExpression != null && bindingExpression.ParentBinding != null)
			{
				string fieldName = bindingExpression.ParentBinding.ConverterParameter.ToString();
				if (GraphicSource.Attributes.ContainsKey(fieldName))
				{
					object currentValueInGraphic = this.GraphicSource.Attributes[fieldName];
					if (this._codedValueDomains != null && this._codedValueDomains.ContainsKey(fieldName))
					{
						CodedValueDomain codedValueDomain = this._codedValueDomains[fieldName];
						foreach (KeyValuePair<object, string> codeVal in codedValueDomain.CodedValues)
						{
							if (Utilities.AreEqual(codeVal.Key, this.GraphicSource.Attributes[fieldName], codeVal.Key.GetType(), new CultureInfo(Language.IetfLanguageTag)))
							{
								currentValueInGraphic = new CodedValueSource() { Code = codeVal.Key, DisplayName = codeVal.Value == null ? "" : codeVal.Value };
								break;
							}
						}
					}					

					Type type = Utilities.GetFieldType(fieldName, this.FeatureLayer.LayerInfo.Fields);
					bool hasChange = HasChange(fieldControl, currentValueInGraphic, type, out hasError);
					if (hasChange || (this._attributeValidationStatus != null &&
						this._attributeValidationStatus.ContainsKey(fieldName)))
					{
						if (shouldUpdateGraphicSource)
							this._isUpdatedByCommitButton = true;
						else
							this._isVerifyingAfterLosingFocus = true;
						try
						{
							bindingExpression.UpdateSource();
							if (!hasChange && this._attributeValidationStatus != null &&
								this._attributeValidationStatus.ContainsKey(fieldName) &&
								this._attributeValidationStatus[fieldName])
								RemoveFromDictionary(ref this._attributeValidationStatus, fieldName);
						}
						catch
						{
							UpdateDictionary(ref this._attributeValidationStatus, fieldName, false);
							hasError = true;
						}
						if (shouldUpdateGraphicSource)
							this._isUpdatedByCommitButton = false;
						else
							this._isVerifyingAfterLosingFocus = false;

						if (!hasChange && this._attributeValidationStatus != null &&
							this._attributeValidationStatus.ContainsKey(fieldName) &&
							this._attributeValidationStatus[fieldName])
							return hasError;

						if (!hasError)
						{
							if (shouldUpdateGraphicSource)
								UpdateDictionary(ref this._attributeValidationStatus, fieldName, true);
						}
						else
							UpdateDictionary(ref this._attributeValidationStatus, fieldName, false);
					}
					else
						RemoveFromDictionary(ref this._attributeValidationStatus, fieldName);
				}
			}

			return hasError;
		}

		private bool HasChange(Control control, object valueInGraphic, Type type, out bool hasError)
		{
			hasError = false;
			CultureInfo culture = new CultureInfo(Language.IetfLanguageTag);
			try
			{
				if (control is ComboBox)
				{
					if ((control as ComboBox).SelectedItem == null && valueInGraphic is CodedValueSource)
					{
						if ((valueInGraphic as CodedValueSource).Code == null && (valueInGraphic as CodedValueSource).DisplayName == null)
							return false;
					}
					else
					{
						if (Utilities.AreEqual((control as ComboBox).SelectedItem, valueInGraphic, type, culture, out hasError))
							return false;
					}
				}
				else if (control is DateTimePicker)
				{
					DateTime? selectedDate = (control as DateTimePicker).SelectedDate;
					DateTime? dateToCheck = null;
					if (selectedDate != null)
						dateToCheck = new DateTime(selectedDate.Value.Ticks, selectedDate.Value.Kind);
					if (Utilities.AreEqual(dateToCheck, valueInGraphic, type, culture, out hasError))
						return false;
				}
				else if (control is TextBox)
				{
					string input = (control as TextBox).Text;
					if (type == typeof(string))
					{
						if ((valueInGraphic as string) == null && input.Length == 0)
							return false;
						return valueInGraphic as string != input;
					}
					if ((string.IsNullOrEmpty((control as TextBox).Text.Trim()) && valueInGraphic == null) ||
						Utilities.AreEqual((control as TextBox).Text, valueInGraphic, type, culture,  out hasError))
						return false;
				}
			}
			catch   // There was definitely a change causing the exception, let BindingExpression take care of it.
			{
				hasError = true;
			}

			return true;
		}

		private void FieldControl_LostFocus(object sender, RoutedEventArgs e)
		{
			bool hasError = false;
			var controls = this._attributeFrameworkElements.Values.ToList();
			for (int i = 0; i < this._attributeFrameworkElements.Values.Count; i++)
			{
				Control control = controls[i] as Control;
				if (control != null)
				{
					if (ApplyChanges(control, false))
						hasError = true;
				}
			}

			if (hasError)
				IsValid = false;

			UpdateStates();
		}

		private bool RepopulateDomains(Control control)
		{
			if (control == null || this._contentPresenter == null)
				return false;

			Grid grid = this._contentPresenter.Content as Grid;
			if (grid != null)
			{
				List<Field> fields = null;
				if (this.FeatureLayer != null && this.FeatureLayer.LayerInfo != null)
					fields = this.FeatureLayer.LayerInfo.Fields;
				if (fields != null)
				{
					Type typeIdFieldType = Utilities.GetFieldType(this._typeIdField, fields);
					object typeIdFieldValue = null;
					try
					{
						object value = null;
						if (control is ComboBox)
							value = ((control as ComboBox).SelectedItem as CodedValueSource).Code;
						else if (control is DateTimePicker)
							value = (control as DateTimePicker).SelectedDate;
						else if (control is TextBox)
							value = (control as TextBox).Text;
						if (Utilities.IsNotOfTypeSystemNullable(typeIdFieldType))
							typeIdFieldValue = System.Convert.ChangeType(value, typeIdFieldType, new CultureInfo(Language.IetfLanguageTag));
						else
							typeIdFieldValue = System.Convert.ChangeType(value, System.Nullable.GetUnderlyingType(typeIdFieldType), new CultureInfo(Language.IetfLanguageTag));
					}
					catch { }

					foreach (Field field in fields)
					{
						string fieldName = field.Name;
						if (this._attributeFrameworkElements.ContainsKey(fieldName) &&
							this._dataFields.ContainsKey(fieldName))
						{
							Control previousFieldControl = this._attributeFrameworkElements[fieldName];
							object previousFieldControlValue = null;
							if (previousFieldControl is TextBox)
								previousFieldControlValue = (previousFieldControl as TextBox).Text;
							else if (previousFieldControl is DateTimePicker)
								previousFieldControlValue = (previousFieldControl as DateTimePicker).SelectedDate;
							else if (previousFieldControl is ComboBox)
							{
								if ((previousFieldControl as ComboBox).SelectedItem == null && this.GraphicSource.Attributes[fieldName] != null)
									previousFieldControlValue = this.GraphicSource.Attributes[fieldName];
								else
								{
									CodedValueSource selectedCodedValueSource = (previousFieldControl as ComboBox).SelectedItem as CodedValueSource;
									if (selectedCodedValueSource != null)
										previousFieldControlValue = selectedCodedValueSource.Code;
								}
							}
							if (!this.IsReadOnly && this.CanUserEdit)
							{
								previousFieldControl.LostFocus -= FieldControl_LostFocus;
								previousFieldControl.GotFocus -= FieldControl_GotFocus;
							}

							BindingMode bindingMode = this.IsReadOnly && !this.CanUserEdit ? BindingMode.OneWay : BindingMode.TwoWay;
							int? gridRow = previousFieldControl.GetValue(Grid.RowProperty) as int?;
							Domain fieldDomain = GetDomain(field.Name, typeIdFieldValue);
							if (fieldDomain == null)
								fieldDomain = field.Domain;

							Control newFieldControl = PopulateFieldControl(FeatureLayer != null ? FeatureLayer.LayerInfo : null, field, grid, bindingMode, gridRow.Value, fieldDomain);

							if (newFieldControl is TextBox)
								(newFieldControl as TextBox).SetValue(TextBox.TextProperty, previousFieldControlValue == null ? "" : previousFieldControlValue.ToString());
							else if (newFieldControl is DateTimePicker)
								(newFieldControl as DateTimePicker).SetValue(DateTimePicker.SelectedDateProperty, previousFieldControlValue as DateTime?);
							else if (newFieldControl is ComboBox)
							{
								var newComboBox = (ComboBox)newFieldControl;

								if (previousFieldControl is ComboBox)
								{
									int selectedIndex = -1;
									Control combobox = (field.Name != _typeIdField && field.Domain == null) ? newComboBox : previousFieldControl;
									selectedIndex = ((ComboBox)combobox).SelectedIndex;
									newComboBox.SetValue(ComboBox.SelectedIndexProperty, selectedIndex);
								}
								else if (previousFieldControl is TextBox || previousFieldControl is DateTimePicker)
								{
									var codedValueSources = newComboBox.ItemsSource as IEnumerable<CodedValueSource>;
									if (codedValueSources != null)
									{
										var codedValueSource = codedValueSources.FirstOrDefault(cvs => Utilities.AreEqual(cvs.Code, previousFieldControlValue, Utilities.GetFieldType(field), new CultureInfo(Language.IetfLanguageTag)));
										if (codedValueSource != null)
											newComboBox.SetValue(ComboBox.SelectedItemProperty, codedValueSource);
										else
										{
											var list = new List<CodedValueSource>(codedValueSources);
											
											var temp = list.FirstOrDefault(cvd => cvd.Temp == true);
											if (temp != null)
												list.Remove(temp);

											int index = field.Nullable ? 1 : 0;
											list.Insert(index, new CodedValueSource()
											{
												Code = previousFieldControlValue,
												DisplayName = previousFieldControlValue == null ? "" : previousFieldControlValue.ToString(),
												Temp = true
											});
											newComboBox.SetValue(ComboBox.ItemsSourceProperty, list);
											newComboBox.SetValue(ComboBox.SelectedIndexProperty, index );
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		private void ResetValidation()
		{
			if (_attributeValidationStatus != null && _attributeValidationStatus.Keys.Any())
			{
				foreach (var key in _attributeValidationStatus.Keys.ToArray())
					_attributeValidationStatus[key] = true;
			}
			IsValid = true;
			UpdateStates();
		}

		private void RegenerateLayout(Control control, Control controlToFocus)
		{
			ResetValidation();
			int tabIndex = controlToFocus != null ? controlToFocus.TabIndex : 0;
			this._isBindingData = true;
			if (RepopulateDomains(control))	// Layout is regenerated, apply the correct tab order
			{
				Grid grid = this._contentPresenter.Content as Grid;
				if (grid != null && controlToFocus != null)
				{
					foreach (Control c in grid.Children)
					{
						if (c.IsTabStop && c.TabIndex == tabIndex)
						{
							isFocusedInternally = true;		// Avoid endless loop when hitting the next line
							c.Focus();
							break;
						}
					}
				}
			}
#if SILVERLIGHT
			Dispatcher.BeginInvoke((Action)delegate()
			{
#endif
				this._isBindingData = false;
#if SILVERLIGHT
			});
#endif
		}
		/// <summary>
		/// Handles the GotFocus event of the FieldControl control.
		/// </summary>
		/// <remarks>
		/// In the case that a TextBox control holds the TypeId field we may need to re-generate the form layout when the 
		/// TypeId field control has lost its focus. This has been identified via the private typeIdFieldTextBox variable 
		/// in which has been populated in its LostFocus event handler (FieldControl_LostFocus).
		/// </remarks>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void FieldControl_GotFocus(object sender, RoutedEventArgs e)
		{
			if (isFocusedInternally)
			{
				isFocusedInternally = false;
				return;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Updates the GraphicSource by applying all changes that have been made and not applied to the associated graphic's attributes.
		/// </summary>
		/// <returns>true if the edits could be applied</returns>
		/// <seealso cref="HasEdits"/>
		/// <seealso cref="IsValid"/>
		public bool ApplyChanges()
		{
			if (!IsValid) return false;
			if (!HasEdits)
			{
				if (IsValid)
					OnEditEnded(EventArgs.Empty);
				return false;
			}
			if (this.IsReadOnly && !this.CanUserEdit)
				throw new InvalidOperationException(Properties.Resources.FeatureDataForm_EditInReadOnlyModeNotAllowed);

			foreach (KeyValuePair<string, Control> pair in this._attributeFrameworkElements)
			{
				if (!IsEditFieldsInfo(pair.Key))
					if(ApplyChanges(pair.Value, true)) // Sets _isAtLeastOneAttributeChangeFailed if any validation problem exists
						IsValid = false;
			}

			bool retVal = HasInvalidField();
			if (retVal)
				IsValid = false;
			else
				this._attributeValidationStatus = null;

			UpdateStates();

			if (IsValid)
			{
				GenerateUI(false);
				OnEditEnded(EventArgs.Empty);
			}

			return retVal;
		}

		private bool IsEditFieldsInfo(string field)
		{
			return (this.FeatureLayer != null && this.FeatureLayer.LayerInfo != null && this.FeatureLayer.LayerInfo.EditFieldsInfo != null
					 && (this.FeatureLayer.LayerInfo.EditFieldsInfo.EditDateField == field || this.FeatureLayer.LayerInfo.EditFieldsInfo.EditorField == field));			
		}
		#endregion

		#region Events
		#region EditEnded Members
		/// <summary>
		/// Occurs when the last update was successfully finished in FeatureDataForm.
		/// </summary>
		public event EventHandler<EventArgs> EditEnded;
		private void OnEditEnded(EventArgs e)
		{
			if (EditEnded != null)
				EditEnded(this, e);
		}
		#endregion

		#region INotifyPropertyChanged Members
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#endregion

		#region FeatureDataForm helper methods
		internal sealed class Utilities
		{			
			internal static Type GetFieldType(Field field)
			{
				Type fieldType = Type.GetType("System.Object");
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
					case Field.FieldType.GUID:
						fieldType = typeof(Guid);
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
						break;
					case Field.FieldType.GlobalID:
					case Field.FieldType.String:
					case Field.FieldType.XML:
						fieldType = typeof(string);
						break;
					default:
						throw new NotSupportedException(string.Format(Properties.Resources.FieldDomain_FieldTypeNotSupported, fieldType.GetType()));
				}
				return fieldType;
			}

			internal static Type GetFieldType(string fieldName, IEnumerable<Field> fields)
			{
				Type fieldType = Type.GetType("System.Object");
				foreach (Field field in fields)
				{
					if (field.Name == fieldName)					
						return GetFieldType(field);					
				}
				return fieldType;
			}

			internal static void IsValidRange<T>(RangeDomain<T> range, T val) where T : IComparable
			{
				if (val.CompareTo(range.MinimumValue) < 0 || val.CompareTo(range.MaximumValue) > 0)
					throw new ArgumentException(string.Format(Properties.Resources.Validation_InvalidRangeDomain, range.MinimumValue, range.MaximumValue));
			}

			internal static bool AreEqual(object firstObject, object secondObject, Type type, CultureInfo culture)
			{
				bool err = false;
				return AreEqual(firstObject, secondObject, type, culture, out err);
			}

			internal static bool AreEqual(object firstObject, object secondObject, Type type, CultureInfo culture, out bool hasError)
			{
				hasError = false;
				bool isNullable = type == typeof(string) ? true : !IsNotOfTypeSystemNullable(type);										
				if (!isNullable && (firstObject == null || secondObject == null))
					hasError = true;

				CodedValueSource firstCodedValueSource = firstObject as CodedValueSource;
				CodedValueSource secondCodedValueSource = secondObject as CodedValueSource;
				if (firstCodedValueSource != null && secondCodedValueSource != null)
					return AreEqual(firstCodedValueSource.Code, secondCodedValueSource.Code, firstCodedValueSource.Code != null ? firstCodedValueSource.Code.GetType() : null, culture);
				else if (firstCodedValueSource == null && secondCodedValueSource != null)
					return AreEqual(firstObject, secondCodedValueSource.Code, secondCodedValueSource.Code != null ? secondCodedValueSource.Code.GetType() : null, culture);
				else if (firstCodedValueSource != null && secondCodedValueSource == null)
					return AreEqual(firstCodedValueSource.Code, secondObject, firstCodedValueSource.Code != null ? firstCodedValueSource.Code.GetType() : null, culture);
				else
				{
					if (firstObject == null && secondObject == null) //	Both are null
						return true;
					else if (firstObject == null || secondObject == null) // One is null
						return false;
					try //None are null. Try convert to same type
					{
						object val1 = ConvertToType(firstObject, type, culture);
						object val2 = ConvertToType(secondObject, type, culture);
						return val1 != null ? val1.Equals(val2) : val1 == val2;
					}
					catch { hasError = true; }
				}

				return false;
			}
			internal static object ConvertToType(object value, Type type, CultureInfo culture)
			{
				bool isNullable = !IsNotOfTypeSystemNullable(type);
				if (value == null && isNullable) return null;
				if (value != null && type == value.GetType()) return value; //Same type => no conversion
				if (value is string) //Conversion from string
				{
					value = value as string;
					if (value != null)
					{
						if ((value as string).Trim().Length == 0)
						{
							if (isNullable)
								return null;
							else
								throw new ArgumentException();
						}
                        else if (type == typeof(Guid))
                        {                                                        
                                Guid guid;
                                if (Guid.TryParse((string)value, out guid))
                                    return guid;                            
                        }
					}
				}
				if (isNullable)
					type = System.Nullable.GetUnderlyingType(type);
				return Convert.ChangeType(value, type, culture);

			}
			internal static bool IsNotOfTypeSystemNullable(Type type)
			{
				return (type == typeof(string) || type == typeof(int) || type == typeof(short) ||
						type == typeof(double) || type == typeof(float) || type == typeof(DateTime) ||
						type == typeof(long) || type == typeof(byte) || type == typeof(bool) || type == typeof(Guid) || type == typeof(object));
			}
		}
		#endregion
	}
}