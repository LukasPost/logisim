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
	using Instance = logisim.instance.Instance;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using WireRepairData = logisim.tools.WireRepairData;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	internal class NorGate : AbstractGate
	{
		public static NorGate FACTORY = new NorGate();

		private NorGate() : base("NOR Gate", Strings.getter("norGateComponent"))
		{
			NegateOutput = true;
			RectangularLabel = OrGate.FACTORY.getRectangularLabel(null);
			setIconNames("norGate.gif", "norGateRect.gif", "dinNorGate.gif");
			PaintInputLines = true;
		}

		public override void paintIconShaped(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			GraphicsUtil.drawCenteredArc(g, 0, -5, 22, -90, 53);
			GraphicsUtil.drawCenteredArc(g, 0, 23, 22, 90, -53);
			GraphicsUtil.drawCenteredArc(g, -12, 9, 16, -30, 60);
			g.drawOval(16, 8, 4, 4);
		}

		protected internal override void paintShape(InstancePainter painter, int width, int height)
		{
			PainterShaped.paintOr(painter, width, height);
		}

		protected internal override void paintDinShape(InstancePainter painter, int width, int height, int inputs)
		{
			PainterDin.paintOr(painter, width, height, true);
		}

		protected internal override Value computeOutput(Value[] inputs, int numInputs, InstanceState state)
		{
			return GateFunctions.computeOr(inputs, numInputs).not();
		}

		protected internal override bool shouldRepairWire(Instance instance, WireRepairData data)
		{
			return !data.Point.Equals(instance.Location);
		}

		protected internal override Expression computeExpression(Expression[] inputs, int numInputs)
		{
			Expression ret = inputs[0];
			for (int i = 1; i < numInputs; i++)
			{
				ret = Expressions.or(ret, inputs[i]);
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
