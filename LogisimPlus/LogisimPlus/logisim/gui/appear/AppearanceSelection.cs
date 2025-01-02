// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{

	using Selection = draw.canvas.Selection;
	using CanvasObject = draw.model.CanvasObject;
	using AppearanceElement = logisim.circuit.appear.AppearanceElement;

	public class AppearanceSelection : Selection
	{
		public override void setMovingShapes<T1>(ICollection<T1> shapes, int dx, int dy) where T1 : draw.model.CanvasObject
		{
			if (shouldSnap(shapes))
			{
				dx = (dx + 5) / 10 * 10;
				dy = (dy + 5) / 10 * 10;
			}
			base.setMovingShapes(shapes, dx, dy);
		}

		public override void setMovingDelta(int dx, int dy)
		{
			if (shouldSnap(Selected))
			{
				dx = (dx + 5) / 10 * 10;
				dy = (dy + 5) / 10 * 10;
			}
			base.setMovingDelta(dx, dy);
		}

		private bool shouldSnap<T1>(ICollection<T1> shapes) where T1 : draw.model.CanvasObject
		{
			foreach (CanvasObject o in shapes)
			{
				if (o is AppearanceElement)
				{
					return true;
				}
			}
			return false;
		}
	}

}
