/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.comp;

public interface ComponentListener {
	public void endChanged(ComponentEvent e);

	public void componentInvalidated(ComponentEvent e);
}
