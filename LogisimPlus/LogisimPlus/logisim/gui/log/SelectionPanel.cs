// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{


	internal class SelectionPanel : LogPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new Listener(this);
		}

		private class Listener : MouseAdapter, ActionListener, TreeSelectionListener, ListSelectionListener
		{
			private readonly SelectionPanel outerInstance;

			public Listener(SelectionPanel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override void mouseClicked(MouseEvent e)
			{
				if (e.getClickCount() == 2)
				{
					TreePath path = outerInstance.selector.getPathForLocation(e.getX(), e.getY());
					if (path != null && outerInstance.listener != null)
					{
						doAdd(outerInstance.selector.SelectedItems);
					}
				}
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.addTool)
				{
					doAdd(outerInstance.selector.SelectedItems);
				}
				else if (src == outerInstance.changeBase)
				{
					SelectionItem sel = (SelectionItem) outerInstance.list.getSelectedValue();
					if (sel != null)
					{
						int radix = sel.Radix;
						switch (radix)
						{
						case 2:
							sel.Radix = 10;
							break;
						case 10:
							sel.Radix = 16;
							break;
						default:
							sel.Radix = 2;
						break;
						}
					}
				}
				else if (src == outerInstance.moveUp)
				{
					doMove(-1);
				}
				else if (src == outerInstance.moveDown)
				{
					doMove(1);
				}
				else if (src == outerInstance.remove)
				{
					Selection sel = outerInstance.Selection;
					IList<SelectionItem> toRemove = outerInstance.list.getSelectedValuesList();
					bool changed = false;
					for (int i = 0; i < toRemove.Count; i++)
					{
						int index = sel.indexOf(toRemove[i]);
						if (index >= 0)
						{
							sel.remove(index);
							changed = true;
						}
					}
					if (changed)
					{
						outerInstance.list.clearSelection();
					}
				}
			}

			public virtual void valueChanged(TreeSelectionEvent @event)
			{
				computeEnabled();
			}

			public virtual void valueChanged(ListSelectionEvent @event)
			{
				computeEnabled();
			}

			internal virtual void computeEnabled()
			{
				int index = outerInstance.list.getSelectedIndex();
				outerInstance.addTool.setEnabled(outerInstance.selector.hasSelectedItems());
				outerInstance.changeBase.setEnabled(index >= 0);
				outerInstance.moveUp.setEnabled(index > 0);
				outerInstance.moveDown.setEnabled(index >= 0 && index < outerInstance.list.getModel().getSize() - 1);
				outerInstance.remove.setEnabled(index >= 0);
			}

			internal virtual void doAdd(IList<SelectionItem> selectedItems)
			{
				if (selectedItems != null && selectedItems.Count > 0)
				{
					SelectionItem last = null;
					foreach (SelectionItem item in selectedItems)
					{
						outerInstance.Selection.add(item);
						last = item;
					}
					outerInstance.list.setSelectedValue(last, true);
				}
			}

			internal virtual void doMove(int delta)
			{
				Selection sel = outerInstance.Selection;
				int oldIndex = outerInstance.list.getSelectedIndex();
				int newIndex = oldIndex + delta;
				if (oldIndex >= 0 && newIndex >= 0 && newIndex < sel.size())
				{
					sel.move(oldIndex, newIndex);
					outerInstance.list.setSelectedIndex(newIndex);
				}
			}
		}

		private Listener listener;

		private ComponentSelector selector;
		private JButton addTool;
		private JButton changeBase;
		private JButton moveUp;
		private JButton moveDown;
		private JButton remove;
		private SelectionList list;

		public SelectionPanel(LogFrame window) : base(window)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			selector = new ComponentSelector(Model);
			addTool = new JButton();
			changeBase = new JButton();
			moveUp = new JButton();
			moveDown = new JButton();
			remove = new JButton();
			list = new SelectionList();
			list.Selection = Selection;

			JPanel buttons = new JPanel(new GridLayout(5, 1));
			buttons.add(addTool);
			buttons.add(changeBase);
			buttons.add(moveUp);
			buttons.add(moveDown);
			buttons.add(remove);

			addTool.addActionListener(listener);
			changeBase.addActionListener(listener);
			moveUp.addActionListener(listener);
			moveDown.addActionListener(listener);
			remove.addActionListener(listener);
			selector.addMouseListener(listener);
			selector.addTreeSelectionListener(listener);
			list.addListSelectionListener(listener);
			listener.computeEnabled();

			GridBagLayout gridbag = new GridBagLayout();
			GridBagConstraints gbc = new GridBagConstraints();
			setLayout(gridbag);
			JScrollPane explorerPane = new JScrollPane(selector, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			JScrollPane listPane = new JScrollPane(list, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			gbc.fill = GridBagConstraints.BOTH;
			gbc.weightx = 1.0;
			gbc.weighty = 1.0;
			gridbag.setConstraints(explorerPane, gbc);
			add(explorerPane);
			gbc.fill = GridBagConstraints.NONE;
			gbc.anchor = GridBagConstraints.NORTH;
			gbc.weightx = 0.0;
			gridbag.setConstraints(buttons, gbc);
			add(buttons);
			gbc.fill = GridBagConstraints.BOTH;
			gbc.weightx = 1.0;
			gridbag.setConstraints(listPane, gbc);
			add(listPane);
		}

		public override string Title
		{
			get
			{
				return Strings.get("selectionTab");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("selectionHelp");
			}
		}

		public override void localeChanged()
		{
			addTool.setText(Strings.get("selectionAdd"));
			changeBase.setText(Strings.get("selectionChangeBase"));
			moveUp.setText(Strings.get("selectionMoveUp"));
			moveDown.setText(Strings.get("selectionMoveDown"));
			remove.setText(Strings.get("selectionRemove"));
			selector.localeChanged();
			list.localeChanged();
		}

		public override void modelChanged(Model oldModel, Model newModel)
		{
			if (Model == null)
			{
				selector.LogModel = newModel;
				list.Selection = null;
			}
			else
			{
				selector.LogModel = newModel;
				list.Selection = Selection;
			}
			listener.computeEnabled();
		}
	}

}
