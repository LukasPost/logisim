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
	using Oval = draw.shapes.Oval;
	using logisim.data;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    public class OvalTool : RectangularTool
	{
		private DrawingAttributeSet attrs;

		public OvalTool(DrawingAttributeSet attrs)
		{
			this.attrs = attrs;
		}

		public override Icon Icon
		{
			get
			{
				return Icons.getIcon("drawoval.gif");
			}
		}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public java.util.List<logisim.data.Attribute<?>> getAttributes()
		public override List<Attribute> Attributes
		{
			get
			{
				return DrawAttr.getFillAttributes((AttributeOption)attrs.getValue(DrawAttr.PAINT_TYPE));
			}
		}

		public override CanvasObject createShape(int x, int y, int w, int h)
		{
			return attrs.applyTo(new Oval(x, y, w, h));
		}

		public override void drawShape(JGraphics g, int x, int y, int w, int h)
		{
			g.drawOval(x, y, w, h);
		}

		public override void fillShape(JGraphics g, int x, int y, int w, int h)
		{
			g.fillOval(x, y, w, h);
		}
	}

}
