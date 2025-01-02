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
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using IntegerConfigurator = logisim.tools.key.IntegerConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;

	public class BitAdder : InstanceFactory
	{
		internal static readonly Attribute<int> NUM_INPUTS = Attributes.forIntegerRange("inputs", Strings.getter("gateInputsAttr"), 1, 32);

		public BitAdder() : base("BitAdder", Strings.getter("bitAdderComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH, NUM_INPUTS}, new object[] {BitWidth.create(8), Convert.ToInt32(1)});
			KeyConfigurator = JoinedConfigurator.create(new IntegerConfigurator(NUM_INPUTS, 1, 32, 0), new BitWidthConfigurator(StdAttr.WIDTH));
			IconName = "bitadder.gif";
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			int inputs = (int)attrs.getValue(NUM_INPUTS);
			int h = Math.Max(40, 10 * inputs);
			int y = inputs < 4 ? 20 : (((inputs - 1) / 2) * 10 + 5);
			return Bounds.create(-40, -y, 40, h);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == StdAttr.WIDTH)
			{
				configurePorts(instance);
			}
			else if (attr == NUM_INPUTS)
			{
				configurePorts(instance);
				instance.recomputeBounds();
			}
		}

		private void configurePorts(Instance instance)
		{
			BitWidth inWidth = instance.getAttributeValue(StdAttr.WIDTH);
			int inputs = (int)instance.getAttributeValue(NUM_INPUTS);
			int outWidth = computeOutputBits(inWidth.Width, inputs);

			int y;
			int dy = 10;
			switch (inputs)
			{
			case 1:
				y = 0;
				break;
			case 2:
				y = -10;
				dy = 20;
				break;
			case 3:
				y = -10;
				break;
			default:
				y = ((inputs - 1) / 2) * -10;
			break;
			}

			Port[] ps = new Port[inputs + 1];
			ps[0] = new Port(0, 0, Port.OUTPUT, BitWidth.create(outWidth));
			ps[0].setToolTip(Strings.getter("bitAdderOutputManyTip"));
			for (int i = 0; i < inputs; i++)
			{
				ps[i + 1] = new Port(-40, y + i * dy, Port.INPUT, inWidth);
				ps[i + 1].setToolTip(Strings.getter("bitAdderInputTip"));
			}
			instance.setPorts(ps);
		}

		private int computeOutputBits(int width, int inputs)
		{
			int maxBits = width * inputs;
			int outWidth = 1;
			while ((1 << outWidth) <= maxBits)
			{
				outWidth++;
			}
			return outWidth;
		}

		public override void propagate(InstanceState state)
		{
			int width = state.getAttributeValue(StdAttr.WIDTH).getWidth();
			int inputs = (int)state.getAttributeValue(NUM_INPUTS);

			// compute the number of 1 bits
			int minCount = 0; // number that are definitely 1
			int maxCount = 0; // number that are definitely not 0 (incl X/Z)
			for (int i = 1; i <= inputs; i++)
			{
				Value v = state.getPort(i);
				Value[] bits = v.All;
				for (int j = 0; j < bits.Length; j++)
				{
					Value b = bits[j];
					if (b == Value.TRUE)
					{
						minCount++;
					}
					if (b != Value.FALSE)
					{
						maxCount++;
					}
				}
			}

			// compute which output bits should be error bits
			int unknownMask = 0;
			for (int i = minCount + 1; i <= maxCount; i++)
			{
				unknownMask |= (minCount ^ i);
			}

			Value[] @out = new Value[computeOutputBits(width, inputs)];
			for (int i = 0; i < @out.Length; i++)
			{
				if (((unknownMask >> i) & 1) != 0)
				{
					@out[i] = Value.ERROR;
				}
				else if (((minCount >> i) & 1) != 0)
				{
					@out[i] = Value.TRUE;
				}
				else
				{
					@out[i] = Value.FALSE;
				}
			}

			int delay = @out.Length * Adder.PER_DELAY;
			state.setPort(0, Value.create(@out), delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			painter.drawBounds();
			painter.drawPorts();

			GraphicsUtil.switchToWidth(g, 2);
			Location loc = painter.Location;
			int x = loc.X - 10;
			int y = loc.Y;
			g.drawLine(x - 2, y - 5, x - 2, y + 5);
			g.drawLine(x + 2, y - 5, x + 2, y + 5);
			g.drawLine(x - 5, y - 2, x + 5, y - 2);
			g.drawLine(x - 5, y + 2, x + 5, y + 2);
		}
	}

}
