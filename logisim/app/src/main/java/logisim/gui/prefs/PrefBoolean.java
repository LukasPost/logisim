/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.prefs;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import javax.swing.JCheckBox;

import logisim.prefs.PrefMonitor;
import logisim.util.StringGetter;

class PrefBoolean extends JCheckBox implements ActionListener, PropertyChangeListener {
	private PrefMonitor<Boolean> pref;
	private StringGetter title;

	PrefBoolean(PrefMonitor<Boolean> pref, StringGetter title) {
		super(title.get());
		this.pref = pref;
		this.title = title;

		addActionListener(this);
		pref.addPropertyChangeListener(this);
		setSelected(pref.getBoolean());
	}

	void localeChanged() {
		setText(title.get());
	}

	public void actionPerformed(ActionEvent e) {
		pref.setBoolean(isSelected());
	}

	public void propertyChange(PropertyChangeEvent event) {
		if (pref.isSource(event)) setSelected(pref.getBoolean());
	}
}
