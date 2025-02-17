/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.wiring;

import java.awt.FontMetrics;
import java.awt.Graphics;

import logisim.data.Attribute;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.tools.key.BitWidthConfigurator;
import logisim.tools.key.JoinedConfigurator;
import logisim.util.GraphicsUtil;

public class BitExtender extends InstanceFactory {
	private static final Attribute<BitWidth> ATTR_IN_WIDTH = Attributes.forBitWidth("in_width",
			Strings.getter("extenderInAttr"));
	private static final Attribute<BitWidth> ATTR_OUT_WIDTH = Attributes.forBitWidth("out_width",
			Strings.getter("extenderOutAttr"));
	private static final Attribute<AttributeOption> ATTR_TYPE = Attributes.forOption("type",
			Strings.getter("extenderTypeAttr"),
			new AttributeOption[] { new AttributeOption("zero", "zero", Strings.getter("extenderZeroType")),
					new AttributeOption("one", "one", Strings.getter("extenderOneType")),
					new AttributeOption("sign", "sign", Strings.getter("extenderSignType")),
					new AttributeOption("input", "input", Strings.getter("extenderInputType")), });

	public static final BitExtender FACTORY = new BitExtender();

	public BitExtender() {
		super("Bit Extender", Strings.getter("extenderComponent"));
		setIconName("extender.gif");
		setAttributes(new Attribute[] { ATTR_IN_WIDTH, ATTR_OUT_WIDTH, ATTR_TYPE },
				new Object[] { BitWidth.create(8), BitWidth.create(16), ATTR_TYPE.parse("zero") });
		setFacingAttribute(StdAttr.FACING);
		setKeyConfigurator(JoinedConfigurator.create(new BitWidthConfigurator(ATTR_OUT_WIDTH),
				new BitWidthConfigurator(ATTR_IN_WIDTH, 1, WireValue.MAX_WIDTH, 0)));
		setOffsetBounds(Bounds.create(-40, -20, 40, 40));
	}

	//
	// graphics methods
	//
	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		FontMetrics fm = g.getFontMetrics();
		int asc = fm.getAscent();

		painter.drawBounds();

		String type = getType(painter.getAttributeSet());
		String s0 = switch (type) {
			case "zero" -> Strings.get("extenderZeroLabel");
			case "one" -> Strings.get("extenderOneLabel");
			case "sign" -> Strings.get("extenderSignLabel");
			case "input" -> Strings.get("extenderInputLabel");
			case null, default -> "???"; // should never happen
		};
		String s1 = Strings.get("extenderMainLabel");
		Bounds bds = painter.getBounds();
		int x = bds.getX() + bds.getWidth() / 2;
		int y0 = bds.getY() + (bds.getHeight() / 2 + asc) / 2;
		int y1 = bds.getY() + (3 * bds.getHeight() / 2 + asc) / 2;
		GraphicsUtil.drawText(g, s0, x, y0, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
		GraphicsUtil.drawText(g, s1, x, y1, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);

		BitWidth w0 = painter.getAttributeValue(ATTR_OUT_WIDTH);
		BitWidth w1 = painter.getAttributeValue(ATTR_IN_WIDTH);
		painter.drawPort(0, "" + w0.getWidth(), Direction.West);
		painter.drawPort(1, "" + w1.getWidth(), Direction.East);
		if ("input".equals(type))
			painter.drawPort(2);
	}

	//
	// methods for instances
	//
	@Override
	protected void configureNewInstance(Instance instance) {
		configurePorts(instance);
		instance.addAttributeListener();
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == ATTR_TYPE) {
			configurePorts(instance);
			instance.fireInvalidated();
		} else instance.fireInvalidated();
	}

	private void configurePorts(Instance instance) {
		Port p0 = new Port(0, 0, Port.OUTPUT, ATTR_OUT_WIDTH);
		Port p1 = new Port(-40, 0, Port.INPUT, ATTR_IN_WIDTH);
		String type = getType(instance.getAttributeSet());
		if ("input".equals(type)) instance.setPorts(new Port[]{p0, p1, new Port(-20, -20, Port.INPUT, 1)});
		else instance.setPorts(new Port[]{p0, p1});
	}

	@Override
	public void propagate(InstanceState state) {
		WireValue in = state.getPort(1);
		BitWidth wout = state.getAttributeValue(ATTR_OUT_WIDTH);
		String type = getType(state.getAttributeSet());
		WireValue extend;
		switch (type) {
			case "one" -> extend = WireValues.TRUE;
			case "sign" -> {
				int win = in.getWidth();
				extend = win > 0 ? in.get(win - 1) : WireValues.ERROR;
			}
			case "input" -> {
				extend = state.getPort(2);
				if (extend.getWidth() != 1)
					extend = WireValues.ERROR;
			}
			case null, default -> extend = WireValues.FALSE;
		}

		WireValue out = in.extendWidth(wout.getWidth(), extend);
		state.setPort(0, out, 1);
	}

	private String getType(AttributeSet attrs) {
		AttributeOption topt = attrs.getValue(ATTR_TYPE);
		return (String) topt.getValue();
	}
}