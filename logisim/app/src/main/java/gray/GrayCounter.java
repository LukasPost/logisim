/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package gray;

import java.net.URL;

import javax.swing.ImageIcon;

import logisim.data.Attribute;
import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.instance.Instance;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.instance.StdAttr;
import logisim.util.GraphicsUtil;
import logisim.util.StringUtil;

/**
 * Manufactures a counter that iterates over Gray codes. This demonstrates several additional features beyond the
 * SimpleGrayCounter class.
 */
class GrayCounter extends InstanceFactory {
	public GrayCounter() {
		super("Gray Counter");
		setOffsetBounds(Bounds.create(-30, -15, 30, 30));
		setPorts(new Port[] { new Port(-30, 0, Port.INPUT, 1), new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH), });

		// We'll have width, label, and label font attributes. The latter two
		// attributes allow us to associate a label with the component (though
		// we'll also need configureNewInstance to configure the label's
		// location).
		setAttributes(new Attribute[] { StdAttr.WIDTH, StdAttr.LABEL, StdAttr.LABEL_FONT },
				new Object[] { BitWidth.create(4), "", StdAttr.DEFAULT_LABEL_FONT });

		// The following method invocation sets things up so that the instance's
		// state can be manipulated using the Poke Tool.
		setInstancePoker(CounterPoker.class);

		// These next two lines set it up so that the explorer window shows a
		// customized icon representing the component type. This should be a
		// 16x16 image.
		URL url = getClass().getClassLoader().getResource("com/cburch/gray/counter.gif");
		if (url != null)
			setIcon(new ImageIcon(url));
	}

	/**
	 * The configureNewInstance method is invoked every time a new instance is created. In the superclass, the method
	 * doesn't do anything, since the new instance is pretty thoroughly configured already by default. But sometimes you
	 * need to do something particular to each instance, so you would override the method. In this case, we need to set
	 * up the location for its label.
	 */
	@Override
	protected void configureNewInstance(Instance instance) {
		Bounds bds = instance.getBounds();
		instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.getX() + bds.getWidth() / 2, bds.getY() - 3,
				GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
	}

	@Override
	public void propagate(InstanceState state) {
		// This is the same as with SimpleGrayCounter, except that we use the
		// StdAttr.WIDTH attribute to determine the bit width to work with.
		BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
		CounterData cur = CounterData.get(state, width);
		boolean trigger = cur.updateClock(state.getPort(0));
		if (trigger)
			cur.setValue(GrayIncrementer.nextGray(cur.getValue()));
		state.setPort(1, cur.getValue(), 9);
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		// This is essentially the same as with SimpleGrayCounter, except for
		// the invocation of painter.drawLabel to make the label be drawn.
		painter.drawBounds();
		painter.drawClock(0, Direction.East);
		painter.drawPort(1);
		painter.drawLabel();

		if (painter.getShowState()) {
			BitWidth width = painter.getAttributeValue(StdAttr.WIDTH);
			CounterData state = CounterData.get(painter, width);
			Bounds bds = painter.getBounds();
			GraphicsUtil.drawCenteredText(painter.getGraphics(),
					StringUtil.toHexString(width.getWidth(), state.getValue().toIntValue()),
					bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		}
	}
}
