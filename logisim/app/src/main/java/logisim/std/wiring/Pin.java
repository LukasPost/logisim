/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.Color;
import java.awt.Component;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.util.Arrays;

import javax.swing.Icon;
import javax.swing.JOptionPane;
import javax.swing.SwingUtilities;

import logisim.circuit.CircuitState;
import logisim.circuit.RadixOption;
import logisim.comp.EndData;
import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.gui.main.Canvas;
import logisim.instance.Instance;
import logisim.instance.InstanceData;
import logisim.instance.InstanceFactory;
import logisim.instance.InstanceLogger;
import logisim.instance.InstancePainter;
import logisim.instance.InstancePoker;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;
import logisim.tools.key.DirectionConfigurator;
import logisim.tools.key.JoinedConfigurator;
import logisim.util.GraphicsUtil;
import logisim.util.Icons;

public class Pin extends InstanceFactory {
	public static final Attribute<Boolean> ATTR_TRISTATE = Attributes.forBoolean("tristate",
			Strings.getter("pinThreeStateAttr"));
	public static final Attribute<Boolean> ATTR_TYPE = Attributes.forBoolean("output", Strings.getter("pinOutputAttr"));
	public static final Attribute<Direction> ATTR_LABEL_LOC = Attributes.forDirection("labelloc",
			Strings.getter("pinLabelLocAttr"));

	public static final AttributeOption PULL_NONE = new AttributeOption("none", Strings.getter("pinPullNoneOption"));
	public static final AttributeOption PULL_UP = new AttributeOption("up", Strings.getter("pinPullUpOption"));
	public static final AttributeOption PULL_DOWN = new AttributeOption("down", Strings.getter("pinPullDownOption"));
	public static final Attribute<AttributeOption> ATTR_PULL = Attributes.forOption("pull",
			Strings.getter("pinPullAttr"), new AttributeOption[] { PULL_NONE, PULL_UP, PULL_DOWN });

	public static final Pin FACTORY = new Pin();

	private static final Icon ICON_IN = Icons.getIcon("pinInput.gif");
	private static final Icon ICON_OUT = Icons.getIcon("pinOutput.gif");
	private static final Font ICON_WIDTH_FONT = new Font("SansSerif", Font.BOLD, 9);
	private static final Color ICON_WIDTH_COLOR = WireValue.Companion.getWIDTH_ERROR_COLOR().darker();

	public Pin() {
		super("Pin", Strings.getter("pinComponent"));
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(JoinedConfigurator.create(new BitWidthConfigurator(StdAttr.WIDTH),
				new DirectionConfigurator(ATTR_LABEL_LOC, KeyEvent.ALT_DOWN_MASK)));
		setInstanceLogger(PinLogger.class);
		setInstancePoker(PinPoker.class);
	}

	@Override
	public AttributeSet createAttributeSet() {
		return new PinAttributes();
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		BitWidth width = attrs.getValue(StdAttr.WIDTH);
		return Probe.getOffsetBounds(facing, width, RadixOption.RADIX_2);
	}

	//
	// graphics methods
	//
	@Override
	public void paintIcon(InstancePainter painter) {
		paintIconBase(painter);
		BitWidth w = painter.getAttributeValue(StdAttr.WIDTH);
		if (!w.equals(BitWidth.ONE)) {
			Graphics g = painter.getGraphics();
			g.setColor(ICON_WIDTH_COLOR);
			g.setFont(ICON_WIDTH_FONT);
			GraphicsUtil.drawCenteredText(g, "" + w.getWidth(), 10, 9);
			g.setColor(Color.BLACK);
		}
	}

	private void paintIconBase(InstancePainter painter) {
		PinAttributes attrs = (PinAttributes) painter.getAttributeSet();
		Direction dir = attrs.facing;
		boolean output = attrs.isOutput();
		Graphics g = painter.getGraphics();
		if (output) {
			if (ICON_OUT != null) {
				Icons.paintRotated(g, 2, 2, dir, ICON_OUT, painter.getDestination());
				return;
			}
		} else if (ICON_IN != null) {
			Icons.paintRotated(g, 2, 2, dir, ICON_IN, painter.getDestination());
			return;
		}
		int pinx = 16;
		int piny = 9;
		if (dir == Direction.East) { // keep defaults
		} else if (dir == Direction.West) pinx = 4;
		else if (dir == Direction.North) {
			pinx = 9;
			piny = 4;
		} else if (dir == Direction.South) {
			pinx = 9;
			piny = 16;
		}

		g.setColor(Color.black);
		if (output) g.drawOval(4, 4, 13, 13);
		else g.drawRect(4, 4, 13, 13);
		g.setColor(WireValues.TRUE.getColor());
		g.fillOval(7, 7, 8, 8);
		g.fillOval(pinx, piny, 3, 3);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		PinAttributes attrs = (PinAttributes) painter.getAttributeSet();
		Location loc = painter.getLocation();
		Bounds bds = painter.getOffsetBounds();
		int x = loc.x();
		int y = loc.y();
		Graphics g = painter.getGraphics();
		GraphicsUtil.switchToWidth(g, 2);
		boolean output = attrs.isOutput();
		if (output) {
			BitWidth width = attrs.getValue(StdAttr.WIDTH);
			if (width == BitWidth.ONE)
				g.drawOval(x + bds.getX() + 1, y + bds.getY() + 1, bds.getWidth() - 1, bds.getHeight() - 1);
			else
				g.drawRoundRect(x + bds.getX() + 1, y + bds.getY() + 1, bds.getWidth() - 1, bds.getHeight() - 1, 6, 6);
		} else g.drawRect(x + bds.getX() + 1, y + bds.getY() + 1, bds.getWidth() - 1, bds.getHeight() - 1);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		PinAttributes attrs = (PinAttributes) painter.getAttributeSet();
		Graphics g = painter.getGraphics();
		Bounds bds = painter.getInstance().getBounds(); // intentionally with no graphics object - we don't want label
														// included
		int x = bds.getX();
		int y = bds.getY();
		GraphicsUtil.switchToWidth(g, 2);
		g.setColor(Color.black);
		if (attrs.type == EndData.OUTPUT_ONLY)
			if (attrs.width.getWidth() == 1) g.drawOval(x + 1, y + 1, bds.getWidth() - 1, bds.getHeight() - 1);
			else g.drawRoundRect(x + 1, y + 1, bds.getWidth() - 1, bds.getHeight() - 1, 6, 6);
		else g.drawRect(x + 1, y + 1, bds.getWidth() - 1, bds.getHeight() - 1);

		painter.drawLabel();

		if (!painter.getShowState()) {
			g.setColor(Color.BLACK);
			GraphicsUtil.drawCenteredText(g, "x" + attrs.width.getWidth(), bds.getX() + bds.getWidth() / 2,
					bds.getY() + bds.getHeight() / 2);
		} else {
			PinState state = getState(painter);
			if (attrs.width.getWidth() <= 1) {
				WireValue receiving = state.receiving;
				g.setColor(receiving.getColor());
				g.fillOval(x + 4, y + 4, 13, 13);

				if (attrs.width.getWidth() == 1) {
					g.setColor(Color.WHITE);
					GraphicsUtil.drawCenteredText(g, state.sending.toBinString(), x + 11, y + 9);
				}
			} else Probe.paintValue(painter, state.sending);
		}

		painter.drawPorts();
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
		instance.addAttributeListener();
		configurePorts(instance);
		Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == ATTR_TYPE) configurePorts(instance);
		else if (attr == StdAttr.WIDTH || attr == StdAttr.FACING || attr == Pin.ATTR_LABEL_LOC) {
			instance.recomputeBounds();
			PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
			Probe.configureLabel(instance, attrs.labelloc, attrs.facing);
		}
	}

	private void configurePorts(Instance instance) {
		PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
		String endType = attrs.isOutput() ? Port.INPUT : Port.OUTPUT;
		Port port = new Port(0, 0, endType, StdAttr.WIDTH);
		if (attrs.isOutput()) port.setToolTip(Strings.getter("pinOutputToolTip"));
		else port.setToolTip(Strings.getter("pinInputToolTip"));
		instance.setPorts(new Port[] { port });
	}

	@Override
	public void propagate(InstanceState state) {
		PinAttributes attrs = (PinAttributes) state.getAttributeSet();
		WireValue val = state.getPort(0);

		PinState q = getState(state);
		if (attrs.type == EndData.OUTPUT_ONLY) {
			q.sending = val;
			q.receiving = val;
			state.setPort(0, WireValue.Companion.createUnknown(attrs.width), 1);
		} else if (!val.isFullyDefined() && !attrs.threeState && state.isCircuitRoot()) {
			q.sending = pull2(q.sending, attrs.width);
			q.receiving = pull2(val, attrs.width);
			state.setPort(0, q.sending, 1);
		}
		else {
			q.receiving = val;
			// ignore if no change
			if (!val.equals(q.sending)) state.setPort(0, q.sending, 1);
		}
	}

	private static WireValue pull2(WireValue mod, BitWidth expectedWidth) {
		if (mod.getWidth() == expectedWidth.getWidth()) {
			WireValue[] vs = mod.getAll();
			for (int i = 0; i < vs.length; i++)
				if (vs[i] == WireValues.UNKNOWN)
					vs[i] = WireValues.FALSE;
			return WireValue.Companion.create(vs);
		} else return WireValue.Companion.createKnown(expectedWidth, 0);
	}

	//
	// basic information methods
	//
	public BitWidth getWidth(Instance instance) {
		PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
		return attrs.width;
	}

	public int getType(Instance instance) {
		PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
		return attrs.type;
	}

	public boolean isInputPin(Instance instance) {
		PinAttributes attrs = (PinAttributes) instance.getAttributeSet();
		return attrs.type != EndData.OUTPUT_ONLY;
	}

	//
	// state information methods
	//
	public WireValue getValue(InstanceState state) {
		return getState(state).sending;
	}

	public void setValue(InstanceState state, WireValue value) {
		PinAttributes attrs = (PinAttributes) state.getAttributeSet();
		Object pull = attrs.pull;
		if (pull != PULL_NONE && pull != null && !value.isFullyDefined()) {
			WireValue[] bits = value.getAll();
			if (pull == PULL_UP) for (int i = 0; i < bits.length; i++) {
				if (bits[i] != WireValues.FALSE)
					bits[i] = WireValues.TRUE;
			}
			else if (pull == PULL_DOWN)
				for (int i = 0; i < bits.length; i++)
					if (bits[i] != WireValues.TRUE)
						bits[i] = WireValues.FALSE;
			value = WireValue.Companion.create(bits);
		}

		PinState myState = getState(state);
		if (value == WireValues.NIL) myState.sending = WireValue.Companion.createUnknown(attrs.width);
		else myState.sending = value;
	}

	private static PinState getState(InstanceState state) {
		PinAttributes attrs = (PinAttributes) state.getAttributeSet();
		BitWidth width = attrs.width;
		PinState ret = (PinState) state.getData();
		if (ret == null) {
			WireValue val = attrs.threeState ? WireValues.UNKNOWN : WireValues.FALSE;
			if (width.getWidth() > 1) {
				WireValue[] arr = new WireValue[width.getWidth()];
				Arrays.fill(arr, val);
				val = WireValue.Companion.create(arr);
			}
			ret = new PinState(val, val);
			state.setData(ret);
		}
		if (ret.sending.getWidth() != width.getWidth())
			ret.sending = ret.sending.extendWidth(width.getWidth(), attrs.threeState ? WireValues.UNKNOWN : WireValues.FALSE);
		if (ret.receiving.getWidth() != width.getWidth())
			ret.receiving = ret.receiving.extendWidth(width.getWidth(), WireValues.UNKNOWN);
		return ret;
	}

	private static class PinState implements InstanceData, Cloneable {
		WireValue sending;
		WireValue receiving;

		public PinState(WireValue sending, WireValue receiving) {
			this.sending = sending;
			this.receiving = receiving;
		}

		@Override
		public Object clone() {
			try {
				return super.clone();
			}
			catch (CloneNotSupportedException e) {
				return null;
			}
		}
	}

	public static class PinPoker extends InstancePoker {
		int bitPressed = -1;

		@Override
		public void mousePressed(InstanceState state, MouseEvent e) {
			bitPressed = getBit(state, e);
		}

		@Override
		public void mouseReleased(InstanceState state, MouseEvent e) {
			int bit = getBit(state, e);
			if (bit == bitPressed && bit >= 0) handleBitPress(state, bit, e);
			bitPressed = -1;
		}

		private void handleBitPress(InstanceState state, int bit, MouseEvent e) {
			PinAttributes attrs = (PinAttributes) state.getAttributeSet();
			if (!attrs.isInput())
				return;

			Component sourceComp = e.getComponent();
			if (sourceComp instanceof Canvas && !state.isCircuitRoot()) {
				Canvas canvas = (Canvas) e.getComponent();
				CircuitState circState = canvas.getCircuitState();
				Component frame = SwingUtilities.getRoot(canvas);
				int choice = JOptionPane.showConfirmDialog(frame, Strings.get("pinFrozenQuestion"),
						Strings.get("pinFrozenTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.WARNING_MESSAGE);
				if (choice == JOptionPane.OK_OPTION) {
					circState = circState.cloneState();
					canvas.getProject().setCircuitState(circState);
					state = circState.getInstanceState(state.getInstance());
				} else return;
			}

			PinState pinState = getState(state);
			WireValue val = pinState.sending.get(bit);
			if (val == WireValues.FALSE) val = WireValues.TRUE;
			else if (val == WireValues.TRUE) val = attrs.threeState ? WireValues.UNKNOWN : WireValues.FALSE;
			else val = WireValues.FALSE;
			pinState.sending = pinState.sending.set(bit, val);
			state.fireInvalidated();
		}

		private int getBit(InstanceState state, MouseEvent e) {
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			if (width.getWidth() == 1) return 0;
			else {
				Bounds bds = state.getInstance().getBounds(); // intentionally with no graphics object - we don't want
																// label included
				int i = (bds.getX() + bds.getWidth() - e.getX()) / 10;
				int j = (bds.getY() + bds.getHeight() - e.getY()) / 20;
				int bit = 8 * j + i;
				if (bit < 0 || bit >= width.getWidth()) return -1;
				else return bit;
			}
		}
	}

	public static class PinLogger extends InstanceLogger {
		@Override
		public String getLogName(InstanceState state, Object option) {
			PinAttributes attrs = (PinAttributes) state.getAttributeSet();
			String ret = attrs.label;
			if (ret == null || ret.isEmpty()) {
				String type = attrs.type == EndData.INPUT_ONLY ? Strings.get("pinInputName")
						: Strings.get("pinOutputName");
				return type + state.getInstance().getLocation();
			} else return ret;
		}

		@Override
		public WireValue getLogValue(InstanceState state, Object option) {
			PinState s = getState(state);
			return s.sending;
		}
	}
}