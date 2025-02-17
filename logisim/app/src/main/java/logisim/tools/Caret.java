/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.awt.Graphics;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;

import logisim.data.Bounds;

public interface Caret {
	// listener methods
	void addCaretListener(CaretListener e);

	void removeCaretListener(CaretListener e);

	// query/Graphics methods
	String getText();

	Bounds getBounds(Graphics g);

	void draw(Graphics g);

	// finishing
	void commitText(String text);

	void cancelEditing();

	void stopEditing();

	// events to handle
	void mousePressed(MouseEvent e);

	void mouseDragged(MouseEvent e);

	void mouseReleased(MouseEvent e);

	void keyPressed(KeyEvent e);

	void keyReleased(KeyEvent e);

	void keyTyped(KeyEvent e);
}
