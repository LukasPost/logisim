/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package gray;

import logisim.data.BitWidth;
import logisim.data.Bounds;
import logisim.data.Direction;
import logisim.instance.InstanceFactory;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.instance.Port;
import logisim.util.GraphicsUtil;
import logisim.util.StringUtil;

/**
 * Manufactures a simple counter that iterates over the 4-bit Gray Code. This example illustrates how a component can
 * maintain its own internal state. All of the code relevant to state, though, appears in CounterData class.
 */
class SimpleGrayCounter extends InstanceFactory {
	private static final BitWidth BIT_WIDTH = BitWidth.create(4);

	// Again, notice how we don't have any instance variables related to an
	// individual instance's state. We can't put that here, because only one
	// SimpleGrayCounter object is ever created, and its job is to manage all
	// instances that appear in any circuits.

	public SimpleGrayCounter() {
		super("Gray Counter (Simple)");
		setOffsetBounds(Bounds.create(-30, -15, 30, 30));
		setPorts(new Port[] { new Port(-30, 0, Port.INPUT, 1), new Port(0, 0, Port.OUTPUT, BIT_WIDTH.getWidth()), });
	}

	@Override
	public void propagate(InstanceState state) {
		// Here I retrieve the state associated with this component via a helper
		// method. In this case, the state is in a CounterData object, which is
		// also where the helper method is defined. This helper method will end
		// up creating a CounterData object if one doesn't already exist.
		CounterData cur = CounterData.get(state, BIT_WIDTH);

		boolean trigger = cur.updateClock(state.getPort(0));
		if (trigger)
			cur.setValue(GrayIncrementer.nextGray(cur.getValue()));
		state.setPort(1, cur.getValue(), 9);

		// (You might be tempted to determine the counter's current value
		// via state.getPort(1). This is erroneous, though, because another
		// component may be pushing a value onto the same point, which would
		// "corrupt" the value found there. We really do need to store the
		// current value in the instance.)
	}

	@Override
	public void paintInstance(InstancePainter painter) {
		painter.drawBounds();
		painter.drawClock(0, Direction.East); // draw a triangle on port 0
		painter.drawPort(1); // draw port 1 as just a dot

		// Display the current counter value centered within the rectangle.
		// However, if the context says not to show state (as when generating
		// printer output), then skip this.
		if (painter.getShowState()) {
			CounterData state = CounterData.get(painter, BIT_WIDTH);
			Bounds bds = painter.getBounds();
			GraphicsUtil.drawCenteredText(painter.getGraphics(),
					StringUtil.toHexString(BIT_WIDTH.getWidth(), state.getValue().toIntValue()),
					bds.getX() + bds.getWidth() / 2, bds.getY() + bds.getHeight() / 2);
		}
	}
}
