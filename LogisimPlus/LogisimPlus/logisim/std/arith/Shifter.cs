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
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;

	public class Shifter : InstanceFactory
	{
		internal static readonly AttributeOption SHIFT_LOGICAL_LEFT = new AttributeOption("ll", Strings.getter("shiftLogicalLeft"));
		internal static readonly AttributeOption SHIFT_LOGICAL_RIGHT = new AttributeOption("lr", Strings.getter("shiftLogicalRight"));
		internal static readonly AttributeOption SHIFT_ARITHMETIC_RIGHT = new AttributeOption("ar", Strings.getter("shiftArithmeticRight"));
		internal static readonly AttributeOption SHIFT_ROLL_LEFT = new AttributeOption("rl", Strings.getter("shiftRollLeft"));
		internal static readonly AttributeOption SHIFT_ROLL_RIGHT = new AttributeOption("rr", Strings.getter("shiftRollRight"));
		internal static readonly Attribute<AttributeOption> ATTR_SHIFT = Attributes.forOption("shift", Strings.getter("shifterShiftAttr"), new AttributeOption[] {SHIFT_LOGICAL_LEFT, SHIFT_LOGICAL_RIGHT, SHIFT_ARITHMETIC_RIGHT, SHIFT_ROLL_LEFT, SHIFT_ROLL_RIGHT});

		private const int IN0 = 0;
		private const int IN1 = 1;
		private const int OUT = 2;

		public Shifter() : base("Shifter", Strings.getter("shifterComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH, ATTR_SHIFT}, new object[] {BitWidth.create(8), SHIFT_LOGICAL_LEFT});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);
			OffsetBounds = Bounds.create(-40, -20, 40, 40);
			IconName = "shifter.gif";
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
		}

		private void configurePorts(Instance instance)
		{
			BitWidth dataWid = instance.getAttributeValue(StdAttr.WIDTH);
			int data = dataWid == null ? 32 : dataWid.Width;
			int shift = 1;
			while ((1 << shift) < data)
			{
				shift++;
			}

			Port[] ps = new Port[3];
			ps[IN0] = new Port(-40, -10, Port.INPUT, data);
			ps[IN1] = new Port(-40, 10, Port.INPUT, shift);
			ps[OUT] = new Port(0, 0, Port.OUTPUT, data);
			ps[IN0].setToolTip(Strings.getter("shifterInputTip"));
			ps[IN1].setToolTip(Strings.getter("shifterDistanceTip"));
			ps[OUT].setToolTip(Strings.getter("shifterOutputTip"));
			instance.setPorts(ps);
		}

		public override void propagate(InstanceState state)
		{
			// compute output
			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
			int bits = dataWidth.Width;
			Value vx = state.getPort(IN0);
			Value vd = state.getPort(IN1);
			Value vy; // y will by x shifted by d
			if (vd.FullyDefined && vx.Width == bits)
			{
				int d = vd.toIntValue();
				object shift = state.getAttributeValue(ATTR_SHIFT);
				if (d == 0)
				{
					vy = vx;
				}
				else if (vx.FullyDefined)
				{
					int x = vx.toIntValue();
					int y;
					if (shift == SHIFT_LOGICAL_RIGHT)
					{
						y = x >>> d;
					}
					else if (shift == SHIFT_ARITHMETIC_RIGHT)
					{
						if (d >= bits)
						{
							d = bits - 1;
						}
						y = x >> d | ((x << (32 - bits)) >> (32 - bits + d));
					}
					else if (shift == SHIFT_ROLL_RIGHT)
					{
						if (d >= bits)
						{
							d -= bits;
						}
						y = (x >>> d) | (x << (bits - d));
					}
					else if (shift == SHIFT_ROLL_LEFT)
					{
						if (d >= bits)
						{
							d -= bits;
						}
						y = (x << d) | (x >>> (bits - d));
					}
					else
					{ // SHIFT_LOGICAL_LEFT
						y = x << d;
					}
					vy = Value.createKnown(dataWidth, y);
				}
				else
				{
					Value[] x = vx.All;
					Value[] y = new Value[bits];
					if (shift == SHIFT_LOGICAL_RIGHT)
					{
						if (d >= bits)
						{
							d = bits;
						}
						Array.Copy(x, d, y, 0, bits - d);
						Arrays.Fill(y, bits - d, bits, Value.FALSE);
					}
					else if (shift == SHIFT_ARITHMETIC_RIGHT)
					{
						if (d >= bits)
						{
							d = bits;
						}
						Array.Copy(x, d, y, 0, x.Length - d);
						Arrays.Fill(y, bits - d, y.Length, x[bits - 1]);
					}
					else if (shift == SHIFT_ROLL_RIGHT)
					{
						if (d >= bits)
						{
							d -= bits;
						}
						Array.Copy(x, d, y, 0, bits - d);
						Array.Copy(x, 0, y, bits - d, d);
					}
					else if (shift == SHIFT_ROLL_LEFT)
					{
						if (d >= bits)
						{
							d -= bits;
						}
						Array.Copy(x, x.Length - d, y, 0, d);
						Array.Copy(x, 0, y, d, bits - d);
					}
					else
					{ // SHIFT_LOGICAL_LEFT
						if (d >= bits)
						{
							d = bits;
						}
						Arrays.Fill(y, 0, d, Value.FALSE);
						Array.Copy(x, 0, y, d, bits - d);
					}
					vy = Value.create(y);
				}
			}
			else
			{
				vy = Value.createError(dataWidth);
			}

			// propagate them
			int delay = dataWidth.Width * (3 * Adder.PER_DELAY);
			state.setPort(OUT, vy, delay);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			painter.drawBounds();

			painter.drawPorts();

			Location loc = painter.Location;
			int x = loc.X - 15;
			int y = loc.Y;
			object shift = painter.getAttributeValue(ATTR_SHIFT);
			g.setColor(Color.BLACK);
			if (shift == SHIFT_LOGICAL_RIGHT)
			{
				g.fillRect(x, y - 1, 8, 3);
				drawArrow(g, x + 10, y, -4);
			}
			else if (shift == SHIFT_ARITHMETIC_RIGHT)
			{
				g.fillRect(x, y - 1, 2, 3);
				g.fillRect(x + 3, y - 1, 5, 3);
				drawArrow(g, x + 10, y, -4);
			}
			else if (shift == SHIFT_ROLL_RIGHT)
			{
				g.fillRect(x, y - 1, 5, 3);
				g.fillRect(x + 8, y - 7, 2, 8);
				g.fillRect(x, y - 7, 2, 8);
				g.fillRect(x, y - 7, 10, 2);
				drawArrow(g, x + 8, y, -4);
			}
			else if (shift == SHIFT_ROLL_LEFT)
			{
				g.fillRect(x + 6, y - 1, 4, 3);
				g.fillRect(x + 8, y - 7, 2, 8);
				g.fillRect(x, y - 7, 2, 8);
				g.fillRect(x, y - 7, 10, 2);
				drawArrow(g, x + 3, y, 4);
			}
			else
			{ // SHIFT_LOGICAL_LEFT
				g.fillRect(x + 2, y - 1, 8, 3);
				drawArrow(g, x, y, 4);
			}
		}

		private void drawArrow(Graphics g, int x, int y, int d)
		{
			int[] px = new int[] {x + d, x, x + d};
			int[] py = new int[] {y + d, y, y - d};
			g.fillPolygon(px, py, 3);
		}
	}

}
