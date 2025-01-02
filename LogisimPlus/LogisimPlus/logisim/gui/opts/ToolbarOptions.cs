// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{


	using ToolbarData = logisim.file.ToolbarData;
	using ProjectExplorer = logisim.gui.main.ProjectExplorer;
	using Event = logisim.gui.main.ProjectExplorer.Event;
	using AddTool = logisim.tools.AddTool;
	using Tool = logisim.tools.Tool;
	using TableLayout = logisim.util.TableLayout;

	internal class ToolbarOptions : OptionsPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new Listener(this);
		}

		private class Listener : ProjectExplorer.Listener, ActionListener, ListSelectionListener
		{
			private readonly ToolbarOptions outerInstance;

			public Listener(ToolbarOptions outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void selectionChanged(ProjectExplorer.Event @event)
			{
				computeEnabled();
			}

			public virtual void doubleClicked(ProjectExplorer.Event @event)
			{
				object target = @event.Target;
				if (target is Tool)
				{
					doAddTool((Tool) target);
				}
			}

			public virtual void moveRequested(ProjectExplorer.Event @event, AddTool dragged, AddTool target)
			{
			}

			public virtual void deleteRequested(ProjectExplorer.Event @event)
			{
			}

			public virtual JPopupMenu menuRequested(ProjectExplorer.Event @event)
			{
				return null;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.addTool)
				{
					doAddTool(outerInstance.explorer.SelectedTool.cloneTool());
				}
				else if (src == outerInstance.addSeparator)
				{
					outerInstance.Options.ToolbarData.addSeparator();
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
					int index = outerInstance.list.getSelectedIndex();
					if (index >= 0)
					{
						outerInstance.Project.doAction(ToolbarActions.removeTool(outerInstance.Options.ToolbarData, index));
						outerInstance.list.clearSelection();
					}
				}
			}

			public virtual void valueChanged(ListSelectionEvent @event)
			{
				computeEnabled();
			}

			internal virtual void computeEnabled()
			{
				int index = outerInstance.list.getSelectedIndex();
				outerInstance.addTool.setEnabled(outerInstance.explorer.SelectedTool != null);
				outerInstance.moveUp.setEnabled(index > 0);
				outerInstance.moveDown.setEnabled(index >= 0 && index < outerInstance.list.getModel().getSize() - 1);
				outerInstance.remove.setEnabled(index >= 0);
			}

			internal virtual void doAddTool(Tool tool)
			{
				if (tool != null)
				{
					outerInstance.Project.doAction(ToolbarActions.addTool(outerInstance.Options.ToolbarData, tool));
				}
			}

			internal virtual void doMove(int delta)
			{
				int oldIndex = outerInstance.list.getSelectedIndex();
				int newIndex = oldIndex + delta;
				ToolbarData data = outerInstance.Options.ToolbarData;
				if (oldIndex >= 0 && newIndex >= 0 && newIndex < data.size())
				{
					outerInstance.Project.doAction(ToolbarActions.moveTool(data, oldIndex, newIndex));
					outerInstance.list.setSelectedIndex(newIndex);
				}
			}
		}

		private Listener listener;

		private ProjectExplorer explorer;
		private JButton addTool;
		private JButton addSeparator;
		private JButton moveUp;
		private JButton moveDown;
		private JButton remove;
		private ToolbarList list;

		public ToolbarOptions(OptionsFrame window) : base(window)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			explorer = new ProjectExplorer(Project);
			addTool = new JButton();
			addSeparator = new JButton();
			moveUp = new JButton();
			moveDown = new JButton();
			remove = new JButton();

			list = new ToolbarList(Options.ToolbarData);

			TableLayout middleLayout = new TableLayout(1);
			JPanel middle = new JPanel(middleLayout);
			middle.add(addTool);
			middle.add(addSeparator);
			middle.add(moveUp);
			middle.add(moveDown);
			middle.add(remove);
			middleLayout.setRowWeight(4, 1.0);

			explorer.Listener = listener;
			addTool.addActionListener(listener);
			addSeparator.addActionListener(listener);
			moveUp.addActionListener(listener);
			moveDown.addActionListener(listener);
			remove.addActionListener(listener);
			list.addListSelectionListener(listener);
			listener.computeEnabled();

			GridBagLayout gridbag = new GridBagLayout();
			GridBagConstraints gbc = new GridBagConstraints();
			setLayout(gridbag);
			JScrollPane explorerPane = new JScrollPane(explorer, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			JScrollPane listPane = new JScrollPane(list, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			gbc.fill = GridBagConstraints.BOTH;
			gbc.weightx = 1.0;
			gbc.weighty = 1.0;
			gridbag.setConstraints(explorerPane, gbc);
			add(explorerPane);
			gbc.fill = GridBagConstraints.VERTICAL;
			gbc.anchor = GridBagConstraints.NORTH;
			gbc.weightx = 0.0;
			gridbag.setConstraints(middle, gbc);
			add(middle);
			gbc.fill = GridBagConstraints.BOTH;
			gbc.weightx = 1.0;
			gridbag.setConstraints(listPane, gbc);
			add(listPane);
		}

		public override string Title
		{
			get
			{
				return Strings.get("toolbarTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("toolbarHelp");
			}
		}

		public override void localeChanged()
		{
			addTool.setText(Strings.get("toolbarAddTool"));
			addSeparator.setText(Strings.get("toolbarAddSeparator"));
			moveUp.setText(Strings.get("toolbarMoveUp"));
			moveDown.setText(Strings.get("toolbarMoveDown"));
			remove.setText(Strings.get("toolbarRemove"));
			list.localeChanged();
		}
	}

}
