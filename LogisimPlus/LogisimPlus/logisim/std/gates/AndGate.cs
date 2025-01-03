// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using Expression = logisim.analyze.model.Expression;
	using Expressions = logisim.analyze.model.Expressions;
	using Value = logisim.data.Value;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
    using LogisimPlus.Java;

    internal class AndGate : AbstractGate
	{
		public static AndGate FACTORY = new AndGate();

		private AndGate() : base("AND Gate", Strings.getter("andGateComponent"))
		{
			RectangularLabel = "&";
			setIconNames("andGate.gif", "andGateRect.gif", "dinAndGate.gif");
		}

		protected internal override void paintIconShaped(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			int[] xp = new int[] {10, 2, 2, 10};
			int[] yp = new int[] {2, 2, 18, 18};
			g.drawPolyline(xp, yp, 4);
			JGraphicsUtil.drawCenteredArc(g, 10, 10, 8, -90, 180);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			PainterShaped.paintAnd(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			PainterDin.paintAnd(painter, width, height, false);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			return GateFunctions.computeAnd(inputs, numInputs);
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.and(ret, inputs[i]);
			}
			return ret;
		}

		protected internal override Value Identity
		{
			get
			{
				return Value.TRUE;
			}
		}
	}

}
