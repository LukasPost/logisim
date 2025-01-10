/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.plexers;

import java.awt.Color;
import java.awt.Graphics;

import logisim.data.Attribute;
import logisim.data.AttributeSet;
import logisim.data.Attributes;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.data.Location;
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

public class BitSelector extends InstanceFactory {
	public static final Attribute<BitWidth> GROUP_ATTR = Attributes.forBitWidth("group",
			Strings.getter("bitSelectorGroupAttr"));

	public BitSelector() {
		super("BitSelector", Strings.getter("bitSelectorComponent"));
		setAttributes(new Attribute[] { StdAttr.FACING, StdAttr.WIDTH, GROUP_ATTR },
				new Object[] { Direction.East, BitWidth.create(8), BitWidth.ONE });
		setKeyConfigurator(JoinedConfigurator.create(new BitWidthConfigurator(GROUP_ATTR, 1, WireValue.MAX_WIDTH, 0),
				new BitWidthConfigurator(StdAttr.WIDTH)));

		setIconName("bitSelector.gif");
		setFacingAttribute(StdAttr.FACING);
	}

	@Override
	public Bounds getOffsetBounds(AttributeSet attrs) {
		Direction facing = attrs.getValue(StdAttr.FACING);
		Bounds base = Bounds.create(-30, -15, 30, 30);
		return base.rotate(Direction.East, facing, 0, 0);
	}

	@Override
	protected void configureNewInstance(Instance instance) {
		instance.addAttributeListener();
		updatePorts(instance);
	}

	@Override
	protected void instanceAttributeChanged(Instance instance, Attribute<?> attr) {
		if (attr == StdAttr.FACING) {
			instance.recomputeBounds();
			updatePorts(instance);
		} else if (attr == StdAttr.WIDTH || attr == GROUP_ATTR) updatePorts(instance);
	}

	private void updatePorts(Instance instance) {
		Direction facing = instance.getAttributeValue(StdAttr.FACING);
		BitWidth data = instance.getAttributeValue(StdAttr.WIDTH);
		BitWidth group = instance.getAttributeValue(GROUP_ATTR);
		int groups = (data.getWidth() + group.getWidth() - 1) / group.getWidth() - 1;
		int selectBits = 1;
		if (groups > 0) while (groups != 1) {
			groups >>= 1;
			selectBits++;
		}
		BitWidth select = BitWidth.create(selectBits);

		Location inPt;
		Location selPt;
		if (facing == Direction.West) {
			inPt = new Location(30, 0);
			selPt = new Location(10, 10);
		} else if (facing == Direction.North) {
			inPt = new Location(0, 30);
			selPt = new Location(-10, 10);
		} else if (facing == Direction.South) {
			inPt = new Location(0, -30);
			selPt = new Location(-10, -10);
		} else {
			inPt = new Location(-30, 0);
			selPt = new Location(-10, 10);
		}

		Port[] ps = new Port[3];
		ps[0] = new Port(0, 0, Port.OUTPUT, group.getWidth());
		ps[1] = new Port(inPt.x(), inPt.y(), Port.INPUT, data.getWidth());
		ps[2] = new Port(selPt.x(), selPt.y(), Port.INPUT, select.getWidth());
		ps[0].setToolTip(Strings.getter("bitSelectorOutputTip"));
		ps[1].setToolTip(Strings.getter("bitSelectorDataTip"));
		ps[2].setToolTip(Strings.getter("bitSelectorSelectTip"));
		instance.setPorts(ps);
	}

	@Override
	public void propagate(InstanceState state) {
		WireValue data = state.getPort(1);
		WireValue select = state.getPort(2);
		BitWidth groupBits = state.getAttributeValue(GROUP_ATTR);
		WireValue group;
		if (!select.isFullyDefined()) group = WireValue.Companion.createUnknown(groupBits);
		else {
			int shift = select.toIntValue() * groupBits.getWidth();
			if (shift >= data.getWidth()) group = WireValue.Companion.createKnown(groupBits, 0);
			else if (groupBits.getWidth() == 1) group = data.get(shift);
			else {
				WireValue[] bits = new WireValue[groupBits.getWidth()];
				for (int i = 0; i < bits.length; i++)
					if (shift + i >= data.getWidth()) bits[i] = WireValues.FALSE;
					else bits[i] = data.get(shift + i);
				group = WireValue.Companion.create(bits);
			}
		}
		state.setPort(0, group, Plexers.DELAY);
	}

	@Override
	public void paintGhost(InstancePainter painter) {
		Plexers.drawTrapezoid(painter.getGraphics(), painter.getBounds(), painter.getAttributeValue(StdAttr.FACING), 9);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		Direction facing = painter.getAttributeValue(StdAttr.FACING);

		Plexers.drawTrapezoid(g, painter.getBounds(), facing, 9);
		Bounds bds = painter.getBounds();
		g.setColor(Color.BLACK);
		GraphicsUtil.drawCenteredText(g, "Sel", bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		painter.drawPorts();
	}
}
