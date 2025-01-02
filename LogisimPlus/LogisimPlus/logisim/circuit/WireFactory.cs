// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Component = logisim.comp.Component;
	using AbstractComponentFactory = logisim.comp.AbstractComponentFactory;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringGetter = logisim.util.StringGetter;

	internal class WireFactory : AbstractComponentFactory
	{
		public static readonly WireFactory instance = new WireFactory();

		private WireFactory()
		{
		}

		public override string Name
		{
			get
			{
				return "Wire";
			}
		}

		public override StringGetter DisplayGetter
		{
			get
			{
				return Strings.getter("wireComponent");
			}
		}

		public override AttributeSet createAttributeSet()
		{
			return Wire.create(new Location(0, 0), new Location(100, 0));
		}

		public override Component createComponent(Location loc, AttributeSet attrs)
		{
			object dir = attrs.getValue(Wire.dir_attr);
			int len = (int)attrs.getValue(Wire.len_attr);

			if (dir == Wire.VALUE_HORZ)
			{
				return Wire.create(loc, loc.translate(len, 0));
			}
			else
			{
				return Wire.create(loc, loc.translate(0, len));
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			object dir = attrs.getValue(Wire.dir_attr);
			int len = (int)attrs.getValue(Wire.len_attr);

			if (dir == Wire.VALUE_HORZ)
			{
				return Bounds.create(0, -2, len, 5);
			}
			else
			{
				return Bounds.create(-2, 0, 5, len);
			}
		}

		//
		// user interface methods
		//
		public override void drawGhost(ComponentDrawContext context, Color color, int x, int y, AttributeSet attrs)
		{
			Graphics g = context.Graphics;
			object dir = attrs.getValue(Wire.dir_attr);
			int len = (int)attrs.getValue(Wire.len_attr);

			g.setColor(color);
			GraphicsUtil.switchToWidth(g, 3);
			if (dir == Wire.VALUE_HORZ)
			{
				g.drawLine(x, y, x + len, y);
			}
			else
			{
				g.drawLine(x, y, x, y + len);
			}
		}
	}

}
