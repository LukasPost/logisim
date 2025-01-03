// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.arith
{
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;

	public class Comparator : InstanceFactory
	{
		private static readonly AttributeOption SIGNED_OPTION = new AttributeOption("twosComplement", "twosComplement", Strings.getter("twosComplementOption"));
		private static readonly AttributeOption UNSIGNED_OPTION = new AttributeOption("unsigned", "unsigned", Strings.getter("unsignedOption"));
		private static readonly Attribute MODE_ATTRIBUTE = Attributes.forOption("mode", Strings.getter("comparatorType"), new AttributeOption[] {SIGNED_OPTION, UNSIGNED_OPTION});

		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int GT = 2;
		private const int EQ = 3;
		private const int LT = 4;

		public Comparator() : base("Comparator", Strings.getter("comparatorComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.Width, MODE_ATTRIBUTE}, new object[] {BitWidth.create(8), SIGNED_OPTION});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "comparator.gif";

			Port[] ps = new Port[5];
			ps[IN0] = new Port(-40, -10, Port.INPUT, StdAttr.Width);
			ps[IN1] = new Port(-40, 10, Port.INPUT, StdAttr.Width);
			ps[GT] = new Port(0, -10, Port.OUTPUT, 1);
			ps[EQ] = new Port(0, 0, Port.OUTPUT, 1);
			ps[LT] = new Port(0, 10, Port.OUTPUT, 1);
			ps[IN0].setToolTip(Strings.getter("comparatorInputATip"));
			ps[IN1].setToolTip(Strings.getter("comparatorInputBTip"));
			ps[GT].setToolTip(Strings.getter("comparatorGreaterTip"));
			ps[EQ].setToolTip(Strings.getter("comparatorEqualTip"));
			ps[LT].setToolTip(Strings.getter("comparatorLessTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth dataWidth = state.getAttributeValue(StdAttr.Width);

			// compute outputs
			Value gt = Value.FALSE;
			Value eq = Value.TRUE;
			Value lt = Value.FALSE;

			Value a = state.getPort(IN0);
			Value b = state.getPort(IN1);
			Value[] ax = a.All;
			Value[] bx = b.All;
			int maxlen = Math.Max(ax.Length, bx.Length);
			for (int pos = maxlen - 1; pos >= 0; pos--)
			{
				Value ab = pos < ax.Length ? ax[pos] : Value.ERROR;
				Value bb = pos < bx.Length ? bx[pos] : Value.ERROR;
				if (pos == ax.Length - 1 && ab != bb)
				{
					object mode = state.getAttributeValue(MODE_ATTRIBUTE);
					if (mode != UNSIGNED_OPTION)
					{
						Value t = ab;
						ab = bb;
						bb = t;
					}
				}

				if (ab == Value.ERROR || bb == Value.ERROR)
				{
					gt = Value.ERROR;
					eq = Value.ERROR;
					lt = Value.ERROR;
					break;
				}
				else if (ab == Value.UNKNOWN || bb == Value.UNKNOWN)
				{
					gt = Value.UNKNOWN;
					eq = Value.UNKNOWN;
					lt = Value.UNKNOWN;
					break;
				}
				else if (ab != bb)
				{
					eq = Value.FALSE;
					if (ab == Value.TRUE)
					{
						gt = Value.TRUE;
					}
					else
					{
						lt = Value.TRUE;
					}
					break;
				}
			}

			// propagate them
			int delay = (dataWidth.Width + 2) * Adder.PER_DELAY;
			state.setPort(GT, gt, delay);
			state.setPort(EQ, eq, delay);
			state.setPort(LT, lt, delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			painter.drawBounds();
			painter.drawPort(IN0);
			painter.drawPort(IN1);
			painter.drawPort(GT, ">", Direction.West);
			painter.drawPort(EQ, "=", Direction.West);
			painter.drawPort(LT, "<", Direction.West);
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			instance.fireInvalidated();
		}
	}

}
