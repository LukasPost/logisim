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
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class OddParityGate : AbstractGate
	{
		public static OddParityGate FACTORY = new OddParityGate();

		private OddParityGate() : base("Odd Parity", Strings.getter("oddParityComponent"))
		{
			RectangularLabel = "2k+1";
			IconNames = "parityOddGate.gif";
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			paintIconRectangular(painter);
		}

		public override void paintIconRectangular(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			g.setColor(Color.black);
			g.drawRect(1, 2, 16, 16);
			Font old = g.getFont();
			g.setFont(old.deriveFont(9.0f));
			GraphicsUtil.drawCenteredText(g, "2k", 9, 6);
			GraphicsUtil.drawCenteredText(g, "+1", 9, 13);
			g.setFont(old);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			paintRectangular(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			paintRectangular(painter, width, height);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			return GateFunctions.computeOddParity(inputs, numInputs);
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.xor(ret, inputs[i]);
			}
			return ret;
		}

		protected internal override Value Identity
		{
			get
			{
				return Value.FALSE;
			}
		}

	}

}
