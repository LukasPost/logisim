/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.appear;

import java.util.HashMap;
import java.util.Map;

import logisim.gui.main.EditHandler;
import logisim.gui.main.EditHandler.Listener;
import logisim.gui.menu.EditPopup;
import logisim.gui.menu.LogisimMenuBar;
import logisim.gui.menu.LogisimMenuItem;

public class AppearanceEditPopup extends EditPopup implements Listener {
	private AppearanceCanvas canvas;
	private EditHandler handler;
	private Map<LogisimMenuItem, Boolean> enabled;

	public AppearanceEditPopup(AppearanceCanvas canvas) {
		super(true);
		this.canvas = canvas;
		handler = new AppearanceEditHandler(canvas);
		handler.setListener(this);
		enabled = new HashMap<>();
		handler.computeEnabled();
		initialize();
	}

	public void enableChanged(EditHandler handler, LogisimMenuItem action, boolean value) {
		enabled.put(action, value);
	}

	@Override
	protected boolean shouldShow(LogisimMenuItem item) {
		return item != LogisimMenuBar.ADD_CONTROL && item != LogisimMenuBar.REMOVE_CONTROL || canvas.getSelection().getSelectedHandle() != null;
	}

	@Override
	protected boolean isEnabled(LogisimMenuItem item) {
		Boolean value = enabled.get(item);
		return value != null && value;
	}

	@Override
	protected void fire(LogisimMenuItem item) {
		if (item == LogisimMenuBar.CUT) handler.cut();
		else if (item == LogisimMenuBar.COPY) handler.copy();
		else if (item == LogisimMenuBar.DELETE) handler.delete();
		else if (item == LogisimMenuBar.DUPLICATE) handler.duplicate();
		else if (item == LogisimMenuBar.RAISE) handler.raise();
		else if (item == LogisimMenuBar.LOWER) handler.lower();
		else if (item == LogisimMenuBar.RAISE_TOP) handler.raiseTop();
		else if (item == LogisimMenuBar.LOWER_BOTTOM) handler.lowerBottom();
		else if (item == LogisimMenuBar.ADD_CONTROL) handler.addControlPoint();
		else if (item == LogisimMenuBar.REMOVE_CONTROL) handler.removeControlPoint();
	}
}
