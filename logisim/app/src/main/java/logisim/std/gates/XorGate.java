/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Graphics;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.AttributeSet;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.tools.WireRepairData;
import logisim.util.GraphicsUtil;

class XorGate extends AbstractGate {
	public static XorGate FACTORY = new XorGate();

	private XorGate() {
		super("XOR Gate", Strings.getter("xorGateComponent"), true);
		setAdditionalWidth(10);
		setIconNames("xorGate.gif", "xorGateRect.gif", "dinXorGate.gif");
		setPaintInputLines(true);
	}

	@Override
	public String getRectangularLabel(AttributeSet attrs) {
		if (attrs == null)
			return "";
		boolean isOdd = false;
		Object behavior = attrs.getValue(GateAttributes.ATTR_XOR);
		if (behavior == GateAttributes.XOR_ODD) {
			Integer inputs = attrs.getValue(GateAttributes.ATTR_INPUTS);
			if (inputs == null || inputs != 2) isOdd = true;
		}
		return isOdd ? "2k+1" : "=1";
	}

	@Override
	public void paintIconShaped(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.drawCenteredArc(g, 2, -5, 22, -90, 53);
		GraphicsUtil.drawCenteredArc(g, 2, 23, 22, 90, -53);
		GraphicsUtil.drawCenteredArc(g, -10, 9, 16, -30, 60);
		GraphicsUtil.drawCenteredArc(g, -12, 9, 16, -30, 60);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		PainterShaped.paintXor(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height) {
		PainterDin.paintXor(painter, width, height, false);
	}

	@Override
	protected WireValue computeOutput(WireValue[] inputs, int numInputs, InstanceState state) {
		Object behavior = state.getAttributeValue(GateAttributes.ATTR_XOR);
		if (behavior == GateAttributes.XOR_ODD) return GateFunctions.computeOddParity(inputs, numInputs);
		else return GateFunctions.computeExactlyOne(inputs, numInputs);
	}

	@Override
	protected boolean shouldRepairWire(Instance instance, WireRepairData data) {
		return !data.getPoint().equals(instance.getLocation());
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		return xorExpression(inputs, numInputs);
	}

	@Override
	protected WireValue getIdentity() {
		return WireValues.FALSE;
	}

	protected static Expression xorExpression(Expression[] inputs, int numInputs) {
		if (numInputs > 2) throw new UnsupportedOperationException("XorGate");
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = Expressions.xor(ret, inputs[i]);
		return ret;
	}
}
