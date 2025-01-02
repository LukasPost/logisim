// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using Location = logisim.data.Location;

	internal class WireIterator : IEnumerator<Location>
	{
		private int curX;
		private int curY;
		private int destX;
		private int destY;
		private int deltaX;
		private int deltaY;
		private bool destReturned;

		public WireIterator(Location e0, Location e1)
		{
			curX = e0.X;
			curY = e0.Y;
			destX = e1.X;
			destY = e1.Y;
			destReturned = false;
			if (curX < destX)
			{
				deltaX = 10;
			}
			else if (curX > destX)
			{
				deltaX = -10;
			}
			else
			{
				deltaX = 0;
			}
			if (curY < destY)
			{
				deltaY = 10;
			}
			else if (curY > destY)
			{
				deltaY = -10;
			}
			else
			{
				deltaY = 0;
			}

			int offX = (destX - curX) % 10;
			if (offX != 0)
			{ // should not happen, but in case it does...
				destX = curX + deltaX * ((destX - curX) / 10);
			}
			int offY = (destY - curY) % 10;
			if (offY != 0)
			{ // should not happen, but in case it does...
				destY = curY + deltaY * ((destY - curY) / 10);
			}
		}

		public virtual bool hasNext()
		{
			return !destReturned;
		}

		public virtual Location next()
		{
			Location ret = new Location(curX, curY);
			destReturned |= curX == destX && curY == destY;
			curX += deltaX;
			curY += deltaY;
			return ret;
		}

		public virtual void remove()
		{
			throw new System.NotSupportedException();
		}
	}

}
