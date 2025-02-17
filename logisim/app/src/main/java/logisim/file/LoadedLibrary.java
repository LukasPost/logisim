/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;

import logisim.circuit.Circuit;
import logisim.circuit.CircuitMutation;
import logisim.circuit.SubcircuitFactory;
import logisim.comp.Component;
import logisim.comp.ComponentFactory;
import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.proj.Project;
import logisim.proj.Projects;
import logisim.tools.AddTool;
import logisim.tools.Library;
import logisim.tools.Tool;
import logisim.util.EventSourceWeakSupport;

public class LoadedLibrary extends Library implements LibraryEventSource {
	private class MyListener implements LibraryListener {
		public void libraryChanged(LibraryEvent event) {
			fireLibraryEvent(event);
		}
	}

	private Library base;
	private boolean dirty;
	private MyListener myListener;
	private EventSourceWeakSupport<LibraryListener> listeners;

	LoadedLibrary(Library base) {
		dirty = false;
		myListener = new MyListener();
		listeners = new EventSourceWeakSupport<>();

		while (base instanceof LoadedLibrary)
			base = ((LoadedLibrary) base).base;
		this.base = base;
		if (base instanceof LibraryEventSource) ((LibraryEventSource) base).addLibraryListener(myListener);
	}

	public void addLibraryListener(LibraryListener l) {
		listeners.add(l);
	}

	public void removeLibraryListener(LibraryListener l) {
		listeners.remove(l);
	}

	@Override
	public String getName() {
		return base.getName();
	}

	@Override
	public String getDisplayName() {
		return base.getDisplayName();
	}

	@Override
	public boolean isDirty() {
		return dirty || base.isDirty();
	}

	@Override
	public List<? extends Tool> getTools() {
		return base.getTools();
	}

	@Override
	public List<Library> getLibraries() {
		return base.getLibraries();
	}

	void setDirty(boolean value) {
		if (dirty != value) {
			dirty = value;
			fireLibraryEvent(LibraryEvent.DIRTY_STATE, isDirty() ? Boolean.TRUE : Boolean.FALSE);
		}
	}

	Library getBase() {
		return base;
	}

	void setBase(Library value) {
		if (base instanceof LibraryEventSource) ((LibraryEventSource) base).removeLibraryListener(myListener);
		Library old = base;
		base = value;
		resolveChanges(old);
		if (base instanceof LibraryEventSource) ((LibraryEventSource) base).addLibraryListener(myListener);
	}

	private void fireLibraryEvent(int action, Object data) {
		fireLibraryEvent(new LibraryEvent(this, action, data));
	}

	private void fireLibraryEvent(LibraryEvent event) {
		if (event.getSource() != this) event = new LibraryEvent(this, event.getAction(), event.getData());
		for (LibraryListener l : listeners) l.libraryChanged(event);
	}

	private void resolveChanges(Library old) {
		if (listeners.isEmpty())
			return;

		if (!base.getDisplayName().equals(old.getDisplayName()))
			fireLibraryEvent(LibraryEvent.SET_NAME, base.getDisplayName());

		HashSet<Library> changes = new HashSet<>(old.getLibraries());
		changes.removeAll(base.getLibraries());
		for (Library lib : changes) fireLibraryEvent(LibraryEvent.REMOVE_LIBRARY, lib);

		changes.clear();
		changes.addAll(base.getLibraries());
		changes.removeAll(old.getLibraries());
		for (Library lib : changes) fireLibraryEvent(LibraryEvent.ADD_LIBRARY, lib);

		HashMap<ComponentFactory, ComponentFactory> componentMap = new HashMap<>();
		HashMap<Tool, Tool> toolMap = new HashMap<>();
		for (Tool oldTool : old.getTools()) {
			Tool newTool = base.getTool(oldTool.getName());
			toolMap.put(oldTool, newTool);
			if (oldTool instanceof AddTool) {
				ComponentFactory oldFactory = ((AddTool) oldTool).getFactory();
				if (newTool instanceof AddTool addTool) {
					ComponentFactory newFactory = addTool.getFactory();
					componentMap.put(oldFactory, newFactory);
				} else componentMap.put(oldFactory, null);
			}
		}
		replaceAll(componentMap, toolMap);

		HashSet<Tool> toolChanges = new HashSet<>(old.getTools());
		toolChanges.removeAll(toolMap.keySet());
		for (Tool tool : toolChanges) fireLibraryEvent(LibraryEvent.REMOVE_TOOL, tool);

		toolChanges = new HashSet<>(base.getTools());
		toolChanges.removeAll(toolMap.values());
		for (Tool tool : toolChanges) fireLibraryEvent(LibraryEvent.ADD_TOOL, tool);
	}

	private static void replaceAll(Map<ComponentFactory, ComponentFactory> compMap, Map<Tool, Tool> toolMap) {
		for (Project proj : Projects.getOpenProjects()) {
			Tool oldTool = proj.getTool();
			Circuit oldCircuit = proj.getCurrentCircuit();
			if (toolMap.containsKey(oldTool)) proj.setTool(toolMap.get(oldTool));
			SubcircuitFactory oldFactory = oldCircuit.getSubcircuitFactory();
			if (compMap.containsKey(oldFactory)) {
				SubcircuitFactory newFactory = (SubcircuitFactory) compMap.get(oldFactory);
				proj.setCurrentCircuit(newFactory.getSubcircuit());
			}
			replaceAll(proj.getLogisimFile(), compMap, toolMap);
		}
		for (LogisimFile file : LibraryManager.instance.getLogisimLibraries()) replaceAll(file, compMap, toolMap);
	}

	private static void replaceAll(LogisimFile file, Map<ComponentFactory, ComponentFactory> compMap,
			Map<Tool, Tool> toolMap) {
		file.getOptions().getToolbarData().replaceAll(toolMap);
		file.getOptions().getMouseMappings().replaceAll(toolMap);
		for (Circuit circuit : file.getCircuits()) replaceAll(circuit, compMap);
	}

	private static void replaceAll(Circuit circuit, Map<ComponentFactory, ComponentFactory> compMap) {
		ArrayList<Component> toReplace = null;
		for (Component comp : circuit.getNonWires())
			if (compMap.containsKey(comp.getFactory())) {
				if (toReplace == null)
					toReplace = new ArrayList<>();
				toReplace.add(comp);
			}
		if (toReplace != null) {
			CircuitMutation xn = new CircuitMutation(circuit);
			for (Component comp : toReplace) {
				xn.remove(comp);
				ComponentFactory factory = compMap.get(comp.getFactory());
				if (factory != null) {
					AttributeSet newAttrs = createAttributes(factory, comp.getAttributeSet());
					xn.add(factory.createComponent(comp.getLocation(), newAttrs));
				}
			}
			xn.execute();
		}
	}

	private static AttributeSet createAttributes(ComponentFactory factory, AttributeSet src) {
		AttributeSet dest = factory.createAttributeSet();
		copyAttributes(dest, src);
		return dest;
	}

	static void copyAttributes(AttributeSet dest, AttributeSet src) {
		for (Attribute<?> destAttr : dest.getAttributes()) {
			Attribute<?> srcAttr = src.getAttribute(destAttr.getName());
			if (srcAttr != null) {
				@SuppressWarnings("unchecked")
				Attribute<Object> destAttr2 = (Attribute<Object>) destAttr;
				dest.setValue(destAttr2, src.getValue(srcAttr));
			}
		}
	}
}
