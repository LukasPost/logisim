/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Graphics;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.util.GraphicsUtil;

class NandGate extends AbstractGate {
	public static NandGate FACTORY = new NandGate();

	private NandGate() {
		super("NAND Gate", Strings.getter("nandGateComponent"));
		setNegateOutput(true);
		setRectangularLabel(AndGate.FACTORY.getRectangularLabel(null));
		setIconNames("nandGate.gif", "nandGateRect.gif", "dinNandGate.gif");
	}

	@Override
	public void paintIconShaped(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		int[] xp = { 8, 0, 0, 8 };
		int[] yp = { 2, 2, 18, 18 };
		g.drawPolyline(xp, yp, 4);
		GraphicsUtil.drawCenteredArc(g, 8, 10, 8, -90, 180);
		g.drawOval(16, 8, 4, 4);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		PainterShaped.paintAnd(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height) {
		PainterDin.paintAnd(painter, width, height, true);
	}

	@Override
	protected WireValue computeOutput(WireValue[] inputs, int numInputs, InstanceState state) {
		return GateFunctions.computeAnd(inputs, numInputs).not();
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = Expressions.and(ret, inputs[i]);
		return Expressions.not(ret);
	}

	@Override
	protected WireValue getIdentity() {
		return WireValues.TRUE;
	}
}
