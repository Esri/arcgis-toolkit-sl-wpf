// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        /// <summary>
        /// Used to Attached FeatureCollectionView schema information to a DataGridColumn for easy reference 
        /// useful to validate column and cell data such as DataType and FieldName.
        /// </summary>
        private static readonly DependencyProperty KeyColumnProperty =
            DependencyProperty.RegisterAttached("KeyColumn", typeof(KeyValuePair<string,DataType>), typeof(DataGridColumn), null);

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

	    private bool AddItems(IEnumerable newItems)
					{
	        var result = false;
	        if (newItems == null || cleanupGraphics == null) return false;
	        
            foreach (var item in newItems)
						{
	            cleanupGraphics.Add((Graphic)item);
	            ((Graphic)item).AttributeValueChanged += Graphic_AttributeValueChanged;
                if (result == false)
                {
	            foreach (var attribute in ((Graphic)item).Attributes)
	            {
	                if (ResetRequired(attribute)) 
                        {
                        result = true;                                                
                            break;
                        }                        
                    }
						}
					}
	        return result;
				}

	    private void RemoveItems(IEnumerable oldItems)
				{
	        if (oldItems == null || cleanupGraphics == null) return;

	        foreach (var item in oldItems)
					{
	            cleanupGraphics.Remove((Graphic)item);
	            ((Graphic)item).AttributeValueChanged -= Graphic_AttributeValueChanged;
					}
				}

	    private void ClearItems()
	    {
	        if (cleanupGraphics == null) return;
            cleanupGraphics.ForEach(g => g.AttributeValueChanged -= Graphic_AttributeValueChanged);            
            cleanupGraphics.Clear();
	    }

	    void Graphics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// if collection changes then need to check the attributes to make sure that
			// new fields were not added. this is only for non feature layer types.

	        if (cleanupGraphics == null) return;
			var reset = false;
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{                
                // Clear 
                if (cleanupGraphics.Count > 0  && GraphicsLayer.Graphics.Count == 0)
                    ClearItems();

                // Remove Range 
			    var oldItems = cleanupGraphics.Except(GraphicsLayer.Graphics);
                RemoveItems(oldItems);

                // Add Range
			    var newItems = GraphicsLayer.Graphics.Except(cleanupGraphics);			                   
                reset = AddItems(newItems);
			}
			else switch (e.Action)
			{
			    case NotifyCollectionChangedAction.Add: // single item added
			        reset = AddItems(e.NewItems);
			        break;
			    case NotifyCollectionChangedAction.Replace: // single item replaced
			        RemoveItems(e.OldItems);
			        reset = AddItems(e.NewItems);
			        break;
			    case NotifyCollectionChangedAction.Remove: // single item removed
			        RemoveItems(e.OldItems);
			        break;
			}
            
            // Need to add new column because a new item had 
            //a new attribute that we don't have a column for yet.
			if (reset)
				SetItemsSource(GraphicsLayer != null ? GraphicsLayer.Graphics : null);
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
            else if(featureLayer == null)
		{
                var fcv = ItemsSource as FeatureCollectionView;
                if(fcv != null)                
                   return fcv.IsResetRequired(attribute.Key, attribute.Value);                
			}
			return false;
		}

		void Graphic_AttributeValueChanged(object sender, Graphics.DictionaryChangedEventArgs e)
		{
			var attribute = new KeyValuePair<string, object>(e.Key, e.NewValue);
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                SetItemsSource(GraphicsLayer != null ? GraphicsLayer.Graphics : null);
		}

		void FeatureLayer_UpdateCompleted(object sender, EventArgs e)
		{		
			string outFields = (sender as FeatureLayer).OutFields.ToString();
			if (outFields != this.OutFields)
			{
				this.OutFields = outFields;
				SetItemsSource(GraphicsLayer != null ? GraphicsLayer.Graphics : null);
			}
		}		

		private void GraphicsLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Graphics")
			{
                var graphics = GraphicsLayer != null ? GraphicsLayer.Graphics : null;
				if (GraphicCollection != null)
				{
					GraphicCollection = graphics;
				} 
				SetItemsSource(graphics);		// Update the ItemsSource
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
                System.Globalization.CultureInfo culture = Language.GetSpecificCulture();
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
                        bool isDynamicField = FieldDomainUtils.IsDynamicDomain(field, featureLayer.LayerInfo);						                        
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
									if (isDynamicField)
									{										 
                                            var dynamicRangeDateDomains = FieldDomainUtils.BuildDynamicRangeDomain<DateTime>(field, featureLayer.LayerInfo);
											if (dynamicRangeDateDomains != null)
                                            {
												var rangeDomain = FieldDomainUtils.GetRangeDomain<DateTime>(featureLayer.LayerInfo.TypeIdField, graphic, dynamicRangeDateDomains);
                                                if(rangeDomain != null)
                                                    isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);                                                
                                            }
                                            break;
									}
									else if (isRangeDomain)
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
									value = StringToPrimitiveTypeConverter.ConvertToType(field.Type, null, value, culture);

									// Validate field type and validate if it can be nullable type																		
									isValidChange = FeatureValidationRule.IsValid(field.Type, null, field.Nullable, value, culture, out errorContent);

                                    if(isValidChange && isDynamicField)
                                    {
                                        switch (field.Type)
                                        {
                                            case Field.FieldType.Double:
                                                var dynamicRangeDoubleDomains = FieldDomainUtils.BuildDynamicRangeDomain<double>(field, featureLayer.LayerInfo);
                                                if (dynamicRangeDoubleDomains != null)
                                                {
                                                    var rangeDomain = FieldDomainUtils.GetRangeDomain<double>(featureLayer.LayerInfo.TypeIdField, graphic, dynamicRangeDoubleDomains);
                                                    if(rangeDomain != null)                                                    
                                                        isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);                                                    
                                                }
                                                break;
                                            case Field.FieldType.Integer:
                                                var dynamicRangeIntegerDomains = FieldDomainUtils.BuildDynamicRangeDomain<int>(field, featureLayer.LayerInfo);
                                                if (dynamicRangeIntegerDomains != null)
                                                {
                                                    var rangeDomain = FieldDomainUtils.GetRangeDomain<int>(featureLayer.LayerInfo.TypeIdField, graphic, dynamicRangeIntegerDomains);
                                                    if(rangeDomain != null)                                                   
                                                        isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);                                                    
                                                }
                                                break;
                                            case Field.FieldType.Single:
                                               var dynamicRangeSingleDomains = FieldDomainUtils.BuildDynamicRangeDomain<float>(field, featureLayer.LayerInfo);
                                               if (dynamicRangeSingleDomains != null)
                                                {
                                                    var rangeDomain = FieldDomainUtils.GetRangeDomain<float>(featureLayer.LayerInfo.TypeIdField, graphic, dynamicRangeSingleDomains);
                                                    if(rangeDomain != null)
                                                        isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);                                                    
                                                }
                                                break;
                                            case Field.FieldType.SmallInteger:
                                               var dynamicRangeShortDomains = FieldDomainUtils.BuildDynamicRangeDomain<short>(field, featureLayer.LayerInfo);
                                               if (dynamicRangeShortDomains != null)
                                                {
                                                    var rangeDomain = FieldDomainUtils.GetRangeDomain<short>(featureLayer.LayerInfo.TypeIdField, graphic, dynamicRangeShortDomains);
                                                    if(rangeDomain != null)
                                                        isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorContent);                                                    
                                                }
                                                break;
                                        }                                        
                                    }									
                                    else if (isValidChange && isRangeDomain)
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
                else if (GraphicsLayer is GraphicsLayer)
                {
                    bool isValidChange = true;
                    var kvp = (KeyValuePair<string, DataType>)e.Column.GetValue(KeyColumnProperty);
                    FrameworkElement element = e.EditingElement;
                    var fcv = ItemsSource as FeatureCollectionView;
                    if (element != null && fcv != null)
                    {
                        var fieldName = kvp.Key;
                        var dataType = kvp.Value;
                        var graphic = e.Row.DataContext as Graphic;
                        string errorContent = null;
                        object value = null;
                        switch (dataType)
                        {
                            case DataType.DateTime:
                                var datepicker = FindChild<DateTimePicker>(element) as DateTimePicker;
                                if (datepicker == null)
                                    return;
                                value = datepicker.SelectedDate;                                
                                break;
                            case DataType.String:
                            case DataType.Int16:
                            case DataType.Int32:
                            case DataType.Int64:
                            case DataType.Decimal:
                            case DataType.Single:
                            case DataType.Double:
                            case DataType.UInt16:
                            case DataType.UInt32:
                            case DataType.UInt64:
                                var control = FindChild<TextBox>(element) as TextBox;
                                if (control == null)
                                    return;

                                value = control.Text;

                                // Attempt to convert string to field type primitive
                                value = StringToPrimitiveTypeConverter.ConvertToType(null, dataType, value, culture);

                                // Validate field type and validate if it can be nullable type																		
                                isValidChange = FeatureValidationRule.IsValid(null, dataType, true, value, culture, out errorContent);                                
                                break;
                            default:
                                return; // none of the types we are looking for
                        }
                        // if all validation was passes then commit the value to the graphic
                        // if validation has failed DataGrid control will properly handle the
                        // validation error
                        if (isValidChange && graphic.Attributes.ContainsKey(fieldName))
                            graphic.Attributes[fieldName] = value;
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
			if (result.Contains('_'))
				result = result.Replace("_", "__"); // else _ are not displayed 
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
    /// This class is used to validate subtypes 
    /// that have range domain validation.
    /// </summary>        
    internal sealed class DynamicRangeDomainValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets the field info. Used to 
        /// determine what type to use in validation.
        /// </summary>		
        public Field Field { get; set; }

        /// <summary>
        /// Gets or sets the feature layer info. Used to dynamically
        /// selected the range domain need (if any) based on the sub
        /// type value.
        /// </summary>
        public FeatureLayerInfo LayerInfo { get; set; }

        /// <summary>
        /// Gets or sets the graphic being validated.
        /// </summary>
        public Graphic Graphic { get; set; }

        /// <summary>
		/// When overridden in a derived class, performs validation checks on a value.
		/// </summary>
		/// <param name="value">The value from the binding target to check.</param>
		/// <param name="cultureInfo">The culture to use in this rule.</param>		
		public override System.Windows.Controls.ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{						
			string errorContent = null;
			return new System.Windows.Controls.ValidationResult(isValid(this.Field, this.LayerInfo, this.Graphic, value, out errorContent), errorContent);
		}

		internal static bool isValid(Field field, FeatureLayerInfo layerInfo, Graphic graphic,  object value, out string errorMessage)
		{
            bool isValidChange = true;         
            bool isRangeDomain = (field.Domain != null && !(field.Domain is CodedValueDomain));
			bool isSubType = FieldDomainUtils.IsDynamicDomain(field, layerInfo);
            errorMessage = null;

            if (value == null)
                return field.Nullable;

			if (isSubType)
			{
				switch (field.Type)
				{
					case ESRI.ArcGIS.Client.Field.FieldType.Double:
						var dynamicRangeDoubleDomains = FieldDomainUtils.BuildDynamicRangeDomain<double>(field, layerInfo);
						if (dynamicRangeDoubleDomains != null)
						{
							var rangeDomain = FieldDomainUtils.GetRangeDomain<double>(layerInfo.TypeIdField, graphic, dynamicRangeDoubleDomains);
							if (rangeDomain != null)															
								isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorMessage);							
						}
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Integer:
						var dynamicRangeIntegerDomains = FieldDomainUtils.BuildDynamicRangeDomain<int>(field, layerInfo);
						if (dynamicRangeIntegerDomains != null)
						{
							var rangeDomain = FieldDomainUtils.GetRangeDomain<int>(layerInfo.TypeIdField, graphic, dynamicRangeIntegerDomains);
							if (rangeDomain != null)							
								isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorMessage);							
						}
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Single:
						var dynamicRangeSingleDomains = FieldDomainUtils.BuildDynamicRangeDomain<float>(field, layerInfo);
						if (dynamicRangeSingleDomains != null)
						{
							var rangeDomain = FieldDomainUtils.GetRangeDomain<float>(layerInfo.TypeIdField, graphic, dynamicRangeSingleDomains);
							if (rangeDomain != null)							
								isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorMessage);							
						}
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.SmallInteger:
						var dynamicRangeShortDomains = FieldDomainUtils.BuildDynamicRangeDomain<short>(field, layerInfo);
						if (dynamicRangeShortDomains != null)
						{
							var rangeDomain = FieldDomainUtils.GetRangeDomain<short>(layerInfo.TypeIdField, graphic, dynamicRangeShortDomains);
							if (rangeDomain != null)							
								isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorMessage);							
						}
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Date:
						var dynamicRangeDateDomains = FieldDomainUtils.BuildDynamicRangeDomain<DateTime>(field, layerInfo);
						if (dynamicRangeDateDomains != null)
						{
							var rangeDomain = FieldDomainUtils.GetRangeDomain<DateTime>(layerInfo.TypeIdField, graphic, dynamicRangeDateDomains);
							if (rangeDomain != null)
								isValidChange = RangeDomainValidationRule.isValid(rangeDomain.MinimumValue, rangeDomain.MaximumValue, value, out errorMessage);
						}
						break;
				}
			}
            else if (isRangeDomain)
			{										
				switch (field.Type)
				{
					case ESRI.ArcGIS.Client.Field.FieldType.Double:
						var rangeDomainDouble = field.Domain as RangeDomain<double>;
                        isValidChange = RangeDomainValidationRule.isValid(rangeDomainDouble.MinimumValue, rangeDomainDouble.MaximumValue, value, out errorMessage);
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Integer:
						var rangeDomainInteger = field.Domain as RangeDomain<int>;
                        isValidChange = RangeDomainValidationRule.isValid(rangeDomainInteger.MinimumValue, rangeDomainInteger.MaximumValue, value, out errorMessage);
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Single:
						var rangeDomainSingle = field.Domain as RangeDomain<float>;
                        isValidChange = RangeDomainValidationRule.isValid(rangeDomainSingle.MinimumValue, rangeDomainSingle.MaximumValue, value, out errorMessage);
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.SmallInteger:
						var rangeDomainShort = field.Domain as RangeDomain<short>;
                        isValidChange = RangeDomainValidationRule.isValid(rangeDomainShort.MinimumValue, rangeDomainShort.MaximumValue, value, out errorMessage);
						break;
					case ESRI.ArcGIS.Client.Field.FieldType.Date:
						var rangeDomainDate = field.Domain as RangeDomain<DateTime>;
						isValidChange = RangeDomainValidationRule.isValid(rangeDomainDate.MinimumValue, rangeDomainDate.MaximumValue, value, out errorMessage);
						break;
				}
			}
            return isValidChange;
		}	
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
        /// Gets or sets the data type of the field. Indicates what the type is 
        /// for converting if FieldType is not set. FieldType take priority if set.
        /// </summary>
        public DataType? dataType { get; set; }

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
			return new System.Windows.Controls.ValidationResult(IsValid(FieldType, dataType, Nullable,value,cultureInfo, out errorContent), errorContent);
		}		

		internal static bool IsValid(Field.FieldType? FieldType, DataType? dataType, bool Nullable, object value,CultureInfo culture, out string errorContent) 
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
                                    value = double.Parse(value as string, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture);
								else if (!(value is double))
									return false;
								break;
							case Field.FieldType.Integer:
								if (value is string)
                                    value = int.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
								else if (!(value is int))
									return false;
								break;
							case Field.FieldType.Single:
								if (value is string)
                                    value = float.Parse(value as string, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture);
								else if (!(value is float))
									return false;
								break;
							case Field.FieldType.SmallInteger:
								if (value is string)
                                    value = short.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
								if (!(value is short))
									return false;
								break;
							case Field.FieldType.Date:
								if (!(value is DateTime))
									return false;
								break;
                            case Field.FieldType.GUID:
                                if (value is string)
                                    value = Guid.Parse(value as string);
                                if (!(value is Guid))
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
            else if(dataType.HasValue)
            {
                if (value == null)
                    return Nullable;
                else
                {
                    try
                    {
                        switch (dataType)
                        {
                            case DataType.Int16:
                                if (value is string)
                                    value = short.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                if (!(value is short))
                                    return false;
                                break;
                            case DataType.Int32:
                                if (value is string)
                                    value = int.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                else if (!(value is int))
                                    return false;
                                break;
                            case DataType.Int64:
                                if (value is string)
                                    value = long.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                else if (!(value is long))
                                    return false;
                                break;
                            case DataType.Decimal:
                                if (value is string)
                                    value = decimal.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                else if (!(value is decimal))
                                    return false;
                                break;
                            case DataType.Single:
                                if (value is string)
                                    value = float.Parse(value as string, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture);
                                else if (!(value is float))
                                    return false;
                                break;
                            case DataType.Double:
                                if (value is string)
                                    value = double.Parse(value as string, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture);
                                else if (!(value is double))
                                    return false;
                                break;
                            case DataType.DateTime:
                                if (!(value is DateTime))
                                    return false;
                                break;
                            case DataType.UInt16:
                                if (value is string)
                                    value = UInt16.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                if (!(value is UInt16))
                                    return false;
                                break;
                            case DataType.UInt32:
                                if (value is string)
                                    value = UInt32.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                else if (!(value is UInt32))
                                    return false;
                                break;
                            case DataType.UInt64:
                                if (value is string)
                                    value = UInt64.Parse(value as string, System.Globalization.NumberStyles.Number, culture);
                                else if (!(value is UInt64))
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

        /// <summary>
        /// Gets or setws the data type of the field. Indicates waht the type is 
        /// for converting if FieldType is not set. FieldType take priority if set.
        /// </summary>
        public DataType? dataType {get; set;}

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
            if (value is Guid && targetType == typeof(string))
                return string.Format("{{{0}}}", value);
			return value; // Note: No need to convert the object to a string by using the culture because it's what WPF is doing by default
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
			return ConvertToType(FieldType, dataType, value, culture);
		}

		internal static object ConvertToType(Field.FieldType? fieldType, DataType? dataType, object value, CultureInfo culture)
		{
			if (fieldType.HasValue && value is string)
			{
				switch (fieldType)
				{
					case Field.FieldType.Double:
						return EnsureType<double?>((string)value, culture);
					case Field.FieldType.Integer:
						return EnsureType<int?>((string)value, culture);
					case Field.FieldType.Single:
						return EnsureType<float?>((string)value, culture);
					case Field.FieldType.SmallInteger:
						return EnsureType<short?>((string)value, culture);
					case Field.FieldType.Date:
						return EnsureType<DateTime?>((string)value, culture);
                    case Field.FieldType.GUID:
                        return EnsureType<Guid?>((string)value, culture);
					default:
						return value;
				}
			}
            else if (dataType.HasValue && value is string)
            {
                switch(dataType)
                {
                    case DataType.Int16:
                        return EnsureType<Int16?>((string)value, culture);
                    case DataType.Int32:
                        return EnsureType<Int32?>((string)value, culture);
                    case DataType.Int64:
                        return EnsureType<Int64?>((string)value, culture);
                    case DataType.Decimal:
                        return EnsureType<Decimal?>((string)value, culture);
                    case DataType.Single:
                        return EnsureType<Single?>((string)value, culture);
                    case DataType.Double:
                        return EnsureType<Double?>((string)value, culture);
                    case DataType.DateTime:
                        return EnsureType<DateTime?>((string)value, culture);
                    case DataType.UInt16:
                        return EnsureType<UInt16?>((string)value, culture);
                    case DataType.UInt32:
                        return EnsureType<UInt32?>((string)value, culture);
                    case DataType.UInt64:
                        return EnsureType<UInt64?>((string)value, culture);
                    
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
		/// <param name="culture">The culture to use for the validation</param>
		/// <returns>returns validated type or null if validation fails.</returns>
		private static object EnsureType<T>(string str, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(str))
				return null;
			else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
			{                
				int outInt; 
				if (int.TryParse(str,System.Globalization.NumberStyles.Number, culture, out outInt))
					return outInt;
			}
			else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
			{
                double outDouble; 
                if (double.TryParse(str, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture, out outDouble))
					return outDouble;
			}
			else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
			{
                float outFloat; 
                if (float.TryParse(str, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent, culture, out outFloat))
					return outFloat;
			}
			else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
			{
                short outShort; 
                if (short.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outShort))
					return outShort;
			}
			else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
			{
				DateTime outDateTime;
				if (DateTime.TryParse(str, culture, DateTimeStyles.None, out outDateTime))
					return new DateTime(outDateTime.Ticks, DateTimeKind.Utc);
			}
            else if(typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
            {
                Guid outGuid;
                if (Guid.TryParse(str, out outGuid))
                    return outGuid;
            }
            else if (typeof(T) == typeof(Decimal) || typeof(T) == typeof(Decimal?))
            {
                Decimal outDecimal;
                if (Decimal.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outDecimal))
                    return outDecimal;
            }
            else if (typeof(T) == typeof(Int64) || typeof(T) == typeof(Int64?))
            {
                Int64 outInt64;
                if (Int64.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outInt64))
                    return outInt64;
            }
            else if (typeof(T) == typeof(UInt16) || typeof(T) == typeof(UInt16?))
            {
                UInt16 outUInt16;
                if (UInt16.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outUInt16))
                    return outUInt16;
            }
            else if (typeof(T) == typeof(UInt32) || typeof(T) == typeof(UInt32?))
            {
                UInt32 outUInt32;
                if (UInt32.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outUInt32))
                    return outUInt32;
            }
            else if (typeof(T) == typeof(UInt64) || typeof(T) == typeof(UInt64?))
            {
                UInt64 outUInt64;
                if (UInt64.TryParse(str, System.Globalization.NumberStyles.Number, culture, out outUInt64))
                    return outUInt64;
            }
			return str;
		}

		#endregion
	}
}
