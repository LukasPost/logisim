// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using Component = logisim.comp.Component;
	using Value = logisim.data.Value;

	internal class SelectionList : JList<SelectionItem>
	{
		private class Model : AbstractListModel<SelectionItem>, ModelListener
		{
			private readonly SelectionList outerInstance;

			public Model(SelectionList outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Size
			{
				get
				{
					return outerInstance.selection == null ? 0 : outerInstance.selection.size();
				}
			}

			public virtual SelectionItem getElementAt(int index)
			{
				return outerInstance.selection.get(index);
			}

			public virtual void selectionChanged(ModelEvent @event)
			{
				fireContentsChanged(this, 0, Size);
			}

			public virtual void entryAdded(ModelEvent @event, Value[] values)
			{
			}

			public virtual void filePropertyChanged(ModelEvent @event)
			{
			}
		}

		private class MyCellRenderer : DefaultListCellRenderer
		{
			private readonly SelectionList outerInstance;

			public MyCellRenderer(SelectionList outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override java.awt.Component getListCellRendererComponent<T1>(JList<T1> list, object value, int index, bool isSelected, bool hasFocus)
			{
				java.awt.Component ret = base.getListCellRendererComponent(list, value, index, isSelected, hasFocus);
				if (ret is JLabel && value is SelectionItem)
				{
					JLabel label = (JLabel) ret;
					SelectionItem item = (SelectionItem) value;
					Component comp = item.Component;
					label.setIcon(new ComponentIcon(comp));
					label.setText(item.ToString() + " - " + item.Radix);
				}
				return ret;
			}
		}

		private Selection selection;

		public SelectionList()
		{
			selection = null;
			setModel(new Model(this));
			setCellRenderer(new MyCellRenderer(this));
			setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
		}

		public virtual Selection Selection
		{
			set
			{
				if (selection != value)
				{
					Model model = (Model) getModel();
					if (selection != null)
					{
						selection.removeModelListener(model);
					}
					selection = value;
					if (selection != null)
					{
						selection.addModelListener(model);
					}
					model.selectionChanged(null);
				}
			}
		}

		public virtual void localeChanged()
		{
			repaint();
		}
	}

}
