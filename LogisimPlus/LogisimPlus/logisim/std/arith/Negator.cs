// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.arith
{
	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;

	public class Negator : InstanceFactory
	{
		private const int IN = 0;
		private const int OUT = 1;

		public Negator() : base("Negator", Strings.getter("negatorComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH}, new object[] {BitWidth.create(8)});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "negator.gif";

			Port[] ps = new Port[2];
			ps[IN] = new Port(-40, 0, Port.INPUT, StdAttr.WIDTH);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[IN].setToolTip(Strings.getter("negatorInputTip"));
			ps[OUT].setToolTip(Strings.getter("negatorOutputTip"));
			setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// get attributes
			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);

			// compute outputs
			Value @in = state.getPort(IN);
			Value @out;
			if (@in.FullyDefined)
			{
				@out = Value.createKnown(@in.BitWidth, -@in.toIntValue());
			}
			else
			{
				Value[] bits = @in.All;
				Value fill = Value.FALSE;
				int pos = 0;
				while (pos < bits.Length)
				{
					if (bits[pos] == Value.FALSE)
					{
						bits[pos] = fill;
					}
					else if (bits[pos] == Value.TRUE)
					{
						if (fill != Value.FALSE)
						{
							bits[pos] = fill;
						}
						pos++;
						break;
					}
					else if (bits[pos] == Value.ERROR)
					{
						fill = Value.ERROR;
					}
					else
					{
						if (fill == Value.FALSE)
						{
							fill = bits[pos];
						}
						else
						{
							bits[pos] = fill;
						}
					}
					pos++;
				}
				while (pos < bits.Length)
				{
					if (bits[pos] == Value.TRUE)
					{
						bits[pos] = Value.FALSE;
					}
					else if (bits[pos] == Value.FALSE)
					{
						bits[pos] = Value.TRUE;
					}
					pos++;
				}
				@out = Value.create(bits);
			}

			// propagate them
			int delay = (dataWidth.Width + 2) * Adder.PER_DELAY;
			state.setPort(OUT, @out, delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			painter.drawBounds();
			painter.drawPort(IN);
			painter.drawPort(OUT, "-x", Direction.West);
		}
	}

}
