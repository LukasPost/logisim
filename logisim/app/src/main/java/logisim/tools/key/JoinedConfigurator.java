/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools.key;

public class JoinedConfigurator implements KeyConfigurator, Cloneable {
	public static JoinedConfigurator create(KeyConfigurator a, KeyConfigurator b) {
		return new JoinedConfigurator(new KeyConfigurator[] { a, b });
	}

	public static JoinedConfigurator create(KeyConfigurator[] configs) {
		return new JoinedConfigurator(configs);
	}

	private KeyConfigurator[] handlers;

	private JoinedConfigurator(KeyConfigurator[] handlers) {
		this.handlers = handlers;
	}

	@Override
	public JoinedConfigurator clone() {
		JoinedConfigurator ret;
		try {
			ret = (JoinedConfigurator) super.clone();
		}
		catch (CloneNotSupportedException e) {
			e.printStackTrace();
			return null;
		}
		int len = handlers.length;
		ret.handlers = new KeyConfigurator[len];
		for (int i = 0; i < len; i++) ret.handlers[i] = handlers[i].clone();
		return ret;
	}

	public KeyConfigurationResult keyEventReceived(KeyConfigurationEvent event) {
		KeyConfigurator[] hs = handlers;
		if (event.isConsumed()) return null;
		for (KeyConfigurator h : hs) {
			KeyConfigurationResult result = h.keyEventReceived(event);
			if (result != null || event.isConsumed()) return result;
		}
		return null;
	}
}
