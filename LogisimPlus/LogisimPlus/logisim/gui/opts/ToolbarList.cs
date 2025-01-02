// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{


	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using ToolbarData = logisim.file.ToolbarData;
	using ToolbarListener = logisim.file.ToolbarData.ToolbarListener;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Tool = logisim.tools.Tool;

	internal class ToolbarList : JList
	{
		private class ToolIcon : Icon
		{
			internal Tool tool;

			internal ToolIcon(Tool tool)
			{
				this.tool = tool;
			}

			public virtual void paintIcon(Component comp, Graphics g, int x, int y)
			{
				Graphics gNew = g.create();
				tool.paintIcon(new ComponentDrawContext(comp, null, null, g, gNew), x + 2, y + 2);
				gNew.dispose();
			}

			public virtual int IconWidth
			{
				get
				{
					return 20;
				}
			}

			public virtual int IconHeight
			{
				get
				{
					return 20;
				}
			}
		}

		private class ListRenderer : DefaultListCellRenderer
		{
			public override Component getListCellRendererComponent(JList list, object value, int index, bool isSelected, bool cellHasFocus)
			{
				Component ret;
				Icon icon;
				if (value is Tool)
				{
					Tool t = (Tool) value;
					ret = base.getListCellRendererComponent(list, t.DisplayName, index, isSelected, cellHasFocus);
					icon = new ToolIcon(t);
				}
				else if (value == null)
				{
					ret = base.getListCellRendererComponent(list, "---", index, isSelected, cellHasFocus);
					icon = null;
				}
				else
				{
					ret = base.getListCellRendererComponent(list, value.ToString(), index, isSelected, cellHasFocus);
					icon = null;
				}
				if (ret is JLabel)
				{
					((JLabel) ret).setIcon(icon);
				}
				return ret;
			}
		}

		private class Model : AbstractListModel, ToolbarData.ToolbarListener, AttributeListener, PropertyChangeListener
		{
			private readonly ToolbarList outerInstance;

			public Model(ToolbarList outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Size
			{
				get
				{
					return @base.size();
				}
			}

			public virtual object getElementAt(int index)
			{
				return @base.get(index);
			}

			public virtual void toolbarChanged()
			{
				fireContentsChanged(this, 0, Size);
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				repaint();
			}

			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				if (AppPreferences.GATE_SHAPE.isSource(@event))
				{
					repaint();
				}
			}
		}

		private ToolbarData @base;
		private Model model;

		public ToolbarList(ToolbarData @base)
		{
			this.@base = @base;
			this.model = new Model(this);

			setModel(model);
			setCellRenderer(new ListRenderer());
			setSelectionMode(ListSelectionModel.SINGLE_SELECTION);

			AppPreferences.GATE_SHAPE.addPropertyChangeListener(model);
			@base.addToolbarListener(model);
			@base.addToolAttributeListener(model);
		}

		public virtual void localeChanged()
		{
			model.toolbarChanged();
		}
	}

}
