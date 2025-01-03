// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using Element = org.w3c.dom.Element;

	using AbstractCanvasObject = draw.model.AbstractCanvasObject;
	using SvgReader = draw.shapes.SvgReader;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;

	public class AppearanceSvgReader
	{
		public static AbstractCanvasObject createShape(Element elt, Dictionary<Location, Instance> pins)
		{
			string name = elt.getTagName();
			if (name.Equals("circ-anchor") || name.Equals("circ-origin"))
			{
				Location loc = getLocation(elt);
				AbstractCanvasObject ret = new AppearanceAnchor(loc);
				if (elt.hasAttribute("facing"))
				{
					Direction facing = Direction.parse(elt.getAttribute("facing"));
					ret.setValue(AppearanceAnchor.FACING, facing);
				}
				return ret;
			}
			else if (name.Equals("circ-port"))
			{
				Location loc = getLocation(elt);
				string[] pinStr = elt.getAttribute("pin").Split(",");
				Location pinLoc = new Location(int.Parse(pinStr[0].Trim()), int.Parse(pinStr[1].Trim()));
				Instance pin = pins[pinLoc];
				if (pin == null)
				{
					return null;
				}
				else
				{
					return new AppearancePort(loc, pin);
				}
			}
			else
			{
				return SvgReader.createShape(elt);
			}
		}

		private static Location getLocation(Element elt)
		{
			double x = double.Parse(elt.getAttribute("x"));
			double y = double.Parse(elt.getAttribute("y"));
			double w = double.Parse(elt.getAttribute("width"));
			double h = double.Parse(elt.getAttribute("height"));
			int px = (int) (long)Math.Round(x + w / 2, MidpointRounding.AwayFromZero);
			int py = (int) (long)Math.Round(y + h / 2, MidpointRounding.AwayFromZero);
			return new Location(px, py);
		}
	}

}
