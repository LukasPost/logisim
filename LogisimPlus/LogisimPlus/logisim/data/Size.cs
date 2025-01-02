// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.data
{
	/// <summary>
	/// Represents the Sizes of a rectangle. This is analogous to java.awt's <code>Size</code> class, except that
	/// objects of this type are immutable.
	/// </summary>
	public class Size
	{
		private readonly int width;
		private readonly int height;

		public Size(int wid, int ht)
		{
			this.width = wid;
			this.height = ht;
		}

		public override bool Equals(object other_obj)
		{
			return other_obj is Size other && width == other.width && height == other.height;
		}

		public override string ToString()
		{
			return width + "x" + height;
		}

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public virtual int Height
		{
			get
			{
				return height;
			}
		}

		public virtual java.awt.Size toAwtSize()
		{
			return new java.awt.Size(width, height);
		}

		public virtual bool contains(Location p)
		{
			return contains(p.X, p.Y);
		}

		public virtual bool contains(int x, int y)
		{
			return x >= 0 && y >= 0 && x < this.width && y < this.height;
		}

		public virtual bool contains(int x, int y, int wid, int ht)
		{
			int oth_x = (wid <= 0 ? x : x + wid - 1);
			int oth_y = (ht <= 0 ? y : y + wid - 1);
			return contains(x, y) && contains(oth_x, oth_y);
		}

		public virtual bool contains(Size bd)
		{
			return contains(bd.width, bd.height);
		}

	}

}
