// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using ESRI.ArcGIS.Client.Toolkit.Utilities;
using ESRI.ArcGIS.Client.FeatureService;
using System.Collections.ObjectModel;

namespace ESRI.ArcGIS.Client.Toolkit
{
	/// <summary>
	/// Collection View for sorting and filtering <see cref="Graphic">Graphics</see>.
	/// </summary>
	/// <remarks>
	/// FeatureCollectionview is a custom CollectionView class that extends a
	/// CollectionView to include sorting and filtering. FeatureCollectionView is 
	/// specifically designed to work with a collection of Graphic. SortDescriptors 
	/// can be added to sort the Graphics according to and Attribute in 
	/// Ascending/Descending order. A filter can be applied to the FeatureCollection 
	/// using a Predicate. see Predicate definition for more information on filter.
	/// </remarks>
	public sealed class FeatureCollectionView : CollectionView, IItemProperties, IEditableCollectionView, INotifyCollectionChanged
	{
		#region Private Properties
		private IList _internalList;										// view
		private IList _baseCollection;										// base collection
		private object _editItem;											// current edit item
		private NewItemPlaceholderPosition _newItemPlaceholderPosition;		// place holder for new item
		private SortDescriptionCollection _sortDescriptions;				// sorting instructions for view
		private GraphicComparer _activeComparer = new GraphicComparer();	// sort comparer		
		private Predicate<object> _filter;									// filter view with custom criteria
		#endregion Private Properties

		#region Constructor
		/// <summary>
		/// Creates an instance of <see cref="FeatureCollectionView"/>. This collection can be sorted and filtered
		/// </summary>
		/// <param name="collection"></param>
		public FeatureCollectionView(GraphicCollection collection)
			: base(collection)
		{
			// Create a default copy of the existing collection items. It is very 
			// important that _internalList is an instance that supports INotifyCollectionChanged
			// Any control bound to this class will need to update when _internalList is changed.
			_internalList = new GraphicCollection(collection);	// GraphicCollection supports INotifyCollectionChanged					
			
			// hold a reference to the base collection to determine if certain actions can be performed on the view
			_baseCollection = collection;

			// listen to changes to the base collection. Anytime the base collection changes
			// this view will need to refresh (rebuild the view based on new data).			
			if (collection is INotifyCollectionChanged)
				(collection as INotifyCollectionChanged).CollectionChanged += BaseSourceCollection_CollectionChanged;
		}				

		private void BaseSourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// if the underlying collection changes our view needs to be re-created		
			RefreshOrDefer();
		}
		#endregion Constructor

		#region Public Properties
		/// <summary>
		/// FeatureLayerInfo if present will be used in the sorting, filtering
		/// and Grouping processes. Coded value domains will be sorted, filtered
		/// and grouped according to the display text not the underlying code.
		/// </summary>
		/// <example>
		/// Coded Value: [[0,New][1,Open][2,Closed]] with the FeatureLayerInfo 
		/// sorting will be performed on New,Open and Closed which is the display text
		/// and not on 0,1,2 which is the code.
		/// </example>
		public FeatureLayerInfo LayerInfo { get; set; }		

		/// <summary>
		/// Can use predicate to filter graphics based on a custom criteria.
		/// </summary>	
		/// <returns>A delegate that represents the method used to determine if an item is suitable for inclusion in the view.</returns>		
		public override Predicate<object> Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				RefreshOrDefer();
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Re-creates the view.
		/// </summary>
		protected override void RefreshOverride()
		{
			object currentItem = this.CurrentItem;
			int num = this.IsEmpty ? -1 : this.CurrentPosition;
			bool isCurrentAfterLast = this.IsCurrentAfterLast;
			bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
			base.OnCurrentChanging();

			// Copy the base collection to list that will be used to sort, 
			// filter and group result for our view

			#region Sorting, Filtering and Grouping
			// Filter out the results that do not need to be in the view						
			var list = ApplyFilter(base.SourceCollection as IEnumerable<Graphic>);

			// TODO: Grouping logic. this will most likely cause a change in
			// sorting logic.			
			// if(grouping)
			//{
			//  then group and sort here. sorting is different when applied
			//  to a grouped list.
			//}
			//else // just sort normally. 
			// {			
			list = ApplySort(list);
			// }
			#endregion Sorting, filtering and Grouping

			ResetInternalSource(list); // repopulates the view.

			// if the current item is before the first item in the collection or
			// if the collection is empty, set the current item to null and 
			// current position to -1.
			if (isCurrentBeforeFirst || IsEmpty)
			{
				// update the current item and current index.
				SetCurrent(null, -1);
			}
			// else if the current item is beyond the last item in the collection
			// set current item to null and index to the index of (LastItemIndex + 1)
			// this is the location of the NewItemPlaceHolder.
			else if (isCurrentAfterLast)
			{
				// set current item to null and index to +1 position beyond the range of the collection
				SetCurrent(null, _internalList.Count);
			}
			else
			{
				// get the index of the current item within the view
				int index = _internalList.IndexOf(currentItem);
				if (index < 0)
				{
					object obj3;
					index = ((this as IEditableCollectionView).NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) ? 1 : 0;
					if ((index < _internalList.Count) && ((obj3 = _internalList.IndexOf(index)) != CollectionView.NewItemPlaceholder))
					{
						SetCurrent(obj3, index);
					}
					else
					{
						// if current item can not be found within the view set the current
						// item to null and the current index to -1. 
						SetCurrent(null, -1);
					}
				}
				else
				{
					// set current item and index
					SetCurrent(currentItem, index);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			OnCurrentChanged();
			if (isCurrentAfterLast != IsCurrentAfterLast)
			{
				OnPropertyChanged("IsCurrentAfterLast");
			}
			if (isCurrentBeforeFirst != IsCurrentBeforeFirst)
			{
				OnPropertyChanged("IsCurrentBeforeFirst");
			}
			if (num != CurrentPosition)
			{
				OnPropertyChanged("CurrentPosition");
			}
			if (currentItem != CurrentItem)
			{
				OnPropertyChanged("CurrentItem");
			}
		}		

		/// <summary>
		/// Returns an enumeration of items from the view, which may be filtered, sorted
		/// and grouped.
		/// </summary>		
		protected override IEnumerator GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		/// <summary>
		/// This is the property that all bindings will use as the ItemsSource.		
		/// </summary>
		public override IEnumerable SourceCollection
		{
			get
			{
				// returns the view that can be sorted and filtered
				return _internalList;
			}
		}

		/// <summary>
		/// Indicates if this collection view supports sorting. 
		/// </summary>
		public override bool CanSort
		{
			get
			{
				// sorting is supported
				return true;
			}
		}
		/// <summary>
		/// Indicates if the collection view supports filtering.
		/// </summary>
		public override bool CanFilter
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// The sort description is used to apply sorting to the view
		/// </summary>
		public override SortDescriptionCollection SortDescriptions
		{
			get
			{
				// if private member is null create a new instance 
				if (_sortDescriptions == null)
				{
					// create a new instance
					_sortDescriptions = new SortDescriptionCollection();
					// listen for changes to the collection
					(_sortDescriptions as INotifyCollectionChanged).CollectionChanged += SortDescriptions_CollectionChanged;
				}
				return _sortDescriptions; // return the sort descriptions collection
			}
		}

		#endregion Overrides

		/// <summary>
		/// When the sort descriptions collections change we need to 
		/// rebuild the view to reflect the changes 
		/// </summary>		
		private void SortDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// if sort descriptions changes then the view needs to be re-created.
			RefreshOrDefer();
		}

		#region Helper Methods

		/// <summary>
		/// Raises on property changed event
		/// </summary>		
		private void OnPropertyChanged(string propertyName)
		{
			// raise the property changed event
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}				

		/// <summary>
		/// Finds the index of an item in the view.
		/// </summary>		
		private int InternalIndexOf(object item)
		{
			int num = _internalList.IndexOf(item);
			if (((this as IEditableCollectionView).NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning) && (num >= 0))
			{
				num += (this as IEditableCollectionView).IsAddingNew ? 2 : 1;
			}
			return num;
		}

		/// <summary>
		/// Filters the specified list.
		/// </summary>		
		private IEnumerable<Graphic> ApplyFilter(IEnumerable<Graphic> list)
		{
			if (list != null)
			{
				foreach (var item in list)
				{
					if (Filter != null)
					{
						if (Filter(item)) yield return item;
					}
					else
					{
						yield return item;
					}
				}
			}
		}

		/// <summary>
		/// Performs multiple sorts based on the sort descriptions collections. 
		/// sorts a list by multiple fields in ascending and descending directions
		/// </summary>	
		private IEnumerable<Graphic> ApplySort(IEnumerable<Graphic> list)
		{
			if (list != null)
			{
				// perform a sort for each sort description
				foreach (SortDescription sort in SortDescriptions)
				{
					// call the sort for this sort description
					list = ApplySort(list, sort);
				}
			}
			return list; // return list after all sorts are performed.
		}
		/// <summary>
		/// Performs a sort on a list according to a sort descriptions. Sort 
		/// descriptions contains the field to sort by and the direction to sort
		/// the collection. the _activeComparer is responsible for the sort logic.
		/// </summary>	
		private IEnumerable<Graphic> ApplySort(IEnumerable<Graphic> list, SortDescription sort)
		{
			if (list != null)
			{
				// check to make sure sort and comparer are not null
				if (sort != null && _activeComparer != null)
				{
					_activeComparer.Field = sort.PropertyName; // field to sort by
					_activeComparer.LayerInfo = this.LayerInfo; // layer info for sorting coded value domain types
					list = sort.Direction == ListSortDirection.Ascending ? list.OrderBy(x => x, _activeComparer) : list.OrderByDescending(x => x, _activeComparer);
				}
			}
			return list; // returns sorted list
		}

		/// <summary>
		/// Clears the view and re-creates and new view based on a list of items
		/// </summary>		
		private void ResetInternalSource(IEnumerable list)
		{						
			// clear the existing items
			_internalList.Clear();

			// repopulate the list with items that will be the new view
			foreach (object o in list)
				_internalList.Add(o);		
		}
		#endregion Helper Methods

		#region IItemProperties Members

		ReadOnlyCollection<ItemPropertyInfo> IItemProperties.ItemProperties
		{
			get
			{
				List<ItemPropertyInfo> itemsInfo = new List<ItemPropertyInfo>();
				if (LayerInfo != null && LayerInfo.Fields != null)
				{
					LayerInfo.Fields.Where(f => f != null).ForEach(f =>
					{
						itemsInfo.Add(new ItemPropertyInfo(f.Name, typeof(object), null));
					});
				}
				else
				{
					List<string> uniqueAttributes = new List<string>();
					IEnumerator enumerator = this.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Graphic graphic = enumerator.Current as Graphic;
						foreach (string key in graphic.Attributes.Keys)
						{
							if (uniqueAttributes.Contains(key))
								continue;
							uniqueAttributes.Add(key);

							itemsInfo.Add(new ItemPropertyInfo(key, typeof(object), null));
						}
					}
				}
				return new ReadOnlyCollection<ItemPropertyInfo>(itemsInfo);
			}
		}

		#endregion

		#region IEditableCollectionView Members

		/// <summary>
		/// Adds a new item to the collection.
		/// </summary>
		/// <returns>
		/// The new item that is added to the collection.
		/// </returns>
		object IEditableCollectionView.AddNew()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets a value that indicates whether a new item can be added to the collection.
		/// </summary>
		/// <value></value>
		/// <returns>true if a new item can be added to the collection; otherwise, false.</returns>
		bool IEditableCollectionView.CanAddNew
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value that indicates whether the collection view can discard 
		/// pending changes and restore the original values of an edited object.
		/// </summary>
		/// <value></value>
		/// <returns>true if the collection view can discard pending changes and
		/// restore the original values of an edited object; otherwise, false.</returns>
		bool IEditableCollectionView.CanCancelEdit
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value that indicates whether an item can be removed from the collection.
		/// </summary>
		/// <value></value>
		/// <returns>true if an item can be removed from the collection; otherwise, false.</returns>
		bool IEditableCollectionView.CanRemove
		{
			get
			{
				// if base collection is not null and it contains the IList interface
				// then items can be removed because of IList.Remove() and IList.RemoveAt()
				return (_baseCollection != null && _baseCollection is IList);
			}
		}

		/// <summary>
		/// Ends the edit transaction and, if possible, restores the original value to the item.
		/// </summary>
		void IEditableCollectionView.CancelEdit()
		{
			_editItem = null;
		}

		/// <summary>
		/// Ends the add transaction and discards the pending new item.
		/// </summary>
		void IEditableCollectionView.CancelNew()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Ends the edit transaction and saves the pending changes.
		/// </summary>
		void IEditableCollectionView.CommitEdit()
		{
			_editItem = null;
		}

		/// <summary>
		/// Ends the add transaction and saves the pending new item.
		/// </summary>
		void IEditableCollectionView.CommitNew()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the item that is being added during the current add transaction.
		/// </summary>
		/// <value></value>
		/// <returns>The item that is being added if <see cref="P:System.ComponentModel.IEditableCollectionView.IsAddingNew"/> is true; otherwise, null.</returns>
		object IEditableCollectionView.CurrentAddItem
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the item in the collection that is being edited.
		/// </summary>
		/// <value></value>
		/// <returns>The item in the collection that is being edited if <see cref="P:System.ComponentModel.IEditableCollectionView.IsEditingItem"/> is true; otherwise, null.</returns>
		object IEditableCollectionView.CurrentEditItem
		{
			get { return _editItem; }
		}

		/// <summary>
		/// Begins an edit transaction of the specified item.
		/// </summary>
		/// <param name="item">The item to edit.</param>
		void IEditableCollectionView.EditItem(object item)
		{
			_editItem = item;
		}

		/// <summary>
		/// Gets a value that indicates whether an add transaction is in progress.
		/// </summary>
		/// <value></value>
		/// <returns>true if an add transaction is in progress; otherwise, false.</returns>
		bool IEditableCollectionView.IsAddingNew
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value that indicates whether an edit transaction is in progress.
		/// </summary>
		/// <value></value>
		/// <returns>true if an edit transaction is in progress; otherwise, false.</returns>
		bool IEditableCollectionView.IsEditingItem
		{
			get { return _editItem != null; }
		}

		/// <summary>
		/// Gets or sets the position of the new item placeholder in the collection view.
		/// </summary>
		/// <value></value>
		/// <returns>One of the enumeration values that specifies the position of
		/// the new item placeholder in the collection view.</returns>
		NewItemPlaceholderPosition IEditableCollectionView.NewItemPlaceholderPosition
		{
			get
			{
				return _newItemPlaceholderPosition;
			}
			set
			{
				_newItemPlaceholderPosition = value;
			}
		}
		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		void IEditableCollectionView.Remove(object item)
		{
			// if the base collection contains the same item as the view then remove.
			if (_baseCollection.Contains(item))
				_baseCollection.Remove(item);
		}

		/// <summary>
		/// Removes the item at the specified position from the collection.
		/// </summary>
		/// <param name="index">The position of the item to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than 0 or greater than the number 
		/// 	of items in the collection view.</exception>
		void IEditableCollectionView.RemoveAt(int index)
		{
			// get the item from the views based on index. the indexes of the view
			// and the base collection may not be the same if the view is sorted,
			// filtered or grouped. 
			object item = _internalList[index];
			// check to see if the item in the view exist in the base collection.
			if (_baseCollection.Contains(item))
				_baseCollection.Remove(item); // if found remove the the base collection.
		}

		#endregion		

		/// <summary>
		/// Used for comparing two graphics on an attribute
		/// </summary>
		private class GraphicComparer : IComparer<Graphic>
		{
			private bool isDynamicCodedValue = false;
			private bool isCodedValue = false;
			private bool isTypeIDField = false;
			DynamicCodedValueSource dynamicCodedValueSource = null;
			CodedValueSources codedValueSources = null;
			private string TypeIDField;
			private System.Collections.Comparer comparer = new System.Collections.Comparer(System.Globalization.CultureInfo.CurrentCulture);

			private string _field;
			public string Field
			{
				get { return _field; }
				set
				{
					if (_field == value)
						return;
					_field = value;
					Initialize();
				}
			}
			private FeatureLayerInfo _layerInfo;
			public FeatureLayerInfo LayerInfo
			{
				get { return _layerInfo; }
				set
				{
					if (_layerInfo == value)
						return;
					_layerInfo = value;
					Initialize();
				}
			}

			public GraphicComparer() { }
			public GraphicComparer(string Field) { this.Field = Field; }
			public GraphicComparer(string Field, FeatureLayerInfo LayerInfo) { this.Field = Field; this.LayerInfo = LayerInfo; }

			private void Initialize()
			{
				isDynamicCodedValue = false;
				isCodedValue = false;
				isTypeIDField = false;
				dynamicCodedValueSource = null;
				codedValueSources = null;
				TypeIDField = string.Empty;

				if (string.IsNullOrEmpty(_field) || _layerInfo == null)
					return;

				TypeIDField = _layerInfo.TypeIdField;

				Field field = _layerInfo.Fields.FirstOrDefault(x => x.Name == _field);
				isDynamicCodedValue = FieldDomainUtils.IsDynamicDomain(field, LayerInfo);
				isCodedValue = (field.Domain is CodedValueDomain);
				isTypeIDField = (field.Name == TypeIDField);

				if (isDynamicCodedValue)
					dynamicCodedValueSource = FieldDomainUtils.BuildDynamicCodedValueSource(field, _layerInfo);
				else if (isTypeIDField)
					codedValueSources = FieldDomainUtils.BuildTypeIDCodedValueSource(field, _layerInfo);
				else if (isCodedValue)
					codedValueSources = FieldDomainUtils.BuildCodedValueSource(field);
			}

			#region IComparer Members

			/// <summary>
			/// Compares the specified x.
			/// </summary>
			/// <param name="x">The first graphic.</param>
			/// <param name="y">The second graphic.</param>
			/// <returns>
			/// -1 if less than, 1 is greater than and 0 if equal.</returns>
			int IComparer<Graphic>.Compare(Graphic x, Graphic y)
			{
				if (Field == null) return 0;
				Graphic g1 = x as Graphic;
				Graphic g2 = y as Graphic;
				if (g1 == null && g2 == null) // both null return equal
					return 0;
				else if (g1 != null && g2 == null) // y is null return greater than
					return 1;
				else if (g1 == null && g2 != null) // x is null return less than
					return -1;
				else if (!g1.Attributes.ContainsKey(Field) && g2.Attributes.ContainsKey(Field)) // x does not contain the key return less than
					return -1;
				else if (g1.Attributes.ContainsKey(Field) && !g2.Attributes.ContainsKey(Field)) // y does not contain the key return greater than 
					return 1;
				if (isDynamicCodedValue)
				{
					if (!g1.Attributes.ContainsKey(TypeIDField) & g2.Attributes.ContainsKey(TypeIDField))
						return -1;
					else if (g1.Attributes.ContainsKey(TypeIDField) & !g2.Attributes.ContainsKey(TypeIDField))
						return 1;
				}
				return this.Compare(g1, g2); // neither are null and both contain the attribute to evaluate.
			}

			internal int Compare(Graphic x, Graphic y)
			{
				var o1 = x.Attributes[Field]; // get primitive value
				var o2 = y.Attributes[Field]; // get primitive value

				if (isDynamicCodedValue)
				{
					o1 = DynamicCodedValueSource.CodedValueNameLookup(TypeIDField, Field, x, dynamicCodedValueSource);
					o2 = DynamicCodedValueSource.CodedValueNameLookup(TypeIDField, Field, y, dynamicCodedValueSource);
				}
				if (isCodedValue || isTypeIDField)
				{
					o1 = CodedValueSources.CodedValueNameLookup(Field, x, codedValueSources);
					o2 = CodedValueSources.CodedValueNameLookup(Field, y, codedValueSources);
				}

				return comparer.Compare(o1, o2); // compare primitives
			}
			#endregion

		}
	}
	
}
