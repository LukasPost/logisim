/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import javax.swing.JPopupMenu;

import logisim.circuit.Circuit;
import logisim.circuit.SubcircuitFactory;
import logisim.comp.ComponentFactory;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeSet;
import logisim.file.LibraryEvent;
import logisim.file.LibraryEventSource;
import logisim.file.LibraryListener;
import logisim.file.LogisimFile;
import logisim.file.LogisimFileActions;
import logisim.gui.generic.AttrTableModel;
import logisim.gui.main.ProjectExplorer.Event;
import logisim.gui.main.ProjectExplorer.Listener;
import logisim.gui.menu.ProjectCircuitActions;
import logisim.gui.menu.ProjectLibraryActions;
import logisim.gui.menu.Popups;
import logisim.proj.Project;
import logisim.proj.ProjectEvent;
import logisim.proj.ProjectListener;
import logisim.tools.AddTool;
import logisim.tools.Library;
import logisim.tools.Tool;

class ToolboxManip implements Listener {
	private class MyListener implements ProjectListener, LibraryListener, AttributeListener {
		private LogisimFile curFile;

		public void projectChanged(ProjectEvent event) {
			int action = event.getAction();
			if (action == ProjectEvent.ACTION_SET_FILE) {
				setFile((LogisimFile) event.getOldData(), (LogisimFile) event.getData());
				explorer.repaint();
			}
		}

		private void setFile(LogisimFile oldFile, LogisimFile newFile) {
			if (oldFile != null) {
				removeLibrary(oldFile);
				for (Library lib : oldFile.getLibraries()) removeLibrary(lib);
			}
			curFile = newFile;
			if (newFile != null) {
				addLibrary(newFile);
				for (Library lib : newFile.getLibraries()) addLibrary(lib);
			}
		}

		public void libraryChanged(LibraryEvent event) {
			int action = event.getAction();
			if (action == LibraryEvent.ADD_LIBRARY) {
				if (event.getSource() == curFile) addLibrary((Library) event.getData());
			} else if (action == LibraryEvent.REMOVE_LIBRARY) {
				if (event.getSource() == curFile) removeLibrary((Library) event.getData());
			} else if (action == LibraryEvent.ADD_TOOL) {
				Tool tool = (Tool) event.getData();
				AttributeSet attrs = tool.getAttributeSet();
				if (attrs != null)
					attrs.addAttributeListener(this);
			} else if (action == LibraryEvent.REMOVE_TOOL) {
				Tool tool = (Tool) event.getData();
				AttributeSet attrs = tool.getAttributeSet();
				if (attrs != null)
					attrs.removeAttributeListener(this);
			}
			explorer.repaint();
		}

		private void addLibrary(Library lib) {
			if (lib instanceof LibraryEventSource) ((LibraryEventSource) lib).addLibraryListener(this);
			for (Tool tool : lib.getTools()) {
				AttributeSet attrs = tool.getAttributeSet();
				if (attrs != null)
					attrs.addAttributeListener(this);
			}
		}

		private void removeLibrary(Library lib) {
			if (lib instanceof LibraryEventSource) ((LibraryEventSource) lib).removeLibraryListener(this);
			for (Tool tool : lib.getTools()) {
				AttributeSet attrs = tool.getAttributeSet();
				if (attrs != null)
					attrs.removeAttributeListener(this);
			}
		}

		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			explorer.repaint();
		}

	}

	private Project proj;
	private ProjectExplorer explorer;
	private Tool lastSelected;

	ToolboxManip(Project proj, ProjectExplorer explorer) {
		this.proj = proj;
		this.explorer = explorer;
		MyListener myListener = new MyListener();
		proj.addProjectListener(myListener);
		myListener.setFile(null, proj.getLogisimFile());
	}

	public void selectionChanged(Event event) {
		Object selected = event.getTarget();
		if (selected instanceof Tool tool) {
			if (selected instanceof AddTool addTool) {
				ComponentFactory source = addTool.getFactory();
				if (source instanceof SubcircuitFactory circFact) {
					Circuit circ = circFact.getSubcircuit();
					if (proj.getCurrentCircuit() == circ) {
						AttrTableModel m = new AttrTableCircuitModel(proj, circ);
						proj.getFrame().setAttrTableModel(m);
						return;
					}
				}
			}

			lastSelected = proj.getTool();
			proj.setTool(tool);
			proj.getFrame().viewAttributes(tool);
		}
	}

	public void doubleClicked(Event event) {
		Object clicked = event.getTarget();
		if (clicked instanceof AddTool tool) {
			ComponentFactory source = tool.getFactory();
			if (source instanceof SubcircuitFactory circFact) {
				proj.setCurrentCircuit(circFact.getSubcircuit());
				proj.getFrame().setEditorView(Frame.EDIT_LAYOUT);
				if (lastSelected != null)
					proj.setTool(lastSelected);
			}
		}
	}

	public void moveRequested(Event event, AddTool dragged, AddTool target) {
		LogisimFile file = proj.getLogisimFile();
		int draggedIndex = file.getTools().indexOf(dragged);
		int targetIndex = file.getTools().indexOf(target);
		if (targetIndex > draggedIndex)
			targetIndex++;
		proj.doAction(LogisimFileActions.moveCircuit(dragged, targetIndex));
	}

	public void deleteRequested(Event event) {
		Object request = event.getTarget();
		if (request instanceof Library) ProjectLibraryActions.doUnloadLibrary(proj, (Library) request);
		else if (request instanceof AddTool) {
			ComponentFactory factory = ((AddTool) request).getFactory();
			if (factory instanceof SubcircuitFactory circFact)
				ProjectCircuitActions.doRemoveCircuit(proj, circFact.getSubcircuit());
		}
	}

	public JPopupMenu menuRequested(Event event) {
		Object clicked = event.getTarget();
		if (clicked instanceof AddTool tool) {
			ComponentFactory source = tool.getFactory();
			if (source instanceof SubcircuitFactory) {
				Circuit circ = ((SubcircuitFactory) source).getSubcircuit();
				return Popups.forCircuit(proj, circ);
			} else return null;
		} else if (clicked instanceof Tool) return null;
		else if (clicked == proj.getLogisimFile()) return Popups.forProject(proj);
		else if (clicked instanceof Library) {
			boolean is_top = event.getTreePath().getPathCount() <= 2;
			return Popups.forLibrary(proj, (Library) clicked, is_top);
		} else return null;
	}

}
