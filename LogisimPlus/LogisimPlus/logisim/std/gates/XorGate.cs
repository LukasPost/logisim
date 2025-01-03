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
	using AttributeSet = logisim.data.AttributeSet;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using WireRepairData = logisim.tools.WireRepairData;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;

	internal class XorGate : AbstractGate
	{
		public static XorGate FACTORY = new XorGate();

		private XorGate() : base("XOR Gate", Strings.getter("xorGateComponent"), true)
		{
			AdditionalWidth = 10;
			setIconNames("xorGate.gif", "xorGateRect.gif", "dinXorGate.gif");
			PaintInputLines = true;
		}

		protected override string getRectangularLabel(AttributeSet attrs)
		{
			if (attrs == null)
			{
				return "";
			}
			bool isOdd = false;
			object behavior = attrs.getValue(GateAttributes.ATTR_XOR);
			if (behavior == GateAttributes.XOR_ODD)
			{
				object inputs = attrs.getValue(GateAttributes.ATTR_INPUTS);
				if (inputs == null || ((int?) inputs).Value != 2)
				{
					isOdd = true;
				}
			}
			return isOdd ? "2k+1" : "=1";
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.drawCenteredArc(g, 2, -5, 22, -90, 53);
			JGraphicsUtil.drawCenteredArc(g, 2, 23, 22, 90, -53);
			JGraphicsUtil.drawCenteredArc(g, -10, 9, 16, -30, 60);
			JGraphicsUtil.drawCenteredArc(g, -12, 9, 16, -30, 60);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			PainterShaped.paintXor(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			PainterDin.paintXor(painter, width, height, false);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			object behavior = state.getAttributeValue(GateAttributes.ATTR_XOR);
			if (behavior == GateAttributes.XOR_ODD)
			{
				return GateFunctions.computeOddParity(inputs, numInputs);
			}
			else
			{
				return GateFunctions.computeExactlyOne(inputs, numInputs);
			}
		}

		protected internal override bool shouldRepairWire(Instance instance, WireRepairData data)
		{
			return !data.Point.Equals(instance.Location);
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			return xorExpression(inputs, numInputs);
		}

		protected internal override Value Identity
		{
			get
			{
				return Value.FALSE;
			}
		}

		protected internal static Expression xorExpression(Expression[] inputs, int numInputs)
		{
			if (numInputs > 2)
			{
				throw new System.NotSupportedException("XorGate");
			}
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.xor(ret, inputs[i]);
			}
			return ret;
		}
	}

}
