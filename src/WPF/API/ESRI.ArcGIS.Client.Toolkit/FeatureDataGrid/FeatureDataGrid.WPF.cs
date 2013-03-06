// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit.Utilities;
using System.ComponentModel;
using ESRI.ArcGIS.Client.FeatureService;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Web;
using System.Collections.Specialized;
using System.Windows.Media;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
#if NET35
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{
	//WPF specific code for FeatureDataGrid
	public partial class FeatureDataGrid : DataGrid
	{
		internal static Dictionary<string, object> TempAttributes = new Dictionary<string, object>();
		private FeatureCollectionView _collection { get; set; }
		private FeatureLayer featureLayer;
		private List<Graphic> cleanupGraphics;
		private GraphicCollection graphicCollection;
		private GraphicCollection GraphicCollection
		{
			get { return graphicCollection; }
			set
			{
				if (graphicCollection != value)
				{
					if (graphicCollection != null)
					{
						graphicCollection.CollectionChanged -= Graphics_CollectionChanged;
						foreach (var g in graphicCollection)
							g.AttributeValueChanged -= Graphic_AttributeValueChanged;
					}
					graphicCollection = value;
					cleanupGraphics = graphicCollection != null ? graphicCollection.ToList() : null;
					CreateAnAttributeTypeLookup();
					if (graphicCollection != null)
					{
						graphicCollection.CollectionChanged += Graphics_CollectionChanged;
						foreach (var g in graphicCollection)
						{
							foreach (var attribute in g.Attributes)
								ValidateAttributeType(attribute);
							g.AttributeValueChanged += Graphic_AttributeValueChanged;
						}
					}
				}
			}
		}
		private List<Graphic> _selected = new List<Graphic>();
		private string OutFields;
		private Dictionary<string, Type> UniqueAttributes = new Dictionary<string, Type>();

		/// <summary>
		/// Initializes the <see cref="FeatureDataGrid"/> class.
		/// </summary>
		static FeatureDataGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FeatureDataGrid),
					new FrameworkPropertyMetadata(typeof(FeatureDataGrid)));
		}
		
		#region Private Properties
		/// <summary>
		/// Binds to the GraphicsLayer.SelectedGraphics.Count property
		/// </summary>
		private int SelectedGraphicCount
		{
			get { return (int)GetValue(SelectedGraphicCountProperty); }
			set { SetValue(SelectedGraphicCountProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="SelectedGraphicCount"/> Dependency Property.
		/// </summary>
		private static readonly DependencyProperty SelectedGraphicCountProperty =
			DependencyProperty.Register("SelectedGraphicCount", typeof(int), typeof(FeatureDataGrid), new PropertyMetadata(0, OnSelectedGraphicCountPropertyChanged));

		private static void OnSelectedGraphicCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			int oldValue = (int)e.OldValue;
			int newValue = (int)e.NewValue;

			// if they are different update the status bar text to reflect changes
			if (oldValue != newValue)
				grid.UpdateRecordsText();

			if (grid.GraphicsLayer == null) return;
			IEnumerable<Graphic> gridSelected = grid.SelectedItems.Cast<Graphic>(); ;			
			var addedSelections = grid.GraphicsLayer.SelectedGraphics.Except(gridSelected).ToList();
			IEnumerable<Graphic> layerSelected = grid.GraphicsLayer.SelectedGraphics;
			var removedSelections = gridSelected.Except(grid.GraphicsLayer.SelectedGraphics).ToList();

			foreach (var add in addedSelections)
				grid.SelectedItems.Add(add);				
			foreach (var remove in removedSelections)
				grid.SelectedItems.Remove(remove);			
		}

		/// <summary>
		/// Binds to the GraphicsLayer.Graphics.Count property
		/// </summary>
		private int TotalGraphicCount
		{
			get { return (int)GetValue(TotalGraphicCountProperty); }
			set { SetValue(TotalGraphicCountProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="TotalGraphicCount"/> Dependency Property.
		/// </summary>
		private static readonly DependencyProperty TotalGraphicCountProperty =
			DependencyProperty.Register("TotalGraphicCount", typeof(int), typeof(FeatureDataGrid), new PropertyMetadata(0, OnTotalGraphicCountPropetyChanged));

		private static void OnTotalGraphicCountPropetyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			int oldValue = (int)e.OldValue;
			int newValue = (int)e.NewValue;

			// if they are different update the status bar text to reflect changes
			if (oldValue != newValue)
				grid.UpdateRecordsText();
		}

		#endregion

		#region Public Properties
		

		#region GraphicsLayar
	
		private static void OnGraphicsLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			GraphicsLayer oldValue = e.OldValue as GraphicsLayer;
			GraphicsLayer newValue = e.NewValue as GraphicsLayer;

			if (oldValue != null)
			{
				if (grid.featureLayer != null)
				{
					grid.featureLayer.UpdateCompleted -= grid.FeatureLayer_UpdateCompleted;
					grid.featureLayer = null; // clear out reference to old feature layer if it exists	
				}
				oldValue.PropertyChanged -= grid.GraphicsLayer_PropertyChanged;
				BindingOperations.ClearBinding(grid, FeatureDataGrid.TotalGraphicCountProperty);
				BindingOperations.ClearBinding(grid, FeatureDataGrid.SelectedGraphicCountProperty);
				if (!oldValue.IsInitialized)
					oldValue.Initialized -= grid.GraphicsLayer_Initialized;
			}
			if (newValue != null)
			{
				// if new layer is feature layer set private member
				if (newValue is FeatureLayer)
				{
					grid.featureLayer = newValue as FeatureLayer;
					grid.OutFields = grid.featureLayer.OutFields.ToString();
					grid.featureLayer.UpdateCompleted += grid.FeatureLayer_UpdateCompleted;
				}
				else
				{
					grid.GraphicCollection = newValue.Graphics;
				}

				if (newValue.IsInitialized)
				{
					newValue.PropertyChanged += grid.GraphicsLayer_PropertyChanged;
					grid.SetItemsSource(newValue.Graphics);		// Set the ItemsSource
					grid.BindToTotalGraphicsCount();			// Add total graphic count binding.
					grid.BindToSelectedGraphicsCount();			// Add selected graphic count binding.					
				}
				else
					// Wait for initialize event to fire before configuring feature data grid
					newValue.Initialized += grid.GraphicsLayer_Initialized;
			}
			grid.UpdateRecordsText();							// Update the status bar text.
			grid.SetSubmitButtonVisibility();					// Enable/Disable submit button option.
			grid.SetDeleteSelectedRowsMenuButtonEnableState();	// Enable/Disable delete option.
		}

		void Graphics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// if collection changes then need to check the attributes to make sure that
			// new fields were not added. this is only for non feature layer types.
			bool reset = false;
			if (cleanupGraphics != null && e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (var g in cleanupGraphics)
					g.AttributeValueChanged -= Graphic_AttributeValueChanged;
				cleanupGraphics.Clear();
			}
			else if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems != null)
				{
					foreach (var item in e.NewItems)
					{
						if (cleanupGraphics != null) cleanupGraphics.Add(item as Graphic);
						(item as Graphic).AttributeValueChanged += Graphic_AttributeValueChanged;
						foreach (var attribute in (item as Graphic).Attributes)
						{
							reset = ResetRequired(attribute);
							if (reset)
								break;
							else
								ValidateAttributeType(attribute);
						}
						if (reset) break;
					}
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems != null)
				{
					foreach (var item in e.OldItems)
					{
						if (cleanupGraphics != null) cleanupGraphics.Remove(item as Graphic);
						(item as Graphic).AttributeValueChanged -= Graphic_AttributeValueChanged;
					}
				}
			}
			if (reset)
				SetItemsSource(GraphicsLayer.Graphics);
		}

		private void CreateAnAttributeTypeLookup()
		{
			if (GraphicCollection == null || GraphicCollection.Count == 0) return;
			var graphic = GraphicCollection.FirstOrDefault();
			UniqueAttributes.Clear();
			foreach (var attribute in graphic.Attributes)
				UniqueAttributes.Add(attribute.Key, attribute.Value == null ? typeof(string) : attribute.Value.GetType());
		}

		private bool ResetRequired(KeyValuePair<string, object> attribute)
		{
			if (!UniqueAttributes.ContainsKey(attribute.Key))
			{
				UniqueAttributes.Add(attribute.Key, attribute.Value == null ? typeof(string) : attribute.Value.GetType());
				return true;
			}
			return false;
		}

		private void ValidateAttributeType(KeyValuePair<string, object> attribute)
		{
			if (attribute.Value == null)
			{
				if (UniqueAttributes[attribute.Key] != typeof(string))
				{
					throw new InvalidCastException(string.Format(Properties.Resources.FeatureDataGrid_MixedAttributeTypesNotAllowed,
																		 UniqueAttributes[attribute.Key], attribute.Key));
				}

			}
			else if (attribute.Value.GetType() != UniqueAttributes[attribute.Key])
			{
				throw new InvalidCastException(string.Format(Properties.Resources.FeatureDataGrid_MixedAttributeTypesNotAllowed,
																		 UniqueAttributes[attribute.Key], attribute.Key));
			}
		}

		void Graphic_AttributeValueChanged(object sender, Graphics.DictionaryChangedEventArgs e)
		{
			var attribute = new KeyValuePair<string, object>(e.Key, e.NewValue);
			if (!ResetRequired(attribute))
				ValidateAttributeType(attribute);
		}

		void FeatureLayer_UpdateCompleted(object sender, EventArgs e)
		{		
			string outFields = (sender as FeatureLayer).OutFields.ToString();
			if (outFields != this.OutFields)
			{
				this.OutFields = outFields;
				SetItemsSource(GraphicsLayer.Graphics);
			}
		}		

		private void GraphicsLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Graphics")
			{
				if (GraphicCollection != null)
				{
					GraphicCollection = GraphicsLayer.Graphics;
				} 
				SetItemsSource(GraphicsLayer.Graphics);		// Update the ItemsSource
			}
			else if (e.PropertyName == "HasEdits")
				SetSubmitButtonEnableState();
		}

		#region Private EventHandlers
		/// <summary>
		/// When graphics layer initializes set default configurations.
		/// </summary>
		private void GraphicsLayer_Initialized(object sender, System.EventArgs e)
		{
			GraphicsLayer layer = sender as GraphicsLayer;	// Get the calling graphics layer.
			layer.Initialized -= GraphicsLayer_Initialized; // Remove event handler, no longer need it.

			layer.PropertyChanged += GraphicsLayer_PropertyChanged;			
			SetItemsSource(layer.Graphics);		// Set the ItemsSource
			BindToTotalGraphicsCount();			// Add total graphic count binding.
			BindToSelectedGraphicsCount();		// Add selected graphic count binding.
			UpdateRecordsText();				// Update the status bar text.
			SetSubmitButtonVisibility();		// Enable/Disable submit button option.
			SetDeleteSelectedRowsMenuButtonEnableState(); // Enable/Disable delete option.
		}
		#endregion Private EventHandlers

		#endregion GraphicsLayer

		#region FilterSource
		/// <summary>
		/// When filter source changes the view and record text needs to be updated.
		/// </summary>
		private static void OnFilterSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			grid.ApplyFilter();
			grid.UpdateRecordsText();
		}
		#endregion

		#endregion Public Properties

		#region Override
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.DataGrid.CellEditEnding"/> event.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
		{
			base.OnCellEditEnding(e);
			if (e.EditAction == DataGridEditAction.Commit)
			{
				if (GraphicsLayer is FeatureLayer)
				{
					// This handles coded value domain fields.
					var graphic = e.Row.DataContext as Graphic;
					foreach (var item in TempAttributes)
						graphic.Attributes[item.Key] = item.Value;
					TempAttributes.Clear();

					// this handles all non coded value domain fields.
					Field field = e.Column.GetValue(FieldColumnProperty) as Field;
					bool isValidChange = true;
					if (field != null)
					{
						bool isRangeDomain = field.Domain != null && !(field.Domain is CodedValueDomain);
						FrameworkElement element = e.EditingElement;
						if (element != null)
						{
							string errorContent = null;
							object value = null;
							switch (field.Type)
							{
								case Field.FieldType.Date:
									var datepicker = FindChild<DateTimePicker>(element) as DateTimePicker;
									if (datepicker == null)
										return;
									value = datepicker.SelectedDate;
									if (isRangeDomain)
									{
										var rangeDomain = field.Domain as RangeDomain<DateTime>;										
										isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);
									}
									break;
								case Field.FieldType.Double:
								case Field.FieldType.Integer:
								case Field.FieldType.Single:
								case Field.FieldType.SmallInteger:
								case Field.FieldType.String:
									var control = FindChild<TextBox>(element) as TextBox;
									if (control == null)
										return;

									value = control.Text;

									// Attempt to convert string to field type primitive
									value = StringToPrimitiveTypeConverter.ConvertToType(field.Type, value);

									// Validate field type and validate if it can be nullable type																		
									isValidChange = FeatureValidationRule.IsValid(field.Type, field.Nullable, value, out errorContent);

									// if still valid value after first validation check move to next validation check
									if (isValidChange && isRangeDomain)
									{										
										switch (field.Type)
										{
											case Field.FieldType.Double:
												var rangeDomainDouble = field.Domain as RangeDomain<double>;
												isValidChange = RangeDomainValidationRule.isValid(rangeDomainDouble.MinimumValue, rangeDomainDouble.MaximumValue, value, out errorContent);
												break;
											case Field.FieldType.Integer:
												var rangeDomainInteger = field.Domain as RangeDomain<int>;
												isValidChange = RangeDomainValidationRule.isValid(rangeDomainInteger.MinimumValue, rangeDomainInteger.MaximumValue, value, out errorContent);
												break;
											case Field.FieldType.Single:
												var rangeDomainSingle = field.Domain as RangeDomain<float>;
												isValidChange = RangeDomainValidationRule.isValid(rangeDomainSingle.MinimumValue, rangeDomainSingle.MaximumValue, value, out errorContent);
												break;
											case Field.FieldType.SmallInteger:
												var rangeDomainShort = field.Domain as RangeDomain<short>;
												isValidChange = RangeDomainValidationRule.isValid(rangeDomainShort.MinimumValue, rangeDomainShort.MaximumValue, value, out errorContent);
												break;
										}
									}
									break;
								default:
									return; // none of the types we are looking for
							}
							// if all validation was passes then commit the value to the graphic
							// if validation has failed DataGrid control will properly handle the
							// validation error
							if (isValidChange && graphic.Attributes.ContainsKey(field.Name))
								graphic.Attributes[field.Name] = value;
						}
					}
				}
			}
		}

		private T FindChild<T>(DependencyObject parent) where T : DependencyObject
		{			
			if (parent is T || parent == null)
				return parent as T;

			int childCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				if (child is T)
					return child as T;

				bool HasChild = VisualTreeHelper.GetChildrenCount(child) > 0;
				if (HasChild)
					return FindChild<T>(child);
			}
			return null;
		}

		/// <summary>
		/// Intercepts the OnSort event and performs a custom sort on Graphic.Attributes
		/// </summary>		
		protected override void OnSorting(DataGridSortingEventArgs e)
		{
			base.OnSorting(e);									
		}				
#if NET35
		/// <summary>
		/// Before allowing the data grid to remove a record check to see if the
		/// Delete action can be executed.
		/// </summary>		
		protected override void OnCanExecuteDelete(CanExecuteRoutedEventArgs e)
		{
			if(e.CanExecute)
				base.OnCanExecuteDelete(e);
		}		
#endif
		#endregion Override		

		#region Private Methods
		/// <summary>
		/// Selected Graphics Count should reflect GraphicsLayer.SelectedGraphics.Count
		/// Update this binding if GraphicsLayer property changes
		/// </summary>
		private void SetSelectedGraphicBinding()
		{
			Binding binding = new Binding("SelectedGraphics.Count"); // Path is SelectedGraphics.Count
			binding.Mode = BindingMode.OneWay;
			binding.Source = GraphicsLayer; // Source is our GraphicsLayer
			this.SetBinding(SelectedGraphicCountProperty, binding);
		}
		/// <summary>
		/// Total Graphics Count should reflect GraphicsLayer.Graphics.Count
		/// Update this binding if GraphicsLayer property changes
		/// </summary>
		private void SetTotalGraphicsBinding()
		{
			Binding binding = new Binding("Graphics.Count"); // Path is Graphics.Count
			binding.Mode = BindingMode.OneWay;
			binding.Source = GraphicsLayer; // Source is our GraphicsLayer
			this.SetBinding(TotalGraphicCountProperty, binding);
		}
		/// <summary>
		/// Sets the ItemSource of the FeatureDataGrid.
		/// </summary>
		private void SetItemsSource(GraphicCollection graphics)
		{
			if(_collection != null)
			{
				if (_collection is INotifyCollectionChanged)
					(_collection as INotifyCollectionChanged).CollectionChanged -= FeatureCollectionView_CollectionChanged;
				_collection.CurrentChanged -= CollectionViewCurrentItemChanged;
			}
			_collection = new FeatureCollectionView(graphics ?? new GraphicCollection());
			_collection.CurrentChanged += CollectionViewCurrentItemChanged;
			if (_collection is INotifyCollectionChanged)
				(_collection as INotifyCollectionChanged).CollectionChanged += FeatureCollectionView_CollectionChanged;
			if (featureLayer != null)
				this._collection.LayerInfo = featureLayer.LayerInfo; // Uses coded value info during sort,filter and group

			ItemsSource = _collection; // Set the ItemsSource			
		}

		/// <summary>
		/// Raised when the FeatureCollectionView collection changes. 
		/// </summary>
		private void FeatureCollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				// The items source collection is about to get cleared causing the selections 
				// to be lost when the items are removed from DataGrid.SelectedItems collection
				// the current selected items need to be preserved.
				foreach (Graphic g in _collection)
				{
					if (g.Selected == true)
						_selected.Add(g); // dump current selected items into a temp list.
				}				
			}
		}

		/// <summary>
		/// Applies filter to the FeatureCollectionView only displaying graphics 
		/// that the FeatureCollectionView and the FilterSource have in common.
		/// </summary>
		private void ApplyFilter()
		{
			if (_collection != null)
			{
				// Apply filter. this will reset the selections of the data grid.
				if (FilterSource != null)
					_collection.Filter = (a) => { return this.FilterSource.Contains(a as Graphic); };
				else
					_collection.Filter = null;
			}
		}
		
		/// <summary>
		/// Used to lookup the Alias of a Field in a FeatureLayer. If not found 
		/// the FieldName will be returned this is a helper method for creating 
		/// Header text.
		/// </summary>
		/// <param name="name">Name of the field to find.</param>
		/// <returns>Returns the alias name if found. Returns the original name if not found.</returns> 
		private string AliasLookup(string name)
		{
			string result = name;
			if (featureLayer != null && featureLayer.LayerInfo != null)
			{
				Field field = featureLayer.LayerInfo.Fields.FirstOrDefault(x => x.Name == name);
				if (field != null)
					result = field.Alias;
			}
			return result;
		}
		/// <summary>
		/// Creates binding to SelectedGraphicsCount dependency property and 
		/// GraphicsLayer.SelectedGraphics.Count
		/// </summary>
		private void BindToSelectedGraphicsCount()
		{
			Binding binding = new Binding("SelectedGraphics.Count");
			binding.Mode = BindingMode.OneWay;
			binding.Source = GraphicsLayer;
			this.SetBinding(FeatureDataGrid.SelectedGraphicCountProperty, binding);
		}
		/// <summary>
		/// Creates binding to TotalGraphicsCount dependency property and 
		/// Graphicslayer.Graphics.Count
		/// </summary>
		private void BindToTotalGraphicsCount()
		{
			Binding binding = new Binding("Graphics.Count");
			binding.Mode = BindingMode.OneWay;
			binding.Source = GraphicsLayer;
			this.SetBinding(FeatureDataGrid.TotalGraphicCountProperty, binding);
		}

		/// <summary>
		/// Updates the Selected Row and Total Row text in the status bar.
		/// </summary>
		private void UpdateRecordsText()
		{
			if (numberOfRecordsTextBlock != null)
			{
				var totalRowCount = TotalGraphicCount;
				if (FilterSource != null && GraphicsLayer != null && GraphicsLayer.Graphics != null)
					totalRowCount = GraphicsLayer.Graphics.Intersect(FilterSource).Count();
				numberOfRecordsTextBlock.Text = string.Format(recordsText, SelectedGraphicCount, totalRowCount);
			}
		}
		/// <summary>
		/// Moves the current record to a specific index in the collection view.
		/// </summary>
		/// <param name="position">the index in the view.</param>
		/// <param name="reset">if set to <c>true</c> [reset].</param>
		private void MoveRecord(int position = 0, bool reset = false)
		{
			if (_collection != null)
			{
				if (reset)
					_collection.MoveCurrentToPosition(-1);
				else if (position <= 0)
					_collection.MoveCurrentToFirst();
				else if (position >= _collection.Count - 1)
					_collection.MoveCurrentToLast();
				else
					_collection.MoveCurrentToPosition(position);

				ScrollToCurrentRecord();
			}
		}

		/// <summary>
		/// Scrolls to the current record and updates the current item text 
		/// in the status bar.
		/// </summary>
		private void ScrollToCurrentRecord()
		{
			// Scroll current item into view
			if (_collection.CurrentItem != null)
				ScrollIntoView(_collection.CurrentItem);
		}

		private void CollectionViewCurrentItemChanged(object sender, EventArgs e)
		{
			UpdateCurrentRecordTextBox();
		}

		private void UpdateCurrentRecordTextBox()
		{		
			// if the FeatureDataGrid template contains CurrentRecordNumberTextBox
			if (currentRecordNumberTextBox != null)
			{
				var position = _collection == null ? 0 : _collection.CurrentPosition + 1;
				currentRecordNumberTextBox.Text = position.ToString();
			}
		}
		private static string GetRangeDomainValidation(Field field)
		{
			object minValue = null;
			object maxValue = null;
			if (field != null && field.Domain != null && !(field.Domain is CodedValueDomain))
			{
				switch (field.Type)
				{
					case Field.FieldType.Double:
						minValue = (field.Domain as RangeDomain<double>).MinimumValue;
						maxValue = (field.Domain as RangeDomain<double>).MaximumValue;
						break;
					case Field.FieldType.Integer:
						minValue = (field.Domain as RangeDomain<int>).MinimumValue;
						maxValue = (field.Domain as RangeDomain<int>).MaximumValue;
						break;
					case Field.FieldType.Single:
						minValue = (field.Domain as RangeDomain<float>).MinimumValue;
						maxValue = (field.Domain as RangeDomain<float>).MaximumValue;
						break;
					case Field.FieldType.SmallInteger:
						minValue = (field.Domain as RangeDomain<short>).MinimumValue;
						maxValue = (field.Domain as RangeDomain<short>).MaximumValue;
						break;
					case Field.FieldType.Date:
						minValue = (field.Domain as RangeDomain<DateTime>).MinimumValue;
						maxValue = (field.Domain as RangeDomain<DateTime>).MaximumValue;
						break;
				}
				if (minValue != null && maxValue != null)
					return string.Format("<local:RangeDomainValidationRule ValidationStep=\"ConvertedProposedValue\" MinValue=\"{0}\" MaxValue=\"{1}\" />", minValue, maxValue);
			}
			return string.Empty;
		}
		#endregion Private methods																						
	}	

	/// <summary>
	/// *FOR INTERNAL USE ONLY* This class is used to validate edits in the 
	/// DataGrid before saving them back to the Graphic attribute.
	/// </summary>
 /// <exclude/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class FeatureValidationRule : ValidationRule
	{
		/// <summary>
		/// Gets or sets the type of the field. Used to determine what type to use
		/// in validation.
		/// </summary>		
		public Field.FieldType? FieldType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether nulls are accepted as valid value
		/// during validation.
		/// </summary>		
		public bool Nullable
		{
			get { return nullable; }
			set { nullable = value; }
		}
		private bool nullable = true;

		/// <summary>
		/// When overridden in a derived class, performs validation checks on a value.
		/// </summary>
		/// <param name="value">The value from the binding target to check.</param>
		/// <param name="cultureInfo">The culture to use in this rule.</param>
		/// <returns>
		/// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
		/// </returns>
		public override System.Windows.Controls.ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			string errorContent = null;
			return new System.Windows.Controls.ValidationResult(IsValid(FieldType,Nullable,value, out errorContent), errorContent);
		}		

		internal static bool IsValid(Field.FieldType? FieldType, bool Nullable, object value, out string errorContent) 
		{
			errorContent = null;
			if (FieldType.HasValue)
			{				
				if (value == null)
					return Nullable;
				else
				{
					try
					{
						switch (FieldType)
						{
							case Field.FieldType.Double:
								if (value is string)
                                    value = double.Parse(value as string, System.Globalization.NumberStyles.Number);
								else if (!(value is double))
									return false;
								break;
							case Field.FieldType.Integer:
								if (value is string)
                                    value = int.Parse(value as string, System.Globalization.NumberStyles.Number);
								else if (!(value is int))
									return false;
								break;
							case Field.FieldType.Single:
								if (value is string)
                                    value = float.Parse(value as string, System.Globalization.NumberStyles.Number);
								else if (!(value is float))
									return false;
								break;
							case Field.FieldType.SmallInteger:
								if (value is string)
                                    value = short.Parse(value as string, System.Globalization.NumberStyles.Number);
								if (!(value is short))
									return false;
								break;
							case Field.FieldType.Date:
								if (!(value is DateTime))
									return false;
								break;
						}
					}
					catch(Exception ex)
					{
						errorContent = ex.Message;
						return false;
					}
				}
			}
			return true;
		}
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* Validates RangeDomains values in the FeatureDataGrid.
	/// </summary>
 /// <exclude/> 
	[EditorBrowsable(EditorBrowsableState.Never)]	
	public sealed class RangeDomainValidationRule : ValidationRule
	{
		/// <summary>
		/// Gets or sets the min value.
		/// </summary>
		/// <value>The min value.</value>
		public object MinValue { get; set; }
		/// <summary>
		/// Gets or sets the max value.
		/// </summary>
		/// <value>The max value.</value>
		public object MaxValue { get; set; }

		/// <summary>
		/// When overridden in a derived class, performs validation checks on a value.
		/// </summary>
		/// <param name="value">The value from the binding target to check.</param>
		/// <param name="cultureInfo">The culture to use in this rule.</param>		
		public override System.Windows.Controls.ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{						
			string errorContent = null;
			return new System.Windows.Controls.ValidationResult(isValid(this.MinValue,this.MaxValue,value, out errorContent), errorContent);
		}

		internal static bool isValid(object min, object max, object value, out string errorMessage)
		{
			string s_min = min.ToString();
			string s_max = max.ToString();
			try
			{
				if (value is double)
					RangeDomainValidator.IsInValidRange((double)value, s_min, s_max);
				else if (value is double?)
					RangeDomainValidator.IsInValidRange((double?)value, s_min, s_max);
				else if (value is int)
					RangeDomainValidator.IsInValidRange((int)value, s_min, s_max);
				else if (value is int?)
					RangeDomainValidator.IsInValidRange((int?)value, s_min, s_max);
				else if (value is float)
					RangeDomainValidator.IsInValidRange((float)value, s_min, s_max);
				else if (value is float?)
					RangeDomainValidator.IsInValidRange((float?)value, s_min, s_max);
				else if (value is short)
					RangeDomainValidator.IsInValidRange((short)value, s_min, s_max);
				else if (value is short?)
					RangeDomainValidator.IsInValidRange((short?)value, s_min, s_max);
				else if (value is long)
					RangeDomainValidator.IsInValidRange((long)value, s_min, s_max);
				else if (value is long?)
					RangeDomainValidator.IsInValidRange((long?)value, s_min, s_max);
				else if (value is DateTime)
					RangeDomainValidator.IsInValidRange((DateTime)value, s_min, s_max);
				else if (value is DateTime?)
					RangeDomainValidator.IsInValidRange((DateTime?)value, s_min, s_max);

			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return false;
			}
			errorMessage = null;
			return true;
		}
	}

	/// <summary>
	/// *FOR INTERNAL USE ONLY* Used to convert values from TextBox controls back
	/// to the original data type of the graphic attribute. This is used when 
	/// editing values in the FeatureDataGrid.
	/// </summary>	
 /// <exclude/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class StringToPrimitiveTypeConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the type of the field. The FieldType indicates what the
		/// type is for converting.
		/// </summary>
		/// <value>The type of the field.</value>
		public Field.FieldType? FieldType { get; set; }

		#region IValueConverter Members

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ConvertToType(FieldType, value);
		}

		internal static object ConvertToType(Field.FieldType? FieldType, object value)
		{
			if (FieldType.HasValue && value is string)
			{
				switch (FieldType)
				{
					case Field.FieldType.Double:
						return EnsureType<double?>((string)value);
					case Field.FieldType.Integer:
						return EnsureType<int?>((string)value);
					case Field.FieldType.Single:
						return EnsureType<float?>((string)value);
					case Field.FieldType.SmallInteger:
						return EnsureType<short?>((string)value);
					case Field.FieldType.Date:
						return EnsureType<DateTime?>((string)value);
					default:
						return value;
				}
			}
			return value;
		}

		/// <summary>
		/// Used to ensure original types are maintained when UI Elements change the underlying type. 
		/// (i.e. TextBox.Text changes double,integer,float,short,etc.. to string when displayed)
		/// when reading values form TextBox.Text back need to cast string back to original value before
		/// updating Graphic.Attributes otherwise FeatureLayer will throw and exception because a double will 
		/// be replaced with a string value.
		/// </summary>
		/// <typeparam name="T">The type to validate.</typeparam>
		/// <param name="str">the string to validate</param>
		/// <returns>returns validated type or null if validation fails.</returns>
		private static object EnsureType<T>(string str)
		{
			if (string.IsNullOrEmpty(str))
				return null;
			else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
			{                
				int outInt; 
				if (int.TryParse(str,System.Globalization.NumberStyles.Number,System.Globalization.CultureInfo.CurrentCulture, out outInt))
					return outInt;
			}
			else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
			{
                double outDouble; 
                if (double.TryParse(str, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out outDouble))
					return outDouble;
			}
			else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
			{
                float outFloat; 
                if (float.TryParse(str, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out outFloat))
					return outFloat;
			}
			else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
			{
                short outShort; 
                if (short.TryParse(str, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out outShort))
					return outShort;
			}
			else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
			{
				DateTime outDateTime;
				if (DateTime.TryParse(str, out outDateTime))
					return new DateTime(outDateTime.Ticks, DateTimeKind.Utc);
			}
			return str;
		}

		#endregion
	}
}
