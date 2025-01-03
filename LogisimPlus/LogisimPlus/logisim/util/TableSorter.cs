﻿// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;

// retrieved from http://ouroborus.org/java/2.1/TableSorter.java
namespace logisim.util
{



	/// <summary>
	/// TableSorter is a decorator for TableModels; adding sorting functionality to a supplied TableModel. TableSorter does
	/// not store or copy the data in its TableModel; instead it maintains a map from the row indexes of the view to the row
	/// indexes of the model. As requests are made of the sorter (like getValueAt(row, col)) they are passed to the
	/// underlying model after the row numbers have been translated via the internal mapping array. This way, the TableSorter
	/// appears to hold another copy of the table with the rows in a different order.
	/// <p/>
	/// TableSorter registers itself as a listener to the underlying model, just as the JTable itself would. Events recieved
	/// from the model are examined, sometimes manipulated (typically widened), and then passed on to the TableSorter's
	/// listeners (typically the JTable). If a change to the model has invalidated the order of TableSorter's rows, a note of
	/// this is made and the sorter will resort the rows the next time a value is requested.
	/// <p/>
	/// When the tableHeader property is set, either by using the setTableHeader() method or the two argument constructor,
	/// the table header may be used as a complete UI for TableSorter. The default renderer of the tableHeader is decorated
	/// with a renderer that indicates the sorting status of each column. In addition, a mouse listener is installed with the
	/// following behavior:
	/// <ul>
	/// <li>Mouse-click: Clears the sorting status of all other columns and advances the sorting status of that column
	/// through three values: {NOT_SORTED, ASCENDING, DESCENDING} (then back to NOT_SORTED again).
	/// <li>SHIFT-mouse-click: Clears the sorting status of all other columns and cycles the sorting status of the column
	/// through the same three values, in the opposite order: {NOT_SORTED, DESCENDING, ASCENDING}.
	/// <li>CONTROL-mouse-click and CONTROL-SHIFT-mouse-click: as above except that the changes to the column do not cancel
	/// the statuses of columns that are already sorting - giving a way to initiate a compound sort.
	/// </ul>
	/// <p/>
	/// This class first appeared in the swing table demos in 1997 (v1.5) and then had a major rewrite in 2004 (v2.0) to make
	/// it compatible with Java 1.4.
	/// <p/>
	/// This rewrite makes the class compile cleanly with Java 1.5 while maintaining backward compatibility with TableSorter
	/// v2.0.
	/// 
	/// @author Philip Milne
	/// @author Brendon McLean
	/// @author Dan van Enckevort
	/// @author Parwinder Sekhon
	/// @author ouroborus@ouroborus.org
	/// @version 2.1 04/29/06
	/// </summary>

	public class TableSorter : AbstractTableModel
	{
		protected internal TableModel tableModel;

		public const int DESCENDING = -1;
		public const int NOT_SORTED = 0;
		public const int ASCENDING = 1;

		private static Directive EMPTY_DIRECTIVE = new Directive(-1, NOT_SORTED);

		public static readonly IComparer<object> COMPARABLE_COMPARATOR = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer<object>
		{
			public int Compare(object o1, object o2)
			{
				System.Reflection.MethodInfo m;
				try
				{
					// See if o1 is capable of comparing itself to o2
					m = o1.GetType().getDeclaredMethod("compareTo", o2.GetType());
				}
				catch (NoSuchMethodException)
				{
					throw new System.InvalidCastException();
				}

				object retVal;
				try
				{
					// make the comparison
					retVal = m.invoke(o1, o2);
				}
				catch (IllegalAccessException)
				{
					throw new System.InvalidCastException();
				}
				catch (InvocationTargetException)
				{
					throw new System.InvalidCastException();
				}

				// Comparable.compareTo() is supposed to return int but invoke()
				// returns Object. We can't cast an Object to an int but we can
				// cast it to an Integer and then extract the int from the Integer.
				// But first, make sure it can be done.
				int? i = 0;
				if (!i.GetType().IsInstanceOfType(retVal))
				{
					throw new System.InvalidCastException();
				}

				return i.GetType().cast(retVal).intValue();
			}
		}

		public static readonly IComparer<object> LEXICAL_COMPARATOR = new ComparatorAnonymousInnerClass2();

		private class ComparatorAnonymousInnerClass2 : IComparer<object>
		{
			public int Compare(object o1, object o2)
			{
				return string.CompareOrdinal(o1.ToString(), o2.ToString());
			}
		}

		private Row[] viewToModel;
		private int[] modelToView;

		private JTableHeader tableHeader;
		private MouseListener mouseListener;
		private TableModelListener tableModelListener;
		private Dictionary<Type, IComparer<object>> columnComparators = new Dictionary<Type, IComparer<object>>();
		private List<Directive> sortingColumns = new List<Directive>();

		public TableSorter()
		{
			this.mouseListener = new MouseHandler(this);
			this.tableModelListener = new TableModelHandler(this);
		}

		public TableSorter(TableModel tableModel) : this()
		{
			TableModel = tableModel;
		}

		public TableSorter(TableModel tableModel, JTableHeader tableHeader) : this()
		{
			TableHeader = tableHeader;
			TableModel = tableModel;
		}

		private void clearSortingState()
		{
			viewToModel = null;
			modelToView = null;
		}

		public virtual TableModel TableModel
		{
			get
			{
				return tableModel;
			}
			set
			{
				if (this.tableModel != null)
				{
					this.tableModel.removeTableModelListener(tableModelListener);
				}
    
				this.tableModel = value;
				if (this.tableModel != null)
				{
					this.tableModel.addTableModelListener(tableModelListener);
				}
    
				clearSortingState();
				fireTableStructureChanged();
			}
		}


		public virtual JTableHeader TableHeader
		{
			get
			{
				return tableHeader;
			}
			set
			{
				if (this.tableHeader != null)
				{
					this.tableHeader.removeMouseListener(mouseListener);
					TableCellRenderer defaultRenderer = this.tableHeader.getDefaultRenderer();
					if (defaultRenderer is SortableHeaderRenderer)
					{
						this.tableHeader.setDefaultRenderer(((SortableHeaderRenderer) defaultRenderer).tableCellRenderer);
					}
				}
				this.tableHeader = value;
				if (this.tableHeader != null)
				{
					this.tableHeader.addMouseListener(mouseListener);
					this.tableHeader.setDefaultRenderer(new SortableHeaderRenderer(this, this.tableHeader.getDefaultRenderer()));
				}
			}
		}


		public virtual bool Sorting
		{
			get
			{
				return sortingColumns.Count != 0;
			}
		}

		private Directive getDirective(int column)
		{
			for (int i = 0; i < sortingColumns.Count; i++)
			{
				Directive directive = sortingColumns[i];
				if (directive.column == column)
				{
					return directive;
				}
			}
			return EMPTY_DIRECTIVE;
		}

		public virtual int getSortingStatus(int column)
		{
			return getDirective(column).direction;
		}

		private void sortingStatusChanged()
		{
			clearSortingState();
			fireTableDataChanged();
			if (tableHeader != null)
			{
				tableHeader.repaint();
			}
		}

		public virtual void setSortingStatus(int column, int status)
		{
			Directive directive = getDirective(column);
			if (directive != EMPTY_DIRECTIVE)
			{
				sortingColumns.Remove(directive);
			}
			if (status != NOT_SORTED)
			{
				sortingColumns.Add(new Directive(column, status));
			}
			sortingStatusChanged();
		}

		protected internal virtual Icon getHeaderRendererIcon(int column, int size)
		{
			Directive directive = getDirective(column);
			if (directive == EMPTY_DIRECTIVE)
			{
				return null;
			}
			return new Arrow(directive.direction == DESCENDING, size, sortingColumns.IndexOf(directive));
		}

		private void cancelSorting()
		{
			sortingColumns.Clear();
			sortingStatusChanged();
		}

		public virtual void setColumnComparator<T1>(Type type, IComparer<T1> comparator)
		{
			if (comparator == null)
			{
				columnComparators.Remove(type);
			}
			else
			{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") Comparator<Object> castComparator = (Comparator<Object>) comparator;
				IComparer<object> castComparator = (IComparer<object>) comparator;
				columnComparators[type] = castComparator;
			}
		}

		protected internal virtual IComparer<object> getComparator(int column)
		{
			Type columnType = tableModel.getColumnClass(column);
			IComparer<object> comparator = columnComparators[columnType];
			if (comparator != null)
			{
				return comparator;
			}
			if (columnType.IsAssignableFrom(typeof(IComparable)))
			{
				return COMPARABLE_COMPARATOR;
			}
			return LEXICAL_COMPARATOR;
		}

		private Row[] ViewToModel
		{
			get
			{
				if (viewToModel == null)
				{
					int tableModelRowCount = tableModel.getRowCount();
					viewToModel = new Row[tableModelRowCount];
					for (int row = 0; row < tableModelRowCount; row++)
					{
						viewToModel[row] = new Row(this, row);
					}
    
					if (Sorting)
					{
						Array.Sort(viewToModel);
					}
				}
				return viewToModel;
			}
		}

		public virtual int modelIndex(int viewIndex)
		{
			return ViewToModel[viewIndex].modelIndex;
		}

		private int[] ModelToView
		{
			get
			{
				if (modelToView == null)
				{
					int n = ViewToModel.Length;
					modelToView = new int[n];
					for (int i = 0; i < n; i++)
					{
						modelToView[modelIndex(i)] = i;
					}
				}
				return modelToView;
			}
		}

		// TableModel interface methods

		public virtual int RowCount
		{
			get
			{
				return (tableModel == null) ? 0 : tableModel.getRowCount();
			}
		}

		public virtual int ColumnCount
		{
			get
			{
				return (tableModel == null) ? 0 : tableModel.getColumnCount();
			}
		}

		public virtual string getColumnName(int column)
		{
			return tableModel.getColumnName(column);
		}

		public virtual Type getColumnClass(int column)
		{
			return tableModel.getColumnClass(column);
		}

		public virtual bool isCellEditable(int row, int column)
		{
			return tableModel.isCellEditable(modelIndex(row), column);
		}

		public virtual object getValueAt(int row, int column)
		{
			return tableModel.getValueAt(modelIndex(row), column);
		}

		public virtual void setValueAt(object aValue, int row, int column)
		{
			tableModel.setValueAt(aValue, modelIndex(row), column);
		}

		// Helper classes

		private class Row : IComparable<Row>
		{
			private readonly TableSorter outerInstance;

			internal int modelIndex;

			public Row(TableSorter outerInstance, int index)
			{
				this.outerInstance = outerInstance;
				this.modelIndex = index;
			}

			public virtual int CompareTo(Row o)
			{
				int row1 = modelIndex;
				int row2 = o.modelIndex;

				for (IEnumerator<Directive> it = outerInstance.sortingColumns.GetEnumerator(); it.MoveNext();)
				{
					Directive directive = it.Current;
					int column = directive.column;

					object o1 = outerInstance.tableModel.getValueAt(row1, column);
					object o2 = outerInstance.tableModel.getValueAt(row2, column);

					int comparison = 0;
					// Define null less than everything, except null.
					if (o1 == null && o2 == null)
					{
						comparison = 0;
					}
					else if (o1 == null)
					{
						comparison = -1;
					}
					else if (o2 == null)
					{
						comparison = 1;
					}
					else
					{
						comparison = outerInstance.getComparator(column).Compare(o1, o2);
					}
					if (comparison != 0)
					{
						return directive.direction == DESCENDING ? -comparison : comparison;
					}
				}
				return 0;
			}
		}

		private class TableModelHandler : TableModelListener
		{
			private readonly TableSorter outerInstance;

			public TableModelHandler(TableSorter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void tableChanged(TableModelEvent e)
			{
				// If we're not sorting by anything, just pass the event along.
				if (!outerInstance.Sorting)
				{
					outerInstance.clearSortingState();
					fireTableChanged(e);
					return;
				}

				// If the table structure has changed, cancel the sorting; the
				// sorting columns may have been either moved or deleted from
				// the model.
				if (e.getFirstRow() == TableModelEvent.HEADER_ROW)
				{
					outerInstance.cancelSorting();
					fireTableChanged(e);
					return;
				}

				// We can map a cell event through to the view without widening
				// when the following conditions apply:
				//
				// a) all the changes are on one row (e.getFirstRow() == e.getLastRow()) and,
				// b) all the changes are in one column (column != TableModelEvent.ALL_COLUMNS) and,
				// c) we are not sorting on that column (getSortingStatus(column) == NOT_SORTED) and,
				// d) a reverse lookup will not trigger a sort (modelToView != null)
				//
				// Note: INSERT and DELETE events fail this test as they have column == ALL_COLUMNS.
				//
				// The last check, for (modelToView != null) is to see if modelToView
				// is already allocated. If we don't do this check; sorting can become
				// a performance bottleneck for applications where cells
				// change rapidly in different parts of the table. If cells
				// change alternately in the sorting column and then outside of
				// it this class can end up re-sorting on alternate cell updates -
				// which can be a performance problem for large tables. The last
				// clause avoids this problem.
				int column = e.getColumn();
				if (e.getFirstRow() == e.getLastRow() && column != TableModelEvent.ALL_COLUMNS && outerInstance.getSortingStatus(column) == NOT_SORTED && outerInstance.modelToView != null)
				{
					int viewIndex = outerInstance.ModelToView[e.getFirstRow()];
					fireTableChanged(new TableModelEvent(outerInstance, viewIndex, viewIndex, column, e.getType()));
					return;
				}

				// Something has happened to the data that may have invalidated the row order.
				outerInstance.clearSortingState();
				fireTableDataChanged();
				return;
			}
		}

		private class MouseHandler : MouseAdapter
		{
			private readonly TableSorter outerInstance;

			public MouseHandler(TableSorter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void mouseClicked(MouseEvent e)
			{
				JTableHeader h = (JTableHeader) e.getSource();
				TableColumnModel columnModel = h.getColumnModel();
				int viewColumn = columnModel.getColumnIndexAtX(e.getX());
				int column = columnModel.getColumn(viewColumn).getModelIndex();
				if (column != -1)
				{
					int status = outerInstance.getSortingStatus(column);
					if (!e.isControlDown())
					{
						outerInstance.cancelSorting();
					}
					// Cycle the sorting states through {NOT_SORTED, ASCENDING, DESCENDING} or
					// {NOT_SORTED, DESCENDING, ASCENDING} depending on whether shift is pressed.
					status = status + (e.isShiftDown() ? -1 : 1);
					status = (status + 4) % 3 - 1; // signed mod, returning {-1, 0, 1}
					outerInstance.setSortingStatus(column, status);
				}
			}
		}

		private class Arrow : Icon
		{
			internal bool descending;
			internal int size;
			internal int priority;

			public Arrow(bool descending, int size, int priority)
			{
				this.descending = descending;
				this.size = size;
				this.priority = priority;
			}

			public virtual void paintIcon(Component c, JGraphics g, int x, int y)
			{
				Color color = c == null ? Color.Gray : c.getBackground();
				// In a compound sort, make each succesive triangle 20%
				// smaller than the previous one.
				int dx = (int)(size / 2 * Math.Pow(0.8, priority));
				int dy = descending ? dx : -dx;
				// Align icon (roughly) with font baseline.
				y = y + 5 * size / 6 + (descending ? -dy : 0);
				int shift = descending ? 1 : -1;
				g.translate(x, y);

				// Right diagonal.
				g.setColor(color.darker());
				g.drawLine(dx / 2, dy, 0, 0);
				g.drawLine(dx / 2, dy + shift, 0, shift);

				// Left diagonal.
				g.setColor(color.brighter());
				g.drawLine(dx / 2, dy, dx, 0);
				g.drawLine(dx / 2, dy + shift, dx, shift);

				// Horizontal line.
				if (descending)
				{
					g.setColor(color.darker().darker());
				}
				else
				{
					g.setColor(color.brighter().brighter());
				}
				g.drawLine(dx, 0, 0, 0);

				g.setColor(color);
				g.translate(-x, -y);
			}

			public virtual int IconWidth
			{
				get
				{
					return size;
				}
			}

			public virtual int IconHeight
			{
				get
				{
					return size;
				}
			}
		}

		private class SortableHeaderRenderer : TableCellRenderer
		{
			private readonly TableSorter outerInstance;

			internal TableCellRenderer tableCellRenderer;

			public SortableHeaderRenderer(TableSorter outerInstance, TableCellRenderer tableCellRenderer)
			{
				this.outerInstance = outerInstance;
				this.tableCellRenderer = tableCellRenderer;
			}

			public virtual Component getTableCellRendererComponent(JTable table, object value, bool isSelected, bool hasFocus, int row, int column)
			{
				Component c = tableCellRenderer.getTableCellRendererComponent(table, value, isSelected, hasFocus, row, column);
				if (c is JLabel)
				{
					JLabel l = (JLabel) c;
					l.setHorizontalTextPosition(JLabel.LEFT);
					int modelColumn = table.convertColumnIndexToModel(column);
					l.setIcon(outerInstance.getHeaderRendererIcon(modelColumn, l.getFont().getSize()));
				}
				return c;
			}
		}

		private class Directive
		{
			internal int column;
			internal int direction;

			public Directive(int column, int direction)
			{
				this.column = column;
				this.direction = direction;
			}
		}
	}
}
