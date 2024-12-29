/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.main;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Vector;

import javax.swing.JList;

import logisim.circuit.Circuit;
import logisim.data.Bounds;
import logisim.file.LogisimFile;
import logisim.proj.Project;

class CircuitJList extends JList {
	public CircuitJList(Project proj, boolean includeEmpty) {
		LogisimFile file = proj.getLogisimFile();
		Circuit current = proj.getCurrentCircuit();
		Vector<Circuit> options = new Vector<Circuit>();
		boolean currentFound = false;
		for (Circuit circ : file.getCircuits()) {
			if (!includeEmpty || circ.getBounds() != Bounds.EMPTY_BOUNDS) {
				if (circ == current)
					currentFound = true;
				options.add(circ);
			}
		}

		setListData(options);
		if (currentFound)
			setSelectedValue(current, true);
		setVisibleRowCount(Math.min(6, options.size()));
	}

	public List<Circuit> getSelectedCircuits() {
		Object[] selected = getSelectedValues();
		if (selected != null && selected.length > 0) {
			ArrayList<Circuit> ret = new ArrayList<Circuit>(selected.length);
			for (Object sel : selected) {
				if (sel instanceof Circuit)
					ret.add((Circuit) sel);
			}
			return ret;
		} else {
			return Collections.emptyList();
		}
	}

}
