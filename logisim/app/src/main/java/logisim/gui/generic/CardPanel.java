/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.generic;

import java.awt.CardLayout;
import java.awt.Component;
import java.util.ArrayList;

import javax.swing.JPanel;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;

public class CardPanel extends JPanel {
	private ArrayList<ChangeListener> listeners;
	private String current;

	public CardPanel() {
		super(new CardLayout());
		listeners = new ArrayList<>();
		current = "";
	}

	public void addChangeListener(ChangeListener listener) {
		listeners.add(listener);
	}

	public void addView(String name, Component comp) {
		add(comp, name);
	}

	public String getView() {
		return current;
	}

	public void setView(String choice) {
		if (choice == null)
			choice = "";
		String oldChoice = current;
		if (!oldChoice.equals(choice)) {
			current = choice;
			((CardLayout) getLayout()).show(this, choice);
			ChangeEvent e = new ChangeEvent(this);
			for (ChangeListener listener : listeners) listener.stateChanged(e);
		}
	}

}
