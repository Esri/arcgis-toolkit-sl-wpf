// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ESRI.ArcGIS.Client.Toolkit.Primitives
{
	/// <summary>
 /// *FOR INTERNAL USE ONLY* Represents a control that displays hierarchical data in a tree structure
	/// that has items that can expand and collapse.
	/// This control inherits from the TreeView control
	/// and adds a binding for the <see cref="P:System.Windows.Controls.TreeView.IsExpanded"/>
	/// and <see cref="P:System.Windows.Controls.TreeView.IsSelected"/> properties
	/// </summary>
 /// <exclude/>
 [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class TreeViewExtended : TreeView
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewExtended"/> class.
		/// </summary>
		public TreeViewExtended()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(TreeView);
#endif
		}
		/// <summary>
		/// Static initialization for the <see cref="TreeViewExtended"/> control.
		/// </summary>
		static TreeViewExtended()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeView),
				new FrameworkPropertyMetadata(typeof(TreeView)));
#endif
		}
		#endregion

		#region protected override DependencyObject GetContainerForItemOverride()
		/// <summary>
		/// Creates a <see cref="TreeViewItemExtended" /> to
		/// display content.
		/// </summary>
		/// <returns>
		/// A <see cref="TreeViewItemExtended" /> to use as a
		/// container for content.
		/// </returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			var itm = new TreeViewItemExtended();
			itm.SetBinding(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay });
			itm.SetBinding(TreeViewItem.IsSelectedProperty, new Binding("IsSelected") { Mode = BindingMode.TwoWay });
			return itm;
		} 
		#endregion

		#region protected override bool IsItemItsOwnContainerOverride(object item)
		/// <summary>
		/// Determines whether an object is a
		/// <see cref="TreeViewItemExtended" />.
		/// </summary>
		/// <param name="item">The object to evaluate.</param>
		/// <returns>
		/// True if <paramref name="item" /> is a
		/// <see cref="TreeViewItemExtended" />; otherwise,
		/// false.
		/// </returns>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeViewItemExtended;
		}
		#endregion
	}

	/// <summary>
	/// Provides an item for the <see cref="TreeViewExtended" /> control.
	/// This control inherits from the TreeViewItem control
	/// and adds a binding for the <see cref="P:System.Windows.Controls.TreeViewItem.IsExpanded"/>
	/// and <see cref="P:System.Windows.Controls.TreeViewItem.IsSelected"/> properties
	/// </summary>
	public sealed class TreeViewItemExtended : TreeViewItem
	{
		#region protected override DependencyObject GetContainerForItemOverride()
		/// <summary>
		/// Creates a <see cref="TreeViewItemExtended" /> to
		/// display content.
		/// </summary>
		/// <returns>
		/// A <see cref="TreeViewItemExtended" /> to use as a
		/// container for content.
		/// </returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			var itm = new TreeViewItemExtended();
			itm.SetBinding(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay });
			itm.SetBinding(TreeViewItem.IsSelectedProperty, new Binding("IsSelected") { Mode = BindingMode.TwoWay });
			return itm;
		}
		#endregion

		#region protected override bool IsItemItsOwnContainerOverride(object item)
		/// <summary>
		/// Determines whether an object is a
		/// <see cref="TreeViewItemExtended" />.
		/// </summary>
		/// <param name="item">The object to evaluate.</param>
		/// <returns>
		/// True if <paramref name="item" /> is a
		/// <see cref="TreeViewItemExtended" />; otherwise,
		/// false.
		/// </returns>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeViewItemExtended;
		}
		
		#endregion
	}
}
