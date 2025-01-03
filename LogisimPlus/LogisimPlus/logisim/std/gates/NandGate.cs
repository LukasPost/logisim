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

    internal class NandGate : AbstractGate
	{
		public static NandGate FACTORY = new NandGate();

		private NandGate() : base("NAND Gate", Strings.getter("nandGateComponent"))
		{
			NegateOutput = true;
			RectangularLabel = AndGate.FACTORY.getRectangularLabel(null);
			setIconNames("nandGate.gif", "nandGateRect.gif", "dinNandGate.gif");
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			int[] xp = new int[] {8, 0, 0, 8};
			int[] yp = new int[] {2, 2, 18, 18};
			g.drawPolyline(xp, yp, 4);
			JGraphicsUtil.drawCenteredArc(g, 8, 10, 8, -90, 180);
			g.drawOval(16, 8, 4, 4);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			PainterShaped.paintAnd(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			PainterDin.paintAnd(painter, width, height, true);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			return GateFunctions.computeAnd(inputs, numInputs).not();
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.and(ret, inputs[i]);
			}
			return Expressions.not(ret);
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
