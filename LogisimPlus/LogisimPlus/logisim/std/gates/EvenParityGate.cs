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

	internal class EvenParityGate : AbstractGate
	{
		public static EvenParityGate FACTORY = new EvenParityGate();

		private EvenParityGate() : base("Even Parity", Strings.getter("evenParityComponent"))
		{
			RectangularLabel = "2k";
			IconNames = "parityEvenGate.gif";
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			paintIconRectangular(painter);
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
			return GateFunctions.computeOddParity(inputs, numInputs).not();
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.xor(ret, inputs[i]);
			}
			return Expressions.not(ret);
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
