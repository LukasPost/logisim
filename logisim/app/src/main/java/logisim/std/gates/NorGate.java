/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Graphics;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.tools.WireRepairData;
import logisim.util.GraphicsUtil;

class NorGate extends AbstractGate {
	public static NorGate FACTORY = new NorGate();

	private NorGate() {
		super("NOR Gate", Strings.getter("norGateComponent"));
		setNegateOutput(true);
		setRectangularLabel(OrGate.FACTORY.getRectangularLabel(null));
		setIconNames("norGate.gif", "norGateRect.gif", "dinNorGate.gif");
		setPaintInputLines(true);
	}

	@Override
	public void paintIconShaped(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.drawCenteredArc(g, 0, -5, 22, -90, 53);
		GraphicsUtil.drawCenteredArc(g, 0, 23, 22, 90, -53);
		GraphicsUtil.drawCenteredArc(g, -12, 9, 16, -30, 60);
		g.drawOval(16, 8, 4, 4);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		PainterShaped.paintOr(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height) {
		PainterDin.paintOr(painter, width, height, true);
	}

	@Override
	protected WireValue computeOutput(WireValue[] inputs, int numInputs, InstanceState state) {
		return GateFunctions.computeOr(inputs, numInputs).not();
	}

	@Override
	protected boolean shouldRepairWire(Instance instance, WireRepairData data) {
		return !data.getPoint().equals(instance.getLocation());
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = Expressions.or(ret, inputs[i]);
		return Expressions.not(ret);
	}

	@Override
	protected WireValue getIdentity() {
		return WireValues.FALSE;
	}
}
