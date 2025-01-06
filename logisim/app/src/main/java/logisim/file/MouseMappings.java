/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import java.awt.event.MouseEvent;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map.Entry;
import java.util.Set;
import java.util.Map;

import logisim.comp.ComponentFactory;
import logisim.data.AttributeSets;
import logisim.tools.AddTool;
import logisim.tools.SelectTool;
import logisim.tools.Tool;

public class MouseMappings {
	public interface MouseMappingsListener {
		void mouseMappingsChanged();
	}

	private ArrayList<MouseMappingsListener> listeners;
	private HashMap<Integer, Tool> map;
	private int cache_mods;
	private Tool cache_tool;

	public MouseMappings() {
		listeners = new ArrayList<>();
		map = new HashMap<>();
	}

	//
	// listener methods
	//
	public void addMouseMappingsListener(MouseMappingsListener l) {
		listeners.add(l);
	}

	public void removeMouseMappingsListener(MouseMappingsListener l) {
		listeners.add(l);
	}

	private void fireMouseMappingsChanged() {
		for (MouseMappingsListener l : listeners) l.mouseMappingsChanged();
	}

	//
	// query methods
	//
	public Map<Integer, Tool> getMappings() {
		return map;
	}

	public Set<Integer> getMappedModifiers() {
		return map.keySet();
	}

	public Tool getToolFor(MouseEvent e) {
		return getToolFor(e.getModifiersEx());
	}

	public Tool getToolFor(int mods) {
		if (mods == cache_mods) return cache_tool;
		else {
			Tool ret = map.get(mods);
			cache_mods = mods;
			cache_tool = ret;
			return ret;
		}
	}

	public Tool getToolFor(Integer mods) {
		if (mods == cache_mods) return cache_tool;
		else {
			Tool ret = map.get(mods);
			cache_mods = mods;
			cache_tool = ret;
			return ret;
		}
	}

	public boolean usesToolFromSource(Tool query) {
		for (Tool tool : map.values()) if (tool.sharesSource(query)) return true;
		return false;
	}

	public boolean containsSelectTool() {
		for (Tool tool : map.values())
			if (tool instanceof SelectTool)
				return true;
		return false;
	}

	//
	// modification methods
	//
	public void copyFrom(MouseMappings other, LogisimFile file) {
		if (this == other)
			return;
		cache_mods = -1;
		map.clear();
		for (Integer mods : other.map.keySet()) {
			Tool srcTool = other.map.get(mods);
			Tool dstTool = file.findTool(srcTool);
			if (dstTool != null) {
				dstTool = dstTool.cloneTool();
				AttributeSets.copy(srcTool.getAttributeSet(), dstTool.getAttributeSet());
				map.put(mods, dstTool);
			}
		}
		fireMouseMappingsChanged();
	}

	public void setToolFor(MouseEvent e, Tool tool) {
		setToolFor(e.getModifiersEx(), tool);
	}

	public void setToolFor(int mods, Tool tool) {
		if (mods == cache_mods)
			cache_mods = -1;

		if (tool == null) {
			Object old = map.remove(mods);
			if (old != null)
				fireMouseMappingsChanged();
		} else {
			Object old = map.put(mods, tool);
			if (old != tool)
				fireMouseMappingsChanged();
		}
	}

	public void setToolFor(Integer mods, Tool tool) {
		if (mods == cache_mods)
			cache_mods = -1;

		if (tool == null) {
			Object old = map.remove(mods);
			if (old != null)
				fireMouseMappingsChanged();
		} else {
			Object old = map.put(mods, tool);
			if (old != tool)
				fireMouseMappingsChanged();
		}
	}

	//
	// package-protected methods
	//
	void replaceAll(Map<Tool, Tool> toolMap) {
		boolean changed = false;
		for (Entry<Integer, Tool> entry : map.entrySet()) {
			Integer key = entry.getKey();
			Tool tool = entry.getValue();
			if (tool instanceof AddTool at) {
				ComponentFactory factory = at.getFactory();
				Tool newTool = toolMap.get((Tool)factory);
				if (newTool != null) {
					changed = true;
					Tool clone = newTool.cloneTool();
					LoadedLibrary.copyAttributes(clone.getAttributeSet(), tool.getAttributeSet());
					map.put(key, clone);
				}
			} else if (toolMap.containsKey(tool)) {
				changed = true;
				Tool newTool = toolMap.get(tool);
				if (newTool == null) map.remove(key);
				else {
					Tool clone = newTool.cloneTool();
					LoadedLibrary.copyAttributes(clone.getAttributeSet(), tool.getAttributeSet());
					map.put(key, clone);
				}
			}

		}
		if (changed)
			fireMouseMappingsChanged();
	}
}
