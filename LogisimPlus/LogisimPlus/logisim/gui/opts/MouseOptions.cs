// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{


	using MouseMappings = logisim.file.MouseMappings;
	using AttrTable = logisim.gui.generic.AttrTable;
	using AttrTableModel = logisim.gui.generic.AttrTableModel;
	using AttrTableToolModel = logisim.gui.main.AttrTableToolModel;
	using ProjectExplorer = logisim.gui.main.ProjectExplorer;
	using Event = logisim.gui.main.ProjectExplorer.Event;
	using Project = logisim.proj.Project;
	using AddTool = logisim.tools.AddTool;
	using Tool = logisim.tools.Tool;
	using InputEventUtil = logisim.util.InputEventUtil;
	using StringUtil = logisim.util.StringUtil;

	internal class MouseOptions : OptionsPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			listener = new MyListener(this);
			addArea = new AddArea(this);
		}

		private class AddArea : JPanel
		{
			private readonly MouseOptions outerInstance;

			public AddArea(MouseOptions outerInstance)
			{
				this.outerInstance = outerInstance;
				setPreferredSize(new Size(75, 60));
				setMinimumSize(new Size(75, 60));
				setBorder(BorderFactory.createCompoundBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10), BorderFactory.createEtchedBorder()));
			}

			public override void paintComponent(Graphics g)
			{
				base.paintComponent(g);
				Size sz = getSize();
				g.setFont(outerInstance.remove.getFont());
				string label1;
				string label2;
				if (outerInstance.curTool == null)
				{
					g.setColor(Color.GRAY);
					label1 = Strings.get("mouseMapNone");
					label2 = null;
				}
				else
				{
					g.setColor(Color.BLACK);
					label1 = Strings.get("mouseMapText");
					label2 = StringUtil.format(Strings.get("mouseMapText2"), outerInstance.curTool.DisplayName);
				}
				FontMetrics fm = g.getFontMetrics();
				int x1 = (sz.width - fm.stringWidth(label1)) / 2;
				if (string.ReferenceEquals(label2, null))
				{
					int y = Math.Max(0, (sz.height - fm.getHeight()) / 2 + fm.getAscent() - 2);
					g.drawString(label1, x1, y);
				}
				else
				{
					int x2 = (sz.width - fm.stringWidth(label2)) / 2;
					int y = Math.Max(0, (sz.height - 2 * fm.getHeight()) / 2 + fm.getAscent() - 2);
					g.drawString(label1, x1, y);
					y += fm.getHeight();
					g.drawString(label2, x2, y);
				}
			}
		}

		private class MyListener : ActionListener, MouseListener, ListSelectionListener, MouseMappings.MouseMappingsListener, ProjectExplorer.Listener
		{
			private readonly MouseOptions outerInstance;

			public MyListener(MouseOptions outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			//
			// ActionListener method
			//
			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == outerInstance.remove)
				{
					int row = outerInstance.mappings.getSelectedRow();
					outerInstance.Project.doAction(OptionsActions.removeMapping(outerInstance.Options.MouseMappings, outerInstance.model.getKey(row)));
					row = Math.Min(row, outerInstance.model.RowCount - 1);
					if (row >= 0)
					{
						outerInstance.SelectedRow = row;
					}
				}
			}

			//
			// MouseListener methods
			//
			public virtual void mouseClicked(MouseEvent e)
			{
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}

			public virtual void mousePressed(MouseEvent e)
			{
				if (e.getSource() == outerInstance.addArea && outerInstance.curTool != null)
				{
					Tool t = outerInstance.curTool.cloneTool();
					int? mods = Convert.ToInt32(e.getModifiersEx());
					outerInstance.Project.doAction(OptionsActions.setMapping(outerInstance.Options.MouseMappings, mods, t));
					outerInstance.SelectedRow = outerInstance.model.getRow(mods);
				}
			}

			public virtual void mouseReleased(MouseEvent e)
			{
			}

			//
			// ListSelectionListener method
			//
			public virtual void valueChanged(ListSelectionEvent e)
			{
				int row = outerInstance.mappings.getSelectedRow();
				if (row < 0)
				{
					outerInstance.remove.setEnabled(false);
					outerInstance.attrTable.AttrTableModel = null;
				}
				else
				{
					outerInstance.remove.setEnabled(true);
					Tool tool = outerInstance.model.getTool(row);
					Project proj = outerInstance.Project;
					AttrTableModel model;
					if (tool.AttributeSet == null)
					{
						model = null;
					}
					else
					{
						model = new AttrTableToolModel(proj, tool);
					}
					outerInstance.attrTable.AttrTableModel = model;
				}
			}

			//
			// MouseMappingsListener method
			//
			public virtual void mouseMappingsChanged()
			{
				outerInstance.model.fireTableStructureChanged();
			}

			//
			// Explorer.Listener methods
			//
			public virtual void selectionChanged(ProjectExplorer.Event @event)
			{
				object target = @event.Target;
				if (target is Tool)
				{
					outerInstance.CurrentTool = (Tool) @event.Target;
				}
				else
				{
					outerInstance.CurrentTool = null;
				}
			}

			public virtual void doubleClicked(ProjectExplorer.Event @event)
			{
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
		}

		private class MappingsModel : AbstractTableModel
		{
			private readonly MouseOptions outerInstance;

			internal List<int> cur_keys;

			internal MappingsModel(MouseOptions outerInstance)
			{
				this.outerInstance = outerInstance;
				fireTableStructureChanged();
			}

			// AbstractTableModel methods
			public override void fireTableStructureChanged()
			{
				cur_keys = new List<int>(outerInstance.Options.MouseMappings.MappedModifiers);
				cur_keys.Sort();
				base.fireTableStructureChanged();
			}

			public virtual int RowCount
			{
				get
				{
					return cur_keys.Count;
				}
			}

			public virtual int ColumnCount
			{
				get
				{
					return 2;
				}
			}

			public virtual object getValueAt(int row, int column)
			{
				int? key = cur_keys[row];
				if (column == 0)
				{
					return InputEventUtil.toDisplayString(key.Value);
				}
				else
				{
					Tool tool = outerInstance.Options.MouseMappings.getToolFor(key.Value);
					return tool.DisplayName;
				}
			}

			// other methods
			internal virtual int? getKey(int row)
			{
				return cur_keys[row];
			}

			internal virtual Tool getTool(int row)
			{
				if (row < 0 || row >= cur_keys.Count)
				{
					return null;
				}
				int? key = cur_keys[row];
				return outerInstance.Options.MouseMappings.getToolFor(key.Value);
			}

			internal virtual int getRow(int? mods)
			{
				int row = Collections.binarySearch(cur_keys, mods);
				if (row < 0)
				{
					row = -(row + 1);
				}
				return row;
			}
		}

		private MyListener listener;
		private Tool curTool = null;
		private MappingsModel model;

		private ProjectExplorer explorer;
		private JPanel addArea;
		private JTable mappings = new JTable();
		private AttrTable attrTable;
		private JButton remove = new JButton();

		public MouseOptions(OptionsFrame window) : base(window, new GridLayout(1, 3))
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}

			explorer = new ProjectExplorer(Project);
			explorer.Listener = listener;

			// Area for adding mappings
			addArea.addMouseListener(listener);

			// Area for viewing current mappings
			model = new MappingsModel(this);
			mappings.setTableHeader(null);
			mappings.setModel(model);
			mappings.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			mappings.getSelectionModel().addListSelectionListener(listener);
			mappings.clearSelection();
			JScrollPane mapPane = new JScrollPane(mappings);

			// Button for removing current mapping
			JPanel removeArea = new JPanel();
			remove.addActionListener(listener);
			remove.setEnabled(false);
			removeArea.add(remove);

			// Area for viewing/changing attributes
			attrTable = new AttrTable(OptionsFrame);

			GridBagLayout gridbag = new GridBagLayout();
			GridBagConstraints gbc = new GridBagConstraints();
			setLayout(gridbag);
			gbc.weightx = 1.0;
			gbc.weighty = 1.0;
			gbc.gridheight = 4;
			gbc.fill = GridBagConstraints.BOTH;
			JScrollPane explorerPane = new JScrollPane(explorer, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_AS_NEEDED);
			gridbag.setConstraints(explorerPane, gbc);
			add(explorerPane);
			gbc.weightx = 0.0;
			JPanel gap = new JPanel();
			gap.setPreferredSize(new Size(10, 10));
			gridbag.setConstraints(gap, gbc);
			add(gap);
			gbc.weightx = 1.0;
			gbc.gridheight = 1;
			gbc.gridx = 2;
			gbc.gridy = GridBagConstraints.RELATIVE;
			gbc.weighty = 0.0;
			gridbag.setConstraints(addArea, gbc);
			add(addArea);
			gbc.weighty = 1.0;
			gridbag.setConstraints(mapPane, gbc);
			add(mapPane);
			gbc.weighty = 0.0;
			gridbag.setConstraints(removeArea, gbc);
			add(removeArea);
			gbc.weighty = 1.0;
			gridbag.setConstraints(attrTable, gbc);
			add(attrTable);

			Options.MouseMappings.addMouseMappingsListener(listener);
			CurrentTool = null;
		}

		public override string Title
		{
			get
			{
				return Strings.get("mouseTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("mouseHelp");
			}
		}

		public override void localeChanged()
		{
			remove.setText(Strings.get("mouseRemoveButton"));
			addArea.repaint();
		}

		private Tool CurrentTool
		{
			set
			{
				curTool = value;
				localeChanged();
			}
		}

		private int SelectedRow
		{
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value >= model.RowCount)
				{
					value = model.RowCount - 1;
				}
				if (value >= 0)
				{
					mappings.getSelectionModel().setSelectionInterval(value, value);
				}
			}
		}
	}

}
