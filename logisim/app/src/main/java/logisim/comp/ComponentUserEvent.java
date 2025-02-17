/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.comp;

import logisim.circuit.CircuitState;
import logisim.gui.main.Canvas;

public class ComponentUserEvent {
	private Canvas canvas;
	private int x;
	private int y;

	ComponentUserEvent(Canvas canvas) {
		this.canvas = canvas;
	}

	public ComponentUserEvent(Canvas canvas, int x, int y) {
		this.canvas = canvas;
		this.x = x;
		this.y = y;
	}

	public Canvas getCanvas() {
		return canvas;
	}

	public CircuitState getCircuitState() {
		return canvas.getCircuitState();
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}
}
