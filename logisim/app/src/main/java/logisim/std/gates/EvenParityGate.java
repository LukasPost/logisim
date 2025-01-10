/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.std.gates;

import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.InstancePainter;
import logisim.instance.InstanceState;

class EvenParityGate extends AbstractGate {
	public static EvenParityGate FACTORY = new EvenParityGate();

	private EvenParityGate() {
		super("Even Parity", Strings.getter("evenParityComponent"));
		setRectangularLabel("2k");
		setIconNames("parityEvenGate.gif");
	}

	@Override
	public void paintIconShaped(InstancePainter painter) {
		paintIconRectangular(painter);
	}

	@Override
	protected void paintShape(InstancePainter painter, int width, int height) {
		paintRectangular(painter, width, height);
	}

	@Override
	protected void paintDinShape(InstancePainter painter, int width, int height) {
		paintRectangular(painter, width, height);
	}

	@Override
	protected WireValue computeOutput(WireValue[] inputs, int numInputs, InstanceState state) {
		return GateFunctions.computeOddParity(inputs, numInputs).not();
	}

	@Override
	protected Expression computeExpression(Expression[] inputs, int numInputs) {
		Expression ret = inputs[0];
		for (int i = 1; i < numInputs; i++) ret = Expressions.xor(ret, inputs[i]);
		return Expressions.not(ret);
	}

	@Override
	protected WireValue getIdentity() {
		return WireValues.FALSE;
	}
}
