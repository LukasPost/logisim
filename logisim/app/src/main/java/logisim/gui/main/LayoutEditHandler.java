/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import logisim.circuit.Circuit;
import logisim.file.LibraryEvent;
import logisim.file.LibraryListener;
import logisim.gui.menu.LogisimMenuBar;
import logisim.proj.Action;
import logisim.proj.Project;
import logisim.proj.ProjectEvent;
import logisim.proj.ProjectListener;
import logisim.std.base.Base;
import logisim.tools.Library;
import logisim.tools.Tool;

public class LayoutEditHandler extends EditHandler implements ProjectListener, LibraryListener, PropertyChangeListener {
	private Frame frame;

	LayoutEditHandler(Frame frame) {
		this.frame = frame;

		Project proj = frame.getProject();
		Clipboard.addPropertyChangeListener(Clipboard.contentsProperty, this);
		proj.addProjectListener(this);
		proj.addLibraryListener(this);
	}

	@Override
	public void computeEnabled() {
		Project proj = frame.getProject();
		if(proj == null)
			return;
		Selection sel = proj.getSelection();
		boolean selEmpty = sel == null || sel.isEmpty();
		boolean canChange = proj.getLogisimFile().contains(proj.getCurrentCircuit());

		boolean selectAvailable = false;
		for (Library lib : proj.getLogisimFile().getLibraries())
			if (lib instanceof Base) {
				selectAvailable = true;
				break;
			}

		setEnabled(LogisimMenuBar.CUT, !selEmpty && selectAvailable && canChange);
		setEnabled(LogisimMenuBar.COPY, !selEmpty && selectAvailable);
		setEnabled(LogisimMenuBar.PASTE, selectAvailable && canChange && !Clipboard.isEmpty());
		setEnabled(LogisimMenuBar.DELETE, !selEmpty && selectAvailable && canChange);
		setEnabled(LogisimMenuBar.DUPLICATE, !selEmpty && selectAvailable && canChange);
		setEnabled(LogisimMenuBar.SELECT_ALL, selectAvailable);
		setEnabled(LogisimMenuBar.RAISE, false);
		setEnabled(LogisimMenuBar.LOWER, false);
		setEnabled(LogisimMenuBar.RAISE_TOP, false);
		setEnabled(LogisimMenuBar.LOWER_BOTTOM, false);
		setEnabled(LogisimMenuBar.ADD_CONTROL, false);
		setEnabled(LogisimMenuBar.REMOVE_CONTROL, false);
	}

	@Override
	public void cut() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		proj.doAction(SelectionActions.cut(sel));
	}

	@Override
	public void copy() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		proj.doAction(SelectionActions.copy(sel));
	}

	@Override
	public void paste() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		selectSelectTool(proj);
		Action action = SelectionActions.pasteMaybe(proj, sel);
		proj.doAction(action);
	}

	@Override
	public void delete() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		proj.doAction(SelectionActions.clear(sel));
	}

	@Override
	public void duplicate() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		proj.doAction(SelectionActions.duplicate(sel));
	}

	@Override
	public void selectAll() {
		Project proj = frame.getProject();
		Selection sel = frame.getCanvas().getSelection();
		selectSelectTool(proj);
		Circuit circ = proj.getCurrentCircuit();
		sel.addAll(circ.getWires());
		sel.addAll(circ.getNonWires());
		proj.repaintCanvas();
	}

	@Override
	public void raise() {
		// not yet supported in layout mode
	}

	@Override
	public void lower() {
		// not yet supported in layout mode
	}

	@Override
	public void raiseTop() {
		// not yet supported in layout mode
	}

	@Override
	public void lowerBottom() {
		// not yet supported in layout mode
	}

	@Override
	public void addControlPoint() {
		// not yet supported in layout mode
	}

	@Override
	public void removeControlPoint() {
		// not yet supported in layout mode
	}

	private void selectSelectTool(Project proj) {
		for (Library sub : proj.getLogisimFile().getLibraries())
			if (sub instanceof Base base) {
				Tool tool = base.getTool("Edit Tool");
				if (tool != null)
					proj.setTool(tool);
			}
	}

	public void projectChanged(ProjectEvent e) {
		int action = e.getAction();
		if (action == ProjectEvent.ACTION_SET_FILE) computeEnabled();
		else if (action == ProjectEvent.ACTION_SET_CURRENT) computeEnabled();
		else if (action == ProjectEvent.ACTION_SELECTION) computeEnabled();
	}

	public void libraryChanged(LibraryEvent e) {
		int action = e.getAction();
		if (action == LibraryEvent.ADD_LIBRARY) computeEnabled();
		else if (action == LibraryEvent.REMOVE_LIBRARY) computeEnabled();
	}

	public void propertyChange(PropertyChangeEvent event) {
		if (event.getPropertyName().equals(Clipboard.contentsProperty)) computeEnabled();
	}
}
