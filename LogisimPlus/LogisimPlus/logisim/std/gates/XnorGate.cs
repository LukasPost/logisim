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

	internal class XnorGate : AbstractGate
	{
		public static XnorGate FACTORY = new XnorGate();

		private XnorGate() : base("XNOR Gate", Strings.getter("xnorGateComponent"), true)
		{
			NegateOutput = true;
			AdditionalWidth = 10;
			setIconNames("xnorGate.gif", "xnorGateRect.gif", "dinXnorGate.gif");
			PaintInputLines = true;
		}

		protected internal override string getRectangularLabel(AttributeSet attrs)
		{
			return XorGate.FACTORY.getRectangularLabel(attrs);
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			JGraphicsUtil.drawCenteredArc(g, 0, -5, 22, -90, 53);
			JGraphicsUtil.drawCenteredArc(g, 0, 23, 22, 90, -53);
			JGraphicsUtil.drawCenteredArc(g, -8, 9, 16, -30, 60);
			JGraphicsUtil.drawCenteredArc(g, -10, 9, 16, -30, 60);
			g.drawOval(16, 8, 4, 4);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			PainterShaped.paintXor(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			PainterDin.paintXnor(painter, width, height, false);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			object behavior = state.getAttributeValue(GateAttributes.ATTR_XOR);
			if (behavior == GateAttributes.XOR_ODD)
			{
				return GateFunctions.computeOddParity(inputs, numInputs).not();
			}
			else
			{
				return GateFunctions.computeExactlyOne(inputs, numInputs).not();
			}
		}

		protected internal override bool shouldRepairWire(Instance instance, WireRepairData data)
		{
			return !data.Point.Equals(instance.Location);
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			return Expressions.not(XorGate.xorExpression(inputs, numInputs));
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
