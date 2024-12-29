/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Graphics;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.Value;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.util.GraphicsUtil;

class AndGate extends AbstractGate {
	public static AndGate FACTORY = new AndGate();

	private AndGate() {
		super("AND Gate", Strings.getter("andGateComponent"));
		setRectangularLabel("&");
		setIconNames("andGate.gif", "andGateRect.gif", "dinAndGate.gif");
	}

	@Override
	protected void paintIconShaped(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		int[] xp = new int[] { 10, 2, 2, 10 };
		int[] yp = new int[] { 2, 2, 18, 18 };
		g.drawPolyline(xp, yp, 4);
		GraphicsUtil.drawCenteredArc(g, 10, 10, 8, -90, 180);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		PainterShaped.paintAnd(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height, int inputs) {
		PainterDin.paintAnd(painter, width, height, false);
	}

	@Override
	protected Value computeOutput(Value[] inputs, int numInputs, InstanceState state) {
		return GateFunctions.computeAnd(inputs, numInputs);
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) {
			ret = Expressions.and(ret, inputs[i]);
		}
		return ret;
	}

	@Override
	protected Value getIdentity() {
		return Value.TRUE;
	}
}
