/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.key;

import java.awt.event.KeyEvent;

import logisim.data.Attribute;
import logisim.data.Direction;

public class DirectionConfigurator implements KeyConfigurator, Cloneable {
	private Attribute<Direction> attr;
	private int modsEx;

	public DirectionConfigurator(Attribute<Direction> attr, int modifiersEx) {
		this.attr = attr;
		modsEx = modifiersEx;
	}

	@Override
	public DirectionConfigurator clone() {
		try {
			return (DirectionConfigurator) super.clone();
		}
		catch (CloneNotSupportedException e) {
			e.printStackTrace();
			return null;
		}
	}

	public KeyConfigurationResult keyEventReceived(KeyConfigurationEvent event) {
		if (event.getType() == KeyConfigurationEvent.KEY_PRESSED) {
			KeyEvent e = event.getKeyEvent();
			if (e.getModifiersEx() == modsEx) {
				Direction value = switch (e.getKeyCode()) {
					case KeyEvent.VK_UP -> Direction.North;
					case KeyEvent.VK_DOWN -> Direction.South;
					case KeyEvent.VK_LEFT -> Direction.West;
					case KeyEvent.VK_RIGHT -> Direction.East;
					default -> null;
				};
				if (value != null) {
					event.consume();
					return new KeyConfigurationResult(event, attr, value);
				}
			}
		}
		return null;
	}
}
