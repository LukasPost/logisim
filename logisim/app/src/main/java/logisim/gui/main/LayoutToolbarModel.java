/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.awt.Component;
import java.awt.Graphics;
import java.awt.Color;
import java.awt.Dimension;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import draw.toolbar.AbstractToolbarModel;
import draw.toolbar.ToolbarItem;
import draw.toolbar.ToolbarSeparator;
import logisim.comp.ComponentDrawContext;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.file.LogisimFile;
import logisim.file.ToolbarData;
import logisim.file.ToolbarData.ToolbarListener;
import logisim.prefs.AppPreferences;
import logisim.proj.Project;
import logisim.proj.ProjectEvent;
import logisim.proj.ProjectListener;
import logisim.tools.Tool;
import logisim.util.InputEventUtil;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

class LayoutToolbarModel extends AbstractToolbarModel {
	private class ToolItem implements ToolbarItem {
		private Tool tool;

		ToolItem(Tool tool) {
			this.tool = tool;
		}

		public boolean isSelectable() {
			return true;
		}

		public void paintIcon(Component destination, Graphics g) {
			// draw halo
			if (tool == haloedTool && AppPreferences.ATTRIBUTE_HALO.getBoolean()) {
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

		public String getToolTip() {
			String ret = tool.getDescription();
			int index = 1;
			for (ToolbarItem item : items) {
				if (item == this)
					break;
				if (item instanceof ToolItem)
					++index;
			}
			if (index <= 10) {
				if (index == 10)
					index = 0;
				int mask = frame.getToolkit().getMenuShortcutKeyMaskEx();
				ret += " (" + InputEventUtil.toKeyDisplayString(mask) + "-" + index + ")";
			}
			return ret;
		}

		public Dimension getDimension(Object orientation) {
			return new Dimension(24, 24);
		}
	}

	private class MyListener
			implements ProjectListener, AttributeListener, ToolbarListener, PropertyChangeListener {
		//
		// ProjectListener methods
		//
		public void projectChanged(ProjectEvent e) {
			int act = e.getAction();
			if (act == ProjectEvent.ACTION_SET_TOOL) fireToolbarAppearanceChanged();
			else if (act == ProjectEvent.ACTION_SET_FILE) {
				LogisimFile old = (LogisimFile) e.getOldData();
				if (old != null) {
					ToolbarData data = old.getOptions().getToolbarData();
					data.removeToolbarListener(this);
					data.removeToolAttributeListener(this);
				}
				LogisimFile file = (LogisimFile) e.getData();
				if (file != null) {
					ToolbarData data = file.getOptions().getToolbarData();
					data.addToolbarListener(this);
					data.addToolAttributeListener(this);
				}
				buildContents();
			}
		}

		//
		// ToolbarListener methods
		//
		public void toolbarChanged() {
			buildContents();
		}

		//
		// AttributeListener methods
		//
		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			fireToolbarAppearanceChanged();
		}

		//
		// PropertyChangeListener method
		//
		public void propertyChange(PropertyChangeEvent event) {
			if (AppPreferences.GATE_SHAPE.isSource(event)) fireToolbarAppearanceChanged();
		}
	}

	private Frame frame;
	private Project proj;
	private List<ToolbarItem> items;
	private Tool haloedTool;

	public LayoutToolbarModel(Frame frame, Project proj) {
		this.frame = frame;
		this.proj = proj;
		MyListener myListener = new MyListener();
		items = Collections.emptyList();
		haloedTool = null;
		buildContents();

		// set up listeners
		ToolbarData data = proj.getOptions().getToolbarData();
		data.addToolbarListener(myListener);
		data.addToolAttributeListener(myListener);
		AppPreferences.GATE_SHAPE.addPropertyChangeListener(myListener);
		proj.addProjectListener(myListener);
	}

	@Override
	public List<ToolbarItem> getItems() {
		return items;
	}

	@Override
	public boolean isSelected(ToolbarItem item) {
		if (item instanceof ToolItem) {
			Tool tool = ((ToolItem) item).tool;
			return tool == proj.getTool();
		} else return false;
	}

	@Override
	public void itemSelected(ToolbarItem item) {
		if (item instanceof ToolItem) {
			Tool tool = ((ToolItem) item).tool;
			proj.setTool(tool);
		}
	}

	public void setHaloedTool(Tool t) {
		if (haloedTool != t) {
			haloedTool = t;
			fireToolbarAppearanceChanged();
		}
	}

	private void buildContents() {
		List<ToolbarItem> oldItems = items;
		List<ToolbarItem> newItems = new ArrayList<>();
		ToolbarData data = proj.getLogisimFile().getOptions().getToolbarData();
		for (Tool tool : data.getContents())
			if (tool == null) newItems.add(new ToolbarSeparator(4));
			else {
				ToolbarItem i = findItem(oldItems, tool);
				if (i == null) newItems.add(new ToolItem(tool));
				else newItems.add(i);
			}
		items = Collections.unmodifiableList(newItems);
		fireToolbarContentsChanged();
	}

	private static ToolbarItem findItem(List<ToolbarItem> items, Tool tool) {
		for (ToolbarItem item : items) if (item instanceof ToolItem) if (tool == ((ToolItem) item).tool) return item;
		return null;
	}
}
