/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import java.awt.Graphics;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.Value;
import logisim.instance.Instance;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;
import logisim.tools.WireRepairData;
import logisim.util.GraphicsUtil;

class OrGate extends AbstractGate {
	public static OrGate FACTORY = new OrGate();

	private OrGate() {
		super("OR Gate", Strings.getter("orGateComponent"));
		setRectangularLabel("\u2265" + "1");
		setIconNames("orGate.gif", "orGateRect.gif", "dinOrGate.gif");
		setPaintInputLines(true);
	}

	@Override
	public void paintIconShaped(InstancePainter painter) {
		Graphics g = painter.getGraphics();
		GraphicsUtil.drawCenteredArc(g, 0, -5, 22, -90, 53);
		GraphicsUtil.drawCenteredArc(g, 0, 23, 22, 90, -53);
		GraphicsUtil.drawCenteredArc(g, -12, 9, 16, -30, 60);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		PainterShaped.paintOr(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height, int inputs) {
		PainterDin.paintOr(painter, width, height, false);
	}

	@Override
	protected Value computeOutput(Value[] inputs, int numInputs, InstanceState state) {
		return GateFunctions.computeOr(inputs, numInputs);
	}

	@Override
	protected boolean shouldRepairWire(Instance instance, WireRepairData data) {
		boolean ret = !data.getPoint().equals(instance.getLocation());
		return ret;
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) {
			ret = Expressions.or(ret, inputs[i]);
		}
		return ret;
	}

	@Override
	protected Value getIdentity() {
		return Value.FALSE;
	}
}
