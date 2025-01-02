// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.io
{

	using logisim.data;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using InstanceDataSingleton = logisim.instance.InstanceDataSingleton;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;

	public class SevenSegment : InstanceFactory
	{
		internal static Bounds[] SEGMENTS = null;
		internal static Color DEFAULT_OFF = new Color(220, 220, 220);

		public SevenSegment() : base("7-Segment Display", Strings.getter("sevenSegmentComponent"))
		{
			setAttributes(new Attribute[] {Io.ATTR_ON_COLOR, Io.ATTR_OFF_COLOR, Io.ATTR_BACKGROUND, Io.ATTR_ACTIVE}, new object[] {new Color(240, 0, 0), DEFAULT_OFF, Io.DEFAULT_BACKGROUND, true});
			OffsetBounds = Bounds.create(-5, 0, 40, 60);
			IconName = "7seg.gif";
			setPorts(new Port[]
			{
				new Port(20, 0, Port.INPUT, 1),
				new Port(30, 0, Port.INPUT, 1),
				new Port(20, 60, Port.INPUT, 1),
				new Port(10, 60, Port.INPUT, 1),
				new Port(0, 60, Port.INPUT, 1),
				new Port(10, 0, Port.INPUT, 1),
				new Port(0, 0, Port.INPUT, 1),
				new Port(30, 60, Port.INPUT, 1)
			});
		}

		public override void propagate(InstanceState state)
		{
			int summary = 0;
			for (int i = 0; i < 8; i++)
			{
				Value val = state.getPort(i);
				if (val == Value.TRUE)
				{
					summary |= 1 << i;
				}
			}
			object value = Convert.ToInt32(summary);
			InstanceDataSingleton data = (InstanceDataSingleton) state.Data;
			if (data == null)
			{
				state.Data = new InstanceDataSingleton(value);
			}
			else
			{
				data.Value = value;
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			drawBase(painter);
		}

		internal static void drawBase(InstancePainter painter)
		{
			ensureSegments();
			InstanceDataSingleton data = (InstanceDataSingleton) painter.Data;
			int summ = (data == null ? 0 : ((int?) data.Value).Value);
			bool? active = painter.getAttributeValue(Io.ATTR_ACTIVE);
			int desired = active == null || active.Value ? 1 : 0;

			Bounds bds = painter.Bounds;
			int x = bds.X + 5;
			int y = bds.Y;

			Graphics g = painter.Graphics;
			Color onColor = painter.getAttributeValue(Io.ATTR_ON_COLOR);
			Color offColor = painter.getAttributeValue(Io.ATTR_OFF_COLOR);
			Color bgColor = painter.getAttributeValue(Io.ATTR_BACKGROUND);
			if (painter.shouldDrawColor() && bgColor.getAlpha() != 0)
			{
				g.setColor(bgColor);
				g.fillRect(bds.X, bds.Y, bds.Width, bds.Height);
				g.setColor(Color.BLACK);
			}
			painter.drawBounds();
			g.setColor(Color.DARK_GRAY);
			for (int i = 0; i <= 7; i++)
			{
				if (painter.ShowState)
				{
					g.setColor(((summ >> i) & 1) == desired ? onColor : offColor);
				}
				if (i < 7)
				{
					Bounds seg = SEGMENTS[i];
					g.fillRect(x + seg.X, y + seg.Y, seg.Width, seg.Height);
				}
				else
				{
					g.fillOval(x + 28, y + 48, 5, 5); // draw decimal point
				}
			}
			painter.drawPorts();
		}

		internal static void ensureSegments()
		{
			if (SEGMENTS == null)
			{
				SEGMENTS = new Bounds[] {Bounds.create(3, 8, 19, 4), Bounds.create(23, 10, 4, 19), Bounds.create(23, 30, 4, 19), Bounds.create(3, 47, 19, 4), Bounds.create(-2, 30, 4, 19), Bounds.create(-2, 10, 4, 19), Bounds.create(3, 28, 19, 4)};
			}
		}
	}

}
