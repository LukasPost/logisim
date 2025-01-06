/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.generic;

import java.beans.PropertyChangeListener;

public interface ZoomModel {
	String ZOOM = "zoom";
	String SHOW_GRID = "grid";

	void addPropertyChangeListener(String prop, PropertyChangeListener l);

	void removePropertyChangeListener(String prop, PropertyChangeListener l);

	boolean getShowGrid();

	double getZoomFactor();

	double[] getZoomOptions();

	void setShowGrid(boolean value);

	void setZoomFactor(double value);
}
