// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{

	using AbstractToolbarModel = draw.toolbar.AbstractToolbarModel;
	using ToolbarItem = draw.toolbar.ToolbarItem;
	using ToolbarSeparator = draw.toolbar.ToolbarSeparator;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using LogisimFile = logisim.file.LogisimFile;
	using ToolbarData = logisim.file.ToolbarData;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using Tool = logisim.tools.Tool;
	using InputEventUtil = logisim.util.InputEventUtil;


	internal class LayoutToolbarModel : AbstractToolbarModel
	{
		private class ToolItem : ToolbarItem
		{
			private readonly LayoutToolbarModel outerInstance;

			internal Tool tool;

			internal ToolItem(LayoutToolbarModel outerInstance, Tool tool)
			{
				this.outerInstance = outerInstance;
				this.tool = tool;
			}

			public virtual bool Selectable
			{
				get
				{
					return true;
				}
			}

			public virtual void paintIcon(Component destination, Graphics g)
			{
				// draw halo
				if (tool == outerInstance.haloedTool && AppPreferences.ATTRIBUTE_HALO.Boolean)
				{
					g.setColor(Canvas.HALO_COLOR);
					g.fillRect(1, 1, 22, 22);
				}

				// draw tool icon
				g.setColor(Color.BLACK);
				Graphics g_copy = g.create();
				ComponentDrawContext c = new ComponentDrawContext(destination, null, null, g, g_copy);
				tool.paintIcon(c, 2, 2);
				g_copy.dispose();
			}

			public virtual string ToolTip
			{
				get
				{
					string ret = tool.Description;
					int index = 1;
					foreach (ToolbarItem item in outerInstance.items)
					{
						if (item == this)
						{
							break;
						}
						if (item is ToolItem)
						{
							++index;
						}
					}
					if (index <= 10)
					{
						if (index == 10)
						{
							index = 0;
						}
						int mask = outerInstance.frame.getToolkit().getMenuShortcutKeyMaskEx();
						ret += " (" + InputEventUtil.toKeyDisplayString(mask) + "-" + index + ")";
					}
					return ret;
				}
			}

			public virtual Dimension getDimension(object orientation)
			{
				return new Dimension(24, 24);
			}
		}

		private class MyListener : ProjectListener, AttributeListener, ToolbarData.ToolbarListener, PropertyChangeListener
		{
			private readonly LayoutToolbarModel outerInstance;

			public MyListener(LayoutToolbarModel outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			//
			// ProjectListener methods
			//
			public virtual void projectChanged(ProjectEvent e)
			{
				int act = e.Action;
				if (act == ProjectEvent.ACTION_SET_TOOL)
				{
					outerInstance.fireToolbarAppearanceChanged();
				}
				else if (act == ProjectEvent.ACTION_SET_FILE)
				{
					LogisimFile old = (LogisimFile) e.OldData;
					if (old != null)
					{
						ToolbarData data = old.Options.ToolbarData;
						data.removeToolbarListener(this);
						data.removeToolAttributeListener(this);
					}
					LogisimFile file = (LogisimFile) e.Data;
					if (file != null)
					{
						ToolbarData data = file.Options.ToolbarData;
						data.addToolbarListener(this);
						data.addToolAttributeListener(this);
					}
					outerInstance.buildContents();
				}
			}

			//
			// ToolbarListener methods
			//
			public virtual void toolbarChanged()
			{
				outerInstance.buildContents();
			}

			//
			// AttributeListener methods
			//
			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				outerInstance.fireToolbarAppearanceChanged();
			}

			//
			// PropertyChangeListener method
			//
			public virtual void propertyChange(PropertyChangeEvent @event)
			{
				if (AppPreferences.GATE_SHAPE.isSource(@event))
				{
					outerInstance.fireToolbarAppearanceChanged();
				}
			}
		}

		private Frame frame;
		private Project proj;
		private MyListener myListener;
		private IList<ToolbarItem> items;
		private Tool haloedTool;

		public LayoutToolbarModel(Frame frame, Project proj)
		{
			this.frame = frame;
			this.proj = proj;
			myListener = new MyListener(this);
			items = Collections.emptyList();
			haloedTool = null;
			buildContents();

			// set up listeners
			ToolbarData data = proj.Options.ToolbarData;
			data.addToolbarListener(myListener);
			data.addToolAttributeListener(myListener);
			AppPreferences.GATE_SHAPE.addPropertyChangeListener(myListener);
			proj.addProjectListener(myListener);
		}

		public override IList<ToolbarItem> Items
		{
			get
			{
				return items;
			}
		}

		public override bool isSelected(ToolbarItem item)
		{
			if (item is ToolItem)
			{
				Tool tool = ((ToolItem) item).tool;
				return tool == proj.Tool;
			}
			else
			{
				return false;
			}
		}

		public override void itemSelected(ToolbarItem item)
		{
			if (item is ToolItem)
			{
				Tool tool = ((ToolItem) item).tool;
				proj.Tool = tool;
			}
		}

		public virtual Tool HaloedTool
		{
			set
			{
				if (haloedTool != value)
				{
					haloedTool = value;
					fireToolbarAppearanceChanged();
				}
			}
		}

		private void buildContents()
		{
			IList<ToolbarItem> oldItems = items;
			IList<ToolbarItem> newItems = new List<ToolbarItem>();
			ToolbarData data = proj.LogisimFile.Options.ToolbarData;
			foreach (Tool tool in data.Contents)
			{
				if (tool == null)
				{
					newItems.Add(new ToolbarSeparator(4));
				}
				else
				{
					ToolbarItem i = findItem(oldItems, tool);
					if (i == null)
					{
						newItems.Add(new ToolItem(this, tool));
					}
					else
					{
						newItems.Add(i);
					}
				}
			}
			items = newItems.AsReadOnly();
			fireToolbarContentsChanged();
		}

		private static ToolbarItem findItem(IList<ToolbarItem> items, Tool tool)
		{
			foreach (ToolbarItem item in items)
			{
				if (item is ToolItem)
				{
					if (tool == ((ToolItem) item).tool)
					{
						return item;
					}
				}
			}
			return null;
		}
	}

}
