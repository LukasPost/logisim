/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.memory;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.event.MouseEvent;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceData;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceLogger;
import logisim.instance.InstancePainter;
import logisim.instance.InstancePoker;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.util.GraphicsUtil;
import logisim.util.StringGetter;

abstract class AbstractFlipFlop extends InstanceFactory {
	private static final int STD_PORTS = 6;

	private Attribute<AttributeOption> triggerAttribute;

	protected AbstractFlipFlop(String name, String iconName, StringGetter desc, int numInputs,
			boolean allowLevelTriggers) {
		super(name, desc);
		setIconName(iconName);
		triggerAttribute = allowLevelTriggers ? StdAttr.TRIGGER : StdAttr.EDGE_TRIGGER;
		setAttributes(new Attribute[] { triggerAttribute, StdAttr.LABEL, StdAttr.LABEL_FONT },
				new Object[] { StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT });
		setOffsetBounds(Bounds.create(-40, -10, 40, 40));
		setInstancePoker(Poker.class);
		setInstanceLogger(Logger.class);

		Port[] ps = new Port[numInputs + STD_PORTS];
		if (numInputs == 1) {
			ps[0] = new Port(-40, 20, Port.INPUT, 1);
			ps[1] = new Port(-40, 0, Port.INPUT, 1);
		} else if (numInputs == 2) {
			ps[0] = new Port(-40, 0, Port.INPUT, 1);
			ps[1] = new Port(-40, 20, Port.INPUT, 1);
			ps[2] = new Port(-40, 10, Port.INPUT, 1);
		} else throw new RuntimeException("flip-flop input > 2");
		ps[numInputs + 1] = new Port(0, 0, Port.OUTPUT, 1);
		ps[numInputs + 2] = new Port(0, 20, Port.OUTPUT, 1);
		ps[numInputs + 3] = new Port(-10, 30, Port.INPUT, 1);
		ps[numInputs + 4] = new Port(-30, 30, Port.INPUT, 1);
		ps[numInputs + 5] = new Port(-20, 30, Port.INPUT, 1);
		ps[numInputs].setToolTip(Strings.getter("flipFlopClockTip"));
		ps[numInputs + 1].setToolTip(Strings.getter("flipFlopQTip"));
		ps[numInputs + 2].setToolTip(Strings.getter("flipFlopNotQTip"));
		ps[numInputs + 3].setToolTip(Strings.getter("flipFlopResetTip"));
		ps[numInputs + 4].setToolTip(Strings.getter("flipFlopPresetTip"));
		ps[numInputs + 5].setToolTip(Strings.getter("flipFlopEnableTip"));
		setPorts(ps);
	}

	//
	// abstract methods intended to be implemented in subclasses
	//
	protected abstract String getInputName(int index);

	protected abstract WireValue computeValue(WireValue[] inputs, WireValue curValue);

	//
	// concrete methods not intended to be overridden
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		Bounds bds = instance.getBounds();
		instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.getX() + bds.getWidth() / 2, bds.getY() - 3,
				GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
	}

	@Override
	public void propagate(InstanceState state) {
		StateData data = (StateData) state.getData();
		if (data == null) {
			data = new StateData();
			state.setData(data);
		}

		int n = getPorts().size() - STD_PORTS;
		Object triggerType = state.getAttributeValue(triggerAttribute);
		boolean triggered = data.updateClock(state.getPort(n), triggerType);

		if (state.getPort(n + 3) == WireValues.TRUE) // clear requested
			data.curValue = WireValues.FALSE;
		else if (state.getPort(n + 4) == WireValues.TRUE) // preset requested
			data.curValue = WireValues.TRUE;
		else if (triggered && state.getPort(n + 5) != WireValues.FALSE) {
			// Clock has triggered and flip-flop is enabled: Update the state
			WireValue[] inputs = new WireValue[n];
			for (int i = 0; i < n; i++) inputs[i] = state.getPort(i);

			WireValue newVal = computeValue(inputs, data.curValue);
			if (newVal == WireValues.TRUE || newVal == WireValues.FALSE) data.curValue = newVal;
		}

		state.setPort(n + 1, data.curValue, Memory.DELAY);
		state.setPort(n + 2, data.curValue.not(), Memory.DELAY);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		painter.drawBounds();
		painter.drawLabel();
		if (painter.getShowState()) {
			Location loc = painter.getLocation();
			StateData myState = (StateData) painter.getData();
			if (myState != null) {
				int x = loc.x();
				int y = loc.y();
				g.setColor(myState.curValue.getColor());
				g.fillOval(x - 26, y + 4, 13, 13);
				g.setColor(Color.WHITE);
				GraphicsUtil.drawCenteredText(g, myState.curValue.toBinString(), x - 19, y + 9);
				g.setColor(Color.BLACK);
			}
		}

		int n = getPorts().size() - STD_PORTS;
		g.setColor(Color.GRAY);
		painter.drawPort(n + 3, "0", Direction.South);
		painter.drawPort(n + 4, "1", Direction.South);
		painter.drawPort(n + 5, Strings.get("memEnableLabel"), Direction.South);
		g.setColor(Color.BLACK);
		for (int i = 0; i < n; i++) painter.drawPort(i, getInputName(i), Direction.East);
		painter.drawClock(n, Direction.East);
		painter.drawPort(n + 1, "Q", Direction.West);
		painter.drawPort(n + 2);
	}

	private static class StateData extends ClockState implements InstanceData {
		WireValue curValue = WireValues.FALSE;
	}

	public static class Logger extends InstanceLogger {
		@Override
		public String getLogName(InstanceState state, Object option) {
			String ret = state.getAttributeValue(StdAttr.LABEL);
			return ret != null && !ret.isEmpty() ? ret : null;
		}

		@Override
		public WireValue getLogValue(InstanceState state, Object option) {
			StateData s = (StateData) state.getData();
			return s == null ? WireValues.FALSE : s.curValue;
		}
	}

	public static class Poker extends InstancePoker {
		boolean isPressed = true;

		@Override
		public void mousePressed(InstanceState state, MouseEvent e) {
			isPressed = isInside(state, e);
		}

		@Override
		public void mouseReleased(InstanceState state, MouseEvent e) {
			if (isPressed && isInside(state, e)) {
				StateData myState = (StateData) state.getData();
				if (myState == null) return;

				myState.curValue = myState.curValue.not();
				state.fireInvalidated();
			}
			isPressed = false;
		}

		private boolean isInside(InstanceState state, MouseEvent e) {
			Location loc = state.getInstance().getLocation();
			int dx = e.getX() - (loc.x() - 20);
			int dy = e.getY() - (loc.y() + 10);
			int d2 = dx * dx + dy * dy;
			return d2 < 8 * 8;
		}
	}
}
