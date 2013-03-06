// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Toolkit.Utilities;
using System.Windows.Threading;

namespace ESRI.ArcGIS.Client.Toolkit
{
	//Silverlight specific code for FeatureDataGrid
	public partial class FeatureDataGrid : DataGrid
	{
		private int SelectedGraphicCount								// Holds number of selected records
		{
			get
			{
				if (SelectedGraphics == null)
					return 0;
				return SelectedGraphics.Count;
			}
		}
		private int recordsCount										// Holds total number of records in the FeatureDataGrid
		{
			get
			{
				if (ItemsSource == null)
					return 0;
				return ItemsSource.AsList().Count;
			}
		}
		private int currentRecordNumber = 0;                            // Holds current record index
		private bool isSelectionChangedFromFeatureDataGrid = false;     // To determine whether last selection made from FeatureDataGrid

		private Type objectType;                                        // The object type holding FeatureDataGrid contents
		private object datePickerDataContext = null;

		private FeatureLayerInfo featureLayerInfo = null;
		private Dictionary<string, Type> fieldInfo = null;
		private FeatureLayer featureLayer = null;
		private Dictionary<string, string> fieldAliasMapping = null;    // Mapping between actual header titles and the titles changed to field aliases
		private string columnForCellBeingEdited = null;
		private Dictionary<string, object[]> rangeDomainInfo = null;
		private DispatcherTimer throttler = null;
		private DateTime startTime;

		DataGridRowsPresenter rowsPresenter = null;

		private IEnumerable<Graphic> currentGraphicCollection = null;

		private bool isCreatingItemsSource = false;						// Flag to indicate that the ItemsSource is just getting created

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FeatureDataGrid"/> class.
		/// </summary>
		public FeatureDataGrid()
		{
			DefaultStyleKey = typeof(FeatureDataGrid);
			throttler = new DispatcherTimer();
			throttler.Interval = new TimeSpan(0, 0, 0, 0, 500);
			throttler.Tick += DispatcherTimer_Tick;			
		}

		void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			if (startTime != null && (DateTime.Now.Ticks - startTime.Ticks) >= 500)
			{
				throttler.Stop();
				// Synchronizing selections in current DataGrid's page and the associated GraphicsLayer:
				RestorePreviousSelection(GraphicsLayer.SelectedGraphics);			
			}			
		}

		#endregion

		#region Properties
		private IEnumerable<Graphic> Graphics
		{
			get
			{
				if (FilterSource == null)
					foreach (Graphic graphic in GraphicsLayer.Graphics)
						yield return graphic;
				else
				{
					foreach (Graphic graphic in FilterSource)
					{
						if (GraphicsLayer.Graphics.Contains(graphic))
							yield return graphic;
					}
				}
			}
		}
		#endregion

		#region Dependency Properties

		private static void OnGraphicsLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			GraphicsLayer oldValue = e.OldValue as GraphicsLayer;
			GraphicsLayer newValue = e.NewValue as GraphicsLayer;
			if (oldValue != null)
			{
					oldValue.PropertyChanged -= grid.GraphicsLayer_PropertyChanged;
				#region Mouse Hovering Support
				oldValue.MouseEnter -= grid.GraphicsLayer_MouseEnter;
				oldValue.MouseLeave -= grid.GraphicsLayer_MouseLeave;
				#endregion
				grid.UnregisterGraphicCollectionEventHandlers();
				grid.currentGraphicCollection = null;

				// Setting the FeatureDataGrid's ItemsSource property to null:
				grid.ItemsSource = null;
				grid.ResetLayout();
			}
			if (newValue != null)
			{
				#region Mouse Hovering Support
				newValue.MouseEnter += grid.GraphicsLayer_MouseEnter;
				newValue.MouseLeave += grid.GraphicsLayer_MouseLeave;
				#endregion
				if (!newValue.IsInitialized)
				{
					EventHandler<EventArgs> handler = null;
					handler = new EventHandler<EventArgs>(
						delegate(object s, EventArgs args) { grid.PopulateItemsSource(s as GraphicsLayer, handler); });
					newValue.Initialized += handler;
				}
				else
					grid.PopulateItemsSource(newValue, null);
			}
			// Restoring previously selected graphics (if any):
			if (grid.GraphicsLayer != null)
				grid.RestorePreviousSelection(grid.GraphicsLayer.SelectedGraphics);
		}

		private static void OnFilterSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FeatureDataGrid grid = d as FeatureDataGrid;
			IEnumerable<Graphic> oldValue = e.OldValue as IEnumerable<Graphic>;
			IEnumerable<Graphic> newValue = e.NewValue as IEnumerable<Graphic>;
			if (oldValue != null && oldValue is ObservableCollection<Graphic>)
				(oldValue as ObservableCollection<Graphic>).CollectionChanged -= grid.FilterSource_CollectionChanged;
			if (newValue != null && newValue is ObservableCollection<Graphic>)
				(newValue as ObservableCollection<Graphic>).CollectionChanged += grid.FilterSource_CollectionChanged;

			if (grid.GraphicsLayer != null)
				grid.SetItemsSource((grid.GraphicsLayer != null && grid.GraphicsLayer.Graphics != null) ? (IList<Graphic>)grid.GraphicsLayer.Graphics : new List<Graphic>());
			else
				grid.ItemsSource = null;
			grid.ResetLayout();
			// Restoring previously selected graphics (if any):
			if (grid.GraphicsLayer != null)
				grid.RestorePreviousSelection(grid.GraphicsLayer.SelectedGraphics);
		}
		#endregion

		#region FeatureDataGrid Overriden and Initializing Methods

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.DataGrid.RowEditEnded"/> event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
		{
			base.OnRowEditEnded(e);

			if (e.EditAction == DataGridEditAction.Commit)
			{
				Graphic relatedGraphic = DataSourceCreator.GetGraphicSibling(e.Row.DataContext);
				if (relatedGraphic != null)
				{
					try
					{
						e.Row.RefreshGraphic(relatedGraphic, objectType);
					}
					catch { }
				}
			}
		}		

		private static string XmlEncode(string xml)
		{
			return xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.DataGrid.PreparingCellForEdit"/> event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
		{
			base.OnPreparingCellForEdit(e);

			FrameworkElement frameworkElement = e.Column.GetCellContent(e.Row);
			Field associatedField = e.Column.GetValue(FieldColumnProperty) as Field;
			if (associatedField != null)
			{
				string columnHeader = associatedField.Name;
				DatePicker datePicker = frameworkElement as DatePicker;

				if (datePicker != null)
				{
					columnForCellBeingEdited = columnHeader;
					datePickerDataContext = datePicker.DataContext;
					datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
				}
			}
		}	
		#endregion
		
		/// <summary>
		/// Validates current record number and sets an appropriate value based upon the accepted range.
		/// </summary>
		private void ValidateCurrentRecordNumber()
		{
			if (currentRecordNumber >= recordsCount)
				currentRecordNumber = recordsCount - 1;
			else if (currentRecordNumber < 0)
				currentRecordNumber = 0;
		}
		/// <summary>
		/// Selects current record in the <see cref="FeatureDataGrid"/>.
		/// </summary>
		private void SelectCurrentRecord()
		{
			if (recordsCount == 0)
				return;

			IList gridRows = ItemsSource.AsList();

			ValidateCurrentRecordNumber();
			SelectedItem = gridRows[currentRecordNumber];
			ScrollGridRowIntoView(SelectedItem);
		}
		
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.DataGrid.CellEditEnded"/> event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
		{			
			if (e.EditAction == DataGridEditAction.Commit)
			{
				Field field = e.Column.GetValue(FieldColumnProperty) as Field;
				if(field != null && featureLayer != null && featureLayer.LayerInfo != null)
				{
					if (field.Domain is CodedValueDomain 
						|| featureLayer.LayerInfo.TypeIdField == field.Name 
						|| FieldDomainUtils.IsDynamicDomain(field, featureLayer.LayerInfo))
					{
						// When cell edit ends update the graphic with the cell change.
						var xObject = e.Row.DataContext;
						var graphic = DataSourceCreator.GetGraphicSibling(xObject);
						xObject.RefreshGraphic(graphic, xObject.GetType());
					}
				}
			}
			base.OnCellEditEnded(e);		
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.DataGrid.CellEditEnding"/> event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
		{			
			if (e.EditAction == DataGridEditAction.Commit)
			{
				// If the original value is null and the textbox value is an 
				// empty string, then the null value should be preserved instead
				// of saving an empty string over the null value.
				TextBox tb = e.EditingElement as TextBox; 
				if (tb != null)
				{
					var tempClass = e.Row.DataContext;
					if (tempClass != null)
					{

						Field field = e.Column.GetValue(FieldColumnProperty) as Field;
						if (field != null)
						{
							Type type = tempClass.GetType();
							System.Reflection.PropertyInfo propertyInfo = type.GetProperty(field.Name.MappedKey());
							if (propertyInfo != null)
							{
								var value = propertyInfo.GetValue(tempClass, null);
								if (value == null && string.IsNullOrEmpty(tb.Text))
									this.CancelEdit();
							}
						}
					}
				}
			}
			base.OnCellEditEnding(e);
		}

		#region The Action Area's Methods and Events
		/// <summary>
		/// Selects all rows and graphic features.
		/// </summary>
		private void SelectAll()
		{
			if (ItemsSource == null || recordsCount == SelectedGraphicCount)
				return;
			
			foreach (object gridRow in ItemsSource)
				SelectedItems.Add(gridRow);
		}										
		#endregion

		#region Private Methods
		private void SetLayoutAndInternalVariables()
		{
			ShowNumberOfRecords();
			SetSubmitButtonVisibility();
			SetDeleteSelectedRowsMenuButtonEnableState();
		}

		/// <summary>
		/// Sets the text in current record number text box.
		/// </summary>
		private void SetCurrentRecordNumberTextBox()
		{
			if (currentRecordNumberTextBox != null)
				currentRecordNumberTextBox.Text = (currentRecordNumber + 1).ToString();
		}

		/// <summary>
		/// Sets the text in number of records text block.
		/// </summary>
		/// <param name="countSelected">number of selected records.</param>
		/// <param name="countTotal">total number of records.</param>
		private void SetNumberOfRecordsTextBlock(int countSelected, int countTotal)
		{
			if (numberOfRecordsTextBlock != null)
				numberOfRecordsTextBlock.Text = string.Format(recordsText, countSelected, countTotal);
		}

		/// <summary>
		/// Shows number of selected records and total number of records in the grid row.
		/// </summary>
		private void ShowNumberOfRecords()
		{
			SetNumberOfRecordsTextBlock(SelectedGraphicCount, recordsCount);
		}

		/// <summary>
		/// Sets the <see cref="FeatureDataGrid"/>'s ItemsSource after converting the source parameter to 
		/// the proper format.
		/// </summary>
		/// <param name="graphics">The graphics.</param>
		private void SetItemsSource(IEnumerable<Graphic> graphics)
		{
			Dictionary<string, Field> fieldProps = null;
			string uniqueID = null;

			if (featureLayer == null)
				featureLayer = GraphicsLayer as FeatureLayer;
			if (featureLayer != null)
			{
				featureLayerInfo = featureLayer.LayerInfo;
				SetSubmitButtonVisibility();

				if (featureLayer.LayerInfo != null)
				{
					fieldInfo = FieldDomainUtils.SetFieldInfo(featureLayerInfo, out rangeDomainInfo, out fieldProps);
					uniqueID = featureLayerInfo.ObjectIdField;
				}
			}
			else
			{
				fieldInfo = null;
				rangeDomainInfo = null;
			}
			// Indicate that the ItemsSource is about to be created by setting the isCreatingItemsSource flag.
			// We have to do this for both Silverlight and WPF as PagedCollectionView and CollectionViewSource 
			// always add the first object to their selection:
			isCreatingItemsSource = true;

			var enumerableGraphics = graphics.ToDataSource(fieldInfo, rangeDomainInfo, fieldProps, uniqueID, FilterSource, out objectType) as IEnumerable<object>;
			if (enumerableGraphics.Count<object>() == 0)
			{
				// use this when collection is empty, because it shows the column headers. 
				// PagedCollectionView does not create the headers for the collection it contains 
				// when the collection is empty.
				ItemsSource = enumerableGraphics; 
			}
			else
			{
				ItemsSource = new PagedCollectionView(enumerableGraphics);				
				(ItemsSource as PagedCollectionView).CollectionChanged += PagedCollectionView_CollectionChanged;
			}
			if(GraphicsLayer != null)
				RestorePreviousSelection(GraphicsLayer.SelectedGraphics);
			ShowNumberOfRecords();
		}	

		private void PagedCollectionView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (!throttler.IsEnabled)
				throttler.Start();
			startTime = DateTime.Now;
			currentRecordNumber = GetRowIndexInRowsCollection(CurrentItem);
			SetCurrentRecordNumberTextBox();
		}


		/// <summary>
		/// Gets the corresponding row in <see cref="FeatureDataGrid"/> for the graphic.
		/// </summary>
		/// <param name="graphic">The graphic.</param>
		/// <returns></returns>
		private object GetCorrespondingGridRow(Graphic graphic)
		{
			if (graphic != null && ItemsSource != null)
			{
				foreach (var item in ItemsSource)
					if (DataSourceCreator.GetGraphicSibling(item) == graphic)
						return item;
			}

			return null;
		}

		private void Graphic_AttributeValueChanged(object sender, ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				Graphic updatedGraphic = sender as Graphic;
				if (updatedGraphic != null)
				{
					if (e.NewValue != null)
					{
						// DateTime data types must be of kind UTC. DatePicker always sets 
						// its SelectedDate to Unspecified hence; conversion is necessary:
						if (e.NewValue.GetType() == typeof(DateTime?) || e.NewValue.GetType() == typeof(DateTime))
						{
							updatedGraphic.AttributeValueChanged -= Graphic_AttributeValueChanged;
							updatedGraphic.Attributes[e.Key] = new DateTime((e.NewValue as DateTime?).Value.Ticks, (e.NewValue as DateTime?).Value.Kind);
							updatedGraphic.AttributeValueChanged += Graphic_AttributeValueChanged;
						}
					}
					Dispatcher.BeginInvoke((Action)delegate()
					{
						RefreshRow(updatedGraphic);
					});
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
					 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (featureLayer != null)
				{
					// Only update attributes that exist in out fields of feature layer.				
					Tasks.OutFields outFields = featureLayer.OutFields;
					if (outFields == null || (!outFields.Contains("*") && !outFields.Contains(e.Key.MappedKey())))
						return;
					if (outFields.Contains("*"))
					{
						// if the field name does not exist in the service layer info ignore the field
						var field = featureLayer.LayerInfo.Fields.FirstOrDefault(x => x.FieldName == e.Key.MappedKey());
						if (field == null)
							return;
					}
				}
				SetItemsSource((GraphicsLayer != null && GraphicsLayer.Graphics != null) ? (IList<Graphic>)GraphicsLayer.Graphics : new List<Graphic>());
			}
		}

		private void RegisterGraphicCollectionEventHandlers()
		{
			if (currentGraphicCollection != null)
			{
				if (currentGraphicCollection is ObservableCollection<Graphic>)
					(currentGraphicCollection as ObservableCollection<Graphic>).CollectionChanged += Graphics_CollectionChanged;
				foreach (Graphic graphic in currentGraphicCollection)
				{
					graphic.AttributeValueChanged += Graphic_AttributeValueChanged;
				}
			}
		}

		private void UnregisterGraphicCollectionEventHandlers()
		{
			if (currentGraphicCollection != null)
			{
				if (currentGraphicCollection is ObservableCollection<Graphic>)
					(currentGraphicCollection as ObservableCollection<Graphic>).CollectionChanged -= Graphics_CollectionChanged;
				foreach (Graphic graphic in currentGraphicCollection)
				{
					graphic.AttributeValueChanged -= Graphic_AttributeValueChanged;
				}
			}
		}

		private void GraphicsLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Graphics")
			{
				GraphicsLayer graphicsLayer = sender as GraphicsLayer;
				// Resetting internal graphic collection and respective event handlers:
				UnregisterGraphicCollectionEventHandlers();
				currentGraphicCollection = (sender as GraphicsLayer).Graphics;
				RegisterGraphicCollectionEventHandlers();

				SetItemsSource((GraphicsLayer != null && GraphicsLayer.Graphics != null) ? (IList<Graphic>)GraphicsLayer.Graphics : new List<Graphic>());
				ResetLayout();
				// Restoring previously selected graphics (if any):
				if (GraphicsLayer != null)
					RestorePreviousSelection(GraphicsLayer.SelectedGraphics);
			}
			else if (e.PropertyName == "SelectionCount")
			{
				if (!isSelectionChangedFromFeatureDataGrid && SelectedItems != null && SelectedGraphics != null)
				{
					if (SelectedGraphics.Count == 0)
						SelectedItems.Clear();
					var selectedItemsCount = SelectedItems.Count;
					for (int i = selectedItemsCount - 1; i >= 0; i--)
					{
						var g = DataSourceCreator.GetGraphicSibling(SelectedItems[i]);
						if (!g.Selected)
							SelectedItems.Remove(SelectedItems[i]);
					}
					foreach (var g in SelectedGraphics)
					{
						var r = GetCorrespondingGridRow(g);
						if (r != null && !SelectedItems.Contains(r))
							SelectedItems.Add(r);
					}
				}
				ShowNumberOfRecords();
			}
			else if (e.PropertyName == "HasEdits")
				SetSubmitButtonEnableState();		
		}
		
		private void ResetLayout()
		{
			SetDeleteSelectedRowsMenuButtonEnableState();
			// Setting FeatureDataGrid layout and its internal variables:
			SetLayoutAndInternalVariables();
		}

		/// <summary>
		/// Populates associated GraphicsLayer's PropertyChanged event handler (if it's not a FeatureLayer) to update 
		/// contents of the FeatureDataGrid when its Graphics collection changes. Also, checks whether the layer is a 
		/// FeatureLayer and populates the internal LayerInfo and EndSaveEdits event handlers for editing purposes.
		/// At the end sets the ItemsSource of FeatureDataGrid and resets its internal variables states and values, and
		/// if it's called as a result of the Initialized event handler of GraphicsLayer unregisters the handler.
		/// </summary>
		/// <param name="graphicsLayer">The GraphicsLayer.</param>
		/// <param name="handler">The Initialized event handler to be unregistered.</param>
		private void PopulateItemsSource(GraphicsLayer graphicsLayer, EventHandler<EventArgs> handler)
		{
			featureLayer = graphicsLayer as FeatureLayer;
			if (featureLayer != null)
			{
				featureLayerInfo = featureLayer.LayerInfo;
			}
			GraphicsLayer.PropertyChanged += GraphicsLayer_PropertyChanged;

			UnregisterGraphicCollectionEventHandlers();
			currentGraphicCollection = graphicsLayer.Graphics;
			RegisterGraphicCollectionEventHandlers();
			SetItemsSource((GraphicsLayer != null && GraphicsLayer.Graphics != null) ? (IList<Graphic>)GraphicsLayer.Graphics : new List<Graphic>());
			ResetLayout();
			// Restoring previously selected graphics (if any):
			if (GraphicsLayer != null)
				RestorePreviousSelection(GraphicsLayer.SelectedGraphics);

			if (handler != null)
				graphicsLayer.Initialized -= handler;
		}

		private void FilterSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (FilterSource != null)
			{
				UpdateItemsSource(sender, e);
				ResetLayout();
			}
		}

		/// <summary>
		/// Will be fired whenever there were any changes in graphics collection 
		/// of the GraphicsLayer to update <see cref="FeatureDataGrid"/>'s ItemsSource.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Graphics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (FilterSource != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
				SetItemsSource((GraphicsLayer != null && GraphicsLayer.Graphics != null) ? (IList<Graphic>)GraphicsLayer.Graphics : new List<Graphic>());
			if (FilterSource == null)
			{
				UnregisterGraphicCollectionEventHandlers();
				UpdateItemsSource(sender, e);
				RegisterGraphicCollectionEventHandlers();
			}
		}

		private bool AllAttributesMatch(System.Reflection.PropertyInfo[] itemsSourceProperties, IDictionary<string, object> graphicAttributes)
		{
			//Check if type of each attribute matches object properties
			foreach (System.Reflection.PropertyInfo prop in itemsSourceProperties)
			{
				if (graphicAttributes.ContainsKey(prop.Name))	// Check existance of each property in attributes
				{
					if (graphicAttributes[prop.Name] != null && !graphicAttributes[prop.Name].GetType().IsOfType(prop.PropertyType))	// data types NOT the same?
					{
						RegisterGraphicCollectionEventHandlers();
						throw new InvalidCastException(string.Format(Properties.Resources.FeatureDataGrid_MixedAttributeTypesNotAllowed,
																	 prop.PropertyType, prop.Name));
					}
				}
			}
			//check if attributes contains a key not available as a property on object
			foreach (KeyValuePair<string, object> attribute in graphicAttributes)
			{
				bool match = false;
				foreach (System.Reflection.PropertyInfo prop in itemsSourceProperties)	// Check whether attributes are not contained in the property
				{
					if (attribute.Key == prop.Name)
					{
						match = true;
						continue;
					}
				}
				if (!match)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Updates contents of the ItemsSource when FeatureDataGrid's associated graphic collection changes.
		/// </summary>
		/// <param name="sender">Observable collection of Graphic.</param>
		/// <param name="e">Collection changed event arguments.</param>
		private void UpdateItemsSource(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			IList newItems = e.NewItems;
			IList oldItems = e.OldItems;
			if (ItemsSource == null)
			{
				SetItemsSource(sender as ObservableCollection<Graphic>);
			}
			else
			{
				if ((ItemsSource as PagedCollectionView) != null)
				{
					object currentItem = (ItemsSource as PagedCollectionView).CurrentItem;
					if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
					{
						var features = (from object a in ItemsSource select DataSourceCreator.GetGraphicSibling(a));
						newItems = (sender as ObservableCollection<Graphic>).Except(features).ToList();
						oldItems = features.Except(sender as ObservableCollection<Graphic>).ToList();
					}
					if (newItems != null && newItems.Count > 0)     // New item(s) added
					{
						bool shouldResetItemsSource = false;

						IEnumerator enumItemsSource = ItemsSource.GetEnumerator();
						if (enumItemsSource != null)
						{
							if (enumItemsSource.MoveNext())
							{								
								if (!AllAttributesMatch(enumItemsSource.Current.GetType().GetProperties(), (newItems[0] as Graphic).Attributes))
									shouldResetItemsSource = true;								
							}
							else
								shouldResetItemsSource = true;
						}						

						if (shouldResetItemsSource)
						{
							UnregisterGraphicCollectionEventHandlers();
							SetItemsSource(sender as ObservableCollection<Graphic>);
							IEnumerator enumAddedGraphics = newItems.GetEnumerator();							
							while (enumAddedGraphics.MoveNext())
							{
								Graphic graphic = enumAddedGraphics.Current as Graphic;								
								if (graphic != null)
								{
									graphic.AttributeValueChanged += Graphic_AttributeValueChanged;
								}
							}							
						}
						else
						{							
							IEnumerator enumAddedGraphics = newItems.GetEnumerator();
							List<Graphic> selected = GraphicsLayer.SelectedGraphics.ToList();
							while (enumAddedGraphics.MoveNext())
							{
								Graphic graphic = enumAddedGraphics.Current as Graphic;
								if (graphic != null)
								{
									if (graphic.Selected)														
										selected.Add(graphic);									
									ItemsSource.AddToDataSource(graphic, objectType);
									graphic.AttributeValueChanged += Graphic_AttributeValueChanged;
								}
							}							
							RestorePreviousSelection(selected);
						}
					}
					if (oldItems != null && oldItems.Count > 0)     // Item(s) removed
					{
						int selCount = SelectedItems.Count;
						// In Silverlight removing a graphic from the GraphicsCollection causes to lose current 
						// selection in both GraphicsLayer and the FeatureDataGrid.
						// Preserving selected items in the FeatureDataGrid:
						List<Graphic> selItems = new List<Graphic>(selCount);
						for (int i = 0; i < selCount; i++)
						{
							var row = SelectedItems[i];
							var graphic = DataSourceCreator.GetGraphicSibling(row);
							selItems.Add(graphic);
						}
						IEnumerator enumRemovedGraphics = oldItems.GetEnumerator();
						while (enumRemovedGraphics.MoveNext())
						{
							Graphic graphic = enumRemovedGraphics.Current as Graphic;
							int idxInItemsSource = GetRowIndexInItemsSource(graphic);
							if (graphic != null && idxInItemsSource > -1)
							{																								
								if (graphic != null)
									selItems.Remove(graphic);
								ItemsSource.RemoveFromDataSource(idxInItemsSource, objectType);
								// RemoveFromDataSource() method causes first item in the ItemsSource to be selected when there were no 
								// items selected in the data grid. We should avoid this selection by removing it from the selection:
								if (selCount == 0 && SelectedItems.Count == 1)
									SelectedItems.Clear();
								graphic.AttributeValueChanged -= Graphic_AttributeValueChanged;
								SelectedGraphics.Remove(graphic);
							}
						}
						RestorePreviousSelection(selItems);
					}					
				}
				else
				{
					// If exiting ItemsSource is not a PagedCollectionView then it was empty before
					// just populate it with everything in GraphicsLayer.
					SetItemsSource((GraphicsLayer != null && GraphicsLayer.Graphics != null) ? (IList<Graphic>)GraphicsLayer.Graphics : new List<Graphic>());
				}
			}
			ShowNumberOfRecords();
		}

		/// <summary>
		/// Finds the row in the input parameter then scrolls the data gird vertically to 
		/// make it visible to the user.
		/// </summary>
		/// <param name="item"></param>
		private void ScrollGridRowIntoView(object item)
		{
			ScrollIntoView(item, null);
		}

		/// <summary>
		/// Finds and returns the given graphics in the graphics collection 
		/// in the GraphicsLayer.
		/// </summary>
		/// <param name="graphic"></param>
		/// <returns></returns>
		private int GetGraphicIndexInGraphicsCollection(Graphic graphic)
		{
			if (Graphics != null)
			{
				int idx = -1;
				foreach (Graphic g in Graphics)
				{
					idx++;
					if (g.Equals(graphic))
						return idx;
				}
			}

			return -1;
		}

		/// <summary>
		/// Selects/deselects related graphic objects in the GraphicsLayer 
		/// when related grid rows have been selected/deselected by the user.
		/// </summary>
		/// <param name="rowsToLookup"></param>
		/// <param name="shouldSelectGraphics"></param>
		private void SelectGraphics(IList rowsToLookup, bool shouldSelectGraphics)
		{
			foreach (object objRow in rowsToLookup)
			{
				Graphic graphic = DataSourceCreator.GetGraphicSibling(objRow);
				if (graphic != null && GraphicsLayer != null && GraphicsLayer.Contains(graphic))
				{
					graphic.Selected = shouldSelectGraphics;

					if (shouldSelectGraphics)
						currentRecordNumber = GetRowIndexInRowsCollection(objRow);
					else
					{
						if (SelectedGraphics.Count == 0)
							currentRecordNumber = -1;
					}
				}
			}
		}

		/// <summary>
		/// Finds and returns index of the given row in the ItemsSource 
		/// object of the <see cref="FeatureDataGrid"/>.
		/// </summary>
		/// <param name="gridRow"></param>
		/// <returns></returns>
		private int GetRowIndexInRowsCollection(object gridRow)
		{
			if (gridRow == null)
				return -1;

			int idx = -1;
			IEnumerable enumItemsSource = ItemsSource as ICollectionView;
			foreach (object obj in enumItemsSource)
			{
				idx++;
				if (obj.Equals(gridRow))
					return idx;
			}

			return -1;
		}

		private void SetFieldAliasMapping(DataGridColumn dgColumn, string fieldName, string fieldAlias)
		{
			if (fieldAlias != null && fieldAlias != "")
			{
				if (fieldAliasMapping == null)
					fieldAliasMapping = new Dictionary<string, string>();

				if (!fieldAliasMapping.ContainsKey(fieldAlias))
					fieldAliasMapping.Add(fieldAlias, fieldName);
			}
		}
			
		private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			bool wasDatePickerDataContextNull = false;
			DatePicker datePicker = sender as DatePicker;

			if (datePicker != null && datePicker.DataContext == null)
			{
				wasDatePickerDataContextNull = true;
				datePicker.DataContext = datePickerDataContext;
			}

			if (datePicker != null && datePicker.DataContext != null &&
				columnForCellBeingEdited != null && columnForCellBeingEdited != "")
			{
				try
				{
					if (!e.AddedItems[0].Equals(datePicker.DataContext.GetType().GetProperty(columnForCellBeingEdited).GetValue(datePicker.DataContext, null)))
					{
						DataSourceCreator.SetProperty(e.AddedItems[0], datePicker.DataContext,
													  datePicker.DataContext.GetType().GetProperty(columnForCellBeingEdited));
						Graphic correspondingGraphic = DataSourceCreator.GetGraphicSibling(datePicker.DataContext);
						if (correspondingGraphic != null &&
							correspondingGraphic.Attributes.ContainsKey(columnForCellBeingEdited))
						{
							// Unsubscribing from graphic's AttributeValueChanged event to manually refresh corresponding row:
							correspondingGraphic.AttributeValueChanged -= Graphic_AttributeValueChanged;
							DateTime? selectedDate = e.AddedItems[0] as DateTime?;
							DateTime? dateToSet = null;
							if (selectedDate != null)
								dateToSet = new DateTime(selectedDate.Value.Ticks, selectedDate.Value.Kind);
							correspondingGraphic.Attributes[columnForCellBeingEdited] = dateToSet;
							if (wasDatePickerDataContextNull)
							{
								correspondingGraphic.RefreshRow(ItemsSource, GetGraphicIndexInGraphicsCollection(correspondingGraphic), objectType);
								datePickerDataContext = null;
							}
							// Subscribing back to the graphic's AttributeValueChanged event:
							correspondingGraphic.AttributeValueChanged += Graphic_AttributeValueChanged;
						}
					}
				}
				catch { }
			}
		}		
			
		#region Mouse Hovering Support
		private void GoToVisualState(Graphic graphic, string stateName)
		{
			if (rowsPresenter != null && rowsPresenter.Children != null)
			{
				foreach (UIElement element in rowsPresenter.Children)
				{
					DataGridRow row = element as DataGridRow;
					if (row != null)
					{
						Graphic g = DataSourceCreator.GetGraphicSibling(row.DataContext);
						if (graphic.Equals(g))
						{
							VisualStateManager.GoToState(row, stateName, true);
							break;
						}
					}
				}
			}
		}
		private void GraphicsLayer_MouseEnter(object sender, GraphicMouseEventArgs e)
		{
			if (e.Graphic != null)
			{
				if (e.Graphic.Selected)
					GoToVisualState(e.Graphic, "MouseOverSelected");
				else
					GoToVisualState(e.Graphic, "MouseOver");
			}
		}
		private void GraphicsLayer_MouseLeave(object sender, GraphicMouseEventArgs e)
		{
			if (e.Graphic != null)
			{
				if (e.Graphic.Selected)
					GoToVisualState(e.Graphic, "NormalSelected");
				else
					GoToVisualState(e.Graphic, "Normal");
			}
		}
		#endregion

		private void RestorePreviousSelection(IEnumerable<Graphic> selectedGraphics)
		{
			if (selectedGraphics != null && selectedGraphics.GetEnumerator().MoveNext())
			{
				foreach (Graphic graphic in selectedGraphics)
				{
					object correspondingGridRow = GetCorrespondingGridRow(graphic);
					if (correspondingGridRow != null)
					{
						Dispatcher.BeginInvoke((Action)delegate()
						{
							// Make sure that the corresponding grid row is contained in the PagedCollectionView ItemsSource:
							if (ItemsSource is PagedCollectionView)
							{
							    if ((ItemsSource as PagedCollectionView).Contains(correspondingGridRow))
							        SelectedItems.Add(correspondingGridRow);
							}
							else
							    SelectedItems.Add(correspondingGridRow);
						});
					}
				}
				// Get the new position of the current item after the sorting has finished.
				// update the current record textbox to display the new location.
				currentRecordNumber = GetRowIndexInRowsCollection(CurrentItem);
				SetCurrentRecordNumberTextBox();
			}			
		}

		private bool AreEqual(object item, Graphic graphic)
		{
			return graphic.Equals(DataSourceCreator.GetGraphicSibling(item));
		}
		private int GetRowIndexInItemsSource(Graphic graphic)
		{
			int retVal = -1;
			int indexInItemsSource = -1;

			IEnumerable enumItemsSource = (ItemsSource as ICollectionView).SourceCollection;
			foreach (object item in enumItemsSource)
			{
				indexInItemsSource++;
				if (AreEqual(item, graphic))
				{
					retVal = indexInItemsSource;
					break;
				}
			}

			return retVal;
		}
		private int GetRowIndexInItemsSource(object item)
		{
			int retVal = -1;
			int indexInItemsSource = -1;
			if (Graphics != null)
			{
				foreach (Graphic graphic in Graphics)
				{
					indexInItemsSource++;
					if (AreEqual(item, graphic))
					{
						retVal = indexInItemsSource;
						break;
					}
				}
			}

			return retVal;
		}
		#endregion			

        #region Public Methods
        /// <summary>
        /// Get a row from the FeatureDataGrid and returns the matching
        /// Graphic for the row.
        /// </summary>
        /// <param name="row">Row object from FeatureDataGrid.ItemsSource collection</param>
        /// <returns>Graphic that represented by a row object</returns>
        public Graphic GetGraphicFromRow(object row)
        {
            return DataSourceCreator.GetGraphicSibling(row);
        }
        #endregion

    }
}
