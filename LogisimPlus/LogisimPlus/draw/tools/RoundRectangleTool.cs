// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.tools
{

	using CanvasObject = draw.model.CanvasObject;
	using DrawAttr = draw.shapes.DrawAttr;
	using RoundRectangle = draw.shapes.RoundRectangle;
	using logisim.data;
	using Icons = logisim.util.Icons;

	public class RoundRectangleTool : RectangularTool
	{
		private DrawingAttributeSet attrs;

		public RoundRectangleTool(DrawingAttributeSet attrs)
		{
			this.attrs = attrs;
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("drawrrct.gif");
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override IList<Attribute<object>> Attributes
		{
			get
			{
				return DrawAttr.getRoundRectAttributes(attrs.getValue(DrawAttr.PAINT_TYPE));
			}
		}

		public override CanvasObject createShape(int x, int y, int w, int h)
		{
			return attrs.applyTo(new RoundRectangle(x, y, w, h));
		}

		public override void drawShape(Graphics g, int x, int y, int w, int h)
		{
			int r = 2 * (int)attrs.getValue(DrawAttr.CORNER_RADIUS);
			g.drawRoundRect(x, y, w, h, r, r);
		}

		public override void fillShape(Graphics g, int x, int y, int w, int h)
		{
			int r = 2 * (int)attrs.getValue(DrawAttr.CORNER_RADIUS);
			g.fillRoundRect(x, y, w, h, r, r);
		}
	}

}
