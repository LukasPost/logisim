// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.generic
{

	using JDialogOk = logisim.util.JDialogOk;
	using JInputComponent = logisim.util.JInputComponent;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;


	public class AttrTable : JPanel, LocaleListener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			editor = new CellEditor(this);
		}

		private static readonly AttrTableModel NULL_ATTR_MODEL = new NullAttrModel();

		private class NullAttrModel : AttrTableModel
		{
			public virtual void addAttrTableModelListener(AttrTableModelListener listener)
			{
			}

			public virtual void removeAttrTableModelListener(AttrTableModelListener listener)
			{
			}

			public virtual string Title
			{
				get
				{
					return null;
				}
			}

			public virtual int RowCount
			{
				get
				{
					return 0;
				}
			}

			public virtual AttrTableModelRow getRow(int rowIndex)
			{
				return null;
			}
		}

		private class TitleLabel : JLabel
		{
			public override Size MinimumSize
			{
				get
				{
					Size ret = base.getMinimumSize();
					return new Size(1, ret.height);
				}
			}
		}

		private class MyDialog : JDialogOk
		{
			internal JInputComponent input;
			internal object value;

			public MyDialog(Dialog parent, JInputComponent input) : base(parent, Strings.get("attributeDialogTitle"), true)
			{
				configure(input);
			}

			public MyDialog(Frame parent, JInputComponent input) : base(parent, Strings.get("attributeDialogTitle"), true)
			{
				configure(input);
			}

			internal virtual void configure(JInputComponent input)
			{
				this.input = input;
				this.value = input.Value;

				// Thanks to Christophe Jacquet, who contributed a fix to this
				// so that when the dialog is resized, the component within it
				// is resized as well. (Tracker #2024479)
				JPanel p = new JPanel(new BorderLayout());
				p.setBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10));
				p.add((JComponent) input, BorderLayout.CENTER);
				ContentPane.add(p, BorderLayout.CENTER);

				pack();
			}

			public override void okClicked()
			{
				value = input.Value;
			}

			public virtual object Value
			{
				get
				{
					return value;
				}
			}
		}

		private class TableModelAdapter : TableModel, AttrTableModelListener
		{
			private readonly AttrTable outerInstance;

			internal Window parent;
			internal LinkedList<TableModelListener> listeners;
			internal AttrTableModel attrModel;

			internal TableModelAdapter(AttrTable outerInstance, Window parent, AttrTableModel attrModel)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.listeners = new LinkedList<TableModelListener>();
				this.attrModel = attrModel;
			}

			internal virtual AttrTableModel AttrTableModel
			{
				set
				{
					if (attrModel != value)
					{
						TableCellEditor editor = outerInstance.table.getCellEditor();
						if (editor != null)
						{
							editor.cancelCellEditing();
						}
						attrModel.removeAttrTableModelListener(this);
						attrModel = value;
						attrModel.addAttrTableModelListener(this);
						fireTableChanged();
					}
				}
			}

			public virtual void addTableModelListener(TableModelListener l)
			{
				listeners.AddLast(l);
			}

			public virtual void removeTableModelListener(TableModelListener l)
			{
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
				listeners.remove(l);
			}

			internal virtual void fireTableChanged()
			{
				TableModelEvent e = new TableModelEvent(this);
				foreach (TableModelListener l in new List<TableModelListener>(listeners))
				{
					l.tableChanged(e);
				}
			}

			public virtual int ColumnCount
			{
				get
				{
					return 2;
				}
			}

			public virtual string getColumnName(int columnIndex)
			{
				if (columnIndex == 0)
				{
					return "Attribute";
				}
				else
				{
					return "Value";
				}
			}

			public virtual Type getColumnClass(int columnIndex)
			{
				return typeof(string);
			}

			public virtual int RowCount
			{
				get
				{
					return attrModel.RowCount;
				}
			}

			public virtual object getValueAt(int rowIndex, int columnIndex)
			{
				if (columnIndex == 0)
				{
					return attrModel.getRow(rowIndex).Label;
				}
				else
				{
					return attrModel.getRow(rowIndex).getValue();
				}
			}

			public virtual bool isCellEditable(int rowIndex, int columnIndex)
			{
				return columnIndex > 0 && attrModel.getRow(rowIndex).ValueEditable;
			}

			public virtual void setValueAt(object value, int rowIndex, int columnIndex)
			{
				if (columnIndex > 0)
				{
					try
					{
						attrModel.getRow(rowIndex).setValue(value);
					}
					catch (AttrTableSetException e)
					{
						JOptionPane.showMessageDialog(parent, e.Message, Strings.get("attributeChangeInvalidTitle"), JOptionPane.WARNING_MESSAGE);
					}
				}
			}

			//
			// AttrTableModelListener methods
			//
			public virtual void attrTitleChanged(AttrTableModelEvent e)
			{
				if (e.Source != attrModel)
				{
					attrModel.removeAttrTableModelListener(this);
					return;
				}
				outerInstance.updateTitle();
			}

			public virtual void attrStructureChanged(AttrTableModelEvent e)
			{
				if (e.Source != attrModel)
				{
					attrModel.removeAttrTableModelListener(this);
					return;
				}
				fireTableChanged();
			}

			public virtual void attrValueChanged(AttrTableModelEvent e)
			{
				if (e.Source != attrModel)
				{
					attrModel.removeAttrTableModelListener(this);
					return;
				}
				fireTableChanged();
			}
		}

		private class CellEditor : TableCellEditor, FocusListener, ActionListener
		{
			private readonly AttrTable outerInstance;

			public CellEditor(AttrTable outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal LinkedList<CellEditorListener> listeners = new LinkedList<CellEditorListener>();
			internal Component currentEditor;

			//
			// TableCellListener management
			//
			public virtual void addCellEditorListener(CellEditorListener l)
			{
				// Adds a listener to the list that's notified when the
				// editor stops, or cancels editing.
				listeners.AddLast(l);
			}

			public virtual void removeCellEditorListener(CellEditorListener l)
			{
				// Removes a listener from the list that's notified
// JAVA TO C# CONVERTER TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
				listeners.remove(l);
			}

			public virtual void fireEditingCanceled()
			{
				ChangeEvent e = new ChangeEvent(outerInstance);
				foreach (CellEditorListener l in new List<CellEditorListener>(listeners))
				{
					l.editingCanceled(e);
				}
			}

			public virtual void fireEditingStopped()
			{
				ChangeEvent e = new ChangeEvent(outerInstance);
				foreach (CellEditorListener l in new List<CellEditorListener>(listeners))
				{
					l.editingStopped(e);
				}
			}

			//
			// other TableCellEditor methods
			//
			public virtual void cancelCellEditing()
			{
				// Tells the editor to cancel editing and not accept any
				// partially edited value.
				fireEditingCanceled();
			}

			public virtual bool stopCellEditing()
			{
				// Tells the editor to stop editing and accept any partially
				// edited value as the value of the editor.
				fireEditingStopped();
				return true;
			}

			public virtual object CellEditorValue
			{
				get
				{
					// Returns the value contained in the editor.
					Component comp = currentEditor;
					if (comp is JTextField)
					{
						return ((JTextField) comp).getText();
					}
					else if (comp is JComboBox)
					{
	// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
	// ORIGINAL LINE: return ((javax.swing.JComboBox<?>) comp).getSelectedItem();
						return ((JComboBox<object>) comp).getSelectedItem();
					}
					else
					{
						return null;
					}
				}
			}

			public virtual bool isCellEditable(EventObject anEvent)
			{
				// Asks the editor if it can start editing using anEvent.
				return true;
			}

			public virtual bool shouldSelectCell(EventObject anEvent)
			{
				// Returns true if the editing cell should be selected,
				// false otherwise.
				return true;
			}

			public virtual Component getTableCellEditorComponent(JTable table, object value, bool isSelected, int rowIndex, int columnIndex)
			{
				AttrTableModel attrModel = outerInstance.tableModel.attrModel;
				AttrTableModelRow row = attrModel.getRow(rowIndex);

				if (columnIndex == 0)
				{
					return new JLabel(row.Label);
				}
				else
				{
					if (currentEditor != null)
					{
						currentEditor.transferFocus();
					}

					Component editor = row.getEditor(outerInstance.parent);
					if (editor is JComboBox)
					{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: ((javax.swing.JComboBox<?>) editor).addActionListener(this);
						((JComboBox<object>) editor).addActionListener(this);
						editor.addFocusListener(this);
					}
					else if (editor is JInputComponent)
					{
						JInputComponent input = (JInputComponent) editor;
						MyDialog dlog;
						Window parent = outerInstance.parent;
						if (parent is Frame)
						{
							dlog = new MyDialog((Frame) parent, input);
						}
						else
						{
							dlog = new MyDialog((Dialog) parent, input);
						}
						dlog.setVisible(true);
						object retval = dlog.Value;
						try
						{
							row.setValue(retval);
						}
						catch (AttrTableSetException e)
						{
							JOptionPane.showMessageDialog(parent, e.Message, Strings.get("attributeChangeInvalidTitle"), JOptionPane.WARNING_MESSAGE);
						}
						editor = new JLabel(row.getValue());
					}
					else
					{
						editor.addFocusListener(this);
					}
					currentEditor = editor;
					return editor;
				}
			}

			//
			// FocusListener methods
			//
			public virtual void focusLost(FocusEvent e)
			{
				object dst = e.getOppositeComponent();
				if (dst is Component)
				{
					Component p = (Component) dst;
					while (p != null && !(p is Window))
					{
						if (p == outerInstance)
						{
							// switch to another place in this table,
							// no problem
							return;
						}
						p = p.getParent();
					}
					// focus transferred outside table; stop editing
					outerInstance.editor.stopCellEditing();
				}
			}

			public virtual void focusGained(FocusEvent e)
			{
			}

			//
			// ActionListener methods
			//
			public virtual void actionPerformed(ActionEvent e)
			{
				stopCellEditing();
			}

		}

		private Window parent;
		private bool titleEnabled;
		private JLabel title;
		private JTable table;
		private TableModelAdapter tableModel;
		private CellEditor editor;

		public AttrTable(Window parent) : base(new BorderLayout())
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.parent = parent;

			titleEnabled = true;
			title = new TitleLabel();
			title.setHorizontalAlignment(SwingConstants.CENTER);
			title.setVerticalAlignment(SwingConstants.CENTER);
			tableModel = new TableModelAdapter(this, parent, NULL_ATTR_MODEL);
			table = new JTable(tableModel);
			table.setDefaultEditor(typeof(object), editor);
			table.setTableHeader(null);
			table.setRowHeight(20);

			Font baseFont = title.getFont();
			int titleSize = (long)Math.Round(baseFont.getSize() * 1.2f, MidpointRounding.AwayFromZero);
			Font titleFont = baseFont.deriveFont((float) titleSize).deriveFont(Font.BOLD);
			title.setFont(titleFont);
			Color bgColor = new Color(240, 240, 240);
			setBackground(bgColor);
			table.setBackground(bgColor);
			object renderer = table.getDefaultRenderer(typeof(string));
			if (renderer is JComponent)
			{
				((JComponent) renderer).setBackground(Color.WHITE);
			}

			JScrollPane tableScroll = new JScrollPane(table);

			this.add(title, BorderLayout.PAGE_START);
			this.add(tableScroll, BorderLayout.CENTER);
			LocaleManager.addLocaleListener(this);
			localeChanged();
		}

		public virtual bool TitleEnabled
		{
			set
			{
				titleEnabled = value;
				updateTitle();
			}
			get
			{
				return titleEnabled;
			}
		}


		public virtual AttrTableModel AttrTableModel
		{
			set
			{
				tableModel.AttrTableModel = value == null ? NULL_ATTR_MODEL : value;
				updateTitle();
			}
			get
			{
				return tableModel.attrModel;
			}
		}


		public virtual void localeChanged()
		{
			updateTitle();
			tableModel.fireTableChanged();
		}

		private void updateTitle()
		{
			if (titleEnabled)
			{
				string text = tableModel.attrModel.Title;
				if (string.ReferenceEquals(text, null))
				{
					title.setVisible(false);
				}
				else
				{
					title.setText(text);
					title.setVisible(true);
				}
			}
			else
			{
				title.setVisible(false);
			}
		}
	}

}
