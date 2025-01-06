/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.awt.Graphics;
import java.awt.event.MouseEvent;
import java.awt.event.KeyEvent;
import java.util.ArrayList;

import logisim.data.Bounds;

public class AbstractCaret implements Caret {
	private ArrayList<CaretListener> listeners = new ArrayList<>();
	private Bounds bds = Bounds.EMPTY_BOUNDS;

	public AbstractCaret() {
	}

	// listener methods
	public void addCaretListener(CaretListener e) {
		listeners.add(e);
	}

	public void removeCaretListener(CaretListener e) {
		listeners.remove(e);
	}

	// query/Graphics methods
	public String getText() {
		return "";
	}

	public Bounds getBounds(Graphics g) {
		return bds;
	}

	public void draw(Graphics g) {
	}

	// finishing
	public void commitText(String text) {
	}

	public void cancelEditing() {
	}

	public void stopEditing() {
	}

	// events to handle
	public void mousePressed(MouseEvent e) {
	}

	public void mouseDragged(MouseEvent e) {
	}

	public void mouseReleased(MouseEvent e) {
	}

	public void keyPressed(KeyEvent e) {
	}

	public void keyReleased(KeyEvent e) {
	}

	public void keyTyped(KeyEvent e) {
	}
}
